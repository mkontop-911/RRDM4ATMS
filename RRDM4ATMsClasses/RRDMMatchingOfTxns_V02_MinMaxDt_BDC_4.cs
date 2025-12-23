using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;

using System.Threading.Tasks;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4 : Logger
    {
        public RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4() : base() { }
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
        public decimal TransAmt;
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

        // Define the data table 
        public DataTable TableUnMatchedCompressed = new DataTable();

        public int TotalSelected2;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        bool ShowMessage;

        // Define the data table 

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);
        readonly DateTime MaxFutureDate = new DateTime(2050, 12, 31);

        string SqlString; // Do not delete 

        readonly string connectionStringATMs = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;
        readonly string connectionStringRec = ConfigurationManager.ConnectionStrings
            ["ReconConnectionString"].ConnectionString;

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
        public int Type;
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

        bool FromOwnATMs;

        //bool TestingTwoFiles; 

        string WMatchingCateg;
        int WRMCycle;
        int WGroupId;
        string WReconcCategoryId;
        string WSignedId;
        string WOperator;
        bool WTestingWorkingFiles;

        //
        // Make Matching with Categories that are ready to be matched
        // 
        public void MatchReadyCategoriesUpdate(string InOperator, string InSignedId,
                                       int InRMCycle)
        {
            CategForMatch = "";

            WOperator = InOperator;
            // THIS IS FOR BANK De Caire where journal and Ist and Flecube have same date

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }

            EqualDatesForATMS = true;

            ShowMessage = false;

            Mc.ReadMatchingCategoriesAndFillTable(WOperator, "ALL");

            WMatchingCateg = "";

            bool ReadyCat;

            // LOOP FOR Matching Categories

            int I = 0;

            while (I <= (Mc.TableMatchingCateg.Rows.Count - 1))
            {
                // Do 

                int WCat_SeqNo = (int)Mc.TableMatchingCateg.Rows[I]["SeqNo"];

                Mc.ReadMatchingCategorybySeqNoActive(WOperator, WCat_SeqNo);

                WMatchingCateg = Mc.CategoryId;
                POS_Type = Mc.Pos_Type;
                UnMatchedForWorkingDays = Mc.UnMatchedForWorkingDays;
                UnMatchedForCalendarDays = Mc.UnMatchedForCalendarDays;
                WOrigin = Mc.Origin;

                ReadyCat = false;

                ////
                // ALLOW THIS
                //

                //if (WMatchingCateg == PRX + "272"
                // || WMatchingCateg == PRX + "273"
                ////   //    //        || WMatchingCateg == PRX + "215"// 123
                ////   //    //        || WMatchingCateg == PRX + "230"
                ////   //    //        || WMatchingCateg == PRX + "231"
                ////   //    //        || WMatchingCateg == PRX + "232"
                ////   //    //        || WMatchingCateg == PRX + "233"
                ////   //    //        || WMatchingCateg == PRX + "234"
                ////   //    //        || WMatchingCateg == PRX + "235"
                ////   //    //|| WMatchingCateg == PRX+"240" //  Master Card BDC ATMs
                ////   //    //WMatchingCateg == PRX+"215" //  123 Net BDC ATMs
                ////   //    // WMatchingCateg == PRX+"225" // Visa BDC ATMs

                ////   //    //|| WMatchingCateg == PRX + "215" // Credit Card BDC ATMs

                //   )
                //{

                //    Message = "THIS THE ONE - " + WMatchingCateg + " POS  ";
                //    System.Windows.Forms.MessageBox.Show(Message);
                //    // CONTINUE TO MATCHING 
                //}
                //else
                //{
                //    I++;
                //    continue;
                //}

                //ALLOW ONLY THIS
                // RRDM-PANICOS
                //if (Environment.MachineName == "RRDM_ABE")
                //{
                //    if (WMatchingCateg == PRX + "101"   // 
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



                //****************
                // POS TYPE = File distributed to many dates 
                //****************
                //if (
                //       // INCOMING 
                //       WMatchingCateg == PRX+"230" // Master Incoming Debit  
                //    || WMatchingCateg == PRX+"231" // Master incoming POS
                //    || WMatchingCateg == PRX+"232" // Master incoming Prepaid
                //    || WMatchingCateg == PRX+"210" // 123 NET
                //    || WMatchingCateg == PRX+"211" // 123 NET - Prepaid
                //                                  // AND SOME OUTGOING
                //                                  // THIS WAS IN ANOTHER VERSION
                //    || WMatchingCateg == PRX+"215" // 123 Net Other Cards used our ATMs 
                //    || WMatchingCateg == PRX+"225" // Visa BDC ATMs
                //    || WMatchingCateg == PRX+"235" // Master Card BDC ATMs
                //    || WMatchingCateg == PRX+"240" // Credit Card BDC ATMs
                //    )
                //{
                //    // Incoming File is distributed to many dates 
                //    POS_Type = true;
                //}
                //else
                //{
                //    POS_Type = false;
                //}
                //
                ////************************************
                //// DOES IT FOLLOWS the Groups of ATMS? 
                //// ONLY BDC201 Which ATMs to Flexcube that has Groups
                //if (
                //      WMatchingCateg == PRX+"202" // NO POS
                //   || WMatchingCateg == PRX+"203" // NO POS
                //   || WMatchingCateg == PRX+"204" // But POS Type - Credit 
                //   || WMatchingCateg == PRX+"205" // NO POS
                //   || WMatchingCateg == PRX+"206" // But POS Type - Master outgoing
                //   || WMatchingCateg == PRX+"207" // But POS Type - VISA outgoing 
                //   )
                //{
                //    // Incoming File is distributed to many dates 
                //    No_Group_Type = true;
                //}
                //else
                //{
                //    No_Group_Type = false;
                //    // ONLY BDC201 is based on Groups
                //}

                ////*************
                //// Start from three FILE TO MAKE MATCHING
                //// ATMs=>IST=>MASTER Card ... since is POS TYPE you stART FROM THIRD FILE
                ////*************
                //if (WMatchingCateg == PRX+"204" // Credit Card Outgoing
                //    || WMatchingCateg == PRX+"206" // Master Card ougoing 
                //         || WMatchingCateg == PRX+"207" // Visa Outgoing
                //         )
                //{
                //    // Matching must be done like POS but starting from Three file. 
                //    StartFromThree = true;
                //}
                //else
                //{
                //    StartFromThree = false;

                //}

                //// ANY EXTERNAL TO THE BANK HAS TO DO WITH RRN - AND The Credit is included 
                //if (WMatchingCateg == PRX+"204" // Credit card
                //         || WMatchingCateg == PRX+"205" // 123
                //             || WMatchingCateg == PRX+"206" // Master 
                //              || WMatchingCateg == PRX+"207" // Visa
                //            //   || WMatchingCateg == PRX+"208" 
                //            //    || WMatchingCateg == PRX+"209"
                //             )
                //{
                //    // MEANS THAT SECOND AND THIRD FILE IS BASED ON RRN
                //    TwoAndThreeIsRRN = true;
                //}
                //else
                //{
                //    TwoAndThreeIsRRN = false;
                //}

                //******************
                // Has adjustments from the original txn
                // THESE ARE THE POS FROM MASTER
                //******************
                //if (WMatchingCateg == PRX+"231")
                //{
                //    // Incoming File is distributed to many dates 
                //    Has_Adjustments = true;
                //}
                //else
                //{
                //    Has_Adjustments = false;
                //}

                SelectionCriteria = " CategoryId ='" + WMatchingCateg + "'";

                Mcsf.ReadReconcCategoriesVsSourcebyCategory(WMatchingCateg);

                // if (WMatchingCateg == PRX+"231") Mcsf.TotalOne = 0;
                //if (WMatchingCateg == PRX+"206") Mcsf.TotalOne = 0;
                //if (WMatchingCateg == PRX+"207") Mcsf.TotalOne = 0;
                ////if (WMatchingCateg == PRX+"203") Mcsf.TotalOne = 0;
                //if (WMatchingCateg == PRX+"211") Mcsf.TotalOne = 0;

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

                    if (ShowMessage == true & Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }



                    //******+++++++++++++++++++++++++++*******************************

                    // Update Journal with -1 and DateTime = now

                    if (WOrigin == "Our Atms")
                    {
                        Mcsf.UpdateReconcCategoryVsSourceRecordProcessCodeForSourceFileName("Atms_Journals_Txns", -1);
                    }


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

                    //if (WMatchingCateg.Substring(0, 3) == PRX+"")
                    //{
                    //    Task.Factory.StartNew(() =>
                    //    {
                    //        MessageBox.Show("Matching Starts For Category:" + WMatchingCateg + Environment.NewLine
                    //                        + "Starts At :" + DateTime.Now
                    //                        );

                    //    });
                    //}


                    Message = "Matching Starts";
                    // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                    //**********************
                    BeforeCallDtTime = DateTime.Now;

                    W_MPComment += DateTime.Now + "_" + "Matching Starts Category.. " + WMatchingCateg + Environment.NewLine;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WMatchingCateg, InRMCycle, W_MPComment);

                    //**********************

                    bool TestingWorkingFiles = false;

                    Matching_FindGroupsForThisMatchingCategoryAndProcessPerGroup(WOperator, InSignedId, WMatchingCateg,
                                                                                                 InRMCycle, TestingWorkingFiles);

                    //TableThisCategoryGroups
                    // Initialise for next

                    // Message = "Matching Finishes";
                    // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, BeforeCallDtTime = DateTime.Now;, DateTime.Now, Message);

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

                    //if (WMatchingCateg.Substring(0, 3) == PRX+"")
                    //{
                    //    Task.Factory.StartNew(() =>
                    //    {
                    //        MessageBox.Show("Matching Finishes For Category:" + WMatchingCateg + Environment.NewLine
                    //                        + "Finishes At :" + DateTime.Now + Environment.NewLine
                    //                        //+ "Discrepancies ...=.." + NumberOfUnmatchedForCategory.ToString()
                    //                        );

                    //    });
                    //}

                }
                #endregion

                I++; // Read Next entry of the table ... Next Category 
            }
        }

        public int ENQ_NumberOfCatToBeMatched;
        public string ENQ_CategForMatch;
        public string ENQ_MatchedCateg;

        public void MatchReadyCategoriesEnquiry(string InOperator, string InSignedId,
                                     int InRMCycle)
        {

            CategForMatch = "";

            WOperator = InOperator;

            ShowMessage = false;

            Mc.ReadMatchingCategoriesAndFillTable(WOperator, "ALL");

            string WMatchingCateg;
            bool ReadyCat;
            W_MPComment = "CATEGORIES MATCHING STATUS" + "\r\n" + "__________" + "\r\n";

            // LOOP FOR Matching Categories

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
                // Dont include the not ready 
                //if (ReadyCat == false)
                //{
                //    W_MPComment = W_MPComment + WMatchingCateg + ".....NON Ready For Matching" + "\r\n";
                //}


                I++; // Read Next entry of the table ... Next Category 
            }
        }


        // For this Category Find Groups and Call to Create Working Files
        // For each atm in each group 
        public void Matching_FindGroupsForThisMatchingCategoryAndProcessPerGroup
                                      (string InOperator, string InSignedId,
                                       string InMatchingCategoryId,
                                       int InRMCycle, bool InTestingWorkingFiles)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WMatchingCateg = InMatchingCategoryId;
            WRMCycle = InRMCycle;
            WTestingWorkingFiles = InTestingWorkingFiles;

            ShowMessage = true;

            string text = "In Progress Matching For Category.." + WMatchingCateg;
            string caption = "MATCHING PROCESS";
            int timeout = 2000;
            if (ShowMessage == true & Environment.UserInteractive)
            {
                AutoClosingMessageBox.Show(text, caption, timeout);
            }


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

            int TempCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            WCut_Off_Date = Rjc.Cut_Off_Date;

            #region Assign Files as defined in Matching Category

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WMatchingCateg);

            WOrigin = Mc.Origin;

            if (WOrigin == "Our Atms")
            {
                FromOwnATMs = true;

                if (Mc.ReconcMaster == true)
                {

                    No_Group_Type = true;
                }
                else
                {
                    No_Group_Type = false;
                }
            }
            else
            {
                FromOwnATMs = false;
            }


            Mcsf.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);
            NumberOfTables = Mcsf.TotalRecords;

            if (NumberOfTables > 3)
            {
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    MessageBox.Show("Category.." + WMatchingCateg + Environment.NewLine
                                                 + "Has more than three files to matched" + Environment.NewLine
                                                 + "ASK RRDM to set up the parameters for this job.." + Environment.NewLine
                                                      );
                }

                return;
            }

            if (NumberOfTables == 3 & FromOwnATMs == true)
            {
                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_A_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_B_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameC);
                SourceTable_C = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_C_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";

            }

            if (NumberOfTables == 3 & FromOwnATMs == false)
            {
                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
                //SourceTable_A_Name_Only = Msourcef.InportTableName;

                // Move Records from A to Master Pool

                Mgt.ReadUniversalAndCreateMasterPoolRecords_NEW(WOperator, SourceTable_A, WMatchingCateg, WRMCycle);

                // Change SourceTable_A to master Pool 

                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";
                SourceTable_A_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_B_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameC);
                SourceTable_C = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_C_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";

            }

            if (NumberOfTables == 2 & FromOwnATMs == true)
            {

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_A_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_B_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";

            }

            if (NumberOfTables == 2 & FromOwnATMs == false)
            {
                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                // Move Records from A to Master Pool

                Mgt.ReadUniversalAndCreateMasterPoolRecords_NEW(WOperator, SourceTable_A, WMatchingCateg, WRMCycle);


                // Change SourceTable_A to master Pool 

                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";
                SourceTable_A_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_B_Matched = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Msourcef.InportTableName + "]";
            }

            #endregion

            #region Make Matching For All Groups

            #region Read And Find All Groups of this Category 
            // Make Loop for all Groups of this category

            // Read All Groups that have this Matching category 
            // Fill Up a Table 

            NumberOfUnmatchedForCategory = 0;


            if (FromOwnATMs == true)
            {
                if (POS_Type == true)
                {
                    // Means that this category will not follow the grouping and MinMax 
                    // All data will be loaded. The third file will be matched against the other two
                    // Eg BDC207 
                    // Transactions come in more than one dates. 
                    WReconcCategoryId = WMatchingCateg;
                    CreateWorkingTables_POS_Or_OutGoing(WOperator, WSignedId,
                                                WMatchingCateg, WReconcCategoryId,
                                                               WRMCycle);

                    if (RecordsToMatch == true)
                    {
                        W_MPComment += DateTime.Now + "_" + "Working Files were created.." + Environment.NewLine;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        //*******************************
                        // MAkE MATCHING

                        //******************************

                        W_MPComment += DateTime.Now + "_" + "Matching Starts." + Environment.NewLine;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);
                        // Match
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

                        //************************
                        // FINALLY - UPDATING AFTER MATCHING 
                        //************************
                        UpdatingAfterMatching_Mode_2(WOperator, WMatchingCateg, WReconcCategoryId);

                        // UPDATING IF PRESENTER GOES TO REPLENISHMENT
                        // 

                    }
                    else
                    {
                        W_MPComment += DateTime.Now + "_" + "No Records to match for this Category.. Process finishes for this category" + Environment.NewLine;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        //************************
                        // FINALLY - UPDATING AFTER MATCHING 
                        //************************
                        UpdatingAfterMatching_Mode_2(WOperator, WMatchingCateg, WReconcCategoryId);

                    }
                    return;
                }


                if (No_Group_Type == true) // This is not for the ATMs 
                {
                    // Means that this category will not follow the grouping and MinMax 
                    // All data will be loaded. The third file will be matched against the other two
                    // Eg BDC207 
                    // Transactions come in more than one dates. 
                    WReconcCategoryId = WMatchingCateg;

                    WGroupId = 999999; // six 9s 
                    CreateWorkingTables(WOperator, WSignedId, WMatchingCateg, WReconcCategoryId,
                                                                                WRMCycle, WGroupId);

                    if (NumberOfLoadedToWorkingTablesATMs > 0)

                    {
                        W_MPComment += DateTime.Now + "_" + "Working Files were created.." + Environment.NewLine;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        //*******************************
                        // MAkE MATCHING
                        //******************************

                        W_MPComment += DateTime.Now + "_" + "Matching Starts." + Environment.NewLine;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);
                        // Match
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

                        //************************
                        // FINALLY - UPDATING AFTER MATCHING 
                        //************************
                        UpdatingAfterMatching_Mode_2(WOperator, WMatchingCateg, WReconcCategoryId);
                    }
                    else
                    {
                        W_MPComment += DateTime.Now + "_" + "No Records to match for this Category.. Process finishes for this category" + Environment.NewLine;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        //************************
                        // FINALLY - UPDATING AFTER MATCHING 
                        //************************
                        UpdatingAfterMatching_Mode_2(WOperator, WMatchingCateg, WReconcCategoryId);

                    }
                    return;
                }

                // Continue with groups of ATMs 
                TableThisCategoryGroups = new DataTable();
                TableThisCategoryGroups.Clear();
                if (WOperator == "BDACEGCA" & MatchingCateg == "EMR203")
                {
                    MatchingCateg = MatchingCateg;
                }


                SqlString =
                   " SELECT MatchingCategoryId , GroupId, ReconcCategoryId "
                   + " FROM [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories] "
                   + " WHERE MatchingCategoryId = @MatchingCategoryId "
                   + " ORDER BY GroupId ";

                using (SqlConnection conn =
                 new SqlConnection(connectionStringATMs))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                        {

                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCategoryId", WMatchingCateg);

                            //Create a datatable that will be filled with the data retrieved from the command

                            sqlAdapt.Fill(TableThisCategoryGroups);

                            // Close conn
                            conn.Close();

                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        W_MPComment = W_MPComment + "Process has been cancelled stage 236" + Environment.NewLine;
                        W_MPComment += DateTime.Now;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        CatchDetails(ex, "602");
                    }



                Message = "Groups identified within category : " + WMatchingCateg;
                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion
            #region Work with category Loop for Groups of this matching category , make matching and update sessions 

            if (FromOwnATMs == true)
            {
                int I = 0;

                // LOOP FOR GROUPS OF THIS MATCHING CATEGORY

                while (I <= (TableThisCategoryGroups.Rows.Count - 1))
                {

                    WMatchingCateg = (string)TableThisCategoryGroups.Rows[I]["MatchingCategoryId"];
                    WGroupId = (int)TableThisCategoryGroups.Rows[I]["GroupId"];
                    WReconcCategoryId = (string)TableThisCategoryGroups.Rows[I]["ReconcCategoryId"];
                    //******************************************************
                    // CREATE FILES FOR THE SPECIFIC GROUP BASED ON MINMAX DATE TIME
                    // Find corresponding datetimes in corresponding files
                    // Create table with Cut Off last Seq Numbers
                    // UPDATE SOURCE FILES AS PROCESSED             
                    //******************************************************
                    //**********************

                    //if (WReconcCategoryId == "RECATMS-114"
                    //    )
                    //{
                    //    Message = "THIS THE ONE - " + WMatchingCateg + " AND " + WReconcCategoryId;
                    //    System.Windows.Forms.MessageBox.Show(Message);
                    //    // CONTINUE TO MATCHING 
                    //}
                    //else
                    //{
                    //    I++;
                    //    continue;
                    //}

                    BeforeCallDtTime_G = DateTime.Now;

                    Rcs.ReadReconcCategoriesSessionsByCategoryAndCycle(WReconcCategoryId, WRMCycle);

                    W_MPComment = Rcs.MPComment; // Initiation of Comment

                    W_MPComment += DateTime.Now + "_" + "Matching Starts Category.. " + WMatchingCateg + Environment.NewLine;
                    W_MPComment += DateTime.Now + "_" + "..And Reconciliation Group . ID:.." + WReconcCategoryId + Environment.NewLine;
                    //**********************
                    W_MPComment += DateTime.Now + "_" + "Create Working Files for Group Identified.. ID:.." + WReconcCategoryId + Environment.NewLine;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);


                    CreateWorkingTables(WOperator, WSignedId, WMatchingCateg, WReconcCategoryId,
                                                                                 WRMCycle, WGroupId);

                    //if (WReconcCategoryId == "RECATMS-108" & WMatchingCateg == "BDC205")
                    //{
                    //    MessageBox.Show("This is the one");
                    //}

                    if (NumberOfLoadedToWorkingTablesATMs > 0)
                    {
                        // UPDATE BEFORE MATCHING 
                        //
                        W_MPComment += DateTime.Now + "_" + "Working Files have been created:..for ATMs..Records loaded no... " + NumberOfLoadedToWorkingTablesATMs.ToString() + Environment.NewLine;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        //*******************************
                        // MAkE MATCHING For this Group 
                        //*******************************
                        W_MPComment += DateTime.Now + "_" + "Matching starts... " + Environment.NewLine;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        MakeMatchingByGroupOrCategory(WOperator, WSignedId, WMatchingCateg, WReconcCategoryId, WRMCycle, WGroupId);

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

                        //************************
                        // FINALLY - UPDATING AFTER MATCHING 
                        //************************

                        UpdatingAfterMatching_OurATMs(WOperator, WMatchingCateg, WReconcCategoryId);
                        //
                        //**********************************
                    }
                    else
                    {
                        //UpdatingAfterMatching_OurATMs(WOperator, WMatchingCateg, WReconcCategoryId);

                        // Update Matching Category as passing Matched. 
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat01(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat02(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat03(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat04(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat05(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat06(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat07(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat08(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat09(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);


                        W_MPComment += DateTime.Now + "_" + "No ATM to match for this Group. Process finishes " + NumberOfLoadedToWorkingTablesATMs.ToString() + Environment.NewLine;

                        W_MPComment += DateTime.Now + "_________________________" + Environment.NewLine;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    }
                    // Insert Performance 

                    Message = "Matching Finishes for Group : " + WGroupId.ToString() + "..Unmatched.." + NumberOfUnmatched.ToString();
                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, BeforeCallDtTime_G, DateTime.Now, Message);

                    I++; // Read Next entry of the table - next Group

                }



            }
            else
            {
                //*************************
                // From NOT_ATMs
                //************************* 
                WGroupId = 0;

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
                    CreateWorkingTables(WOperator, WSignedId,
                                                WMatchingCateg, WReconcCategoryId,
                                                               WRMCycle, WGroupId);

                }

                

                if (RecordsToMatch == true)
                {
                    W_MPComment += DateTime.Now + "_" + "Working Files were created.." + Environment.NewLine;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    //*******************************
                    // MAkE MATCHING

                    //******************************

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

            // Make Loop for this Group 
            if (FromOwnATMs == true)
            {
                //*****************************************
                // LOOP - LOOP - LOOP
                //***************************************** 
                // Read All ATMs belonging to this Group 
                // Fill Up a Table 
                TableThisGroupAtms = new DataTable();
                TableThisGroupAtms.Clear();

                if (InAtmsReconcGroup == 999999)
                {
                    // Get all ATMs 
                    SqlString =
                     " SELECT AtmsReconcGroup , AtmNo, DepCurNm"
                   + " FROM [ATMS].[dbo].[ATMsFields] "
                    + " WHERE AtmNo NOT IN ('DBLModelATM','NCRModelATM','WINModelATM') ";
                }
                else
                {
                    // Get the ATMs for this Group
                    SqlString =
                      " SELECT AtmsReconcGroup , AtmNo, DepCurNm"
                    + " FROM [ATMS].[dbo].[ATMsFields] "
                    + " WHERE AtmsReconcGroup = @AtmsReconcGroup AND AtmNo NOT IN ('DBLModelATM','NCRModelATM','WINModelATM')";
                }


                using (SqlConnection conn =
                 new SqlConnection(connectionStringATMs))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                        {

                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmsReconcGroup", InAtmsReconcGroup);
                            //Create a datatable that will be filled with the data retrieved from the command

                            sqlAdapt.Fill(TableThisGroupAtms);

                            // Close conn
                            conn.Close();

                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        W_MPComment = W_MPComment + "Process has been cancelled stage 237" + Environment.NewLine;
                        W_MPComment += DateTime.Now;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        CatchDetails(ex, "603");
                    }
            }

            #endregion

            #region For Each ATM Find MinMax DateTime
            if (FromOwnATMs == true)
            {
                RecordsToMatch = false;

                while (I <= (TableThisGroupAtms.Rows.Count - 1))
                {
                    RecordFound = true;

                    WAtmsReconcGroup = (int)TableThisGroupAtms.Rows[I]["AtmsReconcGroup"];
                    WAtmNo = (string)TableThisGroupAtms.Rows[I]["AtmNo"];
                    WDepCurNm = (string)TableThisGroupAtms.Rows[I]["DepCurNm"];
                    //AtmNo = '00015999'
                    if (WAtmNo == "00015999")
                    {
                        // testing
                        WAtmNo = WAtmNo;
                    }


                    // FIND 
                    Mm.ReadRRDMAtmsMinMaxSpecific(WRMCycle, WAtmNo);

                    if (Mm.RecordFound == true)
                    {
                        // WORK BASED ON INFO
                        if (NumberOfTables == 2 & Mm.MinMaxDtTwoTables == NullPastDate)
                        {
                            // MinMax for two found but it is NULL
                            I++; // Read Next entry of the table 
                            continue;
                        }
                        if (NumberOfTables == 3 & Mm.MinMaxDtThreeTables == NullPastDate)
                        {
                            // MinMax for three found but it is NULL
                            I++; // Read Next entry of the table 
                            continue;
                        }
                    }
                    else
                    {
                        // FIND MINMAX
                        MaxDt01 = NullPastDate;
                        MaxDt02 = NullPastDate;

                        MaxDt03 = NullPastDate;

                        TableId = SourceTable_A;

                        MaxDt01 = Mgt.ReadAndFindMaxDateTimeForMpaNoCategory(TableId, WAtmNo);

                        if (MaxDt01 == NullPastDate)
                        {
                            // No records for one of files 
                            I++; // Read Next entry of the table 
                            continue;
                        }

                        TableId = SourceTable_B;

                        MaxDt02 = Mgt.ReadAndFindMaxDt_No_Category_ByATM(TableId, WAtmNo); // Leave it here - handles reversals 

                        if (Mgt.Comment == "Reversals" & MaxDt02 != NullPastDate)
                        {
                            RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();
                            string WSelectionCriteria = "where Left(FileName, 17) = '" + WAtmNo + "_"
                                    + MaxDt02.Date.ToString("yyyyMMdd") + "'";

                            Rfm.ReadLoadedFilesAND_Find_If_Loaded(WSelectionCriteria);

                            if (Rfm.RecordFound == true)
                            {
                                // Journal loaded
                                MaxDt01 = MaxDt02;
                            }

                        }

                        if (MaxDt02 == NullPastDate)
                        {
                            // No records for one of files 
                            // Or Cancel during finding the maximum 
                            I++; // Read Next entry of the table 
                            continue;
                        }

                        if (NumberOfTables == 3)
                        {

                            TableId = SourceTable_C;

                            MaxDt03 = Mgt.ReadAndFindMaxDt_No_Category(TableId, WAtmNo);

                            // LEAVE IT AS COMMENT 
                            if (MaxDt03 == NullPastDate)
                            {
                                // This is case there is last reversal
                                MaxDt03 = MaxDt02;
                            }

                        }


                        Mm.MinMaxDtTwoTables = NullPastDate;
                        Mm.MinMaxDtThreeTables = NullPastDate;


                        //if (NumberOfTables == 2)
                        //{
                        // Two Tables
                        if (MaxDt01 == NullPastDate || MaxDt02 == NullPastDate)
                        {
                            Mm.MinMaxDtTwoTables = NullPastDate;
                        }
                        else
                        {
                            DateTime[] array1 = { MaxDt01, MaxDt02 };

                            //
                            // Find minimum number.
                            // 

                            Mm.MinMaxDtTwoTables = array1.Where(a => a != NullPastDate).Min();

                            // EqualDates Applies to BDC where dates in ATM and IST are the same

                        }

                        // }

                        if (NumberOfTables == 3)
                        {
                            // Three Tables
                            if (MaxDt01 == NullPastDate || MaxDt02 == NullPastDate || MaxDt03 == NullPastDate)
                            {
                                Mm.MinMaxDtThreeTables = NullPastDate;
                            }
                            else
                            {

                                DateTime[] array1 = { MaxDt01, MaxDt02, MaxDt03 };

                                //
                                // Find minimum date.
                                // 

                                Mm.MinMaxDtThreeTables = array1.Where(a => a != NullPastDate).Min();
                                //

                            }

                        }

                        if (EqualDatesForATMS == true)
                        {
                            Mm.InsertToMinMax(WRMCycle, WAtmNo, Mm.MinMaxDtTwoTables, Mm.MinMaxDtThreeTables);
                        }
                        else
                        {
                            if (ShowMessage == true & Environment.UserInteractive)
                            {
                                MessageBox.Show("28381_Comes from Loading of Working Files for ATMs" + Environment.NewLine
                                                                + "Check that code is working");
                            }

                        }


                    }


                    if (NumberOfTables == 2)
                    {

                        if (EqualDatesForATMS == true)
                        {
                            // ALL EQUAL to MINMax
                            MinMaxDt_01 = Mm.MinMaxDtTwoTables;
                            MinMaxDt_02 = Mm.MinMaxDtTwoTables;
                        }

                    }

                    if (NumberOfTables == 3)
                    {


                        if (EqualDatesForATMS == true)
                        {
                            // ALL EQUAL to MINMax FOR BANK de Caire 
                            MinMaxDt_01 = Mm.MinMaxDtThreeTables;
                            MinMaxDt_02 = Mm.MinMaxDtThreeTables;
                            MinMaxDt_03 = Mm.MinMaxDtThreeTables;
                        }

                    }


                    #endregion

                    #region For Each ATM Insert Records in Working Files and set original records as processed

                    if (WAtmNo == "AB1V0069")
                    {
                        WAtmNo = "AB1V0069";
                    }
                    // WORKING FILES FOR ATMS

                    //if (MinMaxDt != NullPastDate)
                    //{
                    RecordsToMatch = true;

                    if (NumberOfLoadedToWorkingTablesATMs == 0)
                    {
                        // This is the FIRST ATM within Group to go to Working Tables
                        Method_TruncateWorkingTables(NumberOfTables);
                    }

                    int TempCount1 = 0;
                    int TempCount2 = 0;
                    int TempCount3 = 0;
                    // Call stored Procedure to create files
                    // Insert Records In File01Y from Mpa

                    TempCount1 = Mtw.PopulateWorkingFile_ATMs_Working01("01", SourceTable_A, WMatchingCateg, WRMCycle, WAtmNo, MinMaxDt_01);
                    // Insert Records In File02Y from IST

                    //InsertRecordsInWorkingFile_02(connectionString, WMatchingCategoryId, WAtmNo, MinMaxTrace, WRMCycle);
                    TempCount2 = Mtw.PopulateWorkingFile_ATMs_V02("02", SourceTable_B, WMatchingCateg, WRMCycle, WAtmNo, MinMaxDt_02);

                    if (NumberOfTables == 3)
                    {
                        // Insert Records In File03Y from Fiserv

                        //InsertRecordsInWorkingFile_03(connectionString, WMatchingCategoryId, WAtmNo, MinMaxTrace, WRMCycle);
                        TempCount3 = Mtw.PopulateWorkingFile_ATMs_V03("03", SourceTable_C, WMatchingCateg, WRMCycle, WAtmNo, MinMaxDt_03);
                    }

                    int TempCountTotal = TempCount1 + TempCount2 + TempCount3;

                    if (TempCountTotal > 0)
                        NumberOfLoadedToWorkingTablesATMs = NumberOfLoadedToWorkingTablesATMs + TempCountTotal;

                    //if (WAtmNo == "00000508" & WMatchingCateg == "BDC205")
                    //{
                    //    MessageBox.Show("This is the one ");
                    //}

                    //CORRECTION1950
                    // Second Working File Contains all DEPOSIT Reversals with starting Full traceNumber = 2
                    // HERE WE READ THE DEPOSITS REVERSALS for this ATM and this Category NOT PROCESSED LESS TAN MINMAX DATE 
                    // WE CHECK IF THERE IS A CORRESPONDING RECORD IN Mpa. 
                    //
                    // IF it is in MPA then Delete Record From Second Working File => we donot want to take part in matching 
                    //
                    // Also =>Make IST Deposit reversals as processed 


                    TableId = SourceTable_B;
                    int WMode = 2;
                    Mgt.ReadTableAndFillTableWithDepositReversals(WMatchingCateg, TableId, MinMaxDt_02, WAtmNo, WMode);

                    if (NumberOfTables == 3)
                    {
                        TableId = SourceTable_C;
                        WMode = 3;
                        Mgt.ReadTableAndFillTableWithDepositReversals(WMatchingCateg, TableId, MinMaxDt_03, WAtmNo, WMode);
                    }

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

                    Mpa.UpdateMpaRecordsAsProcessed_With_ATM_V02(WMatchingCateg, WAtmNo, WRMCycle, MatchMask, MinMaxDt_01, 1);

                    // Update SourceTable_B with Processed = 1 and RMCycle
                    //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankSwitchTxns]"; 
                    TableId = SourceTable_B;

                    Mgt.UpdateSourceTablesAsProcessed_ATMs_V02(WMatchingCateg, TableId, MinMaxDt_02, WAtmNo, WRMCycle);

                    if (NumberOfTables == 3)
                    {
                        // Update SourceTable_C with Processed = 1 and RMCycle
                        //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankingSystemTxns]";
                        TableId = SourceTable_C;
                        Mgt.UpdateSourceTablesAsProcessed_ATMs_V02(WMatchingCateg, TableId, MinMaxDt_03, WAtmNo, WRMCycle);


                    }

                    I++; // Read Next entry of the table 

                }


                if (NumberOfLoadedToWorkingTablesATMs > 0)
                {
                    Message = "Cut Off Date : .. " + WCut_Off_Date.ToShortDateString() + Environment.NewLine
                                                    + "Txns added in Working Files for "
                                                    + NumberOfLoadedToWorkingTablesATMs.ToString() + " ATMs " + Environment.NewLine
                                                    + " of Group No: " + InAtmsReconcGroup.ToString();

                    // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);


                    if (ShowMessage == true & Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }
                }
            }

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

            if (FromOwnATMs == false)
            {

                //*****************************************************
                // CHECK IF THERE ARE RECORDS TO MATCHED FOR THE THREE FILES
                //*****************************************************
                // Find MaxDate 
                //
                //if (InMatchingCateg == "BDC251")
                //{
                //    MessageBox.Show("Here is the problem"); 
                //}

                MaxDt01 = NullPastDate;
                MaxDt02 = NullPastDate;
                if (NumberOfTables == 3)
                {
                    MaxDt03 = NullPastDate;
                }
                RRNumber01 = "";
                RRNumber02 = "";
                RRNumber03 = "";
                Mask_Card01 = "";
                Mask_Card02 = "";
                Mask_Card03 = "";

                TransAmt01 = 0;
                TransAmt02 = 0;
                TransAmt03 = 0;

                TableId = SourceTable_A;

                DateTime MaxSetl_date = new DateTime(1900 - 01 - 01);

                //MaxDt01 = Mgt_BDC.ReadAndFindMaxDateTimeForMpa(TableId, InMatchingCateg, WAtmNo);

                //MaxDt01 = Mgt_BDC.ReadAndFindMaxDateTimeForMpa_NO_ATM(TableId, InMatchingCateg);

                if (InOperator == "BCAIEGCX")
                {
                    PRX = "BDC"; // "+PRX+" eg "BDC"
                }
                else
                {
                    PRX = "EMR";
                }

                if (PRX == "BDC")
                {
                    MaxDt01 = Mpa.ReadAndFindMaxDateTimeForMpa_NonATM(TableId, InMatchingCateg, 1);

                    if (Mpa.RecordFound == true)
                    {
                        Mpa.ReadAndFindRRNUMBER_NO_ATM(TableId, InMatchingCateg, MaxDt01, 1);// Find Unique
                        RRNumber01 = Mpa.RRNumber;
                        Mask_Card01 = Mpa.CardNumber;
                        TransAmt01 = Mpa.TransAmount;
                        MaxSetl_date = Mpa.SET_DATE;
                    }
                }

                if (PRX == "EMR")
                {
                    if (InMatchingCateg == "EMR285")
                    {
                        InMatchingCateg = "EMR285";
                    }

                    MaxDt01 = Mpa.ReadAndFindMaxDateTimeForMpa_NonATM_2(TableId, InMatchingCateg, 1);

                    if (Mpa.RecordFound == true)
                    {
                        Mpa.ReadAndFindRRNUMBER_NO_ATM(TableId, InMatchingCateg, MaxDt01, 1);// Find Unique
                        RRNumber01 = Mpa.RRNumber;
                        Mask_Card01 = Mpa.CardNumber;
                        TransAmt01 = Mpa.TransAmount;
                        MaxSetl_date = Mpa.SET_DATE;
                    }
                }


                // TRACE Step1
                Message = "MATCHING.." + WMatchingCateg + Environment.NewLine
                                   + "Step1:" + MaxDt01.ToString();
                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                //
                // Handling the 123 exception where after cut off 123 do not sends all transactions
                // AND OTHER CATEGORIES
                int MinusMin = 0;

                string ParId;

                if (InMatchingCateg == PRX + "250") // FAWRY UP TO 
                {

                    string OccurId;

                    // THE BANK WILL BE ASKED FOR THIS

                    // WCAP_DATE = MaxDt01.Date;

                    WCAP_DATE = WCut_Off_Date;

                    // Get TXNS to Working files with less or Equal to WCAP_DATE

                }

                // Check if this Matching Category goes to the direction of SET to CAP_DATE reconciliation method
                bool SET_TO_SET = false;

                ParId = "821";

                Gp.ReadParametersSpecificNm(InOperator, ParId, InMatchingCateg);

                if (Gp.RecordFound == true)
                {
                    SET_TO_SET = true;
                    // 
                }

                // SET it to avoid 
                MinusMin = 0; // Set it to avoid activation of the next process
                if (SET_TO_SET == true & MinusMin == 0)

                //(InMatchingCateg == PRX+"210" 
                //|| InMatchingCateg == PRX + "211"
                //|| InMatchingCateg == PRX + "270"
                //|| InMatchingCateg == PRX + "271"

                //)
                //& MinusMin == 0)
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
                    Method_TruncateWorkingTables(NumberOfTables);
                    ///
                    RecordsToMatch = true;

                    // Call stored Procedure to create files
                    // Insert Records In File01Y from Mpa
                    //if (WOperator == "BDACEGCA")
                    //{
                    //    DateTime temp = new DateTime(2021, 09, 28);
                    //    WSET_DATE = WSET_DATE.AddDays(-1);
                    //}
                    // TAKE ALL RECORDS FROM Mpa
                    Mtw.PopulateWorkingFile_Working01_NOT_ATMS_No_Date("01", SourceTable_A, WMatchingCateg, WRMCycle, WSET_DATE);

                    //Take all records with the set Settlement date 
                    Mtw.PopulateWorkingFile_V02_NotATMs_On_SET_DATE("02", SourceTable_B, WMatchingCateg, WRMCycle, WSET_DATE);

                    if (NumberOfTables == 3)
                    {
                        // Insert Records In Working File03

                        Mtw.PopulateWorkingFile_V02_NotATMs_On_SET_DATE("03", SourceTable_C, WMatchingCateg, WRMCycle, WSET_DATE);
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

                    // UPDATE Mpa
                    Mpa.UpdateMpaRecordsAsProcessed_NO_ATM_No_Date(WMatchingCateg, WSET_DATE, WRMCycle, MatchMask, 1);

                    // Update SourceTable_B with Processed = 1 and RMCycle
                    //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankSwitchTxns]"; 
                    TableId = SourceTable_B;

                    Mgt.UpdateSourceTablesAsProcessed_NonAtms_Based_SET_DATE(WMatchingCateg, TableId, WSET_DATE, WRMCycle, MatchMask);

                    if (NumberOfTables == 3)
                    {
                        // Update SourceTable_C with Processed = 1 and RMCycle
                        //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankingSystemTxns]";
                        TableId = SourceTable_C;

                        Mgt.UpdateSourceTablesAsProcessed_NonAtms_Based_SET_DATE(WMatchingCateg, TableId, WSET_DATE, WRMCycle, MatchMask);


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
                else
                {
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

                                //System.Windows.Forms.MessageBox.Show("ERROR 1059...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());

                                //return;

                                MinMaxDt_02 = MinMaxDt;
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

                                //System.Windows.Forms.MessageBox.Show("ERROR 1061...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());

                                //return;

                                MinMaxDt_01 = MinMaxDt;
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

                                //System.Windows.Forms.MessageBox.Show("ERROR 1059...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());

                                //return;

                                MinMaxDt_02 = MinMaxDt;
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

                                //System.Windows.Forms.MessageBox.Show("ERROR 10592...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());

                                //return;

                                MinMaxDt_03 = MinMaxDt;

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

                                //System.Windows.Forms.MessageBox.Show("ERROR 1061...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());

                                //return;

                                MinMaxDt_01 = MinMaxDt;
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

                                //System.Windows.Forms.MessageBox.Show("ERROR 10593...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());
                                ////}
                                //if (InMatchingCateg == PRX + "210")
                                //{
                                //    MessageBox.Show(" RRNumber01 " + RRNumber01 + Environment.NewLine

                                //        );
                                //}

                                //return;
                                MinMaxDt_03 = MinMaxDt;
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

                                //System.Windows.Forms.MessageBox.Show("ERROR 1061...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());

                                //return;
                                MinMaxDt_01 = MinMaxDt;
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

                                //System.Windows.Forms.MessageBox.Show("ERROR 10594...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());

                                //return;
                                MinMaxDt_02 = MinMaxDt;
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
                        //
                        // Check for Credit card Up to 13:00 
                        //
                        ParId = "823";
                        //  OccurId = "01"; // 
                        Gp.ReadParametersSpecificNm(WOperator, ParId, WMatchingCateg);

                        if (Gp.RecordFound == true)
                        {
                            // THIS APPLIES ONLY ON IST AND FLEXCUBE
                            int UpToMinutes = (int)Gp.Amount;

                            if (UpToMinutes > 0)
                            {
                                MinMaxDt = MinMaxDt.Date.AddMinutes(UpToMinutes);
                            }

                            MinMaxDt_01 = MaxFutureDate; // Include all records eg for BDC240 = Credit Card
                            MinMaxDt_02 = MinMaxDt;
                            if (NumberOfTables == 3)
                            {
                                MinMaxDt_03 = MinMaxDt;
                            }
                        }

                        RecordsToMatch = true;

                        //
                        // INITIALISE WORKING TABLE
                        //
                        Method_TruncateWorkingTables(NumberOfTables);
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
            Method_TruncateWorkingTables(NumberOfTables);
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

        // Fields of Working files 
        private void WorkingTableFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            MatchingCateg = (string)rdr["MatchingCateg"];
            RMCycle = (int)rdr["RMCycle"];
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
            FullDtTm = (DateTime)rdr["FullDtTm"];
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
            TransAmt = (decimal)rdr["TransAmt"];
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
        private void Method_TruncateWorkingTables(int InNumberOfTables)
        {
            // Initialise File 01
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";

            if (WTestingWorkingFiles == false)
                Mtw.TruncateInWorkingFile(TableId);
            // Initialise File 02
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";

            if (WTestingWorkingFiles == false)
                Mtw.TruncateInWorkingFile(TableId);

            if (InNumberOfTables == 3)
            {
                // Initialise File 03
                TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";

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
                MakeMatchingOf_2_Working_Tables(WOperator, WSignedId,
                                                              InMatchingCateg, InReconcCateg,
                                                              WRMCycle);
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

            //if (FromOwnATMs == false)
            //{
            //    //  MAKE MATCHING AND Update a record per ATM + Rcs (Reconciliation Category)
            //    //
            //    if (RecordsToMatch == true)
            //    {
            //        Message = "Category : " + InMatchingCateg + Environment.NewLine
            //                        + "MATCHING PROCESS WILL START FOR THIS CATEGORY  ";

            //        Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);

            //        if (ShowMessage == true & Environment.UserInteractive)
            //        {
            //            System.Windows.Forms.MessageBox.Show(Message);
            //        }

            //        // ******************************************************************
            //        // MAKE MATCHING , Create Report, Save Discrepancies After Summarisation
            //        //*******************************************************************

            //        if (NumberOfTables == 3 & POS_Type == false)
            //        {
            //            MakeMatchingOf_3_Working_Tables(WOperator, WSignedId,
            //                                                          InMatchingCateg, InReconcCateg,
            //                                                          WRMCycle);
            //        }
            //        if (NumberOfTables == 3 & POS_Type == true & StartFromThree == false)
            //        {
            //            MakeMatchingOf_3_Working_Tables_POS(WOperator, WSignedId,
            //                                                          InMatchingCateg, InReconcCateg,
            //                                                          WRMCycle);
            //        }
            //        if (NumberOfTables == 3 & POS_Type == true & StartFromThree == true)
            //        {
            //            MakeMatchingOf_3_Working_Tables_POS_StartFromThree(WOperator, WSignedId,
            //                                                          InMatchingCateg, InReconcCateg,
            //                                                          WRMCycle);
            //        }
            //        if (NumberOfTables == 2)
            //        {
            //            MakeMatchingOf_2_Working_Tables(WOperator, WSignedId,
            //                                                          InMatchingCateg, InReconcCateg,
            //                                                          WRMCycle);
            //        }

            //        // *****************************************************************************
            //        // Final Message  
            //        // *****************************************************************************
            //        Message = "MATCHING COMPLETED FOR CATEGORY :"
            //                                              + WMatchingCategoryId;

            //        Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
            //        if (ShowMessage == true & Environment.UserInteractive)
            //        {
            //            System.Windows.Forms.MessageBox.Show(Message);
            //        }

            //    }
            //}

            #endregion
        }
        // Updating after Matching 
        private void UpdatingAfterMatching_OurATMs(string InOperator, string InMatchingCateg, string InReconcCateg)
        {
            // *********************************************************************
            // Update a Reconciliation record per ATM + Rcs (Reconciliation Category)
            // ******************************************************************
            SelectionCriteria = "WHERE RMCateg='" + InReconcCateg + "'"
                                    + " AND MatchingCateg='" + InMatchingCateg + "'"
                                    + " AND IsMatchingDone = 1"
                                    + " AND MatchingAtRMCycle =" + WRMCycle;

            //if (InReconcCateg == "RECATMS-108" & InMatchingCateg == "BDC205")
            //{
            //    MessageBox.Show("This is the one _2"); 
            //}

            ReadUnMatchedTxnsMasterPoolATMsTotalsForAtmsAND_UPDATE_OurATMs(WOperator, WRMCycle, SelectionCriteria);



            // *****************************************************************************
            // Update [tblMatchingTxnsMasterPoolATMs] mask card number with full card number 
            // *****************************************************************************
            //  Check Parameters if wish from cardnumber masked to full 
            // 

            string ParId = "266";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                // IF YES THEN UPDATE MASK CARD WITH FULL 

                if (NumberOfTables == 2)
                {
                    // Second File = Destination File ... SourceTable_B
                    UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_B);

                }

                if (NumberOfTables == 3)
                {
                    // Second File = Destination File ... SourceTable_C
                    UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_C);
                }

            }
        }

        // Updating after Matching 
        private void UpdatingAfterMatching_Mode_2(string InOperator, string InMatchingCateg, string InReconcCateg)
        {
            // *********************************************************************
            // Update a Reconciliation record per ATM + Rcs (Reconciliation Category)
            // ******************************************************************

            SelectionCriteria = "WHERE RMCateg='" + InReconcCateg + "'"
                                + " AND MatchingCateg='" + InMatchingCateg + "'"
                                + " AND IsMatchingDone = 1"
                                + " AND MatchingAtRMCycle =" + WRMCycle;

            ReadUnMatchedTxnsMasterPoolATMsTotals_AND_Update_Mode_2(WOperator, WRMCycle, SelectionCriteria);


            // *****************************************************************************
            // Update [tblMatchingTxnsMasterPoolATMs] mask card number with full card number 
            // *****************************************************************************
            //  Check Parameters if wish from cardnumber masked to full 
            // 

            string ParId = "266";
            string OccurId = "1";
            RRDMGasParameters Gp = new RRDMGasParameters();
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                // IF YES THEN UPDATE MASK CARD WITH FULL 

                if (NumberOfTables == 2)
                {
                    // Second File = Destination File ... SourceTable_B
                    UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_B);

                }

                if (NumberOfTables == 3)
                {
                    // Second File = Destination File ... SourceTable_C
                    UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_C);
                }

            }
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
            TableUnMatched.Columns.Add("Type", typeof(int));
            TableUnMatched.Columns.Add("DublInPos", typeof(int));
            TableUnMatched.Columns.Add("InPos", typeof(int));
            TableUnMatched.Columns.Add("NotInPos", typeof(int));
            TableUnMatched.Columns.Add("TerminalId", typeof(string));
            TableUnMatched.Columns.Add("TraceNo", typeof(int));
            TableUnMatched.Columns.Add("RRNumber", typeof(string));
            TableUnMatched.Columns.Add("CardNumber", typeof(string));
            TableUnMatched.Columns.Add("AccNo", typeof(string));
            TableUnMatched.Columns.Add("TransAmt", typeof(decimal));

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
            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            //// Dublicates in B : Switch
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "Dublicate In File02";
            WPos = 2;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX + ", FullDtTm";
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX + " AND  y.FullDtTm = dt.FullDtTm";

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            // Dublicates in C : BANKING File 
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            WCase = "Dublicate In File03";
            WPos = 3;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");

            ListMatchingFields = Mf.Dublicate_List_FieldsStageX + ", FullDtTm";
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX + " AND  y.FullDtTm = dt.FullDtTm";

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

            // ******************************
            // In C and Not In B
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "In File03 And Not In File02";

            WPosA = 3;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

            //// ******************************
            //// In C and Not In A
            /// NOT NEEDED AS IF A=B and B=C then A=C
            //// ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "In File03 And Not In File01";

            WPosA = 3;
            WPosB = 1; // If NOT FOUND IN THIS POSITION

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);


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

            Tr.DeleteWReport97_ForMatching(WSignedId);

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
                    if (Environment.UserInteractive)
                    {
                        if (NumberOfUnmatched > 6000)
                        {
                            MessageBox.Show("There are high number of discrepancies!.." + Environment.NewLine
                                + "Reconciliation Matching Category: " + WReconcCategoryId + Environment.NewLine
                                + "Number of discrepancies : " + NumberOfUnmatched.ToString()
                                );
                        }
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

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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

            Tr.DeleteWReport97_ForMatching(WSignedId);

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
                    if (ShowMessage == true & Environment.UserInteractive)
                    {
                        if (NumberOfUnmatched > 6000)
                        {
                            MessageBox.Show("There are high number of discrepancies!" + Environment.NewLine
                                + "Reconciliation Matching Category: " + WReconcCategoryId + Environment.NewLine
                                + "Number of discrepancies :.. " + NumberOfUnmatched.ToString()
                                );
                        }
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

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);


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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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

            Tr.DeleteWReport97_ForMatching(WSignedId);

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
                    if (ShowMessage == true & Environment.UserInteractive)
                    {
                        if (NumberOfUnmatched > 6000)
                        {
                            MessageBox.Show("There are high number of discrepancies!" + Environment.NewLine
                                + "Reconciliation Matching Category:.. " + WReconcCategoryId + Environment.NewLine
                                + "Number of discrepancies : " + NumberOfUnmatched.ToString()
                                );
                        }
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

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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

            Tr.DeleteWReport97_ForMatching(WSignedId);

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
                    if (ShowMessage == true & Environment.UserInteractive)
                    {
                        if (NumberOfUnmatched > 6000)
                        {
                            MessageBox.Show("There are high number of discrepancies!" + Environment.NewLine
                                + "Reconciliation Matching Category:.. " + WReconcCategoryId + Environment.NewLine
                                + "Number of discrepancies :.. " + NumberOfUnmatched.ToString()
                                );
                        }
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
        public void MakeMatchingOf_2_Working_Tables
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

            UnmatchedTableFieldsDefinition();

            #endregion

            #region Find dublicates 

            //string FileId;
            int WPos;

            string FileIdA;
            string FileIdB;
            //string WCase;

            // Dublicates 
            // Dublicates in A : 
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "Dublicate In File01";
            WPos = 1;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX + ", FullDtTm";
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX + " AND  y.FullDtTm = dt.FullDtTm";

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            // Dublicates in B : 
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "Dublicate In File02";
            WPos = 2;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX + ", FullDtTm";
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX + " AND  y.FullDtTm = dt.FullDtTm";

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

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

            #region Do Matching based on the created working files

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


            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

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
                Tr.DeleteWReport97_ForMatching(WSignedId);

                //Insert Records For WReport97_ForMatching
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

                        W_MPComment = W_MPComment + "Process has been cancelled stage 241" + Environment.NewLine;

                        W_MPComment += DateTime.Now;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        CatchDetails(ex, "606");
                    }

                #endregion

                #region Saved Dicrepancies And Summarise (compress) Discrepancies
                //Insert Records For MatchingDiscrepancies_BDC
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
                        W_MPComment = W_MPComment + "Process has been cancelled stage 242" + Environment.NewLine;

                        W_MPComment += DateTime.Now;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        CatchDetails(ex, "607");
                    }
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
                        if (ShowMessage == true & Environment.UserInteractive)
                        {
                            if (NumberOfUnmatched > 6000)
                            {
                                MessageBox.Show("There are high number of discrepancies!" + Environment.NewLine
                                    + "Reconciliation Matching Category: " + WReconcCategoryId + Environment.NewLine
                                    + "Number of discrepancies :.. " + NumberOfUnmatched.ToString()
                                    );
                            }
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

        //
        // Methods 
        // Find Duplicate file X 
        // FILL UP A TABLE
        // GROUP OF ATMS / Category 
        public void FindDuplicateAddTable(string InFileId, string InCase, int InPos, string InListMatchingFields, string IN_OnMatchingFields)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

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
       //+ " GROUP BY TerminalId, TraceNo, AccNo, TransAmt, TransDate "
       + " GROUP BY " + InListMatchingFields
       + " HAVING COUNT(*)>1 "
       + " ) dt"
       + " ON " + IN_OnMatchingFields
       //+ " ON y.TerminalId=dt.TerminalId AND y.TraceNo= dt.TraceNo AND y.AccNo=dt.AccNo "
       //     + " AND y.TransAmt=dt.TransAmt AND y.TransDate =dt.TransDate "
       + " ";

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

                            WorkingTableFields(rdr);

                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = TableUnMatched.NewRow();

                            NumberOfDublicates = NumberOfDublicates + 1;

                            RowSelected["UserId"] = WSignedId;
                            RowSelected["OriginSeqNo"] = SeqNo;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["WCase"] = InCase;
                            RowSelected["Type"] = 1; // Dublicate
                            RowSelected["DublInPos"] = InPos;
                            RowSelected["InPos"] = InPos;
                            RowSelected["NotInPos"] = 0;
                            RowSelected["TerminalId"] = TerminalId;

                            RowSelected["TraceNo"] = TraceNo;

                            // Not From Our ATMs 
                            RowSelected["RRNumber"] = RRNumber;

                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNo"] = AccNo;
                            RowSelected["TransAmt"] = TransAmt;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = RMCycle;
                            RowSelected["FileId"] = InFileId;
                            RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + TraceNo + RRNumber + TransAmt.ToString() + CardNumber;
                            RowSelected["FullDtTm"] = FullDtTm;
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
        public void FindNotExistAddTableTableXToTableY(string InFileIdA,
                                string InFileIdB, string InCase, int InPos, int NotInPos, string InMatchingFields)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string MatchingString = Mf.CreateStringOfMatchingFieldsForStage_String(WMatchingCateg, "Stage A");

            string TraceOrRRN;

            TotalSelected = 0;

            SqlString =

            " SELECT * , " + MatchingString + " As WMatchingString "
             + " FROM " + InFileIdA + " c1"
             + " WHERE NOT EXISTS(SELECT * "
             + " FROM " + InFileIdB + " c2 "
             + " WHERE " + InMatchingFields + ")";

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

                            //WorkingTableFields(rdr);
                            SeqNo = (int)rdr["SeqNo"];
                            MatchingCateg = (string)rdr["MatchingCateg"];
                            RMCycle = (int)rdr["RMCycle"];
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
                            FullDtTm = (DateTime)rdr["FullDtTm"];
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
                            RowSelected["TransAmt"] = TransAmt;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = RMCycle;
                            RowSelected["FileId"] = TableId;
                            if (FromOwnATMs == true)
                            {
                                RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + TraceNo + "_" + TransAmt.ToString() + CardNumber;
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

            int Count = 0; 

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
             + " WHERE c1.MatchingCateg ='" + InMatchingCateg + "' AND c2.MatchingCateg = c1.MatchingCateg AND c1.Processed = 0 "
             + " AND c1.ResponseCode = '0' AND c2.Mask = '' AND "
             + InMatchingFields + " AND c2.LoadedAtRMCycle=@LoadedAtRMCycle AND C2.ResponseCode <> '200000' AND C2.Comment <> 'Reversals' ) ";

            using (SqlConnection conn =
                      new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            //return; 
                            Count = Count + 1;
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
                            TransAmt = (decimal)rdr["TransAmt"];
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
                            RowSelected["TransAmt"] = TransAmt;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = RMCycle;
                            RowSelected["FileId"] = TableId;
                            if (FromOwnATMs == true)
                            {
                                RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + TraceNo + TransAmt.ToString() + CardNumber;
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
                            RowSelected["TransAmt"] = TransAmt;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = InRMCycle;
                            RowSelected["FileId"] = TableId;
                            if (FromOwnATMs == true)
                            {
                                RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + TraceNo + TransAmt.ToString() + CardNumber;
                            }
                            else
                            {
                                RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + RRNumber + TransAmt.ToString() + CardNumber;
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

        //
        // Not Matched for days in Master pool 
        //
        public void ExistInTableButNotMatchedForDays_Our_ATMS(string InTableId, string InMatchingCateg,
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
             + " WHERE MatchingCateg = @MatchingCateg And TransDate< @LimitDate "
             + " AND IsMatchingDone = 0 "
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
                        cmd.Parameters.AddWithValue("@LimitDate", InLimitDate);


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
                            RowSelected["TransAmt"] = TransAmt;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = InRMCycle;
                            RowSelected["FileId"] = TableId;
                            if (FromOwnATMs == true)
                            {
                                RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + TraceNo + TransAmt.ToString() + CardNumber;
                            }
                            else
                            {
                                RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + RRNumber + TransAmt.ToString() + CardNumber;
                            }

                            RowSelected["FullDtTm"] = FullDtTm;
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
            " SELECT  *"
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
                Type = (int)TableUnMatched.Rows[I]["Type"];
                DublInPos = (int)TableUnMatched.Rows[I]["DublInPos"];
                InPos = (int)TableUnMatched.Rows[I]["InPos"];
                NotInPos = (int)TableUnMatched.Rows[I]["NotInPos"];
                TerminalId = (string)TableUnMatched.Rows[I]["TerminalId"];
                TraceNo = (int)TableUnMatched.Rows[I]["TraceNo"];
                RRNumber = (string)TableUnMatched.Rows[I]["RRNumber"];
                CardNumber = (string)TableUnMatched.Rows[I]["CardNumber"];
                AccNo = (string)TableUnMatched.Rows[I]["AccNo"];
                TransAmt = (decimal)TableUnMatched.Rows[I]["TransAmt"];
                MatchingCateg = (string)TableUnMatched.Rows[I]["MatchingCateg"];
                RMCycle = (int)TableUnMatched.Rows[I]["RMCycle"];
                Matched_Characters = (string)TableUnMatched.Rows[I]["Matched_Characters"];
                FullDtTm = (DateTime)TableUnMatched.Rows[I]["FullDtTm"];
                ResponseCode = (string)TableUnMatched.Rows[I]["ResponseCode"];

                if ((Matched_Characters.Trim() != LastMatched_Characters.Trim())
                    || AccNo != LastAccNo
                    || TransAmt != LastTransAmt
                    || I == K - 1)
                {
                    //  if (K == 1) FirstSeqNo = OriginSeqNo;

                    if (CheckAccNo == true)
                    {
                        if (AccNo != LastAccNo & Matched_Characters.Trim() == LastMatched_Characters.Trim())
                        {
                            NotSameAccount = true;
                        }
                    }

                    if (TransAmt != LastTransAmt & Matched_Characters.Trim() == LastMatched_Characters.Trim())
                    {
                        NotSameAmt = true;
                    }

                    if ((LastMatched_Characters.Trim() != "" & Matched_Characters.Trim() != LastMatched_Characters.Trim())) //  
                    {

                        char[] WMask = { '0', '0' };
                        if (FoundInOne == true) WMask[0] = '1';
                        if (FoundInTwo == true) WMask[1] = '1';
                        //if (ZeroInThree == true) WMask[2] = '0';

                        string WWMask = new string(WMask);

                        if (NotSameAccount == true || NotSameAmt == true)
                        {

                            WWMask = "AA";
                            //if (NotSameAccount == true)
                            if (ShowMessage == true & Environment.UserInteractive)
                            {
                                MessageBox.Show("TerminalId = " + TerminalId + Environment.NewLine
                                                                + "LastTerminalId = " + LastTerminalId + Environment.NewLine
                                                                + "LastAccNo = " + LastAccNo + Environment.NewLine
                                                                + "LastTransAmt  = " + LastTransAmt + Environment.NewLine
                                                                + "");
                            }

                        }

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
                        LastTerminalId = TerminalId;
                        LastAccNo = AccNo;
                        LastTraceNo = TraceNo;
                        LastRRNumber = RRNumber;
                        LastTransAmt = TransAmt;
                        LastType = Type;

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
                    LastTerminalId = TerminalId;
                    LastAccNo = AccNo;
                    LastTransAmt = TransAmt;
                    LastTraceNo = TraceNo;
                    LastRRNumber = RRNumber;
                    LastType = Type;
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
                Type = (int)TableUnMatched.Rows[I]["Type"];
                DublInPos = (int)TableUnMatched.Rows[I]["DublInPos"];
                InPos = (int)TableUnMatched.Rows[I]["InPos"];
                NotInPos = (int)TableUnMatched.Rows[I]["NotInPos"];
                TerminalId = (string)TableUnMatched.Rows[I]["TerminalId"];
                TraceNo = (int)TableUnMatched.Rows[I]["TraceNo"];
                RRNumber = (string)TableUnMatched.Rows[I]["RRNumber"];
                CardNumber = (string)TableUnMatched.Rows[I]["CardNumber"];
                AccNo = (string)TableUnMatched.Rows[I]["AccNo"];
                TransAmt = (decimal)TableUnMatched.Rows[I]["TransAmt"];
                MatchingCateg = (string)TableUnMatched.Rows[I]["MatchingCateg"];
                RMCycle = (int)TableUnMatched.Rows[I]["RMCycle"];
                Matched_Characters = (string)TableUnMatched.Rows[I]["Matched_Characters"];
                FullDtTm = (DateTime)TableUnMatched.Rows[I]["FullDtTm"];
                ResponseCode = (string)TableUnMatched.Rows[I]["ResponseCode"];
                //string LastAccNo;
                //decimal LastTransAmt;



                if (Matched_Characters.Trim() != LastMatched_Characters.Trim()
                    || AccNo != LastAccNo
                    || TransAmt != LastTransAmt
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


                    if (TransAmt != LastTransAmt & Matched_Characters == LastMatched_Characters)
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

                        // FirstSeqNo = SeqNo;
                        LastMatched_Characters = Matched_Characters;
                        LastTransDate = TransDate;
                        LastTerminalId = TerminalId;
                        LastAccNo = AccNo;
                        LastTraceNo = TraceNo;
                        LastRRNumber = RRNumber;
                        LastTransAmt = TransAmt;
                        LastType = Type;

                        char[] WMask = { '0', '0', '0' };


                        //if (ZeroInOne == false) WMask[0] = '1';

                        //if (ZeroInTwo == false) WMask[1] = '1';

                        //if (ZeroInThree == false) WMask[2] = '1';

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
                    LastTransAmt = TransAmt;
                    LastTraceNo = TraceNo;
                    LastRRNumber = RRNumber;
                    LastType = Type;
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


        // Insert and Update exceptions  
        private void InsertAndUpdateTablesFromOurAtms_AND_JCC(string WWMask)
        {
            // Insert Or Update Footer 
            try
            {
                if (LastTerminalId == "00000008")
                {
                    LastTraceNo = LastTraceNo;
                }
                int SeqOriginRecord = 0;
                string WSelectionCriteria = "";
                bool MasterPOS = false;

                if (WMatchingCateg.Substring(3, 3) == "231"
                    || WMatchingCateg.Substring(3, 3) == "233"
                    || WMatchingCateg.Substring(3, 3) == "272"
                    || WMatchingCateg.Substring(3, 3) == "273"
                    )
                {
                    MasterPOS = true; // MASTER OR MEEZA
                }
                // READ DETAILS FROM OTHER WORKING TABLES 
                if (WWMask == "10"
                    || WWMask == "101"
                    || WWMask == "110"
                    || WWMask == "100"
                    || WWMask == "AA"
                    || WWMask == "AAA"
                    || WWMask == "A0A"
                    )
                {
                    //if (WWMask == "110" & WMatchingCateg == "BDC272")
                    //{

                    //    MessageBox.Show("Testing 110");

                    //}

                    Mpa.RecordFound = false;

                    SelectionCriteria = " WHERE SeqNo = " + SeqNoInOne;


                    Mpa.ReadMatchingTxnsMasterPoolFirstRecordFound(SelectionCriteria, 1);

                    if (Mpa.RecordFound == false)
                    {
                        if (ShowMessage == true & Environment.UserInteractive)
                        {
                            MessageBox.Show("1829 : Issue to be reported to RRDM " + Environment.NewLine
                               + "WWMask = " + WWMask + Environment.NewLine
                               + "SeqNo1=" + SeqNoInOne + Environment.NewLine
                                + "SeqNo2=" + SeqNoInTwo + Environment.NewLine
                                + "SeqNo3=" + SeqNoInThree + Environment.NewLine
                                + "MatchingCateg=" + WMatchingCateg + Environment.NewLine
                                + "WReconcCategoryId=" + WReconcCategoryId
                               );
                        }

                    }

                    // Deal Here with all the POS with Comment = 'OLD_Transaction'
                    if (Mpa.RecordFound == true & MasterPOS == true & Mpa.Comments == "OLD_Transaction")
                    {
                        // UPDATE Mpa and return
                        Mpa.UserId = "";
                        Mpa.ActionByUser = false;
                        Mpa.Authoriser = "";

                        Mpa.SettledRecord = false;

                        Mpa.IsMatchingDone = true;
                        Mpa.Matched = false;
                        if (WWMask == "10")
                        {
                            WWMask = "1X";
                        }
                        if (WWMask == "100")
                        {
                            WWMask = "1XX";
                        }
                        Mpa.MatchMask = WWMask;

                        Mpa.SystemMatchingDtTm = DateTime.Now;

                        Mpa.MatchedType = "Not Matched";

                        Mpa.UnMatchedType = "Old POS";

                        Mpa.Comments = Mpa.Comments;
                           // + Environment.NewLine
                              //         + "POS TXN"; 

                        // Leave here the unique record id 
                        Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId, 1);

                        SeqNoInOne = 0;
                        SeqNoInTwo = 0;
                        SeqNoInThree = 0;

                        return; 
                    }
                    // DO THE OTHERS
                    if (Mpa.RecordFound == true)
                    {
                        // Set The SeqNo of the original record
                        if (FromOwnATMs == true)
                        {
                            // Do nothing
                            if (Mpa.TerminalId == "01901009" & WWMask == "10")
                            {
                                WWMask = WWMask;
                                //trace 171268
                                //RRN 124616171268

                            }
                        }
                        else
                        {
                            // Origin Record not ATM
                            SeqOriginRecord = Mpa.SeqNo01;

                        }

                        // FIND IF REVERSALS
                        bool FoundInTwo_Reversals = false;
                        bool FoundInThree_Reversals = false;
                        if (WWMask.Substring(1, 1) == "0")
                        {
                            if (FromOwnATMs == true)
                            {

                                Mgt.ReadAndFindDateBasedOnDetails_2(SourceTable_B, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                   , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);

                            }
                            else
                            {
                                // NOT ATMS 
                                // Check the same with RRN

                                if (MasterPOS == true)
                                {
                                    Mgt.ReadAndFindIfReversalForMaster_POS_Based_On_RRN(SourceTable_B, WMatchingCateg,
                                            Mpa.RRNumber, Mpa.TransAmount, Mpa.Card_Encrypted);
                                }
                                else
                                {
                                    Mgt.ReadAndFindDateBasedOnDetails_2_Based_On_RRN(SourceTable_B, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                   , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);
                                }


                            }

                            ///if (Mgt.RecordFound == true & Mgt.Processed == true & Mgt.Comment == "Reversals")

                            if (Mgt.RecordFound == true & Mgt.Comment == "Reversals")
                            {
                                // The second position has reversals 
                                FoundInTwo_Reversals = true;

                                // Update the Reversals 
                                if (FromOwnATMs == true)
                                {
                                    // We do update all records to be updated 
                                    Mgt.UpdateReversalsForMask_By_Trace(SourceTable_B, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                   , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, WWMask);
                                }
                                else
                                {

                                    if (MasterPOS == true)
                                    {
                                        Mgt.UpdateReversalsForMask_By_RRN_For_POS(SourceTable_B, WMatchingCateg,
                                            Mpa.RRNumber, Mpa.TransAmount, Mpa.Card_Encrypted, WWMask);
                                    }
                                    else
                                    {
                                        Mgt.UpdateReversalsForMask_By_RRN(SourceTable_B, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                  , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, WWMask);

                                    }

                                }

                            }
                        }
                        if (WWMask.Length == 3)
                        {
                            if (WWMask.Substring(2, 1) == "0")
                            {
                                if (FromOwnATMs == true)
                                {

                                    Mgt.ReadAndFindDateBasedOnDetails_2(SourceTable_C, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                       , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);

                                }
                                else
                                {

                                    // Check the same with RRN

                                    if (MasterPOS == true)
                                    {
                                        Mgt.ReadAndFindIfReversalForMaster_POS_Based_On_RRN(SourceTable_C, WMatchingCateg,
                                                Mpa.RRNumber, Mpa.TransAmount, Mpa.Card_Encrypted);
                                    }
                                    else
                                    {
                                        Mgt.ReadAndFindDateBasedOnDetails_2_Based_On_RRN(SourceTable_C, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                       , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);
                                    }

                                    

                                }
                                if (Mgt.RecordFound == true & Mgt.Processed == true
                                                                                         & Mgt.Comment == "Reversals")
                                {
                                    // The second position has reversals 
                                    FoundInThree_Reversals = true;

                                    // Update the Reversals 
                                    if (FromOwnATMs == true)
                                    {
                                        Mgt.UpdateReversalsForMask_By_Trace(SourceTable_C, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                       , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, WWMask);
                                    }
                                    else
                                    {
                                        if (MasterPOS == true)
                                        {
                                            Mgt.UpdateReversalsForMask_By_RRN_For_POS(SourceTable_C, WMatchingCateg,
                                                Mpa.RRNumber, Mpa.TransAmount, Mpa.Card_Encrypted, WWMask);
                                        }
                                        else
                                        {
                                            Mgt.UpdateReversalsForMask_By_RRN(SourceTable_C, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                      , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, WWMask);

                                        }
                                    }
                                }
                            }
                        }

                        // Check if position Two has reversals
                        //
                        // This is an extra check for deposits 
                        //
                        if (WWMask.Substring(1, 1) == "1")
                        {
                            TableId = SourceTable_B;
                            SelectionCriteria = " WHERE SeqNo = " + SeqNoInTwo;

                            Mgt.ReadTransSpecificFromSpecificTable_Primary(SelectionCriteria, TableId, 1);

                            if (Mgt.RecordFound == true)
                            {

                                if (Mgt.Comment == "Reversals")
                                {
                                    //
                                    // This is the case of Deposits that we have allowed deposits in Matching 
                                    //
                                    FoundInTwo_Reversals = true;

                                }

                            }
                        }
                        //
                        // Apply Reversals ON MASK 
                        //
                        if (FoundInTwo_Reversals == true || FoundInThree_Reversals == true)
                        {
                            if (WWMask == "10" & FoundInTwo_Reversals == true)
                            {
                                // case 10
                                WWMask = "1R";
                            }
                            if (WWMask == "101" & FoundInTwo_Reversals == true)
                            {
                                // case 101
                                WWMask = "1R1";
                            }
                            if (WWMask == "110" & FoundInTwo_Reversals == false & FoundInThree_Reversals == true)
                            {
                                // case 110
                                WWMask = "11R";
                            }
                            if (WWMask == "110" & FoundInTwo_Reversals == true & FoundInThree_Reversals == false)
                            {
                                // case 110
                                WWMask = "1R0";
                            }
                            if (WWMask == "110" & FoundInTwo_Reversals == true & FoundInThree_Reversals == true)
                            {
                                // case 110
                                WWMask = "1RR";
                            }
                            if (WWMask == "100" & (FoundInTwo_Reversals == true || FoundInThree_Reversals == true))
                            {
                                // case 100
                                if (FoundInTwo_Reversals == true & FoundInThree_Reversals == true)
                                {
                                    WWMask = "1RR";
                                }
                                if (FoundInTwo_Reversals == true & FoundInThree_Reversals == false)
                                {
                                    WWMask = "1R0";
                                }
                                if (FoundInTwo_Reversals == false & FoundInThree_Reversals == true)
                                {
                                    WWMask = "10R";
                                }
                            }

                        }
                        //
                        // Check If WWMASK for origin ATMs in second position is R 
                        // If so check if this a dublicateted record that came late
                        // ie check if already processed in Mpa with discrepancies 1R or 1RR
                        // If so make delete record from file 
                        bool Duplicate = false; 
                        if (Duplicate == true)
                        {
                            if ((WWMask.Substring(1, 1) == "R") & FromOwnATMs == true)
                            {
                                Mpa.ReadInPoolTransSpecificToSeeIfInMatchedAndDeleteItFromPrimary(Mpa.TerminalId,
                                           Mpa.TraceNoWithNoEndZero, Mpa.SeqNo);

                                if (Mpa.RecordFound == true)
                                {
                                    // Mpa record was deleted 
                                    SeqNoInOne = 0;
                                    SeqNoInTwo = 0;
                                    SeqNoInThree = 0;
                                    return;
                                }
                            }
                        }
                        

                        //
                        // Check If Matched Before For ATMS
                        // OR Reversals
                        // For Reversals we denote eg as 1RR
                        // For Matched before we consider them as Matched? 
                        // OR Maybe Kill them? 
                       
                        if ((WWMask == "10" || WWMask == "100") & FromOwnATMs == true)
                        {
                            // If still 10 and 100 they might be the Gap journal came for processing 
                            // In such a case we turn them to matched 11 or 111
                            // CheckInMatched == true .... here we skip this because it consumes a lot of time

                            bool CheckInMatched = false; // WE SET IT IN FALSE BECAUSE it is an overhead in searching
                                                         // IT is an over engineering !!!!!!! to be true
                                                         // It searches in Matched where there are millions 
                            if (CheckInMatched == true)
                            {
                                //
                                // If Found in Matched entries then this must be deleted
                                // 
                                Mpa.ReadInPoolTransSpecificToSeeIfInMatchedAndDeleteItFromPrimary(Mpa.TerminalId,
                                          Mpa.TraceNoWithNoEndZero, Mpa.SeqNo);

                                if (Mpa.RecordFound == true)
                                {
                                    // Mpa record was deleted 
                                    SeqNoInOne = 0;
                                    SeqNoInTwo = 0;
                                    SeqNoInThree = 0;
                                    return;
                                }
                                else
                                {
                                    // Continue to next 
                                }
                            }
                            
                            //
                            // OUR ATMS check for LATE Journals 
                            // 
                            bool FoundInTwo = false;
                            bool FoundInThree = false;
                            SeqNoInThree = 0;
                            //
                            // Look to find if there are corresponding records already processed as 01 or 011
                            //
                            TableId = SourceTable_B; // IST 
                            Mgt.ReadAndFindDateBasedOnDetails_3(SourceTable_B, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                   , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);

                            if (Mgt.RecordFound == true & Mgt.Processed == true
                                                            & Mgt.ProcessedAtRMCycle != WRMCycle)
                            {
                                // This the case of late coming Journal
                                // First time was like 011 because record was not matched 
                                // And Now record has come and there is the case of 100 that we have to turn it to 11 or 111

                                FoundInTwo = true;
                                SeqNoInTwo = Mgt.SeqNo;
                            }

                            // Find SeqNo for three
                            if (WWMask == "100") 
                            {
                                // FLEX OR COREBANKING
                                Mgt.ReadAndFindDateBasedOnDetails_3(SourceTable_C, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                                                  , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);
                                if (Mgt.RecordFound == true & Mgt.Processed == true
                                                           & Mgt.ProcessedAtRMCycle != WRMCycle)
                                {
                                    FoundInThree = true;
                                    SeqNoInThree = Mgt.SeqNo;
                                }
                            }

                            //
                            // OUR ATMS check matching for Aging records 
                            // 
                            bool Aging = false;
                            string Panicos = "NO"; // Leave it out- over engineering 
                            // if (Panicos== "YES" AND (WWMask == "10" & FoundInTwo == ) || (WWMask == "100" & FoundInTwo & FoundInThree))
                            if (Panicos == "YES" & (WWMask == "10" || WWMask == "100") & FoundInTwo == false & FoundInThree == false)
                            {
                                // Not found yet.
                                // Check if it aging transaction 
                                FoundInTwo = false;
                                FoundInThree = false;
                                SeqNoInThree = 0;
                                //Mgt.Comment = ""; 
                                //
                                // Look to find if there are corresponding records already processed by Aging 
                                //
                                TableId = SourceTable_B_Matched;
                                Mgt.ReadAndFindDateBasedOnDetails_3(TableId, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                       , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);
                                if (Mgt.RecordFound == true)
                                {
                                    if (Mgt.Comment.Length >= 5)
                                    {
                                        if (Mgt.Processed == true
                                                                & Mgt.ProcessedAtRMCycle != WRMCycle & Mgt.Comment.Substring(0, 5) == "Aging")
                                        {
                                            // This the case of late coming Journal
                                            // First time was like 011 because record was not matched 
                                            // And Now record has come and there is the case of 100 that we have to turn it to 11 or 111
                                            Aging = true;
                                            FoundInTwo = true;
                                            SeqNoInTwo = Mgt.SeqNo;
                                        }
                                    }

                                }


                                // Find SeqNo for three
                                if (WWMask == "100")
                                {
                                    TableId = SourceTable_C_Matched;
                                    Mgt.ReadAndFindDateBasedOnDetails_3(TableId, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                                                      , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);

                                    if (Mgt.RecordFound == true)
                                    {
                                        if (Mgt.Comment.Length >= 5)
                                        {
                                            if (Mgt.Processed == true
                                                          & Mgt.ProcessedAtRMCycle != WRMCycle & Mgt.Comment.Substring(0, 5) == "Aging")
                                            {
                                                Aging = true;
                                                FoundInThree = true;
                                                SeqNoInThree = Mgt.SeqNo;
                                            }
                                        }

                                    }

                                }
                            }



                            if ((WWMask == "10" & FoundInTwo) || (WWMask == "100" & FoundInTwo & FoundInThree))
                            {

                                // Make them Matched 
                                // Because it was found before in other cycle 
                                // We do not noted as 1P 
                                Mpa.UserId = "";
                                Mpa.ActionByUser = false;
                                Mpa.Authoriser = "";

                                Mpa.SettledRecord = true;

                                Mpa.IsMatchingDone = true;
                                Mpa.Matched = true;

                                if (WWMask == "10")
                                {
                                    Mpa.MatchMask = "11";
                                    if (FoundInTwo_Reversals == true)
                                    {
                                        Mpa.MatchMask = "1R";
                                    }
                                }

                                if (WWMask == "100")
                                {
                                    Mpa.MatchMask = "111";

                                    // Handle Reversals
                                    if (FoundInTwo_Reversals == true & FoundInTwo_Reversals == false)
                                    {
                                        Mpa.MatchMask = "1R1";
                                    }
                                    if (FoundInTwo_Reversals == true & FoundInThree_Reversals == true)
                                    {
                                        Mpa.MatchMask = "1RR";
                                    }
                                    if (FoundInTwo_Reversals == false & FoundInThree_Reversals == true)
                                    {
                                        Mpa.MatchMask = "11R";
                                    }
                                }

                                Mpa.SystemMatchingDtTm = DateTime.Now;

                                Mpa.MatchedType = "Late Matched";

                                Mpa.UnMatchedType = "N/A";

                                // Leave here the unique record id 
                                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId, 1);

                                // UPDATE IST RECORD AS MATCHED

                                if (Aging == true)
                                {
                                    Mgt.UpdateTablesRecordAsMatchedBySeqNo(SourceTable_B_Matched, SeqNoInTwo, Mpa.MatchMask, "Late_Matched",
                                                                           Mpa.MatchingAtRMCycle);
                                }
                                else
                                {
                                    Mgt.UpdateTablesRecordAsMatchedBySeqNo(SourceTable_B, SeqNoInTwo, Mpa.MatchMask, "Late_Matched",
                                                                            Mpa.MatchingAtRMCycle);
                                }

                                if (WWMask == "100")
                                {
                                    // UPDATE FLEX RECORD AS MATCHED
                                    if (Aging == true)
                                    {
                                        Mgt.UpdateTablesRecordAsMatchedBySeqNo(SourceTable_C_Matched, SeqNoInThree, Mpa.MatchMask, "Late_Matched",
                                                                          Mpa.MatchingAtRMCycle);
                                    }
                                    else
                                    {
                                        Mgt.UpdateTablesRecordAsMatchedBySeqNo(SourceTable_C, SeqNoInThree, Mpa.MatchMask, "Late_Matched",
                                                                           Mpa.MatchingAtRMCycle);
                                    }

                                }

                                if (Aging == false)
                                {
                                    string TempSelect = " WHERE SeqNo=" + SeqNoInTwo;
                                    Mgt.ReadTable_BySelectionCriteria(SourceTable_B, "", TempSelect, 1);

                                    // FIND AND UPDATE OLD UNMATCHED RECORD
                                    if (Mgt.RecordFound = true)
                                        Mpa.ReadInPoolTransSpecificDuringMatching_5(Mgt.MatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                                , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo, 1);

                                    if (Mpa.RecordFound == true)
                                    {
                                        // Update Mpa Comment by SeqNo 
                                        Mpa.Comments = "Recovered_Gap";
                                        Mpa.UnMatchedType = "UnMatched" + ".. But recovered at Cycle.." + WRMCycle.ToString();
                                        TempSelect = " WHERE SeqNo = " + Mpa.SeqNo;
                                        Mpa.UpdateMatchingTxnsMasterPoolATMsComments(TempSelect, 1);
                                    }
                                }
                            }
                            else
                            {
                                Mpa.UserId = "";
                                Mpa.ActionByUser = false;
                                Mpa.Authoriser = "";

                                Mpa.SettledRecord = false;

                                Mpa.IsMatchingDone = true;
                                Mpa.Matched = false;

                                Mpa.MatchMask = WWMask;

                                Mpa.SystemMatchingDtTm = DateTime.Now;

                                Mpa.MatchedType = "Not Matched";

                                Mpa.UnMatchedType = "No Specific";

                                // if (WWMask.Substring(1, 1) == "0")
                                if (WWMask == "10")
                                {
                                    Mpa.Comments = "Not Valid Record in IST";
                                }
                                if (WWMask == "100")
                                {
                                    if (FoundInTwo)
                                    {
                                        WWMask = "110";
                                        Mpa.Comments = "Valid in IST BUT Not Valid In Flex";
                                    }
                                    if (FoundInThree)
                                    {
                                        WWMask = "101";
                                        Mpa.Comments = "Not Valid in IST BUT Valid In Flex";
                                    }

                                    if (FoundInTwo == false)
                                    {
                                        Mpa.Comments = "Not Valid Record in IST";
                                    }

                                }
                                // Leave here the unique record id 
                                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId, 1);
                            }

                        }
                        else
                        {
                            // NOT OUR ATMs
                            // FIND IF PREVIOUS PROCESSED FOR Cases where we matched per Settlement date
                            // Parameter 821

                            bool SET_TO_SET = false;

                            string ParId = "821";

                            Gp.ReadParametersSpecificNm(WOperator, ParId, WMatchingCateg);

                            if (Gp.RecordFound == true || WMatchingCateg == PRX + "240")
                            {
                                //
                                // Here we are setting Credit card too = BDC240
                                //
                                //if (Mpa.RRNumber == "227210280407")
                                //{
                                //    ParId = "821"; 
                                //}
                                SET_TO_SET = true;
                                // 
                            }

                            if (SET_TO_SET == true & (WWMask == "10" || WWMask == "100")
                                & FoundInTwo_Reversals == false & FoundInThree_Reversals == false)
                            {
                                bool FoundInTwo_Processed = false;
                                bool FoundInThree_Processed = false;
                                // FIND OUT IF WERE PROCESSED BEFORE and It is found in the Unmatched 
                                if (WWMask == "10" || WWMask == "100")
                                {
                                    TableId = SourceTable_B;
                                    Mgt.ReadAndFindDateBasedOnDetails_2_Based_On_RRN_P_type(SourceTable_B, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                   , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);

                                    if (Mgt.RecordFound == true & Mgt.Processed == true
                                                                    & Mgt.ProcessedAtRMCycle != WRMCycle)
                                    {
                                        // This the case of late coming Journal
                                        FoundInTwo_Processed = true;
                                        SeqNoInTwo = Mgt.SeqNo;
                                    }
                                    // Find SeqNo for three
                                    if (WWMask == "100")
                                    {
                                        Mgt.ReadAndFindDateBasedOnDetails_2_Based_On_RRN_P_type(SourceTable_C, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                   , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);

                                        if (Mgt.RecordFound == true & Mgt.Processed == true
                                                                   & Mgt.ProcessedAtRMCycle != WRMCycle)
                                        {
                                            FoundInThree_Processed = true;
                                            SeqNoInThree = Mgt.SeqNo;
                                        }
                                    }
                                }
                                // APPLY ON MASK 
                                //
                                // Apply Processed ON MASK 
                                //
                                if (FoundInTwo_Processed == true || FoundInThree_Processed == true)
                                {
                                    if (WWMask == "10" & FoundInTwo_Processed == true)
                                    {
                                        // case 10
                                        WWMask = "1P";
                                    }

                                    if (WWMask == "100" & (FoundInTwo_Processed == true || FoundInThree_Processed == true))
                                    {
                                        // case 100
                                        if (FoundInTwo_Processed == true & FoundInThree_Processed == true)
                                        {
                                            WWMask = "1PP";
                                        }
                                        if (FoundInTwo_Processed == true & FoundInThree_Processed == false)
                                        {
                                            WWMask = "1P0";
                                        }
                                        if (FoundInTwo_Processed == false & FoundInThree_Processed == true)
                                        {
                                            WWMask = "10P";
                                        }
                                    }
                                }
                                // ALL The Rest 
                                Mpa.UserId = "";
                                Mpa.ActionByUser = false;
                                Mpa.Authoriser = "";

                                Mpa.SettledRecord = false;

                                Mpa.IsMatchingDone = true;
                                Mpa.Matched = false;
                                Mpa.MatchMask = WWMask;
                                Mpa.SystemMatchingDtTm = DateTime.Now;

                                Mpa.MatchedType = "Not Matched";

                                if (FoundInTwo_Processed == true || FoundInThree_Processed == true)
                                {
                                    Mpa.UnMatchedType = "Record Previously Processed At Cycle:"
                                            + Mgt.ProcessedAtRMCycle.ToString();
                                }

                                if (MatchingCateg == "BDC240" & WWMask == "10" & Mpa.TransDate > MinMaxDt_02)
                                {
                                    // This is Credit card request by Bank the Credit card file to contain all 
                                    // But the IST to contain up to MinMaxDt_02 = 13:00
                                    Mpa.UnMatchedType = "Unmatched because the record has time greater than fixed 13:00 hour."
                                                        //+ "Second file contains records up to 13:00."
                                                        ;
                                }


                                // Leave here the unique record id 
                                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId, 1);
                            }
                            else
                            {
                                // We have some cases in Meeza POS where the RRN is not the same as in IST
                                // For this reason we try to check against AUTHNUMB
                                //bool MeezaPOS = false;

                                //if (WMatchingCateg.Substring(3, 3) == "272" || WMatchingCateg.Substring(3, 3) == "273")
                                //{
                                //    MeezaPOS = true;
                                //}
                                //if (MeezaPOS == true & (WWMask == "10" || WWMask == "100")
                                //       & FoundInTwo_Reversals == false & FoundInThree_Reversals == false)
                                //{
                                // POS CASE
                                // We want to check if exist based on Auth_Number 
                                // If Found we update as 11 or 111
                                //bool FoundInTwo_Based_On_Auth_Number = false;
                                //bool FoundInThree_On_Auth_Number = false;
                                //// FIND OUT IF WERE PROCESSED BEFORE and It is found in the Unmatched 
                                //if (WWMask == "10" || WWMask == "100")
                                //{
                                //    TableId = SourceTable_B;
                                //    Mgt.ReadAndFindDateBasedOnDetails_2_Based_On_RRN_AUTH_type(SourceTable_B, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                //                                   , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);

                                //    if (Mgt.RecordFound == true & Mgt.Processed == true
                                //                                    & Mgt.ProcessedAtRMCycle != WRMCycle)
                                //    {
                                //        // This the case of late coming Journal
                                //        FoundInTwo_Based_On_Auth_Number = true;
                                //        SeqNoInTwo = Mgt.SeqNo;
                                //    }
                                //    // Find SeqNo for three
                                //    if (WWMask == "100")
                                //    {
                                //        Mgt.ReadAndFindDateBasedOnDetails_2_Based_On_RRN_AUTH_type(SourceTable_C, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                //                                   , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);

                                //        if (Mgt.RecordFound == true & Mgt.Processed == true
                                //                                   & Mgt.ProcessedAtRMCycle != WRMCycle)
                                //        {
                                //            FoundInThree_On_Auth_Number = true;
                                //            SeqNoInThree = Mgt.SeqNo;
                                //        }
                                //    }
                                //}
                                //// APPLY ON MASK 
                                ////
                                //// Apply Processed ON MASK 
                                ////
                                //if (FoundInTwo_Based_On_Auth_Number == true || FoundInThree_On_Auth_Number == true)
                                //{
                                //    if (WWMask == "10" & FoundInTwo_Based_On_Auth_Number == true)
                                //    {
                                //        // case 10
                                //        WWMask = "1P";
                                //    }

                                //    if (WWMask == "100" & (FoundInTwo_Based_On_Auth_Number == true || FoundInThree_On_Auth_Number == true))
                                //    {
                                //        // case 100
                                //        if (FoundInTwo_Based_On_Auth_Number == true & FoundInThree_On_Auth_Number == true)
                                //        {
                                //            WWMask = "1PP";
                                //        }
                                //        if (FoundInTwo_Based_On_Auth_Number == true & FoundInThree_On_Auth_Number == false)
                                //        {
                                //            WWMask = "1P0";
                                //        }
                                //        if (FoundInTwo_Based_On_Auth_Number == false & FoundInThree_On_Auth_Number == true)
                                //        {
                                //            WWMask = "10P";
                                //        }
                                //    }
                                //}
                                //// ALL The Rest 
                                //Mpa.UserId = "";
                                //Mpa.ActionByUser = false;
                                //Mpa.Authoriser = "";

                                //Mpa.SettledRecord = false;

                                //Mpa.IsMatchingDone = true;
                                //Mpa.Matched = false;
                                //Mpa.MatchMask = WWMask;
                                //Mpa.SystemMatchingDtTm = DateTime.Now;

                                //Mpa.MatchedType = "Not Matched";

                                //if (FoundInTwo_Based_On_Auth_Number == true || FoundInThree_On_Auth_Number == true)
                                //{
                                //    Mpa.UnMatchedType = "Record Previously Processed At Cycle:"
                                //            + Mgt.ProcessedAtRMCycle.ToString();
                                //}

                                //// Leave here the unique record id 
                                //Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId, 1);


                                // ALL The Rest 
                                Mpa.UserId = "";
                                Mpa.ActionByUser = false;
                                Mpa.Authoriser = "";

                                Mpa.SettledRecord = false;

                                Mpa.IsMatchingDone = true;
                                Mpa.Matched = false;
                                Mpa.MatchMask = WWMask;  // Here  we have if it is reversal too! For not our ATMs
                                Mpa.SystemMatchingDtTm = DateTime.Now;

                                Mpa.MatchedType = "Not Matched";

                                if (WWMask == "AA" || WWMask == "AAA" || WWMask == "A0A")
                                {

                                    if (NotSameAccount == true)
                                    {
                                        Mpa.UnMatchedType = "Alert-Not Same Account";
                                    }
                                    if (NotSameAmt == true)
                                    {
                                        Mpa.UnMatchedType = "Alert-Not Same Amount";
                                    }
                                    if (NotSameAccount == true & NotSameAmt == true)
                                    {
                                        Mpa.UnMatchedType = "Alert-Not Same Account And Amount";
                                    }

                                }
                                else
                                {
                                    Mpa.UnMatchedType = "No Specific";
                                }

                                // Leave here the unique record id 
                                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId, 1);

                            }

                        }


                        if (FromOwnATMs == true)
                        {
                            Mpa.SeqNo01 = SeqNoInOne;
                        }
                        else
                        {
                            Mpa.SeqNo01 = SeqOriginRecord;
                        }


                        Mpa.SeqNo02 = SeqNoInTwo;

                        Mpa.SeqNo03 = SeqNoInThree;

                        Mpa.UpdateSeqNosInMpa(WOperator, Mpa.UniqueRecordId, 1);

                        if (SeqOriginRecord > 0 & FromOwnATMs == false & Mpa.Matched == false)
                        {
                            // Update origin record as not matched. 
                            // 
                            Mcsf.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);
                            Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                            string OriginSourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                            Mgt.UpdateRecordForMaskBySeqNumber(OriginSourceTable_A, SeqOriginRecord, WWMask);

                        }

                        if (SeqNoInTwo > 0 & Mpa.Matched == false)
                        {
                            // Update record as not matched. 
                            // Update SourceTable_B

                            Mgt.UpdateRecordForMaskBySeqNumber(SourceTable_B, SeqNoInTwo, WWMask);

                        }
                        if (SeqNoInThree > 0 & Mpa.Matched == false & NumberOfTables == 3)
                        {
                            // Update record as not matched. 
                            // Update SourceTable_C

                            Mgt.UpdateRecordForMaskBySeqNumber(SourceTable_C, SeqNoInThree, WWMask);
                        }

                    }
                    SeqNoInOne = 0;
                    SeqNoInTwo = 0;
                    SeqNoInThree = 0;
                }

                //
                // First Journal record Missing 
                //  
                //
                if (WWMask == "01"
                      || WWMask == "011"
                      || WWMask == "010"
                      || WWMask == "001"
                      || WWMask == "0AA"
                      )
                {
                    //bool PassThrough = false;
                    bool FoundInOne_Reversals = false;
                    bool FoundInTwo_Reversals = false;
                    bool FoundInThree_Reversals = false;

                    string WReversalCard = "";

                    if (WWMask == "01")
                    {
                        WWMask = "01";
                    }
                    if (WWMask.Substring(1, 1) == "1")
                    {
                        TableId = SourceTable_B;
                        SelectionCriteria = " WHERE SeqNo = " + SeqNoInTwo;

                        Mgt.ReadTransSpecificFromSpecificTable_Primary(SelectionCriteria, TableId, 1);

                        if (Mgt.RecordFound == true)
                        {
                            if (WWMask.Substring(1, 1) == "1" & Mgt.Comment == "Reversals")
                            {
                                //
                                // This is the case of Deposits that we have allowed deposits in Matching 
                                //
                                FoundInTwo_Reversals = true;
                            }
                        }

                        if (POS_Type == true)
                        {
                            // Update target file
                            // Leave this here
                            bool T_Processed = true;
                            Mgt.UpdateProcessedBySeqNo(TableId, SeqNoInTwo, T_Processed, WRMCycle);
                        }
                    }
                    if (WWMask.Length == 3)
                    //    if (WWMask.Length == 3 & WWMask.Substring(1, 1) == "0")
                    {
                        if (WWMask.Substring(2, 1) == "1")
                        {
                            // Insert for deposits reversals
                            string TSelectionCriteria = " WHERE SeqNo = " + SeqNoInThree;
                            string TTableId = SourceTable_C;
                            Mgt.ReadTransSpecificFromSpecificTable_Primary(TSelectionCriteria, TTableId, 1);

                            if (Mgt.RecordFound == true)
                            {
                                if (WWMask.Substring(2, 1) == "1" & Mgt.Comment == "Reversals")
                                {
                                    //
                                    // This is the case of Deposits that we have allowed deposits in Matching 
                                    //
                                    FoundInThree_Reversals = true;
                                }
                            }
                            if (WWMask.Substring(1, 1) == "0")
                            {
                                SelectionCriteria = " WHERE SeqNo = " + SeqNoInThree;
                                TableId = SourceTable_C;
                            }

                            if (POS_Type == true)
                            {
                                // Update target file
                                // Leave this here
                                bool T_Processed = true;
                                Mgt.UpdateProcessedBySeqNo(SourceTable_C, SeqNoInThree, T_Processed, WRMCycle);
                            }
                        }
                    }

                    // GET INFO from corresonding fields 
                    // BASED ON SEQUENCE 

                    //SelectionCriteria = " WHERE SeqNo = " + SeqNoInThree;
                    //TableId = SourceTable_C;

                    Mgt.ReadTransSpecificFromSpecificTable_Primary(SelectionCriteria, TableId, 1);

                    //if (Mgt.TerminalId == "01905505")
                    //{
                    //    MessageBox.Show("This is the one ");
                    //}

                    if (Mgt.RecordFound == false)
                    {
                        if (ShowMessage == true & Environment.UserInteractive)
                        {
                            MessageBox.Show("1833 : Check for System Error AND TableId " + TableId + Environment.NewLine
                                                       + "AND Selection Criteria.." + SelectionCriteria + TableId + Environment.NewLine
                                                       + "WWMask.. " + WWMask + Environment.NewLine
                                                       + "Category: " + WReconcCategoryId + Environment.NewLine
                                                       + LastMatched_Characters + Environment.NewLine
                                                       + Matched_Characters + Environment.NewLine
                                                       );
                        }

                    }
                    string OriginSourceTable_A = "";
                    if (Mgt.RecordFound == true)
                    {
                        // FIND IF REVERSALS


                        if (WWMask.Substring(0, 1) == "0")
                        {
                            if (FromOwnATMs == true)
                            {
                                // OK 
                                // No RECORDS TO FIND REVERSALS
                                //  Mgt_BDC.ReadAndFindDateBasedOnDetails_2(SourceTable_A, WMatchingCateg, Mgt_BDC.TerminalId, Mgt_BDC.TransDate.Date
                                //                                     , Mgt_BDC.TraceNo, Mgt_BDC.TransAmt, Mgt_BDC.CardNumber, Mgt_BDC.AccNo);

                            }
                            else
                            {
                                // Check the same with RRN
                                // Not OUR ATMS
                                // Find the origin file 
                                Mcsf.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);
                                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                                OriginSourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                                Mgt.ReadAndFindDateBasedOnDetails_2_Based_On_RRN(OriginSourceTable_A, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);

                                if (Mgt.RecordFound == true & Mgt.Processed == true & Mgt.Comment == "Reversals")
                                {
                                    // The position has reversals 
                                    FoundInOne_Reversals = true;

                                    WReversalCard = Mgt.CardNumber;

                                    // Update the Reversals 
                                    //if (FromOwnATMs == true)
                                    //{
                                    //    Mgt.UpdateReversalsForMask_By_Trace(OriginSourceTable_A, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                    //                                   , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, WWMask);
                                    //}
                                    //else
                                    //{
                                        Mgt.UpdateReversalsForMask_By_RRN(OriginSourceTable_A, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                       , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, WWMask);

                                   // }

                                }

                            }

                        }
                        if (WWMask.Substring(1, 1) == "0")
                        {
                            if (FromOwnATMs == true)
                            {

                                Mgt.ReadAndFindDateBasedOnDetails_2(SourceTable_B, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);



                            }
                            else
                            {
                                // Check the same with RRN
                                Mgt.ReadAndFindDateBasedOnDetails_2_Based_On_RRN(SourceTable_B, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);


                            }


                            if (Mgt.RecordFound == true & Mgt.Processed == true
                                                                                     & Mgt.Comment == "Reversals")
                            {
                                // The second position has reversals 
                                FoundInTwo_Reversals = true;

                                // Update the Reversals 
                                if (FromOwnATMs == true)
                                {
                                    Mgt.UpdateReversalsForMask_By_Trace(SourceTable_B, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, WWMask);
                                }
                                else
                                {
                                    Mgt.UpdateReversalsForMask_By_RRN(SourceTable_B, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                  , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, WWMask);
                                }

                            }
                        }
                        if (WWMask.Length == 3)
                        {
                            if (WWMask.Substring(2, 1) == "0")
                            {
                                if (FromOwnATMs == true)
                                {

                                    Mgt.ReadAndFindDateBasedOnDetails_2(SourceTable_C, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);

                                }
                                else
                                {
                                    // Check the same with RRN
                                    Mgt.ReadAndFindDateBasedOnDetails_2_Based_On_RRN(SourceTable_C, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                    , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);


                                }


                                if (Mgt.RecordFound == true & Mgt.Processed == true
                                                                                         & Mgt.Comment == "Reversals")
                                {
                                    // The second position has reversals 
                                    FoundInThree_Reversals = true;

                                    // Update the Reversals 
                                    if (FromOwnATMs == true)
                                    {
                                        Mgt.UpdateReversalsForMask_By_Trace(SourceTable_C, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                    , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, WWMask);
                                    }
                                    else
                                    {
                                        Mgt.UpdateReversalsForMask_By_RRN(SourceTable_C, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, WWMask);
                                    }

                                }
                            }
                        }

                        //
                        // Apply Reversals ON MASK 
                        //

                        //   if (WWMask == "01"
                        //|| WWMask == "011"
                        //|| WWMask == "010"
                        //|| WWMask == "001"
                        if (FoundInOne_Reversals || FoundInTwo_Reversals || FoundInThree_Reversals)
                        {
                            if (WWMask == "01" & FoundInOne_Reversals == true)
                            {
                                // case
                                WWMask = "R1";
                            }
                            if (WWMask == "01" & FoundInTwo_Reversals == true)
                            {
                                // case of Deposit
                                WWMask = "0R";
                            }
                            if (WWMask == "011" & FoundInOne_Reversals == true)
                            {
                                // case 
                                WWMask = "R11";
                            }

                            if (WWMask == "011" & (FoundInTwo_Reversals == true & FoundInThree_Reversals == true))
                            {
                                // case 
                                WWMask = "0RR";
                            }
                            if (WWMask == "011" & (FoundInTwo_Reversals == true & FoundInThree_Reversals == false))
                            {
                                // case 
                                WWMask = "0R1";
                            }
                            if (WWMask == "011" & (FoundInTwo_Reversals == false & FoundInThree_Reversals == true))
                            {
                                // case 
                                WWMask = "01R";
                            }
                            if (WWMask == "010" & (FoundInTwo_Reversals == true & FoundInThree_Reversals == true))
                            {
                                // case of Deposit
                                WWMask = "0RR";
                            }
                            if (WWMask == "010" & (FoundInTwo_Reversals == true))
                            {
                                // case of Deposit

                                WWMask = "0R0";
                            }
                            if (WWMask == "010" & (FoundInOne_Reversals == true || FoundInThree_Reversals == true))
                            {
                                // case 
                                if (FoundInOne_Reversals == true & FoundInThree_Reversals == true)
                                {
                                    WWMask = "R1R";
                                }
                                if (FoundInOne_Reversals == true & FoundInThree_Reversals == false)
                                {
                                    WWMask = "R10";
                                }
                                if (FoundInOne_Reversals == false & FoundInThree_Reversals == true)
                                {
                                    WWMask = "01R";
                                }
                            }
                            if (WWMask == "001" & (FoundInOne_Reversals == true || FoundInTwo_Reversals == true))
                            {
                                // case 
                                if (FoundInOne_Reversals == true & FoundInTwo_Reversals == true)
                                {
                                    WWMask = "RR1";
                                }
                                if (FoundInOne_Reversals == true & FoundInTwo_Reversals == false)
                                {
                                    WWMask = "R01";
                                }
                                if (FoundInOne_Reversals == false & FoundInTwo_Reversals == true)
                                {
                                    WWMask = "0R1";
                                }
                            }
                        }
                    }

                    if (MatchingCateg == "BDC240")
                    {
                        // Check if previous process
                        // Check master file 
                        //                      WHERE MatchingCateg = 'BDC240' and RRNumber = '227211137823' AND TransAmount = 1000
                        //AND TerminalId = '00000507' AND CardNumber = '440700******1681'
                        string TempSelect = "WHERE MatchingCateg = 'BDC240' and RRNumber ='" + Mgt.RRNumber + "' AND TransAmount =" + Mgt.TransAmt
                            + " AND TerminalId ='" + Mgt.TerminalId + "' AND CardNumber ='" + Mgt.CardNumber + "'";
                        ;
                        Mpa.ReadInPoolTransSpecificBySelectionCriteria(TempSelect, 1); // Search In Primary Area
                        if (Mpa.RecordFound & Mpa.IsMatchingDone == true)
                        {
                            WWMask = "P1";
                        }
                    }

                    //
                    // REFRESH READ
                    //
                    Mgt.ReadTransSpecificFromSpecificTable_Primary(SelectionCriteria, TableId, 1);

                    DateTime Found_SET_DATE = NullPastDate;

                    if (WMatchingCateg == PRX + "215" || WMatchingCateg == PRX + "275")
                    {
                        // Find the previous in Mpa
                        Mpa.ReadInPoolTransLessThanGiven_To_FIND_SET_DATE(WMatchingCateg, Mgt.TransDate, 1);
                        if (Mpa.RecordFound == true)
                        {
                            Found_SET_DATE = Mpa.SET_DATE;
                        }

                    }
                    if (Mgt.RecordFound)
                    {
                        //WReplCycleNo = Mpa.ReadMatchingTxnsMasterPoolFirstRecordLessThanGivenDateTime(Mgt_BDC.TerminalId, Mgt_BDC.TransDate);
                        int WReplCycleNo = 0;
                        // SeqNoTwo = Mgt_BDC.SeqNo;
                        Mpa.OriginalRecordId = Mgt.OriginalRecordId;
                        Mpa.TerminalType = Mgt.TerminalType;
                        //Mpa.OriginFileName = Mgt.OriginFileName;

                        Mpa.LoadedAtRMCycle = Mgt.LoadedAtRMCycle;

                        Mpa.TransTypeAtOrigin = Mgt.TransTypeAtOrigin;

                        Mpa.TerminalId = Mgt.TerminalId;
                        Mpa.TransType = Mgt.TransType;
                        Mpa.TransDescr = Mgt.TransDescr;

                        if (WReversalCard == "")
                        {
                            Mpa.CardNumber = Mgt.CardNumber;
                        }
                        else
                        {
                            Mpa.CardNumber = WReversalCard;
                        }

                        Mpa.AccNumber = Mgt.AccNo;

                        Mpa.TransCurr = Mgt.TransCurr;
                        Mpa.TransAmount = Mgt.TransAmt;
                        // +",[AmtFileBToFileC] " // AMOUNT_EQUIV
                        //Mpa.SpareField = Mgt.AmtFileBToFileC.ToString(); // // AMOUNT_EQUIV

                        Mpa.TransDate = Mgt.TransDate;
                        Mpa.RRNumber = Mgt.RRNumber;
                        Mpa.AUTHNUM = Mgt.AUTHNUM;

                        //ReadWorkingFileBySelectionCriteria(TableId, WSelectionCriteria, LastTransDate);

                        Mpa.OriginFileName = "Term:" + Mgt.TerminalId + "";

                        Mpa.LoadedAtRMCycle = WRMCycle;

                        Mpa.MatchingAtRMCycle = WRMCycle;

                        Mpa.MatchingCateg = WMatchingCateg;

                        if (FromOwnATMs == true & No_Group_Type == false)
                        {
                            RRDMAtmsClass Ac = new RRDMAtmsClass();
                            Ac.ReadAtm(Mgt.TerminalId);

                            Mpa.RMCateg = "RECATMS-" + Ac.AtmsReconcGroup.ToString();
                        }
                        else
                        {
                            Mpa.RMCateg = WMatchingCateg;
                        }

                        Mpa.UniqueRecordId = GetNextValue(connectionStringATMs);

                        Mc.ReadMatchingCategorybyActiveCategId(WOperator, WMatchingCateg);
                        Mpa.Origin = Mc.Origin;

                        Mpa.TargetSystem = 6;

                        Mpa.Product = "";
                        Mpa.CostCentre = "";

                        Mpa.DepCount = 0;

                        // TRACE
                        Mpa.TraceNoWithNoEndZero = Mgt.TraceNo;
                        Mpa.AtmTraceNo = Mgt.TraceNo * 10;
                        Mpa.MasterTraceNo = Mgt.TraceNo * 10;

                        Mpa.IsOwnCard = false;

                        RRDMMatchingCategoriesVsBINs Mcb = new RRDMMatchingCategoriesVsBINs();
                        if (Mpa.CardNumber != "")
                        {
                            Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, 0, Mpa.CardNumber.Substring(0, 6), "Our Atms", 12);
                            if (Mcb.RecordFound)
                            {
                                // Own Card

                                Mpa.IsOwnCard = true;
                            }
                            else
                            {
                                // Other Card 

                                Mpa.IsOwnCard = false;
                            }
                        }
                        else
                        {
                            Mpa.IsOwnCard = false;
                        }


                        Mpa.ResponseCode = "0";
                        Mpa.SpareField = "";
                        Mpa.Comments = "";

                        Mpa.IsMatchingDone = true;
                        Mpa.Matched = false;
                        Mpa.MatchMask = WWMask;
                        Mpa.SystemMatchingDtTm = DateTime.Now;
                        Mpa.MatchedType = "No Match";
                        if (WWMask == "0AA")
                        {

                            if (NotSameAccount == true)
                            {
                                Mpa.UnMatchedType = "Alert-Not Same Account";
                            }
                            if (NotSameAmt == true)
                            {
                                Mpa.UnMatchedType = "Alert-Not Same Amount";
                            }
                            if (NotSameAccount == true & NotSameAmt == true)
                            {
                                Mpa.UnMatchedType = "Alert-Not Same Account And Amount";
                            }

                        }
                        else
                        {
                            Mpa.UnMatchedType = "Not Matched";
                        }

                        Mpa.UnMatchedType = "";
                        Mpa.MetaExceptionId = 0;
                        Mpa.MetaExceptionNo = 0;
                        Mpa.FastTrack = false;
                        Mpa.ActionByUser = false;
                        Mpa.UserId = "";
                        Mpa.Authoriser = "";
                        Mpa.AuthoriserDtTm = DateTime.Now;
                        Mpa.ActionType = "";
                        Mpa.NotInJournal = true;
                        Mpa.WaitingForUpdating = false;
                        Mpa.SettledRecord = false;
                        Mpa.Operator = WOperator;

                        Mpa.FileId01 = Mcsf.SourceFileNameA;
                        Mpa.SeqNo01 = SeqNoInOne;
                        Mpa.FileId02 = Mcsf.SourceFileNameB;
                        Mpa.SeqNo02 = SeqNoInTwo;
                        Mpa.FileId03 = Mcsf.SourceFileNameC;
                        if (NumberOfTables == 3)
                        {
                            Mpa.SeqNo03 = SeqNoInThree;
                        }

                        Mpa.FileId04 = Mcsf.SourceFileNameD;
                        Mpa.SeqNo04 = 0;
                        Mpa.FileId05 = "";
                        Mpa.SeqNo05 = 0;
                        Mpa.FileId06 = "";
                        Mpa.SeqNo06 = 0;

                        Mpa.Card_Encrypted = Mgt.Card_Encrypted;
                        Mpa.TXNSRC = Mgt.TXNSRC;
                        Mpa.TXNDEST = Mgt.TXNDEST;

                        Mpa.ACCEPTOR_ID = Mgt.ACCEPTOR_ID;
                        Mpa.ACCEPTORNAME = Mgt.ACCEPTORNAME;
                        Mpa.CAP_DATE = Mgt.CAP_DATE;
                        //if (WMatchingCateg == "BDC210")
                        //{
                        //    Mpa.TXNSRC = Mgt.TXNSRC;
                        //}
                        if (Mgt.SET_DATE == NullPastDate)
                        {
                            if (Found_SET_DATE == NullPastDate)
                            {
                                Mpa.SET_DATE = Mgt.CAP_DATE; // same as for BDC210 
                            }
                            else
                            {
                                Mpa.SET_DATE = Found_SET_DATE;
                            }
                        }
                        else
                        {
                            Mpa.SET_DATE = Mgt.SET_DATE;
                        }

                        // Find the right (proper cycle number)

                        Mpa.ReplCycleNo = WReplCycleNo; // THIS CYCLE NO WAS FOUND FROM PREVIOUS TRACE ... See Code Above

                        int NewSeqNo = Mpa.InsertTransMasterPoolATMs(Mpa.Operator);

                        // Find Out the Replenishment Cycle No by using Last Trace Number
                        // UPDATE INSERTED
                        SelectionCriteria = " WHERE  SeqNo =" + NewSeqNo;
                        Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

                        Mpa.IsMatchingDone = true;
                        Mpa.Matched = false;
                        Mpa.MatchMask = WWMask;
                        Mpa.MatchedType = "by System";
                        Mpa.SystemMatchingDtTm = DateTime.Now;

                        //Mpa.TraceNoWithNoEndZero = LastTraceNo;

                        Mpa.UserId = "";
                        Mpa.ActionByUser = false;
                        Mpa.Authoriser = "";

                        Mpa.NotInJournal = true;

                        Mpa.SettledRecord = false;

                        Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId, 1);

                        // UPDATE RECORDS Involved 

                        if (SeqNoInTwo > 0 & Mpa.Matched == false)
                        {
                            // Update record as not matched. 
                            // Update SourceTable_B

                            Mgt.UpdateRecordForMaskBySeqNumber(SourceTable_B, SeqNoInTwo, Mpa.MatchMask);

                        }
                        if (SeqNoInThree > 0 & Mpa.Matched == false & NumberOfTables == 3)
                        {
                            // Update record as not matched. 
                            // Update SourceTable_C

                            Mgt.UpdateRecordForMaskBySeqNumber(SourceTable_C, SeqNoInThree, Mpa.MatchMask);
                        }
                    }


                    SeqNoInOne = 0;
                    SeqNoInTwo = 0;
                    SeqNoInThree = 0;

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
            + " FROM [ATMS].[dbo].[WReport97_ForMatching] " // leave it as is 
            + " WHERE Type = 1 AND UserId = @UserId "              /* Dublicate  */
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
                Type = (int)TableUnMatched.Rows[I]["Type"];
                DublInPos = (int)TableUnMatched.Rows[I]["DublInPos"];
                InPos = (int)TableUnMatched.Rows[I]["InPos"];
                NotInPos = (int)TableUnMatched.Rows[I]["NotInPos"];
                TerminalId = (string)TableUnMatched.Rows[I]["TerminalId"];
                TraceNo = (int)TableUnMatched.Rows[I]["TraceNo"];
                RRNumber = (string)TableUnMatched.Rows[I]["RRNumber"];
                CardNumber = (string)TableUnMatched.Rows[I]["CardNumber"];
                AccNo = (string)TableUnMatched.Rows[I]["AccNo"];
                TransAmt = (decimal)TableUnMatched.Rows[I]["TransAmt"];
                MatchingCateg = (string)TableUnMatched.Rows[I]["MatchingCateg"];
                RMCycle = (int)TableUnMatched.Rows[I]["RMCycle"];
                Matched_Characters = (string)TableUnMatched.Rows[I]["Matched_Characters"];
                FullDtTm = (DateTime)TableUnMatched.Rows[I]["FullDtTm"];
                ResponseCode = (string)TableUnMatched.Rows[I]["ResponseCode"];

                //if (RRNumber == "011314331195")
                //{
                //    MessageBox.Show("THIS IS THE ONE");
                //}

                if ((LastMatched_Characters != "" & Matched_Characters.Trim() != LastMatched_Characters.Trim()) || I == K - 1)
                {
                    //if (K == 1) FirstSeqNo = SeqNo;

                    if ((Matched_Characters != "" & Matched_Characters.Trim() != LastMatched_Characters.Trim())) //    
                    {
                        // Do action for previous 

                        // OUR 
                        UpdateDuplicatesInPrimaryTableFromOurATMs_And_JCC();

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
                        LastTerminalId = TerminalId;
                        //LastAccNo = AccNo;
                        LastTransAmt = TransAmt;
                        LastType = Type;

                        // OUR ATMs
                        UpdateDuplicatesInPrimaryTableFromOurATMs_And_JCC();

                        I++;

                        continue;


                    }



                }
                //else
                //{
                //    //// TO COVER ALL DUBLICATE RECORDS IN MPA
                //    //if ((LastMatched_Characters != "" & Matched_Characters == LastMatched_Characters)) //    
                //    //{
                //    //    if (DublInPos == 1)
                //    //    {
                //    //        CounterDublInPosOne = CounterDublInPosOne + 1;
                //    //        UpdateDuplicatesInPrimaryTableFromOurATMs_And_JCC();
                //    //    }

                //    //}
                //}

                //if (I != K - 1) FirstSeqNo = OriginSeqNo;
                LastMatched_Characters = Matched_Characters;
                LastTransDate = TransDate;
                LastTerminalId = TerminalId;
                //LastAccNo = AccNo;
                LastTransAmt = TransAmt;
                LastType = Type;

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
        // Update Primary Table with dublicates - FROM Our ATMs 
        //
        private void UpdateDuplicatesInPrimaryTableFromOurATMs_And_JCC()
        {
            // UPDATE Mpa
            //
            string SelectionCriteria2 = "";
            int SeqNoInTwoCount = 0;

            Mpa.RecordFound = false;

            if (SeqNoInOne > 0)
            {
                SelectionCriteria2 = " WHERE  SeqNo =" + SeqNoInOne;
                Mpa.ReadMatchingTxnsMasterPoolFirstRecordFound(SelectionCriteria2, 1);
                // Find SeqNo of the others 
                if (FromOwnATMs == true)
                {
                    Mgt.ReadAndFindDateBasedOnDetails(SourceTable_B, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                               , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);
                    if (Mgt.RecordFound == true)
                    {
                        SeqNoInTwo = Mgt.SeqNo;
                    }
                    if (TwoAndThreeIsRRN == true)
                    {
                        // ReadAndFindDateBasedOnDetails_RRNumber
                        if (NumberOfTables == 3)
                        {
                            Mgt.ReadAndFindDateBasedOnDetails_RRNumber(SourceTable_C, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                              , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);
                            if (Mgt.RecordFound == true)
                            {
                                SeqNoInThree = Mgt.SeqNo;
                            }
                        }

                    }
                    else
                    {
                        if (NumberOfTables == 3)
                        {
                            Mgt.ReadAndFindDateBasedOnDetails(SourceTable_C, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                                                     , Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);
                            if (Mgt.RecordFound == true)
                            {
                                SeqNoInThree = Mgt.SeqNo;
                            }
                        }
                        // Based on TraceNo 

                    }

                }
                else
                {
                    // NOT OUR ATMS then work with RRNumber 
                    //
                    // System.Windows.Forms.MessageBox.Show("MAKE DEVELOPMENT for SeqNos");
                    Mgt.ReadAndFindDateBasedOnDetails_RRNumber(SourceTable_B, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                               , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);
                    if (Mgt.RecordFound == true)
                    {
                        SeqNoInTwo = Mgt.SeqNo;

                        SeqNoInTwoCount = Mgt.WCount;
                    }
                    if (NumberOfTables == 3)
                    {
                        Mgt.ReadAndFindDateBasedOnDetails_RRNumber(SourceTable_C, WMatchingCateg, Mpa.TerminalId, Mpa.TransDate.Date
                                                            , Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, Mpa.AccNumber);
                        if (Mgt.RecordFound == true)
                        {
                            SeqNoInThree = Mgt.SeqNo;
                        }
                    }

                }
            }
            else
            {
                // Case where SeqNoInOne = 0 
                if (FromOwnATMs == true)
                {
                    if (SeqNoInThree > 0)
                    {
                        // Read third file and then Mpa
                        TableId = SourceTable_C;
                        SelectionCriteria = " WHERE SeqNo = " + SeqNoInThree;

                        Mgt.ReadTransSpecificFromSpecificTable_Primary(SelectionCriteria, TableId, 1);

                        if (TwoAndThreeIsRRN == true)
                        {
                            // ReadAndFindDateBasedOnDetails_RRNumber
                            Mgt.ReadAndFindDateBasedOnDetails_RRNumber(SourceTable_B, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                                         , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);
                            if (Mgt.RecordFound == true)
                            {
                                SeqNoInTwo = Mgt.SeqNo;
                            }
                            // Not RRN two and three
                            Mpa.ReadInPoolTransSpecificDuringMatching_3(WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo, 1);
                            if (Mpa.RecordFound)
                            {
                                SeqNoInOne = Mpa.SeqNo;
                            }
                        }
                        else
                        {
                            // Not RRN two and three
                            Mpa.ReadInPoolTransSpecificDuringMatching_3(WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo, 1);
                            if (Mpa.RecordFound)
                            {
                                SeqNoInOne = Mpa.SeqNo;
                            }
                            Mgt.ReadAndFindDateBasedOnDetails(SourceTable_B, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                    , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);
                            if (Mgt.RecordFound == true)
                            {
                                SeqNoInTwo = Mgt.SeqNo;
                            }
                        }

                    }
                    else
                    {
                        // Second File 

                        //
                        //(SeqNoInTwo > 0)
                        // Read second file and then Mpa
                        TableId = SourceTable_B;
                        SelectionCriteria = " WHERE SeqNo = " + SeqNoInTwo;

                        Mgt.ReadTransSpecificFromSpecificTable_Primary(SelectionCriteria, TableId, 1);

                        Mpa.ReadInPoolTransSpecificDuringMatching_3(WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo, 1);
                        if (Mpa.RecordFound)
                        {
                            SeqNoInOne = Mpa.SeqNo;
                        }

                        if (TwoAndThreeIsRRN == true)
                        {
                            // ReadAndFindDateBasedOnDetails_RRNumber
                            if (NumberOfTables == 3)
                            {
                                Mgt.ReadAndFindDateBasedOnDetails_RRNumber(SourceTable_C, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                                        , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);
                                if (Mgt.RecordFound == true)
                                {
                                    SeqNoInThree = Mgt.SeqNo;
                                }
                            }

                        }
                        else
                        {
                            if (NumberOfTables == 3)
                            {
                                Mgt.ReadAndFindDateBasedOnDetails(SourceTable_C, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                            , Mgt.TraceNo, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);
                                if (Mgt.RecordFound == true)
                                {
                                    SeqNoInThree = Mgt.SeqNo;
                                }
                            }

                        }
                    }
                }
                else
                {
                    // NOT OUR ATMS 

                    // USE RRNumber 
                    //
                    // Not from Our ATMS -- USE RRNumber
                    //
                    if (SeqNoInThree > 0)
                    {
                        // Read third file and then Mpa
                        TableId = SourceTable_C;
                        SelectionCriteria = " WHERE SeqNo = " + SeqNoInThree;

                        Mgt.ReadTransSpecificFromSpecificTable_Primary(SelectionCriteria, TableId, 1);

                        Mpa.ReadInPoolTransSpecificDuringMatching_4(WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                  , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo, 1);
                        if (Mpa.RecordFound)
                        {
                            SeqNoInOne = Mpa.SeqNo;
                        }

                        // Find second 
                        Mgt.ReadAndFindDateBasedOnDetails_RRNumber(SourceTable_B, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                                        , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);
                        if (Mgt.RecordFound == true)
                        {
                            SeqNoInTwo = Mgt.SeqNo;
                        }
                    }
                    else
                    {
                        //(SeqNoInTwo > 0)
                        // Read second file and then Mpa
                        TableId = SourceTable_B;
                        SelectionCriteria = " WHERE SeqNo = " + SeqNoInTwo;

                        Mgt.ReadTransSpecificFromSpecificTable_Primary(SelectionCriteria, TableId, 1);

                        Mpa.ReadInPoolTransSpecificDuringMatching_4(WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                   , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo, 1);

                        if (Mpa.RecordFound)
                        {
                            SeqNoInOne = Mpa.SeqNo;
                        }
                        // Find second 
                        if (NumberOfTables == 3)
                        {
                            Mgt.ReadAndFindDateBasedOnDetails_RRNumber(SourceTable_C, WMatchingCateg, Mgt.TerminalId, Mgt.TransDate.Date
                                                                                     , Mgt.RRNumber, Mgt.TransAmt, Mgt.CardNumber, Mgt.AccNo);
                            if (Mgt.RecordFound == true)
                            {
                                SeqNoInThree = Mgt.SeqNo;
                            }
                        }

                    }

                }

            }


            if (Mpa.RecordFound == false)
            {
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    Message = " 343567: There is an error.. record not found.";

                    System.Windows.Forms.MessageBox.Show(Message);
                }
                return;

            }

            char[] WMask = Mpa.MatchMask.ToCharArray();

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
            //
            // leave the below to cover cases for dublicate in second position 
            //
            //if (WMask[1] == '0' & SeqNoInTwoCount == 1)
            //{
            //    WMask[1] = '1';
            //}
            if (WMask[1] == '1' & SeqNoInTwoCount == 2)
            {
                WMask[1] = '2';
            }
            if (WMask[1] == '1' & SeqNoInTwoCount == 3)
            {
                WMask[1] = '3';
            }

            string WWMask = new string(WMask);

            // Update Footer 

            // UNMATCHED DUE TO DUBLICATES

            Mpa.UserId = "";
            Mpa.ActionByUser = false;
            Mpa.Authoriser = "";

            Mpa.SettledRecord = false;

            Mpa.IsMatchingDone = true;
            Mpa.Matched = false;

            Mpa.MatchMask = WWMask;
            Mpa.SystemMatchingDtTm = DateTime.Now;

            Mpa.MatchedType = "by System";

            Mpa.UnMatchedType = "Dublicate";

            // UPDATE ALL RECORDS
            Mpa.UpdateMatchingTxnsMasterPoolATMsFooterBySelection(Mpa.MatchingCateg
                , Mpa.TerminalId
                , Mpa.TransAmount
                , Mpa.TraceNoWithNoEndZero
                , Mpa.RRNumber
                , Mpa.CardNumber
                , Mpa.TransDate.Date
                , 1);

            // UPDATE OTHER RECORDS with MASK
            if (TwoAndThreeIsRRN == false)
            {
                // Trace Based 
                if (SeqNoInOne > 0 & FromOwnATMs == false)
                {
                    // Update origin record as not matched. 
                    // 
                    Mcsf.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);
                    Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                    string OriginSourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                    Mgt.UpdateBasedOnDetails_Based_Trace(OriginSourceTable_A, WMatchingCateg, Mpa.TerminalId,
                                            Mpa.TransDate.Date,
                                            Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, WWMask);

                }

                if (SeqNoInTwo > 0)
                {
                    // Update record as not matched. 
                    // Update SourceTable_B

                    Mgt.UpdateBasedOnDetails_Based_Trace(SourceTable_B, WMatchingCateg, Mpa.TerminalId,
                                             Mpa.TransDate.Date,
                                             Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, WWMask);

                }
                if (SeqNoInThree > 0 & NumberOfTables == 3)
                {
                    // Update record as not matched. 
                    // Update SourceTable_C
                    Mgt.UpdateBasedOnDetails_Based_Trace(SourceTable_C, WMatchingCateg, Mpa.TerminalId,
                                                             Mpa.TransDate.Date,
                                                             Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.CardNumber, WWMask);
                }

            }
            else
            {
                // RRN BASED

                // Trace Based 
                if (SeqNoInOne > 0 & FromOwnATMs == false)
                {
                    // Update origin record as not matched. 
                    // 
                    Mcsf.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);
                    Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                    string OriginSourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                    Mgt.UpdateBasedOnDetails_Based_RRN(OriginSourceTable_A, WMatchingCateg, Mpa.TerminalId,
                                            Mpa.TransDate.Date,
                                            Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, WWMask);

                }

                if (SeqNoInTwo > 0)
                {
                    // Update record as not matched. 
                    // Update SourceTable_B

                    Mgt.UpdateBasedOnDetails_Based_RRN(SourceTable_B, WMatchingCateg, Mpa.TerminalId,
                                             Mpa.TransDate.Date,
                                             Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, WWMask);

                }
                if (SeqNoInThree > 0 & NumberOfTables == 3)
                {
                    // Update record as not matched. 
                    // Update SourceTable_C
                    Mgt.UpdateBasedOnDetails_Based_RRN(SourceTable_C, WMatchingCateg, Mpa.TerminalId,
                                                             Mpa.TransDate.Date,
                                                             Mpa.RRNumber, Mpa.TransAmount, Mpa.CardNumber, WWMask);
                }

            }



            //Mpa.SeqNo01 = SeqNoInOne;

            //Mpa.SeqNo02 = SeqNoInTwo;

            //Mpa.SeqNo03 = SeqNoInThree;

            //Mpa.UpdateSeqNosInMpa(WOperator, Mpa.UniqueRecordId);
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
            if (FromOwnATMs == true)
            {
                SelectionCriteria = " WHERE  MatchingCateg ='" + InMatchingCateg + "'"
                                        + " AND MatchingAtRMCycle = " + RMCycle
                                        + " AND IsMatchingDone = 1 AND Matched = 0 "
                                        + " Order by TransDate , TraceNoWithNoEndZero";

                SqlString =
                " SELECT  SeqNo,  "
                + " CAST(TransDate AS Date) As Date,"
                + " TraceNoWithNoEndZero,  "
                + " Matched,  "
                + " MatchMask,  "
                + " TerminalId,  "
                + " TransDescr,  "
                + " AccNumber,  "
                + " TransAmount,  "
                + " SettledRecord,  "
                + " Comments  "
                //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                + " FROM " + SourceTable_A
                + SelectionCriteria;

            }
            else
            {
                SelectionCriteria = " WHERE  MatchingCateg ='" + InMatchingCateg + "'"
                                        + " AND MatchingAtRMCycle = " + RMCycle
                                        + " AND IsMatchingDone = 1 AND Matched = 0 "
                                        + " Order by TransDate ";
                SqlString =
                " SELECT  SeqNo,  "
                + " CAST(TransDate AS Date) As Date,"
                + " RRNumber ,  "
                + " Matched,  "
                + " MatchMask,  "
                + " TransDescr,  "
                + " AccNumber,  "
                + " TransAmount,  "
                + " SettledRecord,  "
                + " Comments  "
                //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                + " FROM " + SourceTable_A
                + SelectionCriteria;
            }


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

                    //if (ShowMessage == true & Environment.UserInteractive)
                    //{
                    if (ShowMessage == true & Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                    //}

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


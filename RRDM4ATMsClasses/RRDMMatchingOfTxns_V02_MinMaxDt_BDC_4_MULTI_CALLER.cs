using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;

using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MULTI_CALLER : Logger
    {
        public RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MULTI_CALLER() : base() { }
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

        string SqlString; // Do not delete 

        readonly string connectionStringATMs = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;
        readonly string connectionStringRec = ConfigurationManager.ConnectionStrings
            ["ReconConnectionString"].ConnectionString;

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

    
        RRDMMatchingCategoriesVsSourcesFiles Mcsf = new RRDMMatchingCategoriesVsSourcesFiles();
    
        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MULTI M_Multi = new RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MULTI(); 

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

       // RRDMGasParameters Gp = new RRDMGasParameters();
        RRDM_THREADS Tr = new RRDM_THREADS();
        string SelectionCriteria;

        public int NumberOfMatchedCategories;

        // Initialise for next
        DateTime BeforeCallDtTime; // Use For Performance 
   
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

        bool POS_Type;
        int UnMatchedForWorkingDays;
        int UnMatchedForCalendarDays;

        string PRX;

        bool FromOwnATMs;

        //bool TestingTwoFiles; 

        string WMatchingCateg;
        int WRMCycle;
        int WGroupId;
        string WReconcCategoryId;
        string WSignedId;
        string WOperator;   
        //
        // Make Matching with Categories that are ready to be matched
        // 
        public void MatchReadyCategoriesUpdateForAtms_Thread_MULTI_Caller(string InOperator, string InSignedId,
                                       int InRMCycle)
        {
            WSignedId = InSignedId; 
            CategForMatch = "";

            WOperator = InOperator;
            WRMCycle = InRMCycle; 
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
            // Update MinMax For ATMs 
            M_Multi.FIND_MinMaxFor_ALL_ATMS(WOperator, InSignedId, InRMCycle); 

            // 
            Mc.ReadMatchingCategoriesAndFillTable(WOperator, "ALL");

            WMatchingCateg = "";

            bool ReadyCat;
            //
            // Clear Thread Table
            //
            Tr.TruncateTempThreadsTable();

           // MessageBox.Show("Multithreading Matching Starts");

            // LOOP FOR Matching Categories

            int I = 0;

            //
            // PER CATEGORY FIND THE GROUPS AND CREATE 
            //

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

                if (WMatchingCateg == PRX + "201"
                    || WMatchingCateg == PRX + "202"
                    || WMatchingCateg == PRX + "203"
                    || WMatchingCateg == PRX + "204"
                    || WMatchingCateg == PRX + "205"
                    || WMatchingCateg == PRX + "206"
                    || WMatchingCateg == PRX + "207"
                    || WMatchingCateg == PRX + "208"
                    || WMatchingCateg == PRX + "209"

                    )
                {

                    //Message = "THIS THE ONE - " + WMatchingCateg + " ATMS   ";
                    //System.Windows.Forms.MessageBox.Show(Message);
                    // CONTINUE TO MATCHING 
                }
                else
                {
                    I++;
                    continue;
                }

             

                SelectionCriteria = " CategoryId ='" + WMatchingCateg + "'";

                Mcsf.ReadReconcCategoriesVsSourcebyCategory(WMatchingCateg);

               
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

                //
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
                    // FOR ATMS READ PER CATEGORY AND GROUP 
                    // DO Thread per Category/Group = Maybe 9 times 15 equals to 135 threads
                    // In Alecos Service there is a counter that we control the number of threads we shoot at the time
                    // To make it easy for each Category start 15 threds and start start the new one when all threads are completed
                    // OR ....... look at it statistically 
                    //*****************************************************
                  
                    //Message = "Multithreading Matching Starts";
                    //MessageBox.Show("Multithreading Matching Starts");
                    // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                    //**********************
                    //BeforeCallDtTime = DateTime.Now;

                    //W_MPComment += DateTime.Now + "_" + "Matching Starts Category.. " + WMatchingCateg + Environment.NewLine;

                    //Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WMatchingCateg, InRMCycle, W_MPComment);

                    //**********************

                    bool TestingWorkingFiles = false;

                    // Continue with groups of ATMs 
                    TableThisCategoryGroups = new DataTable();
                    TableThisCategoryGroups.Clear();

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

                    FromOwnATMs = true;
                    int L; 
                    if (FromOwnATMs == true)
                    {
                        L = 0;

                        // LOOP FOR GROUPS OF THIS MATCHING CATEGORY

                        //MessageBox.Show("Start the creation of threads"); 

                        while (L <= (TableThisCategoryGroups.Rows.Count - 1))
                        {

                            WMatchingCateg = (string)TableThisCategoryGroups.Rows[L]["MatchingCategoryId"];
                            WGroupId = (int)TableThisCategoryGroups.Rows[L]["GroupId"];
                            WReconcCategoryId = (string)TableThisCategoryGroups.Rows[L]["ReconcCategoryId"];
                            //******************************************************

                            // CREATE Threads to call 
                            // ONLY RECORDS IN TABLE
                            Tr.CreateThreadEntryInTable(WMatchingCateg, WGroupId, WReconcCategoryId, WRMCycle, "ThreadsForMatching");

                            // Intitialise 
                            W_MPComment = "Matching Starts for this"; 
                            Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                            L++; // Read Next entry of the table - next Group

                        }


                        // FOR THIS CATEGORY TURN ALL CATEGORIES/FILES COMBINATIONS TO 1

                        Mcsf.UpdateReconcCategoryVsSourceRecordProcessCodeToOne(WMatchingCateg);

                        MatchedCategories = MatchedCategories + WMatchingCateg + "..MATCHED" + "\r\n";


                    }
                    #endregion

                    
                }
                I++; // Read Next entry of the table ... Next Category 
            }
            // Here You 
            // ACTIVATTE THREADS 
            // 
            Tr.CreateThreads(WOperator, WSignedId, WRMCycle);

            // RETURN HERE AFTER ALL THREADS HAVE FINISHED 
            // AND DO SEQUENTIAL UPDATING

            //MessageBox.Show("Threads work had finished."); 

            string WSelectionCriteria = " WHERE StatusFromRRDM = 1 AND Status = 1 "+ "Order By  ReconcCategoryId, MatchingCateg  "; 
            Tr.ReadThreadsAndFillTable(WSelectionCriteria);

            // ThreadsTable
            // NEED FOR SEQUENTIAL UPDATING
            // LOOP FOR GROUPS OF THIS MATCHING CATEGORY

            try
            {
                int K = 0;

                while (K <= (Tr.ThreadsTable.Rows.Count - 1))
                {

                    WMatchingCateg = (string)Tr.ThreadsTable.Rows[K]["MatchingCateg"];
                    WGroupId = (int)Tr.ThreadsTable.Rows[K]["GroupId"];
                    WReconcCategoryId = (string)Tr.ThreadsTable.Rows[K]["ReconcCategoryId"];
                    //******************************************************
                    // CREATE Threads to call 

                    M_Multi.UpdatingAfterMatching_OurATMs(WOperator, WMatchingCateg, WReconcCategoryId, WRMCycle);


                    K++; // Read Next entry of the table - next Group

                }
            }
            catch (Exception ex)
            {
                //conn.Close();
                
                CatchDetails(ex, "608");
            }
                

           // MessageBox.Show("Last Updating after In MULTI had finished.");
            // M_Multi.UpdatingAfterMatching_OurATMs(WOperator, "BDC201", "RECATMS-101", WRMCycle);

        }

      


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


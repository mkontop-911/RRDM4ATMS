using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMReconcCategoriesSessions : Logger
    {
        public RRDMReconcCategoriesSessions() : base() { }

        public int SeqNo;
        public string CategoryId;
        public string CategoryName;

        public int AtmGroup;
        public string OwnerUserID;

        public int RunningJobNo;

        public int LastRRDMKey;
        public string ITMXUniqueTxnRef;

        public string DRBank;
        public string MatchingCatDr;
        public int MatchingSessionDr;

        public string CRBank;
        public string MatchingCatCr;
        public int MatchingSessionCr;

        public DateTime StartDateTm;

        //public string GlCurrency;
        public string GlAccountNo;

        public decimal GlYesterdaysBalance;
        public decimal GlTodaysBalance;

        public string NostroCcy;
        public decimal NostroCcyRate;

        public decimal MatchedTransAmt;
        public decimal NotMatchedTransAmt;

        public DateTime EndDateTm;

        public int NumberOfProcessFiles;
        public int NumberOfMatchedRecs;
        public int NumberOfUnMatchedRecs;

        public bool Difference;

        public DateTime StartReconcDtTm;
        public DateTime EndReconcDtTm;

        public decimal SettledUnMatchedAmtDefault;
        public decimal SettledUnMatchedAmtWorkFlow;

        public int NumberSettledUnMatchedDefault;
        public int NumberSettledUnMatchedWorkFlow;
        public int RemainReconcExceptions;

        public DateTime GL_StartReconcDtTm;
        public DateTime GL_EndReconcDtTm;

        public int GL_Original_Atms_Cash_Diff;
        public int GL_Remain_Atms_Cash_Diff;

        public string MatchingCat01;
        public bool MatchingCat01Updated;
        public string MatchingCat02;
        public bool MatchingCat02Updated;
        public string MatchingCat03;
        public bool MatchingCat03Updated;
        public string MatchingCat04;
        public bool MatchingCat04Updated;
        public string MatchingCat05;
        public bool MatchingCat05Updated;
        public string MatchingCat06;
        public bool MatchingCat06Updated;
        public string MatchingCat07;
        public bool MatchingCat07Updated;
        public string MatchingCat08;
        public bool MatchingCat08Updated;
        public string MatchingCat09;
        public bool MatchingCat09Updated;

        public int ProcessMode; // If -1 not ready for reconciliation 
                                // Means a file didnt come yet
                                // If 0 then it is ready if from BancNet(single route) means all files are present
                                // If for ATMs (multible route) means at least one route is completed 
                                // Therefore is ready for the ready routes. The other will be done later
                                // If 1 means it has passed the process of reconciliation   

        public string MPComment; // MATCHING PROGRESS Comment
                                 // To be fill up during matching to show progress

        public bool OpenRecord;

        public string Operator;

        public int TotalFiles;

        // Define the data table 
        public DataTable TableReconcSessionsDistinct = new DataTable();

        // Define the data table 
        public DataTable TableReconcSessionsPerCategory = new DataTable();

        public DataTable TableReconciledCategories = new DataTable();
        public DataTable TableNonReconciledCategories = new DataTable();

        public int TotalSelected;
        public int TotalReconcDone;
        public int TotalReconcNotDone;
        public int TotalPreviousRunningCycleReconcNotDone;
        public int TotalReconciledExceptions;
        public int TotalNonReconciledExceptions;

        public int TotalUnMatchedRecs;
        public int TotalRemainReconcExceptions;
        public int TotalGL_Remain_Atms_Cash_Diff; 

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        

        // string WhatFile = "[ATMS].[dbo].[ReconcCategoriesSessions]";

        // READER DETAILS 
        private void SessionsReaderDetails(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CategoryId = (string)rdr["CategoryId"];
            CategoryName = (string)rdr["CategoryName"];

            AtmGroup = (int)rdr["AtmGroup"];
            OwnerUserID = (string)rdr["OwnerUserID"];

            RunningJobNo = (int)rdr["RunningJobNo"];

            LastRRDMKey = (int)rdr["LastRRDMKey"];
            ITMXUniqueTxnRef = (string)rdr["ITMXUniqueTxnRef"];

            DRBank = (string)rdr["DRBank"];
            MatchingCatDr = (string)rdr["MatchingCatDr"];
            MatchingSessionDr = (int)rdr["MatchingSessionDr"];
            CRBank = (string)rdr["CRBank"];
            MatchingCatCr = (string)rdr["MatchingCatCr"];
            MatchingSessionCr = (int)rdr["MatchingSessionCr"];

            StartDateTm = (DateTime)rdr["StartDateTm"];

            //GlCurrency = (string)rdr["GlCurrency"];
            GlAccountNo = (string)rdr["GlAccountNo"];

            GlYesterdaysBalance = (decimal)rdr["GlYesterdaysBalance"];
            GlTodaysBalance = (decimal)rdr["GlTodaysBalance"];

            NostroCcy = (string)rdr["NostroCcy"];
            NostroCcyRate = (decimal)rdr["NostroCcyRate"];

            MatchedTransAmt = (decimal)rdr["MatchedTransAmt"];
            NotMatchedTransAmt = (decimal)rdr["NotMatchedTransAmt"];

            EndDateTm = (DateTime)rdr["EndDateTm"];

            NumberOfProcessFiles = (int)rdr["NumberOfProcessFiles"];

            NumberOfMatchedRecs = (int)rdr["NumberOfMatchedRecs"];
            NumberOfUnMatchedRecs = (int)rdr["NumberOfUnMatchedRecs"];

            Difference = (bool)rdr["Difference"];

            StartReconcDtTm = (DateTime)rdr["StartReconcDtTm"];
            EndReconcDtTm = (DateTime)rdr["EndReconcDtTm"];

            SettledUnMatchedAmtDefault = (decimal)rdr["SettledUnMatchedAmtDefault"];
            SettledUnMatchedAmtWorkFlow = (decimal)rdr["SettledUnMatchedAmtWorkFlow"];

            NumberSettledUnMatchedDefault = (int)rdr["NumberSettledUnMatchedDefault"];
            NumberSettledUnMatchedWorkFlow = (int)rdr["NumberSettledUnMatchedWorkFlow"];

            RemainReconcExceptions = (int)rdr["RemainReconcExceptions"];

            GL_StartReconcDtTm = (DateTime)rdr["GL_StartReconcDtTm"];
            GL_EndReconcDtTm = (DateTime)rdr["GL_EndReconcDtTm"];

            GL_Original_Atms_Cash_Diff = (int)rdr["GL_Original_Atms_Cash_Diff"];
            GL_Remain_Atms_Cash_Diff = (int)rdr["GL_Remain_Atms_Cash_Diff"];

            MatchingCat01 = (string)rdr["MatchingCat01"];
            MatchingCat01Updated = (bool)rdr["MatchingCat01Updated"];

            MatchingCat02 = (string)rdr["MatchingCat02"];
            MatchingCat02Updated = (bool)rdr["MatchingCat02Updated"];

            MatchingCat03 = (string)rdr["MatchingCat03"];
            MatchingCat03Updated = (bool)rdr["MatchingCat03Updated"];

            MatchingCat04 = (string)rdr["MatchingCat04"];
            MatchingCat04Updated = (bool)rdr["MatchingCat04Updated"];

            MatchingCat05 = (string)rdr["MatchingCat05"];
            MatchingCat05Updated = (bool)rdr["MatchingCat05Updated"];

            MatchingCat06 = (string)rdr["MatchingCat06"];
            MatchingCat06Updated = (bool)rdr["MatchingCat06Updated"];

            MatchingCat07 = (string)rdr["MatchingCat07"];
            MatchingCat07Updated = (bool)rdr["MatchingCat07Updated"];

            MatchingCat08 = (string)rdr["MatchingCat08"];
            MatchingCat08Updated = (bool)rdr["MatchingCat08Updated"];

            MatchingCat09 = (string)rdr["MatchingCat09"];
            MatchingCat09Updated = (bool)rdr["MatchingCat09Updated"];

            ProcessMode = (int)rdr["ProcessMode"];

            MPComment = (string)rdr["MPComment"];

            OpenRecord = (bool)rdr["OpenRecord"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ ReconcCategoriesSessions
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesSessionsSpecificCat(string InOperator, string InSignedId, string InCategoryId, int InLimitRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SelectionCriteria;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            RRDMGasParameters Gp = new RRDMGasParameters();

            bool Is_Presenter_InReconciliation = false;

            // Presenter
            string ParId = "946";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Presenter_InReconciliation = true;
            }

            TableReconcSessionsPerCategory = new DataTable();
            TableReconcSessionsPerCategory.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableReconcSessionsPerCategory.Columns.Add("RunningJobNo", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("CutOff Date", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("CategoryId", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("StartDateTm", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("EndDateTm", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("UnMatched", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("Pending", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("Atms_GL_Diff", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("Files", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("MatchedRecs", typeof(int));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE Operator = @Operator"
                    + " AND CategoryId = @CategoryId AND ProcessMode = 0 AND OwnerUserID = @OwnerUserID AND RunningJobNo >=@RunningJobNo "
                    + " ORDER BY RunningJobNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@OwnerUserID", InSignedId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InLimitRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

                            DataRow RowSelected = TableReconcSessionsPerCategory.NewRow();
         
                            RowSelected["RunningJobNo"] = RunningJobNo;

                            if (AtmGroup > 0 & Is_Presenter_InReconciliation == true)
                            {
                                // Our ATMS
                                Mpa.ReadPoolAndFindTotals_Presenter_PerRMCategory(RunningJobNo, CategoryId, 2);
                                NumberOfUnMatchedRecs = NumberOfUnMatchedRecs + Mpa.Presenter_Not_Settled + Mpa.PresenterSettled;
                                RemainReconcExceptions = RemainReconcExceptions + Mpa.Presenter_Not_Settled;
                            }
                            
                            Rjc.ReadReconcJobCyclesById(InOperator, RunningJobNo); 
                            RowSelected["CutOff Date"] = Rjc.Cut_Off_Date.ToShortDateString();
                            
                            RowSelected["CategoryId"] = CategoryId;

                            if (NumberOfUnMatchedRecs == 0)
                            {
                                RowSelected["StartDateTm"] = "N/A";
                                RowSelected["EndDateTm"] = "N/A";
                            }
                            if (NumberOfUnMatchedRecs > 0)
                            {
                                if (StartReconcDtTm == NullPastDate)
                                {
                                    RowSelected["StartDateTm"] = "Didn't Start";
                                }
                                else
                                {
                                    RowSelected["StartDateTm"] = StartReconcDtTm.ToString();
                                }
                                if (EndReconcDtTm == NullPastDate)
                                {
                                    RowSelected["StartDateTm"] = "Didn't Start";
                                }
                                else
                                {
                                    RowSelected["EndDateTm"] = EndReconcDtTm.ToString();
                                }

                            }
                            
                            RowSelected["UnMatched"] = NumberOfUnMatchedRecs;
                            RowSelected["Pending"] = RemainReconcExceptions;

                            //SelectionCriteria = " WHERE Operator ='" + Operator + "'"
                            //                          + " AND AtmsReconcGroup =" + AtmGroup
                            //                          + " AND GL_ReconcDiff = 1 "
                            //                          ;

                            //RowSelected["Atms_GL_Diff"] = Am.ReadAtmsMainByReconcGroupForTotal(SelectionCriteria);

                            RowSelected["Atms_GL_Diff"] = 0;

                            RowSelected["Files"] = NumberOfProcessFiles;
                            RowSelected["MatchedRecs"] = NumberOfMatchedRecs;


                            // ADD ROW
                            TableReconcSessionsPerCategory.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error In ReadReconcCategoriesSessionsSpecificCat"); 
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        // Methods 
        // READ ReconcCategoriesSessions
        // FILL UP A TABLE
        //
        public decimal AmtOfUnMatchedWithPresenter; 
        public void ReadReconcCategoriesSessionsSpecificCatWithPresenter(string InOperator, string InCategoryId, int InRMycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            AmtOfUnMatchedWithPresenter = 0; 
            string SelectionCriteria;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            RRDMGasParameters Gp = new RRDMGasParameters();

            bool Is_Presenter_InReconciliation = false;

            // Presenter
            string ParId = "946";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Presenter_InReconciliation = true;
            }

         
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 



            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                      + " WHERE Operator = @Operator AND CategoryId = @CategoryId  AND RunningJobNo = @RunningJobNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRMycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

                            //DataRow RowSelected = TableReconcSessionsPerCategory.NewRow();

                            //RowSelected["RunningJobNo"] = RunningJobNo;

                            if (AtmGroup > 0 & Is_Presenter_InReconciliation == true)
                            {
                                // Our ATMS
                                Mpa.ReadPoolAndFindTotals_Presenter_PerRMCategory(RunningJobNo, CategoryId, 2);
                                NumberOfUnMatchedRecs = NumberOfUnMatchedRecs + Mpa.Presenter_Not_Settled + Mpa.PresenterSettled;
                                RemainReconcExceptions = RemainReconcExceptions + Mpa.Presenter_Not_Settled;
                                AmtOfUnMatchedWithPresenter = NotMatchedTransAmt + Mpa.TotalPresenter; 
                            }
                            //if (AtmGroup > 0 & Is_Presenter_InReconciliation == true)
                            //{
                            //    // Our ATMS
                            //    Mpa.ReadPoolAndFindTotals_Presenter_PerRMCategory(RunningJobNo, CategoryId, 2);
                            //    NumberOfUnMatchedRecs = NumberOfUnMatchedRecs + Mpa.Presenter_Not_Settled + Mpa.PresenterSettled;
                            //    RemainReconcExceptions = RemainReconcExceptions + Mpa.Presenter_Not_Settled;
                            //}
                            //Rjc.ReadReconcJobCyclesById(InOperator, RunningJobNo);
                            //RowSelected["CutOff Date"] = Rjc.Cut_Off_Date.ToShortDateString();

                            //RowSelected["CategoryId"] = CategoryId;

                            //if (NumberOfUnMatchedRecs == 0)
                            //{
                            //    RowSelected["StartDateTm"] = "N/A";
                            //    RowSelected["EndDateTm"] = "N/A";
                            //}
                            //if (NumberOfUnMatchedRecs > 0)
                            //{
                            //    if (StartReconcDtTm == NullPastDate)
                            //    {
                            //        RowSelected["StartDateTm"] = "Didn't Start";
                            //    }
                            //    else
                            //    {
                            //        RowSelected["StartDateTm"] = StartReconcDtTm.ToString();
                            //    }
                            //    if (EndReconcDtTm == NullPastDate)
                            //    {
                            //        RowSelected["StartDateTm"] = "Didn't Start";
                            //    }
                            //    else
                            //    {
                            //        RowSelected["EndDateTm"] = EndReconcDtTm.ToString();
                            //    }

                            //}

                            //RowSelected["UnMatched"] = NumberOfUnMatchedRecs;
                            //RowSelected["Pending"] = RemainReconcExceptions;

                            ////SelectionCriteria = " WHERE Operator ='" + Operator + "'"
                            ////                          + " AND AtmsReconcGroup =" + AtmGroup
                            ////                          + " AND GL_ReconcDiff = 1 "
                            ////                          ;

                            ////RowSelected["Atms_GL_Diff"] = Am.ReadAtmsMainByReconcGroupForTotal(SelectionCriteria);

                            //RowSelected["Atms_GL_Diff"] = 0;

                            //RowSelected["Files"] = NumberOfProcessFiles;
                            //RowSelected["MatchedRecs"] = NumberOfMatchedRecs;


                            //// ADD ROW
                            //TableReconcSessionsPerCategory.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error In ReadReconcCategoriesSessionsSpecificCat");
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        // Methods 
        // READ ReconcCategoriesSessions FOR GL 
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesSessionsSpecificCat_GL(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            TableReconcSessionsPerCategory = new DataTable();
            TableReconcSessionsPerCategory.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableReconcSessionsPerCategory.Columns.Add("RunningJobNo", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("Cut_Off_Date", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("CategoryId", typeof(string));

            TableReconcSessionsPerCategory.Columns.Add("GL_StartDateTm", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("GL_EndDateTm", typeof(string));

            TableReconcSessionsPerCategory.Columns.Add("Atms_GL_Diff", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("Pending", typeof(int));

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE Operator = @Operator"
                    + " AND CategoryId = @CategoryId "
                    + " ORDER BY RunningJobNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

                            Rjc.ReadReconcJobCyclesById(Operator, RunningJobNo);

                            DataRow RowSelected = TableReconcSessionsPerCategory.NewRow();

                            RowSelected["RunningJobNo"] = RunningJobNo;

                            RowSelected["Cut_Off_Date"] = Rjc.Cut_Off_Date.ToShortDateString();

                            RowSelected["CategoryId"] = CategoryId;

                            if (GL_Original_Atms_Cash_Diff > 0)
                            {
                                RowSelected["GL_StartDateTm"] = GL_StartReconcDtTm.ToString();
                                RowSelected["GL_EndDateTm"] = GL_EndReconcDtTm.ToString();
                                if (GL_StartReconcDtTm == NullPastDate)
                                {
                                    RowSelected["GL_StartDateTm"] = "Not Started";
                                }
                                if (GL_EndReconcDtTm == NullPastDate)
                                {
                                    RowSelected["GL_EndDateTm"] = "Not Ended";
                                }
                            }
                            else
                            {
                                RowSelected["GL_StartDateTm"] = "N/A";
                                RowSelected["GL_EndDateTm"] = "N/A";
                            }
                           

                            RowSelected["Atms_GL_Diff"] = GL_Original_Atms_Cash_Diff;
                            RowSelected["Pending"] = GL_Remain_Atms_Cash_Diff;


                            // ADD ROW
                            TableReconcSessionsPerCategory.Rows.Add(RowSelected);

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
        // Methods 
        // READ ReconcCategoriesSessions by SeqNo
        // 
        //
        public void ReadReconcCategoriesSessionsBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;

            SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE SeqNo = @SeqNo ";

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

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

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
        // Methods 
        // READ to Check if Matching has done 
        // 
        //
        public void ReadReconcCategoriesSessions_To_Check_If_MatchingDONE(int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;
            //NumberOfMatchedRecs = (int)rdr["NumberOfMatchedRecs"];
            //NumberOfUnMatchedRecs = (int)rdr["NumberOfUnMatchedRecs"];

            SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE RunningJobNo = @RunningJobNo "
                       //+ " AND right(CategoryId, 3) not in ( '700','701' ,'702' ,'703','704','705' )"
                       + " AND (NumberOfMatchedRecs > 0 OR NumberOfUnMatchedRecs > 0) ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

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
        // Methods 
        // READ to Check if Matching has done 
        // 
        //
        public void ReadReconcCategoriesSessions_To_Check_If_MatchingDONE_MOBILE(string InSelection)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;
            //NumberOfMatchedRecs = (int)rdr["NumberOfMatchedRecs"];
            //NumberOfUnMatchedRecs = (int)rdr["NumberOfUnMatchedRecs"];

            SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + InSelection
                       // +" AND right(CategoryId, 3) in ( '700','701' ,'702' ,'703','704','705' )"
                       + " AND (NumberOfMatchedRecs > 0 OR NumberOfUnMatchedRecs > 0) ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                       // cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

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
        // Methods 
        // READ ReconcCategoriesSessions by SeqNo
        // 
        //
        public void ReadReconcCategoriesSessionsByCategoryAndCycle(string InCategoryId, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;

            SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE CategoryId = @CategoryId AND RunningJobNo = @RunningJobNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

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
        // Methods 
        // READ ReconcCategories and find the ones with matching and others no matching    
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesForReconcStatus(string InOperator, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconciledCategories = new DataTable();
            TableReconciledCategories.Clear();

            TableNonReconciledCategories = new DataTable();
            TableNonReconciledCategories.Clear();

            TotalSelected = 0;
            TotalReconcDone = 0;
            TotalReconcNotDone = 0;
            TotalPreviousRunningCycleReconcNotDone = 0;
            TotalReconciledExceptions = 0;
            TotalNonReconciledExceptions = 0;


            // DATA TABLE ROWS DEFINITION - Reconciled  
            TableReconciledCategories.Columns.Add("CategoryNm", typeof(string));
            TableReconciledCategories.Columns.Add("RunningJobNo", typeof(int));
            TableReconciledCategories.Columns.Add("OwnerUserID", typeof(string));


            // DATA TABLE ROWS DEFINITION - Not Reconciled  

            TableNonReconciledCategories.Columns.Add("CategoryNm", typeof(string));
            TableNonReconciledCategories.Columns.Add("RunningJobNo", typeof(int));
            TableNonReconciledCategories.Columns.Add("OwnerUserID", typeof(string));


            SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE Operator = @Operator "
                    + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

                            if (RemainReconcExceptions == 0 & RunningJobNo == InRunningJobNo)
                            {
                                //Records Reconciled
                                TotalReconcDone = TotalReconcDone + 1;

                                TotalReconciledExceptions = TotalReconciledExceptions + NumberOfUnMatchedRecs;
                                //public int TotalNonReconciledExceptions;

                                DataRow RowSelected = TableReconciledCategories.NewRow();

                                RowSelected["CategoryNm"] = CategoryName;
                                //RowSelected["Category_Name"] = CategoryName;
                                RowSelected["RunningJobNo"] = RunningJobNo;
                                RowSelected["OwnerUserID"] = OwnerUserID;

                                // ADD ROW
                                TableReconciledCategories.Rows.Add(RowSelected);
                            }
                            if (RemainReconcExceptions > 0)
                            {
                                if (RunningJobNo <= InRunningJobNo)
                                {
                                    TotalNonReconciledExceptions = TotalNonReconciledExceptions + RemainReconcExceptions;

                                    DataRow RowSelected = TableNonReconciledCategories.NewRow();

                                    RowSelected["CategoryNm"] = CategoryName;
                                    //RowSelected["Category_Name"] = CategoryName;
                                    RowSelected["RunningJobNo"] = RunningJobNo;
                                    RowSelected["OwnerUserID"] = OwnerUserID;

                                    // ADD ROW
                                    TableNonReconciledCategories.Rows.Add(RowSelected);
                                    //Records Reconciled
                                    if (RunningJobNo == InRunningJobNo)
                                    {
                                        // This Running Cycle 
                                        TotalReconcNotDone = TotalReconcNotDone + 1;
                                    }
                                    else
                                    {
                                        // Previous Running Cycles 
                                        TotalPreviousRunningCycleReconcNotDone = TotalPreviousRunningCycleReconcNotDone + 1;
                                    }
                                }

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

                    CatchDetails(ex);

                }
        }
        //
        // Methods 
        // READ ReconcCategoriesSessions Specific RMReconc and by Category Id 
        // 
        //
        public bool ReadReconcCategorySessionBySlaveCatAndRunningJobNo(string InOperator, string InCategoryId, string InSlaveCategoryId, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool MatchingDone = false; 

            TotalFiles = 0;

            SqlString = "SELECT *"
                       + " FROM  [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE Operator = @Operator AND CategoryId = @CategoryId  AND RunningJobNo = @RunningJobNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SessionsReaderDetails(rdr);

        

                            if (MatchingCat01 == InSlaveCategoryId)
                            {
                                if (MatchingCat01Updated == true)
                                {
                                    MatchingDone = true; 
                                }
                                else
                                {
                                    MatchingDone = false;
                                }
                            }
                            if (MatchingCat02 == InSlaveCategoryId)
                            {
                                if (MatchingCat02Updated == true)
                                {
                                    MatchingDone = true;
                                }
                                else
                                {
                                    MatchingDone = false;
                                }
                            }
                            if (MatchingCat03 == InSlaveCategoryId)
                            {
                                if (MatchingCat03Updated == true)
                                {
                                    MatchingDone = true;
                                }
                                else
                                {
                                    MatchingDone = false;
                                }
                            }
                            if (MatchingCat04 == InSlaveCategoryId)
                            {
                                if (MatchingCat04Updated == true)
                                {
                                    MatchingDone = true;
                                }
                                else
                                {
                                    MatchingDone = false;
                                }
                            }
                            if (MatchingCat05 == InSlaveCategoryId)
                            {
                                if (MatchingCat05Updated == true)
                                {
                                    MatchingDone = true;
                                }
                                else
                                {
                                    MatchingDone = false;
                                }
                            }
                            if (MatchingCat06 == InSlaveCategoryId)
                            {
                                if (MatchingCat06Updated == true)
                                {
                                    MatchingDone = true;
                                }
                                else
                                {
                                    MatchingDone = false;
                                }
                            }
                            if (MatchingCat07 == InSlaveCategoryId)
                            {
                                if (MatchingCat07Updated == true)
                                {
                                    MatchingDone = true;
                                }
                                else
                                {
                                    MatchingDone = false;
                                }
                            }
                            if (MatchingCat08 == InSlaveCategoryId)
                            {
                                if (MatchingCat08Updated == true)
                                {
                                    MatchingDone = true;
                                }
                                else
                                {
                                    MatchingDone = false;
                                }
                            }
                            if (MatchingCat09 == InSlaveCategoryId)
                            {
                                if (MatchingCat09Updated == true)
                                {
                                    MatchingDone = true;
                                }
                                else
                                {
                                    MatchingDone = false;
                                }
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

                    CatchDetails(ex);

                }
            return MatchingDone; 
        }

        //
        // Methods 
        // READ ReconcCategoriesSessions For Work Allocation 
        // FILL UP A TABLE
        //
        bool ValidSelection;
        int K, L;
        public void ReadReconcCategoriesSessionsFillTable(string InOperator, int InRunningJobNo, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMUsersRecords Us = new RRDMUsersRecords();

            TableReconcSessionsPerCategory = new DataTable();
            TableReconcSessionsPerCategory.Clear();

            TotalSelected = 0;

            ValidSelection = false;

            // DATA TABLE ROWS DEFINITION 
            TableReconcSessionsPerCategory.Columns.Add("SeqNo", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("RunningJobNo", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("CategoryId", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("MatchedRecs", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("UnMatchedRecs", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("StartReconc", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("EndReconc", typeof(string));

            TableReconcSessionsPerCategory.Columns.Add("OwnerUserID", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("OwnerName", typeof(string));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE Operator = @Operator "
                            + " AND RemainReconcExceptions > 0 AND RunningJobNo <= @RunningJobNo"
                            + " ORDER by CategoryId, RunningJobNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

                            if (InMode == 1)
                            {
                                ValidSelection = true;
                            }
                            if (InMode == 2)
                            {
                                K = L = 0;
                                ValidSelection = false;
                                if (ProcessMode == 0)
                                {
                                    if (MatchingCat01 != "")
                                    {
                                        K++;
                                        if (MatchingCat01Updated == true) L++;
                                    }
                                    if (MatchingCat02 != "")
                                    {
                                        K++;
                                        if (MatchingCat02Updated == true) L++;
                                    }
                                    if (MatchingCat03 != "")
                                    {
                                        K++;
                                        if (MatchingCat03Updated == true) L++;
                                    }
                                    if (MatchingCat04 != "")
                                    {
                                        K++;
                                        if (MatchingCat04Updated == true) L++;
                                    }
                                    if (MatchingCat05 != "")
                                    {
                                        K++;
                                        if (MatchingCat05Updated == true) L++;
                                    }
                                    if (MatchingCat06 != "")
                                    {
                                        K++;
                                        if (MatchingCat06Updated == true) L++;
                                    }
                                    if (MatchingCat07 != "")
                                    {
                                        K++;
                                        if (MatchingCat07Updated == true) L++;
                                    }
                                    if (MatchingCat08 != "")
                                    {
                                        K++;
                                        if (MatchingCat08Updated == true) L++;
                                    }
                                    if (MatchingCat09 != "")
                                    {
                                        K++;
                                        if (MatchingCat09Updated == true) L++;
                                    }

                                    if (K == L) ValidSelection = true;
                                    else ValidSelection = false;
                                }

                            }

                            if (InMode == 3)
                            {
                                K = L = 0;
                                ValidSelection = false;
                                if (ProcessMode == 0)
                                {
                                    if (MatchingCat01 != "")
                                    {
                                        K++;
                                        if (MatchingCat01Updated == true) L++;
                                    }
                                    if (MatchingCat02 != "")
                                    {
                                        K++;
                                        if (MatchingCat02Updated == true) L++;
                                    }
                                    if (MatchingCat03 != "")
                                    {
                                        K++;
                                        if (MatchingCat03Updated == true) L++;
                                    }
                                    if (MatchingCat04 != "")
                                    {
                                        K++;
                                        if (MatchingCat04Updated == true) L++;
                                    }
                                    if (MatchingCat05 != "")
                                    {
                                        K++;
                                        if (MatchingCat05Updated == true) L++;
                                    }
                                    if (MatchingCat06 != "")
                                    {
                                        K++;
                                        if (MatchingCat06Updated == true) L++;
                                    }
                                    if (MatchingCat07 != "")
                                    {
                                        K++;
                                        if (MatchingCat07Updated == true) L++;
                                    }
                                    if (MatchingCat08 != "")
                                    {
                                        K++;
                                        if (MatchingCat08Updated == true) L++;
                                    }

                                    if (MatchingCat09 != "")
                                    {
                                        K++;
                                        if (MatchingCat09Updated == true) L++;
                                    }
                                    if (K > L) ValidSelection = true;
                                    else ValidSelection = false;
                                }

                            }

                            if (ValidSelection == true)
                            {
                                DataRow RowSelected = TableReconcSessionsPerCategory.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["RunningJobNo"] = RunningJobNo;
                                RowSelected["CategoryId"] = CategoryId;

                                RowSelected["MatchedRecs"] = NumberOfMatchedRecs;

                                RowSelected["UnMatchedRecs"] = RemainReconcExceptions;

                                if (StartReconcDtTm == NullPastDate)
                                {
                                    RowSelected["StartReconc"] = "Not Started";
                                    RowSelected["EndReconc"] = "Not Started";
                                }
                                else
                                {
                                    RowSelected["StartReconc"] = StartReconcDtTm.ToString();
                                    if (EndReconcDtTm == NullPastDate)
                                    {
                                        RowSelected["EndReconc"] = "Not Finish";
                                    }
                                    else RowSelected["EndReconc"] = EndReconcDtTm.ToString();
                                }


                                RowSelected["OwnerUserID"] = OwnerUserID;
                                if (OwnerUserID != "")
                                {
                                    Us.ReadUsersRecord(OwnerUserID);
                                    RowSelected["OwnerName"] = Us.UserName;
                                }
                                else
                                {
                                    RowSelected["OwnerName"] = "Reconc Category has no Owner.";
                                }

                                // ADD ROW
                                TableReconcSessionsPerCategory.Rows.Add(RowSelected);
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

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ ReconcCategoriesSessions For Work Allocation 
        // 
        //
        string ReturnValue;
        public string ReadReconcCategoriesSessionsSpecificForTotalPicture
                    (string InOperator, string InCategoryId, int InRunningJobNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            ReturnValue = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE "
                    + "  CategoryId = @CategoryId"
                    + " AND RunningJobNo = @RunningJobNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                       // cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

                            K = L = 0;

                            if (ProcessMode == 0)
                            {
                                if (MatchingCat01 != "")
                                {
                                    K++;
                                    if (MatchingCat01Updated == true) L++;
                                }
                                if (MatchingCat02 != "")
                                {
                                    K++;
                                    if (MatchingCat02Updated == true) L++;
                                }
                                if (MatchingCat03 != "")
                                {
                                    K++;
                                    if (MatchingCat03Updated == true) L++;
                                }
                                if (MatchingCat04 != "")
                                {
                                    K++;
                                    if (MatchingCat04Updated == true) L++;
                                }
                                if (MatchingCat05 != "")
                                {
                                    K++;
                                    if (MatchingCat05Updated == true) L++;
                                }
                                if (MatchingCat06 != "")
                                {
                                    K++;
                                    if (MatchingCat06Updated == true) L++;
                                }
                                if (MatchingCat07 != "")
                                {
                                    K++;
                                    if (MatchingCat07Updated == true) L++;
                                }
                                if (MatchingCat08 != "")
                                {
                                    K++;
                                    if (MatchingCat08Updated == true) L++;
                                }
                                if (MatchingCat09 != "")
                                {
                                    K++;
                                    if (MatchingCat09Updated == true) L++;
                                }

                                if (K > 0 & K == L) ReturnValue = "Fully Done";
                                else ReturnValue = "Partially";
                                if (L == 0) ReturnValue = "Not Done";
                            }
                            if (ProcessMode == -1)
                            {
                                ReturnValue = "Not Done";
                            }

                        }

                        if (RecordFound == false) ReturnValue = "Inop";

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
            return ReturnValue;
        }

        //
        // Methods 
        // READ ReadReconcCategoriesSessionsSpecific
        // 
        //

        public void  ReadReconcCategoriesSessionsSpecific
                    (string InOperator,string InCategoryId, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

        
            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE Operator = @Operator"
                    + " AND CategoryId = @CategoryId"
                    + " AND RunningJobNo = @RunningJobNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

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

// FIND OUT IF THIS MATCHING CATEGORY WAS UPDATED 
        public void ReadReconcCategoriesSessionsForMatchingCateg
                    (string InOperator, string InCategoryId, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE Operator = @Operator"
                    + " AND CategoryId = @CategoryId"
                    + " AND RunningJobNo = @RunningJobNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                         cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

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
        // Methods 
        // READ ReconcCategoriesSessions DISTINCT
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesDistinctFillTable(string InOperator, int InMode, string InWhatBank, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool IncludeInTable;

            //If InMode = 1 then show only the unreconciled 
            //If InMode = 2 then show ALL 
            //If InMode = 3 then show only the specific category of "Our Atms for Cash GL" 
            //If InMode = 5 Per UserId

            TableReconcSessionsDistinct = new DataTable();
            TableReconcSessionsDistinct.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableReconcSessionsDistinct.Columns.Add("CategoryName", typeof(string));
            TableReconcSessionsDistinct.Columns.Add("CategoryId", typeof(string));
            TableReconcSessionsDistinct.Columns.Add("OutStanding", typeof(int));
            TableReconcSessionsDistinct.Columns.Add("OwnerUserID", typeof(string));

            //SELECT DISTINCT 
            
            if (InMode == 1 & InOperator != "ITMX")
            {
                SqlString = "SELECT DISTINCT CategoryId "
                                  + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                                  + " WHERE Operator = @Operator AND RemainReconcExceptions > 0 "
                                  + " ORDER BY CategoryId";
            }
            if (InMode == 5 & InOperator != "ITMX")
            {
                SqlString = "SELECT DISTINCT CategoryId, OwnerUserID "
                                  + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                                  + " WHERE Operator = @Operator AND RemainReconcExceptions > 0 "
                                  + " ORDER BY CategoryId";
            }
            if (InMode == 2 & InOperator != "ITMX")
            {
                SqlString = "SELECT DISTINCT CategoryId "
                                  + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                                  + " WHERE Operator = @Operator "
                                  + " ORDER BY CategoryId";
            }

            if (InMode == 3 & InOperator != "ITMX")
            {
                SqlString = "SELECT DISTINCT CategoryId "
                                  + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                                  + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
                                  + " ORDER BY CategoryId";
            }

            if (InMode == 2 & InOperator == "ITMX" & InWhatBank == "ITMX")
            {
                SqlString = "SELECT DISTINCT CategoryId "
                                  + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                                  + " WHERE Operator = @Operator  "
                                  + " ORDER BY CategoryId";
            }
            if (InMode == 2 & InOperator == "ITMX" & InWhatBank != "ITMX")
            {
                SqlString = "SELECT DISTINCT CategoryId "
                                  + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                                  + " WHERE Operator = @Operator AND (DRBank = @BankId OR CRBank = @BankId) "
                                  + " ORDER BY CategoryId";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        if (InMode == 2 & InWhatBank != "ITMX")
                        {
                            cmd.Parameters.AddWithValue("@BankId", InWhatBank);
                        }
                        if (InMode == 3 & InOperator != "ITMX")
                        {
                            cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        }
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            CategoryId = (string)rdr["CategoryId"];

                            Mc.ReadMatchingCategorybyActiveCategId(InOperator, CategoryId);

                            if ((InMode == 1 || InMode == 2) & Mc.Origin == "Our Atms for Cash GL")
                            {
                                IncludeInTable = false;
                            }
                            else IncludeInTable = true;

                            if (IncludeInTable == true)
                            {
                                DataRow RowSelected = TableReconcSessionsDistinct.NewRow();
                                string InSignedId = "To be defined";
                                ReadReconcCategoriesByCategoryIdToGetOther(CategoryId, InSignedId, InMode);

                                RowSelected["CategoryName"] = CategoryName;
                                RowSelected["CategoryId"] = CategoryId;

                                RowSelected["OutStanding"] = RemainReconcExceptions;
                                RowSelected["OwnerUserID"] = OwnerUserID;

                                // ADD ROW
                                TableReconcSessionsDistinct.Rows.Add(RowSelected);
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

                    CatchDetails(ex);

                }
        }


        //
        // Methods 
        // READ ReconcCategoriesSessions 
        // FILL UP A TABLE
        //
        string CategoryNmOld;
        public void ReadReconcUserCategoriesFillTable(string InOwnerUserID, string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            CategoryNmOld = "";

            TableReconcSessionsDistinct = new DataTable();
            TableReconcSessionsDistinct.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableReconcSessionsDistinct.Columns.Add("CategoryName", typeof(string));
            TableReconcSessionsDistinct.Columns.Add("CategoryId", typeof(string));
            TableReconcSessionsDistinct.Columns.Add("OutStanding", typeof(int));
            TableReconcSessionsDistinct.Columns.Add("OwnerUserID", typeof(string));

            SqlString = "SELECT * "
                              + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                              + " WHERE Operator = @Operator "
                              + " AND OwnerUserID = @OwnerUserID "
                              + " AND ProcessMode = 0 "
                              + " ORDER by CategoryId , RunningJobNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@OwnerUserID", InOwnerUserID);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

                            CategoryNmOld = CategoryName;

                            DataRow RowSelected = TableReconcSessionsDistinct.NewRow();

                            RowSelected["CategoryName"] = CategoryName;
                            RowSelected["CategoryId"] = CategoryId;

                            RowSelected["OutStanding"] = RemainReconcExceptions;
                            RowSelected["OwnerUserID"] = OwnerUserID;

                            // ADD ROW
                            TableReconcSessionsDistinct.Rows.Add(RowSelected);

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
        // Methods 
        // READ ReconcCategoriesSessions by CategoryId 
        // TO GET NAME 
        // 
        public void ReadReconcCategoriesByCategoryIdForName(string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //SELECT DISTINCT 
            SqlString = "SELECT CategoryName "
                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE CategoryId = @CategoryId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            CategoryName = (string)rdr["CategoryName"];

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

        //
        // Methods 
        // READ ReconcCategoriesSessions by CategoryId 
        // TO GET NAME 
        // 
        public void ReadReconcCategoriesSessionsByCategoryId_And_Userid(string InCategoryId, string InOwnerUserID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //SELECT DISTINCT 
            SqlString = "SELECT CategoryName "
                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE CategoryId = @CategoryId AND OwnerUserID = @OwnerUserID ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@OwnerUserID", InOwnerUserID);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            CategoryName = (string)rdr["CategoryName"];

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

        // Methods 
        // READ ReconcCategoriesSessions OwnerUserID
        // TO GET if exists 
        // 
        public void ReadReconcCategoriesSessionsBy_Userid(string InOwnerUserID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //SELECT DISTINCT 
            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                    + " WHERE OwnerUserID = @OwnerUserID ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        
                        cmd.Parameters.AddWithValue("@OwnerUserID", InOwnerUserID);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SessionsReaderDetails(rdr);

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


        //
        // Methods 
        // READ ReconcCategoriesSessions by CategoryId 
        // TO GET the first one with outstanding exceptions Other needed fields 
        // 
        public void ReadReconcCategoriesByCategoryIdToGetOther(string InCategoryId, string InUserId ,int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            //If InMode = 1 then show only the unreconciled 
            //If InMode = 2 then show ALL 
            //If InMode = 3 then show ALL 
            //SELECT 
            if (InMode == 1)
            {
                SqlString = "SELECT * "
                                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                                    + " WHERE CategoryId = @CategoryId AND RemainReconcExceptions > 0 "
                                     + " ORDER BY RunningJobNo DESC ";
            }
            //SELECT 
            if (InMode == 2 || InMode == 3)
            {
                SqlString = "SELECT * "
                                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                                    + " WHERE CategoryId = @CategoryId "
                                    + " ORDER BY RunningJobNo DESC ";
            }

            //SELECT 
            if (InMode ==5)
            {
                SqlString = "SELECT * "
                                    + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                                    + " WHERE CategoryId = @CategoryId AND "
                                    + " ORDER BY RunningJobNo DESC ";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            CategoryName = (string)rdr["CategoryName"];
                            RemainReconcExceptions = (int)rdr["RemainReconcExceptions"];
                            OwnerUserID = (string)rdr["OwnerUserID"];
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
        //
        // Methods 
        // READ ReconcCategoriesSessions to find outstanding exceptions 
        // 
        //
        public void ReadReconcCategoriesSessionsForRemainUnMatched(string InOperator, string InCategoryId, int InRMCycle, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int LimitRMCycle = InRMCycle;
            DateTime WCut_Off_Date;
            // InMode = 1 get for this Cycle
            // InMode = 2 get all after the Limit 
            // InMode = 3 get all less than the given cycle

            if (InMode == 2)
            {
                RRDMGasParameters Gp = new RRDMGasParameters();
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                RRDMReconcCategories Rc = new RRDMReconcCategories();
                
                string WOperator = InOperator;
                string WJobCategory = "ATMs";
                int Minus_Days;
                DateTime LIMITDate;
                
                // Find Current Job Cycle 
                int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
                if (WReconcCycleNo != 0)
                {
                    WCut_Off_Date = Rjc.Cut_Off_Date;
                }
                else
                {
                    WCut_Off_Date = NullPastDate;
                }
                // FIND Cycle ID For Cut Off date - minus 25 

                Gp.ReadParametersSpecificId(WOperator, "503", "01", "", ""); // 
                if (Gp.RecordFound == true)
                {
                    Minus_Days = (int)Gp.Amount;
                }
                else
                {
                    Minus_Days = 30;
                }

                //labelLimitParameter_503.Text = "We consider in Parameter 503.Days." + Minus_Days.ToString() + "..prior to today cut off";

                // SHOW ONLY Records above this 
                LIMITDate = WCut_Off_Date.AddDays(-Minus_Days);

                Rjc.ReadReconcJobCyclesByCutOffDate(LIMITDate);

                if (Rjc.RecordFound == true)
                {
                    LimitRMCycle = Rjc.JobCycle;
                }
                else
                {
                    Rjc.ReadReconcJobCyclesAndFindFirstCycle(LIMITDate);
                    LimitRMCycle = Rjc.JobCycle;
                }
                //Rc.ReadReconcCategoriesForMatrix(WOperator, LimitRMCycle, 2);

            }

            NumberOfUnMatchedRecs = 0;
            TotalRemainReconcExceptions = 0;
            TotalGL_Remain_Atms_Cash_Diff = 0; 

            if (InMode == 1)
            {
                SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                      + " WHERE CategoryId= @CategoryId AND RunningJobNo = @RunningJobNo";
            }
            if (InMode == 2)
            {
                SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE  CategoryId= @CategoryId AND RunningJobNo >= @RunningJobNo ";

            }
            if (InMode == 3)
            {
                // Less or Equal to given Cycle 
                SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE  CategoryId= @CategoryId AND RunningJobNo <= @RunningJobNo ";

              
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        if (InMode == 2)
                        {
                            cmd.Parameters.AddWithValue("@RunningJobNo", LimitRMCycle);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@RunningJobNo", InRMCycle);
                        }
                        
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            NumberOfUnMatchedRecs = (int)rdr["NumberOfUnMatchedRecs"];
                            RemainReconcExceptions = (int)rdr["RemainReconcExceptions"];
                            GL_Remain_Atms_Cash_Diff = (int)rdr["GL_Remain_Atms_Cash_Diff"];

                            AtmGroup = (int)rdr["AtmGroup"];

                            TotalUnMatchedRecs = TotalUnMatchedRecs + NumberOfUnMatchedRecs;
                            TotalRemainReconcExceptions = TotalRemainReconcExceptions + RemainReconcExceptions;
                            TotalGL_Remain_Atms_Cash_Diff = TotalGL_Remain_Atms_Cash_Diff + GL_Remain_Atms_Cash_Diff;

                            // Read 
                            if (AtmGroup > 0)
                            {
                                // Check if any 
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

                    CatchDetails(ex);

                }
        }

        //
        public void ReadReconcCategoriesSessionsForRemainUnMatched_MOBILE(string InOperator, string InCategoryId, int InRMCycle,string W_Application ,int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int LimitRMCycle = InRMCycle;
            DateTime WCut_Off_Date;
            // InMode = 1 get for this Cycle
            // InMode = 2 get all after the Limit 
            // InMode = 3 get all less than the given cycle

            if (InMode == 2)
            {
                RRDMGasParameters Gp = new RRDMGasParameters();
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                RRDMReconcCategories Rc = new RRDMReconcCategories();

                string WOperator = InOperator;
                string WJobCategory = W_Application;
                int Minus_Days;
                DateTime LIMITDate;

                // Find Current Job Cycle 
                int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
                if (WReconcCycleNo != 0)
                {
                    WCut_Off_Date = Rjc.Cut_Off_Date;
                }
                else
                {
                    WCut_Off_Date = NullPastDate;
                }
                // FIND Cycle ID For Cut Off date - minus 25 

                Gp.ReadParametersSpecificId(WOperator, "503", "01", "", ""); // 
                if (Gp.RecordFound == true)
                {
                    Minus_Days = (int)Gp.Amount;
                }
                else
                {
                    Minus_Days = 30;
                }

                //labelLimitParameter_503.Text = "We consider in Parameter 503.Days." + Minus_Days.ToString() + "..prior to today cut off";

                // SHOW ONLY Records above this 
                LIMITDate = WCut_Off_Date.AddDays(-Minus_Days);

                Rjc.ReadReconcJobCyclesByCutOffDate(LIMITDate);

                if (Rjc.RecordFound == true)
                {
                    LimitRMCycle = Rjc.JobCycle;
                }
                else
                {
                    Rjc.ReadReconcJobCyclesAndFindFirstCycle_MOBILE(W_Application);
                    LimitRMCycle = Rjc.JobCycle;
                }
                //Rc.ReadReconcCategoriesForMatrix(WOperator, LimitRMCycle, 2);

            }

            NumberOfUnMatchedRecs = 0;
            TotalRemainReconcExceptions = 0;
            TotalGL_Remain_Atms_Cash_Diff = 0;

            if (InMode == 1)
            {
                SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                      + " WHERE CategoryId= @CategoryId AND RunningJobNo = @RunningJobNo";
            }
            if (InMode == 2)
            {
                SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE  CategoryId= @CategoryId AND RunningJobNo >= @RunningJobNo ";

            }
            if (InMode == 3)
            {
                // Less or Equal to given Cycle 
                SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE  CategoryId= @CategoryId AND RunningJobNo <= @RunningJobNo ";


            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        if (InMode == 2)
                        {
                            cmd.Parameters.AddWithValue("@RunningJobNo", LimitRMCycle);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@RunningJobNo", InRMCycle);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            NumberOfUnMatchedRecs = (int)rdr["NumberOfUnMatchedRecs"];
                            RemainReconcExceptions = (int)rdr["RemainReconcExceptions"];
                            GL_Remain_Atms_Cash_Diff = (int)rdr["GL_Remain_Atms_Cash_Diff"];

                            AtmGroup = (int)rdr["AtmGroup"];

                            TotalUnMatchedRecs = TotalUnMatchedRecs + NumberOfUnMatchedRecs;
                            TotalRemainReconcExceptions = TotalRemainReconcExceptions + RemainReconcExceptions;
                            TotalGL_Remain_Atms_Cash_Diff = TotalGL_Remain_Atms_Cash_Diff + GL_Remain_Atms_Cash_Diff;

                            // Read 
                            if (AtmGroup > 0)
                            {
                                // Check if any 
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

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ ReconcCategoriesSessions Specific by RunningJobNo
        // 
        //
        public void ReadReconcCategorySessionByCatAndRunningJobNo(string InOperator, string InCategoryId, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;

            SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE Operator = @Operator AND CategoryId = @CategoryId  AND RunningJobNo = @RunningJobNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

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
        // Methods 
        // READ All Comments
        // 
        //
        public string AllMPComment; 
        public void ReadAllCommentsByCatAndRunningJobNo(string InOperator, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;
            AllMPComment = ""; 

            SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo"
                       +" ORDER BY CategoryId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                       // cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

                            AllMPComment += "CATEGORY.." + CategoryId + Environment.NewLine;
                            AllMPComment = AllMPComment + MPComment + Environment.NewLine;
                            
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
        // Methods 
        // READ ReconcCategoriesSessions Specific by RunningJobNo
        // 
        //
        public void ReadReconcCategorySessionByCatNameAndRunningJobNo(string InOperator, string InCategoryName, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;

            SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE Operator = @Operator AND CategoryName = @CategoryName  AND RunningJobNo = @RunningJobNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryName", InCategoryName);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);


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
        // Methods 
        // READ ReconcCategoriesSessions by CategoryId
        // To find if ProcessMode = -1 
        // READ The LAST ONE 
        // 
        public void ReadReconcCategorySessionByCatToFindTheLastRecord(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;

            SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[ReconcCategoriesSessions]"
                       + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
                       + " ORDER By SeqNo DESC ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Reader Details
                            SessionsReaderDetails(rdr);

                            break; // AT THE LAST ONE 

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

        // Insert Reconciliation Category Session 
        public int InsertReconcCategoriesSessionRecord()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcCategoriesSessions]"
                + " ([CategoryId],"
                + " [CategoryName],"
                + " [AtmGroup],"            
                + " [OwnerUserID],"
                + " [RunningJobNo],"
                + " [GlAccountNo],"
                + " [GlYesterdaysBalance],"
                + " [GlTodaysBalance],"
                + " [MatchingCat01],"
                + " [MatchingCat02],"
                + " [MatchingCat03],"
                + " [MatchingCat04],"
                + " [MatchingCat05],"
                + " [MatchingCat06],"
                 + " [MatchingCat07],"
                  + " [MatchingCat08],"
                   + " [MatchingCat09],"
                + " [ProcessMode],"
                + " [Operator] )"
                + " VALUES"
                + " (@CategoryId,"
                + " @CategoryName,"
                + " @AtmGroup,"
                + " @OwnerUserID,"
                + " @RunningJobNo,"
                + " @GlAccountNo,"
                + " @GlYesterdaysBalance,"
                + " @GlTodaysBalance,"
                + " @MatchingCat01,"
                + " @MatchingCat02,"
                + " @MatchingCat03,"
                + " @MatchingCat04,"
                + " @MatchingCat05,"
                + " @MatchingCat06,"
                   + " @MatchingCat07,"
                      + " @MatchingCat08,"
                       + " @MatchingCat09,"
                + " @ProcessMode,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@CategoryName", CategoryName);

                        cmd.Parameters.AddWithValue("@AtmGroup", AtmGroup);

                        //cmd.Parameters.AddWithValue("@GlCurrency", GlCurrency);

                        cmd.Parameters.AddWithValue("@OwnerUserID", OwnerUserID);

                        cmd.Parameters.AddWithValue("@RunningJobNo", RunningJobNo);

                        cmd.Parameters.AddWithValue("@GlAccountNo", GlAccountNo);
                        cmd.Parameters.AddWithValue("@GlYesterdaysBalance", GlYesterdaysBalance);
                        cmd.Parameters.AddWithValue("@GlTodaysBalance", GlTodaysBalance);

                        cmd.Parameters.AddWithValue("@MatchingCat01", MatchingCat01);
                        cmd.Parameters.AddWithValue("@MatchingCat02", MatchingCat02);
                        cmd.Parameters.AddWithValue("@MatchingCat03", MatchingCat03);
                        cmd.Parameters.AddWithValue("@MatchingCat04", MatchingCat04);
                        cmd.Parameters.AddWithValue("@MatchingCat05", MatchingCat05);
                        cmd.Parameters.AddWithValue("@MatchingCat06", MatchingCat06);
                        cmd.Parameters.AddWithValue("@MatchingCat07", MatchingCat07);
                        cmd.Parameters.AddWithValue("@MatchingCat08", MatchingCat08);
                        cmd.Parameters.AddWithValue("@MatchingCat09", MatchingCat09);

                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return SeqNo;
        }

        //
        // 
        // UPDATE ReconcCategoriesSessions Start date time 
        // 
        public void UpdateReconcCategorySessionWithReconWithOwner(string InCategoryId, string InOwnerUserID)
        {

            ErrorFound = false;
            ErrorOutput = "";
            //
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " OwnerUserID = @OwnerUserID "
                         //   + " WHERE RunningJobNo = @RunningJobNo AND " +
                           + " WHERE CategoryId = @CategoryId ", conn))
                    {
                        //cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@OwnerUserID", InOwnerUserID);

                        cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions Owner 
        // 
        public void UpdateReconcCategorySessionStartDate(string InCategoryId, int InRunningJobNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " StartReconcDtTm = @StartReconcDtTm "
                            + " WHERE CategoryId = @CategoryId AND RunningJobNo = @RunningJobNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@StartReconcDtTm", StartReconcDtTm);

                        cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions With All Fields
        // 
        public void UpdateReconcCategorySessionAtMatchingProcess(string InCategoryId, int InRunningJobNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows ;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " GlTodaysBalance = @GlTodaysBalance ,"
                            + " MatchedTransAmt = @MatchedTransAmt ,"
                            + " NotMatchedTransAmt = @NotMatchedTransAmt ,"
                            + " NumberOfMatchedRecs = @NumberOfMatchedRecs ,"
                            + " NumberOfUnMatchedRecs = @NumberOfUnMatchedRecs , "
                            + " Difference = @Difference ,"
                            + " RemainReconcExceptions = @RemainReconcExceptions "
                            + " WHERE CategoryId =@CategoryId AND RunningJobNo = @RunningJobNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@GlTodaysBalance", GlTodaysBalance);
                        cmd.Parameters.AddWithValue("@MatchedTransAmt", MatchedTransAmt);
                        cmd.Parameters.AddWithValue("@NotMatchedTransAmt", NotMatchedTransAmt);

                        cmd.Parameters.AddWithValue("@NumberOfMatchedRecs", NumberOfMatchedRecs);
                        cmd.Parameters.AddWithValue("@NumberOfUnMatchedRecs", NumberOfUnMatchedRecs);
                        cmd.Parameters.AddWithValue("@Difference", Difference);
                        cmd.Parameters.AddWithValue("@RemainReconcExceptions", RemainReconcExceptions);
                      
                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions With MPComment
        // 
        public void UpdateReconcCategorySessionAtMatchingProcess_MPComment(string InCategoryId, int InRunningJobNo, string InMPComment)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InCategoryId == null)
            {
                return; 
            }

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MPComment = @MPComment "
                            + " WHERE CategoryId =@CategoryId AND RunningJobNo = @RunningJobNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@MPComment", InMPComment);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions for 
        // 
        public void UpdateReconcCategorySession_ForAtms_Cash_Diff(string InCategoryId, int InRunningJobNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows ;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + "  GL_StartReconcDtTm =  @GL_StartReconcDtTm "
                            + ", GL_EndReconcDtTm = @GL_EndReconcDtTm "
                            + ", GL_Original_Atms_Cash_Diff = @GL_Original_Atms_Cash_Diff "
                            + ", GL_Remain_Atms_Cash_Diff = @GL_Remain_Atms_Cash_Diff "
                            + " WHERE CategoryId =@CategoryId AND RunningJobNo = @RunningJobNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@GL_StartReconcDtTm", GL_StartReconcDtTm);
                        cmd.Parameters.AddWithValue("@GL_EndReconcDtTm", GL_EndReconcDtTm);

                        cmd.Parameters.AddWithValue("@GL_Original_Atms_Cash_Diff", GL_Original_Atms_Cash_Diff);
                        cmd.Parameters.AddWithValue("@GL_Remain_Atms_Cash_Diff", GL_Remain_Atms_Cash_Diff);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions At Matching Process CAT 01
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycleCat01(string InOperator, int InRunningJobNo, string InCategoryId , string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MatchingCat01Updated = 1, ProcessMode = 0 "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId AND MatchingCat01 = @MatchingCat01", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        
                        cmd.Parameters.AddWithValue("@MatchingCat01", InMatchingCateg);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions At Matching Process CAT 02
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycleCat02(string InOperator, int InRunningJobNo, string InCategoryId, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MatchingCat02Updated = 1, ProcessMode = 0 "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId AND MatchingCat02 = @MatchingCat02", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@MatchingCat02", InMatchingCateg);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions At Matching Process CAT 03
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycleCat03(string InOperator, int InRunningJobNo, string InCategoryId, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MatchingCat03Updated = 1, ProcessMode = 0 "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId AND MatchingCat03 = @MatchingCat03", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@MatchingCat03", InMatchingCateg);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions At Matching Process CAT 04
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycleCat04(string InOperator, int InRunningJobNo, string InCategoryId, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MatchingCat04Updated = 1, ProcessMode = 0 "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId AND MatchingCat04 = @MatchingCat04", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@MatchingCat04", InMatchingCateg);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions At Matching Process CAT 05
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycleCat05(string InOperator, int InRunningJobNo, string InCategoryId, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MatchingCat05Updated = 1, ProcessMode = 0 "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId AND MatchingCat05 = @MatchingCat05", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@MatchingCat05", InMatchingCateg);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions At Matching Process CAT 06
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycleCat06(string InOperator, int InRunningJobNo, string InCategoryId, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MatchingCat06Updated = 1, ProcessMode = 0 "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId AND MatchingCat06 = @MatchingCat06", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@MatchingCat06", InMatchingCateg);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions At Matching Process CAT 07
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycleCat07(string InOperator, int InRunningJobNo, string InCategoryId, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MatchingCat07Updated = 1, ProcessMode = 0 "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId AND MatchingCat07 = @MatchingCat07", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@MatchingCat07", InMatchingCateg);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions At Matching Process CAT 08
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycleCat08(string InOperator, int InRunningJobNo, string InCategoryId, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MatchingCat08Updated = 1, ProcessMode = 0 "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId AND MatchingCat08 = @MatchingCat08", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@MatchingCat08", InMatchingCateg);

                        rows = cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions At Matching Process CAT 09
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycleCat09(string InOperator, int InRunningJobNo, string InCategoryId, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " MatchingCat09Updated = 1, ProcessMode = 0 "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId AND MatchingCat09 = @MatchingCat09", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@MatchingCat09", InMatchingCateg);

                        rows = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    MessageBox.Show("Cancel during:...:.."+InCategoryId, "AND" + InMatchingCateg); 

                    CatchDetails(ex);
                }
        }

        //
        // UPDATE ReconcCategoriesSessions With ProcessMode
        // 
        public void UpdateReconcCategorySessionAtOpeningNewCycle(string InOperator, int InRunningJobNo, int InProcessMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //int rows ;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" 
                         + " SET "
                            + " ProcessMode = @ProcessMode "
                            + " WHERE Operator = @Operator AND RunningJobNo = @RunningJobNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        cmd.Parameters.AddWithValue("@ProcessMode", InProcessMode);

                        cmd.ExecuteNonQuery();

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
        // UPDATE ReconcCategoriesSessions at Reconciliation closing 
        // 
        public void UpdateReconcCategorySessionWithAuthorClosing(string InCategoryId, int InRunningJobNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
                            + " EndReconcDtTm = @EndReconcDtTm ,"
                            + " SettledUnMatchedAmtDefault = @SettledUnMatchedAmtDefault ,SettledUnMatchedAmtWorkFlow = @SettledUnMatchedAmtWorkFlow ,"
                            + " NumberSettledUnMatchedDefault = @NumberSettledUnMatchedDefault ,NumberSettledUnMatchedWorkFlow = @NumberSettledUnMatchedWorkFlow ,"
                            + " RemainReconcExceptions = @RemainReconcExceptions "
                            //+ " ProcessMode = @ProcessMode "
                            + " WHERE CategoryId =@CategoryId AND RunningJobNo = @RunningJobNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@EndReconcDtTm", EndReconcDtTm);
                        cmd.Parameters.AddWithValue("@SettledUnMatchedAmtDefault", SettledUnMatchedAmtDefault);
                        cmd.Parameters.AddWithValue("@SettledUnMatchedAmtWorkFlow", SettledUnMatchedAmtWorkFlow);
                        cmd.Parameters.AddWithValue("@NumberSettledUnMatchedDefault", NumberSettledUnMatchedDefault);
                        cmd.Parameters.AddWithValue("@NumberSettledUnMatchedWorkFlow", NumberSettledUnMatchedWorkFlow);
                        cmd.Parameters.AddWithValue("@RemainReconcExceptions", RemainReconcExceptions);
                        //cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);

                        cmd.ExecuteNonQuery();


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
        // DELETE Sessions for Current Not Started Cycles
        // 
        //
        public void DeleteCycleNo(string InOperator, int InJobCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    // ATM FIELDS 
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE [ATMS].[dbo].[ReconcCategoriesSessions]  "
                            + " WHERE RunningJobNo = @RunningJobNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@RunningJobNo", InJobCycle);

                        cmd.ExecuteNonQuery();

                    }

                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //    //
        //    // UPDATE During Replenishement 
        //    // Update number of presented errors(Or Other) found during Replenishement- 
        //    // Use field of unmatched to do so 
        //    //
        //    public void UpdateReconcCategorySessionWithReplenishementErrors(string InCategoryId, int InRunningJobNo)
        //    {

        //        ErrorFound = false;
        //        ErrorOutput = "";

        //        using (SqlConnection conn =
        //            new SqlConnection(connectionString))
        //            try
        //            {
        //                conn.Open();
        //                using (SqlCommand cmd =
        //                     new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesSessions]" + " SET "
        //                        + " NumberOfUnMatchedRecs = @NumberOfUnMatchedRecs, "
        //                        + " RemainReconcExceptions = @RemainReconcExceptions, "
        //                        + " GL_Remain_Atms_Cash_Diff = @GL_Remain_Atms_Cash_Diff "
        //                        + " WHERE CategoryId =@CategoryId AND RunningJobNo = @RunningJobNo", conn))
        //                {
        //                    cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
        //                    cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
        //                    cmd.Parameters.AddWithValue("@NumberOfUnMatchedRecs", NumberOfUnMatchedRecs);
        //                    cmd.Parameters.AddWithValue("@RemainReconcExceptions", RemainReconcExceptions);
        //                    cmd.Parameters.AddWithValue("@GL_Remain_Atms_Cash_Diff", GL_Remain_Atms_Cash_Diff);

        //                    cmd.ExecuteNonQuery();


        //}
        //                // Close conn
        //                conn.Close();
        //            }
        //            catch (Exception ex)
        //            {
        //                conn.Close();

        //                CatchDetails(ex);
        //            }
        //    }


        // ERRORS 

        decimal ErrAmount;

        public int NumberOfErrors;
        public int NumberOfErrJournal;
        public int ErrJournalThisCycle;
        public int NumberOfErrDep;
        public int NumberOfErrHost;
        public int ErrHostToday;
        public int ErrOutstanding; // Action was not taken on them 
        public int ErrorsAdjastingBalances;

        public bool ErrorsFound;
        public int WBanksClosedBalNubOfErr;
        public int WBanksClosedBalErrOutstanding;
        //public string WBanksClosedBalCurrNm; 

        public decimal MatchedTransAdjWithErrors;
        public decimal BanksClosedBalAdjWithErrors;

        public string AtmNo;

        public int WSesNo;
        public int WFunction;

        // READ Errors TO CALCULATE REFRESED BALANCES for this category and this RMCycle 
        //
        int WRMCycle;
        public void ReadAllErrorsFromCategSessionForAllAtmsWithErrors(string InCategoryId, int InRMCycle, decimal InBanksClosedBal, int InFunction)
        {
            WRMCycle = InRMCycle;
            WFunction = InFunction;
            BanksClosedBalAdjWithErrors = InBanksClosedBal;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorsFound = false; // Related to errors 

            NumberOfErrors = 0;
            NumberOfErrJournal = 0;
            ErrJournalThisCycle = 0;
            NumberOfErrDep = 0;
            NumberOfErrHost = 0;
            ErrHostToday = 0;
            ErrOutstanding = 0;

            ErrorsAdjastingBalances = 0;

            WBanksClosedBalNubOfErr = 0;
            WBanksClosedBalErrOutstanding = 0;

            int ErrId; int ErrType;
            string ErrDesc; int TraceNo; int SesNo;
            int UniqueRecordId; int TransType; string TransDescr;
            DateTime DateTime; bool NeedAction;

            string CurDes;
            bool DrCust; bool CrCust; bool UnderAction; bool DisputeAct;
            bool ManualAct; bool DrAtmCash; bool CrAtmCash;
            bool DrAtmSusp; bool CrAtmSusp; bool MainOnly;

            SqlString = "SELECT *"
                 + " FROM [dbo].[ErrorsTable] "
                 + " WHERE ErrId <> 55 AND CategoryId = @CategoryId AND RMCycle<=@RMCycle AND (OpenErr=1 OR (OpenErr=0 AND RMCycle = @RMCycle))  ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ErrorsFound = true;

                            // Read error Details

                            ErrId = (int)rdr["ErrId"];
                            ErrType = (int)rdr["ErrType"];
                            ErrDesc = rdr["ErrDesc"].ToString();
                            AtmNo = rdr["AtmNo"].ToString();
                            SesNo = (int)rdr["SesNo"];
                            TraceNo = (int)rdr["TraceNo"];

                            UniqueRecordId = (int)rdr["UniqueRecordId"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();
                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            UnderAction = (bool)rdr["UnderAction"];
                            DisputeAct = (bool)rdr["DisputeAct"];

                            ManualAct = (bool)rdr["ManualAct"];
                            DrAtmCash = (bool)rdr["DrAtmCash"];
                            CrAtmCash = (bool)rdr["CrAtmCash"];
                            DrAtmSusp = (bool)rdr["DrAtmSusp"];
                            CrAtmSusp = (bool)rdr["CrAtmSusp"];
                            MainOnly = (bool)rdr["MainOnly"];

                            NumberOfErrors = NumberOfErrors + 1;

                            if ((UnderAction == true || DisputeAct == true) & ErrId != 165)
                            {
                                BanksClosedBalAdjWithErrors = BanksClosedBalAdjWithErrors + ErrAmount;
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

                    CatchDetails(ex);
                }
        }

        // READ Errors TO CALCULATE REFRESED BALANCES 
        //
        public void ReadAllErrorsFromCategSessionGL(string InCategoryId, int InRMCycle,
            decimal InMatchedAmt, decimal InBanksClosedBal, int InFunction)
        {
            //int WRMCycle = InRMCycle; 
            WFunction = InFunction;

            MatchedTransAdjWithErrors = InMatchedAmt;
            BanksClosedBalAdjWithErrors = InBanksClosedBal;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorsFound = false; // Related to errors 

            NumberOfErrors = 0;
            NumberOfErrJournal = 0;
            ErrJournalThisCycle = 0;
            NumberOfErrDep = 0;
            NumberOfErrHost = 0;
            ErrHostToday = 0;
            ErrOutstanding = 0;

            ErrorsAdjastingBalances = 0;

            WBanksClosedBalNubOfErr = 0;
            WBanksClosedBalErrOutstanding = 0;

            int ErrId; int ErrType;
            string ErrDesc; int TraceNo; int SesNo;
            int TransType; string TransDescr;
            DateTime DateTime; bool NeedAction;

            string CurDes;
            bool DrCust; bool CrCust; bool UnderAction; bool DisputeAct;
            bool ManualAct; bool DrAtmCash; bool CrAtmCash;
            bool DrAtmSusp; bool CrAtmSusp; bool MainOnly;

            SqlString = "SELECT *"
                 + " FROM [dbo].[ErrorsTable] "
                 + " WHERE CategoryId = @CategoryId AND RMCycle<=@RMCycle AND (OpenErr=1 OR (OpenErr=0 AND ActionSes = @ActionSes ))  ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@ActionSes", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ErrorsFound = true;

                            // Read error Details

                            ErrId = (int)rdr["ErrId"];
                            ErrType = (int)rdr["ErrType"];
                            ErrDesc = rdr["ErrDesc"].ToString();
                            SesNo = (int)rdr["SesNo"];
                            TraceNo = (int)rdr["TraceNo"];
                            //TransNo = (int)rdr["TransNo"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();
                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            UnderAction = (bool)rdr["UnderAction"];
                            DisputeAct = (bool)rdr["DisputeAct"];
                            ManualAct = (bool)rdr["ManualAct"];
                            DrAtmCash = (bool)rdr["DrAtmCash"];
                            CrAtmCash = (bool)rdr["CrAtmCash"];
                            DrAtmSusp = (bool)rdr["DrAtmSusp"];
                            CrAtmSusp = (bool)rdr["CrAtmSusp"];
                            MainOnly = (bool)rdr["MainOnly"];

                            NumberOfErrors = NumberOfErrors + 1;
                            // (ErrType = 1 || ErrType = 2 || ErrType = 5)
                            // Values
                            // 1 : Withdrawl EJournal Errors
                            // 2 : Mainframe Withdrawl Errors
                            // 3 : Deposit Errors Journal 
                            // 4 : Deposit Mainframe Errors
                            // 5 : Created by user Errors = eg moving to suspense 
                            // 6 : Empty 
                            // 7 : Created System Errors 
                            // 
                            if (ErrType == 1)
                            {
                                NumberOfErrJournal = NumberOfErrJournal + 1;
                                if (SesNo == WSesNo) ErrJournalThisCycle = ErrJournalThisCycle + 1; // Errors in this journal 
                            }

                            if (ErrType == 2) NumberOfErrHost = NumberOfErrHost + 1;

                            if (ErrType == 3) NumberOfErrDep = NumberOfErrDep + 1;

                            if (ErrType == 2) // FIND Todays Host errors 
                            {
                                int result = DateTime.Compare(DateTime.Date, DateTime.Today);

                                if (result == 0) // Equal dates or less
                                {
                                    // Not done Repl

                                    ErrHostToday = ErrHostToday + 1;
                                }
                            }

                            if (UnderAction == false & DisputeAct == false & ManualAct == false & ErrId < 200) ErrOutstanding = ErrOutstanding + 1;

                            // FIND NUMBER OF ERRORS PER CURRENCY 

                            WBanksClosedBalNubOfErr = WBanksClosedBalNubOfErr + 1;

                            if (UnderAction == false & DisputeAct == false & ManualAct == false & (ErrType == 1 || ErrType == 2)) WBanksClosedBalErrOutstanding = WBanksClosedBalErrOutstanding + 1;

                            // MAKE ADJUSTMENTS ON BALANCES 
                            // SesNo = WSesNo means this is within this Repl Cycle
                            // 
                            if (((UnderAction == true || DisputeAct == true) & ManualAct == false & WFunction == 4 & SesNo == WSesNo)
                                || (UnderAction == true & ManualAct == false & WFunction == 4 & SesNo != WSesNo & ErrId > 100 & ErrId < 200)
                                || ((WFunction == 5 & ManualAct == false & NeedAction == true & SesNo == WSesNo) & ErrId < 200)
                                || ((WFunction == 5 & ManualAct == false & NeedAction == true & SesNo != WSesNo) & (ErrId > 100 & ErrId < 200))
                                )
                            {
                                ErrorsAdjastingBalances = ErrorsAdjastingBalances + 1;

                                //  if (DrCust == true )
                                if ((DrCust == true & CrAtmCash == true) || (CrAtmCash == true & DrAtmSusp == true))
                                {
                                    // Missing at target system and therefore it was reported as not matched 

                                    //MatchedTransAdjWithErrors = MatchedTransAdjWithErrors + ErrAmount;
                                    // Make it negative to subtract from GL 
                                    BanksClosedBalAdjWithErrors = BanksClosedBalAdjWithErrors - ErrAmount;
                                }
                                else // Cr customer as in double at Host 
                                {
                                    BanksClosedBalAdjWithErrors = BanksClosedBalAdjWithErrors + ErrAmount;
                                }

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

                    CatchDetails(ex);
                }
        }


    }
}



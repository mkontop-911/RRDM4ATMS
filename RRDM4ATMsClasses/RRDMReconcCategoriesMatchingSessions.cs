using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;

namespace RRDM4ATMs
{
    public class RRDMReconcCategoriesMatchingSessions
    {
     
       public int SeqNo ; 
       public string CategoryId;
       public string CategoryName;

       public DateTime StartDateTm;

       public string GlCurrency;
       public string GlAccountNo;

       public decimal GlYesterdaysBalance;
       public decimal GlTodaysBalance;
       public decimal MatchedTransAmt;
       public decimal NotMatchedTransAmt; 

       public DateTime EndDateTm;

       public int NumberOfProcessFiles ;
       public int NumberOfMatchedRecs; 
       public int NumberOfUnMatchedRecs ;

       public bool Difference; 

       public DateTime StartReconcDtTm ;
       public DateTime EndReconcDtTm ;
       public int SettledUnMatched; 
       public int RemainReconcExceptions ;

       public string FileId11;
       public string FileId12;
       public string FileId13;

       public int SourceFileName1StartKey;
       public int SourceFileName1EndKey;
       public string FileId21;
       public string FileId22;
       public string FileId23;
       public int SourceFileName2StartKey;
       public int SourceFileName2EndKey;
       public string FileId31;
       public string FileId32;
       public string FileId33;
       public int SourceFileName3StartKey;
       public int SourceFileName3EndKey;
       public string FileId41;
       public string FileId42;
       public string FileId43;
       public int SourceFileName4StartKey;
       public int SourceFileName4EndKey;
       public string FileId51;
       public string FileId52;
       public string FileId53;
       public int SourceFileName5StartKey;
       public int SourceFileName5EndKey;
       public string FileId61;
       public string FileId62;
       public string FileId63;
       public int SourceFileName6StartKey;
       public int SourceFileName6EndKey;
       public string FileId71;
       public string FileId72;
       public string FileId73;
       public int SourceFileName7StartKey;
       public int SourceFileName7EndKey;
       public bool OpenRecord;
       public string Operator;

       public int TotalFiles; 


       // Define the data table 
       public DataTable TableMatchingSessionsPerCategory = new DataTable();

       public int TotalSelected;
       public int TotalUnMatchedRecs; 

       string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        //
        // Methods 
        // READ ReconcCategoriesMatchingSessions
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesMatchingSessionsSpecificCat(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMatchingSessionsPerCategory = new DataTable();
            TableMatchingSessionsPerCategory.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableMatchingSessionsPerCategory.Columns.Add("MRCycleNo", typeof(int));
            TableMatchingSessionsPerCategory.Columns.Add("CategoryId", typeof(string));
            //TableMatchingSessionsPerCategory.Columns.Add("Category_Name", typeof(string));
            TableMatchingSessionsPerCategory.Columns.Add("StartDateTm", typeof(DateTime));
            TableMatchingSessionsPerCategory.Columns.Add("EndDateTm", typeof(DateTime));
            TableMatchingSessionsPerCategory.Columns.Add("NumberOfUnMatchedRecs", typeof(int));
            TableMatchingSessionsPerCategory.Columns.Add("NumberOfProcessFiles", typeof(int));
            TableMatchingSessionsPerCategory.Columns.Add("NumberOfMatchedRecs", typeof(int));
            
            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategoriesMatchingSessions] "
                    + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
                    + " ORDER BY SeqNo DESC";

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

                            SeqNo = (int)rdr["SeqNo"];
                           
                            CategoryId = (string)rdr["CategoryId"];
                            CategoryName = (string)rdr["CategoryName"];

                            StartDateTm = (DateTime)rdr["StartDateTm"];

                            GlCurrency = (string)rdr["GlCurrency"];
                            GlAccountNo = (string)rdr["GlAccountNo"];

                            GlYesterdaysBalance = (decimal)rdr["GlYesterdaysBalance"];
                            GlTodaysBalance = (decimal)rdr["GlTodaysBalance"];
                            MatchedTransAmt = (decimal)rdr["MatchedTransAmt"];
                            NotMatchedTransAmt = (decimal)rdr["NotMatchedTransAmt"];

                            EndDateTm = (DateTime)rdr["EndDateTm"];

                            NumberOfProcessFiles = (int)rdr["NumberOfProcessFiles"];

                            NumberOfMatchedRecs = (int)rdr["NumberOfMatchedRecs"];
                            NumberOfUnMatchedRecs = (int)rdr["NumberOfUnMatchedRecs"];

                            Difference = (bool)rdr["Difference"];

                            StartReconcDtTm = (DateTime)rdr["StartReconcDtTm"];
                            EndReconcDtTm = (DateTime)rdr["EndReconcDtTm"];
                            SettledUnMatched = (int)rdr["SettledUnMatched"];
                            RemainReconcExceptions = (int)rdr["RemainReconcExceptions"];

                            FileId11 = (string)rdr["FileId11"];
                            FileId12 = (string)rdr["FileId12"];
                            FileId13 = (string)rdr["FileId13"];
                            SourceFileName1StartKey = (int)rdr["SourceFileName1StartKey"];
                            SourceFileName1EndKey = (int)rdr["SourceFileName1EndKey"];

                            FileId21 = (string)rdr["FileId21"];
                            FileId22 = (string)rdr["FileId22"];
                            FileId23 = (string)rdr["FileId23"];
                            SourceFileName2StartKey = (int)rdr["SourceFileName2StartKey"];
                            SourceFileName2EndKey = (int)rdr["SourceFileName2EndKey"];

                            FileId31 = (string)rdr["FileId31"];
                            FileId32 = (string)rdr["FileId32"];
                            FileId33 = (string)rdr["FileId33"];
                            SourceFileName3StartKey = (int)rdr["SourceFileName3StartKey"];
                            SourceFileName3EndKey = (int)rdr["SourceFileName3EndKey"];

                            FileId41 = (string)rdr["FileId41"];
                            FileId42 = (string)rdr["FileId42"];
                            FileId43 = (string)rdr["FileId43"];
                            SourceFileName4StartKey = (int)rdr["SourceFileName4StartKey"];
                            SourceFileName4EndKey = (int)rdr["SourceFileName4EndKey"];

                            FileId51 = (string)rdr["FileId51"];
                            FileId52 = (string)rdr["FileId52"];
                            FileId53 = (string)rdr["FileId53"];
                            SourceFileName5StartKey = (int)rdr["SourceFileName5StartKey"];
                            SourceFileName5EndKey = (int)rdr["SourceFileName5EndKey"];

                            FileId61 = (string)rdr["FileId61"];
                            FileId62 = (string)rdr["FileId62"];
                            FileId63 = (string)rdr["FileId63"];
                            SourceFileName6StartKey = (int)rdr["SourceFileName6StartKey"];
                            SourceFileName6EndKey = (int)rdr["SourceFileName6EndKey"];

                            FileId71 = (string)rdr["FileId71"];
                            FileId72 = (string)rdr["FileId72"];
                            FileId73 = (string)rdr["FileId73"];
                            SourceFileName6StartKey = (int)rdr["SourceFileName7StartKey"];
                            SourceFileName6EndKey = (int)rdr["SourceFileName7EndKey"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableMatchingSessionsPerCategory.NewRow();

                            RowSelected["MRCycleNo"] = SeqNo;
                            RowSelected["CategoryId"] = CategoryId;
                            //RowSelected["Category_Name"] = CategoryName;
                            RowSelected["StartDateTm"] = StartDateTm;
                            RowSelected["EndDateTm"] = EndDateTm;
                            RowSelected["NumberOfUnMatchedRecs"] = NumberOfUnMatchedRecs;
                            RowSelected["NumberOfProcessFiles"] = NumberOfProcessFiles;
                            RowSelected["NumberOfMatchedRecs"] = NumberOfMatchedRecs;                  

                            // ADD ROW
                            TableMatchingSessionsPerCategory.Rows.Add(RowSelected);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReconcCategoriesMatchingSessions......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ ReconcCategoriesMatchingSessions to find outstanding exceptions 
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesMatchingSessionsSpecificCatForExceptions(string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalUnMatchedRecs = 0;


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategoriesMatchingSessions] "
                    + " WHERE CategoryId = @CategoryId AND (StartReconcDtTm = @StartReconcDtTm OR StartReconcDtTm < @Today )" 
                    + "AND NumberOfUnMatchedRecs > 0 "; 
                  
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        cmd.Parameters.AddWithValue("@StartReconcDtTm", NullPastDate);

                        cmd.Parameters.AddWithValue("@Today", DateTime.Now.Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            NumberOfUnMatchedRecs = (int)rdr["NumberOfUnMatchedRecs"];


                            TotalUnMatchedRecs = TotalUnMatchedRecs + NumberOfUnMatchedRecs;

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReconcCategoriesMatchingSessions......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ ReconcCategoriesMatchingSessions Specific 
        // 
        //
        public void ReadReconcCategoriesMatchingSessionsByRmCycle(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0; 

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategoriesMatchingSessions] "
                    + " WHERE Operator = @Operator AND SeqNo = @SeqNo " ;
                 
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];
                            CategoryName = (string)rdr["CategoryName"];

                            StartDateTm = (DateTime)rdr["StartDateTm"];

                            GlCurrency = (string)rdr["GlCurrency"];
                            GlAccountNo = (string)rdr["GlAccountNo"];

                            GlYesterdaysBalance = (decimal)rdr["GlYesterdaysBalance"];
                            GlTodaysBalance = (decimal)rdr["GlTodaysBalance"];
                            MatchedTransAmt = (decimal)rdr["MatchedTransAmt"];
                            NotMatchedTransAmt = (decimal)rdr["NotMatchedTransAmt"];

                            EndDateTm = (DateTime)rdr["EndDateTm"];

                            NumberOfProcessFiles = (int)rdr["NumberOfProcessFiles"];
                            NumberOfMatchedRecs = (int)rdr["NumberOfMatchedRecs"];
                            NumberOfUnMatchedRecs = (int)rdr["NumberOfUnMatchedRecs"];

                            Difference = (bool)rdr["Difference"];

                            StartReconcDtTm = (DateTime)rdr["StartReconcDtTm"];

                            EndReconcDtTm = (DateTime)rdr["EndReconcDtTm"];
                            SettledUnMatched = (int)rdr["SettledUnMatched"];
                            
                            RemainReconcExceptions = (int)rdr["RemainReconcExceptions"];

                            FileId11 = (string)rdr["FileId11"];
                            FileId12 = (string)rdr["FileId12"];
                            FileId13 = (string)rdr["FileId13"];
                            SourceFileName1StartKey = (int)rdr["SourceFileName1StartKey"];
                            SourceFileName1EndKey = (int)rdr["SourceFileName1EndKey"];

                            FileId21 = (string)rdr["FileId21"];
                            FileId22 = (string)rdr["FileId22"];
                            FileId23 = (string)rdr["FileId23"];
                            SourceFileName2StartKey = (int)rdr["SourceFileName2StartKey"];
                            SourceFileName2EndKey = (int)rdr["SourceFileName2EndKey"];

                            FileId31 = (string)rdr["FileId31"];
                            FileId32 = (string)rdr["FileId32"];
                            FileId33 = (string)rdr["FileId33"];
                            SourceFileName3StartKey = (int)rdr["SourceFileName3StartKey"];
                            SourceFileName3EndKey = (int)rdr["SourceFileName3EndKey"];

                            FileId41 = (string)rdr["FileId41"];
                            FileId42 = (string)rdr["FileId42"];
                            FileId43 = (string)rdr["FileId43"];
                            SourceFileName4StartKey = (int)rdr["SourceFileName4StartKey"];
                            SourceFileName4EndKey = (int)rdr["SourceFileName4EndKey"];

                            FileId51 = (string)rdr["FileId51"];
                            FileId52 = (string)rdr["FileId52"];
                            FileId53 = (string)rdr["FileId53"];
                            SourceFileName5StartKey = (int)rdr["SourceFileName5StartKey"];
                            SourceFileName5EndKey = (int)rdr["SourceFileName5EndKey"];

                            FileId61 = (string)rdr["FileId61"];
                            FileId62 = (string)rdr["FileId62"];
                            FileId63 = (string)rdr["FileId63"];
                            SourceFileName6StartKey = (int)rdr["SourceFileName6StartKey"];
                            SourceFileName6EndKey = (int)rdr["SourceFileName6EndKey"];

                            FileId71 = (string)rdr["FileId71"];
                            FileId72 = (string)rdr["FileId72"];
                            FileId73 = (string)rdr["FileId73"];
                            SourceFileName6StartKey = (int)rdr["SourceFileName7StartKey"];
                            SourceFileName6EndKey = (int)rdr["SourceFileName7EndKey"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

                            if (FileId11 != "") TotalFiles = TotalFiles + 1;
                            if (FileId21 != "") TotalFiles = TotalFiles + 1;
                            if (FileId31 != "") TotalFiles = TotalFiles + 1;
                            if (FileId41 != "") TotalFiles = TotalFiles + 1;
                            if (FileId51 != "") TotalFiles = TotalFiles + 1;
                            if (FileId61 != "") TotalFiles = TotalFiles + 1; 
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReconcCategoriesMatchingSessions......... " + ex.Message;

                }
        }
        // 
        // UPDATE RM Category Cycle Start date time 
        // 
        public void UpdateCategRMCycleWithReconStartDate(string InCategoryId, int InMatchSession)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesMatchingSessions] SET "
                            + " StartReconcDtTm = @StartReconcDtTm "
                            + " WHERE SeqNo = @SeqNo", conn))
                    { 
                        cmd.Parameters.AddWithValue("@SeqNo", InMatchSession);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@StartReconcDtTm", StartReconcDtTm); 

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UpdateCategMatchSession Class............. " + ex.Message;
                }
        }
  //
        // UPDATE Category Session at Reconciliation closing 
  // 
        public void UpdateCategRMCycleWithAuthorClosing(string InCategoryId, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesMatchingSessions] SET "
                            + " EndReconcDtTm = @EndReconcDtTm , SettledUnMatched = @SettledUnMatched ,RemainReconcExceptions = @RemainReconcExceptions "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InRMCycle);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@EndReconcDtTm", EndReconcDtTm);
                        cmd.Parameters.AddWithValue("@SettledUnMatched", SettledUnMatched);
                        cmd.Parameters.AddWithValue("@RemainReconcExceptions", RemainReconcExceptions);

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UpdateCategMatchSession Class............. " + ex.Message;
                }
        }
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
        public void ReadAllErrorsTableFromCategSessionForAllAtmsWithErrors(string InCategoryId, int InActionSes, decimal InBanksClosedBal, int InFunction)
        {
            WSesNo = InActionSes;
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
            int TransNo; int TransType; string TransDescr;
            DateTime DateTime; bool NeedAction;

            string CurDes;
            bool DrCust; bool CrCust; bool UnderAction;
            bool ManualAct; bool DrAtmCash; bool CrAtmCash;
            bool DrAtmSusp; bool CrAtmSusp; bool MainOnly;

            SqlString = "SELECT *"
                 + " FROM [dbo].[ErrorsTable] "
                 + " WHERE CategoryId = @CategoryId AND SesNo<=@ActionSes AND (OpenErr=1 OR (OpenErr=0 AND ActionSes = @ActionSes))  ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@ActionSes", InActionSes);

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
                            TransNo = (int)rdr["TransNo"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();
                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            UnderAction = (bool)rdr["UnderAction"];
                            ManualAct = (bool)rdr["ManualAct"];
                            DrAtmCash = (bool)rdr["DrAtmCash"];
                            CrAtmCash = (bool)rdr["CrAtmCash"];
                            DrAtmSusp = (bool)rdr["DrAtmSusp"];
                            CrAtmSusp = (bool)rdr["CrAtmSusp"];
                            MainOnly = (bool)rdr["MainOnly"];

                            NumberOfErrors = NumberOfErrors + 1;

                            NumberOfErrors = NumberOfErrors + 1;

                            if (UnderAction == true & ErrId != 165) 
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
                }
        }   

        // READ Errors TO CALCULATE REFRESED BALANCES 
        //
        public void ReadAllErrorsTableFromCategSessionGL(string InCategoryId, int InRMCycle, 
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
            int TransNo; int TransType; string TransDescr;
            DateTime DateTime; bool NeedAction;

            string CurDes;
            bool DrCust; bool CrCust; bool UnderAction;
            bool ManualAct; bool DrAtmCash; bool CrAtmCash;
            bool DrAtmSusp; bool CrAtmSusp; bool MainOnly;

            SqlString = "SELECT *"
                 + " FROM [dbo].[ErrorsTable] "
                 + " WHERE CategoryId = @CategoryId AND RMCycle<=@RMCycle AND (OpenErr=1 OR (OpenErr=0 AND ActionSes =" + InRMCycle + "))  ";
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
                            SesNo = (int)rdr["SesNo"];
                            TraceNo = (int)rdr["TraceNo"];
                            TransNo = (int)rdr["TransNo"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();
                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            UnderAction = (bool)rdr["UnderAction"];
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

                            if (UnderAction == false & ManualAct == false & ErrId < 200) ErrOutstanding = ErrOutstanding + 1;

                            // FIND NUMBER OF ERRORS PER CURRENCY 

                            WBanksClosedBalNubOfErr = WBanksClosedBalNubOfErr + 1;

                            if (UnderAction == false & ManualAct == false & (ErrType == 1 || ErrType == 2)) WBanksClosedBalErrOutstanding = WBanksClosedBalErrOutstanding + 1;
                           
                            // MAKE ADJUSTMENTS ON BALANCES 
                            // SesNo = WSesNo means this is within this Repl Cycle
                            // 
                            if ((UnderAction == true & ManualAct == false & WFunction == 4 & SesNo == WSesNo)
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
                }
        }   
    }
}

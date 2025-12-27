using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMatchingCategoriesSessions : Logger
    {
        public RRDMMatchingCategoriesSessions() : base() { }

        public int SeqNo;
        public string CategoryId;
        public string CategoryName;

        public int RunningJobNo;

        public int LastRRDMKey;
        public string ITMXUniqueTxnRef;

        public DateTime StartDateTm;

        public string GlCurrency;
        public string GlAccountNo;

        public decimal GlYesterdaysBalance;
        public decimal GlTodaysBalance;
        public decimal MatchedTransAmt;
        public decimal NotMatchedTransAmt;

        public DateTime EndDateTm;

        public int NumberOfProcessFiles;
        public int NumberOfMatchedRecs;
        public int NumberOfUnMatchedRecs;

        public bool Difference;

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

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // READER FIELDS 
        private void MatchingCatSessionsReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CategoryId = (string)rdr["CategoryId"];
            CategoryName = (string)rdr["CategoryName"];

            RunningJobNo = (int)rdr["RunningJobNo"];

            LastRRDMKey = (int)rdr["LastRRDMKey"];
            ITMXUniqueTxnRef = (string)rdr["ITMXUniqueTxnRef"];

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
        }

        //
        // Methods 
        // READ ReconcCategoriesMatchingSessions
        // FILL UP A TABLE
        //
        public void ReadMatchingCategoriesSessionsSpecificCatFillTable(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMatchingSessionsPerCategory = new DataTable();
            TableMatchingSessionsPerCategory.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableMatchingSessionsPerCategory.Columns.Add("RunningJobNo", typeof(int));
            TableMatchingSessionsPerCategory.Columns.Add("CategoryId", typeof(string));
            //TableMatchingSessionsPerCategory.Columns.Add("Category_Name", typeof(string));
            TableMatchingSessionsPerCategory.Columns.Add("StartDateTm", typeof(DateTime));
            TableMatchingSessionsPerCategory.Columns.Add("EndDateTm", typeof(DateTime));
            TableMatchingSessionsPerCategory.Columns.Add("NumberOfUnMatchedRecs", typeof(int));
            TableMatchingSessionsPerCategory.Columns.Add("NumberOfProcessFiles", typeof(int));
            TableMatchingSessionsPerCategory.Columns.Add("NumberOfMatchedRecs", typeof(int));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategoriesSessions] "
                    + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
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

                            // Read Reader Fields
                            MatchingCatSessionsReaderFields(rdr);

                            DataRow RowSelected = TableMatchingSessionsPerCategory.NewRow();

                            RowSelected["RunningJobNo"] = RunningJobNo;
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

                    CatchDetails(ex);

                }
        }


        //
        // Methods 
        // READ ReconcCategoriesMatchingSessions to find status of matching 
        // FILL UP A TABLE
        //
        public void ReadMatchingCategoriesSessionsSpecificCatForRunningJob(string InCategoryId )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalUnMatchedRecs = 0;


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategoriesSessions] "
                    + " WHERE CategoryId = @CategoryId " 
                    +" ORDER BY RunningJobNo ASC ";

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


                            // Read Reader Fields
                            MatchingCatSessionsReaderFields(rdr);

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
        // READ ReconcCategoriesMatchingSessions to find outstanding exceptions 
        // FILL UP A TABLE
        //
        public void ReadMatchingCategoriesSessionsSpecificCatForExceptions(string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalUnMatchedRecs = 0;


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategoriesSessions] "
                    + " WHERE CategoryId = @CategoryId "
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

                        //cmd.Parameters.AddWithValue("@StartReconcDtTm", NullPastDate);

                        //cmd.Parameters.AddWithValue("@Today", DateTime.Now.Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Reader Fields
                            MatchingCatSessionsReaderFields(rdr);

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

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ CategoriesMatchingSessions Specific running Job Number  
        // 
        //
        public void ReadMatchingCategoriesSessionsByCatAndRunningJobNo
            (string InOperator,string InCategoryId, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategoriesSessions] "
                    + " WHERE Operator = @Operator AND CategoryId = @CategoryId AND RunningJobNo = @RunningJobNo ";

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


                            // Read Reader Fields
                            MatchingCatSessionsReaderFields(rdr);

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

                    CatchDetails(ex);

                }
        }
      
        // ERRORS 

        //decimal ErrAmount;

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

       

    }
}



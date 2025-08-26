using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//using System.Collections;


namespace RRDM4ATMs
{
    public class RRDMReconcCateqoriesVsMatchingCategories : Logger
    {
        public RRDMReconcCateqoriesVsMatchingCategories() : base() { }

        public int SeqNo;

        public string ReconcCategoryId;
        public int GroupId; 
        public string MatchingCategoryId;

        public DateTime OpeningDateTm;

        public string Operator;

        public string WMatchingCat01;
        public string WMatchingCat02;
        public string WMatchingCat03;
        public string WMatchingCat04;
        public string WMatchingCat05;
        public string WMatchingCat06;
        public string WMatchingCat07;
        public string WMatchingCat08;
        public string WMatchingCat09;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        // Define the data table 
        public DataTable TableReconcVsMatchingCateg = new DataTable();

        public int TotalSelected;
        public int TotalMatchingDone;
        public int TotalMatchingNotDone;
        public int TotalReconc;
        public int TotalNotReconc;
        public int TotalUnMatchedRecords;

        string SqlString; // Do not delete 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Reconc Cat Reader Fields 
        private void ReconcCatReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            ReconcCategoryId = (string)rdr["ReconcCategoryId"];
            GroupId = (int)rdr["GroupId"];
            MatchingCategoryId = (string)rdr["MatchingCategoryId"];
            OpeningDateTm = (DateTime)rdr["OpeningDateTm"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ ReconcCateqoriesVsMatchingCategories  
        // FILL UP A TABLE
        //
        public void ReadReconcCateqoriesVsMatchingCategoriesAndFillTable
                             (string InOperator, string InReconcCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcVsMatchingCateg = new DataTable();
            TableReconcVsMatchingCateg.Clear();

            TotalSelected = 0;

        // DATA TABLE ROWS DEFINITION 
            TableReconcVsMatchingCateg.Columns.Add("SeqNo", typeof(int));
            TableReconcVsMatchingCateg.Columns.Add("ReconcCategoryId", typeof(string));
            TableReconcVsMatchingCateg.Columns.Add("MatchingCategoryId", typeof(string));
            TableReconcVsMatchingCateg.Columns.Add("OpeningDateTm", typeof(DateTime));
          
            
                SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories] "
                   + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId "
                   + " ORDER BY ReconcCategoryId ASC ";
          

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            ReconcCatReaderFields(rdr);

                            DataRow RowSelected = TableReconcVsMatchingCateg.NewRow();
                          
                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["ReconcCategoryId"] = ReconcCategoryId;
                            RowSelected["MatchingCategoryId"] = MatchingCategoryId;
                            RowSelected["OpeningDateTm"] = OpeningDateTm;

                            // ADD ROW
                            TableReconcVsMatchingCateg.Rows.Add(RowSelected);

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
        // READ ReconcCateqoriesVsMatchingCategories  by Seq no  
        // 
        //
        public void ReadReconcCateqoriesVsMatchingCategoriesbySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories] "
                    + " WHERE Operator = @Operator AND SeqNo = @SeqNo";

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

                            // Read Fields 
                            ReconcCatReaderFields(rdr);

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
        // READ ReconcCateqoriesVsMatchingCategories by Cat Id   
        // 
        //
        public void ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0; 

            WMatchingCat01 = "";
            WMatchingCat02 = "";
            WMatchingCat03 = "";
            WMatchingCat04 = "";
            WMatchingCat05 = "";
            WMatchingCat06 = "";
            WMatchingCat07 = "";
            WMatchingCat08 = "";
            WMatchingCat09 = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories] "
                    + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            ReconcCatReaderFields(rdr);

                            if (TotalSelected == 1) WMatchingCat01 = MatchingCategoryId;
                            if (TotalSelected == 2) WMatchingCat02 = MatchingCategoryId;
                            if (TotalSelected == 3) WMatchingCat03 = MatchingCategoryId;
                            if (TotalSelected == 4) WMatchingCat04 = MatchingCategoryId;
                            if (TotalSelected == 5) WMatchingCat05 = MatchingCategoryId;
                            if (TotalSelected == 6) WMatchingCat06 = MatchingCategoryId;
                            if (TotalSelected == 7) WMatchingCat07 = MatchingCategoryId;
                            if (TotalSelected == 8) WMatchingCat08 = MatchingCategoryId;
                            if (TotalSelected == 9) WMatchingCat09 = MatchingCategoryId;

                            if (TotalSelected == 9)
                            {
                               // System.Windows.Forms.MessageBox.Show("Warning: 487116 There are more than EIGHT matching categories in Reconciliation" + ReconcCategoryId);
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

      



        // Insert ReconcCateqoriesVsMatchingCategories Category
        //
        public int InsertReconcCateqoriesVsMatchingCategories()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories]"
                    + "([ReconcCategoryId],"
                    + " [GroupId] ,"
                    + " [MatchingCategoryId],  "
                    + " [OpeningDateTm], "
                    + " [Operator] )"
                    + " VALUES (@ReconcCategoryId,"
                    + " @GroupId,"
                    + " @MatchingCategoryId,"
                    + " @OpeningDateTm, "
                    + " @Operator )"
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
       
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", ReconcCategoryId);

                        cmd.Parameters.AddWithValue("@GroupId", GroupId);
                        cmd.Parameters.AddWithValue("@MatchingCategoryId", MatchingCategoryId);

                        cmd.Parameters.AddWithValue("@OpeningDateTm", OpeningDateTm);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        SeqNo = (int)cmd.ExecuteScalar();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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

        // UPDATE ReconcCateqoriesVsMatchingCategories
        // 
        public void UpdateReconcCateqoriesVsMatchingCategories(string InOperator, string InCategoryId)
        {
            Successful = false;
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories] SET "
                            + " ReconcCategoryId = @ReconcCategoryId, "
                            + " GroupId = @GroupId, "
                            + " MatchingCategoryId = @MatchingCategoryId  "
                            + " WHERE ReconcCategoryId = @ReconcCategoryId", conn))
                    {

                        cmd.Parameters.AddWithValue("@ReconcCategoryId", ReconcCategoryId);
                        cmd.Parameters.AddWithValue("@GroupId", GroupId);
                        cmd.Parameters.AddWithValue("@MatchingCategoryId", MatchingCategoryId);

                        cmd.Parameters.AddWithValue("@OpeningDateTm", OpeningDateTm);

                        cmd.Parameters.AddWithValue("@Operator", Operator);


                        //rows number of record got updated

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
        // DELETE ReconcCateqoriesVsMatchingCategories
        //
        public void DeleteReconcCateqoriesVsMatchingCategoriesBySeqNo(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        //rows number of record got updated

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
        // DELETE ReconcCateqoriesVsMatchingCategories for all of this Reconciliation Category 
        //
        public void DeleteReconcCateqoriesVsMatchingCategoriesByReconcCategory(string InReconcCategoryId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            // Delete other table Entries - Categories Matching Categories

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories] "
                            + " WHERE ReconcCategoryId =  @ReconcCategoryId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);

                        //rows number of record got updated

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


    }
}

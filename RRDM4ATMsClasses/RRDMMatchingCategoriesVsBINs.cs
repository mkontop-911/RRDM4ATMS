using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMatchingCategoriesVsBINs : Logger
    {
        public RRDMMatchingCategoriesVsBINs() : base() { }

        public int SeqNo;

        public string CategoryId;

        public string BIN;

        public string BIN_Description;

        public string Origin;

        public string Operator; 

        // Define the data table 
        public DataTable TableMatchingCategoriesVsBINs = new DataTable();
        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

  //
  // Reader fields 
  //
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            CategoryId = (string)rdr["CategoryId"];
            BIN = (string)rdr["BIN"];
            BIN_Description = (string)rdr["BIN_Description"];

        }

        //
        // READ MatchingFields to fill table 
        //
        public void ReadReconcMatchingFieldsToFillDataTableByCategory(string InOperator, string InOrigin, string InCategoryId)
        { 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //WSelectionCriteria = " WHERE CategoryId = @CategoryId ";
            TableMatchingCategoriesVsBINs = new DataTable();
            TableMatchingCategoriesVsBINs.Clear();

            TotalSelected = 0;


            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsBINs]"
               + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
               + " ORDER By BIN ";

            using (SqlConnection conn =
            new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        sqlAdapt.Fill(TableMatchingCategoriesVsBINs);

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
        // READ MatchingFields to fill table 
        //
        public void ReadReconcMatchingFieldsToFillDataTable_AllCategories(string InOperator, string InOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //WSelectionCriteria = " WHERE CategoryId = @CategoryId ";
            TableMatchingCategoriesVsBINs = new DataTable();
            TableMatchingCategoriesVsBINs.Clear();

            TotalSelected = 0;

         
            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsBINs]"
               + " WHERE Operator = @Operator AND Origin = @Origin "
               + " ORDER By BIN ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Origin", InOrigin);

                        sqlAdapt.Fill(TableMatchingCategoriesVsBINs);
                        RecordFound = true;
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
        // READ MatchingCategories Vs BINs SINGLE By Selection Criteria 
        //
        string SqlString;
        public void ReadMatchingCategoriesVsBINsBySelectionCriteria(string InOperator, int InSeqNo, string InBIN , string InOrigin,int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //WSelectionCriteria = " WHERE BIN = @BIN " + textBoxBIN.Text + "'";
            //WSelectionCriteria = " WHERE SeqNo = @SeqNo "; 

            if (InMode == 11)
            {
                SqlString = "SELECT *"
                              + " FROM [ATMS].[dbo].[MatchingCategoriesVsBINs]"
                              + " WHERE Operator = @Operator AND SeqNo = @SeqNo ";
            }
            if (InMode == 12)
            {
                SqlString = "SELECT *"
                              + " FROM [ATMS].[dbo].[MatchingCategoriesVsBINs]"
                              + " WHERE Operator = @Operator AND BIN = @BIN AND Origin = @Origin ";
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

                        if (InMode == 11)
                        {
                            cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        }
                        if (InMode == 12)
                        {
                            cmd.Parameters.AddWithValue("@BIN", InBIN);
                            cmd.Parameters.AddWithValue("@Origin", InOrigin);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

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

        // Insert MatchingCategoryVsBINs Record 
        //
        public void InsertMatchingCategoryVsBIN_FromIST_OR_TWIN(string InOperator, string InFileId
                               , int InRMCycle, string InCategSet)
        {

            // DELETE IF ANY WITHIN THIS CYCLE 

            ErrorFound = false;
            ErrorOutput = "";
            int stpLineCount = 0;

            string cmdstring = " DELETE FROM [ATMS].[dbo].[MatchingCategoriesVsBINs] "
                   
                    + " WHERE  RMCycle = @RMCycle AND CategoryId IN " 
                    + InCategSet
                    //+ "('BDC277','BDC278','BDC279') "     
                    ;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdstring, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                       

                        cmd.CommandTimeout = 120;  // seconds
                        stpLineCount = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    ErrorFound = true;
                    conn.Close();

                    CatchDetails(ex);
                }

            // INSERT 
            ErrorFound = false;
            ErrorOutput = "";
            stpLineCount = 0;

            cmdstring = "INSERT INTO [ATMS].[dbo].[MatchingCategoriesVsBINs]"
                    + "([CategoryId] "
                    + ", [BIN]  "
                    + " ,[Operator] "
                     + " ,[RMCycle] "
                    + ") "
                    + " Select "
                    + " MatchingCateg,Left(Cardnumber,6) "
                    + " ,@Operator "
                    + " ,@RMCycle "
                    + " From [RRDM_Reconciliation_ITMX].[dbo]." + InFileId
                    + " WHERE  ResponseCode =  '0' " 
                    + " AND MatchingCateg in "
                    + InCategSet 
                    //+ "('BDC277','BDC278','BDC279') " 
                    + " AND LoadedAtRMCycle = @RMCycle "
                    + " group by MatchingCateg,Left(Cardnumber,6) "
                    + " ORDER by MatchingCateg "
                    + " "
                    ;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdstring, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        cmd.CommandTimeout = 120;  // seconds
                        stpLineCount = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    ErrorFound = true;
                    conn.Close();

                    CatchDetails(ex);
                }

            
        }


        // Insert MatchingCategoryVsBINs Record 
        //
        public int InsertMatchingCategoryVsBIN()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[MatchingCategoriesVsBINs]"
                    + "([CategoryId] " 
                    + ", [BIN]  "
                    + " ,[BIN_Description] "
                    + " ,[Origin] "
                    + " ,[Operator] )"
                    + " VALUES (@CategoryId"
                    + " ,@BIN "
                    + " ,@BIN_Description "
                    + " ,@Origin "
                    + " ,@Operator )"
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@BIN", BIN);
                        cmd.Parameters.AddWithValue("@BIN_Description", BIN_Description);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
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

        // UPDATE MatchingCategoryVsBIN
        // 
        public void UpdateMatchingCategoryVsBIN(string InOperator, int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //int rows; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesVsBINs] SET "
                              + " CategoryId = @CategoryId, "
                              + " BIN = @BIN, "
                              + " BIN_Description = @BIN_Description  "
                              + " WHERE Operator = @Operator AND SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@BIN", BIN);
                        cmd.Parameters.AddWithValue("@BIN_Description", BIN_Description);


                      
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
        // DELETE MatchingCategoryVsBIN
        //
        public void DeleteMatchingCategoryVsBIN(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingCategoriesVsBINs] "
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



    }
}

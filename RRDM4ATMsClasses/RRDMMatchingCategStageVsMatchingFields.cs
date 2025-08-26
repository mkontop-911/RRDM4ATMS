using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{

    public class RRDMMatchingCategStageVsMatchingFields : Logger
    {
        public RRDMMatchingCategStageVsMatchingFields() : base() { }

        public int SeqNo;

        public string CategoryId;
        public string Stage;
       
        public string SourceFileNameA;
       
        public string SourceFileNameB;
   
        public string MatchingField;

        public int SortSequence;

        public string MatchingOperator; // Equal, Like, Variance

        public Decimal LowVarianceAmount;
        public Decimal UpperVarianceAmount;

        public int TotalRecords;

        //public DataTable  = new DataTable();
        // Define the data table 
        public DataTable ReconcCategStagesDataTable = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
                                 ["ATMSConnectionString"].ConnectionString;

        //RRDMMatchingFields Mf = new RRDMMatchingFields(); 
        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        // Reader Fields 
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CategoryId = (string)rdr["CategoryId"];
            Stage = (string)rdr["Stage"];

            SourceFileNameA = (string)rdr["SourceFileNameA"];

            SourceFileNameB = (string)rdr["SourceFileNameB"];

            MatchingField = (string)rdr["MatchingField"];

            SortSequence = (int)rdr["SortSequence"];

            MatchingOperator = (string)rdr["MatchingOperator"];
            LowVarianceAmount = (decimal)rdr["LowVarianceAmount"];
            UpperVarianceAmount = (decimal)rdr["UpperVarianceAmount"];
        }
        //
        // READ Category vs Matching Fields AND FILL TABLE  
        //
        public void ReadReconcCategVsMatchingFieldsDataTable(string InSelectionCriteria, string InTableStructureId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ReconcCategStagesDataTable = new DataTable();
            ReconcCategStagesDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ReconcCategStagesDataTable.Columns.Add("SeqNo", typeof(int));
            ReconcCategStagesDataTable.Columns.Add("Stage", typeof(string));
            ReconcCategStagesDataTable.Columns.Add("MatchingOperator", typeof(string));
            ReconcCategStagesDataTable.Columns.Add("MatchingField", typeof(string));
            ReconcCategStagesDataTable.Columns.Add("FieldDBName", typeof(string));
            ReconcCategStagesDataTable.Columns.Add("FieldType", typeof(string));
            ReconcCategStagesDataTable.Columns.Add("CategoryId", typeof(string));

            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields]"
                + " WHERE " + InSelectionCriteria
                + " ORDER BY Stage, SortSequence "; // Konto to guide uu if order can change based on matching fields priorities 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReaderFields(rdr);

                            //Fill Table 

                            DataRow RowSelected = ReconcCategStagesDataTable.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Stage"] = Stage;
                            RowSelected["MatchingOperator"] = MatchingOperator;
                            RowSelected["MatchingField"] = MatchingField;
                            Utd.ReadUniversalTableFieldsDefinitionToGetTechnical(MatchingField, InTableStructureId);
                            RowSelected["FieldDBName"] = Utd.FieldDBName;
                            RowSelected["FieldType"] = Utd.FieldType;
                            RowSelected["CategoryId"] = CategoryId;

                            // ADD ROW
                            ReconcCategStagesDataTable.Rows.Add(RowSelected);

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
        // READ Category vs Matching Fields AND FILL TABLE  
        //
        public void ReadReconcCategVsMatchingFieldsDataTableByMatchingCateg(string InMatchingCateg)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ReconcCategStagesDataTable = new DataTable();
            ReconcCategStagesDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ReconcCategStagesDataTable.Columns.Add("MatchingCateg", typeof(string));
            ReconcCategStagesDataTable.Columns.Add("MatchingField", typeof(string));
            
            SqlString = "SELECT CategoryId As MatchingCateg, MatchingField "
               + " FROM [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields] "
                + "WHERE Stage = 'Stage A' and CategoryId ='"+ InMatchingCateg + "'"
                + " ORDER BY SortSequence "; // Konto to guide uu if order can change based on matching fields priorities 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            CategoryId = (string)rdr["MatchingCateg"];
                            MatchingField = (string)rdr["MatchingField"];

                            //Fill Table 
                            if (TotalSelected == 1)
                            {
                                DataRow RowSelected1 = ReconcCategStagesDataTable.NewRow();

                                RowSelected1["MatchingCateg"] = CategoryId;
                                RowSelected1["MatchingField"] = CategoryId;

                                // ADD ROW
                                ReconcCategStagesDataTable.Rows.Add(RowSelected1);
                            }

                            DataRow RowSelected2 = ReconcCategStagesDataTable.NewRow();

                            RowSelected2["MatchingCateg"] = CategoryId;
                            RowSelected2["MatchingField"] = MatchingField;
                          
                            // ADD ROW
                            ReconcCategStagesDataTable.Rows.Add(RowSelected2);

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
        // READ Category vs Matching Fields ALL 
        //
        public void ReadReconcCategVsMatchingFieldsAll(string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalRecords = 0; 

            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields]"
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

                            TotalRecords = TotalRecords + 1;

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

        //
        // Create StringOfMatchingFields ForStageX
        //
        public string MatchingFieldsStageX;
        public string Dublicate_List_FieldsStageX;
        public string Dublicate_ON_FieldsStageX;
        string WTableStructureId; 
        public void CreateStringOfMatchingFieldsForStageX(string InCategoryId, string InStage)
        {

            if (InCategoryId.Substring(0, 3) == "QAH"
                || InCategoryId.Substring(3, 3) == "ETI"
                || InCategoryId.Substring(3, 3) == "IPN"
                )
            {
                WTableStructureId = "QAHERA";   
            }
            else
            {
                WTableStructureId = "Atms And Cards";
            }
            try
                {
                StringBuilder WOrderCriteriaBuildMatching1 = new StringBuilder();
                StringBuilder WOrderCriteriaBuildDublicate1 = new StringBuilder();
                StringBuilder WOrderCriteriaBuildDublicate2 = new StringBuilder();

                string SelectionCriteria = " CategoryId ='" + InCategoryId + "' AND Stage ='"+ InStage+ "'";
                ReadReconcCategVsMatchingFieldsDataTable(SelectionCriteria, WTableStructureId);

                int K = 0;
                string WFieldDBName4;
                string WFieldDBName5; // Matching
                string WFieldDBName6; // Dublicate String
                string WFieldDBName7; // Dublicate ON 



                //
                //VALIDATION 
                //
                while (K <= (ReconcCategStagesDataTable.Rows.Count - 1))
                {
                    WFieldDBName4 = (string)ReconcCategStagesDataTable.Rows[K]["FieldDBName"];
                    WFieldDBName5 = " c2." + WFieldDBName4 + " = c1." + WFieldDBName4; // For Matching
                    WFieldDBName6 = WFieldDBName4;
                    WFieldDBName7 = " y." + WFieldDBName4 + " = dt." + WFieldDBName4;

                    WOrderCriteriaBuildMatching1.Append(WFieldDBName5);
                    WOrderCriteriaBuildDublicate1.Append(WFieldDBName6);
                    WOrderCriteriaBuildDublicate2.Append(WFieldDBName7);

                    if (ReconcCategStagesDataTable.Rows.Count - 1 != K) WOrderCriteriaBuildMatching1.Append(" AND ");
                    if (ReconcCategStagesDataTable.Rows.Count - 1 == K) WOrderCriteriaBuildMatching1.Append(" ");

                    if (ReconcCategStagesDataTable.Rows.Count - 1 != K) WOrderCriteriaBuildDublicate1.Append(" , ");
                    if (ReconcCategStagesDataTable.Rows.Count - 1 == K) WOrderCriteriaBuildDublicate1.Append(" ");

                    if (ReconcCategStagesDataTable.Rows.Count - 1 != K) WOrderCriteriaBuildDublicate2.Append(" AND ");
                    if (ReconcCategStagesDataTable.Rows.Count - 1 == K) WOrderCriteriaBuildDublicate2.Append(" ");
                    K++; // Read Next entry of the table 
                }

                MatchingFieldsStageX = WOrderCriteriaBuildMatching1.ToString();
                Dublicate_List_FieldsStageX = WOrderCriteriaBuildDublicate1.ToString();
                Dublicate_ON_FieldsStageX = WOrderCriteriaBuildDublicate2.ToString();
            }
                catch (Exception ex)
                {
                    CatchDetails(ex);
                }
        }

        public void CreateStringOfMatchingFieldsForStageX_ABS(string InCategoryId, string InStage)
        {
            string temp = InCategoryId.Substring(0, 3);
            if (temp == "QAH"
                || temp == "ETI"
                || temp == "IPN"
                || temp == "EGA"
                )
            {
                WTableStructureId = "QAHERA";
            }
            else
            {
                WTableStructureId = "Atms And Cards";
            }
            try
            {
                StringBuilder WOrderCriteriaBuildMatching1 = new StringBuilder();
                StringBuilder WOrderCriteriaBuildDublicate1 = new StringBuilder();
                StringBuilder WOrderCriteriaBuildDublicate2 = new StringBuilder();

                string SelectionCriteria = " CategoryId ='" + InCategoryId + "' AND Stage ='" + InStage + "'";
                ReadReconcCategVsMatchingFieldsDataTable(SelectionCriteria, WTableStructureId);

                int K = 0;
                string WFieldDBName4;
                string WFieldDBName5; // Matching
                string WFieldDBName6; // Dublicate String
                string WFieldDBName7; // Dublicate ON 



                //
                //VALIDATION 
                //
                while (K <= (ReconcCategStagesDataTable.Rows.Count - 1))
                {
                    WFieldDBName4 = (string)ReconcCategStagesDataTable.Rows[K]["FieldDBName"];
                    if (WFieldDBName4 == "TransAmount")
                    {
                        //abs(t.TransAmount)
                        WFieldDBName5 = "abs(c2." + WFieldDBName4 + ") = abs(c1." + WFieldDBName4 + ")"; // for amount
                    }
                    else
                    {
                        WFieldDBName5 = " c2." + WFieldDBName4 + " = c1." + WFieldDBName4; // For other 
                    }
                  
                    WFieldDBName6 = WFieldDBName4;
                    WFieldDBName7 = " y." + WFieldDBName4 + " = dt." + WFieldDBName4;

                    WOrderCriteriaBuildMatching1.Append(WFieldDBName5);
                    WOrderCriteriaBuildDublicate1.Append(WFieldDBName6);
                    WOrderCriteriaBuildDublicate2.Append(WFieldDBName7);

                    if (ReconcCategStagesDataTable.Rows.Count - 1 != K) WOrderCriteriaBuildMatching1.Append(" AND ");
                    if (ReconcCategStagesDataTable.Rows.Count - 1 == K) WOrderCriteriaBuildMatching1.Append(" ");

                    if (ReconcCategStagesDataTable.Rows.Count - 1 != K) WOrderCriteriaBuildDublicate1.Append(" , ");
                    if (ReconcCategStagesDataTable.Rows.Count - 1 == K) WOrderCriteriaBuildDublicate1.Append(" ");

                    if (ReconcCategStagesDataTable.Rows.Count - 1 != K) WOrderCriteriaBuildDublicate2.Append(" AND ");
                    if (ReconcCategStagesDataTable.Rows.Count - 1 == K) WOrderCriteriaBuildDublicate2.Append(" ");
                    K++; // Read Next entry of the table 
                }

                MatchingFieldsStageX = WOrderCriteriaBuildMatching1.ToString();
                Dublicate_List_FieldsStageX = WOrderCriteriaBuildDublicate1.ToString();
                Dublicate_ON_FieldsStageX = WOrderCriteriaBuildDublicate2.ToString();
            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }
        }
        // Create Matching String
        public string CreateStringOfMatchingFieldsForStage_String(string InCategoryId, string InStage)
        {


            string WMatchingString = "";
            try
            {
                if (InCategoryId.Substring(0, 3) == "QAH"
                || InCategoryId.Substring(3, 3) == "ETI"
                || InCategoryId.Substring(3, 3) == "IPN"
                
               )
                {
                    WTableStructureId = "QAHERA";
                }
                else
                {
                    WTableStructureId = "Atms And Cards";
                }

                string SelectionCriteria = " CategoryId ='" + InCategoryId + "' AND Stage ='" + InStage + "'";
                ReadReconcCategVsMatchingFieldsDataTable(SelectionCriteria, WTableStructureId);

                int K = 0;
                
                int WRows = ReconcCategStagesDataTable.Rows.Count; 
                //
                //VALIDATION 
                //
                while (K <= (ReconcCategStagesDataTable.Rows.Count - 1))
                {
                   // TransDate.Date.ToString() + RRNumber + TransAmt.ToString() + CardNumber;

                    string WFieldDBName = (string)ReconcCategStagesDataTable.Rows[K]["FieldDBName"];
                    string WFieldType = (string)ReconcCategStagesDataTable.Rows[K]["FieldType"];
                    // Types
                    // Character, Numeric,Decimal,  Date
                    //[TerminalId] + CAST(TransAmt As nvarchar) + CAST(TransDate As nvarchar)
                   
                    if (WFieldType == "Numeric" || WFieldType == "Decimal" || WFieldType == "DateTime")
                    {
                        WFieldDBName = "CAST(" + WFieldDBName + " As nvarchar)";
                    }
                    
                    WMatchingString = WMatchingString + WFieldDBName;
                    
                    if (K == (WRows-1)  )
                    {
                        // Do nothing  
                    }
                    else
                    {
                        WMatchingString = WMatchingString + "+";
                    }
                    
                    
                    K++; // Read Next entry of the table 
                }

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

            return WMatchingString; 
        }

        //
        // READ CategoryStage vs Matching Fields ALL 
        //
        public void ReadReconcCategStageVsMatchingField(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

          
            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields]"
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
        //
        // READ Category vs Matching Fields
        //
        public void ReadReconcCategVsMatchingFields(string InCategoryId, string InSourceFileNameA, string InSourceFileNameB,string InMatchingField)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields]"
               + " WHERE CategoryId = @CategoryId " 
               + " AND SourceFileNameA = @SourceFileNameA AND SourceFileNameB = @SourceFileNameB"
               + " AND MatchingField = @MatchingField ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@SourceFileNameA", InSourceFileNameA);
                        cmd.Parameters.AddWithValue("@SourceFileNameB", InSourceFileNameB);
                        cmd.Parameters.AddWithValue("@MatchingField", InMatchingField);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

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

        // Insert Category Vs MatchingFields Record 
        //
        public void InsertReconcCategVsMatchingFieldsRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields]"
                    + "([CategoryId], [Stage], "
                    + " [SourceFileNameA],[SourceFileNameB],"
                    + " [MatchingField] , [SortSequence])"
                    + " VALUES (@CategoryId,@Stage, "
                    + " @SourceFileNameA, @SourceFileNameB,"
                    + " @MatchingField ,@SortSequence)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@Stage", Stage);
                        cmd.Parameters.AddWithValue("@SourceFileNameA", SourceFileNameA);
                      
                        cmd.Parameters.AddWithValue("@SourceFileNameB", SourceFileNameB);
                   
                        cmd.Parameters.AddWithValue("@MatchingField", MatchingField);

                        cmd.Parameters.AddWithValue("@SortSequence", SortSequence);


                        

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
        // UPDATE ATMs Basic 
        //
        public void UpdateCategStageVsMatchingFieldsRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields] SET "
                            + "[MatchingOperator] =@MatchingOperator, "
                            + "[LowVarianceAmount] =@LowVarianceAmount, "
                            + "[UpperVarianceAmount] =@UpperVarianceAmount "
                            + " WHERE SeqNo= @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@MatchingOperator", MatchingOperator);
                        cmd.Parameters.AddWithValue("@LowVarianceAmount", LowVarianceAmount);
                        cmd.Parameters.AddWithValue("@UpperVarianceAmount", UpperVarianceAmount);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {

                        }

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            //  return outcome;

        }

        //
        // DELETE Category Vs Matching filed Record 
        //
        public void DeleteReconcCategVsMatchingFieldsRecord(string InCategoryId, string InSourceFileNameA, string InSourceFileNameB, string InMatchingField)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields]"
                            + " WHERE CategoryId = @CategoryId AND SourceFileNameA = @SourceFileNameA AND SourceFileNameB = @SourceFileNameB AND MatchingField = @MatchingField ", conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@SourceFileNameA", InSourceFileNameA);
                        cmd.Parameters.AddWithValue("@SourceFileNameB", InSourceFileNameB);
                        cmd.Parameters.AddWithValue("@MatchingField", InMatchingField);

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

        //--------------------------------------------
 // DELETE all records from matching fields  involving this file 
        //---------------------------------------------
        public void DeleteAllReconcCategVsMatchingFieldsForSourceFile(string InCategoryId, string InSourceFileNameX)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields]"
                            + " WHERE CategoryId = @CategoryId AND (SourceFileNameA = @SourceFileNameA OR SourceFileNameB = @SourceFileNameB) ", conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@SourceFileNameA", InSourceFileNameX);
                        cmd.Parameters.AddWithValue("@SourceFileNameB", InSourceFileNameX);
                      
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

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

namespace RRDM4ATMsWin
{
 
    public class RRDMReconcCategStageVsMatchingFields
    {
        
        public int SeqNo;

        public string ReconcCategory;
        public string Stage; 
        public string SourceFileNameA;
       
        public string SourceFileNameB;
   
        public string MatchingField;

        public string MatchingOperator; // Equal, Like, Variance

        public Decimal LowVarianceAmount;
        public Decimal UpperVarianceAmount;

        public int TotalRecords;

        // Define the data table 
        public DataTable ReconcCateg = new DataTable();
        public int TotalSelected;
        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //
        // READ Category vs Matching Fields ALL 
        //
        public void ReadReconcCategVsMatchingFieldsAll(string InReconcCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalRecords = 0; 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcCategoryStageVsMatchingFields]"
               + " WHERE ReconcCategory = @ReconcCategory "; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@ReconcCategory", InReconcCategory);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalRecords = TotalRecords + 1; 

                            SeqNo = (int)rdr["SeqNo"];

                            ReconcCategory = (string)rdr["ReconcCategory"];
                            Stage = (string)rdr["Stage"];
                            SourceFileNameA = (string)rdr["SourceFileNameA"];
                            
                            SourceFileNameB = (string)rdr["SourceFileNameB"];
                            
                            MatchingField = (string)rdr["MatchingField"];

                            MatchingOperator = (string)rdr["MatchingOperator"];
                            LowVarianceAmount = (Decimal)rdr["LowVarianceAmount"];
                            UpperVarianceAmount = (Decimal)rdr["UpperVarianceAmount"];

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
                    ErrorOutput = "An error occured in ReconcCategVsMatchingFields Class............. " + ex.Message;
                }
        }

        //
        // READ Category vs Matching Fields
        //
        public void ReadReconcCategVsMatchingFields(string InReconcCategory, string InSourceFileNameA, string InSourceFileNameB,string InMatchingField)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcCategoryStageVsMatchingFields]"
               + " WHERE ReconcCategory = @ReconcCategory " 
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
                        cmd.Parameters.AddWithValue("@ReconcCategory", InReconcCategory);
                        cmd.Parameters.AddWithValue("@SourceFileNameA", InSourceFileNameA);
                        cmd.Parameters.AddWithValue("@SourceFileNameB", InSourceFileNameB);
                        cmd.Parameters.AddWithValue("@MatchingField", InMatchingField);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            ReconcCategory = (string)rdr["ReconcCategory"];
                            Stage = (string)rdr["Stage"];
                            SourceFileNameA = (string)rdr["SourceFileNameA"];  
                            SourceFileNameB = (string)rdr["SourceFileNameB"];
                       
                            MatchingField = (string)rdr["MatchingField"];
                        
                            MatchingOperator = (string)rdr["MatchingOperator"];
                            LowVarianceAmount = (Decimal)rdr["LowVarianceAmount"];
                            UpperVarianceAmount = (Decimal)rdr["UpperVarianceAmount"];

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
                    ErrorOutput = "An error occured in ReconcCategVsMatchingFields Class............. " + ex.Message;
                }
        }

        // Insert Category Vs MatchingFields Record 
        //
        public void InsertReconcCategVsMatchingFieldsRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcCategoryStageVsMatchingFields]"
                    + "([ReconcCategory], [Stage], "
                    + " [SourceFileNameA],[SourceFileNameB],"
                    + " [MatchingField] )"
                    + " VALUES (@ReconcCategory,@Stage, "
                    + " @SourceFileNameA, @SourceFileNameB,"
                    + " @MatchingField )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@ReconcCategory", ReconcCategory);
                        cmd.Parameters.AddWithValue("@Stage", Stage);
                        cmd.Parameters.AddWithValue("@SourceFileNameA", SourceFileNameA);
                      
                        cmd.Parameters.AddWithValue("@SourceFileNameB", SourceFileNameB);
                   
                        cmd.Parameters.AddWithValue("@MatchingField", MatchingField);
                       
                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in InsertReconcCategVsMatchingFieldsRecord............. " + ex.Message;
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoryStageVsMatchingFields] SET "
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in CategStageVsMatchingFieldsRecord Class............. " + ex.Message;

                }

            //  return outcome;

        }

        //
        // DELETE Category Vs Matching filed Record 
        //
        public void DeleteReconcCategVsMatchingFieldsRecord(string InReconcCategory, string InSourceFileNameA, string InSourceFileNameB, string InMatchingField)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcCategoryVsMatchingFields]"
                            + " WHERE ReconcCategory = @ReconcCategory AND SourceFileNameA = @SourceFileNameA AND SourceFileNameB = @SourceFileNameB AND MatchingField = @MatchingField ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ReconcCategory", InReconcCategory);
                        cmd.Parameters.AddWithValue("@SourceFileNameA", InSourceFileNameA);
                        cmd.Parameters.AddWithValue("@SourceFileNameB", InSourceFileNameB);
                        cmd.Parameters.AddWithValue("@MatchingField", InMatchingField);

                        //rows number of record got updated

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
                    ErrorOutput = "An error occured in DeleteReconcCategoryVsVsMatchingFieldsRecord............. " + ex.Message;
                }

        }
    }
}

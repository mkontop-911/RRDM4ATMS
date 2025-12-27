using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMOutputFileFieldsMappingDefinition : Logger
    {
        public RRDMOutputFileFieldsMappingDefinition() : base() { }

        public int SeqNo;

        public string OutputFile_Id;
        public string OutputFile_Version;

        public string Source_Field_Nm;

        public string SourceFieldType;

        public string Target_Field_Nm;

        public string TargetFieldType;

        public int TargetFieldPositionStart;
        public int TargetFieldPositionEnd;

        public string TargetDefaultValue;

        public string TransformationRoutine;

        public string Operator; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        // Define the data table 
        public DataTable DataTableFileFields = new DataTable();

        public int TotalSelected;
        string SqlString; // Do not delete 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // Reader fields 
        //
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            OutputFile_Id = (string)rdr["OutputFile_Id"];
            OutputFile_Version = (string)rdr["OutputFile_Version"];

            Source_Field_Nm = (string)rdr["Source_Field_Nm"];

            SourceFieldType = (string)rdr["SourceFieldType"];      

            Target_Field_Nm = (string)rdr["Target_Field_Nm"];

            TargetFieldType = (string)rdr["TargetFieldType"];

            TargetFieldPositionStart = (int)rdr["TargetFieldPositionStart"];
            TargetFieldPositionEnd = (int)rdr["TargetFieldPositionEnd"];

            TargetDefaultValue = (string)rdr["TargetDefaultValue"];

            TransformationRoutine = (string)rdr["TransformationRoutine"];

            Operator = (string)rdr["Operator"];

        }

        //
        // READ OutputFileFieldsDefinition to fill table - BY Selection Criteria 
        //
        public void ReadOutputFileFieldsDefinitionToFillDataTable(string InSelectionCriteria)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableFileFields = new DataTable();
            DataTableFileFields.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 


            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[OutputFileFieldsMappingDefinition] "
               + InSelectionCriteria
               + " Order By TargetFieldPositionStart";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableFileFields);

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
        // READ By Source Field 

        //
        public void ReadFileFieldsBySourceField( string InOutputFile_Id, string InOutputFile_Version, string InSource_Field_Nm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[OutputFileFieldsMappingDefinition] "
                   + " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version AND Source_Field_Nm = @Source_Field_Nm   "
                   + " ORDER BY SeqNo ASC ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@OutputFile_Id", InOutputFile_Id);
                        cmd.Parameters.AddWithValue("@OutputFile_Version", InOutputFile_Version);
                        cmd.Parameters.AddWithValue("@Source_Field_Nm", InSource_Field_Nm);
                        
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

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
        // Methods 
        // READ By Source Field 

        //
        public void ReadTableFieldsByTargetField(string InOutputFile_Id, string InOutputFile_Version ,string InTarget_Field_Nm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[OutputFileFieldsMappingDefinition] "
                   + " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version AND Target_Field_Nm = @Target_Field_Nm   "
                   + "  ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                      
                        cmd.Parameters.AddWithValue("@OutputFile_Id", InOutputFile_Id);
                        cmd.Parameters.AddWithValue("@OutputFile_Version", InOutputFile_Version);
                        cmd.Parameters.AddWithValue("@Target_Field_Nm", InTarget_Field_Nm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

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

        //
        // Methods 
        // READ ReconcCategory  by Seq no  
        // 
        //
        public void ReadReadFileFieldsbySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[OutputFileFieldsMappingDefinition] "
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
        // Methods 
        // READ ReconcCategory  by Seq no  
        // 
        //
        public string ReadCheckForBrokenSequence(string InOutputFile_Id, string InOutputFile_Version)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int OldEnd = 0 ;

            string BrokenSequence = "" ;  

            SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[OutputFileFieldsMappingDefinition] "
                  + " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version"
                  + " Order By TargetFieldPositionStart";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@OutputFile_Id", InOutputFile_Id);
                        cmd.Parameters.AddWithValue("@OutputFile_Version", InOutputFile_Version);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            if (OldEnd > 0)
                            {
                                if (OldEnd + 1 != TargetFieldPositionStart)
                                {
                                    // Problem 

                                    BrokenSequence = "Broken Sequence Just Before Source Field Name " + Source_Field_Nm;

                                }
                            }
                           
                            OldEnd = TargetFieldPositionEnd; 
                        
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
            return BrokenSequence; 
        }

        //
        // Methods 
        // READ CheckWithinOtherRange
        // 
        //
        public string ReadCheckWithinOtherRange(string InOutputFile_Id, string InOutputFile_Version , string InSource_Field_Nm, int InFrom, int InTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string Illegal = ""; 

            SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[OutputFileFieldsMappingDefinition] "
                  + " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version =@OutputFile_Version  "
                  + " Order By TargetFieldPositionStart";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                      
                        cmd.Parameters.AddWithValue("@OutputFile_Id", InOutputFile_Id);
                        cmd.Parameters.AddWithValue("@OutputFile_Version", InOutputFile_Version);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            if (InFrom >= TargetFieldPositionStart & InFrom <= TargetFieldPositionEnd & InSource_Field_Nm != Source_Field_Nm)
                            {
                                // Illegal 
                                Illegal = "The From Position falls within range of " + Source_Field_Nm;
                                break;
                            }
                            

                            if (InTo >= TargetFieldPositionStart & InTo <= TargetFieldPositionEnd & InSource_Field_Nm != Source_Field_Nm)
                            {
                                // Illegal 
                                Illegal = "The To Position falls within range of " + Source_Field_Nm;
                                break;
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
            return Illegal; 
        }

        //SeqNo = (int)rdr["SeqNo"];

        //    OutputFile_Id = (string)rdr["OutputFile_Id"];
        //    OutputFile_Version = (int)rdr["OutputFile_Version"];

        //    Source_Field_Nm = (string)rdr["Source_Field_Nm"];

        //    Target_Field_Nm = (string)rdr["Target_Field_Nm"];

        //    TargetFieldType = (string)rdr["TargetFieldType"];

        //    TargetTargetFieldPositionStart = (int)rdr["TargetTargetFieldPositionStart"];
        //    TargetFieldPositionEnd = (int)rdr["TargetFieldPositionEnd"];

        //    TargetDefaultValue = (string)rdr["TargetDefaultValue"];

        //    TransformationRoutine = (string)rdr["TransformationRoutine"];

        //    Operator = (string)rdr["Operator"];

        // Insert File Field Record 
        //
        public void InsertFileFieldRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";
            
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[OutputFileFieldsMappingDefinition]"
                    + "([OutputFile_Id], [OutputFile_Version], "
                    + " [Source_Field_Nm],[SourceFieldType],"
                    + " [Target_Field_Nm],[TargetFieldType], "
                    + " [TargetFieldPositionStart], [TargetFieldPositionEnd], "
                    + " [TargetDefaultValue],  "
                    + " [TransformationRoutine], [Operator]"
                    + "  )"
                    + " VALUES (@OutputFile_Id, @OutputFile_Version,"
                    + " @Source_Field_Nm, @SourceFieldType,"
                    + "  @Target_Field_Nm, @TargetFieldType,"
                    + "  @TargetFieldPositionStart, @TargetFieldPositionEnd, "
                    + " @TargetDefaultValue,"
                    + " @TransformationRoutine, @Operator "
                    + " )";
           
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@OutputFile_Id", OutputFile_Id);
                        cmd.Parameters.AddWithValue("@OutputFile_Version", OutputFile_Version);

                        cmd.Parameters.AddWithValue("@Source_Field_Nm", Source_Field_Nm);

                        cmd.Parameters.AddWithValue("@SourceFieldType", SourceFieldType);

                        cmd.Parameters.AddWithValue("@Target_Field_Nm", Target_Field_Nm);

                        cmd.Parameters.AddWithValue("@TargetFieldType", TargetFieldType);

                        cmd.Parameters.AddWithValue("@TargetFieldPositionStart", TargetFieldPositionStart);
                        cmd.Parameters.AddWithValue("@TargetFieldPositionEnd", TargetFieldPositionEnd);

                        cmd.Parameters.AddWithValue("@TargetDefaultValue", TargetDefaultValue);

                        cmd.Parameters.AddWithValue("@TransformationRoutine", TransformationRoutine);
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

        // UPDATE Category
        // 
        public void UpdateFileFieldRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[OutputFileFieldsMappingDefinition] SET "
                              + " Source_Field_Nm = @Source_Field_Nm, "
                              + " SourceFieldType = @SourceFieldType, "
                              + " Target_Field_Nm = @Target_Field_Nm, "
                              + " TargetFieldType = @TargetFieldType, "
                              + " TargetFieldPositionStart = @TargetFieldPositionStart, "
                              + " TargetFieldPositionEnd = @TargetFieldPositionEnd, "
                              + " TargetDefaultValue = @TargetDefaultValue, "
                              + " TransformationRoutine = @TransformationRoutine "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@Source_Field_Nm", Source_Field_Nm);

                        cmd.Parameters.AddWithValue("@SourceFieldType", SourceFieldType);

                        cmd.Parameters.AddWithValue("@Target_Field_Nm", Target_Field_Nm);

                        cmd.Parameters.AddWithValue("@TargetFieldType", TargetFieldType);

                        cmd.Parameters.AddWithValue("@TargetFieldPositionStart", TargetFieldPositionStart);
                        cmd.Parameters.AddWithValue("@TargetFieldPositionEnd", TargetFieldPositionEnd);

                        cmd.Parameters.AddWithValue("@TargetDefaultValue", TargetDefaultValue);

                        cmd.Parameters.AddWithValue("@TransformationRoutine", TransformationRoutine);
                    
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
                    CatchDetails(ex);
                }
        }

        //
        // DELETE Category
        //
        public void DeleteFileFieldRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[OutputFileFieldsMappingDefinition] "
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



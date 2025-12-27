using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMUniversalTableFieldsDefinition : Logger
    {
        public RRDMUniversalTableFieldsDefinition() : base() { }

        public int SeqNo;

        public int SortSequence; // Sequence of field within the table 

        public string TableStructureId; // ATMs And Cards , Bank Account Vs CITs  

        public string FieldName;              
        public string FieldDBName;

        public string FieldType;
        public int FieldLength;

        public bool IsMatchingField;

        public bool IsPrimaryMatchingField;

        public bool IsMergedField;

        public string Application; // EG: 
        //                           // Trace Number is numeric and it is Primary matching field for Application:OUR_ATMS 
        //                           // RR Number is numeric and it is Primary matching field for Application: NOT_OUR_ATMS 
      

        string SqlString; 

        // Define the data table 
        public DataTable DataTableUniversalTableFields = new DataTable();

        public DataTable MatchingFieldsDataTable = new DataTable();

        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMMappingFileFieldsFromBankToRRDM Rff = new RRDMMappingFileFieldsFromBankToRRDM();
  //
  // Reader fields 
  //
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            SortSequence = (int)rdr["SortSequence"];

            TableStructureId = (string)rdr["TableStructureId"];

            FieldName = (string)rdr["FieldName"];
            FieldDBName = (string)rdr["FieldDBName"];

            FieldType = (string)rdr["FieldType"];
            FieldLength = (int)rdr["FieldLength"];

            IsMatchingField = (bool)rdr["IsMatchingField"];

            IsPrimaryMatchingField = (bool)rdr["IsPrimaryMatchingField"];

            IsMergedField = (bool)rdr["IsMergedField"];

            Application = (string)rdr["Application"];

          
        }

        //
        // READ UniversalTableFieldsDefinition to fill table 
        //
        public void ReadUniversalTableFieldsDefinitionToFillDataTable(string InSelectionCriteria)
        { 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableUniversalTableFields = new DataTable();
            DataTableUniversalTableFields.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 


            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[UniversalTableFieldsDefinition]"
               +   InSelectionCriteria
               + " Order By SortSequence ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableUniversalTableFields);
                        
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
        // READ Universal table by Selection Criteria 
        //
        public void ReadUniversalTableFieldsDefinitionBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[UniversalTableFieldsDefinition]"
               +  InSelectionCriteria ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FieldName", InFieldName);

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
        // READ Universal table by Selection Criteria 
        //
        public int PositionCount; 
        public void ReadUniversalTableFieldsDefinitionToFindPossition(int InSeqNo, string InTableStructureId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            
            PositionCount = -1 ; 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[UniversalTableFieldsDefinition]"
                  + " WHERE TableStructureId = @TableStructureId"
                  + " Order By SortSequence ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableStructureId", InTableStructureId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            PositionCount = PositionCount + 1 ; 

                            if (SeqNo == InSeqNo) break;

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
        // READ UniversalTableFieldsDefinition to RRDM File/Fields records 
        //
        public void ReadUniversalTableFieldsDefinitionToCreateBankToRRDMRecords( string InSource_File_Id, string InTableStructureId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "" ;

            if (InSource_File_Id == "Atms_Journals_Txns")
            {
                return; 
            }

            string SqlString = " SELECT * "
               + " FROM [ATMS].[dbo].[UniversalTableFieldsDefinition] "
               + " WHERE TableStructureId = @TableStructureId "
               + " Order By SortSequence ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@TableStructureId", InTableStructureId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            Rff.SourceFileId = InSource_File_Id;
                            Rff.SourceFieldNm = "Please Fill";
                            Rff.IsUniversal = true; 
                            Rff.TargetFieldType = FieldType;
                            Rff.SourceFieldPositionStart = 0;
                            Rff.SourceFieldPositionEnd = 0;
                            Rff.SourceFieldValue = "";
                            Rff.TargetFieldValue = "";                  

                            Rff.TargetFieldNm = FieldDBName;

                            Rff.ReadTableFieldsByTargetField(Rff.SourceFileId, Rff.TargetFieldNm); 
                            if (Rff.RecordFound)
                            {
                                // Do not ADD
                            }
                            else
                            {
                                Rff.InsertFileFieldRecord();
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
        // READ Universal Table MatchingFields to fill table 
        //
        public void ReadUniversalTableMatchingFieldsToFillDataTable( string InTableStructureId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MatchingFieldsDataTable = new DataTable();
            MatchingFieldsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            MatchingFieldsDataTable.Columns.Add("SeqNo", typeof(int));
            MatchingFieldsDataTable.Columns.Add("Field Name", typeof(string));
            MatchingFieldsDataTable.Columns.Add("DB Field Name", typeof(string));
            MatchingFieldsDataTable.Columns.Add("Field Type", typeof(string));
            MatchingFieldsDataTable.Columns.Add("Application", typeof(string));

            string SqlString = "SELECT * "
                + " FROM [ATMS].[dbo].[UniversalTableFieldsDefinition] "
                + " WHERE IsMatchingField = 1 AND TableStructureId = @TableStructureId "
                + " Order By SortSequence ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@TableStructureId", InTableStructureId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            DataRow RowSelected = MatchingFieldsDataTable.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Field Name"] = FieldName;
                            RowSelected["DB Field Name"] = FieldDBName;
                            RowSelected["Field Type"] = FieldType;
                            RowSelected["Application"] = Application;

                            // ADD ROW
                            MatchingFieldsDataTable.Rows.Add(RowSelected);

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
        // READ Source File by name to find technical 
        //
        public void ReadUniversalTableFieldsDefinitionToGetTechnical(string InFieldName, string InTableStructureId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[UniversalTableFieldsDefinition]"
               + " WHERE FieldName = @FieldName AND TableStructureId = @TableStructureId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@FieldName", InFieldName);
                        cmd.Parameters.AddWithValue("@TableStructureId", InTableStructureId);

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
           // return FieldDBName; 
        }


        //
        // READ MatchingFields by SequenceNumber 
        //
        public void ReadUniversalTableFieldsDefinitionSeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[UniversalTableFieldsDefinition]"
               + " WHERE SeqNo = @SeqNo";

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
        // Insert UniversalTableFieldsDefinition Record 
        //
        public int InsertUniversalTableFieldsDefinitionRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[UniversalTableFieldsDefinition]"
                    +  " ([SortSequence]"
                    + ",[TableStructureId] "
                    + ",[FieldName] " 
                    + ",[FieldDBName]"
                    + ",[FieldType] " 
                    + ",[FieldLength]"
                    + ",[IsMatchingField]"
                    + ",[IsPrimaryMatchingField]"
                    + ",[IsMergedField]"
                    + ",[Application]"
                    + ")"
                    + " VALUES "
                    + " (@SortSequence"
                    + ",@TableStructureId "
                    + ",@FieldName "
                    + ",@FieldDBName "
                    + ",@FieldType "
                    + ",@FieldLength "
                    + ",@IsMatchingField "
                    + ",@IsPrimaryMatchingField "
                    + ",@IsMergedField "
                    + ",@Application "
                    + ")  "
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                      
                        cmd.Parameters.AddWithValue("@SortSequence", SortSequence);

                        cmd.Parameters.AddWithValue("@TableStructureId", TableStructureId);

                        cmd.Parameters.AddWithValue("@FieldName", FieldName);                       
                        cmd.Parameters.AddWithValue("@FieldDBName", FieldDBName);

                        cmd.Parameters.AddWithValue("@FieldType", FieldType);
                        cmd.Parameters.AddWithValue("@FieldLength", FieldLength);
                        cmd.Parameters.AddWithValue("@IsMatchingField", IsMatchingField);

                        cmd.Parameters.AddWithValue("@IsPrimaryMatchingField", IsPrimaryMatchingField);
                        cmd.Parameters.AddWithValue("@IsMergedField", IsMergedField);
                        
                        cmd.Parameters.AddWithValue("@Application", Application);

                       

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
        // UPDATE UniversalTableFieldsDefinition
        // 
        public void UpdateUniversalTableFieldsDefinitionRecord(int InSeqNo)
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[UniversalTableFieldsDefinition] SET "
                              + " SortSequence = @SortSequence "
                              + ",TableStructureId = @TableStructureId "
                              + ",FieldName = @FieldName "
                              + ",FieldDBName = @FieldDBName "
                              + ",FieldType = @FieldType "
                              + ",FieldLength = @FieldLength "
                              + ",IsMatchingField = @IsMatchingField "
                              + ",IsPrimaryMatchingField = @IsPrimaryMatchingField "
                              + ",IsMergedField = @IsMergedField "
                              + ",Application = @Application "
                              + ""
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@SortSequence", SortSequence);

                        cmd.Parameters.AddWithValue("@TableStructureId", TableStructureId);

                        cmd.Parameters.AddWithValue("@FieldName", FieldName);
                        cmd.Parameters.AddWithValue("@FieldDBName", FieldDBName);

                        cmd.Parameters.AddWithValue("@FieldType", FieldType);
                        cmd.Parameters.AddWithValue("@FieldLength", FieldLength);

                        cmd.Parameters.AddWithValue("@IsMatchingField", IsMatchingField);

                        cmd.Parameters.AddWithValue("@IsPrimaryMatchingField", IsPrimaryMatchingField);

                        cmd.Parameters.AddWithValue("@IsMergedField", IsMergedField);

                        cmd.Parameters.AddWithValue("@Application", Application);

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
        // DELETE 
        //
        public void DeleteUniversalTableFieldsDefinitionRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[UniversalTableFieldsDefinition] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                   
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



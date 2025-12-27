using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMOutputFileDefinition : Logger
    {
        public RRDMOutputFileDefinition() : base() { }

        public int SeqNo;
  
        public string OutputFile_Id;
        public string OutputFile_Version;

        public string LayoutType; // Flat File , CSV , HTML
     
        public int LinesHeader;
        public int LinesTrailer;

        public string FileNameMask;
        public string TargetDirectory;
        public string ArchiveDirectory;
        public string ExceptionsDirectory;
      
        public string SourceTableName; // eg ATMs Pool Table
        public DateTime EffectiveDate; // When to be activated

        public string Operator;

        // Define the data table 
        public DataTable OutputFilesDataTable = new DataTable();

        public DataTable OutputFieldsDataTable = new DataTable();

        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string SqlString;

        string connectionStringATMs = AppConfig.GetConnectionString("ATMSConnectionString");

     
        //
        // Read Fields 
        //
        private void ReadTblFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            OutputFile_Id = (string)rdr["OutputFile_Id"];
            OutputFile_Version = (string)rdr["OutputFile_Version"];

            LayoutType = (string)rdr["LayoutType"];
         
            LinesHeader = (int)rdr["LinesHeader"];
            LinesTrailer = (int)rdr["LinesTrailer"];

            FileNameMask = (string)rdr["FileNameMask"];

            TargetDirectory = (string)rdr["TargetDirectory"];
            ArchiveDirectory = (string)rdr["ArchiveDirectory"];
            ExceptionsDirectory = (string)rdr["ExceptionsDirectory"];
            
            SourceTableName = (string)rdr["SourceTableName"];
            EffectiveDate = (DateTime)rdr["EffectiveDate"];
         
            Operator = (string)rdr["Operator"];
        }


        //
        // READ Output File 
        //
        public void ReadReconcOutputFilesToFillDataTable(string InSelectionCriteria)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            OutputFilesDataTable = new DataTable();
            OutputFilesDataTable.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 


            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[OutputFileDefinition]"
               + InSelectionCriteria
               + " Order By  OutputFile_Id, OutputFile_Version";

            using (SqlConnection conn =
             new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(OutputFilesDataTable);

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
        // READ Output File 
        //
        public void ReadReconcOutputFilesToFillDataTable2(string InSelectionCriteria)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            OutputFieldsDataTable = new DataTable();
            OutputFieldsDataTable.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 


            //SqlString = "SELECT *"
            //   + " FROM [ATMS].[dbo].[OutputFileDefinition]"
            //   + InSelectionCriteria
            //   + " Order By  OutputFile_Id, OutputFile_Version";

            SqlString = "SELECT name as FieldNm"
     + " FROM sys.columns "
     + " WHERE[object_id] = OBJECT_ID('[ATMS].[dbo].[OutputFileDefinition]')"
     + " ";

            using (SqlConnection conn =
             new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(OutputFieldsDataTable);

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
        // READ Output File by name to find technical 
        //

        public void ReadReconcOutputFilesByFileIdAndVersion(string InOutputFile_Id, string InOutputFile_Version)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[OutputFileDefinition]"
               + " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version";

            using (SqlConnection conn =
                          new SqlConnection(connectionStringATMs))
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

                            ReadTblFields(rdr);

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
        // READ ReconcOutputFilesSeqNo
        //
        public void ReadOutputFilesBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[OutputFileDefinition]"
               + " WHERE SeqNo = @SeqNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionStringATMs))
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

                            ReadTblFields(rdr);
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


        // Insert File Field Record 
        //
        public void InsertReconcOutputFileRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";
            
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[OutputFileDefinition]"
                    + "([OutputFile_Id], [OutputFile_Version],[LayoutType], [LinesHeader], [LinesTrailer], [FileNameMask],  "
                    + " [ArchiveDirectory], [ExceptionsDirectory],  "
                    + " [TargetDirectory], [SourceTableName], [EffectiveDate],"
                    + "  "
                    + "  "
                    + " [Operator] )"
                    + " VALUES (@OutputFile_Id, @OutputFile_Version, @LayoutType, @LinesHeader, @LinesTrailer, @FileNameMask,"
                    + " @ArchiveDirectory, @ExceptionsDirectory, "
                    + "@TargetDirectory, @SourceTableName, @EffectiveDate,"
                    + "   "
                    + "   "
                    + " @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@OutputFile_Id", OutputFile_Id);
                        cmd.Parameters.AddWithValue("@OutputFile_Version", OutputFile_Version);
                        cmd.Parameters.AddWithValue("@LayoutType", LayoutType);

                        cmd.Parameters.AddWithValue("@LinesHeader", LinesHeader);
                        cmd.Parameters.AddWithValue("@LinesTrailer", LinesTrailer);

                        cmd.Parameters.AddWithValue("@FileNameMask", FileNameMask);
                        cmd.Parameters.AddWithValue("@ArchiveDirectory", ArchiveDirectory);

                        cmd.Parameters.AddWithValue("@ExceptionsDirectory", ExceptionsDirectory);

                        cmd.Parameters.AddWithValue("@TargetDirectory", TargetDirectory);
                        cmd.Parameters.AddWithValue("@SourceTableName", SourceTableName);
                        cmd.Parameters.AddWithValue("@EffectiveDate", EffectiveDate);


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

        // UPDATE File 
        // 
        public void UpdateReconcOutputFileRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[OutputFileDefinition] SET "
                              + " OutputFile_Id = @OutputFile_Id, OutputFile_Version = @OutputFile_Version, "
                              + " LayoutType = @LayoutType, LinesHeader = @LinesHeader, "
                              + " LinesTrailer = @LinesTrailer, FileNameMask = @FileNameMask, "
                              + " ArchiveDirectory = @ArchiveDirectory, ExceptionsDirectory = @ExceptionsDirectory, "
                              + " TargetDirectory = @TargetDirectory, SourceTableName = @SourceTableName, "
                              + " EffectiveDate = @EffectiveDate, "
                              + "  Operator = @Operator  "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@OutputFile_Id", OutputFile_Id);
                        cmd.Parameters.AddWithValue("@OutputFile_Version", OutputFile_Version);
                        cmd.Parameters.AddWithValue("@LayoutType", LayoutType);
                        cmd.Parameters.AddWithValue("@LinesHeader", LinesHeader);
                        cmd.Parameters.AddWithValue("@LinesTrailer", LinesTrailer);
                        cmd.Parameters.AddWithValue("@FileNameMask", FileNameMask);

                        cmd.Parameters.AddWithValue("@ArchiveDirectory", ArchiveDirectory);

                        cmd.Parameters.AddWithValue("@ExceptionsDirectory", ExceptionsDirectory);

                        cmd.Parameters.AddWithValue("@TargetDirectory", TargetDirectory);
                        cmd.Parameters.AddWithValue("@SourceTableName", SourceTableName);
                        cmd.Parameters.AddWithValue("@EffectiveDate", EffectiveDate);

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
        // DELETE file 
        //
        public void DeleteFileFieldRecord(int InSeqNo, string InLayoutType)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[OutputFileDefinition] "
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

            using (SqlConnection conn =
                new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[OutputFileFieldsMappingDefinition] "
                            + " WHERE LayoutType =  @LayoutType ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LayoutType", InLayoutType);

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



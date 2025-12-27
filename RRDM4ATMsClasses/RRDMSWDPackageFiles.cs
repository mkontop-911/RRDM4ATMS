using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
//using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMSWDPackageFiles : Logger
    {
        public RRDMSWDPackageFiles() : base() { }

        public int SeqNo;

        public string SWDCategoryId;

        public string PackId;

        public string Directory;
        public string FileIdOrigin;
        public string FileIdDestination;

        public string AtmDirId; 

        public string Operator;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable TableSWDPackageFiles = new DataTable();

        public int TotalSelected;
        public int TotalMatchingDone;
        public int TotalMatchingNotDone;
        public int TotalReconc;
        public int TotalNotReconc;
        public int TotalUnMatchedRecords;

        string SqlString; // Do not delete 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // SWD PackageFiles Reader Fields 
        private void SWDPackageFilesReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            SWDCategoryId = (string)rdr["SWDCategoryId"];

            PackId = (string)rdr["PackId"];

            Directory = (string)rdr["Directory"];

            FileIdOrigin = (string)rdr["FileIdOrigin"];

            FileIdDestination = (string)rdr["FileIdDestination"];

            AtmDirId = (string)rdr["AtmDirId"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ SWDPackageFiles
        // FILL UP A TABLE
        //
        public void ReadSWDPackageFilesFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableSWDPackageFiles = new DataTable();
            TableSWDPackageFiles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableSWDPackageFiles.Columns.Add("SeqNo", typeof(int));
            TableSWDPackageFiles.Columns.Add("SWDCategory", typeof(string));
            TableSWDPackageFiles.Columns.Add("PackId", typeof(string));
            TableSWDPackageFiles.Columns.Add("Directory", typeof(string));
            TableSWDPackageFiles.Columns.Add("FileIdOrigin", typeof(string));
            TableSWDPackageFiles.Columns.Add("FileIdDestination", typeof(string));
            TableSWDPackageFiles.Columns.Add("AtmDirId", typeof(string));
            //AtmDirId
            SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[SWDPackageFiles] "
               + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            SWDPackageFilesReaderFields(rdr);

                            DataRow RowSelected = TableSWDPackageFiles.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["SWDCategory"] = SWDCategoryId;
                            RowSelected["PackId"] = PackId;
                            RowSelected["Directory"] = Directory;

                            RowSelected["FileIdOrigin"] = FileIdOrigin;
                            RowSelected["FileIdDestination"] = FileIdDestination;
                            
                            RowSelected["AtmDirId"] = AtmDirId;

                            // ADD ROW
                            TableSWDPackageFiles.Rows.Add(RowSelected);

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
        // READ PackageFiles  by Seq no  
        // 
        //
        public void ReadPackageFilesbySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDPackageFiles] "
                    + " WHERE Operator = @Operator AND SeqNo = @SeqNo ";

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
                            SWDPackageFilesReaderFields(rdr);

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
        // READ SWDFromFileIdectories by Cat Id   
        // 
        //
        public void ReadPackageFilesbyCategId(string InOperator, string InSWDCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDPackageFiles] "
                    + " WHERE Operator = @Operator AND SWDCategoryId = @SWDCategoryId ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SWDCategoryId", InSWDCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                            // Read Fields 
                            SWDPackageFilesReaderFields(rdr);

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
        // READ PackageFiles by FileIdOrigin
        // 
        //
        public void ReadPackageFilesbyFileIdOrigin(string InOperator, string InPackId, string InFileIdOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDPackageFiles] "
                    + " WHERE Operator = @Operator AND PackId = @PackId AND FileIdOrigin = @FileIdOrigin ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@PackId", InPackId);
                        cmd.Parameters.AddWithValue("@FileIdOrigin", InFileIdOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields 
                            SWDPackageFilesReaderFields(rdr);

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
        // Insert PackageFile
        //
        public int InsertPackageFile()
        {
            //
            ErrorFound = false;
            ErrorOutput = "";
            //
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[SWDPackageFiles]"
                    + "([SWDCategoryId],"
                    + " [PackId],  "
                    + " [Directory], "
                    + " [FileIdOrigin], "
                    + " [FileIdDestination], "
                    + " [AtmDirId], "
                    + " [Operator] )"
                    + " VALUES (@SWDCategoryId,"
                    + " @PackId,"
                    + " @Directory, "
                    + " @FileIdOrigin, "
                    + " @FileIdDestination, "
                    + " @AtmDirId, "
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

                        cmd.Parameters.AddWithValue("@SWDCategoryId", SWDCategoryId);
                        cmd.Parameters.AddWithValue("@PackId", PackId);

                        cmd.Parameters.AddWithValue("@Directory", Directory);

                        cmd.Parameters.AddWithValue("@FileIdOrigin", FileIdOrigin);

                        cmd.Parameters.AddWithValue("@FileIdDestination", FileIdDestination);

                        cmd.Parameters.AddWithValue("@AtmDirId", AtmDirId);

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
        // UPDATE FromFileIdectories
        // 
        public void UpdatePackageFile(string InOperator, int InSWDCategoryId)
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[SWDPackageFiles] SET "
                            + " SWDCategoryId = @SWDCategoryId," 
                            + " PackId = @PackId, "
                            + " Directory = @Directory, "
                            + " FileIdOrigin = @FileIdOrigin,  "
                            + " FileIdDestination = @FileIdDestination  "
                             + " AtmDirId = @AtmDirId  "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                        cmd.Parameters.AddWithValue("@SWDCategoryId", SWDCategoryId);
                        cmd.Parameters.AddWithValue("@PackId", PackId);

                        cmd.Parameters.AddWithValue("@Directory", Directory);

                        cmd.Parameters.AddWithValue("@FileIdOrigin", FileIdOrigin);

                        cmd.Parameters.AddWithValue("@FileIdDestination", FileIdDestination);

                        cmd.Parameters.AddWithValue("@AtmDirId", AtmDirId);

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
        // DELETE FromFileIdectories
        //
        public void DeletePackageFile(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SWDPackageFiles] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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
  
        

    }
}



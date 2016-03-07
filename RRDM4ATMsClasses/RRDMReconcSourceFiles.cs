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

namespace RRDM4ATMs
{
    public class RRDMReconcSourceFiles
    {
        public int SeqNo;

        public string SystemOfOrigin;
        public bool Enabled;
        public string SourceFileId;
        public string LayoutId;

        public string FileNameMask;
        public string ArchiveDirectory;
        public string Type;

        public string InitialTableName;
        public string WorkingTableName;
   
        public string SourceDirectory;

        public string Operator;

        // Define the data table 
        public DataTable DataTableSourceFiles = new DataTable();
        public int TotalSelected;
      
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

   //
        // READ Source File 
   //
        public void ReadReconcSourceFilesToFillDataTable(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableSourceFiles = new DataTable();
            DataTableSourceFiles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableSourceFiles.Columns.Add("SeqNo", typeof(int));
            DataTableSourceFiles.Columns.Add("SourceFileId", typeof(string));
            DataTableSourceFiles.Columns.Add("Type", typeof(string));
            DataTableSourceFiles.Columns.Add("SourceDirectory", typeof(string));

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcSourceFiles]"
               + " WHERE Operator = @Operator";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            SystemOfOrigin = (string)rdr["SystemOfOrigin"];
                            Enabled = (bool)rdr["Enabled"]; 
                            SourceFileId = (string)rdr["SourceFileId"];
                            LayoutId = (string)rdr["LayoutId"];
                            FileNameMask = (string)rdr["FileNameMask"];

                            ArchiveDirectory = (string)rdr["ArchiveDirectory"];
                            Type = (string)rdr["Type"];
                            InitialTableName = (string)rdr["InitialTableName"];
                            WorkingTableName = (string)rdr["WorkingTableName"];
                            SourceDirectory = (string)rdr["SourceDirectory"];

                            Operator = (string)rdr["Operator"];


                            DataRow RowSelected = DataTableSourceFiles.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["SourceFileId"] = SourceFileId;
                            RowSelected["Type"] = Type;
                            RowSelected["SourceDirectory"] = SourceDirectory;

                            // ADD ROW
                            DataTableSourceFiles.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReconcSourceFiles Class............. " + ex.Message;
                }
        }

        //
        // READ Source File by name to find technical 
        //
        public void ReadReconcSourceFilesByFileId(string InSourceFileId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcSourceFiles]"
               + " WHERE SourceFileId = @SourceFileId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            SystemOfOrigin = (string)rdr["SystemOfOrigin"];
                            Enabled = (bool)rdr["Enabled"];
                            SourceFileId = (string)rdr["SourceFileId"];
                            LayoutId = (string)rdr["LayoutId"];
                            FileNameMask = (string)rdr["FileNameMask"];

                            ArchiveDirectory = (string)rdr["ArchiveDirectory"];
                            Type = (string)rdr["Type"];
                            InitialTableName = (string)rdr["InitialTableName"];
                            WorkingTableName = (string)rdr["WorkingTableName"];
                            SourceDirectory = (string)rdr["SourceDirectory"];

                            Operator = (string)rdr["Operator"];

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
                    ErrorOutput = "An error occured in ReconcSourceFiles Class............. " + ex.Message;
                }
        }
        //
        // READ ReconcSourceFilesSeqNo
        //
        public void ReadReconcSourceFilesSeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcSourceFiles]"
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

                            SeqNo = (int)rdr["SeqNo"];

                            SystemOfOrigin = (string)rdr["SystemOfOrigin"];
                            Enabled = (bool)rdr["Enabled"];
                            SourceFileId = (string)rdr["SourceFileId"];
                            LayoutId = (string)rdr["LayoutId"];
                            FileNameMask = (string)rdr["FileNameMask"];

                            ArchiveDirectory = (string)rdr["ArchiveDirectory"];
                            Type = (string)rdr["Type"];
                            InitialTableName = (string)rdr["InitialTableName"];
                            WorkingTableName = (string)rdr["WorkingTableName"];
                            SourceDirectory = (string)rdr["SourceDirectory"];

                            Operator = (string)rdr["Operator"];
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
                    ErrorOutput = "An error occured in ReconcSourceFilesSeqNo Class............. " + ex.Message;
                }
        }


        // Insert File Field Record 
        //
        public void InsertReconcSourceFileRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcSourceFiles]"
                    + "([SystemOfOrigin], [Enabled],[SourceFileId], [LayoutId], [FileNameMask],  "
                    + " [ArchiveDirectory], [Type], [InitialTableName], [WorkingTableName],"
                    + " [SourceDirectory],  "
                    + " [Operator] )"
                    + " VALUES (@SystemOfOrigin, @Enabled, @SourceFileId, @LayoutId, @FileNameMask,"
                    + " @ArchiveDirectory, @Type, @InitialTableName, @WorkingTableName,"
                    + " @SourceDirectory,  "
                    + " @Operator )"; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@Enabled", Enabled);
                        cmd.Parameters.AddWithValue("@SourceFileId", SourceFileId);
                        cmd.Parameters.AddWithValue("@LayoutId", LayoutId);
                        cmd.Parameters.AddWithValue("@FileNameMask", FileNameMask);

                        cmd.Parameters.AddWithValue("@ArchiveDirectory", ArchiveDirectory);
                        cmd.Parameters.AddWithValue("@Type", Type);
                        cmd.Parameters.AddWithValue("@InitialTableName", InitialTableName);
                        cmd.Parameters.AddWithValue("@WorkingTableName", WorkingTableName); 
                        cmd.Parameters.AddWithValue("@SourceDirectory", SourceDirectory);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

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
                    ErrorOutput = "An error occured in InsertReconcSourceFileRecord Class............. " + ex.Message;
                }
        }

        // UPDATE File 
        // 
        public void UpdateReconcSourceFileRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcSourceFiles] SET "
                              + " SystemOfOrigin = @SystemOfOrigin, Enabled = @Enabled, "
                              + " SourceFileId = @SourceFileId, LayoutId = @LayoutId, FileNameMask = @FileNameMask, "
                              + " ArchiveDirectory = @ArchiveDirectory, Type = @Type,InitialTableName = @InitialTableName, "
                              + " WorkingTableName = @WorkingTableName, "
                              + " SourceDirectory = @SourceDirectory, Operator = @Operator  "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@Enabled", Enabled);
                        cmd.Parameters.AddWithValue("@SourceFileId", SourceFileId);
                        cmd.Parameters.AddWithValue("@LayoutId", LayoutId);
                        cmd.Parameters.AddWithValue("@FileNameMask", FileNameMask);

                        cmd.Parameters.AddWithValue("@ArchiveDirectory", ArchiveDirectory);
                        cmd.Parameters.AddWithValue("@Type", Type);
                        cmd.Parameters.AddWithValue("@InitialTableName", InitialTableName);
                        cmd.Parameters.AddWithValue("@WorkingTableName", WorkingTableName);
                        cmd.Parameters.AddWithValue("@SourceDirectory", SourceDirectory);

                        cmd.Parameters.AddWithValue("@Operator", Operator);


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
                    ErrorOutput = "An error occured in UpdateFileFieldRecord Class............. " + ex.Message;
                }
        }

        //
        // DELETE file 
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
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcSourceFiles] "
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in DeleteFileFieldRecord Class............. " + ex.Message;
                }

        }
    }
}

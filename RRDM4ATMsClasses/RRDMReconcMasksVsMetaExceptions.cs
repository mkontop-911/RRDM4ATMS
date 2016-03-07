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
    public class RRDMReconcMasksVsMetaExceptions
    {
  //      SELECT TOP 1000 [SeqNo]
  //    ,[CategoryId]
  //    ,[MaskId]
  //    ,[MaskName]
  //    ,[MetaExceptionNo]
  //    ,[Operator]
  //FROM [ATMS].[dbo].[ReconcMasksVsMetaExceptions]
        public int SeqNo;

        public string CategoryId;
        public string MaskId;
        public string MaskName;
        public int MetaExceptionNo;
        public string Operator;

        // Define the data table 
        public DataTable DataTableMasks = new DataTable();
        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ Source File 
        //
        public void ReadReconcMasksToFillDataTable(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableMasks = new DataTable();
            DataTableMasks.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableMasks.Columns.Add("SeqNo", typeof(int));
            DataTableMasks.Columns.Add("MaskId", typeof(string));
            DataTableMasks.Columns.Add("MaskName", typeof(string));
            DataTableMasks.Columns.Add("MetaId", typeof(int));

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcMasksVsMetaExceptions]"
               + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
               + "  ORDER BY MetaExceptionNo ";
           
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

                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];
                           
                            MaskId = (string)rdr["MaskId"];
                            MaskName = (string)rdr["MaskName"];
                         
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];
                          
                            Operator = (string)rdr["Operator"];


                            DataRow RowSelected = DataTableMasks.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["MaskId"] = MaskId;
                            RowSelected["MaskName"] = MaskName;
                            RowSelected["MetaId"] = MetaExceptionNo;

                            // ADD ROW
                            DataTableMasks.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured ReconcMasksToFillDataTable............. " + ex.Message;
                }
        }

        //
        // READ Mask to find Metaexception 
        //
        public void ReadReconcMaskRecord(string InOperator, string InCategoryId, string InMaskId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcMasksVsMetaExceptions]"
               + " WHERE Operator = @Operator AND CategoryId = @CategoryId AND MaskId = @MaskId";

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
                        cmd.Parameters.AddWithValue("@MaskId", InMaskId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];
                         
                            MaskId = (string)rdr["MaskId"];
                            MaskName = (string)rdr["MaskName"];
                          
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];
                           
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
                    ErrorOutput = "An error occured in ReadReconcMaskForMetaException............. " + ex.Message;
                }
        }

        //
        // READ by meta exception to see if already exist 
        //
        public void ReadReconcMaskForMetaException(string InOperator, string InCategoryId, int InMeta)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcMasksVsMetaExceptions]"
               + " WHERE Operator = @Operator AND CategoryId = @CategoryId AND MetaExceptionNo = @MetaExceptionNo";

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
                        cmd.Parameters.AddWithValue("@MetaExceptionNo", InMeta);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];

                            MaskId = (string)rdr["MaskId"];
                            MaskName = (string)rdr["MaskName"];

                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

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
                    ErrorOutput = "An error occured in ReadReconcMaskForMetaException............. " + ex.Message;
                }
        }
        //
        // READ ReconcMaskSeqNo
        //
        public void ReadReconcMaskBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcMasksVsMetaExceptions]"
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

                            CategoryId = (string)rdr["CategoryId"];

                            MaskId = (string)rdr["MaskId"];
                            MaskName = (string)rdr["MaskName"];

                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

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
                    ErrorOutput = "An error occured in ReadReconcMaskBySeqNo............. " + ex.Message;
                }
        }


        // Insert Category Mask Record 
        //
        public void InsertReconcCategoryMaskRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcMasksVsMetaExceptions]"
                    + "([CategoryId], [MaskId], [MaskName], "
                    + " [MetaExceptionNo], "
                    + " [Operator] )"
                    + " VALUES (@CategoryId, @MaskId, @MaskName,"
                    + " @MetaExceptionNo, "
                    + " @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                      
                        cmd.Parameters.AddWithValue("@MaskId", MaskId);
                        cmd.Parameters.AddWithValue("@MaskName", MaskName);

                        cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

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
                    ErrorOutput = "An error occured in InsertReconcCategoryMaskRecord()............. " + ex.Message;
                }
        }

        // UPDATE Masks Record
        // 
        public void UpdateReconcMaskRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcMasksVsMetaExceptions] SET "
                              + " CategoryId = @CategoryId, "
                              + " MaskId = @MaskId, MaskName = @MaskName, "
                              + " Operator = @Operator  "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                        cmd.Parameters.AddWithValue("@MaskId", MaskId);
                        cmd.Parameters.AddWithValue("@MaskName", MaskName);

                        cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

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
                    ErrorOutput = "An error occured in UpdateReconcMaskRecord(int InSeqNo)............. " + ex.Message;
                }
        }

        //
        // DELETE Category
        //
        public void DeleteMaskRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcMasksVsMetaExceptions] "
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
                    ErrorOutput = "An error occured in DeleteMaskRecord(int InSeqNo)........ " + ex.Message;
                }

        }
    }
}

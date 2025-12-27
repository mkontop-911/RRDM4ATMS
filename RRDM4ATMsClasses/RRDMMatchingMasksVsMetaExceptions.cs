using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMatchingMasksVsMetaExceptions : Logger
    {
        public RRDMMatchingMasksVsMetaExceptions() : base() { }
        //    
        public int SeqNo;

        public string CategoryId;
        public string MaskId;
        public string MaskName;
        public int TransType; 
        public int MetaExceptionId; // Equals to ERROR ID
        public string Operator;

        // Define the data table 
        public DataTable DataTableMasks = new DataTable();
        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        //
        // Read Fields
        //
        private void ReadFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CategoryId = (string)rdr["CategoryId"];

            MaskId = (string)rdr["MaskId"];
            MaskName = (string)rdr["MaskName"];

            TransType = (int)rdr["TransType"];

            MetaExceptionId = (int)rdr["MetaExceptionId"];

            Operator = (string)rdr["Operator"];
        }

        // READ Source File 
        //
        public void ReadMatchingMasksToFillDataTable(string InOperator, string InCategoryId)
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
            DataTableMasks.Columns.Add("Type", typeof(string));

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingMasksVsMetaExceptions]"
               + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
               + "  ORDER BY MetaExceptionId ";
           
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

                            // Read Fields
                            ReadFields(rdr);

                            //Fill IN Table 
                            DataRow RowSelected = DataTableMasks.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["MaskId"] = MaskId;
                            RowSelected["MaskName"] = MaskName;
                            RowSelected["MetaId"] = MetaExceptionId;

                            if (TransType == 11)
                            {
                                RowSelected["Type"] = "For Debit";
                            }
                            if (TransType == 21)
                            {
                                RowSelected["Type"] = "For Credit";
                            }

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

                    CatchDetails(ex);
                }
        }


        //
        // READ Mask to find Metaexception 
        //
        public void ReadMatchingMaskRecordbyMaskId(string InOperator, string InCategoryId, string InMaskId, int InTransType)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingMasksVsMetaExceptions]"
               + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
               + " AND MaskId = @MaskId AND TransType = @TransType ";

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
                        cmd.Parameters.AddWithValue("@TransType", InTransType);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;


                            // Read Fields
                            ReadFields(rdr);


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
        // READ by meta exception to see if already exist 
        //
        public void ReadMatchingMaskForMetaException(string InOperator, string InCategoryId, int InMeta)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingMasksVsMetaExceptions]"
               + " WHERE Operator = @Operator AND CategoryId = @CategoryId AND MetaExceptionId = @MetaExceptionId";

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
                        cmd.Parameters.AddWithValue("@MetaExceptionId", InMeta);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;


                            // Read Fields
                            ReadFields(rdr);


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
        // READ ReconcMaskSeqNo
        //
        public void ReadMatchingMaskBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingMasksVsMetaExceptions]"
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


                            // Read Fields
                            ReadFields(rdr);

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
        // Insert Category Mask Record 
        //
        public void InsertMatchingCategoryMaskRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[MatchingMasksVsMetaExceptions]"
                    + "([CategoryId], [MaskId], [MaskName], "
                    + " [TransType], "
                     + " [MetaExceptionId], "
                    + " [Operator] )"
                    + " VALUES (@CategoryId, @MaskId, @MaskName,"
                    + " @TransType, "
                   + " @MetaExceptionId, "
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

                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);

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

                    CatchDetails(ex);
                }
        }

        // UPDATE Masks Record
        // 
        public void UpdateMatchingMaskRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingMasksVsMetaExceptions] SET "
                              + " CategoryId = @CategoryId, TransType = @TransType,"
                              + " MaskId = @MaskId, MaskName = @MaskName, "
                              + " Operator = @Operator  "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                        cmd.Parameters.AddWithValue("@TransType", TransType);

                        cmd.Parameters.AddWithValue("@MaskId", MaskId);
                        cmd.Parameters.AddWithValue("@MaskName", MaskName);

                        cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);

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

                    CatchDetails(ex);
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
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingMasksVsMetaExceptions] "
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



using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace RRDM4ATMs
{
    public class RRDMMatchingBankToRRDMFileFields_EXTRA : Logger
    {
        public RRDMMatchingBankToRRDMFileFields_EXTRA() : base() { }
        //
        // DEFINITION OF UPDATING NEW FIELDS
        //
        public int SeqNo;

        public string TableId;

        public string FieldNm;

        public string FieldValueFormula;

        public string FieldDefaultValue;

        public string MatchingProduct; //  eg Meeza, NCR_FOREX, Fawry

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable DataTableExtraFields = new DataTable();

        public int TotalSelected;
        string SqlString; // Do not delete 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        //
        // Methods 
        // READ fields  
        // FILL UP A TABLE
        //
        // Read Fields In Table 
        private void ReadFieldsInTable(SqlDataReader rdr)
        {
       
            SeqNo = (int)rdr["SeqNo"];

            TableId = (string)rdr["TableId"];
            FieldNm = (string)rdr["FieldNm"];

            FieldValueFormula = (string)rdr["FieldValueFormula"];
            FieldDefaultValue = (string)rdr["FieldDefaultValue"];
            MatchingProduct = (string)rdr["MatchingProduct"];
        }

        //
        // READ  EXTRA table as per request.
        //
        public void ReadExtraFieldsBySelectionCriteria(string InUserId, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
           
            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields_EXTRA] "
                 + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {
                        //  cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
        // READ  EXTRA table and Fill in Table.
        //
        public void ReadExtraFieldsAndFillTable(string InTableId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableExtraFields = new DataTable();
            DataTableExtraFields.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableExtraFields.Columns.Add("SeqNo", typeof(int));
            DataTableExtraFields.Columns.Add("FieldNm", typeof(string));
            DataTableExtraFields.Columns.Add("FieldValueFormula", typeof(string));

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields_EXTRA] "
                 + "WHERE TableId = @TableId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableId", InTableId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            DataRow RowSelected = DataTableExtraFields.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["FieldNm"] = FieldNm;
                            RowSelected["FieldValueFormula"] = FieldValueFormula;

                            // ADD ROW
                            DataTableExtraFields.Rows.Add(RowSelected);

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
        // READ  EXTRA table and Fill in Table DISTINCT
        //
        public void ReadExtraFieldsAndFillTable_Distinct(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            string SqlString = "";
            
            DataTableExtraFields = new DataTable();
            DataTableExtraFields.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableExtraFields.Columns.Add("TableId", typeof(string));
            
            SqlString = "SELECT DISTINCT TableId "
                 + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields_EXTRA] "; 
                            
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

                            TableId = (string)rdr["TableId"];
                            
                            DataRow RowSelected = DataTableExtraFields.NewRow();

                            RowSelected["TableId"] = TableId;
                           
                            // ADD ROW
                            DataTableExtraFields.Rows.Add(RowSelected);

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


        string SQL_UpdateHeader;
        string SQL_UpdateBody;
        public string FullCreatedSqlCommand;

        public void ReadTable_EXTRA_AND_CREATE_COMMAND(string InTableId)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            TotalSelected = 0 ; 

            SQL_UpdateBody = "";
            string  SQL_WHEREClause = " WHERE LoadedAtRMCycle = @LoadedAtRMCycle "; 
            FullCreatedSqlCommand = "";

            string SqlString = "SELECT * "
              + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields_EXTRA] "
                + "WHERE TableId = @TableId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@TableId", InTableId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadFieldsInTable(rdr);

                            SeqNo = (int)rdr["SeqNo"];

                            TableId = (string)rdr["TableId"];
                            FieldNm = (string)rdr["FieldNm"];

                            FieldValueFormula = (string)rdr["FieldValueFormula"];
                           
                           if (TotalSelected == 1)
                            {
                                SQL_UpdateHeader = " UPDATE [RRDM_Reconciliation_ITMX].[dbo]." + TableId + Environment.NewLine
                                                 + " SET " + Environment.NewLine
                                                 ;
                                // No comma in front 
                                SQL_UpdateBody =   FieldNm + "=" + FieldValueFormula + Environment.NewLine;
                            }
                            else
                            {
                                SQL_UpdateBody = SQL_UpdateBody + " , " + FieldNm + "=" + FieldValueFormula + Environment.NewLine;
                            }
                            
                        }

                        // Close Reader
                        rdr.Close();
                    }
                    // Close conn
                    conn.Close();

                    FullCreatedSqlCommand =  SQL_UpdateHeader  + SQL_UpdateBody + SQL_WHEREClause;
                     
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }


        // Insert  Record in table
        //
        public void InsertRecordInTable()
        {
           
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[MatchingBankToRRDMFileFields_EXTRA] "
                    + "([TableId], "
                    + " [FieldNm], [FieldValueFormula], [FieldDefaultValue], "
                    + " [MatchingProduct]"
                    + "  )"
                    + " VALUES (@TableId, "
                    + " @FieldNm, @FieldValueFormula, @FieldDefaultValue, "
                    + " @MatchingProduct "
                    + " )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@TableId", TableId);

                        cmd.Parameters.AddWithValue("@FieldNm", FieldNm);
                        cmd.Parameters.AddWithValue("@FieldValueFormula", FieldValueFormula);

                        cmd.Parameters.AddWithValue("@FieldDefaultValue", FieldDefaultValue);
                        cmd.Parameters.AddWithValue("@MatchingProduct", MatchingProduct);

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

        // UPDATE RECORD IN TABLE 
        // 
        public void UpdateRecordInTable(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingBankToRRDMFileFields_EXTRA]"
                              +" SET "
                              + " TableId = @TableId, "
                              + " FieldNm = @FieldNm, "
                              + " FieldValueFormula = @FieldValueFormula, "
                              + " FieldDefaultValue = @FieldDefaultValue, "
                              + " MatchingProduct = @MatchingProduct "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@TableId", TableId);

                        cmd.Parameters.AddWithValue("@FieldNm", FieldNm);
                        cmd.Parameters.AddWithValue("@FieldValueFormula", FieldValueFormula);

                        cmd.Parameters.AddWithValue("@FieldDefaultValue", FieldDefaultValue);
                        cmd.Parameters.AddWithValue("@MatchingProduct", MatchingProduct);

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
        // DELETE Record In Table 
        //
        public void DeleteRecordInTable(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields_EXTRA] "
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



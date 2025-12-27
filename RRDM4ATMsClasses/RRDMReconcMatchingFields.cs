using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMReconcMatchingFields
    {

        public int SeqNo;

        public string MatchingFieldName;
       
        public string DBName;
        public string FieldType;

        public string Operator;

        // Define the data table 
        public DataTable DataTableMatchingFields = new DataTable();
        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMReconcFileFieldsFromBankToRRDM Rff = new RRDMReconcFileFieldsFromBankToRRDM(); 

        //
        // READ MatchingFields to fill table 
        //
        public void ReadReconcMatchingFieldsToFillDataTable(string InOperator)
        { 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableMatchingFields = new DataTable();
            DataTableMatchingFields.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableMatchingFields.Columns.Add("SeqNo", typeof(int));
            DataTableMatchingFields.Columns.Add("MatchingFieldName", typeof(string));

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcMatchingFields]"
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

                            MatchingFieldName = (string)rdr["MatchingFieldName"];
                       
                            DBName = (string)rdr["DBName"];
                            FieldType = (string)rdr["FieldType"];

                            Operator = (string)rdr["Operator"];


                            DataRow RowSelected = DataTableMatchingFields.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["MatchingFieldName"] = MatchingFieldName;
                       

                            // ADD ROW
                            DataTableMatchingFields.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReconcMatchingFields Class............. " + ex.Message;
                }
        }


        //
        // READ MatchingFields to create Bank to RRDM File/Filds records 
        //
        public void ReadMatchingFieldsToCreateBankToRRDMRecords(string InOperator, string InSourceFileId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcMatchingFields]"
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

                            MatchingFieldName = (string)rdr["MatchingFieldName"];

                            DBName = (string)rdr["DBName"];
                            FieldType = (string)rdr["FieldType"];

                            Operator = (string)rdr["Operator"];

                            Rff.SourceFieldNm = "";
                            Rff.SourceFieldType = "";
                            Rff.SourceFieldPositionStart = 0;
                            Rff.SourceFieldPositionEnd = 0;
                            Rff.SourceFieldValue = "";
                            Rff.TargetFieldValue = "";

                            Rff.RoutineValidation = false;
                            Rff.RoutineNm = ""; 

                            Rff.SourceFileId = InSourceFileId;

                            Rff.TargetFieldNm = MatchingFieldName;

                            Rff.InsertFileFieldRecord(); 

                           
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
                    ErrorOutput = "An error occured in ReconcMatchingFields Class............. " + ex.Message;
                }
        }

        //
        // READ Source File by name to find technical 
        //
        public void ReadReconcMatchingFieldsToGetTechnical(string InDBName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcMatchingFields]"
               + " WHERE DBName = @DBName";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DBName", InDBName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            MatchingFieldName = (string)rdr["MatchingFieldName"];
                          
                            DBName = (string)rdr["DBName"];
                            FieldType = (string)rdr["FieldType"];
                         
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
                    ErrorOutput = "An error occured in ReconcMatchingFields Class............. " + ex.Message;
                }
        }


        //
        // READ MatchingFields by SequenceNumber 
        //
        public void ReadReconcMatchingFieldsSeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcMatchingFields]"
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

                            MatchingFieldName = (string)rdr["MatchingFieldName"];
                            
                            DBName = (string)rdr["DBName"];
                            FieldType = (string)rdr["FieldType"];
                          
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
                    ErrorOutput = "An error occured in ReconcMatchingFieldsSeqNo Class............. " + ex.Message;
                }
        }

        
        

        // Insert MatchingFields Record 
        //
        public void InsertReconcSourceFileRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcMatchingFields]"
                    + "([MatchingFieldName], [DBName], [FieldType], "
                    + " [Operator] )"
                    + " VALUES (@MatchingFieldName, @DBName, @FieldType, "
                    + " @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@MatchingFieldName", MatchingFieldName);
                       
                        cmd.Parameters.AddWithValue("@DBName", DBName);
                        cmd.Parameters.AddWithValue("@FieldType", FieldType);
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

        // UPDATE Category
        // 
        public void UpdateReconcMatchingFieldsRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcMatchingFields] SET "
                              + " MatchingFieldName = @MatchingFieldName, "
                              + " DBName = @DBName, "
                              + " Operator = @Operator  "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@MatchingFieldName", MatchingFieldName);
                        
                        cmd.Parameters.AddWithValue("@DBName", DBName);
                        cmd.Parameters.AddWithValue("@FieldType", FieldType);

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
        // DELETE Category
        //
        public void DeleteMatchingFieldsRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcMatchingFields] "
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



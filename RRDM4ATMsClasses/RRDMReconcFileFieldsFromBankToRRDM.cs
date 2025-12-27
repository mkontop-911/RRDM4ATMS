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
using System.Collections;
using System.IO;

namespace RRDM4ATMs
{
    public class RRDMReconcFileFieldsFromBankToRRDM
    {
        public int SeqNo;

        public string SourceFileId;
        public string SourceFieldNm;

        public string SourceFieldType;

        public string SourceFieldValue;

        public int SourceFieldPositionStart;
        public int SourceFieldPositionEnd;


        public string TargetFieldNm;
        public string TargetFieldValue;

        public bool RoutineValidation;
        public string RoutineNm;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();

        // Define the data table 
        public DataTable DataTableFileFields = new DataTable();
        public int TotalSelected;
        string SqlString; // Do not delete 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // Methods 
        // READ fields  
        // FILL UP A TABLE
        //
        public void ReadFileFields(string InSourceFileId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableFileFields = new DataTable();
            DataTableFileFields.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableFileFields.Columns.Add("SeqNo", typeof(int));
            //DataTableFileFields.Columns.Add("SourceFileId", typeof(string));
            DataTableFileFields.Columns.Add("SourceFieldNm", typeof(string));
            DataTableFileFields.Columns.Add("TargetFieldNm", typeof(string));
            DataTableFileFields.Columns.Add("PositionStart", typeof(int));
            DataTableFileFields.Columns.Add("PositionEnd", typeof(int));
         
            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[ReconcBankToRRDMFileFields] "
                   + " WHERE SourceFileId = @SourceFileId" 
                   + " ORDER BY SeqNo ASC ";
         

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

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];

                            SourceFieldType = (string)rdr["SourceFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];

                            RoutineValidation = (bool)rdr["RoutineValidation"];
                            RoutineNm = (string)rdr["RoutineNm"];

                            DataRow RowSelected = DataTableFileFields.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            //RowSelected["SourceFileId"] = SourceFileId;
                            if (SourceFieldNm == "")
                            {
                                RowSelected["SourceFieldNm"] = "Empty";
                            }
                            else
                            {
                                RowSelected["SourceFieldNm"] = SourceFieldNm;
                            }  
                            RowSelected["TargetFieldNm"] = TargetFieldNm;
                            RowSelected["PositionStart"] = SourceFieldPositionStart;
                            RowSelected["PositionEnd"] = SourceFieldPositionEnd;

                            // ADD ROW
                            DataTableFileFields.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReadFileFields......... " + ex.Message;
                }
        }

        //
        // Methods 
        // READ record by target field 
        
        //
        public void ReadFileFieldsByTargetField(string InSourceFileId, string InSourceFieldNm, int InSourceFieldPositionStart)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[ReconcBankToRRDMFileFields] "
                   + " WHERE SourceFileId = @SourceFileId AND SourceFieldNm = @SourceFieldNm AND SourceFieldPositionStart = @SourceFieldPositionStart  "
                   + " ORDER BY SeqNo ASC ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);
                        cmd.Parameters.AddWithValue("@SourceFieldNm", InSourceFieldNm);
                        cmd.Parameters.AddWithValue("@SourceFieldPositionStart", InSourceFieldPositionStart);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];

                            SourceFieldType = (string)rdr["SourceFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];

                            RoutineValidation = (bool)rdr["RoutineValidation"];
                            RoutineNm = (string)rdr["RoutineNm"];

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
                    ErrorOutput = "An error occured in ReadFileFields......... " + ex.Message;
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
                  + " FROM [ATMS].[dbo].[ReconcBankToRRDMFileFields] "
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

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];
                            SourceFieldType = (string)rdr["SourceFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];

                            RoutineValidation = (bool)rdr["RoutineValidation"];
                            RoutineNm = (string)rdr["RoutineNm"]; 

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }   

        // Insert File Field Record 
        //
        public void InsertFileFieldRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcBankToRRDMFileFields]"
                    + "([SourceFileId], [SourceFieldNm],  "
                    + " [SourceFieldType], [SourceFieldPositionStart], [SourceFieldPositionEnd], "
                    + " [SourceFieldValue],  "
                    + " [TargetFieldNm], [TargetFieldValue],"
                    + " [RoutineValidation], [RoutineNm] )"
                    + " VALUES (@SourceFileId, @SourceFieldNm,"
                    + " @SourceFieldType, @SourceFieldPositionStart, @SourceFieldPositionEnd, "
                    + " @SourceFieldValue,"
                    + " @TargetFieldNm, @TargetFieldValue, " 
                    + " @RoutineValidation,@RoutineNm )";
           
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", SourceFileId);

                        cmd.Parameters.AddWithValue("@SourceFieldNm", SourceFieldNm);
                        cmd.Parameters.AddWithValue("@SourceFieldType", SourceFieldType);
                        cmd.Parameters.AddWithValue("@SourceFieldPositionStart", SourceFieldPositionStart);
                        cmd.Parameters.AddWithValue("@SourceFieldPositionEnd", SourceFieldPositionEnd);

                        cmd.Parameters.AddWithValue("@SourceFieldValue", SourceFieldValue);

                        cmd.Parameters.AddWithValue("@TargetFieldNm", TargetFieldNm);
                        cmd.Parameters.AddWithValue("@TargetFieldValue", TargetFieldValue);

                        cmd.Parameters.AddWithValue("@RoutineValidation", RoutineValidation);
                        cmd.Parameters.AddWithValue("@RoutineNm", RoutineNm);
                        

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
                    ErrorOutput = "An error occured in Reconc Category Class............. " + ex.Message;
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcBankToRRDMFileFields] SET "
                              + " SourceFileId = @SourceFileId, SourceFieldNm = @SourceFieldNm, "
                              + " SourceFieldType = @SourceFieldType, SourceFieldPositionStart = @SourceFieldPositionStart, "
                              + " SourceFieldPositionEnd = @SourceFieldPositionEnd, "
                              + " SourceFieldValue = @SourceFieldValue, "
                              + "TargetFieldNm = @TargetFieldNm, TargetFieldValue = @TargetFieldValue , "
                              + " RoutineValidation = @RoutineValidation, RoutineNm = @RoutineNm  "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@SourceFileId", SourceFileId);

                        cmd.Parameters.AddWithValue("@SourceFieldNm", SourceFieldNm);
                        cmd.Parameters.AddWithValue("@SourceFieldType", SourceFieldType);
                        cmd.Parameters.AddWithValue("@SourceFieldPositionStart", SourceFieldPositionStart);
                        cmd.Parameters.AddWithValue("@SourceFieldPositionEnd", SourceFieldPositionEnd);
                        
                        cmd.Parameters.AddWithValue("@SourceFieldValue", SourceFieldValue);

                        cmd.Parameters.AddWithValue("@TargetFieldNm", TargetFieldNm);
                        cmd.Parameters.AddWithValue("@TargetFieldValue", TargetFieldValue);

                        cmd.Parameters.AddWithValue("@RoutineValidation", RoutineValidation);
                        cmd.Parameters.AddWithValue("@RoutineNm", RoutineNm);

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
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcBankToRRDMFileFields] "
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



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
    public class RRDMReconcCategoriesVsSourcesFiles
    {
        //
        // RECORD FIELDS
        //
        public int SeqNo;

        public string CategoryId;
        public string SourceFileName;
        public bool PrimaryFile;
        public bool WithRemains; // Like VISA AUTHORISATIONS 

        public DateTime LastInFileDtTm;
        public DateTime LastMatchingDtTm; 
        public int ProcessMode; 

        //
        // WORKING FIELDS 
        //

        public string SourceFileNameA;
        public string SourceFileNameB;
        public string SourceFileNameC;
        public string SourceFileNameD;
        public string SourceFileNameE;

        string OutReturnedMessage ;
        int OutReturnedValue ;

        bool KontoAvailable; 

        public int TotalRecords;

        public string WProcessModeInWords;

        int TotalMinusOne = 0;
        int TotalZero = 0;
        int TotalOne = 0;

        // Define the data table 
        public DataTable RMCategoryFiles = new DataTable();
        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // READ Category vs Source Files ALL for this category 
        //
        public void ReadReconcCategoriesVsSourcesAll(string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalRecords = 0 ;

            SourceFileNameA = "";
            SourceFileNameB = "";
            SourceFileNameC = "";
            SourceFileNameD = "";
            SourceFileNameE = "";

            string SqlString = "SELECT *"
                + " FROM [ATMS].[dbo].[ReconcCategoryVsSourceFiles]"
               + " WHERE CategoryId = @CategoryId";
               

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalRecords = TotalRecords + 1 ; 

                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];
                            SourceFileName = (string)rdr["SourceFileName"];

                            if (TotalRecords == 1)
                            {
                                SourceFileNameA = SourceFileName; 
                            }

                            if (TotalRecords == 2)
                            {
                                SourceFileNameB = SourceFileName;
                            }
                            if (TotalRecords == 3)
                            {
                                SourceFileNameC = SourceFileName;
                            }
                            if (TotalRecords == 4)
                            {
                                SourceFileNameD = SourceFileName;
                            }
                            if (TotalRecords == 5)
                            {
                                SourceFileNameE = SourceFileName;
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReconcCategoriesVsSourcesSeqNo Class............. " + ex.Message;
                }
        }
        //
        // READ Category vs Source Files 
        //
        public void ReadReconcCategoryVsSourcesANDFillTable(string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMCategoryFiles = new DataTable();
            RMCategoryFiles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            RMCategoryFiles.Columns.Add("SeqNo", typeof(int));
            RMCategoryFiles.Columns.Add("RMCategId", typeof(string));
            RMCategoryFiles.Columns.Add("FileName", typeof(string));
            RMCategoryFiles.Columns.Add("ProcessMode", typeof(string));
            RMCategoryFiles.Columns.Add("LastInFileDtTm", typeof(DateTime));
            RMCategoryFiles.Columns.Add("LastMatchingDtTm", typeof(DateTime));
            

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcCategoryVsSourceFiles]"
               + " WHERE CategoryId = @CategoryId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                       

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];
                            SourceFileName = (string)rdr["SourceFileName"];
                            PrimaryFile = (bool)rdr["PrimaryFile"];
                            WithRemains = (bool)rdr["WithRemains"];

                            LastInFileDtTm = (DateTime)rdr["LastInFileDtTm"];
                            LastMatchingDtTm = (DateTime)rdr["LastMatchingDtTm"];
                            ProcessMode = (int)rdr["ProcessMode"];

                            if (ProcessMode == -1)
                            {
                                WProcessModeInWords = "Ready For Match"; 
                            }
                            if (ProcessMode == 0)
                            {
                                WProcessModeInWords = "Under Matching Process"; 
                            }
                            if (ProcessMode == 1)
                            {
                                WProcessModeInWords = "Matching Finished"; 
                            }

                            DataRow RowSelected = RMCategoryFiles.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["RMCategId"] = CategoryId;
                            RowSelected["FileName"] = SourceFileName;
                            RowSelected["ProcessMode"] = WProcessModeInWords ; 
                            RowSelected["LastInFileDtTm"] = LastInFileDtTm;
                            RowSelected["LastMatchingDtTm"] = LastMatchingDtTm;

                            // ADD ROW
                            RMCategoryFiles.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReconcCategoriesVsSourcesSeqNo Class............. " + ex.Message;
                }
        }
        //
        // READ Category vs Source Files 
        //
        public void ReadReconcCategoriesVsSources(string InCategoryId, string InSourceFileName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcCategoryVsSourceFiles]"
               + " WHERE CategoryId = @CategoryId AND SourceFileName = @SourceFileName";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@SourceFileName", InSourceFileName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];
                            SourceFileName = (string)rdr["SourceFileName"];
                            PrimaryFile = (bool)rdr["PrimaryFile"];
                            WithRemains = (bool)rdr["WithRemains"];

                            LastInFileDtTm = (DateTime)rdr["LastInFileDtTm"];
                            LastMatchingDtTm = (DateTime)rdr["LastMatchingDtTm"];
                            ProcessMode = (int)rdr["ProcessMode"];

                            if (ProcessMode == -1)
                            {
                                WProcessModeInWords = "Ready For Match";
                            }
                            if (ProcessMode == 0)
                            {
                                WProcessModeInWords = "Under Matching Process";
                            }
                            if (ProcessMode == 1)
                            {
                                WProcessModeInWords = "Matching Finished";
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReconcCategoriesVsSources Class............. " + ex.Message;
                }
        }

        //
        // READ Category vs Source Files by SeqNo 
        //
        public void ReadReconcCategoriesVsSourcebySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcCategoryVsSourceFiles]"
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

                            CategoryId = (string)rdr["CategoryId"];
                            SourceFileName = (string)rdr["SourceFileName"];
                            PrimaryFile = (bool)rdr["PrimaryFile"];
                            WithRemains = (bool)rdr["WithRemains"];

                            LastInFileDtTm = (DateTime)rdr["LastInFileDtTm"];
                            LastMatchingDtTm = (DateTime)rdr["LastMatchingDtTm"];
                            ProcessMode = (int)rdr["ProcessMode"];

                            if (ProcessMode == -1)
                            {
                                WProcessModeInWords = "Ready For Match";
                            }
                            if (ProcessMode == 0)
                            {
                                WProcessModeInWords = "Under Matching Process";
                            }
                            if (ProcessMode == 1)
                            {
                                WProcessModeInWords = "Matching Finished";
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReconcCategoriesVsSourcesSeqNo Class............. " + ex.Message;
                }
        }

        //
        // Execute choreographer 
        //
        public void ReadAndCallKontoToReconcile()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Check that KONTO is available 

            ReadIfKontoAvailable();
            if (KontoAvailable == false)
            {
                return; 
            }

            string SqlString = "SELECT DISTINCT [CategoryId]"
               + " FROM [ATMS].[dbo].[ReconcCategoryVsSourceFiles]"
                + " ORDER BY CategoryId " ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            CategoryId = (string)rdr["CategoryId"];

                            ReadReconcCategoriesVsSourcebyCategory(CategoryId);

                            if (TotalZero == 0 & TotalOne == 0)
                            {
                                
                                //// Call Konto 
                                //StoreProcKontoReconciliation(CategoryId);
                                //// Make -1 to 0 
                                //if (ErrorFound == false & OutReturnedValue == 0)
                                //{
                                //  UpdateReconcCategoryVsSourceRecordProcessCodeToZero(CategoryId);                 
                                //}

                                UpdateReconcCategoryVsSourceRecordProcessCodeToZero(CategoryId);    
   
                                return; 
                            }
                            else
                            {
                                // do nothing
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReconcCategoriesVsSourcesSeqNo Class............. " + ex.Message;
                }
        }


        //
        // IS KONTO AVAILABLE 
        //
        public void ReadIfKontoAvailable()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Check that KONTO is available 

            KontoAvailable = true; 

            string SqlString = "SELECT DISTINCT [CategoryId]"
               + " FROM [ATMS].[dbo].[ReconcCategoryVsSourceFiles]"
                + " ORDER BY CategoryId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            CategoryId = (string)rdr["CategoryId"];

                            ReadReconcCategoriesVsSourcebyCategory(CategoryId);

                            if (TotalZero > 0 )
                            {

                                KontoAvailable = false; 

                                return;
                            }
                            else
                            {
                                // do nothing
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReconcCategoriesVsSourcesSeqNo Class............. " + ex.Message;
                }
        }

        //
        // READ Category vs Source Files by Category 
        //
        public void ReadReconcCategoriesVsSourcebyCategory(string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalMinusOne = 0;
            TotalZero = 0;
            TotalOne = 0;


            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcCategoryVsSourceFiles]"
               + " WHERE CategoryId = @CategoryId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];
                            SourceFileName = (string)rdr["SourceFileName"];
                            PrimaryFile = (bool)rdr["PrimaryFile"];
                            WithRemains = (bool)rdr["WithRemains"];

                            LastInFileDtTm = (DateTime)rdr["LastInFileDtTm"];
                            LastMatchingDtTm = (DateTime)rdr["LastMatchingDtTm"];
                            ProcessMode = (int)rdr["ProcessMode"];

                            if (ProcessMode == -1)
                            {
                                TotalMinusOne = TotalMinusOne + 1; 
                                WProcessModeInWords = "Ready For Match";
                            }
                            if (ProcessMode == 0)
                            {
                                TotalZero = TotalZero + 1; 
                                WProcessModeInWords = "Under Matching Process";
                            }
                            if (ProcessMode == 1)
                            {
                                TotalOne = TotalOne + 1; 
                                WProcessModeInWords = "Matching Finished";
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReconcCategoriesVsSourcesSeqNo Class............. " + ex.Message;
                }
        }

          //
        // CALL KONTO Procedure to make Reconciliation 
        //
        private void StoreProcKontoReconciliation(string InCategoryId)
        {
            ErrorFound = false ;

            string RCT = "[RRDM_Reconciliation].[dbo].[stp_00_RunRecociliationProcess]";
            OutReturnedMessage = ""; 
            OutReturnedValue = 0; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        cmd.Parameters.AddWithValue("@pReconciliationCategory", InCategoryId);
                        cmd.Parameters.AddWithValue("@pReturnedValue", OutReturnedValue);
                        cmd.Parameters.AddWithValue("@pReturnedMessage", OutReturnedMessage);
                        
                        int rows = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in EKonto Store ............. " + ex.Message;

                }
        }

        //
        // UPDATE ProcessMode with Zero = in process 
        // 
        public void UpdateReconcCategoryVsSourceRecordProcessCodeToZero(string InCategoryId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoryVsSourceFiles] SET "
                              + " ProcessMode = @ProcessMode "
                            + " WHERE CategoryId = @CategoryId", conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@ProcessMode", 0);

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
                    ErrorOutput = "An error occured in ReconcCategoryVsSourceRecord Class............. " + ex.Message;
                }
        }

        // Insert Category Vs Source Record 
        //
        public void InsertReconcCategoryVsSourceRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcCategoryVsSourceFiles]"
                    + "([CategoryId], "
                    + " [SourceFileName])"
                    + " VALUES (@CategoryId, "
                    + " @SourceFileName)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@SourceFileName", SourceFileName);
                        //cmd.Parameters.AddWithValue("@TechnicalSourceName", TechnicalSourceName);
                                      
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
                    ErrorOutput = "An error occured in InsertReconcCategoryVsSourceRecord............. " + ex.Message;
                }
        }


        // UPDATE Category
        // 
        public void UpdateReconcCategoryVsSourceRecord(string InOperator, int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoryVsSourceFiles] SET "
                              + " PrimaryFile = @PrimaryFile, WithRemains = @WithRemains "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                        cmd.Parameters.AddWithValue("@PrimaryFile", PrimaryFile);
                        cmd.Parameters.AddWithValue("@WithRemains", WithRemains);

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
                    ErrorOutput = "An error occured in ReconcCategoryVsSourceRecord Class............. " + ex.Message;
                }
        }

        //
        // DELETE Category Vs Source Record 
        //
        public void DeleteReconcCategoryVsSourceRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcCategoryVsSourceFiles]"
                            + " WHERE SeqNo = @SeqNo ", conn))
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
                    ErrorOutput = "An error occured in DeleteReconcCategoryVsSourceRecord............. " + ex.Message;
                }

        }


        //
        // UPDATE ProcessMode using SeqNo
        // 
        public void AgentUpdateReconcCategoryVsSourcesProcessMode(int SeqNo, int ProcessMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            LastInFileDtTm = DateTime.Now;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[ReconcCategoryVsSourceFiles] SET ProcessMode = @ProcessMode,  LastInFileDtTm = @LastInFileDtTm" +
                                                                                        " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);
                        cmd.Parameters.AddWithValue("@LastInFileDtTm", LastInFileDtTm);
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);

                        int rows = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while updating 'ProcessMode' in ReconcCategoriesVsSources.\n The error message is:\n" + ex.Message;
                }
        }

        // For demo purposes
        public void AgentReconcCategoryVsSourcesReset()
        {
            int ProcessMode = 1;
            ErrorFound = false;
            ErrorOutput = "";


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[ReconcCategoryVsSourceFiles] SET ProcessMode = @ProcessMode", conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);
                        int rows = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                }
        }

    
    
    }
}



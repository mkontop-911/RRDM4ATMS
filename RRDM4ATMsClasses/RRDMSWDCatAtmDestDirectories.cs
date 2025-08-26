using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMSWDCatAtmDestDirectories : Logger
    {
        public RRDMSWDCatAtmDestDirectories() : base() { }

        public int SeqNo;

        public string SWDCategoryId;
  
        public string AtmDirectory;
        public string Purpose;

        public string Operator;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMReconcCategoriesSessions Scs = new RRDMReconcCategoriesSessions();

        // Define the data table 
        public DataTable TableCatAtmDestDirectories = new DataTable();

        string SqlString; // Do not delete 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // CatAtmDestDirectories Reader Fields 
        private void CatAtmDestDirectoriesReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            SWDCategoryId = (string)rdr["SWDCategoryId"];

            AtmDirectory = (string)rdr["AtmDirectory"];

            Purpose = (string)rdr["Purpose"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ CatAtmDestDirectories
        // FILL UP A TABLE
        //
        public int TotalSelected ;
        public void ReadCatAtmDestDirectoriesAndFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableCatAtmDestDirectories = new DataTable();
            TableCatAtmDestDirectories.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableCatAtmDestDirectories.Columns.Add("SeqNo", typeof(int));
            TableCatAtmDestDirectories.Columns.Add("SWDCategory", typeof(string));      
            TableCatAtmDestDirectories.Columns.Add("AtmDirectory", typeof(string));
            TableCatAtmDestDirectories.Columns.Add("Purpose", typeof(string));  

            SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[SWDCatAtmDestDirectories] "
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
                            CatAtmDestDirectoriesReaderFields(rdr);

                            DataRow RowSelected = TableCatAtmDestDirectories.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["SWDCategory"] = SWDCategoryId;                      
                            RowSelected["AtmDirectory"] = AtmDirectory;
                            RowSelected["Purpose"] = Purpose;
                           
                            // ADD ROW
                            TableCatAtmDestDirectories.Rows.Add(RowSelected);

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
        // READ CatAtmDestDirectories by Seq no  
        // 
        //
        public void ReadCatAtmDestDirectoriesbySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDCatAtmDestDirectories] "
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

                            // Read Fields 
                            CatAtmDestDirectoriesReaderFields(rdr);

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
        // READ CatAtmDestDirectories by Category  
        // 
        //
        public void ReadCatAtmDestDirectoriesbyCategory(string InSWDCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0; 

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDCatAtmDestDirectories] "
                    + " WHERE SWDCategoryId = @SWDCategoryId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SWDCategoryId", InSWDCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            CatAtmDestDirectoriesReaderFields(rdr);

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

        // GET Array List Occurance Nm 
        //
        public ArrayList GetCatAtmDestDirectories(string InSWDCategoryId)
        {
            ArrayList OccurancesListNm = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * FROM [ATMS].[dbo].[SWDCatAtmDestDirectories]"
                     + " WHERE SWDCategoryId = @SWDCategoryId ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SWDCategoryId", InSWDCategoryId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;

                            AtmDirectory = (string)rdr["AtmDirectory"];

                            //SWDCategoryName = (string)rdr["SWDCategoryName"];

                            //string CatIdAndName = SWDCategoryId + "*" + SWDCategoryName;

                            OccurancesListNm.Add(AtmDirectory);
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

            return OccurancesListNm;
        }

        // Insert CatAtmDestDirectory
        //
        public int InsertCatAtmDestDirectory()
        {
            //
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[SWDCatAtmDestDirectories]"
                    + "([SWDCategoryId],"
                    + " [AtmDirectory], "
                    + " [Purpose], "
                    + " [Operator] )"
                    + " VALUES (@SWDCategoryId,"                
                    + " @AtmDirectory, "
                    + " @Purpose, "                
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
                    
                        cmd.Parameters.AddWithValue("@AtmDirectory", AtmDirectory);

                        cmd.Parameters.AddWithValue("@Purpose", Purpose);
                      
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

        // UPDATE CatAtmDestDirectory
        // 
        public void UpdateCatAtmDestDirectory(string InOperator, int InSeqNo)
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[SWDCatAtmDestDirectories] SET "
                            + " SWDCategoryId = @SWDCategoryId," 
                            + " AtmDirectory = @AtmDirectory, "
                            + " Purpose = @Purpose "
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@SWDCategoryId", SWDCategoryId);
                      
                        cmd.Parameters.AddWithValue("@AtmDirectory", AtmDirectory);

                        cmd.Parameters.AddWithValue("@Purpose", Purpose);

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
        // DELETE CatAtmDestDirectory
        //
        public void DeleteCatAtmDestDirectory(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SWDCatAtmDestDirectories] "
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

using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMSWDCategories : Logger
    {
        public RRDMSWDCategories() : base() { }

        public int SeqNo;

        public string SWDCategoryId;
        public string SWDCategoryName;

        public string SWDOrigin;
        public string ATMsSupplier;

        public string ATMsModel;

        public string OperatingSystem;

        public DateTime OpeningDateTm;
        public string EffectivePackageId; 
        public string Operator;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMReconcCategoriesSessions Scs = new RRDMReconcCategoriesSessions();

        // Define the data table 
        public DataTable TableSWDCateg = new DataTable();

        public int TotalSelected;
        public int TotalMatchingDone;
        public int TotalMatchingNotDone;
        public int TotalReconc;
        public int TotalNotReconc;
        public int TotalUnMatchedRecords;

        string SqlString; // Do not delete 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // SWD Cat Reader Fields 
        private void SWDCatReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            SWDCategoryId = (string)rdr["SWDCategoryId"];

            SWDCategoryName = (string)rdr["SWDCategoryName"];

            SWDOrigin = (string)rdr["SWDOrigin"];

            ATMsSupplier = (string)rdr["ATMsSupplier"];

            ATMsModel = (string)rdr["ATMsModel"];

            OperatingSystem = (string)rdr["OperatingSystem"];

            OpeningDateTm = (DateTime)rdr["OpeningDateTm"];

            EffectivePackageId = (string)rdr["EffectivePackageId"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ SWDCategories  
        // FILL UP A TABLE
        //
        public void ReadSWDCategoriesAndFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableSWDCateg = new DataTable();
            TableSWDCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableSWDCateg.Columns.Add("SeqNo", typeof(int));
            TableSWDCateg.Columns.Add("SWDCategory", typeof(string));
            TableSWDCateg.Columns.Add("Category-Name", typeof(string));
            TableSWDCateg.Columns.Add("SWDOrigin", typeof(string));
            TableSWDCateg.Columns.Add("ATMsSupplier", typeof(string));
            TableSWDCateg.Columns.Add("ATMsModel", typeof(string));
            TableSWDCateg.Columns.Add("EffectivePackageId", typeof(string));

            SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[SWDCategories] "
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
                            SWDCatReaderFields(rdr);

                            DataRow RowSelected = TableSWDCateg.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["SWDCategory"] = SWDCategoryId;
                            RowSelected["Category-Name"] = SWDCategoryName;
                            RowSelected["SWDOrigin"] = SWDOrigin;

                            RowSelected["ATMsSupplier"] = ATMsSupplier;
                            RowSelected["ATMsModel"] = ATMsModel;

                            RowSelected["EffectivePackageId"] = EffectivePackageId;
                           
                            // ADD ROW
                            TableSWDCateg.Rows.Add(RowSelected);

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
        // READ SWDCategory  by Seq no  
        // 
        //
        public void ReadSWDCategoriesbySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDCategories] "
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
                            SWDCatReaderFields(rdr);

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
        // READ SWDCategories by Cat Id   
        // 
        //
        public void ReadSWDCategorybyCategId(string InOperator, string InSWDCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDCategories] "
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
                            SWDCatReaderFields(rdr);

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
        // READ SWDCategories by Cat Name 
        // 
        //
        public void ReadSWDCategorybyCategName(string InOperator, string InSWDCategoryName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDCategories] "
                    + " WHERE Operator = @Operator AND SWDCategoryName = @SWDCategoryName ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SWDCategoryName", InSWDCategoryName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            SWDCatReaderFields(rdr);

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
        // Find Total for this USER
        // 
        //
        public int TotalCatForUser;

        public void ReadSWDCategoriesNumberForUser(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalCatForUser = 0;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDCategories] "
                    + " WHERE OwnerId = @OwnerId ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@OwnerId", InUserId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalCatForUser = TotalCatForUser + 1;


                            SeqNo = (int)rdr["SeqNo"];

                            SWDCategoryId = (string)rdr["SWDCategoryId"];

                            SWDCategoryName = (string)rdr["SWDCategoryName"];


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
        public ArrayList GetCategories(string InOperator)
        {
            ArrayList OccurancesListNm = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * FROM [ATMS].[dbo].[SWDCategories]"
                     + " WHERE Operator = @Operator   Order by SWDCategoryId ASC ";


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

                            SWDCategoryId = (string)rdr["SWDCategoryId"];

                            SWDCategoryName = (string)rdr["SWDCategoryName"];

                            string CatIdAndName = SWDCategoryId + "*" + SWDCategoryName;

                            OccurancesListNm.Add(CatIdAndName);
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
        //
        // Methods 
        // Find Position In Grid
        // 
        //
        public int PositionInGrid;
        public void ReadCategoriesToFindPositionOfSeqNo(string InOperator, int InSeqNo, string InSWDOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            PositionInGrid = -1;
            if (InSWDOrigin == "")
            {
                SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[SWDCategories] "
                   + " WHERE Operator = @Operator "
                   + " ORDER BY SWDCategoryId ASC ";
            }
            else
            {
                SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[SWDCategories] "
                   + " WHERE Operator = @Operator AND SWDOrigin = @SWDOrigin "
                    + " ORDER BY SWDCategoryId ASC ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        if (InSWDOrigin != "")
                        {
                            cmd.Parameters.AddWithValue("@SWDOrigin", InSWDOrigin);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            PositionInGrid = PositionInGrid + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            if (SeqNo == InSeqNo)
                            {
                                break;
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

                    CatchDetails(ex);

                }
        }

        // Insert Reconc Category
        //
        public int InsertSWDCategory()
        {
            //
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[SWDCategories]"
                    + "([SWDCategoryId],"
                    + " [SWDCategoryName],  "
                    + " [SWDOrigin], "
                    + " [ATMsSupplier], "
                    + " [ATMsModel], "
                    + " [OperatingSystem], "
                    + " [OpeningDateTm], "
                    + " [Operator] )"
                    + " VALUES (@SWDCategoryId,"
                    + " @SWDCategoryName,"
                    + " @SWDOrigin, "
                    + " @ATMsSupplier, "
                    + " @ATMsModel, "                  
                    + " @OperatingSystem, "
                    + " @OpeningDateTm, "
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
                        cmd.Parameters.AddWithValue("@SWDCategoryName", SWDCategoryName);

                        cmd.Parameters.AddWithValue("@SWDOrigin", SWDOrigin);

                        cmd.Parameters.AddWithValue("@ATMsSupplier", ATMsSupplier);
                      
                        cmd.Parameters.AddWithValue("@ATMsModel", ATMsModel);

                        cmd.Parameters.AddWithValue("@OperatingSystem", OperatingSystem);

                        cmd.Parameters.AddWithValue("@OpeningDateTm", OpeningDateTm);

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

        // UPDATE Category
        // 
        public void UpdateSWDCategory(string InOperator, int InSeqNo)
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[SWDCategories] SET "
                            + " SWDCategoryId = @SWDCategoryId," 
                            + " SWDCategoryName = @SWDCategoryName, "
                            + " SWDOrigin = @SWDOrigin, "
                            + " ATMsSupplier = @ATMsSupplier,  "
                            + " ATMsModel = @ATMsModel,  "                         
                            + " OperatingSystem = @OperatingSystem,  "
                            + " EffectivePackageId = @EffectivePackageId  "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@SWDCategoryId", SWDCategoryId);
                        cmd.Parameters.AddWithValue("@SWDCategoryName", SWDCategoryName);

                        cmd.Parameters.AddWithValue("@SWDOrigin", SWDOrigin);

                        cmd.Parameters.AddWithValue("@ATMsSupplier", ATMsSupplier);
                     
                        cmd.Parameters.AddWithValue("@ATMsModel", ATMsModel);

                        cmd.Parameters.AddWithValue("@OperatingSystem", OperatingSystem);

                        cmd.Parameters.AddWithValue("@EffectivePackageId", EffectivePackageId);

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
        public void DeleteSWDCategory(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            int rows; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SWDCategories] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

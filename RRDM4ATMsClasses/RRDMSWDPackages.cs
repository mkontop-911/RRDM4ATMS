using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
//using System.Collections;


namespace RRDM4ATMs
{
    public class RRDMSWDPackages : Logger
    {
        public RRDMSWDPackages() : base() { }

        public int SeqNo;
        public string SWDCategoryId;
        public string PackageId;

        public string BankPackId;

        public string PackageName;

        public string PackageDescription;

        public string Origin;
        public DateTime CreatedDtTm;
        public bool ForUpdating;

        public bool ForFullInstallation;

        public bool PutOutOfService;
        public bool NeedReboot;

        public string Maker;
        public string Approver;
        public int PriorityOneToFive;

        public bool ClosedPack;

        public string Operator;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable TableSWDPackages = new DataTable();

        public int TotalSelected;
        public int TotalMatchingDone;
        public int TotalMatchingNotDone;
        public int TotalReconc;
        public int TotalNotReconc;
        public int TotalUnMatchedRecords;

        string SqlString; // Do not delete 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // SWD Packages Reader Fields 
        private void SWDPackagesReaderFields(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            SWDCategoryId = (string)rdr["SWDCategoryId"];

            PackageId = (string)rdr["PackageId"];

            BankPackId = (string)rdr["BankPackId"];

            PackageName = (string)rdr["PackageName"];

            PackageDescription = (string)rdr["PackageDescription"];

            Origin = (string)rdr["Origin"];

            CreatedDtTm = (DateTime)rdr["CreatedDtTm"];

            ForUpdating = (bool)rdr["ForUpdating"];

            ForFullInstallation = (bool)rdr["ForFullInstallation"];

            PutOutOfService = (bool)rdr["PutOutOfService"];
            NeedReboot = (bool)rdr["NeedReboot"];

            Maker = (string)rdr["Maker"];
            Approver = (string)rdr["Approver"];
            PriorityOneToFive = (int)rdr["PriorityOneToFive"];

            ClosedPack = (bool)rdr["ClosedPack"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ SWDPackages
        // FILL UP A TABLE
        //
        public void ReadSWDPackagesAndFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableSWDPackages = new DataTable();
            TableSWDPackages.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableSWDPackages.Columns.Add("SeqNo", typeof(int));
            TableSWDPackages.Columns.Add("SWDIdentity", typeof(string));
            TableSWDPackages.Columns.Add("PackageName", typeof(string));
            TableSWDPackages.Columns.Add("CreatedDtTm", typeof(DateTime));

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[SWDPackages] "
                   + InSelectionCriteria;
            //}

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
                            SWDPackagesReaderFields(rdr);

                            DataRow RowSelected = TableSWDPackages.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["SWDIdentity"] = PackageId;

                            RowSelected["PackageName"] = PackageName;
                            RowSelected["CreatedDtTm"] = CreatedDtTm;

                            // ADD ROW
                            TableSWDPackages.Rows.Add(RowSelected);

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
        // READ SWDPackagesVersions  by Seq no  
        // 
        //
        public void ReadSWDPackagesbySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDPackages] "
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
                            SWDPackagesReaderFields(rdr);

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
        // READ SWDPackages by Package Id   
        // 
        //
        public void ReadSWDPackagesbyPackageId(string InOperator, string InPackageId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDPackages] "
                    + " WHERE Operator = @Operator AND PackageId = @PackageId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@PackageId", InPackageId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                            // Read Fields 
                            SWDPackagesReaderFields(rdr);

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
        // READ SWDPackages by Package Name 
        // 
        //
        public void ReadSWDPackagesbyPackageName(string InOperator, string InPackageName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDPackages] "
                    + " WHERE Operator = @Operator AND PackageName = @PackageName ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@PackageName", InPackageName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            SWDPackagesReaderFields(rdr);

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



        // Insert SWDPackage
        //
        public int InsertSWDPackage()
        {
            //
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[SWDPackages]"
                    + "([SWDCategoryId],"
                    + " [PackageId],  "
                     + " [BankPackId],  "
                    + " [PackageName], "
                    + " [PackageDescription], "
                    + " [CreatedDtTm], "
                    + " [ForUpdating], "
                    + " [ForFullInstallation], "
                    + " [PutOutOfService], "
                    + " [NeedReboot], "
                    + " [Maker], "
                    + " [Approver], "
                    + " [PriorityOneToFive], "
                    + " [Operator] )"
                    + " VALUES (@SWDCategoryId,"
                    + " @PackageId,"
                    + " @BankPackId,"
                    + " @PackageName, "
                    + " @PackageDescription, "
                    + " @CreatedDtTm, "
                    + " @ForUpdating, "
                    + " @ForFullInstallation, "
                    + " @PutOutOfService, "
                    + " @NeedReboot, "
                    + " @Maker, "
                    + " @Approver, "
                    + " @PriorityOneToFive, "
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
                        cmd.Parameters.AddWithValue("@PackageId", PackageId);
                        cmd.Parameters.AddWithValue("@BankPackId", BankPackId);

                        cmd.Parameters.AddWithValue("@PackageName", PackageName);

                        cmd.Parameters.AddWithValue("@PackageDescription", PackageDescription);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@CreatedDtTm", CreatedDtTm);
                        cmd.Parameters.AddWithValue("@ForUpdating", ForUpdating);

                        cmd.Parameters.AddWithValue("@ForFullInstallation", ForFullInstallation);

                        cmd.Parameters.AddWithValue("@PutOutOfService", PutOutOfService);
                        cmd.Parameters.AddWithValue("@NeedReboot", NeedReboot);

                        cmd.Parameters.AddWithValue("@Maker", Maker);
                        cmd.Parameters.AddWithValue("@Approver", Approver);
                        cmd.Parameters.AddWithValue("@PriorityOneToFive", PriorityOneToFive);

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
        public void UpdateSWDPackages(string InOperator, int InSeqNo)
        {
            Successful = false;
            ErrorFound = false;
            ErrorOutput = "";
            int rows;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[SWDPackages] SET "
                            + " SWDCategoryId = @SWDCategoryId,"
                            + " PackageId = @PackageId,"
                            + " BankPackId = @BankPackId,"
                            + " PackageName = @PackageName, "
                            + " PackageDescription = @PackageDescription, "
                            + " Origin = @Origin,  "
                            + " ForUpdating = @ForUpdating,"
                            + " ForFullInstallation = @ForFullInstallation, "
                            + " PutOutOfService = @PutOutOfService, "
                            + " NeedReboot = @NeedReboot, "
                            + " Maker = @Maker,  "
                            + " Approver = @Approver,  "
                            + " PriorityOneToFive = @PriorityOneToFive,  "
                            + " ClosedPack = @ClosedPack  "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@SWDCategoryId", SWDCategoryId);
                        cmd.Parameters.AddWithValue("@PackageId", PackageId);
                        cmd.Parameters.AddWithValue("@BankPackId", BankPackId);
                        cmd.Parameters.AddWithValue("@PackageName", PackageName);

                        cmd.Parameters.AddWithValue("@PackageDescription", PackageDescription);

                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        cmd.Parameters.AddWithValue("@ForUpdating", ForUpdating);

                        cmd.Parameters.AddWithValue("@ForFullInstallation", ForFullInstallation);

                        cmd.Parameters.AddWithValue("@PutOutOfService", PutOutOfService);
                        cmd.Parameters.AddWithValue("@NeedReboot", NeedReboot);

                        cmd.Parameters.AddWithValue("@Maker", Maker);
                        cmd.Parameters.AddWithValue("@Approver", Approver);
                        cmd.Parameters.AddWithValue("@PriorityOneToFive", PriorityOneToFive);
                        cmd.Parameters.AddWithValue("@ClosedPack", ClosedPack);

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

        //
        // DELETE Category
        //
        public void DeleteSWDPackages(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SWDPackages] "
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



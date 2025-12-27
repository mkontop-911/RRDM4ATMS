using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
//using System.Collections;


namespace RRDM4ATMs
{
    public class RRDMSWDPackagesDistributionSessions : Logger
    {
        public RRDMSWDPackagesDistributionSessions() : base() { }

        public int PackDistrSesNo;
        public string SWDCategoryId;
        public string PackageId;
        public int TypeOfDistribution; // 1= PreProduction, 2=Pilot, 3=Production, 4= Single Atm

        public string SingleAtmNo;

        public DateTime StartDateTm;
        public DateTime EndDateTm;

        public int ATMsForSWD;
        public int ATMsDone;
        public int ATMsNotDone;

        public bool SuccessPreProduction;
        public bool SuccessPilot;

        public string Maker;
        public string Approver;

        public int ProcessStage;
        public string Operator;
        //
        //
        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable TablePackDistrSes = new DataTable();

        public int TotalSelected;
        public int TotalMatchingDone;
        public int TotalMatchingNotDone;
        public int TotalReconc;
        public int TotalNotReconc;
        public int TotalUnMatchedRecords;

        string SqlString; // Do not delete 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // SWD Packages Reader Fields 
        private void SWDPackDistrSesReaderFields(SqlDataReader rdr)
        {
            
            PackDistrSesNo = (int)rdr["PackDistrSesNo"];
            SWDCategoryId = (string)rdr["SWDCategoryId"];
            PackageId = (string)rdr["PackageId"];

            TypeOfDistribution = (int)rdr["TypeOfDistribution"];
            SingleAtmNo = (string)rdr["SingleAtmNo"];

            StartDateTm = (DateTime)rdr["StartDateTm"];
            EndDateTm = (DateTime)rdr["EndDateTm"];

            ATMsForSWD = (int)rdr["ATMsForSWD"];
            ATMsDone = (int)rdr["ATMsDone"];
            ATMsNotDone = (int)rdr["ATMsNotDone"];

            SuccessPreProduction = (bool)rdr["SuccessPreProduction"];
            SuccessPilot = (bool)rdr["SuccessPilot"];

            Maker = (string)rdr["Maker"];
            Approver = (string)rdr["Approver"];

            ProcessStage = (int)rdr["ProcessStage"];
            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ SWDPackDistrSes
        // FILL UP A TABLE
        //
        public void ReadSWDPackDistrSesAndFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TablePackDistrSes = new DataTable();
            TablePackDistrSes.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TablePackDistrSes.Columns.Add("PackDistrSesNo", typeof(int));
            TablePackDistrSes.Columns.Add("SWDCategoryId", typeof(string));
            TablePackDistrSes.Columns.Add("PackageId", typeof(string));
            TablePackDistrSes.Columns.Add("TypeOfDistribution", typeof(int));
            TablePackDistrSes.Columns.Add("StartDateTm", typeof(DateTime));
            TablePackDistrSes.Columns.Add("ATMsForSWD", typeof(int));
            TablePackDistrSes.Columns.Add("ATMsDone", typeof(int));
            TablePackDistrSes.Columns.Add("ATMsNotDone", typeof(int));

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[SWDPackagesDistributionSessions] "
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
                            SWDPackDistrSesReaderFields(rdr);

                            DataRow RowSelected = TablePackDistrSes.NewRow();

                            RowSelected["PackDistrSesNo"] = PackDistrSesNo;
                            RowSelected["SWDCategoryId"] = SWDCategoryId;
                            RowSelected["PackageId"] = PackageId;
                            RowSelected["TypeOfDistribution"] = TypeOfDistribution;

                            RowSelected["StartDateTm"] = StartDateTm;
                            RowSelected["ATMsForSWD"] = ATMsForSWD;
                            RowSelected["ATMsDone"] = ATMsDone;
                            RowSelected["ATMsNotDone"] = ATMsNotDone;

                            // ADD ROW
                            TablePackDistrSes.Rows.Add(RowSelected);

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
        // READ SWDPackDistrSes  by Seq no  
        // 
        //
        public void ReadSWDPackDistrSesbyPackDistrSesNo(string InOperator, int InPackDistrSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDPackagesDistributionSessions] "
                    + " WHERE Operator = @Operator AND PackDistrSesNo = @PackDistrSesNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@PackDistrSesNo", InPackDistrSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            SWDPackDistrSesReaderFields(rdr);

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
        // READ SWDPackDistrSes by Package Id   
        // 
        //
        public void ReadSWDPackDistrSesbyPackageId(string InOperator, string InPackageId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDPackagesDistributionSessions] "
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
                            SWDPackDistrSesReaderFields(rdr);

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
        // READ SWDPackDistrSes by SelectionCriteria
        // 
        //
        public void ReadSWDPackDistrSesbySelectionCriteria(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SWDPackagesDistributionSessions] "
                    + InSelectionCriteria;


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);
                    

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            SWDPackDistrSesReaderFields(rdr);

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

        // Insert SWDPackDistrSes
        //
        public int InsertSWDPackDistrSes()
        {
            //
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[SWDPackagesDistributionSessions]"
                    + "([SWDCategoryId],"
                    + " [PackageId],  "
                    + " [TypeOfDistribution],  "
                    + " [SingleAtmNo],  "
                    + " [StartDateTm], "
                    + " [EndDateTm], "
                    + " [ATMsForSWD], "
                    + " [ATMsDone], "
                    + " [ATMsNotDone], "
                    + " [SuccessPreProduction], "
                    + " [SuccessPilot], "
                    + " [Maker], "
                    + " [Approver], "
                    + " [ProcessStage], "
                    + " [Operator] )"
                    + " VALUES (@SWDCategoryId,"
                    + " @PackageId,"
                    + " @TypeOfDistribution,"
                    + " @SingleAtmNo,"
                    + " @StartDateTm, "
                    + " @EndDateTm, "
                    + " @ATMsForSWD, "
                    + " @ATMsDone, "
                    + " @ATMsNotDone, "
                    + " @SuccessPreProduction, "
                    + " @SuccessPilot, "
                    + " @Maker, "
                    + " @Approver, "
                    + " @ProcessStage, "
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
                        cmd.Parameters.AddWithValue("@TypeOfDistribution", TypeOfDistribution);
                        cmd.Parameters.AddWithValue("@SingleAtmNo", SingleAtmNo);

                        cmd.Parameters.AddWithValue("@StartDateTm", StartDateTm);
                        cmd.Parameters.AddWithValue("@EndDateTm", EndDateTm);

                        cmd.Parameters.AddWithValue("@ATMsForSWD", ATMsForSWD);
                        cmd.Parameters.AddWithValue("@ATMsDone", ATMsDone);
                        cmd.Parameters.AddWithValue("@ATMsNotDone", ATMsNotDone);

                        cmd.Parameters.AddWithValue("@SuccessPreProduction", SuccessPreProduction);
                        cmd.Parameters.AddWithValue("@SuccessPilot", SuccessPilot);

                        cmd.Parameters.AddWithValue("@Maker", Maker);
                        cmd.Parameters.AddWithValue("@Approver", Approver);

                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);

                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        //rows number of record got updated

                        PackDistrSesNo = (int)cmd.ExecuteScalar();
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
            return PackDistrSesNo;
        }

        // UPDATE PackDistrSesNo
        // 
        public void UpdateSWDPackDistrSes(string InOperator, int InPackDistrSesNo)
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[SWDPackagesDistributionSessions] SET "
                            + " SWDCategoryId = @SWDCategoryId,"
                            + " PackageId = @PackageId,"
                             + " TypeOfDistribution = @TypeOfDistribution,"
                              + " SingleAtmNo = @SingleAtmNo,"
                            + " StartDateTm = @StartDateTm, "
                            + " EndDateTm = @EndDateTm, "
                            + " ATMsForSWD = @ATMsForSWD,  "
                            + " ATMsDone = @ATMsDone,"
                            + " ATMsNotDone = @ATMsNotDone, "
                            + " SuccessPreProduction = @SuccessPreProduction, "
                            + " SuccessPilot = @SuccessPilot, "
                            + " Maker = @Maker,  "
                            + " Approver = @Approver,  "
                            + " ProcessStage = @ProcessStage  "
                            + " WHERE PackDistrSesNo = @PackDistrSesNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@PackDistrSesNo", InPackDistrSesNo);
                        cmd.Parameters.AddWithValue("@SWDCategoryId", SWDCategoryId);
                        cmd.Parameters.AddWithValue("@PackageId", PackageId);
                        cmd.Parameters.AddWithValue("@TypeOfDistribution", TypeOfDistribution);
                        cmd.Parameters.AddWithValue("@SingleAtmNo", SingleAtmNo);

                        cmd.Parameters.AddWithValue("@StartDateTm", StartDateTm);
                        cmd.Parameters.AddWithValue("@EndDateTm", EndDateTm);

                        cmd.Parameters.AddWithValue("@ATMsForSWD", ATMsForSWD);
                        cmd.Parameters.AddWithValue("@ATMsDone", ATMsDone);
                        cmd.Parameters.AddWithValue("@ATMsNotDone", ATMsNotDone);

                        cmd.Parameters.AddWithValue("@SuccessPreProduction", SuccessPreProduction);
                        cmd.Parameters.AddWithValue("@SuccessPilot", SuccessPilot);

                        cmd.Parameters.AddWithValue("@Maker", Maker);
                        cmd.Parameters.AddWithValue("@Approver", Approver);

                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);

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
        public void DeleteSWDPackDistrSes(int InPackDistrSesNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SWDPackagesDistributionSessions] "
                            + " WHERE PackDistrSesNo =  @PackDistrSesNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@PackDistrSesNo", InPackDistrSesNo);

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



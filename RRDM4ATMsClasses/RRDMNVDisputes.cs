using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;


namespace RRDM4ATMs
{
    public class RRDMNVDisputes : Logger
    {
        public RRDMNVDisputes() : base() { }

        public int SeqNo;
        public bool IsExternal;
        public string InternalAccNo;
        public string VostroBank;
        public string ExternalAccNo;

        public bool IsInternalDispute;
        public string InternalBranchId;
        public string InternalTellerName;
        public string InternalTellerPhone;
        public DateTime OpenDate;

        public int DispType;
        public string TypeDescription;
        public DateTime TargetDateTm;
        public string OpenByUserId;
        public string DispComments;

        public bool HasOwner;
        public string OwnerId;
        public DateTime SettledDate;
        public string SettledReason;
        public bool Active;
        public string Operator;

        // Define the data table 
        public DataTable TableDisputes = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        //
        // Methods 
        // READ NVDisputes
        // FILL UP A TABLE
        //
        public void ReadNVDisputesFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableDisputes = new DataTable();
            TableDisputes.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableDisputes.Columns.Add("SeqNo", typeof(int));
            TableDisputes.Columns.Add("OpenDate", typeof(DateTime));
            TableDisputes.Columns.Add("TargetDateTm", typeof(DateTime));
            TableDisputes.Columns.Add("Description", typeof(string));
            TableDisputes.Columns.Add("IsExternal", typeof(string));
            TableDisputes.Columns.Add("VostroBank", typeof(string));
            TableDisputes.Columns.Add("ExternalAccNo", typeof(string));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVDisputesTable] "
                    + InSelectionCriteria
                    + " Order  By SeqNo DESC ";


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

                            SeqNo = (int)rdr["SeqNo"];
                            IsExternal = (bool)rdr["IsExternal"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            VostroBank = (string)rdr["VostroBank"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];

                            IsInternalDispute = (bool)rdr["IsInternalDispute"];
                            InternalBranchId = (string)rdr["InternalBranchId"];
                            InternalTellerName = (string)rdr["InternalTellerName"];
                            InternalTellerPhone = (string)rdr["InternalTellerPhone"];
                            OpenDate = (DateTime)rdr["OpenDate"];

                            DispType = (int)rdr["DispType"];
                            TypeDescription = (string)rdr["TypeDescription"];
                            TargetDateTm = (DateTime)rdr["TargetDateTm"];
                            OpenByUserId = (string)rdr["OpenByUserId"];
                            DispComments = (string)rdr["DispComments"];

                            HasOwner = (bool)rdr["HasOwner"];
                            OwnerId = (string)rdr["OwnerId"];

                            SettledDate = (DateTime)rdr["SettledDate"];
                            SettledReason = (string)rdr["SettledReason"];

                            Active = (bool)rdr["Active"];
                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableDisputes.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["OpenDate"] = OpenDate;
                            RowSelected["TargetDateTm"] = TargetDateTm;
                            RowSelected["Description"] = TypeDescription;
                            RowSelected["IsExternal"] = IsExternal;
                            RowSelected["VostroBank"] = VostroBank;
                            RowSelected["ExternalAccNo"] = ExternalAccNo;

                            // ADD ROW
                            TableDisputes.Rows.Add(RowSelected);

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
        // READ NVDisputes SeqNo
        // 
        //
        public void ReadNVDisputesBySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVDisputesTable] "
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

                            SeqNo = (int)rdr["SeqNo"];
                            IsExternal = (bool)rdr["IsExternal"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            VostroBank = (string)rdr["VostroBank"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];

                            IsInternalDispute = (bool)rdr["IsInternalDispute"];
                            InternalBranchId = (string)rdr["InternalBranchId"];
                            InternalTellerName = (string)rdr["InternalTellerName"];
                            InternalTellerPhone = (string)rdr["InternalTellerPhone"];
                            OpenDate = (DateTime)rdr["OpenDate"];

                            DispType = (int)rdr["DispType"];
                            TypeDescription = (string)rdr["TypeDescription"];
                            TargetDateTm = (DateTime)rdr["TargetDateTm"];
                            OpenByUserId = (string)rdr["OpenByUserId"];
                            DispComments = (string)rdr["DispComments"];

                            HasOwner = (bool)rdr["HasOwner"];
                            OwnerId = (string)rdr["OwnerId"];
                            SettledDate = (DateTime)rdr["SettledDate"];
                            SettledReason = (string)rdr["SettledReason"];
                            Active = (bool)rdr["Active"];
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

                    CatchDetails(ex);

                }
        }

        //
        // Insert NVDisputes
        //
        public int InsertNewNVDispute()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = " INSERT INTO [dbo].[NVDisputesTable] "
                 + " ([IsExternal] "
          + " ,[InternalAccNo] "
          + " ,[VostroBank] "
          + " ,[ExternalAccNo] "
          + " ,[IsInternalDispute]"
          + " ,[InternalBranchId]"
          + " ,[InternalTellerName]"
          + " ,[InternalTellerPhone]"
          + " ,[OpenDate]"
          + " ,[DispType]"
          + " ,[TypeDescription]"
          + " ,[TargetDateTm]"
          + " ,[OpenByUserId]"
          + " ,[DispComments]"
          + " ,[HasOwner]"
          + " ,[OwnerId]"
          + " ,[Active]"
          + " ,[Operator])"
     + " VALUES"
          + " ( @IsExternal "
          + " ,@InternalAccNo "
          + " ,@VostroBank "
          + " ,@ExternalAccNo "
          + " ,@IsInternalDispute "
          + " ,@InternalBranchId "
          + " ,@InternalTellerName "
          + " ,@InternalTellerPhone "
          + " ,@OpenDate "
          + " ,@DispType "
          + " ,@TypeDescription "
          + " ,@TargetDateTm "
          + " ,@OpenByUserId "
          + " ,@DispComments "
          + " ,@HasOwner "
          + " ,@OwnerId "
          + " ,@Active "
          + " ,@Operator ) "
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                      //  cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                        cmd.Parameters.AddWithValue("@IsExternal", IsExternal);
                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                        cmd.Parameters.AddWithValue("@VostroBank", VostroBank);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", ExternalAccNo);

                        cmd.Parameters.AddWithValue("@IsInternalDispute", IsInternalDispute);
                        cmd.Parameters.AddWithValue("@InternalBranchId", InternalBranchId);
                        cmd.Parameters.AddWithValue("@InternalTellerName", InternalTellerName);
                        cmd.Parameters.AddWithValue("@InternalTellerPhone", InternalTellerPhone);
                        cmd.Parameters.AddWithValue("@OpenDate", OpenDate);

                        cmd.Parameters.AddWithValue("@DispType", DispType);
                        cmd.Parameters.AddWithValue("@TypeDescription", TypeDescription);
                        cmd.Parameters.AddWithValue("@TargetDateTm", TargetDateTm);
                        cmd.Parameters.AddWithValue("@OpenByUserId", OpenByUserId);
                        cmd.Parameters.AddWithValue("@DispComments", DispComments);

                        cmd.Parameters.AddWithValue("@HasOwner", HasOwner);
                        cmd.Parameters.AddWithValue("@OwnerId", OwnerId);
                        cmd.Parameters.AddWithValue("@Active", Active);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    ErrorFound = true;

                    CatchDetails(ex);
                }

            return SeqNo;
        }
        // 
        // UPDATE NVDisputes Record 
        // 
        public void UpdateNVDisputesRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[NVDisputesTable] SET "
                            + " InternalBranchId = @InternalBranchId, "
                            + " InternalTellerName = @InternalTellerName, "
                            + " InternalTellerPhone = @InternalTellerPhone, "
                            + " TargetDateTm = @TargetDateTm, "
                            + " DispComments = @DispComments, "
                            + " HasOwner = @HasOwner, "
                            + " OwnerId = @OwnerId, "
                            + " SettledDate = @SettledDate, "
                            + " SettledReason = @SettledReason, "
                            + " Active = @Active "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);

                        cmd.Parameters.AddWithValue("@InternalBranchId", InternalBranchId);
                        cmd.Parameters.AddWithValue("@InternalTellerName", InternalTellerName);
                        cmd.Parameters.AddWithValue("@InternalTellerPhone", InternalTellerPhone);

                        cmd.Parameters.AddWithValue("@TargetDateTm", TargetDateTm);
                        cmd.Parameters.AddWithValue("@DispComments", DispComments);

                        cmd.Parameters.AddWithValue("@HasOwner", HasOwner);
                        cmd.Parameters.AddWithValue("@OwnerId", OwnerId);

                        cmd.Parameters.AddWithValue("@SettledDate", SettledDate);
                        cmd.Parameters.AddWithValue("@SettledReason", SettledReason);
                        cmd.Parameters.AddWithValue("@Active", Active);

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

        //
        // DELETE NVDisputes
        //
        public void DeleteNVDisputesEntry(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVDisputesTable] "
                            + " WHERE SeqNo = @SeqNo ", conn))
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

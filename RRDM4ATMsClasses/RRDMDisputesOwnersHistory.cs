using System;
using System.Data;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMDisputesOwnersHistory : Logger
    {
        public RRDMDisputesOwnersHistory() : base() { }

        RRDMUsersRecords Us = new RRDMUsersRecords(); 

        public int SeqNumber;
        public int DispId;

        public DateTime StartDate;
        public DateTime EndDate;

        public DateTime CloseDate;

        public string CreatorId;
        public bool HasOwner;
        public string OwnerId;

        public string Reason; 

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // Define the data table 
        public DataTable OwnersSelected = new DataTable();
        public int TotalSelected;
        string SqlString; // Do not delete

        string connectionString = ConfigurationManager.ConnectionStrings
         ["ATMSConnectionString"].ConnectionString;

        // Methods 
        // READ Dispute Owners History  
        // FILL UP A TABLE 
        public void ReadDisputeOwnersHistory(string InOperator, int InDispId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            OwnersSelected = new DataTable();
            OwnersSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            OwnersSelected.Columns.Add("SeqNumber", typeof(int));
            OwnersSelected.Columns.Add("OwnerId", typeof(string));
            OwnersSelected.Columns.Add("OwnerName", typeof(string));
            OwnersSelected.Columns.Add("StartDate", typeof(DateTime));
            OwnersSelected.Columns.Add("EndDate", typeof(string));
            OwnersSelected.Columns.Add("CreatorId", typeof(string));
            OwnersSelected.Columns.Add("Reason", typeof(string));

            SqlString = "SELECT *"
                     + " FROM [dbo].[DisputeOwnersHistory] "
                     + " WHERE Operator=@Operator AND DispId=@DispId ";
            
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@DispId", InDispId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                            DataRow RowSelected = OwnersSelected.NewRow();

                            RowSelected["SeqNumber"] = (int)rdr["SeqNumber"];

                            HasOwner = (bool)rdr["HasOwner"];

                            if (HasOwner == true)
                            {
                                string Temp = (string)rdr["OwnerId"];
                                Us.ReadUsersRecord(Temp);
                                RowSelected["OwnerId"] = Temp;
                                RowSelected["OwnerName"] = Us.UserName;
                            }
                            else
                            {
                                RowSelected["OwnerId"] = "NoId";
                                RowSelected["OwnerName"] = "Record with No Owner";
                            }
                            RowSelected["StartDate"] = (DateTime)rdr["StartDate"];

                            DateTime Ed = (DateTime)rdr["EndDate"];
                            string EdStr = Ed.ToString(); 

                            if (Ed.Date == NullPastDate)
                            {
                                RowSelected["EndDate"] = "N/A";
                            }
                            else
                            {
                                RowSelected["EndDate"] = EdStr ;
                            }

                            RowSelected["CreatorId"] = (string)rdr["CreatorId"];
                            RowSelected["Reason"] = (string)rdr["Reason"]; 

                            // ADD ROW
                            OwnersSelected.Rows.Add(RowSelected);

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

        // Methods 
        // READ Specific Dispute Owner by sequence Number 
        // 
        public void ReadDisputeOwnersRecordSpecificSeqNo(string InOperator, int InSeqNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
              + " FROM [dbo].[DisputeOwnersHistory] "
              + " WHERE Operator=@Operator AND SeqNumber=@SeqNumber ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            DispId = (int)rdr["DispId"];

                            StartDate = (DateTime)rdr["StartDate"];
                            EndDate = (DateTime)rdr["EndDate"];
                            CloseDate = (DateTime)rdr["CloseDate"];

                            CreatorId = (string)rdr["CreatorId"];
                            HasOwner = (bool)rdr["HasOwner"];
                            OwnerId = (string)rdr["OwnerId"];

                            Reason = (string)rdr["Reason"];

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

        // Methods 
        // READ Specific Dispute Owner by Dispute Number 
        // The last record one is read. 
        //
        public void ReadDisputeOwnersRecordSpecificDispId(string InOperator, int InDispId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
              + " FROM [dbo].[DisputeOwnersHistory] "
              + " WHERE Operator=@Operator AND DispId=@DispId ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@DispId", InDispId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            DispId = (int)rdr["DispId"];

                            StartDate = (DateTime)rdr["StartDate"];
                            EndDate = (DateTime)rdr["EndDate"];
                            CloseDate = (DateTime)rdr["CloseDate"];

                            CreatorId = (string)rdr["CreatorId"];
                            HasOwner = (bool)rdr["HasOwner"];
                            OwnerId = (string)rdr["OwnerId"];

                            Reason = (string)rdr["Reason"];

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
        // INSERT New Record in Dispute Owners History 
        //
        public void InsertDisputeOwnersHistory(string InOperator, int InDispId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[DisputeOwnersHistory] "
                + "( [DispId],[StartDate],[CreatorId],[Reason], "
                + " [HasOwner],[OwnerId],"
                + " [Operator]) "
                + " VALUES "
                + "( @DispId, @StartDate, @CreatorId, @Reason, "
                + " @HasOwner, @OwnerId, "
                + " @Operator ) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@DispId", DispId);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);

                        cmd.Parameters.AddWithValue("@CreatorId", CreatorId);
                        cmd.Parameters.AddWithValue("@Reason", Reason);

                        cmd.Parameters.AddWithValue("@HasOwner", HasOwner);
                        cmd.Parameters.AddWithValue("@OwnerId", OwnerId);
                     
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            // outcome = " Record Inserted ";
                        }


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
        // UPDATE Dispute Owners History 
        //
        public void UpdateDisputeOwnersHistory(string InOperator, int InSeqNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[DisputeOwnersHistory] SET "
                            + "[EndDate] =@EndDate,"
                            + "[CloseDate] = @CloseDate  "
                            + " WHERE SeqNumber= @SeqNumber AND Operator= @Operator", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        cmd.Parameters.AddWithValue("@EndDate", EndDate);
                        cmd.Parameters.AddWithValue("@CloseDate", CloseDate);

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //  outcome = " ATMs Table UPDATED ";
                        }

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            //  return outcome;

        }
    }
}

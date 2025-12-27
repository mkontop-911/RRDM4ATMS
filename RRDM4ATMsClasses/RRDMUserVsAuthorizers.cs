using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMUserVsAuthorizers : Logger
    {
        public RRDMUserVsAuthorizers() : base() { }
        // Class of User and his Authorizers 

        public int SeqNumber;

        public string UserId;
        public string Authoriser;
        public string AuthorName;

        public int TypeOfAuth;

        public DateTime DateOfInsert;
        public DateTime DateOfClose;
   
        public bool OpenRecord;
        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public DataTable UsersToAuthorDataTable = new DataTable();

        public int TotalSelected;
        string SqlString; // Do not delete

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        //
        // READ UserVsAuthorisation and FILL Table
        // CALLED FROM FORM13
        //
        public void ReadUserVsAuthorizersFillDataTable(string InFilter)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            UsersToAuthorDataTable = new DataTable();
            UsersToAuthorDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            UsersToAuthorDataTable.Columns.Add("SeqNo", typeof(int));
            UsersToAuthorDataTable.Columns.Add("UserId", typeof(string));
            UsersToAuthorDataTable.Columns.Add("AuthorId", typeof(string));
            UsersToAuthorDataTable.Columns.Add("AuthoriserName", typeof(string));
            UsersToAuthorDataTable.Columns.Add("TypeOfAuthor", typeof(int));
            UsersToAuthorDataTable.Columns.Add("DateOfInsert", typeof(string));

            SqlString = "SELECT *"
                          + " FROM [ATMS].[dbo].[UserVsAuthorizers]"
                         + " WHERE " + InFilter
                          + " Order by UserId ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthorName = (string)rdr["AuthorName"];
                            TypeOfAuth = (int)rdr["TypeOfAuth"];

                            DateOfInsert = (DateTime)rdr["DateOfInsert"];
                            DateOfClose = (DateTime)rdr["DateOfClose"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

                            //
                            //FILL IN TABLE
                            //

                            DataRow RowSelected = UsersToAuthorDataTable.NewRow();

                            RowSelected["SeqNo"] = SeqNumber;

                            RowSelected["UserId"] = UserId;
                            RowSelected["AuthorId"] = Authoriser;
                            RowSelected["AuthoriserName"] = AuthorName;
                            RowSelected["TypeOfAuthor"] = TypeOfAuth;
                            RowSelected["DateOfInsert"] = DateOfInsert.ToString();

                            // ADD ROW
                            UsersToAuthorDataTable.Rows.Add(RowSelected);

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
        // READ UserVsAuthorisation and FILL Table
        // CALLED FROM AUTHORISATION PROCESS
        //
        public void ReadUserVsAuthorizersFillDataTable2(string InFilter)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            UsersToAuthorDataTable = new DataTable();
            UsersToAuthorDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            UsersToAuthorDataTable.Columns.Add("AuthId", typeof(string));
            UsersToAuthorDataTable.Columns.Add("Authoriser Name", typeof(string));
            UsersToAuthorDataTable.Columns.Add("Type Of Author", typeof(string));
            UsersToAuthorDataTable.Columns.Add("Openning Date", typeof(string));

            SqlString = "SELECT *"
                          + " FROM [ATMS].[dbo].[UserVsAuthorizers]"
                         + " WHERE " + InFilter
                          + " Order by UserId ASC ";

            //string SqlString = "SELECT *"
            //   + " FROM [ATMS].[dbo].[UserVsAuthorizers]"
            //   + " WHERE SeqNumber = @SeqNumber";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthorName = (string)rdr["AuthorName"];
                            TypeOfAuth = (int)rdr["TypeOfAuth"];

                            DateOfInsert = (DateTime)rdr["DateOfInsert"];
                            DateOfClose = (DateTime)rdr["DateOfClose"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

                            //
                            //FILL IN TABLE
                            //

                            DataRow RowSelected = UsersToAuthorDataTable.NewRow();
                         
                            RowSelected["AuthId"] = Authoriser;
                            RowSelected["Authoriser Name"] = AuthorName;
                            RowSelected["Type Of Author"] = TypeOfAuth;
                            RowSelected["Openning Date"] = DateOfInsert.ToString();

                            // ADD ROW
                            UsersToAuthorDataTable.Rows.Add(RowSelected);

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
        // READ UserVsAuthorisation Record for a SequenceNumber 
        //
        public void ReadUserVsAuthorizationSeqNumber(int InSeqNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[UserVsAuthorizers]"
               + " WHERE SeqNumber = @SeqNumber";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthorName = (string)rdr["AuthorName"];
                            TypeOfAuth = (int)rdr["TypeOfAuth"];
                           
                            DateOfInsert = (DateTime)rdr["DateOfInsert"];
                            DateOfClose = (DateTime)rdr["DateOfClose"];

                            OpenRecord = (bool)rdr["OpenRecord"];
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
        // READ UserVsAuthorisation Record 
        //
        public void ReadUserVsAuthorizationSpecific(string InUserId, string InAuthoriser)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[UserVsAuthorizers]"
               + " WHERE UserId = @UserId AND Authoriser = @Authoriser ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@Authoriser", InAuthoriser);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthorName = (string)rdr["AuthorName"];
                            TypeOfAuth = (int)rdr["TypeOfAuth"];

                            DateOfInsert = (DateTime)rdr["DateOfInsert"];
                            DateOfClose = (DateTime)rdr["DateOfClose"];

                            OpenRecord = (bool)rdr["OpenRecord"];
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

        // Insert UserVsAuthorisation Record 
        //
        public void InsertUserVsAuthorizationRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[UserVsAuthorizers]"
                    + "([UserId], [Authoriser], [AuthorName],  "
                    + " [TypeOfAuth], [DateOfInsert], [Operator] )"
                    + " VALUES (@UserId, @Authoriser, @AuthorName,"
                    + "          @TypeOfAuth, @DateOfInsert, @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                      
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@AuthorName", AuthorName);

                        cmd.Parameters.AddWithValue("@TypeOfAuth", TypeOfAuth);

                        cmd.Parameters.AddWithValue("@DateOfInsert", DateOfInsert);

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

                    CatchDetails(ex);
                }
        }
        //
        // UPDATE UserVsAuthorisation Record 
        // 
        public void UpdateUserVsAuthorisationRecord(int InSeqNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[UserVsAuthorizers] SET "
                            + " TypeOfAuth = @TypeOfAuth, "
                            + " DateOfClose = @DateOfClose, "
                            + " OpenRecord = @OpenRecord "
                            + " WHERE SeqNumber = @SeqNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        cmd.Parameters.AddWithValue("@TypeOfAuth", TypeOfAuth);

                        cmd.Parameters.AddWithValue("@DateOfClose", DateOfClose);

                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

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
        // UPDATE ALL UserVsAuthorisation Record - Open Record 
        // 
        public void UpdateUserVsAuthorisationRecordOpenRecord(string InAuthoriser, bool InOpenRecord)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[UserVsAuthorizers] SET "
                            + " OpenRecord = @OpenRecord "
                            + " WHERE Authoriser = @Authoriser ", conn)) 
                    {
                        cmd.Parameters.AddWithValue("@Authoriser", InAuthoriser);

                        cmd.Parameters.AddWithValue("@OpenRecord", InOpenRecord);

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
        // DELETE User vs Authorizer record 
        //
        public void DeleteUserAuthoriserRecord(string InUserId, string InAuthoriser)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[UserVsAuthorizers]"
                            + " WHERE UserId = @UserId AND Authoriser = @Authoriser ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@Authoriser", InAuthoriser);

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



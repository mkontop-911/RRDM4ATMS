using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMUserVsAuthorizers
    {
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

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //
        // READ UserVsAuthorisation Record for a SequenceNumber 
        //
        public void ReadUserVsAuthorizationSeqNumber(int InSeqNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UserVsAuthorization Class............. " + ex.Message;
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

            string SqlString = "SELECT *"
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UserVsAuthorization Class............. " + ex.Message;
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UserVsAuthorization Class............. " + ex.Message;
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UserVsAuthorization Update method Class............. " + ex.Message;
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UserVsAuthorization Update method Class............. " + ex.Message;
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UserVsAuthorization Class............. " + ex.Message;
                }

        }
    }
}

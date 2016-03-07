using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using RRDM4ATMs; 

namespace RRDM4ATMsWin
{
    class RRDMCaseNotes
    {
        //      
        //
        public int SeqNumber;

        public string Parameter3;
        public string Parameter4;

        public string UserId; 
        public string UserName; 
        public DateTime DateCreated;

        public string Notes; 

        public bool PrivateForUser; 

        public bool OpenRecord;
        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public string AttachmentPath;

        public int TotalNotes; 

        string SortType;

        string SqlString; 

        public ArrayList NoteControlsArray = new ArrayList();

        //string WPrivate = ""; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //
        // READ CaseNotes  for a SequenceNumber 
        //
 
        public void ReadCaseNotesSpecific(int InSeqNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[CaseNotes]"
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

                            Parameter3 = (string)rdr["Parameter3"];
                            Parameter4 = (string)rdr["Parameter4"];

                            DateCreated = (DateTime)rdr["DateCreated"];

                            UserId = (string)rdr["UserId"];
                            UserName = (string)rdr["UserName"];
                            Notes = (string)rdr["Notes"];
                            PrivateForUser = (bool)rdr["PrivateForUser"];

                            AttachmentPath = (string)rdr["AttachmentPath"];

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
                    ErrorOutput = "An error occured in CaseNotes Class............. " + ex.Message;
                }
        }

        //  
        // For a particular case read the notes and present them
        // Create a String throught a the StringBuilder
        // Read Notes sequentially and insert data in string 
        public void ReadAllNotes(string InParameter4, string InUserId, string InOrder, string SearchP4) 
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";  
       
            TotalNotes = 0 ; 

            if (InOrder == "Descending")
            {
                SortType = "DESC ";  
            }

            if (InOrder == "Ascending")
            {
                SortType = "ASC ";  
            }

            StringBuilder NotesString = new StringBuilder();

      
            if (SearchP4 == "")
            {
                SqlString = "SELECT *"
                                   + " FROM [ATMS].[dbo].[CaseNotes] "
                                   + " WHERE (Parameter4=@Parameter4 AND PrivateForUser = 0) "
                                              + " OR (Parameter4=@Parameter4 AND PrivateForUser = 1 AND UserId = @UserId) "
                                   + " ORDER BY SeqNumber " + SortType; 
            }
            if (SearchP4 != "")
            {
                SqlString = "SELECT *"
                                   + " FROM [ATMS].[dbo].[CaseNotes] "
                                   + " WHERE Parameter4 LIKE '%" +SearchP4 + "%'"
                                   + " ORDER BY SeqNumber " + SortType;
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Parameter4", InParameter4);
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        // Read table 
                       

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalNotes = TotalNotes + 1; 

                            SeqNumber = (int)rdr["SeqNumber"];

                            Parameter3 = (string)rdr["Parameter3"];
                            Parameter4 = (string)rdr["Parameter4"];

                            DateCreated = (DateTime)rdr["DateCreated"];

                            UserId = (string)rdr["UserId"];
                            UserName = (string)rdr["UserName"];
                            Notes = (string)rdr["Notes"];
                            PrivateForUser = (bool)rdr["PrivateForUser"];
                            AttachmentPath = (string)rdr["AttachmentPath"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

                            //if (PrivateForUser == true)
                            //{
                            //    WPrivate = "  PRIVATE"; 
                            //}
                            //else
                            //{
                            //    WPrivate = ""; 
                            //}

                            NoteControlsArray.Add(new Control197(SeqNumber.ToString(), Parameter3, UserId, UserName, 
                                                                Notes, DateCreated.ToString(),AttachmentPath));
                            
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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

                }
        }

        // Insert CaseNotes Record 
        //
        public void InsertCaseNotesRecord(string InParameter4)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[CaseNotes]"
                    + "( [Parameter3], [Parameter4], "
                    + " [UserId], [UserName], [DateCreated], [Notes], [PrivateForUser], [Operator],[AttachmentPath] )"
                    + " VALUES (@Parameter3, @Parameter4,"
                    + " @UserId, @UserName, @DateCreated, @Notes, @PrivateForUser, @Operator,@AttachmentPath )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        TotalNotes = TotalNotes + 1;

                        cmd.Parameters.AddWithValue("@Parameter3", Parameter3);
                        cmd.Parameters.AddWithValue("@Parameter4", InParameter4);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@UserName", UserName);

                        cmd.Parameters.AddWithValue("@DateCreated", DateCreated);

                        cmd.Parameters.AddWithValue("@Notes", Notes);

                        cmd.Parameters.AddWithValue("@PrivateForUser", PrivateForUser);
                        
                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        cmd.Parameters.AddWithValue("@AttachmentPath", AttachmentPath);

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
                    ErrorOutput = "An error occured in CaseNotes Class............. " + ex.Message;
                }
        }
        //
        // UPDATE CaseNotes Record 
        // 
        public void UpdateCaseNotesRecord(int InSeqNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[CaseNotes] SET "
                            + " Notes = @Notes,"
                            + " PrivateForUser = @PrivateForUser,"
                            + " AttachmentPath = @AttachmentPath,"
                            + " OpenRecord = @OpenRecord "
                            + " WHERE SeqNumber = @SeqNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        cmd.Parameters.AddWithValue("@Notes", Notes);

                        cmd.Parameters.AddWithValue("@PrivateForUser", PrivateForUser);

                        cmd.Parameters.AddWithValue("@AttachmentPath", AttachmentPath);

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
                    ErrorOutput = "An error occured in Authorization Update method Class............. " + ex.Message;
                }
        }
        // READ LAST CaseNotes FOR These parameters 
        public void FindCaseNotesLastNo(string InParameter4)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM [ATMS].[dbo].[CaseNotes]"
                   + " WHERE SeqNumber = (SELECT MAX(SeqNumber) FROM [ATMS].[dbo].[CaseNotes])"
                   + " AND Parameter4 = @Parameter4 ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Parameter4", InParameter4);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Parameter3 = (string)rdr["Parameter3"];
                            Parameter4 = (string)rdr["Parameter4"];

                            DateCreated = (DateTime)rdr["DateCreated"];

                            UserId = (string)rdr["UserId"];
                            UserName = (string)rdr["UserName"];
                            Notes = (string)rdr["Notes"];
                            PrivateForUser = (bool)rdr["PrivateForUser"];

                            AttachmentPath = (string)rdr["AttachmentPath"];

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
                    ErrorOutput = "An error occured in Authorization Update method Class............. " + ex.Message;

                }
        }

        //
        // DELETE CaseNotes record 
        //
        public void DeleteCaseNotesRecord(int InSeqNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[CaseNotes] "
                            + " WHERE SeqNumber =  @SeqNumber ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        //rows number of record got updated
                        TotalNotes = TotalNotes - 1 ;
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
                    ErrorOutput = "An error occured in CaseNotes Class............. " + ex.Message;
                }

        }

        /*
        //
        // READ Authorization Records to find if this user has to Authorize 
        //
        public void CheckOutstandingAuthorizationsStage1(string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[CaseNotes]"
               + " WHERE Parameter2 = @InSignedId AND Stage = 1";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@InSignedId", InSignedId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Parameter1 = (string)rdr["Parameter1"];
                            Parameter2 = (string)rdr["Parameter2"];
                            Parameter3 = (string)rdr["Parameter3"];
                            Parameter4 = (string)rdr["Parameter4"];

                            DateCreated = (DateTime)rdr["DateCreated"];

                            UserId = (string)rdr["UserId"];
                            UserName = (string)rdr["UserName"];
                            Notes = (string)rdr["Notes"];

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
                    ErrorOutput = "An error occured in CaseNotes Class............. " + ex.Message;
                }
        }

        //
        // READ Authorization Records to find if this user has to Authorize 
        //
        public void CheckOutstandingAuthorizationsStage3(string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[CaseNotes]"
               + " WHERE Parameter1 = @InSignedId AND Stage = 3";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@InSignedId", InSignedId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Parameter1 = (string)rdr["Parameter1"];
                            Parameter2 = (string)rdr["Parameter2"];
                            Parameter3 = (string)rdr["Parameter3"];
                            Parameter4 = (string)rdr["Parameter4"];

                            DateCreated = (DateTime)rdr["DateCreated"];

                            UserId = (string)rdr["UserId"];
                            UserName = (string)rdr["UserName"];
                            Notes = (string)rdr["Notes"];

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
                    ErrorOutput = "An error occured in CaseNotes Class............. " + ex.Message;
                }
        }
         */
      
    }
}
/*
//
        // READ Authorization Record for a Dispute and a transaction 
        //
        public void ReadAuthorizationForDisputeAndTransaction(int InDisputeNumber, int InDisputeTransaction)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[CaseNotes]"
               + " WHERE DisputeNumber = @DisputeNumber AND DisputeTransaction = @DisputeTransaction ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DisputeNumber", InDisputeNumber);
                        cmd.Parameters.AddWithValue("@DisputeTransaction", InDisputeTransaction);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Parameter1 = (string)rdr["Parameter1"];
                            Parameter2 = (string)rdr["Parameter2"];
                            Parameter3 = (string)rdr["Parameter3"];
                            Parameter4 = (string)rdr["Parameter4"];

                            DateCreated = (DateTime)rdr["DateCreated"];

                            UserId = (string)rdr["UserId"];
                            UserName = (string)rdr["UserName"];
                            Notes = (string)rdr["Notes"];

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
                    ErrorOutput = "An error occured in CaseNotes Class............. " + ex.Message;
                }
        }
*/
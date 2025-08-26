using System;
using System.Text;
using System.Collections;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public class RRDMCaseNotes
    {
        //      
        //
        public int SeqNumber;

        public string Parameter2;
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

        // Define the data table 
        public DataTable NotesDataTable = new DataTable();
        public int TotalSelected;

        public ArrayList NoteControlsArray = new ArrayList();

        //string WPrivate = ""; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // Reader Fields
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNumber = (int)rdr["SeqNumber"];

            Parameter2 = (string)rdr["Parameter2"];
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

                            ReaderFields(rdr);

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
        // READ NOTES AND FILL UP Table
        //
        public void ReadAllCaseNotesAndFillTableBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NotesDataTable = new DataTable();
            NotesDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            NotesDataTable.Columns.Add("SeqNo", typeof(int));
            NotesDataTable.Columns.Add("Parameter3", typeof(string));
            NotesDataTable.Columns.Add("Parameter4", typeof(string));
            NotesDataTable.Columns.Add("DateCreated", typeof(DateTime));
            NotesDataTable.Columns.Add("UserId", typeof(string));

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[CaseNotes]"
               + InSelectionCriteria ;

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
                            TotalSelected = TotalSelected + 1;

                            RecordFound = true;

                            ReaderFields(rdr);
                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = NotesDataTable.NewRow();

                            RowSelected["SeqNo"] = SeqNumber;
                            RowSelected["Parameter3"] = Parameter3;
                            RowSelected["Parameter4"] = Parameter4;
                            RowSelected["DateCreated"] = DateCreated;
                            RowSelected["UserId"] = UserId;

                            // ADD ROW
                            NotesDataTable.Rows.Add(RowSelected);

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

                            ReaderFields(rdr);

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

                    CatchDetails(ex);

                }
        }

        // Insert CaseNotes Record 
        //
        public void InsertCaseNotesRecord(string InParameter4)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[CaseNotes]"
                    + "( [Parameter2], [Parameter3], [Parameter4], "
                    + " [UserId], [UserName], [DateCreated], [Notes], [PrivateForUser], [Operator],[AttachmentPath] )"
                    + " VALUES (@Parameter2, @Parameter3, @Parameter4,"
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

                        cmd.Parameters.AddWithValue("@Parameter2", Parameter2);
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


                    CatchDetails(ex);
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


                    CatchDetails(ex);
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

                            ReaderFields(rdr);

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
        // DELETE CaseNotes record by SeqNumber 
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


                    CatchDetails(ex);
                }

        }

        //
        // DELETE CaseNotes record by SeqNumber 
        //
        public void DeleteCaseNotesRecordByParameter4(string InParameter4)
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
                            + " WHERE Parameter4 =  @Parameter4 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Parameter4", InParameter4);
                        
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

        // Catch Details
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

            //  Environment.Exit(0);
        }


    

    }
}

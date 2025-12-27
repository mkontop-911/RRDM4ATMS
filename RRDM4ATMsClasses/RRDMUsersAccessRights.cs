using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;


namespace RRDM4ATMs
{
    public class RRDMUsersAccessRights : Logger
    {
        public RRDMUsersAccessRights() : base() { }
        // Declarations 


        public int SeqNo;
        public string MainFormId;
        public string PanelName;
        public string ButtonName;
        public string ButtonText;

        public bool SecLevel02;
        public bool SecLevel03;
        public bool SecLevel04;

        public bool SecLevel05;
        public bool SecLevel06;
        public bool SecLevel07;

        public bool SecLevel08;
        public bool SecLevel09;
        public bool SecLevel10;
        public bool SecLevel11;
        public bool SecLevel12;
        public bool SecLevel13;
        public bool SecLevel14;
        public bool IsUpdated; 
        public string Operator; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public DataTable UsersAccessTable = new DataTable();

        public int TotalSelected;
        string SqlString; // Do not delete

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Fields of record 
        private void FieldsForAccessControl(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            MainFormId = (string)rdr["MainFormId"];
            PanelName = (string)rdr["PanelName"];
            ButtonName = (string)rdr["ButtonName"];
            ButtonText = (string)rdr["ButtonText"];

            SecLevel02 = (bool)rdr["SecLevel02"];
            SecLevel03 = (bool)rdr["SecLevel03"];
            SecLevel04 = (bool)rdr["SecLevel04"];
            SecLevel05 = (bool)rdr["SecLevel05"];
            SecLevel06 = (bool)rdr["SecLevel06"];
            SecLevel07 = (bool)rdr["SecLevel07"];
            SecLevel08 = (bool)rdr["SecLevel08"];
            SecLevel09 = (bool)rdr["SecLevel09"];
            SecLevel10 = (bool)rdr["SecLevel10"];
            SecLevel11 = (bool)rdr["SecLevel11"];
            SecLevel12 = (bool)rdr["SecLevel12"];
            SecLevel13 = (bool)rdr["SecLevel13"];
            SecLevel14 = (bool)rdr["SecLevel14"];

            IsUpdated = (bool)rdr["IsUpdated"];

            Operator = (string)rdr["Operator"];
        }

        // READ Record 
        public void ReadUserAccessRightsFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            UsersAccessTable = new DataTable();
            UsersAccessTable.Clear();

            TotalSelected = 0;
       
            // DATA TABLE ROWS DEFINITION 
            UsersAccessTable.Columns.Add("SeqNo", typeof(int));
            UsersAccessTable.Columns.Add("MainFormId", typeof(string));
            UsersAccessTable.Columns.Add("Function Category", typeof(string));
            UsersAccessTable.Columns.Add("Function Descr", typeof(string));

            UsersAccessTable.Columns.Add("Role 02", typeof(bool));
            UsersAccessTable.Columns.Add("Role 03", typeof(bool));
            UsersAccessTable.Columns.Add("Role 04", typeof(bool));
            UsersAccessTable.Columns.Add("Role 05", typeof(bool));
            UsersAccessTable.Columns.Add("Role 06", typeof(bool));
            UsersAccessTable.Columns.Add("Role 07", typeof(bool));
            UsersAccessTable.Columns.Add("Role 08", typeof(bool));
            UsersAccessTable.Columns.Add("Role 09", typeof(bool));
            UsersAccessTable.Columns.Add("Role 10", typeof(bool));
            UsersAccessTable.Columns.Add("Role 11", typeof(bool));
            UsersAccessTable.Columns.Add("Role 12", typeof(bool));
            UsersAccessTable.Columns.Add("Role 13", typeof(bool));
            UsersAccessTable.Columns.Add("Role 14", typeof(bool));

            SqlString = "SELECT *"
                         + " FROM [ATMS].[dbo].[UsersAccessRights] "
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

                            // Read USER Details
                            FieldsForAccessControl(rdr);

                            //
                            //FILL IN TABLE
                            //

                            DataRow RowSelected = UsersAccessTable.NewRow();
                           
                            RowSelected["SeqNo"] = SeqNo;

                            RowSelected["MainFormId"] = MainFormId;
                            RowSelected["Function Category"] = PanelName;
                            RowSelected["Function Descr"] = ButtonText;
                            RowSelected["Role 02"] = SecLevel02;
                            RowSelected["Role 03"] = SecLevel03;
                            RowSelected["Role 04"] = SecLevel04;
                            RowSelected["Role 05"] = SecLevel05;
                            RowSelected["Role 06"] = SecLevel06;
                            RowSelected["Role 07"] = SecLevel07;
                            RowSelected["Role 08"] = SecLevel08;
                            RowSelected["Role 09"] = SecLevel09;
                            RowSelected["Role 10"] = SecLevel10;
                            RowSelected["Role 11"] = SecLevel11;
                            RowSelected["Role 12"] = SecLevel12;
                            RowSelected["Role 13"] = SecLevel13;
                            RowSelected["Role 14"] = SecLevel14;

                            // ADD ROW
                            UsersAccessTable.Rows.Add(RowSelected);

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


        // READ Record 
        public void ReadUserAccessRightsBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
          
            SqlString = "SELECT *"
                         + " FROM [ATMS].[dbo].[UsersAccessRights] "
                         + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@UserId", InUserId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details
                            FieldsForAccessControl(rdr);

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


        // Insert Access Record 
        //
        public int InsertAccessRecord(string InButtonName)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT [ATMS].[dbo].[UsersAccessRights]"
                + " ([MainFormId],[PanelName],[ButtonName],[ButtonText],"
                + " [SecLevel02],"
                + " [SecLevel03],"
                + " [SecLevel04],"
                + " [SecLevel05],"
                + " [SecLevel06],"
                + " [SecLevel07],"
                + " [SecLevel08],"
                + " [SecLevel09],"
                  + " [SecLevel10],"
                    + " [SecLevel11],"
                      + " [SecLevel12],"
                        + " [SecLevel13],"
                          + " [SecLevel14],"
                           + " [IsUpdated],"
                + " [Operator] )"
                + " VALUES (@MainFormId,@PanelName,@ButtonName,@ButtonText,"
                + " @SecLevel02,"
                + " @SecLevel03,"
                + " @SecLevel04,"
                + " @SecLevel05,"
                + " @SecLevel06,"
                + " @SecLevel07,"
                + " @SecLevel08,"
                + " @SecLevel09,"
                + " @SecLevel10,"
                + " @SecLevel11,"
                + " @SecLevel12,"
                + " @SecLevel13,"
                + " @SecLevel14,"
                + " @IsUpdated,"
                + " @Operator )"
                +" SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@MainFormId", MainFormId);

                        cmd.Parameters.AddWithValue("@PanelName", PanelName);
                        cmd.Parameters.AddWithValue("@ButtonName", InButtonName);

                        cmd.Parameters.AddWithValue("@ButtonText", ButtonText);

                        cmd.Parameters.AddWithValue("@SecLevel02", SecLevel02);
                        cmd.Parameters.AddWithValue("@SecLevel03", SecLevel03);
                        cmd.Parameters.AddWithValue("@SecLevel04", SecLevel04);
                        cmd.Parameters.AddWithValue("@SecLevel05", SecLevel05);
                        cmd.Parameters.AddWithValue("@SecLevel06", SecLevel06);
                        cmd.Parameters.AddWithValue("@SecLevel07", SecLevel07);
                        cmd.Parameters.AddWithValue("@SecLevel08", SecLevel08);
                        cmd.Parameters.AddWithValue("@SecLevel09", SecLevel09);
                        cmd.Parameters.AddWithValue("@SecLevel10", SecLevel10);
                        cmd.Parameters.AddWithValue("@SecLevel11", SecLevel11);
                        cmd.Parameters.AddWithValue("@SecLevel12", SecLevel12);
                        cmd.Parameters.AddWithValue("@SecLevel13", SecLevel13);
                        cmd.Parameters.AddWithValue("@SecLevel14", SecLevel14);
                        cmd.Parameters.AddWithValue("@IsUpdated", IsUpdated);

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


        // UPDATE Access Rights Table
        //
        public void UpdateUsersAccessRights(int InSeqNo)
        {
            int rows; 

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[UsersAccessRights] SET "
                            + " MainFormId = @MainFormId, "
                            + " PanelName = @PanelName, "
                            + " ButtonName = @ButtonName, "
                            + " ButtonText = @ButtonText, "
                            + " SecLevel02 = @SecLevel02, "
                            + " SecLevel03 = @SecLevel03, "
                            + " SecLevel04 = @SecLevel04, "
                            + " SecLevel05 = @SecLevel05, "
                            + " SecLevel06 = @SecLevel06, "
                            + " SecLevel07 = @SecLevel07, "
                            + " SecLevel08 = @SecLevel08, "
                            + " SecLevel09 = @SecLevel09, "
                            + " SecLevel10 = @SecLevel10, "
                            + " SecLevel11 = @SecLevel11, "
                            + " SecLevel12 = @SecLevel12, "
                            + " SecLevel13 = @SecLevel13, "
                            + " SecLevel14 = @SecLevel14, "
                            + " IsUpdated = @IsUpdated  "
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@MainFormId", MainFormId);
                     
                        cmd.Parameters.AddWithValue("@PanelName", PanelName);
                        cmd.Parameters.AddWithValue("@ButtonName", ButtonName);

                        cmd.Parameters.AddWithValue("@ButtonText", ButtonText);

                        cmd.Parameters.AddWithValue("@SecLevel02", SecLevel02);
                        cmd.Parameters.AddWithValue("@SecLevel03", SecLevel03);
                        cmd.Parameters.AddWithValue("@SecLevel04", SecLevel04);
                        cmd.Parameters.AddWithValue("@SecLevel05", SecLevel05);
                        cmd.Parameters.AddWithValue("@SecLevel06", SecLevel06);
                        cmd.Parameters.AddWithValue("@SecLevel07", SecLevel07);
                        cmd.Parameters.AddWithValue("@SecLevel08", SecLevel08);
                        cmd.Parameters.AddWithValue("@SecLevel09", SecLevel09);
                        cmd.Parameters.AddWithValue("@SecLevel10", SecLevel10);
                        cmd.Parameters.AddWithValue("@SecLevel11", SecLevel11);
                        cmd.Parameters.AddWithValue("@SecLevel12", SecLevel12);
                        cmd.Parameters.AddWithValue("@SecLevel13", SecLevel13);
                        cmd.Parameters.AddWithValue("@SecLevel14", SecLevel14);
                        cmd.Parameters.AddWithValue("@IsUpdated", IsUpdated);
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


        // UPDATE Access Rights with is Updated
        //
        public void UpdateUsersAccessRightsInitialiseWithIsUpdated(string InMainFormId)
        {
            int rows;

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[UsersAccessRights] SET "
                            + " IsUpdated = @IsUpdated  "
                            + " WHERE MainFormId= @MainFormId ", conn))
                    {
                 
                        cmd.Parameters.AddWithValue("@MainFormId", InMainFormId);

                        cmd.Parameters.AddWithValue("@IsUpdated", IsUpdated);
                     
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

     
        // DELETE UsersAccessRights
        //
        public void DeleteUsersAccessRights(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[UsersAccessRights] "
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



using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMUsers_Applications_Roles : Logger
    {
        public RRDMUsers_Applications_Roles() : base() { }

        public int SeqNo;

        public string UserId;
        public string Application; //  Identity 

        public string SecLevel;

        public string RoleName;

        public bool Authoriser;
        public bool DisputeOfficer;
        public bool ReconcOfficer;
        public bool ReconcMgr;

        public bool MsgsAllowed;

        public DateTime UpdatedDate;

        public string Operator;

        /// <summary>
        /// 
        /// </summary>
        /// 

        // Define the data table 
        public DataTable UsersVsApplicationsVsRolesDataTable = new DataTable();

        public int TotalSelected;

        /// <summary>
        /// 
        /// </summary>

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        //
        // Read Fields 
        // 
        private void UsersVsApplicationsVsRolesReaderFields(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            UserId = (string)rdr["UserId"];

            Application = (string)rdr["Application"];
            SecLevel = (string)rdr["SecLevel"];

            RoleName = (string)rdr["RoleName"];

            Authoriser = (bool)rdr["Authoriser"];
            DisputeOfficer = (bool)rdr["DisputeOfficer"];
            ReconcOfficer = (bool)rdr["ReconcOfficer"];
            ReconcMgr = (bool)rdr["ReconcMgr"];
            MsgsAllowed = (bool)rdr["MsgsAllowed"];

            UpdatedDate = (DateTime)rdr["UpdatedDate"];

            Operator = (string)rdr["Operator"];

        }

        //
        // READ UsersVsApplicationsVsRoles  and created table as per request.
        //
        public void ReadUsersVsApplicationsVsRolesAndFillTable(string InSelectionCriteria, int InMode, string InBranch)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            RRDMUsersRecords Us = new RRDMUsersRecords();
            // InMode = 1 ... No special Selection
            // InMode = 2 then we want authorisers of User Branch 
            // InMode = 3 then we want Dispute Officers
            // InMode = 4 then we want Reconciliation Officers

            ClearDefineTable();

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[UsersVsApplicationsVsRoles]"
                 + InSelectionCriteria
                 + "  Order by UserId "
                 ;

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
                            // read one by one 
                            UsersVsApplicationsVsRolesReaderFields(rdr);

                            if (InMode == 1)
                            {

                                if (InBranch != "")
                                {
                                    Us.ReadUsersRecord(UserId);
                                    if (Us.Branch == InBranch)
                                    {
                                        AddLineToTable();
                                    }

                                }
                                else
                                {
                                    // Add All Authorisers for this Application 
                                    AddLineToTable();
                                }
                            }

                            if (InMode == 2)
                            {
                                if (InBranch != "")
                                {
                                    Us.ReadUsersRecord(UserId);
                                    if (Us.Branch == InBranch)
                                    {
                                        AddLineToTable();
                                    }

                                }
                                else
                                {
                                    // Add All Authorisers for this Application 
                                    AddLineToTable();
                                }
                            }

                            if (InMode == 3)
                            {

                                // Add All Dispute Officers for this Application 
                                AddLineToTable();

                            }

                            if (InMode == 4)
                            {

                                // Add All Reconciliation Officers for this Application 
                                AddLineToTable();

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


        // Clear Define Table
        private void ClearDefineTable()
        {
            UsersVsApplicationsVsRolesDataTable = new DataTable();
            UsersVsApplicationsVsRolesDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            UsersVsApplicationsVsRolesDataTable.Columns.Add("SeqNo", typeof(int));
            UsersVsApplicationsVsRolesDataTable.Columns.Add("UserId", typeof(string));
            
            UsersVsApplicationsVsRolesDataTable.Columns.Add("Application", typeof(string));
            UsersVsApplicationsVsRolesDataTable.Columns.Add("SecLevel", typeof(string));
            UsersVsApplicationsVsRolesDataTable.Columns.Add("RoleName", typeof(string));

            UsersVsApplicationsVsRolesDataTable.Columns.Add("Authoriser", typeof(bool));
            UsersVsApplicationsVsRolesDataTable.Columns.Add("DisputeOfficer", typeof(bool));
            UsersVsApplicationsVsRolesDataTable.Columns.Add("ReconcOfficer", typeof(bool));
            UsersVsApplicationsVsRolesDataTable.Columns.Add("ReconcMgr", typeof(bool));
            UsersVsApplicationsVsRolesDataTable.Columns.Add("UpdatedDate", typeof(DateTime));

        }

        // ADD LINE TO TABLE

        private void AddLineToTable()
        {
            //
            // Fill In Table
            //

            DataRow RowSelected = UsersVsApplicationsVsRolesDataTable.NewRow();

            RowSelected["SeqNo"] = SeqNo;

            RowSelected["UserId"] = UserId;
            
            RowSelected["Application"] = Application;

            RowSelected["SecLevel"] = SecLevel;
            RowSelected["RoleName"] = RoleName;

            RowSelected["Authoriser"] = Authoriser;
            RowSelected["DisputeOfficer"] = DisputeOfficer;
            RowSelected["ReconcOfficer"] = ReconcOfficer;
            RowSelected["ReconcMgr"] = ReconcMgr;

            RowSelected["UpdatedDate"] = UpdatedDate;

            // ADD ROW
            UsersVsApplicationsVsRolesDataTable.Rows.Add(RowSelected);
        }


        //
        // READ UsersVsApplicationsVsRoles by SeqNo
        //
        public void ReadUsersVsApplicationsVsRolesBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM UsersVsApplicationsVsRoles"
               + " WHERE SeqNo=@SeqNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            UsersVsApplicationsVsRolesReaderFields(rdr);

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
        // READ UsersVsApplicationsVsRoles by Application
        //
        public void ReadUsersVsApplicationsVsRolesByApplication(string InUserId, string InApplication)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM UsersVsApplicationsVsRoles "
               + " WHERE UserId = @UserId AND Application=@Application";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@Application", InApplication);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            
                            UsersVsApplicationsVsRolesReaderFields(rdr);

                            RecordFound = true;

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
        // READ UsersVsApplicationsVsRoles by User Id
        //
        public void ReadUsersVsApplicationsVsRolesByUser(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
               + " FROM UsersVsApplicationsVsRoles "
               + " WHERE UserId = @UserId ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        //cmd.Parameters.AddWithValue("@Application", InApplication);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            UsersVsApplicationsVsRolesReaderFields(rdr);

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
        public bool W_Admin = false;
        public bool W_Controller = false;
        public bool W_Authoriser = false;
        public bool W_DisputeOfficer = false;
        public bool W_ReconcOfficer = false;
        public bool W_ReconcMgr = false;
        public int W_Counter; 

        // GET THE ACCESS RIGHTS PER USER 
        public void ReadUsersVsApplicationsVsRolesByUserAndAccessRights(string InUserId)
        {
            W_Counter = 0; 
            string WSecLevel = "03";
            ReadUsersVsApplicationsVsRolesByUserAndRole(InUserId, WSecLevel);

            if (Authoriser == true)
            {
                W_Authoriser = true;
                W_Counter++; 
            }
            if (DisputeOfficer == true)
            {
                W_DisputeOfficer = true;
                W_Counter++;
            }
            if (ReconcOfficer == true)
            {
                W_ReconcOfficer = true;
                W_Counter++;
            }
            if (ReconcMgr == true)
            {
                W_ReconcMgr = true;
                W_Counter++;
            }
            WSecLevel = "07";
            ReadUsersVsApplicationsVsRolesByUserAndRole(InUserId, WSecLevel);
            if (Authoriser == true)
            {
                W_Authoriser = true;
                W_Counter++;
            }
            WSecLevel = "08"; // Controller 
            ReadUsersVsApplicationsVsRolesByUserAndRole(InUserId, WSecLevel);
            if (RecordFound == true)
            {
                W_Controller = true;
                W_Counter++;
            }
            WSecLevel = "10"; // Admin
            ReadUsersVsApplicationsVsRolesByUserAndRole(InUserId, WSecLevel);
            if (RecordFound == true)
            {
                W_Admin = true;
                W_Counter++;
            }

        }

        //
        // READ UsersVsApplicationsVsRoles by User Id
        //
        public void ReadUsersVsApplicationsVsRolesByUserAndRole(string InUserId, string InSecLevel)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
               + " FROM UsersVsApplicationsVsRoles "
               + " WHERE UserId = @UserId AND SecLevel = @SecLevel ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@SecLevel", InSecLevel);
                        //cmd.Parameters.AddWithValue("@Application", InApplication);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            UsersVsApplicationsVsRolesReaderFields(rdr);

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
        // READ UsersVsApplicationsVsRoles by Application
        // READ ALL DISPUTE OFFICERS AND FIND THE ONE With minimum 
        // 
        public string ReadUsersVsApplicationsVsRolesByApplication_For_Disputes_Min_User(string InApplication)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

            int NumberOfDisp = 0;
            string MinUser = ""; 
            int MinDisputes = 0;

            bool FirstUser = true; 

            string SqlString = "SELECT * "
               + " FROM UsersVsApplicationsVsRoles "
               + " Where SecLevel = '04' and Application = @Application "; 
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@Application", InApplication);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            UsersVsApplicationsVsRolesReaderFields(rdr);

                            NumberOfDisp = Di.FindSumByOwner(UserId); 

                            if (FirstUser == true)
                            {
                                MinDisputes = NumberOfDisp;
                                MinUser = UserId;

                                FirstUser = false; 
                            }
                            else
                            {
                                if (MinDisputes > NumberOfDisp)
                                {
                                    MinDisputes = NumberOfDisp;
                                    MinUser = UserId;
                                }
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
            return MinUser; 
        }

        //
        // Insert UsersVsApplicationsVsRoles
        //
        //int rows;
        public int InsertUsersVsApplicationsVsRoles(string InApplication)
        {
            //public bool MsgsAllowed;

            string cmdinsert = "INSERT INTO [UsersVsApplicationsVsRoles] "
                + " ( [UserId],"
                + "[Application],[SecLevel],"
                + "[RoleName],"
                 + "[Authoriser],"
                  + "[DisputeOfficer],"
                   + "[ReconcOfficer],"
                    + "[ReconcMgr],"
                    + "[MsgsAllowed],"
                 + "[UpdatedDate],"
                + "[Operator])"
                + " VALUES (@UserId,"
                + "@Application,@SecLevel,"
                 + "@RoleName, "
                   + "@Authoriser, "
                     + "@DisputeOfficer, "
                       + "@ReconcOfficer, "
                         + "@ReconcMgr, "
                           + "@MsgsAllowed, "
                  + "@UpdatedDate,"
                + "@Operator)"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@Application", Application);
                        cmd.Parameters.AddWithValue("@SecLevel", SecLevel);

                        cmd.Parameters.AddWithValue("@RoleName", RoleName);

                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@DisputeOfficer", DisputeOfficer);
                        cmd.Parameters.AddWithValue("@ReconcOfficer", ReconcOfficer);
                        cmd.Parameters.AddWithValue("@ReconcMgr", ReconcMgr);

                        cmd.Parameters.AddWithValue("@MsgsAllowed", MsgsAllowed);

                        cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        SeqNo = cmd.ExecuteNonQuery();
                        //if (rows > 0) exception = " A NEW ATM WAS CREADED - ITs ATM No iS GIVEN AUTOMATICALLY. GO TO TABLE TO SEE IT";
                        //else exception = " Nothing WAS UPDATED ";

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

        // UPDATE 

        public void UpdateUsersVsApplicationsVsRoles(int InSeqNo)
        {
            int rows;

            string strUpdate = "UPDATE UsersVsApplicationsVsRoles SET"
                + " UserId=@UserId,"
                + "Application=@Application, SecLevel=@SecLevel,"
                + "RoleName=@RoleName, "
                  + "Authoriser=@Authoriser, "
                    + "DisputeOfficer=@DisputeOfficer, "
                      + "ReconcOfficer=@ReconcOfficer, "
                        + "ReconcMgr=@ReconcMgr, "
                          + "MsgsAllowed=@MsgsAllowed, "
                + "UpdatedDate=@UpdatedDate "
                + " WHERE SeqNo=@SeqNo ";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(strUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@Application", Application);
                        cmd.Parameters.AddWithValue("@SecLevel", SecLevel);

                        cmd.Parameters.AddWithValue("@RoleName", RoleName);

                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@DisputeOfficer", DisputeOfficer);
                        cmd.Parameters.AddWithValue("@ReconcOfficer", ReconcOfficer);
                        cmd.Parameters.AddWithValue("@ReconcMgr", ReconcMgr);

                        cmd.Parameters.AddWithValue("@MsgsAllowed", MsgsAllowed);

                        cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);

                        rows = cmd.ExecuteNonQuery();

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
        // DELETE UsersVsApplicationsVsRoles by SeqNo
        // DELETE ALL RELATIONS 
        //
        public void DeleteUsersVsApplicationsVsRolesBySeqNo(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    // ATM FIELDS 
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM dbo.UsersVsApplicationsVsRoles "
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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

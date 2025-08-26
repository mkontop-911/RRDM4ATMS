using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

//using RRDMEncrypt;

namespace RRDM4ATMs
{
    public class RRDMUsersRecords : Logger
    {
        public RRDMUsersRecords() : base() { }
        // Declarations 

        // Users Table 
        public int SeqNo;
        public string UserId;
        public string UserName;

        public string Culture;
        public string BankId;
        public string Branch;
        public string UserType;
        //    public string RoleNm;
        //  public string SecLevel;

        public string CitId;
        public DateTime DateTimeOpen;

        public string email;
        public string MobileNo;
        public bool Authoriser; 

        public bool UserInactive;
        public bool UserOnLeave;
        public string PassWord;
        public DateTime DtChanged;
        public DateTime DtToBeChanged;
        public DateTime MaxDateTmForTempPassword;
        public bool ForceChangePassword;
        public string Operator;

        // Maker and Checker

        public string ChangedByWhatMaker;
        public DateTime DtTimeOfChange;

        public bool Is_New_User;
        public bool HadToMakeInactive;
        public bool HadToUndoInactive;
        public bool Is_NewAccessRights;
        public bool Is_NewCategory;
        public bool Is_NewAuthoriser;

        public string ApprovedByWhatChecker;

        public string ApproveOR_Reject;

        public DateTime DtTimeOfApproving;

        public bool Is_Approved;

        // USER VERSUS ATM OR GROUP for REPL or RECONC

        //  public string GeneralUsedComment;
        //  public bool EnableInfo;

        public string WPassword;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data tables 
        public DataTable DisputeUsersSelected = new DataTable();

        public DataTable AuthorisersUsersSelected = new DataTable();

        public DataTable ReconcUsersSelected = new DataTable();

        public DataTable UsersInDataTable = new DataTable();

        public DataTable UserHistoryDataTable = new DataTable();

        public int TotalSelected;
        string SqlString; // Do not delete

        //  RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMMatchingCategories Rc = new RRDMMatchingCategories();

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // READ USER FIELDS
        private void ReadUserFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            UserId = (string)rdr["UserId"];
            UserName = (string)rdr["UserName"];

            Culture = rdr["Culture"].ToString();

            BankId = rdr["BankId"].ToString();
            Branch = rdr["Branch"].ToString();

            UserType = rdr["UserType"].ToString();

            CitId = (string)rdr["CitId"];

            DateTimeOpen = (DateTime)rdr["DateTimeOpen"];

            email = rdr["email"].ToString();
            MobileNo = rdr["MobileNo"].ToString();
          
            Authoriser = (bool)rdr["Authoriser"];

            UserInactive = (bool)rdr["UserInactive"];
            UserOnLeave = (bool)rdr["UserOnLeave"];

            PassWord = rdr["PassWord"].ToString();

            DtChanged = (DateTime)rdr["DtChanged"];
            DtToBeChanged = (DateTime)rdr["DtToBeChanged"];

            MaxDateTmForTempPassword= (DateTime)rdr["MaxDateTmForTempPassword"];

            ForceChangePassword = (bool)rdr["ForceChangePassword"];

            Operator = (string)rdr["Operator"];
        }

        // READ USER FIELDS
        private void ReadUserFields_FULL(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            UserId = (string)rdr["UserId"];
            UserName = (string)rdr["UserName"];

            Culture = rdr["Culture"].ToString();

            BankId = rdr["BankId"].ToString();
            Branch = rdr["Branch"].ToString();

            UserType = rdr["UserType"].ToString();

            CitId = (string)rdr["CitId"];

            DateTimeOpen = (DateTime)rdr["DateTimeOpen"];

            email = rdr["email"].ToString();
            MobileNo = rdr["MobileNo"].ToString();

            Authoriser = (bool)rdr["Authoriser"];

            UserInactive = (bool)rdr["UserInactive"];
            UserOnLeave = (bool)rdr["UserOnLeave"];

            PassWord = rdr["PassWord"].ToString();

            DtChanged = (DateTime)rdr["DtChanged"];
            DtToBeChanged = (DateTime)rdr["DtToBeChanged"];

            MaxDateTmForTempPassword = (DateTime)rdr["MaxDateTmForTempPassword"];

            ForceChangePassword = (bool)rdr["ForceChangePassword"];

            Operator = (string)rdr["Operator"];
            
            User_Is_Maker = (bool)rdr["User_Is_Maker"];
            User_Is_Checker = (bool)rdr["User_Is_Checker"];
            DtTimeOfAssigment = (DateTime)rdr["DtTimeOfAssigment"];
            // If An ADD is made by what Maker
            ChangedByWhatMaker = (string)rdr["ChangedByWhatMaker"];
            DtTimeOfChange = (DateTime)rdr["DtTimeOfChange"];

            Is_New_User = (bool)rdr["Is_New_User"];
            HadToMakeInactive= (bool)rdr["HadToMakeInactive"];
            HadToUndoInactive= (bool) rdr["HadToUndoInactive"];

           Is_NewAccessRights = (bool)rdr["Is_NewAccessRights"];
            Is_NewCategory = (bool)rdr["Is_NewCategory"];
            Is_NewAuthoriser = (bool)rdr["Is_NewAuthoriser"];
           
            ApprovedByWhatChecker = (string)rdr["ApprovedByWhatChecker"];
            ApproveOR_Reject = (string)rdr["ApproveOR_Reject"];
            DtTimeOfApproving = (DateTime)rdr["DtTimeOfApproving"];
            Is_Approved = (bool)rdr["Is_Approved"];
        }



        // Methods 
        // READ Reconc Officers   
        // FILL UP A TABLE 
        public void ReadReconcOfficers(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ReconcUsersSelected = new DataTable();
            ReconcUsersSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ReconcUsersSelected.Columns.Add("UserId", typeof(string));
            ReconcUsersSelected.Columns.Add("UserName", typeof(string));
            ReconcUsersSelected.Columns.Add("TotalAssigned", typeof(int));
            ReconcUsersSelected.Columns.Add("SecLevel", typeof(string));
            ReconcUsersSelected.Columns.Add("ReconcOfficer", typeof(bool));
            ReconcUsersSelected.Columns.Add("MobileNo", typeof(string));

            SqlString = "SELECT *"
                     + " FROM [dbo].[UsersTable] "
                     + " WHERE Operator=@Operator AND ReconcOfficer = 1 ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            DataRow RowSelected = ReconcUsersSelected.NewRow();
                            string TUserId = (string)rdr["UserId"];
                            RowSelected["UserId"] = TUserId;
                            RowSelected["UserName"] = (string)rdr["UserName"];

                            Rc.ReadMatchingCategoriesNumberForUser(TUserId);

                            RowSelected["TotalAssigned"] = Rc.TotalCatForUser;

                            RowSelected["SecLevel"] = (string)rdr["SecLevel"];
                            RowSelected["ReconcOfficer"] = (bool)rdr["ReconcOfficer"];
                            RowSelected["MobileNo"] = (string)rdr["MobileNo"];

                            // ADD ROW
                            ReconcUsersSelected.Rows.Add(RowSelected);

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

        // GET Users List
        public ArrayList Get_Users_List(string InOperator)
        {
            ArrayList Users_List = new ArrayList();
            //Cut_Off_Dates_List.Add("No Value");

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string UserId_Name; 

            SqlString = "SELECT *"
                  + " FROM [dbo].[UsersTable] "
                  + " WHERE Operator=@Operator AND UserType = 'Employee' "
                  + " ORDER BY UserId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadUserFields(rdr);

                            Users_List.Add(UserId);

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

            return Users_List;
        }

        // Methods 
        // READ Users and Fill Table 
        // FILL UP A TABLE 
        public void ReadUsersAndFillDataTable(string InSignedId, string InOperator, string InFilter, string InApplication)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();

            UsersInDataTable = new DataTable();
            UsersInDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            UsersInDataTable.Columns.Add("UserId", typeof(string));
            UsersInDataTable.Columns.Add("UserName", typeof(string));
            UsersInDataTable.Columns.Add("email", typeof(string));
            UsersInDataTable.Columns.Add("MobileNo", typeof(string));
            UsersInDataTable.Columns.Add("DateOpen", typeof(string));
            UsersInDataTable.Columns.Add("UserType", typeof(string));
            UsersInDataTable.Columns.Add("CitId", typeof(string));

            SqlString = "SELECT * "
                         + " FROM [dbo].[UsersTable] "
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

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadUserFields(rdr);

                            if (InApplication == "")
                            {
                                // GET ALL USERS

                                //
                                //FILL IN TABLE
                                //

                                DataRow RowSelected = UsersInDataTable.NewRow();

                                RowSelected["UserId"] = UserId;

                                RowSelected["UserName"] = UserName;
                                RowSelected["email"] = email;
                                RowSelected["MobileNo"] = MobileNo;
                                RowSelected["DateOpen"] = DateTimeOpen.ToString();
                                RowSelected["UserType"] = UserType;
                                RowSelected["CitId"] = CitId;

                                // ADD ROW
                                UsersInDataTable.Rows.Add(RowSelected);

                            }
                            else
                            {
                                // GET ONLY FOR THIS APPLICATION
                                Uar.ReadUsersVsApplicationsVsRolesByApplication(UserId, InApplication);
                                if (Uar.RecordFound == true)
                                {
                                    //
                                    //FILL IN TABLE
                                    //

                                    DataRow RowSelected = UsersInDataTable.NewRow();

                                    RowSelected["UserId"] = UserId;

                                    RowSelected["UserName"] = UserName;
                                    RowSelected["email"] = email;
                                    RowSelected["MobileNo"] = MobileNo;
                                    RowSelected["DateOpen"] = DateTimeOpen.ToString();
                                    RowSelected["UserType"] = UserType;
                                    RowSelected["CitId"] = CitId;

                                    // ADD ROW
                                    UsersInDataTable.Rows.Add(RowSelected);
                                }
                            }



                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertRecordsOfReport(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // Methods 
        // READ Users and Fill Table 
        // FILL UP A TABLE 
        public void ReadUsersAndFillDataTable_Maker_Checker(string InSignedId, string InOperator, string InFilter, string InApplication)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();

            UsersInDataTable = new DataTable();
            UsersInDataTable.Clear();

            TotalSelected = 0;
            if (InSignedId == "ADMIN-BDC")
            {
                // DATA TABLE ROWS DEFINITION for MAKER AND CHECKER 
         
                UsersInDataTable.Columns.Add("UserId", typeof(string));
                UsersInDataTable.Columns.Add("UserName", typeof(string));
                UsersInDataTable.Columns.Add("DateOpen", typeof(DateTime));
                UsersInDataTable.Columns.Add("IsInactive", typeof(bool));
                UsersInDataTable.Columns.Add("User_Is_Maker", typeof(bool));
                UsersInDataTable.Columns.Add("User_Is_Checker", typeof(bool));
                UsersInDataTable.Columns.Add("DtTimeOfAssigment", typeof(DateTime));
            }
            else
            {
                // DATA TABLE ROWS DEFINITION for MAKER AND CHECKER 
               // UsersInDataTable.Columns.Add("Alert", typeof(bool));
                UsersInDataTable.Columns.Add("UserId", typeof(string));
                UsersInDataTable.Columns.Add("UserName", typeof(string));
                UsersInDataTable.Columns.Add("DateOpen", typeof(DateTime));
                UsersInDataTable.Columns.Add("IsInactive", typeof(bool));
                UsersInDataTable.Columns.Add("Is_Approved", typeof(bool));
                UsersInDataTable.Columns.Add("Maker", typeof(string));
                UsersInDataTable.Columns.Add("MakerDate", typeof(DateTime));
                UsersInDataTable.Columns.Add("Checker", typeof(string));
                UsersInDataTable.Columns.Add("CheckerDate", typeof(string));
                UsersInDataTable.Columns.Add("CheckerReply", typeof(string)); // Approve = YES not Approve = No
            }      

            SqlString = "SELECT * "
                         + " FROM [dbo].[UsersTable] "
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

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadUserFields_FULL(rdr);

                            if (InApplication == "")
                            {
                                // GET ALL USERS

                                //
                                //FILL IN TABLE
                                //
                                if (InSignedId == "ADMIN-BDC")
                                {
                                    // Coming from ADMIN 
                                    DataRow RowSelected = UsersInDataTable.NewRow();

                                    //RowSelected["Alert"] = false;
                                    RowSelected["UserId"] = UserId;
                                    RowSelected["UserName"] = UserName;
                                    RowSelected["DateOpen"] = DateTimeOpen.ToString();

                                    if (UserInactive == true)
                                    {
                                        RowSelected["IsInactive"] = true;
                                    }
                                    else
                                    {
                                        RowSelected["IsInactive"] = false;
                                    }

                                    RowSelected["User_Is_Maker"] = User_Is_Maker;
                                    RowSelected["User_Is_Checker"] = User_Is_Checker;
                                    RowSelected["DtTimeOfAssigment"] = DtTimeOfAssigment.ToString();

                                    // ADD ROW
                                    UsersInDataTable.Rows.Add(RowSelected);
                                }
                                else
                                {
                                    // coming from USER management 
                                    DataRow RowSelected = UsersInDataTable.NewRow();

                                  //  RowSelected["Alert"] = false;
                                    RowSelected["UserId"] = UserId;
                                    RowSelected["UserName"] = UserName;
                                    RowSelected["DateOpen"] = DateTimeOpen.ToString();

                                    if (UserInactive == true)
                                    {
                                        RowSelected["IsInactive"] = true;
                                    }
                                    else
                                    {
                                        RowSelected["IsInactive"] = false;
                                    }
                                    RowSelected["Is_Approved"] = Is_Approved;
                                   
                                    RowSelected["Maker"] = ChangedByWhatMaker;
                                    RowSelected["MakerDate"] = DtTimeOfChange.ToString();

                                    RowSelected["Checker"] = ApprovedByWhatChecker;
                                    RowSelected["CheckerDate"] = DtTimeOfApproving.ToString();

                                    RowSelected["CheckerReply"] = ApproveOR_Reject;

                                    // ADD ROW
                                    UsersInDataTable.Rows.Add(RowSelected);
                                }

                                

                            }
                            else
                            {
                                // GET ONLY FOR THIS APPLICATION
                                Uar.ReadUsersVsApplicationsVsRolesByApplication(UserId, InApplication);
                                if (Uar.RecordFound == true)
                                {
                                    //
                                    //FILL IN TABLE
                                    //

                                    DataRow RowSelected = UsersInDataTable.NewRow();

                                    RowSelected["UserId"] = UserId;

                                    RowSelected["UserName"] = UserName;
                                    RowSelected["email"] = email;
                                    RowSelected["MobileNo"] = MobileNo;
                                    RowSelected["DateOpen"] = DateTimeOpen.ToString();
                                    RowSelected["UserType"] = UserType;
                                    RowSelected["CitId"] = CitId;

                                    // ADD ROW
                                    UsersInDataTable.Rows.Add(RowSelected);
                                }
                            }



                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //InsertRecordsOfReport(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }


        // Insert Records Of Report 
        private void InsertRecordsOfReport(string InUserId)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport78(InUserId);

            int I = 0;

            while (I <= (UsersInDataTable.Rows.Count - 1))
            {

                RecordFound = true;

                Tr.LineUserId = (string)UsersInDataTable.Rows[I]["UserId"];
                Tr.UserName = (string)UsersInDataTable.Rows[I]["UserName"];
                Tr.email = (string)UsersInDataTable.Rows[I]["email"];

                Tr.MobileNo = (string)UsersInDataTable.Rows[I]["MobileNo"];
                Tr.DateOpen = (string)UsersInDataTable.Rows[I]["DateOpen"];
                Tr.UserType = (string)UsersInDataTable.Rows[I]["UserType"];
                Tr.CitId = (string)UsersInDataTable.Rows[I]["CitId"];

                // Insert record for printing 
                //
                Tr.InsertReport78(InUserId);

                I++; // Read Next entry of the table 

            }
        }

        public void Insert_In_Maintenance_History(string InUserId)
        {
            RRDMUsers_Applications_Roles Ar = new RRDMUsers_Applications_Roles();

            // GET THE ACCESS RIGHTS and Update the history 

            Ar.ReadUsersVsApplicationsVsRolesByUserAndAccessRights(InUserId);

            int Counter; 
            // History File 
            string SQLCmd = "INSERT INTO [ATMS].[dbo].[Users_Maintenance_History] "
              + " ( "
              + "[UserId], [UserName] "
              + " ,Admin "
              + " , Controller "
                 + " ,[Authoriser] "
                 + " ,[DisputeOfficer] "

                 + " ,[ReconcOfficer]" 
                 + ", [ReconcMgr] "

                 + " ,[ChangedByWhatMaker],[DtTimeOfChange] "
                 + " ,[Is_New_User] "
                 + " ,[HadToMakeInactive] "
                  + " ,[HadToUndoInactive] "
                  + " ,[Is_NewAccessRights] "
                 + " ,[Is_NewCategory],[Is_NewAuthoriser] "

                 + " ,[ApprovedByWhatChecker],[ApproveOR_Reject] "
                 + ", [DtTimeOfApproving]"
                 + " ,[Is_Approved] "
                   + ") "
            //
            + " SELECT  "

               + "[UserId], [UserName] "
                + " , @Admin "
              + " , @Controller "
                 + " ,@Authoriser "
                 + " ,@DisputeOfficer "

                 + " ,@ReconcOfficer " 
                + ",@ReconcMgr "

                 + " ,[ChangedByWhatMaker],[DtTimeOfChange] "
                 + " ,[Is_New_User] "
                 + " , [HadToMakeInactive] "
                  + " , [HadToUndoInactive] "
                  + " ,[Is_NewAccessRights] "
                 + " ,[Is_NewCategory],[Is_NewAuthoriser] "

                 + " ,[ApprovedByWhatChecker],[ApproveOR_Reject] "
                 + ", [DtTimeOfApproving]"
                 + " ,[Is_Approved] "

           + " FROM [ATMS].[dbo].[UsersTable]" 
           + " WHERE  UserId = @UserId "
           + "  "
           ;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@Admin", Ar.W_Admin);

                        cmd.Parameters.AddWithValue("@Controller", Ar.W_Controller);
                        cmd.Parameters.AddWithValue("@Authoriser", Ar.W_Authoriser);
                        cmd.Parameters.AddWithValue("@DisputeOfficer", Ar.W_DisputeOfficer);
                        cmd.Parameters.AddWithValue("@ReconcOfficer", Ar.W_ReconcOfficer);
                        cmd.Parameters.AddWithValue("@ReconcMgr", Ar.W_ReconcMgr);

                        cmd.CommandTimeout = 50;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();

                    //System.Windows.Forms.MessageBox.Show("Records Inserted" + Environment.NewLine
                    //             + "..:.." + Counter.ToString());

                }
                catch (Exception ex)
                {
                    conn.Close();

                  //  MessageBox.Show("Cancel While Loading At Matching Category:" + InMatchingCateg);

                    CatchDetails(ex);
                }
       
            //    RecordFound = false;
            //    ErrorFound = false;
            //    ErrorOutput = "";

            //    string UserId_Name;

            //    SqlString = "SELECT SeqNo "
            //          + " FROM [ATMS].[dbo].[Users_Maintenance_History] "
            //          + " WHERE UserId=@UserId  "
            //          + " ";

            //    using (SqlConnection conn =
            //                  new SqlConnection(connectionString))
            //        try
            //        {
            //            conn.Open();
            //            using (SqlCommand cmd =
            //                new SqlCommand(SqlString, conn))
            //            {
            //                cmd.Parameters.AddWithValue("@UserId", InUserId);

             
            //                // Read table 

            //                SqlDataReader rdr = cmd.ExecuteReader();

            //                while (rdr.Read())
            //                {
            //                RecordFound = true;

            //                SeqNo = (int)rdr["SeqNo"];

            //            }

            //                // Close Reader
            //                rdr.Close();
            //            }

            //            // Close conn
            //            conn.Close();
            //        }
            //        catch (Exception ex)
            //        {
            //            conn.Close();

            //            CatchDetails(ex);
            //        }

            
            //ErrorFound = false;
            //ErrorOutput = "";
            
            //using (SqlConnection conn =
            //    new SqlConnection(connectionString))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand("UPDATE [ATMS].[dbo].[Users_Maintenance_History] SET "
            //                + " Admin = @Admin, Controller = @Controller,"
            //                + "Authoriser = @Authoriser, DisputeOfficer = @DisputeOfficer"
            //                + "ReconcOfficer = @ReconcOfficer, "
            //                + "ReconcMgr = @ReconcMgr "
            //                + " WHERE SeqNo=@SeqNo", conn))
            //        {
            //            cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
            //            cmd.Parameters.AddWithValue("@Admin", Ar.W_Admin);
            //            cmd.Parameters.AddWithValue("@Controller", Ar.W_Controller);

            //            cmd.Parameters.AddWithValue("@Authoriser", Ar.W_Authoriser);

            //            cmd.Parameters.AddWithValue("@DisputeOfficer", Ar.W_DisputeOfficer);
            //            cmd.Parameters.AddWithValue("@ReconcOfficer", Ar.W_ReconcOfficer);

            //            cmd.Parameters.AddWithValue("@ReconcMgr", Ar.W_ReconcMgr);

            //            int rows = cmd.ExecuteNonQuery();
                       
            //        }
            //        // Close conn
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.Close();

            //        CatchDetails(ex);
            //    }


        }





        // READ  HISTORY OF CHANGES for USER

        public void Read_HST_TXNS_FOR_USER(string InUSER)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            UserHistoryDataTable = new DataTable();
            UserHistoryDataTable.Clear();

            SqlString =
               " SELECT * "
               + " FROM [ATMS].[dbo].[Users_Maintenance_History] " 
               + " WHERE UserId = @UserId "
                 + " ORDER by SeqNo DESC  "
                ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@UserId", InUSER);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(UserHistoryDataTable);

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
        // READ User RECORD 
        public void ReadUsersRecord(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                 + " FROM [dbo].[UsersTable] "
                 + " WHERE UserId = @UserId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //       cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@UserId", InUserId);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details

                            ReadUserFields(rdr);

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
        public void ReadUsersRecord_FULL(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                 + " FROM [dbo].[UsersTable] "
                 + " WHERE UserId = @UserId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //       cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@UserId", InUserId);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details

                            ReadUserFields_FULL(rdr);

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
        public bool User_Is_Maker;
        public bool User_Is_Checker;
        public DateTime DtTimeOfAssigment; 
        // READ Tail User RECORD 
        public void ReadUsersRecord_Tail_Info(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
       
            SqlString = " SELECT UserId,User_Is_Maker,User_Is_Checker,DtTimeOfAssigment  "
                 + " FROM [dbo].[UsersTable] "
                 + " WHERE UserId = @UserId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //       cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@UserId", InUserId);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details
                            UserId = (string)rdr["UserId"];

                            User_Is_Maker = (bool)rdr["User_Is_Maker"];

                            User_Is_Checker = (bool)rdr["User_Is_Checker"];

                            DtTimeOfAssigment = (DateTime)rdr["DtTimeOfAssigment"];

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

        // READ User RECORD 
        public void ReadIfExist(string InOperator, string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                 + " FROM [dbo].[UsersTable] "
                 + " WHERE Operator = @Operator AND UserId = @UserId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@UserId", InUserId);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details

                            ReadUserFields(rdr);

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


        // Get Nostro Users 
        public ArrayList GetUsersList(string InOperator, string InUserId)
        {
            ArrayList UsersList = new ArrayList();
            UsersList.Add("ALL");

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InUserId == "NOSTROAuther")
            {
                SqlString = "SELECT * FROM [dbo].[UsersTable] "
                                             + " WHERE Operator = @Operator "
                                             + " AND (SeqNo = 69 OR SeqNo = 71) "
                                             + " ORDER BY  UserId";
            }
            if (InUserId == "487116")
            {
                SqlString = "SELECT * FROM [dbo].[UsersTable] "
                                             + " WHERE Operator = @Operator "
                                             + " AND (SeqNo = 8 OR SeqNo = 51) "
                                             + " ORDER BY  UserId";
            }

            if (InUserId == "ITMXUser3")
            {
                SqlString = "SELECT * FROM [dbo].[UsersTable] "
                                             + " WHERE Operator = @Operator "
                                             + " AND (SeqNo = 69 OR SeqNo = 71) "
                                             + " ORDER BY  UserId";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            UserId = (string)rdr["UserId"];
                            UsersList.Add(UserId);
                            UserType = rdr["UserType"].ToString();
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

            return UsersList;
        }


        // Insert USER ID
        public void InsertUser(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";
            
            string cmdinsert = "INSERT INTO [UsersTable]"
                + " ([UserId],[UserName],[Culture],[BankId],[Branch],[UserType],"
                + " [CitId],"
                + " [DateTimeOpen],[email],[MobileNo],"

                + " [UserInactive],[UserOnLeave],"
                + "[PassWord],[DtChanged],[DtToBeChanged],"
                + " [MaxDateTmForTempPassword],"
                + "[ForceChangePassword], [Operator])"
                + " VALUES (@UserId,@UserName,@Culture,@BankId,@Branch,@UserType,"
                + "@CitId,"
                + "@DateTimeOpen,@email,@MobileNo,"

                + "@UserInactive,@UserOnLeave,"
                + "@PassWord, @DtChanged, @DtToBeChanged,"
                + "@MaxDateTmForTempPassword,"
                + " @ForceChangePassword, @Operator)";


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@UserName", UserName);

                        cmd.Parameters.AddWithValue("@Culture", Culture);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@Branch", Branch);

                        cmd.Parameters.AddWithValue("@UserType", UserType);

                        //  cmd.Parameters.AddWithValue("@RoleNm", RoleNm);
                        //    cmd.Parameters.AddWithValue("@SecLevel", SecLevel);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@DateTimeOpen", DateTimeOpen);

                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@MobileNo", MobileNo);

                        cmd.Parameters.AddWithValue("@UserInactive", UserInactive);
                        cmd.Parameters.AddWithValue("@UserOnLeave", UserOnLeave);

                        cmd.Parameters.AddWithValue("@PassWord", PassWord);

                        cmd.Parameters.AddWithValue("@DtChanged", DtChanged);
                        cmd.Parameters.AddWithValue("@DtToBeChanged", DtToBeChanged);

                        cmd.Parameters.AddWithValue("@MaxDateTmForTempPassword", MaxDateTmForTempPassword);
                        cmd.Parameters.AddWithValue("@ForceChangePassword", ForceChangePassword);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        
                        int rows = cmd.ExecuteNonQuery();
                       
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

        
        // UPDATE USER TABLE   
        public void UpdateUser(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";
            //MaxDateTmForTempPassword
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE UsersTable SET "
                            + " UserId = @UserId, UserName = @UserName,"
                            + "Culture = @Culture, BankId = @BankId, Branch = @Branch,"
                            + " UserType = @UserType,"
                            + " CitId = @CitId,"
                            + "DateTimeOpen = @DateTimeOpen, "
                            + "email = @email,MobileNo = @MobileNo,"
                            //+ "Authoriser = @Authoriser,DisputeOfficer = @DisputeOfficer,"
                            //+ "ReconcOfficer = @ReconcOfficer,ReconcMgr = @ReconcMgr,"
                            //+ "MsgsAllowed = @MsgsAllowed,"
                            + "UserInactive = @UserInactive,"
                            + "UserOnLeave = @UserOnLeave,PassWord = @PassWord, "
                            + "DtChanged = @DtChanged,DtToBeChanged = @DtToBeChanged, "
                            + "MaxDateTmForTempPassword = @MaxDateTmForTempPassword, "
                            + "ForceChangePassword = @ForceChangePassword "
                            + " WHERE UserId=@UserId", conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@UserName", UserName);

                        cmd.Parameters.AddWithValue("@Culture", Culture);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@Branch", Branch);

                        cmd.Parameters.AddWithValue("@UserType", UserType);
                        //cmd.Parameters.AddWithValue("@RoleNm", RoleNm);
                        //cmd.Parameters.AddWithValue("@SecLevel", SecLevel);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@DateTimeOpen", DateTime.Now);

                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@MobileNo", MobileNo);

                        //cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        //cmd.Parameters.AddWithValue("@DisputeOfficer", DisputeOfficer);

                        //cmd.Parameters.AddWithValue("@ReconcOfficer", ReconcOfficer);
                        //cmd.Parameters.AddWithValue("@ReconcMgr", ReconcMgr);

                        //cmd.Parameters.AddWithValue("@MsgsAllowed", MsgsAllowed);

                        cmd.Parameters.AddWithValue("@UserInactive", UserInactive);
                        cmd.Parameters.AddWithValue("@UserOnLeave", UserOnLeave);
                        cmd.Parameters.AddWithValue("@PassWord", PassWord);

                        cmd.Parameters.AddWithValue("@DtChanged", DtChanged);
                        cmd.Parameters.AddWithValue("@DtToBeChanged", DtToBeChanged);

                        cmd.Parameters.AddWithValue("@MaxDateTmForTempPassword", MaxDateTmForTempPassword);
                        cmd.Parameters.AddWithValue("@ForceChangePassword", ForceChangePassword);

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

        // UPDATE USER TABLE  TAIL  
        public void UpdateTail(string InUserId)
        {
            //        ,[Maker]
            //,[Checker]
            //,[DtTimeOfAssigment]
            int rows;
            ErrorFound = false;
            ErrorOutput = "";
            //MaxDateTmForTempPassword
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE UsersTable SET "
                            + " User_Is_Maker = @User_Is_Maker, User_Is_Checker = @User_Is_Checker,"
                            + "DtTimeOfAssigment = @DtTimeOfAssigment "
                            + " WHERE UserId=@UserId", conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@User_Is_Maker", User_Is_Maker);
                        cmd.Parameters.AddWithValue("@User_Is_Checker", User_Is_Checker);

                        cmd.Parameters.AddWithValue("@DtTimeOfAssigment", DtTimeOfAssigment);

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

        // UPDATE USER TABLE FOR MAKER WORK


        //            ChangedByWhatMaker nvarchar(50)	Unchecked
        //DtTimeOfChange  datetime Unchecked
        //ApprovedByWhatChecker nvarchar(50)	Unchecked
        //ApproveOR_Reject    nvarchar(10)    Unchecked
        //DtTimeOfApproving   datetime Unchecked
        //Is_Approved bit Unchecked

        public void UpdateTail_For_Maker_Work(string InTargetUserId)
        {
            //        ,[Maker]
            //,[Checker]
            //,[DtTimeOfAssigment]
            int rows;
            ErrorFound = false;
            ErrorOutput = "";
            //            ChangedByWhatMaker nvarchar(50)	Unchecked
            //DtTimeOfChange  datetime Unchecked
            //ApprovedByWhatChecker nvarchar(50)	Unchecked
            //Us.Is_New_User = true;
            //Us.Is_NewAccessRights = false;
            //Us.Is_NewCategory = false;
            //Us.Is_NewAuthoriser = false;
            //Us.HadToUndoInactive
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE UsersTable SET "
                            + " ChangedByWhatMaker = @ChangedByWhatMaker,"
                            + " DtTimeOfChange = @DtTimeOfChange, "
                              + " Is_New_User = @Is_New_User, "
                              + " HadToMakeInactive = @HadToMakeInactive, "
                               + " HadToUndoInactive = @HadToUndoInactive, "
                                //HadToMakeInactive
                                + " Is_NewAccessRights = @Is_NewAccessRights, "
                                  + " Is_NewCategory = @Is_NewCategory, "
                                    + " Is_NewAuthoriser = @Is_NewAuthoriser "
                            + " WHERE UserId=@UserId ", conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InTargetUserId);

                        cmd.Parameters.AddWithValue("@ChangedByWhatMaker", ChangedByWhatMaker);
                        cmd.Parameters.AddWithValue("@DtTimeOfChange", DtTimeOfChange);

                        cmd.Parameters.AddWithValue("@Is_New_User", Is_New_User);
                        cmd.Parameters.AddWithValue("@HadToMakeInactive", HadToMakeInactive);
                        cmd.Parameters.AddWithValue("@HadToUndoInactive", HadToUndoInactive);
                        cmd.Parameters.AddWithValue("@Is_NewAccessRights", Is_NewAccessRights);
                        cmd.Parameters.AddWithValue("@Is_NewCategory", Is_NewCategory);
                        cmd.Parameters.AddWithValue("@Is_NewAuthoriser", Is_NewAuthoriser);

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


        public void UpdateTail_For_Admin_Work(string InTargetUserId)
        {

            int rows;
            ErrorFound = false;
            ErrorOutput = "";

//            User_Is_Maker bit Unchecked
//User_Is_Checker bit Unchecked
//DtTimeOfAssigment datetime    Unchecked
//Is_Approved

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[UsersTable] SET "
                            + " User_Is_Maker = @User_Is_Maker,"
                            + " User_Is_Checker = @User_Is_Checker, "
                            + " DtTimeOfAssigment = @DtTimeOfAssigment, "
                            + " Is_Approved = @Is_Approved "
                            + " WHERE UserId=@UserId", conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InTargetUserId);

                        cmd.Parameters.AddWithValue("@User_Is_Maker", User_Is_Maker);
                        cmd.Parameters.AddWithValue("@User_Is_Checker", User_Is_Checker);

                        cmd.Parameters.AddWithValue("@DtTimeOfAssigment", DtTimeOfAssigment);

                        cmd.Parameters.AddWithValue("@Is_Approved", Is_Approved);

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


        public void UpdateTail_For_Checker_Work(string InTargetUserId)
        {
          
            int rows;
            ErrorFound = false;
            ErrorOutput = "";
            //            ChangedByWhatMaker nvarchar(50)	Unchecked
            //DtTimeOfChange  datetime Unchecked
            //ApprovedByWhatChecker nvarchar(50)	Unchecked
            //ApproveOR_Reject    nvarchar(10)    Unchecked
            //DtTimeOfApproving   datetime Unchecked
            //Is_Approved bit Unchecked
            // UserInactive
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[UsersTable] SET "
                            + " ApprovedByWhatChecker = @ApprovedByWhatChecker,"
                            + "ApproveOR_Reject = @ApproveOR_Reject, "
                            + "DtTimeOfApproving = @DtTimeOfApproving , "
                          
                            //+ "HadToMakeInactive = @HadToMakeInactive , "
                            + "UserInactive = @UserInactive , "
                            + "Is_Approved = @Is_Approved "
                            + " WHERE UserId=@UserId", conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InTargetUserId);

                        cmd.Parameters.AddWithValue("@ApprovedByWhatChecker", ApprovedByWhatChecker);
                        cmd.Parameters.AddWithValue("@ApproveOR_Reject", ApproveOR_Reject);

                        cmd.Parameters.AddWithValue("@DtTimeOfApproving", DtTimeOfApproving);

                        //cmd.Parameters.AddWithValue("@HadToMakeInactive", HadToMakeInactive);

                        cmd.Parameters.AddWithValue("@UserInactive", UserInactive);

                        cmd.Parameters.AddWithValue("@Is_Approved", Is_Approved);

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

        //// READ User Signed In Latest Activity 

        //
        // DELETE User by Id
        //
        public void DeleteUserById(string InUserId)
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
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[UsersTable] "
                            + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

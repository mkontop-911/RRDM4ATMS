using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMUsersAndSignedRecord
    {
        // Declarations 

        // Users Table 

        public string UserId;
        public string UserName;
  
        public string Culture;
        public string BankId;
        public string Branch;
        public string UserType; 
        public string RoleNm;
        public int SecLevel;
       
        public string CitId;
        public DateTime DateTimeOpen;
        
        public string email;
        public string MobileNo;
        public bool Authoriser;
        public bool DisputeOfficer;
        public bool ReconcOfficer;
        public bool ReconcMgr;
        public bool UserInactive;
        public bool UserOnLeave;
        public string PassWord;
        public DateTime DtChanged;
        public DateTime DtToBeChanged;
        public bool ForceChangePassword;
        public string Operator; 

        // USER VERSUS ATM OR GROUP for REPL or RECONC

        // SiGNED IN OCCURANCE VARIABLES 
        public int SignRecordNo;
        public bool SignInStatus; 
        public DateTime DtTmIn;
        public DateTime DtTmOut;
        public bool Replenishment;
        public bool Reconciliation;
        public bool OtherActivity;
        public int StepLevel;
        public int ProcessNo;
        public int ProcessStatus;
        public bool ReplStep1_Updated;
        public bool ReplStep2_Updated;
        public bool ReplStep3_Updated;
        public bool ReplStep4_Updated;
        public bool ReplStep5_Updated;
        public int ReconcDifferenceStatus; 
        public int WFieldNumeric1; 
        public int WFieldNumeric11; 
        public int WFieldNumeric12;
        public string WFieldChar1;
        public string WFieldChar2; 
        public string GeneralUsedComment; 
      //  public bool EnableInfo;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data tables 
        public DataTable DisputeUsersSelected = new DataTable();

        public DataTable ReconcUsersSelected = new DataTable();

        public int TotalSelected;
        string SqlString; // Do not delete

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMReconcCategories Rc = new RRDMReconcCategories(); 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Methods 
        // READ Dispute Officers   
        // FILL UP A TABLE 
        public void ReadDisputeOfficers(string InOperator )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DisputeUsersSelected = new DataTable();
            DisputeUsersSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DisputeUsersSelected.Columns.Add("UserId", typeof(string));
            DisputeUsersSelected.Columns.Add("UserName", typeof(string));
            DisputeUsersSelected.Columns.Add("TotalDisp", typeof(int));
            DisputeUsersSelected.Columns.Add("SecLevel", typeof(int));
            DisputeUsersSelected.Columns.Add("DisputeOfficer", typeof(bool));
            DisputeUsersSelected.Columns.Add("MobileNo", typeof(string));

            SqlString = "SELECT *"
                     + " FROM [dbo].[UsersTable] "
                     + " WHERE Operator=@Operator AND DisputeOfficer= 1 ";

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

                            DataRow RowSelected = DisputeUsersSelected.NewRow();
                            string TUserId = (string)rdr["UserId"];
                            RowSelected["UserId"] = TUserId;
                            RowSelected["UserName"] = (string)rdr["UserName"];

                            Di.ReadDisputeOwnerTotal(TUserId);

                            RowSelected["TotalDisp"] = Di.DisputeOwnerTotal;

                            RowSelected["SecLevel"] = (int)rdr["SecLevel"];
                            RowSelected["DisputeOfficer"] = (bool)rdr["DisputeOfficer"];
                            RowSelected["MobileNo"] = (string)rdr["MobileNo"];

                            // ADD ROW
                            DisputeUsersSelected.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reading table Disputes User Officers Class............. " + ex.Message;

                }
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
            ReconcUsersSelected.Columns.Add("SecLevel", typeof(int));
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

                            Rc.ReadReconcCategoriesNumberForUser(TUserId);

                            RowSelected["TotalAssigned"] = Rc.TotalCatForUser;

                            RowSelected["SecLevel"] = (int)rdr["SecLevel"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Reading table Reconcs User Officers Class............. " + ex.Message;

                }
        }
       
        // READ User RECORD 
        public void ReadUsersRecord(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
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
                            UserName = (string)rdr["UserName"];

                            Culture = rdr["Culture"].ToString();

                            BankId = rdr["BankId"].ToString();
                            Branch = rdr["Branch"].ToString();

                            UserType = rdr["UserType"].ToString();

                            RoleNm = rdr["RoleNm"].ToString();
                            SecLevel = (int)rdr["SecLevel"];

                            CitId = (string)rdr["CitId"];

                            DateTimeOpen = (DateTime)rdr["DateTimeOpen"];
                            
                            email = rdr["email"].ToString();
                            MobileNo = rdr["MobileNo"].ToString();

                            Authoriser = (bool)rdr["Authoriser"];

                            DisputeOfficer = (bool)rdr["DisputeOfficer"];
                            ReconcOfficer = (bool)rdr["ReconcOfficer"];
                            ReconcMgr = (bool)rdr["ReconcMgr"];

                            UserInactive = (bool)rdr["UserInactive"];
                            UserOnLeave = (bool)rdr["UserOnLeave"];

                            PassWord = rdr["PassWord"].ToString();

                            DtChanged = (DateTime)rdr["DtChanged"];
                            DtToBeChanged = (DateTime)rdr["DtToBeChanged"];
                            ForceChangePassword = (bool)rdr["ForceChangePassword"];

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
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }
        }

        // READ ROLE NAME RECORD SUCH AS Controller RECORD 
        // 
        public void ReadRoleRecord(string InBankId, string InRoleNm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[UsersTable] "
          + " WHERE BankId = @BankId AND RoleNm = @RoleNm";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@RoleNm", InRoleNm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details

                            UserId = (string)rdr["UserId"];
                            UserName = (string)rdr["UserName"];

                            BankId = rdr["BankId"].ToString();
                        
                            Culture = rdr["Culture"].ToString();

                            Branch = rdr["Branch"].ToString();

                            UserType = rdr["UserType"].ToString();

                            RoleNm = rdr["RoleNm"].ToString();
                            SecLevel = (int)rdr["SecLevel"];

                            CitId = (string)rdr["CitId"];

                            DateTimeOpen = (DateTime)rdr["DateTimeOpen"];

                            email = rdr["email"].ToString();
                            MobileNo = rdr["MobileNo"].ToString();

                            Authoriser = (bool)rdr["Authoriser"];
                            DisputeOfficer = (bool)rdr["DisputeOfficer"];

                            ReconcOfficer = (bool)rdr["ReconcOfficer"];
                            ReconcMgr = (bool)rdr["ReconcMgr"];

                            UserInactive = (bool)rdr["UserInactive"];
                            UserOnLeave = (bool)rdr["UserOnLeave"];

                            PassWord = rdr["PassWord"].ToString();

                            DtChanged = (DateTime)rdr["DtChanged"];
                            DtToBeChanged = (DateTime)rdr["DtToBeChanged"];
                            ForceChangePassword = (bool)rdr["ForceChangePassword"];

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
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }
        }
        // Insert USER ID
        public void InsertUser(string InUserId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [UsersTable]"
                + " ([UserId],[UserName],[Culture],[BankId],[Branch],[UserType],[RoleNm],[SecLevel],"
                + " [CitId],"
                + " [DateTimeOpen],[email],[MobileNo],"
                + " [Authoriser],[DisputeOfficer],[ReconcOfficer],[ReconcMgr],"
                +" [UserInactive],[UserOnLeave],"
                + "[PassWord],[DtChanged],[DtToBeChanged],[ForceChangePassword], [Operator])"
                + " VALUES (@UserId,@UserName,@Culture,@BankId,@Branch,@UserType,@RoleNm,@SecLevel,"
                + "@CitId,"
                + "@DateTimeOpen,@email,@MobileNo,"
                + "@Authoriser,@DisputeOfficer,@ReconcOfficer,@ReconcMgr,"
                + "@UserInactive,@UserOnLeave,"
                + "@PassWord, @DtChanged, @DtToBeChanged, @ForceChangePassword, @Operator)";

            
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

                        cmd.Parameters.AddWithValue("@RoleNm", RoleNm);
                        cmd.Parameters.AddWithValue("@SecLevel", SecLevel);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@DateTimeOpen", DateTimeOpen);

                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@MobileNo", MobileNo);

                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);

                        cmd.Parameters.AddWithValue("@DisputeOfficer", DisputeOfficer);

                        cmd.Parameters.AddWithValue("@ReconcOfficer", ReconcOfficer);
                        cmd.Parameters.AddWithValue("@ReconcMgr", ReconcMgr);

                        cmd.Parameters.AddWithValue("@UserInactive", UserInactive);
                        cmd.Parameters.AddWithValue("@UserOnLeave", UserOnLeave);

                        cmd.Parameters.AddWithValue("@PassWord", PassWord);

                        cmd.Parameters.AddWithValue("@DtChanged", DtChanged);
                        cmd.Parameters.AddWithValue("@DtToBeChanged", DtToBeChanged);
                        cmd.Parameters.AddWithValue("@ForceChangePassword", ForceChangePassword);

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
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }
        }


        // UPDATE USER TABLE   
        public void UpdateUser(string InUserId)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE UsersTable SET "
                            + " UserId = @UserId, UserName = @UserName,"
                            + "Culture = @Culture, BankId = @BankId, Branch = @Branch,"
                            + " UserType = @UserType, RoleNm = @RoleNm,SecLevel = @SecLevel,"
                            + " CitId = @CitId,"
                            + "DateTimeOpen = @DateTimeOpen, "
                            + "email = @email,MobileNo = @MobileNo,"
                            + "Authoriser = @Authoriser,DisputeOfficer = @DisputeOfficer,"
                            + "ReconcOfficer = @ReconcOfficer,ReconcMgr = @ReconcMgr,"
                            + "UserInactive = @UserInactive,"
                            + "UserOnLeave = @UserOnLeave,PassWord = @PassWord, "
                            + "DtChanged = @DtChanged,DtToBeChanged = @DtToBeChanged, ForceChangePassword = @ForceChangePassword "
                            + " WHERE UserId=@UserId", conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@UserName", UserName);

                        cmd.Parameters.AddWithValue("@Culture", Culture);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@Branch", Branch);

                        cmd.Parameters.AddWithValue("@UserType", UserType);
                        cmd.Parameters.AddWithValue("@RoleNm", RoleNm);
                        cmd.Parameters.AddWithValue("@SecLevel", SecLevel);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@DateTimeOpen", DateTime.Now);

                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@MobileNo", MobileNo);

                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@DisputeOfficer", DisputeOfficer);

                        cmd.Parameters.AddWithValue("@ReconcOfficer", ReconcOfficer);
                        cmd.Parameters.AddWithValue("@ReconcMgr", ReconcMgr);

                        cmd.Parameters.AddWithValue("@UserInactive", UserInactive);
                        cmd.Parameters.AddWithValue("@UserOnLeave", UserOnLeave);
                        cmd.Parameters.AddWithValue("@PassWord", PassWord);

                        cmd.Parameters.AddWithValue("@DtChanged", DtChanged);
                        cmd.Parameters.AddWithValue("@DtToBeChanged", DtToBeChanged);
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }
        }

        // READ User Signed In Latest Activity 

        public void ReadSignedActivity(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[UsersSignedInActivity] "
          + " WHERE UserId = @UserId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details
                            SignRecordNo = (int)rdr["SignRecordNo"];
                            UserId = (string)rdr["UserId"];
                            UserName = rdr["UserName"].ToString();
                            SecLevel = (int)rdr["SecLevel"];

                            Culture = rdr["Culture"].ToString();

                            SignInStatus = (bool)rdr["SignInStatus"];

                            DtTmIn = (DateTime)rdr["DtTmIn"];
                            DtTmOut = (DateTime)rdr["DtTmOut"];

                            Replenishment = (bool)rdr["Replenishment"];
                            Reconciliation = (bool)rdr["Reconciliation"];
                            OtherActivity = (bool)rdr["OtherActivity"];
                            StepLevel = (int)rdr["StepLevel"];
                            ProcessNo = (int)rdr["ProcessNo"];
                            ProcessStatus = (int)rdr["ProcessStatus"];

                            ReplStep1_Updated = (bool)rdr["ReplStep1_Updated"];
                            ReplStep2_Updated = (bool)rdr["ReplStep2_Updated"];
                            ReplStep3_Updated = (bool)rdr["ReplStep3_Updated"];
                            ReplStep4_Updated = (bool)rdr["ReplStep4_Updated"];
                            ReplStep5_Updated = (bool)rdr["ReplStep5_Updated"];

                            WFieldNumeric1 = (int)rdr["WFieldNumeric1"];
                            WFieldNumeric11 = (int)rdr["WFieldNumeric11"];
                            WFieldNumeric12 = (int)rdr["WFieldNumeric12"];

                            WFieldChar1 = (string)rdr["WFieldChar1"];
                            WFieldChar2 = (string)rdr["WFieldChar2"];

                            ReconcDifferenceStatus = (int)rdr["ReconcDifferenceStatus"];

                            GeneralUsedComment = (string)rdr["GeneralUsedComment"];

                            
                  
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
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }

        }

        // READ User Signed by record number not user  

        public void ReadSignedActivityByKey(int InSignRecordNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[UsersSignedInActivity] "
          + " WHERE SignRecordNo = @SignRecordNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SignRecordNo", InSignRecordNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details
                            SignRecordNo = (int)rdr["SignRecordNo"];
                            UserId = (string)rdr["UserId"];
                            UserName = rdr["UserName"].ToString();
                            SecLevel = (int)rdr["SecLevel"];
                            Culture = rdr["Culture"].ToString();

                            SignInStatus = (bool)rdr["SignInStatus"];

                            DtTmIn = (DateTime)rdr["DtTmIn"];
                            DtTmOut = (DateTime)rdr["DtTmOut"];

                            Replenishment = (bool)rdr["Replenishment"];
                            Reconciliation = (bool)rdr["Reconciliation"];
                            OtherActivity = (bool)rdr["OtherActivity"];
                            StepLevel = (int)rdr["StepLevel"];
                            ProcessNo = (int)rdr["ProcessNo"];
                            ProcessStatus = (int)rdr["ProcessStatus"];

                            ReplStep1_Updated = (bool)rdr["ReplStep1_Updated"];
                            ReplStep2_Updated = (bool)rdr["ReplStep2_Updated"];
                            ReplStep3_Updated = (bool)rdr["ReplStep3_Updated"];
                            ReplStep4_Updated = (bool)rdr["ReplStep4_Updated"];
                            ReplStep5_Updated = (bool)rdr["ReplStep5_Updated"];

                            WFieldNumeric1 = (int)rdr["WFieldNumeric1"];
                            WFieldNumeric11 = (int)rdr["WFieldNumeric11"];
                            WFieldNumeric12 = (int)rdr["WFieldNumeric12"];

                            WFieldChar1 = (string)rdr["WFieldChar1"];
                            WFieldChar2 = (string)rdr["WFieldChar2"];

                            ReconcDifferenceStatus = (int)rdr["ReconcDifferenceStatus"];

                            GeneralUsedComment = (string)rdr["GeneralUsedComment"];

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
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }
        }

        // Get Operator from SignedIn Record   

        public void GetOperator(int InSignRecordNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[UsersSignedInActivity] "
          + " WHERE SignRecordNo = @SignRecordNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SignRecordNo", InSignRecordNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details
                            SignRecordNo = (int)rdr["SignRecordNo"];
                            UserId = (string)rdr["UserId"];
                            UserName = rdr["UserName"].ToString();
                            SecLevel = (int)rdr["SecLevel"];

                            BankId = rdr["BankId"].ToString();
                            //      Prive = (bool)rdr["Prive"];
                            Culture = rdr["Culture"].ToString();

                            SignInStatus = (bool)rdr["SignInStatus"];

                            DtTmIn = (DateTime)rdr["DtTmIn"];
                            DtTmOut = (DateTime)rdr["DtTmOut"];

                            Replenishment = (bool)rdr["Replenishment"];
                            Reconciliation = (bool)rdr["Reconciliation"];
                            OtherActivity = (bool)rdr["OtherActivity"];
                            StepLevel = (int)rdr["StepLevel"];
                            ProcessNo = (int)rdr["ProcessNo"];
                            ProcessStatus = (int)rdr["ProcessStatus"];

                            ReplStep1_Updated = (bool)rdr["ReplStep1_Updated"];
                            ReplStep2_Updated = (bool)rdr["ReplStep2_Updated"];
                            ReplStep3_Updated = (bool)rdr["ReplStep3_Updated"];
                            ReplStep4_Updated = (bool)rdr["ReplStep4_Updated"];
                            ReplStep5_Updated = (bool)rdr["ReplStep5_Updated"];

                            WFieldNumeric1 = (int)rdr["WFieldNumeric1"];
                            WFieldNumeric11 = (int)rdr["WFieldNumeric11"];
                            WFieldNumeric12 = (int)rdr["WFieldNumeric12"];

                            WFieldChar1 = (string)rdr["WFieldChar1"];
                            WFieldChar2 = (string)rdr["WFieldChar2"];

                            ReconcDifferenceStatus = (int)rdr["ReconcDifferenceStatus"];

                            GeneralUsedComment = (string)rdr["GeneralUsedComment"];
                            
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
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }
        }
        //
        // Insert USER SIGNED ACTIVITY 
        //
        public void InsertSignedActivity(string InUserId)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [UsersSignedInActivity]"
                + " ([SignRecordNo],[UserId],[UserName],[SecLevel],[Culture],[SignInStatus],[DtTmIn],[DtTmOut],"
                + "[Replenishment],[Reconciliation],[OtherActivity],[StepLevel],[ProcessNo],[ProcessStatus]  )"
                + " VALUES (NEXT VALUE FOR SignInRecordSeq,@UserId,@UserName,@SecLevel,@Culture,@SignInStatus,@DtTmIn,@DtTmOut,"
                + "@Replenishment,@Reconciliation,@OtherActivity,@StepLevel,@ProcessNo,@ProcessStatus)";
            
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
                        cmd.Parameters.AddWithValue("@SecLevel", SecLevel);

                        cmd.Parameters.AddWithValue("@Culture", Culture);

                        cmd.Parameters.AddWithValue("@SignInStatus", SignInStatus);

                        cmd.Parameters.AddWithValue("@DtTmIn", DtTmIn);
                        cmd.Parameters.AddWithValue("@DtTmOut", DtTmOut);

                        cmd.Parameters.AddWithValue("@Replenishment", Replenishment);
                        cmd.Parameters.AddWithValue("@Reconciliation", Reconciliation);
                        cmd.Parameters.AddWithValue("@OtherActivity", OtherActivity);
                        cmd.Parameters.AddWithValue("@StepLevel", 0);
                        cmd.Parameters.AddWithValue("@ProcessNo", 0);
                        cmd.Parameters.AddWithValue("@ProcessStatus", 0);


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
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }
        }

        //
        // UPDATE SIGNED IN TABLE   with DATE and TIME OUT 
        //
        public void UpdateSignedInTableDateTmOut(int InSignRecordNo)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE UsersSignedInActivity SET "
                            + " SignInStatus = @SignInStatus, DtTmOut = @DtTmOut,"
                             + " Replenishment = @Replenishment, Reconciliation = @Reconciliation,"
                             + " OtherActivity = @OtherActivity "
                            + " WHERE SignRecordNo = @SignRecordNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SignRecordNo", InSignRecordNo);
                        cmd.Parameters.AddWithValue("@SignInStatus", SignInStatus);
                        cmd.Parameters.AddWithValue("@DtTmOut", DtTmOut);
                        cmd.Parameters.AddWithValue("@Replenishment", Replenishment);
                        cmd.Parameters.AddWithValue("@Reconciliation", Reconciliation);
                        cmd.Parameters.AddWithValue("@OtherActivity", OtherActivity);

                       
                     //   cmd.Parameters.AddWithValue("@EnableInf);

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
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }
        }
        // UPDATE SIGNED IN TABLE  WITH STEPLEVEL AND PROCESS NO And Process Status
        //
        public void UpdateSignedInTableStepLevelAndOther(int InSignRecordNo)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE UsersSignedInActivity SET "
                            + "StepLevel = @StepLevel, ProcessNo = @ProcessNo,ProcessStatus = @ProcessStatus, "
                            + "ReplStep1_Updated = @ReplStep1_Updated, ReplStep2_Updated = @ReplStep2_Updated, ReplStep3_Updated = @ReplStep3_Updated, "
                            + "ReplStep4_Updated = @ReplStep4_Updated, ReplStep5_Updated = @ReplStep5_Updated, "
                            + "WFieldNumeric1 = @WFieldNumeric1, WFieldNumeric11 = @WFieldNumeric11, WFieldNumeric12 = @WFieldNumeric12, "
                            + "WFieldChar1 = @WFieldChar1,WFieldChar2 = @WFieldChar2, "
                            + "ReconcDifferenceStatus = @ReconcDifferenceStatus , GeneralUsedComment = @GeneralUsedComment"
                            + " WHERE SignRecordNo = @SignRecordNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SignRecordNo", InSignRecordNo);
                        cmd.Parameters.AddWithValue("@StepLevel", StepLevel);
                        cmd.Parameters.AddWithValue("@ProcessNo", ProcessNo);
                        cmd.Parameters.AddWithValue("@ProcessStatus", ProcessStatus);

                        cmd.Parameters.AddWithValue("@ReplStep1_Updated", ReplStep1_Updated);
                        cmd.Parameters.AddWithValue("@ReplStep2_Updated", ReplStep2_Updated);
                        cmd.Parameters.AddWithValue("@ReplStep3_Updated", ReplStep3_Updated);
                        cmd.Parameters.AddWithValue("@ReplStep4_Updated", ReplStep4_Updated);
                        cmd.Parameters.AddWithValue("@ReplStep5_Updated", ReplStep5_Updated);

                        cmd.Parameters.AddWithValue("@WFieldNumeric1", WFieldNumeric1);
                        cmd.Parameters.AddWithValue("@WFieldNumeric11", WFieldNumeric11);
                        cmd.Parameters.AddWithValue("@WFieldNumeric12", WFieldNumeric12);

                        cmd.Parameters.AddWithValue("@WFieldChar1", WFieldChar1);
                        cmd.Parameters.AddWithValue("@WFieldChar2", WFieldChar2);

                        cmd.Parameters.AddWithValue("@ReconcDifferenceStatus", ReconcDifferenceStatus);

                        cmd.Parameters.AddWithValue("@GeneralUsedComment", GeneralUsedComment);

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
                    ErrorOutput = "An error occured in UsersAndSignedRecord Class............. " + ex.Message;
                }
        }
        //
// Generate Password 
        //
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}



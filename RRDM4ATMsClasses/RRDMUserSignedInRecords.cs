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
    public class RRDMUserSignedInRecords : Logger
    {
        public RRDMUserSignedInRecords() : base() { }
        // Declarations 

        // SiGNED IN OCCURANCE VARIABLES 
        public int SignRecordNo;
        public string UserId;
        public string UserName;
        public string SecLevel;
        public string Culture;

        public bool SignInStatus;

        public string SignInApplication; 

        public DateTime DtTmIn;
        public DateTime DtTmOut;

        public bool ATMS_Reconciliation;
        public bool CARDS_Settlement;
        public bool E_FIN_Reconciliation;
        public bool NOSTRO_Reconciliation;
        public bool SWITCH_Reconciliation;

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

        public bool ReplStep6_Updated;
        public bool ReplStep7_Updated;
        public bool ReplStep8_Updated;

        public int ReconcDifferenceStatus;
        public int WFieldNumeric1;
        public int WFieldNumeric11;
        public int WFieldNumeric12;
        public string WFieldChar1;
        public string WFieldChar2;
        public string GeneralUsedComment;
      
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data tables 
        public DataTable DisputeUsersSelected = new DataTable();

        public DataTable OpenUsersSelected = new DataTable();

        public DataTable UsersInDataTable = new DataTable();

        public int TotalSelected;
        string SqlString; // Do not delete

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMMatchingCategories Rc = new RRDMMatchingCategories();
        readonly string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;  

        // READ SIGNED RECORD FIELDS 
        private void ReadSignedRecFields(SqlDataReader rdr)
        {
            SignRecordNo = (int)rdr["SignRecordNo"];
            UserId = (string)rdr["UserId"];
            UserName = rdr["UserName"].ToString();
            SecLevel = (string)rdr["SecLevel"];

            Culture = rdr["Culture"].ToString();

            SignInStatus = (bool)rdr["SignInStatus"];

            SignInApplication = (string)rdr["SignInApplication"];

            DtTmIn = (DateTime)rdr["DtTmIn"];
            DtTmOut = (DateTime)rdr["DtTmOut"];

            ATMS_Reconciliation = (bool)rdr["ATMS_Reconciliation"];
            CARDS_Settlement = (bool)rdr["CARDS_Settlement"];
            E_FIN_Reconciliation = (bool)rdr["E_FIN_Reconciliation"];
            NOSTRO_Reconciliation = (bool)rdr["NOSTRO_Reconciliation"];
            SWITCH_Reconciliation = (bool)rdr["SWITCH_Reconciliation"];

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
            ReplStep6_Updated = (bool)rdr["ReplStep6_Updated"];
            ReplStep7_Updated = (bool)rdr["ReplStep7_Updated"];
            ReplStep8_Updated = (bool)rdr["ReplStep8_Updated"];

            WFieldNumeric1 = (int)rdr["WFieldNumeric1"];
            WFieldNumeric11 = (int)rdr["WFieldNumeric11"];
            WFieldNumeric12 = (int)rdr["WFieldNumeric12"];

            WFieldChar1 = (string)rdr["WFieldChar1"];
            WFieldChar2 = (string)rdr["WFieldChar2"];

            ReconcDifferenceStatus = (int)rdr["ReconcDifferenceStatus"];

            GeneralUsedComment = (string)rdr["GeneralUsedComment"];
        }

        // Methods 
        // READ And Find Open Users 
        // FILL UP A TABLE 
        public void ReadOpenUsers(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            OpenUsersSelected = new DataTable();
            OpenUsersSelected.Clear();

            //TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            //OpenUsersSelected.Columns.Add("UserId", typeof(string));
            //OpenUsersSelected.Columns.Add("UserName", typeof(string));
            //OpenUsersSelected.Columns.Add("DtTmIn", typeof(string));
            //OpenUsersSelected.Columns.Add("DtTmOut", typeof(string));
            //OpenUsersSelected.Columns.Add("SecLevel", typeof(string));
            //OpenUsersSelected.Columns.Add("SignInStatus", typeof(string));

            SqlString = "  WITH OpenUsers AS "
                       + " (  "
                       + "  SELECT UserId, UserName, DtTmIn, DtTmOut, SecLevel, SignInStatus "
                       + " , ROW_NUMBER() OVER(PARTITION BY Userid ORDER BY SignRecordNo DESC) AS rn "
                       + " FROM[ATMS].[dbo].[UsersSignedInActivity] "
                       + " ) "
                       + "  SELECT * FROM OpenUsers WHERE rn = 1 "
                       + " AND DtTmIn = DtTmOut "
                     + " AND SecLevel<> '08' ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(OpenUsersSelected);

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
  

        // READ User Signed In Latest Activity 

        public void ReadSignedActivity(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[UsersSignedInActivity] "
          + " WHERE UserId = @UserId "
          + " ORDER By SignRecordNo DESC  ";
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

                            // Read Signed Record Details
                            ReadSignedRecFields(rdr);

                            break; 

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
        // READ User Signed by record number not user  
        //
        public void ReadSignedActivityByKey(int InSignRecordNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[UsersSignedInActivity] "
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

                            // Read Signed Record Details
                            ReadSignedRecFields(rdr);

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

        // Get Operator from SignedIn Record   

        public void GetOperator(int InSignRecordNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
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


                            // Read Signed Record Details
                            ReadSignedRecFields(rdr);

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
        // Insert USER SIGNED ACTIVITY 
        //
        public int InsertSignedActivity(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";
            //
            string cmdinsert = "INSERT INTO [UsersSignedInActivity]"
                + " ([UserId],[UserName],[SecLevel],[Culture],[SignInStatus],[SignInApplication],[DtTmIn],[DtTmOut],"
                + "[ATMS_Reconciliation],[CARDS_Settlement],[E_FIN_Reconciliation],[NOSTRO_Reconciliation],[SWITCH_Reconciliation],"
                + "[Replenishment],[Reconciliation],[OtherActivity],[StepLevel],[ProcessNo],[ProcessStatus] ,[WFieldNumeric11] )"
                + " VALUES (@UserId,@UserName,@SecLevel,@Culture,@SignInStatus,@SignInApplication, @DtTmIn,@DtTmOut,"
                 + "@ATMS_Reconciliation,@CARDS_Settlement,@E_FIN_Reconciliation,@NOSTRO_Reconciliation,@SWITCH_Reconciliation,"
                + "@Replenishment,@Reconciliation,@OtherActivity,@StepLevel,@ProcessNo,@ProcessStatus,@WFieldNumeric11)"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

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

                        cmd.Parameters.AddWithValue("@SignInApplication", SignInApplication);

                        cmd.Parameters.AddWithValue("@DtTmIn", DtTmIn);
                        cmd.Parameters.AddWithValue("@DtTmOut", DtTmOut);

                        cmd.Parameters.AddWithValue("@ATMS_Reconciliation", ATMS_Reconciliation);
                        cmd.Parameters.AddWithValue("@CARDS_Settlement", CARDS_Settlement);
                        cmd.Parameters.AddWithValue("@E_FIN_Reconciliation", E_FIN_Reconciliation);
                        cmd.Parameters.AddWithValue("@NOSTRO_Reconciliation", NOSTRO_Reconciliation);
                        cmd.Parameters.AddWithValue("@SWITCH_Reconciliation", SWITCH_Reconciliation);

                        cmd.Parameters.AddWithValue("@Replenishment", Replenishment);
                        cmd.Parameters.AddWithValue("@Reconciliation", Reconciliation);
                        cmd.Parameters.AddWithValue("@OtherActivity", OtherActivity);
                        cmd.Parameters.AddWithValue("@StepLevel", 0);
                        cmd.Parameters.AddWithValue("@ProcessNo", 0);
                        cmd.Parameters.AddWithValue("@ProcessStatus", 0);
                        cmd.Parameters.AddWithValue("@WFieldNumeric11", WFieldNumeric11 );


                        SignRecordNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return (SignRecordNo);
        }

        //
        // Insert USER SIGNED ACTIVITY 
        //
        public int InsertUnsuccesfulAttempts(string InUserId,string InUserName ,int InAttemptNo, bool InIsMaxAllowed)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int UnsRecordNo = 0; 
            //
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[UsersUnsuccesfulSignedInActivity]"
                + " ([UserId],[UserName],[DtTmOfAttempt],[AttemptNo],"
                + "[IsMaxAllowed]  )"
                + " VALUES (@UserId,@UserName,@DtTmOfAttempt,@AttemptNo,"
                + " @IsMaxAllowed)"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@UserName", InUserName);
                        cmd.Parameters.AddWithValue("@DtTmOfAttempt", DateTime.Now);

                        cmd.Parameters.AddWithValue("@AttemptNo", InAttemptNo);

                        cmd.Parameters.AddWithValue("@IsMaxAllowed", InIsMaxAllowed);


                        UnsRecordNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return (UnsRecordNo);
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
                    CatchDetails(ex);
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
                                + "ReplStep6_Updated = @ReplStep6_Updated,"
                                +" ReplStep7_Updated = @ReplStep7_Updated, "
                                 + " ReplStep8_Updated = @ReplStep8_Updated, "
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

                        cmd.Parameters.AddWithValue("@ReplStep6_Updated", ReplStep6_Updated);
                        cmd.Parameters.AddWithValue("@ReplStep7_Updated", ReplStep7_Updated);
                        cmd.Parameters.AddWithValue("@ReplStep8_Updated", ReplStep8_Updated);

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

                    CatchDetails(ex);
                }
        }
     


    }
}

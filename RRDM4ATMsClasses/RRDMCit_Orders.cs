using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
////using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    class RRDMCit_Orders
    {
//        OrderNo	int	Unchecked
//OrderDate	datetime	Unchecked
//NeedPriority	nvarchar(50)	Unchecked
//AtmNo	nvarchar(20)	Unchecked
//AtmName	nvarchar(100)	Unchecked
//BankId	nvarchar(8)	Unchecked
//RespBranch	nvarchar(20)	Unchecked
//BranchName	nvarchar(100)	Unchecked
//Street	nvarchar(200)	Unchecked
//Town	nvarchar(100)	Unchecked
//District	nvarchar(100)	Unchecked
//PostalCode	nvarchar(100)	Unchecked
//Country	nvarchar(100)	Unchecked
//Latitude	float	Unchecked
//Longitude	float	Unchecked
//CassetteType	nvarchar(50)	Unchecked
//CurrNm	nvarchar(3)	Unchecked
//NeedType	int	Unchecked
//NewAmount	decimal(10, 2)	Unchecked
//CassetteOneNotes	int	Unchecked
//CassetteTwoNotes	int	Unchecked
//CassetteThreeNotes	int	Unchecked
//CassetteFourNotes	int	Unchecked
//RefActionNo	int	Unchecked
//OwnerUser	nvarchar(50)	Unchecked
//CitId	nvarchar(50)	Unchecked
//CassetteId_1	nvarchar(50)	Unchecked
//CassetteId_2	nvarchar(50)	Unchecked
//CassetteId_3	nvarchar(50)	Unchecked
//CassetteId_4	nvarchar(50)	Unchecked
//RouteId	nvarchar(50)	Unchecked
//CarId	nvarchar(50)	Unchecked
//CassettesLoaded	bit	Unchecked
//CassettesInCar	bit	Unchecked
//CassettesInAtm	bit	Unchecked
//InactivateComment	nvarchar(300)	Unchecked
//Operator	nvarchar(8)	Unchecked
        // Variables
        // ATM MAIN FIELDS 
        public string AtmNo;
        public int CurrentSesNo;
        public string AtmName;
        public string BankId;

        public string RespBranch;
        public string BranchName;
        public DateTime LastReplDt;
        public DateTime NextReplDt;

        public bool ReconcDiff;
        public bool MoreMaxCash;
        public bool LessMinCash;
        public int NeedType;

        public decimal CurrCassettes;
        public decimal CurrentDeposits;
        public DateTime EstReplDt;

        public string CitId;
        public DateTime LastUpdated;
        public string AuthUser;
        public int ActionNo;
        public DateTime LastDispensedHistor;
        public DateTime LastInNeedReview;

        public int SessionsInDiff;
        public int ErrOutstanding;
        public int ReplCycleNo;
        public int ReconcCycleNo;
        public DateTime ReconcDt;

        public string CurrNm1;
        public decimal DiffCurr1;

        public int ProcessMode;

        public int AtmsReconcGroup;

        public string Operator;

        // Define the data table 
        public DataTable ATMsMainSelected = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;


        RRDMDepositsClass Dc = new RRDMDepositsClass();
        //     ReplDatesCalc Rc = new ReplDatesCalc(); // Locate next Replenishment 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // Methods 
        // READ ATMs Main For ATM 
        // 
        public void ReadAtmsMainSpecific(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
              + " FROM [dbo].[AtmsMain] "
              + " WHERE AtmNo=@AtmNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            AtmNo = (string)rdr["AtmNo"];
                            CurrentSesNo = (int)rdr["CurrentSesNo"];

                            AtmName = (string)rdr["AtmName"];

                            BankId = (string)rdr["BankId"];
                            RespBranch = (string)rdr["RespBranch"];
                            BranchName = (string)rdr["BranchName"];

                            LastReplDt = (DateTime)rdr["LastReplDt"];

                            NextReplDt = (DateTime)rdr["NextReplDt"];

                            ReconcDiff = (bool)rdr["ReconcDiff"];
                            MoreMaxCash = (bool)rdr["MoreMaxCash"];
                            LessMinCash = (bool)rdr["LessMinCash"];
                            NeedType = (int)rdr["NeedType"];

                            CurrCassettes = (decimal)rdr["CurrCassettes"];
                            CurrentDeposits = (decimal)rdr["CurrentDeposits"];
                            EstReplDt = (DateTime)rdr["EstReplDt"];

                            CitId = (String)rdr["CitId"];

                            LastUpdated = (DateTime)rdr["LastUpdated"];

                            AuthUser = (string)rdr["AuthUser"];

                            ActionNo = (int)rdr["ActionNo"];

                            LastDispensedHistor = (DateTime)rdr["LastDispensedHistor"];

                            LastInNeedReview = (DateTime)rdr["LastInNeedReview"];

                            SessionsInDiff = (int)rdr["SessionsInDiff"];
                            ErrOutstanding = (int)rdr["ErrOutstanding"];
                            ReplCycleNo = (int)rdr["ReplCycleNo"];
                            ReconcCycleNo = (int)rdr["ReconcCycleNo"];
                            ReconcDt = (DateTime)rdr["ReconcDt"];

                            CurrNm1 = (string)rdr["CurrNm1"];
                            DiffCurr1 = (decimal)rdr["DiffCurr1"];

                            ProcessMode = (int)rdr["ProcessMode"];

                            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];

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
                    ErrorOutput = "An error occured in ATMS Main Class............. " + ex.Message;

                }
        }

        // Methods 
        // READ ATMs Main For ALL ATMS specific authorised user 
        // FILL UP A TABLE 
        public void ReadAtmsMainForAuthUserAndFillTable(string InOperator, string InAuthUser)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ATMsMainSelected = new DataTable();
            ATMsMainSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsMainSelected.Columns.Add("AtmNo", typeof(string));
            ATMsMainSelected.Columns.Add("ReplCycle", typeof(string));
            ATMsMainSelected.Columns.Add("AtmName", typeof(string));
            ATMsMainSelected.Columns.Add("RespBranch", typeof(string));
            ATMsMainSelected.Columns.Add("AuthUser", typeof(string));

            SqlString = "SELECT *"
                     + " FROM [dbo].[AtmsMain] "
                     + " WHERE Operator=@Operator AND AuthUser=@AuthUser ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AuthUser", InAuthUser);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                            DataRow RowSelected = ATMsMainSelected.NewRow();

                            RowSelected["AtmNo"] = (string)rdr["AtmNo"];

                            RowSelected["ReplCycle"] = (int)rdr["CurrentSesNo"];
                            RowSelected["AtmName"] = (string)rdr["AtmName"];
                            RowSelected["RespBranch"] = (string)rdr["RespBranch"];
                            RowSelected["AuthUser"] = (string)rdr["AuthUser"];

                            // ADD ROW
                            ATMsMainSelected.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ATMS Main Class............. " + ex.Message;

                }
        }
        // 
        // UPDATE ATMs Main 
        //
        public void UpdateAtmsMain(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE AtmsMain SET "
                            + "[AtmNo] =@AtmNo,[CurrentSesNo] = @CurrentSesNo,[AtmName]=@AtmName,"
                            + "[BankId] = @BankId,[RespBranch] = @RespBranch,[BranchName] = @BranchName,"
                            + " [LastReplDt] = @LastReplDt, "
                            + " [NextReplDt] = @NextReplDt, "
                             + "[ReconcDiff] = @ReconcDiff,[MoreMaxCash] = @MoreMaxCash,[LessMinCash] = @LessMinCash,[NeedType] = @NeedType,"
                              + "[CurrCassettes] = @CurrCassettes,[CurrentDeposits] = @CurrentDeposits,[EstReplDt] = @EstReplDt,"
                              + "[CitId] = @CitId,"
                            + "[LastUpdated] = @LastUpdated,[AuthUser] = @AuthUser,[ActionNo] = @ActionNo,[LastDispensedHistor] = @LastDispensedHistor,[LastInNeedReview] = @LastInNeedReview, "
                             + "[SessionsInDiff] = @SessionsInDiff,[ErrOutstanding] = @ErrOutstanding,[ReplCycleNo] = @ReplCycleNo, "
                              + "[ReconcCycleNo] = @ReconcCycleNo,[ReconcDt] = @ReconcDt, "
                               + "[CurrNm1] = @CurrNm1,[DiffCurr1] = @DiffCurr1,[ProcessMode] = @ProcessMode,[AtmsReconcGroup] = @AtmsReconcGroup  "
                            + " WHERE AtmNo= @AtmNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@CurrentSesNo", CurrentSesNo);

                        cmd.Parameters.AddWithValue("@AtmName", AtmName);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@RespBranch", RespBranch);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);

                        cmd.Parameters.AddWithValue("@LastReplDt", LastReplDt);

                        cmd.Parameters.AddWithValue("@NextReplDt", NextReplDt);

                        cmd.Parameters.AddWithValue("@ReconcDiff", ReconcDiff);
                        cmd.Parameters.AddWithValue("@MoreMaxCash", MoreMaxCash);
                        cmd.Parameters.AddWithValue("@LessMinCash", LessMinCash);
                        cmd.Parameters.AddWithValue("@NeedType", NeedType);

                        cmd.Parameters.AddWithValue("@CurrCassettes", CurrCassettes);
                        cmd.Parameters.AddWithValue("@CurrentDeposits", CurrentDeposits);
                        cmd.Parameters.AddWithValue("@EstReplDt", EstReplDt);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@LastUpdated", LastUpdated);

                        cmd.Parameters.AddWithValue("@AuthUser", AuthUser);

                        cmd.Parameters.AddWithValue("@ActionNo", ActionNo);

                        cmd.Parameters.AddWithValue("@LastDispensedHistor", LastDispensedHistor);

                        cmd.Parameters.AddWithValue("@LastInNeedReview", LastInNeedReview);

                        cmd.Parameters.AddWithValue("@SessionsInDiff", SessionsInDiff);
                        cmd.Parameters.AddWithValue("@ErrOutstanding", ErrOutstanding);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);
                        cmd.Parameters.AddWithValue("@ReconcCycleNo", ReconcCycleNo);
                        cmd.Parameters.AddWithValue("@ReconcDt", ReconcDt);

                        cmd.Parameters.AddWithValue("@CurrNm1", CurrNm1);
                        cmd.Parameters.AddWithValue("@DiffCurr1", DiffCurr1);

                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);

                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ATMS Main Class............. " + ex.Message;
                }

            //  return outcome;

        }
        //
        // INSERT New Record in Main Table 
        //
        public void InsertInAtmsMain(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [AtmsMain] ([AtmNo],[CurrentSesNo],[AtmName],"
                + "[BankId],[RespBranch],[BranchName],"
                + "[LastReplDt],[NextReplDt],"
                 + "[ReconcDiff],[MoreMaxCash], [LessMinCash],[NeedType],"
                  + "[CurrCassettes],[CurrentDeposits],[EstReplDt],"
                + "[CitId],[LastUpdated],[AuthUser],[ActionNo], [AtmsReconcGroup], [Operator])"
                + " VALUES (@AtmNo, @CurrentSesNo,@AtmName,"
                + "@BankId,@RespBranch,@BranchName,"
                + "@LastReplDt,@NextReplDt,"
                 + "@ReconcDiff,@MoreMaxCash, @LessMinCash,@NeedType,"
                  + "@CurrCassettes,@CurrentDeposits,@EstReplDt,"
                + "@CitId,@LastUpdated,@AuthUser,@ActionNo, @AtmsReconcGroup, @Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@CurrentSesNo", 0);

                        cmd.Parameters.AddWithValue("@AtmName", AtmName);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@RespBranch", RespBranch);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);

                        cmd.Parameters.AddWithValue("@LastReplDt", DateTime.Now);

                        cmd.Parameters.AddWithValue("@NextReplDt", NextReplDt);

                        cmd.Parameters.AddWithValue("@CurrCassettes", CurrCassettes);
                        cmd.Parameters.AddWithValue("@CurrentDeposits", CurrentDeposits);
                        cmd.Parameters.AddWithValue("@EstReplDt", EstReplDt);

                        cmd.Parameters.AddWithValue("@ReconcDiff", 0);
                        cmd.Parameters.AddWithValue("@MoreMaxCash", 0);
                        cmd.Parameters.AddWithValue("@LessMinCash", 0);
                        cmd.Parameters.AddWithValue("@NeedType", 0);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@LastUpdated", LastUpdated);

                        //    cmd.Parameters.AddWithValue("@SequenceChangeDateA", DateTime.Today);
                        //    cmd.Parameters.AddWithValue("@SequenceChangeDateH", DateTime.Today);

                        cmd.Parameters.AddWithValue("@AuthUser", "");

                        cmd.Parameters.AddWithValue("@ActionNo", 0);

                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ATMS Main Class............. " + ex.Message;

                }
        }
    }
}

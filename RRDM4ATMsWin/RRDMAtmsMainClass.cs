using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    class RRDMAtmsMainClass
    {
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

        string SqlString; // Do not delete

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

       
        RRDMDepositsClass Dc = new RRDMDepositsClass();
   //     ReplDatesCalc Rc = new ReplDatesCalc(); // Locate next Replenishment 

        DateTime NullPastDate = new DateTime(1950, 11, 21);
 
        // Methods 
        // READ ATMs Main
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
                        cmd.Parameters.AddWithValue("@CurrentSesNo", 0 );

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

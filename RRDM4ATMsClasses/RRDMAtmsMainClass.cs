using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMAtmsMainClass
    {

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

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

        public bool UnderReconcMode;  

        public int AtmsReconcGroup;
        public bool ExpressAction; 

        public string Maker; // Used for reconciliation purposes 

        public string Authoriser; // Used for Authorisation purposes 

        public string Operator;

        // Counters 

        public int ExpressTotal;
        public int UnMatchedTotal;
        public int NoDecisionTotal;
        public int MakerActionsTotal;
        public int AuthNoDecisTotal;

        public int AuthorisedTotal;
        public int RejectedTotal;

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

        //RRDMNotesBalances Na = new RRDMNotesBalances(); 
   

        DateTime NullPastDate = new DateTime(1900, 01, 01);


 // Methods 
 // READ ATMs Main For ALL ATMS specific authorised user 
 // FILL UP A TABLE 

        public void ReadAtmsMainForAuthUserAndFillTableForBulk(string InSelectionCriteria, DateTime InToday, bool InWithDate, int InMode)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ExpressTotal = 0 ;
            UnMatchedTotal = 0 ;
            NoDecisionTotal = 0 ;
            MakerActionsTotal = 0 ;

            AuthorisedTotal = 0 ;
            RejectedTotal = 0 ;
            AuthNoDecisTotal = 0; 

            ATMsMainSelected = new DataTable();
            ATMsMainSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsMainSelected.Columns.Add("AtmNo", typeof(string));
            ATMsMainSelected.Columns.Add("ReplCycle", typeof(int));
            ATMsMainSelected.Columns.Add("AtmName", typeof(string));
            ATMsMainSelected.Columns.Add("RespBranch", typeof(string));
          
            if (InMode == 11)
            {
                ATMsMainSelected.Columns.Add("Maker", typeof(string));
                ATMsMainSelected.Columns.Add("Authoriser", typeof(string));
            }

            string SqlString = "SELECT *"
                       + " FROM [dbo].[AtmsMain]  "
                       + " WHERE " + InSelectionCriteria; 


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        if (InWithDate == true)
                        {
                            cmd.Parameters.AddWithValue("@Today", InToday);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                            

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycleNo = (int)rdr["ReplCycleNo"];

                            ExpressAction = (bool)rdr["ExpressAction"];

                            Maker = (string)rdr["Maker"];
                            Authoriser = (string)rdr["Authoriser"];

                            // TOTALS
                            if (Maker == "UnMatched") UnMatchedTotal = UnMatchedTotal + 1;

                            if (Maker == "No Decision") NoDecisionTotal = NoDecisionTotal + 1;

                            if (Maker == "Express") ExpressTotal = ExpressTotal + 1;
                            
                            if (Maker == "Maker Actions") MakerActionsTotal = MakerActionsTotal + 1;

                            if (Authoriser == "Authorised") AuthorisedTotal = AuthorisedTotal + 1;
                            if (Authoriser == "Rejected") RejectedTotal = RejectedTotal + 1 ;

                            if (Authoriser == "No Decision") AuthNoDecisTotal = AuthNoDecisTotal + 1; 
                         
                                DataRow RowSelected = ATMsMainSelected.NewRow();

                                RowSelected["AtmNo"] = (string)rdr["AtmNo"];

                                RowSelected["ReplCycle"] = (int)rdr["ReplCycleNo"];
                                RowSelected["AtmName"] = (string)rdr["AtmName"];
                                RowSelected["RespBranch"] = (string)rdr["RespBranch"];
                                //RowSelected["User"] = (string)rdr["AuthUser"];

                                RowSelected["Maker"] = Maker;
                                RowSelected["Authoriser"] = Authoriser;

                                // ADD ROW
                                ATMsMainSelected.Rows.Add(RowSelected);
                                       

                        }

                        // Close Reader
                        rdr.Close();
                    }


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

        public void ReadAtmsMainForAuthUserAndFillTable(string InOperator, string InAuthUser, string InAtmNo)
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

            //if (InMode == 11)
            //{
            //    ATMsMainSelected.Columns.Add("Action", typeof(bool));
            //}

            if (InAtmNo == "")
            {
                SqlString = "SELECT *"
                     + " FROM [dbo].[AtmsMain] "
                     + " WHERE Operator=@Operator AND AuthUser=@AuthUser ";

                //+ " WHERE Operator=@Operator ";
            }

            if (InAtmNo != "")
            {
                SqlString = "SELECT *"
                     + " FROM [dbo].[AtmsMain] "
                     + " WHERE Operator=@Operator AND AuthUser=@AuthUser AND AtmNo=@AtmNo ";
            }



            //SqlString = "SELECT *"
            //     + " FROM [dbo].[AtmsMain] "
            //     + " WHERE Operator=@Operator ";

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

                        if (InAtmNo != "")
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        }

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

                            UnderReconcMode = (bool)rdr["UnderReconcMode"];

                            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];

                            ExpressAction = (bool)rdr["ExpressAction"];

                            Maker = (string)rdr["Maker"];
                            Authoriser = (string)rdr["Authoriser"];

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
                            + "[CurrNm1] = @CurrNm1,[DiffCurr1] = @DiffCurr1,[ProcessMode] = @ProcessMode, [UnderReconcMode] = @UnderReconcMode,"
                            + "[ExpressAction] = @ExpressAction,[Maker] = @Maker,[Authoriser] = @Authoriser,"
                            + "[AtmsReconcGroup] = @AtmsReconcGroup  "
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

                        cmd.Parameters.AddWithValue("@UnderReconcMode", UnderReconcMode);

                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@ExpressAction", ExpressAction);

                        cmd.Parameters.AddWithValue("@Maker", Maker);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        
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

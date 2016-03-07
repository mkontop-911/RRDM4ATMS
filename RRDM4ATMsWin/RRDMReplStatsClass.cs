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
    class RRDMReplStatsClass
    {
        // Variables
        // ATM MAIN FIELDS 

         public int RecordSeq;
         public string AtmNo;
         public string UserId;
         public string UserName;

         public int ReplDtRev;
         public DateTime ReplDate; 
         public string BankId;

         public int AtmGroup;
         public int ReplGroup;
         public int ReconcGroup;

         public int ReplCycleNo;

         public int ReplMinutes;
         public int OtherNop;

         public decimal InMoneyLast;
         public decimal RemainMoney;
         public decimal InMoneyNew;
                            
         public int ErrorsAtm;
         public int NoAtmOutst;
         public int ErrorsHost;

         public int NoHostOut;

         public string CurName;
         public decimal DiffPlus;
         public decimal DiffMinus;

         public int NotReconc;
         public string CitId;

         public string Operator;

         public int MinReplMinutes;
         public int MaxReplMinutes;
         public int AvgReplMinutes;

         public int TotalReplMinutes; // Total for a specific ATM

         string SqlString; // Do not delete

         public bool RecordFound;
         public bool ErrorFound;
         public string ErrorOutput; 

        // DATATable for Grid 
        // public DataTable GridDays = new DataTable();

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

   //     NotesBalances Na = new NotesBalances();
    //    DepositsClass Dc = new DepositsClass();
        //     ReplDatesCalc Rc = new ReplDatesCalc(); // Locate next Replenishment 

        DateTime NullPastDate = new DateTime(1950, 11, 21);

     //   string outcome = ""; // TO FACILITATE EXCEPTIONS 

        // Methods 
        // READ ReplStatClass MIN and MAX 
        // 

        public void ReadReplStatClassMinAndMax(DateTime InDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT ReplDate, MIN(ReplMinutes) AS MinReplMinutes , MAX(ReplMinutes) AS MaxReplMinutes, AVG (ReplMinutes) AS AvgReplMinutes "
          + " FROM [dbo].[ReplStatsTable] "
          + " WHERE ReplDate=@ReplDate "
           + " GROUP BY ReplDate";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                      cmd.Parameters.AddWithValue("@ReplDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                 //           DateTime Test1 = (DateTime)rdr["ReplDate"]; 

                            MinReplMinutes = (int)rdr["MinReplMinutes"];
                         //   MaxReplMinutes = (int)rdr["MaxReplMinutes"];
                            AvgReplMinutes = (int)rdr["AvgReplMinutes"];
                            MaxReplMinutes = (int)rdr["MaxReplMinutes"];
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
                    ErrorOutput = "An error occured in ReplStats Class............. " + ex.Message;
                }
        }


        // Methods 
        // READ ReplStatClass Specific by Date 
        // 

        public void ReadReplStatClassSpecificDt(string InAtmNo, DateTime InDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            TotalReplMinutes = 0; 

            SqlString = "SELECT *"
          + " FROM [dbo].[ReplStatsTable] "
           + " WHERE AtmNo = @AtmNo AND ReplDate=@ReplDate ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            RecordSeq = (int)rdr["RecordSeq"];
                            AtmNo = (string)rdr["AtmNo"];
                            UserId = (string)rdr["UserId"];
                            UserName = (string)rdr["UserName"];

                            ReplDtRev = (int)rdr["ReplDtRev"];

                            ReplDate = (DateTime)rdr["ReplDate"];

                            BankId = (string)rdr["BankId"];
                       

                            AtmGroup = (int)rdr["AtmGroup"];
                            ReplGroup = (int)rdr["ReplGroup"];
                            ReconcGroup = (int)rdr["ReconcGroup"];

                            ReplCycleNo = (int)rdr["ReplCycleNo"];

                            ReplMinutes = (int)rdr["ReplMinutes"];
                            OtherNop = (int)rdr["OtherNop"];

                            InMoneyLast = (decimal)rdr["InMoneyLast"];
                            RemainMoney = (decimal)rdr["RemainMoney"];
                            InMoneyNew = (decimal)rdr["InMoneyNew"];

                            ErrorsAtm = (int)rdr["ErrorsAtm"];
                            NoAtmOutst = (int)rdr["NoAtmOutst"];
                            ErrorsHost = (int)rdr["ErrorsHost"];

                            NoHostOut = (int)rdr["NoHostOut"];

                            CurName = (string)rdr["CurName"];
                            DiffPlus = (Decimal)rdr["DiffPlus"];
                            DiffMinus = (Decimal)rdr["DiffMinus"];

                            NotReconc = (int)rdr["NotReconc"];
                            CitId = (string)rdr["CitId"];
                            Operator = (string)rdr["Operator"];

                            TotalReplMinutes = TotalReplMinutes + ReplMinutes; 
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
                    ErrorOutput = "An error occured in ReplStats Class............. " + ex.Message;
                }
        }

        // Methods 
        // READ ReplStatClass Specific REPL Cycle 
        // 

        public void ReadReplStatClassSpecific(string InAtmNo, int InReplCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
          + " FROM [dbo].[ReplStatsTable] "
          + " WHERE AtmNo=@AtmNo AND ReplCycleNo =@ReplCycleNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true; 

                            RecordSeq = (int)rdr["RecordSeq"];
                            AtmNo = (string)rdr["AtmNo"];
                            UserId = (string)rdr["UserId"];
                            UserName = (string)rdr["UserName"];

                            ReplDtRev = (int)rdr["ReplDtRev"];

                            ReplDate = (DateTime)rdr["ReplDate"]; 

                            BankId = (string)rdr["BankId"];
                    

                            AtmGroup = (int)rdr["AtmGroup"];
                            ReplGroup = (int)rdr["ReplGroup"];
                            ReconcGroup = (int)rdr["ReconcGroup"];

                            ReplCycleNo = (int)rdr["ReplCycleNo"];

                            ReplMinutes = (int)rdr["ReplMinutes"];
                            OtherNop = (int)rdr["OtherNop"];
 
                            InMoneyLast = (decimal)rdr["InMoneyLast"];
                            RemainMoney = (decimal)rdr["RemainMoney"];
                            InMoneyNew = (decimal)rdr["InMoneyNew"];
                            
                            ErrorsAtm = (int)rdr["ErrorsAtm"];
                            NoAtmOutst = (int)rdr["NoAtmOutst"];
                            ErrorsHost = (int)rdr["ErrorsHost"];

                            NoHostOut = (int)rdr["NoHostOut"];

                            CurName = (string)rdr["CurName"];
                            DiffPlus = (Decimal)rdr["DiffPlus"];
                            DiffMinus = (Decimal)rdr["DiffMinus"];

                            NotReconc = (int)rdr["NotReconc"];
                            CitId = (string)rdr["CitId"];
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
                    ErrorOutput = "An error occured in ReplStats Class............. " + ex.Message;
                }
        }
        // 
        // UPDATE ReplStat
        //
        public void UpdateReplStatClass(string InAtmNo, int InReplCycleNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

           SqlString = 
            "UPDATE [dbo].[ReplStatsTable]"
                            + " SET [AtmNo] = @AtmNo, [UserId] = @UserId,[UserName] = @UserName,"
                            + " [ReplDtRev] = @ReplDtRev, [ReplDate] = @ReplDate,[BankId] = @BankId, " 
                            + "[AtmGroup] = @AtmGroup, [ReplGroup] = @ReplGroup, [ReconcGroup] = @ReconcGroup," 
                            + " [ReplCycleNo] = @ReplCycleNo, [ReplMinutes] = @ReplMinutes, [OtherNop] = @OtherNop,"
                            + " [InMoneyLast] = @InMoneyLast, [RemainMoney] = @RemainMoney,[InMoneyNew] = @InMoneyNew,"
                            +" [ErrorsAtm] = @ErrorsAtm, [NoAtmOutst] = @NoAtmOutst, " 
                            +"[ErrorsHost] = @ErrorsHost, [NoHostOut] = @NoHostOut, "
                            +"[CurName] = @CurName, [DiffPlus] = @DiffPlus, [DiffMinus] = @DiffMinus,"
                            + " [NotReconc] = @NotReconc,[CitId] = @CitId "
                             + " WHERE AtmNo= @AtmNo AND ReplCycleNo = @ReplCycleNo"; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        
                        cmd.Parameters.AddWithValue("@ReplDtRev", ReplDtRev);
                        cmd.Parameters.AddWithValue("@ReplDate", ReplDate);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                   

                        cmd.Parameters.AddWithValue("@AtmGroup", AtmGroup);
                        cmd.Parameters.AddWithValue("@ReplGroup", ReplGroup);
                        cmd.Parameters.AddWithValue("@ReconcGroup", ReconcGroup);

                        cmd.Parameters.AddWithValue("@ReplMinutes", ReplMinutes);
                        cmd.Parameters.AddWithValue("@OtherNop", OtherNop);

                        cmd.Parameters.AddWithValue("@InMoneyLast", InMoneyLast);
                        cmd.Parameters.AddWithValue("@RemainMoney", RemainMoney);
                        cmd.Parameters.AddWithValue("@InMoneyNew", InMoneyNew);
                                                
                        cmd.Parameters.AddWithValue("@ErrorsAtm", ErrorsAtm);
                        cmd.Parameters.AddWithValue("@NoAtmOutst", NoAtmOutst);

                        cmd.Parameters.AddWithValue("@ErrorsHost", ErrorsHost);
                        cmd.Parameters.AddWithValue("@NoHostOut", NoHostOut);

                        cmd.Parameters.AddWithValue("@CurName", CurName);
                        cmd.Parameters.AddWithValue("@DiffPlus", DiffPlus);
                        cmd.Parameters.AddWithValue("@DiffMinus", DiffMinus);

                        cmd.Parameters.AddWithValue("@NotReconc", NotReconc);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                         //   outcome = " ATMs Table UPDATED ";
                        }

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReplStats Class............. " + ex.Message;
                }

            //  return outcome;

        }
        // INSERT New Record in ReplStatsTable
        public void InsertInAtmsStatsTable(string InAtmNo, int InReplCycle)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "INSERT INTO [dbo].[ReplStatsTable] "
                + "([AtmNo], [UserId], [UserName],"
                +"[ReplDtRev],[ReplDate], [BankId], " 
                + "[AtmGroup], [ReplGroup], [ReconcGroup], [ReplCycleNo],"
                + " [ReplMinutes], [OtherNop], [InMoneyLast], [RemainMoney], [InMoneyNew],"
                + "[ErrorsAtm], [NoAtmOutst], [ErrorsHost], [NoHostOut],"
                + " [CurName], [DiffPlus], [DiffMinus], [NotReconc], [CitId] , [Operator])"
                + " VALUES ("
                + " @AtmNo, @UserId, @UserName, @ReplDtRev, @ReplDate, @BankId, "
                + " @AtmGroup, @ReplGroup, @ReconcGroup, @ReplCycleNo,"
                + " @ReplMinutes, @OtherNop, @InMoneyLast, @RemainMoney,@InMoneyNew,"
                + " @ErrorsAtm, @NoAtmOutst, @ErrorsHost,"
                + "@NoHostOut, @CurName, @DiffPlus, @DiffMinus, @NotReconc , @CitId , @Operator)";
                
          
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@UserName", UserName);

                        cmd.Parameters.AddWithValue("@ReplDtRev", ReplDtRev);
                        cmd.Parameters.AddWithValue("@ReplDate", ReplDate);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                
                        cmd.Parameters.AddWithValue("@AtmGroup", AtmGroup);
                        cmd.Parameters.AddWithValue("@ReplGroup", ReplGroup);
                        cmd.Parameters.AddWithValue("@ReconcGroup", ReconcGroup);

                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@ReplMinutes", ReplMinutes);
                        cmd.Parameters.AddWithValue("@OtherNop", OtherNop);

                        cmd.Parameters.AddWithValue("@InMoneyLast", InMoneyLast);
                        cmd.Parameters.AddWithValue("@RemainMoney", RemainMoney);
                        cmd.Parameters.AddWithValue("@InMoneyNew", InMoneyNew);

                        cmd.Parameters.AddWithValue("@ErrorsAtm", ErrorsAtm);
                        cmd.Parameters.AddWithValue("@NoAtmOutst", NoAtmOutst);

                        cmd.Parameters.AddWithValue("@ErrorsHost", ErrorsHost);
                        cmd.Parameters.AddWithValue("@NoHostOut", NoHostOut);

                        cmd.Parameters.AddWithValue("@CurName", CurName);
                        cmd.Parameters.AddWithValue("@DiffPlus", DiffPlus);
                        cmd.Parameters.AddWithValue("@DiffMinus", DiffMinus);

                        cmd.Parameters.AddWithValue("@NotReconc", NotReconc);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@Operator", Operator);
                     
                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                          //  outcome = " Record Inserted ";
                        }
                     

                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReplStats Class............. " + ex.Message;
                }
        }
        //
  // Create STATS Record for this ATM and this ReplCycle
        //
        // This record is created at Repl-End in EJournal operation just after Cash In Time at 9:00 am 
        // 
        public void InsertInAtmsStats(string InOperator, string InAtmNo, int InReplCycle, DateTime InSMEnd)
        {

            RRDMAtmsClass Ac = new RRDMAtmsClass(); 
            
            RRDMNotesBalances Na = new RRDMNotesBalances();
            RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); 
            RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord ();

            Ac.ReadAtm(InAtmNo); 
            string WBankId = Ac.BankId;

            Na.ReadSessionsNotesAndValues(InAtmNo, InReplCycle, 2);

            decimal MoneyIn = Na.Balances1.OpenBal; 

            int ReplCycle = Na.PreSes;
            if (ReplCycle > 0) // It is not a new ATM therefore it has previous SesNo 
            {

                Na.ReadSessionsNotesAndValues( InAtmNo, ReplCycle, 2);

            Ta.ReadSessionsStatusTraces(InAtmNo, ReplCycle);

            Us.ReadUsersRecord(Ta.Repl1.SignIdRepl); 

            TimeSpan SM_Duration = InSMEnd - Ta.SesDtTimeEnd;

            // Now we have available all fields from Na and Ta
            // Also we have the time SM - end 

            AtmNo = InAtmNo;
            UserId = Ta.Repl1.SignIdRepl;

            UserName = Us.UserName;

            ReplDtRev = 0 ;

            ReplDate = Ta.SesDtTimeEnd.Date;

            BankId = Ta.BankId;
  

            AtmGroup = Ta.AtmsStatsGroup;
            ReplGroup = Ta.AtmsReplGroup;
            ReconcGroup = Ta.AtmsReconcGroup;

            ReplCycleNo = ReplCycle;

            ReplMinutes = Convert.ToInt32(SM_Duration.TotalMinutes);

            OtherNop = Ta.Stats1.NoOpMinutes;

            InMoneyLast = Na.Balances1.OpenBal;
            RemainMoney = Na.Balances1.MachineBal;
            InMoneyNew = MoneyIn;

            ErrorsAtm = Na.ErrJournalThisCycle;
            NoAtmOutst = Na.ErrOutstanding;
            ErrorsHost = Na.ErrHostToday;

            NoHostOut = Na.NumberOfErrHost; 

            CurName = Ta.Diff1.CurrNm1;
            if (Ta.Diff1.DiffCurr1 > 0) DiffPlus = Ta.Diff1.DiffCurr1;
            if (Ta.Diff1.DiffCurr1 < 0) DiffMinus = Ta.Diff1.DiffCurr1;
            
            NotReconc = Ta.SessionsInDiff;

            CitId = Us.CitId ;

            Operator = InOperator; 

            InsertInAtmsStatsTable(InAtmNo, ReplCycle);

            }
        }
    }
}

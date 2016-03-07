using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMTracesReadUpdate
    {
        // Variables
        // TRACES FIELDS 
       public int SesNo;
       public string AtmNo;
    
       public int PreSes;
       public int NextSes;
       public string AtmName;
       public string BankId;
   
       public string RespBranch;
       public int AtmsStatsGroup;
       public int AtmsReplGroup;
       public int AtmsReconcGroup;
       public DateTime SesDtTimeStart;
       public DateTime SesDtTimeEnd;
       public int FirstTraceNo;
       public int LastTraceNo;

      
       public struct Stats
       {
           public int OfflineMinutes;
           public int NoOpMinutes;
           public int CommErrNum;
           public int NoOfCustLocals;
           public int NoOfCustOther;
           public int NoOfCustForeign;
           public int NoOfTranCash;
           public int NoOfTranDepCash;
           public int NoOfTranDepCheq;
           public int NoOfCheques;
       }

       public Stats Stats1; // Declare ForStats 

       public struct Repl
       {
           public string SignIdRepl;
           public bool StartRepl;
           public bool FinishRepl;
           public int StepLevel; 
           public DateTime ReplStartDtTm;
           public DateTime ReplFinDtTm;
           public bool DiffRepl;
           public bool ErrsRepl;
           public DateTime NextRepDtTm;
        
       }
       public Repl Repl1; // Declare Repl  

       public struct Recon
       {
           public string SignIdReconc;
           public bool DelegRecon;
           public bool StartReconc;
           public bool FinishReconc;
           public DateTime RecStartDtTm;
           public DateTime RecFinDtTm;
           public bool DiffReconcStart;
           public bool DiffReconcEnd;
           public int WFlowDifStatus; 
           public bool TurboReconc;
       }
       public Recon Recon1; // Declare Recon 

        public int NumOfErrors;
        public int ErrOutstanding;

        public int BalSetsNo;


        public struct Diff
        {
  
            public string CurrNm1;
            public decimal DiffCurr1;

            public string CurrNm2;
            public decimal DiffCurr2;

            public string CurrNm3;
            public decimal DiffCurr3;

            public string CurrNm4;
            public decimal DiffCurr4;
        }
        public Diff Diff1; // Declare Diff
     
        public int SessionsInDiff;
        public int YtdInDiff;
        public int LatestBatchNo;

        public int InNeedType;

        public bool UpdatedBatch; 
        public bool Last; 
        public bool InProcess;

        public int ProcessMode;

        public string ReplGenComment; 

        public int NextReplNo;
        public int LastNo;
        public int Last_1; // LAST SESSION NUMBER 

        public int ReplOutstanding; 

        public int LastReplCyclId; // latest repl cycle 
        public int LLatestBatchNo; // Latest 
        public bool LUpdatedBatch; // Latest 

        public string Operator;

    //    bool UpdSesTraces;

        bool AutoRec;

        // Define the data table 
        public DataTable ATMsReplCyclesSelectedPeriod = new DataTable();

        public int TotalSelected; 

        // 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;


        // Methods 
        // For an ATM read its replecycles for a period 
        // FILL UP A TABLE 
        public void ReadReplCyclesForFromToDate(string InOperator, string InAtmNo, DateTime InDtFrom, DateTime InDtTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ATMsReplCyclesSelectedPeriod = new DataTable();
            ATMsReplCyclesSelectedPeriod.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsReplCyclesSelectedPeriod.Columns.Add("ReplCycle", typeof(int));
            ATMsReplCyclesSelectedPeriod.Columns.Add("CycleStart", typeof(DateTime));
            ATMsReplCyclesSelectedPeriod.Columns.Add("CycleEnd", typeof(DateTime));
            ATMsReplCyclesSelectedPeriod.Columns.Add("FirstTraceNo", typeof(int));
            ATMsReplCyclesSelectedPeriod.Columns.Add("LastTraceNo", typeof(int));
            ATMsReplCyclesSelectedPeriod.Columns.Add("AtmNo", typeof(string));

            string SqlString = "SELECT *"
                     + " FROM [dbo].[SessionsStatusTraces] "
                     + " WHERE Operator=@Operator AND AtmNo =@AtmNo AND SesDtTimeStart >= @SesDtTimeStart AND SesDtTimeEnd <= @SesDtTimeEnd  "
                     + " Order by SesNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesDtTimeStart", InDtFrom);
                        cmd.Parameters.AddWithValue("@SesDtTimeEnd", InDtTo);
                        
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                            DataRow RowSelected = ATMsReplCyclesSelectedPeriod.NewRow();

                            RowSelected["ReplCycle"] = (int)rdr["SesNo"];

                            RowSelected["CycleStart"] = (DateTime)rdr["SesDtTimeStart"];
                            RowSelected["CycleEnd"] = (DateTime)rdr["SesDtTimeEnd"];

                            RowSelected["FirstTraceNo"] = (int)rdr["FirstTraceNo"];
                            RowSelected["LastTraceNo"] = (int)rdr["LastTraceNo"];

                            RowSelected["AtmNo"] = (string)rdr["AtmNo"];

                            // ADD ROW
                            ATMsReplCyclesSelectedPeriod.Rows.Add(RowSelected);

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

      //  string outcome = ""; // TO FACILITATE EXCEPTIONS 
        // Methods 
        // READ Session Traces
        // 
 public void ReadSessionsStatusTraces(string InAtmNo, int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[SessionsStatusTraces] "
          + " WHERE AtmNo=@AtmNo AND SesNo = @SesNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true; 

                            SesNo = (int)rdr["SesNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            PreSes = (int)rdr["PreSes"]; ;
                            NextSes = (int)rdr["NextSes"]; ;
                            AtmName = (string)rdr["AtmName"];
                     
                            BankId = (string)rdr["BankId"];
                            RespBranch = (string)rdr["RespBranch"];

                            AtmsStatsGroup = (int)rdr["AtmsStatsGroup"];
                            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
                            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];

                            SesDtTimeStart = (DateTime)rdr["SesDtTimeStart"];
                            SesDtTimeEnd = (DateTime)rdr["SesDtTimeEnd"];
                            FirstTraceNo = (int)rdr["FirstTraceNo"];
                            LastTraceNo = (int)rdr["LastTraceNo"];

                            Stats1.OfflineMinutes = (int)rdr["OfflineMinutes"];
                            Stats1.NoOpMinutes = (int)rdr["NoOpMinutes"];
                            Stats1.CommErrNum = (int)rdr["CommErrNum"];
                            Stats1.NoOfCustLocals = (int)rdr["NoOfCustLocals"];
                            Stats1.NoOfCustOther = (int)rdr["NoOfCustOther"];
                            Stats1.NoOfCustForeign = (int)rdr["NoOfCustForeign"];
                            Stats1.NoOfTranCash = (int)rdr["NoOfTranCash"];
                            Stats1.NoOfTranDepCash = (int)rdr["NoOfTranDepCash"];
                            Stats1.NoOfTranDepCheq = (int)rdr["NoOfTranDepCheq"];
                            Stats1.NoOfCheques = (int)rdr["NoOfCheques"];

                      Repl1.SignIdRepl = (string)rdr["SignIdRepl"];
                      Repl1.StartRepl = (bool)rdr["StartRepl"];
                      Repl1.FinishRepl = (bool)rdr["FinishRepl"];
                      Repl1.StepLevel = (int)rdr["StepLevel"];
                      Repl1.ReplStartDtTm = (DateTime)rdr["ReplStartDtTm"];
                      Repl1.ReplFinDtTm = (DateTime)rdr["ReplFinDtTm"];
                      Repl1.DiffRepl = (bool)rdr["DiffRepl"];
                      Repl1.ErrsRepl = (bool)rdr["ErrsRepl"];
                      Repl1.NextRepDtTm = (DateTime)rdr["NextRepDtTm"];
             

                    Recon1.SignIdReconc = (string)rdr["SignIdReconc"];
                    Recon1.DelegRecon = (bool)rdr["DelegRecon"];
                    Recon1.StartReconc = (bool)rdr["StartReconc"];
                    Recon1.FinishReconc = (bool)rdr["FinishReconc"];
                    Recon1.RecStartDtTm = (DateTime)rdr["RecStartDtTm"];
                    Recon1.RecFinDtTm = (DateTime)rdr["RecFinDtTm"];
                    Recon1.DiffReconcStart = (bool)rdr["DiffReconcStart"];
                    Recon1.DiffReconcEnd = (bool)rdr["DiffReconcEnd"];
                    Recon1.WFlowDifStatus = (int)rdr["WFlowDifStatus"];
                            
                    Recon1.TurboReconc = (bool)rdr["TurboReconc"];

                    NumOfErrors = (int)rdr["NumOfErrors"];
                    ErrOutstanding = (int)rdr["ErrOutstanding"];
                    BalSetsNo = (int)rdr["BalSetsNo"];

                       
                             Diff1.CurrNm1 = (string)rdr["CurrNm1"];
                             Diff1.DiffCurr1 = (decimal)rdr["DiffCurr1"];

                       
                             Diff1.CurrNm2 = (string)rdr["CurrNm2"];
                             Diff1.DiffCurr2 = (decimal)rdr["DiffCurr2"];

                      
                             Diff1.CurrNm3 = (string)rdr["CurrNm3"];
                             Diff1.DiffCurr3 = (decimal)rdr["DiffCurr3"];

                     
                             Diff1.CurrNm4 = (string)rdr["CurrNm4"];
                             Diff1.DiffCurr4 = (decimal)rdr["DiffCurr4"];

                             SessionsInDiff = (int)rdr["SessionsInDiff"];
                             YtdInDiff = (int)rdr["YtdInDiff"];
                             LatestBatchNo = (int)rdr["LatestBatchNo"];

                             InNeedType = (int)rdr["InNeedType"];

                             UpdatedBatch = (bool)rdr["UpdatedBatch"];
                             Last = (bool)rdr["Last"];
                             InProcess = (bool)rdr["InProcess"];

                             ProcessMode = (int)rdr["ProcessMode"]; 

                             ReplGenComment = (string)rdr["ReplGenComment"];

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
                    ErrorOutput = "An error occured in TracesReadUpdate Class............. " + ex.Message;
                }
        }

    
// Find last Trace No Replenished  
 // 
 public void FindLastReplCycleId(String InAtmNo)
 {
     RecordFound = false;
     ErrorFound = false;
     ErrorOutput = ""; 

     string SqlString = "SELECT *"
         + " FROM [dbo].[SessionsStatusTraces] "
         + " WHERE AtmNo=@AtmNo AND ProcessMode >= 1" 
         + " ORDER BY SesNo ASC ";
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

                     SesNo = (int)rdr["SesNo"];
                     LatestBatchNo = (int)rdr["LatestBatchNo"];      
                     UpdatedBatch = (bool)rdr["UpdatedBatch"];
                     ProcessMode = (int)rdr["ProcessMode"];

                     if (ProcessMode >= 1)
                     {
                         LastReplCyclId = SesNo;
                         LLatestBatchNo = LatestBatchNo;
                         LUpdatedBatch = UpdatedBatch;  
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
             ErrorFound = true;
             ErrorOutput = "An error occured in TracesReadUpdate Class............. " + ex.Message;
         }
 }


 // Find Trace No Next to Replenish 
 // To locate last trace number 
 public void FindNextReplCycleId(String InAtmNo)
 {
     RecordFound = false;
     ErrorFound = false;
     ErrorOutput = ""; 

     bool First = true; 

     string SqlString = "SELECT *"
         + " FROM [dbo].[SessionsStatusTraces] "
          + " WHERE AtmNo=@AtmNo " 
   //      + " WHERE AtmNo=@AtmNo AND (ProcessMode = 0 OR ProcessMode = -1 OR ProcessMode = 1)" 
         + " ORDER BY SesNo ASC ";
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

                     ProcessMode = (int)rdr["ProcessMode"];

                     SesNo = (int)rdr["SesNo"];

                     if ( ProcessMode == 0 & First == true ) // The first 0 
                     {
                         NextReplNo = SesNo;
                         if (ProcessMode == 0) First = false; 
                     }

                     if (ProcessMode == 1) // The last 1  
                     {
                         Last_1 = SesNo;                     
                     }

                     LastNo = SesNo;

                     ReplOutstanding = ReplOutstanding + 1; 

                    
                     // THESE ARE FOR THE LAST Record
                     FirstTraceNo = (int)rdr["FirstTraceNo"];
                     LastTraceNo = (int)rdr["LastTraceNo"];


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
             ErrorOutput = "An error occured in TracesReadUpdate Class............. " + ex.Message;
         }
 }
 
        // 
        // UPDATE TRACES WITH THE FIRST AND LAST TRACE FROM REPL TO REPL 
        //
public void UpdateSessionsStatusTraces(string InAtmNo, int InSesNo)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

       //     UpdSesTraces = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsStatusTraces  SET "
               + "[AtmNo] =@AtmNo,[PreSes] = @PreSes,[NextSes] = @NextSes,[AtmName]=@AtmName,"
                            + "[BankId] = @BankId,[RespBranch] = @RespBranch,"
                            + "[AtmsStatsGroup] = @AtmsStatsGroup,[AtmsReplGroup] = @AtmsReplGroup,[AtmsReconcGroup] = @AtmsReconcGroup,"
                            + "[SesDtTimeStart]= @SesDtTimeStart,[SesDtTimeEnd] = @SesDtTimeEnd,"
                            + " [FirstTraceNo] = @FirstTraceNo,[LastTraceNo] = @LastTraceNo,"
               + " [OfflineMinutes] = @OfflineMinutes,[NoOpMinutes] = @NoOpMinutes,"
                            + " [CommErrNum] = @CommErrNum,[NoOfCustLocals] = @NoOfCustLocals,"
                            + " [NoOfCustOther] = @NoOfCustOther,[NoOfCustForeign] = @NoOfCustForeign, "
                            + " [NoOfTranCash] = @NoOfTranCash,[NoOfTranDepCash] = @NoOfTranDepCash, "
                            + " [NoOfTranDepCheq] = @NoOfTranDepCheq,[NoOfCheques] = @NoOfCheques,"
               + " [SignIdRepl] = @SignIdRepl,[StartRepl] = @StartRepl,"
                            + " [FinishRepl] = @FinishRepl,[StepLevel] = @StepLevel,[ReplStartDtTm] = @ReplStartDtTm,"
                            + " [ReplFinDtTm] = @ReplFinDtTm,[DiffRepl] = @DiffRepl," 
                            + " [ErrsRepl] = @ErrsRepl,[NextRepDtTm] = @NextRepDtTm," 
               + " [SignIdReconc] = @SignIdReconc,[DelegRecon] = @DelegRecon,"
                             + "[StartReconc] = @StartReconc,[FinishReconc] = @FinishReconc, "
                            + " [RecStartDtTm] = @RecStartDtTm,[RecFinDtTm] = @RecFinDtTm, "
                           + " [DiffReconcStart] = @DiffReconcStart,[DiffReconcEnd] = @DiffReconcEnd,[WFlowDifStatus] = @WFlowDifStatus,"
                           + " [TurboReconc] = @TurboReconc,"
                           + "[NumOfErrors] = @NumOfErrors,[ErrOutstanding] = @ErrOutstanding, "
                          + " [BalSetsNo] = @BalSetsNo,"
               + "[CurrNm1] = @CurrNm1,[DiffCurr1] = @DiffCurr1,"
                          + "[CurrNm2] = @CurrNm2,[DiffCurr2] = @DiffCurr2,"
                          + "[CurrNm3] = @CurrNm3,[DiffCurr3] = @DiffCurr3,"
                          + "[CurrNm4] = @CurrNm4,[DiffCurr4] = @DiffCurr4,"
                           + "[SessionsInDiff] = @SessionsInDiff,[YtdInDiff] = @YtdInDiff,[LatestBatchNo] = @LatestBatchNo,"
                       /*     + "[LastTrace01] = @LastTrace01,[LastTrace02] = @LastTrace02, "
                            + "[LastTrace03] = @LastTrace03,[LastTrace04] = @LastTrace04, " */
                            + "[InNeedType] = @InNeedType, "
                           + "[UpdatedBatch] = @UpdatedBatch,[Last] = @Last, [InProcess] = @InProcess,"
                           + "[ProcessMode] = @ProcessMode, "
                            + "[ReplGenComment] = @ReplGenComment "
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                        cmd.Parameters.AddWithValue("@PreSes", PreSes);
                        cmd.Parameters.AddWithValue("@NextSes", NextSes);
                        cmd.Parameters.AddWithValue("@AtmName", AtmName);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                   
                        cmd.Parameters.AddWithValue("@RespBranch", RespBranch);
                        cmd.Parameters.AddWithValue("@AtmsStatsGroup", AtmsStatsGroup);
                        cmd.Parameters.AddWithValue("@AtmsReplGroup", AtmsReplGroup);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);
                        cmd.Parameters.AddWithValue("@SesDtTimeStart", SesDtTimeStart);
                        cmd.Parameters.AddWithValue("@SesDtTimeEnd", SesDtTimeEnd);
                        cmd.Parameters.AddWithValue("@FirstTraceNo", FirstTraceNo);
                        cmd.Parameters.AddWithValue("@LastTraceNo", LastTraceNo);

                        cmd.Parameters.AddWithValue("@OfflineMinutes", Stats1.OfflineMinutes);
                        cmd.Parameters.AddWithValue("@NoOpMinutes", Stats1.NoOpMinutes);
                        cmd.Parameters.AddWithValue("@CommErrNum", Stats1.CommErrNum);
                        cmd.Parameters.AddWithValue("@NoOfCustLocals", Stats1.NoOfCustLocals);
                        cmd.Parameters.AddWithValue("@NoOfCustOther", Stats1.NoOfCustOther);
                        cmd.Parameters.AddWithValue("@NoOfCustForeign", Stats1.NoOfCustForeign);
                        cmd.Parameters.AddWithValue("@NoOfTranCash", Stats1.NoOfTranCash);
                        cmd.Parameters.AddWithValue("@NoOfTranDepCash", Stats1.NoOfTranDepCash);
                        cmd.Parameters.AddWithValue("@NoOfTranDepCheq", Stats1.NoOfTranDepCheq);
                        cmd.Parameters.AddWithValue("@NoOfCheques", Stats1.NoOfCheques);

                        cmd.Parameters.AddWithValue("@SignIdRepl", Repl1.SignIdRepl);
                        cmd.Parameters.AddWithValue("@StartRepl", Repl1.StartRepl);
                        cmd.Parameters.AddWithValue("@FinishRepl", Repl1.FinishRepl);
                        cmd.Parameters.AddWithValue("@StepLevel", Repl1.StepLevel);
                        cmd.Parameters.AddWithValue("@ReplStartDtTm", Repl1.ReplStartDtTm);
                        cmd.Parameters.AddWithValue("@ReplFinDtTm", Repl1.ReplFinDtTm);
                        cmd.Parameters.AddWithValue("@DiffRepl", Repl1.DiffRepl);
                        cmd.Parameters.AddWithValue("@ErrsRepl", Repl1.ErrsRepl);
                        cmd.Parameters.AddWithValue("@NextRepDtTm", Repl1.NextRepDtTm);
                   
                        
                        cmd.Parameters.AddWithValue("@SignIdReconc", Recon1.SignIdReconc);
                        cmd.Parameters.AddWithValue("@DelegRecon", Recon1.DelegRecon);
                        cmd.Parameters.AddWithValue("@StartReconc", Recon1.StartReconc);
                        cmd.Parameters.AddWithValue("@FinishReconc", Recon1.FinishReconc);
                        cmd.Parameters.AddWithValue("@RecStartDtTm", Recon1.RecStartDtTm);
                        cmd.Parameters.AddWithValue("@RecFinDtTm", Recon1.RecFinDtTm);
                        cmd.Parameters.AddWithValue("@DiffReconcStart", Recon1.DiffReconcStart);
                        cmd.Parameters.AddWithValue("@DiffReconcEnd", Recon1.DiffReconcEnd);
                        cmd.Parameters.AddWithValue("@WFlowDifStatus", Recon1.WFlowDifStatus);
                       
                        cmd.Parameters.AddWithValue("@TurboReconc", Recon1.TurboReconc);
                        
                        cmd.Parameters.AddWithValue("@NumOfErrors", NumOfErrors);
                        cmd.Parameters.AddWithValue("@ErrOutstanding", ErrOutstanding);
                        cmd.Parameters.AddWithValue("@BalSetsNo", BalSetsNo);

                
                        cmd.Parameters.AddWithValue("@CurrNm1", Diff1.CurrNm1);
                        cmd.Parameters.AddWithValue("@DiffCurr1", Diff1.DiffCurr1);
                  
                        cmd.Parameters.AddWithValue("@CurrNm2", Diff1.CurrNm2);
                        cmd.Parameters.AddWithValue("@DiffCurr2", Diff1.DiffCurr2);
                     
                        cmd.Parameters.AddWithValue("@CurrNm3", Diff1.CurrNm3);
                        cmd.Parameters.AddWithValue("@DiffCurr3", Diff1.DiffCurr3);
                      
                        cmd.Parameters.AddWithValue("@CurrNm4", Diff1.CurrNm4);
                        cmd.Parameters.AddWithValue("@DiffCurr4", Diff1.DiffCurr4);
                        
                        cmd.Parameters.AddWithValue("@SessionsInDiff", SessionsInDiff);
                        cmd.Parameters.AddWithValue("@YtdInDiff", YtdInDiff);
                        cmd.Parameters.AddWithValue("@LatestBatchNo", LatestBatchNo);

                        cmd.Parameters.AddWithValue("@InNeedType", InNeedType);

                        cmd.Parameters.AddWithValue("@UpdatedBatch", UpdatedBatch);
                        cmd.Parameters.AddWithValue("@Last", Last);
                        cmd.Parameters.AddWithValue("@InProcess", InProcess);

                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);

                        cmd.Parameters.AddWithValue("@ReplGenComment", ReplGenComment);
                     
                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                      //  if (rows > 0)
                     //   {
                     //       UpdSesTraces = true;
                          
                     //   }
                     //   else UpdSesTraces = false; // textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TracesReadUpdate Class............. " + ex.Message;
                }

          //  return outcome;
        }
// UPDATE TRACES AT REPlENISHMENT Start  
//

public void UpdateTracesStartRepl(string InUserid, string InAtmNo, int InSesNo)
{

    ReadSessionsStatusTraces(InAtmNo, InSesNo);

    Repl1.ReplStartDtTm = DateTime.Now; 

    Repl1.SignIdRepl = InUserid;
    Repl1.StartRepl = true;
    //
    // UPDATE SESSION TRACES
    //
    UpdateSessionsStatusTraces(InAtmNo, InSesNo); // UPDATE TRACES 
}

// UPDATE TRACES AT REPlENISHMENT END AND RECONCILIATION END 
//

public void UpdateTracesFinishReplOrReconc(string InAtmNo, int InSesNo, string InUserId, int WMode)
{
    // If WMode = 1 then REPLENISHEMNT HAS FINISHED 
    // If WMode = 2 then RECONCILIATION HAS FINISHED 
    
    RRDMNotesBalances Na = new RRDMNotesBalances();
    RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
    Ta.ReadSessionsStatusTraces(InAtmNo, InSesNo);

    AutoRec = false ; 
    
    int WFunction = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 
    Na.ReadSessionsNotesAndValues(InAtmNo, InSesNo, WFunction); // CALL

    ReadSessionsStatusTraces(InAtmNo, InSesNo);

    if (Na.SystemTargets1.LastTrace < FirstTraceNo & Na.SystemTargets2.LastTrace < FirstTraceNo &
      Na.SystemTargets3.LastTrace < FirstTraceNo & Na.SystemTargets4.LastTrace < FirstTraceNo &
      Na.SystemTargets5.LastTrace < FirstTraceNo)
    {
        UpdatedBatch = false; // HOST FILES NOT RECEIVED YET 
    }
    else
    {
        UpdatedBatch = true; // HOST FILES RECEIVED 
    }

    if (WMode == 1) // Initialize variables 
    {
         Repl1.FinishRepl = true;
         Repl1.ReplFinDtTm = DateTime.Now;

         Recon1.StartReconc = false;
         Recon1.FinishReconc = false;
         Recon1.DiffReconcEnd = true;
    }

    // UPDATE SESSION TRACES 
    //
    if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false & UpdatedBatch == true)
    {
       
        // Initialise 
        NumOfErrors = 0;
        ErrOutstanding = 0;
        Diff1.CurrNm1 = "";
        Diff1.DiffCurr1 = 0;

        Diff1.CurrNm2 = "";
        Diff1.DiffCurr2 = 0;

        Diff1.CurrNm3 = "";
        Diff1.DiffCurr3 = 0;

        Diff1.CurrNm4 = "";
        Diff1.DiffCurr4 = 0;

        BalSetsNo = Na.BalSets;

        if (WMode == 1)
        {
            Repl1.DiffRepl = false;
            Repl1.ErrsRepl = false;
        }
        if (WMode == 1 & UpdatedBatch == true) // During Replenishment no difference found 
        {
            AutoRec = true; 

            Recon1.SignIdReconc = InUserId; // Find out 
            Recon1.DelegRecon = false;
            Recon1.StartReconc = true;
            Recon1.FinishReconc = true;
            Recon1.RecStartDtTm = DateTime.Now;
            Recon1.RecFinDtTm = DateTime.Now;
            Recon1.DiffReconcStart = false;
            Recon1.DiffReconcEnd = false;

            SessionsInDiff = 0;
            // Here Update all next Traces with zero 
            LatestBatchNo = Na.HBatchNo;     
        }

        if (WMode == 2 ) 
        {
         //   Recon1.SignIdReconc = InUserNo; // Find out 
            Recon1.DelegRecon = false;
         //   Recon1.StartReconc = true;
            Recon1.FinishReconc = true;
       //     Recon1.RecStartDtTm = DateTime.Now;
            Recon1.RecFinDtTm = DateTime.Now;
       //     Recon1.DiffReconcStart = false;
            Recon1.DiffReconcEnd = false;

            SessionsInDiff = 0;
            // Here Update all next Traces with zero 
            LatestBatchNo = Na.HBatchNo;
        }
       
    }
    else
    {
        //   DiffRepl = true;
        if (WMode == 1)
        {
            if (Na.DiffAtAtmLevel == true || Na.DiffWithErrors == true)
            {
                Repl1.ErrsRepl = true;
                Repl1.DiffRepl = true;
                SessionsInDiff = SessionsInDiff + 1;

                Recon1.DiffReconcStart = true;
                Recon1.DiffReconcEnd = true;
            }
        }
       
        //
        // UPDATE SESSION TRACES as a result of reconciliation process 
        //
        if (WMode == 2)
        {
            Recon1.SignIdReconc = Repl1.SignIdRepl;
      
            Recon1.DiffReconcEnd = true;
        }

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions(); 

        Ec.ReadAllErrorsTableForCounterReplCycle(BankId, InAtmNo, InSesNo);

        NumOfErrors = Ec.NumOfErrors;
        ErrOutstanding = Ec.NumOfErrors - Ec.ErrUnderAction;
        BalSetsNo = Na.BalSets;

        if (Na.DiffAtHostLevel == true || (Na.DiffAtHostLevel == false & Na.DiffAtAtmLevel == true))
        {

            Diff1.CurrNm1 = Na.BalDiff1.CurrNm;
            Diff1.DiffCurr1 = Na.BalDiff1.HostAdj;

            Diff1.CurrNm2 = Na.BalDiff2.CurrNm;
            Diff1.DiffCurr2 = Na.BalDiff2.HostAdj;

            Diff1.CurrNm3 = Na.BalDiff3.CurrNm;
            Diff1.DiffCurr3 = Na.BalDiff3.HostAdj;

            Diff1.CurrNm4 = Na.BalDiff4.CurrNm;
            Diff1.DiffCurr4 = Na.BalDiff4.HostAdj;
        }
        else
        {
            Diff1.CurrNm1 = "";
            Diff1.DiffCurr1 = 0 ;

            Diff1.CurrNm2 = "";
            Diff1.DiffCurr2 = 0;

            Diff1.CurrNm3 = "";
            Diff1.DiffCurr3 = 0;

            Diff1.CurrNm4 = "";
            Diff1.DiffCurr4 = 0;
        }

        LatestBatchNo = Na.HBatchNo;
        BalSetsNo = Na.BalSets;
        // UPDATE IF NOT ALREADY UPDATED IN REPLENISHMENT 
        if (WMode == 2 & Ta.Repl1.DiffRepl == false)

        {
            SessionsInDiff = SessionsInDiff + 1;
        } 
       
    }
    if (WMode == 1) ProcessMode = 1;
    if (WMode == 2) ProcessMode = 2;

    if (AutoRec == true & UpdatedBatch == true & WMode == 1)
    {
        ProcessMode = 2; // WE HAVE PASTED AUTOMATIC RECONCILIATION 
    }

    if (Recon1.DiffReconcEnd == true & WMode == 2)
    {
        ProcessMode = 3; // WE HAVE PASTED AUTOMATIC RECONCILIATION 
    }

    //
    // UPDATE SESSION TRACES
    //


    UpdateSessionsStatusTraces(InAtmNo, InSesNo); // UPDATE TRACES 

}
        
// UPDATE TRACES AT RECONCILIATION Start  
//

public void UpdateTracesStartReconc(string InUserBankId, string InUserId, string InAtmNo, int InSesNo)
{

        ReadSessionsStatusTraces(InAtmNo, InSesNo);

            Recon1.SignIdReconc = InUserId; ;
            Recon1.DelegRecon = false;
            Recon1.StartReconc = true;
            Recon1.RecStartDtTm = DateTime.Now;
     RRDMNotesBalances Na = new RRDMNotesBalances(); 
    //  readSessionNotesAndValues to get BALANCES AND OTHER INFORMATION  

    int WProcess = 4; // INCLUDE IN BALANCES ANY COREECTED ERRORS 

    Na.ReadSessionsNotesAndValues(InAtmNo, InSesNo, WProcess); // CALL TO MAKE BALANCES AVAILABLE 

    if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false)
    {

        Recon1.DiffReconcStart = false;
    }
    else
    {
        Recon1.DiffReconcStart = true;
    }
            //
            // UPDATE SESSION TRACES
            //

            UpdateSessionsStatusTraces(InAtmNo, InSesNo); // UPDATE TRACES 
}


    }
}

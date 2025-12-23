using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMSessionsTracesReadUpdate : Logger
    {
        public RRDMSessionsTracesReadUpdate() : base() { }
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
        public DateTime SM_LAST_CLEARED;
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
            public int RecAtRMCycle;
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
        public int LatestBatchNo;// Is used for the CIT EXCEL INPUT at the Finish of workflow 
                                 // 0 It didnt pass through Excel 
                                 // 2 is done by auto load and unload came and reconcile 
                                 // 3 Load but Not Unload - wait 
                                 // 4 Unload but not load - wait 
                                 // 5 Should be made manually  
        public int InNeedType;

        public bool Is_Updated_GL;
        public string Maker; // Words like No Action etc 
        public string Authoriser;
        public bool Last;
        public bool InProcess;

        public int ProcessMode;
        //  -1 : Not Replenished yet
        //   0 : In process of Replenishment workflow
        //   1 : Atm Replenished for this cycle
        //   2 : Atm had passed the Cash Reconcilitaion 

        public string ReplGenComment;

        public int NextReplNo;
        public int LastNo;
        public int Last_1; // LAST SESSION NUMBER 

        public int ReplOutstanding;

        public int LastReplCyclId; // latest repl cycle 
        public int LLatestBatchNo; // Latest 
        public bool LUpdatedBatch; // Latest 

        public int LoadedAtRMCycle;
        public int ReconcAtRMCycle;

        public string Operator;

        //    bool UpdSesTraces;

        bool AutoRec;

        // Define the data table 
        public DataTable ATMsReplCyclesSelectedPeriod = new DataTable();
        public DataTable ATMsReplCyclesTheBaddies = new DataTable();
        public DataTable ATMsReplCycles_loaded_at_This_Cycle = new DataTable();

        public int TotalSelected;

        DateTime NullDate = new DateTime(1900, 01, 01);

        // 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;


        private void ReaderFields(SqlDataReader rdr)
        {
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
            SM_LAST_CLEARED = (DateTime)rdr["SM_LAST_CLEARED"];
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
            Recon1.RecAtRMCycle = (int)rdr["RecAtRMCycle"];
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
            LatestBatchNo = (int)rdr["LatestBatchNo"]; // Is used for the CIT EXCEL INPUT at the Finish of workflow 
                                                       // 0 It didnt pass through Excel 
                                                       // 2 is done by auto load and unload came and reconcile 
                                                       // 3 Load but Not Unload - wait 
                                                       // 4 Unload but not load - wait 
                                                       // 5 Should be made manually  
                                                       // 7 Done manually  
            InNeedType = (int)rdr["InNeedType"];

            Is_Updated_GL = (bool)rdr["UpdatedBatch"];

            Maker = (string)rdr["Maker"];
            Authoriser = (string)rdr["Authoriser"];

            Last = (bool)rdr["Last"];
            InProcess = (bool)rdr["InProcess"];

            ProcessMode = (int)rdr["ProcessMode"];

            ReplGenComment = (string)rdr["ReplGenComment"];

            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
            ReconcAtRMCycle = (int)rdr["ReconcAtRMCycle"];

            Operator = (string)rdr["Operator"];
        }
        // Methods 
        // For an ATM read its replecycles for a period 
        // FILL UP A TABLE 
        public string ExcelStatus;
        public void ReadReplCyclesForFromToDateFillTable(string InOperator, string InSignedId, string InAtmNo, DateTime InDtFrom, DateTime InDtTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int WSesNo;

            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            RRDMGasParameters Gp = new RRDMGasParameters();
            bool AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(InOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;


                }
                else
                {
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }
            ExcelStatus = "";

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

            ATMsReplCyclesSelectedPeriod = new DataTable();
            ATMsReplCyclesSelectedPeriod.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsReplCyclesSelectedPeriod.Columns.Add("ReplCycle", typeof(int));

            ATMsReplCyclesSelectedPeriod.Columns.Add("CycleStart", typeof(string));
            ATMsReplCyclesSelectedPeriod.Columns.Add("CycleEnd", typeof(string));

            ATMsReplCyclesSelectedPeriod.Columns.Add("Mode", typeof(string));

            ATMsReplCyclesSelectedPeriod.Columns.Add("Mode_2", typeof(int));
            if (AudiType == true)
            {
                ATMsReplCyclesSelectedPeriod.Columns.Add("ExcelStatus", typeof(string));
            }


            string SqlString = " SELECT *"
                     + " FROM [dbo].[SessionsStatusTraces] "
                     + " WHERE Operator=@Operator AND AtmNo =@AtmNo AND (SesDtTimeStart >= @SesDtTimeStart AND SesDtTimeEnd <= @SesDtTimeEnd)  "
                     + " Order by SesDtTimeStart DESC ";

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

                            ReaderFields(rdr);

                            TotalSelected = TotalSelected + 1;
                            DataRow RowSelected = ATMsReplCyclesSelectedPeriod.NewRow();

                            RowSelected["ReplCycle"] = WSesNo = (int)rdr["SesNo"];

                            DateTime START_Date = (DateTime)rdr["SesDtTimeStart"];
                            DateTime END_Date = (DateTime)rdr["SesDtTimeEnd"];

                            RowSelected["CycleStart"] = START_Date.ToString();

                            if (END_Date != NullDate)
                            {
                                RowSelected["CycleEnd"] = END_Date.ToString();
                            }
                            else
                            {
                                RowSelected["CycleEnd"] = "In Progress";
                            }

                            int TempMode = (int)rdr["ProcessMode"];

                            if (TempMode == -1)
                            {
                                RowSelected["Mode"] = "In Progress";
                            }
                            if (TempMode == -5)
                            {
                                RowSelected["Mode"] = "Missing Journal for this";
                            }
                            if (TempMode == -6)
                            {
                                RowSelected["Mode"] = "Toxic Journal at SM for this";
                            }
                            if (TempMode == 0)
                            {
                                RowSelected["Mode"] = "Ready For Repl Workflow";
                                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(InAtmNo, WSesNo, "Replenishment");
                                // Check if record in Authorisations 
                                if (Ap.RecordFound == true)
                                {
                                    RowSelected["Mode"] = "Under Author Process_Stage.." + Ap.Stage;
                                }

                            }
                            if (TempMode == 1 || TempMode == 2 || TempMode == 3)
                            {
                                RowSelected["Mode"] = "Completed";
                            }

                            RowSelected["Mode_2"] = TempMode;
                            //   RowSelected["LastTraceNo"] = (int)rdr["LastTraceNo"];

                            // Is used for the CIT EXCEL INPUT at the Finish of workflow 
                            // 0 It didnt pass through Excel 
                            // 2 is done by auto load and unload came and reconcile 
                            // 3 Load but Not Unload - wait 
                            // 4 Unload but not load - wait 
                            // 5 Should be made manually  
                            // 7 Done manually  

                            if (AudiType == true)
                            {

                                switch (LatestBatchNo)
                                {
                                    case 0:
                                        {
                                            if (TempMode == -1)
                                            {
                                                RowSelected["ExcelStatus"] = ExcelStatus = "N/A";
                                            }
                                            else
                                            {
                                                RowSelected["ExcelStatus"] = ExcelStatus = "0: Excel_Loaded_But_Not_Processed";
                                            }
                                            break;
                                        }
                                    case 2:
                                        {

                                            RowSelected["ExcelStatus"] = ExcelStatus = "2: Auto DONE";

                                            break;
                                        }
                                    case 3:
                                        {

                                            RowSelected["ExcelStatus"] = ExcelStatus = "3: Load but Not Unload - wait";

                                            break;
                                        }

                                    case 4:
                                        {

                                            RowSelected["ExcelStatus"] = ExcelStatus = "4: Unload but not load - wait ";

                                            break;
                                        }
                                    case 5:
                                        {
                                            if (ProcessMode == 2)
                                            {
                                                RowSelected["ExcelStatus"] = "Done Manually-" + Recon1.RecFinDtTm.ToString();
                                            }
                                            else
                                            {
                                                RowSelected["ExcelStatus"] = ExcelStatus = "5: Should be made manually";
                                            }


                                            break;
                                        }

                                    default:
                                        {
                                            RowSelected["ExcelStatus"] = ExcelStatus = "Not Defined";
                                            break;
                                        }
                                }

                            }


                            // ADD ROW
                            ATMsReplCyclesSelectedPeriod.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }
        // For Form153 _ Show the Baddies
        public void ReadReplCycles_TheBaddies(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ATMsReplCyclesTheBaddies = new DataTable();
            ATMsReplCyclesTheBaddies.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsReplCyclesTheBaddies.Columns.Add("AtmNo", typeof(string));
            ATMsReplCyclesTheBaddies.Columns.Add("ReplCycle", typeof(int));

            ATMsReplCyclesTheBaddies.Columns.Add("CycleStart", typeof(string));
            ATMsReplCyclesTheBaddies.Columns.Add("CycleEnd", typeof(string));

            ATMsReplCyclesTheBaddies.Columns.Add("Mode", typeof(string));

            string SqlString = "SELECT  * "
                     + " FROM [dbo].[SessionsStatusTraces] "
                     //  + " WHERE AtmNo =@AtmNo "
                     + InSelectionCriteria
                     + " Order by AtmNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //  cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            TotalSelected = TotalSelected + 1;
                            DataRow RowSelected = ATMsReplCyclesTheBaddies.NewRow();
                            RowSelected["ATMNo"] = (string)rdr["AtmNo"];
                            RowSelected["ReplCycle"] = (int)rdr["SesNo"];

                            DateTime START_Date = (DateTime)rdr["SesDtTimeStart"];
                            DateTime END_Date = (DateTime)rdr["SesDtTimeEnd"];

                            RowSelected["CycleStart"] = START_Date.ToString();

                            if (END_Date != NullDate)
                            {
                                RowSelected["CycleEnd"] = END_Date.ToString();
                            }
                            else
                            {
                                RowSelected["CycleEnd"] = "In Progress";
                            }
                            int TempMode = (int)rdr["ProcessMode"];
                            if (TempMode == -1)
                            {
                                RowSelected["Mode"] = "In Progress";
                            }
                            if (TempMode == -5)
                            {
                                RowSelected["Mode"] = "Missing Journal for this";
                            }
                            if (TempMode == -6)
                            {
                                RowSelected["Mode"] = "Toxic Journal at SM for this";
                            }
                            if (TempMode == 0)
                            {
                                RowSelected["Mode"] = "Ready For Repl Workflow";
                            }
                            if (TempMode == 1 || TempMode == 2 || TempMode == 3)
                            {
                                RowSelected["Mode"] = "Completed";
                            }


                            // ADD ROW
                            ATMsReplCyclesTheBaddies.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }
        // For Form153
        public void ReadReplCycles_Last_Numberof_Cycles(string InAtmNo, int InNumberOfCycles)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ATMsReplCyclesSelectedPeriod = new DataTable();
            ATMsReplCyclesSelectedPeriod.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsReplCyclesSelectedPeriod.Columns.Add("ReplCycle", typeof(int));

            ATMsReplCyclesSelectedPeriod.Columns.Add("CycleStart", typeof(string));
            ATMsReplCyclesSelectedPeriod.Columns.Add("CycleEnd", typeof(string));
            ATMsReplCyclesSelectedPeriod.Columns.Add("Mode_N", typeof(int));
            ATMsReplCyclesSelectedPeriod.Columns.Add("Mode", typeof(string));

            string SqlString = "SELECT TOP(" + InNumberOfCycles + ") * "
                     + " FROM [dbo].[SessionsStatusTraces] "
                     //  + " WHERE AtmNo =@AtmNo "
                     + " WHERE AtmNo ='" + InAtmNo + "'"
                     + " Order by SesDtTimeStart DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //  cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            TotalSelected = TotalSelected + 1;
                            DataRow RowSelected = ATMsReplCyclesSelectedPeriod.NewRow();

                            RowSelected["ReplCycle"] = (int)rdr["SesNo"];

                            DateTime START_Date = (DateTime)rdr["SesDtTimeStart"];
                            DateTime END_Date = (DateTime)rdr["SesDtTimeEnd"];

                            RowSelected["CycleStart"] = START_Date.ToString();

                            if (END_Date != NullDate)
                            {
                                RowSelected["CycleEnd"] = END_Date.ToString();
                            }
                            else
                            {
                                RowSelected["CycleEnd"] = "In Progress";
                            }
                            int TempMode = (int)rdr["ProcessMode"];
                            RowSelected["Mode_N"] = TempMode;
                            if (TempMode == -1)
                            {
                                RowSelected["Mode"] = "In Progress";
                            }
                            if (TempMode == -5)
                            {
                                RowSelected["Mode"] = "Missing Journal for this";
                            }
                            if (TempMode == -6)
                            {
                                RowSelected["Mode"] = "Toxic Journal at SM for this";
                            }
                            if (TempMode == 0)
                            {
                                RowSelected["Mode"] = "Ready For Repl Workflow";
                            }
                            if (TempMode == 1 || TempMode == 2 || TempMode == 3)
                            {
                                RowSelected["Mode"] = "Completed";
                            }


                            // ADD ROW
                            ATMsReplCyclesSelectedPeriod.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // For Form153
        public int ReadReplCycles_Last_SesNo_For_This_ATM(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            TotalSelected = 0;

            string SqlString = "SELECT TOP(1) * "
                     + " FROM [dbo].[SessionsStatusTraces] "
                     + " WHERE AtmNo ='" + InAtmNo + "' AND ProcessMode <> -1 "
                     + " Order by SesNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //  cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            TotalSelected = TotalSelected + 1;


                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
            return SesNo;
        }


        // For Form153
        public int ReadReplCycles_FOR_ATM_And_DATE(string InAtmNo, DateTime InDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            TotalSelected = 0;

            string SqlString = "SELECT Top(1) SesNo, SesDtTimeStart, SesDtTimeEnd, ProcessMode "
                     + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
                     + " WHERE AtmNo =@AtmNo AND CAST(SesDtTimeEnd as Date ) = @InDate "
                     + " Order By SesNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@InDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SesNo = (int)rdr["SesNo"];

                            SesDtTimeStart = (DateTime)rdr["SesDtTimeStart"];
                            SesDtTimeEnd = (DateTime)rdr["SesDtTimeEnd"];

                            ProcessMode = (int)rdr["ProcessMode"];

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
            return SesNo;
        }


        public void ReadReplCycles_Not_TheReplenished(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ATMsReplCyclesSelectedPeriod = new DataTable();
            ATMsReplCyclesSelectedPeriod.Clear();

            SqlString = " Select * "
                                  + " FROM [dbo].[SessionsStatusTraces] "
                                  + " WHERE AtmNo =@AtmNo AND ProcessMode in (0, -1, -5, -6) ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        sqlAdapt.Fill(ATMsReplCyclesSelectedPeriod);

                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                    return;
                }
        }
        // READ ALL THIS CYCLE REPLENISHMENT AND CREATE AND CALCULATE CASH RECONCILIATION RECORD
        public void ReadReplCycles_Created_This_Cycle_For_CASH_RECO(int In_LoadedAtRMCycle, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ATMsReplCycles_loaded_at_This_Cycle = new DataTable();
            ATMsReplCycles_loaded_at_This_Cycle.Clear();

            SqlString = " Select * "
                                  + " FROM [dbo].[SessionsStatusTraces] "
                                  + " WHERE LoadedAtRMCycle =@LoadedAtRMCycle AND ProcessMode <> -1 "
                                  + " AND Cast(SesDtTimeEnd as Date) <= @Cut_Off_Date ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@LoadedAtRMCycle", In_LoadedAtRMCycle);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);

                        sqlAdapt.Fill(ATMsReplCycles_loaded_at_This_Cycle);

                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                    return;
                }

            RRDM_ATMs_CASH_RECON_MASTER_RECORD Cr = new RRDM_ATMs_CASH_RECON_MASTER_RECORD();

            int I = 0;

            while (I <= (ATMsReplCycles_loaded_at_This_Cycle.Rows.Count - 1))
            {
                //    RecordFound = true;
                int WSesNo = (int)ATMsReplCycles_loaded_at_This_Cycle.Rows[I]["SesNo"];
                string WAtmNo = (string)ATMsReplCycles_loaded_at_This_Cycle.Rows[I]["AtmNo"];
                string WAtmName = (string)ATMsReplCycles_loaded_at_This_Cycle.Rows[I]["AtmName"];

                DateTime WSesDtTimeStart = (DateTime)ATMsReplCycles_loaded_at_This_Cycle.Rows[I]["WSesDtTimeStart"];
                DateTime WSesDtTimeEnd = (DateTime)ATMsReplCycles_loaded_at_This_Cycle.Rows[I]["SesDtTimeEnd"];

                string WOperator = (string)ATMsReplCycles_loaded_at_This_Cycle.Rows[I]["Operator"];

                Cr.AtmNo = WAtmNo;
                Cr.AtmName = WAtmName;
                Cr.Previous_ReplDate = WSesDtTimeStart;
                Cr.ReplDate = WSesDtTimeEnd;
                Cr.ReplCycleNo = WSesNo;
                Cr.LoadedAtRMCycle = In_LoadedAtRMCycle;
                Cr.Cut_Off_date = InCut_Off_Date; 
                Cr.Operator = WOperator; 

                int InsertSeqNo = Cr.Insert_CASH_RECON(WAtmNo, WSesNo);

                Cr.Read_CASH_RECON_BySeqNo(WOperator, InsertSeqNo);

                // Get the totals and Update all Fields 
      //           ,[SM_DATA]
      //,[SM_OpeningBalance]
      //,[SM_Dispensed]
      //,[SM_Remaining]
      //,[SM_Cash_Loaded]
      //,[SM_Cash_Loaded_Minus_SM_Remaining]
      //,[SM_Deposits]
        // UPDATE TOTALS 
               Cr.Update_CASH_RECON(InsertSeqNo); 



            }

            I = I + 1;
        }

   

        // Methods 
        // Validate new dates 
        // FILL UP A TABLE 
        public bool ErrorInDates;
        public string ErrorInDatesMsg;
        public void ReadReplCyclesAndValidateDatesNewCycle(string InAtmNo, DateTime InDtFrom, DateTime InDtTo)
        {
            // BREAK AT THE FIRST ERROR
            //
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorInDates = false;


            TotalSelected = 0;

            // Check for the first 20

            // string SqlString = "SELECT TOP (20) * "
            //          + " FROM [dbo].[SessionsStatusTraces] "
            //          + " WHERE AtmNo =@AtmNo"
            //         // + "AND SesNo <> @SesNo "
            //+ " AND  ( "
            // + " @Date1 between SesDtTimeStart and SesDtTimeEnd "
            // + " OR "
            //  + " @Date2 between SesDtTimeStart and SesDtTimeEnd "
            //    + " )   "
            //          + " Order by SesDtTimeStart DESC ";

            string SqlString = "SELECT TOP (20) * "
                     + " FROM [dbo].[SessionsStatusTraces] "
                     + " WHERE AtmNo =@AtmNo"
           // + "AND SesNo <> @SesNo "
           + " AND  ( "
            + " (@Date1 > SesDtTimeStart and @Date1 < SesDtTimeEnd) "
            + " OR "
             + " (@Date2 > SesDtTimeStart and @Date2 < SesDtTimeEnd) "
               + " )   "
                     + " Order by SesDtTimeStart DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Date1", InDtFrom);
                        cmd.Parameters.AddWithValue("@Date2", InDtTo);
                        // cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;


                            ReaderFields(rdr);


                            if (ProcessMode == -1)
                            {
                                continue;
                            }
                            else
                            {
                                ErrorInDates = true;

                                // 
                                ErrorInDatesMsg = "There is overlapping in Repl Cycle No.." + SesNo.ToString() + Environment.NewLine
                                   + "The input dates fall within this cycle." + Environment.NewLine
                                     + "If missing Journal then delete the cycle and create a new Cycle." + Environment.NewLine
                                       + "" + Environment.NewLine
                                    ;

                                break; // break at the first you find 
                            }

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }

        public void ReadReplCyclesAndValidateDatesUpdate(string InAtmNo, DateTime InDtFrom, DateTime InDtTo, int InSesNo)
        {
            // BREAK AT THE FIRST ERROR
            //
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorInDates = false;


            TotalSelected = 0;

            // Check for the first 20

            // string SqlString = "SELECT TOP (20) * "
            //          + " FROM [dbo].[SessionsStatusTraces] "
            //          + " WHERE AtmNo =@AtmNo"
            // + " AND SesNo <> @SesNo "
            //+ " AND  ( "
            // + " @Date1 between SesDtTimeStart and SesDtTimeEnd "
            // + " OR "
            //  + " @Date2 between SesDtTimeStart and SesDtTimeEnd "
            //    + " )   "
            //          + " Order by SesDtTimeStart DESC ";

            string SqlString = "SELECT TOP (20) * "
                    + " FROM [dbo].[SessionsStatusTraces] "
                    + " WHERE AtmNo =@AtmNo "
          + " AND SesNo <> @SesNo "
          + " AND  ( "
           + " (@Date1 > SesDtTimeStart and @Date1 < SesDtTimeEnd) "
           + " OR "
            + " (@Date2 > SesDtTimeStart and @Date2 < SesDtTimeEnd) "
              + " )   "
                    + " Order by SesDtTimeStart DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Date1", InDtFrom);
                        cmd.Parameters.AddWithValue("@Date2", InDtTo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;


                            ReaderFields(rdr);


                            if (ProcessMode == -1)
                            {
                                continue;
                            }
                            else
                            {
                                ErrorInDates = true;

                                // 
                                ErrorInDatesMsg = "There is overlapping in Repl Cycle No.." + SesNo.ToString() + Environment.NewLine
                                   + "The input dates fall within this cycle."
                                    ;

                                break; // break at the first you find 
                            }

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }

        public void ReadReplCyclesAndValidateDatesforSelected(string InAtmNo, DateTime InDtFrom, DateTime InDtTo, int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorInDates = false;


            TotalSelected = 0;


            string SqlString = "SELECT  * "
                     + " FROM [dbo].[SessionsStatusTraces] "
                     + " WHERE AtmNo =@AtmNo AND SesNo = @SesNo "
           + " AND  ( "
            + " @Date1 between SesDtTimeStart and SesDtTimeEnd "
            + " OR "
             + " @Date2 between SesDtTimeStart and SesDtTimeEnd "
               + " )   "
                     + " Order by SesDtTimeStart DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Date1", InDtFrom);
                        cmd.Parameters.AddWithValue("@Date2", InDtTo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            ErrorInDates = true;

                            // 
                            ErrorInDatesMsg = "There is overlapping in Repl Cycle No.." + SesNo.ToString() + Environment.NewLine
                               + "The input dates fall within this cycle."
                                ;

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }

        public int ReadFindReplCycleForGivenDate(string InAtmNo, DateTime InDtTime)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorInDates = false;

            SesNo = 0;

            TotalSelected = 0;


            string SqlString = "SELECT  * "
                     + " FROM [dbo].[SessionsStatusTraces] "
                     + " WHERE AtmNo =@AtmNo "
           + " AND  ( "
            + " @DtTime between SesDtTimeStart and SesDtTimeEnd "
               + " )   "
                     + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@DtTime", InDtTime);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }

            return SesNo;
        }

        // Methods 
        // Read All Atms with difference and Process =1 
        // FILL UP A TABLE 
        string SqlString;
        public DataTable TableCashReconciliation = new DataTable();
        public void ReadReplCyclesWithDifferForThisAtmGroup_For_Viewing(string InOperator, string InSignedId, int InReconcGroupNo, int InRecAtRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            int WProcess = 0;

            TableCashReconciliation = new DataTable();
            TableCashReconciliation.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableCashReconciliation.Columns.Add("AtmNo", typeof(string));

            TableCashReconciliation.Columns.Add("GL At Cut Off", typeof(decimal));
            TableCashReconciliation.Columns.Add("GL At Repl", typeof(decimal));
            TableCashReconciliation.Columns.Add("GL & Actions", typeof(decimal));
            TableCashReconciliation.Columns.Add("Cash Unloaded", typeof(decimal));
            TableCashReconciliation.Columns.Add("GL Diff", typeof(decimal));

            TableCashReconciliation.Columns.Add("Maker", typeof(string));
            TableCashReconciliation.Columns.Add("Authoriser", typeof(string));

            TableCashReconciliation.Columns.Add("ReplCycleNo", typeof(int));


            SqlString = "SELECT * "
                                               + " FROM [ATMs].[dbo].[SessionsStatusTraces] "
                                               + " WHERE Operator=@Operator AND RecAtRMCycle = @RecAtRMCycle "
                                               + " AND AtmsReconcGroup = @AtmsReconcGroup "
                                               + " Order by AtmNo ";



            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InReconcGroupNo);
                        cmd.Parameters.AddWithValue("@RecAtRMCycle", InRecAtRMCycle);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            TotalSelected = TotalSelected + 1;

                            DataRow RowSelected = TableCashReconciliation.NewRow();

                            WProcess = 4;

                            Na.ReadSessionsNotesAndValues(AtmNo, SesNo, WProcess);

                            RowSelected["AtmNo"] = AtmNo;

                            if (InOperator == "CRBAGRAA")
                            {
                                RowSelected["GL At Cut Off"] = Na.Balances1.HostBal;
                                RowSelected["GL At Repl"] = Na.Balances1.ReplToRepl;
                                RowSelected["GL & Actions"] = Na.Balances1.HostBalAdj; // With Actions 
                                RowSelected["Cash Unloaded"] = Na.Balances1.CountedBal;
                                RowSelected["GL Diff"] = Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj);
                            }
                            else
                            {
                                RowSelected["GL At Cut Off"] = Na.Balances1.HostBal;
                                RowSelected["GL At Repl"] = Na.GL_Bal_Repl_Adjusted;
                                RowSelected["GL & Actions"] = Na.Balances1.HostBalAdj; // With Actions 
                                RowSelected["Cash Unloaded"] = Na.Balances1.CountedBal;
                                RowSelected["GL Diff"] = Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj);
                            }

                            RowSelected["Maker"] = Maker;
                            RowSelected["Authoriser"] = Authoriser;

                            RowSelected["ReplCycleNo"] = SesNo;

                            // ADD ROW
                            TableCashReconciliation.Rows.Add(RowSelected);


                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // Methods 
        // Read All Atms with difference and Process =1 
        // FILL UP A TABLE 

        public void ReadReplCyclesWithDifferForThisAtmGroup_For_Processing(string InOperator, string InSignedId, int InReconcGroupNo, int InRecAtRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string OldAtm = "";

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            int WProcess = 0;

            TableCashReconciliation = new DataTable();
            TableCashReconciliation.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableCashReconciliation.Columns.Add("AtmNo", typeof(string));

            TableCashReconciliation.Columns.Add("GL At Cut Off", typeof(decimal));
            TableCashReconciliation.Columns.Add("GL At Repl", typeof(decimal));
            TableCashReconciliation.Columns.Add("GL & Actions", typeof(decimal));
            TableCashReconciliation.Columns.Add("Cash Unloaded", typeof(decimal));
            TableCashReconciliation.Columns.Add("GL Diff", typeof(decimal));

            TableCashReconciliation.Columns.Add("Maker", typeof(string));
            TableCashReconciliation.Columns.Add("Authoriser", typeof(string));

            TableCashReconciliation.Columns.Add("ReplCycleNo", typeof(int));


            SqlString = "SELECT * "
                                               + " FROM [ATMs].[dbo].[SessionsStatusTraces] "
                                               + " WHERE Operator=@Operator AND ProcessMode > 0 "
                                               + " AND AtmsReconcGroup = @AtmsReconcGroup "
                                               + " Order by AtmNo, SesNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InReconcGroupNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            TotalSelected = TotalSelected + 1;

                            if (AtmNo != OldAtm)
                            {
                                // READ THE FIRST RECORD(Latest Record with Repl reconciliation) WITH EITHER PROCESS MODE = 1 OR Process Mode = 2
                                // CHECK IF IT IS VALID 
                                if (ProcessMode == 1)
                                {
                                    // Valid to go into table
                                    DataRow RowSelected = TableCashReconciliation.NewRow();

                                    WProcess = 4;

                                    Na.ReadSessionsNotesAndValues(AtmNo, SesNo, WProcess);

                                    RowSelected["AtmNo"] = AtmNo;

                                    //if (InOperator == "CRBAGRAA")
                                    //{
                                    //    RowSelected["GL At Cut Off"] = Na.Balances1.HostBal;
                                    //    RowSelected["GL At Repl"] = Na.Balances1.ReplToRepl;
                                    //    RowSelected["GL & Actions"] = Na.Balances1.HostBalAdj; // With Actions 
                                    //    RowSelected["Cash Unloaded"] = Na.Balances1.CountedBal;
                                    //    RowSelected["GL Diff"] = Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj);
                                    //}
                                    //else
                                    //{
                                    RowSelected["GL At Cut Off"] = Na.Balances1.HostBal;
                                    RowSelected["GL At Repl"] = Na.GL_Bal_Repl_Adjusted;
                                    RowSelected["GL & Actions"] = Na.Balances1.HostBalAdj; // With Actions 
                                    RowSelected["Cash Unloaded"] = Na.Balances1.CountedBal;
                                    RowSelected["GL Diff"] = Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj);
                                    //}

                                    RowSelected["Maker"] = Maker;
                                    RowSelected["Authoriser"] = Authoriser;

                                    RowSelected["ReplCycleNo"] = SesNo;

                                    // ADD ROW
                                    TableCashReconciliation.Rows.Add(RowSelected);
                                }
                                else
                                {
                                    // Skip 
                                    // Means Process Mode is 
                                }
                                // THEN Set 
                                OldAtm = AtmNo;

                            }
                            else
                            {
                                // READ NEXT 
                            }

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //InsertWReportAtmRepl(InSignedId);
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }


        //// Insert  Replenishment Report 
        //private void InsertWReportAtmRepl(string InSignedId)
        //{

        //    try
        //    {
        //        ////ReadTable And Insert In Sql Table 
        //        RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
        //        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        //        RRDMDepositsClass Dc = new RRDMDepositsClass();
        //        RRDMAtmsClass Ac = new RRDMAtmsClass();

        //        //Clear Table 
        //        Tr.DeleteReportAtmRepl(InSignedId);

        //        int I = 0;

        //        while (I <= (ATMsReplCyclesSelectedPeriod.Rows.Count - 1))
        //        {

        //            RecordFound = true;

        //            Tr.ReplCycleNo = (int)ATMsReplCyclesSelectedPeriod.Rows[I]["ReplCycle"];
        //            Tr.AtmNo = (string)ATMsReplCyclesSelectedPeriod.Rows[I]["AtmNo"];

        //            ReadSessionsStatusTraces(Tr.AtmNo, Tr.ReplCycleNo);

        //            Na.ReadSessionsNotesAndValues(Tr.AtmNo, Tr.ReplCycleNo, 2);

        //            Dc.ReadDepositsSessionsNotesAndValuesDeposits(Tr.AtmNo, Tr.ReplCycleNo);

        //            //             public string CurrNm;
        //            //public decimal OpenBal;
        //            //public decimal CountedBal;
        //            //public decimal MachineBal;
        //            //public decimal ReplToRepl;
        //            //public decimal HostBal;
        //            //public decimal HostBalAdj;
        //            //public int NubOfErr;
        //            //public int ErrOutstanding;

        //            Ac.ReadAtm(Tr.AtmNo);

        //            Tr.AtmName = Ac.AtmName;

        //            Tr.ReplDate = SesDtTimeEnd;

        //            Tr.OpenningBalance = Na.Balances1.OpenBal;
        //            //       +" [WithDrawls], "
        //            //+ " [MachineUnloaded], "

        //            Tr.WithDrawls = Na.Balances1.OpenBal - Na.Balances1.MachineBal;
        //            Tr.MachineUnloaded = Na.Balances1.MachineBal;

        //            Tr.CountedUnloaded = Na.Balances1.CountedBal;

        //            Tr.DifferenceWithdrawals = Na.Balances1.CountedBal - Na.Balances1.MachineBal;

        //            Tr.PresentedErrors = Na.Balances1.PresenterValue;

        //            Tr.MoneyLoaded = Na.ReplAmountTotal;

        //            Tr.MachineDeposits = (Dc.DepositsMachine1.Amount + Dc.DepositsMachine1.AmountRej)
        //                                + (Dc.DepositsMachine1.EnvAmount)
        //                                + (Dc.ChequesMachine1.Amount);
        //            Tr.CountedDeposits = (Dc.DepositsCount1.Amount + Dc.DepositsCount1.AmountRej)
        //                + (Dc.DepositsCount1.EnvAmount)
        //                + (Dc.ChequesCount1.Amount)
        //                ;

        //            Tr.DifferenceDeposits = Tr.CountedDeposits - Tr.MachineDeposits;

        //            if ((Tr.DifferenceWithdrawals != 0 & Tr.CountedUnloaded != 0)
        //                 || (Tr.DifferenceDeposits != 0 & Tr.CountedDeposits != 0))
        //            {
        //                Tr.Comments = "Diff ALERT";
        //            }
        //            else
        //            {
        //                Tr.Comments = "NO ALERT";
        //                if (
        //                    (Tr.CountedUnloaded == 0 & Tr.MoneyLoaded > 0)
        //                    || (Tr.CountedDeposits == 0))
        //                {
        //                    // ATM OR CDM but not done yet 
        //                    Tr.Comments = "NOT DONE YET";
        //                }

        //            }


        //            Tr.Operator = Ac.Operator;

        //            // Insert record for printing 
        //            //
        //            Tr.InsertReportAtmRepl(InSignedId);

        //            I++; // Read Next entry of the table 

        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        CatchDetails(ex);

        //    }

        //}


        //  string outcome = ""; // TO FACILITATE EXCEPTIONS 
        // Methods 
        // READ Session Traces
        // 
        public void ReadSessionsStatusTraces(string InAtmNo, int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = " SELECT * "
          + " FROM ATMS.[dbo].[SessionsStatusTraces] "
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

                            ReaderFields(rdr);

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
        public void ReadSessionsStatusTraces_2_OSMAN(string InAtmNo, int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = " SELECT * "
          + " FROM ATMS_SAMIH_For_Osman.[dbo].[SessionsStatusTraces] "
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

                            ReaderFields(rdr);

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



        //  string outcome = ""; // TO FACILITATE EXCEPTIONS 
        // Methods 
        // READ Session Traces
        // 
        public void ReadSessionsStatusTracesToFindSesNoBasedDateAndProcessCode(string InAtmNo
                                                              , DateTime InSesDtTimeStart)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // SesDtTimeStart = (DateTime)rdr["SesDtTimeStart"];

            string SqlString = "SELECT *"
          + " FROM [dbo].[SessionsStatusTraces] "
          + " WHERE AtmNo=@AtmNo  "
            // + " AND Cast(SesDtTimeStart as Date) = @SesDtTimeStart AND ProcessMode =@ProcessMode  ";
            + " AND Cast(SesDtTimeStart as Date) = @SesDtTimeStart ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@SesDtTimeStart", InSesDtTimeStart.Date);
                        // cmd.Parameters.AddWithValue("@ProcessMode", InProcessMode);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReaderFields(rdr);

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

        // Methods 
        // READ Session Traces
        // Find Cycle 
        public void ReadSessionsStatusTracesToFindSesNoBasedDateEnd(string InAtmNo
                                                              , DateTime InSesDtTimeEnd)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // SesDtTimeStart = (DateTime)rdr["SesDtTimeStart"];

            string SqlString = "SELECT *"
          + " FROM [dbo].[SessionsStatusTraces] "
          + " WHERE AtmNo=@AtmNo  "
            // + " AND Cast(SesDtTimeStart as Date) = @SesDtTimeStart AND ProcessMode =@ProcessMode  ";
            + " AND Cast(SesDtTimeEnd as Date) = @SesDtTimeEnd and ProcessMode <> -1 ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@SesDtTimeEnd", InSesDtTimeEnd.Date);
                        // cmd.Parameters.AddWithValue("@ProcessMode", InProcessMode);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReaderFields(rdr);

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


        //  string outcome = ""; // TO FACILITATE EXCEPTIONS 
        // Methods 
        // READ Session Traces
        // 
        public void ReadSessionsStatusTracesToFindFirstRecord(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            string SqlString = "SELECT TOP(1) * "
          + " FROM [dbo].[SessionsStatusTraces] "
          + " WHERE AtmNo=@AtmNo  ";
            //  + " ORDER by SesNo DESC  ";
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

                            ReaderFields(rdr);

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

        public void ReadSessionsStatusTracesToFindNextSesion(string InAtmNo, DateTime InCycleEnddate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            string SqlString = "SELECT TOP(1) * "
          + " FROM [dbo].[SessionsStatusTraces] "
          + " WHERE AtmNo=@AtmNo "
              //+"and SesDtTimeEnd > @SesDtTimeEnd  "
              + " ORDER by SesDtTimeStart DESC  ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesDtTimeEnd", InCycleEnddate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReaderFields(rdr);

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
        public void ReadSessionsStatusTracesToFindLastRecord(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            string SqlString = "SELECT TOP(1) * "
          + " FROM [dbo].[SessionsStatusTraces] "
          + " WHERE AtmNo=@AtmNo  "
            + " ORDER by SesNo DESC  ";
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

                            ReaderFields(rdr);

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



        ////  
        //// Read And Find By ATM and Date 
        //// 
        //// 
        //public void ReadSessionsStatusTracesByAtmAndDate(string InAtmNo, DateTime InReplDateTime)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    string SqlString = "SELECT *"
        //  + " FROM [dbo].[SessionsStatusTraces] "
        //  + " WHERE AtmNo=@AtmNo AND CAST(SesDtTimeEnd AS Date) = @SesDtTimeEnd AND ProcessMode = 0 ";
        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
        //                cmd.Parameters.AddWithValue("@SesDtTimeEnd", InReplDateTime);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    ReaderFields(rdr);

        //                }

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
        //}



        // Find last Trace No Replenished  
        // 
        public void FindLastReplCycleId(string InAtmNo)
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
                            Is_Updated_GL = (bool)rdr["UpdatedBatch"];
                            ProcessMode = (int)rdr["ProcessMode"];

                            if (ProcessMode >= 1)
                            {
                                LastReplCyclId = SesNo;
                                LLatestBatchNo = LatestBatchNo;
                                LUpdatedBatch = Is_Updated_GL;
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
        // 
        // Does this ATM have Repl Cycle? 
        // 
        public void FindIfReplCycleExist(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
            + " WHERE AtmNo = @AtmNo ";
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

                            ReaderFields(rdr);

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
        // Does this ATM have Repl Cycle? 
        // 
        public void READ_CycleIn_Progress(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
            + " WHERE AtmNo = @AtmNo AND ProcessMode = -1 ";
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

                            ReaderFields(rdr);

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
        // Find within dates 
        public void READ_Cycle_WithinDate(string InAtmNo, DateTime InDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
            + " WHERE AtmNo = @AtmNo AND Cast(SesDtTimeStart as Date) >= @Date AND Cast(SesDtTimeEnd as Date) <= @Date "
            + " order by Sesno Desc ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Date", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReaderFields(rdr);

                            // break;

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

        // Find last Before that date 
        // Read all less and get last
        public void READ_Cycle_Last_WithinDate(string InAtmNo, DateTime InDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
            + " WHERE AtmNo = @AtmNo AND Cast(SesDtTimeStart as Date) <= @Date "
            + " order by Sesno Asc ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Date", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReaderFields(rdr);

                            // break;

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


        // Find Trace No Next to Replenish 
        // To locate last trace number 
        public int LastReconReady;
        public int LastReconciled;
        public int NextLatestBatchNo;
        public void FindNextAndLastReplCycleId(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool First = false;
            NextReplNo = 0;
            LastReconReady = 0;
            LastReconciled = 0;

            NextLatestBatchNo = 0;

            ReplOutstanding = 0;

            string SqlString = "SELECT   * "
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

                            PreSes = (int)rdr["PreSes"];

                            SesDtTimeStart = (DateTime)rdr["SesDtTimeStart"];
                            SesDtTimeEnd = (DateTime)rdr["SesDtTimeEnd"];

                            LatestBatchNo = (int)rdr["LatestBatchNo"];

                            if (ProcessMode == 0 & First == false
                               || ProcessMode == -1 & First == false
                               ) // The first 0 or -1 
                            {
                                NextReplNo = SesNo;
                                if (ProcessMode == 0) First = true;
                                if (ProcessMode == 0)
                                    NextLatestBatchNo = LatestBatchNo;
                            }

                            if (ProcessMode == 0 || ProcessMode == 4 || ProcessMode == -6) // Outstanding to be replenished
                                                                                           // the process mode is for excel not in agreement
                            {
                                ReplOutstanding = ReplOutstanding + 1;
                            }

                            if (ProcessMode == -1) // The last -1  
                            {
                                Last_1 = SesNo; // Current Ses No 
                            }
                            if (ProcessMode == 1) // The last Ready For Reconciliation 
                            {
                                LastReconReady = SesNo; // Last Reconciliation Ready
                            }
                            if (ProcessMode == 2) // Last Reconciled
                            {
                                LastReconciled = SesNo; //  Last Reconciled
                                NextLatestBatchNo = LatestBatchNo;
                            }

                            // THESE ARE FOR THE LAST Record
                            LastNo = SesNo;

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

                    CatchDetails(ex);
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
                            + "[SesDtTimeStart]= @SesDtTimeStart,[SesDtTimeEnd] = @SesDtTimeEnd, [SM_LAST_CLEARED]= @SM_LAST_CLEARED, "
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
                             + " [RecAtRMCycle] = @RecAtRMCycle, "
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
                           + "[UpdatedBatch] = @UpdatedBatch,"
                           + "[Maker] = @Maker,"
                           + "[Authoriser] = @Authoriser,"
                           + "[Last] = @Last, [InProcess] = @InProcess,"
                           + "[ProcessMode] = @ProcessMode, "
                             + "[ReconcAtRMCycle] = @ReconcAtRMCycle, "
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

                        cmd.Parameters.AddWithValue("@SM_LAST_CLEARED", SM_LAST_CLEARED);

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
                        cmd.Parameters.AddWithValue("@RecAtRMCycle", Recon1.RecAtRMCycle);
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

                        cmd.Parameters.AddWithValue("@UpdatedBatch", Is_Updated_GL);

                        cmd.Parameters.AddWithValue("@Maker", Maker);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);

                        cmd.Parameters.AddWithValue("@Last", Last);
                        cmd.Parameters.AddWithValue("@InProcess", InProcess);

                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);
                        cmd.Parameters.AddWithValue("@ReconcAtRMCycle", ReconcAtRMCycle);

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

                    CatchDetails(ex);
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
        // 
        // UPDATE TRACES For MAX Date the running cycles
        //

        public void UpdateTracesWithMaxDate(string InAtmNo, DateTime InMax_Date)
        {

            int rows = 0;

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
                        + " [SesDtTimeEnd] = @SesDtTimeEnd "
                        + " WHERE AtmNo= @AtmNo AND ProcessMode = -1 ", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@SesDtTimeEnd", InMax_Date);

                        // Execute and check success 
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
        // AT the time of UNDO take care the atms with sessions not -1
        // 
        public DataTable RMDataTableRight = new DataTable();

        public void ReadNotesSesionsByAndCorrectLast(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableRight = new DataTable();
            RMDataTableRight.Clear();

            string SqlString =
            " SELECT AtmNo, Max(SesNo) as MaxSesNo "
               + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
               //  + " WHERE Origin = 'Our Atms' "
               + " Group by AtmNo "
               + " ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        //  sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        sqlAdapt.Fill(RMDataTableRight);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            int I = 0;

            while (I <= (RMDataTableRight.Rows.Count - 1))
            {

                string WAtmNo = (string)RMDataTableRight.Rows[I]["AtmNo"];
                int WMaxSesNo = (int)RMDataTableRight.Rows[I]["MaxSesNo"];

                ReadSessionsStatusTraces(WAtmNo, WMaxSesNo);

                if (ProcessMode == -1)
                {
                    // Do nothing .. it has the right value
                }
                else
                {
                    ProcessMode = -1;

                    UpdateSessionsStatusTraces(WAtmNo, WMaxSesNo);
                }

                I++; // Read Next entry of the table 

            }
        }

        public void DeleteALL_during_Undo(string InOperator, int InLoadedAtRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //DELETE FROM[ATMS].[dbo].[ReconcFileMonitorLog]
   //     WHERE RMCycleNo = @RMCycleNo AND SourceFileID<> 'Atms_Journals_Txns'
	  //set @rowcount =  @@ROWCOUNT
  

              int count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SessionsStatusTraces] "
                            + " WHERE LoadedAtRMCycle =  @LoadedAtRMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);

                        count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SessionsNotesAndValues] "
                            + " WHERE LoadedAtRMCycle =  @LoadedAtRMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);

                        count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SessionsPhysicalInspection] "
                            + " WHERE LoadedAtRMCycle =  @LoadedAtRMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);

                        count = cmd.ExecuteNonQuery();

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
        // Delete a Cycle 
        //
        public void Delete_A_Cycle(string InAtmNo, int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SessionsStatusTraces] "
                            + " WHERE AtmNo =  @AtmNo AND SesNo = @SesNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SessionsNotesAndValues] "
                            + " WHERE AtmNo =  @AtmNo AND SesNo = @SesNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SessionsPhysicalInspection] "
                            + " WHERE AtmNo =  @AtmNo AND SesNo = @SesNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            //
            // DELETE Uncomplete Actions Occurances 
            //
            count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" DELETE FROM [ATMS].[dbo].[Actions_Occurances] "
                            + " WHERE AtmNo =  @AtmNo AND ReplCycle = @ReplCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InSesNo);

                        count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            //
            // DELETE Uncomplete authorisation
            //
            count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" DELETE FROM [ATMS].[dbo].[AuthorizationTable] "
                            + " WHERE AtmNo =  @AtmNo AND ReplCycle = @ReplCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InSesNo);

                        count = cmd.ExecuteNonQuery();

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



        // UPDATE TRACES AT REPlENISHMENT END AND RECONCILIATION END 
        //

        public void UpdateTracesFinishReplOrReconc(string InAtmNo, int InSesNo, string InUserId, int WMode)
        {
            // If WMode = 1 then REPLENISHEMNT HAS FINISHED 
            // If WMode = 2 then RECONCILIATION HAS FINISHED 

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            Ta.ReadSessionsStatusTraces(InAtmNo, InSesNo);

            AutoRec = false;

            int WFunction = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 
            Na.ReadSessionsNotesAndValues(InAtmNo, InSesNo, WFunction); // CALL

            ReadSessionsStatusTraces(InAtmNo, InSesNo);

            if (Na.SystemTargets1.LastTrace < FirstTraceNo & Na.SystemTargets2.LastTrace < FirstTraceNo &
              Na.SystemTargets3.LastTrace < FirstTraceNo & Na.SystemTargets4.LastTrace < FirstTraceNo &
              Na.SystemTargets5.LastTrace < FirstTraceNo)
            {
                Is_Updated_GL = false; // HOST FILES NOT RECEIVED YET 
            }
            else
            {
                Is_Updated_GL = true; // HOST FILES RECEIVED 
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
            if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false & Is_Updated_GL == true)
            {
                // NO DIFFERENCES 

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
                if (WMode == 1 & Is_Updated_GL == true) // During Replenishment no difference found 
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

                if (WMode == 2)
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
                    Diff1.DiffCurr1 = 0;

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

            if (AutoRec == true & Is_Updated_GL == true & WMode == 1)
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



        // UPDATE TRACES AT REPLENISHMENT END 
        //
        // FOR G4S
        DateTime Last_Cut_Off_Date;
        int WReconcCycleNo;
        public void UpdateTracesFinishRepl_From_Form51(string InAtmNo, int InSesNo, string InUserId, string InAuthoriser,
            string InReconcCategoryId)
        {
            //
            // THIS METHOD IS CALLED WHEN REPLENISMENT HAS FINISHED 
            //
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            Ac.ReadAtm(InAtmNo);


            if (Ac.Operator == "BCAIEGCX")
            {
                //
                // BANK DE CAIRE CASE
                //
                // All necesssary GL transactions had been made at the level of replenishment 
                // Any differences has been moved to the General ATM suspense for Branch. 
                ReadSessionsStatusTraces(InAtmNo, InSesNo);

                //if (Na.Is_GL_Adjusted == false)
                //{
                //    Is_Updated_GL = false; // HOST FILES NOT RECEIVED YET 

                //    if (Na.IsNewAtm == true)
                //    {
                //        Is_Updated_GL = true; // FORCE IT TO TRUE 
                //    }
                //}
                //else
                //{
                //    Is_Updated_GL = true; // HOST FILES RECEIVED 
                //}

                Repl1.FinishRepl = true;
                Repl1.ReplFinDtTm = DateTime.Now;

                Recon1.StartReconc = false;
                Recon1.FinishReconc = false;
                Recon1.DiffReconcEnd = true;

                //
                // NO DIFFERENCES FOUND DURING REPLENISHMENT 
                // BECAUSE HAS BEEN MOVED TO SUSPENSE
                // Initialise 
                NumOfErrors = 0;
                ErrOutstanding = 0;
                Diff1.CurrNm1 = "";
                Diff1.DiffCurr1 = 0;

                BalSetsNo = 1;

                Repl1.DiffRepl = false; // No Difference is identified at Replenishment 
                Repl1.ErrsRepl = false;

                AutoRec = true;

                Recon1.SignIdReconc = InUserId; // Find out 
                Recon1.DelegRecon = false;
                Recon1.StartReconc = false;
                Recon1.FinishReconc = false;
                Recon1.RecStartDtTm = NullDate.Date;
                Recon1.RecFinDtTm = NullDate.Date;
                Recon1.DiffReconcStart = false; // Difference at Reconciliation Start 
                Recon1.DiffReconcEnd = false;

                // SEt the same for both 
                Maker = InUserId;
                Authoriser = InAuthoriser;


                SessionsInDiff = 0;
                // Here Update all next Traces with zero 
               // LatestBatchNo = 999;

                ProcessMode = 2; // FULLY RECONCILED AT REPLENISHMENT BECAUSE DIFFERENCE HAS BEEN MOVED TO SUSPENSE

                //
                UpdateSessionsStatusTraces(InAtmNo, InSesNo); // UPDATE TRACES 

                //
                // UPDATE ATMs MAIN For this ATM - With latest Information 
                //
                Am.ReadAtmsMainSpecific(InAtmNo);

                //Am.LastReplDt = DateTime.Now;


                // Reconciled 

                Am.GL_ReconcDiff = false;

                Am.LastReplDt = Ta.SesDtTimeEnd;

                Am.GL_ReconcDate = DateTime.Now;
                Am.GL_Balance_At_CutOff = Na.GL_Balance;

                //  Ec.ReadAllErrorsTableForCounters(Am.Operator, "", InAtmNo, InSesNo, "");

                Am.GL_At_Repl = Na.GL_Bal_Repl_Adjusted;

                Am.GL_Counted = Na.Balances1.CountedBal;

                Am.LastReplDt = SesDtTimeStart;

                Am.GL_ReplenishmentDt = Repl1.ReplFinDtTm;
                Am.GL_ReplCycle = InSesNo;

                Am.GL_CurrNm1 = Diff1.CurrNm1;
                Am.GL_DiffCurr1 = Diff1.DiffCurr1;
                Am.GL_DiffCurr1 = 0;
                Am.GL_OutStandingErrors = 0;

                Am.SessionsInDiff = SessionsInDiff;
                Am.ReplCycleNo = InSesNo;
                Am.ReconcCycleNo = WReconcCycleNo;

                Ec.ReadAllErrorsTableForCounters(Am.Operator, InReconcCategoryId, InAtmNo, InSesNo, "All");
                Am.ErrOutstanding = Ec.NumOfErrors;

                if (Am.CurrentSesNo == InSesNo)
                {
                    Am.ProcessMode = ProcessMode;
                }
                //
                // Update AM
                //
                Am.UpdateAtmsMain(InAtmNo);


                if (ProcessMode == 1 & WReconcCycleNo > 0)
                {
                    // There is difference

                    string recgroup = "RECATMS-" + Ac.AtmsReconcGroup;
                    Rcs.ReadReconcCategoriesSessionsSpecific(Am.Operator, recgroup, WReconcCycleNo);

                    Rcs.GL_Original_Atms_Cash_Diff = Rcs.GL_Original_Atms_Cash_Diff + 1;

                    Rcs.GL_Remain_Atms_Cash_Diff = Rcs.GL_Remain_Atms_Cash_Diff + 1;

                    Rcs.UpdateReconcCategorySession_ForAtms_Cash_Diff(recgroup, WReconcCycleNo);

                }

            }

            // ***********************
            // If Bank de Caire ..... 
            // ************
            // AFTER THIS LINE THERE IS CODE THAT WE MAY NEED IT in the future
            // However now we return
            return; 

            if (Ac.Operator == "BCAIEGCX")
            {
                // If Bank De Caire Return
                return;
            }

            Ta.ReadSessionsStatusTraces(InAtmNo, InSesNo);

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            Rjc.Find_GL_Cut_Off_Before_GivenDate(Ta.Operator, Ta.SesDtTimeEnd.Date);
            if (Rjc.RecordFound == true & Rjc.Counter == 0)
            {
                // Cut off of previous to Replenishment 
                Last_Cut_Off_Date = Rjc.Cut_Off_Date;
                WReconcCycleNo = Rjc.JobCycle;
                //IsDataFound = true;
            }
            else
            {
                //IsDataFound = false;
                WReconcCycleNo = 0;
            }


            int WFunction = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 
            Na.ReadSessionsNotesAndValues(InAtmNo, InSesNo, WFunction); // CALL

            ReadSessionsStatusTraces(InAtmNo, InSesNo);

            if (Na.Is_GL_Adjusted == false)
            {
                Is_Updated_GL = false; // HOST FILES NOT RECEIVED YET 

                if (Na.IsNewAtm == true)
                {
                    Is_Updated_GL = true; // FORCE IT TO TRUE 
                }
            }
            else
            {
                Is_Updated_GL = true; // HOST FILES RECEIVED 
            }

            Repl1.FinishRepl = true;
            Repl1.ReplFinDtTm = DateTime.Now;

            Recon1.StartReconc = false;
            Recon1.FinishReconc = false;
            Recon1.DiffReconcEnd = true;

            // UPDATE SESSION TRACES 
            //
            if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false & Is_Updated_GL == true)
            {
                //
                // NO DIFFERENCES FOUND DURING REPLENISHMENT 
                //
                // Initialise 
                NumOfErrors = 0;
                ErrOutstanding = 0;
                Diff1.CurrNm1 = "";
                Diff1.DiffCurr1 = 0;

                BalSetsNo = 1;

                Repl1.DiffRepl = false; // No Difference is identified at Replenishment 
                Repl1.ErrsRepl = false;

                AutoRec = true;

                Recon1.SignIdReconc = InUserId; // Find out 
                Recon1.DelegRecon = false;
                Recon1.StartReconc = false;
                Recon1.FinishReconc = false;
                Recon1.RecStartDtTm = NullDate.Date;
                Recon1.RecFinDtTm = NullDate.Date;
                Recon1.DiffReconcStart = false; // Difference at Reconciliation Start 
                Recon1.DiffReconcEnd = false;

                SessionsInDiff = 0;
                // Here Update all next Traces with zero 
                LatestBatchNo = 999;

                ProcessMode = 2; // FULLY RECONCILED AT REPLENISHMENT 
            }
            else
            {
                //   DiffRepl = true;

                Repl1.ErrsRepl = true;
                Repl1.DiffRepl = true;
                SessionsInDiff = SessionsInDiff + 1;

                Recon1.DiffReconcStart = true;
                Recon1.DiffReconcEnd = true;

                Ec.ReadAllErrorsTableForCounterReplCycle(BankId, InAtmNo, InSesNo);

                NumOfErrors = Ec.NumOfErrors;
                ErrOutstanding = Ec.NumOfErrors - Ec.ErrUnderAction;
                BalSetsNo = Na.BalSets;

                Diff1.CurrNm1 = Na.BalDiff1.CurrNm;
                Diff1.DiffCurr1 = Na.BalDiff1.HostAdj;

                LatestBatchNo = 999;
                BalSetsNo = 1;
                //
                // SET PROCESS MODE 
                //
                ProcessMode = 1;
            }
            //
            // UPDATE SESSION TRACES
            //

            UpdateSessionsStatusTraces(InAtmNo, InSesNo); // UPDATE TRACES 

            //
            // UPDATE ATMs MAIN For this ATM - With latest Information 
            //
            Am.ReadAtmsMainSpecific(InAtmNo);

            //Am.LastReplDt = DateTime.Now;

            //TEST 
            if (InAtmNo == "AB102" || InAtmNo == "ServeUk102" || InAtmNo == "ABC501")
            {
                DateTime WDTm = new DateTime(2014, 02, 28);
                Am.LastReplDt = WDTm;
            }
            if (InAtmNo == "AB104" || InAtmNo == "ABC502")
            {
                DateTime WDTm = new DateTime(2014, 02, 13);
                Am.LastReplDt = WDTm;
            }

            if (Repl1.DiffRepl == true)
            {
                Am.GL_ReconcDiff = true;
                Am.LastReplDt = Ta.SesDtTimeEnd;
                Am.GL_ReconcDate = DateTime.Now;
                Am.GL_Balance_At_CutOff = Na.GL_Balance;
                //Ec.ReadAllErrorsTableForCounters(Am.Operator, "", InAtmNo, InSesNo, "");

                Am.GL_At_Repl = Na.Balances1.HostBalAdj;

                Am.GL_Counted = Na.Balances1.CountedBal;

                Am.GL_ReplenishmentDt = SesDtTimeEnd;
                Am.GL_ReplCycle = InSesNo;

                Am.GL_CurrNm1 = Diff1.CurrNm1;
                Am.GL_DiffCurr1 = Diff1.DiffCurr1;

                Am.GL_OutStandingErrors = ErrOutstanding;

                Am.Maker = "No Decision";
                Am.Authoriser = "No Decision";

            }
            else
            {
                // Reconciled 

                Am.GL_ReconcDiff = false;

                Am.LastReplDt = Ta.SesDtTimeEnd;

                Am.GL_ReconcDate = DateTime.Now;
                Am.GL_Balance_At_CutOff = Na.GL_Balance;

                //  Ec.ReadAllErrorsTableForCounters(Am.Operator, "", InAtmNo, InSesNo, "");

                Am.GL_At_Repl = Na.GL_Bal_Repl_Adjusted;

                Am.GL_Counted = Na.Balances1.CountedBal;

                Am.GL_ReplenishmentDt = Repl1.ReplFinDtTm;
                Am.GL_ReplCycle = InSesNo;

                Am.GL_CurrNm1 = Diff1.CurrNm1;
                Am.GL_DiffCurr1 = Diff1.DiffCurr1;
                Am.GL_DiffCurr1 = 0;
                Am.GL_OutStandingErrors = 0;
            }

            Am.SessionsInDiff = SessionsInDiff;
            Am.ReplCycleNo = InSesNo;
            Am.ReconcCycleNo = WReconcCycleNo;

            Ec.ReadAllErrorsTableForCounters(Am.Operator, InReconcCategoryId, InAtmNo, InSesNo, "All");
            Am.ErrOutstanding = Ec.NumOfErrors;

            if (Am.CurrentSesNo == InSesNo)
            {
                Am.ProcessMode = ProcessMode;
            }
            //
            // Update AM
            //
            Am.UpdateAtmsMain(InAtmNo);

            if (ProcessMode == 1 & WReconcCycleNo > 0)
            {
                // There is difference

                string recgroup = "RECATMS-" + Ac.AtmsReconcGroup;
                Rcs.ReadReconcCategoriesSessionsSpecific(Am.Operator, recgroup, WReconcCycleNo);

                Rcs.GL_Original_Atms_Cash_Diff = Rcs.GL_Original_Atms_Cash_Diff + 1;

                Rcs.GL_Remain_Atms_Cash_Diff = Rcs.GL_Remain_Atms_Cash_Diff + 1;

                Rcs.UpdateReconcCategorySession_ForAtms_Cash_Diff(recgroup, WReconcCycleNo);

            }
            else
            {
                Am.ReadAtmsMainSpecific(InAtmNo);
                Am.Maker = "N/A";
                Am.Authoriser = "N/A";
                Am.UpdateAtmsMain(InAtmNo);
            }

            // UPDATE BANK RECORD WITH UNLOADED COUNTED

            if (InAtmNo == "AB102" || InAtmNo == "AB104")
            {
                // These are the testing ATMs
            }
            else
            {
                // Find If AUDI Type 
                // If found and it is 1 is Audi Type If Zero then is normal 
                RRDMGasParameters Gp = new RRDMGasParameters();
                
                bool AudiType = false;
                int IsAmountOneZero;
                string WOperator = "AUDBEGCA"; 
                Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
                if (Gp.RecordFound == true)
                {
                    IsAmountOneZero = (int)Gp.Amount;

                    if (IsAmountOneZero == 1)
                    {
                        // Transactions will be done at the end 
                        AudiType = true;


                    }
                    else
                    {
                        AudiType = false;
                    }
                }
                else
                {
                    AudiType = false;
                }

                if (AudiType == true)
                {
                    RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

                    string WSelectionCriteria = " Where AtmNo='" + InAtmNo + "' AND ReplCycleNo =" + InSesNo;

                    G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(WSelectionCriteria, 2);// Mode is 2 for Bank record

                    RRDMReplOrdersClass Ra = new RRDMReplOrdersClass();
                    Ra.ReadReplActionsForAtmReplCycleNo(InAtmNo, InSesNo);

                    if (Ra.RecordFound)
                    {
                        G4.OrderNo = Ra.ReplOrderNo;
                        G4.OrderToBeLoaded = Ra.SystemAmount;
                    }
                    else
                    {
                        G4.OrderNo = 0;
                        G4.OrderToBeLoaded = 0;
                    }

                    G4.UnloadedCounted = Na.Balances1.CountedBal;

                    decimal Tempdiff = Na.Balances1.CountedBal - Na.Balances1.MachineBal;
                    if (Tempdiff > 0)
                    {
                        G4.OverFound = Tempdiff;
                    }
                    else
                    {
                        G4.ShortFound = -Tempdiff;
                    }

                    G4.PresentedErrors = Ec.TotalErrorsAmtLess100;

                    G4.ProcessMode_Load = 1;

                    G4.Cut_Off_date = Last_Cut_Off_Date;
                    G4.Gl_Balance_At_CutOff = Na.GL_Bal_Repl_Adjusted;

                    G4.RemarksG4S = "No Remark"; 

                    G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 2); // Mode is 2 for Bank record
                }

               
            }

        }


        public void UpdateTracesFinishRepl_From_Form152(string InAtmNo, int InSesNo, string InUserId, string InReconcCategoryId)
        {
            //
            // THIS METHOD IS CALLED FROM Form152 when the ATM is first introduced on RRDM 
            // There is no complete cycle 
            // 
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            Ac.ReadAtm(InAtmNo);


            //
            // BANK DE CAIRE CASE
            //
            // All necesssary GL transactions had been made at the level of replenishment 
            // Any differences has been moved to the General ATM suspense for Branch. 
            ReadSessionsStatusTraces(InAtmNo, InSesNo);


            Repl1.FinishRepl = true;
            Repl1.ReplFinDtTm = DateTime.Now;

            Recon1.StartReconc = false;
            Recon1.FinishReconc = false;
            Recon1.DiffReconcEnd = true;

            //
            // NO DIFFERENCES FOUND DURING REPLENISHMENT 
            // BECAUSE HAS BEEN MOVED TO SUSPENSE
            // Initialise 
            NumOfErrors = 0;
            ErrOutstanding = 0;
            Diff1.CurrNm1 = "";
            Diff1.DiffCurr1 = 0;

            BalSetsNo = 1;

            Repl1.DiffRepl = false; // No Difference is identified at Replenishment 
            Repl1.ErrsRepl = false;

            AutoRec = true;

            Recon1.SignIdReconc = InUserId;

            Recon1.DelegRecon = false;
            Recon1.StartReconc = false;
            Recon1.FinishReconc = false;
            Recon1.RecStartDtTm = NullDate.Date;
            Recon1.RecFinDtTm = NullDate.Date;
            Recon1.DiffReconcStart = false; // Difference at Reconciliation Start 
            Recon1.DiffReconcEnd = false;

            // SEt the same for both 
            Maker = InUserId;
            Authoriser = InUserId;


            SessionsInDiff = 0;
            // Here Update all next Traces with zero 
            LatestBatchNo = 999;

            ProcessMode = 2; // FULLY RECONCILED AT REPLENISHMENT BECAUSE DIFFERENCE HAS BEEN MOVED TO SUSPENSE

            //
            UpdateSessionsStatusTraces(InAtmNo, InSesNo); // UPDATE TRACES 

            //
            // UPDATE ATMs MAIN For this ATM - With latest Information 
            //
            Am.ReadAtmsMainSpecific(InAtmNo);

            //Am.LastReplDt = DateTime.Now;


            // Reconciled 

            Am.GL_ReconcDiff = false;

            Am.LastReplDt = Ta.SesDtTimeEnd;

            Am.GL_ReconcDate = DateTime.Now;
            Am.GL_Balance_At_CutOff = Na.GL_Balance;

            //  Ec.ReadAllErrorsTableForCounters(Am.Operator, "", InAtmNo, InSesNo, "");

            Am.GL_At_Repl = Na.GL_Bal_Repl_Adjusted;

            Am.GL_Counted = Na.Balances1.CountedBal;

            Am.LastReplDt = SesDtTimeStart;

            Am.GL_ReplenishmentDt = Repl1.ReplFinDtTm;
            Am.GL_ReplCycle = InSesNo;

            Am.GL_CurrNm1 = Diff1.CurrNm1;
            Am.GL_DiffCurr1 = Diff1.DiffCurr1;
            Am.GL_DiffCurr1 = 0;
            Am.GL_OutStandingErrors = 0;

            Am.SessionsInDiff = SessionsInDiff;
            Am.ReplCycleNo = InSesNo;
            Am.ReconcCycleNo = WReconcCycleNo;

            Ec.ReadAllErrorsTableForCounters(Am.Operator, InReconcCategoryId, InAtmNo, InSesNo, "All");
            Am.ErrOutstanding = Ec.NumOfErrors;

            if (Am.CurrentSesNo == InSesNo)
            {
                Am.ProcessMode = ProcessMode;
            }
            //
            // Update AM
            //
            Am.UpdateAtmsMain(InAtmNo);

            if (Ac.Operator == "BCAIEGCX")
            {
                // If Bank De Caire Return
                return;
            }

        }



        // UPDATE TRACES AT REPlENISHMENT END FROM G4S
        //
        // FOR G4S

        public void UpdateTracesFinishRepl_From__G4S(string InAtmNo, int InSesNo,
                                                  string InUserId, string InReconcCategoryId)
        {
            //
            // THIS METHOD IS CALLED WHEN REPLENISMENT HAS FINISHED 
            //
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

            Ta.ReadSessionsStatusTraces(InAtmNo, InSesNo);

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            Rjc.Find_GL_Cut_Off_Before_GivenDate(Ta.Operator, Ta.SesDtTimeEnd.Date);
            if (Rjc.RecordFound == true & Rjc.Counter == 0)
            {
                // Cut off of prvious to Replenishment 
                Last_Cut_Off_Date = Rjc.Cut_Off_Date;
                WReconcCycleNo = Rjc.JobCycle;
                //IsDataFound = true;
            }
            else
            {
                //IsDataFound = false;
            }

            int WFunction = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 
            Na.ReadSessionsNotesAndValues(InAtmNo, InSesNo, WFunction); // CALL

            ReadSessionsStatusTraces(InAtmNo, InSesNo);

            if (Na.Is_GL_Adjusted == false)
            {
                Is_Updated_GL = false; // HOST FILES NOT RECEIVED YET 

                if (Na.IsNewAtm == true)
                {
                    Is_Updated_GL = true; // FORCE IT TO TRUE 
                }
            }
            else
            {
                Is_Updated_GL = true; // HOST FILES RECEIVED 
            }

            //Repl1.FinishRepl = true;
            //Repl1.ReplFinDtTm = DateTime.Now;

            Recon1.StartReconc = false;
            Recon1.FinishReconc = false;
            Recon1.DiffReconcEnd = true;

            // UPDATE SESSION TRACES 
            //
            if (Na.DiffAtAtmLevel_Cit == false & Na.DiffAtHostLevel_Cit == false & Na.DiffWithErrors_Cit == false & Is_Updated_GL == true)
            {
                //
                // NO DIFFERENCES FOUND DURING REPLENISHMENT 
                //
                // Initialise 
                NumOfErrors = 0;
                ErrOutstanding = 0;
                Diff1.CurrNm1 = Na.BalDiff1.CurrNm;
                Diff1.DiffCurr1 = 0;

                BalSetsNo = 1;

                Repl1.DiffRepl = false; // No Difference is identified at Replenishment 
                Repl1.ErrsRepl = false;

                AutoRec = true;

                Recon1.SignIdReconc = InUserId; // Find out 
                Recon1.DelegRecon = false;
                Recon1.StartReconc = false;
                Recon1.FinishReconc = false;
                Recon1.RecStartDtTm = NullDate.Date;
                Recon1.RecFinDtTm = NullDate.Date;
                Recon1.DiffReconcStart = false; // Difference at Reconciliation Start 
                Recon1.DiffReconcEnd = false;

                // SEt the same for both 
                Maker = InUserId;
                Authoriser = InUserId;


                SessionsInDiff = 0;
                // Here Update all next Traces with zero 
                LatestBatchNo = 999;

                ProcessMode = 2; // FULLY RECONCILED AT REPLENISHMENT 
            }
            else
            {
                //   DiffRepl = true;

                Repl1.ErrsRepl = true;
                Repl1.DiffRepl = true;

                Recon1.DiffReconcStart = true;
                Recon1.DiffReconcEnd = true;

                Ec.ReadAllErrorsTableForCounterReplCycle(BankId, InAtmNo, InSesNo);

                NumOfErrors = Ec.NumOfErrors;
                ErrOutstanding = Ec.NumOfErrors - Ec.ErrUnderAction;
                BalSetsNo = Na.BalSets;

                Diff1.CurrNm1 = Na.BalDiff1.CurrNm;
                Diff1.DiffCurr1 = Na.BalDiff1.HostAdj;

                LatestBatchNo = 999;
                BalSetsNo = 1;

                SessionsInDiff = SessionsInDiff + 1;
                //
                // SET PROCESS MODE 
                //
                ProcessMode = 1;
            }
            //
            // UPDATE SESSION TRACES
            //

            UpdateSessionsStatusTraces(InAtmNo, InSesNo); // UPDATE TRACES 

            //
            // UPDATE ATMs MAIN For this ATM - With latest Information 
            //

            ReadSessionsStatusTraces(InAtmNo, InSesNo);

            Am.ReadAtmsMainSpecific(InAtmNo);

            if (Repl1.DiffRepl == true)
            {
                Am.GL_ReconcDiff = true;

                Am.GL_ReconcDate = DateTime.Now;
                Am.GL_Balance_At_CutOff = Na.GL_Balance;

                Ec.ReadAllErrorsTableForCounters(Am.Operator, "", InAtmNo, InSesNo, "");

                Am.GL_At_Repl = Na.GL_Bal_Repl_Adjusted;

                Am.GL_Counted = Na.Cit_UnloadedCounted;

                Am.GL_ReplenishmentDt = Repl1.ReplFinDtTm;
                Am.GL_ReplCycle = InSesNo;

                Am.GL_CurrNm1 = Diff1.CurrNm1;
                Am.GL_DiffCurr1 = Diff1.DiffCurr1;

                //Am.GL_OutStandingErrors = ErrOutstanding;

                Am.Maker = "No Decision";
                Am.Authoriser = "No Decision";


            }
            else
            {
                // Reconciled 

                Am.GL_ReconcDiff = false;

                Am.GL_ReconcDate = DateTime.Now;
                Am.GL_Balance_At_CutOff = Na.GL_Balance;
                //Ec.ReadAllErrorsTableForCounters(Am.Operator, "", InAtmNo, InSesNo, "");

                Am.GL_At_Repl = Na.GL_Bal_Repl_Adjusted;

                Am.GL_Counted = Na.Cit_UnloadedCounted;

                Am.GL_ReplenishmentDt = Repl1.ReplFinDtTm;
                Am.GL_ReplCycle = InSesNo;

                Am.GL_CurrNm1 = Diff1.CurrNm1;
                Am.GL_DiffCurr1 = Diff1.DiffCurr1;
                // Am.GL_CurrNm1 = "";
                Am.GL_DiffCurr1 = 0;
            }

            Am.SessionsInDiff = SessionsInDiff;
            Am.ReplCycleNo = InSesNo;
            Am.ReconcCycleNo = WReconcCycleNo;

            Ec.ReadAllErrorsTableForCounters(Am.Operator, InReconcCategoryId, InAtmNo, InSesNo, "All");

            Am.ErrOutstanding = Ec.NumOfErrors;

            if (Am.CurrentSesNo == InSesNo)
            {
                Am.ProcessMode = ProcessMode;
            }
            //
            // Update AM
            //
            Am.UpdateAtmsMain(InAtmNo);
        }

        public void UpdateTracesFinishReconc_From_Form52c(string InAtmNo, int InSesNo, string InUserId, string InAuther, int InRMCycle)
        {

            // If WMode = 2 then RECONCILIATION HAS FINISHED 

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

            //int WFunction = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 
            //Na.ReadSessionsNotesAndValues(InAtmNo, InSesNo, WFunction); // CALL

            ReadSessionsStatusTraces(InAtmNo, InSesNo);

            //if (Na.Is_GL_Adjusted == false)
            //{
            //    Is_Updated_GL = false; // HOST FILES NOT RECEIVED YET 

            //    if (Na.IsNewAtm == true)
            //    {
            //        Is_Updated_GL = true; // FORCE IT TO TRUE 
            //    }
            //}
            //else
            //{
            //    Is_Updated_GL = true; // HOST FILES RECEIVED 
            //}

            // UPDATE SESSION TRACES 
            //
            if (Na.Operator == "CRBAGRAA")
            {
                if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false)
                {
                    //// AFTER RECONCILIATION THERE ARE NO DIFFERENCES         
                    Recon1.SignIdReconc = InUserId;

                    Recon1.DelegRecon = false;
                    //   Recon1.StartReconc = true;
                    Recon1.FinishReconc = true;
                    //     Recon1.RecStartDtTm = DateTime.Now;
                    Recon1.RecFinDtTm = DateTime.Now;

                    //     Recon1.DiffReconcStart = false;
                    Recon1.DiffReconcEnd = false;

                    SessionsInDiff = 0;
                    // Here Update all next Traces with zero 
                    LatestBatchNo = 999;

                    Recon1.RecAtRMCycle = InRMCycle;

                    ProcessMode = 2;

                }
                else
                {
                }
            }
            else
            {
                //if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false & Is_Updated_GL == true)
                //{
                //// AFTER RECONCILIATION THERE ARE NO DIFFERENCES         
                Recon1.SignIdReconc = InUserId;

                Recon1.DelegRecon = false;
                //   Recon1.StartReconc = true;
                Recon1.FinishReconc = true;
                //     Recon1.RecStartDtTm = DateTime.Now;
                Recon1.RecFinDtTm = DateTime.Now;

                //     Recon1.DiffReconcStart = false;
                Recon1.DiffReconcEnd = false;

                SessionsInDiff = 0;
                // Here Update all next Traces with zero 
                LatestBatchNo = 999;

                Recon1.RecAtRMCycle = InRMCycle;

                Maker = InUserId;

                Authoriser = InAuther;

                ProcessMode = 2;

                //}
                //else
                //{
                //}
            }


            //
            // UPDATE SESSION TRACES
            //

            UpdateSessionsStatusTraces(InAtmNo, InSesNo); // UPDATE TRACES 

            //
            // UPDATE ATMs MAIN For this ATM - With latest Information 
            //

            // Read updated information
            //
            ReadSessionsStatusTraces(InAtmNo, InSesNo);

            // Read Atms Main for updating 
            Am.ReadAtmsMainSpecific(InAtmNo);

            Am.ReconcCycleNo = InSesNo;
            Am.ReconcDt = Recon1.RecFinDtTm;

            Am.LastUpdated = DateTime.Now;

            if (Am.CurrentSesNo == InSesNo)
            {
                Am.ProcessMode = ProcessMode;
            }

            Am.GL_ReconcDiff = false;
            Am.GL_CurrNm1 = "";
            Am.GL_DiffCurr1 = 0;

            Am.SessionsInDiff = 0;

            Am.ErrOutstanding = 0;
            //
            // Update AM
            //
            Am.UpdateAtmsMain(InAtmNo);
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
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
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
        //

    }
}

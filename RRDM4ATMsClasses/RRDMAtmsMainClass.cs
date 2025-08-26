using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMAtmsMainClass : Logger
    {
        public RRDMAtmsMainClass() : base() { }

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

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
        public bool ActionConfirmed;

        public DateTime LastDispensedHistor;
        public DateTime LastInNeedReview;

        public int SessionsInDiff;
        public int ErrOutstanding;
        public int ReplCycleNo;
        public int ReconcCycleNo;
        public DateTime ReconcDt;

        public int ProcessMode;

        public bool UnderReconcMode;

        public int AtmsReplGroup;
        public int AtmsReconcGroup;
        public bool ExpressAction;

        // LATEST GL INFOMATION 

        public bool GL_ReconcDiff;

        public DateTime GL_ReconcDate;
        public decimal GL_Balance_At_CutOff;

        public decimal GL_At_Repl;
        public decimal GL_Counted;

        public DateTime GL_ReplenishmentDt;
        public int GL_ReplCycle;

        public string GL_CurrNm1;
        public decimal GL_DiffCurr1;

        public int GL_OutStandingErrors;

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
        public DataTable TableATMsMainSelected = new DataTable();
        public DataTable TableATMsMainReplNeeds = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete

        public bool RecordFound;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //string ViewAtmsMain = "[ATMS].[dbo].[ViewAtmsMainVsUserOwners]";

        RRDMDepositsClass Dc = new RRDMDepositsClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        private void RdrFieldsForMain(SqlDataReader rdr)
        {
            AtmNo = (string)rdr["AtmNo"];
            CurrentSesNo = (int)rdr["CurrentSesNo"];

            AtmName = (string)rdr["AtmName"];

            BankId = (string)rdr["BankId"];
            RespBranch = (string)rdr["RespBranch"];
            BranchName = (string)rdr["BranchName"];

            LastReplDt = (DateTime)rdr["LastReplDt"];

            NextReplDt = (DateTime)rdr["NextReplDt"];

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

            ActionConfirmed = (bool)rdr["ActionConfirmed"];

            LastDispensedHistor = (DateTime)rdr["LastDispensedHistor"];

            LastInNeedReview = (DateTime)rdr["LastInNeedReview"];

            SessionsInDiff = (int)rdr["SessionsInDiff"];
            ErrOutstanding = (int)rdr["ErrOutstanding"];
            ReplCycleNo = (int)rdr["ReplCycleNo"];
            ReconcCycleNo = (int)rdr["ReconcCycleNo"];
            ReconcDt = (DateTime)rdr["ReconcDt"];

            ProcessMode = (int)rdr["ProcessMode"];

            UnderReconcMode = (bool)rdr["UnderReconcMode"];

            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];

            ExpressAction = (bool)rdr["ExpressAction"];

            GL_ReconcDiff = (bool)rdr["GL_ReconcDiff"];

            GL_ReconcDate = (DateTime)rdr["GL_ReconcDate"];
            GL_Balance_At_CutOff = (decimal)rdr["GL_Balance_At_CutOff"];

            GL_At_Repl = (decimal)rdr["GL_At_Repl"];
            GL_Counted = (decimal)rdr["GL_Counted"];

            GL_ReplenishmentDt = (DateTime)rdr["GL_ReplenishmentDt"];
            GL_ReplCycle = (int)rdr["GL_ReplCycle"];

            GL_CurrNm1 = (string)rdr["GL_CurrNm1"];
            GL_DiffCurr1 = (decimal)rdr["GL_DiffCurr1"];

            GL_OutStandingErrors = (int)rdr["GL_OutStandingErrors"];

            Maker = (string)rdr["Maker"];
            Authoriser = (string)rdr["Authoriser"];

            Operator = (string)rdr["Operator"];
        }

        // Methods 
        // READ ATMs Main For ALL ATMS specific authorised user 
        // FILL UP A TABLE 

        public void ReadViewAtmsMainForAuthUserAndFillTable(string InOperator, string InSignedId,
                                int InSignRecordNo, string InAtmNo, string InFromFunction, string InCitId)
        {
            //
            // METHOD DESCRIPTION
            //
            // If Security level is 02 then fill in table only the ATMs connected to this user
            // If Security level is 03 or 04 then all ATMs are included in Table
            // if CitId is presence then the ATMs of this CIT are shown 
            //
            RRDMUsersRecords Us = new RRDMUsersRecords();
            RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();

            RecordFound = false;

            TableATMsMainSelected = new DataTable();
            TableATMsMainSelected.Clear();

            // DATA TABLE ROWS DEFINITION 
            TableATMsMainSelected.Columns.Add("ATMNo", typeof(string));
            TableATMsMainSelected.Columns.Add("ReplCycle", typeof(string));

            TableATMsMainSelected.Columns.Add("ATMName", typeof(string));
            TableATMsMainSelected.Columns.Add("RespBranch", typeof(string));
            TableATMsMainSelected.Columns.Add("UserId", typeof(string));

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(InSignRecordNo);

            if (Usi.SecLevel == "02")
            {
                // Branch Officer
                // ACCESS Only your ATMs

                // READ ALL ATMS User Has
                // Case of Role 02 

                string WFilter = " UserId='" + InSignedId + "'";
                Uaa.ReadUserAccessToAtmsFillTable(WFilter);

                int I = 0;
                string AtmNo;
                bool Replenishment;
                while (I <= (Uaa.UsersToAtmsDataTable.Rows.Count - 1))
                {

                    RecordFound = true;

                    //       UserId = (string)Uaa.UsersToAtmsDataTable.Rows[I]["UserId"];

                    AtmNo = (string)Uaa.UsersToAtmsDataTable.Rows[I]["AtmNo"];
                    Replenishment = (bool)Uaa.UsersToAtmsDataTable.Rows[I]["Replenishment"];

                    // Read ATMs
                    ReadAtmsMainSpecific(AtmNo);

                    DataRow RowSelected = TableATMsMainSelected.NewRow();

                    RowSelected["AtmNo"] = AtmNo;
                    RowSelected["ReplCycle"] = CurrentSesNo;

                    RowSelected["AtmName"] = AtmName;
                    RowSelected["RespBranch"] = RespBranch;

                    RowSelected["UserId"] = InSignedId;

                    // ADD ROW
                    TableATMsMainSelected.Rows.Add(RowSelected);

                    I++;

                }

            }
            if (Usi.SecLevel == "03" || Usi.SecLevel == "04")
            {
                // Reconciliator Officer
                // Or Controller
                // ACCESS All ATMs
                TotalSelected = 0;

                if (InAtmNo == "")
                {
                    if (InFromFunction == "General")
                    {
                        SqlString = "SELECT *"
                                   + " FROM [ATMS].[dbo].[AtmsMain]  "
                                   + " WHERE Operator=@Operator ";
                    }

                    if (InFromFunction == "ATMsInNeed" & InCitId == "")
                    {
                        SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[ViewAtmsMainVsUserOwners]"
                        + " WHERE Operator=@Operator AND CurrentSesNo > 0";
                    }

                    if (InFromFunction == "ATMsInNeed" & InCitId != "")
                    {
                        SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[ViewAtmsMainVsUserOwners]"
                        + " WHERE Operator=@Operator AND CurrentSesNo > 0 AND CitId=@CitId ";
                    }


                    if (InFromFunction == "FromCit")
                    {
                        SqlString = "SELECT *"
                                + " FROM [ATMS].[dbo].[AtmsMain]  "
                                + " WHERE Operator=@Operator AND CitId=@CitId ";
                    }

                }

                if (InAtmNo != "")
                {
                    SqlString = "SELECT *"
                         + " FROM [ATMS].[dbo].[ViewAtmsMainVsUserOwners]"
                         + " WHERE Operator=@Operator AND UserId=@UserId AND AtmNo=@AtmNo ";
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
                            cmd.Parameters.AddWithValue("@UserId", InSignedId);
                            if (InAtmNo == "")
                            {
                                if (InFromFunction == "General" || InFromFunction == "PrepareMoneyIn")
                                {
                                    //cmd.Parameters.AddWithValue("@Operator", InOperator);
                                  //  cmd.Parameters.AddWithValue("@UserId", InSignedId);
                                }

                                if (InFromFunction == "FromCit" || InCitId != "")
                                {
                                    //cmd.Parameters.AddWithValue("@Operator", InOperator);
                                    cmd.Parameters.AddWithValue("@CitId", InCitId);
                                }

                            }
                            else
                            {
                                // InAtmNo != ""

                                //cmd.Parameters.AddWithValue("@UserId", InSignedId);
                                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                            }

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {

                                RecordFound = true;

                                ActionNo = (int)rdr["ActionNo"];
                                ActionConfirmed = (bool)rdr["ActionConfirmed"];

                                TotalSelected = TotalSelected + 1;

                                DataRow RowSelected = TableATMsMainSelected.NewRow();

                                RowSelected["AtmNo"] = (string)rdr["AtmNo"];
                                RowSelected["ReplCycle"] = (int)rdr["CurrentSesNo"];


                                RowSelected["AtmName"] = (string)rdr["AtmName"];
                                RowSelected["RespBranch"] = (string)rdr["RespBranch"];

                                RowSelected["UserId"] = InSignedId;

                                // ADD ROW
                                TableATMsMainSelected.Rows.Add(RowSelected);

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

        }

        // Methods 
        // READ ATMs Main For ATMs Replenishment need status 
        // FILL UP A TABLE 

        public void ReadAtmsMainForReplNeedsStatus(string InOperator, string InSignedId, int InSignRecordNo
            , string InCitId, DateTime InWDt, bool InTemp_NBG_FIX, int InOrderCycle)
        {
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 
            RRDMDepositsClass Da = new RRDMDepositsClass();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();
            RRDMReplDatesCalc Rc = new RRDMReplDatesCalc();
            RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();


            int WCurrentSesNo = 0;
            decimal WCurrCassettes = 0;
            decimal WCurrentDeposits = 0;
            int WCapturedCards = 0;
            int WErrorsOutstanding = 0;
            decimal WNewAmount = 0;


            RecordFound = false;

            TableATMsMainReplNeeds = new DataTable();
            TableATMsMainReplNeeds.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableATMsMainReplNeeds.Columns.Add("AtmNo", typeof(string));
            TableATMsMainReplNeeds.Columns.Add("AtmName", typeof(string));
            TableATMsMainReplNeeds.Columns.Add("ReplCycleNo", typeof(int));
            TableATMsMainReplNeeds.Columns.Add("NeedStatus", typeof(string));
            TableATMsMainReplNeeds.Columns.Add("OrderCycle", typeof(int));
            TableATMsMainReplNeeds.Columns.Add("CurrentCassetes", typeof(decimal));
            TableATMsMainReplNeeds.Columns.Add("DaysToLast", typeof(int));
            TableATMsMainReplNeeds.Columns.Add("Deposits", typeof(decimal));
            TableATMsMainReplNeeds.Columns.Add("CaptureCards", typeof(int));
            TableATMsMainReplNeeds.Columns.Add("Errors", typeof(int));
            TableATMsMainReplNeeds.Columns.Add("NextRepl", typeof(DateTime));
            TableATMsMainReplNeeds.Columns.Add("ToLoad", typeof(decimal));

            TableATMsMainReplNeeds.Columns.Add("UserId", typeof(string));

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(InSignRecordNo);
            // If from Branch 
            if (Usi.SecLevel == "02")
            {
                // Branch Officer
                // ACCESS Only your ATMs

                // READ ALL ATMS User Has
                // Case of Role 02 

                string WFilter = " UserId='" + InSignedId + "'";
                Ua.ReadUserAccessToAtmsFillTable(WFilter);

                int I = 0;
                string AtmNo;
                bool Replenishment;
                while (I <= (Ua.UsersToAtmsDataTable.Rows.Count - 1))
                {

                    RecordFound = true;

                    //       UserId = (string)Uaa.UsersToAtmsDataTable.Rows[I]["UserId"];

                    AtmNo = (string)Ua.UsersToAtmsDataTable.Rows[I]["AtmNo"];
                    Replenishment = (bool)Ua.UsersToAtmsDataTable.Rows[I]["Replenishment"];

                    // For each loop Read ATM from Main
                    ReadAtmsMainSpecific(AtmNo);

                    // For NBG Set variables
                    Ta.FindNextAndLastReplCycleId(AtmNo);
                    if (Ta.RecordFound == true)
                    {
                        WCurrentSesNo = Ta.Last_1; // Current open session
                        if (InTemp_NBG_FIX == true)
                        {
                            WCurrentSesNo = Ta.NextReplNo;
                        }
                        // Initialise variables

                        Na.ReadSessionsNotesAndValues(AtmNo, WCurrentSesNo, 2);
                        WCurrCassettes = Na.Balances1.ReplToRepl;
                        WCapturedCards = Na.CaptCardsMachine;
                        WErrorsOutstanding = Na.ErrOutstanding;

                        Da.ReadDepositsSessionsNotesAndValuesDeposits(AtmNo, WCurrentSesNo);
                        WCurrentDeposits = Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej + Da.DepositsMachine1.EnvAmount + Da.ChequesMachine1.Amount;

                        // Get Ro Amount 
                        Ro.ReadReplActionsForAtmReplCycleNo(AtmNo, WCurrentSesNo);
                        if (Ro.RecordFound == true)
                        {
                            WNewAmount = Ro.NewAmount;
                        }
                        else
                        {
                            WNewAmount = 0; 
                        }
                    }
                    else
                    {
                        WCurrentSesNo = 0;
                        WCurrCassettes = 0;
                        WCurrentDeposits = 0;
                        WCapturedCards = 0;
                        WErrorsOutstanding = 0;
                        WNewAmount = 0;
                    }


                    // NEW ROW 
                    DataRow RowSelected = TableATMsMainReplNeeds.NewRow();

                    RowSelected["AtmNo"] = AtmNo;
                    RowSelected["AtmName"] = AtmName;

                    RowSelected["ReplCycleNo"] = WCurrentSesNo;
             
                    //
                    // SWITCH StepNumber
                    //
                    switch (NeedType)
                    {
                        case 10:
                            {
                                RowSelected["NeedStatus"] = "NO NEED";
                                break;
                            }
                        case 11:
                            {
                                RowSelected["NeedStatus"] = "REPL DELAYED";
                                break;
                            }
                        case 12:
                            {
                                RowSelected["NeedStatus"] = "URGENT NEED";
                                RowSelected["NeedStatus"] = "USERs WISH";
                                break;
                            }
                        case 13:
                            {
                                RowSelected["NeedStatus"] = "NORMAL NEED OR ATM without SM";
                                break;
                            }
                        case 25:
                            {
                                RowSelected["NeedStatus"] = "Forced Order";
                                break;
                            }
                        default:
                            {
                                RowSelected["NeedStatus"] = "NOT DEFINED";
                                break;
                            }
                    }

                    RowSelected["OrderCycle"] = InOrderCycle;

                    RowSelected["CurrentCassetes"] = WCurrCassettes;
                    //
                    Rc.FindNextRepl(Operator, AtmNo, WCurrentSesNo, WCurrCassettes, InWDt);
                    RowSelected["DaysToLast"] = (Rc.NoOfDays).ToString();
                    //
                    RowSelected["Deposits"] = WCurrentDeposits;

                    RowSelected["CaptureCards"] = WCapturedCards.ToString();

                    RowSelected["Errors"] = WErrorsOutstanding.ToString();

                    RowSelected["NextRepl"] = Rc.EstReplDt;
                    RowSelected["ToLoad"] = WNewAmount;

                    RowSelected["UserId"] = InSignedId;

                    // ADD ROW
                    TableATMsMainReplNeeds.Rows.Add(RowSelected);

                    I++;

                }

            }


            if (Usi.SecLevel == "03" & InCitId != "1000")
            {
                SqlString = "SELECT *"
                    + " FROM [dbo].[AtmsMain]  "
                     + " WHERE Operator = @Operator AND CitId = @CitId AND  CurrentSesNo > 0 ";
                using (SqlConnection conn =
                         new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {

                            cmd.Parameters.AddWithValue("@Operator", InOperator);
                            cmd.Parameters.AddWithValue("@CitId", InCitId);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                RecordFound = true;

                                TotalSelected = TotalSelected + 1;

                                RdrFieldsForMain(rdr);

                                // NEW ROW 
                                DataRow RowSelected = TableATMsMainReplNeeds.NewRow();

                                RowSelected["AtmNo"] = AtmNo;
                                RowSelected["AtmName"] = AtmName;

                                RowSelected["ReplCycleNo"] = CurrentSesNo;

                                // Get Ro Amount 
                                Ro.ReadReplActionsForAtmReplCycleNo(AtmNo, WCurrentSesNo);
                                if (Ro.RecordFound == true)
                                {
                                    WNewAmount = Ro.NewAmount;
                                }
                                else
                                {
                                    WNewAmount = 0;
                                }
                                //
                                // SWITCH StepNumber
                                //
                                switch (NeedType)
                                {
                                    case 10:
                                        {
                                            RowSelected["NeedStatus"] = "NO NEED";
                                            break;
                                        }
                                    case 11:
                                        {
                                            RowSelected["NeedStatus"] = "REPL DELAYED";
                                            break;
                                        }
                                    case 12:
                                        {
                                            RowSelected["NeedStatus"] = "URGENT NEED";
                                            break;
                                        }
                                    case 13:
                                        {
                                            RowSelected["NeedStatus"] = "NORMAL NEED";
                                            break;
                                        }
                                    case 25:
                                        {
                                            RowSelected["NeedStatus"] = "Forced Order";
                                            break;
                                        }
                                    default:
                                        {
                                            RowSelected["NeedStatus"] = "NOT DEFINED";
                                            break;
                                        }
                                }

                                RowSelected["OrderCycle"] = InOrderCycle;

                                RowSelected["CurrentCassetes"] = CurrCassettes;
                                //
                                Rc.FindNextRepl(Operator, AtmNo, CurrentSesNo, CurrCassettes, InWDt);
                                RowSelected["DaysToLast"] = (Rc.NoOfDays).ToString();
                                //
                                RowSelected["Deposits"] = CurrentDeposits;

                                if (CurrentSesNo > 0)
                                {
                                    Na.ReadSessionsNotesAndValues(AtmNo, CurrentSesNo, 2);
                                    RowSelected["CaptureCards"] = Na.CaptCardsMachine.ToString();
                                }
                                else
                                {
                                    RowSelected["CaptureCards"] = 0;
                                }

                                RowSelected["Errors"] = ErrOutstanding.ToString(); ;

                                RowSelected["NextRepl"] = Rc.EstReplDt;
                                RowSelected["ToLoad"] = WNewAmount;

                                RowSelected["UserId"] = InSignedId;

                                // ADD ROW
                                TableATMsMainReplNeeds.Rows.Add(RowSelected);

                            }

                            // Close Reader
                            rdr.Close();
                        }
                        conn.Close();

                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);

                    }
            }
            RRDMReplOrders_Pre_ATMs Rpre = new RRDMReplOrders_Pre_ATMs();
            Rpre.DeleteReplOrders_Pre_ATMs(InOrderCycle);
            Rpre.Insert_In_ReplOrders_Pre_ATMs(InOperator, InSignedId, TableATMsMainReplNeeds);

            InsertReportRecords(InOperator, InSignedId, TableATMsMainReplNeeds);

        }

        private void InsertReportRecords(string InOperator, string InSignedId, DataTable InTable)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport80(InSignedId);

            Tr.InsertReport80(InOperator, InSignedId, InTable);
        }


        // Methods 
        // READ ATMs Main For ALL ATMS specific authorised user 
        // FILL UP A TABLE 

        public void ReadAtmsMainForAuthUserAndFillTableForBulk(string InOperator, string InSignedId, DateTime InToday, bool InWithDate, int InMode, int InReconcGroupNo)
        {


            RecordFound = false;

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            int WProcess = 0;

            ExpressTotal = 0;
            UnMatchedTotal = 0;
            NoDecisionTotal = 0;
            MakerActionsTotal = 0;

            AuthorisedTotal = 0;
            RejectedTotal = 0;
            AuthNoDecisTotal = 0;

            TableATMsMainSelected = new DataTable();
            TableATMsMainSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableATMsMainSelected.Columns.Add("AtmNo", typeof(string));

            TableATMsMainSelected.Columns.Add("GL At Cut Off", typeof(decimal));
            TableATMsMainSelected.Columns.Add("GL At Repl", typeof(decimal));
            TableATMsMainSelected.Columns.Add("GL & Actions", typeof(decimal));
            TableATMsMainSelected.Columns.Add("Cash Unloaded", typeof(decimal));
            TableATMsMainSelected.Columns.Add("GL Diff", typeof(decimal));

            TableATMsMainSelected.Columns.Add("Maker", typeof(string));
            TableATMsMainSelected.Columns.Add("Authoriser", typeof(string));

            TableATMsMainSelected.Columns.Add("ReplCycleNo", typeof(int));

            if (InMode == 10)
            {
                SqlString = "SELECT *"
                      + " FROM [dbo].[AtmsMain]  "
                       + " WHERE Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup AND GL_ReconcDiff = 1 ";
            }
            if (InMode == 11)
            {
                SqlString = "SELECT * "
                      + " FROM [dbo].[AtmsMain]  "
                      + " WHERE Operator = @Operator AND AuthUser = @AuthUser "
                      + " AND (GL_ReconcDiff = 1 OR Maker = 'UnMatched') AND Maker <> 'UnReplenish' ";
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
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InReconcGroupNo);
                        //cmd.Parameters.AddWithValue("@ReconcCycleNo", InRmCycle);

                        if (InMode == 11)
                        {
                            cmd.Parameters.AddWithValue("@AuthUser", InSignedId);
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
                            if (Authoriser == "Rejected") RejectedTotal = RejectedTotal + 1;

                            if (Authoriser == "No Decision") AuthNoDecisTotal = AuthNoDecisTotal + 1;

                            // NEW ROW 
                            DataRow RowSelected = TableATMsMainSelected.NewRow();

                            WProcess = 4;

                            Na.ReadSessionsNotesAndValues(AtmNo, ReplCycleNo, WProcess);

                            RowSelected["AtmNo"] = (string)rdr["AtmNo"];

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

                            RowSelected["ReplCycleNo"] = ReplCycleNo;

                            // ADD ROW
                            TableATMsMainSelected.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }


        // Methods 
        // READ ATMs Main For ALL ATMS specific authorised user 
        // FILL UP A TABLE 

        public void ReadAtmsMainForAuthUserAndFillTableFor_GL_Cash(string InOperator, string InSignedId, int InMode, int InReconcGroupNo)
        {

            RecordFound = false;

            RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

            int WProcess = 0;

            ExpressTotal = 0;
            UnMatchedTotal = 0;
            NoDecisionTotal = 0;
            MakerActionsTotal = 0;

            AuthorisedTotal = 0;
            RejectedTotal = 0;
            AuthNoDecisTotal = 0;

            TableATMsMainSelected = new DataTable();
            TableATMsMainSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableATMsMainSelected.Columns.Add("AtmNo", typeof(string));

            TableATMsMainSelected.Columns.Add("Last_Repl_Dt", typeof(string));
            TableATMsMainSelected.Columns.Add("GL_ReconcDate", typeof(string));

            TableATMsMainSelected.Columns.Add("GL_At_Cut_Off", typeof(decimal));

            TableATMsMainSelected.Columns.Add("GL_At_Repl", typeof(decimal));
            TableATMsMainSelected.Columns.Add("Cash_Unloaded", typeof(decimal));

            TableATMsMainSelected.Columns.Add("GL_ReplenishmentDt", typeof(string));

            TableATMsMainSelected.Columns.Add("GL_ReplCycle", typeof(int));
            TableATMsMainSelected.Columns.Add("GL_Diff", typeof(decimal));

            TableATMsMainSelected.Columns.Add("GL_OutStandingErrors", typeof(int));

            TableATMsMainSelected.Columns.Add("Maker", typeof(string));
            TableATMsMainSelected.Columns.Add("Authoriser", typeof(string));

            TableATMsMainSelected.Columns.Add("Pending_Errors_Amt", typeof(decimal));

            if (InMode == 10)
            {
                SqlString = "SELECT *"
                      + " FROM [dbo].[AtmsMain]  "
                       //+ " WHERE Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup AND GL_ReconcDiff = 1 ";
                       + " WHERE Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup AND CurrentSesNo > 0 ";
            }
            if (InMode == 11)
            {
                SqlString = "SELECT *"
                      + " FROM [dbo].[AtmsMain]  "
                      + " WHERE Operator = @Operator AND AuthUser = @AuthUser "
                      + " AND (GL_ReconcDiff = 1 OR Maker = 'UnMatched') AND Maker <> 'UnReplenish' AND CurrentSesNo > 0 ";
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
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InReconcGroupNo);

                        if (InMode == 11)
                        {
                            cmd.Parameters.AddWithValue("@AuthUser", InSignedId);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            RdrFieldsForMain(rdr);

                            // TOTALS
                            if (Maker == "UnMatched") UnMatchedTotal = UnMatchedTotal + 1;

                            if (Maker == "No Decision") NoDecisionTotal = NoDecisionTotal + 1;

                            if (Maker == "Maker Actions") MakerActionsTotal = MakerActionsTotal + 1;

                            if (Authoriser == "Authorised") AuthorisedTotal = AuthorisedTotal + 1;
                            if (Authoriser == "Rejected") RejectedTotal = RejectedTotal + 1;

                            if (Authoriser == "No Decision") AuthNoDecisTotal = AuthNoDecisTotal + 1;

                            // NEW ROW 
                            DataRow RowSelected = TableATMsMainSelected.NewRow();

                            RowSelected["AtmNo"] = AtmNo;

                            if (LastReplDt.Date == NullPastDate)
                            {
                                RowSelected["Last_Repl_Dt"] = "Not Available";
                            }
                            else
                            {
                                RowSelected["Last_Repl_Dt"] = LastReplDt.ToString();

                            }

                            if (GL_ReconcDate.Date == NullPastDate)
                            {
                                RowSelected["GL_ReconcDate"] = "Not Available";
                            }
                            else
                            {
                                RowSelected["GL_ReconcDate"] = GL_ReconcDate.ToString();
                            }

                            RowSelected["GL_At_Cut_Off"] = GL_Balance_At_CutOff;

                            RowSelected["GL_At_Repl"] = GL_At_Repl;
                            RowSelected["Cash_Unloaded"] = GL_Counted;

                            if (GL_ReplenishmentDt.Date == NullPastDate)
                            {
                                RowSelected["GL_ReplenishmentDt"] = "Not Available";
                            }
                            else
                            {
                                RowSelected["GL_ReplenishmentDt"] = GL_ReplenishmentDt.ToString();
                            }

                            RowSelected["GL_ReplCycle"] = GL_ReplCycle;

                            RowSelected["GL_Diff"] = GL_DiffCurr1;

                            RowSelected["GL_OutStandingErrors"] = GL_OutStandingErrors;

                            RowSelected["Maker"] = Maker;
                            RowSelected["Authoriser"] = Authoriser;


                            Er.ReadAllErrorsTableForCounters(Operator, "", AtmNo, GL_ReplCycle, "");
                            if (Er.TotalErrorsAmtLess100 > 0)
                            {
                                RowSelected["Pending_Errors_Amt"] = Er.TotalErrorsAmtLess100;
                            }
                            else
                            {
                                RowSelected["Pending_Errors_Amt"] = Er.TotalErrorsAmtLess100;
                            }

                            // ADD ROW
                            TableATMsMainSelected.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }

            InsertWReport74();
        }

        // Insert report 74
        public void InsertWReport74()
        {

            if (TableATMsMainSelected.Rows.Count > 0)
            {
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport74();

                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport74]";

                            foreach (var column in TableATMsMainSelected.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(TableATMsMainSelected);
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }
        }


        // Methods 
        // READ ATMs Main For ALL ATMS specific authorised user ANd Do All Presenter 
        // 

        public void ReadAtmsMainForAuthUserAndDoAllPresenter(string InOperator, string InSignedId, DateTime InToday, bool InWithDate)
        {

            RecordFound = false;

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

            int WProcess = 0;

            TotalSelected = 0;

            ExpressTotal = 0;

            SqlString = "SELECT *"
                  + " FROM [dbo].[AtmsMain]  "
                  + " WHERE Operator = @Operator AND AuthUser = @AuthUser AND GL_ReconcDiff = 1 ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AuthUser", InSignedId);

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

                            Operator = (string)rdr["Operator"];

                            Er.ReadPresentedErrorsForParticularAtmAndSession(Operator, AtmNo, ReplCycleNo);

                            if (Er.AllPresenterMatched == true & Er.PresenterErrorsAmt > 0)
                            {
                                // This is Valid
                            }
                            else
                            {
                                // This is invalid 
                                continue;
                            }

                            bool WUnderAction = true;
                            // Do presenter 
                            Er.UpdatePresenterErrorsWithChangeUnderAction(Operator, AtmNo, ReplCycleNo, WUnderAction);

                            WProcess = 4;

                            Na.ReadSessionsNotesAndValues(AtmNo, ReplCycleNo, WProcess);

                            if (Na.Balances1.CountedBal - (-Na.Balances1.HostBalAdj) == 0)
                            {
                                // This is valid
                                ReadAtmsMainSpecific(AtmNo);

                                Maker = "Maker Actions";

                                UpdateAtmsMain(AtmNo);

                                ExpressTotal = ExpressTotal + 1;

                            }
                            else
                            {
                                // Not Valid ... Reverse Actions  
                                WUnderAction = false;
                                // Do presenter 
                                Er.UpdatePresenterErrorsWithChangeUnderAction(Operator, AtmNo, ReplCycleNo, WUnderAction);

                                continue;
                            }

                        }

                        // Close Reader
                        rdr.Close();
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // Methods 
        // READ ATMs Main For ALL ATMS specific authorised user ANd Do All Presenter 
        // 

        public void ReadAtmsMainForAuthUserAnd_Un_Do_AllPresenter(string InOperator, string InSignedId, DateTime InToday, bool InWithDate)
        {

            RecordFound = false;

            bool WUnderAction = false;

            RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

            int WProcess = 0;

            TotalSelected = 0;

            ExpressTotal = 0;

            SqlString = "SELECT *"
                  + " FROM [dbo].[AtmsMain]  "
                  + " WHERE Operator = @Operator AND AuthUser = @AuthUser AND GL_ReconcDiff = 1 ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AuthUser", InSignedId);

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

                            Operator = (string)rdr["Operator"];

                            Er.ReadPresentedErrorsForParticularAtmAndSession(Operator, AtmNo, ReplCycleNo);

                            //TotalPresenter = 0;
                            //TotalPresentedAmt = 0;

                            //TolalErrorsUnderAction = 0;
                            //TotalPresenterUnderAction = 0;

                            if (Er.TotalPresenterUnderAction > 0)
                            {
                                WUnderAction = false;
                                // Do presenter 
                                int TotalUpdated = Er.UpdatePresenterErrorsWith_Un_Do(Operator, AtmNo, ReplCycleNo, WUnderAction);

                                if (TotalUpdated == Er.TolalErrorsUnderAction)
                                {
                                    ReadAtmsMainSpecific(AtmNo);

                                    Maker = "No Decision";

                                    UpdateAtmsMain(AtmNo);
                                }
                            }

                        }

                        // Close Reader
                        rdr.Close();
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }


        // Methods 
        // READ ATMs Main For ALL ATMS specific authorised user 
        // FILL UP A TABLE 

        public void ReadAtmsMainForAuthUserAndFillTable(string InOperator, string InAuthUser, string InAtmNo)
        {
            RecordFound = false;

            TableATMsMainSelected = new DataTable();
            TableATMsMainSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableATMsMainSelected.Columns.Add("AtmNo", typeof(string));
            TableATMsMainSelected.Columns.Add("ReplCycle", typeof(string));
            TableATMsMainSelected.Columns.Add("AtmName", typeof(string));
            TableATMsMainSelected.Columns.Add("RespBranch", typeof(string));
            TableATMsMainSelected.Columns.Add("AuthUser", typeof(string));

            if (InAtmNo == "")
            {
                SqlString = "SELECT *"
                     + " FROM [dbo].[AtmsMain] "
                     + " WHERE  Operator=@Operator AND AuthUser=@AuthUser ";
            }

            if (InAtmNo != "")
            {
                SqlString = "SELECT *"
                     + " FROM [dbo].[AtmsMain] "
                     + " WHERE Operator=@Operator AND AuthUser=@AuthUser AND AtmNo=@AtmNo ";
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
                            DataRow RowSelected = TableATMsMainSelected.NewRow();

                            RowSelected["AtmNo"] = (string)rdr["AtmNo"];

                            RowSelected["ReplCycle"] = (int)rdr["CurrentSesNo"];
                            RowSelected["AtmName"] = (string)rdr["AtmName"];
                            RowSelected["RespBranch"] = (string)rdr["RespBranch"];
                            RowSelected["AuthUser"] = (string)rdr["AuthUser"];

                            // ADD ROW
                            TableATMsMainSelected.Rows.Add(RowSelected);

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
        // READ ATMs Main For ATM 
        // 
        public void ReadAtmsMainSpecific(string InAtmNo)
        {
            RecordFound = false;

            SqlString = "SELECT *"
              + " FROM [dbo].[AtmsMain] "
              + " WHERE AtmNo=@AtmNo ";

            SqlConnection conn = new SqlConnection(connectionString);

            //using (SqlConnection conn =
            //              new SqlConnection(connectionString))
            using (conn)
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

                            RdrFieldsForMain(rdr);

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
        // ReadAtmsMainByReconcGroupForTotal
        // 

        public int ReadAtmsMainByReconcGroupForTotal(string InSelectionCriteria)
        {
            RecordFound = false;

            int RequestedTotal = 0;

            SqlString = "SELECT *"
              + " FROM [dbo].[AtmsMain] "
              + InSelectionCriteria;

            SqlConnection conn = new SqlConnection(connectionString);

            //using (SqlConnection conn =
            //              new SqlConnection(connectionString))
            using (conn)
                try
                {
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            RdrFieldsForMain(rdr);

                            RequestedTotal = RequestedTotal + 1;
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

            return RequestedTotal;
        }


        // Methods 
        // READ ATMs Main For ATM 
        // 
        public void ReadAtmsMainSpecificWithConn(string InAtmNo, SqlConnection InConn)
        {
            RecordFound = false;

            SqlString = "SELECT *"
              + " FROM [dbo].[AtmsMain] "
              + " WHERE AtmNo=@AtmNo ";

            //SqlConnection Inconn = new SqlConnection(connectionString);

            //using (SqlConnection conn =
            //              new SqlConnection(connectionString))
            using (InConn)
                try
                {
                    //InConn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, InConn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;


                            RdrFieldsForMain(rdr);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    //InConn.Close();
                }
                catch (Exception ex)
                {
                    InConn.Close();

                    CatchDetails(ex);

                }
        }
        // 
        // UPDATE ATMs Main 
        //
        public void UpdateAtmsMain(string InAtmNo)
        {

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE AtmsMain SET "
                            + "[AtmNo] =@AtmNo,[CurrentSesNo] = @CurrentSesNo,[AtmName]=@AtmName,"
                            + "[BankId] = @BankId,[RespBranch] = @RespBranch,[BranchName] = @BranchName,"
                            + "[LastReplDt] = @LastReplDt, "
                            + "[NextReplDt] = @NextReplDt, "
                            + "[MoreMaxCash] = @MoreMaxCash,[LessMinCash] = @LessMinCash,[NeedType] = @NeedType,"
                            + "[CurrCassettes] = @CurrCassettes,[CurrentDeposits] = @CurrentDeposits,[EstReplDt] = @EstReplDt,"
                            + "[CitId] = @CitId,"
                            + "[LastUpdated] = @LastUpdated,[AuthUser] = @AuthUser,[ActionNo] = @ActionNo,[ActionConfirmed] = @ActionConfirmed,[LastDispensedHistor] = @LastDispensedHistor,[LastInNeedReview] = @LastInNeedReview, "
                            + "[SessionsInDiff] = @SessionsInDiff,[ErrOutstanding] = @ErrOutstanding,[ReplCycleNo] = @ReplCycleNo, "
                            + "[ReconcCycleNo] = @ReconcCycleNo,[ReconcDt] = @ReconcDt, "
                            + "[ProcessMode] = @ProcessMode, [UnderReconcMode] = @UnderReconcMode,"
                            + "[ExpressAction] = @ExpressAction,"
                            + "[GL_ReconcDiff] = @GL_ReconcDiff, "
                            + "[GL_ReconcDate] = @GL_ReconcDate, [GL_Balance_At_CutOff] = @GL_Balance_At_CutOff,"
                            + "[GL_At_Repl] = @GL_At_Repl,[GL_Counted] = @GL_Counted,"
                            + "[GL_ReplenishmentDt] = @GL_ReplenishmentDt,[GL_ReplCycle] = @GL_ReplCycle,"
                            + "[GL_CurrNm1] = @GL_CurrNm1,[GL_DiffCurr1] = @GL_DiffCurr1,"
                            + "[GL_OutStandingErrors] = @GL_OutStandingErrors,"
                            + "[Maker] = @Maker,[Authoriser] = @Authoriser,"
                            + "[AtmsReplGroup] = @AtmsReplGroup,  "
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

                        cmd.Parameters.AddWithValue("@ActionConfirmed", ActionConfirmed);

                        cmd.Parameters.AddWithValue("@LastDispensedHistor", LastDispensedHistor);

                        cmd.Parameters.AddWithValue("@LastInNeedReview", LastInNeedReview);

                        cmd.Parameters.AddWithValue("@SessionsInDiff", SessionsInDiff);
                        cmd.Parameters.AddWithValue("@ErrOutstanding", ErrOutstanding);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);
                        cmd.Parameters.AddWithValue("@ReconcCycleNo", ReconcCycleNo);
                        cmd.Parameters.AddWithValue("@ReconcDt", ReconcDt);

                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);

                        cmd.Parameters.AddWithValue("@UnderReconcMode", UnderReconcMode);

                        cmd.Parameters.AddWithValue("@AtmsReplGroup", AtmsReplGroup);

                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@ExpressAction", ExpressAction);

                        cmd.Parameters.AddWithValue("@GL_ReconcDiff", GL_ReconcDiff);

                        cmd.Parameters.AddWithValue("@GL_ReconcDate", GL_ReconcDate);
                        cmd.Parameters.AddWithValue("@GL_Balance_At_CutOff", GL_Balance_At_CutOff);

                        cmd.Parameters.AddWithValue("@GL_At_Repl", GL_At_Repl);
                        cmd.Parameters.AddWithValue("@GL_Counted", GL_Counted);

                        cmd.Parameters.AddWithValue("@GL_ReplenishmentDt", GL_ReplenishmentDt);
                        cmd.Parameters.AddWithValue("@GL_ReplCycle", GL_ReplCycle);

                        cmd.Parameters.AddWithValue("@GL_CurrNm1", GL_CurrNm1);
                        cmd.Parameters.AddWithValue("@GL_DiffCurr1", GL_DiffCurr1);

                        cmd.Parameters.AddWithValue("@GL_OutStandingErrors", GL_OutStandingErrors);

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
                    RRDMAtmsClass Ac = new RRDMAtmsClass();
                    Ac.ReadAtm(InAtmNo);
                   
                    if (Ac.RecordFound == true)
                    {
                        // OK
                        CatchDetails(ex);
                    }
                    else
                    {
                        // Skip 
                    }
     
                }

        }
        //
        // INSERT New Record in Main Table 
        //
        public void InsertInAtmsMain(string InAtmNo)
        {

            string cmdinsert = "INSERT INTO [AtmsMain] ([AtmNo],[CurrentSesNo],[AtmName],"
                + "[BankId],[RespBranch],[BranchName],"
                 + "[GL_ReconcDiff],[MoreMaxCash], [LessMinCash],[NeedType],"
                + "[GL_CurrNm1], "
                  + "[CurrCassettes],[CurrentDeposits],[EstReplDt],"
                + "[CitId],[LastUpdated],[AuthUser],[ActionNo], [AtmsReplGroup],[AtmsReconcGroup], [Operator])"
                + " VALUES (@AtmNo, @CurrentSesNo,@AtmName,"
                + "@BankId,@RespBranch,@BranchName,"
                 + "@GL_ReconcDiff,@MoreMaxCash, @LessMinCash,@NeedType, "
                 + "@GL_CurrNm1,"
                  + "@CurrCassettes,@CurrentDeposits,@EstReplDt,"
                + "@CitId,@LastUpdated,@AuthUser,@ActionNo, @AtmsReplGroup, @AtmsReconcGroup, @Operator)";

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

                        cmd.Parameters.AddWithValue("@NextReplDt", NextReplDt);

                        cmd.Parameters.AddWithValue("@CurrCassettes", CurrCassettes);
                        cmd.Parameters.AddWithValue("@CurrentDeposits", CurrentDeposits);
                        cmd.Parameters.AddWithValue("@EstReplDt", EstReplDt);

                        cmd.Parameters.AddWithValue("@GL_ReconcDiff", 0);
                        cmd.Parameters.AddWithValue("@MoreMaxCash", 0);
                        cmd.Parameters.AddWithValue("@LessMinCash", 0);
                        cmd.Parameters.AddWithValue("@NeedType", 0);

                        cmd.Parameters.AddWithValue("@GL_CurrNm1", GL_CurrNm1);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@LastUpdated", LastUpdated);

                        cmd.Parameters.AddWithValue("@AuthUser", "");

                        cmd.Parameters.AddWithValue("@ActionNo", 0);

                        cmd.Parameters.AddWithValue("@AtmsReplGroup", AtmsReplGroup);

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

                    CatchDetails(ex);
                }
        }



    }
}

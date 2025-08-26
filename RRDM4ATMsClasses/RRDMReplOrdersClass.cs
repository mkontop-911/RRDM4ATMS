using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMReplOrdersClass : Logger
    {
        public RRDMReplOrdersClass() : base() { }
        // Variables
        // Repl ACtion FIELDS 
        public int ReplOrderNo;
        public int ReplOrderId;
        public int OrdersCycleNo;
        public DateTime AuthorisedDate; // Effective date 
        public string AtmNo;
        public string AtmName;
        public int ReplCycleNo;
        public string BankId;

        public string RespBranch;
        public string BranchName;

        public bool OffSite;
        public DateTime LastReplDt;
        public string TypeOfRepl;

        public int OverEst;    // % To fill more 

        // NULL Values 
        public DateTime NextReplDt;
        public string CurrNm;
        public decimal MinCash;
        public decimal MaxCash;
        public int ReplAlertDays;

        public bool ReconcDiff;
        public bool MoreMaxCash;
        public bool LessMinCash;
        public int NeedType;

        public decimal CurrCassettes;
        public decimal CurrentDeposits;
        public DateTime EstReplDt;

        public DateTime NewEstReplDt;

        public decimal InsuredAmount;
        public decimal SystemAmount;
        public decimal NewAmount;

        public int FaceValue_1;
        public int Cassette_1;
        public int FaceValue_2;
        public int Cassette_2;
        public int FaceValue_3;
        public int Cassette_3;
        public int FaceValue_4;
        public int Cassette_4;

        public int AtmsStatsGroup;
        public int AtmsReplGroup;
        public int AtmsReconcGroup;
        public DateTime DateInsert;

        public string AuthUser;

        public string OwnerUser;

        public string CitId;

        public bool AuthorisedRecord;

        public bool ActiveRecord; // When created and throught the process is true
                                  // If Bank replenishment then this because false at the end of replenishment by Branch user
                                  // If CIT then this become false on Journal loading and after Supervisor Mode.
        public bool PassReplCycle;
        public DateTime PassReplCycleDate;
        public decimal CashInAmount; // Cash that says in Repl Cycle 
        public decimal InMoneyReal; // Cash in ATM registers 

        public string InactivateComment;

        public string Operator;

        public string PublicCitString;

        string connectionString = ConfigurationManager.ConnectionStrings
        ["ATMSConnectionString"].ConnectionString;

        string WhatFile = "[ATMS].[dbo].[ReplOrdersTable]";

        decimal TotNewAmount;

        // Define the data table 
        public DataTable TableReplOrders = new DataTable();
        DataTable TempOrdersTable = new DataTable();
        public int TotalSelected;

        string SqlString;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // 
        // Classes 
        // 
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMAccountsClass Acc = new RRDMAccountsClass();
        RRDMPostedTrans Cs = new RRDMPostedTrans();
        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        //     ReplDatesCalc Rc = new ReplDatesCalc(); // Locate next Replenishment 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // READER FIELDS
        private void ReplActionsReaderFields(SqlDataReader rdr)
        {
            ReplOrderNo = (int)rdr["ReplOrderNo"];

            ReplOrderId = (int)rdr["ReplOrderId"];

            OrdersCycleNo = (int)rdr["OrdersCycleNo"];

            AuthorisedDate = (DateTime)rdr["AuthorisedDate"];
            AtmNo = (string)rdr["AtmNo"];

            AtmName = (string)rdr["AtmName"];

            ReplCycleNo = (int)rdr["ReplCycleNo"];

            BankId = (string)rdr["BankId"];
            RespBranch = (string)rdr["RespBranch"];
            BranchName = (string)rdr["BranchName"];

            OffSite = (bool)rdr["OffSite"];
            LastReplDt = (DateTime)rdr["LastReplDt"];

            TypeOfRepl = (string)rdr["TypeOfRepl"];

            OverEst = (int)rdr["OverEst"];

            NextReplDt = (DateTime)rdr["NextReplDt"];

            CurrNm = (string)rdr["CurrNm"];

            MinCash = (decimal)rdr["MinCash"];

            MaxCash = (decimal)rdr["MaxCash"];

            ReplAlertDays = (int)rdr["ReplAlertDays"];

            ReconcDiff = (bool)rdr["ReconcDiff"];
            MoreMaxCash = (bool)rdr["MoreMaxCash"];
            LessMinCash = (bool)rdr["LessMinCash"];
            NeedType = (int)rdr["NeedType"];

            CurrCassettes = (decimal)rdr["CurrCassettes"];
            CurrentDeposits = (decimal)rdr["CurrentDeposits"];
            EstReplDt = (DateTime)rdr["EstReplDt"];

            NewEstReplDt = (DateTime)rdr["NewEstReplDt"];

            InsuredAmount = (decimal)rdr["InsuredAmount"];
            SystemAmount = (decimal)rdr["SystemAmount"];
            NewAmount = (decimal)rdr["NewAmount"];

            FaceValue_1 = (int)rdr["FaceValue_1"];
            Cassette_1 = (int)rdr["Cassette_1"];
            FaceValue_2 = (int)rdr["FaceValue_2"];
            Cassette_2 = (int)rdr["Cassette_2"];
            FaceValue_3 = (int)rdr["FaceValue_3"];
            Cassette_3 = (int)rdr["Cassette_3"];
            FaceValue_4 = (int)rdr["FaceValue_4"];
            Cassette_4 = (int)rdr["Cassette_4"];

            AtmsStatsGroup = (int)rdr["AtmsStatsGroup"];
            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];

            DateInsert = (DateTime)rdr["DateInsert"];

            AuthUser = (string)rdr["AuthUser"];

            OwnerUser = (string)rdr["OwnerUser"];

            CitId = (string)rdr["CitId"];

            AuthorisedRecord = (bool)rdr["AuthorisedRecord"];

            ActiveRecord = (bool)rdr["ActiveRecord"];

            PassReplCycle = (bool)rdr["PassReplCycle"];
            PassReplCycleDate = (DateTime)rdr["PassReplCycleDate"];
            CashInAmount = (decimal)rdr["CashInAmount"];
            InMoneyReal = (decimal)rdr["InMoneyReal"];

            InactivateComment = (string)rdr["InactivateComment"];

            Operator = (string)rdr["Operator"];
        }

        //    string outcome = ""; // TO FACILITATE EXCEPTIONS 
        // Methods 
        // READ ReplOrdersTable
        // 
        public int ExcelRecords;
        public decimal ExcelAmount;
        public void ReadReplActionsAndFillTable(string InOperator, string InFilter,
                                                   DateTime InFromDt, DateTime InToDt, int InMode)
        {
            RRDMReplDatesCalc Rc = new RRDMReplDatesCalc();
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            // InMode = 1 = No Dates or specific Orders Cycle
            // In Mode = 2 = Dates Range 
            // In Mode = 3 = Alerts EstReplDate < today

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ExcelRecords = 0;
            ExcelAmount = 0;

            TableReplOrders = new DataTable();
            TableReplOrders.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableReplOrders.Columns.Add("OrderNo", typeof(int));
            TableReplOrders.Columns.Add("AtmNo", typeof(string));
            TableReplOrders.Columns.Add("AtmName", typeof(string));

            TableReplOrders.Columns.Add("ReplCycleNo", typeof(int));
            TableReplOrders.Columns.Add("NeedStatus", typeof(string));
            TableReplOrders.Columns.Add("OrdersCycleNo", typeof(int));

            TableReplOrders.Columns.Add("CurrentCassettes", typeof(decimal));
            TableReplOrders.Columns.Add("DaysToLast", typeof(int));
            TableReplOrders.Columns.Add("LastReplDt", typeof(DateTime));

            TableReplOrders.Columns.Add("NextReplDt", typeof(DateTime));
            TableReplOrders.Columns.Add("ToLoadAmount", typeof(decimal));

            switch (InMode)
            {
                case 1:
                    {
                        SqlString = "SELECT *"
                       + " FROM " + WhatFile
                + " WHERE " + InFilter
                + " ORDER BY ReplOrderNo ASC ";
                        break;
                    }
                case 2:
                    {
                        SqlString = "SELECT *"
                  + " FROM " + WhatFile
                  + " WHERE " + InFilter + " AND (DateInsert >= @FromDt AND DateInsert <= @ToDt) "
                  + " ORDER BY ReplOrderNo ASC ";
                        break;
                    }
                case 3:
                    {
                        SqlString = "SELECT *"
                  + " FROM " + WhatFile
                  + " Where AuthorisedRecord = 1 AND ActiveRecord = 1 AND PassReplCycle = 0 AND NewEstReplDt < @InDt "
                        + " ORDER BY ReplOrderNo ASC ";
                        break;
                    }
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
                        if (InMode == 2)
                        {
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }
                        if (InMode == 3)
                        {
                            cmd.Parameters.AddWithValue("@InDt", InFromDt);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);

                            ExcelRecords = ExcelRecords + 1;
                            ExcelAmount = ExcelAmount + NewAmount;

                            // Fill Table 

                            DataRow RowSelected = TableReplOrders.NewRow();

                            RowSelected["OrderNo"] = ReplOrderNo;
                            RowSelected["AtmNo"] = AtmNo;
                            Ac.ReadAtm(AtmNo);
                            RowSelected["AtmName"] = Ac.AtmName;

                            RowSelected["ReplCycleNo"] = ReplCycleNo;
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
                            RowSelected["OrdersCycleNo"] = OrdersCycleNo;

                            RowSelected["CurrentCassettes"] = CurrCassettes;

                            Rc.FindNextRepl(Operator, AtmNo, ReplCycleNo, CurrCassettes, InFromDt);
                            RowSelected["DaysToLast"] = (Rc.NoOfDays ).ToString();

                            RowSelected["LastReplDt"] = LastReplDt;

                            RowSelected["NextReplDt"] = NextReplDt;
                            RowSelected["ToLoadAmount"] = NewAmount;


                            // ADD ROW
                            TableReplOrders.Rows.Add(RowSelected);
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
        // READ ReplOrdersTable By Selection Criteria
        // 
        public void ReadReplOrdersBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ReplOrdersTable] "
          +  InSelectionCriteria ;
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
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

                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);

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
        // READ ReplOrdersTable
        // 
        public void ReadReplActionsForAtm(string InAtmNo, int InReplOrderNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ReplOrdersTable] "
          + " WHERE AtmNo=@AtmNo AND ReplOrderNo=@ReplOrderNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplOrderNo", InReplOrderNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;


                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);

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
        // READ ReplOrdersTable 
        // 
        public void Read_Latest_ReplActionsForAtm(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ReplOrdersTable] "
          + " WHERE AtmNo=@AtmNo "
          + " Order by ReplOrderNo DESC ";
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

                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);

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
        // READ ReplOrdersTable
        // 
        public void ReadReplActionsForAtmReplCycleNo(string InAtmNo, int InReplCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ReplOrdersTable] "
          + " WHERE AtmNo=@AtmNo AND ReplCycleNo = @ReplCycleNo";
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

                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);

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
        // READ ReplOrdersTable
        // 
        public void ReadReplActionsForAtmReplCycleAuther(string InAtmNo, int InInOrdersCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ReplOrdersTable] "
          + " WHERE AtmNo=@AtmNo AND OrdersCycle = @OrdersCycle AND AuthorisedRecord = 1";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@InOrdersCycle", InInOrdersCycle);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);

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
        // READ ReplOrdersTable
        // 
        public void ReadReplActionsSpecific(int InReplOrderNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                + " FROM [dbo].[ReplOrdersTable] "
                + " WHERE ReplOrderNo=@ReplOrderNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@ReplOrderNo", InReplOrderNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);

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
        // READ TO FIND Counters  
        // 
        public int NumberOfOrders; 
        public int NumberOfActiveOrders;
        public int CanceledOrders;
        public decimal TotalInsuredAmount;
        public decimal TotalSystemAmount;
        public decimal TotalNewAmount;
        public void ReadReplActionsForCounters(string InCitId, int InOrdersCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfOrders = 0; 
            NumberOfActiveOrders = 0;
            CanceledOrders = 0;
            TotalInsuredAmount = 0;
            TotalSystemAmount = 0;
            TotalNewAmount = 0;

            SqlString = "SELECT *"
                + " FROM [dbo].[ReplOrdersTable] "
                + " WHERE CitId=@CitId AND OrdersCycleNo=@OrdersCycleNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CitId", InCitId);
                        cmd.Parameters.AddWithValue("@OrdersCycleNo", InOrdersCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);

                            NumberOfOrders = NumberOfOrders + 1; 

                            InsuredAmount = (decimal)rdr["InsuredAmount"];
                            SystemAmount = (decimal)rdr["SystemAmount"];

                            if (ActiveRecord == true)
                            {
                                NumberOfActiveOrders = NumberOfActiveOrders + 1;
                                TotalInsuredAmount = TotalInsuredAmount + InsuredAmount;
                                TotalSystemAmount = TotalSystemAmount + SystemAmount;
                                TotalNewAmount = TotalNewAmount + NewAmount;
                            }
                            else
                            {
                                CanceledOrders = CanceledOrders + 1;
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
        // READ all ATMs for Action for this CIT Provider
        // Create a String throught a the StringBuilder
        // Insert transaction and Update 
        //
        public void ReadReplActionsForCITAndUpdate(string InCitId, int InOrdersCycleNo, string InBankId, string InUser)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TempOrdersTable = new DataTable();
            TempOrdersTable.Clear();

            TotalSelected = 0;
            //***************************************************
            // SELECT All REPL ACTIONS FOR THIS CIT AND FOR EACH RECORD  MAKE TRANS TO BE POSTED
            //***************************************************
            SqlString = "SELECT *"
               + " FROM [dbo].[ReplOrdersTable] "
               + " WHERE CitId=@CitId AND OrdersCycleNo = @OrdersCycleNo AND ActiveRecord = 1";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CitId", InCitId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@OrdersCycleNo", InOrdersCycleNo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TempOrdersTable);

                        // Close conn
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            int I = 0;
            TotNewAmount = 0;

            while (I <= (TempOrdersTable.Rows.Count - 1))
            {

                RecordFound = true;

                ReplOrderNo = (int)TempOrdersTable.Rows[I]["ReplOrderNo"];

                ReadReplActionsSpecific(ReplOrderNo);

                TotNewAmount = TotNewAmount + NewAmount;

                Ac.ReadAtm(AtmNo); // Read information of face values 

                if (CitId != "1000") // NOT EQUAL _Create TRANSACTIONS FOR CIT
                {

                    // Create transactions to be posted related with CIT account 
                    // CR Bank Cash Account 
                    // And DR CIT CASH Account 


                    Tp.OriginId = "08"; // *
                    Tp.OriginName = "To CIT-OurATMs";  // Money sent to CIT
                    Tp.RMCateg = "N/A";
                    Tp.RMCategCycle = 0;
                    Tp.UniqueRecordId = 0;

                    Tp.ErrNo = 0;
                    Tp.AtmNo = AtmNo;
                    Tp.SesNo = ReplCycleNo;
                    Tp.BankId = BankId;

                    Tp.AtmTraceNo = 0;

                    Tp.BranchId = "Controller's Branch";
                    Tp.AtmDtTime = DateTime.Now;
                    //Tp.HostDtTime = DateTime.Now; 
                    Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                    Tp.CardNo = "N/A";
                    Tp.CardOrigin = 5; // Find OUT

                    // First Entry 
                    Tp.TransType = 21; // CR BANK CASH 
                    Tp.TransDesc = "BANK TO CIT : " + CitId + " For ATM: " + AtmNo;

                    string WUser = "1000";
                    Acc.ReadAndFindAccount("1000", "", "", Operator, "", Ac.DepCurNm, "User or CIT Cash");
                    if (Acc.RecordFound == false)
                    {
                        ErrorFound = false;
                        ErrorOutput = "Account not found for BAnk till  : " + WUser;
                        return;
                    }

                    Tp.AccNo = Acc.AccNo;  // Bank Cash account 
                    Tp.GlEntry = true;

                    // Second Entry 
                    // CREATE A TRANSACTION TO CR SUSPENSE 

                    Tp.TransType2 = 11; // MAKE REVERSE 
                    Tp.TransDesc2 = "CIT Got Money from: " + InCitId + " For ATM: " + AtmNo;

                    WUser = CitId;
                    Acc.ReadAndFindAccount(WUser, "", "", Operator, "", Ac.DepCurNm, "User or CIT Cash");
                    if (Acc.RecordFound == false)
                    {
                        ErrorFound = false;
                        ErrorOutput = "Account not found for User : " + WUser;
                        return;
                    }

                    Tp.AccNo2 = Acc.AccNo;  // CIT Cash account 
                    Tp.GlEntry2 = true;

                    Cs.Operator = Ac.Operator;

                    Tp.CurrDesc = CurrNm;
                    Tp.TranAmount = NewAmount;
                    Tp.AuthCode = 0;
                    Tp.RefNumb = 0;
                    Tp.RemNo = 0;
                    Tp.TransMsg = "Not available";
                    Tp.AtmMsg = "";
                    Tp.OpenDate = DateTime.Now;
                    Tp.OpenRecord = true;
                    Tp.MakerUser = AuthUser;
                    Tp.AuthUser = AuthUser;

                    Tp.Operator = Ac.Operator;

                    //****************************
                    // INSERT ********************
                    //****************************
                    //NOSTRO
                    Tp.NostroCcy = "";
                    Tp.NostroAdjAmt = 0;

                    int PostedNo = Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo, Tp.OriginName);

                }

                // UPDATE REPL ACTIONS TABLE 
                //AuthorisedDate = DateTime.Now;

                // Update ATMs Main 
                Am.ReadAtmsMainSpecific(AtmNo);

                Am.ActionConfirmed = true;

                Am.UpdateAtmsMain(AtmNo);


                I++; // Read Next entry of the table 
            }


        }

        //
        // READ TO FIND to update Authorised User  
        // 
        public void ReadReplActionsForUpdatingUser(string InUser, string InAtmNo, int Action)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            // If Action = 1 then update if 2 initialise with zero 
            SqlString = "SELECT *"
                 + " FROM [dbo].[ReplOrdersTable] "
                 + " WHERE AtmNo=@AtmNo AND ActiveRecord=1";
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
                            ReplOrderNo = (int)rdr["ReplOrderNo"];

                            ReplOrderId = (int)rdr["ReplOrderId"];

                            ReadReplActionsForAtm(InAtmNo, ReplOrderNo);

                            if (Action == 1) AuthUser = InUser;
                            if (Action == 2) AuthUser = "";

                            UpdateReplActionsForAtm(InAtmNo, ReplOrderNo);

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
        // READ TO FIND IF ACTION ALREADY TAKEN TODAY  or any other specific date 
        // 
        public void ReadReplActionsForSpecificDate(string InAtmNo, DateTime InDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                  + " FROM [dbo].[ReplOrdersTable] "
                  + " WHERE AtmNo=@AtmNo AND Cast(DateInsert AS DATE) = @InDate";

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

                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);


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
        // READ TO FIND 
        // 
        public void ReadReplActionsForSpecificReplCycle(string InAtmNo, int InOrdersCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                  + " FROM [dbo].[ReplOrdersTable] "
                  + " WHERE AtmNo=@AtmNo AND OrdersCycle = @OrdersCycle AND ActiveRecord = 1";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@OrdersCycle", InOrdersCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Reader Fields
                            ReplActionsReaderFields(rdr);

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
        // UPDATE ReplOrdersTable
        //
        public void UpdateReplActionsForAtm(string InAtmNo, int InReplActionNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ReplOrdersTable SET "
                            + "[ReplOrderId]=@ReplOrderId,"
                            //+ "[OrdersCycleNo]=@OrdersCycleNo,"
                            + "[AuthorisedDate]=@AuthorisedDate,"
                            + "[AtmNo] =@AtmNo,[AtmName]=@AtmName,[ReplCycleNo]=@ReplCycleNo,"
                            + "[BankId] = @BankId,[RespBranch] = @RespBranch,[BranchName] = @BranchName,"
                            + " [OffSite] = @OffSite,[LastReplDt] = @LastReplDt, "
                            + " [TypeOfRepl] = @TypeOfRepl,[OverEst] = @OverEst,[NextReplDt] = @NextReplDt, "
                            + " [CurrNm] = @CurrNm,"
                            + " [MinCash] = @MinCash,[MaxCash] = @MaxCash,"
                            + " [ReplAlertDays] = @ReplAlertDays,"
                             + "[ReconcDiff] = @ReconcDiff,[MoreMaxCash] = @MoreMaxCash,[LessMinCash] = @LessMinCash,[NeedType] = @NeedType,"
                              + "[CurrCassettes] = @CurrCassettes,[CurrentDeposits] = @CurrentDeposits,[EstReplDt] = @EstReplDt,"
                              + "[NewEstReplDt] = @NewEstReplDt,"
                              + "[InsuredAmount] = @InsuredAmount,[SystemAmount] = @SystemAmount,[NewAmount] = @NewAmount,"
                              + "[FaceValue_1] = @FaceValue_1,"
                              + "[Cassette_1] = @Cassette_1,"
                              + "[FaceValue_2] = @FaceValue_2,"
                              + "[Cassette_2] = @Cassette_2,"
                              + "[FaceValue_3] = @FaceValue_3,"
                              + "[Cassette_3] = @Cassette_3,"
                              + "[FaceValue_4] = @FaceValue_4,"
                              + "[Cassette_4] = @Cassette_4,"
                            + "[AtmsStatsGroup] = @AtmsStatsGroup,[AtmsReplGroup] = @AtmsReplGroup,[AtmsReconcGroup] = @AtmsReconcGroup,"
                            + "[DateInsert] = @DateInsert,[AuthUser] = @AuthUser,[OwnerUser] = @OwnerUser,[CitId] = @CitId,"
                            + " [AuthorisedRecord] = @AuthorisedRecord,[ActiveRecord] = @ActiveRecord,"
                            + " [PassReplCycle] = @PassReplCycle, [PassReplCycleDate] = @PassReplCycleDate, [CashInAmount] = @CashInAmount, [InMoneyReal] = @InMoneyReal, [InactivateComment] = @InactivateComment "
                            + " WHERE AtmNo= @AtmNo AND ReplOrderNo = @ReplOrderNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplOrderNo", InReplActionNo);

                        cmd.Parameters.AddWithValue("@ReplOrderId", ReplOrderId);

                        //cmd.Parameters.AddWithValue("@OrdersCycleNo", OrdersCycleNo);

                        cmd.Parameters.AddWithValue("@AuthorisedDate", AuthorisedDate);

                        cmd.Parameters.AddWithValue("@AtmName", AtmName);

                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@RespBranch", RespBranch);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);

                        cmd.Parameters.AddWithValue("@OffSite", OffSite);
                        cmd.Parameters.AddWithValue("@LastReplDt", LastReplDt);

                        cmd.Parameters.AddWithValue("@TypeOfRepl", TypeOfRepl);

                        cmd.Parameters.AddWithValue("@OverEst", OverEst);

                        cmd.Parameters.AddWithValue("@NextReplDt", NextReplDt);

                        cmd.Parameters.AddWithValue("@CurrNm", CurrNm);

                        cmd.Parameters.AddWithValue("@MinCash", MinCash);
                        cmd.Parameters.AddWithValue("@MaxCash", MaxCash);
                        cmd.Parameters.AddWithValue("@ReplAlertDays", ReplAlertDays);

                        cmd.Parameters.AddWithValue("@ReconcDiff", ReconcDiff);
                        cmd.Parameters.AddWithValue("@MoreMaxCash", MoreMaxCash);
                        cmd.Parameters.AddWithValue("@LessMinCash", LessMinCash);
                        cmd.Parameters.AddWithValue("@NeedType", NeedType);

                        cmd.Parameters.AddWithValue("@CurrCassettes", CurrCassettes);
                        cmd.Parameters.AddWithValue("@CurrentDeposits", CurrentDeposits);
                        cmd.Parameters.AddWithValue("@EstReplDt", EstReplDt.Date);

                        cmd.Parameters.AddWithValue("@NewEstReplDt", NewEstReplDt);

                        cmd.Parameters.AddWithValue("@InsuredAmount", InsuredAmount);
                        cmd.Parameters.AddWithValue("@SystemAmount", SystemAmount);
                        cmd.Parameters.AddWithValue("@NewAmount", NewAmount);

                        cmd.Parameters.AddWithValue("@FaceValue_1", FaceValue_1);
                        cmd.Parameters.AddWithValue("@Cassette_1", Cassette_1);
                        cmd.Parameters.AddWithValue("@FaceValue_2", FaceValue_2);
                        cmd.Parameters.AddWithValue("@Cassette_2", Cassette_2);
                        cmd.Parameters.AddWithValue("@FaceValue_3", FaceValue_3);
                        cmd.Parameters.AddWithValue("@Cassette_3", Cassette_3);
                        cmd.Parameters.AddWithValue("@FaceValue_4", FaceValue_4);
                        cmd.Parameters.AddWithValue("@Cassette_4", Cassette_4);

                        cmd.Parameters.AddWithValue("@AtmsStatsGroup", AtmsStatsGroup);
                        cmd.Parameters.AddWithValue("@AtmsReplGroup", AtmsReplGroup);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@DateInsert", DateInsert);

                        cmd.Parameters.AddWithValue("@AuthUser", AuthUser);

                        cmd.Parameters.AddWithValue("@OwnerUser", OwnerUser);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@AuthorisedRecord", AuthorisedRecord);

                        cmd.Parameters.AddWithValue("@ActiveRecord", ActiveRecord);

                        cmd.Parameters.AddWithValue("@PassReplCycle", PassReplCycle);
                        cmd.Parameters.AddWithValue("@PassReplCycleDate", PassReplCycleDate);
                        cmd.Parameters.AddWithValue("@CashInAmount", CashInAmount);
                        cmd.Parameters.AddWithValue("@InMoneyReal", InMoneyReal);

                        cmd.Parameters.AddWithValue("@InactivateComment", InactivateComment);


                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //    outcome = " ATMs Table UPDATED ";
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

        // 
        // UPDATE ReplOrdersTable as Authorised
        //
        public void UpdateReplActionsForAuthorised(string InAuthUser, string InCitId, int InOrdersCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ReplOrdersTable SET "
                            + "[AuthorisedDate]=@AuthorisedDate,"
                            + "[AuthorisedRecord]=@AuthorisedRecord,"
                            + " [AuthUser] = @AuthUser "
                            + " WHERE CitId= @CitId AND OrdersCycleNo = @OrdersCycleNo ", conn))
                    {


                        cmd.Parameters.AddWithValue("@CitId", InCitId);
                        cmd.Parameters.AddWithValue("@OrdersCycleNo", InOrdersCycleNo);

                        cmd.Parameters.AddWithValue("@AuthorisedDate", DateTime.Now);

                        cmd.Parameters.AddWithValue("@AuthorisedRecord", true);

                        cmd.Parameters.AddWithValue("@AuthUser", InAuthUser);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //    outcome = " ATMs Table UPDATED ";
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



        //
        // INSERT New Record in ReplOrdersTable
        //
        public int InsertReplOrdersTable(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ReplOrdersTable]"
                + " ([ReplOrderId],"
                 + "[OrdersCycleNo],"
                + "[AtmNo],[AtmName],[ReplCycleNo],"
                + "[BankId],[RespBranch],[BranchName],"
                + "[OffSite],[LastReplDt],[TypeOfRepl],[OverEst],[NextReplDt],"
                 + "[CurrNm],"
                + "[MinCash],[MaxCash],[ReplAlertDays],"
                 + "[ReconcDiff],[MoreMaxCash],[LessMinCash],[NeedType],"
                  + "[CurrCassettes],[CurrentDeposits],[EstReplDt],"
                  + "[NewEstReplDt],"
                  + "[InsuredAmount],[SystemAmount],[NewAmount],"
                   + "[FaceValue_1],[FaceValue_2],[FaceValue_3],[FaceValue_4],"
                   + "[Cassette_1],[Cassette_2],[Cassette_3],[Cassette_4],"
                + "[AtmsStatsGroup],[AtmsReplGroup],[AtmsReconcGroup],[DateInsert],[AuthUser],[OwnerUser],"
                + "[CitId],[ActiveRecord],[Operator])"
                + " VALUES "
                + " (@ReplOrderId,"
                 + "@OrdersCycleNo,"
                + "@AtmNo,@AtmName,@ReplCycleNo,"
                + "@BankId, @RespBranch,@BranchName,"
                + "@OffSite,@LastReplDt,@TypeOfRepl,@OverEst,@NextReplDt,"
                 + "@CurrNm,"
                + "@MinCash,@MaxCash,@ReplAlertDays,"
                 + "@ReconcDiff,@MoreMaxCash, @LessMinCash,@NeedType,"
                  + "@CurrCassettes,@CurrentDeposits,@EstReplDt,"
                  + "@NewEstReplDt,"
                  + "@InsuredAmount,@SystemAmount,@NewAmount,"
                  + "@FaceValue_1,@FaceValue_2,@FaceValue_3,@FaceValue_4,"
                  + "@Cassette_1,@Cassette_2,@Cassette_3,@Cassette_4,"
                + "@AtmsStatsGroup,@AtmsReplGroup,@AtmsReconcGroup,@DateInsert,@AuthUser,@OwnerUser,"
                + "@CitId,@ActiveRecord,@Operator)"
                  + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReplOrderId", ReplOrderId);
                        //cmd.Parameters.AddWithValue("@AuthorisedDate", AuthorisedDate);
                        cmd.Parameters.AddWithValue("@OrdersCycleNo", OrdersCycleNo);

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmName", AtmName);

                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@RespBranch", RespBranch);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);

                        cmd.Parameters.AddWithValue("@OffSite", OffSite);
                        cmd.Parameters.AddWithValue("@LastReplDt", LastReplDt);
                        cmd.Parameters.AddWithValue("@TypeOfRepl", TypeOfRepl);

                        cmd.Parameters.AddWithValue("@OverEst", OverEst);

                        cmd.Parameters.AddWithValue("@NextReplDt", NextReplDt);

                        cmd.Parameters.AddWithValue("@CurrNm", CurrNm);

                        cmd.Parameters.AddWithValue("@MinCash", MinCash);
                        cmd.Parameters.AddWithValue("@MaxCash", MaxCash);
                        cmd.Parameters.AddWithValue("@ReplAlertDays", ReplAlertDays);

                        cmd.Parameters.AddWithValue("@ReconcDiff", ReconcDiff);
                        cmd.Parameters.AddWithValue("@MoreMaxCash", MoreMaxCash);
                        cmd.Parameters.AddWithValue("@LessMinCash", LessMinCash);
                        cmd.Parameters.AddWithValue("@NeedType", NeedType);

                        cmd.Parameters.AddWithValue("@CurrCassettes", CurrCassettes);
                        cmd.Parameters.AddWithValue("@CurrentDeposits", CurrentDeposits);
                        cmd.Parameters.AddWithValue("@EstReplDt", EstReplDt);

                        cmd.Parameters.AddWithValue("@NewEstReplDt", NewEstReplDt);

                        cmd.Parameters.AddWithValue("@InsuredAmount", InsuredAmount);
                        cmd.Parameters.AddWithValue("@SystemAmount", SystemAmount);
                        cmd.Parameters.AddWithValue("@NewAmount", NewAmount);

                        cmd.Parameters.AddWithValue("@FaceValue_1", FaceValue_1);
                        cmd.Parameters.AddWithValue("@Cassette_1", Cassette_1);
                        cmd.Parameters.AddWithValue("@FaceValue_2", FaceValue_2);
                        cmd.Parameters.AddWithValue("@Cassette_2", Cassette_2);
                        cmd.Parameters.AddWithValue("@FaceValue_3", FaceValue_3);
                        cmd.Parameters.AddWithValue("@Cassette_3", Cassette_3);
                        cmd.Parameters.AddWithValue("@FaceValue_4", FaceValue_4);
                        cmd.Parameters.AddWithValue("@Cassette_4", Cassette_4);

                        cmd.Parameters.AddWithValue("@AtmsStatsGroup", AtmsStatsGroup);
                        cmd.Parameters.AddWithValue("@AtmsReplGroup", AtmsReplGroup);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@DateInsert", DateInsert);

                        cmd.Parameters.AddWithValue("@AuthUser", AuthUser);

                        cmd.Parameters.AddWithValue("@OwnerUser", OwnerUser);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@ActiveRecord", ActiveRecord);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
                        ReplOrderNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return ReplOrderNo;
        }

        //
        // DELETE Replenishment order 
        //
        public void DeleteReplOrder(int InReplOrderNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReplOrdersTable] "
                            + " WHERE ReplOrderNo =  @ReplOrderNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ReplOrderNo", InReplOrderNo);

                        //rows number of record got updated

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

        //
        // DELETE All Replenishment orders for this cycle
        //
        public void DeleteReplOrderForThisCycle(int InOrdersCycleNo)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int DelNumber; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReplOrdersTable] "
                            + " WHERE OrdersCycleNo =  @OrdersCycleNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@OrdersCycleNo", InOrdersCycleNo);

                        //rows number of record got updated

                        DelNumber = cmd.ExecuteNonQuery();
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

using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMITMXSettlementBanksToBanksFTFeesCycleTotals : Logger
    {
        public RRDMITMXSettlementBanksToBanksFTFeesCycleTotals() : base() { }

        public int SeqNo;
        public int ITMXSettlementCycle;
        public DateTime DateTmCreated;
        public string BankA;
        public string BankB;
        public string FeesEntity;
        public string Range;
        public decimal DRAmount;
        public decimal CRAmount;
        public bool IsProcessed;
        public string Operator;

        // Define the data tables 
        public DataTable TableTotalsFeesForBank = new DataTable();

        public DataTable TableTotalsForAllFeesEntities = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        //
        // Methods 
        // READ BanksToBanks FEES
        // BY PAIR OF BANKS 
        // FILL UP A TABLE
        //
        public void ReadTableTotalsForAllFeesEntities(string InOperator, string InSignedId, int InITMXSettlementCycle, string InBankA)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableTotalsForAllFeesEntities = new DataTable();
            TableTotalsForAllFeesEntities.Clear();

            TotalSelected = 0;

            if (InBankA == "")
            {
                /*
                SELECT FeesEntity, ITMXSettlementCycle As Cycle, CAST(DateTmCreated AS Date) AS Date
                , SUM(DRAmount)As ToDebited
                , SUM(CRAmount) As ToCredited
                , SUM(CRAmount - DRAmount) As NetFees
                FROM[ATMS].[dbo].[ITMXSettlementBanksToBanksFTFeesCycleTotals]
                Where[ITMXSettlementCycle] = 503
                Group by FeesEntity, ITMXSettlementCycle, CAST(DateTmCreated AS Date)
                */

                SqlString =
                    " SELECT FeesEntity ,  ITMXSettlementCycle As SettlementCycle, CAST(DateTmCreated AS Date) AS Date "
                              + " , SUM(DRAmount) As DebitAmount "
                              + " , SUM(CRAmount) As CreditAmount "
                              + " , SUM(CRAmount - DRAmount) As NetFees "
                              + " FROM [ATMS].[dbo].[ITMXSettlementBanksToBanksFTFeesCycleTotals] "
                              + " WHERE Operator = @Operator AND ITMXSettlementCycle = @ITMXSettlementCycle "
                              + " Group by FeesEntity, ITMXSettlementCycle, CAST(DateTmCreated AS Date)";

            }
            else
            {
                SqlString =
                              " SELECT FeesEntity ,  ITMXSettlementCycle As SettlementCycle, CAST(DateTmCreated AS Date) AS Date "
                              + " , SUM(DRAmount) As DebitAmount "
                              + " , SUM(CRAmount) As CreditAmount "
                              + " , SUM(CRAmount - DRAmount) As NetFees "
                              + " FROM [ATMS].[dbo].[ITMXSettlementBanksToBanksFTFeesCycleTotals] "
                              + " WHERE Operator = @Operator AND ITMXSettlementCycle = @ITMXSettlementCycle AND FeesEntity = @FeesEntity"
                              + " Group by FeesEntity, ITMXSettlementCycle, CAST(DateTmCreated AS Date)";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    { 

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@ITMXSettlementCycle", InITMXSettlementCycle);

                    if (InBankA != "")
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FeesEntity", InBankA);
                    }

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TableTotalsForAllFeesEntities);

                    // Close conn
                    conn.Close();

                    //ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();
                    RRDMBanks Ba = new RRDMBanks();

                    //Clear Table 
                    Tr.DeleteAllTableEntriesReport41(InSignedId);

                    int I = 0;

                    while (I <= (TableTotalsForAllFeesEntities.Rows.Count - 1))
                    {

                        RecordFound = true;

                        Tr.FeesEntity = (string)TableTotalsForAllFeesEntities.Rows[I]["FeesEntity"];
                        Ba.ReadBank(Tr.FeesEntity);
                        Tr.FullName = Ba.BankName;

                        Tr.SettlementCycle = (int)TableTotalsForAllFeesEntities.Rows[I]["SettlementCycle"];
                        Tr.Date = (DateTime)TableTotalsForAllFeesEntities.Rows[I]["Date"];

                        Tr.DebitAmount = (decimal)TableTotalsForAllFeesEntities.Rows[I]["DebitAmount"];
                        Tr.CreditAmount = (decimal)TableTotalsForAllFeesEntities.Rows[I]["CreditAmount"];

                        Tr.NetFees = (decimal)TableTotalsForAllFeesEntities.Rows[I]["NetFees"];

                        Tr.InsertReport41(InSignedId);

                        I++; // Read Next entry of the table 
                    }
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ Banks STATEMENT 
        // BY PAIR OF BANKS 
        // FILL UP A TABLE
        //
        public void ReadTableFeesTotalsForABankOverAllSettlements(string InOperator, string InSignedId, string InBankA, int InFrom, int InTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableTotalsFeesForBank = new DataTable();
            TableTotalsFeesForBank.Clear();

            TotalSelected = 0;

            //ITMXSettlementCycle = (int)rdr["ITMXSettlementCycle"];
            //DateTmCreated = (DateTime)rdr["DateTmCreated"];
            //BankA = (string)rdr["BankA"];
            //BankB = (string)rdr["BankB"];

            //FeesEntity = (string)rdr["FeesEntity"];
            //Range = (string)rdr["Range"];

            //DRAmount = (decimal)rdr["DRAmount"];
            //CRAmount = (decimal)rdr["CRAmount"];
            //IsProcessed = (bool)rdr["IsProcessed"];

            SqlString =
                   " SELECT [ITMXSettlementCycle] As SettlementCycle, CAST(DateTmCreated AS Date) AS Date    "
                     + "  ,SUM([DRAmount]) AS DebitedAmount  "
                     + "  ,SUM([CRAmount]) AS CreditedAmount "
                     + "  ,SUM(CRAmount - DRAmount) AS Difference  "
                     + "   FROM[ATMS].[dbo].[ITMXSettlementBanksToBanksFTFeesCycleTotals] "
                     + "   WHERE FeesEntity = @BankA AND ITMXSettlementCycle >= @From AND ITMXSettlementCycle <= @To "
                     + "   GROUP by ITMXSettlementCycle, CAST(DateTmCreated AS Date)  "
                     + "   ORDER by ITMXSettlementCycle DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using(SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                        { 

                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@BankA ", InBankA);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@From ", InFrom);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@To ", InTo);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TableTotalsFeesForBank);

                    // Close conn
                    conn.Close();

                    //ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();
                    RRDMBanks Ba = new RRDMBanks();

                    //Clear Table 
                    Tr.DeleteAllTableEntriesReport30(InSignedId);

                    int I = 0;

                    while (I <= (TableTotalsFeesForBank.Rows.Count - 1))
                    {

                        RecordFound = true;

                        Tr.SettlementCycle = (int)TableTotalsFeesForBank.Rows[I]["SettlementCycle"];
                        Tr.Date = (DateTime)TableTotalsFeesForBank.Rows[I]["Date"];
                        Tr.DebitedAmount = (decimal)TableTotalsFeesForBank.Rows[I]["DebitedAmount"];
                        Tr.CreditedAmount = (decimal)TableTotalsFeesForBank.Rows[I]["CreditedAmount"];

                        Tr.Difference = (decimal)TableTotalsFeesForBank.Rows[I]["Difference"];

                        Tr.InsertReport30(InSignedId);

                        I++; // Read Next entry of the table 
                    }
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ BanksToBanksDailyTotals
        // Sum up in second table 
        // FILL UP A TABLE
        //
        public void ReadTableFeesTotalsForFeesEntity(string InOperator, string InSignedId, int InITMXSettlementCycle, string InFeesEntity)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableTotalsFeesForBank = new DataTable();
            TableTotalsFeesForBank.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableTotalsFeesForBank.Columns.Add("FeesEntity", typeof(string));
            TableTotalsFeesForBank.Columns.Add("BankA", typeof(string));
            TableTotalsFeesForBank.Columns.Add("BankB", typeof(string));
            TableTotalsFeesForBank.Columns.Add("DRAmount", typeof(decimal));
            TableTotalsFeesForBank.Columns.Add("CRAmount", typeof(decimal));
            TableTotalsFeesForBank.Columns.Add("NetFees", typeof(decimal));
            TableTotalsFeesForBank.Columns.Add("DateTmCreated", typeof(DateTime));
            TableTotalsFeesForBank.Columns.Add("SettlementCycle", typeof(int));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXSettlementBanksToBanksFTFeesCycleTotals]"
                    + " WHERE Operator = @Operator AND ITMXSettlementCycle = @ITMXSettlementCycle AND FeesEntity = @FeesEntity ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@FeesEntity", InFeesEntity);
                        cmd.Parameters.AddWithValue("@ITMXSettlementCycle", InITMXSettlementCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            ITMXSettlementCycle = (int)rdr["ITMXSettlementCycle"];
                            DateTmCreated = (DateTime)rdr["DateTmCreated"];
                            BankA = (string)rdr["BankA"];
                            BankB = (string)rdr["BankB"];

                            FeesEntity = (string)rdr["FeesEntity"];
                            Range = (string)rdr["Range"];

                            DRAmount = (decimal)rdr["DRAmount"];
                            CRAmount = (decimal)rdr["CRAmount"];
                            IsProcessed = (bool)rdr["IsProcessed"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableTotalsFeesForBank.NewRow();

                            RowSelected["FeesEntity"] = FeesEntity;
                            RowSelected["BankA"] = BankA;
                            RowSelected["BankB"] = BankB;
                            RowSelected["DRAmount"] = DRAmount;
                            RowSelected["CRAmount"] = CRAmount;
                            RowSelected["NetFees"] = CRAmount - DRAmount;
                            RowSelected["DateTmCreated"] = DateTmCreated;
                            RowSelected["SettlementCycle"] = ITMXSettlementCycle;
                            // ADD ROW
                            TableTotalsFeesForBank.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();

                    //Clear Table 
                    Tr.DeleteReport42(InSignedId);

                    int I = 0;

                    while (I <= (TableTotalsFeesForBank.Rows.Count - 1))
                    {

                        RecordFound = true;

                        Tr.FeesEntity = (string)TableTotalsFeesForBank.Rows[I]["FeesEntity"];
                        Tr.BankA = (string)TableTotalsFeesForBank.Rows[I]["BankA"];
                        Tr.BankB = (string)TableTotalsFeesForBank.Rows[I]["BankB"];

                        Tr.DRAmount = (decimal)TableTotalsFeesForBank.Rows[I]["DRAmount"];
                        Tr.CRAmount = (decimal)TableTotalsFeesForBank.Rows[I]["CRAmount"];
                        Tr.NetFees = (decimal)TableTotalsFeesForBank.Rows[I]["NetFees"];

                        Tr.DateTmCreated = (DateTime)TableTotalsFeesForBank.Rows[I]["DateTmCreated"];
                        Tr.SettlementCycle = (int)TableTotalsFeesForBank.Rows[I]["SettlementCycle"];

                        Tr.InsertReport42(InSignedId);

                        I++; // Read Next entry of the table 
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        //
        // Methods 
        // READ BanksToBanksDailyTotals
        // FILL UP A TABLE
        //
        public void ReadBanksToBanksCycleTotalsByJobCategory(string InOperator, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ITMXSettlementBanksToBanksFTFeesCycleTotals] "
                      + " WHERE Operator = @Operator AND JobCategory = @JobCategory "
                      + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@JobCategory", InJobCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            ITMXSettlementCycle = (int)rdr["ITMXSettlementCycle"];
                            DateTmCreated = (DateTime)rdr["DateTmCreated"];
                            BankA = (string)rdr["BankA"];
                            BankB = (string)rdr["BankB"];

                            FeesEntity = (string)rdr["FeesEntity"];
                            Range = (string)rdr["Range"];

                            DRAmount = (decimal)rdr["DRAmount"];
                            CRAmount = (decimal)rdr["CRAmount"];
                            IsProcessed = (bool)rdr["IsProcessed"];

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

                    CatchDetails(ex);
                }
        }
        //
        // Methods 
        // READ ReconcCategoriesMatchingSessions Specific 
        // 
        //
        public void ReadFeesBanksToBanksCycleTotalsById(string InOperator, int InITMXSettlementCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ITMXSettlementBanksToBanksFTFeesCycleTotals] "
                      + " WHERE Operator = @Operator AND ITMXSettlementCycle = @ITMXSettlementCycle ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ITMXSettlementCycle", InITMXSettlementCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                            SeqNo = (int)rdr["SeqNo"];
                            ITMXSettlementCycle = (int)rdr["ITMXSettlementCycle"];
                            DateTmCreated = (DateTime)rdr["DateTmCreated"];
                            BankA = (string)rdr["BankA"];
                            BankB = (string)rdr["BankB"];

                            FeesEntity = (string)rdr["FeesEntity"];
                            Range = (string)rdr["Range"];

                            DRAmount = (decimal)rdr["DRAmount"];
                            CRAmount = (decimal)rdr["CRAmount"];
                            IsProcessed = (bool)rdr["IsProcessed"];

                            Operator = (string)rdr["Operator"];

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

                    CatchDetails(ex);
                }
        }

        //
        //  
        // READ TOTALS and Make Pair of transactions  
        // 
        //

        //int WTxnCode;
        //int WDisputeActionId;
        string WSettlementClearingAccount;

        string SettlementBankId;
        string FullName;
        decimal DebitAmount;
        decimal CreditAmount;
        decimal Difference;

        public void CreateTransForFeesSettlementClearing(string InUserId, string InAuthUser, string InOperator, int InITMXSettlementCycle)
        {

            RRDMGetUniqueNumber Gnext = new RRDMGetUniqueNumber();

            RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            RRDMBanks Ba = new RRDMBanks();

            //Find Clearing Account 
            Ba.ReadSettlementBank(InOperator);
            if (Ba.RecordFound == true)
            {
                SettlementBankId = Ba.BankId; 
                WSettlementClearingAccount = Ba.SettlementClearingAccount;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("There is no Settlement Clearing Account");
                return;
            }

            //Read Table  
            ReadTableTotalsForAllFeesEntities(InOperator, InUserId, InITMXSettlementCycle, "");

            RecordFound = false;
            try
            {
                int I = 0;

                while (I <= (TableTotalsForAllFeesEntities.Rows.Count - 1))
                {

                    RecordFound = true;

                    FeesEntity = (string)TableTotalsForAllFeesEntities.Rows[I]["FeesEntity"];
                    Ba.ReadBank(FeesEntity);
                    FullName = Ba.BankName;

                    int SettlementCycle = (int)TableTotalsForAllFeesEntities.Rows[I]["SettlementCycle"];
                    DateTime Date = (DateTime)TableTotalsForAllFeesEntities.Rows[I]["Date"];

                    DebitAmount = (decimal)TableTotalsForAllFeesEntities.Rows[I]["DebitAmount"];
                    CreditAmount = (decimal)TableTotalsForAllFeesEntities.Rows[I]["CreditAmount"];

                    decimal NetFees = (decimal)TableTotalsForAllFeesEntities.Rows[I]["NetFees"];

                    // Testing
                    Difference = 107; 
                    if (NetFees > 0) // Credit Settlement and Debit Clearing 
                    {
                        Tp.OriginId = "06"; // "05" Settlement  
                        Tp.OriginName = "SettlementFeesAuth";  // ORIGIN 
                        Tp.RMCateg = "FeesSettlement";
                        Tp.RMCategCycle = InITMXSettlementCycle; // Pass it
                        Tp.UniqueRecordId = Gnext.GetNextValue();

                        Tp.ErrNo = 0;
                        Tp.AtmNo = "";
                        Tp.SesNo = 0;
                        Tp.BankId = SettlementBankId; 

                        Tp.AtmTraceNo = 0;

                        Tp.BranchId = "Central Bank HO";
                        Tp.AtmDtTime = NullPastDate;
                        //Tp.HostDtTime = DateTimeH;
                        Tp.SystemTarget = 10;

                        Tp.CardNo = "N/A";
                        Tp.CardOrigin = 10; // Find OUT ... 

                        // First Entry 
                        Tp.TransType = 21; // 
                        Tp.TransDesc = "Credit-FeesSettlement for :" + FeesEntity;

                        Tp.AccNo = Ba.SettlementAccount; // Think About it 
                        Tp.GlEntry = true;

                        // Second Entry
                        Tp.TransType2 = 11;

                        Tp.TransDesc2 = "Debit at FeesSettlement for Bank : " + FeesEntity;

                        Tp.AccNo2 = WSettlementClearingAccount; //  

                        Tp.GlEntry2 = true;
                        // End Second Entry 

                        Tp.CurrDesc = Ba.BasicCurName; // Find it 
                        Tp.TranAmount = Difference;
                        Tp.AuthCode = 0;
                        Tp.RefNumb = 0;
                        Tp.RemNo = 0;
                        Tp.TransMsg = "No comment from user";
                        Tp.AtmMsg = "";
                        Tp.MakerUser = InUserId;
                        Tp.AuthUser = InAuthUser;
                        Tp.OpenDate = DateTime.Now;
                        Tp.OpenRecord = true;
                        Tp.Operator = Operator;
                        //NOSTRO
                        Tp.NostroCcy = "";
                        Tp.NostroAdjAmt = 0;

                        int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

                    }

                    if (NetFees < 0) // Debit Settlement and Credit Clearing 
                    {

                        Tp.OriginId = "06"; // "05" Settlement  
                        Tp.OriginName = "SettlementFeesAuth";  // ORIGIN 
                        Tp.RMCateg = "FeesSettlement";
                        Tp.RMCategCycle = InITMXSettlementCycle; // Pass it
                        Tp.UniqueRecordId = Gnext.GetNextValue();

                        Tp.ErrNo = 0;
                        Tp.AtmNo = "";
                        Tp.SesNo = 0;
                        Tp.BankId = SettlementBankId;

                        Tp.AtmTraceNo = 0;

                        Tp.BranchId = "Central Bank HO";
                        Tp.AtmDtTime = DateTime.Now;
                        //Tp.HostDtTime = DateTimeH;
                        Tp.SystemTarget = 10;

                        Tp.CardNo = "N/A";
                        Tp.CardOrigin = 9; // Find OUT ... 

                        // First Entry 
                        Tp.TransType = 11; // 
                        Tp.TransDesc = "Debit-Settlement for :" + FeesEntity;

                        Tp.AccNo = Ba.SettlementAccount; // Think About it 
                        Tp.GlEntry = true;

                        // Second Entry
                        Tp.TransType2 = 21;

                        Tp.TransDesc2 = "Credit at Settlement for Bank : " + FeesEntity;

                        Tp.AccNo2 = WSettlementClearingAccount; //  

                        Tp.GlEntry2 = true;
                        // End Second Entry 

                        Tp.CurrDesc = Ba.BasicCurName; // Find it 
                        Tp.TranAmount = -Difference;
                        Tp.AuthCode = 0;
                        Tp.RefNumb = 0;
                        Tp.RemNo = 0;
                        Tp.TransMsg = "No comment from user";
                        Tp.AtmMsg = "";
                        Tp.MakerUser = InUserId;
                        Tp.AuthUser = InAuthUser;
                        Tp.OpenDate = DateTime.Now;
                        Tp.OpenRecord = true;
                        Tp.Operator = Operator;
                        //NOSTRO
                        Tp.NostroCcy = "";
                        Tp.NostroAdjAmt = 0;

                        int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

                    }

                    I++; // Read Next entry of the table 
                }

            }

            catch (Exception ex)
            {
                CatchDetails(ex);
            }
        }
    }
}

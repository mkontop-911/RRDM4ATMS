using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMITMXSettlementBanksToBanksFTCycleTotals : Logger
    {
        public RRDMITMXSettlementBanksToBanksFTCycleTotals() : base() { }

        public int SeqNo;
        public int ITMXSettlementCycle;
        public DateTime DateTmCreated;
       
        public string BankA;
        public string BankB;
        public string Description;
        public string TransactionType;
        public decimal DRAmount;
        public decimal CRAmount;
        public bool IsProcessed;
        public string Operator;

        // Define the data tables 
        public DataTable TableTotalsForBank = new DataTable();

        public DataTable TableTotalsForAllBanks = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // Methods 
        // READ BanksToBanksDailyTotals
        // BY PAIR OF BANKS 
        // FILL UP A TABLE
        //
        public void ReadTableTotalsForAllBanks(string InOperator, string InSignedId, int InITMXSettlementCycle, string InBankA)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableTotalsForAllBanks = new DataTable();
            TableTotalsForAllBanks.Clear();

            TotalSelected = 0;

            if (InBankA == "")
            {
                SqlString =
                                " SELECT BankA AS BANKA, SUM(DRAmount) AS DebitedAmount, SUM(CRAmount)AS CreditedAmount, "
                                   + " SUM(CRAmount - DRAmount) AS Difference "
                                   + " FROM[ATMS].[dbo].[ITMXSettlementBanksToBanksFTCycleTotals] "
                                   + " WHERE Operator = @Operator AND ITMXSettlementCycle = @ITMXSettlementCycle "
                                   + "  GROUP BY  BankA  "
                                   + "  Order by BankA  ";
            }
            else
            {
                SqlString =
                               " SELECT BankA AS BANKA, SUM(DRAmount) AS DebitedAmount, SUM(CRAmount)AS CreditedAmount, "
                                  + " SUM(CRAmount - DRAmount) AS Difference "
                                  + " FROM[ATMS].[dbo].[ITMXSettlementBanksToBanksFTCycleTotals] "
                                  + " WHERE Operator = @Operator AND ITMXSettlementCycle = @ITMXSettlementCycle AND BankA = @BankA"
                                  + "  GROUP BY  BankA  " ; 
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
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@BankA", InBankA);
                        }

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableTotalsForAllBanks);

                        // Close conn
                        conn.Close();

   
                        //ReadTable And Insert In Sql Table 
                        RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();
                        RRDMBanks Ba = new RRDMBanks();

                        //Clear Table 
                        Tr.DeleteAllTableEntriesReport31(InSignedId);

                        int I = 0;

                        while (I <= (TableTotalsForAllBanks.Rows.Count - 1))
                        {

                            RecordFound = true;

                            Tr.BankId = (string)TableTotalsForAllBanks.Rows[I]["BANKA"];
                            Ba.ReadBank(Tr.BankId);
                            Tr.FullName = Ba.BankName;
                            Tr.DebitAmount = (decimal)TableTotalsForAllBanks.Rows[I]["DebitedAmount"];
                            Tr.CreditAmount = (decimal)TableTotalsForAllBanks.Rows[I]["CreditedAmount"];

                            Tr.Difference = (decimal)TableTotalsForAllBanks.Rows[I]["Difference"];

                            Tr.InsertReport31(InSignedId);

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
        public void ReadTableTotalsForABankOverAllSettlements(string InOperator, string InSignedId, string InBankA, int InFrom, int InTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableTotalsForBank = new DataTable();
            TableTotalsForBank.Clear();

            TotalSelected = 0;

            SqlString =
                   " SELECT [ITMXSettlementCycle] As SettlementCycle, CAST(DateTmCreated AS Date) AS Date    "
                     + "  ,SUM([DRAmount]) AS DebitedAmount  "
                     + "  ,SUM([CRAmount]) AS CreditedAmount "
                     + "  ,SUM(CRAmount - DRAmount) AS Difference  "
                     + "   FROM[ATMS].[dbo].[ITMXSettlementBanksToBanksFTCycleTotals] "
                     + "   WHERE BankA = @BankA AND ITMXSettlementCycle >= @From AND ITMXSettlementCycle <= @To "
                     + "   GROUP by ITMXSettlementCycle, CAST(DateTmCreated AS Date)  "
                     + "   ORDER by ITMXSettlementCycle DESC "; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@BankA ", InBankA);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@From ", InFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@To ", InTo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableTotalsForBank);

                        // Close conn
                        conn.Close();

                        //ReadTable And Insert In Sql Table 
                        RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();
                        RRDMBanks Ba = new RRDMBanks();

                        //Clear Table 
                        Tr.DeleteAllTableEntriesReport30(InSignedId);

                        int I = 0;

                        while (I <= (TableTotalsForBank.Rows.Count - 1))
                        {

                            RecordFound = true;

                            Tr.SettlementCycle = (int)TableTotalsForBank.Rows[I]["SettlementCycle"];
                            Tr.Date = (DateTime)TableTotalsForBank.Rows[I]["Date"];
                            Tr.DebitedAmount = (decimal)TableTotalsForBank.Rows[I]["DebitedAmount"];
                            Tr.CreditedAmount = (decimal)TableTotalsForBank.Rows[I]["CreditedAmount"];

                            Tr.Difference = (decimal)TableTotalsForBank.Rows[I]["Difference"];

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
        public void ReadTableTotalsForBank(string InOperator, string InSignedId, int InITMXSettlementCycle, string InBankA)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableTotalsForBank = new DataTable();
            TableTotalsForBank.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableTotalsForBank.Columns.Add("BankA", typeof(string));
            TableTotalsForBank.Columns.Add("BankB", typeof(string));
            TableTotalsForBank.Columns.Add("DRAmount", typeof(decimal));
            TableTotalsForBank.Columns.Add("CRAmount", typeof(decimal));
            TableTotalsForBank.Columns.Add("Difference", typeof(decimal));
            TableTotalsForBank.Columns.Add("DateTmCreated", typeof(DateTime));
            TableTotalsForBank.Columns.Add("SettlementCycle", typeof(int));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXSettlementBanksToBanksFTCycleTotals]"
                    + " WHERE Operator = @Operator AND ITMXSettlementCycle = @ITMXSettlementCycle AND BankA = @BankA "; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@BankA", InBankA);
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
                            Description = (string)rdr["Description"];

                            TransactionType = (string)rdr["TransactionType"];
                            DRAmount = (decimal)rdr["DRAmount"];
                            CRAmount = (decimal)rdr["CRAmount"];
                            IsProcessed = (bool)rdr["IsProcessed"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableTotalsForBank.NewRow();

                            RowSelected["BankA"] = BankA ;
                            RowSelected["BankB"] = BankB;             
                            RowSelected["DRAmount"] = DRAmount; 
                            RowSelected["CRAmount"] = CRAmount;
                            RowSelected["Difference"] = CRAmount - DRAmount;
                            RowSelected["DateTmCreated"] = DateTmCreated;
                            RowSelected["SettlementCycle"] = ITMXSettlementCycle;
                            // ADD ROW
                            TableTotalsForBank.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();
                 
                    //Clear Table 
                    Tr.DeleteReport32(InSignedId);

                    int I = 0;

                    while (I <= (TableTotalsForBank.Rows.Count - 1))
                    {

                        RecordFound = true;

                        Tr.BankId = (string)TableTotalsForBank.Rows[I]["BANKA"];
                        Tr.OtherBank = (string)TableTotalsForBank.Rows[I]["BankB"];
                        Tr.DebitAmount = (decimal)TableTotalsForBank.Rows[I]["DRAmount"];
                        Tr.CreditAmount = (decimal)TableTotalsForBank.Rows[I]["CRAmount"];

                        Tr.Difference = (decimal)TableTotalsForBank.Rows[I]["Difference"];

                        Tr.InsertReport32(InSignedId);

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
        public void ReadBanksToBanksDailyTotalsByJobCategory(string InOperator, string InJobCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ITMXSettlementBanksToBanksFTCycleTotals] "
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
                            Description = (string)rdr["Description"];

                            TransactionType = (string)rdr["TransactionType"];
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
        public void ReadBanksToBanksDailyTotalsById(string InOperator, int InITMXSettlementCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ITMXSettlementBanksToBanksFTCycleTotals] "
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
                            Description = (string)rdr["Description"];

                            TransactionType = (string)rdr["TransactionType"];
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

        string BankId;
        string FullName;
        decimal DebitAmount;
        decimal CreditAmount;
        decimal Difference;

        public void CreateTransForSettlementClearing(string InUserId, string InAuthUser, string InOperator, int InITMXSettlementCycle)
        {
            RRDMGetUniqueNumber Gnext = new RRDMGetUniqueNumber(); 

            RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();
         
            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            RRDMBanks Ba = new RRDMBanks();

            //Find Clearing Account 
            Ba.ReadSettlementBank(InOperator); 
            if (Ba.RecordFound == true)
            {
                WSettlementClearingAccount = Ba.SettlementClearingAccount; 
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("There is no Settlement Clearing Account"); 
                return; 
            }

            //Read Table  
            ReadTableTotalsForAllBanks(InOperator, InUserId, InITMXSettlementCycle, "");

            RecordFound = false;
            try
            {
                int I = 0;

                while (I <= (TableTotalsForAllBanks.Rows.Count - 1))
                {

                    RecordFound = true;

                    BankId = (string)TableTotalsForAllBanks.Rows[I]["BANKA"];
                    Ba.ReadBank(BankId);
                    FullName = Ba.BankName;
                    DebitAmount = (decimal)TableTotalsForAllBanks.Rows[I]["DebitedAmount"];
                    CreditAmount = (decimal)TableTotalsForAllBanks.Rows[I]["CreditedAmount"];

                    Difference = (decimal)TableTotalsForAllBanks.Rows[I]["Difference"];

                    if (Difference > 0 ) // Credit Settlement and Debit Clearing 
                    {
                        Tp.OriginId = "05"; // "05" Settlement  
                        Tp.OriginName = "SettlementAuth";  // ORIGIN 
                        Tp.RMCateg = "Settlement";
                        Tp.RMCategCycle = InITMXSettlementCycle; // Pass it
                        Tp.UniqueRecordId = Gnext.GetNextValue(); 

                        Tp.ErrNo = 0;
                        Tp.AtmNo = "";
                        Tp.SesNo = 0;
                        Tp.BankId = Operator;

                        Tp.AtmTraceNo = 0;

                        Tp.BranchId = "Central Bank HO";
                        Tp.AtmDtTime = NullPastDate;
                        //Tp.HostDtTime = DateTimeH;
                        Tp.SystemTarget = 10;

                        Tp.CardNo = "N/A";
                        Tp.CardOrigin = 10 ; // Find OUT ... 

                        // First Entry 
                        Tp.TransType = 21; // 
                        Tp.TransDesc = "Credit-Settlement for :" + BankId;
                        
                        Tp.AccNo = Ba.SettlementAccount; // Think About it 
                        Tp.GlEntry = true;

                        // Second Entry
                        Tp.TransType2 = 11;

                        Tp.TransDesc2 = "Debit at Settlement for Bank : " + BankId;

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

                    if (Difference < 0) // Debit Settlement and Credit Clearing 
                    {

                        Tp.OriginId = "05"; // "05" Settlement  
                        Tp.OriginName = "SettlementAuth";  // ORIGIN 
                        Tp.RMCateg = "Settlement";
                        Tp.RMCategCycle = InITMXSettlementCycle; // Pass it
                        Tp.UniqueRecordId = Gnext.GetNextValue();

                        Tp.ErrNo = 0;
                        Tp.AtmNo = "";
                        Tp.SesNo = 0;
                        Tp.BankId = Operator;

                        Tp.AtmTraceNo = 0;

                        Tp.BranchId = "Central Bank HO";
                        Tp.AtmDtTime = DateTime.Now;
                        //Tp.HostDtTime = DateTimeH;
                        Tp.SystemTarget = 10;

                        Tp.CardNo = "N/A";
                        Tp.CardOrigin = 9; // Find OUT ... 

                        // First Entry 
                        Tp.TransType = 11; // 
                        Tp.TransDesc = "Debit-Settlement for :" + BankId;

                        Tp.AccNo = Ba.SettlementAccount; // Think About it 
                        Tp.GlEntry = true;

                        // Second Entry
                        Tp.TransType2 = 21;

                        Tp.TransDesc2 = "Credit at Settlement for Bank : " + BankId;

                        Tp.AccNo2 = WSettlementClearingAccount; //  

                        Tp.GlEntry2 = true;
                        // End Second Entry 

                        Tp.CurrDesc = Ba.BasicCurName; // Find it 
                        Tp.TranAmount = - Difference;
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



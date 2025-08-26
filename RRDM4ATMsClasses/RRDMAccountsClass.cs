using System;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMAccountsClass : Logger
    {
        public RRDMAccountsClass() : base() { }
        // f
        // 1) Input Accounts
        // 2) Read Accounts
        //

        public int SeqNumber;
        public string BranchId; // Each Branch can have one or more accounts eg Excess and Shortage
                                // Also An atm cash account is identified from Branch and AccNo
                                // Same account can belong to many branches 
        public string ShortAccID; // 20 Customer Acc
                                  // 30 for Atm Cash
                                  // 35 CIT_Account
                                  // 40 Branch_Excess
                                  // 50 Branch_Shortage
                                  // 70 Category_Account
                                  // 90 Has to do with Swift 
        public string AccName;
        public string CurrNm;
        public string AccNo;

        public string EntityNm;
        public string EntityNo;
        public string AtmNo;
        public string UserId;

        public string CategoryId; // Category Accounts
        public string AccNoInternal; // Used For Nostro Definition 
                                     // AccNo is the Vostro belonging to external Bank
        public string BankId;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable AccountsForAtmDataTable;

        public DataTable AccountsTable;

        string ParId ;
        string OccurId;
        string WTable; 

        RRDMGasParameters Gp = new RRDMGasParameters();  

        readonly string connectionString = ConfigurationManager.ConnectionStrings
                                  ["ATMSConnectionString"].ConnectionString;

        // Account Fields
        private void ReadAccountFields(SqlDataReader rdr)
        {
            SeqNumber = (int)rdr["SeqNumber"];

            BranchId = (string)rdr["BranchId"];

            ShortAccID = (string)rdr["ShortAccID"];

            AccName = (string)rdr["AccName"];
            CurrNm = (string)rdr["CurrNm"];
            AccNo = (string)rdr["AccNo"];

            EntityNm = (string)rdr["EntityNm"];
            EntityNo = (string)rdr["EntityNo"];

            AtmNo = (string)rdr["AtmNo"];
            UserId = (string)rdr["UserId"];

            CategoryId = (string)rdr["CategoryId"];
            AccNoInternal = (string)rdr["AccNoInternal"];

            BankId = (string)rdr["BankId"];
            Operator = (string)rdr["Operator"];
        }
        //
        // READ ACCOUNT BASED ON ShortAccID
        //
        public void ReadAccountsBasedOn_ShortAccID(string InOperator,
                                                string InShortAccID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]"; 
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }
                
            }
            

            string SqlString = "SELECT TOP (1) * "
                         + " FROM " + WTable
                         + " WHERE Operator = @Operator "
                         + " AND ShortAccID = @ShortAccID "

                          + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ShortAccID", InShortAccID);
                        // cmd.Parameters.AddWithValue("@EntityNo", InEntityNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);
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
        // READ ACCOUNT NAME BASED ON ACCOUNT NO
        //
        public void ReadAccount_Name_Based_On_Account_No(string InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string SqlString = "SELECT TOP (1) * "
                         + " FROM " + WTable
                         + " WHERE AccNo = @AccNo "
                          + " ORDER By SeqNumber Desc ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccNo", InAccNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);
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
        // READ ACCOUNT NAME BASED ON ACCOUNT NO
        //
        public void ReadAccountTableBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string SqlString = "SELECT  * "
                         + " FROM " + WTable + " "
                         + InSelectionCriteria;
            //  + " ORDER By SeqNumber Desc ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@AccNo", InAccNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);
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
        // READ ACCOUNT BASED ON EntityNo
        //
        public void ReadAccountsBasedOn_ShortAccID_EntityNo(string InOperator,
                                                string InShortAccID, string InEntityNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ClearDefineTable();

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string SqlString = "SELECT TOP (1) * "
                         + " FROM " + WTable
                         + " WHERE Operator = @Operator "
                         + " AND ShortAccID = @ShortAccID "
                         + " AND EntityNo = @EntityNo "
                          + " ORDER By SeqNumber Desc ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ShortAccID", InShortAccID);
                        cmd.Parameters.AddWithValue("@EntityNo", InEntityNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);
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
        // READ ACCOUNT BASED ON ATMNo  
        //
        public void ReadAccountsAndFillTableForAtmNo(string InOperator, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ClearDefineTable();

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            // When we put 1000 we want to get the accounts for ATM 
            // If different than 1000 we get the accounts fot the CIT 

            string SqlString = "SELECT *"
                         + " FROM " + WTable
                         // + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND AccName = 'ATM Cash' "
                         + " WHERE Operator = @Operator AND AtmNo = @AtmNo "
                          + " Order by AccNo ASC ";

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
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);

                            FillTableLine();

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
        public void ReadAccountsAtmCashForAtmNo(string InOperator, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ClearDefineTable();

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            // When we put 1000 we want to get the accounts for ATM 
            // If different than 1000 we get the accounts fot the CIT 

            string SqlString = "SELECT * "
                         + " FROM " + WTable 
                         + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND AccName = 'ATM Cash' "
                          + " Order by AccNo ASC ";

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
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);

                            //FillTableLine();

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
        // Find by By ATM No and short Account ID 
        //
        public void ReadAccountsAtmCashForAtmNoAndShortAccId(string InAtmNo, string InShortAccID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ClearDefineTable();

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            // When we put 1000 we want to get the accounts for ATM 
            // If different than 1000 we get the accounts fot the CIT 

            string SqlString = "SELECT *"
                         + " FROM " + WTable
                         + " WHERE AtmNo = @AtmNo AND ShortAccID = @ShortAccID "
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
                        cmd.Parameters.AddWithValue("@ShortAccID", InShortAccID);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);

                            //FillTableLine();

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
        // FindInfoForAccountingTransactions Based on the 
        //
        public string WAtm_BranchNo;
        public string WATM_CitNo;
        public string WAtm_CashAcno;
        public string WCit_GL_Acno;
        public string WCit_Branch_No;
        public string WBranch_ExcessAcno;
        public string WBranch_ShortageAcno;
        //public string W_TXNDST_Category_Acno; // this is the GL account for 123 say. 
        public string W_EGP_Category_Acno; // this is the GL account for 123 say. 
        public string W_USD_Category_Acno; // this is the GL account for MASTER say. 
        public string WTXNSRC;
        public string WTXNDEST;

        public void ReadAndFindInfoForAccountingTransactions(string InOperator, string InAtmNo,
                          string InCategoryId,
                          string InTXNSRC, string InTXNDEST, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 1 comes from ATMS REPLENISHMENT the InTXNSRC = "" and  InTXNDEST = ""
            // InMode = 2 comes from transaction so we need the InTXNSRC != "" and  InTXNDEST != ""

            // Need to find:
            // Branch for this ATM
            // CIT Number for this ATM
            // ATM - GL Account 
            // CIT - GL Account
            // WCit_Branch_No
            // Branch - Excess acc
            // Branch - Shortage Acc
            // Category Account no

            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMBank_Branches Bb = new RRDMBank_Branches();

            WAtm_BranchNo = "Not Found";
            WATM_CitNo = "Not Found";
            WAtm_CashAcno = "Not Found";
            WCit_GL_Acno = "N/A";
            WCit_Branch_No = "N/A";
            WBranch_ExcessAcno = "Not Found";
            WBranch_ShortageAcno = "Not Found";
            // W_TXNDST_Category_Acno = "N/A";
            W_EGP_Category_Acno = "N/A"; // this is the GL account for 123 say. 
            W_USD_Category_Acno = "N/A"; // this is the GL account for MASTER say. 
            WTXNSRC = InTXNSRC;
            WTXNDEST = InTXNDEST;

            try
            {
                Ac.ReadAtm(InAtmNo);

                WAtm_BranchNo = Ac.Branch;

                WATM_CitNo = Ac.CitId;

                // Find Atm Accno
                // ReadAndFindAccount(string InUserId, string InBranchId, string InCategoryId, string InOperator, string InAtmNo, string InCurrNm, string InAccName)
                ReadAndFindAccount("1000", "", "", InOperator, InAtmNo, Ac.DepCurNm, "ATM Cash");
                if (RecordFound == true)
                {
                    WAtm_CashAcno = AccNo;
                }
                else
                {
                    WAtm_CashAcno = "AccNo_Not_found";
                }
                // Find For Cit Accno
                if (WATM_CitNo != "1000")
                {
                    // Find account number
                    ReadAndFindAccount(WATM_CitNo, "", "", InOperator, "", Ac.DepCurNm, "CIT_Account");
                    if (RecordFound == true)
                    {
                        WCit_GL_Acno = AccNo;
                    }
                    else
                    {
                        WCit_GL_Acno = "AccNo_Not_found";
                    }

                    // Find For Cit Branch no
                    string WSelectionCriteria = " WHERE CitId ='" + WATM_CitNo + "'";
                    Bb.ReadBranchBySelectionCriteria(WSelectionCriteria);
                    if (Bb.RecordFound == true)
                    {
                        WCit_Branch_No = Bb.BranchId;
                    }
                    else
                    {
                        WCit_Branch_No = "Branch Not Found";
                    }
                }
                // 
                // FIND BRANCH EXCESS Or Shortage Account

                // ReadAndFindAccount(string InUserId, string InBranchId, string InCategoryId, string InOperator, string InAtmNo, string InCurrNm, string InAccName)

                ReadAndFindAccount("", WAtm_BranchNo, "", InOperator, "", Ac.DepCurNm, "Branch_Excess");
                if (RecordFound == true)
                {
                    WBranch_ExcessAcno = AccNo;
                }

                ReadAndFindAccount("", WAtm_BranchNo, "", InOperator, "", Ac.DepCurNm, "Branch_Shortage");
                if (RecordFound == true)
                {
                    WBranch_ShortageAcno = AccNo;
                }

                if (InMode == 2 & WTXNDEST != "1" & WTXNDEST != "")
                {
                    W_EGP_Category_Acno = "N/A"; // this is the GL account for 123 say. 
                    W_USD_Category_Acno = "N/A"; // this is the GL account for MASTER say. 

                    ReadAndFindAccount("", "", InCategoryId, InOperator, "", "EGP", "Category_Account");
                    if (RecordFound == true)
                    {
                        W_EGP_Category_Acno = AccNo;
                    }
                    ReadAndFindAccount("", "", InCategoryId, InOperator, "", "USD", "Category_Account");
                    if (RecordFound == true)
                    {
                        W_USD_Category_Acno = AccNo;
                    }
                    // Find Account no of destination 
                    // To be discuss and create one
                    // W_TXNDST_Category_Acno = "Not Defined Yet";
                }

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }
        }
        //
        // READ ACCOUNT BASED ON User Id
        //
        public void ReadAccountsAndFillTableForUserId(string InOperator, string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ClearDefineTable();

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            // When we put 1000 we want to get the accounts for ATM 
            // If different than 1000 we get the accounts fot the CIT 

            string SqlString = "SELECT * "
                         + " FROM " + WTable
                         + " WHERE Operator = @Operator AND EntityNo = @UserId "
                          + " Order by AccNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);

                            FillTableLine();

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
        // READ ACCOUNT BASED ON Branch Id
        //
        public void ReadAccountsAndFillTableForBranchId(string InOperator, string InBranchId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ClearDefineTable();

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string SqlString = "SELECT * "
                         + " FROM " + WTable
                         + " WHERE Operator = @Operator AND BranchId = @BranchId "
                          + " Order by AccNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@BranchId", InBranchId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);

                            FillTableLine();

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
        // READ ACCOUNT BASED ON Category Id
        //
        public void ReadAccountsAndFillTableForCategoryId(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ClearDefineTable();

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string SqlString = "SELECT * "
                         + " FROM " + WTable 
                         + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
                          + " Order by AccNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);

                            FillTableLine();

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
        // READ ACCOUNT BASED ON ATMNo  
        //
        public void ReadAccountsAndFillTableForBankId(string InOperator, string InBankId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ClearDefineTable();

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            // When we put 1000 we want to get the accounts for ATM 
            // If different than 1000 we get the accounts fot the CIT 

            string SqlString = "SELECT * "
                         + " FROM " + WTable 
                         + " WHERE Operator = @Operator AND BankId = @BankId "
                          + " Order by AccNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);

                            FillTableLine();

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
        // Clear Define table
        private void ClearDefineTable()
        {
            AccountsTable = new DataTable();
            AccountsTable.Clear();

            // DATA TABLE ROWS DEFINITION 
            AccountsTable.Columns.Add("SeqNumber", typeof(int));
            AccountsTable.Columns.Add("Branch", typeof(string));
            AccountsTable.Columns.Add("Short", typeof(string));
            AccountsTable.Columns.Add("AccNo", typeof(string));
            AccountsTable.Columns.Add("CurrNm", typeof(string));
            AccountsTable.Columns.Add("AccName", typeof(string));
            AccountsTable.Columns.Add("AtmNo", typeof(string));
            AccountsTable.Columns.Add("CategoryId", typeof(string));
            AccountsTable.Columns.Add("UserId", typeof(string));
            AccountsTable.Columns.Add("BankId", typeof(string));
            AccountsTable.Columns.Add("Operator", typeof(string));
        }
        // File Table Line 
        private void FillTableLine()
        {
            //
            //FILL IN TABLE
            //

            DataRow RowSelected = AccountsTable.NewRow();

            RowSelected["SeqNumber"] = SeqNumber;
            RowSelected["Branch"] = BranchId;
            RowSelected["Short"] = ShortAccID;
            RowSelected["AccNo"] = AccNo;

            RowSelected["CurrNm"] = CurrNm;
            RowSelected["AccName"] = AccName;
            RowSelected["AtmNo"] = AtmNo;
            RowSelected["CategoryId"] = CategoryId;
            RowSelected["UserId"] = UserId;
            RowSelected["BankId"] = BankId;
            RowSelected["Operator"] = Operator;
            // ADD ROW
            AccountsTable.Rows.Add(RowSelected);
        }


        //
        // READ ACCOUNT BASED ON CRITERIA ONE  
        //
        public void ReadAndFindAccount(string InUserId, string InBranchId, string InCategoryId
            , string InOperator, string InAtmNo, string InCurrNm, string InAccName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            // When we put 1000 we want to get the accounts for ATM 
            // If different than 1000 we get the accounts fot the CIT 
            // If Blank we get the accounts for Branch or CategoryId

            string SqlString = "SELECT *"
               + " FROM " + WTable
               + " WHERE UserId = @UserId "
               + " AND BranchId = @BranchId "
               + " AND CategoryId = @CategoryId "
               + " AND Operator=@Operator AND AtmNo =@AtmNo AND CurrNm=@CurrNm AND AccName = @AccName";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@BranchId", InBranchId);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@CurrNm", InCurrNm);
                        cmd.Parameters.AddWithValue("@AccName", InAccName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            ReadAccountFields(rdr);


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
        // READ ACCOUNT BASED ON CRITERIA TWO With Account Number   
        //
        public void ReadAndFindAccount_AND_Accno(string InUserId, string InBranchId, string InCategoryId
            , string InOperator, string InAtmNo, string InCurrNm, string InAccName, string InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            // When we put 1000 we want to get the accounts for ATM 
            // If different than 1000 we get the accounts fot the CIT 
            // If Blank we get the accounts for Branch or CategoryId

            string SqlString = "SELECT *"
               + " FROM " + WTable
               + " WHERE UserId = @UserId "
               + " AND BranchId = @BranchId "
               + " AND CategoryId = @CategoryId "
               + " AND Operator=@Operator AND AtmNo =@AtmNo "
               + " AND CurrNm=@CurrNm AND AccName = @AccName AND AccNo = @AccNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@BranchId", InBranchId);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@CurrNm", InCurrNm);
                        cmd.Parameters.AddWithValue("@AccName", InAccName);
                        cmd.Parameters.AddWithValue("@AccNo", InAccNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            ReadAccountFields(rdr);


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

        //NOSTRO _ VOSTRO 
        // READ ACCOUNT BASED ON CRITERIA  
        //
        public void ReadAndFindAccountSpecificForNostroVostro(string InAccNo, string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string SqlString = "SELECT *"
               + " FROM " + WTable 
               + " WHERE AccNo=@AccNo AND Operator =@Operator  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadAccountFields(rdr);
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
        // READ ACCOUNT FOT CIT NO 
        //
        public void ReadAndFindAccountForUserId(string InUserId, string InOperator, string InCurrNm, string InAccName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string SqlString = "SELECT *"
               + " FROM " + WTable
               + " WHERE UserId = @UserId AND Operator=@Operator AND CurrNm=@CurrNm AND AccName = @AccName";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CurrNm", InCurrNm);
                        cmd.Parameters.AddWithValue("@AccName", InAccName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            ReadAccountFields(rdr);


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
        // GET External Accounts For a specific External Bank 
        //
        public ArrayList GetExternalAccountsForExternalBank(string InOperator, string InExternalBank)
        {
            //USED ONLY TO DEFINE CATEGORIES IT IS NOT USED FOR OTHER 
            ArrayList ExternalAccountsIdsList = new ArrayList();

            ExternalAccountsIdsList.Add("SelectEntity");

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                + " FROM " + WTable
                + " WHERE Operator = @Operator AND BankId = @BankId ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@BankId", InExternalBank);
                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Fields
                            ReadAccountFields(rdr);

                            ExternalAccountsIdsList.Add(AccNo);
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

            return ExternalAccountsIdsList;
        }

        //
        // READ SPECIFIC Acc SEQ NUMBER 
        //
        public void ReadAndFindAccountBySeqNo(int InSeqNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string SqlString = "SELECT * "
               + " FROM "+ WTable
               + " WHERE SeqNumber = @SeqNumber";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            ReadAccountFields(rdr);


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
        // READ SPECIFIC Acc SEQ NUMBER 
        //
        DataTable AllATMs;

        public void ReadAllATMsAndUpdateAccNo(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            //    bool WInsert = false;

            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            AllATMs = new DataTable();
            AllATMs.Clear();

            string SqlString = "SELECT AtmNo "
              + " FROM [ATMS].[dbo].[ATMsFields] " 
                + " WHERE Operator = @Operator and ATMNo NOT IN ('DBLModelATM','NCRModelATM','WINModelATM')"
                 + " ORDER BY AtmNo ASC ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        sqlAdapt.Fill(AllATMs);

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
            // Update ATM Cash? 
            try
            {

                int I = 0;
                int K = AllATMs.Rows.Count - 1;
                string WAtmNo;

                while (I <= K)
                {

                    RecordFound = true;

                    WAtmNo = (string)AllATMs.Rows[I]["AtmNo"];

                    ReadAccountsAtmCashForAtmNo(InOperator, WAtmNo);
                    //ReadAccountsAndFillTableForAtmNo(InOperator, AtmNo);
                    if (RecordFound == true)
                    {
                        // If Found update the previous by deleting and inserting

                        Mgt.ReadTransFrom_Bulk_Flexube(WAtmNo);


                        if (Mgt.RecordFound == true)
                        {
                            //AccNo = "3333"; 
                            if ((AccNo != Mgt.AtmGLCash) || (BranchId != Mgt.AtmBranch))
                            {

                                // Build the insert
                                AccNo = Mgt.AtmGLCash;
                                BankId = InOperator;
                                BranchId = Mgt.AtmBranch; // 
                                CurrNm = "EGP";
                                ShortAccID = "30";
                                AccName = "ATM Cash";

                                EntityNm = "AtmNo";
                                EntityNo = WAtmNo;

                                AtmNo = WAtmNo;

                                UserId = "";

                                CategoryId = "";

                                AccNoInternal = "";

                                Operator = InOperator;

                                UpdateAccount(SeqNumber, Operator);
                            }
                            // Clear Up table From Previous
                            // Maybe Branch or Account has changed 

                            // Update ATM Branch 
                            Ac.ReadAtm(WAtmNo);

                            Ac.Branch = BranchId;

                            if (Ac.CitId != "1000")
                            {
                                Ac.AtmsReplGroup = Ac.AtmsReconcGroup;
                            }

                            Ac.UpdateATM(WAtmNo);
                        }
                    }
                    else
                    {
                        // Not Found Account in Accounts 
                        string Temp = WAtmNo.Substring(0, 3);

                        Mgt.ReadTransFrom_Bulk_Flexube(WAtmNo);

                        if (Mgt.RecordFound == true)
                        {
                            // DeleteAccountAtmCash(WAtmNo);
                            AccNo = Mgt.AtmGLCash;
                            BankId = InOperator;
                            BranchId = Mgt.AtmBranch; // 
                            CurrNm = "EGP";
                            ShortAccID = "30";
                            AccName = "ATM Cash";

                            EntityNm = "AtmNo";
                            EntityNo = WAtmNo;

                            AtmNo = WAtmNo;

                            UserId = "";

                            CategoryId = "";

                            AccNoInternal = "";

                            Operator = InOperator;

                            InsertAccount();

                            // Update ATM Branch 
                            Ac.ReadAtm(WAtmNo);

                            Ac.Branch = BranchId;

                            if (Ac.CitId != "1000")
                            {
                                Ac.AtmsReplGroup = Ac.AtmsReconcGroup;
                            }

                            Ac.UpdateATM(WAtmNo);
                        }

                    }

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                return;

            }

        }

        public void ReadAllATMsAndUpdateAccNo_T24(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //    bool WInsert = false;

            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            RRDMAtmsClass Ac = new RRDMAtmsClass();

           // WTable = "[ATMS].[dbo].[AccountsTable_T24]";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            AllATMs = new DataTable();
            AllATMs.Clear();

            string SqlString = "SELECT AtmNo "
              + "  FROM [ATMS].[dbo].[ATMsFields] "  
                + " WHERE Operator = @Operator and ATMNo NOT IN ('DBLModelATM','NCRModelATM','WINModelATM')"
                 + " ORDER BY AtmNo ASC ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        sqlAdapt.Fill(AllATMs);

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
            // Update ATM Cash? 
            try
            {

                int I = 0;
                int K = AllATMs.Rows.Count - 1;
                string WAtmNo;

                while (I <= K)
                {

                    RecordFound = true;

                    WAtmNo = (string)AllATMs.Rows[I]["AtmNo"];


                    if (WAtmNo == "00000003")
                    {
                        WAtmNo = "00000003";
                    }


                    // Find if exist in Accounts Table 
                    ReadAccountsAtmCashForAtmNo(InOperator, WAtmNo);
                    //ReadAccountsAndFillTableForAtmNo(InOperator, AtmNo);
                    if (RecordFound == true)
                    {
                        // If Found in accounts update the previous by deleting and inserting

                        Mgt.ReadTransFrom_Bulk_COREBANKING_2(WAtmNo);

                        if (Mgt.RecordFound == true)
                        {
                            //AccNo = "3333"; 
                            if ((AccNo != Mgt.AtmGLCash) || (BranchId != Mgt.AtmBranch))
                            {

                                // Build the insert
                                AccNo = Mgt.AtmGLCash;
                                BankId = InOperator;
                                BranchId = Mgt.AtmBranch; // 
                                CurrNm = "EGP";
                                ShortAccID = "30";
                                AccName = "ATM Cash";

                                EntityNm = "AtmNo";
                                EntityNo = WAtmNo;

                                AtmNo = WAtmNo;

                                UserId = "";

                                CategoryId = "";

                                AccNoInternal = "";

                                Operator = InOperator;

                                UpdateAccount(SeqNumber, Operator);
                            }
                            // Clear Up table From Previous
                            // Maybe Branch or Account has changed 

                            // Update ATM Branch 
                            Ac.ReadAtm(WAtmNo);

                            Ac.Branch = BranchId;

                            if (Ac.CitId != "1000")
                            {
                                Ac.AtmsReplGroup = Ac.AtmsReconcGroup;
                            }

                            Ac.UpdateATM(WAtmNo);
                        }
                    }
                    else
                    {
                        // Not Found Account in Accounts 

                        // We are T24 case


                        Mgt.ReadTransFrom_Bulk_COREBANKING_2(WAtmNo);


                        if (Mgt.RecordFound == true)
                        {
                            // DeleteAccountAtmCash(WAtmNo);
                            AccNo = Mgt.AtmGLCash;
                            BankId = InOperator;
                            BranchId = Mgt.AtmBranch; // 
                            CurrNm = "EGP";
                            ShortAccID = "30";
                            AccName = "ATM Cash";

                            EntityNm = "AtmNo";
                            EntityNo = WAtmNo;

                            AtmNo = WAtmNo;

                            UserId = "";

                            CategoryId = "";

                            AccNoInternal = "";

                            Operator = InOperator;

                            InsertAccount();

                            // Update ATM Branch 
                            Ac.ReadAtm(WAtmNo);

                            Ac.Branch = BranchId;

                            if (Ac.CitId != "1000")
                            {
                                Ac.AtmsReplGroup = Ac.AtmsReconcGroup;
                            }

                            Ac.UpdateATM(WAtmNo);
                        }

                    }

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                return;

            }

        }
        //
        // GET ACCOUNT NUMBERS FOR AUDI
        //
        public void ReadAllATMsAndUpdateAccNo_AUDI(string InOperator, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //    bool WInsert = false;

            //RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            RRDMGL_Balances_Atms_Daily_AUDI Gl = new RRDMGL_Balances_Atms_Daily_AUDI();

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            //ReadGL_Balances_Atms_Daily_ATM_CUT_OFF(string InAtmNo, DateTime InCut_Off_Date);

            Gl.ReadGL_Balances_Atms_Daily_AUDI_Table("", InCut_Off_Date, 2);

            try
            {

                int I = 0;
                int K = Gl.TableGL_Balances_Atms_Daily_AUDI.Rows.Count - 1;
                string WAtmNo;

                while (I <= K)
                {

                    int SeqNo = (int)Gl.TableGL_Balances_Atms_Daily_AUDI.Rows[I]["SeqNo"];

                    Gl.ReadGL_Balances_Atms_DailyBySeqNo(SeqNo);

                    WAtmNo = Gl.AtmNo;
                    //string ATMCash = Gl.GL_Acc_ATM_Cash; // Name:"ATM Cash"
                    //string ATMInter = Gl.GL_Acc_ATM_Inter; // Name:"ATM Intermediate"
                    //string ATMExcess = Gl.GL_Acc_ATM_Excess; // Name:"ATM Excess"
                    //string ATMShort = Gl.GL_Acc_ATM_Short; // Name:"ATM Shortage"

                    //**************************************
                    // WAccName = "ATM Cash";
                    //**************************************
                    string WShortAccID = "30";
                    string WAccName = "ATM Cash";
                    string WAccNo = Gl.GL_Acc_ATM_Cash;

                    ReadAccountsAtmCashForAtmNoAndShortAccId(WAtmNo, WShortAccID);
                    if (RecordFound == true)
                    {
                        if (WAccNo != AccNo)
                        {
                            // Update New Account
                            AccNo = WAccNo;
                            UpdateAccount(SeqNumber, InOperator);
                        }
                    }
                    else
                    {
                        PrepareAndInsert(InOperator, WAtmNo, WShortAccID, WAccName, WAccNo);
                    }
                    //**************************************

                    //**************************************
                    // WAccName = "ATM Intermediate"
                    //**************************************
                    WShortAccID = "31";
                    WAccName = "ATM Intermediate";
                    WAccNo = Gl.GL_Acc_ATM_Inter;

                    ReadAccountsAtmCashForAtmNoAndShortAccId(WAtmNo, WShortAccID);
                    if (RecordFound == true)
                    {
                        if (WAccNo != AccNo)
                        {
                            // Update New Account
                            AccNo = WAccNo;
                            UpdateAccount(SeqNumber, InOperator);
                        }
                    }
                    else
                    {
                        PrepareAndInsert(InOperator, WAtmNo, WShortAccID, WAccName, WAccNo);
                    }
                    //**************************************

                    //**************************************
                    // WAccName = "ATM Excess"
                    //**************************************
                    WShortAccID = "32";
                    WAccName = "ATM Excess";
                    WAccNo = Gl.GL_Acc_ATM_Excess;

                    ReadAccountsAtmCashForAtmNoAndShortAccId(WAtmNo, WShortAccID);
                    if (RecordFound == true)
                    {
                        if (WAccNo != AccNo)
                        {
                            // Update New Account
                            AccNo = WAccNo;
                            UpdateAccount(SeqNumber, InOperator);
                        }
                    }
                    else
                    {
                        PrepareAndInsert(InOperator, WAtmNo, WShortAccID, WAccName, WAccNo);
                    }
                    //**************************************

                    //**************************************
                    // WAccName = "ATM Shortage"
                    //**************************************
                    WShortAccID = "33";
                    WAccName = "ATM Shortage";
                    WAccNo = Gl.GL_Acc_ATM_Short;

                    ReadAccountsAtmCashForAtmNoAndShortAccId(WAtmNo, WShortAccID);
                    if (RecordFound == true)
                    {
                        if (WAccNo != AccNo)
                        {
                            // Update New Account
                            AccNo = WAccNo;
                            UpdateAccount(SeqNumber, InOperator);
                        }
                    }
                    else
                    {
                        PrepareAndInsert(InOperator, WAtmNo, WShortAccID, WAccName, WAccNo);
                    }
                    //**************************************

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                return;

            }

        }
        //
        // Prepare and Insert 
        //
        private void PrepareAndInsert(string InOperator, string InAtmNo, string InShortAccID, string InAccName, string InAccNo)
        {
            // Insert 
            //
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            // Build the insert
            Ac.ReadAtm(InAtmNo);
            if (Ac.RecordFound == true)
            {
                BranchId = Ac.Branch; // 
            }
            else
            {
                BranchId = "Branch for.." + InAtmNo;
            }

            BankId = InOperator;

            CurrNm = "EGP";
            ShortAccID = InShortAccID;
            AccName = InAccName;

            AccNo = InAccNo;

            EntityNm = "AtmNo";
            EntityNo = InAtmNo;

            AtmNo = InAtmNo;

            UserId = "";

            CategoryId = "";

            AccNoInternal = "";

            Operator = InOperator;

            InsertAccount();
        }
        // Insert Acc no
        //
        public void InsertAccount()
        {

            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string cmdinsert = "INSERT INTO " + WTable
                    + " ([AccNo], [BankId],  "
                    + " [CurrNm], [ShortAccID], [AccName],"
                     + " [EntityNm], [EntityNo], "
                     + " [AtmNo], [UserId], "
                      + " [BranchId], [CategoryId], "
                     + " [AccNoInternal], [Operator] )"
                    + " VALUES (@AccNo, @BankId,"
                    + "@CurrNm, @ShortAccID,@AccName, "
                    + " @EntityNm, @EntityNo,"
                     + " @AtmNo, @UserId,"
                     + " @BranchId, @CategoryId,"
                    + " @AccNoInternal, @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@CurrNm", CurrNm);

                        cmd.Parameters.AddWithValue("@ShortAccID", ShortAccID);

                        cmd.Parameters.AddWithValue("@AccName", AccName);

                        cmd.Parameters.AddWithValue("@EntityNm", EntityNm);

                        cmd.Parameters.AddWithValue("@EntityNo", EntityNo);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                        cmd.Parameters.AddWithValue("@AccNoInternal", AccNoInternal);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

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

        // UPDATE 
        // 
        public void UpdateAccount(int InSeqNumber, string InOperator)
        {
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTable +" SET "
                            + " AccNo = @AccNo, BankId = @BankId, "
                            + " CurrNm = @CurrNm, ShortAccID = @ShortAccID, AccName = @AccName,"
                            + " EntityNm = @EntityNm, EntityNo = @EntityNo, "
                             + " AtmNo = @AtmNo, UserId = @UserId, "
                            + " BranchId = @BranchId, CategoryId = @CategoryId, "
                            + " AccNoInternal = @AccNoInternal"
                            + " WHERE SeqNumber = @SeqNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@CurrNm", CurrNm);
                        cmd.Parameters.AddWithValue("@ShortAccID", ShortAccID);
                        cmd.Parameters.AddWithValue("@AccName", AccName);

                        cmd.Parameters.AddWithValue("@EntityNm", EntityNm);
                        cmd.Parameters.AddWithValue("@EntityNo", EntityNo);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                        cmd.Parameters.AddWithValue("@AccNoInternal", AccNoInternal);

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
        // DELETE AN ACCOUNT  
        //
        public void DeleteAccount(int InSeqNumber)
        {
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + WTable
                            + " WHERE SeqNumber =  @SeqNumber ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

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
        // DELETE AN ACCOUNT for ATM Cash 
        //
        public void DeleteAccountAtmCash(string InAtmNo)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int Count;

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + WTable
                            + " WHERE AtmNo = @AtmNo AND AccName = 'ATM Cash' ", conn))

                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        Count = cmd.ExecuteNonQuery();

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
        // DELETE AN ACCOUNT for ATM and Short Acc Id 
        //
        public void DeleteAccountAtm_ShortAccId(string InAtmNo, string InShortAccID)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int Count;

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + WTable
                            + " WHERE AtmNo = @AtmNo AND ShortAccID = @ShortAccID ", conn))

                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ShortAccID", InShortAccID);

                        Count = cmd.ExecuteNonQuery();

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
        // COPY ALL ACCOUNTS FROM ONE ATM to another   
        //
        public void CopyAccountsAtmToAtm(string InOperatorA, string InAtmA, String TargetOperatorB, string InAtmB)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ParId = "950";
            OccurId = "1";
            //TEST
            Gp.ReadParametersSpecificIdNoOperator(ParId, OccurId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // IT is T24 Version
                    WTable = "[ATMS].[dbo].[AccountsTable_T24]";
                }
                else
                {
                    WTable = "[ATMS].[dbo].[AccountsTable]";
                }

            }

            string InSelectionCriteria = " WHERE Operator ='" + InOperatorA + "' AND  AtmNo='" + InAtmA + "'";

            AccountsForAtmDataTable = new DataTable();
            AccountsForAtmDataTable.Clear();

            string SqlString = "SELECT *"
                      + " FROM " + WTable + " "
                      + InSelectionCriteria;
            //+ " WHERE Operator = @Operator AND AtmNo = @AtmNo";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(AccountsForAtmDataTable);

                    }
                    // Close conn
                    conn.Close();

                    int I = 0;

                    while (I <= (AccountsForAtmDataTable.Rows.Count - 1))
                    {

                        RecordFound = true;

                        // GET Table fields - Line by Line
                        //
                        SeqNumber = (int)AccountsForAtmDataTable.Rows[I]["SeqNumber"];

                        AccNo = (string)AccountsForAtmDataTable.Rows[I]["AccNo"];
                        BankId = (string)AccountsForAtmDataTable.Rows[I]["BankId"];

                        ShortAccID = (string)AccountsForAtmDataTable.Rows[I]["ShortAccID"];
                        EntityNm = (string)AccountsForAtmDataTable.Rows[I]["EntityNm"];
                        EntityNo = (string)AccountsForAtmDataTable.Rows[I]["EntityNo"];

                        CurrNm = (string)AccountsForAtmDataTable.Rows[I]["CurrNm"];
                        AccName = (string)AccountsForAtmDataTable.Rows[I]["AccName"];
                        AtmNo = (string)AccountsForAtmDataTable.Rows[I]["AtmNo"];

                        UserId = (string)AccountsForAtmDataTable.Rows[I]["UserId"];
                        //ShortAccID = "30";

                        //EntityNm = "AtmNo";
                        //EntityNo = AtmNo;
                        BranchId = "";
                        CategoryId = "";
                        AccNoInternal = "";

                        AtmNo = InAtmB;
                        Operator = TargetOperatorB;
                        InsertAccount();

                        I++; // Read Next entry of the table 
                    }
                }

                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }


        }

    }
}

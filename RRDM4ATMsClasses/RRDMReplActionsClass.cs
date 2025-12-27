using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using RRDM4ATMs;

namespace RRDM4ATMs
{
    public class RRDMReplActionsClass
    {
        // Variables
        // Repl ACtion FIELDS 
        public int ReplActNo; 
        public int ReplActId;
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
        public decimal NewAmount;

        public int CassetteOneNotes; // Isurance amount for one day
        public int CassetteTwoNotes;
        public int CassetteThreeNotes;
        public int CassetteFourNotes;

        public int AtmsStatsGroup;
        public int AtmsReplGroup;
        public int AtmsReconcGroup;
        public DateTime DateInsert;

        public string AuthUser;

        public string OwnerUser; 

        public string CitId;

        public bool AuthorisedRecord; 

        public bool ActiveRecord;

        public bool PassReplCycle;
        public DateTime PassReplCycleDate;
        public decimal CashInAmount; // Cash that says in Repl Cycle 
        public decimal InMoneyReal; // Cash in ATM registers 

        public string InactivateComment; 
        
        public string Operator; 

  //      StringBuilder CitString;

        public string PublicCitString; 

      //  string SqlString; // Do not delete

        //    int Function;

        //     int I;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        string WhatFile = "[ATMS].[dbo].[ReplActionsTable]";

        decimal TotNewAmount;

        // Define the data table 
        public DataTable TableReplActions = new DataTable();

        public int TotalSelected;

        string SqlString; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;        

        // 
    // Classes 
        // 

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMAccountsClass Acc = new RRDMAccountsClass(); 
        RRDMPostedTrans Cs = new RRDMPostedTrans();
        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 

        //     ReplDatesCalc Rc = new ReplDatesCalc(); // Locate next Replenishment 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

    //    string outcome = ""; // TO FACILITATE EXCEPTIONS 
        // Methods 
        // READ ReplActionsTable 
        // 
        public void ReadReplActionsAndFillTable(string InOperator, string InFilter,
                                                   DateTime InFromDt, DateTime InToDt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReplActions = new DataTable();
            TableReplActions.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableReplActions.Columns.Add("ActNo", typeof(int));
            TableReplActions.Columns.Add("ActId", typeof(int));
            TableReplActions.Columns.Add("AtmNo", typeof(string));
            TableReplActions.Columns.Add("CycleNo", typeof(int));
            TableReplActions.Columns.Add("AmountWas", typeof(string));
            TableReplActions.Columns.Add("LastReplDt", typeof(DateTime));
            TableReplActions.Columns.Add("NewEstReplDt", typeof(DateTime));
            TableReplActions.Columns.Add("PassReplCycleDate", typeof(DateTime));
            TableReplActions.Columns.Add("EstAmount", typeof(string));
            TableReplActions.Columns.Add("InMoneyReal", typeof(string));

            if (InFromDt == NullPastDate)
            {
                SqlString = "SELECT *"
                  + " FROM " + WhatFile
                  + " WHERE " + InFilter 
                  + " ORDER BY ReplActNo DESC ";     
            }
            else
            {
                SqlString = "SELECT *"
                  + " FROM " + WhatFile
                  + " WHERE " + InFilter + " AND (DateInsert >= @FromDt AND DateInsert <= @ToDt) "
                  + " ORDER BY ReplActNo DESC ";     

            }
            

          //  SqlString = "SELECT *"
          //+ " FROM [dbo].[ReplActionsTable] "
          //+ " WHERE AtmNo=@AtmNo AND ReplActNo=@ReplActNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        if (InFromDt != NullPastDate)
                        {
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }
                     
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReplActNo = (int)rdr["ReplActNo"];

                            ReplActId = (int)rdr["ReplActId"];

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
                            NewAmount = (decimal)rdr["NewAmount"];

                            CassetteOneNotes = (int)rdr["CassetteOneNotes"];
                            CassetteTwoNotes = (int)rdr["CassetteTwoNotes"];
                            CassetteThreeNotes = (int)rdr["CassetteThreeNotes"];
                            CassetteFourNotes = (int)rdr["CassetteFourNotes"];

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


                            // Fill Table 

                            DataRow RowSelected = TableReplActions.NewRow();

                            RowSelected["ActNo"] = ReplActNo;
                            RowSelected["ActId"] = ReplActId;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["CycleNo"] = ReplCycleNo;
                            RowSelected["AmountWas"] = CurrCassettes.ToString("#,##0.00");
                            RowSelected["LastReplDt"] = LastReplDt;
                            RowSelected["NewEstReplDt"] = NewEstReplDt;
                            RowSelected["PassReplCycleDate"] = PassReplCycleDate;
                            RowSelected["EstAmount"] = NewAmount.ToString("#,##0.00");
                            RowSelected["InMoneyReal"] = InMoneyReal.ToString("#,##0.00"); 

                            // ADD ROW
                            TableReplActions.Rows.Add(RowSelected);
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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

                }
        }

        // Methods 
        // READ ReplActionsTable 
        // 
        public void ReadReplActionsForAtm(string InAtmNo, int InReplActNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
          + " FROM [dbo].[ReplActionsTable] "
          + " WHERE AtmNo=@AtmNo AND ReplActNo=@ReplActNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplActNo", InReplActNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true; 

                            ReplActNo = (int)rdr["ReplActNo"];

                            ReplActId = (int)rdr["ReplActId"];

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
                            NewAmount = (decimal)rdr["NewAmount"];

                            CassetteOneNotes = (int)rdr["CassetteOneNotes"];
                            CassetteTwoNotes = (int)rdr["CassetteTwoNotes"];
                            CassetteThreeNotes = (int)rdr["CassetteThreeNotes"];
                            CassetteFourNotes = (int)rdr["CassetteFourNotes"];

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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

                }
        }

        // Methods 
        // READ ReplActionsTable 
        // 
        public void ReadReplActionsForAtmReplCycleNo(string InAtmNo, int InReplCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ReplActionsTable] "
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

                            ReplActNo = (int)rdr["ReplActNo"];

                            ReplActId = (int)rdr["ReplActId"];

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
                            NewAmount = (decimal)rdr["NewAmount"];

                            CassetteOneNotes = (int)rdr["CassetteOneNotes"];
                            CassetteTwoNotes = (int)rdr["CassetteTwoNotes"];
                            CassetteThreeNotes = (int)rdr["CassetteThreeNotes"];
                            CassetteFourNotes = (int)rdr["CassetteFourNotes"];

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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

                }
        }

        // Methods 
        // READ ReplActionsTable 
        // 
        public void ReadReplActionsSpecific(int InReplActNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
                + " FROM [dbo].[ReplActionsTable] "
                + " WHERE ReplActNo=@ReplActNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@ReplActNo", InReplActNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReplActNo = (int)rdr["ReplActNo"];

                            ReplActId = (int)rdr["ReplActId"];

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
                            NewAmount = (decimal)rdr["NewAmount"];

                            CassetteOneNotes = (int)rdr["CassetteOneNotes"];
                            CassetteTwoNotes = (int)rdr["CassetteTwoNotes"];
                            CassetteThreeNotes = (int)rdr["CassetteThreeNotes"];
                            CassetteFourNotes = (int)rdr["CassetteFourNotes"];

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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

                }
        }

        // Methods 
        // READ TO FIND LAST  
        // 
        public void ReadReplActionsForLast(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
                + " FROM [dbo].[ReplActionsTable] "
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

                            ReplActNo = (int)rdr["ReplActNo"];

                            ReplActId = (int)rdr["ReplActId"];

                            ReplCycleNo = (int)rdr["ReplCycleNo"];    

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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

                }
        }

        //  
        // READ all ATMs for Action for this CIT Provider
        // Create a String throught a the StringBuilder
        // Insert transaction and Update 
        public void ReadReplActionsForCITAndUpdate(string InCitId, string InBankId, string InUser, int InFunction)
        {
            // If IN Function is 1 then just prepare string but not update or insert. 
            // If IN Function is 2 then make update and insert. 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            StringBuilder CitString = new StringBuilder();

            CitString.Append("****" + InBankId + "****TO****" + InCitId + "********************").AppendLine();
            CitString.AppendLine();

            CitString.Append("DATE: " + DateTime.Now.ToShortDateString()).Append("  BANK: " + InBankId).
                Append("  UserId: " + InUser).AppendLine();

            CitString.AppendLine();

            CitString.Append("REPLENISHMENT ORDERS FOR CIT PROVIDER : " + InCitId).AppendLine().AppendLine();
            //*************************************************
            //***************************************************
            // SELECT All REPL ACTIONS FOR THIS CIT AND FOR EACH RECORD  MAKE TRANS TO BE POSTED
            //***************************************************
            //*************************************************
            SqlString = "SELECT *"
                + " FROM [dbo].[ReplActionsTable] "
                + " WHERE CitId=@CitId AND AuthorisedRecord=0 AND ActiveRecord = 1";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CitId", InCitId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true; 

                            ReplActNo = (int)rdr["ReplActNo"];

                            ReplActId = (int)rdr["ReplActId"];

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
                            NewAmount = (decimal)rdr["NewAmount"];

                            CassetteOneNotes = (int)rdr["CassetteOneNotes"];
                            CassetteTwoNotes = (int)rdr["CassetteTwoNotes"];
                            CassetteThreeNotes = (int)rdr["CassetteThreeNotes"];
                            CassetteFourNotes = (int)rdr["CassetteFourNotes"];

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

                            cmd.Parameters.AddWithValue("@InactivateComment", InactivateComment);

                            Operator = (string)rdr["Operator"]; 
                           
                            TotNewAmount = TotNewAmount + NewAmount;

                            Ac.ReadAtm(AtmNo); // Read information of face values 

                            //if (InFunction == 2 )  // Create Transactions and Update ReplAction Record 
                            if (InFunction == 2)  // Create Transactions to be posted and Update ReplAction Record 
                            {
                                if (CitId != "1000") // NOT EQUAL 
                                {
                                  
                                    // Create transactions to be posted related with CIT account 
                                    // CR Bank Cash Account 
                                    // And DR CIT CASH Account 


                                    Tp.OriginId = "08"; // *
                                    Tp.OriginName = "To CIT-OurATMs";  // Money sent to CIT
                                    Tp.RMCateg = "N/A";
                                    Tp.RMCategCycle = 0;
                                    Tp.MaskRecordId = 0;

                                    Tp.ErrNo = 0 ;
                                    Tp.AtmNo = AtmNo;
                                    Tp.SesNo = ReplCycleNo;
                                    Tp.BankId = BankId;

                                    Tp.AtmTraceNo = 0 ;

                                    Tp.BranchId = "Controller's Branch" ;
                                    Tp.AtmDtTime = DateTime.Now;
                                    //Tp.HostDtTime = DateTime.Now; 
                                    Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                                    Tp.CardNo = "N/A";
                                    Tp.CardOrigin = 5; // Find OUT

                                    // First Entry 
                                    Tp.TransType = 21; // CR BANK CASH 
                                    Tp.TransDesc = "BANK TO CIT : " + CitId + " For ATM: " + AtmNo;

                                    string WUser = "1000" ; 
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

                                    WUser = CitId ; 
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
                                    Tp.AuthCode = 0 ;
                                    Tp.RefNumb = 0 ;
                                    Tp.RemNo = 0 ;
                                    Tp.TransMsg = "Not available";
                                    Tp.AtmMsg = "";
                                    Tp.OpenDate = DateTime.Now;
                                    Tp.OpenRecord = true;

                                    Tp.AuthUser = AuthUser;  

                                    Tp.Operator = Ac.Operator;

                                    //****************************
                                    // INSERT ********************
                                    //****************************
                                    Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo,Tp.OriginName);
                                   
                                }

                                // UPDATE REPL ACTIONS TABLE 
                                AuthorisedDate = DateTime.Now; 

                                AuthorisedRecord = true;

                                UpdateReplActionsForAtm(AtmNo, ReplActNo);
                                //UpdateReplActionsInactive(AtmNo, ReplActNo, ActiveRecord); 
                            }
                            

                            // Add To CitString through StringBuilder

                            CitString.Append("_______________" + AtmNo + "_________________________-").AppendLine();
                            CitString.AppendLine();

                            CitString.Append("Order for ATM : " + AtmNo).AppendLine();


                            CitString.Append("Replenishment Date not later than :" + NewEstReplDt.ToShortDateString()).AppendLine().AppendLine();

                            CitString.Append("Replenishment Amount:" + NewAmount.ToString("#,##0.00")).Append(" CurrNm:" + CurrNm).AppendLine().AppendLine();

                            CitString.Append("    Cassette 1 :" + CassetteOneNotes.ToString()).Append(" Face Value:" + Ac.FaceValue_11).AppendLine();
                            CitString.Append("    Cassette 2 :" + CassetteTwoNotes.ToString()).Append(" Face Value:" + Ac.FaceValue_12).AppendLine();
                            CitString.Append("    Cassette 3 :" + CassetteThreeNotes.ToString()).Append(" Face Value:" + Ac.FaceValue_13).AppendLine();
                            CitString.Append("    Cassette 4 :" + CassetteFourNotes.ToString()).Append(" Face Value:" + Ac.FaceValue_14).AppendLine();
                            CitString.Append("_____________________________________________").AppendLine();

                            CitString.AppendLine();

                        }

                        CitString.Append("___YOUR "+CurrNm+" CASH ACCOUNT IS CREDITED__________");
                        CitString.AppendLine();
                        CitString.AppendLine();
                        CitString.Append("_______________ONE ENTRY PER ATM_____________________");
                        CitString.AppendLine();

                        CitString.Append("Total Amount :" + TotNewAmount.ToString("#,##0.00")).AppendLine();
                        CitString.AppendLine();
                        CitString.AppendLine();

                        CitString.Append("**************END OF EMAIL OR REPORT *************").AppendLine();

                        PublicCitString = CitString.ToString();

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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

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
                 + " FROM [dbo].[ReplActionsTable] "
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
                            ReplActNo = (int)rdr["ReplActNo"];

                            ReplActId = (int)rdr["ReplActId"];

                            ReadReplActionsForAtm(InAtmNo, ReplActNo);

                            if (Action ==1) AuthUser = InUser;
                            if (Action == 2) AuthUser = "" ; 

                            UpdateReplActionsForAtm(InAtmNo, ReplActNo); 

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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

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
                  + " FROM [dbo].[ReplActionsTable] "
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
                            ReplActNo = (int)rdr["ReplActNo"];

                            ReplActId = (int)rdr["ReplActId"];

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
                            NewAmount = (decimal)rdr["NewAmount"];

                            CassetteOneNotes = (int)rdr["CassetteOneNotes"];
                            CassetteTwoNotes = (int)rdr["CassetteTwoNotes"];
                            CassetteThreeNotes = (int)rdr["CassetteThreeNotes"];
                            CassetteFourNotes = (int)rdr["CassetteFourNotes"];

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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

                }
        }
        // 
        // UPDATE ReplActionsTable
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
                        new SqlCommand("UPDATE ReplActionsTable SET "
                            + "[ReplActId]=@ReplActId,[AuthorisedDate]=@AuthorisedDate,"
                            + "[AtmNo] =@AtmNo,[AtmName]=@AtmName,[ReplCycleNo]=@ReplCycleNo,"
                            + "[BankId] = @BankId,[RespBranch] = @RespBranch,[BranchName] = @BranchName,"
                            + " [OffSite] = @OffSite,[LastReplDt] = @LastReplDt, "
                            + " [TypeOfRepl] = @TypeOfRepl,[OverEst] = @OverEst,[NextReplDt] = @NextReplDt, "
                            + " [CurrNm] = @CurrNm,"
                            + " [MinCash] = @MinCash,[MaxCash] = @MaxCash,"
                            + " [ReplAlertDays] = @ReplAlertDays,"   
                             + "[ReconcDiff] = @ReconcDiff,[MoreMaxCash] = @MoreMaxCash,[LessMinCash] = @LessMinCash,[NeedType] = @NeedType,"
                              + "[CurrCassettes] = @CurrCassettes,[CurrentDeposits] = @CurrentDeposits,[EstReplDt] = @EstReplDt,"
                              + "[NewEstReplDt] = @NewEstReplDt,[NewAmount] = @NewAmount,[CassetteOneNotes] = @CassetteOneNotes,"
                              + "[CassetteTwoNotes] = @CassetteTwoNotes,[CassetteThreeNotes] = @CassetteThreeNotes,[CassetteFourNotes] = @CassetteFourNotes,"
                            + "[AtmsStatsGroup] = @AtmsStatsGroup,[AtmsReplGroup] = @AtmsReplGroup,[AtmsReconcGroup] = @AtmsReconcGroup,"
                            + "[DateInsert] = @DateInsert,[AuthUser] = @AuthUser,[OwnerUser] = @OwnerUser,[CitId] = @CitId,"
                            + " [AuthorisedRecord] = @AuthorisedRecord,[ActiveRecord] = @ActiveRecord,"
                            + " [PassReplCycle] = @PassReplCycle, [PassReplCycleDate] = @PassReplCycleDate, [CashInAmount] = @CashInAmount, [InMoneyReal] = @InMoneyReal, [InactivateComment] = @InactivateComment "
                            + " WHERE AtmNo= @AtmNo AND ReplActNo = @ReplActNo", conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplActNo", InReplActionNo);
                        
                        cmd.Parameters.AddWithValue("@ReplActId", ReplActId);
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
                        cmd.Parameters.AddWithValue("@EstReplDt", EstReplDt);

                        cmd.Parameters.AddWithValue("@NewEstReplDt", NewEstReplDt);
                        cmd.Parameters.AddWithValue("@NewAmount", NewAmount);

                        cmd.Parameters.AddWithValue("@CassetteOneNotes", CassetteOneNotes);
                        cmd.Parameters.AddWithValue("@CassetteTwoNotes", CassetteTwoNotes);
                        cmd.Parameters.AddWithValue("@CassetteThreeNotes", CassetteThreeNotes);
                        cmd.Parameters.AddWithValue("@CassetteFourNotes", CassetteFourNotes);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;
                }

        }

        //// 
        //// UPDATE De- activate or Activate ACtions record 
        ////
        //public void UpdateReplActionsInactive(string InAtmNo, int InReplActionNo, bool InActiveRecord)
        //{
           
        //    ErrorFound = false;
        //    ErrorOutput = ""; 

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand("UPDATE ReplActionsTable SET "
        //                    + "[ActiveRecord] = @ActiveRecord"
        //                    + " WHERE AtmNo= @AtmNo AND ReplActNo = @ReplActNo", conn))
        //            {
        //                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
        //                cmd.Parameters.AddWithValue("@ReplActNo", InReplActionNo);
        //                cmd.Parameters.AddWithValue("@ActiveRecord", InActiveRecord);

        //                // Execute and check success 
        //                int rows = cmd.ExecuteNonQuery();
        //                if (rows > 0)
        //                {
        //                   // outcome = " ATMs Table UPDATED ";
        //                }

        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            ErrorFound = true;
        //            ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;
        //        }

        //}
        // INSERT New Record in ReplActionsTable
        public void InsertReplActionsTable(string InAtmNo)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [ReplActionsTable]"
                + " ([ReplActId],"
                + "[AtmNo],[AtmName],[ReplCycleNo],"
                + "[BankId],[RespBranch],[BranchName],"
                + "[OffSite],[LastReplDt],[TypeOfRepl],[OverEst],[NextReplDt],"
                 + "[CurrNm],"
                + "[MinCash],[MaxCash],[ReplAlertDays],"
                 + "[ReconcDiff],[MoreMaxCash],[LessMinCash],[NeedType],"
                  + "[CurrCassettes],[CurrentDeposits],[EstReplDt],"
                  + "[NewEstReplDt],[NewAmount],"
                   + "[CassetteOneNotes],[CassetteTwoNotes],[CassetteThreeNotes],[CassetteFourNotes],"
                + "[AtmsStatsGroup],[AtmsReplGroup],[AtmsReconcGroup],[DateInsert],[AuthUser],[OwnerUser],"
                + "[CitId],[ActiveRecord],[Operator])"
                + " VALUES "
                + " (@ReplActId,"
                + "@AtmNo,@AtmName,@ReplCycleNo,"
                + "@BankId, @RespBranch,@BranchName,"
                + "@OffSite,@LastReplDt,@TypeOfRepl,@OverEst,@NextReplDt,"
                 + "@CurrNm,"
                + "@MinCash,@MaxCash,@ReplAlertDays,"
                 + "@ReconcDiff,@MoreMaxCash, @LessMinCash,@NeedType,"
                  + "@CurrCassettes,@CurrentDeposits,@EstReplDt,"
                  + "@NewEstReplDt,@NewAmount,"
                  + "@CassetteOneNotes,@CassetteTwoNotes,@CassetteThreeNotes,@CassetteFourNotes,"
                + "@AtmsStatsGroup,@AtmsReplGroup,@AtmsReconcGroup,@DateInsert,@AuthUser,@OwnerUser,"
                + "@CitId,@ActiveRecord,@Operator)";
           
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReplActId", ReplActId);
                        //cmd.Parameters.AddWithValue("@AuthorisedDate", AuthorisedDate);

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

                        cmd.Parameters.AddWithValue("@NewEstReplDt",NewEstReplDt);
                        cmd.Parameters.AddWithValue("@NewAmount", NewAmount);

                        cmd.Parameters.AddWithValue("@CassetteOneNotes",CassetteOneNotes);
                        cmd.Parameters.AddWithValue("@CassetteTwoNotes", CassetteTwoNotes);
                        cmd.Parameters.AddWithValue("@CassetteThreeNotes", CassetteThreeNotes);
                        cmd.Parameters.AddWithValue("@CassetteFourNotes", CassetteFourNotes);

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
                    ErrorOutput = "An error occured in Repl Actions Class............. " + ex.Message;

                }
        }
    }
}



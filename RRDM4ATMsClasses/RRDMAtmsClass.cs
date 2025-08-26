using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMAtmsClass : Logger
    {
        public RRDMAtmsClass() : base() { }
        //public class RRDMGroups : Logger
        //{
        //    public RRDMGroups() : base() { }

        public string AtmNo;

        public string AtmName;
        public string BankId;
        public string Branch; // Branch Identity 
        public string BranchName;

        public string Street;
        public string Town;
        public string District;
        public string PostalCode;
        public string Country;

        public double Latitude;
        public double Longitude;

        public int AtmsStatsGroup;
        public int AtmsReplGroup;
        public int AtmsReconcGroup;

        public bool Loby;
        public bool Wall;
        public bool Drive;
        public bool OffSite;

        public string TypeOfRepl;
        public string CashInType;
        public string HolidaysVersion;
        public string MatchDatesCateg;

        public int OverEst;
        public decimal MinCash;
        public decimal MaxCash;
        public int ReplAlertDays;

        public decimal InsurOne;
        public decimal InsurTwo;
        public decimal InsurThree;
        public decimal InsurFour;

        public string Supplier;
        public string Model;

        public string TerminalType;

        public string EjournalTypeId;

        public string CitId;

        public int NoCassettes;

        public bool DepoReader;
        public bool DepoRecycling;

        public bool ChequeReader;
        public bool EnvelopDepos;
        public string DepCurNm;

        public int CasNo_11;
        public string CurName_11;
        public int FaceValue_11;
        public int CasCapacity_11;

        public int CasNo_12;
        public string CurName_12;
        public int FaceValue_12;
        public int CasCapacity_12;

        public int CasNo_13;
        public string CurName_13;
        public int FaceValue_13;
        public int CasCapacity_13;

        public int CasNo_14;
        public string CurName_14;
        public int FaceValue_14;
        public int CasCapacity_14;

        public int CasNo_15;
        public string CurName_15;
        public int FaceValue_15;
        public int CasCapacity_15;

        public bool ActiveAtm;
        public string Operator;

        /// <summary>
        /// 
        /// </summary>
        /// 

        // Define the data table 
        public DataTable ATMsDetailsDataTable = new DataTable();

        public DataTable TableATMsVsGroups = new DataTable();

        public int TotalSelected;

        /// <summary>
        /// 
        /// </summary>

        public string AtmReplUserId;
        public string AtmReplUserName;
        public string AtmReplUserEmail;
        public string AtmReplMobileNo;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        //
        // Read Fields 
        // 
        private void ATMSReaderFields(SqlDataReader rdr)
        {

            AtmNo = (string)rdr["AtmNo"];
            AtmName = (string)rdr["AtmName"];
            BankId = (string)rdr["BankId"];

            Branch = (string)rdr["Branch"];
            BranchName = (string)rdr["BranchName"];

            Street = (string)rdr["Street"];
            Town = (string)rdr["Town"];
            District = (string)rdr["District"];

            PostalCode = (string)rdr["PostalCode"];
            Country = (string)rdr["Country"];

            Latitude = (double)rdr["Latitude"];
            Longitude = (double)rdr["Longitude"];

            //LocationScreenshot = (byte[])rdr["LocationImage"];

            AtmsStatsGroup = (int)rdr["AtmsStatsGroup"];
            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];

            Loby = (bool)rdr["Loby"];
            Wall = (bool)rdr["Wall"];
            Drive = (bool)rdr["Drive"];

            OffSite = (bool)rdr["OffSite"];

            TypeOfRepl = (string)rdr["TypeOfRepl"];
            CashInType = (string)rdr["CashInType"];

            HolidaysVersion = (string)rdr["HolidaysVersion"];
            MatchDatesCateg = (string)rdr["MatchDatesCateg"];

            OverEst = (int)rdr["OverEst"];

            MinCash = (decimal)rdr["MinCash"];
            MaxCash = (decimal)rdr["MaxCash"];

            ReplAlertDays = (int)rdr["ReplAlertDays"];

            InsurOne = (decimal)rdr["InsurOne"];
            InsurTwo = (decimal)rdr["InsurTwo"];
            InsurThree = (decimal)rdr["InsurThree"];
            InsurFour = (decimal)rdr["InsurFour"];

            Supplier = (string)rdr["Supplier"];
            Model = (string)rdr["Model"];

            TerminalType = (string)rdr["TerminalType"];

            EjournalTypeId = (string)rdr["EjournalTypeId"];

            CitId = (string)rdr["CitId"];

            NoCassettes = (int)rdr["NoCassettes"];

            DepoReader = (bool)rdr["DepoReader"];

            DepoRecycling = (bool)rdr["DepoRecycling"];

            ChequeReader = (bool)rdr["ChequeReader"];
            EnvelopDepos = (bool)rdr["EnvelopDepos"];

            DepCurNm = (string)rdr["DepCurNm"];

            CasNo_11 = (int)rdr["CasNo_11"];

            CurName_11 = (string)rdr["CurName_11"];
            FaceValue_11 = (int)rdr["FaceValue_11"];
            CasCapacity_11 = (int)rdr["CasCapacity_11"];

            CasNo_12 = (int)rdr["CasNo_12"];

            CurName_12 = (string)rdr["CurName_12"];
            FaceValue_12 = (int)rdr["FaceValue_12"];
            CasCapacity_12 = (int)rdr["CasCapacity_12"];

            CasNo_13 = (int)rdr["CasNo_13"];

            CurName_13 = (string)rdr["CurName_13"];
            FaceValue_13 = (int)rdr["FaceValue_13"];
            CasCapacity_13 = (int)rdr["CasCapacity_13"];

            CasNo_14 = (int)rdr["CasNo_14"];

            CurName_14 = (string)rdr["CurName_14"];
            FaceValue_14 = (int)rdr["FaceValue_14"];
            CasCapacity_14 = (int)rdr["CasCapacity_14"];


            CasNo_15 = (int)rdr["CasNo_15"];

            CurName_15 = (string)rdr["CurName_15"];
            FaceValue_15 = (int)rdr["FaceValue_15"];
            CasCapacity_15 = (int)rdr["CasCapacity_15"];

            ActiveAtm = (bool)rdr["ActiveAtm"];

            Operator = (string)rdr["Operator"];

        }

        //
        // READ ATMs and created table as per request.
        //
        string WUserId; 
        public void ReadAtmAndFillTableByOperator(string InUserId, string InOperator)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WUserId = InUserId; 

            RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

            ClearDefineTable();

            // Operator = @Operator OR AtmNo = @AtmNo ";

            string SqlString = "SELECT *"
               + " FROM ATMsFields "
                 + " WHERE Operator = @Operator "
                  + " ORDER BY AtmNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ATMSReaderFields(rdr);

                            AddLineToTable(Jd);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertRecordsOfReport(InUserId);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // Added by ALECOS : READ ATMs and created table for Validation on EJ Import
        //
        public void ReadAtmAndFillTableForImportValidation(string InOperator)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

            ClearDefineTable();

            // Operator = @Operator OR AtmNo = @AtmNo ";

            string SqlString = "SELECT *" + " FROM ATMsFields" + " WHERE Operator = @Operator ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ATMSReaderFields(rdr);

                            AddLineToTable(Jd);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    // InsertRecordsOfReport(InUserId);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }


        //
        // READ ATMs and created table as per request.
        //
        public void ReadAtmAndFillTableByAtmNo(string InUserId, string InOperator, string InAtmNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

            ClearDefineTable();

            // Operator = @Operator OR AtmNo = @AtmNo ";

            string SqlString = "SELECT *"
               + " FROM ATMsFields"
                 + " WHERE Operator = @Operator AND AtmNo = @AtmNo ";

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

                            ATMSReaderFields(rdr);

                            AddLineToTable(Jd);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertRecordsOfReport(InUserId);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // READ ATMs and created table as per request.
        //
        public void ReadAtmAndFillTableByAtmsReconcGroup(string InUserId, string InOperator, int InAtmsReconcGroup)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

            ClearDefineTable();

            // Operator = @Operator OR AtmNo = @AtmNo ";

            string SqlString = "SELECT *"
               + " FROM ATMsFields"
                 + " WHERE Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InAtmsReconcGroup);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ATMSReaderFields(rdr);

                            AddLineToTable(Jd);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertRecordsOfReport(InUserId);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // READ ATMs and created table by CIT
        //
        public void ReadAtmAndFillTableByAtmsCitId(string InUserId, string InOperator, string InCitId)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

            ClearDefineTable();

            // Operator = @Operator OR AtmNo = @AtmNo ";

            string SqlString = "SELECT *"
               + " FROM ATMsFields"
                 + " WHERE Operator = @Operator AND CitId = @CitId ";

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

                            ATMSReaderFields(rdr);

                            AddLineToTable(Jd);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertRecordsOfReport(InUserId);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // Clear Define Table
        private void ClearDefineTable()
        {
            ATMsDetailsDataTable = new DataTable();
            ATMsDetailsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsDetailsDataTable.Columns.Add("AtmNo", typeof(string));
            ATMsDetailsDataTable.Columns.Add("AtmName", typeof(string));
            ATMsDetailsDataTable.Columns.Add("Branch", typeof(string));
            ATMsDetailsDataTable.Columns.Add("AtmsReconcGroup", typeof(int));
            ATMsDetailsDataTable.Columns.Add("CitId", typeof(string));
            ATMsDetailsDataTable.Columns.Add("CashInType", typeof(string));
            ATMsDetailsDataTable.Columns.Add("BranchName", typeof(string));
            ATMsDetailsDataTable.Columns.Add("Street", typeof(string));
            ATMsDetailsDataTable.Columns.Add("Town", typeof(string));
            ATMsDetailsDataTable.Columns.Add("District", typeof(string));
            ATMsDetailsDataTable.Columns.Add("PostalCode", typeof(string));
            ATMsDetailsDataTable.Columns.Add("Country", typeof(string));
           
            ATMsDetailsDataTable.Columns.Add("Model", typeof(string));
            ATMsDetailsDataTable.Columns.Add("FaceValue_11", typeof(int));
            ATMsDetailsDataTable.Columns.Add("FaceValue_12", typeof(int));
            ATMsDetailsDataTable.Columns.Add("FaceValue_13", typeof(int));
            ATMsDetailsDataTable.Columns.Add("FaceValue_14", typeof(int));
      
            ATMsDetailsDataTable.Columns.Add("TypeOfRepl", typeof(string));
          
            ATMsDetailsDataTable.Columns.Add("ATMIPAddress", typeof(string));
            ATMsDetailsDataTable.Columns.Add("SWVersion", typeof(string));
            ATMsDetailsDataTable.Columns.Add("UserId", typeof(string));
        }

        // ADD LINE TO TABLE

        private void AddLineToTable(RRDMJTMIdentificationDetailsClass Jd)
        {
            //
            // Fill In Table
            //

            DataRow RowSelected = ATMsDetailsDataTable.NewRow();

            RowSelected["AtmNo"] = AtmNo;
            RowSelected["AtmName"] = AtmName;
            RowSelected["Branch"] = Branch;
            RowSelected["AtmsReconcGroup"] = AtmsReconcGroup;
            RowSelected["CitId"] = CitId;
            RowSelected["CashInType"] = CashInType;
            RowSelected["BranchName"] = BranchName;
            RowSelected["Street"] = Street;

            RowSelected["Town"] = Town;
            RowSelected["District"] = District;
            RowSelected["PostalCode"] = PostalCode;

            RowSelected["Country"] = Country;

            RowSelected["Model"] = Supplier;
            RowSelected["FaceValue_11"] = FaceValue_11;
            RowSelected["FaceValue_12"] = FaceValue_12;
            RowSelected["FaceValue_13"] = FaceValue_13;
            RowSelected["FaceValue_14"] = FaceValue_14;

            RowSelected["TypeOfRepl"] = TypeOfRepl;

            Jd.ReadJTMIdentificationDetailsByAtmNo(AtmNo);
            if (Jd.RecordFound == true)
            {
                RowSelected["ATMIPAddress"] = Jd.ATMIPAddress;
                RowSelected["SWVersion"] = Jd.SWVersion;
            }
            else
            {
                RowSelected["ATMIPAddress"] = "FillData";
                RowSelected["SWVersion"] = "FillData";
            }
            RowSelected["UserId"] = WUserId; 
            // ADD ROW
            ATMsDetailsDataTable.Rows.Add(RowSelected);
        }

        // Insert Records Of Report 
        private void InsertRecordsOfReport(string InUserId)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport56(InUserId);

            // RECORDS READ AND PROCESSED 
            //TableMpa
            //using (SqlConnection conn2 =
            //               new SqlConnection(connectionString))
            //    try
            //    {
            //        conn2.Open();

            //        using (SqlBulkCopy s = new SqlBulkCopy(conn2))
            //        {
            //            s.DestinationTableName = "[ATMS].[dbo].[WReport56]";

            //            foreach (var column in ATMsDetailsDataTable.Columns)
            //                s.ColumnMappings.Add(column.ToString(), column.ToString());

            //            s.WriteToServer(ATMsDetailsDataTable);
            //        }
            //        conn2.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn2.Close();

            //        CatchDetails(ex);
            //    }

        }
        //
        // READ AtmAndFillATMsVsGroupsTable
        //
        public void ReadAtmAndFillATMsVsGroupsTable(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableATMsVsGroups = new DataTable();
            TableATMsVsGroups.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableATMsVsGroups.Columns.Add("AtmNo", typeof(string));
            TableATMsVsGroups.Columns.Add("AtmsReconcGroup", typeof(int));

            string SqlString = "SELECT *"
               + " FROM ATMsFields"
                 + " WHERE AtmsReconcGroup > 0 AND Operator = @Operator";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ATMSReaderFields(rdr);

                            //
                            // Fill In Table
                            //

                            DataRow RowSelected = TableATMsVsGroups.NewRow();

                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["AtmsReconcGroup"] = AtmsReconcGroup;

                            // ADD ROW
                            TableATMsVsGroups.Rows.Add(RowSelected);

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
        // READ ATM
        //
        public void ReadAtm(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].ATMsFields"
               + " WHERE AtmNo=@AtmNo";
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

                            ATMSReaderFields(rdr);
                        

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
        public void ReadAtm_OSMAN(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS_SAMIH_For_Osman].[dbo].ATMsFields"
               + " WHERE AtmNo=@AtmNo";
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

                            ATMSReaderFields(rdr);


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
        // READ ATM
        //
        public void ReadAtmFrom_Hst_AndCreate_ATM(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
           
            string SqlString = "SELECT DISTINCT AtmNo "
                  + " FROM[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] ";
          
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

                            AtmNo = (string)rdr["AtmNo"];

                            ReadAtm(AtmNo);
                            if (RecordFound == true)
                            {
                                // ATM Found
                            }
                            else
                            {
                                // Insert ATM_No
                                CreateNewAtmBasedOnGeneral_Model(InOperator, AtmNo, "00000506_20190702_EJ_NCR.000" );
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
        // CREATE NEW ATM BASED ON MODEL 
        //
        public void CreateNewAtmBasedOnGeneral_Model(string InOperator, string InAtmNo, string InJournalName)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //   string WJournalNAME = "00000506_20190702_EJ_NCR.000";

            string WJournalNAME = InJournalName; 

            string ATM_Supplier = WJournalNAME.Substring(21, 3); 
         
            string WModelAtm = ""; 
            DateTime FutureDate = new DateTime(2050, 11, 21);

            RRDMGasParameters Gp = new RRDMGasParameters();

            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            RRDMAtmsCostClass Ap = new RRDMAtmsCostClass();
            RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();
            RRDMAccountsClass Acc = new RRDMAccountsClass();

            // 
            string ParId = "932";
            string OccurId = "1";

            if (ATM_Supplier == "NCR")
            {
                OccurId = "1";
            }
            if (ATM_Supplier == "DBL")
            {
                OccurId = "2";
            }
            if (ATM_Supplier == "WCR")
            {
                OccurId = "3";
            }
            if (ATM_Supplier == "HYO")
            {
                OccurId = "4"; 
            }
            if (ATM_Supplier == "WDN")
            {
                OccurId = "5";
            }


            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound == true)
            {
                WModelAtm = Gp.OccuranceNm;
            }
            else
            {
               
                    ErrorFound = true;
                    ErrorOutput = "Model ATM Not defined in parameters";
             
            }

            try
                {
                    // This is for creating new ATM   

                    ReadAtm(WModelAtm);

                    if (RecordFound == true)
                    {
                        //*************************
                        // This is for insert the new ATM                  
                        //**************************
                        AtmNo = InAtmNo;
                        AtmName = "ATM with ID..."+ InAtmNo;
                        Branch = "Br.."+ InAtmNo;
                        BranchName = "Branch..." + InAtmNo;
                        Street = "UnKnown";
                        District = "UnKnown";
                   //     AtmsReconcGroup = 101;

                        // INSERT INSERT INSERT INSERT INSERT
                        InsertATM(InAtmNo); // Insert ATM record

                        //*************************
                        //  INSERT ATMs MAIN                   
                        //**************************
                        Am.ReadAtmsMainSpecific(WModelAtm); // READ FROM MODEL 

                        Am.AtmNo = InAtmNo;
                        Am.AtmName = "Name.." + InAtmNo;
                        Am.BankId = InOperator;

                        Am.RespBranch = "UnKnown";
                        Am.BranchName = "UnKnown";

                       // Initialise for new atm 
                        Am.NextReplDt = FutureDate;
                        Am.EstReplDt = FutureDate;

                        Am.LastUpdated = DateTime.Now;

                        Am.CitId = CitId;
                        Am.AtmsReplGroup = AtmsReplGroup;
                        Am.AtmsReconcGroup = AtmsReconcGroup;

                        Am.GL_CurrNm1 = DepCurNm;

                        Am.Operator = Operator;

                        // INSERT INSERT INSERT INSERT INSERT
                        Am.InsertInAtmsMain(InAtmNo); // Insert AtmMain record 

                        //*************************
                        //  COST                    
                        //**************************

                        Ap.ReadTableATMsCostSpecific(WModelAtm);

                        Ap.AtmNo = InAtmNo;
                    // Insert FIELDS SUCH AS 
                    // Ap.PurchaseDt = PurchaseDt;

                    // ===============Insert ================
                        Ap.Operator = Operator; 
                        Ap.InsertTableATMsCost(InAtmNo, BankId);


                        //*************************
                        // Insert Physical                  
                        //**************************

                        Jd.ReadJTMIdentificationDetailsByAtmNo(WModelAtm);

                        Jd.AtmNo = InAtmNo;

                        Jd.ATMIPAddress = "ATMIPAddress";

                        // ===============Insert ================
                        Jd.InsertNewRecordInJTMIdentificationDetails();

                        // ==============Copy ACCOUNTS FROM LIKE==========
                        ReadAtm(WModelAtm);

                        Acc.CopyAccountsAtmToAtm(BankId, WModelAtm, BankId, InAtmNo);
                        if (Acc.RecordFound == false)
                        {
                            //MessageBox.Show("There were no accounts to copy. After ATM creation go and create accounts manually please for the added ATM .");
                            //MessageBox.Show("ATM added without accounts");
                        }
                        else
                        {
                            // Everything OK 
                        }

                        //TotalNew = TotalNew + 1;

                    }
                }
                catch (Exception ex)
                {
                ErrorFound = true;
               
                string outmsg = ex.Message;
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    outmsg = @"\r\n\" + outmsg;
                    ex1 = ex1.InnerException;
                }

                ErrorOutput = outmsg;
                CatchDetails(ex);

                }

            ReadAtm(InAtmNo);
            if (RecordFound = true)
            {
                // OK
            }
            else
            {
                ErrorFound = true;
                ErrorOutput = "ATM Not Created.Look in RRDM Errors";
            }
        }


        // 
        // UPDATE ATMs Basic with active or not 
        //
        public void UpdateAtmsBasic(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ATMsFields SET "
                            + "[ActiveAtm] =@ActiveAtm "
                            + " WHERE AtmNo= @AtmNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ActiveAtm", ActiveAtm);

                        // Execute and check success 
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

            //  return outcome;

        }

        // 
        // UPDATE EjournalTypeId
        //
        public void UpdateEjournalTypeId(string InAtmNo,string InSupplier ,string InEjournalTypeId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ATMsFields SET "
                            + "[Supplier] =@Supplier "
                            + ",[Model] ='N/A' "
                            + ",[EjournalTypeId] =@EjournalTypeId "
                            + " WHERE AtmNo= @AtmNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Supplier", InSupplier);
                        cmd.Parameters.AddWithValue("@EjournalTypeId", InEjournalTypeId);

                        // Execute and check success 
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

            //  return outcome;

        }


        // 
        // FIND ATM OWNER for Replenishment  
        //
        public void ReadAtmOwner(string InAtmNo)
        {
            RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();

            RRDMUsersRecords Ua = new RRDMUsersRecords();

            ReadAtm(InAtmNo);

            ErrorFound = false;
            ErrorOutput = "";

            try
            {
                // Get the USER RESPONSIBLE FOR THIS ATM
                if (AtmsReplGroup > 0) Uaa.FindUserForRepl("", AtmsReplGroup);

                //       UsersAccessToAtms Uaa = new UsersAccessToAtms(); 
                //    UsersAndSignedRecord Ua = new UsersAndSignedRecord();

                else Uaa.FindUserForRepl(InAtmNo, 0);

                // FIND USER FOR THIS 

                if (Uaa.RecordFound == true)
                {
                    Ua.ReadUsersRecord(Uaa.UserId); // Get Info for User 

                    AtmReplUserId = Ua.UserId;
                    AtmReplUserName = Ua.UserName;
                    AtmReplUserEmail = Ua.email;
                    AtmReplMobileNo = Ua.MobileNo;

                }


            }
            catch (Exception ex)
            {


                CatchDetails(ex);


            }

            //  return outcome;

        }
        //
        // Insert ATM
        //
        int rows;
        public void InsertATM(string InAtmNo)
        {
            string cmdinsert = "INSERT INTO [ATMsFields] "
                + " ([AtmNo],[AtmName],[BankId],"
                + "[Branch],[BranchName],"
                + "[Street],[Town],[District],[PostalCode],[Country],"
                + "[Latitude],[Longitude],"
                + "[AtmsStatsGroup],[AtmsReplGroup],[AtmsReconcGroup],"
                + "[Loby],[Wall],[Drive],[OffSite],"
                + "[TypeOfRepl],  [CashInType], [HolidaysVersion],[MatchDatesCateg],[OverEst],[MinCash],[MaxCash],[ReplAlertDays],"
                + "[InsurOne],[InsurTwo],[InsurThree],[InsurFour],"
                + "[Supplier],[Model], [TerminalType], [EjournalTypeId],"
                + "[CitId],[NoCassettes],"
                + "[DepoReader],[DepoRecycling],"
                + "[ChequeReader],[EnvelopDepos],"
                + " [DepCurNm],"
                + "[CasNo_11],[CurName_11],[FaceValue_11] ,[CasCapacity_11],"
                + "[CasNo_12],[CurName_12],[FaceValue_12],[CasCapacity_12],"
                + "[CasNo_13],[CurName_13],[FaceValue_13] ,[CasCapacity_13],"
                + "[CasNo_14],[CurName_14],[FaceValue_14] ,[CasCapacity_14],"
                + "[CasNo_15],[CurName_15],[FaceValue_15] ,[CasCapacity_15],"
                + "[ActiveAtm],"
                + "[Operator])"
                + " VALUES (@AtmNo,@AtmName,@BankId,"
                + "@Branch,@BranchName,"
                 + "@Street,@Town,@District,@PostalCode,@Country,"
                + "@Latitude,@Longitude,"
                + "@AtmsStatsGroup,@AtmsReplGroup,@AtmsReconcGroup,"
                + "@Loby,@Wall,@Drive,@OffSite,"
                + "@TypeOfRepl, @CashInType, @HolidaysVersion,@MatchDatesCateg,@OverEst,@MinCash,@MaxCash,@ReplAlertDays,"
                 + "@InsurOne,@InsurTwo,@InsurThree,@InsurFour,"
                + "@Supplier,@Model, @TerminalType, @EjournalTypeId,"
                + "@CitId,@NoCassettes,"
                + "@DepoReader,@DepoRecycling,"
                + "@ChequeReader,@EnvelopDepos,"
                + " @DepCurNm,"
                + "@CasNo_11,@CurName_11,@FaceValue_11 ,@CasCapacity_11,"
                + "@CasNo_12,@CurName_12,@FaceValue_12,@CasCapacity_12,"
                + "@CasNo_13,@CurName_13,@FaceValue_13 ,@CasCapacity_13,"
                + "@CasNo_14,@CurName_14,@FaceValue_14 ,@CasCapacity_14, "
                + "@CasNo_15,@CurName_15,@FaceValue_15 ,@CasCapacity_15, "
                + "@ActiveAtm,"
                + "@Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmName", AtmName);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@Branch", Branch);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);

                        cmd.Parameters.AddWithValue("@Street", Street);
                        cmd.Parameters.AddWithValue("@Town", Town);
                        cmd.Parameters.AddWithValue("@District", District);

                        cmd.Parameters.AddWithValue("@PostalCode", PostalCode);

                        cmd.Parameters.AddWithValue("@Country", Country);

                        cmd.Parameters.AddWithValue("@Latitude", Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", Longitude);

                        cmd.Parameters.AddWithValue("@AtmsStatsGroup", AtmsStatsGroup);
                        cmd.Parameters.AddWithValue("@AtmsReplGroup", AtmsReplGroup);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@Loby", Loby);
                        cmd.Parameters.AddWithValue("@Wall", Wall);
                        cmd.Parameters.AddWithValue("@Drive", Drive);

                        cmd.Parameters.AddWithValue("@OffSite", OffSite);

                        cmd.Parameters.AddWithValue("@TypeOfRepl", TypeOfRepl);
                        cmd.Parameters.AddWithValue("@CashInType", CashInType);

                        cmd.Parameters.AddWithValue("@HolidaysVersion", HolidaysVersion);

                        cmd.Parameters.AddWithValue("@MatchDatesCateg", MatchDatesCateg);

                        cmd.Parameters.AddWithValue("@OverEst", OverEst);

                        cmd.Parameters.AddWithValue("@MinCash", MinCash);
                        cmd.Parameters.AddWithValue("@MaxCash", MaxCash);
                        cmd.Parameters.AddWithValue("@ReplAlertDays", ReplAlertDays);

                        cmd.Parameters.AddWithValue("@InsurOne", InsurOne);
                        cmd.Parameters.AddWithValue("@InsurTwo", InsurTwo);
                        cmd.Parameters.AddWithValue("@InsurThree", InsurThree);
                        cmd.Parameters.AddWithValue("@InsurFour", InsurFour);

                        cmd.Parameters.AddWithValue("@Supplier", Supplier);
                        cmd.Parameters.AddWithValue("@Model", Model);
                        cmd.Parameters.AddWithValue("@TerminalType", TerminalType);
                        
                        cmd.Parameters.AddWithValue("@EjournalTypeId", EjournalTypeId);

                        cmd.Parameters.AddWithValue("@CitId", CitId);
                        cmd.Parameters.AddWithValue("@NoCassettes", NoCassettes);
                        cmd.Parameters.AddWithValue("@DepoReader", DepoReader);
                        cmd.Parameters.AddWithValue("@DepoRecycling", DepoRecycling);
                        
                        cmd.Parameters.AddWithValue("@ChequeReader", ChequeReader);

                        cmd.Parameters.AddWithValue("@EnvelopDepos", EnvelopDepos);

                        cmd.Parameters.AddWithValue("@ActiveAtm", ActiveAtm);

                        cmd.Parameters.AddWithValue("@DepCurNm", DepCurNm);

                        cmd.Parameters.AddWithValue("@CasNo_11", 1); // Cassette 1 

                        cmd.Parameters.AddWithValue("@CurName_11", CurName_11);
                        cmd.Parameters.AddWithValue("@FaceValue_11", FaceValue_11);
                        cmd.Parameters.AddWithValue("@CasCapacity_11", CasCapacity_11);

                        cmd.Parameters.AddWithValue("@CasNo_12", 2); // Cassette 2

                        cmd.Parameters.AddWithValue("@CurName_12", CurName_12);
                        cmd.Parameters.AddWithValue("@FaceValue_12", FaceValue_12);
                        cmd.Parameters.AddWithValue("@CasCapacity_12", CasCapacity_12);

                        cmd.Parameters.AddWithValue("@CasNo_13", 3); // Cassette 3

                        cmd.Parameters.AddWithValue("@CurName_13", CurName_13);
                        cmd.Parameters.AddWithValue("@FaceValue_13", FaceValue_13);
                        cmd.Parameters.AddWithValue("@CasCapacity_13", CasCapacity_13);

                        cmd.Parameters.AddWithValue("@CasNo_14", 4); // Cassette 4

                        cmd.Parameters.AddWithValue("@CurName_14", CurName_14);
                        cmd.Parameters.AddWithValue("@FaceValue_14", FaceValue_14);
                        cmd.Parameters.AddWithValue("@CasCapacity_14", CasCapacity_14);

                        cmd.Parameters.AddWithValue("@CasNo_15", 5); // Cassette 5

                        cmd.Parameters.AddWithValue("@CurName_15", CurName_15);
                        cmd.Parameters.AddWithValue("@FaceValue_15", FaceValue_15);
                        cmd.Parameters.AddWithValue("@CasCapacity_15", CasCapacity_15);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
                        //if (rows > 0) exception = " A NEW ATM WAS CREADED - ITs ATM No iS GIVEN AUTOMATICALLY. GO TO TABLE TO SEE IT";
                        //else exception = " Nothing WAS UPDATED ";

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

        public void UpdateATM(string InAtmNo)
        {
            string strUpdate = "UPDATE ATMsFields SET"
                + " AtmName=@AtmName, BankId=@BankId,"
                + "Branch=@Branch, BranchName=@BranchName,"
                + "Street=@Street, Town=@Town, District=@District, PostalCode=@PostalCode,Country=@Country,"
                + "Latitude=@Latitude,Longitude=@Longitude,"
                + " AtmsStatsGroup=@AtmsStatsGroup,AtmsReplGroup=@AtmsReplGroup, AtmsReconcGroup=@AtmsReconcGroup,"
                + " Loby=@Loby, Wall=@Wall, Drive=@Drive,OffSite=@OffSite,"
                + "TypeOfRepl=@TypeOfRepl, CashInType=@CashInType, HolidaysVersion=@HolidaysVersion,MatchDatesCateg=@MatchDatesCateg,"
                + "OverEst=@OverEst, MinCash=@MinCash, MaxCash=@MaxCash, ReplAlertDays=@ReplAlertDays,"
                 + "InsurOne=@InsurOne, InsurTwo =@InsurTwo, InsurThree=@InsurThree, InsurFour=@InsurFour,"
                + "Supplier=@Supplier, Model=@Model, TerminalType=@TerminalType, EjournalTypeId = @EjournalTypeId, "
                + "CitId=@CitId, NoCassettes=@NoCassettes,"
                + " DepoReader=@DepoReader, DepoRecycling=@DepoRecycling,"
                + " ChequeReader=@ChequeReader,EnvelopDepos=@EnvelopDepos,"
               + " ActiveAtm=@ActiveAtm,"
               + " DepCurNm=@DepCurNm,"
                + " CurName_11=@CurName_11,FaceValue_11=@FaceValue_11,CasCapacity_11=@CasCapacity_11,"
                + " CurName_12=@CurName_12,FaceValue_12=@FaceValue_12,CasCapacity_12=@CasCapacity_12,"
                + " CurName_13=@CurName_13,FaceValue_13=@FaceValue_13,CasCapacity_13=@CasCapacity_13,"
                + " CurName_14=@CurName_14,FaceValue_14=@FaceValue_14,CasCapacity_14=@CasCapacity_14,"
                + " CurName_15=@CurName_15,FaceValue_15=@FaceValue_15,CasCapacity_15=@CasCapacity_15"
                + " WHERE AtmNo=@AtmNo";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(strUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmName", AtmName);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@Branch", Branch);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);

                        cmd.Parameters.AddWithValue("@Street", Street);
                        cmd.Parameters.AddWithValue("@Town", Town);
                        cmd.Parameters.AddWithValue("@District", District);

                        cmd.Parameters.AddWithValue("@PostalCode", PostalCode);

                        cmd.Parameters.AddWithValue("@Country", Country);

                        cmd.Parameters.AddWithValue("@Latitude", Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", Longitude);

                        cmd.Parameters.AddWithValue("@AtmsStatsGroup", AtmsStatsGroup);
                        cmd.Parameters.AddWithValue("@AtmsReplGroup", AtmsReplGroup);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@Loby", Loby);
                        cmd.Parameters.AddWithValue("@Wall", Wall);
                        cmd.Parameters.AddWithValue("@Drive", Drive);

                        cmd.Parameters.AddWithValue("@OffSite", OffSite);

                        cmd.Parameters.AddWithValue("@TypeOfRepl", TypeOfRepl);
                        cmd.Parameters.AddWithValue("@CashInType", CashInType);

                        cmd.Parameters.AddWithValue("@HolidaysVersion", HolidaysVersion);

                        cmd.Parameters.AddWithValue("@MatchDatesCateg", MatchDatesCateg);

                        cmd.Parameters.AddWithValue("@OverEst", OverEst);

                        cmd.Parameters.AddWithValue("@MinCash", MinCash);
                        cmd.Parameters.AddWithValue("@MaxCash", MaxCash);
                        cmd.Parameters.AddWithValue("@ReplAlertDays", ReplAlertDays);

                        cmd.Parameters.AddWithValue("@InsurOne", InsurOne);
                        cmd.Parameters.AddWithValue("@InsurTwo", InsurTwo);
                        cmd.Parameters.AddWithValue("@InsurThree", InsurThree);
                        cmd.Parameters.AddWithValue("@InsurFour", InsurFour);

                        cmd.Parameters.AddWithValue("@Supplier", Supplier);
                        cmd.Parameters.AddWithValue("@Model", Model);
                        cmd.Parameters.AddWithValue("@TerminalType", TerminalType);

                        cmd.Parameters.AddWithValue("@EjournalTypeId", EjournalTypeId);

                        cmd.Parameters.AddWithValue("@CitId", CitId);
                        cmd.Parameters.AddWithValue("@NoCassettes", NoCassettes);
                        cmd.Parameters.AddWithValue("@DepoReader", DepoReader);
                        cmd.Parameters.AddWithValue("@DepoRecycling", DepoRecycling);
                        cmd.Parameters.AddWithValue("@ChequeReader", ChequeReader);

                        cmd.Parameters.AddWithValue("@EnvelopDepos", EnvelopDepos);

                        cmd.Parameters.AddWithValue("@ActiveAtm", ActiveAtm);

                        cmd.Parameters.AddWithValue("@DepCurNm", DepCurNm);

                        cmd.Parameters.AddWithValue("@CasNo_11", 1); // Cassette 1 

                        cmd.Parameters.AddWithValue("@CurName_11", CurName_11);
                        cmd.Parameters.AddWithValue("@FaceValue_11", FaceValue_11);
                        cmd.Parameters.AddWithValue("@CasCapacity_11", CasCapacity_11);

                        cmd.Parameters.AddWithValue("@CasNo_12", 2); // Cassette 2

                        cmd.Parameters.AddWithValue("@CurName_12", CurName_12);
                        cmd.Parameters.AddWithValue("@FaceValue_12", FaceValue_12);
                        cmd.Parameters.AddWithValue("@CasCapacity_12", CasCapacity_12);

                        cmd.Parameters.AddWithValue("@CasNo_13", 3); // Cassette 3

                        cmd.Parameters.AddWithValue("@CurName_13", CurName_13);
                        cmd.Parameters.AddWithValue("@FaceValue_13", FaceValue_13);
                        cmd.Parameters.AddWithValue("@CasCapacity_13", CasCapacity_13);

                        cmd.Parameters.AddWithValue("@CasNo_14", 4); // Cassette 4

                        cmd.Parameters.AddWithValue("@CurName_14", CurName_14);
                        cmd.Parameters.AddWithValue("@FaceValue_14", FaceValue_14);
                        cmd.Parameters.AddWithValue("@CasCapacity_14", CasCapacity_14);

                        cmd.Parameters.AddWithValue("@CasNo_15", 5); // Cassette 5

                        cmd.Parameters.AddWithValue("@CurName_15", CurName_15);
                        cmd.Parameters.AddWithValue("@FaceValue_15", FaceValue_15);
                        cmd.Parameters.AddWithValue("@CasCapacity_15", CasCapacity_15);
                        //rows number of record got updated

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
        // DELETE ATM by ATM No
        // DELETE ALL RELATIONS 
        //
        public void DeleteNotActiveATMBasic(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    // ATM FIELDS 
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM dbo.ATMsFields "
                            + " WHERE AtmNo = @AtmNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.ExecuteNonQuery();


                    }

                    // ATM MAIN
                    using (SqlCommand cmd =
                      new SqlCommand("DELETE FROM dbo.AtmsMain "
                          + " WHERE AtmNo = @AtmNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.ExecuteNonQuery();


                    }

                    // ATM COST 
                    using (SqlCommand cmd =
                    new SqlCommand("DELETE FROM dbo.ATMsCost "
                        + " WHERE AtmNo = @AtmNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.ExecuteNonQuery();


                    }

                    // ATM JTMIdentificationDetails
                    using (SqlCommand cmd =
                    new SqlCommand("DELETE FROM dbo.JTMIdentificationDetails "
                        + " WHERE AtmNo = @AtmNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.ExecuteNonQuery();


                    }

                    // TempAtmLocation

                    using (SqlCommand cmd =
                   new SqlCommand("DELETE FROM dbo.TempAtmLocation "
                       + " WHERE AtmNo = @AtmNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.ExecuteNonQuery();


                    }

                    // ACCOUNTS
                    //
                    using (SqlCommand cmd =
                          new SqlCommand("DELETE FROM dbo.AccountsTable "
                   + " WHERE AtmNo = @AtmNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

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

        ////
        //// Catch Details 
        ////
        //private static void CatchDetails(Exception ex)
        //{
        //    RRDMLog4Net Log = new RRDMLog4Net();

        //    StringBuilder WParameters = new StringBuilder();

        //    WParameters.Append("User : ");
        //    WParameters.Append("NotAssignYet");
        //    WParameters.Append(Environment.NewLine);

        //    WParameters.Append("ATMNo : ");
        //    WParameters.Append("NotDefinedYet");
        //    WParameters.Append(Environment.NewLine);

        //    string Logger = "RRDM4Atms";
        //    string Parameters = WParameters.ToString();

        //    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

        //    if (Environment.UserInteractive)
        //    {
        //        System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
        //                                                 + " . Application will be aborted! Call controller to take care. ");
        //    }

        //    //Environment.Exit(0);
        //}
    }
}

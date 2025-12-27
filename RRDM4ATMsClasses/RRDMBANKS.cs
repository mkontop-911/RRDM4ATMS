using System;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMBanks : Logger
    {
        public RRDMBanks() : base() { }

        private string _BankId;
        private string _ShortName;
        private string _ActiveDirectoryDM;
        private string _AdGroup;

        private string _BankName;
        private string _BankCountry;

        private string _GroupName;

        private string _BasicCurName;

        private string _SettlementAccount;
        private bool _SettlementBank;
        private string _SettlementClearingAccount;

        private DateTime _DtTmCreated;
        private bool _UsingGAS;

        private int _BanksInGroup;

        private byte[] _Logo;

        private DateTime _LastMatchingDtTm; // This date is checked during sign in
                                            // If Today is bigger then we run the matching process 
                                            // of Trans to be posted with Host

        private string _SenderEmail; // the Banks Sender email
        private string _SenderUserName;
        private string _SenderPassword;
        private string _SenderSmtpClient;
        private int _SenderPort;

        private string _Operator;

        private int _BanksWithOperator;

        // Define the data table 
        private DataTable _BanksDataTable;
        private int _TotalSelected;

        private bool _RecordFound;
        private bool _ErrorFound;
        private string _ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        // string connectionString;

        public string BankId { get { return _BankId; } set { _BankId = value; } }
        public string ShortName { get { return _ShortName; } set { _ShortName = value; } }
        public string ActiveDirectoryDM { get { return _ActiveDirectoryDM; } set { _ActiveDirectoryDM = value; } }
        public string AdGroup { get { return _AdGroup; } set { _AdGroup = value; } }
        public string BankName { get { return _BankName; } set { _BankName = value; } }
        public string BankCountry { get { return _BankCountry; } set { _BankCountry = value; } }
        public string GroupName { get { return _GroupName; } set { _GroupName = value; } }
        public string BasicCurName { get { return _BasicCurName; } set { _BasicCurName = value; } }
        public string SettlementAccount { get { return _SettlementAccount; } set { _SettlementAccount = value; } }
        public bool SettlementBank { get { return _SettlementBank; } set { _SettlementBank = value; } }
        public string SettlementClearingAccount { get { return _SettlementClearingAccount; } set { _SettlementClearingAccount = value; } }
        public DateTime DtTmCreated { get { return _DtTmCreated; } set { _DtTmCreated = value; } }
        public bool UsingGAS { get { return _UsingGAS; } set { _UsingGAS = value; } }
        public int BanksInGroup { get { return _BanksInGroup; } set { _BanksInGroup = value; } }
        public byte[] Logo { get { return _Logo; } set { _Logo = value; } }
        public DateTime LastMatchingDtTm { get { return _LastMatchingDtTm; } set { _LastMatchingDtTm = value; } }

        public string SenderEmail { get { return _SenderEmail; } set { _SenderEmail = value; } }
        public string SenderUserName { get { return _SenderUserName; } set { _SenderUserName = value; } }
        public string SenderPassword { get { return _SenderPassword; } set { _SenderPassword = value; } }
        public string SenderSmtpClient { get { return _SenderSmtpClient; } set { _SenderSmtpClient = value; } }
        public int SenderPort { get { return _SenderPort; } set { _SenderPort = value; } }
        public string Operator { get { return _Operator; } set { _Operator = value; } }


        // The following do not have their values set. They are 'return' only properties
        public int TotalSelected { get { return _TotalSelected; } }
        public bool RecordFound { get { return _RecordFound; } }
        public bool ErrorFound { get { return _ErrorFound; } }
        public string ErrorOutput { get { return _ErrorOutput; } }
        public DataTable BanksDataTable { get { return _BanksDataTable; } }

       // public RRDMBanks()
       // {
       //     try
       //     {
       //         connectionString = AppConfig.Configuration.GetConnectionString
       //["ATMSConnectionString"].ConnectionString;
       //     }
       //     catch (Exception ex)
       //     {
       //         string msg = ex.Message;
       //     }

       // }


        // Bank's Reader's fields 
        private void BanksReaderFields(SqlDataReader rdr)
        {
            _BankId = (string)rdr["BankId"];
            _ShortName = (string)rdr["ShortName"];
            _ActiveDirectoryDM = (string)rdr["ActiveDirectoryDM"];
            _AdGroup = (string)rdr["AdGroup"];

            _BankName = (string)rdr["BankName"];
            _BankCountry = (string)rdr["BankCountry"];

            _GroupName = (string)rdr["GroupName"];

            _BasicCurName = (string)rdr["BasicCurName"];

            _SettlementAccount = (string)rdr["SettlementAccount"];
            _SettlementBank = (bool)rdr["SettlementBank"];
            _SettlementClearingAccount = (string)rdr["SettlementClearingAccount"];

            _DtTmCreated = (DateTime)rdr["DtTmCreated"];
            _UsingGAS = (bool)rdr["UsingGAS"];
            _Logo = (byte[])rdr["Logo"];

            _LastMatchingDtTm = (DateTime)rdr["LastMatchingDtTm"];

            _SenderEmail = (string)rdr["SenderEmail"];
            _SenderUserName = (string)rdr["SenderUserName"];
            _SenderPassword = (string)rdr["SenderPassword"];
            _SenderSmtpClient = (string)rdr["SenderSmtpClient"];
            _SenderPort = (int)rdr["SenderPort"];

            _Operator = (string)rdr["Operator"];
        }

        // READ BANK 

        public void ReadBank(string InBankId)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = ""; 

            string SqlString = "SELECT *"
                   + " FROM [dbo].[BANKS] "
                   + " WHERE BankId = @BankId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            _RecordFound = true;

                            // Read Bank Details
                            BanksReaderFields(rdr);

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


        // READ BANK by short name  

        public void ReadBankByShortName(string InShortName)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [dbo].[BANKS] "
          + " WHERE ShortName = @ShortName";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ShortName", InShortName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            _RecordFound = true;

                            // Read Bank Details
                            BanksReaderFields(rdr);

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

        // READ BANK by short name  

        public void ReadBankToGetName(int InMode)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = "";

            string SqlString = "SELECT Top (1) *"
          + " FROM [dbo].[BANKS] "
          + "  ";
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
                            _RecordFound = true;

                            // Read Bank Details
                            BanksReaderFields(rdr);

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
                    _ErrorFound = true;
                  
                    CatchDetails(ex);

                }
          
        }


        //
        // READ Banks AND Fill table by Operator
        //

        public void ReadBanksForDataTableByOperator(string InOperator, int InMode)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = "";

            //InMode = 1 ... is ITMX
            //InMode = 2 ... is Clearing Bank = central Bank

            _BanksDataTable = new DataTable();
            _BanksDataTable.Clear();

            _TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            _BanksDataTable.Columns.Add("BankId", typeof(string));
            _BanksDataTable.Columns.Add("ShortName", typeof(string));
            _BanksDataTable.Columns.Add("Full Name", typeof(string));
            _BanksDataTable.Columns.Add("Country", typeof(string));
            _BanksDataTable.Columns.Add("DateInRRDM", typeof(string));

            if (InMode == 2)
            {
                _BanksDataTable.Columns.Add("Ccy", typeof(string));
                _BanksDataTable.Columns.Add("SettlementAcc", typeof(string));
                _BanksDataTable.Columns.Add("Settl_Clearing", typeof(string));
            }

            string SqlString = "SELECT *"
               + " FROM [dbo].[BANKS] "
               + " WHERE Operator = @Operator";
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
                            _RecordFound = true;


                            // Read Bank Details
                            BanksReaderFields(rdr);
                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = _BanksDataTable.NewRow();

                            RowSelected["BankId"] = _BankId;
                            RowSelected["ShortName"] = _ShortName;
                            RowSelected["Full Name"] = _BankName;
                            RowSelected["Country"] = _BankCountry;
                            RowSelected["DateInRRDM"] = _DtTmCreated;
                            if (InMode == 2)
                            {
                                RowSelected["Ccy"] = _BasicCurName;
                                RowSelected["SettlementAcc"] = _SettlementAccount;
                                RowSelected["Settl_Clearing"] = _SettlementClearingAccount;
                            }

                            // ADD ROW
                            _BanksDataTable.Rows.Add(RowSelected);

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
        // READ Banks AND Fill table by Operator
        //

        public void ReadBanksForDataTableByBankId(string InOperator, string InBankId, int InMode)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = "";

            //InMode = 1 ... is ITMX
            //InMode = 2 ... is Clearing Bank = central Bank

            _BanksDataTable = new DataTable();
            _BanksDataTable.Clear();

            _TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            _BanksDataTable.Columns.Add("BankId", typeof(string));
            _BanksDataTable.Columns.Add("ShortName", typeof(string));
            _BanksDataTable.Columns.Add("Full Name", typeof(string));
            _BanksDataTable.Columns.Add("Country", typeof(string));
            _BanksDataTable.Columns.Add("DateInRRDM", typeof(string));

            if (InMode == 2)
            {
                _BanksDataTable.Columns.Add("Ccy", typeof(string));
                _BanksDataTable.Columns.Add("SettlementAcc", typeof(string));
                _BanksDataTable.Columns.Add("Settl_Clearing", typeof(string));
            }

            string SqlString = "SELECT *"
               + " FROM [dbo].[BANKS] "
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
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            _RecordFound = true;


                            // Read Bank Details
                            BanksReaderFields(rdr);
                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = _BanksDataTable.NewRow();

                            RowSelected["BankId"] = _BankId;
                            RowSelected["ShortName"] = _ShortName;
                            RowSelected["Full Name"] = _BankName;
                            RowSelected["Country"] = _BankCountry;
                            RowSelected["DateInRRDM"] = _DtTmCreated;
                            if (InMode == 2)
                            {
                                RowSelected["Ccy"] = _BasicCurName;
                                RowSelected["SettlementAcc"] = _SettlementAccount;
                                RowSelected["Settl_Clearing"] = _SettlementClearingAccount;
                            }

                            // ADD ROW
                            _BanksDataTable.Rows.Add(RowSelected);

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
        // GET BANKS belonging 
        //
        public ArrayList GetBanksShortNames(string InOperator)
        {
            //USED ONLY TO DEFINE CATEGORIES IT IS NOT USED FOR OTHER 
            ArrayList BanksIdsList = new ArrayList
            {
                "SelectEntity"
            };

            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [dbo].[BANKS] "
          + " WHERE Operator = @Operator";
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
                            _RecordFound = true;
                            _ShortName = (string)rdr["ShortName"];
                          
                            BanksIdsList.Add(_ShortName);    
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

            return BanksIdsList;
        }
        // READ Operator

        public void ReadBanksForOperator(string InOperator)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[BANKS] "
          + " WHERE Operator = @Operator";
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
                            _RecordFound = true;

                            // Read Bank Details
                            BanksReaderFields(rdr);

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

        // READ Settlement Bank 

        public void ReadSettlementBank(string InOperator)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [dbo].[BANKS] "
          + " WHERE Operator = @Operator AND SettlementBank = 1";
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
                            _RecordFound = true;

                            // Read Bank Details
                            BanksReaderFields(rdr);

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

        // READ Active Directory 

        public void ReadBankActiveDirectory(string InActiveDirectoryDM)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = "";

            string SqlString = "SELECT * "
          + " FROM [dbo].[BANKS] "
          + " WHERE ActiveDirectoryDM = @ActiveDirectoryDM";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ActiveDirectoryDM", InActiveDirectoryDM);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            _RecordFound = true;


                            // Read Bank Details
                            BanksReaderFields(rdr);

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
        // READ What is the Number of Banks in Group of Banks  

        public void ReadNoBanksInGroup(string InGroupName)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = ""; 

            _BanksInGroup = 0; 
          
            string SqlString = "SELECT *"
          + " FROM [dbo].[BANKS] "
          + " WHERE GroupName = @GroupName";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupName", InGroupName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            _RecordFound = true;

                            _BanksInGroup = _BanksInGroup + 1; 
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

        // READ What is the Number of Banks in Operator

        public void ReadNoBanksWithOperator(string InOperator)
        {
            _RecordFound = false;
            _ErrorFound = false;
            _ErrorOutput = ""; 

            _BanksWithOperator = 0;
         
            string SqlString = "SELECT *"
          + " FROM [dbo].[BANKS] "
          + " WHERE Operator = @Operator";
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
                            _RecordFound = true;

                            _BanksWithOperator = _BanksWithOperator + 1;
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
        // Insert NEW BANK 
        //
        public void InsertBank(string InBankId)
        {
            
            _ErrorFound = false;
            _ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [BANKS]"
                + " ([BankId],[ShortName],[ActiveDirectoryDM],[AdGroup],[BankName],[BankCountry],"
                + " [GroupName],"
                + "[BasicCurName], "
                + "[SettlementAccount], "
                + "[SettlementBank], "
                + "[SettlementClearingAccount], "
                + "[DtTmCreated],[UsingGAS],[Logo],"
                + "[SenderEmail],[SenderUserName],[SenderPassword],[SenderSmtpClient],[SenderPort],"
                + "[Operator]  ) "
                + " VALUES (@BankId,@ShortName,@ActiveDirectoryDM,@AdGroup,@BankName,@BankCountry,"
                + "@GroupName,"
                + "@BasicCurName,"
                + "@SettlementAccount,"
                + "@SettlementBank,"
                + "@SettlementClearingAccount,"
                + "@DtTmCreated,@UsingGAS,@Logo,"
                + "@SenderEmail,@SenderUserName,@SenderPassword,@SenderSmtpClient,@SenderPort,"
                + "@Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@BankId", _BankId);
                        cmd.Parameters.AddWithValue("@ShortName", _ShortName);

                        cmd.Parameters.AddWithValue("@ActiveDirectoryDM", _ActiveDirectoryDM);
                        cmd.Parameters.AddWithValue("@AdGroup", _AdGroup);
                
                        cmd.Parameters.AddWithValue("@BankName", _BankName);
                        cmd.Parameters.AddWithValue("@BankCountry", _BankCountry);

                        cmd.Parameters.AddWithValue("@GroupName", _GroupName);

                        cmd.Parameters.AddWithValue("@BasicCurName", _BasicCurName);

                        cmd.Parameters.AddWithValue("@SettlementAccount", _SettlementAccount);
                        cmd.Parameters.AddWithValue("@SettlementBank", _SettlementBank);
                        cmd.Parameters.AddWithValue("@SettlementClearingAccount", _SettlementClearingAccount);

                        cmd.Parameters.AddWithValue("@DtTmCreated", _DtTmCreated);
                        cmd.Parameters.AddWithValue("@UsingGAS", _UsingGAS);
                        cmd.Parameters.AddWithValue("@Logo", _Logo);

                        cmd.Parameters.AddWithValue("@SenderEmail", _SenderEmail);
                        cmd.Parameters.AddWithValue("@SenderUserName", _SenderUserName);
                        cmd.Parameters.AddWithValue("@SenderPassword", _SenderPassword);
                        cmd.Parameters.AddWithValue("@SenderSmtpClient", _SenderSmtpClient);
                        cmd.Parameters.AddWithValue("@SenderPort", _SenderPort);
                       
                        cmd.Parameters.AddWithValue("@Operator", _Operator);

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


        // UPDATE BANK
        // 
        public void UpdateBank(string InBankId)
        {
           
            _ErrorFound = false;
            _ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE BANKS SET "
                            + " BankId = @BankId, ShortName = @ShortName, ActiveDirectoryDM = @ActiveDirectoryDM, AdGroup = @AdGroup,"
                             + "BankName = @BankName, BankCountry = @BankCountry,"
                             + "GroupName = @GroupName,"
                             + "BasicCurName = @BasicCurName,"
                             + "SettlementAccount = @SettlementAccount,"
                             + "SettlementBank = @SettlementBank,"
                             + "SettlementClearingAccount = @SettlementClearingAccount,"
                             + "DtTmCreated = @DtTmCreated, UsingGAS = @UsingGAS, Logo = @Logo,  LastMatchingDtTm = @LastMatchingDtTm, "
                              + "SenderEmail = @SenderEmail, SenderUserName = @SenderUserName, SenderPassword = @SenderPassword,"
                              + " SenderSmtpClient = @SenderSmtpClient, SenderPort = @SenderPort "
                            + " WHERE BankId = @BankId", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@ShortName", _ShortName);

                        cmd.Parameters.AddWithValue("@ActiveDirectoryDM", _ActiveDirectoryDM);
                        cmd.Parameters.AddWithValue("@AdGroup", _AdGroup);

                        cmd.Parameters.AddWithValue("@BankName", _BankName);
                        cmd.Parameters.AddWithValue("@BankCountry", _BankCountry);

                        cmd.Parameters.AddWithValue("@GroupName", _GroupName);
                        //    cmd.Parameters.AddWithValue("@BasicCurCode", BasicCurCode);

                        cmd.Parameters.AddWithValue("@BasicCurName", _BasicCurName);
                        cmd.Parameters.AddWithValue("@SettlementAccount", _SettlementAccount);
                        cmd.Parameters.AddWithValue("@SettlementBank", _SettlementBank);
                        cmd.Parameters.AddWithValue("@SettlementClearingAccount", _SettlementClearingAccount);

                        cmd.Parameters.AddWithValue("@DtTmCreated", _DtTmCreated);
                        cmd.Parameters.AddWithValue("@UsingGAS", _UsingGAS);

                        cmd.Parameters.AddWithValue("@Logo", _Logo);

                        cmd.Parameters.AddWithValue("@LastMatchingDtTm", _LastMatchingDtTm);

                        cmd.Parameters.AddWithValue("@SenderEmail", _SenderEmail);
                        cmd.Parameters.AddWithValue("@SenderUserName", _SenderUserName);
                        cmd.Parameters.AddWithValue("@SenderPassword", _SenderPassword);
                        cmd.Parameters.AddWithValue("@SenderSmtpClient", _SenderSmtpClient);
                        cmd.Parameters.AddWithValue("@SenderPort", _SenderPort);

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
// DELETE BANK
        //
        public void DeleteBankEntry(string InBankId)
        {
            
            _ErrorFound = false;
            _ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[BANKS] "
                            + " WHERE BankId = @BankId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

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

            _ErrorFound = false;
            _ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ErrorsIdCharacteristics] "
                            + " WHERE BankId = @BankId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

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

    }
}



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

namespace RRDM4ATMsWin
{
    class RRDMBanks
    {
        public string BankSwiftId;

        public string ActiveDirectoryDM;
        public string AdGroup; 

        public string BankName;
        public string BankCountry;

        public string GroupName; 

        public string BasicCurName;

        public DateTime DtTmCreated;
        public bool UsingGAS;

        public int BanksInGroup;

        public byte[] Logo;

        public DateTime LastMatchingDtTm; // This date is checked during sign in
                                          // If Today is bigger then we run the matching process 
                                          // of Trans to be posted with Host

        public string SenderEmail; // the Banks Sender email
        public string SenderUserName;
        public string SenderPassword;
        public string SenderSmtpClient;
        public int SenderPort; 

        public string Operator; 

        public int BanksWithOperator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ BANK 

        public void ReadBank(string InBankSwiftId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[BANKS] "
          + " WHERE BankSwiftId = @BankSwiftId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankSwiftId", InBankSwiftId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Bank Details
                            BankSwiftId = (string)rdr["BankSwiftId"];

                            ActiveDirectoryDM = (string)rdr["ActiveDirectoryDM"];
                            AdGroup = (string)rdr["AdGroup"];
                
                            BankName = (string)rdr["BankName"];
                            BankCountry = (string)rdr["BankCountry"];
                            
                            GroupName = (string)rdr["GroupName"];
                            BasicCurName = (string)rdr["BasicCurName"];
                            DtTmCreated = (DateTime)rdr["DtTmCreated"];
                            UsingGAS = (bool)rdr["UsingGAS"];
                            Logo = (byte[])rdr["Logo"];

                            LastMatchingDtTm = (DateTime)rdr["LastMatchingDtTm"];

                            SenderEmail = (string)rdr["SenderEmail"];
                            SenderUserName = (string)rdr["SenderUserName"];
                            SenderPassword = (string)rdr["SenderPassword"];
                            SenderSmtpClient = (string)rdr["SenderSmtpClient"];
                            SenderPort = (int)rdr["SenderPort"];
                            
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
                    ErrorOutput = "An error occured in Banks Class............. " + ex.Message;

                }
        }

        // READ Operator

        public void ReadOperator(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

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
                            RecordFound = true;

                            // Read Bank Details
                            BankSwiftId = (string)rdr["BankSwiftId"];

                            ActiveDirectoryDM = (string)rdr["ActiveDirectoryDM"];
                            AdGroup = (string)rdr["AdGroup"];
                        
                            BankName = (string)rdr["BankName"];
                            BankCountry = (string)rdr["BankCountry"];

                            GroupName = (string)rdr["GroupName"];
                          
                            BasicCurName = (string)rdr["BasicCurName"];
                            DtTmCreated = (DateTime)rdr["DtTmCreated"];
                            UsingGAS = (bool)rdr["UsingGAS"];

                            Logo = (byte[])rdr["Logo"];

                            LastMatchingDtTm = (DateTime)rdr["LastMatchingDtTm"];

                            SenderEmail = (string)rdr["SenderEmail"];
                            SenderUserName = (string)rdr["SenderUserName"];
                            SenderPassword = (string)rdr["SenderPassword"];
                            SenderSmtpClient = (string)rdr["SenderSmtpClient"];
                            SenderPort = (int)rdr["SenderPort"];
       
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
                    ErrorOutput = "An error occured in Banks Class............. " + ex.Message;

                }
        }

        // READ Active Directory 

        public void ReadBankActiveDirectory(string InActiveDirectoryDM)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
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
                            RecordFound = true;

                            // Read Bank Details
                            BankSwiftId = (string)rdr["BankSwiftId"];

                            ActiveDirectoryDM = (string)rdr["ActiveDirectoryDM"];
                            AdGroup = (string)rdr["AdGroup"];

                            BankName = (string)rdr["BankName"];
                            BankCountry = (string)rdr["BankCountry"];

                            GroupName = (string)rdr["GroupName"];

                            BasicCurName = (string)rdr["BasicCurName"];
                            DtTmCreated = (DateTime)rdr["DtTmCreated"];
                            UsingGAS = (bool)rdr["UsingGAS"];

                            Logo = (byte[])rdr["Logo"];

                            LastMatchingDtTm = (DateTime)rdr["LastMatchingDtTm"];

                            SenderEmail = (string)rdr["SenderEmail"];
                            SenderUserName = (string)rdr["SenderUserName"];
                            SenderPassword = (string)rdr["SenderPassword"];
                            SenderSmtpClient = (string)rdr["SenderSmtpClient"];
                            SenderPort = (int)rdr["SenderPort"];

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
                    ErrorOutput = "An error occured in Banks Class............. " + ex.Message;

                }
        }
        // READ What is the Number of Banks in Group of Banks  

        public void ReadNoBanksInGroup(string InGroupName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            BanksInGroup = 0; 
          
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
                            RecordFound = true;

                            BanksInGroup = BanksInGroup + 1; 
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
                    ErrorOutput = "An error occured in Banks Class............. " + ex.Message;

                }
        }

        // READ What is the Number of Banks in Operator

        public void ReadNoBanksWithOperator(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            BanksWithOperator = 0;
         
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
                            RecordFound = true;

                            BanksWithOperator = BanksWithOperator + 1;
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
                    ErrorOutput = "An error occured in Banks Class............. " + ex.Message;

                }
        }
        // Insert NEW BANK 
        //
        public void InsertBank(string InBankSwiftId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [BANKS]"
                + " ([BankSwiftId],[ActiveDirectoryDM],[AdGroup],[BankName],[BankCountry],"
                + " [GroupName],"
                + "[BasicCurName], "
                + "[DtTmCreated],[UsingGAS],[Logo],"
                + "[SenderEmail],[SenderUserName],[SenderPassword],[SenderSmtpClient],[SenderPort],"
                + "[Operator]  ) "
                + " VALUES (@BankSwiftId,@ActiveDirectoryDM,@AdGroup,@BankName,@BankCountry,"
                + "@GroupName,"
                + "@BasicCurName,"
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

                        cmd.Parameters.AddWithValue("@BankSwiftId", BankSwiftId);

                        cmd.Parameters.AddWithValue("@ActiveDirectoryDM", ActiveDirectoryDM);
                        cmd.Parameters.AddWithValue("@AdGroup", AdGroup);
                
                        cmd.Parameters.AddWithValue("@BankName", BankName);
                        cmd.Parameters.AddWithValue("@BankCountry", BankCountry);

                        cmd.Parameters.AddWithValue("@GroupName", GroupName);

                        cmd.Parameters.AddWithValue("@BasicCurName", BasicCurName);
                        cmd.Parameters.AddWithValue("@DtTmCreated", DtTmCreated);
                        cmd.Parameters.AddWithValue("@UsingGAS", UsingGAS);
                        cmd.Parameters.AddWithValue("@Logo", Logo);

                        cmd.Parameters.AddWithValue("@SenderEmail", SenderEmail);
                        cmd.Parameters.AddWithValue("@SenderUserName", SenderUserName);
                        cmd.Parameters.AddWithValue("@SenderPassword", SenderPassword);
                        cmd.Parameters.AddWithValue("@SenderSmtpClient", SenderSmtpClient);
                        cmd.Parameters.AddWithValue("@SenderPort", SenderPort);
                       
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Banks Class............. " + ex.Message;

                }
        }


        // UPDATE BANK
        // 
        public void UpdateBank(string InBankSwiftId)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE BANKS SET "
                            + " BankSwiftId = @BankSwiftId, ActiveDirectoryDM = @ActiveDirectoryDM, AdGroup = @AdGroup,"
                             + "BankName = @BankName, BankCountry = @BankCountry,"
                             + "GroupName = @GroupName,"
                             + "BasicCurName = @BasicCurName,"
                             + "DtTmCreated = @DtTmCreated, UsingGAS = @UsingGAS, Logo = @Logo,  LastMatchingDtTm = @LastMatchingDtTm, "
                              + "SenderEmail = @SenderEmail, SenderUserName = @SenderUserName, SenderPassword = @SenderPassword,"
                              + " SenderSmtpClient = @SenderSmtpClient, SenderPort = @SenderPort "
                            + " WHERE BankSwiftId = @BankSwiftId", conn))

                    {
                        cmd.Parameters.AddWithValue("@BankSwiftId", InBankSwiftId);

                        cmd.Parameters.AddWithValue("@ActiveDirectoryDM", ActiveDirectoryDM);
                        cmd.Parameters.AddWithValue("@AdGroup", AdGroup);
                    
                        cmd.Parameters.AddWithValue("@BankName", BankName);
                        cmd.Parameters.AddWithValue("@BankCountry", BankCountry);
                      
                        cmd.Parameters.AddWithValue("@GroupName", GroupName);
                    //    cmd.Parameters.AddWithValue("@BasicCurCode", BasicCurCode);
                        cmd.Parameters.AddWithValue("@BasicCurName", BasicCurName);
                        cmd.Parameters.AddWithValue("@DtTmCreated", DtTmCreated);
                        cmd.Parameters.AddWithValue("@UsingGAS", UsingGAS);

                        cmd.Parameters.AddWithValue("@Logo", Logo);

                        cmd.Parameters.AddWithValue("@LastMatchingDtTm", LastMatchingDtTm);

                        cmd.Parameters.AddWithValue("@SenderEmail", SenderEmail);
                        cmd.Parameters.AddWithValue("@SenderUserName", SenderUserName);
                        cmd.Parameters.AddWithValue("@SenderPassword", SenderPassword);
                        cmd.Parameters.AddWithValue("@SenderSmtpClient", SenderSmtpClient);
                        cmd.Parameters.AddWithValue("@SenderPort", SenderPort);
                  
                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Banks Class............. " + ex.Message;

                }
        }
        //
// DELETE BANK
        //
        public void DeleteBankEntry(string InBankId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[BANKS] "
                            + " WHERE BankSwiftId = @BankSwiftId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankSwiftId", InBankId);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Banks Class............. " + ex.Message;

                }
        }
    }
}

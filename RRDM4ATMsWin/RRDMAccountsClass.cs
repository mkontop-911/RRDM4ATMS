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
    class RRDMAccountsClass
    {      
        // 
        // 1) Input Accounts
        // 2) Read Accounts
        //
        public int SeqNumber;

        public string AccNo;
        public string BankId;
 
        public string CurrNm;
        public string AccName; 
        public string AtmNo;
        public string UserId;
        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        //
        // READ ACCOUNT BASED ON CRITERIA  
        //
        public void ReadAndFindAccount(string InUserId, string InOperator, string InAtmNo, string InCurrNm, string InAccName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // When we put 1000 we want to get the accounts for ATM 
            // If different than 1000 we get the accounts fot the CIT 

            string SqlString = "SELECT *"
               + " FROM [dbo].[AccountsTable]"
               + " WHERE UserId = @UserId AND Operator=@Operator AND AtmNo =@AtmNo AND CurrNm=@CurrNm AND AccName = @AccName";

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
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@CurrNm", InCurrNm);
                        cmd.Parameters.AddWithValue("@AccName", InAccName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            
                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            AccNo = (string)rdr["AccNo"];
                            BankId = (string)rdr["BankId"];
          
                            CurrNm = (string)rdr["CurrNm"];
                            AccName = (string)rdr["AccName"];
                            AtmNo = (string)rdr["AtmNo"];

                            UserId = (string)rdr["UserId"];

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
                    ErrorOutput = "An error occured in Accounts Class............. " + ex.Message;
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

            string SqlString = "SELECT *"
               + " FROM [dbo].[AccountsTable]"
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

                            SeqNumber = (int)rdr["SeqNumber"];

                            AccNo = (string)rdr["AccNo"];
                            BankId = (string)rdr["BankId"];

                            CurrNm = (string)rdr["CurrNm"];
                            AccName = (string)rdr["AccName"];
                            AtmNo = (string)rdr["AtmNo"];

                            UserId = (string)rdr["UserId"];

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
                    ErrorOutput = "An error occured in Accounts Class............. " + ex.Message;
                }
        }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL BY SEQ NUMBER 
        //
        public void ReadAndFindAccountBySeqNo(int InSeqNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
               + " FROM [dbo].[AccountsTable]"
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

                            SeqNumber = (int)rdr["SeqNumber"];

                            AccNo = (string)rdr["AccNo"];
                            BankId = (string)rdr["BankId"];

                            CurrNm = (string)rdr["CurrNm"];
                            AccName = (string)rdr["AccName"];
                            AtmNo = (string)rdr["AtmNo"];

                            UserId = (string)rdr["UserId"];

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
                    ErrorOutput = "An error occured in Accounts Class............. " + ex.Message;
                }
        }
        // Insert Acc no
        //
        public void InsertAccount()
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [dbo].[AccountsTable]"
                    + "([AccNo], [BankId],  "
                    + " [CurrNm], [AccName], [AtmNo], [UserId], [Operator] )"
                    + " VALUES (@AccNo, @BankId,"
                    + "@CurrNm, @AccName, @AtmNo, @UserId, @Operator )";

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
                         cmd.Parameters.AddWithValue("@AccName", AccName);

                         cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                         cmd.Parameters.AddWithValue("@UserId", UserId);

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
                    ErrorOutput = "An error occured in Accounts Class............. " + ex.Message;
                }
        }

        // UPDATE 
        // 
        public void UpdateAccount(int InSeqNumber, string InOperator)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[AccountsTable] SET "
                            + " AccNo = @AccNo, BankId = @BankId, "
                            + " CurrNm = @CurrNm, AccName = @AccName, AtmNo = @AtmNo, UserId = @UserId"
                            + " WHERE SeqNumber = @SeqNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@CurrNm", CurrNm);
                        cmd.Parameters.AddWithValue("@AccName", AccName);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

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
                    ErrorOutput = "An error occured in Accounts Class............. " + ex.Message;
                }
        }

        //
        // DELETE AN ACCOUNT  
        //
        public void DeleteAccount(int InSeqNumber)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[AccountsTable] "
                            + " WHERE SeqNumber =  @SeqNumber ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

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
                    ErrorOutput = "An error occured in Accounts Class............. " + ex.Message;
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

            string SqlString = "SELECT *"
               + " FROM [dbo].[AccountsTable]"
               + " WHERE Operator = @Operator AND AtmNo = @AtmNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperatorA);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmA);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            // ===============================
                            // ========Read Account from ATMA ========

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            AccNo = (string)rdr["AccNo"];
                            BankId = (string)rdr["BankId"];

                    //        Prive = (bool)rdr["Prive"];

                            CurrNm = (string)rdr["CurrNm"];
                            AccName = (string)rdr["AccName"];
                            AtmNo = (string)rdr["AtmNo"];

                            UserId = (string)rdr["UserId"];



                            // ======================================
                            // ========Insert Account to ATMB========
                            
                            AtmNo = InAtmB;
                            Operator = TargetOperatorB; 
                            InsertAccount(); 

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
                    ErrorOutput = "An error occured in Accounts Class............. " + ex.Message;
                }
        }

    }
}

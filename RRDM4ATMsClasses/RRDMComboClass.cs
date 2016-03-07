using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;

namespace RRDM4ATMs
{
    public class RRDMComboClass
    {
        public string BankSwiftId;
        public string UserId;
        public string UserType;
        public string CurrNm; 
        public string AccName;
        //public string CitId;
        public int GroupNo;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;
        //
        // GET BANKS belonging to Seed and Seed Bank too
        //
        public ArrayList GetBanksIds(string InOperator)
        {
            ArrayList BanksIdsList = new ArrayList();

     //       BanksIdsList.Add("");

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
                            BankSwiftId = (string)rdr["BankSwiftId"];
                            BanksIdsList.Add(BankSwiftId);
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }

            return BanksIdsList;
        }


// Get Cit Ids 
        public ArrayList GetCitIds(string InOperator)
        {
            ArrayList CitNosList = new ArrayList();
          //  CitNosList.Add("Own");

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [dbo].[UsersTable] WHERE Operator = @Operator "
                               + " AND (UserType = 'CIT Company' OR UserType = 'Operator Entity') "
                                +" ORDER BY  UserId";
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
                            UserId = (string)rdr["UserId"];
                            CitNosList.Add(UserId);
                            UserType = rdr["UserType"].ToString();
                          
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }

            return CitNosList;
        }
        // GET User ACCOUNTS 
        public ArrayList GetUserAccs(string InOperator, string InUserId)
        {
            ArrayList UserAccsList = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT DISTINCT [AccName] FROM [dbo].[AccountsTable] WHERE Operator = @Operator AND UserId = @UserId";
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
                        
                            AccName = (string)rdr["AccName"];
                        
                            UserAccsList.Add(AccName); 
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }

            return UserAccsList;
        }

        // GET User ACCOUNTS currencies 
        public ArrayList GetUserAccsCurr(string InOperator, string InUserId)
        {
            ArrayList UserAccsCurrList = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT DISTINCT [CurrNm] FROM [dbo].[AccountsTable] WHERE Operator = @Operator AND UserId = @UserId";
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
                           
                            CurrNm = (string)rdr["CurrNm"];
                         
                            UserAccsCurrList.Add(CurrNm);
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }

            return UserAccsCurrList;
        }

        // GET ATM ACCOUNTS 
        public ArrayList GetAtmAccs(string InOperator, string InAtmNo)
        {
            ArrayList AtmAccsList = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT DISTINCT [AccName] FROM [dbo].[AccountsTable] WHERE Operator = @Operator AND AtmNo = @AtmNo";
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

                            AccName = (string)rdr["AccName"];

                            AtmAccsList.Add(AccName);
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }

            return AtmAccsList;
        }

        // GET ATM ACCOUNTS currencies 
        public ArrayList GetAtmAccsCurr(string InOperator, string InAtmNo)
        {
            ArrayList AtmAccsCurrList = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT DISTINCT [CurrNm] FROM [dbo].[AccountsTable] WHERE Operator = @Operator AND AtmNo = @AtmNo";
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

                            CurrNm = (string)rdr["CurrNm"];

                            AtmAccsCurrList.Add(CurrNm);
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }

            return AtmAccsCurrList;
        }
        // GET ATM Categories 
        public ArrayList GetAtmCategories(string InOperator)
        {
            ArrayList AtmCategoriesList = new ArrayList();

            AtmCategoriesList.Add(0);

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [ATMS].[dbo].[Groups] WHERE Operator = @Operator AND Stats = 1 ";
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
                            // Read GROUP Details
                            GroupNo = (int)rdr["GroupNo"];
                            AtmCategoriesList.Add(GroupNo);
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }

            return AtmCategoriesList;
        }

        // GET ATM Replenishment groups  
        public ArrayList GetAtmsReplGroups(string InOperator)
        {
            ArrayList AtmsReplGroupsList = new ArrayList();

            AtmsReplGroupsList.Add(0);

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [ATMS].[dbo].[Groups] WHERE Replenishment = 1 ";
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
                            // Read GROUP Details
                            GroupNo = (int)rdr["GroupNo"];
                            AtmsReplGroupsList.Add(GroupNo);
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }

            return AtmsReplGroupsList;
        }

        // GET ATM Reconc groups  
        public ArrayList GetAtmsReconcGroups(string InOperator)
        {
            ArrayList AtmsReconcGroupsList = new ArrayList();

            AtmsReconcGroupsList.Add(0);

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [ATMS].[dbo].[Groups] WHERE Reconciliation = 1 ";
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
                            // Read GROUP Details
                            GroupNo = (int)rdr["GroupNo"];
                            AtmsReconcGroupsList.Add(GroupNo);
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }

            return AtmsReconcGroupsList;
        }
    }
}

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

namespace RRDM4ATMs
{
    public class RRDMGroups
    {
       // DECLARE GROUP FIELDS 
        public int GroupNo;
        public string BankId;
    
        public bool MoreThanOneBank;
        public bool Stats;
        public bool Replenishment;
        public bool Reconciliation;
        public string Description;
        public DateTime DtTmCreated;
        public bool Inactive;
        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ Group 

        public void ReadGroup(int InGroupNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[Groups] "
          + " WHERE GroupNo = @GroupNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", InGroupNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read GROUP Details
                            GroupNo = (int)rdr["GroupNo"];
                            BankId = (string)rdr["BankId"];
                   
                            MoreThanOneBank = (bool)rdr["MoreThanOneBank"];
                            Stats = (bool)rdr["Stats"];
                            Replenishment = (bool)rdr["Replenishment"];
                            Reconciliation = (bool)rdr["Reconciliation"];
                            Description = (string)rdr["Description"];
                            DtTmCreated = (DateTime)rdr["DtTmCreated"];
                            Inactive = (bool)rdr["Inactive"];
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
                    ErrorOutput = "An error occured in Groups Class............. " + ex.Message;

                }
        }
        //
        // READ Group's last Number Inserted  
        // USE BANK ID as an identification key 
        //
        public void ReadGroupLastNo()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM    Groups"
                   + " WHERE   GroupNo = (SELECT MAX(GroupNo)  FROM Groups)"; 
        
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

                            // Read GROUP Details
                            GroupNo = (int)rdr["GroupNo"];
                            BankId = (string)rdr["BankId"];
                    //        Prive = (bool)rdr["Prive"];
                            MoreThanOneBank = (bool)rdr["MoreThanOneBank"];
                            Stats = (bool)rdr["Stats"];
                            Replenishment = (bool)rdr["Replenishment"];
                            Reconciliation = (bool)rdr["Reconciliation"];
                            Description = (string)rdr["Description"];
                            DtTmCreated = (DateTime)rdr["DtTmCreated"];
                            Inactive = (bool)rdr["Inactive"];
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
                    ErrorOutput = "An error occured in Groups Class............. " + ex.Message;

                }
        }
        // Insert NEW Group 
        //
        public void InsertGroup()
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [Groups]"
                + " ([BankId],[MoreThanOneBank],[Stats],[Replenishment],[Reconciliation],"
                 + " [Description],[DtTmCreated],[Inactive], [Operator])"
                + " VALUES(@BankId,@MoreThanOneBank,@Stats,@Replenishment,@Reconciliation,"
                + "@Description,@DtTmCreated,@Inactive,@Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                
                        cmd.Parameters.AddWithValue("@MoreThanOneBank", MoreThanOneBank);
                        cmd.Parameters.AddWithValue("@Stats", Stats);
                        cmd.Parameters.AddWithValue("@Replenishment", Replenishment);
                        cmd.Parameters.AddWithValue("@Reconciliation", Reconciliation);

                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@DtTmCreated", DtTmCreated);
                        cmd.Parameters.AddWithValue("@Inactive", Inactive);
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
                    ErrorOutput = "An error occured in Groups Class............. " + ex.Message;
                }
        }


        // UPDATE Group
        // 
        public void UpdateGroup(int InGroupNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE Groups SET "
                            + " BankId = @BankId,"
                             + "MoreThanOneBank = @MoreThanOneBank,"
                             + "Stats = @Stats,Replenishment = @Replenishment,Reconciliation = @Reconciliation,"
                             + "Description = @Description,DtTmCreated = @DtTmCreated, Inactive = @Inactive"
                            + " WHERE GroupNo = @GroupNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", GroupNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
             
                        cmd.Parameters.AddWithValue("@MoreThanOneBank", MoreThanOneBank);
                        cmd.Parameters.AddWithValue("@Stats", Stats);
                        cmd.Parameters.AddWithValue("@Replenishment", Replenishment);
                        cmd.Parameters.AddWithValue("@Reconciliation", Reconciliation);

                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@DtTmCreated", DtTmCreated);
                        cmd.Parameters.AddWithValue("@Inactive", Inactive);


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
                    ErrorOutput = "An error occured in Groups Class............. " + ex.Message;

                }
        }
        // DELETE Group  
        //
        public void DeleteGroupsEntry(int InGroupNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[Groups] "
                            + " WHERE GroupNo = @GroupNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", InGroupNo);
                        
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
                    ErrorOutput = "An error occured in Groups Class............. " + ex.Message;

                }
        }
    }
}

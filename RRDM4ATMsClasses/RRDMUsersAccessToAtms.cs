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
    public class RRDMUsersAccessToAtms
    {
        // Declarations 

        // Relation of Users to Atms Table 

        public string UserId;
        public string AtmNo;
        public int GroupOfAtms;
        public bool UseOfGroup;
        
        public bool Replenishment;
        public bool Reconciliation;

        public DateTime DateOfInsert; 

        public int NoOfAtmsRepl;
        public int NoOfAtmsReconc;

        public int NoOfGroupsRepl;
        public int NoOfGroupsReconc; 

        // ATM INFORMATION 

        public string AtmName ;
        public string BankId ;
 
        public string RespBranch ;
        public int AtmsStatsGroup ;
        public int AtmsReplGroup ;
        public int AtmsReconcGroup ;
        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // READ User RECORD 
        public void ReadUsersAccessAtmTable(string InUserId)
        {
            NoOfAtmsRepl = 0;
            NoOfAtmsReconc = 0; 
            NoOfGroupsRepl = 0;
            NoOfGroupsReconc = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[UsersAtmTable] "
          + " WHERE UserId = @UserId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details

                            UserId = (string)rdr["UserId"];

                            AtmNo = (string)rdr["AtmNo"];
                            GroupOfAtms = (int)rdr["GroupOfAtms"];
                            BankId = (string)rdr["BankId"];
                   
                            UseOfGroup = (bool)rdr["UseOfGroup"];
                            Replenishment = (bool)rdr["Replenishment"];
                            Reconciliation = (bool)rdr["Reconciliation"];
                            DateOfInsert = (DateTime)rdr["DateOfInsert"]; 

                            if (UseOfGroup == false & AtmNo != "") // User do not have a group 
                            {
                                if (Replenishment == true) NoOfAtmsRepl = NoOfAtmsRepl + 1;
                                if (Reconciliation == true) NoOfAtmsReconc = NoOfAtmsReconc + 1; 
                            }
                            if (UseOfGroup == true & AtmNo == "") // User has Group/s 
                            {
                                if (Replenishment == true) NoOfGroupsRepl = NoOfGroupsRepl + 1;
                                if (Reconciliation == true) NoOfGroupsReconc = NoOfGroupsReconc + 1;
                            }

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
                    ErrorOutput = "An error occured in UsersAccessToAtms Class............. " + ex.Message;

                }
        }
        // READ SPECIFIC User RELATION RECORD 
        public void ReadUsersAccessAtmTableSpecific(string InUserId, string InAtmNo, int InGroupOfAtms)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[UsersAtmTable] "
          + " WHERE UserId = @UserId AND AtmNo = @AtmNo AND GroupOfAtms = @GroupOfAtms";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details

                            UserId = (string)rdr["UserId"];

                            AtmNo = (string)rdr["AtmNo"];
                            GroupOfAtms = (int)rdr["GroupOfAtms"];
                            BankId = (string)rdr["BankId"];
                      //      Prive = (bool)rdr["Prive"];
                            UseOfGroup = (bool)rdr["UseOfGroup"];
                            Replenishment = (bool)rdr["Replenishment"];
                            Reconciliation = (bool)rdr["Reconciliation"];
                            DateOfInsert = (DateTime)rdr["DateOfInsert"];

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
                    ErrorOutput = "An error occured in UsersAccessToAtms Class............. " + ex.Message;

                }
        }
        // Insert USER ATM RELATION 
        //
        public void InsertUsersAtmTable(string InUserId, string InAtmNo, int InGroupOfAtms)
        {
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [UsersAtmTable]"
                + " ([UserId],[AtmNo],[GroupOfAtms],[BankId],"
                + "[UseOfGroup],[Replenishment],[Reconciliation],[DateOfInsert], [Operator] )"
                + " VALUES (@UserId,@AtmNo,@GroupOfAtms,@BankId, @UseOfGroup,"
                + "@Replenishment,@Reconciliation,@DateOfInsert, @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
          

                        cmd.Parameters.AddWithValue("@UseOfGroup", UseOfGroup);
                        cmd.Parameters.AddWithValue("@Replenishment", Replenishment);
                        cmd.Parameters.AddWithValue("@Reconciliation", Reconciliation);
                        cmd.Parameters.AddWithValue("@DateOfInsert", DateOfInsert);

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
                    ErrorOutput = "An error occured in UsersAccessToAtms Class............. " + ex.Message;
                }
        }


        // UPDATE USER TO ATMs Or Group of ATMs Table
        //
        public void UpdateUsersAtmTable(string InUserId, string InAtmNo, int InGroupOfAtms)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE UsersAtmTable SET "
                            + " UserId = @UserId,AtmNo = @AtmNo,GroupOfAtms = @GroupOfAtms,BankId = @BankId,"
                             + "UseOfGroup = @UseOfGroup,Replenishment = @Replenishment, Reconciliation = @Reconciliation, DateOfInsert = @DateOfInsert"
                            + " WHERE UserId = @UserId AND AtmNo = @AtmNo AND GroupOfAtms = @GroupOfAtms", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
               

                        cmd.Parameters.AddWithValue("@UseOfGroup", UseOfGroup);
                        cmd.Parameters.AddWithValue("@Replenishment", Replenishment);
                        cmd.Parameters.AddWithValue("@Reconciliation", Reconciliation);
                        cmd.Parameters.AddWithValue("@DateOfInsert", DateOfInsert);

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
                    ErrorOutput = "An error occured in UsersAccessToAtms Class............. " + ex.Message;

                }
        }
        // DELETE USER TO ATMs Or Group of ATMs Table entry 
        //
        public void DeleteUsersAtmTableEntry(string InUserId, string InAtmNo, int InGroupOfAtms)
        {
            ErrorFound = false;
            ErrorOutput = ""; 
            
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[UsersAtmTable] "
                            + " WHERE UserId = @UserId AND AtmNo = @AtmNo AND GroupOfAtms = @GroupOfAtms", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);

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
                    ErrorOutput = "An error occured in UsersAccessToAtms Class............. " + ex.Message;
                }
        }
        //
        // READ ATM to FIND its GROUP If ANY 
        //
        public void FindIfAtmHasGroup (string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string stringlSelect = "SELECT * "
                 + " FROM TableATMsBasic"
                 + " WHERE AtmNo=@AtmNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(stringlSelect, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            // Assign Values
                            RecordFound = true;

                            AtmName = (string)rdr["AtmName"];
                            BankId = (string)rdr["BankId"];
                      //      Prive = (bool)rdr["Prive"];
                            RespBranch = (string)rdr["Branch"];
                            AtmsStatsGroup = (int)rdr["AtmsStatsGroup"];
                            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
                            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];
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
                    ErrorOutput = "An error occured in UsersAccessToAtms Class............. " + ex.Message;

                }
        }

        //
        // Through ATM find user of replenishment 
        //
        public void FindUserForRepl(string InAtmNo, int InGroupOfAtms)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[UsersAtmTable] "
          + " WHERE AtmNo = @AtmNo AND GroupOfAtms = @GroupOfAtms";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                     
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details

                            UserId = (string)rdr["UserId"];

                            AtmNo = (string)rdr["AtmNo"];
                            GroupOfAtms = (int)rdr["GroupOfAtms"];
                            BankId = (string)rdr["BankId"];
              
                            UseOfGroup = (bool)rdr["UseOfGroup"];
                            Replenishment = (bool)rdr["Replenishment"];
                            Reconciliation = (bool)rdr["Reconciliation"];
                            DateOfInsert = (DateTime)rdr["DateOfInsert"];
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
                    ErrorOutput = "An error occured in UsersAccessToAtms Class............. " + ex.Message;

                }
        }
    }
}

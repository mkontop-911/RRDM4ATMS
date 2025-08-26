using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMGroups :Logger
    {
        public RRDMGroups() : base() { }

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

        public DataTable TableGroupsOfAtms = new DataTable();

        RRDMAtmsClass Ac = new RRDMAtmsClass(); 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // Action Fields
        private void ReadGroupFields(SqlDataReader rdr)
        {
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

                            ReadGroupFields(rdr);

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

        // Read Groups and fill table 
        //
        public void ReadGroupsAndFillTable(string InSelectionCriteria)
        {
           
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableGroupsOfAtms = new DataTable();
            TableGroupsOfAtms.Clear();

            string SqlString = "SELECT *"
                + " FROM [dbo].[Groups] "
                + InSelectionCriteria;

            using (SqlConnection conn =
           new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableGroupsOfAtms);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            Ac.ReadAtm("00000508"); 

            if (Ac.HasErrors)
            {
                HasErrors = true;
                ErrorDetails = Ac.ErrorDetails;
                return; 
            }
            
        }

        // GET Array List Occurance Nm 
        //
        public ArrayList GetGroupNosList(string InOperator)
        {
            ArrayList GroupNosList = new ArrayList();

            //GroupNosList.Add("NoGroup");

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                + " FROM [dbo].[Groups] "
                + " WHERE Operator = @Operator AND Reconciliation = 1 AND Inactive = 0 ";

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

                            GroupNo = (int)rdr["GroupNo"];

                            GroupNosList.Add(GroupNo);


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

            return GroupNosList;
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
                            ReadGroupFields(rdr);

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

        // Insert  Group 
        // This was built to count groups during loading 
        public void InsertGroupDuringLoading(int InGroupNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[Group_Count]"
                + " ([GroupNo]"
                 + " ,[DateTimeStart]) "
                + " VALUES(@GroupNo "
                + " ,@DateTimeStart)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@GroupNo", InGroupNo);

                        cmd.Parameters.AddWithValue("@DateTimeStart", DateTime.Now);
                  
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

        // DELETE Group from counter 
        //
        public void DeleteGroupFromCounter(int InGroupNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[Group_Count] "
                            + " WHERE GroupNo = @GroupNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", InGroupNo);

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

        //        SELECT COUNT(*)
        //FROM orders;
        // READ Group 
        public int Total; 
        public int ReadRowsInGroupTable()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Total = 0; 

            string SqlString = "SELECT COUNT(*) As Total"
          + " FROM [ATMS].[dbo].[Group_Count] "; 
     
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       // cmd.Parameters.AddWithValue("@GroupNo", InGroupNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read GROUP Details
                            Total = (int)rdr["Total"]; 

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
            return Total; 
        }

        // Catch Details
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
        //    //    Environment.Exit(0);
        //}
    }
}

using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;


namespace RRDM4ATMs
{
    public class RRDMUsersAccessToAtms : Logger
    {
        public RRDMUsersAccessToAtms() : base() { }
        // Declarations 

        public string UserId;

        public string AtmNo;
        public int GroupOfAtms;
        public bool IsCit;
        public string BankId;

        public bool UseOfGroup;       
        public bool Replenishment;
        public bool Reconciliation;
        public DateTime DateOfInsert;

        public string Operator;

        public int NoOfAtmsRepl;
        public int NoOfAtmsReconc;

        public int NoOfGroupsRepl;
        public int NoOfGroupsReconc; 

        // ATM INFORMATION 

        public string AtmName ;
 
        public string RespBranch ;
        public int AtmsStatsGroup ;
        public int AtmsReplGroup ;
        public int AtmsReconcGroup ;
   
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public DataTable UsersToAtmsDataTable = new DataTable();

        public DataTable UserGroups_ToAtms_Table = new DataTable();

        public DataTable ATMsForThisGroup = new DataTable(); 

        public int TotalSelected;
        string SqlString; // Do not delete

        readonly string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Read Fields of Users vs ATMs
        private void ReadUsersToAtms(SqlDataReader rdr)
        {
            // Read USER To ATMs Details

            UserId = (string)rdr["UserId"];

            AtmNo = (string)rdr["AtmNo"];
            GroupOfAtms = (int)rdr["GroupOfAtms"];
            IsCit = (bool)rdr["IsCit"];
            BankId = (string)rdr["BankId"];

            UseOfGroup = (bool)rdr["UseOfGroup"];
            Replenishment = (bool)rdr["Replenishment"];
            Reconciliation = (bool)rdr["Reconciliation"];
            DateOfInsert = (DateTime)rdr["DateOfInsert"];

            Operator = (string)rdr["Operator"];
        }


        // READ ATMs Based on User RECORD 
        public void ReadUserAccessToAtmsFillTable(string InFilter)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            UsersToAtmsDataTable = new DataTable();
            UsersToAtmsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            UsersToAtmsDataTable.Columns.Add("UserId", typeof(string));
            UsersToAtmsDataTable.Columns.Add("AtmNo", typeof(string));
            UsersToAtmsDataTable.Columns.Add("AtmName", typeof(string));
            UsersToAtmsDataTable.Columns.Add("GroupOfAtms", typeof(int));
            UsersToAtmsDataTable.Columns.Add("Replenishment", typeof(bool));
            UsersToAtmsDataTable.Columns.Add("Reconciliation", typeof(bool));
            UsersToAtmsDataTable.Columns.Add("DateOfInsert", typeof(string));
          
            SqlString = "SELECT * "
                         + " FROM [dbo].[UsersAtmTable] "
                         + " WHERE " + InFilter
                          + " Order by UserId ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@UserId", InUserId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details

                            ReadUsersToAtms(rdr);
                            //
                            //FILL IN TABLE
                            //

                            DataRow RowSelected = UsersToAtmsDataTable.NewRow();

                            RowSelected["UserId"] = UserId;

                            RowSelected["AtmNo"] = AtmNo;

                            Ac.ReadAtm(AtmNo); 

                            RowSelected["AtmName"] = Ac.AtmName;
                          
                            RowSelected["GroupOfAtms"] = GroupOfAtms;
                            RowSelected["Replenishment"] = Replenishment;
                            RowSelected["Reconciliation"] = Reconciliation;
                            RowSelected["DateOfInsert"] = DateOfInsert.ToString();
                           
                            // ADD ROW
                            UsersToAtmsDataTable.Rows.Add(RowSelected);

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
                    CatchDetails(ex);
                }
        }

      
        //RRDMReconcCategories Rc = new RRDMReconcCategories();
        //RRDMAtmsClass Ac = new RRDMAtmsClass(); 

      // READ All ATMs of his groups  for this User 

        public void ReadUserAccess_ToAtmsFillTable(string InOperator, string InUserId,string InAtmNo ,int InMode)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMGasParameters Gp = new RRDMGasParameters();

            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            //RRDMGasParameters Gp = new RRDMGasParameters();
            bool AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(InOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;


                }
                else
                {
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }


            // InMode = 1 Show All 
            // InMode = 2 Show Replenishemnt only 

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            UserGroups_ToAtms_Table = new DataTable();
            UserGroups_ToAtms_Table.Clear();

            // DATA TABLE ROWS DEFINITION 
            UserGroups_ToAtms_Table.Columns.Add("OwnerId", typeof(string));
            UserGroups_ToAtms_Table.Columns.Add("InNeed", typeof(bool));

            UserGroups_ToAtms_Table.Columns.Add("AtmNo", typeof(string));
            UserGroups_ToAtms_Table.Columns.Add("AtmName", typeof(string));

            if (AudiType == true)
            {
                //UserGroups_ToAtms_Table.Columns.Add("ExcelStatus", typeof(string));
            }

            UserGroups_ToAtms_Table.Columns.Add("Repl Pending", typeof(int));
            UserGroups_ToAtms_Table.Columns.Add("Repl Cycle", typeof(int));

            UserGroups_ToAtms_Table.Columns.Add("Branch", typeof(string));
            //UserGroups_ToAtms_Table.Columns.Add("BranchName", typeof(string));
            UserGroups_ToAtms_Table.Columns.Add("Cit_Id", typeof(string));
            UserGroups_ToAtms_Table.Columns.Add("GroupOfAtms", typeof(int));
            //
            // READ THE GROUPS FOR THIS OWNER 
            //
            string SqlString =""; 
            //  FROM [ATMS].[dbo].[UsersAtmTable]
            if (InMode == 1 & InAtmNo == "")
            {
                SqlString = "SELECT * "
                 + " FROM [ATMS].[dbo].[UsersAtmTable] "
                 + " WHERE UserId = @UserId  "
                 + "  ";
            }
            if (InMode == 1 & InAtmNo != "")
            {
                SqlString = "SELECT * "
                 + " FROM [ATMS].[dbo].[UsersAtmTable] "
                 + " WHERE UserId = @UserId AND AtmNo = @AtmNo "
                 + "  ";
            }
            if (InMode == 2)
            {
                SqlString = "SELECT *"
                 + " FROM [ATMS].[dbo].[UsersAtmTable] "
                 + " WHERE UserId = @UserId and Replenishment = 1 AND AtmNo NOT LIKE '%Model%' "
                 + "  ";
            }
            if (InMode == 2 & InOperator =="ALPHA_CY")
            {
                SqlString = "SELECT *"
                 + " FROM [ATMS].[dbo].[UsersAtmTable] "
                 + " WHERE UserId = @UserId and Replenishment = 1 AND AtmNo LIKE '%ATM%' AND AtmNo <>'HyosungATM' "
                 + "  ";
            }

            if (InMode == 4)
            {
                SqlString = "SELECT *"
                 + " FROM [ATMS].[dbo].[UsersAtmTable] "
                // + " WHERE Replenishment = 1 AND GroupOfAtms <> 0 AND AtmNo NOT LIKE '%Model%' "
                + " WHERE Replenishment = 1 AND GroupOfAtms <> 0 AND AtmNo NOT LIKE '%Model%' "
                 + " ORDER By UserId "
                 + "  ";
            }

            if (InMode == 5)
            {
                SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[UsersAtmTable] "
               + " WHERE AtmNo = @AtmNo "
               + "  ";
            }


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

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read  Details

                            ReadUsersToAtms(rdr);

                            Ac.ReadAtm(AtmNo); 

                            if (Ac.Operator == "BCAIEGCX")
                            {
                                // OK 
                                if (Ac.Branch == "015")
                                {
                                    // OK 
                                }
                                else
                                {
                                    // ELSE NOT OK and continue to the next 
                                    continue; 
                                }
                            }
                            else
                            {
                                // No Restriction 
                            }

                                //
                                //FILL IN TABLE
                                //

                                DataRow RowSelected = UserGroups_ToAtms_Table.NewRow();

                            RowSelected["OwnerId"] = UserId;

                            RowSelected["AtmNo"] = AtmNo;

                            RowSelected["AtmName"] = Ac.AtmName;

                            if (AtmNo == "01900501")
                            {
                                AtmNo = AtmNo;
                            }

                            Ta.FindNextAndLastReplCycleId(AtmNo);
                            if (Ta.RecordFound == true & Ta.ReplOutstanding > 0)
                            {
                               

                                RowSelected["Repl Pending"] = Ta.ReplOutstanding;
                                RowSelected["Repl Cycle"] = Ta.NextReplNo;
                                RowSelected["InNeed"] = true;
                            }
                            else
                            {
                                RowSelected["Repl Pending"] = 0;
                                RowSelected["Repl Cycle"] = 0;
                                RowSelected["InNeed"] = false;
                                if (Ta.RecordFound == true)
                                {
                                   // OK
                                }
                                else
                                {
                                    //if (AudiType == true)
                                    //RowSelected["ExcelStatus"] = "..Not Active ATM";
                                }
                            }

                            if (AudiType == true & Ta.RecordFound == true)
                            {
                                // Is used for the CIT EXCEL INPUT at the Finish of workflow 
                                // 0 It didnt pass through Excel 
                                // 2 is done by auto load and unload came and reconcile 
                                // 3 Load but Not Unload - wait 
                                // 4 Unload but not load - wait 
                                // 5 Should be made manually  


                                switch (Ta.NextLatestBatchNo)
                                {
                                    case 0:
                                        {

                                            //["ExcelStatus"] = "0: Excel_Not_Loaded";

                                            break;
                                        }
                                    case 2:
                                        {

                                            //RowSelected["ExcelStatus"] = "2: Auto DONE";

                                            break;
                                        }
                                    case 3:
                                        {

                                           // RowSelected["ExcelStatus"] = "3: Load but Not Unload - wait";

                                            break;
                                        }

                                    case 4:
                                        {

                                            //RowSelected["ExcelStatus"] = "4: Unload but not load - wait ";

                                            break;
                                        }
                                    case 5:
                                        {

                                           // RowSelected["ExcelStatus"] = "5: Should be made manually";

                                            break;
                                        }

                                    default:
                                        {
                                            //RowSelected["ExcelStatus"] = "Not Defined";
                                            break;
                                        }
                                }

                            }

                            RowSelected["Branch"] = Ac.Branch;
                            RowSelected["Cit_Id"] = Ac.CitId;

                            //RowSelected["Supplier"] = Supplier;

                            RowSelected["GroupOfAtms"] = GroupOfAtms;


                            // ADD ROW
                            UserGroups_ToAtms_Table.Rows.Add(RowSelected);

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

        // READ  ATMs for a specific group for this User 

        public void ReadUserAccess_ToAtmsFillTableUserAndGroup(string InUserId, int InGroupOfAtms)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            UserGroups_ToAtms_Table = new DataTable();
            UserGroups_ToAtms_Table.Clear();

            // DATA TABLE ROWS DEFINITION 
            UserGroups_ToAtms_Table.Columns.Add("OwnerId", typeof(string));
            UserGroups_ToAtms_Table.Columns.Add("InNeed", typeof(bool));

            UserGroups_ToAtms_Table.Columns.Add("AtmNo", typeof(string));
            UserGroups_ToAtms_Table.Columns.Add("AtmName", typeof(string));

            UserGroups_ToAtms_Table.Columns.Add("Repl Pending", typeof(int));
            UserGroups_ToAtms_Table.Columns.Add("Repl Cycle", typeof(int));

            UserGroups_ToAtms_Table.Columns.Add("Branch", typeof(string));
            //UserGroups_ToAtms_Table.Columns.Add("BranchName", typeof(string));
            UserGroups_ToAtms_Table.Columns.Add("Cit_Id", typeof(string));
            UserGroups_ToAtms_Table.Columns.Add("GroupOfAtms", typeof(int));
            //
            // READ THE GROUPS FOR THIS OWNER 
            //

            //  FROM [ATMS].[dbo].[UsersAtmTable]

            string SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[UsersAtmTable] "
                  + " WHERE UserId = @UserId AND GroupOfAtms = @GroupOfAtms "
                  + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read  Details

                            ReadUsersToAtms(rdr);

                            Ac.ReadAtm(AtmNo);

                            //
                            //FILL IN TABLE
                            //

                            DataRow RowSelected = UserGroups_ToAtms_Table.NewRow();

                            RowSelected["OwnerId"] = UserId;

                            RowSelected["AtmNo"] = AtmNo;

                            RowSelected["AtmName"] = Ac.AtmName;

                            Ta.FindNextAndLastReplCycleId(AtmNo);
                            if (Ta.RecordFound == true & Ta.ReplOutstanding > 0)
                            {
                                RowSelected["Repl Pending"] = Ta.ReplOutstanding;
                                RowSelected["Repl Cycle"] = Ta.NextReplNo;
                                RowSelected["InNeed"] = true;
                            }
                            else
                            {
                                RowSelected["Repl Pending"] = 0;
                                RowSelected["Repl Cycle"] = 0;
                                RowSelected["InNeed"] = false;
                            }

                            RowSelected["Branch"] = Ac.Branch;
                            RowSelected["Cit_Id"] = Ac.CitId;

                            //RowSelected["Supplier"] = Supplier;

                            RowSelected["GroupOfAtms"] = GroupOfAtms;


                            // ADD ROW
                            UserGroups_ToAtms_Table.Rows.Add(RowSelected);

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

        // READ User RECORD 
        public void ReadUserAccessToAtms(string InUserId)
        {
            NoOfAtmsRepl = 0;
            NoOfAtmsReconc = 0; 
            NoOfGroupsRepl = 0;
            NoOfGroupsReconc = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
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

                            ReadUsersToAtms(rdr);

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

       

        // READ SPECIFIC User RELATION RECORD 
        public void ReadUsersAccessAtmTableSpecific(string InUserId, string InAtmNo, int InGroupOfAtms)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
          + " FROM [dbo].[UsersAtmTable] "
          + " WHERE UserId = @UserId AND AtmNo = @AtmNo ";
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
                        //cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read USER Details

                            ReadUsersToAtms(rdr);

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

        // READ SPECIFIC for ATM No
        public void ReadUsersAccessAtmTableSpecificForAtmNo(string InAtmNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[UsersAtmTable] "
          + " WHERE AtmNo = @AtmNo ";
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

                            // Read USER Details

                            ReadUsersToAtms(rdr);

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

        // READ SPECIFIC for ATM No for Repl
        public void ReadUsersAccessAtmTableSpecificForAtmNoForRepl(string InAtmNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
          + " FROM [dbo].[UsersAtmTable] "
          + " WHERE AtmNo = @AtmNo AND Replenishment = 1 ";
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

                            // Read USER Details

                            ReadUsersToAtms(rdr);

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

        // READ SPECIFIC for ATM No for Repl
        public void ReadUsersAccessAtmTableSpecificForAtmNoForRecon(string InAtmNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[UsersAtmTable] "
          + " WHERE AtmNo = @AtmNo AND Reconciliation = 1 ";
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

                            // Read USER Details

                            ReadUsersToAtms(rdr);

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
        // Insert USER ATM RELATION 
        //
        public void InsertUsersAtmTable(string InUserId, string InAtmNo, int InGroupOfAtms)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int rows; 

            string cmdinsert = "INSERT INTO [UsersAtmTable]"
                + " ([UserId],[AtmNo],[GroupOfAtms],"
                + " [IsCit],"
                 + " [BankId],"
                + "[UseOfGroup],[Replenishment],[Reconciliation],[DateOfInsert], [Operator] )"
                + " VALUES (@UserId,@AtmNo,@GroupOfAtms,"
                + " @IsCit, "
                + " @BankId, "
                + " @UseOfGroup,"
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

                        cmd.Parameters.AddWithValue("@IsCit", IsCit);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@UseOfGroup", UseOfGroup);
                        cmd.Parameters.AddWithValue("@Replenishment", Replenishment);

                        cmd.Parameters.AddWithValue("@Reconciliation", Reconciliation);
                        cmd.Parameters.AddWithValue("@DateOfInsert", DateOfInsert);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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
                            + " UserId = @UserId,AtmNo = @AtmNo,GroupOfAtms = @GroupOfAtms,IsCit = @IsCit,BankId = @BankId,"
                             + "UseOfGroup = @UseOfGroup,Replenishment = @Replenishment, Reconciliation = @Reconciliation, DateOfInsert = @DateOfInsert"
                            + " WHERE UserId = @UserId AND AtmNo = @AtmNo AND GroupOfAtms = @GroupOfAtms", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);

                        cmd.Parameters.AddWithValue("@IsCit", IsCit);

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
                    CatchDetails(ex);
                }
        }

        // UPDATE USER TO ATMs Or Group of ATMs Table
        //
        public void UpdateUsersAtmTableForAtmAndCit( string InAtmNo , bool InIsCit)
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
                            + " IsCit = @IsCit"               
                            + " WHERE  AtmNo = @AtmNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@IsCit", InIsCit);
                     
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
                    CatchDetails(ex);
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
                    CatchDetails(ex);
                }
        }

        // DELETE USER TO ATMs Or Group of ATMs Table entry 
        //
        public void DeleteUsersAtmTableEntryForATM(string InAtmNo, string InUserId)
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
                            + " WHERE AtmNo = @AtmNo AND UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
                    CatchDetails(ex);
                }
        }
        // 
        // DELETE ATM If IsCit = true 
        //
        public void DeleteUsersAtmTableEntryForATMNo(string InAtmNo, bool InIsCit)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int rows; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[UsersAtmTable] "
                            + " WHERE AtmNo = @AtmNo AND IsCit = IsCit ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@IsCit", InIsCit);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

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
        // DELETE ATM If IsCit = true 
        //
        public void DeleteUsersAtmTableEntryForGroup(int InGroupOfAtms)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[UsersAtmTable] "
                            + " WHERE GroupOfAtms = @GroupOfAtms ", conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

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
        // READ ATM to FIND its GROUP If ANY 
        //
        public void FindIfAtmHasGroup (string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string stringlSelect = "SELECT * "
                 + " FROM ATMsFields"
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
                            IsCit = (bool)rdr["IsCit"];
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
                    CatchDetails(ex);
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

            SqlString = "SELECT *"
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

                            ReadUsersToAtms(rdr);

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
        // Through ATM find user of replenishment 
        //
        public void FindUserForReplForATM(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " SELECT *  "
          + " FROM [dbo].[UsersAtmTable] "
          + " WHERE AtmNo = @AtmNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        //cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadUsersToAtms(rdr);

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
        // Through ATM find user of replenishment 
        //
        public void FindIfUserHasATMs(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " SELECT *  "
          + " FROM [dbo].[UsersAtmTable] "
          + " WHERE UserId = @UserId ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        //cmd.Parameters.AddWithValue("@GroupOfAtms", InGroupOfAtms);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadUsersToAtms(rdr);

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

        // 1) Read ATMs of this group
        // 2) Delete them from table [dbo].[UsersAtmTable] 
        // 3) Insert them in Table with the new user owner 
        public void CreateEntriesInUsersAtmTableFortheNewOwner(string InOperator, int InGroupOfAtms,
                                                                                     string InUserId )
        {
            // Delete all previous 
            DeleteUsersAtmTableEntryForGroup(InGroupOfAtms); 
            // Clear all Entries in Table for this group

            ATMsForThisGroup = new DataTable();
            ATMsForThisGroup.Clear();

            SqlString = " Select * "
                                    + "FROM [ATMS].[dbo].[ATMsFields]"
                                    + " WHERE AtmsReconcGroup = @AtmsReconcGroup "; 

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmsReconcGroup", InGroupOfAtms);

                        sqlAdapt.Fill(ATMsForThisGroup);

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
         
            try
            {
                
                int I = 0;
                int K = ATMsForThisGroup.Rows.Count - 1;

                while (I <= K)
                {

                    RecordFound = true;

                    AtmNo = (string)ATMsForThisGroup.Rows[I]["AtmNo"];
                    string CitId = (string)ATMsForThisGroup.Rows[I]["CitId"];
                    int WAtmsReplGroup = (int)ATMsForThisGroup.Rows[I]["AtmsReplGroup"];

                    UserId = InUserId;
                    GroupOfAtms = InGroupOfAtms;
                    
                    BankId = InOperator;
                    UseOfGroup = true;
                    if (CitId == "1000")
                    {
                        IsCit = false;
                    }
                    else
                    {
                        IsCit = true;
                    }
                    if (WAtmsReplGroup>0)
                    {
                        Replenishment = true;
                    }
                    else
                    {
                        Replenishment = false;
                    }
                   
                    Reconciliation = true;
                    DateOfInsert = DateTime.Now;
                    Operator = InOperator;

                    InsertUsersAtmTable(UserId, AtmNo, GroupOfAtms); 

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                return;

            }

        }

    }
}

using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMUpdateGrids : Logger
    {
        public RRDMUpdateGrids() : base() { }
        //
        // UPDATE WORKING AT THE TIME DATAGRID WITH AUTHRISED USER
        //
        // Declarations 

        // Relation of Users to Atms Table 

        public string UserId;
        public string AtmNo;
        public int GroupOfAtms;
        public bool UseOfGroup;

        public bool Replenishment;
        public bool Reconciliation;

        public DateTime DateOfInsert;

        string WFunction;
  
        public string BankId;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 
 
        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass(); 

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMReplOrdersClass Ra = new RRDMReplOrdersClass(); 

        // READ User RECORD and set ATMs Main Authorised User Field 
        //
        public void ReadUsersAccessAtmAndUpdateMain(string InUserId, int InAction, string InFunction)
        {
            // InAction = 1 for Assign 

            WFunction = InFunction;

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

                            if (GroupOfAtms==0 & InAction == 1) // Means that this user owns ATM and we have to assign field
                            {
                                Am.ReadAtmsMainSpecific(AtmNo);

                                Am.AuthUser = UserId; 

                                Am.UpdateAtmsMain(AtmNo);
                            }
                            else
                            {
                                
                            }

                            if (GroupOfAtms > 0 & InAction == 1) // Means that this user owns a GROUP
                            {
                             UpdateRecordsOfAtmsMainForAuthUser(InUserId, GroupOfAtms, InAction);
                            }
                            else
                            {
                                
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

        // READ User RECORD and set ATMs Replenishment actions with Authorised User Field 
        // FOR REPLENISHMENT AND RECONCILATION ACTIONS 
        public void ReadUsersAccessAtmAndUpdateReplActions(string InUserId, int InAction)
        {
            // InAction = 1 for Assign 

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

                            if (GroupOfAtms == 0 & InAction == 1) // Means that this user owns ATM and we have to assign field
                            {
                                Ra.ReadReplActionsForUpdatingUser(UserId, AtmNo, 1); 
                            }
                            else
                            {
                                if (GroupOfAtms == 0 & InAction == 2) // Means that this user owns ATM we have to initialise field 
                                {

                                    Ra.ReadReplActionsForUpdatingUser(UserId, AtmNo, 2); 
                                }
                            }

                            if (GroupOfAtms > 0 & InAction == 1) // Means that this user owns a GROUP
                            {
                                UpdateRecordsOfAtmsReplAuthUserActions(InUserId, GroupOfAtms, InAction);
                            }
                            else
                            {
                                if (GroupOfAtms > 0 & InAction == 2) // Means that this user owns Group 
                                {
                                    // CALL the routine to make it zero 
                                    UpdateRecordsOfAtmsReplAuthUserActions(InUserId, GroupOfAtms, InAction);
                                }
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
        // Update Auth User  FOR Groups for REPLENISHEMENT OR RECONC
        //  
        // 
        public void UpdateRecordsOfAtmsMainForAuthUser(string InUserId, int InGroup, int InAction)
        {
            // Read sequentially all records and do 

            ErrorFound = false;
            ErrorOutput = ""; 

            int AtmsReplGroup;
            int AtmsReconcGroup;

            string SqlString = "SELECT *"
         + " FROM [dbo].[AtmsMain] "
          + " WHERE AtmsReplGroup=@AtmsReplGroup OR AtmsReconcGroup=@AtmsReconcGroup ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmsReplGroup", InGroup);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InGroup);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            AtmNo = (string)rdr["AtmNo"];
                            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
                            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"]; 

                            Am.ReadAtmsMainSpecific(AtmNo);

                            if (InAction == 1 & AtmsReplGroup == InGroup & WFunction == "Replen") // REPLNISHEMENT 
                            {
                                Am.AuthUser = InUserId;
                                Am.UpdateAtmsMain(AtmNo);
                            }

                            if (InAction == 1 & AtmsReconcGroup == InGroup & WFunction == "Reconc") // RECOCILIATION 
                            {
                                Am.AuthUser = InUserId;
                                Am.UpdateAtmsMain(AtmNo);
                            }
                            if (InAction == 1 &  WFunction == "Any") // RECOCILIATION 
                            {
                                Am.AuthUser = InUserId;
                                Am.UpdateAtmsMain(AtmNo);
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


        // UPDATE ATM Main with Auth User = blank --- Initialise user
        //
        // 
        public void UpdateAtmMainAuthUserWithBlanks(string InOperator, string InUserId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            // When Bank is Prive
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[AtmsMain] SET "
                            + " AuthUser = ''"
                            + " WHERE Operator = @Operator AND AuthUser = @AuthUser ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AuthUser", InUserId);

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
        // Update Auth User  FOR Action Repl Records for Groups 
        //  
        // 
        public void UpdateRecordsOfAtmsReplAuthUserActions(string InUserId, int InGroup, int InAction)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            // Read sequentially all records and do 

            string SqlString = "SELECT *"
         + " FROM [dbo].[ReplActionsTable] "
          + " WHERE AtmsReplGroup=@AtmsReplGroup OR AtmsReconcGroup=@AtmsReconcGroup ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmsReplGroup", InGroup);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InGroup);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            AtmNo = (string)rdr["AtmNo"];

                            if (InAction == 1)
                            {
                                Ra.ReadReplActionsForUpdatingUser(InUserId, AtmNo, 1); 
                            }

                            if (InAction == 2)
                            {
                                Ra.ReadReplActionsForUpdatingUser(InUserId, AtmNo, 2); 
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
        // READ TO FIND to update Authorised User  
        // 
        public void ReadAtmsAndUpdateTransToBePostedAuthUser(DataTable IndtAtmsMain, string InUser)
        {

            int DtSize = IndtAtmsMain.Rows.Count;

            //string AtmNo; 

            int J = 0;

            while (J < DtSize)
            {
                AtmNo = (string)IndtAtmsMain.Rows[J]["AtmNo"];

                Tc.UpdateTransToBePostedAuthUser(AtmNo, InUser);

                J++;
            }

            //TEST
            Tc.UpdateTransToBePostedAuthUser("EWB511", InUser);
            Tc.UpdateTransToBePostedAuthUser("EWB311", InUser); 

        }


    }
}



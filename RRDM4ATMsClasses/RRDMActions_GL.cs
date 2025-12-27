using System;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMActions_GL : Logger
    {
        public RRDMActions_GL() : base() { }
        // 
        //
        public int SeqNo;

        public string ActionId; // eg 11 for debit customer CR Atm cash
                                  //    21 for Credit customer and DR Atm cash 
        public int Occurance;
 
        public string ActionNm;
        public bool Is_GL_Action; 

        public string GL_Sign_1;
        public string ShortAccID_1;
        public string AccName_1;
        
        public string WhatBranch_1;
        public string StatementDesc_1;

        public string GL_Sign_2; 
        public string ShortAccID_2;
        public string AccName_2;

        public string WhatBranch_2;

        public string StatementDesc_2;  
        
        public string ViewVersion;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string SqlString; 

        // Define the data table 
        public DataTable ActionsDataTable;
        
        readonly string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Action Fields
        private void ReadActionFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            ActionId = (string)rdr["ActionId"];
            Occurance = (int)rdr["Occurance"];

            ActionNm = (string)rdr["ActionNm"];
            Is_GL_Action = (bool)rdr["Is_GL_Action"];

            GL_Sign_1 = (string)rdr["GL_Sign_1"];
            ShortAccID_1 = (string)rdr["ShortAccID_1"];
           
            AccName_1 = (string) rdr["AccName_1"];
            WhatBranch_1 = (string)rdr["WhatBranch_1"];

            StatementDesc_1 = (string)rdr["StatementDesc_1"];
            
            GL_Sign_2 = (string)rdr["GL_Sign_2"];
            ShortAccID_2 = (string)rdr["ShortAccID_2"];
            AccName_2 = (string)rdr["AccName_2"];

            WhatBranch_2 = (string)rdr["WhatBranch_2"];

            StatementDesc_2 = (string)rdr["StatementDesc_2"];

            ViewVersion = (string)rdr["ViewVersion"];

            Operator = (string)rdr["Operator"];
        }
        //
        // READ Action BASED ON  Action Type
        //
        public void ReadActionByActionId(string InOperator, string InActionId, int InOccurance)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                         + " FROM [ATMS].[dbo].[Actions_GL]"
                         + " WHERE Operator = @Operator AND ActionId = @ActionId AND Occurance = @Occurance "
                          + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        cmd.Parameters.AddWithValue("@Occurance", InOccurance);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadActionFields(rdr);

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

        ////
        //// READ Action BASED ON  Action Id and Occurance
        ////
        //public void ReadActionByActionId_2(string InOperator, string InActionId, int InOccurance, SqlConnection conn)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    string SqlString = "SELECT * "
        //                 + " FROM [ATMS].[dbo].[Actions_GL]"
        //                 + " WHERE Operator = @Operator AND ActionId = @ActionId AND Occurance = @Occurance "
        //                  + "  ";

        //    //using (SqlConnection conn =
        //    //              new SqlConnection(connectionString))
        //        try
        //        {
        //            //conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@Operator", InOperator);
        //                cmd.Parameters.AddWithValue("@ActionId", InActionId);
        //                cmd.Parameters.AddWithValue("@Occurance", InOccurance);
        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;
        //                    // Read Fields
        //                    ReadActionFields(rdr);

        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //         //   conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            //conn.Close();

        //            CatchDetails(ex);
        //        }
        //}
        //
        // READ Action BASED ON ActionId
        // Check is Customer Account is needed 
        //
        public void ReadActionByActionIdAndCustomerAccountNeeded(string InActionId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                         + " FROM [ATMS].[dbo].[Actions_GL]"
                         + " WHERE ActionId = @ActionId AND (ShortAccID_1 = '20' OR ShortAccID_2 = '20') "
                          + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadActionFields(rdr);

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
        // READ Action BASED ON ActionNm
        //
        public void ReadActionByActionNm(string InOperator, string InActionNm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                         + " FROM [ATMS].[dbo].[Actions_GL]"
                         + " WHERE Operator = @Operator AND ActionNm = @ActionNm "
                          + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ActionNm", InActionNm);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadActionFields(rdr);

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
        // READ Action BASED ON  Action Type and Ocuurance 
        //
        public void ReadActionByActionId_And_Occ(string InOperator, string InActionId, 
                                                                 int InOccurance)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                         + " FROM [ATMS].[dbo].[Actions_GL]"
                         + " WHERE Operator = @Operator AND ActionId = @ActionId And Occurance = @Occurance "
                          + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        cmd.Parameters.AddWithValue("@Occurance", InOccurance);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadActionFields(rdr);

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
        // READ Action BASED ON  SeqNo 
        //
        public void ReadActionBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                         + " FROM [ATMS].[dbo].[Actions_GL]"
                         + " WHERE SeqNo = @SeqNo  "
                          + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadActionFields(rdr);

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
        // READ Action AND Filled table
        //

        public void ReadActionsAndFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ActionsDataTable = new DataTable();
            ActionsDataTable.Clear();

            string SqlString = "SELECT * "
                         + " FROM [ATMS].[dbo].[Actions_GL] "
                         + InSelectionCriteria
                          + " Order By ActionId ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(ActionsDataTable);

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

        // Read ActionNm Array
        //

        public ArrayList ReadTableToGet_ActionNm_Array_List(string InOperator, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Mode = 1 or 2 or 3

            ArrayList ActionNmArray = new ArrayList();


            if (InMode == 1)
            {
                SqlString = "SELECT Distinct (ActionNm) "
                         + " FROM [ATMS].[dbo].[Actions_GL] "
                         + " WHERE Operator = @Operator AND (ViewVersion = 'In_All' "
                         + "  OR ViewVersion = 'Reconciliation Atms At Matching')  "
                          + " Order By ActionNm ";
            }
            if (InMode == 2)
            {
                SqlString = "SELECT Distinct (ActionNm) "
                         + " FROM [ATMS].[dbo].[Actions_GL] "
                         + " WHERE Operator = @Operator AND (ViewVersion = 'In_All' "
                         + "  OR ViewVersion = 'Reconciliation Categories')  "
                          + " Order By ActionNm ";
            }
            if (InMode == 3)
            {
                // NO AUTO GL 
                SqlString = "SELECT Distinct (ActionNm) "
                         + " FROM [ATMS].[dbo].[Actions_GL] "
                         + " WHERE Operator = @Operator AND (ViewVersion = 'In_All' "
                         + "  OR ViewVersion = 'Manual(No Auto GL)')  "
                          + " Order By ActionNm ";
            }
            if (InMode == 4)
            {
                // NO AUTO GL 
                SqlString = "SELECT Distinct (ActionNm) "
                         + " FROM [ATMS].[dbo].[Actions_GL] "
                         + " WHERE Operator = @Operator AND Is_GL_Action = 0 AND (ViewVersion = 'In_All' "
                         + "  OR ViewVersion = 'Reconciliation Categories')  "
                          + " Order By ActionNm ";
            }
            if (InMode == 5)
            {
                // FOR CIT SHORTAGES 
                SqlString = "SELECT Distinct (ActionNm) "
                         + " FROM [ATMS].[dbo].[Actions_GL] "
                         + " WHERE Operator = @Operator "
                         + "  AND ViewVersion = 'CIT_Shortages_Excess(Shown)'  "
                          + " Order By ActionNm DESC ";
            }
            if (InMode == 6)
            {
                // FOR Replenishment 
                SqlString = "SELECT Distinct (ActionNm) "
                         + " FROM [ATMS].[dbo].[Actions_GL] "
                         + " WHERE Operator = @Operator "
                         + "  AND ViewVersion in (  'In_All', 'Reconciliation Atms At Replenishment')  "
                          + " Order By ActionNm ";
            }


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
                            
                            string WActionNm = (string)rdr["ActionNm"];
                            ActionNmArray.Add(WActionNm);
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

            return ActionNmArray;
        }

        // Insert Action_GL
        //
        public void InsertAction_GL()
        {

            ErrorFound = false;
            ErrorOutput = "";
            // AccName_2 
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[Actions_GL] "
           + "   ([ActionId] "
         + "   ,[Occurance] "
         + "   ,[ActionNm] "
         + "   ,[Is_GL_Action] "
         + "   ,[GL_Sign_1] "
         + "   ,[ShortAccID_1] "
          + "   ,[AccName_1] "
         + "   ,[WhatBranch_1] "
         + "   ,[StatementDesc_1] "
         + "   ,[GL_Sign_2] "
         + "   ,[ShortAccID_2] "
          + "   ,[AccName_2] "
         + "   ,[WhatBranch_2] "
         + "   ,[StatementDesc_2] "
         + "  ,[ViewVersion] "
         + "   ,[Operator]) "
                    + " VALUES "
          + "   (@ActionId"
         + "    ,@Occurance "
         + "   ,@ActionNm "
         + "  ,@Is_GL_Action "
         + "    ,@GL_Sign_1 "
         + "   ,@ShortAccID_1 "
         + "   ,@AccName_1  "
         + "   ,@WhatBranch_1 "
         + "   ,@StatementDesc_1 "
         + "   ,@GL_Sign_2"
         + "   ,@ShortAccID_2 "
          + "   ,@AccName_2 "
         + "   ,@WhatBranch_2"
         + "   ,@StatementDesc_2 "
         + "   ,@ViewVersion"
         + "   ,@Operator) "; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@ActionId", ActionId);
                        cmd.Parameters.AddWithValue("@Occurance", Occurance);
                        cmd.Parameters.AddWithValue("@ActionNm", ActionNm);

                        cmd.Parameters.AddWithValue("@Is_GL_Action", Is_GL_Action);

                        cmd.Parameters.AddWithValue("@GL_Sign_1", GL_Sign_1);
                        cmd.Parameters.AddWithValue("@ShortAccID_1", ShortAccID_1);
                        cmd.Parameters.AddWithValue("@AccName_1", AccName_1);

                        cmd.Parameters.AddWithValue("@WhatBranch_1", WhatBranch_1);
                        cmd.Parameters.AddWithValue("@StatementDesc_1", StatementDesc_1);
                        
                        cmd.Parameters.AddWithValue("@GL_Sign_2", GL_Sign_2);
                        cmd.Parameters.AddWithValue("@ShortAccID_2", ShortAccID_2);
                        cmd.Parameters.AddWithValue("@AccName_2", AccName_2);

                        cmd.Parameters.AddWithValue("@WhatBranch_2", WhatBranch_2);
                        cmd.Parameters.AddWithValue("@StatementDesc_2", StatementDesc_2);

                        cmd.Parameters.AddWithValue("@ViewVersion", ViewVersion);
                       
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

        // UPDATE Action_GL
        // 
        public void UpdateAction_GL(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [ATMS].[dbo].[Actions_GL] "
                   + " SET "
                   + "    [ActionId] = @ActionId"
                   + "   ,[Occurance] = @Occurance"
                   + "   ,[ActionNm] = @ActionNm "
                   + "   ,[Is_GL_Action] = @Is_GL_Action "
                   + "   ,[GL_Sign_1] = @GL_Sign_1 "
                   + "   ,[ShortAccID_1] = @ShortAccID_1 "
                   + "   ,[AccName_1] = @AccName_1 "
                   + "   ,[WhatBranch_1] = @WhatBranch_1 "
                   + "   ,[StatementDesc_1] = @StatementDesc_1 "
                   + "   ,[GL_Sign_2] = @GL_Sign_2 "
                   + "   ,[ShortAccID_2] = @ShortAccID_2 "
                   + "   ,[AccName_2] = @AccName_2 "
                   + "   ,[WhatBranch_2] = @WhatBranch_2 "
                   + "   ,[StatementDesc_2] = @StatementDesc_2 "
                   + "   ,[ViewVersion] = @ViewVersion "
                   + "   ,[Operator] = @Operator"
                   + "  WHERE SeqNo = @SeqNo "
                   , conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@ActionId", ActionId);
                        cmd.Parameters.AddWithValue("@Occurance", Occurance);
                        cmd.Parameters.AddWithValue("@ActionNm", ActionNm);

                        cmd.Parameters.AddWithValue("@Is_GL_Action", Is_GL_Action);

                        cmd.Parameters.AddWithValue("@GL_Sign_1", GL_Sign_1);
                        cmd.Parameters.AddWithValue("@ShortAccID_1", ShortAccID_1);
                        cmd.Parameters.AddWithValue("@AccName_1", AccName_1);

                        cmd.Parameters.AddWithValue("@WhatBranch_1", WhatBranch_1);
                        cmd.Parameters.AddWithValue("@StatementDesc_1", StatementDesc_1);

                        cmd.Parameters.AddWithValue("@GL_Sign_2", GL_Sign_2);
                        cmd.Parameters.AddWithValue("@ShortAccID_2", ShortAccID_2);
                        cmd.Parameters.AddWithValue("@AccName_2", AccName_2);

                        cmd.Parameters.AddWithValue("@WhatBranch_2", WhatBranch_2);
                        cmd.Parameters.AddWithValue("@StatementDesc_2", StatementDesc_2);

                        cmd.Parameters.AddWithValue("@ViewVersion", ViewVersion);

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

        //
        // DELETE Action 
        //
        public void DeleteActionRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[Actions_GL] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        //rows number of record got updated

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


    }
}



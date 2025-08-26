using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMAuthorisationProcess : Logger
    {
        public RRDMAuthorisationProcess() : base() { }
        //      
        //
        public int SeqNumber;

       public string Requestor; 
       public string Authoriser;

       public string Origin; // Values 
                             // Dispute Action
                             // Replenishment
                             // Reconciliation 
       public int TranNo; 
       public int DisputeNumber; 
       public int DisputeTransaction;
// Following two fields needed for Replenishment 
       public string AtmNo;
       public int ReplCycle; 

       public string AuthDecision; // Yes, NO
       public string AuthComment;
       public DateTime DateOriginated;
       public DateTime DateAuthorised;
       public int Stage; // 1=Just Created by Requestor , 
                         // 2= Authorizer had receieved a mmessage , 
                         // 3=Authorizer took action, 
                         // 4=message was received by Originator, 
                         // 5 = Work completed 

       public bool Transfered;
       public DateTime TransferedDate;
       public string ReasonOfTransfer;

       public int DifferenceStatus;

       public string RMCategory; // F
       public int RMCycle; 

       public bool OpenRecord; 
       public string Operator;

        public int TotalNumberforUser;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public string MessageOut;


        // Define the data table 
        public DataTable ATMsAuthorisationsDataTable = new DataTable();

        public int TotalSelected;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // Reader Fields 
        private void AuthReaderFields(SqlDataReader rdr)
        {
            SeqNumber = (int)rdr["SeqNumber"];

            Requestor = (string)rdr["Requestor"];
            Authoriser = (string)rdr["Authoriser"];

            Origin = (string)rdr["Origin"];

            TranNo = (int)rdr["TranNo"];
            DisputeNumber = (int)rdr["DisputeNumber"];
            DisputeTransaction = (int)rdr["DisputeTransaction"];

            AtmNo = (string)rdr["AtmNo"];
            ReplCycle = (int)rdr["ReplCycle"];

            AuthDecision = (string)rdr["AuthDecision"];
            AuthComment = (string)rdr["AuthComment"];

            DateOriginated = (DateTime)rdr["DateOriginated"];
            DateAuthorised = (DateTime)rdr["DateAuthorised"];

            Stage = (int)rdr["Stage"];

            Transfered = (bool)rdr["Transfered"];
            TransferedDate = (DateTime)rdr["TransferedDate"];

            ReasonOfTransfer = (string)rdr["ReasonOfTransfer"];

            DifferenceStatus = (int)rdr["DifferenceStatus"];

            RMCategory = (string)rdr["RMCategory"];

            RMCycle = (int)rdr["RMCycle"];

            OpenRecord = (bool)rdr["OpenRecord"];
            Operator = (string)rdr["Operator"];
        }
        //
        // READ Authorization Record for a SequenceNumber 
        //
        string SqlString;

        public void ReadAuthorizationsDataTable(string InOperator, string InSignedId, string InAtmNo, int InSesNo, 
             string InRMCateg, int InRMCycle, int InDisputeNo, int InDisputeTranNo, int InMode)
        {
            //WGridFilter = " Operator = @Operator AND AtmNo = @AtmNo "
            //                    + "' AND ReplCycle = @ReplCycle "; 
            //WGridFilter = " Operator = @Operator AND RMCategory = @RMCategory
            //                    + "' AND RMCycle = @RMCycle" ;
            //WGridFilter = " Operator = @Operator AND DisputeNumber = @DisputeNo "
                              //+ " AND DisputeTransaction = @DisputeTranNo ";
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ATMsAuthorisationsDataTable = new DataTable();
            ATMsAuthorisationsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsAuthorisationsDataTable.Columns.Add("SeqNumber", typeof(int));
            ATMsAuthorisationsDataTable.Columns.Add("Requestor", typeof(string));
            ATMsAuthorisationsDataTable.Columns.Add("Authoriser", typeof(string));
            ATMsAuthorisationsDataTable.Columns.Add("Stage", typeof(int));
            ATMsAuthorisationsDataTable.Columns.Add("Origin", typeof(string));
            ATMsAuthorisationsDataTable.Columns.Add("Created", typeof(string));

            if (InMode == 11)
            {
                SqlString = "SELECT * "
                                + " FROM [dbo].[AuthorizationTable]"
                                + " WHERE  (Operator = @Operator AND Requestor = @Requestor AND OpenRecord = 1) "
                                       + "  OR  (Operator = @Operator AND Authoriser = @Authoriser AND OpenRecord = 1) "
                                        + "  ORDER by Stage, AtmNo ";
            }
            if (InMode == 12)
            {
                SqlString = "SELECT * "
                                + " FROM [dbo].[AuthorizationTable]"
                                + " WHERE  Operator = @Operator AND AtmNo = @AtmNo "
                                + " AND ReplCycle = @ReplCycle ";
            }
            if (InMode == 13)
            {
                SqlString = "SELECT * "
                                + " FROM [dbo].[AuthorizationTable]"
                                + " WHERE Operator = @Operator AND RMCategory = @RMCategory "
                                + " AND RMCycle = @RMCycle " ;
            }

            if (InMode == 14)
            {
                SqlString = "SELECT *"
                                + " FROM [dbo].[AuthorizationTable]"
                                + " WHERE Operator = @Operator AND DisputeNumber = @DisputeNo "
                              + " AND DisputeTransaction = @DisputeTranNo ";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InMode == 11)
                        {
                            cmd.Parameters.AddWithValue("@Operator", InOperator);
                            cmd.Parameters.AddWithValue("@Requestor", InSignedId);
                            cmd.Parameters.AddWithValue("@Authoriser", InSignedId);
                        }
                        if (InMode == 12)
                        {
                            cmd.Parameters.AddWithValue("@Operator", InOperator);
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                            cmd.Parameters.AddWithValue("@ReplCycle", InSesNo);
                        }
                        if (InMode == 13)
                        {
                            cmd.Parameters.AddWithValue("@Operator", InOperator);
                            cmd.Parameters.AddWithValue("@RMCategory", InRMCateg);
                            cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        }
                        if (InMode == 14)
                        {
                            cmd.Parameters.AddWithValue("@Operator", InOperator);
                            cmd.Parameters.AddWithValue("@DisputeNo", InDisputeNo);
                            cmd.Parameters.AddWithValue("@DisputeTranNo", InDisputeTranNo);
                        }
                        //cmd.Parameters.AddWithValue("@SeqNumber", 123);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            AuthReaderFields(rdr);

                            //
                            // Fill In Table
                            //
        
                            DataRow RowSelected = ATMsAuthorisationsDataTable.NewRow();

                            RowSelected["SeqNumber"] = SeqNumber;
                            RowSelected["Requestor"] = Requestor;
                            RowSelected["Authoriser"] = Authoriser;
                            RowSelected["Stage"] = Stage;

                            if (Origin == "ReconciliationCat")
                            {
                                RowSelected["Origin"] = Origin+ "_Categ:_"+ RMCategory+"_RMCycle_"+ RMCycle.ToString();
                            }
                            if (Origin == "Replenishment")
                            {
                                RowSelected["Origin"] = Origin + "_AtmNo:_" + AtmNo + "_ReplCycle_" + ReplCycle.ToString();
                            }
                            if (Origin == "Dispute Action")
                            {
                                RowSelected["Origin"] = Origin + "_DisputeNo:_" + DisputeNumber + "_DisputeTxn_" + DisputeTransaction.ToString();
                            }
                            if (Origin == "LoadingExcel")
                            {
                                RowSelected["Origin"] = "Loading CIT Records for :" + RMCategory; // Here is the CIT ID
                            }

                            RowSelected["Created"] = DateOriginated.ToString();

                           
                            // ADD ROW
                            ATMsAuthorisationsDataTable.Rows.Add(RowSelected);


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
        // READ Authorization Record for a SequenceNumber 
        //
        public void ReadAuthorizationSpecific(int InSeqNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
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

                            // Read Fields
                            AuthReaderFields(rdr);

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
        // READ Authorization Records 
        //
        public void ReadAuthorizations(string InAuthoriser)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE Authoriser = @Authoriser"
               + " ORDER by SeqNumber "; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Authoriser", InAuthoriser);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            AuthReaderFields(rdr);

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
        // READ Authorization Records 
        //
        public void ReadAuthorizationsUserTotal(string InUser)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalNumberforUser = 0;

            string SqlString = " SELECT * "
               + " FROM [ATMS].[dbo].[AuthorizationTable] "
               + " WHERE ((Requestor= @Requestor OR Authoriser = @Authoriser) AND OpenRecord = 1)"
               + " ORDER by SeqNumber "; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Requestor", InUser);
                        cmd.Parameters.AddWithValue("@Authoriser", InUser);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalNumberforUser = TotalNumberforUser + 1; 

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            OpenRecord = (bool)rdr["OpenRecord"];
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
        public void ReadAuthorizationsUserTopOne(string InUser)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalNumberforUser = 0;

            string SqlString = " SELECT TOP (1) *  "
               + " FROM [ATMS].[dbo].[AuthorizationTable] "
               + " WHERE ((Requestor= @Requestor OR Authoriser = @Authoriser) AND OpenRecord = 1)"
               + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Requestor", InUser);
                        cmd.Parameters.AddWithValue("@Authoriser", InUser);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            OpenRecord = (bool)rdr["OpenRecord"];
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
        // READ Authorization Record for a Dispute and a transaction 
        //
        public void ReadAuthorizationForDisputeAndTransaction(int InDisputeNumber, int InDisputeTransaction)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WCounter = 0;

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE DisputeNumber = @DisputeNumber AND DisputeTransaction = @DisputeTransaction AND OpenRecord = 1 "
               + " ORDER by SeqNumber "; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DisputeNumber", InDisputeNumber);
                        cmd.Parameters.AddWithValue("@DisputeTransaction", InDisputeTransaction);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;


                            // Read Fields
                            AuthReaderFields(rdr);

                            WCounter = WCounter + 1; 

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

        public void ReadAuthorizationForDisputeAndTransaction_VIEW(int InDisputeNumber, int InDisputeTransaction)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WCounter = 0;

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE DisputeNumber = @DisputeNumber AND DisputeTransaction = @DisputeTransaction AND OpenRecord = 0 "
               + " ORDER by SeqNumber ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DisputeNumber", InDisputeNumber);
                        cmd.Parameters.AddWithValue("@DisputeTransaction", InDisputeTransaction);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;


                            // Read Fields
                            AuthReaderFields(rdr);

                            WCounter = WCounter + 1;

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
        // READ Authorization Record for Replenishement and Reconciliation  , ATMNo and Repl Cycle  
        //
        public int WCounter; 
        public void ReadAuthorizationForReplenishmentReconcSpecificForAtm(string InAtmNo, int InReplCycle, string InOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WCounter = 0; 

            string SqlString = "SELECT * "
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle AND Origin = @Origin AND OpenRecord = 1 "
               + " ORDER by SeqNumber ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@Origin", InOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            AuthReaderFields(rdr);

                            WCounter = WCounter+1;

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

        public void ReadAuthorizationForReplenishmentReconcSpecificForAtmViewClose(string InAtmNo, int InReplCycle, string InOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WCounter = 0;

            string SqlString = "SELECT * "
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle AND Origin = @Origin AND OpenRecord = 0 "
               + " ORDER by SeqNumber ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@Origin", InOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            AuthReaderFields(rdr);

                            WCounter = WCounter + 1;

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
        // READ Authorization Record for RMCategory and RMCycle 
        //
        public void ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(string InRMCategory, int InRmCycle, string InOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WCounter = 0;

            string SqlString = "SELECT * "
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE RMCategory = @RMCategory AND RmCycle = @RmCycle AND Origin = @Origin AND OpenRecord = 1 "
               + " ORDER by SeqNumber " ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCategory", InRMCategory);
                        cmd.Parameters.AddWithValue("@RmCycle", InRmCycle);
                        cmd.Parameters.AddWithValue("@Origin", InOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            AuthReaderFields(rdr);

                            WCounter = WCounter + 1;

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
        // READ Authorization Record for RMCategory and RMCycle 
        //
        public void ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle_Loading_Excel(string InRMCategory, int InRmCycle, string InOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WCounter = 0;

            string SqlString = "SELECT * "
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE RMCategory = @RMCategory AND RmCycle = @RmCycle AND Origin = @Origin AND OpenRecord = 1 "
               + " ORDER by SeqNumber ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCategory", InRMCategory);
                        cmd.Parameters.AddWithValue("@RmCycle", InRmCycle);
                        cmd.Parameters.AddWithValue("@Origin", InOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            AuthReaderFields(rdr);

                            WCounter = WCounter + 1;

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

        public void ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycleVIEW_close(string InRMCategory, int InRmCycle, string InOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WCounter = 0;

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE RMCategory = @RMCategory AND RmCycle = @RmCycle AND Origin = @Origin AND OpenRecord = 0 "
               + " ORDER by SeqNumber ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCategory", InRMCategory);
                        cmd.Parameters.AddWithValue("@RmCycle", InRmCycle);
                        cmd.Parameters.AddWithValue("@Origin", InOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            AuthReaderFields(rdr);

                            WCounter = WCounter + 1;

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
        // READ Authorization Record By 
        // RMCategory 
        // RMCycle 
        //
        public void ReadAuthorizationRecordByRMCategoryAndRMCycle(string InRMCategory, int InRMCycle )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE RMCategory = @RMCategory AND RMCycle = @RMCycle "
                  + " ORDER by SeqNumber "; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCategory", InRMCategory);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            AuthReaderFields(rdr);

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

        // Insert Authorization Record 
        //
        public int InsertAuthorizationRecord()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[AuthorizationTable]"
                    + "([Requestor], [Authoriser], [Origin],  "
                    + " [TranNo], [DisputeNumber], [DisputeTransaction], "
                    + " [AtmNo], [ReplCycle], "
                    + " [DateOriginated], [Stage], [DifferenceStatus],"
                    + " [RMCategory], [RMCycle]," 
                    + " [Operator] )"
                    + " VALUES (@Requestor, @Authoriser, @Origin,"
                    + " @TranNo, @DisputeNumber, @DisputeTransaction, "
                    + " @AtmNo, @ReplCycle, "
                    + " @DateOriginated, @Stage, @DifferenceStatus,"
                     + "@RMCategory, @RMCycle," 
                    + " @Operator )" 
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@Requestor", Requestor);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                      
                        cmd.Parameters.AddWithValue("@TranNo", TranNo);
                        cmd.Parameters.AddWithValue("@DisputeNumber", DisputeNumber);
                        cmd.Parameters.AddWithValue("@DisputeTransaction", DisputeTransaction);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);

                        cmd.Parameters.AddWithValue("@DateOriginated", DateOriginated);

                        cmd.Parameters.AddWithValue("@Stage", Stage);

                        cmd.Parameters.AddWithValue("@DifferenceStatus", DifferenceStatus);

                        cmd.Parameters.AddWithValue("@RMCategory", RMCategory);
                        cmd.Parameters.AddWithValue("@RMCycle", RMCycle);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        SeqNumber = (int)cmd.ExecuteScalar();
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

            return SeqNumber; 
        }
        //
        // UPDATE Authorization Record 
        // 

        public void UpdateAuthorisationRecord( int InSeqNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[AuthorizationTable] SET "
                            + " AuthDecision = @AuthDecision, AuthComment = @AuthComment, "
                            + " DateAuthorised = @DateAuthorised, Stage = @Stage, "
                            + " Transfered = @Transfered, TransferedDate = @TransferedDate," 
                            + " ReasonOfTransfer = @ReasonOfTransfer,"
                            + "OpenRecord = @OpenRecord "
                            + " WHERE SeqNumber = @SeqNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        cmd.Parameters.AddWithValue("@AuthDecision", AuthDecision);
                        cmd.Parameters.AddWithValue("@AuthComment", AuthComment);
                        cmd.Parameters.AddWithValue("@DateAuthorised", DateAuthorised);

                        cmd.Parameters.AddWithValue("@Stage", Stage);

                        cmd.Parameters.AddWithValue("@Transfered", Transfered);
                        cmd.Parameters.AddWithValue("@TransferedDate", TransferedDate);

                        cmd.Parameters.AddWithValue("@ReasonOfTransfer", ReasonOfTransfer);

                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

                        cmd.CommandTimeout = 20;  // seconds
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
// READ LAST AUTHORIAZATION FOR THIS ORIGINATOR 
        public void FindAuthorizationLastNo(string InRequestor, int InTranNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM AuthorizationTable"
                   + " WHERE SeqNumber = (SELECT MAX(SeqNumber) FROM AuthorizationTable) AND Requestor = @Requestor AND TranNo = @TranNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Requestor", InRequestor);
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;


                            // Read Fields
                            AuthReaderFields(rdr);

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
           // READ Authorization Records to find if this user has to Authorize 
        //
        public void CheckOutstandingAuthorizationsStage1(string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = " SELECT * "
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE Authoriser = @InSignedId AND Stage = 1 AND OpenRecord = 1"
               + " ORDER By SeqNumber "; // Added to avoid deadlock 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@InSignedId", InSignedId);

                        // Read table 
                        cmd.CommandTimeout = 20;  // seconds
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields
                            AuthReaderFields(rdr);

                            //**********************
                            if (Stage == 1) Stage = 2; // Message was sent to Authorizer
                      
                            UpdateAuthorisationRecord(SeqNumber);

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
        // READ Authorization Records and Update if needed 
        //

        public int CheckOutstandingAuthorizationsStage1_with_Update(string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int CountUpdated = 0; 

          
            // If found make from Stage 1 to Stage 2 
            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[AuthorizationTable] SET "
                            + "  "
                            + " Stage = @Stage "
                            + " WHERE Authoriser = @InSignedId AND Stage = 1 AND OpenRecord = 1 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@InSignedId", InSignedId);

                        cmd.Parameters.AddWithValue("@Stage", 2 ); // Make stage 2 as be gotten by authoriser

                        cmd.CommandTimeout = 90;  // seconds
                        CountUpdated = cmd.ExecuteNonQuery();


                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return CountUpdated; 
        }

        public void UpdateStageBasedOnSeqNumber(int InSeqNumber, int InStage)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int CountUpdated = 0;


            // If found make from Stage 1 to Stage 2 
            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[AuthorizationTable] SET "
                            + "  "
                            + " Stage = @Stage "
                            + " WHERE SeqNumber = @SeqNumber ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);
                        cmd.Parameters.AddWithValue("@Stage", InStage); 

                        //cmd.CommandTimeout = 90;  // seconds
                        CountUpdated = cmd.ExecuteNonQuery();


                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

          //  return CountUpdated;
        }

        //
        // READ Authorization Records and Update if needed 
        //

        public int CheckOutstandingAuthorizationsStage3_with_Update(string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int CountUpdated = 0;


            // If found make from Stage 3 to Stage 4
            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[AuthorizationTable] SET "
                            + "  "
                            + " Stage = @Stage "
                            + " WHERE Requestor = @InSignedId AND Stage = 3 AND OpenRecord = 1 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@InSignedId", InSignedId);

                        cmd.Parameters.AddWithValue("@Stage", 4); // Make it stage as 4 - gotten by Maker 

                        cmd.CommandTimeout = 80;  // seconds
                        CountUpdated = cmd.ExecuteNonQuery();


                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return CountUpdated;
        }
        // Check for server connection
        public bool IsServerConnected()
        {
           
            string connectionStringShortTime = "";
           
            connectionStringShortTime = ConfigurationManager.ConnectionStrings
                     ["ATMSConnectionStringShortTime"].ConnectionString;
            

            using (SqlConnection conn =
                          new SqlConnection(connectionStringShortTime))
            {
                try
                {
                    //SqlCommand command = new SqlCommand(conn);
                    //conn.ConnectionTimeout = 5; 
                    conn.Open();
                    //command.conn.ConnectionTimeout=5
                    conn.Close();
                    return true;
                }
                catch (SqlException ex) 
                {
                    MessageBox.Show("SQL is Not Operational - Check it please!");
                    string MessageForSQL = ex.Message;
                    MessageBox.Show(MessageForSQL); 
                    return false;
                }
            }
        }

        //
        // READ Authorization Records to find if this user has to Authorize 
        //
        public void CheckOutstandingAuthorizationsStage3(string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE Requestor = @InSignedId AND Stage = 3 AND OpenRecord = 1"
               + " ORDER By SeqNumber "; // Added to avoid deadlock 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@InSignedId", InSignedId);

                        // Read table 
                        cmd.CommandTimeout = 80;  // seconds
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;


                            // Read Fields
                            AuthReaderFields(rdr);

                            //**********************

                            if (Stage == 3) Stage = 4; // Message Was sent to Originator 
                            UpdateAuthorisationRecord(SeqNumber);

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
        // DELETE Authorization record 
        //
        public void DeleteAuthorisationRecord(int InSeqNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[AuthorizationTable] "
                            + " WHERE SeqNumber =  @SeqNumber ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

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
        // DELETE Authorization record for Replenishment 
        //
        //DisputeNumber = (int) rdr["DisputeNumber"];
        //DisputeTransaction = (int) rdr["DisputeTransaction"];

        //AtmNo = (string) rdr["AtmNo"];
        //ReplCycle = (int) rdr["ReplCycle"];

        //RMCategory = (string) rdr["RMCategory"];

        //RMCycle = (int) rdr["RMCycle"];

        // Delete single or double entries
        public void DeleteAuthorisationRecord_Repl(string InAtmNo, int InReplCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[AuthorizationTable] "
                            + " WHERE AtmNo =  @AtmNo AND ReplCycle = @ReplCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);

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
        // Delete single or double entries
        public void DeleteAuthorisationRecord_Cat(string InRMCategory, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[AuthorizationTable] "
                            + " WHERE RMCategory =  @RMCategory AND RMCycle = @RMCycle", conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCategory", InRMCategory);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

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
        // Delete single or double entries
        public void DeleteAuthorisationRecord_By_RMCycle( int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[AuthorizationTable] "
                            + " WHERE RMCycle = @RMCycle", conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

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
        // Delete single or double entries
        // Ap.DisputeNumber, Ap.DisputeTransaction;
        public void DeleteAuthorisationRecord_Disp(int InDisputeNumber, int InDisputeTransaction)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[AuthorizationTable] "
                            + " WHERE DisputeNumber =  @DisputeNumber AND DisputeTransaction = @DisputeTransaction", conn))
                    {
                        cmd.Parameters.AddWithValue("@DisputeNumber", InDisputeNumber);
                        cmd.Parameters.AddWithValue("@DisputeTransaction", InDisputeTransaction);

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
        // DELETE outstanding Authorization record for a sepcific ATM No and user
        //
        public void DeleteAuthorisationRecord_ChangeOwner(string InAtmNo, string InOldOwner)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int count = 0; 
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[AuthorizationTable] "
                            + " WHERE Stage < 5 AND Requestor =  @Requestor AND AtmNo = @AtmNo ", conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@Requestor", InOldOwner);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        count = cmd.ExecuteNonQuery();


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
        // DELETE outstanding Authorization records for a specific user 
        //
        public int DeleteAuthorisationRecord_ForSpecificUserAndStageLessThan5(string InOldOwner, string InRMCategory)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[AuthorizationTable] "
                            + " WHERE Stage < 5 AND Requestor =  @Requestor AND RMCategory = @RMCategory ", conn))
                    {

                        cmd.Parameters.AddWithValue("@Requestor", InOldOwner);
                        cmd.Parameters.AddWithValue("@RMCategory", InRMCategory);

                        count = cmd.ExecuteNonQuery();


                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return count; 

        }

        //
        // This is to Find Dulicates Auth
        //
        //    conn.Open();
        //                using (SqlCommand cmd =
        //                    new SqlCommand("DELETE FROM [dbo].[AuthorizationTable] "
        //                        + " WHERE AtmNo =  @AtmNo AND ReplCycle = @ReplCycle ", conn))
        //                {
        //                    cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
        //                    cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);

        //                    cmd.ExecuteNonQuery();


        //                }
        //// Close conn
        //conn.Close();
        public DataTable TableDublicates_Auth = new DataTable();
        public void FindDuplicateRecordsInAuth()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableDublicates_Auth = new DataTable();
            TableDublicates_Auth.Clear();

            TotalSelected = 0;

            SqlString =

           "     SELECT * "
+ " FROM  [ATMS].[dbo].[AuthorizationTable] y "
+ " INNER JOIN "
+ " (SELECT  AtmNo, ReplCycle, COUNT(*) AS CountOf "

+ " FROM   [ATMS].[dbo].[AuthorizationTable] "

+ " WHERE Origin = 'Replenishment' AND OpenRecord = 1 "

+ " GROUP BY AtmNo, ReplCycle HAVING COUNT(*) > 1) dt "
+ " ON  y.AtmNo = dt.AtmNo AND y.ReplCycle = dt.ReplCycle "
+ "  "
+ "  ";

            using (SqlConnection conn =
           new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                      
                        sqlAdapt.Fill(TableDublicates_Auth);

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
        // Get Message ONe for Replen and Reconciliation 
        //
        public void GetMessageOne(string InAtmNo, int InSesNo,string InOrigin ,bool InAuthoriser, bool InRequestor, bool Reject)
        {
            //RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 
            //Us.ReadSignedActivityByKey(InSignRecordNo);


            // InOrigin      ..... 55 or 56  

            ReadAuthorizationForReplenishmentReconcSpecificForAtm(InAtmNo, InSesNo, InOrigin);

            if (RecordFound == true & OpenRecord == true)
            {
                if (InRequestor == true & Stage < 4) // Coming from originator and authorisation still outstanding 
                {
                    MessageOut = "A message was sent to authoriser. Refresh for progress monitoring.";
                }

                if (InAuthoriser == true & Stage == 2 )
                {
                    MessageOut = "Proceed to authorisation.";
                }

                if (InAuthoriser == true & Stage == 3)
                {
                    if (AuthDecision == "YES")
                    {
                        MessageOut = "Authorisation Made - Accepted ";
                    }
                    if (AuthDecision == "NO")
                    {
                        MessageOut = "Authorisation Made - Rejected ";
                    }           
                }

                if (InAuthoriser == true & Stage == 4)
                {
                    MessageOut = "Requestor at the stage of Finishing.";
                }

                if (InRequestor == true & Stage == 4 & AuthDecision == "YES")
                {
                    MessageOut = "Authorisation made. Replenishment workflow can finish! ";
                }
            }

            if (Reject == true)
            {
                MessageOut = "Review and redo authorisation. ";
            }

        }

        //
        // Get Message ONe for Replen and Reconciliation 
        //
        public void GetMessageReconCateg(string InRMCateg, int InRMCycle, string InOrigin, bool InAuthoriser, bool InRequestor, bool Reject)
        {
            //RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 
            //Us.ReadSignedActivityByKey(InSignRecordNo);


            // InOrigin      ..... 55 or 56  

            ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(InRMCateg, InRMCycle, InOrigin);

            if (RecordFound == true & OpenRecord == true)
            {
                if (InRequestor == true & Stage < 4) // Coming from originator and authorisation still outstanding 
                {
                    MessageOut = "A message was sent to authoriser. Refresh for progress monitoring.";
                }

                if (InAuthoriser == true & Stage == 2)
                {
                    MessageOut = "Proceed to authorisation.";
                }

                if (InAuthoriser == true & Stage == 3)
                {
                    if (AuthDecision == "YES")
                    {
                        MessageOut = "Authorisation Made - Accepted ";
                    }
                    if (AuthDecision == "NO")
                    {
                        MessageOut = "Authorisation Made - Rejected ";
                    }
                }

                if (InAuthoriser == true & Stage == 4)
                {
                    MessageOut = "Requestor at the stage of Finishing.";
                }

                if (InRequestor == true & Stage == 4 & AuthDecision == "YES")
                {
                    MessageOut = "Authorisation made. Replenishment workflow can finish! ";
                }
            }

            if (Reject == true)
            {
                MessageOut = "Review and redo authorisation. ";
            }

        }

        //
        // Get Message TWO  --- dispute 
        //
        public void GetMessageTwo(int InDisputeNumber, int InDispTranNo, bool InAuthoriser, bool InRequestor, bool Reject)
        {
            //RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 
            //Us.ReadSignedActivityByKey(InSignRecordNo);


            // InOrigin      ..... 55 or 56  

            ReadAuthorizationForDisputeAndTransaction(InDisputeNumber, InDispTranNo);

            if (RecordFound == true & OpenRecord == true)
            {
                if (InRequestor == true & Stage < 4) // Coming from originator and authorisation still outstanding 
                {
                    MessageOut = "A message was sent to authoriser. Refresh for progress monitoring.";
                }

                if (InAuthoriser == true & Stage == 2)
                {
                    MessageOut = "Proceed to authorisation.";
                }

                if (InAuthoriser == true & Stage == 3)
                {
                    if (AuthDecision == "YES")
                    {
                        MessageOut = "Authorisation Made - Accepted ";
                    }
                    if (AuthDecision == "NO")
                    {
                        MessageOut = "Authorisation Made - Rejected ";
                    }
                }

                if (InAuthoriser == true & Stage == 4)
                {
                    MessageOut = "Requestor at the stage of Finishing.";
                }

                if (InRequestor == true & Stage == 4 & AuthDecision == "YES")
                {
                    MessageOut = "Authorisation made. Workflow can finish! ";
                }
            }

            if (Reject == true)
            {
                MessageOut = "Review and redo authorisation. ";
            }

        }

       
    }
}

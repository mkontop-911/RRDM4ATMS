using System;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace RRDM4ATMs
{
    public class RRDMMatchingReconcExceptionsInfoITMX : Logger
    {
        public RRDMMatchingReconcExceptionsInfoITMX() : base() { }

        public int SeqNo;
        public int MaskRecordId;
        public string ITMXUniqueTxnRef;
        public string ReconcCategoryId;
        public int ReconcCycleNo;

        public string UnMatchedName;
        public bool DefaultAction; 
        public string ExceptionRecomm;
        public int MetaExceptionId;
        public int MetaExceptionNo;
        public int ActionTypeId; // Only the number
        //public string ActionTypeId;// 1 ... Creation of posted transaction  
        //                           // 2 ... Creation Of Dispute 
        //                           // 3 ... Creation of SMS 
        //                           // 4 ... Create Disputes for both Banks 
        //                           // 5 ... Postponed 
        //                           // 6 ... Closed exception by Reconc Officer 
        //                           // 7 ... Default Action 
        public string ActionTypeDescr; // Second part after the number 
        public bool ActionByUser;
        public string UserId;

        public string Authoriser;
        public DateTime AuthoriserDtTm;
        public DateTime CreatedDtTm;

        public string Operator;

        // Define the data table 
        public DataTable DataTableMatchingExceptions = new DataTable();
        public int TotalSelected;

        public int TotalExceptionsOutstandingToBeAuthorised;
        public int TotalExceptionsDoneAndAuthorised;
        public int TotalExceptionsHandleByUser;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        string SqlString; // Do not delete 

        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX(); 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //
        // Methods 
        // READ MatchingReconcExceptionsInfo to fill table 
        // 
        //
        public void ReadMatchingReconcExceptionsInfoToFillTable(string InOperator, string InReconcCategoryId, int InReconcCycleNo, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // In Mode = 1 ... show all not authorised yet 
            // In Mode = 2 ... show specific Category and Reconciliation Cycle 
            // In Mode = 3 ... show specific ALL Categories for specific Reconciliation Cycle 

            string WSelectionCriteria; 

            DataTableMatchingExceptions = new DataTable();
            DataTableMatchingExceptions.Clear();

            TotalSelected = 0;     

            // DATA TABLE ROWS DEFINITION 
            DataTableMatchingExceptions.Columns.Add("RecordId", typeof(int));
            DataTableMatchingExceptions.Columns.Add("Origin", typeof(string));
            DataTableMatchingExceptions.Columns.Add("TxnType", typeof(string));
            DataTableMatchingExceptions.Columns.Add("Ccy", typeof(string));
            DataTableMatchingExceptions.Columns.Add("Amount", typeof(string));
            DataTableMatchingExceptions.Columns.Add("Particulars", typeof(string));
            DataTableMatchingExceptions.Columns.Add("UnMatchedName", typeof(string));
            DataTableMatchingExceptions.Columns.Add("System Recommendation", typeof(string));
            DataTableMatchingExceptions.Columns.Add("Action Taken", typeof(string));
            DataTableMatchingExceptions.Columns.Add("ExecutionDtTm", typeof(string));

            if (InMode == 1)
            {
                SqlString = "SELECT *"
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolReconcExceptions] "
                   + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId AND  Authoriser = '' "
                   + " ORDER by SeqNo ASC ";
            }

            if (InMode == 2)
            {
                SqlString = "SELECT *"
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolReconcExceptions] "
                   + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId AND  ReconcCycleNo = @ReconcCycleNo "
                   + " ORDER by SeqNo ASC ";
            }

            if (InMode == 3) // Show all Exceptions for this ReconcCycle Number 
            {
                SqlString = "SELECT *"
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolReconcExceptions] "
                   + " WHERE Operator = @Operator AND  ReconcCycleNo = @ReconcCycleNo "
                   + " ORDER by SeqNo ASC ";
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
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);

                        if (InMode == 2)
                        {
                            cmd.Parameters.AddWithValue("@ReconcCycleNo", InReconcCycleNo);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            ITMXUniqueTxnRef = (string)rdr["ITMXUniqueTxnRef"];
                            ReconcCategoryId = (string)rdr["ReconcCategoryId"];
                            ReconcCycleNo = (int)rdr["ReconcCycleNo"];

                            UnMatchedName = (string)rdr["UnMatchedName"];
                            DefaultAction = (bool)rdr["DefaultAction"];
                            DefaultAction = (bool)rdr["DefaultAction"];

                            ExceptionRecomm = (string)rdr["ExceptionRecomm"];
                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionTypeId = (int)rdr["ActionTypeId"];
                            ActionTypeDescr = (string)rdr["ActionTypeDescr"];

                            ActionByUser = (bool)rdr["ActionByUser"];

                            UserId = (string)rdr["UserId"];

                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            CreatedDtTm = (DateTime)rdr["CreatedDtTm"];
                            Operator = (string)rdr["Operator"];

                            TotalSelected = TotalSelected + 1;                    
                            
                            //Read Record Details

                            WSelectionCriteria = "WHERE MaskRecordId =" + MaskRecordId;

                            Mp.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria);
                            //Fill IN Table 
                            DataRow RowSelected = DataTableMatchingExceptions.NewRow();

                            RowSelected["RecordId"] = MaskRecordId;
                            RowSelected["Origin"] = Mp.RequestOrigin;
                            RowSelected["TxnType"] = Mp.TxnType;
                            RowSelected["Ccy"] = Mp.Ccy;
                            RowSelected["Amount"] = Mp.Amount.ToString("#,##0.00");
                            RowSelected["Particulars"] = Mp.Particulars;
                            RowSelected["UnMatchedName"] = UnMatchedName;
                            RowSelected["System Recommendation"] = ExceptionRecomm;
                            RowSelected["Action Taken"] = ActionTypeDescr;
                            RowSelected["ExecutionDtTm"] = Mp.ExecutionTxnDtTm.ToString();

                            // ADD ROW
                            DataTableMatchingExceptions.Rows.Add(RowSelected);
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
        // Methods 
        // READ MatchingReconcExceptionsInfo to find totals
        // 
        //
        public void ReadMatchingReconcExceptionsInfoForTotals(string InOperator, string InReconcCategoryId, int InReconcCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;
            TotalExceptionsOutstandingToBeAuthorised = 0;
            TotalExceptionsDoneAndAuthorised = 0;
            TotalExceptionsHandleByUser = 0; 


            // DATA TABLE ROWS DEFINITION       

            SqlString = "SELECT *"
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolReconcExceptions] "
                    + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId AND  ReconcCycleNo = @ReconcCycleNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@ReconcCycleNo", InReconcCycleNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            ITMXUniqueTxnRef = (string)rdr["ITMXUniqueTxnRef"];
                            ReconcCategoryId = (string)rdr["ReconcCategoryId"];
                            ReconcCycleNo = (int)rdr["ReconcCycleNo"];

                            UnMatchedName = (string)rdr["UnMatchedName"];
                            DefaultAction = (bool)rdr["DefaultAction"];
                            DefaultAction = (bool)rdr["DefaultAction"];
                            ExceptionRecomm = (string)rdr["ExceptionRecomm"];
                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionTypeId = (int)rdr["ActionTypeId"];
                            ActionTypeDescr = (string)rdr["ActionTypeDescr"];

                            ActionByUser = (bool)rdr["ActionByUser"];

                            UserId = (string)rdr["UserId"];

                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            CreatedDtTm = (DateTime)rdr["CreatedDtTm"];
                            Operator = (string)rdr["Operator"];

                            TotalSelected = TotalSelected + 1;
                            
                            if (Authoriser == "")
                            {
                                TotalExceptionsOutstandingToBeAuthorised = TotalExceptionsOutstandingToBeAuthorised + 1;
                            }
                            else
                            {
                                TotalExceptionsDoneAndAuthorised = TotalExceptionsDoneAndAuthorised + 1;
                            }

                            if (ActionByUser == true)
                            {
                                TotalExceptionsHandleByUser = TotalExceptionsHandleByUser + 1;
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
        // Methods 
        // READ MatchingReconcExceptionsInfo by UniqueTxnRef
        // 
        //
        public void ReadMatchingReconcExceptionsInfobyMaskRecordId(string InOperator, int InMaskRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolReconcExceptions] "
                    + " WHERE Operator = @Operator AND MaskRecordId = @MaskRecordId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            ITMXUniqueTxnRef = (string)rdr["ITMXUniqueTxnRef"];
                            ReconcCategoryId = (string)rdr["ReconcCategoryId"];
                            ReconcCycleNo = (int)rdr["ReconcCycleNo"];

                            UnMatchedName = (string)rdr["UnMatchedName"];
                            DefaultAction = (bool)rdr["DefaultAction"];
                            ExceptionRecomm = (string)rdr["ExceptionRecomm"];
                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionTypeId = (int)rdr["ActionTypeId"];
                            ActionTypeDescr = (string)rdr["ActionTypeDescr"];
                            
                            ActionByUser = (bool)rdr["ActionByUser"];

                            UserId = (string)rdr["UserId"];

                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            CreatedDtTm = (DateTime)rdr["CreatedDtTm"];
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

                    RRDMLog4Net Log = new RRDMLog4Net();

                    StringBuilder WParameters = new StringBuilder();

                    WParameters.Append("User : ");
                    WParameters.Append("NotAssignYet");
                    WParameters.Append(Environment.NewLine);

                    WParameters.Append("ATMNo : ");
                    WParameters.Append("NotDefinedYet");
                    WParameters.Append(Environment.NewLine);

                    string Logger = "RRDM4Atms";
                    string Parameters = WParameters.ToString();

                    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                    if (Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                                 + " . Application will be aborted! Call controller to take care. ");
                    }

                }
        }

        // Insert Matching Exception Info
        public int InsertMatchingReconcExceptionsInfo()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolReconcExceptions]"
                + " ([MaskRecordId],[ITMXUniqueTxnRef],[ReconcCategoryId],[ReconcCycleNo],"
                + "[UnMatchedName],[DefaultAction],[ExceptionRecomm],[MetaExceptionId],"
                + "[ActionTypeId],[ActionTypeDescr],"
                + "[CreatedDtTm],"
                + "[Operator] )"
                + " VALUES"
                + " (@MaskRecordId,@ITMXUniqueTxnRef,@ReconcCategoryId,@ReconcCycleNo, "
                + " @UnMatchedName,@DefaultAction,@ExceptionRecomm,@MetaExceptionId, "
                + " @ActionTypeId,@ActionTypeDescr,"
                + " @CreatedDtTm,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@MaskRecordId", MaskRecordId);
                        cmd.Parameters.AddWithValue("@ITMXUniqueTxnRef", ITMXUniqueTxnRef);
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", ReconcCategoryId);
                        cmd.Parameters.AddWithValue("@ReconcCycleNo", ReconcCycleNo);

                        cmd.Parameters.AddWithValue("@UnMatchedName", UnMatchedName);
                        cmd.Parameters.AddWithValue("@DefaultAction", DefaultAction);
                      
                        cmd.Parameters.AddWithValue("@ExceptionRecomm", ExceptionRecomm);
                        cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);

                        cmd.Parameters.AddWithValue("@ActionTypeId", ActionTypeId);
                        cmd.Parameters.AddWithValue("@ActionTypeDescr", ActionTypeDescr);

                        cmd.Parameters.AddWithValue("@CreatedDtTm", DateTime.Now);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //
                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();


                    CatchDetails(ex);
                }

            return SeqNo;
        }


        // UPDATE MatchingReconcExceptionsInfo by UniqueTxnRef
        // 
        public void UpdateMatchingReconcExceptionsInfo(string InOperator, int InMaskRecordId)
        {
            Successful = false;
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolReconcExceptions] SET "
                            //+ " ExceptionRecomm = @ExceptionRecomm, MetaExceptionId = @MetaExceptionId, "
                            + " MetaExceptionNo = @MetaExceptionNo, ActionTypeId = @ActionTypeId,ActionTypeDescr = @ActionTypeDescr, "
                            + " ActionByUser = @ActionByUser, UserId = @UserId, "
                            + " Authoriser = @Authoriser, AuthoriserDtTm = @AuthoriserDtTm  "
                            + " WHERE MaskRecordId = @MaskRecordId ", conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);

                        //cmd.Parameters.AddWithValue("@ExceptionRecomm", ExceptionRecomm);
                        //cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);
                        cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

                        cmd.Parameters.AddWithValue("@ActionTypeId", ActionTypeId);
                        cmd.Parameters.AddWithValue("@ActionTypeDescr", ActionTypeDescr);
                        cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);
                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);

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
        // DELETE MatchingReconcExceptionsInfo
        //
        public void DeleteMatchingReconcExceptionsInfo(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolReconcExceptions] "
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

        //
        //  
        // READ Exceptions one by one and do as per instructions 
        // 
        //

        int WTxnCode;
        int WDisputeActionId;
     
        public void CreateActionsforMatchingReconcExceptionsInfoClassForALLNotAuthor(string InOperator, string InReconcCategoryId)
        {
            RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();
            //RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();
            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            RRDMGasParameters Gp = new RRDMGasParameters();

            string WOriginName = "ReconciliationCat";

            DataTableMatchingExceptions = new DataTable();
            DataTableMatchingExceptions.Clear();

            TotalSelected = 0;

            RecordFound = false;

            SqlString = "SELECT *"
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolReconcExceptions] "
                   + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId AND  Authoriser = '' ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                       
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableMatchingExceptions);

                        // Close conn
                        conn.Close();
                     

                        int I = 0;

                        while (I <= (DataTableMatchingExceptions.Rows.Count - 1))
                        {

                            RecordFound = true;

                            SeqNo = (int)DataTableMatchingExceptions.Rows[I]["SeqNo"];

                            MaskRecordId = (int)DataTableMatchingExceptions.Rows[I]["MaskRecordId"];
                            ITMXUniqueTxnRef = (string)DataTableMatchingExceptions.Rows[I]["ITMXUniqueTxnRef"];
                            ReconcCategoryId = (string)DataTableMatchingExceptions.Rows[I]["ReconcCategoryId"];
                            ReconcCycleNo = (int)DataTableMatchingExceptions.Rows[I]["ReconcCycleNo"];

                            UnMatchedName = (string)DataTableMatchingExceptions.Rows[I]["UnMatchedName"];
                            ExceptionRecomm = (string)DataTableMatchingExceptions.Rows[I]["ExceptionRecomm"];
                            MetaExceptionId = (int)DataTableMatchingExceptions.Rows[I]["MetaExceptionId"];
                            MetaExceptionNo = (int)DataTableMatchingExceptions.Rows[I]["MetaExceptionNo"];

                            ActionTypeId = (int)DataTableMatchingExceptions.Rows[I]["ActionTypeId"];
                            ActionTypeDescr = (string)DataTableMatchingExceptions.Rows[I]["ActionTypeDescr"];

                            ActionByUser = (bool)DataTableMatchingExceptions.Rows[I]["ActionByUser"];
                            UserId = (string)DataTableMatchingExceptions.Rows[I]["UserId"];

                            Authoriser = (string)DataTableMatchingExceptions.Rows[I]["Authoriser"];
                            AuthoriserDtTm = (DateTime)DataTableMatchingExceptions.Rows[I]["AuthoriserDtTm"];

                            CreatedDtTm = (DateTime)DataTableMatchingExceptions.Rows[I]["CreatedDtTm"];
                            Operator = (string)DataTableMatchingExceptions.Rows[I]["Operator"];

                            Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(Operator, MaskRecordId);

                            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(ReconcCategoryId, ReconcCycleNo, WOriginName);

                            //   if ActionTypeId = 1 => "Creation of posted transaction "
                            //   if ActionTypeId = 2 => "Creation Of Dispute"
                            //   if ActionTypeId = 4 => "Creation Of Disputes For Both Banks"
                            //   if ActionTypeId = 5 => "Creation of SMS Transaction"
                            //
                            if (ActionTypeId == 1) // Normal Transaction 
                            {

                                Tp.OriginId = "02"; // "02" Matching Process   
                                Tp.OriginName = WOriginName;  // ORIGIN RECONCILIATION
                                Tp.RMCateg = ReconcCategoryId;
                                Tp.RMCategCycle = ReconcCycleNo;
                                Tp.UniqueRecordId = MaskRecordId;

                                Tp.ErrNo = 0;
                                Tp.AtmNo = "";
                                Tp.SesNo = 0;
                                Tp.BankId = Operator;

                                Tp.AtmTraceNo = 0;

                                Tp.BranchId = "ITMX HO";
                                Tp.AtmDtTime = DateTime.Now;
                                //Tp.HostDtTime = DateTimeH;
                                Tp.SystemTarget = 11;

                                Tp.CardNo = Mp.MobileRequestor;
                                Tp.CardOrigin = 5; // Find OUT ... 

                                // First Entry 
                                Tp.TransType = 11; // 
                                Tp.TransDesc = "Correction of ITMX To BankA:" + Mp.DebitBank;

                                Tp.AccNo = "ITMX to :" + Mp.DebitBank;
                                Tp.GlEntry = true;


                                // Second Entry


                                Tp.TransType2 = 22;

                                Tp.TransDesc2 = "Correction of ITMX To BankA:" + Mp.CreditBank;

                                Tp.AccNo2 = "ITMX to :" + Mp.CreditBank;

                                Tp.GlEntry2 = true;
                                // End Second Entry 

                                Tp.CurrDesc = Mp.Ccy;
                                Tp.TranAmount = Mp.Amount;
                                Tp.AuthCode = 0;
                                Tp.RefNumb = 0;
                                Tp.RemNo = 0;
                                Tp.TransMsg = "No comment from user";
                                Tp.AtmMsg = "";
                                Tp.MakerUser = UserId;
                                Tp.AuthUser = "";
                                Tp.OpenDate = DateTime.Now;
                                Tp.OpenRecord = true;
                                Tp.Operator = Operator;
                                //NOSTRO
                                Tp.NostroCcy = "";
                                Tp.NostroAdjAmt = 0;

                                int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

                                // UPDATE AND CLOSE UNMATCHED AND META EXCEPTIONS 
                                if (Tp.ErrorFound == false)
                                {
                                    MetaExceptionNo = PostedNo;
                                    Authoriser = Ap.Authoriser;
                                    AuthoriserDtTm = DateTime.Now;
                                    UpdateMatchingReconcExceptionsInfo(Operator, MaskRecordId);
                                }

                            }

                            if (ActionTypeId == 2) // Creation of Dispute + Transaction 
                            {
                                RRDMDisputesTableClassITMX Di = new RRDMDisputesTableClassITMX();
                                RRDMDisputeTransactionsClassITMX Dt = new RRDMDisputeTransactionsClassITMX();

                                if (MetaExceptionId == 47) // Dispute to Bank A
                                {
                                    Di.BankId = Mp.DebitBank;
                                    Di.CardNo = Mp.MobileRequestor;
                                    Di.CustPhone = Mp.MobileRequestor;
                                    Di.AccNo = Mp.AccountRequestor;
                                    Di.DispComments = "It appears in our records that your Bank didnt Debit the account " + Mp.AccountRequestor
                                                       + " with the amount of " + Mp.Amount.ToString() + ". Please investigate and act on this dispute";

                                    Dt.BankId = Mp.DebitBank;
                                    WTxnCode = 11;
                                    WDisputeActionId = 3;
                                }

                                if (MetaExceptionId == 50) // Dispute to Bank B
                                {
                                    Di.BankId = Mp.CreditBank;
                                    Di.CardNo = Mp.MobileBeneficiary;
                                    Di.CustPhone = Mp.MobileBeneficiary;
                                    Di.AccNo = Mp.AccountBeneficiary;
                                    Di.DispComments = "It appears in our records that your Bank didnt Credit the account " + Mp.AccountRequestor
                                                       + " with the amount of " + Mp.Amount.ToString() + ". Please investigate and act timely on this dispute!";

                                    Dt.BankId = Mp.CreditBank;
                                    WTxnCode = 21;
                                    WDisputeActionId = 1;
                                }

                                Di.RespBranch = "Head Office :" + Di.BankId;
                                Di.LastUpdateDtTm = DateTime.Now;
                                Di.DispFrom = 7;

                                Di.CreatedByEntity = "ITMX";
                                Di.DispType = 0;

                                Di.DispType = 5; // Reconciliation Difference 

                                Di.OpenDate = DateTime.Now;
                                DateTime today = DateTime.Now;

                                Gp.ReadParametersSpecificId(Operator, "605", "1", "", ""); // LIMIT to be solved date // Dispute target dates 
                                int QualityRange1 = (int)Gp.Amount;

                                Di.TargetDate = today.AddDays(QualityRange1);

                                Di.CloseDate = NullPastDate;

                                Di.CustName = "George Koulis";

                                Di.CustEmail = "koulis.george@cablenet.com.cy";

                                Di.OtherDispTypeDescr = UnMatchedName;

                                Di.VisitType = "ITMX was called by customer";

                                Di.OpenByUserId = UserId;

                                Di.Active = true;

                                Di.Operator = Operator;

                                Di.DispId = Di.InsertDisputeRecord();

                                if (Mp.RecordFound == true)
                                {
                                    //FoundInMatchedUnMatched = true;
                                    Dt.MaskRecordId = Mp.MaskRecordId;

                                    Dt.DispDtTm = Di.OpenDate;

                                    Dt.DisputedAmt = Mp.Amount;

                                    Dt.DisputeNumber = Di.DispId;

                                    Dt.Origin = WOriginName;

                                    //For ATMS
                                    Dt.ErrNo = 0;
                                    Dt.ReplCycle = 0;

                                    Dt.StartTrxn = 0;
                                    Dt.StartTrxn = 0;
                                    Dt.Operator = Operator;

                                    int DTranNo = Dt.InsertDisputeTran(Di.DispId);

                                    Dt.ReadDisputeTran(DTranNo); // Get all values
                                                                 // Prepare the two values 
                                    Dt.TxnCode = WTxnCode;
                                    Dt.DisputeActionId = WDisputeActionId;

                                    Dt.UpdateDisputeTranRecord(DTranNo);

                                }

                                // UPDATE AND CLOSE UNMATCHED AND META EXCEPTIONS 
                                if (Tp.ErrorFound == false)
                                {
                                    MetaExceptionNo = Di.DispId;
                                    Authoriser = Ap.Authoriser;
                                    AuthoriserDtTm = DateTime.Now;
                                    UpdateMatchingReconcExceptionsInfo(Operator, MaskRecordId);
                                }
                            }

                            if (ActionTypeId == 3) // SMS 
                            {

                                if (MetaExceptionId == 52) // Send SMS TO CUSTOMER 
                                {

                                }

                                // UPDATE AND CLOSE UNMATCHED AND META EXCEPTIONS 

                                MetaExceptionNo = 0;
                                Authoriser = Ap.Authoriser;
                                AuthoriserDtTm = DateTime.Now;
                                UpdateMatchingReconcExceptionsInfo(Operator, MaskRecordId);

                            }

                            if (ActionTypeId != 5) // Normal Transaction 
                                                   // if 5 is postponed 
                            {
                                string SelectionCriteria = " WHERE MaskRecordId =" + MaskRecordId;
                                Mp.ReadMatchingTxnsMasterPoolBySelectionCriteria("");

                                Mp.ActionTaken = true;
                                Mp.SettledRecord = true;

                                Mp.UpdateMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(MaskRecordId);
                            }

                            I++; // Read Next entry of the table 
                        }
                    }
                }

                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }



    }
}



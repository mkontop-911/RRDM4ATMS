using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMDisputeTransactionsClassITMX : Logger
    {
        public RRDMDisputeTransactionsClassITMX() : base() { }

        //Declare dispute TRAN fields

        public int DispTranNo;

        public int MaskRecordId; 
 
        public int DisputeNumber;

        public DateTime DispDtTm; 

        public string BankId;
     
        public string Origin;
        // "OurATMs"
        // "ProcessorATMs"
        // "ProcessorMerchants" 
        // ReconciliationCat
        public int TxnCode; 
        public int ErrNo; 
        public int ReplCycle; 
        public int StartTrxn; 
        public int EndTrxn; 
        public decimal DisputedAmt; 
        public int DisputeActionId;
        public DateTime ActionDtTm; 
        public decimal DecidedAmount;  
        public int ReasonForAction; 
        public string ActionComment;
        public DateTime PostDate;
 
        //****************
        public bool ChooseAuthor; 
        public bool PendingAuthorization; 
        public String AuthorOriginator;
        public int AuthorKey ; 
        public string Authoriser; 
        public bool Authorised;
        public bool RejectedFromAuth;
        public string AuthoriserComment; 
        //****************

        public bool ClosedDispute;
        public string Operator; 

        public int OpenDispTran; // Total
        public int SettleDispTran; // Total 

        // Define the data table 
        public DataTable DisputeTransDataTable = new DataTable();

        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();
        RRDMMatchingReconcExceptionsInfoITMX Mre = new RRDMMatchingReconcExceptionsInfoITMX(); 

        // READ DISPUTE Transaction 
        public void ReadDisputeTransDataTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            DisputeTransDataTable = new DataTable();
            DisputeTransDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DisputeTransDataTable.Columns.Add("DispTranNo", typeof(int));
            DisputeTransDataTable.Columns.Add("MaskRecordId", typeof(int));
            DisputeTransDataTable.Columns.Add("TranDate", typeof(DateTime));
            DisputeTransDataTable.Columns.Add("TranAmount", typeof(string));
            DisputeTransDataTable.Columns.Add("DisputedAmt", typeof(string));
            DisputeTransDataTable.Columns.Add("DecidedAmount", typeof(string));

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[DisputesTransTableITMX] "
                + " WHERE " + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@DispTranNo", InDispTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            DispTranNo = (int)rdr["DispTranNo"];

                            MaskRecordId = (int)rdr["MaskRecordId"];

                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DispDtTm = (DateTime)rdr["DispDtTm"];

                            BankId = (string)rdr["BankId"];

                            Origin = (string)rdr["Origin"];

                            TxnCode = (int)rdr["TxnCode"];
                            ErrNo = (int)rdr["ErrNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DisputedAmt = (decimal)rdr["DisputedAmt"];

                            DisputeActionId = (int)rdr["DisputeActionId"];

                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            DecidedAmount = (decimal)rdr["DecidedAmount"];
                            ReasonForAction = (int)rdr["ReasonForAction"];
                            ActionComment = (string)rdr["ActionComment"];

                            PostDate = (DateTime)rdr["PostDate"];
                            //***
                            ChooseAuthor = (bool)rdr["ChooseAuthor"];
                            PendingAuthorization = (bool)rdr["PendingAuthorization"];
                            AuthorOriginator = (string)rdr["AuthorOriginator"];
                            AuthorKey = (int)rdr["AuthorKey"];
                            Authoriser = (string)rdr["Authoriser"];
                            Authorised = (bool)rdr["Authorised"];
                            RejectedFromAuth = (bool)rdr["RejectedFromAuth"];
                            AuthoriserComment = (string)rdr["AuthoriserComment"];
                            //****

                            ClosedDispute = (bool)rdr["ClosedDispute"];
                            Operator = (string)rdr["Operator"];

                            Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(Operator, MaskRecordId);  

                            //Fill Table 

                            DataRow RowSelected = DisputeTransDataTable.NewRow();

                            RowSelected["DispTranNo"] = DispTranNo;
                            RowSelected["MaskRecordId"] = MaskRecordId;
                            RowSelected["TranDate"] = Mp.ExecutionTxnDtTm.ToString();
                            RowSelected["TranAmount"] = Mp.Amount.ToString("#,##0.00");
                            RowSelected["DisputedAmt"] = DisputedAmt.ToString("#,##0.00");
                            RowSelected["DecidedAmount"] = DecidedAmount.ToString("#,##0.00");

                            // ADD ROW
                            DisputeTransDataTable.Rows.Add(RowSelected);

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

        // READ DISPUTE Transaction 
        public void ReadDisputeTran(int InDispTranNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[DisputesTransTableITMX] "
          + " WHERE DispTranNo = @DispTranNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DispTranNo", InDispTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
      
                            // Read Dispute Details
                            DispTranNo = (int)rdr["DispTranNo"];

                            MaskRecordId = (int)rdr["MaskRecordId"];

                            DisputeNumber= (int)rdr["DisputeNumber"];
                            DispDtTm = (DateTime)rdr["DispDtTm"];
                            
                            BankId = (string)rdr["BankId"];        

                            Origin = (string)rdr["Origin"];

                            TxnCode = (int)rdr["TxnCode"];
                            ErrNo = (int)rdr["ErrNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DisputedAmt = (decimal)rdr["DisputedAmt"];

                            DisputeActionId = (int)rdr["DisputeActionId"];

                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            DecidedAmount = (decimal)rdr["DecidedAmount"];
                            ReasonForAction = (int)rdr["ReasonForAction"];
                            ActionComment = (string)rdr["ActionComment"];

                            PostDate = (DateTime)rdr["PostDate"];
                            //***
                            ChooseAuthor = (bool)rdr["ChooseAuthor"];
                            PendingAuthorization = (bool)rdr["PendingAuthorization"];
                            AuthorOriginator = (string)rdr["AuthorOriginator"];
                            AuthorKey = (int)rdr["AuthorKey"];
                            Authoriser = (string)rdr["Authoriser"];
                            Authorised = (bool)rdr["Authorised"];
                            RejectedFromAuth = (bool)rdr["RejectedFromAuth"];
                            AuthoriserComment = (string)rdr["AuthoriserComment"];
                            //****

                            ClosedDispute = (bool)rdr["ClosedDispute"];
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

        // READ DISPUTE Transaction based on MaskRecordId
        public void ReadDisputeTranByMaskRecordId(int InMaskRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
             + " FROM [ATMS].[dbo].[DisputesTransTableITMX] "
             + " WHERE MaskRecordId = @MaskRecordId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);
                    
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            DispTranNo = (int)rdr["DispTranNo"];
                            MaskRecordId = (int)rdr["MaskRecordId"];
                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DispDtTm = (DateTime)rdr["DispDtTm"];

                            BankId = (string)rdr["BankId"];
                   

                            Origin = (string)rdr["Origin"];

                            TxnCode = (int)rdr["TxnCode"];
                            ErrNo = (int)rdr["ErrNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DisputedAmt = (decimal)rdr["DisputedAmt"];

                            DisputeActionId = (int)rdr["DisputeActionId"];

                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            DecidedAmount = (decimal)rdr["DecidedAmount"];
                            ReasonForAction = (int)rdr["ReasonForAction"];
                            ActionComment = (string)rdr["ActionComment"];

                            PostDate = (DateTime)rdr["PostDate"];

                            //***
                            ChooseAuthor = (bool)rdr["ChooseAuthor"];
                            PendingAuthorization = (bool)rdr["PendingAuthorization"];
                            AuthorOriginator = (string)rdr["AuthorOriginator"];
                            AuthorKey = (int)rdr["AuthorKey"];
                            Authoriser = (string)rdr["Authoriser"];
                            Authorised = (bool)rdr["Authorised"];
                            RejectedFromAuth = (bool)rdr["RejectedFromAuth"];
                            AuthoriserComment = (string)rdr["AuthoriserComment"];
                            //****

                            ClosedDispute = (bool)rdr["ClosedDispute"];
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

      

        public int InsertDisputeTran(int InDisputeNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[DisputesTransTableITMX] "
                + " ([MaskRecordId],[DisputeNumber], [DispDtTm],[BankId],[Origin],"
                + " [ErrNo], [ReplCycle], [StartTrxn], [EndTrxn],"
                + " [DisputedAmt], "
                + " [Operator])"
                + " VALUES (@MaskRecordId,@DisputeNumber,@DispDtTm, @BankId,"
                + " @Origin, "
                + " @ErrNo, @ReplCycle, @StartTrxn, @EndTrxn,"
                + " @DisputedAmt, "
                + " @Operator)"
                +" SELECT CAST(SCOPE_IDENTITY() AS int)";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                      
                        cmd.Parameters.AddWithValue("@MaskRecordId", MaskRecordId);
                        cmd.Parameters.AddWithValue("@DisputeNumber", DisputeNumber);
                        cmd.Parameters.AddWithValue("@DispDtTm", DispDtTm);
                        cmd.Parameters.AddWithValue("@BankId", BankId);                

                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        cmd.Parameters.AddWithValue("@ErrNo", ErrNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);
                        cmd.Parameters.AddWithValue("@StartTrxn", StartTrxn);
                        cmd.Parameters.AddWithValue("@EndTrxn", EndTrxn);

                        cmd.Parameters.AddWithValue("@DisputedAmt", DisputedAmt);
                    
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        DispTranNo = (int)cmd.ExecuteScalar();

                    }

                    // Close conn
                    conn.Close();

                }

                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return DispTranNo; 

        }

        // UPDATE   
        public void UpdateDisputeTranRecord(int InDiTranNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[DisputesTransTableITMX] SET "
                                      + "[MaskRecordId] = @MaskRecordId,[DisputeNumber] = @DisputeNumber,[DispDtTm] = @DispDtTm, [BankId] = @BankId,"
                                      + "  [Origin] = @Origin,"
                                      + " [TxnCode] = @TxnCode,[ErrNo] = @ErrNo, [ReplCycle] = @ReplCycle,"
                                      + " [StartTrxn] = @StartTrxn, [EndTrxn] = @EndTrxn, [DisputedAmt] = @DisputedAmt,"
                                      + " [DisputeActionId] = @DisputeActionId, [ActionDtTm] = @ActionDtTm,"
                                      + " [DecidedAmount] = @DecidedAmount, [ReasonForAction] = @ReasonForAction,"
                                      + " [ActionComment] = @ActionComment, [PostDate] = @PostDate,"
                                      + " [ChooseAuthor] = @ChooseAuthor,"
                                      + " [PendingAuthorization] = @PendingAuthorization, [AuthorOriginator] = @AuthorOriginator,"
                                      + " [AuthorKey] = @AuthorKey, [Authoriser] = @Authoriser,"
                                      + " [Authorised] = @Authorised, [RejectedFromAuth] = @RejectedFromAuth,"
                                      + " [AuthoriserComment] = @AuthoriserComment," 
                                      + " [ClosedDispute] = @ClosedDispute"
                                      + " WHERE DispTranNo = @DispTranNo", conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@DispTranNo", InDiTranNo);

                        cmd.Parameters.AddWithValue("@MaskRecordId", MaskRecordId);

                        cmd.Parameters.AddWithValue("@DisputeNumber", DisputeNumber);
                        cmd.Parameters.AddWithValue("@DispDtTm", DispDtTm);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        cmd.Parameters.AddWithValue("@TxnCode", TxnCode);
                        cmd.Parameters.AddWithValue("@ErrNo", ErrNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);
                        cmd.Parameters.AddWithValue("@StartTrxn", StartTrxn);
                        cmd.Parameters.AddWithValue("@EndTrxn", EndTrxn);

                        cmd.Parameters.AddWithValue("@DisputedAmt", DisputedAmt);
                        cmd.Parameters.AddWithValue("@DisputeActionId", DisputeActionId);
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);
                        cmd.Parameters.AddWithValue("@DecidedAmount", DecidedAmount);
                        cmd.Parameters.AddWithValue("@ReasonForAction", ReasonForAction);

                        cmd.Parameters.AddWithValue("@ActionComment", ActionComment);
                        cmd.Parameters.AddWithValue("@PostDate", PostDate);

                        //********
                        cmd.Parameters.AddWithValue("@ChooseAuthor", ChooseAuthor);
                        cmd.Parameters.AddWithValue("@PendingAuthorization", PendingAuthorization);
                        cmd.Parameters.AddWithValue("@AuthorOriginator", AuthorOriginator);
                        cmd.Parameters.AddWithValue("@AuthorKey", AuthorKey);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@Authorised", Authorised);
                        cmd.Parameters.AddWithValue("@RejectedFromAuth", RejectedFromAuth);
                        cmd.Parameters.AddWithValue("@AuthoriserComment", AuthoriserComment);        
                        //**

                        cmd.Parameters.AddWithValue("@ClosedDispute", ClosedDispute);


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
        // READ DISPUTE Transaction for TOTALS
        public void ReadAllTranForDispute(int InDisputeNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            OpenDispTran = 0;
            SettleDispTran = 0; 
         
            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[DisputesTransTableITMX] "
          + " WHERE DisputeNumber = @DisputeNumber";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DisputeNumber", InDisputeNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                           
                            ClosedDispute = (bool)rdr["ClosedDispute"];

                            if (ClosedDispute == false) OpenDispTran = OpenDispTran + 1;
                            if (ClosedDispute == true) SettleDispTran = SettleDispTran + 1; 
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
        // CLEAR ALL DISPUTE Transactions for a Specific Dispute Number  
        public void DeleteTransOfthisDispute(int InDisputeNumber)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "DELETE "
          + " FROM [ATMS].[dbo].[DisputesTransTableITMX] "
          + " WHERE DisputeNumber = @DisputeNumber";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DisputeNumber", InDisputeNumber);
                  
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

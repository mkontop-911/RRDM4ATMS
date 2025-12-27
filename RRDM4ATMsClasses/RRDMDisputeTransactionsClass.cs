using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMDisputeTransactionsClass : Logger
    {
        public RRDMDisputeTransactionsClass() : base() { }

        //Declare dispute TRAN fields

        public int DispTranNo;

        public int UniqueRecordId; 
 
        public int DisputeNumber;
        //

        public DateTime DispDtTm; 

        public string BankId;
     
        public int  DbTranNo;
        //

        public string Origin; // "ATM" OR "JCC" OR "RMCategory"; 
                              // "OurATMs"
                              // "ProcessorATMs"
                              // "ProcessorMerchants" 
        public DateTime TranDate;

        public string AtmNo;
        //
        public int ReplGroup; 
        public string CardType; 
        public string CardNo;

        public string AccNo;
        public string CurrencyNm; 
        public decimal TranAmount; 

        public int SystemTarget; 
        public int TransType; 
        public string TransDesc; 

        public int ErrNo; 
        public int ReplCycle; 
        public int StartTrxn; 

        public int EndTrxn; 
        public decimal DisputedAmt; 
        public int DisputeActionId;
        //string WActionDesc = "Not Specified";
        //        if (Dt.DisputeActionId == 1) WActionDesc = "Customer was credited";
        //        if (Dt.DisputeActionId == 2) WActionDesc = "Customer was credited";
        //        if (Dt.DisputeActionId == 3) WActionDesc = "Customer was Debited";
        //        if (Dt.DisputeActionId == 4) WActionDesc = "Customer was Debited";
        //        if (Dt.DisputeActionId == 5) WActionDesc = "Postponed";
        //        if (Dt.DisputeActionId == 6) WActionDesc = "Dispute Txn Cancelled";
        public DateTime ActionDtTm; 
        public decimal DecidedAmount;  
        public int ReasonForAction; 

        public string ActionComment;
        public DateTime PostDate;
 
        //****************
        public bool ChooseAuthor; 
        public bool PendingAuthorization; 
        public string AuthorOriginator;
        public int AuthorKey ; 
        public string Authoriser; 
        public bool Authorised;
        public bool RejectedFromAuth;
        public string AuthoriserComment; 
        //****************

        public bool ClosedDispute;
        public string Operator;

        //
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public int OpenDispTran; // Total

        public int SettleDispTran; // Total 

        // Define the data table 
        public DataTable DisputeTransDataTable = new DataTable();

        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        private void ReadDispTxnFields(SqlDataReader rdr)
        {
            // Read Dispute Details
            DispTranNo = (int)rdr["DispTranNo"];
            UniqueRecordId = (int)rdr["UniqueRecordId"];
            DisputeNumber = (int)rdr["DisputeNumber"];
            DispDtTm = (DateTime)rdr["DispDtTm"];

            BankId = (string)rdr["BankId"];

            DbTranNo = (int)rdr["DbTranNo"];

            Origin = (string)rdr["Origin"];

            TranDate = (DateTime)rdr["TranDate"];

            AtmNo = (string)rdr["AtmNo"];
            CardType = (string)rdr["CardType"];
            CardNo = (string)rdr["CardNo"];
            AccNo = (string)rdr["AccNo"];
            CurrencyNm = (string)rdr["CurrencyNm"];

            TranAmount = (decimal)rdr["TranAmount"];

            SystemTarget = (int)rdr["SystemTarget"];
            TransType = (int)rdr["TransType"];
            TransDesc = (string)rdr["TransDesc"];
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

        // READ DISPUTE Transaction 
        public void ReadDisputeTransDataTable(int InDisputeNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
           
            DisputeTransDataTable = new DataTable();
            DisputeTransDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DisputeTransDataTable.Columns.Add("DispTranNo", typeof(int));
            DisputeTransDataTable.Columns.Add("UniqueRecordId", typeof(int));
            DisputeTransDataTable.Columns.Add("TranDate", typeof(DateTime));
            DisputeTransDataTable.Columns.Add("TranAmount", typeof(string));
            DisputeTransDataTable.Columns.Add("DisputedAmt", typeof(string));
            DisputeTransDataTable.Columns.Add("DecidedAmount", typeof(string));
            DisputeTransDataTable.Columns.Add("ActionDate", typeof(string));
            DisputeTransDataTable.Columns.Add("ClosedDispute", typeof(bool));
            DisputeTransDataTable.Columns.Add("Action Desc", typeof(string));

            string SqlString = "SELECT *"
               + " FROM ATMS.[dbo].[DisputesTransTable] "
                + " WHERE DisputeNumber = @DisputeNumber ";

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
                            ReadDispTxnFields(rdr);
                            
                            //Fill Table 

                            DataRow RowSelected = DisputeTransDataTable.NewRow();

                            RowSelected["DispTranNo"] = DispTranNo;
                            RowSelected["UniqueRecordId"] = UniqueRecordId;
                            RowSelected["TranDate"] = TranDate.ToString();
                            RowSelected["TranAmount"] = TranAmount.ToString("#,##0.00");
                            RowSelected["DisputedAmt"] = DisputedAmt.ToString("#,##0.00");
                            RowSelected["DecidedAmount"] = DecidedAmount.ToString("#,##0.00");
                            RowSelected["ActionDate"] = ActionDtTm.ToString();
                            RowSelected["ClosedDispute"] = ClosedDispute;

                            RowSelected["Action Desc"] = "Not Specified";
                            if (DisputeActionId == 1) RowSelected["Action Desc"] = "1_Customer was credited";
                            if (DisputeActionId == 2) RowSelected["Action Desc"] = "2_Customer was credited";
                            if (DisputeActionId == 3) RowSelected["Action Desc"] = "3_Customer was Debited";
                            if (DisputeActionId == 4) RowSelected["Action Desc"] = "4_Customer was Debited";
                            if (DisputeActionId == 5) RowSelected["Action Desc"] = "5_Postponed";
                            if (DisputeActionId == 6) RowSelected["Action Desc"] = "6_Dispute Txn Cancelled";


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

            string SqlString = "SELECT * "
          + " FROM ATMS.[dbo].[DisputesTransTable] "
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
                            ReadDispTxnFields(rdr);
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
        public void ReadDisputeTranByATMAndReplCycle(string InAtmNo , int InReplCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            // AND DisputeActionId != 6 and not cancelled 
            RRDMDisputesTableClass Di = new RRDMDisputesTableClass(); 

            string SqlString = "SELECT TOP (1) * "
          + " FROM ATMS.[dbo].[DisputesTransTable] "
          + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle AND ClosedDispute = 0 ";
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

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            ReadDispTxnFields(rdr);
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


        // READ DISPUTE Transaction based on UniqueRecordId
        public void ReadDisputeTranByUniqueRecordId(int InUniqueRecordId )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 
            // Show disputes not canceled => Outstanding or Settled to customer 
            string SqlString = "SELECT *"
             + " FROM ATMS.[dbo].[DisputesTransTable] "
             + " WHERE UniqueRecordId = @UniqueRecordId AND (" 
             + "(DisputeActionId <> 6) OR (DisputeActionId = 6 AND ClosedDispute = 0) " 
             + ")";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                    
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            ReadDispTxnFields(rdr);

                            
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

        // READ DISPUTE Transaction including Cancel ones based on UniqueRecordId
        public void ReadDisputeTranByUniqueRecordIdIncludingCancel(int InUniqueRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //
            // Show disputes including canceled ones based on Unique Record Id
            //
            string SqlString = "SELECT *"
             + " FROM ATMS.[dbo].[DisputesTransTable] "
             + " WHERE UniqueRecordId = @UniqueRecordId " ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            ReadDispTxnFields(rdr);

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

        // Create a psaudo Dispute 
        public void Create_Pseudo_Dispute_TXN(string InOperator, string InSignedId, int InDisputeNo, int InUniqueRecordId, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
            Di.ReadDispute(InDisputeNo);
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            string SelectionCriteria = " WHERE UniqueRecordId = " + InUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //  DispId = (int)rdr["DispId"]; // Set Up automatically

           // DispTranNo = (int)rdr["DispTranNo"];
            UniqueRecordId = InUniqueRecordId;
            DisputeNumber = InDisputeNo;
            DispDtTm = Di.OpenDate;

            BankId = Di.BankId;

            DbTranNo = Mpa.SeqNo ;

            Origin = "ATM";

            TranDate = DateTime.Now;

            AtmNo = Mpa.TerminalId;
            ReplGroup = 999; 
            CardType = "Chip";
            CardNo = Mpa.CardNumber;
            AccNo = Mpa.AccNumber;
            CurrencyNm = Mpa.TransCurr;

            TranAmount = Mpa.TransAmount;

            SystemTarget = 1 ;
            TransType = Mpa.TransType;
            TransDesc = Mpa.TransDescr;
            ErrNo = 0 ;
            ReplCycle = Mpa.ReplCycleNo;

            StartTrxn = 0;
            EndTrxn = 0;

            DisputedAmt = Mpa.TransAmount;

            DisputeActionId = 0;

            ActionDtTm = NullPastDate; 
            DecidedAmount = 0 ;
            ReasonForAction = 0;
            ActionComment = "";

            PostDate = NullPastDate;

            //***
            ChooseAuthor = false;
            PendingAuthorization = false ;
            AuthorOriginator = "";
            AuthorKey = 0;
            Authoriser = "";
            Authorised = false;
            RejectedFromAuth = false;
            AuthoriserComment = "";
            //****

            ClosedDispute = false;
            Operator = Mpa.Operator;


            InsertDisputeTran(InDisputeNo); 

         

        }


        public void InsertDisputeTran(int InDisputeNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO ATMS.[dbo].[DisputesTransTable]"
                + " ([UniqueRecordId],[DisputeNumber], [DispDtTm],[BankId],[DbTranNo],[Origin],"
                + " [TranDate], [AtmNo], [ReplGroup], [CardType], [CardNo], [AccNo],"
                + " [CurrencyNm], [TranAmount], [SystemTarget], [TransType], [TransDesc],"
                + " [ErrNo], [ReplCycle], [StartTrxn], [EndTrxn],"
                + " [DisputedAmt], [DisputeActionId], [ActionDtTm], [DecidedAmount], [ReasonForAction],"
                + " [ActionComment], [ClosedDispute] , [Operator])"
                + " VALUES (@UniqueRecordId,@DisputeNumber,@DispDtTm, @BankId,"
                + " @DbTranNo, @Origin, @TranDate, @AtmNo,@ReplGroup, @CardType, @CardNo, @AccNo,"
                + " @CurrencyNm, @TranAmount, @SystemTarget, @TransType, @TransDesc,"
                + " @ErrNo, @ReplCycle, @StartTrxn, @EndTrxn,"
                + " @DisputedAmt, @DisputeActionId, @ActionDtTm, @DecidedAmount, @ReasonForAction,"
                + " @ActionComment, @ClosedDispute, @Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);
                        cmd.Parameters.AddWithValue("@DisputeNumber", DisputeNumber);
                        cmd.Parameters.AddWithValue("@DispDtTm", DispDtTm);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@DbTranNo", DbTranNo);

                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        cmd.Parameters.AddWithValue("@TranDate", TranDate);
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@ReplGroup", ReplGroup);

                        cmd.Parameters.AddWithValue("@CardType", CardType);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);

                        cmd.Parameters.AddWithValue("@CurrencyNm", CurrencyNm);
                        cmd.Parameters.AddWithValue("@TranAmount", TranAmount);
                        cmd.Parameters.AddWithValue("@SystemTarget", SystemTarget);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDesc", TransDesc);

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

                        cmd.Parameters.AddWithValue("@ClosedDispute", ClosedDispute);

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

            //RecordFound = true;

        }

        // UPDATE   
        public void UpdateDisputeTranRecord(int TranNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE DisputesTransTable SET "
                                      + "[UniqueRecordId] = @UniqueRecordId,[DisputeNumber] = @DisputeNumber,[DispDtTm] = @DispDtTm, [BankId] = @BankId,"
                                      + " [DbTranNo] = @DbTranNo, [Origin] = @Origin, [TranDate] = @TranDate,"
                                      + " [AtmNo] = @AtmNo, [ReplGroup] = @ReplGroup, [CardType] = @CardType, [CardNo] = @CardNo, [AccNo] = @AccNo,"
                                      + " [CurrencyNm] = @CurrencyNm, [TranAmount] = @TranAmount,"
                                      + " [SystemTarget] = @SystemTarget, [TransType] = @TransType,"
                                      + " [TransDesc] = @TransDesc, [ErrNo] = @ErrNo, [ReplCycle] = @ReplCycle,"
                                      + " [StartTrxn] = @StartTrxn, [EndTrxn] = @EndTrxn, [DisputedAmt] = @DisputedAmt,"
                                      + " [DisputeActionId] = @DisputeActionId, [ActionDtTm] = @ActionDtTm,"
                                      + " [DecidedAmount] = @DecidedAmount, [ReasonForAction] = @ReasonForAction,"
                                      + " [ActionComment] = @ActionComment, [PostDate] = @PostDate,"
                                      + " [ChooseAuthor] = @ChooseAuthor,"
                                      + " [PendingAuthorization] = @PendingAuthorization, [AuthorOriginator] = @AuthorOriginator,"
                                      + " [AuthorKey] = @AuthorKey, [Authoriser] = @Authoriser,"
                                      + " [Authorised] = @Authorised, [RejectedFromAuth] = @RejectedFromAuth,"
                                      + " [AuthoriserComment] = @AuthoriserComment," 
                                      + "[ClosedDispute] = @ClosedDispute"
                                     + " WHERE DispTranNo = @DispTranNo", conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@DispTranNo", DispTranNo);

                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);

                        cmd.Parameters.AddWithValue("@DisputeNumber", DisputeNumber);
                        cmd.Parameters.AddWithValue("@DispDtTm", DispDtTm);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@DbTranNo", DbTranNo);

                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        cmd.Parameters.AddWithValue("@TranDate", TranDate);
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@CardType", CardType);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@ReplGroup", ReplGroup);
                        cmd.Parameters.AddWithValue("@CurrencyNm", CurrencyNm);
                        cmd.Parameters.AddWithValue("@TranAmount", TranAmount);
                        cmd.Parameters.AddWithValue("@SystemTarget", SystemTarget);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDesc", TransDesc);

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
            string SqlString;

            // Mode = 1
            // Mode = 2

            OpenDispTran = 0;
            SettleDispTran = 0; 
        
                SqlString = "SELECT *"
                + " FROM ATMS.[dbo].[DisputesTransTable] "
                + " WHERE DisputeNumber = @DisputeNumber ";
           
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

        public void DeletethisDispute(int InDisputeNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "DELETE "
          + " FROM [ATMS].[dbo].[DisputesTable] "
          + " WHERE DispId = @DisputeNumber";
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
        public void DeleteTransOfthisDispute(int InDisputeNumber)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "DELETE "
          + " FROM ATMS.[dbo].[DisputesTransTable] "
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    class RRDMDisputeTrasactionClass
    {
        //Declare dispute TRAN fields
        
        public int DispTranNo;
        public int DisputeNumber;

        public DateTime DispDtTm; 

        public string BankId;
     
        public int  DbTranNo;
        public DateTime TranDate;

        public string AtmNo;
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

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

       
        // READ DISPUTE Transaction 
        public void ReadDisputeTran(int InDispTranNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[DisputesTransTable] "
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
                            DisputeNumber= (int)rdr["DisputeNumber"];
                            DispDtTm = (DateTime)rdr["DispDtTm"];
                            
                            BankId = (string)rdr["BankId"];
              

                            DbTranNo = (int)rdr["DbTranNo"];

                            TranDate = (DateTime)rdr["TranDate"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplGroup = (int)rdr["ReplGroup"];

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
                    ErrorOutput = "An error occured in Dispute Trans Class............. " + ex.Message;
                }
        }

        // READ DISPUTE Transaction based on DB In Pool TRAN Number 
        public void ReadDisputeTranForInPool(int InPoolTranNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
             + " FROM [dbo].[DisputesTransTable] "
             + " WHERE DbTranNo = @DbTranNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DbTranNo", InPoolTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            DispTranNo = (int)rdr["DispTranNo"];
                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DispDtTm = (DateTime)rdr["DispDtTm"];

                            BankId = (string)rdr["BankId"];
                   

                            DbTranNo = (int)rdr["DbTranNo"];

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
                    ErrorOutput = "An error occured in Dispute Trans Class............. " + ex.Message;
                }
        }

        /*
        public void ReadDisputeLastNo(int InUserNo)
        {
            RecordFound = false;

            string SqlString = "SELECT * FROM Dispute"
                   + " WHERE DispId = (SELECT MAX(DispId) FROM Dispute) AND UserNo = @UserNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserNo", InUserNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            DispId = (int)rdr["DispId"];
                            LastUpdate = (DateTime)rdr["DateTime"];
                            DispForm = (int)rdr["DispForm"];
                            DispType = (int)rdr["DispType"];
                            OpenDtTm = (DateTime)rdr["OpenDate"];
                            TargetDate = (DateTime)rdr["TargetDate"];
                            CloseDate = (DateTime)rdr["CloseDate"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];
                            CustName = (string)rdr["CustName"];
                            CustPhone = (string)rdr["CustPhone"];
                            CustEmail = (string)rdr["CustEmail"];
                            DateFrom = (DateTime)rdr["DateFrom"];
                            DateTo = (DateTime)rdr["DateTo"];
                            DispComments = (string)rdr["DispComments"];

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();
                    //       MessageBox.Show(ex.ToString());
                    //     MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }
 */

        public void InsertDisputeTran(int InDisputeNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[DisputesTransTable]"
                + " ([DisputeNumber], [DispDtTm],[BankId],[DbTranNo],"
                + " [TranDate], [AtmNo], [ReplGroup], [CardType], [CardNo], [AccNo],"
                + " [CurrencyNm], [TranAmount], [SystemTarget], [TransType], [TransDesc],"
                + " [ErrNo], [ReplCycle], [StartTrxn], [EndTrxn],"
                + " [DisputedAmt], [DisputeActionId], [ActionDtTm], [DecidedAmount], [ReasonForAction],"
                + " [ActionComment], [ClosedDispute] , [Operator])"
                + " VALUES (@DisputeNumber,@DispDtTm, @BankId,"
                + " @DbTranNo, @TranDate, @AtmNo,@ReplGroup, @CardType, @CardNo, @AccNo,"
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
                        cmd.Parameters.AddWithValue("@DisputeNumber", DisputeNumber);
                        cmd.Parameters.AddWithValue("@DispDtTm", DispDtTm);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@DbTranNo", DbTranNo);
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
                    ErrorOutput = "An error occured in Dispute Trans Class............. " + ex.Message;
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
                                      + "[DisputeNumber] = @DisputeNumber,[DispDtTm] = @DispDtTm, [BankId] = @BankId,"
                                      + " [DbTranNo] = @DbTranNo, [TranDate] = @TranDate,"
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

                        cmd.Parameters.AddWithValue("@DisputeNumber", DisputeNumber);
                        cmd.Parameters.AddWithValue("@DispDtTm", DispDtTm);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@DbTranNo", DbTranNo);
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
                    ErrorOutput = "An error occured in Dispute Trans Class............. " + ex.Message;
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
          + " FROM [dbo].[DisputesTransTable] "
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Dispute Trans Class............. " + ex.Message;
                }
        }
        // CLEAR ALL DISPUTE Transactions for a Specific Dispute Number  
        public void DeleteTransOfthisDispute(int InDisputeNumber)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "DELETE "
          + " FROM [dbo].[DisputesTransTable] "
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Dispute Trans Class............. " + ex.Message;
                }
        }
      /*  DELETE FROM [dbo].[DisputesTransTable]
      WHERE DisputeNumber = 6 */
    }
}

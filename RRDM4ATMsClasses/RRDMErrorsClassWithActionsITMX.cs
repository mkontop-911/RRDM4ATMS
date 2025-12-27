using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMErrorsClassWithActionsITMX : Logger
    {
        public RRDMErrorsClassWithActionsITMX() : base() { }
        // Declarations 

        // Variables for reading errors 
        public int ErrNo;
        public int ErrId;
        public int ErrType;
        // Values
        // 1 : Withdrawl EJournal Errors
        // 2 : Mainframe Withdrawl Errors
        // 3 : Deposit Errors Journal 
        // 4 : Deposit Mainframe Errors
        // 5 : Created by user Errors = eg moving to suspense 
        // 6 : Empty 
        // 7 : Created System Errors 
        // 
        public string ErrDesc;
        public string CategoryId;

        public int RMCycle;
        public int MaskRecordId;

        public string AtmNo;
        public int SesNo;
        public int TraceNo;
        public int TransNo;
        public int TransType;
        public string TransDescr;
        public DateTime DateTime;
        public bool NeedAction; public bool OpenErr;

        public string CurDes; public decimal ErrAmount; public int ActionId;
        public bool DrCust; public bool CrCust; public bool DrAtmCash; public bool CrAtmCash; public bool DrAtmSusp; public bool CrAtmSusp;
        public bool UnderAction;
        public bool DisputeAct;
        public bool ManualAct;
        public bool MainOnly;
        public bool FullCard;
        public bool ForeignCard;

        public DateTime DateInserted;
        public string BankId;

        public string BranchId;

        public bool TurboReconc;

        public string CardNo;
        public string ByWhom;
        public DateTime ActionDtTm;
        public int ActionSes;
        public string CustAccNo;
        public string AccountNo1;
        public string AccountNo2;
        public string AccountNo3;

        public bool DrAccount3;
        public bool CrAccount3;
        public string UserComment;
        public bool Printed;
        public DateTime DatePrinted;
        public string CircularDesc;
        public string CitId;
        public string Operator;

        public int NumOfErrors;
        public int NumOfErrorsLess200;

        public int NumOfOpenErrorsLess100;
        public decimal TotalErrorsAmtLess100;

        public int ErrUnderAction;
        public int ErrDisputeAction;
        public int ErrUnderManualAction;

        public decimal TotalErrorsAmt;
        public decimal TotalUnderActionAmt;

        //int InTraceNumber;

        //bool boolEWB1xx;
        //bool boolEWB3xx;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public DataTable ErrorsTable = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public int TotalSelected;

        //string ErrorsFileId = "[ATMS].[dbo].[ErrorsTable]";

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();
        RRDMAccountsClass Acc = new RRDMAccountsClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        string SqlString;


        // READ Error ID Record 
        public void ReadErrorsIDRecord(int InErrId, string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
            + " FROM [dbo].[ErrorsIdCharacteristics] "
            + " WHERE Errid = @ErrId AND Operator=@Operator";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrId", InErrId);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details

                            ErrId = (int)rdr["ErrId"];
                            ErrDesc = rdr["ErrDesc"].ToString();
                            ErrType = (int)rdr["ErrType"];

                            CategoryId = (string)rdr["CategoryId"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            DateInserted = (DateTime)rdr["DateInserted"];

                            BankId = rdr["BankId"].ToString();

                            BranchId = rdr["BranchId"].ToString();
                            TurboReconc = (bool)rdr["TurboReconc"];

                            TraceNo = (int)rdr["TraceNo"];
                            CardNo = rdr["CardNo"].ToString();
                            TransNo = (int)rdr["TransNo"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();

                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];
                            OpenErr = (bool)rdr["OpenErr"];
                            FullCard = (bool)rdr["FullCard"];
                            UnderAction = (bool)rdr["UnderAction"];
                            DisputeAct = (bool)rdr["DisputeAct"];
                            ManualAct = (bool)rdr["ManualAct"];
                            ByWhom = (string)rdr["ByWhom"];

                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            ActionSes = (int)rdr["ActionSes"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            ActionId = (int)rdr["ActionId"];

                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            CustAccNo = rdr["CustAccNo"].ToString();

                            DrAtmCash = (bool)rdr["DrAtmCash"];
                            CrAtmCash = (bool)rdr["CrAtmCash"];
                            AccountNo1 = rdr["AccountNo1"].ToString();

                            DrAtmSusp = (bool)rdr["DrAtmSusp"];
                            CrAtmSusp = (bool)rdr["CrAtmSusp"];
                            AccountNo2 = rdr["AccountNo2"].ToString();

                            DrAccount3 = (bool)rdr["DrAccount3"];
                            CrAccount3 = (bool)rdr["CrAccount3"];
                            AccountNo3 = rdr["AccountNo3"].ToString();

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"];

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString();

                            //        CitId = (int)rdr["CitId"];
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


      
        // READ Error specific 
        public void ReadErrorsTableSpecific(int InErrNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
             + " FROM [dbo].[ErrorsTable] "
             + " WHERE ErrNo = @ErrNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details

                            ErrNo = (int)rdr["ErrNo"];
                            ErrId = (int)rdr["ErrId"];
                            ErrDesc = rdr["ErrDesc"].ToString();
                            ErrType = (int)rdr["ErrType"];

                            CategoryId = (string)rdr["CategoryId"];

                            RMCycle = (int)rdr["RMCycle"];
                            MaskRecordId = (int)rdr["MaskRecordId"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            DateInserted = (DateTime)rdr["DateInserted"];

                            BankId = rdr["BankId"].ToString();

                            BranchId = rdr["BranchId"].ToString();
                            TurboReconc = (bool)rdr["TurboReconc"];

                            TraceNo = (int)rdr["TraceNo"];
                            CardNo = rdr["CardNo"].ToString();
                            TransNo = (int)rdr["TransNo"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();

                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];
                            OpenErr = (bool)rdr["OpenErr"];
                            FullCard = (bool)rdr["FullCard"];

                            UnderAction = (bool)rdr["UnderAction"];
                            DisputeAct = (bool)rdr["DisputeAct"];
                            ManualAct = (bool)rdr["ManualAct"];

                            ByWhom = (string)rdr["ByWhom"];
                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            ActionSes = (int)rdr["ActionSes"];


                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            ActionId = (int)rdr["ActionId"];

                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            CustAccNo = rdr["CustAccNo"].ToString();

                            DrAtmCash = (bool)rdr["DrAtmCash"];
                            CrAtmCash = (bool)rdr["CrAtmCash"];
                            AccountNo1 = rdr["AccountNo1"].ToString();

                            DrAtmSusp = (bool)rdr["DrAtmSusp"];
                            CrAtmSusp = (bool)rdr["CrAtmSusp"];
                            AccountNo2 = rdr["AccountNo2"].ToString();

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"];

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString();

                            CitId = (string)rdr["CitId"];

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

        // UPDATE ERROR TABLE   
        public void UpdateErrorsTableSpecific(int InErrNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ErrorsTable SET OpenErr=@OpenErr,"
                            + "CardNo=@CardNo, CustAccNo=@CustAccNo,"
                            + "FullCard=@FullCard, UnderAction=@UnderAction, DisputeAct=@DisputeAct,"
                            + " ManualAct = @ManualAct, UserComment = @UserComment,"
                            + " ByWhom = @ByWhom, ActionDtTm = @ActionDtTm, ActionSes = @ActionSes,"
                                       + " Printed = @Printed, DatePrinted = @DatePrinted,CitId = @CitId"
                            + " WHERE ErrNo=@ErrNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);

                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CustAccNo", CustAccNo);

                        cmd.Parameters.AddWithValue("@FullCard", FullCard);
                        cmd.Parameters.AddWithValue("@UnderAction", UnderAction);
                        cmd.Parameters.AddWithValue("@DisputeAct", DisputeAct);

                        cmd.Parameters.AddWithValue("@ManualAct", ManualAct);
                        cmd.Parameters.AddWithValue("@UserComment", UserComment);

                        cmd.Parameters.AddWithValue("@ByWhom", ByWhom);
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);
                        cmd.Parameters.AddWithValue("@ActionSes", ActionSes);

                        cmd.Parameters.AddWithValue("@Printed", Printed);
                        cmd.Parameters.AddWithValue("@DatePrinted", DatePrinted);
                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@OpenErr", OpenErr);

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
        // This is called from Disputes 
        //
        public void CreateTransTobepostedfromDisputes(string InOperator, string InUserId, string InAuthUser, int InDispTranNo, int InTranType, decimal InAmount)
        {
            //
            // Create a Credit or a Debit to customer AS A RESULT OF a dispute 
            //
            //
            int PostedNo;

            // Insert Transactions (TWO Trans) In TransTo BePosted 

            RRDMDisputesTableClassITMX Di = new RRDMDisputesTableClassITMX();
            RRDMDisputeTransactionsClassITMX Dt = new RRDMDisputeTransactionsClassITMX();

            RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();

            ErrorFound = false;
            ErrorOutput = "";

            bool OurAtmTran;

            Dt.ReadDisputeTran(InDispTranNo);

            // Find Details of Masked REcord 

            Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(InOperator, Dt.MaskRecordId);

            // check if the transaction comes from our ATMs 

            Mpa.ReadInPoolTransSpecificUniqueRecordId(Dt.MaskRecordId, 2);
            if (Mpa.RecordFound == true)
            {
                OurAtmTran = true;
            }
            else
            {
                OurAtmTran = false;
            }

            //
            // If InTranType = 21 means CRCust if InTranType = 11 then means DRCust
            //

            if (InTranType == 21)
            {
                // CREATE A TRANSACTION TO CR CUSTOMER 
                // CREATE A TRANSACTION TO DR CASH
                if (OurAtmTran == true)
                {
                    // Fill in data 
                }


                if (OurAtmTran == false) // Not our ATM therefore information is needed for different fields 
                {
                    Tp.OriginId = "07"; // *
                    Tp.OriginName = "Disputes";  // From Disputes 
                    Tp.RMCateg = Mp.ReconcCategoryId;
                    Tp.RMCategCycle = Mp.ReconcCycleNo;

                    Tp.UniqueRecordId = Mp.MaskRecordId;

                    Tp.ErrNo = 0;
                    Tp.AtmNo = "";
                    Tp.SesNo = 0;
                    Tp.BankId = Mp.CreditBank;

                    Tp.AtmTraceNo = 0;

                    Tp.BranchId = "HO";
                    Tp.AtmDtTime = Mp.ExecutionTxnDtTm;
                    //Tp.HostDtTime = DateTimeH;
                    Tp.SystemTarget = 5;

                    Tp.CardNo = Mp.MobileBeneficiary;
                    Tp.AccNo = Mp.AccountBeneficiary;

                    Tp.CardOrigin = 5; // Find OUT

                    // First Entry 
                    Tp.TransType = 21; //

                    Tp.TransDesc = "Result of Dispute:" + Dt.DisputeNumber.ToString();
                    Tp.GlEntry = false;

                    // Second Entry 
                    Tp.TransType2 = 11; //

                    Tp.TransDesc2 = "Result of Dispute:" + Dt.DisputeNumber.ToString();

                    Tp.AccNo2 = Mp.CreditBank + " to ITMX";  // MUST BE LESS THAN 16 

                    Tp.GlEntry2 = true;

                    Tp.CurrDesc = Mp.Ccy;

                    Tp.GlEntry2 = true;

                    Tp.CurrDesc = Mp.Ccy;
                    Tp.TranAmount = InAmount;

                    Tp.OpenDate = DateTime.Now;

                    Tp.TransMsg = Dt.ActionComment;

                    Tp.DisputeNo = Dt.DisputeNumber;
                    Tp.DispTranNo = Dt.DispTranNo;

                    Tp.MakerUser = InUserId;
                    Tp.AuthUser = InAuthUser;

                    Tp.OpenRecord = true;

                    Tp.AuthCode = 0;
                    Tp.RefNumb = 0;
                    Tp.RemNo = 0;

                    Tp.AtmMsg = "";
                    Tp.OpenDate = DateTime.Now;
                    Tp.OpenRecord = true;

                    Tp.Operator = Dt.Operator;
                    //NOSTRO
                    Tp.NostroCcy = "";
                    Tp.NostroAdjAmt = 0;

                    PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);
                }


            }
            if (InTranType == 11)
            {
                // CREATE A TRANSACTION TO DR CUSTOMER 
                // CREATE A TRANSACTION TO CR CASH

                Tp.OriginId = "07"; // *
                Tp.OriginName = "Disputes";  // From Dispute 
                Tp.RMCateg = Mp.ReconcCategoryId;
                Tp.RMCategCycle = Mp.ReconcCycleNo;
                Tp.UniqueRecordId = Mp.MaskRecordId;

                Tp.ErrNo = 0;
                Tp.AtmNo = "";
                Tp.SesNo = 0;
                Tp.BankId = Mp.DebitBank;

                Tp.AtmTraceNo = 0;

                Tp.BranchId = "HO";
                Tp.AtmDtTime = Mp.ExecutionTxnDtTm;
                //Tp.HostDtTime = DateTimeH;
                Tp.SystemTarget = 5;


                Tp.CardNo = Mp.MobileRequestor;
                Tp.AccNo = Mp.AccountRequestor;

                Tp.CardOrigin = 5; // Find OUT

                // First Entry 
                Tp.TransType = 11; //

                Tp.TransDesc = "Result of Dispute:" + Dt.DisputeNumber.ToString();
                Tp.GlEntry = false;

                // Second Entry 
                Tp.TransType2 = 21; //

                Tp.TransDesc2 = "Result of Dispute:" + Dt.DisputeNumber.ToString();

                Tp.AccNo2 = Mp.DebitBank + " to ITMX"; // MUST BE LESS THAN 16 

                Tp.GlEntry2 = true;

                Tp.CurrDesc = Mp.Ccy;

                Tp.GlEntry2 = true;

                Tp.CurrDesc = Mp.Ccy;
                Tp.TranAmount = InAmount;

                Tp.OpenDate = DateTime.Now;

                Tp.TransMsg = Dt.ActionComment;

                Tp.DisputeNo = Dt.DisputeNumber;
                Tp.DispTranNo = Dt.DispTranNo;

                Tp.MakerUser = InUserId;
                Tp.AuthUser = InAuthUser;

                Tp.OpenRecord = true;

                Tp.AuthCode = 0;
                Tp.RefNumb = 0;
                Tp.RemNo = 0;

                Tp.AtmMsg = "";
                Tp.OpenDate = DateTime.Now;
                Tp.OpenRecord = true;

                Tp.Operator = Dt.Operator;
                //NOSTRO
                Tp.NostroCcy = "";
                Tp.NostroAdjAmt = 0;

                PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);
            }

        }

        // 
        // This is called from Confirm matching - Nostro
        //
        public void CreateTransTobepostedfromNOSTRO(string InUserId, string InAuthUser, string InCategory, string InternalAcc, int InSeqNo)
        {
            // Take adjustments made in Internal and ... 
            // Create a transaction to be posted to the Bank's GL  
            //
            //
            ErrorFound = false;
            ErrorOutput = "";

            RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();
            RRDMUsersRecords Us = new RRDMUsersRecords();
            RRDMBanks Ba = new RRDMBanks();

            try
            {
                Us.ReadUsersRecord(InUserId);
                Ba.ReadBank(Us.BankId);

                string SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + InSeqNo;

                Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);
                //
                // If 
                //
                if (Se.StmtLineAmt < 0)
                {
                    Tp.TransType = 11;  // First Entry
                    Tp.TransType2 = 21; // Second Entry
                }
                if (Se.StmtLineAmt > 0)
                {
                    Tp.TransType = 21;  // First Entry
                    Tp.TransType2 = 11; // Second Entry
                }

                // MAKE First Entry .. FROM

                Tp.OriginId = "09"; // *
                Tp.OriginName = "Nostro Adjustments";    // From Dispute 
                Tp.RMCateg = InCategory;
                Tp.RMCategCycle = Se.MatchedRunningCycle;
                Tp.UniqueRecordId = Se.UniqueMatchingNo; // Reference 

                Tp.AtmNo = "N/A";
                Tp.SesNo = 0;

                Tp.BankId = Us.BankId;
                Tp.AtmTraceNo = 0;
                Tp.BranchId = "N/A";
                Tp.AtmDtTime = Se.AuthoriserDtTm;
                //Tp.HostDtTime = Rm.TransDate;
                Tp.SystemTarget = 10; // GL 

                Tp.CardNo = "";
                Tp.AccNo = Se.StmtAccountID; // From Account 

                // MAKE Second Entry

                Tp.TransDesc = "Adjustment Entry with record :" + Se.UniqueMatchingNo.ToString();
                Tp.GlEntry = true;

                Tp.AtmTraceNoH = 0;

                // Second Entry ... TO 

                Tp.TransDesc2 = "Transfer from Nostro :" + Se.StmtAccountID;

                Tp.AccNo2 = Se.AdjGlAccount;  // TO ACCOUNT 
                Tp.GlEntry2 = true;

                Tp.CurrDesc = Ba.BasicCurName;

                Tp.TranAmount = Se.StmtLineAmt * Se.CcyRate; // Local Amount 
                                                             //Tp.TranAmount = Se.StmtLineAmt;
                Tp.OpenDate = DateTime.Now;

                Tp.TransMsg = "Created Adjustment Txn for " + Se.StmtAccountID 
                             + " and for amount in " + Se.Ccy + " Of " + Se.StmtLineAmt.ToString()
                             + " at a rate of " + Se.CcyRate.ToString()
                              ;
                Tp.AtmMsg = "";

                Tp.DisputeNo = 0;
                Tp.DispTranNo = 0;
                Tp.ErrNo = 0;
                Tp.Operator = Se.Operator;
                //TEST
                Tp.MakerUser = InUserId;
                Tp.AuthUser = InAuthUser;

                Tp.OpenRecord = true;
                //NOSTRO
                Tp.NostroCcy = Se.Ccy;
                Tp.NostroAdjAmt = Se.StmtLineAmt;

                int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);
            }
            catch (Exception ex)
            {

                CatchDetails(ex);
            }

        }

        // 
        // This is called from Confirm matching - Visa Settlement
        //
        public void CreateTransTobepostedfromVisa(string InUserId, string InAuthUser, string InCategory, string InternalAcc, int InSeqNo)
        {
            // Take adjustments made in Internal and ... 
            // Create a transaction to be posted to the Bank's GL  
            //
            //
            ErrorFound = false;
            ErrorOutput = "";

            RRDMNVCardsBothAuthorAndSettlement Sec = new RRDMNVCardsBothAuthorAndSettlement(); 
         
            RRDMUsersRecords Us = new RRDMUsersRecords();
            RRDMBanks Ba = new RRDMBanks();

            try
            {
                Us.ReadUsersRecord(InUserId);
                Ba.ReadBank(Us.BankId);

                string SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + InSeqNo;

                Sec.ReadNVFromBothVisaAutherAndSettlementBySelectionCriteria(SelectionCriteria);
                //
                // If 
                //
                if (Sec.TxnAmt < 0)
                {
                    Tp.TransType = 11;  // First Entry
                    Tp.TransType2 = 21; // Second Entry
                }
                if (Sec.TxnAmt > 0)
                {
                    Tp.TransType = 21;  // First Entry
                    Tp.TransType2 = 11; // Second Entry
                }

                // MAKE First Entry .. FROM

                Tp.OriginId = "09"; // *
                Tp.OriginName = "Visa Adjustments";    // 
                Tp.RMCateg = InCategory;
                Tp.RMCategCycle = Sec.MatchedRunningCycle;
                Tp.UniqueRecordId = Sec.UniqueMatchingNo; // Reference 

                Tp.AtmNo = "N/A";
                Tp.SesNo = 0;

                Tp.BankId = Us.BankId;
                Tp.AtmTraceNo = 0;
                Tp.BranchId = "N/A";
                Tp.AtmDtTime = Sec.AuthoriserDtTm;
                //Tp.HostDtTime = Rm.TransDate;
                Tp.SystemTarget = 3; // GL 

                Tp.CardNo = Sec.CardNo;
                Tp.AccNo = Sec.AccNo; // From Account 

                // MAKE Second Entry

                Tp.TransDesc = "Adjustment Entry with record :" + Sec.UniqueMatchingNo.ToString();
                Tp.GlEntry = false;

                Tp.AtmTraceNoH = 0;

                // Second Entry ... TO 

                Tp.TransDesc2 = "Transfer from Visa Settlement :" + Sec.StmtAccountID;

                Tp.AccNo2 = "UnDefined Acc";  // TO ACCOUNT 
                Tp.GlEntry2 = true;

                Tp.CurrDesc = Ba.BasicCurName;

                Tp.TranAmount = Sec.TxnAmt * Sec.TxnCcyRate; // Local Amount 
                                                             //Tp.TranAmount = Se.StmtLineAmt;
                Tp.OpenDate = DateTime.Now;

                Tp.TransMsg = "Created Adjustment Txn for " + Sec.AccNo
                             + " and for amount in " + Sec.TxnCcy+ " Of " + Sec.TxnAmt.ToString()
                             + " at a rate of " + Sec.TxnCcyRate.ToString()
                              ;
                Tp.AtmMsg = "";

                Tp.DisputeNo = 0;
                Tp.DispTranNo = 0;
                Tp.ErrNo = 0;
                Tp.Operator = Sec.Operator;
                //TEST
                Tp.MakerUser = InUserId;
                Tp.AuthUser = InAuthUser;

                Tp.OpenRecord = true;
                //NOSTRO
                Tp.NostroCcy = Sec.TxnCcy;
                Tp.NostroAdjAmt = Sec.TxnAmt;

                int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);
            }
            catch (Exception ex)
            {

                CatchDetails(ex);
            }

        }




        // UPDATE All ERROR TABLE FOR A SPECIFIC ATM - DURING TURBO PROCESS 

        public void UpdateErrorsTableActionTaken(int InErrNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ErrorsTable SET UnderAction=@UnderAction, UserComment = @UserComment, ActionDtTm = @ActionDtTm, "
                                       + " Printed = @Printed" +
                            " WHERE ErrNo = @ErrNo AND OpenErr=1 AND NeedAction=1", conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);
                        cmd.Parameters.AddWithValue("@UnderAction", 1);
                        UserComment = " Turbo Action ";
                        cmd.Parameters.AddWithValue("@UserComment", UserComment);
                        cmd.Parameters.AddWithValue("@ActionDtTm", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Printed", 0);

                        cmd.ExecuteNonQuery();
                       

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

       

    }
}



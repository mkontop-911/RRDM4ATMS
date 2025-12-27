using System;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace RRDM4ATMs
{
    public class RRDMMatchingReconcExceptionsInfoAnyTables : Logger
    {
        public RRDMMatchingReconcExceptionsInfoAnyTables() : base() { }
        //
        // CLASS TO SET EXCEPTIONS FOR MAtching of Any Files 
        //
        // ITMX has a separate class
        // ATMs have a different process based on master pool Mpa   
        //

        public int SeqNo;
        public int UniqueRecordId;

        public string MatchingCateg;
        public int MatchingAtRMCycle;
        public bool Matched;
        public string MatchMask;

        public DateTime SystemMatchingDtTm;
        public string MatchedType;
        public string UnMatchedType;
        public int MetaExceptionId;

        public int MetaExceptionNo;
        public bool ActionByUser;
        public string UserId;
        public string Authoriser;

        public DateTime AuthoriserDtTm;
        public string ActionType; // 0 ... No Action Taken 
                                  // 1 ... Meta Exception Suggested By System 
                                  // 2 ... Meta Exception Manual 
                                  // 3 ... 
                                  // 4 ... Force Match - Broken Disc Case type 
                                  // 5 ... Move To Dispute 
                                  // 7 ... Default By System
                                  // 8 ... UnDo Default
        public bool SettledRecord;
        public string Operator;

        public string Table01;
        public string Table02;
        public string Table03;
        public string Table04;

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

        //RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX(); 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        private void ReadTableFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            UniqueRecordId = (int)rdr["UniqueRecordId"];
            MatchingCateg = (string)rdr["MatchingCateg"];
            MatchingAtRMCycle = (int)rdr["MatchingAtRMCycle"];
            Matched = (bool)rdr["Matched"];

            MatchMask = (string)rdr["MatchMask"];

            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];
            MatchedType = (string)rdr["MatchedType"];
            UnMatchedType = (string)rdr["UnMatchedType"];
            MetaExceptionId = (int)rdr["MetaExceptionId"];

            MetaExceptionNo = (int)rdr["MetaExceptionNo"];
            ActionByUser = (bool)rdr["ActionByUser"];
            UserId = (string)rdr["UserId"];
            Authoriser = (string)rdr["Authoriser"];

            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];
            ActionType = (string)rdr["ActionType"];

            SettledRecord = (bool)rdr["SettledRecord"];

            Operator = (string)rdr["Operator"];

            Table01 = (string)rdr["Table01"];
            Table02 = (string)rdr["Table02"];
            Table03 = (string)rdr["Table03"];
            Table04 = (string)rdr["Table04"];

        }

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
            DataTableMatchingExceptions.Columns.Add("MatchingCateg", typeof(string));
            DataTableMatchingExceptions.Columns.Add("MatchingAtRMCycle", typeof(int));

            DataTableMatchingExceptions.Columns.Add("Matched", typeof(string));
            DataTableMatchingExceptions.Columns.Add("MatchMask", typeof(string));

            DataTableMatchingExceptions.Columns.Add("Amount", typeof(string));

            if (InMode == 1)
            {
                SqlString = "SELECT *"
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingReconcExceptionsInfoAnyTables] "
                   + " WHERE Operator = @Operator AND MatchingCateg = @MatchingCateg AND  Authoriser = '' "
                   + " ORDER by SeqNo ASC ";
            }

            if (InMode == 2)
            {
                SqlString = "SELECT *"
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingReconcExceptionsInfoAnyTables] "
                   + " WHERE Operator = @Operator AND MatchingCateg = @MatchingCateg AND  MatchingAtRMCycle = @MatchingAtRMCycle "
                   + " ORDER by SeqNo ASC ";
            }

            if (InMode == 3) // Show all Exceptions for this ReconcCycle Number 
            {
                SqlString = "SELECT *"
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingReconcExceptionsInfoAnyTables] "
                   + " WHERE Operator = @Operator AND  MatchingAtRMCycle = @MatchingAtRMCycle "
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
                        cmd.Parameters.AddWithValue("@MatchingCateg", InReconcCategoryId);

                        if (InMode == 2)
                        {
                            cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InReconcCycleNo);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTableFields(rdr);

                            TotalSelected = TotalSelected + 1;

                            string TableName = "Define it";

                            //Read Record Details
                            RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables();
                            WSelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;
                            Mgt.ReadTransSpecificFromSpecificWorking(WSelectionCriteria, TableName);
                            //Fill IN Table 
                            DataRow RowSelected = DataTableMatchingExceptions.NewRow();

                            RowSelected["RecordId"] = UniqueRecordId;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["MatchingAtRMCycle"] = MatchingAtRMCycle;
                            RowSelected["Matched"] = Matched;
                            RowSelected["MatchMask"] = MatchMask;
                            RowSelected["Amount"] = Mgt.TransAmt.ToString("#,##0.00");

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
        // READ TABLE FIELDS 


        //
        // Methods 
        // READ MatchingReconcExceptionsInfo to find totals
        // 
        //
        public int TotalExceptionsNo;
        public decimal TotalExceptionsAmt;
        public int TotalMetaExceptionsNo;
        public decimal TotalMetaExceptionsAmt;
        public int TotalForcedMatchedNo;
        public decimal TotalForcedMatchedAmt;
        public int TotalMoveToDisputesNo;
        public decimal TotalMoveToDisputesAmt;

        public int TotalFastTrackNo;
        public decimal TotalFastTrackAmt;


        public void ReadMatchingReconcExceptionsInfoForTotals(string InOperator
            , string InReconcCategoryId, int InReconcCycleNo, string InTableId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingTxns_InGeneralTables Mtg = new RRDMMatchingTxns_InGeneralTables();
            string SelelectionCriteria = "";

            TotalSelected = 0;
            TotalExceptionsOutstandingToBeAuthorised = 0;
            TotalExceptionsDoneAndAuthorised = 0;
            TotalExceptionsHandleByUser = 0;

            TotalExceptionsNo = 0;
            TotalExceptionsAmt = 0;
            TotalMetaExceptionsNo = 0;
            TotalMetaExceptionsAmt = 0;
            TotalForcedMatchedNo = 0;
            TotalForcedMatchedAmt = 0;
            TotalMoveToDisputesNo = 0;
            TotalMoveToDisputesAmt = 0;

            TotalFastTrackNo = 0;
            TotalFastTrackAmt = 0;


        // DATA TABLE ROWS DEFINITION       

        SqlString = "SELECT * "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingReconcExceptionsInfoAnyTables] "
                        + " WHERE Operator = @Operator AND MatchingCateg = @MatchingCateg AND  MatchingAtRMCycle = @MatchingAtRMCycle ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InReconcCycleNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadTableFields(rdr);

                            SelelectionCriteria = " WHERE UniqueRecordId =" + UniqueRecordId; 
                            Mtg.ReadTransSpecificFromSpecificWorking(SelelectionCriteria, InTableId); 

                            TotalExceptionsNo = TotalExceptionsNo + 1;
                            TotalExceptionsAmt = TotalExceptionsAmt + Mtg.TransAmt;

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
                            //public string ActionType; // 0 ... No Action Taken 
                            // 1 ... Meta Exception Suggested By System 
                            // 2 ... Meta Exception Manual 
                            // 3 ... 
                            // 4 ... Force Match - Broken Disc Case type 
                            // 5 ... Move To Dispute 
                            // 7 ... Default By System
                            // 8 ... UnDo Default                        
                           
                            if (ActionType == "1" || ActionType == "2")
                            {
                                TotalMetaExceptionsNo = TotalMetaExceptionsNo + 1;
                                TotalMetaExceptionsAmt = TotalMetaExceptionsAmt + Mtg.TransAmt;
                            }
                            if (ActionType == "4")
                            {
                                TotalForcedMatchedNo = TotalForcedMatchedNo + 1;
                                TotalForcedMatchedAmt = TotalForcedMatchedAmt + Mtg.TransAmt;
                            }
                            if (ActionType == "5")
                            {
                                TotalMoveToDisputesNo = TotalMoveToDisputesNo + 1;
                                TotalMoveToDisputesAmt = TotalMoveToDisputesAmt + Mtg.TransAmt;
                            }
                            if (MatchedType == "FastTrack")
                            {
                                TotalFastTrackNo = TotalFastTrackNo + 1;
                                TotalFastTrackAmt = TotalFastTrackAmt + +Mtg.TransAmt;
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


        // Insert Matching Exception Info
        public int InsertMatchingReconcExceptionsInfo()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingReconcExceptionsInfoAnyTables]"
                + " ([UniqueRecordId],[MatchingCateg],[MatchingAtRMCycle],[Matched],"
                + "[MatchMask],[SystemMatchingDtTm],[MatchedType],[UnMatchedType],"
                + "[MetaExceptionId],[MetaExceptionNo],"
                + "[ActionByUser],"
                + "[UserId],"
                + "[Authoriser],"
                + "[AuthoriserDtTm],"
                + "[ActionType],"
                + "[SettledRecord],"
                + "[Table01],"
                + "[Table02],"
                + "[Table03],"
                + "[Table04],"
                + "[Operator] )"
                + " VALUES"
                + " (@UniqueRecordId,@MatchingCateg,@MatchingAtRMCycle,@Matched, "
                + " @MatchMask,@SystemMatchingDtTm,@MatchedType,@UnMatchedType, "
                + " @MetaExceptionId,@MetaExceptionNo,"
                + " @ActionByUser,"
                + " @UserId,"
                + " @Authoriser,"
                + " @AuthoriserDtTm,"
                + " @ActionType,"
                + " @SettledRecord,"
                + " @Table01,"
                + " @Table02,"
                + " @Table03,"
                + " @Table04,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                        cmd.Parameters.AddWithValue("@MatchingAtRMCycle", MatchingAtRMCycle);
                        cmd.Parameters.AddWithValue("@Matched", Matched);

                        cmd.Parameters.AddWithValue("@MatchMask", MatchMask);

                        cmd.Parameters.AddWithValue("@SystemMatchingDtTm", SystemMatchingDtTm);
                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);
                        cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);
                        cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);

                        cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);
                        cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);

                        cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);
                        cmd.Parameters.AddWithValue("@ActionType", ActionType);
                        cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        cmd.Parameters.AddWithValue("@Table01", Table01);
                        cmd.Parameters.AddWithValue("@Table02", Table02);
                        cmd.Parameters.AddWithValue("@Table03", Table03);
                        cmd.Parameters.AddWithValue("@Table04", Table04);
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
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingReconcExceptionsInfoAnyTables] SET "
                            + " MetaExceptionNo = @MetaExceptionNo, ActionType = @ActionType, "
                            + " ActionByUser = @ActionByUser, UserId = @UserId, "
                            + " Authoriser = @Authoriser, AuthoriserDtTm = @AuthoriserDtTm  "
                            + " WHERE UniqueRecordId = @UniqueRecordId ", conn))
                    {

                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);

                        cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

                        cmd.Parameters.AddWithValue("@ActionType", ActionType);

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
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingReconcExceptionsInfoAnyTables] "
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

        //int WTxnCode;
        //int WDisputeActionId;

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
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingReconcExceptionsInfoAnyTables] "
                   + " WHERE Operator = @Operator AND MatchingCateg = @MatchingCateg AND  Authoriser = '' ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InReconcCategoryId);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableMatchingExceptions);

                        // Close conn
                        conn.Close();


                        int I = 0;

                        while (I <= (DataTableMatchingExceptions.Rows.Count - 1))
                        {

                            RecordFound = true;

                            SeqNo = (int)DataTableMatchingExceptions.Rows[I]["SeqNo"];



                            //MaskRecordId = (int)DataTableMatchingExceptions.Rows[I]["MaskRecordId"];
                            //ITMXUniqueTxnRef = (string)DataTableMatchingExceptions.Rows[I]["ITMXUniqueTxnRef"];
                            //ReconcCategoryId = (string)DataTableMatchingExceptions.Rows[I]["ReconcCategoryId"];
                            //ReconcCycleNo = (int)DataTableMatchingExceptions.Rows[I]["ReconcCycleNo"];

                            //UnMatchedName = (string)DataTableMatchingExceptions.Rows[I]["UnMatchedName"];
                            //ExceptionRecomm = (string)DataTableMatchingExceptions.Rows[I]["ExceptionRecomm"];
                            //MetaExceptionId = (int)DataTableMatchingExceptions.Rows[I]["MetaExceptionId"];
                            //MetaExceptionNo = (int)DataTableMatchingExceptions.Rows[I]["MetaExceptionNo"];

                            //ActionTypeId = (int)DataTableMatchingExceptions.Rows[I]["ActionTypeId"];
                            //ActionTypeDescr = (string)DataTableMatchingExceptions.Rows[I]["ActionTypeDescr"];

                            //ActionByUser = (bool)DataTableMatchingExceptions.Rows[I]["ActionByUser"];
                            //UserId = (string)DataTableMatchingExceptions.Rows[I]["UserId"];

                            //Authoriser = (string)DataTableMatchingExceptions.Rows[I]["Authoriser"];
                            //AuthoriserDtTm = (DateTime)DataTableMatchingExceptions.Rows[I]["AuthoriserDtTm"];

                            //CreatedDtTm = (DateTime)DataTableMatchingExceptions.Rows[I]["CreatedDtTm"];
                            //Operator = (string)DataTableMatchingExceptions.Rows[I]["Operator"];

                            //Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(Operator, MaskRecordId);

                            //Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(ReconcCategoryId, ReconcCycleNo, WOriginName);

                            ////   if ActionTypeId = 1 => "Creation of posted transaction "
                            ////   if ActionTypeId = 2 => "Creation Of Dispute"
                            ////   if ActionTypeId = 4 => "Creation Of Disputes For Both Banks"
                            ////   if ActionTypeId = 5 => "Creation of SMS Transaction"
                            ////
                            //if (ActionTypeId == 1) // Normal Transaction 
                            //{

                            //    Tp.OriginId = "02"; // "02" Matching Process   
                            //    Tp.OriginName = WOriginName;  // ORIGIN RECONCILIATION
                            //    Tp.RMCateg = ReconcCategoryId;
                            //    Tp.RMCategCycle = ReconcCycleNo;
                            //    Tp.UniqueRecordId = MaskRecordId;

                            //    Tp.ErrNo = 0;
                            //    Tp.AtmNo = "";
                            //    Tp.SesNo = 0;
                            //    Tp.BankId = Operator;

                            //    Tp.AtmTraceNo = 0;

                            //    Tp.BranchId = "ITMX HO";
                            //    Tp.AtmDtTime = DateTime.Now;
                            //    //Tp.HostDtTime = DateTimeH;
                            //    Tp.SystemTarget = 11;

                            //    Tp.CardNo = Mp.MobileRequestor;
                            //    Tp.CardOrigin = 5; // Find OUT ... 

                            //    // First Entry 
                            //    Tp.TransType = 11; // 
                            //    Tp.TransDesc = "Correction of ITMX To BankA:" + Mp.DebitBank;

                            //    Tp.AccNo = "ITMX to :" + Mp.DebitBank;
                            //    Tp.GlEntry = true;


                            //    // Second Entry


                            //    Tp.TransType2 = 22;

                            //    Tp.TransDesc2 = "Correction of ITMX To BankA:" + Mp.CreditBank;

                            //    Tp.AccNo2 = "ITMX to :" + Mp.CreditBank;

                            //    Tp.GlEntry2 = true;
                            //    // End Second Entry 

                            //    Tp.CurrDesc = Mp.Ccy;
                            //    Tp.TranAmount = Mp.Amount;
                            //    Tp.AuthCode = 0;
                            //    Tp.RefNumb = 0;
                            //    Tp.RemNo = 0;
                            //    Tp.TransMsg = "No comment from user";
                            //    Tp.AtmMsg = "";
                            //    Tp.MakerUser = UserId;
                            //    Tp.AuthUser = "";
                            //    Tp.OpenDate = DateTime.Now;
                            //    Tp.OpenRecord = true;
                            //    Tp.Operator = Operator;
                            //    //NOSTRO
                            //    Tp.NostroCcy = "";
                            //    Tp.NostroAdjAmt = 0;

                            //    int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

                            //    // UPDATE AND CLOSE UNMATCHED AND META EXCEPTIONS 
                            //    if (Tp.ErrorFound == false)
                            //    {
                            //        MetaExceptionNo = PostedNo;
                            //        Authoriser = Ap.Authoriser;
                            //        AuthoriserDtTm = DateTime.Now;
                            //        UpdateMatchingReconcExceptionsInfo(Operator, MaskRecordId);
                            //    }

                            //}

                            //if (ActionTypeId == 2) // Creation of Dispute + Transaction 
                            //{
                            //    RRDMDisputesTableClassITMX Di = new RRDMDisputesTableClassITMX();
                            //    RRDMDisputeTransactionsClassITMX Dt = new RRDMDisputeTransactionsClassITMX();

                            //    if (MetaExceptionId == 47) // Dispute to Bank A
                            //    {
                            //        Di.BankId = Mp.DebitBank;
                            //        Di.CardNo = Mp.MobileRequestor;
                            //        Di.CustPhone = Mp.MobileRequestor;
                            //        Di.AccNo = Mp.AccountRequestor;
                            //        Di.DispComments = "It appears in our records that your Bank didnt Debit the account " + Mp.AccountRequestor
                            //                           + " with the amount of " + Mp.Amount.ToString() + ". Please investigate and act on this dispute";

                            //        Dt.BankId = Mp.DebitBank;
                            //        WTxnCode = 11;
                            //        WDisputeActionId = 3;
                            //    }

                            //    if (MetaExceptionId == 50) // Dispute to Bank B
                            //    {
                            //        Di.BankId = Mp.CreditBank;
                            //        Di.CardNo = Mp.MobileBeneficiary;
                            //        Di.CustPhone = Mp.MobileBeneficiary;
                            //        Di.AccNo = Mp.AccountBeneficiary;
                            //        Di.DispComments = "It appears in our records that your Bank didnt Credit the account " + Mp.AccountRequestor
                            //                           + " with the amount of " + Mp.Amount.ToString() + ". Please investigate and act timely on this dispute!";

                            //        Dt.BankId = Mp.CreditBank;
                            //        WTxnCode = 21;
                            //        WDisputeActionId = 1;
                            //    }

                            //    Di.RespBranch = "Head Office :" + Di.BankId;
                            //    Di.LastUpdateDtTm = DateTime.Now;
                            //    Di.DispFrom = 7;

                            //    Di.CreatedByEntity = "ITMX";
                            //    Di.DispType = 0;

                            //    Di.DispType = 5; // Reconciliation Difference 

                            //    Di.OpenDate = DateTime.Now;
                            //    DateTime today = DateTime.Now;

                            //    Gp.ReadParametersSpecificId(Operator, "605", "1", "", ""); // LIMIT to be solved date // Dispute target dates 
                            //    int QualityRange1 = (int)Gp.Amount;

                            //    Di.TargetDate = today.AddDays(QualityRange1);

                            //    Di.CloseDate = NullPastDate;

                            //    Di.CustName = "George Koulis";

                            //    Di.CustEmail = "koulis.george@cablenet.com.cy";

                            //    Di.OtherDispTypeDescr = UnMatchedName;

                            //    Di.VisitType = "ITMX was called by customer";

                            //    Di.OpenByUserId = UserId;

                            //    Di.Active = true;

                            //    Di.Operator = Operator;

                            //    Di.DispId = Di.InsertDisputeRecord();

                            //    if (Mp.RecordFound == true)
                            //    {
                            //        //FoundInMatchedUnMatched = true;
                            //        Dt.MaskRecordId = Mp.MaskRecordId;

                            //        Dt.DispDtTm = Di.OpenDate;

                            //        Dt.DisputedAmt = Mp.Amount;

                            //        Dt.DisputeNumber = Di.DispId;

                            //        Dt.Origin = WOriginName;

                            //        //For ATMS
                            //        Dt.ErrNo = 0;
                            //        Dt.ReplCycle = 0;

                            //        Dt.StartTrxn = 0;
                            //        Dt.StartTrxn = 0;
                            //        Dt.Operator = Operator;

                            //        int DTranNo = Dt.InsertDisputeTran(Di.DispId);

                            //        Dt.ReadDisputeTran(DTranNo); // Get all values
                            //                                     // Prepare the two values 
                            //        Dt.TxnCode = WTxnCode;
                            //        Dt.DisputeActionId = WDisputeActionId;

                            //        Dt.UpdateDisputeTranRecord(DTranNo);

                            //    }

                            //    // UPDATE AND CLOSE UNMATCHED AND META EXCEPTIONS 
                            //    if (Tp.ErrorFound == false)
                            //    {
                            //        MetaExceptionNo = Di.DispId;
                            //        Authoriser = Ap.Authoriser;
                            //        AuthoriserDtTm = DateTime.Now;
                            //        UpdateMatchingReconcExceptionsInfo(Operator, MaskRecordId);
                            //    }
                            //}

                            //if (ActionTypeId == 3) // SMS 
                            //{

                            //    if (MetaExceptionId == 52) // Send SMS TO CUSTOMER 
                            //    {

                            //    }

                            //    // UPDATE AND CLOSE UNMATCHED AND META EXCEPTIONS 

                            //    MetaExceptionNo = 0;
                            //    Authoriser = Ap.Authoriser;
                            //    AuthoriserDtTm = DateTime.Now;
                            //    UpdateMatchingReconcExceptionsInfo(Operator, MaskRecordId);

                            //}

                            //if (ActionTypeId != 5) // Normal Transaction 
                            //                       // if 5 is postponed 
                            //{
                            //    string SelectionCriteria = " WHERE MaskRecordId =" + MaskRecordId;
                            //    Mp.ReadMatchingTxnsMasterPoolBySelectionCriteria("");

                            //    Mp.ActionTaken = true;
                            //    Mp.SettledRecord = true;

                            //    Mp.UpdateMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(MaskRecordId);
                            //}

                            //I++; // Read Next entry of the table 
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



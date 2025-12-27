using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMatchingTxns_MasterPoolITMX : Logger
    {
        public RRDMMatchingTxns_MasterPoolITMX() : base() { }

        public int MaskRecordId;

        public string MobileRequestor;
        public string MobileBeneficiary;
        public string ITMXUniqueTxnRef;
        //
        public string RequestOrigin;
        public string TxnType;
        public string AccountRequestor;
        //
        public string AccountBeneficiary;
        public DateTime RequestDateTm;
        public string Ccy;

        public decimal Amount;
        public string Particulars;
        public DateTime ExecutionTxnDtTm;
        //
        public bool TwoLegs;
        public string EmptyField;

        public string DebitMatchingCategory;

        public int DebitMatchingCycle;
        public int DebitTxnCode;

        public string DebitMASK;
        public string DebitBank;
        //
        public DateTime DebitDateTm;
        public string CreditMatchingCategory;
        public int CreditMatchingCycle;
        public int CreditTxnCode;
        public string CreditMASK;
        public string CreditBank;
        public DateTime CreditDateTm;

        public string ReconcCategoryId;
        public int ReconcCycleNo;
        public DateTime ReconcMatchingDateTm;
        public bool ReconcMatched;
        public bool ActionTaken;
        public bool Postponed;

        public string DebitFileId01;
        public int DebitSeqNo01;

        public string DebitFileId02;
        public int DebitSeqNo02;

        public string DebitFileId03;
        public int DebitSeqNo03;

        public string CreditFileId01;
        public int CreditSeqNo01;

        public string CreditFileId02;
        public int CreditSeqNo02;

        public string CreditFileId03;
        public int CreditSeqNo03;

        public bool SettledRecord; // Currently only for UnMatched 
        public string Operator;

        //*******************************************************************
        //*******************************************************************

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public int TotalUnMatched;
        public decimal TotalAmountUnMatched;
        public int TotalOutstandingForAction;

        RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        RRDMMatchingMasksVsMetaExceptions Rme = new RRDMMatchingMasksVsMetaExceptions();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        // Define the data table 

        public DataTable MatchingMasterDataTableITMX = new DataTable();
        public DataTable MatchingMasterDataTableITMX_AllFields = new DataTable();

        //public DataTable UnMatchedForDefaultActions = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public int TotalSelectedITMX;

        public decimal TotalAmount;

        string SqlString; // Do not delete 

        //string MatchingMasterFileId = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]";

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // Read Fields
        //
        private void ReadFields(SqlDataReader rdr)
        {
            MaskRecordId = (int)rdr["MaskRecordId"];

            MobileRequestor = (string)rdr["MobileRequestor"];
            MobileBeneficiary = (string)rdr["MobileBeneficiary"];
            ITMXUniqueTxnRef = (string)rdr["ITMXUniqueTxnRef"];

            RequestOrigin = (string)rdr["RequestOrigin"];
            TxnType = (string)rdr["TxnType"];
            AccountRequestor = (string)rdr["AccountRequestor"];

            AccountBeneficiary = (string)rdr["AccountBeneficiary"];
            RequestDateTm = (DateTime)rdr["RequestDateTm"];
            Ccy = (string)rdr["Ccy"];

            Amount = (decimal)rdr["Amount"];
            Particulars = (string)rdr["Particulars"];
            ExecutionTxnDtTm = (DateTime)rdr["ExecutionTxnDtTm"];

            TwoLegs = (bool)rdr["TwoLegs"];
            EmptyField = (string)rdr["EmptyField"];
            DebitMatchingCategory = (string)rdr["DebitMatchingCategory"];

            DebitMatchingCycle = (int)rdr["DebitMatchingCycle"];
            DebitTxnCode = (int)rdr["DebitTxnCode"];
            DebitMASK = (string)rdr["DebitMASK"];
            DebitBank = (string)rdr["DebitBank"];

            DebitDateTm = (DateTime)rdr["DebitDateTm"];
            CreditMatchingCategory = (string)rdr["CreditMatchingCategory"];
            CreditMatchingCycle = (int)rdr["CreditMatchingCycle"];
            CreditTxnCode = (int)rdr["CreditTxnCode"];
            CreditMASK = (string)rdr["CreditMASK"];
            CreditBank = (string)rdr["CreditBank"];
            CreditDateTm = (DateTime)rdr["CreditDateTm"];

            ReconcCategoryId = (string)rdr["ReconcCategoryId"];
            ReconcCycleNo = (int)rdr["ReconcCycleNo"];
            ReconcMatchingDateTm = (DateTime)rdr["ReconcMatchingDateTm"];
            ReconcMatched = (bool)rdr["ReconcMatched"];
            ActionTaken = (bool)rdr["ActionTaken"];
            Postponed = (bool)rdr["Postponed"];

            DebitFileId01 = (string)rdr["DebitFileId01"];
            DebitSeqNo01 = (int)rdr["DebitSeqNo01"];
            DebitFileId02 = (string)rdr["DebitFileId02"];
            DebitSeqNo02 = (int)rdr["DebitSeqNo02"];
            DebitFileId03 = (string)rdr["DebitFileId03"];
            DebitSeqNo03 = (int)rdr["DebitSeqNo03"];

            CreditFileId01 = (string)rdr["CreditFileId01"];
            CreditSeqNo01 = (int)rdr["CreditSeqNo01"];
            CreditFileId02 = (string)rdr["CreditFileId02"];
            CreditSeqNo02 = (int)rdr["CreditSeqNo02"];
            CreditFileId03 = (string)rdr["CreditFileId03"];
            CreditSeqNo03 = (int)rdr["CreditSeqNo03"];

            SettledRecord = (bool)rdr["SettledRecord"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ Specific by MaskRecordId
        // FILL UP A TABLE
        //
        public void ReadMatchingTxnsMasterPoolBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]"
                      + " "
                      + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);

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
        // READ Specific by MaskRecordId
        // 
        //
        public void ReadMatchingTxnsMasterPoolByMaskRecordId(int InMaskRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]"
                      + " WHERE MaskRecordId = @MaskRecordId ";

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

                            ReadFields(rdr);

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
        // READ ... NO RANGE ... No DATES
        // FILL UP A TABLE 
        //
        public void ReadMatchingTxnsMasterPoolAndFillTable(int InMode, string InSelectionCriteria, string InSortCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // If InMode = 1 then is normal without action 
            // If InMode = 2 then we examine action taken 
            // if InMode = 3 then this comes from Dispute registration 

            MatchingMasterDataTableITMX = new DataTable();
            MatchingMasterDataTableITMX.Clear();
            TotalSelectedITMX = 0;

            // DATA TABLE ROWS DEFINITION 
            if (InMode == 3)
            {
                MatchingMasterDataTableITMX.Columns.Add("Select", typeof(bool));
                MatchingMasterDataTableITMX.Columns.Add("DisputedAmnt", typeof(decimal));
            }
            MatchingMasterDataTableITMX.Columns.Add("RecordId", typeof(int));
            MatchingMasterDataTableITMX.Columns.Add("RecCycle", typeof(string));
            if (InMode == 2)
            {
                MatchingMasterDataTableITMX.Columns.Add("Done", typeof(string));
            }
            MatchingMasterDataTableITMX.Columns.Add("DebitMASK", typeof(string));
            MatchingMasterDataTableITMX.Columns.Add("CreditMASK", typeof(string));
            MatchingMasterDataTableITMX.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableITMX.Columns.Add("Amount", typeof(string));
            MatchingMasterDataTableITMX.Columns.Add("ExecutionTxnDtTm", typeof(DateTime));

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]"
                      + " "
                      + InSelectionCriteria
                      + " "
                      + InSortCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelectedITMX = TotalSelectedITMX + 1;

                            ReadFields(rdr);

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableITMX.NewRow();

                            if (InMode == 3)
                            {
                                RowSelected["Select"] = false;
                                RowSelected["DisputedAmnt"] = Amount;
                            }

                            RowSelected["RecordId"] = MaskRecordId;
                            RowSelected["RecCycle"] = ReconcCycleNo;
                            if (InMode == 2)
                            {
                                if (ActionTaken == false) RowSelected["Done"] = "NO";
                                else RowSelected["Done"] = "YES";
                            }

                            RowSelected["DebitMASK"] = DebitMASK;
                            RowSelected["CreditMASK"] = CreditMASK;
                            RowSelected["Ccy"] = Ccy;
                            RowSelected["Amount"] = Amount.ToString("#,##0.00");
                            RowSelected["ExecutionTxnDtTm"] = ExecutionTxnDtTm;

                            // ADD ROW
                            MatchingMasterDataTableITMX.Rows.Add(RowSelected);

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
        // READ Range ... DATES 
        // FILL UP A TABLE LEFT 
        //
        public void ReadMatchingTxnsMasterPoolByRangeAndFillTable(string InSignedId, int InMode, string InSelectionCriteria, string InCriteria2, string InCriteria3, string InSortCriteria, DateTime InFromDt, DateTime InToDt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // If InMode = 1 then is normal without action 
            // If InMode = 2 then we examine action taken 
            // if InMode = 3 then this comes from Dispute registration 

            MatchingMasterDataTableITMX = new DataTable();
            MatchingMasterDataTableITMX.Clear();
            TotalSelectedITMX = 0;

            // DATA TABLE ROWS DEFINITION 
            if (InMode == 3)
            {
                MatchingMasterDataTableITMX.Columns.Add("Select", typeof(bool));
                MatchingMasterDataTableITMX.Columns.Add("DisputedAmnt", typeof(decimal));
                MatchingMasterDataTableITMX.Columns.Add("RecordId", typeof(int));
            }
            else
            {
                if (InMode == 2)
                {
                    MatchingMasterDataTableITMX.Columns.Add("Done", typeof(string));
                    MatchingMasterDataTableITMX.Columns.Add("RecordId", typeof(int));
                    MatchingMasterDataTableITMX.Columns.Add("RecCycle", typeof(string));
                }
                else
                {
                    // This is InMode == 1
                    MatchingMasterDataTableITMX.Columns.Add("RecordId", typeof(int));
                    MatchingMasterDataTableITMX.Columns.Add("Status", typeof(string));
                    MatchingMasterDataTableITMX.Columns.Add("ReconcCategory", typeof(string));
                    MatchingMasterDataTableITMX.Columns.Add("RecCycle", typeof(string));
                }

            }

            MatchingMasterDataTableITMX.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableITMX.Columns.Add("Amount", typeof(string));

            MatchingMasterDataTableITMX.Columns.Add("DebitMASK", typeof(string));
            MatchingMasterDataTableITMX.Columns.Add("CreditMASK", typeof(string));

            MatchingMasterDataTableITMX.Columns.Add("ExecutionTxnDtTm", typeof(DateTime));

            if (InFromDt != NullPastDate)
            {
                SqlString = "SELECT *"
                     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]"
                     + " "
                     + InSelectionCriteria + InCriteria2 + InCriteria3
                     + " AND(ExecutionTxnDtTm >= @FromDt AND ExecutionTxnDtTm <= @ToDt) "
                     + " "
                     + InSortCriteria;
            }
            if (InFromDt == NullPastDate)
            {
                SqlString = "SELECT *"
                     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]"
                     + " "
                     + InSelectionCriteria + InCriteria2 + InCriteria3
                     + " "
                     + InSortCriteria;
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InFromDt != NullPastDate)
                        {
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelectedITMX = TotalSelectedITMX + 1;

                            ReadFields(rdr);

                            // DATA TABLE ROWS DEFINITION 

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableITMX.NewRow();

                            if (InMode == 3)
                            {
                                RowSelected["Select"] = false;
                                RowSelected["DisputedAmnt"] = Amount;
                                RowSelected["RecordId"] = MaskRecordId;
                            }
                            else
                            {
                                if (InMode == 2)
                                {
                                    if (ActionTaken == false) RowSelected["Done"] = "NO";
                                    else RowSelected["Done"] = "YES";
                                    RowSelected["RecordId"] = MaskRecordId;
                                    RowSelected["RecCycle"] = ReconcCycleNo;
                                }
                                else
                                {
                                    // InMode == 1
                                    RowSelected["RecordId"] = MaskRecordId;
                                    if (ReconcMatched == true) RowSelected["Status"] = "M";
                                    else
                                    {
                                        RowSelected["Status"] = "U";
                                    }
                                    Rcs.ReadReconcCategoriesByCategoryIdForName(ReconcCategoryId);
                                    RowSelected["ReconcCategory"] = Rcs.CategoryName;
                                    RowSelected["RecCycle"] = ReconcCycleNo;
                                }
                            }
                            RowSelected["Ccy"] = Ccy;
                            RowSelected["Amount"] = Amount.ToString("#,##0.00");

                            RowSelected["DebitMASK"] = DebitMASK;
                            RowSelected["CreditMASK"] = CreditMASK;
                            RowSelected["ExecutionTxnDtTm"] = ExecutionTxnDtTm;

                            // ADD ROW
                            MatchingMasterDataTableITMX.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    ////ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();

                    //Clear Table 
                    Tr.DeleteReport34(InSignedId);

                    int I = 0;

                    while (I <= (MatchingMasterDataTableITMX.Rows.Count - 1))
                    {

                        //    RecordFound = true;

                        Tr.RecordId = (int)MatchingMasterDataTableITMX.Rows[I]["RecordId"];
                        Tr.Status = (string)MatchingMasterDataTableITMX.Rows[I]["Status"];
                        Tr.ReconcCategory = (string)MatchingMasterDataTableITMX.Rows[I]["ReconcCategory"];
                        Tr.RecCycle = (string)MatchingMasterDataTableITMX.Rows[I]["RecCycle"];
                        Tr.Ccy = (string)MatchingMasterDataTableITMX.Rows[I]["Ccy"];
                        Tr.Amount = (string)MatchingMasterDataTableITMX.Rows[I]["Amount"];
                        Tr.DebitMASK = (string)MatchingMasterDataTableITMX.Rows[I]["DebitMASK"];
                        Tr.CreditMASK = (string)MatchingMasterDataTableITMX.Rows[I]["CreditMASK"];
                        Tr.ExecutionTxnDtTm = (DateTime)MatchingMasterDataTableITMX.Rows[I]["ExecutionTxnDtTm"];

                        Tr.InsertReport34(InSignedId);

                        I++; // Read Next entry of the table 
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ TOTAL UNMATCHED FOR RM CATEGORY 
        // FILL UP A TABLE
        //
        public void ReadMatchingTxnsMasterPoolForTotalsForUnMatched(string InOperator, string InReconcCategoryId, int InReconcCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalUnMatched = 0;

            TotalAmountUnMatched = 0;

            TotalOutstandingForAction = 0;

            SqlString = "SELECT *"
               + " FROM " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]"
            + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId"
                          + " AND ReconcCycleNo = @ReconcCycleNo AND ReconcMatched = 0 ";

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

                            ReadFields(rdr);

                            // Update Totals 

                            TotalUnMatched = TotalUnMatched + 1;
                            TotalAmountUnMatched = TotalAmountUnMatched + Amount;

                            if (ActionTaken == false)
                            {
                                TotalOutstandingForAction = TotalOutstandingForAction + 1;
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
        // READ Specific by unique ITMX ref 
        // 
        //
        public void ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(string InOperator, int InMaskRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
               + " FROM " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]"
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

                            ReadFields(rdr);

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
        // READ UnMatched for default actions
        // 
        // 
        //
        public int TotalSelected;
        bool DefaultAction;
        public void ReadTableUnMatchedForDefaultActions(string InOperator, string InSignedId, int InITMXSettlementCycle)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMErrorsClassWithActionsITMX Er = new RRDMErrorsClassWithActionsITMX();

            // Clear MatchingMasterDataTableITMX_AllFields 

            MatchingMasterDataTableITMX_AllFields = new DataTable();
            MatchingMasterDataTableITMX_AllFields.Clear();

            TotalSelected = 0;

            SqlString = "SELECT *"
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]"
              + " WHERE Operator = @Operator AND ReconcMatched = 0 AND SettledRecord = 0 ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                    //sqlAdapt.SelectCommand.Parameters.AddWithValue("@ITMXSettlementCycle", InITMXSettlementCycle);

                    //Create a datatable that will be filled with the data retrieved from the command

                    sqlAdapt.Fill(MatchingMasterDataTableITMX_AllFields);

                    // Close conn
                    conn.Close();

                    RRDMMatchingReconcExceptionsInfoITMX Mre = new RRDMMatchingReconcExceptionsInfoITMX();


                    int I = 0;
                    //Read All Unmatched and create table for default actions 
                    while (I <= (MatchingMasterDataTableITMX_AllFields.Rows.Count - 1))
                    {

                        RecordFound = true;

                        MaskRecordId = (int)MatchingMasterDataTableITMX_AllFields.Rows[I]["MaskRecordId"];

                        MobileRequestor = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["MobileRequestor"];

                        MobileBeneficiary = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["MobileBeneficiary"];
                        ITMXUniqueTxnRef = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["ITMXUniqueTxnRef"];

                        RequestOrigin = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["RequestOrigin"];
                        TxnType = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["TxnType"];
                        AccountRequestor = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["AccountRequestor"];

                        AccountBeneficiary = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["AccountBeneficiary"];
                        RequestDateTm = (DateTime)MatchingMasterDataTableITMX_AllFields.Rows[I]["RequestDateTm"];
                        Ccy = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["Ccy"];

                        Amount = (decimal)MatchingMasterDataTableITMX_AllFields.Rows[I]["Amount"];
                        Particulars = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["Particulars"];
                        ExecutionTxnDtTm = (DateTime)MatchingMasterDataTableITMX_AllFields.Rows[I]["ExecutionTxnDtTm"];

                        DebitMASK = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["DebitMASK"];
                        CreditMASK = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["CreditMASK"];

                        ReconcCategoryId = (string)MatchingMasterDataTableITMX_AllFields.Rows[I]["ReconcCategoryId"];
                        ReconcCycleNo = (int)MatchingMasterDataTableITMX_AllFields.Rows[I]["ReconcCycleNo"];
                        ReconcMatchingDateTm = (DateTime)MatchingMasterDataTableITMX_AllFields.Rows[I]["ReconcMatchingDateTm"];
                        ReconcMatched = (bool)MatchingMasterDataTableITMX_AllFields.Rows[I]["ReconcMatched"];
                        ActionTaken = (bool)MatchingMasterDataTableITMX_AllFields.Rows[I]["ActionTaken"];

                        // Find meta EXCEPTION NUMBER 

                        string DrCrMask = DebitMASK + CreditMASK;

                        Rme.ReadMatchingMaskRecordbyMaskId(InOperator, ReconcCategoryId, DrCrMask, 11);

                        string UnMatchedType;
                        int MetaExceptionId;

                        if (Rme.RecordFound)
                        {
                            UnMatchedType = Rme.MaskName;
                            MetaExceptionId = Rme.MetaExceptionId;
                        }
                        else
                        {
                            UnMatchedType = "Not Specified";
                            MetaExceptionId = 0;
                        }


                        DefaultAction = false;

                        Er.ReadErrorsIDRecord(Rme.MetaExceptionId, InOperator);

                        if (Er.RecordFound)
                        {
                            if (Er.TurboReconc == true)
                            {
                                DefaultAction = true;
                            }
                          
                        }
                   
                        if (DefaultAction == true)
                        {
                            Mre.ReadMatchingReconcExceptionsInfobyMaskRecordId(InOperator, MaskRecordId);
                            if (Mre.RecordFound == true)
                            {
                               
                            }
                            else
                            {
                                // Create Exception Record

                                Mre.MaskRecordId = MaskRecordId;
                                Mre.ITMXUniqueTxnRef = ITMXUniqueTxnRef;
                                Mre.ReconcCategoryId = ReconcCategoryId;
                                Mre.ReconcCycleNo = ReconcCycleNo;

                                Mre.UnMatchedName = Rme.MaskName;

                                Mre.DefaultAction = true;

                                Mre.ExceptionRecomm = Er.CircularDesc;

                                Mre.MetaExceptionId = Rme.MetaExceptionId;

                                Mre.ActionTypeId = 7;
                                Mre.ActionTypeDescr = "Default Action";

                                Mre.CreatedDtTm = DateTime.Now;
                                Mre.Operator = InOperator;

                                Mre.InsertMatchingReconcExceptionsInfo();

                            }
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
        // UPDATE MatchingTxnsMasterPoolSpecific
        // 
        public void UpdateMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(int InMaskRecordId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolITMX]" + " SET "
                              + " ActionTaken = @ActionTaken, Postponed = @Postponed ,  SettledRecord = @SettledRecord "
                              + " WHERE MaskRecordId = @MaskRecordId", conn))
                    {
                        cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);

                        cmd.Parameters.AddWithValue("@ActionTaken", ActionTaken);
                        cmd.Parameters.AddWithValue("@Postponed", Postponed);
                        cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);
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



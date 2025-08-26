using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;


namespace RRDM4ATMs
{
    public class RRDMTransAndTransToBePostedClass : Logger
    {
        public RRDMTransAndTransToBePostedClass() : base() { }
        // Fields for both InPool and Trans to be posted 

        public int PostedNo;

        public string OriginId; // *
                                // "01" OurATMS-Matching
                                // "02" BancNet Matching                               
                                // "03" OurATMS-Reconc
                                // "04" OurATMS-Repl
                                // "05" Settlement
                                // "07" Disputes 
                                // "08" Instructions to CIT

        public string OriginName;

        public string RMCateg; //* BDC201, BDC202 etc
        public int RMCategCycle; //*

        public int ActionSeqNo;

        public int UniqueRecordId;

        public int ErrNo;
        public string AtmNo;
        public int SesNo;

        public int RMCycleNo;

        public int DisputeNo;
        public int DispTranNo;

        public string BankId;

        public int AtmTraceNo;

        public string RRNumber;
        public string TXNSRC;
        public string TXNDEST;

        public int MasterTraceNo;

        public string BranchId;
        public DateTime AtmDtTime;


        //public DateTime HostDtTime;

        public string CardNo;
        public int CardOrigin;

        public int SystemTarget;
        public string BranchId1; // Branch for the first Txn 
        public int TransType; // 11 And 21 is related with customer withdrwals and reversals cassettes 
                              // 12 And 22 are related with Cash In and Cash out during Replenishment 
                              // 23 Cash customer deposits 24 Cheques customer deposits 
        public string TransDesc;
        public string AccNo;
        public bool GlEntry;

        public string BranchId2; // Branch for the second Txn 
        public int TransType2;
        public string TransDesc2;
        public string AccNo2;
        public bool GlEntry2;

        public string CurrDesc;
        public decimal TranAmount;
        public int AuthCode;
        public int RefNumb;
        public int RemNo;

        public string TransMsg;
        public string AtmMsg;

        //     public string OriginatorUser;
        public string AuthUser; // THIS IS FOR AUTHORise user to access these transactions 
        public string ActionBy;

        public int ActionCd2;

        public DateTime ActionDate;

        public DateTime OpenDate;

        public bool OpenRecord;

        public int StartTrxn;
        public int EndTrxn;

        public decimal DepCount;

        public int CommissionCode;
        public decimal CommissionAmount;

        public bool SuccTran;

        // DEFINE FIELDS FOR HOST TRANSACTIONS 

        public int TranNoH;
        public int TranOriginH;
        public int AtmTraceNoH;
        public int HostTraceNoH;

        public DateTime HostDtTimeH;

        public string TransDescH;
        public string CardNoH;

        public string AccNoH;

        public int AuthCodeH;
        public int RefNumbH;
        public int RemNoH;

        public string TransMsgH;
        public int ErrNoH;

        public bool SuccTranH;

        public bool HostMatched;
        public DateTime MatchedDtTm;
        public int Reconciled;
        public bool IsReversal;
        public string Operator;

        string ParamId;
        string OccuranceId;
        string RelatedParmId;
        string RelatedOccuranceId;

        public DateTime GridFilterDate; // THIS WILL NOT CONTAIN MINUTES       

        // Fields for InPool Trans
        public int TranNo;
        public int EJournalTraceNo;

        public int TotActions;
        public int TotPairTransactions;
        public int TotActionsTaken;
        public int TotActionsNotTaken;

        // Define the data table 
        public DataTable TransToBePostedDataTable = new DataTable();

        public DataTable DisputedTransDataTable = new DataTable();

        public DataTable InPoolTransactionsDataTable = new DataTable();
        public DataTable InPoolMasterTraceNoDataTable = new DataTable();

        public DataTable BDC_GIFU = new DataTable();
        public DataTable BDC_GL = new DataTable();
        public DataTable BDC_Settlement_Dpt = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;
        readonly string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMAtmsDailyTransHistory Ah = new RRDMAtmsDailyTransHistory();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMHolidays Ho = new RRDMHolidays();
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMReplStatsClass Rs = new RRDMReplStatsClass();
        RRDMAtmsCostClass Ap = new RRDMAtmsCostClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();


        // READ FIELDS
        private void ReadFieldsTransToBePosted(SqlDataReader rdr)
        {
            PostedNo = (int)rdr["PostedNo"];

            OriginId = (string)rdr["OriginId"];
            OriginName = (string)rdr["OriginName"];
            RMCateg = (string)rdr["RMCateg"];
            RMCategCycle = (int)rdr["RMCategCycle"];
            ActionSeqNo = (int)rdr["ActionSeqNo"];
            UniqueRecordId = (int)rdr["UniqueRecordId"];
            ErrNo = (int)rdr["ErrNo"];
            AtmNo = (string)rdr["AtmNo"];
            SesNo = (int)rdr["SesNo"];

            RMCycleNo = (int)rdr["RMCycleNo"];

            DisputeNo = (int)rdr["DisputeNo"];
            DispTranNo = (int)rdr["DispTranNo"];

            BankId = (string)rdr["BankId"];

            AtmTraceNo = (int)rdr["AtmTraceNo"];
            RRNumber = (string)rdr["RRNumber"];

            TXNSRC = (string)rdr["TXNSRC"];
            TXNDEST = (string)rdr["TXNDEST"];

            BranchId = (string)rdr["BranchId"];

            AtmDtTime = (DateTime)rdr["AtmDtTime"];

            SystemTarget = (int)rdr["SystemTarget"];

            CardNo = (string)rdr["CardNo"];
            CardOrigin = (int)rdr["CardOrigin"];

            // First Transaction

            BranchId1 = (string)rdr["BranchId1"];
            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
            TransDesc = (string)rdr["TransDesc"];
            AccNo = (string)rdr["AccNo"];
            GlEntry = (bool)rdr["GlEntry"];
            //NOSTRO
            NostroCcy = (string)rdr["NostroCcy"];
            NostroAdjAmt = (decimal)rdr["NostroAdjAmt"];
            // Second Transaction
            BranchId2 = (string)rdr["BranchId2"];
            TransType2 = (int)rdr["TransType2"]; // 11 for debit 21 for credit
            TransDesc2 = (string)rdr["TransDesc2"];
            AccNo2 = (string)rdr["AccNo2"];
            GlEntry2 = (bool)rdr["GlEntry2"];
            // End of second 

            CurrDesc = (string)rdr["CurrDesc"];
            TranAmount = (decimal)rdr["TranAmount"];

            AuthCode = (int)rdr["AuthCode"];
            RefNumb = (int)rdr["RefNumb"];
            RemNo = (int)rdr["RemNo"];

            TransMsg = (string)rdr["TransMsg"];
            AtmMsg = (string)rdr["AtmMsg"];

            MakerUser = (string)rdr["MakerUser"];
            AuthUser = (string)rdr["AuthUser"];
            ActionBy = (string)rdr["ActionBy"];

            ActionCd2 = (int)rdr["ActionCd2"];

            ActionDate = (DateTime)rdr["ActionDate"];

            OpenDate = (DateTime)rdr["OpenDate"];

            OpenRecord = (bool)rdr["OpenRecord"];

            HostMatched = (bool)rdr["HostMatched"];
            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];
            Reconciled = (int)rdr["Reconciled"];

            GridFilterDate = (DateTime)rdr["GridFilterDate"];

            IsReversal = (bool)rdr["IsReversal"];

            Operator = (string)rdr["Operator"];
        }

        // READ TRANS TO BE POSTED  BASED ON SELECTION CRITERIA  
        // To find one 

        public void ReadAllTransToBePostedToSeeIfAny(string InSignedId, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT TOP (1) * "
                       + " FROM [ATMS].[dbo].[TransToBePosted] "
                       + " WHERE " + InSelectionCriteria
                       + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
         
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //************************************************
                            //************************************************

                            // Read Txns fields
                            ReadFieldsTransToBePosted(rdr);
         
                            //************************************************
                        }

                        // Close ReaderOpenRecord
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

        // READ ALL TRANS TO BE POSTED  BASED ON SELECTION CRITERIA  

        public void ReadAllTransToBePostedAndFillTable(string InSignedId, string InSelectionCriteria, DateTime InOpenDate, bool InWithDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TransToBePostedDataTable = new DataTable();
            TransToBePostedDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TransToBePostedDataTable.Columns.Add("PostedNo", typeof(int));
            TransToBePostedDataTable.Columns.Add("Select", typeof(bool));
            TransToBePostedDataTable.Columns.Add("Origin", typeof(string));
            TransToBePostedDataTable.Columns.Add("Done", typeof(string));

            TransToBePostedDataTable.Columns.Add("CurrDesc", typeof(string));
            TransToBePostedDataTable.Columns.Add("TranAmount", typeof(string));

            TransToBePostedDataTable.Columns.Add("Type", typeof(string));
            TransToBePostedDataTable.Columns.Add("TransDesc", typeof(string));
            TransToBePostedDataTable.Columns.Add("AccNo", typeof(string));
            TransToBePostedDataTable.Columns.Add("GlEntry", typeof(bool));

            TransToBePostedDataTable.Columns.Add("Type2", typeof(string));
            TransToBePostedDataTable.Columns.Add("TransDesc2", typeof(string));
            TransToBePostedDataTable.Columns.Add("AccNo2", typeof(string));
            TransToBePostedDataTable.Columns.Add("GlEntry2", typeof(bool));

            TransToBePostedDataTable.Columns.Add("AtmNo", typeof(string));
            TransToBePostedDataTable.Columns.Add("ReplCycle", typeof(int));

            SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[TransToBePosted] "
                       + " WHERE " + InSelectionCriteria
                       + " Order By PostedNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InWithDate == true)
                        {
                            cmd.Parameters.AddWithValue("@OpenDate", InOpenDate);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //************************************************
                            //************************************************

                            TotalSelected = TotalSelected + 1;

                            // Read Txns fields
                            ReadFieldsTransToBePosted(rdr);

                            //OpenRecord = (bool)rdr["OpenRecord"];

                            DataRow RowSelected = TransToBePostedDataTable.NewRow();

                            RowSelected["PostedNo"] = PostedNo;

                            RowSelected["Select"] = false;

                            RowSelected["Origin"] = OriginName;
                            if (OpenRecord == true)
                            {
                                RowSelected["Done"] = "NO";
                            }
                            else
                            {
                                RowSelected["Done"] = "YES";
                            }

                            RowSelected["CurrDesc"] = CurrDesc;

                            //decimal Temp = (decimal)rdr["TranAmount"] ;
                            //string Temp2 = Temp.ToString("#,##0.00");
                            RowSelected["TranAmount"] = TranAmount.ToString("#,##0.00");
                            TransType = TransType; // 11 for debit 21 for credit

                            if (TransType == 11 || TransType == 12)
                            {
                                RowSelected["Type"] = "DR";
                            }
                            if (TransType == 21 || TransType == 22)
                            {
                                RowSelected["Type"] = "CR";
                            }

                            RowSelected["TransDesc"] = TransDesc;
                            RowSelected["AccNo"] = AccNo;
                            RowSelected["GlEntry"] = GlEntry;

                            TransType2 = TransType2; // 11 for debit 21 for credit

                            if (TransType2 == 11 || TransType2 == 12)
                            {
                                RowSelected["Type2"] = "DR";
                            }
                            if (TransType2 == 21 || TransType2 == 22)
                            {
                                RowSelected["Type2"] = "CR";
                            }

                            RowSelected["TransDesc2"] = TransDesc2;
                            RowSelected["AccNo2"] = AccNo2;
                            RowSelected["GlEntry2"] = GlEntry2;

                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["ReplCycle"] = SesNo;

                            // ADD ROW
                            TransToBePostedDataTable.Rows.Add(RowSelected);

                            //************************************************
                        }

                        // Close ReaderOpenRecord
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertReport(InSignedId);
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }
        // READ ALL TRANS TO BE Fill POSTED  BASED ON SELECTION CRITERIA  
        public int TotalDr;
        public decimal TotalDrAmt;

        public int TotalCr;
        public decimal TotalCrAmt;
        // Create table for _BDC_GL
        public void ReadToBePostedAndFillTable_BDC_GL(string InSignedId, int InRMCycle, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMBank_Branches Br = new RRDMBank_Branches();
            RRDMAccountsClass Acc = new RRDMAccountsClass();

            // CLEAR TABLE
            BDC_GL = new DataTable();
            BDC_GL.Clear();

            TotalDr = 0;
            TotalDrAmt = 0;

            TotalCr = 0;
            TotalCrAmt = 0;

            // DATA TABLE ROWS DEFINITION 
            BDC_GL.Columns.Add("BRANCH_NAME", typeof(string));
            BDC_GL.Columns.Add("A/c_Brn", typeof(string));
            BDC_GL.Columns.Add("A/c_No", typeof(string));

            BDC_GL.Columns.Add("A/CNo/CIT_NAME", typeof(string));

            BDC_GL.Columns.Add("Remarks", typeof(string));
            BDC_GL.Columns.Add("A/c_Ccy", typeof(string));

            BDC_GL.Columns.Add("LCY_AMOUNT", typeof(string));
            BDC_GL.Columns.Add("NET_LCY", typeof(string));

            BDC_GL.Columns.Add("AUTH_ID", typeof(string));
            BDC_GL.Columns.Add("Type", typeof(string)); // DR-CR
            //
            // GET THE GL ENTRIES
            //
            if (InMode == 1)
            {
                SqlString = "SELECT *"
                       + " FROM [dbo].[TransToBePosted] "
                       + " WHERE GLEntry = 1 AND GlEntry2 = 1 AND RMCycleNo = @RMCycleNo ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //************************************************
                            //************************************************

                            TotalSelected = TotalSelected + 1;

                            // Read Txns fields
                            ReadFieldsTransToBePosted(rdr);

                            DataRow RowSelected = BDC_GL.NewRow();


                            RowSelected["A/c_Brn"] = BranchId1;
                            Br.ReadBranchByBranchId(BranchId1);
                            RowSelected["BRANCH_NAME"] = Br.BranchName;

                            RowSelected["A/c_No"] = AccNo;
                            Acc.ReadAccount_Name_Based_On_Account_No(AccNo);

                            RowSelected["A/CNo/CIT_NAME"] = Acc.AccName;
                            RowSelected["Remarks"] = TransDesc;
                            RowSelected["A/c_Ccy"] = "L.E";
                            RowSelected["LCY_AMOUNT"] = TranAmount.ToString("###0");
                            RowSelected["NET_LCY"] = "-" + (TranAmount.ToString("###0"));
                            RowSelected["AUTH_ID"] = InSignedId;
                            if (TransType == 11 || TransType == 12)
                            {
                                RowSelected["Type"] = "DR";
                                TotalDr = TotalDr + 1;
                                TotalDrAmt = TotalDrAmt + TranAmount;
                            }
                            if (TransType == 21 || TransType == 22)
                            {
                                RowSelected["Type"] = "CR";
                                TotalCr = TotalCr + 1;
                                TotalCrAmt = TotalCrAmt + TranAmount;
                            }

                            // ADD ROW
                            BDC_GL.Rows.Add(RowSelected);

                            DataRow RowSelected2 = BDC_GL.NewRow();


                            RowSelected2["A/c_Brn"] = BranchId2;
                            Br.ReadBranchByBranchId(BranchId2);
                            RowSelected2["BRANCH_NAME"] = Br.BranchName;

                            RowSelected2["A/c_No"] = AccNo2;
                            Acc.ReadAccount_Name_Based_On_Account_No(AccNo2);

                            RowSelected2["A/CNo/CIT_NAME"] = Acc.AccName;
                            RowSelected2["Remarks"] = TransDesc2;
                            RowSelected2["A/c_Ccy"] = "L.E";
                            RowSelected2["LCY_AMOUNT"] = TranAmount.ToString("###0");
                            RowSelected2["NET_LCY"] = "-" + (TranAmount.ToString("###0"));
                            RowSelected2["AUTH_ID"] = InSignedId;
                            if (TransType2 == 11 || TransType2 == 12)
                            {
                                RowSelected2["Type"] = "DR";
                                TotalDr = TotalDr + 1;
                                TotalDrAmt = TotalDrAmt + TranAmount;
                            }
                            if (TransType2 == 21 || TransType2 == 22)
                            {
                                RowSelected2["Type"] = "CR";
                                TotalCr = TotalCr + 1;
                                TotalCrAmt = TotalCrAmt + TranAmount;
                            }

                            // ADD ROW
                            BDC_GL.Rows.Add(RowSelected2);

                            //************************************************
                        }

                        // Close ReaderOpenRecord
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertReport(InSignedId);
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }
        // GL transactions 
        public void ReadToBePostedAndFillTable_BDC_GL_NEW(string InSignedId, int InRMCycle,DateTime InOpenDate, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //RRDMBank_Branches Br = new RRDMBank_Branches();
            //RRDMAccountsClass Acc = new RRDMAccountsClass();
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool IsSelectByDate = false;

            if (InOpenDate != NullPastDate)
            {
                // Select By Date
                IsSelectByDate = true;
            }
            else
            {
                // Select By Repl 
                IsSelectByDate = false;
            }

            // CLEAR TABLE
            BDC_GL = new DataTable();
            BDC_GL.Clear();


            TotalDr = 0;
            TotalDrAmt = 0;

            TotalCr = 0;
            TotalCrAmt = 0;

            TotalSelected = 0; 

            // DATA TABLE ROWS DEFINITION 
            BDC_GL.Columns.Add("Branch Code", typeof(string));
            BDC_GL.Columns.Add("DR/CR", typeof(string));
            BDC_GL.Columns.Add("GL", typeof(string));

            BDC_GL.Columns.Add("Amount", typeof(string));
            BDC_GL.Columns.Add("Description", typeof(string));

            BDC_GL.Columns.Add("Terminal ID", typeof(string));
            BDC_GL.Columns.Add("Terminal Name", typeof(string));

            BDC_GL.Columns.Add("Type of TXN", typeof(string));
            //
            // GET THE GL ENTRIES
            //
            if (InMode == 1 & IsSelectByDate == true)
            {
                SqlString = "SELECT * "
                       + " FROM [dbo].[TransToBePosted] "
                       + " WHERE GLEntry = 1 AND GlEntry2 = 1 AND CAST(OpenDate As DATE) = @OpenDate ";
            }
            else
            {
                SqlString = "SELECT * "
                      + " FROM [dbo].[TransToBePosted] "
                      + " WHERE GLEntry = 1 AND GlEntry2 = 1 AND RMCycleNo = @RMCycleNo ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@OpenDate", InOpenDate);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //************************************************
                            //************************************************

                            TotalSelected = TotalSelected + 1;

                            // Read Txns fields
                            ReadFieldsTransToBePosted(rdr);

                            DataRow RowSelected = BDC_GL.NewRow();
                            //s.TrimStart('0')
                           // RowSelected["Branch Code"] = BranchId1.TrimStart('0');
                            RowSelected["Branch Code"] = "'" + BranchId1;
                            if (TransType == 11 || TransType == 12)
                            {
                                RowSelected["DR/CR"] = "D";
                                TotalDr = TotalDr + 1;
                                TotalDrAmt = TotalDrAmt + TranAmount;
                            }
                            if (TransType == 21 || TransType == 22)
                            {
                                RowSelected["DR/CR"] = "C";
                                TotalCr = TotalCr + 1;
                                TotalCrAmt = TotalCrAmt + TranAmount;
                            }
                         
                            RowSelected["GL"] = AccNo;  // Account Number

                            //RowSelected["Amount"] = TranAmount.ToString("###0");
                            RowSelected["Amount"] = TranAmount.ToString();
                            RowSelected["Description"] = TransDesc;
                         
                            RowSelected["Terminal ID"] = "'" + AtmNo;
                            Ac.ReadAtm(AtmNo); 
                            if (Ac.RecordFound == true)
                            {
                                RowSelected["Terminal Name"] = Ac.AtmName;
                            }
                            else
                            {
                                RowSelected["Terminal Name"] = "Not defined";
                            }

                            RowSelected["Type of TXN"] = GetTypeOfTxn(TransDesc);


                            // ADD ROW
                            BDC_GL.Rows.Add(RowSelected);

                            DataRow RowSelected2 = BDC_GL.NewRow();

                            //RowSelected2["Branch Code"] = BranchId2.TrimStart('0');
                            RowSelected2["Branch Code"] = "'" + BranchId2;
                            if (TransType2 == 11 || TransType2 == 12)
                            {
                                RowSelected2["DR/CR"] = "D";
                                TotalDr = TotalDr + 1;
                                TotalDrAmt = TotalDrAmt + TranAmount;
                            }
                            if (TransType2 == 21 || TransType2 == 22)
                            {
                                RowSelected2["DR/CR"] = "C";
                                TotalCr = TotalCr + 1;
                                TotalCrAmt = TotalCrAmt + TranAmount;
                            }

                            RowSelected2["GL"] = AccNo2;  // Account Number

                            //RowSelected2["Amount"] = TranAmount.ToString("###0");
                            RowSelected2["Amount"] = TranAmount.ToString();
                            RowSelected2["Description"] = TransDesc2;

                            RowSelected2["Terminal ID"] = "'" + AtmNo;

                            Ac.ReadAtm(AtmNo);
                            if (Ac.RecordFound == true)
                            {
                                RowSelected2["Terminal Name"] = Ac.AtmName;
                            }
                            else
                            {
                                RowSelected2["Terminal Name"] = "Not defined";
                            }

                            RowSelected2["Type of TXN"] = GetTypeOfTxn(TransDesc2);
                            // ADD ROW
                            BDC_GL.Rows.Add(RowSelected2);

                            //************************************************
                        }

                        // Close ReaderOpenRecord
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertReport(InSignedId);
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }

        private string GetTypeOfTxn(string InStatementDesc)
        {
            string WTypeOfTxn; 
            string ShortDesc = InStatementDesc.Substring(0, 2);

            // FOR FIRST ENTRY 
            switch (ShortDesc)
            {
                case "01":
                    {
                        //01_Reversal,Trace 155688,date 03-10-2019_ATM_0567
                        WTypeOfTxn = "Reversal";
                        break;
                    }
                case "02":
                    {
                        //02_REPLENISHMENT 00000071 13-10-2019
                        WTypeOfTxn = "REPLENISHMENT ATM ";
                                     
                        break;
                    }
                case "03":
                    {
                        //03_REMAINING 00000071 13-10-2019
                        WTypeOfTxn = "REMAINING ATM ";
                                  
                        break;
                    }
                case "04":
                    {
                        // 04_EXCESS 000000900 10-10-2019
                        WTypeOfTxn = "EXCESS ATM ";
                        break;
                    }
                case "05":
                    {
                        // 05_SHORTAGE 000000900 10-10-2019
                        WTypeOfTxn = "SHORTAGE ATM ";
                        break;
                    }
                case "06":
                    {
                        // 06_Reversal_Cust_Acc_xxxxxxxxxxx 13 - 10 - 2019
                        WTypeOfTxn = "Reversal_For_Cust_Acc_" ;
                        break;
                    }
                case "07":
                    {
                        // 07_Reversal_Cust_Reference_xxxxxxxxxxx 13-10-2019 - Dstn 5
                        WTypeOfTxn = "Reversal_Cust_Reference_" ;
                        break;
                    }
                case "08":
                    {
                        //08_Reversal Reference xxxxxxxx_ 13-10-2019 
                        WTypeOfTxn = "Reversal Reference-";
                        break;
                    }
                case "09":
                    {
                        //09_Reversal Reference xxxxxxxx_ 13-10-2019 to Cat_ 
                        WTypeOfTxn = "Reversal Reference-" ;
                        break;
                    }
                case "10":
                    {
                        // 10_Reversal Reference xxxxxxxx_ 13-10-2019 From Cat_ 
                        WTypeOfTxn = "Reversal Reference-" ;
                        break;
                    }
                case "11":
                    {
                        // 11_DEPOSITS Counted 00000071 13-10-2019 
                        WTypeOfTxn = "DEPOSITS Counted-ATM-" ;
                        break;
                    }
                case "12":
                    {
                        //12_EXCESS 000000900 10-10- 2019(DEPOSITS)
                        WTypeOfTxn = "EXCESS ATM " ;
                        break;
                    }
                case "13":
                    {
                        //13_SHORTAGE 000000900 10 - 10 - 2019(DEPOSITS)
                        WTypeOfTxn = "SHORTAGE ATM " ;
                        break;
                    }
                case "14":
                    {
                        //01_Reversal,Trace 155688,date 03-10-2019_ATM_0567
                        WTypeOfTxn = "Reversal Missing";
                        break;
                    }
                case "15":
                    {
                        //01_Reversal,Trace 155688,date 03-10-2019_ATM_0567
                        WTypeOfTxn = "Reversal bills";
                        break;
                    }
                default:
                    {
                        //MessageBox.Show("Not defined ");
                        WTypeOfTxn = "Not Found Definition";

                        break;
                    }         
            }
            return WTypeOfTxn;
        }
        // Create table for _BDC_GIFU 
        public void ReadToBePostedAndFillTable_BDC_GIFU(string InSignedId, int InRMCycle,DateTime InOpenDate, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMBank_Branches Br = new RRDMBank_Branches();
            RRDMAccountsClass Acc = new RRDMAccountsClass();

            bool IsSelectByDate = false;

            if (InOpenDate != NullPastDate)
            {
                // Select By Date
                IsSelectByDate = true;
            }
            else
            {
                // Select By Repl 
                IsSelectByDate = false;
            }

            // CLEAR TABLE
            BDC_GIFU = new DataTable();
            BDC_GIFU.Clear();

            TotalDr = 0;
            TotalDrAmt = 0;

            TotalCr = 0;
            TotalCrAmt = 0;

            TotalSelected = 0; 

            string GIFU_GL_Account = "";

            // DATA TABLE ROWS DEFINITION 
            BDC_GIFU.Columns.Add("Txn_Desc", typeof(string));
            BDC_GIFU.Columns.Add("Group_Kind", typeof(string));
            BDC_GIFU.Columns.Add("TxnCurr", typeof(string));

            BDC_GIFU.Columns.Add("Amount", typeof(string));

            BDC_GIFU.Columns.Add("Branch", typeof(string));
            BDC_GIFU.Columns.Add("ScreenNo", typeof(string));

            BDC_GIFU.Columns.Add("Flexacc", typeof(string));
            BDC_GIFU.Columns.Add("TABLENO", typeof(string));
            //
            // GET THE _GIFU ENTRIES
            //
            if (InMode == 2 & IsSelectByDate == true)
            {
                SqlString = "SELECT *"
                       + " FROM [dbo].[TransToBePosted] "
                       + " WHERE ((GLEntry = 1 AND GlEntry2 = 0) "
                       + " OR (GLEntry = 0 AND GlEntry2 = 1)) "
                       + " AND CAST(OpenDate As DATE) = @OpenDate ";
            }
            else
            {
                SqlString = "SELECT *"
                       + " FROM [dbo].[TransToBePosted] "
                       + " WHERE ((GLEntry = 1 AND GlEntry2 = 0) "
                       + " OR (GLEntry = 0 AND GlEntry2 = 1)) "
                       + " AND RMCycleNo = @RMCycleNo ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycle);
                        cmd.Parameters.AddWithValue("@OpenDate", InOpenDate.Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //************************************************
                            //************************************************

                            TotalSelected = TotalSelected + 1;

                            // Read Txns fields
                            ReadFieldsTransToBePosted(rdr);

                            DataRow RowSelected = BDC_GIFU.NewRow();

                            if (GlEntry == false)
                            {
                                // THE FIRST OF PAIR IS FLEXCUBE 
                                RowSelected["Txn_Desc"] = TransDesc;

                                if (TransType == 11 || TransType == 12)
                                {
                                    RowSelected["Group_Kind"] = "2";
                                    TotalDr = TotalDr + 1;
                                    TotalDrAmt = TotalDrAmt + TranAmount;
                                }
                                if (TransType == 21 || TransType == 22)
                                {
                                    RowSelected["Group_Kind"] = "1";
                                    TotalCr = TotalCr + 1;
                                    TotalCrAmt = TotalCrAmt + TranAmount;
                                }

                                RowSelected["TxnCurr"] = "'00100";

                                RowSelected["Amount"] = TranAmount.ToString("###0.00");
                                RowSelected["Branch"] = "'0001";
                                RowSelected["ScreenNo"] = "'01408";
                                RowSelected["Flexacc"] = "'"+AccNo;
                                RowSelected["TABLENO"] = "20200610";

                                GIFU_GL_Account = AccNo2;

                            }
                            else
                            {
                                // THE FIRST OF PAIR IS FLEXCUBE 
                                RowSelected["Txn_Desc"] = TransDesc2;

                                if (TransType2 == 11 || TransType2 == 12)
                                {
                                    RowSelected["Group_Kind"] = "2";
                                    TotalDr = TotalDr + 1;
                                    TotalDrAmt = TotalDrAmt + TranAmount;
                                }
                                if (TransType2 == 21 || TransType2 == 22)
                                {
                                    RowSelected["Group_Kind"] = "1";
                                    TotalCr = TotalCr + 1;
                                    TotalCrAmt = TotalCrAmt + TranAmount;
                                }

                                RowSelected["TxnCurr"] = "'00100";

                                RowSelected["Amount"] = TranAmount.ToString("###0.00");
                                RowSelected["Branch"] = "'0001";
                                RowSelected["ScreenNo"] = "'01408";
                                RowSelected["Flexacc"] = "'"+AccNo2;
                                RowSelected["TABLENO"] = DateTime.Now.Date.ToString("yyyyMMdd");

                                GIFU_GL_Account = AccNo;
                            }

                            // ADD ROW
                            BDC_GIFU.Rows.Add(RowSelected);

                            //************************************************
                        }

                        // Close ReaderOpenRecord
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    if (TotalSelected > 0)
                    {
                        DataRow RowSelected2 = BDC_GIFU.NewRow();

                        // THE FIRST OF PAIR IS FLEXCUBE 
                        RowSelected2["Txn_Desc"] = "TOTAL FAILURE TXN ";

                        RowSelected2["Group_Kind"] = "1";
                        TotalDr = TotalDr + 1;
                        TotalDrAmt = TotalCrAmt;

                        RowSelected2["TxnCurr"] = "'00100";

                        RowSelected2["Amount"] = TotalCrAmt.ToString("###0.00");
                        RowSelected2["Branch"] = "'0001";
                        RowSelected2["ScreenNo"] = "'01060";
                        RowSelected2["Flexacc"] = "'"+GIFU_GL_Account;
                        RowSelected2["TABLENO"] = DateTime.Now.Date.ToString("yyyyMMdd");

                        // ADD ROW
                        BDC_GIFU.Rows.Add(RowSelected2);

                        InsertReport(InSignedId);
                    }

                    
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }

        // 
        // Create table for BDC_Settlement_Dpt   
        // ONLY THE ATMS

        public void ReadToBePostedAndFillTable_BDC_Settlement_Dpt(string InSignedId, int InRMCycle, DateTime InOpenDate, int InMode, string InSetlAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            RRDMBank_Branches Br = new RRDMBank_Branches();
            RRDMAccountsClass Acc = new RRDMAccountsClass();

            bool IsSelectByDate = false;

            if (InOpenDate != NullPastDate)
            {
                // Select By Date
                IsSelectByDate = true;
            }
            else
            {
                // Select By Repl 
                IsSelectByDate = false;
            }

            // CLEAR TABLE
            BDC_Settlement_Dpt = new DataTable();
            BDC_Settlement_Dpt.Clear();

            TotalDr = 0;
            TotalDrAmt = 0;
            TotalSelected = 0; 

            TotalCr = 0;
            TotalCrAmt = 0;

            // DATA TABLE ROWS DEFINITION 
            BDC_Settlement_Dpt.Columns.Add("MASK_PAN", typeof(string));
            BDC_Settlement_Dpt.Columns.Add("AMOUNT", typeof(string));
            BDC_Settlement_Dpt.Columns.Add("ORIGTRACE", typeof(string));

            BDC_Settlement_Dpt.Columns.Add("RESPCODE", typeof(string));

            BDC_Settlement_Dpt.Columns.Add("TERMID", typeof(string));
            BDC_Settlement_Dpt.Columns.Add("LOCAL_DATE", typeof(string));

            BDC_Settlement_Dpt.Columns.Add("REFNUM", typeof(string));

            BDC_Settlement_Dpt.Columns.Add("TXNDEST", typeof(string));
            BDC_Settlement_Dpt.Columns.Add("Type", typeof(string));
            //
            // GET THE _BDC_Settlement_Dpt
            //
            if (InMode == 3 & IsSelectByDate == true)
            {
                SqlString = "SELECT *"
                       + " FROM ATMS.[dbo].[TransToBePosted] "
                       + " WHERE (GLEntry = 1 AND GlEntry2 = 1) "
                       + " AND ((AccNo2 = '"+InSetlAccNo +"') "
                       + " OR (AccNo = '" + InSetlAccNo + "')) "
                       + " AND CAST(OpenDate As DATE) = @OpenDate ";
            }
            else
            {
                SqlString = "SELECT *"
                      + " FROM ATMS.[dbo].[TransToBePosted] "
                      + " WHERE (GLEntry = 1 AND GlEntry2 = 1) "
                      //+ " AND ((AccNo = '222517256' AccNo2 = '222588083') "
                      //+ " OR (AccNo = '222588083' AND AccNo2 = '222517256')) "
                      + " AND ((AccNo2 = '" + InSetlAccNo + "') "
                      + " OR (AccNo = '" + InSetlAccNo + "')) "
                      + " AND RMCycleNo = @RMCycleNo ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycle);
                        cmd.Parameters.AddWithValue("@OpenDate", InOpenDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //************************************************
                            //************************************************

                            TotalSelected = TotalSelected + 1;

                            // Read Txns fields
                            ReadFieldsTransToBePosted(rdr);

                            Mpa.ReadInPoolTransSpecificUniqueRecordId(UniqueRecordId, 2);

                            DataRow RowSelected = BDC_Settlement_Dpt.NewRow();

                            // 
                            TotalDr = TotalDr + 1 ;
                            TotalDrAmt = TotalDrAmt + TranAmount;

                            RowSelected["MASK_PAN"] = Mpa.CardNumber;

                            RowSelected["AMOUNT"] = TranAmount.ToString("###0");

                            RowSelected["ORIGTRACE"] = Mpa.TraceNoWithNoEndZero;
                            RowSelected["RESPCODE"] = "112";
                            RowSelected["TERMID"] = Mpa.TerminalId;
                            RowSelected["LOCAL_DATE"] = Mpa.TransDate.ToShortDateString();
                            RowSelected["REFNUM"] = Mpa.RRNumber;
                            RowSelected["TXNDEST"] = Mpa.TXNDEST;
                            RowSelected["Type"] = "WithDrawl";

                            // ADD ROW
                            BDC_Settlement_Dpt.Rows.Add(RowSelected);

                            //************************************************
                        }

                        // Close ReaderOpenRecord
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    if (TotalSelected>0)
                    {
                        DataRow RowSelected2 = BDC_Settlement_Dpt.NewRow();

                        RowSelected2["MASK_PAN"] = "TOTAL";

                        RowSelected2["AMOUNT"] = TotalDrAmt.ToString("#,##0");

                        RowSelected2["ORIGTRACE"] = "";
                        RowSelected2["RESPCODE"] = "";
                        RowSelected2["TERMID"] = "";
                        RowSelected2["LOCAL_DATE"] = "";
                        RowSelected2["TXNDEST"] = "";
                        RowSelected2["Type"] = "";

                        // ADD ROW
                        BDC_Settlement_Dpt.Rows.Add(RowSelected2);

                        InsertReport(InSignedId);
                    }
                    
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }


        // READ ALL TRANS TO BE POSTED  BASED ON FROM TO DATE   

        public void ReadAllTransToBePostedRange(string InSignedId, string InSelectionCriteria, DateTime InDtFrom, DateTime InDtTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TransToBePostedDataTable = new DataTable();
            TransToBePostedDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TransToBePostedDataTable.Columns.Add("PostedNo", typeof(int));
            TransToBePostedDataTable.Columns.Add("Select", typeof(bool));
            TransToBePostedDataTable.Columns.Add("Origin", typeof(string));
            TransToBePostedDataTable.Columns.Add("Done", typeof(string));

            TransToBePostedDataTable.Columns.Add("CurrDesc", typeof(string));
            TransToBePostedDataTable.Columns.Add("TranAmount", typeof(string));

            TransToBePostedDataTable.Columns.Add("Type", typeof(string));
            TransToBePostedDataTable.Columns.Add("TransDesc", typeof(string));
            TransToBePostedDataTable.Columns.Add("AccNo", typeof(string));
            TransToBePostedDataTable.Columns.Add("GlEntry", typeof(bool));

            TransToBePostedDataTable.Columns.Add("Type2", typeof(string));
            TransToBePostedDataTable.Columns.Add("TransDesc2", typeof(string));
            TransToBePostedDataTable.Columns.Add("AccNo2", typeof(string));
            TransToBePostedDataTable.Columns.Add("GlEntry2", typeof(bool));

            TransToBePostedDataTable.Columns.Add("AtmNo", typeof(string));
            TransToBePostedDataTable.Columns.Add("ReplCycle", typeof(int));


            SqlString = "SELECT *"
                       + " FROM [dbo].[TransToBePosted] "
                       + " WHERE " + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@WDtFrom", InDtFrom.Date);
                        cmd.Parameters.AddWithValue("@WDtTo", InDtTo.Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //************************************************

                            TotalSelected = TotalSelected + 1;

                            // Read Txns fields
                            ReadFieldsTransToBePosted(rdr);

                            //OpenRecord = (bool)rdr["OpenRecord"];

                            DataRow RowSelected = TransToBePostedDataTable.NewRow();

                            RowSelected["PostedNo"] = PostedNo;

                            RowSelected["Select"] = false;

                            RowSelected["Origin"] = OriginName;
                            if (OpenRecord == true)
                            {
                                RowSelected["Done"] = "NO";
                            }
                            else
                            {
                                RowSelected["Done"] = "YES";
                            }

                            RowSelected["CurrDesc"] = CurrDesc;

                            //decimal Temp = (decimal)rdr["TranAmount"] ;
                            //string Temp2 = Temp.ToString("#,##0.00");
                            RowSelected["TranAmount"] = TranAmount.ToString("#,##0.00");
                            TransType = TransType; // 11 for debit 21 for credit

                            if (TransType == 11 || TransType == 12)
                            {
                                RowSelected["Type"] = "DR";
                            }
                            if (TransType == 21 || TransType == 22)
                            {
                                RowSelected["Type"] = "CR";
                            }

                            RowSelected["TransDesc"] = TransDesc;
                            RowSelected["AccNo"] = AccNo;
                            RowSelected["GlEntry"] = GlEntry;

                            TransType2 = TransType2; // 11 for debit 21 for credit

                            if (TransType2 == 11 || TransType2 == 12)
                            {
                                RowSelected["Type2"] = "DR";
                            }
                            if (TransType2 == 21 || TransType2 == 22)
                            {
                                RowSelected["Type2"] = "CR";
                            }

                            RowSelected["TransDesc2"] = TransDesc2;
                            RowSelected["AccNo2"] = AccNo2;
                            RowSelected["GlEntry2"] = GlEntry2;

                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["ReplCycle"] = SesNo;

                            // ADD ROW
                            TransToBePostedDataTable.Rows.Add(RowSelected);

                            //************************************************
                        }

                        // Close ReaderOpenRecord
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertReport(InSignedId);
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }

        // Insert 
        private void InsertReport(string InSignedId)
        {

            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport61(InSignedId);

            int I = 0;

            while (I <= (TransToBePostedDataTable.Rows.Count - 1))
            {

                Tr.PostedNo = (int)TransToBePostedDataTable.Rows[I]["PostedNo"];
                Tr.Origin = (string)TransToBePostedDataTable.Rows[I]["Origin"];
                Tr.Done = (string)TransToBePostedDataTable.Rows[I]["Done"];

                Tr.CurrDesc = (string)TransToBePostedDataTable.Rows[I]["CurrDesc"];
                Tr.TranAmount = (string)TransToBePostedDataTable.Rows[I]["TranAmount"];

                Tr.DrCrType = (string)TransToBePostedDataTable.Rows[I]["Type"];
                Tr.TransDesc = (string)TransToBePostedDataTable.Rows[I]["TransDesc"];
                Tr.AccNo = (string)TransToBePostedDataTable.Rows[I]["AccNo"];
                Tr.GlEntry = (bool)TransToBePostedDataTable.Rows[I]["GlEntry"];

                Tr.DrCrType2 = (string)TransToBePostedDataTable.Rows[I]["Type2"];
                Tr.TransDesc2 = (string)TransToBePostedDataTable.Rows[I]["TransDesc2"];
                Tr.AccNo2 = (string)TransToBePostedDataTable.Rows[I]["AccNo2"];
                Tr.GlEntry2 = (bool)TransToBePostedDataTable.Rows[I]["GlEntry2"];

                Tr.AtmNo = (string)TransToBePostedDataTable.Rows[I]["AtmNo"];
                Tr.ReplCycle = (int)TransToBePostedDataTable.Rows[I]["ReplCycle"];

                // Insert record for printing 
                //
                Tr.InsertReport61(InSignedId);

                I++; // Read Next entry of the table 

            }
        }

        // READ TRANS TO BE POSTED  BASED ON ERROR NUMBER AND CARD NUMBER 

        public void ReadTransToBePosted(int InErrNo, string InCardNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE ErrNo = @ErrNo AND CardNo =@CardNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);
                        cmd.Parameters.AddWithValue("@CardNo", InCardNo);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ReadFieldsTransToBePosted(rdr);
                        }

                        // Close ReaderOpenRecord
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


        // READ TRANS TO BE POSTED  BASED ON ATMNo , TraceNo,  

        public void ReadTransToBePostedTraceSequence(string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE AtmNo = @AtmNo AND AtmTraceNo = @AtmTraceNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", InTraceNo);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsTransToBePosted(rdr);
                        }

                        // Close ReaderOpenRecord
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

        // READ TRANS TO BE POSTED  BASED ON IDentity  

        public void ReadTransToBePostedSpecific(int InPostedNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                 + " FROM [dbo].[TransToBePosted] "
                 + " WHERE PostedNo = @PostedNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@PostedNo", InPostedNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ReadFieldsTransToBePosted(rdr);
                        }

                        // Close ReaderOpenRecord
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

        // READ TRANS TO BE POSTED  BASED ON ATM Trace Number 

        public void ReadTransToBePostedSpecificTraceNo(string InAtmNo, int InAtmTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE AtmNo = @AtmNo AND AtmTraceNo = @AtmTraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", InAtmTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ReadFieldsTransToBePosted(rdr);
                        }

                        // Close ReaderOpenRecord
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
        // READ TRANS TO BE POSTED  BASED ON THE UNIQUE NUMBER  
        //
        public void ReadTransToBePostedSpecificByUniqueRecordId(int InUniqueRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
           + " FROM [dbo].[TransToBePosted] "
           + " WHERE UniqueRecordId = @UniqueRecordId AND IsReversal = 0 ";
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

                            ReadFieldsTransToBePosted(rdr);
                        }

                        // Close ReaderOpenRecord
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
        // READ TRANS TO BE POSTED  BASED ON THE UNIQUE NUMBER  
        //
        public void ReadTransToBePostedSpecificByUniqueRecordIdForReversal(int InUniqueRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            IsReversal = false;

            SqlString = "SELECT *"
           + " FROM [dbo].[TransToBePosted] "
           + " WHERE UniqueRecordId = @UniqueRecordId AND IsReversal = 1 ";
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

                            ReadFieldsTransToBePosted(rdr);
                        }

                        // Close ReaderOpenRecord
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


        // READ OPEN TRANS TO BE POSTED  
        // Read all Sequentially
        // For each record create two transactions in PostedTrans table 
        // Denote trans with action taken 

        public void ReadTransToBePostedAllAndCreatePostedTrans(string InSignedId, string InOperator, int InActionCd2, string InOriginName, string InForm)
        {

            RecordFound = false;

            TotPairTransactions = 0;

            RRDMPostedTrans Pt = new RRDMPostedTrans();

            TransToBePostedDataTable = new DataTable();
            TransToBePostedDataTable.Clear();

            if (InForm == "Form271")
            {
                SqlString = "SELECT *"
                  + " FROM [dbo].[TransToBePosted] "
                  + " WHERE Operator = @Operator AND OriginName = @OriginName AND OpenRecord = 1 ";
            }
            if (InForm == "Form78")
            {
                SqlString = "SELECT *"
                  + " FROM [dbo].[TransToBePosted] "
                  + " WHERE Operator = @Operator AND OpenRecord = 1 ";
            }


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        if (InForm == "Form271")
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@OriginName", InOriginName);
                        }

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TransToBePostedDataTable);

                        // Close conn
                        conn.Close();


                        int I = 0;

                        while (I <= (TransToBePostedDataTable.Rows.Count - 1))
                        {

                            RecordFound = true;

                            TotPairTransactions = TotPairTransactions + 1;

                            // GET Table fields - Line by Line
                            //
                            PostedNo = (int)TransToBePostedDataTable.Rows[I]["PostedNo"];
                            OriginId = (string)TransToBePostedDataTable.Rows[I]["OriginId"];
                            OriginName = (string)TransToBePostedDataTable.Rows[I]["OriginName"];

                            RMCateg = (string)TransToBePostedDataTable.Rows[I]["RMCateg"];

                            RMCategCycle = (int)TransToBePostedDataTable.Rows[I]["RMCategCycle"];
                            UniqueRecordId = (int)TransToBePostedDataTable.Rows[I]["UniqueRecordId"];
                            ErrNo = (int)TransToBePostedDataTable.Rows[I]["ErrNo"];
                            AtmNo = (string)TransToBePostedDataTable.Rows[I]["AtmNo"];

                            SesNo = (int)TransToBePostedDataTable.Rows[I]["SesNo"];
                            DisputeNo = (int)TransToBePostedDataTable.Rows[I]["DisputeNo"];
                            DispTranNo = (int)TransToBePostedDataTable.Rows[I]["DispTranNo"];
                            BankId = (string)TransToBePostedDataTable.Rows[I]["BankId"];

                            AtmTraceNo = (int)TransToBePostedDataTable.Rows[I]["AtmTraceNo"];
                            BranchId = (string)TransToBePostedDataTable.Rows[I]["BranchId"];
                            AtmDtTime = (DateTime)TransToBePostedDataTable.Rows[I]["AtmDtTime"];
                            SystemTarget = (int)TransToBePostedDataTable.Rows[I]["SystemTarget"];

                            CardNo = (string)TransToBePostedDataTable.Rows[I]["CardNo"];
                            CardOrigin = (int)TransToBePostedDataTable.Rows[I]["CardOrigin"];
                            // First Transaction
                            TransType = (int)TransToBePostedDataTable.Rows[I]["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)TransToBePostedDataTable.Rows[I]["TransDesc"];

                            AccNo = (string)TransToBePostedDataTable.Rows[I]["AccNo"];
                            GlEntry = (bool)TransToBePostedDataTable.Rows[I]["GlEntry"];
                            // Second Transaction
                            TransType2 = (int)TransToBePostedDataTable.Rows[I]["TransType2"]; // 11 for debit 21 for credit
                            TransDesc2 = (string)TransToBePostedDataTable.Rows[I]["TransDesc2"];

                            AccNo2 = (string)TransToBePostedDataTable.Rows[I]["AccNo2"];
                            GlEntry2 = (bool)TransToBePostedDataTable.Rows[I]["GlEntry2"];
                            // End of second 
                            CurrDesc = (string)TransToBePostedDataTable.Rows[I]["CurrDesc"];
                            TranAmount = (decimal)TransToBePostedDataTable.Rows[I]["TranAmount"];

                            AuthCode = (int)TransToBePostedDataTable.Rows[I]["AuthCode"];
                            RefNumb = (int)TransToBePostedDataTable.Rows[I]["RefNumb"];
                            RemNo = (int)TransToBePostedDataTable.Rows[I]["RemNo"];
                            TransMsg = (string)TransToBePostedDataTable.Rows[I]["TransMsg"];

                            AtmMsg = (string)TransToBePostedDataTable.Rows[I]["AtmMsg"];

                            MakerUser = (string)TransToBePostedDataTable.Rows[I]["MakerUser"];

                            AuthUser = (string)TransToBePostedDataTable.Rows[I]["AuthUser"];
                            ActionBy = (string)TransToBePostedDataTable.Rows[I]["ActionBy"];
                            ActionCd2 = (int)TransToBePostedDataTable.Rows[I]["ActionCd2"];

                            ActionDate = (DateTime)TransToBePostedDataTable.Rows[I]["ActionDate"];
                            OpenDate = (DateTime)TransToBePostedDataTable.Rows[I]["OpenDate"];
                            OpenRecord = (bool)TransToBePostedDataTable.Rows[I]["OpenRecord"];
                            HostMatched = (bool)TransToBePostedDataTable.Rows[I]["HostMatched"];

                            MatchedDtTm = (DateTime)TransToBePostedDataTable.Rows[I]["MatchedDtTm"];
                            Reconciled = (int)TransToBePostedDataTable.Rows[I]["Reconciled"];
                            GridFilterDate = (DateTime)TransToBePostedDataTable.Rows[I]["GridFilterDate"];
                            Operator = (string)TransToBePostedDataTable.Rows[I]["Operator"];

                            //
                            // Insert a pair of transactions in Posted Trans

                            Pt.UpdateAsClosedTheAlreadyInTable(PostedNo);

                            //
                            Pt.TranToBePostedKey = PostedNo;
                            Pt.Origin = OriginName;
                            Pt.UserId = MakerUser;
                            Pt.AccNo = AccNo;
                            Pt.AtmNo = AtmNo;
                            Pt.ReplCycle = SesNo;
                            Pt.BankId = BankId;

                            Pt.TranDtTime = DateTime.Now;
                            Pt.TransType = TransType;
                            Pt.TransDesc = TransDesc;
                            //TEST
                            Pt.CurrDesc = CurrDesc;
                            Pt.TranAmount = TranAmount;
                            Pt.ValueDate = DateTime.Now;
                            Pt.OpenRecord = true;

                            Pt.Operator = Operator;

                            Pt.InsertTran(PostedNo, Pt.Origin);

                            // Posted Second transaction 
                            //

                            Pt.TranToBePostedKey = PostedNo;
                            Pt.Origin = OriginName;
                            Pt.UserId = MakerUser;
                            Pt.AccNo = AccNo2;
                            Pt.AtmNo = AtmNo;
                            Pt.ReplCycle = SesNo;
                            Pt.BankId = BankId;

                            Pt.TranDtTime = DateTime.Now;
                            Pt.TransType = TransType2;
                            Pt.TransDesc = TransDesc2;
                            //TEST
                            Pt.CurrDesc = CurrDesc;
                            Pt.TranAmount = TranAmount;
                            Pt.ValueDate = DateTime.Now;
                            Pt.OpenRecord = true;

                            Pt.Operator = Operator;

                            Pt.InsertTran(PostedNo, Pt.Origin);


                            // UPDATE Transtobe posted and close 

                            ActionDate = DateTime.Now;

                            GridFilterDate = DateTime.Today.Date; // DATE For Grid filter 

                            // From Trans To be posted Form 

                            if (InActionCd2 == 4)
                            {
                                // Close Transaction to be posted 

                                ActionCd2 = 4;

                                ActionDate = DateTime.Now;

                                GridFilterDate = DateTime.Today.Date; // DATE For Grid filter 

                                OpenRecord = false; // Close Record 

                                UpdateTransToBePostedAction1(PostedNo, InSignedId, InActionCd2);
                            }
                            else
                            {
                                // Do not update 
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
        ////
        //// READ SPECIFIC TRANSACTION FROM IN POOL FOR Disputes  
        ////

        //public void ReadInPoolTransSpecificForDisputesTable(int InTranNo)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    DisputedTransDataTable = new DataTable();
        //    DisputedTransDataTable.Clear();
        //    TotalSelected = 0;

        //    // DATA TABLE ROWS DEFINITION 

        //    DisputedTransDataTable.Columns.Add("Chosen", typeof(bool));
        //    DisputedTransDataTable.Columns.Add("DisputedAmnt", typeof(decimal));
        //    DisputedTransDataTable.Columns.Add("Card", typeof(string));
        //    DisputedTransDataTable.Columns.Add("Account", typeof(string));
        //    DisputedTransDataTable.Columns.Add("Curr", typeof(string));
        //    DisputedTransDataTable.Columns.Add("Amount", typeof(string));
        //    DisputedTransDataTable.Columns.Add("TransDate", typeof(DateTime));
        //    DisputedTransDataTable.Columns.Add("TransDescr", typeof(string));
        //    DisputedTransDataTable.Columns.Add("UniqueRecordId", typeof(int));
        //    DisputedTransDataTable.Columns.Add("RMCategory", typeof(string));
        //    DisputedTransDataTable.Columns.Add("MatchingDt", typeof(string));


        //    SqlString = "SELECT *"
        //  + " FROM [ATMS].[dbo].[InPoolTrans] "
        //  + " WHERE TranNo = @TranNo";
        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@TranNo", InTranNo);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    TotalSelected = TotalSelected + 1;

        //                    ReadFieldsInPoolTxn(rdr);

        //                    // Fill Table 

        //                    DataRow RowSelected = DisputedTransDataTable.NewRow();

        //                    RowSelected["Chosen"] = false;
        //                    RowSelected["DisputedAmnt"] = TranAmount;

        //                    RowSelected["Card"] = CardNo;
        //                    RowSelected["Account"] = AccNo;
        //                    RowSelected["Curr"] = CurrDesc;
        //                    RowSelected["Amount"] = TranAmount.ToString("#,##0.00");
        //                    RowSelected["TransDate"] = AtmDtTime;
        //                    RowSelected["TransDescr"] = TransDesc;
        //                    RowSelected["UniqueRecordId"] = UniqueRecordId;
        //                    RowSelected["RMCategory"] = RMCateg;
        //                    RowSelected["MatchingDt"] = AtmDtTime.ToString();

        //                    // ADD ROW
        //                    DisputedTransDataTable.Rows.Add(RowSelected);


        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            CatchDetails(ex);
        //        }
        //}
        // Read Fields In Pool TXNs 
        private void ReadFieldsInPoolTxn(SqlDataReader rdr)
        {
            TranNo = (int)rdr["TranNo"];
            OriginName = (string)rdr["OriginName"];
            RMCateg = (string)rdr["RMCateg"];
            AtmTraceNo = (int)rdr["AtmTraceNo"];
            MasterTraceNo = (int)rdr["MasterTraceNo"];
            EJournalTraceNo = (int)rdr["EJournalTraceNo"];

            AtmNo = (string)rdr["AtmNo"];
            SesNo = (int)rdr["SesNo"];
            BankId = (string)rdr["BankId"];

            BranchId = (string)rdr["BranchId"];

            AtmDtTime = (DateTime)rdr["AtmDtTime"];
            UniqueRecordId = (int)rdr["UniqueRecordId"];

            SystemTarget = (int)rdr["SystemTarget"];

            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
            TransDesc = (string)rdr["TransDesc"];
            CardNo = (string)rdr["CardNo"];

            CardOrigin = (int)rdr["CardOrigin"];
            AccNo = (string)rdr["AccNo"];

            CurrDesc = (string)rdr["CurrDesc"];

            TranAmount = (decimal)rdr["TranAmount"];

            AuthCode = (int)rdr["AuthCode"];
            RefNumb = (int)rdr["RefNumb"];
            RemNo = (int)rdr["RemNo"];

            TransMsg = (string)rdr["TransMsg"];
            AtmMsg = (string)rdr["AtmMsg"];

            ErrNo = (int)rdr["ErrNo"];

            StartTrxn = (int)rdr["StartTrxn"];
            EndTrxn = (int)rdr["EndTrxn"];

            DepCount = (decimal)rdr["DepCount"];

            CommissionCode = (int)rdr["CommissionCode"];
            CommissionAmount = (decimal)rdr["CommissionAmount"];

            SuccTran = (bool)rdr["SuccTran"];

            Operator = (string)rdr["Operator"];
        }

        ////
        //// READ SPECIFIC TRANSACTION FROM IN POOL based on Number 
        ////
        //public void ReadInPoolTransDataTable(int InMode, string InSelectionCriteria, DateTime InDate, DateTime InDtFrom, DateTime InDtTo)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    // InMode = 1 then selection criteria and no dates
        //    // InMode = 2 then single date exist 
        //    // InMode = 3 then range of dates exist 

        //    InPoolTransactionsDataTable = new DataTable();
        //    InPoolTransactionsDataTable.Clear();

        //    TotalSelected = 0;

        //    // DATA TABLE ROWS DEFINITION 
        //    InPoolTransactionsDataTable.Columns.Add("UniqueRecordId", typeof(int));
        //    InPoolTransactionsDataTable.Columns.Add("AtmTraceNo", typeof(string));
        //    InPoolTransactionsDataTable.Columns.Add("AtmDtTime", typeof(string));

        //    InPoolTransactionsDataTable.Columns.Add("CardNo", typeof(string));
        //    InPoolTransactionsDataTable.Columns.Add("Type", typeof(string));
        //    InPoolTransactionsDataTable.Columns.Add("TransDesc", typeof(string));

        //    InPoolTransactionsDataTable.Columns.Add("TranAmount", typeof(string));
        //    InPoolTransactionsDataTable.Columns.Add("Ccy", typeof(string));
        //    InPoolTransactionsDataTable.Columns.Add("OriginName", typeof(string));

        //    InPoolTransactionsDataTable.Columns.Add("AtmNo", typeof(string));
        //    InPoolTransactionsDataTable.Columns.Add("Repl Cycle", typeof(string));
        //    InPoolTransactionsDataTable.Columns.Add("BranchId", typeof(string));

        //    InPoolTransactionsDataTable.Columns.Add("SystemTarget", typeof(string));


        //    SqlString = "SELECT *"
        //                + " FROM [ATMS].[dbo].[InPoolTrans] "
        //               + " WHERE " + InSelectionCriteria;

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                if (InMode == 1)
        //                {
        //                    cmd.Parameters.AddWithValue("@AtmDtTime", InDate);
        //                }
        //                if (InMode == 2)
        //                {
        //                    cmd.Parameters.AddWithValue("@DtFrom", InDtFrom);
        //                    cmd.Parameters.AddWithValue("@DtTo", InDtTo);
        //                }

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;
        //                    ReadFieldsInPoolTxn(rdr);
        //                    //
        //                    //Fill In Table
        //                    //
        //                    DataRow RowSelected = InPoolTransactionsDataTable.NewRow();

        //                    RowSelected["UniqueRecordId"] = UniqueRecordId;
        //                    RowSelected["AtmTraceNo"] = AtmTraceNo.ToString();
        //                    RowSelected["AtmDtTime"] = AtmDtTime.ToString();

        //                    RowSelected["CardNo"] = CardNo;

        //                    if (TransType > 10 & TransType < 20)
        //                    {
        //                        RowSelected["Type"] = "DR";
        //                    }
        //                    if (TransType > 20 & TransType < 30)
        //                    {
        //                        RowSelected["Type"] = "CR";
        //                    }
        //                    RowSelected["TransDesc"] = TransDesc;
        //                    RowSelected["TranAmount"] = TranAmount.ToString("#,##0.00");
        //                    RowSelected["Ccy"] = CurrDesc;
        //                    RowSelected["OriginName"] = OriginName;

        //                    RowSelected["AtmNo"] = AtmNo;
        //                    RowSelected["Repl Cycle"] = SesNo.ToString();
        //                    RowSelected["BranchId"] = BranchId;

        //                    RowSelected["SystemTarget"] = SystemTarget.ToString();

        //                    // ADD ROW
        //                    InPoolTransactionsDataTable.Rows.Add(RowSelected);

        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            CatchDetails(ex);
        //        }
        //}


        ////
        //// READ SPECIFIC TRANSACTION FROM IN POOL based on Number 
        ////
        //public void ReadInPoolTransSpecific(int InUniqueRecordId)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    SqlString = "SELECT *"
        //  + " FROM [ATMS].[dbo].[InPoolTrans] "
        //  + " WHERE UniqueRecordId = @UniqueRecordId";
        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;
        //                    ReadFieldsInPoolTxn(rdr);
        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            CatchDetails(ex);
        //        }
        //}
        //
        // READ SPECIFIC TRANSACTION FROM From Pool And Insert InMasterPoolATMS
        //
        string SqlString;
        public void ReadInPoolTransFromPoolAndInsertInMasterPoolATMS(int InMode)
        {
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            RRDMMatchingCategories Mc = new RRDMMatchingCategories();

            RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

            // InMode = 11 means we read only cash withdrawls
            // InMode = 23 means we read only deposits 23, 24, 25 

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 11)
            {
                SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[InPoolTrans] "
                    + " WHERE TransType = 11 AND (AtmNo = 'AB102' OR AtmNo = 'AB104') ";
            }
            if (InMode == 23)
            {
                SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[InPoolTrans] "
                    + " WHERE (TransType = 23 OR TransType = 24 OR TransType = 25) AND (AtmNo = 'AB102' OR AtmNo = 'AB104') ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@TranNo", InTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];
                            OriginName = (string)rdr["OriginName"];
                            RMCateg = (string)rdr["RMCateg"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            MasterTraceNo = (int)rdr["MasterTraceNo"];
                            EJournalTraceNo = (int)rdr["EJournalTraceNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            CardNo = (string)rdr["CardNo"];

                            CardOrigin = (int)rdr["CardOrigin"];
                            AccNo = (string)rdr["AccNo"];

                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ErrNo = (int)rdr["ErrNo"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DepCount = (decimal)rdr["DepCount"];

                            CommissionCode = (int)rdr["CommissionCode"];
                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            SuccTran = (bool)rdr["SuccTran"];

                            Operator = (string)rdr["Operator"];

                            Ac.ReadAtm(AtmNo);

                            Mpa.OriginFileName = "ATM:" + AtmNo + " Journal";
                            Mpa.OriginalRecordId = TranNo;
                            Mpa.UniqueRecordId = UniqueRecordId;

                            //451174******6838    
                            if (CardNo.Substring(0, 6) == "451174")
                            {
                                Mpa.MatchingCateg = "EWB102";
                                Mpa.RMCateg = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                Mc.ReadMatchingCategorybyActiveCategId(Operator, Mpa.MatchingCateg);
                                Mpa.Origin = Mc.Origin;
                                Mpa.TransTypeAtOrigin = Mc.TransTypeAtOrigin;
                                Mpa.Product = Mc.Product;
                                Mpa.CostCentre = Mc.CostCentre;
                            }
                            else
                            {
                                Mpa.MatchingCateg = "EWB103";
                                Mpa.RMCateg = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                Mc.ReadMatchingCategorybyActiveCategId(Operator, Mpa.MatchingCateg);
                                Mpa.Origin = Mc.Origin;
                                Mpa.TransTypeAtOrigin = Mc.TransTypeAtOrigin;
                                Mpa.Product = Mc.Product;
                                Mpa.CostCentre = Mc.CostCentre;

                            }

                            Mpa.TargetSystem = SystemTarget;
                            Mpa.LoadedAtRMCycle = 205;
                            Mpa.MatchingAtRMCycle = 205; // This is the Daily Running Job Cycle 

                            Mpa.TerminalId = (string)rdr["AtmNo"];

                            Mpa.TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            Mpa.TransDescr = (string)rdr["TransDesc"];

                            Mpa.CardNumber = (string)rdr["CardNo"];
                            Mpa.AccNumber = (string)rdr["AccNo"];

                            Mpa.TransCurr = (string)rdr["CurrDesc"];
                            Mpa.TransAmount = (decimal)rdr["TranAmount"];
                            Mpa.DepCount = (decimal)rdr["DepCount"];

                            Mpa.TransDate = (DateTime)rdr["AtmDtTime"];

                            Mpa.AtmTraceNo = (int)rdr["AtmTraceNo"];

                            Mpa.MasterTraceNo = (int)rdr["AtmTraceNo"];

                            if (ErrNo > 0)
                            {
                                Er.ReadErrorsTableSpecific(ErrNo);
                                Mpa.MetaExceptionId = Er.ErrId;
                            }
                            else
                            {
                                Mpa.MetaExceptionId = 0;
                            }

                            Mpa.MetaExceptionNo = ErrNo;

                            Mpa.RRNumber = "0";

                            Mpa.ResponseCode = "";

                            Mpa.Operator = (string)rdr["Operator"];

                            Mpa.ReplCycleNo = SesNo;

                            if (Mpa.MetaExceptionNo > 0)
                            {
                                Mpa.CardNumber = "4375071234567892";
                                Mpa.Matched = true;
                                Mpa.MatchMask = "111";
                                Mpa.FileId01 = "ATM - Ej Transactions";
                                Mpa.FileId02 = "Switch File";
                                Mpa.FileId03 = "T-24 File";
                            }
                            else
                            {
                                Mpa.Matched = false;
                                Mpa.MatchMask = "";
                                Mpa.FileId01 = "";
                                Mpa.FileId02 = "";
                                Mpa.FileId03 = "";
                            }

                            Mpa.Comments = "";

                            string SelectionCriteria = " Where UniqueRecordId =" + UniqueRecordId;
                            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
                            if (Mpa.RecordFound == false)
                            {
                                Mpa.InsertTransMasterPoolATMs(Operator);
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Found same " + UniqueRecordId);
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
        // READ SPECIFIC TRANSACTION FROM IN POOL ATM Based on Trace No 
        //
        public void ReadInPoolAtmTrace(string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                       + " FROM [ATMS].[dbo].[InPoolTrans] "
                       + " WHERE AtmNo = @AtmNo AND AtmTraceNo = @AtmTraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ReadFieldsInPoolTxn(rdr);
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
        // READ SPECIFIC TRANSACTION FROM IN POOL Host Based on Trace No 
        //
        public void ReadInPoolHostTrace(string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolHost] "
          + " WHERE AtmNoH = @AtmNoH AND AtmTraceNoH = @AtmTraceNoH";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmTraceNoH", InTraceNo);
                        cmd.Parameters.AddWithValue("@AtmNoH", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TranNoH = (int)rdr["TranNoH"];
                            TranOriginH = (int)rdr["TranOriginH"];
                            AtmTraceNoH = (int)rdr["AtmTraceNoH"];
                            HostTraceNoH = (int)rdr["HostTraceNoH"];

                            HostDtTimeH = (DateTime)rdr["HostDtTimeH"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransDescH = (string)rdr["TransDescH"];
                            CardNoH = (string)rdr["CardNoH"];

                            AccNoH = (string)rdr["AccNoH"];

                            AuthCodeH = (int)rdr["AuthCodeH"];
                            RefNumbH = (int)rdr["RefNumbH"];
                            RemNoH = (int)rdr["RemNoH"];

                            TransMsgH = (string)rdr["TransMsgH"];

                            ErrNoH = (int)rdr["ErrNoH"];

                            SuccTranH = (bool)rdr["SuccTranH"];

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
        // UPDATE Transaction for Deposit 
        // 
        public void UpdateTransforDep(int InTranNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[InPoolTrans] SET "
                            + " AtmMsg = @AtmMsg,DepCount = @DepCount"
                            + " WHERE TranNo = @TranNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", TranNo);
                        cmd.Parameters.AddWithValue("@AtmMsg", AtmMsg);
                        cmd.Parameters.AddWithValue("@DepCount", DepCount);

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
        // READ TRANSACTIONs For creating the Dispensed History Records  
        //
        public void ReadUpdateTransForDispensedHistory(string InOperator, string InAtmNo, DateTime FromDate, DateTime ToDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int DrTrans = 0;
            decimal DispensedAmt = 0;
            int CrTrans = 0;
            decimal DepAmt = 0;
            bool First = true;

            int[] NumberOfTrans = new int[999];
            decimal[] AmountPerType = new decimal[999];

            DateTime PreviousDt = new DateTime(1900, 01, 01);

            InPoolTransactionsDataTable = new DataTable();
            InPoolTransactionsDataTable.Clear();

            TotalSelected = 0;

            SqlString = "SELECT *"
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                 + " WHERE AtmNo = @AtmNo AND TransDate>= @FromDate AND  TransDate<= @ToDate";

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDate", FromDate);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ToDate", ToDate);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(InPoolTransactionsDataTable);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (InPoolTransactionsDataTable.Rows.Count - 1))
                        {

                            RecordFound = true;

                            // GET Table needed fields - Line by Line
                            //

                            AtmDtTime = (DateTime)InPoolTransactionsDataTable.Rows[I]["TransDate"];
                            TransType = (int)InPoolTransactionsDataTable.Rows[I]["TransType"];

                            CurrDesc = (string)InPoolTransactionsDataTable.Rows[I]["TransCurr"];
                            TranAmount = (decimal)InPoolTransactionsDataTable.Rows[I]["TransAmount"];
                            //CommissionCode = (int)InPoolTransactionsDataTable.Rows[I]["CommissionCode"];
                            //CommissionAmount = (decimal)InPoolTransactionsDataTable.Rows[I]["CommissionAmount"];
                            CommissionCode = 0;
                            CommissionAmount = 0;

                            if (First == true)
                            {
                                PreviousDt = AtmDtTime;
                                First = false;
                            }

                            if (CommissionCode > 0)
                            {
                                // Create totals for commission records
                                NumberOfTrans[CommissionCode] = NumberOfTrans[CommissionCode] + 1;
                                AmountPerType[CommissionCode] = AmountPerType[CommissionCode] + CommissionAmount;
                            }

                            if (TransType == 11)
                            {
                                RecordFound = true;
                                DrTrans = DrTrans + 1;
                                DispensedAmt = DispensedAmt + TranAmount;
                                // Create totals for turnover 
                                NumberOfTrans[TransType] = NumberOfTrans[TransType] + 1;
                                AmountPerType[TransType] = AmountPerType[TransType] + TranAmount;
                            }

                            if (TransType == 23 || TransType == 24 || TransType == 25)
                            {
                                CrTrans = CrTrans + 1;
                                DepAmt = DepAmt + TranAmount;
                                NumberOfTrans[TransType] = NumberOfTrans[TransType] + 1;
                                AmountPerType[TransType] = AmountPerType[TransType] + TranAmount;
                            }

                            if (AtmDtTime.Date != PreviousDt.Date & DispensedAmt > 0)
                            {
                                // Insert Dispensed History transaction 

                                Am.ReadAtmsMainSpecific(InAtmNo);// READ MAIN 

                                Ah.AtmNo = InAtmNo;
                                Ah.BankId = Am.BankId;
                                Ah.Dt = PreviousDt.Date;
                                Ah.LoadedAtRMCycle = PreviousDt.Year;

                                Ah.DrTransactions = DrTrans;
                                Ah.DispensedAmt = DispensedAmt;
                                Ah.PreEstimated = 0;
                                Ah.CrTransactions = CrTrans;
                                Ah.DepAmount = DepAmt;

                                // Update Array values for other cost parameters 

                                // Annual Maintenance divided daily 
                                Ap.ReadTableATMsCostSpecific(InAtmNo);

                                decimal YearMaintenance = Ap.AnnualMaint;
                                Ho.GetDaysInAYear(DateTime.Now.Year);
                                Ah.C301DailyMaintAmount = YearMaintenance / Ho.daysInYear;

                                NumberOfTrans[301] = 1;
                                AmountPerType[301] = Ah.C301DailyMaintAmount;

                                // Annual Cost of Overhead Divided Daily 

                                ParamId = "370";
                                OccuranceId = "325"; // find annual 
                                RelatedParmId = "";
                                RelatedOccuranceId = "";

                                Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);
                                //      Ds.ReadParametersSpecificNo(325); // find annual 
                                Ah.C307OverheadCost = Gp.Amount / Ho.daysInYear;

                                NumberOfTrans[307] = 1;
                                AmountPerType[307] = Ah.C307OverheadCost;

                                // Cost of investement Divided Daily 
                                Gp.ReadParametersSpecificId(InOperator, "500", "1", "", "");
                                int LastingYears = (int)Gp.Amount;

                                Ah.C309CostOfInvest = Ap.PurchaseCost / (LastingYears * 365);

                                NumberOfTrans[309] = 1;
                                AmountPerType[309] = Ah.C309CostOfInvest;


                                // Replenishment cost 

                                Rs.ReadReplStatClassSpecificDt(Ah.AtmNo, Ah.Dt); // Get Total Repl Time and other fields 

                                if (Rs.RecordFound == true)
                                {
                                    NumberOfTrans[303] = 1;

                                    //      Ds.ReadParametersSpecificNo(324); // find employee cost per minute 


                                    ParamId = "370";
                                    OccuranceId = "324";
                                    RelatedParmId = "";
                                    RelatedOccuranceId = "";

                                    // find employee cost per minute 

                                    Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                                    AmountPerType[303] = Rs.TotalReplMinutes * Gp.Amount;

                                    Ah.C303ReplTimeCost = Rs.TotalReplMinutes * Gp.Amount;

                                    // Cost of Money 
                                    // Find Repl No and Money in 
                                    // Find days for money in 
                                    // Find cost of money based on days

                                    Ta.ReadSessionsStatusTraces(Ah.AtmNo, Rs.ReplCycleNo);

                                    TimeSpan Diff = Ta.SesDtTimeEnd - Ta.SesDtTimeStart;
                                    string NumberDays1 = Diff.TotalHours.ToString();
                                    int NumberHours = Convert.ToInt32(Diff.TotalHours);

                                    // Ds.ReadParametersSpecificNo(322); // find cost of money

                                    ParamId = "370";
                                    OccuranceId = "322";
                                    RelatedParmId = "";
                                    RelatedOccuranceId = "";

                                    // find cost of money

                                    Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                                    NumberOfTrans[308] = NumberHours / 24;

                                    AmountPerType[308] = Rs.InMoneyLast * NumberHours * Gp.Amount / (Ho.daysInYear * 24);

                                    Ah.C308CostOfMoney = Rs.InMoneyLast * NumberHours * Gp.Amount / (Ho.daysInYear * 24);
                                }



                                /* Create Stats records  */
                                for (int i = 0; i < 999; i++)
                                {
                                    if (NumberOfTrans[i] > 0 || AmountPerType[i] > 0)
                                    {
                                        if (i == 401)
                                        {
                                            Ah.R401CommTran = NumberOfTrans[i];
                                            Ah.R401CommAmount = AmountPerType[i];
                                        }

                                        if (i == 402)
                                        {
                                            Ah.R402CommTran = NumberOfTrans[i];
                                            Ah.R402CommAmount = AmountPerType[i];
                                        }

                                        if (i == 403)
                                        {
                                            Ah.R403CommTran = NumberOfTrans[i];
                                            Ah.R403CommAmount = AmountPerType[i];
                                        }

                                        if (i == 404)
                                        {
                                            Ah.R404CommTran = NumberOfTrans[i];
                                            Ah.R404CommAmount = AmountPerType[i];
                                        }

                                        if (i == 405)
                                        {
                                            Ah.R405CommTran = NumberOfTrans[i];
                                            Ah.R405CommAmount = AmountPerType[i];
                                        }

                                        Ah.AtmNo = InAtmNo;
                                        Ah.BankId = Am.BankId;
                                        //     Ah.Prive = Am.Prive;
                                        Ah.Dt = PreviousDt.Date;
                                        Ah.RecordType = i;

                                        // Read and assign description 
                                        //      Ds.ReadParametersSpecificNo(i);

                                        ParamId = "370";
                                        OccuranceId = i.ToString();
                                        RelatedParmId = "";
                                        RelatedOccuranceId = "";

                                        Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                                        Ah.Description = Gp.OccuranceNm;

                                        Ah.NumberOfTrans = NumberOfTrans[i];
                                        Ah.Amount = AmountPerType[i];
                                        Ah.DateCreated = DateTime.Now;

                                        Am.ReadAtmsMainSpecific(Ah.AtmNo);
                                        Ah.Operator = Am.Operator;

                                        Ah.InsertTransHistoryByType(AtmNo, PreviousDt);
                                        // Initilise 
                                        NumberOfTrans[i] = 0;
                                        AmountPerType[i] = 0;
                                    }
                                }

                                Ah.Operator = Am.Operator;
                                // Create Record 
                                Ah.InsertTransHistory(AtmNo, PreviousDt, DispensedAmt);

                                // Initialised amounts 
                                DrTrans = 0;
                                DispensedAmt = 0;
                                CrTrans = 0;
                                DepAmt = 0;

                            }

                            PreviousDt = AtmDtTime;

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

        //
        // READ TRANSACTIONs For creating the Dispensed History Records  
        //
        public void ReadUpdateTransForDispensedHistory_NEW(string InOperator, string InTerminalId,
                                                                      DateTime FromDate, DateTime ToDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DateTime TransDate;
            string TransCurr;
            decimal TransAmount;
            //         ,[TransDate]  ,[TerminalId]
            //,[TransCurr]
            //  ,[TransAmount]
            //  ,[DepCount]

            //   ,[TraceNoWithNoEndZero]
            //  ,[AtmTraceNo]
            //  ,[MasterTraceNo]

            int DrTrans = 0;
            decimal DispensedAmt = 0;
            int CrTrans = 0;
            decimal DepAmt = 0;
            bool First = true;

            int[] NumberOfTrans = new int[999];
            decimal[] AmountPerType = new decimal[999];

            DateTime PreviousDt = new DateTime(1900, 01, 01);

            InPoolTransactionsDataTable = new DataTable();
            InPoolTransactionsDataTable.Clear();

            TotalSelected = 0;

            SqlString = "SELECT *"
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                 + " WHERE TerminalId = @TerminalId AND TransDate>= @FromDate AND  TransDate<= @ToDate";

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDate", FromDate);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ToDate", ToDate);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(InPoolTransactionsDataTable);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (InPoolTransactionsDataTable.Rows.Count - 1))
                        {

                            RecordFound = true;

                            // GET Table needed fields - Line by Line
                            //

                            TransDate = (DateTime)InPoolTransactionsDataTable.Rows[I]["TransDate"];

                            TransType = (int)InPoolTransactionsDataTable.Rows[I]["TransType"];

                            TransCurr = (string)InPoolTransactionsDataTable.Rows[I]["TransCurr"];

                            TransAmount = (decimal)InPoolTransactionsDataTable.Rows[I]["TransAmount"];

                            CommissionCode = 0;
                            CommissionAmount = 0;

                            if (First == true)
                            {
                                PreviousDt = AtmDtTime;
                                First = false;
                            }

                            if (CommissionCode > 0)
                            {
                                // Create totals for commission records
                                NumberOfTrans[CommissionCode] = NumberOfTrans[CommissionCode] + 1;
                                AmountPerType[CommissionCode] = AmountPerType[CommissionCode] + CommissionAmount;
                            }

                            if (TransType == 11)
                            {
                                RecordFound = true;
                                DrTrans = DrTrans + 1;
                                DispensedAmt = DispensedAmt + TranAmount;
                                // Create totals for turnover 
                                NumberOfTrans[TransType] = NumberOfTrans[TransType] + 1;
                                AmountPerType[TransType] = AmountPerType[TransType] + TranAmount;
                            }

                            if (TransType == 23 || TransType == 24 || TransType == 25)
                            {
                                CrTrans = CrTrans + 1;
                                DepAmt = DepAmt + TranAmount;
                                NumberOfTrans[TransType] = NumberOfTrans[TransType] + 1;
                                AmountPerType[TransType] = AmountPerType[TransType] + TranAmount;
                            }

                            if (AtmDtTime.Date != PreviousDt.Date & DispensedAmt > 0)
                            {
                                // Insert Dispensed History transaction 

                                Am.ReadAtmsMainSpecific(InTerminalId);// READ MAIN 

                                Ah.AtmNo = InTerminalId;
                                Ah.BankId = Am.BankId;
                                Ah.Dt = PreviousDt.Date;
                                Ah.LoadedAtRMCycle = PreviousDt.Year;

                                Ah.DrTransactions = DrTrans;
                                Ah.DispensedAmt = DispensedAmt;
                                Ah.PreEstimated = 0;
                                Ah.CrTransactions = CrTrans;
                                Ah.DepAmount = DepAmt;

                                // Update Array values for other cost parameters 

                                // Annual Maintenance divided daily 
                                Ap.ReadTableATMsCostSpecific(InTerminalId);

                                decimal YearMaintenance = Ap.AnnualMaint;
                                Ho.GetDaysInAYear(DateTime.Now.Year);
                                Ah.C301DailyMaintAmount = YearMaintenance / Ho.daysInYear;

                                NumberOfTrans[301] = 1;
                                AmountPerType[301] = Ah.C301DailyMaintAmount;

                                // Annual Cost of Overhead Divided Daily 

                                ParamId = "370";
                                OccuranceId = "325"; // find annual 
                                RelatedParmId = "";
                                RelatedOccuranceId = "";

                                Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);
                                //      Ds.ReadParametersSpecificNo(325); // find annual 
                                Ah.C307OverheadCost = Gp.Amount / Ho.daysInYear;

                                NumberOfTrans[307] = 1;
                                AmountPerType[307] = Ah.C307OverheadCost;

                                // Cost of investement Divided Daily 
                                Gp.ReadParametersSpecificId(InOperator, "500", "1", "", "");
                                int LastingYears = (int)Gp.Amount;

                                Ah.C309CostOfInvest = Ap.PurchaseCost / (LastingYears * 365);

                                NumberOfTrans[309] = 1;
                                AmountPerType[309] = Ah.C309CostOfInvest;


                                // Replenishment cost 

                                Rs.ReadReplStatClassSpecificDt(Ah.AtmNo, Ah.Dt); // Get Total Repl Time and other fields 

                                if (Rs.RecordFound == true)
                                {
                                    NumberOfTrans[303] = 1;

                                    //      Ds.ReadParametersSpecificNo(324); // find employee cost per minute 


                                    ParamId = "370";
                                    OccuranceId = "324";
                                    RelatedParmId = "";
                                    RelatedOccuranceId = "";

                                    // find employee cost per minute 

                                    Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                                    AmountPerType[303] = Rs.TotalReplMinutes * Gp.Amount;

                                    Ah.C303ReplTimeCost = Rs.TotalReplMinutes * Gp.Amount;

                                    // Cost of Money 
                                    // Find Repl No and Money in 
                                    // Find days for money in 
                                    // Find cost of money based on days

                                    Ta.ReadSessionsStatusTraces(Ah.AtmNo, Rs.ReplCycleNo);

                                    TimeSpan Diff = Ta.SesDtTimeEnd - Ta.SesDtTimeStart;
                                    string NumberDays1 = Diff.TotalHours.ToString();
                                    int NumberHours = Convert.ToInt32(Diff.TotalHours);

                                    // Ds.ReadParametersSpecificNo(322); // find cost of money

                                    ParamId = "370";
                                    OccuranceId = "322";
                                    RelatedParmId = "";
                                    RelatedOccuranceId = "";

                                    // find cost of money

                                    Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                                    NumberOfTrans[308] = NumberHours / 24;

                                    AmountPerType[308] = Rs.InMoneyLast * NumberHours * Gp.Amount / (Ho.daysInYear * 24);

                                    Ah.C308CostOfMoney = Rs.InMoneyLast * NumberHours * Gp.Amount / (Ho.daysInYear * 24);
                                }



                                /* Create Stats records  */
                                for (int i = 0; i < 999; i++)
                                {
                                    if (NumberOfTrans[i] > 0 || AmountPerType[i] > 0)
                                    {
                                        if (i == 401)
                                        {
                                            Ah.R401CommTran = NumberOfTrans[i];
                                            Ah.R401CommAmount = AmountPerType[i];
                                        }

                                        if (i == 402)
                                        {
                                            Ah.R402CommTran = NumberOfTrans[i];
                                            Ah.R402CommAmount = AmountPerType[i];
                                        }

                                        if (i == 403)
                                        {
                                            Ah.R403CommTran = NumberOfTrans[i];
                                            Ah.R403CommAmount = AmountPerType[i];
                                        }

                                        if (i == 404)
                                        {
                                            Ah.R404CommTran = NumberOfTrans[i];
                                            Ah.R404CommAmount = AmountPerType[i];
                                        }

                                        if (i == 405)
                                        {
                                            Ah.R405CommTran = NumberOfTrans[i];
                                            Ah.R405CommAmount = AmountPerType[i];
                                        }

                                        Ah.AtmNo = InTerminalId;
                                        Ah.BankId = Am.BankId;
                                        //     Ah.Prive = Am.Prive;
                                        Ah.Dt = PreviousDt.Date;
                                        Ah.RecordType = i;

                                        // Read and assign description 
                                        //      Ds.ReadParametersSpecificNo(i);

                                        ParamId = "370";
                                        OccuranceId = i.ToString();
                                        RelatedParmId = "";
                                        RelatedOccuranceId = "";

                                        Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                                        Ah.Description = Gp.OccuranceNm;

                                        Ah.NumberOfTrans = NumberOfTrans[i];
                                        Ah.Amount = AmountPerType[i];
                                        Ah.DateCreated = DateTime.Now;

                                        Am.ReadAtmsMainSpecific(Ah.AtmNo);
                                        Ah.Operator = Am.Operator;

                                        Ah.InsertTransHistoryByType(AtmNo, PreviousDt);
                                        // Initilise 
                                        NumberOfTrans[i] = 0;
                                        AmountPerType[i] = 0;
                                    }
                                }

                                Ah.Operator = Am.Operator;
                                // Create Record 
                                Ah.InsertTransHistory(AtmNo, PreviousDt, DispensedAmt);

                                // Initialised amounts 
                                DrTrans = 0;
                                DispensedAmt = 0;
                                CrTrans = 0;
                                DepAmt = 0;

                            }

                            PreviousDt = AtmDtTime;

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

        //
        // READ  TRANS FOR A CARD 
        //
        public void ReadTransForCard(string InCardNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE CardNo = @CardNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CardNo", InCardNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            TranNo = (int)rdr["TranNo"];
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
        // READ TRANS No by Using Trace No 
        //
        public void ReadTranForTrace(string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE AtmNo = @AtmNo AND AtmTraceNo = @AtmTraceNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

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

        // UPDATE TRANSACTION ERROR NUMBER 
        // 
        public void UpdateTransErrNo(int InTranNo, int InErrNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[InPoolTrans] SET "
                            + " ErrNo = @ErrNo"
                            + " WHERE TranNo = @TranNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);

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

        // Insert TRANS FROM EJOURNAL TO IN POOLTO BE UPDATED 
        //
        public int InsertTransInPool(string InOperator, string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[InPoolTrans]"
                + " ([OriginName] ,[RMCateg] ,[AtmTraceNo] ,[MasterTraceNo] ,[EJournalTraceNo] ,"
                + "[AtmNo] ,[SesNo] ,[BankId] ,[BranchId] ,"
                + "[AtmDtTime], [UniqueRecordId],"
                + "[SystemTarget] ,[TransType] ,[TransDesc] ,[CardNo] ,[CardOrigin],"
                + "[AccNo] ,[CurrDesc] ,[TranAmount] ,[AuthCode] ,[RefNumb],[RemNo],"
                + "[TransMsg] ,[AtmMsg] ,[ErrNo] ,[StartTrxn] ,[EndTrxn] ,[DepCount] ,"
                + "[CommissionCode],[CommissionAmount],[SuccTran],[Operator] )"
                + " VALUES "
                 + " (@OriginName ,@RMCateg ,@AtmTraceNo ,@MasterTraceNo,@EJournalTraceNo ,"
                + "@AtmNo ,@SesNo ,@BankId ,@BranchId ,"
                + "@AtmDtTime, @UniqueRecordId, "
                + "@SystemTarget ,@TransType ,@TransDesc ,@CardNo ,@CardOrigin,"
                + "@AccNo ,@CurrDesc ,@TranAmount ,@AuthCode ,@RefNumb,@RemNo ,"
                + "@TransMsg,@AtmMsg,@ErrNo ,@StartTrxn ,@EndTrxn,@DepCount ,"
                + "@CommissionCode,@CommissionAmount,@SuccTran,"
                + " @Operator ) ;"
               + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@OriginName", OriginName);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                        cmd.Parameters.AddWithValue("@MasterTraceNo", MasterTraceNo);
                        cmd.Parameters.AddWithValue("@EJournalTraceNo", EJournalTraceNo);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);

                        cmd.Parameters.AddWithValue("@AtmDtTime", AtmDtTime);

                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);

                        cmd.Parameters.AddWithValue("@SystemTarget", SystemTarget);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDesc", TransDesc);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CardOrigin", CardOrigin);

                        cmd.Parameters.AddWithValue("@AccNo", AccNo);

                        cmd.Parameters.AddWithValue("@CurrDesc", CurrDesc);
                        cmd.Parameters.AddWithValue("@TranAmount", TranAmount);
                        cmd.Parameters.AddWithValue("@AuthCode", AuthCode);
                        cmd.Parameters.AddWithValue("@RefNumb", RefNumb);
                        cmd.Parameters.AddWithValue("@RemNo", RemNo);

                        cmd.Parameters.AddWithValue("@TransMsg", TransMsg);
                        cmd.Parameters.AddWithValue("@AtmMsg", AtmMsg);
                        cmd.Parameters.AddWithValue("@ErrNo", ErrNo);
                        cmd.Parameters.AddWithValue("@StartTrxn", StartTrxn);
                        cmd.Parameters.AddWithValue("@EndTrxn", EndTrxn);

                        cmd.Parameters.AddWithValue("@DepCount", DepCount);

                        cmd.Parameters.AddWithValue("@CommissionCode", CommissionCode);
                        cmd.Parameters.AddWithValue("@CommissionAmount", CommissionAmount);

                        cmd.Parameters.AddWithValue("@SuccTran", SuccTran);

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        TranNo = (int)cmd.ExecuteScalar();
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
            return TranNo;
        }
        //
        // Insert NEW TRANS TO BE READY FOR UPDATING  
        //
        public string NostroCcy;
        public decimal NostroAdjAmt;
        public string MakerUser;
        public int InsertTransToBePosted(string InAtmNo, int InErrNo, string InOriginName)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[TransToBePosted]"
                + " ("
                + "[OriginId] "
                + ",[OriginName]"
                + ",[RMCateg] "
                + ",[RMCategCycle] "
                + ",[UniqueRecordId] "
                + ",[ErrNo]"
                + ",[AtmNo]"
                + ",[SesNo]"
                + ",[DisputeNo] "
                + ",[DispTranNo] "
                + ",[BankId]"
                + ",[AtmTraceNo]"
                + ",[BranchId]"
                + ",[AtmDtTime]"
                + ",[SystemTarget]"
                + ",[CardNo] "
                + ",[CardOrigin]"
                + ",[TransType]"
                + ",[TransDesc]"
                + ",[AccNo]"
                + ",[GlEntry]"
                + ",[NostroCcy]"
                + ",[NostroAdjAmt]"
                + ",[TransType2]"
                + ",[TransDesc2]"
                + ",[AccNo2]"
                + ",[GlEntry2]"
                + ",[CurrDesc] "
                + ",[TranAmount] "
                + ",[AuthCode] "
                + ",[RefNumb]"
                + ",[RemNo] "
                + ",[TransMsg]"
                + ",[AtmMsg]"
                + ",[MakerUser]"
                + ",[AuthUser] "
                + ",[OpenDate] "
                + ",[OpenRecord]"
                + ",[Operator]"
                + ")"
                + " VALUES "
                + " (@OriginId ,@OriginName ,@RMCateg ,@RMCategCycle ,"
                + "@UniqueRecordId ,@ErrNo ,@AtmNo ,@SesNo , @DisputeNo, @DispTranNo ,@BankId ,"
                + "@AtmTraceNo ,@BranchId ,@AtmDtTime,"
                + "@SystemTarget ,@CardNo ,@CardOrigin,"
                + " @TransType ,@TransDesc , @AccNo , @GlEntry, "
                + " @NostroCcy , @NostroAdjAmt, "
                + " @TransType2 ,@TransDesc2 , @AccNo2 , @GlEntry2 , "
                + "@CurrDesc ,@TranAmount ,@AuthCode ,@RefNumb,"
                + "@RemNo ,@TransMsg,@AtmMsg,@MakerUser,@AuthUser,@OpenDate,@OpenRecord,@Operator )"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginId", OriginId);
                        cmd.Parameters.AddWithValue("@OriginName", InOriginName);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@RMCategCycle", RMCategCycle);

                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);
                        cmd.Parameters.AddWithValue("@ErrNo", ErrNo);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);

                        cmd.Parameters.AddWithValue("@DisputeNo", DisputeNo);
                        cmd.Parameters.AddWithValue("@DispTranNo", DispTranNo);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@AtmDtTime", AtmDtTime);

                        cmd.Parameters.AddWithValue("@SystemTarget", SystemTarget);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CardOrigin", CardOrigin);

                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDesc", TransDesc);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@GlEntry", GlEntry);

                        cmd.Parameters.AddWithValue("@NostroCcy", NostroCcy);
                        cmd.Parameters.AddWithValue("@NostroAdjAmt", NostroAdjAmt);

                        cmd.Parameters.AddWithValue("@TransType2", TransType2);
                        cmd.Parameters.AddWithValue("@TransDesc2", TransDesc2);
                        cmd.Parameters.AddWithValue("@AccNo2", AccNo2);
                        cmd.Parameters.AddWithValue("@GlEntry2", GlEntry2);

                        cmd.Parameters.AddWithValue("@CurrDesc", CurrDesc);
                        cmd.Parameters.AddWithValue("@TranAmount", TranAmount);

                        cmd.Parameters.AddWithValue("@AuthCode", AuthCode);
                        cmd.Parameters.AddWithValue("@RefNumb", RefNumb);

                        cmd.Parameters.AddWithValue("@RemNo", RemNo);
                        cmd.Parameters.AddWithValue("@TransMsg", TransMsg);
                        cmd.Parameters.AddWithValue("@AtmMsg", AtmMsg);

                        cmd.Parameters.AddWithValue("@MakerUser", MakerUser);
                        cmd.Parameters.AddWithValue("@AuthUser", AuthUser);

                        cmd.Parameters.AddWithValue("@OpenDate", OpenDate);
                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        PostedNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            return PostedNo;
        }


        public int InsertTransToBePosted_BDC(int InActionSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";
            // RMCycleNo = WReconcCycleNo, 
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[TransToBePosted]"
                + " ("
                + "[OriginId] "
                + ",[OriginName]"
                + ",[RMCateg] "
                + ",[RMCategCycle] "
                + ",[ActionSeqNo] "
                 + ",[UniqueRecordId] "
                + ",[ErrNo]"
                + ",[AtmNo]"
                + ",[SesNo]"
                + ",[RMCycleNo]"
                + ",[DisputeNo] "
                + ",[DispTranNo] "
                + ",[BankId]"
                + ",[AtmTraceNo]"
                + ",[RRNumber]"
                + ",[TXNSRC]"
                + ",[TXNDEST]"
                + ",[BranchId]"
                + ",[AtmDtTime]"
                + ",[SystemTarget]"
                + ",[CardNo] "
                + ",[CardOrigin]"
                 + ",[BranchId1]"
                + ",[TransType]"
                + ",[TransDesc]"
                + ",[AccNo]"
                + ",[GlEntry]"
                 //+ ",[NostroCcy]"
                 //+ ",[NostroAdjAmt]"
                 + ",[BranchId2]"
                + ",[TransType2]"
                + ",[TransDesc2]"
                + ",[AccNo2]"
                + ",[GlEntry2]"
                + ",[CurrDesc] "
                + ",[TranAmount] "
                + ",[AuthCode] "
                //+ ",[RefNumb]"
                //+ ",[RemNo] "
                + ",[TransMsg]"
                + ",[AtmMsg]"
                + ",[MakerUser]"
                + ",[AuthUser] "
                + ",[OpenDate] "
                + ",[OpenRecord]"
                + ",[Operator]"
                + ")"
                + " VALUES "
                + " (@OriginId ,@OriginName ,@RMCateg ,@RMCategCycle "
                + ",@ActionSeqNo "
                   + ",@UniqueRecordId "
                + ",@ErrNo ,@AtmNo ,@SesNo , @RMCycleNo ,@DisputeNo, @DispTranNo ,@BankId "
                + " ,@AtmTraceNo "
                 + " ,@RRNumber "
                  + " ,@TXNSRC "
                   + " ,@TXNDEST "
                + ",@BranchId ,@AtmDtTime,"
                + "@SystemTarget ,@CardNo ,@CardOrigin"
                 + " ,@BranchId1 "
                + " ,@TransType "
                + ",@TransDesc , @AccNo , @GlEntry "
                // + " @NostroCcy , @NostroAdjAmt "
                + ", @BranchId2 "
                + ", @TransType2 "
                + ",@TransDesc2 , @AccNo2 , @GlEntry2 , "
                + "@CurrDesc ,@TranAmount ,@AuthCode "
                //+",@RefNumb,"
                //+ "@RemNo "
                + ",@TransMsg,@AtmMsg,@MakerUser,@AuthUser,@OpenDate,@OpenRecord,@Operator )"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginId", OriginId);
                        cmd.Parameters.AddWithValue("@OriginName", OriginName);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@RMCategCycle", RMCategCycle);

                        cmd.Parameters.AddWithValue("@ActionSeqNo", InActionSeqNo);

                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);
                        cmd.Parameters.AddWithValue("@ErrNo", ErrNo);
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);
                        cmd.Parameters.AddWithValue("@RMCycleNo", RMCycleNo);

                        cmd.Parameters.AddWithValue("@DisputeNo", DisputeNo);
                        cmd.Parameters.AddWithValue("@DispTranNo", DispTranNo);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);

                        cmd.Parameters.AddWithValue("@RRNumber", RRNumber);
                        cmd.Parameters.AddWithValue("@TXNSRC", TXNSRC);
                        cmd.Parameters.AddWithValue("@TXNDEST", TXNDEST);

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@AtmDtTime", AtmDtTime);

                        cmd.Parameters.AddWithValue("@SystemTarget", SystemTarget);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CardOrigin", CardOrigin);

                        cmd.Parameters.AddWithValue("@BranchId1", BranchId1);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDesc", TransDesc);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@GlEntry", GlEntry);

                        //cmd.Parameters.AddWithValue("@NostroCcy", NostroCcy);
                        //cmd.Parameters.AddWithValue("@NostroAdjAmt", NostroAdjAmt);

                        cmd.Parameters.AddWithValue("@BranchId2", BranchId2);
                        cmd.Parameters.AddWithValue("@TransType2", TransType2);
                        cmd.Parameters.AddWithValue("@TransDesc2", TransDesc2);
                        cmd.Parameters.AddWithValue("@AccNo2", AccNo2);
                        cmd.Parameters.AddWithValue("@GlEntry2", GlEntry2);

                        cmd.Parameters.AddWithValue("@CurrDesc", CurrDesc);
                        cmd.Parameters.AddWithValue("@TranAmount", TranAmount);

                        cmd.Parameters.AddWithValue("@AuthCode", AuthCode);

                        //cmd.Parameters.AddWithValue("@RefNumb", RefNumb);
                        //cmd.Parameters.AddWithValue("@RemNo", RemNo);

                        cmd.Parameters.AddWithValue("@TransMsg", TransMsg);
                        cmd.Parameters.AddWithValue("@AtmMsg", AtmMsg);

                        cmd.Parameters.AddWithValue("@MakerUser", MakerUser);
                        cmd.Parameters.AddWithValue("@AuthUser", AuthUser);

                        cmd.Parameters.AddWithValue("@OpenDate", OpenDate);
                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        PostedNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            return PostedNo;
        }

        // 
        // Mark Txn As Reversal
        // 
        int rows;
        public void UpdateReversalTransToBePosted(int InPostedNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " IsReversal = 1 "
                            + " WHERE PostedNo = @PostedNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@PostedNo", InPostedNo);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();

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


        // DELETE OLD TRANS TO BE POSTED 
        // 
        public void DeleteOldTransToBePosted(string InAtmNo, int InErrNo)
        {
            // DELETE ACTION If Already taken   

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[TransToBePosted] "
                            + " WHERE ErrNo = @ErrNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);

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


        // DELETE OLD TRANS TO BE POSTED By ATMNo And SessNo
        // 
        public void DeleteOldTransToBePostedByATMNoAndSession(string InAtmNo, int InSesNo, string InTransDesc)
        {
            // DELETE ACTION If Already taken   

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[TransToBePosted] "
                            + " WHERE AtmNo = @AtmNo AND SesNo=@SesNo and TransDesc=@TransDesc", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                        cmd.Parameters.AddWithValue("@TransDesc", InTransDesc);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        // CLOSE TRANS TO BE POSTED
        // 
        public void UpdateTransToBePostedClose(int InPostedNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " OpenRecord = @OpenRecord "
                            + " WHERE PostedNo = @PostedNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@PostedNo", InPostedNo);
                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        // Initialize Trans to be posted Auth User for this User
        // Make all Auth User = 0 where Signed User = '500' say
        // UPDATE AUTHORISED USER ON TRANSCTION TO BE POSTED 
        // 
        public void ClearTransToBePostedAuthUser(string InAuthUser)
        {


            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " AuthUser = '' "
                            + " WHERE AuthUser = @AuthUser ", conn))

                    {
                        cmd.Parameters.AddWithValue("@AuthUser", InAuthUser);

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



        // UPDATE AUTHORISED USER ON TRANSCTION TO BE POSTED 
        // 
        public void UpdateTransToBePostedAuthUser(string InAtmNo, string InAuthUser)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " AuthUser = @AuthUser "
                            + " WHERE AtmNo = @AtmNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AuthUser", InAuthUser);

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

        // UPDATE ACTION ON TRANSCTION TO BE POSTED based on Tran number 
        // USED FOR MATCHED DATES TOO
        // Close it too 
        public void UpdateTransToBePostedAction1(int InTranNumber, string InUser, int InActionCode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " ActionBy = @ActionBy,  ActionCd2 = @ActionCd2 , ActionDate = @ActionDate, "
                            + " GridFilterDate = @GridFilterDate,OpenRecord = @OpenRecord   "
                             + " WHERE PostedNo = @PostedNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@PostedNo", InTranNumber);
                        cmd.Parameters.AddWithValue("@ActionBy", InUser);
                        cmd.Parameters.AddWithValue("@ActionCd2", InActionCode);
                        cmd.Parameters.AddWithValue("@ActionDate", ActionDate);
                        //cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        //cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@GridFilterDate", GridFilterDate);
                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

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
        // USED FOR MATCHED DATES 
        //
        public void UpdateTransToBePostedMatched(string InAtmNo, int InPostedNo, bool InHostMatched,
                                                  DateTime InMatchedDtTm, int InReconciled)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " HostMatched = @HostMatched,  MatchedDtTm = @MatchedDtTm , Reconciled = @Reconciled "
                             + " WHERE AtmNo = @AtmNo AND PostedNo = @PostedNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@PostedNo", InPostedNo);
                        cmd.Parameters.AddWithValue("@HostMatched", InHostMatched);
                        cmd.Parameters.AddWithValue("@MatchedDtTm", InMatchedDtTm);
                        cmd.Parameters.AddWithValue("@Reconciled", InReconciled);


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
        // Read to find actions finalised and not Finalised TOTAls Based on AtmNo And SesNo
        // 

        public void ReadTransToBePostedTotals(string InAtmNo, int InSesNo)
        {
            TotActions = 0;
            TotActionsTaken = 0;
            TotActionsNotTaken = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE AtmNo = @AtmNo AND SesNo =@SesNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsTransToBePosted(rdr);


                            // All Actions 

                            TotActions = TotActions + 1;


                            if (ActionCd2 == 1) // Action  taken
                            {
                                TotActionsTaken = TotActionsTaken + 1;
                            }
                            if (ActionCd2 != 1) // Action Not Taken 
                            {
                                TotActionsNotTaken = TotActionsNotTaken + 1;
                            }

                        }

                        // Close ReaderOpenRecord
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

        // Copy transactions for testing purpose
        // read and Copy from one ATM to another 

        //
        // READ all transactions based on Criteria  
        //
        public void CopyInPoolTrans(string InBankId, string InAtmNo, int InSesNo, string TargetBank, string TargetAtm,
                                  int TargetSesNo, bool TargetPrive)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE BankId = @BankId AND AtmNo = @AtmNo AND SesNo = @SesNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            TranNo = (int)rdr["TranNo"];
                            OriginName = (string)rdr["OriginName"];
                            RMCateg = (string)rdr["RMCateg"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            MasterTraceNo = (int)rdr["MasterTraceNo"];

                            EJournalTraceNo = (int)rdr["EJournalTraceNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            CardNo = (string)rdr["CardNo"];

                            CardOrigin = (int)rdr["CardOrigin"];
                            AccNo = (string)rdr["AccNo"];

                            //            CurrCode = (int)rdr["CurrCode"];
                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ErrNo = (int)rdr["ErrNo"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DepCount = (decimal)rdr["DepCount"];

                            CommissionCode = (int)rdr["CommissionCode"];
                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            SuccTran = (bool)rdr["SuccTran"];

                            Operator = (string)rdr["Operator"];

                            // Insert / Copy transaction 

                            BankId = TargetBank;
                            //     Prive = TargetPrive; 
                            AtmNo = TargetAtm;
                            SesNo = TargetSesNo;
                            CurrDesc = "GBP";

                            Am.ReadAtmsMainSpecific(InAtmNo);
                            // ADD THE TRANSACTION 
                            InsertTransInPool(Am.Operator, InAtmNo);

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
      

    }
}

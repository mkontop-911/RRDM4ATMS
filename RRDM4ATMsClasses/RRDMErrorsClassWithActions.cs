using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMErrorsClassWithActions : Logger
    {
        public RRDMErrorsClassWithActions() : base() { }
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
        public int UniqueRecordId;

        public string AtmNo;
        public int SesNo;
        public int TraceNo;
        //public int TransNo;
        public int TransType;
        public string TransDescr;
        public DateTime DateTime;
        public bool NeedAction;
        public bool OpenErr;

        public string CurDes;
        public decimal ErrAmount;

        public decimal NoteFaceValue;
        public string NoteSerialId;

        public int ActionId;
        public bool DrCust;
        public bool CrCust;
        public bool DrAtmCash;
        public bool CrAtmCash;
        public bool DrAtmSusp;
        public bool CrAtmSusp;
        public bool UnderAction; // If true then Txns are created
        public bool DisputeAct;
        public bool ManualAct;   // if true no txns are created
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
        public int ActionSesNo;
        public int ActionRMCycle; // At what Cut Off Id Action was taken
        public string CustAccNo;
        public string AccountNo1;
        public string AccountNo2;

        public bool DrAccount3;
        public bool CrAccount3;
        public string AccountNo3;

        public bool Dr_Intermediary;
        public bool Cr_Intermediary;
        public string AccountNo4;

        public string UserComment;
        public bool Printed;
        public DateTime DatePrinted;
        public string CircularDesc;
        public string AuthoriserId;
        public string CitId;
        public string Operator;

        public int NumOfErrors;
        public int NumOfErrorsLess200;

        public int NumOfOpenErrorsLess100; // ATM Errors Presenter
        public decimal TotalErrorsAmtLess100;

        public int NumOfOpenErrorsBetween100And200; // Host Errors 
        public decimal TotalErrorsBetween100And200;

        public int NumOfOpenErrorsBetween200And300; // Deposit Errors 
        public decimal TotalErrorsBetween200And300;

        public int ErrUnderAction;
        public int ErrDisputeAction;
        public int ErrUnderManualAction;

        public decimal TotalErrorsAmt;
        public decimal TotalUnderActionAmt;

        int InTraceNumber;


        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public DataTable ErrorsTable = new DataTable();
        public DataTable ErrorsTableReport = new DataTable();
        public DataTable ErrorsTableAllFields = new DataTable();
        public DataTable ShortErrorsTable = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public int TotalSelected;

        string ErrorsFileId = "[ATMS].[dbo].[ErrorsTable]";

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();
        RRDMAccountsClass Acc = new RRDMAccountsClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();
        //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        //
        // Read Error Characteristics Fields
        //
        private void ReadErrorCharacteristicsFields(SqlDataReader rdr)
        {
            // Read error Characteristics Details

            ErrId = (int)rdr["ErrId"];
            ErrDesc = rdr["ErrDesc"].ToString();
            ErrType = (int)rdr["ErrType"];
            UniqueRecordId = (int)rdr["UniqueRecordId"];
            AtmNo = (string)rdr["AtmNo"];
            SesNo = (int)rdr["SesNo"];
            DateInserted = (DateTime)rdr["DateInserted"];

            BankId = rdr["BankId"].ToString();

            BranchId = rdr["BranchId"].ToString();
            TurboReconc = (bool)rdr["TurboReconc"];

            TraceNo = (int)rdr["TraceNo"];
            CardNo = rdr["CardNo"].ToString();

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
            ActionRMCycle = (int)rdr["ActionRMCycle"];

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

            Dr_Intermediary = (bool)rdr["Dr_Intermediary"];
            Cr_Intermediary = (bool)rdr["Cr_Intermediary"];
            AccountNo4 = rdr["AccountNo4"].ToString();

            ForeignCard = (bool)rdr["ForeignCard"];
            MainOnly = (bool)rdr["MainOnly"];

            UserComment = rdr["UserComment"].ToString();

            Printed = (bool)rdr["Printed"];
            DatePrinted = (DateTime)rdr["DatePrinted"];

            CircularDesc = rdr["CircularDesc"].ToString();

            Operator = (string)rdr["Operator"];
        }


        // READ ERROR FIELDS 
        private void ReadErrorFields(SqlDataReader rdr)
        {
            // Read error Details

            ErrNo = (int)rdr["ErrNo"];
            ErrId = (int)rdr["ErrId"];
            ErrDesc = rdr["ErrDesc"].ToString();
            ErrType = (int)rdr["ErrType"];

            CategoryId = (string)rdr["CategoryId"];

            RMCycle = (int)rdr["RMCycle"];
            UniqueRecordId = (int)rdr["UniqueRecordId"];

            AtmNo = (string)rdr["AtmNo"];
            SesNo = (int)rdr["SesNo"];
            DateInserted = (DateTime)rdr["DateInserted"];

            BankId = rdr["BankId"].ToString();

            BranchId = rdr["BranchId"].ToString();
            TurboReconc = (bool)rdr["TurboReconc"];

            TraceNo = (int)rdr["TraceNo"];
            CardNo = rdr["CardNo"].ToString();

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
            ActionSesNo = (int)rdr["ActionSesNo"];
            ActionRMCycle = (int)rdr["ActionRMCycle"];

            CurDes = rdr["CurDes"].ToString();
            ErrAmount = (decimal)rdr["ErrAmount"];

            NoteFaceValue = (decimal)rdr["NoteFaceValue"];
            NoteSerialId = (string)rdr["NoteSerialId"];

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

            Dr_Intermediary = (bool)rdr["Dr_Intermediary"];
            Cr_Intermediary = (bool)rdr["Cr_Intermediary"];
            AccountNo3 = rdr["AccountNo3"].ToString();

            ForeignCard = (bool)rdr["ForeignCard"];
            MainOnly = (bool)rdr["MainOnly"];

            UserComment = rdr["UserComment"].ToString();

            Printed = (bool)rdr["Printed"];
            DatePrinted = (DateTime)rdr["DatePrinted"];

            CircularDesc = rdr["CircularDesc"].ToString();

            AuthoriserId = (string)rdr["AuthoriserId"];

            CitId = (string)rdr["CitId"];

            Operator = (string)rdr["Operator"];
        }


        string SqlString;


        // READ Error ID Record 
        public void ReadErrorsIDRecord(int InErrId, string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
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

                            ReadErrorCharacteristicsFields(rdr);
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

        // READ Errors TO CALCULATE Totals Under Action 
        //
        public int NumberOfErrors = 0;
        public int NumberOfErrJournal = 0;
        public int ErrJournalThisCycle = 0;
        public int NumberOfErrDep = 0;
        public int NumberOfErrHost = 0;
        public int ErrHostToday = 0;
        public int ErrOutstanding = 0;

        public int NumberOfPresenterErrors;

        public decimal PresenterErrorsAmt;

        public int ErrorsAdjastingBalances = 0;

        public decimal EffectOnMachineBal;
        public decimal EffectOnReplToRepl;
        public decimal EffectOnHostBalAdj;



        // Insert Error 
        public int InsertError()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ErrorsTable] ([ErrId],[ErrDesc],[ErrType],"
                   + "[CategoryId],[RMCycle],[UniqueRecordId],[AtmNo],[SesNo],[DateInserted],"
                   + "[BankId],[BranchId],[TurboReconc],[TraceNo],[CardNo],[TransType],[TransDescr],"
                   + "[DateTime],[NeedAction],[OpenErr],[FullCard],[UnderAction],[ManualAct],"
                   + "[ByWhom],[ActionDtTm],[ActionRMCycle],[CurDes],[ErrAmount],[ActionId],"
                   + "[DrCust],[CrCust],[CustAccNo],[DrAtmCash],[CrAtmCash],[AccountNo1],[DrAtmSusp],[CrAtmSusp],[AccountNo2],"
                   + "[DrAccount3],[CrAccount3],[AccountNo3],"
                   + "[Dr_Intermediary],[Cr_Intermediary],[AccountNo4],"
                   + "[ForeignCard],[MainOnly],[UserComment],"
                   + "[Printed],[DatePrinted],[CircularDesc],[CitId], [Operator])"
                + " VALUES (@ErrId,@ErrDesc,@ErrType,"
                    + "@CategoryId,@RMCycle,@UniqueRecordId,@AtmNo,@SesNo,@DateInserted,"
                    + "@BankId,@BranchId,@TurboReconc,@TraceNo,@CardNo,@TransType,@TransDescr,"
                   + "@DateTime,@NeedAction,@OpenErr,@FullCard,@UnderAction,@ManualAct,"
                   + "@ByWhom,@ActionDtTm,@ActionRMCycle,@CurDes,@ErrAmount,@ActionId,"
                   + "@DrCust,@CrCust,@CustAccNo,@DrAtmCash,@CrAtmCash,@AccountNo1,@DrAtmSusp,@CrAtmSusp,@AccountNo2,"
                   + "@DrAccount3,@CrAccount3,@AccountNo3,"
                   + "@Dr_Intermediary,@Cr_Intermediary,@AccountNo4,"
                   + "@ForeignCard, @MainOnly,@UserComment,"
                   + "@Printed,@DatePrinted,@CircularDesc,@CitId, "
                   + " @Operator ) ;"
                   + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            //+ " SELECT CAST(SCOPE_IDENTITY() AS int)"; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@ErrId", ErrId);
                        cmd.Parameters.AddWithValue("@ErrDesc", ErrDesc);
                        cmd.Parameters.AddWithValue("@ErrType", ErrType);

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", RMCycle);
                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);

                        cmd.Parameters.AddWithValue("@DateInserted", DateInserted);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@TurboReconc", TurboReconc);

                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);

                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDescr", TransDescr);

                        cmd.Parameters.AddWithValue("@DateTime", DateTime);
                        cmd.Parameters.AddWithValue("@NeedAction", NeedAction);
                        cmd.Parameters.AddWithValue("@OpenErr", OpenErr);
                        cmd.Parameters.AddWithValue("@FullCard", FullCard);

                        cmd.Parameters.AddWithValue("@UnderAction", UnderAction); // 
                        cmd.Parameters.AddWithValue("@ManualAct", ManualAct);  //

                        cmd.Parameters.AddWithValue("@ByWhom", ByWhom);  //
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);
                        cmd.Parameters.AddWithValue("@ActionRMCycle", ActionRMCycle);

                        cmd.Parameters.AddWithValue("@CurDes", CurDes);
                        cmd.Parameters.AddWithValue("@ErrAmount", ErrAmount);
                        cmd.Parameters.AddWithValue("@ActionId", ActionId);

                        cmd.Parameters.AddWithValue("@DrCust", DrCust);
                        cmd.Parameters.AddWithValue("@CrCust", CrCust);
                        cmd.Parameters.AddWithValue("@CustAccNo", CustAccNo);

                        cmd.Parameters.AddWithValue("@DrAtmCash", DrAtmCash);
                        cmd.Parameters.AddWithValue("@CrAtmCash", CrAtmCash);
                        cmd.Parameters.AddWithValue("@AccountNo1", AccountNo1);

                        cmd.Parameters.AddWithValue("@DrAtmSusp", DrAtmSusp);
                        cmd.Parameters.AddWithValue("@CrAtmSusp", CrAtmSusp);
                        cmd.Parameters.AddWithValue("@AccountNo2", AccountNo2);

                        cmd.Parameters.AddWithValue("@DrAccount3", DrAccount3);
                        cmd.Parameters.AddWithValue("@CrAccount3", CrAccount3);
                        cmd.Parameters.AddWithValue("@AccountNo3", AccountNo3);

                        cmd.Parameters.AddWithValue("@Dr_Intermediary", Dr_Intermediary);
                        cmd.Parameters.AddWithValue("@Cr_Intermediary", Cr_Intermediary);
                        cmd.Parameters.AddWithValue("@AccountNo4", AccountNo4);

                        cmd.Parameters.AddWithValue("@ForeignCard", ForeignCard);
                        cmd.Parameters.AddWithValue("@MainOnly", MainOnly);

                        cmd.Parameters.AddWithValue("@UserComment", UserComment);
                        cmd.Parameters.AddWithValue("@Printed", Printed);
                        cmd.Parameters.AddWithValue("@DatePrinted", DatePrinted);

                        cmd.Parameters.AddWithValue("@CircularDesc", CircularDesc);

                        cmd.Parameters.AddWithValue("@CitId", CitId);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        ErrNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return ErrNo;
        }

        //
        //
        // READ Errors to fill table  by Filter 
        //
        //
        //public int NotInTransactionPoolYet; 
        public void ReadErrorsAndFillTable(string InOperator, string InUser, string InFilter)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorsTable = new DataTable();
            ErrorsTable.Clear();
            TotalSelected = 0;

            //NotInTransactionPoolYet = 0; 

            // DATA TABLE ROWS DEFINITION 

            ErrorsTable.Columns.Add("ExcNo", typeof(int));
            ErrorsTable.Columns.Add("AtmNo", typeof(string));
            ErrorsTable.Columns.Add("Descrip", typeof(string));
            ErrorsTable.Columns.Add("Card", typeof(string));
            ErrorsTable.Columns.Add("Ccy", typeof(string));
            ErrorsTable.Columns.Add("Amount", typeof(string));

            ErrorsTable.Columns.Add("NeedAction", typeof(string));
            ErrorsTable.Columns.Add("UnderAction", typeof(string));
            ErrorsTable.Columns.Add("DisputeAct", typeof(string));
            ErrorsTable.Columns.Add("ManualAct", typeof(string));

            ErrorsTable.Columns.Add("UserComment", typeof(string));

            ErrorsTable.Columns.Add("NoteSerialId", typeof(string));

            ErrorsTable.Columns.Add("DateTime", typeof(DateTime));
            ErrorsTable.Columns.Add("TransDescr", typeof(string));

            ErrorsTable.Columns.Add("DrCust", typeof(bool));
            ErrorsTable.Columns.Add("CrCust", typeof(bool));
            ErrorsTable.Columns.Add("CustAccNo", typeof(string));

            ErrorsTable.Columns.Add("DrAtmCash", typeof(bool));
            ErrorsTable.Columns.Add("CrAtmCash", typeof(bool));
            ErrorsTable.Columns.Add("AccountNo1", typeof(string));

            ErrorsTable.Columns.Add("DrAtmSusp", typeof(bool));
            ErrorsTable.Columns.Add("CrAtmSusp", typeof(bool));
            ErrorsTable.Columns.Add("AccountNo2", typeof(string));

            ErrorsTable.Columns.Add("SessionNo", typeof(string));

            ErrorsTable.Columns.Add("Mask", typeof(string));
            ErrorsTable.Columns.Add("ActionTaken", typeof(string)); // Yes, NO
            ErrorsTable.Columns.Add("UserId", typeof(string));

            SqlString = "SELECT * "
                         + " FROM " + ErrorsFileId
                         + " WHERE " + InFilter
                          + " Order by ErrNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Fields
                            ReadErrorFields(rdr);

                            // Fill Table 

                            DataRow RowSelected = ErrorsTable.NewRow();

                            RowSelected["ExcNo"] = ErrNo;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["Descrip"] = ErrDesc;
                            RowSelected["Card"] = CardNo;
                            RowSelected["Ccy"] = CurDes;
                            RowSelected["Amount"] = ErrAmount.ToString("#,##0.00");

                            if (NeedAction == true)
                            {
                                RowSelected["NeedAction"] = "YES";
                            }
                            else RowSelected["NeedAction"] = "NO";
                            if (UnderAction == true)
                            {
                                RowSelected["UnderAction"] = "YES";
                            }
                            else RowSelected["UnderAction"] = "NO";

                            if (DisputeAct == true)
                            {
                                RowSelected["DisputeAct"] = "YES";
                            }
                            else RowSelected["DisputeAct"] = "NO";

                            if (ManualAct == true)
                            {
                                RowSelected["ManualAct"] = "YES";
                            }
                            else RowSelected["ManualAct"] = "NO";

                            //RowSelected["ManualAct"] = ManualAct;

                            RowSelected["DateTime"] = DateTime;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["UserComment"] = UserComment;
                            //                 
                            RowSelected["SessionNo"] = SesNo;

                            RowSelected["NoteSerialId"] = NoteSerialId;
                           
                            RowSelected["DrCust"] = DrCust;
                            RowSelected["CrCust"] = CrCust;
                            RowSelected["CustAccNo"] = CustAccNo;
                           
                            RowSelected["DrAtmCash"] = DrAtmCash;
                            RowSelected["CrAtmCash"] = CrAtmCash;
                            RowSelected["AccountNo1"] = AccountNo1;

                            RowSelected["DrAtmSusp"] = DrAtmSusp;
                            RowSelected["CrAtmSusp"] = CrAtmSusp;
                            RowSelected["AccountNo2"] = AccountNo2;

                            RowSelected["Mask"] = "";

                            RowSelected["ActionTaken"] = "";
                            RowSelected["UserId"] = InUser;

                            TotalSelected = TotalSelected + 1; 
                            // ADD ROW
                            ErrorsTable.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //InsertReport(InOperator, InUser);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        //
        //
        // READ Errors to fill table  by Filter 
        //
        //
        //public int NotInTransactionPoolYet; 
        //public void ReadErrorsAndDiscrepanciesAndFillTable(string InOperator, string InUser, string InFilter
        //                                                   ,string InAtmNo, DateTime InDtFrom, DateTime InDtTo)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    ErrorsTable = new DataTable();
        //    ErrorsTable.Clear();
        //    TotalSelected = 0;

        //    //NotInTransactionPoolYet = 0; 

        //    // DATA TABLE ROWS DEFINITION 

        //    ErrorsTable.Columns.Add("ExcNo", typeof(int));
        //    ErrorsTable.Columns.Add("AtmNo", typeof(string));
        //    ErrorsTable.Columns.Add("Descrip", typeof(string));
        //    ErrorsTable.Columns.Add("Card", typeof(string));
        //    ErrorsTable.Columns.Add("Ccy", typeof(string));
        //    ErrorsTable.Columns.Add("Amount", typeof(string));

        //    ErrorsTable.Columns.Add("NeedAction", typeof(string));
        //    ErrorsTable.Columns.Add("UnderAction", typeof(string));
        //    ErrorsTable.Columns.Add("DisputeAct", typeof(string));
        //    ErrorsTable.Columns.Add("ManualAct", typeof(string));

        //    ErrorsTable.Columns.Add("UserComment", typeof(string));

        //    ErrorsTable.Columns.Add("NoteSerialId", typeof(string));

        //    ErrorsTable.Columns.Add("DateTime", typeof(DateTime));
        //    ErrorsTable.Columns.Add("TransDescr", typeof(string));

        //    ErrorsTable.Columns.Add("DrCust", typeof(bool));
        //    ErrorsTable.Columns.Add("CrCust", typeof(bool));
        //    ErrorsTable.Columns.Add("CustAccNo", typeof(string));

        //    ErrorsTable.Columns.Add("DrAtmCash", typeof(bool));
        //    ErrorsTable.Columns.Add("CrAtmCash", typeof(bool));
        //    ErrorsTable.Columns.Add("AccountNo1", typeof(string));

        //    ErrorsTable.Columns.Add("DrAtmSusp", typeof(bool));
        //    ErrorsTable.Columns.Add("CrAtmSusp", typeof(bool));
        //    ErrorsTable.Columns.Add("AccountNo2", typeof(string));

        //    ErrorsTable.Columns.Add("SessionNo", typeof(string));

        //    ErrorsTable.Columns.Add("Mask", typeof(string));
        //    ErrorsTable.Columns.Add("ActionTaken", typeof(string)); // Yes, NO
        //    ErrorsTable.Columns.Add("UserId", typeof(string));

        //    SqlString = "SELECT *"
        //                 + " FROM " + ErrorsFileId
        //                 + " WHERE " + InFilter
        //                  + " Order by ErrNo ASC ";

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                // cmd.Parameters.AddWithValue("@Operator", InOperator);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    // Read Fields
        //                    ReadErrorFields(rdr);

        //                    // Fill Table 

        //                    DataRow RowSelected = ErrorsTable.NewRow();

        //                    RowSelected["ExcNo"] = ErrNo;
        //                    RowSelected["AtmNo"] = AtmNo;
        //                    RowSelected["Descrip"] = ErrDesc;
        //                    RowSelected["Card"] = CardNo;
        //                    RowSelected["Ccy"] = CurDes;
        //                    RowSelected["Amount"] = ErrAmount.ToString("#,##0.00");

        //                    if (NeedAction == true)
        //                    {
        //                        RowSelected["NeedAction"] = "YES";
        //                    }
        //                    else RowSelected["NeedAction"] = "NO";
        //                    if (UnderAction == true)
        //                    {
        //                        RowSelected["UnderAction"] = "YES";
        //                    }
        //                    else RowSelected["UnderAction"] = "NO";

        //                    if (DisputeAct == true)
        //                    {
        //                        RowSelected["DisputeAct"] = "YES";
        //                    }
        //                    else RowSelected["DisputeAct"] = "NO";

        //                    if (ManualAct == true)
        //                    {
        //                        RowSelected["ManualAct"] = "YES";
        //                    }
        //                    else RowSelected["ManualAct"] = "NO";

        //                    //RowSelected["ManualAct"] = ManualAct;

        //                    RowSelected["DateTime"] = DateTime;
        //                    RowSelected["TransDescr"] = TransDescr;
        //                    RowSelected["UserComment"] = UserComment;
        //                    //                 
        //                    RowSelected["SessionNo"] = SesNo;

        //                    RowSelected["NoteSerialId"] = NoteSerialId;

        //                    RowSelected["DrCust"] = DrCust;
        //                    RowSelected["CrCust"] = CrCust;
        //                    RowSelected["CustAccNo"] = CustAccNo;

        //                    RowSelected["DrAtmCash"] = DrAtmCash;
        //                    RowSelected["CrAtmCash"] = CrAtmCash;
        //                    RowSelected["AccountNo1"] = AccountNo1;

        //                    RowSelected["DrAtmSusp"] = DrAtmSusp;
        //                    RowSelected["CrAtmSusp"] = CrAtmSusp;
        //                    RowSelected["AccountNo2"] = AccountNo2;

        //                    RowSelected["Mask"] = "";

        //                    RowSelected["ActionTaken"] = "";
        //                    RowSelected["UserId"] = InUser;

        //                    TotalSelected = TotalSelected + 1;
        //                    // ADD ROW
        //                    ErrorsTable.Rows.Add(RowSelected);

        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();

        //            //InsertReport(InOperator, InUser);
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchDetails(ex);
        //        }
        //    //
        //    // Check for descrepancies 
        //    // For unmatched

        //    RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 
            
        //    SqlString = "SELECT * "
        //                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]" 
        //                 + " WHERE TerminalId = @TerminalId AND TransDate Between @DateFrom and @DateTo "
        //                 + " AND Matched = 0  AND MatchMask <> '' "
        //                  + " Order by SeqNo ";

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
        //                cmd.Parameters.AddWithValue("@DateFrom", InDtFrom);
        //                cmd.Parameters.AddWithValue("@DateTo", InDtTo);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    // Read Fields
        //                    ReadErrorFields(rdr);

        //                    // Fill Table 

        //                    DataRow RowSelected = ErrorsTable.NewRow();

        //                    RowSelected["ExcNo"] = ErrNo;
        //                    RowSelected["AtmNo"] = AtmNo;
        //                    RowSelected["Descrip"] = ErrDesc;
        //                    RowSelected["Card"] = CardNo;
        //                    RowSelected["Ccy"] = CurDes;
        //                    RowSelected["Amount"] = ErrAmount.ToString("#,##0.00");

        //                    if (NeedAction == true)
        //                    {
        //                        RowSelected["NeedAction"] = "YES";
        //                    }
        //                    else RowSelected["NeedAction"] = "NO";
        //                    if (UnderAction == true)
        //                    {
        //                        RowSelected["UnderAction"] = "YES";
        //                    }
        //                    else RowSelected["UnderAction"] = "NO";

        //                    if (DisputeAct == true)
        //                    {
        //                        RowSelected["DisputeAct"] = "YES";
        //                    }
        //                    else RowSelected["DisputeAct"] = "NO";

        //                    if (ManualAct == true)
        //                    {
        //                        RowSelected["ManualAct"] = "YES";
        //                    }
        //                    else RowSelected["ManualAct"] = "NO";

        //                    //RowSelected["ManualAct"] = ManualAct;

        //                    RowSelected["DateTime"] = DateTime;
        //                    RowSelected["TransDescr"] = TransDescr;
        //                    RowSelected["UserComment"] = UserComment;
        //                    //                 
        //                    RowSelected["SessionNo"] = SesNo;

        //                    RowSelected["NoteSerialId"] = NoteSerialId;

        //                    RowSelected["DrCust"] = DrCust;
        //                    RowSelected["CrCust"] = CrCust;
        //                    RowSelected["CustAccNo"] = CustAccNo;

        //                    RowSelected["DrAtmCash"] = DrAtmCash;
        //                    RowSelected["CrAtmCash"] = CrAtmCash;
        //                    RowSelected["AccountNo1"] = AccountNo1;

        //                    RowSelected["DrAtmSusp"] = DrAtmSusp;
        //                    RowSelected["CrAtmSusp"] = CrAtmSusp;
        //                    RowSelected["AccountNo2"] = AccountNo2;

        //                    RowSelected["Mask"] = "";

        //                    RowSelected["ActionTaken"] = "";
        //                    RowSelected["UserId"] = InUser;

        //                    TotalSelected = TotalSelected + 1;
        //                    // ADD ROW
        //                    ErrorsTable.Rows.Add(RowSelected);

        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();

        //            //InsertReport(InOperator, InUser);
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchDetails(ex);
        //        }
        //}

        //
        //
        // READ Errors to fill table  by Range of dates 
        //
        //
        //public int NotInTransactionPoolYet; 
        public void ReadErrorsAndFillTableByDates(string InOperator, string InUser, string InAtmNo , DateTime InDateFrom, DateTime InDateTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorsTable = new DataTable();
            ErrorsTable.Clear();
            TotalSelected = 0;

            //NotInTransactionPoolYet = 0; 

            // DATA TABLE ROWS DEFINITION 

            ErrorsTable.Columns.Add("ExcNo", typeof(int));
            ErrorsTable.Columns.Add("AtmNo", typeof(string));
            ErrorsTable.Columns.Add("Descrip", typeof(string));
            ErrorsTable.Columns.Add("Card", typeof(string));
            ErrorsTable.Columns.Add("Ccy", typeof(string));
            ErrorsTable.Columns.Add("Amount", typeof(string));

            ErrorsTable.Columns.Add("NeedAction", typeof(string));
            ErrorsTable.Columns.Add("UnderAction", typeof(string));
            ErrorsTable.Columns.Add("DisputeAct", typeof(string));
            ErrorsTable.Columns.Add("ManualAct", typeof(string));

            ErrorsTable.Columns.Add("UserComment", typeof(string));

            ErrorsTable.Columns.Add("NoteSerialId", typeof(string));

            ErrorsTable.Columns.Add("DateTime", typeof(DateTime));
            ErrorsTable.Columns.Add("TransDescr", typeof(string));

            ErrorsTable.Columns.Add("DrCust", typeof(bool));
            ErrorsTable.Columns.Add("CrCust", typeof(bool));
            ErrorsTable.Columns.Add("CustAccNo", typeof(string));

            ErrorsTable.Columns.Add("DrAtmCash", typeof(bool));
            ErrorsTable.Columns.Add("CrAtmCash", typeof(bool));
            ErrorsTable.Columns.Add("AccountNo1", typeof(string));

            ErrorsTable.Columns.Add("DrAtmSusp", typeof(bool));
            ErrorsTable.Columns.Add("CrAtmSusp", typeof(bool));
            ErrorsTable.Columns.Add("AccountNo2", typeof(string));

            ErrorsTable.Columns.Add("SessionNo", typeof(string));

            ErrorsTable.Columns.Add("Mask", typeof(string));
            ErrorsTable.Columns.Add("ActionTaken", typeof(string)); // Yes, NO
            ErrorsTable.Columns.Add("UserId", typeof(string));

            SqlString = "SELECT *"
                         + " FROM " + ErrorsFileId
                         + " WHERE AtmNo = @AtmNo AND DateTime Between @DateFrom AND @DateTo "
                          + " Order by ErrNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Fields
                            ReadErrorFields(rdr);

                            // Fill Table 

                            DataRow RowSelected = ErrorsTable.NewRow();

                            RowSelected["ExcNo"] = ErrNo;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["Descrip"] = ErrDesc;
                            RowSelected["Card"] = CardNo;
                            RowSelected["Ccy"] = CurDes;
                            RowSelected["Amount"] = ErrAmount.ToString("#,##0.00");

                            if (NeedAction == true)
                            {
                                RowSelected["NeedAction"] = "YES";
                            }
                            else RowSelected["NeedAction"] = "NO";
                            if (UnderAction == true)
                            {
                                RowSelected["UnderAction"] = "YES";
                            }
                            else RowSelected["UnderAction"] = "NO";

                            if (DisputeAct == true)
                            {
                                RowSelected["DisputeAct"] = "YES";
                            }
                            else RowSelected["DisputeAct"] = "NO";

                            if (ManualAct == true)
                            {
                                RowSelected["ManualAct"] = "YES";
                            }
                            else RowSelected["ManualAct"] = "NO";

                            //RowSelected["ManualAct"] = ManualAct;

                            RowSelected["DateTime"] = DateTime;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["UserComment"] = UserComment;
                            //                 
                            RowSelected["SessionNo"] = SesNo;

                            RowSelected["NoteSerialId"] = NoteSerialId;

                            RowSelected["DrCust"] = DrCust;
                            RowSelected["CrCust"] = CrCust;
                            RowSelected["CustAccNo"] = CustAccNo;

                            RowSelected["DrAtmCash"] = DrAtmCash;
                            RowSelected["CrAtmCash"] = CrAtmCash;
                            RowSelected["AccountNo1"] = AccountNo1;

                            RowSelected["DrAtmSusp"] = DrAtmSusp;
                            RowSelected["CrAtmSusp"] = CrAtmSusp;
                            RowSelected["AccountNo2"] = AccountNo2;

                            RowSelected["Mask"] = "";

                            RowSelected["ActionTaken"] = "";
                            RowSelected["UserId"] = InUser;

                            TotalSelected = TotalSelected + 1;
                            // ADD ROW
                            ErrorsTable.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //InsertReport(InOperator, InUser);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }


        //
        //
        // READ Errors to fill table  
        //
        //
        //public int NotInTransactionPoolYet; 
        public void ReadErrorsAndFillTableFrom_Form52c(string InOperator, string InUser, string InFilter, DateTime InReplDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorsTable = new DataTable();
            ErrorsTable.Clear();
            TotalSelected = 0;

            //NotInTransactionPoolYet = 0; 

            // DATA TABLE ROWS DEFINITION 

            ErrorsTable.Columns.Add("ExcNo", typeof(int));
            ErrorsTable.Columns.Add("AtmNo", typeof(string));
            ErrorsTable.Columns.Add("Descrip", typeof(string));
            ErrorsTable.Columns.Add("Card", typeof(string));
            ErrorsTable.Columns.Add("Ccy", typeof(string));
            ErrorsTable.Columns.Add("Amount", typeof(string));
            ErrorsTable.Columns.Add("NeedAction", typeof(string));
            ErrorsTable.Columns.Add("UnderAction", typeof(string));

            ErrorsTable.Columns.Add("DisputeAct", typeof(string));
            ErrorsTable.Columns.Add("ManualAct", typeof(string));
            ErrorsTable.Columns.Add("DateTime", typeof(DateTime));
            ErrorsTable.Columns.Add("TransDescr", typeof(string));
            ErrorsTable.Columns.Add("UserComment", typeof(string));

            ErrorsTable.Columns.Add("SessionNo", typeof(string));
            ErrorsTable.Columns.Add("FirstEntry", typeof(string));
            ErrorsTable.Columns.Add("FirstEntryAccno", typeof(string));

            ErrorsTable.Columns.Add("SecondEntry", typeof(string));
            ErrorsTable.Columns.Add("SecondEntryAccno", typeof(string));

            ErrorsTable.Columns.Add("Mask", typeof(string));
            ErrorsTable.Columns.Add("ActionTaken", typeof(string)); // Yes, NO
            ErrorsTable.Columns.Add("UserId", typeof(string));

            SqlString = "SELECT *"
                         + " FROM " + ErrorsFileId
                         + " WHERE " + InFilter
                          + " Order by ErrNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@DateTime", InReplDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadErrorFields(rdr);

                            //// Count if any error is assosciated with transactions that didnt go to matching process
                            //  string Selection = " WHERE UniqueRecordId = " + UniqueRecordId; 
                            //  Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(Selection); 
                            //  if (Mpa.RecordFound == false)
                            //  {
                            //      NotInTransactionPoolYet = NotInTransactionPoolYet + 1 ; 
                            //  }

                            // Fill Table 

                            DataRow RowSelected = ErrorsTable.NewRow();

                            RowSelected["ExcNo"] = ErrNo;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["Descrip"] = ErrDesc;
                            RowSelected["Card"] = CardNo;
                            RowSelected["Ccy"] = CurDes;
                            RowSelected["Amount"] = ErrAmount.ToString("#,##0.00");

                            if (NeedAction == true)
                            {
                                RowSelected["NeedAction"] = "YES";
                            }
                            else RowSelected["NeedAction"] = "NO";
                            if (UnderAction == true)
                            {
                                RowSelected["UnderAction"] = "YES";
                            }
                            else RowSelected["UnderAction"] = "NO";

                            if (DisputeAct == true)
                            {
                                RowSelected["DisputeAct"] = "YES";
                            }
                            else RowSelected["DisputeAct"] = "NO";

                            if (ManualAct == true)
                            {
                                RowSelected["ManualAct"] = "YES";
                            }
                            else RowSelected["ManualAct"] = "NO";

                            //RowSelected["ManualAct"] = ManualAct;

                            RowSelected["DateTime"] = DateTime;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["UserComment"] = UserComment;
                            //                 
                            RowSelected["SessionNo"] = "";
                            RowSelected["FirstEntry"] = "";
                            RowSelected["FirstEntryAccno"] = "";

                            RowSelected["SecondEntry"] = "";
                            RowSelected["SecondEntryAccno"] = "";
                            RowSelected["Mask"] = "";

                            RowSelected["ActionTaken"] = "";
                            RowSelected["UserId"] = "";

                            // ADD ROW
                            ErrorsTable.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //InsertReport(InOperator, InUser);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        //
        //
        // READ Errors to fill table  
        //
        //
        //public int NotInTransactionPoolYet; 
        public void ReadErrorsAndFillTableForReport(string InOperator, string InUser, string InFilter)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            RRDMAccountsClass Acc = new RRDMAccountsClass();


            ErrorsTableReport = new DataTable();
            ErrorsTableReport.Clear();
            TotalSelected = 0;

            //NotInTransactionPoolYet = 0; 

            // DATA TABLE ROWS DEFINITION 

            ErrorsTableReport.Columns.Add("ExcNo", typeof(int));
            ErrorsTableReport.Columns.Add("AtmNo", typeof(string));
            ErrorsTableReport.Columns.Add("Descrip", typeof(string));
            ErrorsTableReport.Columns.Add("Card", typeof(string));
            ErrorsTableReport.Columns.Add("Ccy", typeof(string));
            ErrorsTableReport.Columns.Add("Amount", typeof(decimal));
            ErrorsTableReport.Columns.Add("NeedAction", typeof(string));
            ErrorsTableReport.Columns.Add("UnderAction", typeof(string));

            ErrorsTableReport.Columns.Add("DisputeAct", typeof(string));
            ErrorsTableReport.Columns.Add("ManualAct", typeof(string));
            ErrorsTableReport.Columns.Add("DateTime", typeof(DateTime));
            ErrorsTableReport.Columns.Add("TransDescr", typeof(string));
            ErrorsTableReport.Columns.Add("UserComment", typeof(string));

            ErrorsTableReport.Columns.Add("ActionRMCycle", typeof(int));
            ErrorsTableReport.Columns.Add("FirstEntry", typeof(string));
            ErrorsTableReport.Columns.Add("FirstEntryAccno", typeof(string));

            ErrorsTableReport.Columns.Add("SecondEntry", typeof(string));
            ErrorsTableReport.Columns.Add("SecondEntryAccno", typeof(string));
            ErrorsTableReport.Columns.Add("Mask", typeof(string));
            ErrorsTableReport.Columns.Add("IsOwnCard", typeof(string));
            ErrorsTableReport.Columns.Add("ActionTaken", typeof(string)); // Yes, NO
            ErrorsTableReport.Columns.Add("UserId", typeof(string));

            SqlString = "SELECT *"
                         + " FROM " + ErrorsFileId
                         + " WHERE " + InFilter
                          + " Order by ErrNo DESC ";

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

                            ReadErrorFields(rdr);

                            //// Count if any error is assosciated with transactions that didnt go to matching process
                            //  string Selection = " WHERE UniqueRecordId = " + UniqueRecordId; 
                            //  Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(Selection); 
                            //  if (Mpa.RecordFound == false)
                            //  {
                            //      NotInTransactionPoolYet = NotInTransactionPoolYet + 1 ; 
                            //  }

                            // Fill Table 

                            DataRow RowSelected = ErrorsTableReport.NewRow();

                            RowSelected["ExcNo"] = ErrNo;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["Descrip"] = ErrDesc;
                            RowSelected["Card"] = CardNo;
                            RowSelected["Ccy"] = CurDes;
                            RowSelected["Amount"] = ErrAmount;

                            if (NeedAction == true)
                            {
                                RowSelected["NeedAction"] = "YES";
                            }
                            else RowSelected["NeedAction"] = "NO";
                            if (UnderAction == true)
                            {
                                RowSelected["UnderAction"] = "YES";
                            }
                            else RowSelected["UnderAction"] = "NO";

                            if (DisputeAct == true)
                            {
                                RowSelected["DisputeAct"] = "YES";
                            }
                            else RowSelected["DisputeAct"] = "NO";

                            if (ManualAct == true)
                            {
                                RowSelected["ManualAct"] = "YES";


                                RowSelected["FirstEntry"] = "N/A";
                                RowSelected["FirstEntryAccno"] = "N/A";

                                RowSelected["SecondEntry"] = "N/A ";
                                RowSelected["SecondEntryAccno"] = "N/A";

                            }
                            else
                            {
                                RowSelected["ManualAct"] = "NO";

                                string ATMSuspence = "Not Found";
                                string ATMCash = "Not Found";

                                Acc.ReadAndFindAccount("1000", "", "", Operator, AtmNo, CurDes, "ATM Suspense");

                                if (Acc.RecordFound == true)
                                {
                                    ATMSuspence = Acc.AccNo;
                                }
                                else
                                {
                                    ATMSuspence = "No Suspense for:" + AtmNo;
                                }

                                Acc.ReadAndFindAccount("1000", "", "", Operator, AtmNo, CurDes, "ATM Cash");
                                if (Acc.RecordFound == true)
                                {
                                    ATMCash = Acc.AccNo;
                                }
                                else
                                {
                                    ATMCash = "No ATM Cash for:" + AtmNo;
                                }

                                if (DrCust == true)
                                {
                                    SelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;

                                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                    RowSelected["FirstEntry"] = "DEBIT CUSTOMER ";
                                    RowSelected["FirstEntryAccno"] = Mpa.AccNumber;
                                }
                                if (CrCust == true)
                                {
                                    SelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;

                                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                    RowSelected["FirstEntry"] = " CREDIT CUSTOMER ";
                                    RowSelected["FirstEntryAccno"] = Mpa.AccNumber;
                                }
                                if (DrAtmCash == true)
                                {
                                    RowSelected["SecondEntry"] = "DEBIT ATM CASH ";
                                    RowSelected["SecondEntryAccno"] = ATMCash;
                                }
                                if (CrAtmCash == true)
                                {
                                    RowSelected["SecondEntry"] = "CREDIT ATM CASH ";
                                    RowSelected["SecondEntryAccno"] = ATMCash;
                                }

                                if (DrCust == false & CrCust == false)
                                {
                                    //
                                    // GL TRXN ONLY
                                    //
                                    if (DrAtmCash == true)
                                    {
                                        RowSelected["FirstEntry"] = " DEBIT ATM CASH ";
                                        RowSelected["FirstEntryAccno"] = ATMCash;
                                    }
                                    if (CrAtmCash == true)
                                    {
                                        RowSelected["FirstEntry"] = "CREDIT ATM CASH ";
                                        RowSelected["FirstEntryAccno"] = ATMCash;
                                    }

                                    if (DrAtmSusp == true)
                                    {
                                        RowSelected["SecondEntry"] = " DEBIT ATM SUSPENSE";
                                        RowSelected["SecondEntryAccno"] = ATMSuspence;
                                    }
                                    if (CrAtmSusp == true)
                                    {
                                        RowSelected["SecondEntry"] = " CREDIT ATM SUSPENSE";
                                        RowSelected["SecondEntryAccno"] = ATMSuspence;
                                    }
                                }
                            }


                            //RowSelected["ManualAct"] = ManualAct;

                            RowSelected["DateTime"] = DateTime;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["UserComment"] = UserComment;

                            RowSelected["ActionRMCycle"] = ActionRMCycle;

                            if (ErrId > 500 & ErrId < 600)
                            {
                                // EG 598
                                // ONLY GL ERROR ... THERE IS NO RECORD IN MASTER FILE 
                                RowSelected["Mask"] = "N/A";
                                RowSelected["IsOwnCard"] = "N/A";
                            }
                            else
                            {
                                SelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;

                                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                RowSelected["Mask"] = Mpa.MatchMask;

                                if (Mpa.IsOwnCard)
                                {
                                    RowSelected["IsOwnCard"] = "YES";
                                }
                                else
                                {
                                    RowSelected["IsOwnCard"] = "NO";
                                }
                            }

                            //  
                            if (ActionId > 0)
                            {
                                RowSelected["ActionTaken"] = "YES";
                            }
                            else
                            {
                                RowSelected["ActionTaken"] = "NO";
                            }

                            RowSelected["UserId"] = InUser;

                            // ADD ROW
                            ErrorsTableReport.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //InsertReport(InOperator, InUser);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        // Insert 
        public void InsertReport(string InOperator, string InSignedId, DataTable InTable)
        {

            if (InTable.Rows.Count > 0)
            {
                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport70]";

                            foreach (var column in InTable.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(InTable);
                        }

                        conn.Close();

                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }

        }

        //
        //
        // READ Errors to fill Short table  
        //
        //

        public void ReadErrorsAndFillShortTable(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            ShortErrorsTable = new DataTable();
            ShortErrorsTable.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            ShortErrorsTable.Columns.Add("ExcNo", typeof(int));
            ShortErrorsTable.Columns.Add("Type", typeof(string));
            ShortErrorsTable.Columns.Add("Sign", typeof(string));
            ShortErrorsTable.Columns.Add("Amount", typeof(string));
            ShortErrorsTable.Columns.Add("Desc", typeof(string));
            ShortErrorsTable.Columns.Add("CustAccNo", typeof(string));
            ShortErrorsTable.Columns.Add("DateTime", typeof(DateTime));
            ShortErrorsTable.Columns.Add("TransDescr", typeof(string));

            SqlString = "SELECT *"
                         + " FROM " + ErrorsFileId
                         + " WHERE " + InSelectionCriteria
                          + " Order by ErrNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadErrorFields(rdr);

                            Mpa.ReadInPoolTransSpecificUniqueRecordId(UniqueRecordId, 1); 

                            TotalSelected = TotalSelected + 1;

                            DataRow RowSelected = ShortErrorsTable.NewRow();

                            RowSelected["ExcNo"] = ErrNo;
                            if (ManualAct == true)
                            {
                                RowSelected["Type"] = "Manual";
                            }
                            else
                            {
                                if (Mpa.ActionType == "1")
                                RowSelected["Type"] = "Default";
                                if (Mpa.ActionType == "9")
                                    RowSelected["Type"] = "To Suspense";
                            }
                            if (Mpa.ActionType == "9")
                            {
                                //DrAtmCash = (bool)rdr["DrAtmCash"];
                                //CrAtmCash = (bool)rdr["CrAtmCash"];
                                //AccountNo1 = rdr["AccountNo1"].ToString();

                                RowSelected["Sign"] = "N/A";
                                if (DrAtmSusp == true) RowSelected["Sign"] = "DR";
                                if (CrAtmSusp == true) RowSelected["Sign"] = "CR";
                            }
                            else
                            {
                                //DrCust = (bool)rdr["DrCust"];
                                //CrCust = (bool)rdr["CrCust"];
                                RowSelected["Sign"] = "N/A";
                                if (DrCust == true) RowSelected["Sign"] = "DR";
                                if (CrCust == true) RowSelected["Sign"] = "CR";
                            }
                           
                            
                            RowSelected["Amount"] = ErrAmount.ToString("#,##0.00");
                            RowSelected["Desc"] = ErrDesc;
                            RowSelected["CustAccNo"] = CustAccNo;
                            RowSelected["DateTime"] = DateTime;

                            // ADD ROW
                            ShortErrorsTable.Rows.Add(RowSelected);

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
        // READ Errors TO Effect On GL For this particular RMCyCle and SesNo
        //
        public decimal TotalOnErrorsAmt;
        public decimal ReadAllErrorsTableToFindGL_Total(string InAtmNo, int InReplCycle, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalOnErrorsAmt = 0;

            string SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE AtmNo = @AtmNo AND SesNo =@SesNo "
          + " AND ActionRMCycle = @ActionRMCycle"
          + " AND (DrAtmCash = 1 OR CrAtmCash = 1) ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InReplCycle);

                        cmd.Parameters.AddWithValue("@ActionRMCycle", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadErrorFields(rdr);

                            NumberOfErrors = NumberOfErrors + 1;

                            if (CrAtmCash == true)
                            {
                                ErrAmount = -ErrAmount;
                            }

                            TotalOnErrorsAmt = TotalOnErrorsAmt + ErrAmount;

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

            return TotalOnErrorsAmt;
        }

        // 
        // READ Totals For particular Txn By UniqueTxnNo
        // For Presenter, Suspect and Fake Notes
        //
        public int Total_Errors;
        public int Total_ErrId_55;
        public decimal Total_ErrId_55_Value;
        public int Total_ErrId_225;
        public decimal Total_ErrId_225_Value;
        public int Total_ErrId_226;
        public decimal Total_ErrId_226_Value;
        public int TotalOutstanding;

        public void ReadAllErrorsTableToFindTotalsForSuspectAndFake(string InAtmNo, int InReplCycle, int InUniqueRecordId, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 1 = all for atm for repl cycle
            // InMode = 2 = Unique number

            Total_ErrId_55 = 0;
            Total_ErrId_225 = 0;
            Total_ErrId_226 = 0;
            Total_ErrId_55_Value = 0;
            Total_ErrId_225_Value = 0;
            Total_ErrId_226_Value = 0;

            TotalOutstanding = 0; 

            if (InMode == 1)
            {
                SqlString = "SELECT *"
                    + " FROM [dbo].[ErrorsTable] "
                    + " WHERE AtmNo = @AtmNo AND SesNo =@SesNo "
                    + " AND (ErrId = 55 OR ErrId = 225 OR ErrId = 226) ";
            }

            if (InMode == 2)
            {
                SqlString = "SELECT *"
                    + " FROM [dbo].[ErrorsTable] "
                    + " WHERE AtmNo = @AtmNo AND SesNo =@SesNo "
                    + " AND UniqueRecordId = @UniqueRecordId"
                    + " AND (ErrId =55 OR ErrId =225 OR ErrId =226) ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InReplCycle);

                        if (InMode == 2)
                        {
                            cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                        }
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read fields
                            ReadErrorFields(rdr);

                            Total_Errors = Total_Errors + 1; 

                            if (ErrId == 55)
                            {
                                Total_ErrId_55 = Total_ErrId_55 + 1;
                                Total_ErrId_55_Value = Total_ErrId_55_Value + ErrAmount;
                            }

                            if (ErrId == 225)
                            {
                                Total_ErrId_225 = Total_ErrId_225 + 1;
                                Total_ErrId_225_Value = Total_ErrId_225_Value + ErrAmount;
                            }

                            if (ErrId == 226)
                            {
                                Total_ErrId_226 = Total_ErrId_226 + 1;
                                Total_ErrId_226_Value = Total_ErrId_226_Value + ErrAmount;
                            }

                            if (UnderAction == false)
                            TotalOutstanding = TotalOutstanding + 1;

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

        // Read Open Presented Errors for a particular ATM and see if matched in Mpa  
        // WOperator, WAtmNo, WSesNo,
        public bool ReadErrorsWithPresentedErrorsAndCheckIfRecordsAreMatched(string InOperator, string InAtmNo, int InSesNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool AreAllMatched = true;

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            ErrorsTableAllFields = new DataTable();
            ErrorsTableAllFields.Clear();
            TotalSelected = 0;

            SqlString = "SELECT *"
                        + " FROM " + ErrorsFileId
                        + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND SesNo<=@SesNo and SesNo > 0 AND OpenErr = 1 AND ErrId < 100 "
                         + " ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@SesNo", InSesNo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.Fill(ErrorsTableAllFields);

                        // Close conn
                        conn.Close();

                        // For two testing ATMs four records in table are created 
                        // Matched and Unmatched for each ATM 

                        int I = 0;

                        while (I <= (ErrorsTableAllFields.Rows.Count - 1))
                        {

                            RecordFound = true;

                            UniqueRecordId = (int)ErrorsTableAllFields.Rows[I]["UniqueRecordId"];

                            Mpa.ReadInPoolTransSpecificUniqueRecordId(UniqueRecordId,1);

                            if (Mpa.Matched == false)

                            {
                                AreAllMatched = false;
                                break;
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
            return AreAllMatched;
        }

        // Read Open Presented Errors for a particular ATM and Session
        // WOperator, WAtmNo, WSesNo,
        public bool AllPresenterMatched;
        public int TotalPresenter;
        public int TotalPresenterUnderAction;
        public Decimal TotalPresentedAmt;
        public int TolalErrorsUnderAction;
        public void ReadPresentedErrorsForParticularAtmAndSession(string InOperator, string InAtmNo, int InSesNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            AllPresenterMatched = false;

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            TotalPresenter = 0;

            PresenterErrorsAmt = 0;

            TolalErrorsUnderAction = 0;
            TotalPresenterUnderAction = 0;

            TotalSelected = 0;

            SqlString = "SELECT *"
                        + " FROM " + ErrorsFileId
                        + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND SesNo <= @SesNo AND OpenErr = 1 "
                         + " ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details
                            ReadErrorFields(rdr);

                            if (UnderAction == true) TolalErrorsUnderAction = TolalErrorsUnderAction + 1;

                            if (ErrId == 55)
                            {
                                if (UnderAction == true) TotalPresenterUnderAction = TotalPresenterUnderAction + 1;

                                TotalPresenter = TotalPresenter + 1;
                                TotalPresentedAmt = TotalPresentedAmt + ErrAmount;

                                Mpa.ReadInPoolTransSpecificUniqueRecordId(UniqueRecordId,1);
                                if (Mpa.IsMatchingDone == true & (Mpa.MatchMask == "111" || Mpa.MatchMask == "11"))
                                {
                                    AllPresenterMatched = true;
                                }
                                else
                                {
                                    AllPresenterMatched = false;
                                    break;
                                }

                                PresenterErrorsAmt = PresenterErrorsAmt + ErrAmount;
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
                            ReadErrorFields(rdr);

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

        // READ Errors by card number and trace number 
        //
        public void ReadErrorsByCardNoAndTrace(string InOperator, string InCardNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE Operator = @Operator AND CardNo = @CardNo AND TraceNo =@TraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CardNo", InCardNo);
                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details

                            ReadErrorFields(rdr);

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

        // READ Errors by card number ONLY  - with Full card or Bin only  
        //
        public void ReadErrorsByCardNo(string InOperator, int InSelectMode, string InEnteredId, DateTime InDateStart, DateTime InDateEnd, string InCardNoBin)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InSelectMode == 8) // Crad 
            {
                SqlString = "SELECT *"
            + " FROM [dbo].[ErrorsTable] "
            + " WHERE Operator=@Operator and DateTime >=@InDateStart AND DateTime <= @InDateEnd AND (CardNo = @CardNo OR  CardNo = @CardNoBin)  ";

            }

            if (InSelectMode == 9) // Account number  
            {
                SqlString = "SELECT *"
            + " FROM [dbo].[ErrorsTable] "
            + " WHERE Operator=@Operator and DateTime >=@InDateStart AND DateTime <= @InDateEnd  and CustAccNo =@AccNo  ";

            }

            if (InSelectMode == 10) // Trace Number which is numeric 
            {

                InTraceNumber = Convert.ToInt32(InEnteredId);

                SqlString = "SELECT *"
           + " FROM [dbo].[ErrorsTable] "
           + " WHERE Operator=@Operator and TraceNo=@TraceNo  ";


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
                        cmd.Parameters.AddWithValue("@InDateStart", InDateStart);
                        cmd.Parameters.AddWithValue("@InDateEnd", InDateEnd);
                        if (InSelectMode == 8) // Card 
                        {
                            cmd.Parameters.AddWithValue("@CardNo", InEnteredId);
                            cmd.Parameters.AddWithValue("@CardNoBin", InCardNoBin);
                        }

                        if (InSelectMode == 9) // Account
                        {
                            cmd.Parameters.AddWithValue("@AccNo", InEnteredId);
                        }
                        if (InSelectMode == 10) // Trace Number  
                        {
                            cmd.Parameters.AddWithValue("@TraceNo", InTraceNumber);
                        }


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details
                            ReadErrorFields(rdr);

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

        // READ Error specific Trace No 
        public void ReadErrorsTableSpecificTraceNo(string InOperator, string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND TraceNo = @TraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details
                            ReadErrorFields(rdr);

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
        // READ Error specific by UniqueRecordId
        //
        public void ReadErrorsTableSpecificByUniqueRecordId(int InUniqueRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE UniqueRecordId = @UniqueRecordId";
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

                            // Read error Details
                            ReadErrorFields(rdr);

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
                            + "CardNo=@CardNo, "
                            + "FullCard=@FullCard, UnderAction=@UnderAction, DisputeAct=@DisputeAct,"
                            + " ManualAct = @ManualAct, UserComment = @UserComment,"
                            + " ByWhom = @ByWhom, ActionDtTm = @ActionDtTm, "
                            + " ActionSesNo = @ActionSesNo, ActionRMCycle = @ActionRMCycle,"
                                       + " Printed = @Printed, DatePrinted = @DatePrinted,"
                                       + " AuthoriserId = @AuthoriserId ,"
                 
                                           + " DrCust = @DrCust ,"
                                             + " CrCust = @CrCust ,"
                                               + " CustAccNo = @CustAccNo ,"
                 
                                                  + " DrAtmCash = @DrAtmCash ,"
                                                   + " CrAtmCash = @CrAtmCash ,"
                                                     + " AccountNo1 = @AccountNo1 ,"
                   
                                                       + " DrAtmSusp = @DrAtmSusp ,"
                                                         + " CrAtmSusp = @CrAtmSusp ,"
                                                           + " AccountNo2 = @AccountNo2 ,"
                                                        
                                       + "CitId = @CitId"
                            + " WHERE ErrNo=@ErrNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);

                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                       
                        cmd.Parameters.AddWithValue("@FullCard", FullCard);
                        cmd.Parameters.AddWithValue("@UnderAction", UnderAction);
                        cmd.Parameters.AddWithValue("@DisputeAct", DisputeAct);

                        cmd.Parameters.AddWithValue("@ManualAct", ManualAct);
                        cmd.Parameters.AddWithValue("@UserComment", UserComment);

                        cmd.Parameters.AddWithValue("@ByWhom", ByWhom);
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);
                        cmd.Parameters.AddWithValue("@ActionSesNo", ActionSesNo);
                        cmd.Parameters.AddWithValue("@ActionRMCycle", ActionRMCycle);

                        cmd.Parameters.AddWithValue("@Printed", Printed);
                        cmd.Parameters.AddWithValue("@DatePrinted", DatePrinted);

                        cmd.Parameters.AddWithValue("@AuthoriserId", AuthoriserId);
                        // 
                   
                        cmd.Parameters.AddWithValue("@DrCust", DrCust);
                        cmd.Parameters.AddWithValue("@CrCust", CrCust);
                        cmd.Parameters.AddWithValue("@CustAccNo", CustAccNo);

                        cmd.Parameters.AddWithValue("@DrAtmCash", DrAtmCash);
                        cmd.Parameters.AddWithValue("@CrAtmCash", CrAtmCash);
                        cmd.Parameters.AddWithValue("@AccountNo1", AccountNo1);

                        cmd.Parameters.AddWithValue("@DrAtmSusp", DrAtmSusp);
                        cmd.Parameters.AddWithValue("@CrAtmSusp", CrAtmSusp);
                        cmd.Parameters.AddWithValue("@AccountNo2", AccountNo2);
                        //
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
        // DO OR UNDO EXPRESS --- Turn errors UnderAction = true OR Under Action == false
        // 
        // 
        public void UpdatePresenterErrorsWithChangeUnderAction(string InOperator, string InAtmNo, int InSesNo, bool InUnderAction)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ErrorsTable SET  "
                            + "UnderAction = @UnderAction "
                             + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND SesNo<=@SesNo and SesNo > 0 AND OpenErr = 1 AND ErrId = 55 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@UnderAction", InUnderAction);

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
        // DO OR UNDO EXPRESS --- Turn errors UnderAction = true OR Under Action == false
        // 
        // 
        public int UpdatePresenterErrorsWith_Un_Do(string InOperator, string InAtmNo, int InSesNo, bool InUnderAction)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int NumberUpdated = 0;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ErrorsTable SET  "
                            + "UnderAction = @UnderAction "
                             + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND SesNo<=@SesNo and SesNo > 0 AND OpenErr = 1 AND ErrId = 55 AND DisputeAct = 0 AND ManualAct = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@UnderAction", InUnderAction);

                        NumberUpdated = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return NumberUpdated;
        }
        // 
        // Update All Errors with the new SesNo where sesno = zero 
        // 
        // 
        public void UpdateAllPresenterErrorsWithNewSessionNumber(string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int rows;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ErrorsTable SET  "
                            + "SesNo = @SesNo "
                             + " WHERE AtmNo = @AtmNo AND SesNo= 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

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
        // 
        // Update All Errors with SesNo 
        // 
        // 
        public void UpdateErrorsWithReplCycleNo(string InAtmNo, DateTime InDtFrom, DateTime InDtTo,  int InSesNo)
        {

        
            ErrorFound = false;
            ErrorOutput = "";
            int rows;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ErrorsTable SET  "
                            + "SesNo = @SesNo "
                             + " WHERE AtmNo = @AtmNo  AND DateTime BETWEEN @DateFrom AND @DateTo "
                             , conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@DateFrom", InDtFrom);

                        cmd.Parameters.AddWithValue("@DateTo", InDtTo);

                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

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
        // 
        // UPDATE all old errors < 100 with main only
        // WHEN ERROR BECOMES OLD IT CANNOT INFLUENCE ATM BALANCES 
        // 
        public void UpdateOldErrorsWithMainOnly(string InOperator, string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ErrorsTable SET  "
                            + "MainOnly = @MainOnly "
                             + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND UnderAction = 0 AND SesNo < @SesNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@MainOnly", true);
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
        // READ ALL ACTIVE for ATM AND COUNT THE NUMBER and The ones action has been taken   
        //
        // 
        string AtmNoInError = "";
        int SesNoInError = 0;
        public void ReadAllErrorsTableForCounters(string InOperator, string InCategoryId, string InAtmNo, int InSesNo, string InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            AtmNoInError = InAtmNo;
            SesNoInError = InSesNo;

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            Ta.ReadSessionsStatusTraces(InAtmNo, InSesNo); 

            // InMode Gets Values "All" or Black 
            //string SqlString;

            NumOfErrors = 0;
            ErrUnderAction = 0;
            ErrDisputeAction = 0;
            ErrUnderManualAction = 0;

            NumOfOpenErrorsLess100 = 0;
            TotalErrorsAmtLess100 = 0;

            NumOfOpenErrorsBetween100And200 = 0; // Host Errors 
            TotalErrorsBetween100And200 = 0;

            NumOfOpenErrorsBetween200And300 = 0;
            TotalErrorsBetween200And300 = 0;

            TotalErrorsAmt = 0;
            TotalUnderActionAmt = 0;

            if (InAtmNo != "")
            {
                if (InMode == "All")
                {
                    SqlString = "SELECT *"
                                       + " FROM [dbo].[ErrorsTable] "
                                       + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND OpenErr=1";
                }
                else
                {
                    // Only for this session 
                    SqlString = "SELECT *"
                   + " FROM [dbo].[ErrorsTable] "
                   + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND OpenErr=1 " +
                   " AND DateTime BETWEEN @SesDtTimeStart AND @SesDtTimeEnd " +
                   "";
                }

            }
            else
            {
                SqlString = "SELECT *"
                    + " FROM [dbo].[ErrorsTable] "
                    + " WHERE Operator = @Operator AND CategoryId = @CategoryId AND OpenErr=1";
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

                        if (InAtmNo != "")
                        {
                            if (InMode == "All")
                            {
                                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                                cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                                cmd.Parameters.AddWithValue("@SesDtTimeStart", Ta.SesDtTimeStart);
                                cmd.Parameters.AddWithValue("@SesDtTimeEnd", Ta.SesDtTimeEnd);
                            }

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadErrorFields(rdr);

                            NumOfErrors = NumOfErrors + 1;

                            if (ErrId < 100)
                            {
                                NumOfOpenErrorsLess100 = NumOfOpenErrorsLess100 + 1;
                                TotalErrorsAmtLess100 = TotalErrorsAmtLess100 + ErrAmount;
                            }

                            if (ErrId > 100 & ErrId < 200)
                            {
                                NumOfOpenErrorsBetween100And200 = NumOfOpenErrorsBetween100And200 + 1;
                                TotalErrorsBetween100And200 = TotalErrorsBetween100And200 + ErrAmount;
                            }

                            if (ErrId > 200 & ErrId < 300)
                            {
                                NumOfOpenErrorsBetween200And300 = NumOfOpenErrorsBetween200And300 + 1;
                                TotalErrorsBetween200And300 = TotalErrorsBetween200And300 + ErrAmount;
                            }

                            TotalErrorsAmt = TotalErrorsAmt + ErrAmount;

                            if (UnderAction == true)
                            {
                                ErrUnderAction = ErrUnderAction + 1;
                                TotalUnderActionAmt = TotalUnderActionAmt + ErrAmount;
                            }
                            if (ManualAct == true) ErrUnderManualAction = ErrUnderManualAction + 1;

                            if (DisputeAct == true) ErrDisputeAction = ErrDisputeAction + 1;

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

        // READ ALL CLOSED for ATM AND COUNT THE NUMBER and The ones action has been taken   
        //
        public void ReadAllErrorsTableClosedForCounters(string InOperator, string InCategoryId, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            string SqlString;

            NumOfErrors = 0;
            ErrUnderAction = 0;
            ErrUnderManualAction = 0;

            TotalErrorsAmt = 0;
            TotalUnderActionAmt = 0;

            if (InAtmNo != "")
            {
                SqlString = "SELECT *"
                   + " FROM [dbo].[ErrorsTable] "
                   + " WHERE Operator = @Operator AND AtmNo = @AtmNo ";
            }
            else
            {
                SqlString = "SELECT *"
                    + " FROM [dbo].[ErrorsTable] "
                    + " WHERE Operator = @Operator AND CategoryId = @CategoryId ";
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

                        if (InAtmNo != "")
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        }


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ReadErrorFields(rdr);

                            NumOfErrors = NumOfErrors + 1;


                            TotalErrorsAmt = TotalErrorsAmt + ErrAmount;

                            if (UnderAction == true)
                            {
                                ErrUnderAction = ErrUnderAction + 1;
                                TotalUnderActionAmt = TotalUnderActionAmt + ErrAmount;
                            }
                            if (ManualAct == true) ErrUnderManualAction = ErrUnderManualAction + 1;

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

        // READ ALL ACTIVE for ATM+Repl Cycle number  AND COUNT THE NUMBER and The ones action has been taken   
        // EXCLUDE THE DEPOSITS 

        public void ReadAllErrorsTableForCounterReplCycle(string InOperator, string InAtmNo, int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumOfErrors = 0;
            ErrUnderAction = 0;
            ErrUnderManualAction = 0;

            NumOfErrorsLess200 = 0;

            SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND SesNo=@SesNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadErrorFields(rdr);

                            NumOfErrors = NumOfErrors + 1;

                            if (ErrType == 1 || ErrType == 2) NumOfErrorsLess200 = NumOfErrorsLess200 + 1;
                            if (ErrType == 1) TotalErrorsAmtLess100 = TotalErrorsAmtLess100 + 1;
                            if (UnderAction == true) ErrUnderAction = ErrUnderAction + 1;
                            if (ManualAct == true) ErrUnderManualAction = ErrUnderManualAction + 1;

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

        // AT CLOSING OF RECONCILIATION PROCESS READ Errors TO CREATE THE TRANSACTIONS  
        // ALSO CLOSE ERRORS FOR WHICH MANUAL ACTION WILL BE TAKEN 
        //
        bool boolFromOurAtms;
        bool boolFromJCC;
        string WMainCateg;
        string SelectionCriteria;
        public void ReadAllErrorsTableForPostingTrans(string InOperator, string InCategoryId, string InAtmNo,
                                                                   string InUserId, string InAuthUser, int InActionRMCycle, string InTMaker, string InOriginId, string InOriginName)
        {

            int TargetSystem;

            DateTime DateTimeH;

            string SqlString;

            int AuthCodeH;
            int RefNumbH;
            int RemNoH;

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            WMainCateg = InCategoryId.Substring(0, 4);

            ErrorsTable = new DataTable();
            ErrorsTable.Clear();
            TotalSelected = 0;

            RecordFound = false;

            if (InAtmNo != "") // ATM was input
            {
                SqlString = "SELECT *"
                  + " FROM [dbo].[ErrorsTable] "
                  + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND OpenErr=1"
                  + "   AND ((NeedAction=1 AND UnderAction = 1) OR (NeedAction=1 AND ManualAct = 1)) ";
            }
            else
            {
                SqlString = "SELECT *"
                  + " FROM [dbo].[ErrorsTable] "
                  + " WHERE Operator = @Operator AND CategoryId = @CategoryId AND OpenErr=1"
                  + "   AND ((NeedAction=1 AND UnderAction = 1) OR (NeedAction=1 AND ManualAct = 1)) ";
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

                        if (InAtmNo != "")
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        }
                        else
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        }

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(ErrorsTable);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (ErrorsTable.Rows.Count - 1))
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ErrNo = (int)ErrorsTable.Rows[I]["ErrNo"];
                            ErrId = (int)ErrorsTable.Rows[I]["ErrId"];
                            ErrDesc = (string)ErrorsTable.Rows[I]["ErrDesc"];
                            ErrType = (int)ErrorsTable.Rows[I]["ErrType"];

                            CategoryId = (string)ErrorsTable.Rows[I]["CategoryId"];
                            RMCycle = (int)ErrorsTable.Rows[I]["RMCycle"];
                            UniqueRecordId = (int)ErrorsTable.Rows[I]["UniqueRecordId"];
                            AtmNo = (string)ErrorsTable.Rows[I]["AtmNo"];

                            SesNo = (int)ErrorsTable.Rows[I]["SesNo"];
                            DateInserted = (DateTime)ErrorsTable.Rows[I]["DateInserted"];
                            BankId = (string)ErrorsTable.Rows[I]["BankId"];
                            BranchId = (string)ErrorsTable.Rows[I]["BranchId"];

                            TurboReconc = (bool)ErrorsTable.Rows[I]["TurboReconc"];
                            TraceNo = (int)ErrorsTable.Rows[I]["TraceNo"];
                            CardNo = (string)ErrorsTable.Rows[I]["CardNo"];

                            TransType = (int)ErrorsTable.Rows[I]["TransType"];
                            TransDescr = (string)ErrorsTable.Rows[I]["TransDescr"];
                            DateTime = (DateTime)ErrorsTable.Rows[I]["DateTime"];
                            NeedAction = (bool)ErrorsTable.Rows[I]["NeedAction"];

                            OpenErr = (bool)ErrorsTable.Rows[I]["OpenErr"];
                            FullCard = (bool)ErrorsTable.Rows[I]["FullCard"];
                            UnderAction = (bool)ErrorsTable.Rows[I]["UnderAction"];
                            DisputeAct = (bool)ErrorsTable.Rows[I]["DisputeAct"];

                            ManualAct = (bool)ErrorsTable.Rows[I]["ManualAct"];
                            ByWhom = (string)ErrorsTable.Rows[I]["ByWhom"];
                            ActionDtTm = (DateTime)ErrorsTable.Rows[I]["ActionDtTm"];
                            ActionRMCycle = (int)ErrorsTable.Rows[I]["ActionRMCycle"];

                            CurDes = (string)ErrorsTable.Rows[I]["CurDes"];
                            ErrAmount = (decimal)ErrorsTable.Rows[I]["ErrAmount"];
                            ActionId = (int)ErrorsTable.Rows[I]["ActionId"];
                            DrCust = (bool)ErrorsTable.Rows[I]["DrCust"];

                            CrCust = (bool)ErrorsTable.Rows[I]["CrCust"];
                            CustAccNo = (string)ErrorsTable.Rows[I]["CustAccNo"];
                            DrAtmCash = (bool)ErrorsTable.Rows[I]["DrAtmCash"];
                            CrAtmCash = (bool)ErrorsTable.Rows[I]["CrAtmCash"];

                            AccountNo1 = (string)ErrorsTable.Rows[I]["AccountNo1"];
                            DrAtmSusp = (bool)ErrorsTable.Rows[I]["DrAtmSusp"];
                            CrAtmSusp = (bool)ErrorsTable.Rows[I]["CrAtmSusp"];
                            AccountNo2 = (string)ErrorsTable.Rows[I]["AccountNo2"];

                            ForeignCard = (bool)ErrorsTable.Rows[I]["ForeignCard"];
                            MainOnly = (bool)ErrorsTable.Rows[I]["MainOnly"];
                            UserComment = (string)ErrorsTable.Rows[I]["UserComment"];
                            Printed = (bool)ErrorsTable.Rows[I]["Printed"];

                            DatePrinted = (DateTime)ErrorsTable.Rows[I]["DatePrinted"];
                            CircularDesc = (string)ErrorsTable.Rows[I]["CircularDesc"];
                            CitId = (string)ErrorsTable.Rows[I]["CitId"];
                            Operator = (string)ErrorsTable.Rows[I]["Operator"];
                            //
                            // Close Error ... Do not create transaction
                            //
                            if (ManualAct == true)
                            {
                                ReadErrorsTableSpecific(ErrNo);
                                ByWhom = InUserId;
                                ActionDtTm = DateTime.Now;
                                ActionRMCycle = InActionRMCycle;
                                AuthoriserId = InAuthUser;
                                OpenErr = false;
                                UpdateErrorsTableSpecific(ErrNo);
                            }

                            if (DisputeAct == true)
                            {

                            }
                            // CLEAR OLD TRANSACTION TO BE POSTED 
                            if (OpenErr == true)
                            {
                                Tp.DeleteOldTransToBePosted(AtmNo, ErrNo);
                            }
                            //
                            //Create Transactions
                            //
                            if (UnderAction == true)
                            {
                                TargetSystem = 9; // T24 
                                RemNoH = 0;
                                //  DateTime = DateTime.Now;
                                AuthCodeH = 0;
                                RefNumbH = 0;

                                if (InAtmNo != "")
                                {
                                    // FIND OTHER NEEDED INFORMATION
                                    // Such as Target system, If foreign find authorization code etc.                     

                                    boolFromOurAtms = true;
                                    TargetSystem = 9;


                                    //Tp.ReadInPoolAtmTrace(AtmNo, TraceNo);

                                    Tp.OriginId = InOriginId; // *
                                    Tp.OriginName = InOriginName;

                                    DateTimeH = DateTime;

                                    AuthCodeH = 0;
                                    RefNumbH = 0;
                                    RemNoH = 0;

                                    if (ErrId == 598)
                                    {
                                        // No need to update 
                                    }
                                    else
                                    {
                                        //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                                        SelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;
                                        Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);


                                        TargetSystem = Mpa.TargetSystem;

                                        DateTime = Mpa.TransDate;
                                    }

                                }
                                else
                                {
                                    //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                                    SelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;
                                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                    if (WMainCateg == "RECA")
                                    {
                                        boolFromOurAtms = true;

                                        RemNoH = 0;

                                        AuthCodeH = 0;
                                        Tp.OriginId = InOriginId; // *
                                        Tp.OriginName = InOriginName;
                                        //WOriginId = "01";
                                        //WOriginName = "OurATMs-" + InCategoryId;
                                    }

                                    if (ErrId == 598)
                                    {
                                        // No need to update 
                                    }
                                    else
                                    {

                                        SelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;
                                        Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                        TargetSystem = Mpa.TargetSystem;

                                        DateTime = Mpa.TransDate;
                                    }

                                }


                                if (DrCust == true & CrAtmCash == true & DisputeAct == false)
                                {
                                    // DElete old for this error 
                                    //   Tp.DeleteOldTransToBePosted(WAtmNo, ErrNo);
                                    // DR CUSTOMER 
                                    Tp.OriginId = InOriginId; // *
                                    Tp.OriginName = InOriginName;
                                    Tp.RMCateg = InCategoryId;
                                    Tp.RMCategCycle = RMCycle;
                                    Tp.UniqueRecordId = UniqueRecordId;

                                    Tp.ErrNo = ErrNo;
                                    Tp.AtmNo = AtmNo;
                                    Tp.SesNo = SesNo;
                                    Tp.BankId = BankId;

                                    Tp.AtmTraceNo = TraceNo;
                                    
                                    Tp.BranchId = BranchId;
                                   
                                    Tp.AtmDtTime = DateTime;
                                   
                                    //Tp.HostDtTime = DateTimeH;
                                    Tp.SystemTarget = TargetSystem;

                                    Tp.CardNo = CardNo;
                                    Tp.CardOrigin = 5; // Find OUT ... 

                                    // First Entry 
                                    Tp.TransType = 11; // MAKE REVERSE
                                    Tp.TransDesc = " Atm Rever For:" + DateTime.ToString();

                                    // If Jcc then use JCC GL SAME for AMex
                                    if (TargetSystem == 1 || TargetSystem == 3) // JCC=1 OR AMEX=3 
                                    {
                                        if (TargetSystem == 1) Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM Suspense");
                                        if (TargetSystem == 3) Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM American Express");

                                        if (Acc.RecordFound == false)
                                        {
                                            ErrorFound = false;
                                            ErrorOutput = "Suspense Account not found for ATMNo: " + AtmNo;
                                            System.Windows.Forms.MessageBox.Show("9849 : Account not found for ATMNo: " + AtmNo);

                                            //return; 
                                        }
                                        Tp.AccNo = Acc.AccNo;  // Cash account No
                                        Tp.GlEntry = true;
                                    }
                                    else
                                    {
                                        Tp.AccNo = CustAccNo;
                                        if (Tp.AccNo == "") Tp.AccNo = "Not Available";
                                        Tp.GlEntry = false;
                                    }

                                    // Second Entry

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType2 = 21; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType2 = 22; // MAINFRAME CASH ONLY  
                                    }
                                    Tp.TransDesc2 = " CASH Rever-Trace: " + TraceNo;

                                    Tp.AccNo2 = "No Account Found";
                                    //
                                    // Find ATM Cash account or Category GL Account 
                                    //
                                    if (boolFromOurAtms == true)
                                    {
                                        Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM Cash");
                                        if (Acc.RecordFound == true)
                                        {
                                            Tp.AccNo2 = Acc.AccNo;
                                        }
                                        else
                                        {
                                            Tp.AccNo2 = "No Account Found";
                                            System.Windows.Forms.MessageBox.Show("8365 : Cash Account not found for ATMNo: " + AtmNo);
                                        }
                                    }
                                    if (boolFromJCC == true)
                                    {
                                        RRDMMatchingCategories Rc = new RRDMMatchingCategories();

                                        Rc.ReadMatchingCategorybyActiveCategId(InOperator, InCategoryId);

                                        Tp.AccNo2 = Rc.GlAccount;
                                    }

                                    // Cash account No Cash account No
                                    Tp.GlEntry2 = true;
                                    // End Second Entry 

                                    Tp.CurrDesc = CurDes;
                                    Tp.TranAmount = ErrAmount;
                                    Tp.AuthCode = AuthCodeH;
                                    Tp.RefNumb = RefNumbH;
                                    Tp.RemNo = RemNoH;
                                    Tp.TransMsg = UserComment;
                                    Tp.AtmMsg = "";
                                    Tp.MakerUser = InUserId;
                                    Tp.AuthUser = InAuthUser;
                                    Tp.OpenDate = DateTime.Now;
                                    Tp.OpenRecord = true;
                                    Tp.Operator = Operator;
                                    //NOSTRO
                                    Tp.NostroCcy = "";
                                    Tp.NostroAdjAmt = 0;

                                    int PostedNo = Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo, Tp.OriginName);

                                    // UPDATE AND CLOSE UNMATCHED AND META EXCEPTIONS 
                                    if (Tp.ErrorFound == false & ErrId != 55)
                                    {
                                        //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                                        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                                        RRDMReconcCategATMsAtRMCycles Ratms = new RRDMReconcCategATMsAtRMCycles();
                                        string SelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;
                                        Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                        if (Mpa.RecordFound == true)
                                        {
                                            // Update Reconciliation Record
                                            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(Operator, CategoryId, Mpa.MatchingAtRMCycle);
                                            Rcs.SettledUnMatchedAmtWorkFlow = Rcs.SettledUnMatchedAmtWorkFlow + Mpa.TransAmount;
                                            Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow + 1;
                                            Rcs.RemainReconcExceptions = Rcs.RemainReconcExceptions - 1;
                                            Rcs.UpdateReconcCategorySessionWithAuthorClosing(CategoryId, Mpa.MatchingAtRMCycle);

                                            // Update ATMs Matched
                                            if (InAtmNo != "")
                                            {
                                                Ratms.ReadReconcCategoriesATMsRMCycleSpecific(Operator, Mpa.RMCateg, Mpa.MatchingAtRMCycle, Mpa.TerminalId);
                                                if (Ratms.RecordFound == true)
                                                {
                                                    Ratms.MatchedAmtAtWorkFlow = Ratms.MatchedAmtAtWorkFlow + Mpa.TransAmount;
                                                    Ratms.UpdateReconcCategoriesATMsRMCycleForAtmForAdjusted(Mpa.TerminalId, Mpa.RMCateg, Mpa.MatchingAtRMCycle, Ratms.MatchedAmtAtDefault, Ratms.MatchedAmtAtWorkFlow);
                                                }
                                            }

                                            Mpa.ActionByUser = true;
                                            Mpa.UserId = InUserId;
                                            Mpa.Authoriser = InAuthUser;
                                            Mpa.AuthoriserDtTm = DateTime.Now;

                                            Mpa.SettledRecord = true;
                                            Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Operator, UniqueRecordId,1);

                                        }

                                    }

                                    //UPDATE ERROR RECORD
                                    ReadErrorsTableSpecific(ErrNo);
                                    ByWhom = InUserId;
                                    ActionDtTm = DateTime.Now;
                                    ActionSesNo = SesNo;
                                    ActionRMCycle = InActionRMCycle;
                                    AuthoriserId = InAuthUser;
                                    OpenErr = false;
                                    UpdateErrorsTableSpecific(ErrNo);

                                }

                                if (CrCust == true & DrAtmCash == true & DisputeAct == false)
                                {
                                    // DElete old for this error 
                                    //   Tp.DeleteOldTransToBePosted(WAtmNo, ErrNo);
                                    // CREATE A TRANSACTION TO CR CUSTOMER 

                                    Tp.AccNo2 = "JCC Acc 123";

                                    Tp.OriginId = InOriginId; // *
                                    Tp.OriginName = InOriginName;
                                    Tp.RMCateg = InCategoryId;
                                    Tp.RMCategCycle = RMCycle;
                                    Tp.UniqueRecordId = UniqueRecordId;

                                    Tp.ErrNo = ErrNo;
                                    Tp.AtmNo = AtmNo;
                                    Tp.SesNo = SesNo;
                                    Tp.BankId = BankId;

                                    Tp.AtmTraceNo = TraceNo;

                                    Tp.BranchId = BranchId;
                                    Tp.AtmDtTime = DateTime;
                                    //Tp.HostDtTime = DateTimeH;
                                    Tp.SystemTarget = TargetSystem;

                                    Tp.CardNo = CardNo;
                                    Tp.CardOrigin = 5; // Find OUT

                                    // First Entry 
                                    Tp.TransType = 21; // MAKE REVERSE 
                                    Tp.TransDesc = " Atm Rever For: " + DateTime.ToShortDateString();

                                    if (TargetSystem == 1 || TargetSystem == 3) // JCC=1 OR AMEX=3 
                                    {
                                        if (TargetSystem == 1) Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM Suspense");
                                        if (TargetSystem == 3) Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM American Express");

                                        if (Acc.RecordFound == false)
                                        {
                                            ErrorFound = false;
                                            ErrorOutput = "Account not found for ATMNo: " + AtmNo;
                                            //return; 
                                            System.Windows.Forms.MessageBox.Show("8378 : Account not found for ATMNo: " + AtmNo);
                                            //  MessageBox.Show("Account not found for ATMNo: " + AtmNo);
                                        }
                                        Tp.AccNo = Acc.AccNo;  // Cash account No
                                        Tp.GlEntry = true;
                                    }
                                    else
                                    {
                                        Tp.AccNo = CustAccNo;
                                        if (Tp.AccNo == "") Tp.AccNo = "Not Available";
                                        Tp.GlEntry = false;
                                    }

                                    // Second Transaction 
                                    // CREATE A TRANSACTION TO DR CASH

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType2 = 11; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType2 = 12; // MAINFRAME CASH ONLY  
                                    }

                                    Tp.TransDesc2 = "CASH Rever-Trace: " + TraceNo;

                                    //
                                    // Find ATM Cash account or Category GL Account 
                                    //
                                    if (boolFromOurAtms == true)
                                    {
                                        Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM Cash");
                                        if (Acc.RecordFound == true)
                                        {
                                            Tp.AccNo2 = Acc.AccNo;
                                        }
                                        else
                                        {
                                            Tp.AccNo2 = "No Account Found";
                                            System.Windows.Forms.MessageBox.Show("8367 : Cash Account not found for ATMNo: " + AtmNo);
                                        }
                                    }
                                    if (boolFromJCC == true)
                                    {
                                        RRDMMatchingCategories Rc = new RRDMMatchingCategories();

                                        Rc.ReadMatchingCategorybyActiveCategId(InOperator, InCategoryId);

                                        Tp.AccNo2 = Rc.GlAccount;
                                    }

                                    Tp.GlEntry2 = true;
                                    // End of second 

                                    Tp.CurrDesc = CurDes;
                                    Tp.TranAmount = ErrAmount;
                                    Tp.AuthCode = AuthCodeH;
                                    Tp.RefNumb = RefNumbH;
                                    Tp.RemNo = RemNoH;
                                    Tp.TransMsg = UserComment;
                                    Tp.AtmMsg = "";
                                    Tp.MakerUser = InUserId;
                                    Tp.AuthUser = InAuthUser;
                                    Tp.OpenDate = DateTime.Now;
                                    Tp.OpenRecord = true;
                                    Tp.Operator = InOperator;
                                    //NOSTRO
                                    Tp.NostroCcy = "";
                                    Tp.NostroAdjAmt = 0;

                                    int PostedNo = Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo, Tp.OriginName);

                                    // UPDATE AND CLOSE UNMATCHED AND META EXCEPTIONS 
                                    if (Tp.ErrorFound == false & ErrId != 55)
                                    {
                                        //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                                        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                                        RRDMReconcCategATMsAtRMCycles Ratms = new RRDMReconcCategATMsAtRMCycles();
                                        string SelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;
                                        Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                        if (Mpa.RecordFound == true)
                                        {
                                            // Update Reconciliation Record
                                            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(Operator, CategoryId, Mpa.MatchingAtRMCycle);
                                            Rcs.SettledUnMatchedAmtWorkFlow = Rcs.SettledUnMatchedAmtWorkFlow + Mpa.TransAmount;
                                            Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow + 1;
                                            Rcs.RemainReconcExceptions = Rcs.RemainReconcExceptions - 1;
                                            Rcs.UpdateReconcCategorySessionWithAuthorClosing(CategoryId, Mpa.MatchingAtRMCycle);

                                            // Update ATMs Matched
                                            if (InAtmNo != "")
                                            {
                                                Ratms.ReadReconcCategoriesATMsRMCycleSpecific(Operator, Mpa.RMCateg, Mpa.MatchingAtRMCycle, Mpa.TerminalId);
                                                if (Ratms.RecordFound == true)
                                                {
                                                    Ratms.MatchedAmtAtWorkFlow = Ratms.MatchedAmtAtWorkFlow + Mpa.TransAmount;
                                                    Ratms.UpdateReconcCategoriesATMsRMCycleForAtmForAdjusted(Mpa.TerminalId, Mpa.RMCateg, Mpa.MatchingAtRMCycle, Ratms.MatchedAmtAtDefault, Ratms.MatchedAmtAtWorkFlow);
                                                }
                                            }

                                            Mpa.ActionByUser = true;
                                            Mpa.UserId = InUserId;
                                            Mpa.Authoriser = InAuthUser;
                                            Mpa.AuthoriserDtTm = DateTime.Now;

                                            Mpa.SettledRecord = true;
                                            Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Operator, UniqueRecordId,1);

                                        }

                                    }
                                    //UPDATE ERROR RECORD
                                    ReadErrorsTableSpecific(ErrNo);
                                    ByWhom = InUserId;
                                    ActionDtTm = DateTime.Now;
                                    ActionSesNo = SesNo;
                                    ActionRMCycle = InActionRMCycle;
                                    AuthoriserId = InAuthUser;
                                    OpenErr = false;
                                    UpdateErrorsTableSpecific(ErrNo);

                                }

                                //if ((DrAtmCash == true & CrAtmSusp == true) || (CrCust == true & DrAtmCash == true & DisputeAct == true))
                                if ((DrAtmCash == true & CrAtmSusp == true))
                                {
                                    // CREATE A TRANSACTION TO DR AtmCash
                                    // CREATE A TRANSACTION TO CR Suspense

                                    Tp.OriginId = InOriginId; // *
                                    Tp.OriginName = InOriginName;
                                    Tp.RMCateg = InCategoryId;
                                    Tp.RMCategCycle = RMCycle;
                                    Tp.UniqueRecordId = UniqueRecordId;

                                    Tp.ErrNo = ErrNo;
                                    Tp.AtmNo = AtmNo;
                                    Tp.SesNo = SesNo;
                                    Tp.BankId = BankId;

                                    Tp.AtmTraceNo = TraceNo;

                                    Tp.BranchId = BranchId;
                                    Tp.AtmDtTime = DateTime;
                                    //Tp.HostDtTime = DateTimeH;
                                    Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                                    Tp.CardNo = "N/A";
                                    Tp.CardOrigin = 5; // Find OUT

                                    // First Entry 

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType = 11; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType = 12; // MAINFRAME CASH ONLY  
                                    }

                                    Tp.TransDesc = " Transfer to Suspense";

                                    //
                                    // Find ATM Cash account or Category GL Account 
                                    //
                                    if (boolFromOurAtms == true)
                                    {
                                        Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM Cash");
                                        if (Acc.RecordFound == true)
                                        {
                                            Tp.AccNo = Acc.AccNo;
                                        }
                                        else
                                        {
                                            Tp.AccNo = "No Account Found";
                                            System.Windows.Forms.MessageBox.Show("8355 : Cash Account not found for ATMNo: " + AtmNo);
                                        }
                                    }
                                    if (boolFromJCC == true)
                                    {
                                        RRDMMatchingCategories Rc = new RRDMMatchingCategories();

                                        Rc.ReadMatchingCategorybyActiveCategId(InOperator, InCategoryId);

                                        Tp.AccNo = Rc.GlAccount;
                                    }

                                    Tp.GlEntry = true;

                                    // Second Entry 
                                    // CREATE A TRANSACTION TO CR SUSPENSE 

                                    Tp.TransType2 = 21; // MAKE REVERSE 

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType2 = 21; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType2 = 22; // MAINFRAME CASH ONLY  
                                    }
                                    Tp.TransDesc2 = " Transfer from ATM Cash";
                                    //
                                    // Find ATM Cash account or Category GL Account 
                                    //
                                    if (boolFromOurAtms == true)
                                    {
                                        Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM Suspense");
                                        if (Acc.RecordFound == true)
                                        {
                                            Tp.AccNo2 = Acc.AccNo;
                                        }
                                        else
                                        {
                                            Tp.AccNo2 = "No Account found";
                                            System.Windows.Forms.MessageBox.Show("8355 : Suspense Account not found for ATMNo: " + AtmNo);
                                        }
                                    }
                                    if (boolFromJCC == true)
                                    {
                                        Tp.AccNo2 = "No Account found";
                                        System.Windows.Forms.MessageBox.Show("8352 : JCC Account not found for ATMNo: " + AtmNo);
                                    }

                                    Tp.GlEntry2 = true;

                                    //         Tp.CurrCode = CurrCd;
                                    Tp.CurrDesc = CurDes;
                                    Tp.TranAmount = ErrAmount;
                                    Tp.AuthCode = AuthCodeH;
                                    Tp.RefNumb = RefNumbH;
                                    Tp.RemNo = RemNoH;
                                    Tp.TransMsg = UserComment;
                                    Tp.AtmMsg = "";
                                    Tp.MakerUser = InUserId;
                                    Tp.AuthUser = InAuthUser;
                                    Tp.OpenDate = DateTime.Now;
                                    Tp.OpenRecord = true;
                                    Tp.Operator = InOperator;
                                    //NOSTRO
                                    Tp.NostroCcy = "";
                                    Tp.NostroAdjAmt = 0;

                                    int PostedNo = Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo, Tp.OriginName);

                                    if (Tp.ErrorFound == false)
                                    {
                                        ReadErrorsTableSpecific(ErrNo);
                                        ByWhom = InUserId;
                                        ActionDtTm = DateTime.Now;
                                        ActionSesNo = SesNo;
                                        ActionRMCycle = InActionRMCycle;
                                        AuthoriserId = InAuthUser;
                                        OpenErr = false;
                                        UpdateErrorsTableSpecific(ErrNo);
                                    }

                                }

                                //if ((CrAtmCash == true & DrAtmSusp == true) || (DrCust == true & CrAtmCash == true & DisputeAct == false))
                                if (CrAtmCash == true & DrAtmSusp == true)
                                {
                                    // DElete old for this error 

                                    // CREATE A TRANSACTION TO CR CASH

                                    Tp.OriginId = InOriginId; // *
                                    Tp.OriginName = InOriginName;
                                    Tp.RMCateg = InCategoryId;
                                    Tp.RMCategCycle = RMCycle;
                                    Tp.UniqueRecordId = UniqueRecordId;

                                    Tp.ErrNo = ErrNo;
                                    Tp.AtmNo = AtmNo;
                                    Tp.SesNo = SesNo;
                                    Tp.BankId = BankId;

                                    Tp.AtmTraceNo = TraceNo;

                                    Tp.BranchId = BranchId;
                                    Tp.AtmDtTime = DateTime;
                                    //Tp.HostDtTime = DateTimeH;
                                    Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER
                                    Tp.CardNo = "";
                                    Tp.CardOrigin = 5; // Find OUT

                                    // First Entry 

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType = 21; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType = 22; // MAINFRAME CASH ONLY  
                                    }

                                    Tp.TransDesc = "Transfer to Suspense";
                                    //
                                    // Find ATM Cash account or Category GL Account 
                                    //
                                    if (boolFromOurAtms == true)
                                    {
                                        Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM Cash");
                                        if (Acc.RecordFound == true)
                                        {
                                            Tp.AccNo = Acc.AccNo;
                                        }
                                        else
                                        {
                                            Tp.AccNo = "No Account Found";
                                            System.Windows.Forms.MessageBox.Show("9389 : Account not found for ATMNo: " + AtmNo);
                                        }
                                    }
                                    if (boolFromJCC == true)
                                    {
                                        RRDMMatchingCategories Rc = new RRDMMatchingCategories();

                                        Rc.ReadMatchingCategorybyActiveCategId(InOperator, InCategoryId);

                                        Tp.AccNo = Rc.GlAccount;
                                    }

                                    Tp.GlEntry = true;

                                    // Second Entry 

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType2 = 11; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType2 = 12; // MAINFRAME CASH ONLY  
                                    }

                                    Tp.TransDesc2 = " Transfer from ATM Cash";

                                    if (boolFromOurAtms == true)
                                    {
                                        Acc.ReadAndFindAccount("1000", "", "", InOperator, AtmNo, CurDes, "ATM Suspense");
                                        if (Acc.RecordFound == true)
                                        {
                                            Tp.AccNo2 = Acc.AccNo;
                                        }
                                        else
                                        {
                                            Tp.AccNo2 = "No Account found";
                                            System.Windows.Forms.MessageBox.Show("7872 : Account not found for ATMNo: " + AtmNo);
                                        }
                                    }
                                    if (boolFromJCC == true)
                                    {
                                        Tp.AccNo2 = "No Account found";
                                        System.Windows.Forms.MessageBox.Show("7865 : JCC Account not found for ATMNo: " + AtmNo);
                                    }
                                    Tp.GlEntry2 = true;

                                    Tp.CurrDesc = CurDes;
                                    Tp.TranAmount = ErrAmount;
                                    Tp.AuthCode = AuthCodeH;
                                    Tp.RefNumb = RefNumbH;
                                    Tp.RemNo = RemNoH;
                                    Tp.TransMsg = UserComment;
                                    Tp.AtmMsg = "";
                                    Tp.MakerUser = InUserId;
                                    Tp.AuthUser = InAuthUser;
                                    Tp.OpenDate = DateTime.Now;
                                    Tp.OpenRecord = true;
                                    Tp.Operator = InOperator;
                                    //NOSTRO
                                    Tp.NostroCcy = "";
                                    Tp.NostroAdjAmt = 0;

                                    int PostedNo = Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo, Tp.OriginName);

                                    if (Tp.ErrorFound == false)
                                    {
                                        ReadErrorsTableSpecific(ErrNo);
                                        ByWhom = InUserId;
                                        ActionDtTm = DateTime.Now;
                                        ActionSesNo = SesNo;
                                        ActionRMCycle = InActionRMCycle;
                                        AuthoriserId = InAuthUser;
                                        OpenErr = false;
                                        UpdateErrorsTableSpecific(ErrNo);
                                    }

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

        // AT CLOSING OF RECONCILIATION PROCESS FOR UNMATCHED READ Errors TO CREATE UPDATE THE RECORDS OF TRANSACTIONS  
        // With The Action By and Authirised By 
        //
        public void ReadAllErrorsTableForUpdatingTheUnMatchedTrans(string InOperator, string InRMCateg, int InRMCycle, string InUserId, string InAuthoriser)
        {
            //   RRDMPostedTrans Cs = new RRDMPostedTrans();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE Operator = @Operator AND SesNo = @SesNo AND OpenErr=1";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SesNo", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details
                            ReadErrorFields(rdr);

                            //TEST for EAST WEST BANK 

                            if (UnderAction == true)
                            {
                                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                                string SelectionCriteria = " WHERE UniqueRecordId = " + UniqueRecordId;
                                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                Mpa.ActionByUser = true;
                                Mpa.UserId = InUserId;
                                Mpa.Authoriser = InAuthoriser;
                                Mpa.AuthoriserDtTm = DateTime.Now;
                                Mpa.SettledRecord = true;

                                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Operator, UniqueRecordId,1);
                                // FIND OTHER NEEDED INFORMATION
                                // Such as Traget system, If foreign find authorization code etc. 

                                // UNMATCHED TRansactions with 
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
        // This is called from Disputes 
        //
        public void CreateTransTobepostedfromDisputes(string InUserId, string InAuthUser, int InDispTranNo, int InTranType, decimal InAmount)
        {
            //
            // Create a Credit or a Debit to customer AS A RESULT OF a dispute 
            //
            //

            // Insert Transactions (TWO Trans) In TransTo BePosted 

            RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            ErrorFound = false;
            ErrorOutput = "";

            bool OurAtmTran;

            Dt.ReadDisputeTran(InDispTranNo);

            // Find Details of Masked REcord 
            string SelectionCriteria = " WHERE UniqueRecordId = " + Dt.UniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

            if (Mpa.RecordFound == true)
            {
                //FoundInMatchedUnMatched = true;
            }


            // check if the transaction comes from our ATMs 

            Mpa.ReadInPoolTransSpecificUniqueRecordId(Dt.UniqueRecordId,2);
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
                    Tp.OriginId = "07"; // *
                    Tp.OriginName = "Disputes";  // From Dispute 
                    Tp.RMCateg = "Disputes";
                    Tp.RMCategCycle = Mpa.MatchingAtRMCycle;
                    Tp.UniqueRecordId = Dt.UniqueRecordId;

                    Tp.Operator = Dt.Operator;
                    Tp.AtmNo = Dt.AtmNo;
                    Tp.CurrDesc = Dt.CurrencyNm;

                    Tp.CardNo = Dt.CardNo;
                    Tp.AccNo = Dt.AccNo;

                    Tp.BankId = Mpa.Operator;
                    Tp.AtmTraceNo = Mpa.TraceNoWithNoEndZero;
                    Tp.BranchId = "";

                    Tp.AtmDtTime = Mpa.TransDate;

                    Tp.TransType = 21; // MAKE REVERSE

                    Tp.TransDesc = "Result of Dispute:" + Dt.DisputeNumber.ToString();
                    Tp.GlEntry = false;

                    // Second Entry 
                    Tp.TransType2 = 11; // MAKE REVERSE 

                    Tp.TransDesc2 = "Result of Dispute:" + Dt.DisputeNumber.ToString();
                    if (OurAtmTran == true)
                    {
                        Acc.ReadAndFindAccount("1000", "", "", Tp.Operator, Tp.AtmNo, Tp.CurrDesc, "Disputes Acc");
                        if (Acc.RecordFound == true)
                        {
                            Tp.AccNo2 = Acc.AccNo;
                        }
                        else
                        {
                            Tp.AccNo2 = "NoAccount Found";
                        }
                    }
                    else
                    {
                        Tp.AccNo2 = "GeneralDispute";
                    }

                    Tp.GlEntry2 = true;

                    Tp.TranAmount = InAmount;

                    Tp.OpenDate = DateTime.Now;

                    Tp.TransMsg = Dt.ActionComment;

                    Tp.DisputeNo = Dt.DisputeNumber;
                    Tp.DispTranNo = Dt.DispTranNo;

                    Tp.MakerUser = InUserId;
                    Tp.AuthUser = InAuthUser;
                    Tp.AtmMsg = "";

                    Tp.OpenRecord = true;
                }


                if (OurAtmTran == false) // Not our ATM therefore information is needed
                {
                    Tp.OriginId = "07"; // *
                    Tp.OriginName = "Disputes";  // From Dispute 
                    Tp.RMCateg = "Disputes";
                    Tp.RMCategCycle = 0;
                    Tp.UniqueRecordId = Dt.UniqueRecordId;

                    Tp.ErrNo = Mpa.MetaExceptionNo;
                    Tp.AtmNo = Mpa.TerminalId;
                    Tp.SesNo = 0;
                    Tp.BankId = Mpa.Operator;

                    Tp.AtmTraceNo = Mpa.AtmTraceNo;

                    Tp.BranchId = "Central";
                    Tp.AtmDtTime = Mpa.TransDate;
                    //Tp.HostDtTime = DateTimeH;
                    Tp.SystemTarget = 5;

                    Tp.CardNo = Dt.CardNo;
                    Tp.AccNo = Dt.AccNo;

                    Tp.CardOrigin = 5; // Find OUT

                    // First Entry 
                    Tp.TransType = 21; // MAKE REVERSE 
                    Tp.TransDesc = " Atm Rever For: " + DateTime.Now.ToShortDateString();

                    // Second Entry 
                    Tp.TransType2 = 11; // MAKE REVERSE 

                    Tp.TransDesc2 = "Result of Dispute:" + Dt.DisputeNumber.ToString();
                    if (OurAtmTran == true)
                    {
                        Acc.ReadAndFindAccount("1000", "", "", Tp.Operator, Tp.AtmNo, Tp.CurrDesc, "Disputes Acc");
                        if (Acc.RecordFound == true)
                        {
                            Tp.AccNo2 = Acc.AccNo;
                        }
                        else
                        {
                            Tp.AccNo2 = "No Account Found";
                        }
                    }
                    else
                    {
                        Tp.AccNo2 = "GeneralDispute";
                    }

                    Tp.GlEntry2 = true;

                    Tp.CurrDesc = Mpa.TransCurr;
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
                }
                //NOSTRO
                Tp.NostroCcy = "";
                Tp.NostroAdjAmt = 0;

                int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

            }
            if (InTranType == 11)
            {
                // CREATE A TRANSACTION TO DR CUSTOMER 
                // CREATE A TRANSACTION TO CR CASH

                Tp.OriginId = "07"; // *
                Tp.OriginName = "Disputes";  // From Dispute 
                Tp.RMCateg = "Disputes";
                Tp.RMCategCycle = Mpa.MatchingAtRMCycle;

                Tp.UniqueRecordId = Dt.UniqueRecordId;

                Tp.ErrNo = Mpa.MetaExceptionNo;
                Tp.AtmNo = Mpa.TerminalId;
                Tp.SesNo = 0;
                Tp.BankId = Mpa.Operator;

                Tp.AtmTraceNo = Mpa.AtmTraceNo;

                Tp.BranchId = "Central";
                Tp.AtmDtTime = Mpa.TransDate;

                Tp.CardNo = Dt.CardNo;

                Tp.AccNo = Dt.AccNo;

                Tp.TransType = 11; // MAKE REVERSE

                Tp.TransDesc = "Result of Dispute:" + Dt.DisputeNumber.ToString();

                Tp.GlEntry = false;

                // SECOND ENTRY 
                Tp.TransType2 = 21; // MAKE REVERSE 
                Tp.TransDesc2 = "Result of Dispute:" + Dt.DisputeNumber.ToString();

                if (OurAtmTran == true)
                {
                    Acc.ReadAndFindAccount("1000", "", "", Tp.Operator, Tp.AtmNo, Tp.CurrDesc, "Disputes Acc");
                    if (Acc.RecordFound == true)
                    {
                        Tp.AccNo2 = Acc.AccNo;
                    }
                    else
                    {
                        Tp.AccNo2 = "No Account Found";
                    }
                }
                else
                {
                    Tp.AccNo2 = "GeneralDispute";
                }

                Tp.GlEntry2 = true;

                Tp.TranAmount = InAmount;

                Tp.OpenDate = DateTime.Now;

                Tp.TransMsg = Dt.ActionComment;

                Tp.DisputeNo = Dt.DisputeNumber;
                Tp.DispTranNo = Dt.DispTranNo;

                Tp.AtmMsg = "";
                Tp.Operator = Acc.Operator;

                Tp.MakerUser = InUserId;
                Tp.AuthUser = InAuthUser;

                Tp.OpenRecord = true;
                //NOSTRO
                Tp.NostroCcy = "";
                Tp.NostroAdjAmt = 0;

                int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

            }

        }

        // 
        // This is called from SETTLEMENT AUTHORISATION ACTIONS
        //
        public void CreateTransTobepostedfromForceSettlement(string InUserId, string InAuthUser, string InWhatFile, int InSeqNo, int InTranType, decimal InAmount)
        {
            //
            // Create a Credit or a Debit to customer AS A RESULT OF Force matching 
            //
            //

            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            string SelectionCriteria = " WHERE SeqNo = " + InSeqNo;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

            RRDMMatchingCategories Rc = new RRDMMatchingCategories();

            Rc.ReadMatchingCategorybyActiveCategId(Mpa.Operator, Mpa.RMCateg);

            //
            // If InTranType = 21 means CRCust if InTranType = 11 then means DRCust
            //

            if (InTranType == 21)
            {
                // CREATE A TRANSACTION TO CR CUSTOMER 
                // CREATE A TRANSACTION TO DR RM Category

                Tp.OriginId = "05"; // *
                Tp.OriginName = "Settlement";    // From Dispute 
                Tp.RMCateg = Mpa.RMCateg;
                Tp.RMCategCycle = Mpa.MatchingAtRMCycle;
                Tp.UniqueRecordId = Mpa.UniqueRecordId;

                Tp.AtmNo = "N/A";
                Tp.SesNo = 0;

                Tp.BankId = "Other";
                Tp.AtmTraceNo = 0;
                Tp.BranchId = "Other";
                Tp.AtmDtTime = Mpa.TransDate;
                //Tp.HostDtTime = Rm.TransDate;
                Tp.SystemTarget = 9; // t24 and fs studio 

                Tp.CardNo = Mpa.CardNumber;
                Tp.AccNo = Mpa.AccNumber;

                Tp.TransType = 21; // MAKE Credit

                Tp.TransDesc = "Diff In Visa Settlement Id :" + Mpa.UniqueRecordId;
                Tp.GlEntry = false;

                Tp.AtmTraceNoH = Mpa.UniqueRecordId;

                // Second Entry 
                Tp.TransType2 = 11; // MAKE GL

                Tp.TransDesc2 = "Diff In Visa Settlement Id :" + Mpa.UniqueRecordId;

                Tp.AccNo2 = Rc.GlAccount;  // Category GL Account 
                Tp.GlEntry2 = true;

                Tp.CurrDesc = Mpa.TransCurr;

                Tp.TranAmount = InAmount;

                Tp.OpenDate = DateTime.Now;

                Tp.TransMsg = "";
                Tp.AtmMsg = "";

                Tp.DisputeNo = 0;
                Tp.DispTranNo = 0;
                Tp.ErrNo = 0;
                Tp.Operator = Mpa.Operator;
                //TEST
                Tp.MakerUser = InUserId;
                Tp.AuthUser = InAuthUser;

                Tp.OpenRecord = true;
                //NOSTRO
                Tp.NostroCcy = "";
                Tp.NostroAdjAmt = 0;

                int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

            }
            if (InTranType == 11)
            {
                // CREATE A TRANSACTION TO DR CUSTOMER 
                // CREATE A TRANSACTION TO CR RM Category 
                Tp.OriginId = "05"; // *
                Tp.OriginName = "Settlement";    // From Dispute 
                Tp.RMCateg = Mpa.RMCateg;
                Tp.RMCategCycle = Mpa.MatchingAtRMCycle;
                Tp.UniqueRecordId = Mpa.UniqueRecordId;

                Tp.AtmNo = "N/A";
                Tp.SesNo = 0;

                Tp.BankId = "Other";
                Tp.AtmTraceNo = 0;
                Tp.BranchId = "Other";
                Tp.AtmDtTime = Mpa.TransDate;
                //Tp.HostDtTime = Rm.TransDate;
                Tp.SystemTarget = 9; // t24 and fs studio 

                Tp.CardNo = Mpa.CardNumber;
                Tp.AccNo = Mpa.AccNumber;

                Tp.TransType = 11; // MAKE Debit

                Tp.TransDesc = "Diff In Visa Settlement Id :" + Mpa.UniqueRecordId;
                Tp.GlEntry = false;

                Tp.AtmTraceNoH = Mpa.UniqueRecordId;

                // Second Entry 
                Tp.TransType2 = 21; // MAKE GL

                Tp.TransDesc2 = "Diff In Visa Settlement Id :" + Mpa.UniqueRecordId;

                Tp.AccNo2 = Rc.GlAccount;  // Category GL Account 
                Tp.GlEntry2 = true;

                Tp.CurrDesc = Mpa.TransCurr;

                Tp.TranAmount = InAmount;

                Tp.OpenDate = DateTime.Now;

                Tp.TransMsg = "";
                Tp.AtmMsg = "";

                Tp.DisputeNo = 0;
                Tp.DispTranNo = 0;
                Tp.ErrNo = 0;
                Tp.Operator = Mpa.Operator;
                //TEST
                Tp.MakerUser = InUserId;
                Tp.AuthUser = InAuthUser;

                Tp.OpenRecord = true;
                //NOSTRO
                Tp.NostroCcy = "";
                Tp.NostroAdjAmt = 0;

                int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

            }

        }
        // 
        // 
        // This is called from Form51 - Replenishment  
        //
        public void CreateTransTobepostedfromReplenishment(string InOperator, string InAtmNo, int InSesNo, string InUserId, string InAuthUser,
                                                           decimal OutCassetteAmnt, decimal InCassetteAmnt,
                                                          string InRMCateg, int InRMCategCycle)
        {
            //
            // At the end of Replenishment we create two sets of transactions
            // One for the money taken out and other for the money in 
            //
      
            int WUniqueRecordId = 0;

            ErrorFound = false;
            ErrorOutput = "";
            string WUser = InUserId;

            try
            {
                Ac.ReadAtm(InAtmNo);

                // OVERRIDE USER
                if (Ac.CitId != "1000")
                {
                    WUser = Ac.CitId;
                }

                // Insert Transactions (TWO Trans) In TransTo BePosted 

                // Money Out of Cassettes
                // 
                if (OutCassetteAmnt > 0)
                {
                    WUniqueRecordId = Gu.GetNextValue();

                    Tp.OriginId = "04"; // *
                    Tp.OriginName = "OurATMs-Repl : " + InAtmNo;   // ORIGIN Replenishment
                    Tp.RMCateg = InRMCateg;
                    Tp.RMCategCycle = InRMCategCycle;
                    Tp.UniqueRecordId = WUniqueRecordId;

                    Tp.ErrNo = 0;
                    Tp.AtmNo = InAtmNo;
                    Tp.SesNo = InSesNo;
                    Tp.BankId = Ac.BankId;

                    Tp.AtmTraceNo = 0;

                    Tp.BranchId = Ac.Branch;
                    Tp.AtmDtTime = DateTime.Now;
                    //Tp.HostDtTime = DateTime.Now;
                    Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                    Tp.CardNo = "N/A";
                    Tp.CardOrigin = 5; // Find OUT

                    // First Entry 
                    Tp.TransType = 11;
                    Tp.TransDesc = "Unloaded Money.Return them to Till";

                    Acc.ReadAndFindAccount(WUser, "", "", InOperator, "", Ac.DepCurNm, "User or CIT Cash");
                    if (Acc.RecordFound == true)
                    {
                        Tp.AccNo = Acc.AccNo;  // USER Till Account 
                    }
                    else
                    {
                        Tp.AccNo = "Not Found Acc";
                        ErrorFound = false;
                        ErrorOutput = "Account not found for User : " + WUser;
                    }

                    Tp.GlEntry = true;

                    // Second Entry 
                    // CREATE A TRANSACTION TO CR ATM CASH 

                    Tp.TransType2 = 21; // MAKE Second Entry 
                    Tp.TransDesc2 = "ATM Cash credited. Unloaded Money(Notes).";
                    // When we put 1000 we want to get the accounts for ATM 

                    Acc.ReadAndFindAccount("1000", "", "", InOperator, InAtmNo, Ac.DepCurNm, "ATM Cash");
                    if (Acc.RecordFound == true)
                    {
                        Tp.AccNo2 = Acc.AccNo;  // ATM Cash Account 
                    }
                    else
                    {
                        Tp.AccNo2 = "Not Found Acc";
                        ErrorFound = false;
                        ErrorOutput = "Account not found for User : " + WUser;
                    }

                    Tp.GlEntry2 = true;

                    Tp.CurrDesc = Ac.DepCurNm;
                    Tp.TranAmount = OutCassetteAmnt;

                    Tp.AuthCode = 0;
                    Tp.RefNumb = 0;
                    Tp.RemNo = 0;
                    Tp.TransMsg = "N/A";
                    Tp.AtmMsg = "";
                    Tp.MakerUser = InUserId;
                    Tp.AuthUser = InAuthUser;
                    Tp.OpenDate = DateTime.Now;
                    Tp.OpenRecord = true;
                    Tp.Operator = InOperator;
                    //NOSTRO
                    Tp.NostroCcy = "";
                    Tp.NostroAdjAmt = 0;

                    int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);
                }

                // MAKE SECOND PAIR OF TRANSACTIONS

                // Money In Cassettes
                // 
                if (InCassetteAmnt > 0)
                {
                    WUniqueRecordId = Gu.GetNextValue();
                    Tp.OriginId = "04"; // *
                    Tp.OriginName = "OurATMs-Repl : " + InAtmNo;   // ORIGIN Replenishment
                    Tp.RMCateg = InRMCateg;
                    Tp.RMCategCycle = InRMCategCycle;
                    Tp.UniqueRecordId = WUniqueRecordId;

                    Tp.ErrNo = 0;
                    Tp.AtmNo = InAtmNo;
                    Tp.SesNo = InSesNo;
                    Tp.BankId = Ac.BankId;

                    Tp.AtmTraceNo = 0;

                    Tp.BranchId = Ac.Branch;
                    Tp.AtmDtTime = DateTime.Now;
                    //Tp.HostDtTime = DateTime.Now;
                    Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                    Tp.CardNo = "N/A";
                    Tp.CardOrigin = 5; // Find OUT

                    // First Entry 
                    Tp.TransType = 11;
                    Tp.TransDesc = "Loaded Money(Notes) - DR ATM cash";

                    Acc.ReadAndFindAccount("1000", "", "", InOperator, InAtmNo, Ac.DepCurNm, "ATM Cash");
                    if (Acc.RecordFound == true)
                    {
                        Tp.AccNo = Acc.AccNo;  // ATM CAsh   // ATM Cash Account 
                    }
                    else
                    {
                        Tp.AccNo = "Not Found Cash Acc";
                        ErrorFound = false;
                        ErrorOutput = "Account not found for User : " + WUser;
                    }

                    Tp.GlEntry = true;

                    // Second Entry 
                    // CREATE A TRANSACTION TO CR ATM CASH 

                    Tp.TransType2 = 21; // MAKE Second Entry 
                    Tp.TransDesc2 = "User Till Credited. Money Loaded To ATM.";

                    Acc.ReadAndFindAccount(WUser, "", "", InOperator, "", Ac.DepCurNm, "User or CIT Cash");
                    if (Acc.RecordFound == true)
                    {
                        Tp.AccNo2 = Acc.AccNo;  // USER Till Account 
                    }
                    else
                    {
                        Tp.AccNo2 = "Not Found Acc";
                        ErrorFound = false;
                        ErrorOutput = "Account not found for User : " + WUser;
                    }

                    Tp.GlEntry2 = true;

                    Tp.CurrDesc = Ac.DepCurNm;
                    Tp.TranAmount = InCassetteAmnt;
                    Tp.AuthCode = 0;
                    Tp.RefNumb = 0;
                    Tp.RemNo = 0;
                    Tp.TransMsg = "N/A";
                    Tp.AtmMsg = "";
                    Tp.MakerUser = InUserId;
                    Tp.AuthUser = InAuthUser;
                    Tp.OpenDate = DateTime.Now;
                    Tp.OpenRecord = true;
                    Tp.Operator = InOperator;
                    //NOSTRO
                    Tp.NostroCcy = "";
                    Tp.NostroAdjAmt = 0;

                    int PostedNo2 = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

                }

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

        }

        // 
        // 
        // This is called from Form51 - Replenishment  
        //
        public void CreateTransTobepostedfromReplenishment_CIT(string InOperator, string InAtmNo, int InSesNo, string InUserId, string InAuthUser,
                                                           decimal OutCassetteAmnt, decimal InCassetteAmnt)
        {
            //
            // At the end of Loading G4S Excel we create two sets of transactions
            // One for undloaded money and the other of loaded money  
            //

            int WUniqueRecordId = Gu.GetNextValue();

            ErrorFound = false;
            ErrorOutput = "";
            string WUser = InUserId;

            try
            {
                Ac.ReadAtm(InAtmNo);

                // OVERRIDE USER
                if (Ac.CitId != "1000")
                {
                    WUser = Ac.CitId;
                }

                // Insert Transactions (TWO Trans) In TransTo BePosted 

                // Money Out of Cassettes
                // 
                Tp.OriginId = "04"; // *
                Tp.OriginName = "OurATMs-Repl : " + InAtmNo;   // ORIGIN Replenishment
                Tp.RMCateg = "N/A";
                Tp.RMCategCycle = 0;
                Tp.UniqueRecordId = WUniqueRecordId;

                Tp.ErrNo = 0;
                Tp.AtmNo = InAtmNo;
                Tp.SesNo = InSesNo;
                Tp.BankId = Ac.BankId;

                Tp.AtmTraceNo = 0;

                Tp.BranchId = Ac.Branch;
                Tp.AtmDtTime = DateTime.Now;
                //Tp.HostDtTime = DateTime.Now;
                Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                Tp.CardNo = "N/A";
                Tp.CardOrigin = 5; // Find OUT

                // First Entry 
                Tp.TransType = 11;
                Tp.TransDesc = "Unloaded Money.Return them to Till";

                Acc.ReadAndFindAccount(WUser, "", "", InOperator, "", Ac.DepCurNm, "User or CIT Cash");
                if (Acc.RecordFound == true)
                {
                    Tp.AccNo = Acc.AccNo;  // USER Till Account 
                }
                else
                {
                    Tp.AccNo = "Not Found Acc";
                    ErrorFound = false;
                    ErrorOutput = "Account not found for User : " + WUser;
                }

                Tp.GlEntry = true;

                // Second Entry 
                // CREATE A TRANSACTION TO CR ATM CASH 

                Tp.TransType2 = 21; // MAKE Second Entry 
                Tp.TransDesc2 = "ATM Cash credited. Unloaded Money(Notes).";
                // When we put 1000 we want to get the accounts for ATM 

                Acc.ReadAndFindAccount("1000", "", "", InOperator, InAtmNo, Ac.DepCurNm, "ATM Cash");
                if (Acc.RecordFound == true)
                {
                    Tp.AccNo2 = Acc.AccNo;  // ATM Cash Account 
                }
                else
                {
                    Tp.AccNo2 = "Not Found Acc";
                    ErrorFound = false;
                    ErrorOutput = "Account not found for User : " + WUser;
                }

                Tp.GlEntry2 = true;

                Tp.CurrDesc = Ac.DepCurNm;
                Tp.TranAmount = OutCassetteAmnt;

                Tp.AuthCode = 0;
                Tp.RefNumb = 0;
                Tp.RemNo = 0;
                Tp.TransMsg = "N/A";
                Tp.AtmMsg = "";
                Tp.MakerUser = InUserId;
                Tp.AuthUser = InAuthUser;
                Tp.OpenDate = DateTime.Now;
                Tp.OpenRecord = true;
                Tp.Operator = InOperator;
                //NOSTRO
                Tp.NostroCcy = "";
                Tp.NostroAdjAmt = 0;

                int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

                // MAKE SECOND PAIR OF TRANSACTIONS

                // LOADED Money 
                // 

                Tp.OriginId = "04"; // *
                Tp.OriginName = "OurATMs-Repl : " + InAtmNo;   // ORIGIN Replenishment
                Tp.RMCateg = "N/A";
                Tp.RMCategCycle = 0;
                Tp.UniqueRecordId = WUniqueRecordId;

                Tp.ErrNo = 0;
                Tp.AtmNo = InAtmNo;
                Tp.SesNo = InSesNo;
                Tp.BankId = Ac.BankId;

                Tp.AtmTraceNo = 0;

                Tp.BranchId = Ac.Branch;
                Tp.AtmDtTime = DateTime.Now;
                //Tp.HostDtTime = DateTime.Now;
                Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                Tp.CardNo = "N/A";
                Tp.CardOrigin = 5; // Find OUT

                // First Entry 
                Tp.TransType = 11;
                Tp.TransDesc = "Loaded Money(Notes) - DR ATM cash";

                Tp.AccNo = Acc.AccNo;  // ATM CAsh 
                Tp.GlEntry = true;

                // Second Entry 
                // CREATE A TRANSACTION TO CR ATM CASH 

                Tp.TransType2 = 21; // MAKE Second Entry 
                Tp.TransDesc2 = "User or CIT Credited. Money Loaded To ATM.";

                Acc.ReadAndFindAccount(WUser, "", "", InOperator, "", Ac.DepCurNm, "User or CIT Cash");
                if (Acc.RecordFound == true)
                {
                    Tp.AccNo2 = Acc.AccNo;  // ATM Cash Account 
                }
                else
                {
                    Tp.AccNo2 = "Not Found Acc";
                    ErrorFound = false;
                    ErrorOutput = "Account not found for User : " + WUser;
                }

                Tp.AccNo2 = Acc.AccNo;  // // USER Till Account 
                Tp.GlEntry2 = true;

                Tp.CurrDesc = Ac.DepCurNm;
                Tp.TranAmount = InCassetteAmnt;
                Tp.AuthCode = 0;
                Tp.RefNumb = 0;
                Tp.RemNo = 0;
                Tp.TransMsg = "N/A";
                Tp.AtmMsg = "";
                Tp.MakerUser = InUserId;
                Tp.AuthUser = InAuthUser;
                Tp.OpenDate = DateTime.Now;
                Tp.OpenRecord = true;
                Tp.Operator = InOperator;
                //NOSTRO
                Tp.NostroCcy = "";
                Tp.NostroAdjAmt = 0;

                int PostedNo2 = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

        }

        // 
        // 
        // This is called from Form50b - Cash Management during replenishment 
        //
        public void CreateTransTobepostedfromCashManagement(string InOperator, string InAtmNo, int InSesNo, string InUserId,
                                                           decimal MoneyFromCashInVault)
        {
            //
            // At the end of Replenishment we create two sets of transactions
            // One for the money taken out and other for the money in 
            //

            int WUniqueRecordId = Gu.GetNextValue();

            ErrorFound = false;
            ErrorOutput = "";
            string WUser = InUserId;

            try
            {
                Ac.ReadAtm(InAtmNo);

                // OVERRIDE USER
                if (Ac.CitId != "1000")
                {
                    WUser = Ac.CitId;
                }

                // Insert Transactions (TWO Trans) In TransTo BePosted 
                // CR CASH IN VAULT  
                // DR ATM SUPERVISOR TILL

                //
                // 

                Tp.OriginId = "04"; // *
                Tp.OriginName = "OurATMs-Repl : " + InAtmNo;   // ORIGIN Replenishment
                Tp.RMCateg = "N/A";
                Tp.RMCategCycle = 0;
                Tp.UniqueRecordId = WUniqueRecordId;

                Tp.ErrNo = 0;
                Tp.AtmNo = InAtmNo;
                Tp.SesNo = InSesNo;
                Tp.BankId = Ac.BankId;

                Tp.AtmTraceNo = 0;

                Tp.BranchId = Ac.Branch;
                Tp.AtmDtTime = DateTime.Now;
                //Tp.HostDtTime = DateTime.Now;
                Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                Tp.CardNo = "N/A";
                Tp.CardOrigin = 5; // Find OUT

                // First Entry 
                Tp.TransType = 11;
                Tp.TransDesc = "Cash moved to Atm Supervisor from Vaults ";

                Acc.ReadAndFindAccount(WUser, "", "", InOperator, "", Ac.DepCurNm, "User or CIT Cash");
                if (Acc.RecordFound == true)
                {
                    Tp.AccNo = Acc.AccNo;  // USER Till Account 
                }
                else
                {
                    Tp.AccNo = "Not Found Acc";
                    ErrorFound = false;
                    ErrorOutput = "Account not found for User : " + WUser;
                }

                Tp.GlEntry = true;

                // Second Entry 
                // CREATE A TRANSACTION TO CR Cash in vaults 

                Tp.TransType2 = 21; // MAKE Second Entry 
                Tp.TransDesc2 = "Cash in Vault is credited.";
                // When we put 1000 we want to get the accounts for ATM 

                Acc.ReadAndFindAccount("1000", "", "", InOperator, InAtmNo, Ac.DepCurNm, "Cash in Vault");
                if (Acc.RecordFound == true)
                {
                    Tp.AccNo2 = Acc.AccNo;  // ATM Cash Account 
                }
                else
                {
                    Tp.AccNo2 = "Not Found Acc";
                    ErrorFound = false;
                    ErrorOutput = "Account not found for User : " + WUser;
                }

                Tp.GlEntry2 = true;

                Tp.CurrDesc = Ac.DepCurNm;
                Tp.TranAmount = MoneyFromCashInVault;

                Tp.AuthCode = 0;
                Tp.RefNumb = 0;
                Tp.RemNo = 0;
                Tp.TransMsg = "N/A";
                Tp.AtmMsg = "";
                Tp.MakerUser = InUserId;
                Tp.AuthUser = "";
                Tp.OpenDate = DateTime.Now;
                Tp.OpenRecord = true;
                Tp.Operator = InOperator;
                //NOSTRO
                Tp.NostroCcy = "";
                Tp.NostroAdjAmt = 0;

                int PostedNo = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);

                //// MAKE SECOND PAIR OF TRANSACTIONS

                //// Money In Cassettes
                //// 

                //Tp.OriginId = "04"; // *
                //Tp.OriginName = "OurATMs-Repl : " + InAtmNo;   // ORIGIN Replenishment
                //Tp.RMCateg = "N/A";
                //Tp.RMCategCycle = 0;
                //Tp.UniqueRecordId = WUniqueRecordId;

                //Tp.ErrNo = 0;
                //Tp.AtmNo = InAtmNo;
                //Tp.SesNo = InSesNo;
                //Tp.BankId = Ac.BankId;

                //Tp.AtmTraceNo = 0;

                //Tp.BranchId = Ac.Branch;
                //Tp.AtmDtTime = DateTime.Now;
                ////Tp.HostDtTime = DateTime.Now;
                //Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                //Tp.CardNo = "N/A";
                //Tp.CardOrigin = 5; // Find OUT

                //// First Entry 
                //Tp.TransType = 11;
                //Tp.TransDesc = "Loaded Money(Notes) - DR ATM cash";

                //Tp.AccNo = Acc.AccNo;  // ATM CAsh 
                //Tp.GlEntry = true;

                //// Second Entry 
                //// CREATE A TRANSACTION TO CR ATM CASH 

                //Tp.TransType2 = 21; // MAKE Second Entry 
                //Tp.TransDesc2 = "User Till Credited. Money Loaded To ATM.";

                //Acc.ReadAndFindAccount(WUser, InOperator, "", Ac.DepCurNm, "User or CIT Cash");
                //if (Acc.RecordFound == true)
                //{
                //    Tp.AccNo2 = Acc.AccNo;  // ATM Cash Account 
                //}
                //else
                //{
                //    Tp.AccNo2 = "Not Found Acc";
                //    ErrorFound = false;
                //    ErrorOutput = "Account not found for User : " + WUser;
                //}

                //Tp.AccNo2 = Acc.AccNo;  // // USER Till Account 
                //Tp.GlEntry2 = true;

                //Tp.CurrDesc = Ac.DepCurNm;
                //Tp.TranAmount = InCassetteAmnt;
                //Tp.AuthCode = 0;
                //Tp.RefNumb = 0;
                //Tp.RemNo = 0;
                //Tp.TransMsg = "N/A";
                //Tp.AtmMsg = "";
                //Tp.MakerUser = InUserId;
                //Tp.AuthUser = InAuthUser;
                //Tp.OpenDate = DateTime.Now;
                //Tp.OpenRecord = true;
                //Tp.Operator = InOperator;
                ////NOSTRO
                //Tp.NostroCcy = "";
                //Tp.NostroAdjAmt = 0;

                //int PostedNo2 = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);


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
                    CatchDetails(ex);
                }
        }
        // ===============================REad and copy errors Ids===================
        // ===============================From ModelBak to new Bank==================
        // For each read record create ==============================================
        // ==========================================================================
        public void CopyErrorIds(string InBankA, string InBankB, string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsIdCharacteristics] "
          + " WHERE Operator=@Operator";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InBankA);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadErrorCharacteristicsFields(rdr);

                            // =========== Insert Record Id for new bank =======
                            // =================================================
                            // INSERT GAS PARAMETER WITH DIFFERENT BANK 
                            BankId = InBankB;
                            Operator = InOperator;

                            InsertErrorIdRecord();
                            // =================================================      
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

        // ==============================================================================
        // ===============================INSERT ERROR ID==================================
        // ================================================================================
        public void InsertErrorIdRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[ErrorsIdCharacteristics] ([ErrId],[ErrDesc],[ErrType],[AtmNo],[SesNo],[DateInserted],"
                   + "[BankId],[BranchId],[TurboReconc],[TraceNo],[CardNo],[TransType],[TransDescr],"
                   + "[DateTime],[NeedAction],[OpenErr],[FullCard],[UnderAction],[ManualAct],"
                   + "[ByWhom],[ActionDtTm],[ActionRMCycle],[CurDes],[ErrAmount],[ActionId],"
                   + "[DrCust],[CrCust],[CustAccNo],[DrAtmCash],[CrAtmCash],[AccountNo1],[DrAtmSusp],[CrAtmSusp],[AccountNo2],"
                   + "[DrAccount3],[CrAccount3],[AccountNo3],[ForeignCard],[MainOnly],[UserComment],"
                   + "[Printed],[DatePrinted],[CircularDesc],[Operator] )"
                + " VALUES (@ErrId,@ErrDesc,@ErrType,@AtmNo,@SesNo,@DateInserted,"
                    + "@BankId,@BranchId,@TurboReconc,@TraceNo,@CardNo,@TransType,@TransDescr,"
                   + "@DateTime,@NeedAction,@OpenErr,@FullCard,@UnderAction,@ManualAct,"
                   + "@ByWhom,@ActionDtTm,@ActionRMCycle,@CurDes,@ErrAmount,@ActionId,"
                   + "@DrCust,@CrCust,@CustAccNo,@DrAtmCash,@CrAtmCash,@AccountNo1,@DrAtmSusp,@CrAtmSusp,@AccountNo2,"
                   + "@DrAccount3,@CrAccount3,@AccountNo3,@ForeignCard, @MainOnly,@UserComment,"
                   + "@Printed,@DatePrinted,@CircularDesc,@Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@ErrId", ErrId);
                        cmd.Parameters.AddWithValue("@ErrDesc", ErrDesc);
                        cmd.Parameters.AddWithValue("@ErrType", ErrType);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);

                        cmd.Parameters.AddWithValue("@DateInserted", DateInserted);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@TurboReconc", TurboReconc);

                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);

                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDescr", TransDescr);

                        cmd.Parameters.AddWithValue("@DateTime", DateTime);
                        cmd.Parameters.AddWithValue("@NeedAction", NeedAction);
                        cmd.Parameters.AddWithValue("@OpenErr", OpenErr);
                        cmd.Parameters.AddWithValue("@FullCard", FullCard);

                        cmd.Parameters.AddWithValue("@UnderAction", UnderAction); // 
                        cmd.Parameters.AddWithValue("@ManualAct", ManualAct);  //

                        cmd.Parameters.AddWithValue("@ByWhom", ByWhom);  //
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);
                        cmd.Parameters.AddWithValue("@ActionRMCycle", ActionRMCycle);

                        cmd.Parameters.AddWithValue("@CurDes", CurDes);
                        cmd.Parameters.AddWithValue("@ErrAmount", ErrAmount);
                        cmd.Parameters.AddWithValue("@ActionId", ActionId);
                        cmd.Parameters.AddWithValue("@DrCust", DrCust);
                        cmd.Parameters.AddWithValue("@CrCust", CrCust);
                        cmd.Parameters.AddWithValue("@CustAccNo", CustAccNo);

                        cmd.Parameters.AddWithValue("@DrAtmCash", DrAtmCash);
                        cmd.Parameters.AddWithValue("@CrAtmCash", CrAtmCash);
                        cmd.Parameters.AddWithValue("@AccountNo1", AccountNo1);

                        cmd.Parameters.AddWithValue("@DrAtmSusp", DrAtmSusp);
                        cmd.Parameters.AddWithValue("@CrAtmSusp", CrAtmSusp);
                        cmd.Parameters.AddWithValue("@AccountNo2", AccountNo2);

                        cmd.Parameters.AddWithValue("@DrAccount3", DrAccount3);
                        cmd.Parameters.AddWithValue("@CrAccount3", CrAccount3);
                        cmd.Parameters.AddWithValue("@AccountNo3", AccountNo3);


                        cmd.Parameters.AddWithValue("@ForeignCard", ForeignCard);
                        cmd.Parameters.AddWithValue("@MainOnly", MainOnly);

                        cmd.Parameters.AddWithValue("@UserComment", UserComment);
                        cmd.Parameters.AddWithValue("@Printed", Printed);
                        cmd.Parameters.AddWithValue("@DatePrinted", DatePrinted);

                        cmd.Parameters.AddWithValue("@CircularDesc", CircularDesc);

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
        // DELETE Error bY SeqNo
        //
        public void DeleteErrorRecordByErrNo(int InErrNo)
        {

            ErrorFound = false;
            ErrorOutput = "";


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[ErrorsTable] "
                            + " WHERE ErrNo =  @ErrNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);

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

using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMCaptureCardsClass : Logger
    {
        public RRDMCaptureCardsClass() : base() { }
        // Declare fields 
        //
        public int CaptNo; 
        public string AtmNo; 
        public string BankId; 
   
        public string BranchId; 
        public int SesNo;
        public int TraceNo;
        public int MasterTraceNo; 
        public string CardNo;
        public DateTime CaptDtTm;
        public int CaptureCd; 
        public string ReasonDesc;
        public DateTime ActionDtTm; 
        public string CustomerNm;
        public string ActionComments;
        public int ActionCode;
        public int LoadedAtRMCycle;
        public bool OpenRec;

        public bool Received;
        public DateTime DateReceived;
        public string Origin; 

        public string Operator; 
        
    //    public bool Inactive;

        public int CaptureCardsNo; 

        public bool RecordFound;

        public bool ErrorFound;
        public string ErrorOutput;
        readonly DateTime LongFutureDate = new DateTime(2050, 11, 21);
        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        // Define the data table 
        public DataTable CapturedCardsDataTable = new DataTable();

        public int TotalSelected;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Capture Reader Fields
        private void CaptureReaderFields(SqlDataReader rdr)
        {
            CaptNo = (int)rdr["CaptNo"];
            AtmNo = (string)rdr["AtmNo"];
            BankId = (string)rdr["BankId"];

            BranchId = (string)rdr["BranchId"];
            SesNo = (int)rdr["SesNo"];
            TraceNo = (int)rdr["TraceNo"];
            MasterTraceNo = (int)rdr["MasterTraceNo"];

            CardNo = (string)rdr["CardNo"];
            CaptDtTm = (DateTime)rdr["CaptDtTm"];
            CaptureCd = (int)rdr["CaptureCd"];
            ReasonDesc = (string)rdr["ReasonDesc"];
            ActionDtTm = (DateTime)rdr["ActionDtTm"];

            CustomerNm = (string)rdr["CustomerNm"];
            ActionComments = (string)rdr["ActionComments"];
            ActionCode = (int)rdr["ActionCode"];
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
            OpenRec = (bool)rdr["OpenRec"];

            Received = (bool)rdr["Received"];
            DateReceived = (DateTime)rdr["DateReceived"];
            Origin = (string)rdr["Origin"];
            
            Operator = (string)rdr["Operator"];
        }

        // READ Captured Cards and Fill Table 
        string SqlString;
        public void ReadCaptureCardsTable(string InOperator, string InAtmNo, 
                                  string InCardNo, int InSesNo ,DateTime InActionDtTm, bool InWithDate, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //Gridfilter = " AtmNo = @AtmNo AND CardNo = @CardNo ";

            //Gridfilter = "AtmNo = '" + WAtmNo + "'";

            CapturedCardsDataTable = new DataTable();
            CapturedCardsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            CapturedCardsDataTable.Columns.Add("CapturedNo", typeof(int));
            CapturedCardsDataTable.Columns.Add("AtmNo", typeof(string));
            CapturedCardsDataTable.Columns.Add("ReplCycle", typeof(int));
            CapturedCardsDataTable.Columns.Add("CardNo", typeof(string));

            CapturedCardsDataTable.Columns.Add("CaptDtTm", typeof(DateTime));
            CapturedCardsDataTable.Columns.Add("ReasonDesc", typeof(string));
            CapturedCardsDataTable.Columns.Add("ActionDtTm", typeof(string));
            CapturedCardsDataTable.Columns.Add("CustomerNm", typeof(string));

            CapturedCardsDataTable.Columns.Add("TraceNo", typeof(int));
            if (InMode == 11)
            {
                SqlString = "SELECT *"
                          + " FROM [dbo].[CapturedCards] "
                            + " WHERE Operator = @Operator AND AtmNo = @AtmNo ";
            }
            if (InMode == 12)
            {
                SqlString = "SELECT *"
                          + " FROM [dbo].[CapturedCards] "
                            + " WHERE AtmNo = @AtmNo AND CardNo = @CardNo ";
            }
            if (InMode == 13)
            {
                SqlString = "SELECT *"
                          + " FROM [dbo].[CapturedCards] "
                            + " AtmNo = @AtmNo AND ActionDtTm = @ActionDtTm ";
            }
            if (InMode == 14)
            {
                SqlString = "SELECT *"
                          + " FROM [dbo].[CapturedCards] "
                            + " AtmNo = @AtmNo ";
            }
            if (InMode == 15)
            {
                SqlString = "SELECT *"
                          + " FROM [dbo].[CapturedCards] "
                            + " AtmNo = @AtmNo AND SesNo = @SesNo ";
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
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        }
                        if (InMode == 12)
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                            cmd.Parameters.AddWithValue("@CardNo", InCardNo);
                        }

                        if (InMode == 13)
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                       
                            cmd.Parameters.AddWithValue("@ActionDtTm", InActionDtTm);
                        }

                        if (InMode == 14)
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        }
                        if (InMode == 15)
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                            cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Captured Details 
                            CaptureReaderFields(rdr);

                            DataRow RowSelected = CapturedCardsDataTable.NewRow();

                            RowSelected["CapturedNo"] = CaptNo;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["ReplCycle"] = SesNo;
                            RowSelected["CardNo"] = CardNo;

                            RowSelected["CaptDtTm"] = CaptDtTm;
                            RowSelected["ReasonDesc"] = ReasonDesc;

                            if (ActionDtTm == LongFutureDate)
                            {
                                RowSelected["ActionDtTm"] = "No Action";
                            }
                            else
                            {
                                RowSelected["ActionDtTm"] = ActionDtTm.ToString();
                            }

                            RowSelected["CustomerNm"] = CustomerNm;

                            RowSelected["TraceNo"] = TraceNo;

                            CapturedCardsDataTable.Rows.Add(RowSelected);

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
        // READ BY DATE RANGE - eg Repl Cycle 
        public int CapturedNumber; 
        public void ReadCaptureCardsTableByDatesTm_Range(string InOperator, string InInput,
                                  DateTime InFromCaptDtTm, DateTime InToCaptDtTm, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            CapturedNumber = 0; 

            CapturedCardsDataTable = new DataTable();
            CapturedCardsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            CapturedCardsDataTable.Columns.Add("CapturedNo", typeof(int));
            CapturedCardsDataTable.Columns.Add("AtmNo", typeof(string));
            CapturedCardsDataTable.Columns.Add("ReplCycle", typeof(int));
            CapturedCardsDataTable.Columns.Add("CardNo", typeof(string));

            CapturedCardsDataTable.Columns.Add("CaptDtTm", typeof(DateTime));
            CapturedCardsDataTable.Columns.Add("ReasonDesc", typeof(string));
            CapturedCardsDataTable.Columns.Add("ActionDtTm", typeof(string));
            CapturedCardsDataTable.Columns.Add("CustomerNm", typeof(string));

            CapturedCardsDataTable.Columns.Add("TraceNo", typeof(int));

            if (InMode == 20)
            {

                // ATM
                SqlString = "SELECT *"
                              + " FROM [dbo].[CapturedCards] "
                                + " WHERE AtmNo = @AtmNo AND (CaptDtTm BETWEEN @FromCaptDtTm AND @ToCaptDtTm)  ";


            }

            if (InMode == 21)
            {

                // ATM
                SqlString = "SELECT *"
                              + " FROM [dbo].[CapturedCards] "
                                + " WHERE BranchId = @BranchId AND (CaptDtTm BETWEEN @FromCaptDtTm AND @ToCaptDtTm)  ";


            }

            if (InMode == 22)
            {

                // BANK
                SqlString = "SELECT *"
                              + " FROM [dbo].[CapturedCards] "
                                + " WHERE Operator = @Operator AND (CaptDtTm >= @FromCaptDtTm AND CaptDtTm <= @ToCaptDtTm)  ";


            }

            if (InMode == 23)
            {

                // BANK
                SqlString = "SELECT *"
                              + " FROM [dbo].[CapturedCards] "
                                + " WHERE Operator = @Operator AND AtmNo = @AtmNo "
                                + "AND (CaptDtTm >= @FromCaptDtTm AND CaptDtTm <= @ToCaptDtTm)  ";


            }

            if (InMode == 24)
            {

                // Full card

                SqlString = "SELECT *"
                            + " FROM [dbo].[CapturedCards] "
                              + " WHERE CardNo = @CardNo AND (CaptDtTm BETWEEN @FromCaptDtTm AND @ToCaptDtTm)  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InMode == 20 || InMode == 23)
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InInput);
                        }
                        if (InMode == 22 || InMode == 23)
                        {
                            cmd.Parameters.AddWithValue("@Operator", InOperator);
                        }

                        if (InMode == 21 )
                        {
                            cmd.Parameters.AddWithValue("@BranchId", InInput);
                        }

                        if (InMode == 24)
                        {
                            cmd.Parameters.AddWithValue("@CardNo", InInput);
                        }

                        cmd.Parameters.AddWithValue("@FromCaptDtTm", InFromCaptDtTm);
                        cmd.Parameters.AddWithValue("@ToCaptDtTm", InToCaptDtTm);
                        
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            CaptureCardsNo = CaptureCardsNo + 1 ; 

                            // Read Captured Details 
                            CaptureReaderFields(rdr);

                            DataRow RowSelected = CapturedCardsDataTable.NewRow();

                            RowSelected["CapturedNo"] = CaptNo;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["ReplCycle"] = SesNo;
                            RowSelected["CardNo"] = CardNo;

                            RowSelected["CaptDtTm"] = CaptDtTm;
                            RowSelected["ReasonDesc"] = ReasonDesc;

                            if (ActionDtTm == LongFutureDate || ActionDtTm == NullPastDate)
                            {
                                RowSelected["ActionDtTm"] = "No Action";
                            }
                            else
                            {
                                RowSelected["ActionDtTm"] = ActionDtTm.ToString();
                            }

                            RowSelected["CustomerNm"] = CustomerNm;

                            RowSelected["TraceNo"] = TraceNo;

                            CapturedCardsDataTable.Rows.Add(RowSelected);

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


        // READ Captured Cards and Fill Table by Card No

        public void ReadCaptureCardsTableByCardNo(string InCardNo, DateTime InActionDtTm, bool InWithDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            CapturedCardsDataTable = new DataTable();
            CapturedCardsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            CapturedCardsDataTable.Columns.Add("CapturedNo", typeof(int));
            CapturedCardsDataTable.Columns.Add("AtmNo", typeof(string));
            CapturedCardsDataTable.Columns.Add("ReplCycle", typeof(int));
            CapturedCardsDataTable.Columns.Add("CardNo", typeof(string));

            CapturedCardsDataTable.Columns.Add("CaptDtTm", typeof(DateTime));
            CapturedCardsDataTable.Columns.Add("ReasonDesc", typeof(string));
            CapturedCardsDataTable.Columns.Add("ActionDtTm", typeof(string));
            CapturedCardsDataTable.Columns.Add("CustomerNm", typeof(string));

            CapturedCardsDataTable.Columns.Add("TraceNo", typeof(int));

            string SqlString = "SELECT *"
            + " FROM [dbo].[CapturedCards] "
              + " WHERE CardNo = @CardNo " ;
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

                            // Read Captured Details 
                            CaptureReaderFields(rdr);

                            DataRow RowSelected = CapturedCardsDataTable.NewRow();

                            RowSelected["CapturedNo"] = CaptNo;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["ReplCycle"] = SesNo;
                            RowSelected["CardNo"] = CardNo;

                            RowSelected["CaptDtTm"] = CaptDtTm;
                            RowSelected["ReasonDesc"] = ReasonDesc;

                            if (ActionDtTm == LongFutureDate)
                            {
                                RowSelected["ActionDtTm"] = "No Action";
                            }
                            else
                            {
                                RowSelected["ActionDtTm"] = ActionDtTm.ToString();
                            }

                            RowSelected["CustomerNm"] = CustomerNm;

                            RowSelected["TraceNo"] = TraceNo;

                            CapturedCardsDataTable.Rows.Add(RowSelected);

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

        // READ Captured Card by SeqNo

        public void ReadCaptureCardBySeqNo(int InCaptNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[CapturedCards] "
          + " WHERE CaptNo = @CaptNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CaptNo", InCaptNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Captured Details 
                            CaptureReaderFields(rdr);

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

        // READ Captured Card by SeqNo

        public void ReadCaptureCardByCardNo(string InCardNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = " SELECT * "
          + " FROM [dbo].[CapturedCards] "
          + " WHERE CardNo = @CardNo ";
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

                            // Read Captured Details 
                            CaptureReaderFields(rdr);

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

        // NUMBER OF CAPTURED CARDS WITHIN GIVEN DATES RANGE
        //
        public void ReadCapturedCardsNoWithinDatesRange(string InAtmNo, DateTime InDateTmFrom, DateTime InDateTmTo)
        {
            // initialise variables

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            CaptureCardsNo = 0; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[CapturedCards] "
          + " WHERE AtmNo=@AtmNo AND ( CaptDtTm > @DateTmFrom  AND CaptDtTm < @DateTmTo ) ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@DateTmFrom", InDateTmFrom);
                        cmd.Parameters.AddWithValue("@DateTmTo", InDateTmTo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            CaptureCardsNo = CaptureCardsNo + 1;
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


        // NUMBER OF CAPTURED CARDS WITHIN GIVEN SESSION
        //
        public void ReadCapturedCardsNoWithinSession(string InAtmNo, int InSesNo)
        {
            // initialise variables

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            CaptureCardsNo = 0;

            string SqlString = "SELECT *"
          + " FROM [dbo].[CapturedCards] "
          + " WHERE SesNo = @SesNo AND AtmNo=@AtmNo AND OpenRec =1";
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
                            CaptureCardsNo = CaptureCardsNo + 1;
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

        // Insert NEW Captured Card 
        //
        public void InsertCapturedCard(string InAtmNo)
        {
            
            ErrorFound = false;
            ErrorOutput = "";
           
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[CapturedCards]"
                + " ([AtmNo],[BankId],[BranchId],[SesNo],"
                + " [TraceNo],[MasterTraceNo],[CardNo],[CaptDtTm],[CaptureCd],[ReasonDesc],[ActionDtTm],"
                + " [CustomerNm],[ActionComments],[ActionCode],"
                + " [LoadedAtRMCycle],"
                 + "[OpenRec], [Operator])"
                + " VALUES("
                + " @AtmNo,@BankId,@BranchId,@SesNo,"
                + " @TraceNo,@MasterTraceNo,@CardNo,@CaptDtTm,@CaptureCd,@ReasonDesc,@ActionDtTm,"
                + " @CustomerNm,@ActionComments,@ActionCode,"
                + " @LoadedAtRMCycle, "
                 + "@OpenRec, @Operator"
                +  ")";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                    
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);

                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
                        cmd.Parameters.AddWithValue("@MasterTraceNo", MasterTraceNo);
                       
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CaptDtTm", CaptDtTm);
                        cmd.Parameters.AddWithValue("@CaptureCd", CaptureCd);
                        cmd.Parameters.AddWithValue("@ReasonDesc", ReasonDesc);

                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);

                        cmd.Parameters.AddWithValue("@CustomerNm", CustomerNm);
                        cmd.Parameters.AddWithValue("@ActionComments", ActionComments);
                        cmd.Parameters.AddWithValue("@ActionCode", ActionCode);

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
                        //     cmd.Parameters.AddWithValue("@ScannedSigned", ); // image 

                        cmd.Parameters.AddWithValue("@OpenRec", 1);

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

        public void InsertCapturedCard_From_Form26(string InAtmNo)
        {
        //    public bool Received;
        //public DateTime DateReceived;
        //public string Origin;
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[CapturedCards]"
                + " ([AtmNo],[BankId],[BranchId],[SesNo],"
                + " [TraceNo],[MasterTraceNo],[CardNo],[CaptDtTm],[CaptureCd],[ReasonDesc],[ActionDtTm],"
                + " [CustomerNm],[ActionComments],[ActionCode],"
                + " [Received],[DateReceived],[Origin],"
                + " [LoadedAtRMCycle],"
                 + "[OpenRec], [Operator])"
                + " VALUES("
                + " @AtmNo,@BankId,@BranchId,@SesNo,"
                + " @TraceNo,@MasterTraceNo,@CardNo,@CaptDtTm,@CaptureCd,@ReasonDesc,@ActionDtTm,"
                + " @CustomerNm,@ActionComments,@ActionCode,"
                 + " @Received,@DateReceived,@Origin,"
                + " @LoadedAtRMCycle, "
                 + "@OpenRec, @Operator"
                + ")";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);

                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
                        cmd.Parameters.AddWithValue("@MasterTraceNo", MasterTraceNo);

                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CaptDtTm", CaptDtTm);
                        cmd.Parameters.AddWithValue("@CaptureCd", CaptureCd);
                        cmd.Parameters.AddWithValue("@ReasonDesc", ReasonDesc);

                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);

                        cmd.Parameters.AddWithValue("@CustomerNm", CustomerNm);
                        cmd.Parameters.AddWithValue("@ActionComments", ActionComments);
                        cmd.Parameters.AddWithValue("@ActionCode", ActionCode);

                        cmd.Parameters.AddWithValue("@Received", Received);
                        cmd.Parameters.AddWithValue("@DateReceived", DateReceived);
                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
                        //     cmd.Parameters.AddWithValue("@ScannedSigned", ); // image 

                        cmd.Parameters.AddWithValue("@OpenRec", 1);

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


        // UPDATE Captured Cards
        // 
        public void UpdateCapturedCardSpecific(int InCaptNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE CapturedCards SET "
                            + " CardNo = @CardNo,ActionDtTm = @ActionDtTm,CustomerNm = @CustomerNm,"
                             + "ActionComments = @ActionComments,"
                             + "ActionCode = @ActionCode,"
                               + "Received = @Received,"
                                 + "DateReceived = @DateReceived,"
                                   + "Origin = @Origin,"
                             + "OpenRec = @OpenRec "
                            + " WHERE CaptNo = @CaptNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@CaptNo", InCaptNo);

                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);

                        cmd.Parameters.AddWithValue("@CustomerNm", CustomerNm);
                        cmd.Parameters.AddWithValue("@ActionComments", ActionComments);
                        cmd.Parameters.AddWithValue("@ActionCode", ActionCode);

                        cmd.Parameters.AddWithValue("@Received", Received);
                        cmd.Parameters.AddWithValue("@DateReceived", DateReceived);
                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        cmd.Parameters.AddWithValue("@OpenRec", OpenRec);


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

        // UPDATE Captured Cards that had Zero SesNo
        // 
        public void UpdateCapturedCardsWhichHadZeroSesNo(string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE CapturedCards SET "
                             + " SesNo = @SesNo "
                            + " WHERE AtmNo = @AtmNo AND SesNo = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                     
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



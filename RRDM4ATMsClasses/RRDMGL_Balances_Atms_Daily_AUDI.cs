using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMGL_Balances_Atms_Daily_AUDI : Logger
    {
        public RRDMGL_Balances_Atms_Daily_AUDI() : base() { }

        public int SeqNo;
        public string OriginFileName;
        public int LoadedAtRMCycle;
        public DateTime Cut_Off_Date; 

        public string MatchingCateg;
        public string AtmNo;
       
        public string Ccy_Cash;
        public string GL_Acc_ATM_Cash;
        public decimal GL_Bal_ATM_Cash;

        public string Ccy_Inter;
        public string GL_Acc_ATM_Inter;
        public decimal GL_Bal_ATM_Inter;

        public string Ccy_Excess;
        public string GL_Acc_ATM_Excess;
        public decimal GL_Bal_ATM_Excess;

        public string Ccy_Short;
        public string GL_Acc_ATM_Short;
        public decimal GL_Bal_ATM_Short;

        public DateTime DateCreated;
        public bool Processed;

        public int ProcessedAtRMCycle;
        public int ReplCycleNo; // Taken at the time of processing
        public DateTime ReplStartDtTm; 
        public decimal OpenningBalance; // Last Replenishment 

        public decimal Withdrawls_JNL; // From last repl to end of day of this Cap date - based on Journal
        public decimal Deposits_JNL; // Same
        public decimal Net_Balance_JNL; // = to oppening - withdrawls + Deposits
        public decimal GL_Difference_JNL; // Is the Net_Balance minus the GL_atm cash

        public decimal Withdrawls_IST; // From last repl to end of day of this Cap date - based on Journal
        public decimal Deposits_IST; // Same
        public decimal Net_Balance_IST; // = to oppening - withdrawls + Deposits
        public decimal GL_Difference_IST; // Is the Net_Balance minus the GL_atm cash

        public string Operator; 
    
        // Define the data table 
        public DataTable TableGL_Balances_Atms_Daily_AUDI = new DataTable();

      //  public DataTable TableGL_Balances_Atms_Daily_AUDI_2 = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Read Table Fields
        private void ReadTableFields(SqlDataReader rdr)
        {
            
            SeqNo = (int)rdr["SeqNo"];
            OriginFileName = (string)rdr["OriginFileName"];
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
            Cut_Off_Date = (DateTime)rdr["Cut_Off_Date"];

            MatchingCateg = (string)rdr["MatchingCateg"];
            AtmNo = (string)rdr["AtmNo"];

            Ccy_Cash = (string)rdr["Ccy_Cash"];
            GL_Acc_ATM_Cash = (string)rdr["GL_Acc_ATM_Cash"];
            GL_Bal_ATM_Cash = (decimal)rdr["GL_Bal_ATM_Cash"];


            Ccy_Inter = (string)rdr["Ccy_Inter"];
            GL_Acc_ATM_Inter = (string)rdr["GL_Acc_ATM_Inter"];
            GL_Bal_ATM_Inter = (decimal)rdr["GL_Bal_ATM_Inter"];

           
            Ccy_Excess = (string)rdr["Ccy_Excess"];
            GL_Acc_ATM_Excess = (string)rdr["GL_Acc_ATM_Excess"];
            GL_Bal_ATM_Excess = (decimal)rdr["GL_Bal_ATM_Excess"];

            Ccy_Short = (string)rdr["Ccy_Short"];
            GL_Acc_ATM_Short = (string)rdr["GL_Acc_ATM_Short"];
            GL_Bal_ATM_Short = (decimal)rdr["GL_Bal_ATM_Short"];

            DateCreated = (DateTime)rdr["DateCreated"];

            // FOR UPDATING 
            Processed = (bool)rdr["Processed"];
            ProcessedAtRMCycle = (int)rdr["ProcessedAtRMCycle"];
            ReplCycleNo = (int)rdr["ReplCycleNo"];

            ReplStartDtTm = (DateTime)rdr["ReplStartDtTm"];

            OpenningBalance = (decimal)rdr["OpenningBalance"];

            Withdrawls_JNL = (decimal)rdr["Withdrawls_JNL"];
            Deposits_JNL = (decimal)rdr["Deposits_JNL"];
            Net_Balance_JNL = (decimal)rdr["Net_Balance_JNL"];
            GL_Difference_JNL = (decimal)rdr["GL_Difference_JNL"];

            Withdrawls_IST = (decimal)rdr["Withdrawls_IST"];
            Deposits_IST = (decimal)rdr["Deposits_IST"];
            Net_Balance_IST = (decimal)rdr["Net_Balance_IST"];
            GL_Difference_IST = (decimal)rdr["GL_Difference_IST"];

            Operator = (string)rdr["Operator"];       

        }

        //
        // Methods 
        // READ GL_Balances_Atms_Daily
        // FILL UP A TABLE
        //
        public void ReadGL_Balances_Atms_Daily_AUDI_Table(string InSelectionCriteria, DateTime InCut_Off_Date, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableGL_Balances_Atms_Daily_AUDI = new DataTable();
            TableGL_Balances_Atms_Daily_AUDI.Clear();

            TotalSelected = 0;

            if (InMode == 1)
            {
                // No Date
                SqlString = " SELECT * "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily] "
                    + InSelectionCriteria;
            }

            if (InMode == 2)
            {
                // Only Cut_Off_Date
                //
                SqlString = " SELECT * "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily] "
                    + " WHERE Cut_Off_Date = @Cut_Off_Date "
                    + " ORDER BY AtmNo ";
            }

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        if (InMode == 2)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);
                        }
                            
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableGL_Balances_Atms_Daily_AUDI);

                        // Close conn
                        conn.Close();

                       // InsertReport(InOperator, InSignedId, TableSessionsDataCombined);

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
        // READ GL_Balances_Atms_Daily
        // FILL UP A TABLE SMART 
        //
        public void ReadGL_Balances_Atms_Daily_AUDI_Table_Short(string InSelectionCriteria, DateTime InCut_Off_Date
             , DateTime InFromDt, DateTime InToDt, string InAtmNo, int InMode
            )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAtmsClass Ac = new RRDMAtmsClass(); 

            TableGL_Balances_Atms_Daily_AUDI = new DataTable();
            TableGL_Balances_Atms_Daily_AUDI.Clear();

            // Table Definition 

            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("SeqNo", typeof(int));
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("Color", typeof(int));
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("Cut_Off_Date", typeof(DateTime));

            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("AtmNo", typeof(string));
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("AtmName", typeof(string));

            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("ReplStartDtTm", typeof(DateTime));

            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("GL_Bal_ATM_Cash", typeof(decimal));

            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("OpenningBalance", typeof(decimal));
            // JNL
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("Withdrawls_JNL", typeof(decimal));
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("Deposits_JNL", typeof(decimal));
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("Net_Balance_JNL", typeof(decimal));
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("GL_Difference_JNL", typeof(decimal));
            // IST
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("Withdrawls_IST", typeof(decimal));
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("Deposits_IST", typeof(decimal));
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("Net_Balance_IST", typeof(decimal));
            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("GL_Difference_IST", typeof(decimal));

            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("Status", typeof(string));

            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("DaysInDiff", typeof(int));

            TableGL_Balances_Atms_Daily_AUDI.Columns.Add("LastDayReconc", typeof(DateTime));

            TotalSelected = 0;

            if (InMode == 1)
            {
                // No Date
                SqlString = " SELECT * "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily] "
                    + InSelectionCriteria
                      + " ORDER By Cut_Off_Date DESC, AtmNo ASC ";
            }

            if (InMode == 2)
            {
                // Only Cut_Off_Date
                //
                SqlString = " SELECT * "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily] "
                   + " WHERE AtmNo = @AtmNo AND Cut_Off_Date BETWEEN @FromDt AND @ToDt "
                     + " ORDER By Cut_Off_Date DESC , AtmNo ASC " ;
            }

            using (SqlConnection conn =
                       new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);

                        if (InMode == 2)
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            DataRow RowSelected = TableGL_Balances_Atms_Daily_AUDI.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Color"] = 11 ;
                            RowSelected["Cut_Off_Date"] = Cut_Off_Date;
                           
                            RowSelected["AtmNo"] = AtmNo;
                            Ac.ReadAtm(AtmNo); 
                            RowSelected["AtmName"] = Ac.AtmName;

                            RowSelected["ReplStartDtTm"] = ReplStartDtTm;
                            RowSelected["GL_Bal_ATM_Cash"] = GL_Bal_ATM_Cash;
                            RowSelected["OpenningBalance"] = OpenningBalance;

                            RowSelected["Withdrawls_JNL"] = Withdrawls_JNL;
                            RowSelected["Deposits_JNL"] = Deposits_JNL;
                            RowSelected["Net_Balance_JNL"] = Net_Balance_JNL;
                            RowSelected["GL_Difference_JNL"] = GL_Difference_JNL;

                            RowSelected["Withdrawls_IST"] = Withdrawls_IST;
                            RowSelected["Deposits_IST"] = Deposits_IST;
                            RowSelected["Net_Balance_IST"] = Net_Balance_IST;
                            RowSelected["GL_Difference_IST"] = GL_Difference_IST;

                            if (GL_Difference_JNL == 0)
                            {
                                RowSelected["Status"] = "GL is OK";
                                RowSelected["Color"] = 11;
                            }
                            else
                            {
                                RowSelected["Status"] = "GL in Difference";
                                RowSelected["Color"] = 12;
                                int DaysInDiff = ReadGL_Balances_Atms_Daily_how_Many_Consecutive(AtmNo);
                               
                                RowSelected["DaysInDiff"] = DaysInDiff;

                                RowSelected["LastDayReconc"] = LastDate ;

                            }
                            

                            // ADD ROW
                            TableGL_Balances_Atms_Daily_AUDI.Rows.Add(RowSelected);
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

        // Insert 
        public void InsertReport(string InOperator, string InSignedId, DataTable InTable)
        {

            if (InTable.Rows.Count > 0)
            {
                //Clear REPORT Table 
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport79();

                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport79]";

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
        // Methods 
        // READ  
        // 
        //
        public void ReadGL_Balances_Atms_DailyBySelectionCriteria(string  InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

           
                // No Date
                SqlString = " SELECT * "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily] "
                    + InSelectionCriteria;
          
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

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
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
        // Count days in Difference
        //
        DateTime LastDate; 
        public int ReadGL_Balances_Atms_Daily_how_Many_Consecutive(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            LastDate = NullPastDate; 

            int TotalSelected = 0; 

            // No Date
            SqlString = " SELECT * "
                + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily] "
                + " WHERE AtmNo = @AtmNo"
                + " ORDER BY SeqNo DESC"
                ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTableFields(rdr);

                            if (GL_Difference_JNL == 0)
                            {
                                LastDate = Cut_Off_Date; 
                                break; 
                            }
                            else
                            {
                                TotalSelected = TotalSelected + 1;
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

            return TotalSelected; 
        }

        //
        // READ PER AtmNo and CUTOff DATE
        //
        public int ReadGL_Balances_Atms_Daily_ATM_CUT_OFF(string InAtmNo, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            LastDate = NullPastDate;

            int TotalSelected = 0;

            // No Date
            SqlString = " SELECT * "
                + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily] "
                + " WHERE AtmNo = @AtmNo AND Cut_Off_Date = @Cut_Off_Date "
                + " "
                ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTableFields(rdr);

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

            return TotalSelected;
        }

        //
        // Methods 
        // READ  by SeqNo
        // 
        //
        public void ReadGL_Balances_Atms_DailyBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily] "
                      + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
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
        // 
        // UPDATE CALCULATED GL FOR ALL ATMS 
        // 
        //
        public void UpdateCalculatedGL_For_All_ATMs(int InProcessedAtRMCycle, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingTxns_InGeneralTables_BDC Gt = new RRDMMatchingTxns_InGeneralTables_BDC();

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Class Traces 

            string WSelectionCriteria = ""; 
            ReadGL_Balances_Atms_Daily_AUDI_Table(WSelectionCriteria, InCut_Off_Date, 2);
            //
            // UPDATE RECORD DETAILS OF THE REPLENISHMENT IN PROGRESS
            //
            int I = 0;

            while (I <= (TableGL_Balances_Atms_Daily_AUDI.Rows.Count - 1))
            {
                // GET ALL fields

                int WSeqNo = (int)TableGL_Balances_Atms_Daily_AUDI.Rows[I]["SeqNo"];

                ReadGL_Balances_Atms_DailyBySeqNo(WSeqNo);

                // Get Last less than that date 

                Ta.READ_Cycle_Last_WithinDate(AtmNo, InCut_Off_Date); 

               // Ta.READ_CycleIn_Progress(AtmNo);

                ReplCycleNo = Ta.SesNo; // Taken at the time of processing

                ReplStartDtTm = Ta.SesDtTimeStart; 

                Na.ReadSessionsNotesAndValues(AtmNo, ReplCycleNo, 2);

                OpenningBalance = Na.ReplAmountTotal;

                ProcessedAtRMCycle = InProcessedAtRMCycle;

                Update_GL_Balances_Atms_Daily(WSeqNo); 

               // public decimal OpenningBalance; // Last Replenishment 

                I++; // Read Next entry of the table 

            }

            int K;
            decimal WithdrawlsTotals_JNL; 
            decimal WDepositsTotals_JNL;
            decimal WithdrawlsTotals_IST ;
            decimal WDepositsTotals_IST ;
            I = 0;

            while (I <= (TableGL_Balances_Atms_Daily_AUDI.Rows.Count - 1))
            {
                // GET ALL fields

                //    RecordFound = true;
                int WSeqNo = (int)TableGL_Balances_Atms_Daily_AUDI.Rows[I]["SeqNo"];
                decimal WOpenningBalance = (decimal)TableGL_Balances_Atms_Daily_AUDI.Rows[I]["OpenningBalance"];

                ReadGL_Balances_Atms_DailyBySeqNo(WSeqNo); 
                //
                // FIND TOTALS BASED ON JOURNALS
                //
                Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Previous_END_CAP_DATE_2(AtmNo,
                     Cut_Off_Date, ReplStartDtTm, 2);

                WithdrawlsTotals_JNL = 0;
                WDepositsTotals_JNL = 0;

                K = 0;
                while (K <= (Gt.DataTableAllFields.Rows.Count - 1))
                {
                    int WTransType = (int)Gt.DataTableAllFields.Rows[K]["TransType"];
                    decimal TotalAmt = (decimal)Gt.DataTableAllFields.Rows[K]["TotalAmt"];

                    if (WTransType ==11)
                    {
                        WithdrawlsTotals_JNL = WithdrawlsTotals_JNL + TotalAmt; 
                    }

                    if (WTransType == 23)
                    {
                        WDepositsTotals_JNL = WDepositsTotals_JNL + TotalAmt; 
                    }

                    K = K + 1; 
                }

                //
                // FIND TOTALS BASED ON IST 
                //
                Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Previous_END_CAP_DATE_3(AtmNo,
                    Cut_Off_Date, ReplStartDtTm, 2);

                WithdrawlsTotals_IST = 0;
                WDepositsTotals_IST = 0;

                K = 0;
                while (K <= (Gt.DataTableAllFields.Rows.Count - 1))
                {
                    int WTransType = (int)Gt.DataTableAllFields.Rows[K]["TransType"];
                    decimal TotalAmt = (decimal)Gt.DataTableAllFields.Rows[K]["TotalAmt"];

                    if (WTransType == 11)
                    {
                        WithdrawlsTotals_IST = WithdrawlsTotals_IST + TotalAmt;
                    }

                    if (WTransType == 23)
                    {
                        WDepositsTotals_IST = WDepositsTotals_IST + TotalAmt;
                    }

                    K = K + 1;
                }

                // Assign The Values Journal related 
                Withdrawls_JNL = WithdrawlsTotals_JNL;
                Deposits_JNL = WDepositsTotals_JNL;
                Net_Balance_JNL = OpenningBalance - Withdrawls_JNL + Deposits_JNL;

                GL_Difference_JNL = Net_Balance_JNL - GL_Bal_ATM_Cash;

                // Assign The Values IST realted 
                Withdrawls_IST = WithdrawlsTotals_IST;
                Deposits_IST = WDepositsTotals_IST;
                Net_Balance_IST = OpenningBalance - Withdrawls_IST + Deposits_IST;

                GL_Difference_IST = Net_Balance_IST - GL_Bal_ATM_Cash;

                Processed = true;

                ProcessedAtRMCycle = InProcessedAtRMCycle; 

                Update_GL_Balances_Atms_Daily(WSeqNo); 

                I++; // Read Next entry of the table 

            }
        }


        // 
        // UPDATE GL_Balances_Atms_Daily
        // UPON counted input you do this Update
        //
        public void Update_GL_Balances_Atms_Daily(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows = 0;
           // ReplStartDtTm = (DateTime)rdr["ReplStartDtTm"];
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily] SET "
                            + "[Processed]=@Processed,"
                             + "[ProcessedAtRMCycle]=@ProcessedAtRMCycle,"
                            + "[ReplCycleNo]=@ReplCycleNo,"
                            + "[ReplStartDtTm]=@ReplStartDtTm,"

                            + "[OpenningBalance] =@OpenningBalance,"

                            + "[Withdrawls_JNL] = @Withdrawls_JNL,"
                            + " [Deposits_JNL] = @Deposits_JNL, "
                            + " [Net_Balance_JNL] = @Net_Balance_JNL, "
                            + " [GL_Difference_JNL] = @GL_Difference_JNL,"

                            + "[Withdrawls_IST] = @Withdrawls_IST,"
                            + " [Deposits_IST] = @Deposits_IST, "
                            + " [Net_Balance_IST] = @Net_Balance_IST, "
                            + " [GL_Difference_IST] = @GL_Difference_IST"

                            + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@Processed", Processed); 

                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", ProcessedAtRMCycle);

                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);
                        cmd.Parameters.AddWithValue("@ReplStartDtTm", ReplStartDtTm);

                        cmd.Parameters.AddWithValue("@OpenningBalance", OpenningBalance);
                        //********************
                        cmd.Parameters.AddWithValue("@Withdrawls_JNL", Withdrawls_JNL);
                        cmd.Parameters.AddWithValue("@Deposits_JNL", Deposits_JNL);
                        cmd.Parameters.AddWithValue("@Net_Balance_JNL", Net_Balance_JNL);
                        cmd.Parameters.AddWithValue("@GL_Difference_JNL", GL_Difference_JNL);

                        cmd.Parameters.AddWithValue("@Withdrawls_IST", Withdrawls_IST);
                        cmd.Parameters.AddWithValue("@Deposits_IST", Deposits_IST);
                        cmd.Parameters.AddWithValue("@Net_Balance_IST", Net_Balance_IST);
                        cmd.Parameters.AddWithValue("@GL_Difference_IST", GL_Difference_IST);

                        // Execute and check success 
                        rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //    outcome = " ATMs Table UPDATED ";
                        }

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

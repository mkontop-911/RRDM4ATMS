using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDM_Journal_TransactionSummary : Logger
    {
        public RRDM_Journal_TransactionSummary() : base() { }

        //        RAF17002 varchar(50) Checked
        //fuid    bigint Checked
        //LoadedAtRMCycle bigint  Checked
        //TRACE   varchar(8)  Checked
        //SeqNo   int Checked
        //TRANDESC varchar(50) Checked
        //TRANDATE    datetime Checked
        //TRANTIME varchar(50) Checked
        //TransType   varchar(50) Checked
        //ATMTYPE varchar(50) Checked
        public string RAF17002;
        public int fuid;
        public int LoadedAtRMCycle;
        public string TRACE;
        public int SeqNo;
        public string TRANDESC;
        public DateTime TRANDATE;
        public string TransType;
        public string ATMTYPE;

        //        OPEN_DENOM_TYP1 int Checked
        //OPEN_DENOM_TYP2 int Checked
        //OPEN_DENOM_TYP3 int Checked
        //OPEN_DENOM_TYP4 int Checked
        //OPEN_DIS_TYPE1 int Checked
        //OPEN_DIS_TYPE2 int Checked
        //OPEN_DIS_TYPE3 int Checked
        //OPEN_DIS_TYPE4 int Checked
        //OPEN_REJ_TYPE1 int Checked
        //OPEN_REJ_TYPE2 int Checked
        //OPEN_REJ_TYPE3 int Checked
        //OPEN_REJ_TYPE4 int Checked
        //OPEN_REMAINING_TYPE1 int Checked
        //OPEN_REMAINING_TYPE2 int Checked
        //OPEN_REMAINING_TYPE3 int Checked
        //OPEN_REMAINING_TYPE4 int Checked

        public int OPEN_DENOM_TYP1;
        public int OPEN_DENOM_TYP2;
        public int OPEN_DENOM_TYP3;
        public int OPEN_DENOM_TYP4;

        public int OPEN_DIS_TYPE1;
        public int OPEN_DIS_TYPE2;
        public int OPEN_DIS_TYPE3;
        public int OPEN_DIS_TYPE4;

        public int OPEN_REJ_TYPE1;
        public int OPEN_REJ_TYPE2;
        public int OPEN_REJ_TYPE3;
        public int OPEN_REJ_TYPE4;

        public int OPEN_REMAINING_TYPE1;
        public int OPEN_REMAINING_TYPE2;
        public int OPEN_REMAINING_TYPE3;
        public int OPEN_REMAINING_TYPE4;
    
        public decimal TXN_AMOUNT_CR_DR;

        public int RON1;
        public int RON5;
        public int RON10;
        public int RON20;
        public int RON50;
        public int RON100;
        public int RON200;
        public int RON500;

        public int RON1_Recycle_Total;
        public int RON5_Recycle_Total;
        public int RON10_Recycle_Total;
        public int RON20_Recycle_Total;
        public int RON50_Recycle_Total;
        public int RON100_Recycle_Total;
        public int RON200_Recycle_Total;
        public int RON500_Recycle_Total;

        public int RON1_NONRecycle_Total;
        public int RON5_NONRecycle_Total;
        public int RON10_NONRecycle_Total;
        public int RON20_NONRecycle_Total;
        public int RON50_NONRecycle_Total;
        public int RON100_NONRecycle_Total;
        public int RON200_NONRecycle_Total;
        public int RON500_NONRecycle_Total;

       
        public int NotesPresented_TYPE1;
        public int NotesPresented_TYPE2;
        public int NotesPresented_TYPE3;
        public int NotesPresented_TYPE4;

        // CLOSE_DENOM_TYP1 int Checked
        public int CLOSE_DENOM_TYP1;
        public int CLOSE_DENOM_TYP2;
        public int CLOSE_DENOM_TYP3;
        public int CLOSE_DENOM_TYP4;

        public int CLOSE_DIS_TYPE1;
        public int CLOSE_DIS_TYPE2;
        public int CLOSE_DIS_TYPE3;
        public int CLOSE_DIS_TYPE4;

        public int CLOSE_REJ_TYPE1;
        public int CLOSE_REJ_TYPE2;
        public int CLOSE_REJ_TYPE3;
        public int CLOSE_REJ_TYPE4;

        public int CLOSE_REMAINING_TYPE1;
        public int CLOSE_REMAINING_TYPE2;
        public int CLOSE_REMAINING_TYPE3;
        public int CLOSE_REMAINING_TYPE4;


        //JOURNAL_DIS_TYPE1 int Checked;

        public int JOURNAL_DIS_TYPE1;
        public int JOURNAL_DIS_TYPE2;
        public int JOURNAL_DIS_TYPE3;
        public int JOURNAL_DIS_TYPE4;

        public int JOURNAL_REJ_TYPE1;
        public int JOURNAL_REJ_TYPE2;
        public int JOURNAL_REJ_TYPE3;
        public int JOURNAL_REJ_TYPE4;

        public int JOURNAL_REMAINING_TYPE1;
        public int JOURNAL_REMAINING_TYPE2;
        public int JOURNAL_REMAINING_TYPE3;
        public int JOURNAL_REMAINING_TYPE4;


        // public int SeqNo;
        public string FileId;
        public string MatchingCateg;

        public string TerminalId_4;
        public string TerminalId_2;
        public string CardNumber_4;
        public string CardNumber_2;

        public string AccNo_4;
        public string AccNo_2;

        public string TransDescr_4;
        public string TransDescr_2;

        public string TransCurr_4;
        public string TransCurr_2;

        public decimal TransAmt_4;
        public decimal TransAmt_2;
        public int TraceNo_4;
        public int TraceNo_2;

        public string RRNumber_4;
        public string RRNumber_2;
        public string FullTraceno_4;
        public string FullTraceno_2;

        public int SeqNo_4;
        public int SeqNo_2;
        public string ResponseCode_4;
        public string ResponseCode_2;
        
        public DateTime TransDate_4;
        public DateTime TransDate_2;
        public string TXNSRC_4;
        public string TXNSRC_2;

        public string TXNDEST_4;
        public string TXNDEST_2;
        public int RMCycleNo;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);



        string SqlString; // Do not delete 

        //************************************************
        //

        public DataTable ReversalsDataTable = new DataTable();

        public DataTable BalancingTable = new DataTable();

        public int TotalSelected;

        readonly string ATMSconnectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        readonly string recconConnString = AppConfig.GetConnectionString("ReconConnectionString");

        // Read Fields In Table 
        private void ReadFieldsInTable(SqlDataReader rdr)
        {
 
            SeqNo = (int)rdr["SeqNo"];

            FileId = (string)rdr["FileId"];
            MatchingCateg = (string)rdr["MatchingCateg"];

            TerminalId_4 = (string)rdr["TerminalId_4"];
            TerminalId_2 = (string)rdr["TerminalId_2"];
            CardNumber_4 = (string)rdr["CardNumber_4"];
            CardNumber_2 = (string)rdr["CardNumber_2"];

            AccNo_4 = (string)rdr["AccNo_4"];
            AccNo_2 = (string)rdr["AccNo_2"];
            TransDescr_4 = (string)rdr["TransDescr_4"];
            TransDescr_2 = (string)rdr["TransDescr_2"];
            TransCurr_4 = (string)rdr["TransCurr_4"];
            TransCurr_2 = (string)rdr["TransCurr_2"];

            TransAmt_4 = (decimal)rdr["TransAmt_4"];
            TransAmt_2 = (decimal)rdr["TransAmt_2"];
            TraceNo_4 = (int)rdr["TraceNo_4"];
            TraceNo_2 = (int)rdr["TraceNo_2"];

            RRNumber_4 = (string)rdr["RRNumber_4"];
            RRNumber_2 = (string)rdr["RRNumber_2"];
            FullTraceno_4 = (string)rdr["FullTraceno_4"];
            FullTraceno_2 = (string)rdr["FullTraceno_2"];

            SeqNo_4 = (int)rdr["SeqNo_4"];
            SeqNo_2 = (int)rdr["SeqNo_2"];
            ResponseCode_4 = (string)rdr["ResponseCode_4"];
            ResponseCode_2 = (string)rdr["ResponseCode_2"];

            TransDate_4 = (DateTime)rdr["TransDate_4"];
            TransDate_2 = (DateTime)rdr["TransDate_2"];
            TXNSRC_4 = (string)rdr["TXNSRC_4"];
            TXNSRC_2 = (string)rdr["TXNSRC_2"];

            TXNDEST_4 = (string)rdr["TXNDEST_4"];
            TXNDEST_2 = (string)rdr["TXNDEST_2"];
            RMCycleNo = (int)rdr["RMCycleNo"];
            
        }

        private void ReadFieldsInTable_ROM(SqlDataReader rdr)
        {


            //    public string RAF17002;
            //public int fuid;
            //public int LoadedAtRMCycle;
            //public string TRACE;
            RAF17002 = (string)rdr["RAF17002"];
            fuid = (int)rdr["fuid"];
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
            TRACE = (string)rdr["TRACE"];

            //public int SeqNo;
            //public string TRANDESC;
            //public DateTime TRANDATE;
            //public string TransType;
            //public string ATMTYPE;
            SeqNo = (int)rdr["SeqNo"];
            TRANDESC = (string)rdr["TRANDESC"];
            TRANDATE = (DateTime)rdr["trandatetime"];
            TransType = (string)rdr["TransType"];
            ATMTYPE = (string)rdr["ATMTYPE"];
            
            OPEN_DENOM_TYP1 = (int)rdr["OPEN_DENOM_TYP1"];
            OPEN_DENOM_TYP2 = (int)rdr["OPEN_DENOM_TYP2"];
            OPEN_DENOM_TYP3 = (int)rdr["OPEN_DENOM_TYP3"];
            OPEN_DENOM_TYP4 = (int)rdr["OPEN_DENOM_TYP4"];

            OPEN_DIS_TYPE1 = (int)rdr["OPEN_DIS_TYPE1"];
            OPEN_DIS_TYPE2 = (int)rdr["OPEN_DIS_TYPE2"];
            OPEN_DIS_TYPE3 = (int)rdr["OPEN_DIS_TYPE3"];
            OPEN_DIS_TYPE4 = (int)rdr["OPEN_DIS_TYPE4"];

            OPEN_REJ_TYPE1 = (int)rdr["OPEN_REJ_TYPE1"];
            OPEN_REJ_TYPE2 = (int)rdr["OPEN_REJ_TYPE2"];
            OPEN_REJ_TYPE3 = (int)rdr["OPEN_REJ_TYPE3"];
            OPEN_REJ_TYPE4 = (int)rdr["OPEN_REJ_TYPE4"];

            OPEN_REMAINING_TYPE1 = (int)rdr["OPEN_REMAINING_TYPE1"];
            OPEN_REMAINING_TYPE2 = (int)rdr["OPEN_REMAINING_TYPE2"];
            OPEN_REMAINING_TYPE3 = (int)rdr["OPEN_REMAINING_TYPE3"];
            OPEN_REMAINING_TYPE4 = (int)rdr["OPEN_REMAINING_TYPE4"];
          
         

            //     public decimal TXN_AMOUNT_CR_DR;
            TXN_AMOUNT_CR_DR = (decimal)rdr["TXN_AMOUNT_CR_DR"];

            //public int RON1;
            //public int RON5;
            //public int RON10;
            //public int RON20;
            RON1 = (int)rdr["RON1"];
            RON5 = (int)rdr["RON5"];
            RON10 = (int)rdr["RON10"];
            RON20 = (int)rdr["RON20"];
         
            RON50 = (int)rdr["RON50"];
            RON100 = (int)rdr["RON100"];
            RON200 = (int)rdr["RON200"];
            RON500 = (int)rdr["RON500"];

            RON1_Recycle_Total = (int)rdr["RON1_Recycle_Total"];
            RON5_Recycle_Total = (int)rdr["RON5_Recycle_Total"];
            RON10_Recycle_Total = (int)rdr["RON10_Recycle_Total"];
            RON20_Recycle_Total = (int)rdr["RON20_Recycle_Total"];

            RON50_Recycle_Total = (int)rdr["RON50_Recycle_Total"];
            RON100_Recycle_Total = (int)rdr["RON100_Recycle_Total"];
            RON200_Recycle_Total = (int)rdr["RON200_Recycle_Total"];
            RON500_Recycle_Total = (int)rdr["RON500_Recycle_Total"];

            RON1_NONRecycle_Total = (int)rdr["RON1_NONRecycle_Total"];
            RON5_NONRecycle_Total = (int)rdr["RON5_NONRecycle_Total"];
            RON10_NONRecycle_Total = (int)rdr["RON10_NONRecycle_Total"];
            RON20_NONRecycle_Total = (int)rdr["RON20_NONRecycle_Total"];

            RON50_NONRecycle_Total = (int)rdr["RON50_NONRecycle_Total"];
            RON100_NONRecycle_Total = (int)rdr["RON100_NONRecycle_Total"];
            RON200_NONRecycle_Total = (int)rdr["RON200_NONRecycle_Total"];
            RON500_NONRecycle_Total = (int)rdr["RON500_NONRecycle_Total"];



            NotesPresented_TYPE1 = (int)rdr["NotesPresented_TYPE1"];
            NotesPresented_TYPE2 = (int)rdr["NotesPresented_TYPE2"];
            NotesPresented_TYPE3 = (int)rdr["NotesPresented_TYPE3"];
            NotesPresented_TYPE4 = (int)rdr["NotesPresented_TYPE4"];

            // *********

            CLOSE_DENOM_TYP1 = (int)rdr["CLOSE_DENOM_TYP1"];
            CLOSE_DENOM_TYP2 = (int)rdr["CLOSE_DENOM_TYP2"];
            CLOSE_DENOM_TYP3 = (int)rdr["CLOSE_DENOM_TYP3"];
            CLOSE_DENOM_TYP4 = (int)rdr["CLOSE_DENOM_TYP4"];

            CLOSE_DIS_TYPE1 = (int)rdr["CLOSE_DIS_TYPE1"];
            CLOSE_DIS_TYPE2 = (int)rdr["CLOSE_DIS_TYPE2"];
            CLOSE_DIS_TYPE3 = (int)rdr["CLOSE_DIS_TYPE3"];
            CLOSE_DIS_TYPE4 = (int)rdr["CLOSE_DIS_TYPE4"];

            CLOSE_REJ_TYPE1 = (int)rdr["CLOSE_REJ_TYPE1"];
            CLOSE_REJ_TYPE2 = (int)rdr["CLOSE_REJ_TYPE2"];
            CLOSE_REJ_TYPE3 = (int)rdr["CLOSE_REJ_TYPE3"];
            CLOSE_REJ_TYPE4 = (int)rdr["CLOSE_REJ_TYPE4"];

            CLOSE_REMAINING_TYPE1 = (int)rdr["CLOSE_REMAINING_TYPE1"];
            CLOSE_REMAINING_TYPE2 = (int)rdr["CLOSE_REMAINING_TYPE2"];
            CLOSE_REMAINING_TYPE3 = (int)rdr["CLOSE_REMAINING_TYPE3"];
            CLOSE_REMAINING_TYPE4 = (int)rdr["CLOSE_REMAINING_TYPE4"];


            // *************

            JOURNAL_DIS_TYPE1 = (int)rdr["JOURNAL_DIS_TYPE1"];
            JOURNAL_DIS_TYPE2 = (int)rdr["JOURNAL_DIS_TYPE2"];
            JOURNAL_DIS_TYPE3 = (int)rdr["JOURNAL_DIS_TYPE3"];
            JOURNAL_DIS_TYPE4 = (int)rdr["JOURNAL_DIS_TYPE4"];

            JOURNAL_REJ_TYPE1 = (int)rdr["JOURNAL_REJ_TYPE1"];
            JOURNAL_REJ_TYPE2 = (int)rdr["JOURNAL_REJ_TYPE2"];
            JOURNAL_REJ_TYPE3 = (int)rdr["JOURNAL_REJ_TYPE3"];
            JOURNAL_REJ_TYPE4 = (int)rdr["JOURNAL_REJ_TYPE4"];

            JOURNAL_REMAINING_TYPE1 = (int)rdr["JOURNAL_REMAINING_TYPE1"];
            JOURNAL_REMAINING_TYPE2 = (int)rdr["JOURNAL_REMAINING_TYPE2"];
            JOURNAL_REMAINING_TYPE3 = (int)rdr["JOURNAL_REMAINING_TYPE3"];
            JOURNAL_REMAINING_TYPE4 = (int)rdr["JOURNAL_REMAINING_TYPE4"];

        }
        //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 
        public void ReadTXN_BALANCINGAndCorrect(string InATM_NO, DateTime InDateTmFrom, DateTime InDateTmTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            BalancingTable = new DataTable();
            BalancingTable.Clear();
            //string WATMNo = 
            //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            //Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);


            //// DATA TABLE ROWS DEFINITION 
            ///
            string SqlString = " SELECT SeqNo "
                              + "  ,[RON1] "
                              + "  ,[RON5] "
                              + "  ,[RON10] "
                              + "  ,[RON20] "
                              + "  ,[RON50] "
                              + "  ,[RON100] "
                              + "  ,[RON200] "
                              + "  ,[RON500] "
                              + "  FROM [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary] "
                              + " WHERE RAF17002 = @ATM_NO "
                              + " AND trandatetime > @trandatetime_From AND trandatetime<= @trandatetime_To "
                             + " ORDER By trandatetime,  trace ";

            using (SqlConnection conn =
             new SqlConnection(recconConnString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ATM_NO", InATM_NO);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@trandatetime_From", InDateTmFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@trandatetime_To", InDateTmTo);
                        //Create a datatable that will be filled with the data retrieved from the command

                        //sqlAdapt.UpdateCommand.CommandTimeout = 300;  // seconds

                        sqlAdapt.Fill(BalancingTable);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            // Define fields 
            // 10, 50, 100 are recycle
            int WSeqNo; 
            int WRON1 = 0;
            int WRON5 = 0;
            int WRON10 = 0;
            int WRON20 = 0;
            int WRON50 = 0;
            int WRON100 = 0;
            int WRON200 = 0;
            int WRON500 = 0;

        RON1_NONRecycle_Total = 0;
        RON5_NONRecycle_Total = 0;
        RON10_Recycle_Total = 0;
        RON20_NONRecycle_Total = 0;
        RON50_Recycle_Total = 0;
        RON100_Recycle_Total = 0;
        RON200_NONRecycle_Total = 0;
        RON500_NONRecycle_Total = 0;

        int Count = 0;

            int I = 0;
            // 
            while (I <= (BalancingTable.Rows.Count - 1))
            {
                // READ
                WSeqNo = (int)BalancingTable.Rows[I]["SeqNo"];

                if (WSeqNo == 35803 || WSeqNo == 35992 || WSeqNo == 36064)
                {
                    WSeqNo = WSeqNo;
                }

                WRON1 = (int)BalancingTable.Rows[I]["RON1"];
                WRON5 = (int)BalancingTable.Rows[I]["RON5"];
                WRON10 = (int)BalancingTable.Rows[I]["RON10"];
                WRON20 = (int)BalancingTable.Rows[I]["RON20"];
                WRON50 = (int)BalancingTable.Rows[I]["RON50"];
                WRON100 = (int)BalancingTable.Rows[I]["RON100"];
                WRON200 = (int)BalancingTable.Rows[I]["RON200"];
                WRON500 = (int)BalancingTable.Rows[I]["RON500"];

                //ADD TO TOTALS
                RON1_NONRecycle_Total = RON1_NONRecycle_Total + WRON1;
                RON5_NONRecycle_Total = RON5_NONRecycle_Total + WRON5;
                RON10_Recycle_Total = RON10_Recycle_Total + WRON10; // recycle 
                RON20_NONRecycle_Total = RON20_NONRecycle_Total + WRON20; 
                RON50_Recycle_Total = RON50_Recycle_Total + WRON50; // Recycle 
                RON100_Recycle_Total = RON100_Recycle_Total + WRON100; // Recycle
                RON200_NONRecycle_Total = RON200_NONRecycle_Total + WRON200;
                RON500_NONRecycle_Total = RON500_NONRecycle_Total + WRON500;

                if (WRON500> 0)
                {
                    WRON500 = WRON500; 
                }

                Update_TOTALS_RECYCLE_AND_NOT(WSeqNo); 

                I = I + 1;
            }




        }

        //
        // READ Reversals and created table as per request.
        //
        public void ReadReversalsAndFillTable(string InUserId, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            // CLEAR TABLE
            ClearDefineTable();

            string SqlString = "SELECT *"
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs] "
                 + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {
                        //  cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            AddLineToTable(InUserId);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();


                    InsertReport(InUserId, ReversalsDataTable); 
                   // InsertRecordsOfReport(InUserId);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // Clear Define Table
        private void ClearDefineTable()
        {
            ReversalsDataTable = new DataTable();
            ReversalsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ReversalsDataTable.Columns.Add("SeqNo", typeof(int));
            ReversalsDataTable.Columns.Add("RevSeqNo1", typeof(int));
            ReversalsDataTable.Columns.Add("Terminal", typeof(string));
            ReversalsDataTable.Columns.Add("Descr", typeof(string));
            ReversalsDataTable.Columns.Add("Card", typeof(string));
            ReversalsDataTable.Columns.Add("Account", typeof(string));
            ReversalsDataTable.Columns.Add("Ccy", typeof(string));
            ReversalsDataTable.Columns.Add("Amount", typeof(decimal));
            ReversalsDataTable.Columns.Add("Date", typeof(DateTime));
            ReversalsDataTable.Columns.Add("TraceNo", typeof(int));
            ReversalsDataTable.Columns.Add("RRNumber", typeof(string));
            ReversalsDataTable.Columns.Add("RespCode", typeof(string));
            ReversalsDataTable.Columns.Add("FileId", typeof(string));
            ReversalsDataTable.Columns.Add("Category", typeof(string));
            ReversalsDataTable.Columns.Add("RevSeqNo2", typeof(int));
            ReversalsDataTable.Columns.Add("RevResp", typeof(string));
            ReversalsDataTable.Columns.Add("UserId", typeof(string));
        }

     
        // Insert 
        public void InsertReport(string InUserId, DataTable InTable)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport55_3(InUserId);

            if (InTable.Rows.Count > 0)
            {
                // RECORDS READ AND PROCESSED 
                
                using (SqlConnection conn =
                               new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport55_3]";

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

        // ADD LINE TO TABLE
        //RRDMAccountsClass Acc = new RRDMAccountsClass();
        private void AddLineToTable(string InUserId)
        {
            //
            // Fill In Table
            //

            DataRow RowSelected = ReversalsDataTable.NewRow();

            RowSelected["SeqNo"] = SeqNo;
            RowSelected["RevSeqNo1"] = SeqNo_2;
            RowSelected["Terminal"] = TerminalId_2;
            RowSelected["Descr"] = TransDescr_2;
            RowSelected["Card"] = CardNumber_2;
            RowSelected["Account"] = AccNo_2;
           
            RowSelected["Ccy"] = TransCurr_2;
            RowSelected["Amount"] = TransAmt_2;
            RowSelected["Date"] = TransDate_2;
            RowSelected["TraceNo"] = TraceNo_2;
            
            RowSelected["RRNumber"] = RRNumber_2;
            RowSelected["RespCode"] = ResponseCode_2;
            RowSelected["FileId"] = FileId;
            RowSelected["Category"] = MatchingCateg;
         
            RowSelected["RevSeqNo2"] = SeqNo_4;
            RowSelected["RevResp"] = ResponseCode_4;

            RowSelected["UserId"] = InUserId;

            // ADD ROW
            ReversalsDataTable.Rows.Add(RowSelected);
        }


        public int Count_11;
        public int Count_23;
        //
        // READ SPECIFIC TRANSACTION based on Number 
        //
        //  string WorkingTableName;
        public void ReadTXN_By_Selection_criteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary] "
                  + InSelectionCriteria
                  + " ";

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable_ROM(rdr);

                           
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
        public int Toteur5;
        public int Toteur10;
        public int Toteur20;
        public int Toteur50;

        public int Toteur100;
        public int Toteur200;
        public int Toteur500;
        // Get totals for FX
        //Ts.ReadAND_Get_FX_TOTALS(WAtmNo, W_DtFrom, W_DtTo); 
        public void ReadAND_Get_FX_TOTALS(string InAtmNo, DateTime InDtFrom, DateTime InDtTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " select sum(cast(eur5 as int)) as Toteur5 "
              + ",sum(cast(eur10 as int)) as Toteur10 "
              + ",sum(cast(eur20 as int)) as Toteur20 "
              + ",sum(cast(eur50 as int)) as Toteur50 "

              + ",sum(cast(eur100 as int)) as Toteur100 "
              + ",sum(cast(eur200 as int)) as Toteur200 "
              + ",sum(cast(eur500 as int)) as Toteur500 "
                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] "
                  + " where ATMNo= @ATMNo and (trandatetime > @DtFrom AND trandatetime<= @DtTo ) ";


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                           cmd.Parameters.AddWithValue("@ATMNo", InAtmNo);
                           cmd.Parameters.AddWithValue("@DtFrom", InDtFrom);
                           cmd.Parameters.AddWithValue("@DtTo", InDtTo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            Toteur5 = (int)rdr["Toteur5"];
                            Toteur10 = (int)rdr["Toteur10"];
                            Toteur20 = (int)rdr["Toteur20"];
                            Toteur50 = (int)rdr["Toteur50"];

                            Toteur100 = (int)rdr["Toteur100"];
                            Toteur200 = (int)rdr["Toteur200"];
                            Toteur500 = (int)rdr["Toteur500"];

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

        public int eur5 ;
        public int eur10;
        public int eur20;
        public int eur50;

        public int eur100;
        public int eur200;
        public int eur500;
   
        public void ReadAND_Get_FX_Notes(string InAtmNo,int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " select cast(eur5 as int) as eur5"
              + ", cast(eur10 as int) as eur10 "
              + ", cast(eur20 as int)  as eur20 "
              + ", cast(eur50 as int) As eur50"

              + ", cast(eur100 as int) as eur100"
              + ",cast(eur200 as int)  as eur200"
              + ",cast(eur500 as int) as eur500 "
                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] "
                  + " where ATMNo= @ATMNo and SeqNo = @SeqNo ";


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ATMNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                       
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            eur5 = (int)rdr["eur5"];
                            eur10 = (int)rdr["eur10"];
                            eur20 = (int)rdr["eur20"];
                            eur50 = (int)rdr["eur50"];

                            eur100 = (int)rdr["eur100"];
                            eur200 = (int)rdr["eur200"];
                            eur500 = (int)rdr["eur500"];

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
        public string TXNCcy; 
        public void ReadCurrencyFromtblHstAtmTxnsROM(string InAtmNo, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " Select Currency "
                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] "
                  + " where SeqNo= @SeqNo  ";


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
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

                            TXNCcy = (string)rdr["Currency"];
                           
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
        //  string WorkingTableName;
        

        // 
        // UPDATE  NULLS 2
        //
        public void Update_NULL_Values_InTxnBalances(int InReplCycle)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
     
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary]" 
                        + " SET "
                        + " [NotesPresented_TYPE1] = 0 "
                        + " ,[NotesPresented_TYPE2] = 0 "
                        + " ,[NotesPresented_TYPE3] = 0 "
                        + " ,[NotesPresented_TYPE4] = 0 "
     + "  , [JOURNAL_DIS_TYPE1] = 0 "
       + "  , [JOURNAL_DIS_TYPE2] = 0 "
         + "  , [JOURNAL_DIS_TYPE3] = 0 "
           + "  , [JOURNAL_DIS_TYPE4] = 0 "
     + "  ,[JOURNAL_REJ_TYPE1] = 0 "
     + "  ,[JOURNAL_REJ_TYPE2] = 0 "
     + "  ,[JOURNAL_REJ_TYPE3] = 0 "
     + "  ,[JOURNAL_REJ_TYPE4] = 0 "

     + "  ,[JOURNAL_REMAINING_TYPE1] = 0 "
      + "  ,[JOURNAL_REMAINING_TYPE2] = 0 "
       + "  ,[JOURNAL_REMAINING_TYPE3] = 0 "
        + "  ,[JOURNAL_REMAINING_TYPE4] = 0 "

            + " WHERE TranDesc = 'DEPOSIT' AND [LoadedAtRMCycle]  = @LoadedAtRMCycle  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReplCycle);
                      
                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            // outcome = " ATMs Table UPDATED ";
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

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary] "
                        + " SET "
                        + " [JOURNAL_REMAINING_TYPE4] = 0  "
            + " WHERE TranDesc = 'WITHDRAWAL' AND [LoadedAtRMCycle]  = @LoadedAtRMCycle  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReplCycle);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            // outcome = " ATMs Table UPDATED ";
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

            //  return outcome;

        }

        // 
        // UPDATE  NULLS 2
        //
        public void Update_TOTALS_RECYCLE_AND_NOT(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                   
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary]"
                        + " SET "
                        + " [RON1_NONRecycle_Total] = @RON1_NONRecycle_Total "
                         + " , [RON5_NONRecycle_Total] = @RON5_NONRecycle_Total "
                         + " , [RON10_Recycle_Total] = @RON10_Recycle_Total "
                         + " , [RON20_NONRecycle_Total] = @RON20_NONRecycle_Total "

                         + " , [RON50_Recycle_Total] = @RON50_Recycle_Total "
                         + " , [RON100_Recycle_Total] = @RON100_Recycle_Total "
                         + " , [RON200_NONRecycle_Total] = @RON200_NONRecycle_Total "
                         + " , [RON500_NONRecycle_Total] = @RON500_NONRecycle_Total "
  
                        + " WHERE SeqNo=@SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@RON1_NONRecycle_Total", RON1_NONRecycle_Total);
                        cmd.Parameters.AddWithValue("@RON5_NONRecycle_Total", RON5_NONRecycle_Total);
                        cmd.Parameters.AddWithValue("@RON10_Recycle_Total", RON10_Recycle_Total);
                        cmd.Parameters.AddWithValue("@RON20_NONRecycle_Total", RON20_NONRecycle_Total);

                        cmd.Parameters.AddWithValue("@RON50_Recycle_Total", RON50_Recycle_Total);
                        cmd.Parameters.AddWithValue("@RON100_Recycle_Total", RON100_Recycle_Total);
                        cmd.Parameters.AddWithValue("@RON200_NONRecycle_Total", RON200_NONRecycle_Total);
                        cmd.Parameters.AddWithValue("@RON500_NONRecycle_Total", RON500_NONRecycle_Total);

                        // Execute and check success 
                        rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            // outcome = " ATMs Table UPDATED ";
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

        // Get both tables 
        public void ReadTransSpecificFromBothTables_By_SelectionCriteria(string InSelectionCriteria, string InTableName)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Count_11 = 0;
            Count_23 = 0;

            SqlString =
           " WITH MergedTbl AS "
           + " ( "
           + " SELECT *  "
           + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTableName
           + InSelectionCriteria // Includes Where
           + " UNION ALL  "
           + " SELECT * "
           + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTableName
           + InSelectionCriteria // Includes Where
           + " ) "
           + " SELECT * FROM MergedTbl "
           + " ORDER BY SeqNo DESC "
            + "  ";


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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



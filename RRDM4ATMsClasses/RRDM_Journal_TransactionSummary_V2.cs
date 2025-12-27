using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDM_Journal_TransactionSummary_V2 : Logger
    {
        public RRDM_Journal_TransactionSummary_V2() : base() { }

        public int SeqNo;
        public string AtmNo;
        public int fuid;
        public int LoadedAtRMCycle;
       
        public int Origin_SeqNo;

        public string TRANDESC;
        public DateTime TRANDATE;
        public string TransType;
        public string ATMTYPE;

        public string TRACE;
        public string AUTHNUM;
        public string UTRNNO;

        public string Ccy;
        public decimal TXN_AMOUNT_CR_DR;

    
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

        //        eur5 int Checked
        //eur10 int Checked
        //eur20 int Checked
        //eur50 int Checked
        //eur100 int Checked
        //eur200 int Checked
        //eur500 int Checked

        public int eur5;
        public int eur10;
        public int eur20;
        public int eur50;

        public int eur100;
        public int eur200;
        public int eur500;
        // TOTALS
        public int eur5_TOT;
        public int eur10_TOT;
        public int eur20_TOT;
        public int eur50_TOT;

        public int eur100_TOT;
        public int eur200_TOT;
        public int eur500_TOT;

        public int ReplCycle;
        public int PartialRepl; 
        public string flag_process;

        

        //THIS IS FOR ANOTHER STRUCTURE
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
        // TRANSACTIONS BALANCING
        private void ReadFieldsInTable_ROM(SqlDataReader rdr)
        {


            SeqNo = (int)rdr["SeqNo"];
            AtmNo = (string)rdr["AtmNo"];
            fuid = (int)rdr["fuid"];
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
            Origin_SeqNo = (int)rdr["Origin_SeqNo"];


            TRANDESC = (string)rdr["TRANDESC"];
            TRANDATE = (DateTime)rdr["trandatetime"];
            TransType = (string)rdr["TransType"];
            ATMTYPE = (string)rdr["ATMTYPE"];

            TRACE = (string)rdr["TRACE"];
            AUTHNUM = (string)rdr["AUTHNUM"];
            UTRNNO = (string)rdr["UTRNNO"];

            Ccy = (string)rdr["Ccy"];
            TXN_AMOUNT_CR_DR = (decimal)rdr["TXN_AMOUNT_CR_DR"];

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


            //     public int eur5;
            //public int eur10;
            //public int eur20;
            //public int eur50;

            eur5 = (int)rdr["eur5"];
            eur10 = (int)rdr["eur10"];
            eur20 = (int)rdr["eur20"];
            eur50 = (int)rdr["eur50"];

            //public int eur100;
            //public int eur200;
            //public int eur500;
            eur100 = (int)rdr["eur100"];
            eur200 = (int)rdr["eur200"];
            eur500 = (int)rdr["eur500"];

            //// TOTALS
            //public int eur5_TOT;
            //public int eur10_TOT;
            //public int eur20_TOT;
            //public int eur50_TOT;
            eur5_TOT = (int)rdr["eur5_TOT"];
            eur10_TOT = (int)rdr["eur10_TOT"];
            eur20_TOT = (int)rdr["eur20_TOT"];
            eur50_TOT = (int)rdr["eur50_TOT"];

            //public int eur100_TOT;
            //public int eur200_TOT;
            //public int eur500_TOT;
            eur100_TOT = (int)rdr["eur100_TOT"];
            eur200_TOT = (int)rdr["eur200_TOT"];
            eur500_TOT = (int)rdr["eur500_TOT"];

            ReplCycle = (int)rdr["ReplCycle"];
            PartialRepl = (int)rdr["PartialRepl"];

            flag_process = (string)rdr["flag_process"];


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
            /// UPDATE RON AND EURO
            string SqlString = " SELECT SeqNo "
                              + "  ,[RON1] "
                              + "  ,[RON5] "
                              + "  ,[RON10] "
                              + "  ,[RON20] "
                              + "  ,[RON50] "
                              + "  ,[RON100] "
                              + "  ,[RON200] "
                              + "  ,[RON500] "

                              // EUR
                              + "  , eur5 "
                              + "  , eur10 "
                              + "  , eur20 "
                              + "  , eur50"

                              + "  , eur100 "
                              + "  , eur200 "
                              + "  , eur500 "

                              + "  FROM [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary_NEW] "
                              + " WHERE AtmNO = @ATM_NO "
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

            //eur5 = (int)rdr["eur5"];
            //eur10 = (int)rdr["eur10"];
            //eur20 = (int)rdr["eur20"];
            //eur50 = (int)rdr["eur50"];
            int Weur5 = 0;
            int Weur10 = 0;
            int Weur20 = 0;
            int Weur50 = 0;

            int Weur100= 0;
            int Weur200 = 0;
            int Weur500 = 0;

            eur5_TOT = 0;
            eur10_TOT = 0;
            eur20_TOT = 0;
            eur50_TOT = 0;

            eur100_TOT = 0;
            eur200_TOT = 0;
            eur500_TOT = 0;


            ////public int eur100;
            ////public int eur200;
            ////public int eur500;

            //eur100 = (int)rdr["eur100"];
            //eur200 = (int)rdr["eur200"];
            //eur500 = (int)rdr["eur500"];

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
                // EURO
                Weur5 = (int)BalancingTable.Rows[I]["eur5"];
                Weur10 = (int)BalancingTable.Rows[I]["eur10"];
                Weur20 = (int)BalancingTable.Rows[I]["eur20"];
                Weur50 = (int)BalancingTable.Rows[I]["eur50"];

                Weur100= (int)BalancingTable.Rows[I]["eur100"];
                Weur200 = (int)BalancingTable.Rows[I]["eur200"];
                Weur500 = (int)BalancingTable.Rows[I]["eur500"];

                //ADD TO TOTALS
                RON1_NONRecycle_Total = RON1_NONRecycle_Total + WRON1;
                RON5_NONRecycle_Total = RON5_NONRecycle_Total + WRON5;
                RON10_Recycle_Total = RON10_Recycle_Total + WRON10; // recycle 
                RON20_NONRecycle_Total = RON20_NONRecycle_Total + WRON20; 
                RON50_Recycle_Total = RON50_Recycle_Total + WRON50; // Recycle 
                RON100_Recycle_Total = RON100_Recycle_Total + WRON100; // Recycle
                RON200_NONRecycle_Total = RON200_NONRecycle_Total + WRON200;
                RON500_NONRecycle_Total = RON500_NONRecycle_Total + WRON500;
                // ADD TO TOTALS EU

                eur5_TOT = eur5_TOT + Weur5;
                eur10_TOT = eur10_TOT + Weur10;
                eur20_TOT = eur20_TOT + Weur20;
                eur50_TOT = eur50_TOT + Weur50;

                eur100_TOT = eur100_TOT + Weur100;
                eur200_TOT = eur200_TOT + Weur200;
                eur500_TOT = eur500_TOT + Weur500;

                Update_TOTALS_RECYCLE_AND_NOT(WSeqNo); 

                I = I + 1;
            }




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
                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary_NEW] "
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
        
        //Find Out if UTRNNO exist in Journals 
        public void ReadAndFind_UTRNNO(string InAtmNo, string InUTRNNO)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " SELECT AtmNo, UTRNNO "
                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] "
                  + " WHERE AtmNo = @AtmNo AND UTRNNO= @UTRNNO  ";


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ATMNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@UTRNNO", InUTRNNO);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            AtmNo = (string)rdr["AtmNo"];
                            UTRNNO = (string)rdr["UTRNNO"];
                            
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
                        new SqlCommand(" UPDATE [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary_NEW]" 
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
        
        + "  ,[PartialRepl] = 0 "


            //PartialRepl


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
                        new SqlCommand(" UPDATE [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary_NEW]"
                        + " SET "
                        + " [RON1] = 0 "
                        + " ,[RON5] =  0"
                         + " ,[RON10] =  0"
                          + " ,[RON20] =  0"
                           + " ,[RON50] =  0"
                            + " ,[RON100] =  0"
                             + " ,[RON200] =  0"
                              + " ,[RON500] =  0"

                     
                        + " ,[RON1_Recycle_Total] = 0 "
                        + " ,[RON5_Recycle_Total] = 0 "
                        + " ,[RON10_Recycle_Total] = 0 "
                        + " ,[RON20_Recycle_Total] = 0 "
                        + " ,[RON50_Recycle_Total] = 0 "
                        + " ,[RON100_Recycle_Total] = 0 "
                        + " ,[RON200_Recycle_Total] = 0 "
                        + " ,[RON500_Recycle_Total] = 0 "

                          + ", [RON1_NONRecycle_Total] = 0 "
                            + ", [RON5_NONRecycle_Total] = 0 "
                              + ", [RON10_NONRecycle_Total] = 0 "
                                + ", [RON20_NONRecycle_Total] = 0 "
                                  + ", [RON50_NONRecycle_Total] = 0 "
                                    + ", [RON100_NONRecycle_Total] = 0 "
                                      + ", [RON200_NONRecycle_Total] = 0 "
                                        + ", [RON500_NONRecycle_Total] = 0 "

                         + " ,[CLOSE_DENOM_TYP1] = 10 "
                         + " ,[CLOSE_DENOM_TYP2] = 50 "
                         + " ,[CLOSE_DENOM_TYP3] = 100 "
                         + " ,[CLOSE_DENOM_TYP4] = 0 "
                         
        + "  ,[PartialRepl] = 0 "

            + " WHERE TranDesc = 'DEPOSIT'  AND CCy = 'EUR' AND [LoadedAtRMCycle]  = @LoadedAtRMCycle  ", conn))
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
                        new SqlCommand(" UPDATE [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary_NEW] "
                        + " SET "
                        + " [JOURNAL_REMAINING_TYPE4] = 0  "
                          + " ,[PartialRepl] = 0  "
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

            // Temporary Fix for Euro 50 

            using (SqlConnection conn =
               new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE  [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] "
                        + " SET [eur50] = 1  "
                        + " WHERE UTRNNO = '250403224364740661' and eur50 = 0  ", conn))
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
            //
            // Temporary 
            // Update eu50 
            //
            using (SqlConnection conn =
            new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary_NEW] "
                        + " SET [eur50] = 1  "
                        + " WHERE UTRNNO = '250403224364740661' and eur50 = 0  ", conn))
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

        }


        // 
        // UPDATE  SWITCH 
        //
        public void Update_RESPONSE_ValuesInSWITCH(string InUTRNNO)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [RRDM_Reconciliation_ITMX].[dbo].[BULK_Switch_IST_Txns] "
                        + " SET "
                        + "  [RESPONSE_CODE] = '99' "
                        + "  , [RESPONSE_DESCRIPTION] = 'Change by RRDM Looks to be not valid txn'  "
                        + " WHERE UTRNNO = @UTRNNO  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UTRNNO", InUTRNNO);

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
                        new SqlCommand(" UPDATE [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary_NEW] "
                        + " SET "
                        + " [RON1_NONRecycle_Total] = @RON1_NONRecycle_Total "
                         + " , [RON5_NONRecycle_Total] = @RON5_NONRecycle_Total "
                         + " , [RON10_Recycle_Total] = @RON10_Recycle_Total "
                         + " , [RON20_NONRecycle_Total] = @RON20_NONRecycle_Total "

                         + " , [RON50_Recycle_Total] = @RON50_Recycle_Total "
                         + " , [RON100_Recycle_Total] = @RON100_Recycle_Total "
                         + " , [RON200_NONRecycle_Total] = @RON200_NONRecycle_Total "
                         + " , [RON500_NONRecycle_Total] = @RON500_NONRecycle_Total "

                             + " , [eur5_TOT] = @eur5_TOT "
                             + " , [eur10_TOT] = @eur10_TOT "
                             + " , [eur20_TOT] = @eur20_TOT "
                             + " , [eur50_TOT] = @eur50_TOT "

                             + " , [eur100_TOT] = @eur100_TOT "
                             + " , [eur200_TOT] = @eur200_TOT "
                             + " , [eur500_TOT] = @eur500_TOT "

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

                        cmd.Parameters.AddWithValue("@eur5_TOT", eur5_TOT);
                        cmd.Parameters.AddWithValue("@eur10_TOT", eur10_TOT);
                        cmd.Parameters.AddWithValue("@eur20_TOT", eur20_TOT);
                        cmd.Parameters.AddWithValue("@eur50_TOT", eur50_TOT);

                        cmd.Parameters.AddWithValue("@eur100_TOT", eur100_TOT);
                        cmd.Parameters.AddWithValue("@eur200_TOT", eur200_TOT);
                        cmd.Parameters.AddWithValue("@eur500_TOT", eur500_TOT);


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



using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDM_CIT_EXCEL_TO_BANK : Logger
    {
        public RRDM_CIT_EXCEL_TO_BANK() : base() { }

        public int SeqNo;

        public string OriginTextFile;
        public DateTime ExcelDate;
        public DateTime LoadedDtTm;
        public int BulkRecordId;
        public string CIT_ID;
        public string AtmNo;

        public bool Valid_Entry;
        public string GROUP_ATMS;
        public DateTime ReplCycleStartDate;
        public DateTime ReplCycleEndDate;
        public decimal CIT_Total_Replenished;

        public decimal CIT_Total_Returned;
        public decimal CIT_Total_Deposit_Local_Ccy;
        public decimal SWITCH_Total_Returned;
        public decimal SWITCH_Total_Deposit_Local_Ccy;
        public decimal JNL_SM_Total_Replenished_At_Cycle_start;
        public decimal JNL_SM_Total_Returned;
        public decimal JNL_SM_Deposit_Local_Ccy;

        public DateTime CreatedDate;

        public decimal OverFound_Amt_Cassettes;
        public decimal ShortFound_Amt_Cassettes;
        public decimal OverFound_Amt_Deposits;
        public decimal ShortFound_Amt_Deposits;
        public decimal PresentedErrors;

        public string STATUS;
        public int LoadedAtRMCycle;
        public int ProcessedAtReplCycleNo;
        public string Mask;
        public decimal Gl_Balance_At_CutOff;
        public bool Journal;
        public bool JournalForced;
        public bool IsAuto;
        
        public bool Processed;
        public DateTime SesDtTimeStart;
        public DateTime SesDtTimeEnd;
        public int ProcessMode;
        public int ReplCycle;
        public int GroupOfAtmsRRDM;
        public string UserId;
        public string CIT_ID_RRDM; 
        public DateTime ReplCompletionDt;

        /// <summary>
        /// till here is the definition of CIT TO BANK EXCEL
        /// ******************
        /// </summary>

        public string CitId;
        public string Description;

        public string OrdersFunction;
        // It has two Values
        // "ATMsinNeed" and
        // "PrepareMoneyIn"

        public DateTime CreatedDateTm;

        public string ExcelIdAndLocation;
        public int ExcelRecords;
        public decimal ExcelAmount;

        public bool SendByEmail;
        public DateTime SendDateTm;

        public string MakerId;
        public DateTime AuthorisedDateTm;
        public string AuthoriserId;

        public int ProcessStage; // 1 at Makers
                                 // 2 at Authoriser
                                 // 3 Completed 
        public string Operator;


        // Define the data table 
        public DataTable TableExcelOutputCycles = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public DataTable DataTableAllFields = new DataTable();

        public DataTable DataTableSwitchTimers = new DataTable();

        public DataTable DataTableATMFields = new DataTable();

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Read Table Fields
        private void ReadTableFields_CIT_EXCEL(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            OriginTextFile = (string)rdr["OriginTextFile"];
            ExcelDate = (DateTime)rdr["ExcelDate"];
            LoadedDtTm = (DateTime)rdr["LoadedDtTm"];
            BulkRecordId = (int)rdr["BulkRecordId"];
            CIT_ID = (string)rdr["CIT_ID"];
            AtmNo = (string)rdr["AtmNo"];



            Valid_Entry = (bool)rdr["Valid_Entry"];
            GROUP_ATMS = (string)rdr["GROUP_ATMS"];
            ReplCycleStartDate = (DateTime)rdr["ReplCycleStartDate"];
            ReplCycleEndDate = (DateTime)rdr["ReplCycleEndDate"];
            CIT_Total_Replenished = (decimal)rdr["CIT_Total_Replenished"];



            CIT_Total_Returned = (decimal)rdr["CIT_Total_Returned"];
            CIT_Total_Deposit_Local_Ccy = (decimal)rdr["CIT_Total_Deposit_Local_Ccy"];
            SWITCH_Total_Returned = (decimal)rdr["SWITCH_Total_Returned"];
            SWITCH_Total_Deposit_Local_Ccy = (decimal)rdr["SWITCH_Total_Deposit_Local_Ccy"];
            JNL_SM_Total_Replenished_At_Cycle_start = (decimal)rdr["JNL_SM_Total_Replenished_At_Cycle_start"];
            JNL_SM_Total_Returned = (decimal)rdr["JNL_SM_Total_Returned"];
            JNL_SM_Deposit_Local_Ccy = (decimal)rdr["JNL_SM_Deposit_Local_Ccy"];



            CreatedDate = (DateTime)rdr["CreatedDate"];
            OverFound_Amt_Cassettes = (decimal)rdr["OverFound_Amt_Cassettes"];
            ShortFound_Amt_Cassettes = (decimal)rdr["ShortFound_Amt_Cassettes"];
            OverFound_Amt_Deposits = (decimal)rdr["OverFound_Amt_Deposits"];
            ShortFound_Amt_Deposits = (decimal)rdr["ShortFound_Amt_Deposits"];
            PresentedErrors = (decimal)rdr["PresentedErrors"];

            //  ProcessedMode

            STATUS = (string)rdr["STATUS"];
            // 01, Action To be taken 
            // 02, Under Authorisation 
            // 03, Created Through Maker / Checker
            // 04, Auto Created ... You can do undo on it and turn it to 01  
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
            ProcessedAtReplCycleNo = (int)rdr["ProcessedAtReplCycleNo"];
            Mask = (string)rdr["Mask"];
            Gl_Balance_At_CutOff = (decimal)rdr["Gl_Balance_At_CutOff"];
            Journal = (bool)rdr["Journal"];
            JournalForced = (bool)rdr["JournalForced"];
            IsAuto = (bool)rdr["IsAuto"];

            //   
            Processed = (bool)rdr["Processed"];
            SesDtTimeStart = (DateTime)rdr["SesDtTimeStart"];
            SesDtTimeEnd = (DateTime)rdr["SesDtTimeEnd"];
            ProcessMode = (int)rdr["ProcessMode"];
            ReplCycle = (int)rdr["ReplCycle"];
            GroupOfAtmsRRDM = (int)rdr["GroupOfAtmsRRDM"];
            UserId = (string)rdr["UserId"];
            CIT_ID_RRDM = (string)rdr["CIT_ID_RRDM"];
           
            ReplCompletionDt = (DateTime)rdr["ReplCompletionDt"];

        }

        // Read Table Fields
        private void ReadTableFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CitId = (string)rdr["CitId"];
            Description = (string)rdr["Description"];

            OrdersFunction = (string)rdr["OrdersFunction"];

            CreatedDateTm = (DateTime)rdr["CreatedDateTm"];

            ExcelIdAndLocation = (string)rdr["ExcelIdAndLocation"];
            ExcelRecords = (int)rdr["ExcelRecords"];
            ExcelAmount = (decimal)rdr["ExcelAmount"];

            SendByEmail = (bool)rdr["SendByEmail"];
            SendDateTm = (DateTime)rdr["SendDateTm"];

            MakerId = (string)rdr["MakerId"];
            AuthorisedDateTm = (DateTime)rdr["AuthorisedDateTm"];
            AuthoriserId = (string)rdr["AuthoriserId"];

            ProcessStage = (int)rdr["ProcessStage"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ  
        // 
        //
        public void Read_CIT_Excel_Table_BySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
                      + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields_CIT_EXCEL(rdr);
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

        // UPDATE FOREX TABLE IN SM
        // 1
        int _USD_1_;
        int _USD_5_;
        int _USD_10_;
        int _USD_20_;
        int _USD_50_;
        int _USD_100_;
        decimal total_Dep_USD;

        // 2
        int _EUR_5_;
        int _EUR_10_;
        int _EUR_20_;
        int _EUR_50_;
        int _EUR_100_;
        int _EUR_200_;
        decimal EUR_Total_Deposit;

        // 3
        int _GBP_5_;
        int _GBP_10_;
        int _GBP_20_;
        int _GBP_50_;
        decimal GBP_Total_Deposit_;

        // 4 
        int _KWD_1_;
        int _KWD_5_;
        int _KWD_10_;
        int _KWD_20_;
        decimal KWD_Total_Deposit;

        // 5

        int AED_10;
        int AED_20;
        int AED_50;
        int AED_100;
        int AED_200;
        int AED_500;
        int AED_1000;
        decimal AED_Total_Deposit;

        // 6
        int _SAR_1_;
        int _SAR_5_;
        int _SAR_10_;
        int _SAR_20_;
        int _SAR_50_;
        int _SAR_100_;
        int _SAR_200;
        int _SAR_500_;
        decimal SAR_Total_Deposit;



        public void UPDATE_FOREX_IN_SM(string InAtmNo, int InReplCycle, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT "
                 + "  Cast(_USD_1_ As Int) as _USD_1_"
                 + " , Cast(_USD_5_ As Int) as _USD_5_ "
                    + " ,Cast(_USD_10_ As Int) as _USD_10_ "
                       + " ,Cast(_USD_20_ As Int) as _USD_20_ "
                          + " ,Cast(_USD_50_ As Int) as _USD_50_ "
                             + " ,Cast(_USD_100_ As Int) as _USD_100_ "
                             + " ,Cast([Total_Deposit_USD] as Decimal(18,2) ) AS total_Dep_USD "

                 + " , Cast(_EUR_5_ As Int) as _EUR_5_ "
                    + " ,Cast(_EUR_10_ As Int) as _EUR_10_ "
                       + " ,Cast(_EUR_20_ As Int) as _EUR_20_ "
                          + " ,Cast(_EUR_50_ As Int) as _EUR_50_ "
                             + " ,Cast(_EUR_100_ As Int) as _EUR_100_ "
                               + " ,Cast(_EUR_200_ As Int) as _EUR_200_ "
                             + " ,Cast([EUR_Total_Deposit] as Decimal(18,2) ) AS EUR_Total_Deposit "

            //                   // 3
            //int _GBP_5_;
            //int _GBP_10_;
            //int _GBP_20_;
            //int _GBP_50_;
            //decimal GBP_Total_Deposit_;
              + " , Cast(_GBP_5_ As Int) as _GBP_5_ "
                    + " ,Cast(_GBP_10_ As Int) as _GBP_10_ "
                       + " ,Cast(_GBP_20_ As Int) as _GBP_20_ "
                          + " ,Cast(_GBP_50_ As Int) as _GBP_50_ "
                           + " ,Cast([GBP_Total_Deposit_] as Decimal(18,2) ) AS GBP_Total_Deposit_ "

            //  // 4 
            //int _KWD_1_;
            //int _KWD_5_;
            //int _KWD_10_;
            //int _KWD_20_;
            //decimal KWD_Total_Deposit;
              + " , Cast(_KWD_1_ As Int) as _KWD_1_ "
                    + " ,Cast(_KWD_5_ As Int) as _KWD_5_ "
                       + " ,Cast(_KWD_10_ As Int) as _KWD_10_ "
                          + " ,Cast(_KWD_20_ As Int) as _KWD_20_ "
                           + " ,Cast([KWD_Total_Deposit] as Decimal(18,2) ) AS KWD_Total_Deposit "

            //   // 5

            //int AED_10;
            //int AED_20;
            //int AED_50;
            //int AED_100;
            //int AED_200;
            //int AED_500;
            //int AED_1000;
            //decimal AED_Total_Deposit;

             + " , Cast(AED_10 As Int) as AED_10 "
                    + " ,Cast(AED_20 As Int) as AED_20 "
                       + " ,Cast(AED_50 As Int) as AED_50 "
                          + " ,Cast(AED_100 As Int) as AED_100 "
                           + " ,Cast(AED_200 As Int) as AED_200 "
                       + " ,Cast(AED_500 As Int) as AED_500 "
                          + " ,Cast(AED_1000 As Int) as AED_1000 "

                            + " ,Cast([AED_Total_Deposit] as Decimal(18,2) ) AS AED_Total_Deposit "

               //// 6
               //           int _SAR_1_;
               //           int _SAR_5_;
               //           int _SAR_10_;
               //           int _SAR_20_;
               //           int _SAR_50_;
               //           int _SAR_100_;
               //           int _SAR_200;
               //           int _SAR_500_;
               //           decimal SAR_Total_Deposit;

               + " , Cast(_SAR_1_ As Int) as _SAR_1_ "
                    + " ,Cast(_SAR_5_ As Int) as _SAR_5_ "
                     + " ,Cast(_SAR_10_ As Int) as _SAR_10_ "
                      + " ,Cast(_SAR_20_ As Int) as _SAR_20_ "
                       + " ,Cast(_SAR_50_ As Int) as _SAR_50_ "
                        + " ,Cast(_SAR_100_ As Int) as _SAR_100_ "
                         + " ,Cast(_SAR_200 As Int) as _SAR_200 "
                          + " ,Cast(_SAR_500_ As Int) as _SAR_500_ "

             + " ,Cast([SAR_Total_Deposit] as Decimal(18,2) ) AS SAR_Total_Deposit "

            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[BULK_CIT_EXCEL_TO_BANK_ALL] "
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

                            //AtmNo = (string)rdr["AtmNo"];

                            _USD_1_ = (int)rdr["_USD_1_"];
                            _USD_5_ = (int)rdr["_USD_5_"];
                            _USD_10_ = (int)rdr["_USD_10_"];
                            _USD_20_ = (int)rdr["_USD_20_"];
                            _USD_50_ = (int)rdr["_USD_50_"];
                            _USD_100_ = (int)rdr["_USD_100_"];
                            int totalNotes = _USD_1_ + _USD_5_ + _USD_10_ + _USD_20_ + _USD_50_ + _USD_10_;
                            total_Dep_USD = (decimal)rdr["total_Dep_USD"];

                            if (totalNotes > 0)
                            {
                                SM.Read_SM_Record_SpecificDepositsCounted(InAtmNo, InReplCycle, "USD");
                                if (SM.RecordFound == true)
                                {
                                    // OK 
                                }
                                else
                                {
                                    // Insert empty Record 
                                    SM.InsertCountedRecord(InAtmNo, InReplCycle, "USD");
                                }

                                // INITIALISE 
                                SM.TotalCassetteNotesCount = totalNotes;
                                SM.TotalCassetteAmountCount = total_Dep_USD;

                                SM.TotalRetractedNotesCount = 0;
                                SM.TotalRetractedAmountCount = 0;

                                SM.TotalRecycledNotesCount = 0;
                                SM.TotalRecycledAmountCount = 0;

                                SM.Update_SM_Deposit_analysis_Counted(InAtmNo, InReplCycle, "USD");
                            }

                            //// 2
                            //int _EUR_5_;
                            //int _EUR_10_;
                            //int _EUR_20_;
                            //int _EUR_50_;
                            //int _EUR_100_;
                            //int _EUR_200_;
                            //decimal EUR_Total_Deposit;

                            _EUR_5_ = (int)rdr["_EUR_5_"];
                            _EUR_10_ = (int)rdr["_EUR_10_"];
                            _EUR_20_ = (int)rdr["_EUR_20_"];
                            _EUR_50_ = (int)rdr["_EUR_50_"];
                            _EUR_100_ = (int)rdr["_EUR_100_"];
                            _EUR_200_ = (int)rdr["_EUR_200_"];
                            totalNotes = _EUR_5_ + _EUR_10_ + _EUR_20_ + _EUR_50_ + _EUR_100_ + _EUR_200_;
                            EUR_Total_Deposit = (decimal)rdr["EUR_Total_Deposit"];

                            if (totalNotes > 0)
                            {
                                SM.Read_SM_Record_SpecificDepositsCounted(InAtmNo, InReplCycle, "EUR");
                                if (SM.RecordFound == true)
                                {
                                    // OK 
                                }
                                else
                                {
                                    // Insert empty Record 
                                    SM.InsertCountedRecord(InAtmNo, InReplCycle, "EUR");
                                }

                                // INITIALISE 
                                SM.TotalCassetteNotesCount = totalNotes;
                                SM.TotalCassetteAmountCount = EUR_Total_Deposit;

                                SM.TotalRetractedNotesCount = 0;
                                SM.TotalRetractedAmountCount = 0;

                                SM.TotalRecycledNotesCount = 0;
                                SM.TotalRecycledAmountCount = 0;

                                SM.Update_SM_Deposit_analysis_Counted(InAtmNo, InReplCycle, "EUR");

                            }


                            //                   // 3
                            //int _GBP_5_;
                            //int _GBP_10_;
                            //int _GBP_20_;
                            //int _GBP_50_;
                            //decimal GBP_Total_Deposit_;

                            _GBP_5_ = (int)rdr["_GBP_5_"];
                            _GBP_10_ = (int)rdr["_GBP_10_"];
                            _GBP_20_ = (int)rdr["_GBP_20_"];
                            _GBP_50_ = (int)rdr["_GBP_50_"];

                            totalNotes = _GBP_5_ + _GBP_10_ + _GBP_20_ + _GBP_50_;
                            GBP_Total_Deposit_ = (decimal)rdr["GBP_Total_Deposit_"];

                            if (totalNotes > 0)
                            {
                                SM.Read_SM_Record_SpecificDepositsCounted(InAtmNo, InReplCycle, "GBP");
                                if (SM.RecordFound == true)
                                {
                                    // OK 
                                }
                                else
                                {
                                    // Insert empty Record 
                                    SM.InsertCountedRecord(InAtmNo, InReplCycle, "GBP");
                                }

                                // INITIALISE 
                                SM.TotalCassetteNotesCount = totalNotes;
                                SM.TotalCassetteAmountCount = GBP_Total_Deposit_;

                                SM.TotalRetractedNotesCount = 0;
                                SM.TotalRetractedAmountCount = 0;

                                SM.TotalRecycledNotesCount = 0;
                                SM.TotalRecycledAmountCount = 0;

                                SM.Update_SM_Deposit_analysis_Counted(InAtmNo, InReplCycle, "GBP");

                            }

                            //  // 4 
                            //int _KWD_1_;
                            //int _KWD_5_;
                            //int _KWD_10_;
                            //int _KWD_20_;
                            //decimal KWD_Total_Deposit;

                            _KWD_1_ = (int)rdr["_KWD_1_"];
                            _KWD_5_ = (int)rdr["_KWD_5_"];
                            _KWD_10_ = (int)rdr["_KWD_10_"];
                            _KWD_20_ = (int)rdr["_KWD_20_"];

                            totalNotes = _KWD_1_ + _KWD_5_ + _KWD_10_ + _KWD_20_;
                            KWD_Total_Deposit = (decimal)rdr["KWD_Total_Deposit"];


                            if (totalNotes > 0)
                            {
                                SM.Read_SM_Record_SpecificDepositsCounted(InAtmNo, InReplCycle, "KWD");
                                if (SM.RecordFound == true)
                                {
                                    // OK 
                                }
                                else
                                {
                                    // Insert empty Record 
                                    SM.InsertCountedRecord(InAtmNo, InReplCycle, "KWD");
                                }

                                // INITIALISE 
                                SM.TotalCassetteNotesCount = totalNotes;
                                SM.TotalCassetteAmountCount = KWD_Total_Deposit;

                                SM.TotalRetractedNotesCount = 0;
                                SM.TotalRetractedAmountCount = 0;

                                SM.TotalRecycledNotesCount = 0;
                                SM.TotalRecycledAmountCount = 0;

                                SM.Update_SM_Deposit_analysis_Counted(InAtmNo, InReplCycle, "KWD");

                            }


                            //   // 5

                            //int AED_10;
                            //int AED_20;
                            //int AED_50;
                            //int AED_100;
                            //int AED_200;
                            //int AED_500;
                            //int AED_1000;
                            //decimal AED_Total_Deposit;

                            AED_10 = (int)rdr["AED_10"];
                            AED_20 = (int)rdr["AED_20"];
                            AED_50 = (int)rdr["AED_50"];
                            AED_100 = (int)rdr["AED_100"];
                            AED_200 = (int)rdr["AED_200"];
                            AED_500 = (int)rdr["AED_500"];
                            AED_1000 = (int)rdr["AED_1000"];

                            totalNotes = AED_10 + AED_20 + AED_50 + AED_100 + AED_200 + AED_500 + AED_1000;
                            AED_Total_Deposit = (decimal)rdr["AED_Total_Deposit"];


                            if (totalNotes > 0)
                            {
                                SM.Read_SM_Record_SpecificDepositsCounted(InAtmNo, InReplCycle, "AED");
                                if (SM.RecordFound == true)
                                {
                                    // OK 
                                }
                                else
                                {
                                    // Insert empty Record 
                                    SM.InsertCountedRecord(InAtmNo, InReplCycle, "AED");
                                }

                                // INITIALISE 
                                SM.TotalCassetteNotesCount = totalNotes;
                                SM.TotalCassetteAmountCount = AED_Total_Deposit;

                                SM.TotalRetractedNotesCount = 0;
                                SM.TotalRetractedAmountCount = 0;

                                SM.TotalRecycledNotesCount = 0;
                                SM.TotalRecycledAmountCount = 0;

                                SM.Update_SM_Deposit_analysis_Counted(InAtmNo, InReplCycle, "AED");

                            }



                            //// 6
                            //           int _SAR_1_;
                            //           int _SAR_5_;
                            //           int _SAR_10_;
                            //           int _SAR_20_;
                            //           int _SAR_50_;
                            //           int _SAR_100_;
                            //           int _SAR_200;
                            //           int _SAR_500_;
                            //           decimal SAR_Total_Deposit;

                            _SAR_1_ = (int)rdr["_SAR_1_"];
                            _SAR_5_ = (int)rdr["_SAR_5_"];
                            _SAR_10_ = (int)rdr["_SAR_10_"];
                            _SAR_20_ = (int)rdr["_SAR_20_"];
                            _SAR_50_ = (int)rdr["_SAR_50_"];
                            _SAR_100_ = (int)rdr["_SAR_100_"];
                            _SAR_200 = (int)rdr["_SAR_200"];
                            _SAR_500_ = (int)rdr["_SAR_500_"];

                            totalNotes = _SAR_1_ + _SAR_5_ + _SAR_10_ + _SAR_20_ + _SAR_50_ + _SAR_100_ + _SAR_200 + _SAR_500_;
                            SAR_Total_Deposit = (decimal)rdr["SAR_Total_Deposit"];


                            if (totalNotes > 0)
                            {
                                SM.Read_SM_Record_SpecificDepositsCounted(InAtmNo, InReplCycle, "SAR");
                                if (SM.RecordFound == true)
                                {
                                    // OK 
                                }
                                else
                                {
                                    // Insert empty Record 
                                    SM.InsertCountedRecord(InAtmNo, InReplCycle, "SAR");
                                }

                                // INITIALISE 
                                SM.TotalCassetteNotesCount = totalNotes;
                                SM.TotalCassetteAmountCount = SAR_Total_Deposit;

                                SM.TotalRetractedNotesCount = 0;
                                SM.TotalRetractedAmountCount = 0;

                                SM.TotalRecycledNotesCount = 0;
                                SM.TotalRecycledAmountCount = 0;

                                SM.Update_SM_Deposit_analysis_Counted(InAtmNo, InReplCycle, "SAR");

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


        public bool Check_IF_FOREX_IN_BULK(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool HasForex = false;

            decimal GrandTotal = 0;

            SqlString = "SELECT "
                             //+ "  Cast(_USD_1_ As Int) as _USD_1_"
                             //+ " , Cast(_USD_5_ As Int) as _USD_5_ "
                             //   + " ,Cast(_USD_10_ As Int) as _USD_10_ "
                             //      + " ,Cast(_USD_20_ As Int) as _USD_20_ "
                             //         + " ,Cast(_USD_50_ As Int) as _USD_50_ "
                             //            + " ,Cast(_USD_100_ As Int) as _USD_100_ "
                             + " Cast([Total_Deposit_USD] as Decimal(18,2) ) AS total_Dep_USD "



                             //+ " , Cast(_EUR_5_ As Int) as _EUR_5_ "
                             //   + " ,Cast(_EUR_10_ As Int) as _EUR_10_ "
                             //      + " ,Cast(_EUR_20_ As Int) as _EUR_20_ "
                             //         + " ,Cast(_EUR_50_ As Int) as _EUR_50_ "
                             //            + " ,Cast(_EUR_100_ As Int) as _EUR_100_ "
                             //              + " ,Cast(_EUR_200_ As Int) as _EUR_200_ "
                             + " ,Cast([EUR_Total_Deposit] as Decimal(18,2) ) AS EUR_Total_Deposit "

                           //                   // 3

                           //+ " , Cast(_GBP_5_ As Int) as _GBP_5_ "
                           //      + " ,Cast(_GBP_10_ As Int) as _GBP_10_ "
                           //         + " ,Cast(_GBP_20_ As Int) as _GBP_20_ "
                           //            + " ,Cast(_GBP_50_ As Int) as _GBP_50_ "
                           + " ,Cast([GBP_Total_Deposit_] as Decimal(18,2) ) AS GBP_Total_Deposit_ "

                           //  // 4 

                           //+ " , Cast(_KWD_1_ As Int) as _KWD_1_ "
                           //      + " ,Cast(_KWD_5_ As Int) as _KWD_5_ "
                           //         + " ,Cast(_KWD_10_ As Int) as _KWD_10_ "
                           //            + " ,Cast(_KWD_20_ As Int) as _KWD_20_ "
                           + " ,Cast([KWD_Total_Deposit] as Decimal(18,2) ) AS KWD_Total_Deposit "

                            //   // 5



                            //+ " , Cast(AED_10 As Int) as AED_10 "
                            //       + " ,Cast(AED_20 As Int) as AED_20 "
                            //          + " ,Cast(AED_50 As Int) as AED_50 "
                            //             + " ,Cast(AED_100 As Int) as AED_100 "
                            //              + " ,Cast(AED_200 As Int) as AED_200 "
                            //          + " ,Cast(AED_500 As Int) as AED_500 "
                            //             + " ,Cast(AED_1000 As Int) as AED_1000 "

                            + " ,Cast([AED_Total_Deposit] as Decimal(18,2) ) AS AED_Total_Deposit "

             //// 6


             //+ " , Cast(_SAR_1_ As Int) as _SAR_1_ "
             //     + " ,Cast(_SAR_5_ As Int) as _SAR_5_ "
             //      + " ,Cast(_SAR_10_ As Int) as _SAR_10_ "
             //       + " ,Cast(_SAR_20_ As Int) as _SAR_20_ "
             //        + " ,Cast(_SAR_50_ As Int) as _SAR_50_ "
             //         + " ,Cast(_SAR_100_ As Int) as _SAR_100_ "
             //          + " ,Cast(_SAR_200 As Int) as _SAR_200 "
             //           + " ,Cast(_SAR_500_ As Int) as _SAR_500_ "

             + " ,Cast([SAR_Total_Deposit] as Decimal(18,2) ) AS SAR_Total_Deposit "

            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[BULK_CIT_EXCEL_TO_BANK_ALL] "
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


                            total_Dep_USD = (decimal)rdr["total_Dep_USD"];

                            GrandTotal = GrandTotal + total_Dep_USD;


                            EUR_Total_Deposit = (decimal)rdr["EUR_Total_Deposit"];

                            GrandTotal = GrandTotal + EUR_Total_Deposit;


                            KWD_Total_Deposit = (decimal)rdr["KWD_Total_Deposit"];

                            GrandTotal = GrandTotal + KWD_Total_Deposit;

                            //   // 5

                            AED_Total_Deposit = (decimal)rdr["AED_Total_Deposit"];

                            GrandTotal = GrandTotal + AED_Total_Deposit;

                            //// 6

                            SAR_Total_Deposit = (decimal)rdr["SAR_Total_Deposit"];

                            GrandTotal = GrandTotal + SAR_Total_Deposit;

                            if (GrandTotal > 0)
                            {
                                HasForex = true;
                            }
                            else
                            {
                                HasForex = false;
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
            return HasForex;
        }


        //
        // Methods 
        // READ  
        // 
        //
        public void Read_CIT_Excel_TableBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
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

                            ReadTableFields_CIT_EXCEL(rdr);
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

        // READ The transactions based on dates for IST say 

        public void ReadRecordsFrom_Excel_DataBySelectionCriteria(string InTableIdA, string InSelectionCriteria)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();
            //WLoadedAtRMCycle = (int)Ce.DataTableAllFields.Rows[I]["LoadedAtRMCycle"];
            //WReplCycle = (int)Ce.DataTableAllFields.Rows[I]["ReplCycle"];

            //WGroupOfAtmsRRDM = (int)Ce.DataTableAllFields.Rows[I]["GroupOfAtmsRRDM"];

            SqlString =
               " SELECT SeqNo,BulkRecordId ,AtmNo, GROUP_ATMS "
                + " ,ReplCycleStartDate, ReplCycleEndDate "
                + " , CIT_Total_Replenished, CIT_Total_Returned, CIT_Total_Deposit_Local_Ccy, Journal "
                + " , LoadedAtRMCycle, ReplCycle, GroupOfAtmsRRDM, STATUS "
               + " FROM " + InTableIdA
               + InSelectionCriteria
               + " ORDER by AtmNo "
                ;


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);


                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            //InsertReport("Controller", DataTableAllFields);
        }


        public void ReadSWITCH_ShortTableToFindTimeSpaces(string InSignedId, string InTable, string InTerminalId,
                                                         DateTime InDateFrom, DateTime InDateTo, int InMode, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int WSeqNo;
            string WTerminalId;
            decimal WTransAmt;
            int WTraceNo;

            DateTime WCurrentTransDate;
            DateTime WPreviousTransDate;

            //  RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            TotalSelected = 0;

            WPreviousTransDate = NullPastDate;

            DataTableSwitchTimers = new DataTable();
            DataTableSwitchTimers.Clear();

            // DATA TABLE ROWS DEFINITION 

            DataTableSwitchTimers.Columns.Add("SeqNo", typeof(int));
            DataTableSwitchTimers.Columns.Add("ATM_NO", typeof(string));
            DataTableSwitchTimers.Columns.Add("TransAmt", typeof(decimal));
            DataTableSwitchTimers.Columns.Add("TraceNo", typeof(int));
            DataTableSwitchTimers.Columns.Add("TransDate", typeof(DateTime));
            DataTableSwitchTimers.Columns.Add("PreTranDate", typeof(DateTime));
            DataTableSwitchTimers.Columns.Add("Diff_Min", typeof(double));

            string SqlString = " SELECT SeqNo "
                      + " , TerminalId "
                      + " , TransAmt "
                      + " , TraceNo "
                      + " , TransDate "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_SWITCH_TXNS] "
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo)  "
                      + " ORDER BY TransDate ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // InTerminalId
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);


                        // Read table 
                        cmd.CommandTimeout = 150;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            WSeqNo = (int)rdr["SeqNo"];

                            WTerminalId = (string)rdr["TerminalId"];
                            WCurrentTransDate = (DateTime)rdr["TransDate"];
                            WTransAmt = (decimal)rdr["TransAmt"];
                            WTraceNo = (int)rdr["TraceNo"];

                            //DateTime WCurrentTransDate;


                            TotalSelected = TotalSelected + 1;


                            // Fill Table 

                            if (TotalSelected > 1)
                            {
                                DataRow RowSelected = DataTableSwitchTimers.NewRow();


                                RowSelected["SeqNo"] = WSeqNo;

                                RowSelected["ATM_NO"] = WTerminalId;
                                RowSelected["TransAmt"] = WTransAmt;
                                RowSelected["TraceNo"] = WTraceNo;
                                RowSelected["TransDate"] = WCurrentTransDate;
                                RowSelected["PreTranDate"] = WPreviousTransDate;

                                TimeSpan difference = WCurrentTransDate - WPreviousTransDate;
                                double minutesDifference = difference.TotalMinutes;
                                int wholeNumber = (int)minutesDifference;

                                RowSelected["Diff_Min"] = wholeNumber;


                                // ADD ROW

                                DataTableSwitchTimers.Rows.Add(RowSelected);
                            }

                            WPreviousTransDate = WCurrentTransDate;

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // Insert printing record and update meta exception 

                    //InsertReport(InOperator, InSignedId, InMode);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }


        public void ReadRecordsFrom_Excel_DataByDatesParameters(string InTableIdA, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();


            SqlString =
               " SELECT SeqNo, AtmNo  "
                + " ,ReplCycleStartDate, ReplCycleEndDate "
                + "  "
               + " FROM " + InTableIdA
               + InSelectionCriteria
               //+ " WHERE Cast(ReplCycleEndDate as Date) = '2025-03-06' OR  Cast(ReplCycleEndDate as Date) = '2025-03-07' "
               + " ORDER by AtmNo "
                ;


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);


                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            //InsertReport("Controller", DataTableAllFields);
        }


        public void ReadRecordsFrom_CIT_Excel_Records_Form276_CIT_Replenish(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();
            //
            SqlString =
               " SELECT SeqNo  "
                 + " , STATUS "
                + " , AtmNo "
                + " , ReplCycleStartDate As CIT_Start "
                + "  , ReplCycleEndDate As CIT_End  "
                + "  , CIT_Total_Returned "
                 + "  , CIT_Total_Deposit_Local_Ccy "
                  + "  , SWITCH_Total_Returned As SWITCH_Returns "
                 + "  , SWITCH_Total_Deposit_Local_Ccy As SWITCH_Deposits "
                + "  , Journal  "
                + "  , CIT_ID  "
                + "  , GROUP_ATMS  "
                + "  , SesDtTimeStart As JLN_Start  "
                 + "  , SesDtTimeEnd As JLN_End  "
                 
                + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
                + InSelectionCriteria
                + " ORDER by AtmNo , ReplCycleEndDate "
                ;


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            //InsertReport("Controller", DataTableAllFields);
        }

        public void UpdateInfoForInvalidATM(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            //
            // UPDATE OTHER INFO FROM UsersAtmTable
            //
            string SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
          + " SET  "
          + " GroupOfAtmsRRDM = t2.GroupOfAtms "
          + ",UserId = t2.UserId "

          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] t1 "
          + " INNER JOIN [ATMS].[dbo].[UsersAtmTable] t2"

          + " ON t1.AtmNo = t2.AtmNo "

          + " WHERE  t1.AtmNo =  @AtmNo AND Valid_Entry = 0 ";//
          //+ " OR (t1.GroupOfAtmsRRDM = 0 AND t1.LoadedAtRMCycle <> @LoadedAtRMCycle )" // Or in Previous 
          // + " OR (t1.UserId = '' AND t1.LoadedAtRMCycle <> @LoadedAtRMCycle )"; // Or in Previous 

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    
                    conn.Close();
                    ErrorFound = true;
                   
                    CatchDetails(ex);
                    return;
                }

            //
            // UPDATE CIT_ID_RRDM FROM ATMS
            //
            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
          + " SET  "
          + " CIT_ID_RRDM = t2.CitId "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] t1 "
          + " INNER JOIN [ATMS].[dbo].[ATMsFields] t2"

          + " ON t1.AtmNo = t2.AtmNo "

         + " WHERE  t1.AtmNo =  @AtmNo AND Valid_Entry = 0 ";//
         //   + " OR (t1.CIT_ID_RRDM = '' AND t1.LoadedAtRMCycle <> @LoadedAtRMCycle )"; // Or in Previous 

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.StatisticsEnabled = false;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;

                    conn.Close();
                    ErrorFound = true;
                    CatchDetails(ex);
                    return;
                }

           
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] " 
                    + " SET [Valid_Entry] = 1 "
                     + " , STATUS = '01' "
                    + "  WHERE GroupOfAtmsRRDM <> 0 AND UserId <> '' "
                      + " AND CIT_ID_RRDM <> '' AND CIT_ID = CIT_ID_RRDM "
                        //+" AND CIT_ID_RRDM <> '1000' "
                        + " AND Valid_Entry = 0 AND AtmNo='"+InAtmNo+"'" // take only the ones with no valid yet 
                   + " ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                    return;
                }
        }


        public void ReadRecordsFrom_CIT_Excel_Records_The_Invalid(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();
            //
            SqlString =
               " SELECT SeqNo  "
                 + " , STATUS "
                + " , AtmNo "
                + "  , CIT_ID  AS CIT_ON_Excel"
                 + "  , CIT_ID_RRDM  "
                + "  , GROUP_ATMS Group_on_Excel "
                + "  , GroupOfAtmsRRDM Group_RRDM"
                + "  , UserId User_Owner "
                // + " , ReplCycleStartDate As CIT_Start "
                //+ "  , ReplCycleEndDate As CIT_End  "

                + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
                + InSelectionCriteria
                + " ORDER by AtmNo , ReplCycleEndDate "
                ;


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            //InsertReport("Controller", DataTableAllFields);
        }


        public void ReadRecordsFrom_CIT_Excel_Records_Replenish_for_ATM(string InAtmNo, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Exclude record having the in SeqNo

            DataTableATMFields = new DataTable();
            DataTableATMFields.Clear();
            //
            SqlString =
               " SELECT SeqNo  "
                 + " , STATUS "
                //+ " , AtmNo "
                + " , ReplCycleStartDate As CIT_Start "
                + "  , ReplCycleEndDate As CIT_End  "
                + "  , Journal  "
                + "  , CIT_ID  "
                + "  , GROUP_ATMS  "
                //+ "  , SesDtTimeStart As JLN_Start  "
                // + "  , SesDtTimeEnd As JLN_End  "
                // + "  , CIT_Total_Returned "
                // + "  , CIT_Total_Deposit_Local_Ccy "
                //  + "  , SWITCH_Total_Returned As SWITCH_Returns "
                // + "  , SWITCH_Total_Deposit_Local_Ccy As SWITCH_Deposits "
                + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
                + " WHERE AtmNo = @AtmNo And SeqNo <> @SeqNo " 
                + " ORDER BY ReplCycleEndDate DESC"
                ;


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@SeqNo", InSeqNo);
                       // sqlAdapt.SelectCommand.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        sqlAdapt.Fill(DataTableATMFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            //InsertReport("Controller", DataTableAllFields);
        }

        //public decimal SWITCH_Total_Returned;
        //public decimal SWITCH_Total_Deposit_Local_Ccy;

        //public decimal JNL_SM_Total_Returned;
        //public decimal JNL_SM_Deposit_Local_Ccy;
        ////
        //public decimal OverFound_Amt_Cassettes;
        //public decimal ShortFound_Amt_Cassettes;

        //public decimal OverFound_Amt_Deposits;
        //public decimal ShortFound_Amt_Deposits;

        //public decimal PresentedErrors; 

        //public DateTime CreatedDate;

        //public string ProcessedMode; // 01 Matching
        // 02 Maker Action
        // 03 Ready for Transaction Creation 

        //public bool Journal; 

        // public DateTime SesDtTimeStart;

        // public DateTime SesDtTimeEnd;

        //public int ProcessMode;

        //public int ReplCycle;

        public void UpdateAtmOfDatesFromJournal(int InSeqNo)
        {
            // Updatind of ATM during Mathcing 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    //              ,[JNL_SM_Total_Returned]
                    //,[JNL_SM_Deposit_Local_Ccy]
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
                           + " SET "
                            + "[Journal]=@Journal ,"
                              + "[JournalForced]=@JournalForced ,"
                            + "[SesDtTimeStart]=@SesDtTimeStart,"
                            + " [SesDtTimeEnd] = @SesDtTimeEnd, "
                              + "[ProcessMode]=@ProcessMode,"
                            + " [ReplCycle] = @ReplCycle "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@Journal", Journal);

                        cmd.Parameters.AddWithValue("@JournalForced", JournalForced);

                        cmd.Parameters.AddWithValue("@SesDtTimeStart", SesDtTimeStart);

                        cmd.Parameters.AddWithValue("@SesDtTimeEnd", SesDtTimeEnd);

                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);
                        cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
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


        //public int ReplCycle;

        public void Update_CIT_DATES(int InSeqNo)
        {
            // Updatind of ATM during Mathcing 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    //              ,[JNL_SM_Total_Returned]
                    //,[JNL_SM_Deposit_Local_Ccy]
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
                           + " SET "
                            + "[ReplCycleStartDate]=@ReplCycleStartDate "
                           + ",[ReplCycleEndDate]=@ReplCycleEndDate "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@ReplCycleStartDate", ReplCycleStartDate);

                        cmd.Parameters.AddWithValue("@ReplCycleEndDate", ReplCycleEndDate);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
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

        public void Update_User_AND_GROUP(int InSeqNo)
        {
            // Updatind of ATM during Mathcing 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    //              ,[JNL_SM_Total_Returned]
                    //,[JNL_SM_Deposit_Local_Ccy]
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
                           + " SET "
                            + "[GroupOfAtmsRRDM]=@GroupOfAtmsRRDM "
                           + ",[UserId]=@UserId "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@GroupOfAtmsRRDM", GroupOfAtmsRRDM);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
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


        public void Update_STATUS_And_ReplCompletion(int InSeqNo)
        {
            // Updatind of ATM during Mathcing 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //Ce.STATUS = "03";
            //Ce.ReplCompletionDt = DateTime.Now;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    //              ,[JNL_SM_Total_Returned]
                    //,[JNL_SM_Deposit_Local_Ccy]
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
                           + " SET "
                            + "[STATUS]=@STATUS "
                           + ",[ReplCompletionDt]=@ReplCompletionDt "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@STATUS", STATUS);

                        cmd.Parameters.AddWithValue("@ReplCompletionDt", ReplCompletionDt);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
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

        public void UpdateAtmDuringMatchingOfCitExcel(int InSeqNo)
        {
            // Updatind of ATM during Mathcing 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    //              ,[JNL_SM_Total_Returned]
                    //,[JNL_SM_Deposit_Local_Ccy]
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
                           + " SET "
                            + "[SWITCH_Total_Returned]=@SWITCH_Total_Returned,"
                            + "[SWITCH_Total_Deposit_Local_Ccy]=@SWITCH_Total_Deposit_Local_Ccy,"
                             + "[JNL_SM_Total_Returned]=@JNL_SM_Total_Returned,"
                            + "[JNL_SM_Deposit_Local_Ccy]=@JNL_SM_Deposit_Local_Ccy,"
                            + "[CreatedDate] =@CreatedDate,"
                            + "[OverFound_Amt_Cassettes] = @OverFound_Amt_Cassettes,"
                            + "[ShortFound_Amt_Cassettes] = @ShortFound_Amt_Cassettes,"
                             + "[OverFound_Amt_Deposits] = @OverFound_Amt_Deposits,"
                            + "[ShortFound_Amt_Deposits] = @ShortFound_Amt_Deposits,"
                            + "[PresentedErrors] = @PresentedErrors,"
                             + "[IsAuto] = @IsAuto,"
                            + " [STATUS] = @STATUS "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@SWITCH_Total_Returned", SWITCH_Total_Returned);

                        cmd.Parameters.AddWithValue("@SWITCH_Total_Deposit_Local_Ccy", SWITCH_Total_Deposit_Local_Ccy);

                        cmd.Parameters.AddWithValue("@JNL_SM_Total_Returned", JNL_SM_Total_Returned);

                        cmd.Parameters.AddWithValue("@JNL_SM_Deposit_Local_Ccy", JNL_SM_Deposit_Local_Ccy);

                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);

                        cmd.Parameters.AddWithValue("@OverFound_Amt_Cassettes", OverFound_Amt_Cassettes);

                        cmd.Parameters.AddWithValue("@ShortFound_Amt_Cassettes", ShortFound_Amt_Cassettes);

                        cmd.Parameters.AddWithValue("@OverFound_Amt_Deposits", OverFound_Amt_Deposits);

                        cmd.Parameters.AddWithValue("@ShortFound_Amt_Deposits", ShortFound_Amt_Deposits);

                        cmd.Parameters.AddWithValue("@PresentedErrors", PresentedErrors);

                        cmd.Parameters.AddWithValue("@IsAuto", IsAuto);

                        cmd.Parameters.AddWithValue("@STATUS", STATUS);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
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



        public void Insert_SWITCH_TXNS_For_ATMS(string InTableIdA, int InRMCycle, int InLastSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Count;

            string SQLCmd = "INSERT INTO " + InTableIdA
                 + "( "

                 + " [OriginalRecordId] " // 1
                 + " ,[TerminalId] " // 2
                 + " ,[TransType] " // 3
                  + " ,[TransCurr] " // 4

                 + " ,[TransAmt] " //5
                 + " ,[TransDate] "  //6
                 + " ,[TraceNo] " //7

                 + ",[LoadedAtRMCycle] " //8

                 + ") "
                 + " SELECT "
                 + "  SeqNo " // 1
                  + " ,TerminalId" // 2
                 + " , TransType" // 3
                  + " , TransCurr " // 4

                 + " , TransAmt " //5
                 + " , TransDate "  //6
                 + " , TraceNo " //7

                + ", @LoadedAtRMCycle" //8

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                 + " WHERE SeqNo >@SeqNo AND LoadedAtRMCycle=" + InRMCycle
                 + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                 + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                 + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' AND TransDescr <> 'FOREX_DEPOSIT'  ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    //conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@OriginTextFile", WFullPath_01.Trim());
                        //cmd.Parameters.AddWithValue("@ExcelDate", FileDATEresult);
                        //cmd.Parameters.AddWithValue("@LoadedDtTm", DateTime.Now);
                        cmd.Parameters.AddWithValue("@SeqNo", InLastSeqNo);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                        //cmd.Parameters.AddWithValue("@ReversedCut_Off_Date", ReversedCut_Off_Date);

                        cmd.CommandTimeout = 350;  // seconds
                        Count = cmd.ExecuteNonQuery();
                        //var stats = conn.RetrieveStatistics();
                        //  commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    //if (Environment.UserInteractive & TEST == true)
                    //{
                    //    System.Windows.Forms.MessageBox.Show("Records Inserted For MEEZA_POS" + Environment.NewLine
                    //             + "..:.." + stpLineCount.ToString());
                    //}
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    //stpErrorText = stpErrorText + "Cancel During CIT EXCEL File Creation insert of records";
                    //stpReturnCode = -1;

                    //stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

        }


        public void Insert_SWITCH_TXNS_For_ATMS_FOREX(string InTableIdA, int InRMCycle, int InLastSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Count;

            string SQLCmd = "INSERT INTO " + InTableIdA
                 + "( "

                 + " [OriginalRecordId] " // 1
                 + " ,[TerminalId] " // 2
                 + " ,[TransType] " // 3
                  + " ,[TransCurr] " // 4

                 + " ,[TransAmt] " //5
                 + " ,[TransDate] "  //6
                 + " ,[TraceNo] " //7

                 + ",[LoadedAtRMCycle] " //8

                 + ") "
                 + " SELECT "
                 + "  SeqNo " // 1
                  + " ,TerminalId" // 2
                 + " , TransType" // 3
                  + " , TransCurr " // 4

                 + " , TransAmt " //5
                 + " , TransDate "  //6
                 + " , TraceNo " //7

                + ", @LoadedAtRMCycle" //8

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                 + " WHERE LoadedAtRMCycle=" + InRMCycle
                 + " AND TransDescr = 'FOREX_WITHDRAWL' "
                  ;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    //conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@OriginTextFile", WFullPath_01.Trim());
                        //cmd.Parameters.AddWithValue("@ExcelDate", FileDATEresult);
                        //cmd.Parameters.AddWithValue("@LoadedDtTm", DateTime.Now);
                        cmd.Parameters.AddWithValue("@SeqNo", InLastSeqNo);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                        //cmd.Parameters.AddWithValue("@ReversedCut_Off_Date", ReversedCut_Off_Date);

                        cmd.CommandTimeout = 350;  // seconds
                        Count = cmd.ExecuteNonQuery();
                        //var stats = conn.RetrieveStatistics();
                        //  commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    //if (Environment.UserInteractive & TEST == true)
                    //{
                    //    System.Windows.Forms.MessageBox.Show("Records Inserted For MEEZA_POS" + Environment.NewLine
                    //             + "..:.." + stpLineCount.ToString());
                    //}
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    //stpErrorText = stpErrorText + "Cancel During CIT EXCEL File Creation insert of records";
                    //stpReturnCode = -1;

                    //stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

        }



        public void Insert_JOURNAL_TXNS_For_CIT(string InTableIdA, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Count;

            string SQLCmd = "INSERT INTO " + InTableIdA // FOR MASTER
                 + "( "
                 + " [OriginalRecordId] " // 1
                 + " ,[TerminalId] " // 2
                 + " ,[TransType] " // 3
                  + " ,[TransCurr] " // 4

                 + " ,[TransAmt] " //5
                 + " ,[TransDate] "  //6
                 + " ,[TraceNo] " //7

                 + ",[LoadedAtRMCycle] " //8

                 + ") "
                 + " SELECT "
                 + "  SeqNo " // 1
                  + " ,TerminalId" // 2
                 + " , TransType" // 3
                  + " , TransCurr " // 4

                 + " , TransAmount " //5
                 + " , TransDate "  //6
                 + " , AtmTraceNo " //7

                + ", @LoadedAtRMCycle" //8

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                 + " WHERE LoadedAtRMCycle=" + InRMCycle
                 + " AND ResponseCode = '0' AND NotInJournal = 0 AND Origin = 'Our Atms' "
                 + " AND (TransType = 11 OR TransType = 23) ";
            //  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' AND TransDescr <> 'FOREX_DEPOSIT'  ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    //conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@OriginTextFile", WFullPath_01.Trim());
                        //cmd.Parameters.AddWithValue("@ExcelDate", FileDATEresult);
                        //cmd.Parameters.AddWithValue("@LoadedDtTm", DateTime.Now);
                        // cmd.Parameters.AddWithValue("@SeqNo", InLastSeqNo);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                        //cmd.Parameters.AddWithValue("@ReversedCut_Off_Date", ReversedCut_Off_Date);

                        cmd.CommandTimeout = 350;  // seconds
                        Count = cmd.ExecuteNonQuery();
                        //var stats = conn.RetrieveStatistics();
                        //  commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    //if (Environment.UserInteractive & TEST == true)
                    //{
                    //    System.Windows.Forms.MessageBox.Show("Records Inserted For MEEZA_POS" + Environment.NewLine
                    //             + "..:.." + stpLineCount.ToString());
                    //}
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    //stpErrorText = stpErrorText + "Cancel During CIT EXCEL File Creation insert of records";
                    //stpReturnCode = -1;

                    //stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

        }



        public void UpdateRecordsAsProcessed(string InTerminalId, DateTime InDateFrom, DateTime InDateTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

           // UPDATE INTERIM SWITCH
            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    //              ,[JNL_SM_Total_Returned]
                    //,[JNL_SM_Deposit_Local_Ccy]
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_SWITCH_TXNS] "
                           + " SET "
                            + "[Processed]= 1 "
                            +" WHERE TerminalId = @TerminalId "
                         + " AND (TransDate BETWEEN @DateFrom AND @DateTo)" , conn))
                    {

                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);

                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
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

            // UPDATE as process INTERIM Journal 
            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    //              ,[JNL_SM_Total_Returned]
                    //,[JNL_SM_Deposit_Local_Ccy]
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_JOURNAL_TXNS] "
                           + " SET "
                            + "[Processed]= 1 "
                            + " WHERE TerminalId = @TerminalId "
                         + " AND (TransDate BETWEEN @DateFrom AND @DateTo)", conn))
                    {

                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);

                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
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




        //        LastSeqNo = 0;


        public decimal GetTotalsFrom_SWITCH_By_Dates(string InTerminalId, DateTime InDateFrom, DateTime InDateTo, int InTransType)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            decimal WTotal = 0;
            //SUM(TransAmt)
            string SQLCmd = "SELECT ISNULL(SUM(TransAmt), 0) AS TotalAmount "
               + "FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_SWITCH_TXNS] "
               + " WHERE TerminalId = @TerminalId AND Processed = 0 "
                 + " AND (TransDate BETWEEN @DateFrom AND @DateTo) AND Transtype=@Transtype "
               + "  "
               + "";

            using (SqlConnection conn =
                             new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);
                        cmd.Parameters.AddWithValue("@Transtype", InTransType);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            WTotal = (decimal)rdr["TotalAmount"];

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
                    //stpErrorText = stpErrorText + "Cancel At _Finding The Maximum";

                    //stpReturnCode = -1;

                    //stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    //return;
                }

            return WTotal;

        }


        public decimal GetTotalsFrom_JOURNAL_By_Dates(string InTerminalId, DateTime InDateFrom, DateTime InDateTo, int InTransType)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            decimal WTotal = 0;
            //SUM(TransAmt)
            string SQLCmd = "SELECT ISNULL(SUM(TransAmt), 0) AS TotalAmount "
               + "FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_JOURNAL_TXNS] "
               + " WHERE TerminalId = @TerminalId  AND Processed = 0 "
                 + " AND (TransDate BETWEEN @DateFrom AND @DateTo) AND Transtype=@Transtype "
               + "  "
               + "";

            using (SqlConnection conn =
                             new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);
                        cmd.Parameters.AddWithValue("@Transtype", InTransType);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            WTotal = (decimal)rdr["TotalAmount"];

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
                    //stpErrorText = stpErrorText + "Cancel At _Finding The Maximum";

                    //stpReturnCode = -1;

                    //stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    //return;
                }

            return WTotal;

        }




        public decimal GetTotalsFrom_Master_For_Presented(string InTerminalId, DateTime InDateFrom, DateTime InDateTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            decimal WTotalPresenter = 0;
            //SUM(TransAmt)
            string SQLCmd = "SELECT ISNULL(SUM(TransAmount), 0) AS TotalPresenter "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
               + " WHERE TerminalId = @TerminalId "
                 + " AND (TransDate BETWEEN @DateFrom AND @DateTo) and MetaExceptionId = 55 "
               + "  "
               + "";

            using (SqlConnection conn =
                             new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);
                        //cmd.Parameters.AddWithValue("@Transtype", InTransType);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            WTotalPresenter = (decimal)rdr["TotalPresenter"];

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
                    //stpErrorText = stpErrorText + "Cancel At _Finding The Maximum";

                    //stpReturnCode = -1;

                    //stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    //return;
                }

            return WTotalPresenter;

        }


        //
        // Methods 
        // READ ExcelOutputCycles
        // FILL UP A TABLE
        //
        public void ReadExcelOutputCyclesFillTable(string InOperator, string InSignedId, string InCitId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableExcelOutputCycles = new DataTable();
            TableExcelOutputCycles.Clear();

            TotalSelected = 0;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[Cit_ExcelOutputCycles] "
                    + " WHERE Operator = @Operator AND CitId = @CitId "
                    + " ORDER BY SeqNo DESC";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CitId", InCitId);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableExcelOutputCycles);

                        // Close conn
                        conn.Close();

                        InsertReport(InOperator, InSignedId, TableExcelOutputCycles);

                    }
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
        public void ReadExcelOutputCyclesBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[Cit_ExcelOutputCycles] "
                      + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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
        // Methods 
        // READ  
        // 
        //
        public void ReadExcelOutputCyclesBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[Cit_ExcelOutputCycles] "
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
        // Methods 
        // READ  
        // 
        //
        public void ReadExcelOutputCyclesByCutOffDate(DateTime InCreatedDateTm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[Cit_ExcelOutputCycles] "
                      + " WHERE CreatedDateTm = @CreatedDateTm ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@CreatedDateTm", InCreatedDateTm);

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
        // UPDATE Cit Out Cycles
        //
        public void Update_Cit_ExcelOutputCycles(string InCitId, int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE Cit_ExcelOutputCycles SET "
                            + "[Description]=@Description,"
                            + "[OrdersFunction]=@OrdersFunction,"
                            + "[CreatedDateTm] =@CreatedDateTm,"
                            + "[ExcelIdAndLocation] = @ExcelIdAndLocation,"
                            + " [ExcelRecords] = @ExcelRecords, "
                            + " [ExcelAmount] = @ExcelAmount, "
                            + " [SendByEmail] = @SendByEmail,"
                            + " [SendDateTm] = @SendDateTm,"
                            + " [MakerId] = @MakerId,"
                            + "[AuthorisedDateTm] = @AuthorisedDateTm,"
                            + " [AuthoriserId] = @AuthoriserId,"
                            + " [ProcessStage] = @ProcessStage "
                            + " WHERE CitId= @CitId AND SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@Description", Description);

                        cmd.Parameters.AddWithValue("@OrdersFunction", OrdersFunction);

                        cmd.Parameters.AddWithValue("@CreatedDateTm", CreatedDateTm);

                        cmd.Parameters.AddWithValue("@ExcelIdAndLocation", ExcelIdAndLocation);

                        cmd.Parameters.AddWithValue("@ExcelRecords", ExcelRecords);

                        cmd.Parameters.AddWithValue("@ExcelAmount", ExcelAmount);

                        cmd.Parameters.AddWithValue("@SendByEmail", SendByEmail);

                        cmd.Parameters.AddWithValue("@SendDateTm", SendDateTm);

                        cmd.Parameters.AddWithValue("@MakerId", MakerId);

                        cmd.Parameters.AddWithValue("@AuthorisedDateTm", AuthorisedDateTm);
                        cmd.Parameters.AddWithValue("@AuthoriserId", AuthoriserId);
                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
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
        // Insert ExcelOutputCycle
        public int InsertExcelOutputCycle()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[Cit_ExcelOutputCycles] "
                + " ( "
                + " [CitId],"
                + " [Description],"
                + " [OrdersFunction],"
                + " [CreatedDateTm],"
                + " [MakerId],"
                + " [ProcessStage],"
                + " [Operator] )"
                + " VALUES"
                + " ( "
                + " @CitId,"
                + " @Description,"
                + " @OrdersFunction,"
                + " @CreatedDateTm,"
                + " @MakerId,"
                + " @ProcessStage,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            //+ " SELECT MsgID  = CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@Description", Description);

                        cmd.Parameters.AddWithValue("@OrdersFunction", OrdersFunction);

                        cmd.Parameters.AddWithValue("@CreatedDateTm", CreatedDateTm);

                        cmd.Parameters.AddWithValue("@MakerId", MakerId);

                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
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

        // UPDATE  MigrationCycle
        // 
        public void UpdateLoadExcelCycle(int InSeqNo)
        {
            //int rows; 
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[Cit_ExcelOutputCycles] SET "
                            + " AuthorisedDateTm = @AuthorisedDateTm "
                            + " , ExcelRecords = @ExcelRecords "
                            + " ,InExcelRecords = @InExcelRecords  "

                            + " ,SendByEmail = @SendByEmail  "
                            + " ,NotInG4S = @NotInG4S  "
                            + " ,PresenterNumberEqual = @PresenterNumberEqual  "
                            + " ,PresenterDiff = @PresenterDiff  "
                            + " ,ShortFound = @ShortFound  "

                            + " ,ExcelAmount = @ExcelAmount  "
                            + " ,ShortAmt = @ShortAmt  "

                            + " ,ExcelIdAndLocation = @ExcelIdAndLocation  "
                            + " ,ProcessStage = @ProcessStage "
                            + " ,IsReversed = @IsReversed  "
                            + " ,MakerId = @MakerId  "
                            + " ,AuthoriserId = @AuthoriserId  "

                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@AuthorisedDateTm", AuthorisedDateTm);

                        cmd.Parameters.AddWithValue("@ExcelRecords", ExcelRecords);



                        cmd.Parameters.AddWithValue("@SendByEmail", SendByEmail);

                        cmd.Parameters.AddWithValue("@ExcelAmount", ExcelAmount);


                        cmd.Parameters.AddWithValue("@ExcelIdAndLocation", ExcelIdAndLocation);

                        cmd.Parameters.AddWithValue("@MakerId", MakerId);
                        cmd.Parameters.AddWithValue("@AuthoriserId", AuthoriserId);

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

        // UPDATE  MigrationCycle by CutOff
        // 
        public void UpdateLoadExcelCycleByCutOff(DateTime InCreatedDateTm)
        {

            //int rows; 
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[Cit_ExcelOutputCycles] SET "
                            + " AuthorisedDateTm = @AuthorisedDateTm "
                            + " , ExcelRecords = @ExcelRecords "
                            + " ,InExcelRecords = @InExcelRecords  "

                            + " ,SendByEmail = @SendByEmail  "
                            + " ,NotInG4S = @NotInG4S  "
                            + " ,PresenterNumberEqual = @PresenterNumberEqual  "
                            + " ,PresenterDiff = @PresenterDiff  "
                            + " ,ShortFound = @ShortFound  "

                            + " ,ExcelAmount = @ExcelAmount  "
                            + " ,ShortAmt = @ShortAmt  "

                            + " ,ExcelIdAndLocation = @ExcelIdAndLocation  "
                            + " ,ProcessStage = @ProcessStage "
                            + " ,IsReversed = @IsReversed  "
                            + " ,MakerId = @MakerId  "
                            + " ,AuthoriserId = @AuthoriserId  "

                            + " WHERE CreatedDateTm = @CreatedDateTm", conn))
                    {
                        cmd.Parameters.AddWithValue("@CreatedDateTm", InCreatedDateTm);

                        cmd.Parameters.AddWithValue("@AuthorisedDateTm", AuthorisedDateTm);

                        cmd.Parameters.AddWithValue("@ExcelRecords", ExcelRecords);


                        cmd.Parameters.AddWithValue("@SendByEmail", SendByEmail);

                        cmd.Parameters.AddWithValue("@ExcelAmount", ExcelAmount);


                        cmd.Parameters.AddWithValue("@ExcelIdAndLocation", ExcelIdAndLocation);


                        cmd.Parameters.AddWithValue("@MakerId", MakerId);
                        cmd.Parameters.AddWithValue("@AuthoriserId", AuthoriserId);

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



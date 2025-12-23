using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMRepl_SupervisorMode_Details : Logger
    {
        public RRDMRepl_SupervisorMode_Details() : base() { }
        // Variables
        public int SeqNo;

        public string AtmNo;

        public string FlagValid;
        public string AdditionalCash;
        
        public string BANK;
        public int Fuid;

        public string txtline;
        public int RUID;
        public int ruid_max_cashAdded;
        public int seqnum;
        public int sessionstart_ruid;

        public int sessionend_ruid;
        public DateTime SM_dateTime_Start;
        public DateTime SM_dateTime_Finish;
        public DateTime SM_LAST_CLEARED;
        
        public string previous_Repl_trace;
        public int previous_Repl_trace_ruid;
        public DateTime previous_Repl_trace_DateTime;

        public string after_Repl_trace;
        public int after_Repl_trace_ruid;
        public DateTime after_Repl_trace_DateTime;
      
        //SPARE_UNLOAD_SECTION nvarchar(50)    Checked
        public int CaptureCards;
        public int ATM_cassette1;
        public int ATM_cassette2;
        public int ATM_cassette3;
        public int ATM_cassette4;
        public int ATM_cassette5;

        public int ATM_Rejected1;
        public int ATM_Rejected2;
        public int ATM_Rejected3;
        public int ATM_Rejected4;
        public int ATM_Rejected5;

        public int ATM_Remaining1;
        public int ATM_Remaining2;
        public int ATM_Remaining3;
        public int ATM_Remaining4;
        public int ATM_Remaining5;

        public int ATM_Dispensed1;
        public int ATM_Dispensed2;
        public int ATM_Dispensed3;
        public int ATM_Dispensed4;
        public int ATM_Dispensed5;

        public int ATM_total1;
        public int ATM_total2;
        public int ATM_total3;
        public int ATM_total4;
        public int ATM_total5;

        public string SPARE_LOAD_SECTION;
        public DateTime LastClearRepl;
        public int ruid_max_cashCountClear; 
        public bool Flag_cashCountClear; 

        public int cashaddtype1;
        public int cashaddtype2;
        public int cashaddtype3;
        public int cashaddtype4;
        public int cashaddtype5;

        public int RRDM_ReplCycleNo;
        public DateTime RRDM_DateTime_Created;
        public bool RRDM_Processed;

        // Other Fields

        public int LoadedAtRMCycle ;
        public int DEP_ENCASHED ;
        public int DEP_REJECTED ;
        public int DEP_RETRACTED ;
        public int DEP_IN_TRANSPORT ;
        public int DEP_ESCRW_DEPS ;
        public int DEP_ESCRW_RFND ;
        public int DEP_COUNTERFEIT ;
        public int DEP_SUSPECT;
        public string is_recycle; // Y and N 

        // Fields FOR [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis]

        //        SeqNo int Unchecked

        // Deposits
      
        public DateTime SM_DATE_TIME;
        public string TYPE;
        public string Currency;
      
        public int FaceValue;
        public int CASSETTE;
        public int RETRACT;
      
        public int RECYCLED;
        public int ReplCycle;
        public bool Processed;

        // COUNTED
        public int TotalCassetteNotesCount;
        public decimal TotalCassetteAmountCount;

        public int TotalRetractedNotesCount;
        public decimal TotalRetractedAmountCount;
       
        public int TotalRecycledNotesCount;
        public decimal TotalRecycledAmountCount;

        //    string SqlString; // Do not delete

        public DataTable DataTable_SM = new DataTable();

        public DataTable DataTable_SM_Deposits = new DataTable();
        public DataTable DataTable_SM_Deposits_2 = new DataTable();
        public DataTable DataTable_SM_Deposits_3 = new DataTable();
        public DataTable DataTable_Forex = new DataTable();
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        string SM_Table = "[ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table]";

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // Reader Fields 
        private void SM_ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            AtmNo = (string)rdr["AtmNo"];
            FlagValid = (string)rdr["FlagValid"];
            AdditionalCash = (string)rdr["AdditionalCash"];

            BANK = (string)rdr["BANK"];
            Fuid = (int)rdr["Fuid"];
          
            txtline = (string)rdr["txtline"];
            RUID = (int)rdr["RUID"];
            ruid_max_cashAdded = (int)rdr["ruid_max_cashAdded"];
            seqnum = (int)rdr["seqnum"];
            sessionstart_ruid = (int)rdr["sessionstart_ruid"];

            sessionend_ruid = (int)rdr["sessionend_ruid"];

            SM_dateTime_Start = (DateTime)rdr["SM_dateTime_Start"];
            SM_dateTime_Finish = (DateTime)rdr["SM_dateTime_Finish"];

            SM_LAST_CLEARED = (DateTime)rdr["SM_LAST_CLEARED"];
            
            previous_Repl_trace = (string)rdr["previous_Repl_trace"];
            previous_Repl_trace_ruid = (int)rdr["previous_Repl_trace_ruid"];

            previous_Repl_trace_DateTime = (DateTime)rdr["previous_Repl_trace_DateTime"];
            after_Repl_trace = (string)rdr["after_Repl_trace"];
            after_Repl_trace_ruid = (int)rdr["after_Repl_trace_ruid"];
            after_Repl_trace_DateTime = (DateTime)rdr["after_Repl_trace_DateTime"];

         
            CaptureCards = (int)rdr["CaptureCards"];

            ATM_cassette1 = (int)rdr["ATM_cassette1"];
            ATM_cassette2 = (int)rdr["ATM_cassette2"];
            ATM_cassette3 = (int)rdr["ATM_cassette3"];
            ATM_cassette4 = (int)rdr["ATM_cassette4"];
            ATM_cassette5 = (int)rdr["ATM_cassette5"];

            ATM_Rejected1 = (int)rdr["ATM_Rejected1"];
            ATM_Rejected2 = (int)rdr["ATM_Rejected2"];
            ATM_Rejected3 = (int)rdr["ATM_Rejected3"];
            ATM_Rejected4 = (int)rdr["ATM_Rejected4"];
            ATM_Rejected5 = (int)rdr["ATM_Rejected5"];

            ATM_Remaining1 = (int)rdr["ATM_Remaining1"];
            ATM_Remaining2 = (int)rdr["ATM_Remaining2"];
            ATM_Remaining3 = (int)rdr["ATM_Remaining3"];
            ATM_Remaining4 = (int)rdr["ATM_Remaining4"];
            ATM_Remaining5 = (int)rdr["ATM_Remaining5"];

            ATM_Dispensed1 = (int)rdr["ATM_Dispensed1"];
            ATM_Dispensed2 = (int)rdr["ATM_Dispensed2"];
            ATM_Dispensed3 = (int)rdr["ATM_Dispensed3"];
            ATM_Dispensed4 = (int)rdr["ATM_Dispensed4"];
            ATM_Dispensed5 = (int)rdr["ATM_Dispensed5"];

            ATM_total1 = (int)rdr["ATM_total1"];
            ATM_total2 = (int)rdr["ATM_total2"];
            ATM_total3 = (int)rdr["ATM_total3"];
            ATM_total4 = (int)rdr["ATM_total4"];
            ATM_total5 = (int)rdr["ATM_total5"];

            LastClearRepl = (DateTime)rdr["LastClearRepl"];
            ruid_max_cashCountClear = (int)rdr["ruid_max_cashCountClear"];
            Flag_cashCountClear = (bool)rdr["Flag_cashCountClear"];

            cashaddtype1 = (int)rdr["cashaddtype1"];
            cashaddtype2 = (int)rdr["cashaddtype2"];
            cashaddtype3 = (int)rdr["cashaddtype3"];
            cashaddtype4 = (int)rdr["cashaddtype4"];
            cashaddtype5 = (int)rdr["cashaddtype5"];

            RRDM_ReplCycleNo = (int)rdr["RRDM_ReplCycleNo"];
            RRDM_DateTime_Created = (DateTime)rdr["RRDM_DateTime_Created"];
            RRDM_Processed = (bool)rdr["RRDM_Processed"];

            LoadedAtRMCycle = (int) rdr["LoadedAtRMCycle"];
            DEP_ENCASHED = (int) rdr["DEP_ENCASHED"];
            DEP_REJECTED = (int) rdr["DEP_REJECTED"];

            DEP_RETRACTED = (int) rdr["DEP_RETRACTED"];
            DEP_IN_TRANSPORT = (int) rdr["DEP_IN_TRANSPORT"];
            DEP_ESCRW_DEPS = (int) rdr["DEP_ESCRW_DEPS"];

            DEP_ESCRW_RFND = (int) rdr["DEP_ESCRW_RFND"];
            DEP_COUNTERFEIT = (int) rdr["DEP_COUNTERFEIT"];
            DEP_SUSPECT = (int) rdr["DEP_SUSPECT"];
            is_recycle = (string)rdr["is_recycle"];
        }

        private void SM_ReaderFields_Deposits(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];
            Fuid = (int)rdr["Fuid"];
            AtmNo = (string)rdr["AtmNo"];
           
            SM_DATE_TIME = (DateTime)rdr["SM_DATE_TIME"];
            TYPE = (string)rdr["TYPE"];
            Currency = (string)rdr["Currency"];
            
            FaceValue = (int)rdr["FaceValue"];
            CASSETTE = (int)rdr["CASSETTE"];
            RETRACT = (int)rdr["RETRACT"];
          
            RECYCLED = (int)rdr["RECYCLED"];
            ReplCycle = (int)rdr["ReplCycle"];
            Processed = (bool)rdr["Processed"];
          
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
           
        }

        private void SM_ReaderFields_Deposits_Counted(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"]; //
            AtmNo = (string)rdr["AtmNo"]; //
            ReplCycle = (int)rdr["ReplCycle"];

            Currency = (string)rdr["Currency"];

            TotalCassetteNotesCount = (int)rdr["TotalCassetteNotesCount"];
            TotalCassetteAmountCount = (decimal)rdr["TotalCassetteAmountCount"];

            TotalRetractedNotesCount = (int)rdr["TotalRetractedNotesCount"];
            TotalRetractedAmountCount = (decimal)rdr["TotalRetractedAmountCount"];

            TotalRecycledNotesCount = (int)rdr["TotalRecycledNotesCount"];
            TotalRecycledAmountCount = (decimal)rdr["TotalRecycledAmountCount"];

        }

        // Methods 
        // READ TableATMsPhys
        // 
        public bool IsValid ;
        public void Read_SM_Record_Specific(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            IsValid = false; 

            string SqlString = "SELECT *"
          + " FROM " + SM_Table
          + " WHERE SeqNo=@SeqNo ";
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

                            SM_ReaderFields(rdr);

                            // VALIDATE

                            if (
                                // Make first check 
                              ATM_total1 == 0 & ATM_Dispensed1 == 0 & ATM_Remaining1 == 0
                            & ATM_total2 == 0 & ATM_Dispensed2 == 0 & ATM_Remaining2 == 0
                            & ATM_total3 == 0 & ATM_Dispensed3 == 0 & ATM_Remaining3 == 0
                            & ATM_total4 == 0 & ATM_Dispensed4 == 0 & ATM_Remaining4 == 0
                                )
                            {
                                IsValid = false;
                            }
                            else
                            {
                                IsValid = true;
                            }

                            if (IsValid == true)
                            {
                                // Make second check 
                                if (ATM_total1 - ATM_Dispensed1 - ATM_Remaining1 != 0
                           || ATM_total2 - ATM_Dispensed2 - ATM_Remaining2 != 0
                           || ATM_total3 - ATM_Dispensed3 - ATM_Remaining3 != 0
                           || ATM_total4 - ATM_Dispensed4 - ATM_Remaining4 != 0
                               )
                                {
                                    IsValid = false;
                                }
                                else
                                {
                                    IsValid = true;
                                }
                            }
                            if (IsValid == true)
                            {
                                // Make Last Cleared check 
                                if (SM_LAST_CLEARED == NullPastDate
                                  //  || SM_dateTime_Start > SM_dateTime_Finish   
                                    )
                                {
                                    IsValid = false;
                                }
                                else
                                {
                                    IsValid = true;
                                }
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

                    CatchDetails(ex, "",0);
                }
        }

        // Methods 
        // READ TableATMsPhys
        // 
        public int Total_cashaddtype1;
        public int Total_cashaddtype2;
        public int Total_cashaddtype3;
        public int Total_cashaddtype4;

        public void Read_SM_Record_For_AddedCash(string InAtmNo, int InRRDM_ReplCycleNo, DateTime InCap_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Total_cashaddtype1 = 0;
            Total_cashaddtype2 = 0;
            Total_cashaddtype3 = 0;
            Total_cashaddtype4 = 0;

        string SqlString = "SELECT *"
          + " FROM " + SM_Table
          + " WHERE RRDM_ReplCycleNo=@RRDM_ReplCycleNo "
          + " AND AdditionalCash = 'Y' AND Cast(SM_dateTime_Start as Date)>@Cap_Date ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RRDM_ReplCycleNo", InRRDM_ReplCycleNo);
                        cmd.Parameters.AddWithValue("@Cap_Date", InCap_Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SM_ReaderFields(rdr);

                            Total_cashaddtype1 = Total_cashaddtype1 + cashaddtype1;
                            Total_cashaddtype2 = Total_cashaddtype2 + cashaddtype2;
                            Total_cashaddtype3 = Total_cashaddtype3 + cashaddtype3;
                            Total_cashaddtype4 = Total_cashaddtype4 + cashaddtype4;

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

                    CatchDetails(ex, "", 0);
                }
        }


        // Methods 
        // READ TableATMsPhys
        // 
        public int Read_SM_Find_Cycle_No_From_End_Date(string InAtmNo, DateTime InSM_dateTime_Start, string InSortBy)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SeqNo = 0; 

            string SqlString = "SELECT  TOP (1) *  "
              + " FROM " + SM_Table
              + " WHERE AtmNo=@AtmNo AND FlagValid = 'Y' AND AdditionalCash = 'N'  "
              + " AND CAST(SM_dateTime_Start AS DATE) = @SM_dateTime_Start "
              + InSortBy
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
                        cmd.Parameters.AddWithValue("@SM_dateTime_Start", InSM_dateTime_Start.Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            SM_ReaderFields(rdr);

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

                    CatchDetails(ex, InAtmNo, 0);
                }
            return SeqNo; 
        }

        // Methods 
        // READ Table
        // 
        public void Read_SM_Record_Specific_By_Selection(string InSelectionCriteria, string InAtmNo, int InFuid, int Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 1 comes from loading of files, The AtmNo and Fuid.  
          // string SM_Table = "[ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table]";

            string SqlString = "SELECT * "
          + " FROM " + SM_Table
          + InSelectionCriteria ;
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       // cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                           
                            SM_ReaderFields(rdr);

                            RecordFound = true;

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

                    CatchDetails(ex, InAtmNo, InFuid);
                }
        }

        public void Read_SM_Record_Specific_By_Selection_ROM(string InSelectionCriteria, string InAtmNo, int InFuid, int Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 1 comes from loading of files, The AtmNo and Fuid.  

            string SqlString = "SELECT * "
          + " FROM " + SM_Table
          + InSelectionCriteria;
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            SM_ReaderFields(rdr);

                            RecordFound = true;

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

                    CatchDetails(ex, InAtmNo, InFuid);
                }
        }

        // Methods 
        // READ Table
        // For Cash added after replenishement 
        public void Read_SM_Record_Specific_By_ReplCycle(int InRRDM_ReplCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //FlagValid = (string)rdr["FlagValid"];
            //AdditionalCash = (string)rdr["AdditionalCash"];

            string SqlString = "SELECT *"
          + " FROM " + SM_Table
          + " WHERE RRDM_ReplCycleNo =@RRDM_ReplCycleNo AND FlagValid = 'Y' AND AdditionalCash = 'N' ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RRDM_ReplCycleNo", InRRDM_ReplCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            SM_ReaderFields(rdr);

                            RecordFound = true;

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

                    CatchDetails(ex, "SearchThis", 99);
                }
        }

        // Methods 
        // READ Table
        // For Cash added after replenishement 
        public void Read_SM_Record_Specific_By_ATMno_ReplCycle(string InAtmNo, int InRRDM_ReplCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //FlagValid = (string)rdr["FlagValid"];
            //AdditionalCash = (string)rdr["AdditionalCash"];

            string SqlString = "SELECT * "
          + " FROM " + SM_Table
          + " WHERE AtmNo = @AtmNo AND RRDM_ReplCycleNo =@RRDM_ReplCycleNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RRDM_ReplCycleNo", InRRDM_ReplCycleNo);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            SM_ReaderFields(rdr);

                            RecordFound = true;

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

                    CatchDetails(ex, "SearchThis", 99);
                }
        }
        //
        // FILL TABLE for CASSETTES WITHDRAWLS
        //
        public void ReadT_SM_AND_FillTable(string InSelectionCriteria, string InOrderBy)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTable_SM = new DataTable();
            DataTable_SM.Clear();

            string SqlString = "SELECT * "
                      + " FROM " + SM_Table
                      + InSelectionCriteria
                      +  InOrderBy ;

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                       // sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);
                       
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTable_SM);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex, "",0);
                }

        }

        //
        // FILL TABLE for CASSETTES WITHDRAWLS
        //
        //public void ReadT_SM_AND_FillTable_Handle_AddedCash()
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    DataTable_SM = new DataTable();
        //    DataTable_SM.Clear();


        //    string SqlString = " SELECT AtmNo, fuid "
        //        + " , cashaddtype1 "
        //       + " , cashaddtype2 "
        //       + " , cashaddtype3 "
        //       + " , cashaddtype4 "
        //       + "  FROM[ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table] "
        //       + " WHERE AdditionalCash = 'Y'  AND RRDM_Processed = 0 "
        //      // + " group by fuid "
        //       ;

        //    using (SqlConnection conn =
        //      new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();

        //            //Create an Sql Adapter that holds the connection and the command
        //            using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
        //            {
        //                // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
        //                // sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);

        //                //Create a datatable that will be filled with the data retrieved from the command

        //                sqlAdapt.Fill(DataTable_SM);

        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            CatchDetails(ex, "", 0);
        //        }

        //}

        // Get read First record in Deposits if exist
        public void Read_SM_Deposits_Get_DetailsOfFirstRecord(string InAtmNo, int InReplCycle, string InCurrency)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

     
            string SqlString =
                        "  SELECT TOP(1) * "                
                        + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                        + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle  AND Currency = @Currency  "
                        + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@Currency", InCurrency);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SM_ReaderFields_Deposits(rdr);

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

                    CatchDetails(ex, "", 0);
                }
    }

        // DELETE DEPOSIT ENTRIES 
        // THIS IS A PROCESS IN BADDIES
        public void Delete_Deposit_Entries(string InAtmNo, int InReplCycle, string InCurrency)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                            + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle  AND Currency = @Currency  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@Currency", InCurrency);

                        count = cmd.ExecuteNonQuery();

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
        // INsert Deposit Record due to Baddies action
        public void InsertDepositRecord(string InAtmNo, int InReplCycle, string InCurrency)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count; 


        string cmdinsert = "INSERT INTO [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                    + "([FuId],  "
                    + " [AtmNo], "
                      + " [SM_DATE_TIME], "
                        + " [TYPE], "

                          + " [Currency], "
                            + " [FaceValue], "
                              + " [CASSETTE], "
                                + " [RETRACT], "

                            + " [RECYCLED], "
                            // + " [NCR_DepositsDispensed], "
                            // NCR_DepositsDispensed
                            + " [ReplCycle], "
                            + " [Processed], "
                     + " [LoadedAtRMCycle] )"
                    + " VALUES (@FuId, "
                     + " @AtmNo, "
                       + " @SM_DATE_TIME, "
                         + " @TYPE, "

                           + " @Currency, "
                             + " @FaceValue, "
                               + " @CASSETTE, "
                                 + " @RETRACT, "

                             + " @RECYCLED, "
                           //   + " @NCR_DepositsDispensed, "
                             + " @ReplCycle, "
                             + " @Processed, "
                    + "  @LoadedAtRMCycle )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@FuId", Fuid);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SM_DATE_TIME", SM_DATE_TIME);
                        cmd.Parameters.AddWithValue("@TYPE", TYPE);

                        cmd.Parameters.AddWithValue("@Currency", InCurrency);
                        cmd.Parameters.AddWithValue("@FaceValue", FaceValue);
                        cmd.Parameters.AddWithValue("@CASSETTE", CASSETTE);
                        cmd.Parameters.AddWithValue("@RETRACT", RETRACT);

                        cmd.Parameters.AddWithValue("@RECYCLED", RECYCLED);
                       
                        //cmd.Parameters.AddWithValue("@NCR_DepositsDispensed", NCR_DepositsDispensed);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@Processed", Processed);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);

                        Count = cmd.ExecuteNonQuery();

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
        // Get totals for SM 
        public void Read_SM_AND_FillTable_Deposits(string InAtmNo, int InReplCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTable_SM_Deposits = new DataTable();
            DataTable_SM_Deposits.Clear();

            string SqlString =
                        "  SELECT Currency As Ccy, "
                        + " SUM(Cassette) as TotalCassetteNotes, "
                        + " CAST(SUM(Facevalue * CASSETTE) As Decimal(18, 2)) As TotalCassetteMoney, "

                         + " SUM(RETRACT) as TotalRETRACTNotes, "
                        + " CAST(SUM(Facevalue * RETRACT) As Decimal(18, 2)) As TotalRETRACTMoney, "

                         + " SUM(RECYCLED) as TotalRECYCLEDNotes, "
                        + " CAST(SUM(Facevalue * RECYCLED) As Decimal(18, 2)) As TotalRECYCLEDMoney "

                        + " FROM[ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                        + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle "
                        + " GROUP By Currency " 
                        + " Order By Currency " ;

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                       

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTable_SM_Deposits);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex, "", 0);
                }

            if (DataTable_SM_Deposits.Rows.Count>0)
            {
                RecordFound = true; 
            }
            else
            {
                RecordFound = false;
            }

        }

        // Get totals for SM for Deposits for certain fuid 
        public void Read_SM_AND_FillTable_Deposits_2(string InAtmNo, int InReplCycle, int InFuid)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTable_SM_Deposits = new DataTable();
            DataTable_SM_Deposits.Clear();

            string SqlString =
                        "  SELECT Currency As Ccy, "
                        + " SUM(Cassette) as TotalCassetteNotes, "
                        + " CAST(SUM(Facevalue * CASSETTE) As Decimal(18, 2)) As TotalCassetteMoney, "

                         + " SUM(RETRACT) as TotalRETRACTNotes, "
                        + " CAST(SUM(Facevalue * RETRACT) As Decimal(18, 2)) As TotalRETRACTMoney, "

                         + " SUM(RECYCLED) as TotalRECYCLEDNotes, "
                        + " CAST(SUM(Facevalue * RECYCLED) As Decimal(18, 2)) As TotalRECYCLEDMoney "

                        + " FROM[ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                        + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle AND Fuid = @Fuid"
                        + " GROUP By Currency "
                        + " Order By Currency ";

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Fuid", InFuid);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTable_SM_Deposits);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex, "", 0);
                }

            if (DataTable_SM_Deposits.Rows.Count > 0)
            {
                RecordFound = true;
            }
            else
            {
                RecordFound = false;
            }

        }

        public int TotalCassetteNotes; 
        public decimal TotalCassetteMoney;

        public int TotalRETRACTNotes;
        public decimal TotalRETRACTMoney;

        // Get totals for SM for Deposits for certain fuid 
        public void Read_SM_AND_FillTable_Deposits_2_FOR_Ccy(string InAtmNo, int InReplCycle, string InCurrency)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //DataTable_SM_Deposits = new DataTable();
            //DataTable_SM_Deposits.Clear();

            string SqlString =
                        "  SELECT Currency As Ccy, "
                        + " SUM(Cassette) as TotalCassetteNotes, "
                        + " CAST(SUM(Facevalue * CASSETTE) As Decimal(18, 2)) As TotalCassetteMoney, "

                         + " SUM(RETRACT) as TotalRETRACTNotes, "
                        + " CAST(SUM(Facevalue * RETRACT) As Decimal(18, 2)) As TotalRETRACTMoney, "

                         + " SUM(RECYCLED) as TotalRECYCLEDNotes, "
                        + " CAST(SUM(Facevalue * RECYCLED) As Decimal(18, 2)) As TotalRECYCLEDMoney "

                        + " FROM[ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                        + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle AND Currency = @Currency"
                          + " GROUP By Currency "; 
                       // + " Order By Currency ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@Currency", InCurrency);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
        
                            TotalCassetteNotes = (int)rdr["TotalCassetteNotes"];
                            TotalCassetteMoney = (decimal)rdr["TotalCassetteMoney"];
                            TotalRETRACTNotes = (int)rdr["TotalRETRACTNotes"];
                            TotalRETRACTMoney = (decimal)rdr["TotalRETRACTMoney"];

                            RecordFound = true;

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

                    CatchDetails(ex, "SearchThis", 99);
                }


        }

        // Get Fuid from Depoists get first fuid
        public void Read_SM_AND_Get_First_fuid(string InAtmNo, int InReplCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
     
            string SqlString =
                        "  SELECT  TOP (1) "
                        + " [FuId],  [AtmNo],[ReplCycle]  "
                        + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                        + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle "
                        + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            AtmNo = (string)rdr["AtmNo"];
                          
                            Fuid = (int)rdr["fuid"];

                            ReplCycle = (int)rdr["ReplCycle"];

                            RecordFound = true;

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

                    CatchDetails(ex, "SearchThis", 99);
                }

        }

        // Get totals for SM 
        public void Read_SM_AND_FillTable_Deposits_2(string InAtmNo, int InReplCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTable_SM_Deposits_2 = new DataTable();
            DataTable_SM_Deposits_2.Clear();

            string SqlString =
                        "  SELECT  "
                        + "  [AtmNo],[ReplCycle]  "
                        + "  ,[Currency] ,[FaceValue]  "
                        + "  ,[CASSETTE] ,[RETRACT] ,[RECYCLED]  "
                        + "  , FaceValue* CASSETTE As CASSETTE_Amt "
                        + "  , FaceValue * RETRACT As RETRACT_Amt  "
                        + "  , FaceValue* RECYCLED As RECYCLED_Amt  "
                        + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                        + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle "
                        + " ORDER BY Currency, FaceValue "; 
     
            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplCycle", InReplCycle);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTable_SM_Deposits_2);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex, "", 0);
                }

        }

        // Get totals for FOREX Deposited 

        public void Read_ForexChildALL_Get_Txns(string InAtmNo, DateTime InFromDt, DateTime InToDt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTable_Forex = new DataTable();
            DataTable_Forex.Clear();

      //      SELECT[SeqNo]
      //,[NewATMDate]
      //,[ATMId]
      //,[TransactionSequence]
      //,[TotalNetValue]
      //,[CurrencyCode]
      //,[DepositAmt]
      //,[LcyEquivalentTotal]
      //,[NoteRate]
      //,[CommisionTotal]
      //,[Canceled]
      //,[Confirmed]
      //,[Withdrawl]
      //,[LoadedAtRMCycle]
      //  FROM[RRDM_Reconciliation_ITMX].[dbo].[BULK_NCR_FOREX_CHILD_ALL]

        string SqlString =
                        "  SELECT  * "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[BULK_NCR_FOREX_CHILD_ALL] "
                        + " WHERE ATMId = @AtmNo AND (NewATMDate BETWEEN @FromDt AND @ToDt) "
                        + " ORDER BY TransactionSequence, DepositAmt ";

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDt", InFromDt);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ToDt", InToDt);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTable_Forex);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex, "", 0);
                }

        }
        public decimal TotalForCust ;
        public decimal TotalCommision ;

        public void Read_ForexChildALL_Get_Totals(string InAtmNo, DateTime InFromDt, DateTime InToDt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalForCust = 0 ;
            TotalCommision = 0 ;


            string SqlString =
                            "  SELECT ISNULL(SUM(TotalNetValue),0) As TotalForCust , ISNULL(SUM(CommisionTotal),0) As TotalCommision    "
                            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[BULK_NCR_FOREX_CHILD_ALL] "
                            + " WHERE ATMId = @AtmNo AND (NewATMDate BETWEEN @FromDt AND @ToDt) "
                            + "  ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                        cmd.Parameters.AddWithValue("@ToDt", InToDt);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalForCust = (decimal)rdr["TotalForCust"];
                            TotalCommision = (decimal)rdr["TotalCommision"];

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

                    CatchDetails(ex, "", 0);
                }
            if (TotalForCust == 0 & TotalCommision ==0)
            {
                RecordFound = false;
            }
        }

        //public void Read_SM_Record_Specific_Deposits(int InSeqNo)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    string SqlString = "SELECT *"
        //  + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis]  " 
        //  + " WHERE SeqNo=@SeqNo ";

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    SM_ReaderFields_Deposits(rdr);

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

        //            CatchDetails(ex, "", 0);
        //        }
        //}

        // Get counted 
        public void Read_SM_AND_Get_CountedByCcy(string InAtmNo, int InReplCycle, string InCurrency)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTable_SM_Deposits_2 = new DataTable();
            DataTable_SM_Deposits_2.Clear();

            string SqlString =
                        "  SELECT TOP (1) * " 
                        + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis_Count] "
                        + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle AND Currency = @Currency "
                        + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@Currency", InCurrency);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SM_ReaderFields_Deposits_Counted(rdr);

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

                    CatchDetails(ex, "", 0);
                }

           

        }

        // //
        // // Totals By Currency
        // //
        // public void ReadDepositsAndGet_Table_With_TotalsByCurrency(string InAtmNo, int InReplCycle)
        // {
        //     RecordFound = false;
        //     ErrorFound = false;
        //     ErrorOutput = "";

        //     DataTable_SM_Deposits_3 = new DataTable();
        //     DataTable_SM_Deposits_3.Clear();

        //     string SqlString =
        //  " SELECT [Currency] "
        //+ "     ,Sum([notes]) As TotalNotes "
        //+ " FROM [ATM_MT_Journals_AUDI].[dbo].[Deposit_Txns_Analysis] "
        //+ "  where [AtmNo] = @AtmNo AND ReplCycle = @ReplCycle  "
        //+ "  group by [Currency] "
        //+ "  Order By Currency  ";


        //     using (SqlConnection conn =
        //    new SqlConnection(connectionString))
        //         try
        //         {
        //             conn.Open();

        //             //Create an Sql Adapter that holds the connection and the command
        //             using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
        //             {

        //                 sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
        //                 sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplCycle", InReplCycle);
        //                 //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Currency", InCurrency);

        //                 //Create a datatable that will be filled with the data retrieved from the command

        //                 sqlAdapt.Fill(DataTable_SM_Deposits_3);

        //             }
        //             // Close conn
        //             conn.Close();
        //         }
        //         catch (Exception ex)
        //         {
        //             conn.Close();
        //             CatchDetails(ex);
        //         }

        // }


        public void ReadSMLineByReplCycleFillTable(string InSignedId, string InTable, string InTerminalId, int InSesNo, int In_DB_Mode)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            DataTable_SM = new DataTable();
            DataTable_SM.Clear();

            string SqlString = "SELECT * "

                + " FROM [ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table] "
                  + "  WHERE AtmNo = @AtmNo AND RRDM_ReplCycleNo =@RRDM_ReplCycleNo "
                  + " ORDER by SM_dateTime_Start  "
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
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RRDM_ReplCycleNo", InSesNo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTable_SM);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex, "", 0);
                }

        }

        // 
        // UPDATE  table with ReplCycle etc 
        //
        public void Update_SM_Record(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = 
                        new SqlCommand("UPDATE " + SM_Table
                        +" SET "
                    + " [RRDM_ReplCycleNo]=@RRDM_ReplCycleNo "
                    + " ,[RRDM_DateTime_Created]=@RRDM_DateTime_Created"
                    + " ,[LoadedAtRMCycle]=@LoadedAtRMCycle"
                    + " , [RRDM_Processed]=@RRDM_Processed "
                    + " WHERE SeqNo=@SeqNo  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@RRDM_ReplCycleNo", RRDM_ReplCycleNo);
                     
                        cmd.Parameters.AddWithValue("@RRDM_DateTime_Created", RRDM_DateTime_Created);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
                        cmd.Parameters.AddWithValue("@RRDM_Processed", RRDM_Processed);

                        // Execute and check success 
                        cmd.CommandTimeout = 200; 
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

                    CatchDetails(ex, "",0);
                }

           

            //  return outcome;

        }

        // DURING UNDO UPDATE AS NOT PROCESSED
        // UPDATE  table with ReplCycle etc 
        //
        public void Update_SM_RecordsForCycle(int InLoadedAtRMCycle, bool InRRDM_Processed)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + SM_Table
                        + " SET "
                       + "  [RRDM_Processed]=@RRDM_Processed, [RRDM_ReplCycleNo] = 0 "
                       + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle  ", conn))
                    {
                      
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);

                   
                        cmd.Parameters.AddWithValue("@RRDM_Processed", InRRDM_Processed);

                        // Execute and check success 
                        cmd.CommandTimeout = 200;
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

                    CatchDetails(ex, "", 0);
                }



            //  return outcome;

        }

        // 
        // UPDATE  table 
        //
        public void Update_SM_RecordFromNewData(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + SM_Table
                        + " SET "
                    + " [fuid]=@fuid "
                    + " ,[sessionstart_ruid]=@sessionstart_ruid"
                    + " , [sessionend_ruid]=@sessionend_ruid "
                    + " WHERE SeqNo=@SeqNo  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@fuid", Fuid);

                        cmd.Parameters.AddWithValue("@sessionstart_ruid", sessionstart_ruid);
                        cmd.Parameters.AddWithValue("@sessionend_ruid", sessionend_ruid);

                        // Execute and check success 
                        cmd.CommandTimeout = 200;
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

                    CatchDetails(ex, "", 0);
                }



            //  return outcome;

        }

        // 
        // UPDATE  table with ReplCycle etc 
        //
        public void Update_SM_RecordWithNewJournal(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + SM_Table
                        + " SET "
                    + " [RRDM_ReplCycleNo]=@RRDM_ReplCycleNo "
                    + " ,[RRDM_DateTime_Created]=@RRDM_DateTime_Created"
                    + " , [RRDM_Processed]=@RRDM_Processed "
                    + " WHERE SeqNo=@SeqNo  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@RRDM_ReplCycleNo", RRDM_ReplCycleNo);

                        cmd.Parameters.AddWithValue("@RRDM_DateTime_Created", RRDM_DateTime_Created);
                        cmd.Parameters.AddWithValue("@RRDM_Processed", RRDM_Processed);

                        // Execute and check success 
                        cmd.CommandTimeout = 200;
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

                    CatchDetails(ex, "", 0);
                }

            //  return outcome;

        }

        // 
        // UPDATE  Deposits table with ReplCycle etc 
        //
        public void Update_SM_Deposit_analysis(string InAtmNo, int Infuid)
        {
            ErrorFound = false;
            ErrorOutput = "";
      
            int Counter = 0;

            string SQLCmd =
        " UPDATE [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
        + " SET "
         + " ReplCycle = RRDM_ReplCycleNo "
         + " , Processed = 1 "
     + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] t1 "
     + " INNER JOIN  [ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table] t2 "

     + " ON "
     + " t1.Fuid= t2.fuid "
     + " AND t1.AtmNo= t2.AtmNo "
     + " WHERE  t1.AtmNo = @AtmNo AND t1.Fuid = @Fuid "; // For not processed yet records

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Fuid", Infuid);
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
                    //return;
                }

            //  return outcome;

        }

        // 
        // UPDATE  Deposits table with ReplCycle etc 
        //
        public void Update_SM_Deposit_analysis_ALPHA(int InLoadedAtRMCycle)
        {
            ErrorFound = false;
            ErrorOutput = "";
            // The input SeqNo is from Panicos_SM
            int Counter = 0;

            string SQLCmd =
        " UPDATE [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
        + " SET "
         + " ReplCycle = t2.RRDM_ReplCycleNo "
         + " , Processed = 1 "
     + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] t1 "
     + " INNER JOIN  [ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table] t2 "
     + " ON "
     + " t1.AtmNo= t2.AtmNo "
      + "AND t1.OriginSeqNo= t2.SeqNo "
      + " WHERE t1.LoadedAtRMCycle = @LoadedAtRMCycle AND t2.LoadedAtRMCycle = @LoadedAtRMCycle and t2.AdditionalCash = 'N' "
      ; 
     //+ " WHERE  t1.AtmNo = @AtmNo "
     //+ " AND t1.OriginSeqNo = @SeqNo "; // 

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);
                     //   cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
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
                    //return;
                }

            //  return outcome;

        }

        // Methods 
        // READ 
        // 
        public void Read_SM_Record_SpecificDepositsCounted(string InAtmNo, int InReplCycle, string InCurrency)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis_Count] " 
          + " WHERE AtmNo=@AtmNo AND ReplCycle=@ReplCycle AND Currency=@Currency ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@Currency", InCurrency);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SM_ReaderFields_Deposits_Counted(rdr);

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

                    CatchDetails(ex, "", 0);
                }
        }
        // Insert counted or Create the record 
        public void InsertCountedRecord(string InAtmNo, int InReplCycle, string InCurrency)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis_Count] "
                    + "([AtmNo],  "
                    + " [ReplCycle], "
                     + " [Currency] )"
                    + " VALUES (@AtmNo, "
                     + " @ReplCycle, "
                    + "  @Currency )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);

                        cmd.Parameters.AddWithValue("@Currency", InCurrency);

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
        // UPDATE  Deposits table with ReplCycle etc 
        // IN UCForm51c_SM
        public void Update_SM_Deposit_analysis_Counted(string InAtmNo, int InReplCycle, string InCurrency)
        {
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;
       
            using (SqlConnection conn =
                     new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis_Count]"
                        + " SET "
                    + " [TotalCassetteNotesCount]=@TotalCassetteNotesCount "
                    + " ,[TotalCassetteAmountCount]=@TotalCassetteAmountCount"

                    + " ,[TotalRetractedNotesCount]=@TotalRetractedNotesCount "
                    + " ,[TotalRetractedAmountCount]=@TotalRetractedAmountCount"

                    + " ,[TotalRecycledNotesCount]=@TotalRecycledNotesCount"
                    + " ,[TotalRecycledAmountCount]=@TotalRecycledAmountCount "
                    + " WHERE AtmNo=@AtmNo AND  ReplCycle=@ReplCycle AND Currency=@Currency ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@Currency", InCurrency);

                        cmd.Parameters.AddWithValue("@TotalCassetteNotesCount", TotalCassetteNotesCount);
                        cmd.Parameters.AddWithValue("@TotalCassetteAmountCount", TotalCassetteAmountCount);

                        cmd.Parameters.AddWithValue("@TotalRetractedNotesCount", TotalRetractedNotesCount);
                        cmd.Parameters.AddWithValue("@TotalRetractedAmountCount", TotalRetractedAmountCount);

                        cmd.Parameters.AddWithValue("@TotalRecycledNotesCount", TotalRecycledNotesCount);
                        cmd.Parameters.AddWithValue("@TotalRecycledAmountCount", TotalRecycledAmountCount);

                        // Execute and check success 
                        cmd.CommandTimeout = 200;
                        Counter = cmd.ExecuteNonQuery();
                        if (Counter > 0)
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

                    CatchDetails(ex, "", 0);
                }

            //  return outcome;

        }

        // 
        // UPDATE  NULLS 1
        //
        public void Update_SM_Record_NULL_Values_ONE(int InReconcCycleNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            string SQLCmd =
 " UPDATE[ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table] "
  + " SET "
   + "      [RUID] = ISNULL([RUID],0) "
    + "   ,[ruid_max_cashAdded] = ISNULL([ruid_max_cashAdded] ,0) "
   + "     ,[seqnum]= ISNULL([seqnum],0) "
    + "    ,[sessionstart_ruid]= ISNULL([sessionstart_ruid],0)"
    + "    ,[sessionend_ruid]= ISNULL([sessionend_ruid],0) "
    + "    ,[SM_dateTime_Start]= ISNULL([SM_dateTime_Start], ('1900-01-01')) "
    + "    ,[SM_dateTime_Finish]= ISNULL([SM_dateTime_Finish], ('1900-01-01')) "
     + "    ,[SM_LAST_CLEARED]= ISNULL([SM_LAST_CLEARED], ('1900-01-01')) "


    + "    ,[previous_Repl_trace]= ISNULL([previous_Repl_trace],'0') "
    + "    ,[previous_Repl_trace_ruid]= ISNULL([previous_Repl_trace_ruid],0) "
     + "   ,[previous_Repl_trace_DateTime]= ISNULL([previous_Repl_trace_DateTime], ('1900-01-01')) "
    + "    ,[after_Repl_trace]= ISNULL([after_Repl_trace],'0') "
    + "    ,[after_Repl_trace_ruid]= ISNULL([after_Repl_trace_ruid],0) "
    + "    ,[after_Repl_trace_DateTime]= ISNULL([after_Repl_trace_DateTime], ('1900-01-01')) "

    + "    ,[SPARE_UNLOAD_SECTION]= 'SPARE_UNLOAD_SECTION' "
    + "    ,[CaptureCards]= ISNULL([CaptureCards],0) "
 + " ,[ATM_cassette1]= ISNULL([ATM_cassette1],0) "
    + "    ,[ATM_cassette2]= ISNULL([ATM_cassette2],0) "
    + "    ,[ATM_cassette3]= ISNULL([ATM_cassette3],0) "
     + "   ,[ATM_cassette4]= ISNULL([ATM_cassette4],0) "
   + "     ,[ATM_cassette5]= ISNULL([ATM_cassette5],0) "

    + "    ,[ATM_Rejected1]= ISNULL([ATM_Rejected1],0) "
   + "     ,[ATM_Rejected2]= ISNULL([ATM_Rejected2],0) "
   + "     ,[ATM_Rejected3]= ISNULL([ATM_Rejected3],0) "
    + "    ,[ATM_Rejected4]= ISNULL([ATM_Rejected4],0) "
    + "    ,[ATM_Rejected5]= ISNULL([ATM_Rejected5],0) "

    + "    ,[ATM_Remaining1]= ISNULL([ATM_Remaining1],0) "
     + "    ,[ATM_Remaining2]= ISNULL([ATM_Remaining2],0) "
      + "    ,[ATM_Remaining3]= ISNULL([ATM_Remaining3],0) "
       + "    ,[ATM_Remaining4]= ISNULL([ATM_Remaining4],0) "
      + "      ,[ATM_Remaining5]= ISNULL([ATM_Remaining5],0) "

     + "       ,[ATM_Dispensed1]= ISNULL([ATM_Dispensed1],0) "
      + "       ,[ATM_Dispensed2]= ISNULL([ATM_Dispensed2],0) "
      + "        ,[ATM_Dispensed3]= ISNULL([ATM_Dispensed3],0) "
      + "         ,[ATM_Dispensed4]= ISNULL([ATM_Dispensed4],0) "
      + "          ,[ATM_Dispensed5]= ISNULL([ATM_Dispensed5],0) "


     + "   ,[ATM_total1]= ISNULL([ATM_total1],0) "
      + "   ,[ATM_total2]= ISNULL([ATM_total2],0)"
      + "    ,[ATM_total3]= ISNULL([ATM_total3],0) "
      + "     ,[ATM_total4]= ISNULL([ATM_total4],0) "
       + "     ,[ATM_total5]= ISNULL([ATM_total5],0) "

   + "     ,[SPARE_LOAD_SECTION]= 'SPARE_LOAD_SECTION' "
    + "    ,[LastClearRepl]= ISNULL([LastClearRepl], ('1900-01-01')) "
     + "   ,[ruid_max_cashCountClear]= ISNULL([ruid_max_cashCountClear],0) "
     + "   ,[Flag_cashCountClear]= ISNULL([Flag_cashCountClear],0) "

     + "   ,[cashaddtype1]= ISNULL([cashaddtype1],0) "
      + "   ,[cashaddtype2]= ISNULL([cashaddtype2],0) "
    + "      ,[cashaddtype3]= ISNULL([cashaddtype3],0) "
      + "     ,[cashaddtype4]= ISNULL([cashaddtype4],0) "
      + "      ,[cashaddtype5]= ISNULL([cashaddtype5],0) "

        + "   ,[LoadedAtRMCycle]= ISNULL([LoadedAtRMCycle],@LoadedAtRMCycle) "

     + "   ,[RRDM_ReplCycleNo]= ISNULL([RRDM_ReplCycleNo],0) "

      + "  ,[RRDM_Processed]= ISNULL([RRDM_Processed],0) "
        
        + "  ,[DEP_ENCASHED]= ISNULL([DEP_ENCASHED],0) "
          + "  ,[DEP_REJECTED]= ISNULL([DEP_REJECTED],0) "
            + "  ,[DEP_RETRACTED]= ISNULL([DEP_RETRACTED],0) "
              + "  ,[DEP_IN_TRANSPORT]= ISNULL([DEP_IN_TRANSPORT],0) "

          + "  ,[DEP_ESCRW_DEPS]= ISNULL([DEP_ESCRW_DEPS],0) "
            + "  ,[DEP_ESCRW_RFND]= ISNULL([DEP_ESCRW_RFND],0) "
              + "  ,[DEP_COUNTERFEIT]= ISNULL([DEP_COUNTERFEIT],0) "
                + "  ,[DEP_SUSPECT]= ISNULL([DEP_SUSPECT],0) "

      + " WHERE RRDM_Processed = 0 OR RRDM_Processed IS Null "
      + "  "; 
          
            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, "", 0);
                }

        }

        // 
        // UPDATE  NULLS 2
        //
        public void Update_SM_Record_NULL_Values_TWO()
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + SM_Table
                        + " SET "
                        + "[FlagValid] = 'Y'"
                  //  + " ,[SM_dateTime_Finish] = [SM_dateTime_Start] "
     + "  ,[previous_Repl_trace_DateTime] = [SM_dateTime_Start] "
     + "  ,[after_Repl_trace_DateTime] = [SM_dateTime_Start] "
    
     + "  ,[LastClearRepl] = [SM_dateTime_Start]  "
    
     + "  ,[Flag_cashCountClear] = 1  "
  
     + "  ,[RRDM_DateTime_Created] = '1900-01-01 00:00:00.000'  "
    
            + " WHERE RRDM_Processed = 0  ", conn))
                    {
                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        //cmd.Parameters.AddWithValue("@RRDM_ReplCycleNo", RRDM_ReplCycleNo);

                        //cmd.Parameters.AddWithValue("@RRDM_DateTime_Created", RRDM_DateTime_Created);
                        //cmd.Parameters.AddWithValue("@RRDM_Processed", RRDM_Processed);

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

                    CatchDetails(ex, "", 0);
                }

            //  return outcome;

        }

        // 
        // UPDATE  NULLS 3
        //
        public void Update_SM_Record_NULL_Values_THREE()
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + SM_Table
                        + " SET "
                        + "[SM_dateTime_Finish] = [SM_dateTime_Start]"
                       + " WHERE SM_dateTime_Finish = '1900-01-01' OR [SM_dateTime_Start] > [SM_dateTime_Finish] ", conn))
                    {
                       
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

                    CatchDetails(ex, "", 0);
                }

            //  return outcome;

        }

        // 
        // UPDATE  ALPHA ADDITIONAL AMOUNT
        //
        //public void Update_SM_Record_WithAddedCash_ALPHA()
        //{
        //    //string SqlString = " SELECT AtmNo, fuid "
        //    //   + " , cashaddtype1 "
        //    //  + " , cashaddtype2 "
        //    //  + " , cashaddtype3 "
        //    //  + " , cashaddtype4 "
        //    //  + "  FROM[ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table] "
        //    //  + " WHERE AdditionalCash = 'Y'  AND RRDM_Processed = 0 "

        //    //          select fuid, sum(cashaddtype1) , sum(cashaddtype2)
        //    // , sum(cashaddtype3), sum(cashaddtype4)
        //    //FROM[ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table]
        //    //group by fuid
        //    ReadT_SM_AND_FillTable_Handle_AddedCash();
            
        //    // ADDED CASH WAS ADDED ON The initial one 

        //    int I = 0;

        //    while (I <= (DataTable_SM.Rows.Count - 1))
        //    {

        //        //
        //        // This table contains all Superviosr mode Actions by ATM
        //        // Here we are creating the necessary records for the SM 
        //        //
        //        string WAtmNo = (string)DataTable_SM.Rows[I]["AtmNo"];
        //        int Wfuid = (int)DataTable_SM.Rows[I]["fuid"];
        //        int cashaddtype1 = (int)DataTable_SM.Rows[I]["cashaddtype1"];
        //        int cashaddtype2 = (int)DataTable_SM.Rows[I]["cashaddtype2"];
        //        int cashaddtype3 = (int)DataTable_SM.Rows[I]["cashaddtype3"];
        //        int cashaddtype4 = (int)DataTable_SM.Rows[I]["cashaddtype4"];

        //        //
        //        Update_SM_Record_WithAddedCash(Wfuid, cashaddtype1
        //                  , cashaddtype2, cashaddtype3, cashaddtype4); 

        //        // 
        //        //
        //        I = I + 1; 
        //    }

        //}

        // 
        // UPDATE SM with added cash 
        //
 //       public void Update_SM_Record_WithAddedCash(int Infuid, int Incashaddtype1
 //                         , int Incashaddtype2, int Incashaddtype3, int Incashaddtype4)
          
 //       {
 //           ErrorFound = false;
 //           ErrorOutput = "";

 //           int Counter = 0;

 //           string SQLCmd =
 //" UPDATE[ATM_MT_Journals_AUDI].[dbo].[PANICOS_SM_Table] "
 // + " SET "
 //    + "   [cashaddtype1]= @cashaddtype1 " // YES 
 //     + "   ,[cashaddtype2]= @cashaddtype2 "
 //   + "      ,[cashaddtype3]= @cashaddtype3 "
 //     + "     ,[cashaddtype4]= @cashaddtype4 "
 //     + " WHERE fuid = @fuid and AdditionalCash = 'N' "
 //     + "  ";

 //           using (SqlConnection conn = new SqlConnection(connectionString))
 //               try
 //               {
 //                   conn.Open();
 //                   using (SqlCommand cmd =
 //                       new SqlCommand(SQLCmd, conn))
 //                   {
                        
 //                       cmd.Parameters.AddWithValue("@fuid", Infuid);
                      
 //                       cmd.Parameters.AddWithValue("@cashaddtype1", Incashaddtype1);
 //                       cmd.Parameters.AddWithValue("@cashaddtype2", Incashaddtype2);
 //                       cmd.Parameters.AddWithValue("@cashaddtype3", Incashaddtype3);
 //                       cmd.Parameters.AddWithValue("@cashaddtype4", Incashaddtype4);

 //                       Counter = cmd.ExecuteNonQuery();
 //                   }
 //                   // Close conn
 //                   conn.Close();
 //               }
 //               catch (Exception ex)
 //               {
 //                   conn.Close();

 //                   CatchDetails(ex, "", 0);
 //               }

 //       }

        // Insert Record when Journal is missing
        public void InsertToPANICOS_SM_TableForNewCycle()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO " + SM_Table
                    + "([AtmNo], [FlagValid],  "
                    + " [AdditionalCash], [BANK], [fuid],"
                     + " [SM_dateTime_Start], [SM_dateTime_Finish], "
                     + " [SM_LAST_CLEARED], [LoadedAtRMCycle], "
                     + " [txtline], [previous_Repl_trace], "
                     + " [after_Repl_trace], "
                     + " [ATM_total1],  "
                       + " [ATM_total2],  "
                         + " [ATM_total3],  "
                           + " [ATM_total4],  "

                       + " [ATM_Dispensed1],  "
                       + " [ATM_Dispensed2],  "
                       + " [ATM_Dispensed3],  "
                       + " [ATM_Dispensed4],  "

                       + " [ATM_Remaining1],  "
                       + " [ATM_Remaining2],  "
                       + " [ATM_Remaining3],  "
                       + " [ATM_Remaining4],  "

                         + " [ATM_Rejected1],  "
                         + " [ATM_Rejected2],  "
                         + " [ATM_Rejected3],  "
                         + " [ATM_Rejected4],  "

                           + " [ATM_cassette1],  "
                           + " [ATM_cassette2],  "
                           + " [ATM_cassette3],  "
                           + " [ATM_cassette4],  "

                              + " [cashaddtype1],  "
                           + " [cashaddtype2],  "
                           + " [cashaddtype3],  "
                           + " [cashaddtype4]  "

                     + " )" 
                    + " VALUES (@AtmNo, @FlagValid ,"
                    + "@AdditionalCash, @BANK,@fuid, "
                    + " @SM_dateTime_Start, @SM_dateTime_Finish,"
                     + " @SM_LAST_CLEARED, @LoadedAtRMCycle,"
                     + " @txtline, @previous_Repl_trace, "
                     + " @after_Repl_trace, "
                     + " @ATM_total1,  "
                       + " @ATM_total2,  "
                         + " @ATM_total3,  "
                           + " @ATM_total4,  "

                       + " @ATM_Dispensed1,  "
                       + " @ATM_Dispensed2,  "
                       + " @ATM_Dispensed3,  "
                       + " @ATM_Dispensed4,  "

                       + " @ATM_Remaining1,  "
                       + " @ATM_Remaining2,  "
                       + " @ATM_Remaining3,  "
                       + " @ATM_Remaining4,  "

                         + " @ATM_Rejected1,  "
                         + " @ATM_Rejected2,  "
                         + " @ATM_Rejected3,  "
                         + " @ATM_Rejected4,  "

                           + " @ATM_cassette1,  "
                           + " @ATM_cassette2,  "
                           + " @ATM_cassette3,  "
                           + " @ATM_cassette4, "

                             + " @cashaddtype1,  "
                           + " @cashaddtype2,  "
                           + " @cashaddtype3,  "
                           + " @cashaddtype4 "

                     + " )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
               

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@FlagValid", FlagValid);

                        cmd.Parameters.AddWithValue("@AdditionalCash", AdditionalCash);
                        cmd.Parameters.AddWithValue("@Bank", BANK);
                        cmd.Parameters.AddWithValue("@fuid", Fuid);

                        cmd.Parameters.AddWithValue("@SM_dateTime_Start", SM_dateTime_Start);
                        cmd.Parameters.AddWithValue("@SM_dateTime_Finish", SM_dateTime_Finish);

                        cmd.Parameters.AddWithValue("@SM_LAST_CLEARED", SM_LAST_CLEARED);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);

                        cmd.Parameters.AddWithValue("@txtline", txtline);
                        cmd.Parameters.AddWithValue("@previous_Repl_trace", previous_Repl_trace);
                        cmd.Parameters.AddWithValue("@after_Repl_trace", after_Repl_trace);

                        cmd.Parameters.AddWithValue("@ATM_total1", ATM_total1);
                        cmd.Parameters.AddWithValue("@ATM_total2", ATM_total2);
                        cmd.Parameters.AddWithValue("@ATM_total3", ATM_total3);
                        cmd.Parameters.AddWithValue("@ATM_total4", ATM_total4);

                        cmd.Parameters.AddWithValue("@ATM_Dispensed1", ATM_Dispensed1);
                        cmd.Parameters.AddWithValue("@ATM_Dispensed2", ATM_Dispensed2);
                        cmd.Parameters.AddWithValue("@ATM_Dispensed3", ATM_Dispensed3);
                        cmd.Parameters.AddWithValue("@ATM_Dispensed4", ATM_Dispensed4);

                        cmd.Parameters.AddWithValue("@ATM_Remaining1", ATM_Remaining1);
                        cmd.Parameters.AddWithValue("@ATM_Remaining2", ATM_Remaining2);
                        cmd.Parameters.AddWithValue("@ATM_Remaining3", ATM_Remaining3);
                        cmd.Parameters.AddWithValue("@ATM_Remaining4", ATM_Remaining4);

                        cmd.Parameters.AddWithValue("@ATM_Rejected1", ATM_Rejected1);
                        cmd.Parameters.AddWithValue("@ATM_Rejected2", ATM_Rejected2);
                        cmd.Parameters.AddWithValue("@ATM_Rejected3", ATM_Rejected3);
                        cmd.Parameters.AddWithValue("@ATM_Rejected4", ATM_Rejected4);

                        cmd.Parameters.AddWithValue("@ATM_cassette1", ATM_cassette1);
                        cmd.Parameters.AddWithValue("@ATM_cassette2", ATM_cassette2);
                        cmd.Parameters.AddWithValue("@ATM_cassette3", ATM_cassette3);
                        cmd.Parameters.AddWithValue("@ATM_cassette4", ATM_cassette4);

                        cmd.Parameters.AddWithValue("@cashaddtype1", cashaddtype1);
                        cmd.Parameters.AddWithValue("@cashaddtype2", cashaddtype2);
                        cmd.Parameters.AddWithValue("@cashaddtype3", cashaddtype3);
                        cmd.Parameters.AddWithValue("@cashaddtype4", cashaddtype4);

                        //cmd.Parameters.AddWithValue("@Operator", Operator);

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
        // Catch Details 
        //
        private static void CatchDetails(Exception ex, string InAtmNo, int InFuid)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : " + InAtmNo);
            WParameters.Append("Fuid ..:" + InFuid.ToString());
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

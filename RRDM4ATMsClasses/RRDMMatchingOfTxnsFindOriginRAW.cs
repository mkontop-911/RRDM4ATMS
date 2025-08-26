using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//using System.Linq;

namespace RRDM4ATMs
{
    public class RRDMMatchingOfTxnsFindOriginRAW : Logger
    {
        public RRDMMatchingOfTxnsFindOriginRAW() : base() { }

        public int SeqNo;
        public string Origin;
        public string OriginFileName;
        public int OriginalRecordId; // Combination of OriginFileName and OriginalRecordId gives unique record

        // Special For POS

        public string AuthorisationId;
        public string MerchantId;
        public string MerchantNm;
        public string TranDescription; 

        // Define the data tables 
        public DataTable TableJournalDetails = new DataTable();
        public DataTable TableDetails_Physical = new DataTable();
        public DataTable TableDetails_RAW = new DataTable();
        //public DataTable TableICBSDetails = new DataTable();

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        string WSignedId;
        string WOperator;

        //
        // READ the Fields of the Switch File 
        //

        public string TermID;
        public DateTime Transactiondate;
        public string MessageType;
        public string TransactionType;
        public string Char_Amount;
        public decimal Amount;
        public decimal AmtFileBToFileC;
        public string ResponseCode;

        int WTrace;
        string WRRN; 

        bool ReturnRecordFoundInUniversal; 

        // This returns Header Values of RAW Record 

        public bool FindRawRecordFromMasterRecord(string InOperator, string InFileID, int InMatchingAtRMCycle,
            DateTime InTransDate,                                       
            string InTerminalId, int InTrace, string InRRN, int InMode)
        {
            WTrace = InTrace;
            WRRN = InRRN; 
            ReturnRecordFoundInUniversal = false;
            try
            {
                //
                // IF IN MODE = 2 read POS 
                //
                RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

                RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables();

                Msf.ReadReconcSourceFilesByFileId(InFileID);

                string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "]";
                string PhysicalNameRAW = ""; 
                if (InOperator != "BCAIEGCX")
                {
                    PhysicalNameRAW = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "_RAW]";
                }
              
                // Find Origin Record 

                string WSelectionCriteria = " WHERE TerminalId ='" + InTerminalId + "'"
                                              + " AND  ProcessedAtRMCycle =" + InMatchingAtRMCycle
                                              + " AND (TraceNo =" + InTrace + " OR RRNumber =" + InRRN + ")";

                Mgt.ReadTransSpecificFromSpecificWorking(WSelectionCriteria, PhysicalName);

                if (Mgt.RecordFound == true)
                {
                    //// This returns Header Values of RAW Record 

                    //OriginFileName = Mgt.OriginFileName;
                    //OriginalRecordId = Mgt.OriginalRecordId;

                    ReturnRecordFoundInUniversal = true;
                }
                else
                {
                    ReturnRecordFoundInUniversal = false;
                }

                // NAME OF RAW TABLE 
                //

                if (InMode == 1) // This fills the Table by Trace, Terminal and Date (includes Succesfull and not) 
                {
                    FillTablesProcessForOtherSourceFile_RAW(InOperator,
                                   PhysicalNameRAW, InTransDate, InTerminalId, WTrace);
                }

                if (InMode == 2) // GIVES THE DETAILS FOR Merchant (Succesful or not )
                {
                    ReadTransSpecificRAW_POS(InOperator,
                                                PhysicalNameRAW, InTransDate, InTerminalId, InTrace);
                }
            }
            catch (Exception ex)
            {
               
                CatchDetails(ex);
            }
           
            return ReturnRecordFoundInUniversal; 
        }

        //
        // READ RAW FOR POINT OF SALE (succesful and not successful)
        //
        //  string WorkingTableName;
        public void ReadTransSpecificRAW_POS(string InOperator,
                                              string InFileId,
                                  DateTime InTransDate, string InTerminalIdentification, int InTrace)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
                " SELECT * "
                + " FROM " + InFileId
                + " WHERE TransDate = @TransDate  AND TerminalIdentification = @TerminalIdentification AND TransTraceNumber =@TransTraceNumber ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 
                     
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        cmd.Parameters.AddWithValue("@TerminalIdentification", InTerminalIdentification);
                        cmd.Parameters.AddWithValue("@TransTraceNumber", InTrace);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            AuthorisationId = (string)rdr["AuthorisationId"];
                            MerchantId = (string)rdr["MerchantId"];
                            MerchantNm = (string)rdr["MerchantNm"];
                            TranDescription = (string)rdr["TranDescription"];
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

        // Read From Switch File
        public void ReadFieldsFromSwitchFile(string InOperator, string InSignedId,
                                       string InAtmNo,
                                       int InTrace, DateTime InTrandate)
        {
            //RecordFound = false;
            //ErrorFound = false;
            //ErrorOutput = "";

           
        }

        public string terminalID;
      //  public DateTime Transactiondate;
        public string cardnumber;
        public string AccountNumber;
        public string transactioncode;
        // public decimal Amount;
        public string fromaccount;
        public string Toaccount;
        public string IsoProcode; 

        // Read From Banking File
        public void ReadFieldsFromBankingFile(string InOperator, string InSignedId,
                                       string InAtmNo,
                                       int InTraceNumber, DateTime InTrandate)
        {
            //RecordFound = false;
            //ErrorFound = false;
            //ErrorOutput = "";

       
        }

        // For this Category Find Groups and Call to Create Working Files
        // For each atm in each group 
        public void FillTablesProcessForJournal(string InOperator, string InSignedId,
                                       string InAtmNo,
                                       int InTraceNumber, DateTime InTrandate )
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            // Fill Up a Table Journal 

            TableJournalDetails = new DataTable();
            TableJournalDetails.Clear();
            
            if (WOperator == "CRBAGRAA")
            {
              SqlString =
                  " SELECT * "
                  + " FROM [ATMS_Journals].[dbo].[tblHstAtmTxns] "
                  + " WHERE AtmNo = @AtmNo AND TraceNumber = @TraceNumber AND Trandate = @Trandate ";
            }
            else
            {

                SqlString =
                  " SELECT * "
                  + " FROM [ATM_MT_Journals].[dbo].[tblHstAtmTxns] "
                  + " WHERE AtmNo = @AtmNo AND TraceNumber = @TraceNumber AND Trandate = @Trandate ";
            }

            //Testing 

            //+ " where TraceNumber = 88450 and AtmNo = 'NB0553C1' "; 

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TraceNumber", InTraceNumber);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Trandate", InTrandate);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableJournalDetails);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }

        // For this Category Find Groups and Call to Create Working Files
        // For each atm in each group 
        // (WOperator, WSignedId, (PhysicalNameA + "_RAW", OriginFileName, OriginalRecordId); 
        public void FillTablesProcessForOtherSourceFile_RAW(string InOperator, 
                                                                     string InFileId, 
                                                                     DateTime InTransDate, string InTerminalIdentification, int InTrace)
                                       
        {
            WOperator = InOperator;
            

            // Fill Up a TableSwitchDetails

            TableDetails_RAW = new DataTable();
            TableDetails_RAW.Clear();

            SqlString =
               " SELECT * "
               + " FROM " + InFileId
               + " WHERE TransDate = @TransDate  AND TerminalIdentification = @TerminalIdentification AND TransTraceNumber =@TransTraceNumber "; 

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDate", InTransDate);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalIdentification", InTerminalIdentification);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransTraceNumber", InTrace);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableDetails_RAW);

                        // Close conn
                        conn.Close();


                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }           

        }

      
        // Pending from physical 
        public void FillTableOfPending(string InOperator, string InMatchingCateg, 
                                                          string InFileId )

        {
            WOperator = InOperator;


            // Fill Up a TableSwitchDetails

            TableDetails_Physical = new DataTable();
            TableDetails_Physical.Clear();

            SqlString =
               " SELECT * "
               + " FROM " + InFileId
               + " WHERE Operator = @Operator AND MatchingCateg = @MatchingCateg AND Processed = 0 ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                     

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableDetails_Physical);

                        // Close conn
                        conn.Close();


                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

        }

        // For this Category Find Groups and Call to Create Working Files
        // For each atm in each group 
        // (WOperator, WSignedId, (PhysicalNameA + "_RAW", OriginFileName, OriginalRecordId); 
        //public void FillTablesProcessForOtherSourceFile(string InOperator,
        //                                                             string InFileId,
        //                                                             string InOriginFileName, string InTerminalIdentification, int InTraceRRN)

        //{
        //    WOperator = InOperator;


        //    // Fill Up a TableSwitchDetails

        //    TableDetails = new DataTable();
        //    TableDetails.Clear();

        //    SqlString =
        //       " SELECT * "
        //       + " FROM " + InFileId
        //       + " WHERE OriginFileName = @OriginFileName  AND TerminalIdentification = @TerminalIdentification AND TransTraceNumber =@TransTraceNumber ";

        //    using (SqlConnection conn =
        //     new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();

        //            //Create an Sql Adapter that holds the connection and the command
        //            using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
        //            {
        //                sqlAdapt.SelectCommand.Parameters.AddWithValue("@OriginFileName", InOriginFileName);
        //                sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalIdentification", InTerminalIdentification);
        //                sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransTraceNumber", InTraceRRN);

        //                //Create a datatable that will be filled with the data retrieved from the command

        //                sqlAdapt.Fill(TableDetails);

        //                // Close conn
        //                conn.Close();


        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            CatchDetails(ex);
        //        }

        //}



    }
}

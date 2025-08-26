using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//using System.Linq;

namespace RRDM4ATMs
{
    public class RRDMJournalAudi_BDC : Logger
    {
        public RRDMJournalAudi_BDC() : base() { }

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
        public DataTable TableJournalPresented = new DataTable();
        public DataTable TableJournalDeposits = new DataTable();
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

        bool ReturnRecordFoundInUniversal;


        //
        // READ RAW FOR POINT OF SALE (succesful and not successful)
        //
        //  string WorkingTableName;
        //public void ReadTransSpecific_POS(string InOperator,
        //                                      string InFileId,
        //                          DateTime InTransDate, string InTerminalIdentification, string InRRNumber)

        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    SqlString =
        //        " SELECT * "
        //        + " FROM " + InFileId
        //        + " WHERE TransDate = @TransDate  AND TerminalIdentification = @TerminalIdentification AND RRNumber =@TransTraceNumber ";


        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {

        //                // Read table 

        //                cmd.Parameters.AddWithValue("@TransDate", InTransDate);
        //                cmd.Parameters.AddWithValue("@TerminalIdentification", InTerminalIdentification);
        //                cmd.Parameters.AddWithValue("@TransTraceNumber", InRRNumber);

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    SeqNo = (int)rdr["SeqNo"];

        //                    AuthorisationId = (string)rdr["AuthorisationId"];
        //                    MerchantId = (string)rdr["MerchantId"];
        //                    MerchantNm = (string)rdr["MerchantNm"];
        //                    TranDescription = (string)rdr["TranDescription"];
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
        public string WResponseCode;
        public void FillTablesProcessForJournal(string InOperator, string InSignedId,
                                       string InAtmNo,
                                       int InTraceNumber, DateTime InTrandate, Decimal InCAmount)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WResponseCode = "";

            // Fill Up a Table Journal 

            TableJournalDetails = new DataTable();
            TableJournalDetails.Clear();


            SqlString =
                " SELECT * "
                + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] "
                + " WHERE AtmNo = @AtmNo "
                + " AND TraceNumber = @TraceNumber "
                + " AND Trandate = @Trandate "
                + "  AND CAmount = @CAmount  "
                + " ORDER BY Source";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TraceNumber", InTraceNumber); // Multiply By zero
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Trandate", InTrandate);
                        //if (WOperator == "BCAIEGCX")
                        //{
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAmount", InCAmount);
                        //}
                        sqlAdapt.SelectCommand.CommandTimeout = 350;
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

            int Count = 0;

            int I = 0;
            // 
            while (I <= (TableJournalDetails.Rows.Count - 1))
            {
                int WSeqNo = (int)TableJournalDetails.Rows[I]["SeqNo"];
                SqlString =
                     " SELECT * "
                     + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] "
                     + " WHERE SeqNo = @SeqNo "
                     + "";

                using (SqlConnection conn =
                         new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {

                            // Read table 

                            cmd.Parameters.AddWithValue("@SeqNo", WSeqNo);

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                RecordFound = true;

                                Count = Count + 1;

                                if (Count > 1) WResponseCode = WResponseCode + "..AND.. ";

                                string WResponseCodetemp = (string)rdr["Source"];

                                WResponseCode = WResponseCode + WResponseCodetemp;

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
                I = I + 1;
            }


        }

        public void FillTablesProcessForJournal_ROM( string InAtmNo,string UTRNNO)
        {
           // WOperator = InOperator;
           // WSignedId = InSignedId;

            WResponseCode = "";

            // Fill Up a Table Journal 

            TableJournalDetails = new DataTable();
            TableJournalDetails.Clear();


            SqlString =
                " SELECT * "
                + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] "
                + " WHERE AtmNo = @AtmNo "
                + " AND UTRNNO = @UTRNNO "
                + " ORDER BY Source";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@UTRNNO", UTRNNO); // Multiply By zero
                     
                        //}
                        sqlAdapt.SelectCommand.CommandTimeout = 350;
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

            int Count = 0;

            int I = 0;
            // 
            while (I <= (TableJournalDetails.Rows.Count - 1))
            {
                int WSeqNo = (int)TableJournalDetails.Rows[I]["SeqNo"];
                SqlString =
                     " SELECT * "
                     + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] "
                     + " WHERE SeqNo = @SeqNo "
                     + "";

                using (SqlConnection conn =
                         new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {

                            // Read table 

                            cmd.Parameters.AddWithValue("@SeqNo", WSeqNo);

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                RecordFound = true;

                                Count = Count + 1;

                                if (Count > 1) WResponseCode = WResponseCode + "..AND.. ";

                                string WResponseCodetemp = (string)rdr["Source"];

                                WResponseCode = WResponseCode + WResponseCodetemp;

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
                I = I + 1;
            }


        }

        public void FillTablesProcessForJournal_HST(string InOperator, string InSignedId,
                                    string InAtmNo,
                                    int InTraceNumber, DateTime InTrandate, Decimal InCAmount)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WResponseCode = "";

            // Fill Up a Table Journal 

            TableJournalDetails = new DataTable();
            TableJournalDetails.Clear();


            SqlString =
                " SELECT * "
                + " FROM [ATM_MT_Journals_AUDI_HST].[dbo].[tblHstAtmTxns] "
                + " WHERE AtmNo = @AtmNo "
                + " AND TraceNumber = @TraceNumber "
                + " AND Trandate = @Trandate "
                + "  AND CAmount = @CAmount  "
                + " ORDER BY Source";

            //}

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
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TraceNumber", InTraceNumber); // Multiply By zero
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Trandate", InTrandate);
                        //if (WOperator == "BCAIEGCX")
                        //{
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAmount", InCAmount);
                        sqlAdapt.SelectCommand.CommandTimeout = 350;
                        //}

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

            int Count = 0;

            int I = 0;
            // 
            while (I <= (TableJournalDetails.Rows.Count - 1))
            {
                int WSeqNo = (int)TableJournalDetails.Rows[I]["SeqNo"];
                SqlString =
                     " SELECT * "
                     + " FROM [ATM_MT_Journals_AUDI_HST].[dbo].[tblHstAtmTxns] "
                     + " WHERE SeqNo = @SeqNo "
                     + "";

                using (SqlConnection conn =
                         new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {

                            // Read table 

                            cmd.Parameters.AddWithValue("@SeqNo", WSeqNo);

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                RecordFound = true;

                                Count = Count + 1;

                                if (Count > 1) WResponseCode = WResponseCode + "..AND.. ";

                                string WResponseCodetemp = (string)rdr["Source"];

                                WResponseCode = WResponseCode + WResponseCodetemp;

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
                I = I + 1;
            }
        }

        public int Tot_Type1;
        public int Tot_Type2;
        public int Tot_Type3;
        public int Tot_Type4;

        public void FillTablePresentedFromJournal(string InOperator, string InSignedId,
                                     string InSelectionCriteria, 
                                     DateTime InDtFrom, DateTime InDtTo)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WResponseCode = "";

            // Fill Up a Table Journal 

            TableJournalPresented = new DataTable();
            TableJournalPresented.Clear();

            SqlString =
                " SELECT  [SeqNo]  ,[AtmNo] ,[TraceNumber]/10 As TraceNo ,[Type1]  ,[Type2]   ,[Type3]  ,[Type4]"
                + "  ,Cast([CAmount] As decimal(15, 2)) As Amount "
                + " ,[TranDesc]  ,[Trandatetime]  ,[FuID], [PresenterError] "
                + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] "
                +  InSelectionCriteria
                + " AND Trandatetime >= @DtFrom AND Trandatetime <= @DtTo "
                + " ORDER BY Trandatetime ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DtFrom", InDtFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DtTo", InDtTo); // 
                        
                        sqlAdapt.SelectCommand.CommandTimeout = 350;
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableJournalPresented);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
         //   ISNULL([RETRIEVAL_REF_NO] , '')
            SqlString =
                    " SELECT  ISNULL(SUM([Type1]),0) As Tot_Type1  ,ISNULL(SUM([Type2]),0) As Tot_Type2   ,ISNULL(SUM([Type3]),0) As Tot_Type3 ,ISNULL(SUM([Type4]),0) As Tot_Type4 "
                + "  "
                + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] "
                + InSelectionCriteria
                + " AND Trandatetime >= @DtFrom AND Trandatetime <= @DtTo "; 
                //+ " ORDER BY Trandatetime ";

            using (SqlConnection conn =
                     new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 
                        cmd.Parameters.AddWithValue("@DtFrom", InDtFrom);
                        cmd.Parameters.AddWithValue("@DtTo", InDtTo);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            Tot_Type1 = (int)rdr["Tot_Type1"];
                            Tot_Type2 = (int)rdr["Tot_Type2"];
                            Tot_Type3 = (int)rdr["Tot_Type3"];
                            Tot_Type4 = (int)rdr["Tot_Type4"];

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

            //int Count = 0;

            //int I = 0;
            //// 
            //while (I <= (TableJournalDetails.Rows.Count - 1))
            //{
            //    int WSeqNo = (int)TableJournalDetails.Rows[I]["SeqNo"];
            //    SqlString =
            //         " SELECT * "
            //         + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] "
            //         + " WHERE SeqNo = @SeqNo "
            //         + "";

            //    using (SqlConnection conn =
            //             new SqlConnection(connectionString))
            //        try
            //        {
            //            conn.Open();
            //            using (SqlCommand cmd =
            //                new SqlCommand(SqlString, conn))
            //            {

            //                // Read table 

            //                cmd.Parameters.AddWithValue("@SeqNo", WSeqNo);

            //                SqlDataReader rdr = cmd.ExecuteReader();

            //                while (rdr.Read())
            //                {
            //                    RecordFound = true;

            //                    Count = Count + 1;

            //                    if (Count > 1) WResponseCode = WResponseCode + "..AND.. ";

            //                    string WResponseCodetemp = (string)rdr["Source"];

            //                    WResponseCode = WResponseCode + WResponseCodetemp;

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
            //    I = I + 1;
            //}


        }

        public int Tot_DepAmt; 
        public void FillTableDepositsFromJournalByDenomination(string InOperator, string InSignedId,
                                  string InSelectionCriteria
                                  )
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WResponseCode = "";

            // Fill Up a Table Journal 

            TableJournalDeposits = new DataTable();
            TableJournalDeposits.Clear();

            SqlString =
                " SELECT  [SeqNo], AtmNo  , 'DEPOSIT' as Deposit  ,[Currency]  ,[FaceValue]   ,[CASSETTE]   ,[RETRACT], RECYCLED  "
                //+ "  ,Cast([CAmount] As decimal(15, 2)) As Amount "
                //+ " ,[TranDesc]  ,[Trandatetime]  ,[FuID], [PresenterError] "
                + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                + InSelectionCriteria
                //+ " AND Trandatetime >= @DtFrom AND Trandatetime <= @DtTo "
                + " ORDER BY Currency Desc, FaceValue ASC ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                       // sqlAdapt.SelectCommand.Parameters.AddWithValue("@DtFrom", InDtFrom);
                       // sqlAdapt.SelectCommand.Parameters.AddWithValue("@DtTo", InDtTo); // 

                        sqlAdapt.SelectCommand.CommandTimeout = 350;
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableJournalDeposits);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            SqlString =
                    " SELECT ISNULL(SUM(FaceValue*CASSETTE),0) As Tot_DepAmt "
                + "  "
                + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                + InSelectionCriteria
                + " AND Currency = 'EGP' "
               // + " AND Trandatetime >= @DtFrom AND Trandatetime <= @DtTo "
                ;
            //+ " ORDER BY Trandatetime ";

            using (SqlConnection conn =
                     new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 
                        //cmd.Parameters.AddWithValue("@DtFrom", InDtFrom);
                        //cmd.Parameters.AddWithValue("@DtTo", InDtTo);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            Tot_DepAmt = (int)rdr["Tot_DepAmt"];
                        

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

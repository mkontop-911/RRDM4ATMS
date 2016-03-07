using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;
namespace RRDM4ATMs
{
    class RRDMKontoTempJournal
    {
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMNotesBalances Na = new RRDMNotesBalances();
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMNewSession Sa = new RRDMNewSession(); // NEW SESSION CLASS 
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMCashInOut Ct = new RRDMCashInOut();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        //    E_JournalTxtClass Ejc = new E_JournalTxtClass(); 

        RRDMErrorsFromKontoToMatchingClass Em = new RRDMErrorsFromKontoToMatchingClass();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMReplActionsClass Ra = new RRDMReplActionsClass();


        DateTime NullPastDate = new DateTime(1900, 01, 01); 
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        int WSaveSesNo;

        string WCitId;
        string WOperator;

        int WLastTraceNo;
        int WSesNo;

        string WSignedId;
        int WSignRecordNo;
        string WBankId;

        string WAtmNo;
        int WMode;


        public void ReadAndCreateTables(string InSignedId, int InSignRecordNo, string InBankId,
            string InAtmNo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WBankId = InBankId;

            WAtmNo = InAtmNo;
            WMode = InMode;

            // Read transactions from EJournal file
            // 1) Update InPool Trans
            // 2) Update Captured Cards
            // 3) Update cassettes
            // 4) Update Fist Last Trace or maybe not? Yes it is better here than in replenishment workflow 


            int I = 0;

            DateTime SmStartDtTime;
            DateTime SmEndDtTime;

            DateTime TRanDate;

            int TraceNo;

            int fuid;
            string trandesc;

            string CardNo;
            string AccNo;

            int StartTrxn;
            int EndTrxn;

            int PreStartTrxn = 0;
            int PreEndTrxn = 0;
            int PreJournalTraceNo = 0;

            int EJournalTraceNo;

            int Reconciled;
            int TransactionType;

            string CurrDesc;
            decimal TranAmount;

            int CommissionCode;
            decimal CommissionAmount;

            string CardCaptured;

            string PresError;

            string SuspectDesc;

            //        Int32 LastDigit;
            //     int SmTrace; 

            string Result;

            int Cassette1;
            int Cassette2;
            int Cassette3;
            int Cassette4;

            int CurrentSessionNo;

            DateTime NullDate = new DateTime(1900, 01, 01);

            Ac.ReadAtm(InAtmNo); // Read Information for ATM 

            WOperator = Ac.Operator;

            WCitId = Ac.CitId;

            bool KontoReady = true;

            if (KontoReady == true & WAtmNo == "AB104" & WSignedId == "501") // DO THIS FOR TESTING USER 
            {
                //TEST

                // STEP A . Copy from ATM EJournal from ATM directory to Konto Directory

                int WAction = 1;

                string WIpAddress = "123432";

                AtmEjournalCopyToDirectory(WBankId, WAtmNo, WIpAddress, WAction); // Copy ATM file

                //TEST 
                // STEP B . Procedures reads EJournal from Konto directory and parse it. 
                // CALL Test procedures
                string JournalType = "NCR01";

                //TEST
                // 1. READ First Journal - KONTO 1
                string PathName = @"C:\ATMBackUps\KONTO\EJDATA.LOG.10.1.86.16.20140213.030935.1.LOG";
                StoreProcKontoEJournal(WBankId, WAtmNo, JournalType, PathName);

                // 2.... Create File for host - KONTO 2

                // 3.... Create Error Conditions ( One double at Hosst and one missing) - KONTO 3 

                // 4.... Create Host file with compltete trace number KONTO 4

                // 5.....Check results 
                //TESTING
                //MessageBox.Show("Check results for HOST FILE CREATION"); 

                // 6...... Matching Files 
                // CALL PROCEDURE FOR MATCHING THE FILES - HOST AND ATMS - KONTO 5 
                StoreProcKontoMatchingOfFiles(WBankId, WAtmNo);

                //TESTING
                //MessageBox.Show("Check Errors File Status, Host last trace numbers and Errors created"); 

                // 7...... Continue with the rest  

                //TEST
                PathName = @"C:\ATMBackUps\KONTO\EJDATA.LOG.10.1.86.16.20140213.030935.2.LOG";
                StoreProcKontoEJournal(WBankId, WAtmNo, JournalType, PathName);
                //TEST
                PathName = @"C:\ATMBackUps\KONTO\EJDATA.LOG.10.1.86.16.20140213.030935.3.LOG";
                StoreProcKontoEJournal(WBankId, WAtmNo, JournalType, PathName);



                KontoReady = false;

                //MessageBox.Show("YOU are Testing User 501. NOW RUN MANUALLY THE PROCEDURE START"); 
            }

            bool ReplCyclefound = false;

            // STEP C : Update In Pool with transactions 

            // FIND LATEST REPLCYCLE AND Trace Number 

            Ta.FindNextReplCycleId(WAtmNo);

            if (Ta.RecordFound == true)
            {
                ReplCyclefound = true;
                WSesNo = Ta.LastNo;
                WLastTraceNo = Ta.LastTraceNo;
                //TEST
                // MessageBox.Show("MSG:998 Start Trace Number: " + WLastTraceNo.ToString());
            }
            else
            {
                // THIS IS THE FIRST TIME ATM WAS INSTALLED IN GAS 
                WSesNo = 0;
                WLastTraceNo = 0;

                RecordFound = false;
                ErrorFound = false;
                ErrorOutput = "";

                bool First = true;

                string SqlString1 = "SELECT atmno, TraceNumber, ISNULL(trandesc, '') AS trandesc,"
                                    + "ISNULL(Result, '') AS Result,TransactionType"
                                    + " FROM [ATMS_Journals].[dbo].[tblHstAtmTxns] "
                                    + " WHERE atmno = @atmno AND trandesc = 'SM-CASH ADDED'"
                                     + " order by TraceNumber";

                using (SqlConnection conn =
                                       new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString1, conn))
                        {
                            cmd.Parameters.AddWithValue("@atmno", WAtmNo);
                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                RecordFound = true;

                                if (First == true)
                                {
                                    TraceNo = Convert.ToInt32(rdr["TraceNumber"]);
                                    WLastTraceNo = TraceNo - 1;
                                    First = false;
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
                        ErrorFound = true;
                        ErrorOutput = "An error occured in E-JournalProcess............. " + ex.Message;
                    }

                if (RecordFound == false)
                {
                    // CASE OF NEW ATM => Create the FIRST  Repl Cycle record 
                    // Reple Cycle not found and not a new ATM

                    ErrorFound = true;
                    ErrorOutput = "No Repl Cycle record in System " + "and  NO Cash Added Record";

                    return;

                }
            }

            // 
            // AT this point UPDATED KONTO Files are ready to be read by GAS  
            // 

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString2 = "SELECT BankID, atmno, TraceNumber, ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
                + "ISNULL(currency, '') AS currency, CAmount,ISNULL(cardnum, '') AS cardnum,"
                + "ISNULL(starttxn, 0) AS starttxn,ISNULL(endtxn, 0) AS endtxn,ISNULL(TRanDate, '') AS TRanDate,"
                + "ISNULL(trantime, '') AS trantime,ISNULL(acct1, '') AS acct1,ISNULL(Result, '') AS Result,"
                + "ISNULL(CardCaptured, '') AS CardCaptured, ISNULL(CardCapturedMES, '') AS CardCapturedMes,"
                + "ISNULL(PresenterError, '') AS PresError, ISNULL(SuspectDesc, '') AS SuspectDesc,ISNULL(SuspectNotes,'') AS SuspectNotes,"
                + "Type1, type2, type3, type4, Reconciled, CardTargetSystem,  TransactionType,"
                 + "CommissionCode, CommissionAmount"
                    + " FROM [ATMS_Journals].[dbo].[tblHstAtmTxns] "
                    + " WHERE atmno = @atmno AND TraceNumber>@LastTraceNo"
                    + " ORDER by TraceNumber";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString2, conn))
                    {
                        cmd.Parameters.AddWithValue("@atmno", WAtmNo);
                        cmd.Parameters.AddWithValue("@LastTraceNo", WLastTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // READ BASIC INFORMATION 

                            TraceNo = (int)rdr["TraceNumber"];

                            //      LastDigit = int.Parse(TRACENUMBER.Substring(11, 1));
                            //      TraceNo = int.Parse(TRACENUMBER);   // Trace Number 

                            fuid = (int)rdr["fuid"];
                            trandesc = (string)rdr["trandesc"]; // Description 
                            if (trandesc == "ΚΑΤΑΘΕΣΗ")
                            {
                                trandesc = "DEP CHEQUES";
                            }

                            if (trandesc == "DEPOSIT")
                            {
                                trandesc = "DEP CHEQUES";
                            }
                            Result = (string)rdr["Result"];

                            CardNo = (string)rdr["cardnum"];

                            AccNo = (string)rdr["acct1"];

                            StartTrxn = (int)rdr["starttxn"];   // Start in Journal 
                            EndTrxn = (int)rdr["endtxn"];       // End  

                            // Check if this falls within the previous Trace Number 
                            if (StartTrxn == PreStartTrxn & EndTrxn == PreEndTrxn)
                            {
                                EJournalTraceNo = PreJournalTraceNo;
                            }
                            else EJournalTraceNo = TraceNo;

                            // Keep the previuos values 
                            // 
                            PreStartTrxn = StartTrxn;
                            PreEndTrxn = EndTrxn;
                            PreJournalTraceNo = EJournalTraceNo;

                            Reconciled = Convert.ToInt32(rdr["Reconciled"]);
                            TransactionType = (int)rdr["TransactionType"]; // 11 for withdrawl, .... 77 for Normal Supervisor Mode
                            // .. 78 for not normal 

                            // TRANSACTION DATE Tm 
                            TRanDate = (DateTime)rdr["TRanDate"];

                            if (TRanDate == NullDate)
                            {
                                ErrorFound = true;
                                ErrorOutput = "There is null date in Trace Number = " + TraceNo.ToString();
                                //  MessageBox.Show("There is null date in Trace Number = " + TraceNo.ToString()); 
                            }

                            TimeSpan Time = (TimeSpan)rdr["trantime"];

                            TRanDate = TRanDate.Add(Time);

                            // TRANSACTION AMOUNT
                            CurrDesc = (string)rdr["currency"];
                            TranAmount = (decimal)rdr["camount"];

                            CardCaptured = (string)rdr["CardCaptured"];

                            PresError = (string)rdr["PresError"];

                            SuspectDesc = (string)rdr["SuspectDesc"];

                            // FILL UNIVERSAL RECORD 

                            RRDMReconcMatchedUnMatchedVisaAuthorClass Rma = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 

  //                          SELECT TOP 1000 [BankID]
  //    ,[AtmNo]
  //    ,[TraceNumber]
  //     ,[CardNum]
  //         ,[Acct1]
  //    ,[TranDesc]
  //    ,[Currency]
  //    ,[CAmount]
  //     ,[Result]
  //      ,[TransactionType]
     
  //FROM [ATMS_Journals].[dbo].[tblHstAtmTxns]
  //WHERE TranDesc = 'WITHDRAWAL' AND Result = 'OK'
  //Order by CardNum

                            RRDMReconcCategories Rc = new RRDMReconcCategories();

                            Rma.Operator = (string)rdr["BankID"];
                            Rma.CardNumber = (string)rdr["cardnum"];

                            //451174******6838    
                            if (Rma.CardNumber.Substring(0, 6) == "451174" )
                            {
                                Rma.RMCateg = "EWB102";
                                
                                Rc.ReadReconcCategorybyCategId(Rma.Operator, Rma.RMCateg);
                                Rma.Origin = Rc.Origin;
                                Rma.TransTypeAtOrigin = Rc.TransTypeAtOrigin;
                                Rma.Product = Rc.Product;
                                Rma.CostCentre = Rc.CostCentre; 
                            }
                            else
                            {
                                Rma.RMCateg = "EWB103";

                                Rc.ReadReconcCategorybyCategId(Rma.Operator, Rma.RMCateg);
                                Rma.Origin = Rc.Origin;
                                Rma.TransTypeAtOrigin = Rc.TransTypeAtOrigin;
                                Rma.Product = Rc.Product;
                                Rma.CostCentre = Rc.CostCentre; 

                            }


                            Rma.OriginFileName = "[ATMS_Journals].[dbo].[tblHstAtmTxns]";
                            Rma.OriginalRecordId = (int)rdr["TraceNumber"];
                            
                            Rma.RMCycle = 2; // We think what to insert here 

                            Rma.TerminalId = (string)rdr["atmno"]; 
                            Rma.TransType = (int)rdr["TransactionType"]; // We are interested only the ones with 11 and 21  
                            Rma.TransDescr = (string)rdr["trandesc"]; 
                            
                            
                            Rma.AccNumber = (string)rdr["acct1"]; 
                            Rma.TransCurr = (string)rdr["currency"];
                            Rma.TransAmount = (decimal)rdr["camount"];


                            // TRANSACTION DATE Tm 
                            DateTime TRanDate2 = (DateTime)rdr["TRanDate"];

                            TimeSpan Time2 = (TimeSpan)rdr["trantime"];

                            TRanDate2 = TRanDate2.Add(Time2);

                            Rma.TransDate = TRanDate2; 

                            Rma.AtmTraceNo = (int)rdr["TraceNumber"];

                            Rma.RRNumber = 0; 
                     
                            Rma.ResponseCode = 0; 
                     
                            Rma.T24RefNumber = " ";

                            Rma.OpenRecord = true;

                           

                          

                            // CHECK FIRST RECORD OF JOURNAL 

                            if ((ReplCyclefound == false & trandesc == "SM-CASH ADDED") // CASE OF NEW ATM => Create the FIRST  Repl Cycle record 
                                 || (ReplCyclefound == true & trandesc == "SM-CASH ADDED")) // CASE OF Creating a New Repl Cycle 
                            {

                                // At this point create a new Repl Cycle ( NEW SESSION)
                                // with process mode = -1
                                // Set UP the WSesNo
                                // Update first trace number 

                                if (ReplCyclefound == false & trandesc == "SM-CASH ADDED") // MAKE ATM ACTIVE 
                                {
                                    Ac.ReadAtm(WAtmNo);
                                    Ac.ActiveAtm = true;
                                    Ac.UpdateAtmsBasic(WAtmNo);
                                }

                                WSaveSesNo = WSesNo;

                                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate); // The necessary records are created and the new Session No is available 

                                CurrentSessionNo = Sa.NewSessionNo;

                                WSesNo = Sa.NewSessionNo;

                                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                                Ta.FirstTraceNo = TraceNo;
                                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                                ReplCyclefound = true;

                            }
                            //---------------------------------------------------------------
                            //--------------------MOVE THIS CODE TO STORE PROCEDURES--------
                            //--------------------------------------------------------------
                            // UPDATE Tbl Ejourmal text with Repl Cycle and other fields
                            //

                            //   Ejc.ReplCycle = WSesNo;
                            /*   
                               Ejc.CardNo = CardNo;
                               Ejc.AccNo = AccNo;
                               Ejc.TranType = TransactionType;
                               Ejc.Descr = trandesc;
                               Ejc.CurNm = CurrDesc;
                               Ejc.TranAmnt = TranAmount;
                               Ejc.ErrId = 0;
                               Ejc.ErrDesc = "";
                               if (CardCaptured == "CAPTURED CARD") 
                               {
                                   Ejc.ErrId = 301;
                                   Ejc.ErrDesc = "CAPTURED CARD";
                               }
                               if (PresError == "PRESENTER ERROR")
                               {
                                   Ejc.ErrId = 55;
                                   Ejc.ErrDesc = "PRESENTER ERROR";
                               }
                               if (SuspectDesc == "SUSPECT FOUND")
                               {
                                   Ejc.ErrId = 225;
                                   Ejc.ErrDesc = "SUSPECT FOUND";
                               }

                               // Check START 
                               LastDigit = TraceNo % 10;

                               if (LastDigit == 0)
                               {
                                  SmTrace = TraceNo; 
                               }
                               else
                               {
                                   SmTrace = (TraceNo - LastDigit) + 1;
                               }

                               Ejc.Operator = WOperator; 

                               Ejc.UpdateEjTextFromEjProcess(WAtmNo, SmTrace); 

                               //-------------MOVE THE ABOVE TO STORE PROCEDURES-------//
                               //-----------------------------------------------------//
                             
                              */
                            // Deal with Captured cards



                            if (CardCaptured == "CAPTURED CARD") // Trace 10043050
                            {
                                // Insert details in Capture cards table 
                                RRDMCaptureCardsClass Cc = new RRDMCaptureCardsClass();

                                Cc.AtmNo = WAtmNo;
                                Cc.BankId = WBankId;

                                Cc.BranchId = Ac.Branch;
                                Cc.SesNo = WSesNo;
                                Cc.TraceNo = TraceNo;
                                Cc.CardNo = CardNo;
                                Cc.CaptDtTm = TRanDate;
                                Cc.CaptureCd = 12;
                                Cc.ReasonDesc = (string)rdr["CardCapturedMes"];
                                Cc.ActionDtTm = NullFutureDate;
                                Cc.CustomerNm = "";
                                Cc.ActionComments = "";
                                Cc.ActionCode = 0;

                                Cc.OpenRec = true;

                                Cc.Operator = WOperator;

                                Cc.InsertCapturedCard(WAtmNo);
                            }


                            // AT THIS POINT WE HAVE A Repl Cycle record 
                            // We continue reading and processing the Transactions (DR and CR) 

                            Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                            Ta.LastTraceNo = TraceNo;

                            if (trandesc == "WITHDRAWAL" & Result.Substring(0, 2) == "OK")
                            {
                                Ta.SesDtTimeEnd = TRanDate;  // Continuously update the SesDtTime end 
                            }
                            Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);

                            if (trandesc == "SM-TOTALS")
                            {
                                //  SessionEnd = true; // Last digit = 6 

                                // AT this point Deposits must update Notes and values
                                RRDMDepositsClass Da = new RRDMDepositsClass();

                                Da.ReadDepositsTotals(WAtmNo, WSesNo); // Find Totals 
                                Da.UpdateDepositsNaWithMachineTotals(WAtmNo, WSesNo); // Update Totals 

                                // Close current Repl Cycle and continue process 
                                // Turn status to 0 = ready for Repl Cycle Workflow 
                                Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                                Ta.ProcessMode = 0; // from -1 we have made it 0 : Repl Cycle Record is ready for Repl Cycle workfow 
                                Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);
                            }

                            if ((trandesc == "WITHDRAWAL"
                                || trandesc == "DEPOSIT_BNA" || trandesc == "DEPOSIT" || trandesc == "DEP CHEQUES")
                                & Result.Substring(0, 2) == "OK")
                            {
                                // MAP THE INPUT
                                I = I + 1;


                                CommissionCode = (int)rdr["CommissionCode"];
                                CommissionAmount = (decimal)rdr["CommissionAmount"];

                                //
                                // BUILD THE TRANSACTION 
                                //
                                Tc.OriginName = "OurATMs";
                                Tc.AtmTraceNo = TraceNo;
                                Tc.EJournalTraceNo = EJournalTraceNo;

                                Tc.AtmNo = InAtmNo;
                                Tc.SesNo = WSesNo;
                                Tc.BankId = WBankId;

                                Tc.BranchId = Ac.Branch;

                                Tc.AtmDtTime = TRanDate;
                                //Tc.HostDtTime = TRanDate;

                                Tc.SystemTarget = (int)rdr["CardTargetSystem"];

                                if (trandesc == "WITHDRAWAL")
                                {
                                    Tc.TransType = 11; // If Withdrawl 
                                    Tc.DepCount = 0;
                                }

                                if (trandesc == "DEPOSIT_BNA")
                                {
                                    Tc.TransType = 23; // Deposit Cash 
                                    Tc.DepCount = TranAmount;
                                }

                                if (trandesc == "DEPOSIT")
                                {
                                    Tc.TransType = 24; // Deposit Cheque 
                                    Tc.DepCount = TranAmount;

                                }
                                if (trandesc == "DEP CHEQUES")
                                {
                                    Tc.TransType = 25; // Deposit Envelops 
                                    Tc.DepCount = TranAmount;
                                }

                                Tc.TransDesc = trandesc;
                                Tc.CardNo = CardNo;
                                Tc.CardOrigin = 1;

                                Tc.AccNo = AccNo;

                                Tc.CurrDesc = CurrDesc;

                                Tc.TranAmount = TranAmount;
                                Tc.AuthCode = 0;
                                Tc.RefNumb = 0;
                                Tc.RemNo = 0;

                                Tc.TransMsg = "";
                                Tc.AtmMsg = "";
                                Tc.ErrNo = 0;

                                Tc.StartTrxn = StartTrxn;
                                Tc.EndTrxn = EndTrxn;

                                Tc.CommissionCode = CommissionCode;
                                Tc.CommissionAmount = CommissionAmount;

                                if (Result.Substring(0, 2) == "OK") Tc.SuccTran = true;
                                else Tc.SuccTran = false;

                                // ADD THE TRANSACTION 
                                Tc.InsertTransInPool(InAtmNo);

                                //   PresError = (string)rdr["PresError"];

                                // SuspectDesc = (string)rdr["SuspectDesc"];

                                if (PresError == "PRESENTER ERROR" || SuspectDesc == "SUSPECT FOUND")
                                {
                                    Tc.ReadTranForTrace(WAtmNo, TraceNo); // To get the Transaction No

                                    if (PresError == "PRESENTER ERROR")
                                    {
                                        Ec.ErrId = 55;
                                    }
                                    if (SuspectDesc == "SUSPECT FOUND")
                                    {
                                        Ec.ErrId = 225;
                                    }

                                    Ec.BankId = WBankId;
                                    Ec.ReadErrorsIDRecord(Ec.ErrId, WOperator); // READ TO GET THE CHARACTERISTICS

                                    Am.ReadAtmsMainSpecific(WAtmNo);

                                    // INITIALISED WHAT IS NEEDED 

                                    Ec.CategoryId = "N/A"; 
                                    Ec.RMCycle = 0;
                                    Ec.MaskRecordId = 0; 

                                    Ec.AtmNo = WAtmNo;
                                    Ec.SesNo = WSesNo;
                                    Ec.DateInserted = DateTime.Now;
                                    Ec.DateTime = TRanDate;
                                    Ec.BranchId = Ac.Branch;
                                    Ec.ByWhom = Am.AuthUser;

                                    //    Ec.ActionDtTm = NullPastDate; 

                                    //  Ec.CurrCd = Tc.CurrCode;
                                    Ec.CurDes = Tc.CurrDesc;
                                    Ec.ErrAmount = Tc.TranAmount;

                                    Ec.TraceNo = Tc.AtmTraceNo;
                                    Ec.CardNo = Tc.CardNo;
                                    Ec.TransNo = Tc.TranNo;
                                    Ec.TransType = Tc.TransType;
                                    Ec.TransDescr = Tc.TransDesc;

                                    Ec.CustAccNo = Tc.AccNo;

                                    Ec.DatePrinted = NullPastDate;

                                    Ec.OpenErr = true;

                                    Ec.CitId = Am.CitId;

                                    Ec.Operator = Am.Operator;

                                    Ec.InsertError(); // INSERT ERROR

                                    // GET The just inserted last Error No
                                    //
                                    Ec.ReadLastErrorNo(WOperator, WAtmNo, WSesNo); // GET THE just inserted error number 

                                    // Update Error Number in transaction 

                                    Tc.UpdateTransErrNo(Tc.TranNo, Ec.ErrNo);

                                }
                            }

                            if (trandesc == "SM-CASH ADDED") // Cash ADDED 
                            {
                                // 
                                // UPdate Cassettes
                                // 
                                Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                                Ta.FirstTraceNo = TraceNo;
                                Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);

                                Cassette1 = (int)rdr["Type1"];
                                Cassette2 = (int)rdr["Type2"];
                                Cassette3 = (int)rdr["Type3"];
                                Cassette4 = (int)rdr["Type4"];

                                // Update Notes record

                                Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                                Na.Cassettes_1.InNotes = Cassette1;
                                Na.Cassettes_2.InNotes = Cassette2;
                                Na.Cassettes_3.InNotes = Cassette3;
                                Na.Cassettes_4.InNotes = Cassette4;

                                Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);
                                //
                                // Read Updated NEW 
                                //
                                Na.ReadSessionsNotesAndValues(InAtmNo, WSaveSesNo, 2);
                                decimal SaveMoneyIn = Na.Balances1.OpenBal;

                                // READ OLD - Previous data

                                Na.ReadSessionsNotesAndValues(InAtmNo, WSaveSesNo, 2);

                                // UPDATE Action TABLE WITH REAL MONEY 
                                Ra.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSaveSesNo);

                                if (Ra.RecordFound)
                                {
                                    Ra.InMoneyReal = Na.Balances1.OpenBal;

                                    Ra.ActiveRecord = false;

                                    Ra.UpdateReplActionsForAtm(WAtmNo, Ra.ReplActNo);
                                }

                                if (SaveMoneyIn != Na.Balances1.OpenBal)
                                {
                                    Ra.ActiveRecord = false;
                                    //MessageBox.Show(" MsgNo: 456387 An email will be sent to Controller. Money said in Replenishment different of what actual in"); 
                                }
                                //
                                // INSERT TRANSACTION WITH CASH ADDED
                                // 
                                // Build amount

                                Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                                int TraceNoCashIn = TraceNo;

                                if (Na.BalSets >= 1)
                                {
                                    // CALL METHOD TO ADD TRANSACTION FOR CASH ADDED 

                                    InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn,
                                        Na.Balances1.CurrNm, Na.Balances1.OpenBal, StartTrxn, EndTrxn);

                                    //if (WCitId != "1000") // IF ATM is replenished by CIT provider then Do create a trans in CIT Statement 
                                    //{
                                    //    Ct.InsertTranForCashInCit(WSignedId, WSignRecordNo, WOperator, InAtmNo, WSesNo);
                                    //}
                                }

                                if (Na.BalSets >= 2)
                                {
                                    TraceNoCashIn = TraceNo + 1; // Last digit becomes 7 
                                    InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn,
                                         Na.Balances2.CurrNm, Na.Balances2.OpenBal, StartTrxn, EndTrxn);
                                }
                                if (Na.BalSets >= 3)
                                {
                                    TraceNoCashIn = TraceNo + 2;
                                    InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn,
                                         Na.Balances3.CurrNm, Na.Balances3.OpenBal, StartTrxn, EndTrxn);
                                }

                                if (Na.BalSets == 4)
                                {
                                    TraceNoCashIn = TraceNo + 3;
                                    InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn,
                                          Na.Balances4.CurrNm, Na.Balances4.OpenBal, StartTrxn, EndTrxn);
                                }

                            }

                            if (trandesc == "SM-START" & TransactionType == 77) // SUPERVISOR MODE START = THIS IS THE END OF REPL CYCLE 
                            {
                                // Update the right Trace with the start
                                // Get TraceNo with zero last digit 
                                // Look and find Traces with this trace within 
                                // Update Start time for this Traces Record

                                SmStartDtTime = TRanDate;
                                if (WSesNo > 0) // If it is equal to zero it means this is the first time and there is no Trace No 
                                {
                                    Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                                    Ta.SesDtTimeEnd = SmStartDtTime;
                                    Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);
                                }

                            }

                            if (trandesc == "SM-END" & TransactionType == 77) // YESTERDAYS SM 
                            {
                                // Update the Right Trace with the end 
                                // HERE WE FIND THE DURATION OF THE REPL CYCLE

                                SmEndDtTime = TRanDate; // THIS THE YESTERDAYS SM  

                                Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                                Ta.SesDtTimeStart = SmEndDtTime;

                                Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);

                                RRDMReplStatsClass Rs = new RRDMReplStatsClass();
                                Rs.InsertInAtmsStats(WOperator, InAtmNo, WSesNo, Ta.SesDtTimeStart);

                            }

                            if (trandesc == "SM-DISPENCED") // Dispensed
                            {

                                // HERE WE NEED DATE AND TIME 

                                Cassette1 = (int)rdr["Type1"];
                                Cassette2 = (int)rdr["Type2"];
                                Cassette3 = (int)rdr["Type3"];
                                Cassette4 = (int)rdr["Type4"];

                                Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                                Na.Cassettes_1.DispNotes = Cassette1;
                                Na.Cassettes_2.DispNotes = Cassette2;
                                Na.Cassettes_3.DispNotes = Cassette3;
                                Na.Cassettes_4.DispNotes = Cassette4;

                                Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);
                            }

                            if (trandesc == "SM-REJECTED") // Rejected
                            {
                                // HERE WE NEED DATE AND TIME 

                                Cassette1 = (int)rdr["Type1"];
                                Cassette2 = (int)rdr["Type2"];
                                Cassette3 = (int)rdr["Type3"];
                                Cassette4 = (int)rdr["Type4"];

                                Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                                Na.Cassettes_1.RejNotes = Cassette1;
                                Na.Cassettes_2.RejNotes = Cassette2;
                                Na.Cassettes_3.RejNotes = Cassette3;
                                Na.Cassettes_4.RejNotes = Cassette4;

                                Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);
                            }

                            if (trandesc == "SM-CASSETTE") // Cassette equivalent Gas Remaining = What it is in cassettes 
                            {

                                // HERE WE NEED DATE AND TIME 

                                Cassette1 = (int)rdr["Type1"];
                                Cassette2 = (int)rdr["Type2"];
                                Cassette3 = (int)rdr["Type3"];
                                Cassette4 = (int)rdr["Type4"];

                                Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                                Na.Cassettes_1.RemNotes = Cassette1; // Equivalent what it is in cassettes NOT REJected 
                                Na.Cassettes_2.RemNotes = Cassette2;
                                Na.Cassettes_3.RemNotes = Cassette3;
                                Na.Cassettes_4.RemNotes = Cassette4;

                                Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);
                            }

                            if (trandesc == "SM-REMAINING") // Cash OUT FROM MACHINE ( CASSETTE + REJECTED )
                            {
                                // CALL CASH OUT CLASS 
                                // Create transaction for 
                                // a) ATM Pool
                                // b) CIT provider STATEMENT 

                                Ct.InsertTranForCashOut(WSignedId, WSignRecordNo, WOperator,
                                    InAtmNo, WSesNo, StartTrxn, EndTrxn);

                            }

                            I = I + 1;

                        }

                        // Close Reader
                        rdr.Close();
                        //TEST 
                        if (I > 0)
                        {
                            //  MessageBox.Show("MSG:999 Records read: " + I.ToString());
                        }

                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in E-JournalProcess............. " + ex.Message;

                }

            // Read Konto Errors table and create Errors in GAS
            // READ Errors 

            Em.ReadInsertMatchedErrors(WAtmNo);

            // UPDATE ATMsMain with Number of outstanding errors 

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            Am.ReadAtmsMainSpecific(WAtmNo);

            Ec.ReadAllErrorsTableForCounters(WOperator, "EWB110", WAtmNo);
            Am.ErrOutstanding = Ec.NumOfErrors;
            Am.ProcessMode = Ta.ProcessMode;

            Am.UpdateAtmsMain(WAtmNo);

        }

        // PROCEDURE TO READ EJOURNAL FROM ATM AND COPY IT IN DIRECTORY FOR KONTO TO READ
        private void AtmEjournalCopyToDirectory(string InBankId, string InAtmNo, string InIpAddress, int InAction)
        {
            // In 
            // .NET PROCESS 
            // If InAction = 1 = Just copy and do not intialise ATM
            // If InAction = 2 = Copy and Initialise ATM Journal. 
            // Copy To Directory for Konto always
            // If InAction = 2 then copy Backup too. 

            // Process: 1) read journal, 2) insert it in Konto Directory 3) Keep a copy in backup directory
            //           4) Initialise the ATM, 5) Finish. 

            /*
            string sourcePath = @"C:\Journals\TESTING THREE FILES";
            string destinationPath = @"C:\Journals\KONTO";
            string sourceFileName = "EJDATA.LOG.10.1.86.16.20140213.030935.2 - Copy.LOG";
            string destinationFileName = "ATM 1" + " EJDATA.LOG.10.1.86.16.20140213.030935.2.LOG";
            string sourceFile = System.IO.Path.Combine(sourcePath, sourceFileName);
            string destinationFile = System.IO.Path.Combine(destinationPath, destinationFileName);

            if (!System.IO.Directory.Exists(destinationPath))
            {
                System.IO.Directory.CreateDirectory(destinationPath);
            }
            System.IO.File.Copy(sourceFile, destinationFile, true);

            //Delete source file
            System.IO.File.Delete(sourcePath + @"\" + sourceFileName);
            */
        }
        //
        // CALL KONTO Procedure to read ejournal from directory and insert it in Files
        //
        private void StoreProcKontoEJournal(string InBankId, string InAtmNo, string InJournalType, string InPathName)
        {

            // Call Store procedure for reading ATM ejournal and preparing the files

            // Read Ejournal and create Data bases  

            string RCT = "[ATMS_Journals].[dbo].[stp_00_Run_Process]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@FullPath", InPathName);


                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in E-JournalProcess............. " + ex.Message;

                }
        }

        //
        // CALL KONTO Procedure to Make Reconciliation of Host Files with ATM Journal records  
        //
        private void StoreProcKontoMatchingOfFiles(string InBankId, string InAtmNo)
        {

            // Call Store procedure for reading ATM ejournal and preparing the files

            // Read Ejournal and create Data bases  

            string RCT = "[ATMS_Journals].[dbo].[usp_H_Reconciliation_Step_00]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in E-JournalProcess............. " + ex.Message;
                }
        }

        //
        // Insert Build and Insert Transaction during Creation of Session 
        //

        private void InsertTranInPool(string InAtmNo, int WSesNo, int InTraceNo,
                                      string InCurrNm, decimal InTranAmount, int Start, int End)
        {
            //
            // BUILD THE TRANSACTION 
            //

            Tc.CurrDesc = InCurrNm;
            Tc.TranAmount = InTranAmount;

            Tc.AtmTraceNo = InTraceNo;
            Tc.EJournalTraceNo = InTraceNo;

            Tc.OriginName = "OurATMs-Repl : " + InAtmNo;

            Tc.AtmNo = InAtmNo;
            Tc.SesNo = WSesNo;
            Tc.BankId = WBankId;

            Tc.BranchId = Ac.Branch;

            Tc.AtmDtTime = DateTime.Now; // THIS IS A TEMPORARY SOLUTION 
            //Tc.HostDtTime = DateTime.Now;

            Tc.SystemTarget = 9; // System Transaction 
            Tc.TransType = 22; // Deposit By ATM 
            Tc.TransDesc = "Cash Added To ATM";
            Tc.CardNo = "";
            Tc.CardOrigin = 1;

            Acc.ReadAndFindAccount("1000", WOperator, InAtmNo, InCurrNm, "ATM Cash");
            if (Acc.RecordFound == false)
            {
                ErrorFound = true;
                ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
                //  MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
            }
            Tc.AccNo = Acc.AccNo;  // Cash account No


            Tc.AuthCode = 0;
            Tc.RefNumb = 0;
            Tc.RemNo = 0;

            Tc.TransMsg = "";
            Tc.AtmMsg = "";
            Tc.ErrNo = 0;
            Tc.StartTrxn = Start;
            Tc.EndTrxn = End;
            Tc.SuccTran = true;

            // ADD THE TRANSACTION 
            Tc.InsertTransInPool(InAtmNo);
        }
    }
}

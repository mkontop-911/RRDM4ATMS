using System;
using System.Data;
////using System.Windows.Forms;
//using System.Data.SqlClient;
using System.Configuration;
using System.Text; 


namespace RRDM4ATMs
{
    public class RRDMJournalAndAllowUpdate : Logger
    {
        public RRDMJournalAndAllowUpdate() : base() { }
        // THIS CLASS PROVIDES A TABLE WITH ALLOWED ATMS FOR THIS USER 
        readonly string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); 
       // UsersAndSignedRecord Ua = new UsersAndSignedRecord();
        RRDMUpdateGrids Ug = new RRDMUpdateGrids();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMDepositsClass Da = new RRDMDepositsClass();
        //RRDMErrorsFromKontoToMatchingClass Em = new RRDMErrorsFromKontoToMatchingClass();

        //RRDMJournalLoadingTransProcess Ej = new RRDMJournalLoadingTransProcess();
        RRDMHostBatchesClass Hb = new RRDMHostBatchesClass();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMJournalRead_HstAtmTxns_AndCreateTable Rj = new RRDMJournalRead_HstAtmTxns_AndCreateTable();

        //  FormProcessing NFormProcessing;

        public DataTable dtAtmsMain = new DataTable();    
            
        //SqlDataAdapter daAtmsMain;
        
        //string SQLString;

        public int DtSize;

        int J;

        //int TotalTrans; 

        string WAtmNo;

        string WSignedId;
        int WSignRecordNo ;
        string WOperator;
  
        //string WFunction; 

        // CREATE TABLE WITH ALL ATMs ALLOWED FOR THIS USER 

        // READ ALLOWED TABLE AND UPDATE DATA BASES with latest ejournal values 

        public void UpdateLatestEjStatusVersion2(string InSignedId, int InSignRecordNo, string InOperator, DataTable InTableATMsMain )
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            //public DataTable ATMsMainSelected = new DataTable();
            DtSize = InTableATMsMain.Rows.Count;
             
            // Read all in Table and see if need 
            int tempJ = 0;

            //FormProcessing NFormProcess = new FormProcessing(DtSize);
            //NFormProcess.Show();

            //    Application.DoEvents();

            DateTime WDtTm;

            J = 0;

            while (J < DtSize)
            {

                WAtmNo = (string)InTableATMsMain.Rows[J]["AtmNo"];

                Am.ReadAtmsMainSpecific(WAtmNo);

                // If this is a new ATM then skip 
                if (Am.CurrentSesNo == 0)
                {
                    J++; 
                    continue;
                }

                //
                // Insert Performance Record
                //
                Pt.InsertPerformanceTrace(Am.BankId, Am.Operator, 1, "LoadTrans", WAtmNo, DateTime.Now, DateTime.Now, "");

                // Find current status for Money
                //TEST
                if (WAtmNo == "AB104" || WAtmNo == "AB102" || WAtmNo == "AB1044" || WAtmNo == "ABC502")
                {
                    // create a connection object
                    using (var scope = new System.Transactions.TransactionScope())
                        try
                        {
                            int WMode = 1; // READ TRANSACTIONS AND MADE UPDATING FOR NEW SESSION
                                           // IF 2 then it is continuation on existing                    
                            Rj.ReadAndCreateTables(WSignedId, WSignRecordNo, Am.BankId, WAtmNo, WMode);
                            //Ej.ReadAndCreateTables(WSignedId, WSignRecordNo, Am.BankId, WAtmNo, WMode);

                            //COMPLETE SCOPE

                            scope.Complete();

                        }

                        catch (Exception ex)
                        {
                            RRDMLog4Net Log = new RRDMLog4Net();

                            StringBuilder WParameters = new StringBuilder();

                            WParameters.Append("User : ");
                            WParameters.Append("NotAssignYet");
                            WParameters.Append(Environment.NewLine);

                            WParameters.Append("ATMNo : ");
                            WParameters.Append("NotDefinedYet");
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
                        finally
                        {
                            scope.Dispose();
                        }


                }

                //TEST
                //READING HOST WORK 
                if (WAtmNo == "AB102")
                {
                    // STEP1 READ HOST FILES AND PUT THEM IN KONTO DIRECTORY
                    // THIS WILL BE DONE BY THE BANK 

                    // FOR THIS ATM ( EACH ATM )
                    // STEP2 Call Konto to make matching and update of errors files 

                    // STEP3 READ ERRORS 
                    //Em.ReadInsertMatchedErrors(WAtmNo);

                    // STEP4 READ AND CREATE HOST BATCH

                    //  public int LastReplCyclId; // latest repl cycle 
                    //  public int LLatestBatchNo; // Latest 
                    //  public bool LUpdatedBatch; // Latest 

                    // STEP5 READ HOST BATCH To SEE if is the latest
                    //
                    Ta.FindLastReplCycleId(WAtmNo);

                    if (Ta.RecordFound == true & Ta.Is_Updated_GL == false)
                    {
                        Hb.ReadHostLastBatch(WAtmNo); // Read latest host
                        if (Ta.LLatestBatchNo < Hb.BatchNo)
                        {
                            // We have new Host file
                            // UPDATE TRACES WITH FINISH 
                            int Mode = 1; // Before reconciliation 
                            Ta.UpdateTracesFinishReplOrReconc(WAtmNo, Ta.LastReplCyclId, WSignedId, Mode);

                            // READ Ta to see if differences AND SEND EMAIL ALERTS

                            Ta.ReadSessionsStatusTraces(WAtmNo, Ta.LastReplCyclId);

                            if (Ta.Recon1.DiffReconcEnd == false & Ta.Is_Updated_GL == true)
                            {
                                // There differences to reconcile

                                //  ReconcComment = "EVERYTHING INCLUDING HOST FILES RECONCILE";
                            }

                            if (Ta.Recon1.DiffReconcEnd == true & Ta.Is_Updated_GL == true)
                            {
                                // There differences to reconcile

                                //  ReconcComment = "NEED TO GO TO RECONCILIATION PROCESS";
                            }

                            if (Ta.Recon1.DiffReconcEnd == true & Ta.Is_Updated_GL == false)
                            {
                                // There differences to reconcile

                                //    ReconcComment = "NEED OF RECONCILIATION BUT HOST FILES NOT AVAILABLE YET";
                            }

                            if (Ta.Recon1.DiffReconcEnd == false & Ta.Is_Updated_GL == false)
                            {
                                // There differences to reconcile

                                //  ReconcComment = "HOST FILES NOT AVAILABLE YET";
                            }

                        }
                    }
                }

                // UPDATE AM and Na

                // Current balance, current deposits for AM and Na

                Am.ReadAtmsMainSpecific(WAtmNo);

                Na.ReadSessionsNotesAndValues(WAtmNo, Am.CurrentSesNo, 2);

                Am.CurrCassettes = Na.Balances1.MachineBal;

                //TEST
                // DEPOSITS TOTALS TO UPDATE Na
                if (WAtmNo == "AB102" || WAtmNo == "12507" || WAtmNo == "ServeUk102" || WAtmNo == "ABC501")
                {
                    // Do nothing on deposits for these atms 
                }
                else
                {
                    Da.ReadDepositsTotals(WAtmNo, Am.CurrentSesNo); // Find Totals from deposit transactions 
                    Da.UpdateDepositsNaWithMachineTotals(WAtmNo, Am.CurrentSesNo); // Update Totals 
                }

                Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, Am.CurrentSesNo);

                Am.CurrentDeposits = Da.DepositsMachine1.Amount + Da.DepositsMachine1.EnvAmount;

                // Update Dispensed History

                Am.LastUpdated = DateTime.Now;

                WDtTm = Am.LastUpdated.AddDays(-1);

                //TEST
                // DISPENSED HISTORY
                if (WAtmNo == "AB104" || WAtmNo == "AB1044" || WAtmNo == "ABC502")
                {
                    //TEST
                    WDtTm = DateTime.Today;
                    WDtTm = new DateTime(2014, 02, 14);

                    if (Am.LastDispensedHistor < WDtTm)
                    {
                        // Dispensed History 

                        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
                        // UPDATE TRANS HISTORY RECORD 
                        Tc.ReadUpdateTransForDispensedHistory(WOperator, WAtmNo, Am.LastDispensedHistor, WDtTm);

                        if (Tc.RecordFound == true)
                        {
                            Am.LastDispensedHistor = WDtTm;
                            Am.    UpdateAtmsMain(WAtmNo);
                        }

                    }
                }

                tempJ = J + 1;
 //
                Pt.ReadMaxRecordNo(Am.BankId, WAtmNo, "LoadTrans");

                Pt.ReadPerformanceTraceRecNo(Pt.RecordNo);

                Pt.EndDT = DateTime.Now;

                // Now you have the two dates 

                //TotalTrans = 55; // Till we decide what to include 

                //Pt.UpdatePerformanceTrace(Pt.RecordNo, TotalTrans);

                //NFormProcess.SetStatus("Reading ATM " + tempJ + " out of " + DtSize.ToString(), tempJ);

                ////Application.DoEvents();
                //System.Threading.Thread.Sleep(200); //wait 0,2 secs

                J++;
                //
                // UPDATE PERFORMANCE RECORD
               
            }

            //NFormProcess.Dispose();
        }

        //
        // READ LATEST JOURNAL STATUS FROM JOURNAL HISTORY FOR A SPECIFIC ATM
        //
        
        public void UpdateLatestEjStatusForSpecificAtm(string InSignedId, 
                                   int InSignRecordNo, string InOperator, string InAtmNo )
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WAtmNo = InAtmNo; 

            Am.ReadAtmsMainSpecific(WAtmNo);

                // Find current status for Money
                //TEST
                if (WAtmNo == "AB104" || WAtmNo == "AB1044" || WAtmNo == "AB102" || WAtmNo == "ABC502")
                {
                    int WMode = 2; // READ TRANSACTIONS AND MADE UPDATING FOR NEW SESSION
                                   // IF 2 then it is continuation on existing                    

                Rj.ReadAndCreateTables(WSignedId, WSignRecordNo, Am.BankId, WAtmNo, WMode);

                    // if (WAtmNo == "AB104") dateTimePicker1.Value = new DateTime(2014, 02, 13);

                }

            if (WOperator == "ETHNCY2N") 
            {
                RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

                RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables();
                //Test This
                // Load all unprocessed records from Primary Tableto the Master Pool 
                //

                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                string WJobCategory = "ATMs";
                int WReconcCycleNo;
                string Message;

                bool ShowMessage = true;

                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                // Load File 
                RRDMJournalRead_HstAtmTxns_AndCreateTable_NBG Jr = new RRDMJournalRead_HstAtmTxns_AndCreateTable_NBG();

                RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM();

                // INPORT ALL ENTRIES CREATED FROM JOURNALs For This ATM
                int WMode = 2;
                
                //Jr.ReadJournalAndCreateTables(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WMode);
                JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo,WOperator, 0, WAtmNo, 0 ,WMode);

                //**********************************************************
                // Insert Message And show it if needed
                //
              
                    Message =
                                 "Parsed Journal Txns for all ATMs" + Environment.NewLine
                                + "moved to RRDM Data Repository." + Environment.NewLine
                                + "Total Records read.............: " + JrNew.TotalRecords.ToString() + Environment.NewLine
                                + "Total Txns Moved to Repository : " + JrNew.GrandTotalTxns.ToString();

                

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Load Atms Journal Txns", "", DateTime.Now.Date, DateTime.Now, Message);

                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }

            // UPDATE AM and Na

            // Current balance, current deposits for AM and Na

            Am.ReadAtmsMainSpecific(WAtmNo);

            if (Am.CurrentSesNo == 0)
            {
                // NEW ATM WITHOUT TXNS YET 
                return; 
            }

                Na.ReadSessionsNotesAndValues(WAtmNo, Am.CurrentSesNo, 2);

                Am.CurrCassettes = Na.Balances1.MachineBal;

                //TEST
                // DEPOSITS TOTALS TO UPDATE Na
                if (WAtmNo == "AB102" || WAtmNo == "12507" || WAtmNo == "ServeUk102" || WAtmNo == "ABC501")
                {
                    // Do nothing on deposits for these atms 
                }
                else
                {
                    Da.ReadDepositsTotals(WAtmNo, Am.CurrentSesNo); // Find Totals from deposit transactions 
                    Da.UpdateDepositsNaWithMachineTotals(WAtmNo, Am.CurrentSesNo); // Update Totals 
                }

                Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, Am.CurrentSesNo);

                Am.CurrentDeposits = Da.DepositsMachine1.Amount + Da.DepositsMachine1.EnvAmount;

                // Update Dispensed History

                Am.LastUpdated = DateTime.Now;

                DateTime WDtTm = Am.LastUpdated.AddDays(-1);

                //TEST
                // DISPENSED HISTORY
                if (WAtmNo == "AB104" || WAtmNo == "AB1044" || WAtmNo == "ABC502")
                {
                    //TEST
                    WDtTm = DateTime.Today;
                    WDtTm = new DateTime(2014, 02, 14);

                    if (Am.LastDispensedHistor < WDtTm)
                    {
                        // Dispensed History 

                        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
                        // UPDATE TRANS HISTORY RECORD 
                        Tc.ReadUpdateTransForDispensedHistory(WOperator, WAtmNo, Am.LastDispensedHistor, WDtTm);

                        if (Tc.RecordFound == true)
                        {
                            Am.LastDispensedHistor = WDtTm;
                            Am.UpdateAtmsMain(WAtmNo);
                        }

                    }
                }
        }
    }
}

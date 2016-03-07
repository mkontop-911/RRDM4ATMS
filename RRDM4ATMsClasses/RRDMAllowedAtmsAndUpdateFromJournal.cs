using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMAllowedAtmsAndUpdateFromJournal
    {
        // THIS CLASS PROVIDES A TABLE WITH ALLOWED ATMS FOR THIS USER 
        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;
        RRDMNotesBalances Na = new RRDMNotesBalances();
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); 
       // UsersAndSignedRecord Ua = new UsersAndSignedRecord();
        RRDMUpdateGrids Ug = new RRDMUpdateGrids();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMDepositsClass Da = new RRDMDepositsClass();
        RRDMErrorsFromKontoToMatchingClass Em = new RRDMErrorsFromKontoToMatchingClass();

        RRDME_Journal_Process Ej = new RRDME_Journal_Process();
        RRDMHostBatchesClass Hb = new RRDMHostBatchesClass();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

    //    FormProcessing NFormProcessing;

        public DataTable dtAtmsMain = new DataTable();
       
            
        SqlDataAdapter daAtmsMain;
        
        string SQLString;

        public int DtSize;

        int J;

        int TotalTrans; 

        string WAtmNo;

        string WSignedId;
        int WSignRecordNo ;
        string WOperator;
    //    bool WPrive;
        string WFunction; 

        // CREATE TABLE WITH ALL ATMs ALLOWED FOR THIS USER 

        public void CreateTableOfAccess(string InSignedId, int InSignRecordNo, string InOperator, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator; // if Prive = the User Bank if not Prive = NoBank
       
            WFunction = InFunction; 

            // Read USER and ATM Table 
            //WAction = 1; // Update Main record AuthUser field with User Applies to single or group of ATMs 

            Us.ReadSignedActivityByKey(WSignRecordNo); // REad Access level 

            if (Us.SecLevel == 3) // 2: Initialise Data Grid , // 1: Update Data Grid with AUthorised User 
            {
               
                Ug.UpdateAtmMainAuthUserWithBlanks(WOperator, WSignedId); 
                
                Ug.ReadUsersAccessAtmAndUpdateMain(WSignedId, 1, WFunction);
            }

            if (Us.SecLevel == 3)
            {
                SQLString = "Select [AtmNo]  FROM [dbo].[AtmsMain] WHERE Operator = '"
                    + WOperator + "' AND AuthUser ='" + WSignedId +"'";
            }

            // THIS ACCESS LEVEL = 4 is the Controller - Is allowed to see all 

            if (Us.SecLevel == 4)
            {
                SQLString = "Select [AtmNo]  FROM [dbo].[AtmsMain] WHERE Operator = '" + WOperator + "'";
            }
        

            // BASED ON THE SQLString Read ATM MAIN 
            //

            dtAtmsMain.Clear(); 
 
            SqlConnection conn =
                 new SqlConnection(connectionString);
            daAtmsMain = new SqlDataAdapter(SQLString, conn);

            SqlCommandBuilder cmdBldr = new SqlCommandBuilder(daAtmsMain);

            daAtmsMain.Fill(dtAtmsMain); // ATMs Numbers are now in data set table

            DtSize = dtAtmsMain.Rows.Count;
            string DtsSize = DtSize.ToString();
        }

        // READ ALLOWED TABLE AND UPDATE DATA BASES with latest ejournal values 

        public void UpdateLatestEjStatus(string InSignedId, int InSignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            // Read all in Table and see if need 
            int tempJ = 0;
            
           //FormProcessing NFormProcess = new FormProcessing(DtSize);
           //NFormProcess.Show();

       //    Application.DoEvents();

            DateTime WDtTm;

            J = 0;

            while (J < DtSize)
            {
                
                WAtmNo = (string)dtAtmsMain.Rows[J]["AtmNo"];
                
                Am.ReadAtmsMainSpecific(WAtmNo);
                //
                // Insert Performance Record
                //
                Pt.InsertPerformanceTrace(Am.BankId, Am.Operator, "LoadTrans", WAtmNo, DateTime.Now); 

                // Find current status for Money
                //TEST
                if (WAtmNo == "AB104" || WAtmNo == "ABC502")
                {
                    int WMode = 1; // READ TRANSACTIONS AND MADE UPDATING FOR NEW SESSION
                    // IF 2 then it is continuation on existing                    

                    Ej.ReadAndCreateTables(WSignedId, WSignRecordNo, Am.BankId, WAtmNo, WMode);                   

                   // if (WAtmNo == "AB104") dateTimePicker1.Value = new DateTime(2014, 02, 13);

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
                    Em.ReadInsertMatchedErrors(WAtmNo);

                    // STEP4 READ AND CREATE HOST BATCH

                 //  public int LastReplCyclId; // latest repl cycle 
                 //  public int LLatestBatchNo; // Latest 
                 //  public bool LUpdatedBatch; // Latest 

                    // STEP5 READ HOST BATCH To SEE if is the latest
                    //
                    Ta.FindLastReplCycleId(WAtmNo);

                    if (Ta.RecordFound == true & Ta.UpdatedBatch == false)
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

                         if (Ta.Recon1.DiffReconcEnd == false & Ta.UpdatedBatch == true)
                         {
                             // There differences to reconcile

                           //  ReconcComment = "EVERYTHING INCLUDING HOST FILES RECONCILE";
                         }

                         if (Ta.Recon1.DiffReconcEnd == true & Ta.UpdatedBatch == true)
                         {
                             // There differences to reconcile

                           //  ReconcComment = "NEED TO GO TO RECONCILIATION PROCESS";
                         }

                         if (Ta.Recon1.DiffReconcEnd == true & Ta.UpdatedBatch == false)
                         {
                             // There differences to reconcile

                         //    ReconcComment = "NEED OF RECONCILIATION BUT HOST FILES NOT AVAILABLE YET";
                         }

                         if (Ta.Recon1.DiffReconcEnd == false & Ta.UpdatedBatch == false)
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
                if (WAtmNo == "AB104" || WAtmNo == "ABC502")
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
            
                tempJ = J + 1;
               
                //NFormProcess.SetStatus("Reading ATM " + tempJ + " out of " + DtSize.ToString(), tempJ);

                //Application.DoEvents();
                System.Threading.Thread.Sleep(200); //wait 0,2 secs

                J++;
                //
                // UPDATE PERFORMANCE RECORD
                //
                Pt.ReadMaxRecordNo(Am.BankId, WAtmNo, "LoadTrans");

                Pt.ReadPerformanceTraceRecNo(Pt.RecordNo); 

                Pt.EndDT = DateTime.Now; 

                // Now you have the two dates 

                TotalTrans = 55; // Till we decide what to include 

                Pt.UpdatePerformanceTrace(Pt.RecordNo,TotalTrans); 
            }
           
            //NFormProcess.Dispose();
        }

    }
}

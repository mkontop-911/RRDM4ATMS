using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMTurboReconcClass
    {
        // TURBO reconciliation applies to predefined error types like Presenter Error. 
        // SYSTEM examines that if with Turbo there is reconciliation the process finishes. 
        // Errors are set under Action and TRACES AND Main are updated. 

        public int TotTurboDone;
        public int TotTurboNonDone;

        public int TotalATMsReady;

        public int TotalErrorsAtATMs; 

         public decimal TotalAmountErrors; 
        public decimal TotalAmountUnMatched;

        public int TotalAtmsWithUnMatchedRecords; 
        public int TotalUnMatchedRecords;  

        string WAtmNo;
        int WSesNo;

        bool ReconcDiff;


        DateTime LastUpdated;

        string Maker; 

        string AuthUser;

        string SqlString;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
        RRDMNotesBalances Na = new RRDMNotesBalances();
        RRDMErrorsClassWithActions La = new RRDMErrorsClassWithActions();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 

      //  string WUserBankId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     //   bool WPrive;

        // READ Session Traces FOR TURBO RECONCILIATION 
        // 

      //  private void ReadSessionsStatusTracesForTurbo(int InChosenGroup)
        
public void ReadUpdateSessionsStatusTracesForTurbo(string InSignedId, int InSignRecordNo, string InOperator, int InMode)
          
        {
    //
    // Read all ATMs find the ones that are ready for reconciliation = Process =1 
    // Find out if Outstanding unmatched 
    // Do the turbo if needed 
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            TotalAtmsWithUnMatchedRecords = 0; 
            TotalUnMatchedRecords = 0 ; 
  
            TotTurboDone = 0;
            TotTurboNonDone = 0;

            bool NotGood = false; 

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                SqlString = "SELECT *"
                 + " FROM [dbo].[AtmsMain] "
                 + " WHERE Operator = @Operator AND AuthUser = @AuthUser AND UnderReconcMode = 0 "
                 + " order by AtmsReconcGroup";
            }

            if (InMode == 2) // THIS MODE NOT USED AT THIS MOMENT 
            {
                SqlString = "SELECT *"
                 + " FROM [dbo].[AtmsMain] "
                 + " WHERE Operator = @Operator AND AuthUser = @AuthUser "
                 + " order by AtmsReconcGroup";
            }
             

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@AuthUser", WSignedId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            NotGood = false; 

                            WAtmNo = (string)rdr["AtmNo"];
                            WSesNo = (int)rdr["ReplCycleNo"]; // this is the Repl No waiting antention is the 7759 and not the 7760 which the running one

                            ReconcDiff = (bool)rdr["ReconcDiff"];

                            LastUpdated = (DateTime)rdr["LastUpdated"];

                            Maker = (string)rdr["Maker"];

                            AuthUser = (string)rdr["AuthUser"];

                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                            if (Ta.ProcessMode == 1)
                            {
                                // Replenish and ready 
                                Am.ReadAtmsMainSpecific(WAtmNo); // REad all fields 
                                Am.Maker = "No Decision";
                                Am.UpdateAtmsMain(WAtmNo);
                                
                            }
                            else
                            {
                                // THIS ATM is not ready for reconciliation 
                                Am.ReadAtmsMainSpecific(WAtmNo); // REad all fields 
                                Am.ExpressAction = false;
                                Am.Maker = "UnReplenish";
                                Am.Authoriser = "N/A";
                                Am.UpdateAtmsMain(WAtmNo);
                                NotGood = true; 
                            }
                            //
                            // Check if outstanding unmatched for this ATM 
                            //
                            string SearchingStringLeft = "Operator ='" + WOperator
                                          + "' AND (RMCateg ='EWB102' OR RMCateg ='EWB103' OR RMCateg ='EWB104' OR RMCateg ='EWB105' OR RMCateg ='EWB106') "
                                          + " AND TerminalId ='" + WAtmNo + "' AND (OpenRecord = 1 AND ActionType <> '4') ";

                            string WhatFile = "UnMatched"; 
                            string WSortValue = "SeqNo";
                            Rm.ReadMatchedORUnMatchedFileTableLeft(WOperator, SearchingStringLeft, WhatFile, WSortValue);
                            if (Rm.RecordFound == true)
                            {
                                TotalAtmsWithUnMatchedRecords = TotalAtmsWithUnMatchedRecords + 1; 
                                TotalUnMatchedRecords = TotalUnMatchedRecords + Rm.TotalSelected; 
                                // Outstanding unmatched 
                                Am.ReadAtmsMainSpecific(WAtmNo); // REad all fields 
                                Am.ExpressAction = false;
                                Am.Maker = "UnMatched";
                                Am.Authoriser = "N/A";
                                Am.UpdateAtmsMain(WAtmNo);
                                NotGood = true; 
                            }

                            // STart Turbo process 

                            // Call Notes Balances with Parameter 5 to consider all errors in balances 

                            if (ReconcDiff == true & NotGood == false)
                            {
                                // UPDATE ATMS MAIN Express 
                                Am.ReadAtmsMainSpecific(WAtmNo);

                                Am.UnderReconcMode = true;

                                Am.UpdateAtmsMain(WAtmNo);

                                int Process = 5;

                                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process);

                                //  If by considering all errors there is success in no difference then we update the ERRORS and Sessions 

                                
                                //if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false)
                                if (Na.DiffAtAtmLevel == false )
                                {
                                    TotTurboDone = TotTurboDone + 1;

                                    if (InMode == 1) // Without Updating ..... Update only Main with Express and errors 
                                    {
                                        // UPDATE ATMS MAIN Express 
                                        Am.ReadAtmsMainSpecific(WAtmNo);

                                        Am.ExpressAction = true;

                                        Am.Maker = "Express";

                                        Am.Authoriser = "No Decision";

                                        Am.UpdateAtmsMain(WAtmNo);

                                        // Update Errors table for under action 

                                        bool WUnderAction = true;

                                        Er.UpdateErrorsWithChangeUnderAction(WOperator, WAtmNo, WUnderAction);

                                    }

                                    if (InMode == 2) // With Updating 
                                    {

                                        if (Maker == "Express")
                                        {
                                        // REad errors one by one and update the Under action status 
                                        La.ReadAllErrorsTableForTurboAction(WOperator, WAtmNo); // READ AND UPDATE WITHIN 
                                        }

                                        // UPDATE ATMS MAIN 
                                        Am.ReadAtmsMainSpecific(WAtmNo);
                                        Am.ReconcDiff = false;
                                        Am.LastUpdated = DateTime.Now;
                                        Am.UpdateAtmsMain(WAtmNo);

                                        // Create the transactions as a result of actions taken on Errors
                                        //
                                        La.ReadAllErrorsTableForPostingTrans(WOperator, "EWB110", WAtmNo, WSignedId, AuthUser, WSesNo);
                                        //

                                        // UPDATE SESSION STATUS TRACES 

                                        Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                                        Ta.Recon1.FinishReconc = true;
                                        Ta.Recon1.RecFinDtTm = DateTime.Now;
                                        Ta.Recon1.DiffReconcEnd = false;
                                        Ta.NumOfErrors = Na.NumberOfErrors;
                                        Ta.ErrOutstanding = 0;

                                        Ta.SessionsInDiff = 0;

                                        //
                                        // UPDATE SESSION TRACES
                                        //

                                        Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
                                    }
                                }
                                else TotTurboNonDone = TotTurboNonDone + 1; 
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
                    ErrorOutput = "An error occured in TurboReconcClass............. " + ex.Message;

                }

        }

//
// 

// find totals to report 

public void ReadTotalsForATMsCashReconciliation(string InSignedId, int InSignRecordNo, string InOperator)
{
    //
    // Read all ATMs find the ones that are ready for reconciliation = Process =1 
    // Find out if Outstanding unmatched 
    // Do the turbo if needed 
    WSignedId = InSignedId;
    WSignRecordNo = InSignRecordNo;
    WOperator = InOperator;

    TotalATMsReady = 0 ; // Replenishment Done and no UnMatched Records
 

    TotalErrorsAtATMs = 0 ; // Total Errors to be handle 

    TotalAmountErrors = 0 ; // Total Errors amount

    TotalAmountUnMatched = 0 ; // Total  

    TotalAtmsWithUnMatchedRecords = 0;
    TotalUnMatchedRecords = 0;

    RecordFound = false;
    ErrorFound = false;
    ErrorOutput = "";

    
        SqlString = "SELECT *"
         + " FROM [dbo].[AtmsMain] "
         + " WHERE Operator = @Operator AND AuthUser = @AuthUser "
         + " order by AtmsReconcGroup";
  

    using (SqlConnection conn =
                  new SqlConnection(connectionString))
        try
        {
            conn.Open();
            using (SqlCommand cmd =
                new SqlCommand(SqlString, conn))
            {
                cmd.Parameters.AddWithValue("@Operator", WOperator);
                cmd.Parameters.AddWithValue("@AuthUser", WSignedId);

                // Read table 

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    RecordFound = true;

                    WAtmNo = (string)rdr["AtmNo"];
                    WSesNo = (int)rdr["ReplCycleNo"]; // this is the Repl No waiting antention is the 7759 and not the 7760 which the running one

                    ReconcDiff = (bool)rdr["ReconcDiff"];

                    LastUpdated = (DateTime)rdr["LastUpdated"];

                    Maker = (string)rdr["Maker"];

                    AuthUser = (string)rdr["AuthUser"];

                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                    if (Ta.ProcessMode == 1)
                    {
                        
                        TotalATMsReady = TotalATMsReady + 1;

                        La.ReadAllErrorsTableForCounters(WOperator, "", WAtmNo);

                        TotalErrorsAtATMs = TotalErrorsAtATMs + La.NumOfOpenErrorsLess100;

                        TotalAmountErrors = TotalAmountErrors + La.TotalErrorsAmtLess100; 
                 
                    }
                    else
                    {
                                   

                    }
                    //
                    // Check if outstanding unmatched for this ATM 
                    //
                    string SearchingStringLeft = "Operator ='" + WOperator
                                  + "' AND (RMCateg ='EWB102' OR RMCateg ='EWB103' OR RMCateg ='EWB104' OR RMCateg ='EWB105' OR RMCateg ='EWB106') "
                                  + " AND TerminalId ='" + WAtmNo + "' AND (OpenRecord = 1 AND ActionType <> '4') ";

                    string WhatFile = "UnMatched";
                    string WSortValue = "SeqNo";
                    Rm.ReadMatchedORUnMatchedFileTableLeft(WOperator, SearchingStringLeft, WhatFile, WSortValue);
                    if (Rm.RecordFound == true)
                    {
                        TotalAtmsWithUnMatchedRecords = TotalAtmsWithUnMatchedRecords + 1;
                        TotalUnMatchedRecords = TotalUnMatchedRecords + Rm.TotalSelected;

                        TotalAmountUnMatched = TotalAmountUnMatched + Rm.TotalAmountUnMatched;
                  
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
            ErrorOutput = "An error occured in ReadTotalsForATMsCashReconciliation(string InSignedId, int InSignRecordNo, string InOperator)............. " + ex.Message;

        }
}
    }
}

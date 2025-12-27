using System;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMTurboReconcClass : Logger
    {
        public RRDMTurboReconcClass() : base() { }
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

        bool GL_ReconcDiff;


        DateTime LastUpdated;

        string Maker;

        string AuthUser;

        string SqlString;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMErrorsClassWithActions La = new RRDMErrorsClassWithActions();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        //  string WUserBankId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        //   bool WPrive;

        // READ Session Traces FOR TURBO RECONCILIATION 
        // 

        //  private void ReadSessionsStatusTracesForTurbo(int InChosenGroup)

        //public void ReadUpdateSessionsStatusTracesForTurbo(string InSignedId, int InSignRecordNo, string InOperator, int InAtmsReconcGroup, int InMode)

        //{
        //    //
        //    // Read all ATMs find the ones that are ready for reconciliation = Process =1 
        //    // Find out if Outstanding unmatched 
        //    // Do the turbo if needed 
        //    WSignedId = InSignedId;
        //    WSignRecordNo = InSignRecordNo;
        //    WOperator = InOperator;

        //    TotalAtmsWithUnMatchedRecords = 0;
        //    TotalUnMatchedRecords = 0;

        //    TotTurboDone = 0;
        //    TotTurboNonDone = 0;

        //    bool IsGood = false;

        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    if (InMode == 1)
        //    {
        //        SqlString = "SELECT *"
        //         + " FROM [dbo].[AtmsMain] "
        //         + " WHERE Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup AND UnderReconcMode = 0 "
        //         + " order by AtmsReconcGroup";
        //    }

        //    if (InMode == 2) // THIS MODE NOT USED AT THIS MOMENT 
        //    {
        //        SqlString = "SELECT *"
        //         + " FROM [dbo].[AtmsMain] "
        //         + " WHERE Operator = @Operator AND AuthUser = @AuthUser "
        //         + " order by AtmsReconcGroup";
        //    }


        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@Operator", WOperator);
        //                cmd.Parameters.AddWithValue("@AuthUser", WSignedId);
        //                cmd.Parameters.AddWithValue("@AtmsReconcGroup", InAtmsReconcGroup);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    IsGood = false;

        //                    WAtmNo = (string)rdr["AtmNo"];
        //                    WSesNo = (int)rdr["ReplCycleNo"]; // this is the Repl No waiting antention is the 7759 and not the 7760 which the running one

        //                    ReconcDiff = (bool)rdr["ReconcDiff"];

        //                    LastUpdated = (DateTime)rdr["LastUpdated"];

        //                    Maker = (string)rdr["Maker"];

        //                    AuthUser = (string)rdr["AuthUser"];

        //                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

        //                    if (Ta.ProcessMode == 1)
        //                    {
        //                        // Replenish and ready 
        //                        Am.ReadAtmsMainSpecific(WAtmNo); // REad all fields 
        //                        Am.Maker = "No Decision";
        //                        Am.UpdateAtmsMain(WAtmNo);
        //                        IsGood = true;
        //                    }
        //                    else
        //                    {
        //                        // THIS ATM is not ready for reconciliation 
        //                        Am.ReadAtmsMainSpecific(WAtmNo); // REad all fields 
        //                        Am.ExpressAction = false;
        //                        Am.Maker = "UnReplenish";
        //                        Am.Authoriser = "N/A";
        //                        Am.UpdateAtmsMain(WAtmNo);
        //                        IsGood = false;
        //                        continue;
        //                    }
        //                    //
        //                    // Check if outstanding unmatched for this ATM 
        //                    //
        //                    string SearchingStringLeft = " WHERE Operator ='" + WOperator + "'"
        //                                  //    + "' AND (RMCateg ='EWB102' OR RMCateg ='EWB103' OR RMCateg ='EWB104' OR RMCateg ='EWB105' OR RMCateg ='EWB106') "
        //                                  + " AND TerminalId ='" + WAtmNo + "'"
        //                                  + " AND ( IsMatchingDone = 1 AND SettledRecord = 0 AND ActionType <> '4') AND Matched = 0 ";

        //                    string WSortValue = "";

        //                    Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, 1, SearchingStringLeft, WSortValue, NullPastDate, NullPastDate);

        //                    if (Mpa.RecordFound == true)
        //                    {
        //                        TotalAtmsWithUnMatchedRecords = TotalAtmsWithUnMatchedRecords + 1;
        //                        TotalUnMatchedRecords = TotalUnMatchedRecords + Mpa.TotalSelected;
        //                        // Outstanding unmatched 
        //                        Am.ReadAtmsMainSpecific(WAtmNo); // REad all fields 
        //                        Am.ExpressAction = false;
        //                        Am.Maker = "UnMatched";
        //                        Am.Authoriser = "N/A";
        //                        Am.UpdateAtmsMain(WAtmNo);
        //                        IsGood = false;
        //                        continue;
        //                    }

        //                    // STart Turbo process 

        //                    // Call Notes Balances with Parameter 5 to consider all errors in balances 

        //                    if (ReconcDiff == true & IsGood == true)
        //                    {
        //                        // UPDATE ATMS MAIN Express 
        //                        Am.ReadAtmsMainSpecific(WAtmNo);

        //                        Am.UnderReconcMode = true;

        //                        Am.UpdateAtmsMain(WAtmNo);

        //                        int Process = 5;

        //                        Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process);

        //                        //  If by considering all errors there is success in no difference then we update the ERRORS and Sessions 

        //                        // Find out whether there are errors whose record not in Master pool
        //                        // Eg Presenter error after last nights matching
        //                        //string filter = "AtmNo='" + WAtmNo + "' AND CurDes ='" + Na.Balances1.CurrNm
        //                        //+ "'" + " AND ErrType = 1  AND SesNo<=" + WSesNo + " AND (OpenErr=1 OR (OpenErr=0 AND ActionSes =" + WSesNo + "))  ";

        //                        //Er.ReadErrorsAndFillTable(WOperator, filter); // This returns the errors not in pool yet. 

        //                        //if (Na.DiffAtAtmLevel == false & Er.NotInTransactionPoolYet == 0)
        //                        if (Na.DiffAtAtmLevel == false)
        //                        {
        //                            // Check if the records for this ERROR are Matched. eg = "111"
        //                            if (Er.ReadErrorsWithPresentedErrorsAndCheckIfRecordsAreMatched(WOperator, WAtmNo, WSesNo) == false)
        //                            {
        //                                TotTurboNonDone = TotTurboNonDone + 1;
        //                                break; // THIS CANNOT BE EXPRESS .. IT is not matched 
        //                            }

        //                            TotTurboDone = TotTurboDone + 1;

        //                            if (InMode == 1) // Without Updating ..... Update only Main with Express and errors 
        //                            {
        //                                // UPDATE ATMS MAIN Express 
        //                                Am.ReadAtmsMainSpecific(WAtmNo);

        //                                Am.ExpressAction = true;

        //                                Am.Maker = "Express";

        //                                Am.Authoriser = "No Decision";

        //                                Am.UpdateAtmsMain(WAtmNo);

        //                                // Update Errors table for under action 

        //                                bool WUnderAction = true;

        //                                Er.UpdatePresenterErrorsWithChangeUnderAction(WOperator, WAtmNo, WSesNo, WUnderAction);

        //                            }

        //                            if (InMode == 2) // With Updating 
        //                            {

        //                                if (Maker == "Express")
        //                                {
        //                                    // REad errors one by one and update the Under action status 
        //                                    //La.ReadAllErrorsTableForTurboActionAndMakeThemUnderAction(WOperator, WAtmNo, WSesNo); // READ AND UPDATE WITHIN 
        //                                }

        //                                // UPDATE ATMS MAIN 
        //                                Am.ReadAtmsMainSpecific(WAtmNo);
        //                                Am.ReconcDiff = false;
        //                                Am.LastUpdated = DateTime.Now;
        //                                Am.UpdateAtmsMain(WAtmNo);

        //                                // Create the transactions as a result of actions taken on Errors
        //                                //
        //                                Er.ReadAllErrorsTableForPostingTrans(WOperator, "EWB110", WAtmNo, WSignedId, AuthUser, WSesNo, "");
        //                                //

        //                                // UPDATE SESSION STATUS TRACES 

        //                                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

        //                                Ta.Recon1.FinishReconc = true;
        //                                Ta.Recon1.RecFinDtTm = DateTime.Now;
        //                                Ta.Recon1.DiffReconcEnd = false;
        //                                Ta.NumOfErrors = Na.NumberOfErrors;
        //                                Ta.ErrOutstanding = 0;

        //                                Ta.SessionsInDiff = 0;

        //                                //
        //                                // UPDATE SESSION TRACES
        //                                //

        //                                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
        //                            }
        //                        }
        //                        else TotTurboNonDone = TotTurboNonDone + 1;
        //                    }
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


        // READ Session Traces FOR  RECONCILIATION FROM NEW WAY 
        // 

        public void ReadToFindAtmsofThisGroupThatNeedReconciliation(string InSignedId, int InSignRecordNo, string InOperator, int InAtmsReconcGroup, int InMode)

        {
            //
            // Read all ATMs find the ones that are ready for reconciliation = Process =1 
            // Find out if Outstanding unmatched 
            // Do the turbo if needed 
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            TotalAtmsWithUnMatchedRecords = 0;
            TotalUnMatchedRecords = 0;

            TotTurboDone = 0;
            TotTurboNonDone = 0;

            //bool IsGood = false;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                 + " FROM [dbo].[AtmsMain]  "
                 + " WHERE Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup "
                 + " AND GL_ReconcDiff = 1 ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InAtmsReconcGroup);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //IsGood = false;

                            WAtmNo = (string)rdr["AtmNo"];
                            WSesNo = (int)rdr["ReplCycleNo"]; // this is the Repl No waiting antention is the 7759 and not the 7760 which the running one

                            GL_ReconcDiff = (bool)rdr["GL_ReconcDiff"];

                            LastUpdated = (DateTime)rdr["LastUpdated"];

                            Maker = (string)rdr["Maker"];

                            AuthUser = (string)rdr["AuthUser"];
                  
                            // Check if outstanding unmatched for this ATM 
                            //
                            string SearchingStringLeft = " WHERE Operator ='" + WOperator + "'"
                                          //    + "' AND (RMCateg ='EWB102' OR RMCateg ='EWB103' OR RMCateg ='EWB104' OR RMCateg ='EWB105' OR RMCateg ='EWB106') "
                                          + " AND TerminalId ='" + WAtmNo + "'"
                                          + " AND ( IsMatchingDone = 1 AND SettledRecord = 0 AND ActionType <> '4') AND Matched = 0 ";

                            string WSortValue = "";

                            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, 1, SearchingStringLeft, WSortValue, NullPastDate, NullPastDate,2);

                            if (Mpa.RecordFound == true)
                            {
                                TotalAtmsWithUnMatchedRecords = TotalAtmsWithUnMatchedRecords + 1;
                                TotalUnMatchedRecords = TotalUnMatchedRecords + Mpa.TotalSelected;
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
// Read the ATMs that were reconciled at that date 
        public void ReadToFindAtmsofThisGroupReconciledAtThatDate(string InSignedId, int InSignRecordNo, string InOperator, int InAtmsReconcGroup, int InMode)

        {
            //
            // Read all ATMs find the ones that are ready for reconciliation = Process =1 
            // Find out if Outstanding unmatched 
            // Do the turbo if needed 
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            TotalAtmsWithUnMatchedRecords = 0;
            TotalUnMatchedRecords = 0;

            TotTurboDone = 0;
            TotTurboNonDone = 0;

            bool IsGood = false;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                 + " FROM [dbo].[AtmsMain]  "
                 + " WHERE Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup "
                 + " AND ReconcDiff = 1 ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InAtmsReconcGroup);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            IsGood = false;

                            WAtmNo = (string)rdr["AtmNo"];
                            WSesNo = (int)rdr["ReplCycleNo"]; // this is the Repl No waiting antention is the 7759 and not the 7760 which the running one

                            GL_ReconcDiff = (bool)rdr["ReconcDiff"];

                            LastUpdated = (DateTime)rdr["LastUpdated"];

                            Maker = (string)rdr["Maker"];

                            AuthUser = (string)rdr["AuthUser"];

                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                            if (Ta.ProcessMode == 1 || Ta.Repl1.DiffRepl == true)
                            {
                                // Replenish and ready 
                                Am.ReadAtmsMainSpecific(WAtmNo); // Read all fields 
                                if (Am.Maker == "Maker Actions")
                                {
                                    // Do nothing 
                                }
                                else
                                {
                                    // No Action was taken yet 
                                    Am.Maker = "No Decision";
                                }

                                Am.UpdateAtmsMain(WAtmNo);
                                IsGood = true;
                            }
                            else
                            {
                                IsGood = false;
                                continue;
                            }
                            //
                            // Check if outstanding unmatched for this ATM 
                            //
                            string SearchingStringLeft = " WHERE Operator ='" + WOperator + "'"
                                          //    + "' AND (RMCateg ='EWB102' OR RMCateg ='EWB103' OR RMCateg ='EWB104' OR RMCateg ='EWB105' OR RMCateg ='EWB106') "
                                          + " AND TerminalId ='" + WAtmNo + "'"
                                          + " AND ( IsMatchingDone = 1 AND SettledRecord = 0 AND ActionType <> '4') AND Matched = 0 ";

                            string WSortValue = "";

                            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, 1, SearchingStringLeft, WSortValue, NullPastDate, NullPastDate,2);

                            if (Mpa.RecordFound == true)
                            {
                                TotalAtmsWithUnMatchedRecords = TotalAtmsWithUnMatchedRecords + 1;
                                TotalUnMatchedRecords = TotalUnMatchedRecords + Mpa.TotalSelected;
                                // Outstanding unmatched 
                                Am.ReadAtmsMainSpecific(WAtmNo); // REad all fields 
                                Am.ExpressAction = false;
                                Am.Maker = "UnMatched";
                                Am.Authoriser = "N/A";
                                Am.UpdateAtmsMain(WAtmNo);
                                IsGood = false;
                                continue;
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

        //
        // 

        // find totals to report 

        public void ReadTotalsForATMsCashReconciliation(string InSignedId, int InSignRecordNo, string InOperator, int InAtmsReconcGroup)
        {
            //
            // Read all ATMs find the ones that are ready for reconciliation = Process =1 
            // Find out if Outstanding unmatched 
            // Do the turbo if needed 
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            TotalATMsReady = 0; // Replenishment Done and no UnMatched Records

            TotalErrorsAtATMs = 0; // Total Errors to be handle 

            TotalAmountErrors = 0; // Total Errors amount

            TotalAmountUnMatched = 0; // Total  

            TotalAtmsWithUnMatchedRecords = 0;
            TotalUnMatchedRecords = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
             + " FROM [dbo].[AtmsMain] "
             //+ " WHERE Operator = @Operator AND AuthUser = @AuthUser "
             + " WHERE Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup "
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
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", InAtmsReconcGroup);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            WAtmNo = (string)rdr["AtmNo"];
                            WSesNo = (int)rdr["ReplCycleNo"]; // this is the Repl No waiting antention is the 7759 and not the 7760 which the running one

                            GL_ReconcDiff = (bool)rdr["GL_ReconcDiff"];

                            LastUpdated = (DateTime)rdr["LastUpdated"];

                            Maker = (string)rdr["Maker"];

                            AuthUser = (string)rdr["AuthUser"];

                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                            if (Ta.RecordFound == true)
                            {
                                if (Ta.ProcessMode == 1 || (Ta.ProcessMode == 2 & Ta.Repl1.DiffRepl == true))
                                {
                                    TotalATMsReady = TotalATMsReady + 1;

                                    Er.ReadAllErrorsTableForCounters(WOperator, "", WAtmNo, WSesNo, "");

                                    TotalErrorsAtATMs = TotalErrorsAtATMs + Er.NumOfOpenErrorsLess100;

                                    TotalAmountErrors = TotalAmountErrors + Er.TotalErrorsAmtLess100;
                                }
                                else
                                {

                                }
                            }
                            //
                            // Check if outstanding unmatched for this ATM 
                            //
                            string SearchingStringLeft = " WHERE Operator ='" + WOperator + "'"
                                          //+ "' AND (RMCateg ='EWB102' OR RMCateg ='EWB103' OR RMCateg ='EWB104' OR RMCateg ='EWB105' OR RMCateg ='EWB106') "
                                          + " AND TerminalId ='" + WAtmNo + "'"
                                          + " AND ( IsMatchingDone = 1 AND SettledRecord = 0 AND ActionType <> '4') AND Matched = 0 ";

                            string WSortValue = "";
                            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, 1, SearchingStringLeft, WSortValue, NullPastDate, NullPastDate,2);
                            if (Mpa.RecordFound == true)
                            {
                                TotalAtmsWithUnMatchedRecords = TotalAtmsWithUnMatchedRecords + 1;
                                TotalUnMatchedRecords = TotalUnMatchedRecords + Mpa.TotalSelected;

                                TotalAmountUnMatched = TotalAmountUnMatched + Mpa.TotalAmountSelected;

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
       

    }
}



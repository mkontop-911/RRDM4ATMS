using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    class RRDMTurboReconcClass
    {
        // TURBO reconciliation applies to predefined error types like Presenter Error. 
        // SYSTEM examines that if with Turbo there is reconciliation the process finishes. 
        // Errors are set under Action and TRACES AND Main are updated. 

        public int TotTurboDone;
        public int TotTurboNonDone;

        string WAtmNo;
        int WSesNo;

     //   int SessionsInDiff;

        bool OffSite;

        DateTime LastReplDt;

        string TypeOfRepl;

        DateTime NextReplDt;

        bool ReconcDiff;

        int AtmsStatsGroup;

        int AtmsReplGroup;

        int AtmsReconcGroup;

      //  int AtmsReconcGroupOld;

        DateTime LastUpdated;

        int AuthUser;

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

      //  string WUserBankId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     //   bool WPrive;

        // READ Session Traces FOR TURBO RECONCILIATION 
        // 

      //  private void ReadSessionsStatusTracesForTurbo(int InChosenGroup)
        
public void ReadUpdateSessionsStatusTracesForTurbo(string InSignedId, int InSignRecordNo, string InOperator)
          
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
     //       WPrive = InPrive;
            
            TotTurboDone = 0;
            TotTurboNonDone = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

          
                SqlString = "SELECT *"
                    + " FROM [dbo].[AtmsMain] "
                    + " WHERE Operator = @Operator AND AuthUser = @AuthUser"
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
                            WSesNo = (int)rdr["CurrentSesNo"];

                            OffSite = (bool)rdr["OffSite"];
                            LastReplDt = (DateTime)rdr["LastReplDt"];

                            TypeOfRepl = (string)rdr["TypeOfRepl"];

                            NextReplDt = (DateTime)rdr["NextReplDt"];

                            ReconcDiff = (bool)rdr["ReconcDiff"];

                            AtmsStatsGroup = (int)rdr["AtmsStatsGroup"];
                            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
                            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];
                            LastUpdated = (DateTime)rdr["LastUpdated"];

                            AuthUser = (int)rdr["AuthUser"];

                            // STart Turbo process 

                            // Call Notes Balances with Parameter 5 to consider all errors in balances 

                            if (ReconcDiff == true)
                            {

                                int Process = 5;

                                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process);

                                //  If by considering all errors there is success in no difference then we update the ERRORS and Sessions 

                                if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false)
                                {
                                    TotTurboDone = TotTurboDone + 1;
                                    // REad errors one by one and update the Under action status 
                                    La.ReadAllErrorsTableForTurboAction(WOperator, WAtmNo); // READ AND UPDATE WITHIN 

                                    // UPDATE ATMS MAIN 
                                    Am.ReadAtmsMainSpecific(WAtmNo);
                                    Am.ReconcDiff = false;
                                    Am.LastUpdated = DateTime.Now;
                                    Am.UpdateAtmsMain(WAtmNo);

                                    // Create the transactions as a result of actions taken on Errors
                                    //
                                    La.ReadAllErrorsTableForPostingTrans(WOperator, WAtmNo, WSignedId, WSesNo);
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
    }
}

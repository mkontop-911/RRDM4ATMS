using System;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMCounterClass : Logger
    {
        public RRDMCounterClass() : base() { }

        // USED FOR ATM OPERATIONAL MONITORING 

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        public int TotAtms;
        public int TotRepl;
        public int TotNotRepl;
        public int TotUnderRepl;
        public int TotReconc;
        public int TotNotReconc1;
        public int TotNotReconc2;
        public int TotNotReconc3;
        public int TotErrors;
        public int TotErrorsAtm;
        public int TotErrorsHost;
        public int TotHostToday;

        public decimal TotDiffPlus;
        public decimal TotDiffMinus;


        public string CitId;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // 
        // Read ATMs Main for Replenishment and Reconciliation status and find totals 
        //
        public void ReadAtmsMainTotals(DateTime InWReplDt, string InOperator, string InCitId, int InReconcDays)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            TotAtms = 0;
            TotRepl = 0;
            TotNotRepl = 0;
            TotUnderRepl = 0;
            TotReconc = 0;
            TotNotReconc1 = 0;
            TotNotReconc2 = 0;
            TotNotReconc3 = 0; 
            TotErrors = 0;
            TotErrorsAtm = 0;
            TotErrorsHost = 0;
            TotHostToday = 0;

            TotDiffPlus = 0;
            TotDiffMinus = 0;
         //   string CitId;
            string AtmNo;

            int result1;

            int CurrentSesNo;

            DateTime LastReplDt;

            DateTime NextReplDt;

            bool GL_ReconcDiff;
          

            bool Consider = false;

            string SqlString = "SELECT *"
                   + " FROM [dbo].[AtmsMain] "
                   + " WHERE Operator=@Operator ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true; 
                            // Check wether is under consideration 
                            AtmNo = (string)rdr["AtmNo"];

                            CitId = (string)rdr["CitId"];

                            if (InCitId == CitId) // It is true even if CitId =0 
                            {
                                Consider = true;
                            }
                            else Consider = false;

                            Ac.ReadAtm(AtmNo);

                            if (InCitId !="1000") // WE ARE HAVING A CIT COOMPANY Check  it is for Replenishment 
                            {
                                Ua.ReadUsersAccessAtmTableSpecific(InCitId, "", Ac.AtmsReplGroup);
                                if (Ua.RecordFound == true & Ua.Replenishment == true)
                                {
                                    Consider = true; // We have A CIT ATM
                                }
                                else
                                {
                                    Consider = false; // We do not have A CIT ATM
                                }
                            }

                            if (Consider == true) // DO FOR ALL RELATED 
                            {
                                TotAtms = TotAtms + 1;

                                CurrentSesNo = (int)rdr["CurrentSesNo"];

                                LastReplDt = (DateTime)rdr["LastReplDt"];

                                NextReplDt = (DateTime)rdr["NextReplDt"];

                                GL_ReconcDiff = (bool)rdr["GL_ReconcDiff"];

                                if (GL_ReconcDiff == false)
                                {
                                    TotReconc = TotReconc + 1;
                                }
                                if (CurrentSesNo>0)
                                {
                                    Ta.ReadSessionsStatusTraces(AtmNo, CurrentSesNo);
                                    if (Ta.ProcessMode == -1 & Ta.RecordFound == true) // ATM is still in Process
                                    {
                                        // Find previous session Trace 

                                        Ta.ReadSessionsStatusTraces(AtmNo, Ta.PreSes);

                                    }

                                    if (Ta.RecordFound == true)
                                    {
                                        // Check the currently under Replenishment
                                        if (Ta.ProcessMode == 0)
                                        {
                                            TotUnderRepl = TotUnderRepl + 1;
                                        }

                                        // Check the reconciliations 
                                        if (Ta.SessionsInDiff == 1)
                                        {
                                            TotNotReconc1 = TotNotReconc1 + 1;
                                        }

                                        if (Ta.SessionsInDiff > 1)
                                        {
                                            TotNotReconc2 = TotNotReconc2 + 1;
                                        }

                                        if (Ta.SessionsInDiff >= InReconcDays)
                                        {
                                            TotNotReconc3 = TotNotReconc3 + 1;
                                        }

                                        if (Ta.SessionsInDiff > 0)
                                        {
                                            if (Ta.Diff1.DiffCurr1 > 0)
                                            {
                                                TotDiffPlus = TotDiffPlus + Ta.Diff1.DiffCurr1;
                                            }
                                            if (Ta.Diff1.DiffCurr1 < 0)
                                            {

                                                decimal Temp = -Ta.Diff1.DiffCurr1; // turn to possitive 
                                                TotDiffMinus = TotDiffMinus + Temp;
                                                Temp = 0;
                                            }
                                        }
                                    }

                                    Na.ReadAllErrorsTable(AtmNo, CurrentSesNo);

                                    if (Na.ErrorsFound == true)
                                    {
                                        TotErrors = TotErrors + Na.NumberOfErrJournal + Na.NumberOfErrHost;
                                        TotErrorsAtm = TotErrorsAtm + Na.NumberOfErrJournal;
                                        TotErrorsHost = TotErrorsHost + Na.NumberOfErrHost;
                                        TotHostToday = TotHostToday + Na.ErrHostToday;
                                    }

                                }


                                // If Next Repl date is today and last Repl date is less than today then = delayed Repl 
                                // Next repl > today and and last repl = today => Repl doone today
                                // Next repl > today and last repl < today then not in question = do nothing 

                                if (NextReplDt != NullPastDate)
                                {
                                    result1 = DateTime.Compare(LastReplDt.Date, InWReplDt.Date);
                                    if (result1 == 0) // Equal dates 
                                    {
                                        //  Repl Done today 

                                        TotRepl = TotRepl + 1;
                                    }

                                    result1 = DateTime.Compare(NextReplDt.Date, InWReplDt.Date);

                                    if (result1 == 0 || result1 < 0) // Equal dates or less
                                    {
                                        // Not done Repl

                                        TotNotRepl = TotNotRepl + 1;
                                    }
                                    if (result1 > 0) // Bigger than today
                                    {
                                        // Future Repl
                                    }

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

                    CatchDetails(ex); 

                }
        }

       
    }
}



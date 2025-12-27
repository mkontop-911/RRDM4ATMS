using System;
using System.Text;
// using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
//multilingual

namespace RRDM4ATMs
{
    public class RRDMSessionsNewSession : Logger
    {
        public RRDMSessionsNewSession() : base() { }

        public string WAtmNo;

        public string WUser;

        public int NewSessionNo;
        public int InProcessSesNo;
        public int LastSesNo;

        public bool ZeroOccurance;
        public int SesNoAfterZero;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        int SessionsInDiff;
        int PreSes; // Previous to last 

        DateTime LongDateInPast = new DateTime(1900, 01, 01);

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMSessionsPhysicalInspection Pi = new RRDMSessionsPhysicalInspection();
        //
        //
        // NEW SESSION 
        // 
        // create records for starting a new Session 
        //
        int WLoadedAtRMCycle; 
        public void CreateNewSession(string InAtmNo, string InUser, DateTime InTRanDate,
            int InTraceNo , int InLoadedAtRMCycle)
        {
            WAtmNo = InAtmNo;
            WUser = InUser;
            WLoadedAtRMCycle = InLoadedAtRMCycle; 
            // fill rest of record 

            Ac.ReadAtm(WAtmNo); // Get Information neaded to create the Trace and the NOTES 

            if (Ac.RecordFound == false)
            {
                //  MessageBox.Show("ATM Not Found in DataBases! Please enter correct ATM number ");
                return;
            }

            bool PreviousSesWasZero; 

            ReadSessionsStatusTracesLastNo(WAtmNo); // Get the Last Session Number  

            if (RecordFound == false)
            {
                // Initialise 
                PreviousSesWasZero = true; 
                LastSesNo = 0;
            }
            else
            {
                PreviousSesWasZero = false;
            }

            NewSessionNo = InsertSessionsStatusTraces(WAtmNo);

            if (PreviousSesWasZero == true)
            {             
                ZeroOccurance = true; 
                SesNoAfterZero = NewSessionNo;
            }

            // Find key 

            /*ReadSessionsStatusTracesRecordNo(WAtmNo);*/ // To find the in process Ses No to open a new Notes 

            // return NewSessionNo

            InsertSessionsNotesAndValues(NewSessionNo);

            InsertPhysicalInspectionRecords(WAtmNo, NewSessionNo);

            Ta.ReadSessionsStatusTraces(WAtmNo, NewSessionNo);
     
            //  textBox2.Text = NewSessionNo.ToString();

            Ta.AtmNo = WAtmNo;
            Ta.PreSes = LastSesNo;
            Ta.NextSes = 0;
            Ta.BankId = Ac.BankId;

            Ta.FirstTraceNo = InTraceNo;

            Ta.AtmName = Ac.AtmName;
            Ta.RespBranch = Ac.Branch;
            Ta.AtmsStatsGroup = Ac.AtmsStatsGroup;
            Ta.AtmsReplGroup = Ac.AtmsReplGroup;
            Ta.AtmsReconcGroup = Ac.AtmsReconcGroup;
            Ta.Repl1.SignIdRepl = WUser; // ............
            Ta.Repl1.StartRepl = true;
            Ta.Repl1.FinishRepl = false;
            Ta.Repl1.StepLevel = 0;
            Ta.SessionsInDiff = SessionsInDiff;

            Ta.SesDtTimeStart = InTRanDate;
            Ta.SesDtTimeEnd = LongDateInPast;
            Ta.Repl1.ReplStartDtTm = LongDateInPast;
            Ta.Repl1.ReplFinDtTm = LongDateInPast;

            //Ta.Repl1.ReplStartDtTm = InTRanDate;
            //Ta.Repl1.ReplFinDtTm = InTRanDate;

            Ta.Repl1.NextRepDtTm = LongDateInPast;
            Ta.Recon1.SignIdReconc = "";
            Ta.Recon1.RecStartDtTm = LongDateInPast;
            Ta.Recon1.RecFinDtTm = LongDateInPast;

            Ta.Diff1.CurrNm1 = "N/A";
            Ta.Diff1.CurrNm2 = "N/A";
            Ta.Diff1.CurrNm3 = "N/A";
            Ta.Diff1.CurrNm4 = "N/A";
            Ta.InNeedType = 0;

            Ta.Is_Updated_GL = false;
            Ta.Last = false;
            Ta.InProcess = true;

            Ta.ProcessMode = -1;

            Ta.ReplGenComment = "N/A";

            Ta.UpdateSessionsStatusTraces(WAtmNo, NewSessionNo);

            // Update Old Session Traces with New Session No

            Ta.ReadSessionsStatusTraces(WAtmNo, LastSesNo);
            Ta.NextSes = NewSessionNo;
            Ta.UpdateSessionsStatusTraces(WAtmNo, LastSesNo);

            // Update Old Session Notes with New Session No
            if (LastSesNo>0)
            {
                // It has an old session is not a new atm
                UpdateSessionsNotesAndValues(WAtmNo, LastSesNo);
            }
          
            //
            // UPDATE ATMS MAIN
            //

            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

            Am.ReadAtmsMainSpecific(WAtmNo);

            Am.CurrentSesNo = NewSessionNo;
            if (LastSesNo == 0)
            {
                Am.LastReplDt = InTRanDate;
            }

            Am.LastUpdated = DateTime.Now;

            Am.UpdateAtmsMain(WAtmNo);

        }

        //
        // READ Last SESSION TRACE to find last Session No 
        //
        //
        public void ReadSessionsStatusTracesLastNo(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString =
                "SELECT * "
                + " FROM [dbo].[SessionsStatusTraces] "
                + " WHERE AtmNo = @AtmNo "
                + " Order by SesNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            // Assign Values
                            RecordFound = true;

                            LastSesNo = (int)rdr["SesNo"];
                            PreSes = (int)rdr["PreSes"];
                            SessionsInDiff = (int)rdr["SessionsInDiff"];

                            break; // When found stop processing


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
        // READ Last SESSION TRACE to find last Session No which is process 
        //
        //
        //public void ReadSessionsStatusTracesRecordNo(string InAtmNo)
        //{
        //    RecordFound = false;

        //    string SqlString =
        //        "SELECT [SesNo]"
        //        + " FROM [dbo].[SessionsStatusTraces] "
        //        + " WHERE AtmNo = @AtmNo";

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {

        //                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;
        //                    InProcessSesNo = (int)rdr["SesNo"];// Read second time 
        //                    NewSessionNo = InProcessSesNo;
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

        //            RRDMLog4Net Log = new RRDMLog4Net();

        //            StringBuilder WParameters = new StringBuilder();

        //            WParameters.Append("User : ");
        //            WParameters.Append("NotAssignYet");
        //            WParameters.Append(Environment.NewLine);

        //            WParameters.Append("ATMNo : ");
        //            WParameters.Append("NotDefinedYet");
        //            WParameters.Append(Environment.NewLine);

        //            string Logger = "RRDM4Atms";
        //            string Parameters = WParameters.ToString();

        //            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

        //            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
        //                + " . Application will be aborted! Call controller to take care. ");

        //            Environment.Exit(0);

        //        }
        //}
        // INSERT SESSION TRACES
        int SeqNo;
        private int InsertSessionsStatusTraces(string AtmNo)
        {

            Ac.ReadAtm(WAtmNo);

            ErrorFound = false;
            ErrorOutput = "";

            //RunSession = true;
            string cmdinsert = "INSERT INTO [SessionsStatusTraces] "
                        + "([AtmNo],[InProcess],[LoadedAtRMCycle] ,[Operator]) "
                        + " VALUES "
                            + "(@AtmNo,@InProcess,@LoadedAtRMCycle ,@Operator) "
                            + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        // Header 
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@InProcess", 1);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", WLoadedAtRMCycle);
                        cmd.Parameters.AddWithValue("@Operator", Ac.Operator);

                        //rows number of record got updated

                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }

            return SeqNo;
        }



        // INSERT SESSION NOTES
        private void InsertSessionsNotesAndValues(int InSessionNo)
        {

            Ac.ReadAtm(WAtmNo);

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [SessionsNotesAndValues] "
                 + "([SesNo],[AtmNo],[PreSes],[NextSes],[AtmName],"
                 + "[BankId],[RespBranch],[AtmGroup],"
                 + "[CaptCardsMachine],[CaptCardsCount],"
                 + "[ReplMethod],[InUserDate],[InReplAmount],"
             + "[CasNo_51],[CurName_51],[FaceValue_51],"
             + "[InNotes_51],[DispNotes_51],[RejNotes_51],[RemNotes_51],[CasCount_51],[RejCount_51],"
             + "[NewInSuggest_51],[NewInUser_51],"
             + "[CasNo_52],[CurName_52],[FaceValue_52],"
             + "[InNotes_52],[DispNotes_52],[RejNotes_52],[RemNotes_52],[CasCount_52],[RejCount_52],"
             + "[NewInSuggest_52],[NewInUser_52],"
             + "[CasNo_53],[CurName_53],[FaceValue_53],"
              + "[InNotes_53],[DispNotes_53],[RejNotes_53],[RemNotes_53],[CasCount_53],[RejCount_53],"
              + "[NewInSuggest_53],[NewInUser_53],"
               + "[CasNo_54],[CurName_54],[FaceValue_54],"
              + "[InNotes_54],[DispNotes_54],[RejNotes_54],[RemNotes_54],[CasCount_54],[RejCount_54],"
              + "[NewInSuggest_54],[NewInUser_54],"
              + "[DepCurNm],"
              + "[DepTransMach],[DepNotesMach],[DepAmountMach],[DepNotesRejMach],[DepAmountRejMach],"
              + "[EnvelopsMach],[EnvAmountMach],"
              + "[DepTransCount],[DepNotesCount],[DepAmountCount],[DepNotesRejCount],[DepAmountRejCount],"
               + "[EnvelopsCount],[EnvAmountCount],"
              + "[ChequesTransMach],[ChequesNoMach],[ChequesAmountMach],"
              + "[ChequesTransCount],[ChequesNoCount],[ChequesAmountCount],"
              + "[ReplToReplAmt_1],[ReplToReplAmt_2],[ReplToReplAmt_3],[ReplToReplAmt_4],"
              + "[ReplAmountSuggest],[ReplAmountTotal],[ReplUserComment],"
              + " [Passed],[Last],[InProcess],[LoadedAtRMCycle],[Operator] ) "
             + " VALUES"
              + "(@SesNo, @AtmNo, @PreSes,@NextSes,@AtmName,@BankId,@RespBranch, @AtmGroup,"
              + "@CaptCardsMachine,@CaptCardsCount,"
              + "@ReplMethod,@InUserDate,@InReplAmount,"
             + "@CasNo_51,@CurName_51,@FaceValue_51,"
             + "@InNotes_51,@DispNotes_51,@RejNotes_51,@RemNotes_51,@CasCount_51,@RejCount_51,"
             + "@NewInSuggest_51,@NewInUser_51,"
             + "@CasNo_52,@CurName_52,@FaceValue_52,"
             + "@InNotes_52,@DispNotes_52,@RejNotes_52,@RemNotes_52,@CasCount_52,@RejCount_52,"
             + "@NewInSuggest_52,@NewInUser_52,"
             + "@CasNo_53,@CurName_53,@FaceValue_53,"
              + "@InNotes_53,@DispNotes_53,@RejNotes_53,@RemNotes_53,@CasCount_53,@RejCount_53,"
              + "@NewInSuggest_53,@NewInUser_53,"
               + "@CasNo_54,@CurName_54,@FaceValue_54,"
              + "@InNotes_54,@DispNotes_54,@RejNotes_54,@RemNotes_54,@CasCount_54,@RejCount_54,"
              + "@NewInSuggest_54,@NewInUser_54,"
               + "@DepCurNm,"
              + "@DepTransMach,@DepNotesMach,@DepAmountMach,@DepNotesRejMach,@DepAmountRejMach,"
              + "@EnvelopsMach,@EnvAmountMach,"
              + "@DepTransCount,@DepNotesCount,@DepAmountCount,@DepNotesRejCount,@DepAmountRejCount,"
               + "@EnvelopsCount,@EnvAmountCount,"
              + "@ChequesTransMach,@ChequesNoMach,@ChequesAmountMach,"
              + "@ChequesTransCount,@ChequesNoCount,@ChequesAmountCount,"
              + "@ReplToReplAmt_1,@ReplToReplAmt_2,@ReplToReplAmt_3,@ReplToReplAmt_4,"
                + "@ReplAmountSuggest,@ReplAmountTotal,@ReplUserComment,"
              + " @Passed, @Last, @InProcess ,@LoadedAtRMCycle, @Operator) ";

            using (SqlConnection conn =
            new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@SesNo", InSessionNo); // The key 

                        // Header 

                        cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                        cmd.Parameters.AddWithValue("@PreSes", LastSesNo);
                        cmd.Parameters.AddWithValue("@NextSes", 0);
                        cmd.Parameters.AddWithValue("@AtmName", Ac.AtmName);

                        cmd.Parameters.AddWithValue("@BankId", Ac.BankId);

                        cmd.Parameters.AddWithValue("@RespBranch", Ac.Branch);
                        cmd.Parameters.AddWithValue("@AtmGroup", Ac.AtmsStatsGroup);

                        cmd.Parameters.AddWithValue("@CaptCardsMachine", 0); // Cassette 1
                        cmd.Parameters.AddWithValue("@CaptCardsCount", 0);

                        cmd.Parameters.AddWithValue("@ReplMethod", 0);
                        cmd.Parameters.AddWithValue("@InUserDate", LongDateInPast);
                        cmd.Parameters.AddWithValue("@InReplAmount", 0);

                        cmd.Parameters.AddWithValue("@CasNo_51", 1); // Cassette 1

                        cmd.Parameters.AddWithValue("@CurName_51", Ac.CurName_11);
                        cmd.Parameters.AddWithValue("@FaceValue_51", Ac.FaceValue_11);
                        cmd.Parameters.AddWithValue("@InNotes_51", 0);
                        cmd.Parameters.AddWithValue("@DispNotes_51", 0);
                        cmd.Parameters.AddWithValue("@RejNotes_51", 0);
                        cmd.Parameters.AddWithValue("@RemNotes_51", 0);
                        cmd.Parameters.AddWithValue("@CasCount_51", 0);
                        cmd.Parameters.AddWithValue("@RejCount_51", 0);
                        cmd.Parameters.AddWithValue("@NewInSuggest_51", 0);
                        cmd.Parameters.AddWithValue("@NewInUser_51", -1);

                        cmd.Parameters.AddWithValue("@CasNo_52", 2); // Cassette 2

                        cmd.Parameters.AddWithValue("@CurName_52", Ac.CurName_12);
                        cmd.Parameters.AddWithValue("@FaceValue_52", Ac.FaceValue_12);
                        cmd.Parameters.AddWithValue("@InNotes_52", 0);
                        cmd.Parameters.AddWithValue("@DispNotes_52", 0);
                        cmd.Parameters.AddWithValue("@RejNotes_52", 0);
                        cmd.Parameters.AddWithValue("@RemNotes_52", 0);
                        cmd.Parameters.AddWithValue("@CasCount_52", 0);
                        cmd.Parameters.AddWithValue("@RejCount_52", 0);
                        cmd.Parameters.AddWithValue("@NewInSuggest_52", 0);
                        cmd.Parameters.AddWithValue("@NewInUser_52", -1);

                        cmd.Parameters.AddWithValue("@CasNo_53", 3); // Cassette 3

                        cmd.Parameters.AddWithValue("@CurName_53", Ac.CurName_13);
                        cmd.Parameters.AddWithValue("@FaceValue_53", Ac.FaceValue_13);
                        cmd.Parameters.AddWithValue("@InNotes_53", 0);
                        cmd.Parameters.AddWithValue("@DispNotes_53", 0);
                        cmd.Parameters.AddWithValue("@RejNotes_53", 0);
                        cmd.Parameters.AddWithValue("@RemNotes_53", 0);
                        cmd.Parameters.AddWithValue("@CasCount_53", 0);
                        cmd.Parameters.AddWithValue("@RejCount_53", 0);
                        cmd.Parameters.AddWithValue("@NewInSuggest_53", 0);
                        cmd.Parameters.AddWithValue("@NewInUser_53", -1);


                        cmd.Parameters.AddWithValue("@CasNo_54", 4); // Cassette 4

                        cmd.Parameters.AddWithValue("@CurName_54", Ac.CurName_14);
                        cmd.Parameters.AddWithValue("@FaceValue_54", Ac.FaceValue_14);
                        cmd.Parameters.AddWithValue("@InNotes_54", 0);
                        cmd.Parameters.AddWithValue("@DispNotes_54", 0);
                        cmd.Parameters.AddWithValue("@RejNotes_54", 0);
                        cmd.Parameters.AddWithValue("@RemNotes_54", 0);
                        cmd.Parameters.AddWithValue("@CasCount_54", 0);
                        cmd.Parameters.AddWithValue("@RejCount_54", 0);
                        cmd.Parameters.AddWithValue("@NewInSuggest_54", 0);
                        cmd.Parameters.AddWithValue("@NewInUser_54", -1);


                        cmd.Parameters.AddWithValue("@DepCurNm", Ac.DepCurNm);

                        cmd.Parameters.AddWithValue("@DepTransMach", 0);
                        cmd.Parameters.AddWithValue("@DepNotesMach", 0);
                        cmd.Parameters.AddWithValue("@DepAmountMach", 0);
                        cmd.Parameters.AddWithValue("@DepNotesRejMach", 0);
                        cmd.Parameters.AddWithValue("@DepAmountRejMach", 0);

                        cmd.Parameters.AddWithValue("@EnvelopsMach", 0); // 
                        cmd.Parameters.AddWithValue("@EnvAmountMach", 0);

                        cmd.Parameters.AddWithValue("@DepTransCount", 0);
                        cmd.Parameters.AddWithValue("@DepNotesCount", 0);
                        cmd.Parameters.AddWithValue("@DepAmountCount", 0);
                        cmd.Parameters.AddWithValue("@DepNotesRejCount", 0);
                        cmd.Parameters.AddWithValue("@DepAmountRejCount", 0);

                        cmd.Parameters.AddWithValue("@EnvelopsCount", 0); // 
                        cmd.Parameters.AddWithValue("@EnvAmountCount", 0);

                        cmd.Parameters.AddWithValue("@ChequesTransMach", 0);
                        cmd.Parameters.AddWithValue("@ChequesNoMach", 0);
                        cmd.Parameters.AddWithValue("@ChequesAmountMach", 0);

                        cmd.Parameters.AddWithValue("@ChequesTransCount", 0);
                        cmd.Parameters.AddWithValue("@ChequesNoCount", 0);
                        cmd.Parameters.AddWithValue("@ChequesAmountCount", 0);

                        cmd.Parameters.AddWithValue("@ReplToReplAmt_1", 0);
                        cmd.Parameters.AddWithValue("@ReplToReplAmt_2", 0);
                        cmd.Parameters.AddWithValue("@ReplToReplAmt_3", 0);
                        cmd.Parameters.AddWithValue("@ReplToReplAmt_4", 0);

                        //bool Temp = false;

                        cmd.Parameters.AddWithValue("@ReplAmountSuggest", 0);
                        cmd.Parameters.AddWithValue("@ReplAmountTotal", 0);

                        cmd.Parameters.AddWithValue("@ReplUserComment", "");

                        // Trailer 
                        cmd.Parameters.AddWithValue("@Passed", 0); // False  
                        cmd.Parameters.AddWithValue("@Last", 0); // False  
                        cmd.Parameters.AddWithValue("@InProcess", 1); // True
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", WLoadedAtRMCycle); // True
                        cmd.Parameters.AddWithValue("@Operator", Ac.Operator);
                        //rows number of record got updated

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

                    CatchDetails(ex);

                }
        }

        // UPDATE Previous Session with next one 
        public void UpdateSessionsNotesAndValues(string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsNotesAndValues  SET "
                           + " [NextSes] = @NextSes"
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@NextSes", NewSessionNo);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //  textBoxMsg.Text = " ATMs Table UPDATED ";
                        }

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
        // INSERT Physical Inspection Records For the ATM and Ses No
        // 
        private void InsertPhysicalInspectionRecords(string InAtmNo, int InSessionNo)
        {
            Ac.ReadAtm(WAtmNo);
            string ParamId = "212"; // Physical Inspection Data 
            Gp.ReadAllOccurancesNmsForSpecificParDataTable(Ac.BankId, ParamId);
            //Gp.DataTableOccurancesIds
            int I = 0;

            while (I <= (Gp.DataTableOccurancesIds.Rows.Count - 1))
            {

                RecordFound = true;

                // GET Table fields - Line by Line
                //

                Pi.AtmNo = InAtmNo;
                Pi.SesNo = InSessionNo;
                Pi.Selection = false;

                Pi.InspectionId = (string)Gp.DataTableOccurancesIds.Rows[I]["OccuranceId"];
                Pi.PhysicalInspectionNm = (string)Gp.DataTableOccurancesIds.Rows[I]["OccuranceNm"];

                Pi.InsertSessionsPhysicalInspectionRecord(WLoadedAtRMCycle);

                I++; // Read Next entry of the table 
            }
        }

     

      
    }

}





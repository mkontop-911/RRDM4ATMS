using System;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;
//multilingual


namespace RRDM4ATMs
{
    public class RRDMSessionsNotesBalances : Logger
    {
        public RRDMSessionsNotesBalances() : base() { }
        /// <summary>
        /// Working Function 1 = Reconciliation from Repl to repl
        /// Working Function 2 = Show status of differences For ATM Only (No Host)
        /// Working Function 3 = Show status of differences For ATM and HOST
        /// Working Function 4 = Show status after correction was made - include actions on errors 
        /// Working Function 5 = Show status if action was taken on all errors 
        /// Working Function 12 = CommingFromSM_Master - we turn it to 2 but we do not call find errors
        /// </summary>
        /// 

        bool CommingFromSM_Master; 

        public int FirstTraceNo;
        public int LastTraceNo;
        // Working variables

        string WAtmNo;
        int WReplCycle;
        int WTranNo;
        int WFunction;

       // string WOperator;

        public bool ErrorsFound; // Related to errors in journal 

        public int SesNo;
        public string AtmNo;

        public int PreSes;
        public int NextSes;

        public string AtmName;
        public string BankId;

        public string RespBranch;

        public int CaptCardsMachine;
        public int CaptCardsCount;

        public int ReplMethod;
        public DateTime InUserDate;
        public decimal InReplAmount; // Loaded Amount

        public decimal ReplAmountTotal;

        public decimal ReplAmountSuggest;

        public decimal InsuranceAmount;

        public string ReplUserComment;
        //
        // CIT Fields 
        //
        public DateTime Cit_ExcelUpdatedDate;
        public decimal Cit_UnloadedCounted;
        public decimal Cit_Over; // Found over amount 
        public decimal Cit_Short; // Found Short amount 
        public decimal Cit_Loaded; // Loaded Amount 

        public bool IsNewAtm;  

        public decimal GL_Balance; 

        public bool Is_GL_Adjusted; // It is true if it is adjusted 

        public decimal GL_Bal_Repl_Adjusted; // Last Nights GL Balance adjusted at to the point of Unloaded Cash 

        public bool DiffAtAtmLevel_Cit; // Set while reading the excel 
        public bool DiffAtHostLevel_Cit;
        public bool DiffWithErrors_Cit;

        public struct Cassettes
        {

            public int CasNo; // CasNo
            public string CurName; // Currency Name 
            public decimal FaceValue; // Face Value
            public int InNotes;
            public int DispNotes;
            public int RejNotes; // Rejected Notes 
            public int RemNotes; // Remaining Notes 
            public int CasCount;
            public int RejCount;
            public int DiffCas;
            public int DiffRej;
            public int NewInSuggest;
            public int NewInUser;
        };

        public Cassettes Cassettes_1;  // Cassette 1 
        public Cassettes Cassettes_2; // Cassette 2
        public Cassettes Cassettes_3; // Cassette 3
        public Cassettes Cassettes_4; // Cassette 4
        public Cassettes Cassettes_5; // Cassette 5

        public int ActiveCassettesNo;


        bool One; bool Two; bool Three; bool Four; bool Five;

        public struct Balances
        {
            public string CurrNm;
            public decimal OpenBal;
            public decimal CountedBal;
            public decimal MachineBal;
            public decimal ReplToRepl;
            public decimal HostBal;
            public decimal HostBalAdj;
            public int NubOfErr;
            public int ErrOutstanding;
            public decimal PresenterValue;
        };

        public Balances Balances1; // Declare Balances1 of type Balances .. Currency one 
        public Balances Balances2; // Declare Balances2 of type Balances ... Currency two
        public Balances Balances3; // Declare Balances3 of type Balances .... Currency three
        public Balances Balances4; // Declare Balances4 of type Balances .... Currency four 
        public Balances Balances5; // Declare Balances5 of type Balances .... Currency five

        public int BalSets; // How Many Balances we have

        public struct BalDiff // DIFFE IN BALANCES 
        {
            public string CurrNm;
            public decimal Machine;
            public decimal ReplToRepl;
            public decimal Host;
            public decimal HostAdj;
            public bool AtmLevel;
            public bool HostLevel;
        };

        public BalDiff BalDiff1; // Declare BalDiff1 of type BalDiff .. Currency one 
        public BalDiff BalDiff2; // Declare BalDiff1 of type BalDiff ... Currency two
        public BalDiff BalDiff3; // Declare BalDiff1 of type BalDiff .... Currency three
        public BalDiff BalDiff4; // Declare BalDiff1 of type BalDiff .... Currency four 

        // DIFFERENCES IN BALANCES PER CURRENCY 


        public string HCurrNm1; decimal HBal1;

        string HCurrNm2; decimal HBal2;

        string HCurrNm3; decimal HBal3;

        string HCurrNm4; decimal HBal4;

        public bool DiffAtHostLevel;
        public bool DiffAtAtmLevel;
        public bool DiffWithErrors;

        // ADJUSTING HOST BALANCES 
        // FIND LAST TRACES PER SYSTEM 

        public struct SystemTargets // Target Systems .. Each one updates GL at different times 
        {
            //     public int TargetNo;
            public string Name;
            public int LastTrace;
            public DateTime DateTm;
        };

        public SystemTargets SystemTargets1; // Structure for this target 
        public SystemTargets SystemTargets2; // Structure for this target 
        public SystemTargets SystemTargets3; // Structure for this target 
        public SystemTargets SystemTargets4; // Structure for this target 
        public SystemTargets SystemTargets5; // Structure for this target 

        public int SystemTarget; // 1 = AMX, 2 = JCC etc 

        bool ValidTran;
        public int MinTraceTarget; // Trace Targets for sub-systems 
        public int MaxTraceTarget;

        decimal HAdjBal1; decimal HAdjBal2; decimal HAdjBal3; decimal HAdjBal4; // Adjusted Host Balances 

        decimal ErrAmount;

        public int NumberOfErrors;
        public int NumberOfErrJournal;
        public int ErrJournalThisCycle;
        public int NumberOfErrDep;
        public int NumberOfErrHost;
        public int ErrHostToday;
        public int ErrOutstanding; // Action was not taken on them 
        public int ErrorsAdjastingBalances;

        // Batch from Host fields 

        public int HBatchNo;
        public DateTime HDtTmStart;
        public DateTime HDtTmEnd;

        //   public DateTime LastHostTranDtTm;

        public string Operator;


        bool Na_RecordFound = false; 
        //public PhysicalCheck PhysicalCheck1; // Declare PhysicalCheck1 of type PhysicalCheck .. 

        // END of Declarations 
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        //    string outcome = ""; // TO FACILITATE EXCEPTIONS 

        // END OF NEW METHODOLOGY 

        //  string WUserBank; 
        // READ Session Notes AND CALCULATE BALANCES 
        //
        public void ReadSessionsNotesAndValues(string InAtmNo, int InSesNo, int InFunction)
        {
            /// Working Function 1 = Reconciliation from Repl to repl
            /// Working Function 2 = Show status of differences For ATM Only (No Host)
            /// Working Function 3 = Show status of differences For ATM and HOST
            /// Working Function 4 = Show status after correction was made - include actions on errors 
            /// Working Function 5 = Show status if action was taken on all errors - include Presenter Errors
            /// Working Function 12 = CommingFromSM_Master - we turn it to 2 but we do not call find errors
            /// 
            // initialise variables
            try
            {
                if ((InAtmNo == null || InAtmNo == "") || InSesNo == 0 || InFunction == 0)
                {
                    if (Environment.UserInteractive)
                    {
                       System.Windows.Forms.MessageBox.Show("Wrong input in Class Call. Coming from Notes and Value");
                       return;
                    }
                    else
                    {
                        //RRDM_Custom_Exceptions RrdmEx = new RRDM_Custom_Exceptions("ExMessage");
                        //RrdmEx.Source = "RRDMSessionsNotesBalances";
                        //RrdmEx.RRDMCode = 987;
                        //RrdmEx.RRDMFatal = false;
                        //throw (RrdmEx);
                        // CatchDetails(ex);
                        //catch (RRDM_Custom_Exceptions ex)
                        if (InAtmNo == null || InAtmNo == "") WAtmNo = "NotDefined";
                        else WAtmNo = InAtmNo; 
                        int a = 4;
                        int b = a / 0;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                CatchDetails(ex, WAtmNo);
                return; 
            }
            WAtmNo = InAtmNo;
            WReplCycle = InSesNo;
            if (InFunction == 12)
            {
                WFunction = 2;
                CommingFromSM_Master = true; 
            }
            else
            {
                WFunction = InFunction;
                CommingFromSM_Master = false;
            }
            
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
         
            string SqlString = "SELECT *"
                                + " FROM [ATMS].[dbo].[SessionsNotesAndValues] "
                                + " WHERE SesNo = @SesNo AND AtmNo=@AtmNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", WReplCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            // RecordFound = true; // It is used throughout the Class
                            Na_RecordFound = true; 

                            SesNo = (int)rdr["SesNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            PreSes = (int)rdr["PreSes"];
                            NextSes = (int)rdr["NextSes"];

                            AtmName = (string)rdr["AtmName"];
                            BankId = (string)rdr["BankId"];

                            RespBranch = (string)rdr["RespBranch"];

                            ReplUserComment = (string)rdr["ReplUserComment"];

                            ReplAmountSuggest = (decimal)rdr["ReplAmountSuggest"];
                            ReplAmountTotal = (decimal)rdr["ReplAmountTotal"];

                            InsuranceAmount = (decimal)rdr["InsuranceAmount"];

                            // Capture cards 

                            CaptCardsMachine = (int)rdr["CaptCardsMachine"];
                            CaptCardsCount = (int)rdr["CaptCardsCount"];

                            ReplMethod = (int)rdr["ReplMethod"];
                            InUserDate = (DateTime)rdr["InUserDate"];
                            InReplAmount = (decimal)rdr["InReplAmount"];

                            // CIT , Like G4S data

                            Cit_ExcelUpdatedDate = (DateTime)rdr["Cit_ExcelUpdatedDate"];

                            Cit_UnloadedCounted = (decimal)rdr["Cit_UnloadedCounted"];
                            Cit_Over = (decimal)rdr["Cit_Over"];
                            Cit_Short = (decimal)rdr["Cit_Short"];
                            Cit_Loaded = (decimal)rdr["Cit_Loaded"];

                            GL_Balance = (decimal)rdr["GL_Balance"];
                            IsNewAtm = (bool)rdr["IsNewAtm"];
                            Is_GL_Adjusted = (bool)rdr["Is_GL_Adjusted"];
                            GL_Bal_Repl_Adjusted = (decimal)rdr["GL_Bal_Repl_Adjusted"];

                            DiffAtAtmLevel_Cit = (bool)rdr["DiffAtAtmLevel_Cit"];
                            DiffAtHostLevel_Cit = (bool)rdr["DiffAtHostLevel_Cit"];
                            DiffWithErrors_Cit = (bool)rdr["DiffWithErrors_Cit"];

                            // WHAT CASSETTES CONTAIN - FOUR CASSETTES 

                            // Assign Values to 51
                            Cassettes_1.CasNo = (int)rdr["CasNo_51"];
                            Cassettes_1.CurName = rdr["CurName_51"].ToString();
                            Cassettes_1.FaceValue = (decimal)rdr["FaceValue_51"];
                            Cassettes_1.InNotes = (int)rdr["InNotes_51"];
                            Cassettes_1.DispNotes = (int)rdr["DispNotes_51"];
                            Cassettes_1.RejNotes = (int)rdr["RejNotes_51"];
                            Cassettes_1.RemNotes = (int)rdr["RemNotes_51"];
                            Cassettes_1.CasCount = (int)rdr["CasCount_51"];
                            Cassettes_1.RejCount = (int)rdr["RejCount_51"];
                            Cassettes_1.NewInSuggest = (int)rdr["NewInSuggest_51"];
                            Cassettes_1.NewInUser = (int)rdr["NewInUser_51"];

                            // Assign Values to 52
                            Cassettes_2.CasNo = (int)rdr["CasNo_52"];
                            Cassettes_2.CurName = rdr["CurName_52"].ToString();
                            Cassettes_2.FaceValue = (decimal)rdr["FaceValue_52"];
                            Cassettes_2.InNotes = (int)rdr["InNotes_52"];
                            Cassettes_2.DispNotes = (int)rdr["DispNotes_52"];
                            Cassettes_2.RejNotes = (int)rdr["RejNotes_52"];
                            Cassettes_2.RemNotes = (int)rdr["RemNotes_52"];
                            Cassettes_2.CasCount = (int)rdr["CasCount_52"];
                            Cassettes_2.RejCount = (int)rdr["RejCount_52"];
                            Cassettes_2.NewInSuggest = (int)rdr["NewInSuggest_52"];
                            Cassettes_2.NewInUser = (int)rdr["NewInUser_52"];

                            // Assign Values to 53
                            Cassettes_3.CasNo = (int)rdr["CasNo_53"];
                            Cassettes_3.CurName = rdr["CurName_53"].ToString();
                            Cassettes_3.FaceValue = (decimal)rdr["FaceValue_53"];
                            Cassettes_3.InNotes = (int)rdr["InNotes_53"];
                            Cassettes_3.DispNotes = (int)rdr["DispNotes_53"];
                            Cassettes_3.RejNotes = (int)rdr["RejNotes_53"];
                            Cassettes_3.RemNotes = (int)rdr["RemNotes_53"];
                            Cassettes_3.CasCount = (int)rdr["CasCount_53"];
                            Cassettes_3.RejCount = (int)rdr["RejCount_53"];
                            Cassettes_3.NewInSuggest = (int)rdr["NewInSuggest_53"];
                            Cassettes_3.NewInUser = (int)rdr["NewInUser_53"];

                            // Assign Values to 54
                            Cassettes_4.CasNo = (int)rdr["CasNo_54"];
                            Cassettes_4.CurName = rdr["CurName_54"].ToString();
                            Cassettes_4.FaceValue = (decimal)rdr["FaceValue_54"];
                            Cassettes_4.InNotes = (int)rdr["InNotes_54"];
                            Cassettes_4.DispNotes = (int)rdr["DispNotes_54"];
                            Cassettes_4.RejNotes = (int)rdr["RejNotes_54"];
                            Cassettes_4.RemNotes = (int)rdr["RemNotes_54"];
                            Cassettes_4.CasCount = (int)rdr["CasCount_54"];
                            Cassettes_4.RejCount = (int)rdr["RejCount_54"];
                            Cassettes_4.NewInSuggest = (int)rdr["NewInSuggest_54"];
                            Cassettes_4.NewInUser = (int)rdr["NewInUser_54"];

                            // Assign Values to 55
                            Cassettes_5.CasNo = (int)rdr["CasNo_55"];
                            Cassettes_5.CurName = rdr["CurName_55"].ToString();
                            Cassettes_5.FaceValue = (decimal)rdr["FaceValue_55"];
                            Cassettes_5.InNotes = (int)rdr["InNotes_55"];
                            Cassettes_5.DispNotes = (int)rdr["DispNotes_55"];
                            Cassettes_5.RejNotes = (int)rdr["RejNotes_55"];
                            Cassettes_5.RemNotes = (int)rdr["RemNotes_55"];
                            Cassettes_5.CasCount = (int)rdr["CasCount_55"];
                            Cassettes_5.RejCount = (int)rdr["RejCount_55"];
                            Cassettes_5.NewInSuggest = (int)rdr["NewInSuggest_55"];
                            Cassettes_5.NewInUser = (int)rdr["NewInUser_55"];

                            Operator = (string)rdr["Operator"];

                            if (WFunction > 1)
                            {
                                Balances1.ReplToRepl = (decimal)rdr["ReplToReplAmt_1"];
                                Balances2.ReplToRepl = (decimal)rdr["ReplToReplAmt_2"];
                                Balances3.ReplToRepl = (decimal)rdr["ReplToReplAmt_3"];
                                Balances4.ReplToRepl = (decimal)rdr["ReplToReplAmt_4"];
                                Balances5.ReplToRepl = (decimal)rdr["ReplToReplAmt_5"];
                            }
                        }

                        // Close Reader
                        rdr.Close();
                    }
                    //TEST
                    if (Na_RecordFound == true)
                    {
                        Am.ReadAtmsMainSpecificWithConn(WAtmNo, conn);
                    }     
                    // Close conn
                    conn.Close();
                    //
                    // Set Notes Value after closing of connection
                    //
                    if (Na_RecordFound == true)
                    {
                        SetNotesBalancesValues();
                    }
                    if (Na_RecordFound == true)
                    {
                        RecordFound = true;  // this is the final because was destroyed in process
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, WAtmNo);
                }
        }
        //
        // Close Connection and Set Values
        //  
        public void SetNotesBalancesValues()
        {
            // GET REPLENISHMENT TO REPLENISHMENT VALUES 
            // Number of active cassettes

            ActiveCassettesNo = 0;

            if (Cassettes_1.InNotes > 0) ActiveCassettesNo = ActiveCassettesNo + 1;
            if (Cassettes_2.InNotes > 0) ActiveCassettesNo = ActiveCassettesNo + 1;
            if (Cassettes_3.InNotes > 0) ActiveCassettesNo = ActiveCassettesNo + 1;
            if (Cassettes_4.InNotes > 0) ActiveCassettesNo = ActiveCassettesNo + 1;
            if (Cassettes_5.InNotes > 0) ActiveCassettesNo = ActiveCassettesNo + 1;

            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle); // READ TO FIND STATUS. 
                                                             // IF ProcessMode = -1 then we are at the middle of Replenishement Cycle
                                                             // Therefore we do not have data for Cassetes current status we only have the cassette In
                                                             // If ProcessMode = 0 then we have data for cassettes 

            // DIFFERENCES FOR NOTES 
            //
            if (Ta.ProcessMode != -1) // We have data for Cassettes 
            {
                Cassettes_1.DiffCas = Cassettes_1.CasCount - Cassettes_1.RemNotes;
                Cassettes_1.DiffRej = Cassettes_1.RejCount - Cassettes_1.RejNotes;
                Cassettes_2.DiffCas = Cassettes_2.CasCount - Cassettes_2.RemNotes;
                Cassettes_2.DiffRej = Cassettes_2.RejCount - Cassettes_2.RejNotes;
                Cassettes_3.DiffCas = Cassettes_3.CasCount - Cassettes_3.RemNotes;
                Cassettes_3.DiffRej = Cassettes_3.RejCount - Cassettes_3.RejNotes;
                Cassettes_4.DiffCas = Cassettes_4.CasCount - Cassettes_4.RemNotes;
                Cassettes_4.DiffRej = Cassettes_4.RejCount - Cassettes_4.RejNotes;
                Cassettes_5.DiffCas = Cassettes_5.CasCount - Cassettes_5.RemNotes;
                Cassettes_5.DiffRej = Cassettes_5.RejCount - Cassettes_5.RejNotes;
            }
            

            // BALANCES BY CURRENCY 
            // 
            // 
            //   Balances1.CurrCd = Cassettes_1.CurCode;
            Balances1.CurrNm = Cassettes_1.CurName;
            Balances1.OpenBal = Cassettes_1.InNotes * Cassettes_1.FaceValue;
            Balances1.CountedBal = Cassettes_1.CasCount * Cassettes_1.FaceValue + Cassettes_1.RejCount * Cassettes_1.FaceValue;
            Balances1.MachineBal = Cassettes_1.RemNotes * Cassettes_1.FaceValue + Cassettes_1.RejNotes * Cassettes_1.FaceValue;


            if ((Balances1.OpenBal + Balances1.CountedBal + Balances1.MachineBal) > 0)
            {
                One = true;
            }
            else
            {
                One = false;

                Balances1.CurrNm = " ";
            }


            Balances2.CurrNm = Cassettes_2.CurName;
            Balances2.OpenBal = Cassettes_2.InNotes * Cassettes_2.FaceValue;
            Balances2.CountedBal = Cassettes_2.CasCount * Cassettes_2.FaceValue + Cassettes_2.RejCount * Cassettes_2.FaceValue;
            Balances2.MachineBal = Cassettes_2.RemNotes * Cassettes_2.FaceValue + Cassettes_2.RejNotes * Cassettes_2.FaceValue;

            if ((Balances2.OpenBal + Balances2.CountedBal + Balances2.MachineBal) > 0)
            {
                Two = true;
            }
            else
            {
                Two = false;

                Balances2.CurrNm = " ";
            }


            Balances3.CurrNm = Cassettes_3.CurName;
            Balances3.OpenBal = Cassettes_3.InNotes * Cassettes_3.FaceValue;
            Balances3.CountedBal = Cassettes_3.CasCount * Cassettes_3.FaceValue + Cassettes_3.RejCount * Cassettes_3.FaceValue;
            Balances3.MachineBal = Cassettes_3.RemNotes * Cassettes_3.FaceValue + Cassettes_3.RejNotes * Cassettes_3.FaceValue;

            if ((Balances3.OpenBal + Balances3.CountedBal + Balances3.MachineBal) > 0)
            {
                Three = true;
            }
            else
            {
                Three = false;

                Balances3.CurrNm = " ";
            }

            Balances4.CurrNm = Cassettes_4.CurName;
            Balances4.OpenBal = Cassettes_4.InNotes * Cassettes_4.FaceValue;
            Balances4.CountedBal = Cassettes_4.CasCount * Cassettes_4.FaceValue + Cassettes_4.RejCount * Cassettes_4.FaceValue;
            Balances4.MachineBal = Cassettes_4.RemNotes * Cassettes_4.FaceValue + Cassettes_4.RejNotes * Cassettes_4.FaceValue;

            if ((Balances4.OpenBal + Balances4.CountedBal + Balances4.MachineBal) > 0)
            {
                Four = true;
            }
            else
            {
                Four = false;

                Balances4.CurrNm = " ";
            }

            Balances5.CurrNm = Cassettes_5.CurName;
            Balances5.OpenBal = Cassettes_5.InNotes * Cassettes_5.FaceValue;
            Balances5.CountedBal = Cassettes_5.CasCount * Cassettes_5.FaceValue + Cassettes_5.RejCount * Cassettes_5.FaceValue;
            Balances5.MachineBal = Cassettes_5.RemNotes * Cassettes_5.FaceValue + Cassettes_5.RejNotes * Cassettes_5.FaceValue;

            if ((Balances5.OpenBal + Balances5.CountedBal + Balances5.MachineBal) > 0)
            {
                Five = true;
            }
            else
            {
                Five = false;

                Balances5.CurrNm = " ";
            }

            // MERGE SAME CURRENCY eg if 1 and 3 = same currency then add three to one and make 3 = zero 
            // MERGE OF CURRENCIES 

            if (One == true & Two == true)
            {
                if (Cassettes_1.CurName == Cassettes_2.CurName)
                {
                    // Add balances of 2 to 1
                    Balances1.OpenBal = Balances1.OpenBal + Balances2.OpenBal;
                    Balances1.CountedBal = Balances1.CountedBal + Balances2.CountedBal;
                    Balances1.MachineBal = Balances1.MachineBal + Balances2.MachineBal;

                    Balances2.CurrNm = " ";
                    Balances2.OpenBal = 0;
                    Balances2.CountedBal = 0;
                    Balances2.MachineBal = 0;

                    Two = false;
                }
            }
            if (One == true & Three == true)
            {
                if (Cassettes_1.CurName == Cassettes_3.CurName)
                {
                    // Add balances of 3 to 1
                    Balances1.OpenBal = Balances1.OpenBal + Balances3.OpenBal;
                    Balances1.CountedBal = Balances1.CountedBal + Balances3.CountedBal;
                    Balances1.MachineBal = Balances1.MachineBal + Balances3.MachineBal;

                    Balances3.CurrNm = " ";
                    Balances3.OpenBal = 0;
                    Balances3.CountedBal = 0;
                    Balances3.MachineBal = 0;

                    Three = false;
                }
            }

            if (One == true & Four == true)
            {
                if (Cassettes_1.CurName == Cassettes_4.CurName)
                {
                    // Add balances of 4 to 1

                    Balances1.OpenBal = Balances1.OpenBal + Balances4.OpenBal;
                    Balances1.CountedBal = Balances1.CountedBal + Balances4.CountedBal;
                    Balances1.MachineBal = Balances1.MachineBal + Balances4.MachineBal;

                    Balances4.CurrNm = " ";
                    Balances4.OpenBal = 0;
                    Balances4.CountedBal = 0;
                    Balances4.MachineBal = 0;

                    Four = false;
                }
            }
            if (One == true & Five == true)
            {
                if (Cassettes_1.CurName == Cassettes_5.CurName)
                {
                    // Add balances of 5 to 1

                    Balances1.OpenBal = Balances1.OpenBal + Balances5.OpenBal;
                    Balances1.CountedBal = Balances1.CountedBal + Balances5.CountedBal;
                    Balances1.MachineBal = Balances1.MachineBal + Balances5.MachineBal;

                    Balances5.CurrNm = " ";
                    Balances5.OpenBal = 0;
                    Balances5.CountedBal = 0;
                    Balances5.MachineBal = 0;

                    Five = false;
                }
            }
            if (Two == true & Three == true)
            {
                if (Cassettes_2.CurName == Cassettes_3.CurName)
                {
                    // Add balances of 3 to 2
                    Balances2.OpenBal = Balances2.OpenBal + Balances3.OpenBal;
                    Balances2.CountedBal = Balances2.CountedBal + Balances3.CountedBal;
                    Balances2.MachineBal = Balances2.MachineBal + Balances3.MachineBal;
                    Balances2.ReplToRepl = Balances2.ReplToRepl + Balances3.ReplToRepl;

                    Balances3.CurrNm = " ";
                    Balances3.OpenBal = 0;
                    Balances3.CountedBal = 0;
                    Balances3.MachineBal = 0;

                    Three = false;
                }
            }
            if (Two == true & Four == true)
            {
                if (Cassettes_2.CurName == Cassettes_4.CurName)
                {
                    // Add balances of 4 to 2
                    Balances2.OpenBal = Balances2.OpenBal + Balances4.OpenBal;
                    Balances2.CountedBal = Balances2.CountedBal + Balances4.CountedBal;
                    Balances2.MachineBal = Balances2.MachineBal + Balances4.MachineBal;

                    Balances4.CurrNm = " ";
                    Balances4.OpenBal = 0;
                    Balances4.CountedBal = 0;
                    Balances4.MachineBal = 0;

                    Four = false;
                }
            }
            if (Two == true & Five == true)
            {
                if (Cassettes_2.CurName == Cassettes_5.CurName)
                {
                    // Add balances of 5 to 2
                    Balances2.OpenBal = Balances2.OpenBal + Balances5.OpenBal;
                    Balances2.CountedBal = Balances2.CountedBal + Balances5.CountedBal;
                    Balances2.MachineBal = Balances2.MachineBal + Balances5.MachineBal;

                    Balances5.CurrNm = " ";
                    Balances5.OpenBal = 0;
                    Balances5.CountedBal = 0;
                    Balances5.MachineBal = 0;

                    Five = false;
                }
            }
            if (Three == true & Four == true)
            {
                if (Cassettes_3.CurName == Cassettes_4.CurName)
                {
                    // Add balances of 4 to 3
                    Balances3.OpenBal = Balances3.OpenBal + Balances4.OpenBal;
                    Balances3.CountedBal = Balances3.CountedBal + Balances4.CountedBal;
                    Balances3.MachineBal = Balances3.MachineBal + Balances4.MachineBal;

                    Balances4.CurrNm = " ";
                    Balances4.OpenBal = 0;
                    Balances4.CountedBal = 0;
                    Balances4.MachineBal = 0;

                    Four = false;
                }
            }
            if (Three == true & Five == true)
            {
                if (Cassettes_3.CurName == Cassettes_5.CurName)
                {
                    // Add balances of 5 to 3
                    Balances3.OpenBal = Balances3.OpenBal + Balances5.OpenBal;
                    Balances3.CountedBal = Balances3.CountedBal + Balances5.CountedBal;
                    Balances3.MachineBal = Balances3.MachineBal + Balances5.MachineBal;

                    Balances5.CurrNm = " ";
                    Balances5.OpenBal = 0;
                    Balances5.CountedBal = 0;
                    Balances5.MachineBal = 0;

                    Five = false;
                }
            }
            if (Four == true & Five == true)
            {
                if (Cassettes_4.CurName == Cassettes_5.CurName)
                {
                    // Add balances of 5 to 4
                    Balances4.OpenBal = Balances4.OpenBal + Balances5.OpenBal;
                    Balances4.CountedBal = Balances4.CountedBal + Balances5.CountedBal;
                    Balances4.MachineBal = Balances4.MachineBal + Balances5.MachineBal;

                    Balances5.CurrNm = " ";
                    Balances5.OpenBal = 0;
                    Balances5.CountedBal = 0;
                    Balances5.MachineBal = 0;

                    Five = false;
                }
            }

            // MAKE COMPRESSION .....  THE MERGING HAD CREATED EMPTY SETS. eg 1 and 2 is full but 3 might be empty and 4 full
            ///                                  => Bring 4 to 3 => You create three continous sets 

            // FILL ONE 

            if (One == false & Two == true)
            {
                // Move 2 to 1

                Balances1.CurrNm = Cassettes_2.CurName;
                Balances1.OpenBal = Balances2.OpenBal;
                Balances1.CountedBal = Balances2.CountedBal;
                Balances1.MachineBal = Balances2.MachineBal;

                Balances2.CurrNm = " ";
                Balances2.OpenBal = 0;
                Balances2.CountedBal = 0;
                Balances2.MachineBal = 0;

                One = true; Two = false;
            }
            if (One == false & Two == false & Three == true)
            {
                // Move 3 to 1

                Balances1.CurrNm = Cassettes_3.CurName;
                Balances1.OpenBal = Balances3.OpenBal;
                Balances1.CountedBal = Balances3.CountedBal;
                Balances1.MachineBal = Balances3.MachineBal;

                Balances3.CurrNm = " ";
                Balances3.OpenBal = 0;
                Balances3.CountedBal = 0;
                Balances3.MachineBal = 0;

                One = true; Three = false;
            }
            if (One == false & Two == false & Three == false & Four == true)
            {
                // Move 4 to 1

                Balances1.CurrNm = Cassettes_4.CurName;
                Balances1.OpenBal = Balances4.OpenBal;
                Balances1.CountedBal = Balances4.CountedBal;
                Balances1.MachineBal = Balances4.MachineBal;

                Balances4.CurrNm = " ";
                Balances4.OpenBal = 0;
                Balances4.CountedBal = 0;
                Balances4.MachineBal = 0;

                One = true; Four = false;
            }
            if (One == false & Two == false & Three == false & Four == false & Five == true)
            {
                // Move 5 to 1

                Balances1.CurrNm = Cassettes_5.CurName;
                Balances1.OpenBal = Balances5.OpenBal;
                Balances1.CountedBal = Balances5.CountedBal;
                Balances1.MachineBal = Balances5.MachineBal;

                Balances5.CurrNm = " ";
                Balances5.OpenBal = 0;
                Balances5.CountedBal = 0;
                Balances5.MachineBal = 0;

                One = true; Five = false;
            }
            // FILL TWO 
            if (Two == false & Three == true)
            {
                // Move 3 to 2

                Balances2.CurrNm = Cassettes_3.CurName;
                Balances2.OpenBal = Balances3.OpenBal;
                Balances2.CountedBal = Balances3.CountedBal;
                Balances2.MachineBal = Balances3.MachineBal;

                Balances3.CurrNm = " ";
                Balances3.OpenBal = 0;
                Balances3.CountedBal = 0;
                Balances3.MachineBal = 0;

                Two = true; Three = false;
            }
            if (Two == false & Four == true)
            {
                // Move 4 to 2

                Balances2.CurrNm = Cassettes_4.CurName;
                Balances2.OpenBal = Balances4.OpenBal;
                Balances2.CountedBal = Balances4.CountedBal;
                Balances2.MachineBal = Balances4.MachineBal;

                Balances4.CurrNm = " ";
                Balances4.OpenBal = 0;
                Balances4.CountedBal = 0;
                Balances4.MachineBal = 0;

                Two = true; Four = false;
            }
            if (Two == false & Five == true)
            {
                // Move 5 to 2

                Balances2.CurrNm = Cassettes_5.CurName;
                Balances2.OpenBal = Balances5.OpenBal;
                Balances2.CountedBal = Balances5.CountedBal;
                Balances2.MachineBal = Balances5.MachineBal;

                Balances5.CurrNm = " ";
                Balances5.OpenBal = 0;
                Balances5.CountedBal = 0;
                Balances5.MachineBal = 0;

                Two = true; Five = false;
            }
            // FILL THREE
            if (Three == false & Four == true)
            {
                // Move 4 to 3

                Balances3.CurrNm = Cassettes_4.CurName;
                Balances3.OpenBal = Balances4.OpenBal;
                Balances3.CountedBal = Balances4.CountedBal;
                Balances3.MachineBal = Balances4.MachineBal;

                Balances4.CurrNm = " ";
                Balances4.OpenBal = 0;
                Balances4.CountedBal = 0;
                Balances4.MachineBal = 0;

                Three = true; Four = false;
            }
            if (Three == false & Five == true)
            {
                // Move 5 to 3

                Balances3.CurrNm = Cassettes_5.CurName;
                Balances3.OpenBal = Balances5.OpenBal;
                Balances3.CountedBal = Balances5.CountedBal;
                Balances3.MachineBal = Balances5.MachineBal;

                Balances5.CurrNm = " ";
                Balances5.OpenBal = 0;
                Balances5.CountedBal = 0;
                Balances5.MachineBal = 0;

                Three = true; Five = false;
            }
            // FILL FOUR
            if (Four == false & Five == true)
            {
                // Move 5 to 4

                Balances4.CurrNm = Cassettes_5.CurName;
                Balances4.OpenBal = Balances5.OpenBal;
                Balances4.CountedBal = Balances5.CountedBal;
                Balances4.MachineBal = Balances5.MachineBal;

                Balances5.CurrNm = " ";
                Balances5.OpenBal = 0;
                Balances5.CountedBal = 0;
                Balances5.MachineBal = 0;

                Four = true; Five = false;
            }

            // How many SETS of Balances ??? 

            BalSets = 0; // How Many Balance Sets 

            if (One == true) BalSets = BalSets + 1;
            if (Two == true) BalSets = BalSets + 1;
            if (Three == true) BalSets = BalSets + 1;
            if (Four == true) BalSets = BalSets + 1;
            if (Five == true) BalSets = BalSets + 1;


            if (WFunction == 1 & Ta.ProcessMode != -1) // This called only during UCForm51a 
            {
                // Till here we have a) ATM Balances for Notes and Cash per currency -
                // Needed for General but to calculate replanishement to replenishement reconciliation too
                // It can be called at any time 

                if (Operator == "CRBAGRAA")
                {
                    // OLD TABLE 
                    ReadInPoolTransReplToRepl(WAtmNo, WReplCycle); // READ TRANSACTIONS AND CALCULATE BALANCES FOR REPL TO REPL 
                }
                else
                {
                    ReadInPoolTransReplToRepl_NEW(WAtmNo, WReplCycle); // READ TRANSACTIONS AND CALCULATE BALANCES FOR REPL TO REPL 
                }

                UpdateSessionsNotesAndValues2(WAtmNo, WReplCycle); // UPDATE WITH REPL VALUES  

                ReadAllErrorsTable(WAtmNo, WReplCycle); // Find NumberOfErrors

                return;

            }

            // WE ARE NOT USING NOW the below functionality 
            //return; 

            if (Ta.ProcessMode == -1) // We Do not have cassette data but we need to know what money we have in ATMs
            {
                // Find all DRs 
                // Machine Balance = Open Balance - all DR

                if (Operator == "CRBAGRAA")
                {
                    // OLD TABLE 
                    ReadInPoolTransReplToRepl(WAtmNo, WReplCycle); // READ TRANSACTIONS AND CALCULATE BALANCES FOR REPL TO REPL 
                }
                else
                {
                    ReadInPoolTransReplToRepl_NEW(WAtmNo, WReplCycle); // READ TRANSACTIONS AND CALCULATE BALANCES FOR REPL TO REPL 
                }

                UpdateSessionsNotesAndValues2(WAtmNo, WReplCycle); // UPDATE WITH REPL VALUES  

                Balances1.MachineBal = Balances1.ReplToRepl;
                Balances2.MachineBal = Balances2.ReplToRepl;
                Balances3.MachineBal = Balances3.ReplToRepl;
                Balances4.MachineBal = Balances4.ReplToRepl;
                Balances5.MachineBal = Balances5.ReplToRepl;
            }

            Ac.ReadAtm(AtmNo);

            if (Ac.CitId != "1000" & Cit_UnloadedCounted>0)
            {
                // CIT CASE
                // The UNLOADED COUNTED COME FROM CIT DATA
                Balances1.CountedBal = Cit_UnloadedCounted;
            }

            //   MessageBox.Show("Number of Balances=" + BalSets.ToString());


            if (WFunction >= 3) // Include HOST BALANCES IN BALANCES STRUCTURE  
            {
                // READ HOST ID BATCH 

                // Get Host Balances and update structures 

                // Adjust GL Balances 
                if (Operator == "CRBAGRAA")
                {
                    ReadHostBatchesTable(WAtmNo); // GET HOST BALANCES FROM BATCHES 

                    // Adjust Host Balances with transactions made since updated by Host transactions 

                    // Initialise Targets

                    Gp.ReadParametersSpecificId(Operator, "705", "1", "", "");
                    SystemTargets1.Name = Gp.OccuranceNm;

                    Gp.ReadParametersSpecificId(Operator, "705", "2", "", "");
                    SystemTargets2.Name = Gp.OccuranceNm;

                    Gp.ReadParametersSpecificId(Operator, "705", "3", "", "");
                    SystemTargets3.Name = Gp.OccuranceNm;

                    Gp.ReadParametersSpecificId(Operator, "705", "4", "", "");
                    SystemTargets4.Name = Gp.OccuranceNm;

                    Gp.ReadParametersSpecificId(Operator, "705", "5", "", "");
                    SystemTargets5.Name = Gp.OccuranceNm;

                    ReadHLastTraceNumberAtHost(WAtmNo);// Find the latest traces of targets 

                    if (RecordFound == true)
                    {

                        int[] array1 = { SystemTargets1.LastTrace, SystemTargets2.LastTrace, SystemTargets3.LastTrace,
                                                   SystemTargets4.LastTrace, SystemTargets5.LastTrace };

                        //
                        // Find minimum number. FROM WHERE TO START SEARCHING 
                        // 

                        MinTraceTarget = array1.Where(a => a > 0).Min();

                        MaxTraceTarget = array1.Where(a => a > 0).Max();

                        if (SystemTargets1.LastTrace == 0)
                        {

                            SystemTargets1.LastTrace = MinTraceTarget;
                            SystemTargets1.DateTm = NullPastDate;
                        }
                        if (SystemTargets2.LastTrace == 0)
                        {
                            SystemTargets2.LastTrace = MinTraceTarget;
                            SystemTargets2.DateTm = NullPastDate;
                        }
                        if (SystemTargets3.LastTrace == 0)
                        {
                            SystemTargets3.LastTrace = MinTraceTarget;
                            SystemTargets3.DateTm = NullPastDate;
                        }
                        if (SystemTargets4.LastTrace == 0)
                        {
                            SystemTargets4.LastTrace = MinTraceTarget;
                            SystemTargets4.DateTm = NullPastDate;
                        }
                        if (SystemTargets5.LastTrace == 0)
                        {
                            SystemTargets5.LastTrace = MinTraceTarget;
                            SystemTargets5.DateTm = NullPastDate;
                        }

                        ReadInPoolTransTranNo(WAtmNo, MinTraceTarget); // Find the TranNo to start with  

                        ReadInPoolTransGreaterThanTranNo(WAtmNo, WTranNo); // 
                    }
                    else
                    {
                        ReadInPoolTransGreaterThanTranNo(WAtmNo, 0); // Adjust Host balances to ATM current balances 
                    }

                    // ASSIGN HOST BALANCES TO THE RIGHT PLACE OF STRUCTURE OF BALANCES 

                    if (BalSets > 0) UpdateHostBal(HCurrNm1, HBal1, HAdjBal1, BalSets);
                    if (BalSets > 1) UpdateHostBal(HCurrNm2, HBal2, HAdjBal2, BalSets);
                    if (BalSets > 2) UpdateHostBal(HCurrNm3, HBal3, HAdjBal3, BalSets);
                    if (BalSets > 3) UpdateHostBal(HCurrNm4, HBal4, HAdjBal4, BalSets);

                }
                else
                {
                    // NATIONAL BANK
                    //
                    // NEW METHOD TO CALCULATE ADJUSTED GL 
                    if (Operator == "ETHNCY2N")
                    {
                        if (Is_GL_Adjusted == false)
                        {
                            RRDM_GL_Balances_For_Categories_And_Atms Gadj = new RRDM_GL_Balances_For_Categories_And_Atms();
                            Ta.ReadSessionsStatusTraces(AtmNo, SesNo);
                            // If NEW METHOD 
                            GL_Bal_Repl_Adjusted = Gadj.FindAdjusted_GL_Balance_AND_Update_Session_First_Method(Operator, AtmNo, SesNo, Ta.SesDtTimeEnd.Date);

                            if (Gadj.IsDataFound == true)
                            {
                                ReadSessionsNotesAndValues(AtmNo, SesNo, 2);

                                UpdateHostBal(Ac.DepCurNm, GL_Balance, GL_Bal_Repl_Adjusted, 1);
                            }
                            else
                            {
                                Is_GL_Adjusted = false;

                                if (Ac.CitId != "1000")
                                {
                                    GL_Bal_Repl_Adjusted = Cit_UnloadedCounted;
                                    GL_Balance = Cit_UnloadedCounted;
                                }
                                else
                                {
                                    // TO BE COMPLETED
                                }

                                UpdateSessionsNotesAndValues(AtmNo, SesNo);

                                //UpdateHostBal(Gadj.Ccy, GL_Balance, GL_Bal_Repl_Adjusted, 1);
                            }

                        }
                        else
                        {

                            UpdateHostBal(Ac.DepCurNm, GL_Balance, GL_Bal_Repl_Adjusted, 1);
                        }

                    }

                }


                // Refressed with actions made by USER on Errors

                if (WFunction == 4) ReadAllErrorsTable(WAtmNo, WReplCycle); // READ ERRORS TO ADJUST BALANCES WITH THE ONES WITH ACTION TAKEN 

                // OUTSTANDING // Refressed with actions all Actions suggested by SYSTEM on Errors - this will be an option 

                if (WFunction == 5) ReadAllErrorsTable(WAtmNo, WReplCycle); // READ ERRORS TO ADJUST BALANCES WITH ANY ERROR 

                //     if (WFunction == 6) ReadAllErrorsTableClosed(WAtmNo,WSesNo); // READ ERRORS TO ADJUST BALANCES WITH Errors corrected and close during this Session  

                // Calculate Differences per set of Balances USE Structures as Per Balances 

                // DIFF FOR CURRENCY 1 // HOST LEVEL 


                BalDiff1.CurrNm = Balances1.CurrNm;

                if (Operator == "CRBAGRAA")
                {
                    BalDiff1.HostAdj = Balances1.CountedBal - (Balances1.HostBalAdj);
                }
                else
                {
                    BalDiff1.HostAdj = Balances1.CountedBal - (Balances1.HostBalAdj);
                }
             
                if (BalDiff1.HostAdj != 0)
                {
                    BalDiff1.HostLevel = true;
                }
                else BalDiff1.HostLevel = false;

                // DIFF FOR CURRENCY 2 // HOST LEVEL

                BalDiff2.CurrNm = Balances2.CurrNm;
                BalDiff2.Host = Balances2.CountedBal - Balances2.HostBal;
                BalDiff2.HostAdj = Balances2.CountedBal - Balances2.HostBalAdj;
                if (BalDiff2.HostAdj != 0)
                {
                    BalDiff2.HostLevel = true;
                }
                else BalDiff2.HostLevel = false;

                // DIFF FOR CURRENCY 3 // HOST LEVEL

                BalDiff3.CurrNm = Balances3.CurrNm;
                BalDiff3.Host = Balances3.CountedBal - Balances3.HostBal;
                BalDiff3.HostAdj = Balances3.CountedBal - Balances3.HostBalAdj;
                if (BalDiff3.HostAdj != 0)
                {
                    BalDiff3.HostLevel = true;
                }
                else BalDiff3.HostLevel = false;

                // DIFF FOR CURRENCY 4 // HOST LEVEL   

                BalDiff4.CurrNm = Balances4.CurrNm;
                BalDiff4.Host = Balances4.CountedBal - Balances4.HostBal;
                BalDiff4.HostAdj = Balances4.CountedBal - Balances4.HostBalAdj;
                if (BalDiff4.HostAdj != 0)
                {
                    BalDiff4.HostLevel = true;
                }
                else BalDiff4.HostLevel = false;

                // CHECK IF DIFFERENCE AT HOST 

                if (BalDiff1.HostLevel == true || BalDiff2.HostLevel == true || BalDiff3.HostLevel == true || BalDiff4.HostLevel == true)
                {
                    DiffAtHostLevel = true;
                }
                else DiffAtHostLevel = false;
            }

            // DIFF FOR CURRENCY 1 - ATM LEVEL ... ALWAYS DIFFERENCES FROM COUNTED CASH 
            if (WFunction >= 2 & CommingFromSM_Master==false) // IN CASE WE ADJUST FOR THE ERRORS THESE MIGHT CHANGE 
            {

                BalDiff1.CurrNm = Balances1.CurrNm;
                BalDiff1.Machine = Balances1.CountedBal - Balances1.MachineBal;
                BalDiff1.ReplToRepl = Balances1.CountedBal - Balances1.ReplToRepl;

                //if (BalDiff1.Machine != 0 || BalDiff1.ReplToRepl != 0)

                if (BalDiff1.Machine != 0 )
                {
                    BalDiff1.AtmLevel = true;
                }
                else BalDiff1.AtmLevel = false;

                // DIFF FOR CURRENCY 2 - ATM LEVEL


                BalDiff2.CurrNm = Balances2.CurrNm;
                BalDiff2.Machine = Balances2.CountedBal - Balances2.MachineBal;
                BalDiff2.ReplToRepl = Balances2.CountedBal - Balances2.ReplToRepl;

                if (BalDiff2.Machine != 0 || BalDiff2.ReplToRepl != 0)
                {
                    BalDiff2.AtmLevel = true;
                }
                else BalDiff2.AtmLevel = false;

                // DIFF FOR CURRENCY 3 - ATM LEVEL


                BalDiff3.CurrNm = Balances3.CurrNm;
                BalDiff3.Machine = Balances3.CountedBal - Balances3.MachineBal;
                BalDiff3.ReplToRepl = Balances3.CountedBal - Balances3.ReplToRepl;

                if (BalDiff3.Machine != 0 || BalDiff3.ReplToRepl != 0)
                {
                    BalDiff3.AtmLevel = true;
                }
                else BalDiff3.AtmLevel = false;

                // DIFF FOR CURRENCY 4 - ATM LEVEL 


                BalDiff4.CurrNm = Balances4.CurrNm;
                BalDiff4.Machine = Balances4.CountedBal - Balances4.MachineBal;
                BalDiff4.ReplToRepl = Balances4.CountedBal - Balances4.ReplToRepl;

                if (BalDiff4.Machine != 0 || BalDiff4.ReplToRepl != 0)
                {
                    BalDiff4.AtmLevel = true;
                }
                else BalDiff4.AtmLevel = false;

                //           if (WFunction == 2) return; // PROVIDE ALL BALANCES AND INFORMATION OF ATM INCLUDING REPL TO REPL (NO HOST)

                // DIFFERENCE AT ATM 

                if (BalDiff1.AtmLevel == true || BalDiff2.AtmLevel == true || BalDiff3.AtmLevel == true || BalDiff4.AtmLevel == true)
                {
                    DiffAtAtmLevel = true;
                }
                else DiffAtAtmLevel = false;

                // FIND OUT IF ERRORS OUTSTANDING 

                if (WFunction < 4) // If 4 or 5 then already read. 
                {
                    ErrOutstanding = 0;
                    ReadAllErrorsTable(WAtmNo, WReplCycle);
                    Balances1.PresenterValue = 0; // get the dates value from below otherwise keep it as zero. 
                    // 
                    ReadAllErrorsTable_Presenter_By_Date(WAtmNo, WReplCycle);
                }

                if (ErrOutstanding > 0)
                {
                    DiffWithErrors = true;
                }
                else DiffWithErrors = false;

            }
        }
        //
        // Read Transactions to CALCULATE REPL TO REPL FROM OLD DATABASE 
        //
        public void ReadInPoolTransReplToRepl(string InAtmNo, int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool first = true;

            int AtmTraceNo;

            // Initialise to openning balances 
            Balances1.ReplToRepl = Balances1.OpenBal;
            Balances2.ReplToRepl = Balances2.OpenBal;
            Balances3.ReplToRepl = Balances3.OpenBal;
            Balances4.ReplToRepl = Balances4.OpenBal;
            Balances5.ReplToRepl = Balances5.OpenBal;

            string SqlString = "SELECT *"
          + " FROM [dbo].[InPoolTrans] "
          + " WHERE SesNo = @SesNo AND AtmNo=@AtmNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                        int TransType;

                        decimal TranAmount;
                        string TransDesc;
                        string CurrDesc;
                        bool SuccTran;
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            if (first == true)
                            {
                                FirstTraceNo = (int)rdr["AtmTraceNo"];
                                if (FirstTraceNo == 0) first = true; // in other words ignore the first record which 0 
                                else first = false;
                            }
                            LastTraceNo = (int)rdr["AtmTraceNo"];

                            // Assign Values 
                            TransType = (int)rdr["TransType"];

                            CurrDesc = rdr["CurrDesc"].ToString();
                            TranAmount = (decimal)rdr["TranAmount"];
                            TransDesc = (string)rdr["TransDesc"];
                            SuccTran = (bool)rdr["SuccTran"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            if (AtmTraceNo == 0)
                            {
                                ValidTran = false;
                            }
                            else ValidTran = true;

                            if (ValidTran == true)
                            {
                                if (CurrDesc == Balances1.CurrNm)
                                {
                                    if (TransType == 11 & SuccTran)
                                    {
                                        // Debits 
                                        Balances1.ReplToRepl = Balances1.ReplToRepl - TranAmount;

                                    }
                                    if (TransType == 21 & SuccTran)
                                    {
                                        // Credits  
                                        Balances1.ReplToRepl = Balances1.ReplToRepl + TranAmount;
                                    }
                                }
                                if (CurrDesc == Balances2.CurrNm)
                                {
                                    if (TransType == 11 & SuccTran)
                                    {
                                        // Debits 
                                        Balances2.ReplToRepl = Balances2.ReplToRepl - TranAmount;
                                    }
                                    if (TransType == 21 & SuccTran)
                                    {
                                        // Credits  
                                        Balances2.ReplToRepl = Balances2.ReplToRepl + TranAmount;
                                    }
                                }
                                if (CurrDesc == Balances3.CurrNm)
                                {
                                    if (TransType == 11 & SuccTran)
                                    {
                                        // Debits 
                                        Balances3.ReplToRepl = Balances3.ReplToRepl - TranAmount;
                                    }
                                    if (TransType == 21 & SuccTran)
                                    {
                                        // Credits  
                                        Balances3.ReplToRepl = Balances3.ReplToRepl + TranAmount;
                                    }
                                }
                                if (CurrDesc == Balances4.CurrNm)
                                {
                                    if (TransType == 11 & SuccTran)
                                    {
                                        // Debits 
                                        Balances4.ReplToRepl = Balances4.ReplToRepl - TranAmount;
                                    }
                                    if (TransType == 21 & SuccTran)
                                    {
                                        // Credits  
                                        Balances4.ReplToRepl = Balances4.ReplToRepl + TranAmount;
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

                    CatchDetails(ex, WAtmNo);
                }
        }

        //
        // Read Transactions to CALCULATE REPL TO REPL FROM NEW DATABASE 
        //
        public void ReadInPoolTransReplToRepl_NEW(string InTerminalId, int InReplCycleNo)
        {
            //RecordFound = false;
            //ErrorFound = false;
            //ErrorOutput = "";

            int TransType;
            string TransCurr;
            decimal TransAmount;
            string WUser; 
            RRDMUsersAccessToAtms Uat = new RRDMUsersAccessToAtms();
            Uat.FindUserForReplForATM(InTerminalId);
            if (Uat.RecordFound == true)
            {
                WUser = Uat.UserId; 
            }
            else
            {
                WUser = "Pilot_XXX";
            }

            Ta.ReadSessionsStatusTraces(WAtmNo, InReplCycleNo);

            DateTime SesStart = Ta.SesDtTimeStart;
            DateTime SesEnd = Ta.SesDtTimeEnd;

            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            string WTableId = "Atms_Journals_Txns"; 
            Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates(WUser, WTableId, InTerminalId, SesStart, SesEnd, 1 , 2);
            //textBoxTotalDebit.Text = Mgt.TotalDebit.ToString("#,##0.00");

            if (IsNewAtm == true)
            {
                Balances1.ReplToRepl = Balances1.MachineBal;
                return;
            }
            decimal TolalDebits = 0;

            // Initialise to openning balances 
            Balances1.ReplToRepl = Balances1.OpenBal;
            Balances2.ReplToRepl = Balances2.OpenBal;
            Balances3.ReplToRepl = Balances3.OpenBal;
            Balances4.ReplToRepl = Balances4.OpenBal;

            Balances1.ReplToRepl = Balances1.ReplToRepl - Mgt.TotalDebit;

          //  string SqlString = "SELECT *"
          //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
          //+ " Where TerminalId = @TerminalId "
          //+ " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
          //+ " AND NotInJournal = 0 "
          //+ " AND(ResponseCode = '0') AND TXNSRC = '1' AND TransType = 11 AND Origin = 'Our Atms'";

          //  using (SqlConnection conn =
          //                new SqlConnection(connectionString))
          //      try
          //      {
          //          conn.Open();
          //          using (SqlCommand cmd =
          //              new SqlCommand(SqlString, conn))
          //          {
          //              cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
          //              cmd.Parameters.AddWithValue("@DateFrom", SesStart);
          //              cmd.Parameters.AddWithValue("@DateTo", SesEnd);

          //              // Read table 

          //              SqlDataReader rdr = cmd.ExecuteReader();

          //              while (rdr.Read())
          //              {
          //                  RecordFound = true;


          //                  // Assign Values 
          //                  TransType = (int)rdr["TransType"];
          //                  TransCurr = rdr["TransCurr"].ToString();
          //                  TransAmount = (decimal)rdr["TransAmount"];


          //                  if (TransCurr == Balances1.CurrNm)
          //                  {
          //                      if (TransType == 11)
          //                      {
          //                          // Debits .= . Dispensed 
          //                          Balances1.ReplToRepl = Balances1.ReplToRepl - TransAmount;

          //                          TolalDebits = TolalDebits + TransAmount; 

          //                      }
          //                      //if (TransType == 21 & SuccTran)
          //                      //{
          //                      //    // Credits  
          //                      //    Balances1.ReplToRepl = Balances1.ReplToRepl + TranAmount;
          //                      //}
          //                  }
          //                  if (TransCurr == Balances2.CurrNm)
          //                  {
          //                      if (TransType == 11)
          //                      {
          //                          // Debits .= . Dispensed 
          //                          Balances2.ReplToRepl = Balances2.ReplToRepl - TransAmount;
          //                      }
          //                      //if (TransType == 21 & SuccTran)
          //                      //{
          //                      //    // Credits  
          //                      //    Balances2.ReplToRepl = Balances2.ReplToRepl + TranAmount;
          //                      //}
          //                  }
          //                  if (TransCurr == Balances3.CurrNm)
          //                  {
          //                      if (TransType == 11)
          //                      {
          //                          // Debits .= . Dispensed 
          //                          Balances3.ReplToRepl = Balances3.ReplToRepl - TransAmount;
          //                      }
          //                      //if (TransType == 21 & SuccTran)
          //                      //{
          //                      //    // Credits  
          //                      //    Balances3.ReplToRepl = Balances3.ReplToRepl + TranAmount;
          //                      //}
          //                  }
          //                  if (TransCurr == Balances4.CurrNm)
          //                  {
          //                      if (TransType == 11)
          //                      {
          //                          // Debits .= . Dispensed 
          //                          Balances4.ReplToRepl = Balances4.ReplToRepl - TransAmount;
          //                      }
          //                      //if (TransType == 21 & SuccTran)
          //                      //{
          //                      //    // Credits  
          //                      //    Balances4.ReplToRepl = Balances4.ReplToRepl + TranAmount;
          //                      //}
          //                  }


          //              }

          //              // Close Reader
          //              rdr.Close();

          //          }

          //          // Close conn
          //          conn.Close();
          //      }
          //      catch (Exception ex)
          //      {
          //          conn.Close();

          //          CatchDetails(ex, WAtmNo);
          //      }
        }

        // UPDATE SESSION NOTES WITH REPLENISHEMENT VALUES 
        private void UpdateSessionsNotesAndValues2(string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //   UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsNotesAndValues  SET "
                           + " [CurrNm_1] = @CurrNm_1, [ReplToReplAmt_1] = @ReplToReplAmt_1,"
                          + "  [CurrNm_2] = @CurrNm_2, [ReplToReplAmt_2] = @ReplToReplAmt_2,"
                          + "  [CurrNm_3] = @CurrNm_3, [ReplToReplAmt_3] = @ReplToReplAmt_3,"
                          + "  [CurrNm_4] = @CurrNm_4, [ReplToReplAmt_4] = @ReplToReplAmt_4,"
                          + "  [CurrNm_5] = @CurrNm_5, [ReplToReplAmt_5] = @ReplToReplAmt_5"
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@CurrNm_1", Balances1.CurrNm);
                        cmd.Parameters.AddWithValue("@ReplToReplAmt_1", Balances1.ReplToRepl);

                        cmd.Parameters.AddWithValue("@CurrNm_2", Balances2.CurrNm);
                        cmd.Parameters.AddWithValue("@ReplToReplAmt_2", Balances2.ReplToRepl);

                        cmd.Parameters.AddWithValue("@CurrNm_3", Balances3.CurrNm);
                        cmd.Parameters.AddWithValue("@ReplToReplAmt_3", Balances3.ReplToRepl);

                        cmd.Parameters.AddWithValue("@CurrNm_4", Balances4.CurrNm);
                        cmd.Parameters.AddWithValue("@ReplToReplAmt_4", Balances4.ReplToRepl);

                        cmd.Parameters.AddWithValue("@CurrNm_5", Balances4.CurrNm);
                        cmd.Parameters.AddWithValue("@ReplToReplAmt_5", Balances4.ReplToRepl);
                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, WAtmNo);
                }
        }
        //
        // READ BATCH TAble to take the last Batch 
        //
        public void ReadHostBatchesTable(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[HostBatchesTableATMs] "
            + " WHERE AtmNo=@AtmNo AND ReplCycle = @ReplCycle ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", WReplCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Trace ranges 
                            HBatchNo = (int)rdr["BatchNo"];
                            HDtTmStart = (DateTime)rdr["DtTmStart"];
                            HDtTmEnd = (DateTime)rdr["DtTmEnd"];

                            HCurrNm1 = (string)rdr["HCurrNm1"];
                            HBal1 = (decimal)rdr["HBal1"];

                            HCurrNm2 = (string)rdr["HCurrNm2"];
                            HBal2 = (decimal)rdr["HBal2"];

                            HCurrNm3 = (string)rdr["HCurrNm3"];
                            HBal3 = (decimal)rdr["HBal3"];

                            HCurrNm4 = (string)rdr["HCurrNm4"];
                            HBal4 = (decimal)rdr["HBal4"];

                            Operator = (string)rdr["Operator"];
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //
                    // Check if Differences 
                    //
                    RRDMAccountsClass Acc = new RRDMAccountsClass();
                    RRDMPostedTrans Pt = new RRDMPostedTrans();

                    decimal WTempAdj = HBal1;

                    Acc.ReadAndFindAccount("1000", "", "", Operator, InAtmNo, HCurrNm1, "ATM Cash");
                    if (Acc.RecordFound == true)
                    {
                        Pt.ReadTransForAccountTotals(InAtmNo, Acc.AccNo, HDtTmEnd);

                        WTempAdj = WTempAdj + Pt.TotalDebit12;
                        WTempAdj = WTempAdj - Pt.TotalCredit22;
                    }

                    HAdjBal1 = WTempAdj;
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, WAtmNo);
                }
        }
        // 
        // FIND THE LATEST TRACES IN HOST FILE FROM KONTO FILE
        //
        public void ReadHLastTraceNumberAtHost(string InAtmNo)// Find the latest traces of targets 
        {
            //
            // Read KONTO FILE 
            //
            SystemTargets1.LastTrace = 0;
            SystemTargets1.DateTm = NullPastDate;

            SystemTargets2.LastTrace = 0;
            SystemTargets2.DateTm = NullPastDate;

            SystemTargets3.LastTrace = 0;
            SystemTargets3.DateTm = NullPastDate;

            SystemTargets4.LastTrace = 0;
            SystemTargets4.DateTm = NullPastDate;

            SystemTargets5.LastTrace = 0;
            SystemTargets5.DateTm = NullPastDate;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int TraceNumber;
            int TargetSystem;

            DateTime TraceDateTime;

            string SqlString = "SELECT *"
          + " FROM [ATMS_Journals].[dbo].[tblHLastTraceNumberProcessedBySystem]  "
          + " WHERE AtmNo= @AtmNo ORDER BY TraceNumber ASC ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TraceNumber = (int)rdr["TraceNumber"];

                            TargetSystem = (int)rdr["TargetSystem"];

                            TraceDateTime = (DateTime)rdr["TraceDateTime"];


                            if (TargetSystem == 1) // JCC
                            {
                                SystemTargets1.LastTrace = TraceNumber;
                                SystemTargets1.DateTm = TraceDateTime;
                            }
                            if (TargetSystem == 2) // AMX 
                            {
                                SystemTargets2.LastTrace = TraceNumber;
                                SystemTargets2.DateTm = TraceDateTime;
                            }
                            if (TargetSystem == 3) // Other 1
                            {
                                SystemTargets3.LastTrace = TraceNumber;
                                SystemTargets3.DateTm = TraceDateTime;
                            }
                            if (TargetSystem == 4) // Other 2 
                            {
                                SystemTargets4.LastTrace = TraceNumber;
                                SystemTargets4.DateTm = TraceDateTime;
                            }
                            if (TargetSystem == 5) // Other 3
                            {
                                SystemTargets5.LastTrace = TraceNumber;
                                SystemTargets5.DateTm = TraceDateTime;
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

                    CatchDetails(ex, WAtmNo);
                }
            //TEST
            //
            if (InAtmNo == "AB102" || InAtmNo == "ServeUk102" || InAtmNo == "ABC501")
            {
                RecordFound = true;
                DateTime WDTma;
                //TEST
                WDTma = new DateTime(2014, 02, 28);
                SystemTargets1.LastTrace = 9;
                SystemTargets1.DateTm = WDTma;
                SystemTargets2.LastTrace = 4;
                SystemTargets2.DateTm = WDTma;
                SystemTargets3.LastTrace = 10;
                SystemTargets3.DateTm = WDTma;

            }
            if (InAtmNo == "12507")
            {
                RecordFound = true;
                DateTime WDTma;
                //TEST
                WDTma = new DateTime(2014, 02, 28);
                SystemTargets1.LastTrace = 7;
                SystemTargets1.DateTm = WDTma;
                SystemTargets2.LastTrace = 9;
                SystemTargets2.DateTm = WDTma;
                SystemTargets3.LastTrace = 8;
                SystemTargets3.DateTm = WDTma;

            }
            if (InAtmNo == "AB104")
            {
                RecordFound = true;
                DateTime WDTma;
                //TEST
                WDTma = new DateTime(2014, 02, 12);
                SystemTargets1.LastTrace = 10045450;
                SystemTargets1.DateTm = WDTma;
                SystemTargets2.LastTrace = 10045420;
                SystemTargets2.DateTm = WDTma;
                SystemTargets4.LastTrace = 10045410;
                SystemTargets4.DateTm = WDTma;

            }

        }

        // Read Transactions find the Transaction Number for the minimum target Host trace no 
        // This the from case 
        // 
        public void ReadInPoolTransTranNo(string InAtmNo, int InMinTraceTarget)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [dbo].[InPoolTrans] "
          + " WHERE AtmNo=@AtmNo AND AtmTraceNo= @MinTraceTarget";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@MinTraceTarget", InMinTraceTarget);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            // Assign Values 
                            WTranNo = (int)rdr["TranNo"]; // This is the Tran No from which we should start adding

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

                    CatchDetails(ex, WAtmNo);
                }
        }

        // 
        // Get Atm Number through the Ses No 
        // 
        public string ReadNotesSesionsBySesNoToGetATMNo(int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT AtmNo "
                       + " FROM[ATMS].[dbo].[SessionsNotesAndValues] "
                       + " WHERE SesNo =@SesNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                      
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            // Assign Values 
                            AtmNo = (string)rdr["AtmNo"]; //

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

                    CatchDetails(ex, WAtmNo);
                }

            return AtmNo; 
        }

      


        // Read Transactions > than TranNo   
        //
        public void ReadInPoolTransGreaterThanTranNo(string InAtmNo, int InTranNo)
        {
            HAdjBal1 = HBal1;
            HAdjBal2 = HBal2;
            HAdjBal3 = HBal3;
            HAdjBal4 = HBal4;
            int AFirstTraceNo;
            int ALastTraceNo;

            int TaLast;

            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);

            //TEST
            if (InAtmNo == "AB102" || InAtmNo == "12507" || InAtmNo == "ServeUk102" || InAtmNo == "ABC501")
            {
                TaLast = Ta.LastTraceNo;
            }
            else
            {
                TaLast = Ta.LastTraceNo - 6;
            }

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool first = true;
            string SqlString = "SELECT *"
          + " FROM [dbo].[InPoolTrans] "
          + " WHERE AtmNo=@AtmNo AND TranNo>@WTranNo AND AtmTraceNo <= @AtmTraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@WTranNo", InTranNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", TaLast);

                        int TransType;
                        //       int CurrCode;
                        decimal TranAmount;
                        string CurrDesc;
                        bool SuccTran;
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            ValidTran = false; // Initialise Valid 

                            RecordFound = true;
                            if (first == true)
                            {
                                AFirstTraceNo = (int)rdr["AtmTraceNo"];
                                first = false;
                            }

                            // Take action on transaction you should 


                            ALastTraceNo = (int)rdr["AtmTraceNo"];

                            // Assign Values 
                            SystemTarget = (int)rdr["SystemTarget"];
                            TransType = (int)rdr["TransType"];

                            CurrDesc = rdr["CurrDesc"].ToString();
                            TranAmount = (decimal)rdr["TranAmount"];
                            SuccTran = (bool)rdr["SuccTran"];

                            if (SystemTarget == 1) // 
                            {
                                if (ALastTraceNo > SystemTargets1.LastTrace) ValidTran = true;
                            }
                            if (SystemTarget == 2) // 
                            {
                                if (ALastTraceNo > SystemTargets2.LastTrace) ValidTran = true;
                            }
                            if (SystemTarget == 3) // Other 1
                            {
                                if (ALastTraceNo > SystemTargets3.LastTrace) ValidTran = true;
                            }
                            if (SystemTarget == 4) // Other 2 
                            {
                                if (ALastTraceNo > SystemTargets4.LastTrace) ValidTran = true;
                            }
                            if (SystemTarget == 5) // Other 3 ... CASH CREATED BY SYSTEM 
                            {
                                if (ALastTraceNo > SystemTargets5.LastTrace) ValidTran = true;
                            }

                            if (SystemTarget == 9 & (TransType == 12 || TransType == 22)) // Cash IN and cash out transactions  
                            {
                                ValidTran = true;
                            }

                            if (ValidTran == true)
                            {
                                if (CurrDesc == HCurrNm1)
                                {
                                    if ((TransType == 11 || TransType == 12) & SuccTran)
                                    {
                                        // Debits 
                                        HAdjBal1 = HAdjBal1 - TranAmount;

                                    }
                                    if (TransType == 22 & SuccTran)
                                    {
                                        // Credits  
                                        HAdjBal1 = HAdjBal1 + TranAmount;
                                    }
                                }
                                if (CurrDesc == HCurrNm2)
                                {
                                    if ((TransType == 11 || TransType == 12) & SuccTran)
                                    {
                                        // Debits 
                                        HAdjBal2 = HAdjBal2 - TranAmount;
                                    }
                                    if (TransType == 22 & SuccTran)
                                    {
                                        // Credits  
                                        HAdjBal2 = HAdjBal2 + TranAmount;
                                    }
                                }
                                if (CurrDesc == HCurrNm3)
                                {
                                    if ((TransType == 11 || TransType == 12) & SuccTran)
                                    {
                                        // Debits 
                                        HAdjBal3 = HAdjBal3 - TranAmount;
                                    }
                                    if (TransType == 22 & SuccTran)
                                    {
                                        // Credits  
                                        HAdjBal3 = HAdjBal3 + TranAmount;
                                    }
                                }
                                if (CurrDesc == HCurrNm4)
                                {
                                    if ((TransType == 11 || TransType == 12) & SuccTran)
                                    {
                                        // Debits 
                                        HAdjBal4 = HAdjBal4 - TranAmount;
                                    }
                                    if (TransType == 22 & SuccTran)
                                    {
                                        // Credits  
                                        HAdjBal4 = HAdjBal4 + TranAmount;
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

                    CatchDetails(ex, WAtmNo);
                }
        }

        // Read Transactions > than TranNo   
        //
        public void ReadInPoolTransGreaterThanTranNo_New(string InAtmNo, int InTranNo)
        {
            HAdjBal1 = HBal1;
            HAdjBal2 = HBal2;
            HAdjBal3 = HBal3;
            HAdjBal4 = HBal4;
            int AFirstTraceNo;
            int ALastTraceNo;

            int TaLast;

            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);

            //TEST
            if (InAtmNo == "AB102" || InAtmNo == "12507" || InAtmNo == "ServeUk102" || InAtmNo == "ABC501")
            {
                TaLast = Ta.LastTraceNo;
            }
            else
            {
                TaLast = Ta.LastTraceNo - 6;
            }

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool first = true;
            string SqlString = "SELECT *"
          + " FROM [dbo].[InPoolTrans] "
          + " WHERE AtmNo=@AtmNo AND TranNo>@WTranNo AND AtmTraceNo <= @AtmTraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@WTranNo", InTranNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", TaLast);

                        int TransType;
                        //       int CurrCode;
                        decimal TranAmount;
                        string CurrDesc;
                        bool SuccTran;
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            ValidTran = false; // Initialise Valid 

                            RecordFound = true;
                            if (first == true)
                            {
                                AFirstTraceNo = (int)rdr["AtmTraceNo"];
                                first = false;
                            }

                            // Take action on transaction you should 


                            ALastTraceNo = (int)rdr["AtmTraceNo"];

                            // Assign Values 
                            SystemTarget = (int)rdr["SystemTarget"];
                            TransType = (int)rdr["TransType"];

                            CurrDesc = rdr["CurrDesc"].ToString();
                            TranAmount = (decimal)rdr["TranAmount"];
                            SuccTran = (bool)rdr["SuccTran"];

                            if (SystemTarget == 1) // 
                            {
                                if (ALastTraceNo > SystemTargets1.LastTrace) ValidTran = true;
                            }
                            if (SystemTarget == 2) // 
                            {
                                if (ALastTraceNo > SystemTargets2.LastTrace) ValidTran = true;
                            }
                            if (SystemTarget == 3) // Other 1
                            {
                                if (ALastTraceNo > SystemTargets3.LastTrace) ValidTran = true;
                            }
                            if (SystemTarget == 4) // Other 2 
                            {
                                if (ALastTraceNo > SystemTargets4.LastTrace) ValidTran = true;
                            }
                            if (SystemTarget == 5) // Other 3 ... CASH CREATED BY SYSTEM 
                            {
                                if (ALastTraceNo > SystemTargets5.LastTrace) ValidTran = true;
                            }

                            if (SystemTarget == 9 & (TransType == 12 || TransType == 22)) // Cash IN and cash out transactions  
                            {
                                ValidTran = true;
                            }

                            if (ValidTran == true)
                            {
                                if (CurrDesc == HCurrNm1)
                                {
                                    if ((TransType == 11 || TransType == 12) & SuccTran)
                                    {
                                        // Debits 
                                        HAdjBal1 = HAdjBal1 - TranAmount;

                                    }
                                    if (TransType == 22 & SuccTran)
                                    {
                                        // Credits  
                                        HAdjBal1 = HAdjBal1 + TranAmount;
                                    }
                                }
                                if (CurrDesc == HCurrNm2)
                                {
                                    if ((TransType == 11 || TransType == 12) & SuccTran)
                                    {
                                        // Debits 
                                        HAdjBal2 = HAdjBal2 - TranAmount;
                                    }
                                    if (TransType == 22 & SuccTran)
                                    {
                                        // Credits  
                                        HAdjBal2 = HAdjBal2 + TranAmount;
                                    }
                                }
                                if (CurrDesc == HCurrNm3)
                                {
                                    if ((TransType == 11 || TransType == 12) & SuccTran)
                                    {
                                        // Debits 
                                        HAdjBal3 = HAdjBal3 - TranAmount;
                                    }
                                    if (TransType == 22 & SuccTran)
                                    {
                                        // Credits  
                                        HAdjBal3 = HAdjBal3 + TranAmount;
                                    }
                                }
                                if (CurrDesc == HCurrNm4)
                                {
                                    if ((TransType == 11 || TransType == 12) & SuccTran)
                                    {
                                        // Debits 
                                        HAdjBal4 = HAdjBal4 - TranAmount;
                                    }
                                    if (TransType == 22 & SuccTran)
                                    {
                                        // Credits  
                                        HAdjBal4 = HAdjBal4 + TranAmount;
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

                    CatchDetails(ex, WAtmNo);
                }
        }
        // ASSIGN HOST BALANCES 
        private void UpdateHostBal(string InCurrNm, decimal InHBal, decimal InHAdjBal, int InBalSets)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InCurrNm == Balances1.CurrNm)
            {
                Balances1.HostBal = InHBal;
                Balances1.HostBalAdj = InHAdjBal;

            }
            if (InCurrNm == Balances2.CurrNm & InBalSets > 1)
            {
                Balances2.HostBal = InHBal;
                Balances2.HostBalAdj = InHAdjBal;
            }
            if (InCurrNm == Balances3.CurrNm & InBalSets > 2)
            {
                Balances3.HostBal = InHBal;
                Balances3.HostBalAdj = InHAdjBal;
            }
            if (InCurrNm == Balances4.CurrNm & InBalSets > 3)
            {
                Balances4.HostBal = InHBal;
                Balances4.HostBalAdj = InHAdjBal;
            }
        }

        // READ Errors TO CALCULATE REFRESED BALANCES 
        //
        public decimal TotalOnErrorsAmt; 
        public void ReadAllErrorsTable(string InAtmNo, int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalOnErrorsAmt = 0; 

            ErrorsFound = false; // Related to errors 
            Ta.ReadSessionsStatusTraces(InAtmNo, InSesNo);
            // If NEW METHOD 

            DateTime Last_Cut_Off_Date;
            int WPreRMCycle = 0; 

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            Rjc.Find_GL_Cut_Off_Before_GivenDate(Ta.Operator, Ta.SesDtTimeEnd.Date);
            if (Rjc.RecordFound == true & Rjc.Counter == 0)
            {
               Last_Cut_Off_Date = Rjc.Cut_Off_Date;
                WPreRMCycle = Rjc.JobCycle;
            }
            else
            {
                Last_Cut_Off_Date = NullPastDate;
                WPreRMCycle = 0;
            }

            NumberOfErrors = 0;
            NumberOfErrJournal = 0;
            ErrJournalThisCycle = 0;
            NumberOfErrDep = 0;
            NumberOfErrHost = 0;
            ErrHostToday = 0;
            ErrOutstanding = 0;

            ErrorsAdjastingBalances = 0;

            Balances1.NubOfErr = 0;
            Balances2.NubOfErr = 0;
            Balances3.NubOfErr = 0;
            Balances4.NubOfErr = 0;
            Balances1.ErrOutstanding = 0;
            Balances2.ErrOutstanding = 0;
            Balances3.ErrOutstanding = 0;
            Balances4.ErrOutstanding = 0;
            Balances1.PresenterValue = 0;
            Balances2.PresenterValue = 0;
            Balances3.PresenterValue = 0;
            Balances4.PresenterValue = 0;

            int ErrId; int ErrType;
            string ErrDesc; int TraceNo;
            int UniqueRecordId; int TransType; string TransDescr;
            DateTime DateTime; bool NeedAction;

            string CurDes;
            bool DrCust; bool CrCust; bool UnderAction; bool DisputeAct;
            bool ManualAct; bool DrAtmCash; bool CrAtmCash;
            bool DrAtmSusp; bool CrAtmSusp; bool MainOnly;
    //        " AtmNo ='" + WAtmNo + "'"
    //+ " AND SesNo <=" + WSesNo
    //+ " AND "
    //+ "("
    //+ " OpenErr = 1 AND ErrType = 5 " // GL Error 
    //+ " OR (OpenErr=1 AND DateTime < @DateTime )"
    //+ " OR (OpenErr=0 AND ActionRMCycle = " + WRMCycle + ")"
    //+ " )  ";

            string SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE AtmNo = @AtmNo AND SesNo<=@SesNo "
          + " AND ("
          + " OpenErr = 1 AND ErrType = 5 " // GL Error 
          + " OR (OpenErr=1 AND DateTime < @DateTime )"
          + " OR (OpenErr=0 AND ActionRMCycle = @ActionRMCycle_Pre)"
          + " OR (OpenErr=0 AND ActionRMCycle = @ActionRMCycle_At)"
          + " OR (OpenErr=0 AND ActionRMCycle > @ActionRMCycle_Pre AND DateTime < @DateTime )" // To cover all from GL Balance at cut off till now
          + " )  ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                        cmd.Parameters.AddWithValue("@ActionRMCycle_Pre", WPreRMCycle);
                        cmd.Parameters.AddWithValue("@ActionRMCycle_At", Ta.Recon1.RecAtRMCycle);
                        cmd.Parameters.AddWithValue("@DateTime", Ta.SesDtTimeEnd);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ErrorsFound = true;

                            // Read error Details

                            ErrId = (int)rdr["ErrId"];
                            ErrType = (int)rdr["ErrType"];
                            ErrDesc = rdr["ErrDesc"].ToString();
                            SesNo = (int)rdr["SesNo"];
                            TraceNo = (int)rdr["TraceNo"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();
                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            UnderAction = (bool)rdr["UnderAction"];
                            DisputeAct = (bool)rdr["DisputeAct"];
                            ManualAct = (bool)rdr["ManualAct"];
                            DrAtmCash = (bool)rdr["DrAtmCash"];
                            CrAtmCash = (bool)rdr["CrAtmCash"];
                            DrAtmSusp = (bool)rdr["DrAtmSusp"];
                            CrAtmSusp = (bool)rdr["CrAtmSusp"];
                            MainOnly = (bool)rdr["MainOnly"];

                            NumberOfErrors = NumberOfErrors + 1;
                            // (ErrType = 1 || ErrType = 2 || ErrType = 5)
                            // Values
                            // 1 : Withdrawl EJournal Errors
                            // 2 : Mainframe Withdrawl Errors
                            // 3 : Deposit Errors Journal 
                            // 4 : Deposit Mainframe Errors
                            // 5 : Created by user Errors = eg moving to suspense Or ATM Cash
                            // 6 : Empty 
                            // 7 : Created System Errors 
                            // 
                            if (ErrType == 1)
                            {
                                NumberOfErrJournal = NumberOfErrJournal + 1;
                                if (SesNo == WReplCycle) ErrJournalThisCycle = ErrJournalThisCycle + 1; // Errors in this journal 
                            }

                            if (ErrType == 2) NumberOfErrHost = NumberOfErrHost + 1;

                            if (ErrType == 3) NumberOfErrDep = NumberOfErrDep + 1;

                            if (ErrType == 2) // FIND Todays Host errors 
                            {
                                int result = DateTime.Compare(DateTime.Date, DateTime.Today);

                                if (result == 0) // Equal dates or less
                                {
                                    // Not done Repl

                                    ErrHostToday = ErrHostToday + 1;
                                }
                            }

                            if (UnderAction == false & DisputeAct == false & ManualAct == false & ErrId < 200) ErrOutstanding = ErrOutstanding + 1;

                            // FIND NUMBER OF ERRORS PER CURRENCY 

                            if (CurDes == Balances1.CurrNm) Balances1.NubOfErr = Balances1.NubOfErr + 1;
                            if (CurDes == Balances2.CurrNm) Balances2.NubOfErr = Balances2.NubOfErr + 1;
                            if (CurDes == Balances3.CurrNm) Balances3.NubOfErr = Balances3.NubOfErr + 1;
                            if (CurDes == Balances4.CurrNm) Balances4.NubOfErr = Balances4.NubOfErr + 1;

                            if (UnderAction == false & DisputeAct == false & ManualAct == false & CurDes == Balances1.CurrNm & (ErrType == 1 || ErrType == 2)) Balances1.ErrOutstanding = Balances1.ErrOutstanding + 1;
                            if (UnderAction == false & DisputeAct == false & ManualAct == false & CurDes == Balances2.CurrNm & (ErrType == 1 || ErrType == 2)) Balances2.ErrOutstanding = Balances2.ErrOutstanding + 1;
                            if (UnderAction == false & DisputeAct == false & ManualAct == false & CurDes == Balances3.CurrNm & (ErrType == 1 || ErrType == 2)) Balances3.ErrOutstanding = Balances3.ErrOutstanding + 1;
                            if (UnderAction == false & DisputeAct == false & ManualAct == false & CurDes == Balances4.CurrNm & (ErrType == 1 || ErrType == 2)) Balances4.ErrOutstanding = Balances4.ErrOutstanding + 1;
                            // WSesNo == SesNo means we take into consideration errors for this SesNo
                            if (CurDes == Balances1.CurrNm & ErrId == 55 & WReplCycle == SesNo) Balances1.PresenterValue = Balances1.PresenterValue + ErrAmount;
                            if (CurDes == Balances2.CurrNm & ErrId == 55 & WReplCycle == SesNo) Balances2.PresenterValue = Balances2.PresenterValue + ErrAmount;
                            if (CurDes == Balances3.CurrNm & ErrId == 55 & WReplCycle == SesNo) Balances3.PresenterValue = Balances3.PresenterValue + ErrAmount;
                            if (CurDes == Balances4.CurrNm & ErrId == 55 & WReplCycle == SesNo) Balances4.PresenterValue = Balances4.PresenterValue + ErrAmount;

                            // MAKE ADJUSTMENTS ON BALANCES 
                            // SesNo = WSesNo means this is within this Repl Cycle
                            // 
                            if ((UnderAction == true & ManualAct == false & WFunction == 4 & SesNo == WReplCycle)
                                || (UnderAction == true & ManualAct == false & WFunction == 4 & SesNo != WReplCycle & ErrId > 100 & ErrId < 200)
                                || ((WFunction == 5 & ManualAct == false & NeedAction == true & SesNo == WReplCycle) & ErrId < 200)
                                || ((WFunction == 5 & ManualAct == false & NeedAction == true & SesNo != WReplCycle) & (ErrId > 100 & ErrId < 200))
                                )
                            {
                                ErrorsAdjastingBalances = ErrorsAdjastingBalances + 1;

                                //  if (DrCust == true )
                                if ((DrCust == true & CrAtmCash == true) || (CrAtmCash == true & DrAtmSusp == true))
                                {
                                    ErrAmount = -ErrAmount;
                                }
                                if (CurDes == Balances1.CurrNm)
                                {
                                    if (MainOnly == false) // eg Presenter Error 
                                    {
                                        // Adjust Machine balance
                                        Balances1.MachineBal = Balances1.MachineBal + ErrAmount;
                                        //Balances1.ReplToRepl = Balances1.ReplToRepl + ErrAmount;
                                        if (Operator == "CRBAGRAA")
                                        {
                                            Balances1.HostBalAdj = Balances1.HostBalAdj + (ErrAmount);
                                        }
                                        else
                                        {
                                          Balances1.HostBalAdj = Balances1.HostBalAdj + (ErrAmount);
                                        }
                                        
                                    }
                                    if (MainOnly == true) // eg Double Entry 
                                    {
                                        if (Operator == "CRBAGRAA")
                                        {
                                            Balances1.HostBalAdj = Balances1.HostBalAdj + (ErrAmount);
                                        }
                                        else
                                        {
                                            Balances1.HostBalAdj = Balances1.HostBalAdj + (ErrAmount);
                                            TotalOnErrorsAmt = TotalOnErrorsAmt + ErrAmount; 
                                        }
                                   
                                    }
                                }
                                if (CurDes == Balances2.CurrNm)
                                {
                                    if (MainOnly == false) // eg Presenter Error 
                                    {
                                        Balances2.MachineBal = Balances2.MachineBal + ErrAmount;
                                        Balances2.ReplToRepl = Balances2.ReplToRepl + ErrAmount;
                                        Balances2.HostBal = Balances2.HostBal + ErrAmount;
                                        Balances2.HostBalAdj = Balances2.HostBalAdj + ErrAmount;
                                    }
                                    if (MainOnly == true) // eg Double Entry 
                                    {
                                        Balances2.HostBal = Balances2.HostBal + ErrAmount;
                                        Balances2.HostBalAdj = Balances2.HostBalAdj + ErrAmount;
                                    }
                                }
                                if (CurDes == Balances3.CurrNm)
                                {
                                    if (MainOnly == false) // eg Presenter Error 
                                    {
                                        Balances3.MachineBal = Balances3.MachineBal + ErrAmount;
                                        Balances3.ReplToRepl = Balances3.ReplToRepl + ErrAmount;
                                        Balances3.HostBal = Balances3.HostBal + ErrAmount;
                                        Balances3.HostBalAdj = Balances3.HostBalAdj + ErrAmount;
                                    }
                                    if (MainOnly == true) // eg Double Entry 
                                    {
                                        Balances3.HostBal = Balances3.HostBal + ErrAmount;
                                        Balances3.HostBalAdj = Balances3.HostBalAdj + ErrAmount;
                                    }
                                }
                                if (CurDes == Balances4.CurrNm)
                                {
                                    if (MainOnly == false) // eg Presenter Error 
                                    {
                                        Balances4.MachineBal = Balances4.MachineBal + ErrAmount;
                                        Balances4.ReplToRepl = Balances4.ReplToRepl + ErrAmount;
                                        Balances4.HostBal = Balances4.HostBal + ErrAmount;
                                        Balances4.HostBalAdj = Balances4.HostBalAdj + ErrAmount;
                                    }
                                    if (MainOnly == true) // eg Double Entry 
                                    {
                                        Balances4.HostBal = Balances4.HostBal + ErrAmount;
                                        Balances4.HostBalAdj = Balances4.HostBalAdj + ErrAmount;
                                    }
                                }
                            }
                        }

                        if (ErrAmount < 0)
                        {
                            ErrAmount = -ErrAmount; // Turn it to its original value 
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

                    CatchDetails(ex, WAtmNo);
                }
        }

        public void ReadAllErrorsTable_Presenter_By_Date(string InAtmNo, int InSesNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

           
            ErrorsFound = false; // Related to errors 
            Ta.ReadSessionsStatusTraces(InAtmNo, InSesNo);

            Balances1.PresenterValue = 0;

           string  SqlString =
               " WITH TempTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE " //IsMatchingDone = 1 
               + "  TerminalId =@TerminalId "
               + " AND ( "
                  //+ " (Matched = 0 AND ActionType = '08') " // Move from Reconciliation 
                  // + " OR (Matched = 0 AND SeqNo06 = 8 ) " // At replenishment when we take action on 08
                  + "  (Matched = 1 AND MetaExceptionId = 55)"
                  + " OR (Matched = 0 AND MetaExceptionId = 55)"
                   + " )"
               + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
               + " UNION ALL  "
               + " SELECT * "
               + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE IsMatchingDone = 1  AND TerminalId =@TerminalId "
                 + " AND (Matched = 1  AND  MetaExceptionId = 55 ) "
                 + "  "
               + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
               + " ) "
               + " SELECT * FROM TempTbl "

               + " ORDER  By MetaExceptionId DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@FromDt", Ta.SesDtTimeStart);
                        cmd.Parameters.AddWithValue("@ToDt", Ta.SesDtTimeEnd);

                        cmd.CommandTimeout = 350; 

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //ErrorsFound = true;

                            string TransCurr = (string)rdr["TransCurr"];
                            decimal TransAmount = (decimal)rdr["TransAmount"];

                            Balances1.PresenterValue = Balances1.PresenterValue + TransAmount;

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

                    CatchDetails(ex, WAtmNo);
                }
        }
        //
        // UPDATE SESSION NOTES WITH NOTES 
        //
        public void UpdateSessionsNotesAndValues(string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //  UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsNotesAndValues  SET "
                           + " [CaptCardsMachine] = @CaptCardsMachine,[CaptCardsCount] = @CaptCardsCount, "
                           + " [ReplMethod] = @ReplMethod,[InUserDate] = @InUserDate,[InReplAmount] = @InReplAmount, "
                           + " [Cit_ExcelUpdatedDate] = @Cit_ExcelUpdatedDate, "
                           + " [Cit_UnloadedCounted] = @Cit_UnloadedCounted,[Cit_Over] = @Cit_Over,[Cit_Short] = @Cit_Short, [Cit_Loaded] = @Cit_Loaded, "
                           + " [GL_Balance] = @GL_Balance, IsNewAtm = @IsNewAtm, "
                           + " [Is_GL_Adjusted] = @Is_GL_Adjusted,[GL_Bal_Repl_Adjusted] = @GL_Bal_Repl_Adjusted, "
                           + " [ReplUserComment] = @ReplUserComment, "
                           + " [DiffAtAtmLevel_Cit] = @DiffAtAtmLevel_Cit,[DiffAtHostLevel_Cit] = @DiffAtHostLevel_Cit,[DiffWithErrors_Cit] = @DiffWithErrors_Cit,  "
                           + " [CasNo_51] = @CasNo_51, [CurName_51] = @CurName_51, [FaceValue_51] = @FaceValue_51,"
                        + " [InNotes_51] = @InNotes_51, [DispNotes_51] = @DispNotes_51,"
                        + " [RejNotes_51] = @RejNotes_51, [RemNotes_51] = @RemNotes_51, [CasCount_51] = @CasCount_51,"
                         + "  [RejCount_51] = @RejCount_51, [NewInSuggest_51] = @NewInSuggest_51,"
                         + " [NewInUser_51] = @NewInUser_51,"
                        + "[CasNo_52] = @CasNo_52, "
                        + " [CurName_52] = @CurName_52, [FaceValue_52] = @FaceValue_52, [InNotes_52] = @InNotes_52,"
                       + " [DispNotes_52] = @DispNotes_52, [RejNotes_52] = @RejNotes_52,"
                        + " [RemNotes_52] = @RemNotes_52, [CasCount_52] = @CasCount_52, [RejCount_52] = @RejCount_52,"
                       + " [NewInSuggest_52] = @NewInSuggest_52, [NewInUser_52] = @NewInUser_52, ["
                       + "CasNo_53] = @CasNo_53, [CurName_53] = @CurName_53, [FaceValue_53] = @FaceValue_53, "
                         + "[InNotes_53] = @InNotes_53, [DispNotes_53] = @DispNotes_53,"
                        + " [RejNotes_53] = @RejNotes_53, [RemNotes_53] = @RemNotes_53, [CasCount_53] = @CasCount_53,"
                       + " [RejCount_53] = @RejCount_53, [NewInSuggest_53] = @NewInSuggest_53,"
                       + " [NewInUser_53] = @NewInUser_53, [CasNo_54] = @CasNo_54, "
                       + " [CurName_54] = @CurName_54, [FaceValue_54] = @FaceValue_54, [InNotes_54] = @InNotes_54,"
                       + " [DispNotes_54] = @DispNotes_54, [RejNotes_54] = @RejNotes_54,"
                       + " [RemNotes_54] = @RemNotes_54, [CasCount_54] = @CasCount_54, [RejCount_54] = @RejCount_54,"
                       + " [NewInSuggest_54] = @NewInSuggest_54, [NewInUser_54] = @NewInUser_54,"
                         + " [CurName_55] = @CurName_55, [FaceValue_55] = @FaceValue_55, [InNotes_55] = @InNotes_55,"
                       + " [DispNotes_55] = @DispNotes_55, [RejNotes_55] = @RejNotes_55,"
                       + " [RemNotes_55] = @RemNotes_55, [CasCount_55] = @CasCount_55, [RejCount_55] = @RejCount_55,"
                       + " [NewInSuggest_55] = @NewInSuggest_55, [NewInUser_55] = @NewInUser_55,"
                            + " [ReplAmountSuggest] = @ReplAmountSuggest,[ReplAmountTotal] = @ReplAmountTotal,[InsuranceAmount] = @InsuranceAmount "
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@CaptCardsMachine", CaptCardsMachine);
                        cmd.Parameters.AddWithValue("@CaptCardsCount", CaptCardsCount);

                        cmd.Parameters.AddWithValue("@ReplMethod", ReplMethod);
                        cmd.Parameters.AddWithValue("@InUserDate", InUserDate);
                        cmd.Parameters.AddWithValue("@InReplAmount", InReplAmount);

                        cmd.Parameters.AddWithValue("@Cit_ExcelUpdatedDate", Cit_ExcelUpdatedDate);

                        cmd.Parameters.AddWithValue("@Cit_UnloadedCounted", Cit_UnloadedCounted);
                        cmd.Parameters.AddWithValue("@Cit_Over", Cit_Over);
                        cmd.Parameters.AddWithValue("@Cit_Short", Cit_Short);
                        cmd.Parameters.AddWithValue("@Cit_Loaded", Cit_Loaded);

                        cmd.Parameters.AddWithValue("@GL_Balance", GL_Balance);

                        cmd.Parameters.AddWithValue("@IsNewAtm", IsNewAtm);

                        cmd.Parameters.AddWithValue("@Is_GL_Adjusted", Is_GL_Adjusted);
                        cmd.Parameters.AddWithValue("@GL_Bal_Repl_Adjusted", GL_Bal_Repl_Adjusted);

                        cmd.Parameters.AddWithValue("@ReplUserComment", ReplUserComment);

                        cmd.Parameters.AddWithValue("@DiffAtAtmLevel_Cit", DiffAtAtmLevel_Cit);
                        cmd.Parameters.AddWithValue("@DiffAtHostLevel_Cit", DiffAtHostLevel_Cit);
                        cmd.Parameters.AddWithValue("@DiffWithErrors_Cit", DiffWithErrors_Cit);

                        cmd.Parameters.AddWithValue("@CasNo_51", Cassettes_1.CasNo);

                        cmd.Parameters.AddWithValue("@CurName_51", Cassettes_1.CurName);
                        cmd.Parameters.AddWithValue("@FaceValue_51", Cassettes_1.FaceValue);

                        cmd.Parameters.AddWithValue("@InNotes_51", Cassettes_1.InNotes);
                        cmd.Parameters.AddWithValue("@DispNotes_51", Cassettes_1.DispNotes);
                        cmd.Parameters.AddWithValue("@RejNotes_51", Cassettes_1.RejNotes);
                        cmd.Parameters.AddWithValue("@RemNotes_51", Cassettes_1.RemNotes);

                        cmd.Parameters.AddWithValue("@CasCount_51", Cassettes_1.CasCount);
                        cmd.Parameters.AddWithValue("@RejCount_51", Cassettes_1.RejCount);
                        cmd.Parameters.AddWithValue("@NewInSuggest_51", Cassettes_1.NewInSuggest);
                        cmd.Parameters.AddWithValue("@NewInUser_51", Cassettes_1.NewInUser);

                        cmd.Parameters.AddWithValue("@CasNo_52", Cassettes_2.CasNo);

                        cmd.Parameters.AddWithValue("@CurName_52", Cassettes_2.CurName);
                        cmd.Parameters.AddWithValue("@FaceValue_52", Cassettes_2.FaceValue);

                        cmd.Parameters.AddWithValue("@InNotes_52", Cassettes_2.InNotes);
                        cmd.Parameters.AddWithValue("@DispNotes_52", Cassettes_2.DispNotes);
                        cmd.Parameters.AddWithValue("@RejNotes_52", Cassettes_2.RejNotes);
                        cmd.Parameters.AddWithValue("@RemNotes_52", Cassettes_2.RemNotes);

                        cmd.Parameters.AddWithValue("@CasCount_52", Cassettes_2.CasCount);
                        cmd.Parameters.AddWithValue("@RejCount_52", Cassettes_2.RejCount);
                        cmd.Parameters.AddWithValue("@NewInSuggest_52", Cassettes_2.NewInSuggest);
                        cmd.Parameters.AddWithValue("@NewInUser_52", Cassettes_2.NewInUser);

                        cmd.Parameters.AddWithValue("@CasNo_53", Cassettes_3.CasNo);

                        cmd.Parameters.AddWithValue("@CurName_53", Cassettes_3.CurName);
                        cmd.Parameters.AddWithValue("@FaceValue_53", Cassettes_3.FaceValue);

                        cmd.Parameters.AddWithValue("@InNotes_53", Cassettes_3.InNotes);
                        cmd.Parameters.AddWithValue("@DispNotes_53", Cassettes_3.DispNotes);
                        cmd.Parameters.AddWithValue("@RejNotes_53", Cassettes_3.RejNotes);
                        cmd.Parameters.AddWithValue("@RemNotes_53", Cassettes_3.RemNotes);

                        cmd.Parameters.AddWithValue("@CasCount_53", Cassettes_3.CasCount);
                        cmd.Parameters.AddWithValue("@RejCount_53", Cassettes_3.RejCount);
                        cmd.Parameters.AddWithValue("@NewInSuggest_53", Cassettes_3.NewInSuggest);
                        cmd.Parameters.AddWithValue("@NewInUser_53", Cassettes_3.NewInUser);

                        cmd.Parameters.AddWithValue("@CasNo_54", Cassettes_4.CasNo);

                        cmd.Parameters.AddWithValue("@CurName_54", Cassettes_4.CurName);
                        cmd.Parameters.AddWithValue("@FaceValue_54", Cassettes_4.FaceValue);

                        cmd.Parameters.AddWithValue("@InNotes_54", Cassettes_4.InNotes);
                        cmd.Parameters.AddWithValue("@DispNotes_54", Cassettes_4.DispNotes);
                        cmd.Parameters.AddWithValue("@RejNotes_54", Cassettes_4.RejNotes);
                        cmd.Parameters.AddWithValue("@RemNotes_54", Cassettes_4.RemNotes);

                        cmd.Parameters.AddWithValue("@CasCount_54", Cassettes_4.CasCount);
                        cmd.Parameters.AddWithValue("@RejCount_54", Cassettes_4.RejCount);
                        cmd.Parameters.AddWithValue("@NewInSuggest_54", Cassettes_4.NewInSuggest);
                        cmd.Parameters.AddWithValue("@NewInUser_54", Cassettes_4.NewInUser);

                        cmd.Parameters.AddWithValue("@CasNo_55", Cassettes_5.CasNo);

                        cmd.Parameters.AddWithValue("@CurName_55", Cassettes_5.CurName);
                        cmd.Parameters.AddWithValue("@FaceValue_55", Cassettes_5.FaceValue);

                        cmd.Parameters.AddWithValue("@InNotes_55", Cassettes_5.InNotes);
                        cmd.Parameters.AddWithValue("@DispNotes_55", Cassettes_5.DispNotes);
                        cmd.Parameters.AddWithValue("@RejNotes_55", Cassettes_5.RejNotes);
                        cmd.Parameters.AddWithValue("@RemNotes_55", Cassettes_5.RemNotes);

                        cmd.Parameters.AddWithValue("@CasCount_55", Cassettes_5.CasCount);
                        cmd.Parameters.AddWithValue("@RejCount_55", Cassettes_5.RejCount);
                        cmd.Parameters.AddWithValue("@NewInSuggest_55", Cassettes_5.NewInSuggest);
                        cmd.Parameters.AddWithValue("@NewInUser_55", Cassettes_5.NewInUser);

                        cmd.Parameters.AddWithValue("@ReplAmountSuggest", ReplAmountSuggest);

                        cmd.Parameters.AddWithValue("@ReplAmountTotal", ReplAmountTotal);

                        cmd.Parameters.AddWithValue("@InsuranceAmount", InsuranceAmount);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();


                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, WAtmNo);
                }
        }

        // UPDATE SESSION NOTES CASH IN 
        public void UpdateSessionsNotesAndValues_CASH_In(string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows = 0; 

            //Na.Cassettes_1.InNotes = Cassette1;
            //Na.Cassettes_2.InNotes = Cassette2;
            //Na.Cassettes_3.InNotes = Cassette3;
            //Na.Cassettes_4.InNotes = Cassette4;

            //Na.ReplAmountTotal = InCashLoaded;
            //Na.InReplAmount = InCashLoaded;

            //  UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsNotesAndValues  SET "
                           + " [InNotes_51] = @InNotes_51 "
                             + " ,[InNotes_52] = @InNotes_52 "
                             + " ,[InNotes_53] = @InNotes_53 "
                             + " ,[InNotes_54] = @InNotes_54 "
                                   + " , [ReplAmountTotal] = @ReplAmountTotal "
                                     + " , [InReplAmount] = @InReplAmount"
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@InNotes_51", Cassettes_1.InNotes);
                        cmd.Parameters.AddWithValue("@InNotes_52", Cassettes_2.InNotes);
                        cmd.Parameters.AddWithValue("@InNotes_53", Cassettes_3.InNotes);
                        cmd.Parameters.AddWithValue("@InNotes_54", Cassettes_4.InNotes);

                        cmd.Parameters.AddWithValue("@ReplAmountTotal", ReplAmountTotal);
                        cmd.Parameters.AddWithValue("@InReplAmount", InReplAmount);

                        // cmd.Parameters.AddWithValue("@ReplUserComment", ReplUserComment);


                        // Execute and check success 
                        rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0)
                        //    {
                        //        UpdSesNotes = true;
                        //  textBoxMsg.Text = " ATMs Table UPDATED ";
                        //   }
                        //   else UpdSesNotes = false;//textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, WAtmNo);

                }
        }

        // UPDATE SESSION NOTES WITH NOTES 
        public void UpdateSessionsNotesAndValuesUserComment(string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //  UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsNotesAndValues  SET "
                           + " [ReplUserComment] = @ReplUserComment "
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@ReplUserComment", ReplUserComment);


                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0)
                        //    {
                        //        UpdSesNotes = true;
                        //  textBoxMsg.Text = " ATMs Table UPDATED ";
                        //   }
                        //   else UpdSesNotes = false;//textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, WAtmNo);

                }
        }

        // For Updating of ATM you update Cassettes for the not replenished yet
        //
        public void UpdateSessionsNotesAndValuesWithCassettes_1(string InAtmNo
            , decimal InFaceValue_51, decimal InFaceValue_52, decimal InFaceValue_53, decimal InFaceValue_54, decimal InFaceValue_55
            )
        {
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            Ta.ReadReplCycles_Not_TheReplenished(InAtmNo);

            try
            {

                int I = 0;
                int K = Ta.ATMsReplCyclesSelectedPeriod.Rows.Count - 1;

                while (I <= K)
                {

                    RecordFound = true;

                    string T_AtmNo = (string)Ta.ATMsReplCyclesSelectedPeriod.Rows[I]["AtmNo"];
                    int T_SesNo = (int)Ta.ATMsReplCyclesSelectedPeriod.Rows[I]["SesNo"];
                    int ProcessMode = (int)Ta.ATMsReplCyclesSelectedPeriod.Rows[I]["ProcessMode"];
                    
                    UpdateSessionsNotesAndValuesWithCassettes_2(T_AtmNo, T_SesNo
                        , InFaceValue_51, InFaceValue_52,  InFaceValue_53,  InFaceValue_54, InFaceValue_55
                        ); 

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                return;

            }

        }

        public void UpdateSessionsNotesAndValuesWithCassettes_2(string InAtmNo, int InSesNo
             , decimal InFaceValue_51, decimal InFaceValue_52, decimal InFaceValue_53, decimal InFaceValue_54, decimal InFaceValue_55
            )
        {
           
            ErrorFound = false;
            ErrorOutput = "";

            int rows = 0; 

            //  UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].SessionsNotesAndValues  SET "
                           + " [FaceValue_51] = @FaceValue_51 "
                            + " ,[FaceValue_52] = @FaceValue_52 "
                              + " ,[FaceValue_53] = @FaceValue_53 "
                                + " ,[FaceValue_54] = @FaceValue_54 "
                                  + " ,[FaceValue_55] = @FaceValue_55 "
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@FaceValue_51", InFaceValue_51);
                        cmd.Parameters.AddWithValue("@FaceValue_52", InFaceValue_52);
                        cmd.Parameters.AddWithValue("@FaceValue_53", InFaceValue_53);
                        cmd.Parameters.AddWithValue("@FaceValue_54", InFaceValue_54);
                        cmd.Parameters.AddWithValue("@FaceValue_55", InFaceValue_55);


                        // Execute and check success 
                        rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0)
                        //    {
                        //        UpdSesNotes = true;
                        //  textBoxMsg.Text = " ATMs Table UPDATED ";
                        //   }
                        //   else UpdSesNotes = false;//textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, WAtmNo);

                }
        }

        public void UpdateSessionsNotesAndValuesWithCountedFromCITExcel(string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows = 0;

            //  UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
      //[CasCount_51] = RemNotes_51
      //,[RejCount_51] = RejNotes_51
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].SessionsNotesAndValues  SET "
                           + " [CasCount_51] = RemNotes_51 "
                            + " ,[RejCount_51] = RejNotes_51 "
                            + " ,[CasCount_52] = RemNotes_52 "
                            + " ,[RejCount_52] = RejNotes_52 "
                            + " ,[CasCount_53] = RemNotes_53 "
                            + " ,[RejCount_53] = RejNotes_53 "
                            + " ,[CasCount_54] =RemNotes_54 "
                            + " ,[RejCount_54] =  RejNotes_54 "
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);
           
                        // Execute and check success 
                        rows = cmd.ExecuteNonQuery();
                        
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, WAtmNo);

                }
        }
        //
        // Catch Details 
        //
        private static void CatchDetails(Exception ex, string InAtmNo)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();
            
            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append(InAtmNo);
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                         + " . Application will be aborted! Call controller to take care. ");
            }

            //Environment.Exit(0);
        }

    }
}


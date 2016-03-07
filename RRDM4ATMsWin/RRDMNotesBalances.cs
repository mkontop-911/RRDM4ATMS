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
//multilingual
using System.Resources;
using System.Globalization;


namespace RRDM4ATMsWin
{
    class RRDMNotesBalances
    {
        /// <summary>
        /// Working Function 1 = Reconciliation from Repl to repl
        /// Working Function 2 = Show status of differences For ATM Only (No Host)
        /// Working Function 3 = Show status of differences For ATM and HOST
        /// Working Function 4 = Show status after correction was made - include actions on errors 
        /// Working Function 5 = Show status if action was taken on all errors 
        /// </summary>
    
        public int FirstTraceNo;
        public int LastTraceNo; 
        // Working variables

        string WAtmNo;
        int WSesNo;
        int WTranNo;
        int WFunction;

        public bool ErrorsFound; // Related to errors in journal 

    //    int ErrorsCounter;
       
     //   bool UpdSesNotes;
        // Variables for Reading Session Notes and also convert them to money 
        // Define Session Record  
    //    public int PreSes;
      //  public int NextSes;

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
        public decimal InReplAmount; 

        public decimal ReplAmountTotal;

        public decimal ReplAmountSuggest; 

        public decimal InsuranceAmount; 

        public string ReplUserComment; 

        public struct Cassettes
        {
           
            public int CasNo; // CasNo
   //         public int CurCode;
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

        public int ActiveCassettesNo; 


        bool One; bool Two; bool Three; bool Four;

        public struct Balances
        {
        //    public int CurrCd;
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

        public int BalSets; // How Many Balances we have

        public struct BalDiff // DIFFE IN BALANCES 
        {
        //    public int CurrCd;
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

    
        string HCurrNm1; decimal HBal1;
    
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
        public int ErrHostToday ; 
        public int ErrOutstanding; // Action was not taken on them 

        // Batch from Host fields 

        public int HBatchNo;
        public DateTime HDtTmStart;
        public DateTime HDtTmEnd;

     //   public DateTime LastHostTranDtTm;

        public string Operator; 

        public struct PhysicalCheck // Physical Check Structure  
        {
            public bool NoChips;
            public bool NoCameras; 
            public bool NoSuspCards;
            public bool NoGlue; 
            public bool NoOtherSusp; 
            public string OtherSuspComm;

            public bool Problem; 
            
        };

        public PhysicalCheck PhysicalCheck1; // Declare PhysicalCheck1 of type PhysicalCheck .. 

        // END of Declarations 
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMGasParameters Gp = new RRDMGasParameters();

        DateTime NullPastDate = new DateTime(1950, 11, 21); 

    //    string outcome = ""; // TO FACILITATE EXCEPTIONS 

        // END OF NEW METHODOLOGY 

      //  string WUserBank; 
        // READ Session Notes AND CALCULATE BALANCES 
        //
        public void ReadSessionsNotesAndValues(string InAtmNo, int InSesNo, int InFunction)
        {
            // initialise variables
          //  WUserBank = InUserBank; 
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WFunction = InFunction;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 


            string SqlString = "SELECT *"
                                + " FROM [dbo].[SessionsNotesAndValues] "
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
                        cmd.Parameters.AddWithValue("@SesNo", WSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true; 

                            SesNo = (int)rdr["SesNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            PreSes = (int)rdr["PreSes"];
                            NextSes = (int)rdr["NextSes"];
                            
                            AtmName= (string)rdr["AtmName"];
                            BankId= (string)rdr["BankId"];
                    
                            RespBranch= (string)rdr["RespBranch"];

                            ReplUserComment= (string)rdr["ReplUserComment"];

                            ReplAmountSuggest = (decimal)rdr["ReplAmountSuggest"];
                            ReplAmountTotal = (decimal)rdr["ReplAmountTotal"];

                            InsuranceAmount = (decimal)rdr["InsuranceAmount"];

                           // Capture cards 

                            CaptCardsMachine = (int)rdr["CaptCardsMachine"];
                            CaptCardsCount = (int)rdr["CaptCardsCount"];

                            ReplMethod = (int)rdr["ReplMethod"];
                            InUserDate = (DateTime)rdr["InUserDate"];
                            InReplAmount = (decimal)rdr["InReplAmount"];

                            // WHAT CASSETTES CONTAIN - FOUR CASSETTES 

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
                            Cassettes_2.DispNotes= (int)rdr["DispNotes_52"];
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

                            Operator = (string)rdr["Operator"];



                            // GET REPLENISHMENT TO REPLENISHMENT VALUES 
                            if (WFunction > 1)
                            {
                                Balances1.ReplToRepl = (decimal)rdr["ReplToReplAmt_1"];
                                Balances2.ReplToRepl = (decimal)rdr["ReplToReplAmt_2"];
                                Balances3.ReplToRepl = (decimal)rdr["ReplToReplAmt_3"];
                                Balances4.ReplToRepl = (decimal)rdr["ReplToReplAmt_4"];
                            }

                            // Number of active cassettes

                            ActiveCassettesNo = 0;

                            if (Cassettes_1.InNotes > 0) ActiveCassettesNo = ActiveCassettesNo + 1;
                            if (Cassettes_2.InNotes > 0) ActiveCassettesNo = ActiveCassettesNo + 1;
                            if (Cassettes_3.InNotes > 0) ActiveCassettesNo = ActiveCassettesNo + 1;
                            if (Cassettes_4.InNotes > 0) ActiveCassettesNo = ActiveCassettesNo + 1;

                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo); // READ TO FIND STATUS. 
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
                         //       Balances1.CurrCd = 0;
                                Balances1.CurrNm = " ";
                            }

                        //    Balances2.CurrCd = Cassettes_2.CurCode;
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
                          //      Balances2.CurrCd = 0;
                                Balances2.CurrNm = " ";
                            }

                        //    Balances3.CurrCd = Cassettes_3.CurName;
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
                      //          Balances3.CurrCd = 0;
                                Balances3.CurrNm = " ";
                            }

                       //     Balances4.CurrCd = Cassettes_4.CurCode;
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
                             //   Balances4.CurrCd = 0;
                                Balances4.CurrNm = " ";
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

                            // MAKE COMPRESSION .....  THE MERGING HAD CREATED EMPTY SETS. eg 1 and 2 is full but 3 might be empty and 4 full
                            ///                                  => Bring 4 to 3 => You create three continous sets 

                            // FILL ONE 

                            if (One == false & Two == true)
                            {
                                // Move 2 to 1
                            //    Balances1.CurrCd = Cassettes_2.CurCode;
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
                           //     Balances1.CurrCd = Cassettes_3.CurCode;
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
                             //   Balances1.CurrCd = Cassettes_4.CurCode;
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
                            // FILL TWO 
                            if (Two == false & Three == true)
                            {
                                // Move 3 to 2
                           //     Balances2.CurrCd = Cassettes_3.CurCode;
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
                             //   Balances2.CurrCd = Cassettes_4.CurCode;
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
                            // FILL THREE
                            if (Three == false & Four == true)
                            {
                                // Move 4 to 3
                           //     Balances3.CurrCd = Cassettes_4.CurCode;
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

                            // How many SETS of Balances ??? 

                            BalSets = 0; // How Many Balance Sets 

                            if (One == true) BalSets = BalSets + 1;
                            if (Two == true) BalSets = BalSets + 1;
                            if (Three == true) BalSets = BalSets + 1;
                            if (Four == true) BalSets = BalSets + 1;


                            if (WFunction == 1 & Ta.ProcessMode != -1) // This called only during UCForm51a 
                            {
                                // Till here we have a) ATM Balances for Notes and Cash per currency -
                                // Needed for General but to calculate replanishement to replenishement reconciliation too
                                // It can be called at any time 


                                ReadInPoolTransReplToRepl(WAtmNo, WSesNo); // READ TRANSACTIONS AND CALCULATE BALANCES FOR REPL TO REPL 

                                UpdateSessionsNotesAndValues2(WAtmNo, WSesNo); // UPDATE WITH REPL VALUES  

                                ReadAllErrorsTable(WAtmNo, WSesNo); // Find NumberOfErrors

                                return;

                            }

                            if (Ta.ProcessMode == -1) // We Do not have cassette data but we need to know what money we have in ATMs
                            {
                                // Find all DRs 
                                // Machine Balance = Open Balance - all DR

                                ReadInPoolTransReplToRepl(WAtmNo, WSesNo); // READ TRANSACTIONS AND CALCULATE BALANCES FOR REPL TO REPL 

                                UpdateSessionsNotesAndValues2(WAtmNo, WSesNo); // UPDATE WITH REPL VALUES  

                                Balances1.MachineBal = Balances1.ReplToRepl;
                                Balances2.MachineBal = Balances2.ReplToRepl;
                                Balances3.MachineBal = Balances3.ReplToRepl;
                                Balances4.MachineBal = Balances4.ReplToRepl;

                            }

                         //   MessageBox.Show("Number of Balances=" + BalSets.ToString());


                            if (WFunction >= 3) // Include HOST BALANCES IN BALANCES STRUCTURE  
                            {
                                // READ HOST ID BATCH 

                                // Get Host Balances and update structures 
/*
                                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                                if (Ta.LatestBatchNo > 0)
                                {
                                    HostBatchesClass Hb = new HostBatchesClass();

                                    Hb.ReadHostBatchesSpecific(Ta.LatestBatchNo); 
                                }
 */

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
                                    // Find minimum number.
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

                                UpdateHostBal(HCurrNm1, HBal1, HAdjBal1, BalSets);
                                UpdateHostBal(HCurrNm2, HBal2, HAdjBal2, BalSets);
                                UpdateHostBal(HCurrNm3, HBal3, HAdjBal3, BalSets);
                                UpdateHostBal(HCurrNm4, HBal4, HAdjBal4, BalSets);
/*
                                if (Ta.LatestBatchNo == 0) // First Time 
                                {
                                    HostBatchesClass Hb = new HostBatchesClass();

                                    Hb.ReadHostBatchesSpecific(Ta.LatestBatchNo);
                                }
*/

                                // Refressed with actions made by USER on Errors

                                if (WFunction == 4) ReadAllErrorsTable(WAtmNo,WSesNo); // READ ERRORS TO ADJUST BALANCES WITH THE ONES WITH ACTION TAKEN 

                                // OUTSTANDING // Refressed with actions all Actions suggested by SYSTEM on Errors - this will be an option 

                                if (WFunction == 5) ReadAllErrorsTable(WAtmNo, WSesNo); // READ ERRORS TO ADJUST BALANCES WITH ANY ERROR 

                           //     if (WFunction == 6) ReadAllErrorsTableClosed(WAtmNo,WSesNo); // READ ERRORS TO ADJUST BALANCES WITH Errors corrected and close during this Session  

                                // Calculate Differences per set of Balances USE Structures as Per Balances 

                                // DIFF FOR CURRENCY 1 // HOST LEVEL 

                             //   BalDiff1.CurrCd = Balances1.CurrCd;
                                BalDiff1.CurrNm = Balances1.CurrNm; 
                             //   BalDiff1.Host = Balances1.CountedBal - Balances1.HostBal;
                                BalDiff1.HostAdj = Balances1.CountedBal - Balances1.HostBalAdj;
                                if (BalDiff1.HostAdj != 0)
                                {
                                    BalDiff1.HostLevel = true;
                                }
                                else BalDiff1.HostLevel = false;

                                // DIFF FOR CURRENCY 2 // HOST LEVEL

                            //    BalDiff2.CurrCd = Balances2.CurrCd;
                                BalDiff2.CurrNm = Balances2.CurrNm; 
                                BalDiff2.Host = Balances2.CountedBal - Balances2.HostBal;
                                BalDiff2.HostAdj = Balances2.CountedBal - Balances2.HostBalAdj;
                                if (BalDiff2.HostAdj != 0)
                                {
                                    BalDiff2.HostLevel = true;
                                }
                                else BalDiff2.HostLevel = false;

                                // DIFF FOR CURRENCY 3 // HOST LEVEL

                                
                           //     BalDiff3.CurrCd = Balances3.CurrCd;
                                BalDiff3.CurrNm = Balances3.CurrNm; 
                                BalDiff3.Host = Balances3.CountedBal - Balances3.HostBal;
                                BalDiff3.HostAdj = Balances3.CountedBal - Balances3.HostBalAdj;
                                if (BalDiff3.HostAdj != 0)
                                {
                                    BalDiff3.HostLevel = true;
                                }
                                else BalDiff3.HostLevel = false;

                                // DIFF FOR CURRENCY 4 // HOST LEVEL
                               
                            //    BalDiff4.CurrCd = Balances4.CurrCd;
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
                            if (WFunction >= 2) // IN CASE WE ADJUST FOR THE ERRORS THESE MIGHT CHANGE 
                            {
                               
                        //        BalDiff1.CurrCd = Balances1.CurrCd;
                                BalDiff1.CurrNm = Balances1.CurrNm; 
                                BalDiff1.Machine = Balances1.CountedBal - Balances1.MachineBal;
                                BalDiff1.ReplToRepl = Balances1.CountedBal - Balances1.ReplToRepl;

                                if (BalDiff1.Machine != 0 || BalDiff1.ReplToRepl != 0)
                                {
                                    BalDiff1.AtmLevel = true;
                                }
                                else BalDiff1.AtmLevel = false;

                                // DIFF FOR CURRENCY 2 - ATM LEVEL

                         //       BalDiff2.CurrCd = Balances2.CurrCd;
                                BalDiff2.CurrNm = Balances2.CurrNm; 
                                BalDiff2.Machine = Balances2.CountedBal - Balances2.MachineBal;
                                BalDiff2.ReplToRepl = Balances2.CountedBal - Balances2.ReplToRepl;

                                if (BalDiff2.Machine != 0 || BalDiff2.ReplToRepl != 0)
                                {
                                    BalDiff2.AtmLevel = true;
                                }
                                else BalDiff2.AtmLevel = false;

                                // DIFF FOR CURRENCY 3 - ATM LEVEL
                                
                             //   BalDiff3.CurrCd = Balances3.CurrCd;
                                BalDiff3.CurrNm = Balances3.CurrNm; 
                                BalDiff3.Machine = Balances3.CountedBal - Balances3.MachineBal;
                                BalDiff3.ReplToRepl = Balances3.CountedBal - Balances3.ReplToRepl;

                                if (BalDiff3.Machine != 0 || BalDiff3.ReplToRepl != 0)
                                {
                                    BalDiff3.AtmLevel = true;
                                }
                                else BalDiff3.AtmLevel = false;

                                // DIFF FOR CURRENCY 4 - ATM LEVEL 
                                
                          //      BalDiff4.CurrCd = Balances4.CurrCd;
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
                                    ReadAllErrorsTable(WAtmNo,WSesNo);
                                }

                                if (ErrOutstanding > 0)
                                {
                                    DiffWithErrors = true;
                                }
                                else DiffWithErrors = false;

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
                    ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
                }
        }
        //
        // Read Transactions to CALCULATE REPL TO REPL 
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
                                    if (TransType == 21 & SuccTran )
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
                }
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
                          + "  [CurrNm_4] = @CurrNm_4, [ReplToReplAmt_4] = @ReplToReplAmt_4"
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


                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                     //   if (rows > 0)
                    //    {
                     //       UpdSesNotes = true;
                       //     textBoxMsg.Text = " ATMs Table UPDATED ";
                     ///   }
                      //  else UpdSesNotes = false ;// textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
                }
        }
        //
        // UPDATE SESSION NOTES WITH Physical Check
        //
        public void UpdateSessionsNotesAndValues3PhyCheck(string InAtmNo, int InSesNo)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

        //    UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsNotesAndValues  SET "
                           + " [NoChips] = @NoChips,[NoCameras] = @NoCameras, [NoSuspCards] = @NoSuspCards,"
                          + " [NoGlue] = @NoGlue,[NoOtherSusp] = @NoOtherSusp, [OtherSuspComm] = @OtherSuspComm"
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))

                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@NoChips", PhysicalCheck1.NoChips);
                        cmd.Parameters.AddWithValue("@NoCameras", PhysicalCheck1.NoCameras);
                        cmd.Parameters.AddWithValue("@NoSuspCards", PhysicalCheck1.NoSuspCards);
                        cmd.Parameters.AddWithValue("@NoGlue", PhysicalCheck1.NoGlue);
                        cmd.Parameters.AddWithValue("@NoOtherSusp", PhysicalCheck1.NoOtherSusp);
                        cmd.Parameters.AddWithValue("@OtherSuspComm", PhysicalCheck1.OtherSuspComm);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                 //       if (rows > 0)
                  //      {
                  //          UpdSesNotes = true;
                            //     textBoxMsg.Text = " ATMs Table UPDATED ";
                  //      }
                   //     else UpdSesNotes = false;// textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
                }
        }

        // READ Session Notes With physical Check values  
        //
        public void ReadSessionsNotesAndValues3PhyCheck(string InAtmNo, int InSesNo)
        {
            // initialise variables
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 


            string SqlString = "SELECT *"
                                + " FROM [dbo].[SessionsNotesAndValues] "
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

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            // READ Physical Check Values 
                            RecordFound = true; 
                            PhysicalCheck1.NoChips = (bool)rdr["NoChips"];
                            PhysicalCheck1.NoCameras = (bool)rdr["NoCameras"];
                            PhysicalCheck1.NoSuspCards = (bool)rdr["NoSuspCards"];
                            PhysicalCheck1.NoGlue = (bool)rdr["NoGlue"];
                            PhysicalCheck1.NoOtherSusp = (bool)rdr["NoOtherSusp"];
                            PhysicalCheck1.OtherSuspComm = (string)rdr["OtherSuspComm"];


                        }

                        // Close Reader
                        rdr.Close();

                        if (PhysicalCheck1.NoChips == false || PhysicalCheck1.NoCameras == false || PhysicalCheck1.NoSuspCards == false
                             || PhysicalCheck1.NoGlue == false || PhysicalCheck1.NoOtherSusp == false)
                        {
                            PhysicalCheck1.Problem =true; 
                        }
                        else PhysicalCheck1.Problem = false; 
                    }


                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
                }
        }
// READ BATCH TAble to take the last Batch 
//
public void ReadHostBatchesTable(string InAtmNo)
{
    RecordFound = false;
    ErrorFound = false;
    ErrorOutput = ""; 

    string SqlString = "SELECT *"
  + " FROM [dbo].[HostBatchesTable] "
    + " WHERE AtmNo=@AtmNo ";
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
            ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
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
            ErrorFound = true;
            ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
        }
    //TEST
    //
    if (InAtmNo == "AB102" || InAtmNo == "ServeUk102" || InAtmNo == "ABC501")
    {
        RecordFound = true;
        DateTime WDTma;
        //TEST
        WDTma = new DateTime(2014, 02, 28);
        SystemTargets1.LastTrace = 9 ;
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

}
        /*

public void ReadInPoolHostTrans(string InAtmNo)// Find the latest traces of targets 
{
    //
    // Read Transactions HOST
    //

    RecordFound = false;

    DateTime HostDtTime; 

    string SqlString = "SELECT *"
  + " FROM [dbo].[InPoolHost] "
  + " WHERE AtmNoH= @AtmNo ORDER BY AtmTraceNoH ASC ";
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

                int AtmTraceNo;

                while (rdr.Read())
                {
                    RecordFound = true;

                    AtmTraceNo = (int)rdr["AtmTraceNoH"];
                    HostDtTime = (DateTime )rdr["HostDtTimeH"];

                    // Assign Last Host Trans Dt Time 

                    LastHostTranDtTm = HostDtTime; 

                    // Assign Values 
                    SystemTarget = (int)rdr["SystemTarget"];


                    if (SystemTarget == 1) // JCC
                    {
                        SystemTargets1.LastTrace = AtmTraceNo;
                        SystemTargets1.DateTm = HostDtTime; 
                    }
                    if (SystemTarget == 2) // AMX 
                    {
                        SystemTargets2.LastTrace = AtmTraceNo;
                        SystemTargets2.DateTm = HostDtTime; 
                    }
                    if (SystemTarget == 3) // Other 1
                    {
                        SystemTargets3.LastTrace = AtmTraceNo;
                        SystemTargets3.DateTm = HostDtTime; 
                    }
                    if (SystemTarget == 4) // Other 2 
                    {
                        SystemTargets4.LastTrace = AtmTraceNo;
                        SystemTargets4.DateTm = HostDtTime; 
                    }
                    if (SystemTarget == 5) // Other 3
                    {
                        SystemTargets5.LastTrace = AtmTraceNo;
                        SystemTargets5.DateTm = HostDtTime; 
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

            string exception = ex.ToString();
        //    MessageBox.Show(ex.ToString());
            MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

        }
}
*/      
// Read Transactions find the Transaction Number for the minimum target Host trace no 
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
            ErrorFound = true;
            ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
        }
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

    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

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
                                if ((TransType == 11 ||TransType == 12)  & SuccTran)
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
                                if ( TransType == 22 & SuccTran)
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
            ErrorFound = true;
            ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
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
public void ReadAllErrorsTable(string InAtmNo, int InActionSes)
{
    RecordFound = false;
    ErrorFound = false;
    ErrorOutput = ""; 

    ErrorsFound = false; // Related to errors 

    NumberOfErrors = 0;
    NumberOfErrJournal = 0;
    ErrJournalThisCycle = 0;
    NumberOfErrDep = 0; 
    NumberOfErrHost = 0;
    ErrHostToday = 0; 
    ErrOutstanding = 0; 
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
    string ErrDesc;  int TraceNo; 
    int TransNo; int TransType; string TransDescr; 
    DateTime DateTime; bool NeedAction;
  //  int CurrCd; 
    string CurDes; 
    bool DrCust; bool CrCust ; bool UnderAction; 
    bool ManualAct; bool DrAtmCash ; bool CrAtmCash;
    bool DrAtmSusp ; bool CrAtmSusp ; bool MainOnly ;

    string SqlString = "SELECT *"
  + " FROM [dbo].[ErrorsTable] "
  + " WHERE AtmNo = @AtmNo AND SesNo<=@ActionSes AND (OpenErr=1 OR (OpenErr=0 AND ActionSes = @ActionSes))  ";
    using (SqlConnection conn =
                  new SqlConnection(connectionString))
        try
        {
            conn.Open();
            using (SqlCommand cmd =
                new SqlCommand(SqlString, conn))
            {
                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                cmd.Parameters.AddWithValue("@ActionSes", InActionSes);

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
                    TransNo = (int)rdr["TransNo"];
                    TransType = (int)rdr["TransType"];
                    TransDescr = rdr["TransDescr"].ToString();
                    DateTime = (DateTime)rdr["DateTime"];
                    NeedAction = (bool)rdr["NeedAction"];
       
                    CurDes = rdr["CurDes"].ToString();
                    ErrAmount = (decimal)rdr["ErrAmount"];
                    DrCust = (bool)rdr["DrCust"];
                    CrCust = (bool)rdr["CrCust"];
                    UnderAction = (bool)rdr["UnderAction"];
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
                    // 5 : Created by user Errors = eg moving to suspense 
                    // 6 : Empty 
                    // 7 : Created System Errors 
                    // 
                    if (ErrType == 1)
                    {
                        NumberOfErrJournal = NumberOfErrJournal + 1;
                        if (SesNo == WSesNo) ErrJournalThisCycle = ErrJournalThisCycle + 1; // Errors in this journal 
                    }

                    if (ErrType == 2) NumberOfErrHost = NumberOfErrHost + 1;

                    if (ErrType == 3) NumberOfErrDep = NumberOfErrDep + 1;

                    if (ErrType == 2) // FIND Todays Host errors 
                    {
                       int result = DateTime.Compare(DateTime.Date, DateTime.Today);

                                if (result == 0 ) // Equal dates or less
                                {
                                    // Not done Repl

                                    ErrHostToday = ErrHostToday + 1;
                                }
                    }

                    if (UnderAction == false & ManualAct == false & ErrId<200 ) ErrOutstanding = ErrOutstanding + 1; 

                    // FIND NUMBER OF ERRORS PER CURRENCY 

                    if (CurDes == Balances1.CurrNm) Balances1.NubOfErr = Balances1.NubOfErr + 1;
                    if (CurDes == Balances2.CurrNm) Balances2.NubOfErr = Balances2.NubOfErr + 1;
                    if (CurDes == Balances3.CurrNm) Balances3.NubOfErr = Balances3.NubOfErr + 1;
                    if (CurDes == Balances4.CurrNm) Balances4.NubOfErr = Balances4.NubOfErr + 1;

                    if (UnderAction == false & ManualAct == false & CurDes == Balances1.CurrNm & (ErrType == 1 || ErrType == 2) ) Balances1.ErrOutstanding = Balances1.ErrOutstanding + 1;
                    if (UnderAction == false & ManualAct == false & CurDes == Balances2.CurrNm & (ErrType == 1 || ErrType == 2) ) Balances2.ErrOutstanding = Balances2.ErrOutstanding + 1;
                    if (UnderAction == false & ManualAct == false & CurDes == Balances3.CurrNm & (ErrType == 1 || ErrType == 2) ) Balances3.ErrOutstanding = Balances3.ErrOutstanding + 1;
                    if (UnderAction == false & ManualAct == false & CurDes == Balances4.CurrNm & (ErrType == 1 || ErrType == 2) ) Balances4.ErrOutstanding = Balances4.ErrOutstanding + 1;
                    // WSesNo == SesNo means we take into consideration errors for this SesNo
                    if (CurDes == Balances1.CurrNm & ErrId == 55 & WSesNo == SesNo) Balances1.PresenterValue = Balances1.PresenterValue + ErrAmount;
                    if (CurDes == Balances2.CurrNm & ErrId == 55 & WSesNo == SesNo) Balances2.PresenterValue = Balances2.PresenterValue + ErrAmount;
                    if (CurDes == Balances3.CurrNm & ErrId == 55 & WSesNo == SesNo) Balances3.PresenterValue = Balances3.PresenterValue + ErrAmount;
                    if (CurDes == Balances4.CurrNm & ErrId == 55 & WSesNo == SesNo) Balances4.PresenterValue = Balances4.PresenterValue + ErrAmount;

                    // MAKE ADJUSTMENTS ON BALANCES 
                    // SesNo = WSesNo means this is within this Repl Cycle
                    // 
                    if ((UnderAction == true & ManualAct == false & WFunction == 4 & SesNo == WSesNo )
                        || (UnderAction == true & ManualAct == false & WFunction == 4 & SesNo != WSesNo & ErrId > 100 & ErrId < 200)
                        || (WFunction == 5 & ManualAct == false & NeedAction == true & SesNo == WSesNo)
                        || (WFunction == 5 & ManualAct == false & NeedAction == true & SesNo != WSesNo & ErrId > 100 & ErrId < 200)
                        )
                    {

                      //  if (DrCust == true )
                        if ((DrCust == true & CrAtmCash == true) || (CrAtmCash == true & DrAtmSusp == true))
                        {
                            ErrAmount = -ErrAmount;
                        }
                        if (CurDes == Balances1.CurrNm)
                        {
                            if (MainOnly == false) // eg Presenter Error 
                            {
                                Balances1.MachineBal = Balances1.MachineBal + ErrAmount;
                                Balances1.ReplToRepl = Balances1.ReplToRepl + ErrAmount;
                            //    Balances1.HostBal = Balances1.HostBal + ErrAmount;
                                Balances1.HostBalAdj = Balances1.HostBalAdj + ErrAmount;
                            }
                            if (MainOnly == true) // eg Double Entry 
                            {
                            //    Balances1.HostBal = Balances1.HostBal + ErrAmount;
                                Balances1.HostBalAdj = Balances1.HostBalAdj + ErrAmount;
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
            ErrorFound = true;
            ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
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

                cmd.Parameters.AddWithValue("@CasNo_51", Cassettes_1.CasNo);
           //     cmd.Parameters.AddWithValue("@CurCode_51", Cassettes_1.CurCode);
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
            //    cmd.Parameters.AddWithValue("@CurCode_52", Cassettes_2.CurCode);
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
           //     cmd.Parameters.AddWithValue("@CurCode_53", Cassettes_3.CurCode);
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
            //    cmd.Parameters.AddWithValue("@CurCode_54", Cassettes_4.CurCode);
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

                cmd.Parameters.AddWithValue("@ReplAmountSuggest", ReplAmountSuggest);

                cmd.Parameters.AddWithValue("@ReplAmountTotal", ReplAmountTotal);

                cmd.Parameters.AddWithValue("@InsuranceAmount", InsuranceAmount);

                // Execute and check success 
                int rows = cmd.ExecuteNonQuery();
           //     if (rows > 0)
           //     {
             //       UpdSesNotes = true;
                  //  textBoxMsg.Text = " ATMs Table UPDATED ";
             //   }
        //        else UpdSesNotes = false;//textBoxMsg.Text = " Nothing WAS UPDATED ";

            }
            // Close conn
            conn.Close();
        }
        catch (Exception ex)
        {
            conn.Close();
            ErrorFound = true;
            ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
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
            ErrorFound = true;
            ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;

        }
}

    }
}
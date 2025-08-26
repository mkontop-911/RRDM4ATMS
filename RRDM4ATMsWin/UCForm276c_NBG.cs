using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm276c_NBG : UserControl
    {
      //  public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        Form110 NForm110;
        Form112 NForm112;

        //bool ReconciliationAuthor;
        string StageDescr;
        int WAuthorSeqNumber;

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        bool NormalProcess;

      //  string WBankId;
        bool ViewWorkFlow;
        string WMode;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

    //    RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDM_Cit_ExcelOutputCycles Cec = new RRDM_Cit_ExcelOutputCycles();

        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

        //   int WDifStatus;
        string WOrigin;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;
        string WCitId;
        int WLoadingCycle;
      
        string WFunction; 

        //(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InOutputCycleNo, string InFunction)
        public void UCForm276c_NBG_Par(string InSignedId, int InSignRecordNo, string InOperator, string InCitId, int InLoadingCycle)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
           
            WCitId = WCategoryId = InCitId;

            WLoadingCycle = InLoadingCycle;
            
            WOrigin = "LoadingExcel";

            InitializeComponent();     

            // Update Step

            Usi.ReadSignedActivityByKey(WSignRecordNo);
    
            Usi.StepLevel = 2;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
            NormalProcess = false;

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor)
            {
                NormalProcess = false;
            }
            else NormalProcess = true;

            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageReconCateg(WCategoryId, WLoadingCycle, "LoadingExcel", WAuthoriser, WRequestor, Reject);
                guidanceMsg = Ap.MessageOut;
            }

            //************************************************************

            // CREATE THE TRANSACTIONS 
            // 
            if (NormalProcess == true)
            {
               // Aoc.DeleteOccurancesByLoadingExcelCycleNo(WLoadingCycle); 
                Create_TXNS_For_Validated();
            }   
            //
            //************************************************************

            SetScreen();

        }

        //*************************************
        // Set Screen
        //*************************************
        public void SetScreen()
        {
            TotalsAfterValidation(); 
            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageReconCateg(WCategoryId, WLoadingCycle, "LoadingExcel", WAuthoriser, WRequestor, Reject);
            }

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Closing stage for" + " CIT: " + WCategoryId + " Loading Cycle: " + WLoadingCycle;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (WViewFunction == true || WAuthoriser == true || WRequestor == true) // THIS is not normal process 
            {
                ViewWorkFlow = true;

                if (Cn.TotalNotes == 0)
                {
                    //label1.Hide();

                    buttonNotes2.Hide();
                    labelNumberNotes2.Hide();
                }
                else
                {
                    buttonNotes2.Show();
                    labelNumberNotes2.Show();
                }
            }
            else
            {
                buttonNotes2.Show();
                labelNumberNotes2.Show();
            }

            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WLoadingCycle, "LoadingExcel"); //

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        // guidanceMsg = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {

                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ProcessNo = 2; // Return to stage 2  
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                        NormalProcess = true; // TURN TO NORMAL TO SHOW WHAT IS NEEDED 

                        buttonNotes2.Show();
                        labelNumberNotes2.Show();
                    }

                }

            }

            // Show Authorisation record 
            ShowAuthorisationInfo();

          //  ChangeBoardMessage(this, new EventArgs());
        }


        // UPDATE AND FINISH BUTTON

        private void Create_TXNS_For_Validated()
        {
            // Update RRDM Replenishmet record
            //
            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

            RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMReconcCategories Rc = new RRDMReconcCategories();
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            RRDM_GL_Balances_For_Categories_And_Atms Gadj = new RRDM_GL_Balances_For_Categories_And_Atms();

            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            RRDMAccountsClass Acc = new RRDMAccountsClass();

            int TotalInDiff = 0;

            string WJobCategory = "ATMs";
            int WReconcCycleNo;
            string Message;

            decimal WUnloadedMachine;

            decimal WDiffGL;

            decimal WUnloadedMachineDep;

            decimal WCash_Loaded_Machine;

            int WSeqNo = 0;

            bool ShowMessage = true;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            string WReconCategGroup;
            string WAtmNo = "";
            int WSesNo = 0;
            // Read all Outstanding Matched Entries from G4S file
            // Make a loop and update 

            // Make Selection Of Validated Entries 
            int TempMode = 1;
            string SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode <> 1 "
                + " AND (Mask = '11' OR Mask = 'AA') AND LoadingExcelCycleNo=" + WLoadingCycle;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            if (G4.DataTableG4SEntries.Rows.Count == 0)
            {
                MessageBox.Show("There are no records to update");
                return;
            }

            int I = 0;

            while (I <= (G4.DataTableG4SEntries.Rows.Count - 1))
            {
                // GET ALL fields

                //    RecordFound = true;
                WSeqNo = (int)G4.DataTableG4SEntries.Rows[I]["SeqNo"];
                WAtmNo = (string)G4.DataTableG4SEntries.Rows[I]["AtmNo"];
                WSesNo = (int)G4.DataTableG4SEntries.Rows[I]["ProcessedAtReplCycleNo"];
                decimal WUnloadedCounted = (decimal)G4.DataTableG4SEntries.Rows[I]["UnloadedCounted"];
                decimal WCash_Loaded_CIT = (decimal)G4.DataTableG4SEntries.Rows[I]["Cash_Loaded"];
                decimal WDepositsCounted = (decimal)G4.DataTableG4SEntries.Rows[I]["Deposits"];
                string WMask = (string)G4.DataTableG4SEntries.Rows[I]["Mask"];

                DateTime ReplDate = (DateTime)G4.DataTableG4SEntries.Rows[I]["ReplDateG4S"]; 

                //UnloadedMachine = (decimal)rdr["UnloadedMachine"];
                //UnloadedCounted = (decimal)rdr["UnloadedCounted"];
                //Cash_Loaded = (decimal)rdr["Cash_Loaded"];

                //Deposits = (decimal)rdr["Deposits"];
                if (WMask == "11")
                {
                    // Do Transactions and close Reple Cycle = Make process mode to 2 
                    SelectionCriteria = " WHERE AtmNo ='" + WAtmNo
                           + "' AND ProcessedAtReplCycleNo =" + WSesNo;
                    TempMode = 2;
                    G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);
                    if (G4.RecordFound == false)
                    {
                        MessageBox.Show("RECORD NOT FOUND IN BANKING"); 
                    }
                    WUnloadedMachine = G4.UnloadedMachine;

                    WDiffGL = WUnloadedCounted - WUnloadedMachine;
                    if (WUnloadedMachine > 0)
                    {
                        CreateActions_Occurances_WithDrawels(WAtmNo, WSesNo, WUnloadedCounted, WDiffGL);
                    }


                    WUnloadedMachineDep = G4.Deposits;
                    WDiffGL = WDepositsCounted - WUnloadedMachineDep;
                    if (WUnloadedMachineDep > 0)
                    {
                        CreateActions_Occurances_Dep(WAtmNo, WSesNo, WUnloadedCounted, WDiffGL);
                    }

                    WCash_Loaded_Machine = G4.Cash_Loaded;
                    WDiffGL = WCash_Loaded_CIT - WCash_Loaded_Machine;

                    if (WCash_Loaded_Machine > 0)
                    {
                        CreateActions_Occurances_Load(WAtmNo, WSesNo, WCash_Loaded_Machine, WDiffGL);
                    }

                    // UPDATE LOADING excel CYCLE for each ATM 

                    

                   // Aoc.UpdateOccurancesLoadingExcelCycleNo(WAtmNo, WSesNo, WLoadingCycle); 


                }
                else
                {
                    // Do not make transactions and make process mode to 1 
                }

                I++; // Read Next entry of the table 

            }


            bool testlevelone = false;

            if (testlevelone == true)
            {
                // Get Unloaded Machine from Second = Bank record 
                SelectionCriteria = " WHERE AtmNo ='" + WAtmNo
                           + "' AND LoadingExcelCycleNo =" + WLoadingCycle;
                TempMode = 2;
                G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);
                WUnloadedMachine = G4.UnloadedMachine;

                // GET ALL fields from first 
                TempMode = 1;
                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, TempMode);

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                decimal UnlMachine = Na.Balances1.MachineBal;

                Ac.ReadAtm(WAtmNo); // Read Information for ATM 

                if (G4.Gl_Balance_At_CutOff > 0)
                {
                    // Insert record in GL File 
                    Gadj.OriginFileName = G4.OriginFileName;
                    Gadj.OriginalRecordId = G4.SeqNo;
                    Gadj.Cut_Off_Date = G4.Cut_Off_date;
                    Gadj.MatchingCateg = "";
                    Gadj.AtmNo = G4.AtmNo;
                    Gadj.Origin = "BANK";
                    Gadj.TransTypeAtOrigin = "GL Entry";

                    string ATMSuspence = "";
                    string ATMCash = "";

                    //if (Mpa.TargetSystem == 1) Acc.ReadAndFindAccount("1000", WOperator, WAtmNo, Er.CurDes, "ATM Suspense");

                    Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Ac.DepCurNm, "ATM Suspense");

                    if (Acc.RecordFound == true)
                    {
                        ATMSuspence = Acc.AccNo;
                    }
                    else
                    {
                        MessageBox.Show("ATM Suspense Account Not Found for ATM :" + WAtmNo);
                    }

                    Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Ac.DepCurNm, "ATM Cash");
                    if (Acc.RecordFound == true)
                    {
                        ATMCash = Acc.AccNo;
                    }
                    else
                    {
                        MessageBox.Show("ATM Cash Account Not Found for ATM :" + WAtmNo);
                    }

                    Gadj.GL_AccountNo = ATMCash;
                    Gadj.Ccy = Ac.DepCurNm;

                    if (WOperator == "ETHNCY2N")
                    {
                        // Make correction of input General Ledger
                        Gadj.GL_Balance = G4.Gl_Balance_At_CutOff - G4.Cash_Loaded + WUnloadedMachine;
                    }
                    else
                    {
                        Gadj.GL_Balance = G4.Gl_Balance_At_CutOff;
                    }

                    Gadj.DateCreated = DateTime.Now;
                    Gadj.Processed = false;

                    Gadj.ProcessedAtRMCycle = WReconcCycleNo;
                    Gadj.Operator = WOperator;
                    // THIS IS DONE FOR NATIONAL BANK
                    int GLSeqNo = Gadj.Insert_GL_Balances();
                }

                // ****************************
                // Set ANd Find Basic Information 
                // ****************************          

                WOperator = Ac.Operator;

                Rc.ReadReconcCategorybyGroupId(Ac.AtmsReconcGroup);

                WReconCategGroup = Rc.CategoryId;

                //**********************************

                // Update Notes and Values  

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                Na.InUserDate = DateTime.Now;
                Na.Cit_ExcelUpdatedDate = DateTime.Now;
                Na.Cit_UnloadedCounted = G4.UnloadedCounted;
                Na.Cit_Over = G4.OverFound;
                Na.Cit_Short = G4.ShortFound;
                Na.Cit_Loaded = G4.Cash_Loaded;

                // UPDATE OTHER USED FIELDS IN NA Record 
                Na.InReplAmount = G4.Cash_Loaded;
                Na.ReplAmountSuggest = G4.Cash_Loaded;
                Na.ReplAmountTotal = G4.Cash_Loaded;
                Na.InsuranceAmount = G4.Cash_Loaded;

                Na.ReplUserComment = G4.RemarksG4S;

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                // Find if for this ATM there is GL difference

                decimal GL_Adjusted = 0;

                GL_Adjusted = Gadj.FindAdjusted_GL_Balance_AND_Update_Session_First_Method(WOperator, WAtmNo, WSesNo, G4.ReplDateG4S);

                if (Gadj.IsDataFound == true)
                {
                    // THIS ASSIGNMENT ALready Done in Gadj.FindAdjusted_GL_Balance
                    //Na.Is_GL_Adjusted = true;
                    //Na.GL_Bal_Repl_Adjusted = GL_Adjusted;

                    // REFRESH Na
                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                }
                else
                {
                    // SET TO THIS for this new ATM 
                    // 

                    GL_Adjusted = G4.UnloadedCounted;

                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                    if (Na.IsNewAtm == true)
                    {
                        Na.Is_GL_Adjusted = true;
                    }
                    else
                    {
                        Na.Is_GL_Adjusted = false;
                    }

                    Na.GL_Bal_Repl_Adjusted = G4.UnloadedCounted;
                }

                // Initialise

                Na.DiffAtAtmLevel_Cit = false;
                Na.DiffAtHostLevel_Cit = false;
                Na.DiffWithErrors_Cit = false;

                if (G4.OverFound > 0 || G4.ShortFound > 0)
                {
                    Na.DiffAtAtmLevel_Cit = true;
                }
                if (G4.UnloadedCounted != GL_Adjusted)
                {
                    Na.DiffAtHostLevel_Cit = true;
                }
                Ec.ReadAllErrorsTableForCounterReplCycle(WOperator, WAtmNo, WSesNo);

                if (Ec.TotalErrorsAmtLess100 > 0)
                {
                    Na.DiffWithErrors_Cit = true;
                }

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                // UPDATE TRACES WITH FINISH 
                // Update all fields and Reconciliation mode = 2 if all reconcile and Host files available 
                // After "Replenishement and Before reconciliation 

                Ta.UpdateTracesFinishRepl_From__G4S(WAtmNo, WSesNo, WSignedId, WReconCategGroup);

                // READ LATEST FROM Ta
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (Ta.ProcessMode == 1)
                {
                    TotalInDiff = TotalInDiff + 1;
                }

                // READ AGAIN G4 Record 
                G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 1);

                // ************************************************************
                // CHECK GL DIFFERENCES 
                // ************************************************************

                // Ready for replenishment 

                // Compare GL_Adjusted VS G4.UnloadedCounted 

                if (G4.UnloadedCounted != GL_Adjusted || G4.OverFound != 0 || G4.ShortFound != 0 || Ec.NumOfErrors > 0)
                {
                    // There is difference

                    Rcs.ReadReconcCategoriesSessionsSpecific(WOperator, WReconCategGroup, WReconcCycleNo);

                    Rcs.GL_Original_Atms_Cash_Diff = Rcs.GL_Original_Atms_Cash_Diff + 1;

                    Rcs.GL_Remain_Atms_Cash_Diff = Rcs.GL_Remain_Atms_Cash_Diff + 1;

                    Rcs.UpdateReconcCategorySession_ForAtms_Cash_Diff(WReconCategGroup, WReconcCycleNo);

                }
                else
                {
                    Am.ReadAtmsMainSpecific(WAtmNo);
                    Am.Maker = "N/A";
                    Am.Authoriser = "N/A";
                    Am.UpdateAtmsMain(WAtmNo);
                }

                //
                // FINALLY UPDATE G4S Records 
                //

                // Update G4.Record.With Process Mode == 1
                G4.ProcessMode_Load = 1; // Updated Mode
                G4.UpdateCIT_G4S_Repl_EntriesRecord(WSeqNo, 1);

                // Read and Update Update corresponding Banks record  
                G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 2);
                G4.ProcessMode_Load = 1; // 
                G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 2);

                // CREATE TRANSACTIONS TO BE POSTED FOR REPLENISHMENT 
                // Whole Amount Goes to ATM Cash 
                if (G4.CITId != "1000")
                {
                    // Transactions = 1000 were made during replenishment
                    Ec.CreateTransTobepostedfromReplenishment_CIT(WOperator, WAtmNo, WSesNo, WSignedId, WSignedId, Na.Cit_UnloadedCounted, Na.Cit_Loaded);
                }


                //  

                // Update Cycle Record
                Cec.ReadExcelLoadCyclesBySeqNo(WLoadingCycle);

                TotalsAfterValidation(); // Call totals to assign values 

                //Cec.PresenterNumberEqual = TotalPresenterEqual;

                //Cec.PresenterDiffAmt = TotalPresenterDiffAmt;  

                // SET THESE TEMPORARILY TO ZERO

                //Cec.PresenterNumberEqual = 0;

                //Cec.PresenterDiffAmt = 0;

                //Cec.AtmsInGl_Differ = TotalInDiff;

                Cec.ProcessStage = 3;

                Cec.FinishDateTm = DateTime.Now;

                Cec.UpdateLoadExcelCycle(WLoadingCycle);

                if (TotalInDiff > 0)
                {
                    Form2 MessageForm = new Form2("Updating Done! " + Environment.NewLine
                                              + "Transactions for ATMs Cash GL account created for posting. " + Environment.NewLine
                                              + "There is need for GL reconciliation " + Environment.NewLine
                                              + "For :" + TotalInDiff.ToString() + "..ATM/s " + Environment.NewLine
                                              );
                    MessageForm.ShowDialog();
                }
                else
                {
                    Form2 MessageForm = new Form2("Updating Done! " + Environment.NewLine
                                             + "Transactions for ATMs Cash GL account created for posting. " + Environment.NewLine
                                             + "Not found differences."
                                             );
                    MessageForm.ShowDialog();
                }
                RRDMUserSignedInRecords Usr = new RRDMUserSignedInRecords();
                Usr.ReadSignedActivity(WSignedId);

            }


        }


        // Create Action Occurances
        DataTable TEMPTableFromAction;
        string WUniqueRecordIdOrigin = "Replenishment";
        public void CreateActions_Occurances_WithDrawels(string InAtmNo, int InSesNo, decimal InUnloadedCounted, decimal InDiffGL)
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 
            string WWAtmNo = InAtmNo;
            int WWSesNo = InSesNo;

            //int WFunction = 2;
            //Na.ReadSessionsNotesAndValues(WWAtmNo, WWSesNo, WFunction); // Read Values from NOTES

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            bool HybridRepl = false;

            Ac.ReadAtm(WWAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }
            //
            if (WOperator == "AUDBEGCA")
                HybridRepl = false;

            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;
            string WMaker_ReasonOfAction;

            DoubleEntryAmt = InUnloadedCounted;
            WUniqueRecordId = WWSesNo; // SesNo 
            WCcy = "EGP";

            if (HybridRepl == false)
            {
                // FIRST DOUBLE ENTRY 
                WActionId = "25"; // 25_DEBIT_ CIT Account/CR_AtmCash (UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment"

                WMaker_ReasonOfAction = "Un Load From ATM";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt,
                                                      WWAtmNo, WWSesNo, WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }


            //
            // CLEAR PREVIOUS ACTIONS FOR THIS REPLENISHMENT
            //
            if (HybridRepl == false)
            {
                WActionId = "29";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            if (HybridRepl == true)
            {
                WActionId = "39";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //
            WActionId = "30";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            // Delete create Dispute Shortage

            if (HybridRepl == false)
            {
                WActionId = "87";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            if (HybridRepl == true)
            {
                WActionId = "77";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //
            WActionId = "88";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            if (InDiffGL == 0)
            {
                // do nothing
            }

            if (InDiffGL > 0)
            {
                //MessageBox.Show("The amount of Difference:.." + InDiffGL.ToString("#,##0.00") + Environment.NewLine
                //         + "Will be moved to the Branch excess account "
                //    );
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
                WActionId = "30"; //30_CREDIT Branch Excess / DR_AtmCash(UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InDiffGL;
                WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (InDiffGL < 0)
            {
                //MessageBox.Show("The amount of Difference:.." + InDiffGL.ToString("#,##0.00") + Environment.NewLine
                //        + "Will be moved to the Branch shortage account "
                //         );
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                if (HybridRepl == false)
                {
                    WActionId = "29"; // 29_DEBIT_CIT Shortages/CR_AtmCash(UNLOAD)
                }
                if (HybridRepl == true)
                {
                    WActionId = "39"; // 29_DEBIT_Branch Shortages/CR_AtmCash(UNLOAD)
                }

                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -InDiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad From ATM-Shortage";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo,
                                                      WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;


            }

            // Handle Any Balance In Action Occurances 
            string WSelectionCriteria = "WHERE AtmNo ='" + WWAtmNo
                       + "' AND ReplCycle =" + WWSesNo
                       + " AND ( (Maker ='" + WSignedId + "' AND Stage<>'03') OR Stage = '03') ";

            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            if (Aoc.Current_DisputeShortage != 0)
            {
                //MessageBox.Show("Also note that Dispute Shortage will be handle here." + Environment.NewLine
                //         + "The Dispute Shortage is :" + Aoc.Current_DisputeShortage.ToString("#,##0.00") + Environment.NewLine
                //         + "Look at the resulted transactions");


                decimal CIT_Shortage = 0;
                decimal Shortage = 0;
                decimal Dispute_Shortage = -(Aoc.Current_DisputeShortage);
                decimal WExcess = Aoc.Excess;

                if (HybridRepl == false)
                {
                    CIT_Shortage = -(Aoc.CIT_Shortage);
                }
                if (HybridRepl == true)
                {
                    Shortage = -(Aoc.CIT_Shortage);
                }


                if (WExcess > 0)
                {
                    if (WExcess >= Dispute_Shortage)
                    {
                        // A
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WWSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = Dispute_Shortage;
                        WMaker_ReasonOfAction = "Settle Dispute Shortage";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                    }
                    else
                    {   // A
                        // Use all amount of Excess
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WWSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = WExcess; // Use all amount iin Excess
                        WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 1";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                        // The rest you take it from Shortage

                        decimal TempDiff1 = Dispute_Shortage - WExcess;
                        if (TempDiff1 > 0)
                        {
                            // Diff1 goes to Shortage
                            // B
                            if (HybridRepl == false)
                            {
                                WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            if (HybridRepl == true)
                            {
                                WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            // 
                            WUniqueRecordId = WWSesNo; // SesNo 
                            WCcy = "EGP";
                            DoubleEntryAmt = TempDiff1;
                            WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 2";
                            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                                  WActionId, WUniqueRecordIdOrigin,
                                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                                  , WMaker_ReasonOfAction, "Replenishment");

                            TEMPTableFromAction = Aoc.TxnsTableFromAction;
                        }

                    }
                }

                if ((CIT_Shortage > 0 || (WExcess == 0 & CIT_Shortage == 0) & HybridRepl == false)
                    || (Shortage > 0 || (WExcess == 0 & Shortage == 0) & HybridRepl == true)
                    )
                {
                    // 
                    if (HybridRepl == false)
                    {
                        WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                    }
                    if (HybridRepl == true)
                    {
                        WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT Branch Shortage
                    }
                    // 
                    WUniqueRecordId = WWSesNo; // SesNo 
                    WCcy = "EGP";
                    DoubleEntryAmt = Dispute_Shortage;
                    WMaker_ReasonOfAction = "Settle Dispute Shortage through Shortage";
                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WActionId, WUniqueRecordIdOrigin,
                                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                          , WMaker_ReasonOfAction, "Replenishment");

                    TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }

            }

        }

        // Create Action Occurances

        public void CreateActions_Occurances_Dep(string InAtmNo, int InSesNo, decimal InUnloadedCounted, decimal InDiffGL)
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 

            string WWAtmNo = InAtmNo;
            int WWSesNo = InSesNo;

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;
            decimal CassetteAmt;
            decimal RetractedAmt;
            //



            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool HybridRepl = false;

            Ac.ReadAtm(WWAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }

            DoubleEntryAmt = InUnloadedCounted;
            WUniqueRecordId = WWSesNo; // SesNo 
            WCcy = "EGP";
            string WMaker_ReasonOfAction;

            //DoubleEntryAmt = CassetteAmt + RetractedAmt;
            // FIRST DOUBLE ENTRY 
            if (HybridRepl == false)
            {
                WActionId = "26"; // 26_CREDIT CIT Account/DR_AtmCash (DEPOSITS)
                                  // WUniqueRecordIdOrigin = "Replenishment";

                //DoubleEntryAmt = Na.Balances1.CountedBal;
                WMaker_ReasonOfAction = "UnLoad Deposits";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            else
            {
                WActionId = "26";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }


            WActionId = "27";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            WActionId = "37";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            //
            WActionId = "28";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);


            if (InDiffGL > 0)
            {
                //MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
                //        + "Will be moved to the CIT excess account ");
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
                WActionId = "28"; //28_CREDIT Branch Excess/DR_AtmCash(DEPOSITS)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InDiffGL;
                WMaker_ReasonOfAction = "UnLoad Deposits-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (InDiffGL < 0)
            {
                //MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
                //        + "Will be moved to the CIT shortage account ");
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                if (HybridRepl == false)
                {
                    WActionId = "27"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
                }
                if (HybridRepl == true)
                {
                    WActionId = "37"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
                }

                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -InDiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad Deposits-Shortages";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }

            //WTotalForCust = SM.TotalForCust;
            //WTotalCommision = SM.TotalCommision;
            //if (HybridRepl == false)
            //{
            //    if (WTotalForCust > 0)
            //    {
            //        // CREATE TRANSACTIONS FOR FOREX 
            //        DoubleEntryAmt = WTotalForCust;
            //        // FIRST DOUBLE ENTRY 
            //        WActionId = "33"; // 33_CREDIT_FOREX_INTERMEDIARY/DR_ATM CASH
            //                          // WUniqueRecordIdOrigin = "Replenishment";
            //        WUniqueRecordId = WSesNo; // SesNo 
            //        WCcy = "EGP";
            //        //DoubleEntryAmt = Na.Balances1.CountedBal;
            //        WMaker_ReasonOfAction = "UnLoad Deposits Forex";
            //        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                              WActionId, WUniqueRecordIdOrigin,
            //                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                              , WMaker_ReasonOfAction, "Replenishment");


            //        TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //        // FOREX
            //        // FOREX 
            //        // FOREX
            //        if (WTotalCommision > 0)
            //        {
            //            // CREATE TRANSACTIONS FOR FOREX Commision 
            //            DoubleEntryAmt = WTotalCommision;
            //            // FIRST DOUBLE ENTRY 
            //            WActionId = "34"; // 34_CREDIT_FOREX_INTERMEDIARY/DR_Commision
            //                              // WUniqueRecordIdOrigin = "Replenishment";
            //            WUniqueRecordId = WSesNo; // SesNo 
            //            WCcy = "EGP";
            //            //DoubleEntryAmt = Na.Balances1.CountedBal;
            //            WMaker_ReasonOfAction = "UnLoad Deposits Forex";
            //            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                                  WActionId, WUniqueRecordIdOrigin,
            //                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                                  , WMaker_ReasonOfAction, "Replenishment");


            //            TEMPTableFromAction = Aoc.TxnsTableFromAction;

            //        }

            //        //// CREATE TRANSACTIONS FOR FOREX CIT
            //        //DoubleEntryAmt = WTotalForCust + WTotalCommision;
            //        //// FIRST DOUBLE ENTRY 
            //        //WActionId = "35"; // 35_CREDIT_CIT ACCOUNT GL/DR_Forex_Intermidiary(DEPOSITS)
            //        //                  // WUniqueRecordIdOrigin = "Replenishment";
            //        //WUniqueRecordId = WSesNo; // SesNo 
            //        //WCcy = "EGP";
            //        ////DoubleEntryAmt = Na.Balances1.CountedBal;
            //        //WMaker_ReasonOfAction = "UnLoad Deposits Forex";
            //        //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //        //                                      WActionId, WUniqueRecordIdOrigin,
            //        //                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //        //                                      , WMaker_ReasonOfAction, "Replenishment");


            //        //TEMPTableFromAction = Aoc.TxnsTableFromAction;

            //    }
            //}

        }

        public void CreateActions_Occurances_Load(string InAtmNo, int InSesNo, decimal InCashInAmount, decimal InDiffGL)
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool HybridRepl = false;

            string WWAtmNo = InAtmNo;
            int WWSesNo = InSesNo;

            Ac.ReadAtm(WWAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }

            // Make transaction if CIT
            if (HybridRepl == false)
            {
                // Create 
                // load transaction for CIT and Bank

                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WActionId;
                // string WUniqueRecordIdOrigin ;
                int WUniqueRecordId;
                string WCcy;
                decimal DoubleEntryAmt;

                // FIRST DOUBLE ENTRY 
                WActionId = "24"; // 24_CREDIT CIT Account/DR_AtmCash (LOAD)

                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InCashInAmount;
                string WMaker_ReasonOfAction = "Load ATM";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;

            }

        }

        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WLoadingCycle, "LoadingExcel"); //
            if ((Ap.RecordFound == true & Ap.OpenRecord == true)
                   || (Ap.RecordFound == true & Ap.OpenRecord == false & Ap.Stage == 5))
            {
                labelDtAuthRequest.Text = "Date of Request : " + Ap.DateOriginated.ToString();

                if (Ap.Stage == 1) StageDescr = "Authoriser Not Available yet.";
                if (Ap.Stage == 2) StageDescr = "Authoriser got the message. He will get action.";
                if (Ap.Stage == 3) StageDescr = "Authoriser took action. Requestor must act. ";
                if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                {
                    StageDescr = "Authorization accepted. Ready for Finish";
                }
                if ((Ap.Stage == 3 || Ap.Stage == 4) & Ap.AuthDecision == "NO")
                {
                    StageDescr = "Authorization REJECTED. ";
                    Color Red = Color.Red;
                    labelAuthStatus.ForeColor = Red;
                }

                if (Ap.Stage == 5) StageDescr = "Authorisation process is completed";
                if (Ap.Stage == 5 & Ap.AuthDecision == "NO")
                {
                    StageDescr = "Authorization REJECTED. ";
                    Color Red = Color.Red;
                    labelAuthStatus.ForeColor = Red;
                }

                labelAuthStatus.Text = "Current Status : " + StageDescr;

                Us.ReadUsersRecord(Ap.Requestor);
                labelRequestor.Text = "Requestor : " + Us.UserName;

                Us.ReadUsersRecord(Ap.Authoriser);
                labelAuthoriser.Text = "Authoriser : " + Us.UserName;

                textBoxComment.Text = Ap.AuthComment;

                WAuthorSeqNumber = Ap.SeqNumber;
                labelAuthHeading.Show();
                labelAuthHeading.Text = "AUTHORISER's SECTION FOR CATEGORY : " + WCategoryId;
                panelAuthor.Show();

                if (WViewFunction == true) // For View only 
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();
                    //buttonAuthorisations.Hide();
                    // Authoriser
                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }

                if (WAuthoriser == true & (Ap.Stage == 2 || Ap.Stage == 3)) // For Authoriser from author management 
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();
                    //buttonAuthorisations.Hide();
                    // Authoriser
                    buttonAuthorise.Show();
                    buttonReject.Show();
                    textBoxComment.ReadOnly = false;
                }

                if (WAuthoriser == true & Ap.Stage == 4) // For Authoriser from author management 
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();
                    //buttonAuthorisations.Hide();
                    // Authoriser
                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }

                if (WRequestor == true || NormalProcess) // For Requestor from author management 
                {
                    if (Ap.Stage < 3) // Not authorise yet
                    {
                        // Main buttons
                        buttonAuthor.Hide();
                        buttonRefresh.Show();
                        //buttonAuthorisations.Show();
                        // Authoriser
                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES") // Authorised and accepted
                    {
                        // Main buttons
                        buttonAuthor.Hide();
                        buttonRefresh.Hide();
                        //buttonAuthorisations.Hide();
                        // Authoriser
                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;

                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO") // Authorised but rejected
                    {
                        // Main buttons
                        buttonAuthor.Show();
                        buttonRefresh.Hide();
                        //buttonAuthorisations.Hide();
                        // Authoriser
                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;
                    }
                }
            }
            else
            {
                // THIS IS THE NORMAL ... You do not show the AUTH box 
                if (NormalProcess & WRequestor == false) // Normal Reconciliation 
                {
                    // Do not show Authorisation Section this will be shown after authorisation 
                    labelAuthHeading.Hide();
                    panelAuthor.Hide();
                    buttonRefresh.Hide();
                }
            }
        }
        // Authorise - choose authoriser 
        private void buttonAuthor_Click(object sender, EventArgs e)
        {
            // Check if Already in authorization process

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.WFieldChar1 = WCategoryId = WCitId;

            Usi.WFieldNumeric1 = WLoadingCycle;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


            Ap.ReadAuthorizationRecordByRMCategoryAndRMCycle(WCategoryId, WLoadingCycle);
            //Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReplOrders");

            if (Ap.RecordFound == true & Ap.OpenRecord == true)
            {
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                }
            }

            Ap.ReadAuthorizationRecordByRMCategoryAndRMCycle(WCategoryId, WLoadingCycle);
          

            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
            {
                MessageBox.Show("This Cycle Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process."
                                                          );
                return;
            }

            // Validate input 
            //    InputValidationAndUpdate("Authorisation");

            //      if (ErrorReturn == true) return;
            int WTranNo = 0;

            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management

            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, "", 0, AuthorSeqNumber, 0, WCategoryId, WLoadingCycle, "Normal");
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();

            //guidanceMsg = "A message was sent to authoriser. Refresh for progress monitoring.";
            //ChangeBoardMessage(this, e);
            guidanceMsg = "A message was sent to authoriser. Refresh for progress monitoring.";
           // ChangeBoardMessage(this, e);
        }

        void NForm110_FormClosed(object sender, FormClosedEventArgs e)
        {

            //************************************************************
            //************************************************************
            // AUTHOR PART
            //************************************************************
            WViewFunction = false;
            WAuthoriser = false;
            WRequestor = false;
            NormalProcess = false;
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor
            else NormalProcess = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WLoadingCycle, "LoadingExcel"); //


            if (WRequestor == true & Ap.Stage == 1)
            {
                guidanceMsg = "Message was sent to authoriser. Refresh for progress ";
                //ChangeBoardMessage(this, e);
            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                guidanceMsg = "Authorisation made. Workflow can finish! ";
                
            }

            if (NormalProcess) // Orginator has deleted authoriser 
            {
                guidanceMsg = "Please make authorisation ";
                //ChangeBoardMessage(this, e);
            }

            SetScreen();


        }
        // REFRESH 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle
                                                   (WCategoryId, WLoadingCycle, "LoadingExcel"); //

            if (Ap.Stage < 3)
            {
                MessageBox.Show("Authoriser didn't take action yet.");
                return;
            }

            ShowAuthorisationInfo();
        }

        // AUthorisation section - Authorise 
        private void buttonAuthorise_Click(object sender, EventArgs e)
        {
            UpdateAuthorRecord("YES");

            guidanceMsg = "Authorisation Made - Accepted ";

            ShowAuthorisationInfo();
        }

        // Reject 
        private void buttonReject_Click(object sender, EventArgs e)
        {
            if (textBoxComment.TextLength < 5)
            {
                MessageBox.Show("Please input comment to explain rejection");
                return;
            }
            UpdateAuthorRecord("NO");

            guidanceMsg = "Authorisation Made - Rejected ";
            //ChangeBoardMessage(this, e);

            ShowAuthorisationInfo();
        }
        // Update Authorization Record 
        private void UpdateAuthorRecord(string InDecision)
        {

            Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);
            if (Ap.OpenRecord == true)
            {
                Ap.AuthDecision = InDecision;
                if (textBoxComment.TextLength > 0)
                {
                    Ap.AuthComment = textBoxComment.Text;
                }
                Ap.DateAuthorised = DateTime.Now;
                Ap.Stage = 3;

                Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);

                if (InDecision == "YES")
                {
                    MessageBox.Show("Authorization ACCEPTED! by : " + labelAuthoriser.Text);
                    //this.Dispose();
                }
                if (InDecision == "NO")
                {
                    MessageBox.Show("Authorization REJECTED! by : " + labelAuthoriser.Text);
                    //   this.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Authorization record is not open. Requestor has closed it.");
                return;
            }
        }
        // HISTORY FOR AUTHORISATIONS 

        // Button History 
        private void buttonAuthHistory_Click(object sender, EventArgs e)
        {
            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", "", 0, WDisputeNo, WDisputeTranNo, WCategoryId, WLoadingCycle);
            NForm112.ShowDialog();
        }

        // NOTES 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Closing stage for" + " CIT: " + WCategoryId + " Loading Cycle: " + WLoadingCycle;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }
        // Totals after updating
        private void TotalsAfterValidation()
        {

            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

            int TempMode = 1;
            string SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingCycle;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTableAND_Totals(WOperator, WSignedId,
                                                       SelectionCriteria, TempMode);
            if ((G4.Total11 + G4.TotalAA + G4.Total01) > 0)
            {
            
            }
            else
            {
             
            }
            //textBoxTotal11.Text = G4.TotalNotProcessed.ToString();
            textBoxTotal11.Text = G4.Total11.ToString();
            textBoxTotalAA.Text = G4.TotalAA.ToString();
            textBoxTotal10.Text = G4.Total10.ToString();

            textBoxShort.Text = G4.TotalShort.ToString();
            textBoxPresenterNotEqual.Text = G4.TotalPresenter.ToString();

            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode <> 1 AND MASK NOT IN ('11','AA')  ";
            TempMode = 2;
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            textBoxTotal01.Text = G4.DataTableG4SEntries.Rows.Count.ToString();

        }
// ALL ACTIONS 
        private void buttonAllActions_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WSelectionCriteria;

            WSelectionCriteria = "WHERE LoadingExcelCycleNo =" + WLoadingCycle; 
               
            Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

            //string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            int WMode = 3; // Actions 
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();
        }
// ALL ACCOUNTING 
        private void buttonGLTxns_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "WHERE LoadingExcelCycleNo =" + WLoadingCycle;

            Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

            Aoc.ClearTableTxnsTableFromAction();

            int I = 0;

            while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
            {

                int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                int WMode2 = 1; // 

                Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey, Aoc.ActionId, Aoc.Occurance
                                                             , Aoc.OriginWorkFlow, WMode2);
                I = I + 1;
            }

            DataTable TempTxnsTableFromAction;
            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;

            Form14b_All_Actions NForm14b_All_Actions;

            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();
        }
    }
}

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class UCForm271b : UserControl
    {

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingOfTxnsFindOriginRAW Msf = new RRDMMatchingOfTxnsFindOriginRAW();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDM_BULK_IST_AndOthers_Records_ALL_2 Bist = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMAuthorisationProcess Au = new RRDMAuthorisationProcess();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMActions_GL Ag = new RRDMActions_GL();
        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

        //   string WUserOperator; 
        //public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool ViewWorkFlow;

        bool AudiType;

        int ComMode = 0;

        bool PresenterError;

        bool IsPseudoReplenishment; 

        bool Is_GL_Creation_Auto;
        bool Is_Presenter_InReconciliation;

        decimal CurrentExcess;
        decimal CurrentShortage;

        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;

        //bool ComingFromMeta;

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;

        int CallingMode; // 

        string WSubString;
        string WMask;
        string PRX;

        // NOTES END 
        bool IsAmtDifferent;

        DateTime LeftDt;

        int WUniqueRecordId;

        //int WMaskRecordIdLeft;

        string WCardNumberLeft;
        string WAccNumberLeft;
        string WTransCurrLeft;
        decimal WTransAmountLeft;
        //int WTraceNo; 

        string SelectionCriteria;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WRMCategoryId;
        int WRMCycleNo;
        string WOperator;
        string WMainCateg;
        string WAtmNo;
        int WSesNo;

        int WActionOrigin;

        public void UCForm271bPar(string InSignedId, int SignRecordNo, string InOperator, string InRMCategoryId, int InRMCycle, int InActionOrigin
                                                                    , string InAtmNo, int InSesNo)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WRMCategoryId = InRMCategoryId;
            WRMCycleNo = InRMCycle;
            WOperator = InOperator;
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            WMainCateg = WRMCategoryId.Substring(0, 4);

            WActionOrigin = InActionOrigin;  // 1 = Matching Actions coming from 271
                                             // 2 = Replenishment Actions coming from 51 

            InitializeComponent();

            //if (WOperator == "BDACEGCA")
            //{
            //    // ABE
            //    buttonAllAccounting.Hide();
            //}

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }

            this.DoubleBuffered = true;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }
            else
            {
                ViewWorkFlow = false;
            }

            //labelStep1.Text = "Invstigation and Force Matching for RM Category Id : " + WRMCategoryId;

            // Force Matching Reason
            Gp.ParamId = "714";
            comboBoxReasonOfAction.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxReasonOfAction.DisplayMember = "DisplayValue";

            // Hide what is needed 
            //***************************************

            //panel3.Hide();

            textBox3.Hide();
            // textBox14.Hide();

            comboBoxLeft.Hide();

            textBoxHeaderLeft.Text = "TRANSACTIONS THAT NEED ACTION";
            //textBoxHeaderRight.Text = "MATCHING MASK";
            textBoxLineDetails.Text = "DETAILS OF SELECTED";
            textBox5.Text = "ACTIONS ON SELECTED UNMATCHED";


            if (WMainCateg == "RECA" || WMainCateg == "EWB3"
                || WMainCateg == "NBG3" || WMainCateg == PRX + "2"
                )
            {
                comboBoxLeft.Items.Add("UnMatched");
                comboBoxLeft.Items.Add("Matched");
                comboBoxLeft.Text = "UnMatched";
            }

            RRDMReconcCategories Rc = new RRDMReconcCategories();
            RRDMActions_GL Ag = new RRDMActions_GL();

            //************************************************************
            // Check if it should be based on IST
            
            //
            // Auto Creation Of Transactions
            //
            string ParId = "945";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_GL_Creation_Auto = true;
                buttonAllAccounting.Show();
            }
            else
            {
                Is_GL_Creation_Auto = false;
                buttonAllAccounting.Hide();
                checkBoxChangeActionAmt.Hide();

            }
            // Presenter
            ParId = "946";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Presenter_InReconciliation = true;
            }

            //Rc.ReadReconcCategorybyCategId(WOperator, WRMCategoryId);
            if (WMainCateg == "RECA") // Leave it as it is
            {
                ShowActionsDropDown();
                
            }
            else
            {
                if (Is_GL_Creation_Auto == true)
                {
                    ComMode = 2;
                }
                else
                {
                    ComMode = 4;
                }
                comboBoxActionNm.DataSource = Ag.ReadTableToGet_ActionNm_Array_List(WOperator, ComMode);
                comboBoxActionNm.DisplayMember = "DisplayValue";
            }


            // ACTION TYPE
            // 00 ... No Action Taken 
            // 03 ... Move case to cash reconciliation 
            // 04 ... Force Match - Broken Disc Case  

            AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;
                }
                else
                {
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }

            if (AudiType == true)
            {
                labelExcess.Text = "ATM_Excess";
                labelShortage.Text = "ATM_Shortage";
            }

            

        }


        // SHOW SCREEN 
        string WSortCriteria;

        public void SetScreen()
        {

            try
            {
                if (WActionOrigin == 1)
                {
                    if (IsPseudoReplenishment == false)
                    {
                        //labelExcess.Hide();
                        //textBoxExcess.Hide();
                    }
                    
                    // Reconciliation 
                    if (comboBoxLeft.Text == "UnMatched")
                    {
                        if (ViewWorkFlow == true) // View Only 
                        {
                            if (Is_Presenter_InReconciliation == true)
                            {
                                SelectionCriteria = " WHERE RMCateg ='"
                                               + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo
                                               + " AND IsMatchingDone = 1 AND FastTrack = 0 "
                                               + " AND  ("
                                               + " (Matched = 0  " + " AND ActionType != '07') "
                                               + "  OR (Matched = 1 AND MetaExceptionId = 55 " + " AND ActionType != '07') )"
                                               ;
                            }
                            else
                            {
                                SelectionCriteria = " WHERE RMCateg ='"
                                               + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo
                                               + " AND IsMatchingDone = 1 AND FastTrack = 0 "
                                               + "  AND (Matched = 0 AND MetaExceptionId <> 55) AND SettledRecord = 1 " + " AND ActionType != '07' ";

                            }

                            CallingMode = 1; // View
                        }
                        else
                        {
                            if (Is_Presenter_InReconciliation == true)
                            {
                                // INCLUDE PRESENTER  
                                SelectionCriteria = " WHERE RMCateg ='" + WRMCategoryId + "'"
                                          + "  AND MatchingAtRMCycle = " + WRMCycleNo
                                          + "  AND IsMatchingDone = 1 AND FastTrack = 0 "
                                          + "  AND ( "
                                          + " (Matched = 0 AND SettledRecord = 0 " + " AND ActionType != '07') "
                                          + "  OR (Matched = 1 AND SettledRecord = 0 AND MetaExceptionId = 55 " + " AND ActionType != '07') "
                                          + ") "
                                          ;
                            }
                            else
                            {
                               
                                // Matching is done but not Settled 
                                SelectionCriteria = " WHERE RMCateg ='" + WRMCategoryId + "'"
                                          + "  AND MatchingAtRMCycle = " + WRMCycleNo
                                          + "  AND IsMatchingDone = 1 AND FastTrack = 0 "
                                          + "  AND (Matched = 0 AND MetaExceptionId <> 55) AND SettledRecord = 0 " + " AND ActionType != '07' ";

                            }

                            CallingMode = 2; // Updating 
                        }

                        textBoxHeaderLeft.Text = "EXCEPTIONS (UNMATCHED ATM TRANSACTIONS)";
                        //buttonMovedToUnMatched.Hide();
                    }

                    if (comboBoxLeft.Text == "Matched")
                    {
                        if (ViewWorkFlow == true) // View Only 
                        {
                            SelectionCriteria = " WHERE RMCateg ='"
                                              + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo + " Matched = 1 ";

                            CallingMode = 1; // View
                        }
                        else
                        {
                            SelectionCriteria = " WHERE RMCateg ='" + WRMCategoryId + "'"
                                              + " AND MatchingAtRMCycle =" + WRMCycleNo + " AND Matched = 1";

                            CallingMode = 1; // View 

                        }

                        textBoxHeaderLeft.Text = "MATCHED ATM TRANSACTIONS";
                        //buttonMoveToMatched.Hide();
                    }

                    WSortCriteria = " ORDER BY TerminalId, TransDate ";

                }
                // From Replenishment


                if (WActionOrigin == 2)
                {
                    // Find the dates of the Replenishment Cycle
                    labelExcess.Show();
                    textBoxExcess.Show();
                    RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
                    RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                    textBoxExcess.Text = (Na.Balances1.CountedBal - Na.Balances1.MachineBal).ToString("#,##0.00");

                    textBox1.Text = "REPL CYCLE";

                    DateTmSesStart = Ta.SesDtTimeStart;
                    DateTmSesEnd = Ta.SesDtTimeEnd;

                    if (ViewWorkFlow == true) // View Only 
                    {
                        //UpdateExceptionIds
                        //SelectionCriteria = " WHERE TerminalId ='" + WAtmNo + "' AND RMCateg ='" + WRMCategoryId + "'"
                        //          // + "  AND [ReplCycleNo] = " + WSesNo
                        //          + "  AND IsMatchingDone = 1 AND FastTrack = 0 "
                        //          + "  AND ((Matched = 0  " + " AND ActionType != '07') "
                        //          + "   OR (Matched = 0 AND SettledRecord = 1 " + " AND ActionType = '08') " // Records that are settled in Reconc and moved to Replenishment 
                        //          + "      OR (Matched = 1 AND MetaExceptionId = 55 " + " AND ActionType != '07')) "
                        //          ;
                        SelectionCriteria = " WHERE  "
                                        + "TerminalId ='" + WAtmNo + "' AND (MetaExceptionId = 55 " // we take all Presenter Errors
                                                                                                    //   + " OR MetaExceptionId = 225 OR MetaExceptionId = 226 "
                                                                                                    // Also we get all moved from Reconcilition 
                                                                                                    // This happens when at reconciliation we are not sure and we want to move it to replenishment
                                                                                                    // Repl can happened every day or every few days
                                        + " OR (Matched = 0 AND ActionType = '08' )" // Move from reconciliation
                                        + " OR (Matched = 0 AND ActionType = '05' )" // Move from Dispute
                                        + " OR (Matched = 0 AND SeqNo06 = 8 ) " // At replenishment when we take action on 08
                                                                                // We turn this to 8 to select it 
                                        + ") "
                                        ;
                        CallingMode = 1; // View
                    }
                    else
                    {

                        SelectionCriteria = " WHERE  "
                                        + "TerminalId ='" + WAtmNo + "' AND (MetaExceptionId = 55 "
                                        //+ " OR MetaExceptionId = 225 OR MetaExceptionId = 226 "
                                        + " OR (Matched = 0 AND ActionType = '08' )" // Move from reconciliation
                                         + " OR (Matched = 0 AND ActionType = '05' )" // Move from Dispute
                                        + " OR (Matched = 0 AND SeqNo06 = 8 ) " // At replenishment when we take action on 08
                                                                                // We turn this to 8 to select it 
                                        + ") ";
                        string WSortCriteria = "";


                        CallingMode = 2; // Updating 
                    }

                    textBoxHeaderLeft.Text = "EXCEPTIONS TRANSACTIONS ";

                    WSortCriteria = " ORDER BY TransDate ";

                }

                //No Dates Are selected

                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;
                if (WActionOrigin == 1)

                {
                    if (ViewWorkFlow == false) // Normal Reconciliation process
                    {
                        if (Is_Presenter_InReconciliation == true)
                        {
                            //Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTableFrom271b(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 2, WActionOrigin);
                            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 2);
                        }
                        else
                        {
                            // Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTableFrom271b(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 1, WActionOrigin);
                            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 1);
                        }
                    }

                    if (ViewWorkFlow == true) // View Only - Reconciliation process
                    {
                        if (Is_Presenter_InReconciliation == true)
                        {
                            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTableFrom271b_VIEW(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 2, WActionOrigin);
                            // Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 2);
                        }
                        else
                        {
                            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTableFrom271b_VIEW(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 1, WActionOrigin);
                            //Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 1);
                        }
                    }


                }
                if (WActionOrigin == 2)
                {
                    // From Replenishment

                    //Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTableFrom271b(WOperator, WSignedId, CallingMode, SelectionCriteria,
                    //                 WSortCriteria, DateTmSesStart, DateTmSesEnd, 2, WActionOrigin);
                    FromDt = DateTmSesStart;
                    ToDt = DateTmSesEnd;

                    if (ViewWorkFlow == false) // Normal Replenishment process
                    {
                        Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 2);
                    }

                    if (ViewWorkFlow == true) // VIEW ONLY - Replenishment process
                    {
                        Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 2);
                    }

                }

                ShowGrid();

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

            if (WActionOrigin == 2 )
            {
                // SHOW Excess
                ShowExcessIfReplenishement(WAtmNo, WSesNo); 
                

            }
            else
            {
                if (IsPseudoReplenishment == true)
                {
                    labelExcessShortage.Show();
                    panelExcessShortage.Show();
                }
                else
                {
                    //labelExcessShortage.Hide();
                    //panelExcessShortage.Hide();
                }
                
            }
        }
        bool IsReversal;
        bool JournalFound;
        string DiscrepancyMessage;
        string FirstActionType;
        // ON Left Grid Row Enter 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WUniqueRecordId = (int)rowSelected.Cells[0].Value;

            SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            if (Mpa.RecordFound == true)
            {
                FirstActionType = Mpa.ActionType; // Leave it here 

                if (WActionOrigin == 1 & ViewWorkFlow == true)
                {
                    Aoc.ReadActionsOccurancesByUniqueRecordIdFirstAction(WUniqueRecordId, "Reconciliation");
                    if (Aoc.RecordFound == true)
                    {
                        FirstActionType = Aoc.ActionId;
                    }
                    else
                    {
                        FirstActionType = Mpa.ActionType;
                    }
                }
                if (WActionOrigin == 2 & ViewWorkFlow == true)
                {
                    Aoc.ReadActionsOccurancesByUniqueRecordIdFirstAction(WUniqueRecordId, "Replenishment");
                    if (Aoc.RecordFound == true)
                    {
                        FirstActionType = Aoc.ActionId;
                    }
                    else
                    {
                        FirstActionType = Mpa.ActionType;
                    }
                }


                // SET AS EQUIVALENT 
                WActionId = FirstActionType;

                Aoc.ReadActionsOccurancesByUniqueRecordIdActionIdLastAction(WUniqueRecordId);

                if (Aoc.RecordFound)
                {
                    textBoxActionOrigin.Show();
                    textBoxActionOrigin.Text = "Last Action taken at workflow.." + Aoc.OriginWorkFlow;
                }
                else
                {
                    textBoxActionOrigin.Hide();
                }

                // Everything is Fine for this line
                if ((FirstActionType == "08" || FirstActionType == "05") & WActionOrigin == 2)
                {
                    // Update SeqNo06
                    Mpa.SeqNo06 = 8; // UPdate to service what was moved from reconciliation OR Disputes
                    Mpa.UpdateMatchingTxnsMasterPoolATMs_SeqNo06(WUniqueRecordId, Mpa.SeqNo06, 1);
                }
            }

            

            // Check mask 
            if (Mpa.MatchMask == "" & Mpa.IsMatchingDone == false)
            {
                MessageBox.Show("Matching Not done for this transaction");
                Mpa.MatchMask = "FFF";
            }

            Aoc.ReadActionsOccurancesByUniqueRecordId(WUniqueRecordId);
            if (Aoc.RecordFound)
            {
                if (Aoc.DoubleEntryAmt != Mpa.TransAmount)
                {
                    checkBoxChangeActionAmt.Checked = true; //  when this is set up the amount is set too
                    textBoxChangedActionAmt.Text = Aoc.DoubleEntryAmt.ToString("#,##0.00");
                }
                else
                {
                    checkBoxChangeActionAmt.Checked = false;
                }
            }
            else
            {
                checkBoxChangeActionAmt.Checked = false;
            }

            // See if found and it is in error
            RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();

            Jt.ReadJournalTextByTrace(WOperator, Mpa.TerminalId, Mpa.MasterTraceNo, Mpa.TransDate.Date);

            if (Jt.RecordFound == true)
            {
                JournalFound = true;
            }
            else
            {
                JournalFound = false;
            }

            DiscrepancyMessage = "";

            if (Mpa.Origin == "Our Atms")
            {
                if (Mpa.NotInJournal == true & JournalFound == false)
                {
                    // HIDE If not IN Journal either Result = OK or in Error
                    buttonJournalLines.Hide();
                    labelJournalNm.Hide();
                    textBoxJournalNm.Hide();
                }
                else
                {

                    buttonJournalLines.Show();

                    labelJournalNm.Show();
                    textBoxJournalNm.Show();
                    RRDMReconcFileMonitorLog Rflog = new RRDMReconcFileMonitorLog();
                    Rflog.GetRecordByFuid(Mpa.FuID);
                    textBoxJournalNm.Text = Rflog.FileName;
                }
                buttonJournal.Show();
                buttonVideoClip.Show();
                buttonSourceRecords.Show();

                //buttonPOS.Hide();

                // FIND OUT IF THIS TRANSACTION BELONGS TO 
                // ALREADY REPLENISHED CYCLE
                RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

                RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 
                Ta.ReadFindReplCycleForGivenDate(Mpa.TerminalId, Mpa.TransDate); 

                if (Ta.RecordFound == true & Ta.ProcessMode ==2)
                {
                    // For this transaction Replenishment has been done
                    IsPseudoReplenishment = true;

                    WAtmNo = Ta.AtmNo;
                    WSesNo = Ta.SesNo;

                    //ShowActionsDropDown(); 
                    ShowExcessIfReplenishement(Ta.AtmNo, Ta.SesNo); 
                }
                else
                {
                    if (WActionOrigin == 1)
                    {
                        IsPseudoReplenishment = false;
                        //ShowActionsDropDown();

                        CurrentExcess = 0;
                        CurrentShortage = 0;
                        WAtmNo = Mpa.TerminalId;
                        ShowExcess_Reconciliation(WAtmNo, WUniqueRecordId);

                    }

                    //  labelExcessShortage.Hide();
                    // panel8.Hide();
                }
            }
            else
            {
                // JCC 
                buttonJournalLines.Hide();
                buttonJournal_Near.Hide();
                buttonJournal.Hide();
                buttonVideoClip.Hide();
                buttonSourceRecords.Show();
                //buttonPOS.Show();
            }

            if (Mpa.Origin != "Our Atms")
            {
                // Check if Used in our ATMs
                RRDMAtmsClass Ac = new RRDMAtmsClass();
                Ac.ReadAtm(Mpa.TerminalId);
                if (Ac.RecordFound == true & Mpa.MatchingCateg != PRX + "250" & Mpa.MatchingCateg != PRX + "251"
                    //& Mpa.MatchingCateg != PRX + "240"
                    )
                {
                    // GET FUID 
                    //RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();

                    String JournalId = "";
                    int WTraceNo = Mpa.AtmTraceNo;
                    int Mode; // FULL

                    //if (WOperator == "BCAIEGCX")
                    //{
                    int WSeqNoA = 0;
                    int WSeqNoB = 0;

                    Jt.ReadJournalTxnsByParameters(WOperator, Mpa.TerminalId, Mpa.AtmTraceNo * 10, Mpa.TransAmount, Mpa.CardNumber, Mpa.TransDate.Date);

                    if (Jt.RecordFound)
                    {
                        WSeqNoA = Jt.SeqNo;
                        WSeqNoB = Jt.SeqNo;
                        Mpa.FuID = Jt.FuId;
                    }

                    labelJournalNm.Show();
                    textBoxJournalNm.Show();
                    RRDMReconcFileMonitorLog Rflog = new RRDMReconcFileMonitorLog();
                    if (Mpa.FuID != 0)
                    {
                        Rflog.GetRecordByFuid(Mpa.FuID);
                        if (Rflog.RecordFound == true)
                            textBoxJournalNm.Text = Rflog.FileName;
                    }
                    else
                    {
                        textBoxJournalNm.Text = "";
                    }


                    //buttonJournalLines.Show();
                    buttonJournal_Near.Show();
                    buttonJournal.Show();
                }
                else
                {
                    buttonJournalLines.Hide();
                    buttonJournal_Near.Hide();
                    buttonJournal.Hide();
                }
            }

            IsReversal = false;
            IsAmtDifferent = false;
            buttonAssignPartner.Hide();

            DiscrepancyMessage = "";
            // 
            if (Mpa.MatchMask.Contains("R") == true)
            {
                DiscrepancyMessage = "There is Reversal in the matching records. " + Environment.NewLine
                             + "If in position there is letter R then this was Reversed. " + Environment.NewLine
                             + "See them by pressing Source Records Button ";

                IsReversal = true;
            }

            //
            if ((WRMCategoryId == PRX + "231" || WRMCategoryId == PRX + "233") & Mpa.MatchMask.Substring(1, 1) == "0"
                                                                                       & IsReversal == false)
            {
                // POS TRANSACTION
                string WSelectionCriteria;
                int WMode = 1;
                string WTableA = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]";
                string WTableB = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]";

                if (Mpa.TransType == 11)
                {
                    // Search first in current table 
                    WSelectionCriteria = " WHERE "
                                       + "RRNumber ='" + Mpa.RRNumber + "'"
                                       + " AND MatchingCateg ='" + WRMCategoryId + "'"
                                       + " AND Card_Encrypted ='" + Mpa.Card_Encrypted + "'"
                                       + " AND Processed = 0 "
                                       ;

                    WMode = 1; // Select from current table 
                    Mgt.ReadTable_BySelectionCriteria(WTableA, "", WSelectionCriteria, WMode);

                    if (Mgt.RecordFound == true)
                    {
                        if (Mpa.TransAmount != Mgt.TransAmt)
                        {
                            int SavedSeqNo = Mgt.SeqNo;
                            string SavedAccNo = Mgt.AccNo;
                            DiscrepancyMessage = "There is a record with same details in IST. " + Environment.NewLine
                                 + "The amount is different. " + Environment.NewLine
                                 + "Amount is : " + Mgt.TransAmt.ToString("#,##0.00");

                            IsAmtDifferent = true;

                            decimal WCH_Amount = Bist.Read_SOURCE_Table_And_CH_Amount(Mgt.LoadedAtRMCycle, Mgt.RRNumber, Mgt.Card_Encrypted);

                            string WComments = "Auth Txn was found-Diff Amt";

                            Mpa.UpdateMpaRecordsWithPartnerDetails_POS(Mpa.SeqNo, SavedAccNo, SavedSeqNo.ToString(), WCH_Amount
                                                        , WComments, 1);
                        }
                        else
                        {
                            if (Mpa.TransDate.Date != Mgt.TransDate.Date)
                            {
                                DiscrepancyMessage = "There is a record with same details in IST. " + Environment.NewLine
                                 + "Amount is : " + Mgt.TransAmt.ToString("#,##0.00") + Environment.NewLine
                                 + "Dates are different. Look at the source records for details. ";
                            }
                            else
                            {
                                DiscrepancyMessage = "There is a record with same details in IST. " + Environment.NewLine
                                 // + "The amount is different. " + Environment.NewLine
                                 + "Amount is : " + Mgt.TransAmt.ToString("#,##0.00");
                            }

                        }

                    }
                    else
                    {
                        // Search in both tables
                        //
                        WSelectionCriteria = " WHERE "
                                      + "RRNumber ='" + Mpa.RRNumber + "'"
                                      + " AND MatchingCateg ='" + WRMCategoryId + "'"
                                      + " AND Card_Encrypted ='" + Mpa.Card_Encrypted + "'"
                                      + "  "
                                      ;
                        WMode = 2; // Select from both tables  
                        Mgt.ReadTable_BySelectionCriteria(WTableA, WTableB, WSelectionCriteria, WMode);

                        if (Mgt.RecordFound & Mgt.IsSecretAccNo == true)
                        {
                            DiscrepancyMessage = "There is a record with same details in IST. " + Environment.NewLine
                                + "AND Accno is SECRET. " + Environment.NewLine
                                + " ";
                        }
                        else
                        {
                            if (WRMCategoryId == PRX + "233")
                            {
                                // RECORD NOT FOUND THE ACCOUNT NUMBER Already Updated FOR PREPAID CARD 
                                DiscrepancyMessage = "NO record to matched. " + Environment.NewLine
                                       + " Account number for this transaction.  " + Environment.NewLine
                                            + "AccNo:.." + Mpa.AccNumber + Environment.NewLine
                                            + "  ";
                            }
                            else
                            {
                                //
                                // Here is BDC231
                                //
                                if (Mgt.RecordFound == true)
                                {
                                    DiscrepancyMessage = "Record found as already processed. " + Environment.NewLine
                                      + "  " + Environment.NewLine
                                           + "Look at Source Records.." + Mpa.AccNumber + Environment.NewLine
                                           + "  ";
                                }
                                else
                                {

                                    WSelectionCriteria = " WHERE "
                                                                         //+ " MatchingCateg ='" + WRMCategoryId + "'"
                                                                         + " Card_Encrypted ='" + Mpa.Card_Encrypted + "'"
                                                                         + " AND RRNumber <>'" + Mpa.RRNumber + "'" // NOT EQUAL RRN
                                                                         + " AND Processed = 1 "
                                                                         ;
                                    WMode = 2; // Look at both data bases and Create table
                                    Mgt.ReadTable_BySelectionCriteria(WTableA, WTableB, WSelectionCriteria, WMode);

                                    if (Mgt.RecordFound == true)
                                    {
                                        if (Mpa.AccNumber != "")
                                        {
                                            DiscrepancyMessage = "NO record to matched. " + Environment.NewLine
                                            + " However Partner has been assigned to get account number.  " + Environment.NewLine
                                                 + "AccNo:.." + Mpa.AccNumber + Environment.NewLine
                                                 + "  ";
                                        }
                                        else
                                        {
                                            DiscrepancyMessage = "NO record to matched. " + Environment.NewLine
                                            + "Assign Partner for this card to get the account number.  " + Environment.NewLine
                                                 + "" + Environment.NewLine
                                                 + "  ";
                                        }

                                        buttonAssignPartner.Show();
                                    }
                                    else
                                    {
                                        DiscrepancyMessage = "No additional information. ";
                                    }
                                }

                            }

                        }
                    }
                }
                else
                {
                    // TransType is 21 Credit to customer 
                    if (Mpa.ResponseCode == "200000" || Mpa.TransType > 20)
                    {
                        WSelectionCriteria = " WHERE "
                                      + " MatchingCateg ='" + WRMCategoryId + "'"
                                      + " AND Card_Encrypted ='" + Mpa.Card_Encrypted + "'"
                                      + " AND Processed = 1 "
                                      ;
                        WMode = 2; // Look at both data bases and Create table
                        Mgt.ReadTable_BySelectionCriteria(WTableA, WTableB, WSelectionCriteria, WMode);

                        if (Mgt.RecordFound == true)
                        {
                            if (Mpa.AccNumber == "")
                            {
                                // Mpa already updated with partner
                                DiscrepancyMessage = "There is a record/s that the card was used. " + Environment.NewLine
                                    + "This is a credit adjustment. " + Environment.NewLine
                                    + "Look at details by pressing Assign Partner. " + Environment.NewLine
                                    + "  ";
                            }
                            else
                            {
                                // Partner assigned with Accno
                                DiscrepancyMessage = "There is a record/s that the card was used. " + Environment.NewLine
                                    + "Look at details by pressing Assign Partner. " + Environment.NewLine
                                      + "Partner was assigned Accno:" + Mpa.AccNumber + Environment.NewLine
                                    + "  ";
                            }

                            buttonAssignPartner.Show();
                        }
                        else
                        {
                            DiscrepancyMessage = "No additional information. "
                                + "For this Credit Adjustment. " + Environment.NewLine
                                ;
                        }
                    }
                    else
                    {
                        DiscrepancyMessage = "No additional information. ";
                    }
                }
            }

            if (Mpa.Comments == "OLD_Transaction")
            {
                DiscrepancyMessage = "This is an old Transaction. " + Environment.NewLine
                             + "Corresponding Matching Transaction " + Environment.NewLine
                             + "Might Have been moved to history";
            }

            // NOTES 2 START  
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
            // NOTES 2 END

            // NOTES 3 START  
            Order = "Descending";
            WParameter4 = "Force Matching for" + " Category: " + WRMCategoryId + " Matching SesNo: " + WRMCycleNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            // NOTES 3 END

            WUniqueRecordId = Mpa.UniqueRecordId;

            WCardNumberLeft = Mpa.CardNumber;
            WAccNumberLeft = Mpa.AccNumber;
            WTransCurrLeft = Mpa.TransCurr;
            WTransAmountLeft = Mpa.TransAmount;

            textBoxAtmNo.Text = Mpa.TerminalId;
            textBoxCardNoLeft.Text = Mpa.CardNumber;
            textBoxAccNoLeft.Text = WAccNumberLeft;
            textBoxCurrLeft.Text = WTransCurrLeft;
            textBoxAmountLeft.Text = WTransAmountLeft.ToString("#,##0.00");
            textBoxDateLeft.Text = Mpa.TransDate.ToString();
            textBoxTXNSRC.Text = Mpa.TXNSRC;
            textBoxTXNDEST.Text = Mpa.TXNDEST;

            textBoxTraceNo.Text = Mpa.TraceNoWithNoEndZero.ToString();
            if ((FirstActionType == "08" || Mpa.SeqNo06 == 8) & WActionOrigin == 2)
            {
                textBoxLineDetails.Text = "DETAILS OF SELECTED" + " .. CAME FROM RECONC";
            }
            else
            {
                textBoxLineDetails.Text = "DETAILS OF SELECTED";
            }

            textBoxRRNumber.Text = Mpa.RRNumber.ToString();

            textBoxCAP_DATE.Text = Mpa.CAP_DATE.ToShortDateString();
            textBoxSET_DATE.Text = Mpa.SET_DATE.ToShortDateString();

            textBoxAuthNo.Text = Mpa.AUTHNUM;

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, Mpa.MatchingCateg);
            textBoxCategoryNm.Text = Mc.CategoryName;

            if (Mpa.RRNumber != "0" & Mpa.Origin != "Our Atms")
            {

                switch (Mpa.TerminalType)
                {
                    case "10":
                        {
                            textBox3.Show();
                            textBox3.Text = "It is an ATM";
                            // JCC ATM
                            if (Mpa.Origin == "Our Atms")
                            {
                                // Do nothing 
                            }
                            else
                            {
                                // Not Our ATMs
                                textBoxLineDetails.Text = "DETAILS OF SELECTED ATM TXN";
                                //buttonPOS.Hide();
                                //   buttonPOS.Text = "ATM Info";
                            }
                            break;
                        }
                    case "20":
                        {
                            // JCC POS
                            textBox3.Show();
                            textBox3.Text = "It is a POS";
                            textBoxLineDetails.Text = "DETAILS OF SELECTED POS";
                            // buttonPOS.Show();
                            //buttonPOS.Text = "Merchant Info";
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

            }

            ShowMask(Mpa.MatchMask);

            if (Mpa.MetaExceptionId == 55)
            {
                PresenterError = true;
            }
            else
            {
                PresenterError = false;
            }


            if (DiscrepancyMessage == "")
            {
                textBoxUnMatchedType.Text = Mpa.UnMatchedType;
                if (Mpa.MetaExceptionId == 55) textBoxUnMatchedType.Text = "PRESENTER ERROR";

            }
            else textBoxUnMatchedType.Text = DiscrepancyMessage;

            SelectionCriteria = " ErrId > 100 AND UniqueRecordId =" + Mpa.UniqueRecordId;

            Er.ReadErrorsAndFillShortTable(WOperator, SelectionCriteria);

            if (Er.RecordFound == true)
            {
                textBox2.Show();
                dataGridView2.Show();
                ShowGrid2();
            }
            else
            {
                textBox2.Hide();
                dataGridView2.Hide();
            }
            //
            // handle cases where replenishment cycle is finished by Maker and Checker 
            // but the txn of presenter error which is e-journal came later  
            //
            if (ViewWorkFlow == true & WActionOrigin == 2)
            {
                Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", WUniqueRecordId, FirstActionType);

                if (Aoc.RecordFound == true)
                {
                    // OK 
                }
                else
                {
                    // Not within this replenishement

                    MessageBox.Show("This TXN came after the Repl Cycle was completed!");
                    //

                }
            }

            //if (WUniqueRecordId == 44179925)
            //{
            //    MessageBox.Show("Trace_1" 
            //        +"Mpa.ActionType:.." + Mpa.ActionType + Environment.NewLine
            //       + "WActionOrigin:.." + WActionOrigin + Environment.NewLine
            //        ); 
            //}
            if ((FirstActionType != "00" & WActionOrigin == 1)
                || (FirstActionType != "00" & FirstActionType != "08" & WActionOrigin == 2)) // This the meta exception no created OR Moved from reconciliation
            {
                Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", WUniqueRecordId, FirstActionType);
                comboBoxReasonOfAction.Text = Aoc.Maker_ReasonOfAction;


                //labelActionType.Text = "Action Taken";
                labelActionType.ForeColor = Color.Black;

                //buttonAction.Hide();
                buttonUndoAction.Show();

                //string ActionNm = 
                Ag.ReadActionByActionId(WOperator, FirstActionType, 1);

                comboBoxActionNm.Text = Ag.ActionNm;

                comboBoxActionNm.Enabled = false;

                pictureBox2.Show();

                comboBoxReasonOfAction.Show();
                comboBoxReasonOfAction.Enabled = false;

                comboBoxReasonOfAction.Text = Mpa.MatchedType;
                //labelForceMatching.Show();
                //comboBoxReasonOfAction.Show();

                //}
                //else
                //{
                //    labelForceMatching.Hide();
                // comboBoxForceMatching.Hide();
                //}

                // Dissable selection and action
                // Enable Undo Action
                buttonAction.Enabled = false;
                buttonUndoAction.Enabled = true;

            }
            else
            {

                //labelActionType.Text = "Action NOT Taken Yet!";
                labelActionType.ForeColor = Color.Red;
                pictureBox2.Hide();

                comboBoxActionNm.Text = "00_No Action Taken";
                comboBoxActionNm.Enabled = true;

                buttonAction.Enabled = true;
                buttonUndoAction.Enabled = false;

                labelForceMatching.Hide();
                comboBoxReasonOfAction.Hide();
                comboBoxReasonOfAction.Enabled = true;

            }
            //ComingFromMeta = false;
            LeftDt = Mpa.TransDate.Date;

            if (ViewWorkFlow == true)
            {
                buttonAction.Hide();
                buttonUndoAction.Hide();
            }

            // Show Dispute 

            Dt.ReadDisputeTranByUniqueRecordIdIncludingCancel(WUniqueRecordId);
            if (Dt.RecordFound == true)
            {
                string WActionDesc = "No Action Yet";
                if (Dt.DisputeActionId == 1) WActionDesc = "Customer was credited";
                if (Dt.DisputeActionId == 2) WActionDesc = "Customer was credited";
                if (Dt.DisputeActionId == 3) WActionDesc = "Customer was Debited";
                if (Dt.DisputeActionId == 4) WActionDesc = "Customer was Debited";
                if (Dt.DisputeActionId == 5) WActionDesc = "Postponed";
                if (Dt.DisputeActionId == 6) WActionDesc = "Dispute Txn Cancelled";

                if (Dt.ClosedDispute == false)
                {
                    RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
                    Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);
                    if (Ap.RecordFound == true) // OPEN AUTHORISATION
                    {
                        WActionDesc = "No Action Yet But Under Authorisation Process";
                    }
                    //else
                    //{
                    //    WActionDesc = "No Action Yet";
                    //}

                }

                // Read Creator 
                Di.ReadDispute(Dt.DisputeNumber);

                textBoxDisputeDetails.Show();

                if (WActionDesc == "No Action Yet")
                {
                    textBoxDisputeDetails.Text = "Action Type: " + WActionDesc + Environment.NewLine
                    + " ,Creator:" + Di.DisputeCreatorId + Environment.NewLine
                  + " ,Maker :" + Dt.AuthorOriginator;
                }
                else
                {
                    // Action taken or under Authorisation process 
                    textBoxDisputeDetails.Text =
                         "Action Type:.. " + WActionDesc + Environment.NewLine
                    + " ,Action Date:.." + Dt.ActionDtTm.ToShortDateString() + Environment.NewLine
                    + " ,Creator:.." + Di.DisputeCreatorId + Environment.NewLine
                    + " ,Maker :.." + Dt.AuthorOriginator;
                }

                labelDisputeId.Show();
                textBoxDisputeId.Show();
                linkLabelDispute.Show();

                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();

                //buttonUndoDispute.Show();
                buttonUndoDispute.Hide();

                //if (Dt.ClosedDispute == true)
                //{
                //    buttonUndoDispute.Show();
                //    labelDisputeId.Text = "Disp ID" + Environment.NewLine + "Postponed"; 
                //}
                //else
                //{
                //    buttonUndoDispute.Hide();
                //}
                if (Dt.ClosedDispute == false)
                {
                    //buttonUndoDispute.Show();
                    labelDisputeId.Text = "Disp ID" + Environment.NewLine + "Active";
                }
                if (Dt.ClosedDispute == true & Dt.DisputeActionId != 5)
                {
                    //buttonUndoDispute.Show();
                    labelDisputeId.Text = "Disp ID" + Environment.NewLine + "Settled";
                }
            }
            else
            {

                labelDisputeId.Hide();
                textBoxDisputeId.Hide();
                linkLabelDispute.Hide();
                textBoxDisputeDetails.Hide();

                buttonUndoDispute.Hide();
            }
        }

        // Row enter for MetaExceptions created 
        int WExcNo;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WExcNo = (int)rowSelected.Cells[0].Value;

            Er.ReadErrorsIDRecord(WExcNo, WOperator);

            if (Er.ManualAct == true)
            {
                // Manual Action
                comboBoxActionNm.Text = "02_Create Manual Meta Exception";
                //radioButtonCreateManual.Checked = true;
            }
            else
            {
                // Default Action
                if (Mpa.ActionType == "01")
                {
                    comboBoxActionNm.Text = "01_Create Default Meta Exception";
                }
                if (Mpa.ActionType == "11")
                {
                    comboBoxActionNm.Text = "11_DEBIT Customer";
                }

                if (Mpa.ActionType == "21")
                {
                    comboBoxActionNm.Text = "21_CREDIT Customer";
                }
                if (Mpa.ActionType == "26")
                {
                    comboBoxActionNm.Text = "26_CREDIT Branch Excess/AtmCash";
                }

                // Suspense Action
                if (Mpa.ActionType == "09")
                {
                    comboBoxActionNm.Text = "09_Move case to Suspense/Intercompany";
                }

            }
        }

        private void ShowActionsDropDown()
        {
            if (WActionOrigin == 1) // 1 = Matching Actions 
            {
                if (Is_GL_Creation_Auto == true)
                {
                    ComMode = 1;
                }
                else
                {
                    ComMode = 3; // Manual
                }
            }
            if (WActionOrigin == 2 || IsPseudoReplenishment ==true) // 2 = Replenishment Actions
            {
                if (Is_GL_Creation_Auto == true)
                {
                    ComMode = 6;
                }
                else
                {
                    ComMode = 3;// Manual 
                }
            }

            comboBoxActionNm.DataSource = Ag.ReadTableToGet_ActionNm_Array_List(WOperator, ComMode);
            comboBoxActionNm.DisplayMember = "DisplayValue";
        }
        // SHOW EXCESS IF REPLENISHEMENT IS FOUND 
        private void ShowExcessIfReplenishement(string InAtmNo, int InSesNo)
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool HybridRepl = false;

            Ac.ReadAtm(InAtmNo);
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

            string WMaker = WSignedId;
            if (ViewWorkFlow == true & WActionOrigin == 2)
            {
                // Find maker
                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(InAtmNo, InSesNo, "Replenishment");

                WMaker = Ap.Requestor;
            }
            if (ViewWorkFlow == true & WActionOrigin == 1)
            {
                // Find maker
                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationsUserTopOne(WSignedId);

                //Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(InAtmNo, InSesNo, "ReconciliationCat");

                WMaker = Ap.Requestor;
            }
            //WHERE Atmno = '00000550' AND ReplCycle = 2220 And Is_GL_Action = 1
            string WSelectionCriteria = "WHERE AtmNo ='" + InAtmNo
                + "' AND ReplCycle =" + InSesNo
                + " AND ( (Maker ='" + WMaker + "' AND Stage<>'03') OR Stage = '03' ) ";

            if (AudiType == true)
            {
                Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals_AUDI_Type(WSelectionCriteria);
            }
            else
            {

                Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            }


            if (HybridRepl == false)
            {
                textBoxExcess.Text = Aoc.Excess.ToString("#,##0.00");

                if (AudiType == true)
                {
                    textBoxShortage.Text = Aoc.Shortage.ToString("#,##0.00");
                }
                else
                {
                    textBoxShortage.Text = Aoc.CIT_Shortage.ToString("#,##0.00");
                }

                textBoxCurrentExcess.Text = Aoc.Current_ExcessBalance.ToString("#,##0.00");
                textBoxCurrentShortage.Text = Aoc.Current_ShortageBalance.ToString("#,##0.00");
                //textBoxDispShort.Text = Aoc.Dispute_Shortage.ToString("#,##0.00");

                CurrentExcess = Aoc.Current_ExcessBalance;
                CurrentShortage = Aoc.Current_ShortageBalance;

            }
            if (HybridRepl == true)
            {
                textBoxExcess.Text = Aoc.Excess.ToString("#,##0.00");
                textBoxShortage.Text = Aoc.Shortage.ToString("#,##0.00");
                textBoxCurrentExcess.Text = Aoc.Current_ExcessBalance.ToString("#,##0.00");
                textBoxCurrentShortage.Text = Aoc.Current_ShortageBalance.ToString("#,##0.00");
                //textBoxDispShort.Text = Aoc.Dispute_Shortage.ToString("#,##0.00");

                CurrentExcess = Aoc.Current_ExcessBalance;
                CurrentShortage = Aoc.Current_ShortageBalance;

            }


            labelExcessShortage.Show();
            panelExcessShortage.Show();
        }

        // SHOW EXCESS/shorageIF Reconciliation  FOUND 
        private void ShowExcess_Reconciliation(string InAtmNo, int InUniqueRecordId)
        {
          
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool HybridRepl = false;

            Ac.ReadAtm(InAtmNo);
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

            string WMaker = WSignedId;
            if (ViewWorkFlow == true & WActionOrigin == 1)
            {
                // Find maker
                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(InAtmNo, WRMCycleNo, "ReconciliationCat");

                WMaker = Ap.Requestor;
            }
            //if (ViewWorkFlow == true & WActionOrigin == 1)
            //{
            //    // Find maker
            //    RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

            //    Ap.ReadAuthorizationsUserTopOne(WSignedId);

            //    //Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(InAtmNo, InSesNo, "ReconciliationCat");

            //    WMaker = Ap.Requestor;
            //}
            //WHERE Atmno = '00000550' AND ReplCycle = 2220 And Is_GL_Action = 1
            string WSelectionCriteria = "WHERE AtmNo ='" + InAtmNo
                + "' AND UniqueKey =" + InUniqueRecordId
                + " AND ( (Maker ='" + WMaker + "' AND Stage<>'03') OR Stage = '03' ) ";

            if (AudiType == true)
            {
                Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals_AUDI_Type(WSelectionCriteria);
            }
            else
            {

                Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            }


            if (HybridRepl == false)
            {
                textBoxExcess.Text = Aoc.Excess.ToString("#,##0.00");

                if (AudiType == true)
                {
                    textBoxShortage.Text = Aoc.Shortage.ToString("#,##0.00");
                }
                else
                {
                    textBoxShortage.Text = Aoc.Current_DisputeShortage.ToString("#,##0.00");
                }

                textBoxCurrentExcess.Text = Aoc.Current_ExcessBalance.ToString("#,##0.00");
                textBoxCurrentShortage.Text = Aoc.Current_ShortageBalance.ToString("#,##0.00");
                //textBoxDispShort.Text = Aoc.Dispute_Shortage.ToString("#,##0.00");

                CurrentExcess = Aoc.Current_ExcessBalance;
                CurrentShortage = Aoc.Current_DisputeShortage;

            }
            if (HybridRepl == true)
            {
                textBoxExcess.Text = Aoc.Excess.ToString("#,##0.00");
                textBoxShortage.Text = Aoc.Shortage.ToString("#,##0.00");
                textBoxCurrentExcess.Text = Aoc.Current_ExcessBalance.ToString("#,##0.00");
                textBoxCurrentShortage.Text = Aoc.Current_ShortageBalance.ToString("#,##0.00");
                //textBoxDispShort.Text = Aoc.Dispute_Shortage.ToString("#,##0.00");

                CurrentExcess = Aoc.Current_ExcessBalance;
                CurrentShortage = Aoc.Current_ShortageBalance;

            }


            labelExcessShortage.Show();
            panelExcessShortage.Show();
        }
        //
        // Show info for Mask
        //
        public void ShowMask(string InMask)
        {

            //****************************************************************************
            //Translate MASK 
            //****************************************************************************

            WMask = InMask;

            if (WMask == "")
            {
                WMask = "EEE";
            }
            // First Line
            if (Mpa.FileId01 != "")
            {
                labelFileA.Show();
                textBox31.Show();

                labelFileA.Text = "File A : " + Mpa.FileId01;
                labelFileA.Show();
                WSubString = WMask.Substring(0, 1);

                if (WSubString == "1")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox31.BackColor = Color.Red;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = WSubString;
                }
            }
            else
            {
                labelFileA.Hide();
                textBox31.Hide();
            }

            // Second Line 
            if (Mpa.FileId02 != "")
            {
                labelFileB.Show();
                textBox32.Show();

                labelFileB.Text = "File B : " + Mpa.FileId02;
                labelFileB.Show();
                WSubString = WMask.Substring(1, 1);

                if (WSubString == "1")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox32.BackColor = Color.Red;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = WSubString;
                }
            }
            else
            {
                labelFileB.Hide();
                textBox32.Hide();
            }

            // Third Line 
            //
            if (Mpa.FileId03 != "" & WMask.Length > 2)
            {
                labelFileC.Show();
                textBox33.Show();

                labelFileC.Text = "File C : " + Mpa.FileId03;
                labelFileC.Show();

                WSubString = WMask.Substring(2, 1);

                if (WSubString == "1")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox33.BackColor = Color.Red;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = WSubString;
                }
            }
            else
            {
                labelFileC.Hide();
                textBox33.Hide();
            }

            // Forth Line 
            if (Mpa.FileId04 != "")
            {
                labelFileD.Show();
                textBox34.Show();

                labelFileD.Text = "File D : " + Mpa.FileId04;
                labelFileD.Show();
                WSubString = WMask.Substring(3, 1);

                if (WSubString == "1")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox34.BackColor = Color.Red;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = WSubString;
                }
            }
            else
            {
                labelFileD.Hide();
                textBox34.Hide();
            }

            // Fifth Line 
            if (Mpa.FileId05 != "")
            {
                labelFileE.Show();
                textBox35.Show();

                labelFileE.Text = "File E : " + Mpa.FileId05;
                labelFileE.Show();
                WSubString = WMask.Substring(4, 1);


                if (WSubString == "1")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox35.BackColor = Color.Red;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = WSubString;
                }

            }
            else
            {
                labelFileE.Hide();
                textBox35.Hide();
            }
            // sixth Line 
            if (Mpa.FileId06 != "")
            {
                labelFileF.Show();
                textBox36.Show();

                labelFileF.Text = "File F : " + Mpa.FileId06;
                labelFileF.Show();
                WSubString = WMask.Substring(5, 1);

                if (WSubString == "1")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox36.BackColor = Color.Red;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = WSubString;
                }
            }
            else
            {
                labelFileF.Hide();
                textBox36.Hide();
            }
        }

        // Show Grid Left 
        public void ShowGrid()
        {
            //WTotalLeft = 0 ;

            //Keyboard.Focus(dataGridView1);

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            textBoxTotalSelected.Text = Mpa.MatchingMasterDataTableATMs.Rows.Count.ToString();

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // MaskRecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 40; // Status
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; //  Done
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 40; //  Action
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 70; // Terminal
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 50; // Terminal Type, ATM, POS etc 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 90; // Descr
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 40; // Err
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 40; // Mask
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 90; // Account
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[10].Width = 50; // Ccy
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 80; // Amount
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[12].Width = 140; // Date
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[13].Width = 70; // Trace
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[14].Width = 90; // RRNumber
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[15].Width = 50; // Trans Type
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[15].HeaderText = "Trans Type";


        }

        public void ShowGrid2()
        {
            //WTotalLeft = 0 ;

            //Keyboard.Focus(dataGridView1);

            dataGridView2.DataSource = Er.ShortErrorsTable.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            //ShortErrorsTable.Columns.Add("ExcNo", typeof(int));
            //ShortErrorsTable.Columns.Add("Type", typeof(string));
            //ShortErrorsTable.Columns.Add("Sign", typeof(string));
            //ShortErrorsTable.Columns.Add("Amount", typeof(string));
            //ShortErrorsTable.Columns.Add("Desc", typeof(string));
            //ShortErrorsTable.Columns.Add("CustAccNo", typeof(string));
            //ShortErrorsTable.Columns.Add("DateTime", typeof(DateTime));
            //ShortErrorsTable.Columns.Add("TransDescr", typeof(string));

            //MatchingMasterDataTableATMs.Columns.Add("ActionType", typeof(string));

            dataGridView2.Columns[0].Width = 40; // ExcNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 50; // Type
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 50; // Sign
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[3].Width = 80; // Amount
            dataGridView2.Columns[3].DefaultCellStyle = style;
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[9].DefaultCellStyle.ForeColor = Color.Red;
            //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView2.Columns[4].Width = 140; // Desc
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 80; // CustAccNo
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[6].Width = 100; // DateTime
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[7].Width = 60; // 
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Force Matching 

        // NOTES FOR RM CYCLE 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Force Matching for" + " Category: " + WRMCategoryId + " Matching SesNo: " + WRMCycleNo;
            string SearchP4 = "";

            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "Force Matching for" + " Category: " + WRMCategoryId + " Matching SesNo: " + WRMCycleNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }
        // NOTES FOR ITEM 
        private void buttonNotes3_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
        }
        //EXPAND GRID 
        Form78b NForm78b;

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            return;
            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            string WHeader = "UNMATCHED TRANSACTIONS FOR RM CATEGORY : " + WRMCategoryId;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mpa.MatchingMasterDataTableATMs, WHeader, "Form271b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //dataGridView1.Rows[NForm78b.WSelectedRow].Selected = true;
            if (NForm78b.UniqueIsChosen == 1)
            {
                SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                               + "  AND MatchingAtRMCycle = " + WRMCycleNo
                              + " AND SettledRecord = 0 AND  UniqueRecordId = " + NForm78b.WPostedNo.ToString() + " ";

                CallingMode = 2;
                string WSortCriteria = " ORDER BY TerminalId ";

                //No Dates Are selected

                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

                // Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTableFrom271b(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 2, WActionOrigin);
                //Mpa.ReadMatchingTxnsMasterPoolAndFillTable(WOperator, CallingMode, SelectionCriteria, "");

                ShowGrid();

            }
        }

        // Proceed to action 
        Form14b NForm14b;
        Form14b_POS NForm14b_POS;
        int WRowIndex;

        string WActionId;
        // string WAccount;
        bool DisputeOpened;
        // int WMetaExceptionId;
        string UniqueRecordIdOrigin;
        DataTable TEMPTableFromAction;
        decimal TempTxnAmount2;
        decimal WActionTxnAmt;
        //string WActionId; 

        private void buttonAction_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            bool ValidAction = false;

            if (textBoxChangedActionAmt.Text != "")
            {
                // Action amount is different and less than Transaction amount 
                if (decimal.TryParse(textBoxChangedActionAmt.Text, out WActionTxnAmt))
                {

                }
                else
                {
                    MessageBox.Show("Please enter correct amount ");
                    return;
                }
            }
            else
            {
                WActionTxnAmt = Mpa.TransAmount;
            }
            // Validation of action amount 
            if (WActionTxnAmt > Mpa.TransAmount)
            {
                MessageBox.Show("Action Amount Cannot be greater than Transaction Amount ");
                return;
            }

            RRDMActions_GL Ag = new RRDMActions_GL();
            Ag.ReadActionByActionNm(WOperator, comboBoxActionNm.Text);

            WActionId = Ag.ActionId;


            if (comboBoxActionNm.Text == "00_No Action Taken")
            {
                MessageBox.Show("Select Action Please!");
                return;
            }

            if (WActionId == "03")
            {
                MessageBox.Show("Not Allowed Action!");
                return;
            }

            //MessageBox.Show(comboBoxActionNm.Text + Environment.NewLine
            //           + WActionId + Environment.NewLine
            //           );

            if (comboBoxReasonOfAction.Text == "Select Reason")
            {
                MessageBox.Show("Please Select Reason For taking this Action");
                return;
            }
            if (Mpa.TXNDEST == "DEBIT")
            {
                Mpa.TXNDEST = "1";
            }
            if (WActionOrigin == 1) // 1 = Matching Actions 
            {
                if ((WActionId == "91" & Mpa.Origin == "Our Atms" & Mpa.TXNDEST != "1")

                    )
                {
                    // Flex Transaction
                    MessageBox.Show("This Action is related to CoreBanking transactions" + Environment.NewLine
                      + "Use Action '92'for this transaction as this in not a CoreBanking Txn" + Environment.NewLine
                      + "Not allowed to proceed."
                       );
                    return;
                }
                if ((WActionId == "92" & Mpa.Origin == "Our Atms" & Mpa.TXNDEST == "1")

                    )
                {
                    // No Flex
                    // Need of an account 
                    MessageBox.Show("This Action is used only for the non COREBANKING Transactions" + Environment.NewLine
                      + "Use Action '91'for this transaction as this is a COREBANKING txn" + Environment.NewLine
                      + "Not allowed to proceed."
                       );
                    return;
                }

            }

            if (WActionOrigin == 2 || WActionOrigin == 1 || IsPseudoReplenishment == true) // 2 = Replenishment Actions
            {
                if ((WActionId == "95" & Mpa.Origin == "Our Atms" & Mpa.TXNDEST != "1")

                        )
                {
                    // Flex Transaction
                    MessageBox.Show("This Action is related to Corebanking transactions" + Environment.NewLine
                      + "Use Action '96'for this transaction as this in not a Corebanking Txn" + Environment.NewLine
                      + "Not allowed to proceed."
                       );
                    return;
                }
                if ((WActionId == "96" & Mpa.Origin == "Our Atms" & Mpa.TXNDEST == "1")

                    )
                {
                    // No Flex
                    // Need of an account 
                    MessageBox.Show("This Action is used only for the non COREBANKING Transactions" + Environment.NewLine
                      + "Use Action '95'for this transaction as this is a COREBANKING txn" + Environment.NewLine
                      + "Not allowed to proceed."
                       );
                    return;
                }

            }

            // 
            bool Is_POS = false;
            if (Mpa.MatchedType == PRX + "231" || Mpa.MatchedType == PRX + "233")
            {
                Is_POS = true;
            }

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParId = "851";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string UnderDevelopment = Gp.OccuranceNm;

            ParId = "852";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string IsProductionEnv = Gp.OccuranceNm;

            if (IsProductionEnv == "YES" & UnderDevelopment == "YES")
            {
                // It Is YES for BDC
                if (WActionId == "04" || WActionId == "06")
                {
                    ValidAction = true; // For BDC too 
                }
                else
                {
                    return;
                }
            }
            else
            {
                // Testing Environment
                ValidAction = true;
            }

            // Read Action Details
            Ag.ReadActionByActionId_And_Occ(WOperator, WActionId, 1);


            if (Ag.Is_GL_Action == true & WActionId != "04" & WActionId != "05"
                & WActionId != "06" & WActionId != "08" & WActionId != "09" & Is_POS == false & ValidAction == true)
            {
                // Do Validation 
                // ALL WITH ACTION 
                UniqueRecordIdOrigin = "Master_Pool";
                //RRDMActions_GL Ag = new RRDMActions_GL();
                //Ag.ReadActionByActionId_And_Occ(WOperator, WActionId, 1);
                if ((Ag.ShortAccID_1 == "20" || Ag.ShortAccID_2 == "20") & (Mpa.TXNSRC == "1" & Mpa.TXNDEST != "1"))
                {
                    MessageBox.Show("No account exist for the customer!" + Environment.NewLine
                                  + "Possibly you want to affect the category"
                        );
                    return;
                }


                string WOriginWorkFlow = "";
                if (WActionOrigin == 1)
                {
                    WOriginWorkFlow = "Reconciliation";
                }
                if (WActionOrigin == 2 )
                {
                    WOriginWorkFlow = "Replenishment";

                }

                if (WActionTxnAmt > CurrentExcess & ((WOriginWorkFlow == "Replenishment" & Mpa.MetaExceptionId == 55) || IsPseudoReplenishment == true)
                    || (WActionTxnAmt > CurrentExcess & WOriginWorkFlow == "Reconciliation" & (WActionId == "95" || WActionId == "96") )
                    )
                {
                    RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 
                    string WWActionId = ""; 
                    // Read Traces to Process
                    Ta.ReadSessionsStatusTraces(Mpa.TerminalId, Mpa.ReplCycleNo);

                    if (Ta.RecordFound & (Ta.ProcessMode < 1 || Ta.ProcessMode == 0) & WActionOrigin == 1)
                    {
                        WWActionId = "89"; // 89_DEBIT Dispute Shortage/CREDIT Branch Excess

                        MessageBox.Show("Replenishment not done yet" + Environment.NewLine
                            + "Money From Dispute Shortage will be used" + Environment.NewLine
                            + "to facilitate crediting the customer" + Environment.NewLine
                                    + "Action ID:.." + WWActionId
                                  + ""
                                   );

                    }
                    else
                    {
                        //if (WActionOrigin == 2)
                        //{
                            WWActionId = "90"; //90_DEBIT Branch Shortage/CREDIT Branch Excess
                                               // WUniqueRecordIdOrigin = "Replenishment";

                            MessageBox.Show("Money From CIT Shortage will be used" + Environment.NewLine
                                 + "to facilitate crediting the customer" + Environment.NewLine
                                        + "Action ID:.." + WWActionId
                                        + ""
                                         );
                        //}
                       
                    }
                    // Decision 
                    decimal DoubleEntryAmt_1 = WActionTxnAmt; 
                    // int WUniqueRecordId = Mpa.ReplCycleNo; // SesNo 
                    string WCcy = "EGP";
                    decimal DoubleEntryAmt = DoubleEntryAmt_1 - CurrentExcess;
                    // string WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                    string WUniqueRecordIdOrigin = "Master_Pool";
                    string WMaker_ReasonOfAction = "Had To Move Money to Excess";

                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WWActionId, WUniqueRecordIdOrigin,
                                                          Mpa.UniqueRecordId, WCcy, DoubleEntryAmt, Mpa.TerminalId, Mpa.ReplCycleNo
                                                          , WMaker_ReasonOfAction, WOriginWorkFlow);

                    TEMPTableFromAction = Aoc.TxnsTableFromAction;

                    //string WActionId90 = "90"; //90_DEBIT Branch Shortage/CREDIT Branch Excess
                    //                           // WUniqueRecordIdOrigin = "Replenishment";

                    //MessageBox.Show("Money From CIT Shortage will be used" + Environment.NewLine
                    //            + "Action ID:.." + WActionId90
                    //            + ""
                    //             );

                    //int WUniqueRecordId = Mpa.UniqueRecordId; // SesNo 
                    //string WCcy = "EGP";
                    //decimal DoubleEntryAmt = WActionTxnAmt - CurrentExcess;
                    //// string WMaker_ReasonOfAction = "UnLoad From ATM-Excess";

                    //string WUniqueRecordIdOrigin = "Master_Pool";
                    //string WMaker_ReasonOfAction = "Had To Move Money to Excess";
                    //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                    //                                      WActionId90, WUniqueRecordIdOrigin,
                    //                                      WUniqueRecordId, WCcy, DoubleEntryAmt, Mpa.TerminalId, Mpa.ReplCycleNo
                    //                                      , WMaker_ReasonOfAction, WOriginWorkFlow);

                    //TEMPTableFromAction = Aoc.TxnsTableFromAction;

                }

                
                NForm14b = new Form14b(WSignedId, WOperator,
                                     UniqueRecordIdOrigin, Mpa.UniqueRecordId,
                                           WActionId, comboBoxReasonOfAction.Text, WActionTxnAmt, WOriginWorkFlow, WSesNo);
                NForm14b.FormClosed += NForm14b_FormClosed;
                NForm14b.ShowDialog();

                WRowIndex = dataGridView1.SelectedRows[0].Index;
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            }

            // Dispute 
            if (WActionId == "05" & ValidAction == true //05_Move to dispute"
                                || WActionId == "12" // Whenever this is selected a dispute is oppened too
                                                     // Already a transaction was created through the previous process
                                || WActionId == "22" // Whenever this is selected a dispute is oppened too
                                )
            {
                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WUniqueRecordIdOrigin = "Master_Pool";
                string WMaker_ReasonOfAction = comboBoxReasonOfAction.Text;

                string WOriginWorkFlow = "";

                //if (WActionOrigin == 1)
                //{
                //    WOriginWorkFlow = "Reconciliation";
                //}
                //if (WActionOrigin == 2)
                //{
                //    WOriginWorkFlow = "Replenishment";
                //}


                //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                //                                    WActionId, WUniqueRecordIdOrigin,
                //                                    WUniqueRecordId, Mpa.TransCurr, WActionTxnAmt,
                //                                    Mpa.TerminalId, Mpa.ReplCycleNo, WMaker_ReasonOfAction, WOriginWorkFlow);



                if (WActionId == "12" || WActionId == "22")
                {
                    MessageBox.Show("The funds will be moved to Intermediate account" + Environment.NewLine
                                + "Also an internal dispute connected to the maker name will be oppened" + Environment.NewLine
                                );
                }

                SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

                Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);
                if (Dt.RecordFound == true)
                {
                    MessageBox.Show(" Dispute already open for this Error");
                    return;
                }
                Form5 NForm5;
                int WFrom = 0; // 
                if (WActionOrigin == 1)
                {
                    WFrom = 7;  // "Reconciliation";
                }
                if (WActionOrigin == 2)
                {
                    WFrom = 5; // "Replenishment";
                }

                NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Mpa.CardNumber, WUniqueRecordId, WActionTxnAmt, 0, "From Recon or Repl", WFrom, "ATM");
                NForm5.FormClosed += NForm5_FormClosed;
                NForm5.ShowDialog();

                if (DisputeOpened == false)
                {
                    MessageBox.Show("Dispute was not opened! ");
                    return;
                }

                Mpa.ActionByUser = true;
                Mpa.UserId = WSignedId;

                Mpa.MatchedType = comboBoxReasonOfAction.Text;

                Mpa.ActionType = WActionId;

                //Mpa.Comments = Mpa.Comments + " ," + comboBoxReasonOfAction.Text;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);

                // Update RM Cycle
                // For having these for Form271a
                //Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

                //Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow + 1;

                //Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);

                MessageBox.Show("UnMatched Record with UniqueRecordId : " + WUniqueRecordId.ToString() + "...Has been moved to dispute");

                WRowIndex = dataGridView1.SelectedRows[0].Index;
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

                return; // leave it here 

            }

            //
            // DEAL WITH POS 
            //
            if ((WActionId == "71"     // 71_DEBIT Branch Shortages/CR_Category_Acc
                || WActionId == "81")  // 81_CREDIT  Branch Excess/DR_Category_Acc
                & Is_POS == true
                )
            {
                if (Mpa.ResponseCode == "200000" & Mpa.AccNumber == "")
                {
                    MessageBox.Show("No partner record was assigned on the Credit");
                    return;
                }
                if (Mpa.ResponseCode == "0" & Mpa.AccNumber == "")
                {
                    MessageBox.Show("No partner record was assigned on the Credit");
                    return;
                }

                SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);


                NForm14b_POS = new Form14b_POS(WSignedId, WOperator,
                                     UniqueRecordIdOrigin, Mpa.UniqueRecordId,
                                           WActionId, comboBoxReasonOfAction.Text);
                NForm14b_POS.FormClosed += NForm14b_POS_FormClosed;
                NForm14b_POS.ShowDialog();

                WRowIndex = dataGridView1.SelectedRows[0].Index;
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
            }

            // Move txns to pool to re-matched

            //if (comboBoxActionNm.Text == "06_Move Txns to Pool")
            //{
            //    // Move txns to pool to re-matched

            //    Order = "Descending";
            //    WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            //    WSearchP4 = "";
            //    Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            //    if (Cn.RecordFound == true)
            //    {
            //        //labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            //        // OK there is a note 
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please enter a note to explain reason of reactivating the case");
            //        return;
            //    }

            //    SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;

            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            //    Mpa.ActionByUser = true;
            //    Mpa.UserId = WSignedId;

            //    Mpa.MatchedType = "All related txns will participate in next matching";

            //    Mpa.ActionType = "06";

            //    Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);


            //    MessageBox.Show("UnMatched Record with UniqueRecordId : " + WUniqueRecordId.ToString() + Environment.NewLine
            //        + "Has been moved to Pool");

            //    WRowIndex = dataGridView1.SelectedRows[0].Index;
            //    int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            //    SetScreen();

            //    dataGridView1.Rows[WRowIndex].Selected = true;
            //    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            //    dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            //}
            // Force matched and other Manual 

            if (Ag.Is_GL_Action == false & (WActionId == "04"
               || WActionId == "06"
               || WActionId == "09"
               || WActionId == "23"
               || WActionId == "13"
               || WActionId == "73"
               || WActionId == "83")
               )
            {
                // This is force Matching and other manual 
                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WUniqueRecordIdOrigin = "Master_Pool";
                string WMaker_ReasonOfAction = comboBoxReasonOfAction.Text;


                string WOriginWorkFlow = "";

                if (WActionOrigin == 1)
                {
                    WOriginWorkFlow = "Reconciliation";
                }
                if (WActionOrigin == 2)
                {
                    WOriginWorkFlow = "Replenishment";
                }


                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                    WActionId, WUniqueRecordIdOrigin,
                                                    WUniqueRecordId, Mpa.TransCurr, WActionTxnAmt,
                                                    Mpa.TerminalId, Mpa.ReplCycleNo, WMaker_ReasonOfAction, WOriginWorkFlow);

                SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                Mpa.ActionByUser = true;
                Mpa.UserId = WSignedId;

                Mpa.MatchedType = comboBoxReasonOfAction.Text;

                Mpa.ActionType = WActionId;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);

                MessageBox.Show("Record with UniqueRecordId : " + WUniqueRecordId.ToString() + Environment.NewLine
                    + "Has been dealt by Maker");

                WRowIndex = dataGridView1.SelectedRows[0].Index;
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            }

            // 08_Move case to Replenishment Reconciliation 
            if (comboBoxActionNm.Text == "08_Move case to Replenishment Reconciliation")
            {

                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WUniqueRecordIdOrigin = "Master_Pool";
                string WMaker_ReasonOfAction = comboBoxReasonOfAction.Text;
                string WOriginWorkFlow = "";

                if (WActionOrigin == 1)
                {
                    WOriginWorkFlow = "Reconciliation";
                }
                if (WActionOrigin == 2)
                {
                    WOriginWorkFlow = "Replenishment";
                }
                Aoc.CreateAndInsertInActionOccurances(WOperator, WSignedId,
                                           WUniqueRecordIdOrigin, WUniqueRecordId, WActionId, 1,
                                           WMaker_ReasonOfAction, WOriginWorkFlow);


                SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
                if (Mpa.RecordFound == true)
                {
                    Mpa.ActionByUser = true;
                    Mpa.UserId = WSignedId;

                    Mpa.MatchedType = comboBoxReasonOfAction.Text;

                    Mpa.ActionType = "08";

                    if (Mpa.Comments != "")
                        Mpa.Comments = Mpa.Comments + " ," + comboBoxReasonOfAction.Text;
                    else Mpa.Comments = comboBoxReasonOfAction.Text;

                    Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);

                    MessageBox.Show("UnMatched Record with UniqueRecordId : " + WUniqueRecordId.ToString() + Environment.NewLine
                    + "Has been moved to Replenishment workflow");


                }
                else
                {
                    MessageBox.Show("Error");
                }


                WRowIndex = dataGridView1.SelectedRows[0].Index;
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            }

        }

        // Form5 Close
        void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {

            Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);
            if (Dt.RecordFound == true)
            {
                DisputeOpened = true;
                labelDisputeId.Show();
                textBoxDisputeId.Show();

                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
            }
            else
            {

                DisputeOpened = false;

            }
        }

        void NForm14b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //ComingFromMeta = true;
            // UPDATE TRANSACTION 
            if (NForm14b.Confirmed == true)
            {
                SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

                Mpa.MetaExceptionNo = 0;
                Mpa.ActionByUser = true;
                Mpa.UserId = WSignedId;

                Mpa.ActionType = WActionId; // 

                Mpa.MatchedType = comboBoxReasonOfAction.Text;

                //if (Mpa.Comments != "")
                //Mpa.Comments = Mpa.Comments + " ," + comboBoxReasonOfAction.Text;
                //else Mpa.Comments = comboBoxReasonOfAction.Text;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);

                // Update RM Cycle
                // For having these for Form271a
                //Rcs.ReadReconcCategorySessionByRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

                //Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow + 1;

                //Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);

            }
            else
            {
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(UniqueRecordIdOrigin, Mpa.UniqueRecordId,
                                                                                                WActionId);
            }


        }

        void NForm14b_POS_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (NForm14b_POS.Confirmed == true)
            {
                SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                Mpa.MetaExceptionNo = 0;
                Mpa.ActionByUser = true;
                Mpa.UserId = WSignedId;

                Mpa.ActionType = WActionId; // 

                //if (Mpa.Comments != "")
                //Mpa.Comments = Mpa.Comments + " ," + comboBoxReasonOfAction.Text;
                //else Mpa.Comments = comboBoxReasonOfAction.Text;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);

                // Update RM Cycle
                // For having these for Form271a
                //Rcs.ReadReconcCategorySessionByRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

                //Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow + 1;

                //Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);

            }
            else
            {
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(UniqueRecordIdOrigin, Mpa.UniqueRecordId,
                                                                                                WActionId);
            }

        }


        // UNDO ACTION 
        private void buttonUndoAction_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            // Read Dispute if any 
            Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);

            if (Dt.RecordFound == true & Mpa.ActionType == "05" & (Dt.ClosedDispute == true & Dt.DisputeActionId != 5))
            {

                MessageBox.Show("You cannot undo dispute" + Environment.NewLine
                          + "Dispute is already Closed/Settled"
                           );

                return;
            }

            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", WUniqueRecordId, "89");
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", WUniqueRecordId, "90");

            // ReadActionsOccurancesByUniqueKey(InUniqueKeyOrigin, InUniqueKey, InActionId);
            Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", WUniqueRecordId, WActionId);
            //Aoc.RecordFound = true; 
            if (Aoc.RecordFound == true)
            {
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", WUniqueRecordId, WActionId);
                // Leave it here
                if (Mpa.ActionType == "08" || Mpa.ActionType == "05")
                {

                    Mpa.SettledRecord = false; // In the case that was 08 which was moved from Reconciliation to Replenishment
                                               // In the case that was 05 which was moved from Dispute by postponed 
                }
                Mpa.MetaExceptionNo = 0;
                Mpa.ActionByUser = false;
                Mpa.UserId = "";
                Mpa.ActionType = "00";
                Mpa.MatchedType = "";
                //Mpa.Comments = "";

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);

                checkBoxChangeActionAmt.Checked = false;

                // Update RM Cycle
                // For having these for Form271a
                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

                Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow - 1;

                Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);
            }

            // Dispute 

            if (WActionId == "05" || WActionId == "12" || WActionId == "22") // This for Dispute 
            {
                // DELETE SEPARATELY as the Stage was set up to 05
                // We use this method because this deletes for the stage = 03
                // 03 was created at the time of dispute creation
                // Aoc.DeleteActionsOccurancesUniqueKeyAndActionID_From_CIT_Mgmnt("Master_Pool", WUniqueRecordId, WActionId);
                //comboBoxForceMatching.Text = "Select Reason";
                Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);
                if (Dt.RecordFound == true & Dt.ClosedDispute == false
                    )
                {
                    //Aoc.DeleteActionsOccurancesUniqueKeyAndActionID_Dispute_Postponed("Master_Pool", WUniqueRecordId, "05");
                    //Aoc.DeleteActionsOccurancesUniqueKeyAndActionID_Dispute_Postponed("Master_Pool", WUniqueRecordId, "10");
                    Dt.DisputeActionId = 6;

                    Dt.AuthorOriginator = "Cancel by Originator";
                    Dt.Authoriser = "Cancel by Originator";
                    Dt.ActionDtTm = DateTime.Now;
                    Dt.ClosedDispute = true;
                    Dt.PendingAuthorization = false;
                    // UPDATE Disputes Transaction record
                    Dt.UpdateDisputeTranRecord(Dt.DispTranNo);

                    // READ AGAIN
                    Dt.ReadAllTranForDispute(Dt.DisputeNumber);

                    if (Dt.OpenDispTran == 0)
                    {
                        Di.ReadDispute(Dt.DisputeNumber);

                        Di.CloseDate = DateTime.Now;

                        Di.Active = false;

                        Di.UpdateDisputeRecord(Dt.DisputeNumber);

                        if (Di.ErrorFound)
                        {
                            MessageBox.Show("ERROR", Dt.ErrorOutput);
                            //    ErrorReturn = true;
                            return;
                        }

                    }

                    // Di.DeleteDisputeRecord(Dt.DisputeNumber);

                    textBoxDisputeId.Hide();
                    labelDisputeId.Hide();

                    // Delete Authorisation record if any 
                    Au.DeleteAuthorisationRecord_Disp(Dt.DisputeNumber, Dt.DispTranNo);

                }

            }


            comboBoxActionNm.Enabled = true;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            SetScreen();

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }


        // Show  Journal 
        private void buttonJournal_Click(object sender, EventArgs e)
        {
            Form67 NForm67;
            RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();

            String JournalId = "";
            int WTraceNo = Mpa.AtmTraceNo;
            int Mode; // FULL

            //if (WOperator == "BCAIEGCX")
            //{
            int WSeqNoA = 0;
            int WSeqNoB = 0;

            Jt.ReadJournalTxnsByParameters(WOperator, Mpa.TerminalId, Mpa.AtmTraceNo*10, Mpa.TransAmount, Mpa.CardNumber, Mpa.TransDate.Date);

            if (Jt.RecordFound)
            {
                WSeqNoA = Jt.SeqNo;
                WSeqNoB = Jt.SeqNo;
                Mpa.FuID = Jt.FuId;
            }
            else
            {
                MessageBox.Show("There is no recognisable valid record in Journal" + Environment.NewLine
                              + "Search in Journals to find the occurance!"
                            );

                Form200cATMs NForm200cATMs;

                NForm200cATMs = new Form200cATMs(WSignedId, WSignRecordNo, "5", WOperator, Mpa.TerminalId);
                NForm200cATMs.ShowDialog();

                return;
            }
            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            Mode = 3; // Specific Journal 
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber
                              , Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();
            return;
          

        }

        // TRANSACTION TRAIL FOR THE LEFT
        private void button1_Click(object sender, EventArgs e)
        {

            Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

            if (IsAmtDifferent == true)
            {
                // Case where amounts are different in POS
                MessageBox.Show("Please note that Amounts of transactions are different!");
            }

            NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, Mpa.UniqueRecordId, 1);

            NForm78d_AllFiles_BDC_3.ShowDialog();

        }
        // If FORCE MATCHING
        //private void radioButtonForceMatching_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (radioButtonForceMatching.Checked == true)
        //    {
        //        labelForceMatching.Show();
        //        comboBoxForceMatching.Show();
        //    }
        //    else
        //    {
        //        labelForceMatching.Hide();
        //        comboBoxForceMatching.Hide();
        //    }
        //}



        // Print Actions 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Matching is done but not Settled 
            SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                      + "  AND MatchingAtRMCycle =" + WRMCycleNo
                      + " AND IsMatchingDone = 1 AND Matched = 0  "
                      //+ " AND IsMatchingDone = 1 AND Matched = 0 AND SettledRecord = 0 "
                      + " AND ActionType != '07' ";

            string WSortCriteria = "Order By TerminalId, SeqNo ";

            Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria, 1);


            DataTable Mpa_TableActions;
            Mpa_TableActions = new DataTable();
            Mpa_TableActions.Clear();

            // DATA TABLE ROWS DEFINITION 
            Mpa_TableActions.Columns.Add("RecordId", typeof(int));
            Mpa_TableActions.Columns.Add("CardNo", typeof(string));
            Mpa_TableActions.Columns.Add("AccountNo", typeof(string));
            Mpa_TableActions.Columns.Add("Amount", typeof(string));
            Mpa_TableActions.Columns.Add("TransDate", typeof(string));
            Mpa_TableActions.Columns.Add("ActionNm", typeof(string));
            Mpa_TableActions.Columns.Add("Mask", typeof(string));
            Mpa_TableActions.Columns.Add("Maker", typeof(string));
            Mpa_TableActions.Columns.Add("Authoriser", typeof(string));

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            Aoc.ClearTableTxnsTableFromAction();

            int I = 0;

            while (I <= (Mpa.MatchingMasterDataTableATMs.Rows.Count - 1))
            {
                //    RecordFound = true;
                int WUniqueKey = (int)Mpa.MatchingMasterDataTableATMs.Rows[I]["RecordId"];

                SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueKey;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

                DataRow RowSelected = Mpa_TableActions.NewRow();

                RowSelected["RecordId"] = Mpa.UniqueRecordId;

                RowSelected["CardNo"] = Mpa.CardNumber;
                RowSelected["AccountNo"] = Mpa.AccNumber;

                RowSelected["Amount"] = Mpa.TransAmount.ToString("#,##0.00");

                RowSelected["TransDate"] = Mpa.TransDate;

                //string ActionNm = 
                Ag.ReadActionByActionId(WOperator, Mpa.ActionType, 1);

                RowSelected["ActionNm"] = Ag.ActionNm;

                RowSelected["Mask"] = Mpa.MatchMask;

                RowSelected["Maker"] = Mpa.UserId;
                RowSelected["Authoriser"] = Mpa.Authoriser;
                // ADD ROW
                Mpa_TableActions.Rows.Add(RowSelected);
                int WMode2 = 1; // DO NOT Create transaction in pool 
                string WCallerProcess = "Reconciliation";
                Aoc.ReadActionsTxnsCreateTableByUniqueKey("Master_Pool", WUniqueKey,
                                                             Mpa.ActionType, 1, WCallerProcess, WMode2);
                I = I + 1;
            }

            DataTable TempTxnsTableFromAction;
            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;

            string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Mpa_TableActions, 2);
            NForm14b_All_Actions.ShowDialog();
            //*******************************************
            //*******************************************
            //*******************************************
            //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            //Aoc.ClearTableTxnsTableFromAction();

            // Aoc.ReadActionsTxnsCreateTableByUniqueKey(WUniqueRecordIdOrigin, Dt.UniqueRecordId, "All");

            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;
            //Form14b_All_Actions NForm14b_All_Actions;
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();



            return;

            string P1 = "Transactions For Reconciliation :" + WRMCategoryId
                         + " AND Cycle : " + WRMCycleNo.ToString();

            string P2 = "";
            string P3 = "";
            if (ViewWorkFlow == true)
            {

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WRMCategoryId, WRMCycleNo, "ReconciliationCat");

                if (Ap.RecordFound == true)
                {
                    Us.ReadUsersRecord(Ap.Requestor);
                    P2 = Us.UserName;
                    Us.ReadUsersRecord(Ap.Authoriser);
                    P3 = Us.UserName;
                }
                else
                {
                    //ReconciliationAuthorNoRecordYet = true;
                }

            }
            else
            {
                Us.ReadUsersRecord(WSignedId);
                P2 = Us.UserName;
                P3 = "N/A";
            }

            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R55ATMS ReportATMS55 = new Form56R55ATMS(P1, P2, P3, P4, P5);
            ReportATMS55.Show();
        }
        // Video clip
        private void buttonVideoClip_Click(object sender, EventArgs e)
        {
            // Based on Transaction No show video clip 
            //TEST
            VideoWindow NvideoForm = new VideoWindow();
            MessageBox.Show("Video will be shown _1");
            NvideoForm.ShowDialog();
        }
        // POS INFORMATION 
        bool WRecordFound;
        private void buttonPOS_Click(object sender, EventArgs e)
        {
            //if (Mpa.Origin == "Our Atms")
            //{
            //    MessageBox.Show("Not Allowed Operation");
            //    return;
            //}
            //RRDMMatchingTxns_InGeneralTables_BDC Mg = new RRDMMatchingTxns_InGeneralTables_BDC();

            //string WSelectionCriteria = " WHERE SeqNo=" + Mpa.OriginalRecordId;
            //string FileId = "[RRDM_Reconciliation_ITMX].[dbo]." + Mpa.FileId01;
            //Mg.ReadTransSpecificFromSpecificTable_Primary(WSelectionCriteria, FileId, 1);
            //if (Mg.RecordFound == true)
            //{
            //    string Message = Mg.FullTraceNo;
            //    MessageBox.Show(Message);
            //}
            //else
            //{
            //    string Message = "Nothing Available to show!";
            //    MessageBox.Show(Message);
            //}

            //return;

            ////
            ////
            //// 
            //RRDMMatchingOfTxnsFindOriginRAW Msr = new RRDMMatchingOfTxnsFindOriginRAW();

            //WRecordFound =
            //    Msr.FindRawRecordFromMasterRecord
            //      (WOperator, Mpa.FileId01, Mpa.MatchingAtRMCycle,
            //      Mpa.TransDate.Date,
            //       Mpa.TerminalId, Mpa.TraceNoWithNoEndZero, Mpa.RRNumber, 2);
            ////(WOperator, Mpa.FileId01, Mpa.MatchingAtRMCycle, Mpa.TerminalId, Mpa.RRNumber, 2);

            //string WHeader = "MERCHANT DETAILS FOR Transaction with RRN : " + Mpa.RRNumber.ToString();
            //string WCardNumber = Mpa.CardNumber;
            //string WAccNo = Mpa.AccNumber;
            //string WDateTime = Mpa.TransDate.ToString();
            //string WAmount = Mpa.TransAmount.ToString("#,##0.00");
            //string WCcy = Mpa.TransCurr;
            //string WAutorisationCd = Msr.AuthorisationId;
            //string WMerchantId = Msr.MerchantId;
            //string WMerchantName = Msr.MerchantNm;
            //string WTranDescription = Msr.TranDescription;

            //Form2Merchant NForm2Merchant;

            //NForm2Merchant = new Form2Merchant(WOperator, WHeader, WCardNumber, WAccNo,
            //                                 WDateTime, WAmount, WCcy, WAutorisationCd,
            //                                        WMerchantId, WMerchantName, WTranDescription);
            //NForm2Merchant.Show();
        }
        // Journal Lines
        private void buttonJournalLines_Click(object sender, EventArgs e)
        {
            // Show Lines of journal 
            string SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            //
            // Check those where Mask Has 0 as first Character 
            //
            string FirstChar = Mpa.MatchMask.Substring(0, 1);

            if (FirstChar == "0")
            {
                MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
                                 + "Select Journal Lines Near To this"
                                 );
                return;
            }

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            if (Mpa.TraceNoWithNoEndZero == 0)
            {
                MessageBox.Show("No Available Trace to show the Journal Lines for this Txn/Category ");
                return;
            }
            else
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;
                WSeqNoB = Mpa.OriginalRecordId;
            }

            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();

        }
        // Near Journal 
        int IntTrace;

        private void buttonJournal_Near_Click(object sender, EventArgs e)
        {
            // Here we search the journal lines close to the selected transaction
            // If selected already in Journal then we show the one before and the one after
            // If transaction not in journal say 011 then we show 5 minutes before and 5 minutes after. 
            // This is because the time at mainframe maybe different that this of ATM. 

            WSubString = WMask.Substring(0, 1);

            DateTime SavedTransDate = Mpa.TransDate;

            if (WSubString == "1")
            {
                IntTrace = Mpa.TraceNoWithNoEndZero;
            }

            bool In_HST = false;

            //if (Mpa.TransDate.Date <= HST_DATE)
            //{
            //    In_HST = true;
            //}
            // Show Lines of journal 
            //string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;
            //if (In_HST == true)
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            //}
            //else
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            //}

            string SelectionCriteria;
            int SaveSeqNo;

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            //DateTime TestingDate = new DateTime(2019, 01, 03);

            SaveSeqNo = Mpa.SeqNo;

            DateTime WDtA = Mpa.TransDate;
            DateTime WDtB = Mpa.TransDate;
            DateTime WDt;

            WDt = WDtA.AddMinutes(-2);

            // FIND THE LESS
            SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                              + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate < @TransDate  ";
            string OrderBy = "  ORDER By TransDate Desc";

            if (In_HST == true)
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
            }
            else
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
            }

            if (Mpa.RecordFound == true)
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;

                // FIND THE GREATEST that exist 
                WDt = WDtB.AddMinutes(2);
                SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                           + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate > @TransDate ";

                OrderBy = "  ORDER By TransDate ASC ";


                if (In_HST == true)
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
                }
                else
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
                }

                if (Mpa.RecordFound == true)
                {
                    // 
                    WSeqNoB = Mpa.OriginalRecordId; // This is the SeqNo in Pambos Journal  

                }
                else
                {
                    MessageBox.Show("No Upper Limit. No Journal Lines to Show");

                    WSeqNoB = 0;
                    // Reestablish Mpa Data
                    //
                    //SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                    //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                    //return;
                }


            }
            else
            {
                MessageBox.Show("No Lower Limit. No Journal Lines to Show");
                // Reestablish Mpa Data
                //
                SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                if (In_HST == true)
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                }
                else
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
                }

                // Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                return;
            }
            //
            // Reestablish Mpa Data
            //
            SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

            if (In_HST == true)
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            }
            else
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            }

            //   Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific range
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (WSubString == "1")
            {
                WTraceRRNumber = IntTrace.ToString();
            }

            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                      SavedTransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();

        }


        protected override CreateParams CreateParams
        {

            get
            {

                CreateParams handleParam = base.CreateParams;

                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED        

                return handleParam;

            }

        }
        // Reversals Status
        private void linkLabelStatusOfReversals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMMask_Reversals Mr = new RRDMMask_Reversals();

            Mr.ReadAndFind_Reversals(WOperator, WSignedId, WUniqueRecordId);

            if (Mr.Reversals_In_One || Mr.Reversals_In_Two || Mr.Reversals_In_Three)
            {

                string Message = " There are reversals ";
                if (Mr.Reversals_In_One == true)
                {
                    Message = Message + "..IN pos 1.. ";
                }
                if (Mr.Reversals_In_Two == true)
                {
                    Message = Message + "..IN pos 2..";
                }
                if (Mr.Reversals_In_Three == true)
                {
                    Message = Message + "..IN pos 3..";
                }

                MessageBox.Show(Message);

            }
            else
            {
                MessageBox.Show("No reversals found.");
            }
        }
        // Relations - show transaction taking part in other category
        private void linkLabelRelations_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // UniqueIdType = 13
            // WFunction = "Investigation"
            // WIncludeNotMatchedYet = true
            int UniqueIdType = 13;
            string WFunction = "Investigation";
            string WIncludeNotMatchedYet = "1";
            Form80b NForm80b;
            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator,
             Mpa.TransDate.AddDays(-1), Mpa.TransDate.AddDays(1),
             textBoxAtmNo.Text, "", 0, Mpa.RRNumber, 0, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);
            NForm80b.ShowDialog();
        }
        // Assign Partner  
        Form78d_POS_Reconc NForm78d_POS_Reconc;

        private void buttonAssignPartner_Click(object sender, EventArgs e)
        {

            int WModeOfPartner = 0;
            string WMatchingCateg = "";
            string WAtmNo = "";

            if (Mpa.TransType == 21)
            {
                WModeOfPartner = 1;
            }
            if (Mpa.TransType == 11)
            {
                WModeOfPartner = 2;
            }
            NForm78d_POS_Reconc = new Form78d_POS_Reconc(WOperator, WSignedId, Mpa.SeqNo, Mgt.TablePOS_Settlement, WModeOfPartner);
            NForm78d_POS_Reconc.FormClosed += NForm78d_POS_Reconc_FormClosed;
            NForm78d_POS_Reconc.ShowDialog();
        }

        void NForm78d_POS_Reconc_FormClosed(object sender, FormClosedEventArgs e)
        {
            // UPDATE TRANSACTION 
            if (NForm78d_POS_Reconc.WPartnerAccNo != "")
            {
                // Partner Assigned 
                textBoxUnMatchedType.Text = textBoxUnMatchedType.Text + Environment.NewLine
                                           + "Partner Assigned with Accno:" + NForm78d_POS_Reconc.WPartnerAccNo
                                           ;

            }
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            SetScreen();

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        private void comboBoxActionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Leave it as it is 


            if (comboBoxActionNm.Text != "00_No Action Taken")
            {
                if ((Mpa.SettledRecord == true & Mpa.ActionType == "05") & ViewWorkFlow == false)
                {

                    labelForceMatching.Hide(); // Settled during Dispute Process no reason to show 
                    comboBoxReasonOfAction.Hide();
                }
                else
                {
                    labelForceMatching.Show();
                    comboBoxReasonOfAction.Show();
                }

            }
            else
            {
                // Leave it as it is
                labelForceMatching.Hide();
                comboBoxReasonOfAction.Hide();
            }

        }
        // All Actions 
        //
        private void buttonAllActions_Click(object sender, EventArgs e)
        {

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WSelectionCriteria = "";
            int WMode;

            if (WActionOrigin == 1)
            {
                WSelectionCriteria = "WHERE RMCateg='" + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo + " AND (OriginWorkFlow ='Reconciliation' OR (OriginWorkFlow ='Dispute' AND Stage = '03')) "; ;
               // WSelectionCriteria = "WHERE RMCateg='" + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo + " AND (OriginWorkFlow ='Reconciliation') "; 
            }
            if (WActionOrigin == 2)
            {
                WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo
                   + " AND (OriginWorkFlow ='Replenishment' OR OriginWorkFlow ='Dispute' OR OriginWorkFlow ='Reconciliation') ";
            }

            if (Is_GL_Creation_Auto == true)
            {
                WMode = 3;
                Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);
            }
            else
            {
                // Manual operation 
                WMode = 4;
                Aoc.ReadActionsOccurancesAndFillTable_Small_Manual(WSelectionCriteria);
            }

            //string WUniqueRecordIdOrigin = "Master_Pool";
            // PROVIDE TABLE to FORM
            Form14b_All_Actions NForm14b_All_Actions;

            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();

            //ReadActionsOccurancesAndFillTable_Small_Manual(string InSelectionCriteria)

        }
        //
        // All Accounting 
        //
        private void buttonAllAccounting_Click(object sender, EventArgs e)
        {

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "";

            if (WActionOrigin == 1)
            {

                WSelectionCriteria = "WHERE RMCateg='" + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo
                    + " AND OriginWorkFlow ='Reconciliation'";
                // SelectionCriteria = "WHERE RMCateg='" + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo;
                Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

                Aoc.ClearTableTxnsTableFromAction();

                int I = 0;

                while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                    Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                    if (Aoc.Is_GL_Action == true)
                    {

                        int WMode2 = 1; // DO NOT Create transaction in pool 
                        string WCallerProcess = Aoc.OriginWorkFlow;
                        Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey,
                                                                     Aoc.ActionId, Aoc.Occurance, WCallerProcess, WMode2);
                    }

                    I = I + 1;
                }

                DataTable TempTxnsTableFromAction;


                string WUniqueRecordIdOrigin = "Master_Pool";

                Form14b_All_Actions NForm14b_All_Actions;

                // Aoc.ReadActionsTxnsCreateTableByUniqueKey(WUniqueRecordIdOrigin, Dt.UniqueRecordId, "All");

                TempTxnsTableFromAction = Aoc.TxnsTableFromAction;
                //Form14b_All_Actions NForm14b_All_Actions;
                NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
                NForm14b_All_Actions.ShowDialog();

            }

            if (WActionOrigin == 2)
            {
                // string WSelectionCriteria = " WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo;
                WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo
                    + " AND (OriginWorkFlow ='Replenishment' OR OriginWorkFlow ='Dispute') ";
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
        // Link to Dispute
        private void linkLabelDispute_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form3 NForm3;
            //
            NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator, textBoxDisputeId.Text, NullPastDate, NullPastDate, 13);
            // NForm3.FormClosed += NForm3_FormClosed;
            NForm3.ShowDialog();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
        // Undo dispute 
        private void buttonUndoDispute_Click(object sender, EventArgs e)
        {
            // Post
            Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);
            if (Dt.RecordFound == true & (Dt.ClosedDispute == true & Dt.DisputeActionId == 5))
            {

                MessageBox.Show("The Dispute is at status postponed but now will be opened." + Environment.NewLine
                          + "Finish replenishment and take action on dispute"
                           );

                Dt.ClosedDispute = false;
                Dt.UpdateDisputeTranRecord(WUniqueRecordId);

                RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
                Di.ReadDispute(Dt.DisputeNumber);
                Di.Active = true;
                Di.UpdateDisputeRecord(Dt.DisputeNumber);

                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
            }
        }
        // Change Action Amount
        private void checkBoxChangeActionAmt_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxChangeActionAmt.Checked == true)
            {
                textBoxChangedActionAmt.Show();

                textBoxChangedActionAmt.Text = "";

                Aoc.ReadActionsOccurancesByUniqueRecordId(WUniqueRecordId);
                if (Aoc.RecordFound)
                {
                    if (Aoc.DoubleEntryAmt != Mpa.TransAmount)
                    {
                        //checkBoxChangeActionAmt.Checked = true;
                        textBoxChangedActionAmt.Text = Aoc.DoubleEntryAmt.ToString("#,##0.00");
                    }
                }

            }
            else
            {
                //Aoc.ReadActionsOccurancesByUniqueRecordId(WUniqueRecordId);
                //if (Aoc.RecordFound)
                //{
                //    MessageBox.Show("Not Allowed operation. Action already taken");
                //    return; 
                //}

                textBoxChangedActionAmt.Hide();
                textBoxChangedActionAmt.Text = "";
            }
        }

        private void labelNumberNotes3_Click(object sender, EventArgs e)
        {

        }
        //void NForm3_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    int WRow = dataGridView1.SelectedRows[0].Index;

        //    int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

        //    Form80b_Load(this, new EventArgs());

        //    dataGridView1.Rows[WRow].Selected = true;
        //    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

        //    dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        //}

    }
}

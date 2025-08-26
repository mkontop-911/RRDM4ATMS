using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm277d_MOB : UserControl
    {
        public event EventHandler ChangeBoardMessage;
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

        string WBankId;
        bool ViewWorkFlow;
        string WMode;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        int WOutstandingErrors;
        int WOutstandingUnMatched;
        string WMessage;

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        //
        int WDifStatus;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        //string WAtmNo;
        //int WSesNo;

        string WCategoryId;
        int WRMCycle;
        string W_Application; 

        public void UCForm277d_MOB_Par(string InSignedId, int SignRecordNo, string InOperator, string InAtmNo, int InSesNo, 
             string InCategory, int InRMCycle, string InW_Application)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            //WAtmNo = InAtmNo;
            //WSesNo = InSesNo;

            WCategoryId = InCategory;
            WRMCycle = InRMCycle;
            W_Application = InW_Application;

            InitializeComponent();

            if (WOperator == "BDACEGCA")
            {
                // ABE
                buttonAllAccounting.Hide();
            }

            Us.ReadUsersRecord(WSignedId);
            WBankId = Us.BankId;

            WMessage = "";

            // Check if outstanding not taken action by Maker 

            //string SearchingStringLeft = " WHERE Operator ='" + WOperator
            //                              + "' AND RMCateg ='" + WCategoryId + "'  "
            //                              + "  AND MatchingAtRMCycle = " + WRMCycle
            //                              + "  AND IsMatchingDone = 1 AND Matched = 0 "
            //                              + "  AND (ActionType = '00' AND MetaExceptionId <> 55 )  ";

            //string WSortValue = "";
            //Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, 1, SearchingStringLeft, WSortValue,
            //                                                 NullPastDate, NullPastDate, 1);

            //WOutstandingUnMatched = Mpa.TotalSelected;

            //if (WOutstandingUnMatched > 0)
            //{

            //    WMessage = "There are Outstanding UnMatched Records. " + WMessage;
            //    textBox1.Hide();
            //    textBox4.Show();

            //    if (WOutstandingUnMatched > 0 || WOutstandingErrors > 0)
            //    {
            //        textBox4.Text = "We Recommend To Pay Attention. " + WMessage;

            //        textBox4.ForeColor = Color.Red;
            //    }
            //}
            //else
            //{
                textBox1.Show();
                textBox4.Hide();
            //}

            // Update Step

            Usi.ReadSignedActivityByKey(WSignRecordNo);
            WDifStatus = Usi.ReconcDifferenceStatus;
            Usi.StepLevel = 3;
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
                Ap.GetMessageReconCateg(WCategoryId, WRMCycle, "ReconciliationCat", WAuthoriser, WRequestor, Reject);
                guidanceMsg = Ap.MessageOut;
            }
            //************************************************************
            //************************************************************

            SetScreen();
        }

        //*************************************
        // Set Screen
        //*************************************
        public void SetScreen()
        {
            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageReconCateg(WCategoryId, WRMCycle, "ReconciliationCat", WAuthoriser, WRequestor, Reject);
                guidanceMsg = Ap.MessageOut;
                ////STAVROS
                //ChangeBoardMessage(this, new EventArgs());
            }

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Reconc closing stage for" + " Category: " + WCategoryId + " Reconc Cycle: " + WRMCycle;
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

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WRMCycle, "ReconciliationCat"); //

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        guidanceMsg = " Finish Authorisation .";
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

        }

        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {
            if (ViewWorkFlow ==true & WAuthoriser == false & WRequestor == false )
            {
                // Close  
             Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycleVIEW_close(WCategoryId, WRMCycle, "ReconciliationCat");
            }
            else
            {
             Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WRMCycle, "ReconciliationCat");
            }
            //
            if ((Ap.RecordFound == true & Ap.OpenRecord == true)
                   || (Ap.RecordFound == true & Ap.OpenRecord == false & Ap.Stage == 5))
            {
                labelDtAuthRequest.Text = "Date of Request : " + Ap.DateOriginated.ToString();

                if (Ap.Stage == 1) StageDescr = "Authoriser Not Available yet.";
                if (Ap.Stage == 2) StageDescr = "Authoriser got the message. He will get action.";
                if (Ap.Stage == 3) StageDescr = "Authoriser must press finish to complete process ";

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

            Ap.ReadAuthorizationRecordByRMCategoryAndRMCycle(WCategoryId, WRMCycle);
            //Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationCat");

            if (Ap.RecordFound == true & Ap.OpenRecord == true)
            {
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                }
            }

            Ap.ReadAuthorizationRecordByRMCategoryAndRMCycle(WCategoryId, WRMCycle);
            //Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationCat");

            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
            {
                MessageBox.Show("This Reconciliation Cycle Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process."
                                                          );
                return;
            }

            // Validate input 
            //    InputValidationAndUpdate("Authorisation");

            //      if (ErrorReturn == true) return;
            int WTranNo = 0;

            string WOrigin = "ReconciliationCat";

            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management

            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, "", 0, AuthorSeqNumber,
                   WDifStatus, WCategoryId , WRMCycle,
                   "Normal");
            //NForm110.FormClosing += NForm110_FormClosing;
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();

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

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WRMCycle, "ReconciliationCat"); //

            if (WRequestor == true & Ap.Stage == 1)
            {
                guidanceMsg = "Message was sent to authoriser. Refresh for progress ";
                //ChangeBoardMessage(this, e);
            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                //guidanceMsg = "Authorisation made. Workflow can finish! ";
                //ChangeBoardMessage(this, e);
            }

            if (NormalProcess==true) // Orginator has deleted authoriser 
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
                (WCategoryId, WRMCycle, "ReconciliationCat"); //

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
           // ChangeBoardMessage(this, e);

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
           // ChangeBoardMessage(this, e);

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
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", "", 0, WDisputeNo, WDisputeTranNo, WCategoryId, WRMCycle);
            NForm112.ShowDialog();
        }

        // NOTES 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Reconc closing stage for" + " Category: " + WCategoryId + " Reconc Cycle: " + WRMCycle;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }
        // Print Actions 
        private void buttonPrintActions_Click(object sender, EventArgs e)
        {
            // Matching is done but not Settled 
            string WSelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WCategoryId + "'"
                      + "  AND MatchingAtRMCycle =" + WRMCycle
                      + " AND IsMatchingDone = 1 AND Matched = 0  "
                      //+ " AND IsMatchingDone = 1 AND Matched = 0 AND SettledRecord = 0 "
                      + " AND ActionType != '07' ";

            string WSortCriteria = " Order By TerminalId ";

            Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, WSelectionCriteria,
                                                                                     WSortCriteria, 1);

            string P1 = "Transactions For Reconciliation :" + WCategoryId + " AND Cycle : " + WRMCycle.ToString();

            string P2 = "";
            string P3 = "";

            if (ViewWorkFlow == true)
            {

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WRMCycle, "ReconciliationCat");

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

            string P4 = WBankId;
            string P5 = WSignedId;

            Form56R55ATMS ReportATMS55 = new Form56R55ATMS(P1, P2, P3, P4, P5);
            ReportATMS55.Show();
        }
        // All Actions 
        private void buttonAllActions_Click(object sender, EventArgs e)
        {
            string WSelectionCriteria = "";
            int WMode;
           
            WSelectionCriteria = "WHERE RMCateg='" + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle + " AND OriginWorkFlow ='Reconciliation'"; ;
          
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            
            // Auto Creation Of Transactions
            //
            bool Is_GL_Creation_Auto; 

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

        }
        // All Transactions 
        private void buttonAllAccounting_Click(object sender, EventArgs e)
        {

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "";

            string SelectionCriteria = "WHERE RMCateg='" + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle + " AND OriginWorkFlow ='Reconciliation'";
            // SelectionCriteria = "WHERE RMCateg='" + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo;
            Aoc.ReadActionsOccurancesAndFillTable_Big(SelectionCriteria);

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
    }
}

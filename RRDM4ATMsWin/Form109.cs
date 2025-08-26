using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text;
using System.Data;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form109 : Form
    {
        Form110 NForm110; // Authoriser
        Form112 NForm112;
        Form14b NForm14b;

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        bool Is_GL_Creation_Auto;
        bool Firsttime;

        bool AudiType; 

        decimal WExcess;
        decimal WShortage;

        int WMode2;
        string WCallerProcess;
        string WSelectionCriteria;
        string WActionId;

        bool NormalProcess;

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
        RRDMActions_GL Act = new RRDMActions_GL();

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        string WMaker;

        int TransType;
        bool ErrorReturn;
        bool DisputeAuthorisation;

        string StageDescr;

        Bitmap SCREENinitial;
        //   string AuditTrailUniqueID = "";

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        int WDisputeNumber;
        int WDispTranNo;

        int WMaskRecordId;

        int WSource;
        int WAuthorSeqNumber;

        public Form109(string InSignedId, int InSignRecordNo, string InOperator, int InDisputeNumber
            , int InDispTranNo, int InTranNo, int InSource)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WDisputeNumber = InDisputeNumber;
            WDispTranNo = InDispTranNo;

            WMaskRecordId = InTranNo;

            WSource = InSource; // 1 = comes from Dispute management, 2 = comes from Authorisation Management, 
                                // 11 = comes from dispute management for settled dispute transaction to view only 
                                // 12 = comes from Dispute pre investigation  
            InitializeComponent();

            // Set Working Date 

            labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            labelToday.Text = DateTime.Now.ToShortDateString();
            UserId.Text = InSignedId;
            pictureBox1.BackgroundImage = appResImg.logo2;

            Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
               int  IsAmountOneZero = (int)Gp.Amount;

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

            //   labelStep1.Text = "Investigation for Dispute No: " + WDispNo;
            textBoxMsgBoard.Text = "Choose Action to be taken. Based on action a transaction to be posted will be created. ";

            buttonAuthor.Hide();

            panelDiffDest.Hide();

            Dt.ReadDisputeTran(WDispTranNo);

            Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (Ap.RecordFound == true & (WSource == 1 || WSource == 12) & WViewFunction == false & WAuthoriser == false & WRequestor == false)
            {
                // It came from main dispute Form management 
                WRequestor = true;
            }



            if (WViewFunction || WAuthoriser || WRequestor)
            {
                NormalProcess = false;
            }
            else
            {
                NormalProcess = true;
                WMaker = WSignedId;

            }

            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageTwo(WDisputeNumber, WDispTranNo, WAuthoriser, WRequestor, Reject);
                textBoxMsgBoard.Text = Ap.MessageOut;
                WMaker = Ap.Requestor;

                buttonAction.Hide();
            }

            if (WSource == 11) // View Only
            {
                buttonAuthor.Enabled = false;
                buttonFinish.Enabled = false;
                buttonAction.Enabled = false;
                labelDiffDest.Hide();
                panelDiffDest.Hide();
                textBoxMsgBoard.Text = "View Only.";
                WViewFunction = true;
            }
            //************************************************************
            //************************************************************

            //
            // Auto Creation Of Transactions
            //
            string ParId = "945";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_GL_Creation_Auto = true;
                // buttonAccountingTxns.Show();
            }
            else
            {
                // buttonAccountingTxns.Hide();
            }

            Firsttime = true;

            SetScreen();
        }

        //*************************************
        // Set Screen
        //*************************************
        public void SetScreen()
        {
            // CHECK IF Dispute ELECTRONIC AUTHORISATION IS NEEDED
            string ParId = "260";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            // NO Cash In Management                                         
            if (Gp.OccuranceNm == "YES")
            {
                DisputeAuthorisation = true;
            }
            else
            {
                DisputeAuthorisation = false;
            }
            //
            // SET MAIN SCREEN
            //
            Dt.ReadDisputeTran(WDispTranNo);

            string SelectionCriteria = " WHERE UniqueRecordId = " + Dt.UniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

            label3.Text = "Transaction is in : " + Dt.CurrencyNm;
            textBoxTrace.Text = Mpa.TraceNoWithNoEndZero.ToString();
            textBoxTerminal.Text = Dt.AtmNo;
            textBoxCard.Text = Dt.CardNo;
            textBox3.Text = Dt.AccNo;
            textBox10.Text = Dt.TranAmount.ToString("#,##0.00");
            textBox11.Text = Dt.TransDesc;
            label18.Text = "Date : " + Dt.TranDate.ToString();
            Di.ReadDispute(Dt.DisputeNumber);
            label15.Text = "Customer : " + Di.CustName;

            textBox5.Text = Dt.DisputedAmt.ToString("#,##0.00");

            tbComments.Text = Di.DispComments;

            SelectionCriteria = " WHERE UniqueRecordId = " + Dt.UniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            if (Mpa.RecordFound == true)
            {
                // Clear previous actions
                // DELETE ALL ACTIONS OCCURANCES WITH STAGE 1
                // UPDATE DISPUTE AS NOT PROCESSED 
                Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);

                if (Firsttime == true & NormalProcess == true & Ap.RecordFound == false) // Without authorisation record 
                {
                    // HERE WE DELETE ALL ACTIONS WITH NOT EQUAL TO 
                    //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

                    //Aoc.DeleteActionsOccurances_ForUniqueNotAuthor("Master_Pool", Dt.UniqueRecordId);
                    Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Dt.UniqueRecordId, "89");
                    Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Dt.UniqueRecordId, "90");
                    Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Dt.UniqueRecordId, "95");
                    Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Dt.UniqueRecordId, "96");

                    Firsttime = false;
                }


                if (Mpa.ReplCycleNo > 0)
                {
                    //WHERE Atmno = '00000550' AND ReplCycle = 2220 And Is_GL_Action = 1
                    string WSelectionCriteria = "WHERE AtmNo ='" + Mpa.TerminalId
                        + "' AND ReplCycle =" + Mpa.ReplCycleNo
                        + " AND ( Stage<>'03' OR Stage = '03') ";


                    if (AudiType == true)
                    {
                        Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals_AUDI_Type(WSelectionCriteria);
                    }
                    else
                    {

                        Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

                    }
                    

                    textBoxExcessBal.Text = Aoc.Current_ExcessBalance.ToString("#,##0.00");
                    textBoxShortage.Text = Aoc.Current_ShortageBalance.ToString("#,##0.00");

                    textBoxDispShort.Text = Aoc.Current_DisputeShortage.ToString("#,##0.00");

                    WExcess = Aoc.Current_ExcessBalance;
                    WShortage = Aoc.Current_ShortageBalance;

                    textBoxATMNo.Text = Mpa.TerminalId;
                    textBoxReplCycle.Text = Mpa.ReplCycleNo.ToString();

                    labelRepl.Show();
                    panel8.Show();

                }
                else
                {
                    labelRepl.Hide();
                    panel8.Hide();
                }

            }

            // NOTES for Attachements 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Dt.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            if (Dt.DisputeActionId > 0)
            {
                if (Dt.DisputeActionId == 1) // CREDIT CUSTOMER
                {
                    radioButtonCrCust.Checked = true;
                }
                if (Dt.DisputeActionId == 2)
                {
                    radioButtonCreditDiff.Checked = true;
                    textBoxCrAmt.Text = Dt.DecidedAmount.ToString();
                }
                if (Dt.DisputeActionId == 3) // DEBIT CUSTOMER 
                {
                    radioButtonDrCust.Checked = true;
                }
                if (Dt.DisputeActionId == 4)
                {
                    radioButtonDebitDiff.Checked = true;
                    textBoxDrAmt.Text = Dt.DecidedAmount.ToString();
                }
                if (Dt.DisputeActionId == 5) // POSTPONED FOR CERTAIN DATE TIME
                {
                    radioButtonPostponed.Checked = true;
                    dateTimePicker1.Value = Dt.PostDate;
                }
                if (Dt.DisputeActionId == 6)
                {
                    radioButtonCancelDispute.Checked = true;
                }
                if (Dt.DisputeActionId == 7)
                {
                    radioButton7.Checked = true;
                }
            }

            if (Dt.ReasonForAction > 0)
            {
                if (Dt.ReasonForAction == 1)
                {
                    radioButton11.Checked = true;
                }
                if (Dt.ReasonForAction == 2)
                {
                    radioButton12.Checked = true;
                }
                if (Dt.ReasonForAction == 3)
                {
                    radioButton13.Checked = true;
                }
                if (Dt.ReasonForAction == 4)
                {
                    radioButton14.Checked = true;
                }
                if (Dt.ReasonForAction == 5)
                {
                    radioButton15.Checked = true;
                }
                if (Dt.ReasonForAction == 6)
                {
                    radioButton16.Checked = true;
                }
            }

            textBoxActionComments.Text = Dt.ActionComment;

            if (Dt.ClosedDispute == true & WSource != 11) // DISPUTE transaction is closed and need to reopen
            {
                checkBox1.Show();
                panel4.Hide();
                panel3.Hide();
                label2.Hide();
                label4.Hide();
                textBoxActionComments.Text = "";
                buttonAuthor.Hide();
                buttonFinish.Show();
                textBoxMsgBoard.Text = "Check box to ReOpen Disputed Transaction. Comments should be input too.";
                return;
            }
            else
            {
                checkBox1.Hide();
            }

            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(panel2.Width, panel2.Height);
            panel2.DrawToBitmap(memoryImage, panel2.ClientRectangle);
            SCREENinitial = memoryImage;
            //*****************************************************
            //*****************************************************
            // Set authorisation section
            //

            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        //  guidanceMsg = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {
                        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ProcessNo = 2; // Return to stage 2  
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                        NormalProcess = true; // TURN TO NORMAL TO SHOW WHAT IS NEEDED 

                        //buttonNotesRead.Hide();
                        //buttonNotesUpdate.Show();
                    }

                    // Show Authorisation record 
                    ShowAuthorisationInfo();
                }

            }

            if (NormalProcess & DisputeAuthorisation == true) // Normal Reconciliation with authorisation 
            {
                // Main buttons
                //buttonAuthor.Show();
                buttonRefresh.Hide();
                buttonFinish.Hide();
            }

            if (NormalProcess & DisputeAuthorisation == false) // Normal Reconciliation without authorisation 
            {
                // Main buttons
                buttonAuthor.Hide();
                buttonRefresh.Hide();
                //buttonAuthorisations.Hide();
            }

            if (NormalProcess) // Normal Reconciliation 
            {
                // Do not show Authorisation Section this will be shown after authorisation 
                labelAuthHeading.Hide();
                panelAuthor.Hide();
            }

            if (NormalProcess & Ap.Stage == 4 & Ap.AuthDecision == "NO") // Authorisation was rejected - mode has turned to 2
            {
                // Show Authorisation section
                labelAuthHeading.Show();
                panelAuthor.Show();
                ShowAuthorisationInfo();

                // Close Authorisation record 
                //
                Ap.Stage = 5;
                Ap.OpenRecord = false;

                Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
            }

            if ((WViewFunction == true & DisputeAuthorisation == true) || (WViewFunction == true & DisputeAuthorisation == true)
                  || WAuthoriser == true) // Comes from Author
            {
                ShowAuthorisationInfo();
            }
            else
            {
                if (WViewFunction == true & DisputeAuthorisation == false)
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();
                    //buttonAuthorisations.Hide();

                    // Do not show Authorisation Section 
                    labelAuthHeading.Hide();
                    panelAuthor.Hide();
                }

            }

        }


        //AUDIT TRAIL : GET IMAGE AND INSERT IT IN AUDIT TRAIL 
        private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory,
            string InTypeOfChange, string InUser, string Message)
        {
            //Bitmap SCREENb;
            //System.Drawing.Bitmap memoryImage;
            //memoryImage = new System.Drawing.Bitmap(panel2.Width, panel2.Height);
            //panel2.DrawToBitmap(memoryImage, panel2.ClientRectangle);
            //SCREENb = memoryImage;

            //AuditTrailClass At = new AuditTrailClass();

            //if (AuditTrailUniqueID.Equals(""))
            //{
            //    AuditTrailUniqueID = At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            //}
            //else
            //{
            //    At.UpdateRecord(AuditTrailUniqueID, InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            //}
        }
        // Update 

        private void ButtonFinish_Click(object sender, EventArgs e)
        {
            if (WViewFunction == true || WRequestor) // For View only 
            {
                //this.Close();
                //return;
            }

            ErrorReturn = false;

            // Update Dispute Tran 
            InputValidationAndUpdate("Update_Closed");


            if (ErrorReturn == true)
            {
                MessageBox.Show("There Is Error In Process!");
                return;
            }


            // FINISH - Make validationsfor Authorisations  
            bool AuthorNoRecordYet = false;
            bool AuthorDone = false;
            bool AuthorOutstanding = false;

            Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);

            if (Ap.RecordFound == true)
            {
                AuthorNoRecordYet = false;

                if (Ap.Stage == 3 || Ap.Stage == 4)
                {
                    AuthorDone = true;
                }
                else
                {
                    AuthorOutstanding = true;
                }
            }
            else
            {
                AuthorNoRecordYet = true;
            }

            if (AuthorNoRecordYet == true || (AuthorNoRecordYet == false & Ap.Stage < 3))
            {
                this.Close();
                return;
            }

            if (WAuthoriser == true & AuthorDone == true & Ap.AuthDecision == "NO")
            {
                // stop
                this.Close();
                return;
            }

            if (AuthorDone == true & Ap.AuthDecision == "YES")
            {
                // Continue
            }
            else
            {
                MessageBox.Show("Authorisation not done yet");
                return;
            }

            //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            //Close Authorization Record
            // create a connection object
            //  using (var scope = new System.Transactions.TransactionScope())
            try
            {
                if (Ap.Stage == 3 || Ap.Stage == 4)
                {
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);
                }

                // FIND CURRENT CUTOFF CYCLE
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                string WJobCategory = "ATMs";
                string WStage;

                int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                //******************************************************************

                //WHERE AtmNo = '00000550' AND ReplCycle = 2250 AND Stage<> '03'
                WSelectionCriteria = " WHERE UniqueRecordId = " + Dt.UniqueRecordId;
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria, 2);
                if (Mpa.RecordFound == true)
                {
                    WSelectionCriteria = " WHERE  OriginWorkFlow = 'Dispute' AND AtmNo ='" + Mpa.TerminalId + "' AND ReplCycle ="
                        + Mpa.ReplCycleNo + " AND Stage <> '03' AND Maker ='" + Ap.Requestor + "' ";
                    //+" AND ( (Maker ='" + WSignedId + "' "AND Stage<>'03) OR Stage = '03') ";
                }
                else
                {
                    // Cancel here 
                }

                Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

                if (Aoc.TableActionOccurances_Big.Rows.Count > 0)
                {
                    Aoc.ClearTableTxnsTableFromAction();

                    int I = 0;

                    while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
                    {

                        int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                        // Update authoriser 
                        Aoc.UpdateOccurancesForAuthoriser_2(WSeqNo, Ap.Authoriser, Ap.SeqNumber);

                        Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                        // Create Txn In Pool - Trans to be updated 
                        //
                        if (Aoc.Is_GL_Action == true)
                        {
                            WMode2 = 2; // 
                            WCallerProcess = "Dispute";
                            Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey, Aoc.ActionId
                                                                         , Aoc.Occurance
                                                                         , WCallerProcess, WMode2);
                        }
                        //******

                        if (Aoc.UniqueKeyOrigin == "Master_Pool")
                        {
                            // THIS 
                            WSelectionCriteria = " WHERE UniqueRecordId =" + Aoc.UniqueKey;
                            Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2);

                            Mpa.ActionType = Aoc.ActionId;
                            Mpa.ActionByUser = true;
                            Mpa.UserId = Ap.Requestor;
                            Mpa.Authoriser = Ap.Authoriser;
                            Mpa.AuthoriserDtTm = DateTime.Now;



                            //RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
                            //RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

                            //WDispNo = Di.Create_Pseudo_Dispute(WOperator, WSignedId, WWUniqueRecordId, 111);



                            if (radioButtonPostponed.Checked == true)
                            {
                                Mpa.SettledRecord = false;
                            }
                            else
                            {
                                Mpa.SettledRecord = true;
                            }

                            WSelectionCriteria = " WHERE UniqueRecordId =" + Aoc.UniqueKey;
                            Mpa.UpdateMatchingTxnsMasterPoolATMsForcedMatched(WOperator, WSelectionCriteria, 2);

                            Di.ReadDispute(WDisputeNumber);

                            if (Di.DispFrom == 111)
                            {
                                Mpa.SeqNo06 = 95;
                                Mpa.SeqNo05 = WReconcCycleNo;
                            }
                            else
                            {
                                // Leave it as it is 
                            }
                            WSelectionCriteria = " WHERE UniqueRecordId =" + Aoc.UniqueKey;
                            Mpa.UpdateMatchingTxnsMasterPoolATMs_SOLO_ACTION(WOperator, WSelectionCriteria, 2);

                        }

                        //********************************************
                        // HERE WE CREATE THE ENTRIES AS PER BDC NEEDS
                        //********************************************

                        I = I + 1;
                    }

                    // Update stage
                    WStage = "02"; // Confirmed by maker and Authorised 
                    Aoc.UpdateOccurancesStage("Master_Pool", Dt.UniqueRecordId, WStage, DateTime.Now, WReconcCycleNo, Ap.Requestor);
                    //
                    // Update authoriser where stage is '02' 
                    // AND MAKE "03"
                    Aoc.UpdateOccurancesForAuthoriser("Master_Pool", Dt.UniqueRecordId, Ap.Authoriser, Ap.SeqNumber, Ap.Requestor);

                    if (Mpa.Origin == "Our Atms")
                    {
                        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

                        // UPDATE Ta
                        string WAtmNo = Mpa.TerminalId;
                        int WSesNo = Mpa.ReplCycleNo;

                        if (Mpa.ReplCycleNo > 0)
                        {
                            // There is Replenishment Cycle
                            Aoc.ReadActionsOccurancesTo_RichPicture_One_ATM(WAtmNo, WSesNo);

                            if (Aoc.Current_ShortageBalance < 0
                                || Aoc.WaitForDisputeNo < Aoc.WaitAndSettledDisputeNo
                                || Aoc.NoWaitDisputeNo < Aoc.NoWaitSettledDisputeNo)
                            {
                                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                                Ta.Repl1.DiffRepl = true;
                                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
                            }
                            else
                            {
                                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                                Ta.Repl1.DiffRepl = false;
                                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
                            }
                        }

                    }
                }

                //  scope.Complete();

                //this.Close();

                //return;


            }
            catch (Exception ex)
            {

                RRDMLog4Net Log = new RRDMLog4Net();

                StringBuilder WParameters = new StringBuilder();

                WParameters.Append("User : ");
                WParameters.Append("NotAssignYet");
                WParameters.Append(Environment.NewLine);

                WParameters.Append("ATMNo : ");
                WParameters.Append("NotDefinedYet");
                WParameters.Append(Environment.NewLine);

                string Logger = "RRDM4Atms";
                string Parameters = WParameters.ToString();

                Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");

                Environment.Exit(0);
            }
            finally
            {
                // scope.Dispose();
            }

            MessageBox.Show("Updating Completed");

            this.Dispose();
            // this.Close();

            // return;




            //string WUniqueRecordIdOrigin = "Master_Pool";
            //Aoc.ClearTableTxnsTableFromAction();

            ////Aoc.ReadActionsTxnsCreateTableByUniqueKey(WUniqueRecordIdOrigin, Dt.UniqueRecordId, "All");

            //WMode2 = 2; // Create transaction in pool 
            //WCallerProcess = "Dispute";
            //Aoc.ReadActionsTxnsCreateTableByUniqueKey("Master_Pool", Dt.UniqueRecordId, "All",1
            //                                             , WCallerProcess, WMode2);

            //TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //Form14b_All_Actions NForm14b_All_Actions;
            //NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TEMPTableFromAction, 1);
            //NForm14b_All_Actions.ShowDialog();

            buttonAction.Enabled = false;
            buttonUndoAction.Show();
            buttonUndoAction.Enabled = true;
            buttonAuthor.Show();

            textBoxMsgBoard.Text = "Action Has Been Applied. ";

            ////MessageBox.Show("No transactions to be posted");
            //Form2 MessageForm = new Form2("Action on Dispute Transaction Updated -" + Environment.NewLine
            //                               + "Transaction to be posted was created as a result" + Environment.NewLine
            //                               );
            //MessageForm.ShowDialog();





            //AUDIT TRAIL 
            string AuditCategory = "Operation";
            string AuditSubCategory = "Disputes";
            string AuditAction = "Update";
            string Message = "Action Updated";
            GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);

        }

        // Input Validation and updating 
        private void InputValidationAndUpdate(string InFunction)
        {
            ErrorReturn = false;
            Dt.ReadDisputeTran(WDispTranNo);

            if (InFunction == "Authorisation")
            {
                Dt.ChooseAuthor = true;
                Dt.UpdateDisputeTranRecord(WDispTranNo);

                return;
            }

            if (checkBox1.Checked == true & Dt.ClosedDispute == true)
            {
                if (textBoxActionComments.Text.Length < 3)
                {
                    MessageBox.Show(" Please Enter Comments");
                    ErrorReturn = true;
                    return;
                }

                checkBox1.Hide();
                checkBox1.Checked = false;
                panel4.Show();
                panel3.Show();
                label2.Show();
                label4.Show();
                textBoxMsgBoard.Text = "Choose Action to be taken. Based on action a transaction will be created. ";

                Dt.ReadDisputeTran(WDispTranNo);

                Dt.ClosedDispute = false;
                Dt.OpenDispTran = Dt.OpenDispTran + 1;
                Dt.DecidedAmount = 0;
                Dt.ActionComment = "";
                Dt.ReasonForAction = 0;
                Dt.UpdateDisputeTranRecord(WDispTranNo);

                Di.ReadDispute(Dt.DisputeNumber);
                Di.Active = true;
                Di.UpdateDisputeRecord(Dt.DisputeNumber);

                Ap.DeleteAuthorisationRecord(Dt.AuthorKey);

                MessageBox.Show(" Tran is now active");
                buttonAuthor.Show();
                buttonFinish.Hide();
                ErrorReturn = true;
                return;
            }

            if (Dt.ClosedDispute == true & checkBox1.Checked == false)
            {
                MessageBox.Show("Dispute Tran is closed. Open it to proceed");
                ErrorReturn = true;
                return;
            }
            // CHECK INPUT 
            if (radioButtonCrCust.Checked == false & radioButtonCreditDiff.Checked == false & radioButtonDrCust.Checked == false &
                 radioButtonDebitDiff.Checked == false & radioButtonPostponed.Checked == false &
                radioButtonCancelDispute.Checked == false & radioButton7.Checked == false)
            {
                MessageBox.Show(" Please Choose Dispute Action");
                ErrorReturn = true;
                return;
            }

            if (radioButton11.Checked == false & radioButton12.Checked == false & radioButton13.Checked == false &
                radioButton14.Checked == false & radioButton15.Checked == false & radioButton16.Checked == false)
            {
                MessageBox.Show(" Please Choose Dispute Reason");
                ErrorReturn = true;
                return;
            }

            if (textBoxActionComments.Text.Length < 3)
            {
                MessageBox.Show(" Please Enter Comments");
                ErrorReturn = true;
                return;
            }

            // DISPUTE ACTION ID
            //
            if (radioButtonCrCust.Checked == true)  // CREDIT CUSTOMER 
            {
                textBoxCrAmt.Text = "";
                textBoxDrAmt.Text = "";
                Dt.DisputeActionId = 1;
                Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 21;
            }

            if (radioButtonCreditDiff.Checked == true)
            {
                //textBoxDrAmt.Text = "";
                if (decimal.TryParse(textBoxCrAmt.Text, out Dt.DecidedAmount))
                {
                }
                else
                {
                    MessageBox.Show(textBoxCrAmt.Text, "Please enter valid money amount!");
                    ErrorReturn = true;
                    return;
                }
                if (Dt.DecidedAmount > Dt.DisputedAmt)
                {
                    MessageBox.Show(textBoxCrAmt.Text, "Please enter valid money amount!");
                    ErrorReturn = true;
                    return;
                }
                Dt.DisputeActionId = 2;
                //Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 21;
                //     Ec.CreateTransTobepostedfromDisputes(WSignedId, Dt.DispTranNo, TransType, Dt.DecidedAmount);
                //     textBoxMsgBoard.Text = "A Transaction to be posted is created. "; 
            }

            if (radioButtonDrCust.Checked == true)  // DEDIT CUSTOMER 
            {
                textBoxCrAmt.Text = "";
                textBoxDrAmt.Text = "";
                Dt.DisputeActionId = 3;
                Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 11;
                //     Ec.CreateTransTobepostedfromDisputes(WSignedId, Dt.DispTranNo, TransType, Dt.DecidedAmount);
                //    textBoxMsgBoard.Text = "A Transaction to be posted is created. ";
            }

            if (radioButtonDebitDiff.Checked == true)
            {
                textBoxCrAmt.Text = "";

                if (decimal.TryParse(textBoxDrAmt.Text, out Dt.DecidedAmount))
                {
                }
                else
                {
                    MessageBox.Show(textBoxDrAmt.Text, "Please enter valid money amount!");
                    ErrorReturn = true;
                    return;
                }
                if (Dt.DecidedAmount > Dt.DisputedAmt)
                {
                    MessageBox.Show(textBoxCrAmt.Text, "Please enter valid money amount!");
                    ErrorReturn = true;
                    return;
                }
                Dt.DisputeActionId = 4;
                //Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 11;
                //    Ec.CreateTransTobepostedfromDisputes(WSignedId, Dt.DispTranNo, TransType, Dt.DecidedAmount);
                //    textBoxMsgBoard.Text = "A Transaction to be posted is created. ";

            }

            if (radioButtonPostponed.Checked == true)
            {
                if (radioButtonPostponed.Checked == true) Dt.DisputeActionId = 5; // Postponed 
                Dt.PostDate = dateTimePicker1.Value;
                if (DateTime.Now >= Dt.PostDate.Date)
                {
                    MessageBox.Show(textBoxDrAmt.Text, "Please enter valid date greater than today!");
                    ErrorReturn = true;
                    return;
                }

            }

            if (radioButtonCancelDispute.Checked == true) Dt.DisputeActionId = 6; // Cancel dispute 

            if (radioButton7.Checked == true) Dt.DisputeActionId = 7; // Legal action 

            // DISPUTE REASON 
            if (radioButton11.Checked == true) Dt.ReasonForAction = 1;
            if (radioButton12.Checked == true) Dt.ReasonForAction = 2;
            if (radioButton13.Checked == true) Dt.ReasonForAction = 3;
            if (radioButton14.Checked == true) Dt.ReasonForAction = 4;
            if (radioButton15.Checked == true) Dt.ReasonForAction = 5;
            if (radioButton16.Checked == true) Dt.ReasonForAction = 6;

            Dt.ActionComment = textBoxActionComments.Text;
            Dt.ActionDtTm = DateTime.Now;

            if (InFunction == "Update_Pre" &
                (Dt.DisputeActionId == 1 || Dt.DisputeActionId == 2 || Dt.DisputeActionId == 3
                || Dt.DisputeActionId == 4 || Dt.DisputeActionId == 5 || Dt.DisputeActionId == 6)
               )
            {
                Dt.UpdateDisputeTranRecord(WDispTranNo);
            }

            if (InFunction == "Update_Closed")
            {
                if (radioButtonPostponed.Checked == true)
                {
                    // Donot Close this
                    //  Dt.ClosedDispute = false;
                }
                else
                {
                    Dt.ClosedDispute = true;
                }

                Dt.PendingAuthorization = false;
                // UPDATE Disputes Transaction record
                Dt.UpdateDisputeTranRecord(WDispTranNo);

                if (Dt.ErrorFound)
                {
                    MessageBox.Show(Dt.ErrorOutput, "System Error During Updating",
                                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorReturn = true;
                    return;
                }

                string SelectionCriteria = " WHERE UniqueRecordId =" + Dt.UniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                if (Mpa.ActionType == "05" & Dt.DisputeActionId == 6)
                {
                    Mpa.ActionType = "00";

                    Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, Dt.UniqueRecordId, 2);
                }


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
                        ErrorReturn = true;
                        return;
                    }

                }
            }
        }

        // AUTHORISER SECTION 
        // Authorise 

        private void ButtonAutho(object sender, EventArgs e)
        {
            UpdateAuthorRecord("YES");
        }

        // Reject Authorization 

        private void buttonReject_Click(object sender, EventArgs e)
        {
            if (textBoxComment.TextLength < 5)
            {
                MessageBox.Show("Please input comment to explain rejection");
                return;
            }
            UpdateAuthorRecord("NO");

        }

        // FINISH Update Authorization Record 

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
                    labelAuthStatus.Text = "Current Status : " + "Authoriser Accepted - Finish will be pressed";
                    MessageBox.Show("Authorization ACCEPTED! by : " + labelAuthoriser.Text);
                    //this.Dispose();
                }
                if (InDecision == "NO")
                {
                    labelAuthStatus.Text = "Current Status : " + "Authoriser Rejected the Action";
                    MessageBox.Show("Authorization REJECTED! by : " + labelAuthoriser.Text);
                    this.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Authorization record is not open. Requestor has closed it.");
                return;
            }

        }

        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {
            Dt.ReadDisputeTran(WDispTranNo);

            if (WViewFunction == true)
            {
                // Close Record just for viewing 
                Ap.ReadAuthorizationForDisputeAndTransaction_VIEW(Dt.DisputeNumber, Dt.DispTranNo);
            }
            else
            {
                // Open Record
                Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);
            }



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
            }
            else
            {
                return;
            }

            labelAuthHeading.Show();
            panelAuthor.Show();

            if (WViewFunction == true) // For View only 
            {
                // Main buttons
                buttonAuthor.Hide();
                buttonRefresh.Hide();
                buttonFinish.Show();
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
                buttonFinish.Show();
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
                buttonFinish.Show();
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
                    buttonFinish.Hide();
                    // Authoriser
                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }
                //if (Ap.Stage == 4 & Ap.AuthDecision == "YES") // Authorised and accepted
                //{
                //    // Main buttons
                //    buttonAuthor.Hide();
                //    buttonRefresh.Hide();
                //    buttonFinish.Show();
                //    buttonFinish.Enabled = true; 
                //    // Authoriser
                //    buttonAuthorise.Hide();
                //    buttonReject.Hide();
                //    textBoxComment.ReadOnly = true;

                //    MessageBox.Show("Once Authorised Please Press the Button Finish"); 

                //}
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO") // Authorised but rejected
                {
                    // Main buttons
                    buttonAuthor.Show();
                    buttonRefresh.Hide();
                    buttonFinish.Hide();
                    // Authoriser
                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }
            }

        }

        // REFRESH 

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            SetScreen();
        }
        // 
        private void buttonAuthor_Click(object sender, EventArgs e)
        {

            ErrorReturn = false;

            // Check if Already in authorization process

            // Dt.ReadDisputeTran(WDispTranNo);

            Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);
            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // NOT COMMING FROM REOPENED DISPUTE 
            {
                if (Ap.Stage == 4)
                {
                    buttonFinish.Show();
                }
                MessageBox.Show("This Dispute Record Already has authorization record!");
                return;
            }

            // Validate input 
            //OriginId
            // "01" OurATMS-Matching
            // "02" BancNet Matching                               
            // "03" OurATMS-Reconc
            // "04" OurATMS-Repl
            // "05" Settlement
            // "07" Disputes 
            // "08" Settlement 
            // 
            InputValidationAndUpdate("Authorisation");

            string WOrigin = "Dispute Action";
            string WAtmNo = "";
            int WReplCycle = 0;
            if (ErrorReturn == true) return;
            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WMaskRecordId, WAtmNo, WReplCycle, AuthorSeqNumber,
                 0, "", 0
                , "Normal");
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();

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

            Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);

            if (WRequestor == true & Ap.Stage == 1)
            {
                textBoxMsgBoard.Text = "Message was sent to authoriser. Refresh for progress ";
            }

            if (Ap.Stage == 4)
            {
                textBoxMsgBoard.Text = "Authorisation made. Workflow can finish! ";
                buttonUndoAction.Enabled = false;

                Dt.ReadDisputeTran(WDispTranNo);

                Dt.Authorised = true;

                Dt.UpdateDisputeTranRecord(WDispTranNo);


                buttonFinish.Show();


            }

            if (NormalProcess) // Orginator has deleted authoriser 
            {
                textBoxMsgBoard.Text = "Please make authorisation ";
            }

            SetScreen();
        }
        // Reopen Dispute 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {

            }
        }
        // History of authorisation 


        private void buttonHistory_Click(object sender, EventArgs e)
        {
            string WAtmNo = "";
            int WReplCycle = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WAtmNo, WReplCycle, Di.DispId, Dt.DispTranNo, "", 0);
            NForm112.ShowDialog();
        }

        string SavedComments;
        // Attached Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            SavedComments = textBoxActionComments.Text;
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "UniqueRecordId: " + Dt.UniqueRecordId;
            // string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            // "UniqueRecordId: " + Dt.UniqueRecordId;
            string SearchP4 = "";
            string WMode;
            if (WViewFunction == false) WMode = "Update";
            else WMode = "Read";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.FormClosed += NForm197_FormClosed;
            NForm197.ShowDialog();
        }

        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetScreen();

            textBoxActionComments.Text = SavedComments;
        }
        // Credit differently 
        private void radioButtonCreditDiff_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCreditDiff.Checked == true)
            {
                textBoxCrAmt.Enabled = true;
                labelDiffDest.Show();
                panelDiffDest.Show();
            }
            else
            {
                textBoxCrAmt.Enabled = false;
                labelDiffDest.Hide();
                panelDiffDest.Hide();
            }
        }

        private void radioButtonDebitDiff_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDebitDiff.Checked == true)
            {
                textBoxDrAmt.Enabled = true;
                labelDiffDest.Show();
                panelDiffDest.Show();
            }
            else
            {
                textBoxDrAmt.Enabled = false;
                labelDiffDest.Hide();
                panelDiffDest.Hide();
            }
        }
        // Proceed to action
        DataTable TEMPTableFromAction;
        decimal DoubleEntryAmt;
        private void buttonAction_Click(object sender, EventArgs e)
        {
            if (radioButtonCreditDiff.Checked == true || radioButtonDebitDiff.Checked == true)
            {
                // MessageBox.Show("Please communicate with RRDM to activate this functionality");
                // return; 
            }
            //
            // Make Validation and update
            //
            InputValidationAndUpdate("Update_Pre");

            if (ErrorReturn == true) return;

            string WSelection = "";

            if (radioButtonPostponed.Checked == true || radioButtonCancelDispute.Checked == true)
            {
                if (radioButtonPostponed.Checked == true)
                {

                    Act.ReadActionByActionId(WOperator, "10", 1);
                    if (Act.RecordFound == true)
                    {
                        string _ActionId = "10";
                        string _WCcy = "EGP";
                        DoubleEntryAmt = Dt.TranAmount;
                        // string WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                        string _WUniqueRecordIdOrigin = "Master_Pool";
                        string _WMaker_ReasonOfAction = "Had To PostPone Dispute";
                        string _WOriginWorkFlow = "Dispute";

                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              _ActionId, _WUniqueRecordIdOrigin,
                                                              Mpa.UniqueRecordId, _WCcy, DoubleEntryAmt, Mpa.TerminalId, Mpa.ReplCycleNo
                                                              , _WMaker_ReasonOfAction, _WOriginWorkFlow);
                    }
                }


                if (radioButtonCancelDispute.Checked == true)
                {
                    // CREATE ACTION 11 IF IT DOES EXISTS 
                    Act.ReadActionByActionId(WOperator, "11", 1);
                    if (Act.RecordFound == true)
                    {
                        string _ActionId = "11";
                        string _WCcy = "EGP";
                        DoubleEntryAmt = Dt.TranAmount;
                        // string WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                        string _WUniqueRecordIdOrigin = "Master_Pool";
                        string _WMaker_ReasonOfAction = "Had To Cancel Dispute";
                        string _WOriginWorkFlow = "Dispute";

                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              _ActionId, _WUniqueRecordIdOrigin,
                                                              Mpa.UniqueRecordId, _WCcy, DoubleEntryAmt, Mpa.TerminalId, Mpa.ReplCycleNo
                                                              , _WMaker_ReasonOfAction, _WOriginWorkFlow);
                    }
                }

                buttonAction.Enabled = false;
                buttonUndoAction.Show();
                buttonUndoAction.Enabled = true;
                buttonAuthor.Show();
                textBoxMsgBoard.Text = "Move to Authorise the action";

                SetScreen();

                return;
            }

            //
            // Do transactions or Create actions 
            //
            decimal DoubleEntryAmt_1 = 0;
            decimal DoubleEntryAmt_2 = Dt.TranAmount - Dt.DecidedAmount; // the difference
            WActionId = "";
            string WUniqueRecordIdOrigin = "";
            string WCcy = "";

            // Examine At what stage we are 
            string WOriginWorkFlow = "Dispute";


            if (radioButtonCrCust.Checked == true)
            {
                DoubleEntryAmt_1 = Dt.DisputedAmt;
            }
            if (radioButtonDrCust.Checked == true)
            {
                DoubleEntryAmt_1 = Dt.DisputedAmt;
            }
            if (radioButtonCreditDiff.Checked == true)
            {
                DoubleEntryAmt_1 = Dt.DecidedAmount;
            }
            if (radioButtonDebitDiff.Checked == true)
            {
                DoubleEntryAmt_1 = Dt.DecidedAmount;
            }
            string WMaker_ReasonOfAction = textBoxActionComments.Text;
            //
            // Refund to customer
            //
            if (radioButtonCrCust.Checked == true || radioButtonCreditDiff.Checked == true)
            {
                if (DoubleEntryAmt_1 > WExcess)
                {
                    RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 

                    // Read Traces to Process
                    Ta.ReadSessionsStatusTraces(Mpa.TerminalId, Mpa.ReplCycleNo);

                    if (Ta.ProcessMode == -1 || Ta.ProcessMode == 0)
                    {
                        WActionId = "89"; // 89_DEBIT Dispute Shortage/CREDIT Branch Excess

                        MessageBox.Show("Replenishment not done yet" + Environment.NewLine
                            + "Money From Dispute Shortage will be used" + Environment.NewLine
                            + "to facilitate crediting the customer" + Environment.NewLine
                                    + "Action ID:.." + WActionId
                                  + ""
                                   );

                    }
                    else
                    {

                        WActionId = "90"; //90_DEBIT Branch Shortage/CREDIT Branch Excess
                                          // WUniqueRecordIdOrigin = "Replenishment";

                        MessageBox.Show("Money From CIT Shortage will be used" + Environment.NewLine
                             + "to facilitate crediting the customer" + Environment.NewLine
                                    + "Action ID:.." + WActionId
                                    + ""
                                     );
                    }
                    // Decision 

                    // int WUniqueRecordId = Mpa.ReplCycleNo; // SesNo 
                    WCcy = "EGP";
                    DoubleEntryAmt = DoubleEntryAmt_1 - WExcess;
                    // string WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                    WUniqueRecordIdOrigin = "Master_Pool";
                    WMaker_ReasonOfAction = "Had To Move Money to Excess";

                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WActionId, WUniqueRecordIdOrigin,
                                                          Mpa.UniqueRecordId, WCcy, DoubleEntryAmt, Mpa.TerminalId, Mpa.ReplCycleNo
                                                          , WMaker_ReasonOfAction, WOriginWorkFlow);

                    TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }
                // And 
                int WUniqueRecordId;

                if (Mpa.TXNSRC == "1" & Mpa.TXNDEST == "1")
                {
                    WActionId = "95"; //95_Refund Money to Customer(112-FLEX)

                    WUniqueRecordId = Mpa.ReplCycleNo; // SesNo 
                    WCcy = "EGP";
                    DoubleEntryAmt = DoubleEntryAmt_1;
                    WUniqueRecordIdOrigin = "Master_Pool";

                    //// string WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                    //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                    //                                      WActionId, WUniqueRecordIdOrigin,
                    //                                      Mpa.UniqueRecordId, WCcy, DoubleEntryAmt, Mpa.TerminalId, Mpa.ReplCycleNo
                    //                                      , WMaker_ReasonOfAction, WOriginWorkFlow);

                    //TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }
                else
                {
                    WActionId = "96"; //96_Refund Money to Settlement(112_OTHER)

                    WUniqueRecordId = Mpa.ReplCycleNo; // SesNo 
                    WCcy = "EGP";
                    DoubleEntryAmt = DoubleEntryAmt_1;
                    WUniqueRecordIdOrigin = "Master_Pool";

                    // string WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                    //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                    //                                      WActionId, WUniqueRecordIdOrigin,
                    //                                      Mpa.UniqueRecordId, WCcy, DoubleEntryAmt, Mpa.TerminalId, Mpa.ReplCycleNo
                    //                                      , WMaker_ReasonOfAction, WOriginWorkFlow);

                    //TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }

                NForm14b = new Form14b(WSignedId, WOperator,
                                    WUniqueRecordIdOrigin, Mpa.UniqueRecordId,
                                          WActionId, WMaker_ReasonOfAction, DoubleEntryAmt, WOriginWorkFlow, Mpa.ReplCycleNo);
                NForm14b.FormClosed += NForm14b_FormClosed;
                NForm14b.ShowDialog();

                // Leave it here 
                WUniqueRecordIdOrigin = "Master_Pool";

                WCcy = "EGP";
            }

            buttonAction.Enabled = false;
            buttonUndoAction.Show();
            buttonUndoAction.Enabled = true;
            buttonAuthor.Show();
            textBoxMsgBoard.Text = "Move to Authorise the action";

            SetScreen();
            // ******************
            return;
            // ********************
            // 
            // Debit Customer
            //
            if (radioButtonDrCust.Checked == true || radioButtonDebitDiff.Checked == true)
            {

                WActionId = "13"; //13_DEBIT Customer/CR_Branch Shortage
                WUniqueRecordIdOrigin = "Master_Pool";
                WCcy = "EGP";
            }
            //****

            string comboBoxReasonOfAction = "Decided by the commitee for.." + DoubleEntryAmt_1.ToString();

            NForm14b = new Form14b(WSignedId, WOperator,
                                    WUniqueRecordIdOrigin, Dt.UniqueRecordId,
                                          WActionId, comboBoxReasonOfAction, DoubleEntryAmt_1, "Dispute", 0);
            NForm14b.FormClosed += NForm14b_FormClosed;
            NForm14b.ShowDialog();
            //****

            //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                         WActionId, WUniqueRecordIdOrigin,
            //                                         Dt.UniqueRecordId, WCcy, DoubleEntryAmt_1, "", 0, WMaker_ReasonOfAction);
            ////
            // If Difference in Amount - Take to ATM Cash   
            //
            if (radioButtonDiffDestATM.Checked == true)
            {
                if (radioButtonCreditDiff.Checked == true)
                {
                    // The rest not credited to customer should go to ATM 

                    // 63_CREDIT AtmCash/DR_Branch Excess
                    WActionId = "63";
                    WUniqueRecordIdOrigin = "Master_Pool";

                    WCcy = "EGP";
                }

                if (radioButtonDebitDiff.Checked == true)
                {
                    // The rest not debited to customer should go to ATM cash

                    // 53_DEBIT AtmCash/CR_Branch Shortage
                    WActionId = "53";
                    WUniqueRecordIdOrigin = "Master_Pool";

                    WCcy = "EGP";
                }

                //
                // If Difference in Amount - Take to Branch Difference    
                //
                if (radioButtonCreditDiff.Checked == true)
                {
                    // The rest not credited to customer should go to Branch Diff

                    // 64_CREDIT Branch_Diff/DR_Branch Excess


                    WActionId = "64";
                    WUniqueRecordIdOrigin = "Master_Pool";

                    WCcy = "EGP";
                }

                if (radioButtonDebitDiff.Checked == true & radioButtonDiffToBranch.Checked == true)
                {
                    // The rest not debited to customer should go to Branch Diff

                    // 54_DEBIT Branch_Diff/ CR_Branch Shortage

                    WActionId = "54";
                    WUniqueRecordIdOrigin = "Master_Pool";

                    WCcy = "EGP";
                }

                // Second entry if difference 
                if (radioButtonCreditDiff.Checked == true || radioButtonDebitDiff.Checked == true)
                {
                    MessageBox.Show("Second Entry will be created now due to difference in amt decided");
                    comboBoxReasonOfAction = "Second Entry for Differences";

                    NForm14b = new Form14b(WSignedId, WOperator,
                                            WUniqueRecordIdOrigin, Dt.UniqueRecordId,
                                                  WActionId, comboBoxReasonOfAction, DoubleEntryAmt_2, "Dispute", 0);
                    NForm14b.FormClosed += NForm14b_FormClosed; ;
                    NForm14b.ShowDialog();
                    //WMaker_ReasonOfAction = textBoxActionComments.Text;
                    //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                    //                                    WActionId, WUniqueRecordIdOrigin,
                    //                                    Dt.UniqueRecordId, WCcy, DoubleEntryAmt_2, "", 0, WMaker_ReasonOfAction);

                }

            }


            if (radioButtonCreditDiff.Checked == true || radioButtonDebitDiff.Checked == true)
            {
                MessageBox.Show("Both will be shown");
                Aoc.ClearTableTxnsTableFromAction();
                int WMode2 = 1; // Create transaction in pool 
                string WCallerProcess = "Dispute";
                Aoc.ReadActionsTxnsCreateTableByUniqueKey("Master_Pool", Dt.UniqueRecordId, "All", 1
                                                             , WCallerProcess, WMode2);
                TEMPTableFromAction = Aoc.TxnsTableFromAction;
                Form14b_All_Actions NForm14b_All_Actions;
                NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TEMPTableFromAction, 1);
                NForm14b_All_Actions.ShowDialog();
            }

            buttonAction.Enabled = false;
            buttonUndoAction.Show();
            buttonUndoAction.Enabled = true;
            buttonAuthor.Show();
            textBoxMsgBoard.Text = "Move to Authorise the action";

        }

        private void NForm14b_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (NForm14b.Confirmed == true)
            {
                // LEAVE IT HERE
                string SelectionCriteria = " WHERE UniqueRecordId =" + Dt.UniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                Mpa.MetaExceptionNo = 0;
                Mpa.ActionByUser = true;
                Mpa.UserId = WSignedId;

                Mpa.ActionType = WActionId; // 

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, Dt.UniqueRecordId, 2);

            }
            else
            {
                buttonAction.Enabled = true;
                buttonAuthor.Enabled = false; 
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Mpa.UniqueRecordId, WActionId);
            }



        }

        // UNDO PROCESSED
        private void buttonUndoAction_Click(object sender, EventArgs e)
        {
            //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            // DELETE ALL ACTIONS OCCURANCES WITH STAGE 1
            // UPDATE DISPUTE AS NOT PROCESSED 
            if (radioButtonPostponed.Checked == true || radioButtonCancelDispute.Checked == true)
            {
                string WSelection = "";
                if (radioButtonPostponed.Checked == true)
                {
                    // Postponed 
                    WSelection = " WHERE UniqueKey =" + Dt.UniqueRecordId + " AND ActionId= '10'";
                    Aoc.ReadCheckActionsOccuarnceBySelectionCriteria(WSelection);

                    if (Aoc.RecordFound == true)
                    {
                        Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Dt.UniqueRecordId, "10");
                    }
                }
                if (radioButtonCancelDispute.Checked == true)
                {
                    // Cancel 
                    WSelection = " WHERE UniqueKey =" + Dt.UniqueRecordId + " AND ActionId= '11'";
                    Aoc.ReadCheckActionsOccuarnceBySelectionCriteria(WSelection);
                    if (Aoc.RecordFound == true)
                    {
                        Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Dt.UniqueRecordId, "11");
                    }
                }

            }
            else
            {

                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Dt.UniqueRecordId, "89");
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Dt.UniqueRecordId, "90");

                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", Dt.UniqueRecordId, WActionId);
            }

            MessageBox.Show("Transaction is now active");
            buttonAction.Enabled = true;
            buttonUndoAction.Hide();
            buttonAuthor.Hide();
            buttonFinish.Hide();

            SetScreen();

        }
        // Accounting Txns 
        private void buttonAccountingTxns_Click(object sender, EventArgs e)
        {
            string WUniqueRecordIdOrigin = "Master_Pool";
            // RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            Aoc.ClearTableTxnsTableFromAction();

            int WMode2 = 1; // 
            string WCallerProcess = "Dispute";
            Aoc.ReadActionsTxnsCreateTableByUniqueKey("Master_Pool", Dt.UniqueRecordId, "All", 1
                                                         , WCallerProcess, WMode2);
            TEMPTableFromAction = Aoc.TxnsTableFromAction;
            Form14b_All_Actions NForm14b_All_Actions;
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TEMPTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();

        }
        // 
        private void radioButtonCrCust_CheckedChanged(object sender, EventArgs e)
        {
            panelDiffDest.Hide();
        }
        // Go to Investigation Screen
        private void linkLabelAuthInvestigation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form4 NForm4;
            NForm4 = new Form4(WSignedId, WSignRecordNo, WOperator, WOperator, WDisputeNumber);
            NForm4.ShowDialog();
        }
        // ALL Actions
        private void buttonAllActions_Click(object sender, EventArgs e)
        {
            if (Mpa.ReplCycleNo > 0)
            {
                //WHERE Atmno = '00000550' AND ReplCycle = 2220 And Is_GL_Action = 1
                string WSelectionCriteria = "WHERE AtmNo ='" + Mpa.TerminalId
                    + "' AND ReplCycle =" + Mpa.ReplCycleNo
                     + " AND ( ( Stage<>'03') OR Stage = '03') ";

                Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

                Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

                string WUniqueRecordIdOrigin = "Master_Pool";

                Form14b_All_Actions NForm14b_All_Actions;
                int WMode = 3; // Actions 
                NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
                NForm14b_All_Actions.ShowDialog();
            }
            else
            {
                string WSelectionCriteria = "WHERE UniqueKey =" + Mpa.UniqueRecordId + " AND UniqueKeyOrigin = 'Master_Pool' ";

                Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

                string WUniqueRecordIdOrigin = "Master_Pool";

                Form14b_All_Actions NForm14b_All_Actions;
                int WMode = 3; // Actions 
                NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
                NForm14b_All_Actions.ShowDialog();
            }

        }
        // ALL Accounting 
        private void buttonAllAccounting_Click(object sender, EventArgs e)
        {
            // READ ALL IN THIS CYCLE
            //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            if (Mpa.ReplCycleNo > 0)
            {
                string WSelectionCriteria = "WHERE AtmNo ='" + Mpa.TerminalId + "' AND ReplCycle =" + Mpa.ReplCycleNo
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
            else
            {
                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                Aoc.ClearTableTxnsTableFromAction();

                string WSelectionCriteria = "WHERE UniqueKey =" + Mpa.UniqueRecordId + " AND UniqueKeyOrigin = 'Master_Pool' ";

                Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

                int I = 0;

                while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                    Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                    if (Aoc.Is_GL_Action == true)
                    {

                        int WMode2 = 1; // DO NOT Create transaction in pool 
                        string WCallerProcess = "Reconciliation";
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
}

// End Author
//*****************************
//if (DisputeAuthorisation == true)
//{
//    Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);
//    if (Ap.RecordFound == true)
//    {

//        WAuthorSeqNumber = Ap.SeqNumber;
//        // Exception to the above statement 
//        // This condition occurs only if authorization record has been deleted from requestor
//        // Therefore we make zero not to show authorization details 
//        if (Dt.ClosedDispute == false & Ap.OpenRecord == false)
//        {
//            WAuthorSeqNumber = 0;
//        }

//        labelDtAuthRequest.Text = "Date of Request : " + Ap.DateOriginated.ToString();

//        if (Ap.Stage == 1) StageDescr = "Authoriser Not Available yet.";
//        if (Ap.Stage == 2) StageDescr = "Authoriser got the message.";
//        if (Ap.Stage == 3) StageDescr = "Authoriser took action";
//        if (Ap.Stage == 4 & Ap.AuthDecision == "YES") StageDescr = "Authorization accepted. Ready for updating";
//        if (Ap.Stage == 4 & Ap.AuthDecision == "NO") StageDescr = "Authorization REJECTED.";
//        if (Ap.Stage == 5) StageDescr = "Authorisation process is completed";

//        labelAuthStatus.Text = "Current Status : " + StageDescr;

//        Us.ReadUsersRecord(Ap.Requestor);
//        labelRequestor.Text = "Requestor : " + Us.UserName;

//        Us.ReadUsersRecord(Ap.Authoriser);
//        labelAuthoriser.Text = "Authoriser : " + Us.UserName;

//        textBoxComment.Text = Ap.AuthComment;
//    }
//    else
//    {
//        WAuthorSeqNumber = 0;
//    }

//    //     if (WAuthorSeqNumber>0) Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);


//    //******************************************
//    // If Comming for Authorization only  or to view 
//    //*****************************************
//    if (WSource == 1 & WAuthorSeqNumber == 0)
//    {
//        ButtonFinish.Hide();

//        if (Dt.ClosedDispute == true)
//        {
//            ButtonFinish.Show();
//            buttonAuthor.Hide();
//        }
//    }

//    if (WSource == 1 & WAuthorSeqNumber > 0 || WSource == 2 & Ap.Stage == 1 & WSignedId == Ap.Requestor || WSource == 11
//           || WSource == 2 & Ap.Stage == 2 & WSignedId == Ap.Requestor || WSource == 11
//           || WSource == 2 & Ap.Stage == 3 & WSignedId == Ap.Authoriser)
//    {
//        // THERE IS AUTHORIZATION RECORD
//        labelAuthHeading.Show();
//        panelAuthor.Show();
//        panel2.Enabled = false;
//        textBoxComment.ReadOnly = true;
//        //        panel7.Enabled = false;
//        buttonRefresh.Show();
//        buttonAuthorise.Hide();
//        buttonReject.Hide();
//        buttonAuthor.Hide();
//        ButtonFinish.Hide();


//        textBoxMsgBoard.Text = "View Information";

//    }

//    //******************************************
//    // Show Authorizer to approve or not 
//    //*****************************************
//    if (WSource == 2 & Ap.Stage == 2 & WSignedId == Ap.Authoriser)
//    {
//        labelAuthHeading.Show();
//        panelAuthor.Show();
//        panel2.Enabled = false;
//        buttonRefresh.Hide();
//        buttonAuthor.Hide();
//        ButtonFinish.Hide();

//        textBoxMsgBoard.Text = "Please take authorization/reject action";

//    }
//    //*********************************************

//    //******************************************
//    // If Comming for Authorization only  
//    //*****************************************
//    if ((WSource == 2 & Ap.Stage == 4 & WSignedId == Ap.Requestor)
//        || (WSource == 1 & Ap.Stage == 4 & WSignedId == Ap.Requestor))
//    {
//        Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);
//        if (Ap.AuthDecision == "YES")
//        {
//            //Update 

//            panel2.Enabled = false;

//            labelAuthHeading.Show();
//            buttonRefresh.Hide();
//            buttonAuthorise.Hide();
//            buttonReject.Hide();

//            ButtonFinish.Show();
//            buttonAuthor.Hide();

//            panelAuthor.Show();
//            textBoxComment.ReadOnly = true;

//            textBoxMsgBoard.Text = "Action is Authorised. You can now update";

//        }
//        if (Ap.AuthDecision == "NO")
//        {
//            // Not allowed to Update 
//            panel2.Enabled = true;
//            panel6.Enabled = false;

//            labelAuthHeading.Show();
//            panelAuthor.Show();
//            textBoxComment.ReadOnly = true;
//            buttonAuthorise.Hide();
//            buttonReject.Hide();

//            buttonAuthor.Show();
//            ButtonFinish.Hide();

//            // Close Authorisation record 
//            //
//            Ap.Stage = 5;
//            Ap.OpenRecord = false;

//            Ap.UpdateAuthorisationRecord(Ap.SeqNumber);


//            textBoxMsgBoard.Text = "Action is rejected. You can change action and try again if you want.  ";
////        }

//    }
//    //*********************************************
//}
//else
//{
//    buttonRefresh.Hide();
//    buttonAuthor.Hide();
//    ButtonFinish.Show();

//    labelAuthHeading.Hide();
//    panelAuthor.Hide();
//}
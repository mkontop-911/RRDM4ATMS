using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form112 : Form
    {
        Form109 NForm109;
        Form110 NForm110;
        Form51 NForm51;
        //Form71 NForm71;
        Form271 NForm271;
        // Form276_NBG NForm276_NBG;
        SWD_Form271 NSWD_Form271;
        Form281 NForm281;
        Form201bITMX NForm201bITMX;
        Form201bITMXFEES NForm201bITMXFEES;
        Form52c NForm52c;
        Form291NVConfirmed NForm291NVConfirmed;

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
        RRDMDisputesTableClassITMX DiITMX = new RRDMDisputesTableClassITMX();
        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
        RRDMDisputeTransactionsClassITMX DtITMX = new RRDMDisputeTransactionsClassITMX();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        int WRow;

        //string WGridFilter; 
        string WSecLevel;

        string W_Application;

        bool TwoEntries;

        string WOrigin;
        int WTranNo;

        bool WTransfered;
        string WReasonOfTransfer;

        int WReplCycle;

        int WSeqNumber;
        int WStage;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WFunction;
        string WAtmNo;
        int WSesNo;
        int WDisputeNo;
        int WDisputeTranNo;
        string WRMCateg;
        int WRMCycle;

        public Form112(string InSignedId, int InSignRecordNo, string InOperator, string InFunction,
            string InAtmNo, int InSesNo, int InDisputeNo, int InDisputeTranNo, string InRMCateg, int InRMCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // Normal and  History 
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WDisputeNo = InDisputeNo;
            WDisputeTranNo = InDisputeTranNo;
            WRMCateg = InRMCateg;
            WRMCycle = InRMCycle;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            WRow = 0;

            buttonDelete.Hide();
        }
        // LOAD
        private void Form112_Load(object sender, EventArgs e)
        {
            WOrigin = "";
            int WMode;
            if (WFunction == "Normal")
            {
                WMode = 11;
                //WGridFilter = " (Operator ='" + WOperator + "' AND Requestor ='" + WSignedId + "' AND OpenRecord = 1) "
                //        + "  OR  (Operator ='" + WOperator + "' AND Authoriser ='" + WSignedId + "' AND OpenRecord = 1) ";
                Ap.ReadAuthorizationsDataTable(WOperator, WSignedId, "", 0, "", 0, 0, 0, WMode);
            }
            if (WFunction == "History")
            {
                if (WAtmNo != "")
                {
                    WMode = 12;
                    //WGridFilter = " Operator ='" + WOperator + "' AND AtmNo ='" + WAtmNo
                    //            + "' AND ReplCycle =" + WSesNo;

                    Ap.ReadAuthorizationsDataTable(WOperator, WSignedId, WAtmNo, WSesNo, "", 0, 0, 0, WMode);
                }

                if (WAtmNo == "") // We have matching Category 
                {
                    WMode = 13;
                    //WGridFilter = " Operator ='" + WOperator + "' AND RMCategory ='" + WRMCateg
                    //            + "' AND RMCycle =" + WRMCycle ;
                    Ap.ReadAuthorizationsDataTable(WOperator, WSignedId, "", WSesNo, WRMCateg, WRMCycle, 0, 0, WMode);

                }

                if (WDisputeNo > 0)
                {
                    WMode = 14;

                    Ap.ReadAuthorizationsDataTable(WOperator, WSignedId, "", WSesNo, WRMCateg, WRMCycle,
                                                                WDisputeNo, WDisputeTranNo, WMode);
                }

            }
            //
            //   RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;

                if (W_Application == "e_MOBILE")
                {
                    if (Usi.WFieldNumeric11 == 11)
                    {
                        W_Application = "ETISALAT";
                    }
                    if (Usi.WFieldNumeric11 == 12)
                    {
                        W_Application = "QAHERA";
                    }
                    if (Usi.WFieldNumeric11 == 13)
                    {
                        W_Application = "IPN";
                    }
                    labelStep1.Text = "Controller's Menu-Mobile_" + W_Application;
                }
                else
                {
                    W_Application = "ATMs";

                }
            }

            ShowGridAuthorisations();
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            buttonDelete.Hide();

            TwoEntries = false;

            WSeqNumber = (int)rowSelected.Cells[0].Value;

            Ap.ReadAuthorizationSpecific(WSeqNumber);

            string AuthOperator = Ap.Operator;
            WOrigin = Ap.Origin;
            WTranNo = Ap.TranNo;
            WAtmNo = Ap.AtmNo;
            WRMCateg = Ap.RMCategory;
            WRMCycle = Ap.RMCycle;
            WReplCycle = Ap.ReplCycle;
            WTransfered = Ap.Transfered;
            WReasonOfTransfer = Ap.ReasonOfTransfer;

            if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
            {
                // Authoriser
                labelHeader.Text = "Authorisation Records for_" + "Authoriser";
            }

            if (WSignedId.ToUpper() == Ap.Requestor.ToUpper())
            {

                // Requestor
                labelHeader.Text = "Authorisation Records for_" + "Requestor";

            }

            if (WOrigin == "Dispute Action")
            {
                // Customer Name 
                if (WOperator == "ITMX")
                {
                    DiITMX.ReadDispute(Ap.DisputeNumber);
                    LabelDescr.Text = "Dispute Authorisaton for customer : " + DiITMX.CustName;
                }
                else
                {
                    Di.ReadDispute(Ap.DisputeNumber);
                    LabelDescr.Text = "Dispute Authorisaton for customer : " + Di.CustName;

                    Ap.ReadAuthorizationForDisputeAndTransaction(Ap.DisputeNumber, Ap.DisputeTransaction);
                    if (Ap.RecordFound == true & Ap.WCounter > 1)
                    {
                        // Problem Here
                        TwoEntries = true;

                        buttonDelete.Show();

                        MessageBox.Show("There are two entries in authorisation records" + Environment.NewLine
                            + "Use the Delete Button to delete both and restart the work." + Environment.NewLine
                            );
                    }
                    else
                    {
                        TwoEntries = false;
                        buttonDelete.Hide();
                    }

                }

            }


            Us.ReadUsersRecord(Ap.Requestor);
            label_Requestor.Text = "Requestor : " + Us.UserName;

            Us.ReadUsersRecord(Ap.Authoriser);
            label_Authoriser.Text = "Authoriser : " + Us.UserName;

            WStage = Ap.Stage; // If Stage = 2 then this is for authorizer if 4 then this is for the requestor 
            //LabelStage.Text = "Stage : " + WStage.ToString();


            // Change on 11/01/2024 
            if (WSignedId.ToUpper() == Ap.Requestor.ToUpper())
            {
                // IS THE REQUESTOR
                if (WStage == 3)
                {
                    // update stage to 4 
                    WStage = 4;
                    Ap.UpdateStageBasedOnSeqNumber(WSeqNumber, WStage);
                }

            }

            if (WSignedId.ToUpper() == Ap.Authoriser.ToUpper())
            {
                // IS THE AUTHORISER
                if (WStage == 1)
                {
                    // update stage to 2 
                    WStage = 2;
                    Ap.UpdateStageBasedOnSeqNumber(WSeqNumber, WStage);
                }
            }
            // END OF CORRECTION 11/01/2024 

            label_DateCreated.Text = "Date Created : " + Ap.DateOriginated.ToString();

            if (Ap.AuthComment != "")
            {
                textBoxCommnet.Text = Ap.AuthComment;
                textBoxCommnet.Show();
                label5.Show();
                label6.Show();
            }
            else
            {
                textBoxCommnet.Hide();
                label5.Hide();
                label6.Hide();
            }

            if (WStage == 1 & WSignedId.ToUpper() == Ap.Requestor.ToUpper())
            {
                textBoxMessage.Text = "Authoriser not available yet.";
                // You can delete if you want 
                buttonDelete.Show();
                buttonNext.Text = "View";
                buttonTransfer.Show();
                buttonFinish.Show();
            }

            if (WStage == 2 & WSignedId.ToUpper() == Ap.Requestor.ToUpper())
            {
                textBoxMessage.Text = "Authoriser didn't authorise yet.";
                buttonDelete.Show();
                buttonNext.Show();
                buttonNext.Text = "View";
                buttonTransfer.Show();
                buttonFinish.Show();
            }

            if (WStage == 2 & WSignedId.ToUpper() == Ap.Authoriser.ToUpper())
            {
                textBoxMessage.Text = "Go to Authorise";
                buttonNext.Show();
                buttonNext.Text = "Go to Authorise";
                buttonTransfer.Hide();
                buttonDelete.Hide();
                buttonFinish.Hide();
            }
            if (WStage == 3 & WSignedId.ToUpper() == Ap.Authoriser.ToUpper())
            {
                textBoxMessage.Text = "Requestor is not available yet.";
                buttonNext.Show();
                buttonNext.Text = "View";
                buttonTransfer.Show();
                buttonTransfer.Text = "Finish";
                buttonDelete.Hide();
                buttonFinish.Hide();
            }
            if (WStage == 4 & WSignedId.ToUpper() == Ap.Authoriser.ToUpper())
            {

                if (Ap.AuthDecision == "NO")
                {
                    textBoxMessage.Text = "Requestor didn't take corrective action yet.";
                }

                if (Ap.AuthDecision == "YES")
                {
                    textBoxMessage.Text = "Requestor didn't finish process yet.";
                }

                buttonNext.Show();
                buttonNext.Text = "View";
                buttonTransfer.Show();
                buttonTransfer.Text = "Finish";
                // buttonDelete.Hide();
                buttonFinish.Hide();
            }
            if ((WStage == 3 || WStage == 4) & WSignedId.ToUpper() == Ap.Requestor.ToUpper())
            {
                if (Ap.AuthDecision == "NO")
                {
                    textBoxMessage.Text = "Rejected. Correct and resubmit.";
                }

                if (Ap.AuthDecision == "YES")
                {
                    textBoxMessage.Text = "Authorisation Accepted. Go to finish process! ";
                }

                buttonNext.Show();
                buttonNext.Text = "Next";
                buttonTransfer.Hide();
                buttonDelete.Show();
            }

            if (WStage == 5)
            {
                textBoxMessage.Text = "Closed Record!";
                buttonNext.Hide();
                buttonTransfer.Hide();
                buttonDelete.Hide();
                buttonTransfer.Hide();

                buttonFinish.Show();
            }

            if (WFunction == "History")
            {
                labelHeader.Text = "History of Authorisations ";
                buttonNext.Hide();
                buttonTransfer.Hide();
                buttonDelete.Hide();
                buttonTransfer.Hide();

                buttonFinish.Show();
            }

            if (WStage == 1) LabelStage.Text = "Stage : " + WStage.ToString() + " Authoriser Not Available yet.";
            if (WStage == 2) LabelStage.Text = "Stage : " + WStage.ToString() + " Authoriser got the message.";
            if (WStage == 3) LabelStage.Text = "Stage : " + WStage.ToString() + " Authoriser took action";
            if ((WStage == 3 || WStage == 4) & Ap.AuthDecision == "YES")
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation accepted. Ready for updating";
            }
            if (WStage == 5 & Ap.AuthDecision == "YES")
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation accepted. Updating done";
            }
            if ((WStage == 3 || WStage == 4) & Ap.AuthDecision == "NO")
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation REJECTED. ";
                Color Red = Color.Red;
                LabelStage.ForeColor = Red;
            }
            else
            {
                Color Black = Color.Black;
                LabelStage.ForeColor = Black;
            }
            if (WStage == 5 & Ap.AuthDecision == "NO")
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation REJECTED. Record closed ";
                Color Red = Color.Red;
                LabelStage.ForeColor = Red;
            }
            else
            {
                Color Black = Color.Black;
                LabelStage.ForeColor = Black;
            }

            if (WTransfered == true)
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation was transfered! Due to: " + WReasonOfTransfer;
                textBoxMessage.Text = "Authorisation was transfered";
            }

            //if (WSecLevel == 4)
            //{
            //    buttonDelete.Show();
            //}

            // ONE

            if (WOrigin == "Replenishment")
            {
                LabelDescr.Text = "Replenishment Authorisaton for Atm No : " + WAtmNo + " and Repl Cycle : " + WReplCycle.ToString();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WReplCycle, "Replenishment");
                if (Ap.RecordFound == true & Ap.WCounter > 1)
                {
                    // Problem Here
                    TwoEntries = true;
                    buttonDelete.Show();

                    MessageBox.Show("There are two entries in authorisation records" + Environment.NewLine
                        + "Use the Delete Button to delete both and restart the work." + Environment.NewLine
                        );
                }
                else
                {
                    TwoEntries = false;
                    buttonDelete.Hide();
                }

            }

            // TWO 

            if (WOrigin == "ReconciliationCat")
            {
                if (WOperator == "ITMX")
                {
                    //RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                    //Rcs.ReadReconcCategoriesByCategoryIdForName(WAtmNo);
                    LabelDescr.Text = "Reconciliation Authorisaton for Category Id : " + WRMCateg + " and Cycle : " + WRMCycle.ToString();
                }
                else
                {
                    LabelDescr.Text = "Reconciliation Authorisaton for Category No : " + WRMCateg + " and Matching Session : " + WRMCycle.ToString();

                    Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WRMCateg, WRMCycle, "ReconciliationCat");

                    if (Ap.RecordFound == true & Ap.WCounter > 1)
                    {
                        TwoEntries = true;
                        buttonDelete.Show();

                        MessageBox.Show("There are two entries in authorisation records" + Environment.NewLine
                            + "Use the Delete Button to delete both and restart the work." + Environment.NewLine
                            );
                    }
                    else
                    {
                        TwoEntries = false;
                        buttonDelete.Hide();
                    }

                }

            }


            if (WOrigin == "Reconciliation")
            {
                LabelDescr.Text = "Reconciliation Authorisaton for Atm No : " + WAtmNo + " and Repl Cycle : " + WReplCycle.ToString();
            }

            if (WOrigin == "ReplOrders")
            {

                LabelDescr.Text = "Authorisation for CIT: " + WRMCateg + " And Orders Cycle No: " + WRMCycle.ToString();
            }
            if (WOrigin == "LoadingExcel")
            {

                LabelDescr.Text = "Authorisation for CIT: " + WRMCateg + " And Loading Excel Cycle No: " + WRMCycle.ToString();
            }


            if (WOrigin == "Reversals")
            {
                LabelDescr.Text = "Reversals for Id : " + WRMCycle;
            }



            if (WOrigin == "SettlementAuth")
            {
                if (WOperator == "ITMX")
                {
                    //RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                    //Rcs.ReadReconcCategoriesByCategoryIdForName(WAtmNo);
                    LabelDescr.Text = "Settlement Authorisaton for Settlement Cycle : " + WRMCycle.ToString();
                }

            }
            if (WOrigin == "SettlementFeesAuth")
            {
                if (WOperator == "ITMX")
                {
                    //RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                    //Rcs.ReadReconcCategoriesByCategoryIdForName(WAtmNo);
                    LabelDescr.Text = "Fees Settlement Authorisaton for Settlement Cycle : " + WRMCycle.ToString();
                }
            }

            if (WOrigin == "ForceMatchingCat")
            {
                LabelDescr.Text = "Force Matching Authorisation for Category No : " + WAtmNo + " and Matching Session : " + WReplCycle.ToString();
            }

            if (WOrigin == "ReconciliationBulk")
            {
                LabelDescr.Text = "Bulk Reconciliation Authorisaton For ATMs";
            }

            if (WOrigin == "ConfirmNostroMatching")
            {
                LabelDescr.Text = "Confirm Nostro Matching";
            }

        }

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridAuthorisations()
        {
            dataGridView1.DataSource = Ap.ATMsAuthorisationsDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                this.Dispose();

                if (WFunction == "History")
                {
                    MessageBox.Show("No History Data!");
                }
                else
                {
                    MessageBox.Show("No other authorisations to manage!");
                }

                return;
            }
            else
            {
                textBoxNumber.Text = dataGridView1.Rows.Count.ToString();
            }

            //ATMsAuthorisationsDataTable.Columns.Add("SeqNumber", typeof(int));
            //ATMsAuthorisationsDataTable.Columns.Add("Requestor", typeof(string));
            //ATMsAuthorisationsDataTable.Columns.Add("Authoriser", typeof(string));
            //ATMsAuthorisationsDataTable.Columns.Add("Stage", typeof(int));
            //ATMsAuthorisationsDataTable.Columns.Add("Origin", typeof(string));

            dataGridView1.Columns[0].Width = 60; // SeqNumber
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 120; //Requestor
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 120; //Authoriser
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 50; //Stage
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 350; //Origin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 150; //Date
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            int CountSize = Ap.ATMsAuthorisationsDataTable.Rows.Count;

            if (WRow == 0 & WRow < CountSize)
            {
                // Valid Index
                if (WStage != 4 & WStage != 5)
                {
                    dataGridView1.Rows[WRow].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));
                }
            }

        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            //
            // SWITCH WOrigin
            //
            if (TwoEntries == true)
            {
                MessageBox.Show("Delete the two etries before you proceed.");
                return;
            }

            switch (WOrigin)
            {
                case "Replenishment":
                    {
                        if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.StepLevel = 0;
                            Usi.ProcessStatus = 0;
                            Usi.ProcessNo = 55; //  Authoriser 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.StepLevel = 0;
                            Usi.ProcessStatus = 0;
                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WOperator == "AUDBEGCA")
                        {
                            // AUDI
                            Form51_FAB_Type NForm51_AUDI_TYPE;
                            NForm51_AUDI_TYPE = new Form51_FAB_Type(WSignedId, WSignRecordNo, WOperator, WAtmNo, WReplCycle);
                            NForm51_AUDI_TYPE.FormClosed += NForm51_AUDI_TYPE_FormClosed;
                            NForm51_AUDI_TYPE.ShowDialog();
                        }
                        else
                        {
                            //Ta.Stats1.NoOfCheques = 1
                            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);

                            if (Ta.Stats1.NoOfCheques == 1)
                            {

                                // CALL THE SAME If Recycle or not 
                                bool IsFromExcel = false;
                                Form51_Repl_For_IST NForm51_Repl_For_IST;
                                NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WReplCycle, IsFromExcel);
                                NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
                                NForm51_Repl_For_IST.ShowDialog();
                            }
                            else
                            {
                                //
                                // Find out if ATM is Recycling 
                                //
                                RRDMGasParameters Gp = new RRDMGasParameters();
                                bool IsRecycle = false;

                                string ParId2 = "948";
                                string OccurId2 = "1"; // 
                                                       //RRDMGasParameters Gp = new RRDMGasParameters(); 
                                Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
                                if (Gp.RecordFound & Gp.OccuranceNm == "YES")
                                {
                                    RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();
                                    SM.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WReplCycle);
                                    if (SM.RecordFound == true)
                                    {
                                        // Check if Reccyle 
                                        if (SM.is_recycle == "Y")
                                        {
                                            IsRecycle = true;
                                        }
                                    }
                                }

                                if (IsRecycle == true)
                                {

                                    Form51_Recycle NForm51_Recycle;
                                    NForm51_Recycle = new Form51_Recycle(WSignedId, WSignRecordNo, WOperator, WAtmNo, WReplCycle);
                                    NForm51_Recycle.FormClosed += NForm51_FormClosed;
                                    NForm51_Recycle.ShowDialog();

                                }
                                else
                                {

                                    // Current Bank De Caire Type 
                                    NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WReplCycle);
                                    NForm51.FormClosed += NForm51_FormClosed;
                                    NForm51.ShowDialog();

                                }

                            }

                        }



                        break;
                    }

                case "ReconciliationCat":
                    {
                        if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);

                            Usi.StepLevel = 0;
                            Usi.ProcessStatus = 0;

                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);

                            Usi.StepLevel = 0;
                            Usi.ProcessStatus = 0;

                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WOperator == "ITMX")
                        {
                            NForm281 = new Form281(WSignedId, WSignRecordNo, WOperator, WRMCateg, WRMCycle);
                            NForm281.FormClosed += NForm281_FormClosed;
                            NForm281.ShowDialog();

                        }
                        else
                        {

                            if (W_Application == "ATMs")
                            {
                                NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WRMCateg, WRMCycle);
                                NForm271.FormClosed += NForm271_FormClosed;
                                NForm271.ShowDialog();
                            }
                            else
                            {
                                Form277_MOBILE NForm277_MOBILE;
                                NForm277_MOBILE = new Form277_MOBILE(WSignedId, WSignRecordNo, WOperator, WRMCateg, WRMCycle, W_Application);
                                NForm277_MOBILE.FormClosed += NForm271_FormClosed;
                                NForm277_MOBILE.ShowDialog();
                            }

                        }
                        break;

                    }

                case "Dispute Action":
                    {

                        if (WSignedId.ToUpper() == Ap.Authoriser.ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);

                            Usi.StepLevel = 0;
                            Usi.ProcessStatus = 0;

                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);

                            Usi.StepLevel = 0;
                            Usi.ProcessStatus = 0;

                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WOperator == "ITMX")
                        {
                            Form109ITMX NForm109ITMX;

                            int WSource = 2; // Comes from authorisation process 

                            NForm109ITMX = new Form109ITMX(WSignedId, WSignRecordNo, WOperator, Ap.DisputeNumber, Ap.DisputeTransaction, Ap.TranNo, WSource);
                            NForm109ITMX.FormClosed += NForm109_FormClosed;
                            NForm109ITMX.ShowDialog();

                        }
                        else
                        {
                            int WSource = 2; // Comes from authorisation process 

                            NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator, Ap.DisputeNumber, Ap.DisputeTransaction, Ap.TranNo, WSource);
                            NForm109.FormClosed += NForm109_FormClosed;
                            NForm109.ShowDialog();
                        }


                        break;

                    }

                case "LoadingExcel":
                    {

                        if (WSignedId.ToUpper() == Ap.Authoriser.ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        //--Origin = 'LoadingExcel'
                        //--RMCategory = '2000'
                        //--RMCycle = 1
                        //
                        //--Assign the relevant information 
                        //
                        WRMCateg = Ap.RMCategory; // Here we keep the CITId 
                        WRMCycle = Ap.RMCycle;
                        int WLoadingCycle = WRMCycle; // This is kept in this field 
                        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

                        Cec.ReadExcelLoadCyclesBySeqNo(WLoadingCycle);

                        int WWRMCycle = Cec.RMCycle;

                        Form276_AUDI_FirstStep NForm276_AUDI_FirstStep;

                        NForm276_AUDI_FirstStep = new Form276_AUDI_FirstStep(WSignedId, WSignRecordNo, WOperator, WRMCateg, WLoadingCycle, WWRMCycle);
                        NForm276_AUDI_FirstStep.FormClosed += NForm271_FormClosed;
                        NForm276_AUDI_FirstStep.ShowDialog();


                        break;

                    }
                case "ReplOrders":
                    {
                        if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.Trim().ToUpper())
                        {
                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        // "LoadingExcel"

                        Form273_AUDI AuthorForm = new Form273_AUDI(WSignedId, WSignRecordNo, WOperator, WRMCateg, WRMCycle, "");
                        AuthorForm.FormClosed += NForm201bITMX_FormClosed;

                        AuthorForm.ShowDialog();

                        break;
                    }
                case "SettlementAuth":
                    {

                        if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WOperator == "ITMX")
                        {
                            NForm201bITMX = new Form201bITMX(WSignedId, WSignRecordNo, WOperator);
                            NForm201bITMX.FormClosed += NForm201bITMX_FormClosed;
                            NForm201bITMX.ShowDialog();
                        }

                        break;

                    }

                case "SettlementFeesAuth":
                    {
                        if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WOperator == "ITMX")
                        {
                            NForm201bITMXFEES = new Form201bITMXFEES(WSignedId, WSignRecordNo, WOperator);
                            NForm201bITMXFEES.FormClosed += NForm201bITMXFEES_FormClosed;
                            NForm201bITMXFEES.ShowDialog();
                        }

                        break;

                    }

                case "Reversals":
                    {
                        if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        Form78Rev NForm78Rev;

                        NForm78Rev = new Form78Rev(WSignedId, WSignRecordNo, WOperator, "", 0, 0, 0);
                        NForm78Rev.FormClosed += NForm19c_FormClosed;
                        NForm78Rev.ShowDialog();

                        break;

                    }
                case "ReconciliationBulk":
                    {
                        if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        int WBulkMode = 1; // For User 
                        NForm52c = new Form52c(Ap.Requestor, WSignRecordNo, WOperator, WRMCateg, WRMCycle, false);
                        NForm52c.FormClosed += NForm52c_FormClosed;
                        NForm52c.ShowDialog();

                        break;

                    }
                case "ConfirmNostroMatching":
                    {
                        if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }


                        NForm291NVConfirmed = new Form291NVConfirmed(Ap.Requestor, WSignRecordNo, WOperator, "NostroReconciliation", WRMCateg, WRMCycle);
                        //(string InSignedId, int SignRecordNo, string InOperator,
                        //                  string InSubSystem, string InReconcCategoryId, int InReconcCycleNo)
                        NForm291NVConfirmed.FormClosed += NForm291NVConfirmed_FormClosed;
                        NForm291NVConfirmed.ShowDialog();

                        break;

                    }
                case "SWDSession":
                    {
                        if (WSignedId.ToUpper() == Ap.Authoriser.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 55; // Authoriser  
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        if (WSignedId.ToUpper() == Ap.Requestor.Trim().ToUpper())
                        {

                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            Usi.ProcessNo = 56; // Requestor 
                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                        }

                        NSWD_Form271 = new SWD_Form271(WSignedId, WSignRecordNo, WOperator, WRMCateg, WRMCycle);
                        NSWD_Form271.FormClosed += NSWD_Form271_FormClosed;
                        NSWD_Form271.ShowDialog();

                        break;

                    }

                default:
                    {

                        break;
                    }
            }

        }

        private void NForm281_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        private void NForm201bITMX_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        private void NForm201bITMXFEES_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        void NForm19c_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        void NForm271_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WStage != 4)
                WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        void NSWD_Form271_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WStage != 4 & WStage != 5)
                WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        void NForm52c_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }
        void NForm291NVConfirmed_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        void NForm71_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WStage != 4 & WStage != 5)
                WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        void NForm51_AUDI_TYPE_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WStage != 4 & WStage != 5)
                WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        void NForm109_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WStage != 4 & WStage != 5)
                WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        private void buttonTransfer_Click(object sender, EventArgs e)
        {
            if (buttonTransfer.Text == "Finish")
            {
                this.Close();
                return;
            }
            int AuthorSeqNumber = WSeqNumber; // This is used >0 when calling from Authorization management 
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WAtmNo, WReplCycle, AuthorSeqNumber,
                  0, WRMCateg, WRMCycle,
            "Transfer");
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();
        }

        void NForm110_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
            Form112_Load(this, new EventArgs());
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Warning: By deleting this record you have to start process right from the start." + Environment.NewLine
                    + " Do you want to delete this authorisation record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
            {

                if (WOrigin == "Dispute Action")
                {

                    // Reverse updating in Dispute 
                    Dt.ReadDisputeTran(Ap.DisputeTransaction);

                    //***********************
                    // UPDATE DISPUTE RECORD
                    Dt.ChooseAuthor = false;
                    Dt.PendingAuthorization = false;
                    Dt.AuthorOriginator = "";
                    Dt.AuthorKey = 0;
                    Dt.Authoriser = "";
                    Dt.DisputeActionId = 0;
                    Dt.ReasonForAction = 0;
                    Dt.ActionComment = "";

                    //Dt.Authorised = true;
                    //Dt.RejectedFromAuth = false; 
                    //Dt.AuthoriserComment = "" ; 
                    Dt.UpdateDisputeTranRecord(Ap.DisputeTransaction);
                    if (Ap.DisputeNumber > 0 & Ap.DisputeTransaction > 0)
                    {
                        Ap.DeleteAuthorisationRecord_Disp(Ap.DisputeNumber, Ap.DisputeTransaction);
                    }

                }

                // Ap.DeleteAuthorisationRecord(WSeqNumber);

                if (WOrigin == "Replenishment")
                {

                    Usi.ReadSignedActivityByKey(WSignRecordNo); // 
                    Usi.ProcessNo = 1;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                    if (WReplCycle > 0)
                    {
                        Ap.DeleteAuthorisationRecord_Repl(WAtmNo, WReplCycle);
                    }
                }

                if (WOrigin == "ReconciliationCat"
                    || WOrigin == "ReplOrders" || WOrigin == "LoadingExcel"
                    || WOrigin == "Reversals")
                {

                    Usi.ReadSignedActivityByKey(WSignRecordNo); // 
                    Usi.ProcessNo = 2;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    //Ap.DeleteAuthorisationRecord(WSeqNumber);
                    if (WRMCycle > 0)
                    {
                        Ap.DeleteAuthorisationRecord_Cat(WRMCateg, WRMCycle);
                    }

                }

                // Load to refresh
                // 
                // WRow = dataGridView1.SelectedRows[0].Index;
                Form112_Load(this, new EventArgs());
            }
            else
            {
            }
        }
    }
}

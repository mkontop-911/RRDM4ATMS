using System;
using System.Windows.Forms;
using RRDM4ATMs;


namespace RRDM4ATMsWin
{
    public partial class Form110 : Form
    {
        //
        // AUTHORISERS 
        // 
        Form111 NForm111;

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
        RRDMDisputeTransactionsClassITMX DtITMX = new RRDMDisputeTransactionsClassITMX();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUserVsAuthorizers UvsA = new RRDMUserVsAuthorizers();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMGasParameters Gp = new RRDMGasParameters();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool WithDate;
        int WAction;
        int I;
        int WDifStatus;
        bool IsSolo; 

        //string Gridfilter; 

        string WAuthoriser;
        string WAuthName;
        int WSeqNumber;

        string WRMCategory;
        int WRMCycle;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WOrigin;
        int WMaskRecordId;
        string WAtmNo;
        int WReplCycle;
        int WAuthorSeqNumber;
        //string WRMCateg;
        //int WRMCycle; 
        string WFunction;

        public Form110(string InSignedId, int SignRecordNo, string InOperator, string InOrigin,
                      int InTraNo, string InAtmNo, int InReplCycle,
                      int InAuthorSeqNumber, int InDifStatus, string InRMCategory, int InRMCycle, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WOrigin = InOrigin;

            WMaskRecordId = InTraNo;

            WAtmNo = InAtmNo;
            WReplCycle = InReplCycle;

            WAuthorSeqNumber = InAuthorSeqNumber; // If this is > 0 means it is coming from Autorization management
                                                  // aiming at transfering autorization to another authorizer 
                                                  //WRMCateg = InRMCateg;
                                                  //WRMCateg = InRMCateg;
            WDifStatus = InDifStatus;

            WRMCategory = InRMCategory;

            WRMCycle = InRMCycle;

            WFunction = InFunction; // Values = Normal, Transfer 


            InitializeComponent();


            string ParId = "264";
            string OccurId = "1";
            //TEST

            //WOperator = "ETHNCY2N"; 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            //Gp.OccuranceNm = "YES";
            // AdDomainName

            if (Gp.OccuranceNm == "YES") // Active directory needed
            {
                buttonLocal.Enabled = false;
            }

            //Usi.ReadSignedActivityByKey(WSignRecordNo);
            //WDifStatus = Usi.ReconcDifferenceStatus;

            //WRMCategory = Usi.WFieldChar1;

            //WRMCycle = Usi.WFieldNumeric1;

            if (WFunction == "Normal")
            {
                textBoxMessage.Text = "Select Authoriser";
            }

            if (WFunction == "Transfer")
            {
                panel2.Show();

                buttonLocal.Hide();

                comboBox1.Items.Add("Select Reason"); // 
                comboBox1.Items.Add("Delayed Reply"); // 
                comboBox1.Items.Add("Away from Office"); // 
                comboBox1.Items.Add("Not right authoriser"); // 
                comboBox1.Items.Add("Turn To local"); // 

                comboBox1.Text = "Select Reason";
                textBoxMessage.Text = "Select Authoriser to tranfer";
            }
            else
            {
                panel2.Hide();
            }

        }
        // Load form 
        private void Form110_Load(object sender, EventArgs e)
        {
            //userVsAuthorizersBindingSource.Filter = "Operator = '" + WOperator + "' "
            //                                 + " AND UserId = '" + WSignedId + "' " + " AND OpenRecord = 1 " ;

            string Filter = "Operator = '" + WOperator + "' "
                                          + " AND UserId = '" + WSignedId + "' "
                                          + " AND OpenRecord = 1 ";

            UvsA.ReadUserVsAuthorizersFillDataTable2(Filter);

            ShowGridUsersVsAuthor();

            //this.userVsAuthorizersTableAdapter.Fill(this.aTMSDataSet36.UserVsAuthorizers);
        }
        // Row is entered 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WAuthoriser = (string)rowSelected.Cells[0].Value;

            Us.ReadUsersRecord(WAuthoriser);

            WAuthName = Us.UserName;

        }
        // Local Authorization 
        private void button1_Click(object sender, EventArgs e)
        {
            if (WOrigin == "ReconciliationBulk") // This function is not allowed for Bulk authorisation 
            {
                MessageBox.Show("This function is not allowed for Bulk authorisation");
                return;
            }

            if (WFunction == "Transfer") // IF TRANSFER THEN UPDATE REASON OF TRANSFER 
            {
                if (comboBox1.Text == "Select Reason")
                {
                    MessageBox.Show("Please enter reason of transfer");
                    return;
                }

                // Close/Update  old Autorization record 

                Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);

                Ap.Transfered = true;
                Ap.TransferedDate = DateTime.Now;
                Ap.OpenRecord = false;

                Ap.ReasonOfTransfer = comboBox1.Text;

                Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);

            }
            //******************
            // INSERT AUTHORISATION RECORD 
            //******************
            if (WOrigin == "Dispute Action")
            {
                if (WOperator == "ITMX")
                {
                    DtITMX.ReadDisputeTranByMaskRecordId(WMaskRecordId);
                    Ap.DisputeNumber = DtITMX.DisputeNumber;
                    Ap.DisputeTransaction = DtITMX.DispTranNo;
                }
                else
                {
                    Dt.ReadDisputeTranByUniqueRecordId(WMaskRecordId);
                    //if(Dt.RecordFound==false)
                    //{
                    //    // This comes from 193_SOLO
                    //    //IsSolo = true;
                    //    //Ap.DisputeNumber = WMaskRecordId;
                    //    //Ap.DisputeTransaction = WMaskRecordId;
                    //}
                    //else
                    //{
                        // Comes from real dispute 
                        Ap.DisputeNumber = Dt.DisputeNumber;
                        Ap.DisputeTransaction = Dt.DispTranNo;
                    //}
                    
                }
            }
            else
            {
                Ap.DisputeNumber = 0;
                Ap.DisputeTransaction = 0;
            }

            Ap.Requestor = WSignedId;
            Ap.Authoriser = WAuthoriser;
            Ap.Origin = WOrigin;

            Ap.TranNo = WMaskRecordId;
            Ap.AtmNo = WAtmNo;
            Ap.ReplCycle = WReplCycle;
            Ap.DateOriginated = DateTime.Now;
            Ap.Stage = 2;
            Ap.DifferenceStatus = WDifStatus;
            Ap.Operator = WOperator;

            Ap.RMCategory = WRMCategory;
            Ap.RMCycle = WRMCycle;

            WSeqNumber = Ap.InsertAuthorizationRecord();


            Usi.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
            Usi.ProcessNo = 56;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //*********************
            // UPDATE Dispute details 
            //*********************

            if (WOrigin == "Dispute Action" )
            {
                // If RECORD FOUND THEN COMES FROM REAL DISPUTE. 
                //***********************
                // UPDATE DISPUTE RECORD

                if (WOperator == "ITMX")
                {
                    DtITMX.ChooseAuthor = true;
                    DtITMX.PendingAuthorization = true;
                    DtITMX.AuthorOriginator = WSignedId;
                    DtITMX.AuthorKey = WSeqNumber;
                    DtITMX.Authoriser = WAuthoriser;

                    DtITMX.UpdateDisputeTranRecord(DtITMX.DispTranNo);
                }
                else
                {
                    Dt.ChooseAuthor = true;
                    Dt.PendingAuthorization = true;
                    Dt.AuthorOriginator = WSignedId;
                    Dt.AuthorKey = WSeqNumber;
                    Dt.Authoriser = WAuthoriser;

                    Dt.UpdateDisputeTranRecord(Dt.DispTranNo);
                }
            }

            //*********************

            //***********************


            NForm111 = new Form111(WSignedId, WSignRecordNo, WOperator, WAuthoriser, WAuthName, WSeqNumber);
            NForm111.FormClosed += NForm111_FormClosed;
            NForm111.ShowDialog();

        }

        void NForm111_FormClosed(object sender, FormClosedEventArgs e)
        {
            Ap.ReadAuthorizationSpecific(WSeqNumber);
            if (Ap.Stage == 2) // Return from local but job not authorised 
            {
                Ap.DeleteAuthorisationRecord(WSeqNumber);

                if (WOrigin == "Replenishment")
                {

                    Usi.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
                    Usi.ProcessNo = 1;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WOrigin == "Reconciliation"
                                   || WOrigin == "ReconciliationCat"
                                   || WOrigin == "Reversals"
                                   || WOrigin == "ReplOrders"
                                   || WOrigin == "LoadingExcel"
                                   || WOrigin == "ForceMatchingCat" || WOrigin == "SettlementAuth"
                                   || WOrigin == "SettlementFeesAuth" || WOrigin == "ConfirmNostroMatching") // No Bulk 
                {

                    Usi.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
                    Usi.ProcessNo = 2;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WOrigin == "SWDSession")
                {

                    Usi.ReadSignedActivityByKey(WSignRecordNo); // Requestor 
                    Usi.ProcessNo = 7;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

            }
            this.Dispose();
        }

        // REMOTE AUTHORIZATION 

        private void buttonRemote_Click(object sender, EventArgs e)
        {
            // Check if Already in authorization process
            // In case of rejection 
            if (WOrigin == "ReconciliationCat")
            {
                Ap.ReadAuthorizationRecordByRMCategoryAndRMCycle(WRMCategory, WRMCycle);
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
            }

            if (WOrigin == "Replenishment")
            {
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WReplCycle, "Replenishment");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if ((Ap.Stage == 4 || Ap.Stage == 3) & Ap.AuthDecision == "NO")
                    {
                        // MessageBox.Show("Authoriser said NO... The Ap.Stage =... " + Ap.Stage); 
                        Ap.Stage = 5;
                        Ap.OpenRecord = false;

                        Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                    }
                }
            }
            

            if (WOrigin == "Reversals")
            {
                MessageBox.Show("Remote authorisation is not allowed for Reversals." + Environment.NewLine
                                  + "Do a local Authorisation ");
                return;
            }

            if (WFunction == "Transfer") // IF TRANSFER THEN UPDATE REASON OF TRANSFER 
            {
                if (WOrigin == "ReconciliationBulk") // This function is not allowed for Bulk authorisation 
                {
                    MessageBox.Show("This function is not allowed for Bulk authorisation");
                }

                if (comboBox1.Text == "Select Reason")
                {
                    MessageBox.Show("Please enter reason of transfer");
                    return;
                }

                // Close/Update  old Autorization record 

                Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);

                Ap.RMCycle = WRMCycle;

                Ap.Transfered = true;
                Ap.TransferedDate = DateTime.Now;
                Ap.OpenRecord = false;

                Ap.ReasonOfTransfer = comboBox1.Text;

                Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);
            }

            //******************
            // INSERT AUTHORISATION RECORD 
            if (WOrigin == "Dispute Action")
            {

                if (WOperator == "ITMX")
                {
                    DtITMX.ReadDisputeTranByMaskRecordId(WMaskRecordId);
                    Ap.DisputeNumber = DtITMX.DisputeNumber;
                    Ap.DisputeTransaction = DtITMX.DispTranNo;
                }
                else
                {

                    Dt.ReadDisputeTranByUniqueRecordId(WMaskRecordId);
                    if (Dt.RecordFound == true)
                    {
                        // Comes From Dispute
                        Ap.DisputeNumber = Dt.DisputeNumber;
                        Ap.DisputeTransaction = Dt.DispTranNo;
                    }
                    
                    
                }

            }
            else
            {
                Ap.DisputeNumber = 0;
                Ap.DisputeTransaction = 0;
            }
            // OTHER THAN BULK then only one
            if (WOrigin != "ReconciliationBulk")  // Different than BULK 
            {
                Ap.Requestor = WSignedId;
                Ap.Authoriser = WAuthoriser;
                Ap.Origin = WOrigin;

                Ap.TranNo = WMaskRecordId;
                Ap.AtmNo = WAtmNo;
                Ap.ReplCycle = WReplCycle;
                Ap.DateOriginated = DateTime.Now;
                Ap.Stage = 1;
                Ap.DifferenceStatus = WDifStatus;
                Ap.Operator = WOperator;

                Ap.RMCategory = WRMCategory;
                Ap.RMCycle = WRMCycle;

                WSeqNumber = Ap.InsertAuthorizationRecord();
            }
            else // BULK creation of records 
            {
                RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

                WAction = 10;

                WithDate = false;

                Rc.ReadReconcCategorybyCategId(WOperator, WRMCategory);

                int AtmGroupNo = Rc.AtmGroup;  // Used for Atms Main

                Am.ReadAtmsMainForAuthUserAndFillTableForBulk(WOperator, WSignedId, NullPastDate, WithDate, WAction, AtmGroupNo);

                //Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, "");

                bool WAlreadyAuther;
                string TempAtmNo;
                int TempReplCycle;

                I = 0;

                while (I <= (Am.TableATMsMainSelected.Rows.Count - 1))
                {
                    WAlreadyAuther = false;
                    TempAtmNo = (string)Am.TableATMsMainSelected.Rows[I]["AtmNo"];
                    TempReplCycle = (int)Am.TableATMsMainSelected.Rows[I]["ReplCycleNo"];

                    Ta.ReadSessionsStatusTraces(TempAtmNo, TempReplCycle);

                    if (Ta.ProcessMode == 1)
                    {
                        // If Am.Authoriser = rejected then close old record, update Am with authoriser = no decision ANd then create a new one. 
                        Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(TempAtmNo, TempReplCycle, "Replenishment");

                        if (Ap.RecordFound == true & Ap.OpenRecord == true)
                        {
                            if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                            {
                                WAlreadyAuther = true;
                                //  guidanceMsg = " Normal outstanding .";
                            }
                            if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                            {
                                Am.ReadAtmsMainSpecific(TempAtmNo);
                                Am.Authoriser = "No Decision";
                                Am.UpdateAtmsMain(TempAtmNo);
                                Ap.RMCategory = WRMCategory;
                                Ap.RMCycle = WRMCycle;
                                //
                                //    // Close Authorisation record 
                                //    //
                                Ap.Stage = 5;
                                Ap.OpenRecord = false;

                                Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                            }

                        }

                        if (WAlreadyAuther == false)
                        {
                            Ap.Requestor = WSignedId;
                            Ap.Authoriser = WAuthoriser;
                            Ap.Origin = WOrigin;

                            Ap.TranNo = WMaskRecordId;
                            Ap.AtmNo = TempAtmNo;
                            Ap.ReplCycle = TempReplCycle;
                            Ap.DateOriginated = DateTime.Now;
                            Ap.Stage = 1;
                            Ap.DifferenceStatus = WDifStatus;
                            Ap.Operator = WOperator;

                            Ap.RMCategory = WRMCategory;
                            Ap.RMCycle = WRMCycle;

                            WSeqNumber = Ap.InsertAuthorizationRecord();
                        }
                    }
                    I++;
                }
            }

            Usi.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
            Usi.ProcessNo = 56;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //*********************
            // UPDATE Dispute details 
            //*********************
            if (WOrigin == "Dispute Action" )
            {

                //***********************
                // UPDATE DISPUTE RECORD

                if (WOperator == "ITMX")
                {
                    DtITMX.ChooseAuthor = true;
                    DtITMX.PendingAuthorization = true;
                    DtITMX.AuthorOriginator = WSignedId;
                    DtITMX.AuthorKey = WSeqNumber;
                    DtITMX.Authoriser = WAuthoriser;

                    DtITMX.UpdateDisputeTranRecord(DtITMX.DispTranNo);
                }
                else
                {
                    Dt.ChooseAuthor = true;
                    Dt.PendingAuthorization = true;
                    Dt.AuthorOriginator = WSignedId;
                    Dt.AuthorKey = WSeqNumber;
                    Dt.Authoriser = WAuthoriser;

                    Dt.UpdateDisputeTranRecord(Dt.DispTranNo);
                }

            }

            //if (WOrigin == "Replenishment" || WOrigin == "Reconciliation" || WOrigin == "ReconciliationCat"
            //    || WOrigin == "ReconciliationBulk" || WOrigin == "ForceMatchingCat" 
            //    || WOrigin == "SettlementAuth" || WOrigin == "SettlementFeesAuth" 
            //    || WOrigin == "ConfirmNostroMatching" || WOrigin == "SWDSession")
            //{
            //    //Find Key of Inserted Record 
            //    Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WReplCycle, WOrigin);

            //    // Create fields 

            //    // Update Ta. 
            //}
            //
            //*********************
            Us.ReadUsersRecord(Ap.Authoriser);
            RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();
            Usr.ReadUsersVsApplicationsVsRolesByApplication(WSignedId, Usi.SignInApplication);

            if (WOrigin == "Dispute Action" & WOperator != "ITMX")
            {
                if (Usr.MsgsAllowed == true)
                {
                    //if (IsSolo == true)
                    //{
                    //    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                    //}
                    //else
                    //{
                        MessageBox.Show("A message was sent to authoriser for : " + Environment.NewLine
                                   + "Dispute Number : " + Dt.DisputeNumber.ToString() + Environment.NewLine
                                   + " And Dispute Transaction Number : " + Dt.DispTranNo.ToString());
                    //}
                    
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }

            if (WOrigin == "Dispute Action" & WOperator == "ITMX")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("A message was sent to authoriser for : " + Environment.NewLine
                      + "Dispute Number : " + Dt.DisputeNumber.ToString() + Environment.NewLine
                      + " And Dispute Transaction Number : " + DtITMX.DispTranNo.ToString());
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }
            }

            if (WOrigin == "Replenishment")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Replenishment a message was sent to authoriser for : " + Environment.NewLine
                + "AtmNo : " + WAtmNo + Environment.NewLine
                + "And Repl Cycle :" + WReplCycle);
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }

            if (WOrigin == "Reconciliation")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Reconciliation a message was sent to authoriser for : " + Environment.NewLine
                        + "AtmNo : " + WAtmNo + Environment.NewLine
                        + " And Repl Cycle :" + WReplCycle);
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }

            if (WOrigin == "ReconciliationCat")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Reconciliation a message was sent to authoriser for : " + Environment.NewLine
                        + "Category ID : " + WRMCategory + Environment.NewLine
                        + " And Reconc Cycle :" + WRMCycle);
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }

            if (WOrigin == "SettlementAuth")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Settlement a message was sent to authoriser for : " + Environment.NewLine
                                + "Authorising " + Environment.NewLine
                                + "Settlement Cycle :" + WReplCycle);
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }
            if (WOrigin == "ReplOrders")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Maker a message was sent to authoriser for : " + Environment.NewLine
                                + "Authorising " + Environment.NewLine
                                + "ReplOrders Cycle :" + WReplCycle);
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }

            if (WOrigin == "LoadingExcel")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Maker a message was sent to authoriser for : " + Environment.NewLine
                                + "Authorising " + Environment.NewLine
                                + "Loading Excel Cycle :" + WReplCycle);
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }



            if (WOrigin == "SettlementFeesAuth")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Fees Settlement a message was sent to authoriser for : " + Environment.NewLine
                        + "Authorising " + Environment.NewLine
                        + "Settlement Cycle :" + WReplCycle);
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }

            if (WOrigin == "ConfirmNostroMatching")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Default Adjustments a message was sent to authoriser for : " + Environment.NewLine
                          + "Authorising " + Environment.NewLine
                          + "Daily Job Cycle :" + WReplCycle);
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }

            if (WOrigin == "ForceMatchingCat")
            {
                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Force Matching a message was sent to authoriser for : " + Environment.NewLine
                           + "Category ID : " + WAtmNo + Environment.NewLine
                           + " And Matching Cycle :" + WReplCycle);
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }

            if (WOrigin == "ReconciliationBulk")
            {

                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("From Reconciliation a message was sent to authoriser  " + Environment.NewLine
                           + "for BULK Authorisation ");
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }

            }

            if (WOrigin == "SWDSession")
            {

                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("A message was sent to authoriser  " + Environment.NewLine
                           + "to check and authorise work for this package. ");
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }
            }

            if (WOrigin == "Reversals")
            {

                if (Usr.MsgsAllowed == true)
                {
                    MessageBox.Show("A message was sent to authoriser  " + Environment.NewLine
                           + "to check and authorise work for these Reversals. ");
                }
                else
                {
                    MessageBox.Show("The Authoriser will Authorise or Reject your request ");
                }
            }

            this.Dispose();
        }

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridUsersVsAuthor()
        {
            dataGridView1.DataSource = UvsA.UsersToAuthorDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Authorisers Available!");
                this.Dispose();
                return;
            }

            dataGridView1.Columns[0].Width = 90; // AuthId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 150; //Authoriser Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 60; //  Type Of Author
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 190; // Openning Date
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // DATA TABLE ROWS DEFINITION 
            //UsersToAuthorDataTable.Columns.Add("AuthId", typeof(string));
            //UsersToAuthorDataTable.Columns.Add("Authoriser Name", typeof(string));
            //UsersToAuthorDataTable.Columns.Add("Type Of Author", typeof(string));
            //UsersToAuthorDataTable.Columns.Add("Openning Date", typeof(string));

            //dataGridView1.Rows[WRowIndex].Selected = true;
            //dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
    }
}

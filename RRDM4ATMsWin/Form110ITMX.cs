using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form110ITMX : Form
    {
        //
        // AUTHORISERS 
        // 
        Form111 NForm111;

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMDisputeTransactionsClassITMX Dt = new RRDMDisputeTransactionsClassITMX();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess(); 
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUserVsAuthorizers UvsA = new RRDMUserVsAuthorizers();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool WithDate;
        int WAction;
        int I; 
        int WDifStatus;

        string Gridfilter; 

        string WAuthoriser;
        string WAuthName;
        int WSeqNumber; 

        string WRMCategory ;
        int WRMCycle;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WOrigin;
        int WTranNo;
        string WAtmNo;
        int WReplCycle; 
        int WAuthorSeqNumber;
        string WFunction; 

        public Form110ITMX(string InSignedId, int SignRecordNo, string InOperator, string InOrigin,
                      int InTraNo, string InAtmNo, int InReplCycle,
                      int InAuthorSeqNumber, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WOrigin = InOrigin;

            WTranNo = InTraNo;

            WAtmNo = InAtmNo;
            WReplCycle = InReplCycle;

            WAuthorSeqNumber = InAuthorSeqNumber; // If this is > 0 means it is coming from Autorization management
                                                  // aiming at transfering autorization to another authorizer 
            WFunction = InFunction; // Values = Normal, Transfer 

            InitializeComponent();

            Us.ReadSignedActivityByKey(WSignRecordNo);
            WDifStatus = Us.ReconcDifferenceStatus;
            WRMCategory = Us.WFieldChar1;

            WRMCycle = Us.WFieldNumeric1; 

            if (WFunction == "Normal")
            {
                textBoxMessage.Text = "Select Authoriser";
            }

            if (WFunction == "Transfer")
            {
                panel2.Show();

                button1.Hide(); 

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
                                          + " AND UserId = '" + WSignedId + "' " + " AND OpenRecord = 1 " ;

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
                MessageBox.Show("This function is not allowed for Bulk authorisation") ;
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
            if (WOrigin == "Dispute Action")
            {
                Dt.ReadDisputeTranByMaskRecordId(WTranNo);
                Ap.DisputeNumber = Dt.DisputeNumber;
                Ap.DisputeTransaction = Dt.DispTranNo;
            }
            else
            {
                Ap.DisputeNumber = 0 ;
                Ap.DisputeTransaction = 0 ;
            }

            Ap.Requestor = WSignedId;
            Ap.Authoriser = WAuthoriser;
            Ap.Origin = WOrigin; 
           
            Ap.TranNo = WTranNo;
            Ap.AtmNo = WAtmNo;
            Ap.ReplCycle = WReplCycle; 
            Ap.DateOriginated = DateTime.Now;
            Ap.Stage = 2;
            Ap.DifferenceStatus = WDifStatus;
            Ap.Operator = WOperator;

            Ap.RMCategory = WRMCategory;
            Ap.RMCycle = WRMCycle;

            WSeqNumber = Ap.InsertAuthorizationRecord();

            Us.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
            Us.ProcessNo = 56;
            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);           
          
            //*********************
            // UPDATE Dispute details 
            //*********************
            if (WOrigin == "Dispute Action")
            {
                //Find Key of Inserted Record 
                Ap.FindAuthorizationLastNo(WSignedId, WTranNo);
                //*********************

                //***********************
                // UPDATE DISPUTE RECORD
                Dt.ChooseAuthor = true;
                Dt.PendingAuthorization = true;
                Dt.AuthorOriginator = WSignedId;
                Dt.AuthorKey = Ap.SeqNumber;
                Dt.Authoriser = WAuthoriser;
              
                Dt.UpdateDisputeTranRecord(Dt.DisputeNumber);
            }

            //*********************
           
            //***********************
            WSeqNumber = Ap.SeqNumber; 

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
                    Us.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
                    Us.ProcessNo = 1 ;
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WOrigin == "Reconciliation" || WOrigin == "ReconciliationCat" || WOrigin == "ForceMatchingCat" ) // No Bulk 
                { 
                    Us.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
                    Us.ProcessNo = 2;
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }
            }
            this.Dispose(); 
        }


// REMOTE AUTHORIZATION 
      
        private void buttonRemote_Click(object sender, EventArgs e)
        {
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

                Dt.ReadDisputeTranByMaskRecordId(WTranNo);

                Ap.DisputeNumber = Dt.DisputeNumber;
                Ap.DisputeTransaction = Dt.DispTranNo;
            }
            else
            {
                Ap.DisputeNumber = 0;
                Ap.DisputeTransaction = 0;
            }

            if (WOrigin != "ReconciliationBulk")  // Different than BULK 
            {
                Ap.Requestor = WSignedId;
                Ap.Authoriser = WAuthoriser;
                Ap.Origin = WOrigin;

                Ap.TranNo = WTranNo;
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

                Gridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'" + " AND ReconcDiff = 1";

                WAction = 11;

                WithDate = false; 

                Am.ReadAtmsMainForAuthUserAndFillTableForBulk(Gridfilter, NullPastDate, WithDate, WAction);

                //Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, "");

                bool WAlreadyAuther ;
                string TempAtmNo ;
                int TempReplCycle ;

                I = 0;

                while (I <= (Am.ATMsMainSelected.Rows.Count - 1))
                {
                    WAlreadyAuther = false; 
                    TempAtmNo = (string)Am.ATMsMainSelected.Rows[I]["AtmNo"];
                    TempReplCycle = (int)Am.ATMsMainSelected.Rows[I]["ReplCycle"];

                    Ta.ReadSessionsStatusTraces(TempAtmNo, TempReplCycle);

                    if (Ta.ProcessMode == 1)
                    {
                        // If Am.Authoriser = rejected then close old record, update Am with authoriser = no decision ANd then create a new one. 
                        Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(TempAtmNo, TempReplCycle, "Reconciliation");

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

                            Ap.TranNo = WTranNo;
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
         
            Us.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
            Us.ProcessNo = 56;
            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //*********************
            // UPDATE Dispute details 
            //*********************
            if (WOrigin == "Dispute Action")
            {
                //Find Key of Inserted Record 
                Ap.FindAuthorizationLastNo(WSignedId, WTranNo);
                //*********************

                //***********************
                // UPDATE DISPUTE RECORD
                Dt.ChooseAuthor = true;
                Dt.PendingAuthorization = true;
                Dt.AuthorOriginator = WSignedId;
                Dt.AuthorKey = Ap.SeqNumber;
                Dt.Authoriser = WAuthoriser;
            
                Dt.UpdateDisputeTranRecord(Dt.DispTranNo);
            }

            //if (WOrigin == "Replenishment" || WOrigin == "Reconciliation" || WOrigin == "ReconciliationCat"
            //    || WOrigin == "ReconciliationBulk" || WOrigin == "ForceMatchingCat")
            //{
            //    //Find Key of Inserted Record 
            //    Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WReplCycle, WOrigin);

            //    // Create fields 

            //    // Update Ta. 
            //}
            //*********************

            if( WOrigin == "Dispute Action" )
            {
                 MessageBox.Show("A message was sent to authoriser for : " + Environment.NewLine
                + "Dispute Number : " + Dt.DisputeNumber.ToString() + Environment.NewLine
                + " And Dispute Transaction Number : " + Dt.DispTranNo.ToString());
            }

            if( WOrigin == "Replenishment" )
            {
                 MessageBox.Show("From Replenishment a message was sent to authoriser for : " + Environment.NewLine
                + "AtmNo : " + WAtmNo + Environment.NewLine
                + "And Repl Cycle :" + WReplCycle);
            }

            if (WOrigin == "Reconciliation")
            {
                MessageBox.Show("From Reconciliation a message was sent to authoriser for : " + Environment.NewLine
               + "AtmNo : " + WAtmNo + Environment.NewLine
               + " And Repl Cycle :" + WReplCycle);
            }

            if (WOrigin == "ReconciliationCat")
            {
                MessageBox.Show("From Reconciliation a message was sent to authoriser for : " + Environment.NewLine
               + "Category ID : " + WAtmNo + Environment.NewLine
               + " And Reconc Cycle :" + WReplCycle);
            }

            if (WOrigin == "ForceMatchingCat")
            {
                MessageBox.Show("From Force Matching a message was sent to authoriser for : " + Environment.NewLine
               + "Category ID : " + WAtmNo + Environment.NewLine
               + " And Matching Cycle :" + WReplCycle);
            }

            if (WOrigin == "ReconciliationBulk")
            {
                MessageBox.Show("From Reconciliation a message was sent to authoriser  " + Environment.NewLine
               + "for BULK Authorisation " );
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
                return;
            }

            dataGridView1.Columns[0].Width = 90; // AuthId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 150; //Authoriser Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 60; //  Type Of Author
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 140; // Openning Date
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

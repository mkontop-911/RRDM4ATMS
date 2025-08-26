using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form201bITMXFEES : Form
    {
        RRDMITMXSettlementCycles Sc = new RRDMITMXSettlementCycles();
        RRDMITMXSettlementBanksToBanksFTCycleTotals SBBT = new RRDMITMXSettlementBanksToBanksFTCycleTotals();

        RRDMITMXSettlementBanksToBanksFTFeesCycleTotals SFT = new RRDMITMXSettlementBanksToBanksFTFeesCycleTotals();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMBanks Ba = new RRDMBanks();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

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

        bool ViewWorkFlow;
        string WMode;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        bool ReconciliationAuthorNoRecordYet;
        bool ReconciliationAuthorDone;

        bool ReconciliationAuthorOutstanding;

        int WITMXSettlementCycle;
        string WFeesEntity; 

        int WRowGrid1;
        int WRowGrid3;

        bool ITMXUser;
        bool SettlementUser; 

        string WFeesSettlement; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        public Form201bITMXFEES(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            Us.ReadUsersRecord(WSignedId);
            Ba.ReadBank(Us.BankId); 
            //
            // THIS FORM IS ALLOWED ONLY FOR THE SETTLEMENT ENTITY 
            // 
            if (Us.Operator == Us.BankId || ((Us.Operator != Us.BankId) & Ba.SettlementBank==true ))
            {
                if (Us.Operator == Us.BankId) ITMXUser = true;
                if ((Us.Operator != Us.BankId) & Ba.SettlementBank == true) SettlementUser = true; 
                labelStep1.Text = "FEES Settlement Transactions Creation" ;
                textBoxMsgBoard.Text = "Select Settlement Cycle to Create Txns ";
            }
            else
            {
                ITMXUser = false;

                labelStep1.Text = "Settlement Cycles Totals For : " + Us.BankId;

                button6.Hide(); 

                textBoxMsgBoard.Text = "Select Settlement Cycle and Pair of Banks. ";
            }

            WFeesSettlement = "FeesSettlement";

            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
            NormalProcess = false;
           // RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor)
            {
                NormalProcess = false;
            }
            else NormalProcess = true;
      
        }

        private void Form201ITMX_Load(object sender, EventArgs e)
        {
         
            ShowGridSettlementCycles(WOperator);
        }

        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

           WITMXSettlementCycle = (int)rowSelected.Cells[0].Value;

           Sc.ReadITMXSettlementCyclesById(WOperator, WITMXSettlementCycle);

            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageReconCateg(WFeesSettlement, WITMXSettlementCycle, "SettlementFeesAuth", WAuthoriser, WRequestor, Reject);
                textBoxMsgBoard.Text = Ap.MessageOut;
            }

            SetScreen();

            // Selected Files (TWO)

            ShowGridTotalsForBanks(WITMXSettlementCycle);

        }
        // Row enter for Category vs Source files 
        private void dataGridView2_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WFeesEntity = (string)rowSelected.Cells[0].Value;

            // Grid  (THREE)
            ShowGridPairsForBankA(WITMXSettlementCycle, WFeesEntity);
        }     
       
        // Row Enter For Matching fields 
        private void dataGridView3_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            labelBankA.Text = (string)rowSelected.Cells[0].Value;

            labelBankB.Text = (string)rowSelected.Cells[1].Value;
        }

        // FINISH
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            //// STEPLEVEL// Check Feasibility to move to next step 

            //if (Us.StepLevel == 3 & Us.ProcessStatus > 1)
            //{
            //    if (MessageBox.Show("You Still have differences...Are you sure you want to move to next step?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            //          == DialogResult.Yes)
            //    {
            //        // Application.Exit();
            //    }
            //    else return;
            //}

            //**************************************************
            //FINISH BUTTON .... HANDLE AUTHORISATION VALIDATION 
            //**************************************************

            if (WViewFunction == true) // Coming from View only 
            {
                this.Close();
                return;
            }

            // FINISH - Make validationsfor Authorisations  

            //  ReconciliationAuthorNeeded = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WFeesSettlement, WITMXSettlementCycle, "SettlementFeesAuth");
            if (Ap.RecordFound == true)
            {
                ReconciliationAuthorNoRecordYet = false;
                if (Ap.Stage == 3 || Ap.Stage == 4 || Ap.Stage == 5)
                {
                    ReconciliationAuthorDone = true;
                }
                else
                {
                    ReconciliationAuthorOutstanding = true;
                }
            }
            else
            {
                ReconciliationAuthorNoRecordYet = true;
            }


            if (WAuthoriser == true & ReconciliationAuthorDone == true) // Coming from authoriser and authoriser done  
            {
                this.Close();
                return;
            }

            if (Usi.ProcessNo == 2 & ReconciliationAuthorDone == true) //  
            {
                this.Close();
                return;
            }

            if (WRequestor == true & ReconciliationAuthorDone == false) // Coming from authoriser and authoriser not done 
            {
                this.Close();
                return;
            }

            if (Usi.ProcessNo == 2 & ReconciliationAuthorNoRecordYet == true) // Cancel by originator without request for authorisation 
            {
                if (MessageBox.Show("Warning: Authorisation outstanding " + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                             == DialogResult.Yes)
                {
                    this.Dispose();
                    return;
                }
                else
                {
                    return;
                }

            }

            if (Usi.ProcessNo == 2 & ReconciliationAuthorOutstanding == true) // Cancel with repl outstanding 
            {

                if (MessageBox.Show("Warning: Authorisation outstanding " + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                              == DialogResult.Yes)
                {
                    this.Dispose();
                    return;
                }
                else
                {
                    return;
                }

            }

            if (WAuthoriser == true & ReconciliationAuthorOutstanding == true) // Cancel by authoriser without making authorisation.
            {
                MessageBox.Show("MSG946 - Authorisation outstanding");
                return;
            }

            if ((Usi.ProcessNo == 2 || WRequestor == true) & ReconciliationAuthorDone == true) // Everything is fined .
            {

            }

            //
            //**************************************************************************
            //**************************************************************************
            // IF YOU CAME TILL HERE THEN RECONCILIATION WILL BE COMPLETED WITH UPDATING 
            //**************************************************************************
            //***********************************************************************
            //**************** USE TRANSACTION SCOPE
            //***********************************************************************

            // create a connection object
            //using (var scope = new System.Transactions.TransactionScope())
               try
               {

                    // Update authorisation record  

                    if (Ap.RecordFound == true & Ap.Stage == 4)
                    {
                        // Update stage 
                        //
                        Ap.Stage = 5;
                        Ap.OpenRecord = false;

                        Ap.UpdateAuthorisationRecord(Ap.SeqNumber);

                        //**************************************************************************
                        // Create Transactions to Be posted
                        //
                        //**************************************************************************

                        SFT.CreateTransForFeesSettlementClearing(WSignedId, Ap.Authoriser ,WOperator, WITMXSettlementCycle);


                        //**************************************************************************
                        // Update Settlement Cycle Record 
                        //
                        //**************************************************************************

                        Sc.ReadITMXSettlementCyclesById(WOperator, WITMXSettlementCycle);
                        Sc.ActionByUser = true;
                        Sc.ActionByAuth = true;
                        Sc.SettlementUser = Ap.Requestor;
                        Sc.AuthoriserUser = Ap.Authoriser;
                        Sc.AuthDateTm = DateTime.Now; 
                        Sc.UpdateSettlementCycle(WITMXSettlementCycle);

                        //cmd.Parameters.AddWithValue("@LastProcessedRecord", LastProcessedRecord);
                        //cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);
                        //cmd.Parameters.AddWithValue("@ActionByAuth", ActionByAuth);
                        //cmd.Parameters.AddWithValue("@SettlementUser", SettlementUser);
                        //cmd.Parameters.AddWithValue("@AuthoriserUser", AuthoriserUser);
                        //cmd.Parameters.AddWithValue("@AuthDateTm", AuthDateTm);
                        //MessageBox.Show("No transactions to be posted");
                        Form2 MessageForm = new Form2("Settlement Cycle Has been Completed for Cycle No: " + WITMXSettlementCycle + Environment.NewLine
                                                       + "Transactions Are Created" + Environment.NewLine
                                                       );
                        MessageForm.ShowDialog();
                        // COMPLETE SCOPE
                        //
                        //scope.Complete();

                        this.Close();

                     }

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
            //finally
            //{
            //    scope.Dispose();
            //}

            this.Dispose();
        }
// Go to Next  

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridSettlementCycles(string Operator)
        {
            // Keep Scroll position 
            
            int scrollPosition = 0;
          
            Sc.ReadITMXSettlementCyclesFillTable(WOperator); 
     
            //this.reconcSourceFilesTableAdapter.Fill(this.aTMSDataSet63.ReconcSourceFiles);

            dataGridView1.DataSource = Sc.TableITMXDailyJobCycles.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }
            else
            {
                WRowGrid1 = dataGridView1.SelectedRows[0].Index;
                if (WRowGrid1 > 0)
                {
                    scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
                }

            }

            dataGridView1.Columns[0].Width = 90; //SettlementCycle
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 90; // Category
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView1.Columns[2].Width = 110; // StartedDate
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 100; // Description
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ////dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            // DATA TABLE ROWS DEFINITION 
            //TableITMXDailyJobCycles.Columns.Add("ITMXSettlementCycle", typeof(int));
            //TableITMXDailyJobCycles.Columns.Add("Category", typeof(string));

            //TableITMXDailyJobCycles.Columns.Add("StartedDate", typeof(DateTime));
            //TableITMXDailyJobCycles.Columns.Add("Description", typeof(string));
        }

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGridTotalsForBanks(int InRunningCycle)
        {
            if (ITMXUser == true || SettlementUser == true)
            {
                SFT.ReadTableTotalsForAllFeesEntities(WOperator,WSignedId, WITMXSettlementCycle, "");
            }
            else
            {
                SFT.ReadTableTotalsForAllFeesEntities(WOperator, WSignedId, WITMXSettlementCycle, Us.BankId);
            }
            
            dataGridView2.DataSource = SFT.TableTotalsForAllFeesEntities.DefaultView;
            
            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }
            dataGridView2.Columns[0].Width = 70; // FeesEntity
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 50; // SettlementCycle
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[1].Visible = false;

            dataGridView2.Columns[2].Width = 90; // Date
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[3].Width = 90; // DebitAmount
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[4].Width = 90; // CreditAmount
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[5].Width = 90; // NetFees
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView2.Columns[3].Visible = false; // ProcessMode
            //dataGridView2.Columns[4].Visible = false; // LastInFileDtTm
            //dataGridView2.Columns[5].Visible = false; // LastMatchingDtTm
     
        }

        //******************
        // SHOW GRID dataGridView3
        //******************
        private void ShowGridPairsForBankA(int InSettlenetCycle, string InFeesEntity)
        {
            // Keep Scroll position 
           
            int scrollPosition = 0;

            SFT.ReadTableFeesTotalsForFeesEntity(WOperator, WSignedId, InSettlenetCycle, InFeesEntity); 
     
            dataGridView3.DataSource = SFT.TableTotalsFeesForBank.DefaultView;

            if (dataGridView3.Rows.Count == 0)
            {
                return;
            }
            else
            {
                // Keep Scroll position 
                WRowGrid3 = dataGridView3.SelectedRows[0].Index;

                if (WRowGrid3 > 0)
                {
                    scrollPosition = dataGridView3.FirstDisplayedScrollingRowIndex;
                }
            }

            labelPair.Text = "TOTALS FOR Entity : " + InFeesEntity + " WITH OTHER Entities" ;

            dataGridView3.Columns[0].Width = 60; // FeesEntity 
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[1].Width = 60; // BankA
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[2].Width = 60; // BankB
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[3].Width = 70; // DRAmount
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[4].Width = 70; // CRAmount
            dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[5].Width = 70; // NetFees 
            dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[6].Width = 115; // DateTmCreated
            dataGridView3.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[7].Width = 100; // SettlementCycle
            dataGridView3.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            dataGridView3.Rows[WRowGrid3].Selected = true;
            dataGridView3_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid3));

            dataGridView3.FirstDisplayedScrollingRowIndex = scrollPosition;

            // DATA TABLE ROWS DEFINITION 
            //TableTotalsFeesForBank.Columns.Add("FeesEntity", typeof(string));
            //TableTotalsFeesForBank.Columns.Add("BankA", typeof(string));
            //TableTotalsFeesForBank.Columns.Add("BankB", typeof(string));
            //TableTotalsFeesForBank.Columns.Add("DRAmount", typeof(decimal));
            //TableTotalsFeesForBank.Columns.Add("CRAmount", typeof(decimal));
            //TableTotalsFeesForBank.Columns.Add("NetFees", typeof(decimal));
            //TableTotalsFeesForBank.Columns.Add("DateTmCreated", typeof(DateTime));
            //TableTotalsFeesForBank.Columns.Add("SettlementCycle", typeof(int));
        }
        //Print Lines of Banks Debits and Credits 
        private void button6_Click(object sender, EventArgs e)
        {
            // Assign Parameters

            // Call and print report 
            string P1 = WITMXSettlementCycle.ToString();
            string P2 = Sc.CreatedDate.ToString();
            string P3 = Us.BankId;
            string P4 = WSignedId; 

            Form56R41ITMX ReportITMX41 = new Form56R41ITMX(P1, P2, P3, P4);
            ReportITMX41.Show();
        }
        
        //Print Lines for Single Bank  
        private void button2_Click(object sender, EventArgs e)
        {
            string P1 = WITMXSettlementCycle.ToString();
            string P2 = Sc.CreatedDate.ToString();
            string P3 = WFeesEntity;
            string P4 = Us.BankId;
            string P5 = WSignedId; 

            Form56R42ITMX ReportITMX42 = new Form56R42ITMX(P1, P2, P3, P4, P5);
            ReportITMX42.Show();
        }
// Print Line Transactions 
        private void button1_Click(object sender, EventArgs e)
        {
            string P1 = WITMXSettlementCycle.ToString();
            string P2 = Sc.CreatedDate.ToString();
            string P3 = WFeesEntity;
            string P4 = labelBankB.Text;
            string P5 = Us.BankId;
            string P6 = WSignedId;

            if (WFeesEntity == "BBL" & labelBankB.Text == "KBANK")
            {
                //OK
            }
            else
            {
                MessageBox.Show("Please select pair BBL Vs KBANK. We have testing data only for this pair.");
                return;
            }

            Form56R33ITMX ReportITMX33 = new Form56R33ITMX(P1, P2, P3, P4, P5, P6);
            ReportITMX33.Show();
        }
//
        private void buttonAuthor_Click(object sender, EventArgs e)
        {

            // Check if Already in authorization process

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WFeesSettlement, WITMXSettlementCycle, "SettlementFeesAuth");

            if (Ap.RecordFound == true & Ap.OpenRecord == true)
            {
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                }
            }

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WFeesSettlement, WITMXSettlementCycle, "SettlementFeesAuth");

            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
            {
                MessageBox.Show("This Replenishment Cycle Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process."
                                                          );
                return;
            }

            // Validate input 
       //     RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.WFieldChar1 = "FeesSettlement" ;
            Usi.WFieldNumeric1 = WITMXSettlementCycle; 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            int WTranNo = 0;

            string WOrigin = "SettlementFeesAuth";

            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WFeesSettlement, WITMXSettlementCycle, AuthorSeqNumber,0,"",0 ,"Normal");
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();

            textBoxMsgBoard.Text = "A message was sent to authoriser. Refresh for progress monitoring.";
           
        }

        private void NForm110_FormClosed(object sender, FormClosedEventArgs e)
        {
            //************************************************************
            //************************************************************
            // AUTHOR PART
            //************************************************************
            WViewFunction = false;
            WAuthoriser = false;
            WRequestor = false;
            NormalProcess = false;

         //   RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor
            else NormalProcess = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WFeesSettlement, WITMXSettlementCycle, "SettlementFeesAuth");

            if (WRequestor == true & Ap.Stage == 1)
            {
                textBoxMsgBoard.Text = "Message was sent to authoriser. Refresh for progress ";
           
            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                textBoxMsgBoard.Text = "Authorisation made. Workflow can finish! ";
               
            }

            if (NormalProcess) // Orginator has deleted authoriser 
            {
                textBoxMsgBoard.Text = "Please make authorisation ";
            }

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
                Ap.GetMessageReconCateg(WFeesSettlement, WITMXSettlementCycle, "SettlementFeesAuth", WAuthoriser, WRequestor, Reject);
                textBoxMsgBoard.Text = Ap.MessageOut;
               
            }

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Fees Settlement for Cycle : " + WITMXSettlementCycle.ToString();
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

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WFeesSettlement, WITMXSettlementCycle, "SettlementFeesAuth");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        textBoxMsgBoard.Text = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {
                       // RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
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

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WFeesSettlement, WITMXSettlementCycle, "SettlementFeesAuth");
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
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    StageDescr = "Authorization REJECTED. ";
                    Color Red = Color.Red;
                    labelAuthStatus.ForeColor = Red;
                }

                if (Ap.Stage == 5) StageDescr = "Authorisation process is completed";

                labelAuthStatus.Text = "Current Status : " + StageDescr;

                Us.ReadUsersRecord(Ap.Requestor);
                labelRequestor.Text = "Requestor : " + Us.UserName;

                Us.ReadUsersRecord(Ap.Authoriser);
                labelAuthoriser.Text = "Authoriser : " + Us.UserName;

                textBoxComment.Text = Ap.AuthComment;

                WAuthorSeqNumber = Ap.SeqNumber;
                labelAuthHeading.Show();

                if (WOperator == "ITMX")
                {
                    labelAuthHeading.Text = "AUTHORISER's SECTION FOR SETTLEMENT : " + WITMXSettlementCycle.ToString();
                }
                else
                {
                    //labelAuthHeading.Text = "AUTHORISER's SECTION FOR : " + WReconcCategoryId;
                }


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
//Notes
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Fees Settlement for Cycle : " + WITMXSettlementCycle.ToString();
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }
//Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            ShowAuthorisationInfo();
        }
        // AUthorisation section - Authorise 
        private void buttonAuthorise_Click(object sender, EventArgs e)
        {
      
            UpdateAuthorRecord("YES");

            textBoxMsgBoard.Text = "Authorisation Made - Accepted ";
            

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
//REJECT 
        private void buttonReject_Click(object sender, EventArgs e)
        {
            if (textBoxComment.TextLength < 5)
            {
                MessageBox.Show("Please input comment to explain rejection");
                return;
            }
            UpdateAuthorRecord("NO");

            textBoxMsgBoard.Text = "Authorisation Made - Rejected ";
          
            ShowAuthorisationInfo();
        }
//HISTORY OF AUTHORISATIONS 
        private void buttonAuthHistory_Click(object sender, EventArgs e)
        {
            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WFeesSettlement, WITMXSettlementCycle, WDisputeNo, WDisputeTranNo, WFeesSettlement, WITMXSettlementCycle);
            NForm112.ShowDialog();
        }
    }
}

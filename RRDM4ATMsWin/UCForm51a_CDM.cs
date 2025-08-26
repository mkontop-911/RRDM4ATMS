using System;
using System.Windows.Forms;
using System.Configuration;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51a_CDM : UserControl
    {
        // Stavros
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool WSetScreen; 

        bool Errors;

        string SelectionCriteria; 

     bool ViewWorkFlow ;
     string WMode;
     // NOTES 
     string Order;

     string WParameter4;
     string WSearchP4;

        // Define Session Balance Record 

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMCaptureCardsClass Cca = new RRDMCaptureCardsClass(); 

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMErrorsClassWithActions Err = new RRDMErrorsClassWithActions();

        RRDMSessionsPhysicalInspection Pi = new RRDMSessionsPhysicalInspection();

        //RRDMGasParameters Gp = new RRDMGasParameters(); 

        RRDMEmailClass2 Em = new RRDMEmailClass2();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();
    
        int WFunction;
        int temp; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;
        
        public void UCForm51a_CDM_Par(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
    
            WAtmNo = InAtmNo;
            WSesNo = InSesNo; 
      
            InitializeComponent();

            // ================USER BANK =============================
        //    Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
       //     WUserOperator = Us.Operator;
            // ========================================================

            this.DoubleBuffered = true; 
            
        }

        private void ReplenishmentStep1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet42.ErrorsTable' table. You can move, or remove it, as needed.
        //    this.errorsTableTableAdapter.Fill(this.aTMSDataSet42.ErrorsTable);
        }

        public void SetScreen()
        {
            try
            {
                WSetScreen = true;

                // NOTES for final comment 
                Order = "Descending";
                WParameter4 = "Physical Inspection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
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

                if (Usi.ProcessNo == 13 || Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
                {
                    ViewWorkFlow = true;

                    if (Cn.TotalNotes == 0)
                    {
                        label1.Hide();
                        //textBox21.Hide();
                        buttonNotes2.Hide();
                        labelNumberNotes2.Hide();
                    }
                    else
                    {
                        label1.Text = "Read Notes";
                        //textBox21.Hide();
                    }
                }

                tableLayoutPanel1.Dock = DockStyle.Top;

                // Read Capture Cards to find the number of them 

                Cca.ReadCapturedCardsNoWithinSession(WAtmNo, WSesNo);


                // labelCaptCards.Text = Cca.CaptureCardsNo.ToString(); // Show Number of capture cards 


                // Read Traces to get values to display 
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                //    ReadSessionsStatusTraces(AtmNo, SesNo, InProcess);
                textBoxOwnCust.Text = Ta.Stats1.NoOfCustLocals.ToString();
                textBoxOtherCust.Text = Ta.Stats1.NoOfCustOther.ToString();
                textBoxTransNo.Text = (Ta.Stats1.NoOfTranCash + Ta.Stats1.NoOfTranDepCash + Ta.Stats1.NoOfTranDepCheq).ToString();   // No of Trans
                textBoxMoneyRem.Text = "";

                textBox1Availability.Text = Ta.Stats1.NoOpMinutes.ToString();
                textBoxNoAvailability.Text = Ta.Stats1.OfflineMinutes.ToString();
                textBoxLineProblems.Text = Ta.Stats1.CommErrNum.ToString();

                //  int PercAvailable = (Ta.Stats1.NoOpMinutes *100 / (Ta.Stats1.NoOpMinutes + Ta.Stats1.OfflineMinutes));
                if ((Ta.Stats1.NoOpMinutes + Ta.Stats1.OfflineMinutes) > 0)
                {
                    textBoxPercUpTime.Text = (Ta.Stats1.NoOpMinutes * 100 / (Ta.Stats1.NoOpMinutes + Ta.Stats1.OfflineMinutes)).ToString();
                }
                //  readSessionNotesAndValues to get Cassettes 

                WFunction = 1;
                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // REPL TO REP IS DONE WITHIN THIS CLASS 

                if (Na.NumberOfErrJournal > 0)
                {
                    Errors = true;
                }
                else Errors = false;

                if (Na.Balances1.OpenBal == 0)
                {
                    textBox6.Hide();
                    textBox54.Hide();
                    textBox20.Hide();
                    textBox1.Hide(); 
                    panel5.Hide();
                    panel3.Hide();
                    panel6.Hide();
                }

                    if (Na.Balances1.OpenBal > 0)
                    textBoxMoneyRem.Text = (Na.Balances1.ReplToRepl / Na.Balances1.OpenBal).ToString("#,##0.00");
                else textBoxMoneyRem.Text = "0";


                if (Na.BalSets >= 1)
                {
                    labelCur1.Text = Na.Balances1.CurrNm; // First Currency  
                    textBoxOpenBal1.Text = Na.Balances1.OpenBal.ToString("#,##0.00");
                    textBoxDispensed1.Text = (Na.Balances1.OpenBal - Na.Balances1.ReplToRepl).ToString("#,##0.00");
                    textBoxClosingCash1.Text = Na.Balances1.ReplToRepl.ToString("#,##0.00");
                    textBoxMachineCounters1.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
                    textBoxDifference1.Text = (Na.Balances1.ReplToRepl - Na.Balances1.MachineBal).ToString("#,##0.00");

                }
                if (Na.BalSets >= 2)
                {
                    labelCur2.Text = Na.Balances2.CurrNm; // Second Currency  
                    labelCur2a.Text = Na.Balances2.OpenBal.ToString("#,##0.00");
                    labelCur2b.Text = (Na.Balances2.OpenBal - Na.Balances2.ReplToRepl).ToString("#,##0.00");
                    labelCur2c.Text = Na.Balances2.ReplToRepl.ToString("#,##0.00");
                    labelCur2d.Text = Na.Balances2.MachineBal.ToString("#,##0.00");
                    labelCur2e.Text = (Na.Balances2.ReplToRepl - Na.Balances2.MachineBal).ToString("#,##0.00");
                }
                if (Na.BalSets >= 3)
                {
                    labelCur3.Text = Na.Balances3.CurrNm; // Third Currency  
                    labelCur3a.Text = Na.Balances3.OpenBal.ToString("#,##0.00");
                    labelCur3b.Text = (Na.Balances3.OpenBal - Na.Balances3.ReplToRepl).ToString("#,##0.00");
                    labelCur3c.Text = Na.Balances3.ReplToRepl.ToString("#,##0.00");
                    labelCur3d.Text = Na.Balances3.MachineBal.ToString("#,##0.00");
                    labelCur3e.Text = (Na.Balances3.ReplToRepl - Na.Balances3.MachineBal).ToString("#,##0.00");
                }
                if (Na.BalSets >= 4)
                {
                    labelCur4.Text = Na.Balances4.CurrNm; // Forth Currency  
                    labelCur4a.Text = Na.Balances4.OpenBal.ToString("#,##0.00");
                    labelCur4b.Text = (Na.Balances4.OpenBal - Na.Balances4.ReplToRepl).ToString("#,##0.00");
                    labelCur4c.Text = Na.Balances4.ReplToRepl.ToString("#,##0.00");
                    labelCur4d.Text = Na.Balances4.MachineBal.ToString("#,##0.00");
                    labelCur4e.Text = (Na.Balances4.ReplToRepl - Na.Balances4.MachineBal).ToString("#,##0.00");
                }
                if (Na.BalSets == 1)
                {
                    labelCur2.Dispose();
                    labelCur2a.Dispose();
                    labelCur2b.Dispose();
                    labelCur2c.Dispose();
                    labelCur2d.Dispose();
                    labelCur2e.Dispose();

                    labelCur3.Dispose();
                    labelCur3a.Dispose();
                    labelCur3b.Dispose();
                    labelCur3c.Dispose();
                    labelCur3d.Dispose();
                    labelCur3e.Dispose();

                    labelCur4.Dispose();
                    labelCur4a.Dispose();
                    labelCur4b.Dispose();
                    labelCur4c.Dispose();
                    labelCur4d.Dispose();
                    labelCur4e.Dispose();
                }

                if (Na.BalSets == 2)
                {
                    labelCur3.Dispose();
                    labelCur3a.Dispose();
                    labelCur3b.Dispose();
                    labelCur3c.Dispose();
                    labelCur3d.Dispose();
                    labelCur3e.Dispose();

                    labelCur4.Dispose();
                    labelCur4a.Dispose();
                    labelCur4b.Dispose();
                    labelCur4c.Dispose();
                    labelCur4d.Dispose();
                    labelCur4e.Dispose();
                }

                if (Na.BalSets == 3)
                {
                    labelCur4.Dispose();
                    labelCur4a.Dispose();
                    labelCur4b.Dispose();
                    labelCur4c.Dispose();
                    labelCur4d.Dispose();
                    labelCur4e.Dispose();
                }

                if (Na.BalSets == 4)
                {
                    // Hide nothing 
                }

                //    labelTraceNo.Text = Na.FirstTraceNo.ToString();


                // SHOW CONTENTS OF CASSETTES
                if (Na.Cassettes_1.InNotes > 0)
                {
                    temp = Convert.ToInt32(Na.Cassettes_1.FaceValue);
                    label2.Text = temp.ToString() + " " + Na.Cassettes_1.CurName;
                    label13.Text = Na.Cassettes_1.RemNotes.ToString();
                    progressBar1.Value = Na.Cassettes_1.RemNotes * 100 / Na.Cassettes_1.InNotes;
                    label9.Text = Na.Cassettes_1.InNotes.ToString();

                    // Create the ToolTip and associate with the Form container.
                    //     ToolTip toolTip1 = new ToolTip();


                    // Force the ToolTip text to be displayed whether or not the form is active.
                    //     toolTip1.ShowAlways = true;

                    // Set up the ToolTip text for the Button and Checkbox.
                    //     toolTip1.SetToolTip(this.label9, "MONEY YOU HAVE PUT IN");

                }
                else
                {
                    label13.Hide();
                    label9.Hide();
                }

                if (Na.Cassettes_2.InNotes > 0)
                {
                    temp = Convert.ToInt32(Na.Cassettes_2.FaceValue);
                    label6.Text = temp.ToString() + " " + Na.Cassettes_2.CurName;
                    label14.Text = Na.Cassettes_2.RemNotes.ToString();
                    progressBar2.Value = Na.Cassettes_2.RemNotes * 100 / Na.Cassettes_2.InNotes;
                    label10.Text = Na.Cassettes_2.InNotes.ToString();
                }
                else
                {
                    label14.Hide();
                    label10.Hide();
                }

                if (Na.Cassettes_3.InNotes > 0)
                {
                    temp = Convert.ToInt32(Na.Cassettes_3.FaceValue);
                    label7.Text = temp.ToString() + " " + Na.Cassettes_3.CurName;
                    label15.Text = Na.Cassettes_3.RemNotes.ToString();
                    progressBar3.Value = Na.Cassettes_3.RemNotes * 100 / Na.Cassettes_3.InNotes;
                    label11.Text = Na.Cassettes_3.InNotes.ToString();
                }
                else
                {
                    label15.Hide();
                    label11.Hide();
                }

                if (Na.Cassettes_4.InNotes > 0)
                {
                    temp = Convert.ToInt32(Na.Cassettes_4.FaceValue);
                    label8.Text = temp.ToString() + " " + Na.Cassettes_4.CurName;
                    label16.Text = Na.Cassettes_4.RemNotes.ToString();
                    progressBar4.Value = Na.Cassettes_4.RemNotes * 100 / Na.Cassettes_4.InNotes;
                    label12.Text = Na.Cassettes_4.InNotes.ToString();
                }
                else
                {
                    label16.Hide();
                    label12.Hide();
                }

                // Rejected tray number of notes and money 

                int WTotalNotesInReject;

                WTotalNotesInReject = Na.Cassettes_1.RejNotes + Na.Cassettes_2.RejNotes + Na.Cassettes_3.RejNotes
                                          + Na.Cassettes_4.RejNotes;

                textBox3.Text = "NOTE : Reject Tray contains " + WTotalNotesInReject.ToString() + " Notes of different denominations";
                Decimal Percentage = 0;
                int Perc = 0;
                if (Na.Balances1.OpenBal > 0)
                {
                    Percentage = ((Na.Balances1.MachineBal / Na.Balances1.OpenBal)) * 100;
                    Perc = Convert.ToInt32(Percentage);
                }


                textBoxMoneyRem.Text = Perc.ToString(); // Money remain in cassettes 

                // Show  Physical Inspection Data 

                SelectionCriteria = " ATMNo='" + WAtmNo + "' AND SesNo = " + WSesNo;
                Pi.ReadPhysicalInspectionRecordsToFillDataTable(SelectionCriteria);
                //Show
                ShowGridView3();

                //Show Errors 
                string WFilter = " AtmNo ='"+ WAtmNo +"' AND SesNo =" + WSesNo + " AND (ErrId = 55 OR ErrId = 225 OR ErrId = 226)";
                Err.ReadErrorsAndFillTable(WOperator, WSignedId, WFilter);

                ShowGridView1();

                if ((Na.Balances1.ReplToRepl - Na.Balances1.MachineBal) == 0 & (Na.Balances2.ReplToRepl - Na.Balances2.MachineBal) == 0 &
                    (Na.Balances3.ReplToRepl - Na.Balances3.MachineBal) == 0 & (Na.Balances3.ReplToRepl - Na.Balances3.MachineBal) == 0)
                {
                    buttonFindRecord.Hide();
                    if (Errors == false)
                    {
                        guidanceMsg = " Do the physical inspection and move to next ! ";
                    }
                    else guidanceMsg = " Do the physical inspection and move to next ! ";
                }
                else
                {
                    // Send email to controller 
                    string Recipient = "panicos.michael@cablenet.com.cy";

                    string Subject = "System Error for ATMNo:" + WAtmNo;

                    string EmailBody = "ATM No : " + WAtmNo + " presence a problem with reconciliation from Repl to Repl. at Date and Time: "
                         + DateTime.Now.ToString() + " Repl Cycle No:" + WSesNo.ToString() + " Error Amount:" + (Na.Balances2.ReplToRepl - Na.Balances2.MachineBal).ToString();

                    // ***********************
                    //Em.SendEmail(WOperator, Recipient, Subject, EmailBody);
                    // ***********************
                    // 
                    MessageBox.Show("There is problem with Balancing of Transactions."+Environment.NewLine
                                    + "Continue Work but also inform controller."
                                   );

                    buttonFindRecord.Show();

                    guidanceMsg = " THERE IS PROBLEM WITH SYSTEM. Controller was informed through an email.";

                    // UPATE ERROR TABLE ABOUT THE RECONCILIATION ERROR 

                    Err.ErrId = 777; // THIS IS THE ID OF THE ERROR FROM REPL TO REPL
                    Err.ReadErrorsIDRecord(Err.ErrId, WOperator); // READ TO GET THE CHARACTERISTICS

                    // INITIALISED WHAT IS NEEDED 

                    Err.CategoryId = "N/A";
                    Err.RMCycle = 0;
                    Err.UniqueRecordId = 0;

                    Err.AtmNo = WAtmNo;
                    Err.SesNo = WSesNo;
                    Err.DateTime = DateTime.Now;

                    // Find CitNo
                    Am.ReadAtmsMainSpecific(WAtmNo);

                    Err.CitId = Am.CitId;

                    Err.OpenErr = true;

                    if ((Na.Balances1.ReplToRepl - Na.Balances1.MachineBal) != 0)
                    {
                        //   Pa.CurrCd = Na.Balances1.CurrCd;
                        Err.CurDes = Na.Balances1.CurrNm;
                        Err.ErrAmount = Na.Balances1.ReplToRepl - Na.Balances1.MachineBal;
                        Err.Operator = Am.Operator;
                        Err.InsertError(); // INSERT ERROR
                    }

                    if ((Na.Balances2.ReplToRepl - Na.Balances2.MachineBal) != 0)
                    {
                        //  Pa.CurrCd = Na.Balances2.CurrCd;
                        Err.CurDes = Na.Balances2.CurrNm;
                        Err.ErrAmount = Na.Balances2.ReplToRepl - Na.Balances2.MachineBal;
                        Err.Operator = Am.Operator;
                        Err.InsertError(); // INSERT ERROR
                    }

                    if ((Na.Balances3.ReplToRepl - Na.Balances3.MachineBal) != 0)
                    {
                        //   Pa.CurrCd = Na.Balances3.CurrCd;
                        Err.CurDes = Na.Balances3.CurrNm;
                        Err.ErrAmount = Na.Balances3.ReplToRepl - Na.Balances3.MachineBal;
                        Err.Operator = Am.Operator;
                        Err.InsertError(); // INSERT ERROR
                    }

                    if ((Na.Balances4.ReplToRepl - Na.Balances4.MachineBal) != 0)
                    {
                        //   Pa.CurrCd = Na.Balances4.CurrCd;
                        Err.CurDes = Na.Balances4.CurrNm;
                        Err.ErrAmount = Na.Balances4.ReplToRepl - Na.Balances4.MachineBal;
                        Err.Operator = Am.Operator;
                        Err.InsertError(); // INSERT ERROR
                    }

                }

                // Handle ViewWorkFlow 

                if (ViewWorkFlow == true)
                {
                    buttonUpdate.Hide();
                    //    textBox3.ReadOnly = true;


                    //     guidanceMsg = " View only "; THIS IS MOVED TO FORM1
                }
                // End of SetScreen 
                WSetScreen = false;
            }
            catch (Exception ex)
            {

                RRDMLog4Net Log = new RRDMLog4Net();

                string WLogger = "RRDM4Atms";

                string WParameters = "";

                Log.CreateAndInsertRRDMLog4NetMessage(ex, WLogger, WParameters);

                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");

                //Environment.Exit(0);
            }
           
        }
        //
        // UPDATE PHYSICAL SECURITY CHECK
        //
        bool Selection;
        int SeqNo;
      
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            //SelectionCriteria = " ATMNo='" + WAtmNo + "' AND SesNo = " + WSesNo;
            //Pi.ReadPhysicalInspectionRecordsToSeeIfAlert(SelectionCriteria);
            bool InspectionAlert = false; 
            int K = 0;

            while (K <= (dataGridView3.Rows.Count - 1))
            {
                Selection = (bool)dataGridView3.Rows[K].Cells["Selection"].Value;
               
                if (Selection == false) InspectionAlert = true; 

                K++; // Read Next entry of the table 
            }

            // Check Notes if a problem is present 
            if (InspectionAlert == true)
            {
                // How many Notes 
                string WParameter4 = "Physical Inspection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
                string Order = "Ascending";
                string SearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, SearchP4);

                if (Cn.RecordFound == true)
                {
                }

                if (Cn.TotalNotes == 0)
                {
                    MessageBox.Show("You must FILL NOTES to describe the not selected checkpoint ",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // STEPLEVEL
                   
                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    if (Usi.ReplStep1_Updated == true)
                    {
                        Usi.ReplStep1_Updated = false;

                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    }
                    return;
                }
                else // There is value = something will be reported 
                {

                }
            }

            K = 0;

            while (K <= (dataGridView3.Rows.Count - 1))
            {
                Selection = (bool)dataGridView3.Rows[K].Cells["Selection"].Value;
                SeqNo = (int)dataGridView3.Rows[K].Cells["SeqNo"].Value;

                Pi.Selection = Selection;
                Pi.UpdateSessionsPhysicalInspectionRecord(SeqNo);

                K++; // Read Next entry of the table 
            }

            // Update STEP
         
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep1_Updated = true;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            SelectionCriteria = " ATMNo='" + WAtmNo + "' AND SesNo = " + WSesNo;
            Pi.ReadPhysicalInspectionRecordsToFillDataTable(SelectionCriteria);
            //Show

            int WRowIndex = dataGridView3.SelectedRows[0].Index;

            int scrollPosition = dataGridView3.FirstDisplayedScrollingRowIndex;

            ShowGridView3();

            dataGridView3.Rows[WRowIndex].Selected = true;
            dataGridView3_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView3.FirstDisplayedScrollingRowIndex = scrollPosition;

            guidanceMsg = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
            ChangeBoardMessage(this, e);

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

// Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter2 = "PhysicalInspection";
            string WParameter3 = WAtmNo;
            string WParameter4 = "Physical Inspection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            string SearchP4 = "";            
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter2 , WParameter3, WParameter4, WMode , SearchP4);
            NForm197.ShowDialog();

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Physical Inspection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep1_Updated = false;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //buttonUpdate.Show();
            //SetScreen();
        }
        //
// On Change CheckBox1
        //
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetSteplevel();        
        }

        //
// On Change CheckBox2
        //

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }
        //
// On Change CheckBox3
        //
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }
        //
// On Change CheckBox4
        //
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }
        //
// On Change CheckBox5
        //
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }
// SET step level to need update if changes 
        private void SetSteplevel()
        {
            if (WSetScreen == false)
            {
                // Update STEP

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ReplStep1_Updated = false;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                //buttonUpdate.Show();
            }
        }

        //******************
        // SHOW GRIDView1
        //******************
        private void ShowGridView1()
        {
            dataGridView1.DataSource = Err.ErrorsTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                dataGridView1.Hide();
                textBox1.Text = "NOT FOUND ERRORS IN JOURNAL";
                return;
            }

            dataGridView1.Columns[0].Width = 40; // ExcNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 140; // Desc
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 100; //  Card
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Columns[3].Width = 50; // Ccy
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 80; // Amount
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 50; // NeedAction
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 50; // UnderAction 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[7].Width = 50; // ManualAct

            dataGridView1.Columns[7].Width = 90; // DateTime

            dataGridView1.Columns[8].Width = 120; // TransDescr      

            dataGridView1.Columns[9].Width = 100; // TransDescr   
            dataGridView1.Columns[10].Width = 160; // TransDescr   

          
        }
        //******************
        // SHOW GRIDView3
        //******************
        private void ShowGridView3()
        {
            dataGridView3.DataSource = Pi.PhysicalInspectionDataTable.DefaultView;

            dataGridView3.Columns[0].Width = 90; // Selection
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[1].Width = 210; // Check point 
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[2].Visible = false;
            dataGridView3.Columns[3].Visible = false;
            dataGridView3.Columns[4].Visible = false;
            dataGridView3.Columns[5].Visible = false;
        }

        private void labelNumberNotes2_Click(object sender, EventArgs e)
        {

        }
// Row Enter 
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }
// Find Record through UnMatched
        private void buttonFindRecord_Click(object sender, EventArgs e)
        {
            
            Form80b NForm80b;
            int UniqueIdType = 8;
            string WFunction = "Investigation";
            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator,
                Ta.SesDtTimeStart, Ta.SesDtTimeEnd, WAtmNo, "", 0, WAtmNo, 0, UniqueIdType, WFunction, "", WSesNo);

            NForm80b.ShowDialog();
        }
    }
}

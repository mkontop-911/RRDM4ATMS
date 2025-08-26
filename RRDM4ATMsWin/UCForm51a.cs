using System;
using System.Windows.Forms;
using System.Configuration;
using RRDM4ATMs;
using System.Drawing;

namespace RRDM4ATMsWin
{
    public partial class UCForm51a : UserControl
    {
        // Stavros
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool WSetScreen;

        bool Errors;

        string SelectionCriteria;

        bool ViewWorkFlow;
        string WMode;
        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        bool AudiType;

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

        RRDMGasParameters Gp = new RRDMGasParameters();

        int WFunction;
        int temp;

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public void UCForm51aPar(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();


            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            //RRDMGasParameters Gp = new RRDMGasParameters();
            AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(InOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // 
                    AudiType = true;

                    // SET UP Values if AUDI Type 
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
                if (Usi.ProcessNo == 1)
                {
                    //
                    // UPDATE PHYSICAL FOR THIS ATM and REplenishment Cycle which was done with AUTO. 
                    //
                    //RRDMSessionsPhysicalInspection Phy = new RRDMSessionsPhysicalInspection();
                    int Counter = 0; 
                    bool WSelection = false;
                    Pi.ReadSessionsPhysicalInspectionBySelection(WAtmNo, WSesNo, WSelection);
                    if (Pi.RecordFound)
                    {
                        Counter = Counter + 1; 
                    }
                    WSelection = true;
                    Pi.ReadSessionsPhysicalInspectionBySelection(WAtmNo, WSesNo, WSelection);
                    if (Pi.RecordFound)
                    {
                        Counter = Counter + 1;
                    }

                    if (Counter == 2)
                    {
                        // There is a mixture 
                        // Do nothing
                    }
                    else
                    {
                        // ALL are zero 
                        WSelection = true;
                        Pi.UpdateSessionsPhysicalInspectionRecord(WAtmNo, WSesNo, WSelection);
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
                    labelOut1.Text = Na.Cassettes_1.RemNotes.ToString();
                    progressBar1.Value = Na.Cassettes_1.RemNotes * 100 / Na.Cassettes_1.InNotes;
                    labelFill1.Text = Na.Cassettes_1.InNotes.ToString();

                }
                else
                {
                    labelOut1.Hide();
                    labelFill1.Hide();
                }

                if (Na.Cassettes_2.InNotes > 0)
                {
                    temp = Convert.ToInt32(Na.Cassettes_2.FaceValue);
                    label6.Text = temp.ToString() + " " + Na.Cassettes_2.CurName;
                    labelOut2.Text = Na.Cassettes_2.RemNotes.ToString();
                    progressBar2.Value = Na.Cassettes_2.RemNotes * 100 / Na.Cassettes_2.InNotes;
                    labelFill2.Text = Na.Cassettes_2.InNotes.ToString();
                }
                else
                {
                    labelOut2.Hide();
                    labelFill2.Hide();
                }

                if (Na.Cassettes_3.InNotes > 0)
                {
                    temp = Convert.ToInt32(Na.Cassettes_3.FaceValue);
                    label7.Text = temp.ToString() + " " + Na.Cassettes_3.CurName;
                    labelOut3.Text = Na.Cassettes_3.RemNotes.ToString();
                    progressBar3.Value = Na.Cassettes_3.RemNotes * 100 / Na.Cassettes_3.InNotes;
                    labelFill3.Text = Na.Cassettes_3.InNotes.ToString();
                }
                else
                {
                    labelOut3.Hide();
                    labelFill3.Hide();
                }

                if (Na.Cassettes_4.InNotes > 0)
                {
                    temp = Convert.ToInt32(Na.Cassettes_4.FaceValue);
                    label8.Text = temp.ToString() + " " + Na.Cassettes_4.CurName;
                    labelOut4.Text = Na.Cassettes_4.RemNotes.ToString();
                    progressBar4.Value = Na.Cassettes_4.RemNotes * 100 / Na.Cassettes_4.InNotes;
                    labelFill4.Text = Na.Cassettes_4.InNotes.ToString();
                }
                else
                {
                    labelOut4.Hide();
                    labelFill4.Hide();
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
                if (Pi.RecordFound == false)
                {
                    Pi.InsertPhysicalInspectionRecords(WAtmNo, WSesNo, Ta.LoadedAtRMCycle);

                    Pi.ReadPhysicalInspectionRecordsToFillDataTable(SelectionCriteria);
                }
                //Show
                ShowGridView3();

                //Show Errors 
                string WFilter = " AtmNo ='" + WAtmNo + "' AND SesNo =" + WSesNo + " AND ErrType = 1";
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

                    labelChangeFaceValue.Hide();
                    panelChangeTypes.Hide();
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
                    MessageBox.Show("There is problem with Balancing of Transactions." + Environment.NewLine
                        + "a. TXNS Might missing from Journal.  " + Environment.NewLine
                        + "OR  " + Environment.NewLine
                        + "b. Cassettes might be wrong defined in ATM Cassette definition." + Environment.NewLine
                                    + "If case a. Continue Work."
                                   );

                    // Face Value 

                    labelChangeFaceValue.Show();
                    panelChangeTypes.Show();

                    RRDMGasParameters Gp = new RRDMGasParameters();
                    Gp.ParamId = "206";
                    comboBox1.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
                    comboBox1.DisplayMember = "DisplayValue";

                    Gp.ParamId = "206";
                    comboBox2.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
                    comboBox2.DisplayMember = "DisplayValue";

                    Gp.ParamId = "206";
                    comboBox3.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
                    comboBox3.DisplayMember = "DisplayValue";

                    Gp.ParamId = "206";
                    comboBox4.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
                    comboBox4.DisplayMember = "DisplayValue";

                    textBoxType1.Text = Convert.ToInt32(Na.Cassettes_1.FaceValue).ToString();
                    textBoxType2.Text = Convert.ToInt32(Na.Cassettes_2.FaceValue).ToString();
                    textBoxType3.Text = Convert.ToInt32(Na.Cassettes_3.FaceValue).ToString();
                    textBoxType4.Text = Convert.ToInt32(Na.Cassettes_4.FaceValue).ToString();

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
            if (Pi.RecordFound == false)
            {
                Pi.InsertPhysicalInspectionRecords(WAtmNo, WSesNo,Ta.LoadedAtRMCycle);

                Pi.ReadPhysicalInspectionRecordsToFillDataTable(SelectionCriteria);
            }
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
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter2, WParameter3, WParameter4, WMode, SearchP4);
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
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            int Mode = 1;
            //  string SelectionCriteria =  " WHERE IsMatchingDone = 1 AND Matched = 1 "
            string SelectionCriteria = " WHERE  "
                            + "TerminalId ='" + WAtmNo + "' AND (MetaExceptionId = 55 "
                            + " OR MetaExceptionId = 225 OR MetaExceptionId = 226) ";
            string WSortCriteria = "";
            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, Mode,
                SelectionCriteria, WSortCriteria, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                dataGridView1.Hide();
                textBox1.Text = "NOT FOUND ERRORS IN JOURNAL";
                return;
            }
            //dataGridView1.DataSource = Err.ErrorsTable.DefaultView;

            //if (dataGridView1.Rows.Count == 0)
            //{
            //    dataGridView1.Hide();
            //    textBox1.Text = "NOT FOUND ERRORS IN JOURNAL";
            //    return;
            //}

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

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
        // NEW FACE VALUES 
        private void buttonNewFaceValues_Click(object sender, EventArgs e)
        {
            //Gp.ParamId = "206";
            //comboBox4.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            //comboBox4.DisplayMember = "DisplayValue";

            //textBoxType1.Text = Convert.ToInt32(Na.Cassettes_1.FaceValue).ToString();
            //textBoxType2.Text = Convert.ToInt32(Na.Cassettes_2.FaceValue).ToString();
            //textBoxType3.Text = Convert.ToInt32(Na.Cassettes_3.FaceValue).ToString();
            //textBoxType4.Text = Convert.ToInt32(Na.Cassettes_4.FaceValue).ToString();

            WFunction = 1;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

            if (decimal.TryParse(comboBox1.Text, out Na.Cassettes_1.FaceValue))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid number comboBox1!");

                return;
            }

            if (decimal.TryParse(comboBox2.Text, out Na.Cassettes_2.FaceValue))
            {
            }
            else
            {
                MessageBox.Show(textBox2.Text, "Please enter a valid number comboBox1!");

                return;
            }

            if (decimal.TryParse(comboBox3.Text, out Na.Cassettes_3.FaceValue))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number comboBox1!");

                return;
            }

            if (decimal.TryParse(comboBox4.Text, out Na.Cassettes_4.FaceValue))
            {
            }
            else
            {
                MessageBox.Show(textBox4.Text, "Please enter a valid number comboBox1!");

                return;
            }

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            SetScreen();
        }
        // Show SM LINES
        private void linkLabelSMLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            DateTime NullPastDate = new DateTime(1900, 01, 01);

            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            string SM_SelectionCriteria1 = " WHERE atmno ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + WSesNo
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

            SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, WSesNo, 2);

            if (SM.RecordFound == true)
            {
                Form67_BDC NForm67_BDC;

                int Mode = 7; // Given Fuid and Ruid 
                string WTraceRRNumber = "";
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, SM.Fuid, WTraceRRNumber, WAtmNo
                    , SM.sessionstart_ruid, SM.sessionend_ruid, NullPastDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not found records");
            }

        }
        // Show Cycle Journal Transactions 
        private void linkLabelFromE_Journal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, WSesNo, NullPastDate, 1);

            NForm78D_ATMRecords.Show();
        }
        // Show Presenter 
        private void linkLabelPresenter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            Form200JobCycles_Presenter_Repl NForm200JobCycles_Presenter_Repl;
            NForm200JobCycles_Presenter_Repl = new Form200JobCycles_Presenter_Repl(WOperator, WSignedId, WAtmNo, WSesNo);

            NForm200JobCycles_Presenter_Repl.ShowDialog();
        }
    }
}

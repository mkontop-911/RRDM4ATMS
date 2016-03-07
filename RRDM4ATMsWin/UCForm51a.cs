using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51a : UserControl
    {
        
      //  Form21 NForm21;
      //  Form26 NForm26; // Capture cards 

        // Stavros
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool WSetScreen; 

        bool Errors;

     bool ViewWorkFlow ;
     string WMode;
     // NOTES 
     string Order;

     string WParameter4;
     string WSearchP4;


        // Define Session Balance Record 

        RRDMNotesBalances Na = new RRDMNotesBalances(); // Activate Class 

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); // Activate Class 

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMCaptureCardsClass Cca = new RRDMCaptureCardsClass(); 

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMErrorsClassWithActions Pa = new RRDMErrorsClassWithActions();

        RRDMEmailClass2 Em = new RRDMEmailClass2();

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
            WSetScreen = true; 

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Physical Ispection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
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
            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 13 || Us.ProcessNo == 54 || Us.ProcessNo == 55 || Us.ProcessNo == 56)
            {
                ViewWorkFlow = true;

                if (Cn.TotalNotes == 0)
                {
                    label1.Hide();
                    textBox21.Hide();
                    buttonNotes2.Hide();
                    labelNumberNotes2.Hide();
                }
                else
                {
                    label1.Text = "Read Notes";
                    textBox21.Hide();
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

            textBoxMoneyRem.Text = (Na.Balances1.ReplToRepl/Na.Balances1.OpenBal).ToString("#,##0.00");
            
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

           
            Decimal Percentage = ((Na.Balances1.MachineBal / Na.Balances1.OpenBal)) * 100;
            int Perc = Convert.ToInt32(Percentage);

            textBoxMoneyRem.Text = Perc.ToString(); // Money remain in cassettes 

            // Show  Fraud Data
            Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

            if (checkBox1.Checked == false) checkBox1.Checked = Na.PhysicalCheck1.NoChips;
            if (checkBox2.Checked == false) checkBox2.Checked = Na.PhysicalCheck1.NoCameras;
            if (checkBox3.Checked == false) checkBox3.Checked = Na.PhysicalCheck1.NoSuspCards;
            if (checkBox4.Checked == false) checkBox4.Checked = Na.PhysicalCheck1.NoGlue;
            if (checkBox5.Checked == false) checkBox5.Checked = Na.PhysicalCheck1.NoOtherSusp;

          //  textBox3.Text = Na.PhysicalCheck1.OtherSuspComm;

            errorsTableBindingSource.Filter = "SesNo =" + WSesNo + " AND ErrType = 1"; // Show only the ejournal errors
            this.errorsTableTableAdapter.Fill(this.aTMSDataSet50.ErrorsTable);

            

            if ((Na.Balances1.ReplToRepl - Na.Balances1.MachineBal) == 0 & (Na.Balances2.ReplToRepl - Na.Balances2.MachineBal) == 0 &
                (Na.Balances3.ReplToRepl - Na.Balances3.MachineBal) == 0 & (Na.Balances3.ReplToRepl - Na.Balances3.MachineBal) == 0)
            {
                if (Errors == false)
                {
                    guidanceMsg = " Do the physical ispection and move to next ! ";
                }
                else guidanceMsg = " Do the physical ispection and move to next ! ";
            }
            else
            {
                // Send email to controller 
                string Recipient = "panicos.michael@cablenet.com.cy";

                string Subject = "System Error for ATMNo:" + WAtmNo;

                string EmailBody = "ATM No : " + WAtmNo + " presence a problem with reconciliation from Repl to Repl. at Date and Time: "
                     + DateTime.Now.ToString() +" Repl Cycle No:"+ WSesNo.ToString() + " Error Amount:" + (Na.Balances2.ReplToRepl - Na.Balances2.MachineBal).ToString() ;

                Em.SendEmail(WOperator, Recipient, Subject, EmailBody);

                // 

                guidanceMsg = " THERE IS PROBLEM WITH SYSTEM. Controller was informed through an email.";

                // UPATE ERROR TABLE ABOUT THE RECONCILIATION ERROR 

                Pa.ErrId = 777; // THIS IS THE ID OF THE ERROR FROM REPL TO REPL
                Pa.ReadErrorsIDRecord(Pa.ErrId, WOperator); // READ TO GET THE CHARACTERISTICS

                // INITIALISED WHAT IS NEEDED 

                Pa.CategoryId = "N/A" ;
                Pa.RMCycle = 0 ;
                Pa.MaskRecordId = 0 ; 
            
                Pa.AtmNo = WAtmNo;
                Pa.SesNo = WSesNo;
                Pa.DateTime = DateTime.Now;

                // Find CitNo
                Am.ReadAtmsMainSpecific(WAtmNo);

                Pa.CitId = Am.CitId;

                Pa.OpenErr = true; 

                if ((Na.Balances1.ReplToRepl - Na.Balances1.MachineBal) != 0)
                {
                 //   Pa.CurrCd = Na.Balances1.CurrCd;
                    Pa.CurDes = Na.Balances1.CurrNm;
                    Pa.ErrAmount = Na.Balances1.ReplToRepl - Na.Balances1.MachineBal;
                    Pa.Operator = Am.Operator; 
                    Pa.InsertError(); // INSERT ERROR
                }

                if ((Na.Balances2.ReplToRepl - Na.Balances2.MachineBal) != 0)
                {
                  //  Pa.CurrCd = Na.Balances2.CurrCd;
                    Pa.CurDes = Na.Balances2.CurrNm;
                    Pa.ErrAmount = Na.Balances2.ReplToRepl - Na.Balances2.MachineBal;
                    Pa.Operator = Am.Operator; 
                    Pa.InsertError(); // INSERT ERROR
                }

                if ((Na.Balances3.ReplToRepl - Na.Balances3.MachineBal) != 0)
                {
                 //   Pa.CurrCd = Na.Balances3.CurrCd;
                    Pa.CurDes = Na.Balances3.CurrNm;
                    Pa.ErrAmount = Na.Balances3.ReplToRepl - Na.Balances3.MachineBal;
                    Pa.Operator = Am.Operator; 
                    Pa.InsertError(); // INSERT ERROR
                }

                if ((Na.Balances4.ReplToRepl - Na.Balances4.MachineBal) != 0)
                {
                 //   Pa.CurrCd = Na.Balances4.CurrCd;
                    Pa.CurDes = Na.Balances4.CurrNm;
                    Pa.ErrAmount = Na.Balances4.ReplToRepl - Na.Balances4.MachineBal;
                    Pa.Operator = Am.Operator; 
                    Pa.InsertError(); // INSERT ERROR
                }

            }
        
            // Handle ViewWorkFlow 

            if ( ViewWorkFlow == true ) 
            {
                button1.Hide();
            //    textBox3.ReadOnly = true;
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;

                textBox21.Hide(); 

            //     guidanceMsg = " View only "; THIS IS MOVED TO FORM1
            }
// End of SetScreen 
            WSetScreen = false; 
        }


        //
        // UPDATE PHYSICAL SECURITY CHECK
        //
        private void button1_Click(object sender, EventArgs e)
        {
            
            // Check Notes 
            if (checkBox1.Checked == false || checkBox2.Checked == false || checkBox3.Checked == false
               || checkBox4.Checked == false || checkBox5.Checked == false)
            {
                // How many Notes 
                string WParameter4 = "Physical Ispection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo; 
                string Order = "Ascending";
                string SearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, SearchP4);
               
                if (Cn.RecordFound == true)
                {
                }

                if (Cn.TotalNotes == 0 )
                {
                    MessageBox.Show("YOU MUST FILL NOTES TO DESCRIBE THE OBSERVED ISSUE ",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // STEPLEVEL

                    Us.ReadSignedActivityByKey(WSignRecordNo);

                    if (Us.ReplStep1_Updated == true)
                    {
                        Us.ReplStep1_Updated = false;

                        Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    }
                    return;
                }
                else // There is value = something will be reported 
                {

                }
            }

            // Update Physical Data
            Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

            Na.PhysicalCheck1.NoChips = checkBox1.Checked;
            Na.PhysicalCheck1.NoCameras = checkBox2.Checked;
            Na.PhysicalCheck1.NoSuspCards = checkBox3.Checked;
            Na.PhysicalCheck1.NoGlue = checkBox4.Checked;
            Na.PhysicalCheck1.NoOtherSusp = checkBox5.Checked;

            Na.PhysicalCheck1.OtherSuspComm = "";

            Na.UpdateSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

            button1.Hide(); 

     //       MessageBox.Show("Physical Inspection information is updated.");

              //Form2MessageBox Mb = new Form2MessageBox("Physical Inspection information is updated.");
              //Mb.StartPosition = FormStartPosition.Manual;
              //Mb.Location = new Point(260, 500);
              //Mb.ShowDialog();

            // Update STEP

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.ReplStep1_Updated = true;

            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //if (Us.StepLevel < 1)
            //{
            //    Us.StepLevel = 1;
            //    Us.UpdateSignedInTableStepLevel(WSignRecordNo);
            //}

             // 
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
            string WParameter3 = WAtmNo;
            string WParameter4 = "Physical Ispection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            string SearchP4 = "";            
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode , SearchP4);
            NForm197.ShowDialog();

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Physical Ispection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.ReplStep1_Updated = false;

            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            button1.Show();
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

                Us.ReadSignedActivityByKey(WSignRecordNo);

                Us.ReplStep1_Updated = false;

                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                button1.Show();
            }
        }

        

    }
}

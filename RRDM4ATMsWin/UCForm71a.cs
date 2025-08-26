using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm71a : UserControl
    {
        Form24 NForm24;
        Form72 NForm72; 
        Form73_SendEmail NForm73_SendEmail; 

        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        int Process;

        bool ExistanceOfDiff;
        
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMDepositsClass Dc = new RRDMDepositsClass();

        RRDMSessionsPhysicalInspection Pi = new RRDMSessionsPhysicalInspection(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01); 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public void UCForm71aPar(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
       
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            button3.Hide();
            button4.Hide();
            button5.Hide();
            button6.Hide();
            button1.Hide();

            // UPDATE SESSION TRACES THAT RECONCILIATION HAD STARTED
        }

        // SHOW SCREEN 

        public void SetScreen()
        {
            

            //  readSessionNotesAndValues to get BALANCES AND OTHER INFORMATION  

            Process = 2; // SHOW STATUS WITH ERRORS NO MATTER IF ACTION WAS TAKEN 
         //   Process = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process); // CALL TO MAKE BALANCES AVAILABLE 

            //Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo); // READ PHYSICAL CHECK

            // Initialise Existance of Difference 
            //

            if (Na.BalDiff1.AtmLevel == true || Na.BalDiff1.HostLevel == true || Na.Balances1.ErrOutstanding > 0)
            {
                if (Na.BalDiff1.AtmLevel == true)
                {
                    checkBox1.Checked = true;
                    button3.Show();
                }
                if (Na.BalDiff1.HostLevel == true) checkBox6.Checked = true;
            }
            else
            {
                //  textBox14.Text = "RECONCILED";
                //   button1.Hide();
            }

            Dc.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            if (Dc.DiffInDeposits == true || Dc.DiffInCheques == true)
            {
                checkBox2.Checked = true;
                button4.Show();
            }
            string SelectionCriteria = " ATMNo='" + WAtmNo + "' AND SesNo = " + WSesNo;
            Pi.ReadPhysicalInspectionRecordsToSeeIfAlert(SelectionCriteria);

            if (Pi.InspectionAlert == true)
            {
                checkBox4.Checked = true;
                button6.Show();
            }

            if (Na.NumberOfErrJournal > 0)
            {
                checkBox3.Checked = true;
                button5.Show();
            }

            if (Na.NumberOfErrHost > 0)
            {
                checkBox5.Checked = true;
                button1.Show(); 
            }         

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            // GET REPLENISHMENT INFORMATION

            textBox6.Text = Ta.RespBranch;
            textBox1.Text = Ta.SesDtTimeEnd.ToString();

            Us.ReadUsersRecord(WSignedId);

            textBox2.Text = Us.UserName;
            textBox11.Text = Us.email; 

            // GET ATM TRACES FROM TRACES
          
            Process = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process); // CALL TO MAKE Differences and traces available 

            ExistanceOfDiff = false;

            if (Na.BalDiff1.AtmLevel == true || Na.BalDiff1.HostLevel == true || Na.Balances1.ErrOutstanding > 0)
            {
                ExistanceOfDiff = true;
            }
            else
            {
                //  textBox14.Text = "RECONCILED";
                //   button1.Hide();
            }

            if (Na.BalSets >= 2)
            {

                if (Na.BalDiff2.AtmLevel == true || Na.BalDiff2.HostLevel == true || Na.Balances2.ErrOutstanding > 0)
                {
                    ExistanceOfDiff = true;
                }

            }
            if (Na.BalSets >= 3)
            {
                if (Na.BalDiff3.AtmLevel == true || Na.BalDiff3.HostLevel == true || Na.Balances3.ErrOutstanding > 0)
                {
                    ExistanceOfDiff = true;
                }
            }
            if (Na.BalSets == 4)
            {

                if (Na.BalDiff4.AtmLevel == true || Na.BalDiff4.HostLevel == true || Na.Balances4.ErrOutstanding > 0)
                {
                    ExistanceOfDiff = true;
                }
            }

            // ATM TRACES 
            textBox53.Text = Ta.FirstTraceNo.ToString();
            textBox52.Text = Ta.LastTraceNo.ToString();
            textBox51.Text = Ta.SesDtTimeStart.ToString();
            textBox49.Text = Ta.SesDtTimeEnd.ToString();

            // HOST TARGET 1 LAST TRACE
            label31.Text = "Host Cut Off - " + Na.SystemTargets1.Name; // Traget System 1 
            textBox55.Text = Na.SystemTargets1.LastTrace.ToString(); // Last Trace 
            textBox54.Text = Na.SystemTargets1.DateTm.ToString(); // Date 


            // TARGET 2 LAST TRACE
            label32.Text = "Host Cut Off - " + Na.SystemTargets2.Name; // Traget System 1 
            textBox57.Text = Na.SystemTargets2.LastTrace.ToString(); // Last Trace 
            textBox56.Text = Na.SystemTargets2.DateTm.ToString(); // Date 

            // TARGET 3 LAST TRACE
            if (Na.SystemTargets3.DateTm.Date <= NullPastDate)
            {
                label33.Hide();
                textBox59.Hide();
                textBox58.Hide();
            }
            else // AVAILABLE 
            {
                label33.Text = "Host Cut Off - " + Na.SystemTargets3.Name; // Traget System 1 
                textBox59.Text = Na.SystemTargets3.LastTrace.ToString(); // Last Trace 
                textBox58.Text = Na.SystemTargets3.DateTm.ToString(); // Date 
            }

            // TARGET 4 LAST TRACE
            if (Na.SystemTargets4.DateTm.Date <= NullPastDate)
            {
                label34.Hide();
                textBox62.Hide();
                textBox61.Hide();
            }
            else // AVAILABLE 
            {
                label34.Text = "Host Cut Off - " + Na.SystemTargets4.Name; // Traget System 1 
                textBox62.Text = Na.SystemTargets4.LastTrace.ToString(); // Last Trace 
                textBox61.Text = Na.SystemTargets4.DateTm.ToString(); // Date 
            }
            

            // TARGET 5 LAST TRACE

            if (Na.SystemTargets5.DateTm.Date <= NullPastDate)
            {
                label35.Hide();
                textBox64.Hide();
                textBox63.Hide();
            }
            else // AVAILABLE 
            {
                label35.Text = "Host Cut Off - " + Na.SystemTargets5.Name; // Traget System 1 
                textBox64.Text = Na.SystemTargets5.LastTrace.ToString(); // Last Trace 
                textBox63.Text = Na.SystemTargets5.DateTm.ToString(); // Date 
            }


            if (Na.SystemTargets1.LastTrace < Ta.FirstTraceNo & Na.SystemTargets2.LastTrace < Ta.FirstTraceNo &
                Na.SystemTargets3.LastTrace < Ta.FirstTraceNo & Na.SystemTargets4.LastTrace < Ta.FirstTraceNo &
                Na.SystemTargets5.LastTrace < Ta.FirstTraceNo)
            {
                textBox65.Text = "Host Trans Files Not Received";
                textBox65.Show();
                checkBox7.Checked = true;
            }
            else
            {
                textBox65.Hide(); 
            }
/*
            if (Na.HDtTmEnd < Na.LastHostTranDtTm)
            {
                textBox65.Text = "Host GL Balances Not Received";
                textBox65.Show();
                checkBox7.Checked = true;
            }
 */

            textBox3.Text = Ta.SessionsInDiff.ToString();

            textBox5.Text = Ta.YtdInDiff.ToString(); 

            if (ExistanceOfDiff == true)
            {
               
                guidanceMsg = " There are items to reconcile. Press next to go to Reconciliation Process";
                ChangeBoardMessage(this, new EventArgs());

            }
            else
            {
            
                guidanceMsg = "There are no items to reconcile. No need for other action.";
                ChangeBoardMessage(this, new EventArgs());
            }

            //Us.ReadSignedActivityByKey(WSignRecordNo);

            //if (Us.StepLevel < 1)
            //{
            //    Us.StepLevel = 1;
            //    Us.UpdateSignedInTableStepLevel(WSignRecordNo);
            //}

        }

        // GO TO ERRORS WITH DOUBLE CLICK ( first balance )
        private void textBox66_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bool Replenishment = false;
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND OpenErr =1";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, Na.Balances1.CurrNm, Replenishment, SearchFilter);
           
            NForm24.ShowDialog();
        }

        
        // Call UCForm51b - NOTES
        //
        private void button3_Click_1(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 11; // Enquiry FROM RECONCILIATION 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm72 = new Form72(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            NForm72.ShowDialog();

        }

        // Call UCForm51c - Deposits 
        //

        private void button4_Click_1(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 12; // Enquiry FROM RECONCILIATION 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm72 = new Form72(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            NForm72.ShowDialog();

        }

        // Physical Check 
        //

        private void button6_Click(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 13; // Enquiry FROM RECONCILIATION 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm72 = new Form72(WSignedId, WSignRecordNo, WOperator,  WAtmNo, WSesNo);
            NForm72.ShowDialog();

        }

        // REPORTED IN JOURNAL ERRORS 
        private void button5_Click(object sender, EventArgs e)
        {
            bool Replenishment = true;
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND (ErrType = 1) AND OpenErr =1 AND SesNo <=" + WSesNo;
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator,WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.ShowDialog();
        }

        // REPORTED FROM HOST ERRORS 

        private void button1_Click_2(object sender, EventArgs e)
        {

            bool Replenishment = true;
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND (ErrType = 2) AND OpenErr =1";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.ShowDialog();

        }

        // SEND EMAIL 
        

        private void button7_Click(object sender, EventArgs e)
        {

            NForm73_SendEmail = new Form73_SendEmail(WOperator, textBox11.Text);
            NForm73_SendEmail.Show(); 

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
        
       
    }
}

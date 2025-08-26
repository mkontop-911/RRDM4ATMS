using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form83 : Form
    {
        Form24 NForm24;
        Form72 NForm72;
        Form78 NForm78; 

        Form73_SendEmail NForm73_SendEmail;

     //   public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        int Process;
    //    bool RecordFound;
    //    bool ExistanceOfDiff;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 

        RRDMDepositsClass Dc = new RRDMDepositsClass();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        string WUserBankId;
        string WOperator;

        string WSignedId;
        int WSignRecordNo;
        string WBankId;
     //   bool WPrive;
        string WAtmNo;
        int WSesNo;
        int WAtmTraceNo;

        public Form83(string InSignedId, int InSignRecordNo, string InBankId, 
            string InAtmNo, int InSesNo, int InAtmTraceNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WBankId = InBankId;
           // WPrive = InPrive;
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WAtmTraceNo = InAtmTraceNo; 

            InitializeComponent();

            // ================USER BANK =============================
            Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            WUserBankId = Us.Operator;
            WOperator = WUserBankId;
            // ========================================================

            button3.Hide();
            button4.Hide();
            button5.Hide();
            button6.Hide();
            button1.Hide();

           
            button9.Hide();
          
            button11.Hide();
          

            label22.Text = "Reconciliation Status for ATM : " + WAtmNo + " at Repl. Cycle: " + WSesNo.ToString(); 

            SetScreen(); 
        }

        // SHOW SCREEN 

        public void SetScreen()
        {
            //  readSessionNotesAndValues to get BALANCES AND OTHER INFORMATION  

            Process = 3; // SHOW STATUS WITH ERRORS NO MATTER IF ACTION WAS TAKEN 
            //   Process = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process); // CALL TO MAKE BALANCES AVAILABLE 
            if (Na.RecordFound == false)
            {
                MessageBox.Show("No available Na record!");
                return;
            }

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

            //if (Na.PhysicalCheck1.Problem == true)
            //{
            //    checkBox4.Checked = true;
            //    button6.Show();
            //}

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

            Us.ReadUsersRecord(Ta.Repl1.SignIdRepl);

            textBox2.Text = Us.UserName;
            textBox11.Text = Us.email;

            // GET RECONCILIATION INFORMATION

            textBox3.Text = Ta.RespBranch;
            textBox7.Text = Ta.Recon1.RecFinDtTm.ToString();

            Us.ReadUsersRecord(Ta.Recon1.SignIdReconc);

            textBox5.Text = Us.UserName;
            textBox4.Text = Us.email;
    
            // GET ATM TRACES FROM TRACES


            Process = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 
            Na.ReadSessionsNotesAndValues( WAtmNo, WSesNo, Process); // CALL TO MAKE Differences and traces available 

            // FILL IN FIELDS AFTER RECONCILIATION 

            // Initialise Existance of Difference 
            //
            Ec.ReadAllErrorsTableForCounterReplCycle(WBankId, WAtmNo, WSesNo);

            // Errors for which actions taken 

            textBox10.Text = Ec.ErrUnderAction.ToString();

            // Errors for which actions not taken 
            textBox12.Text = (Ec.NumOfErrorsLess200-Ec.ErrUnderAction).ToString();

            // OUTSTANDING ACTIONS 
            Tc.ReadTransToBePostedTotals(WAtmNo, WSesNo); 

            textBox13.Text = Tc.TotActionsNotTaken.ToString(); 

            // ANY RELATION WITH ERRORS OR ACTIONS 

            textBox14.Text = "NO";

            Ec.ReadErrorsTableSpecificTraceNo(WBankId, WAtmNo, WAtmTraceNo); 

            if ( Ec.RecordFound == true)
            {
                textBox14.Text = "YES";
            }

            Tc.ReadTransToBePostedSpecificTraceNo(WAtmNo, WAtmTraceNo);

            if (Tc.RecordFound == true)
            {
                textBox14.Text = "YES";
            }

            button9.Show();

            button11.Show();

            if (Ta.Recon1.DiffReconcEnd == true)
            {
                textBox9.Text = Ta.Diff1.CurrNm1.ToString();

                textBox8.Text = Ta.Diff1.DiffCurr1.ToString();
            }
            else
            {
                textBox9.Text = "N/A";

                textBox8.Text = "No Diff";
            }

            


        }



        // GO TO ERRORS WITH DOUBLE CLICK ( first balance )
        private void textBox66_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bool Replenishment = false;
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND OpenErr =1";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, Na.Balances1.CurrNm, Replenishment, SearchFilter);

            NForm24.Show();
        }


        // Call UCForm51b - NOTES
        //

        private void button3_Click(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 11; // Enquiry FROM RECONCILIATION 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm72 = new Form72(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo);
            NForm72.Show();
        }

        // Call UCForm51c - Deposits 
        //

        private void button4_Click(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 12; // Enquiry FROM RECONCILIATION 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm72 = new Form72(WSignedId, WSignRecordNo, WBankId,  WAtmNo, WSesNo);
            NForm72.Show();
        }

        // Physical Check 
        //

        private void button6_Click_1(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 13; // Enquiry FROM RECONCILIATION 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm72 = new Form72(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo);
            NForm72.Show();
        }
       
        // REPORTED IN JOURNAL ERRORS 

        private void button5_Click_1(object sender, EventArgs e)
        {
            bool Replenishment = true;
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND(ErrType = 1 )";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.Show();
        }
       
        // REPORTED FROM HOST ERRORS 

        private void button1_Click(object sender, EventArgs e)
        {
            bool Replenishment = true;
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND ( ErrType = 2 ) AND OpenErr =1";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId,  WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.Show();
        }

        // View Outstanding Errors 
        private void button9_Click(object sender, EventArgs e)
        {

            bool Replenishment = true;
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo =" + WSesNo + " AND UnderAction = 0 ";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.Show();

        }

        // SEND EMAIL 


        private void button7_Click(object sender, EventArgs e)
        {

            NForm73_SendEmail = new Form73_SendEmail(WOperator, textBox11.Text);
            NForm73_SendEmail.Show();

        }
        // View Actions for this replenisment Cycle Reconciliation 
        private void button11_Click(object sender, EventArgs e)
        {
            int Mode = 2;
            NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator,  WAtmNo, WSesNo,0, Mode);
            
            NForm78.Show();
           
        }       

    }
}

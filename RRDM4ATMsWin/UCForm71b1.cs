using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm71b1 : UserControl
    {
     //   Form24 NForm24;

        Form174 NForm174;

        public event EventHandler ChangeBoardMessage;
       
        public string guidanceMsg = "No Available Message";
        
        int WProcess;
        int WDifStatus; 
   
        bool ExistanceOfDiff; 
        
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 

        RRDMUsersRecords Us = new RRDMUsersRecords();

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
 
        string WAtmNo;
        int WSesNo;

        public void UCForm71b1Par(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
    
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();      
        }

        // SHOW SCREEN 

        public void SetScreen()
        {
            
            //  readSessionNotesAndValues to get BALANCES AND OTHER INFORMATION  

            WProcess = 4; // INCLUDE IN BALANCES ANY COREECTED ERRORS 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WProcess); // CALL TO MAKE BALANCES AVAILABLE 

            // Initialise Existance of Difference 
            //

            ExistanceOfDiff = false; 

            textBox3.Text = Na.Balances1.CurrNm.ToString();
            textBox4.Text = Na.Balances1.CountedBal.ToString("#,##0.00"); 
            textBox5.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
            //textBox6.Text = Na.Balances1.ReplToRepl.ToString();
            textBox7.Text = Na.Balances1.HostBal.ToString("#,##0.00");
            textBox8.Text = Na.Balances1.HostBalAdj.ToString("#,##0.00");

            textBox9.Text = Na.BalDiff1.Machine.ToString();
            //textBox10.Text = Na.BalDiff1.ReplToRepl.ToString();
        //    textBox11.Text = Na.BalDiff1.Host.ToString();
            textBox12.Text = Na.BalDiff1.HostAdj.ToString();

       //     textBox66.Text = Na.Balances1.NubOfErr.ToString();

        //    textBox66.Text = Na.ErrOutstanding.ToString();

            textBox66.Text = Na.Balances1.ErrOutstanding.ToString();

            if (Na.BalDiff1.AtmLevel == true || Na.BalDiff1.HostLevel == true || Na.Balances1.ErrOutstanding > 0)
            {
                Color Red = Color.Red;
                label20.ForeColor = Red;
              
                label20.Text = "THESE BALANCES HAVE TO RECONCILED";
                ExistanceOfDiff = true;
                button1.Show();
                button5.Hide(); 
            }
            else
            {
                Color Blue = Color.Blue;
                label20.ForeColor = Blue;
                label20.Text = "THESE BALANCES RECONCILED";
                ExistanceOfDiff = false;
                button1.Hide();
                // If Errors open , under action then
                if (Na.Balances1.NubOfErr > 0)
                {
                    button5.Show(); 
                }
            }

            if (Na.BalSets >= 2)
            {
                textBox25.Text = Na.Balances2.CurrNm.ToString();
                textBox24.Text = Na.Balances2.CountedBal.ToString();
                textBox23.Text = Na.Balances2.MachineBal.ToString();
                //textBox22.Text = Na.Balances2.ReplToRepl.ToString();
                textBox21.Text = Na.Balances2.HostBal.ToString();
                textBox19.Text = Na.Balances2.HostBalAdj.ToString();

                textBox18.Text  = Na.BalDiff2.Machine.ToString();
                //textBox17.Text = Na.BalDiff2.ReplToRepl.ToString();
             //   textBox16.Text = Na.BalDiff2.Host.ToString();
                textBox13.Text = Na.BalDiff2.HostAdj.ToString();

                textBox67.Text = Na.Balances2.ErrOutstanding.ToString();          

                if (Na.BalDiff2.AtmLevel == true || Na.BalDiff2.HostLevel == true || Na.Balances2.ErrOutstanding > 0)
                {
                    label27.Text = "THESE BALANCES HAVE TO RECONCILED";
                    ExistanceOfDiff = true;
                    button2.Show();
                    button6.Hide();
               //     button2.Show();
                }
                else
                {
                    label27.Text = "THESE BALANCES ARE RECONCILED";
                    ExistanceOfDiff = false;
                    button2.Hide();
                    // If Errors open , under action then
                    if (Na.Balances2.NubOfErr > 0)
                    {
                        button6.Show();
                    }
                }
            }
            if (Na.BalSets >= 3)
            {
                textBox36.Text = Na.Balances3.CurrNm.ToString();
                textBox35.Text = Na.Balances3.CountedBal.ToString();
                textBox34.Text = Na.Balances3.MachineBal.ToString();
                textBox33.Text = Na.Balances3.ReplToRepl.ToString();
                textBox32.Text = Na.Balances3.HostBal.ToString();
                textBox31.Text = Na.Balances3.HostBalAdj.ToString();

                textBox30.Text = Na.BalDiff3.Machine.ToString();
                textBox29.Text = Na.BalDiff3.ReplToRepl.ToString();
//textBox28.Text = Na.BalDiff3.Host.ToString();
                textBox27.Text = Na.BalDiff3.HostAdj.ToString();

                textBox68.Text = Na.Balances3.NubOfErr.ToString();

                if (Na.BalDiff3.AtmLevel == true || Na.BalDiff3.HostLevel == true || Na.Balances3.ErrOutstanding > 0)
                {
                    label28.Text = "THESE BALANCES HAVE TO RECONCILED";
                    ExistanceOfDiff = true;
                    button3.Show();
                }
                else
                {
                    label28.Text = "THESE BALANCES ARE RECONCILED";
                    button3.Hide();
                }
            }
            if (Na.BalSets == 4)
            {
                textBox47.Text = Na.Balances4.CurrNm.ToString();
                textBox46.Text = Na.Balances4.CountedBal.ToString();
                textBox45.Text = Na.Balances4.MachineBal.ToString();
                textBox44.Text = Na.Balances4.ReplToRepl.ToString();
                textBox43.Text = Na.Balances4.HostBal.ToString();
                textBox42.Text = Na.Balances4.HostBalAdj.ToString();

                textBox41.Text = Na.BalDiff4.Machine.ToString();
                textBox40.Text = Na.BalDiff4.ReplToRepl.ToString();
             //   textBox39.Text = Na.BalDiff4.Host.ToString();
                textBox38.Text = Na.BalDiff4.HostAdj.ToString();

                textBox68.Text = Na.Balances4.NubOfErr.ToString();

                if (Na.BalDiff4.AtmLevel == true || Na.BalDiff4.HostLevel == true || Na.Balances4.ErrOutstanding > 0)
                {
                    label29.Text = "THESE BALANCES HAVE TO RECONCILED";
                    ExistanceOfDiff = true;
                    button4.Show();
                }
                else
                {
                    label29.Text = "THESE BALANCES ARE RECONCILED";
                    button4.Hide();
                }
            }

            if (Na.BalSets == 1)
            {
                panel3.Hide();
                panel4.Hide();
                panel5.Hide();
                label27.Hide();
                label28.Hide();
                label29.Hide(); 
            }
            if (Na.BalSets == 2)
            {
                panel4.Hide();
                panel5.Hide();
                label28.Hide();
                label29.Hide(); 
            }
            if (Na.BalSets == 3)
            {
                panel5.Hide();
                label29.Hide(); 
            }

            if (ExistanceOfDiff == true)
            {
                if (Na.BalSets > 1 )
                    guidanceMsg = " THERE ARE ITEMS TO RECONCILE! ";
                if (Na.BalSets == 1)
                    guidanceMsg = " THERE ARE ITEMS TO RECONCILE! ";
            }
            else
            {
                guidanceMsg = " THERE ARE NO ITEMS TO RECONCILE. NO NEED FOR OTHER ACTION  ";
            }

            if (Na.ErrOutstanding > 0 & Na.ErrJournalThisCycle == 0 & Na.BalDiff1.AtmLevel == false & Na.BalDiff1.HostLevel == false)
            {
                guidanceMsg ="There are old outstanding errors for reconciliation";
            }

            ChangeBoardMessage(this, new EventArgs());

            //Us.ReadSignedActivityByKey(WSignRecordNo);

         
            //    Us.StepLevel = 2;
            //    Us.UpdateSignedInTableStepLevel(WSignRecordNo);
                
                ReadIfDifferencesAndSetStatus(WAtmNo, WSesNo);

                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo); 
                
                Ta.Recon1.WFlowDifStatus = WDifStatus;

                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo); 
                

        }

        

        // GO TO RECONCILIATION for First Currency 

        private void button1_Click_1(object sender, EventArgs e)
        {
            int ReconcBalance = 1;
            NForm174 = new Form174(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, ReconcBalance);
            NForm174.FormClosed +=NForm174_FormClosed;
            NForm174.Show();
        }

        // REVISIT ERRORS 
        private void button5_Click(object sender, EventArgs e)
        {
            int ReconcBalance = 1;
            NForm174 = new Form174(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, ReconcBalance);
            NForm174.FormClosed += NForm174_FormClosed;
            NForm174.Show();
        }

        // Revisit errors for second currency label
        private void button6_Click(object sender, EventArgs e)
        {

            int ReconcBalance = 1;
            NForm174 = new Form174(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, ReconcBalance);
            NForm174.FormClosed += NForm174_FormClosed;
            NForm174.Show();

        }

        // SHOW BALANCES UPON RETURN 
        void NForm174_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetScreen();
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56) guidanceMsg = "View only";
            ChangeBoardMessage(this, new EventArgs()); 
        }

        // GO TO RECONCILIATION for Second  Currency 

        private void button2_Click(object sender, EventArgs e)
        {
            int ReconcBalance = 2;
            NForm174 = new Form174(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, ReconcBalance);
            NForm174.FormClosed += NForm174_FormClosed;
            NForm174.Show();

        }
       
        // GO TO RECONCILIATION for third  Currency 

        private void button3_Click(object sender, EventArgs e)
        {
            int ReconcBalance = 3;
            NForm174 = new Form174(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, ReconcBalance);
            NForm174.FormClosed += NForm174_FormClosed;
            NForm174.Show();
        }

       
        // GO TO RECONCILIATION for forth Currency 

        private void button4_Click(object sender, EventArgs e)
        {
            int ReconcBalance = 4;
            NForm174 = new Form174(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, ReconcBalance);
            NForm174.FormClosed += NForm174_FormClosed;
            NForm174.Show();

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

        //
        // FIND DIFFERENCE STATUS TO UPDATE  
        //
        
            private void ReadIfDifferencesAndSetStatus(string InAtmNo, int InSesNo)
                 {

            //  readSessionNotesAndValues to get BALANCES AND OTHER INFORMATION  

            WProcess = 4; // INCLUDE IN BALANCES ANY COREECTED ERRORS 

            Na.ReadSessionsNotesAndValues(InAtmNo, InSesNo, WProcess); // CALL TO MAKE BALANCES AVAILABLE 

            if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false)
            {
                //   NO DIFFERENCE AT ATM AND HOST 
                //   MessageBox.Show(" NO NEED TO GO TO RECONCILIATION PROCESS ");
                WDifStatus = 1; // Everything reconcile 

            }
            else  // There is Difference 
            {
                
                //  There is difference you must go to reconciliation process 
                if (Na.DiffAtAtmLevel == true)
                {
                    //   MessageBox.Show(" MACHINE DIFFERENCES GO TO RECONCILIATION");
                    WDifStatus = 2; // Machine Difference  

                }
                //  There is difference you must go to reconciliation process 
                if (Na.DiffAtAtmLevel == true & Na.DiffAtHostLevel == true)
                {
                    //   MessageBox.Show(" DIFFERENCES AT MACHINE AND HOST GO TO RECONCILIATION");
                    WDifStatus = 3; // All Machine and Host Differences 
                }
                //  There is difference you must go to reconciliation process 
                if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == true)
                {
                //    MessageBox.Show(" ERRORS ONLY DIFFERENCE. GO TO RECONCILIATION PROCESS ");
                    WDifStatus = 4; // Errors but NO differences  
                }
                //  There is difference you must go to reconciliation process 
                if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == true)
                {
                    // MessageBox.Show(" HOST DIFFERENCE GO TO RECONCILIATION ");
                    WDifStatus = 5; // Host Only differences  
                }
            }

        }

    }
}

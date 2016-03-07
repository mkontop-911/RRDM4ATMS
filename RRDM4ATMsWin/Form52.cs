using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Configuration;

//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form52 : Form
    {

        Form51 NForm51; // Go to next cash recomendation for replenishment 

        Form62 NForm62; // Show Transactions 
      
        string SingleChoice; 
        string WAtmNo;
        int WSesNo;

        int WRow1;
        int WRow2; 

        int WProcessMode; 
 
        int Action; 
        bool RecordFound;
   
        int CurrentSessionNo;
   
        int LastSesNo;
        int SessionsInDiff;
        int PreSes; // Previous to last 
        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        DateTime FromDate; 
        DateTime ToDate;

        string Gridfilter;

        string WBankId;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();
      
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMNotesBalances Na = new RRDMNotesBalances();

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); 

        RRDMReconcCountersClass Rc = new RRDMReconcCountersClass();

        RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        RRDMRightToAccessAtm Ra = new RRDMRightToAccessAtm();

        RRDMAtmsClass Aa = new RRDMAtmsClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

   //     string WUserBankId;

    //    ClassCashInOut Ct = new ClassCashInOut();

        RRDMAllowedAtmsAndUpdateFromJournal Aj = new RRDMAllowedAtmsAndUpdateFromJournal(); 

 //       int WCitId; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
      //  bool WPrive;
        int WAction; 

        public Form52(string InSignedId, int InSignRecordNo, string InOperator, int Action)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
    
            WAction = Action;
         
            InitializeComponent();   

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            //TEST
            dateTimePicker1.Value = new DateTime(2014, 02, 12);
            dateTimePicker2.Value = new DateTime(2014, 02, 13);
            WRow1 = 1; 

            // WAction codes 
            // 1 : Replenishment of ATMs  => This functionality has gone in Form152
            // 2 : Rconciliation of individual ATms => This functionality has gone in Form152
            // 3 : Captured Cards Management => This functionality has gone in Form152
            // 4 : Manage Deposits => This functionality has gone in Form152
            // 5 : Repl Group of Atms 
            // 7 : My transactions 
            // 8 : Calculate Money in => This functionality has gone in Form152
            // 11 : Group Reconciliation 

            // Call Procedures to see if serious message

            // Read USER and ATM Table 
            //WAction = 1; // Update Main record AuthUser field with User Applies to single or group of ATMs 

            if (WAction == 5) // REPLENISHMENT  Group of Atms 
            {
              // / ==================ACCESS TO ATMS=========================

                // Read USER and ATM Table 
                // GET TABLE OF ALLOWED ATMS FOR REPLENISH
                string WFunction = "Any";
                Aj.CreateTableOfAccess(WSignedId, WSignRecordNo, WOperator, WFunction);

                // From eJournal update traces and transactions based on table  
                Aj.UpdateLatestEjStatus(WSignedId, WSignRecordNo, WOperator);


                //==================================
                //==================================

                labelStep1.Text = "ATMs Replenishment";
                label5.Hide();
                panel4.Hide();
                label12.Hide();
                panel3.Hide();
           
            }

            if (WAction == 7) // TRANSACTIONS 
            {
                // ==================ACCESS TO ATMS=========================

                // Read USER and ATM Table 
                // GET TABLE OF ALLOWED ATMS FOR REPLENISH
                string WFunction = "Any";
                Aj.CreateTableOfAccess(WSignedId, WSignRecordNo, WOperator, WFunction);

                // From eJournal update traces and transactions based on table  
                Aj.UpdateLatestEjStatus(WSignedId, WSignRecordNo, WOperator);


                //==================================
                //==================================

                labelStep1.Text = "Show Transactions per Repl.Cycle, Period or Specific";
              
                button4.Text = "Show";
                label5.Show();
                panel4.Show();
                label12.Show();
                panel3.Show();
           
            }

           
        }

        private void Form52_Load(object sender, EventArgs e)
        {
                      
            // LOAD FIRST DATA GRID ( ATMS MAIN)
            
          
                Gridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'";
              

                atmsMainBindingSource.Filter = Gridfilter;
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                this.atmsMainTableAdapter.Fill(this.aTMSDataSet30.AtmsMain);

                if (dataGridView1.Rows.Count == 0)
                {
                    Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                    MessageForm.ShowDialog();

                    this.Dispose();
                    return;
                }

                // SET ROW Selection POSITIONING 
               dataGridView1.Rows[WRow1].Selected = true;
               dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
  
        }

        // ROW ENTER
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;

            // CHECK IF ATM IS ACTIVE 
            Am.ReadAtmsMainSpecific(WAtmNo);

            WBankId = Am.BankId;

            if (Am.ProcessMode == -2)
            {
                label17.Hide();
                dataGridView2.Hide();
                textBox12.Hide();
                textBox2.Hide();
                button4.Hide();
                label9.Hide();

                textBoxMsgBoard.Text = "This ATM is not active yet! It will become automatically active when money is added. ";

                MessageBox.Show("This ATM is not active yet!");

                return;
            }
            else
            {
                label17.Show();
                dataGridView2.Show();
                textBox12.Show();
                textBox2.Show();
                button4.Show();
                label9.Show();
            }

            label17.Text = "REPL. CYCLE/s FOR ATM : " + WAtmNo; 

         

            if (WAction == 7) // FOR TRANSACTIONS
            {
                label5.Text = "TRANSACTIONS FOR ATM: " + WAtmNo.ToString(); 
            }

            string filter = "AtmNo ='" + WAtmNo + "' AND (ProcessMode = -1 OR ProcessMode = 0 OR ProcessMode = 1 OR ProcessMode = 2 OR ProcessMode = 3)";

            sessionsStatusTracesBindingSource.Filter = filter;
            dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);
            this.sessionsStatusTracesTableAdapter.Fill(this.aTMSDataSet34.SessionsStatusTraces);

            // Read and update 
            Am.ReadAtmsMainSpecific(WAtmNo);

            if (DateTime.Today > Am.NextReplDt &  WAction == 5)
            {
                textBoxMsgBoard.Text = " Replenishment Has been delayed . Please take action ";
            }
            else
            {
                textBoxMsgBoard.Text = " Choose combination of ATM and Repl Cycle and proceed ";
            }

            if (WAction == 7) // My Transactions    
            {
                textBoxMsgBoard.Text = "Make your choice and press Show Button";
            }
            if (WAction == 8) // My Transactions    
            {
                textBoxMsgBoard.Text = "Make your choice and press Proceed";
            }
        }

        // SESSION TRACES SELECTION 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSesNo = (int)rowSelected.Cells[0].Value;

            textBox2.Text = WSesNo.ToString();

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            if (RecordFound == true)
            {
            }

            WProcessMode = Ta.ProcessMode; 

            if (WProcessMode == -1)
            {
                textBox12.Text = "Pending Repl Cycle";
            }
            if (WProcessMode == 0)
            {
                textBox12.Text = "Atm ready for Repl Workflow";
            }

            if (WProcessMode == 1)
            {
                if (WAction == 1)
                {
                    Color Red = Color.Red;
                    textBox12.ForeColor = Red;
                    textBox12.Text = "Atm has already passed the Repl Workflow for this Repl Cycle";
                }
                else
                {
                    Color Black = Color.Black;
                    textBox12.ForeColor = Black;
                    textBox12.Text = "Atm ready for Reconciliation for this Repl Cycle";
                }
            }
            if (WProcessMode == 2 || WProcessMode == 3)
            {
                if (WAction == 2 || WAction == 1)
                {
                    Color Red = Color.Red;
                    textBox12.ForeColor = Red;
                    textBox12.Text = "Atm has already passed the Reconciliation process for this Repl Cycle";
                }
                else
                {
                    Color Black = Color.Black;
                    textBox12.ForeColor = Black;
                    textBox12.Text = "Atm has already passed the Reconciliation process for this Repl Cycle";
                }
                
            }
           
        }

        // AN ATM NO IS CHOSEN - SHOW ONLY THIS ATM IF USER HAS THE RIGHT TO ACCESS 

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Choose An ATM number Please");
                return;
            }
            else
            {
                WAtmNo = textBox3.Text;           
            }

            // See if this ATM belongs to the user            

            Ra.CheckRightToAccessAtm(WSignedId, WAtmNo);

            if (Ra.RecordFound == false)
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO ACCESS THIS ATM ");
                return;
            }

            string Atmfilter = " AtmNo = '" + WAtmNo + "'";

            atmsMainBindingSource.Filter = Atmfilter;

            this.atmsMainTableAdapter.Fill(this.aTMSDataSet30.AtmsMain);

            if (DateTime.Today > Am.NextReplDt)
            {
                textBoxMsgBoard.Text = " Replenishment Has been delayed . Please take action ";
            }

            if (DateTime.Today < Am.NextReplDt)
            {
                // Message for future replenishement 
            }

        }
        // REFRESHED 
        private void button3_Click_1(object sender, EventArgs e)
        {

       
                string errfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'";

                atmsMainBindingSource.Filter = errfilter;
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                this.atmsMainTableAdapter.Fill(this.aTMSDataSet30.AtmsMain);
     
        }

        // Proceed button was pressed
        // GO TO NEXT - REPLENISHMENT OR MANAGE DEPOSITS OR RECONCILIATION 
        // OR CAPTURED CARDS OR Transactions 
        //

        private void button4_Click(object sender, EventArgs e)
        {
            Am.ReadAtmsMainSpecific(WAtmNo);
            if (Am.ProcessMode == -2)
            {
                MessageBox.Show("This ATm is not active!");
                return;
            }
            //Keep Row Selection positioning 
            WRow1 = dataGridView1.SelectedRows[0].Index;
            WRow2 = dataGridView2.SelectedRows[0].Index; 

            if (String.IsNullOrEmpty(textBox2.Text))
            {
                //    SelectAtmMain(AtmNo); // Find what is the running SessionNo for this ATM
            }
            else
            {
                CurrentSessionNo = int.Parse(textBox2.Text);
            }
            // REPLENISHMENT PROCESS CODES
            // SINGLES
            // WFunction = 1 Normal branch ATM
            // 25 Off site ATM = cassettes are ready and go in ATM
            // 26 Belongs to external - CIT 
            // GROUPS
            // 5 Normal Group belonging to Bank . 
            // 30 Offsite Group belonging to Bank
            // 31 Group belonging to - CIT 

           
            //
            // REplenishment for Group
            //
            if (WAction == 5 )
            {
                // User Have Group/s 
            
                Aa.ReadAtm(WAtmNo); // Read ATM

                Ua.ReadUsersAccessAtmTableSpecific(WSignedId, "", Aa.AtmsReplGroup);

                if (Ua.RecordFound == false)
                {
                    MessageBox.Show(" YOU ARE NOT AUTHORISED TO REPLENISH THIS ATM ");
                    return;
                }

                // SHOW When WAS LAST REPLENISHed

               

                if (Am.NextReplDt.Date > DateTime.Today)
                {

                    if (MessageBox.Show("NEXT REPLENISHMENT DATE IS GREATER THAN TODAY. Do you want to proceed with Replenishment?"
                        , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                  == DialogResult.Yes)
                    {
                        // Process No Updating 

                        string LastRepl = Am.LastReplDt.ToString();

                        if (MessageBox.Show("LAST REPLENISHMENT WAS DONE ON " + LastRepl + " DO YOU WANT TO PROCEED WITH THIS ONE?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                      == DialogResult.Yes)
                        {
                            // Process No Updating 
                         
                            Us.ReadSignedActivityByKey(WSignRecordNo);
                            // GROUPS
                            // 5 Normal Group belonging to Bank . 
                            // 30 Offsite Group belonging to Bank  ????? 
                            // 31 Group belonging to external like GROUP 4 OR CitId>0
                            Ac.ReadAtm(WAtmNo);
                            Us.ReadUsersRecord(WSignedId);
                            if (Ac.OffSite == true & Ac.CitId == "")
                            {
                                Us.ProcessNo = 30; // ?????? 
                            }
                            if ((Ac.CitId != ""))
                            {
                                Us.ProcessNo = 31;
                            }
                            if (Ac.OffSite == false & Ac.CitId == "")
                            {
                                Us.ProcessNo = 5; // NORMAL AT BRANCH
                            }
                            // 5 for internal group and 30 for OffSite ATM and 31 for external 

                            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                            if (Us.ProcessNo == 30 || Us.ProcessNo == 25)
                            {
                                MessageBox.Show("MSG667: Process codes 30 and 25 = off-site ATMs note available in Form51 yet");
                            }
                             
                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo); 
                            if (WProcessMode == -1)
                            {
                                MessageBox.Show("MSG670: Process Mode = 1 ... this means that not all information is available"
                                            + " for replenishement .. Supervisor Mode cassette data are missing ");
                                return; 
                            }

                            Ta.FindNextReplCycleId(WAtmNo);
                            if (WProcessMode == 0 & WSesNo != Ta.NextReplNo)
                            {
                                MessageBox.Show("MSG671: Choose the right Repl Number. Choose the : " + Ta.NextReplNo.ToString());
                                return;
                            }

                            Ta.FindNextReplCycleId(WAtmNo);
                            if (WProcessMode == 0 & WSesNo != Ta.NextReplNo)
                            {
                                MessageBox.Show("MSG672: Choose the right Repl Number. Choose the : " + Ta.NextReplNo.ToString());
                                return;
                            } 

                            NForm51 = new Form51(WSignedId, WSignRecordNo, WBankId, WAtmNo, CurrentSessionNo);
                            NForm51.FormClosed += NForm51_FormClosed;
                            NForm51.ShowDialog(); ;
                         
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    // Not Greated than today 
                    string LastRepl = Am.LastReplDt.ToString();

                    if (MessageBox.Show("LAST REPLENISHMENT WAS DONE ON " + LastRepl 
                                          + " DO YOU WANT TO PROCEED WITH THIS ONE?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                           == DialogResult.Yes)
                    {
                        // Process No Updating 

                        Us.ReadSignedActivityByKey(WSignRecordNo);
                        // GROUPS
                        // 5 Normal Group belonging to Bank . 
                        // 30 Offsite Group belonging to Bank
                        // 31 Group belonging to external like GROUP 4 
                        Ac.ReadAtm(WAtmNo);

                        Us.ReadUsersRecord(WSignedId); // Find CIT Company of User 
                        Us.ReadUsersRecord(Us.CitId); // Read the details of CIT Company 

                        if (Ac.OffSite == true & Us.UserType == "Operator Entity") // Our Own User but ATM is Offsite 
                        {
                            Us.ProcessNo = 30;
                        }
                        if ((Ac.OffSite == true & Us.UserType == "CIT Company") || (Ac.OffSite == false & Us.UserType == "CIT Company"))
                        {
                            Us.ProcessNo = 31;
                        }
                        if (Ac.OffSite == false & Us.UserType == "Operator Entity") // Our own User and ATM is at the Baranch 
                        {
                            Us.ProcessNo = 5; // NORMAL AT BRANCH
                        }
                        // 5 for internal group and 30 for OffSite ATM and 31 for external 

                        Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                       
                        if (WProcessMode == -1)
                        {
                            MessageBox.Show("MSG675: Process Mode = 1 ... this means that not all information is available"
                                        + " for replenishement .. Supervisor Mode cassette data are missing ");
                            return; 
                        }


                        Ta.FindNextReplCycleId(WAtmNo);
                        if (WProcessMode == 0 & WSesNo != Ta.NextReplNo)
                        {
                            MessageBox.Show("MSG676: Choose the right Repl Number. Choose the : " + Ta.NextReplNo.ToString());
                            return;
                        } 

                        NForm51 = new Form51(WSignedId, WSignRecordNo, WBankId, WAtmNo, CurrentSessionNo);
                        NForm51.FormClosed += NForm51_FormClosed;
                        NForm51.ShowDialog(); ;
                       
                    }
                    else
                    {
                        return;
                    }
                }

               
            }

         

            // GO TO TRANSACTIONS FOR THIS SESSION 

            if (WAction == 7)
            {
                Action = 21;
                FromDate = NullPastDate;
                ToDate = NullPastDate; 
                
                NForm62 = new Form62(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, Action,
                    FromDate, ToDate, SingleChoice);
                NForm62.ShowDialog(); ;
                
            }
           
        }

     

        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form52_Load(this, new EventArgs());
            //  dataGridView1 will be refresed

        }


      

        // SHOW THE TRANSACTIONS BY PERIOD 
        private void button5_Click(object sender, EventArgs e)
        {
            Am.ReadAtmsMainSpecific(WAtmNo);
            if (Am.ProcessMode == -2)
            {
                MessageBox.Show("This ATm is not active!");
                return;
            }
            if (WAction == 7)
            {
                Action = 22;

                FromDate = dateTimePicker1.Value;
                ToDate = dateTimePicker2.Value;

                string sqlFormattedDate1 = FromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sqlFormattedDate2 = ToDate.ToString("yyyy-MM-dd HH:mm:ss");

                NForm62 = new Form62(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, Action,
                    FromDate, ToDate, SingleChoice);
                NForm62.ShowDialog(); ;
             //   this.Dispose();
            }


        }

        // SHOW SINGLE TRANSACTION 
        private void button7_Click(object sender, EventArgs e)
        {
            Am.ReadAtmsMainSpecific(WAtmNo);
            if (Am.ProcessMode == -2)
            {
                MessageBox.Show("This ATm is not active!");
                return;
            }
            if (radioButton1.Checked == false & radioButton2.Checked == false & radioButton3.Checked == false)
            {
                MessageBox.Show(" Make your choice, enter an account no and then press the button");
                return;
            }
            else
            {
                if (String.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Enter the number please", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    SingleChoice = textBox1.Text;

                    //TEST
                    WAtmNo = "AB104"; // NEEDED FOR THE JOURNAL 
                }
                if (radioButton1.Checked == true) Action = 23; // Card no
                if (radioButton2.Checked == true) Action = 24; // Account no
                if (radioButton3.Checked == true)
                {
                    int TraceNo;
                    if (int.TryParse(textBox1.Text, out TraceNo))
                    {
                        SingleChoice = TraceNo.ToString();
                      
                    }
                    else
                    {
                        MessageBox.Show(textBox1.Text, "Please enter a valid trace number!");
                        return;
                    }
                    Action = 25; // Trace no
                }
            }

            NForm62 = new Form62(WSignedId, WSignRecordNo, WBankId,  WAtmNo, WSesNo, Action,
                FromDate, ToDate, SingleChoice);
            NForm62.ShowDialog(); ;

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            textBox1.Text = ""; 
        }

        
        //
        // READ Last SESSION TRACE to find last Session No 
        //
    //
private void ReadSessionsStatusTraces(string AtmNo)
        { 
     
            string SqlString =
                "SELECT * "
                + " FROM [dbo].[SessionsStatusTraces] "
                + " WHERE AtmNo =" + AtmNo + " and Last = 1 ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            // Assign Values
                            RecordFound = true;    

                            LastSesNo = (int)rdr["SesNo"];
                            PreSes = (int)rdr["PreSes"];
                            SessionsInDiff = (int)rdr["SessionsInDiff"];

                        }
                        // Close Reader
                        rdr.Close();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    //  string exception = ex.ToString();
                    MessageBox.Show(ex.ToString());
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }
        }

        // Show SESSION STATUS 
private void button3_Click(object sender, EventArgs e)
{
    

    if (String.IsNullOrEmpty(textBox2.Text))
    {

     //   SelectAtmMain(AtmNo); // Find what is the running SessionNo for this ATM
    }
    else
    {
        CurrentSessionNo = int.Parse(textBox2.Text);
    }

    
  //  NForm31 = new Form31(WAtmNo);
 //   NForm31.Show();
    
}

        // UNDO LAST 




  

private void tableLayoutPanelMain_Paint(object sender, PaintEventArgs e)
{

}


  

private void button9_Click(object sender, EventArgs e)
{


}

private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
{

}
// Finish 
private void button1_Click(object sender, EventArgs e)
{
    this.Dispose(); 
}
// Testing 
        // card
private void radioButton1_CheckedChanged(object sender, EventArgs e)
{
    textBox1.Text = "450653******7072";
}
// account number
private void radioButton2_CheckedChanged(object sender, EventArgs e)
{
    textBox1.Text = "013600004883";
}
// Trace 
private void radioButton3_CheckedChanged(object sender, EventArgs e)
{
    textBox1.Text = "10042990";
}

private void button10_Click(object sender, EventArgs e)
{

}

        /*
                        if (SelModeB == 8 || SelModeB == 9 || SelModeB == 10)
                        {
                            // Check if Number is entered 
                            if (String.IsNullOrEmpty(textBox1.Text))
                            {
                                if (SelModeB == 8)
                                {
                                    MessageBox.Show("Enter Data for card such as 4506531111117072");
                                    textBox1.Text = "4506531111117072";
                                }
                                if (SelModeB == 9)
                                {
                                    MessageBox.Show("Enter Data for Account such as 012801038482");
                                    textBox1.Text = "012801038482";
                                }
                                if (SelModeB == 10)
                                {
                                    MessageBox.Show("Enter Data for TraceNumber such as 10043180 ");
                                    textBox1.Text = "10043180";
                                }

                                return;
                            }
                            else // There is value = something will be reported 
                            {
                                EnteredNumber = textBox1.Text;
                            }
                        }
                        if (SelModeB == 10)
                        {

                            if (int.TryParse(EnteredNumber, out WTraceNo))
                            {
                                EnteredNumber = WTraceNo.ToString();
                            }
                            else
                            {
                                MessageBox.Show(textBox1.Text, "Please enter a valid Trace number!");
                                return;
                            }
                        }
                      */

    }
}

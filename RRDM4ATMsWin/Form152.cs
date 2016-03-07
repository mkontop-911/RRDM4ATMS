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
    public partial class Form152 : Form
    {

        Form51 NForm51; // Go to next cash recomendation for replenishment 

        Form71 NForm71; // Reconciliation 

        Form26 NForm26; // Go to capture cards 

        Form38 NForm38; // Manage Deposis 

     //   string SingleChoice;
        string WAtmNo;
        int WSesNo;

        int WRow1;
        int WRow2;

     //   string WUserBankId;
     //   string WAccessToBankTypes;
     //   int WSecLevel;
      //   string WBankId;
 //       bool WPrive;

        int WProcessMode;
        //     int WLastTraceNo; 

        //       string MsgFilter;
    //    int Action;
        bool RecordFound;
        //    bool SessionEnd; 

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        string Gridfilter;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
     //   UpdateGrids Ug = new UpdateGrids();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMNotesBalances Na = new RRDMNotesBalances();

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        RRDMReconcCountersClass Rc = new RRDMReconcCountersClass();

        RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        RRDMRightToAccessAtm Ra = new RRDMRightToAccessAtm();

        RRDMAtmsClass Aa = new RRDMAtmsClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        //    ClassCashInOut Ct = new ClassCashInOut();

        RRDMAllowedAtmsAndUpdateFromJournal Aj = new RRDMAllowedAtmsAndUpdateFromJournal();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess(); 

        //       int WCitNo; 



        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WAction;

        public Form152(string InSignedId, int InSignRecordNo, string InOperator, int Action)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WAction = Action;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            // WAction codes 
            // 1 : Replenishment of ATMs
            // 2 : Rconciliation of individual ATms 
            // 3 : Captured Cards Management 
            // 4 : Manage Deposits 
            // 5 : Replenishment of ATMs
            // 8 : Calculate Money in 

            // Call Procedures to see if serious message

            if (WAction == 1) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "ATMs Replenishment";
            }

            if (WAction == 2) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "ATMs Reconciliation";
            }
            if (WAction == 3) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "ATMs Captured Cards";
            }
            if (WAction == 4) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "Navigation Towards Deposits Mgmt";
            }
            if (WAction == 5) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "ATMs Replenishment";
            }
            if (WAction == 8) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "Navigation Towards Money In Need";    
            }

            // ==================ACCESS TO ATMS=========================

                // Read USER and ATM Table 
                // GET TABLE OF ALLOWED ATMS FOR REPLENISH
                string WFunction = "Any";
                Aj.CreateTableOfAccess(WSignedId, WSignRecordNo, WOperator, WFunction);

                // From eJournal update traces and transactions based on table  
                Aj.UpdateLatestEjStatus(WSignedId, WSignRecordNo, WOperator);


        }

        private void Form152_Load(object sender, EventArgs e)
        {
           
            
            // LOAD FIRST DATA GRID ( ATMS MAIN)

                Gridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId +"'";

                atmsMainBindingSource.Filter = Gridfilter;
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                this.atmsMainTableAdapter.Fill(this.aTMSDataSet42.AtmsMain);

                if (dataGridView1.Rows.Count == 0 )
                {
                    Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                    MessageForm.ShowDialog();

                    this.Dispose();
                    return;
                }

                //TEST
                if (WAction == 3 || WAction == 4)
                {
                    WRow1 = 1;
                    dataGridView1.Rows[WRow1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
                }

        }
        // Row Enter 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;

            // CHECK IF ATM IS ACTIVE 
            Am.ReadAtmsMainSpecific(WAtmNo);

            if (Am.ProcessMode == -2)
            {
                label17.Hide();
                panel3.Hide();
                textBoxMsgBoard.Text = "This ATM is not active yet! It will become automatically active when money is added. ";
                MessageBox.Show("This ATM is not active yet!");
                
                return;
            }
            else
            {
                label17.Show();
                panel3.Show();
            }

            label17.Text = "REPL. CYCLE/s FOR ATM : " + WAtmNo; 

            string filter = "AtmNo ='" + WAtmNo + "' AND (ProcessMode = -1 OR ProcessMode = 0 OR ProcessMode = 1 OR ProcessMode = 2 OR ProcessMode = 3)";

            sessionsStatusTracesBindingSource.Filter = filter;
            dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);
            this.sessionsStatusTracesTableAdapter.Fill(this.aTMSDataSet43.SessionsStatusTraces);

           if (WAtmNo == "AB104")
           {
               WRow2 = 1 ;               // SET ROW Selection POSITIONING 
               dataGridView2.Rows[WRow2].Selected = true;
               dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
           }

            // Read and update 
            Am.ReadAtmsMainSpecific(WAtmNo);
            //    textBox2.Text = Am.CurrentSesNo.ToString();

            if (DateTime.Today > Am.NextReplDt & (WAction == 1 || WAction == 5))
            {
                textBoxMsgBoard.Text = " Replenishment Has been delayed . Please take action ";
            }
            else
            {
                textBoxMsgBoard.Text = " Choose combination of ATM and Repl Cycle and proceed ";
            }

            if (WAction == 2) // Reconciliation of individual ATMs   
            {
                textBoxMsgBoard.Text = "Choose combination of ATM and Repl Cycle and proceed ";
            }

            if (WAction == 3) // Capture Cards management 
            {
                textBoxMsgBoard.Text = "Choose combination of ATM and Repl Cycle and proceed ";
            }
            if (WAction == 4) // Deposits  
            {
                textBoxMsgBoard.Text = "Choose combination of ATM and Repl Cycle and proceed ";
            }

        }

        // ON ROW ENTER 

        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSesNo = (int)rowSelected.Cells[0].Value;  

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            WProcessMode = Ta.ProcessMode;
            textBox4.Text = WProcessMode.ToString();

            if (WProcessMode == -1)
            {
                Color Red = Color.Red;
                textBox12.ForeColor = Red;
                textBox12.Text = "Not Ready for Repl Workflow";
                return; 
            }
            if (WProcessMode == 0)
            {
                textBox12.Text = "Ready for Repl Workflow";
            }

            if (WProcessMode == 1)
            {
                if (WAction == 1)
                {
                    Color Red = Color.Red;
                    textBox12.ForeColor = Red;
                    textBox12.Text = "Repl Workflow Done! - Now there is need of Reconciliation";
                }
                else
                {
                    Color Black = Color.Black;
                    textBox12.ForeColor = Black;
                    textBox12.Text = "Ready for Withdrawls and Deposits Reconciliation";
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
     

        // Proceed button was pressed
        // GO TO NEXT - REPLENISHMENT OR MANAGE DEPOSITS OR RECONCILIATION 
        // OR CAPTURED CARDS OR Transactions 
        //

        // Next process 
        private void buttonNext_Click(object sender, EventArgs e)
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

          
            // REPLENISHMENT PROCESS CODES
            // SINGLES
            // WFunction = 1 Normal branch ATM
            // 25 Off site ATM = cassettes are ready and go in ATM
            // 26 Belongs to external - CIT 
            // GROUPS
            // 5 Normal Group belonging to Bank . 
            // 30 Offsite Group belonging to Bank
            // 31 Group belonging to - CIT 

            if (WAction == 1) // REPLENISH NO GROUP 
            {
                // Check LAST RECORD if Already in authorization process

                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Replenishment");

                if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
                {
                    MessageBox.Show("This Replenishment Already has authorization record!" + Environment.NewLine
                                             + "Go to Pending Authorisations process to complete."
                                                              );
                    return;
                }

                // User Does Not Have Groups 
                //TEST
                if (WSignedId == "1005")
                {
                    if (WAtmNo == "AB104")
                    {
                        WAtmNo = "AB104";
                     //   CurrentSessionNo = int.Parse(textBox2.Text);
                    }
                    //TEST
                    if (WAtmNo == "AB102")
                    {
                      //CurrentSessionNo = 3144;
                        WSesNo = 3144;
                    }

                }
               
                if (WSignedId == "500")
                {
                    WAtmNo = "12507";
                 //   CurrentSessionNo = 1122;
                    WSesNo = 1122;
                }

                if (WSignedId == "03ServeUk")
                {
                     if (WAtmNo == "ServeUk102")
                    {
                      //CurrentSessionNo = 3144;
                        WSesNo = 6694;
                    }
                }

                if (WSignedId == "03ServeUk")
                {
                    if (WAtmNo == "ABC501")
                    {
                        WSesNo = 6695;
                    }
                }

                Ua.ReadUsersAccessAtmTableSpecific(WSignedId, WAtmNo, 0);

                if (Ua.RecordFound == false || (Ua.RecordFound == true & Ua.Replenishment == false))
                {
                    MessageBox.Show(" YOU ARE NOT AUTHORISED TO REPLENISH THIS ATM ");
                    return;
                }

                if (WProcessMode == -1)
                {
                    MessageBox.Show("MSG668: Process Mode = -1 ... this means that not all information is available" + Environment.NewLine
                                + " for replenishement .. Supervisor Mode cassette data are missing ");
                    return;
                }

                if (WProcessMode == 1)
                {

                    if (MessageBox.Show("Process Mode = 1 ... This Atm already passed the Repl Workflow." + Environment.NewLine
                        + " Do you want to proceed to workflow?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                    {
                        // If Yes proceed .... 
                    }
                    else
                    {
                        return;
                    }

                }

                //if (WProcessMode == 2 || WProcessMode == 3)
                //{
                    

                //    if (MessageBox.Show("Process Mode = 2 or 3 ... Atm already passed the Replenishement and Reconciliation Workflow." + Environment.NewLine + " Do you want to proceed ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                //         == DialogResult.Yes)
                //    {
                //        // If Yes proceed .... 
                //    }
                //    else
                //    {
                //        return;
                //    }

                //}

                // SHOW When WAS LAST REPLENISHed


                string LastRepl = Am.LastReplDt.ToString();

                if (MessageBox.Show("LAST REPLENISHMENT WAS DONE ON " + LastRepl + Environment.NewLine + Environment.NewLine
                    + " DO YOU WANT TO PROCEED WITH THIS ONE?"
                    , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                              == DialogResult.Yes)
                {
                    // Process No Updating 

                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    // WFunction = 1 Normal branch ATM
                    // 25 Off site ATM = cassettes are ready and go in ATM
                    // 26 Belongs to external 
                    Ac.ReadAtm(WAtmNo);
                    Us.ReadUsersRecord(WSignedId);
                    if (Ac.OffSite == true & Us.UserType == "Employee")
                    {
                        Us.ProcessNo = 25;
                    }
                    if ((Ac.CitId != "1000"))
                    {
                        Us.ProcessNo = 26;
                    }
                    if (Ac.OffSite == false & Ac.CitId == "1000")
                    {
                        Us.ProcessNo = 1; // NORMAL AT BRANCH
                    }

                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


                    //TEST
                    if (WAtmNo == "AB102" || WAtmNo == "12507" || WAtmNo == "AB104" || WAtmNo == "ServeUk102"
                        || WAtmNo == "ABC501" || WAtmNo == "ABC502")
                    {
                        ZeroData();
                        UndoErrors(WAtmNo, WSesNo);

                        if (RecordFound == true)
                        {
                        } 
                    }

                    if (Us.ProcessNo == 30 || Us.ProcessNo == 25)
                    {
                        MessageBox.Show("MSG667: Process codes 30 and 25 = off site ATMs note available in Form51 yet");
                    }


                    Ta.FindNextReplCycleId(WAtmNo);
                    if (WProcessMode == 0 & WSesNo != Ta.NextReplNo)// NextReplNo is the next valid for replenishment 
                    {
                        MessageBox.Show("MSG669: Choose the right Repl Number. Choose the : " + Ta.NextReplNo.ToString());
                        return;
                    }

                    NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    NForm51.FormClosed += NForm51_FormClosed;
                    NForm51.ShowDialog();
                }
                else
                {
                    return;
                }

            }
            //
            // REplenishment for Group
            //
            if (WAction == 5)
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

                Am.ReadAtmsMainSpecific(WAtmNo);

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
                            // 31 Group belonging to external like GROUP 4 OR CitNo>0
                            Ac.ReadAtm(WAtmNo);
                            Us.ReadUsersRecord(WSignedId);
                            if (Ac.OffSite == true & Ac.CitId == "1000")
                            {
                                Us.ProcessNo = 30; // ?????? 
                            }
                            if ((Ac.CitId != "1000"))
                            {
                                Us.ProcessNo = 31;
                            }
                            if (Ac.OffSite == false & Ac.CitId == "1000")
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

                            NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                            NForm51.FormClosed += NForm51_FormClosed;
                            NForm51.ShowDialog();

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

                    if (MessageBox.Show("LAST REPLENISHMENT WAS DONE ON " + LastRepl + " DO YOU WANT TO PROCEED WITH THIS ONE?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
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
                        if (Ac.OffSite == false & Us.UserType == "Operator Entity") // Owr own User and ATM is at the Baranch 
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

                        NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        NForm51.FormClosed += NForm51_FormClosed;
                        NForm51.ShowDialog();

                    }
                    else
                    {
                        return;
                    }
                }


            }

        

            //
            // GO TO UCForm51d to calculate amount to be replenished 
            //

            if (WAction == 8)
            {
                // Process No Updating
                //

                Us.ReadSignedActivityByKey(WSignRecordNo);

                Us.ProcessNo = 8; // Go to calculate money IN

                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Am.ReadAtmsMainSpecific(WAtmNo);


                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                if (WSesNo != Am.CurrentSesNo)
                {
                    MessageBox.Show("MSG679: Choose the right Replenishment Cycle please! This is: " + Am.CurrentSesNo.ToString());

                    return;
                }

                NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm51.FormClosed += NForm51_FormClosed;
                NForm51.ShowDialog();
            }

            // RECONCILIATION for not Groups
            //
            if (WAction == 2)
            {

                // Check LAST RECORD if Already in authorization process

                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Reconciliation");

                if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
                {
                    MessageBox.Show("This Reconciliation Already has authorization record!" + Environment.NewLine
                                             + "Go to Pending Authorisations process to complete."
                                                              );
                    return;
                }
              
                //TEST
                if (WSignedId == "1005")
                {
                    if (WAtmNo == "AB104")
                    {
                        WAtmNo = "AB104";
                    //    CurrentSessionNo = int.Parse(textBox2.Text);
                    }

                    if (WAtmNo == "AB102")
                    {
                     //   CurrentSessionNo = 3144;
                        WSesNo = 3144;
                    }

                }
                
                if (WSignedId == "500")
                {
                    WAtmNo = "12507";
                //    CurrentSessionNo = 1122;
                    WSesNo = 1122;
                }

                 if (WSignedId == "03ServeUk")
            {
                if (WAtmNo == "ServeUk102")
                {
              
                    WSesNo = 6694 ;
                }
               
            }
                 if (WSignedId == "03ServeUk")
                 {
                     if (WAtmNo == "ABC501")
                     {

                         WSesNo = 6695;
                     }

                 }
                Ua.ReadUsersAccessAtmTableSpecific(WSignedId, WAtmNo, 0);

                if (Ua.RecordFound == false || (Ua.RecordFound == true & Ua.Reconciliation == false))
                {
                    MessageBox.Show(" YOU ARE NOT AUTHORISED TO RECONCILE THIS ATM ");
                    return;
                }
                // UPDATE INTENTED FUNCTION 

                Us.ReadSignedActivityByKey(WSignRecordNo);

                Us.ProcessNo = 2;

                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                if (WProcessMode == -1)
                {
                    MessageBox.Show("MSG680: This Repl Cycle is not ready for reconciliation.");
                    return;
                }

                if (WProcessMode == 2 || WProcessMode == 3)
                {

                    if (MessageBox.Show("Process Mode = 2 or 3 ... Atm Has already had passed the Reconciliation Workflow. Do you want to proceed ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                    {
                        // If Yes proceed .... 
                    }
                    else
                    {
                        return;
                    }
                }

                if (WProcessMode <= 1)
                {
                    Ta.FindNextReplCycleId(WAtmNo);
                    if (WSesNo != Ta.Last_1 & Ta.Last_1 > 0)
                    {
                        MessageBox.Show("MSG681: Choose the right Repl Number. Choose the : " + Ta.Last_1.ToString());
                        return;
                    }
                    else
                    {
                        if (Ta.Last_1 == 0)
                        {
                            MessageBox.Show("MSG682: ATM not ready for Reconciliation");
                            return;
                        }
                    }
                }

                NForm71 = new Form71(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm71.FormClosed += NForm71_FormClosed;
                NForm71.ShowDialog(); ;
            }
            //  GOTO MANAGE CAPTURED CARDS
            if (WAction == 3)
            {
         
                //TEST
                if (WSignedId == "1005")
                {
                    // WAtmNo = "AB102";
                    // CurrentSessionNo = 3144;
                    //  WSesNo = 3144;
                }

                if (WSignedId == "500")
                {
                    WAtmNo = "12507";
              //      CurrentSessionNo = 1122;
                    WSesNo = 1122;
                }

                Ua.ReadUsersAccessAtmTableSpecific(WSignedId, WAtmNo, 0);

                if (Ua.RecordFound == false || (Ua.RecordFound == true & Ua.Reconciliation == false))
                {
                    MessageBox.Show(" You are not authorised for this ATM ");
                    return;
                }

                //CAPTURED CARDS 
                NForm26 = new Form26(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm26.FormClosed += NForm26_FormClosed;
                NForm26.ShowDialog(); ;
              
            }
            //
            //
            //  GOTO MANAGE DEPOSITS FOR A PARTICULAR REPL CYCLE
            // 
            if (WAction == 4)
            {
             
                if (WSignedId == "1005")
                {
                    /*
                    WAtmNo = "AB102";
                    CurrentSessionNo = 3144;
                    WSesNo = 3144;
                     */
                }

                if (WSignedId == "500")
                {
                    WAtmNo = "12507";
               //     CurrentSessionNo = 1122;
                    WSesNo = 1122;
                }

                //
                // Check if USER is authorised for this ATM
                //
                Ua.ReadUsersAccessAtmTableSpecific(WSignedId, WAtmNo, 0);

                if (Ua.RecordFound == false || (Ua.RecordFound == true & Ua.Reconciliation == false))
                {
                    MessageBox.Show(" You are not authorised for this ATM ");
                    return;
                }
                // DEPOSITS 
                NForm38 = new Form38(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm38.FormClosed += NForm38_FormClosed;
                NForm38.ShowDialog(); ;
             
            }
        }

        void NForm38_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form152_Load(this, new EventArgs());

            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
        }

        void NForm26_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form152_Load(this, new EventArgs());

            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
        }   

        void NForm71_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            Form152_Load(this, new EventArgs());

            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
        }

        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {

            Form152_Load(this, new EventArgs());

            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
            //  dataGridView1 will be refresed

        }

        // Zero DATA METHOD 
        //   
        //
        private void ZeroData()
        {
            RRDMNotesBalances Na = new RRDMNotesBalances(); // Activate Class 
            RRDMDepositsClass Da = new RRDMDepositsClass();
            RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

            if (WSignedId == "1005")
            {
                if (WAtmNo == "AB102")
                {
                //    CurrentSessionNo = 3144;
                    WSesNo = 3144;
                }
                /*
                if (WAtmNo == "AB104")
                {
                    CurrentSessionNo = 5174;
                    WSesNo = 5174;
                }
                 */
            }
            
            if (WAtmNo == "12507")
            {
                
             //   CurrentSessionNo = 1122;
                WSesNo = 1122;
            }

             if (WSignedId == "03ServeUk")
            {
                if (WAtmNo == "ServeUk102")
                {
              
                    WSesNo = 6694 ;
                }
               
            }
             if (WSignedId == "03ServeUk")
             {
                 if (WAtmNo == "ABC501")
                 {

                     WSesNo = 6695;
                 }

             }
            // Update Physical Data
            Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

            Na.PhysicalCheck1.NoChips = false;
            Na.PhysicalCheck1.NoCameras = false;
            Na.PhysicalCheck1.NoSuspCards = false;
            Na.PhysicalCheck1.NoGlue = false;
            Na.PhysicalCheck1.NoOtherSusp = false;

            Na.PhysicalCheck1.OtherSuspComm = "";

            Na.UpdateSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

            // CASSETTES COUNT AND CAPTURED CARDS 
            Na.ReadSessionsNotesAndValues( WAtmNo, WSesNo, 2);

            Na.Cassettes_1.CasCount = 0;

            Na.Cassettes_1.RejCount = 0;

            Na.Cassettes_2.CasCount = 0;

            Na.Cassettes_2.RejCount = 0;

            Na.Cassettes_3.CasCount = 0;

            Na.Cassettes_3.RejCount = 0;


            Na.Cassettes_4.CasCount = 0;

            Na.Cassettes_4.RejCount = 0;


            // Captured Cards 

            Na.CaptCardsCount = 0;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            // DEPOSITS 

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            Da.DepositsCount1.Trans = 0;

            Da.DepositsCount1.Notes = 0;

            Da.DepositsCount1.Amount = 0;

            Da.DepositsCount1.NotesRej = 0;

            Da.DepositsCount1.AmountRej = 0;

            Da.DepositsCount1.Envelops = 0;

            Da.DepositsCount1.EnvAmount = 0;

            // CHEQUES
            //
            Da.ChequesCount1.Trans = 0;


            Da.ChequesCount1.Number = 0;

            Da.ChequesCount1.Amount = 0;

            Da.UpdateDepositsSessionsNotesAndValuesWithCount(WAtmNo, WSesNo); // UPDATE INPUT VALUES


            //     Replenishement 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.Cassettes_1.NewInUser = 0;

            Na.Cassettes_2.NewInUser = 0;

            Na.Cassettes_3.NewInUser = 0;

            Na.Cassettes_4.NewInUser = 0;

            // Update Notes balances with new in figures 

            Na.ReplMethod = 0;
            Na.InUserDate = new DateTime(2050, 11, 21);
            Na.InReplAmount = 0;

            Na.ReplAmountTotal = 0;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            Na.ReplUserComment = " ";

            Na.UpdateSessionsNotesAndValuesUserComment(WAtmNo, WSesNo);

            // Undo Process Mode in Ta.

            //  Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            //  Ta.ProcessMode = 0;

            //   Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

        }

        // READ Errors AND UNDO  
        //
        private void UndoErrors(string InAtmNo, int InSesNo)
        {
            RecordFound = false;

            int ErrNo;
            int ErrId;
            bool OpenErr;
            bool NeedAction;
            bool UnderAction;
            bool ManualAct;

            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

            string SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE AtmNo = @AtmNo AND SesNo = @SesNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details

                            ErrNo = (int)rdr["ErrNo"];
                            ErrId = (int)rdr["ErrId"];

                            NeedAction = (bool)rdr["NeedAction"];
                            UnderAction = (bool)rdr["UnderAction"];
                            ManualAct = (bool)rdr["ManualAct"];
                            OpenErr = (bool)rdr["OpenErr"];

                            if (ErrId < 200)
                            {
                                Ec.ReadErrorsTableSpecific(ErrNo);
                                Ec.OpenErr = true; 
                                Ec.UnderAction = false;
                                Ec.ManualAct = false;
                                Ec.UpdateErrorsTableSpecific(ErrNo);
                            }

                            if (ErrId > 200) // Deposits 
                            {
                                Ec.ReadErrorsTableSpecific(ErrNo);
                                Ec.OpenErr = true; 
                                Ec.UpdateErrorsTableSpecific(ErrNo);
                            }
                        }
                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();
                    //   MessageBox.Show(ex.ToString());
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }
        // Finish => go back to main 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }    

    }
}

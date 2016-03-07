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

// Alecos
using System.Configuration;
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form47 : Form
    {
    
        Form49 NForm49; // ERRORS MANAGEMENT STRING 

        Form48 NForm48; // Repl Cycles 

        Form42 NForm42; // Repl Actions 

        Form24 NForm24; // Errors

        Form78 NForm78; // Transactions to be posted outstanding 

        string filter; 

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();
      
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
 
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms(); 
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAllowedAtmsAndUpdateFromJournal Aj = new RRDMAllowedAtmsAndUpdateFromJournal();

        RRDMTempAtmLocation Tl = new RRDMTempAtmLocation();

        //public DataTable MyATMsTable = new DataTable();

        string WPreviousAtmNo; 

        string WAtmNo;
        int WSesNo;
        int WRowIndex;

        string WBankId;
   
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCitId;
        int WAction;

        public Form47(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId; 
            WAction = InAction;  // If 1 is normal just watching - 
                                 // If 2 then is for errors management 
                                 // If 3 it comes from CIT Provider Form. 
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            label14.Text = WOperator; 
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
            //TEST
            dateTimePicker2.Value = new DateTime(2014, 02, 01);
            dateTimePicker3.Value = new DateTime(2014, 03, 28);

            //WPreviousAtmNo = ""; 

            //comboBox1.Items.Add("ReplCycles");
            //comboBox1.Items.Add("ReplActions");

            //comboBox1.Text = "ReplCycles"; 
        }

        // ON LOAD DO 
        private void Form47_Load(object sender, EventArgs e)
        {
            if (WAction == 1) textBoxMsgBoard.Text = "Current Atms Status. You can also go to current and historical information of Repl Cycles";
            if (WAction == 2)
            {
                textBoxMsgBoard.Text = "Go to next to view and make changes on errors";
                labelStep1.Text = " Errors Management Workflow";
                buttonNext.Visible = true; 
            } 
            if (WAction == 3) textBoxMsgBoard.Text = "This ATM is Replenished by CIT provider";

            if (WAction == 2)
            {
                label36.Hide();
                label45.Hide();
                textBoxOutstandingErrors.Hide();
                textBoxInProcessForAction.Hide();
                button10.Hide();
                button11.Hide();
                label23.Hide();
                panel4.Hide();
            }

            if (WAction == 1 || WAction == 3)
            {           
                if (WAction == 3)
                {
                    labelStep1.Text = " Atms For CIT Provider : " + WCitId;

                    label2.Text = " ATMs BASIC INFORMATION ";
                }
                label12.Hide(); 
                panel5.Hide();
            }

            if (WAction == 1 || WAction == 2) // ALECOS NO NEED AT THIS STAGE TO UNDERSTAND THIS PART 
            {
                //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE---------------//
                // Read USER and ATM Table 
                // GET TABLE OF ALLOWED ATMS FOR REPLENISH
                string WFunction = "Any";
                Aj.CreateTableOfAccess(WSignedId, WSignRecordNo, WOperator, WFunction);

                // From eJournal update traces and transactions based on table  
                Aj.UpdateLatestEjStatus(WSignedId, WSignRecordNo, WOperator);

                //-----------------------------------------------------------------// 
            }

            //==================================

          
                if (WAction == 1 || WAction == 2)
                {
                    filter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'";
                }
                if (WAction == 3)
                {
                    filter = "Operator ='" + WOperator + "' AND CitId ='" + WCitId +"'";
                }
            // 
            //
            //  DataGrid 
            //
            //

                Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, ""); 

                dataGridViewMyATMS.DataSource = Am.ATMsMainSelected.DefaultView;

                dataGridViewMyATMS.Columns[0].Width = 70; // AtmNo
                dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridViewMyATMS.Columns[1].Width = 70; // ReplCycle
                dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridViewMyATMS.Columns[2].Width = 120; // AtmName

                dataGridViewMyATMS.Columns[3].Width = 130; // RespBranch

                dataGridViewMyATMS.Columns[4].Width = 70; // Auth User 

                dataGridViewMyATMS.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridViewMyATMS.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

                if (dataGridViewMyATMS.Rows.Count == 0)
                {
                    Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                    MessageForm.ShowDialog();

                    this.Dispose();
                    return;
                }

        }

        // AN ATM WAS ENTERED 
        private void button1_Click(object sender, EventArgs e)
        {          
            if (String.IsNullOrEmpty(textBox1.Text))

            {
                MessageBox.Show("Choose An ATM number Please");
                return;
            }
            else
            {
                 WAtmNo = textBox1.Text;

            }
                
                // See if this ATM belongs to the user 

            RRDMRightToAccessAtm Ra = new RRDMRightToAccessAtm();

            Ra.CheckRightToAccessAtm(WSignedId, WAtmNo);

            if (Ra.RecordFound == false)
            {
                MessageBox.Show(" You are not authorised to access this ATM ");
                return;
            }

            Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WAtmNo);

            dataGridViewMyATMS.DataSource = Am.ATMsMainSelected.DefaultView;

            dataGridViewMyATMS.Columns[0].Width = 70; // AtmNo
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMyATMS.Columns[1].Width = 70; // ReplCycle
            dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMyATMS.Columns[2].Width = 120; // AtmName

            dataGridViewMyATMS.Columns[3].Width = 130; // RespBranch

            dataGridViewMyATMS.Columns[4].Width = 70; // Auth User 

            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridViewMyATMS.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            Am.ReadAtmsMainSpecific (WAtmNo);
            WSesNo = Am.CurrentSesNo;         
    
        }

        // A ROW WAS CHOSEN FOR FURHER INFORMATION 
        //
        // ON ROW ENTER for Grid 
        private void dataGridViewMyATMS_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewMyATMS.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;
            
            if (WAtmNo == WPreviousAtmNo)
            {
                return;
            }
            else
            {
                WPreviousAtmNo = WAtmNo; 
            }

            label26.Text = "CURRENT INFO FOR ATM : " + WAtmNo;

            Am.ReadAtmsMainSpecific(WAtmNo);

            // Set up the Bank for this ATM
            WBankId = Am.BankId;

            textBoxBank.Text = WBankId;

            if (Am.ProcessMode == -2)
            {
                if (Am.ProcessMode == -2)
                {
                    label26.Hide();
                    panel8.Hide();

                    label23.Hide();
                    panel4.Hide();

                    MessageBox.Show("This Atm is not active yet");

                }
                return;
            }
            else
            {
                label26.Show();
                panel8.Show();
                if (WAction != 2)
                {
                    label23.Show();
                    panel4.Show();
                }

            }

            textBoxBranch.Text = Am.BranchName;

            if (WAction == 3) // This is coming from a CIT company 
            {
                Us.ReadUsersRecord(WCitId);
                textBoxOwnerUser.Text = Us.UserId;
                textBoxName.Text = Us.UserName;
                textBoxEmail.Text = Us.email;
                textBoxMobile.Text = Us.MobileNo;

            }
            else
            {
                Uaa.FindUserForRepl(WAtmNo, 0);

                Us.ReadUsersRecord(Uaa.UserId); // Get Info for User 

                textBoxOwnerUser.Text = Us.UserId;
                textBoxName.Text = Us.UserName;
                textBoxEmail.Text = Us.email;
                textBoxMobile.Text = Us.MobileNo;
            }


            textBoxReplCycleNo.Text = Am.CurrentSesNo.ToString();
            textBoxLastReplDt.Text = Am.LastReplDt.ToString();
            textBoxNxtReplDt.Text = Am.NextReplDt.ToString();
            textBoxCassettesAmnt.Text = Am.CurrCassettes.ToString("#,##0.00");
            textBoxDepositedAmnt.Text = Am.CurrentDeposits.ToString("#,##0.00");

            textBoxLastReconcDt.Text = Am.ReconcDt.ToString();
            if (Am.ReconcDiff == true)
            {
                textBoxReconcDiff.Text = "YES";

            }
            else
            {
                textBoxReconcDiff.Text = "NO";

            }

            textBoxCurrency.Text = Am.CurrNm1;
            textBoxAmountInDiff.Text = Am.DiffCurr1.ToString("#,##0.00");
            textBoxSessionsInDiff.Text = Am.SessionsInDiff.ToString();
            textBoxOutstandingErrors.Text = Am.ErrOutstanding.ToString();

            Ec.ReadAllErrorsTableForCounters(WBankId,"EWB110" ,WAtmNo);

            textBoxInProcessForAction.Text = Ec.ErrUnderAction.ToString();

            if (Am.ErrOutstanding > 0 & WAction != 2)
            {
                button10.Show();
            }
            else
            {
                button10.Hide();
            }

            if (Ec.ErrUnderAction > 0)
            {
                button11.Show();
            }
            else
            {
                button11.Hide();
            }
            WSesNo = Am.CurrentSesNo;



            if (Am.ProcessMode == -1)
            {
                textBoxStatus.Text = "Atm is currently serving customers";
                if (WAtmNo == "AB104") textBoxStatus.Text = "Atm is currently serving customers." + " Examine if Replenishment or reconciliation is needed";
            }
            if (Am.ProcessMode == 0)
            {
                textBoxStatus.Text = "Atm is currently ready for replenishment";
            }
            if (Am.ProcessMode == 1)
            {
                textBoxStatus.Text = "Atm has been replenished";
            }
            if (Am.ProcessMode == 2)
            {
                textBoxStatus.Text = "Atm has been fully reconciled";
            }
            if (Am.ProcessMode == 3)
            {
                textBoxStatus.Text = "Atm has NOT been fully reconciled";
            }

            if (WAction == 2)
            {
                // READ ALL ERRORS AND SET COUNTER 

                Ec.ReadAllErrorsTableForCounters(WBankId, "EWB110" , WAtmNo);

                textBox18.Text = Ec.NumOfErrors.ToString();
                textBox17.Text = Ec.ErrUnderAction.ToString();
                textBox5.Text = Ec.ErrUnderManualAction.ToString();

            }
        }
      

        // SHOW ERRORS FOR THIS ATM FOR THEIR MANAGEMENT 
        // Next for errors workflow 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridViewMyATMS.SelectedRows[0].Index;

            NForm49 = new Form49(WSignedId, WSignRecordNo, WBankId, WAtmNo);

            NForm49.FormClosed += NForm49_FormClosed;
            NForm49.ShowDialog();
          
        }


        void NForm49_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form47_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndex].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

       
// Refresh - Needed after a single ATM has been chosen
   private void button5_Click(object sender, EventArgs e)
   {

       if (WAction == 1 || WAction == 2)
       {
           {
               filter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'";
           }
           if (WAction == 3)
           {
               filter = "Operator ='" + WOperator + "' AND CitId ='" + WCitId + "'";
           }

           //atmsMainBindingSource.Filter = filter;
           //this.atmsMainTableAdapter.Fill(this.aTMSDataSet28.AtmsMain);

           //  DataGrid 

           Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, "");

           dataGridViewMyATMS.DataSource = Am.ATMsMainSelected.DefaultView;

       }
       
   }
        // Show ATMS LOCATION 

   // ATM LOCATION 
   private void button9_Click(object sender, EventArgs e)
   {
      
       string SeqNoURL = ConfigurationManager.AppSettings["RRDMMapsGeoQueryURL"];
       
       RRDMAtmsClass Ac = new RRDMAtmsClass();
       Ac.ReadAtm(WAtmNo);

       int TempMode = 2;
       Tl.DeleteTempAtmLocationRecord(WAtmNo, TempMode);

       Tl.BankId = WOperator;
       Tl.Mode = 2;

       Tl.GroupNo = 1;
       Tl.GroupDesc = "Show ATM " + WAtmNo;

       Tl.DtTmCreated = DateTime.Now;

       Tl.Street = Ac.Street;
       Tl.Town = Ac.Town;
       Tl.District = Ac.District;
       Tl.PostalCode = Ac.PostalCode; 
       Tl.Country = Ac.Country;

       Tl.Latitude = Ac.Latitude;
       Tl.Longitude = Ac.Longitude;

       Tl.ColorId = "2";
       Tl.ColorDesc = "Normal Color";

       Tl.InsertTempAtmLocationRecord(WAtmNo);

       Tl.FindTempAtmLocationLastNo(WAtmNo, Tl.Mode);

       // Format the URL with the query string (ATMSeqNo=#)

       string QueryURL = SeqNoURL + "?ATMSeqNo=" + Tl.SeqNo.ToString();

       // Invoke default browser
       ProcessStartInfo sInfo = new ProcessStartInfo(QueryURL);
       Process.Start(sInfo);

   }

// GO TO SHOW REPLENISHMENT CYCLES 
   private void button6_Click(object sender, EventArgs e) 
   {
       WRowIndex = dataGridViewMyATMS.SelectedRows[0].Index;

       if (radioButtonReplCycles.Checked == true)
       {
       NForm48 = new Form48(WSignedId, WSignRecordNo, WBankId, WAtmNo, dateTimePicker2.Value, dateTimePicker3.Value);
       NForm48.FormClosed += NForm48_FormClosed;
       NForm48.ShowDialog();
       }

       if (radioButtonActions.Checked == true)
       {
       NForm42 = new Form42(WSignedId, WSignRecordNo, WBankId, WAtmNo, dateTimePicker2.Value, dateTimePicker3.Value);
       NForm42.FormClosed += NForm42_FormClosed;
       NForm42.ShowDialog();
       }

       if (radioButtonAccounts.Checked == true)
       {
           string WAccName = comboBox1.Text;
           string WAccCurr = comboBox2.Text; 
           Form31 NForm31 = new Form31(WSignedId, WSignRecordNo, WOperator, WCitId, WAccName, WAccCurr, 4,
                    dateTimePicker2.Value, dateTimePicker3.Value, WAtmNo);
           NForm31.FormClosed += NForm31_FormClosed;
           NForm31.ShowDialog();
       }

       if (radioButtonRMCategCycles.Checked == true)
       {
           Form80a NForm80a;
           string WFunction = "View";
           NForm80a = new Form80a(WSignedId, WSignRecordNo, WOperator, WFunction, "ALL");
           NForm80a.ShowDialog();
       }
  
   }

   void NForm31_FormClosed(object sender, FormClosedEventArgs e)
   {
       Form47_Load(this, new EventArgs());
       dataGridViewMyATMS.Rows[WRowIndex].Selected = true;
       dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
   }

   void NForm42_FormClosed(object sender, FormClosedEventArgs e)
   {
       Form47_Load(this, new EventArgs());

       dataGridViewMyATMS.Rows[WRowIndex].Selected = true;
       dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
   }

   void NForm48_FormClosed(object sender, FormClosedEventArgs e)
   {
       Form47_Load(this, new EventArgs());

       dataGridViewMyATMS.Rows[WRowIndex].Selected = true;
       dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
   }

        // ALL OUTSTANDING ATM ERRORS 
   private void button10_Click(object sender, EventArgs e)
   {
       bool Mode = true;
       string SearchFilter = "AtmNo = '" + WAtmNo + "'" + " AND OpenErr =1"; 
    
       NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, "", Mode, SearchFilter);
       NForm24.ShowDialog();
   }
        //
// show suspense transactions
        //
   private void button11_Click(object sender, EventArgs e)
   {
       int Mode = 3;
       NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator , WAtmNo, 0, 0, Mode);

       NForm78.ShowDialog();
   }

// FINISH 
   private void button2_Click(object sender, EventArgs e)
   {
       this.Dispose(); 
   }
// Show Accounts 
   private void radioButtonAccounts_CheckedChanged(object sender, EventArgs e)
   {
       if (radioButtonAccounts.Checked == true)
       {
           panelAccounts.Show();

           RRDMComboClass Cc = new RRDMComboClass(); 

           comboBox1.DataSource = Cc.GetAtmAccs(WOperator, WAtmNo);
           comboBox1.DisplayMember = "DisplayValue";

           comboBox2.DataSource = Cc.GetAtmAccsCurr(WOperator, WAtmNo);
           comboBox2.DisplayMember = "DisplayValue";
       }
       else
       {
           panelAccounts.Hide(); 
       }

   }
  
  
    }

  }

    


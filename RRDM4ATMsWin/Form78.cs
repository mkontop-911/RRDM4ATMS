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
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form78 : Form
    {
   /// <summary>
   /// MANAGE ALL TRANSACTIONS CREATED DURING RECONCILIATION PROCESS
   /// VOUCHERS ARE PRINTED FOR UPDATING BY CASHIER 
   /// </summary>
   /// 

        RRDMAllowedAtmsAndUpdateFromJournal Aj = new RRDMAllowedAtmsAndUpdateFromJournal();

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions(); 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        //DateTime NullPastDate = new DateTime(1950, 11, 21); 

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        DateTime NullPastDate = new DateTime(1900, 01 , 01);

        DateTime WDtFrom ;
        DateTime WDtTo ;

        Form112 NForm112;

        bool WMatching ;
        bool WReplenishment;
        bool WReconciliation; 


        int WDisputeNo;
        int WDisputeTranNo;

        bool WithDate; 
        string Gridfilter; 
    
        int WRow;
        string WLineAtmNo ;
        int WLineSesNo ;
        
        int WPostedNo;
        int ErrNo;

        bool WRange; 

        string WSignedId;
        int WSignRecordNo;

        string WOperator; 
    
        string WAtmNo;
        int WSesNo;

        int WErrNo;
        int WMode;

        public Form78(string InSignedId, int InSignRecordNo, string InOperator, 
            string InAtmNo, int InSesNo, int InErrNo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WOperator = InOperator; 

            WAtmNo = InAtmNo; // It is blank if comes from Form1
            WSesNo = InSesNo; // It is >0 From Form83 and Form116

            WErrNo = InErrNo; // It is > 0 THIS TRUE WHEN WE WANT TO SHOW Just this

            WMode = InMode; // If = 3 comes from My ATMs Form47 , if 2 from Form83 and Form116, if 1 from Form1

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            labelUser.Text = InSignedId; 
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            //Preparing Selection Panel 
            Gp.ParamId = "424"; // Origin  
            comboBoxOrigin.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxOrigin.DisplayMember = "DisplayValue";

            radioButton1.Checked = true; 
            
        }

        private void Form78_Load(object sender, EventArgs e)
        {        
          
             try
            { 
                // ===========================================
                // =============================================================
                // Read USER and ATM Table 
                // GET TABLE OF ALLOWED ATMS FOR REPLENISH
                string WFunction = "Any";
                Aj.CreateTableOfAccess(WSignedId, WSignRecordNo, WOperator, WFunction);

                RRDMUpdateAuthUserForSpecialGrids Up = new RRDMUpdateAuthUserForSpecialGrids();
                // if 1 = No updating of latest ejournals info 
                // if 2 = Updating of the last ejournals info 
                Up.UpdateAuthUserForTransToBePostedMethod(WSignedId, WSignRecordNo,
                          WOperator, 1);

                 // SHOW GRID

                ShowGrid("");              
            }
             catch (Exception ex)
             {

                 string exception = ex.ToString();
                 
                 MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                 
             }

        }
// This method loads and shows the Grid 
        public void ShowGrid(string InGridfilter)
        {

            try
            {

                if (InGridfilter == "")
                {
                    // =============================================================
                    // Make Grid ready for all allow trans for this User 

                

                    //InGridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'"
                    //          + " AND (OpenRecord = 1 OR (OpenRecord = 0 AND GridFilterDate = @ToDay ))" ;

                    InGridfilter = "Operator ='" + WOperator + "'"
                            + " AND (OpenRecord = 1 OR (OpenRecord = 0 AND GridFilterDate = @ToDay ))";

                    WithDate = true; 
                    //Tc.ReadAllTransToBePosted(InGridfilter, DateTime.Today, WithDate);

                    //InGridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'"
                    //          + " AND (OpenRecord = 1 OR (OpenRecord = 0 AND (GridFilterDate" + DateTime.Today.AddDays(-1)
                    //          + " AND GridFilterDate<" + DateTime.Today.AddDays(1) + ")))";

                    // Change GridFilter if exception requests 

                    if (WSesNo > 0) // Show only for this Session 
                    {
                        textBox5.Hide(); 
                        panel3.Hide();
                        textBox6.Hide();
                        panel4.Hide();
                        InGridfilter = "Operator ='" + WOperator + "' AND AtmNo ='" + WAtmNo + "' AND SesNo =" + WSesNo;

                        WithDate = false;
                        Tc.ReadAllTransToBePosted(InGridfilter, DateTime.Today, WithDate);
                    }

                    if (WErrNo > 0) // Show Only for this Error 
                    {
                        textBox5.Hide();
                        panel3.Hide();
                        textBox6.Hide();
                        panel4.Hide();
                        InGridfilter = "Operator ='" + WOperator + "' AND ErrNo =" + WErrNo;

                        WithDate = false;
                        Tc.ReadAllTransToBePosted(InGridfilter, DateTime.Today, WithDate);
                    }

                    if (WMode == 3) // ALL Open for a particular ATM 
                    {
                        textBox5.Hide();
                        panel3.Hide();
                        textBox6.Hide();
                        panel4.Hide();
                        InGridfilter = "Operator ='" + WOperator + "' AND AtmNo ='" + WAtmNo + "' AND OpenRecord = 1";

                        WithDate = false;
                        Tc.ReadAllTransToBePosted(InGridfilter, DateTime.Today, WithDate);

                    }
                }

                buttonExpandGrid.Hide(); 
                
                // 
                //
                //  DataGrid LOADING
                //
                //

                if (WRange == false)
                {
                    Tc.ReadAllTransToBePosted(InGridfilter, DateTime.Today, WithDate);
                }
                else
                {
                    // Table Tc.TransToBePostedSelected was filled in Show Button 
                }
                
                dataGridView1.DataSource = Tc.TransToBePostedSelected.DefaultView;

                dataGridView1.Columns[0].Width = 70; // 
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[1].Width = 180; // 
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[2].Width = 70; // 
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[3].Width = 70; //
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[4].Width = 80; //
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[5].Width = 60; //
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[6].Width = 150; //

                dataGridView1.Columns[7].Width = 100; // 

                dataGridView1.Columns[8].Width = 60; //

                dataGridView1.Columns[9].Width = 60; //
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
               

                if (dataGridView1.Rows.Count == 0 & WRange == false)
                {
                    //MessageBox.Show("No transactions to be posted");
                    Form2 MessageForm = new Form2("No transactions to be posted");
                    MessageForm.ShowDialog();

                    this.Dispose();
                    return;
                }
                if (dataGridView1.Rows.Count == 0 & WRange == true)
                {
                    MessageBox.Show("No transactions for this selection");
                    
                    buttonExpandGrid.Hide(); 
                    return;
                }

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

                WPostedNo = (int)rowSelected.Cells[0].Value;

                Tc.ReadTransToBePostedSpecific(WPostedNo);

                WLineAtmNo = Tc.AtmNo;
                WLineSesNo = Tc.SesNo;

                // "01" OurATMS-Matching
                // "02" BancNet Matching                               
                // "03" OurATMS-Reconc
                // "04" OurATMS-Repl
                // "05" Settlement
                // "07" Disputes 
                // "08" Settlement 

                if (Tc.OriginId == "01") // "01" OurATMS-Matching
                {
                    WMatching = true; 
                }
                else WMatching = false;

                if (Tc.OriginId == "04") // "04" OurATMS-Repl
                {
                    WReplenishment = true;
                }
                else WReplenishment = false;

                if (Tc.OriginId == "03") // "03" OurATMS-Reconc
                {
                    WReconciliation = true;
                }
                else WReconciliation = false;

                if (Tc.OriginId == "07") // "07" Disputes 
                if (Tc.OriginName == "Disputes")
                {
                    WDisputeNo = Tc.DisputeNo;
                    WDisputeTranNo = Tc.DispTranNo;               

                }

                if (Tc.OriginId != "07") // Not dispute 
                {
                    WDisputeNo = 0;
                    WDisputeTranNo = 0;
                }

                if (Tc.OriginId == "08") // Not dispute 
                {
                    buttonAuthHistory.Hide(); 
                }
                else
                {
                    buttonAuthHistory.Show(); 
                }

                // SHOW ALL NEEDED DETAILS TO PAY ATTENTION ON
                //
                ErrNo = Tc.ErrNo;
                textBox1.Text = Tc.CardNo;

                textBox20.Text = Tc.AtmDtTime.ToString(); 
 
                if (Tc.AccNo == "Not Available") 
                {
                    textBox4.ReadOnly = false;
                    textBox4.Text = "Input Account";

                    if (Tc.TransType == 11 || Tc.TransType == 12) textBox18.Text = "DR";
                    if (Tc.TransType == 21 || Tc.TransType == 22) textBox18.Text = "CR"; 

                    textBox7.ReadOnly = false;
                    textBox7.Text = "Input Account";

                    if (Tc.TransType2 == 11 || Tc.TransType2 == 12) textBox19.Text = "DR";
                    if (Tc.TransType2 == 21 || Tc.TransType2 == 22) textBox19.Text = "CR"; 
                }
                else 
                {
                    textBox4.ReadOnly = true ;
                    textBox4.Text = Tc.AccNo ;

                    if (Tc.TransType == 11 || Tc.TransType == 12) textBox18.Text = "DR";
                    if (Tc.TransType == 21 || Tc.TransType == 22) textBox18.Text = "CR"; 

                    textBox7.ReadOnly = true;
                    textBox7.Text = Tc.AccNo2;

                    if (Tc.TransType2 == 11 || Tc.TransType2 == 12) textBox19.Text = "DR";
                    if (Tc.TransType2 == 21 || Tc.TransType2 == 22) textBox19.Text = "CR"; 
                }
               
                textBox2.Text = Tc.TransDesc;
                textBox3.Text = Tc.CurrDesc;
                textBox8.Text = Tc.TranAmount.ToString("#,##0.00");
                textBox10.Text = Tc.RemNo.ToString();
                textBox9.Text = Tc.RefNumb.ToString();

                Ec.ReadErrorsTableSpecific(ErrNo);

                textBox16.Text = Ec.ErrDesc;

                textBox17.Text = Tc.TransMsg;
               
                Gp.ReadParametersSpecificId(WOperator, "705", Tc.SystemTarget.ToString(), "", "");

                textBox15.Text = Gp.OccuranceNm;

                textBox14.Text = Tc.ActionBy.ToString();
                textBox13.Text = Tc.ActionCd2.ToString();
                if (Tc.ActionDate != NullPastDate) textBox12.Text = Tc.ActionDate.ToString();
                else textBox12.Text = "";

                if (Tc.ActionDate == NullPastDate)
                {
                    textBoxMsgBoard.Text = "Finalise action for this transaction";
                    // HIDE ACTION Fields 
                    pictureBox2.Hide();
                    pictureBox3.Show();
                    label17.Hide();
                    label13.Hide();
                    label12.Hide();
                    label11.Hide();
                    textBox14.Hide();
                    textBox13.Hide();
                    label3.Hide();
                    textBox12.Hide();
                }
                else
                {
                    pictureBox2.Show();
                    pictureBox3.Hide();
                    label17.Show();
                    label13.Show();
                    label12.Show();
                    label11.Show();
                    textBox14.Show();
                    textBox13.Show();
                    label3.Show();
                    textBox12.Show();

                    if (Tc.ActionCd2 == 1)
                    {
                        textBoxMsgBoard.Text = "Action was finalised for this transaction";
                        label3.Text = "Finalised";
                    }
                    if (Tc.ActionCd2 == 2)
                    {
                        textBoxMsgBoard.Text = "Action was rejected for this transaction";
                        label3.Text = "Rejected";
                    }
                    if (Tc.ActionCd2 == 3)
                    {
                        textBoxMsgBoard.Text = "Action was postponed for this transaction";
                        label3.Text = "Postponed";
                    }

                }

                radioButton8.Checked = false;
                radioButton7.Checked = false;
                radioButton6.Checked = false;
             
            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }
           
        }
        // UPDate ACTION 
        private void button1_Click(object sender, EventArgs e)
        {
            
            WRow = dataGridView1.SelectedRows[0].Index;
           
            int ActionCd2 = 0;

            if (radioButton8.Checked == false & radioButton7.Checked == false & radioButton6.Checked == false & radioButtonPostTrans.Checked == false)
            {
                MessageBox.Show(" Please select a radio button ");
                return; 
            }

            if (radioButton8.Checked == true) ActionCd2 = 1; // Action finalised with voucher 
            if (radioButton7.Checked == true) ActionCd2 = 2; // Action rejected
            if (radioButton6.Checked == true) ActionCd2 = 3; // Action postponed 
            if (radioButtonPostTrans.Checked == true)
            {
                ActionCd2 = 4; // All to be posted with Web Services 
                // Call Class 

                Tc.ReadTransToBePostedAllAndCreatePostedTrans(WSignedId, WOperator, ActionCd2);
                if (Tc.RecordFound == true)
                {
                    //MessageBox.Show("Total number of created transactions = " + Tc.TotTransactions.ToString()); 
                }
                else
                {
                    MessageBox.Show("No open trans to be posted available");
                    return; 
                }

                ShowGrid(""); 

                if (Tc.RecordFound == true)
                {
                    MessageBox.Show("Total number of created transactions = " + Tc.TotTransactions.ToString());
                }

                //MessageBox.Show("An interface with the Banking system is not available yet.");
                return; 
            }
            
            if (ActionCd2 == 1 & Tc.ActionCd2 > 0 || ActionCd2 == 2 & Tc.ActionCd2 > 0 ) // Action Already taken
            {
                if (MessageBox.Show("MSG789 - Action already taken. Do you want to proceed? ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                               == DialogResult.Yes)
                {
                   // Continue 
                }
                else
                {
                    return;
                }

            }

            Tc.ActionDate = DateTime.Now;

            Tc.GridFilterDate = DateTime.Today.Date; // DATE For Grid filter 

            Tc.OpenRecord = false; // Close Record 

            Tc.UpdateTransToBePostedAction1(WPostedNo, WSignedId, ActionCd2); 

            Tc.ReadTransToBePostedSpecific(WPostedNo);

            textBox14.Text = Tc.ActionBy.ToString();
            textBox13.Text = Tc.ActionCd2.ToString();
            if (Tc.ActionDate != NullPastDate) textBox12.Text = Tc.ActionDate.ToString();
            else textBox12.Text = "";

            if (radioButton8.Checked == true) // Action with voucher 
            {
                // Print Transactions 
                PrintTrans();
                if (Tc.SystemTarget == 1)
                {
                    PrintJCC();
                }
            }

            if (ActionCd2 == 1)
            {
                MessageBox.Show(" Record Updated. Cashier vouchers are printed.");

                // Close Error 
                if (Tc.ErrNo > 0)
                {
                    Ec.ReadErrorsTableSpecific(Tc.ErrNo);
                    Ec.ByWhom = WSignedId;
                    Ec.ActionDtTm = DateTime.Now;
                    Ec.ActionSes = Tc.SesNo;
                    Ec.OpenErr = false;
                    Ec.UpdateErrorsTableSpecific(Tc.ErrNo);
                }
               
            }
            if (ActionCd2 == 2) MessageBox.Show(" Record Updated. Action is rejected.");
            if (ActionCd2 == 3) MessageBox.Show(" Record Updated. Action is postponed.");
            if (ActionCd2 == 4) MessageBox.Show(" Record Updated. Action is taken with updating directly the banking system.");

             

             int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

             ShowGrid("");

             dataGridView1.Rows[WRow].Selected = true;
             dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

             dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

        }
        // Show DATA GRID AS PER SELECTION 
        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBoxUnique.Checked == true)
            {
                if (radioButtonCard.Checked == false & radioButtonAccNo.Checked == false & radioButtonTraceNo.Checked == false
                    & radioButtonAtmNo.Checked == false & radioButtonDispNo.Checked == false)
                {
                    MessageBox.Show(" Please select a radio button ");
                    return;
                }

                if ( textBox11.Text == "")
                {
                    MessageBox.Show(" Please enter value for the selection made");
                    return;
                }
            }

            WDtFrom = dateTimePicker1.Value.Date;
            WDtTo = dateTimePicker2.Value.Date;
            WDtFrom = WDtFrom.AddDays(-1);
            WDtTo = WDtTo.AddDays(1);

            if (radioButton1.Checked == true) // Opened 
            {
            }
            if (radioButton2.Checked == true) // Closed 
            {
            }

            if (comboBoxOrigin.Text != "N/A") // Origin was selected
            {
            }

            if (checkBoxUnique.Checked == true) // Unique is chosen 
            {
            }

            if (radioButtonCard.Checked == true) // It is card 
            {
            }

            if (radioButtonAccNo.Checked == true) // It is Account No
            {
            }

            if (radioButtonTraceNo.Checked == true) // It is Trace No
            {
                // Turn textBox to Numeric 
            }

            if (radioButtonAtmNo.Checked == true) // It is ATM No
            {
            }

            if (radioButtonDispNo.Checked == true) // It is Dispute No
            {
                // Turn textBox to Numeric 
            }

            if (radioButton1.Checked == true & comboBoxOrigin.Text == "N/A" & checkBoxUnique.Checked == false) // Opened and all
            {
                Gridfilter = "Operator ='" + WOperator
                           + "' AND AtmDtTime  > @WDtFrom AND AtmDtTime <@WDtTo AND OpenRecord = 1";

                Tc.ReadAllTransToBePostedRange(Gridfilter,  WDtFrom, WDtTo);
            }

            if (radioButton1.Checked == true & comboBoxOrigin.Text == "N/A" & checkBoxUnique.Checked == true
                & radioButtonAtmNo.Checked == true) // ATM 
            {
                Gridfilter = "Operator ='" + WOperator 
                           + "' AND AtmDtTime  > @WDtFrom AND AtmDtTime <@WDtTo AND OpenRecord = 1 AND AtmNo ='" + textBox11.Text +"'";

                Tc.ReadAllTransToBePostedRange(Gridfilter, WDtFrom, WDtTo);
            }

            WRange = true ; 

            ShowGrid(Gridfilter);

            buttonExpandGrid.Show(); // This button goes to show grid in bigger form 
         
        }
        //
        // Print Transactions to be posted 
        //
        private void PrintTrans()
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            Ac.ReadAtm(WLineAtmNo);
            RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
            Us.ReadUsersRecord(WSignedId);
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();         

            Tc.ReadTransToBePostedSpecific(WPostedNo);

            Ec.ReadErrorsTableSpecific(Tc.ErrNo); 

            String P1 = WOperator;
            String P2 = Ba.BankName;
           
            String P3 = Us.Branch;
            String P4 = WLineAtmNo;
            String P5 = "";
            if (Tc.TransType == 21 || Tc.TransType == 22)
            {
                P5 = "CREDIT"; 
            }
            if (Tc.TransType == 11 || Tc.TransType == 12)
            {
                P5 = "DEBIT";
            }
            String P6 = Tc.CardNo; 
            String P7 = Tc.AccNo;
            String P8 = Tc.TranAmount.ToString();
            String P9 = Tc.TransDesc;
            String P10 = Tc.ErrNo.ToString();
            String P11; 
            if (Tc.ErrNo == 0)
            {
                P11 = "";
            }
            else
            {
                P11 = Ec.ErrDesc;
            }
           
            // Second transaction in pair 

            String P12 =""; 

       //     Tc.ReadTransToBePostedTraceSequence(AtmNo, Tc.AtmTraceNo, 2);

            if (Tc.TransType2 == 21 || Tc.TransType2 == 22)
            {
                P12 = "CREDIT";
            }
            if (Tc.TransType2 == 11 || Tc.TransType2 == 12)
            {
                P12 = "DEBIT";
            }

            String P13 = Tc.AccNo2;

            String P14 = Tc.TransDesc2;

            String P21 = Us.UserName;

            String P15;

            String P16; 
            //TEST
            if(WLineAtmNo == "EWB311" || WLineAtmNo == "EWB511")
             {
                 Us.ReadUsersRecord("1005");
                 P15 = Us.UserName;
                   Us.ReadUsersRecord("487116");
                 P16 = Us.UserName;   
             }
            else
            {
                // This is the standard code
                 Ap.ReadAuthorizationForDisputeAndTransaction(WDisputeNo, WDisputeTranNo); 

            Us.ReadUsersRecord(Ap.Requestor);
            P15 = Us.UserName;
            
            Us.ReadUsersRecord(Ap.Authoriser);
            P16 = Us.UserName;   

            }

          
            Form56R3 ReportTrans = new Form56R3(P1, P2, P3, P4, P5, P6, P7,
                                   P8, P9, P10, P11, P12, P13, P14, P21, P15, P16 );
            ReportTrans.Show();

        }

        //
        // Print JCC Form  
        //
        private void PrintJCC()
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            Ac.ReadAtm(WLineAtmNo);
            RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
            Us.ReadUsersRecord(WSignedId);
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

            Tc.ReadTransToBePostedSpecific(WPostedNo);

            Ec.ReadErrorsTableSpecific(Tc.ErrNo);

            String P1 =Ac.Branch;
           
            String P2 = Ac.BranchName;
            String P3 = WLineAtmNo;
            String P4 = "";
            if (Tc.TransType == 21 || Tc.TransType == 22)
            {
                P4 = "CREDIT";
            }
            if (Tc.TransType == 11 || Tc.TransType == 12)
            {
                P4 = "DEBIT";
            }
            String P5 = Tc.CardNo;
          
            String P6 = Tc.TranAmount.ToString();
          
            String P7 = Tc.AtmDtTime.ToString();

            String P8 = Tc.AtmTraceNo.ToString();

            String P9 = Tc.RefNumb.ToString();

            String P10 = Tc.AuthCode.ToString();

            Ap.ReadAuthorizationForDisputeAndTransaction(WDisputeNo, WDisputeTranNo);

            Us.ReadUsersRecord(Ap.Requestor);
            String P15 = Us.UserName;

            Us.ReadUsersRecord(Ap.Authoriser);
            String P16 = Us.UserName;    

           
            String P21 = Us.UserName;        

            Form56R4 ReportTrans = new Form56R4(P1, P2, P3, P4, P5, P6, P7,
                                   P8, P9, P10, P15, P16, P21);
            ReportTrans.Show();

        }
        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Authorisation history
        private void button3_Click(object sender, EventArgs e)
        {

            //OurATMs-Matching-102

            if ( WReplenishment == true || WReconciliation == true)
            {
                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WLineAtmNo, WLineSesNo, "Replenishment");
                if (Ap.RecordFound == true)
                {
                }
                else
                {            
                  MessageBox.Show("No authorisation history for this testing ATM!");
                  return;                   
                }
            }
            if (WMatching == true)
            {   
                //TEST   
                // This part needs redesign

                WLineAtmNo = "EWB102";
                WLineSesNo = 149; 

                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WLineAtmNo, WLineSesNo, "ReconciliationCat");
                if (Ap.RecordFound == true)
                {
                  
                }
                else
                {
                    MessageBox.Show("No authorisation history for this testing ATM!");
                    return;
                }
            }

            if (Tc.OriginId == "02") // "02" BancNet Matching 
            {
                //TEST   
                // This part needs redesign

                WLineAtmNo = "EWB311";
                WLineSesNo = 106;

                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WLineAtmNo, WLineSesNo, "ReconciliationCat");
                if (Ap.RecordFound == true)
                {

                }
                else
                {
                    MessageBox.Show("No authorisation history for this BancNet category!");
                    return;
                }
            }
            if (Tc.OriginId == "07") // "07" Disputes
            {
                //TEST   
                // This part needs redesign

                WLineAtmNo = "";
                WLineSesNo = 0;
                WDisputeNo = Tc.DisputeNo;
                WDisputeTranNo = Tc.DispTranNo; 
            }
            else
            {
                WDisputeNo = 0 ;
                WDisputeTranNo = 0 ; 
            }

            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WLineAtmNo, WLineSesNo, WDisputeNo, WDisputeTranNo);
            NForm112.ShowDialog();
        }
// Chenge Radio button 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUnique.Checked == false)
            {
                radioButtonCard.Checked = false;
                radioButtonAccNo.Checked = false;
                radioButtonTraceNo.Checked = false;
                radioButtonAtmNo.Checked = false;
                radioButtonDispNo.Checked = false;
                textBox11.Text = ""; 
            }
            else
            {
            }
        }
// Show Big Grid 
        Form78b NForm78b;
        private void button4_Click(object sender, EventArgs e)
        {
            
            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            string WHeader = "SELECTED TRANSACTIONS";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Tc.TransToBePostedSelected, WHeader,"Form78");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog(); 
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //dataGridView1.Rows[NForm78b.WSelectedRow].Selected = true;
            if (NForm78b.UniqueIsChosen == 1)
            {
                Gridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId
                         + "' AND AtmDtTime  > @WDtFrom AND AtmDtTime <@WDtTo AND OpenRecord = 1 AND PostedNo =" + NForm78b.WPostedNo.ToString() + " " ;

                Tc.ReadAllTransToBePostedRange(Gridfilter, WDtFrom, WDtTo);

                WRange = true;

                ShowGrid(Gridfilter);

                buttonExpandGrid.Show(); // This button goes to show grid in bigger form 
            }
        }
// EXPAND GRID
        private void buttonExpandGridRight_Click(object sender, EventArgs e)
        {
            string WHeader = "LIST OF TRANSACTIONS TO BE POSTED" ;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Tc.TransToBePostedSelected, WHeader , "Form78");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog(); 
        }

    }
}

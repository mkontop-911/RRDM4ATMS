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

//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form116 : Form
    {
        Form5 NForm5;
   
        Form67 NForm67;
        Form78 NForm78;

        Form83 NForm83;
        Form84 NForm84;

        Form51 NForm51;
        Form71 NForm71; 
      
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
        RRDMMergedAtms_Jcc_HostTrans Mt = new RRDMMergedAtms_Jcc_HostTrans();

        RRDM_JccTransClass Jt = new RRDM_JccTransClass(); 


        string WCardNo;
        string WCardNoBin;
        string WAccNo; 
        int WTranNo;
        int WTranNoJCC;
        int WTraceNo;
        int SelMode;
        string WEnteredNumber;

        int WProcessMode; 

        string DR_CR;


        DateTime WFromDt ;
        DateTime WToDt ;

        int WIndex; 
   
        int WErrNo;
        string Gridfilter1;
        string Gridfilter2;
        string WAtmNo;
        int WSesNo;
        int WAtmTraceNo;
        int WFoundDisp; 
        string WBankId; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
   
        public Form116(string InSignedId, int InSignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;


            InitializeComponent();
            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            //TEST
            //TEST
            dateTimePicker1.Value = new DateTime(2014, 02, 12);
            dateTimePicker2.Value = new DateTime(2014, 02, 15);
            if (WSignedId == "03ServeUk") textBox1.Text = "serve67890123456";
      
         //   if (WSignedId == "1005") textBox1.Text = "4506531111117072";

            radioButton8.Checked = true; 

            label13.Hide();
            label24.Hide();
            panel3.Hide();
            panel6.Hide();
            buttonRegisterDisputeATM.Hide();

            label9.Hide();
            panel4.Hide(); 
          
            textBoxMsgBoard.Text = "Enter a Card No and examine information"; 
        }

        private void Form116_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet26.ErrorsTable' table. You can move, or remove it, as needed.
            this.errorsTableTableAdapter.Fill(this.aTMSDataSet26.ErrorsTable);
            // TODO: This line of code loads data into the 'aTMSDataSet18.MergeAtmsHostTransFile' table. You can move, or remove it, as needed.
            
    
        }

        // A CARD IS ENTERED - SHOW INFORMATION

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Show the JCC
            label9.Show();
            panel4.Show(); 

            if (radioButton8.Checked == true) SelMode = 8; // CARD
            if (radioButton9.Checked == true) SelMode = 9; // ACCOUNT
            if (radioButton10.Checked == true) SelMode = 10; // TRACE NUMBER 

            if (SelMode == 8 || SelMode == 9 || SelMode == 10)
            {
                // Check if Number is entered 
            //    if (String.IsNullOrEmpty(textBox1.Text))
             //   {
                    if (SelMode == 8)
                    {
                     //   MessageBox.Show("Enter Data for card such as 4506531111117072");
                     //   textBox1.Text = "4506531111117072";

                        WCardNo = textBox1.Text;
                        WCardNoBin = WCardNo.Substring(0, 6) + "******" + WCardNo.Substring(12, 4);
                    }
                    if (SelMode == 9)
                    {
                     //   MessageBox.Show("Enter Data for Account such as 222222222");
                     //   textBox1.Text = "222222222";
                        WAccNo = textBox1.Text;
                    }
                    if (SelMode == 10)
                    {
                    //    MessageBox.Show("Enter Data for TraceNumber such as 10042990 ");
                     //   textBox1.Text = "10042990";
                    }

                //    return;
              //  }
             //   else // There is value = something will be reported 
             //   {
                    WEnteredNumber = textBox1.Text;
              //  }
            }
            if (SelMode == 10)
            {

                if (int.TryParse(WEnteredNumber, out WTraceNo))
                {
                    WEnteredNumber = WTraceNo.ToString();
                }
                else
                {
                    MessageBox.Show(textBox1.Text, "Please enter a valid Trace number!");
                    return;
                }
            }

            WFromDt = dateTimePicker1.Value;
            WToDt = dateTimePicker2.Value;

            WFromDt = WFromDt.AddDays(-1);
            WToDt = WToDt.AddDays(1); 
            //
            // Read From merge file
            //
            Mt.ReadMergeFileCard(WOperator, SelMode, WEnteredNumber, WFromDt, WToDt, 0);

            if (Mt.RecordFound == false)
            {
                MessageBox.Show("No Transactions Found For this selection");
                return;
            }
            if (Mt.RecordFound == true) // TRansactions and errors found
            {
                label13.Show();
                label24.Show();
                panel3.Show();
                panel6.Show();
                buttonRegisterDisputeATM.Show();

     
            }
            if (SelMode == 8) // Card is inputed 
            {

                Gridfilter1 = "Operator ='" + WOperator + "' AND CardNo ='" + WCardNo
                               + "' AND AtmDtTime  > '" + WFromDt + "' AND AtmDtTime <'" + WToDt + "'";
            }

            if (SelMode == 9) // Avccount is inputed 
            {

                Gridfilter1 = "Operator ='" + WOperator + "' AND AccNo ='" + WAccNo
                               + "' AND AtmDtTime  > '" + WFromDt + "' AND AtmDtTime <'" + WToDt + "'";

            }

            if (SelMode == 10) // Trace No is inputed 
            {

                Gridfilter1 = "Operator ='" + WOperator + "' AND ATmTraceNo =" + WTraceNo; 
                              
            }
            // 
            // ATM TRans 
            //
            mergeAtmsHostTransFileBindingSource.Filter = Gridfilter1;
            this.mergeAtmsHostTransFileTableAdapter.Fill(this.aTMSDataSet18.MergeAtmsHostTransFile);

          
            dataGridView1.Rows[WIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WIndex));

            //
            // Transactions FROM JCC 
            //

            Jt.ReadJCC_TranFillTable(WCardNo, 0 , "" ); 

            dataGridView3.DataSource = Jt.JCC_TransSelected.DefaultView;

            dataGridView3.Columns[0].Width = 70 ; // RRN
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[1].Width = 70; // RRN
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[2].Width = 100; // HostDateTime
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[3].Width = 70; // Bank ID

            dataGridView3.Columns[4].Width = 70; // Trans type

            dataGridView3.Columns[5].Width = 100; // Trans Descr

            dataGridView3.Columns[6].Width = 70; // Curre Code

            dataGridView3.Columns[7].Width = 70; // TRAN amount 

            dataGridView3.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView3.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;
       
        }
        // ON SELECTED ROW OF TRANSACTIONS FOR ATMS
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WTranNo = (int)rowSelected.Cells[0].Value;

            Mt.ReadMergeFileTranNo(WOperator, WTranNo);

            WAtmNo = Mt.AtmNo;
            WSesNo = Mt.SesNo;
            WAtmTraceNo = Mt.AtmTraceNo;
            WBankId = Mt.BankId;

            // Check if already exist
            Dt.ReadDisputeTranForInPool(WTranNo);
            if (Dt.RecordFound == true)
            {
                textBox7.Text = Dt.DisputeNumber.ToString();
                WFoundDisp = Dt.DisputeNumber;
                MessageBox.Show("Dispute already registered for this transaction.");
                label16.Show();
                textBox7.Show();
                buttonRegisterDisputeATM.Hide(); 
            }
            else
            {
                label16.Hide();
                textBox7.Hide();
                WFoundDisp = 0 ;
                buttonRegisterDisputeATM.Show(); 
            }

            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox8.Text = "";

            button8.Hide();

            //
            // Check whether there is error with Card with BIN - not full card
            //
          
            Ec.ReadErrorsByCardNo(WOperator, SelMode, WEnteredNumber, WFromDt, WToDt, WCardNoBin);

            if (Ec.RecordFound == true)
            {
                if (SelMode == 8)
                {
                    Gridfilter2 = " Operator ='" + WOperator + "' AND (CardNo ='" + WCardNo + "' OR  CardNo ='" + WCardNoBin + "') "
                            + " AND DateTime  > '" + WFromDt + "' AND DateTime <'" + WToDt + "'";
                }
                if (SelMode == 9)
                {
                    Gridfilter2 = " Operator ='" + WOperator + "'"
                            + " AND DateTime  > '" + WFromDt + "' AND DateTime <'" + WToDt + "'" + " AND CustAccNo ='" + WCardNo + "'";
                }

                if (SelMode == 10)
                {
                    
                    Gridfilter2 = " Operator ='" + WOperator + "'"
                            + " AND DateTime  > '" + WFromDt + "' AND DateTime <'" + WToDt + "'" + "  AND TraceNo =" + WTraceNo;
                }

                errorsTableBindingSource.Filter = Gridfilter2;
                this.errorsTableTableAdapter.Fill(this.aTMSDataSet26.ErrorsTable);               
            }
            else
            {
                // Hide 
                label24.Hide();
                panel6.Hide();
            }
          
        }

        //
        // ERROR ROW ENTER FOR JCC
        //
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            WTranNoJCC = (int)rowSelected.Cells[0].Value;

        }   
        //
        // ERROR ROW ENTER for ERRORS  
        //
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WErrNo = (int)rowSelected.Cells[0].Value;

            button8.Show(); 

            textBox2.Text = WErrNo.ToString();

            Ec.ReadErrorsTableSpecific(WErrNo);

            // Read to find Index of error in transactions 

            DateTime From = dateTimePicker1.Value;
            DateTime To = dateTimePicker2.Value;
            //
            // Read From merge file
            //
            Mt.ReadMergeFileCard(WOperator, SelMode, WEnteredNumber, From, To, Ec.TransNo);

         //   Mt.ReadMergeFileCard(WOperator, WCardNo, Ec.TransNo);

            WIndex = Mt.Index;

            textBox6.Text = Ec.ErrDesc;

            if (Ec.OpenErr == true)
            {
                textBox3.Text = "Yes";
            }
            else textBox3.Text = "No";

            if (Ec.UnderAction == true || Ec.ManualAct == true)
            {
                button8.Show(); 

                Tc.ReadTransToBePosted(WErrNo, WCardNo);

                if (Tc.RecordFound == true)
                {
                    if (Tc.ActionCd2 == 0)
                    {
                        textBox4.Text = "Action not finalised yet";
                        textBox5.Text = "";
                    }
                    if (Tc.ActionCd2 == 1)
                    {
                        textBox4.Text = "Action finalised";
                        textBox5.Text = Tc.ActionDate.ToString();
                    }

                    if (Tc.ActionCd2 == 2)
                    {
                        textBox4.Text = "Action rejected";
                        textBox5.Text = Tc.ActionDate.ToString();
                        button8.Hide(); 
                    }

                    if (Tc.ActionCd2 == 3)
                    {
                        textBox4.Text = "Action postponed";
                        textBox5.Text = Tc.ActionDate.ToString();
                        button8.Hide(); 
                    }

                    label17.Show();
                    textBox8.Show();

                    if (Tc.RecordFound == true & (Tc.ActionCd2 == 0 || Tc.ActionCd2 == 2 || Tc.ActionCd2 == 3))
                    {
                        if (Tc.TransType == 11) DR_CR = "Debiting Customer was created but not posted yet";
                        if (Tc.TransType == 21) DR_CR = "Crediting Customer was created but not posted yet";
                        textBox8.Text = "Action Trans for: " + DR_CR;
                    }
                    if (Tc.RecordFound == true & Tc.ActionCd2 == 1)
                    {
                        if (Tc.TransType == 11) DR_CR = "Debiting Customer was created and posted";
                        if (Tc.TransType == 21) DR_CR = "Crediting Customer was created and posted";
                        textBox8.Text = "Action Trans for: " + DR_CR;
                    }
                    if (Tc.RecordFound == false)
                    {
                        textBox8.Text = "Action Not created yet for this error.";
                        button8.Hide(); 
                    }
                }
                else // Manual Action
                {
                    textBox4.Text = "Manual Action Taken";
                    textBox5.Text = "";
                    textBox8.Text = Ec.UserComment;
                    button8.Hide(); 
                }
            }
            else
            {
                textBox4.Text = "Action not taken yet";
                textBox5.Text = "N/A";
                label17.Hide();
                textBox8.Hide();
                button8.Hide(); 
            }

            textBoxMsgBoard.Text = "Examine information and GO to register dispute if need";
        }
      

        // SHOW RECONCILIATION MAIN FORM
        private void button2_Click(object sender, EventArgs e)
        {
            // UPDATE INTENTED FUNCTION 

            //   Us.ReadSignedActivityByKey(WSignRecordNo);
            //
            //    Us.ProcessNo = 2;

            //    Us.UpdateSignedInTableStepLevel(WSignRecordNo);

            //    NForm71 = new Form71(WSignedId, WSignRecordNo, WBankId, WPrive, WAtmNo, WSesNo);
            //   NForm71.FormClosed += NForm71_FormClosed;
            //   NForm71.Show();

            NForm83 = new Form83(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, WAtmTraceNo);
            NForm83.ShowDialog();

        }
        // Transaction info from Journal 
        private void button3_Click(object sender, EventArgs e)
        {
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 1; // Specific

            Tc.ReadInPoolTransSpecific(WTranNo);

            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, WAtmNo, Tc.AtmTraceNo, Tc.AtmTraceNo, Mode);
            NForm67.ShowDialog();
        }
        // SHOW FULL JOURNAL OF THE DAY
        private void button4_Click(object sender, EventArgs e)
        {
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";


            int Mode = 2 ; // FULL

            Tc.ReadInPoolTransSpecific(WTranNo);

            Ta.ReadSessionsStatusTraces(Tc.AtmNo, Tc.SesNo);

            RRDME_JournalTxtClass Jt = new RRDME_JournalTxtClass();

            Jt.ReadJournalTextByTrace(WBankId, WAtmNo, Ta.FirstTraceNo);

            int FileInJournal = Jt.FuId;

            // WE SHOULD FIND OUT THE START AND OF THIS REPL. CYCLE 
            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, FileInJournal, WAtmNo,  Ta.FirstTraceNo, Ta.LastTraceNo, Mode);
            NForm67.Show();

            
        }
        // SHOW VIDEO FOR THIS TRANS
        private void button5_Click(object sender, EventArgs e)
        {
            // Based on Transaction No show video clip 
            //TEST
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();
        }
        // SHOW CARD STATEMENT 
        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("FUTURE INTERFACE WITH HOST");
            return;
        }
        // REgister Dispute ATM

        private void buttonRegisterDisputeATM_Click(object sender, EventArgs e)
        {
            if (WFoundDisp == 0)
            {
                int From = 2 ; // Coming from Pre-Investigattion ATMs 
                NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, WCardNo, WTranNo, 0, 0,"" ,From, "ATM");
                NForm5.ShowDialog();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("This transaction already exist in dispute with number : " + WFoundDisp.ToString());
            }
        }

        // REGISTER DISPUTE JCC
        private void buttonRegisterDisputeJCC_Click(object sender, EventArgs e)
        {
            int From = 2; // Coming from Pre-Investigattion JCC
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, WCardNo, WTranNoJCC, 0, 0,"" ,From, "JCC");
            NForm5.ShowDialog();
            this.Dispose();

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
        /*
        // BIN Related Errors 
        private void button7_Click(object sender, EventArgs e)
        {
            if (Ec.RecordFound == true)
            {
                bool Dispute = true;
                string SearchFilter = "CardNo ='" + WCardNoBin + "'";
                NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId, WPrive, "", 0, "", Dispute, SearchFilter);
                NForm24.Show();
            }
            else
            {
                MessageBox.Show("Nothing to Show");
            }
            

        }
        */

         // SHOW CORRESPONDING DATES FOR TRANSACTION 
        private void button9_Click_1(object sender, EventArgs e)
        {
            NForm84 = new Form84(WSignedId, WSignRecordNo, WOperator, WAtmNo, WAtmTraceNo, WTranNo);
            NForm84.Show();
        }
     
        // Show Action 
        private void button8_Click(object sender, EventArgs e)
        {
            int Mode = 2;
            NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator ,"", 0, WErrNo, Mode);

            NForm78.Show();
        }
        // ON Change 
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            // textBox1.Text = "";
            textBox1.Text = "4506531111117072";
            WCardNo = "";
            WCardNoBin = "";
            WAccNo = "";
            WTraceNo = 0 ;
            label13.Hide();
            label24.Hide();
            panel3.Hide();
            panel6.Hide();
            buttonRegisterDisputeATM.Hide();
            label9.Hide();
            panel4.Hide(); 
        }
        // ON Change 
        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            // textBox1.Text = "";
            textBox1.Text = "222222222";
            WCardNo = "";
            WCardNoBin = "";
            WAccNo = "";
            WTraceNo = 0;
            label13.Hide();
            label24.Hide();
            panel3.Hide();
            panel6.Hide();
            buttonRegisterDisputeATM.Hide();
            label9.Hide();
            panel4.Hide(); 
        }
        // ON Change 
        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
           // textBox1.Text = "";
            textBox1.Text = "10042990";
            WCardNo = "";
            WCardNoBin = "";
            WAccNo = "";
            WTraceNo = 0 ;
            label13.Hide();
            label24.Hide();
            panel3.Hide();
            panel6.Hide();
            buttonRegisterDisputeATM.Hide();
            label9.Hide();
            panel4.Hide(); 
        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// REPLENISHMENT PLAYBACK
        private void buttonReplPlayBack_Click(object sender, EventArgs e)
        {
            Tc.ReadInPoolTransSpecific(WTranNo);

            Ta.ReadSessionsStatusTraces(Tc.AtmNo, Tc.SesNo);

            WProcessMode = Ta.ProcessMode; 

            if (WProcessMode > 0)
            {
                Us.ReadSignedActivityByKey(WSignRecordNo);
                Us.ProcessNo = 54; // View only for replenishment already done  
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm51.ShowDialog();

            }
            else
            {
                MessageBox.Show("Not allowed operation. Repl Workflow not done yet");
            }
        }
// RECONCILIATION PLAYBACK
        private void buttonRecPlayBack_Click(object sender, EventArgs e)
        {
            Tc.ReadInPoolTransSpecific(WTranNo);

            Ta.ReadSessionsStatusTraces(Tc.AtmNo, Tc.SesNo);

            WProcessMode = Ta.ProcessMode; 

            if (WProcessMode > 1)
            {
                Us.ReadSignedActivityByKey(WSignRecordNo);
                Us.ProcessNo = 54; // View only for reconciliation already done  
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                NForm71 = new Form71(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm71.ShowDialog();

            }
            else
            {
                MessageBox.Show("Not allowed operation. Reconciliation Workflow not done yet");
            }
        }  
       
    }
}

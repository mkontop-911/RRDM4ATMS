using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form4 : Form
    {
     
        Form109 NForm109;
        Form60 NForm60;

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); 

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        int WMaskRecordId;
     
        string WCardNo;
  
        string WSignedId;
        int WSignRecordNo;
        string WBankId;
        string WOperator;
   
        int WDispNo;
        //int WDispTranNo;

        public Form4(string InSignedId, int InSignRecordNo, string InOperator, string InBankId, int InDispNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WBankId = InBankId;
            WOperator = InOperator;
      
            WDispNo = InDispNo;
            //WDispTranNo = InDispTranNo; 

            InitializeComponent();

            radioButton1.Checked = true;

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelStep1.Text = "Investigation for Dispute No: " + WDispNo;

            textBoxMsgBoard.Text = "Take action after investigation"; 
        }

        private void Form4_Load(object sender, EventArgs e)
        {          
            // Totals of other disputes for this card
            Di.ReadDispute(WDispNo);

            WCardNo = Di.CardNo;

            textBox3.Text = Di.CustName;

            tbComments.Text = Di.DispComments;

            string TDispType = "251"; // Parameter Id for dispute types

            Gp.ReadParametersSpecificId(WOperator, TDispType, Di.DispType.ToString(), "", "");

            textBox4.Text = Gp.OccuranceNm;

            if (Di.DispType == 5) // Means Other type of Dispute 
            {
                label3.Show();
                textBox13.Show();
                textBox13.Text = Di.OtherDispTypeDescr;
            }
            else
            {
                label3.Hide();
                textBox13.Hide();
            }

            textBox7.Text = Di.TargetDate.ToString();

            TimeSpan Remain = Di.TargetDate - DateTime.Now;
            textBox8.Text = Remain.TotalHours.ToString("#,##0.00");

            //TEST
            if (WDispNo == 5)
            {
                radioButton1.Checked = true;
                checkBox3.Checked = true;
            }

            Di.ReadDisputeTotals(Di.CardNo, WBankId);

            Di.TotalForCard = Di.TotalForCard - 1; // EXCLUDE THIS DISPUTE FROM TOTALS 

            textBox2.Text = Di.TotalForCard.ToString();

            // Total transactions for this dispute that are not closed

            Dt.ReadAllTranForDispute(WDispNo);

            textBox11.Text = Dt.OpenDispTran.ToString();
            textBox12.Text = Dt.SettleDispTran.ToString();

            if (Dt.OpenDispTran == 0)
            {
                label20.Text = " Dispute is settled ";
                textBox8.Hide();
                buttonView.Show();
                buttonFinish.Show();
                buttonFinish.Text = "Finish" ;
                textBoxMsgBoard.Text = "Dispute is settled. You can reviw actions if needed"; 
            }
            else
            {
                label20.Text = "Remaining Hours"; 
                textBox8.Show();
            }

            Color Black = Color.Black;

            Color Red = Color.Red;

            if (Remain.TotalHours < 50)
            {
                label20.ForeColor = Red;
            }
            if (Remain.TotalHours > 50)
            {
                label20.ForeColor = Black;
            }

           

            // Show GRID Of Dispute transactions 

            //string Gridfilter = "DisputeNumber=" + WDispNo;

            Dt.ReadDisputeTransDataTable(WDispNo);

            ShowGridDisputeTrans();     
           
        }

        // Data Grid Row Enter 
        int WTransNo; 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WTransNo= Dt.DispTranNo = (int)rowSelected.Cells[0].Value;

            // Chosen Transaction
            Dt.ReadDisputeTran(WTransNo); // READ TRANSACTION

            WMaskRecordId = Dt.UniqueRecordId;
            textBoxTerminal.Text = Dt.AtmNo; 
            textBoxCard.Text = Dt.CardNo;
            textBoxAccNo.Text = Dt.AccNo;
            textBoxTransAmnt.Text = Dt.TranAmount.ToString("#,##0.00");        
            textBoxDispAmnt.Text = Dt.DisputedAmt.ToString("#,##0.00");

            if (Dt.ClosedDispute == true)
            {
                // Dispute is settled 
                buttonView.Show();
                buttonNext.Hide();
                textBoxSettledMsg.Show();
            }
            else
            {
                buttonView.Hide();
                buttonNext.Show();
                textBoxSettledMsg.Hide();
            }

                if (Dt.ErrNo > 0)
            {
                // NOTES for Attachements 
                Order = "Descending";
                WParameter4 = "UniqueRecordId: " + Dt.UniqueRecordId;
                WSearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
                if (Cn.RecordFound == true)
                {
                    labelNumberNotes2.Text = Cn.TotalNotes.ToString();
                }
                else labelNumberNotes2.Text = "0";
            }

            Ap.ReadAuthorizationForDisputeAndTransaction(WDispNo, WMaskRecordId);
            if (Ap.RecordFound == true & Ap.OpenRecord == true )
            {
                
                label6.Show();
                label7.Show();
                label6.Text = "Authorization in process ";
                label7.Text = "Authorization creation date : " + Ap.DateOriginated.ToString();

                Color Red = Color.Red;

                label6.ForeColor = Red;
                label7.ForeColor = Red;
            }
            else
            {
                label6.Hide();
                label7.Hide(); 
            }
        }  

        void NForm109_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form4_Load(this, new EventArgs());
        }
        
        // SHOW THE NUMBER OF OTHER DISPUTES 
        private void button4_Click(object sender, EventArgs e)
        {
            if (Di.TotalForCard > 0)
            {
                NForm60 = new Form60(WSignedId, WSignRecordNo, WOperator, WCardNo);
                NForm60.ShowDialog();
            }
            else
            {
                MessageBox.Show("No other Disputes for this card"); 
            }  
        }
        //
        // Proceed to action 
        //

        private void buttonNext_Click(object sender, EventArgs e)
        {
            
            Di.ReadDispute(WDispNo);

            //if (Di.OwnerId.Trim().Equals(WSignedId.Trim(), StringComparison.InvariantCultureIgnoreCase)
            //       & Di.Active == true)
            if (WSignedId.Trim().ToUpper() == Di.OwnerId.Trim().ToUpper())
            {
                ///
            }
            else
            {
                MessageBox.Show("Warning : You are not the owner of this Active dispute. You cannot move to action!");
                return;
            }
            //if (Di.OwnerId.Trim() != WSignedId.Trim() & Di.Active == true)
            //{
            //    MessageBox.Show("Warning : You are not the owner of this Acive dispute. You cannot move to action!");
            //    return;
            //}

            if (Dt.OpenDispTran == 0)
            {

                if (MessageBox.Show("MSG986 - Dispute is already settled."
                    + "Do you want to continue to review action?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                               == DialogResult.Yes)
                {
                    int WOrigin = 1; // From Requestor 
                    NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator,WDispNo, Dt.DispTranNo, WMaskRecordId, WOrigin);
                 
                 //   NForm109.FormClosed += NForm109_FormClosed;
                    NForm109.ShowDialog();
                }
                else
                {
                    return;
                }

            }
            else
            {
                // Check if Replenishment outstanding for this Dispute
                // Rule: You take action only after Replenishment is done
                Dt.ReadDisputeTran(Dt.DispTranNo);          

                // Find Unique 
                bool PresenterError = false; 
                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                Mpa.ReadInPoolTransSpecificUniqueRecordId(Dt.UniqueRecordId, 2);

                if (Mpa.MetaExceptionId == 55)
                {
                    PresenterError = true; 
                }
                RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                if (Mpa.ReplCycleNo ==0)
                {
                    // Check if status has been changed 
                    
                    Mpa.ReplCycleNo = Ta.ReadFindReplCycleForGivenDate(Mpa.TerminalId, Mpa.TransDate);

                    // If there is value now  ...  update the cycle 
                    if (Mpa.ReplCycleNo>0)
                    {
                        Mpa.UpdateMatchingTxnsMasterPoolATMsManual(Mpa.Operator, Mpa.SeqNo);
                    }             
                }
               

                if (Mpa.ReplCycleNo > 0)

                    // Check if there is a Cycle for this transaction
                    if (Mpa.ReplCycleNo > 0)
                {
                    // There is a replenishment Cycle 
                }
                else
                {
                    MessageBox.Show("There is no Replenishment Cycle for this transaction " + Environment.NewLine
                       + "Wait till one is created " + Environment.NewLine
                       + "Or act manually." + Environment.NewLine
                       );

                    return;
                }

                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                Aoc.ReadActionsOccurancesByUniqueRecordId(Mpa.UniqueRecordId); 

                if (Aoc.RecordFound == true & Aoc.Is_GL_Action)
                {
                    MessageBox.Show("An action is under way for this transaction " + Environment.NewLine
                        + "Action Id:" + Aoc.ActionId + Environment.NewLine
                        + "Maker Id:" + Aoc.Maker + Environment.NewLine
                        ); 
                }

                string WAtmNo = Mpa.TerminalId;
                int WReplNo = Mpa.ReplCycleNo;

                //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 

                // Read Traces to Process
                Ta.ReadSessionsStatusTraces(WAtmNo, WReplNo);

                if (Ta.ProcessMode ==0 & WAtmNo != "00000550")
                {
                  if (Ta.Recon1.RecStartDtTm != NullPastDate)
                    {
                        if (PresenterError == true)
                        {
                            if (MessageBox.Show("The Replenishment Cycle is under process" + Environment.NewLine
                                     + "Started at.."+ Ta.Recon1.RecStartDtTm.ToString() + Environment.NewLine
                                     + "Do you want to proceed with risks?" + Environment.NewLine
                                     + "Note that this is a presenter error" + Environment.NewLine
                                     , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                    == DialogResult.Yes)
                            {
                                // YES

                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {

                            if (MessageBox.Show("The Replenishment Cycle is under process" + Environment.NewLine
                                 + "Started at.." + Ta.Recon1.RecStartDtTm.ToString() + Environment.NewLine
                                      + "Do you want to proceed with risks?" + Environment.NewLine
                                    , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                   == DialogResult.Yes)
                            {
                                // YES

                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (PresenterError == true)
                        {
                            if (MessageBox.Show("The Replenishment Cycle is under process" + Environment.NewLine
                                     + "Started at.." + Ta.Recon1.RecStartDtTm.ToString() + Environment.NewLine
                                     + "Do you want to proceed?" + Environment.NewLine
                                     + "Note that this is a presenter error" + Environment.NewLine
                                     , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                    == DialogResult.Yes)
                            {
                                // YES

                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {

                            if (MessageBox.Show("The Replenishment Cycle is under process" + Environment.NewLine
                                 + "Started at.." + Ta.Recon1.RecStartDtTm.ToString() + Environment.NewLine
                                      + "Do you want to proceed?" + Environment.NewLine
                                    , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                   == DialogResult.Yes)
                            {
                                // YES

                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                  

                }
                if (Ta.ProcessMode > 0)
                {
                    
                    if (PresenterError == true)
                    {
                        MessageBox.Show("Note that The Replenishment Cycle has been completed" + Environment.NewLine
                                 + "Note that this is a presenter error"
                                  );
                    }
                    else
                    {
                        MessageBox.Show("Note that The Replenishment Cycle has been completed"
                                  + ""
                                   );
                    }
                }

                // Update Us Process number
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 99; // View Only 
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                int WOrigin = 1;
                NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator, WDispNo, Dt.DispTranNo, WMaskRecordId, WOrigin);

                NForm109.FormClosed += NForm109_FormClosed;
                NForm109.ShowDialog();
            }

        }
        // Show settled ACtion 
        private void buttonView_Click(object sender, EventArgs e)
        {
            //Ap.ReadAuthorizationForDisputeAndTransaction(WDispNo, WTranNo);
            //if (Ap.RecordFound == true)
            //{
            //    WAuthSeqNumber = Ap.SeqNumber;
            //}
            //else
            //{
            //    WAuthSeqNumber = 0;
            //}
            // Update Us Process number
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54; // View Only 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


            int WOrigin = 11;
            NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator, WDispNo, Dt.DispTranNo, WMaskRecordId, WOrigin);

            NForm109.FormClosed += NForm109_FormClosed;
            NForm109.ShowDialog();
        }
        //
        //NOTES 
        

        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "UniqueRecordId: " + Dt.UniqueRecordId;
            //string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            // "UniqueRecordId: " + Mpa.UniqueRecordId;
            string SearchP4 = "";
            string WMode;
            if (Di.Active == true) WMode = "Update";
            else WMode = "Read";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.FormClosed += NForm197_FormClosed;
            NForm197.ShowDialog();
        }


        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form4_Load(this, new EventArgs());
        }
// Close Form 
        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Investigate and come back 
        private void button2_Click(object sender, EventArgs e)
        {
            Form80b NForm80b;

            string WFunction = "Investigation";

            //Form80b(string InSignedId, int InSignRecordNo, string InOperator, string InCategoryId, int InRMCycleNo,
            //                                     string InStringUniqueId, int InIntUniqueId, int InUniqueIdType, string InFunction)

            int UniqueIdType = 4;

            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate,"" ,"", 0,"" ,Dt.UniqueRecordId , UniqueIdType, WFunction,"", 0);

            NForm80b.ShowDialog();    
        }

        //**********************************************************************
        // END NOTES 
        //**********************************************************************  

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGridDisputeTrans()
        {
            dataGridView2.DataSource = Dt.DisputeTransDataTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }
            dataGridView2.Columns[0].Width = 70; // DispTranNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 70; // MaskRecordId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 100; //  TranDate
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView2.Columns[3].Width = 80; // TranAmount
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[4].Width = 80; // DisputedAmt
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[5].Width = 80; // DecidedAmount 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //
            // DATA TABLE ROWS DEFINITION 
            //
            //ATMsDetailsDataTable.Columns.Add("DispTranNo", typeof(int));
            //ATMsDetailsDataTable.Columns.Add("MaskRecordId", typeof(int));
            //ATMsDetailsDataTable.Columns.Add("TranDate", typeof(DateTime));
            //ATMsDetailsDataTable.Columns.Add("TranAmount", typeof(string));
            //ATMsDetailsDataTable.Columns.Add("DisputedAmt", typeof(string));
            //ATMsDetailsDataTable.Columns.Add("DecidedAmount", typeof(string));
        }
    }
}

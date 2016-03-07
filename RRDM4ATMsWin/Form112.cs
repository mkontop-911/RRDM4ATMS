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

namespace RRDM4ATMsWin
{
    public partial class Form112 : Form
    {
        Form109 NForm109;
        Form110 NForm110;
        Form51 NForm51;
        Form71 NForm71;
        Form271 NForm271;
        Form52b NForm52b; 

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass(); 
        RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass(); 
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 

        string WGridFilter; 
        int WSecLevel;

        string WOrigin ;
        int WTranNo;

        bool WTransfered;
        string WReasonOfTransfer; 
        
        int TempReplCycle ;

        int WSeqNumber;
        int WStage;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WFunction;
        string WAtmNo ;
        int WSesNo ;
        int WDisputeNo ;
        int WDisputeTranNo ; 

        public Form112(string InSignedId, int InSignRecordNo, string InOperator, string InFunction, 
            string InAtmNo, int InSesNo, int InDisputeNo, int InDisputeTranNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // Normal and  History 
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WDisputeNo = InDisputeNo;
            WDisputeTranNo = InDisputeTranNo; 

            InitializeComponent();

            buttonDelete.Hide();
        }

        private void Form112_Load(object sender, EventArgs e)
        {
          
            if (WFunction == "Normal")
            {
                WGridFilter = " (Operator ='" + WOperator + "' AND Requestor ='" + WSignedId + "' AND OpenRecord = 1) "
                        + "  OR  (Operator ='" + WOperator + "' AND Authoriser ='" + WSignedId + "' AND OpenRecord = 1) ";
            }
            if (WFunction == "History")
            {
                if (WAtmNo != "")
                {
                    WGridFilter = " Operator ='" + WOperator + "' AND AtmNo ='" + WAtmNo
                                + "' AND ReplCycle =" + WSesNo;
                }
                if (WDisputeNo > 0)
                {
                    WGridFilter = " Operator ='" + WOperator + "' AND DisputeNumber =" + WDisputeNo
                                + " AND DisputeTransaction =" + WDisputeTranNo;
                }
                
            }
          
            authorizationTableBindingSource.Filter = WGridFilter;
            this.authorizationTableTableAdapter.Fill(this.aTMSDataSet40.AuthorizationTable);

            if (dataGridView1.Rows.Count == 0)
            {
                this.Dispose();

                if (WFunction == "History")
                {
                    MessageBox.Show("No History Data!");
                }
                else
                {
                    MessageBox.Show("No other authorisations to manage!");
                }
                
                return;
            }

            Us.ReadUsersRecord(WSignedId);
            WSecLevel = Us.SecLevel; 
        }
        // Row Enter 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            buttonDelete.Hide(); 

            WSeqNumber = (int)rowSelected.Cells[0].Value;
            
            Ap.ReadAuthorizationSpecific(WSeqNumber);

            WOrigin = Ap.Origin;
            WTranNo = Ap.TranNo;
            WAtmNo = Ap.AtmNo;
            TempReplCycle = Ap.ReplCycle;
            WTransfered = Ap.Transfered; 
            WReasonOfTransfer = Ap.ReasonOfTransfer; 

            if (WOrigin == "Dispute Action")
            {
                // Customer Name 
                Di.ReadDispute(Ap.DisputeNumber);

                Descr.Text = "Dispute Authorisaton for customer : " + Di.CustName;
            }
          
            if (WOrigin == "Replenishment")
            {
                Descr.Text = "Replenishment Authorisaton for Atm No : " + WAtmNo + " and Repl Cycle : " + TempReplCycle.ToString();
            }

            if (WOrigin == "Reconciliation")
            {
                Descr.Text = "Reconciliation Authorisaton for Atm No : " + WAtmNo + " and Repl Cycle : " + TempReplCycle.ToString();
            }

            if (WOrigin == "ReconciliationCat")
            {
                Descr.Text = "Reconciliation Authorisaton for Category No : " + WAtmNo + " and Matching Session : " + TempReplCycle.ToString();
            }

            if (WOrigin == "ForceMatchingCat")
            {
                Descr.Text = "Force Matching Authorisation for Category No : " + WAtmNo + " and Matching Session : " + TempReplCycle.ToString();
            }

            if (WOrigin == "ReconciliationBulk")
            {
                Descr.Text = "Bulk Reconciliation Authorisaton For ATMs" ;
            }

            Us.ReadUsersRecord(Ap.Requestor);
            label1.Text = "Requestor : " + Us.UserName;

            Us.ReadUsersRecord(Ap.Authoriser);
            label2.Text = "Authoriser : " + Us.UserName;

            WStage = Ap.Stage; // If Stage = 2 then this is for auathorizer if 4 then this is for the requestor 
            //LabelStage.Text = "Stage : " + WStage.ToString();

            label4.Text = "Date Created : " + Ap.DateOriginated.ToString();

            if (Ap.AuthComment != "")
            {
                textBoxCommnet.Text = Ap.AuthComment;
                textBoxCommnet.Show();
                label5.Show();
                label6.Show();
            }
            else
            {
                textBoxCommnet.Hide();
                label5.Hide();
                label6.Hide();
            }

            if (WStage == 1 & WSignedId == Ap.Requestor)
            {
                textBoxMessage.Text = "Authoriser not available yet.";
                // You can delete if you want 
                buttonDelete.Show();
                buttonNext.Text = "View";
                buttonTransfer.Show(); 
            }

            if (WStage == 2 & WSignedId == Ap.Requestor)
            {
                textBoxMessage.Text = "Authoriser didn't authorise yet.";
                buttonDelete.Show();
                buttonNext.Show(); 
                buttonNext.Text = "View";
                buttonTransfer.Show(); 
            }

            if (WStage == 2 & WSignedId == Ap.Authoriser)
            {
                textBoxMessage.Text = "Go to Authorise";
                buttonNext.Show(); 
                buttonNext.Text = "Go to Authorise";
                buttonTransfer.Hide();
                buttonDelete.Hide();
            }
            if (WStage == 3 & WSignedId == Ap.Authoriser)
            {
                textBoxMessage.Text = "Requestor is not available yet.";
                buttonNext.Show(); 
                buttonNext.Text = "View";
                buttonTransfer.Show();
                buttonTransfer.Text = "Finish" ;
                buttonDelete.Hide();
            }
            if (WStage == 4 & WSignedId == Ap.Authoriser)
            {
               
                if (Ap.AuthDecision == "NO")
                {
                    textBoxMessage.Text = "Requestor didn't take corrective action yet.";
                }

                if (Ap.AuthDecision == "YES")
                {
                    textBoxMessage.Text = "Requestor didn't finish process yet.";
                }
                
                buttonNext.Show(); 
                buttonNext.Text = "View";
                buttonTransfer.Show();
                buttonTransfer.Text = "Finish";
                buttonDelete.Hide();
            }
            if (WStage == 4 & WSignedId == Ap.Requestor)
            {
                if (Ap.AuthDecision == "NO")
                {
                    textBoxMessage.Text = "Rejected. Correct and resubmit.";
                }

                if (Ap.AuthDecision == "YES")
                {
                    textBoxMessage.Text = "Authorisation Accepted. Go to finish process! ";
                }
                
                buttonNext.Show(); 
                buttonNext.Text = "Next";
                buttonTransfer.Hide();
                buttonDelete.Hide();
            }

            if (WStage == 5 )
            {
                textBoxMessage.Text = "Closed Record!";
                buttonNext.Hide(); 
                buttonTransfer.Hide();
                buttonDelete.Hide();
                buttonTransfer.Show();
                buttonTransfer.Text = "Finish";
            }

            if (WFunction == "History" )
            {
                label22.Text = "History of Authorisations "; 
                buttonNext.Hide();
                buttonTransfer.Hide();
                buttonDelete.Hide();
                buttonTransfer.Show();
                buttonTransfer.Text = "Finish";
            }

            if (Ap.Stage == 1) LabelStage.Text = "Stage : " + WStage.ToString() + " Authoriser Not Available yet.";
            if (Ap.Stage == 2) LabelStage.Text = "Stage : " + WStage.ToString() + " Authoriser got the message.";
            if (Ap.Stage == 3) LabelStage.Text = "Stage : " + WStage.ToString() + " Authoriser took action";
            if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation accepted. Ready for updating";
            }
            if (Ap.Stage == 5 & Ap.AuthDecision == "YES")
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation accepted. Updating done";
            }
            if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation REJECTED. ";
                Color Red = Color.Red;
                LabelStage.ForeColor = Red;
            }
            else
            {
                Color Black = Color.Black;
                LabelStage.ForeColor = Black;
            }
            if (Ap.Stage == 5 & Ap.AuthDecision == "NO")
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation REJECTED. Record closed ";
                Color Red = Color.Red;
                LabelStage.ForeColor = Red;
            }
            else
            {
                Color Black = Color.Black;
                LabelStage.ForeColor = Black;
            }

            if (WTransfered == true)
            {
                LabelStage.Text = "Stage : " + WStage.ToString() + " Authorisation was transfered! Due to: " + WReasonOfTransfer ;
                textBoxMessage.Text = "Authorisation was transfered"; 
            }

            //if (WSecLevel == 4)
            //{
            //    buttonDelete.Show();
            //}
           

        }
        //
        // Delete WHEN AUTHORISER IS NOT AVAILABLE
        //
        private void buttonDelete_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Warning: By deleting this record you have to start process right from the start."  +Environment.NewLine
                    + " Do you want to delete this authorisation record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                            == DialogResult.Yes)
            {
                Ap.DeleteAuthorisationRecord(WSeqNumber);   

                if (WOrigin == "Dispute Action")
                {

                    // Reverse updating in Dispute 
                    Dt.ReadDisputeTran(Ap.DisputeTransaction);

                    //***********************
                    // UPDATE DISPUTE RECORD
                    Dt.ChooseAuthor = false;
                    Dt.PendingAuthorization = false;
                    Dt.AuthorOriginator = "";
                    Dt.AuthorKey = 0;
                    Dt.Authoriser = "";
                    Dt.DisputeActionId = 0;
                    Dt.ReasonForAction = 0;
                    Dt.ActionComment = "";

                    //Dt.Authorised = true;
                    //Dt.RejectedFromAuth = false; 
                    //Dt.AuthoriserComment = "" ; 
                    Dt.UpdateDisputeTranRecord(Ap.DisputeTransaction); 
                }

                Ap.DeleteAuthorisationRecord(WSeqNumber);

                if (WOrigin == "Replenishment")
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
                    Us.ProcessNo = 1;
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WOrigin == "Reconciliation")
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo); // Requestor becomes view only 
                    Us.ProcessNo = 2;
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                // Load to refresh
                // 
                Form112_Load(this, new EventArgs());
            }
            else
            {
            }
        }

        // Go next step to authorise 
        private void buttonNext_Click(object sender, EventArgs e)
        {

            if (WOrigin == "Replenishment")
            {
                if (WSignedId == Ap.Authoriser)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 55; //  Authoriser 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WSignedId == Ap.Requestor)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 56; // Requestor 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, TempReplCycle);
                NForm51.FormClosed += NForm51_FormClosed;
                NForm51.ShowDialog();     
            }
          

            if (WOrigin == "Reconciliation")
            {
                if (WSignedId == Ap.Authoriser)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 55; // Authoriser  
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WSignedId == Ap.Requestor)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 56; // Requestor 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                NForm71 = new Form71(WSignedId, WSignRecordNo, WOperator, WAtmNo, TempReplCycle);
                NForm71.FormClosed += NForm71_FormClosed;
                NForm71.ShowDialog();
            }

            if (WOrigin == "ReconciliationCat")
            {
                if (WSignedId == Ap.Authoriser)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 55; // Authoriser  
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WSignedId == Ap.Requestor)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 56; // Requestor 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WAtmNo, TempReplCycle);
                NForm271.FormClosed += NForm271_FormClosed;
                NForm271.ShowDialog();
            }

            if (WOrigin == "ForcMatchingCat")
            {
                if (WSignedId == Ap.Authoriser)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 55; // Authoriser  
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WSignedId == Ap.Requestor)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 56; // Requestor 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                Form19c NForm19c;

                NForm19c = new Form19c(WSignedId, WSignRecordNo, WOperator, WAtmNo, TempReplCycle);
                NForm19c.FormClosed += NForm19c_FormClosed;
                NForm19c.ShowDialog();
            }

            if (WOrigin == "ReconciliationBulk")
            {
                if (WSignedId == Ap.Authoriser)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 55; // Authoriser  
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WSignedId == Ap.Requestor)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 56; // Requestor 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                NForm52b = new Form52b(Ap.Requestor, WSignRecordNo, WOperator, 11);
                NForm52b.FormClosed += NForm52b_FormClosed;
                NForm52b.ShowDialog();
            }

            if (WOrigin == "Dispute Action")
            {
                if (WSignedId == Ap.Authoriser)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 55; // Authoriser  
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (WSignedId == Ap.Requestor)
                {
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 56; // Requestor 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                int WSource = 2; // Comes from authorisation process 

                // From Authoriser 

                NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator, Ap.DisputeNumber, Ap.DisputeTransaction, Ap.TranNo, WSource);
                NForm109.FormClosed += NForm109_FormClosed;
                NForm109.ShowDialog();           
            }
             
        }

        void NForm19c_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form112_Load(this, new EventArgs()); 
        }

        void NForm271_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form112_Load(this, new EventArgs()); 
        }

        void NForm52b_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form112_Load(this, new EventArgs()); 
        }

        void NForm71_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form112_Load(this, new EventArgs()); 
        }

        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form112_Load(this, new EventArgs()); 
        }

        void NForm109_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form112_Load(this, new EventArgs()); 
        }
        // Go to tansfer from one Authoriser to other 
        private void buttonTransfer_Click(object sender, EventArgs e)
        {
            if (buttonTransfer.Text == "Finish")
            {
                this.Close();
                return; 
            }
            int AuthorSeqNumber = WSeqNumber ; // This is used >0 when calling from Authorization management 
                NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WAtmNo, TempReplCycle, AuthorSeqNumber, "Transfer");
                NForm110.FormClosed += NForm110_FormClosed;
                NForm110.ShowDialog();

        }

        void NForm110_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form112_Load(this, new EventArgs()); 
        }
    }
}

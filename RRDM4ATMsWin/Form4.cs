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
    public partial class Form4 : Form
    {
     
        Form109 NForm109;
        Form60 NForm60;
        Form67 NForm67;
        Form71 NForm71; 

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
        RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); 

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        int WTranNo;
        string WAtmNo;
        int WSesNo;
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

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

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

            // NOTES for Attachements 
            Order = "Descending";
            WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            string Gridfilter = "DisputeNumber=" + WDispNo;
            disputesTransTableBindingSource.Filter = Gridfilter;
         //   dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
            this.disputesTransTableTableAdapter.Fill(this.aTMSDataSet49.DisputesTransTable);
           
        }

        // Data Grid Row Enter 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WTranNo = (int)rowSelected.Cells[0].Value;

            // Chosen Transaction
            Dt.ReadDisputeTran(WTranNo); // READ TRANSACTION

            textBoxCard.Text = Dt.CardNo;
            textBoxAccNo.Text = Dt.AccNo;
            textBoxTransAmnt.Text = Dt.TranAmount.ToString("#,##0.00");        
            textBoxDispAmnt.Text = Dt.DisputedAmt.ToString("#,##0.00");

            if (Dt.ErrNo > 0)
            {
                //// Read error to see if it is ATM or Host 

                //Ec.ReadErrorsTableSpecific(Dt.ErrNo);

                //if (Ec.ErrId < 100)
                //{
                //    //textBox1.Text = "1";
                 
                //}

                //if (Ec.ErrId > 100 & Ec.ErrId < 200)
                //{
                //    textBox1.Text = "1";
                //}

                //if (Ec.ErrId > 200 )
                //{
                //    textBox1.Text = "1";
                 
                //}

                //textBox5.Text = Ec.ActionDtTm.ToString(); 
                //textBox10.Text = Ec.ErrDesc; 
            }

            Ap.ReadAuthorizationForDisputeAndTransaction(WDispNo, WTranNo);
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
            
            NForm60 = new Form60(WSignedId, WSignRecordNo, WOperator, WCardNo);
            //   NForm71.FormClosed += NForm71_FormClosed;
            NForm60.ShowDialog();
        }
        //
        // Proceed to action 
        //
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (Di.OwnerId != WSignedId & Di.Active == true)
            {
                MessageBox.Show("Warning : You are not the owner of this Acive dispute. You cannot move to action!");
                return;
            }

            if (Dt.OpenDispTran == 0)
            {

                if (MessageBox.Show("MSG986 - Dispute is already settled."
                    + "Do you want to continue to review action?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                               == DialogResult.Yes)
                {
                    int WOrigin = 1; // From Requestor 
                    NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator,WDispNo, Dt.DispTranNo, WTranNo, WOrigin);
                 
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
                //Ap.ReadAuthorizationForDisputeAndTransaction(WDispNo, WTranNo);
                //if (Ap.RecordFound == true)
                //{
                //    WAuthSeqNumber = Ap.SeqNumber;
                //}
                //else
                //{
                //    WAuthSeqNumber = 0 ;
                //}

                int WOrigin = 1;
                NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator, WDispNo, Dt.DispTranNo, WTranNo, WOrigin);

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

            int WOrigin = 11;
            NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator, WDispNo, Dt.DispTranNo, WTranNo, WOrigin);

            NForm109.FormClosed += NForm109_FormClosed;
            NForm109.ShowDialog();
        }
        //
        //NOTES 
        

        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            string SearchP4 = "";
            string WMode;
            if (Di.Active == true) WMode = "Update";
            else WMode = "Read";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
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

            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, "", 0, Dt.MaskRecordId ,WFunction);

            NForm80b.ShowDialog();    
        }


      

        //**********************************************************************
        // END NOTES 
        //**********************************************************************  
    }
}

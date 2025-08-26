using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_POS_Reconc : Form
    {
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        string WSelectionCriteria;
       
        public int WPartnerSeqNo;
        public string WPartnerAccNo;

        int WSeqNo;

        string WOperator;
        string WSignedId;
        int WMpaSeqNo;
        int WMode; 
       
        DataTable WTable_POS_Reconc;

        public Form78d_POS_Reconc(string InOpertor, string InSignedId, int InMpaSeqNo 
                                                                    , DataTable InPOS_Recon, int InMode)
        {
            WOperator = InOpertor;
            WSignedId = InSignedId;
            WMpaSeqNo = InMpaSeqNo;

            WMode = InMode; // Mode = 1 ... originated from Adjustment Credit
                            // Mode = 2 ... Debit to get account number 

            WTable_POS_Reconc = InPOS_Recon;
           
            InitializeComponent();        
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            WPartnerSeqNo = 0 ;
            WPartnerAccNo = "";
            
            WSelectionCriteria = "WHERE SeqNo=" + WMpaSeqNo;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria, 1);

            if (WMode == 1)
            {
                labelWhatGrid.Text = "ADJUSTMENT - 20000 - POS MANAGEMENT ";
            }
            if (WMode == 2)
            {
                labelWhatGrid.Text = "Debit with AUTHNO:." + Mpa.RRNumber + ".- To Get Account - POS MANAGEMENT ";
            }

            textBoxCcy.Text = Mpa.TransCurr;
            textBoxTransAmt.Text = Mpa.TransAmount.ToString("#,##0.00");
            textBox_ACCEPTOR_ID.Text = Mpa.ACCEPTOR_ID;
            textBox_ACCEPTORNAME.Text = Mpa.ACCEPTORNAME;
            textBoxTransDate.Text = Mpa.TransDate.ToString();
            textBoxMaskCard.Text = Mpa.CardNumber; 
            textBoxCard_Encrypted.Text = Mpa.Card_Encrypted;
            if (Mpa.SpareField != "")
            {
                textBoxPartnerSeqNo.Text = Mpa.SpareField;
                //textBoxPartnerSeqNo.Show();
                //label1.Show(); 
                WPartnerSeqNo = Convert.ToInt32(Mpa.SpareField);
            }
            else
            {
                textBoxPartnerSeqNo.Text = "N/A";
                WPartnerSeqNo = 0; 
            }

            dataGridView1.DataSource = WTable_POS_Reconc.DefaultView;

            dataGridView1.Show();
               
            // SHOW GRID

        }

        // On ROW ENTER 
        decimal WCH_Amount = 0;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
            WCH_Amount = (decimal)dataGridView1.Rows[e.RowIndex].Cells["CH_Amount"].Value;

            WSelectionCriteria = " WHERE SeqNo="+ WSeqNo; 
            string TableName = "Switch_IST_Txns"; 
            Mgt.ReadTransSpecificFromBothTables_By_SelectionCriteria(WSelectionCriteria, TableName);

            textBoxCcy2.Text = Mgt.TransCurr;
            textBoxTransAmt2.Text = Mgt.TransAmt.ToString("#,##0.00");
            textBoxCH_Amount.Text = WCH_Amount.ToString("#,##0.00");
            textBox_ACCEPTOR_ID2.Text = Mgt.ACCEPTOR_ID;
            textBox_ACCEPTORNAME2.Text = Mgt.ACCEPTORNAME;
            textBoxTransDate2.Text = Mgt.TransDate.ToString();
            textBoxCard_Encrypted2.Text = Mgt.Card_Encrypted;
            textBoxMaskCard2.Text = Mgt.CardNumber;

            WPartnerAccNo = Mgt.AccNo; 

            if (Mgt.TransType == 11)
            {
                labelLineDetails.Text = "LINE DETAILS - DEBIT";
            }
            else
            {
                labelLineDetails.Text = "LINE DETAILS - CREDIT";
            }
            
            if (WPartnerSeqNo > 0)
            {
                if (WPartnerSeqNo == WSeqNo)
                {
                    buttonUndo.Show();
                    buttonMakePartner.Hide(); 
                }
                else
                {
                    buttonUndo.Hide();
                    buttonMakePartner.Hide();
                }
            }
            else
            {
                buttonMakePartner.Show();
            }
        }
       

        // Show Grid 03 
        public void ShowGrid03()
        {

            //dataGridView1.Columns[0].Width = 70; // WhatFile
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[1].Width = 60; // SeqNo
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

     

// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }


        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print
            string P1 = "";



           // if (WMode == 3)
           // {
              //  P1 = "Discrepancies for Category.." + WMatchingCateg + "..At Cycle.." + WRMCycle.ToString();
          //  }
           // else
           // {
                MessageBox.Show("No Printing for this"); 
            //}

            string P2 = "";  // Category 
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

                Form56R68ATMS_W_Pool_Table ReportATMS_Pool_Table = new Form56R68ATMS_W_Pool_Table(P1, P2, P3, P4, P5);
                ReportATMS_Pool_Table.Show();
           
        }
// MakePartner
        private void buttonMakePartner_Click(object sender, EventArgs e)
        {

            // Make Mpa.DepCount = WCH_Amount
            string WComments = "";
            decimal WD = WCH_Amount; 

            if (WMode == 1)
            {
                WComments = "Partner was Found for Credit";
            }
            if (WMode == 2)
            {
                WComments = "Partner was Found to get Accno ";
                WD = 0; 
            }

            Mpa.UpdateMpaRecordsWithPartnerDetails_POS(WMpaSeqNo,Mgt.AccNo ,WSeqNo.ToString(), WD
                                            ,WComments, 1);
                buttonUndo.Show();
                buttonMakePartner.Hide();
                Form78b_Load(this, new EventArgs());
            
        }
// UNDO
        private void buttonUndo_Click(object sender, EventArgs e)
        {
            // Update with blanks
            string WComments = "";
            Mpa.UpdateMpaRecordsWithPartnerDetails_POS(WMpaSeqNo,"" ,"",0
                                        ,WComments, 1);
            buttonUndo.Hide();
            buttonMakePartner.Show();
            Form78b_Load(this, new EventArgs());
        }
// FINISH 
        private void buttonFinish_Click_1(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}

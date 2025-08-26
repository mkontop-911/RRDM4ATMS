using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form14b : Form
    {
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        //DateTime NullPastDate = new DateTime(1950, 11, 21);

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public bool Confirmed;

        // public int MetaNumber;
        string WAccNo;
        string WCcy;
        decimal WAmount;
        string WTerminalId;
        int WRMCycle;

        string WSignedId;
        string WOperator;

        string WUniqueRecordIdOrigin;

        int WUniqueRecordId;

        string WActionId; // 

        public Form14b(string InSignedId, string InOperator,
                           string InUniqueRecordIdOrigin, int InUniqueRecordId, string InActionId
                          , string InMaker_ReasonOfAction, decimal InAmount, string InOriginWorkFlow, int InReplCycleNo)
        {
            WSignedId = InSignedId;
            WOperator = InOperator;

            WUniqueRecordIdOrigin = InUniqueRecordIdOrigin;
            WUniqueRecordId = InUniqueRecordId;

            WActionId = InActionId; // 

            WAmount = InAmount;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;
            // Call Procedures 

            // Find needed information
            string SelectionCriteria = " WHERE UniqueRecordId = " + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

            WAccNo = Mpa.AccNumber;
            WTerminalId = Mpa.TerminalId;


            WCcy = Mpa.TransCurr;
            WRMCycle = Mpa.MatchingAtRMCycle;

            // Create Body

            RRDMActions_GL Ag = new RRDMActions_GL();
            Ag.ReadActionByActionId_And_Occ(WOperator, WActionId, 1);

            labelStep1.Text = Ag.ActionNm;
            textBoxCcy.Text = WCcy;
            textBoxTransAmt.Text = WAmount.ToString("#,##0.00");
            textBoxAccNo.Text = WAccNo;
            textBoxTransDate.Text = Mpa.TransDate.ToString();
            textBoxCard_Encrypted.Text = Mpa.Card_Encrypted;
            textBoxMaskCard1.Text = Mpa.CardNumber;
            textBoxDesc.Text = Mpa.TransDescr;
            textBoxTrace.Text = Mpa.TraceNoWithNoEndZero.ToString();
            textBoxRRN.Text = Mpa.RRNumber;
            textBoxMask.Text = Mpa.MatchMask;

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WMaker_ReasonOfAction = InMaker_ReasonOfAction;

            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                  WActionId, WUniqueRecordIdOrigin,
                                                  WUniqueRecordId, Mpa.TransCurr, WAmount,
                                                  Mpa.TerminalId, InReplCycleNo, WMaker_ReasonOfAction, InOriginWorkFlow);
            if (Aoc.IsSuccess ==false)
            {
                return; 
            }
            else
            {
                dataGridActionTXNs.DataSource = Aoc.TxnsTableFromAction.DefaultView;

                ShowGrid_1();

                textBoxMsgBoard.Text = "Review and Confirm";
            }
            

        }


        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            // FIND CURRENT CUTOFF CYCLE
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WJobCategory = "ATMs";

            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WStage = "02"; // Confirmed by maker 
            Aoc.UpdateOccurancesStage("Master_Pool", WUniqueRecordId, WStage, DateTime.Now, WReconcCycleNo, WSignedId);

            //string SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;

            //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

            //Mpa.MetaExceptionNo = WUniqueRecordId;
            //Mpa.ActionByUser = true;
            //Mpa.UserId = WSignedId;

            //Mpa.ActionType = WActionId; // 

            Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);

            MessageBox.Show("Action has been confirmed by the Maker" + Environment.NewLine
                            + "The Transactions Created by this action " + Environment.NewLine
                            + "will be only be Settled after authorisation by Authoriser");
            this.Close();
        }

        private void ShowGrid_1()
        {
            dataGridActionTXNs.Columns[0].Width = 60; // SeqNumber
            dataGridActionTXNs.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[1].Width = 60; // ActionId
            dataGridActionTXNs.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridActionTXNs.Columns[2].Width = 60; // Occurance 
            dataGridActionTXNs.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridActionTXNs.Columns[3].Width = 250; // ActionNm
            dataGridActionTXNs.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[4].Width = 60; // Branch
            dataGridActionTXNs.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[5].Width = 120; // AccNo
            dataGridActionTXNs.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[6].Width = 50; // DR/CR
            dataGridActionTXNs.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridActionTXNs.Columns[7].Width = 120; // AccName
            dataGridActionTXNs.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[8].Width = 90; // Amount
            dataGridActionTXNs.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridActionTXNs.Columns[9].Width = 350; // Description
            dataGridActionTXNs.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[10].Width = 60; // Stage 
            dataGridActionTXNs.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }
// FINISH 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}


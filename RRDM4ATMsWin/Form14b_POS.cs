using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
namespace RRDM4ATMsWin
{
    public partial class Form14b_POS : Form
    {
        // Meta exception creation 

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDM_BULK_IST_AndOthers_Records_ALL_2 Bist = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        //DateTime NullPastDate = new DateTime(1950, 11, 21);

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public bool Confirmed;

        int WPartnerSeqNo;  

        public int MetaNumber;

        //decimal NewCcyRate ;
        //decimal NewTxtAmt ;

        decimal CcyRate; 

        string WSignedId;
        string WOperator;
        string WRMCategoryId;
        int WRMCycleId;
        int WMaskRecordId;
        string WAtmNo;
        int WSesNo;// ATM or RM 
        int WUniqueRecordId;
        string WAccNo; 
        decimal WAmount;
        string WCurDes;
        int WMetaExceptionId;
        string WActionId;
        string WUniqueRecordIdOrigin; 

        public Form14b_POS(string InSignedId, string InOperator,
                           string InUniqueRecordIdOrigin, int InUniqueRecordId, string InActionId, 
                           string InMaker_ReasonOfAction)

        {
            WSignedId = InSignedId;
            WOperator = InOperator;
           
            WUniqueRecordId = InUniqueRecordId;
            WUniqueRecordIdOrigin = InUniqueRecordIdOrigin;

            // WMetaExceptionId = InMetaExceptionId;

            MetaNumber = 0; 

            WActionId = InActionId; // 01 = Default, 02 = Manual 

            
            InitializeComponent();

            textBoxMsgBoard.Text = "MetaException will be created which will turn to transaction following authorisation.";

            // Call Procedures 

            // Find needed information
            string WSelectionCriteria = " WHERE UniqueRecordId = " + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria,1);

            WRMCategoryId = Mpa.RMCateg;
            WRMCycleId = Mpa.MatchingAtRMCycle;
            WMaskRecordId = Mpa.UniqueRecordId;
            WAccNo = Mpa.AccNumber;
            WAtmNo = Mpa.TerminalId;
            WSesNo = Mpa.ReplCycleNo;
            WAmount = Mpa.TransAmount;
            WCurDes = Mpa.TransCurr;

            //labelWhatGrid.Text = "ADJUSTMENT - 20000 - POS MANAGEMENT ";

            //WSelectionCriteria = "WHERE SeqNo=" + WMpaSeqNo;
            //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria, 1);

            // FILL Primary INFORMATION
            if (Mpa.ResponseCode == "200000")
            {
              labelPrimary.Text = "MAIN 200000 RECORD - CREDIT"; 
            }
            else
            {
                labelPrimary.Text = "MASTER CARD RECORD - POS";
            }
     
            textBoxCcy.Text = Mpa.TransCurr;
            textBoxTransAmt.Text = Mpa.TransAmount.ToString("#,##0.00");
            textBox_ACCEPTOR_ID.Text = Mpa.ACCEPTOR_ID;
            textBox_ACCEPTORNAME.Text = Mpa.ACCEPTORNAME;
            textBoxTransDate.Text = Mpa.TransDate.ToString();
            textBoxMaskCard1.Text = Mpa.CardNumber;
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
            //
            // Secondary 
            //
                      
            WSelectionCriteria = " WHERE SeqNo=" + WPartnerSeqNo;
            string TableName = "Switch_IST_Txns";
            Mgt.ReadTransSpecificFromBothTables_By_SelectionCriteria(WSelectionCriteria, TableName);

            if (Mpa.ResponseCode == "200000")
            {
                labelSecondary.Text = "PARTNER RECORD";

                textBoxCcy2.Text = Mgt.TransCurr;
                textBoxTransAmt2.Text = Mgt.TransAmt.ToString("#,##0.00");
                textBoxCH_Amount.Text = Mpa.DepCount.ToString("#,##0.00");

                CcyRate = Mpa.DepCount / Mgt.TransAmt;// local divided by fx
                textBoxCcyRate.Text = CcyRate.ToString("#,##0.0000");
                WAccNo = Mgt.AccNo;
                textBox_ACCEPTOR_ID2.Text = Mgt.ACCEPTOR_ID;
                textBox_ACCEPTORNAME2.Text = Mgt.ACCEPTORNAME;
                textBoxTransDate2.Text = Mgt.TransDate.ToString();
                textBoxMaskCard2.Text = Mgt.CardNumber;
                textBoxCard_Encrypted2.Text = Mgt.Card_Encrypted;
                textBoxSecAccNo.Text = WAccNo;
            }
            else
            {
                if (Mpa.TransType == 11 & Mpa.DepCount != 0)
                {
                    // this the case where we have different amount
                   labelSecondary.Text = "AUTHORISATION BY BANK RECORD - WITH DIFFERENT AMOUNT";

                    textBoxCcy2.Text = Mgt.TransCurr;
                    textBoxTransAmt2.Text = Mgt.TransAmt.ToString("#,##0.00");
                    textBoxCH_Amount.Text = Mpa.DepCount.ToString("#,##0.00");

                    CcyRate = Mpa.DepCount / Mgt.TransAmt;// local divided by fx
                    textBoxCcyRate.Text = CcyRate.ToString("#,##0.0000");
                    WAccNo = Mgt.AccNo;
                    textBox_ACCEPTOR_ID2.Text = Mgt.ACCEPTOR_ID;
                    textBox_ACCEPTORNAME2.Text = Mgt.ACCEPTORNAME;
                    textBoxTransDate2.Text = Mgt.TransDate.ToString();
                    textBoxMaskCard2.Text = Mgt.CardNumber;
                    textBoxCard_Encrypted2.Text = Mgt.Card_Encrypted;
                    textBoxSecAccNo.Text = WAccNo; 

                }
                else
                {
                   labelSecondary.Text = "RECORD NOT FOUND BUT FOUND ACCNO.";

                    textBoxCcy2.Text = "N/A";
                    textBoxTransAmt2.Text = "N/A";
                    textBoxCH_Amount.Text = "N/A";

                    // read the first record of todays to find the needed information
                    //
                    Bist.Read_SOURCE_Table_And_GET_FX_AND_LOCAL_Amts(Mpa.LoadedAtRMCycle, Mpa.TransCurr); 
        
                    CcyRate = Bist.LocalAmt / Bist.FxAmt;// local divided by fx

                    textBoxCcyRate.Text = CcyRate.ToString("#,##0.0000");

                    WAccNo = Mpa.AccNumber;
                    textBoxMaskCard2.Text = Mgt.CardNumber;
                    textBoxCard_Encrypted2.Text = Mgt.Card_Encrypted;
                    textBoxSecAccNo.Text = WAccNo;
                    textBox_ACCEPTOR_ID2.Text = "N/A";
                    textBox_ACCEPTORNAME2.Text = "N/A";
                    textBoxTransDate2.Text = "N/A";
                   
                }
                
            }

            if (Mpa.ResponseCode == "200000")
            {
                textBoxFxCcy.Text = Mpa.TransCurr; 
                textBoxFxAmt.Text = Mpa.TransAmount.ToString("#,##0.00");
                WAmount = Mpa.TransAmount * CcyRate;
                //WMetaExceptionId = 165;

            }
            else
            {
                textBoxFxCcy.Text = Mpa.TransCurr;

                if (Mpa.TransType == 11 & Mpa.DepCount != 0)
                {
                    // Master POS MINUS Authorised
                    decimal Diff = Mpa.TransAmount - Mgt.TransAmt;
                    if (Diff > 0)
                    {
                        // Debit Customer 
                        textBoxFxAmt.Text = Diff.ToString("#,##0.00");

                        WAmount = Diff * CcyRate;
                        //WMetaExceptionId = 175;
                    }
                    else
                    {
                        // Credit Customer
                        textBoxFxAmt.Text = (-Diff).ToString("#,##0.00");
                        WAmount = -Diff * CcyRate;
                        //WMetaExceptionId = 165;
                    }
                }
                else
                {
                    // Debit customer 
                    //
                        textBoxFxAmt.Text = Mpa.TransAmount.ToString("#,##0.00");

                        WAmount = Mpa.TransAmount * CcyRate;
                        //WMetaExceptionId = 175;    
                }         
            }

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WMaker_ReasonOfAction = InMaker_ReasonOfAction;
            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                  WActionId, WUniqueRecordIdOrigin,
                                                  WUniqueRecordId, Mpa.TransCurr, Mpa.TransAmount,
                                                  Mpa.TerminalId, Mpa.ReplCycleNo, WMaker_ReasonOfAction, "Reconciliation");

            dataGridActionTXNs.DataSource = Aoc.TxnsTableFromAction.DefaultView;

            ShowGrid_1();

            textBoxMsgBoard.Text = "Review and Confirm";

            return; 

            Er.ReadErrorsIDRecord(WMetaExceptionId, WOperator);
            if (Er.RecordFound == true & Er.NeedAction == false)
            {
               // This logic has been moved to calling
            }

            RRDMBanks Ba = new RRDMBanks();

            Ba.ReadBank(WOperator);

            WCurDes = Ba.BasicCurName; 

            textBoxLocalCcy.Text = WCurDes;
            textBoxLocalAmt.Text = WAmount.ToString("#,##0.00");
            if (Mpa.ResponseCode=="200000")
            {
                // Credit through POS
                textBoxErrDesc.Text = Er.ErrDesc = "Credit due to POS TXN dated:"+ Mpa.TransDate.ToShortDateString();
            }
            else
            {
                if (Mpa.TransType == 11 & Mpa.DepCount != 0)
                {
                    // Different in amount
                    textBoxErrDesc.Text = Er.ErrDesc = "Correction due to POS TXN dated:" + Mpa.TransDate.ToShortDateString();
                }
                else
                {
                    textBoxErrDesc.Text = Er.ErrDesc = "Debit due to Merchant:"+ Mpa.ACCEPTOR_ID + "..TXN dated:" + Mpa.TransDate.ToShortDateString();
                }
            }
                    
            
            
            textBoxCustAccNo.Text = WAccNo = Mgt.AccNo;
            textBox1.Text = WMetaExceptionId.ToString();

            // Double Entry
            if (Er.DrCust == true )
            {
                radioButtonDrCust.Checked = true;
                radioButtonDrCust.Font = new Font(radioButtonDrCust.Font, FontStyle.Bold);
            }
            if (Er.CrAtmCash == true)
            {
                radioButtonCrAtmCash2.Checked = true;
                radioButtonCrAtmCash2.Font = new Font(radioButtonCrAtmCash2.Font, FontStyle.Bold);
            }
            // Double Entry
            if (Er.CrCust == true)
            {
                radioButtonCrCust.Checked = true;
                radioButtonCrCust.Font = new Font(radioButtonCrCust.Font, FontStyle.Bold);
            }
            if (Er.DrAtmCash == true)
            {
                radioButtonDrAtmCash2.Checked = true;
                radioButtonDrAtmCash2.Font = new Font(radioButtonDrAtmCash2.Font, FontStyle.Bold);
            }

            if (WMetaExceptionId == 185) // Double At Host 
            {
                radioButtonCrCust.Checked = true;
                radioButtonCrCust.Font = new Font(radioButtonCrCust.Font, FontStyle.Bold);
                radioButtonDrAtmCash2.Checked = true;
                radioButtonDrAtmCash2.Font = new Font(radioButtonDrAtmCash2.Font, FontStyle.Bold);
            }

            if (WMetaExceptionId == 165) // Wrong Updating At Host - CREDIT Customer
            {
                radioButtonCrCust.Checked = true;
                radioButtonCrCust.Font = new Font(radioButtonCrCust.Font, FontStyle.Bold);
                radioButtonDrAtmCash2.Checked = true;
                radioButtonDrAtmCash2.Font = new Font(radioButtonDrAtmCash2.Font, FontStyle.Bold);
            }

            if (WMetaExceptionId == 175) // Wrong Updating At Host - DEBIT Customer
            {
                radioButtonDrCust.Checked = true;
                radioButtonDrCust.Font = new Font(radioButtonCrCust.Font, FontStyle.Bold);
                radioButtonCrAtmCash2.Checked = true;
                radioButtonCrAtmCash2.Font = new Font(radioButtonDrAtmCash2.Font, FontStyle.Bold);
            }

            if (WMetaExceptionId == 521) // Credit Suspense
            {
                radioButtonCrAtmSusp.Checked = true;
                radioButtonCrAtmSusp.Font = new Font(radioButtonCrAtmSusp.Font, FontStyle.Bold);
                radioButtonDrAtmCash2.Checked = true;
                radioButtonDrAtmCash2.Font = new Font(radioButtonDrAtmCash2.Font, FontStyle.Bold);
            }
            if (WMetaExceptionId == 526) // Debit Suspense
            {
                radioButtonDrAtmSusp.Checked = true;
                radioButtonDrAtmSusp.Font = new Font(radioButtonDrAtmSusp.Font, FontStyle.Bold);
                radioButtonCrAtmCash2.Checked = true;
                radioButtonCrAtmCash2.Font = new Font(radioButtonCrAtmCash2.Font, FontStyle.Bold);
            }

            if (WMetaExceptionId == 145) // Manual 
            {
                panel3.Enabled = true;
                panel4.Enabled = true;
                textBoxLocalAmt.ReadOnly = false; 
            }
            else
            {
                panel3.Enabled = false;
                panel4.Enabled = false;
                textBoxLocalAmt.ReadOnly = true;
            }

            

        }
        // Create  Error record
        bool CrEntry1;
        bool CrEntry2;
        int I; 
        private void button1_Click(object sender, EventArgs e)
        {

            Ac.ReadAtm(WAtmNo);
            if (Ac.RecordFound == true)
            {
                Er.BankId = Ac.BankId;
                Er.BranchId = Ac.Branch;
                Er.CitId = Ac.CitId;
            }
            else
            {
                // Read Category 
                Er.BankId = WOperator;
                Er.BranchId = "BDC HeadQuarters";
                Er.CitId = "1000";
            }

            // INITIALISED WHAT IS NEEDED 
            
            Er.CategoryId = WRMCategoryId;
            Er.RMCycle = WRMCycleId;
            Er.UniqueRecordId = WMaskRecordId;

            Er.AtmNo = WAtmNo;
            Er.SesNo = WSesNo;
            Er.DateInserted = DateTime.Now;

            //string tt = Er.ErrDesc; 
         
            Er.UniqueRecordId = WUniqueRecordId;

            Er.ByWhom = WSignedId;

            //  Pa.CurrCd = WCurrCd;
            Er.CurDes = WCurDes;
            Er.ErrAmount = WAmount;

            Er.TraceNo = Mpa.TraceNoWithNoEndZero;
            Er.CardNo = Mpa.CardNumber;
            Er.CustAccNo = WAccNo;
            Er.TransType = Mpa.TransType;
            //******************
            Er.TransDescr = Mpa.TransDescr;
            //******************
            Er.DateTime = Mpa.TransDate;

            Er.DatePrinted = NullPastDate;

            Er.OpenErr = true;

            Er.UnderAction = true;

            Er.Operator = WOperator;

            if (WActionId == "02")
            {
                Er.ManualAct = true; 
                CrEntry1 = false;
                CrEntry2 = false;

                I = 0; 

                // CREATE FIRST ENTRY 
                if (radioButtonDrCust.Checked == true)
                {
                    Er.DrCust = true;
                    CrEntry1 = false;
                    I++;
                }
                if (radioButtonCrCust.Checked == true)
                {
                    Er.CrCust = true;
                    CrEntry1 = true;
                    I++;
                }

                Er.CustAccNo = textBoxCustAccNo.Text;

                if (radioButtonDrAtmCash.Checked == true)
                {
                    Er.DrAtmCash = true;
                    CrEntry1 = false;
                    I++;
                }
                if (radioButtonCrAtmCash.Checked == true)
                {
                    Er.CrAtmCash = true;
                    CrEntry1 = true;
                    I++;
                }

                if (radioButtonDrAtmSusp.Checked == true)
                {
                    Er.DrAtmSusp = true;
                    CrEntry1 = false;
                    I++;
                }
                if (radioButtonCrAtmSusp.Checked == true)
                {
                    Er.CrAtmSusp = true;
                    CrEntry1 = true;
                    I++;
                }

                // SECOND ENTRY 

                CrEntry2 = false; 

                if (radioButtonDrCust2.Checked == true)
                {
                    Er.DrCust = true;
                    CrEntry2 = false;
                    I++;
                }
                if (radioButtonCrCust2.Checked == true)
                {
                    Er.CrCust = true;
                    CrEntry2 = true;
                    I++;
                }

                if (radioButtonDrAtmCash2.Checked == true)
                {
                    Er.DrAtmCash = true;
                    CrEntry2 = false;
                    I++;
                }
                if (radioButtonCrAtmCash2.Checked == true)
                {
                    Er.CrAtmCash = true;
                    CrEntry2 = true;
                    I++;
                }

                if (radioButtonDrAtmSusp2.Checked == true)
                {
                    Er.DrAtmSusp = true;
                    CrEntry2 = false;
                    I++;
                }
                if (radioButtonCrAtmSusp2.Checked == true)
                {
                    Er.CrAtmSusp = true;
                    CrEntry2 = true;
                    I++;
                }
                if (I !=2)
                {
                    MessageBox.Show("Please correct entries." + Environment.NewLine
                        + " Both First and Second Entry Needed.");

                    return;
                }

                if (CrEntry1 == true & CrEntry2 == true)
                {
                    MessageBox.Show("Please correct entries." + Environment.NewLine
                                    + "Entries cannot have same sign.");
                    return; 
                }
                if (CrEntry1 == false & CrEntry2 == false)
                {
                    MessageBox.Show("Please correct entries." + Environment.NewLine
                                   + "Entries cannot have same sign.");
                    return;
                }

            }

            MetaNumber = Er.InsertError(); // INSERT ERROR 

            MessageBox.Show("Meta Exception with no = " + Er.ErrNo + " is created. ");

            //textBoxMsgBoard.Text = "Meta Exception is created. Close form.";

            this.Close();
        }
// 
        private void ShowGrid_1()
        {
            dataGridActionTXNs.Columns[0].Width = 60; // SeqNumber
            dataGridActionTXNs.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[1].Width = 60; // ActionId
            dataGridActionTXNs.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridActionTXNs.Columns[2].Width = 300; // ActionNm
            dataGridActionTXNs.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[3].Width = 60; // Branch
            dataGridActionTXNs.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[4].Width = 120; // AccNo
            dataGridActionTXNs.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[5].Width = 50; // DR/CR
            dataGridActionTXNs.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridActionTXNs.Columns[6].Width = 120; // AccName
            dataGridActionTXNs.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[7].Width = 90; // Amount
            dataGridActionTXNs.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridActionTXNs.Columns[8].Width = 350; // Description
            dataGridActionTXNs.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridActionTXNs.Columns[9].Width = 60; // Stage 
            dataGridActionTXNs.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        // FINISH 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // 
        private void radioButtonDrCust_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDrCust.Checked == true )
            {

                radioButtonCrCust2.Enabled = false;
                radioButtonDrCust2.Enabled = false;
            }
            else
            {
                radioButtonCrCust2.Enabled = true;
                radioButtonDrCust2.Enabled = true;
            }
        }

        private void radioButtonCrCust_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCrCust.Checked == true)
            {
                radioButtonCrCust2.Enabled = false;
                radioButtonDrCust2.Enabled = false;
            }
            else
            {
                radioButtonCrCust2.Enabled = true;
                radioButtonDrCust2.Enabled = true;
            }
        }
        // Confirm 
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
    }
}

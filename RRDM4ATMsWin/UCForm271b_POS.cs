using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Text;
using RRDM4ATMs;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class UCForm271b_POS : UserControl
    {
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        //RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions();

        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        RRDMNVCardsBothAuthorAndSettlement Sec = new RRDMNVCardsBothAuthorAndSettlement();

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMNVDisputes Di = new RRDMNVDisputes();

        RRDMNVDisputesTrans Dt = new RRDMNVDisputesTrans();

        RRDMNVCurrentCcyRates Cr = new RRDMNVCurrentCcyRates();

        RRDMMatchingTxns_InGeneralTables_BDC Mg = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WModeN;

        bool ViewWorkFlow;

        int WSeqNo1;
        int WSeqNo2;

        int WMode_L;

        int WMode_R;

        int WRowGrid2;

        //string WMatchedType;

        //string WUserBankId;

        public string guidanceMsg;

        //string Ccy;
        //decimal CcyRate;
        //string InternalAcc;
        //string ExternalAcc;

        string TableA = "[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]";  // EXTERNAL (Master POS)

        //string TableB = "[RRDM_Reconciliation_ITMX].[dbo].[Flexcube]";  // INTERNAL (Iflex)
        string TableB = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]";  // temporarily set it instead of Flexube 
        string Table_IST = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]";  // IST
        
        //
        //string W4DigitMainCateg;

        string SelectionCriteria;

        //int WSeqNo3;

        int WRowGrid1;

        decimal GTotal2;

        string WReference;

        string WOrderCriteria;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCategoryId;
        int WReconcCycleNo;


        public void UCForm271bPar_POS(string InSignedId, int SignRecordNo, string InOperator, string InRMCategoryId, int InRMCycle, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WCategoryId = InRMCategoryId;
            WReconcCycleNo = InRMCycle;

            InitializeComponent();

            // DELETE ALL ADJUSTMENTS WITH ACTION TYPE = 3 

            // For EXTERNAL
            //Mg.UpdateActionTypeByALL(TableA, WCategoryId);
            // For INTERNAL
            // Mg.UpdateActionTypeByALL(TableB, WCategoryId);

            // Set working mode to 1

            WMode_L = 1;

            WMode_R = 3;

        }

        // SHOW SCREEN 

        public void SetScreen()
        {
            // 
            // WMode_L
            Mg.ReadMergedTXNWithOthers(TableA, TableB, WCategoryId, WMode_L);

            ShowGrid1();

            textBoxTotalEXT.Text = Mg.TotalEXT.ToString();
            textBoxTotalINT.Text = Mg.TotalINT.ToString();

            // WMode_R
            Mg.ReadMergedTXNWithOthers(TableA, TableB, WCategoryId, WMode_R);

            ShowGrid2WithItsDetails(); // Show all with actiontype = 3 

            textBoxTotalEXT2.Text = Mg.TotalEXT.ToString();
            textBoxTotalINT2.Text = Mg.TotalINT.ToString();

            textBoxEXT_Amt.Text = Mg.TotalEXT_Amt.ToString();
            textBoxINT_Amt.Text = Mg.TotalINT_Amt.ToString();
            textBoxAdj.Text = Mg.TotalAdj_Amt.ToString();
            GTotal2 = Mg.TotalEXT_Amt - (Mg.TotalINT_Amt + Mg.TotalAdj_Amt);
            textBoxDiff_Amt.Text = GTotal2.ToString();


        }

        string WOrigin1;
        string WOrigin2;
        decimal LAmt;
        int WDispNo;

        bool LeftExternal_Bool;
        bool LeftInternal_Bool;
        bool LeftAdjustmt_Bool;

        string WActionTypeL;
        int WUniqueRecordIdL;
        decimal WTransAmtL;
        string WFullTraceNoL; // Contains Merchant information 
        string WCard_EncryptedL;
        DateTime WTransDateL;

        // Row enter for first dataGridview
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo1 = (int)rowSelected.Cells[0].Value;
            WOrigin1 = (string)rowSelected.Cells[1].Value;

            if (WOrigin1 == "EXTERNAL")
            {
                LeftExternal_Bool = true;
            }
            else
            {
                LeftExternal_Bool = false;
            }
            if (WOrigin1 == "INTERNAL")
            {
                LeftInternal_Bool = true;
            }
            else
            {
                LeftInternal_Bool = false;
            }

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo1;

            Mg.ReadTable_BySelectionCriteria(TableA,"" ,SelectionCriteria,2);
            //
            // KEEP THE VALUES IN WORKING FIELDS
            //
            WActionTypeL = Mg.ActionType;
            WUniqueRecordIdL = Mg.UniqueRecordId;
            WTransAmtL = Mg.TransAmt;
            WFullTraceNoL = Mg.FullTraceNo;
            WCard_EncryptedL = Mg.Card_Encrypted;
            WTransDateL = Mg.TransDate;
            //WDispNo = 0;

            //SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";
            //Sec.ReadNVFromBothVisaAutherAndSettlementBySelectionCriteria(SelectionCriteria);

            //LAmt = Sec.TxnAmt;

            //int WColorNo = (int)rowSelected.Cells[1].Value;

            //if (WColorNo == 11)
            //{
            //    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DimGray;
            //}
            //else if (WColorNo == 12)
            //{
            //    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            //}

            //if (Sec.IsException == true)
            //{
            //    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkOrange;
            //    AlertDetails(); // Show Alert Details 

            //}
            //else
            //{
            //    textBoxAlert.Hide();
            //    AlertDetailsIsShown = false;
            //    if (AlertFound == true) labelAlertFound.Show();
            //    linkLabelDispute.Hide();
            //    labelSettledDisp.Hide();
            //}

            //textBox12.Text = Sec.RRN;
            //textBox14.Text = "Panicos";
        }
        // Row enter for second datagridview
        bool RightExternal_Bool;
        bool RightInternal_Bool;
        bool RightAdjustmt_Bool;
        string WActionTypeR;
        int WUniqueRecordIdR;
        decimal WTransAmtR;
        string WFullTraceNoR; // Contains Merchant information 
        string WCard_EncryptedR;
        DateTime WTransDateR;
        string WResponseCodeR;


        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNo2 = (int)rowSelected.Cells[0].Value;

            WOrigin2 = (string)rowSelected.Cells[1].Value;

            if (WOrigin2 == "EXTERNAL")
            {
                RightExternal_Bool = true;
                WTable = TableA;
            }
            else
            {
                RightExternal_Bool = false;
            }
            //
            if (WOrigin2 == "INTERNAL")
            {
                RightInternal_Bool = true;
                WTable = TableB;
            }
            else
            {
                RightInternal_Bool = false;
            }
            //
            if (WOrigin2 == "WAdjustment")
            {
                RightAdjustmt_Bool = true;
                WTable = TableB; // Adjustments are made only on internals
            }
            else
            {
                RightAdjustmt_Bool = false;
            }

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo2;

            Mg.ReadTable_BySelectionCriteria(WTable, "", SelectionCriteria,2);

            //
            // KEEP THE VALUES IN WORKING FIELDS
            //
            WActionTypeR = Mg.ActionType;
            WUniqueRecordIdR = Mg.UniqueRecordId;
            WTransAmtR = Mg.TransAmt;
            WFullTraceNoR = Mg.FullTraceNo;
            WCard_EncryptedR = Mg.Card_Encrypted;
            WTransDateR = Mg.TransDate;
            WResponseCodeR = Mg.ResponseCode;

            // NOTES 2 START  
            if (WUniqueRecordIdR != 0)
            {
                Order = "Descending";
                WParameter4 = "UniqueRecordId: " + WUniqueRecordIdR;
                WSearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
                if (Cn.RecordFound == true)
                {
                    labelNumberNotes3.Text = Cn.TotalNotes.ToString();
                }
                else labelNumberNotes3.Text = "0";
            }
            else
            {
                labelNumberNotes3.Text = "0";
            }

            // NOTES 2 END

            //label3.Text = "SELECTED TRANSACTIONS";

            if (WOrigin2 == "WAdjustment")
            {
                panel5.Show();
                textBoxAmt.Text = Mg.TransAmt.ToString();
                textBoxDetails.Text = Mg.TransDescr;
                //comboBoxGl.Text = Sec.AdjGlAccount;
                //   comboBoxReason.Text = Sec.MatchedType;
                textBoxAccNo.Text = Mg.AccNo;
                Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);
                textBoxGL_1.Text = Mc.GlAccount; 
            }
            else
            {
                panel5.Hide();
                textBoxAmt.Text = "";
                textBoxDetails.Text = "";
                textBox10.Text = "";
                checkBox5.Checked = false;
            }
        }
        // Move to right

        // Add 
        string WTable;
        string WActionType;
        private void buttonMoveToRight_Click(object sender, EventArgs e)
        {
            // Check if old with such reference 
            //WMode_L = 15;
            //  Sec.ReadNVCardRecordsForBothByMode(WOperator, WSignedId, WSubSystem, WMode, WReconcCycleNo, ExternalAcc, InternalAcc, WOrderCriteria, WWorkingDate, WReference);
            if (Sec.RecordFound == true)
            {
                if (MessageBox.Show("There are old matched with this reference. " + Environment.NewLine
                    + "This entry might be double at External "
                    + "Do want to visit them?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                {
                    Form291NVMatched NForm291NVMatched;
                    int Mode = 2;

                    //  NForm291NVMatched = new Form291NVMatched(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo, Mode, WReference);

                    //     NForm291NVMatched.ShowDialog();

                }
                else
                {
                    // Proceed
                }
            }

            WRowGrid1 = dataGridView1.SelectedRows[0].Index;

            if (LeftExternal_Bool == true)
            {
                WTable = TableA;
            }
            if (LeftInternal_Bool == true)
            {
                WTable = TableB;
            }

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo1;

            Mg.ReadTable_BySelectionCriteria(WTable, "", SelectionCriteria,2);



            // Find all entries with the same reference and update Se.ActionType = 3 
            // NO SELECTION 
            if (checkBoxSameRef.Checked == false & checkBoxSameAmt.Checked == false & checkBoxCard.Checked == false)
            {
                WActionType = "3"; // 

                Mg.UpdateActionTypeBySeqNo(WTable, WSeqNo1, WActionType);


                //SelectionCriteria = " WHERE SeqNo =" + WSeqNo1 + " AND Origin ='" + WOrigin1 + "'";

                //if (WOrigin1 == "EXTERNAL")
                //{
                //   Sec.UpdateActionTypeExternal(WOperator, SelectionCriteria,Sec.TxnValueDate , WActionType, WWorkingDate);
                //}
                //else
                //{
                //   // Sec.UpdateActionTypeInternal(WOperator, SelectionCriteria, Sec.TxnValueDate, WActionType, WWorkingDate);
                //}

                WRowGrid1 = dataGridView1.SelectedRows[0].Index;

                WReference = "";
                WOrderCriteria = " Origin ASC ";
                SetScreen(); //   

                return;
            }

            // REFERENCE 
            if (checkBoxSameRef.Checked == true &
                checkBoxSameAmt.Checked == false & checkBoxCard.Checked == false)
            {
                //" SELECT * FROM [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
                //             + " WHERE Mask<>'111' AND Mask<>''  AND MatchingCateg = @MatchingCateg AND ActionType='3' "
                //             + " UNION ALL"
                //             + " SELECT * FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                //             + " WHERE MatchingCateg = @MatchingCateg and Processed = 0 AND ActionType = '3' "
                //             + " ORDER BY OriginFileName, TransDate ASC";
                if (Mg.RRNumber == "")
                {
                    MessageBox.Show("RRNumber is zero ");
                    return;
                }
                if (WMode_L == 1)
                {
                    // Update External
                    WActionType = "3";
                    SelectionCriteria = " WHERE Mask<>'111' AND Mask<>''  AND MatchingCateg ='" + WCategoryId + "'" + " AND ActionType='0'"
                                       + "  AND RRNumber = '" + Mg.RRNumber + "'";
                    Mg.UpdateActionTypeBySelectionCriteria(TableA, SelectionCriteria, WActionType);

                    // Update Internal
                    WActionType = "3";
                    SelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId + "'" + " AND Processed = 0"
                                       + "  AND RRNumber = '" + Mg.RRNumber + "'";
                    Mg.UpdateActionTypeBySelectionCriteria(TableB, SelectionCriteria, WActionType);
                }
                else
                {
                    // 
                    // Update External
                    WActionType = "17";
                    SelectionCriteria = " WHERE UniqueRecordId =" + Mg.UniqueRecordId;

                    Mg.UpdateActionTypeBySelectionCriteria(TableA, SelectionCriteria, WActionType);
                    // Update Internal
                    WActionType = "17";
                    SelectionCriteria = " WHERE UniqueRecordId =" + Mg.UniqueRecordId;

                    Mg.UpdateActionTypeBySelectionCriteria(TableB, SelectionCriteria, WActionType);
                }



            }


            // AMOUNT 
            if (checkBoxSameAmt.Checked == true & checkBoxSameRef.Checked == false
                & checkBoxCard.Checked == false)
            {
                // Update External
                WActionType = "3";
                SelectionCriteria = " WHERE Mask<>'111' AND Mask<>''  AND MatchingCateg ='" + WCategoryId + "'" + " AND ActionType='0'"
                                   + "  AND TransAmt = " + Mg.TransAmt;
                Mg.UpdateActionTypeBySelectionCriteria(TableA, SelectionCriteria, WActionType);

                // Update Internal
                WActionType = "3";
                SelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId + "'" + " AND Processed = 0"
                                    + "  AND TransAmt = " + Mg.TransAmt;
                Mg.UpdateActionTypeBySelectionCriteria(TableB, SelectionCriteria, WActionType);

            }
            // CARD
            if (checkBoxCard.Checked == true & checkBoxSameRef.Checked == false
                & checkBoxSameAmt.Checked == false)
            {
                // Update External
                WActionType = "3";
                SelectionCriteria = " WHERE Mask<>'111' AND Mask<>''  AND MatchingCateg ='" + WCategoryId + "'" + " AND ActionType='0'"
                                   + "  AND Card_Encrypted = '" + Mg.Card_Encrypted + "'";
                Mg.UpdateActionTypeBySelectionCriteria(TableA, SelectionCriteria, WActionType);

                // Update Internal
                WActionType = "3";
                SelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId + "'" + " AND Processed = 0"
                                    + "  AND Card_Encrypted = '" + Mg.Card_Encrypted + "'";
                Mg.UpdateActionTypeBySelectionCriteria(TableB, SelectionCriteria, WActionType);

            }

            // REFERENCE With Amt 
            if (checkBoxSameRef.Checked == true & checkBoxSameAmt.Checked == true
                & checkBoxCard.Checked == false)
            {
                // Update External
                WActionType = "3";
                SelectionCriteria = " WHERE Mask<>'111' AND Mask<>''  AND MatchingCateg ='" + WCategoryId + "'" + " AND ActionType='0'"
                                   + "  AND RRNumber = '" + Mg.RRNumber + "'" + "  AND TransAmt = " + Mg.TransAmt;

                Mg.UpdateActionTypeBySelectionCriteria(TableA, SelectionCriteria, WActionType);

                // Update Internal
                WActionType = "3";
                SelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId + "'" + " AND Processed = 0"
                                     + "  AND RRNumber = '" + Mg.RRNumber + "'" + "  AND TransAmt = " + Mg.TransAmt;
                Mg.UpdateActionTypeBySelectionCriteria(TableB, SelectionCriteria, WActionType);
            }


            if (SelectionCriteria == "" &
                (checkBoxSameRef.Checked == true || checkBoxSameAmt.Checked == true))
            {
                MessageBox.Show("Your selection doesnt make sense");
                return;
            }

            // Sec.UpdateActionTypeExternal(WOperator, SelectionCriteria, Sec.TxnValueDate, WActionType, WWorkingDate);

            //
            //SEARCH IN INTERNAL AND UPDATE WITH 3 if found 
            //


            //  Sec.UpdateActionTypeInternal(WOperator, SelectionCriteria, Sec.TxnValueDate, WActionType, WWorkingDate);

            WRowGrid1 = dataGridView1.SelectedRows[0].Index;
            // WMode_L = 1;
            WReference = "";
            WOrderCriteria = " Origin ASC ";

            SetScreen();

        }
        // Move to left
        private void buttonMoveToLeft_Click(object sender, EventArgs e)
        {
            if (RightExternal_Bool == true)
            {
                WTable = TableA;
            }
            if (RightInternal_Bool == true)
            {
                WTable = TableB;
            }

            if (WOrigin2 == "WAdjustment" & WActionType == "3")
            {
                MessageBox.Show("You cannot move adjustment. You can delete it!");
            }

            if (WActionTypeR == "3") WActionTypeL = "0"; // 

            if (WActionTypeR == "17") WActionTypeL = "12"; // This is manual Matched 

            if (WOrigin2 == "EXTERNAL")
            {
                Mg.UpdateActionTypeBySeqNo(WTable, WSeqNo2, WActionTypeL);
            }

            if (WOrigin2 == "INTERNAL" || WOrigin2 == "WAdjustment")
            {
                Mg.UpdateActionTypeBySeqNo(WTable, WSeqNo2, WActionTypeL);
            }



            SetScreen();
        }

        string WOurReference1;
        bool AlertFound;
        public void ShowGrid1()
        {

            dataGridView1.DataSource = Mg.TablePOS_Settlement.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {

                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";


            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 95; // "Origin"
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 60; // "MASK"
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = true;
            
            dataGridView1.Columns[3].Width = 70; // "Card Encypted"
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 90; // Amt
            dataGridView1.Columns[4].DefaultCellStyle = style;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 90;  // "Auth Number"
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[6].Width = 160;  // DateTime 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = true;
            
            dataGridView1.Columns[7].Width = 60; //"RESP CODE"
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].Visible = true;

            dataGridView1.Columns[8].Width = 70;  // "MerchantId"
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 70; // "DR/CR"
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[10].Width = 40; // DR/CR
            //dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[11].Width = 115; // Amt
            //dataGridView1.Columns[11].DefaultCellStyle = style;
            //dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            //dataGridView1.Columns[12].Width = 50; // Ccy
            //dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[12].Visible = true;

            //dataGridView1.Columns[13].Width = 130; // RRN
            //dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[13].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[13].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[13].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            //dataGridView1.Columns[14].Width = 100; // Other Details
            //dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[15].Width = 95; // Terminal Id 
            //dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[16].Width = 95; // CcyRate
            //dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[16].Visible = true;

            //dataGridView1.Columns[17].Width = 95; // GLAccount
            //dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[17].Visible = true;

            AlertFound = false;

            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    int TempSeqNo = (int)row.Cells[0].Value;
            //    string TempOrigin = row.Cells[5].Value.ToString();

            //    if (TempOrigin == "EXTERNAL")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }
            //    else if (TempOrigin == "INTERNAL")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.White;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }

            //    SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

            //    Sec.ReadNVFromBothVisaAutherAndSettlementBySelectionCriteria(SelectionCriteria);

            //    if (Sec.IsException == true)
            //    {
            //        AlertFound = true;
            //        //dataGridView1.Columns[4].DefaultCellStyle.BackColor = Color.SaddleBrown;
            //        //dataGridView1.Columns[4].DefaultCellStyle.ForeColor = Color.White;
            //        row.DefaultCellStyle.BackColor = Color.SaddleBrown;
            //        row.DefaultCellStyle.ForeColor = Color.White;
            //    }
            //}
            //if (AlertFound == true)
            //{
            //    labelAlertFound.Show();
            //    if (AlertDetailsIsShown == true) labelAlertFound.Hide();
            //}
            //else
            //{
            //    labelAlertFound.Hide();
            //    linkLabelDispute.Hide();
            //    labelSettledDisp.Hide();
            //}

            //TotalExternal1 = 0;
            //TotalInternal1 = 0;
            //TotalAdjust1 = 0;
            //GTotal1 = 0;
            //WOurReference1 = "";

            //int K = 0;
            ////
            //// Totals and Analysis  
            ////
            //while (K <= (dataGridView1.Rows.Count - 1))
            //{
            //    RSeqNo = (int)dataGridView1.Rows[K].Cells["SeqNo"].Value;
            //    ROrigin = (string)dataGridView1.Rows[K].Cells["Origin"].Value;

            //    if (ROrigin == "EXTERNAL")
            //    {
            //        SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + RSeqNo;

            //        Sec.ReadNVCards_External_File_SetllementBySelectionCriteria(SelectionCriteria);

            //        TotalExternal1 = TotalExternal1 + Sec.TxnAmt;
            //    }

            //    if (ROrigin == "INTERNAL")
            //    {
            //        SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

            //        Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(SelectionCriteria);

            //        WOurReference1 = Sec.RRN;

            //        TotalInternal1 = TotalInternal1 + Sec.TxnAmt;
            //    }

            //    if (ROrigin == "WAdjustment")
            //    {
            //        SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

            //        Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(SelectionCriteria);

            //        TotalAdjust1 = TotalAdjust1 + Sec.TxnAmt;
            //    }

            //    K++; // Read Next entry of the table 
            //}
            //// 
            //GTotal1 = TotalExternal1 + TotalInternal1 + TotalAdjust1;
            //textBox13.Text = GTotal1.ToString("#,##0.00");

        }

        string WExternalReference2;
        string WInternalReference2;
        DateTime WExternalDtTm2;
        DateTime WInternalDtTm2;
        int WExternalCount2;
        int WInternalCount2;
        bool WDisputesOn2;

        int WRightRows;
        public void ShowGrid2WithItsDetails()
        {

            dataGridView2.DataSource = Mg.TablePOS_Settlement.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                labelAmt.Hide();
                panel7.Hide();

                label28.Hide();
                panel5.Hide();
                checkBoxAdj.Checked = false;
                checkBoxDisputes.Checked = false;
                comboBoxReason.Hide();
                return;
            }
            else
            {
                labelAmt.Show();
                panel7.Show();
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";


            dataGridView2.Columns[0].Width = 50; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].Visible = false;

            dataGridView2.Columns[1].Width = 95; // "Origin"
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[1].Visible = true;

            dataGridView2.Columns[2].Width = 60; // "MASK"
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[2].Visible = false;

            dataGridView2.Columns[3].Width = 70; // "Card Encypted"
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[3].Visible = true;

            dataGridView2.Columns[4].Width = 90; // Amt
            dataGridView2.Columns[4].DefaultCellStyle = style;
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[5].Width = 90;  // "Auth Number"
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[5].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[6].Width = 160;  // DateTime 
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[6].Visible = true;

            dataGridView2.Columns[7].Width = 60; //"RESP CODE"
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[7].Visible = true;

            dataGridView2.Columns[8].Width = 70;  // "MerchantId"
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[9].Width = 70; // "DR/CR"
            dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;



            //// SET LINE Colors
            //foreach (DataGridViewRow row in dataGridView2.Rows)
            //{
            //    int TempSeqNo = (int)row.Cells[0].Value;
            //    string TempOrigin = row.Cells[5].Value.ToString();

            //    if (TempOrigin == "EXTERNAL")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }
            //    else if (TempOrigin == "INTERNAL")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.White;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }

            //    SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

            //    Sec.ReadNVFromBothVisaAutherAndSettlementBySelectionCriteria(SelectionCriteria);

            //    if (Sec.IsException == true)
            //    {
            //        row.DefaultCellStyle.BackColor = Color.SaddleBrown;
            //        row.DefaultCellStyle.ForeColor = Color.White;
            //    }
            //}
            ////SHOW TOTALS
            //TotalExternal2 = 0;
            //TotalInternal2 = 0;
            //TotalAdjust2 = 0;
            //GTotal2 = 0;
            //WExternalReference2 = "";
            //WInternalReference2 = "";
            //WExternalDtTm2 = NullPastDate;
            //WInternalDtTm2 = NullPastDate;
            //WExternalCount2 = 0;
            //WInternalCount2 = 0;
            //WDisputesOn2 = false; 

            //WRightRows = dataGridView2.Rows.Count;

            //int K = 0;
            ////
            //// Totals and Analysis  
            ////
            //while (K <= (dataGridView2.Rows.Count - 1))
            //{
            //    RSeqNo = (int)dataGridView2.Rows[K].Cells["SeqNo"].Value;
            //    ROrigin = (string)dataGridView2.Rows[K].Cells["Origin"].Value;
            //    RAmt = (decimal)dataGridView2.Rows[K].Cells["Amt"].Value;

            //    if (ROrigin == "EXTERNAL")
            //    {
            //        SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + RSeqNo;

            //        Sec.ReadNVCards_External_File_SetllementBySelectionCriteria(SelectionCriteria);

            //        if (Sec.ExceptionNo > 0)
            //        {
            //            WDispNo = Sec.ExceptionNo;
            //            Di.ReadNVDisputesBySeqNo(WOperator, WDispNo);
            //            if (Di.Active == true)
            //            {
            //                WDisputesOn2 = true;
            //            }
            //        }

            //        WExternalCount2 = WExternalCount2 + 1;

            //        if (WExternalCount2 == 1)
            //        {
            //            // First Record
            //            WExternalDtTm2 = Sec.TxnValueDate;
            //        }
            //        else
            //        {
            //            if (Sec.TxnCode == "11")
            //            {
            //                // Assign the biggest 
            //                if (WExternalDtTm2 < Sec.TxnValueDate)
            //                {
            //                    WExternalDtTm2 = Sec.TxnValueDate;
            //                }
            //            }
            //            if (Sec.TxnCode == "21")
            //            {
            //                // Assign the smallest
            //                if (WExternalDtTm2 > Sec.TxnValueDate)
            //                {
            //                    WExternalDtTm2 = Sec.TxnValueDate;
            //                }
            //            }
            //        }

            //        WExternalReference2 = Sec.RRN;

            //        TotalExternal2 = TotalExternal2 + Sec.TxnAmt;
            //    }

            //    if (ROrigin == "INTERNAL")
            //    {
            //        SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

            //        Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(SelectionCriteria);

            //        if (Sec.ExceptionNo > 0)
            //        {
            //            WDispNo = Sec.ExceptionNo;
            //            Di.ReadNVDisputesBySeqNo(WOperator, WDispNo);
            //            if (Di.Active == true)
            //            {
            //                WDisputesOn2 = true;
            //            }
            //        }

            //        WInternalCount2 = WInternalCount2 + 1;

            //        WInternalDtTm2 = Sec.TxnValueDate;

            //        WInternalReference2 = Sec.RRN;

            //        TotalInternal2 = TotalInternal2 + Sec.TxnAmt;
            //    }

            //    if (ROrigin == "WAdjustment")
            //    {
            //        SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

            //        Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(SelectionCriteria);

            //        TotalAdjust2 = TotalAdjust2 + Sec.TxnAmt;
            //    }

            //    K++; // Read Next entry of the table 
            //}
            //// 
            //textBox2.Text = TotalExternal2.ToString("#,##0.00");
            //textBox4.Text = TotalInternal2.ToString("#,##0.00");
            //textBox7.Text = TotalAdjust2.ToString("#,##0.00");
            //GTotal2 = TotalExternal2 + TotalInternal2 + TotalAdjust2;
            //textBox1.Text = GTotal2.ToString("#,##0.00");

            //if (WExternalCount2 > 0 & WInternalCount2 > 0 & checkBoxAdj.Checked == false)
            //{

            //    //textBoxExtRef.Text = WExternalReference2;
            //    //textBoxIntRef.Text = WInternalReference2;

            //    if (WExternalDtTm2.Date == WInternalDtTm2.Date)
            //    {
            //        //labelValDiff.Text = "Same";
            //        //labelValDiff.ForeColor = Color.Black;
            //        //linkLabelTolerValRight.Hide();
            //    }
            //    else
            //    {
            //        int DiffInEntryDays = Convert.ToInt32((WExternalDtTm2 - WInternalDtTm2).TotalDays);
            //        int AbsDiffInEntryDays = Math.Abs(DiffInEntryDays);

            //        //labelValDiff.Text = "Diff by " + AbsDiffInEntryDays;
            //        //labelValDiff.ForeColor = Color.Red;
            //        //linkLabelTolerValRight.Show();
            //    }
            //    if (WExternalReference2 == WInternalReference2)
            //    {
            //        //labelRefDiff.Text = "Same";
            //        //labelRefDiff.ForeColor = Color.Black;
            //    }
            //    else
            //    {
            //        //labelRefDiff.Text = "Diff";
            //        //labelRefDiff.ForeColor = Color.Red;
            //    }

            //}
            //else
            //{

            //}
            //decimal PictureTotal = GTotal2; 

            //if (PictureTotal < 0) PictureTotal = -PictureTotal;

            //if (PictureTotal > 5)
            //{
            //    pictureBoxBalStatus.BackgroundImage = appResImg.RED_LIGHT_Repl;
            //}

            //if (PictureTotal < 5)
            //{
            //    pictureBoxBalStatus.BackgroundImage = appResImg.YELLOW_Repl;
            //}

            //if (GTotal2 == 0)
            //{
            //    pictureBoxBalStatus.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            //}

            //if (PictureTotal != 0)
            //{
            //    checkBoxAdj.Show();
            //    linkLabelTolerBalRight.Show();

            //    button3.Hide();

            //    label28.Hide();
            //    comboBoxReason.Hide();
            //}
            //else
            //{
            //    checkBoxAdj.Hide();

            //    linkLabelTolerBalRight.Hide();

            //    button3.Show();

            //    label28.Show();
            //    comboBoxReason.Show();
            //    comboBoxReason.Text = "Fill Entry";
            //}
        }

        // Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            // DELETE ALL ADJUSTMENTS WITH ACTION TYPE = 03 

            // For EXTERNAL
            Mg.UpdateActionTypeByALL(TableA, WCategoryId);
            // For INTERNAL
            Mg.UpdateActionTypeByALL(TableB, WCategoryId);
            //  WMode_L = 1; 
            SetScreen(); // Load Grid  
        }
        // print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

            string P1 = "UNMATCHED TRANSACTIONS FOR : " + WCategoryId;
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId;
            string P6 = "No date";
            string P7 = "No date";

            //string P5 = "1005";
            Form56R66VISA ReportVISA66 = new Form56R66VISA(P1, P2, P3, P4, P5, P6, P7);
            ReportVISA66.Show();
        }
        // Make Adjustments 
        private void checkBoxAdj_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAdj.Checked == true)
            {

                FindInternalCustAccNoAndRate();

                if (AccountFound == false)
                {
                    MessageBox.Show("Not Available AccNo");
                }

                if (GTotal2 < 0)
                {
                    // External less than internal => make Internal to correct 
                    labelCRDR.Text = "CR";
                    labelCRDR.ForeColor = Color.Blue;
                    textBoxAmt.Text = (-GTotal2).ToString();
                }
                if (GTotal2 > 0)
                {
                    // Internal less than external 
                    if (WResponseCodeR == "200000")
                    {
                        // Credit Adjustment 
                        labelCRDR.Text = "CR";
                        labelCRDR.ForeColor = Color.Blue;
                    }
                    else
                    {
                        labelCRDR.Text = "DR";
                        labelCRDR.ForeColor = Color.Red;
                    }

                    textBoxAmt.Text = (GTotal2).ToString();
                }


                textBoxDetails.Text = "Adjustment for RRN: " + WInternalReference2;
                textBox8.Text = TodaysCcyRate.ToString();
                if (AccountFound == true)
                {
                    textBoxAccNo.Text = WCustAccNo;
                    textBoxAccNo.ReadOnly = true;
                }
                else
                {
                    textBoxAccNo.ReadOnly = false;
                }

                RRDMMatchingCategories Mc = new RRDMMatchingCategories();

                Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);
                textBoxGL_1.Text = Mc.GlAccount;

                checkBoxDisputes.Checked = false;

                panel5.Show();

            }
            else
            {
                //checkBox6.Hide(); 
                panel5.Hide();
            }
        }

        int InternalSeqNo;
        bool AccountFound;
        string WCustAccNo;
        decimal TodaysCcyRate;
        int WTempRSeqNo;
        private void FindInternalCustAccNoAndRate()
        {
            AccountFound = false;

            // Read all to find internal 
            int K = 0;
            //
            //VALIDATION 
            //
            while (K <= (dataGridView2.Rows.Count - 1))
            {
                WTempRSeqNo = (int)dataGridView2.Rows[K].Cells["SeqNo"].Value;
                ROrigin = (string)dataGridView2.Rows[K].Cells["Origin"].Value;
                RAmt = (decimal)dataGridView2.Rows[K].Cells["Amount"].Value;
                string RespCode = (string)dataGridView2.Rows[K].Cells["RESP CODE"].Value;

                if (ROrigin == "EXTERNAL" & RespCode == "200000")
                {
                    // Search to find the encrypted card
                    SelectionCriteria = " WHERE SeqNo =" + WTempRSeqNo;
                    Mg.ReadTable_BySelectionCriteria(TableA, "", SelectionCriteria,2);

                    // Find the first record with the same encrypted card
                    SelectionCriteria = " WHERE Card_Encrypted ='" + Mg.Card_Encrypted + "'";
                    Mg.ReadTable_BySelectionCriteria_FirstRecord(Table_IST, SelectionCriteria,2);
                    if (Mg.RecordFound == true & Mg.AccNo != "")
                    {
                        AccountFound = true;
                        WCustAccNo = Mg.AccNo;
                        TodaysCcyRate = 1;
                        return;
                    }
                    else
                    {
                        AccountFound = false;
                    }

                }

                if (ROrigin == "INTERNAL")
                {
                    AccountFound = true;

                    WTable = TableB;

                    SelectionCriteria = " WHERE SeqNo =" + WTempRSeqNo;

                    Mg.ReadTable_BySelectionCriteria(WTable, "", SelectionCriteria,2);

                    InternalSeqNo = WTempRSeqNo;
                    TodaysCcyRate = 1;
                    WCustAccNo = Mg.AccNo;
                    WInternalReference2 = Mg.RRNumber;
                    return;

                }

                K++; // Read Next entry of the table 
            }

        }
        //
        // Do Matching 
        //
        int RSeqNo;
        string ROrigin;
        decimal RAmt;
        int WMpaUniqueRecordId;
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        private void buttonDoMatch_Click(object sender, EventArgs e)
        {

            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("No TXNS for matching.");
                return;
            }

            if (WDisputesOn2 == true)
            {
                MessageBox.Show("There is outstanding dispute for this set. Id = " + WDispNo);
                return;
            }

            if (GTotal2 != 0)
            {
                MessageBox.Show("Total different than zero. Make adjustment please.");
                checkBoxAdj.Show();
                return;
            }
            else
            {
                checkBoxAdj.Hide();
            }

            if (comboBoxReason.Text == "Fill Entry")
            {
                MessageBox.Show("Please select reason");
                return;
            }
            else
            {
                //WMatchedType = comboBoxReason.Text;
            }

            bool ExternalFound = false;
            int K = 0;
            //
            // FIND EXTERNAL
            //


            Form14b NForm14b;

            while (K <= (dataGridView2.Rows.Count - 1))
            {
                RSeqNo = (int)dataGridView2.Rows[K].Cells["SeqNo"].Value;
                ROrigin = (string)dataGridView2.Rows[K].Cells["Origin"].Value;
                RAmt = (decimal)dataGridView2.Rows[K].Cells["Amount"].Value;

                if (ROrigin == "EXTERNAL")
                {
                    SelectionCriteria = " WHERE SeqNo =" + RSeqNo;

                    Mg.ReadTable_BySelectionCriteria(TableA, "", SelectionCriteria,2);

                    SelectionCriteria = " WHERE MatchingCateg='" + Mg.MatchingCateg + "'"
                        + " AND OriginalRecordId =" + RSeqNo;

                    Mpa.ReadInPoolTransSpecificBySelectionCriteria(SelectionCriteria,2);

                    WMpaUniqueRecordId = Mpa.UniqueRecordId;

                    ExternalFound = true;
                    // Find Unique Number 

                }

                K++; // Read Next entry of the table 
            }

            if (ExternalFound == false)
            {
                MessageBox.Show("External Not Found. Matching Cannot be done!");
                return;
            }



            Er.ReadErrorsIDRecord(Mpa.MetaExceptionId, WOperator);
            if (Er.RecordFound == true & Er.NeedAction == false)
            {
                MessageBox.Show("No action needed. Force this exception to match");

                return;
            }
            //

            K = 0;
            //
            // UPDATE RECORDS
            //

            while (K <= (dataGridView2.Rows.Count - 1))
            {
                RSeqNo = (int)dataGridView2.Rows[K].Cells["SeqNo"].Value;
                ROrigin = (string)dataGridView2.Rows[K].Cells["Origin"].Value;
                RAmt = (decimal)dataGridView2.Rows[K].Cells["Amount"].Value;

                if (ROrigin == "EXTERNAL")
                {
                    SelectionCriteria = " WHERE SeqNo =" + RSeqNo;

                    Mg.ReadTable_BySelectionCriteria(TableA, "", SelectionCriteria,2);

                    // UPDATE Mg record action type = 12 and UniqueAccountNumber
                    Mg.ActionType = "12";
                    Mg.UniqueRecordId = WMpaUniqueRecordId;
                    Mg.Comment = "Manual_Matching";

                    Mg.UpdateActionTypeBySeqNoForManualMatched(TableA, RSeqNo);

                }

                if (ROrigin == "INTERNAL" || ROrigin == "WAdjustment")
                {
                    SelectionCriteria = " WHERE SeqNo =" + RSeqNo;

                    Mg.ReadTable_BySelectionCriteria(TableB, "", SelectionCriteria,2);

                    // UPDATE Mg record action type = 12 and UniqueAccountNumber
                    Mg.ActionType = "12";
                    Mg.UniqueRecordId = WMpaUniqueRecordId;
                    Mg.Comment = "Manual_Matching";

                    Mg.UpdateActionTypeBySeqNoForManualMatched(TableB, RSeqNo);

                    if (ROrigin == "WAdjustment")
                    {
                        //
                        // INSERT META EXCEPTION 
                        //
                        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
                        Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);

                        int WMetaExceptionId = 145;
                        bool AffectCustomer = true;
                        string FirstDRCR = "DR";
                        int ErrSeqNo = InsertMetaException(WMpaUniqueRecordId
                                                           , Mg.AccNo
                                                           , Mc.GlAccount
                                                           , Mg.TransAmt
                                                           , Mg.TransDescr
                                                           , WMetaExceptionId
                                                           , AffectCustomer
                                                           , Mg.TransType
                                                          );

                        string ActionType = "2";
                        Mpa.MetaExceptionNo = ErrSeqNo;
                        Mpa.ActionByUser = true;
                        Mpa.UserId = WSignedId;

                        Mpa.ActionType = ActionType; // 

                        Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WMpaUniqueRecordId,1);

                    }
                }

                K++; // Read Next entry of the table 
            }

            //Home

            WReference = "";
            WOrderCriteria = " Origin ASC ";

            panel5.Hide();
            labelAmt.Hide();
            panel7.Hide();
            label28.Hide();
            comboBoxReason.Hide();

            MessageBox.Show("Matching Has Been Made." + Environment.NewLine
                 + "Move to next matching.");

            //   WMode_L = 1;
            SetScreen(); // Load Grid  
        }

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        private int InsertMetaException(int InUniqueRecordId
                                     , string InAccount_1
                                     , string InAccount_2
                                     , decimal InTransAmount
                                     , string InDescription
                                     , int InMetaExceptionId
                                     , bool InAffectCustomer
                                     , int InTransType)
        {
            //
            //  CASES FOR CREATING META-EXCEPTION
            //
            // CUSTOMER FOCUSED
            // DR CUSTOMER - Given Account CR GL Category 
            // CR CUSTOMER DR GL CATEGORY
            // GL FOCUSED
            // DR GL Given Account CR GL Given Account
            // CR GL Given ACCOUNT DR GL Given Account
            int WMetaExceptionId = InMetaExceptionId = 145; // Manual meta-exception

            int SeqNo;
            string SelectionCriteria = " WHERE UniqueRecordId = " + InUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);

            Er.ReadErrorsIDRecord(InMetaExceptionId, WOperator);

            if (Mpa.Origin == "Our Atms")
            {
                Ac.ReadAtm("NoATM");
                if (Ac.RecordFound == true)
                {
                    Er.BankId = Ac.BankId;
                    Er.BranchId = Ac.Branch;
                    Er.CitId = Ac.CitId;
                }
            }
            else
            {
                // Read Category 
                Er.BankId = WOperator;
                Er.BranchId = "BDC HeadQuarters";
                Er.CitId = "1000";
            }

            // INITIALISED WHAT IS NEEDED 

            Er.CategoryId = Mpa.RMCateg;
            Er.RMCycle = WReconcCycleNo;
            Er.UniqueRecordId = InUniqueRecordId;

            Er.AtmNo = Mpa.TerminalId;
            Er.SesNo = Mpa.ReplCycleNo;
            Er.DateInserted = DateTime.Now;
            Er.DateTime = DateTime.Now;

            Er.UniqueRecordId = InUniqueRecordId;

            Er.ByWhom = WSignedId;

            Er.CurDes = Mpa.TransCurr;
            Er.ErrAmount = InTransAmount;
            // First Entry 
            if (InAffectCustomer == true)
            {
                if (InTransType == 11)
                {
                    // DR
                    Er.DrCust = true;
                    Er.TransType = 11;
                    // CR
                    Er.CrAccount3 = true;
                }

                if (InTransType == 21)
                {
                    Er.CrCust = true;
                    Er.TransType = 21;
                    // DR
                    Er.DrAccount3 = true;
                }

                Er.CustAccNo = InAccount_1;
                // CATEGORY ACCOUNT NUMBER
                Er.AccountNo3 = InAccount_2;
            }
            else
            {
                // Fill the First GL
            }

            // Second Entry 


            Er.TraceNo = Mpa.TraceNoWithNoEndZero;
            Er.CardNo = Mpa.CardNumber;

            Er.TransDescr = InDescription;

            Er.DatePrinted = NullPastDate;

            Er.OpenErr = true;

            Er.UnderAction = true;

            Er.Operator = WOperator;

            SeqNo = Er.InsertError(); // INSERT ERROR 


            return SeqNo;
        }

        // ADD ADJUSTMENT 
        private void buttonAddAdjust_Click(object sender, EventArgs e)
        {
            if (textBoxAmt.Text == "" || textBoxDetails.Text == "" || textBoxAccNo.Text == "")
            {
                MessageBox.Show("Missing Information");
                return;
            }

            //
            // Read Fields of the selected row
            //
            if (WResponseCodeR == "200000")
            {
                WTable = TableA;
            }
            else
            {
                WTable = TableB;
            }


            SelectionCriteria = " WHERE SeqNo =" + WTempRSeqNo;

            Mg.ReadTable_BySelectionCriteria(WTable, "", SelectionCriteria,2);
            if (Mg.RecordFound == true)
            {
                // Build the needed transaction
                if (WCategoryId == "BDC231") Mg.OriginFileName = "Flexcube";

                Mg.TransTypeAtOrigin = "WAdjustment"; // Internal

                Mg.TransDescr = textBoxDetails.Text;
                // Sec.SubSystem = WSubSystem;

                Mg.AccNo = textBoxAccNo.Text;

                Mg.ActionType = "3"; // 

                if (decimal.TryParse(textBoxAmt.Text, out Mg.TransAmt))
                {
                }
                else
                {
                    MessageBox.Show(textBoxAmt.Text, "Please enter a valid Amount!");
                    return;
                }

                //if (decimal.TryParse(textBox8.Text, out Mg.TransCurr))
                //{
                //}
                //else
                //{
                //    MessageBox.Show(textBox8.Text, "Please enter a valid Rate!");
                //    return;
                //}
                if (labelCRDR.Text == "CR")
                {
                    Mg.TransType = 21;
                    Mg.Comment = "This is a Credit Adjustment!";
                }
                else
                {
                    Mg.TransType = 11;
                    Mg.Comment = "This is a Debit Adjustment!";
                }

                int InsertedId = Mg.InsertNewRecordInTble(TableB);

            }
            else
            {
                MessageBox.Show("Record Not Found");
                return;
            }

            // Clear fields 
            textBoxAmt.Text = "";
            textBoxDetails.Text = "";

            textBox10.Text = "";
            //  WMode_L = 1; 
            SetScreen(); // Load Grid  

            checkBoxAdj.Checked = false;

        }
        // Update Adjustments 
        private void buttonUpdateAdjust_Click(object sender, EventArgs e)
        {
            if (textBoxAmt.Text == "" || textBoxDetails.Text == "")
            {
                MessageBox.Show("Missing Information");
                return;
            }
            MessageBox.Show("Discuss the need for Updating. Currently Not Available. ");
            return;

            //SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + WSeqNo2;

            //Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(SelectionCriteria);

            //if (Sec.Origin != "WAdjustment")
            //{
            //    MessageBox.Show("You can delete only Adjustment");
            //    return;
            //}

            //if (decimal.TryParse(textBoxAmt.Text, out Sec.TxnAmt))
            //{
            //}
            //else
            //{
            //    MessageBox.Show(textBoxAmt.Text, "Please enter a valid Amt!");
            //    return;
            //}

            //if (decimal.TryParse(textBox8.Text, out Sec.TxnCcyRate))
            //{
            //}
            //else
            //{
            //    MessageBox.Show(textBox8.Text, "Please enter a valid Rate!");
            //    return;
            //}

            //if (Sec.TxnAmt > 0)
            //{
            //    Sec.TxnCode = "21";
            //}

            //if (Sec.TxnAmt < 0)
            //{
            //    Sec.TxnCode = "11";
            //}

            //Sec.StmtLineSuplementaryDetails = textBoxDetails.Text;
            //Sec.MatchedType = "";

            ////Sec.AdjGlAccount = comboBoxGl.Text;

            //Sec.UpdateAdjustmentSpecific(WSeqNo2);

            WRowGrid2 = dataGridView2.SelectedRows[0].Index;

            SetScreen(); // Load Grid  

            dataGridView2.Rows[WRowGrid2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid2));

        }
        // DELETE ADJ
        private void buttonDeleteAdjust_Click(object sender, EventArgs e)
        {
            if (Mg.TransTypeAtOrigin != "WAdjustment")
            {
                MessageBox.Show("This not Adjustment. You can delete Adjustment Only");
                return;
            }

            if (MessageBox.Show("Warning: Do you want to delete this Adjustment?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                Mg.DeleteRecordByOriginFileAndSeqNo(WTable, WSeqNo2);

                WRowGrid2 = dataGridView2.SelectedRows[0].Index;

                //    WMode_L = 1 ; 
                SetScreen(); // Load Grid   

            }
            else
            {
            }
        }
        // CLEAR SCREEN 
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                textBoxAmt.Text = "";
                textBoxDetails.Text = "";
                textBox8.Text = "";
                textBox10.Text = "";
                comboBoxReason.Text = "Fill Entry";
            }
        }
        // MAKE DISPUTE 
        private void buttonMakeDispute_Click(object sender, EventArgs e)
        {
            // Go to open Dispute 
            Form5NV NForm5NV;
            int From = 1;
            NForm5NV = new Form5NV(WSignedId, WSignRecordNo, WOperator,
                           From, WCategoryId, Sec.TableNVCards_Records_Both, 0);
            NForm5NV.FormClosed += NForm5NV_FormClosed;
            NForm5NV.ShowDialog();
        }
        private void NForm5NV_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetScreen(); // Load Grid  
        }

        // Notes 3

        private void buttonNotes3_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "UniqueRecordId: " + Mg.UniqueRecordId;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WModeN = "Read";
            else WModeN = "Update";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WModeN, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mg.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "UNMATCHED")
            {
                WMode_L = 1;
                WMode_R = 3;
            }
            if (comboBox1.Text == "MANUAL_MATCHED")
            {
                WMode_L = 5; // For Manual Matched
                WMode_R = 7;
            }

            SetScreen();
        }
        // UNMATCHED
        private void buttonUnMatch_Click(object sender, EventArgs e)
        {
            // 
            // WActionType = "0";
            // UPDATE with Zero Action Type all with the same Unique number 

            MessageBox.Show("Adjustment will be deleted And the set will be unmatched");

            Mg.DeleteAdjustmentRecordByUniqueNo(TableB, Mg.UniqueRecordId);
            //
            // UPDATE REMAINING RECORDS
            //
            // TableA
            Mg.UpdateActionTypeByUniqueNoForManualMatched(TableA, Mg.UniqueRecordId);
            // TableB
            Mg.UpdateActionTypeByUniqueNoForManualMatched(TableB, Mg.UniqueRecordId);

            // UPDATE Mpa
            SelectionCriteria = " WHERE UniqueRecordId =" + Mg.UniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);
            int SaveMetaExceptionNo = Mpa.MetaExceptionNo; 
            Mpa.MetaExceptionNo = 0;
            Mpa.ActionByUser = false;
            Mpa.UserId = WSignedId;
            Mpa.ActionType = "0";

            Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, Mg.UniqueRecordId,1);
            //
            // Delete Meta_exception
            //
            Er.DeleteErrorRecordByErrNo(SaveMetaExceptionNo);

            SetScreen();
        }
        // Interrogate 
        private void linkLabelInterogate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string WTable = TableA; // OR it can be IST or it can be Flexcube 
            int WModeAdj = 21;

            Mg.ReadMergedTXNWithOthersFOR_Adj(WTable, WCategoryId, WCard_EncryptedR, WTransDateR.Date.AddDays(-30), WModeAdj);

            // Form271View_Table(DataTable InTempTable, string InHeading)
            Form271View_Table NForm271View_Table;

            string WHeading = "TXNS amount..:.." + WTransAmtR.ToString() + "..And..Merchant..Info.." + WFullTraceNoR;
            int WWhatGrid = 1;
            NForm271View_Table = new Form271View_Table(WHeading, Mg.TablePOS_Settlement_Adj, WWhatGrid);

            NForm271View_Table.ShowDialog();
        }
        // Print Actions 
        private void buttonPrintActions_Click(object sender, EventArgs e)
        {

            // Matching is done but not Settled 
            SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WCategoryId + "'"
                      + "  AND MatchingAtRMCycle =" + WReconcCycleNo
                      + " AND IsMatchingDone = 1 AND Matched = 0  "
                      //+ " AND IsMatchingDone = 1 AND Matched = 0 AND SettledRecord = 0 "
                      + " AND ActionType != '7' ";
            
            string WSortCriteria = " ORDER BY ActionType DESC ";

            Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria,1);

            string P1 = "Transactions For Reconciliation :" + WCategoryId
                         + " AND Cycle : " + WReconcCycleNo.ToString();

            string P2 = "";
            string P3 = "";
            if (ViewWorkFlow == true)
            {

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WReconcCycleNo, "ReconciliationCat");

                if (Ap.RecordFound == true)
                {
                    Us.ReadUsersRecord(Ap.Requestor);
                    P2 = Us.UserName;
                    Us.ReadUsersRecord(Ap.Authoriser);
                    P3 = Us.UserName;
                }
                else
                {
                    //ReconciliationAuthorNoRecordYet = true;
                }

            }
            else
            {
                Us.ReadUsersRecord(WSignedId);
                P2 = Us.UserName;
                P3 = "N/A";
            }

            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R55ATMS ReportATMS55 = new Form56R55ATMS(P1, P2, P3, P4, P5);
            ReportATMS55.Show();
        }
    }
}

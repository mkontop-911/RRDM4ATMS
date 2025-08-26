using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm271a : UserControl
    {

        //public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        string WCcy;

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMReconcCategATMsAtRMCycles RAtms = new RRDMReconcCategATMsAtRMCycles();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool Is_Presenter_InReconciliation ;

        bool linkLabel1Alert, linkLabel2Alert, linkLabel3Alert, linkLabel4Alert, linkLabel5Alert, linkLabel6Alert;
        //bool IsOriginAtms;
        bool ViewWorkFlow;
        string SelectionCriteria;
        int CallingMode;
        string WSortCriteria = "";
        string WBankId; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;
        int WRMCycle;

        public void UCForm271aPar(string InSignedId, int InSignRecordNo, string InOperator, string InCategory, int InRMCycle)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategoryId = InCategory;

            WRMCycle = InRMCycle;

            InitializeComponent();

            Us.ReadUsersRecord(WSignedId);
            WBankId = Us.BankId;
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WBankId); 
            WCcy = Ba.BasicCurName;

            if (WOperator == "BDACEGCA")
            {
                // ABE
                buttonAllAccounting.Hide();

                panelLightAlert.Hide(); 
            }

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WRMCycle);
            //Rcs.ReadMatchingCategoriesSessionsByRunningJobNo(WOperator, WRMCycle);

            label4.Text = "RM CATEGORY : " + WCategoryId + "--" + Rcs.CategoryName + " ---- RM CYCLE : " + WRMCycle.ToString();

            if (WCategoryId == "EWB311")
            {
                label12.Text = "GL Balance";
            }

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }
            else
            {
                if (Usi.ProcessNo == 2) ViewWorkFlow = false;
            }

            if (ViewWorkFlow == false) // UPDATING  
            {
                // Matching is done but not Settled 
                SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WCategoryId + "'"
                          + "  AND IsMatchingDone = 1 AND FastTrack = 0 "
                          + "  AND Matched = 0 AND SettledRecord = 0 " + " AND ActionType != '07' ";
                CallingMode = 2; // Updating 

                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

                WSortCriteria = " ORDER By TerminalId";

               // Mpa.UpdateExceptionIds(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt,1);
            }

            // Presenter Role ???
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "946";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Presenter_InReconciliation = true;
            }

        }

        // SHOW SCREEN 

        public void SetScreen()
        {
            //
            // LEFT 
            //
            Color Red = Color.Red;
            Color Black = Color.Black;

            SelectionCriteria = " WHERE RMCateg ='"
                       + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle + " AND IsMatchingDone = 1 ";
            //SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
            //         + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle + " AND IsMatchingDone = 1 ";

            Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria,2);

            label5.Text = "Start Date Time...: " + Rcs.StartReconcDtTm.ToString();
            label6.Text = "GL Currency       : " + WCcy;
            label7.Text = "GL Account        : " + Rcs.GlAccountNo;
            label13.Text = "Matched Trans Amt............ : " + Rcs.MatchedTransAmt.ToString("#,##0.00");
            label14.Text = "UNMatched Amt................ : " + Rcs.NotMatchedTransAmt.ToString("#,##0.00");
            label26.Text = "Adjusted with default Actions : " + Rcs.SettledUnMatchedAmtDefault.ToString("#,##0.00");

            if (Is_Presenter_InReconciliation == true)
            {
                Mpa.ReadPoolAndFindTotals_Presenter_PerRMCategory(WRMCycle, WCategoryId, 2);

                labelPresenterAmt.Show();
               
                labelPresenterAmt.Text = "Presenter Errors..............: " + (Mpa.PresenterSettledAmt + Mpa.Presenter_Not_SettledAmt).ToString("#,##0.00");
            }
            else
            {
                labelPresenterAmt.Hide(); 
            }
            


            if (WCategoryId.Substring(0, 4) == "RECA")
            {
                linkLabelMatchedCateg.Show();
            }
            else
            {
                linkLabelMatchedCateg.Hide();
            }

            //
            // Right side
            //

            decimal TempTotal = Rcs.GlYesterdaysBalance + Rcs.MatchedTransAmt + Rcs.SettledUnMatchedAmtDefault;
            textBox1.Text = TempTotal.ToString("#,##0.00");
            textBox2.Text = Rcs.GlTodaysBalance.ToString("#,##0.00");
            textBox3.Text = (Rcs.GlTodaysBalance - TempTotal).ToString("#,##0.00");


            if ((Rcs.GlTodaysBalance - TempTotal) != 0)
            {
                label15.ForeColor = Red;
            }
            else
            {
                label15.ForeColor = Black;
            }
            if (Is_Presenter_InReconciliation == true)
            {
                Rcs.ReadReconcCategoriesSessionsSpecificCatWithPresenter(WOperator, WCategoryId, WRMCycle);
                //Rca.TotalNumberOfUnMatchedRecs.ToString();
                //labelUnMatch3.Text = "Amount for UnMatched.......: " + Rca.TotalUnMatchedAmt.ToString("#,##0.00");
                // textBox7.Text = (Mpa.TotalUnMatched - Mpa.TotalDefaultActionBySystem+(Mpa.PresenterSettled+Mpa.Presenter_Not_Settled)).ToString();
                textBox7.Text = Rcs.NumberOfUnMatchedRecs.ToString();
                textBox6.Text = Rcs.AmtOfUnMatchedWithPresenter.ToString("#,##0.00");

                textBoxMetaExcep.Text = Mpa.TotalActionsByUserDefaultAndManual.ToString();
                textBox4.Text = Mpa.TotalAmountByUserDefaultAndManual.ToString("#,##0.00");
                textBoxForcedMatchDisp.Text = (Mpa.TotalForcedMatched + Mpa.TotalMoveToPool + Mpa.TotalMoveToDisputeNumber+ Mpa.TotalPresenterWithAction).ToString();
                textBox5.Text = (Mpa.TotalForcedMatchedAmount + Mpa.TotalMoveToPoolAmount + Mpa.TotalMoveToDisputeAmt+ Mpa.TotalPresenterWithActionAmt).ToString("#,##0.00");

                textBoxEffect.Text = (Mpa.TotalAmountByUserDefaultAndManual + Mpa.TotalForcedMatchedAmount + Mpa.TotalMoveToPoolAmount + Mpa.TotalMoveToDisputeAmt+ Mpa.TotalPresenterWithActionAmt).ToString("#,##0.00");

                textBox8.Text = (Rcs.AmtOfUnMatchedWithPresenter - (Mpa.TotalAmountByUserDefaultAndManual + Mpa.TotalForcedMatchedAmount + Mpa.TotalMoveToPoolAmount + Mpa.TotalMoveToDisputeAmt+ Mpa.TotalPresenterWithActionAmt)).ToString("#,##0.00");

            }
            else
            {
                textBox7.Text = Rcs.NumberOfUnMatchedRecs.ToString();
                textBox6.Text = Rcs.NotMatchedTransAmt.ToString("#,##0.00");

                textBoxMetaExcep.Text = Mpa.TotalActionsByUserDefaultAndManual.ToString();
                textBox4.Text = Mpa.TotalAmountByUserDefaultAndManual.ToString("#,##0.00");
                textBoxForcedMatchDisp.Text = (Mpa.TotalForcedMatched + Mpa.TotalMoveToPool + Mpa.TotalMoveToDisputeNumber).ToString();
                textBox5.Text = (Mpa.TotalForcedMatchedAmount + Mpa.TotalMoveToPoolAmount + Mpa.TotalMoveToDisputeAmt).ToString("#,##0.00");

                textBoxEffect.Text = (Mpa.TotalAmountByUserDefaultAndManual + Mpa.TotalForcedMatchedAmount + Mpa.TotalMoveToPoolAmount + Mpa.TotalMoveToDisputeAmt).ToString("#,##0.00");

                textBox8.Text = (Rcs.NotMatchedTransAmt - (Mpa.TotalAmountByUserDefaultAndManual + Mpa.TotalForcedMatchedAmount + Mpa.TotalMoveToPoolAmount + Mpa.TotalMoveToDisputeAmt)).ToString("#,##0.00");

            }


            SetGrid();

        }

        // ROW ENTER 
        string WAtmNo;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[1].Value;

            //SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
            //      + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle
            //      + " AND FastTrack = 1 "
            //      + " AND TerminalId ='" + WAtmNo + "'"
            //      + " AND IsMatchingDone = 1 ";
            SelectionCriteria = " WHERE RMCateg ='"
                       + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle + " AND IsMatchingDone = 1 AND FastTrack = 1 ";

            Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria,2);

            textBox11.Text = Mpa.TotalFastTrack.ToString();
            textBox13.Text = Mpa.TotalFastTrackAmount.ToString("#,##0.00");

            if (Mpa.TotalFastTrack > 0)
            {
                radioButtonPerATM.Checked = true;
                radioButtonForALL.Checked = false;
                radioButtonNoFastTrack.Checked = false;
            }
            else
            {
                // Initialise Fast Track 
                radioButtonPerATM.Checked = false;
                radioButtonForALL.Checked = false;
                radioButtonNoFastTrack.Checked = true;            
            }

        }
        int WRowIndex;
        private void buttonFastTrack_Click(object sender, EventArgs e)
        {         
            Form271FastTrack NForm271FastTrack;

            if (radioButtonPerATM.Checked ==true)
            {
                NForm271FastTrack = new Form271FastTrack(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycle, WAtmNo);
                NForm271FastTrack.FormClosed += NForm271FastTrack_FormClosed;
                NForm271FastTrack.ShowDialog();
            }
            else
            {
                // Fast Track for all
                NForm271FastTrack = new Form271FastTrack(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycle, "ALL");
                NForm271FastTrack.FormClosed += NForm271FastTrack_FormClosed;
                NForm271FastTrack.ShowDialog();
            }
         
        }

        void NForm271FastTrack_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            InitialiseFastTrackFields();

            SetScreen();

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;


        }
     //
    //    Initialise Fast Track Fields
    //
        private void InitialiseFastTrackFields()
        {
            if (radioButtonPerATM.Checked == true)
            {
                SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                    + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle
                    + " AND FastTrack = 1 "
                    + " AND TerminalId ='" + WAtmNo + "'"
                    + " AND IsMatchingDone = 1 ";
            }
            if (radioButtonForALL.Checked == true)
            {
                // Fast Track for all
                SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                   + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle
                   //  + " AND TerminalId ='" + WAtmNo + "'"
                   + " AND FastTrack = 1 "
                   + " AND IsMatchingDone = 1 ";
            }

            Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria,2);

            textBox11.Text = Mpa.TotalFastTrack.ToString();
            textBox13.Text = Mpa.TotalFastTrackAmount.ToString("#,##0.00");
        }

       


        private void radioButtonPerATM_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPerATM.Checked == true)
            {
                InitialiseFastTrackFields();
                //radioButtonForALL.Checked = false; 
                panel5.Show();
            }
            else
            {
                panel5.Hide();
            }
        }


        private void radioButtonForALL_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonForALL.Checked == true)
            {
                InitialiseFastTrackFields();
                //radioButtonPerATM.Checked = false; 
                panel5.Show();
            }
            else
            {
                panel5.Hide();
            }
        }
// Matched Categories 
        private void linkLabelMatchedCateg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // SHOW MATCHED
            //
            Form78d_SlaveCategories NForm78d_SlaveCategories;
            int WMode = 5; // show slave 
            NForm78d_SlaveCategories = new Form78d_SlaveCategories(WOperator, WSignRecordNo, WSignedId, WCategoryId, WRMCycle, WMode);
            NForm78d_SlaveCategories.ShowDialog();
           
        }
// Show All Actions 
        private void buttonAllActions_Click(object sender, EventArgs e)
        {
            RRDMGasParameters Gp = new RRDMGasParameters();

            string WSelectionCriteria = "";
            int WMode;

            WSelectionCriteria = "WHERE RMCateg='" + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle + " AND OriginWorkFlow ='Reconciliation'"; ;

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            // Auto Creation Of Transactions
            //
            bool Is_GL_Creation_Auto;

            string ParId = "945";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_GL_Creation_Auto = true;
                buttonAllAccounting.Show();
            }
            else
            {
                Is_GL_Creation_Auto = false;
                buttonAllAccounting.Hide();
            }

            if (Is_GL_Creation_Auto == true)
            {
                WMode = 3;
                Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);
            }
            else
            {
                // Manual operation 
                WMode = 4;
                Aoc.ReadActionsOccurancesAndFillTable_Small_Manual(WSelectionCriteria);
            }

            //string WUniqueRecordIdOrigin = "Master_Pool";
            // PROVIDE TABLE to FORM
            Form14b_All_Actions NForm14b_All_Actions;

            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();


        }
        // Accountng Entries 
        private void buttonAllAccounting_Click(object sender, EventArgs e)
        {

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
           

            SelectionCriteria = "WHERE RMCateg='" + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycle;
            Aoc.ReadActionsOccurancesAndFillTable_Big(SelectionCriteria);

            Aoc.ClearTableTxnsTableFromAction();

            int I = 0;

            while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
            {
                //    RecordFound = true;
                int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                if (Aoc.Is_GL_Action == true)
                {

                    int WMode2 = 1; // DO NOT Create transaction in pool 
                    string WCallerProcess = "Reconciliation";
                    Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey,
                                                                 Aoc.ActionId,Aoc.Occurance ,WCallerProcess, WMode2);
                }

                I = I + 1;
            }

            DataTable TempTxnsTableFromAction;

            string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;

            // Aoc.ReadActionsTxnsCreateTableByUniqueKey(WUniqueRecordIdOrigin, Dt.UniqueRecordId, "All");

            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;
            //Form14b_All_Actions NForm14b_All_Actions;
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void labelAlert_Click(object sender, EventArgs e)
        {

        }

        private void radioButtonNoFastTrack_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNoFastTrack.Checked == true)
            {
                radioButtonPerATM.Checked = false;
                radioButtonForALL.Checked = false;

                panel5.Hide();
            }
        }

        // SetUpGrid
        private void SetGrid()
        {
            int Mode = 1; // Read only 
            RAtms.ReadReconcCategoriesATMsRMCycleToFillTable(WOperator, WCategoryId, WRMCycle, Mode);

            if (RAtms.TableRMATMsCycles.Rows.Count ==0)
            {
                MessageBox.Show("No ATMs In difference. Have you done matching? Please check");
                return; 
            }

            dataGridView1.DataSource = RAtms.TableRMATMsCycles.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            //TableRMATMsCycles.Columns.Add("SeqNo", typeof(int));
            //TableRMATMsCycles.Columns.Add("AtmNo", typeof(string));
            //TableRMATMsCycles.Columns.Add("MatchingCategoryId", typeof(string));
            //TableRMATMsCycles.Columns.Add("Matched", typeof(int));
            //TableRMATMsCycles.Columns.Add("UnMatched", typeof(int));
            //TableRMATMsCycles.Columns.Add("MatchedAmt", typeof(decimal));
            //TableRMATMsCycles.Columns.Add("UnMatchedAmt", typeof(decimal));
            //TableRMATMsCycles.Columns.Add("NotInJournal", typeof(decimal));

            dataGridView1.Columns[0].Width = 60; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false; 

            dataGridView1.Columns[1].Width = 90; // AtmNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 75; // MatchingCategoryId
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = false; 

            dataGridView1.Columns[3].Width = 75; // Matched Recs
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 75; // UnMatched Recs
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 90; // MatchedAmt
            dataGridView1.Columns[5].DefaultCellStyle = style;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[6].Width = 75; // UnMatchedAmt
            dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[7].Width = 70; // Not In Journal 
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].Visible = false;

            dataGridView1.Columns[8].Width = 90; // OpeningBalance
            dataGridView1.Columns[8].DefaultCellStyle = style;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[9].Width = 95; // JournalAmt
            dataGridView1.Columns[9].DefaultCellStyle = style;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[10].Width = 95; // difference
            dataGridView1.Columns[10].DefaultCellStyle = style;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            bool AlertFound = false;
            //string NotInJournalString;
            //int NotInJournalInt;
            RRDMGasParameters Gp = new RRDMGasParameters();
            Gp.ReadParametersSpecificId(WOperator, "221", "1", "", ""); // > is Red 
            int NotInJournalLimit = (int)Gp.Amount;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int WSeqNo = (int)row.Cells[0].Value;

                RAtms.ReadReconcCategoriesATMsRMCycleBySeqNo(WSeqNo); 

                if (RAtms.NumberOfUnMatchedRecs > NotInJournalLimit)
                {
                    AlertFound = true;
                    if (ShowAlertLines == true) // THIS IS true when picture is pressed 
                    {
                        row.DefaultCellStyle.BackColor = Color.SaddleBrown;
                        row.DefaultCellStyle.ForeColor = Color.White;
                    }

                    labelAlert.Show();
                    pictureBox2.Show();
                    textBoxBrokenDisc.Show();
                    pictureBox2.BackgroundImage = appResImg.RED_LIGHT;
                }
            }

        }
        // Test 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Matching is done but not Settled 
            SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WCategoryId + "'"
                      + "  AND MatchingAtRMCycle =" + WRMCycle
                      + " AND IsMatchingDone = 1 AND Matched = 0  "
                      //+ " AND IsMatchingDone = 1 AND Matched = 0 AND SettledRecord = 0 "
                      + " AND ActionType != '07' ";

            WSortCriteria = "Order By TerminalId, SeqNo "; 

            Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria,1);
  
            string P1 = "Transactions For Reconciliation :" + WCategoryId + " AND Cycle : " + WRMCycle.ToString();
            
            string P2 = "";
            string P3 = "";
            if (ViewWorkFlow == true)
            {

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WRMCycle, "ReconciliationCat");

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

            string P4 = WBankId;
            string P5 = WSignedId;

            Form56R55ATMS ReportATMS55 = new Form56R55ATMS(P1, P2, P3, P4, P5);
            ReportATMS55.Show();
        }
        // Show Screen
        // Show Alert Lines 
        bool ShowAlertLines;
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ShowAlertLines = true;
            SetScreen();
        }
    }
}

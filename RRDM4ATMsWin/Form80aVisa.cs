using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form80aVisa : Form
    {
        //Form_NOSTRO_OPERATIONAL NForm_NOSTRO_OPERATIONAL; 
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReconcJobCycles Djc = new RRDMReconcJobCycles();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime WWorkingDate;

        string WBankId;

        //string W4DigitMainCateg;

        int WReconcCycleNo;

        string WSubSystem;

        int WCurrentJobCycle;

        string WJobCategory;

        int WRowIndexLeft;

        string WCategoryId;

        string WWFunction;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        //string WFunction;
        string WReconcCategory;

        public Form80aVisa(string InSignedId, int SignRecordNo, string InOperator,
            string InFunction, string InReconcCategory)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WWFunction = InFunction;
            //WFunction = "View";

            WReconcCategory = InReconcCategory; // Specific or "ALL"

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            label1UserId.Text = WSignedId;

            Us.ReadUsersRecord(InSignedId);
            WBankId = Us.BankId;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            WJobCategory = "CardsSettlement";
            WSubSystem = "CardsSettlement"; // Used For Categories       

            Djc.ReadLastReconcJobCycleATMsAndNostro(WOperator, WJobCategory);
            if (Djc.RecordFound == true)
            {
                WCurrentJobCycle = Djc.JobCycle;
                WWorkingDate = Djc.StartDateTm;
            }

            if (WWFunction == "View")
            {
                textBoxMsgBoard.Text = "View Details of Reconciliation and Matching Cycles";

                labelStep1.Text = "View Visa Matched";

                buttonNext.Text = "Next";
            }

            if (WWFunction == "Matched")
            {
                textBoxMsgBoard.Text = "View Details of Reconciliation and Matching Cycles";

                labelStep1.Text = "Visa Matched";

                buttonNext.Text = "Next";

            }

            if (WWFunction == "Confirm Entries")
            {
                textBoxMsgBoard.Text = "Go to Confirmed";

                //buttonNext.Text = "Confirmed";
                buttonNext.Text = "Next";

                labelStep1.Text = "Visa Matched To be confirmed";

            }

            if (WWFunction == "ReconcVisa")
            {
                textBoxMsgBoard.Text = "Go to Manual Reconciliation";

                buttonNext.Text = "Next";

                labelStep1.Text = "Visa Settlement Reconciliation - Go Manual";

            }

            if (WWFunction == "OutstandingAdjust")
            {
                textBoxMsgBoard.Text = "Go to Manage Outstanding Adjustments";

                buttonNext.Text = "To Adjust";

                labelStep1.Text = "Nostro Outstanding Adjustments";

            }
        }
        // ON LOAD
        private void Form80_Load(object sender, EventArgs e)
        {
            string SelectionCriteria = " WHERE RunningJobNo =" + WCurrentJobCycle + "";

            Rcs.ReadNVReconcCategoriesSessionsSpecificRunningJobCycle(SelectionCriteria);

            ShowGridRcs();
        }

        // ON ROW ENTER CATEGORIES
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WCategoryId = rowSelected.Cells[1].Value.ToString();

            Rcs.ReadNVReconcCategoriesSessionsSpecificCat(WOperator, WCategoryId);
            if (Rcs.TableReconcSessionsPerCategory.Rows.Count == 0)
            {
                labelRight.Hide();
                panel3.Hide();
                MessageBox.Show("No testing data for this category");
            }

            // Fill Grid 2 
            dataGridView2.DataSource = Rcs.TableReconcSessionsPerCategory.DefaultView;

            dataGridView2.Columns[0].Width = 90; // SeqNo 
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);
            dataGridView2.Columns[0].Visible = false;

            dataGridView2.Columns[1].Width = 80; // "RunningJobNo"
            dataGridView2.Columns[1].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[2].Width = 100; // "CategoryId"
            dataGridView2.Columns[2].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView2.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 120; // "StartManual"
            dataGridView2.Columns[3].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView2.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 120; // "EndManual"
            dataGridView2.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView2.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 120; // "UnMatchedRecs"
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[5].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }
        int WSeqNo;
        // On Row Enter for Matching and reconciliations Cycles 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Rcs.ReadNVReconcCategoriesSessionsBySeqNo(WSeqNo);

            WReconcCycleNo = Rcs.RunningJobNo;

            ShowPanel(Rcs.ExternalAccNo, Rcs.InternalAccNo, WWorkingDate);

            label10BalStat.Text = "SUMMARY RECONCILIATION AS AT :" + WWorkingDate.ToShortDateString();

        }

        // Go to Reconciliation or other functionality 
        private void buttonNext_Click(object sender, EventArgs e)
        {

            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            //NOSTRO CASE

            if (WWFunction == "View")
            {
                Form80bNV NForm80bNV;

                NForm80bNV = new Form80bNV(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo,
                                                  0, "View", NullPastDate, NullPastDate);
                NForm80bNV.FormClosed += General_FormClosed;
                NForm80bNV.ShowDialog();
            }

            if (WWFunction == "Matched")
            {
                Form291NVMatched_Visa NForm291NVMatched_Visa;
                int Mode = 1;
                string RefLike = "";
                NForm291NVMatched_Visa = new Form291NVMatched_Visa(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo, Mode, RefLike);
                NForm291NVMatched_Visa.FormClosed += General_FormClosed;
                NForm291NVMatched_Visa.ShowDialog();
            }

            if (WWFunction == "Confirm Entries")
            {

                Form291NVConfirmed_Cards NForm291NVConfirmed_Cards;

                NForm291NVConfirmed_Cards = new Form291NVConfirmed_Cards(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo);
                NForm291NVConfirmed_Cards.FormClosed += General_FormClosed;
                NForm291NVConfirmed_Cards.ShowDialog();
            }

            if (WWFunction == "ReconcVisa")
            {
               
              
                    Form502aVisa NForm502aVisa = new Form502aVisa(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo, WWorkingDate);

                    NForm502aVisa.FormClosed += General_FormClosed;
                    NForm502aVisa.ShowDialog();
              
            }


            if (WWFunction == "OutstandingAdjust")
            {
                Form502aNostroAdj NForm502aNostroAdj = new Form502aNostroAdj(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo, WWorkingDate);
                NForm502aNostroAdj.FormClosed += General_FormClosed;
                NForm502aNostroAdj.ShowDialog();
            }

            return;

        }

        // Show Grid RCS
        private void ShowGridRcs()
        {
            dataGridView1.DataSource = Rcs.TableReconcSessionsPerCategory.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[1].Width = 70; // CategoryId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 220; // Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 60; // ccy 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 90; // MatchedAuto
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 90; // ToBeConfirmed
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 90; // UnMatched
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[7].Width = 120; // LocalAmt
            dataGridView1.Columns[7].DefaultCellStyle = style;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[8].Width = 80; // Alerts
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 80; // Disputes
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 90; // OwnerId
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[11].Width = 150; // OwnerName
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[12].Width = 200; //StartManual
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[13].Width = 200; //EndManual
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        private void General_FormClosed(object sender, FormClosedEventArgs e)
        {
            //WFunction = "View";

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }


        void NForm271_FormClosed(object sender, FormClosedEventArgs e)
        {
            //int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80_Load(this, new EventArgs());

            //dataGridView1.Rows[WRowIndexLeft].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            //dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        void NForm52b_FormClosed(object sender, FormClosedEventArgs e)
        {
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        void NForm80b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //WFunction = "View";

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // Show Panel 
        private void ShowPanel(string InExternalAccNo, string InInternalAccNo, DateTime InWorkingDate)
        {
            // Read and Fill Table only the default 

           //RRDMNVStatement_Lines_InternalAndExternal Sec = new RRDMNVStatement_Lines_InternalAndExternal();
            RRDMNVCardsBothAuthorAndSettlement Sec = new RRDMNVCardsBothAuthorAndSettlement();
            int Mode = 1;
            //string ExternalAccno = "ALPHA67890";
            //string InternalAccNo = "HELLENIC_67890";

            labelInternalAcno.Text = "INTERNAL ACCOUNT :" + InExternalAccNo;
            labelExternalAcno.Text = "EXTERNAL ACCOUNT :" + InInternalAccNo;

            //InWorkingDate = new DateTime(2015, 02, 15);

            Sec.ReadNVStatements_LinesForTotals(WOperator, WSignedId, Mode,
                                                InExternalAccNo, InInternalAccNo, InWorkingDate);

            textBoxInternalAccBalance.Text = Sec.InternalAccBalance.ToString("#,##0.00");
            textBoxInternalAccTxns.Text = Sec.InternalAccTxns.ToString();

            textBoxUnMatchedInternalCR.Text = (-Sec.UnMatchedInternalCR).ToString("#,##0.00");
            textBoxUnMatchedInternalCRTxns.Text = Sec.UnMatchedInternalCRTxns.ToString();

            textBoxUnMatchedInternalDR.Text = Sec.UnMatchedInternalDR.ToString("#,##0.00");
            textBoxUnMatchedInternalDRTxns.Text = Sec.UnMatchedInternalDRTxns.ToString();

            textBoxUnMatchedExternalCREntries.Text = Sec.UnMatchedExternalCR.ToString("#,##0.00");
            textBoxUnMatchedExternalCRTxns.Text = Sec.UnMatchedExternalCRTxns.ToString();

            textBoxUnMatchedExternalDREntries.Text = (-Sec.UnMatchedExternalDR).ToString("#,##0.00");
            textBoxUnMatchedExternalDRTxns.Text = Sec.UnMatchedExternalDRTxns.ToString();

            int InternalUnMatched = Sec.UnMatchedInternalCRTxns + Sec.UnMatchedInternalDRTxns;
            int ExternalUnMatched = Sec.UnMatchedExternalCRTxns + Sec.UnMatchedExternalDRTxns;

            decimal Total = Sec.InternalAccBalance + Sec.UnMatchedInternalCR - Sec.UnMatchedInternalDR
                            + Sec.UnMatchedExternalCR - Sec.UnMatchedExternalDR;

            textBoxTotal.Text = Total.ToString("#,##0.00");

            textBoxExternalAccBalance.Text = Sec.ExternalAccBalance.ToString("#,##0.00");
            textBoxExternalAccTxns.Text = Sec.ExternalAccTxns.ToString();

            decimal Difference = Total + Sec.ExternalAccBalance;

            textBoxOpenAdjustmentsTxns.Text = Sec.MatchedAdjustmentsOpenTxns.ToString();
            textBoxOpenAdjustmentsAmtNegative.Text = Sec.MatchedAdjustmentsOpenNegative.ToString("#,##0.00");
            textBoxtextBoxOpenAdjustmentsAmtPositive.Text = Sec.MatchedAdjustmentsOpenPositive.ToString("#,##0.00");

            textBoxDifference.Text = Difference.ToString("#,##0.00");

            if (Difference != 0 || ExternalUnMatched != 0 || InternalUnMatched !=0)
            {
                pictureBoxBalStatus.BackgroundImage = appResImg.RED_LIGHT_Repl;

                Color Red = Color.Red;

                labelAlert.Show();
                if (Difference != 0) labelAlert.Text = "Bal Diff";
                if (ExternalUnMatched != 0 || InternalUnMatched != 0) labelAlert.Text = "Outstanding";
            }
            else
            {
                pictureBoxBalStatus.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
                labelAlert.Hide();
            }

            if (Sec.ToBeConfirmedTxns > 0)
            {
                labelToBeConfirmed.Show();
                textBoxToBeConfirmedEntries.Show();
                textBoxToBeConfirmedEntries.Text = Sec.ToBeConfirmedTxns.ToString();
            }
            else
            {

                labelToBeConfirmed.Hide();
                textBoxToBeConfirmedEntries.Hide();
            }

            textBoxOpenAdjustmentsTxns.Text = Sec.MatchedAdjustmentsOpenTxns.ToString();

            if (Sec.MatchedAdjustmentsOpenTxns > 0)
            {
                textBoxOpenAdjustmentsAmtNegative.Text = Sec.MatchedAdjustmentsOpenNegative.ToString("#,##0.00");
                textBoxtextBoxOpenAdjustmentsAmtPositive.Text = Sec.MatchedAdjustmentsOpenPositive.ToString("#,##0.00");
            }
            else
            {
                labelDr.Hide();
                labelCr.Hide();
                textBoxOpenAdjustmentsAmtNegative.Hide();
                textBoxtextBoxOpenAdjustmentsAmtPositive.Hide();
            }
        }

        // Finish 
        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Refresh 
        private void button1_Click_1(object sender, EventArgs e)
        {
            ShowPanel(Rcs.ExternalAccNo, Rcs.InternalAccNo, Rcs.StartManualDt);

        }
        // Help 
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            FormHelp helpForm = new FormHelp("Navigation Form");
            helpForm.ShowDialog();
        }
    }
}

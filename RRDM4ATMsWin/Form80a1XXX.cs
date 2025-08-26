using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80a1 : Form
    {

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMReconcCategATMsAtRMCycles Rca = new RRDMReconcCategATMsAtRMCycles();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        //RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WMainCateg;
        int WReconcCycleNo;

        int WRowIndexLeft;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;
        string WRMCategory;
        string WhatBankId;
        public Form80a1(string InSignedId, int SignRecordNo, string InOperator, string InFunction, string InRMCategory, string InWhatBankId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // Reconc , View

            WRMCategory = InRMCategory; // Specific or "ALL"
            WhatBankId = InWhatBankId;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            label1UserId.Text = WSignedId;

            if (WFunction == "Reconc")
            {
                textBoxMsgBoard.Text = "Go to Reconcile";

                buttonNext.Text = "Reconcile";

                panel4.Hide();

                buttonReconcPlayBack.Hide();

                labelStep1.Text = "UnMatched Txns Reconciliation";
                labelLeft.Text = "RECONCILIATION CATEGORIES";
                labelRight.Text = "TXNS RECONC DAILY CYCLES";

            }

            if (WFunction == "View")
            {
                textBoxMsgBoard.Text = "View Details of Reconciliation Cycles";

                labelStep1.Text = "Reconc Categories History";

                buttonNext.Text = "View";
            }
        }

        // ON LOAD

        private void Form80a1_Load(object sender, EventArgs e)
        {
            Rcs.ReadReconcUserCategoriesFillTable(WSignedId, WOperator);

            dataGridView1.DataSource = Rcs.TableReconcSessionsDistinct.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("No Entries available");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            dataGridView1.Columns[0].Width = 200; // Category Name
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[1].Width = 100; // CategoryId 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 80; // OutStanding
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[2].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 80; // OwnerUserID 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // Row Enter for Datagridview1
        string WCategoryId;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WCategoryId = rowSelected.Cells[1].Value.ToString();

            Rcs.ReadReconcCategoriesSessionsSpecificCat(WOperator, WCategoryId);

            dataGridView2.DataSource = Rcs.TableReconcSessionsPerCategory.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                labelRight.Hide();
                panel3.Hide();
                MessageBox.Show("No testing data for this category");
            }
            else
            {
                labelRight.Show();
                panel3.Show();
            }

            dataGridView2.Columns[0].Width = 70; // RunningJobNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridView2.Columns[1].Width = 70; // CategoryId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView2.Columns[1].Visible = false;

            dataGridView2.Columns[2].Width = 105; // StartDateTm
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[2].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[3].Width = 105; // EndDateTm 
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[3].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[4].Width = 70; // UnMatched 
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[5].Width = 70; // Pending
            dataGridView2.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[5].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[6].Width = 70; // Number of Files 
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[6].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView2.Columns[6].Visible = false;

            dataGridView2.Columns[7].Width = 70; // Matched 
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[7].DefaultCellStyle.ForeColor = Color.LightSlateGray;

        }
        // Row enter for Datagridview2
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WReconcCycleNo = (int)rowSelected.Cells[0].Value;

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);

            string WSelectionCriteria = " WHERE CategoryId ='" + WCategoryId + "'"
                                        + " AND RMCycle = " + WReconcCycleNo
                                        + " AND AtmGroup = " + Rcs.AtmGroup;
            Rca.ReadReconcCategoriesATMsRMCycleTotals(WOperator, WSelectionCriteria);

            labelATM1.Text = "ATMs Group.................: " + Rcs.AtmGroup.ToString();
            labelATM2.Text = "Number of Matched ATMs.....: " + Rca.TotalAtmsInGroupMatched.ToString();
            labelATM3.Text = "Number of UnMatched ATMs...: " + Rca.TotalAtmsInGroupUnMatched.ToString();

            labelUnMatch1.Text = "Number Of Matched Txns.....: " + Rca.TotalNumberOfMatchedRecs.ToString();
            labelUnMatch2.Text = "Number of UnMatched Txns...: " + Rca.TotalNumberOfUnMatchedRecs.ToString();
            labelUnMatch3.Text = "Amount for UnMatched.......: " + Rca.TotalUnMatchedAmt.ToString("#,##0.00");

            label3.Text = "DETAILS FOR CYCLE : " + WReconcCycleNo.ToString();

            //---------------------------------------------------------------------
            // RECONCILIATION 
            //---------------------------------------------------------------------
            if (Rcs.StartReconcDtTm == NullPastDate)
            {
                label20.Text = "Reconciliation didnt start";
                label19.Text = "Reconciliation didnt start";
                panel4.Hide();

            }
            else
            {
                if (Rcs.EndReconcDtTm == NullPastDate)
                {
                    panel4.Hide();
                    label20.Text = "Date Tm Start : " + Rcs.StartReconcDtTm.ToString();
                    label19.Text = "Reconciliation is in progess";
                }
                else
                {
                    panel4.Show();

                    label20.Text = "Date Tm Start.. : " + Rcs.StartReconcDtTm.ToString();
                    label19.Text = "Date Tm End.... : " + Rcs.EndReconcDtTm.ToString();

                    TimeSpan Remain2 = Rcs.EndReconcDtTm - Rcs.StartReconcDtTm;
                    label18.Text = "Duration in Minutes : " + Remain2.Minutes.ToString("#,##0.00");

                    label16.Text = "Remaining Exceptions : " + Rcs.RemainReconcExceptions.ToString();

                    if (Remain2.Minutes > 20)
                    {
                        pictureBox3.BackgroundImage = appResImg.YELLOW_Repl;
                    }

                    if (Remain2.Minutes > 20)
                    {
                        pictureBox3.BackgroundImage = appResImg.RED_LIGHT_Repl;

                        Color Red = Color.Red;

                        label18.ForeColor = Red;
                    }
                    else
                    {
                        Color Black = Color.Black;

                        label18.ForeColor = Black;
                    }

                    if (Remain2.Minutes < 20)
                    {
                        pictureBox3.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
                    }

                    if (Rcs.RemainReconcExceptions > 0)
                    {
                        pictureBox3.BackgroundImage = appResImg.RED_LIGHT_Repl;

                        Color Red = Color.Red;

                        label16.ForeColor = Red;
                    }
                    else
                    {
                        Color Black = Color.Black;

                        label16.ForeColor = Black;
                    }
                }
            }
        }
        // NEXT 
        private void buttonNext_Click(object sender, EventArgs e)
        {

            Ap.ReadAuthorizationsUserTotal(WSignedId); // User is requestor or Authoriser 
            if (Ap.RecordFound)
            {
                MessageBox.Show("Please take care with Authorisation record");
                return;
            }

            if (WFunction == "Reconc")
            {

                if (Rcs.RemainReconcExceptions == 0)
                {
                    MessageBox.Show("Not allowed operation. Already reconciled!");
                    return;
                }

                Us.ReadSignedActivityByKey(WSignRecordNo);

                Us.ProcessNo = 2;

                Us.WFieldChar1 = WCategoryId;
                Us.WFieldNumeric1 = WReconcCycleNo;

                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                int WAction = 11; // Reconciliation for Group of ATMs

                //MessageBox.Show(" YOU ARE NOT AUTHORIZED FOR MASS RECONCILIATION ");
                //return;
                Form271 NForm271;

                NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);
                NForm271.FormClosed += NForm271_FormClosed;
                NForm271.ShowDialog();
            }

            if (WFunction == "View")
            {
                Form80b NForm80b;

                WFunction = "View";

                NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WReconcCycleNo, "", 0, 0, WFunction, "");
                NForm80b.FormClosed += NForm80b_FormClosed;
                NForm80b.ShowDialog();
            }

        }

        void NForm271_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80a1_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        void NForm80b_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;
            WFunction = "View";

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80a1_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // Reconciliation Play Back 
        private void buttonReconcPlayBack_Click(object sender, EventArgs e)
        {
            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);
            if (Rcs.EndReconcDtTm == NullPastDate)
            {
                MessageBox.Show("Reconciliation Not done yet!");
                return;
            }

            // Update Us Process number
            Us.ReadSignedActivityByKey(WSignRecordNo);
            Us.ProcessNo = 54; // View Only 
            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form271 NForm271;

            NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);
            NForm271.FormClosed += NForm271_FormClosed;
            NForm271.ShowDialog(); ;


        }


        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80a2 : Form
    {

        RRDMReconcCategories Rc = new RRDMReconcCategories(); 

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        //RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WMainCateg;
        int WReconcCycleNo;

        int WGroup; 

        int WRowIndexLeft;

        string WCategoryId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;
        string WRMCategory;
        string WhatBankId;

        public Form80a2(string InSignedId, int SignRecordNo, string InOperator, string InFunction, string InRMCategory, string InWhatBankId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // Reconc , View

            WRMCategory = InRMCategory; // Specific or "ALL"
            WhatBankId = InWhatBankId;

            InitializeComponent();

            // Set Working Date 
           
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            label1UserId.Text = WSignedId;

            //ShowDetails(1116);

            if (WFunction == "VIEW")
            {
                textBoxMsgBoard.Text = "View Details of Reconciliation and Matching Cycles";

                labelStep1.Text = "RM Categories History";

                buttonNext.Text = "VIEW HIST";
            }
        }

        // ON LOAD
        private void Form80a2_Load(object sender, EventArgs e)
        {

            Rc.ReadReconcCategoriesAndFillTableWithDiscrepancies(WOperator, WSignedId, 0);

            dataGridView1.DataSource = Rc.TableReconcCateg.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("No Entries available for this user.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            dataGridView1.Columns[0].Width = 60; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 90; // Identity 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 140; // Category-Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 80; // Exceptions
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[3].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 80; // Origin 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // Row Enter for Datagridview1
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WCategoryId = rowSelected.Cells[1].Value.ToString();

            Rc.ReadReconcCategorybyCategId(WOperator, WCategoryId);

            WGroup = Rc.AtmGroup; 

            WMainCateg = WCategoryId.Substring(0, 4);

            Rcs.ReadReconcCategoriesSessionsSpecificCat_GL(WOperator, WCategoryId);

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

            ShowDetails(Rc.AtmGroup);

            dataGridView2.Columns[0].Width = 90; // RunningJobNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridView2.Columns[1].Width = 90; // Cut_Off_Date
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[2].Width = 70; // CategoryId
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[2].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView2.Columns[2].Visible = false;

            dataGridView2.Columns[3].Width = 120; // StartDateTm
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[3].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[4].Width = 120; // EndDateTm 
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[5].Width = 60; // Atms_GL_Diff 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[5].HeaderText = "ATMs In Diff";
            //dataGridView2.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[6].Width = 60; // Pending
            dataGridView2.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[6].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        // Row enter for Datagridview2
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WReconcCycleNo = (int)rowSelected.Cells[0].Value;

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);

            if (Rcs.GL_Original_Atms_Cash_Diff == 0) buttonNext.Enabled = false;
                                             else buttonNext.Enabled = true;

            labelDetails.Text = "PENDINGS FOR THIS GROUP:.." + Rc.AtmGroup;

            //---------------------------------------------------------------------
            // RECONCILIATION 
            //---------------------------------------------------------------------
            if (Rcs.GL_StartReconcDtTm == NullPastDate)
            {
                labelDateStart.Text = "Reconciliation didnt start";
                labelDateEnd.Text = "Reconciliation didnt start";
                panel4.Hide();

            }
            else
            {
                if (Rcs.GL_EndReconcDtTm == NullPastDate)
                {
                    panel4.Hide();
                    labelDateStart.Text = "Date Start : " + Rcs.GL_StartReconcDtTm.ToString();
                    labelDateEnd.Text = "Reconciliation is in progess";
                }
                else
                {
                    panel4.Show();

                    labelDateStart.Text = "Date Start : " + Rcs.GL_StartReconcDtTm.ToString();
                    labelDateEnd.Text = "Date End  : " + Rcs.GL_EndReconcDtTm.ToString();

                    TimeSpan Remain2 = Rcs.GL_EndReconcDtTm - Rcs.GL_StartReconcDtTm;
                    label18.Text = "Duration in Minutes : " + Remain2.Minutes.ToString("#,##0.00");

                    //label16.Text = "Remaining Exceptions : " + Rcs.RemainReconcExceptions.ToString();

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

                    //if (Rcs.RemainReconcExceptions > 0)
                    //{
                    //    pictureBox3.BackgroundImage = appResImg.RED_LIGHT_Repl;

                    //    Color Red = Color.Red;

                    //    label16.ForeColor = Red;
                    //}
                    //else
                    //{
                    //    Color Black = Color.Black;

                    //    label16.ForeColor = Black;
                    //}
                }
            }
        }

        // SHOW DETAILS 
        private void ShowDetails(int InAtmsReconcGroup)
        {
            //if (WFunction == "Reconc")
            //{
            if (WFunction == "Reconc")
            {
              textBoxMsgBoard.Text = "Go to Reconcile";
            }
            else
            {
                textBoxMsgBoard.Text = "View History"; 
            }
            

            //buttonNext.Text = "Reconcile";

            panel4.Hide();

            button1.Hide();

            labelStep1.Text = "ATMs Cash Reconciliation";
            labelLeft.Text = "ATM CASH CATEGORY";
            labelRight.Text = "ATMs CASH RECONC DAILY CYCLES";

            Tuc.ReadTotalsForATMsCashReconciliation(WSignedId, WSignRecordNo, WOperator, InAtmsReconcGroup);

            labelATM1.Text = "Ready for Reconciliation...: " + Tuc.TotalATMsReady.ToString();
            labelATM2.Text = "Total Exceptions...........: " + Tuc.TotalErrorsAtATMs.ToString();
            labelATM3.Text = "Total Amount for Exceptions: " + Tuc.TotalAmountErrors.ToString("#,##0.00");

            labelUnMatch1.Text = "ATMs with UnMatched........: " + Tuc.TotalAtmsWithUnMatchedRecords.ToString();
            labelUnMatch2.Text = "UnMatched Records..........: " + Tuc.TotalUnMatchedRecords.ToString();
            labelUnMatch3.Text = "Amount for UnMatched.......: " + Tuc.TotalAmountUnMatched.ToString("#,##0.00");

            if (Tuc.TotalUnMatchedRecords > 0)
            {
                pictureBox2.BackgroundImage = appResImg.RED_LIGHT_Repl;

                Color Red = Color.Red;

                label22.ForeColor = Red;

                label22.Show();

                pictureBox2.Show();
            }
            else
            {
                label22.Hide();

                pictureBox2.Hide();
            }

        //}
            
        }
        // NEXT 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            bool WViewHistory = false;
            if (WFunction == "VIEW")
            {
                WViewHistory = true;
            }
            else
            {
                WViewHistory = false;
            }

            if (Tuc.TotalATMsReady == 0 & WViewHistory == false)
            {
                MessageBox.Show("No Atms ready to reconcile");
                return;
            } 

            Ap.ReadAuthorizationsUserTotal(WSignedId); // User is requestor or Authoriser 
            if (Ap.RecordFound)
            {
                MessageBox.Show("Please take care with Authorisation record");
                return;
            }
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (WFunction == "VIEW")
            {
                Usi.ProcessNo = 54;
            }
            else
            {
                Usi.ProcessNo = 5;
            }
                
            Usi.WFieldChar1 = WCategoryId;
            Usi.WFieldNumeric1 = WReconcCycleNo;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            int WAction = 11; // Reconciliation for Group of ATMs

            //MessageBox.Show(" YOU ARE NOT AUTHORIZED FOR MASS RECONCILIATION ");
            //return;
            if (WhatBankId == "")
            {
                //Form52b NForm52b;

                //NForm52b = new Form52b(WSignedId, WSignRecordNo, WOperator, WAction, WCategoryId, WReconcCycleNo);
                //NForm52b.FormClosed += NForm52b_FormClosed;
                //NForm52b.ShowDialog();
            }

            if (WhatBankId == "TEST")
            {
                MessageBox.Show("ALL ATMs in Difference for this category will be reconciled " + Environment.NewLine
                                + "For:" + Environment.NewLine
                                + "General Ledger Reconciliation" + Environment.NewLine
                                + "Presenter Errors Handling."
                                );
                           
                Form52c NForm52c;
               
                NForm52c = new Form52c(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo, WViewHistory);
                NForm52c.FormClosed += NForm52b_FormClosed;
                NForm52c.ShowDialog();
            }

        }

        void NForm52b_FormClosed(object sender, FormClosedEventArgs e)
        {
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80a2_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // Show ATMs Cash Status 
        private void buttonCashStatus_Click(object sender, EventArgs e)
        {
            Form68_Atms_Main NForm68_Atms_Main;
            int TempGroup = WGroup; // If this Zero and Mode = 2 the it shows all ATMs
            int TempMode = 10; // ALL ATMs this Group
            NForm68_Atms_Main = new Form68_Atms_Main(WSignedId, WSignRecordNo, WOperator, TempGroup, TempMode);
            NForm68_Atms_Main.Show();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}

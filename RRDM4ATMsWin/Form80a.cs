using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form80a : Form
    {

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX(); 

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReconcCategATMsAtRMCycles RATMs = new RRDMReconcCategATMsAtRMCycles(); 

        RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WBankId; 

        string W4DigitMainCateg; 
       
        int  WReconcCycleNo;
        
        int WRowIndexLeft;

        string WCategoryId ; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;
        string WReconcCategory;

        string WhatBankId; 

        public Form80a(string InSignedId, int SignRecordNo, string InOperator, 
            string InFunction, string InReconcCategory, string InWhatBankId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // Reconc , View

            WReconcCategory = InReconcCategory; // Specific or "ALL"

            WhatBankId = InWhatBankId; 

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            label1UserId.Text = WSignedId;

            Us.ReadUsersRecord(InSignedId);
            WBankId = Us.BankId; 

            if (WFunction == "Reconc")
            { 
                textBoxMsgBoard.Text = "Go to Reconcile";

                buttonNext.Text = "Reconcile";

                labelStep1.Text = "ATMs/Cards Trans Reconciliation"; 

                panel4.Hide();
          
                button1.Hide();

                button6.Hide(); 
            }

            if (WFunction == "View")
            {
                textBoxMsgBoard.Text = "View Details of Reconciliation and Matching Cycles";

                labelStep1.Text = "ATMs/Cards Trans Reconciliation History";

                //buttonNext.Hide();
                buttonNext.Text = "View"; 
                button6.Hide(); 

            }

            if (WFunction == "Interactive A")
            {
                textBoxMsgBoard.Text = "Investigate UnMatched/Matched Records and take actions.";

                labelStep1.Text = "Investigation for possible Actions on UnMatched/Matched Records.";

                buttonNext.Text = "Actions";

                button6.Hide();
                button1.Hide(); 

            }

            if (WFunction == "Interactive B")
            {
                textBoxMsgBoard.Text = "Investigate UnMatched/Matched Records For Settlement.";

                labelStep1.Text = "Investigation for Settlement UnMatched Records.";

                buttonNext.Text = "Actions";

                button6.Hide();
                button1.Hide(); 
            }
            if (WOperator == "ITMX")
            {
                labelStep1.Text = "Reconciliation Of Categories with Exceptions";
                labelLeft.Text = "CATEGORIES";
                labelRight.Text = "RECONCILIATION CYCLES"; 
                textBoxMsgBoard.Text = "Select Category and Cycle and  to Reconcile! ";


                label1.Hide(); 
                dataGridView3.Hide();

                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Details of Reconciliation and Matching Cycles";

                    labelStep1.Text = "Transactions Reconciliation History and Trail ";

                    //buttonNext.Hide();

                    //buttonNext.Hide();
                    buttonNext.Text = "View";
                    button6.Hide();

                }
            }
        }
// ON LOAD
        private void Form80_Load(object sender, EventArgs e)
        {

            if (WFunction == "View" & WhatBankId == "ITMX")
            {
                Rcs.ReadReconcCategoriesDistinctFillTable(WOperator, 2, WhatBankId, "");
            }
            if (WFunction == "View" & WhatBankId != "ITMX")
            {
                Rcs.ReadReconcCategoriesDistinctFillTable(WOperator, 2, WBankId, "");
            }
            if (WFunction == "Reconc" & WOperator != "ITMX")
            {
                Rcs.ReadReconcCategoriesDistinctFillTable(WOperator, 1, WhatBankId, "");
            }

            if (WFunction == "Reconc" & WOperator == "ITMX")
            {
                Rcs.ReadReconcCategoriesDistinctFillTable(WOperator, 2, WhatBankId, "");
            }

            dataGridView1.DataSource = Rcs.TableReconcSessionsDistinct.DefaultView;

            if (dataGridView1.Rows.Count == 0 )
            {
                Form2 MessageForm = new Form2("No Entries availble");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }
   
            dataGridView1.Columns[0].Width = 120; // Category Name
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[1].Width = 100; // id 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 80; // id 
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[2].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 80; // id 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // ON ROW ENTER CATEGORIES
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WCategoryId = rowSelected.Cells[1].Value.ToString();

            W4DigitMainCateg = WCategoryId.Substring(0, 4);

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);

            //TEST
            if (Mc.Origin == "Our Atms Matching")
            {
                //SHOW ATMS 
                label1.Show();
                dataGridView3.Show();
            }
            else
            {
                label1.Hide();
                dataGridView3.Hide();
            }

            Rcs.ReadReconcCategoriesSessionsSpecificCat(WOperator,WSignedId ,WCategoryId,0);
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

            dataGridView2.Columns[1].Width = 70; // CutOff
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // dataGridView2.Columns[1].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView2.Columns[1].Visible = true;

            dataGridView2.Columns[2].Width = 70; // CategoryId
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[2].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView2.Columns[2].Visible = true;

            dataGridView2.Columns[3].Width = 105; // StartDateTm
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[3].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[4].Width = 105; // EndDateTm 
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[5].Width = 70; // UnMatched 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[5].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[6].Width = 70; // Pending
            dataGridView2.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[6].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[7].Width = 70; // Number of Files 
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[7].DefaultCellStyle.ForeColor = Color.LightSlateGray;
            dataGridView2.Columns[7].Visible = true;

            dataGridView2.Columns[8].Width = 70; // Matched 
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[8].DefaultCellStyle.ForeColor = Color.LightSlateGray;

        }

        // On Row Enter for Matching and reconciliations Cycles 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WReconcCycleNo = (int)rowSelected.Cells[0].Value;

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);

            label3.Text = "DETAILS FOR CYCLE : " + WReconcCycleNo.ToString();

//---------------------------------------------------------------------
            // MATCHING  
//---------------------------------------------------------------------

            labelDateStart.Text = "Date Start : " + Rcs.StartDateTm.ToString();
            label12.Text =        "Date End  : " + Rcs.EndDateTm.ToString();
            label21.Text = "Records Matched : " + Rcs.NumberOfMatchedRecs.ToString("#,##0");

            TimeSpan Remain1 = Rcs.EndDateTm - Rcs.StartDateTm;
            label13.Text = "Time Duration in Minutes : " + Remain1.Minutes.ToString("#,##0.00");

            label15.Text = "Exceptions : " + Rcs.NumberOfUnMatchedRecs.ToString(); 

            if (Remain1.Minutes > 25)
            {
                pictureBox2.BackgroundImage = appResImg.YELLOW_Repl;
            }

            if (Remain1.Minutes > 30)
            {
                pictureBox2.BackgroundImage = appResImg.RED_LIGHT_Repl;

                Color Red = Color.Red;

                label13.ForeColor = Red;
            }
            else
            {
                Color Black = Color.Black;

                label13.ForeColor = Black;
            }

            if (Remain1.Minutes < 25)
            {
                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }
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
                    label20.Text = "Date Start : " + Rcs.StartReconcDtTm.ToString();
                    label19.Text = "Reconciliation is in progess";
                }
                else
                {
                    panel4.Show();

                    label20.Text = "Date Start : " + Rcs.StartReconcDtTm.ToString();
                    label19.Text = "Date End  : " + Rcs.EndReconcDtTm.ToString();

                    TimeSpan Remain2 = Rcs.EndReconcDtTm - Rcs.StartReconcDtTm;
                    label18.Text = "Duration in Minutes : " + Remain2.Minutes.ToString("#,##0.00");
                    Mp.ReadMatchingTxnsMasterPoolForTotalsForUnMatched(WOperator, WCategoryId, WReconcCycleNo); 
                    //Rm.ReadMatchedORUnMatchedFileForTotals(WOperator, WCategoryId, WRMCycleNo);

                    label16.Text = "Remaining Exceptions : " + Rcs.RemainReconcExceptions.ToString(); 

                    //if (Remain2.Minutes > 20)
                    //{
                    //    pictureBox3.BackgroundImage = Properties.Resources.YELLOW_Repl;
                    //}

                    if (Rcs.RemainReconcExceptions > 0)
                    {
                        pictureBox3.BackgroundImage = appResImg.RED_LIGHT_Repl;

                        Color Red = Color.Red;

                        label18.ForeColor = Red;

                        label16.ForeColor = Red;
                    }
                    else
                    {
                        Color Black = Color.Black;

                        label18.ForeColor = Black;

                        label16.ForeColor = Black;
                    }

                    if (Rcs.RemainReconcExceptions == 0)
                    {
                        pictureBox3.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
                    }
                }
               
            }
            if(WOperator != "ITMX")
            {
                int Mode = 1; // Read only 
                RATMs.ReadReconcCategoriesATMsRMCycleToFillTable(WOperator, WCategoryId, WReconcCycleNo, Mode);
                dataGridView3.DataSource = RATMs.TableRMATMsCycles.DefaultView;

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";         

                dataGridView3.Columns[0].Width = 60; // AtmNo
                dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView3.Columns[1].Width = 70; // OpeningBalance
                dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView3.Columns[2].Width = 90; // JournalAmt
                dataGridView3.Columns[2].DefaultCellStyle = style;
                dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
               
                dataGridView3.Columns[3].Width = 135; // MatchedAmtAtMatching
                dataGridView3.Columns[3].DefaultCellStyle = style;
                dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView3.Columns[4].Width = 135; // MatchedAmtAtDefault
                dataGridView3.Columns[4].DefaultCellStyle = style;
                dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                //TableRMATMsCycles.Columns.Add("AtmNo", typeof(string));
                //TableRMATMsCycles.Columns.Add("OpeningBalance", typeof(decimal));
                //TableRMATMsCycles.Columns.Add("JournalAmt", typeof(decimal));
                //TableRMATMsCycles.Columns.Add("MatchedAmtAtMatching", typeof(decimal));
                //TableRMATMsCycles.Columns.Add("MatchedAmtAtDefault", typeof(decimal));
                //TableRMATMsCycles.Columns.Add("MatchedAmtAtWorkFlow", typeof(decimal));
                //TableRMATMsCycles.Columns.Add("Difference", typeof(decimal));
                //TableRMATMsCycles.Columns.Add("CategoryId", typeof(string));
                //TableRMATMsCycles.Columns.Add("RMCycle", typeof(int));
            }
            
        }

// On ROW ENTER 
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

         // .......................
        }

// Go to Reconciliation or other functionality 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

                if (WFunction == "Reconc")
                {

                    Ap.ReadAuthorizationsUserTotal(WSignedId); // User is requestor or Authoriser 
                    if (Ap.RecordFound)
                    {
                        MessageBox.Show("Please take care with Authorisation record");
                        return;
                    }

                    if (Rcs.RemainReconcExceptions == 0 & Rcs.StartReconcDtTm != NullPastDate & Rcs.EndReconcDtTm != NullPastDate)
                    {
                        MessageBox.Show("No Exceptions to Reconcile!");
                        return;
                    }

                // Update Us Process number
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                    Usi.ProcessNo = 2; // Reconciliation 
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    if (WOperator == "ITMX")
                    {
                        Form281 NForm281;

                        NForm281 = new Form281(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);
                        NForm281.FormClosed += NForm281_FormClosed; ;
                        NForm281.ShowDialog(); ;
                    }

                    if (WOperator == "CRBAGRAA")
                    {
                        Form271 NForm271;

                        NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);
                        NForm271.FormClosed += NForm271_FormClosed;
                        NForm271.ShowDialog(); ;
                    }
                }

                if (WFunction == "Interactive A")
                {
                    Form80b NForm80b;
                    NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "",WCategoryId, WReconcCycleNo, "", 0 ,0,WFunction, "", 0);
                NForm80b.ShowDialog();
                }

                //if (WFunction == "Interactive B") 
                //{
                //    // Update Us Process number
                
                //    Form19b NForm19b;

                //    int Actions = 1;

                //    NForm19b = new Form19b(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo, Actions);
                //    NForm19b.ShowDialog(); ;
                //}
                if (WFunction == "View" & WOperator != "ITMX")
                {
                    WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

                    Form80b NForm80b;

                    WFunction = "View";

                    NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WReconcCycleNo,"", 0, 0, WFunction,"", 0);
                    NForm80b.FormClosed += NForm80b_FormClosed;
                    NForm80b.ShowDialog();
                }
                if (WFunction == "View" & WOperator == "ITMX")
                {
                    WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

                    Form80bΙΤΜΧ NForm80bΙΤΜΧ;

                    WFunction = "View";

                    NForm80bΙΤΜΧ = new Form80bΙΤΜΧ(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo, 0, WFunction);
                    NForm80bΙΤΜΧ.FormClosed += NForm80bΙΤΜΧ_FormClosed; 
                    NForm80bΙΤΜΧ.ShowDialog();
                }           
        }

        private void NForm80bΙΤΜΧ_FormClosed(object sender, FormClosedEventArgs e)
        {
            WFunction = "View";

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        private void NForm281_FormClosed(object sender, FormClosedEventArgs e)
        {
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

 // See the Files 
        private void button6_Click(object sender, EventArgs e)
        {
          
            if (WOperator != "ITMX")
            {
                WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

                Form80b NForm80b;

                WFunction = "View";

                NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WReconcCycleNo, "", 0, 0, WFunction, "", 0);
                NForm80b.FormClosed += NForm80b_FormClosed;
                NForm80b.ShowDialog();
            }
           
            if (WOperator == "ITMX")
            {
                WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

                Form80bΙΤΜΧ NForm80bΙΤΜΧ;

                WFunction = "View";

                NForm80bΙΤΜΧ = new Form80bΙΤΜΧ(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo, 0, WFunction);
                NForm80bΙΤΜΧ.FormClosed += NForm80b_FormClosed;
                NForm80bΙΤΜΧ.ShowDialog();
            }   
        }

        void NForm80b_FormClosed(object sender, FormClosedEventArgs e)
        {
            WFunction = "View"; 

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // PlayBack Reconciliation 
        private void button1_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Ap.ReadAuthorizationsUserTotal(WSignedId); // User is requestor or Authoriser 
            if (Ap.RecordFound)
            {
                MessageBox.Show("Please take care with Authorisation record");
                return;
            }

            if (Rcs.StartReconcDtTm == NullPastDate || Rcs.EndReconcDtTm == NullPastDate)
            {
                MessageBox.Show("Reconcilition Outstanding");
                return;
            }

            if (WOperator == "ITMX")
            {

                //if (Rcs.StartReconcDtTm == NullPastDate)
                //{
                //    MessageBox.Show("Reconciliation not done yet");
                //    return;
                //}

                // Update Us Process number
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // Reconciliation view 
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Form281 NForm281;

                NForm281 = new Form281(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);

                NForm281.ShowDialog();

                //MessageBox.Show("No Reconciliation for this type of transaction");
                return;
            }

            if (WCategoryId == "EWB110")
            {
                MessageBox.Show("Reconcilition PlayBack for this Category" + Environment.NewLine
                                       + "Will be found in My ATMs Button." + Environment.NewLine
                                        + "Reconciliation playback is shown " + Environment.NewLine
                                         + "Per ATM and per Reconciliation Cycle "
                                       );
                return;


            }
            if (WOperator == "CRBAGRAA" & (Mc.Origin == "Our Atms Matching" || Mc.Origin == "BancNet"))
            {
                // Update Us Process number
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // View Only 
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Form271 NForm271;

                NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);
                NForm271.FormClosed += NForm271_FormClosed;
                NForm271.ShowDialog(); ;      
            }

           
        }

// Finish 
        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

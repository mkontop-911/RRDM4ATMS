using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80aITMX : Form
    {
        RRDMMatchingCategories Rc = new RRDMMatchingCategories();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions(); 

        RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        //RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 

        RRDMReconcCategATMsAtRMCycles RATMs = new RRDMReconcCategATMsAtRMCycles(); 

        RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string W4DigitMainCateg; 
       
        int WRMCycleNo;

        int WRowIndexLeft;

        string WCategoryId ; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;
        string WRMCategory; 

        public Form80aITMX(string InSignedId, int SignRecordNo, string InOperator, string InFunction, string InRMCategory)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // Reconc , View

            WRMCategory = InRMCategory; // Specific or "ALL"

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            label1UserId.Text = WSignedId;

          
            if (WFunction == "Reconc")
            { 
                textBoxMsgBoard.Text = "Go to Reconcile";

                buttonNext.Text = "Reconcile";

                labelStep1.Text = "ATMs/Cards Trans Reconciliation"; 

                panel4.Hide();
                //panel6.Hide();
                //button6.Hide();
                button1.Hide();

                if (InRMCategory =="EWB110")
                {
                    labelStep1.Text = "ATMs Cash Reconciliation";
                    labelLeft.Text = "ATM CASH CATEGORY";
                    labelRight.Text = "ATM CASH CATEGORY DAILY CYCLES";

                    Tuc.ReadTotalsForATMsCashReconciliation(WSignedId, WSignRecordNo, WOperator, 1116);

                    //TotalATMsReady = 0; // Replenishment Done and no UnMatched Records
                    //TotalATMsWithUnMatched = 0; // Replenishment Done but there are UnMatched Records

                    //TotalErrorsAtATMs = 0; // Total Errors to be handle 

                    //TotalAmountErrors = 0; // Total Errors amount

                    //TotalAmountUnMatched = 0; // Total  

                    //TotalUnMatchedAtms = 0;
                    //TotalUnMatchedRecords = 0;

                    label14.Text = Tuc.TotalATMsReady.ToString();
                    label14.Text = Tuc.TotalErrorsAtATMs.ToString();
                    label14.Text = Tuc.TotalAmountErrors.ToString();

                    label14.Text = Tuc.TotalAtmsWithUnMatchedRecords.ToString();
                    label14.Text = Tuc.TotalUnMatchedRecords.ToString();
                    label14.Text = Tuc.TotalAmountUnMatched.ToString();
                }

                button6.Hide(); 
            }

            if (WFunction == "View")
            {
                textBoxMsgBoard.Text = "View Details of Reconciliation and Matching Cycles";

                labelStep1.Text = "ATMs/Cards Trans Reconciliation History"; 
             

                buttonNext.Hide(); 

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

        }
// ON LOAD
        private void Form80_Load(object sender, EventArgs e)
        {
            
               //Rc.ReadReconcCategories(WOperator, WOrigin);         

            dataGridView1.DataSource = Rc.TableMatchingCateg.DefaultView;

            dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 60; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; // id 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Red; 

            dataGridView1.Columns[2].Width = 250; // Name

        }

        // ON ROW ENTER CATEGORIES
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WCategoryId = rowSelected.Cells[1].Value.ToString();

            W4DigitMainCateg = WCategoryId.Substring(0, 4);

            Rms.ReadMatchingCategoriesSessionsSpecificCatFillTable(WOperator, WCategoryId);

            dataGridView2.DataSource = Rms.TableMatchingSessionsPerCategory.DefaultView;

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

            dataGridView2.Columns[0].Width = 90; // 
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridView2.Columns[1].Width = 70; // 
            dataGridView2.Columns[1].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[2].Width = 100; // 
            dataGridView2.Columns[2].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[3].Width = 100; // 
            dataGridView2.Columns[3].DefaultCellStyle.ForeColor = Color.LightSlateGray;

        }

        // On Row Enter for Matching and reconciliations Cycles 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WRMCycleNo = (int)rowSelected.Cells[0].Value;
            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WRMCycleNo);
         
            label3.Text = "DETAILS FOR CYCLE : " + WRMCycleNo.ToString();

//---------------------------------------------------------------------
            // MATCHING  
//---------------------------------------------------------------------

            labelDateStart.Text = "Date Start : " + Rms.StartDateTm.ToString();
            label12.Text =        "Date End  : " + Rms.EndDateTm.ToString();
            label21.Text = "Records Matched : " + Rms.NumberOfMatchedRecs.ToString("#,##0");

            TimeSpan Remain1 = Rms.EndDateTm - Rms.StartDateTm;
            label13.Text = "Time Duration in Minutes : " + Remain1.Minutes.ToString("#,##0.00");

            label15.Text = "Exceptions : " + Rms.NumberOfUnMatchedRecs.ToString(); 

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


                    string SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                                                   + WCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo + " AND IsMatchingDone = 1 ";

                    Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria,1);

                    label16.Text = "Remaining Exceptions : " + Mpa.TotalNotSettled.ToString();

                    if (Mpa.TotalNotSettled > 0)
                    {
                        label2.Text = "      Amount :" + Mpa.TotalNotSettledAmt.ToString("#,##0.00");
                    }
                    else
                    {
                        label2.Hide(); 
                    }


                    //if (Remain2.Minutes > 20)
                    //{
                    //    pictureBox3.BackgroundImage = appResImg.YELLOW_Repl;
                    //}

                    if (Mpa.TotalNotSettled > 0)
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

                    if (Mpa.TotalNotSettled == 0)
                    {
                        pictureBox3.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
                    }

                }
               
            }
            int Mode = 1; // Read only 
            RATMs.ReadReconcCategoriesATMsRMCycleToFillTable(WOperator, WCategoryId, WRMCycleNo, Mode);
            dataGridView3.DataSource = RATMs.TableRMATMsCycles.DefaultView;

            dataGridView3.Columns[0].Width = 40; // AtmNo
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[1].Width = 85; // OpeningBalance
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;     

            dataGridView3.Columns[2].Width = 90; //  TotalJournalAmt
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[3].Width = 90; // TotalMatchedAmt
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[4].Width = 90; // MatchedAmtAdj
            dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[5].Width = 85; // Difference
            dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView3.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView3.Columns[6].Width = 50; // CategoryId

            dataGridView3.Columns[7].Width = 50; // RMCycle

            //TableRMATMsCycles.Columns.Add("AtmNo", typeof(string));
            //TableRMATMsCycles.Columns.Add("OpeningBalance", typeof(decimal));
            //TableRMATMsCycles.Columns.Add("TotalJournalAmt", typeof(decimal));
            //TableRMATMsCycles.Columns.Add("TotalMatchedAmt", typeof(decimal));
            //TableRMATMsCycles.Columns.Add("MatchedAmtAdj", typeof(decimal));
            //TableRMATMsCycles.Columns.Add("Difference", typeof(decimal));
            //TableRMATMsCycles.Columns.Add("CategoryId", typeof(string));
            //TableRMATMsCycles.Columns.Add("RMCycle", typeof(int));


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

            if (WCategoryId == "EWB110")
            {
                Ap.ReadAuthorizationsUserTotal(WSignedId); // User is requestor or Authoriser 
                if (Ap.RecordFound)
                {
                    MessageBox.Show("Please take care with Authorisation record");
                    return; 
                }
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ProcessNo = 5;

                Usi.WFieldChar1 = WCategoryId;
                Usi.WFieldNumeric1 = WRMCycleNo; 

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                int WAction = 11; // Reconciliation for Group of ATMs

                //MessageBox.Show(" YOU ARE NOT AUTHORIZED FOR MASS RECONCILIATION ");
                //return;
                //Form52b NForm52b;

                ////NForm52b = new Form52b_XXXX(WSignedId, WSignRecordNo, WOperator, WAction, WCategoryId, WRMCycleNo);
                //NForm52b.FormClosed += NForm52b_FormClosed;
                //NForm52b.ShowDialog(); ;      
            }
            else
            {
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
                    //TEST
                    if (WCategoryId == "EWB311")
                    {
                        WRMCycleNo = 106; 
                    }

                    // Update Us Process number
                    RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);
                    Usi.ProcessNo = 2; // Reconciliation 
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                    Form271 NForm271;

                    NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycleNo);
                    NForm271.FormClosed += NForm271_FormClosed;
                    NForm271.ShowDialog(); ;      
                }

                if (WFunction == "Interactive A")
                {
                    Form80b NForm80b;
                    NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WRMCycleNo, "", 0, 0, WFunction, "", 0);
                    NForm80b.ShowDialog();
                }

                if (WFunction == "Interactive B") 
                {
                    // Update Us Process number
                
                    //Form19b NForm19b;

                    //int Actions = 1;

                    //NForm19b = new Form19b(WSignedId, WSignRecordNo, WOperator, WCategoryId,WRMCycleNo, Actions);
                    //NForm19b.ShowDialog(); ;
                }            
                           
            }    
        }

        void NForm271_FormClosed(object sender, FormClosedEventArgs e)
        {
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
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
            ////if (WCategoryId != "EWB311" || WCategoryId != "EWB511")
            ////{
            ////    MessageBox.Show ("No Testing data for this RM category Select EWB311");
            ////    return; 
            ////}
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b NForm80b;

            WFunction = "View"; 

            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WRMCycleNo, "", 0, 0, WFunction, "", 0);
            NForm80b.FormClosed += NForm80b_FormClosed;
            NForm80b.ShowDialog();
        }

        void NForm80b_FormClosed(object sender, FormClosedEventArgs e)
        {
            WFunction = "Reconc"; 

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

            if (WCategoryId == "EWB110")
            {
                MessageBox.Show("Reconcilition PlayBack for this Category" + Environment.NewLine
                                       + "Will be found in My ATMs Button." + Environment.NewLine
                                        + "Reconciliation playback is shown " + Environment.NewLine
                                         + "Per ATM and per Reconciliation Cycle " 
                                       );
                return;

            }
            if ((W4DigitMainCateg == "EWB1" & WCategoryId != "EWB110") || W4DigitMainCateg == "EWB3")
            {
                // Update Us Process number
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // View Only 
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Form271 NForm271;

                NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycleNo);
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

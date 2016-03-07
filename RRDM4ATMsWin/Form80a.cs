using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos
using System.Configuration;
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form80a : Form
    {
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();

        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass();

        RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WMainCateg; 
       
        int WRMCycleNo;

        int WRowIndexLeft;

        string WCategoryId ; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;
        string WRMCategory; 

        public Form80a(string InSignedId, int SignRecordNo, string InOperator, string InFunction, string InRMCategory)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // Reconc , View

            WRMCategory = InRMCategory; // Specific or "ALL"

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
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

                    Tuc.ReadTotalsForATMsCashReconciliation(WSignedId, WSignRecordNo, WOperator);

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
            string WOrigin = "ALL"; 
          
               Rc.ReadReconcCategories(WOperator,WOrigin, WRMCategory);
           
           

            dataGridView1.DataSource = Rc.ReconcCateg.DefaultView;

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

            WMainCateg = WCategoryId.Substring(0, 4);

            Rms.ReadReconcCategoriesMatchingSessionsSpecificCat(WOperator, WCategoryId);

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

            dataGridView2.Columns[0].Width = 70; // 
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

            Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycleNo);

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
                pictureBox2.BackgroundImage = Properties.Resources.YELLOW_Repl;
            }

            if (Remain1.Minutes > 30)
            {
                pictureBox2.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;

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
                pictureBox2.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl;
            }
 //---------------------------------------------------------------------
            // RECONCILIATION 
 //---------------------------------------------------------------------
            if (Rms.StartReconcDtTm == NullPastDate)
            {
                label20.Text = "Reconciliation didnt start";
                label19.Text = "Reconciliation didnt start";
                panel4.Hide(); 
           
            }
            else
            {
                if (Rms.EndReconcDtTm == NullPastDate)
                {
                    panel4.Hide();
                    label20.Text = "Date Start : " + Rms.StartReconcDtTm.ToString();
                    label19.Text = "Reconciliation is in progess";
                }
                else
                {
                    panel4.Show();

                    label20.Text = "Date Start : " + Rms.StartReconcDtTm.ToString();
                    label19.Text = "Date End  : " + Rms.EndReconcDtTm.ToString();

                    TimeSpan Remain2 = Rms.EndReconcDtTm - Rms.StartReconcDtTm;
                    label18.Text = "Duration in Minutes : " + Remain2.Minutes.ToString("#,##0.00");

                    Rm.ReadMatchedORUnMatchedFileForTotals(WOperator, WCategoryId, WRMCycleNo);

                    label16.Text = "Remaining Exceptions : " + Rm.TotalUnMatched.ToString(); 

                    //if (Remain2.Minutes > 20)
                    //{
                    //    pictureBox3.BackgroundImage = Properties.Resources.YELLOW_Repl;
                    //}

                    if (Rm.TotalUnMatched > 0)
                    {
                        pictureBox3.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;

                        Color Red = Color.Red;

                        label18.ForeColor = Red;
                    }
                    else
                    {
                        Color Black = Color.Black;

                        label18.ForeColor = Black;
                    }

                    if (Rm.TotalUnMatched == 0)
                    {
                        pictureBox3.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl;
                    }

                    if (Rms.RemainReconcExceptions > 0)
                    {
                        pictureBox3.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;

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

                Us.ReadSignedActivityByKey(WSignRecordNo);

                Us.ProcessNo = 5;

                Us.WFieldChar1 = WCategoryId;
                Us.WFieldNumeric1 = WRMCycleNo; 

                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                int WAction = 11; // Reconciliation for Group of ATMs

                //MessageBox.Show(" YOU ARE NOT AUTHORIZED FOR MASS RECONCILIATION ");
                //return;
                Form52b NForm52b;

                NForm52b = new Form52b(WSignedId, WSignRecordNo, WOperator, WAction);
                NForm52b.FormClosed += NForm52b_FormClosed;
                NForm52b.ShowDialog(); ;      
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

                    if (Rms.RemainReconcExceptions == 0 & Rms.StartReconcDtTm != NullPastDate & Rms.EndReconcDtTm != NullPastDate)
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
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ProcessNo = 2; // Reconciliation 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                    Form271 NForm271;

                    NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycleNo);
                    NForm271.FormClosed += NForm271_FormClosed;
                    NForm271.ShowDialog(); ;      
                }

                if (WFunction == "Interactive A")
                {
                    Form80b NForm80b;
                    NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycleNo,0 ,WFunction);
                    NForm80b.ShowDialog();
                }

                if (WFunction == "Interactive B") 
                {
                    // Update Us Process number
                
                    Form19b NForm19b;

                    int Actions = 1;

                    NForm19b = new Form19b(WSignedId, WSignRecordNo, WOperator, WCategoryId,WRMCycleNo, Actions);
                    NForm19b.ShowDialog(); ;
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

            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycleNo, 0 ,WFunction);
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

            if (Rms.StartReconcDtTm == NullPastDate || Rms.EndReconcDtTm == NullPastDate)
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
            if ((WMainCateg == "EWB1" & WCategoryId != "EWB110") || WMainCateg == "EWB3")
            {
                // Update Us Process number
                Us.ReadSignedActivityByKey(WSignRecordNo);
                Us.ProcessNo = 54; // View Only 
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

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

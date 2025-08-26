using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

using System.Data;
using System.Text;
//using System.Windows.Forms;
//using System.Globalization;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80a1 : Form
    {
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMReconcCategATMsAtRMCycles Rca = new RRDMReconcCategATMsAtRMCycles();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMGasParameters Gp = new RRDMGasParameters(); // Get parameters 

        //RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool Is_Presenter_InReconciliation = false;

        bool Is_Decentralised_For_ATMs = false;

        bool Is_ComingFromExternal = false;

        DateTime WCut_Off_Date;
        DateTime LIMITDate;
        int LimitRMCycle;
        int Minus_Days; 

        //string WMainCateg;
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

            // Set Working Date 

            //MessageBox.Show("101"); 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            label1UserId.Text = WSignedId;

            string WJobCategory = "ATMs";
            // Find Current Job Cycle 
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            if (WReconcCycleNo != 0)
            {
                WCut_Off_Date = Rjc.Cut_Off_Date;
            }
            else
            {
                WCut_Off_Date = NullPastDate;
            }
            // FIND Cycle ID For Cut Off date - minus 25 
            
            Gp.ReadParametersSpecificId(WOperator, "503", "01", "", ""); // 
            if (Gp.RecordFound == true)
            {
                Minus_Days = (int)Gp.Amount;
            }
            else
            {
                Minus_Days = 30; 
            }

            labelLimitParameter_503.Text = "We consider in Parameter 503.Days." + Minus_Days.ToString() + "..prior to today cut off";

            // SHOW ONLY Records above this 
            //LIMITDate = WCut_Off_Date.AddDays(-Minus_Days);

            //Rjc.ReadReconcJobCyclesByCutOffDate(LIMITDate); 

            //if (Rjc.RecordFound == true)
            //{
            //    LimitRMCycle = Rjc.JobCycle; 
            //}
            //else
            //{
            //    Rjc.ReadReconcJobCyclesAndFindFirstCycle(LIMITDate); 
            //    LimitRMCycle = Rjc.JobCycle; 
            //}

            if (WFunction == "Reconc")
            {
                textBoxMsgBoard.Text = "Go to Reconcile";

                buttonNext.Text = "Reconcile";

                panelReconciliationPlayBack.Hide();
                //panel6.Hide();
                //button6.Hide();
                buttonReconcPlayBack.Hide();

                labelStep1.Text = "ATMs Txns Reconciliation";
                labelLeft.Text = "ATM RECONCILIATION CATEGORIES";
                labelRight.Text = "ATMs TXNS RECONC DAILY CYCLES";

            }
            else
            {
               
            }

            // Presenter
            string ParId = "946";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Presenter_InReconciliation = true;
            }

            //MessageBox.Show("102");

            if (WFunction == "View")
            {
                textBoxMsgBoard.Text = "View Details of Reconciliation and Matching Cycles";

                labelStep1.Text = "RM Categories History";

                buttonNext.Show();
                buttonNext.Enabled = false;
            }

            // Decentralisation
            ParId = "947";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Decentralised_For_ATMs = true;
            }

        }

        // ON LOAD
        private void Form80a2_Load(object sender, EventArgs e)
        {

                //MessageBox.Show("103");
                Rc.ReadReconcCategoriesAndFillTableWithDiscrepancies(WOperator, WSignedId, WReconcCycleNo);

                ShowGrid1(); 
        }
        // Row Enter for Datagridview1
        string WCategoryId;
        string WOrigin;
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            if (Is_ComingFromExternal == false)
            {
                WCategoryId = rowSelected.Cells[1].Value.ToString();
            }
            else
            {
                WCategoryId = WCategoryId; 
            }


            Rc.ReadReconcCategorybyCategId(WOperator, WCategoryId);
            WOrigin = Rc.Origin;

            ShowGrid2(WCategoryId); // Grid on the right 
    
        }

        // Row enter for Datagridview2
        int WRemainingExceptions;
        int PlusPresenter;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WReconcCycleNo = (int)rowSelected.Cells[0].Value;

            //MessageBox.Show("Rowenter_2 104");

            Rjc.ReadReconcJobCyclesById(WOperator, WReconcCycleNo);

            //Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);

            Rcs.ReadReconcCategoriesSessionsSpecificCatWithPresenter(WOperator, WCategoryId, WReconcCycleNo);

           
            WRemainingExceptions = Rcs.RemainReconcExceptions;
            if (WFunction == "Reconc")
            {
                if (WRemainingExceptions > 0)
                {
                    buttonNext.Enabled = true;
                }
                else
                {
                    buttonNext.Enabled = false;
                }
            }
            //
            // Read open and close ones
            //
            Ap.ReadAuthorizationRecordByRMCategoryAndRMCycle(WCategoryId, WReconcCycleNo);

            if (Ap.RecordFound == true)
            {
                buttonNext.Enabled = false;
                if (Ap.OpenRecord == true)
                {
                    MessageBox.Show("Under Authorisation Process");
                }
                
            }

            //if (WCategoryId == "BDC103")
            //{
            //    buttonNext.Enabled = true;
            //}
            PlusPresenter = 0;
            if (WOrigin == "Our Atms")
            {
                panelATMs.Show();
                Rca.ReadReconcCategoriesATMsRMCycleTotals(WOperator, WCategoryId, WReconcCycleNo, Rcs.AtmGroup);

                labelATM1.Text = "ATMs Group.................: " + Rcs.AtmGroup.ToString();
                labelATM2.Text = "Number of Matched ATMs.....: " + Rca.TotalAtmsInGroupMatched.ToString();
                labelATM3.Text = "Number of UnMatched ATMs...: " + Rca.TotalAtmsInGroupUnMatched.ToString();

                if (Is_Presenter_InReconciliation == true)
                {
                    labelPresenter.Show();
                    labelPresenter.Text = "Number of Presenter Errors.: " + Rca.NumberOfPresenter.ToString();
                    PlusPresenter = Rca.NumberOfPresenter_Not_Settled; // Not Settled Presenter
                }
                else
                {
                    labelPresenter.Hide();
                }


                labelUnMatch1.Text = "Number Of Matched Txns.....: " + Rca.TotalNumberOfMatchedRecs.ToString();
                labelUnMatch2.Text = "Number of UnMatched Txns...: " + Rcs.NumberOfUnMatchedRecs.ToString();
                labelUnMatch3.Text = "Amount for UnMatched.......: " + Rca.TotalUnMatchedAmt.ToString("#,##0.00");
            }
            else
            {
                // Not Our Atms
                panelATMs.Hide();
                //MessageBox.Show("Rowenter_2 105");
                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                string SelectionCriteria = " WHERE  RMCateg ='"
                              + WCategoryId + "' AND MatchingAtRMCycle =" + WReconcCycleNo
                               + " AND IsMatchingDone = 1 ";
                // + " AND Matched = 0 ";

                Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria, 2);

                //MessageBox.Show("Rowenter_2 106");

                labelUnMatch1.Text = "Number Of Matched Txns.....: " + Mpa.TotalMatched.ToString();
                labelUnMatch2.Text = "Number of UnMatched Txns...: " + Mpa.TotalUnMatched.ToString();
                labelUnMatch3.Text = "Amount for UnMatched.......: " + Mpa.TotalAmountUnMatched.ToString("#,##0.00");

                //MessageBox.Show("Rowenter_2 107");

            }

            label3.Text = "DETAILS FOR CYCLE : " + WReconcCycleNo.ToString() + ", FOR CUT OFF DATE:.." + Rjc.Cut_Off_Date.ToShortDateString();

            //---------------------------------------------------------------------
            // RECONCILIATION 
            //---------------------------------------------------------------------
            if (Rcs.StartReconcDtTm == NullPastDate)
            {
                label20.Text = "Reconciliation didnt start";
                label19.Text = "Reconciliation didnt start";
                panelReconciliationPlayBack.Hide();

            }
            else
            {
                // Play Back ... View
                if (Rcs.EndReconcDtTm == NullPastDate)
                {
                    panelReconciliationPlayBack.Hide();
                    label20.Text = "Date Tm Start : " + Rcs.StartReconcDtTm.ToString();
                    label19.Text = "Reconciliation is in progess";
                }
                else
                {
                    panelReconciliationPlayBack.Show();
                    //MessageBox.Show("Rowenter_2 108");
                    label20.Text = "Date Tm Start.. : " + Rcs.StartReconcDtTm.ToString();
                    label19.Text = "Date Tm End.... : " + Rcs.EndReconcDtTm.ToString();

                    TimeSpan Remain2 = Rcs.EndReconcDtTm - Rcs.StartReconcDtTm;
                    label18.Text = "Duration in Minutes : " + Remain2.Minutes.ToString("#,##0.00");
                    if (Is_Presenter_InReconciliation == true)
                    {
                        // MessageBox.Show("Rowenter_2 109");
                      //  label16.Text = "Remaining Exceptions : " + (Rcs.RemainReconcExceptions + PlusPresenter).ToString();
                    }
                    else
                    {
                        //MessageBox.Show("Rowenter_2 110");
                      //  label16.Text = "Remaining Exceptions : " + Rcs.RemainReconcExceptions.ToString();
                    }

                    //MessageBox.Show("Rowenter_2 111");
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

                       // label16.ForeColor = Red;
                    }
                    else
                    {
                        Color Black = Color.Black;

                       // label16.ForeColor = Black;
                    }
                }
            }
        }
        int WRow1;

        int scrollPosition1 ;
        // NEXT 
        private void buttonNext_Click(object sender, EventArgs e)
        {
           // WRowIndexLeft = dataGridView1.SelectedRows[0].Index;
            //WOrigin
            if ( Is_Decentralised_For_ATMs == true & WOrigin == "Our Atms")
            {
                MessageBox.Show("The system is at decentralised mode Reconciliation"+Environment.NewLine
                    + "Responsible Branches will undertake the reconciliation"
                    + "The Controller can view the results"
                    );
                return; 
            }
            //if (Tuc.TotalATMsReady == 0)
            //{
            //    MessageBox.Show("No Atms ready to reconcile");
            //    return;
            //} 
            if (WCategoryId == "BDC103")
            {
                //
                // REMITTANCE CATEGORY
                //
                Form291NV_MT_103 NForm291NV_MT_103;

                string RunningGroup = "ATMs";
                NForm291NV_MT_103 = new Form291NV_MT_103(WSignedId, WSignRecordNo, "3", WOperator, RunningGroup);
                // NForm200JobCycles_Presenter.FormClosed += NForm200JobCycles_FormClosed; ;
                NForm291NV_MT_103.ShowDialog();
                return;
            }

            if (WFunction == "View" & WOperator != "ITMX")
            {
                WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

                Form80b NForm80b;

                WFunction = "View";

                NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WReconcCycleNo, "", 0, 0, WFunction, "", 0);
                NForm80b.FormClosed += NForm80b_FormClosed;
                NForm80b.ShowDialog();
            }

            if (WFunction == "Reconc")
            {
                //Ap.ReadAuthorizationsUserTotal(WSignedId); // User is requestor or Authoriser 
                //if (Ap.RecordFound)
                //{
                //    MessageBox.Show("Please take care with Authorisation record");
                //    return;
                //}
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ProcessNo = 2;

                string WApplication = Usi.SignInApplication;

                Usi.WFieldChar1 = WCategoryId;
                Usi.WFieldNumeric1 = WReconcCycleNo;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                int WAction = 11; // Reconciliation for Group of ATMs

                //MessageBox.Show(" YOU ARE NOT AUTHORIZED FOR MASS RECONCILIATION ");
                //return;
                // if (WOrigin == "Our Atms" || WOrigin == "JCC" || WOrigin == "Master Card" 
                //   || WOrigin == "123 Network" || WOrigin == "Visa Card" || WOrigin == "FAWRY" || WOrigin == "Credit_Card")
                //{

                // Check Reconciliation is needed
                if (WRemainingExceptions == 0)
                {
                    MessageBox.Show("No remaining exceptions to reconcile");
                    return;
                }

                if (WApplication == "ATMS/CARDS")
                {
                    //WRow1 = dataGridView1.SelectedRows[0].Index;

                    //scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

                    Form271 NForm271;

                    NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);
                    NForm271.FormClosed += NForm271_FormClosed_2;
                    NForm271.ShowDialog();
                }


                //if (WApplication == "e_MOBILE")
                //{
                //    Form277_MOBILE NForm277_MOBILE;

                //    NForm277_MOBILE = new Form277_MOBILE(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);
                //    NForm277_MOBILE.FormClosed += NForm271_FormClosed_2;
                //    NForm277_MOBILE.ShowDialog();
                //}

                return; 


            }

        }
        // SHow First Grid
        private void ShowGrid1()
        {
            dataGridView1.DataSource = Rc.TableReconcCateg.DefaultView;

            //MessageBox.Show("103_2");

            if (dataGridView1.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("No Entries available");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            dataGridView1.Columns[0].Width = 60; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 120; // Identity 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 200; // Category-Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 80; // Exceptions
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[3].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 80; // Origin 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        private void ShowGrid2(string InCategory)
        {
            Rcs.ReadReconcCategoriesSessionsSpecificCat(WOperator, WSignedId, InCategory, LimitRMCycle);

            dataGridView2.DataSource = Rcs.TableReconcSessionsPerCategory.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                labelRight.Hide();
                panel3.Hide();
                //MessageBox.Show("No testing data for this category");
            }
            else
            {
                labelRight.Show();
                panel3.Show();
            }

            if (InCategory == "BDC103")
            {
                buttonNext.Enabled = true;
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
            dataGridView2.Columns[2].Visible = false;

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

        private void NForm80b_FormClosed(object sender, FormClosedEventArgs e)
        {
            WFunction = "View";

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80a2_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }



        void NForm271_FormClosed_2(object sender, FormClosedEventArgs e)
        {
            WRow1 = dataGridView1.SelectedRows[0].Index;

           // scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

            int WRow2 = dataGridView2.SelectedRows[0].Index;

            Is_ComingFromExternal = true; 

            Form80a2_Load(this, new EventArgs());
            // First
            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

           // dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition1;

            Is_ComingFromExternal = false;


            // second

            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));


        }

        void NForm261_FormClosed(object sender, FormClosedEventArgs e)
        {
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80a2_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // REconciliation PlayBack 
        private void button1_Click(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54; // View Only 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //WRow1 = dataGridView1.SelectedRows[0].Index;

            //scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form271 NForm271;

            NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);
            NForm271.FormClosed += NForm271_FormClosed_2;
            NForm271.ShowDialog(); ;
        }

        // Catch Details
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                         + " . Application will be aborted! Call controller to take care. ");
            }
        }


    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form291NVMakeSystemMatching : Form
    {

        //bool ViewWorkFlow;

        string WModeNotes;

        DateTime WDt;
        DateTime ToDt;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        bool ValidProcess; 

        RRDMNVMakeMatchingAndAlertsClass Mmc = new RRDMNVMakeMatchingAndAlertsClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions(); 

        RRDMReconcJobCycles Dj = new RRDMReconcJobCycles();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime WDateOfCycle = new DateTime(2015, 02, 15);
        //
     
        //
        //int WDifStatus;

        int WReconcCycleNo;

        string WJobCategory; 

        string WOrigin ;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSubSystem;
   
        public Form291NVMakeSystemMatching(string InSignedId, int SignRecordNo, string InOperator, string InSubSystem)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WSubSystem = InSubSystem; // Used For Transactions 
                                      // "CardsSettlement"
                                      // "NostroReconciliation"  
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            //WSubSystem = "CardsSettlement";
            //WSubSystem = "NostroReconciliation";

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (WSubSystem == "CardsSettlement")
            {
                WJobCategory = "CardsSettlement";
                WOrigin = "Nostro - Vostro"; // Used For Categories 
            }
            if (WSubSystem == "NostroReconciliation")
            {
                WJobCategory = "NostroReconciliation";
                WOrigin = "Nostro - Vostro"; // Used For Categories 
            }

            //No Dates Are selected

            WDt = NullPastDate;
            ToDt = NullPastDate;
            //***************************************************
            // STEP A - Running Job
            //***************************************************
            // Create Daily Reconciliation Job Stream 
            //
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
          
            Rjc.ReadLastReconcJobCycleATMsAndNostro(WOperator, WJobCategory); 
            if (Rjc.RecordFound == true)
            {
                if (Rjc.StartDateTm.Day == WDateOfCycle.Day)
                {
                    WReconcCycleNo = Rjc.JobCycle;
                    MessageBox.Show("Daily Cycle for Nostro Already Opened" + Environment.NewLine
                                                  + "Results of this cycle will be shown" + Environment.NewLine
                                                  + "Run Refresh Data to restart for this DEMO if you want" 
                                                    );
                    ValidProcess = false; 
                   // this.Dispose();
                    //return; 
                }
                else
                {
                    ValidProcess = true;

                    // Insert new Cycle 

                    Rjc.Cut_Off_Date = DateTime.Now.Date;

                    Rjc.JobCategory = WJobCategory;
                    Rjc.StartDateTm = WDateOfCycle;
                    Rjc.FinishDateTm = NullPastDate; 
                    if (WJobCategory == "CardsSettlement") Rjc.Description = "Visa Settlement to Authorisation Reconciliation";
                    else Rjc.Description = "NostroReconciliation";

                    Rjc.InSuspense = false; 

                    Rjc.Operator = WOperator;

                    WReconcCycleNo = Rjc.InsertNewReconcJobCycle();

                }
            }
            else
            {
                ValidProcess = true;
                // Insert new Cycle 
                Rjc.Cut_Off_Date = DateTime.Now.Date;
                Rjc.JobCategory = WJobCategory;
                Rjc.StartDateTm = WDateOfCycle;
                Rjc.FinishDateTm = NullPastDate;
                if (WJobCategory == "CardsSettlement") Rjc.Description = "Visa Settlement to Authorisation Reconciliation";
                else Rjc.Description = "NostroReconciliation";

                Rjc.InSuspense = false;

                Rjc.Operator = WOperator;

                WReconcCycleNo = Rjc.InsertNewReconcJobCycle();
            }

            labelStep1.Text = "Matching Results For Cycle : " + WReconcCycleNo.ToString(); 

            if (ValidProcess == true)
            {
                //***************************************************
                // STEP B - Create this Cycle Reconciliation Categories 
                //***************************************************
                // Create Reconciliation Categories based on Matching Categories 
                //

                Mc.CreateReconciliationSessionsForMatchingCateg(WOperator, WSignedId,
                                                  WReconcCycleNo, WOrigin,
                                                  WDateOfCycle);
                //***************************************************
                // STEP C - Load Rates 
                //***************************************************
                // Update ReconciliationCycles with
                // Rates 
                Rcs.UpdateReconciliationSessionsForNostroVostroForRates(WOperator, WSignedId,
                                                  WReconcCycleNo, WOrigin,
                                                  WDateOfCycle);

                //***************************************************
                // STEP D - Load External And Internal Statements
                //***************************************************
                // From Given Directories read Files
                // Create new header record and Insert line records in Pools 
                // Update Reconciliation Categories for 
                //            Statement loaded
                //            Statement ID
                //            NumberOfLines
                Rcs.LoadStatementsOrVisaFiles(WOperator, WSignedId,
                                                  WReconcCycleNo, WOrigin,
                                                  WDateOfCycle);

                //*********************************************************
                // STEP E - MATCHING Of EXTERNAL TO INTERNAL - Many To Many
                //*********************************************************
                //
                // SYSTEM MATCHING "MANY TO MANY" OR "ONE TO ONE" FOR ALL IDENTICAL
                //
                //
                Rcs.MakeManyToManyMatching(WOperator, WSignedId,
                                                  WReconcCycleNo, WSubSystem,
                                                  WDateOfCycle);
                //******************************************************************
                // STEP F - MATCHING Of EXTERNAL TO INTERNAL ONE TO ONE WITH TOLERANCE 
                //******************************************************************
                // MAKE MATCHING OF EXTERNAL TO INTERNAL WITH ACCEPTED TOLERANCE ON AMOUNT AND VALUE DATE 
                //if (WJobCategory == "VISA")
                //{
                //    Mode = 1;
                //}
                //if (WJobCategory == "NOSTRO")
                //{
                //    Mode = 2;
                //}

                Mmc.MatchedExternalsToInternals(WOperator, WSignedId,
                                                  WSubSystem, WReconcCycleNo,
                                                  WDateOfCycle);

                //***************************************************
                // STEP G  - Outstanding ALerts Due to late reply 
                //***************************************************
                // Identify and Marke Alerts For the Unmatched Internal And External
                // Update Reconciliation Category Session 
                //if (WJobCategory == "VISA")
                //{
                //    Mode = 71;
                //}
                //if (WJobCategory == "NOSTRO")
                //{
                //    Mode = 72;
                //}

                Mmc.IndentifyAndUpdateAlertsInBothInternalAndExternal
                                                (WOperator, WSignedId,
                                                 WSubSystem, WReconcCycleNo,
                                                 WDateOfCycle);

                //***************************************************
                // STEP H  - Check for duplicated in EXTERNAL Statement
                //***************************************************
                // 

                Mode = 72;

                // Check AGAINST Amt , Value Date , Reference 

                // ARE THESE ENOUGH TO IDENTIFY DUBLICATES ??? 
                // OR is it natural 
                // RAISE ALERT 
                // Update Reconciliation Category Session 

                //***************************************************
                // STEP J - Update with Totals 
                //***************************************************
                // Update ReconciliationCycles with
                // Totals 
                Rcs.UpdateReconciliationSessionsForNostroVostroForTotals(WOperator, WSignedId,
                                                  WReconcCycleNo, WSubSystem,
                                                  WDateOfCycle);

            }


            // FILL COMBO 

            comboBoxFilter.Items.Add("Matched_This_Cycle");
            comboBoxFilter.Items.Add("To_Be_Confirmed_This_Cycle");
            comboBoxFilter.Items.Add("All Un Matched");
            comboBoxFilter.Items.Add("Alerts");
            comboBoxFilter.Text = "Matched_This_Cycle";
        }

        // LOAD SCREEN 
        int Mode;
        private void Form291_Load(object sender, EventArgs e)
        {
            // Read and Fill Table only the default 
            //public void ReadNVStatement_LinesForMatched(string InOperator, string InSignedId,
            //                      int InRunningJobNo, string InExternalAccno, string InternalAccNo)
            if (comboBoxFilter.Text == "Matched_This_Cycle")
            {
                Mode = 9; // All auto matched this cycle 
                Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, Mode, WReconcCycleNo, "", "", "", WDateOfCycle, "");

                //Show Table
                ShowGrid9();
            }
            if (comboBoxFilter.Text == "To_Be_Confirmed_This_Cycle")
            {
                Mode = 10; // All matched this cycle 
                Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, Mode, WReconcCycleNo, "", "", "", WDateOfCycle, "");

                //Show Table
                ShowGrid10();
            }
            if (comboBoxFilter.Text == "All Un Matched")
            {
                Mode = 11; // All Un matched this cycle 
                Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, Mode, WReconcCycleNo, "", "", "", WDateOfCycle, "");

                //Show Table
                ShowGrid11();
            }
            if (comboBoxFilter.Text == "Alerts")
            {
                Mode = 13; // All Alerts this cycle 
                Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, Mode, WReconcCycleNo, "", "", "", WDateOfCycle, "");

                //Show Table
                ShowGrid11();
            }

            if (comboBoxFilter.Text != "Matched_This_Cycle")
            {
                buttonUnDo.Hide();
            }
            else
            {
                buttonUnDo.Show();
            }

            SetScreen();

        }

        int WSeqNo;
        string SelectionCriteria;
        
        int WUniqueMatchingNo;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
          
            WSeqNo = (int)rowSelected.Cells[0].Value;
            WOrigin = (string)rowSelected.Cells[4].Value;

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            WUniqueMatchingNo = Se.UniqueMatchingNo;

            int WColorNo = (int)rowSelected.Cells[1].Value;

            if (WColorNo == 11)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DimGray;
            }
            else if (WColorNo == 12)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            }

            if (Se.IsException == true)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkOrange;
            }

            textBox1.Text = Se.UniqueMatchingNo.ToString();
            textBox2.Text = Se.StmtLineValueDate.ToString();
            textBox3.Text = Se.StmtLineAmt.ToString("#,##0.00");

        }

        public void SetScreen()
        {

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Default Approval stage for Job Cycle No: " + WReconcCycleNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            //// ................................
            //// Handle View ONLY 
            //// ''''''''''''''''''''''''''''''''
            //Us.ReadSignedActivityByKey(WSignRecordNo);

            //if (WViewFunction == true || WAuthoriser == true || WRequestor == true) // THIS is not normal process 
            //{
            //    ViewWorkFlow = true;

            //    if (Cn.TotalNotes == 0)
            //    {
            //        //label1.Hide();

            //        buttonNotes2.Hide();
            //        labelNumberNotes2.Hide();
            //    }
            //    else
            //    {
            //        buttonNotes2.Show();
            //        labelNumberNotes2.Show();

            //    }
            //}
            //else
            //{
            //    buttonNotes2.Show();
            //    labelNumberNotes2.Show();
            //}

        }


        // Show Grid 9 - Matched 
        public void ShowGrid9()
        {

            dataGridView1.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;
            //if (dataGridView1.Rows.Count == 0 & WRange == false)
            //{
            //    //MessageBox.Show("No transactions to be posted");
            //    Form2 MessageForm = new Form2("No transactions to be posted");
            //    MessageForm.ShowDialog();

            //    this.Dispose();
            //    return;
            //}

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No Matched");
                MessageForm.ShowDialog();
                this.Dispose();
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";
           
            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; // MatchingNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 170; //MatchedType
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 85;  // Origin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 105;  // Acc No 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = true;

            dataGridView1.Columns[6].Width = 40;  // Done 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = false;

            dataGridView1.Columns[7].Width = 40;  // Code 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 90; //  ValueDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[9].Width = 90; // EntryDate
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 40; // DR/CR
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 100; // Amt
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[12].Width = 130; // OurRef
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[12].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[12].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[12].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[13].Width = 100; // TheirRef
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 95; // OtherDetails
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[15].Width = 95; // Ccy
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[15].Visible = true;

            dataGridView1.Columns[16].Width = 95; // CcyRate
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[16].Visible = true;

            dataGridView1.Columns[17].Width = 95; // GLAccount
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[17].Visible = true;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                int WColorNo = (int)row.Cells[1].Value;

                if (WColorNo == 11)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WColorNo == 12)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }


        }

        // Show Grid 10 - To be confirmed 
        public void ShowGrid10()
        {

            dataGridView1.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No to be confirmed");
                MessageForm.ShowDialog();

                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; // MatchingNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 170; //MatchedType
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 85;  // Origin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 105;  // Acc No 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = true;

            dataGridView1.Columns[6].Width = 40;  // Done 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = false;

            dataGridView1.Columns[7].Width = 40;  // Code 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 90; //  ValueDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[9].Width = 90; // EntryDate
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 40; // DR/CR
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 100; // Amt
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[12].Width = 130; // OurRef
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[12].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[12].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[12].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[13].Width = 100; // TheirRef
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 95; // OtherDetails
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[15].Width = 95; // Ccy
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[15].Visible = true;

            dataGridView1.Columns[16].Width = 95; // CcyRate
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[16].Visible = true;

            dataGridView1.Columns[17].Width = 95; // GLAccount
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[17].Visible = true;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                int WColorNo = (int)row.Cells[1].Value;

                if (WColorNo == 11)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WColorNo == 12)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

        }

        // Show Grid 11 - Unmatched 
        public void ShowGrid11()
        {

            dataGridView1.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No Un Matched");
                MessageForm.ShowDialog();

                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; // MatchingNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 170; //MatchedType
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = false;

            dataGridView1.Columns[4].Width = 85;  // Origin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 105;  // Acc No 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = true;

            dataGridView1.Columns[6].Width = 40;  // Done 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = false;

            dataGridView1.Columns[7].Width = 40;  // Code 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 90; //  ValueDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[9].Width = 90; // EntryDate
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 40; // DR/CR
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 100; // Amt
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[12].Width = 130; // OurRef
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[12].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[12].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[12].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[13].Width = 100; // TheirRef
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 95; // OtherDetails
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[15].Width = 95; // Ccy
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[15].Visible = true;

            dataGridView1.Columns[16].Width = 95; // CcyRate
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[16].Visible = true;

            dataGridView1.Columns[17].Width = 95; // GLAccount
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[17].Visible = false;

            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    //WSeqNo = (int)rowSelected.Cells[0].Value;
            //    int WColorNo = (int)row.Cells[1].Value;

            //    if (WColorNo == 11)
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }
            //    else if (WColorNo == 12)
            //    {
            //        row.DefaultCellStyle.BackColor = Color.White;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }
            //}

        }
        // NOTES 
        private void buttonNotes2_Click_1(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Default Approval stage for Job Cycle No: " + WReconcCycleNo;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WModeNotes = "Read";
            //else WModeNotes = "Update";
            
            WModeNotes = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WModeNotes, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        //Undo Matched 
        private void buttonUnDo_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to undo these matched txns?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {

                SelectionCriteria = " WHERE UniqueMatchingNo =" + WUniqueMatchingNo;

                Se.Matched = false;
                Se.ToBeConfirmed = false;
                Se.ActionType = "00";
                Se.MatchedType = "Manual";
                Se.SettledRecord = false;

                Se.UpdateExternalFooterMatchedToUnmatched(WOperator, SelectionCriteria);
                Se.UpdateInternalFooterMatchedToUnmatched(WOperator, SelectionCriteria);

                Form291_Load(this, new EventArgs());

            }
            else
            {

            }

        }
        // Print 
        private void button1_Click(object sender, EventArgs e)
        {
            //RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            //Usi.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

            string P1 = "REPORT OF : " + comboBoxFilter.Text;
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;
            string P6 = DateTime.Now.ToShortDateString(); 
            string P7 = DateTime.Now.ToShortDateString();

            Form56R64NOSTRO ReportNOSTRO64 = new Form56R64NOSTRO(P1, P2, P3, P4, P5, P6, P7);
            ReportNOSTRO64.Show();
        }
// On Filter Change Load 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form291_Load(this, new EventArgs());
        }
    }
}


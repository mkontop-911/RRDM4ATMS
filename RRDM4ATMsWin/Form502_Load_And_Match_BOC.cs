using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
//using System.Data.OleDb;
using RRDM4ATMs;
using RRDMAgent_Classes;
using System.Globalization;
using Microsoft.Win32;
using System.Security.Principal;


// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502_Load_And_Match_BOC : Form
    {

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        //RRDM_LoadFiles_InGeneral_EMR_BDC Lf_ABE = new RRDM_LoadFiles_InGeneral_EMR_BDC();

        RRDM_LoadFiles_InGeneral_EMR_AUD Lf_AUD = new RRDM_LoadFiles_InGeneral_EMR_AUD();

        RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4 Mt = new RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMJournalReadTxns_Text_Class Jc = new RRDMJournalReadTxns_Text_Class();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMAgentQueue Aq = new RRDMAgentQueue();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions(); 

        public DataTable SavedSourceFilesDataTable = new DataTable(); // Rs
        public DataTable SavedTableMatchingCateg_Matching_Status = new DataTable(); // Mc

        // int WNumberOfLoadingAndMatching;

        string ReversedCut_Off_Date;

        string ProcessName;
        string Message;
        int Mode;

        string PRX;

        DateTime SavedStartDt;

        bool J_UnderLoading;

        //  DateTime J_SavedStartDt;

        DateTime WCut_Off_Date;

        int WSeqNoLeft;

        //int WReconcCycleNo;
        int WReconcCycleNoFirst;
        DateTime WFirstCut_Off_Date;

        int WRowIndex;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WReconcCycleNo;
        int WMode;

        public Form502_Load_And_Match_BOC(string InSignedId, int SignRecordNo, string InOperator
                                                              , int InReconcCycleNo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WReconcCycleNo = InReconcCycleNo;
            WMode = InMode; // If 1 then is called from the administrator
                            // If 2 then is called for view only 

            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }

            // View Only AND Also USED for UNDO Of File
            if (WMode == 2)
            {
                buttonLoadJournals.Enabled = false;
                buttonLoadFiles.Enabled = false;
                buttonDoMatching.Enabled = false;
                label3.Text = "THE CYCLES";
                buttonExportToExcel.Hide();
                //panelFiles.Hide();
               // panelUndo.Hide();

                label6.Hide();
                label7.Hide();
                label9.Hide();

                textBoxNumberOfFiles.Hide();
                textBoxFilesReady.Hide();
                textBoxNotReady.Hide();

                buttonShowComment.Hide();

                label15.Hide();
                label17.Hide();
                label18.Hide();
                label19.Hide();

                textBox1.Hide();
                textBox2.Hide();
                textBox3.Hide();
                textBoxInDirForLoading.Hide();

                // UNDO File Fuctionality
                labelCycle.Hide();
                textBoxCycle.Hide();

                buttonUndoFile.Hide();

            }
            else
            {
                // buttonUndoFile.Hide(); 
            }

            // Set Working Date 
            StopTimer = true;

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;
            // FIND CUTOFF CYCLE

            //    string WOperator = "BCAIEGCX";       
            string WJobCategory = "ATMs";

            //WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            Rjc.ReadReconcJobCyclesById(WOperator, WReconcCycleNo);

            if (Rjc.RecordFound == true)
            {
                WCut_Off_Date = Rjc.Cut_Off_Date.Date;

                ReversedCut_Off_Date = WCut_Off_Date.ToString("yyyyMMdd");

                //WNumberOfLoadingAndMatching = Rjc.NumberOfLoadingAndMatching;
            }

            if (WMode == 1)
            {
                labelStep1.Text = "Loading and Match of Cycle_" + WReconcCycleNo.ToString()
                                + "_and Date_" + WCut_Off_Date.ToShortDateString();
            }

            labelCycleNo.Text = WReconcCycleNo.ToString();

            WReconcCycleNoFirst = Rjc.ReadFirstReconcJobCycle(WOperator, WJobCategory);

            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {
                WFirstCut_Off_Date = Rjc.Cut_Off_Date.Date;
            }

            // Categories
            comboBoxMatchingCateg.DataSource = Mc.GetCategories(WOperator);
            comboBoxMatchingCateg.DisplayMember = "DisplayValue";

            radioButtonCategory.Checked = true;

            textBoxMsgBoard.Text = "Current Status:Ready";

            // GET TOTALS 
        }
        // Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            Rcs.ReadReconcCategoriesSessions_To_Check_If_MatchingDONE(WReconcCycleNo); 
            if (Rcs.RecordFound == true)
            {
                labelFieldsDefinition.Show();
                panelCategories.Show();
                buttonMatching.Show(); 
            }
            else
            {
                labelFieldsDefinition.Hide();
                panelCategories.Hide();
                buttonMatching.Hide();
            }
            if (WMode == 2)
            {

                // View Only
                // labelStep1.Text = "View loaded amd Matched of Cycle_" + WReconcCycleNo.ToString()
                //                + "_and Date_" + WCut_Off_Date.ToShortDateString();
            }
            else
            {
                CheckLoadingOfJournals();
            }
            //if (J_UnderLoading == true)
            //{
            //}
            JournalTotals();
            // Other totals to fill grid
            GetTotals();

            radioButtonCategory.Checked = true;
            if (WMode == 1)
            {
                // Source Files (Grid-ONE)
                // Read them and check them 
                string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1  ";

                Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

                textBoxNumberOfFiles.Text = Rs.TotalFiles.ToString();
                textBoxFilesReady.Text = textBoxInDirForLoading.Text = Rs.TotalReady.ToString();
                textBoxNotReady.Text = (Rs.TotalFiles - Rs.TotalReady).ToString();

                ShowGrid1();


                if (Rs.WFutureFiles > 0)
                {
                    textBoxMsgBoard.Text = "There are other Dates files in directories ..No:.." + Rs.WFutureFiles.ToString();
                }
            }
            else
            {

                string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND JobCategory ='ATMs'";
                Rjc.ReadReconcJobCyclesFillTable(SelectionCriteria);
                ShowCycles();

            }


        }
        // Row FIRST GRID
        string WFileComment;
        private void dataGridViewFiles_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFiles.Rows[e.RowIndex];

            if (WMode == 1)
            {
                WSeqNoLeft = (int)rowSelected.Cells[0].Value;

                WFileComment = rowSelected.Cells[5].Value.ToString();

            }
            else
            {
                WReconcCycleNo = (int)rowSelected.Cells[0].Value;
                Rjc.ReadReconcJobCyclesById(WOperator, WReconcCycleNo);

                if (Rjc.RecordFound == true)
                {
                    WCut_Off_Date = Rjc.Cut_Off_Date.Date;

                    ReversedCut_Off_Date = WCut_Off_Date.ToString("yyyyMMdd");

                    //WNumberOfLoadingAndMatching = Rjc.NumberOfLoadingAndMatching;
                }

                // View Only
                labelStep1.Text = "View loaded amd Matched of Cycle_" + WReconcCycleNo.ToString()
                                + "_and Date_" + WCut_Off_Date.ToShortDateString();
            }


            // GRID 2 LOADED FILES
            Flog.ReadLoadedFiles_Fill_Table(WOperator, WReconcCycleNo);

            ShowGrid2();

            Rc.ReadReconcCategoriesForMatrix(WOperator, WReconcCycleNo,2);

            ShowGrid4();


        }


        // Row ENTER for Read Files
        int WSeqNoLoadedFile;
        private void dataGridViewLoadedFiles_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewLoadedFiles.Rows[e.RowIndex];
            WSeqNoLoadedFile = (int)rowSelected.Cells[0].Value;
        }
        // Row enter 
        string WMatchedCat;
        private void dataGridViewMatched_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewMatched.Rows[e.RowIndex];

            WMatchedCat = (string)rowSelected.Cells[0].Value;

            textBoxCateg.Text = WMatchedCat;
            RRDMMatchingCategories Mc = new RRDMMatchingCategories();
            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WMatchedCat);
            label_Categ_Name.Text = Mc.CategoryName;

            if (WMatchedCat.Substring(0, 4) == "RECA")
            {
                // WMatchedCat = "BDC201";
                label_Categ_Name.Text = "There are more than one categories";
                buttonViewGroupCateg.Show();
                linkLabelShowFiles.Hide();
            }
            else
            {
                buttonViewGroupCateg.Hide();
                linkLabelShowFiles.Show();
            }
            // string xx = WMatchedCat.Substring(0, 7);

        }

        // Row Enter Files Totals
        string WFileId;
        private void dataGridViewFilesStatus_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFilesStatus.Rows[e.RowIndex];
            WFileId = (string)rowSelected.Cells[0].Value;
            textBoxFileId.Text = WFileId;

        }
        // 
        private void ShowGrid1()
        {

            dataGridViewFiles.DataSource = Rs.Table_Files_In_Dir.DefaultView;

            this.dataGridViewFiles.Sort(this.dataGridViewFiles.Columns["IsGood"], ListSortDirection.Descending);

            dataGridViewFiles.Columns[0].Width = 100; // seq
            dataGridViewFiles.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewFiles.Columns[0].Visible = false;

            dataGridViewFiles.Columns[1].Width = 100; // SourceFileId
            dataGridViewFiles.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewFiles.Columns[1].Visible = false;

            dataGridViewFiles.Columns[2].Width = 500; // FullFileName
            dataGridViewFiles.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFiles.Columns[3].Width = 60; // IsPresent
            dataGridViewFiles.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewFiles.Columns[4].Width = 60; // IsGood
            dataGridViewFiles.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewFiles.Columns[5].Width = 170; // Comment
            dataGridViewFiles.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFiles.Columns[6].Width = 100; // DateExpected
            dataGridViewFiles.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            bool IsPresent;
            string IsGood;

            foreach (DataGridViewRow row in dataGridViewFiles.Rows)
            {
                IsPresent = (bool)row.Cells[3].Value;
                IsGood = (string)row.Cells[4].Value;

                if (IsPresent == true & IsGood == "NO")
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }


        }

        // Show CYCLES
        private void ShowCycles()
        {
            //dataGridViewFiles.DataSource = Rs.Table_Files_In_Dir.DefaultView;
            dataGridViewFiles.DataSource = Rjc.TableReconcJobCycles.DefaultView;



            dataGridViewFiles.Columns[0].Width = 60; // JobCycle;
            dataGridViewFiles.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 200; // JobCategory;
            dataGridViewFiles.Columns[1].Visible = false;

            dataGridViewFiles.Columns[2].Width = 120; // StartDateTm.ToString();
            dataGridViewFiles.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFiles.Columns[3].Width = 120; // FinishDateTm
            dataGridViewFiles.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFiles.Columns[4].Width = 120; //  Description
            dataGridViewFiles.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFiles.Columns[5].Width = 120; // "Status"
            dataGridViewFiles.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }


        private void ShowGrid2()
        {
            dataGridViewLoadedFiles.DataSource = Flog.DataTableFileMonitorLog.DefaultView;
            if (dataGridViewLoadedFiles.Rows.Count == 0)
            {
                //  MessageBox.Show("No Loaded Files Exist For This Cycle!");
                //  this.Dispose();
                textBoxFilesRight.Text = "0";

                panelLoaded.Hide();
                label4.Hide();
                linkLabelExpand.Hide(); 

                return;
            }
            else
            {
                panelLoaded.Show();
                label4.Show();
                linkLabelExpand.Show();

                
                textBoxFilesRight.Text = dataGridViewLoadedFiles.Rows.Count.ToString();

                dataGridViewLoadedFiles.Columns[0].Width = 60; //"FileName  as in directory "
                dataGridViewLoadedFiles.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewLoadedFiles.Columns[0].Visible = false;

                dataGridViewLoadedFiles.Columns[1].Width = 170; //"FileName  as in directory "
                dataGridViewLoadedFiles.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewLoadedFiles.Columns[1].Visible = true;

                dataGridViewLoadedFiles.Columns[2].Width = 75; // Status
                dataGridViewLoadedFiles.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewLoadedFiles.Columns[2].Visible = true;

                dataGridViewLoadedFiles.Columns[3].Width = 50; // Min Date
                dataGridViewLoadedFiles.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewLoadedFiles.Columns[3].Visible = true;

                dataGridViewLoadedFiles.Columns[4].Width = 160; // Max Date
                dataGridViewLoadedFiles.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewLoadedFiles.Columns[4].Visible = true;

                dataGridViewLoadedFiles.Columns[5].Width = 100; // DateTimeReceived
                dataGridViewLoadedFiles.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewLoadedFiles.Columns[5].Visible = true;

                dataGridViewLoadedFiles.Columns[6].Width = 70; // DateExpected
                dataGridViewLoadedFiles.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewLoadedFiles.Columns[6].Visible = true;

                dataGridViewLoadedFiles.Columns[7].Width = 70; // LineCount
                dataGridViewLoadedFiles.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewLoadedFiles.Columns[7].Visible = true;

                dataGridViewLoadedFiles.Columns[8].Width = 500; // stpErrorText
                dataGridViewLoadedFiles.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewLoadedFiles.Columns[8].Visible = true;

            }
        }

        int CountNotDone;
        private void ShowGrid4()
        {

            dataGridViewMatched.DataSource = Rc.TableReconcCateg.DefaultView;

            if (dataGridViewMatched.Rows.Count == 0)
            {
                return;
            }
            else
            {
                //dataGridView2.Show(); 
            }

            CountNotDone = 0;

            dataGridViewMatched.Columns[0].Width = 110; // CategoryId
            dataGridViewMatched.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMatched.Columns[1].Width = 80; // This Cycle UnMatched- 
            dataGridViewMatched.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMatched.Columns[2].Width = 80; // UnMatched
            dataGridViewMatched.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMatched.Columns[3].Width = 80; // Cycle1
            dataGridViewMatched.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewMatched.Columns[3].HeaderText = "Cycle " + Rc.Cycle1;

            dataGridViewMatched.Columns[4].Width = 80; // Cycle2
            dataGridViewMatched.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewMatched.Columns[4].HeaderText = "Cycle " + Rc.Cycle2;

            dataGridViewMatched.Columns[5].Width = 80; // Cycle3
            dataGridViewMatched.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewMatched.Columns[5].HeaderText = "Cycle " + Rc.Cycle3;

            dataGridViewMatched.Columns[6].Width = 80; // Cycle4
            dataGridViewMatched.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewMatched.Columns[6].HeaderText = "Cycle " + Rc.Cycle4;

            dataGridViewMatched.Columns[7].Width = 80; // Cycle5
            dataGridViewMatched.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewMatched.Columns[7].HeaderText = "Cycle " + Rc.Cycle5;

            foreach (DataGridViewRow row in dataGridViewMatched.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                string WCycle1 = (string)row.Cells[3].Value;

                if (WCycle1 == "Not Done")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    CountNotDone = CountNotDone + 1;
                }
                if (WCycle1 == "Fully Done")
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                    row.DefaultCellStyle.ForeColor = Color.White;

                }
                if (WCycle1 == "Partially")
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

        }

        private void ShowGrid5()
        {
            dataGridViewFilesStatus.DataSource = Lf_AUD.SourceFilesTotals.DefaultView;

            // dataGridViewFiles.Sort(dataGridViewFiles.Columns[0], ListSortDirection.Ascending);
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridViewFilesStatus.Columns[0].Width = 180; // file 
            dataGridViewFilesStatus.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewFilesStatus.Columns[0].Visible = true;

            dataGridViewFilesStatus.Columns[1].Width = 150; // nonprocessed
            dataGridViewFilesStatus.Columns[1].DefaultCellStyle = style;
            dataGridViewFilesStatus.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewFilesStatus.Columns[1].Visible = true;

            dataGridViewFilesStatus.Columns[2].Width = 150; // processed this cycle 
            dataGridViewFilesStatus.Columns[2].DefaultCellStyle = style;
            dataGridViewFilesStatus.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


        }
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        // Refresh 

        private void button1_Click(object sender, EventArgs e)
        {
        }
        // Check Journals
        private void CheckLoadingOfJournals()
        {
            //return; 
            J_UnderLoading = false;

            // Read The latest info from Cycle 
            string WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;
            bool WJournalLoadingStarted = Rjc.SpareBool_1;
            int WQueueId = Rjc.SpareInt_1;

            RRDMAgentQueue Aq = new RRDMAgentQueue();

            if (WJournalLoadingStarted == true & WQueueId > 0)
            {

                string WSelectionCriteria = " WHERE OriginalReqId =" + WQueueId
                                  + " AND OriginalRequestorID ='" + WSignedId + "'";

                Aq.ReadAgentQueueBySelectionCriteria(WSelectionCriteria);

                if (Aq.RecordFound == false)
                {
                    J_UnderLoading = true; // Record not updated yet 

                }

                if (Aq.RecordFound == true & Aq.MessageSent == false)
                {
                    J_UnderLoading = true;

                }
                if (Aq.RecordFound == true & Aq.MessageSent == true)
                {
                    J_UnderLoading = false;

                }
                string C_AbeDir = ConfigurationManager.AppSettings["C_ABE"];
                string SourceFileId = "Atms_Journals_Txns";

                Rs.ReadReconcSourceFilesByFileId(SourceFileId);

                string InSourceDirectory = Rs.SourceDirectory;
                string InSourceDirectoryABE = Rs.SourceDirectory;
                InSourceDirectoryABE = InSourceDirectoryABE.Replace(@"C:\", C_AbeDir);

                string[] allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

                textBox1.Text = allJournals.Length.ToString();

            }


            if (J_UnderLoading == true)
            {
                //
                // SHOW NUMBER OF REMAINING JOURNALS
                //
                string SourceFileId = "Atms_Journals_Txns";

                Rs.ReadReconcSourceFilesByFileId(SourceFileId);

                string InSourceDirectory = Rs.SourceDirectory;

                string[] allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

                textBox1.Text = allJournals.Length.ToString();


                MessageBox.Show("Under Loading of Journals process. " + Environment.NewLine
                    + "There are.." + allJournals.Length.ToString() + " to be loaded.  ");
                //  textBox1.Text = "0";
                return;

            }
        }


        // Show ready Categories
        private void buttonShowReadyCateg_Click(object sender, EventArgs e)
        {

            // Find the ready categories
            Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                    WReconcCycleNo);

            MessageBox.Show(Mt.W_MPComment);
        }
        ////
        //// Confirm
        ////
        //bool Confirmed;
        //private void buttonConfirm_Click(object sender, EventArgs e)
        //{
        //    Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
        //                           WReconcCycleNo);

        //    //MessageBox.Show(Mt.W_MPComment);



        //    // Verification Message 
        //    if (MessageBox.Show("Do you want to Start Loading And Matching?" + Environment.NewLine
        //                       + "For Ready Categories " + Environment.NewLine
        //                       + Mt.W_MPComment
        //                       , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        //                == DialogResult.Yes)
        //    {
        //        // YES Proceed
        //        // Update Reconciliation Cycle 
        //        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        //        WNumberOfLoadingAndMatching = WNumberOfLoadingAndMatching + 1;

        //        Rjc.UpdateLoadingAndMatchingField(WReconcCycleNo, WNumberOfLoadingAndMatching);

        //      //  textBoxConfirmed.Show();
        //      //  Confirmed = true;

        //        //  buttonLoad.Show(); 
        //    }
        //    else
        //    {
        //        // Stop 
        //        return;
        //    }
        //}
        //
        // Load Journals 
        //
        int WServiceReqID;
        bool CommandSent;
        private void buttonLoadJournals_Click(object sender, EventArgs e)
        {
            // CHECK IF SIGN ON USERS 
            bool IsAllowedToSignIn = false;
            bool ThereAreUsersInSystem = CheckForSignInUsers(IsAllowedToSignIn);

            if (ThereAreUsersInSystem == true)
            {
                // Decide whether to move forward or not. 
            }
            //
            // FROM NOW ON NOBODY CAN SIGN IN EXCEPT THE CONTROLLER
            //
            textBoxMsgBoard.Text = "Current Status:Loading of Journals process";

            string SourceFileId = "Atms_Journals_Txns";

            Rs.ReadReconcSourceFilesByFileId(SourceFileId);

            string InSourceDirectory = Rs.SourceDirectory;

            string[] allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

            if (allJournals.Length == 0)
            {
                MessageBox.Show(" There are no files for loading");
                textBox1.Text = "0";
                return;
            }
            else
            {
                // Change names for AUDI Journals  
                foreach (string file in allJournals)
                {
                    //01900606_08_09_21_RAW.txt
                    string myFilePath = @file;
                    string ext = Path.GetExtension(myFilePath);
                    string OldFileName = Path.GetFileName(myFilePath);

                    if (ext == ".txt")
                    {
                        try
                        {
                            string directoryPath = Path.GetDirectoryName(myFilePath);
                            if (directoryPath == null)
                            {
                                throw new Exception($"Directory not found in given path value:{myFilePath}");
                            }
                            string WATM = OldFileName.Substring(0, 8);
                            string WYYYYY = OldFileName.Substring(9, 4);
                            string WMM = OldFileName.Substring(13, 2);
                            string WDD = OldFileName.Substring(15, 2);

                            string NewFileName = WATM + "_" + WYYYYY + WMM + WDD + "_EJ_NCR.001";
                            var newFilenameWithPath = Path.Combine(directoryPath, NewFileName);
                            FileInfo fileInfo = new FileInfo(myFilePath);
                            fileInfo.MoveTo(newFilenameWithPath);
                        }
                        catch (Exception exf)
                        {
                            CatchDetails(exf);
                        }

                        //File.Delete(file);
                        // Count1 = Count1 + 1;
                    }
                }
                // Change names for log ones ABE 
                foreach (string file in allJournals)
                {
                    //AB1V0015_2021_08_01_EJ_NCR.log
                    string myFilePath = @file;
                    string ext = Path.GetExtension(myFilePath);
                    string OldFileName = Path.GetFileName(myFilePath);

                    if (ext == ".log" || ext == ".LOG")
                    {
                        try
                        {
                            string directoryPath = Path.GetDirectoryName(myFilePath);
                            if (directoryPath == null)
                            {
                                throw new Exception($"Directory not found in given path value:{myFilePath}");
                            }
                            string WATM = OldFileName.Substring(0, 8);
                            string WYYYYY = OldFileName.Substring(9, 4);
                            string WMM = OldFileName.Substring(14, 2);
                            string WDD = OldFileName.Substring(17, 2);

                            string NewFileName = WATM + "_" + WYYYYY + WMM + WDD + "_EJ_NCR.001";
                            var newFilenameWithPath = Path.Combine(directoryPath, NewFileName);
                            FileInfo fileInfo = new FileInfo(myFilePath);
                            fileInfo.MoveTo(newFilenameWithPath);
                        }
                        catch (Exception exf)
                        {
                            CatchDetails(exf);
                        }

                        //File.Delete(file);
                        // Count1 = Count1 + 1;
                    }
                }
            }
            // MessageBox.Show("Deleted Journals .jln..=.." + Count1.ToString());
            // After this check you 
            // Check if Dublicate and delete 
            // allJournals = Directory.GetFiles(InSourceDirectory, "*.*");



            if (allJournals.Length == 0)
            {
                MessageBox.Show(" There are no files for loading");
                textBox1.Text = "0";
                return;
            }
            else
            {
                // DELETE jln
                foreach (string file in allJournals)
                {
                    string myFilePath = @file;
                    string ext = Path.GetExtension(myFilePath);

                    if (ext == ".jln")
                    {
                        File.Delete(file);
                        // Count1 = Count1 + 1;
                    }
                }
                // MessageBox.Show("Deleted Journals .jln..=.." + Count1.ToString());
                // After this check you 
                // Check if Dublicate and delete 
                allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

                if (allJournals.Length == 0)
                {
                    // Re Check here
                    MessageBox.Show(" There are no files for loading");
                    textBox1.Text = "0";
                    textBoxMsgBoard.Text = "Current Status:Ready";
                    return;
                }
                int Count2 = 0;
                foreach (string file in allJournals)
                {
                    if (Rs.CheckIfFileIsDublicate(file) == true)
                    {
                        // Delete File 
                        File.Delete(file);
                        Count2 = Count2 + 1;
                    }

                }

                if (Count2 > 0)
                {
                    MessageBox.Show("Journals found that were loaded before." + Environment.NewLine
                              + "Number of journals :.. " + Count2.ToString()
                              + "...These journals were deleted from directory"
                                                                           );
                }

                allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

                if (allJournals.Length == 0)
                {
                    // Re Check here
                    MessageBox.Show(" There are no files for loading");
                    textBox1.Text = "0";
                    textBoxMsgBoard.Text = "Current Status:Ready";
                    return;
                }

            }

            //MessageBox.Show("Deleted Journals already loaded.=.." + Count2.ToString());
            // Find Number of Threads 
            Gp.ReadParametersSpecificId(WOperator, "914", "1", "", ""); // 
            int NumberOfThreads = (int)Gp.Amount;

            MessageBox.Show("Through this button a service will be activated" + Environment.NewLine
                                    + " Loading and parsing of Journals will start" + Environment.NewLine
                                      + "NUMBER OF THREADS(PAR=914)..=.." + NumberOfThreads.ToString() + Environment.NewLine
                                     );

            // ************************
            // CREATE NOT PRESENT ATMS
            // ************************
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            string DateString;
            string result0;
            string Temp;
            string result1;
            string Vendor;
            string JournalName;
            string WAtmNo;
            string TempEjournalTypeId = "N/A";


            foreach (string file in allJournals)
            {
                DateString = file.Substring(file.Length - 19);
                DateString = DateString.Substring(0, 8);

                bool Condition = false;

                DateTime FileDATEresult;

                if (DateTime.TryParseExact(DateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out FileDATEresult))
                {

                }

                if (Condition == true)
                {
                    if (FileDATEresult > WCut_Off_Date)
                    {
                        MessageBox.Show("There are Journals with Date Creater than the Cycle date." + Environment.NewLine
                                         + "This is not allowed. Remove and restart." + Environment.NewLine
                                         + "Journal is: " + file
                                        );
                        return;
                    }
                }

                result0 = file.Substring(file.Length - 11);
                Temp = result0.Substring(0, 4);
                if (Temp == "_EJ_")
                {
                    // Valid
                    result1 = file.Substring(file.Length - 7);
                    Vendor = result1.Substring(0, 3);
                    JournalName = file.Substring(file.Length - 28);
                    WAtmNo = JournalName.Substring(0, 8);
                    // 00000102_20191024_EJ_DBL.000
                    Ac.ReadAtm(WAtmNo);
                    if (Ac.RecordFound == true)
                    {
                        // DO NOTHING WE WILL UPDATE JOURNAL LATER
                        //// ATM Found
                        //if (Vendor == "NCR") TempEjournalTypeId = "NCR_01";
                        //if (Vendor == "DBL") TempEjournalTypeId = "DBLD_01";
                        //if (Vendor == "WCR") TempEjournalTypeId = "Wincor_01";

                        //if (TempEjournalTypeId != Ac.EjournalTypeId)
                        //Ac.UpdateEjournalTypeId(WAtmNo, TempEjournalTypeId);
                    }
                    else
                    {
                        // Insert ATM_No
                        Ac.CreateNewAtmBasedOnGeneral_Model(WOperator, WAtmNo, JournalName);
                    }
                }
                else
                {
                    // Not Valid 
                }

            }


            //************************************
            //************************************
            // TRUNCATE Temp Table 
            // Do it every morning 
            //  RRDMJournalReadTxns_Text_Class Jc = new RRDMJournalReadTxns_Text_Class();
            string WFile = "[ATM_MT_Journals_AUDI].[dbo].[tblHstEjText_Short]";
            Jc.TruncateTempTable(WFile);


            string ParamId;

            // Check Service status

            string ServiceId = "10"; // For Journals

            bool Available = ServiceAvailableStatus(ServiceId);

            if (Available == true)
            {
                //*************************************************************
                //
                // CALL STORE PROCEDURE TO Start_CMD_Shell and AFter start service
                // 
                //string connectionString_AUDI = ConfigurationManager.ConnectionStrings
                //  ["JournalsConnectionString_AUDI"].ConnectionString;


                //string RCT = "ATM_MT_Journals_AUDI.[dbo].[stp_Start_CMD_Shell]";

                //using (SqlConnection conn =
                //   new SqlConnection(connectionString_AUDI))
                //    try
                //    {
                //        conn.Open();
                //        using (SqlCommand cmd =
                //           new SqlCommand(RCT, conn))
                //        {
                //            cmd.CommandType = CommandType.StoredProcedure;
                //            // Parameters
                //            cmd.CommandTimeout = 1800;  // seconds
                //            int rows = cmd.ExecuteNonQuery();

                //        }
                //        // Close conn
                //        conn.Close();
                //    }
                //    catch (Exception ex)
                //    {
                //        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                //    }
                // *************************************************************
                // Start Service
                //
                // Start Service
                //
                // Insert Command 
                Aq.ReqDateTime = DateTime.Now;
                Aq.RequestorID = WSignedId;
                Aq.RequestorMachine = Environment.MachineName;
                Aq.Command = "SERVICE_START_AND_MONITOR";
                Aq.ServiceId = ServiceId;
                // 
                ParamId = "915";

                Gp.ReadParameterByOccuranceId(ParamId, Aq.ServiceId);
                if (Gp.RecordFound == true)
                {
                    Aq.ServiceName = Gp.OccuranceNm;
                }
                else
                {
                    Aq.ServiceName = "Not Specified";
                }

                Aq.Priority = 0; // Highest
                Aq.Operator = WOperator;
                Aq.OriginalReqID = 0;
                Aq.OriginalRequestorID = WSignedId;

                WServiceReqID = Aq.InsertNewRecordInAgentQueue();

            }
            else
            {
                // If for example is "SERVICE_START_AND_MONITOR" then Journals are in loading process
                string msg = string.Format("Service was in status: {0}", serviceStatus + Environment.NewLine
                    + "Go to Task Manager Service section and start 'RRDM Agent' "
                    );
                MessageBox.Show(msg);

                return;
            }

            J_UnderLoading = true;

            Rjc.ReadReconcJobCyclesById(WOperator, WReconcCycleNo);

            Rjc.SpareBool_1 = true; // Journal Loading strts
            Rjc.SpareInt_1 = WServiceReqID; // Requested service

            Rjc.UpdateSpecialFields(WReconcCycleNo);

            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfJournals";
            Message = "Loading Of Journals starts -Cycle:.." + WReconcCycleNo.ToString() + " - Request Id " + WServiceReqID.ToString();
            DateTime J_SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", J_SavedStartDt, J_SavedStartDt, Message, WSignedId, WReconcCycleNo);
            //******************************
            // Sets the timer interval to 5 seconds.
            //timerJournals.Interval = 10000;
            // timerJournals.Start();

            //// STOP SERVICE NEW INSERT ???
            //// ???? 


            //Aq.ReqDateTime = DateTime.Now;
            //Aq.RequestorID = WSignedId;
            //Aq.RequestorMachine = Environment.MachineName;
            //Aq.Command = "SERVICE_STOP";
            //Aq.ServiceId = ServiceId;
            //// 
            //ParamId = "915";

            //Gp.ReadParameterByOccuranceId(ParamId, Aq.ServiceId);
            //if (Gp.RecordFound == true)
            //{
            //    Aq.ServiceName = Gp.OccuranceNm;
            //}
            //else
            //{
            //    Aq.ServiceName = "Not Specified";
            //}

            //Aq.Priority = 0; // Highest
            //Aq.Operator = WOperator;

            //WServiceNo = Aq.InsertNewRecordInAgentQueue();


        }
        // Service status
        string serviceStatus;
        private bool ServiceAvailableStatus(string InServiceId)
        {

            // Check Status Of Last Service
            bool ServiceAvailable = false;

            RRDMAgentQueue Aq = new RRDMAgentQueue();

            string WServiceId = InServiceId; // 

            // Find Status
            Aq.ReqDateTime = DateTime.Now;
            Aq.RequestorID = WSignedId;
            Aq.RequestorMachine = Environment.MachineName;
            Aq.Command = "SERVICE_STATUS";
            Aq.ServiceId = WServiceId;
            // 
            string ParamId = "915";

            Gp.ReadParameterByOccuranceId(ParamId, Aq.ServiceId);
            if (Gp.RecordFound == true)
            {
                Aq.ServiceName = Gp.OccuranceNm;
            }
            else
            {
                Aq.ServiceName = "Not Specified";
            }

            Aq.Priority = 0; // Highest
            Aq.Operator = WOperator;

            Aq.OriginalReqID = 0;
            Aq.OriginalRequestorID = WSignedId;

            WServiceReqID = Aq.InsertNewRecordInAgentQueue();
            string SelectionCriteria = " WHERE ReqID = " + WServiceReqID;
            int retries = 10;
            bool agentError = false;
            serviceStatus = "No response. Check RRDM Agent too.";
            do
            {
                Thread.Sleep(1000); // milliseconds=1 sec
                Aq.ReadAgentQueueBySelectionCriteria(SelectionCriteria);
                if (Aq.RecordFound == true)
                {
                    if (Aq.ReqStatusCode == AgentStatus.Req_Finished)
                    {
                        serviceStatus = Aq.CmdStatusMessage;
                        if (Aq.CmdStatusCode == AgentProcessingResult.Cmd_Success)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    agentError = true;
                    break;
                }
                retries--;
            }
            while (retries >= 0);

            if (agentError == true)
            {
                MessageBox.Show("Service with name.." + Aq.ServiceName
                                                   + Environment.NewLine
                                                   + "Is OutStanding " + Environment.NewLine
                                                   + "With ReqStatusCode" + Aq.ReqStatusCode + Environment.NewLine
                                                   + "AND " + Environment.NewLine
                                                   + "With CmdStatusCode" + Aq.CmdStatusCode + Environment.NewLine
                                                   );
                //  return;

            }

            if (serviceStatus == "Stopped")
            {
                //OK
                //  
                ServiceAvailable = true;
            }
            else
            {
                ServiceAvailable = false;

            }
            return ServiceAvailable;
        }

        //    int Counter = 10;
        bool JournalsLoaded;
        bool FilesLoaded;
        bool CategoriesAreMatched;
        // TIMER
        // TIMER 
        // TIMER
        private void timerJournals_Tick(object sender, EventArgs e)
        {
            return;
            // Check Progress BAR 
            try
            {
                //timerJournals.Stop();
                //return; 
                if (StopTimer == true)
                {
                    return;
                }

                string SourceFileId = "Atms_Journals_Txns";

                Rs.ReadReconcSourceFilesByFileId(SourceFileId);

                string InSourceDirectory = Rs.SourceDirectory;

                string[] allFiles = Directory.GetFiles(InSourceDirectory, "*.*");

                textBox1.Text = allFiles.Length.ToString();

                RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();

                Rfm.GetTotals(WReconcCycleNo);

                textBox2.Text = Rfm.ValidTotalForCycle.ToString();
                //     textBox2.Text = Counter.ToString();
                textBox3.Text = Rfm.InValidTotalForCycle.ToString();

                if (allFiles == null || allFiles.Length == 0)
                {
                    //   timerJournals.Stop();

                    JournalsLoaded = true;
                }
                if (Rs.TotalReady > 0)
                {
                    textBoxInDirForLoading.Text = Rs.TotalReady.ToString();

                }
                else
                {
                    FilesLoaded = true;
                }

                if (MatchedPressed == true)
                {

                    Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                               WReconcCycleNo);
                    //  textBoxMatchedCateg.Text = Mt.ENQ_CategForMatch;  

                    if (Mt.ENQ_NumberOfCatToBeMatched == 0)
                    {
                        CategoriesAreMatched = true;
                    }
                }

                Form502_Load(this, new EventArgs());

                // Close Service 
            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }
        }
        // DO MATCHING BASED ON THE READY CATEGORIES
        bool StopTimer;
        bool MatchedPressed = false;
        private void buttonDoMatching_Click(object sender, EventArgs e)
        {

            // Start Service
            buttonMatching.Show();
            // Find the ready categories

            Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                     WReconcCycleNo);

            if (MessageBox.Show("Do you want to Start Matching?" + Environment.NewLine
                                 + "For Ready Categories " + Environment.NewLine
                                 + Mt.W_MPComment
                                 , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                // YES Proceed

                //*******************************
                //Mode = 5; // Updating Action 
                //ProcessName = "MatchingProcess";
                //Message = "Matching Process Starts. ";
                //SavedStartDt = DateTime.Now;

                //Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
                //******************************

                // DELETE UNWANTED RECORDS FROM TABLES 
                RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
                DateTime DeleteDate = WFirstCut_Off_Date; // Trans before this date will be deleted
                MessageBox.Show("Transactions will be deleted prior to..Date.." + DeleteDate.ToShortDateString());
                Lf_AUD.DeleteRecordsToSetStartingPoint(WOperator, DeleteDate, WCut_Off_Date, WReconcCycleNo);
            }
            else
            {
                // Stop 
                return;
            }

            textBoxMsgBoard.Text = "Current Status : Matching Process";
            // FIRST UPDATE TRACE
            //*****************************************************
            // UPDATE TRACES and other infor In Order to sychronise files and have journal lines during reconciliation
            //*****************************************************

            Lf_AUD.UpdateRecordsWithTraceAndOther(WOperator, WReconcCycleNo, 0, 2, WCut_Off_Date);

            //
            // Before start we check the sign on users.
            //
            bool IsAllowedToSignIn = false;
            bool ThereAreUsersInSystem = CheckForSignInUsers(IsAllowedToSignIn);

            if (ThereAreUsersInSystem == true)
            {
                // Decide whether to move forward or not. 
            }

            // Clear Tables 
            RRDMAtmsMinMax Mm = new RRDMAtmsMinMax();
            Mm.DeleteTableATMsMinMax();

            MessageBox.Show("Matching Starts Now");

            //Thread thr16 = new Thread(Method16);
            //thr16.Start();
            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "MatchingProcess";
            Message = "Matching Process Starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
            //******************************


            int WMode = 2; // Do matching for all ready categories 
            //
            Mt.MatchReadyCategoriesUpdate(WOperator, WSignedId,
                                           WReconcCycleNo);
            // *****************************

            Mode = 5; // Updating Action 
            ProcessName = "MatchingProcess";
            Message = "Matching Process Finishes.";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************

            MatchedPressed = true;

            // UPDATE GL RECORDS 

            int WWMode;
            //Mgt.CreateRecords_CAP_DATE_For_Category(WOperator, WCut_Off_Date, WReconcCycleNo, WWMode);
            WWMode = 2;
            Mgt.CreateRecords_CAP_DATE_For_ATMs(WOperator, WCut_Off_Date, WReconcCycleNo, WWMode);
            // Check if already exists = Already updated for this cycle 

            // UPDATE Mpa with 818 where we Have EGP
            Mpa.UpdateMatchingTxnsMasterPoolATMsCurrency(WOperator);

            // SETTLEMENT DEPARTMENT 
            // Declare fields
            string WFileId = "";
            string WCategories = "";
            string WIdentity = "";

            RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM Cgl = new RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM();
            string WMatchingCateg = PRX + "210";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "Egypt_123_NET";
                // For Categories BDC210 and BDC211
                WCategories = "('" + PRX + "210','" + PRX + "211'" + ")"; // Issuer
                WIdentity = "123 TXNS_Bank_Is_Issuer";

                WWMode = 1;
                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);
            }

            WMatchingCateg = PRX + "215";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                WFileId = "Egypt_123_NET";
                // For Category BDC215
                WCategories = "('" + PRX + "215' )"; // Acquirer
                WIdentity = "123 TXNS_Bank_Is_Acquirer";
                WWMode = 2;
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

            }
            // ΜΕΕΖΑ
            // ΜΕΕΖΑ
            //
            WMatchingCateg = PRX + "270";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "MEEZA_OTHER_ATMS";
                // For Categories BDC270 and BDC271
                WCategories = "('" + PRX + "270','" + PRX + "271'" + ")"; // Issuer
                WIdentity = "MEEZA TXNS_Bank_Is_Issuer";
                WWMode = 4;
                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

            }

            WMatchingCateg = PRX + "272";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "MEEZA_POS";
                // For Categories BDC272 and BDC273
                WCategories = "('" + PRX + "272','" + PRX + "273'" + ")"; // Issuer POS
                WIdentity = "MEEZA POS TXNS_Bank_Is_Issuer";
                WWMode = 1;
                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

            }

            WMatchingCateg = PRX + "275";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "MEEZA_OWN_ATMS";
                // For Category BDC215
                WCategories = "('" + PRX + "275' )"; // Acquirer
                WIdentity = "MEEZA TXNS_Bank_Is_Acquirer";
                WWMode = 2;
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

            }

            // MASTER CARD - Bank Is Issuer
            // MASTER CARD
            //
            WMatchingCateg = PRX + "230";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "MASTER_CARD";
                // For Categories BDC230 and BDC232
                WCategories = "('" + PRX + "230','" + PRX + "232'" + ")"; // Issuer
                WIdentity = "MASTER TXNS_Bank_Is_Issuer";
                WWMode = 1;
                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

            }

            // MASTER CARD BANK is Acquirer
            // MASTER CARD
            //
            WMatchingCateg = PRX + "235";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "MASTER_CARD";
                // For Categories BDC235 and BDC236? 
                WCategories = "('" + PRX + "235' )"; // Acquirer
                WIdentity = "MASTER TXNS_Bank_Is_Acquirer";
                WWMode = 2;
                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);
            }

            // Credit Card
            // 
            //
            WMatchingCateg = PRX + "240";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "Credit_Card";
                // For Categories BDC240
                WCategories = "('" + PRX + "240' )"; // 
                WIdentity = "Credit Card Txns";
                WWMode = 3;
                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);
            }


            textBoxMsgBoard.Text = "Current Status : Moving Records Process";

            MessageBox.Show("Matching has Finished" + Environment.NewLine
                + "Process of Moving records starts."
                );

            Mode = 5; // Updating Action 
            ProcessName = "Moving Records To MATCHED Data Base";
            Message = "Moving Records Process Starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);

            //UPDATE Mpa Replenishment Cycle After Matching for the 01 and 011
            // *********************************

            Lf_AUD.UPDATE_Mpa_After_Matching_With_ReplCycle(WOperator, WReconcCycleNo);

            // Exclude presenter if so 
            //
            // Presenter
            bool Is_Presenter_InReconciliation = false;
            string ParId = "946";
            string OccurId = "1";
            RRDMGasParameters Gp = new RRDMGasParameters();

            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Presenter_InReconciliation = true;
            }
            else
            {
                // Exclude them 

                Mpa.ReadPoolAndFindTotals_Presenter_Unmatched(WOperator, WReconcCycleNo);
            }


            //******************************
            // 
            // MOVE MATCHED TXNS POOL 
            // stp_00_MOVE_TXNS_TO_MATCHED_DB_01_POOL
            //******************************
            string TotalProgressText = "Cycle: ..."
                                     + WReconcCycleNo.ToString() + Environment.NewLine;
            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            string WSelectionCriteria = " WHERE Operator = @Operator AND Enabled = 1";
            Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

            int I = 0;
            int K = 0;
            string WFileName;

            while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
            {
                //    RecordFound = true;
                int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                if (Mf.IsMoveToMatched == true || Mf.SourceFileId == "Atms_Journals_Txns") // the indication that this table is a moving table 
                {
                    if (Mf.SourceFileId == "Atms_Journals_Txns")
                    {
                        WFileName = "tblMatchingTxnsMasterPoolATMs";
                    }
                    else
                    {
                        WFileName = Mf.SourceFileId;
                    }

                    // MessageBox.Show("Start Moving file.." + WFileName); 

                    Cv.MOVE_ITMX_TXNS_TO_MATCHED(WFileName, WReconcCycleNo);

                    if (Cv.ret == 0)
                    {
                        // GOOD
                        K = K + 1;
                        TotalProgressText = TotalProgressText + Cv.ProgressText;
                    }
                    else
                    {
                        // NO GOOD
                        // public string ProgressText;
                        //public string ErrorReference;
                        //public int ret;
                        MessageBox.Show("VITAL SYSTEM ERROR" + Environment.NewLine
                                       + "PROGRESS TEXT.." + Cv.ProgressText + Environment.NewLine
                                       + "ERROR REFERENCE.." + Cv.ErrorReference + Environment.NewLine
                                       + ""
                                       );

                        return;

                    }

                }

                I = I + 1;
            }

            TotalProgressText = TotalProgressText + DateTime.Now + " Moving of TXNS to matched has finished" + Environment.NewLine;
            TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

            // *****************************

            Mode = 5; // Updating Action 
            ProcessName = "MovingRecordsProcess";
            Message = "MovingRecords Process Finishes.";

            textBoxMsgBoard.Text = "Current Status : Ready";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************

            //MessageBox.Show(TotalProgressText);

            // MATCHING HAS FINISHED 
            // ALLOW users to sign In. 
            //
            IsAllowedToSignIn = true;
            CheckForSignInUsers(IsAllowedToSignIn);

            if (ThereAreUsersInSystem == true)
            {
                // Decide whether to move forward or not. 
            }

            // AT the END UPDATE STATS

            // 
            // 
            string connectionStringITMX = ConfigurationManager.ConnectionStrings
             ["ReconConnectionString"].ConnectionString;

            string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[Stp_00_UPDATE_DB_System_Stats]";

            using (SqlConnection conn =
               new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }

            MessageBox.Show("Matching and movings of records process has finished");

            Form502_Load(this, new EventArgs());

        }
        //
        // LOAD FILES
        //
        string FullFileName;

        private void buttonLoadFiles_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Loading AUDI to Master STARTS");
            DateTime startTime = DateTime.Now;

            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2_Pambos2 PambosLoad = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2_Pambos2();

            PambosLoad.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo);

            DateTime endTime = DateTime.Now;

            TimeSpan span = endTime.Subtract(startTime);

            MessageBox.Show("Loading AUDI to Master FINISHES" + Environment.NewLine
                + "Time Elapsed In Minutes.." + span.TotalMinutes
                );



            CheckLoadingOfJournals();

            if (J_UnderLoading == true)
            {
                MessageBox.Show("Journals Under Loading");
                return;
            }
            // Before start we check the sign on users. 
            bool IsAllowedToSignIn = false;
            bool ThereAreUsersInSystem = CheckForSignInUsers(IsAllowedToSignIn);

            if (ThereAreUsersInSystem == true)
            {
                // Decide whether to move forward or not. 
            }

            // Truncate Table From Audi - tmplog for Pambos
            string WFile = "[ATM_MT_Journals_AUDI].[dbo].[tmplog]";
            Jc.TruncateTempTable(WFile);

            // Truncate Table working table 1 
            WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Working_General_Table]";

            Jc.TruncateTempTable(WFile);

            // Truncate Table 2 
            WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Working_Master_Pool_Report]";

            Jc.TruncateTempTable(WFile);

            bool Is_SuperisorMode = false;
            string ParId = "720";
            string OccurId = "1";
            //RRDMGasParameters Gp = new RRDMGasParameters();

            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_SuperisorMode = true;
            }
            else
            {
                Is_SuperisorMode = false;
            }

            if (Is_SuperisorMode == true)
            {
                //MessageBox.Show("Supervisor mode Work starts");
                textBoxMsgBoard.Text = "Current Status : Supervisor Mode data loading process";
                //
                // Supervisor mode
                //
                RRDMRepl_SupervisorMode_Master Smaster = new RRDMRepl_SupervisorMode_Master();
                int Sm_Mode = 3;
                Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, WSignRecordNo, WOperator,
                                                                 Sm_Mode, "NO_Form153");

               // MessageBox.Show("Supervisor mode Work Finishes");

            }

            // return; // for testing 

            MessageBox.Show("Loading of files starts." + Environment.NewLine
                            + "" + Environment.NewLine
                           );

            textBoxMsgBoard.Text = "Current Status : Loading of Files process";


            // CLEAR Duplicates due to NCR problems
            //*****************************************

            ParId = "103";
            OccurId = "1"; // 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                // Means delete duplicates from loaded journals due to NCR Vision problem 

                // Dublicates in A : 


                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

                Mpa.DeleteDuplicates_NCR_Vision(WReconcCycleNo);

                if (Mpa.Count > 0)
                {
                    MessageBox.Show("Deleted Duplicates due to NCR Vision problem." + Environment.NewLine
                             + "Number deleted..=" + Mpa.Count.ToString()
                              );
                }
                else
                {
                    MessageBox.Show("NO Duplicates FOUND due NCR Vision problem." + Environment.NewLine
                                    + "Maybe NCR has corrected problem." + Environment.NewLine
                                    + " Please check and report"
                                     );
                }

               
            }

            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfFiles";
            Message = "Loading Of Files starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
            //******************************
            // ************************
            // UPDATE CORRECT JOURNAL AND CREATE NOT PRESENT ATMS
            // ************************

            Jc.ReadJournal_tmpATMs_Journal_TypeAndUpdateAtms(WOperator);

            // Truncate Table 
            WFile = "[ATM_MT_Journals_AUDI].[dbo].[tmpATMs_Journal_Type]";

            Jc.TruncateTempTable(WFile);


            // 
            //IST 01 / 07

            // Source Files (Grid-ONE)
            string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1  ";

            Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

            SavedSourceFilesDataTable = Rs.Table_Files_In_Dir;

            buttonLoad.Show();

            // LOAD FILES
            //******************************
            //
            METHOD_LoadFiles();
            //
            //****************************
            string Temp = "Panicos"; //  Check IST 
            if (Temp == "Panicos")
            {

                string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]";
                string WCase = "Dublicate In IST";
                int WPos = 1;
                RRDMMatchingCategStageVsMatchingFields Mf = new RRDMMatchingCategStageVsMatchingFields();
                Mf.CreateStringOfMatchingFieldsForStageX(PRX + "999", "Stage A");
                string ListMatchingFields = Mf.Dublicate_List_FieldsStageX + ", FullDtTm";
                string OnMatchingFields = Mf.Dublicate_ON_FieldsStageX + " AND  y.FullDtTm = dt.FullDtTm";

                RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
                Mgt.FindDuplicateAddTableFromFileLoading(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

                if (Mgt.TableDublicates.Rows.Count > 1)
                {
                    Form78d_BasedOnDataTable NForm78d_DublicateRecords;
                    // textBoxFileId.Text
                    int WMode = 1; //

                    NForm78d_DublicateRecords = new Form78d_BasedOnDataTable(WOperator, WSignedId, "Switch_IST_Txns", Mgt.TableDublicates
                                                                                                                , WMode);
                    NForm78d_DublicateRecords.Show();
                }

            }

            Temp = "NO_Panicos2"; //  Check Authorisations
            if (Temp == "NO_Panicos2")
            {


                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
                Ap.FindDuplicateRecordsInAuth();

                if (Ap.TableDublicates_Auth.Rows.Count > 1)
                {
                    Form78d_BasedOnDataTable NForm78d_DublicateRecords;
                    // textBoxFileId.Text
                    int WMode = 3; //

                    NForm78d_DublicateRecords = new Form78d_BasedOnDataTable(WOperator, WSignedId, "Authorizations", Ap.TableDublicates_Auth
                                                                                                                , WMode);
                    NForm78d_DublicateRecords.Show();
                }
            }

            //
            // THE BELOW MUST BE HERE AFTER THE FILES ARE LOADED
            //
            // AUDI AUDI SECTION , SECTION 
            // 
            ParId = "945";
            OccurId = "2"; // 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {

                // DELETE UNWANTED RECORDS FROM TABLES FOR AUDI POC 
                RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
                DateTime DeleteDate = WFirstCut_Off_Date; // Trans before this date will be deleted
                MessageBox.Show("Transactions will be deleted prior to..Date.." + DeleteDate.ToShortDateString());
                Lf_AUD.DeleteRecordsToSetStartingPoint(WOperator, DeleteDate, WCut_Off_Date, WReconcCycleNo);
                //
                // UPDATE THE Stage A CIT Records with replenishment 
                //
                Lf_AUD.UpdateCIT_FirstStageRecords_AUDI(WOperator); 


                // HERE WE CREATE THE COMBINED RECORDS FOR REPLENISHMENT 
                RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

                int ExcelCycle = 0;

                G4.Create_Compined_Records_AndTXNS_For_Each_ATM_SM_Loaded(WOperator, WSignedId, WReconcCycleNo, ExcelCycle);
            }


            //
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfFiles";
            Message = "Loading Of Files Finishes. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************

            MessageBox.Show("Loading Of Files has Finished");
            // LOADING HAS FINISHED 
            // ALLOW users to sign In. 
            //
            IsAllowedToSignIn = true;
            CheckForSignInUsers(IsAllowedToSignIn);

            if (ThereAreUsersInSystem == true)
            {
                // Decide whether to move forward or not. 
            }

            textBoxMsgBoard.Text = "Current Status : Ready";
            // Refresh 
            Form502_Load(this, new EventArgs());

            //Thread thr15 = new Thread(Method15);
            //thr15.Start();
        }


        // View Loaded Journals
        private void buttonViewFiles_Click(object sender, EventArgs e)
        {
            if (WReconcCycleNo == 0)
            {
                MessageBox.Show("RM Cycle No is zero!");
                return;
            }

            Form18_LoadedFilesStatus NForm18_LoadedFilesStatus;

            int Mode = 12; // ALL
            //WReconcCycleNo = 1296; 
            NForm18_LoadedFilesStatus = new Form18_LoadedFilesStatus(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WCut_Off_Date, Mode);
            NForm18_LoadedFilesStatus.ShowDialog();
        }
        //
        // UNDO MATCHING AND LOADING 
        //
        private void button2_Click(object sender, EventArgs e)
        {
            // PROCESS
            // 1) MOVE TRANSACTIONS FROM MATCHED TABLES TO ITMX 
            //                         (TXNS IN MATCHED ARE DELETED)  
            // 2) USING A STORE PROCEDURE DELETE AND UPDATE OF RRDM GENERAL TABLES 
            // 3) USE AN RRDM CLASS TO DELETE UPDATE TABLES IN ITMX eg Delete loaded records 

            if (MessageBox.Show("Do you want to UNDO Loading and Matching " + Environment.NewLine
                                 + "For this Cycle ...? " + Environment.NewLine
                                 + WReconcCycleNo.ToString()
                                 , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                // YES Proceed
                MessageBox.Show("This process might take few minutes. " + Environment.NewLine
                               + "Wait till a final message is shown. ");
            }
            else
            {
                // Stop 
                return;
            }

            textBoxMsgBoard.Text = "Current Status : Undo Matching and Loading Files process";
            //*******************************

            Mode = 5; // Updating Action 
            ProcessName = "UNDO_LoadingAndMatchingProcess";
            Message = "UNDO Matching and Loading of Files STARTS CYCLE.."+ WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
            // BEFORE UNDO 
            string TotalProgressText = DateTime.Now + "Moving TXNs from Matched to ITMX Starts" + Environment.NewLine;

            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            string WSelectionCriteria = " WHERE Operator = @Operator  AND Enabled = 1 ";
            Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);
            //
            // MOVE TRANSACTIONS FROM MATCHED TO ITMX
            //
            int I = 0;
            int K = 0;
            string WFileName;

            while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
            {
                //    RecordFound = true;
                int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                if (Mf.IsMoveToMatched == true || Mf.SourceFileId == "Atms_Journals_Txns") // the indication that this table is a moving table 
                {
                    if (Mf.SourceFileId == "Atms_Journals_Txns")
                    {
                        WFileName = "tblMatchingTxnsMasterPoolATMs";
                    }
                    else
                    {
                        WFileName = Mf.SourceFileId;
                    }

                    Cv.MOVE_MATCHED_TXNS_TO_ITMX(WFileName, WReconcCycleNo);

                    if (Cv.ret == 0)
                    {
                        // GOOD
                        K = K + 1;
                        TotalProgressText = TotalProgressText + Cv.ProgressText;

                    }
                    else
                    {
                        // NO GOOD
                        // public string ProgressText;
                        //public string ErrorReference;
                        //public int ret;
                        MessageBox.Show("VITAL SYSTEM ERROR" + Environment.NewLine
                                       + "PROGRESS TEXT.." + Cv.ProgressText + Environment.NewLine
                                       + "ERROR REFERENCE.." + Cv.ErrorReference + Environment.NewLine
                                       + ""
                                       );

                        return;

                    }

                }

                I = I + 1;
            }

            TotalProgressText = TotalProgressText + DateTime.Now + " MOVING OF TXNS FROM MATCHED HAS FINISHED" + Environment.NewLine;
            TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

           // MessageBox.Show(TotalProgressText);


            //******************************
            // UNDO FILES AFTER MOVING THEM FROM MATCHED TO
            //******************************

            // CALL STORE PROCEDURE TO UNDO THE REST
            // THESE ARE FILES LIKE Reconciliation Sessions etc. 

            int ReturnCode = -20;
            string ProgressText = "";
            string ErrorReference = "";
            int ret = -1;

            string connectionStringITMX = ConfigurationManager.ConnectionStrings
                 ["ReconConnectionString"].ConnectionString;

            string SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_UNDO_RMCYCLE_FILES_MATCHING]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", WReconcCycleNo));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 300;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }
            //
            // UNDO STATISTICS 
            Lf_AUD.HandleDailyStatisticsForAtms_UNDO(WReconcCycleNo);

            // UNDO Intbl_CIT_Bank_Repl_Entries and Posted Txns ( BOTH ) 
            Lf_AUD.HandleSM_Bank_Records_UNDO(WReconcCycleNo, WCut_Off_Date);
            //
            // 1. UNDO SesStatus Traces if -1 not available make the last one available
            // 2. Make PANICOS_SM_Table records as not processed for this cycle
            // 
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            // DELETE FIRST - LEAVE it here
            Ta.DeleteALL_during_Undo(WOperator, WReconcCycleNo);
            // THEN after delete work with the remains 
            Ta.ReadNotesSesionsByAndCorrectLast(WOperator);

            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();
            bool WRRDM_Processed = false;
            SM.Update_SM_RecordsForCycle(WReconcCycleNo, WRRDM_Processed);
            //
            // CALL RRDM CLASS TO DELETE AND ALSO UPDATE RECORDS OF THIS CYCLE 
            //
            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

            Msf.ReadFilesAND_UNDO_For_Cycle(WOperator, WReconcCycleNo);
            //
            // DELETE THIS THAT IS not taking care from above
            // THESE ARE THE LEFT OVERS FROM ABOVE
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //
            // NEW SWITCH IST 
            //  
            string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo].[BULK_Switch_IST_Txns_ALL_2]";
            Bio.UNDO_Table_For_Cycle_Delete_Loaded_Only(PhysicalName, WReconcCycleNo);
            //
            // NEW NCR FOREX_CHILD 
            //
            PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_NCR_FOREX_CHILD_ALL";
            Bio.UNDO_Table_For_Cycle_Delete_Loaded_Only(PhysicalName, WReconcCycleNo);

            ProgressText = ProgressText + Msf.ProgressText_2;
            // UNDO the files from the Directories


            //RRDMJournalReadTxns_Text_Class Ed = new RRDMJournalReadTxns_Text_Class();
            RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

            Ed.LoopFor_MoveFile_From_Archive_ToOrigin_Directory(WOperator, WReconcCycleNo, ReversedCut_Off_Date);

            // *****************************

            Mode = 5; // Updating Action 
            ProcessName = "UNDO_LoadingAndMatchingProcess";
            Message = "UNDO Matching and Loading of Files FINISHES. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************


            if (ret == 0)
            {

                // OK
                //MessageBox.Show("VALID CALL" + Environment.NewLine
                //            + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("NOT VALID CALL" + Environment.NewLine
                         + ProgressText);
            }

            textBoxMsgBoard.Text = "Current Status : Ready";

            // AT the END UPDATE STATS

            string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[Stp_00_UPDATE_DB_System_Stats]";

            using (SqlConnection conn =
               new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }

            Form502_Load(this, new EventArgs());
        }
        //
        // METHOD LOAD FILES 
        //
        int FlogSeqNo;
        private void METHOD_LoadFiles()
        {
            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();
            RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();

            // Start Services 
            // Based on Directories with Files start the services 
            try
            {

                int I = 0;

                while (I <= (SavedSourceFilesDataTable.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    string SourceFileId = (string)SavedSourceFilesDataTable.Rows[I]["SourceFileId"];
                    string FullFileName = (string)SavedSourceFilesDataTable.Rows[I]["FullFileName"];
                    bool IsPresent = (bool)SavedSourceFilesDataTable.Rows[I]["IsPresent"];
                    string IsGood = (string)SavedSourceFilesDataTable.Rows[I]["IsGood"];
                    string DateExpected = (string)SavedSourceFilesDataTable.Rows[I]["DateExpected"];
                    string HASHValue = (string)SavedSourceFilesDataTable.Rows[I]["HASHValue"];

                    // FOR DEALING WITH IST TEMPORARY PROBLEM

                    // Update with -1 = ready for Matched 

                    //if (SourceFileId == "Switch_IST_Txns")
                    //{
                    //    // DUE TO IST PROBLEM DO ALWAYS THIS
                    //    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne(SourceFileId, WReconcCycleNo);
                    //}

                    if (IsPresent == true & IsGood == "YES" & SourceFileId != "Atms_Journals_Txns")
                    {
                        // Check if still exist in Directory
                        if (Environment.UserInteractive)
                        {
                            if (IsGood == "YES")
                            {
                                // THIS IS GOOD TO LOAD
                                // THIS IS TEMPORARY - ALECOS WILL DO IT
                                //
                                // Check if already exists with same hash value with success 
                                //

                                Flog.GetRecordByFileHASH(HASHValue);
                                if (Flog.RecordFound)
                                {
                                    // FILE READ BEFORE
                                    MessageBox.Show("File read before under the name of: " + Environment.NewLine
                                        + Flog.ArchivedPath
                                        );
                                }

                                RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
                                Mf.ReadReconcSourceFilesByFileId(SourceFileId);
                                // If WORKING WITH SERVICE ALECOS WILL INSERT THIS 
                                Flog.SystemOfOrigin = Mf.SystemOfOrigin;
                                Flog.RMCycleNo = WReconcCycleNo;
                                Flog.SourceFileID = SourceFileId;
                                Flog.StatusVerbose = "";
                                Flog.FileName = FullFileName;
                                Flog.FileSize = 0;
                                Flog.DateTimeReceived = DateTime.Now;
                                Flog.DateExpected = WCut_Off_Date;
                                //Flog.DateOfFile = WCut_Off_Date.Date.Year + "-" + WCut_Off_Date.Date.Month+"-"+WCut_Off_Date.Date.Day;
                                Flog.DateOfFile = WCut_Off_Date.ToString("yyyy-MM-dd");

                                Flog.FileHASH = HASHValue;
                                Flog.LineCount = 999;
                                Flog.stpFuid = 0;
                                Flog.ArchivedPath = FullFileName;
                                Flog.ExceptionPath = "Exception Path";
                                Flog.Status = 0;

                                FlogSeqNo = Flog.Insert(); // WReconcCycleNo
                                                           // LOAD FILE 

                                Lf_AUD.InsertRecordsInTableFromTextFile_InBulk(WOperator, SourceFileId, FullFileName, Ms.InportTableName, Ms.Delimiter, FlogSeqNo);

                                // IT IS TEMPORARY 
                                // ALECOS WILL INSERT THIS

                                //if (Environment.UserInteractive)
                                //{

                                Flog.ReadLoadedFilesBySeqNo(FlogSeqNo);

                                Flog.StatusVerbose = "";

                                Flog.LineCount = Lf_AUD.stpLineCount;

                                Flog.stpReturnCode = Lf_AUD.stpReturnCode;
                                Flog.stpErrorText = Lf_AUD.stpErrorText;
                                Flog.stpReferenceCode = Lf_AUD.stpReferenceCode;

                                if (Lf_AUD.stpReturnCode == 0)
                                    Flog.Status = 1; // Success
                                else Flog.Status = 0; //Failure
                                // Update Flog
                                //Flog.Update(Lf_ABE.WFlogSeqNo);

                                Flog.Update(FlogSeqNo);

                                if (Flog.SourceFileID == "GL_Balances_Atms_Daily")
                                {
                                    // Do not find Max date
                                }
                                else
                                {
                                    Flog.Update_MAX_DATE(Flog.SourceFileID, FlogSeqNo, WReconcCycleNo);
                                }
                               

                                // Update with -1 = ready for Matched if File is good
                                if (Lf_AUD.stpReturnCode == 0)
                                {
                                    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne(SourceFileId, WReconcCycleNo);

                                    if (SourceFileId == "Switch_IST_Txns" || SourceFileId == "COREBANKING")
                                    {
                                        // LEAVE IT HERE to cover the Twin that are created from Switch_IST
                                        // Make Twin 
                                        Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("Switch_IST_Txns_TWIN", WReconcCycleNo);

                                    }

                                    if (SourceFileId == "NCR_FOREX")
                                    {
                                        // LEAVE IT HERE to cover if testing with only FOREX
                                        // Make Twin 
                                        Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("Switch_IST_Txns", WReconcCycleNo);

                                    }
                                }

                                int r = Lf_AUD.stpReturnCode;
                                string D = Lf_AUD.stpErrorText;
                            }

                        }

                    }

                    I++; // Read Next entry of the table 
                }

                // FIRST UPDATE TRACE
                //*****************************************************
                // UPDATE TRACES and other infor In Order to sychronise files and have journal lines during reconciliation
                //*****************************************************
                //MessageBox.Show("ALL Files Loaded");
                //MessageBox.Show("We Start update records with traces");
                Lf_AUD.UpdateRecordsWithTraceAndOther(WOperator, WReconcCycleNo, FlogSeqNo, 1, WCut_Off_Date);
                //
                // EXTRA FIELDS
                //MessageBox.Show("We Start extra fields");
                Lf_AUD.UpdateFiles_With_EXTRA(WOperator, WReconcCycleNo);
                //MessageBox.Show("We finish extra fields");

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

            // textBoxLoadedFiles.Hide();

        }
        // HASHING
        public class FileHASH
        {
            // Compute the file's hash.
            public static byte[] GetHashSha256(string filename)
            {
                byte[] hashval;
                // The cryptographic service provider.
                SHA256 Sha256 = SHA256.Create();

                using (FileStream stream = File.OpenRead(filename))
                {
                    hashval = Sha256.ComputeHash(stream);
                    stream.Close();
                    return (hashval);
                }
            }

            // Return a byte array as a sequence of hex values.
            public static string BytesToString(byte[] bytes)
            {
                string result = "";
                foreach (byte b in bytes) result += b.ToString("x2");
                return result;
            }

        }

        // Journal Totals
        private void JournalTotals()
        {

            string SourceFileId = "Atms_Journals_Txns";

            Rs.ReadReconcSourceFilesByFileId(SourceFileId);

            string InSourceDirectory = Rs.SourceDirectory;

            string[] allFiles = Directory.GetFiles(InSourceDirectory, "*.*");

            textBox1.Text = allFiles.Length.ToString();

            RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();

            Rfm.GetTotals(WReconcCycleNo);

            textBox2.Text = Rfm.ValidTotalForCycle.ToString();
            //     textBox2.Text = Counter.ToString();
            textBox3.Text = Rfm.InValidTotalForCycle.ToString();

            if (allFiles == null || allFiles.Length == 0)
            {
                //   timerJournals.Stop();

                JournalsLoaded = true;
            }
            if (Rs.TotalReady > 0)
            {
                textBoxInDirForLoading.Text = Rs.TotalReady.ToString();

            }
            else
            {
                FilesLoaded = true;
            }

            if (MatchedPressed == true)
            {

                Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                           WReconcCycleNo);
                //  textBoxMatchedCateg.Text = Mt.ENQ_CategForMatch;  

                if (Mt.ENQ_NumberOfCatToBeMatched == 0)
                {
                    CategoriesAreMatched = true;
                }
            }


        }

        // Refresh Total Entries
        private void buttonRefreshTotals_Click(object sender, EventArgs e)
        {
            GetTotals();
        }
        // GET TOTALS
        private void GetTotals()
        {
            Lf_AUD.GetTotals(WOperator, WReconcCycleNo);

            ShowGrid5();


        }

        // View Line Details
        private void buttonLineDetails_Click(object sender, EventArgs e)
        {
            Flog.ReadLoadedFilesBySeqNo(WSeqNoLoadedFile);

            MessageBox.Show(Flog.stpErrorText);

        }


        // View details 
        private void buttonViewRCS_Click(object sender, EventArgs e)
        {

            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WMatchedCat, WReconcCycleNo);
            MessageBox.Show(Rcs.MPComment);

        }

        // View All Lines Files
        private void buttonViewAllLinesFiles_Click(object sender, EventArgs e)
        {

            Flog.ReadLoadedFilesByCycleNumber_No_Journals(WReconcCycleNo);
            int Mode = 1;
            Form2_MessageContent NForm2_MessageContent;
            string WHeading = "TRAIL OF PROCESS FOR FILES LOADING AT CYCLE.." + WReconcCycleNo.ToString();
            NForm2_MessageContent = new Form2_MessageContent(WSignedId, WSignRecordNo, WOperator, WHeading, Flog.ALL_stpErrorText, WReconcCycleNo, Mode);
            NForm2_MessageContent.ShowDialog();
        }


        // View ALL Lines Matching
        private void buttonViewAllLinesMatching_Click(object sender, EventArgs e)
        {
            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            Rcs.ReadAllCommentsByCatAndRunningJobNo(WOperator, WReconcCycleNo);
            int Mode = 2;
            Form2_MessageContent NForm2_MessageContent;
            string WHeading = "TRAIL OF PROCESS FOR MATCHING AT CYCLE.." + WReconcCycleNo.ToString();
            NForm2_MessageContent = new Form2_MessageContent(WSignedId, WSignRecordNo, WOperator, WHeading, Rcs.AllMPComment, WReconcCycleNo, Mode);
            NForm2_MessageContent.ShowDialog();
        }

        // Invalid Journals 
        private void buttonERRORS_Click(object sender, EventArgs e)
        {
            Form18_LoadedFilesStatus NForm18_LoadedFilesStatus;

            int Mode = 0; // only the errors

            NForm18_LoadedFilesStatus = new Form18_LoadedFilesStatus(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WCut_Off_Date, Mode);
            NForm18_LoadedFilesStatus.ShowDialog();
        }
        // Not Looaded Journals 
        private void buttonNotLoaed_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Use this button after IST is loaded" + Environment.NewLine
                + "It shows all ATMs with transactions in IST but no journal"
                );

            Form18_LoadedFilesStatus NForm18_LoadedFilesStatus;

            int Mode = 15; // only the errors

            NForm18_LoadedFilesStatus = new Form18_LoadedFilesStatus(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WCut_Off_Date, Mode);
            NForm18_LoadedFilesStatus.Show();
        }
        // Show File 


        private void buttonNonProcessed_Click(object sender, EventArgs e)
        {
            string WMatchingCateg = comboBoxMatchingCateg.Text.Substring(0, 6);

            Form78d_FileRecords NForm78d_FileRecords;
            // textBoxFileId.Text
            int WMode = 1; //
                           // InMode = 1 : Not processed yet 
                           // InMode = 2 : Processed this Cycle
            if (radioButtonAll.Checked == true) WCategoryOnly = false;
            else WCategoryOnly = true;
            NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, textBoxFileId.Text, "", WReconcCycleNo, WMatchingCateg, WMode, WCategoryOnly);
            NForm78d_FileRecords.Show();
        }

        private void buttonProcessed_Click(object sender, EventArgs e)
        {
            string WMatchingCateg = comboBoxMatchingCateg.Text.Substring(0, 6);

            Form78d_FileRecords NForm78d_FileRecords;
            // textBoxFileId.Text
            int WMode = 2; //
                           // InMode = 1 : Not processed yet 
                           // InMode = 2 : Processed this Cycle
            if (radioButtonAll.Checked == true) WCategoryOnly = false;
            else WCategoryOnly = true;
            NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, textBoxFileId.Text, "", WReconcCycleNo, WMatchingCateg, WMode, WCategoryOnly);
            NForm78d_FileRecords.ShowDialog();
        }
        // Discrepancies
        private void buttonDiscrep_Click(object sender, EventArgs e)
        {
            // SHOW THE DISCREPANCIES
            //
            Form78d_Discre NForm78d_Discre;
            int WMode = 1; // Per category
            NForm78d_Discre = new Form78d_Discre(WOperator, WSignedId, textBoxCateg.Text, WReconcCycleNo, WMode, "");
            NForm78d_Discre.ShowDialog();

        }
        // Discrepancies All ATMS
        private void buttonDiscr_All_ATMS_Click(object sender, EventArgs e)
        {
            // SHOW THE DISCREPANCIES
            //
            Form78d_Discre NForm78d_Discre;
            int WMode = 2; // All ATMS from Master
            NForm78d_Discre = new Form78d_Discre(WOperator, WSignedId, textBoxCateg.Text, WReconcCycleNo, WMode, "");
            NForm78d_Discre.ShowDialog();
        }
        // Discrepancies per Category from master file  
        private void buttonDiscrCategory_Click(object sender, EventArgs e)
        {
            // SHOW THE DISCREPANCIES
            //
            Form78d_Discre NForm78d_Discre;
            int WMode = 3; // Per Category from Master
            NForm78d_Discre = new Form78d_Discre(WOperator, WSignedId, textBoxCateg.Text, WReconcCycleNo, WMode, "");
            NForm78d_Discre.ShowDialog();
        }
        // Matched 
        private void buttonMatched_Click(object sender, EventArgs e)
        {
            // SHOW MATCHED
            //
            Form78d_Matched NForm78d_Matched;
            int WMode = 4; // Per Category from Master
            NForm78d_Matched = new Form78d_Matched(WOperator, WSignedId, textBoxCateg.Text, WReconcCycleNo, WMode);
            NForm78d_Matched.ShowDialog();
        }
        // Radio Button Category
        bool WCategoryOnly;
        private void radioButtonCategory_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCategory.Checked == true)
            {
                WCategoryOnly = true;
                comboBoxMatchingCateg.Show();
            }

        }

        private void radioButtonAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAll.Checked == true)
            {
                WCategoryOnly = false;
                comboBoxMatchingCateg.Hide();

            }
        }
        // View Group Categories 
        private void buttonViewGroupCateg_Click(object sender, EventArgs e)
        {
            // SHOW MATCHED
            //
            Form78d_SlaveCategories NForm78d_SlaveCategories;
            int WMode = 5; // show slave 
            NForm78d_SlaveCategories = new Form78d_SlaveCategories(WOperator, WSignRecordNo, WSignedId, textBoxCateg.Text, WReconcCycleNo, WMode);
            NForm78d_SlaveCategories.ShowDialog();

        }

        // Link expand to show Categories 
        private void linkLabelExpand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // SHOW ALL Categories 
            //
            Form78d_SlaveCategories NForm78d_SlaveCategories;
            int WMode = 4; // show all categories  
            NForm78d_SlaveCategories = new Form78d_SlaveCategories(WOperator, WSignRecordNo, WSignedId, textBoxCateg.Text, WReconcCycleNo, WMode);
            NForm78d_SlaveCategories.ShowDialog();

        }

        // Show Matching Files 


        // Show Files 
        private void linkLabelShowFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool CategoryMatched = false;
            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, textBoxCateg.Text, WReconcCycleNo);
            if (Rcs.RecordFound == true)
            {
                //MatchingCat01 = (string)rdr["MatchingCat01"];
                //MatchingCat01Updated = (bool)rdr["MatchingCat01Updated"];
                if (Rcs.MatchingCat01 != "")
                {
                    if (Rcs.MatchingCat01Updated == true)
                    {

                        CategoryMatched = true;
                    }
                    else
                    {
                        CategoryMatched = false;
                    }
                }
            }
            if (CategoryMatched == false)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }

            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + textBoxCateg.Text;

            RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();

            string SelectionCriteria = " WHERE CategoryId ='" + textBoxCateg.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }
        // EXPORT TO EXCEL
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {

            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelPath = "C:\\RRDM\\Working\\Files_1" + ".xls";

            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Rs.Table_Files_In_Dir, WorkingDir, ExcelPath);
        }
        // UNLOAD JOURNALS
        private void buttonUNDO_Journals_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to UNDO Journals Loading  " + Environment.NewLine
                                + "For this Cycle ...? " + Environment.NewLine
                                + WReconcCycleNo.ToString()
                                , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {
                // YES Proceed
                //MessageBox.Show("This process will move the unloaded Journals to directory. " + Environment.NewLine
                //               + "");
            }
            else
            {
                // Stop 
                return;
            }

            Flog.ReadLoadedFilesByCycleNumber_All(WReconcCycleNo);
            if (Flog.RecordFound == true & Flog.Files_Total > 0)
            {
                MessageBox.Show("Files were loaded with this Cycle." + Environment.NewLine
                                + "You cannot UNDO the Journals" + Environment.NewLine
                               );
                return;
            }

            //if (MessageBox.Show("Do you want to UNDO The Loaded Journals " + Environment.NewLine
            //                    + "For this Cycle ...? " + Environment.NewLine
            //                    + WReconcCycleNo.ToString()
            //                    , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            //             == DialogResult.Yes)
            //{
            //    // YES Proceed
            //    MessageBox.Show("................. " + Environment.NewLine
            //                   + "Wait till a final message is shown. ");
            //}
            //else
            //{
            //    // Stop 
            //    return;
            //}

            textBoxMsgBoard.Text = "Current Status : Unloading Journals";

            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "UNDO_LoadingOfJournals";
            Message = "UNDO of Loading Of Journals STARTS.CYCLE.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
            //******************************

            int ReturnCode = -20;
            string ProgressText = "";
            string ErrorReference = "";

            string connectionStringITMX = ConfigurationManager.ConnectionStrings
                 ["ReconConnectionString"].ConnectionString;

            string SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_UNDO_RMCYCLE_JOURNALS]";
            int ret = 0;

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", WReconcCycleNo));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 1800;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {

                        // OK

                        // RRDMJournalReadTxns_Text_Class Ed = new RRDMJournalReadTxns_Text_Class();
                        RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

                        RRDMGasParameters Gp = new RRDMGasParameters();

                        string ParId = "920";
                        string OccurId = "12"; // C:\RRDM\FilePool\Atms_Journals_Txns\

                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                        string WDestination = Gp.OccuranceNm;

                        //MessageBox.Show("This is from 12 WDestination " + WDestination);  

                        ParId = "920";
                        OccurId = "11"; //C:\RRDM\Archives\Atms_Journals_Txns\

                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                        string WorkingDirectory = Gp.OccuranceNm + ReversedCut_Off_Date + "_" + WReconcCycleNo.ToString() + "\\";

                        //MessageBox.Show("This is from 11 WorkingDirectory " + WorkingDirectory);

                        if (Directory.Exists(WorkingDirectory))
                        {

                            string[] allFiles_2 = Directory.GetFiles(WorkingDirectory, "*.*");
                            if (allFiles_2.Length >= 0)
                            {
                                // bool InWithCopy, string InCopyDestination)
                                MessageBox.Show("Number of files in directory " + allFiles_2.Length);

                                bool WithCopy = true; // Copy back to the origin 

                                Ed.DeleteDirectoryWithCopy(WorkingDirectory, WithCopy, WDestination);

                                ProgressText += DateTime.Now + "_" + "Archive Directory Deleted with File Number..." + allFiles_2.Length.ToString() + "\r\n";
                            }
                        }
                        else
                        {
                            MessageBox.Show("This Directory does not exist." + Environment.NewLine
                                           + WorkingDirectory
                                            );
                        }

                        //
                        ParId = "920";
                        OccurId = "3"; // Exceptions

                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                        WorkingDirectory = Gp.OccuranceNm + ReversedCut_Off_Date + "_" + WReconcCycleNo.ToString() + "\\";

                        //MessageBox.Show("This is from 3 Exceptions " + WorkingDirectory);

                        if (Directory.Exists(WorkingDirectory))
                        {
                            string[] allFiles_3 = Directory.GetFiles(WorkingDirectory, "*.*");
                            if (allFiles_3.Length >= 0)
                            {

                                bool WithCopy = true;

                                Ed.DeleteDirectoryWithCopy(WorkingDirectory, WithCopy, WDestination);

                                ProgressText += DateTime.Now + "_" + "Archive Directory Deleted with File Number..." + allFiles_3.Length.ToString() + "\r\n";
                            }
                        }
                        else
                        {
                            MessageBox.Show("This Directory does not exist." + Environment.NewLine
                                           + WorkingDirectory
                                            );
                        }


                        MessageBox.Show("VALID CALL" + Environment.NewLine
                                       + ProgressText);
                    }
                    else
                    {
                        // NOT OK
                        MessageBox.Show("NOT VALID CALL" + Environment.NewLine
                                 + ProgressText);
                    }

                    // *****************************
                    Mode = 5; // Updating Action 
                    ProcessName = "UNDO_LoadingOfJournals";
                    Message = "UNDO of Loading Of Journals FINISHES. ";

                    Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
                    //*********************************

                    // Delete Archiving Directories

                    Form502_Load(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }

            }
            //
            // UPdate STatistics 
            //
            string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[Stp_00_UPDATE_DB_System_Stats]";

            using (SqlConnection conn =
               new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }
           

            textBoxMsgBoard.Text = "Current Status : Ready ";
        }
        // THe Bad and the Ugly 
        private void buttonBadAndUgly_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Show these results to Hassan please");
            Form78e NForm78e;

            int Mode = 1; // Show Good and Bad
            string InHeader = "THE GOOD AND THE BAD FOR CYCLE =.." + WReconcCycleNo.ToString();

            NForm78e = new Form78e(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WCut_Off_Date, InHeader, Mode);
            NForm78e.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
        // Show Comment
        private void buttonShowComment_Click(object sender, EventArgs e)
        {

            MessageBox.Show(WFileComment);
        }
        // View Gaps
        private void buttonViewGaps_Click(object sender, EventArgs e)
        {
            Form18_LoadedFilesStatus_GAPS NForm18_LoadedFilesStatus_GAPS;

            int Mode = 16; // only the GAPS 

            NForm18_LoadedFilesStatus_GAPS = new Form18_LoadedFilesStatus_GAPS(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WCut_Off_Date, Mode);
            NForm18_LoadedFilesStatus_GAPS.Show();
        }

        private void buttonLineDates_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            string MinDateTm = "";
            string MaxDateTm = "";
            Flog.ReadLoadedFilesBySeqNo(WSeqNoLoadedFile);
            string WFileName = Flog.SeqNo.ToString();
            DateTime MinDt = Mgt.ReadAndFindMinDtForFile(Flog.SourceFileID, WReconcCycleNo, WFileName);

            if (MinDt.Date == NullPastDate)
            {
                MinDateTm = "NO MIN";
            }
            else
            {
                MinDateTm = MinDt.ToString();
            }

            DateTime MaxDt = Mgt.ReadAndFindMaxDtForFile(Flog.SourceFileID, WReconcCycleNo, WFileName);


            if (MaxDt.Date == NullPastDate)
            {
                MaxDateTm = "NO MAX";
            }
            else
            {
                MaxDateTm = MaxDt.ToString();
            }

            MessageBox.Show("File..:_" + Flog.SourceFileID + Environment.NewLine
                            + "MinDateTm..:_" + MinDateTm + Environment.NewLine
                            + "MaxDateTm..:_" + MaxDateTm + Environment.NewLine
                            );
        }

        //
        private void buttonTest_Click_1(object sender, EventArgs e)
        {
            //// UPDATE TOTALS
            //string WMatchingCateg = PRX + "210";

            //int WWMode = 3; // Create GL Entries for Egypt 123 say  
            //Mgt.CreateRecords_GL_ENTRIES_For_Category(WOperator, WMatchingCateg, WCut_Off_Date, WReconcCycleNo, WWMode);

            //MessageBox.Show("TEST OF CAP_DATE Finished" + Environment.NewLine
            //    + ""
            //    );
        }
        // Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (WMode == 2)
            {

            }
            else
            {
                CheckLoadingOfJournals();

                if (J_UnderLoading == true)
                {
                    textBoxMsgBoard.Text = "Current Status:Loading of Journals process";
                }
                else
                {
                    textBoxMsgBoard.Text = "Current Status:Ready";
                }
            }


            // WRowIndex = dataGridViewFiles.SelectedRows[0].Index;

            Form502_Load(this, new EventArgs());

            //   dataGridViewFiles.Rows[WRowIndex].Selected = true;
            //   dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

        }
        // UNDO File This Cycle 
        int ReturnCode;
        string ProgressText;
        //string ErrorReference;
        int ret;
        private void buttonUndoFile_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Avoid using this functionality." + Environment.NewLine
                + "Contact RRDM for further information"
                );
            return;

            if (WFileId == "Atms_Journals_Txns"
                || WFileId == "Switch_IST_Txns"
                  || WFileId == "COREBANKING"
                )
            {
                MessageBox.Show("This File You cannot Undo Alone" + Environment.NewLine
                    + "May you want to undo the whole cycle? "
                    );
                return;
            }

            // PROCESS
            // 1) MOVE TRANSACTIONS FROM MATCHED TABLES TO ITMX 
            //                         (TXNS IN MATCHED ARE DELETED)  
            // 2) USING A STORE PROCEDURE DELETE AND UPDATE OF RRDM GENERAL TABLES 
            // 3) USE AN RRDM CLASS TO DELETE UPDATE TABLES IN ITMX eg Delete loaded records 
            int WorkingRMCycle;
            if (int.TryParse(textBoxCycle.Text, out WorkingRMCycle))
            {

            }
            else
            {
                MessageBox.Show(textBoxCycle.Text, "Please enter for RM Cycle!");

                return;
            }

            if (MessageBox.Show("Do you want to UNDO Loading and Matching " + Environment.NewLine
                                 + "For this File ...? " + WFileId + Environment.NewLine
                                 + "For this Cycle ...? " + WorkingRMCycle.ToString()
                                 , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                // CHECK IF IT IS THE LATEST 
                RRDMReconcFileMonitorLog Fl = new RRDMReconcFileMonitorLog();
                Fl.ReadLoadedFilesAND_Find_The_Latest(WFileId);

                if (WorkingRMCycle == Fl.RMCycleNo)
                {
                    // This is OK. 
                }
                else
                {
                    MessageBox.Show("You try to Undo not the latest file" + Environment.NewLine
                        + "You can Undo only the latest" + Environment.NewLine
                        + "The latest is in Cycle.." + Fl.RMCycleNo.ToString() + Environment.NewLine
                        + "Once you do this you can undo the next latest"
                        );

                    return;
                }
                // YES Proceed
                MessageBox.Show("UNDO starts. " + Environment.NewLine
                               + "For File.." + WFileId);
            }
            else
            {
                // Stop 
                return;
            }

            //*******************************

            Mode = 5; // Updating Action 
            ProcessName = "UNDO_LoadingAndMatchingProcess - For A File";
            Message = "UNDO Matching and Loading of File STARTS. ";
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
            // BEFORE UNDO 
            string TotalProgressText = DateTime.Now + "Moving TXNs from Matched to ITMX Starts" + Environment.NewLine;

            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();

            //
            // Move by name like WFileId = Egypt 123
            //
            string TempFiled = WFileId;
            // Move them all
            Cv.MOVE_MATCHED_TXNS_TO_ITMX(TempFiled, WorkingRMCycle);
            // 

            TotalProgressText = TotalProgressText + DateTime.Now + " MOVING OF TXNS FROM MATCHED HAS FINISHED" + Environment.NewLine;
            //TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;


            RRDMMatchingCategoriesVsSourcesFiles Msvf = new RRDMMatchingCategoriesVsSourcesFiles();

            string WSelectionCriteria = " SourceFileName ='" + WFileId + "'";
            Msvf.ReadReconcCategoryVsSourcesANDFillTable(WSelectionCriteria);

            int I = 0;
            int K = 0;
            // ONE FILE ONLY
            while (I <= (Msvf.RMCategoryFilesDataFiles.Rows.Count - 1))
            {
                //    //    RecordFound = true;
                int SeqNo = (int)Msvf.RMCategoryFilesDataFiles.Rows[I]["SeqNo"];
                Msvf.ReadReconcCategoriesVsSourcebySeqNo(SeqNo);

                string WCategoryId = Msvf.CategoryId;

                // Move the records by Category and make the records active 
                Msvf.ReadReconcCategoriesVsSourcesAll(WCategoryId);

                // Transactions are moved only 
                TempFiled = "tblMatchingTxnsMasterPoolATMs";
                Cv.MOVE_MATCHED_TXNS_TO_ITMX_By_Category(TempFiled, WorkingRMCycle, WCategoryId);

                if (Msvf.SourceFileNameB == "Switch_IST_Txns_TWIN")
                {
                    // TRANSACTIONS ARE MOVED AND ARE MARKED AS NOT PROCESSED
                    TempFiled = "Switch_IST_Txns_TWIN";
                    Cv.MOVE_MATCHED_TXNS_TO_ITMX_By_Category(TempFiled, WorkingRMCycle, WCategoryId);
                }
                if (Msvf.SourceFileNameB == "Switch_IST_Txns")
                {
                    TempFiled = "Switch_IST_Txns";
                    Cv.MOVE_MATCHED_TXNS_TO_ITMX_By_Category(TempFiled, WorkingRMCycle, WCategoryId);
                }

                //stp_00_UNDO_RMCYCLE_FILES_MATCHING(WReconcCycleNo, Msvf.CategoryId, TempFiled);
                // 
                if (Msvf.SourceFileNameC == "COREBANKING")
                {
                    TempFiled = "COREBANKING";
                    Cv.MOVE_MATCHED_TXNS_TO_ITMX_By_Category(TempFiled, WorkingRMCycle, WCategoryId);
                }

                //stp_00_UNDO_RMCYCLE_FILES_MATCHING(WReconcCycleNo, Msvf.CategoryId, TempFiled);

                //******************************
                // UNDO RECONCILIATION MATCHING Records AFTER MOVING THEM FROM MATCHED TOO
                // 
                // FILE id is needed only to be deleted from "ReconcFileMonitorLog"
                //
                //******************************
                TempFiled = WFileId;
                Cv.stp_00_UNDO_RMCYCLE_FILES_MATCHING(WorkingRMCycle, WCategoryId, TempFiled);

                I = I + 1;
            }

            //
            // UNDO STATISTICS 
            //Lf.HandleDailyStatisticsForAtms_UNDO(WReconcCycleNo);
            //
            // CALL RRDM CLASS TO DELETE AND ALSO UPDATE RECORDS OF THIS CYCLE 
            //
            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

            // THE below to be done only for main file which is 123 
            // Not to be done for IST or Flexcube. 
            // It Deletes records of 123
            Msf.ReadFilesAND_UNDO_For_Cycle_File(WOperator, WorkingRMCycle, WFileId);

            ProgressText = ProgressText + Msf.ProgressText_2;
            // UNDO the files from the Directories

            string U_ReversedCut_Off_Date = "";

            Rjc.ReadReconcJobCyclesById(WOperator, WorkingRMCycle);

            if (Rjc.RecordFound == true)
            {
                DateTime U_WCut_Off_Date = Rjc.Cut_Off_Date.Date;

                U_ReversedCut_Off_Date = U_WCut_Off_Date.ToString("yyyyMMdd");

                //WNumberOfLoadingAndMatching = Rjc.NumberOfLoadingAndMatching;
            }

            //RRDMJournalReadTxns_Text_Class Ed = new RRDMJournalReadTxns_Text_Class();
            RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

            Ed.MoveFile_From_Archive_ToOrigin_Directory(WOperator, WorkingRMCycle, U_ReversedCut_Off_Date, WFileId);

            // *****************************

            Mode = 5; // Updating Action 
            ProcessName = "UNDO_LoadingAndMatchingProcess";
            Message = "UNDO Matching and Loading of Files FINISHES. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************

            //MessageBox.Show(TotalProgressText);

            Form502_Load(this, new EventArgs());
        }
        ////
        //// Store procedure to Undo files from Matching
        ////
        //private void stp_00_UNDO_RMCYCLE_FILES_MATCHING(int InReconcCycleNo, string InCategoryId, string InFileId)
        //{
        //    // CALL STORE PROCEDURE TO UNDO THE REST
        //    // THESE ARE FILES LIKE Reconciliation Sessions etc. 

        //    ReturnCode = -20;
        //    ProgressText = "";
        //    ErrorReference = "";
        //    ret = -1;

        //    string connectionStringITMX = ConfigurationManager.ConnectionStrings
        //         ["ReconConnectionString"].ConnectionString;

        //    string SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_UNDO_RMCYCLE_FILES_MATCHING_CATEGORY_ONLY]";

        //    using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
        //    {
        //        try
        //        {

        //            conn2.Open();

        //            SqlCommand cmd = new SqlCommand(SPName, conn2);

        //            cmd.CommandType = CommandType.StoredProcedure;

        //            // the first are input parameters

        //            cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

        //            cmd.Parameters.Add(new SqlParameter("@CategoryId", InCategoryId));

        //            cmd.Parameters.Add(new SqlParameter("@SourceFileID", InFileId));

        //            // the following are output parameters

        //            SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
        //            retCode.Direction = ParameterDirection.Output;
        //            retCode.SqlDbType = SqlDbType.Int;
        //            cmd.Parameters.Add(retCode);

        //            SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
        //            retProgressText.Direction = ParameterDirection.Output;
        //            retProgressText.SqlDbType = SqlDbType.NVarChar;
        //            retProgressText.Size = 3000;
        //            cmd.Parameters.Add(retProgressText);

        //            SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
        //            retErrorReference.Direction = ParameterDirection.Output;
        //            retErrorReference.SqlDbType = SqlDbType.NVarChar;
        //            retErrorReference.Size = 3000;
        //            cmd.Parameters.Add(retErrorReference);

        //            // execute the command
        //            cmd.CommandTimeout = 300;  // seconds
        //            cmd.ExecuteNonQuery(); // errors will be caught in CATCH

        //            ret = (int)cmd.Parameters["@ReturnCode"].Value;
        //            ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

        //            conn2.Close();

        //        }
        //        catch (Exception ex)
        //        {
        //            conn2.Close();
        //            CatchDetails(ex);
        //        }
        //    }
        //}
        // Gaps Not Come yet 
        private void buttonGapsNotComeYet_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please use the button after the Matching of this Cycle");

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            int DB_Mode = 1;
            Mpa.ReadNotLoadedYet(WReconcCycleNo, DB_Mode);

            if (Mpa.TableFullFromMaster.Columns.Count > 1)
            {
                Form78d_BasedOnDataTable NForm78d_BasedOnDataTable;
                // textBoxFileId.Text
                int WMode = 2; //

                string TableId = " Master Pool ";

                NForm78d_BasedOnDataTable = new Form78d_BasedOnDataTable(WOperator, WSignedId, TableId, Mpa.TableFullFromMaster
                                                                                                            , WMode);
                NForm78d_BasedOnDataTable.Show();
            }
        }

        // Catch Details
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append(WindowsIdentity.GetCurrent().Name);
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
        // Performance link Journals

        private void linkLabelPerf_Journals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //ProcessNm IN ('LoadingOfJournals' , 'LoadingOfFiles', 'MatchingProcess')
            Form8 NForm8;
            string WCriticalProcess = "LoadingOfJournals";
            int WAction = 1;
            NForm8 = new Form8(WSignedId, WOperator, WCriticalProcess, WAction);
            NForm8.ShowDialog();
        }
        // Performance link files
        private void linkLabelPerf_Files_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //ProcessNm IN ('LoadingOfJournals' , 'LoadingOfFiles', 'MatchingProcess')
            Form8 NForm8;
            string WCriticalProcess = "LoadingOfFiles";
            int WAction = 1;
            NForm8 = new Form8(WSignedId, WOperator, WCriticalProcess, WAction);
            NForm8.ShowDialog();
        }
        // Performance link matching
        private void linkLabelPerf_Matching_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //ProcessNm IN ('LoadingOfJournals' , 'LoadingOfFiles', 'MatchingProcess')
            Form8 NForm8;
            string WCriticalProcess = "MatchingProcess";
            int WAction = 1;
            NForm8 = new Form8(WSignedId, WOperator, WCriticalProcess, WAction);
            NForm8.ShowDialog();
        }

        private bool CheckForSignInUsers(bool InIsAllowedToSignIn)
        {
            // CHECK IF SIGN ON USERS 
            RRDMUserSignedInRecords Usr = new RRDMUserSignedInRecords();
            // if InIsAllowedToSignIn = false = not allowed 
            // Then check if Users exce[pt the 08 security which is the controller is already in system
            // If in System then you ask them to sign out 
            // And then you continue to stop users not sign in 
            // *************************
            // if InIsAllowedToSignIn = true = allowed 
            // You just set up the parameter to NO

            bool ThereAreUsersInSystem = false;

            if (InIsAllowedToSignIn == false)
            {

                Usr.ReadOpenUsers(WOperator);

                if (Usr.OpenUsersSelected.Rows.Count > 0)
                {
                    MessageBox.Show("There are open Users in the system." + Environment.NewLine
                        + "For this operation only the Controller is opened" + Environment.NewLine
                        + "Ask the open users to sign off."
                        );

                    Form78e NForm78e;

                    int Mode = 2; // SIGNED IN USERS
                    string InHeader = "THE CURRENTLY SIGNED IN USERS";

                    NForm78e = new Form78e(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WCut_Off_Date, InHeader, Mode);
                    NForm78e.ShowDialog();

                    ThereAreUsersInSystem = true;
                }

            }


            if ((InIsAllowedToSignIn == false & ThereAreUsersInSystem == false)
                || (InIsAllowedToSignIn == true)
                )
            {
                //
                // SET IT
                // UPDATE
                //
                string ParId = "105";
                string OccurId = "1";
                RRDMGasParameters Gp = new RRDMGasParameters();

                string IsYESNO;

                if (InIsAllowedToSignIn == true)
                {
                    IsYESNO = "YES";
                }
                else
                {
                    IsYESNO = "NO";
                }
                // Update Occurance 
                Gp.UpdateGasParamByParamIdAndOccur(WOperator, ParId, OccurId, IsYESNO);

                // Check and get current value
                bool IsAllowedToSignIn = true;
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                if (Gp.OccuranceNm == "YES")
                {
                    IsAllowedToSignIn = true;
                }
                else
                {
                    IsAllowedToSignIn = false;
                }


            }

            return ThereAreUsersInSystem;

        }
        // NOT IN IST
        private void buttonNotInIst_Click(object sender, EventArgs e)
        {
            MessageBox.Show("YOU USE THIS BUTTON ONLY AFTER IST LOADING AND BEFORE MATCHING." + Environment.NewLine
                            + "IT DOES NOT SHOW CORRECT RESULTS AFTER MATCHING BECAUSE RECORDS ARE MOVED FROM ONE DATA BASE TO ANOTHER"
                            );
            Form78d_Discre NForm78d_Discre;
            int WMode = 5; // Not In IST
            NForm78d_Discre = new Form78d_Discre(WOperator, WSignedId, textBoxCateg.Text, WReconcCycleNo, WMode, "");
            NForm78d_Discre.ShowDialog();
        }
    }
}

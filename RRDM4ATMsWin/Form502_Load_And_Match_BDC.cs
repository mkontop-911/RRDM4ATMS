using Microsoft.Win32;
//using System.Data.OleDb;
using RRDM4ATMs;
using RRDMAgent_Classes;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using Excel = Microsoft.Office.Interop.Excel;


// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502_Load_And_Match_BDC : Form
    {

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDM_LoadFiles_InGeneral_EMR_BDC Lf_BDC = new RRDM_LoadFiles_InGeneral_EMR_BDC();

        RRDM_LoadFiles_InGeneral_EMR_BDC_T24 Lf_BDC_T24 = new RRDM_LoadFiles_InGeneral_EMR_BDC_T24();

        RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4 Mt = new RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4();
        //RRDMMatchingOfTxns_V02_MinMaxDt_BDC_5 Mt = new RRDMMatchingOfTxns_V02_MinMaxDt_BDC_5();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMJournalReadTxns_Text_Class Jc = new RRDMJournalReadTxns_Text_Class();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMAgentQueue Aq = new RRDMAgentQueue();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();

        public DataTable SavedSourceFilesDataTable = new DataTable(); // Rs
        public DataTable SavedTableMatchingCateg_Matching_Status = new DataTable(); // Mc

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        // Production from the date we had started after 27th of November 
        //public DateTime FixedDate = new DateTime(2025, 03, 03); // THIS THE PRODUCTION DATE

        // UAT HAS TO WORK as Normal => delete all before first day => set it to long future 

        // SECOND CASE FOR PRODUCTTION 
       // public DateTime FixedDate = new DateTime(2025, 07, 05); // THIS THE second case PRODUCTION DATE
        // UAT DATE
       public DateTime FixedDate = new DateTime(2050, 07, 05); // THIS THE second case PRODUCTION DATE
        // int WNumberOfLoadingAndMatching;

        string ReversedCut_Off_Date;

        string TotalProgressText;

        string TotalProgressTextOpenForm;

        string ProcessName;
        string Message;
        int Mode;

        string PRX;

        int Counter; 

        bool IsMatchingDone; 

        DateTime SavedStartDt;

        bool J_UnderLoading;

        bool FirstMessage; 

        //  DateTime J_SavedStartDt;

        DateTime WCut_Off_Date;
        string W_Application;

        int WSeqNoLeft;

        //int WReconcCycleNo;
        int WReconcCycleNoFirst;
        DateTime WFirstCut_Off_Date;

        bool T24Version; 

        int WRowIndex;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WReconcCycleNo;
        int WMode;

        public Form502_Load_And_Match_BDC(string InSignedId, int SignRecordNo, string InOperator
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

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            TotalProgressTextOpenForm = "";
            FirstMessage = true;

            TotalProgressTextOpenForm += DateTime.Now + "_"+"Start Showing Loading screen" + "\r\n";

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;
                //Usi.WFieldNumeric11 = 10; // This is a T24 Version 
                if (Usi.WFieldNumeric11 == 10)
                {
                    T24Version = true;
                }
            }
            else
            {
                MessageBox.Show("Sign On Record Not Found due to restart"+Environment.NewLine
                    + " Sign Off and Sign On Again please "    
                    ); 
            }

          

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
                labelFieldsDefinition.Show();
                panelCategories.Show();
                buttonMatching.Show();

                buttonLoadJournals.Enabled = false;
                buttonLoadFiles.Enabled = false;
                buttonDoMatching.Enabled = false;
                label3.Text = "THE CYCLES";
                buttonExportToExcel.Hide();
                //panelFiles.Hide();
                panelUndo.Hide();

                label6.Hide();
                label7.Hide();
                label9.Hide();

                textBoxNumberOfFiles.Hide();
                textBoxFilesReady.Hide();
                textBoxNotReady.Hide();

                buttonHST.Hide();
                label_HST.Hide();
                textBoxHST_DATE.Hide();
                buttonDeleteHst.Hide();
                buttonSummaryOfDeleted.Hide(); 

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

            // CHECK IF ROMANIA


            // GET TOTALS 
            ShowScreen(); 
        }

        // 
        // SHOW SCREEN
        //

        private void ShowScreen()
        {
            Counter = 0;
            // CHECK IF MATCHING IS DONE
            TotalProgressTextOpenForm += DateTime.Now + "_" + "Start checking if Matching is done" + "\r\n";
            Rcs.ReadReconcCategoriesSessions_To_Check_If_MatchingDONE(WReconcCycleNo);
            if (Rcs.RecordFound == true)
            {
                IsMatchingDone = true;
                panelLoaded.Show();
                labelFieldsDefinition.Show();
                panelCategories.Show();
                buttonMatching.Show();
                linkLabelExpand.Show();
                TotalProgressTextOpenForm += DateTime.Now + "_" + "Response: Matching is done" + "\r\n";

            }
            else
            {
                IsMatchingDone = false;
                // MessageBox.Show("ATMs records Shown That Matching is not done.");
                labelFieldsDefinition.Hide();
                panelCategories.Hide();
                buttonMatching.Hide();
                linkLabelExpand.Hide();
                TotalProgressTextOpenForm += DateTime.Now + "_" + "Response: Matching is not done" + "\r\n";
            }

            if (WMode == 2)
            {

                // View Only
                // labelStep1.Text = "View loaded amd Matched of Cycle_" + WReconcCycleNo.ToString()
                //                + "_and Date_" + WCut_Off_Date.ToShortDateString();
            }
            else
            {
                TotalProgressTextOpenForm += DateTime.Now + "_" + "Start checking loading journals if is done" + "\r\n";
                CheckLoadingOfJournals();
                TotalProgressTextOpenForm += DateTime.Now + "_" + "Checking loading journals finish" + "\r\n";

            }

            labelCurrentEntries.Hide();
            // panelCurrentEntries.Hide();
            dataGridViewFilesStatus.Hide();
            label2.Hide();
            textBoxFileId.Hide();
            radioButtonCategory.Hide();
            radioButtonAll.Hide();
            comboBoxMatchingCateg.Hide();
            labelCycle.Hide();
            textBoxCycle.Hide();
            buttonUndoFile.Hide();
            buttonNonProcessed.Hide();
            // buttonProcessed.Hide();

            // FIND HISTORY DATE

            string ParamId = "853";
            string OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true & Gp.OccuranceNm != "")
            {

                if (DateTime.TryParseExact(Gp.OccuranceNm, "yyyy-MM-dd", CultureInfo.InvariantCulture
                              , System.Globalization.DateTimeStyles.None, out HST_DATE))
                {
                    textBoxHST_DATE.Text = HST_DATE.ToShortDateString();

                    textBoxHST_DATE.Show();
                    label_HST.Show();

                }
            }
            //if (J_UnderLoading == true)
            //{
            //}
            TotalProgressTextOpenForm += DateTime.Now + "_" + "Start Method Journal Totals" + "\r\n";

            JournalTotals();

            TotalProgressTextOpenForm += DateTime.Now + "_" + "Method Journal Totals finishes" + "\r\n";

            // Other totals to fill grid
            // GetTotals();

            radioButtonCategory.Checked = true;
            if (WMode == 1)
            {
                // Source Files (Grid-ONE)
                // Read them and check them 

                if (W_Application == "e_MOBILE")
                {
                    // Get only the e_MOBILE
                    string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1 AND TableStructureId = 'MOBILE_WALLET' ";

                    Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

                }
                else
                {
                    // Normal Case
                    TotalProgressTextOpenForm += DateTime.Now + "_" + "Start Checking Files in Directories" + "\r\n";
                    string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1  AND TableStructureId = 'Atms And Cards' ";

                    Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

                    TotalProgressTextOpenForm += DateTime.Now + "_" + "Finish Checking Files in Directories" + "\r\n";

                }

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
        // Load 

        //private void Form502_Load(object sender, EventArgs e)
        //{
           

        //}
        // Row FIRST GRID
        string WFileComment;
        private void dataGridViewFiles_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFiles.Rows[e.RowIndex];

            Counter = Counter + 1;

            if (WMode == 2) Counter = 2; // because for some strange reason it doesnt go twice 

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

            if (WMode == 2)
            {
                // Check Per Row (Rows show the cycles)
                // CHECK IF MATCHING IS DONE 
                Rcs.ReadReconcCategoriesSessions_To_Check_If_MatchingDONE(WReconcCycleNo);
                if (Rcs.RecordFound == true)
                {
                    IsMatchingDone = true;
                    panelLoaded.Show();
                    labelFieldsDefinition.Show();
                    panelCategories.Show();
                    buttonMatching.Show();
                    linkLabelExpand.Show();

                }
                else
                {
                    IsMatchingDone = false;
                    // MessageBox.Show("ATMs records Shown That Matching is not done.");
                    labelFieldsDefinition.Hide();
                    panelCategories.Hide();
                    buttonMatching.Hide();
                    linkLabelExpand.Hide();
                }

            }
            // GRID 2 LOADED FILES
            if (Counter == 2 & FirstMessage == true)
            {
                TotalProgressTextOpenForm += DateTime.Now + "_" + "Start Preparing Loaded Files table" + "\r\n";
            }

            Flog.ReadLoadedFiles_Fill_Table(WOperator, WReconcCycleNo);

            if (Counter == 2 & FirstMessage == true)
            {
                TotalProgressTextOpenForm += DateTime.Now + "_" + "Finish Preparing Loaded Files table" + "..ROWS.." + Flog.DataTableFileMonitorLog.Rows.Count.ToString() + "\r\n";
            }
            if (Flog.DataTableFileMonitorLog.Rows.Count >0)
            {
                ShowGrid2();

                if (IsMatchingDone == true)
                {
                    if (Counter == 2 & FirstMessage == true)
                    {
                        TotalProgressTextOpenForm += DateTime.Now + "_" + "Start to populate Matching Matrix" + "\r\n";
                        
                    }

                    if (Counter == 2)
                    {
                        if (WMode == 1)
                        {
                            Rc.ReadReconcCategoriesForMatrix(WOperator, WReconcCycleNo, 2);
                        }
                        if (WMode == 2)
                        {
                            Rc.ReadReconcCategoriesForMatrix(WOperator, WReconcCycleNo, 3);
                        }
                            
                    }

                    if (Counter == 2 & FirstMessage == true)
                    {
                        TotalProgressTextOpenForm += DateTime.Now + "_" + "Finish Matching Matrix" + "\r\n";
                    }
                    if (WMode == 2)
                    {
                        labelFieldsDefinition.Show();
                        panelCategories.Show();
                        buttonMatching.Show();
                    }
                    if (Counter == 2 )
                    {
                        ShowGrid4();
                    }
                    
                }            
            }
            else
            {
                textBoxFilesRight.Text = "0";

                panelLoaded.Hide();
                label4.Hide();
                linkLabelExpand.Hide();
            }
           
          
            if (Counter == 2 & FirstMessage == true)
            {
                TotalProgressTextOpenForm += DateTime.Now + "_" + "Show Screen" + "\r\n";

                //MessageBox.Show("TIME TRACES TO SHOW SCREEN...." + Environment.NewLine
                //         + TotalProgressTextOpenForm);
                //TotalProgressTextOpenForm = "";
                FirstMessage = false; 
            }
           
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

            //dataGridViewMatched.Columns[4].Width = 80; // Cycle2
            //dataGridViewMatched.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridViewMatched.Columns[4].HeaderText = "Cycle " + Rc.Cycle2;

            //dataGridViewMatched.Columns[5].Width = 80; // Cycle3
            //dataGridViewMatched.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridViewMatched.Columns[5].HeaderText = "Cycle " + Rc.Cycle3;

            //dataGridViewMatched.Columns[6].Width = 80; // Cycle4
            //dataGridViewMatched.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridViewMatched.Columns[6].HeaderText = "Cycle " + Rc.Cycle4;

            //dataGridViewMatched.Columns[7].Width = 80; // Cycle5
            //dataGridViewMatched.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridViewMatched.Columns[7].HeaderText = "Cycle " + Rc.Cycle5;

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
            if (T24Version == true)
            {
                dataGridViewFilesStatus.DataSource = Lf_BDC_T24.SourceFilesTotals.DefaultView;
            }
            else
            {
                dataGridViewFilesStatus.DataSource = Lf_BDC.SourceFilesTotals.DefaultView;
            }
           

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
                    // DELETE THE ONES WITH ZERO SIZE
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Length ==0)
                    {
                        File.Delete(file);
                        continue; 
                    }
                    // DELETE THE DUBLICATES
                    if (Rs.CheckIfFileIsDublicate(file) == true )
                    {
                        // Delete File 
                        File.Delete(file);
                        Count2 = Count2 + 1;
                        continue;
                    }

                    string Check__ = file.Substring(44, 2);
                    bool ChangeName = false; 

                    if (Check__== "__")
                    {
                        ChangeName = true; 
                    }

                    if (ChangeName == true)
                    {
                        string oldFilePath = file;
                        string One = file.Substring(0, 44);
                        string two = file.Substring(45, 7);
                        string three = file.Substring(52, 12);
                        string newFilePath = One+two + "0" + three;

                        //string oldFilePath = @"C:\example\oldname.txt";
                        //string newFilePath = @"C:\example\newname.txt";

                        if (File.Exists(oldFilePath))
                        {
                            File.Move(oldFilePath, newFilePath);
                            //Console.WriteLine("File renamed successfully.");
                        }
                        else
                        {
                            //Console.WriteLine("File does not exist.");
                        }





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

                

                DateTime FileDATEresult;

                if (DateTime.TryParseExact(DateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out FileDATEresult))
                {

                }
                bool Condition = false; // we do not apply this 
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
                    string SourceFileName = Path.GetFileName(file);
                    WAtmNo = file.Substring(0, 8); // temporary
                    WAtmNo = file.Substring(36 ,8); // temporary
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

                ShowScreen();

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
        DateTime HST_DATE;
        int AgingDays_HST;

        //int AgingDays_HST; // This is the dates from moving to History data Base
        //                   // eg Moving From MATCHED to MATCHED_HST
        //int AgingCycle_HST; // THIS IS THE CYCLE FOR MOVING TO HISTORY
        private void buttonDoMatching_Click(object sender, EventArgs e)
        {
           // MessageBox.Show("This is version UAT"); 
            //
            // Start Service
            buttonMatching.Show();
            // Find the ready categories

            Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                     WReconcCycleNo);

            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParamId;

            string OccuranceId;

            DateTime DeleteDate = NullPastDate;

            if (MessageBox.Show("Do you want to Start Matching?" + Environment.NewLine
                                 + "For Ready Categories " + Environment.NewLine
                                 + Mt.W_MPComment
                                 , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                // YES Proceed

                // FIND CURRENT HISTORY DATE

                Mode = 5; // Updating Action 
                ProcessName = "Matching_Preparation";
                Message = "Matching Preparation Starts. Cycle:.." + WReconcCycleNo.ToString();
                SavedStartDt = DateTime.Now;

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);

                int TempMode = 0;

                ParamId = "853";
                OccuranceId = "5"; // HST

                Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

                if (Gp.RecordFound == true)
                {
                    AgingDays_HST = (int)Gp.Amount; // 

                    //AgingDays_HST = 0; 

                    // Current CutOffdate
                    string WSelection = " WHERE JobCycle =" + WReconcCycleNo;
                    Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

                    DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

                    Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);

                    if (Rjc.RecordFound == true)
                    {
                        DeleteDate = Rjc.Cut_Off_Date.AddDays(1); // leave it here to cover cases of Undo particularly for POS
                        TempMode = 2;
                    }
                    else
                    {
                        DeleteDate = WFirstCut_Off_Date; // Trans before this date will be deleted
                        TempMode = 1;
                    }
                }
                else
                {
                    DeleteDate = WFirstCut_Off_Date; // Trans before this date will be deleted
                    TempMode = 1;
                }

                //radioButtonMaster.Checked = true; 

                // DELETE UNWANTED RECORDS FROM TABLES 
                // IF WITH TODAYS LOADING TRANSACTIONS COME BEFORE THAT DATE WE DO NOT WANT THEM
                // WE DELETE THEM NOT TO TAKE PART IN TODAY's LOADING
                RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

                if (WOperator == "BCAIEGCX" & WCut_Off_Date >= FixedDate)
                {
                    // SPECIAL FIX FOR BANK DE CAIRE 2025-02-08
                    // SECOND SPECIAL FIX FOR BANK DE CAIRE 2025 - 07 - 05
                    if (WCut_Off_Date == FixedDate)
                    {
                        MessageBox.Show("From now on the delete date for the past txns will be "+Environment.NewLine 
                            + FixedDate.ToShortDateString());
                    }

                        DeleteDate = FixedDate; 
                }

                if (T24Version == true)
                {
                    Lf_BDC_T24.DeleteRecordsToSetStartingPoint(WOperator, DeleteDate, WCut_Off_Date, WReconcCycleNo, TempMode);
                }
                else
                {
                    Lf_BDC.DeleteRecordsToSetStartingPoint(WOperator, DeleteDate, WCut_Off_Date, WReconcCycleNo, TempMode);
                }
                
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
            if (T24Version == true)
            {
                Lf_BDC_T24.UpdateRecordsWithTraceAndOther(WOperator, WReconcCycleNo, 0, 2, WCut_Off_Date);
            }
            else
            {
                Lf_BDC.UpdateRecordsWithTraceAndOther(WOperator, WReconcCycleNo, 0, 2, WCut_Off_Date);
            }

            //
            // Before start we check the sign on users.
            //
            bool IsAllowedToSignIn = false;
           // bool ThereAreUsersInSystem = CheckForSignInUsers(IsAllowedToSignIn);

            //if (ThereAreUsersInSystem == true)
            //{
            //    // Decide whether to move forward or not. 
            //}


            // CREATE TABLE WITH MINIMAX DATES

            if (T24Version == true)
            {
                // Insert here the T24 version 
                Lf_BDC.Create_ATMS_AtmsMinMaxWorking_FLEX_Or_COREBANKING(WOperator, WReconcCycleNo, 2);
            }
            else
            {
                Lf_BDC.Create_ATMS_AtmsMinMaxWorking_FLEX_Or_COREBANKING(WOperator, WReconcCycleNo, 1);
            }

            Mode = 5; // Updating Action 
            ProcessName = "Matching_Preparation";
            Message = "Matching Preparation Finishes. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);


            // MATCHING 

            text = "Matching Starts Now";
            caption = "MATCHING";
            timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);

            //MessageBox.Show("Matching Starts Now");

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

            // FIND NUMBER OF DISCREPANCIES
            int TotalDescrepancies = Mpa.ReadInPoolTransTotalNOT_MatchedForCycle(WReconcCycleNo);

            Mode = 5; // Updating Action 
            ProcessName = "MatchingProcess";
            Message = "Matching Finishes.Descrep:." + TotalDescrepancies.ToString();

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************

            MatchedPressed = true;
            //
            // UPDATE GL RECORDS 
            //

            int WWMode;
            //Mgt.CreateRecords_CAP_DATE_For_Category(WOperator, WCut_Off_Date, WReconcCycleNo, WWMode);
            WWMode = 2;
            Mgt.CreateRecords_CAP_DATE_For_ATMs(WOperator, WCut_Off_Date, WReconcCycleNo, WWMode);
            // Check if already exists = Already updated for this cycle 

            // UPDATE Mpa with 818 where we Have EGP
            Mpa.UpdateMatchingTxnsMasterPoolATMsCurrency(WOperator);

            bool MasterTwoCurrencies; 

            DateTime TwoCcyNewVersionDt = new DateTime(2050, 03, 24);
            string ParId = "822"; // When version of files changes 
            string OccurId = "03"; // For IST and flexube and Meeza Global LCL  
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound)
            {
                try
                {
                    TwoCcyNewVersionDt = Convert.ToDateTime(Gp.OccuranceNm);

                    MasterTwoCurrencies = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("822 parameter date is wrong for two currency");
                    MasterTwoCurrencies = false;
                    CatchDetails(ex);
                }


                // MessageBox.Show("Master"); 

                //DateTime NewVersion3 = Convert.ToDateTime("24/03/2021");
                // date of change 
            }
            else
            {
                // Not found 
                MasterTwoCurrencies = false;
            }
           
            if (MasterTwoCurrencies == true)
            {
                if (T24Version== true)
                {
                    Lf_BDC_T24.UpdateMasterAfterMatchingWithSecondCurrency(WOperator, WReconcCycleNo);
                }
                else
                {
                    Lf_BDC.UpdateMasterAfterMatchingWithSecondCurrency(WOperator, WReconcCycleNo);
                }
                // Update second currency amount in spare field
                
            }

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
            //
            // NEW ΜΕΕΖΑ GLOBAL LCL
            // ΜΕΕΖΑ - ISSUER 
            //
            WMatchingCateg = PRX + "277";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "MEEZA_GLOBAL_LCL";
                // For Categories BDC277 and BDC278
                WCategories = "('" + PRX + "277','" + PRX + "278'" + ")"; // Issuer
                WIdentity = "MEEZA TXNS_Bank_Is_Issuer";
                WWMode = 4;
                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

            }

            // NEW ΜΕΕΖΑ GLOBAL LCL
            // ΜΕΕΖΑ - ISSUER - //TELDA
            //
            WMatchingCateg = PRX + "279";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "MEEZA_GLOBAL_LCL";
                // For Categories BDC277 and BDC278
                WCategories = "('" + PRX + "279' )";  // Issuer
                WIdentity = "MEEZA TXNS_Bank_Is_Issuer_TELDA";
                WWMode = 5 ;
                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

            }

            //
            // New MEEZA GLOBAL LCL 
            //
            WMatchingCateg = PRX + "279";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true & Cgl.W_Identity == "MEEZA TXNS_Bank_Is_Acquirer")
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "MEEZA_GLOBAL_LCL";
                // For Category BDC279
                WCategories = "('" + PRX + "279' )"; // Acquirer
                WIdentity = "MEEZA TXNS_Bank_Is_Acquirer";
                WWMode = 2;
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
                WWMode = 4; // New one based on extented BIN
                //WWMode = 1; OLD ONE based on BIN
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
                WCategories = "('" + PRX + "235','" + PRX + "236'" + ")"; // Acquirer
               // WCategories = "('" + PRX + "235' )"; // Acquirer
                WIdentity = "MASTER TXNS_Bank_Is_Acquirer";
                WWMode = 2;
                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);
            }

            WMatchingCateg = PRX + "231";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                // Create entries
                WFileId = "MASTER_POS";
                // For Categories BDC272 and BDC273
                WCategories = "('" + PRX + "231','" + PRX + "233'" + ")"; // Issuer POS
                WIdentity = "MASTER POS TXNS_Bank_Is_Issuer";
                WWMode = 4; // New one based on extented BIN
                //WWMode = 1; OLD ONE based on BIN
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

            //
            // VISA 
            //
            WMatchingCateg = PRX + "225";
            Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

            if (Cgl.RecordFound == true)
            {
                // Already Created
                // You should not create it
            }
            else
            {
                WFileId = "VISA_CARD";
                // For Category BDC215
                WCategories = "('" + PRX + "225' )"; // Acquirer
                WIdentity = "VISA TXNS_Bank_Is_Acquirer";
                WWMode = 2;
                Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

            }



            //UPDATE Mpa Replenishment Cycle After Matching for the 01 and 011
            // *********************************
            if (T24Version == true)
            {
                Lf_BDC_T24.UPDATE_Mpa_After_Matching_With_ReplCycle(WOperator, WReconcCycleNo);
            }
            else
            {
                Lf_BDC.UPDATE_Mpa_After_Matching_With_ReplCycle(WOperator, WReconcCycleNo);
            }
            

            // Exclude presenter if so 
            //
            // Presenter
            bool Is_Presenter_InReconciliation = false;
            ParId = "946";
            OccurId = "1";

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

            text = "Matching has Finished" + Environment.NewLine
                             + "Process of Moving records to Matched starts."; 
            caption = "MATCHING";
            timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);

            //MessageBox.Show("Matching has Finished" + Environment.NewLine
            //+ "Process of Moving records to Matched starts."
            //);

           
            //******************************
            // 
            // MOVE MATCHED TXNS POOL 
            // stp_00_MOVE_TXNS_TO_MATCHED_DB_01_POOL
            //******************************
            textBoxMsgBoard.Text = "Current Status : Moving Records Process";

            Mode = 5; // Updating Action 
            ProcessName = "Moving Records To MATCHED Data Base";
            Message = "Moving Records to MATCHED Process Starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);


            TotalProgressText = "MOVE TO MATCHED Cycle: ..."
                                     + WReconcCycleNo.ToString() + Environment.NewLine;
            
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            string WSelectionCriteria = " WHERE Operator = @Operator ";
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

                    // Check That File Exist in target data base 
                    string TargetDB = "[RRDM_Reconciliation_MATCHED_TXNS]";
                    Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                    if (Cv.RecordFound == true)
                    {
                        // File Exist
                    }
                    else
                    {
                        // File do not exist
                        MessageBox.Show("File.." + WFileName + Environment.NewLine
                            + "DOES NOT EXIST In MATCHED_TXNS Data Base."
                            + "REPORT TO THE HELP DESK."
                            );
                        I = I + 1;
                        continue;
                    }
                    
                    if ( WFileName == "Switch_IST_Txns")
                    {
                        // For IST Use not the current cycle but the previous
                        // in order to delete the Dublicates 
                        string WJobCategory = "ATMs";
                        Rjc.ReadLastReconcJobCycle_Closed_Cycle(WOperator, WJobCategory);

                        if (Rjc.RecordFound == true)
                        {
                            int WRMCycleNo = Rjc.JobCycle;
                            // WE WILL  NOT MOVE TO MATCH TILL CORRECTED
                            Cv.MOVE_ITMX_TXNS_TO_MATCHED(WFileName, WRMCycleNo);
                        }
                        else
                        {
                            // this is the first cycle (-1)
                            // Do not move any IST
                            I = I + 1;
                            continue;
                        }
                        
                    }
                    else
                    {
                        Cv.MOVE_ITMX_TXNS_TO_MATCHED(WFileName, WReconcCycleNo);
                    }
                   

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
                        I = I + 1;
                        continue;

                    }

                }

                I = I + 1;
            }

            TotalProgressText = TotalProgressText + DateTime.Now + " Moving of TXNS to matched has finished" + Environment.NewLine;
            TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

            // *****************************

            Mode = 5; // Updating Action 
            ProcessName = "Moving Records To MATCHED Data Base";
            Message = "Moving Records to Matched Process Finishes.";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************

            //MessageBox.Show(TotalProgressText);

            text = "Moving records to Matched has finished";
            caption = "MOVING RECORDS";
            timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);

            //MessageBox.Show("Moving records to Matched has finished" + Environment.NewLine
            //                + ""
            //                );

            // FIND LIMIT DATE FOR HISTORY 

            // MOVE FROM MATCHED TO MATCHED_HST
            //WSelection = " WHERE JobCycle =" + InReconcCycleNo;
            //Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);
            bool MoveToHistory = false;
            ParamId = "853";
            OccuranceId = "5"; // HST
            DateTime DatefromDeletion = NullPastDate; 

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true)
            {
                MoveToHistory = true;
            }

                //AgingDays_HST = (int)Gp.Amount; // 

                ////AgingDays_HST = 0; 

                //// Current CutOffdate
                //string WSelection = " WHERE JobCycle =" + WReconcCycleNo;
                //Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

                //DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

                //Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);

                //if (Rjc.RecordFound == true)
                //{
                //    string ReversedCut_Off_Date = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

                //    ParamId = "853";
                //    OccuranceId = "6"; // HST

                //    Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);
                //    if (Gp.RecordFound == true)
                //    {
                //        int Int_DeleteFrom_HST = (int)Gp.Amount; // 
                //        DatefromDeletion = WCut_Off_Date.AddDays(-Int_DeleteFrom_HST); 
                //    }

                //    MessageBox.Show("Moving records to History Starts" + Environment.NewLine
                //           + "For date equal or less than.." + ReversedCut_Off_Date + Environment.NewLine
                //            + "Also Deletion of Records from HST will be done." + DatefromDeletion.ToShortDateString()
                //           );
                //    MoveToHistory = true;
                //}
            //}
            MoveToHistory = false;
            if (MoveToHistory == true)
            {
                //******************************
                // 
                // MOVE TO HST
                // stp_00_MOVE_TXNS_TO_HISTORY_DB_01_POOL
                //******************************
                Mode = 5; // Updating Action 
                ProcessName = "Moving To HST And Delete From HST";
                Message = "Moving To HST And Delete From HST Process Starts. Cycle:.." + WReconcCycleNo.ToString();

                text = "Moving records to History Starts";
                caption = "MOVING RECORDS";
                timeout = 5000;
                AutoClosingMessageBox.Show(text, caption, timeout);

                //MessageBox.Show("Moving records to History Starts" + Environment.NewLine
                //            + ""
                //            );

                SavedStartDt = DateTime.Now;

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);


                TotalProgressText = "MOVE TO HISTORY Cycle: ..."
                                         + WReconcCycleNo.ToString() + Environment.NewLine;
                //RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
                //RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
                WSelectionCriteria = " WHERE Operator = @Operator ";
                Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

                I = 0;
                K = 0;

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

                        // Check That File Exist in target data base 
                        string TargetDB = "[RRDM_Reconciliation_ITMX_HST]";
                        Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (Cv.RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            MessageBox.Show("File.." + WFileName + Environment.NewLine
                                + "DOES NOT EXIST In ITMX_HST Data Base."
                                + "REPORT TO THE HELP DESK."
                                );
                            I = I + 1;
                            continue;
                        }

                        // Check That File Exist in target data base 
                        TargetDB = "[RRDM_Reconciliation_MATCHED_TXNS_HST]";
                        Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (Cv.RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            MessageBox.Show("File.." + WFileName + Environment.NewLine
                                + "DOES NOT EXIST In MATCHED_TXNS_HST Data Base."
                                + "REPORT TO THE HELP DESK."
                                );
                            I = I + 1;
                            continue;
                        }

                        // MessageBox.Show("Start Moving file.." + WFileName); 

                        Cv.MOVE_ITMX_TXNS_TO_HST(WFileName, WReconcCycleNo);

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
                            MessageBox.Show("VITAL SYSTEM ERROR DURING MOVING TO HISTORY" + Environment.NewLine
                                           + "PROGRESS TEXT.." + Cv.ProgressText + Environment.NewLine
                                           + "ERROR REFERENCE.." + Cv.ErrorReference + Environment.NewLine
                                           + ""
                                           );

                            //  return;

                            I = I + 1;
                            continue;

                        }

                    }

                    I = I + 1;
                }

                TotalProgressText = TotalProgressText + DateTime.Now + " Moving of TXNS to HST has finished" + Environment.NewLine;
                TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

                // *****************************
                //
                // DELETE RECORDS FROM HISTORY DATA BASES BASED ON PARAMETER 853
                //
                text = "Moving Records to History Finishes" + Environment.NewLine
                            + "Delete Records From History Starts if any proper parameneter present";
                caption = "MOVING RECORDS";
                timeout = 5000;
                AutoClosingMessageBox.Show(text, caption, timeout);

                //MessageBox.Show("Moving Records to History Finishes" + Environment.NewLine
                //            + "Delete Records From History Starts if any proper parameneter present"
                //            );
                int WWWMode = 0;
                Cv.DELETE_DELETE_TXNS_FROM_HST_MAIN(WOperator, WSignedId, WReconcCycleNo, WWWMode);

                Mode = 5; // Updating Action 
                ProcessName = "Moving To HST And Delete From HST";
                Message = "Moving To HST And Delete From HST.";

                text = "Delete From HST has finished";
                caption = "MOVING RECORDS";
                timeout = 5000;
                AutoClosingMessageBox.Show(text, caption, timeout);
                //MessageBox.Show("Delete From HST has finished" + Environment.NewLine
                //             + ""
                //             );

                textBoxMsgBoard.Text = "Current Status : Ready";

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
                //*********************************

              //  MessageBox.Show(TotalProgressText);

            }

            // MATCHING AND MOVING RECORDS HAS FINISHED 


            // AT the END UPDATE STATS

            // 
            // 
            // AT the END UPDATE STATS

            //string connectionStringITMX = ConfigurationManager.ConnectionStrings
            //     ["ReconConnectionString"].ConnectionString;

            //// AT the END UPDATE STATS
            //int ReturnCode = -1;
            //int ret;
            //string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[Stp_00_UPDATE_DB_System_Stats]";

            //using (SqlConnection conn =
            //   new SqlConnection(connectionStringITMX))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd =
            //           new SqlCommand(RCT, conn))
            //        {
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            // Parameters
            //            SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
            //            retCode.Direction = ParameterDirection.Output;
            //            retCode.SqlDbType = SqlDbType.Int;
            //            cmd.Parameters.Add(retCode);

            //            cmd.ExecuteNonQuery();

            //            ret = (int)cmd.Parameters["@ReturnCode"].Value;
            //            //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
            //            //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

            //        }
            //        // Close conn
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
            //    }
            
            IsAllowedToSignIn = true;
            CheckForSignInUsers(IsAllowedToSignIn);

            //if (ThereAreUsersInSystem == true)
            //{
            //    // Decide whether to move forward or not. 
            //}
            text = "Matching and movings of records process has finished";
            caption = "MATCHING";
            timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);
            //MessageBox.Show("Matching and movings of records process has finished");

            ShowScreen();

        }
        //
        // LOAD FILES
        //
        string FullFileName;
        string text ;
        string caption ;
        int timeout; 

        private void buttonLoadFiles_Click(object sender, EventArgs e)
        {
            text = "Loading from Journals DB to Master STARTS";
            caption = "LOADING";
            timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);

           

            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfFiles";
            Message = "Loading Of Files starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);

            //MessageBox.Show("Loading from Journals DB to Master STARTS");
            DateTime startTime = DateTime.Now;
            // READ AUDI AND INSERT IN Master file 
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2_Pambos2 PambosLoad = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2_Pambos2();
            // GET FROM PAMBOS TO MASTER
            PambosLoad.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo);

            DateTime endTime = DateTime.Now;

            TimeSpan span = endTime.Subtract(startTime);

            text = "Loading from Journals DB to Master FINISHES" + Environment.NewLine
                + "Time Elapsed In Minutes.." + span.TotalMinutes; 
            caption = "LOADING";
            timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);

            //MessageBox.Show("Loading from Journals DB to Master FINISHES" + Environment.NewLine
            //    + "Time Elapsed In Minutes.." + span.TotalMinutes
            //    );

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

            startTime = DateTime.Now;

            text = "Supervisor mode Work starts"; 
            caption = "LOADING";
            timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);

           // MessageBox.Show("Supervisor mode Work starts");
            textBoxMsgBoard.Text = "Current Status : Supervisor Mode data loading process";

            // CHECK IF RECYCLING ATMS
            string ParId = "948";
            string OccurId = "1"; // 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                RRDMRepl_SupervisorMode_Master_Recycle Smaster = new RRDMRepl_SupervisorMode_Master_Recycle();

                int Sm_Mode = 3;
                Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, WSignRecordNo, WOperator,
                                                                 Sm_Mode, "NO_Form153");

                endTime = DateTime.Now;

                span = endTime.Subtract(startTime);

                text = "Supervisor mode Work FINISHES" + Environment.NewLine
                    + "Time Elapsed In Minutes.." + span.TotalMinutes; 
                caption = "LOADING";
                timeout = 5000;
                AutoClosingMessageBox.Show(text, caption, timeout);

                //MessageBox.Show("Supervisor mode Work FINISHES" + Environment.NewLine
                //    + "Time Elapsed In Minutes.." + span.TotalMinutes
                //    );
            }
            else
            {
                //
                // Supervisor mode
                //

                RRDMRepl_SupervisorMode_Master Smaster = new RRDMRepl_SupervisorMode_Master();
                int Sm_Mode = 3;
                //
                // HERE IS NO RECYCLE
                //
                Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, WSignRecordNo, WOperator,
                                                                 Sm_Mode, "NO_Form153");

                endTime = DateTime.Now;

                span = endTime.Subtract(startTime);

                MessageBox.Show("Supervisor mode Work FINISHES" + Environment.NewLine
                    + "Time Elapsed In Minutes.." + span.TotalMinutes
                    );
            }


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
                    text = "Deleted Duplicates due to NCR Vision problem." + Environment.NewLine
                             + "Number deleted..=" + Mpa.Count.ToString();
                    caption = "NCR DUPLICATE";
                    timeout = 5000;
                    AutoClosingMessageBox.Show(text, caption, timeout);
                    //MessageBox.Show("Deleted Duplicates due to NCR Vision problem." + Environment.NewLine
                    //         + "Number deleted..=" + Mpa.Count.ToString()
                    //          );
                }
                else
                {
                    //MessageBox.Show("NO Duplicates FOUND due NCR Vision problem." + Environment.NewLine
                    //                + "Maybe NCR has corrected problem." + Environment.NewLine
                    //                + " Please check and report"
                    //                 );
                }
            }
            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            string WFileName = "CIT_EXCEL_TO_BANK";
            string TargetDB = "[RRDM_Reconciliation_ITMX]";
            Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

            if (Cv.RecordFound == true)
            {
                //
                // CREATE WORKING TABLE FOR IST Vs CIT Transactions
                //
                // Here CREATE THE FILE FOR CIT REPLENISHMENT for Journals 
                RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();
                string WTableId = "[RRDM_Reconciliation_ITMX].[dbo].[CIT_JOURNAL_TXNS]";
                Ce.Insert_JOURNAL_TXNS_For_CIT(WTableId, WReconcCycleNo);
            }

           

            // return; // for testing 
            text = "Loading of files starts."; 
            caption = "LOADING";
            timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);
            //MessageBox.Show("Loading of files starts." + Environment.NewLine
            //                + "" + Environment.NewLine
            //               );

            textBoxMsgBoard.Text = "Current Status : Loading of Files process";



           
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

            if (W_Application == "e_MOBILE")
            {
                // Get only the e_MOBILE
                string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1 AND TableStructureId = 'MOBILE_WALLET' ";

                Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

            }
            else
            {
                // Normal Case
                string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1  AND TableStructureId = 'Atms And Cards' ";

                Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

            }

            SavedSourceFilesDataTable = Rs.Table_Files_In_Dir;

            buttonLoad.Show();

            // LOAD FILES
            //******************************
            //
            METHOD_LoadFiles();
            //

            //
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfFiles";
            Message = "Loading Of Files Finishes. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************
            text = "Loading Of Files has Finished";
            caption = "LOADING";
            timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);
            //****************************
            string Temp = "NOT Panicos"; //  Check IST 
            if (Temp == "Panicos....")
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

                //Mgt.DeleteDuplicatesInIST(WReconcCycleNo); 
                
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

           

            //MessageBox.Show("Loading Of Files has Finished");
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
            ShowScreen();

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

            string sourceDir = @"C:\RRDM\FilesArchives\" + ReversedCut_Off_Date + "_" + WReconcCycleNo.ToString(); // directory to zip
            string zipFilePath = @"C:\RRDM\FilesArchives\" + ReversedCut_Off_Date + "_" + WReconcCycleNo.ToString() + ".zip"; // output file

            if (File.Exists(zipFilePath))
            {
                MessageBox.Show("Action A : Unzip file.." + zipFilePath + " " + Environment.NewLine
                    + "Action B : Delete.." + zipFilePath + " " + Environment.NewLine
                    + " AND TRY AGAIN"
                    );
                return;

                //try
                //{
                //    string zipFilePath = @"C:\RRDM\FilesArchives\20250306_200.zip";
                //    string extractPath = @"C:\RRDM\FilesArchives\20250306_200";

                //    // Overwrite existing folder
                //    if (System.IO.Directory.Exists(extractPath))
                //        System.IO.Directory.Delete(extractPath, true);

                //    ZipFile.ExtractToDirectory(zipFilePath, extractPath);

                //    MessageBox.Show("UN_ZIP DONE");

                //    Thread.Sleep(100);

                //    // Delete directory
                //    try
                //    {
                //        File.Delete(zipFilePath);
                //        // Console.WriteLine("Folder deleted successfully.");
                //        //"C:\RRDM\FilesArchives\20250306_200.zip"
                //    }
                //    catch (Exception ex)
                //    {
                //        //Console.WriteLine($"Failed to delete folder: {ex.Message}");
                //        MessageBox.Show($"Failed to delete folder: {ex.Message}");
                //    }
                //}
                //catch (Exception ex)
                //{

                //    CatchDetails(ex);

                //}
            }

            if (Directory.Exists(sourceDir))
            {
                //OK
            }
            else
            {
                MessageBox.Show("Directory.." + sourceDir + " " + Environment.NewLine
                    + " Not Found "
                    );

            }

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
            Message = "UNDO Matching and Loading of Files STARTS. Cycle .." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
            // BEFORE UNDO 
            TotalProgressText = DateTime.Now + "Moving TXNs from Matched to ITMX Starts" + Environment.NewLine;

            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            string WSelectionCriteria = " WHERE Operator = @Operator ";
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

                    string TargetDB = "[RRDM_Reconciliation_ITMX]";
                    Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                    if (Cv.RecordFound == true)
                    {
                        // File Exist
                    }
                    else
                    {
                        // File do not exist
                        MessageBox.Show("File.." + WFileName + Environment.NewLine
                            + "DOES NOT EXIST In ITMX Data Base."
                            + "REPORT TO THE HELP DESK."
                            );
                        I = I + 1;
                        continue;
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

                        // return;

                    }

                }

                I = I + 1;
            }

            TotalProgressText = TotalProgressText + DateTime.Now + " MOVING OF TXNS FROM MATCHED HAS FINISHED" + Environment.NewLine;
            TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

            MessageBox.Show(TotalProgressText);


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

                    //cmd.Parameters.Add(new SqlParameter("@RMCycleNo", WReconcCycleNo));
                    SqlParameter WCycleNo = new SqlParameter("@RMCycleNo", WReconcCycleNo);
                    WCycleNo.Direction = ParameterDirection.Input;
                    WCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WCycleNo);

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
            if (T24Version == true)
            {
                Lf_BDC_T24.HandleDailyStatisticsForAtms_UNDO(WReconcCycleNo);
            }
            else
            {
                Lf_BDC.HandleDailyStatisticsForAtms_UNDO(WReconcCycleNo);
            }
               

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

            // Turn the process code to 0 with loading has become to -1 and with mathing to 1  
            RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();
            Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToZeroForCycle(WReconcCycleNo);
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
                MessageBox.Show("VALID CALL" + Environment.NewLine
                            + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("NOT VALID CALL" + Environment.NewLine
                         + ProgressText);
            }

            textBoxMsgBoard.Text = "Current Status : Ready";

            // AT the END UPDATE STATS
            ReturnCode = -1; 
            int rows;
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
                        SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                        retCode.Direction = ParameterDirection.Output;
                        retCode.SqlDbType = SqlDbType.Int;
                        cmd.Parameters.Add(retCode);

                        cmd.ExecuteNonQuery();

                        ret = (int)cmd.Parameters["@ReturnCode"].Value;
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

            ShowScreen();
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
                    if (SourceFileId == "Flexcube" & T24Version== true & IsGood == "YES")
                    {
                        MessageBox.Show("This is a T24Version. Why do you load Flexcube?");
                        IsGood = "NO"; 
                    }
                    if (SourceFileId == "COREBANKING" & T24Version == false & IsGood == "YES")
                    {
                        MessageBox.Show("There is a good COREBANKING file but the T24 version was not selected.");
                        IsGood = "NO";
                    }

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
                                if (T24Version)
                                {
                                    // T24 Version
                                    Lf_BDC_T24.InsertRecordsInTableFromTextFile_InBulk(WOperator, SourceFileId, FullFileName, Ms.InportTableName, Ms.Delimiter, FlogSeqNo);
                                }
                                else
                                {
                                    // Not T24 Version
                                    Lf_BDC.InsertRecordsInTableFromTextFile_InBulk(WOperator, SourceFileId, FullFileName, Ms.InportTableName, Ms.Delimiter, FlogSeqNo);

                                }

                                // IT IS TEMPORARY 
                                // ALECOS WILL INSERT THIS

                                //if (Environment.UserInteractive)
                                //{

                                Flog.ReadLoadedFilesBySeqNo(FlogSeqNo);

                                Flog.StatusVerbose = "";

                                if (T24Version ==true)
                                {
                                    Flog.LineCount = Lf_BDC_T24.stpLineCount;

                                    Flog.stpReturnCode = Lf_BDC_T24.stpReturnCode;
                                    Flog.stpErrorText = Lf_BDC_T24.stpErrorText;
                                    Flog.stpReferenceCode = Lf_BDC_T24.stpReferenceCode;
                                    if (Lf_BDC_T24.stpReturnCode == 0)
                                        Flog.Status = 1; // Success
                                    else Flog.Status = 0; //Failure
                                }
                                else
                                {
                                    Flog.LineCount = Lf_BDC.stpLineCount;

                                    Flog.stpReturnCode = Lf_BDC.stpReturnCode;
                                    Flog.stpErrorText = Lf_BDC.stpErrorText;
                                    Flog.stpReferenceCode = Lf_BDC.stpReferenceCode;

                                    if (Lf_BDC.stpReturnCode == 0)
                                        Flog.Status = 1; // Success
                                    else Flog.Status = 0; //Failure
                                }      
                                // Update Flog
                                //Flog.Update(Lf_BDC.WFlogSeqNo);

                                Flog.Update(FlogSeqNo);

                                if (Flog.SourceFileID == "GL_Balances_Atms_Daily" || Flog.SourceFileID == "CIT_EXCEL_TO_BANK")
                                {
                                    // Do not find Max date CIT_EXCEL_TO_BANK
                                }
                                else
                                {
                                    Flog.Update_MAX_DATE(Flog.SourceFileID, FlogSeqNo, WReconcCycleNo);
                                }

                                //Flog.Update_MAX_DATE(Flog.SourceFileID, FlogSeqNo, WReconcCycleNo);

                                // Update with -1 = ready for Matched if File is good
                                if (Lf_BDC.stpReturnCode == 0 & T24Version == false)
                                {
                                    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne(SourceFileId, WReconcCycleNo);

                                    if (SourceFileId == "Switch_IST_Txns" || SourceFileId == "Flexcube")
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
                                if (Lf_BDC_T24.stpReturnCode == 0 & T24Version == true)
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
                                int r;
                                string D; 
                                if (T24Version)
                                {
                                    r = Lf_BDC_T24.stpReturnCode;
                                    D = Lf_BDC_T24.stpErrorText;
                                }
                                else
                                {
                                    r = Lf_BDC.stpReturnCode;
                                    D = Lf_BDC.stpErrorText;
                                }
                                
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
                string text = "Extra work during loading in progress";
                string caption = "LOADING OF FILES";
                int timeout = 2000;
                AutoClosingMessageBox.Show(text, caption, timeout);
                if (T24Version == true)
                {
                    Lf_BDC_T24.UpdateRecordsWithTraceAndOther(WOperator, WReconcCycleNo, FlogSeqNo, 1, WCut_Off_Date);
                    //
                    // EXTRA FIELDS
                    //MessageBox.Show("We Start extra fields");
                    Lf_BDC_T24.UpdateFiles_With_EXTRA(WOperator, WReconcCycleNo);
                    //MessageBox.Show("We finish extra fields");
                }
                else
                {
                    Lf_BDC.UpdateRecordsWithTraceAndOther(WOperator, WReconcCycleNo, FlogSeqNo, 1, WCut_Off_Date);
                    //
                    // EXTRA FIELDS
                    //MessageBox.Show("We Start extra fields");
                    Lf_BDC.UpdateFiles_With_EXTRA(WOperator, WReconcCycleNo);
                    //MessageBox.Show("We finish extra fields");
                }


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

      
        // GET TOTALS

        private void buttonShowEntries_Click(object sender, EventArgs e)
        {
            labelCurrentEntries.Show();
            //panelCurrentEntries.Show();
            dataGridViewFilesStatus.Show();
            label2.Show();
            textBoxFileId.Show();
            radioButtonCategory.Show();
            radioButtonAll.Show();
            comboBoxMatchingCateg.Show();
            labelCycle.Show();
            textBoxCycle.Show();
            buttonUndoFile.Show();
            buttonNonProcessed.Show();
            //buttonProcessed.Show(); 
            if (T24Version == true)
            {
                Lf_BDC_T24.GetTotals(WOperator, WReconcCycleNo);
            }
            else
            {
                Lf_BDC.GetTotals(WOperator, WReconcCycleNo);
            }
                

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
            //string WMatchingCateg = comboBoxMatchingCateg.Text.Substring(0, 6);

            //Form78d_FileRecords NForm78d_FileRecords;
            //// textBoxFileId.Text
            //int WMode = 2; //
            //               // InMode = 1 : Not processed yet 
            //               // InMode = 2 : Processed this Cycle
            //if (radioButtonAll.Checked == true) WCategoryOnly = false;
            //else WCategoryOnly = true;
            //NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, textBoxFileId.Text, "", WReconcCycleNo, WMatchingCateg, WMode, WCategoryOnly);
            //NForm78d_FileRecords.ShowDialog();
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
                MessageBox.Show("This process will move the unloaded Journals to directory. " + Environment.NewLine
                               + "");
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

            if (MessageBox.Show("Do you want to UNDO The Loaded Journals " + Environment.NewLine
                                + "For this Cycle ...? " + Environment.NewLine
                                + WReconcCycleNo.ToString()
                                , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {
                // YES Proceed
                MessageBox.Show("................. " + Environment.NewLine
                               + "Wait till a final message is shown. ");
            }
            else
            {
                // Stop 
                return;
            }

            textBoxMsgBoard.Text = "Current Status : Unloading Journals";

            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "UNDO_LoadingOfJournals";
            Message = "UNDO of Loading Of Journals STARTS..Cycle" + WReconcCycleNo.ToString();
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
                            MessageBox.Show("No Journals to move. No such directory..." + Environment.NewLine
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
                            MessageBox.Show("There are no exceptions to move. "
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

                    ShowScreen();
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
            //string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[Stp_00_UPDATE_DB_System_Stats]";

            //using (SqlConnection conn =
            //   new SqlConnection(connectionStringITMX))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd =
            //           new SqlCommand(RCT, conn))
            //        {
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            // Parameters

            //            int rows = cmd.ExecuteNonQuery();
            //            //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
            //            //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

            //        }
            //        // Close conn
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
            //    }

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
            // 
            //MessageBox.Show("To get full results load IST and then press this button."+Environment.NewLine
            //    + "If you have already loaded it then ignore this message."
            //    );
            //return; 
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

            ShowScreen();

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
            //MessageBox.Show("Avoid using this functionality." + Environment.NewLine
            //    + "Contact RRDM for further information"
            //    );
            //return;

            if (WFileId == "Atms_Journals_Txns"
                || WFileId == "Switch_IST_Txns"
                  || WFileId == "Flexcube"
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
            TotalProgressText = DateTime.Now + "Moving TXNs from Matched to ITMX Starts" + Environment.NewLine;

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
                if (Msvf.SourceFileNameC == "Flexcube")
                {
                    TempFiled = "Flexcube";
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

            MessageBox.Show(TotalProgressText);

            ShowScreen();
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


            if ((InIsAllowedToSignIn == false)
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
        // Not in IST
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
        // Move to History 
        private void buttonHST_Click(object sender, EventArgs e)
        {
            // MOVE FROM MATCHED TO MATCHED_HST
            //WSelection = " WHERE JobCycle =" + InReconcCycleNo;
            //Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);
            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();

            bool MoveToHistory = false;
            string ParamId = "853";
            string OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true)
            {
                AgingDays_HST = (int)Gp.Amount; // 

                //AgingDays_HST = 0; 

                // Current CutOffdate
                string WSelection = " WHERE JobCycle =" + WReconcCycleNo;
                Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);
                //AgingDays_HST = 0; 
                DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

                //WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-0);

                Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);

                if (Rjc.RecordFound == true)
                {
                    string ReversedCut_Off_Date = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

                    MessageBox.Show("Moving records to History Starts" + Environment.NewLine
                           + "For date equal or less than.." + ReversedCut_Off_Date
                           );
                    MoveToHistory = true;
                }
            }

            string WFileName = "";
            //MoveToHistory = true;
            if (MoveToHistory == true)
            {
                //******************************
                // 
                // MOVE TO HST
                // stp_00_MOVE_TXNS_TO_HISTORY_DB_01_POOL
                //******************************
                Mode = 5; // Updating Action 
                ProcessName = "Moving Records To HST Data Base";
                Message = "Moving Records to HST Process Starts. Cycle:.." + WReconcCycleNo.ToString();
                SavedStartDt = DateTime.Now;

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);


                TotalProgressText = "MOVE TO HISTORY Cycle: ..."
                                         + WReconcCycleNo.ToString() + Environment.NewLine;
                //RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
                //RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
                string WSelectionCriteria = " WHERE Operator = @Operator ";
                Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

                int I = 0;
                int K = 0;

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

                        // Check That File Exist in target data base 

                        string TargetDB = "[RRDM_Reconciliation_ITMX_HST]";
                        Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (Cv.RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            MessageBox.Show("File.." + WFileName + Environment.NewLine
                                + "DOES NOT EXIST In ITMX_HST Data Base."
                                + "REPORT TO THE HELP DESK."
                                );
                            I = I + 1;
                            continue;
                        }

                        // Check That File Exist in target data base 
                        TargetDB = "[RRDM_Reconciliation_MATCHED_TXNS_HST]";
                        Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (Cv.RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            MessageBox.Show("File.." + WFileName + Environment.NewLine
                                + "DOES NOT EXIST In MATCHED_TXNS_HST Data Base."
                                + "REPORT TO THE HELP DESK."
                                );
                            I = I + 1;
                            continue;
                        }

                        // MessageBox.Show("Start Moving file.." + WFileName); 

                        Cv.MOVE_ITMX_TXNS_TO_HST(WFileName, WReconcCycleNo);

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
                            MessageBox.Show("VITAL SYSTEM ERROR DURING MOVING TO HISTORY" + Environment.NewLine
                                           + "PROGRESS TEXT.." + Cv.ProgressText + Environment.NewLine
                                           + "ERROR REFERENCE.." + Cv.ErrorReference + Environment.NewLine
                                           + ""
                                           );

                            //  return;

                            I = I + 1;
                            continue;

                        }

                    }

                    I = I + 1;
                }

                TotalProgressText = TotalProgressText + DateTime.Now + " Moving of TXNS to HST has finished" + Environment.NewLine;
                TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

                // *****************************

                Mode = 5; // Updating Action 
                ProcessName = "Moving Records To HST Data Base";
                Message = "MovingRecords To HST Process Finishes.";

                textBoxMsgBoard.Text = "Current Status : Ready";

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
                //*********************************

                MessageBox.Show(TotalProgressText);
                
            }

           // ShowScreen();
        }
        // Find Recycle 
        int CountRecycling; 
        private void buttonFIND_Recycle_Click(object sender, EventArgs e)
        {
            CountRecycling = 0 ;
            //
            // Clear destination directory 
            //
            string[] filePaths = Directory.GetFiles("C:\\RRDM\\Working_Recycling");
            foreach (string filePath in filePaths)
                File.Delete(filePath);

            string SourceFileId = "Atms_Journals_Txns";

            Rs.ReadReconcSourceFilesByFileId(SourceFileId);

            string InSourceDirectory = Rs.SourceDirectory;

           // InSourceDirectory = "C:\\RRDM\\Archives\\Atms_Journals_Txns\\20220806_200";

            string[] allJournals = Directory.GetFiles(InSourceDirectory, "*.*");


            if (allJournals.Length == 0)
            {
                // Re Check here
                MessageBox.Show(" There are no files to check");
                textBox1.Text = "0";
                textBoxMsgBoard.Text = "Current Status:Ready";
                return;
            }
            int Count2 = 0;
            foreach (string file in allJournals)
            {
                string WAtmNo = file.Substring(36, 8);
              //  WAtmNo = file.Substring(49, 8);
                CheckReplenishment(WOperator, WSignedId, WAtmNo, file);

                //if (Rs.CheckIfFileIsDublicate(file) == true)
                //{
                //    // Delete File 
                //    File.Delete(file);
                //    Count2 = Count2 + 1;
                //}

            }

            if (CountRecycling > 0)
            {
                MessageBox.Show("RECYCLING Journals found " + Environment.NewLine
                          + "Number of journals :.. " + CountRecycling.ToString()
                           );
            }
            else
            {
                MessageBox.Show("RECYCLING Journals Not found " + Environment.NewLine
                        //  + "Number of journals :.. " + CountRecycling.ToString()
                           );
            }

        }

        private void CheckReplenishment(string InOperator, string InSignedId, string InAtmNo
             , string InJournalTxtFile)
        {
            //RecordFound = false;
            //ErrorFound = false;
            //ErrorOutput = "";
            int ReturnCode = -1;
            string WJournalTxtFile = InJournalTxtFile;
            string EjournalTypeId = "";

            string ErrorText = "";
            string ErrorReference = "";

            string connectionString_AUDI = ConfigurationManager.ConnectionStrings
                   ["JournalsConnectionString_AUDI"].ConnectionString;


            //WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstEjText]";

            // CREATE JOURNAL IF NOT AVAILABLE
            //
            //RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            //#region Create a new file with sequence number in front of each line
            //// Add sequence number in front of each line of the line
            //string jlnFullPathName;
            //RRDMJournalReadTxns_Text_Class Jrt = new RRDMJournalReadTxns_Text_Class();
            //jlnFullPathName = Jrt.ConvertJournal(WJournalTxtFile); // Converted File 
            //                                                       // LineCount = Jrt.LineCounter;
            //#endregion

            Ac.ReadAtm(InAtmNo);
            EjournalTypeId = Ac.EjournalTypeId;

            string SPName = "[ATM_MT_Journals_AUDI].[dbo].[stp_CheckReplenishment]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString_AUDI))
            {
                try
                {
                    int ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@AtmNo", InAtmNo));
                    cmd.Parameters.Add(new SqlParameter("@FullPath", WJournalTxtFile));
                    cmd.Parameters.Add(new SqlParameter("@JournalType", EjournalTypeId));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        //  MEANS IT IS A RECYCLE AND HAS a REPLENISHEMENT 

                        CountRecycling = CountRecycling + 1; 
                        RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

                        Ed.CopyFileFromOneDirectoryToAnother(WJournalTxtFile, "C:\\RRDM\\Working_Recycling");

                        
                    }
                    else
                    {
                        //  NOT RECYCLE ATM
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    // CatchDetails(ex);
                }
            }
        }

        private void buttonBad2_Click(object sender, EventArgs e)
        {
            Form78e NForm78e;

            int Mode = 3; // The Deposits with Wrong Repl Cycle 
            string InHeader = "THE DEPOSITS WITH WRONG REPL CYCLE =.." + WReconcCycleNo.ToString();

            NForm78e = new Form78e(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WCut_Off_Date, InHeader, Mode);
            NForm78e.ShowDialog();
        }
// DELETE FROM HST
        private void buttonDeleteHst_Click(object sender, EventArgs e)
        {
           
            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            int WMode = 2; 
            Cv.DELETE_DELETE_TXNS_FROM_HST_MAIN(WOperator, WSignedId, WReconcCycleNo, WMode);

            //return; 
            //RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();

            //bool DeleteRecords = false;
            //int DeleteDays; 
            //string ParamId = "853";
            //string OccuranceId = "6"; // DELETE
            //string S_DeleteDateLimit = "";
            //DateTime DeleteDateLimit = NullPastDate;
            //DateTime StartDeletionForFile = DateTime.Now; 

            //Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            //if (Gp.RecordFound == true)
            //{
            //    DeleteDays = (int)Gp.Amount; // 

            //    //AgingDays_HST = 0; 

            //    // Current CutOffdate
            //    string WSelection = " WHERE JobCycle =" + WReconcCycleNo;
            //    Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

            //    DeleteDateLimit = Rjc.Cut_Off_Date.AddDays(-DeleteDays);

            //    //WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-0);

            //    Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(DeleteDateLimit);

            //    if (Rjc.RecordFound == true)
            //    {
            //        S_DeleteDateLimit = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

            //        //MessageBox.Show("DELETE RECORDS FROM HST Starts" + Environment.NewLine
            //        //       + "For date equal or less than.." + S_DeleteDateLimit
            //        //       );
            //        DeleteRecords = true;
            //    }
            //}

            //if (MessageBox.Show("Do you want to delete from History records " + Environment.NewLine
            //                    + "Less than date "+ S_DeleteDateLimit +"...????" 
            //                    //+ Mt.W_MPComment
            //                    , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            //             == DialogResult.Yes)
            //{
            //    // YES Proceed
            //}
            //else
            //{
            //    // Stop 
            //    return;
            //}

            //string WFileName = "";
            ////MoveToHistory = true;
            //if (DeleteRecords == true)
            //{
            //    //******************************
            //    // 
            //    // DELETE FROM HST 
            //    // 
            //    //******************************
            //    int Mode = 17; // Updating Action 
            //    ProcessName = "DELETE Records from HST Data Base";
            //    Message = "DELETE RECORDS STARTS for Days before:.." + S_DeleteDateLimit;
            //    SavedStartDt = DateTime.Now;

            //    Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);

            //    string TotalProgressText = "Delete Cycle: ..."
            //                             + WReconcCycleNo.ToString() + Environment.NewLine;
            //    //RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            //    //RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            //    string WSelectionCriteria = " WHERE Operator = @Operator ";
            //    Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

            //    int I = 0;
            //    int K = 0;

            //    while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
            //    {
            //        //    RecordFound = true;
            //        int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
            //        Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

            //        if (Mf.IsMoveToMatched == true || Mf.SourceFileId == "Atms_Journals_Txns") // the indication that this table is a moving table 
            //        {
            //            if (Mf.SourceFileId == "Atms_Journals_Txns")
            //            {
            //                WFileName = "tblMatchingTxnsMasterPoolATMs";
            //            }
            //            else
            //            {
            //                WFileName = Mf.SourceFileId;
            //            }

            //            // Check That File Exist in target data base 
            //            string TargetDB = "[RRDM_Reconciliation_ITMX_HST]";
            //            Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

            //            if (Cv.RecordFound == true)
            //            {
            //                // File Exist
            //            }
            //            else
            //            {
            //                // File do not exist
            //                MessageBox.Show("File.." + WFileName + Environment.NewLine
            //                    + "DOES NOT EXIST In ITMX_HST Data Base."
            //                    + "REPORT TO THE HELP DESK."
            //                    );
            //                I = I + 1;
            //                continue;
            //            }

            //            // Check That File Exist in target data base 
            //            TargetDB = "[RRDM_Reconciliation_MATCHED_TXNS_HST]";
            //            Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

            //            if (Cv.RecordFound == true)
            //            {
            //                // File Exist
            //            }
            //            else
            //            {
            //                // File do not exist
            //                MessageBox.Show("File.." + WFileName + Environment.NewLine
            //                    + "DOES NOT EXIST In MATCHED_TXNS_HST Data Base."
            //                    + "REPORT TO THE HELP DESK."
            //                    );
            //                I = I + 1;
            //                continue;
            //            }

            //            // START DELETION 
            //            StartDeletionForFile = DateTime.Now;

            //            Cv.DELETE_TXNS_FROM_HST(WFileName, WReconcCycleNo, DeleteDateLimit);

            //        }

            //        Mode = 17; // 
            //        ProcessName = "DELETE Records from HST Data Base" ;
            //        Message = "DELETED RECORDS for FILE.." + WFileName + "..Number=."+Cv.TotalDeleted.ToString();
            //        DateTime FinishDeletion = DateTime.Now; 
            //        Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", StartDeletionForFile, FinishDeletion, Message, WSignedId, WReconcCycleNo);

            //        I = I + 1;
            //    }

            //    //TotalProgressText = TotalProgressText + DateTime.Now + " DELETE FROM HST has finished" + Environment.NewLine;
            //    //TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

            //    // *****************************

            //    Mode = 17; // Updating Action 
            //    ProcessName = "DELETE Records from HST Data Base";
            //    Message = "DELETE RECORDS HAS FINISHED.for Days before:.." + S_DeleteDateLimit; 

            //    textBoxMsgBoard.Text = "Current Status : Ready";

            //    Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //    //*********************************

            //    MessageBox.Show("Delete of Records has finished");

            //    Form8_Traces_Oper NForm8_Traces_Oper;
            //    int MMode = 4; 
            //    NForm8_Traces_Oper = new Form8_Traces_Oper(WSignedId, WSignRecordNo, "7", WOperator, MMode);
            //    NForm8_Traces_Oper.ShowDialog();
                // Form502_Load(this, new EventArgs());
           // }
        }
// Show Deleted 
        private void buttonSummaryOfDeleted_Click(object sender, EventArgs e)
        {
            Form8_Traces_Oper NForm8_Traces_Oper;
            int MMode = 4;
            NForm8_Traces_Oper = new Form8_Traces_Oper(WSignedId, WSignRecordNo, "7", WOperator, MMode);
            NForm8_Traces_Oper.ShowDialog();
        }
// View BULK 
        private void buttonViewBULK_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            string MinDateTm = "";
            string MaxDateTm = "";
            Flog.ReadLoadedFilesBySeqNo(WSeqNoLoadedFile);

            MessageBox.Show("It will show only the todays records of file.."+ Flog.SourceFileID);
           
            // SHOW BULK
            //
            Form78d_BULK_Records NForm78d_BULK_Records;
            //int WMode = 4; // 
            NForm78d_BULK_Records = new Form78d_BULK_Records(WOperator, WSignedId, Flog.SourceFileID);
            NForm78d_BULK_Records.ShowDialog();
        }
        // Invalid CIT Entries 
        readonly string recconConnString = ConfigurationManager.ConnectionStrings["ReconConnectionString"].ConnectionString;
        private void buttonInvalidCit_Click(object sender, EventArgs e)
        {
            Form18_CIT_ExcelOutput_Alerts_BDC NForm18_CIT_ExcelOutput_Alerts_BDC;
            // string InSignedId, int SignRecordNo, string InOperator
            NForm18_CIT_ExcelOutput_Alerts_BDC = new Form18_CIT_ExcelOutput_Alerts_BDC(WSignedId, WSignRecordNo, WOperator);
            NForm18_CIT_ExcelOutput_Alerts_BDC.ShowDialog();
            return; 

            // Check if CIT excel is operational do the following 
            //RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            string WFileName_CIT = "CIT_EXCEL_TO_BANK";
            string TargetDB_CIT = "[RRDM_Reconciliation_ITMX]";
            Cv.ReadTableToSeeIfExist(TargetDB_CIT, WFileName_CIT);
            // CHECK THE ATMS THAT DO NOT HAVE A CORRESPONDING ATM Group and OWNER 
            if (Cv.RecordFound == true)
            {
                //
                // UPDATE OTHER INFO FROM UsersAtmTable
                //
                string SQLCmd =
              " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
              + " SET  "
              + " GroupOfAtmsRRDM = t2.GroupOfAtms "
              + ",UserId = t2.UserId "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] t1 "
              + " INNER JOIN [ATMS].[dbo].[UsersAtmTable] t2"
              + " ON t1.AtmNo = t2.AtmNo "

              + " WHERE t1.GroupOfAtmsRRDM = 0  "; // Upade ONLY These without Group 

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                           // cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                            cmd.CommandTimeout = 350;
                            Counter = cmd.ExecuteNonQuery();
                            var stats = conn.RetrieveStatistics();
                            //commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                        }
                        // Close conn
                        conn.StatisticsEnabled = false;

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.StatisticsEnabled = false;

                        conn.Close();
                       
                        CatchDetails(ex);
                        return;
                    }

                Form78d_Matched NForm78d_Matched;
                int _WMode = 5; // For Excel entries without ATM Group and Owner 
                NForm78d_Matched = new Form78d_Matched(WOperator, WSignedId, textBoxCateg.Text, WReconcCycleNo, _WMode = 5);
                NForm78d_Matched.ShowDialog();
            }

        }
    }

}



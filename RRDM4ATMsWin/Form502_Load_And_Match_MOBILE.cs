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
    public partial class Form502_Load_And_Match_MOBILE : Form
    {

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        //RRDM_LoadFiles_InGeneral_EMR_BDC Lf_ABE = new RRDM_LoadFiles_InGeneral_EMR_BDC();

        RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_QAHERA Lf_BDC_QAH = new RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_QAHERA();

        RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_ETISALAT Lf_BDC_ETI = new RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_ETISALAT();

        RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_EGATE Lf_EGATE = new RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_EGATE();

        RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MOBILE Mt = new RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MOBILE();

        RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MOBILE_EGATE Mte = new RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MOBILE_EGATE();

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

        string W_Application;

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

        public Form502_Load_And_Match_MOBILE(string InSignedId, int SignRecordNo, string InOperator
                                                              , int InReconcCycleNo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WReconcCycleNo = InReconcCycleNo;
            WMode = InMode; // If 1 then is called from the administrator
                            // If 2 then is called for view only 

            InitializeComponent();



            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;

                if (W_Application == "e_MOBILE")
                {
                    if (Usi.WFieldNumeric11 == 11)
                    {
                        W_Application = "ETISALAT";
                    }
                    if (Usi.WFieldNumeric11 == 12)
                    {
                        W_Application = "QAHERA";
                    }
                    if (Usi.WFieldNumeric11 == 13)
                    {
                        W_Application = "IPN";
                    }
                    if (Usi.WFieldNumeric11 == 15)
                    {
                        W_Application = "EGATE";
                    }
                    labelStep1.Text = "Load And Match_ " + W_Application;

                    label5.Hide();
                    panel3.Hide();
                }
                else
                {
                    W_Application = "ATMs";
                    if (Usi.WFieldNumeric11 == 10)
                    {
                       // T24Version = true;
                        labelStep1.Text = "Controller's Menu-VER_T24_" + W_Application;
                    }
                    else
                    {
                        labelStep1.Text = "Controller's Menu-_" + W_Application;
                    }

                }
            }

            if (W_Application == "EGATE")
            {
                buttonGL_AND_Disputes.Hide();
                buttonTEST_4_GRIDS.Hide();
                buttonActionsTaken.Hide();
                buttonDisputePreInvestigation.Hide(); 
            }

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

                buttonLoadFiles.Enabled = false;
                buttonDoMatching.Enabled = false;
                label3.Text = "THE CYCLES";
                buttonExportToExcel.Hide();
                //panelFiles.Hide();
                // panelUndo.Hide();

                label6.Hide();
               // label7.Hide();
                label9.Hide();

                textBoxNumberOfFiles.Hide();
                textBoxFilesReady.Hide();
                //textBoxNotReady.Hide();

                buttonShowComment.Hide();


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
            string WJobCategory = "";

            //    string WOperator = "BCAIEGCX";       
            if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "IPN" || W_Application == "EGATE")
            {
                WJobCategory = W_Application; 
            }
            

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
            ShowScreen();
        }
        // 
        // SHOW SCREEN
        //

        private void ShowScreen()
        {
            if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "IPN" || W_Application == "EGATE")
            {
                // Get only the e_MOBILE
                string Prefix = W_Application.Substring(0, 3);
                string WSelection = "where Left(CategoryId,3) = '" + Prefix + "' AND RunningJobNo = " + WReconcCycleNo;
                Rcs.ReadReconcCategoriesSessions_To_Check_If_MatchingDONE_MOBILE(WSelection);
            }
            else
            {
                Rcs.ReadReconcCategoriesSessions_To_Check_If_MatchingDONE(WReconcCycleNo);
            }

            if (Rcs.RecordFound == true)
            {
                labelFieldsDefinition.Show();
                panelCategories.Show();
                //buttonMatching.Show();
            }
            else
            {
                labelFieldsDefinition.Hide();
                panelCategories.Hide();
               // buttonMatching.Hide();
            }

            if (WMode == 2)
            {

                // View Only
                labelStep1.Text = "View loaded amd Matched of Cycle_" + WReconcCycleNo.ToString()
                               + "_and Date_" + WCut_Off_Date.ToShortDateString();
            }
            else
            {
                // CheckLoadingOfJournals();
            }
            //if (J_UnderLoading == true)
            //{
            //}
            //  JournalTotals();
            // Other totals to fill grid
            GetTotals();

            radioButtonCategory.Checked = true;
            if (WMode == 1)
            {
                // Source Files (Grid-ONE)
                // Read them and check them 
                if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "IPN" || W_Application == "EGATE")
                {
                    string Filter1 = "";
                    // Get only the e_MOBILE

                    Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1 AND SystemOfOrigin='" 
                        + W_Application + "' AND Right(SourceFileId, 5) <> 'TWINS' ";


                    Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

                }
                else
                {
                    // Normal Case
                    string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1  AND TableStructureId = 'Atms And Cards' ";

                    Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

                }
                int TotalFiles = 0;
                int TotalReady = 0; 
                int I = 0;

                while (I <= (Rs.Table_Files_In_Dir.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    string SourceFileId = (string)Rs.Table_Files_In_Dir.Rows[I]["SourceFileId"];
                    string FullFileName = (string)Rs.Table_Files_In_Dir.Rows[I]["FullFileName"];
                    bool IsPresent = (bool)Rs.Table_Files_In_Dir.Rows[I]["IsPresent"];
                    string IsGood = (string)Rs.Table_Files_In_Dir.Rows[I]["IsGood"];
                    string DateExpected = (string)Rs.Table_Files_In_Dir.Rows[I]["DateExpected"];
                    string HASHValue = (string)Rs.Table_Files_In_Dir.Rows[I]["HASHValue"];


                    if (IsPresent == true )
                    {
                        TotalFiles = TotalFiles+1;
                    }
                    if (IsPresent == true & IsGood == "YES")
                    {
                        TotalReady = TotalReady + 1;
                    }
                    I = I + 1;
                }


                textBoxNumberOfFiles.Text = TotalFiles.ToString();
                textBoxFilesReady.Text = textBoxInDirForLoading.Text = TotalReady.ToString();

                ShowGrid1();


                if (Rs.WFutureFiles > 0)
                {
                    textBoxMsgBoard.Text = "There are other Dates files in directories ..No:.." + Rs.WFutureFiles.ToString();
                }
            }
            else
            {

                string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND JobCategory ='"+ W_Application+"'" ;
                Rjc.ReadReconcJobCyclesFillTable(SelectionCriteria);
                ShowCycles();

            }


        }
        // Row FIRST GRID
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
            Flog.ReadLoadedFiles_Fill_Table_MOBILE(WOperator, WReconcCycleNo);

            ShowGrid2();

            Rc.ReadReconcCategoriesForMatrix_MOBILE(WOperator, WReconcCycleNo, W_Application);
            if (Rc.TableReconcCateg.Columns.Count == 0)
            {
                string Test = "1";
            }
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

            if (Rs.Table_Files_In_Dir.Columns.Count == 0)
            {
                MessageBox.Show("No Files to show");
                return;
            }

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
            dataGridViewFilesStatus.DataSource = Lf_BDC_QAH.SourceFilesTotals.DefaultView;

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



        // Show ready Categories
        private void buttonShowReadyCateg_Click(object sender, EventArgs e)
        {

            // Find the ready categories
            if (W_Application == "EGATE")
            {
                Mte.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                    WReconcCycleNo, W_Application);

                MessageBox.Show(Mte.W_MPComment);
            }
            else
            {
                // All other Mobile 
                Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                    WReconcCycleNo, W_Application);

                MessageBox.Show(Mt.W_MPComment);
            }
            
        }

        int WServiceReqID;
        bool CommandSent;

        // Service status
        string serviceStatus;

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

                // textBox1.Text = allFiles.Length.ToString();

                RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();

                Rfm.GetTotals(WReconcCycleNo);

                // textBox2.Text = Rfm.ValidTotalForCycle.ToString();
                //     textBox2.Text = Counter.ToString();
                //  textBox3.Text = Rfm.InValidTotalForCycle.ToString();

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
                    if (W_Application == "EGATE")
                    {
                        Mte.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                               WReconcCycleNo, W_Application);
                        //  textBoxMatchedCateg.Text = Mt.ENQ_CategForMatch;  

                        if (Mte.ENQ_NumberOfCatToBeMatched == 0)
                        {
                            CategoriesAreMatched = true;
                        }
                    }
                    else
                    {
                        Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                               WReconcCycleNo, W_Application);
                    //  textBoxMatchedCateg.Text = Mt.ENQ_CategForMatch;  

                    if (Mt.ENQ_NumberOfCatToBeMatched == 0)
                    {
                        CategoriesAreMatched = true;
                    }
                    }

                        
                }

                //Form502_Load(this, new EventArgs());
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
        private void buttonDoMatching_Click(object sender, EventArgs e)
        {

            // Start Service
            //buttonMatching.Show();
            // Find the ready categories
            string W_Message; 

            if (W_Application == "EGATE")
            {
                Mte.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                      WReconcCycleNo, W_Application);

                W_Message = Mte.W_MPComment;
            }
            else
            {
                Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                     WReconcCycleNo, W_Application);

                W_Message = Mt.W_MPComment; 

            }

                

            if (MessageBox.Show("Do you want to Start Matching?" + Environment.NewLine
                                 + "For Ready Categories " + Environment.NewLine
                                 + W_Message
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
                                                          //MessageBox.Show("Transactions will be deleted prior to..Date.." + DeleteDate.ToShortDateString());
                                                          //  Lf_BDC_MOB.DeleteRecordsToSetStartingPoint(WOperator, DeleteDate, WCut_Off_Date, WReconcCycleNo, 1);
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

            //Lf_BDC_MOB.UpdateRecordsWithTraceAndOther(WOperator, WReconcCycleNo, 0, 2, WCut_Off_Date);

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
            //RRDMAtmsMinMax Mm = new RRDMAtmsMinMax();
            //Mm.DeleteTableATMsMinMax();

            string text = "Matching Starts Now" ;
            string caption = "Matching Process";
            int timeout = 2000;
            if (Environment.UserInteractive)
            {
                AutoClosingMessageBox.Show(text, caption, timeout);
            }

            //Thread thr16 = new Thread(Method16);
            //thr16.Start();
            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "MatchingProcess";
            Message = "Matching Process Starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
            //******************************

            //****************************************************
            int WMode = 2; // Do matching for all ready categories 
            //
            if (W_Application == "EGATE")
            {
                Mte.MatchReadyCategoriesUpdate(WOperator, WSignedId,
                                           WReconcCycleNo, W_Application);
            }
            else
            {
                Mt.MatchReadyCategoriesUpdate(WOperator, WSignedId,
                                           WReconcCycleNo, W_Application);
            }
            
            // *****************************

            Mode = 5; // Updating Action 
            ProcessName = "MatchingProcess";
            Message = "Matching Process Finishes.";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            // CHECK IF DONE
            // READ GL ENTRIES BY CUT OFF DATE TO FIND OUT IF ALREADY DONE
           // Lf_BDC_ETI.Check_If_GL_ALREADY_Exist(int InRMCycle, DateTime WCutOffDate);
            Lf_BDC_ETI.Check_If_GL_ALREADY_Exist(WReconcCycleNo, WCut_Off_Date);

            if (Lf_BDC_ETI.RecordFound == true)
            {
                // SKIP GL already created 
                MessageBox.Show(" GL already Created we will not create again ");
            }
            else
            {
                // CREATE GL 
                int ReturnCode = -20;
                string ErrorText = "";
                string ErrorReference = "";
                int ret = -1;
                // THIS STORE PROCEDURE IS IN ATMS STore Procedures 
                string connectionString = ConfigurationManager.ConnectionStrings
                                              ["ATMSConnectionString"].ConnectionString;
                // ETISALAT_TPF_FAWRY_TXNS
                // WE LOAD TPF. After we look at Fawry to find out which ones are for Settlement 
                int rows = 0;
                string SPName = "";
                //
                // stp_ETISALAT_GL_ENTRIES
                //
                if (W_Application == "ETISALAT")
                {
                    SPName = W_Application + ".[dbo].[stp_" + W_Application + "_GL_ENTRIES]";

                    using (SqlConnection conn2 = new SqlConnection(connectionString))
                    {
                        try
                        {

                            conn2.Open();

                            SqlCommand cmd = new SqlCommand(SPName, conn2);

                            cmd.CommandType = CommandType.StoredProcedure;

                            // the first are input parameters

                            cmd.Parameters.Add(new SqlParameter("@RMCycleNo", WReconcCycleNo));

                            //cmd.Parameters.Add(new SqlParameter("@businessDate", WCut_Off_Date));
                            // cmd.Parameters.Add(new SqlParameter("@businessDate", "2024-02-01"));

                            // the following are output parameters

                            SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                            retCode.Direction = ParameterDirection.Output;
                            retCode.SqlDbType = SqlDbType.Int;
                            cmd.Parameters.Add(retCode);

                            SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                            retErrorText.Direction = ParameterDirection.Output;
                            retErrorText.SqlDbType = SqlDbType.NVarChar;
                            retErrorText.Size = 1024;
                            cmd.Parameters.Add(retErrorText);

                            SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                            retErrorReference.Direction = ParameterDirection.Output;
                            retErrorReference.SqlDbType = SqlDbType.NVarChar;
                            retErrorReference.Size = 40;
                            cmd.Parameters.Add(retErrorReference);

                            // execute the command
                            cmd.CommandTimeout = 300;  // seconds
                            cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                            ret = (int)cmd.Parameters["@ReturnCode"].Value;
                            //ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                            conn2.Close();

                        }
                        catch (Exception ex)
                        {
                            conn2.Close();
                            CatchDetails(ex);
                        }

                        if (ret == 0)
                        {

                            // OK
                            //MessageBox.Show("VALID CALL" + Environment.NewLine
                            //            + ProgressText);
                        }
                        else
                        {
                            // NOT OK
                            MessageBox.Show("NOT VALID CALL for GL Transactions" + Environment.NewLine
                                     );
                        }


                    } // here
                }

            }


            //*********************************

            textBoxMsgBoard.Text = "Current Status : Moving Records Process";

            text = "Matching has Finished" + Environment.NewLine
                + "Process of Moving records starts.";
            caption = "Moving Records";
            timeout = 2000;
            if (Environment.UserInteractive)
            {
                AutoClosingMessageBox.Show(text, caption, timeout);
            }

            

            Mode = 5; // Updating Action 
            ProcessName = "Moving Records To MATCHED Data Base";
            Message = "Moving Records Process Starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

           Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);

            
            //******************************
            // 
            // MOVE MATCHED TXNS POOL 
            // stp_00_MOVE_TXNS_TO_MATCHED_DB_01_POOL
            //******************************
            string TotalProgressText = "Cycle: ..."
                                     + WReconcCycleNo.ToString() + Environment.NewLine;
            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice_MOBILE Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice_MOBILE();
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            string WSelectionCriteria = " WHERE Operator = @Operator AND SystemOfOrigin='" + W_Application + "'";
            Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

            int I = 0;
            int K = 0;
            string WFileName = "";
            string TargetDB = "";

            if (W_Application== "EGATE")
            {
                // MOVE first the Master that it is not defined as input file
                // Then move the

                TargetDB = "[" + W_Application + "_MATCHED_TXNS]";
                WFileName = "TXNS_MASTER"; 
                Cv.MOVE_TXNS_TO_MATCHED_PER_TABLE_NAME(WFileName, WReconcCycleNo, W_Application);

                if (Cv.ret == 0)
                {
                    // GOOD
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

                }

                // MOVE THE REST OF FILES

                while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                    Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                    if (Mf.IsMoveToMatched == true) // the indication that this table is a moving table 
                    {
                        
                            WFileName = Mf.SourceFileId;
                        

                        // Check That File Exist in target data base 
                        TargetDB = "[" + W_Application + "_MATCHED_TXNS]";
                        Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (Cv.RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            //MessageBox.Show("File.." + WFileName + Environment.NewLine
                            //    + "DOES NOT EXIST In MATCHED_TXNS Data Base."
                            //    + "REPORT TO THE HELP DESK."
                            //    );
                            I = I + 1;
                            continue;
                        }

                        //Cv.MOVE_TXNS_TO_MATCHED_MOBILE(WFileName, WReconcCycleNo, W_Application);
                        Cv.MOVE_TXNS_TO_MATCHED_PER_TABLE_NAME(WFileName, WReconcCycleNo, W_Application);
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

            }
            else
            {
                // ETISALAT and Other E_WALLETS
                while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                    Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                    if (Mf.IsMoveToMatched == true

                        ) // the indication that this table is a moving table 
                    {
                        if (
                         Mf.SourceFileId == "ETISALAT_TPF_TXNS"
                        || Mf.SourceFileId == "QAHERA_TPF_TXNS"
                        || Mf.SourceFileId == "IPN_TPF_TXNS"
                        )
                        {
                            //WFileName = "tblMatchingTxnsMasterPoolATMs";
                            WFileName = Mf.SourceFileId + "_MASTER";
                        }
                        else
                        {
                            WFileName = Mf.SourceFileId;
                        }

                        // Check That File Exist in target data base 
                        TargetDB = "[" + W_Application + "_MATCHED_TXNS]";
                        Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (Cv.RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            //MessageBox.Show("File.." + WFileName + Environment.NewLine
                            //    + "DOES NOT EXIST In MATCHED_TXNS Data Base."
                            //    + "REPORT TO THE HELP DESK."
                            //    );
                            I = I + 1;
                            continue;
                        }

                        //Cv.MOVE_TXNS_TO_MATCHED_MOBILE(WFileName, WReconcCycleNo, W_Application);
                        Cv.MOVE_TXNS_TO_MATCHED_PER_TABLE_NAME(WFileName, WReconcCycleNo, W_Application);
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

            // FIND LIMIT DATE FOR HISTORY 

            // MOVE FROM MATCHED TO MATCHED_HST
            //WSelection = " WHERE JobCycle =" + InReconcCycleNo;
            //Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);
            bool MoveToHistory = false;
            string ParamId = "853";
            string OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true)
            {
                int AgingDays_HST = (int)Gp.Amount; // 

                //AgingDays_HST = 0; 

                // Current CutOffdate
                string WSelection = " WHERE JobCycle =" + WReconcCycleNo;
                Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

                DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

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
            //MoveToHistory = true;
            if (MoveToHistory == true & W_Application !="EGATE")
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
                WSelectionCriteria = " WHERE Operator = @Operator AND SystemOfOrigin='" + W_Application + "'";
                Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

                I = 0;
                K = 0;

                while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                    Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                    if (Mf.IsMoveToMatched == true) // the indication that this table is a moving table 
                    {
                        if (
                     Mf.SourceFileId == "ETISALAT_TPF_TXNS"
                    || Mf.SourceFileId == "QAHERA_TPF_TXNS"
                    || Mf.SourceFileId == "IPN_TPF_TXNS"
                    )
                        {
                            //WFileName = "tblMatchingTxnsMasterPoolATMs";
                            WFileName = Mf.SourceFileId + "_MASTER";
                        }
                        else
                        {
                            WFileName = Mf.SourceFileId;
                        }

                        // Check That File Exist in target data base 
                        //string TargetDB = "[RRDM_Reconciliation_ITMX_HST]";
                        TargetDB = "[" + W_Application + "_MATCHED_TXNS_HST]";
                        Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (Cv.RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            //MessageBox.Show("File.." + WFileName + Environment.NewLine
                            //    + "DOES NOT EXIST In ITMX_HST Data Base."
                            //    + "REPORT TO THE HELP DESK."
                            //    );
                            I = I + 1;
                            continue;
                        }

                        // Check That File Exist in target data base 
                        //TargetDB = "[RRDM_Reconciliation_MATCHED_TXNS_HST]";
                        //Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                        //if (Cv.RecordFound == true)
                        //{
                        //    // File Exist
                        //}
                        //else
                        //{
                        //    // File do not exist
                        //    //MessageBox.Show("File.." + WFileName + Environment.NewLine
                        //    //    + "DOES NOT EXIST In MATCHED_TXNS_HST Data Base."
                        //    //    + "REPORT TO THE HELP DESK."
                        //    //    );
                        //    I = I + 1;
                        //    continue;
                        //}

                        // MessageBox.Show("Start Moving file.." + WFileName); 

                        Cv.MOVE_TXNS_TO_MATCHED_MOBILE_HST(WFileName, WReconcCycleNo, W_Application);
                       
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

            if (W_Application != "EGATE")
            {
                // 
                // 
                string connectionString = ConfigurationManager.ConnectionStrings
                                               ["ATMSConnectionString"].ConnectionString;

                string RCT = "[ATMS].[dbo].[Stp_00_UPDATE_DB_System_Stats]";

                using (SqlConnection conn =
            new SqlConnection(connectionString))
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
            }

                

            text = "Matching and movings of records process has finished";
            caption = "Process Completion";
            timeout = 2000;
            if (Environment.UserInteractive)
            {
                AutoClosingMessageBox.Show(text, caption, timeout);
            }

            
            //Form502_Load(this, new EventArgs());
            ShowScreen();

        }
        //
        // LOAD FILES
        //
        string FullFileName;

        private void buttonLoadFiles_Click(object sender, EventArgs e)
        {
            //CheckLoadingOfJournals();

            //if (J_UnderLoading == true)
            //{
            //    MessageBox.Show("Journals Under Loading");
            //    return;
            //}
            // Before start we check the sign on users. 
            //bool IsAllowedToSignIn = false;
            //bool ThereAreUsersInSystem = CheckForSignInUsers(IsAllowedToSignIn);

            //if (ThereAreUsersInSystem == true)
            //{
            //    // Decide whether to move forward or not. 
            //}

            // Truncate Table From Audi - tmplog for Pambos
            //string WFile = "[ATM_MT_Journals_AUDI].[dbo].[tmplog]";
            //Jc.TruncateTempTable(WFile);

            // Truncate Table working table 1 
            //string WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Working_General_Table]";

            //Jc.TruncateTempTable(WFile);

            //// Truncate Table 2 
            //WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Working_Master_Pool_Report]";

            //Jc.TruncateTempTable(WFile);

            bool Is_SuperisorMode = false;
            //string ParId = "720";
            //string OccurId = "1";
            ////RRDMGasParameters Gp = new RRDMGasParameters();

            //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            //if (Gp.OccuranceNm == "YES")
            //{
            //    Is_SuperisorMode = true;
            //}
            //else
            //{
            //    Is_SuperisorMode = false;
            //}

            //if (Is_SuperisorMode == true)
            //{
            //    MessageBox.Show("Supervisor mode Work starts");
            //    textBoxMsgBoard.Text = "Current Status : Supervisor Mode data loading process";
            //    //
            //    // Supervisor mode
            //    //
            //    RRDMRepl_SupervisorMode_Master Smaster = new RRDMRepl_SupervisorMode_Master();
            //    int Sm_Mode = 3;
            //    Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, WSignRecordNo, WOperator,
            //                                                     Sm_Mode);

            //    MessageBox.Show("Supervisor mode Work Finishes");

            //}

            //// return; // for testing 

            //MessageBox.Show("Loading of files starts." + Environment.NewLine
            //                + "" + Environment.NewLine
            //               );

            textBoxMsgBoard.Text = "Current Status : Loading of Files process";


            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfFiles";
            Message = "Loading Of Files starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);


            // 
            //IST 01 / 07

            if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "IPN" || W_Application == "EGATE")
            {
                // Get only the e_MOBILE

                string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1 AND SystemOfOrigin='" 
                    + W_Application + "' AND Right(SourceFileId, 5) <> 'TWINS' ";

                Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);
            }
            else
            {
                //// Normal Case
                //string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1  AND TableStructureId = 'Atms And Cards' ";

                //Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

            }


            SavedSourceFilesDataTable = Rs.Table_Files_In_Dir;

            buttonLoad.Show();

            // LOAD FILES
            //******************************


            METHOD_LoadFiles();

            //******************************
            //******************************


            string Temp = "Panicos2"; //  Check Authorisations
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
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfFiles";
            Message = "Loading Of Files Finishes. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************

            string text = "Loading Of Files has Finished " ;
            string caption = "LOADING OF FILES";
            int timeout = 2000;
            if (Environment.UserInteractive)
            {
                AutoClosingMessageBox.Show(text, caption, timeout);
            }

            textBoxMsgBoard.Text = "Current Status : Ready";
            // Refresh 
            //Form502_Load(this, new EventArgs());
            ShowScreen();

            //Thread thr15 = new Thread(Method15);
            //thr15.Start();
        }

        //
        // UNDO MATCHING FOR EGATE
        //

        private void UndoMatchingAndLoading_EGATE()
        {
            // PROCESS
            // 1) MOVE TRANSACTIONS FROM MATCHED TABLES TO EGATE data base 
            //                    
            // 2) USING A STORE PROCEDURE DELETE AND UPDATE OF RRDM GENERAL TABLES 
            // 3) USE AN RRDM CLASS TO DELETE UPDATE TABLES IN EGATE eg Delete loaded records 

            if (MessageBox.Show("Do you want to UNDO Loading and Matching of EGATE " + Environment.NewLine
                                 + "For this Cycle ...? " + Environment.NewLine
                                 + WReconcCycleNo.ToString()
                                 , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                // YES Proceed
               
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
            Message = "UNDO Matching and Loading of Files STARTS CYCLE.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
            // BEFORE UNDO 
            string TotalProgressText = DateTime.Now + "Moving TXNs from Matched to ITMX Starts" + Environment.NewLine;
            string connectionStringITMX = ConfigurationManager.ConnectionStrings
                     ["ReconConnectionString"].ConnectionString;
            //********************
            //***********************
            bool Panicos = true; // MOVE FROM MATCHED TO EGATE


            if (Panicos == true)
            {
                RRDM_Copy_Txns_From_IST_ToMatched_And_Vice_MOBILE Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice_MOBILE();
                RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();

                string WSelectionCriteria = " WHERE Operator = @Operator AND SystemOfOrigin='" + W_Application + "'";
                Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

                int I = 0;
                int K = 0;
                string WFileName;

                // Handle first the Master 
                // Then move the

                string TargetDB = "[" + W_Application + "_MATCHED_TXNS]"; // The matched data base
                WFileName = "TXNS_MASTER";
                Cv.MOVE_MATCHED_TXNS_TO_PRIMARY_PER_TABLE_NAME(WFileName, WReconcCycleNo, W_Application);

                if (Cv.ret == 0)
                {
                    // GOOD
                    TotalProgressText = TotalProgressText + Cv.ProgressText;
                }
                else
                {
                    
                    MessageBox.Show("VITAL SYSTEM ERROR" + Environment.NewLine
                                   + "PROGRESS TEXT.." + Cv.ProgressText + Environment.NewLine
                                   + "ERROR REFERENCE.." + Cv.ErrorReference + Environment.NewLine
                                   + ""
                                   );

                }

                while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                    Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                    if (Mf.IsMoveToMatched == true) // the indication that this table is a moving table 
                    {
                       
                        WFileName = Mf.SourceFileId;

                        // Check That File Exist in target data base 

                        //Cv.MOVE_TXNS_TO_MATCHED_MOBILE(WFileName, WReconcCycleNo, W_Application);
                        Cv.MOVE_MATCHED_TXNS_TO_PRIMARY_PER_TABLE_NAME(WFileName, WReconcCycleNo, W_Application);
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
                                           + WFileName
                                           );

                            // return;

                        }

                    }

                    I = I + 1;
                }


                TotalProgressText = TotalProgressText + DateTime.Now + " MOVING OF TXNS FROM MATCHED HAS FINISHED" + Environment.NewLine;
                TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

              
            }

            //******************************
            // UNDO FILES AFTER MOVING THEM FROM MATCHED TO
            //******************************
            // CALL STORE PROCEDURE TO UNDO THE REST
            // THESE ARE FILES LIKE Reconciliation Sessions etc. 

            int ReturnCode = -20;
            string ProgressText = "";
            string ErrorReference = "";
            int ret = -20;
            // THIS STORE PROCEDURE IS IN ATMS STore Procedures 
            string connectionString = ConfigurationManager.ConnectionStrings
                                          ["ATMSConnectionString"].ConnectionString;

            string SPName = "[ATMS].[dbo].[stp_00_UNDO_RMCYCLE_FILES_MATCHING_MOBILE_EGATE]";

            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                try
                {

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", WReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@Application", W_Application));

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
                    //ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }

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

                //WReconcCycleNo = 1287;


            }
            // Turn the process code to 0 with loading has become to -1 and with mathing to 1  
            RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();
            Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToZeroForCycle(WReconcCycleNo);
            //
            // CALL RRDM CLASS TO DELETE AND ALSO UPDATE RECORDS OF THIS CYCLE 
            //
            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

            Msf.ReadFilesAND_UNDO_For_Cycle_MOBILE(W_Application, WOperator, WReconcCycleNo);
            ProgressText = ProgressText + Msf.ProgressText_2;

            // DELETE LOADED 
            //WReconcCycleNo = 1285; 
           
            Flog.Delete_MOBILE(WReconcCycleNo);
 

            //RRDMJournalReadTxns_Text_Class Ed = new RRDMJournalReadTxns_Text_Class();
            RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

            Ed.LoopFor_MoveFile_From_Archive_ToOrigin_Directory_MOBILE(W_Application, WOperator, WReconcCycleNo, ReversedCut_Off_Date);

            // *****************************

            Mode = 5; // Updating Action 
            ProcessName = "UNDO_LoadingAndMatchingProcess";
            Message = "UNDO Matching and Loading of Files FINISHES. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************

            //Form502_Load(this, new EventArgs());
            ShowScreen();
        }

        private void buttonUNDO_Click(object sender, EventArgs e)
        {
            if (W_Application == "EGATE")
            {
                UndoMatchingAndLoading_EGATE();
                //return; 
            }
            else
            {
                // ETISALAT AND OTHERS 
                // PROCESS
                // 1) MOVE TRANSACTIONS FROM MATCHED TABLES TO QAHERA 
                //                         (TXNS IN MATCHED ARE DELETED)  
                // 2) USING A STORE PROCEDURE DELETE AND UPDATE OF RRDM GENERAL TABLES 
                // 3) USE AN RRDM CLASS TO DELETE UPDATE TABLES IN QAHERA eg Delete loaded records 

                if (MessageBox.Show("Do you want to UNDO Loading and Matching " + Environment.NewLine
                                     + "For this Cycle ...? " + Environment.NewLine
                                     + WReconcCycleNo.ToString()
                                     , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                              == DialogResult.Yes)
                {
                    // YES Proceed
                    //MessageBox.Show("This process might take few minutes. " + Environment.NewLine
                    //               + "Wait till a final message is shown. ");
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
                Message = "UNDO Matching and Loading of Files STARTS CYCLE.." + WReconcCycleNo.ToString();
                SavedStartDt = DateTime.Now;

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
                // BEFORE UNDO 
                string TotalProgressText = DateTime.Now + "Moving TXNs from Matched to ITMX Starts" + Environment.NewLine;
                string connectionStringITMX = ConfigurationManager.ConnectionStrings
                         ["ReconConnectionString"].ConnectionString;
                //********************
                //***********************
                bool Panicos = true; // MOVE FROM MATCHED TO ETI OR QAHERA 


                if (Panicos == true)
                {
                    RRDM_Copy_Txns_From_IST_ToMatched_And_Vice_MOBILE Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice_MOBILE();
                    RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();

                    string WSelectionCriteria = " WHERE Operator = @Operator AND SystemOfOrigin='" + W_Application + "'";
                    Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

                    int I = 0;
                    int K = 0;
                    string WFileName;

                    while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
                    {
                        //    RecordFound = true;
                        int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                        Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                        if (Mf.IsMoveToMatched == true) // the indication that this table is a moving table 
                        {
                            if (
                             Mf.SourceFileId == "ETISALAT_TPF_TXNS"
                            || Mf.SourceFileId == "QAHERA_TPF_TXNS"
                            || Mf.SourceFileId == "IPN_TPF_TXNS"
                            )
                            {
                                //WFileName = "tblMatchingTxnsMasterPoolATMs";
                                WFileName = Mf.SourceFileId + "_MASTER";
                            }
                            else
                            {
                                WFileName = Mf.SourceFileId;
                            }

                            // Check That File Exist in target data base 


                            //Cv.MOVE_TXNS_TO_MATCHED_MOBILE(WFileName, WReconcCycleNo, W_Application);
                            Cv.MOVE_MATCHED_TXNS_TO_PRIMARY_PER_TABLE_NAME(WFileName, WReconcCycleNo, W_Application);
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
                                               + WFileName
                                               );

                                // return;

                            }

                        }

                        I = I + 1;
                    }


                    ////string WSelectionCriteria = " WHERE Operator = '" + WOperator + "' AND Enabled = 1 AND TableStructureId = '" + W_Application + "' ";
                    ////Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);
                    ////
                    //// MOVE TRANSACTIONS FROM MATCHED TO PRIMARY
                    ////

                    ////int I = 0;
                    ////int K = 0;


                    //while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
                    //{
                    //    //    RecordFound = true;
                    //    //int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                    //    Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                    //    if (Mf.IsMoveToMatched == true) // the indication that this table is a moving table 
                    //    {

                    //        WFileName = Mf.SourceFileId;

                    //        // Check That File Exist in target data base 
                    //        string TargetDB = "[RRDM_Reconciliation_MATCHED_TXNS]";
                    //        Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                    //        if (Cv.RecordFound == true)
                    //        {
                    //            // File Exist
                    //        }
                    //        else
                    //        {
                    //            // File do not exist
                    //            //MessageBox.Show("File.." + WFileName + Environment.NewLine
                    //            //    + "DOES NOT EXIST In ITMX_HST Data Base."
                    //            //    + "REPORT TO THE HELP DESK."
                    //            //    );
                    //            I = I + 1;
                    //            continue;
                    //        }




                    TotalProgressText = TotalProgressText + DateTime.Now + " MOVING OF TXNS FROM MATCHED HAS FINISHED" + Environment.NewLine;
                    TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

                    // MessageBox.Show(TotalProgressText);


                    //******************************
                    // UNDO FILES AFTER MOVING THEM FROM MATCHED TO
                    //******************************
                }
                // CALL STORE PROCEDURE TO UNDO THE REST
                // THESE ARE FILES LIKE Reconciliation Sessions etc. 

                int ReturnCode = -20;
                string ProgressText = "";
                string ErrorReference = "";
                int ret = -20;
                // THIS STORE PROCEDURE IS IN ATMS STore Procedures 
                string connectionString = ConfigurationManager.ConnectionStrings
                                              ["ATMSConnectionString"].ConnectionString;

                string SPName = "[ATMS].[dbo].[stp_00_UNDO_RMCYCLE_FILES_MATCHING_MOBILE]";

                using (SqlConnection conn2 = new SqlConnection(connectionString))
                {
                    try
                    {

                        conn2.Open();

                        SqlCommand cmd = new SqlCommand(SPName, conn2);

                        cmd.CommandType = CommandType.StoredProcedure;

                        // the first are input parameters

                        cmd.Parameters.Add(new SqlParameter("@RMCycleNo", WReconcCycleNo));

                        cmd.Parameters.Add(new SqlParameter("@Application", W_Application));

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
                        //ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                        conn2.Close();

                    }
                    catch (Exception ex)
                    {
                        conn2.Close();
                        CatchDetails(ex);
                    }

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



                }
                // Turn the process code to 0 with loading has become to -1 and with mathing to 1  
                RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();
                Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToZeroForCycle(WReconcCycleNo);
                //
                // CALL RRDM CLASS TO DELETE AND ALSO UPDATE RECORDS OF THIS CYCLE 
                //
                RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

                Msf.ReadFilesAND_UNDO_For_Cycle_MOBILE(W_Application, WOperator, WReconcCycleNo);
                ProgressText = ProgressText + Msf.ProgressText_2;
                // DELETE LOADED 
                Flog.Delete_MOBILE(WReconcCycleNo);

                //RRDMJournalReadTxns_Text_Class Ed = new RRDMJournalReadTxns_Text_Class();
                RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

                Ed.LoopFor_MoveFile_From_Archive_ToOrigin_Directory_MOBILE(W_Application, WOperator, WReconcCycleNo, ReversedCut_Off_Date);

                // *****************************

                Mode = 5; // Updating Action 
                ProcessName = "UNDO_LoadingAndMatchingProcess";
                Message = "UNDO Matching and Loading of Files FINISHES. ";

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
                //*********************************
                Panicos = false;
                if (Panicos == true)
                {
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

                }

                //Form502_Load(this, new EventArgs());
                ShowScreen();
            }
           
        }
        // View Loaded Journals
        
        //
        // UNDO MATCHING AND LOADING 
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

                    //// FOR DEALING WITH IST TEMPORARY PROBLEM
                    //// Panicos
                    //if (IsPresent == true & SourceFileId == "QAHERA_TPF_TXNS")
                    //{
                    //    IsGood = "YES";
                    //}

                    if (IsPresent == true & IsGood == "YES")
                    {
                        // Check if still exist in Directory
                        //if (Environment.UserInteractive)
                        //{
                        if (IsGood == "YES")
                        {
                            // THIS IS GOOD TO LOAD
                            // THIS IS TEMPORARY - ALECOS WILL DO IT
                            //
                            // Check if already exists with same hash value with success 
                            //

                            Flog.GetRecordByFileHASH(HASHValue);
                            if (Flog.RecordFound & Environment.UserInteractive)
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

                            if (W_Application == "QAHERA")
                            {
                                Lf_BDC_QAH.InsertRecordsInTableFromTextFile_InBulk_QAHERA(WOperator, SourceFileId, FullFileName
                                                                                      , Ms.InportTableName, Ms.Delimiter, FlogSeqNo, WReconcCycleNo);
                            }
                            if (W_Application == "ETISALAT")
                            {
                                Lf_BDC_ETI.InsertRecordsInTableFromTextFile_InBulk_ETISALAT(WOperator, SourceFileId, FullFileName
                                                                                      , Ms.InportTableName, Ms.Delimiter, FlogSeqNo, WReconcCycleNo);
                            }
                            if (W_Application == "EGATE")
                            {
                                Lf_EGATE.InsertRecordsInTableFromTextFile_InBulk_EGATE(WOperator, SourceFileId, FullFileName
                                                                                      , Ms.InportTableName, Ms.Delimiter, FlogSeqNo, WReconcCycleNo);
                            }

                            // IT IS TEMPORARY 
                            // ALECOS WILL INSERT THIS

                            //if (Environment.UserInteractive)
                            //{

                            Flog.ReadLoadedFilesBySeqNo(FlogSeqNo);

                            Flog.StatusVerbose = "";

                            if (W_Application == "QAHERA")
                            {
                                Flog.LineCount = Lf_BDC_QAH.stpLineCount;

                                Flog.stpReturnCode = Lf_BDC_QAH.stpReturnCode;
                                Flog.stpErrorText = Lf_BDC_QAH.stpErrorText;
                                Flog.stpReferenceCode = Lf_BDC_QAH.stpReferenceCode;

                                if (Lf_BDC_QAH.stpReturnCode == 0) Flog.Status = 1; // Success
                                else Flog.Status = 0; // Failure
                                                      // Update Flog

                            }
                            if (W_Application == "ETISALAT")
                            {
                                Flog.LineCount = Lf_BDC_ETI.stpLineCount;

                                Flog.stpReturnCode = Lf_BDC_ETI.stpReturnCode;
                                Flog.stpErrorText = Lf_BDC_ETI.stpErrorText;
                                Flog.stpReferenceCode = Lf_BDC_ETI.stpReferenceCode;

                                if (Lf_BDC_ETI.stpReturnCode == 0) Flog.Status = 1; // Success
                                else Flog.Status = 0; // Failure
                                                      // Update Flog

                            }

                            if (W_Application == "EGATE")
                            {
                                Flog.LineCount = Lf_EGATE.stpLineCount;

                                Flog.stpReturnCode = Lf_EGATE.stpReturnCode;
                                Flog.stpErrorText = Lf_EGATE.stpErrorText;
                                Flog.stpReferenceCode = Lf_EGATE.stpReferenceCode;

                                if (Lf_EGATE.stpReturnCode == 0) Flog.Status = 1; // Success
                                else Flog.Status = 0; // Failure
                                                      // Update Flog

                            }


                            // Flog.Update(Lf_ABE.WFlogSeqNo);

                            Flog.Update(FlogSeqNo);

                            Flog.Update_MAX_DATE_MOBILE(W_Application, Flog.SourceFileID, FlogSeqNo, WReconcCycleNo);

                            // Update with -1 = ready for Matched if File is good
                            // FOR QAHERA
                            if (Lf_BDC_QAH.stpReturnCode == 0)
                            {
                                Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne(SourceFileId, WReconcCycleNo);

                                if (SourceFileId == "QAHERA_TPF_TXNS")
                                {
                                    // LEAVE IT HERE to cover the Twin that are created from Switch_IST
                                    // Make Twin 
                                    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("QAHERA_TPF_Txns_TWINS", WReconcCycleNo);

                                }

                                if (SourceFileId == "QAHERA_MEEZA_TXNS")
                                {
                                    // LEAVE IT HERE to cover if testing with only QAHERA_MEEZA_TXNS
                                    // File created from QAHERA_MEEZA_TXNS 
                                    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("QAHERA_MEEZA_TXNS_SW", WReconcCycleNo);

                                }
                            }
                            // FOR  ETISALAT
                            if (Lf_BDC_ETI.stpReturnCode == 0)
                            {
                                Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne(SourceFileId, WReconcCycleNo);

                                if (SourceFileId == "ETISALAT_TPF_TXNS")
                                {
                                    // LEAVE IT HERE to cover the Twin that are created from Switch_IST
                                    // Make Twin 
                                    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("ETISALAT_TPF_Txns_TWINS", WReconcCycleNo);

                                }

                                if (SourceFileId == "ETISALAT_MEEZA_TXNS")
                                {
                                    // LEAVE IT HERE to cover if testing with only QAHERA_MEEZA_TXNS
                                    // File created from QAHERA_MEEZA_TXNS 
                                    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("ETISALAT_MEEZA_TXNS_SW", WReconcCycleNo);

                                }
                            }

                            // FOR  EGATE
                            if (Lf_EGATE.stpReturnCode == 0)
                            {
                                Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne(SourceFileId, WReconcCycleNo);

                                //if (SourceFileId == "ETISALAT_TPF_TXNS")
                                //{
                                //    // LEAVE IT HERE to cover the Twin that are created from Switch_IST
                                //    // Make Twin 
                                //    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("ETISALAT_TPF_Txns_TWINS", WReconcCycleNo);

                                //}

                                //if (SourceFileId == "ETISALAT_MEEZA_TXNS")
                                //{
                                //    // LEAVE IT HERE to cover if testing with only QAHERA_MEEZA_TXNS
                                //    // File created from QAHERA_MEEZA_TXNS 
                                //    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("ETISALAT_MEEZA_TXNS_SW", WReconcCycleNo);

                                //}
                            }


                           
                        }



                    }

                    I++; // Read Next entry of the table 
                }

                // UPDATE GL RECORDS AFTER ALL FILES ARE LOADED
                // CALL A STOP PROCEDURE TO DO THE JOB
               
                    int ReturnCode = -20;
                    string ErrorText = "";
                    string ErrorReference = "";
                    int ret = -1;
                    // THIS STORE PROCEDURE IS IN ATMS STore Procedures 
                    string connectionString = ConfigurationManager.ConnectionStrings
                                                  ["ATMSConnectionString"].ConnectionString;
                // ETISALAT_TPF_FAWRY_TXNS
                // WE LOAD TPF. After we look at Fawry to find out which ones are for Settlement 
                int rows = 0;
                string SPName = "";

                if (W_Application == "ETISALAT")
                {
                    rows = 0;
                    SPName = W_Application + ".[dbo].[ETISALAT_TPF_FAWRY_TXNS]";

                    using (SqlConnection conn =
                       new SqlConnection(connectionString))
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd =
                               new SqlCommand(SPName, conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                // Parameters
                                cmd.CommandTimeout = 1800;  // seconds
                                rows = cmd.ExecuteNonQuery();
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


        // GET TOTALS
        private void GetTotals()
        {
            Lf_BDC_QAH.GetTotals(WOperator, WReconcCycleNo);

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
            NForm78d_SlaveCategories = new Form78d_SlaveCategories(WOperator, WSignRecordNo,  WSignedId, textBoxCateg.Text, WReconcCycleNo, WMode);
            NForm78d_SlaveCategories.ShowDialog();

        }

        // Link expand to show Categories 
        private void linkLabelExpand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // SHOW ALL Categories 
            //
            Form78d_SlaveCategories_Mobile NForm78d_SlaveCategories_Mobile;
            int WMode = 4; // show all categories  
            NForm78d_SlaveCategories_Mobile = new Form78d_SlaveCategories_Mobile(WOperator, WSignedId,W_Application ,textBoxCateg.Text, WReconcCycleNo, WMode);
            NForm78d_SlaveCategories_Mobile.ShowDialog();

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

        
        private void button3_Click(object sender, EventArgs e)
        {

        }
        // Show Comment
        private void buttonShowComment_Click(object sender, EventArgs e)
        {

            MessageBox.Show(WFileComment);
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



        // UNDO File This Cycle 
        int ReturnCode;
        string ProgressText;
        // string ErrorReference;
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

            //Form502_Load(this, new EventArgs());
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
       
        private void buttonDiscrCategory_Click_1(object sender, EventArgs e)
        {
            // SHOW THE DISCREPANCIES Per Category
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 1; // Per Category from Master
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchedCat,"" ,WReconcCycleNo, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }
        //
        private void buttonRichDiscrepancies_Click(object sender, EventArgs e)
        {
            //(string InSignedId, int InSignRecordNo, string InOperator, DateTime InDateTimeA,
            //                                     DateTime InDateTimeB, string InAtmNo, 
            //                                     string InCategoryId, int InRMCycleNo,
            //                                     string InStringUniqueId, int InIntUniqueId, int InType,
            //                                     string InFunction, string InIncludeNotMatchedYet, int InReplCycle,
            //                                     DateTime InSettlementDate, string InCategoriesCombined, string InGL_AccountNo)
            Form80b3_MOBILE NForm80b3_MOBILE;
            int WMode = 1; // Per Category from Master
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            DateTime DateTimeA = NullPastDate;
            DateTime DateTimeB = NullPastDate;
            string WAtmNo = "";
            int WType = 17;  // Auditors report 
            NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, W_Application
                , DateTimeA, DateTimeB, WAtmNo
                , WMatchedCat, WReconcCycleNo
                //string InStringUniqueId, int InIntUniqueId, int InType,
                , "", 0, WType
                 // string InFunction, string InIncludeNotMatchedYet, int InReplCycle,
                 , "", "", 0
                 //DateTime InSettlementDate, string InCategoriesCombined, string InGL_AccountNo)
                 , DateTimeA, "", ""
                 );

            NForm80b3_MOBILE.ShowDialog();
        }
        // MATCHED 1000
        private void buttonMatched_Click_1(object sender, EventArgs e)
        {
            // SHOW THE MATCHED Per Category
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 6; // Per Category from Master - MATCHED 1000
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchedCat,"", WReconcCycleNo, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }

        private void buttonDiscr_All_Categ_Click(object sender, EventArgs e)
        {
            // SHOW THE DISCREPANCIES FOR ALL CATEGORIES
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 2; // From Master
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchedCat, "",WReconcCycleNo, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }
        // ACTIONS TAKEN
        private void buttonActionsTaken_Click(object sender, EventArgs e)
        {
            // SHOW ACTIONS TAKEN
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 3; // From Master
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchedCat,"" ,WReconcCycleNo, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }

        private void buttonOutstandingActions_Click(object sender, EventArgs e)
        {
            // OUTSTANDING ACTIONS
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 4; // From Master
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchedCat,"" ,WReconcCycleNo, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }
// Dispute Pre-investigation 
        private void buttonDisputePreInvestigation_Click(object sender, EventArgs e)
        {
            Form3_PreInv_MOBILE NForm3_PreInv_MOBILE;
            //Form3_PreInv_MOBILE(string InSignedId, string InOperator, string InApplication ,int InRMCycle)
            NForm3_PreInv_MOBILE = new Form3_PreInv_MOBILE(WOperator, WSignedId, W_Application,WReconcCycleNo);
            NForm3_PreInv_MOBILE.ShowDialog();
        }

// TEST 4_GRIDS
        private void buttonTEST_4_GRIDS_Click(object sender, EventArgs e)
        {
            Form78d_MOBILE_4_Grids NForm78d_MOBILE_4_Grids;
            int WMode = 8; // SHOW GL ENTRIES
            NForm78d_MOBILE_4_Grids = new Form78d_MOBILE_4_Grids(WOperator, WSignedId, WMatchedCat, WReconcCycleNo, WMode, W_Application);
            NForm78d_MOBILE_4_Grids.ShowDialog();

            //Form78d_MOBILE_4_Grids(string InOpertor, string InSignedId, string InMatchingCateg,
            //                                   int InRMCycle, int InMode, string InApplication); 


        }
// GL and Disputes by Cycle  
        private void buttonGL_AND_Disputes_Click(object sender, EventArgs e)
        {
            Form200JobCycles_GL_Mobile NForm200JobCycles_GL_Mobile;

            string WJobCateg = W_Application;
            NForm200JobCycles_GL_Mobile = new Form200JobCycles_GL_Mobile(WSignedId, WSignRecordNo, "", WOperator, WJobCateg);
            NForm200JobCycles_GL_Mobile.ShowDialog();
        }
    }
}

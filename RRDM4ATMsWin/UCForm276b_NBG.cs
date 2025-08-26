using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
//using System.Runtime.InteropServices;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm276b_NBG : UserControl
    {
        public string guidanceMsg;

        string SelectionCriteria;
        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        bool Presented = false;

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();


        DateTime NullPastDate = new DateTime(1900, 01, 01);

        decimal OpeningBalance;
        decimal Dispensed;
        decimal UnloadedMachine; // Cassettes + Rejected Tray
        decimal UnloadedCounted;
        decimal Cash_Loaded;
        decimal Deposits;
        decimal OverFound;
        decimal ShortFound;

        string RemarksG4S;

        string WAtmNo;
        int WSesNo;

        //TEST
        DateTime WorkingToday = new DateTime(2014, 07, 06);

        int TempMode;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCitId;
        int WLoadingCycle;

        public void UCForm276b_NBG_Par(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InLoadingCycle)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId;
            WLoadingCycle = InLoadingCycle;

            InitializeComponent();

            // Set Working Date 
            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************

            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
                                   //   NormalProcess = false;

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            //WRMCategory = Us.WFieldChar1;
            //WRMCycle = Us.WFieldNumeric1;

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor)
            //      if (WViewFunction || WAuthoriser || WRequestor || WViewHistory)
            {
                //  NormalProcess = false;

                //panelCassettes.Enabled = false;

            }
            else
            {
                //  NormalProcess = true;
            }

            if (WAuthoriser == true)
            {
                //panel2.Location.X.Equals = 9; 
            }

            labelHeader.Text = "SELECTION FOR LOADING EXCEL CYCLE :" + WLoadingCycle;
            TotalsAfterValidation(); 

            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingCycle;

        }

        // SHOW SCREEN 
        // ON LOAD 
        int WTableSize;
        public void SetScreen()
        {
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            ShowGrid1();

        }
        //
        // ROW ENTER 
        //
        bool RequestFromMatched;
        // First DataView1
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            int SeqNo = (int)rowSelected.Cells[0].Value;

            TempMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesBySeqNo(SeqNo, TempMode);

            textBoxRemarksRRDM.Text = G4.RemarksRRDM; 

            WAtmNo = G4.AtmNo;
            WSesNo = G4.ReplCycleNo;

            if (RequestFromMatched)
            {
                TempMode = 2; // From File 2  

                //SelectionCriteria = " WHERE CITId = '" + WCitId + "'" 
                //     + "AND AtmNo ='" + G4.AtmNo + "' ";

                SelectionCriteria = " WHERE AtmNo ='" + G4.AtmNo + "' ";

                G4.ReadCIT_G4S_Repl_EntriesToFillDataTableForGrid2(WOperator, WSignedId, SelectionCriteria,
                                                                                    TempMode, G4.ReplDateG4S.Date);
                WAtmNo = G4.AtmNo;
                WSesNo = G4.ReplCycleNo;

                dataGridView2.Show();
                label12.Show();
                //labelRRDMRemarks.Show();
                //textBoxRemarksRRDM.Show();

                ShowGrid2();

            }
            else
            {
                dataGridView2.Hide();
                label12.Hide();
                //labelRRDMRemarks.Hide();
                //textBoxRemarksRRDM.Hide();
            }

            if (Presented == true)
            {
                label13.Show();
                label14.Show();
                textBoxOver.Show();
                textBoxPresented.Show();
                textBoxOver.Text = G4.OverFound.ToString();
                textBoxPresented.Text = G4.PresentedErrors.ToString();

            }
            else
            {
                label13.Hide();
                label14.Hide();
                textBoxOver.Hide();
                textBoxPresented.Hide();
            }
            //
            // "01" means In G4S but not in Bank 
            //
            if (G4.Mask == "01" || G4.Mask == "")
            {
                buttonShowSM.Hide();
            }
            else
            {
                buttonShowSM.Show();
            }
        }
// Second Dataview
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            int SeqNo = (int)rowSelected.Cells[0].Value;

            TempMode = 1; // Read Remarks 
            G4.ReadCIT_G4S_Repl_EntriesBySeqNo(SeqNo, TempMode);

            textBoxRemarksRRDM.Text = G4.RemarksRRDM;

        }
        // Show SM
        private void buttonShowSM_Click(object sender, EventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            string SM_SelectionCriteria1 = " WHERE atmno ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + WSesNo
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

            SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, WSesNo, 2);

            if (SM.RecordFound == true)
            {
                Form67_BDC NForm67_BDC;

                int Mode = 7; // Given Fuid and Ruid 
                string WTraceRRNumber = "";
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, SM.Fuid, WTraceRRNumber, WAtmNo
                    , SM.sessionstart_ruid, SM.sessionend_ruid, NullPastDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not found records");
            }

            //int WTraceNo;

            //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            //Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            //WTraceNo = Ta.FirstTraceNo;
            //DateTime WDateA = Ta.SesDtTimeEnd.Date;
            //// Check Trace No
            //Int32 LastDigit = WTraceNo % 10;

            //if (LastDigit == 0)
            //{
            //    // OK
            //    // It is just a transaction
            //}
            //else
            //{
            //    // It is the supervisor mode
            //    WTraceNo = (WTraceNo - LastDigit) + 1;
            //}

            //Form67 NForm67;
            //int Mode = 5; // Specific

            //NForm67 = new Form67(WSignedId, 0, WOperator, 0, WAtmNo, WTraceNo, WTraceNo, WDateA, NullPastDate, Mode);
            //NForm67.ShowDialog();
        }

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGrid1()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();

            dataGridView2.DataSource = null;
            dataGridView2.Refresh();

            dataGridView1.DataSource = G4.DataTableG4SEntries.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No entries Available.");

                dataGridView2.Hide();
                label12.Hide();
                labelRRDMRemarks.Hide();
                textBoxRemarksRRDM.Hide();

                return;
            }
            else
            {
                textBoxTotalEnries.Text = dataGridView1.Rows.Count.ToString();
            }

            if (checkBoxShowAll.Checked == true)
            {

            }
            else
            {
                Grid_1_Fields();
            }

        }
        // Grid 1  Fields 
        private void Grid_1_Fields()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 110; // CITId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 100; //  OriginFileName
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 60; // AtmNo
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 80; // AtmName
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].Visible = false;

            dataGridView1.Columns[5].Width = 50; // LoadingCycleNo
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = false;

            dataGridView1.Columns[6].Width = 50; //ReplDateG4S
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = false;

            dataGridView1.Columns[7].Width = 50; // OrderId 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].Visible = false;

            dataGridView1.Columns[8].Width = 50; // CreatedDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].Visible = false;

            dataGridView1.Columns[9].Width = 50; // IsDeposit
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[9].HeaderText = "Is Deposit";

            dataGridView1.Columns[10].Width = 70; // OpeningBalance
            dataGridView1.Columns[10].DefaultCellStyle = style;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[10].HeaderText = "Opening Balance";

            dataGridView1.Columns[11].Width = 70; //Dispensed
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[12].Width = 70; // UnloadedMachine
            dataGridView1.Columns[12].DefaultCellStyle = style;
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[12].HeaderText = "Unloaded Machine";

            dataGridView1.Columns[13].Width = 70; // UnloadedCounted
            dataGridView1.Columns[13].DefaultCellStyle = style;
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[13].HeaderText = "Unloaded Counted";

            dataGridView1.Columns[14].Width = 80; // Cash_Loaded
            dataGridView1.Columns[14].DefaultCellStyle = style;
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[14].HeaderText = "Cash Loaded";

            dataGridView1.Columns[15].Width = 70; // Deposits
            dataGridView1.Columns[15].DefaultCellStyle = style;
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[16].Width = 70; //OverFound
            dataGridView1.Columns[16].DefaultCellStyle = style;
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[16].HeaderText = "Over Found";

            dataGridView1.Columns[17].Width = 70; // ShortFound
            dataGridView1.Columns[17].DefaultCellStyle = style;
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[17].HeaderText = "Short Found";

            // These (the below) are not included 

            //RemarksG4S = (string)rdr["RemarksG4S"];
            //PresentedErrors = (decimal)rdr["PresentedErrors"];
            //AmtCheckedForMatching = (decimal)rdr["AmtCheckedForMatching"];
            //OtherJournalErrors = (decimal)rdr["OtherJournalErrors"];

            //OrderToBeLoaded = (decimal)rdr["OrderToBeLoaded"];

            //RemarksRRDM = (string)rdr["RemarksRRDM"];

            //ProcessMode = (int)rdr["ProcessMode"];

            //ProcessedAtReplCycleNo = (int)rdr["ProcessedAtReplCycleNo"];

            //Mask = (string)rdr["Mask"];

            //Cut_Off_date = (DateTime)rdr["Cut_Off_date"];

            //Gl_Balance_At_CutOff = (decimal)rdr["Gl_Balance_At_CutOff"];

            //Operator = (string)rdr["Operator"];
        }

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGrid2()
        {

            dataGridView2.DataSource = null;
            dataGridView2.Refresh();

            dataGridView2.DataSource = G4.DataTableG4SEntries.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No fould Record = System Error");
                return;
            }
            else
            {
                // Show Grid
            }

            if (checkBoxShowAll.Checked == true)
            {
                // Leave system to use by default all Fields 
            }
            else
            {
                Grid_2_Fields();
            }

        }


        // Grid 2  Fields 
        private void Grid_2_Fields()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView2.Columns[0].Width = 40; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].Visible = false;

            dataGridView2.Columns[1].Width = 110; // CITId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].Visible = false;

            dataGridView2.Columns[2].Width = 100; //  OriginFileName
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[2].Visible = false;

            dataGridView2.Columns[3].Width = 60; // AtmNo
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[3].Visible = true;

            dataGridView2.Columns[4].Width = 80; // AtmName
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[4].Visible = false;

            dataGridView2.Columns[5].Width = 50; // LoadingCycleNo
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[5].Visible = false;

            dataGridView2.Columns[6].Width = 50; //ReplDateG4S
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[6].Visible = false;

            dataGridView2.Columns[7].Width = 50; // OrderId 
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[7].Visible = false;

            dataGridView2.Columns[8].Width = 50; // CreatedDate
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[8].Visible = false;

            dataGridView2.Columns[9].Width = 50; // IsDeposit
            dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[9].HeaderText = "Is Deposit";

            dataGridView2.Columns[10].Width = 70; // OpeningBalance
            dataGridView2.Columns[10].DefaultCellStyle = style;
            dataGridView2.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[10].HeaderText = "Opening Balance";

            dataGridView2.Columns[11].Width = 70; //Dispensed
            dataGridView2.Columns[11].DefaultCellStyle = style;
            dataGridView2.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[12].Width = 70; // UnloadedMachine
            dataGridView2.Columns[12].DefaultCellStyle = style;
            dataGridView2.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[12].HeaderText = "Unloaded Machine";

            dataGridView2.Columns[13].Width = 70; // UnloadedCounted
            dataGridView2.Columns[13].DefaultCellStyle = style;
            dataGridView2.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[13].HeaderText = "Unloaded Counted";

            dataGridView2.Columns[14].Width = 80; // Cash_Loaded
            dataGridView2.Columns[14].DefaultCellStyle = style;
            dataGridView2.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[14].HeaderText = "Cash Loaded";

            dataGridView2.Columns[15].Width = 70; // Deposits
            dataGridView2.Columns[15].DefaultCellStyle = style;
            dataGridView2.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[16].Width = 70; //OverFound
            dataGridView2.Columns[16].DefaultCellStyle = style;
            dataGridView2.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[16].HeaderText = "Over Found";

            dataGridView2.Columns[17].Width = 70; // ShortFound
            dataGridView2.Columns[17].DefaultCellStyle = style;
            dataGridView2.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[17].HeaderText = "Short Found";

            // These (the below) are not included 

            //RemarksG4S = (string)rdr["RemarksG4S"];
            //PresentedErrors = (decimal)rdr["PresentedErrors"];
            //AmtCheckedForMatching = (decimal)rdr["AmtCheckedForMatching"];
            //OtherJournalErrors = (decimal)rdr["OtherJournalErrors"];

            //OrderToBeLoaded = (decimal)rdr["OrderToBeLoaded"];

            //RemarksRRDM = (string)rdr["RemarksRRDM"];

            //ProcessMode = (int)rdr["ProcessMode"];

            //ProcessedAtReplCycleNo = (int)rdr["ProcessedAtReplCycleNo"];

            //Mask = (string)rdr["Mask"];

            //Cut_Off_date = (DateTime)rdr["Cut_Off_date"];

            //Gl_Balance_At_CutOff = (decimal)rdr["Gl_Balance_At_CutOff"];

            //Operator = (string)rdr["Operator"];
        }
// Print 
        private void buttonP11_Click(object sender, EventArgs e)
        {
            string P1 = labelHeader.Text;

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_G4S ReportATMSReplCycles = new Form56R69ATMS_G4S(P1, P2, P3, P4, P5);
            ReportATMSReplCycles.Show();
        }
        // Validate 
        private void buttonValidate_Click(object sender, EventArgs e)
        {
            Validate_LoadedExcel();

            //labelHeader.Text = "SELECTION FOR LOADING EXCEL CYCLE :" + WLoadingCycle;
            
            //TempMode = 1;
            //SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingCycle;
        }
        // Validate 
        // Validate All read 
        int WSeqNo;
        string WMask;
        int Total11;
        int TotalAA;
        int Total10;
        int Total01;

        int TotalPresenterEqual;
        int TotalPresenterNotEqual;

        int WLoadingExcelCycleNo;

        int TotalShortFound = 0;
        decimal TotalPresenterDiffAmt = 0;
        decimal TotalShortAmt = 0;
        private void Validate_LoadedExcel()
        {

            Total11 = 0;
            TotalAA = 0;
            Total10 = 0;
            Total01 = 0;

            TotalPresenterEqual = 0;
            TotalPresenterNotEqual = 0;

            DateTime WReplDate; 

            TotalShortFound = 0;
            TotalPresenterDiffAmt = 0;
            TotalShortAmt = 0;
            // READ ALL ORIGINATED FROM EXCEL
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode <= 0 ";
            TempMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            int I = 0;

            while (I <= (G4.DataTableG4SEntries.Rows.Count - 1))
            {
                WMask = "";
                //    RecordFound = true;
                WSeqNo = (int)G4.DataTableG4SEntries.Rows[I]["SeqNo"];

                TempMode = 1;
                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, TempMode);


                WAtmNo = G4.AtmNo;
                WLoadingExcelCycleNo = G4.LoadingExcelCycleNo; 
                WReplDate = G4.ReplDateG4S; 
                OpeningBalance = G4.OpeningBalance;
                Dispensed = G4.Dispensed;
                UnloadedMachine = G4.UnloadedMachine; // Cassettes + Rejected Tray
                UnloadedCounted = G4.UnloadedCounted;
                Cash_Loaded = G4.Cash_Loaded;
                Deposits = G4.Deposits;
                OverFound = G4.OverFound;
                ShortFound = G4.ShortFound;

                RemarksG4S = G4.RemarksG4S;

                // Now read the Bank's coresponding record 
                //  
                // Check if matched with Bank Record
                string WSelectionCriteria = " WHERE "
                    + " AtmNo=" + WAtmNo
                    + " AND ProcessMode<=0"
                    + " AND OpeningBalance=" + OpeningBalance
                    + " AND Dispensed=" + Dispensed
                    + " AND UnloadedMachine=" + UnloadedCounted
                    + " AND Deposits=" + Deposits
                    + " AND Cash_Loaded=" + Cash_Loaded
                    ;

                TempMode = 2; // Banks file 
                G4.ReadCIT_G4S_Repl_EntriesByATMandDate(WSelectionCriteria, TempMode);

                if (G4.RecordFound == true)
                {
                    Total11 = Total11 + 1;
                    WMask = "11";

                    G4.RemarksRRDM = "Fully Matched Records";

                    if (OverFound > 0 || G4.PresentedErrors > 0)
                    {

                        // Over Found 
                        if (OverFound == G4.PresentedErrors)
                        {
                            TotalPresenterEqual = TotalPresenterEqual + 1;
                            G4.RemarksRRDM = "Matched and equal with Presenter";
                        }
                        else
                        {
                            TotalPresenterNotEqual = TotalPresenterNotEqual + 1;
                            TotalPresenterDiffAmt = TotalPresenterDiffAmt + (G4.PresentedErrors - OverFound);
                            G4.RemarksRRDM = "Matched but differ in Presenter";
                        }
                    }


                    if (ShortFound > 0)
                    {
                        TotalShortFound = TotalShortFound + 1;
                        TotalShortAmt = TotalShortAmt + ShortFound;
                    }
                }

                //
                if (WMask != "11")
                {
                    // NOT found from the first attempt try with the second
                    TempMode = 2;
                    G4.ReadCIT_G4S_Repl_EntriesByATMandDate(WAtmNo, WReplDate, TempMode);

                    if (G4.RecordFound == true)
                    {
                        // Record differ but date is the same 
                        TotalAA = TotalAA + 1;
                        WMask = "AA";
                        int CounterDiff = 0;
                        G4.RemarksRRDM = "";
                        if (OpeningBalance != G4.OpeningBalance)
                        {
                            G4.RemarksRRDM = G4.RemarksRRDM + "_G4s Values differ from Bank's Journal File..-.." + "Openning Balance";
                            CounterDiff = CounterDiff + 1;
                        }
                        if (Dispensed != G4.Dispensed)
                        {
                            G4.RemarksRRDM = G4.RemarksRRDM + "_G4s Values differ from Bank's Journal File..-.." + "Dispensed";
                            CounterDiff = CounterDiff + 1;
                        }
                        //if (UnloadedMachine != G4.UnloadedMachine)
                        //{
                        //    G4.RemarksRRDM = "G4s Values differ from Bank's Journal File..-.." + "UnloadedMachine";
                        //    CounterDiff = CounterDiff + 1;
                        //}
                        if (UnloadedCounted != G4.UnloadedMachine)
                        {
                            G4.RemarksRRDM = G4.RemarksRRDM + "G4s Values differ from Bank's Journal File..-.." + "UnloadedCounted";
                            CounterDiff = CounterDiff + 1;
                        }
                        if (Cash_Loaded != G4.Cash_Loaded)
                        {
                            G4.RemarksRRDM = G4.RemarksRRDM + "G4s Values differ from Bank's Journal File..-.." + "Cash_Loaded";
                            CounterDiff = CounterDiff + 1;
                        }
                        if (Deposits != G4.Deposits)
                        {
                            G4.RemarksRRDM = G4.RemarksRRDM + "G4s Values differ from Bank's Journal File..-.." + "Deposits";
                            CounterDiff = CounterDiff + 1;
                        }
                        if (CounterDiff > 1)
                        {
                            G4.RemarksRRDM = G4.RemarksRRDM + "G4s Values differ from Bank's Journal File..-.." + "In More than One Value";
                        }
                    }
                    else
                    {

                        WMask = "10";
                        G4.RemarksRRDM = "Not Found in Bank File.";
                    }
                }


                // Update both Files 
                //G4.LoadingExcelCycleNo = WLoadingCycle;
                //if (WMask == "11") G4.ProcessMode = 0; // Validated and Ready for Replenishment 
                //else
                //{
                //    if (WMask == "AA")
                //    {
                //        if (Cash_Loaded != G4.Cash_Loaded)
                //        {
                //            G4.ProcessMode = -2;
                //        }
                //        else
                //        {
                //            if (Dispensed != G4.Dispensed
                //                  || UnloadedMachine != G4.UnloadedMachine
                //                  || UnloadedCounted != G4.UnloadedMachine
                //                  || Deposits != G4.Deposits
                //                )
                //            {
                //                G4.ProcessMode = 0;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        G4.ProcessMode = -2;
                //    }
                //}
                // UPDATE G4S Records
                G4.LoadingExcelCycleNo = WLoadingExcelCycleNo; // Re-define 
                G4.ReplCycleNo = G4.ReplCycleNo;
                G4.Mask = WMask;

                G4.UnloadedCounted = UnloadedCounted; 

                G4.RemarksRRDM = G4.RemarksRRDM;

                G4.UpdateCIT_G4S_Repl_EntriesRecord(WSeqNo, 1); // WSEQ taken from the record read of G4 table 

                // Do update for Bank's File
                if (WMask != "10")
                {
                    G4.LoadingExcelCycleNo = WLoadingExcelCycleNo; // Re-define 
                    G4.Mask = WMask;
                    G4.UnloadedCounted = 0; 
                    G4.RemarksRRDM = G4.RemarksRRDM;
                    G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 2); // Mode is 2 // Taken from the last 
                }

                I++; // Read Next entry of the table 

            }

            // FIND ALL IN G4S But not in BANK (This Cycle and previous ones)

            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode <0 AND MASK = '10' ";
            TempMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            Total10 = G4.DataTableG4SEntries.Rows.Count;

            // FIND ALL IN BANK's But not in G4S

            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode <=0 AND Mask NOT IN ('11','AA') ";
            TempMode = 2;
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            I = 0;

            while (I <= (G4.DataTableG4SEntries.Rows.Count - 1))
            {
                WMask = "";
                //    RecordFound = true;
                WSeqNo = (int)G4.DataTableG4SEntries.Rows[I]["SeqNo"];

                TempMode = 2;
                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, TempMode);

                    Total01 = Total01 + 1;
                    WMask = "01"; // First digit denotes that there is not in G4S
                                  // Second says it is in Bank 
                    //
                    // Update  Bank File
                    //
                    G4.ProcessMode_Load = -2; // 
                    //G4.ProcessedAtReplCycleNo = G4.ProcessedAtReplCycleNo;
                    G4.Mask = WMask;
                    G4.RemarksRRDM = "Record Not Found In G4S File ";

                    G4.UpdateCIT_G4S_Repl_EntriesRecord(WSeqNo, 2);
               

                I++; // Read Next entry in the table 

            }

            TotalsAfterValidation(); 

          //  MessageBox.Show("Records Validated. See and Update Accordingly. ");

            Cec.ReadExcelLoadCyclesBySeqNo(WLoadingCycle);

            Cec.ProcessStage = 2;

            Cec.UpdateLoadExcelCycle(WLoadingCycle);

            //    buttonUpdate.Show();

            // Load Grid 
            //TempMode = 1;
            //SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode <> 1";
            labelHeader.Text = "SELECTION FOR LOADING EXCEL CYCLE :" + WLoadingCycle;
            RequestFromMatched = false;
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingCycle;
            SetScreen();
        }

        private void TotalsAfterValidation()
        {
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode <> 1 " ;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTableAND_Totals(WOperator, WSignedId,
                                                       SelectionCriteria, TempMode);
            if ((G4.Total11 + G4.TotalAA + G4.Total10)>0)
            {
                label6.Show();
                panel1.Show();
                label3.Show();

                panel7.Show();

               // buttonValidate.Hide();

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.WFieldNumeric12 = 46; // Data has been validated 

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
            }
            else
            {
                label6.Hide();
                panel1.Hide();
                label3.Hide();

                panel7.Hide();
            }
            //textBoxTotal11.Text = G4.TotalNotProcessed.ToString();
            textBoxTotal11.Text = G4.Total11.ToString();
            textBoxTotalAA.Text = G4.TotalAA.ToString();
            textBoxTotal10.Text = G4.Total10.ToString();
          
            textBoxShort.Text = G4.TotalShort.ToString();
            textBoxPresenterNotEqual.Text = G4.TotalPresenter.ToString();

            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode <> 1 and Mask NOT IN ('11','AA') ";
            TempMode = 2;
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            textBoxTotal01.Text = G4.DataTableG4SEntries.Rows.Count.ToString();

        }

// 11
        private void button11_Click(object sender, EventArgs e)
        {
            labelHeader.Text = "SELECTION FOR MATCHED";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = '11' AND LoadingExcelCycleNo =" + WLoadingCycle;

            RequestFromMatched = true;
            Presented = false;

            SetScreen();
        }
// AA
        private void buttonAA_Click(object sender, EventArgs e)
        {
            labelHeader.Text = "SELECTION FOR MATCHED BUT DIFFERENT VALUES";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = 'AA'  AND LoadingExcelCycleNo =" + WLoadingCycle;

            RequestFromMatched = true;
            Presented = false;

            SetScreen();
        }
//  10
        private void button10_Click(object sender, EventArgs e)
        {
            labelHeader.Text = "SELECTION FOR MISSING IN BANK ";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = '10'  AND LoadingExcelCycleNo <=" + WLoadingCycle;

            RequestFromMatched = false;

            Presented = false;

            SetScreen();
        }
// 01
        private void button01_Click(object sender, EventArgs e)
        {
            labelHeader.Text = "PRESENT IN BANK BUT MISSING IN G4S ";
            TempMode = 2; // From File 2  
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = '01' AND ProcessMode <> 1 ";

            RequestFromMatched = false;
            Presented = false;

            SetScreen();
        }
// Short
        private void buttonShort_Click(object sender, EventArgs e)
        {
            labelHeader.Text = "CASES OF SHORT NOTES FOUND ";
            TempMode = 1; // From File 2  
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ShortFound>0 AND LoadingExcelCycleNo =" + WLoadingCycle;

            RequestFromMatched = false;
            Presented = false;

            SetScreen();
        }
// Presenter not equal 
        private void buttonPresenterNotEqual_Click(object sender, EventArgs e)
        {
            labelHeader.Text = "Presented Errors Amount Not Equal To Suplus ";
            TempMode = 1; // From File 1 as it is the same as in file 2  

            SelectionCriteria = " WHERE (OverFound - PresentedErrors) <> 0 AND Mask = '11' AND LoadingExcelCycleNo =" + WLoadingCycle;

            Presented = true;

            RequestFromMatched = true;

            SetScreen();
        }

    }
}

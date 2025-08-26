using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form18_CIT_ValidateUpdateExcel_AUDI : Form
    {

        //Form31 NForm31;

        //DateTime FromDate;
        //DateTime ToDate;

        string SelectionCriteria;

        bool Presented = false;

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

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

        string WOperator;
        string WSignedId;

        string WCitId;

        int WExcelLoadCycle;

        DateTime WDate;

        int WMode; 

        public Form18_CIT_ValidateUpdateExcel_AUDI(string InOperator, string InSignedId, string InCitId, int InExcelLoadCycle, 
                   DateTime InWorkingDate, int InMode)
        {
            WSignedId = InSignedId;
            //WSignRecordNo = SignRecordNo;
            //WSecLevel = InSecLevel;
            WOperator = InOperator;

            WCitId = InCitId;

            WExcelLoadCycle = InExcelLoadCycle;

            WDate = InWorkingDate;

            WMode = InMode;
                         // 1 : Updating
                         // 2 : View 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = WSignedId;
            
            if (WMode == 2)
            {
                buttonValidate.Hide();
                buttonReverse.Hide();
                buttonUpdate.Hide();
                labelFormTitle.Text = "Results Excel Loading. Working Date.. " + WDate.ToShortDateString();

                panel2.Show();
                
                labelHeaderRight.Text = "MAKE SELECTION";
                TempMode = 1; 
                SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WExcelLoadCycle;
           
                G4.ReadCIT_G4S_Repl_EntriesToFillDataTableAND_Totals(WOperator, WSignedId,
                                                           SelectionCriteria, TempMode);

                //textBoxTotal11.Text = G4.TotalNotProcessed.ToString();
                textBoxTotal11.Text = G4.Total11.ToString();
                textBoxTotalAA.Text = G4.TotalAA.ToString();
                textBoxTotal01.Text = G4.Total01.ToString();

                textBoxShort.Text = G4.TotalShort.ToString();
                textBoxPresenterNotEqual.Text = G4.TotalPresenter.ToString();


            }

            labelHeader.Text = "SELECTION FOR :" + WDate.ToShortDateString();

            textBoxMsgBoard.Text = "View CIT Replenishment Work";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WExcelLoadCycle; 

        }

        // Load 
        private void Form18_Load(object sender, EventArgs e)
        {

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            ShowGrid1();

        }
        //
        // ROW ENTER ON USER 
        //
        bool RequestFromMatched;
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            int SeqNo = (int)rowSelected.Cells[0].Value;

            TempMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesBySeqNo(SeqNo, TempMode);

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
                labelRRDMRemarks.Show();
                textBoxRemarksRRDM.Show(); 

                ShowGrid2();

            }
            else
            {
                dataGridView2.Hide();
                label12.Hide();
                labelRRDMRemarks.Hide();
                textBoxRemarksRRDM.Hide();
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

        // ROW ENTER FOR SECOND GRID
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            int SeqNo = (int)rowSelected.Cells[0].Value;

            TempMode = 1; // Read Remarks 
            G4.ReadCIT_G4S_Repl_EntriesBySeqNo(SeqNo, TempMode);

            textBoxRemarksRRDM.Text = G4.RemarksRRDM; 

        }


        // ACTION MESSAGES 
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("These are the actions messages send to CIT Company");
            return;
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

        // Finish 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // Validate All read 
        int WSeqNo;
        string WMask;
        int Total11;
        int TotalAA;
        int Total10;
        int Total01;

        int TotalPresenterEqual;
        int TotalPresenterNotEqual;

        int TotalShortFound = 0;
        decimal TotalPresenterDiffAmt = 0;
        decimal TotalShortAmt = 0;

        private void buttonValidate_Click(object sender, EventArgs e)
        {

            Total11 = 0;
            TotalAA = 0;
            Total10 = 0;
            Total01 = 0;

            TotalPresenterEqual = 0;
            TotalPresenterNotEqual = 0;

            TotalShortFound = 0;
            TotalPresenterDiffAmt = 0;
            TotalShortAmt = 0;
            // READ ALL ORIGINATED FROM EXCEL
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode = -2 ";
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
                //
                TempMode = 2;
                G4.ReadCIT_G4S_Repl_EntriesByATMandDate(G4.AtmNo, G4.ReplDateG4S.Date, TempMode);

                if (G4.RecordFound == true)
                {
                    // Check other details 
                    if (OpeningBalance == G4.OpeningBalance
                         & Dispensed == G4.Dispensed
                          & UnloadedMachine == G4.UnloadedMachine
                           & Cash_Loaded == G4.Cash_Loaded
                            & Deposits == G4.Deposits
                         )
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
                    else
                    {
                        TotalAA = TotalAA + 1;
                        WMask = "AA";
                        int CounterDiff = 0; 
                        G4.RemarksRRDM = "G4s Values differ from Bank's Journal File -.";
                        if (OpeningBalance != G4.OpeningBalance)
                        {
                            G4.RemarksRRDM = "G4s Values differ from Bank's Journal File..-.." + "Openning Balance";
                            CounterDiff = CounterDiff + 1; 
                        }
                        if (Dispensed != G4.Dispensed)
                        {
                            G4.RemarksRRDM = "G4s Values differ from Bank's Journal File..-.." + "Dispensed";
                            CounterDiff = CounterDiff + 1;
                        }
                        if (UnloadedMachine != G4.UnloadedMachine)
                        {
                            G4.RemarksRRDM = "G4s Values differ from Bank's Journal File..-.." + "UnloadedMachine";
                            CounterDiff = CounterDiff + 1;
                        }
                        if (Cash_Loaded != G4.Cash_Loaded)
                        {
                            G4.RemarksRRDM = "G4s Values differ from Bank's Journal File..-.." + "Cash_Loaded";
                            CounterDiff = CounterDiff + 1;
                        }
                        if (Deposits != G4.Deposits)
                        {
                            G4.RemarksRRDM = "G4s Values differ from Bank's Journal File..-.." + "Deposits";
                            CounterDiff = CounterDiff + 1;
                        }
                        if (CounterDiff > 1)
                        {
                            G4.RemarksRRDM = "G4s Values differ from Bank's Journal File..-.." + "In More than One Value";
                        }
                    }
                }
                else
                {

                    WMask = "10";
                    G4.RemarksRRDM = "Not Found in Bank File.";
                }

                // Update both Files 
                G4.LoadingExcelCycleNo = WExcelLoadCycle;
                if (WMask == "11") G4.ProcessMode_Load = 0; // Validated and Ready for Replenishment 
                else G4.ProcessMode_Load = -2;
                G4.ReplCycleNo = G4.ReplCycleNo;
                G4.Mask = WMask;
                G4.PresentedErrors = G4.PresentedErrors; // Update Both Fileswi this 
                G4.UnloadedCounted = UnloadedCounted; // Update Both Fileswi this 
                G4.OverFound = OverFound;
                G4.ShortFound = ShortFound;

                G4.RemarksRRDM = G4.RemarksRRDM;

                G4.RemarksG4S = "No Remark";

                G4.UpdateCIT_G4S_Repl_EntriesRecord(WSeqNo, 1);

                G4.RemarksG4S = "No Remark";

                // Do update for Bank's File
                if (WMask != "10")
                {
                    G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 2); // Mode is 2 
                }

                I++; // Read Next entry of the table 

            }

            // FIND ALL IN G4S But not in BANK (This Cycle and previous ones)

            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode = -2 AND MASK = '10' ";
            TempMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            Total10 = G4.DataTableG4SEntries.Rows.Count;

            // FIND ALL IN BANK's But not in G4S


            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode = -2 AND MASK <> '11' ";
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

                //  
                // Check if muatched with G4S 
                //
                TempMode = 1;
                G4.ReadCIT_G4S_Repl_EntriesByATMandDate(G4.AtmNo, G4.ReplDateG4S.Date, TempMode);

                if (G4.RecordFound == true)
                {

                }
                else
                {
                    Total01 = Total01 + 1;
                    WMask = "10";
                    //
                    // Update  Bank File
                    //
                    G4.ProcessMode_Load = -2; // 
                    G4.ReplCycleNo = G4.ReplCycleNo;
                    G4.Mask = WMask;
                    G4.RemarksRRDM = "Record Not Found In G4S File ";

                    G4.RemarksG4S = "No Remark";


                    G4.UpdateCIT_G4S_Repl_EntriesRecord(WSeqNo, 2);
                }

                I++; // Read Next entry in the table 

            }

            textBoxTotal11.Text = Total11.ToString();
            textBoxTotalAA.Text = TotalAA.ToString();
            textBoxTotal10.Text = Total10.ToString();
            textBoxTotal01.Text = Total01.ToString();

            textBoxShort.Text = TotalShortFound.ToString();

            textBoxPresenterNotEqual.Text = TotalPresenterNotEqual.ToString();

            MessageBox.Show("Records Validated. See and Update Accordingly. ");

            Cec.ReadExcelLoadCyclesBySeqNo(WExcelLoadCycle);

            Cec.ProcessStage = 2;

            Cec.UpdateLoadExcelCycle(WExcelLoadCycle);

            panel2.Show();

            buttonUpdate.Show();

            // Load Grid 
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode <> 1";
            Form18_Load(this, new EventArgs());
        }


        // Print What you VIEW 
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

        private void button11_Click_1(object sender, EventArgs e)
        {
            labelHeader.Text = "SELECTION FOR MATCHED";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = '11' AND LoadingExcelCycleNo =" + WExcelLoadCycle;

            RequestFromMatched = true;
            Presented = false;

            Form18_Load(this, new EventArgs());

        }

        private void buttonAA_Click_1(object sender, EventArgs e)
        {
            labelHeader.Text = "SELECTION FOR MATCHED BUT DIFFERENT VALUES";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = 'AA'  AND LoadingExcelCycleNo =" + WExcelLoadCycle;

            RequestFromMatched = true;
            Presented = false;

            Form18_Load(this, new EventArgs());

        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            labelHeader.Text = "SELECTION FOR MISSING IN BANK ";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = '10'  AND LoadingExcelCycleNo <=" + WExcelLoadCycle;

            RequestFromMatched = false;

            Presented = false;

            Form18_Load(this, new EventArgs());
        }

        private void button01_Click_1(object sender, EventArgs e)
        {
            labelHeader.Text = "PRESENT IN BANK BUT MISSING IN G4S ";
            TempMode = 2; // From File 2  
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = '10' AND ProcessMode <> 1 ";

            RequestFromMatched = false;
            Presented = false;

            Form18_Load(this, new EventArgs());

        }

        // Short 
        private void button1_Click(object sender, EventArgs e)
        {

            labelHeader.Text = "CASES OF SHORT NOTES FOUND ";
            TempMode = 1; // From File 2  
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ShortFound>0 AND LoadingExcelCycleNo =" + WExcelLoadCycle;

            RequestFromMatched = false;
            Presented = false;

            Form18_Load(this, new EventArgs());
        }

        // Presenter Not Equal

        private void buttonPresenterNotEqual_Click(object sender, EventArgs e)
        {
            labelHeader.Text = "Presented Errors Amount Not Equal To Suplus ";
            TempMode = 1; // From File 1 as it is the same as in file 2  

            SelectionCriteria = " WHERE (OverFound - PresentedErrors) <> 0 AND Mask = '11' AND LoadingExcelCycleNo =" + WExcelLoadCycle;

            Presented = true;

            RequestFromMatched = true;

            Form18_Load(this, new EventArgs());
        }

        // REVERSALS
        private void buttonReverse_Click(object sender, EventArgs e)
        {
            // Dissable Reversal Entry 
            Cec.ReadExcelLoadCyclesBySeqNo(WExcelLoadCycle);

           // Cec.IsReversed = true;

            Cec.UpdateLoadExcelCycle(WExcelLoadCycle);

            TempMode = 1;
            G4.DeleteAndUpdateToReverseEntriesbyLoadingExcelCycleNo(WExcelLoadCycle, WCitId);
            MessageBox.Show("Entries Has Been Reversed" + Environment.NewLine
                            + "Start Reloading Excel "
                            );
            this.Dispose();

        }
        // UPDATE AND FINISH BUTTON

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            // Update RRDM Replenishmet record
            //
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMReconcCategories Rc = new RRDMReconcCategories();
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            RRDM_GL_Balances_For_Categories_And_Atms Gadj = new RRDM_GL_Balances_For_Categories_And_Atms();

            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            RRDMAccountsClass Acc = new RRDMAccountsClass();

            int TotalInDiff = 0; 

            string WJobCategory = "ATMs";
            int WReconcCycleNo;
            string Message;

            bool ShowMessage = true;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            string WReconCategGroup;
            string WAtmNo = "";
            int WSesNo = 0;
            // Read all Outstanding Matched Entries from G4S file
            // Make a loop and update 

            // Make Selection Of Validated Entries 
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode = 0 AND Mask = '11' ";
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            if (G4.DataTableG4SEntries.Rows.Count == 0)
            {
                MessageBox.Show("There are no records to update");
                return;
            }

            int I = 0;

            while (I <= (G4.DataTableG4SEntries.Rows.Count - 1))
            {
                //    RecordFound = true;
                WSeqNo = (int)G4.DataTableG4SEntries.Rows[I]["SeqNo"];

                // GET ALL fields
                TempMode = 1;
                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, TempMode);

                WAtmNo = G4.AtmNo;
                WSesNo = G4.ReplCycleNo;

                Ac.ReadAtm(WAtmNo); // Read Information for ATM 

                if (G4.Gl_Balance_At_CutOff > 0)
                {
                    // Insert record in GL File 
                    Gadj.OriginFileName = G4.OriginFileName;
                    Gadj.OriginalRecordId = G4.SeqNo;
                    Gadj.Cut_Off_Date = G4.Cut_Off_date;
                    Gadj.MatchingCateg = "";
                    Gadj.AtmNo = G4.AtmNo;
                    Gadj.Origin = "BANK";
                    Gadj.TransTypeAtOrigin = "GL Entry";

                    string ATMSuspence = "";
                    string ATMCash = "";

                    //if (Mpa.TargetSystem == 1) Acc.ReadAndFindAccount("1000", WOperator, WAtmNo, Er.CurDes, "ATM Suspense");

                    Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Ac.DepCurNm, "ATM Suspense");

                    if (Acc.RecordFound == true)
                    {
                        ATMSuspence = Acc.AccNo;
                    }
                    else
                    {
                        MessageBox.Show("ATM Suspense Account Not Found for ATM :" + WAtmNo);
                    }

                    Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Ac.DepCurNm, "ATM Cash");
                    if (Acc.RecordFound == true)
                    {
                        ATMCash = Acc.AccNo;
                    }
                    else
                    {
                        MessageBox.Show("ATM Cash Account Not Found for ATM :" + WAtmNo);
                    }

                    Gadj.GL_AccountNo = ATMCash;
                    Gadj.Ccy = Ac.DepCurNm;

                    if (WOperator == "ETHNCY2N")
                    {
                        // Make correction of input General Ledger
                        Gadj.GL_Balance = G4.Gl_Balance_At_CutOff - G4.Cash_Loaded + G4.UnloadedMachine; 
                    }
                    else
                    {
                        Gadj.GL_Balance = G4.Gl_Balance_At_CutOff;
                    }

                    Gadj.DateCreated = DateTime.Now;
                    Gadj.Processed = false;

                    Gadj.ProcessedAtRMCycle = WReconcCycleNo;
                    Gadj.Operator = WOperator;

                    int GLSeqNo = Gadj.Insert_GL_Balances();
                }

                // ****************************
                // Set ANd Find Basic Information 
                // ****************************          

                WOperator = Ac.Operator;

                Rc.ReadReconcCategorybyGroupId(Ac.AtmsReconcGroup);

                WReconCategGroup = Rc.CategoryId;

                //**********************************

                // Update Notes and Values  

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                Na.InUserDate = DateTime.Now;
                Na.Cit_ExcelUpdatedDate = DateTime.Now;
                Na.Cit_UnloadedCounted = G4.UnloadedCounted;
                Na.Cit_Over = G4.OverFound;
                Na.Cit_Short = G4.ShortFound;
                Na.Cit_Loaded = G4.Cash_Loaded;

                // UPDATE OTHER USED FIELDS IN NA Record 
                Na.InReplAmount = G4.Cash_Loaded;
                Na.ReplAmountSuggest = G4.Cash_Loaded;
                Na.ReplAmountTotal = G4.Cash_Loaded;
                Na.InsuranceAmount = G4.Cash_Loaded;

                Na.ReplUserComment = G4.RemarksG4S;

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                // Find if for this ATM there is GL difference

                decimal GL_Adjusted = 0;

                GL_Adjusted = Gadj.FindAdjusted_GL_Balance_AND_Update_Session_First_Method(WOperator, WAtmNo, WSesNo, G4.ReplDateG4S);

                if (Gadj.IsDataFound == true)
                {
                    // THIS ASSIGNMENT ALready Done in Gadj.FindAdjusted_GL_Balance
                    //Na.Is_GL_Adjusted = true;
                    //Na.GL_Bal_Repl_Adjusted = GL_Adjusted;

                    // REFRESH Na
                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                }
                else
                {
                    // SET TO THIS for this new ATM 
                    // 

                    GL_Adjusted = G4.UnloadedCounted;

                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                    if (Na.IsNewAtm == true)
                    {
                        Na.Is_GL_Adjusted = true;                   
                    }
                    else
                    {
                        Na.Is_GL_Adjusted = false;
                    }

                    Na.GL_Bal_Repl_Adjusted = G4.UnloadedCounted;
                }

                // Initialise

                Na.DiffAtAtmLevel_Cit = false;
                Na.DiffAtHostLevel_Cit = false;
                Na.DiffWithErrors_Cit = false;

                if (G4.OverFound > 0 || G4.ShortFound > 0)
                {
                    Na.DiffAtAtmLevel_Cit = true;
                }
                if (G4.UnloadedCounted != GL_Adjusted)
                {
                    Na.DiffAtHostLevel_Cit = true;
                }
                Ec.ReadAllErrorsTableForCounterReplCycle(WOperator, WAtmNo, WSesNo);

                if (Ec.TotalErrorsAmtLess100 > 0)
                {
                    Na.DiffWithErrors_Cit = true;
                }

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                // UPDATE TRACES WITH FINISH 
                // Update all fields and Reconciliation mode = 2 if all reconcile and Host files available 
                // After "Replenishement and Before reconciliation 

                Ta.UpdateTracesFinishRepl_From__G4S(WAtmNo, WSesNo, WSignedId, WReconCategGroup);

                // READ LATEST FROM Ta
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (Ta.ProcessMode == 1)
                {
                    TotalInDiff = TotalInDiff + 1; 
                }

                // READ AGAIN G4 Record 
                G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 1);

                // ************************************************************
                // CHECK GL DIFFERENCES 
                // ************************************************************

                // Ready for replenishment 

                // Compare GL_Adjusted VS G4.UnloadedCounted 

                if (G4.UnloadedCounted != GL_Adjusted || G4.OverFound != 0 || G4.ShortFound != 0 || Ec.NumOfErrors > 0)
                {
                    // There is difference

                    Rcs.ReadReconcCategoriesSessionsSpecific(WOperator, WReconCategGroup, WReconcCycleNo);

                    Rcs.GL_Original_Atms_Cash_Diff = Rcs.GL_Original_Atms_Cash_Diff + 1;

                    Rcs.GL_Remain_Atms_Cash_Diff = Rcs.GL_Remain_Atms_Cash_Diff + 1;

                    Rcs.UpdateReconcCategorySession_ForAtms_Cash_Diff(WReconCategGroup, WReconcCycleNo);

                }
                else
                {
                    Am.ReadAtmsMainSpecific(WAtmNo);
                    Am.Maker = "N/A";
                    Am.Authoriser = "N/A";
                    Am.UpdateAtmsMain(WAtmNo); 
                }

                //
                // FINALLY UPDATE G4S Records 
                //

                // Update G4.Record.With Process Mode == 1
                G4.ProcessMode_Load = 1; // Updated Mode
                G4.UpdateCIT_G4S_Repl_EntriesRecord(WSeqNo, 1);

                // Read and Update Update corresponding Banks record  
                G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 2);
                G4.ProcessMode_Load = 1; // 

                G4.RemarksG4S = "No Remark";

                G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 2);

                // CREATE TRANSACTIONS TO BE POSTED FOR REPLENISHMENT 
                // Whole Amount Goes to ATM Cash 
                if (G4.CITId != "1000")
                {
                    // Transactions = 1000 were made during replenishment
                    Ec.CreateTransTobepostedfromReplenishment_CIT(WOperator, WAtmNo, WSesNo, WSignedId, WSignedId, Na.Cit_UnloadedCounted, Na.Cit_Loaded);
                }
                
                I++; // Read Next entry of the table 

            }
            //  

            // Update Cycle Record
            Cec.ReadExcelLoadCyclesBySeqNo(WExcelLoadCycle);

            //Cec.ValidInExcelRecords = Total11;

            //Cec.NotInBank = Total10;
            //Cec.NotInG4S = Total01;
            //Cec.PresenterNumberEqual = TotalPresenterEqual;
            //Cec.PresenterDiff = TotalPresenterNotEqual;
            //Cec.ShortFound = TotalShortFound;

            //Cec.PresenterDiffAmt = TotalPresenterDiffAmt;
            //Cec.ShortAmt = TotalShortAmt;

            //Cec.AtmsInGl_Differ = TotalInDiff; 

            //Cec.InvalidInExcelRecords = TotalAA;

            Cec.ProcessStage = 3;

            Cec.FinishDateTm = DateTime.Now;

            Cec.UpdateLoadExcelCycle(WExcelLoadCycle);

            // Dissable Reversal Entry 
            buttonReverse.Enabled = false;

            if (TotalInDiff > 0)
            {
                Form2 MessageForm = new Form2("Updating Done! " + Environment.NewLine
                                          + "Transactions for ATMs Cash GL account created for posting. " + Environment.NewLine
                                          + "There is need for GL reconciliation "+ Environment.NewLine
                                          + "For :"  +TotalInDiff.ToString() + "..ATM/s "+ Environment.NewLine
                                          );
                MessageForm.ShowDialog();
            }
            else
            {
                Form2 MessageForm = new Form2("Updating Done! " + Environment.NewLine
                                         + "Transactions for ATMs Cash GL account created for posting. " + Environment.NewLine
                                         + "Not found differences." 
                                         );
                MessageForm.ShowDialog();
            }
            RRDMUserSignedInRecords Usr = new RRDMUserSignedInRecords();
            Usr.ReadSignedActivity(WSignedId);

        }
        //
        // Show Supervisor Mode Lines
        //
        private void buttonShowSM_Click(object sender, EventArgs e)
        {
            int WTraceNo;

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            WTraceNo = Ta.FirstTraceNo;
            DateTime WDateA = Ta.SesDtTimeEnd.Date;
            // Check Trace No
            Int32 LastDigit = WTraceNo % 10;

            if (LastDigit == 0)
            {
                // OK
                // It is just a transaction
            }
            else
            {
                // It is the supervisor mode
                WTraceNo = (WTraceNo - LastDigit) + 1;
            }

            Form67 NForm67;
            int Mode = 5; // Specific

            NForm67 = new Form67(WSignedId, 0, WOperator, 0, WAtmNo, WTraceNo, WTraceNo, WDateA, NullPastDate, Mode);
            NForm67.ShowDialog();
        }

    }
}


using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.InteropServices;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Text;
using RRDM4ATMs;


namespace RRDM4ATMsWin
{
    public partial class UCForm276a_AUDI_FirstStep_Single : UserControl
    {
        public string guidanceMsg;

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        DateTime WDate;
      
        int TempMode;

        int WReconcCycleNo;

        DateTime FutureDate = new DateTime(2050, 11, 21);

        DateTime WDtFrom = new DateTime(2021, 08, 20);

        DateTime WDtTo = new DateTime(2021, 11, 21);
        
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();


        //     FormMainScreen NFormMainScreen;
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCitId;
        int WLoadingExcelCycle;
        int WRMCycle;

        public void UCForm276a_AUDI_FirstStep_Par_Single(string InSignedId, int SignRecordNo, string InOperator,
                                                    string InCitId, int InLoadingCycle, int InRMCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId;
            WLoadingExcelCycle = InLoadingCycle;
            WRMCycle = InRMCycle;

            InitializeComponent();

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            label19.Text = "___________________________________________________________________________________________________________________________________"
                +"_____________________________________________________________________________________________"
                ; 
            string WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
                                   //   NormalProcess = false;

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor)
            //      if (WViewFunction || WAuthoriser || WRequestor || WViewHistory)
            {

                // button8.Enabled = false;
                buttonConfirm.Enabled = false;

                buttonUpdateHash.Hide();

                buttonAdd.Enabled = false; 
                buttonUpdate.Enabled = false; 
                buttonDelete.Enabled = false;

                checkBoxSameAsPrevious.Enabled = false; 

            }
            else
            {
                //  NormalProcess = true;
            }

            if (WAuthoriser == true)
            {
                //panel2.Location.X.Equals = 9; 
            }

        }

        // SHOW SCREEN 
        // ON LOAD 
        int WTableSize;
        bool InternalSwitch;
        decimal TotalInputtedOver1000; 
        public void SetScreen()
        {

            // SHOW GRID

            TempMode = 1;
            string SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingExcelCycle;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable_Feeding(WOperator, WSignedId, SelectionCriteria, TempMode);
            if (G4.DataTableG4SEntries.Rows.Count > 0)
            {
                // Entries Exist
                label2.Show();
                labelHeader.Show(); 
                panel1.Show(); 
                panel6.Show();
                buttonConfirm.Hide();

                buttonUpdateHash.Show(); 

                // Read Specific 
                SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingExcelCycle;
                G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);

                textBoxExcelName.Text = G4.OriginFileName;
                dateTimePicker1.Value = G4.ExcelDate;

                textBoxCount.Text = G4.WTotalEntries.ToString(); 
                textBoxTotalAmt.Text = G4.WTotalLoaded.ToString("#,##0.00");

                Cec.ReadExcelLoadCyclesBySeqNo(WLoadingExcelCycle);

                //Cec.ExcelId = G4.OriginFileName;

                Cec.UpdateLoadExcelCycle(WLoadingExcelCycle);

                Cec.ReadExcelLoadCyclesBySeqNo(WLoadingExcelCycle);

                //textBoxExcelName.Text = Cec.ExcelId;
                //HashTotalOver1000 = Cec.LoadedTotal/1000;
                textBoxHashTotal.Text = HashTotalOver1000.ToString("##0");
                textBoxHashTotal2.Text = HashTotalOver1000.ToString("##0");
                //textBoxTotalAmt.Text = G4.WTotalLoaded/1000.ToString("##0");

                TotalInputtedOver1000 = G4.WTotalLoaded/1000;
                textBoxTotalAmt.Text = TotalInputtedOver1000.ToString("##0");
                textBoxRemain.Text = (HashTotalOver1000 - (TotalInputtedOver1000)).ToString("##0");

                if ((HashTotalOver1000 - (TotalInputtedOver1000))<0)
                {
                    MessageBox.Show("The Excel Amount is less than the inputted amount"+Environment.NewLine
                        + "Change the figure or delete an inputed entry "
                        + " "
                        ); 
                }

                if (HashTotalOver1000 == TotalInputtedOver1000)
                {
                   // ExcelInserted = true;

                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    Usi.WFieldNumeric12 = 45; // Excel had populated RRDM Repository

                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                //buttonInportExcel.Hide();

                labelHeader.Text = "INPUTTED FEEDING ENTRIES IN RRDM";

                ShowGrid1();

            }
            else
            {
                Cec.ReadExcelLoadCyclesBySeqNo(WLoadingExcelCycle); 
                //textBoxExcelName.Text = Cec.ExcelId;
                //HashTotalOver1000 = Cec.LoadedTotal; 
                //if (Cec.LoadedTotal > 0)
                //{
                //    textBoxHashTotal.Text = Cec.LoadedTotal.ToString("##0");
                //    textBoxTotalAmt.Text = Cec.LoadedTotal.ToString("##0");
                //}
                //else
                //{
                //    textBoxHashTotal.Text = "";
                //    textBoxTotalAmt.Text = "";
                //}
                label2.Hide();
                labelHeader.Hide();
                panel1.Hide();
                panel6.Hide();
                buttonConfirm.Show();
                buttonUpdateHash.Hide();
            }

        }

        // Row Enter
        int WSeqNo;
        int WMode;
        decimal OldLoadedOver1000; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;
            WMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, WMode);

            OldLoadedOver1000 = G4.Cash_Loaded/1000;

            textBoxOrderNo.Text = G4.OrderNo.ToString();

            textBoxAtm.Text = G4.AtmNo;

            textBoxNextFeedingCash.Text = (G4.Cash_Loaded/1000).ToString("##0"); // Divide By Thousands

        }


        // Browse for Excel 
        string WExcelFileName;
        string ExcelBrowseDir = ConfigurationManager.AppSettings["BrowseExcelDir"];
        private void buttonBrowse_Click(object sender, EventArgs e)
        {

            string SelectionCriteria = " WHERE OriginFileName ='" + WExcelFileName + "'";
            TempMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);

            if (G4.RecordFound == true)
            {
                MessageBox.Show("Operation not Allowed!"+Environment.NewLine
                    + "Records already inputted for this excel"
                    );
                return; 
            }

            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.InitialDirectory = ExcelBrowseDir;
                //dlg.Filter = "JPG Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif";
                //Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Admin\Desktop\Dropbox\Vandit's Folder\Internship\test.xlsx");
                dlg.Filter = "XLSX Files (*.xlsx)|*.xlsx";
                dlg.Title = "Select an Excel";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    WExcelFileName = dlg.FileName.ToString();
                    textBoxExcelName.Text = WExcelFileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        decimal HashTotalOver1000; 
        // Confirm EXCEL INFORMATTION 
        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            if (textBoxExcelName.Text == "")
            {
                MessageBox.Show("Please Browse for Excel");
                return;
            }

            if (decimal.TryParse(textBoxHashTotal.Text, out HashTotalOver1000))
            {
                if (HashTotalOver1000 == 0)
                {
                    MessageBox.Show("Please enter a valid Total Amount!");

                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid Total Amount!");

                return;
            }


            WExcelFileName = textBoxExcelName.Text; 

            //Excel.Application xlApp = new Excel.Application();
            ////Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Admin\Desktop\Dropbox\Vandit's Folder\Internship\test.xlsx");
            //Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(WExcelFileName);
            //Excel.Worksheet xlWorksheet = xlWorkbook.ActiveSheet;

            WDate = dateTimePicker1.Value.Date;

            string WDay = "";
            string WMonth = "";
            if (WDate.Day < 10)
            {
                WDay = "0" + WDate.Day;
            }
            else WDay = WDate.Day.ToString();

            if (WDate.Month < 10)
            {
                WMonth = "0" + WDate.Month;
            }
            else WMonth = WDate.Month.ToString();

            string DateString = WDay + "." + WMonth + "." + WDate.Year; // eg ("17.11.2017")

            if (WExcelFileName.Contains(DateString))
            {
                //MessageBox.Show("Dates are Matched" + Environment.NewLine
                //                 + "Date selected equal to the one on excel"); 

            }
            else
            {
                MessageBox.Show("Date selected NOT equal to the one on excel");
                return;
            }
            //
            // Check if Already Read 
            //
            string SelectionCriteria = " WHERE OriginFileName ='" + WExcelFileName + "'";
            TempMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);

            if (G4.RecordFound == true)
            {
                if (G4.ProcessMode_Load != 1)
                {
                    MessageBox.Show("This Excel ALready Read and Processed" + Environment.NewLine
                                + "Please Read the Correct Excel " + Environment.NewLine
                                + "Or Continue Input " + Environment.NewLine
                               );

                    // SHOW GRID

                    TempMode = 1;
                    SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND OriginFileName ='" + WExcelFileName + "'";

                    G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);
                    if (G4.DataTableG4SEntries.Rows.Count > 0)
                    {
                        // Entries Exist
                        panel6.Show();
                        SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND OriginFileName ='" + WExcelFileName + "'";
                        G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);

                        textBoxExcelName.Text = G4.OriginFileName;

                        // buttonInportExcel.Hide();

                        labelHeader.Text = "EXCEL ENTRIES AS IN RRDM";

                        SetScreen();

                        //button8.Hide();

                        // buttonReverse.Show();


                    }
                    else
                    {
                        panel6.Hide();
                    }
                    //buttonReverse.Show();
                    //return;
                }
                else
                {
                    //ExcelChecked = true;
                    //MessageBox.Show("This Excel ALready Read and Processed" + Environment.NewLine
                    //               + "Updating was done!" + Environment.NewLine
                    //               + "Reversal not allowed."
                    //               );

                    //xlWorkbook.Close();

                    //xlApp.Quit();

                    //Marshal.FinalReleaseComObject(xlApp);
                    //Marshal.FinalReleaseComObject(xlWorkbook);
                    //Marshal.ReleaseComObject(xlWorksheet);
                    //return;

                }

            }
            else
            {
                textBoxTotalAmt.Text = "0";
                textBoxRemain.Text = HashTotalOver1000.ToString("#,##0.00");
            }

            Cec.ReadExcelLoadCyclesBySeqNo(WLoadingExcelCycle);

            //Cec.ExcelId = textBoxExcelName.Text;
            //Cec.LoadedTotal = HashTotalOver1000*1000; 

            Cec.UpdateLoadExcelCycle(WLoadingExcelCycle);

            label2.Show();
            labelHeader.Show();
            panel1.Show();
            panel6.Show();

            //xlWorkbook.Close();

            //xlApp.Quit();

            //Marshal.FinalReleaseComObject(xlApp);
            //Marshal.FinalReleaseComObject(xlWorkbook);
            //Marshal.ReleaseComObject(xlWorksheet);
           // Marshal.ReleaseComObject(xlRange);

            // buttonConfirm.Hide(); 
        }



        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGrid1()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();

            dataGridView1.DataSource = G4.DataTableG4SEntries.DefaultView;

            textBoxCount.Text = dataGridView1.Rows.Count.ToString();

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 60; // Order Number
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 60; //  ATM No 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 190; // ATM Name 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 90; // Feeding Amt 
            dataGridView1.Columns[4].DefaultCellStyle = style;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].Visible = true;

            dataGridView1.Columns[5].Width = 80; // Excel date 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = true;

            //dataGridView1.Columns[5].Width = 50; // LoadingCycleNo
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[5].Visible = false;

            //dataGridView1.Columns[6].Width = 50; //ReplDateG4S
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[6].Visible = false;

            //dataGridView1.Columns[7].Width = 50; // OrderId 
            //dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[7].Visible = false;

            //dataGridView1.Columns[8].Width = 50; // CreatedDate
            //dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[8].Visible = false;

            //dataGridView1.Columns[9].Width = 50; // IsDeposit
            //dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[9].HeaderText = "Is Deposit";

            //dataGridView1.Columns[10].Width = 70; // OpeningBalance
            //dataGridView1.Columns[10].DefaultCellStyle = style;
            //dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[10].HeaderText = "Opening Balance";

            //dataGridView1.Columns[11].Width = 70; //Dispensed
            //dataGridView1.Columns[11].DefaultCellStyle = style;
            //dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView1.Columns[12].Width = 70; // UnloadedMachine
            //dataGridView1.Columns[12].DefaultCellStyle = style;
            //dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[12].HeaderText = "Unloaded Machine";

            //dataGridView1.Columns[13].Width = 70; // UnloadedCounted
            //dataGridView1.Columns[13].DefaultCellStyle = style;
            //dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[13].HeaderText = "Unloaded Counted";

            //dataGridView1.Columns[14].Width = 80; // Cash_Loaded
            //dataGridView1.Columns[14].DefaultCellStyle = style;
            //dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[14].HeaderText = "Cash Loaded";

            //dataGridView1.Columns[15].Width = 70; // Deposits
            //dataGridView1.Columns[15].DefaultCellStyle = style;
            //dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView1.Columns[16].Width = 70; //OverFound
            //dataGridView1.Columns[16].DefaultCellStyle = style;
            //dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[16].HeaderText = "Over Found";

            //dataGridView1.Columns[17].Width = 70; // ShortFound
            //dataGridView1.Columns[17].DefaultCellStyle = style;
            //dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[17].HeaderText = "Short Found";

        }
        //
        // Catch Details 
        //
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

            MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

            //Environment.Exit(0);
        }

        // ADD 

        string WAtmNo;
        int WSesNo;

        //string WCitId; 
        string WAtmName; // Location
        string OriginFileName;
        int WOrderNo;
        decimal Loaded_NextOver1000;
        DateTime ReplCycleStart;
        decimal LoadedLast;
        decimal CurrentBalance;
        int AddSeqNo; 
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Read Specific 
            AddSeqNo = 0;

            Ac.ReadAtm(textBoxAtm.Text);
            if (Ac.RecordFound == true)
            {
                // OK
            }
            else
            {
                MessageBox.Show("Please enter correct ATM no! " + Environment.NewLine
                   //+ "Feeding Amount:" + G4.Cash_Loaded + Environment.NewLine
                  
                   );
            }

            string SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingExcelCycle
                                       + " AND AtmNo ='"+ WAtmNo + "' " + " AND ProcessedAtReplCycleNo =" + WSesNo;
            G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);

            if (G4.RecordFound == true)
            {
                MessageBox.Show("This Entry Already exist! "+Environment.NewLine
                    + "Feeding Amount:"+G4.Cash_Loaded + Environment.NewLine
                    + "At Excel Id:"+ G4.OriginFileName + Environment.NewLine
                    + "At Excel Cycle:" + G4.LoadingExcelCycleNo + Environment.NewLine
                    );
                return; 
            }


            // Validation 
            if (int.TryParse(textBoxOrderNo.Text, out WOrderNo))
            {
                if (WOrderNo == 0)
                {
                    MessageBox.Show("Please enter a valid number for Order!");

                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number for Order!");

                return;
            }

            if (decimal.TryParse(textBoxNextFeedingCash.Text, out Loaded_NextOver1000))
            {
                if (Loaded_NextOver1000 == 0)
                {
                    MessageBox.Show("Please enter a valid number for Amount!");

                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number for Amount!");

                return;
            }

            if (((TotalInputtedOver1000)+Loaded_NextOver1000) > HashTotalOver1000)
            {
                MessageBox.Show("Operation not allowed!" + Environment.NewLine
                                + "Total Inputted Amount will be more than the defined Total"
                                         );
                return; 
            }

            WMode = 1; // CIT mode

            G4.OriginFileName = textBoxExcelName.Text;
            G4.ExcelDate = dateTimePicker1.Value; //  Excel date

            G4.CITId = WCitId;
            G4.AtmNo = WAtmNo;
            G4.AtmName = WAtmName;
            G4.LoadingExcelCycleNo = WLoadingExcelCycle;

            G4.ReplCycleNo = WSesNo;
            G4.OrderNo = WOrderNo;
            G4.CreatedDate = DateTime.Now ; //  Excel date 

            G4.Cash_Loaded = (Loaded_NextOver1000)*1000; // Amount * 1000

            G4.Operator = WOperator;

            G4.LoadedAtRMCycle = WRMCycle;

            AddSeqNo = G4.InsertCIT_G4S_ATM_Feeding_Entries(WMode);

            //TempMode = 1;
            //SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingExcelCycle;


            //G4.ReadCIT_G4S_Repl_EntriesToFillDataTable_Feeding(WOperator, WSignedId, SelectionCriteria, TempMode);

            //if (dataGridView1.SelectedRows[0].Index>=0)
            //{
            //    int WRow = dataGridView1.SelectedRows[0].Index;
            //    int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            //    // Load Grid 
            //    //Form47_Load(this, new EventArgs());
            //    SetScreen();

            //    dataGridView1.Rows[WRow].Selected = true;
            //    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            //    dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
            //}
            //else
            //{
            //    SetScreen();
            //}

            SetScreen();

        }
        // ATM No is changed 
        private void textBoxAtm_TextChanged(object sender, EventArgs e)
        {
            Ac.ReadAtm(textBoxAtm.Text);
            if (Ac.RecordFound == true)
            {
                WAtmNo = textBoxAtm.Text;

                panel2.Show();
                label1.Show();

                //dataGridView2.Show();
                //labelCycleNo.Show(); 

                // Do 
                WAtmName = Ac.AtmName;

                textBox10.Text = WAtmName; 

                WCitId = Ac.CitId;
                textBoxCit.Text = WCitId;

                textBoxNextFeedingCash.Text = "0";

                Ta.ReadReplCyclesForFromToDateFillTable(WOperator, WSignedId, WAtmNo, WDtFrom, WDtTo);

                dataGridView2.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;

                dataGridView2.Columns[0].Width = 70; // SesNo
                dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                // dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);

                dataGridView2.Columns[1].Width = 130; // SesDtTimeStart
                dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[2].Width = 130; // SesDtTimeEnd
                dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[3].Width = 180; // ProcessMode
                dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView2.Columns[4].Width = 65; // Mode_2
                dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView2.Columns[5].Width = 100; // AtmNo 
                dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


                Ta.READ_CycleIn_Progress(WAtmNo);

                if (Ta.RecordFound == true)
                {
                    // 
                    if (dateTimePicker1.Value >= Ta.SesDtTimeStart.Date)
                    {
                        // This is OK 
                        WSesNo = Ta.SesNo;
                        ReplCycleStart = Ta.SesDtTimeStart;
                        textBox3.Text = ReplCycleStart.ToString();
                    }
                    else
                    {
                        // Try again 
                        MessageBox.Show("ExcelDate less than last replenishment"+Environment.NewLine
                            + "We will get the previous one"
                            );

                        Ta.READ_Cycle_WithinDate(WAtmNo, dateTimePicker1.Value);

                        WSesNo = Ta.SesNo;
                        ReplCycleStart = Ta.SesDtTimeStart;
                        textBox3.Text = ReplCycleStart.ToString();
                       
                    }

                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 1);
                    LoadedLast = Na.Balances1.OpenBal;
                    
                    textBox4.Text = LoadedLast.ToString("#,##0.00");

                    CurrentBalance = Na.Balances1.ReplToRepl;
                    //textBox12.Text = CurrentBalance.ToString("#,##0.00");
                }
                else
                {
                    MessageBox.Show("Repl Cycle in Progress not found");
                    return;
                }


            }
            else
            {
                // Skip 

                panel2.Hide();
                label1.Hide();

                dataGridView2.Hide();
                labelCycleNo.Hide();

                textBoxNextFeedingCash.Text = "0";
                //textBoxNextFeedingCash2.Text = "0";
                textBox10.Text = ""; 
            }
        }
// Insert default 
        private void checkBoxAddCaptured_CheckedChanged(object sender, EventArgs e)
        {

        }
        // UPDATE 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            // Validation 
            if (int.TryParse(textBoxOrderNo.Text, out WOrderNo))
            {
                if (WOrderNo == 0)
                {
                    MessageBox.Show("Please enter a valid number for Order!");

                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number for Order!");

                return;
            }

            if (decimal.TryParse(textBoxNextFeedingCash.Text, out Loaded_NextOver1000))
            {
                if (Loaded_NextOver1000 == 0)
                {
                    MessageBox.Show("Please enter a valid number for Amount!");

                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number for Amount!");

                return;
            }

            if ((TotalInputtedOver1000-OldLoadedOver1000 + Loaded_NextOver1000) > HashTotalOver1000)
            {
                MessageBox.Show("Operation not allowed!" + Environment.NewLine
                                + "Total Inputted Amount will be more than the defined Total"
                                         );
                return;
            }

            WMode = 1; // CIT mode

            G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, WMode); 

            G4.AtmNo = WAtmNo;
            G4.AtmName = WAtmName;
            
            G4.ReplCycleNo = WSesNo;
            G4.OrderNo = WOrderNo;
            G4.CreatedDate = DateTime.Now; //  Excel date 

            G4.Cash_Loaded = Loaded_NextOver1000*1000; // Amount 

            G4.UpdateCIT_G4S_Repl_EntriesRecord(WSeqNo, WMode);

            SetScreen();
        }
// DELETE ENTRY 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this Entry?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
            {
                WMode = 1;
                G4.DeleteCIT_G4S_Repl_EntriesRecord(WSeqNo, WMode);
                SetScreen();
            }
            else
            {
                return;
            }
           
        }
        // input Thousands 
        decimal HashTotal2; 
        private void textBoxHashTotal_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxHashTotal.Text, out HashTotal2))
            {
                //if (HashTotal == 0)
                //{
                //    MessageBox.Show("Please enter a valid Total Amount!");

                //    return;
                //}
                textBox5.Text = (HashTotal2 * 1000).ToString("#,##0.00");
            }
            else
            {
                if (textBoxHashTotal.Text != "")
                {
                    MessageBox.Show("Please enter a valid Total Amount!");
                    textBox5.Text = "";
                }
                else
                {
                    textBox5.Text = "";
                }
                return;
            }

            
        }
        // Inputed detail 
        decimal Loaded_Next2; 
        private void textBoxNextFeedingCash_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxNextFeedingCash.Text, out Loaded_Next2))
            {
                //if (HashTotal == 0)
                //{
                //    MessageBox.Show("Please enter a valid Total Amount!");

                //    return;
                //}
                checkBoxSameAsPrevious.Checked = false; 
                textBoxNextFeedingCash2.Text = (Loaded_Next2 * 1000).ToString("#,##0.00");
            }
            else
            {
                if (textBoxNextFeedingCash.Text != "")
                {
                    MessageBox.Show("Please enter a valid Total Amount!");
                    textBoxNextFeedingCash2.Text = "";
                }
                else
                {
                    textBoxNextFeedingCash2.Text = "";
                }
                return;
            }
        }
// Update Hash 
        private void buttonUpdateHash_Click(object sender, EventArgs e)
        {
            if (textBoxExcelName.Text == "")
            {
                MessageBox.Show("Please Browse for Excel");
                return;
            }

            if (decimal.TryParse(textBoxHashTotal.Text, out HashTotalOver1000))
            {
                if (HashTotalOver1000 == 0)
                {
                    MessageBox.Show("Please enter a valid Total Amount!");

                    return;
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid Total Amount!");

                return;
            }


            WExcelFileName = textBoxExcelName.Text;

            WDate = dateTimePicker1.Value.Date;

            string WDay = "";
            string WMonth = "";
            if (WDate.Day < 10)
            {
                WDay = "0" + WDate.Day;
            }
            else WDay = WDate.Day.ToString();

            if (WDate.Month < 10)
            {
                WMonth = "0" + WDate.Month;
            }
            else WMonth = WDate.Month.ToString();

            string DateString = WDay + "." + WMonth + "." + WDate.Year; // eg ("17.11.2017")

            if (WExcelFileName.Contains(DateString))
            {
                //MessageBox.Show("Dates are Matched" + Environment.NewLine
                //                 + "Date selected equal to the one on excel"); 

            }
            else
            {
                MessageBox.Show("Date selected NOT equal to the one on excel");
                return;
            }
            //
            // Check if Already Read 
            //
            string SelectionCriteria = " WHERE OriginFileName ='" + WExcelFileName + "'";
            TempMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);

            if (G4.RecordFound == true)
            {
                if (G4.ProcessMode_Load != -2)
                {
                    MessageBox.Show("This Excel ALready Read and Processed" + Environment.NewLine
                                + "Please Read the Correct Excel " + Environment.NewLine
                                + " " + Environment.NewLine
                               );

                    // SHOW GRID

                    TempMode = 1;
                    SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND OriginFileName ='" + WExcelFileName + "'";

                    G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);
                    if (G4.DataTableG4SEntries.Rows.Count > 0)
                    {
                        // Entries Exist
                        panel6.Show();
                        SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND OriginFileName ='" + WExcelFileName + "'";
                        G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);

                        textBoxExcelName.Text = G4.OriginFileName;

                        // buttonInportExcel.Hide();

                        labelHeader.Text = "EXCEL ENTRIES AS IN RRDM";

                        SetScreen();


                    }
                    else
                    {
                        panel6.Hide();
                    }
                    //buttonReverse.Show();
                    //return;
                }
                else
                {
                  

                }

            }
            else
            {
                textBoxTotalAmt.Text = "0";
                textBoxRemain.Text = HashTotalOver1000.ToString("#,##0.00");
            }

            Cec.ReadExcelLoadCyclesBySeqNo(WLoadingExcelCycle);

            //Cec.ExcelId = textBoxExcelName.Text;
            //Cec.LoadedTotal = HashTotalOver1000 * 1000;

            Cec.UpdateLoadExcelCycle(WLoadingExcelCycle);

            label2.Show();
            labelHeader.Show();
            panel1.Show();
            panel6.Show();

            SetScreen(); 

        }

        private void checkBoxSameAsPrevious_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSameAsPrevious.Checked == true)
            {
                textBoxNextFeedingCash.Text = (LoadedLast / 1000).ToString("##0");
            }
            else
            {
                textBoxNextFeedingCash.Text = "0";
            }
        }
        // Row enter 
        //int WWSesNo; 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

           // WWSesNo = (int)rowSelected.Cells[0].Value;

           // textBoxCycleNo.Text = WWSesNo.ToString(); 
        }
    }
}

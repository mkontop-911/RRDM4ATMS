using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Text;
using RRDM4ATMs;


namespace RRDM4ATMsWin
{
    public partial class UCForm276a_NBG : UserControl
    {
        public string guidanceMsg;

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

      //  int TotalUpdated;
        int TotalNew;

     //   bool UpdateMode;
     //   bool AddMode;
     //   bool ErrorInValidation;

        bool ExcelInserted = false;
        DateTime WDate;
        bool ExcelChecked;
        int TempMode;

        //   int WExcelLoadCycle;
        int WReconcCycleNo; 

        DateTime FutureDate = new DateTime(2050, 11, 21);
       
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

        public void UCForm276a_NBG_Par(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InLoadingCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId;
            WLoadingExcelCycle = InLoadingCycle; 

            InitializeComponent();

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************

            string WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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
                //panelCassettes.Enabled = false;
                buttonReverse.Enabled = false;
                button8.Enabled = false; 

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
        public void SetScreen()
        {
           
            // SHOW GRID

            TempMode = 1;
            string SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingExcelCycle;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);
            if (G4.DataTableG4SEntries.Rows.Count > 0)
            {
                // Entries Exist
                panel6.Show();

                SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingExcelCycle;
                G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);

                textBoxExcelName.Text = G4.OriginFileName;

                buttonInportExcel.Hide();

                labelHeader.Text = "EXCEL ENTRIES AS IN RRDM";

                ShowGrid1();

                button8.Hide();

                buttonReverse.Show();

                ExcelInserted = true;

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.WFieldNumeric12 = 45; // Excel had populated RRDM Repository

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
            }
            else
            {
                panel6.Hide();
            }      

        }

        // Row Enter
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];


        }

      
        // Browse for Excel 
        string WExcelFileName;
        string ExcelBrowseDir = ConfigurationManager.AppSettings["BrowseExcelDir"];
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
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
// LOAD EXCEL
        private void buttonInportExcel_Click(object sender, EventArgs e)
        {
            if (textBoxExcelName.Text == "")
            {
                MessageBox.Show("Please Browse for Excel");
                return;
            }
            panel6.Show();

            //
            // CLEAR DataGridView1
            //
            do
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    try
                    {
                        dataGridView1.Rows.Remove(row);
                    }
                    catch (Exception) { }
                }
            } while (dataGridView1.Rows.Count > 0);

            Excel.Application xlApp = new Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Admin\Desktop\Dropbox\Vandit's Folder\Internship\test.xlsx");
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(WExcelFileName);
            Excel.Worksheet xlWorksheet = xlWorkbook.ActiveSheet;

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

            if (G4.RecordFound == true )
            {
                if (G4.ProcessMode_Load != 1)
                {
                    MessageBox.Show("This Excel ALready Read and Processed" + Environment.NewLine
                               + "Please Read the Correct Excel " + Environment.NewLine
                               + "OR Make Reversal "
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

                        buttonInportExcel.Hide();

                        labelHeader.Text = "EXCEL ENTRIES AS IN RRDM";

                        ShowGrid1();

                        button8.Hide();

                        buttonReverse.Show();

                        ExcelInserted = true;

                        Usi.ReadSignedActivityByKey(WSignRecordNo);

                        Usi.WFieldNumeric12 = 45; // Excel had populated RRDM Repository

                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    }
                    else
                    {
                        panel6.Hide();
                    }
                    buttonReverse.Show();
                    return;
                }
                else
                {
                    //ExcelChecked = true;
                    MessageBox.Show("This Excel ALready Read and Processed" + Environment.NewLine
                                   + "Updating was done!" + Environment.NewLine
                                   + "Reversal not allowed."
                                   );
                    return;

                }

            }
      

            try
            {
                Excel.Range xlRange;
                xlRange = xlWorksheet.UsedRange;
                int Rows_Count = xlRange.Rows.Count;
                int Columns_Count = xlRange.Columns.Count;

                int i = 0;
                //
                //Initializing Columns NAMES
                //
                dataGridView1.ColumnCount = Columns_Count;

                dataGridView1.Columns[0].Name = xlWorksheet.Cells[i + 1, 1].Value;
                dataGridView1.Columns[1].Name = xlWorksheet.Cells[i + 1, 2].Value;
                dataGridView1.Columns[2].Name = xlWorksheet.Cells[i + 1, 3].Value;
                dataGridView1.Columns[3].Name = xlWorksheet.Cells[i + 1, 4].Value;
                dataGridView1.Columns[4].Name = xlWorksheet.Cells[i + 1, 5].Value;
                dataGridView1.Columns[5].Name = xlWorksheet.Cells[i + 1, 6].Value;
                dataGridView1.Columns[6].Name = xlWorksheet.Cells[i + 1, 7].Value;
                dataGridView1.Columns[7].Name = xlWorksheet.Cells[i + 1, 8].Value;
                dataGridView1.Columns[8].Name = xlWorksheet.Cells[i + 1, 9].Value;
                dataGridView1.Columns[9].Name = xlWorksheet.Cells[i + 1, 10].Value;
                dataGridView1.Columns[10].Name = xlWorksheet.Cells[i + 1, 11].Value;
                dataGridView1.Columns[11].Name = xlWorksheet.Cells[i + 1, 12].Value;
                dataGridView1.Columns[12].Name = xlWorksheet.Cells[i + 1, 13].Value;
                dataGridView1.Columns[13].Name = xlWorksheet.Cells[i + 1, 14].Value;
                dataGridView1.Columns[14].Name = xlWorksheet.Cells[i + 1, 15].Value;
                dataGridView1.Columns[15].Name = xlWorksheet.Cells[i + 1, 16].Value;
                //dataGridView1.Columns[16].Name = worksheet.Cells[i + 1, 17].Value;
                //dataGridView1.Columns[17].Name = worksheet.Cells[i + 1, 18].Value;
                //dataGridView1.Columns[18].Name = worksheet.Cells[i + 1, 19].Value;
                //dataGridView1.Columns[19].Name = worksheet.Cells[i + 1, 20].Value;
                //dataGridView1.Columns[20].Name = worksheet.Cells[i + 1, 21].Value;
                //dataGridView1.Columns[21].Name = worksheet.Cells[i + 1, 22].Value;

                // *******************************
                // FILL UP ROWS 
                // *******************************

                i = 1;

                for (; i < Rows_Count; i++)
                {
                    if (xlWorksheet.Cells[i + 1, 1].Value == null
                      || xlWorksheet.Cells[i + 1, 1].Value.ToString() == ""
                      || xlWorksheet.Cells[i + 1, 1].Value.ToString() == "00"
                      || xlWorksheet.Cells[i + 1, 1].Value.ToString() == "0"
                      )
                    {
                        break;

                        //continue;
                    }

                    dataGridView1.Rows.Add
                        (xlWorksheet.Cells[i + 1, 1].Value,
                         xlWorksheet.Cells[i + 1, 2].Value,
                         xlWorksheet.Cells[i + 1, 3].Value,
                         xlWorksheet.Cells[i + 1, 4].Value,
                         xlWorksheet.Cells[i + 1, 5].Value,
                         xlWorksheet.Cells[i + 1, 6].Value,
                         xlWorksheet.Cells[i + 1, 7].Value,
                         xlWorksheet.Cells[i + 1, 8].Value,
                         xlWorksheet.Cells[i + 1, 9].Value,
                         xlWorksheet.Cells[i + 1, 10].Value,
                         xlWorksheet.Cells[i + 1, 11].Value,
                         xlWorksheet.Cells[i + 1, 12].Value,
                         xlWorksheet.Cells[i + 1, 13].Value,
                         xlWorksheet.Cells[i + 1, 14].Value,
                         xlWorksheet.Cells[i + 1, 15].Value,
                         xlWorksheet.Cells[i + 1, 16].Value
                         //worksheet.Cells[i + 1, 17].Value,
                         //worksheet.Cells[i + 1, 18].Value,
                         //worksheet.Cells[i + 1, 19].Value,
                         //worksheet.Cells[i + 1, 20].Value,
                         //worksheet.Cells[i + 1, 21].Value,
                         //worksheet.Cells[i + 1, 22].Value
                         );

                    //DataGridViewRow rowSelected = dataGridView1.Rows[i];

                }

                // .........

                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No Entries Available in excel!");
                    return;
                }
                else
                {
                    textBoxCount.Text = dataGridView1.Rows.Count.ToString();
                }

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 60; // S/N
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[0].HeaderText = "S/N"; 

                dataGridView1.Columns[1].Width = 60; //AtmNo
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[1].HeaderText = "AtmNo";

                dataGridView1.Columns[2].Width = 150; //Descr
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[3].Width = 90; // Cut Off Date
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[4].Width = 80; // GL Balance 
                dataGridView1.Columns[4].DefaultCellStyle = style;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[5].Width = 70; //Order 
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[6].Width = 90; // Repl Date 
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[7].Width = 80; // Previous Balance 
                dataGridView1.Columns[7].DefaultCellStyle = style;
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[3].HeaderText = "Previuous Balance";

                dataGridView1.Columns[8].Width = 80; // Dispensed 
                dataGridView1.Columns[8].DefaultCellStyle = style;
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[4].HeaderText = "Dispensed ";

                dataGridView1.Columns[9].Width = 80; // Cash Unload 
                dataGridView1.Columns[9].DefaultCellStyle = style;
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[5].HeaderText = "Cash Unload";

                dataGridView1.Columns[10].Width = 80; // Cash Loaded
                dataGridView1.Columns[10].DefaultCellStyle = style;
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[6].HeaderText = "Cash Loaded";

                dataGridView1.Columns[11].Width = 80; // Deposits
                dataGridView1.Columns[11].DefaultCellStyle = style;
                dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[7].HeaderText = "Deposits";

                dataGridView1.Columns[12].Width = 100; // Balance KD
                dataGridView1.Columns[12].DefaultCellStyle = style;
                dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[8].HeaderText = "Balance KD";

                dataGridView1.Columns[13].Width = 70; // ATM Over
                dataGridView1.Columns[13].DefaultCellStyle = style;
                dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[9].HeaderText = "ATM Over";

                dataGridView1.Columns[14].Width = 70; // Atm Short
                dataGridView1.Columns[14].DefaultCellStyle = style;
                dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[14].HeaderText = "Atm Short";

                dataGridView1.Columns[15].Width = 70; // Remarks
                dataGridView1.Columns[15].DefaultCellStyle = style;
                dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[15].HeaderText = "Remarks";

                xlWorkbook.Close();

                xlApp.Quit();

                Marshal.FinalReleaseComObject(xlApp);
                Marshal.FinalReleaseComObject(xlWorkbook);
                Marshal.ReleaseComObject(xlWorksheet);
                Marshal.ReleaseComObject(xlRange);

            }
            catch (Exception ex)
            {
                xlWorkbook.Close();

                xlApp.Quit();

                Marshal.FinalReleaseComObject(xlApp);
                Marshal.FinalReleaseComObject(xlWorkbook);
                Marshal.ReleaseComObject(xlWorksheet);

                CatchDetails(ex);

            }
            finally
            {

            }

            buttonInportExcel.Hide(); 
        }

   
// Populate Entries
        private void button8_Click(object sender, EventArgs e)
        {
            int SeqNo_0;
            string AtmNo_1;
            string Descr_2;

            DateTime Cut_Off_date_3;
            decimal Gl_Balance_At_CutOff_4;
            int Order_5;
            DateTime ReplDate_6;

            decimal OpeningBalance_7;

            decimal Dispensed_8;
            decimal CountedCashUnload_9;
            decimal CashLoaded_10;
            decimal Deposits_11;

            decimal BalanceKD_12;
            decimal AtmOver_13;
            decimal AtmShort_14;

            string Remarks_15;

            //string TempSeq; 

            // Read Excel Line by Line 
            // Read Model ATM
            // Update the ones found 
            // Create new ones for the not found. 
            // Update Migration Session No 

            ExcelChecked = false;

            TotalNew = 0;

            // Update Excel Loading Cycle 
       
            //Cec.Cut_Off_Date = WDate;
          
            //Cec.ExcelId = WExcelFileName;
            Cec.ProcessStage = 1;

            Cec.UpdateLoadExcelCycleAtLoadingStep(WLoadingExcelCycle); 
            //Clear Table 
            //Tr.DeleteReport58(WSignedId);
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Entries to show");
                return;
            }

            int K = 0;

            while (K <= (dataGridView1.Rows.Count - 1))
            {
               // UpdateMode = false;
              //  AddMode = false;
             //   ErrorInValidation = false;

                DataGridViewRow rowSelected = dataGridView1.Rows[K];

                if (rowSelected.Cells[0].Value == null
                    || rowSelected.Cells[0].Value.ToString() == ""
                    || rowSelected.Cells[0].Value.ToString() == "00"
                    || rowSelected.Cells[0].Value.ToString() == "0"
                    )
                {
                    break;

                    //continue;
                }
                else
                {
                    G4.SeqNo = SeqNo_0 = int.Parse(rowSelected.Cells[0].Value.ToString());
                    G4.AtmNo = AtmNo_1 = rowSelected.Cells[1].Value.ToString();
                    if (G4.AtmNo == "" || G4.AtmNo == "00" || G4.AtmNo == "0")
                    {
                        break;
                    }
                    G4.AtmName = Descr_2 = rowSelected.Cells[2].Value.ToString();
                    if (G4.AtmName == "CDM")
                    {
                        break;
                    }

                    G4.Cut_Off_date = Cut_Off_date_3 = DateTime.Parse(rowSelected.Cells[3].Value.ToString());
                    G4.Gl_Balance_At_CutOff = Gl_Balance_At_CutOff_4 = decimal.Parse(rowSelected.Cells[4].Value.ToString());

                    G4.OrderNo = Order_5 = int.Parse(rowSelected.Cells[5].Value.ToString());
                    G4.ReplDateG4S = ReplDate_6 = DateTime.Parse(rowSelected.Cells[6].Value.ToString());

                    G4.OpeningBalance = OpeningBalance_7 = decimal.Parse(rowSelected.Cells[7].Value.ToString());

                    G4.Dispensed = Dispensed_8 = decimal.Parse(rowSelected.Cells[8].Value.ToString());

                    G4.UnloadedCounted = CountedCashUnload_9 = decimal.Parse(rowSelected.Cells[9].Value.ToString());
                    G4.Cash_Loaded = CashLoaded_10 = decimal.Parse(rowSelected.Cells[10].Value.ToString());
                    G4.Deposits = Deposits_11 = decimal.Parse(rowSelected.Cells[11].Value.ToString());

                    BalanceKD_12 = decimal.Parse(rowSelected.Cells[12].Value.ToString());

                    G4.OverFound = AtmOver_13 = decimal.Parse(rowSelected.Cells[13].Value.ToString());
                    G4.ShortFound = AtmShort_14 = decimal.Parse(rowSelected.Cells[14].Value.ToString());

                    G4.RemarksG4S = Remarks_15 = (rowSelected.Cells[15].Value.ToString());

                    G4.CITId = WCitId;

                    G4.OriginFileName = WExcelFileName;

                    G4.UnloadedMachine = OpeningBalance_7 - Dispensed_8;
                    // G4.UnloadedMachine = 0; 

                    //G4.ReplDateG4S = WDate;

                    if (CashLoaded_10 > 0)
                    {
                        G4.IsDeposit = false;
                    }
                    else G4.IsDeposit = true;

                    G4.Operator = WOperator;

                    //
                    // Check Excel not already checked 
                    //

                    if (ExcelChecked == false)
                    {
                        TempMode = 1;
                        //  
                        // Check if Excel Already Read
                        //
                        string SelectionCriteria = " WHERE OriginFileName ='" + WExcelFileName + "'";
                        G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);

                        if (G4.RecordFound == true)
                        {
                            MessageBox.Show("This Excel ALready Read and Processed" + Environment.NewLine
                                            + "Please Read the Correct Excel " + Environment.NewLine
                                            + "OR Make Reversal "
                                            );

                            buttonReverse.Show();

                            return;
                        }
                        else
                        {
                            ExcelChecked = true;
                        }
                    }

                    // NEW Fields 
                    G4.LoadingExcelCycleNo = WLoadingExcelCycle;

                    G4.ReplCycleNo = 0;

                    G4.PresentedErrors = 0;

                    //
                    // Insert Record in Excel Like in Report 
                    //
                    G4.Load_FaceValue_1 = 0;
                    G4.Load_Cassette_1 = 0;
                    G4.Load_FaceValue_2 = 0;
                    G4.Load_Cassette_2 = 0;
                    G4.Load_FaceValue_3 = 0;
                    G4.Load_Cassette_3 = 0;
                    G4.Load_FaceValue_4 = 0;
                    G4.Load_Cassette_4 = 0;

                    G4.Un_Load_FaceValue_1 = 0;
                    G4.Un_Load_Cassette_1 = 0;
                    G4.Un_Load_FaceValue_2 = 0;
                    G4.Un_Load_Cassette_2 = 0;
                    G4.Un_Load_FaceValue_3 = 0;
                    G4.Un_Load_Cassette_3 = 0;
                    G4.Un_Load_FaceValue_4 = 0;
                    G4.Un_Load_Cassette_4 = 0;

                    G4.Deposits_Notes_Denom_1 = 0;
                    G4.Deposits_Notes_Denom_2 = 0;
                    G4.Deposits_Notes_Denom_3 = 0;
                    G4.Deposits_Notes_Denom_4 = 0;

                    G4.LoadedAtRMCycle = WReconcCycleNo;

                    G4.ExcelDate = dateTimePicker1.Value; 

                    G4.InsertCIT_G4S_Repl_EntriesRecord(TempMode);

                    TotalNew = TotalNew + 1;
                }

                K++; // Read Next entry of the table 
            }


            //textBoxTotalNew.Text = TotalNew.ToString();

            if (TotalNew > 0)
            {
                MessageBox.Show("Total Records inserted in RRDM Repository = " + TotalNew.ToString() + Environment.NewLine
                                 + "Loading Cycle created is :" + WLoadingExcelCycle.ToString()
                                 );

              //  textBoxMsgBoard.Text = "RRDM Repository populated with Excel records.";

                ExcelInserted = true;

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.WFieldNumeric12 = 45; // Excel had populated RRDM Repository

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
            }

            button8.Hide();
            buttonInportExcel.Hide();
            buttonReverse.Show();
        }
// Reverse Entries
        private void buttonReverse_Click(object sender, EventArgs e)
        {
            // Dissable Reversal Entry 


            Cec.ReadExcelLoadCyclesBySeqNo(WLoadingExcelCycle);

            if (Cec.ProcessStage == 3)
            {
                MessageBox.Show(" This Loading Cycle cannot be reversed. "+ Environment.NewLine
                                 +"Updating was finalised.  ");
                return;
            }

            //Cec.IsReversed = true;

            Cec.UpdateLoadExcelCycle(Cec.SeqNo);

            TempMode = 1;
            G4.DeleteAndUpdateToReverseEntriesbyLoadingExcelCycleNo(Cec.SeqNo, WCitId);
            MessageBox.Show("Entries Has Been Reversed" + Environment.NewLine
                            + "Start Reloading Excel. "
                            );

            textBoxExcelName.Text = "";

            panel6.Hide();

            button8.Show();

            buttonReverse.Hide(); 

            buttonInportExcel.Show();

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.WFieldNumeric12 = 0; // Excel had populated RRDM Repository

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

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

    }
}

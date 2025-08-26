using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Configuration;
using Excel = Microsoft.Office.Interop.Excel;
//using Word = Microsoft.
using RRDM4ATMs;
using System.Runtime.InteropServices;
using System.Text;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form18_CIT_LoadExcel_KFH : Form
    {

        //Form65 NForm65;

        //int WRow;
        //string WAtmNo;

        int WExcelLoadCycle;

        DateTime FutureDate = new DateTime(2050, 11, 21);

        RRDMAtmsClass Ac = new RRDMAtmsClass();
  
        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles(); 

        //     FormMainScreen NFormMainScreen;
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        
        string WCitId; 

        public Form18_CIT_LoadExcel_KFH(string InSignedId, int SignRecordNo, string InOperator, string InCitId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            textBoxMsgBoard.Text = "Loading G4S Excel";

            labelStep1.Text = "Loading excel for CIT : " + WCitId; 

            //panel2.Hide();
        }

        private void Form108_Load(object sender, EventArgs e)
        {

            try
            {

              //  string Tfilter = "Operator = '" + WOperator + "' OR AtmNo = 'ModelPrive' ";

                //Ac.ReadAtmAndFillTableByOperator(WSignedId, WOperator);

                //ShowGridAtms(); 

            }
            catch (Exception ex)
            {

                RRDMLog4Net Log = new RRDMLog4Net();

                string WLogger = "RRDM4Atms";

                string WParameters = "";

                Log.CreateAndInsertRRDMLog4NetMessage(ex, WLogger, WParameters);

                MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");

                //Environment.Exit(0);

            }

        }

        // Filter incoming data based on selection criteria
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            //WAtmNo = rowSelected.Cells[1].Value.ToString();
            //ExcelBranch = rowSelected.Cells[1].Value.ToString();
            //ExcelBranchName = rowSelected.Cells[2].Value.ToString();
            //ExcelStreet = rowSelected.Cells[3].Value.ToString();
            //ExcelDistrict = rowSelected.Cells[4].Value.ToString();
            //ExcelCountry = rowSelected.Cells[5].Value.ToString();
            //ExcelModel = rowSelected.Cells[6].Value.ToString();
            ////WSeqNoLeft = (int)rowSelected.Cells[0].Value;
            ////ExcelAtmsReconcGroup = rowSelected.Cells[7].Value.ToString();
            //ExcelATMIPAddress = rowSelected.Cells[8].Value.ToString();
            //ExcelModelAtm = rowSelected.Cells[9].Value.ToString();

      
        }



        // Finish 
        private void button5_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridAtms()
        {
            // creating an excel 
            // http://csharp.net-informations.com/excel/csharp-create-excel.htm

            //Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            ////Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Admin\Desktop\Dropbox\Vandit's Folder\Internship\test.xlsx");
            //Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Panicos Michael\Desktop\REPORTS\ATMSView56-2.xlsx");
            //Excel.Worksheet worksheet = workbook.ActiveSheet;


            //int rcount = worksheet.UsedRange.Rows.Count;

            //int i = 0;

            ////Initializing Columns
            //dataGridView1.ColumnCount = worksheet.UsedRange.Columns.Count;
            ////for (int x = 0; x < dataGridView1.ColumnCount; x++)
            ////{
            ////    dataGridView1.Columns[x].Name = "Column " + x.ToString();
            ////}
            //dataGridView1.Columns[0].Name = worksheet.Cells[i + 1, 1].Value;
            //dataGridView1.Columns[1].Name = worksheet.Cells[i + 1, 2].Value;
            //dataGridView1.Columns[2].Name = worksheet.Cells[i + 1, 3].Value;
            //dataGridView1.Columns[3].Name = worksheet.Cells[i + 1, 4].Value;
            //dataGridView1.Columns[4].Name = worksheet.Cells[i + 1, 5].Value;
            //dataGridView1.Columns[5].Name = worksheet.Cells[i + 1, 6].Value;
            //dataGridView1.Columns[6].Name = worksheet.Cells[i + 1, 7].Value;
            //dataGridView1.Columns[7].Name = worksheet.Cells[i + 1, 8].Value;
            //dataGridView1.Columns[8].Name = worksheet.Cells[i + 1, 9].Value;
            //dataGridView1.Columns[9].Name = worksheet.Cells[i + 1, 10].Value;
            //dataGridView1.Columns[10].Name = worksheet.Cells[i + 1, 11].Value;
            //dataGridView1.Columns[11].Name = worksheet.Cells[i + 1, 12].Value;
            //dataGridView1.Columns[12].Name = worksheet.Cells[i + 1, 13].Value;
            //dataGridView1.Columns[13].Name = worksheet.Cells[i + 1, 14].Value;

            //i = 12;

            //for (; i < rcount; i++)
            //{
            //    dataGridView1.Rows.Add
            //        (worksheet.Cells[i + 1, 1].Value,
            //         worksheet.Cells[i + 1, 2].Value,
            //         worksheet.Cells[i + 1, 3].Value,
            //         worksheet.Cells[i + 1, 4].Value,
            //         worksheet.Cells[i + 1, 5].Value,
            //         worksheet.Cells[i + 1, 6].Value,
            //         worksheet.Cells[i + 1, 7].Value,
            //         worksheet.Cells[i + 1, 8].Value,
            //           worksheet.Cells[i + 1, 9].Value,
            //             worksheet.Cells[i + 1, 10].Value,
            //               worksheet.Cells[i + 1, 11].Value,
            //               worksheet.Cells[i + 1, 12].Value,
            //               worksheet.Cells[i + 1, 13].Value,
            //                 worksheet.Cells[i + 1, 14].Value
            //         );
            //}

            //if (dataGridView1.Rows.Count == 0)
            //{
            //    MessageBox.Show("No ATMs Available!");
            //    return;
            //}
            //else
            //{
            //    textBox12.Text = dataGridView1.Rows.Count.ToString();
            //}

            //dataGridView1.Columns[0].Width = 60; // AtmNo
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 60; //Branch
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            //dataGridView1.Columns[2].Width = 60; //  Type Of Author
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[3].Width = 140; // Openning Date
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
     
       
        //// Print 
        //private void buttonPrint_Click(object sender, EventArgs e)
        //{                                                                    

        //    string P1 = "Migrated ATMs Results ";

        //    string P2 = WExcelLoadCycle.ToString();
        //    string P3 = "Third Par";
        //    string P4 = Ac.BankId;
        //    string P5 = WSignedId;

        //    Form56R58ATMS ReportATMS58 = new Form56R58ATMS(P1, P2, P3, P4, P5);
        //    ReportATMS58.Show();
        //}
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
        // Inport Excel
        private void buttonInportExcel_Click(object sender, EventArgs e)
        {
            if  ( textBoxExcelName.Text == "")
            {
                MessageBox.Show("Please Browse for Excel");
                return; 
            }
            panel2.Show();
            
            Excel.Application oXL = new Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Admin\Desktop\Dropbox\Vandit's Folder\Internship\test.xlsx");
            Excel.Workbook workbook = oXL.Workbooks.Open(WExcelFileName);
            Excel.Worksheet worksheet = workbook.ActiveSheet;

            WDate = dateTimePicker1.Value.Date;

            string DateString = WDate.Day + "." + WDate.Month + "." + WDate.Year; // eg ("17.11.2017")

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

            try
            {

                int rcount = worksheet.UsedRange.Rows.Count;

                int i = 8;
                //
                //Initializing Columns NAMES
                //
                dataGridView1.ColumnCount = worksheet.UsedRange.Columns.Count;

                dataGridView1.Columns[0].Name = worksheet.Cells[i + 1, 1].Value;
                dataGridView1.Columns[1].Name = worksheet.Cells[i + 1, 2].Value;
                dataGridView1.Columns[2].Name = worksheet.Cells[i + 1, 3].Value;
                dataGridView1.Columns[3].Name = worksheet.Cells[i + 1, 4].Value;
                dataGridView1.Columns[4].Name = worksheet.Cells[i + 1, 5].Value;
                dataGridView1.Columns[5].Name = worksheet.Cells[i + 1, 6].Value;
                dataGridView1.Columns[6].Name = worksheet.Cells[i + 1, 7].Value;
                dataGridView1.Columns[7].Name = worksheet.Cells[i + 1, 8].Value;
                dataGridView1.Columns[8].Name = worksheet.Cells[i + 1, 9].Value;
                dataGridView1.Columns[9].Name = worksheet.Cells[i + 1, 10].Value;
                dataGridView1.Columns[10].Name = worksheet.Cells[i + 1, 11].Value;
                dataGridView1.Columns[11].Name = worksheet.Cells[i + 1, 12].Value;
                dataGridView1.Columns[12].Name = worksheet.Cells[i + 1, 13].Value;
                dataGridView1.Columns[13].Name = worksheet.Cells[i + 1, 14].Value;
                dataGridView1.Columns[14].Name = worksheet.Cells[i + 1, 15].Value;
                dataGridView1.Columns[15].Name = worksheet.Cells[i + 1, 16].Value;
                dataGridView1.Columns[16].Name = worksheet.Cells[i + 1, 17].Value;
                dataGridView1.Columns[17].Name = worksheet.Cells[i + 1, 18].Value;
                //dataGridView1.Columns[18].Name = worksheet.Cells[i + 1, 19].Value;
                //dataGridView1.Columns[19].Name = worksheet.Cells[i + 1, 20].Value;
                //dataGridView1.Columns[20].Name = worksheet.Cells[i + 1, 21].Value;

                // *******************************
                // GET EXCEL LINES AND FILL UP ROWS 
                // *******************************

                i = 11;

                for (; i < rcount; i++)
                {
                    //if (worksheet.Cells[i + 1, 1].Value =="")
                    //{
                    //    break; 
                    //}
                    dataGridView1.Rows.Add
                        (worksheet.Cells[i + 1, 1].Value,
                         worksheet.Cells[i + 1, 2].Value,
                         worksheet.Cells[i + 1, 3].Value,
                         worksheet.Cells[i + 1, 4].Value,
                         worksheet.Cells[i + 1, 5].Value,
                         worksheet.Cells[i + 1, 6].Value,
                         worksheet.Cells[i + 1, 7].Value,
                         worksheet.Cells[i + 1, 8].Value,
                         worksheet.Cells[i + 1, 9].Value,
                         worksheet.Cells[i + 1, 10].Value,
                         worksheet.Cells[i + 1, 11].Value,
                         worksheet.Cells[i + 1, 12].Value,
                         worksheet.Cells[i + 1, 13].Value,
                         worksheet.Cells[i + 1, 14].Value,
                         worksheet.Cells[i + 1, 15].Value,
                         worksheet.Cells[i + 1, 16].Value,
                         worksheet.Cells[i + 1, 17].Value,
                         worksheet.Cells[i + 1, 18].Value
                         //worksheet.Cells[i + 1, 19].Value,
                         //worksheet.Cells[i + 1, 20].Value,
                         //worksheet.Cells[i + 1, 21].Value
                         );
                    
                    if (i == 13) break; 
                }

                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No Entries Available in excel!");
                    return;
                }
                else
                {
                    textBox12.Text = dataGridView1.Rows.Count.ToString();
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
                //dataGridView1.Columns[2].HeaderText = "Description";

                dataGridView1.Columns[3].Width = 80; // Previous Balance 
                dataGridView1.Columns[3].DefaultCellStyle = style;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[3].HeaderText = "Previuous Balance";

                dataGridView1.Columns[4].Width = 80; // Dispensed 
                dataGridView1.Columns[4].DefaultCellStyle = style;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[4].HeaderText = "Dispensed ";

                dataGridView1.Columns[5].Width = 80; // Cash Unload 
                dataGridView1.Columns[5].DefaultCellStyle = style;
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[5].HeaderText = "Cash Unload";

                dataGridView1.Columns[6].Width = 80; // Cash Loaded
                dataGridView1.Columns[6].DefaultCellStyle = style;
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[6].HeaderText = "Cash Loaded";

                dataGridView1.Columns[7].Width = 80; // Deposits
                dataGridView1.Columns[7].DefaultCellStyle = style;
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[7].HeaderText = "Deposits";

                dataGridView1.Columns[8].Width = 100; // Balance KD
                dataGridView1.Columns[8].DefaultCellStyle = style;
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[8].HeaderText = "Balance KD";

                dataGridView1.Columns[9].Width = 70; // ATM Over
                dataGridView1.Columns[9].DefaultCellStyle = style;
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[9].HeaderText = "ATM Over";

                dataGridView1.Columns[10].Width = 70; // Atm Short
                dataGridView1.Columns[10].DefaultCellStyle = style;
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[10].HeaderText = "Atm Short";

                dataGridView1.Columns[11].Width = 70; // Remarks
                dataGridView1.Columns[11].DefaultCellStyle = style;
                dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[11].HeaderText = "Remarks";

                workbook.Close();

                oXL.Quit();

                Marshal.FinalReleaseComObject(oXL);
                Marshal.FinalReleaseComObject(workbook);
                Marshal.ReleaseComObject(worksheet);

            }
            catch (Exception ex)
            {

                workbook.Close();

                oXL.Quit();

                Marshal.FinalReleaseComObject(oXL);
                Marshal.FinalReleaseComObject(workbook);
                Marshal.ReleaseComObject(worksheet);

                CatchDetails(ex);

            }
            finally
            {

            }

            //Worksheet sheet = excelApp.Worksheets.Open(...);

        }
    
        //string ExcelAtmNo;
        //string ExcelBranch;
        //string ExcelBranchName;
        //string ExcelStreet;
        //string ExcelDistrict;
        //string ExcelCountry;
        //string ExcelModel;
        //string ExcelAtmsReconcGroup;
        //string ExcelATMIPAddress;
        //string ExcelModelAtm;

        int TotalUpdated;
        int TotalNew;

        bool UpdateMode;
        bool AddMode;
        bool ErrorInValidation; 

      

// Populate RRDM 
        bool ExcelInserted = false;
        DateTime WDate;
        bool ExcelChecked;
        int TempMode; 
        private void button8_Click(object sender, EventArgs e)
        {
            int SeqNo_0;
            string AtmNo_1;
            string Descr_2;
            decimal OpeningBalance_3;

            decimal Dispensed_4;
            decimal CountedCashUnload_5;
            decimal CashLoaded_6;
            decimal Deposits_7;

            decimal BalanceKD_8;
            decimal AtmOver_9;
            decimal AtmShort_10;

            string Remarks_11;

            //string TempSeq; 

            // Read Excel Line by Line 
            // Read Model ATM
            // Update the ones found 
            // Create new ones for the not found. 
            // Update Migration Session No 

            ExcelChecked = false; 

            TotalNew = 0;

            // Insert Excel Load Cycle 

            Cec.CitId = WCitId;
            //Cec.Cut_Off_Date = WDate; 
            Cec.StartDateTm = DateTime.Now;
            //Cec.ExcelId = WExcelFileName;
            Cec.ProcessStage = 1;
            Cec.UserId = WSignedId;
            Cec.Operator = WOperator;

            WExcelLoadCycle = Cec.InsertExcelLoadCycle();


            //Clear Table 
            //Tr.DeleteReport58(WSignedId);

            int K = 0;

            while (K <= (dataGridView1.Rows.Count - 1))
            {
                UpdateMode = false;
                AddMode = false;
                ErrorInValidation = false;

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
                    G4.OpeningBalance = OpeningBalance_3 = decimal.Parse(rowSelected.Cells[3].Value.ToString());

                    G4.Dispensed = Dispensed_4 = decimal.Parse(rowSelected.Cells[4].Value.ToString());
                  
                    G4.UnloadedCounted = CountedCashUnload_5 = decimal.Parse(rowSelected.Cells[5].Value.ToString());
                    G4.Cash_Loaded = CashLoaded_6 = decimal.Parse(rowSelected.Cells[6].Value.ToString());
                    G4.Deposits = Deposits_7 = decimal.Parse(rowSelected.Cells[7].Value.ToString());

                    BalanceKD_8 = decimal.Parse(rowSelected.Cells[8].Value.ToString());

                    G4.OverFound = AtmOver_9 = decimal.Parse(rowSelected.Cells[9].Value.ToString());
                    G4.ShortFound = AtmShort_10 = decimal.Parse(rowSelected.Cells[10].Value.ToString());

                    G4.RemarksG4S = Remarks_11 = (rowSelected.Cells[11].Value.ToString());

                    G4.CITId = WCitId;

                    G4.OriginFileName = WExcelFileName;

                    G4.UnloadedMachine = OpeningBalance_3 - Dispensed_4 ; 

                    G4.ReplDateG4S = WDate;

                    if (CashLoaded_6 > 0)
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
                    G4. LoadingExcelCycleNo = WExcelLoadCycle;

                    G4.ReplCycleNo = 0 ;

                    G4.OrderNo = 0;

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

                    G4.InsertCIT_G4S_Repl_EntriesRecord(TempMode);

                    TotalNew = TotalNew + 1; 
                }

                K++; // Read Next entry of the table 
            }

        
            //textBoxTotalNew.Text = TotalNew.ToString();

            if (TotalNew > 0)
            {
                MessageBox.Show("Total Records inserted in RRDM Repository = " + TotalNew.ToString() + Environment.NewLine
                                 + "Loading Cycle created is :" + WExcelLoadCycle.ToString()
                                 );
                        
                textBoxMsgBoard.Text = "RRDM Repository populated with Excel records.";

                ExcelInserted = true; 
            }

            button8.Hide();

            buttonReverse.Show(); 

        }
        // Go to next 
        private void button7_Click(object sender, EventArgs e)
        {
            if (ExcelInserted == false)
            {
                MessageBox.Show("Move Excel to RRDM repository before you move to next!");
                return;
            }

           // Form18_CIT_ValidateUpdateExcel_XXXX NForm18_CIT_Repl;

            int tempMode = 1; 
           // NForm18_CIT_Repl = new Form18_CIT_ValidateUpdateExcel_XXXX(WOperator, WSignedId, WCitId, WExcelLoadCycle, WDate, tempMode);

            //NForm18_CIT_Repl.ShowDialog(); ;
        }

        // Reverse Entries 
        private void buttonReverse_Click(object sender, EventArgs e)
        {
            // Dissable Reversal Entry 
         
           // Cec.ReadExcelLoadCyclesByCutOffDate(WDate);

            if (Cec.ProcessStage == 3)
            {
                MessageBox.Show(" This Loading Cycle cannot be reversed. Updating was finalised.  ");
                return; 
            }

           // Cec.IsReversed = true;

            Cec.UpdateLoadExcelCycle(Cec.SeqNo);

            

            TempMode = 1;
            G4.DeleteAndUpdateToReverseEntriesbyLoadingExcelCycleNo(Cec.SeqNo, WCitId);
            MessageBox.Show("Entries Has Been Reversed" + Environment.NewLine
                            + "Start Reloading Excel. "
                            );
            this.Dispose();
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

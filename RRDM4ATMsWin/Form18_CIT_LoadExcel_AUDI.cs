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
    public partial class Form18_CIT_LoadExcel_AUDI : Form
    {

        //Form65 NForm65;

        //int WRow;
        //string WAtmNo;

        int WExcelLoadCycle;

        DateTime FutureDate = new DateTime(2050, 11, 21);

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();
        RRDMReplOrdersClass Roc = new RRDMReplOrdersClass();

        //     FormMainScreen NFormMainScreen;
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCitId;

        public Form18_CIT_LoadExcel_AUDI(string InSignedId, int SignRecordNo, string InOperator, string InCitId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            textBoxMsgBoard.Text = "Loading G4S Excel";

            labelStep1.Text = "Loading excel AUDI for CIT : " + WCitId;

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
            if (textBoxExcelName.Text == "")
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
                Excel.Range range;
                range = worksheet.UsedRange;
                int Rows_Count = range.Rows.Count;
                int Columns_Count = range.Columns.Count;

                int i = 12;
                //
                //Initializing Columns NAMES
                //
                dataGridView1.ColumnCount = Columns_Count;

                dataGridView1.Columns[0].Name = worksheet.Cells[i + 1, 2].Value;
                dataGridView1.Columns[1].Name = worksheet.Cells[i + 1, 3].Value;
                dataGridView1.Columns[2].Name = worksheet.Cells[i + 1, 4].Value;
                dataGridView1.Columns[3].Name = worksheet.Cells[i + 1, 5].Value;
                dataGridView1.Columns[4].Name = worksheet.Cells[i + 1, 6].Value;
                dataGridView1.Columns[5].Name = worksheet.Cells[i + 1, 7].Value;
                dataGridView1.Columns[6].Name = worksheet.Cells[i + 1, 8].Value;
                dataGridView1.Columns[7].Name = worksheet.Cells[i + 1, 9].Value;
                dataGridView1.Columns[8].Name = worksheet.Cells[i + 1, 10].Value;
                dataGridView1.Columns[9].Name = worksheet.Cells[i + 1, 11].Value;
                dataGridView1.Columns[10].Name = worksheet.Cells[i + 1, 12].Value;
                dataGridView1.Columns[11].Name = worksheet.Cells[i + 1, 13].Value;
                dataGridView1.Columns[12].Name = worksheet.Cells[i + 1, 14].Value;
                dataGridView1.Columns[13].Name = worksheet.Cells[i + 1, 15].Value;
                dataGridView1.Columns[14].Name = worksheet.Cells[i + 1, 16].Value;
                dataGridView1.Columns[15].Name = worksheet.Cells[i + 1, 17].Value;
                dataGridView1.Columns[16].Name = worksheet.Cells[i + 1, 18].Value;
                dataGridView1.Columns[17].Name = worksheet.Cells[i + 1, 19].Value;
                dataGridView1.Columns[18].Name = worksheet.Cells[i + 1, 20].Value;
                //dataGridView1.Columns[19].Name = worksheet.Cells[i + 1, 20].Value;
                //dataGridView1.Columns[20].Name = worksheet.Cells[i + 1, 21].Value;
                //dataGridView1.Columns[21].Name = worksheet.Cells[i + 1, 22].Value;

                // *******************************
                // FILL UP ROWS 
                // *******************************

                i = 13;

                for (; i < Rows_Count; i++)
                {
                    if (worksheet.Cells[i + 1, 2].Value == null
                      || worksheet.Cells[i + 1, 2].Value.ToString() == ""
                      || worksheet.Cells[i + 1, 2].Value.ToString() == "00"
                      || worksheet.Cells[i + 1, 2].Value.ToString() == "0"
                      )
                    {
                        break;

                        //continue;
                    }

                    dataGridView1.Rows.Add
                        (worksheet.Cells[i + 1, 2].Value,
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
                         worksheet.Cells[i + 1, 18].Value,
                         worksheet.Cells[i + 1, 19].Value,
                         worksheet.Cells[i + 1, 20].Value
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
                    textBox12.Text = dataGridView1.Rows.Count.ToString();
                }

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";
                DataGridViewCellStyle style2 = new DataGridViewCellStyle();
                style2.Format = "N0";
                //  dgvmain.Columns["Amount"].DefaultCellStyle.Format = "#.##0";

                dataGridView1.Columns[0].Width = 70; // Order Number
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[0].HeaderText = "S/N"; 

                dataGridView1.Columns[1].Width = 70; //AtmNo
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[1].HeaderText = "AtmNo";

                dataGridView1.Columns[2].Width = 150; //Location
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //
                // Loaded 
                //

                dataGridView1.Columns[3].Width = 70; // Face 1
                dataGridView1.Columns[3].DefaultCellStyle = style2;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[4].Width = 70; // Face 2
                dataGridView1.Columns[4].DefaultCellStyle = style2;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[5].Width = 70; // Face 3
                dataGridView1.Columns[5].DefaultCellStyle = style2;
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[6].Width = 70; // Face 4
                dataGridView1.Columns[6].DefaultCellStyle = style2;
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[7].Width = 80; // Total 
                dataGridView1.Columns[7].DefaultCellStyle = style;
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[3].HeaderText = "Previuous Balance";
                //
                // UNLOADED
                //

                dataGridView1.Columns[8].Width = 70; // Face 1
                dataGridView1.Columns[8].DefaultCellStyle = style2;
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[4].HeaderText = "Dispensed ";

                dataGridView1.Columns[9].Width = 70; // Face 2
                dataGridView1.Columns[9].DefaultCellStyle = style2;
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[5].HeaderText = "Cash Unload";

                dataGridView1.Columns[10].Width = 70; // Face 3
                dataGridView1.Columns[10].DefaultCellStyle = style2;
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[6].HeaderText = "Cash Loaded";

                dataGridView1.Columns[11].Width = 70; // Face 4
                dataGridView1.Columns[11].DefaultCellStyle = style2;
                dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[7].HeaderText = "Deposits";

                dataGridView1.Columns[12].Width = 80; // Total
                dataGridView1.Columns[12].DefaultCellStyle = style;
                dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[8].HeaderText = "Balance KD";
                //
                // DEPOSITS
                //          
                dataGridView1.Columns[13].Width = 70; // Face 1
                dataGridView1.Columns[13].DefaultCellStyle = style2;
                dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[4].HeaderText = "Dispensed ";

                dataGridView1.Columns[14].Width = 70; // Face 2
                dataGridView1.Columns[14].DefaultCellStyle = style2;
                dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[5].HeaderText = "Cash Unload";

                dataGridView1.Columns[15].Width = 70; // Face 3
                dataGridView1.Columns[15].DefaultCellStyle = style2;
                dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[6].HeaderText = "Cash Loaded";

                dataGridView1.Columns[16].Width = 70; // Face 4
                dataGridView1.Columns[16].DefaultCellStyle = style2;
                dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGridView1.Columns[7].HeaderText = "Deposits";

                dataGridView1.Columns[17].Width = 80; // Total
                dataGridView1.Columns[17].DefaultCellStyle = style;
                dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[8].HeaderText = "Balance KD";

                workbook.Close();

                oXL.Quit();

                Marshal.FinalReleaseComObject(oXL);
                Marshal.FinalReleaseComObject(workbook);
                Marshal.ReleaseComObject(worksheet);

            }
            catch (Exception ex)
            {
                workbook.Close();
                CatchDetails(ex);

                oXL.Quit();

                Marshal.FinalReleaseComObject(oXL);
                Marshal.FinalReleaseComObject(workbook);
                Marshal.ReleaseComObject(worksheet);

            }
            finally
            {

            }

            //Worksheet sheet = excelApp.Worksheets.Open(...);

        }


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
        string SelectionCriteria;

        // Populate RRDM
        private void buttonPopulateRRDM_Click(object sender, EventArgs e)
        {

            int OrderNo_0;
            string AtmNo_1;
            string Location_2;

            int Load_Cassette_1_Pos3;
            int Load_Cassette_2_Pos4;
            int Load_Cassette_3_Pos5;
            int Load_Cassette_4_Pos6;
            decimal Cash_loaded_Pos7;

            int Un_Load_Cassette_1_Pos8;
            int Un_Load_Cassette_2_Pos9;
            int Un_Load_Cassette_3_Pos10;
            int Un_Load_Cassette_4_Pos11;
            decimal UnloadedCounted_Pos12;

            int Deposits_Notes_Denom_1_Pos13;
            int Deposits_Notes_Denom_2_Pos14;
            int Deposits_Notes_Denom_3_Pos15;
            int Deposits_Notes_Denom_4_Pos16;
            decimal Deposits_Pos17;

            //string TempSeq; 

            // Read Excel Line by Line 
            // Read Model ATM
            // Update the ones found 
            // Create new ones for the not found. 
            // Update Migration Session No 

            try
            {
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
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No Entries to show");
                    return;
                }

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

                        G4.SeqNo = OrderNo_0 = int.Parse(rowSelected.Cells[0].Value.ToString()); // Order No

                        SelectionCriteria = " WHERE OrderNo =" + OrderNo_0;
                        G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, 1);
                        if (G4.RecordFound == true)
                        {
                            UpdateMode = true;
                        }
                        else
                        {
                            UpdateMode = false;
                        }

                        G4.AtmNo = AtmNo_1 = rowSelected.Cells[1].Value.ToString(); // AtmNo
                        if (G4.AtmNo == "" || G4.AtmNo == "00" || G4.AtmNo == "0")
                        {
                            break;
                        }
                        G4.AtmName = Location_2 = rowSelected.Cells[2].Value.ToString(); // Location

                        // Loaded
                        // 3
                        if (rowSelected.Cells[3].Value == null
                          || rowSelected.Cells[3].Value.ToString() == "")
                        {
                            G4.Load_Cassette_1 = Load_Cassette_1_Pos3 = 0;
                        }
                        else
                        {
                            G4.Load_Cassette_1 = Load_Cassette_1_Pos3 = int.Parse(rowSelected.Cells[3].Value.ToString());
                        }
                        // 4
                        if (rowSelected.Cells[4].Value == null
                                  || rowSelected.Cells[4].Value.ToString() == "")
                        {
                            G4.Load_Cassette_2 = Load_Cassette_2_Pos4 = 0;
                        }
                        else
                        {
                            G4.Load_Cassette_2 = Load_Cassette_2_Pos4 = int.Parse(rowSelected.Cells[4].Value.ToString());
                        }
                        // 5
                        if (rowSelected.Cells[5].Value == null
                                  || rowSelected.Cells[5].Value.ToString() == "")
                        {
                            G4.Load_Cassette_3 = Load_Cassette_3_Pos5 = 0;
                        }
                        else
                        {
                            G4.Load_Cassette_3 = Load_Cassette_3_Pos5 = int.Parse(rowSelected.Cells[5].Value.ToString());
                        }
                        // 6
                        if (rowSelected.Cells[6].Value == null
                                  || rowSelected.Cells[6].Value.ToString() == "")
                        {
                            G4.Load_Cassette_4 = Load_Cassette_4_Pos6 = 0;
                        }
                        else
                        {
                            G4.Load_Cassette_4 = Load_Cassette_4_Pos6 = int.Parse(rowSelected.Cells[6].Value.ToString());
                        }
                        // 7
                        if (rowSelected.Cells[7].Value == null
                                  || rowSelected.Cells[7].Value.ToString() == "")
                        {
                            G4.Cash_Loaded = Cash_loaded_Pos7 = 0;
                        }
                        else
                        {
                            G4.Cash_Loaded = Cash_loaded_Pos7 = decimal.Parse(rowSelected.Cells[7].Value.ToString());
                        }

                        if (UpdateMode == true & G4.Cash_Loaded > 0)
                        {
                            // Update Loaded

                        }
                        // UnLoaded
                        // 8
                        if (rowSelected.Cells[8].Value == null
                                || rowSelected.Cells[8].Value.ToString() == "")
                        {
                            G4.Un_Load_Cassette_1 = Un_Load_Cassette_1_Pos8 = 0;
                        }
                        else
                        {
                            G4.Un_Load_Cassette_1 = Un_Load_Cassette_1_Pos8 = int.Parse(rowSelected.Cells[8].Value.ToString());
                        }
                   
                        // 9
                        if (rowSelected.Cells[9].Value == null
                                || rowSelected.Cells[9].Value.ToString() == "")
                        {
                            G4.Un_Load_Cassette_2 = Un_Load_Cassette_2_Pos9 = 0;
                        }
                        else
                        {
                            G4.Un_Load_Cassette_2 = Un_Load_Cassette_2_Pos9 = int.Parse(rowSelected.Cells[9].Value.ToString());
                        }
                       
                        // 10
                        if (rowSelected.Cells[10].Value == null
                                || rowSelected.Cells[10].Value.ToString() == "")
                        {
                            G4.Un_Load_Cassette_3 = Un_Load_Cassette_3_Pos10 = 0;
                        }
                        else
                        {
                            G4.Un_Load_Cassette_3 = Un_Load_Cassette_3_Pos10 = int.Parse(rowSelected.Cells[10].Value.ToString());
                        }
                      
                        // 11
                        if (rowSelected.Cells[11].Value == null
                                || rowSelected.Cells[11].Value.ToString() == "")
                        {
                            G4.Un_Load_Cassette_4 = Un_Load_Cassette_4_Pos11 = 0;
                        }
                        else
                        {
                            G4.Un_Load_Cassette_4 = Un_Load_Cassette_4_Pos11 = int.Parse(rowSelected.Cells[11].Value.ToString());
                        }
                       
                        // 12
                        if (rowSelected.Cells[12].Value == null
                                || rowSelected.Cells[12].Value.ToString() == "")
                        {
                            G4.UnloadedCounted = UnloadedCounted_Pos12 = 0;
                        }
                        else
                        {
                            G4.UnloadedCounted = UnloadedCounted_Pos12 = decimal.Parse(rowSelected.Cells[12].Value.ToString());
                        }
                      
                        if (UpdateMode == true & G4.UnloadedCounted > 0)
                        {
                            // Update Unloaded
                        }
                        // Deposits 
                        // 13
                        if (rowSelected.Cells[13].Value == null
                                || rowSelected.Cells[13].Value.ToString() == "")
                        {
                            G4.Deposits_Notes_Denom_1 = Deposits_Notes_Denom_1_Pos13 = 0;
                        }
                        else
                        {
                            G4.Deposits_Notes_Denom_1 = Deposits_Notes_Denom_1_Pos13 = int.Parse(rowSelected.Cells[13].Value.ToString());
                        }
                       
                        // 14
                        if (rowSelected.Cells[14].Value == null
                                || rowSelected.Cells[14].Value.ToString() == "")
                        {
                            G4.Deposits_Notes_Denom_2 = Deposits_Notes_Denom_2_Pos14 = 0;
                        }
                        else
                        {
                            G4.Deposits_Notes_Denom_2 = Deposits_Notes_Denom_2_Pos14 = int.Parse(rowSelected.Cells[14].Value.ToString());
                        }
                        
                        // 15
                        if (rowSelected.Cells[15].Value == null
                                || rowSelected.Cells[15].Value.ToString() == "")
                        {
                            G4.Deposits_Notes_Denom_3 = Deposits_Notes_Denom_3_Pos15 = 0;
                        }
                        else
                        {
                            G4.Deposits_Notes_Denom_3 = Deposits_Notes_Denom_3_Pos15 = int.Parse(rowSelected.Cells[15].Value.ToString());
                        }
                       
                        // 16
                        if (rowSelected.Cells[16].Value == null
                                || rowSelected.Cells[16].Value.ToString() == "")
                        {
                            G4.Deposits_Notes_Denom_4 = Deposits_Notes_Denom_4_Pos16 = 0;
                        }
                        else
                        {
                            G4.Deposits_Notes_Denom_4 = Deposits_Notes_Denom_4_Pos16 = int.Parse(rowSelected.Cells[16].Value.ToString());
                        }
                       
                        // 17
                        if (rowSelected.Cells[17].Value == null
                                || rowSelected.Cells[17].Value.ToString() == "")
                        {
                            G4.Deposits = Deposits_Pos17 = 0;
                        }
                        else
                        {
                            G4.Deposits = Deposits_Pos17 = decimal.Parse(rowSelected.Cells[17].Value.ToString());
                        }
                       
                        if (UpdateMode == true & G4.Deposits > 0)
                        {
                            // Update Deposits 
                        }

                        // UnLoaded
                        //if (G4.AtmName == "CDM")
                        //{
                        //    break;
                        //}
                        G4.Cut_Off_date = DateTime.Now;
                        G4.Gl_Balance_At_CutOff = 0;

                        G4.OrderNo = OrderNo_0;
                        G4.ReplDateG4S = DateTime.Now;

                        G4.OpeningBalance = 0;
                        G4.Dispensed = 0;


                        //  G4.Cash_Loaded = CashLoaded_10 = decimal.Parse(rowSelected.Cells[10].Value.ToString());
                        //   G4.Deposits = Deposits_11 = decimal.Parse(rowSelected.Cells[11].Value.ToString());

                        //  BalanceKD_12 = decimal.Parse(rowSelected.Cells[12].Value.ToString());

                        G4.OverFound = 0;
                        G4.ShortFound = 0;

                        G4.RemarksG4S = "N/A";

                        G4.CITId = WCitId;

                        G4.OriginFileName = WExcelFileName;

                        G4.UnloadedMachine = 0;

                        //G4.ReplDateG4S = WDate;

                        if (Deposits_Pos17 > 0)
                        {
                            G4.IsDeposit = true;
                        }
                        else G4.IsDeposit = false;

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
                            SelectionCriteria = " WHERE OriginFileName ='" + WExcelFileName + "'";
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
                        G4.LoadingExcelCycleNo = WExcelLoadCycle;

                        G4.ReplCycleNo = 0;

                        G4.PresentedErrors = 0;

                        // FOUND FROM EXCEL HEADINGS
                        G4.Load_FaceValue_1 = 20;
                        G4.Load_FaceValue_1 = 50;
                        G4.Load_FaceValue_1 = 100;
                        G4.Load_FaceValue_1 = 200;

                        G4.Un_Load_FaceValue_1 = 20;
                        G4.Un_Load_FaceValue_1 = 50;
                        G4.Un_Load_FaceValue_1 = 100;
                        G4.Un_Load_FaceValue_1 = 200;

                        //
                        // Insert Record in Excel Like in Report 
                        //
                        if (UpdateMode == false)
                        {
                            G4.InsertCIT_G4S_Repl_EntriesRecord(TempMode);
                        }

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

                buttonPopulateRRDM.Hide();

                buttonReverse.Show();

            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }


        }
        // Go to next 
        private void button7_Click(object sender, EventArgs e)
        {
            if (ExcelInserted == false)
            {
                MessageBox.Show("Move Excel to RRDM repository before you move to next!");
                return;
            }

            Form18_CIT_ValidateUpdateExcel_AUDI NForm18_CIT_Repl_AUDI;

            int tempMode = 1;
            NForm18_CIT_Repl_AUDI = new Form18_CIT_ValidateUpdateExcel_AUDI(WOperator, WSignedId, WCitId, WExcelLoadCycle, WDate, tempMode);

            NForm18_CIT_Repl_AUDI.ShowDialog();
        }

        // Reverse Entries 
        private void buttonReverse_Click(object sender, EventArgs e)
        {
            // Dissable Reversal Entry 

            //Cec.ReadExcelLoadCyclesByCutOffDate(WDate);

            //if (Cec.ProcessStage == 3)
            //{
            //    MessageBox.Show(" This Loading Cycle cannot be reversed. Updating was finalised.  ");
            //    return;
            //}

            //Cec.IsReversed = true;

            //Cec.UpdateLoadExcelCycle(Cec.SeqNo);

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

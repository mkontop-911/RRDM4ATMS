using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;

using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form18_CIT_ViewBankRecords : Form
    {
     
        //Form31 NForm31;

        //DateTime FromDate;
        //DateTime ToDate;

        string SelectionCriteria;

    //    bool Presented = false; 

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

        //RRDM_CitExcelCycles Cec = new RRDM_CitExcelCycles();

        //RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //TEST
        DateTime WorkingToday = new DateTime(2017, 09, 01);

        DateTime WDateFrom;
        DateTime WDateTo;

        int TempMode; 

        string WOperator;
        string WSignedId;

        string WCitId;

        DateTime WDate;

        public Form18_CIT_ViewBankRecords(string InOperator, string InSignedId, string InCitId, DateTime InWorkingDate)
        {
            WSignedId = InSignedId;
            //WSignRecordNo = SignRecordNo;
            //WSecLevel = InSecLevel;
            WOperator = InOperator;

            WCitId = InCitId;

            WDate = InWorkingDate;

            // 1 Excel Loaded
            // 2 Excel Validated
            // 3 Excel Updated

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = WSignedId;

            dateTimePickerFrom.Value = WDate.Date;
            // Testing Value 
            dateTimePickerFrom.Value = WorkingToday.Date;
            
            dateTimePickerTo.Value = WDate.Date;

            label1.Text = "SELECTION FOR CIT : " + WCitId ;

            textBoxMsgBoard.Text = "View Banks Records. Make Selection.";
            TempMode = 2 ;
            SelectionCriteria = " ORDER BY AtmNo, SeqNo ";

        }

        // Load 
        private void Form18_Load(object sender, EventArgs e)
        {
            WDateFrom = dateTimePickerFrom.Value.Date;
            WDateTo = dateTimePickerTo.Value.Date;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTableAND_TotalsByDateRange(WOperator, WSignedId, "", WDateFrom, WDateTo, SelectionCriteria,TempMode );

            textBoxTotalNotProcessed.Text = G4.TotalNotProcessed.ToString();
            textBoxTotal11.Text = G4.Total11.ToString() ;
            textBoxTotalAA.Text = G4.TotalAA.ToString();
            textBoxTotal01.Text = G4.Total01.ToString();
         
            textBoxShort.Text = G4.TotalShort.ToString();
            textBoxPresenter.Text = G4.TotalPresenter.ToString(); 

            ShowGrid1();

        }
        //
        // ROW ENTER ON USER 
        //
        bool RequestFromMatched;
        int RowSeqNo;
        string WAtmNo;
        int WSesNo; 
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            RowSeqNo = (int)rowSelected.Cells[0].Value;

            // Read Entries for Bank 
            TempMode = 2;
            G4.ReadCIT_G4S_Repl_EntriesBySeqNo(RowSeqNo, TempMode);
            WAtmNo = G4.AtmNo;
            WSesNo = G4.ReplCycleNo; 
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
            dataGridView1.DataSource = G4.DataTableG4SEntries.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No entries Available.");
                this.Dispose(); 
                //return;
            }
            else
            {
                textBoxTotalEnries.Text = dataGridView1.Rows.Count.ToString();
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 40; // SeqNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[0].Visible = false;

                dataGridView1.Columns[1].Width = 60; // CITId
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].Visible = true;

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

                dataGridView1.Columns[6].Width = 100; //ReplDateG4S
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[6].Visible = true;

                dataGridView1.Columns[7].Width = 50; // OrderId 
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[7].Visible = false;

                dataGridView1.Columns[8].Width = 0; // CreatedDate
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

                dataGridView1.Columns[14].Width = 70; // Cash_Loaded
                dataGridView1.Columns[14].DefaultCellStyle = style;
                dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[14].HeaderText = "Cash Loaded";

                dataGridView1.Columns[15].Width = 60; // Deposits
                dataGridView1.Columns[15].DefaultCellStyle = style;
                dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[16].Width = 60; //OverFound
                dataGridView1.Columns[16].DefaultCellStyle = style;
                dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[16].HeaderText = "Over Found";

                dataGridView1.Columns[17].Width = 60; // ShortFound
                dataGridView1.Columns[17].DefaultCellStyle = style;
                dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[17].HeaderText = "Short Found";

                dataGridView1.Columns[18].Width = 50; // RemarksG4S
                dataGridView1.Columns[18].DefaultCellStyle = style;
                dataGridView1.Columns[18].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[18].HeaderText = "Remarks";

                dataGridView1.Columns[19].Width = 60; // PresentedErrors
                dataGridView1.Columns[19].DefaultCellStyle = style;
                dataGridView1.Columns[19].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[19].HeaderText = "Presented Errors";

            }

        }


        // Finish 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // Print What you VIEW 
        private void buttonP11_Click(object sender, EventArgs e)
        {
            string P1 = label1.Text; 

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_Bank ReportATMSReplCycles = new Form56R69ATMS_Bank(P1, P2, P3, P4, P5);
            ReportATMSReplCycles.Show();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            label1.Text = "SELECTION FOR MATCHED";
            TempMode = 2;
            SelectionCriteria = " AND Mask = '11' ";
            Form18_Load(this, new EventArgs());    
        }

        private void buttonAA_Click_1(object sender, EventArgs e)
        {
            label1.Text = "SELECTION FOR MATCHED BUT DIFFERENT VALUES";

            TempMode = 2;
            SelectionCriteria = " AND Mask = 'AA' ";

            RequestFromMatched = true;
          //  Presented = false;

            Form18_Load(this, new EventArgs()); 
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            label1.Text = "SELECTION FOR Not Processed ";
            TempMode = 2;
            SelectionCriteria = " AND Mask = '' ";

            Form18_Load(this, new EventArgs());
        }

        private void button01_Click_1(object sender, EventArgs e)
        {
            label1.Text = "PRESENT IN BANK BUT MISSING IN G4S ";
            TempMode = 2;
            SelectionCriteria = " AND Mask = '10' ";

            Form18_Load(this, new EventArgs());        
        }

        // Short 
        private void button1_Click(object sender, EventArgs e)
        {
            
            label1.Text = "CASES OF SHORT NOTES FOUND ";

            TempMode = 2; // From File 2  
            SelectionCriteria = " AND ShortFound <> 0 ";
          
            Form18_Load(this, new EventArgs());
        }

        // Presenter 

        private void buttonPresenterNotEqual_Click(object sender, EventArgs e)
        {
            label1.Text = "Presented Errors  ";

            TempMode = 2; // From File 2  
            SelectionCriteria = " AND PresentedErrors <> 0 ";
            
            Form18_Load(this, new EventArgs());
        }
// VIEW BY DATE 
        private void button2_Click(object sender, EventArgs e)
        {
            SelectionCriteria = ""; 
            Form18_Load(this, new EventArgs());
        }
        // Create Excel 

        DateTime Last_Cut_Off_Date;

        private void buttonCreateExcel_Click(object sender, EventArgs e)
        {
            // Read Table Entries for Bank 
            TempMode = 2;
            SelectionCriteria = "  Order by CitId, AtmNo Asc, SeqNo Desc  ";
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTableAND_TotalsByDateRange(WOperator, WSignedId, WCitId, 
                WDateFrom, WDateTo, SelectionCriteria, TempMode);

            if (G4.DataTableG4SEntriesSelectedForExcel.Rows.Count == 0)
            {
                MessageBox.Show("No available Entries to create excel! ");
                return; 
            }

            // There are entries to create Excel      

                Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }


            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            //
            // FILL IN EXCEL LINES 
            //
            int I = 1;
          
            try
            {
               
                //S/N	Atm No	ATM Name	Last CutOff	GL Balance At Cut Off	Order	Repl Date	
                //Openning Balance	Withdrawls	Unloaded	Money Loaded	Deposits	Balance KD	Over Found	Short Found	Remarks 

                xlWorkSheet.Cells[I, 1] = "S/N";
                xlWorkSheet.Cells[I, 2] = "Atm No";
                xlWorkSheet.Cells[I, 3] = "ATM Name";
                xlWorkSheet.Cells[I, 4] = "Last CutOff";

                xlWorkSheet.Cells[I, 5] = "GL Balance At Cut Off";
                xlWorkSheet.Cells[I, 6] = "Order";
                xlWorkSheet.Cells[I, 7] = "Repl Date";
                xlWorkSheet.Cells[I, 8] = "Openning Balance";

                xlWorkSheet.Cells[I, 9] = "Withdrawls";
                xlWorkSheet.Cells[I, 10] = "Unloaded";
                xlWorkSheet.Cells[I, 11] = "Money Loaded";
                xlWorkSheet.Cells[I, 12] = "Deposits";

                xlWorkSheet.Cells[I, 13] = "Balance KD";
                xlWorkSheet.Cells[I, 14] = "Over Found";
                xlWorkSheet.Cells[I, 15] = "Short Found";
                xlWorkSheet.Cells[I, 16] = "Remarks";

                int L = 0;

                int K = 2;

                int WSeqNo; 

                while (L <= (G4.DataTableG4SEntriesSelectedForExcel.Rows.Count - 1))
                {

                    WSeqNo = (int)G4.DataTableG4SEntriesSelectedForExcel.Rows[L]["SeqNo"];
                    string WAtmNo = (string)G4.DataTableG4SEntriesSelectedForExcel.Rows[L]["AtmNo"];
                    Last_Cut_Off_Date = (DateTime)G4.DataTableG4SEntriesSelectedForExcel.Rows[L]["Last_Cut_Off_Date"];

                    TempMode = 2;
                    G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, TempMode);

                    // Find GL Amt

                
                        xlWorkSheet.Cells[K, 1] = K - 1; //  "S/N";
                                                         //xlWorkSheet.Cells[K, 1].HorizontalAlignment = ExlLeft;
                        xlWorkSheet.Cells[K, 2] = G4.AtmNo; //  "Atm No"
                        xlWorkSheet.Cells[K, 3] = G4.AtmName; // "ATM Name"

                        xlWorkSheet.Cells[K, 4] = Last_Cut_Off_Date.Date; // "Last CutOff"

                        xlWorkSheet.Cells[K, 5].NumberFormat = "#,##0.00";
                        xlWorkSheet.Cells[K, 5] = ReturnGl_Balance(G4.AtmNo, G4.ReplDateG4S.Date); // "GL Balance At Cut Off"
                        xlWorkSheet.Cells[K, 6] = "123"; // "Order"
                        xlWorkSheet.Cells[K, 7] = G4.ReplDateG4S.Date;// "Repl Date"
                        xlWorkSheet.Cells[K, 8].NumberFormat = "#,##0.00";
                        xlWorkSheet.Cells[K, 8] = G4.OpeningBalance; //"Openning Balance"

                        xlWorkSheet.Cells[K, 9].NumberFormat = "#,##0.00";
                        xlWorkSheet.Cells[K, 9] = G4.Dispensed; //  "Withdrawls"
                        xlWorkSheet.Cells[K, 10].NumberFormat = "#,##0.00";
                       if (G4.UnloadedCounted > 0 )
                       {
                        xlWorkSheet.Cells[K, 10] = G4.UnloadedCounted; // "Unloaded counted"
                       }
                       else
                       {
                        xlWorkSheet.Cells[K, 10] = G4.UnloadedMachine; // "Unloaded"
                       }       
                        xlWorkSheet.Cells[K, 11].NumberFormat = "#,##0.00";
                        xlWorkSheet.Cells[K, 11] = G4.Cash_Loaded; // "Money Loaded"
                        xlWorkSheet.Cells[K, 12].NumberFormat = "#,##0.00";
                        xlWorkSheet.Cells[K, 12] = G4.Deposits; // "Deposits"

                        xlWorkSheet.Cells[K, 13].NumberFormat = "#,##0.00";
                        xlWorkSheet.Cells[K, 13] = "0"; //  "Balance KD"
                        xlWorkSheet.Cells[K, 14].NumberFormat = "#,##0.00";
                 
                        xlWorkSheet.Cells[K, 14] = G4.OverFound; // "Over Found"
                        xlWorkSheet.Cells[K, 15].NumberFormat = "#,##0.00";
                   
                        xlWorkSheet.Cells[K, 15] = G4.ShortFound; // "Short Found"
                        xlWorkSheet.Cells[K, 16].NumberFormat = "#,##0.00";
                        xlWorkSheet.Cells[K, 16] = "0"; // "Remarks"   

                        K++;
                    //}
                   

                    L++;          

                }
         
                DateTime WDate = DateTime.Now;

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

                int Id1 = DateTime.Now.Hour;
                int Id2 = DateTime.Now.Minute;
                string Id3 = Id1.ToString() +"_"+ Id2.ToString(); 

                // THIS is an XLS version 
                //xlWorkBook.SaveAs("d:\\NBG_GL_"+ Id3 + " "+ DateString + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue,
                //                           misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue,
                //                           misValue, misValue, misValue, misValue);

                // This is a xlsx version

                string strFullFilePathNoExt = "C:\\EXCELS_For_GL\\NBG_GL_" + Id3 + " " + DateString + ".xlsx"; 
                xlWorkBook.SaveAs(strFullFilePathNoExt, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue,
                misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                Excel.XlSaveConflictResolution.xlUserResolution, true,
                misValue, misValue, misValue);


                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

                MessageBox.Show("Excel file created , you can find the file in ..." + Environment.NewLine
                    +"...C:\\EXCELS_For_GL\\NBG_GL_" + Id3+" "+ DateString+ ".xlsx"
                    );
            }
            catch (Exception ex)
            {
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

                CatchDetails(ex);
            }
        }
        decimal WGlBal; 
        private decimal ReturnGl_Balance(string InAtmNo, DateTime InReplDate)
        {
            WGlBal = 0;
           
            //if (InAtmNo == "NB0535C1" & InReplDate == new DateTime(2018, 02, 22))
            //{
            //    WGlBal = 103480 ;
            //}
            //if (InAtmNo == "NB0553C1" & InReplDate == new DateTime(2018, 02, 22))
            //{
            //    WGlBal = 102610;
            //}
            //if (InAtmNo == "NB0521C1" & InReplDate == new DateTime(2018, 02, 23))
            //{
            //    WGlBal = 101670;
            //}
            //if (InAtmNo == "NB0528C1" & InReplDate == new DateTime(2018, 02, 28))
            //{
            //    WGlBal = 101800;
            //}
            //if (InAtmNo == "NB0531C1" & InReplDate == new DateTime(2018, 02, 28))
            //{
            //    WGlBal = 101010;
            //}
            //if (InAtmNo == "NB0545C1" & InReplDate == new DateTime(2018, 02, 28))
            //{
            //    WGlBal = 100580;
            //}

            return WGlBal; 
        }

        // Catch Details
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("UserId : ");
            WParameters.Append("");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATM NO : ");
            WParameters.Append("");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

            // Environment.Exit(0);
        }
// Show SM LINES 
        private void buttonShowSM_Click(object sender, EventArgs e)
        {

            int WTraceNo; 

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            WTraceNo = Ta.LastTraceNo;
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}


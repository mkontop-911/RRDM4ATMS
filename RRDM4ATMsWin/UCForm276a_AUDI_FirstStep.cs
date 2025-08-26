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
    public partial class UCForm276a_AUDI_FirstStep : UserControl
    {
        public string guidanceMsg;

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        DateTime WDate;
      
        int TempMode;

        int WReconcCycleNo;

        bool ViewCompletedWorkFlow;
        bool ViewNotCompletedWorkFlow;


        DateTime FutureDate = new DateTime(2050, 11, 21);

        DateTime WDtFrom = new DateTime(2021, 08, 20);

        DateTime WDtTo = new DateTime(2021, 11, 21);
        
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

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

        public void UCForm276a_AUDI_FirstStep_Par(string InSignedId, int SignRecordNo, string InOperator,
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
            // 2: Loaded from Excel
            // 4: Valid At Maker (Ready for transactions creation) 
            // 6: Invalid At Maker - Not Ready for transactions creation
            //************************************************************


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


               // buttonAdd.Enabled = false;
                buttonUpdate.Enabled = false;
                buttonDelete.Enabled = false;

                // checkBoxSameAsPrevious.Enabled = false; 

            }
            else
            {
                //  NormalProcess = true;
            }

            if (WAuthoriser == true)
            {
                //panel2.Location.X.Equals = 9; 
            }

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewCompletedWorkFlow = true;
            }

            if (Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewNotCompletedWorkFlow = true;
            }

            //ViewNotCompletedWorkFlow

        }

        // SHOW SCREEN 
        // ON LOAD 
        public void SetScreen()
        {


            // SHOW GRID
            //public int Repl_Load_Status;// 2: Loaded from Excel
            //                            // 4: Valid At Maker (Ready for transactions creation) 
            //                            // 6: Invalid At Maker - Not Ready for transactions creation
            //                            // 8: Completed Transactions are made
            string SelectionCriteria = ""; 
        if (ViewCompletedWorkFlow == true)
            {
                SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode_Load = 2  AND LoadingExcelCycleNo=" + WLoadingExcelCycle ;

            }
            else
            {
                SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND (Repl_Load_Status = 2 OR Repl_Load_Status = 4 OR Repl_Load_Status = 6)"
               + " AND Cash_Loaded > 0 AND ProcessMode_Load <> 2 ";

            }


            TempMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable_Feeding_WithMatchStatus(WOperator, WSignedId, SelectionCriteria, TempMode);
            if (G4.DataTableG4SEntries.Rows.Count > 0)
            {
               // labelHeader.Text = "EXCEL ENTRIES";

                ShowGrid1();

                textBoxTotalValid.Text = G4.WTotalValidEntries.ToString();
                textBoxTotalInvalid.Text = G4.WTotalInvalidEntries.ToString();

                textBoxValidAmt.Text = G4.WTotalValidAmt.ToString("#,##0.00"); 
                textBoxInvalidAmt.Text = G4.WTotalInvalidAmt.ToString("#,##0.00");

            }
            else
            {
               // Cec.ReadExcelLoadCyclesBySeqNo(WLoadingExcelCycle);
               //// textBoxExcelName.Text = Cec.ExcelId;
               // HashTotalOver1000 = Cec.LoadedTotal;

                //label2.Hide();
                //labelHeader.Hide();
                //panel1.Hide();
                

            }


        }

        // Row Enter
        int WSeqNo;
        int WMode;

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;
            WMode = 1;
            G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, WMode);

            textBoxAtm.Text = G4.AtmNo;
            textBoxATM_Location.Text = G4.AtmName;

            textBoxCITAmount.Text = G4.Cash_Loaded.ToString("#,##0.00");

            textBox3.Text = G4.SM_Loaded.ToString("#,##0.00");

            textBox4.Text = (G4.Cash_Loaded-G4.SM_Loaded).ToString("#,##0.00");


            textBoxReplCycle.Text = G4.ReplCycleNo.ToString();

            //WSeqNo = WLoadingExcelCycle;

            textBoxExcelDate.Text = G4.ExcelDate.ToShortDateString();

            //
            // READ RECORD 
            //

            // SHOW REPLENISHMENT CYCLES
            int NumberOfCycles = 10;
            Ta.ReadReplCycles_Last_Numberof_Cycles(G4.AtmNo, NumberOfCycles);

            ShowGrid_2();

        }
       
        int WWSeqNo;
        // ROW ENTER 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WWSeqNo = (int)rowSelected.Cells[0].Value;
            textBoxReplNo.Text = WWSeqNo.ToString(); 

           // Ta.ReadSessionsStatusTraces(WAtmNo, WWSeqNo); 
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
                   // textBoxExcelName.Text = WExcelFileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        decimal HashTotalOver1000; 

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGrid1()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();

            dataGridView1.DataSource = G4.DataTableG4SEntries.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }
            //// DATA TABLE ROWS DEFINITION 
            //DataTableG4SEntries.Columns.Add("SeqNo", typeof(int));
            //DataTableG4SEntries.Columns.Add("Matched", typeof(bool));
            //DataTableG4SEntries.Columns.Add("Repl_Load_Status", typeof(int));

            //DataTableG4SEntries.Columns.Add("AtmNo", typeof(string));
            //DataTableG4SEntries.Columns.Add("FeededCash", typeof(decimal));
            //DataTableG4SEntries.Columns.Add("CIT_Unloaded", typeof(decimal));

            //DataTableG4SEntries.Columns.Add("Comment", typeof(string));

            //DataTableG4SEntries.Columns.Add("ExcelDay", typeof(DateTime));

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = true;

            dataGridView1.Columns[1].Width = 60; // Matched
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 50; // Repl_Load_Status
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 
            dataGridView1.Columns[2].HeaderText = "Status";

            dataGridView1.Columns[3].Width = 60; // ATM no 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 80; // FeedingCash
            dataGridView1.Columns[4].DefaultCellStyle = style;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[5].Width = 140; // Comment
            dataGridView1.Columns[5].DefaultCellStyle = style;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 80; // CIT REPL DATE 

            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView1.Columns[7].Width = 80; // Excel Date 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            

            dataGridView1.Columns[8].Width = 60; //Denom_1  
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].Visible = true;
            dataGridView1.Columns[8].HeaderText = "Denom_1";

            dataGridView1.Columns[9].Width = 60; // Cass_1 
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[9].Visible = true;
            dataGridView1.Columns[9].HeaderText = "Cass_1";

            dataGridView1.Columns[10].Width = 60; //Denom_2  
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[10].Visible = true;
            dataGridView1.Columns[10].HeaderText = "Denom_2";

            dataGridView1.Columns[11].Width = 60; // Cass_2 
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].Visible = true;
            dataGridView1.Columns[11].HeaderText = "Cass_2";

            dataGridView1.Columns[12].Width = 60; //Denom_3  
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[12].Visible = true;
            dataGridView1.Columns[12].HeaderText = "Denom_3";

            dataGridView1.Columns[13].Width = 60; // Cass_3 
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[13].Visible = true;
            dataGridView1.Columns[13].HeaderText = "Cass_3";

            dataGridView1.Columns[14].Width = 60; //Denom_4  
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[14].Visible = true;
            dataGridView1.Columns[14].HeaderText = "Denom_4";

            dataGridView1.Columns[15].Width = 60; // Cass_4 
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[15].Visible = true;
            dataGridView1.Columns[15].HeaderText = "Cass_4";

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                bool WMatched = (bool)row.Cells[1].Value;

                if (WMatched == false)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        // Show Grid 2
        private void ShowGrid_2()
        {

            dataGridView2.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }

            dataGridView2.Columns[0].Width = 70; // SesNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);

            dataGridView2.Columns[1].Width = 100; // SesDtTimeStart
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 100; // SesDtTimeEnd
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 50; // 
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 250; // ProcessMode
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


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

        public string ImgBrowseDir { get; private set; }
        public string ImgFileName { get; private set; }

        // RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Read Specific 
            MessageBox.Show("Inactive button");
            return; 
        }
     

        // UPDATE 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            // Validation 
            //if (int.TryParse(textBoxOrderNo.Text, out WOrderNo))
            //{
            //    if (WOrderNo == 0)
            //    {
            //        MessageBox.Show("Please enter a valid number for Order!");

            //        return;
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please enter a valid number for Order!");

            //    return;
            //}

            //if (decimal.TryParse(textBoxNextFeedingCash.Text, out Loaded_NextOver1000))
            //{
            //    if (Loaded_NextOver1000 == 0)
            //    {
            //        MessageBox.Show("Please enter a valid number for Amount!");

            //        return;
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please enter a valid number for Amount!");

            //    return;
            //}

            //if ((TotalInputtedOver1000-OldLoadedOver1000 + Loaded_NextOver1000) > HashTotalOver1000)
            //{
            //    MessageBox.Show("Operation not allowed!" + Environment.NewLine
            //                    + "Total Inputted Amount will be more than the defined Total"
            //                             );
            //    return;
            //}

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
// See Excel 
        

        private void buttonBrowseForExcel_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.InitialDirectory = ImgBrowseDir;
                //dlg.Filter = "JPG Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif";
                dlg.Filter = "XLS Files (*.xls)|*.xls|xlsx Files (*.xlsx)|*.xlsx";
                dlg.Title = "Select an Image";
                //Fab Misr 14.11.2022.xls
                dlg.FileName = @"Fab_isr_14.11.2022.xls";
                dlg.InitialDirectory = @"C:\AUDI _ Working\_EXCEL_loading";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ImgFileName = dlg.FileName.ToString();
                    //pictureBox.ImageLocation = ImgFileName;
                    dlg.OpenFile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
// SHOW SM LINES 
        private void linkLabelSMLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            DateTime NullPastDate = new DateTime(1900, 01, 01);
            WAtmNo = textBoxAtm.Text; 

            // The WWSeqNo is the slected SesNo 
            if (WWSeqNo== 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            string SM_SelectionCriteria1 = " WHERE atmno ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + WWSeqNo
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

            SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, WWSeqNo, 2);

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

        }
    }
}

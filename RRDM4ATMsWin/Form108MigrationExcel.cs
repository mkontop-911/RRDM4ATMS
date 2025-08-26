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
    public partial class Form108MigrationExcel : Form
    {

      
        string WAtmNo;
     

        DateTime FutureDate = new DateTime(2050, 11, 21);

        RRDMMigrationCycles Mc = new RRDMMigrationCycles(); 

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMAtmsCostClass Ap = new RRDMAtmsCostClass();
        RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();
        RRDMAccountsClass Acc = new RRDMAccountsClass();

        //     FormMainScreen NFormMainScreen;
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WMigrationCycle; 

        public Form108MigrationExcel(string InSignedId, int SignRecordNo, string InOperator, int InMigrationCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WMigrationCycle = InMigrationCycle; 

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            textBoxMsgBoard.Text = "ATM Migration Process";

            Mc.ReadMigrationCyclesById(WOperator, WMigrationCycle);
            if (Mc.ExcelId != "")
            {
                ExcelFileName = Mc.ExcelId;
                textBoxExcelName.Text = ExcelFileName;
            }

            labelStep1.Text = "ATMs Migration for Cycle : " + WMigrationCycle.ToString(); 

            panel2.Hide();
        }

        private void Form108_Load(object sender, EventArgs e)
        {

            try
            {

              //  string Tfilter = "Operator = '" + WOperator + "' OR AtmNo = 'ModelPrive' ";

                Ac.ReadAtmAndFillTableByOperator(WSignedId, WOperator);

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

            WAtmNo = rowSelected.Cells[0].Value.ToString();
            ExcelBranch = rowSelected.Cells[1].Value.ToString();
            ExcelBranchName = rowSelected.Cells[2].Value.ToString();
            ExcelStreet = rowSelected.Cells[3].Value.ToString();
            ExcelDistrict = rowSelected.Cells[4].Value.ToString();
            ExcelCountry = rowSelected.Cells[5].Value.ToString();
            ExcelModel = rowSelected.Cells[6].Value.ToString();
            //WSeqNoLeft = (int)rowSelected.Cells[0].Value;
            //ExcelAtmsReconcGroup = rowSelected.Cells[7].Value.ToString();
            ExcelATMIPAddress = rowSelected.Cells[8].Value.ToString();
            ExcelModelAtm = rowSelected.Cells[9].Value.ToString();

            textBox1.Text = WAtmNo;
            textBox4.Text = ExcelBranch;
            textBox5.Text = ExcelBranchName;
            textBox9.Text = ExcelStreet;
            textBox6.Text = ExcelDistrict;
            textBox7.Text = ExcelModel;

        }


        // Finish 
        private void button5_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // Finish event 
        private void Form108_FormClosing(object sender, FormClosingEventArgs e)
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

            Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Admin\Desktop\Dropbox\Vandit's Folder\Internship\test.xlsx");
            Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Panicos Michael\Desktop\REPORTS\ATMSView56-2.xlsx");
            Excel.Worksheet worksheet = workbook.ActiveSheet;

            int rcount = worksheet.UsedRange.Rows.Count;

            int i = 0;

            //Initializing Columns
            dataGridView1.ColumnCount = worksheet.UsedRange.Columns.Count;
            //for (int x = 0; x < dataGridView1.ColumnCount; x++)
            //{
            //    dataGridView1.Columns[x].Name = "Column " + x.ToString();
            //}
            dataGridView1.Columns[0].Name = worksheet.Cells[i + 1, 1].Value;
            dataGridView1.Columns[1].Name = worksheet.Cells[i + 1, 2].Value;
            dataGridView1.Columns[2].Name = worksheet.Cells[i + 1, 3].Value;
            dataGridView1.Columns[3].Name = worksheet.Cells[i + 1, 4].Value;
            dataGridView1.Columns[4].Name = worksheet.Cells[i + 1, 5].Value;
            dataGridView1.Columns[5].Name = worksheet.Cells[i + 1, 6].Value;
            dataGridView1.Columns[6].Name = worksheet.Cells[i + 1, 7].Value;
            dataGridView1.Columns[7].Name = worksheet.Cells[i + 1, 8].Value;

            i = 1;

            for (; i < rcount; i++)
            {
                dataGridView1.Rows.Add
                    (worksheet.Cells[i + 1, 1].Value,
                     worksheet.Cells[i + 1, 2].Value,
                     worksheet.Cells[i + 1, 3].Value,
                     worksheet.Cells[i + 1, 4].Value,
                     worksheet.Cells[i + 1, 5].Value,
                     worksheet.Cells[i + 1, 6].Value,
                     worksheet.Cells[i + 1, 7].Value,
                     worksheet.Cells[i + 1, 8].Value
                     );
            }

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No ATMs Available!");
                return;
            }
            else
            {
                textBox12.Text = dataGridView1.Rows.Count.ToString();
            }

            dataGridView1.Columns[0].Width = 60; // AtmNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 60; //Branch
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            //dataGridView1.Columns[2].Width = 60; //  Type Of Author
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[3].Width = 140; // Openning Date
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        //Enter 
        private void button7_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))

            {
                MessageBox.Show("Enter An ATM number Please");
                return;
            }
            else
            {
                WAtmNo = textBox1.Text;

            }

            // See if this ATM belongs to the user 

            //RRDMRightToAccessAtm Ra = new RRDMRightToAccessAtm();

            //Ra.CheckRightToAccessAtm(WSignedId, WAtmNo);

            //if (Ra.RecordFound == false)
            //{
            //    MessageBox.Show(" You are not authorised to access this ATM ");
            //    return;
            //}

            //string Tfilter = "Operator = '" + WOperator + "' AND AtmNo ='" + WAtmNo + "'";

            Ac.ReadAtmAndFillTableByAtmNo(WSignedId, WOperator, WAtmNo);

            ShowGridAtms();
        }
        //Refresh - show all ATMs 
        private void button8_Click(object sender, EventArgs e)
        {
            Form108_Load(this, new EventArgs());
        }

        // Print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {                                                                    

            string P1 = "Migrated ATMs Results ";

            string P2 = WMigrationCycle.ToString();
            string P3 = "Third Par";
            string P4 = Ac.BankId;
            string P5 = WSignedId;

            Form56R58ATMS ReportATMS58 = new Form56R58ATMS(P1, P2, P3, P4, P5);
            ReportATMS58.Show();
        }
        // Browse for Excel 
        string ExcelFileName;
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
                    ExcelFileName = dlg.FileName.ToString();
                    textBoxExcelName.Text = ExcelFileName;
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
            
            Excel.Application app = new Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Open(@"C:\Users\Admin\Desktop\Dropbox\Vandit's Folder\Internship\test.xlsx");
            Excel.Workbook workbook = app.Workbooks.Open(ExcelFileName);
            Excel.Worksheet worksheet = workbook.ActiveSheet;

            try
            {

                int rcount = worksheet.UsedRange.Rows.Count;

                int i = 0;

                //Initializing Columns
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

                i = 1;

                for (; i < rcount; i++)
                {
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
                         worksheet.Cells[i + 1, 10].Value
                         );
                }

                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No ATMs Available in excel!");
                    return;
                }
                else
                {
                    textBox12.Text = dataGridView1.Rows.Count.ToString();
                }

                dataGridView1.Columns[0].Width = 60; // AtmNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[1].Width = 60; //Branch
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                Marshal.FinalReleaseComObject(workbook);
                Marshal.ReleaseComObject(worksheet);

                // UPDATE MIGRATION CYCLE
           
                Mc.ReadMigrationCyclesById(WOperator, WMigrationCycle);

                Mc.ExcelId = textBoxExcelName.Text;
                Mc.ProcessStage = 1;

                Mc.UpdateMigrationCycle(WMigrationCycle); 

            }
            catch (Exception ex)
            {
                Marshal.FinalReleaseComObject(workbook);
                Marshal.ReleaseComObject(worksheet);

                CatchDetails(ex);

            }
            finally
            {

            }

            //Worksheet sheet = excelApp.Worksheets.Open(...);

        }
        // Validate and note the errors
        private void buttonValidate_Click(object sender, EventArgs e)
        {
            // VALIDATE LINE BY LINE (Loop)
            ValidateExcelLine();
        }
        // Create the in Error Excel 
        private void buttonCreateExcel_Click(object sender, EventArgs e)
        {
            // Validate and Create Excel
            ValidateExcelLine();
        }
        // Update and Make new ATMs 
        string ExcelAtmNo;
        string ExcelBranch;
        string ExcelBranchName;
        string ExcelStreet;
        string ExcelDistrict;
        string ExcelCountry;
        string ExcelModel;
        string ExcelAtmsReconcGroup;
        string ExcelATMIPAddress;
        string ExcelModelAtm;

        int TotalUpdated;
        int TotalNew;

        bool UpdateMode;
        bool AddMode;
        bool ErrorInValidation; 

        RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

        private void buttonUpdateNewAtms_Click(object sender, EventArgs e)
        {
            // Read Excel Line by Line 
            // Read Model ATM
            // Update the ones found 
            // Create new ones for the not found. 
            // Update Migration Session No 
            TotalUpdated = 0;
            TotalNew = 0;


            //Clear Table 
            //Tr.DeleteReport58(WSignedId);

            int K = 0;

            while (K <= (dataGridView1.Rows.Count - 1))
            {
                UpdateMode = false;
                AddMode = false;
                ErrorInValidation = false; 

                DataGridViewRow rowSelected = dataGridView1.Rows[K];

                ExcelAtmNo = rowSelected.Cells[0].Value.ToString();
                ExcelBranch = rowSelected.Cells[1].Value.ToString();
                ExcelBranchName = rowSelected.Cells[2].Value.ToString();
                ExcelStreet = rowSelected.Cells[3].Value.ToString();
                ExcelDistrict = rowSelected.Cells[4].Value.ToString();
                ExcelCountry = rowSelected.Cells[5].Value.ToString();
                ExcelModel = rowSelected.Cells[6].Value.ToString();

                ExcelAtmsReconcGroup = rowSelected.Cells[7].Value.ToString();
                ExcelATMIPAddress = rowSelected.Cells[8].Value.ToString();
                ExcelModelAtm = rowSelected.Cells[9].Value.ToString();

                Ac.ReadAtm(ExcelAtmNo);

                if (Ac.RecordFound == true)
                {

                    //*************************
                    // This is for updating the ATM                   
                    //**************************

                    Ac.Branch = ExcelBranch;
                    Ac.BranchName = ExcelBranchName;
                    Ac.Street = ExcelStreet;
                    Ac.District = ExcelDistrict;
                    Ac.AtmsReconcGroup = Convert.ToInt32(ExcelAtmsReconcGroup);

                    Ac.UpdateATM(ExcelAtmNo);

                    //*************************
                    //  UPDATE ATMs MAIN                   
                    //**************************

                    Am.ReadAtmsMainSpecific(ExcelAtmNo);

                    Am.AtmNo = Ac.AtmNo;
                    Am.AtmName = Ac.AtmName;
                    Am.BankId = Ac.BankId;

                    Am.RespBranch = Ac.Branch;
                    Am.BranchName = Ac.BranchName;

                    Am.CitId = Ac.CitId;
                    Am.AtmsReplGroup = Ac.AtmsReplGroup;
                    Am.AtmsReconcGroup = Ac.AtmsReconcGroup;

                    Am.Operator = Ac.Operator;

                    Am.UpdateAtmsMain(ExcelAtmNo); // UPDATE MAIN WITH NEW VALUES 

                    //*************************
                    //  COST                    
                    //**************************

                    Ap.ReadTableATMsCostSpecific(ExcelAtmNo);

                    // UPDATE FIELDS SUCH AS 
                    // Ap.PurchaseDt = PurchaseDt;

                    // UPDATE METHOD
                    Ap.UpdateTableATMsCost(ExcelAtmNo, Ac.BankId);

                    //*************************
                    // Update Physical                  
                    //**************************

                    Jd.ReadJTMIdentificationDetailsByAtmNo(ExcelAtmNo);

                    Jd.ATMIPAddress = ExcelATMIPAddress;

                    Jd.UpdateRecordInJTMIdentificationDetailsByAtmNo(ExcelAtmNo);

                    UpdateMode = true; 

                    TotalUpdated = TotalUpdated + 1;

                }
                else
                {
                    // This is for creating new ATM   

                    Ac.ReadAtm(ExcelModelAtm);

                    if (Ac.RecordFound == true)
                    {
                        //*************************
                        // This is for insert the new ATM                  
                        //**************************
                        Ac.AtmNo = ExcelAtmNo;
                        Ac.Branch = ExcelBranch;
                        Ac.BranchName = ExcelBranchName;
                        Ac.Street = ExcelStreet;
                        Ac.District = ExcelDistrict;
                        Ac.AtmsReconcGroup = Convert.ToInt32(ExcelAtmsReconcGroup);

                        // INSERT INSERT INSERT INSERT INSERT
                        Ac.InsertATM(ExcelAtmNo); // Insert ATM record

                        //*************************
                        //  INSERT ATMs MAIN                   
                        //**************************
                        Am.ReadAtmsMainSpecific(ExcelModelAtm); // READ FROM MODEL 

                        Am.AtmNo = ExcelAtmNo;
                        Am.AtmName = Ac.AtmName;
                        Am.BankId = Ac.BankId;

                        Am.RespBranch = Ac.Branch;
                        Am.BranchName = Ac.BranchName;

                        // Initialise for new atm 
                        Am.NextReplDt = FutureDate;
                        Am.EstReplDt = FutureDate;

                        Am.LastUpdated = DateTime.Now;

                        Am.CitId = Ac.CitId;
                        Am.AtmsReplGroup = Ac.AtmsReplGroup;
                        Am.AtmsReconcGroup = Ac.AtmsReconcGroup;

                        Am.GL_CurrNm1 = Ac.DepCurNm; 

                        Am.Operator = Ac.Operator;

                        // INSERT INSERT INSERT INSERT INSERT
                        Am.InsertInAtmsMain(ExcelAtmNo); // Insert AtmMain record 

                        //*************************
                        //  COST                    
                        //**************************

                        Ap.ReadTableATMsCostSpecific(ExcelModelAtm);

                        Ap.AtmNo = ExcelAtmNo;
                        // Insert FIELDS SUCH AS 
                        // Ap.PurchaseDt = PurchaseDt;

                        // ===============Insert ================
                        Ap.Operator = Ac.Operator;
                        Ap.InsertTableATMsCost(ExcelAtmNo, Ac.BankId);


                        //*************************
                        // Insert Physical                  
                        //**************************

                        Jd.ReadJTMIdentificationDetailsByAtmNo(ExcelModelAtm);

                        Jd.AtmNo = ExcelAtmNo;

                        Jd.ATMIPAddress = ExcelATMIPAddress;

                        // ===============Insert ================
                        Jd.InsertNewRecordInJTMIdentificationDetails();

                        // ==============Copy ACCOUNTS FROM LIKE==========
                        Ac.ReadAtm(ExcelModelAtm);

                        Acc.CopyAccountsAtmToAtm(Ac.BankId, ExcelModelAtm, Ac.BankId, ExcelAtmNo);
                        if (Acc.RecordFound == false)
                        {
                            MessageBox.Show("There were no accounts to copy. After ATM creation go and create accounts manually please for the added ATM .");
                            MessageBox.Show("ATM added without accounts");
                        }
                        else
                        {
                            // Everything OK 
                        }

                        TotalNew = TotalNew + 1;

                    }
                    else
                    {
                        MessageBox.Show("Model ATM: " + ExcelModelAtm + " not found during ATM Add.");
                    }

                    AddMode = true; 
                }
                //
                // Insert Excel Like in Report 
                //
                InsertExcelLineInReport();

                K++; // Read Next entry of the table 
            }

            textBoxTotalUpdated.Text = TotalUpdated.ToString();
            textBoxTotalNew.Text = TotalNew.ToString();

            // UPDATE MIGRATION CYCLE

            Mc.ReadMigrationCyclesById(WOperator, WMigrationCycle);

            Mc.FinishDateTm = DateTime.Now; 
            Mc.InvalidInExcelRecords = 0;
            Mc.OpenedNewAtms = TotalNew;
            Mc.UpdatedATMs = TotalUpdated; 
            Mc.ProcessStage = 2;
            Mc.UpdateMigrationCycle(WMigrationCycle);

            buttonPrint.Show(); 

        }

        // Insert report line 
        private void InsertExcelLineInReport()
        {

            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            Tr.MigrationCycleToString = WMigrationCycle.ToString(); 
            Tr.AtmNo = ExcelAtmNo;
            Tr.Branch = ExcelBranch;
            Tr.BranchName = ExcelBranchName;

            Tr.Model = ExcelModel;

            Tr.AtmsReconcGroup = Convert.ToInt32(ExcelAtmsReconcGroup);
            Tr.ATMIPAddress = ExcelATMIPAddress;

            if (UpdateMode == true) Tr.UpdateOrInsert = "ATM Updated";
            if (AddMode == true) Tr.UpdateOrInsert = "ATM Added";
            if (ErrorInValidation == false) Tr.MigrationResult = "Successful";

            // Insert record for printing 
            //
            Tr.InsertReport58(WSignedId);


        }
     

        private void ValidateExcelLine()
        {

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

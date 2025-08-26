using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form18_ExcelMainMenu : Form
    {
       

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //TEST
        DateTime WorkingToday = new DateTime(2014, 07, 06);

        string WCitId;
        //int WGroupNo;

        string filter; 

        //string WAccName;
        //string WAccCurr; 

        //int WAction; 

  //      int WFunctionNo;
        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
        DateTime WCut_Off_Date; 

        public Form18_ExcelMainMenu(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator,DateTime InCut_Off_Date ,int InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WCut_Off_Date = InCut_Off_Date; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = WSignedId;
            labelCutOff.Text = WCut_Off_Date.ToShortDateString(); 

            textBoxMsgBoard.Text = "View CIT Excel Management"; 

        }

        // Load 
        private void Form18_Load(object sender, EventArgs e)
        {
            
            filter = "Operator = '" + WOperator + "' AND UserType ='CIT Company'";

            Us.ReadUsersAndFillDataTable(WSignedId, WOperator, filter, ""); // Read User table 

            ShowGrid(); 

        }

        // ROW ENTER ON USER 
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            string temp = rowSelected.Cells[0].Value.ToString();
            WCitId = temp;
           
         
            label13.Text = temp;
            
            //filter = "Operator = '" + WOperator + "' AND UserId ='" + WCitId + "'";

            //Ua.ReadUserAccessToAtmsFillTable(filter);

         
        }
    

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGrid()
        {
            dataGridView1.DataSource = Us.UsersInDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No available CIT providers.");
                this.Dispose();
                return;
            }

            dataGridView1.Columns[0].Width = 40; // User Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 110; // User Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 140; //  email 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Columns[3].Width = 50; // Mobile
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 120; // date Open
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 70; // User Type
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 80; // Cit Id 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        
        }


     

        // Finish 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
//  GO TO LOAD EXCEL
        private void button6_Click(object sender, EventArgs e)
        {
          
                // Found a
                Form18_CIT_LoadExcel_KFH NForm108Excel_G4S;
                NForm108Excel_G4S = new Form18_CIT_LoadExcel_KFH(WSignedId, WSignRecordNo, WOperator, WCitId);
                NForm108Excel_G4S.ShowDialog(); 
         
        }
// EXcel Loading Cycles
        private void button7_Click(object sender, EventArgs e)
        {
            Form18_CIT_ExcelLoadingCycles NForm18_CIT_ExcelLoadingCycles;

            int Mode = 2; // update 

            NForm18_CIT_ExcelLoadingCycles = new Form18_CIT_ExcelLoadingCycles(WSignedId, WSignRecordNo, WOperator, WCitId);
            NForm18_CIT_ExcelLoadingCycles.ShowDialog();
        }
// SHOW BANKs Records 
        private void button8_Click(object sender, EventArgs e)
        {
            Form18_CIT_ViewBankRecords NForm18_CIT_ViewBankRecords;

            int Mode = 2; // update 
            DateTime WDate = DateTime.Now.Date; 
            NForm18_CIT_ViewBankRecords = new Form18_CIT_ViewBankRecords(WOperator, WSignedId, WCitId, WDate);
            NForm18_CIT_ViewBankRecords.ShowDialog();
        }

        // LOAD excel For AUDI
        private void buttonLoadForAudi_Click(object sender, EventArgs e)
        {
            // Found a
            Form18_CIT_LoadExcel_AUDI NForm18_CIT_LoadExcel_AUDI;
            NForm18_CIT_LoadExcel_AUDI = new Form18_CIT_LoadExcel_AUDI(WSignedId, WSignRecordNo, WOperator, WCitId);
            NForm18_CIT_LoadExcel_AUDI.ShowDialog();
        }
// NEW EXCEL LOADING 
        private void button1_Click(object sender, EventArgs e)
        {
            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            RRDM_Cit_ExcelProcessedCycles Cic = new RRDM_Cit_ExcelProcessedCycles();

            CreateNewLoadCycle(WCitId, WCut_Off_Date);

            // Check if Authorisation process exist
            //
           
                // There is an open Cycle

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCitId, WLoadingCycle, "LoadingExcel"); //
                if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist 
                {
                    MessageBox.Show("This Loading Process Already has authorization record!" + Environment.NewLine
                                             + "Go to Pending Authorisations process to complete.");

                    return;
                }

            // Process
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 2;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form276_NBG NForm276_NBG;

            NForm276_NBG = new Form276_NBG(WSignedId, WSignRecordNo, WOperator, WCitId, WLoadingCycle);
            NForm276_NBG.ShowDialog();
        }

        int WProcessStage;
        int WLoadingCycle;
        DateTime WOutputDate; 

        private void CreateNewLoadCycle(string InCitId, DateTime InCutOff)
        {
            RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();
            // If not already exist and Open create a new one
            string WSelectionCriteria = " Where CitId='" + WCitId + "' AND ProcessStage != 3 ";
            Cec.ReadExcelLoadCyclesBySelectionCriteria(WSelectionCriteria);
            if (Cec.RecordFound == true)
            {
                // There is an open Cycle
                WLoadingCycle = Cec.SeqNo;
                WProcessStage = Cec.ProcessStage;
                WOutputDate = Cec.StartDateTm;

                //if (Cec.IsReversed == true)
                //{
                //    // turn reversal to false
                //    //
                //    Cec.IsReversed = false;

                //    Cec.UpdateLoadExcelCycle(Cec.SeqNo);
                //}
               
            }
            else
            {

                // Insert Excel Load Cycle 

                Cec.CitId = WCitId;
                //Cec.Cut_Off_Date = InCutOff;
                WOutputDate = Cec.StartDateTm = DateTime.Now;
                //Cec.ExcelId = "Not Define yet";
                Cec.ProcessStage = 0;
                Cec.UserId = WSignedId;
                Cec.Operator = WOperator;

                WLoadingCycle = Cec.InsertExcelLoadCycle();

            }
        }
// INPUT CIT FIGURES 
        private void buttonInputCITFeeding_Click(object sender, EventArgs e)
        {
            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            RRDM_Cit_ExcelProcessedCycles Cic = new RRDM_Cit_ExcelProcessedCycles();
            // COMMENT OUT THIS WILL BE CREATED DURING LOADING OF EXCEL 
            //CreateNewLoadCycle(WCitId, WCut_Off_Date);

            // Check if Authorisation process exist
            //

            // There is an open Cycle

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCitId, WLoadingCycle, "LoadingExcel"); //
            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist 
            {
                MessageBox.Show("This Loading Process Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process to complete.");

                return;
            }

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WJobCategory = "ATMs";

            int WRMCycle = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (WRMCycle == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {

            }


            // 1. CHECK IF THERE ARE OUTSTADING TO BE LOADED
            // 2. CHECK IF EXCEL CYCLE IS ALREADY OPEN
            // 3. If Not OPEN Create a Cycle 
            // DEAL WITH EXCEL CYCLES
            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

            // CHECK if records to process for this CIT
            bool NothingForLoading = false;
            bool NothingForUnloading = false;  

            int TempMode = 1; 
            string SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND (Repl_Load_Status = 4)"
                      + " AND Cash_Loaded > 0 AND ProcessMode_Load <> 2 ";
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            if (G4.DataTableG4SEntries.Rows.Count == 0)
            {
                NothingForLoading = true;
            }
            else
            {
                NothingForLoading = false;
            }

            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Repl_UnLoad_Status in ( 4 ) AND ProcessMode_UnLoad <> 2 ";

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable_UNLOAD_WithMatchStatus(WOperator, WSignedId, SelectionCriteria, TempMode);

            if (G4.DataTableG4SEntries.Rows.Count == 0)
            {
                NothingForUnloading = true;
            }
            else
            {
                NothingForUnloading = false;
            }

            if (NothingForLoading == true & NothingForUnloading == true)
            {
                MessageBox.Show("No other entries for processing are availble");
                return; 
            }

            RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

            // CHECK IF IN PROCESS CYCLE
            SelectionCriteria = " WHERE CitId='" + WCitId + "' AND ProcessStage <> 3 "; 

            Cec.ReadExcelLoadCyclesBySelectionCriteria(SelectionCriteria); 

            if (Cec.RecordFound == true)
            {
                // Move to this Cycle
                WLoadingCycle = Cec.SeqNo; 
            }
            else
            {
                // Create a NEW CYCLE
                Cec.CitId = WCitId;

                Cec.StartDateTm = DateTime.Now;

                Cec.ProcessStage = 2; // 
                Cec.UserId = WSignedId;

                Cec.RMCycle = WRMCycle;

                Cec.Operator = WOperator;

                WLoadingCycle = Cec.InsertExcelLoadCycle();
                ;
            }

            // Process
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 2;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form276_AUDI_FirstStep NForm276_AUDI_FirstStep;

            NForm276_AUDI_FirstStep = new Form276_AUDI_FirstStep(WSignedId, WSignRecordNo, WOperator, WCitId, WLoadingCycle, WRMCycle);
            NForm276_AUDI_FirstStep.ShowDialog();
        }
// Show Alerts 
        private void buttonAlerts_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Functionality can show the delayed Replenishments"+Environment.NewLine
                + "It is not within the scope of the project" + Environment.NewLine
                + "It can, however, be developed " + Environment.NewLine
                + "if the Bank professionals think that it is needed " + Environment.NewLine
                + "" + Environment.NewLine
                );

            return; 

            string WTableId = "";

            string WAtmNo = "";

            int WSesNo = 0;

            int WMode = 8; // ALERTS 

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo
                                                    , NullPastDate, NullPastDate, WSesNo, NullPastDate, WMode);

            NForm78D_ATMRecords.Show();

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}


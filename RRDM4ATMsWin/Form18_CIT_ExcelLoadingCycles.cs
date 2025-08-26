using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form18_CIT_ExcelLoadingCycles : Form
    {
       
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //DateTime WDateTime;
      
        //string WSelectionCriteria;
        int WExcelLoadingCycle; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCitId; 
       
        public Form18_CIT_ExcelLoadingCycles(string InSignedId, int InSignRecordNo, string InOperator, string InCitId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId; 

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

            labelUserId.Text = InSignedId;



        }
// Load 

        private void Form18_CIT_ExcelLoadingCycles_Load(object sender, EventArgs e)
        {
            Cec.ReadExcelLoadCyclesFillTable_Feeding(WOperator, WSignedId, WCitId);

            ShowGrid1();
        }

        // Row Enter 
        int WSeqNo ;
  
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            //labelCycle.Text = "Loading Cycle : " + WSeqNo.ToString(); 

            Cec.ReadExcelLoadCyclesBySeqNo(WSeqNo);

            WExcelLoadingCycle = Cec.SeqNo; 

            if (Cec.ProcessStage == 2)
            {
                // Process
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 2;
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                buttonNext.Text = "Next";
            }

            if (Cec.ProcessStage == 3)
            {
                // Process
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54;
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                buttonNext.Text = "View";

            }


        }

        private void ShowGrid1()
        {
            
            dataGridView1.DataSource = Cec.TableExcelLoadCycles.DefaultView;

            //TableExcelLoadCycles.Columns.Add("SeqNo", typeof(int));

            //TableExcelLoadCycles.Columns.Add("RMCycle", typeof(int));

            //TableExcelLoadCycles.Columns.Add("StartDateTm", typeof(string));
            //TableExcelLoadCycles.Columns.Add("FinishDateTm", typeof(string));


            //TableExcelLoadCycles.Columns.Add("MakerId", typeof(string));

            //TableExcelLoadCycles.Columns.Add("AuthoriserId", typeof(string));

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }

            


            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = true;
            dataGridView1.Columns[0].HeaderText = "Cycle";

            dataGridView1.Columns[1].Width = 70; // RM CYCLE 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 150; //  Date start 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 150; // Date finish 
            dataGridView1.Columns[3].DefaultCellStyle = style;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 200; //Maker 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].Visible = true;

            dataGridView1.Columns[5].Width = 200; // Authoriser 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].Visible = true;

            //dataGridView1.Columns[6].Width = 90; // FinishDateTm
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[6].Visible = true;



        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
//
// print Loading cycles
//
        private void buttonPrintCycle_Click(object sender, EventArgs e)
        {

            string P1 = "EXCEL LOADING CYCLES FOR CIT :  " + WCitId;

            string P2 = "";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;
            Form56R71 ReportATMS71 = new Form56R71(P1, P2, P3, P4, P5);
            ReportATMS71.Show();

        }
// Show Cycle Details 
        private void button1_Click(object sender, EventArgs e)
        {
            //public int ProcessStage;// 0: Excel Not read 
            //                    // 1 : Excel Read, 
            //                    // 2 : "Excel Validated";
            //                    // 3 : "Excel Updated

            //if (Cec.IsReversed == true)
            //{
            //    MessageBox.Show("Cycle was Reversed! " + Environment.NewLine
            //                   + "You can not view");
            //    return;
            //}
            string WStage = "";

            if (Cec.ProcessStage == 0) WStage = "Excel Not Read yet";
            if (Cec.ProcessStage == 1) WStage = "Excel Read Stage";
            if (Cec.ProcessStage == 2) WStage = "Excel at Validation Stage";

            if (Cec.ProcessStage != 3)
            {
                MessageBox.Show("Cycle Is not completed! " + Environment.NewLine
                    +"It is at the stage of " + WStage);
                return;
            }

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            //RRDM_Cit_ExcelInputCycles Cic = new RRDM_Cit_ExcelInputCycles();
       
            // Check if Authorisation process exist
            //

            // There is an open Cycle

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCitId, WExcelLoadingCycle, "LoadingExcel"); //
            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist 
            {
                MessageBox.Show("This Loading Process Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process to complete.");

                return;
            }

            // Process
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form276_NBG NForm276_NBG;

            NForm276_NBG = new Form276_NBG(WSignedId, WSignRecordNo, WOperator, WCitId, WExcelLoadingCycle);
            NForm276_NBG.ShowDialog();
        }
// Show Feeding Information 
        private void buttonFeeding_Click(object sender, EventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WJobCategory = "ATMs";

            int RMCycle = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (RMCycle == 0)
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

            //public int ProcessStage;// 0: Excel Not read 
            //                    // 1 : Excel Read, 
            //                    // 2 : "Excel Validated";
            //                    // 3 : "Excel Updated

            //if (Cec.IsReversed == true)
            //{
            //    MessageBox.Show("Cycle was Reversed! " + Environment.NewLine
            //                   + "You can not view");
            //    return;
            //}
            string WStage = "";

            //if (Cec.ProcessStage == 0) WStage = "Excel Not Read yet"; 
            //if (Cec.ProcessStage == 1) WStage = "Excel Read Stage";
            if (Cec.ProcessStage == 2) WStage = "Excel at Validation Stage";

            if (Cec.ProcessStage != 3)
            {
                MessageBox.Show("Cycle Is not completed! " + Environment.NewLine
                    + "It is at the stage of " + WStage);
                return;
            }

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            //RRDM_Cit_ExcelInputCycles Cic = new RRDM_Cit_ExcelInputCycles();

            // Check if Authorisation process exist
            //

            // There is an open Cycle

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCitId, WExcelLoadingCycle, "LoadingExcel"); //
            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist 
            {
                MessageBox.Show("This Loading Process Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process to complete.");

                return;
            }

            // Process
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form276_AUDI_FirstStep NForm276_AUDI_FirstStep;
            
            NForm276_AUDI_FirstStep = new Form276_AUDI_FirstStep(WSignedId, WSignRecordNo, WOperator, WCitId, WExcelLoadingCycle, RMCycle);
            NForm276_AUDI_FirstStep.ShowDialog();
        }
// EXCEL LOADING WORKFLOW 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            RRDM_Cit_ExcelProcessedCycles Cic = new RRDM_Cit_ExcelProcessedCycles();
            // COMMENT OUT THIS WILL BE CREATED DURING LOADING OF EXCEL 
            //CreateNewLoadCycle(WCitId, WCut_Off_Date);

            // Check if Authorisation process exist
            //

            // There is an open Cycle

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCitId, WExcelLoadingCycle, "LoadingExcel"); //
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

            //// Process
            //RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            //Usi.ReadSignedActivityByKey(WSignRecordNo);
            //Usi.ProcessNo = 2;
            //Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form276_AUDI_FirstStep NForm276_AUDI_FirstStep;

            NForm276_AUDI_FirstStep = new Form276_AUDI_FirstStep(WSignedId, WSignRecordNo, WOperator, WCitId, WExcelLoadingCycle, WRMCycle);
            NForm276_AUDI_FirstStep.ShowDialog();
        }


    }
}

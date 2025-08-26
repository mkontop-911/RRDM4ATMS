using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form18_CIT_ViewUpdatedExcelDetails_XXX : Form
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

        //RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //TEST
        DateTime WorkingToday = new DateTime(2014, 07, 06);

        int TempMode; 

        string WOperator;
        string WSignedId;

        string WCitId;

        int WExcelLoadCycle; 

        DateTime WDate;

        int WCycleStage; 

        public Form18_CIT_ViewUpdatedExcelDetails_XXX(string InOperator, string InSignedId, string InCitId, int InExcelLoadCycle ,DateTime InWorkingDate, int InCycleStage)
        {
            WSignedId = InSignedId;
            //WSignRecordNo = SignRecordNo;
            //WSecLevel = InSecLevel;
            WOperator = InOperator;

            WCitId = InCitId;

            WExcelLoadCycle = InExcelLoadCycle; 

            WDate = InWorkingDate;

            WCycleStage = InCycleStage; 
            // 1 Excel Loaded
            // 2 Excel Validated
            // 3 Excel Updated

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = WSignedId;

            if (WCycleStage == 1)
            {
                panel2.Hide();
            }
            else
            {
                panel2.Show();
            }

            Cec.ReadExcelLoadCyclesBySeqNo(WExcelLoadCycle);

            //textBoxTotal11.Text = Cec.ValidInExcelRecords.ToString();
            //textBoxTotalAA.Text = Cec.InvalidInExcelRecords.ToString();
            //textBoxTotal10.Text = Cec.NotInBank.ToString();
            //textBoxTotal01.Text = Cec.NotInG4S.ToString();

            //textBoxShort.Text = Cec.ShortFound.ToString();

            //textBoxPresenterNotEqual.Text = Cec.PresenterDiff.ToString();

            label11.Text = "View Details For Cycle :" + WExcelLoadCycle.ToString(); 


            label1.Text = "SELECTION FOR :" + WDate.ToShortDateString();

            textBoxMsgBoard.Text = "View All Cycle Records";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingCycleNo =" + WExcelLoadCycle;

        }

        // Load 
        private void Form18_Load(object sender, EventArgs e)
        {

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode );

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

            if (RequestFromMatched)
            {
                TempMode = 2; // From File 2  
                SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode = 1 "
                    + "AND AtmNo ='" + G4.AtmNo +"'";

                G4.ReadCIT_G4S_Repl_EntriesToFillDataTableForGrid2(WOperator, WSignedId, SelectionCriteria, 
                                                                              TempMode, G4.ReplDateG4S.Date);

                dataGridView2.Show();
                label12.Show();

                ShowGrid2();

            }
            else
            {
                dataGridView2.Hide();
                label12.Hide();
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
           


            //filter = "Operator = '" + WOperator + "' AND UserId ='" + WCitId + "'";

            //Ua.ReadUserAccessToAtmsFillTable(filter);


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
               
                return;
            }
            else
            {
                textBoxTotalEnries.Text = dataGridView1.Rows.Count.ToString(); 
            }

            //dataGridView1.Columns[0].Width = 40; // User Id
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 110; // User Name
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[2].Width = 100; //  email 
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ////dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            //dataGridView1.Columns[3].Width = 50; // Mobile
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[4].Width = 80; // date Open
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[5].Width = 50; // User Type
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[6].Width = 50; // Cit Id 
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;



            //UsersInDataTable.Columns.Add("UserId", typeof(string));
            //UsersInDataTable.Columns.Add("UserName", typeof(string));
            //UsersInDataTable.Columns.Add("email", typeof(string));
            //UsersInDataTable.Columns.Add("MobileNo", typeof(string));
            //UsersInDataTable.Columns.Add("DateOpen", typeof(string));
            //UsersInDataTable.Columns.Add("UserType", typeof(string));
            //UsersInDataTable.Columns.Add("CitId", typeof(string));

            //dataGridView1.Rows[WRowIndex].Selected = true;
            //dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGrid2()
        {

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
   
        }

        // Finish 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //// Validate All read 
        //int WSeqNo;
        //string WMask;
        //int Total11;
        //int TotalAA;
        //int Total10;
        //int Total01;

        //int TotalPresenterEqual;
        //int TotalPresenterNotEqual;

        //int TotalShortFound = 0;
        //decimal TotalPresenterDiffAmt = 0;
        //decimal TotalShortAmt = 0;



        // Print What you VIEW 
        private void buttonP11_Click(object sender, EventArgs e)
        {
            string P1 = label1.Text; 

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_G4S ReportATMSReplCycles = new Form56R69ATMS_G4S(P1, P2, P3, P4, P5);
            ReportATMSReplCycles.Show();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            label1.Text = "SELECTION FOR MATCHED";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = '11' AND LoadingCycleNo =" + WExcelLoadCycle;

            RequestFromMatched = true;
            Presented = false;

            Form18_Load(this, new EventArgs());    
        }

        private void buttonAA_Click_1(object sender, EventArgs e)
        {
            label1.Text = "SELECTION FOR MATCHED BUT DIFFERENT VALUES";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = 'AA'  AND LoadingCycleNo =" + WExcelLoadCycle;

            RequestFromMatched = true;
            Presented = false;

            Form18_Load(this, new EventArgs());
      
        }


        private void button10_Click_1(object sender, EventArgs e)
        {
            label1.Text = "SELECTION FOR MISSING IN BANK ";
            TempMode = 1;
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = '10'  AND LoadingCycleNo =" + WExcelLoadCycle;

            RequestFromMatched = false;

            Presented = false;

            Form18_Load(this, new EventArgs());
        }

        private void button01_Click_1(object sender, EventArgs e)
        {
            label1.Text = "PRESENT IN BANK BUT MISSING IN G4S ";
            TempMode = 2; // From File 2  
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND Mask = '10' AND ProcessMode <> 1 ";

            RequestFromMatched = false;
            Presented = false;

            Form18_Load(this, new EventArgs());
         
        }

        // Short 
        private void button1_Click(object sender, EventArgs e)
        {
            
            label1.Text = "CASES OF SHORT NOTES FOUND ";
            TempMode = 1; // From File 2  
            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ShortFound>0 AND LoadingCycleNo =" + WExcelLoadCycle;

            RequestFromMatched = false;
            Presented = false;

            Form18_Load(this, new EventArgs());
        }

        // Presenter Not Equal

        private void buttonPresenterNotEqual_Click(object sender, EventArgs e)
        {
            label1.Text = "Presented Errors Amount Not Equal To Suplus ";
            TempMode = 1; // From File 1 as it is the same as in file 2  
   
            SelectionCriteria = " WHERE (OverFound - PresentedErrors) <> 0 AND Mask = '11' AND LoadingCycleNo =" + WExcelLoadCycle;

            Presented = true;

            RequestFromMatched = true;

            Form18_Load(this, new EventArgs());
        }

       


    }
}


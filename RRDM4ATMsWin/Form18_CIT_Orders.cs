using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form18_CIT_Orders : Form
    {
      
        Form31 NForm31;

        DateTime FromDate;
        DateTime ToDate;

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //TEST
        DateTime WorkingToday = new DateTime(2014, 07, 06);

        string WCitId;
    
        string filter; 

        string WAccName;
        string WAccCurr; 

        int WAction; 

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form18_CIT_Orders(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, int InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
      
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = WSignedId;

            //TEST
            dateTimePicker1.Value = WorkingToday;
            dateTimePicker2.Value = DateTime.Now.Date;  
            
            textBoxMsgBoard.Text = "View CIT operational and financial information"; 

        }

        // Load 
        private void Form18_Load(object sender, EventArgs e)
        {
            
            filter = " Operator = '" + WOperator + "' AND UserType ='CIT Company'";

            Us.ReadUsersAndFillDataTable(WSignedId, WOperator, filter, ""); // Read User table 

            ShowGrid(); 

        }

        // ROW ENTER ON USER 
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            string temp = rowSelected.Cells[0].Value.ToString();
            WCitId = temp;
           
            comboBox1.DataSource = Cc.GetUserAccs(WOperator, WCitId);
            comboBox1.DisplayMember = "DisplayValue";

            comboBox2.DataSource = Cc.GetUserAccsCurr(WOperator, WCitId);
            comboBox2.DisplayMember = "DisplayValue";

            label13.Text = temp;
            
            filter = "Operator = '" + WOperator + "' AND UserId ='" + WCitId + "'";

            Ua.ReadUserAccessToAtmsFillTable(filter);

            ShowGrid2(); 
  
            radioButton1.Checked = true; 

        }
    
        //
        // Group Enter 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            string WAtmNo = rowSelected.Cells[1].Value.ToString();        
        }
      
        // PROCEED TO SHOW STATEMENT 
        private void button2_Click(object sender, EventArgs e)
        {
            // Last 7 days = WAction 1
            // Last Month = 2
            // Current Month = 3
            // Selected Dates = 4
            if (radioButton1.Checked == false & radioButton2.Checked == false 
                & radioButton3.Checked == false & radioButton4.Checked == false)
            {
                MessageBox.Show("Make your choice please!");
                return; 
            }

            if (radioButton1.Checked == true)
            {
                WAction = 1; // Last 7 days = WAction 1
                WAccName = comboBox1.Text;
                WAccCurr = comboBox2.Text; 
                FromDate = DateTime.Today.AddDays(-7);
                ToDate = DateTime.Today;
                //TEST
                FromDate = WorkingToday.AddDays(-7);
                ToDate = WorkingToday;
            }

            if (radioButton2.Checked == true)
            {
                WAction = 2; // Last Month = 2
                WAccName = comboBox1.Text;
                WAccCurr = comboBox2.Text; 
                var today = DateTime.Today;
                //TEST
                today = WorkingToday;
                var month = new DateTime(today.Year, today.Month, 1);
                var first = month.AddMonths(-1);
                var last = month.AddDays(-1);
                FromDate = first;
                ToDate = last;
            }

            if (radioButton3.Checked == true)
            {
                WAction = 3;  // Current Month = 3
                WAccName = comboBox1.Text;
                WAccCurr = comboBox2.Text; 
                var today = DateTime.Today;
                //TEST
                today = WorkingToday;
                var month = new DateTime(today.Year, today.Month, 1);
                FromDate = month;
                ToDate = DateTime.Today;
                //TEST
                ToDate = WorkingToday; 
            }

            if (radioButton4.Checked == true)
            {
                WAction = 4;  // Selected Dates = 4
                WAccName = comboBox1.Text;
                WAccCurr = comboBox2.Text; 
                FromDate = dateTimePicker1.Value;
                ToDate = dateTimePicker2.Value;

                if (ToDate < FromDate)
                {
                    MessageBox.Show("Please check dates. To date is less than from date");
                    return; 
                }
            }

            NForm31 = new Form31(WSignedId, WSignRecordNo, WOperator, WCitId, WAccName, WAccCurr, WAction, 
                    FromDate, ToDate, "" );
                NForm31.ShowDialog();            
            
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

            dataGridView1.Columns[0].Width = 50; // User Id/ CitID
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 110; // User Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 170; //  email 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Columns[3].Width = 150; // Mobile
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 120; // date Open
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 70; // User Type
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 80; // Cit Id 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGrid2()
        {
            dataGridView2.DataSource = Ua.UsersToAtmsDataTable.DefaultView;


            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("No available ATMs or Groups.");
               // this.Dispose();
                return;
            }

            dataGridView2.Columns[0].Width = 50; // User Id
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 70; // Atm no
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 250; //  ATM Name 
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 50; //  Group of ATMs
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[3].Visible = false;

            dataGridView2.Columns[4].Width = 80; // Replenishment
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[5].Width = 80; // Reconciliation 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[6].Width = 130; // Date of insert 
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Finish 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

        // Create CIT Excel 
        // Create a new Orders Cycle
     
        private void buttonCreateCITExcel_Click(object sender, EventArgs e)
        {
            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();

            string Function = "ATMsInNeed";
            // 
            // Create Orders
            //
            CreateNewOrdersCycle(WCitId, Function); 

            // If Cycle Exist check if Authorisation process exist
            //
            string WSelectionCriteria = " Where CitId='" + WCitId + "' AND OrdersFunction='" + Function + "' AND ProcessStage != 3 ";
            Coc.ReadExcelOutputCyclesBySelectionCriteria(WSelectionCriteria);
            if (Coc.RecordFound == true)
            {
                // There is an open Cycle
                // Check LAST RECORD if Already in authorization process

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCitId, Coc.SeqNo, "ReplOrders"); //
                if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
                {
                    MessageBox.Show("This Order Process Already has authorization record!" + Environment.NewLine
                                             + "Go to Pending Authorisations process to complete.");

                    return;
                }

            }

            // STEPLEVEL
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 2; 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form273_AUDI NForm273_AUDI;
          
            NForm273_AUDI = new Form273_AUDI(WSignedId, WSignRecordNo, WOperator, WCitId, WOrdersCycle, Function);
            NForm273_AUDI.ShowDialog();
        }

        //
        // METHOD THAT IS NEEDED FOR ORDERS
        //

        int WProcessStage;
        DateTime WOutputDate;
        int WOrdersCycle; 

        private void CreateNewOrdersCycle(string InCitId, string InFunction)
        {
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();
            RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();
            // If not already exist and Open create a new one
            string WSelectionCriteria = " Where CitId='" + WCitId + "' AND OrdersFunction='" + InFunction + "' AND ProcessStage != 3 ";
            Coc.ReadExcelOutputCyclesBySelectionCriteria(WSelectionCriteria);
            if (Coc.RecordFound == true)
            {
                // There is an open Cycle
                WOrdersCycle = Coc.SeqNo;
                WProcessStage = Coc.ProcessStage;
                WOutputDate = Coc.CreatedDateTm;

                // Delete all orders for this Open Cycle

                Ro.DeleteReplOrderForThisCycle(WOrdersCycle);

                // Delete from ReplOrders_Pre_ATMs
                RRDMReplOrders_Pre_ATMs Rpre = new RRDMReplOrders_Pre_ATMs();
                Rpre.DeleteReplOrders_Pre_ATMs(WOrdersCycle);
            }
            else
            {
                // Create a new cycle

                Coc.CitId = WCitId;
                Coc.Description = "Orders Cycle for CIT.." + WCitId;
                Coc.OrdersFunction = InFunction;
                Coc.CreatedDateTm = WOutputDate = DateTime.Now;
                Coc.MakerId = WSignedId;
                Coc.ProcessStage = 1;
                Coc.Operator = WOperator;

                WOrdersCycle = Coc.InsertExcelOutputCycle();

                WProcessStage = 1;

            }
        }

        // Output Excel Cycles
        private void buttonCreatedExcelCycles_Click(object sender, EventArgs e)
        {
            Form18_CIT_ExcelOutputCycles NForm18_CIT_ExcelOutputCycles;

            int Mode = 2; // 

            NForm18_CIT_ExcelOutputCycles = new Form18_CIT_ExcelOutputCycles(WSignedId, WSignRecordNo, WOperator, WCitId);
            NForm18_CIT_ExcelOutputCycles.ShowDialog();
        }
/// <summary>
/// ORDERS ALERTS
/// </summary>

        private void buttonOrdersAlerts_Click(object sender, EventArgs e)
        {
            Form18_CIT_ExcelOutput_Alerts NForm18_CIT_ExcelOutput_Alerts;

            int Mode = 2; // 

            NForm18_CIT_ExcelOutput_Alerts = new Form18_CIT_ExcelOutput_Alerts(WSignedId, WSignRecordNo, WOperator, WCitId);
            NForm18_CIT_ExcelOutput_Alerts.ShowDialog();
        }
    }
}


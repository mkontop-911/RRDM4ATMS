using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form21MISITMX : Form
    {
      
        bool ITMXUser;

        bool CentralBank;

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        //TEST RANGE 
        DateTime WorkingDtA = new DateTime(2016, 06, 24);
     
        //multilingual
        CultureInfo culture;

        RRDMUsersRecords Us = new RRDMUsersRecords(); // Make class availble 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMBanks Ba = new RRDMBanks();

        RRDMITMX_DailyBankTotals Bt = new RRDMITMX_DailyBankTotals(); 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        string WCurrNm;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        int WAction;

        public Form21MISITMX(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            WAction = InAction;  // 1 = By Period Quality of Repl and Reconc, 
                                 // 2 = By User performance , 
                                 // 3 = By ATMs Daily Business 
                                 // 4 = Cit Analysis
                                 // 5 = Dispute Analysis 
                                 // 6 = By ATM Profitability 
                                 // 11 = ITMX

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            labelUserId.Text = WSignedId;
            pictureBox1.BackgroundImage = appResImg.logo2;
            
            dateTimePicker2.Value = WorkingDtA;

            Gp.ParamId = "752"; // Products 
            comboBoxProducts.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxProducts.DisplayMember = "DisplayValue";

            Us.ReadUsersRecord(WSignedId);
            Ba.ReadBank(Us.BankId);
            //
            // THIS FORM IS ALLOWED ONLY FOR THE SETTLEMENT ENTITY 
            // 
            if (Us.Operator == Us.BankId)
            {
                ITMXUser = true;

                //labelStep1.Text = "Settlement Transactions Creation";
                //textBoxMsgBoard.Text = "Select Settlement Cycle to Create Txns ";
            }
            else
            {
                ITMXUser = false;

                labelStep1.Text = "Turnover For : " + Us.BankId;

                textBoxMsgBoard.Text = "Make Selection for your Bank. ";

                if (Us.BankId == "CBT")
                {
                    CentralBank = true; 
                }
                else
                {
                    radioButton4.Enabled = false;
                }
            }

            label14.Hide();
            panel2.Hide();

            radioButton1.Checked = true;
            radioButton3.Checked = true;

        }

        private void Form21MIS_Load(object sender, EventArgs e)
        {
            // Show Banks 
            ShowGrid1(WOperator);
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.Culture == "English")
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (Usi.Culture == "Français")
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }

            //      if (WMode == 1) label14.Text = LocRM.GetString("Form67label14a", culture);
            //      if (WMode == 2) label14.Text = LocRM.GetString("Form67label14b", culture);

            textBoxMsgBoard.Text = "Click on Graph to Enlarge";

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            WCurrNm = Ba.BasicCurName;

        }

        //SHOW
        private void buttonShow_Click(object sender, EventArgs e)
        {
            if (WAction == 11)
            {
                if (radioButton2.Checked == true)
                {
                    PerMonth = true; 
                }
                else
                {
                    PerMonth = false;
                }

                if (radioButton4.Checked == true)
                {
                    WBankId = "ALLBANKS"; 
                }

                DateTime FromDtTm = dateTimePicker2.Value;
                DateTime ToDtTm = dateTimePicker3.Value;

                if (PerMonth == false) ShowAction11Daily(WOperator, WBankId, FromDtTm, ToDtTm);

                if (PerMonth == true) ShowAction11Monthly(WOperator, WBankId, FromDtTm, ToDtTm);

            }
        }

        //******************
        // SHOW GRID dataGridView1
        //******************

        bool AllBanks;
        //string SqlString;
        bool PerMonth; 
        private void ShowAction11Daily(string Operator, string InBankId, DateTime InFromDtTm, DateTime InToDtTm)
        {
            // By Period Turnover for specific Bank 
          
            if (InBankId == "ALLBANKS")
            {
                AllBanks = true; 
                labelStep1.Text = "Daily TurnOver For All Banks";
            }
            else
            {
                AllBanks = false;
                labelStep1.Text = "Daily Turnover for Bank " + InBankId;
            }
            
            labelChart1.Text = "TXNS BY DAY";
            labelChart2.Text = "AMOUNT BY DAY";
            labelChart3.Text = "UNMATCHED BY DAY";
            labelChart4.Text = "FEES BY DAY";

            Bt.ReadTableTotalsForDaily(Operator, WSignedId ,InBankId,
                                   InFromDtTm, InToDtTm, AllBanks, PerMonth);       

            dataGridView1.DataSource = Bt.MISITMXTable.DefaultView;
            //  MessageBox.Show(" Number of ATMS = " + I);
            if (dataGridView1.RowCount==0)
            {
                MessageBox.Show("No Data For this Selection. Check Dates Range");
                label14.Hide();
                panel2.Hide();
                return; 
            }
            else
            {
                label14.Show();
                panel2.Show();
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();

            style.Format = "N2";

            dataGridView1.Columns[0].Width = 70; // Date
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 65; // TNXS
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 75; // Amount
            dataGridView1.Columns[2].DefaultCellStyle = style;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[3].Width = 65; // Avg_Amnt
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[3].DefaultCellStyle = style;

            dataGridView1.Columns[4].Width = 65; // UnMatched
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 75; // TxnsFees
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 65; // Fees
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].DefaultCellStyle = style;

            dataGridView1.Columns[7].Width = 85; // Avg_FeesAmnt
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].DefaultCellStyle = style;

            //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
            //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
            //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


            dataGridView1.Show();

            //// Set chart1 data source  
            chart1.DataSource = Bt.MISITMXTable.DefaultView;
            chart1.Series[0].Name = "TNXS";
       
            //// Set series members names for the X and Y values  
            chart1.Series[0].XValueMember = "Date";
            chart1.Series[0].YValueMembers = "TNXS";

            //// Data bind to the selected data source  
            chart1.DataBind();

            // Set chart2 data source  
            chart2.DataSource = Bt.MISITMXTable.DefaultView;
            chart2.Series[0].Name = "Amount";
            // Set series members names for the X and Y values  
            chart2.Series[0].XValueMember = "Date";
            chart2.Series[0].YValueMembers = "Amount";

            // Data bind to the selected data source  
            chart2.DataBind();

            chart2.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Chocolate;

            // Set chart3 data source  
            chart3.DataSource = Bt.MISITMXTable.DefaultView;
            chart3.Series[0].Name = "UnMatched";
            // Set series members names for the X and Y values  
            chart3.Series[0].XValueMember = "Date";
            chart3.Series[0].YValueMembers = "UnMatched";

            // Data bind to the selected data source  
            chart3.DataBind();

            chart3.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;

            // Set chart2 data source  
            chart4.DataSource = Bt.MISITMXTable.DefaultView;
            chart4.Series[0].Name = "Fees";
            // Set series members names for the X and Y values  
            chart4.Series[0].XValueMember = "Date";
            chart4.Series[0].YValueMembers = "Fees";

            // Data bind to the selected data source  
            chart4.DataBind();

            chart4.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;

        }
//Monthly 
        private void ShowAction11Monthly(string Operator, string InBankId, DateTime InFromDtTm, DateTime InToDtTm)
        {
            // By Period Turnover for specific Bank 

            if (InBankId == "ALLBANKS")
            {
                AllBanks = true;
                labelStep1.Text = "Monthly TurnOver For All Banks";
            }
            else
            {
                AllBanks = false;
                labelStep1.Text = "Monthly Turnover for Bank " + InBankId;
            }

            labelChart1.Text = "TXNS BY MONTH";
            labelChart2.Text = "AMOUNT BY MONTH";
            labelChart3.Text = "UNMATCHED BY MONTH";
            labelChart4.Text = "FEES BY MONTH";


            Bt.ReadTableTotalsForMonthly(Operator, WSignedId, InBankId,
                                  InFromDtTm, InToDtTm, AllBanks, PerMonth);

          
            dataGridView1.DataSource = Bt.MISITMXTable.DefaultView;

            //  MessageBox.Show(" Number of ATMS = " + I);
            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("No Data For this Selection. Check Dates Range");
                label14.Hide();
                panel2.Hide();
                return;
            }
            else
            {
                label14.Show();
                panel2.Show();
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();

            style.Format = "N2";

            dataGridView1.Columns[0].Width = 70; // Year
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; // Month
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 65; // TNXS
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 75; // Amount
            dataGridView1.Columns[3].DefaultCellStyle = style;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[4].Width = 65; // Avg_Amnt
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].DefaultCellStyle = style;

            dataGridView1.Columns[5].Width = 65; // UnMatched
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 75; // TxnsFees
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 65; // Fees
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].DefaultCellStyle = style;

            dataGridView1.Columns[8].Width = 85; // Avg_FeesAmnt
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[8].DefaultCellStyle = style;

            //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
            //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
            //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


            dataGridView1.Show();

            //// Set chart1 data source  
            chart1.DataSource = Bt.MISITMXTable.DefaultView;
            chart1.Series[0].Name = "TNXS";
    
            //// Set series members names for the X and Y values  
            chart1.Series[0].XValueMember = "perMonth";
            chart1.Series[0].YValueMembers = "TNXS";
         


            //// Data bind to the selected data source  
            chart1.DataBind();

            // Set chart data source  
            chart2.DataSource = Bt.MISITMXTable.DefaultView;
            chart2.Series[0].Name = "Amount";
            // Set series members names for the X and Y values  
            chart2.Series[0].XValueMember = "perMonth";
            chart2.Series[0].YValueMembers = "Amount";

            // Data bind to the selected data source  
            chart2.DataBind();

            chart2.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Chocolate;

            // Set chart3 data source  
            chart3.DataSource = Bt.MISITMXTable.DefaultView;
            chart3.Series[0].Name = "UnMatched";
            // Set series members names for the X and Y values  
            chart3.Series[0].XValueMember = "perMonth";
            chart3.Series[0].YValueMembers = "UnMatched";

            // Data bind to the selected data source  
            chart3.DataBind();

            chart3.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;

            // Set chart2 data source  
            chart4.DataSource = Bt.MISITMXTable.DefaultView;
            chart4.Series[0].Name = "Fees";
            // Set series members names for the X and Y values  
            chart4.Series[0].XValueMember = "perMonth";
            chart4.Series[0].YValueMembers = "Fees";

            // Data bind to the selected data source  
            chart4.DataBind();

            chart4.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
        }
       

        //******************
        // SHOW GRID dataGridView1
        //******************
        int WRowGrid1;
        private void ShowGrid1(string Operator)
        {
            // Keep Scroll position 
            int scrollPosition = 0;

            string SelectionCriteria;
            if (ITMXUser == true || CentralBank == true)
            {
                Ba.ReadBanksForDataTableByOperator(WOperator, 1);
            }
            else
            {
                Ba.ReadBanksForDataTableByBankId(WOperator, Us.BankId, 1);
            }

          
            dataGridViewBanks.DataSource = Ba.BanksDataTable.DefaultView;

            if (dataGridViewBanks.Rows.Count == 0)
            {
                return;
            }
            else
            {
                WRowGrid1 = dataGridViewBanks.SelectedRows[0].Index;

                if (WRowGrid1 > 0)
                {
                    scrollPosition = dataGridViewBanks.FirstDisplayedScrollingRowIndex;
                }
            }

            dataGridViewBanks.Columns[0].Width = 90;
            dataGridViewBanks.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridViewBanks.Columns[1].Width = 70;
            //dataGridViewBanks.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewBanks.Columns[1].Visible = false;

            dataGridViewBanks.Columns[2].Width = 300;
            dataGridViewBanks.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewBanks.Columns[3].Width = 100;
            dataGridViewBanks.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewBanks.Rows[WRowGrid1].Selected = true;
            dataGridViewBanks_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridViewBanks.FirstDisplayedScrollingRowIndex = scrollPosition;

        }
        // Banks Row Enter 
        string WBankId;
        private void dataGridViewBanks_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewBanks.Rows[e.RowIndex];

            WBankId = (string)rowSelected.Cells[0].Value;

            Ba.ReadBank(WBankId);
        }

        // A ROW IS SELECTED ASSIGN VALUES 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

        }
        // GO TO EN-LARGE
        int WPeriod;
        private void chart1_Click_1(object sender, EventArgs e)
        {
            string Heading;

            if (WAction == 11) // By Period 
            {
                Form35ITMX NForm35ITMX;
            
                Heading = labelChart1.Text;

                int WGraph = 1;
                
                if (PerMonth == false) WPeriod = 1;
                if (PerMonth == true) WPeriod = 2;

                NForm35ITMX = new Form35ITMX(WSignedId, WSignRecordNo, WOperator, Bt.MISITMXTable, Heading, WAction, WGraph, WPeriod);
                NForm35ITMX.Show();
                //NForm35ITMX.BringToFront();

            }
        }

        private void chart2_Click_1(object sender, EventArgs e)
        {
            string Heading;
            if (WAction == 11) // By Period 
            {
                Form35ITMX NForm35ITMX;
              
                Heading = labelChart2.Text;

                int WGraph = 2;

                if (PerMonth == false) WPeriod = 1;
                if (PerMonth == true) WPeriod = 2;

                NForm35ITMX = new Form35ITMX(WSignedId, WSignRecordNo, WOperator, Bt.MISITMXTable, Heading, WAction, WGraph, WPeriod);
                NForm35ITMX.Show();
                //NForm35ITMX.BringToFront();

            }
        }
        // Monthly 
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //if (radioButton2.Checked == true)
            //{
            //    MessageBox.Show("There are no testing data for monthly");
            //    radioButton1.Checked = true;
            //    radioButton2.Checked = false;
            //}
        }
        //All Banks 
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked == true)
            {
                dataGridViewBanks.Hide();
            }
            else
            {
                dataGridViewBanks.Show();
            }
        }

        private void chart3_Click(object sender, EventArgs e)
        {
            string Heading;
            if (WAction == 11) // By Period 
            {
                Form35ITMX NForm35ITMX;
            
                Heading = labelChart3.Text;

                int WGraph = 3;

                if (PerMonth == false) WPeriod = 1;
                if (PerMonth == true) WPeriod = 2;

                NForm35ITMX = new Form35ITMX(WSignedId, WSignRecordNo, WOperator, Bt.MISITMXTable, Heading, WAction, WGraph, WPeriod);
                NForm35ITMX.Show();
                //NForm35ITMX.BringToFront();

            }
        }

        private void chart4_Click(object sender, EventArgs e)
        {
            string Heading;
            if (WAction == 11) // By Period 
            {
                Form35ITMX NForm35ITMX;
             
                Heading = labelChart4.Text;

                int WGraph = 4;

                if (PerMonth == false) WPeriod = 1;
                if (PerMonth == true) WPeriod = 2;

                NForm35ITMX = new Form35ITMX(WSignedId, WSignRecordNo, WOperator, Bt.MISITMXTable, Heading, WAction, WGraph, WPeriod);
                NForm35ITMX.Show();
                //NForm35ITMX.BringToFront();

            }
        }
//Finish
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
//Print 
        private void button6_Click(object sender, EventArgs e)
        {

            DateTime FromDtTm = dateTimePicker2.Value;
            DateTime ToDtTm = dateTimePicker3.Value;
            if (PerMonth == false)
            {
                // Call and print report 
                string P1 = labelStep1.Text;
                string P2 = FromDtTm.Date.ToString();
                string P3 = ToDtTm.Date.ToString();
                string P4 = Us.BankId;
                string P5 = WSignedId;

                Form56R44ITMX ReportITMX44 = new Form56R44ITMX(P1, P2, P3, P4, P5);
                ReportITMX44.Show();
            }
            if (PerMonth == true)
            {
                // Call and print report 
                string P1 = labelStep1.Text;
                string P2 = FromDtTm.Date.ToString();
                string P3 = ToDtTm.Date.ToString();
                string P4 = Us.BankId;
                string P5 = WSignedId;

                Form56R45ITMX ReportITMX45 = new Form56R45ITMX(P1, P2, P3, P4, P5);
                ReportITMX45.Show();
            }

        }
    }
}


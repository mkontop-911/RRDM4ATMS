using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form202ITMX : Form
    {
        RRDMITMXSettlementCycles Sc = new RRDMITMXSettlementCycles();
        RRDMITMXSettlementBanksToBanksFTCycleTotals BBT = new RRDMITMXSettlementBanksToBanksFTCycleTotals();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMBanks Ba = new RRDMBanks(); 
     
        int WITMXSettlementCycle;
        //string WBankA;

        bool Start;

        int WMax;

        bool ClearingBank; 

        string WBankId; 

        int WRowGrid1;
        //int WRowGrid3;

        int FromCycleNo;
        int ToCycleNo;

        bool ITMXUser; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        public Form202ITMX(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            Us.ReadUsersRecord(WSignedId);
         
            if (Us.Operator == Us.BankId)
            {
                ITMXUser = true;
                labelStep1.Text = "Banks Settlement Statement ";
                textBoxMsgBoard.Text = "Select Bank And Settlement Cycle. ";
            }
            else
            {
                ITMXUser = false;

                labelStep1.Text = "Settlement Statement For : " + Us.BankId;

                textBoxMsgBoard.Text = "Select Corresponding Bank And Settlement Cycle. ";

                Ba.ReadBank(Us.BankId);
                if (Ba.SettlementBank == true)
                {
                    ClearingBank = true;
                    labelStep1.Text = "Banks Settlement Statement ";
                    textBoxMsgBoard.Text = "Select Bank And Settlement Cycle. ";
                }
                else
                {
                    ClearingBank = false;
                }
            }

            //Sc.ReadITMXDailySettlementByCategory(); 

            Sc.ReadITMXDailySettlementToFindTotals(WOperator);

            Start = true;

            WMax = Sc.ITMXSettlementCycle;
            textBox1.Text = (WMax-3).ToString();
            textBox2.Text = WMax.ToString();
           
            Start = false;

        }
        private void Form202ITMX_Load(object sender, EventArgs e)
        {
            ShowGrid1(WOperator);
        }
      

        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WBankId = (string)rowSelected.Cells[0].Value;

            Ba.ReadBank(WBankId);

        
            //Validation of Input

            if (int.TryParse(textBox1.Text, out FromCycleNo))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid number in the From value!");
                return;
            }

            if (int.TryParse(textBox2.Text, out ToCycleNo))
            {
            }
            else
            {
                MessageBox.Show(textBox2.Text, "Please enter a valid number in the To value!");
                return;
            }

            if (ToCycleNo < FromCycleNo)
            {
                MessageBox.Show("ToCycleNo is less than FromCycleNo"); 
            }

            if (ToCycleNo > WMax)
            {
                MessageBox.Show("ToCycleNo is less than FromCycleNo");
            }

            //Sc.ReadITMXSettlementCyclesById(WOperator, WITMXSettlementCycle);

            // Selected Files (TWO)

            ShowGrid2(WBankId,FromCycleNo,ToCycleNo);

        }
        // Row enter for Category vs Source files 
        private void dataGridView2_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WITMXSettlementCycle = (int)rowSelected.Cells[0].Value;

            Sc.ReadITMXSettlementCyclesById(WOperator, WITMXSettlementCycle);
          
        }     
      

        // FINISH
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// Go to Next  

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGrid1(string Operator)
        {
            // Keep Scroll position 
            int scrollPosition = 0;
           

            //string SelectionCriteria;
            if (ITMXUser == true || ClearingBank == true)
            {
               
                Ba.ReadBanksForDataTableByOperator(WOperator, 1);

            }
            else
            {
               
                Ba.ReadBanksForDataTableByBankId(WOperator, Us.BankId, 1); 
            }
           

            dataGridView1.DataSource = Ba.BanksDataTable.DefaultView;

            //Sc.ReadITMXSettlementCyclesFillTable(WOperator); 


            //this.reconcSourceFilesTableAdapter.Fill(this.aTMSDataSet63.ReconcSourceFiles);

            //dataGridView1.DataSource = Sc.TableITMXDailyJobCycles.DefaultView;


            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }
            else
            {
                WRowGrid1 = dataGridView1.SelectedRows[0].Index;

                if (WRowGrid1 > 0)
                {
                    scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
                }
            }

            dataGridView1.Columns[0].Width = 90; 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 300; 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 100; 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ////dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

        }

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGrid2(string InBankId, int InFromCycleNo, int ToCycleNo)
        {
            if (ITMXUser == true || ClearingBank == true)
            {

                label3.Text = "SETTLEMENT CYCLES TOTALS FOR : " + InBankId; 
            }
            else
            {
                label3.Text = "SETTLEMENT CYCLES TOTALS " ;
            }

            BBT.ReadTableTotalsForABankOverAllSettlements(WOperator, WSignedId, InBankId, InFromCycleNo, ToCycleNo);
            
            dataGridView2.DataSource = BBT.TableTotalsForBank.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }
            dataGridView2.Columns[0].Width = 90; // settlement cycle
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 75; // date
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 85; // DebitedAmount
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[3].Width = 85; // CreditedAmount
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[4].Width = 80; // Difference
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Set chart2 data source  
            chart2.DataSource = BBT.TableTotalsForBank.DefaultView;
            chart2.Series[0].Name = "Difference";
            //chart2.Series.Add("TotErr");
            //chart2.Series.Add("NotReconc");
            // Set series members names for the X and Y values  
            chart2.Series[0].XValueMember = "SettlementCycle";
            chart2.Series[0].YValueMembers = "Difference";
            //chart2.Series[1].XValueMember = "Date";
            //chart2.Series[1].YValueMembers = "TotErr";
            //chart2.Series[2].XValueMember = "Date";
            //chart2.Series[2].YValueMembers = "NotReconc";

            //Data bind to the selected data source
            chart2.DataBind();

            //dataGridView2.Columns[3].Visible = false; // ProcessMode
            //dataGridView2.Columns[4].Visible = false; // LastInFileDtTm
            //dataGridView2.Columns[5].Visible = false; // LastMatchingDtTm

            // DATA TABLE ROWS DEFINITION 
            //" SELECT BankA AS BANKA, SUM(DRAmount) AS DebitedAmount, SUM(CRAmount)AS CreditedAmount, "
            //       + " SUM(DRAmount - CRAmount) AS Diff "
        }

        
//Print NET Position 
        private void button6_Click(object sender, EventArgs e)
        {
            // Assign Parameters

            // Call and print report 
           
            string P1 = WBankId;
            string P2 = FromCycleNo.ToString();
            string P3 = ToCycleNo.ToString();
            string P4 = Us.BankId;
            string P5 = "PER CYCLE NET POSITION FOR BANK : ";
            string P6 = WSignedId ;

            Form56R30ITMX ReportITMX30 = new Form56R30ITMX(P1, P2, P3, P4, P5, P6);
            ReportITMX30.Show();
        }
   
// Show
        private void button3_Click(object sender, EventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form202ITMX_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
            button3.Hide(); 
        }
// From Changes 
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Start == true)
            {

            }
            else
            {
                button3.Show();
            }
            
        }
// To Changes 
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (Start == true)
            {

            }
            else
            {
                button3.Show();
               
            }
            
        }
    }
}

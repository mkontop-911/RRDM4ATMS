using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form201ITMXFEES : Form
    {
        RRDMITMXSettlementCycles Sc = new RRDMITMXSettlementCycles();

        RRDMITMXSettlementBanksToBanksFTFeesCycleTotals SFT = new RRDMITMXSettlementBanksToBanksFTFeesCycleTotals();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMBanks Ba = new RRDMBanks();  

 
        int WITMXSettlementCycle;
        string WFeesEntity; 

        int WRowGrid1;
        int WRowGrid3;

        bool ITMXUser;
        bool SettlementUser; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        public Form201ITMXFEES(string InSignedId, int SignRecordNo, string InOperator)
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
                labelStep1.Text = "Settlement FEES Cycles Totals ";
                textBoxMsgBoard.Text = "Select Settlement Cycle and Bank. ";
            }
            else
            {
                ITMXUser = false;
                Ba.ReadBank(Us.BankId);
                if (Ba.SettlementBank == true)
                {
                    SettlementUser = true; 
                    button6.Show();
                    labelStep1.Text = "Settlement FEES Cycles Totals ";
                    textBoxMsgBoard.Text = "Select Settlement Cycle and Bank. ";
                }
                else
                {
                 
                    labelStep1.Text = "Settlement FEES Cycles Totals For : " + Us.BankId;

                    button6.Hide();

                    textBoxMsgBoard.Text = "Select Settlement Cycle. ";
                }
            }

        }

        private void Form201ITMX_Load(object sender, EventArgs e)
        {
         
            ShowGridSettlementCycles(WOperator);
        }

        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

           WITMXSettlementCycle = (int)rowSelected.Cells[0].Value;

           Sc.ReadITMXSettlementCyclesById(WOperator, WITMXSettlementCycle);

            // Selected Files (TWO)

            ShowGridTotalsForBanks(WITMXSettlementCycle);

        }
        // Row enter for Category vs Source files 
        private void dataGridView2_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WFeesEntity = (string)rowSelected.Cells[0].Value;

            // Grid  (THREE)
            ShowGridPairsForBankA(WITMXSettlementCycle, WFeesEntity);
        }     
       
        // Row Enter For Matching fields 
        private void dataGridView3_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            labelBankA.Text = (string)rowSelected.Cells[0].Value;

            labelBankB.Text = (string)rowSelected.Cells[1].Value;
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
        private void ShowGridSettlementCycles(string Operator)
        {
            // Keep Scroll position 
            
            int scrollPosition = 0;
          
            Sc.ReadITMXSettlementCyclesFillTable(WOperator); 
     
            //this.reconcSourceFilesTableAdapter.Fill(this.aTMSDataSet63.ReconcSourceFiles);

            dataGridView1.DataSource = Sc.TableITMXDailyJobCycles.DefaultView;

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

            dataGridView1.Columns[0].Width = 90; //SettlementCycle
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 90; // Category
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView1.Columns[2].Width = 110; // StartedDate
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 100; // Description
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ////dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            // DATA TABLE ROWS DEFINITION 
            //TableITMXDailyJobCycles.Columns.Add("ITMXSettlementCycle", typeof(int));
            //TableITMXDailyJobCycles.Columns.Add("Category", typeof(string));

            //TableITMXDailyJobCycles.Columns.Add("StartedDate", typeof(DateTime));
            //TableITMXDailyJobCycles.Columns.Add("Description", typeof(string));
        }

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGridTotalsForBanks(int InRunningCycle)
        {
            if (ITMXUser == true || SettlementUser == true)
            {
                SFT.ReadTableTotalsForAllFeesEntities(WOperator, WSignedId, WITMXSettlementCycle, "");
            }
            else
            {
                SFT.ReadTableTotalsForAllFeesEntities(WOperator, WSignedId, WITMXSettlementCycle, Us.BankId);
            }
            
            dataGridView2.DataSource = SFT.TableTotalsForAllFeesEntities.DefaultView;
            
            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }
            dataGridView2.Columns[0].Width = 70; // FeesEntity
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 50; // SettlementCycle
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[1].Visible = false;

            dataGridView2.Columns[2].Width = 90; // Date
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[3].Width = 90; // DebitAmount
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[4].Width = 90; // CreditAmount
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[5].Width = 90; // NetFees
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            //dataGridView2.Columns[3].Visible = false; // ProcessMode
            //dataGridView2.Columns[4].Visible = false; // LastInFileDtTm
            //dataGridView2.Columns[5].Visible = false; // LastMatchingDtTm

            
        }

        //******************
        // SHOW GRID dataGridView3
        //******************
        private void ShowGridPairsForBankA(int InSettlenetCycle, string InFeesEntity)
        {
            // Keep Scroll position 
           
            int scrollPosition = 0;

            SFT.ReadTableFeesTotalsForFeesEntity(WOperator, WSignedId, InSettlenetCycle, InFeesEntity); 
     
            dataGridView3.DataSource = SFT.TableTotalsFeesForBank.DefaultView;

            if (dataGridView3.Rows.Count == 0)
            {
                return;
            }
            else
            {
                // Keep Scroll position 
                WRowGrid3 = dataGridView3.SelectedRows[0].Index;

                if (WRowGrid3 > 0)
                {
                    scrollPosition = dataGridView3.FirstDisplayedScrollingRowIndex;
                }
            }

            labelPair.Text = "TOTALS FOR Entity : " + InFeesEntity + " WITH OTHER Entities" ;

            dataGridView3.Columns[0].Width = 60; // FeesEntity 
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[1].Width = 60; // BankA
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[2].Width = 60; // BankB
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[3].Width = 70; // DRAmount
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[4].Width = 70; // CRAmount
            dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[5].Width = 70; // NetFees 
            dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[6].Width = 115; // DateTmCreated
            dataGridView3.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[7].Width = 100; // SettlementCycle
            dataGridView3.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            dataGridView3.Rows[WRowGrid3].Selected = true;
            dataGridView3_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid3));

            dataGridView3.FirstDisplayedScrollingRowIndex = scrollPosition;

            // DATA TABLE ROWS DEFINITION 
            //TableTotalsFeesForBank.Columns.Add("FeesEntity", typeof(string));
            //TableTotalsFeesForBank.Columns.Add("BankA", typeof(string));
            //TableTotalsFeesForBank.Columns.Add("BankB", typeof(string));
            //TableTotalsFeesForBank.Columns.Add("DRAmount", typeof(decimal));
            //TableTotalsFeesForBank.Columns.Add("CRAmount", typeof(decimal));
            //TableTotalsFeesForBank.Columns.Add("NetFees", typeof(decimal));
            //TableTotalsFeesForBank.Columns.Add("DateTmCreated", typeof(DateTime));
            //TableTotalsFeesForBank.Columns.Add("SettlementCycle", typeof(int));
        }
        //Print Lines of Banks Debits and Credits 
        private void button6_Click(object sender, EventArgs e)
        {
            // Assign Parameters

            // Call and print report 
            string P1 = WITMXSettlementCycle.ToString();
            string P2 = Sc.CreatedDate.ToString();
            string P3 = Us.BankId;
            string P4 = WSignedId;

            Form56R41ITMX ReportITMX41 = new Form56R41ITMX(P1, P2, P3, P4);
            ReportITMX41.Show();
        }
        
        //Print Lines for Single Bank  
        private void button2_Click(object sender, EventArgs e)
        {
            string P1 = WITMXSettlementCycle.ToString();
            string P2 = Sc.CreatedDate.ToString();
            string P3 = WFeesEntity;
            string P4 = Us.BankId;
            string P5 = WSignedId;

            Form56R42ITMX ReportITMX42 = new Form56R42ITMX(P1, P2, P3, P4, P5);
            ReportITMX42.Show();
        }
// Print Line Transactions 
        private void button1_Click(object sender, EventArgs e)
        {
            string P1 = WITMXSettlementCycle.ToString();
            string P2 = Sc.CreatedDate.ToString();
            string P3 = WFeesEntity;
            string P4 = labelBankB.Text;
            string P5 = Us.BankId;
            string P6 = WSignedId; 

            if (WFeesEntity =="BBL" & labelBankB.Text == "KBANK")
            {
                //OK
            }
            else
            {
                MessageBox.Show("Please select pair BBL Vs KBANK. We have testing data only for this pair.");
                return; 
            }

            Form56R33ITMX ReportITMX33 = new Form56R33ITMX(P1, P2, P3, P4, P5, P6);
            ReportITMX33.Show();
        }

    }
}

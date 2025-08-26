using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form291NV_E_FIN_Central : Form
    {
        //RRDMITMXSettlementCycles Sc = new RRDMITMXSettlementCycles();
        RRDMITMXSettlementBanksToBanksFTCycleTotals BBT = new RRDMITMXSettlementBanksToBanksFTCycleTotals();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMBanks Ba = new RRDMBanks();

        RRDMNVCardsBothAuthorAndSettlement Sec = new RRDMNVCardsBothAuthorAndSettlement();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        string WJobCategory = "E_FINANCE Reconciliation";

        int W_E_FIN_Cycle;

        int WRowGrid1;
   
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        public Form291NV_E_FIN_Central(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            Us.ReadUsersRecord(WSignedId);
       
        }

        private void Form291NV_E_FIN_Central_Load(object sender, EventArgs e)
        {         
            ShowGridSettlementCycles(WOperator);
        }
        DateTime WCycleDate;  
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            W_E_FIN_Cycle = (int)rowSelected.Cells[0].Value;

            Rjc.ReadReconcJobCyclesById(WOperator, W_E_FIN_Cycle);

            WCycleDate = Rjc.Cut_Off_Date.Date; 

            // Branches totals 

            ShowGridTotalsForBranches(W_E_FIN_Cycle);

        }
        // Row enter for Category vs Source files 
        private void dataGridView2_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            String WCategoryId = (string)rowSelected.Cells[0].Value;

            // Grid  (THREE)
          //  ShowGridPairsForBankA(W_E_FIN_Cycle, WBankA);
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
            int scrollPosition = 0;

            string WSelectionCriteria = " WHERE JobCategory ='" + WJobCategory + "'";
            Rjc.ReadReconcJobCyclesFillTable(WSelectionCriteria);
        
            //this.reconcSourceFilesTableAdapter.Fill(this.aTMSDataSet63.ReconcSourceFiles);

            dataGridView1.DataSource = Rjc.TableReconcJobCycles.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("NO CYCLES TO SHOW!"); 
                return;
            }
            else
            {
                // Keep Scroll position 
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
        // Branch Totals
        //******************
        private void ShowGridTotalsForBranches(int InRunningCycle)
        {
            Sec.ReadNV_E_FIN_Branches_Totals(WOperator, WSignedId, InRunningCycle); 

            dataGridView2.DataSource = Sec.TableBranchesTotals.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("No Branch Totals for this Cycle");
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView2.Columns[0].Width = 70; // Category / Branch Id
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 70; // 
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 90; // Matched
            dataGridView2.Columns[2].DefaultCellStyle = style;
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[2].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[3].Width = 70; // 
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 90; // 
            dataGridView2.Columns[4].DefaultCellStyle = style;
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[5].Width = 40; // 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[6].Width = 70; // 
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[7].Width = 90; // 
            dataGridView2.Columns[7].DefaultCellStyle = style;
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[7].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[8].Width = 70; // 
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[9].Width = 90; // 
            dataGridView2.Columns[9].DefaultCellStyle = style;
            dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[9].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[9].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            // SHOW TOTALS
            ShowGrandTotals(InRunningCycle); 
        }

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGrandTotals(int InRunningCycle)
        {
            labelBankGrandTotals.Text = "BANK GRAND TOTALS FOR DATE..." + WCycleDate.ToShortDateString(); 

            textBox_INT_Branches.Text = Sec.GT_INT_Branches.ToString();
            textBox_INT_Matched_Txns.Text = Sec.GT_INT_Matched_Txns.ToString(); 
            textBox_INT_Matched_Amt.Text = Sec.GT_INT_Matched_Amt.ToString("#,##0.00");
            textBox_INT_Unmatched_Branches.Text = Sec.GT_INT_Unmatched_Branches.ToString();
            textBox_INT_Unmathed_Txns.Text = Sec.GT_INT_Unmathed_Txns.ToString();
            textBox_INT_UnMatched_Amt.Text = Sec.GT_INT_UnMatched_Amt.ToString("#,##0.00");

            textBox_EXT_Branches.Text = Sec.GT_EXT_Branches.ToString();
            textBox_EXT_Matched_Txns.Text = Sec.GT_EXT_Matched_Txns.ToString();
            textBox_EXT_Matched_Amt.Text = Sec.GT_EXT_Matched_Amt.ToString("#,##0.00");
            textBox_EXT_Unmatched_Branches.Text = Sec.GT_EXT_Unmatched_Branches.ToString();
            textBox_EXT_Unmathed_Txns.Text = Sec.GT_EXT_Unmathed_Txns.ToString();
            textBox_EXT_UnMatched_Amt.Text = Sec.GT_EXT_UnMatched_Amt.ToString("#,##0.00");

        }


        //Print Lines of Banks Debits and Credits 
        private void button6_Click(object sender, EventArgs e)
        {
            // Assign Parameters

            // Call and print report 
            string P1 = W_E_FIN_Cycle.ToString();
            //string P2 = Sc.CreatedDate.ToString();
            string P2 = WCycleDate.ToShortDateString();
            string P3 = Us.BankId;
            string P4 = WSignedId;

            Form56R_E_FINANCE_Branch_Totals Report_E_FINANCE_Branch_Totals = new Form56R_E_FINANCE_Branch_Totals(P1, P2, P3, P4);
            Report_E_FINANCE_Branch_Totals.Show();
        }
        
        //Print Lines for Single Bank  
        private void button2_Click(object sender, EventArgs e)
        {
            string P1 = W_E_FIN_Cycle.ToString();
            string P2 = "";
            string P3 = "";
            string P4 = Us.BankId;
            string P5 = WSignedId;

            Form56R32ITMX ReportITMX32 = new Form56R32ITMX(P1, P2, P3, P4, P5);
            ReportITMX32.Show();
        }
// Print Line Transactions 
        private void button1_Click(object sender, EventArgs e)
        {
            string P1 = W_E_FIN_Cycle.ToString();
            string P2 = "";
            string P3 = "";
            string P4 = "";
            string P5 = Us.BankId;
            string P6 = "ITMXUser1";

            //if (WBankA == "BBL" & labelBankB.Text == "KBANK")
            //{
            //    //OK
            //}
            //else
            //{
            //    MessageBox.Show("Please select pair BBL Vs KBANK. We have testing data only for this pair.");
            //    return;
            //}

            Form56R33ITMX ReportITMX33 = new Form56R33ITMX(P1, P2, P3, P4, P5, P6);
            ReportITMX33.Show();
        }
// Show ALL UnMatched
        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form271ViewAtmUnmatched : Form
    {

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMGasParameters Gp = new RRDMGasParameters();

     
        public int WUniqueRecordId;

        //bool ViewWorkFlow;

        //string LineAtmNo;

        //int CallingMode; // 

        public int WSelectedRow = 0;

        string SelectionCriteria;
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WReconcCategory;
        int WRMCycleNo;
        string WAtmNo;
        int WReplCycleNo;

        public Form271ViewAtmUnmatched(string InSignedId, int InSignRecordNo, string InOperator, string InReconcCategory, int InWRMCycleNo, 
                                             string InAtmNo, int InReplCycleNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WReconcCategory = InReconcCategory;
            WRMCycleNo = InWRMCycleNo;
            WAtmNo = InAtmNo; // If valse = ALL then it is for all ATMs within category
            WReplCycleNo = InReplCycleNo;  // If > 0 include it in selection criteria

            InitializeComponent();
    
            labelView.Show();
            if (WReplCycleNo == 0)
            {
                label3.Text = "UNMATCHED TXNS FOR ATM NO..." + WAtmNo;
            }
            else
            {
                label3.Text = "UNMATCHED TXNS FOR ATM NO..." + WAtmNo +" With Repl Cycle.." + WReplCycleNo.ToString();
            }
            
        
        }
        private void Form271FastTrack_Load(object sender, EventArgs e)
        {
            try
            {
               
                if (WReplCycleNo == 0)
                {
                    SelectionCriteria = " WHERE Operator ='" + WOperator + "'"
                                                        + " AND TerminalId ='" + WAtmNo + "'"
                                                      + " AND IsMatchingDone = 1 AND Matched = 0 AND ActionType != '07' ";

                }
                else
                {
                    SelectionCriteria = " WHERE Operator ='" + WOperator + "'"
                                                        + " AND TerminalId ='" + WAtmNo + "' AND ReplCycleNo =" + WReplCycleNo
                                                      + " AND IsMatchingDone = 1 AND Matched = 0 AND ActionType != '07' ";

                }

                //SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                //                                    + WReconcCategory + "' AND MatchingAtRMCycle =" + WRMCycleNo
                //                                    + " AND TerminalId ='" + WAtmNo + "'"
                //                                    + " AND IsMatchingDone = 1 AND Matched = 0 AND ActionType != '7' ";


                string WSortCriteria = " ORDER BY MatchingAtRMCycle DESC ";

                //No Dates Are selected

                Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria,1);
                ShowGrid();

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();

                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }
        }

        // On ROW ENTER   

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WUniqueRecordId = (int)rowSelected.Cells[0].Value;

        }

        // Show Grid 

        public void ShowGrid()
        {

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            if (Mpa.MatchingMasterDataTableATMs.Rows.Count == 0)
            {
                MessageBox.Show("No Unmatched To Show");

                this.Dispose();
                return; 
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 60; // RecordId
            dataGridView1.Columns[0].Name = "RecordId";
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // Select
            dataGridView1.Columns[1].Name = "Select";
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 50; // mask
            dataGridView1.Columns[2].Name = "MatchMask";
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 70; // ActionType
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 100; // ActionDesc
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 60; // Settled
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 100; // date
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 140; // Descr
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 40; // Ccy
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 80; // Amount
            dataGridView1.Columns[9].DefaultCellStyle = style;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[9].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[10].Width = 60; // MetaExceptionId
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 90; // Card
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                bool WSelect = (bool)row.Cells[1].Value;
                string WActionType = (string)row.Cells[3].Value;

                if (WSelect == true & WActionType == "04")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                if (WSelect == true & WActionType == "01")
                {
                    row.DefaultCellStyle.BackColor = Color.Beige;
                    row.DefaultCellStyle.ForeColor = Color.Black;

                }
                if (WSelect == false)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }

            }
        }

      

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Update 

 
// Drill Down 
        private void buttonDrillDown_Click(object sender, EventArgs e)
        {
            Form80b NForm80b;

            string WFunction = "Investigation";

            //Form80b(string InSignedId, int InSignRecordNo, string InOperator, string InCategoryId, int InRMCycleNo,
            //                                     string InStringUniqueId, int InIntUniqueId, int InUniqueIdType, string InFunction)

            int UniqueIdType = 4;

            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, 
                NullPastDate, NullPastDate, "", "", 0, "", WUniqueRecordId, UniqueIdType, WFunction, "", 0);

            NForm80b.ShowDialog();
        }
    }
}


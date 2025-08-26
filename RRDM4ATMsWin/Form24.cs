using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form24 : Form
    {
        // Values of ERROR Type
        //
        // 1 : Withdrawl EJournal Errors
        // 2 : Mainframe Withdrawl Errors
        // 3 : Deposit Errors Journal 
        // 4 : Deposit Mainframe Errors
        // 5 : Created by user Errors = eg moving to suspense 
        // 6 : Empty 
        // 7 : Created System Errors 
        // 

        int WTraceNo;
        int WMasterTraceNo;
        int WErrNo;
        Form62 NForm62;
        int Action;

        int WRowIndex;

        string WFilter;

        RRDMErrorsClassWithActions Ea = new RRDMErrorsClassWithActions();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

      
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public Form24(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo,
            string CurrNm, bool Replenishment, string InFilter)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WFilter = InFilter;

            InitializeComponent();
            // Call Procedures 
            textBox1.Text = InAtmNo;
            textBox2.Text = InSesNo.ToString();
        }

        private void Form24_Load(object sender, EventArgs e)
        {

            Ea.ReadErrorsAndFillTable(WOperator, WSignedId, WFilter);

            ShowGrid();

            //dataGridView1.Columns[0].HeaderText = LocRM.GetString("ErrorsTableHeader1", culture);;
        }
        // ON ROW ENTER GET THE TRACE NUMBER OR TRANS NO 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
          
            WErrNo = (int)rowSelected.Cells[0].Value;

            Er.ReadErrorsTableSpecific(WErrNo);

            string WFilter = " Where  UniqueRecordId = " + Er.UniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WFilter,2);
         
            WMasterTraceNo = Mpa.MasterTraceNo;

            WTraceNo = Er.TraceNo;
        }
        // Show Journal 
        private void button7_Click(object sender, EventArgs e)
        {
            // Show Lines of journal 
            string SelectionCriteria = " WHERE UniqueRecordId =" + Mpa.UniqueRecordId;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            if (Mpa.MatchMask == "001"
                || Mpa.MatchMask == "011"
                || Mpa.MatchMask == "010"
                || Mpa.MatchMask == "01"
                )
            {
                MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
                                 + "Select Journal Lines Near To this"
                                 );
                return;
            }

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            if (Mpa.TraceNoWithNoEndZero == 0)
            {
                MessageBox.Show("No Available Trace to show the Journal Lines for this Txn/Category ");
                return;
            }
            else
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;
                WSeqNoB = Mpa.OriginalRecordId;
            }

            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, Mpa.TransDate, Mode);
            NForm67_BDC.ShowDialog();

        }

        //******************
        // SHOW GRID
        //******************
        private void ShowGrid()
        {
            dataGridView1.DataSource = Ea.ErrorsTable.DefaultView;
      
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No available errors with this search! ");
                return;
            }
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            dataGridView1.Columns[0].Width = 50; // ExcNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 50; // ATM No 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 100; //  Desc
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Columns[3].Width = 100; // card
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 50; // Ccy
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 80; // amount
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[6].Width = 60; // Need ACtion
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
          
            dataGridView1.Columns[7].Width = 60; // under Action
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 60; // dispute act  
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 100; // Date
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
// Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

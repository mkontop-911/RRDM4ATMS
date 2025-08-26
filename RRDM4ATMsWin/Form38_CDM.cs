using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual


namespace RRDM4ATMsWin
{
    public partial class Form38_CDM : Form
    {

        Form24 NForm24; // Errors 

        //Form67 NForm67; // Journal 

        //Form5 NForm5; // Dispute form 

        DataTable DepositsTran = new DataTable();

        //int RowSelected;

        //int TransType;

        //int WMasterTraceNo;

        //int UniqueRecordId;
        //int TraceNo;
        //string Card;
        //string Account;
        //string CurrNm;

        //decimal Amount;
        //string TransDesc;
        //DateTime DateTm;
        //decimal Counted;

        //decimal Differ;
        //bool Matched;
        //bool Error;

        //string Comments;

        //int ErrNo;

        //int TotNoCa;
        //decimal TotValueCa;
        //decimal TotCountedCa;
        //decimal TotDiffCa;

        //int TotNoCh;
        //decimal TotValueCh;
        //decimal TotCountedCh;
        //decimal TotDiffCh;

        //int TotNoEnv;
        //decimal TotValueEnv;
        //decimal TotCountedEnv;
        //decimal TotDiffEnv;

        string WBankId;

        //string SQLString;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Class Traces 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public Form38_CDM(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

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

            labelATMno.Text = WAtmNo;

            Ac.ReadAtm(WAtmNo);
            WBankId = Ac.BankId;

            labelSessionNo.Text = WSesNo.ToString();

            textBoxMsgBoard.Text = "Review data and take necessary actions. Press Finish when all actions are taken. ";
        }

        private void Form38_Load(object sender, EventArgs e)
        {

            int WMode = 2;
           
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            //WDateFrom = Ta.SesDtTimeStart;
            //WDateTo = Ta.SesDtTimeEnd;
            Mpa.ReadTableDepositsTxnsByAtmNoAndReplCycle_EGP(WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, "818", WMode, 2);

            dataGridView1.DataSource = Mpa.DepositsTranTable.DefaultView;

            ShowGrid();

            textBoxTotalBNA.Text = Mpa.TotNoBNA.ToString();
            textBoxTotalBNAValue.Text = Mpa.TotValueBNA.ToString("#,##0.00");

            //textBoxTotalCheques.Text = Mpa.TotNoCh.ToString();
            //textBoxTotalChequesValue.Text = Mpa.TotValueCh.ToString("#,##0.00");
        }

        // ON Row Enter Fill up the fields 
        int WUniqueRecordId;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WUniqueRecordId = (int)rowSelected.Cells[0].Value;

            string SelectionCriteria = " Where UniqueRecordId =" + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);

        }
        //
        // Show Grid
        //
        private void ShowGrid()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 70; // UniqueId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 70; //TraceNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //   dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 80; //Card
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //   dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[3].Width = 120; //Account
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //           dataGridView1.Columns[1].Visible = false;


            dataGridView1.Columns[4].Width = 90; //TransDesc
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //  dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[5].Width = 140; //DateTm
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[6].Width = 40; //CurrNm
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //         dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[7].Width = 80; // Amount
            
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].DefaultCellStyle = style;

            dataGridView1.Columns[8].Width = 80; // Counted
            dataGridView1.Columns[8].DefaultCellStyle = style;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[9].Width = 80; // Differ
            dataGridView1.Columns[9].DefaultCellStyle = style;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView1.Columns[10].Width = 70; // SuspectNumber
            //dataGridView1.Columns[10].DefaultCellStyle = style;
            //dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[10].HeaderText = "Suspect"; 

            dataGridView1.Columns[10].Width = 70; // OK
            dataGridView1.Columns[10].DefaultCellStyle = style;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 70; // Error
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[12].Width = 150; // Comments
            dataGridView1.Columns[12].DefaultCellStyle = style;
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }

        // FINISH = UPDATE ALL ACTIONS 
        // Update In Pool Transactions with data grid data
        private void buttonNext_Click(object sender, EventArgs e)
        {

            this.Dispose();
        }



        // Show Error 
        private void button5_Click(object sender, EventArgs e)
        {

            bool Deposits = true;
            string SearchFilter = "ErrNo = " + Mpa.MetaExceptionNo;

            NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, "", Deposits, SearchFilter);
            NForm24.ShowDialog(); ;
        }


        // View Journal Part for this deposit 
        private void button3_Click(object sender, EventArgs e)
        {
            // Show Lines of journal 
            string SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;

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
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();


        }
    }
}

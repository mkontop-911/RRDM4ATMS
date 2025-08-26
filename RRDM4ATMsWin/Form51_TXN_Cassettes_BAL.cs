using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form51_TXN_Cassettes_BAL : Form
    {
        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;

        RRDM_Journal_TransactionSummary Ts = new RRDM_Journal_TransactionSummary(); 

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMRepl_SupervisorMode_Details_Recycle Sm = new RRDMRepl_SupervisorMode_Details_Recycle();

        RRDMJournalAudi_BDC Jnl = new RRDMJournalAudi_BDC();

        string WOperator; 
        string WSignedId;
           string WAtmNo;
            int WSeqNo;

        public Form51_TXN_Cassettes_BAL(string InOperator, string InSignedId, string InAtmNo, int InSeqNo)
        {
            WOperator = InOperator; 
            WSignedId = InSignedId;
            WAtmNo = InAtmNo;
            WSeqNo = InSeqNo; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            // GET DATES 
            //Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            //DateTmSesStart = Ta.SesDtTimeStart;
            //DateTmSesEnd = Ta.SesDtTimeEnd;

            //int WFunction = 2;
            //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

            //labelStep1.Text = "Repl for.." + WAtmNo + "  Cycle No" + WSesNo;

            labelANALYSISForCYCLE.Text = "ANALYSIS FOR REPL DATE.." + DateTmSesEnd.ToString(); 

        }
// Load
        private void Form51_SM_Cassettes_Load(object sender, EventArgs e)
        {
            string WSelectionCriteria = " WHERE SeqNo = " + WSeqNo; 
            Ts.ReadTXN_By_Selection_criteria(WSelectionCriteria);

            if (Ts.TRANDESC == "DEPOSIT")
            {
                labelDeposits.Show();
                panel6.Show();
                label3WithDrawl.Hide();
                panel9.Hide();
            }

            if (Ts.TRANDESC == "WITHDRAWAL")
            {
               
                label3WithDrawl.Show();
                panel9.Show();
                labelDeposits.Hide();
                panel6.Hide();
            }

            labelANALYSISForCYCLE.Text = "ANALYSIS FOR.."+ Ts.TRANDESC+ "..DATE.." + Ts.TRANDATE.ToString();

            // OPPENING
            textBoxTYPE1.Text = Ts.OPEN_DENOM_TYP1.ToString();
            textBoxTYPE2.Text = Ts.OPEN_DENOM_TYP2.ToString();
            textBoxTYPE3.Text = Ts.OPEN_DENOM_TYP3.ToString();
            textBoxTYPE4.Text = Ts.OPEN_DENOM_TYP4.ToString();

            textBoxDisp1.Text = Ts.OPEN_DIS_TYPE1.ToString();
            textBoxDisp2.Text = Ts.OPEN_DIS_TYPE2.ToString();
            textBoxDisp3.Text = Ts.OPEN_DIS_TYPE3.ToString();
            textBoxDisp4.Text = Ts.OPEN_DIS_TYPE4.ToString();

            textBoxRej1.Text = Ts.OPEN_REJ_TYPE1.ToString();
            textBoxRej2.Text = Ts.OPEN_REJ_TYPE2.ToString();
            textBoxRej3.Text = Ts.OPEN_REJ_TYPE3.ToString();
            textBoxRej4.Text = Ts.OPEN_REJ_TYPE4.ToString();

            textBoxRem1.Text = Ts.OPEN_REMAINING_TYPE1.ToString();
            textBoxRem2.Text = Ts.OPEN_REMAINING_TYPE2.ToString();
            textBoxRem3.Text = Ts.OPEN_REMAINING_TYPE3.ToString();
            textBoxRem4.Text = Ts.OPEN_REMAINING_TYPE4.ToString();

            // DEPOSITED AMOUNTS THE TRANSACTION
            textBox_RON_1.Text = Ts.RON1.ToString();
            textBox_RON_5.Text = Ts.RON5.ToString();
            textBox_RON_10.Text = Ts.RON10.ToString();
            textBox_RON_20.Text = Ts.RON20.ToString();
            textBox_RON_50.Text = Ts.RON50.ToString();
            textBox_RON_100.Text = Ts.RON100.ToString();
            textBox_RON_200.Text = Ts.RON200.ToString();
            textBox_RON_500.Text = Ts.RON500.ToString();

            // RECYCLE DEPOSIT TOTALS from REPLENISHMENT START
            textBox_RON_1_Recycle.Text = Ts.RON1_Recycle_Total.ToString();
            textBox_RON_5_Recycle.Text = Ts.RON5_Recycle_Total.ToString();
            textBox_RON_10_Recycle.Text = Ts.RON10_Recycle_Total.ToString();
            textBox_RON_20_Recycle.Text = Ts.RON20_Recycle_Total.ToString();
            textBox_RON_50_Recycle.Text = Ts.RON50_Recycle_Total.ToString();
            textBox_RON_100_Recycle.Text = Ts.RON100_Recycle_Total.ToString();
            textBox_RON_200_Recycle.Text = Ts.RON200_Recycle_Total.ToString();
            textBox_RON_500_Recycle.Text = Ts.RON500_Recycle_Total.ToString();

            // NON RECYCLE DEPOSIT TOTALS from REPLENISHMENT START
            textBox_RON_1_NonRecycle.Text = Ts.RON1_NONRecycle_Total.ToString();
            textBox_RON_5_NonRecycle.Text = Ts.RON5_NONRecycle_Total.ToString();
            textBox_RON_10_NonRecycle.Text = Ts.RON10_NONRecycle_Total.ToString();
            textBox_RON_20_NonRecycle.Text = Ts.RON20_NONRecycle_Total.ToString();
            textBox_RON_50_NonRecycle.Text = Ts.RON50_NONRecycle_Total.ToString();
            textBox_RON_100_NonRecycle.Text = Ts.RON100_NONRecycle_Total.ToString();
            textBox_RON_200_NonRecycle.Text = Ts.RON200_NONRecycle_Total.ToString();
            textBox_RON_500_NonRecycle.Text = Ts.RON500_NONRecycle_Total.ToString();


            // Presented // Retracted
            textBoxTYPE1_P.Text = Ts.OPEN_DENOM_TYP1.ToString();
            textBoxTYPE2_P.Text = Ts.OPEN_DENOM_TYP2.ToString();
            textBoxTYPE3_P.Text = Ts.OPEN_DENOM_TYP3.ToString();
            textBoxTYPE4_P.Text = Ts.OPEN_DENOM_TYP4.ToString();

            textBoxPre1.Text = Ts.NotesPresented_TYPE1.ToString();
            textBoxPre2.Text = Ts.NotesPresented_TYPE2.ToString();
            textBoxPre3.Text = Ts.NotesPresented_TYPE3.ToString();
            textBoxPre4.Text = Ts.NotesPresented_TYPE4.ToString();

            textBoxRetracted1.Text = "0";
            textBoxRetracted2.Text = "0";
            textBoxRetracted3.Text = "0";
            textBoxRetracted4.Text = "0";


            // CLOSING

            textBoxTYPE1_C.Text = Ts.CLOSE_DENOM_TYP1.ToString();
            textBoxTYPE2_C.Text = Ts.CLOSE_DENOM_TYP2.ToString();
            textBoxTYPE3_C.Text = Ts.CLOSE_DENOM_TYP3.ToString();
            textBoxTYPE4_C.Text = Ts.CLOSE_DENOM_TYP4.ToString();

            textBoxDisp1_C.Text = Ts.CLOSE_DIS_TYPE1.ToString();
            textBoxDisp2_C.Text = Ts.CLOSE_DIS_TYPE2.ToString();
            textBoxDisp3_C.Text = Ts.CLOSE_DIS_TYPE3.ToString();
            textBoxDisp4_C.Text = Ts.CLOSE_DIS_TYPE4.ToString();

            textBoxRej1_C.Text = Ts.CLOSE_REJ_TYPE1.ToString();
            textBoxRej2_C.Text = Ts.CLOSE_REJ_TYPE2.ToString();
            textBoxRej3_C.Text = Ts.CLOSE_REJ_TYPE3.ToString();
            textBoxRej4_C.Text = Ts.CLOSE_REJ_TYPE4.ToString();

            textBoxRem1_C.Text = Ts.CLOSE_REMAINING_TYPE1.ToString();
            textBoxRem2_C.Text = Ts.CLOSE_REMAINING_TYPE2.ToString();
            textBoxRem3_C.Text = Ts.CLOSE_REMAINING_TYPE3.ToString();
            textBoxRem4_C.Text = Ts.CLOSE_REMAINING_TYPE4.ToString();




            RRDMRepl_SupervisorMode_Details_Recycle Sm = new RRDMRepl_SupervisorMode_Details_Recycle();
            //Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);

            // Opening Balance 


            // Rejected BIN

            //ATM_Rejected1 = (int)rdr["ATM_Rejected1"];
            //ATM_Rejected2 = (int)rdr["ATM_Rejected2"];
            //ATM_Rejected3 = (int)rdr["ATM_Rejected3"];
            //ATM_Rejected4 = (int)rdr["ATM_Rejected4"];
            //ATM_Rejected5 = (int)rdr["ATM_Rejected5"];

            // Denomination
            //textBox_RB_D_One.Text = Na.Cassettes_1.FaceValue.ToString();
            //textBox_RB_D_Two.Text = Na.Cassettes_2.FaceValue.ToString();
            //textBox_RB_D_Three.Text = Na.Cassettes_3.FaceValue.ToString();
            //textBox_RB_D_Four.Text = Na.Cassettes_4.FaceValue.ToString();
            //// Number of Notes 
            //textBox_RON_1.Text = Sm.ATM_Rejected1.ToString();
            //textBox_RON_5.Text = Sm.ATM_Rejected2.ToString();
            //textBox_RON_10.Text = Sm.ATM_Rejected3.ToString();
            //textBox_RON_20.Text = Sm.ATM_Rejected4.ToString();
            // Total 
            //textBox_RB_A_One.Text = (Sm.ATM_Rejected1 * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
            //textBox_RB_A_Two.Text = (Sm.ATM_Rejected2 * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
            //textBox_RB_A_Three.Text = (Sm.ATM_Rejected3 * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
            //textBox_RB_A_Four.Text = (Sm.ATM_Rejected4 * Na.Cassettes_4.FaceValue).ToString("#,##0.00");

            //decimal RejectedBinBal =
            //              Sm.ATM_Rejected1 * Na.Cassettes_1.FaceValue
            //            + Sm.ATM_Rejected2 * Na.Cassettes_2.FaceValue
            //            + Sm.ATM_Rejected3 * Na.Cassettes_3.FaceValue
            //            + Sm.ATM_Rejected4 * Na.Cassettes_4.FaceValue;

            //textBox_RB_A_Total.Text = RejectedBinBal.ToString("#,##0.00");
            return;
           
            //string WSelectionCriteriaDep = " WHERE "
            //                  + "AtmNo ='" + WAtmNo + "'  "
            //                   + "AND ReplCycle=" + WSesNo   ;

            //Jnl.FillTableDepositsFromJournalByDenomination(WOperator, WSignedId,
            //                      WSelectionCriteriaDep);



            ShowGrid_Deposits();

            WSelectionCriteria = " WHERE "
                       + "   PresenterError= 'PresenterError' "
                               + "AND AtmNo ='" + WAtmNo + "'  ";

            Jnl.FillTablePresentedFromJournal(WOperator, WSignedId,
                                     WSelectionCriteria,
                                    DateTmSesStart, DateTmSesEnd); 

            ShowGrid_Presented();


       //     textBoxTotalUnloaded.Text = (RemainingBal + RejectedBinBal).ToString("#,##0.00");

            textBoxDeposits.Text = Jnl.Tot_DepAmt.ToString("#,##0.00");

            textBoxDepRetracted.Text = "0.00"; 

            decimal TotalPresented =
                          Jnl.Tot_Type1 * Na.Cassettes_1.FaceValue
                        + Jnl.Tot_Type2 * Na.Cassettes_2.FaceValue
                        + Jnl.Tot_Type3 * Na.Cassettes_3.FaceValue
                        + Jnl.Tot_Type4 * Na.Cassettes_4.FaceValue;

            textBoxFromPresenter.Text = TotalPresented.ToString("#,##0.00");
            //textBoxTotalOneAndTwo.Text = (RemainingBal + RejectedBinBal+ TotalPresented).ToString("#,##0.00"); ;
            //textBoxGrandTotall.Text= (RemainingBal + RejectedBinBal + TotalPresented + Jnl.Tot_DepAmt).ToString("#,##0.00");

           
        }

        private void ShowGrid_Presented()
        {
            dataGridView1.DataSource = Jnl.TableJournalPresented.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
            }
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            //" SELECT  [SeqNo]  ,[AtmNo] ,[TraceNumber]/10 As TraceNo ,[Type1]  ,[Type2]   ,[Type3]  ,[Type4]"
            //    + "  ,Cast([CAmount] As decimal(15, 2)) As Amount "

            dataGridView1.Columns[0].Width = 40; // MaskRecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 40; // AtmNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; //  TraceNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 40; //  Type1
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 40; // Type2
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 40; // Type3
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 40; // Type4
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 70; // Amount
            dataGridView1.Columns[7].DefaultCellStyle = style;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }

        private void ShowGrid_Deposits()
        {
            //dataGridView2.DataSource = Jnl.TableJournalDeposits.DefaultView;

            //if (dataGridView1.Rows.Count == 0)
            //{
            //}
            //DataGridViewCellStyle style = new DataGridViewCellStyle();
            //style.Format = "N2";

            //dataGridView2.Columns[0].Width = 40; // SeqNo
            //dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView2.Columns[0].Visible = false;

            //dataGridView2.Columns[1].Width = 40; // AtmNo
            //dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView2.Columns[1].Visible = false;

            //dataGridView2.Columns[2].Width = 70; //  'DEPOSIT' as Deposit
            //dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[3].Width = 70; //  Currency
            //dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[4].Width = 70; //  FaceValue
            //dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[5].Width = 70; // CASSETTE
            //dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[6].Width = 70; //  RETRACT
            //dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
    }
}

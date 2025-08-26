using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form51_SM_Cassettes : Form
    {
        DateTime DateTmSesStart;
        DateTime DateTmSesEnd; 


        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMRepl_SupervisorMode_Details_Recycle Sm = new RRDMRepl_SupervisorMode_Details_Recycle();

        RRDMJournalAudi_BDC Jnl = new RRDMJournalAudi_BDC();

        string WOperator; 
        string WSignedId;
           string WAtmNo;
            int WSesNo;

        public Form51_SM_Cassettes(string InOperator, string InSignedId, string InAtmNo, int InSesNo)
        {
            WOperator = InOperator; 
            WSignedId = InSignedId;
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            this.WindowState = FormWindowState.Maximized;

            // GET DATES 
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            DateTmSesStart = Ta.SesDtTimeStart;
            DateTmSesEnd = Ta.SesDtTimeEnd;

            int WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

            labelStep1.Text = "Repl for.." + WAtmNo + "  Cycle No" + WSesNo;

            labelANALYSISForCYCLE.Text = "ANALYSIS FOR REPL DATE.." + DateTmSesEnd.ToString(); 

        }
// Load
        private void Form51_SM_Cassettes_Load(object sender, EventArgs e)
        {

            RRDMRepl_SupervisorMode_Details_Recycle Sm = new RRDMRepl_SupervisorMode_Details_Recycle();
            Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);

            // Opening Balance 
            textBox_O_D_One.Text = Na.Cassettes_1.FaceValue.ToString();
            textBox_O_D_Two.Text = Na.Cassettes_2.FaceValue.ToString();
            textBox_O_D_Three.Text = Na.Cassettes_3.FaceValue.ToString();
            textBox_O_D_Four.Text = Na.Cassettes_4.FaceValue.ToString();

            textBox_O_N_One.Text = Sm.ATM_total1.ToString();
            textBox_O_N_Two.Text = Sm.ATM_total2.ToString();
            textBox_O_N_Three.Text = Sm.ATM_total3.ToString();
            textBox_O_N_Four.Text = Sm.ATM_total4.ToString();

            textBox_O_A_One.Text = (Sm.ATM_total1 * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
            textBox_O_A_Two.Text = (Sm.ATM_total2 * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
            textBox_O_A_Three.Text = (Sm.ATM_total3 * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
            textBox_O_A_Four.Text = (Sm.ATM_total4 * Na.Cassettes_4.FaceValue).ToString("#,##0.00");

            decimal OpeningBal = // After date Cap_date
                          Sm.ATM_total1 * Na.Cassettes_1.FaceValue
                        + Sm.ATM_total2 * Na.Cassettes_2.FaceValue
                        + Sm.ATM_total3 * Na.Cassettes_3.FaceValue
                        + Sm.ATM_total4 * Na.Cassettes_4.FaceValue;
            // Na.GL_Bal_Repl_Adjusted.ToString("##0.00");
            textBox_O_A_Total.Text = OpeningBal.ToString("#,##0.00");

            // LOADED

            // Denomination
            textBox_L_D_One.Text = Na.Cassettes_1.FaceValue.ToString();
            textBox_L_D_Two.Text = Na.Cassettes_2.FaceValue.ToString();
            textBox_L_D_Three.Text = Na.Cassettes_3.FaceValue.ToString();
            textBox_L_D_Four.Text = Na.Cassettes_4.FaceValue.ToString();
            // Number of Notes 
            textBox_L_N_One.Text = Sm.cashaddtype1.ToString();
            textBox_L_N_Two.Text = Sm.cashaddtype2.ToString();
            textBox_L_N_Three.Text = Sm.cashaddtype3.ToString();
            textBox_L_N_Four.Text = Sm.cashaddtype4.ToString();
            // Total 
            textBox_L_A_One.Text = (Sm.cashaddtype1 * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
            textBox_L_A_Two.Text = (Sm.cashaddtype2 * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
            textBox_L_A_Three.Text = (Sm.cashaddtype3 * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
            textBox_L_A_Four.Text = (Sm.cashaddtype4 * Na.Cassettes_4.FaceValue).ToString("#,##0.00");

            decimal LoadedBal = 
                          Sm.cashaddtype1 * Na.Cassettes_1.FaceValue
                        + Sm.cashaddtype2 * Na.Cassettes_2.FaceValue
                        + Sm.cashaddtype3 * Na.Cassettes_3.FaceValue
                        + Sm.cashaddtype4 * Na.Cassettes_4.FaceValue;
            // Na.GL_Bal_Repl_Adjusted.ToString("##0.00");
            textBox_L_A_Total.Text = LoadedBal.ToString("#,##0.00");

            // Remaining In Cassettes 
            // Sm.ATM_cassette1

            // Denomination
            textBox_R_D_One.Text = Na.Cassettes_1.FaceValue.ToString();
            textBox_R_D_Two.Text = Na.Cassettes_2.FaceValue.ToString();
            textBox_R_D_Three.Text = Na.Cassettes_3.FaceValue.ToString();
            textBox_R_D_Four.Text = Na.Cassettes_4.FaceValue.ToString();
            // Number of Notes 
            textBox_R_N_One.Text = Sm.ATM_cassette1.ToString();
            textBox_R_N_Two.Text = Sm.ATM_cassette2.ToString();
            textBox_R_N_Three.Text = Sm.ATM_cassette3.ToString();
            textBox_R_N_Four.Text = Sm.ATM_cassette4.ToString();
            // Total 
            textBox_R_A_One.Text = (Sm.ATM_cassette1 * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
            textBox_R_A_Two.Text = (Sm.ATM_cassette2 * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
            textBox_R_A_Three.Text = (Sm.ATM_cassette3 * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
            textBox_R_A_Four.Text = (Sm.ATM_cassette4 * Na.Cassettes_4.FaceValue).ToString("#,##0.00");

            decimal RemainingBal =
                          Sm.ATM_cassette1 * Na.Cassettes_1.FaceValue
                        + Sm.ATM_cassette2 * Na.Cassettes_2.FaceValue
                        + Sm.ATM_cassette3 * Na.Cassettes_3.FaceValue
                        + Sm.ATM_cassette4 * Na.Cassettes_4.FaceValue;
           
            textBox_R_A_Total.Text = RemainingBal.ToString("#,##0.00");

            // Rejected BIN

            //ATM_Rejected1 = (int)rdr["ATM_Rejected1"];
            //ATM_Rejected2 = (int)rdr["ATM_Rejected2"];
            //ATM_Rejected3 = (int)rdr["ATM_Rejected3"];
            //ATM_Rejected4 = (int)rdr["ATM_Rejected4"];
            //ATM_Rejected5 = (int)rdr["ATM_Rejected5"];

            // Denomination
            textBox_RB_D_One.Text = Na.Cassettes_1.FaceValue.ToString();
            textBox_RB_D_Two.Text = Na.Cassettes_2.FaceValue.ToString();
            textBox_RB_D_Three.Text = Na.Cassettes_3.FaceValue.ToString();
            textBox_RB_D_Four.Text = Na.Cassettes_4.FaceValue.ToString();
            // Number of Notes 
            textBox_RB_N_One.Text = Sm.ATM_Rejected1.ToString();
            textBox_RB_N_Two.Text = Sm.ATM_Rejected2.ToString();
            textBox_RB_N_Three.Text = Sm.ATM_Rejected3.ToString();
            textBox_RB_N_Four.Text = Sm.ATM_Rejected4.ToString();
            // Total 
            textBox_RB_A_One.Text = (Sm.ATM_Rejected1 * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
            textBox_RB_A_Two.Text = (Sm.ATM_Rejected2 * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
            textBox_RB_A_Three.Text = (Sm.ATM_Rejected3 * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
            textBox_RB_A_Four.Text = (Sm.ATM_Rejected4 * Na.Cassettes_4.FaceValue).ToString("#,##0.00");

            decimal RejectedBinBal =
                          Sm.ATM_Rejected1 * Na.Cassettes_1.FaceValue
                        + Sm.ATM_Rejected2 * Na.Cassettes_2.FaceValue
                        + Sm.ATM_Rejected3 * Na.Cassettes_3.FaceValue
                        + Sm.ATM_Rejected4 * Na.Cassettes_4.FaceValue;

            textBox_RB_A_Total.Text = RejectedBinBal.ToString("#,##0.00");

           
            string WSelectionCriteriaDep = " WHERE "
                              + "AtmNo ='" + WAtmNo + "'  "
                               + "AND ReplCycle=" + WSesNo   ;

            Jnl.FillTableDepositsFromJournalByDenomination(WOperator, WSignedId,
                                  WSelectionCriteriaDep);

            ShowGrid_Deposits();

            string WSelectionCriteria = " WHERE "
                       + "   PresenterError= 'PresenterError' "
                               + "AND AtmNo ='" + WAtmNo + "'  ";

            Jnl.FillTablePresentedFromJournal(WOperator, WSignedId,
                                     WSelectionCriteria,
                                    DateTmSesStart, DateTmSesEnd); 

            ShowGrid_Presented();


            textBoxTotalUnloaded.Text = (RemainingBal + RejectedBinBal).ToString("#,##0.00");

            textBoxDeposits.Text = Jnl.Tot_DepAmt.ToString("#,##0.00");

            textBoxDepRetracted.Text = "0.00"; 

            decimal TotalPresented =
                          Jnl.Tot_Type1 * Na.Cassettes_1.FaceValue
                        + Jnl.Tot_Type2 * Na.Cassettes_2.FaceValue
                        + Jnl.Tot_Type3 * Na.Cassettes_3.FaceValue
                        + Jnl.Tot_Type4 * Na.Cassettes_4.FaceValue;

            textBoxFromPresenter.Text = TotalPresented.ToString("#,##0.00");
            textBoxTotalOneAndTwo.Text = (RemainingBal + RejectedBinBal+ TotalPresented).ToString("#,##0.00"); ;
            textBoxGrandTotall.Text= (RemainingBal + RejectedBinBal + TotalPresented + Jnl.Tot_DepAmt).ToString("#,##0.00");

           
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
            dataGridView2.DataSource = Jnl.TableJournalDeposits.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
            }
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView2.Columns[0].Width = 40; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].Visible = false;

            dataGridView2.Columns[1].Width = 40; // AtmNo
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].Visible = false;

            dataGridView2.Columns[2].Width = 70; //  'DEPOSIT' as Deposit
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[3].Width = 70; //  Currency
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 70; //  FaceValue
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[5].Width = 70; // CASSETTE
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[6].Width = 70; //  RETRACT
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[7].Width = 70; // recycle
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
    }
}

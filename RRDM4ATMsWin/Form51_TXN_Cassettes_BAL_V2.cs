using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form51_TXN_Cassettes_BAL_V2 : Form
    {
        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;

        RRDM_Journal_TransactionSummary_V2 Ts = new RRDM_Journal_TransactionSummary_V2(); 

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMRepl_SupervisorMode_Details_Recycle Sm = new RRDMRepl_SupervisorMode_Details_Recycle();

        RRDMJournalAudi_BDC Jnl = new RRDMJournalAudi_BDC();

        string WOperator; 
        string WSignedId;
           string WAtmNo;
        decimal WTransAmount; 
            int WSeqNo;
        DateTime W_DtFrom;
        DateTime W_DtTo;
        // , Ta.SesDtTimeStart, LineDate,WMode );
        public Form51_TXN_Cassettes_BAL_V2(string InOperator, string InSignedId, string InAtmNo, decimal InAmount ,int InSeqNo, DateTime InDtFrom, DateTime InDtTo, int InMode)
        {
            WOperator = InOperator; 
            WSignedId = InSignedId;
            WAtmNo = InAtmNo;
            WTransAmount = InAmount; 
            WSeqNo = InSeqNo;
            W_DtFrom = InDtFrom;
            W_DtTo = InDtTo;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            this.WindowState = FormWindowState.Maximized;

            // GET DATES 
            //Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            //DateTmSesStart = Ta.SesDtTimeStart;
            //DateTmSesEnd = Ta.SesDtTimeEnd;

            //int WFunction = 2;
            //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

            labelStep1.Text = "TXN Balancing from :.." + W_DtFrom.ToString() + " To:.." + W_DtTo.ToString(); 

            //LableANALYSISForTXN.Text = "ANALYSIS FOR REPL DATE.." + DateTmSesEnd.ToString(); 

        }
// Load
        private void Form51_SM_Cassettes_Load(object sender, EventArgs e)
        {
           // Get from transaction Balancing
            string WSelectionCriteria = " WHERE Origin_SeqNo = " + WSeqNo; 
            Ts.ReadTXN_By_Selection_criteria(WSelectionCriteria);

           // FX TOTALS 
            //Ts.ReadAND_Get_FX_TOTALS(WAtmNo, W_DtFrom, W_DtTo);

            // GET CURRENCY
            //Ts.ReadCurrencyFromtblHstAtmTxnsROM(WAtmNo, WSeqNo); 

            // Get Notes
            if (Ts.Ccy == "EUR")
            {
                Ts.Ccy = Ts.Ccy;
                // Read the Euro Notes details 
                //Ts.ReadAND_Get_FX_Notes(WAtmNo, WSeqNo); // With SeqNo we read both 
            }
           

            LableANALYSISForTXN.Text = "TXN IS :"+Ts.TRANDESC+".IN CURRENCY..:"+ Ts.Ccy+ " WITH AMOUNT "
                     + Ts.TXN_AMOUNT_CR_DR.ToString("#,##0.00") + "..TRACE.."+ Ts.TRACE
                       + "_DATE_" + Ts.TRANDATE.ToShortDateString();

            if (Ts.TRANDESC == "DEPOSIT" & Ts.Ccy == "RON")
            {
                labelDeposits.Show();
                panel6.Show();
                label3WithDrawl.Hide();
                panel9.Hide();

                labelThisDeposit.Show();

                textBox_RON_1.Show();
                textBox_RON_5.Show();
                textBox_RON_10.Show();

                textBox_RON_20.Show();
                textBox_RON_50.Show();
                textBox_RON_100.Show();

                textBox_RON_200.Show();
                textBox_RON_500.Show();

                textBox_RB_A_Total.Show();

                textBoxDepAmt.Show();

                textBoxDiff_Deposits.Show();
                // Deposits Labels
                label32.Show();
                label33.Show();
                label14.Show();
                label79.Show();
                label2.Hide();
            }

            if (Ts.TRANDESC == "WITHDRAWAL" )
            {
                label3WithDrawl.Show();
                panel9.Show();
                //labelDeposits.Hide();
            }

            if (Ts.TRANDESC == "WITHDRAWAL" || Ts.Ccy == "EUR")
            {
               
                labelDeposits.Show();
                panel6.Show();
                labelThisDeposit.Hide();

                textBox_RON_1.Hide();
                textBox_RON_5.Hide();
                textBox_RON_10.Hide();

                textBox_RON_20.Hide();
                textBox_RON_50.Hide();
                textBox_RON_100.Hide();

                textBox_RON_200.Hide();
                textBox_RON_500.Hide();

               
                textBox_RB_A_Total.Hide();

                textBoxDepAmt.Hide();

                textBoxDiff_Deposits.Hide();
                // Deposits Labels
                label32.Hide();
                label33.Hide();
                label14.Hide();
                label79.Hide();
                label2.Hide(); 
            }

            //LableANALYSISForTXN.Text = "ANALYSIS FOR.."+ Ts.TRANDESC+ "..DATE.." + Ts.TRANDATE.ToString();

            // OPENING
            textBoxTYPE1.Text = Ts.OPEN_DENOM_TYP1.ToString();
            textBoxTYPE2.Text = Ts.OPEN_DENOM_TYP2.ToString();
            textBoxTYPE3.Text = Ts.OPEN_DENOM_TYP3.ToString();
            textBoxTYPE4.Text = Ts.OPEN_DENOM_TYP4.ToString();

            textBoxDisp1.Text = Ts.OPEN_DIS_TYPE1.ToString();
            textBoxDisp2.Text = Ts.OPEN_DIS_TYPE2.ToString();
            textBoxDisp3.Text = Ts.OPEN_DIS_TYPE3.ToString();
            textBoxDisp4.Text = Ts.OPEN_DIS_TYPE4.ToString();

            //textBoxRej1.Text = Ts.OPEN_REJ_TYPE1.ToString();
            //textBoxRej2.Text = Ts.OPEN_REJ_TYPE2.ToString();
            //textBoxRej3.Text = Ts.OPEN_REJ_TYPE3.ToString();
            //textBoxRej4.Text = Ts.OPEN_REJ_TYPE4.ToString();

            labelREJ_O.Hide(); 

            textBoxRej1.Hide();
            textBoxRej2.Hide();
            textBoxRej3.Hide();
            textBoxRej4.Hide();

            textBoxRem1.Text = (Ts.OPEN_REMAINING_TYPE1 + Ts.OPEN_REJ_TYPE1).ToString();
            textBoxRem2.Text = (Ts.OPEN_REMAINING_TYPE2 + Ts.OPEN_REJ_TYPE2).ToString();
            textBoxRem3.Text = (Ts.OPEN_REMAINING_TYPE3 + Ts.OPEN_REJ_TYPE3).ToString();
            textBoxRem4.Text = (Ts.OPEN_REMAINING_TYPE4 + Ts.OPEN_REJ_TYPE4).ToString();

            textBoxRem1_VAL.Text = ((Ts.OPEN_REMAINING_TYPE1 + Ts.OPEN_REJ_TYPE1) * Ts.OPEN_DENOM_TYP1).ToString("#,##0.00");
            textBoxRem2_VAL.Text = ((Ts.OPEN_REMAINING_TYPE2 + Ts.OPEN_REJ_TYPE2) * Ts.OPEN_DENOM_TYP2).ToString("#,##0.00");
            textBoxRem3_VAL.Text = ((Ts.OPEN_REMAINING_TYPE3 + Ts.OPEN_REJ_TYPE3) * Ts.OPEN_DENOM_TYP3).ToString("#,##0.00");
            textBoxRem4_VAL.Text = ((Ts.OPEN_REMAINING_TYPE4 + Ts.OPEN_REJ_TYPE4) * Ts.OPEN_DENOM_TYP4).ToString("#,##0.00");

            textBoxRemAmt_O.Text =
                (
                ((Ts.OPEN_REMAINING_TYPE1 + Ts.OPEN_REJ_TYPE1) * Ts.OPEN_DENOM_TYP1)
                + ((Ts.OPEN_REMAINING_TYPE2 + Ts.OPEN_REJ_TYPE2) * Ts.OPEN_DENOM_TYP2)
                + ((Ts.OPEN_REMAINING_TYPE3 + Ts.OPEN_REJ_TYPE3) * Ts.OPEN_DENOM_TYP3)
                + ((Ts.OPEN_REMAINING_TYPE4 + Ts.OPEN_REJ_TYPE4) * Ts.OPEN_DENOM_TYP4)
                ).ToString("#,##0.00");

            // DEPOSITED AMOUNTS THE TRANSACTION
            textBox_RON_1.Text = Ts.RON1.ToString();
            textBox_RON_5.Text = Ts.RON5.ToString();
            textBox_RON_10.Text = Ts.RON10.ToString();
            textBox_RON_20.Text = Ts.RON20.ToString();
            textBox_RON_50.Text = Ts.RON50.ToString();
            textBox_RON_100.Text = Ts.RON100.ToString();
            textBox_RON_200.Text = Ts.RON200.ToString();
            textBox_RON_500.Text = Ts.RON500.ToString();

            decimal TotalGiven = Ts.RON1 * 1
                           + Ts.RON5 * 5
                           + Ts.RON10 * 10
                           + Ts.RON20 * 20

                           + Ts.RON50 * 50
                           + Ts.RON100 * 100
                           + Ts.RON200 * 200
                           + Ts.RON500 * 500
                           ;
            textBox_RB_A_Total.Text = TotalGiven.ToString("#,##0.00"); ;

            textBoxDepAmt.Text = WTransAmount.ToString("#,##0.00");

            textBoxDiff_Deposits.Text = (WTransAmount - TotalGiven).ToString("#,##0.00");  

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

            textBox_RON_1_VAL.Text = ((Ts.RON1_Recycle_Total + Ts.RON1_NONRecycle_Total) * 1).ToString("#,##0.00");
            textBox_RON_5_VAL.Text = ((Ts.RON5_Recycle_Total + Ts.RON5_NONRecycle_Total) * 5).ToString("#,##0.00");
            textBox_RON_10_VAL.Text = ((Ts.RON10_Recycle_Total + Ts.RON10_NONRecycle_Total) * 10).ToString("#,##0.00");
            textBox_RON_20_VAL.Text = ((Ts.RON20_Recycle_Total + Ts.RON20_NONRecycle_Total) * 20).ToString("#,##0.00");

            textBox_RON_50_VAL.Text = ((Ts.RON50_Recycle_Total + Ts.RON50_NONRecycle_Total) * 50).ToString("#,##0.00");
            textBox_RON_100_VAL.Text = ((Ts.RON100_Recycle_Total + Ts.RON100_NONRecycle_Total) * 100).ToString("#,##0.00");
            textBox_RON_200_VAL.Text = ((Ts.RON200_Recycle_Total + Ts.RON200_NONRecycle_Total) * 200).ToString("#,##0.00");
            textBox_RON_500_VAL.Text = ((Ts.RON500_Recycle_Total + Ts.RON500_NONRecycle_Total) * 500).ToString("#,##0.00");

            
            decimal Recycle_Total = Ts.RON1_Recycle_Total * 1
                          + Ts.RON5_Recycle_Total * 5
                          + Ts.RON10_Recycle_Total * 10
                          + Ts.RON20_Recycle_Total * 20

                          + Ts.RON50_Recycle_Total * 50
                          + Ts.RON100_Recycle_Total * 100
                          + Ts.RON200_Recycle_Total * 200
                          + Ts.RON500_Recycle_Total * 500
                          ;
            textBox_Recycle_Total.Text = Recycle_Total.ToString("#,##0.00");

            decimal NON_Recycle_Total = Ts.RON1_NONRecycle_Total * 1
                         + Ts.RON5_NONRecycle_Total * 5
                         + Ts.RON10_NONRecycle_Total * 10
                         + Ts.RON20_NONRecycle_Total * 20

                         + Ts.RON50_NONRecycle_Total * 50
                         + Ts.RON100_NONRecycle_Total * 100
                         + Ts.RON200_NONRecycle_Total * 200
                         + Ts.RON500_NONRecycle_Total * 500
                         ; 

            textBox_NON_Recycle_Total.Text = NON_Recycle_Total.ToString("#,##0.00");

            textBoxGRAND_TOTAL_DEP.Text = (Recycle_Total+ NON_Recycle_Total).ToString("#,##0.00");

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

            textBox_Req.Text = WTransAmount.ToString("#,##0.00");

            textBox_Pres.Text = ((Ts.NotesPresented_TYPE1* Ts.OPEN_DENOM_TYP1)
                + (Ts.NotesPresented_TYPE2 * Ts.OPEN_DENOM_TYP2)
                + (Ts.NotesPresented_TYPE3 * Ts.OPEN_DENOM_TYP3)
                + (Ts.NotesPresented_TYPE4 * Ts.OPEN_DENOM_TYP4)
                ).ToString("#,##0.00");
            

            textBoxDiff.Text = (WTransAmount -
                ((Ts.NotesPresented_TYPE1 * Ts.OPEN_DENOM_TYP1)
                + (Ts.NotesPresented_TYPE2 * Ts.OPEN_DENOM_TYP2)
                + (Ts.NotesPresented_TYPE3 * Ts.OPEN_DENOM_TYP3)
                + (Ts.NotesPresented_TYPE4 * Ts.OPEN_DENOM_TYP4)
                )).ToString("#,##0.00");

            // CLOSING

            textBoxTYPE1_C.Text = Ts.CLOSE_DENOM_TYP1.ToString();
            textBoxTYPE2_C.Text = Ts.CLOSE_DENOM_TYP2.ToString();
            textBoxTYPE3_C.Text = Ts.CLOSE_DENOM_TYP3.ToString();
            textBoxTYPE4_C.Text = Ts.CLOSE_DENOM_TYP4.ToString();

            textBoxDisp1_C.Text = Ts.CLOSE_DIS_TYPE1.ToString();
            textBoxDisp2_C.Text = Ts.CLOSE_DIS_TYPE2.ToString();
            textBoxDisp3_C.Text = Ts.CLOSE_DIS_TYPE3.ToString();
            textBoxDisp4_C.Text = Ts.CLOSE_DIS_TYPE4.ToString();



            //textBoxRej1_C.Text = Ts.CLOSE_REJ_TYPE1.ToString();
            //textBoxRej2_C.Text = Ts.CLOSE_REJ_TYPE2.ToString();
            //textBoxRej3_C.Text = Ts.CLOSE_REJ_TYPE3.ToString();
            //textBoxRej4_C.Text = Ts.CLOSE_REJ_TYPE4.ToString();

            labelREJ_C.Hide();

            textBoxRej1_C.Hide();
            textBoxRej2_C.Hide();
            textBoxRej3_C.Hide();
            textBoxRej4_C.Hide();

            textBoxRem1_C.Text = (Ts.CLOSE_REMAINING_TYPE1+ Ts.CLOSE_REJ_TYPE1).ToString();
            textBoxRem2_C.Text = (Ts.CLOSE_REMAINING_TYPE2 + Ts.CLOSE_REJ_TYPE2).ToString();
            textBoxRem3_C.Text = (Ts.CLOSE_REMAINING_TYPE3 + Ts.CLOSE_REJ_TYPE3).ToString();
            textBoxRem4_C.Text = (Ts.CLOSE_REMAINING_TYPE4 + Ts.CLOSE_REJ_TYPE4).ToString();

            
            textBoxRem1_C_VAL.Text = ((Ts.CLOSE_REMAINING_TYPE1 + Ts.CLOSE_REJ_TYPE1) * Ts.OPEN_DENOM_TYP1).ToString("#,##0.00");
            textBoxRem2_C_VAL.Text = ((Ts.CLOSE_REMAINING_TYPE2 + Ts.CLOSE_REJ_TYPE2) * Ts.OPEN_DENOM_TYP2).ToString("#,##0.00");
            textBoxRem3_C_VAL.Text = ((Ts.CLOSE_REMAINING_TYPE3 + Ts.CLOSE_REJ_TYPE3) * Ts.OPEN_DENOM_TYP3).ToString("#,##0.00");
            textBoxRem4_C_VAL.Text = ((Ts.CLOSE_REMAINING_TYPE4 + Ts.CLOSE_REJ_TYPE4) * Ts.OPEN_DENOM_TYP4).ToString("#,##0.00");

            textBoxRemAmt_C.Text =
                (
                ((Ts.CLOSE_REMAINING_TYPE1 + Ts.CLOSE_REJ_TYPE1) * Ts.OPEN_DENOM_TYP1)
                + ((Ts.CLOSE_REMAINING_TYPE2 + Ts.CLOSE_REJ_TYPE2) * Ts.OPEN_DENOM_TYP2)
                + ((Ts.CLOSE_REMAINING_TYPE3 + Ts.CLOSE_REJ_TYPE3) * Ts.OPEN_DENOM_TYP3)
                + ((Ts.CLOSE_REMAINING_TYPE4 + Ts.CLOSE_REJ_TYPE4) * Ts.OPEN_DENOM_TYP4)
                ).ToString("#,##0.00");

            // EURO ANALYSIS

            if (Ts.Ccy == "EUR")
            {
                labelThisTXN.Show();
                labelNotesAmnt.Show();
                labelEntered.Show();
                textBoxEurNotesAmnt.Show();
                textBoxEuroEnter.Show();
                labelEuroDiff.Show();
                textBoxEuroDiff.Show();
                label2.Show();

                textBox_EUR_5.Show();
                textBox_EUR_10.Show();
                textBox_EUR_20.Show();
                textBox_EUR_50.Show();

                textBox_EUR_100.Show();
                textBox_EUR_200.Show();
                textBox_EUR_500.Show();

                textBox_EUR_5.Text = Ts.eur5.ToString();
                textBox_EUR_10.Text = Ts.eur10.ToString();
                textBox_EUR_20.Text = Ts.eur20.ToString();
                textBox_EUR_50.Text = Ts.eur50.ToString();

                textBox_EUR_100.Text = Ts.eur100.ToString();
                textBox_EUR_200.Text = Ts.eur200.ToString();
                textBox_EUR_500.Text = Ts.eur500.ToString();

                decimal NotesTotal = Ts.eur5 * 5
                        + Ts.eur10 * 10
                        + Ts.eur20 * 20
                        + Ts.eur50 * 50

                        + Ts.eur100 * 100
                        + Ts.eur200 * 200
                        + Ts.eur500 * 500
                        ;

                textBoxEurNotesAmnt.Text = NotesTotal.ToString("#,##0.00");
                textBoxEuroEnter.Text = WTransAmount.ToString("#,##0.00");
                textBoxEuroDiff.Text = (WTransAmount- NotesTotal).ToString("#,##0.00");

                label3WithDrawl.Hide();
                panel9.Hide();
                label79.Hide();
               
            }
            else
            {
                labelThisTXN.Hide();
                labelNotesAmnt.Hide();
                labelEntered.Hide();
                textBoxEurNotesAmnt.Hide();
                textBoxEuroEnter.Hide();
                labelEuroDiff.Hide();
                textBoxEuroDiff.Hide();
                label2.Hide();

                textBox_EUR_5.Hide(); 
                textBox_EUR_10.Hide();
                textBox_EUR_20.Hide();
                textBox_EUR_50.Hide();

                textBox_EUR_100.Hide();
                textBox_EUR_200.Hide();
                textBox_EUR_500.Hide();
            }
            

            //     public int eur5;
            //public int eur10;
            //public int eur20;
            //public int eur50;

            //public int eur100;
            //public int eur200;
            //public int eur500;
            textBoxNE5.Text = Ts.eur5_TOT.ToString();
            textBoxNE10.Text = Ts.eur10_TOT.ToString();
            textBoxNE20.Text = Ts.eur20_TOT.ToString();
            textBoxNE50.Text = Ts.eur50_TOT.ToString();

            textBoxNE100.Text = Ts.eur100_TOT.ToString();
            textBoxNE200.Text = Ts.eur200_TOT.ToString();
            textBoxNE500.Text = Ts.eur500_TOT.ToString();

            textBoxEV5.Text = (Ts.eur5_TOT * 5).ToString("#,##0.00");
            textBoxEV10.Text = (Ts.eur10_TOT * 10).ToString("#,##0.00");
            textBoxEV20.Text = (Ts.eur20_TOT * 20).ToString("#,##0.00");
            textBoxEV50.Text = (Ts.eur50_TOT * 50).ToString("#,##0.00");

            textBoxEV100.Text = (Ts.eur100_TOT * 100).ToString("#,##0.00");
            textBoxEV200.Text = (Ts.eur200_TOT * 200).ToString("#,##0.00");
            textBoxEV500.Text = (Ts.eur500_TOT * 500).ToString("#,##0.00");

            decimal EuroTotal =
                (Ts.eur5_TOT * 5)
                + (Ts.eur10_TOT * 10)
                  + (Ts.eur20_TOT * 20)
                    + (Ts.eur50_TOT * 50)

                      + (Ts.eur100_TOT * 100)
                        + (Ts.eur200_TOT * 200)
                          + (Ts.eur500_TOT * 500); 

            textBoxTotalEuro.Text = EuroTotal.ToString("#,##0.00");

            return; 

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

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

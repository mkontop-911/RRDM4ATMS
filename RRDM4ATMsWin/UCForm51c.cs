using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51c : UserControl
    {
        // Working variables

        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;
        

        Form38_CDM NForm38_CDM;

        Form24 NForm24;

        bool ViewWorkFlow; 

        bool WSetScreen;

        string From_SM;

        DateTime WDateFrom;
        DateTime WDateTo;

        //    int Process;

        //    NotesBalances Na = new NotesBalances(); // Class Notes 
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Class Traces 

        RRDMDepositsClass Da = new RRDMDepositsClass(); // Contains all Deposits and Cheques 

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // see if deposits errors 

        RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        //    int WFunction;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
    
        string WAtmNo;
        int WSesNo;
        
        public void UCForm51cPar(string InSignedId, int InSignRecordNo, string InOperator,  string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
   
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            tableLayoutPanel11.Dock = DockStyle.Top;

            WSetScreen = true;

            

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 12 || Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            // Set currency headings
            RRDMGasParameters Gp = new RRDMGasParameters();
            //  CASH MANAGEMENT External?? 
            string ParId = "217";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            labelCurrency1.Text = Gp.OccuranceNm;

            OccurId = "2";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            labelCurrency2.Text = Gp.OccuranceNm;

            OccurId = "3";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            labelCurrency3.Text = Gp.OccuranceNm;

            OccurId = "4";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            labelCurrency3.Text = Gp.OccuranceNm;

            OccurId = "5";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            labelCurrency3.Text = Gp.OccuranceNm;

            OccurId = "6";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            labelCurrency3.Text = Gp.OccuranceNm;
            //
            // Check if Deposit from SM
            //
            ParId = "218";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            From_SM = Gp.OccuranceNm;

            if (From_SM == "YES")
            {
                // Get the totals from SM and not from Mpa            
                // GET TABLE
                SM.Read_SM_AND_FillTable_Deposits(WAtmNo, WSesNo);
                //
                //***********************
                //
                string SM_SelectionCriteria1 = " WHERE atmno ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + WSesNo
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

                SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, WSesNo, 2);

                if (SM.RecordFound == true)
                {
                    // Get other Totals 

                    int WDEP_COUNTERFEIT = SM.DEP_COUNTERFEIT;
                    int WDEP_SUSPECT = SM.DEP_SUSPECT;

                }

                // Read Table 

                int I = 0;

                while (I <= (SM.DataTable_SM_Deposits.Rows.Count - 1))
                {
                    // "  SELECT Currency As Ccy, SUM(Cassette) as TotalNotes, sum(Facevalue * CASSETTE) as TotalMoney "

                    string WCcy = (string)SM.DataTable_SM_Deposits.Rows[I]["Ccy"];
                    int WTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalNotes"];
                    int WTotalMoneyInt = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalMoney"];
                    decimal WTotalMoneyDecimal = WTotalMoneyInt * 100 / 100; 
                    I = I + 1; 
                }

            }

            Na.ReadAllErrorsTable(WAtmNo, WSesNo);

            if (Na.NumberOfErrDep > 0)
            {
                button3.Show(); 
            }
            else button3.Hide();

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            Ac.ReadAtm(WAtmNo);

            if (Ac.DepoRecycling == true)
            {
                labelCashDep.Text = "NON RECYCLED DEPOSITS";
            }

            // GET TOTALS FROM Mpa 
            // For deposits BNA and Cheques
            //Ta.SesDtTimeStart;
            //Ta.SesDtTimeEnd;
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            WDateFrom = Ta.SesDtTimeStart;
            WDateTo = Ta.SesDtTimeEnd;
            // Get the date 
            //Ta.SesDtTimeStart;
            //Ta.SesDtTimeEnd;
            int WMode = 2; 
            Mpa.ReadTableDepositsTxnsByAtmNoAndReplCycle_EGP(WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, labelCurrency1.Text, WMode, 2);

            Da.DepositsMachine1.Trans = Mpa.TotNoBNA;
            Da.DepositsMachine1.Amount = Mpa.TotValueBNA;
            Da.DepositsMachine1.Envelops = 0;
            Da.DepositsMachine1.EnvAmount = 0;
            Da.ChequesMachine1.Trans = 0;
            Da.ChequesMachine1.Amount = 0;

            Da.UpdateDepositsNaWithMachineTotals(WAtmNo, WSesNo);
            // GET totals For Suspect Notes and Fake
            //
            int Mode = 1; // only 
            Er.ReadAllErrorsTableToFindTotalsForSuspectAndFake(WAtmNo, WSesNo, 0, Mode);

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            
            //cmd.Parameters.AddWithValue("@DepTransMach", DepositsMachine1.Trans);
            //cmd.Parameters.AddWithValue("@DepNotesMach", 0); // Value not available yet 
            //cmd.Parameters.AddWithValue("@DepAmountMach", DepositsMachine1.Amount);
            //cmd.Parameters.AddWithValue("@DepNotesRejMach", 0); // Not available yet 
            //cmd.Parameters.AddWithValue("@DepAmountRejMach", 0); // Not Availble yet 

            //cmd.Parameters.AddWithValue("@EnvelopsMach", DepositsMachine1.Envelops);
            //cmd.Parameters.AddWithValue("@EnvAmountMach", DepositsMachine1.EnvAmount);

            //cmd.Parameters.AddWithValue("@ChequesTransMach", ChequesMachine1.Trans);
            //cmd.Parameters.AddWithValue("@ChequesNoMach", 0); // WEDo not currently have the value 
            //cmd.Parameters.AddWithValue("@ChequesAmountMach", ChequesMachine1.Amount);
            
         //   textBoxDepositsMachine1Trans.Text = Mpa.TotNoBNA.ToString();
            textBoxDepositsMachine1Notes.Text = Mpa.TotNoBNA.ToString();
            textBoxDepositsMachine1Amount.Text = Mpa.TotValueBNA.ToString("#,##0.00");
            textBoxDepositsMachine1NotesRej.Text = Er.Total_ErrId_225.ToString();
            textBoxDepositsMachine1AmountRej.Text = Er.Total_ErrId_225_Value.ToString("#,##0.00");
            // FAKE
            textBoxDepositsMachineFakeNotes.Text = Er.Total_ErrId_226.ToString();
            textBoxDepositsMachineFakeAmount.Text = Er.Total_ErrId_226_Value.ToString("#,##0.00");
         
            // Cheques 

            textBoxChequesMachine1Trans.Text = Mpa.TotNoCh.ToString();
            textBoxChequesMachine1Number.Text = Mpa.TotNoCh.ToString();
            textBoxChequesMachine1Amount.Text = Mpa.TotValueCh.ToString("#,##0.00");
            //
            // COUNT
            // 
          //  textBoxDepositsCount1Trans.Text = Da.DepositsCount1.Trans.ToString();
            textBoxDepositsCount1Notes.Text = Da.DepositsCount1.Notes.ToString();
            textBoxDepositsCount1Amount.Text = Da.DepositsCount1.Amount.ToString("#,##0.00");
            textBoxDepositsCount1NotesRej.Text = Da.DepositsCount1.NotesRej.ToString();
            textBoxDepositsCount1AmountRej.Text = Da.DepositsCount1.AmountRej.ToString("#,##0.00");

            textBoxFakeCount.Text = Da.DepositsCount1.Envelops.ToString();
            textBoxFakeAmt.Text = Da.DepositsCount1.EnvAmount.ToString("#,##0.00");

            textBoxChequesCount1Trans.Text = Da.ChequesCount1.Trans.ToString();
            textBoxChequesCount1Number.Text = Da.ChequesCount1.Number.ToString();
            textBoxChequesCount1Amount.Text = Da.ChequesCount1.Amount.ToString("#,##0.00");

            // Differences 

            ShowDifferences(); 
       
            // Show Total Balances 

            textBoxDepositsBNACount.Text = (Da.DepositsCount1.Amount).ToString("#,##0.00");
            textBoxSuspectCount.Text = (Da.DepositsCount1.EnvAmount).ToString("#,##0.00");
            textBoxChequesCount.Text = (Da.ChequesCount1.Amount).ToString("#,##0.00");

            textBoxDepositsBNAMachine.Text = Mpa.TotValueBNA.ToString("#,##0.00");
            textBoxSuspectMachine.Text = Er.Total_ErrId_225_Value.ToString("#,##0.00");
            textBoxChequesMachine.Text = Mpa.TotValueCh.ToString("#,##0.00");

            textBoxBNADiff.Text = (Mpa.TotValueBNA- Da.DepositsCount1.Amount).ToString("#,##0.00");
            textBoxSuspectDiff.Text = (Da.DepositsCount1.EnvAmount - (Er.Total_ErrId_225_Value)).ToString("#,##0.00");
            textBoxChequesDiff.Text = ((Da.ChequesCount1.Amount) - (Mpa.TotValueCh)).ToString("#,##0.00");

            textBoxExpected.Text = Mpa.TotValueBNA.ToString("#,##0.00");
            textBoxDifference.Text = (Da.DepositsCount1.Amount - Mpa.TotValueBNA).ToString("#,##0.00");

            guidanceMsg = " INPUT DATA AND UPDATE";

            // Handle request from Reconciliation 

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (ViewWorkFlow == true)  // If 13 the request came from Reconciliation and not from Replenishemnt 
            {
                button6.Hide();
                button5.Hide();
                button3.Hide();
                button1.Hide();

          //      textBoxDepositsCount1Trans.ReadOnly = true;
                textBoxDepositsCount1Notes.ReadOnly = true;
                textBoxDepositsCount1Amount.ReadOnly = true;
                textBoxDepositsCount1AmountRej.ReadOnly = true;
                textBoxFakeCount.ReadOnly = true;
                textBoxFakeAmt.ReadOnly = true;
                textBoxChequesCount1Trans.ReadOnly = true;
                textBoxChequesCount1Number.ReadOnly = true;
                textBoxChequesCount1Amount.ReadOnly = true;

            //    guidanceMsg = " View only "; // Moved to form51
            }

            WSetScreen = false;   

        }
        // 
        // SHOW ATMs FIGURES AS COUNTED 
        //
        private void button6_Click(object sender, EventArgs e)
        {
            button5.Show(); // Show Update button 
        //    textBoxDepositsDiff1Trans.Text = "0";
            textBoxDepositsDiff1Notes.Text = "0";
            textBoxDepositsDiff1Amount.Text = "0.00";
            textBoxDepositsDiff1NotesRej.Text = "0";
            textBoxDepositsDiff1AmountRej.Text = "0.00";
            textBoxDiffFakeNotes.Text = "0";
            textBoxDepositsDiffFakeAmt.Text = "0.00";

            textBoxChequesDiff1Trans.Text = "0";
            textBoxChequesDiff1Number.Text = "0";
            textBoxChequesDiff1Amount.Text = "0.00";

          //  textBoxDepositsCount1Trans.Text = textBoxDepositsMachine1Trans.Text;
            textBoxDepositsCount1Notes.Text = textBoxDepositsMachine1Notes.Text;
            textBoxDepositsCount1Amount.Text = textBoxDepositsMachine1Amount.Text;
            textBoxDepositsCount1NotesRej.Text = textBoxDepositsMachine1NotesRej.Text;
            textBoxDepositsCount1AmountRej.Text = textBoxDepositsMachine1AmountRej.Text;

            textBoxFakeCount.Text = textBoxDepositsMachineFakeNotes.Text;
            textBoxFakeAmt.Text = textBoxDepositsMachineFakeAmount.Text;

            textBoxChequesCount1Trans.Text = textBoxChequesMachine1Trans.Text;
            textBoxChequesCount1Number.Text = textBoxChequesMachine1Number.Text;
            textBoxChequesCount1Amount.Text = textBoxChequesMachine1Amount.Text;

            // Show Total Balances 
            // COUNT FIGURES WILL BE THE SAME AS PER MACHINE 

            Da.DepositsCount1.Amount = Da.DepositsMachine1.Amount;
            Da.DepositsCount1.AmountRej = Da.DepositsMachine1.AmountRej;
            Da.DepositsCount1.EnvAmount = Da.DepositsMachine1.EnvAmount;
            Da.ChequesCount1.Amount = Da.ChequesMachine1.Amount ;

            textBoxDepositsBNACount.Text = textBoxDepositsMachine1Amount.Text; 
            textBoxSuspectCount.Text = textBoxDepositsMachine1NotesRej.Text;
            textBoxChequesCount.Text = textBoxChequesMachine1Amount.Text;

            textBoxDepositsBNAMachine.Text = textBoxDepositsMachine1Amount.Text;
            textBoxSuspectMachine.Text = textBoxDepositsMachine1NotesRej.Text;
            textBoxChequesMachine.Text = textBoxChequesMachine1Amount.Text;

            textBoxBNADiff.Text = "0.00";
            textBoxSuspectDiff.Text = "0.00";
            textBoxChequesDiff.Text = "0.00";

            textBoxExpected.Text = Mpa.TotValueBNA.ToString("#,##0.00");
            textBoxDifference.Text = (Mpa.TotValueBNA - Mpa.TotValueBNA).ToString("#,##0.00");

            guidanceMsg = " VERIFY FIGURES, CHANGE THEM IF NEEDED AND PRESS UPDATE";

            ChangeBoardMessage(this, e);

        }
        

        //
        // With UPDATE BUTTOM Update ONLY AND SHOW DIFF AND BALANCES 
        //

        private void button5_Click(object sender, EventArgs e)
        {
            // Update Session Notes with Input data

            // Read Input Values
            ReadInputValues();

            Da.UpdateDepositsSessionsNotesAndValuesWithCount(WAtmNo, WSesNo); // UPDATE INPUT VALUES

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo); // READ UPDATED DATA 

            // Show differences 

            ShowDifferences(); 

            // Show Total Balances 

            textBoxDepositsBNACount.Text = (Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej).ToString("#,##0.00");
            textBoxSuspectCount.Text = (Da.DepositsCount1.EnvAmount).ToString("#,##0.00");
            textBoxChequesCount.Text = (Da.ChequesCount1.Amount).ToString("#,##0.00");

            textBoxDepositsBNAMachine.Text = (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej).ToString("#,##0.00");
            textBoxSuspectMachine.Text = (Da.DepositsMachine1.EnvAmount).ToString("#,##0.00");
            textBoxChequesMachine.Text = (Da.ChequesMachine1.Amount).ToString("#,##0.00");

            textBoxBNADiff.Text = ((Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej) - (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej)).ToString("#,##0.00");
            textBoxSuspectDiff.Text = ((Da.DepositsCount1.EnvAmount) - (Da.DepositsMachine1.EnvAmount)).ToString("#,##0.00");
            textBoxChequesDiff.Text = ((Da.ChequesCount1.Amount) - (Da.ChequesMachine1.Amount)).ToString("#,##0.00");

// Check if data inputed 
            if ((Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej) > 0 || (Da.DepositsMachine1.EnvAmount) > 0 || (Da.ChequesMachine1.Amount) > 0)
            {
                // Check if input was made 
                if ((Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej) == 0 & (Da.DepositsCount1.EnvAmount) == 0
                    & (Da.ChequesCount1.Amount) == 0 )
                {
                    MessageBox.Show("Please enter counted data");
                    return;
                }
            }
            //
            // Create GL TXNS 
            //
            if ((Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej)>0)
            {
                // There are deposits 
                CreateActions_Occurances();
            }
            else
            {

            }
            

            // Update STEP

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep3_Updated = true;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

         //   MessageBox.Show("Input Data is updated.");

            //Form2MessageBox Mb = new Form2MessageBox("Input Data has been updated.");
            //Mb.StartPosition = FormStartPosition.Manual;
            //Mb.Location = new Point(300, 480);
            //Mb.ShowDialog();


            button5.Hide(); 

            if (Da.DiffInDeposits == true || Da.DiffInCheques == true)
            {
                guidanceMsg = " Updating done but with differences! - Move to next step. ";
            }
            else
            {
                guidanceMsg = " Updating done .. No Differences !- Move to next step. ";
            }
            
            ChangeBoardMessage(this, e);

           
        }

        protected override CreateParams CreateParams
        {

            get
            {

                CreateParams handleParam = base.CreateParams;

                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED        

                return handleParam;

            }

        }
        // SHOW ERRORS 
        private void button3_Click(object sender, EventArgs e)
        {
            bool Replenishment = true;
       //     string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrId > 200 AND ErrId < 299) AND OpenErr =1";
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrId > 200 AND ErrId < 299)";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.Show();
        }
// INPUT VALUES
        private void ReadInputValues()
        {
            // DEPOSITS 

            //if (int.TryParse(textBoxDepositsCount1Trans.Text, out Da.DepositsCount1.Trans))
            //{
            //    //  MessageBox.Show(textBox33.Text, "The input number is correct!");
            //}
            //else
            //{
            //    MessageBox.Show(textBoxDepositsCount1Trans.Text, "Please enter a valid number!");
            //    return;

            //}
            //
            if (int.TryParse(textBoxDepositsCount1Notes.Text, out Da.DepositsCount1.Notes))
            {
                // MessageBox.Show(textBox9.Text, "The input number is correct!"); 

            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1Notes.Text, "Please enter a valid number!");
                return;
            }
            //
            if (decimal.TryParse(textBoxDepositsCount1Amount.Text, out Da.DepositsCount1.Amount))
            {
                //  MessageBox.Show(textBox10.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1Amount.Text, "Please enter a valid number!");
                return;
            }
            // 
            if (int.TryParse(textBoxDepositsCount1NotesRej.Text, out Da.DepositsCount1.NotesRej))
            {
                //  MessageBox.Show(textBox11.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1NotesRej.Text, "Please enter a valid number!");
                return;
            }

            if (decimal.TryParse(textBoxDepositsCount1AmountRej.Text, out Da.DepositsCount1.AmountRej))
            {
                //  MessageBox.Show(textBox1.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxDepositsCount1AmountRej.Text, "Please enter a valid number!");
                return;
            }
            //FAKE
            // 
            if (int.TryParse(textBoxFakeCount.Text, out Da.DepositsCount1.Envelops))
            {
                //  MessageBox.Show(textBox11.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxFakeCount.Text, "Please enter a valid number!");
                return;
            }

            if (decimal.TryParse(textBoxFakeAmt.Text, out Da.DepositsCount1.EnvAmount))
            {
                //  MessageBox.Show(textBox1.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxFakeAmt.Text, "Please enter a valid number!");
                return;
            }

            // 
            // CHEQUES
            //
            if (int.TryParse(textBoxChequesCount1Trans.Text, out Da.ChequesCount1.Trans))
            {
                //  MessageBox.Show(textBox34.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxChequesCount1Trans.Text, "Please enter a valid number!");
                return;
            }

            if (int.TryParse(textBoxChequesCount1Number.Text, out Da.ChequesCount1.Number))
            {
                //  MessageBox.Show(textBox12.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxChequesCount1Number.Text, "Please enter a valid number!");
                return;
            }

            if (decimal.TryParse(textBoxChequesCount1Amount.Text, out Da.ChequesCount1.Amount))
            {
                //  MessageBox.Show(textBox13.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxChequesCount1Amount.Text, "Please enter a valid number!");
                return;
            }


        }
        // Differences
        private void ShowDifferences()
        {
            // Show differences 

          //  textBoxDepositsDiff1Trans.Text = (Mpa.TotNoBNA - Da.DepositsCount1.Trans).ToString();
            textBoxDepositsDiff1Notes.Text = (Mpa.TotNoBNA - Da.DepositsCount1.Notes).ToString();
            textBoxDepositsDiff1Amount.Text = (Mpa.TotValueBNA - Da.DepositsCount1.Amount).ToString("#,##0.00");
            textBoxDepositsDiff1NotesRej.Text = (Er.Total_ErrId_225 - Da.DepositsCount1.NotesRej).ToString();
            textBoxDepositsDiff1AmountRej.Text = (Er.Total_ErrId_225_Value - Da.DepositsCount1.AmountRej).ToString("#,##0.00");

            textBoxDiffFakeNotes.Text = (Er.Total_ErrId_226 - Da.DepositsCount1.Envelops).ToString();
            textBoxDepositsDiffFakeAmt.Text = (Er.Total_ErrId_226_Value - Da.DepositsCount1.EnvAmount).ToString("#,##0.00");

            textBoxChequesDiff1Trans.Text = (Mpa.TotNoCh - Da.ChequesCount1.Trans).ToString();
            textBoxChequesDiff1Number.Text = "0";
            textBoxChequesDiff1Amount.Text = (Mpa.TotValueCh - Da.ChequesCount1.Amount).ToString("#,##0.00");

        }



        // Show Deposits 
        private void button1_Click(object sender, EventArgs e)
        {
            
            NForm38_CDM = new Form38_CDM(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            NForm38_CDM.Show();
        }
        //
// My Count Change
        // 
        private void textBox33_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox41_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox42_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel(); 
        }
        // SET step level to need update if changes 
        private void SetSteplevel()
        {
            if (WSetScreen == false)
            {
                // Update STEP

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ReplStep3_Updated = false;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                button5.Show();
            }
        }

        private void textBox33_TextChanged_1(object sender, EventArgs e)
        {

        }

// Show Notes 
        private void buttonShowNotes_Click(object sender, EventArgs e)
        {
            Form78d_DepositedNotes NForm78d_DepositedNotes;
            NForm78d_DepositedNotes = new Form78d_DepositedNotes(WOperator, WSignedId, WAtmNo, WSesNo,1);
            NForm78d_DepositedNotes.Show();
        }
        // Create Action Occurances
        DataTable TEMPTableFromAction;
        string WUniqueRecordIdOrigin = "Replenishment";
        public void CreateActions_Occurances()
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;

            if (decimal.TryParse(textBoxDepositsBNACount.Text, out DoubleEntryAmt))
            {
            }
            else
            {
                //MessageBox.Show(textBoxExp.Text, "Please enter a valid number for Expected!");

                //return;
            }

            // FIRST DOUBLE ENTRY 
            WActionId = "26"; // 26_CREDIT CIT Account/DR_AtmCash (DEPOSITS)
                              // WUniqueRecordIdOrigin = "Replenishment";
            WUniqueRecordId = WSesNo; // SesNo 
            WCcy = "EGP";
            //DoubleEntryAmt = Na.Balances1.CountedBal;
            string WMaker_ReasonOfAction = "UnLoad Deposits";
            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                  WActionId, WUniqueRecordIdOrigin,
                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                  , WMaker_ReasonOfAction, "Replenishment");


            TEMPTableFromAction = Aoc.TxnsTableFromAction;

            
            WActionId = "27";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            //
            WActionId = "28";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            decimal DiffGL = 0;
            
            if (decimal.TryParse(textBoxBNADiff.Text, out DiffGL))
            {
            }
            else
            {
                //MessageBox.Show(textBoxExp.Text, "Please enter a valid number for Expected!");

                //return;
            }

            if (DiffGL == 0)
            {
                // do nothing
            }

            if (DiffGL > 0)
            {
                MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#,##0.00") + Environment.NewLine
                        + "Will be moved to the Branch excess account "); 
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
               WActionId = "28"; //28_CREDIT Branch Excess/DR_AtmCash(DEPOSITS)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = DiffGL;
                WMaker_ReasonOfAction = "UnLoad Deposits-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (DiffGL < 0)
            {
                MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#,##0.00") + Environment.NewLine
                        + "Will be moved to the Branch shortage account ");
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                WActionId = "27"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -DiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad Deposits-Shortages";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
        }


        // HOW GL TILL NOW 
        private void buttonShowGL_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo
                 + " AND (OriginWorkFlow ='Replenishment' OR OriginWorkFlow ='Dispute') ";
            Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

            Aoc.ClearTableTxnsTableFromAction();

            int I = 0;

            while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
            {

                int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                int WMode2 = 1; // 

                Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey, Aoc.ActionId, Aoc.Occurance
                                                             , Aoc.OriginWorkFlow, WMode2);
                I = I + 1;
            }

            DataTable TempTxnsTableFromAction;
            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;

            Form14b_All_Actions NForm14b_All_Actions;

            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();

        }
        // Change the expected number 
        private void textBoxExpected_TextChanged(object sender, EventArgs e)
        {
            decimal InputExp = 0;
            decimal InputCount = 0; 
            if (decimal.TryParse(textBoxExpected.Text, out InputExp))
            {
            }
            else
            {
                //MessageBox.Show(textBoxExp.Text, "Please enter a valid number for Expected!");

                //return;
            }

            if (decimal.TryParse(textBoxDepositsBNACount.Text, out InputCount))
            {
            }
            else
            {
                //MessageBox.Show(textBoxExp.Text, "Please enter a valid number for Expected!");

                //return;
            }

            
            textBoxDifference.Text = (InputCount - InputExp).ToString("#,##0.00");

        }
    }
}

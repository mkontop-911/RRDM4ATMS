using RRDM4ATMs;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class UCForm51b : UserControl
    {

       // Form24 NForm24; // Show the errors 

        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool ViewWorkFlow;
        bool WSetScreen;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Class Traces 

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

        RRDMJournalAudi_BDC Jnl = new RRDMJournalAudi_BDC();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        //RRDMErrorsClassWithActions Err = new RRDMErrorsClassWithActions();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // string WUserOperator;
        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;

        bool AudiType;

        int WFunction;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;


        public void UCForm51bPar(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
             AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;
                    buttonGLTxns.Hide();
                }
                else
                {
                    buttonGLTxns.Show();
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            DateTmSesStart = Ta.SesDtTimeStart;
            DateTmSesEnd = Ta.SesDtTimeEnd;

            // ================USER BANK =============================
            //       Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //       WUserOperator = Us.Operator;
            // ========================================================

            this.DoubleBuffered = true;
            SetScreen();
            //   button2.Hide();
        }

        public void SetScreen()
        {
            //buttonUpdateInput.Hide();

            if (WOperator == "ETHNCY2N")
            {
                linkLabelShowDiscrepancies.Hide();
            }
            Ac.ReadAtm(WAtmNo);
            if (Ac.NoCassettes == 4)
            {
                label32.Hide();
                label35.Hide();
            }

            WSetScreen = true;
            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 11 || Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // Read Values from NOTES

            // Cassette 1
            int temp = Convert.ToInt32(Na.Cassettes_1.FaceValue);
            label13.Text = "Type 1- " + temp.ToString() + " " + Na.Cassettes_1.CurName;
            textBox11.Text = Na.Cassettes_1.RemNotes.ToString();
            label11.Text = "Type 1- " + temp.ToString() + " " + Na.Cassettes_1.CurName;
            textBox22.Text = Na.Cassettes_1.RejNotes.ToString();

            textBox1.Text = Na.Cassettes_1.CasCount.ToString();
            textBox5.Text = Na.Cassettes_1.RejCount.ToString();

            // Cassette 2
            temp = Convert.ToInt32(Na.Cassettes_2.FaceValue);
            label14.Text = "Type 2 - " + temp.ToString() + " " + Na.Cassettes_2.CurName;
            textBox13.Text = Na.Cassettes_2.RemNotes.ToString();
            label9.Text = "Type 2 - " + temp.ToString() + " " + Na.Cassettes_2.CurName;
            textBox24.Text = Na.Cassettes_2.RejNotes.ToString();

            textBox2.Text = Na.Cassettes_2.CasCount.ToString();
            textBox6.Text = Na.Cassettes_2.RejCount.ToString();

            // Cassette 3
            temp = Convert.ToInt32(Na.Cassettes_3.FaceValue);
            label16.Text = "Type 3 - " + temp.ToString() + " " + Na.Cassettes_3.CurName;
            textBox15.Text = Na.Cassettes_3.RemNotes.ToString();
            label8.Text = "Type 3 - " + temp.ToString() + " " + Na.Cassettes_3.CurName;
            textBox26.Text = Na.Cassettes_3.RejNotes.ToString();

            textBox3.Text = Na.Cassettes_3.CasCount.ToString();
            textBox7.Text = Na.Cassettes_3.RejCount.ToString();

            // Cassette 4
            temp = Convert.ToInt32(Na.Cassettes_4.FaceValue);
            label17.Text = "Type 4 - " + temp.ToString() + " " + Na.Cassettes_4.CurName;
            textBox17.Text = Na.Cassettes_4.RemNotes.ToString();
            label7.Text = "Type 4 - " + temp.ToString() + " " + Na.Cassettes_4.CurName;
            textBox19.Text = Na.Cassettes_4.RejNotes.ToString();

            textBox4.Text = Na.Cassettes_4.CasCount.ToString();
            textBox8.Text = Na.Cassettes_4.RejCount.ToString();

            // Show differences in Notes  

            textBox12.Text = Na.Cassettes_1.DiffCas.ToString();
            textBox21.Text = (Na.Cassettes_1.FaceValue * Na.Cassettes_1.DiffCas).ToString("#,##0.00");
            textBox23.Text = Na.Cassettes_1.DiffRej.ToString();
            textBox41.Text = (Na.Cassettes_1.FaceValue * Na.Cassettes_1.DiffRej).ToString("#,##0.00");

            textBox14.Text = Na.Cassettes_2.DiffCas.ToString();
            textBox45.Text = (Na.Cassettes_2.FaceValue * Na.Cassettes_2.DiffCas).ToString("#,##0.00");
            textBox25.Text = Na.Cassettes_2.DiffRej.ToString();
            textBox42.Text = (Na.Cassettes_2.FaceValue * Na.Cassettes_2.DiffRej).ToString("#,##0.00");

            textBox16.Text = Na.Cassettes_3.DiffCas.ToString();
            textBox46.Text = (Na.Cassettes_3.FaceValue * Na.Cassettes_3.DiffCas).ToString("#,##0.00");
            textBox27.Text = Na.Cassettes_3.DiffRej.ToString();
            textBox43.Text = (Na.Cassettes_3.FaceValue * Na.Cassettes_3.DiffRej).ToString("#,##0.00");

            textBox18.Text = Na.Cassettes_4.DiffCas.ToString();
            textBox47.Text = (Na.Cassettes_4.FaceValue * Na.Cassettes_4.DiffCas).ToString("#,##0.00");
            textBox20.Text = Na.Cassettes_4.DiffRej.ToString();
            textBox44.Text = (Na.Cassettes_4.FaceValue * Na.Cassettes_4.DiffRej).ToString("#,##0.00");

            // RETRACTED 
            string WSelectionCriteria = " WHERE "
                       + "   PresenterError= 'PresenterError' "
                               + "AND AtmNo ='" + WAtmNo + "'  ";

            Jnl.FillTablePresentedFromJournal(WOperator, WSignedId,
                                     WSelectionCriteria,
                                    DateTmSesStart, DateTmSesEnd);

            textBoxType1.Text = Jnl.Tot_Type1.ToString();
            textBoxType2.Text = Jnl.Tot_Type2.ToString();
            textBoxType3.Text = Jnl.Tot_Type3.ToString();
            textBoxType4.Text = Jnl.Tot_Type4.ToString();

            // Capture Cards 

            textBox9.Text = textBox48.Text = Na.CaptCardsMachine.ToString();
            textBox34.Text = Na.CaptCardsCount.ToString();

            textBox10.Text = textBox49.Text = (Na.CaptCardsCount - Na.CaptCardsMachine).ToString(); // Captured Differences 

            if (Na.ErrJournalThisCycle > 0)
            {
                buttonShowErrors.Show();
            }
            else buttonShowErrors.Hide();

            if (Na.Balances1.PresenterValue >0 )
            {
                textBoxPresenterError.Show();
                textBoxPresenterError.Text = "There is presenter error/s" + Environment.NewLine
                    + " of the value of :.." + Na.Balances1.PresenterValue.ToString();
            }
            else
            {
                textBoxPresenterError.Hide();
            }

          

            ShowBalances();// SHOW BALANCES 

            guidanceMsg = " Input numbers of counted Notes And Press UPDATE - You can make corrections at wish  ";

            // View only 

            if (ViewWorkFlow == true)
            {
                buttonUpdateInput.Hide();
                //buttonShowErrors.Hide();
                buttonUseAtmsFigures.Hide();
                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.ReadOnly = true;
                textBox6.ReadOnly = true;
                textBox7.ReadOnly = true;
                textBox8.ReadOnly = true;
                textBox34.ReadOnly = true;

                //    guidanceMsg = " View only "; // Move to form51
            }

            WSetScreen = false;

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

        // Create and Show input as per ATM
        // 
        private void buttonUseAtmsFigures_Click(object sender, EventArgs e)
        {
            // INITIALISE FORM 
            //     panel1.Hide();
            label24.Hide();
            label3.Hide();
            textBox12.Text = " ";
            textBox23.Text = " ";
            textBox14.Text = " ";
            textBox25.Text = " ";
            textBox16.Text = " ";
            textBox27.Text = " ";
            textBox18.Text = " ";
            textBox20.Text = " ";

            textBox10.Text = " ";

            // SHOW 
            // Cassette 1
            textBox1.Text = Na.Cassettes_1.RemNotes.ToString();
            textBox5.Text = Na.Cassettes_1.RejNotes.ToString();

            // Cassette 2
            textBox2.Text = Na.Cassettes_2.RemNotes.ToString();
            textBox6.Text = Na.Cassettes_2.RejNotes.ToString();

            // Cassette 3
            textBox3.Text = Na.Cassettes_3.RemNotes.ToString();
            textBox7.Text = Na.Cassettes_3.RejNotes.ToString();

            // Cassette 4
            textBox4.Text = Na.Cassettes_4.RemNotes.ToString();
            textBox8.Text = Na.Cassettes_4.RejNotes.ToString();

            textBox34.Text = Na.CaptCardsMachine.ToString();

            Na.Balances1.CountedBal = Na.Balances1.MachineBal;
            Na.Balances2.CountedBal = Na.Balances2.MachineBal;
            Na.Balances3.CountedBal = Na.Balances3.MachineBal;
            Na.Balances4.CountedBal = Na.Balances4.MachineBal;

            Na.BalDiff1.Machine = 0;
            Na.BalDiff2.Machine = 0;
            Na.BalDiff3.Machine = 0;
            Na.BalDiff4.Machine = 0;

            // Show differences in Notes  

            textBox12.Text = "0";
            textBox23.Text = "0";
            textBox14.Text = "0";
            textBox25.Text = "0";
            textBox16.Text = "0";
            textBox27.Text = "0";
            textBox18.Text = "0";
            textBox20.Text = "0";

            textBox10.Text = "0";

            textBox21.Text = "0.00";
            textBox45.Text = "0.00";
            textBox46.Text = "0.00";
            textBox47.Text = "0.00";
            textBox41.Text = "0.00";
            textBox42.Text = "0.00";
            textBox43.Text = "0.00";
            textBox44.Text = "0.00";

            ShowBalances();// SHOW BALANCES 

            textBoxStatus.Text = "Counted Equals To Machine Totals";

            guidanceMsg = " VERIFY FIGURES CHANGE THEM IF YOU WANT AND press UPDATE";

            ChangeBoardMessage(this, e);
        }

        //
        // With UPDATE BUTTOM Update ONLY AND SHOW DIFF AND BALANCES 
        //

        private void buttonUpdateInput_Click(object sender, EventArgs e)
        {
            // Check if actions Already taken from presenter errors action screen
            string WSelectionCriteria = " WHERE AtmNo = '"+WAtmNo +"' AND ReplCycle ="+WSesNo + " AND ActionId in ('90','92', '95', '96')  AND OriginWorkFlow = 'Replenishment' "; 
            Aoc.ReadCheckActionsOccuarnceBySelectionCriteria(WSelectionCriteria); 

            if (Aoc.RecordFound == true)
            {
                MessageBox.Show(
                    "You are trying to update with new figures ... but ..." + Environment.NewLine
                    + "You have done an action on the presenter screen." + Environment.NewLine
                    + "Undo action you have taken on Presenter screen" + Environment.NewLine
                    +"Then come back to this screen and press update " + Environment.NewLine
                    +"Only this way the new counted figures will be accepted." + Environment.NewLine
                    + "Then proceed to the next " + Environment.NewLine
                    + "Take action on presenter "
                    );

                MessageBox.Show(
                    "The new figures has not be updated" + Environment.NewLine
                    );

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ReplStep2_Updated = true;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                return; 
            }

            //// VALIDATION 
            //if (textBox1.Text == "0" & textBox2.Text == "0" & textBox3.Text == "0" & textBox4.Text == "0"
            //     & textBox5.Text == "0" & textBox6.Text == "0" & textBox7.Text == "0" & textBox8.Text == "0")
            //{
            //    MessageBox.Show("Please enter counted values !");
            //    return;
            //}
            //***** 1
            if (int.TryParse(textBox1.Text, out Na.Cassettes_1.CasCount))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid number!");

                return;
            }


            if (int.TryParse(textBox5.Text, out Na.Cassettes_1.RejCount))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid number!");
                return;
            }

            //****** 2
            if (int.TryParse(textBox2.Text, out Na.Cassettes_2.CasCount))
            {
            }
            else
            {
                MessageBox.Show(textBox2.Text, "Please enter a valid number!");
                return;
            }


            if (int.TryParse(textBox6.Text, out Na.Cassettes_2.RejCount))
            {
            }
            else
            {
                MessageBox.Show(textBox6.Text, "Please enter a valid number!");
                return;
            }
            //***** 3

            if (int.TryParse(textBox3.Text, out Na.Cassettes_3.CasCount))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number!");
                return;
            }


            if (int.TryParse(textBox7.Text, out Na.Cassettes_3.RejCount))
            {
            }
            else
            {
                MessageBox.Show(textBox7.Text, "Please enter a valid number!");
                return;
            }

            //****** 4
            if (int.TryParse(textBox4.Text, out Na.Cassettes_4.CasCount))
            {
            }
            else
            {
                MessageBox.Show(textBox4.Text, "Please enter a valid number!");
                return;
            }


            if (int.TryParse(textBox8.Text, out Na.Cassettes_4.RejCount))
            {
            }
            else
            {
                MessageBox.Show(textBox8.Text, "Please enter a valid number!");
                return;
            }
            //
            // Validation 
            // 
            if (Na.Cassettes_1.CasCount == 0 
                & Na.Cassettes_2.CasCount == 0
                & Na.Cassettes_3.CasCount == 0
                  &  Na.Cassettes_4.CasCount == 0

                  & Na.Cassettes_1.RejCount == 0
                   & Na.Cassettes_2.RejCount == 0
                    & Na.Cassettes_3.RejCount == 0
                     & Na.Cassettes_4.RejCount == 0
                )
            {
                // ALL INPUT DATA ARE ZERO 
                if ( checkBoxAcceptZero.Checked == false)
                {
                    MessageBox.Show("Please enter counted values !"+Environment.NewLine
                                    + "All input values are zero" + Environment.NewLine
                                     + "Check the Box Accept Zero if really want to enter zero" + Environment.NewLine
                                    );
                    return;
                }
                else
                {
                    // Continue check box Accept Zero is Checked. 
                    // Continue 
                }
            }

            // Captured Cards 

            if (int.TryParse(textBox34.Text, out Na.CaptCardsCount))
            {
            }
            else
            {
                MessageBox.Show(textBox34.Text, "Please enter a valid number!");
                return;
            }

            //
            // UPDATING 
            //
            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            // Read Session Notes to get the updated data 

            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

            // Show differences in Notes  

            textBox12.Text = Na.Cassettes_1.DiffCas.ToString();
            textBox21.Text = (Na.Cassettes_1.FaceValue * Na.Cassettes_1.DiffCas).ToString("#,##0.00");
            textBox23.Text = Na.Cassettes_1.DiffRej.ToString();
            textBox41.Text = (Na.Cassettes_1.FaceValue * Na.Cassettes_1.DiffRej).ToString("#,##0.00");

            textBox14.Text = Na.Cassettes_2.DiffCas.ToString();
            textBox45.Text = (Na.Cassettes_2.FaceValue * Na.Cassettes_2.DiffCas).ToString("#,##0.00");
            textBox25.Text = Na.Cassettes_2.DiffRej.ToString();
            textBox42.Text = (Na.Cassettes_2.FaceValue * Na.Cassettes_2.DiffRej).ToString("#,##0.00");

            textBox16.Text = Na.Cassettes_3.DiffCas.ToString();
            textBox46.Text = (Na.Cassettes_3.FaceValue * Na.Cassettes_3.DiffCas).ToString("#,##0.00");
            textBox27.Text = Na.Cassettes_3.DiffRej.ToString();
            textBox43.Text = (Na.Cassettes_3.FaceValue * Na.Cassettes_3.DiffRej).ToString("#,##0.00");

            textBox18.Text = Na.Cassettes_4.DiffCas.ToString();
            textBox47.Text = (Na.Cassettes_4.FaceValue * Na.Cassettes_4.DiffCas).ToString("#,##0.00");
            textBox20.Text = Na.Cassettes_4.DiffRej.ToString();
            textBox44.Text = (Na.Cassettes_4.FaceValue * Na.Cassettes_4.DiffRej).ToString("#,##0.00");

            textBox10.Text = (Na.CaptCardsCount - Na.CaptCardsMachine).ToString();

            //

            ShowBalances();// SHOW BALANCES 

            // Check if differences 
            if (Na.Cassettes_1.DiffCas == 0 & Na.Cassettes_1.DiffRej == 0 &
                Na.Cassettes_2.DiffCas == 0 & Na.Cassettes_2.DiffRej == 0 &
                Na.Cassettes_3.DiffCas == 0 & Na.Cassettes_3.DiffRej == 0 &
                Na.Cassettes_4.DiffCas == 0 & Na.Cassettes_4.DiffRej == 0
                )
            {
                if (Na.Balances1.PresenterValue > 0)
                {
                    guidanceMsg = "You may have to recount money! ";
                    buttonShowErrors.Show();

                    if (MessageBox.Show("Warning: Balances reconcile but there are error/s. Do you want to examine errors?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                     == DialogResult.Yes)
                    {
                        Form80b2_Unmatched NForm80b2;
                        string WFunction = "View";

                        // Show For Current Cycle number 
                        NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                            WAtmNo, DateTmSesStart, DateTmSesEnd, 3
                            );
                        NForm80b2.Show();
                        return;
                    }
                    else
                    {
                        // No 
                    }

                }
                else
                {
                    guidanceMsg = "ALL WELL FIGURES AT ATM LEVEL RECONCILE";

                }
            }
            else
            {
                if (Na.BalDiff1.Machine == Na.Balances1.PresenterValue)
                {
                    guidanceMsg = "WARNING: THERE ARE DIFFERENCES but your presented errors is of the same value =" + Na.Balances1.PresenterValue;
                    label3.Show();
                    Color Black = Color.Black;
                    label3.ForeColor = Black;
                    label3.Text = "Difference Same as Errors Value";
                }

                if (Na.BalDiff1.Machine != Na.Balances1.PresenterValue & Na.Balances1.PresenterValue > 0)
                {
                    guidanceMsg = "WARNING: THERE ARE DIFFERENCES. Your presented errors which are "
                        + Na.Balances1.PresenterValue + " are not of the same value";
                    label3.Show();
                    Color Red = Color.Red;
                    label3.ForeColor = Red;
                    label3.Text = "Difference Not Same as Errors Value";
                }

                if (Na.BalDiff1.Machine != 0 & Na.Balances1.PresenterValue == 0)
                {
                    guidanceMsg = "WARNING: THERE ARE DIFFERENCES.  ";
                    //  + Na.Balances1.PresenterValue + " are not of the same value";
                    label3.Show();
                    Color Red = Color.Red;
                    label3.ForeColor = Red;
                    label3.Text = "Difference Exist.";
                }

                if (Na.ErrJournalThisCycle > 0)
                {
                    buttonShowErrors.Show();
                }
                else buttonShowErrors.Hide();

            }

            //// STEPLEVEL

           if (AudiType == true)
            {
                // Do nothing 
            }
           else
            {
                // It is BDC Type
                // CREATE GL TRANSACTIONS 
                CreateActions_Occurances();
            }
           
            // Update STEP

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep2_Updated = true;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            guidanceMsg = "Updating completed. - Move to next step.";
            ChangeBoardMessage(this, e);

        }
        decimal WSwitchTotal;
        public void ShowBalances()
        {
            // FILL ALL FIELDS FOR ALL CURRENCIES 

            //
            // DO NOT CLEAR PREVIOUS ACTIONS HERE 
            //
            

            if (Na.BalSets >= 1)
            {
                label29.Text = Na.Balances1.CurrNm;
                textBoxCounted.Text = Na.Balances1.CountedBal.ToString("#,##0.00");
                textBoxPerAtm.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
                textBoxDiffATM.Text = Na.BalDiff1.Machine.ToString("#,##0.00");
                // Greek Case - Data From Switch

               
                textBoxStatus.Text = "";
                textBoxStatus.Text = "No important notes Available" + Environment.NewLine
                                     + ""
                                     ;

                Mgt.ReadTrans_Totals_FromSpecificTableBetween_Dates(WAtmNo, DateTmSesStart, DateTmSesEnd, 2);
                if (Mgt.TotalWithdrawls == 0)
                {
                    // This is a new ATM
                    WSwitchTotal = Na.Balances1.MachineBal;
                    textBoxSwitch.Text = WSwitchTotal.ToString("#,##0.00");
                }
                else
                {
                    WSwitchTotal = Na.Balances1.OpenBal - Mgt.TotalWithdrawls;
                    textBoxSwitch.Text = WSwitchTotal.ToString("#,##0.00");
                }

                //decimal Dispensed = (Na.Balances1.OpenBal - Na.Balances1.MachineBal);

                //decimal CountPerSwitch = Na.Balances1.OpenBal - WSwitchTotal;

                //textBoxSwitch.Text = CountPerSwitch.ToString("#,##0.00");
                decimal TestingSwitchDiff = Na.Balances1.CountedBal - WSwitchTotal;
                textBox51.Text = TestingSwitchDiff.ToString("#,##0.00");


                // USE MACHINE BALANCE
                // AT ATM LEVEL IS THE MACHINE BALANCE THAT COUNTS 
                //if (Na.Balances1.PresenterValue > 0)
                //{
                label36.Show(); label42.Show();
                textBoxExp.Show(); textBoxDiffExp.Show();
                decimal InputExp = 0;
                if (decimal.TryParse(textBoxExp.Text, out InputExp))
                {
                }
                else
                {
                    //MessageBox.Show(textBoxExp.Text, "Please enter a valid number for Expected!");

                    //return;
                }

                decimal TempExpected = (Na.Balances1.MachineBal + Na.Balances1.PresenterValue);
                if (InputExp == TempExpected || InputExp == 0)
                {
                    textBoxExp.Text = TempExpected.ToString("#,##0.00");
                    InputExp = TempExpected;
                }

                textBoxDiffExp.Text = (Na.Balances1.CountedBal - InputExp).ToString("#,##0.00");

                //if (TestingSwitchDiff != 0)
                //{
                //    textBoxStatus.Text = "";
                //    textBoxStatus.Text = "There is a difference of ATMs counters " + Environment.NewLine
                //                         + "And Back End Systems." + Environment.NewLine
                //                         + "Use link to see suspicious Txns";
                //}

                //if (Na.Balances1.CountedBal > 0 & Na.BalDiff1.Machine > 0 & TestingSwitchDiff != 0)
                //{
                //    textBoxStatus.Text = "";
                //    textBoxStatus.Text = "There is a difference of ATMs counters " + Environment.NewLine
                //                         + "And Back End Systems." + Environment.NewLine
                //                         + "Use link to see suspicious Txns." + Environment.NewLine
                //                          + "Also there is Difference of counted" + Environment.NewLine
                //                         + "Vs Atms Counters .... due to presented error"
                //                         ;
                //}


                if (Na.Balances1.CountedBal > 0 & Na.BalDiff1.Machine > 0 & TestingSwitchDiff == 0 & Na.Balances1.PresenterValue > 0)
                {
                    textBoxStatus.Text = "";
                    textBoxStatus.Text = "There is Difference of counted" + Environment.NewLine
                                         + "Vs Atms Counters .... due to presented error"
                                         ;
                }

                if (Na.BalDiff1.Machine != 0 & Na.Balances1.PresenterValue == 0)
                {
                    textBoxStatus.Text = "";
                    textBoxStatus.Text = "There is Difference of counted" + Environment.NewLine
                                         + "Vs Atms Counters "
                                         ;
                }
            }

            if (Na.BalSets >= 2)
            {
                label19.Text = Na.Balances2.CurrNm;
                textBox30.Text = Na.Balances2.CountedBal.ToString("#,##0.00");
                textBox35.Text = Na.Balances2.MachineBal.ToString("#,##0.00");
                textBox36.Text = Na.BalDiff2.Machine.ToString("#,##0.00");
            }
            if (Na.BalSets >= 3)
            {
                label25.Text = Na.Balances3.CurrNm;
                // To be used for the expected 
                // textBox31.Text = Na.Balances3.CountedBal.ToString("#,##0.00");
                // textBox37.Text = Na.Balances3.MachineBal.ToString("#,##0.00");
                textBox38.Text = Na.BalDiff3.Machine.ToString("#,##0.00");
            }

            if (Na.BalSets == 4)
            {
                label28.Text = Na.Balances4.CurrNm;
                // To be used for the expected 
                textBoxExp.Text = Na.Balances4.CountedBal.ToString("#,##0.00");
                textBoxDiffExp.Text = Na.Balances4.MachineBal.ToString("#,##0.00");
                textBox40.Text = Na.BalDiff4.Machine.ToString("#,##0.00");
            }

            // SHOW ... SHOW ... SHOW 

            panel1.Show();
            label24.Show();
            label19.Show(); textBox30.Show(); textBox35.Show(); textBox36.Show(); textBox52.Show(); textBox53.Show();
            label25.Show();
            // textBox31.Show();
            //textBox37.Show();
            textBox38.Show(); textBox55.Show();
            label28.Show();
            //textBoxDiffExp.Show();
            textBox40.Show(); textBox57.Show();

            if (Na.BalSets == 1)
            {
                label19.Hide(); textBox30.Hide(); textBox35.Hide(); textBox36.Hide(); textBox52.Hide(); textBox53.Hide();
                label25.Hide();
                // textBox31.Hide();
                //textBox37.Hide();
                textBox38.Hide(); textBox55.Hide();
                label28.Hide();
                // textBoxExp.Show();
                // textBoxDiffExp.Hide();
                textBox40.Hide(); textBox57.Hide();
            }
            if (Na.BalSets == 2)
            {
                label25.Hide();
                //extBox31.Hide();
                //textBox37.Hide();
                textBox38.Hide(); textBox55.Hide();
                label28.Hide(); textBoxExp.Hide();
                //textBoxDiffExp.Hide();
                textBox40.Hide();  textBox57.Hide();
            }
            if (Na.BalSets == 3)
            {
                label28.Hide(); textBoxExp.Hide();
                //textBoxDiffExp.Hide();
                textBox40.Hide(); textBox57.Hide();
            }
            if (Na.BalSets == 4)
            {
                // HIDE Nothing
            }

            //if (HybridRepl == false)
            //{
                string WActionId = "87";
                int WWUniqueRecordId = WSesNo;
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WWUniqueRecordId, WActionId);
            //}

            // Handle Any Balance In Action Occurances 
            string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo
                       + "' AND ReplCycle =" + WSesNo
                       + " AND ( (Maker ='" + WSignedId + "' AND Stage<>'03') OR Stage = '03') ";

            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            if (Aoc.Current_DisputeShortage != 0)
            {
                label43.Show();
                textBoxDisp.Show();
                textBoxDisp.Text = Aoc.Current_DisputeShortage.ToString("#,##0.00");
            }
            else
            {
                label43.Hide();
                textBoxDisp.Hide();
            }
        }
        // Create Action Occurances
        DataTable TEMPTableFromAction;
        string WUniqueRecordIdOrigin = "Replenishment";
        public void CreateActions_Occurances()
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 
            bool HybridRepl = false; 

            Ac.ReadAtm(WAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true; 
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }
            //
            if (WOperator == "AUDBEGCA")
            HybridRepl = false;

            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId; 
            string WCcy;
            decimal DoubleEntryAmt;
            string WMaker_ReasonOfAction; 

            DoubleEntryAmt = Na.Balances1.CountedBal;

           
            WUniqueRecordId = WSesNo; // SesNo 
            WCcy = "EGP";

            if (HybridRepl == false & DoubleEntryAmt != 0)
            {
                // FIRST DOUBLE ENTRY 
                WActionId = "25"; // 25_DEBIT_ CIT Account/CR_AtmCash (UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment"

                WMaker_ReasonOfAction = "UnLoad From ATM";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt,
                                                      WAtmNo, WSesNo, WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            else
            {
                if (DoubleEntryAmt == 0)
                {
                    WUniqueRecordIdOrigin = "Replenishment";
                    WUniqueRecordId = WSesNo;
                    WActionId = "25";
                    Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
                }
            }

           
            decimal DiffGL = 0;
            if (decimal.TryParse(textBoxDiffATM.Text, out DiffGL))
            {
            }
            else
            {
                //MessageBox.Show(textBoxDiffExp.Text, "Please enter a valid number for Expected!");

                //return;
            }
            //
            // CLEAR PREVIOUS ACTIONS FOR THIS REPLENISHMENT
            //
            if (HybridRepl == false)
            {
                WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo; 
                WActionId = "29";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            if (HybridRepl == true)
            {
                WActionId = "39";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //
            WActionId = "30";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            // Delete create Dispute Shortage

            if (HybridRepl == false)
            {
                WActionId = "87";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            if (HybridRepl == true)
            {
                WActionId = "77";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }
           
            //
            WActionId = "88";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            if (DiffGL == 0)
            {
                // do nothing
            }

           

            if (DiffGL > 0)
            {
                MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#,##0.00") + Environment.NewLine
                         + "Will be moved to the Branch excess account "
                    );
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
                WActionId = "30"; //30_CREDIT Branch Excess / DR_AtmCash(UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = DiffGL;
                WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (DiffGL < 0)
            {
                MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#,##0.00") + Environment.NewLine
                        + "Will be moved to the Branch shortage account "
                         );
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                if (HybridRepl == false)
                {
                    WActionId = "29"; // 29_DEBIT_CIT Shortages/CR_AtmCash(UNLOAD)
                }
                if (HybridRepl == true)
                {
                    WActionId = "39"; // 29_DEBIT_Branch Shortages/CR_AtmCash(UNLOAD)
                }

                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -DiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad From ATM-Shortage";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo,
                                                      WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }

            // Handle Any Balance In Action Occurances 
            //string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo
            //           + "' AND ReplCycle =" + WSesNo
            //           + " AND ( (Maker ='" + WSignedId + "' AND Stage<>'03') OR Stage = '03') ";

            //Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo
                       + "' AND ( (Maker ='" + WSignedId + "' AND Stage<>'03') OR Stage = '03') ";
            //DateTmSesStart = Ta.SesDtTimeStart;
            //DateTmSesEnd = Ta.SesDtTimeEnd;
            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals_Based_ON_dates(WSelectionCriteria, DateTmSesStart, DateTmSesEnd); 

            if (Aoc.Current_DisputeShortage != 0)
            {
                MessageBox.Show("Also note that Dispute Shortage will be handle here." + Environment.NewLine
                         + "The Dispute Shortage is :" + Aoc.Current_DisputeShortage.ToString("#,##0.00") + Environment.NewLine
                         + "Look at the resulted transactions" );


                decimal CIT_Shortage =  0;
                decimal Shortage =0 ;
                decimal Dispute_Shortage = -(Aoc.Current_DisputeShortage);
                decimal WExcess = Aoc.Excess;

                if (HybridRepl == false)
                {
                    CIT_Shortage = -(Aoc.CIT_Shortage);
                }
                if (HybridRepl == true)
                {
                    Shortage = -(Aoc.CIT_Shortage);
                }


                if (WExcess > 0)
                {
                    if (WExcess >= Dispute_Shortage)
                    {
                        // A
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = Dispute_Shortage;
                        WMaker_ReasonOfAction = "Settle Dispute Shortage";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                    }
                    else
                    {   // A
                        // Use all amount of Excess
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = WExcess; // Use all amount iin Excess
                        WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 1";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                        // The rest you take it from Shortage

                        decimal TempDiff1 = Dispute_Shortage - WExcess;
                        if (TempDiff1 > 0)
                        {
                            // Diff1 goes to Shortage
                            // B
                            if (HybridRepl == false)
                            {
                                WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            if (HybridRepl == true)
                            {
                                WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            // 
                            WUniqueRecordId = WSesNo; // SesNo 
                            WCcy = "EGP";
                            DoubleEntryAmt = TempDiff1;
                            WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 2";
                            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                                  WActionId, WUniqueRecordIdOrigin,
                                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                                  , WMaker_ReasonOfAction, "Replenishment");

                            TEMPTableFromAction = Aoc.TxnsTableFromAction;
                        }
                       
                    }
                }
                
                if ((CIT_Shortage > 0 || (WExcess == 0 & CIT_Shortage == 0) & HybridRepl == false)
                    || (Shortage > 0 || (WExcess == 0 & Shortage == 0) & HybridRepl == true)
                    )
                {
                    // 
                    if (HybridRepl == false)
                    {
                        WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                    }
                    if (HybridRepl == true)
                    {
                        WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT Branch Shortage
                    }
                    // 
                    WUniqueRecordId = WSesNo; // SesNo 
                    WCcy = "EGP";
                    if (Dispute_Shortage <0)
                    {
                        Dispute_Shortage = -Dispute_Shortage;
                    }
                    DoubleEntryAmt = Dispute_Shortage;
                    WMaker_ReasonOfAction = "Settle Dispute Shortage through Shortage";
                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WActionId, WUniqueRecordIdOrigin,
                                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                          , WMaker_ReasonOfAction, "Replenishment");

                    TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }

                label43.Show();
                textBoxDisp.Show();
                textBoxDisp.Text = "0.00" ;
            }

        }


        // Show Errors 

        private void buttonShowErrors_Click(object sender, EventArgs e)
        {
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                WAtmNo, DateTmSesStart, DateTmSesEnd, 3
                );
            NForm80b2.Show();

            //Form24_ByDates NForm24_ByDates;
            //NForm24_ByDates = new Form24_ByDates(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo,DateTmSesStart, DateTmSesEnd);
            //NForm24_ByDates.ShowDialog();

        }
        // My Counts
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel();
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
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

                Usi.ReplStep2_Updated = false;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                //buttonUpdateInput.Show();
            }
        }
        // Show Discrepancies 
        private void linkLabelShowDiscrepancies_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                WAtmNo, DateTmSesStart, DateTmSesEnd, 2
                );
            NForm80b2.Show();

            //string WCategoryId = "";
            //int WRMCycle = 0;

            //Form271ViewAtmUnmatched NForm271ViewAtmUnmatched;
            //NForm271ViewAtmUnmatched = new Form271ViewAtmUnmatched(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycle, WAtmNo, WSesNo);

            //NForm271ViewAtmUnmatched.ShowDialog();
            //Ac.ReadAtm(WAtmNo);

            //RRDMReconcCategories Rc = new RRDMReconcCategories();

            //Rc.ReadReconcCategorybyGroupId(Ac.AtmsReconcGroup);

            //string RecCategoryId = Rc.CategoryId; 

            //Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            //string Function = "Investigation";
            //int UniqueIdType = 7;
            //DateTime temp = new DateTime(2014, 02, 12);
            //Form80b NForm80b; 
            //NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator,
            //      temp.AddDays(-1), temp.AddDays(1), WAtmNo, RecCategoryId , 0, WAtmNo, 0, UniqueIdType, Function, "", 0);

            //NForm80b.ShowDialog();
        }
        // SM LINES
        private void buttonShowSM_Click(object sender, EventArgs e)
        {
            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            string SM_SelectionCriteria1 = " WHERE atmno ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + WSesNo
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

            SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, WSesNo, 2);

            if (SM.RecordFound == true)
            {
                Form67_BDC NForm67_BDC;

                int Mode = 7; // Given Fuid and Ruid 
                string WTraceRRNumber = "";
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, SM.Fuid, WTraceRRNumber, WAtmNo
                    , SM.sessionstart_ruid, SM.sessionend_ruid, NullPastDate, NullPastDate, Mode);
                NForm67_BDC.Show();
            }
            else
            {
                MessageBox.Show("Not found records");
            }
        }
        // Show Replenishment Cycle Txns
        private void buttonCycleTxns_Click(object sender, EventArgs e)
        {
            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateTmSesStart, DateTmSesEnd, WSesNo, NullPastDate, 2);

            NForm78D_ATMRecords.Show();
        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }
        // If change 
        private void textBoxExp_TextChanged(object sender, EventArgs e)
        {
            decimal InputExp = 0;
            if (decimal.TryParse(textBoxExp.Text, out InputExp))
            {
            }
            else
            {
                //MessageBox.Show(textBoxExp.Text, "Please enter a valid number for Expected!");

                //return;
            }

            decimal TempExpected = (Na.Balances1.MachineBal + Na.Balances1.PresenterValue);
            if (InputExp == TempExpected || InputExp == 0)
            {
                textBoxExp.Text = TempExpected.ToString("#,##0.00");
                InputExp = TempExpected;
            }

            textBoxDiffExp.Text = (Na.Balances1.CountedBal - InputExp).ToString("#,##0.00");

        }
        // Show GL Txns 
        private void buttonGLTxns_Click(object sender, EventArgs e)
        {
            // READ ALL IN THIS CYCLE
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
        // 
        private void buttonShowUnmatched_Click(object sender, EventArgs e)
        {
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                                                WAtmNo, DateTmSesStart, DateTmSesEnd, 2
                );
            NForm80b2.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
           

            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateTmSesStart
                               , DateTmSesEnd, WSesNo, NullPastDate, 1);

            NForm78D_ATMRecords.Show();
        }
// Journal Cassettes 
        private void buttonCass_Click(object sender, EventArgs e)
        {
            Form51_SM_Cassettes NForm51_SM_Cassettes;
            NForm51_SM_Cassettes = new Form51_SM_Cassettes(WOperator, WSignedId, WAtmNo, WSesNo);
            NForm51_SM_Cassettes.Show();
        }
    }
}

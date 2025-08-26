using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51d : UserControl
    {
        Form35 NForm35; // Enlarge chart 

        int Tot_51; // Total Notes to be used for Repl.  Already in Cassettes + to be added 
        int Tot_52; // Total Notes to be used for Repl.
        int Tot_53; // Total Notes to be used for Repl.
        int Tot_54; // Total Notes to be used for Repl.

        int Add_51; // Number of Notes to be ADDED in cassette 1 
        int Add_52; // Number of Notes to be ADDED in cassette 2
        int Add_53; // Number of Notes to be ADDED in cassette 3
        int Add_54; // Number of Notes to be ADDED in cassette 4 

        decimal WTotal;

        decimal FinalCorrectedReplTotal;

        //     bool RecordFound;

        //      bool Within;
        bool OldOper;
        bool IsOverRide;
        bool ShowState; // Show when radio is selected
        bool TotalOverrideState; // Show when override field is filled
        bool ReplaceState; // Applies on individual cassette for Replace 
        bool TotalState; // Applies on individual cassettes for total 
                         //   bool IsPerCassetteOverride; 



        int ReplMethod;
        DateTime InUserDate;
        decimal InReplAmount;

        decimal Current;

        decimal TotalRepl;

        decimal InOverride;

        DateTime WDTm;

        DateTime NextRepDtTm; // NEXT REPLENISHMENT DATE

        int I;

        DateTime WReplDate;

        decimal WCurrentBal;

        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool OnlyForInMoney;

        bool ViewWorkFlow;

        //   int tzero = 0;

        int WFunction;
        // DATATable for Grid 
        public DataTable GridDays = new DataTable();


        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass(); // ATM MAIN CLASS TO UPDATE NEXT REPLENISHMENT DATE

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMReplDatesCalc Rc = new RRDMReplDatesCalc(); // Locate next Replenishment 

        RRDMAtmsDailyTransHistory Ah = new RRDMAtmsDailyTransHistory(); // Find transactions history by date 

        RRDMReplOrdersClass Ra = new RRDMReplOrdersClass();

        RRDMFixedDaysReplAtmClass Fr = new RRDMFixedDaysReplAtmClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();


        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        string WBankId;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DataTable DispDaTable = new DataTable(); // THIS FOR THE GRAPH 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public void UCForm51dPar(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();
            //TEST
            Ac.ReadAtm(WAtmNo);
            WBankId = Ac.BankId;
            dateTimePicker1.Value = new DateTime(2014, 02, 28);

            // ================USER BANK =============================
            //   Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //    WUserOperator = Us.Operator;
            // ========================================================

            WDTm = DateTime.Today;
            //TEST
            WDTm = new DateTime(2014, 02, 28);

            if (WAtmNo == "AB104" || WAtmNo == "ABC502")
            {

                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                WDTm = Ta.SesDtTimeEnd.Date;
                dateTimePicker1.Value = WDTm;

            }

          
            // Read U.ProcessNo
            // If 1 : Normal
            // 8 and 9 Atms in need

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

            label24.Text = label24.Text + " - " + Na.Balances1.CurrNm;
            label18.Text = label18.Text + " - " + Na.Balances1.CurrNm;
            label21.Text = label21.Text + " - " + Na.Balances1.CurrNm;

            if (Usi.ProcessNo == 8 || Usi.ProcessNo == 9) // With 8 and 9 we handle the Atms in NEED 
            {
                label18.Show();
                panel8.Show();
                if (Usi.ProcessNo == 9) buttonUpdate.Hide();
                OnlyForInMoney = true;

                Current = Na.Balances1.MachineBal;// ONLY FOR MAIN CURRENCY 
                textBoxBalances1MachineBal.Text = Current.ToString("#,##0.00");
            }
            else
            {
                label18.Hide();
                panel8.Hide();
            }

            if (Usi.ProcessNo == 8) // ONLY FOR INVESTIGATION of IN MONEY 
            {
                // This is just for money estimation 
                // It didnt come throught count of money process. 
                // Therefore current machine figures will be considered


                radioButtonHowLongBal.Checked = true;

                label6.Hide();
                textBoxGlAmount.Hide();

                button4.Hide();

                buttonUpdate.Hide();

                ReplMethod = 3;
                // 3 Means amount was inputed 

                ShowReplInfo(ReplMethod, NullPastDate, Current); // SHOW RESULTS FOR HOW LONG AMOUNT WILL LAST

                textBoxActionCurrent.Text = textBoxBalances1MachineBal.Text;

                textBoxActionWillLast.Text = textBoxNextReplDate.Text;


                // STEPLEVEL

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                if (Usi.StepLevel < 1)
                {
                    Usi.StepLevel = 1;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                panel10.Hide();
                panel9.Hide();
                panel7.Hide();
                panel8.Hide();

                label18.Hide();
                label28.Hide();
                label21.Hide();
                label9.Hide();

                ShowCharts(WAtmNo, WDTm);

                guidanceMsg = " Make Selection and press Show Button ";

                return;

            }

            if (Usi.ProcessNo == 9) // COMES FROM ATM IN NEED - FOR ACTION 
            {
                // This is investigation and action  
                // It didnt come throught count of money process. 
                // Therefore current machine figures will be considered

                radioButtonHowLongBal.Checked = true;

                label6.Hide();
                textBoxGlAmount.Hide();
                button4.Hide();

                ReplMethod = 3;
                // 3 Means amount was inputed 

                ShowReplInfo(ReplMethod, NullPastDate, Current); // SHOW RESULTS FOR HOW LONG AMOUNT WILL LAST
                                                                 // Based on this you put your choice and decide the action
                textBoxActionCurrent.Text = textBoxBalances1MachineBal.Text;

                textBoxActionWillLast.Text = textBoxNextReplDate.Text;

                // STEPLEVEL

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                if (Usi.StepLevel < 1)
                {
                    Usi.StepLevel = 1;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }
                //  label4.Hide(); 
                //  chart1.Hide();
                panel10.Hide();
                panel9.Hide();
                panel7.Hide();
                panel8.Hide();

                label18.Hide();
                label28.Hide();
                label21.Hide();
                label9.Hide();

                radioButtonReplNextWork.Checked = false;
                radioButtonReplAtDate.Checked = false;
                radioButtonReplAtAmount.Checked = false;
                radioButtonHowLongBal.Checked = false;

                // SHOW Grids              

                ShowCharts(WAtmNo, WDTm);

                guidanceMsg = " Make Selection and press Show Button ";

                return;
            }

            // READ CASSETTES NOTES FROM DATA BASES TO SEE WHETHER ALREADY UPDATED 
            // NEEDED WHEN IN REPLENISHMENT MODE 



            OldOper = false;

            if (Na.ReplMethod != 0 & OnlyForInMoney == false)
            {
                if (Na.ReplMethod == 1) radioButtonReplNextWork.Checked = true;
                if (Na.ReplMethod == 2) radioButtonReplAtDate.Checked = true;
                if (Na.ReplMethod == 3) radioButtonReplAtAmount.Checked = true;
                if (Na.ReplMethod == 4) radioButtonHowLongBal.Checked = true;

                radioButtonHowLongBal.Hide();

                OldOper = true;

                Current = Na.Balances1.CountedBal; // ONLY FOR MAIN CURRENCY 
                textBoxBalances1MachineBal.Text = Current.ToString("#,##0.00");

                ReplMethod = Na.ReplMethod;
                InReplAmount = Na.InReplAmount;
                InUserDate = Na.InUserDate;

                if (radioButtonReplNextWork.Checked)
                {
                    // We expect Next Working Date
                    // We expect Amount == 0
                    // We expect Date not to be chosen 


                    ShowReplInfo(Na.ReplMethod, Na.InUserDate, 0);

                    textBoxInReplAmount.Text = "";
                }
                if (radioButtonReplAtDate.Checked)
                {
                    // We expect a future date
                    // Amount = 0

                    dateTimePicker1.Value = Na.InUserDate;
                    ShowReplInfo(Na.ReplMethod, Na.InUserDate, Na.InReplAmount);

                    textBoxInReplAmount.Text = "";
                }
                if (radioButtonReplAtAmount.Checked)
                {
                    // Amount to be > 0

                    textBoxInReplAmount.Text = Na.InReplAmount.ToString("#,##0.00");

                    ShowReplInfo(Na.ReplMethod, Na.InUserDate, Na.InReplAmount);

                }

                if (radioButtonHowLongBal.Checked)
                {
                    // Amount to be > 0

                    textBoxInReplAmount.Text = Na.InReplAmount.ToString("#,##0.00");

                    ShowReplInfo(Na.ReplMethod, Na.InUserDate, Na.InReplAmount);

                }

                ShowCharts(WAtmNo, WDTm);

                guidanceMsg = " Make Selection and press Show Button ";
            }
            if (Na.ReplMethod == 0) // REPLENISHEMENT STATUS
            {
                Current = Na.Balances1.CountedBal; // ONLY FOR MAIN CURRENCY 
                textBoxBalances1MachineBal.Text = Current.ToString("#,##0.00");
                //   label4.Hide(); 
                // chart1.Hide();
                panel10.Hide();
                panel9.Hide();
                panel7.Hide();
                panel8.Hide();

                label18.Hide();
                label28.Hide();
                label21.Hide();
                label9.Hide();

                // SHOW Grids 

                ShowCharts(WAtmNo, WDTm);

                radioButtonHowLongBal.Hide();

                guidanceMsg = " Make Selection and press Show Button ";
            }
        }

        // With SHOW present data based on Type of Replenishment 
        //

        private void buttonShow_Click(object sender, EventArgs e)
        {

            ShowState = true; // Show State starts
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2); // READ NOTES TO FIND NUMBEROF Currency/bal 

            if (Na.BalSets > 1)
            {
                MessageBox.Show("This functionality is currently available only for the local currency. Move to next Replenishment step.");
                // STEPLEVEL

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                if (Usi.StepLevel < 4)
                {
                    Usi.StepLevel = 4;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                Na.ReplMethod = 1;
                Na.InUserDate = DateTime.Now;
                Na.InReplAmount = 1000;

                Na.ReplAmountTotal = 1200;

                Na.ReplAmountSuggest = 1200;

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                guidanceMsg = "This funtionality is not available. Move to Next Step";
                ChangeBoardMessage(this, e);

                return;
            }


            if (radioButtonReplNextWork.Checked == false & radioButtonReplAtDate.Checked == false
                & radioButtonReplAtAmount.Checked == false & radioButtonHowLongBal.Checked == false)
            {
                MessageBox.Show(" Please choose type of Replenishment");
                return;
            }
            //  label4.Show();
            //   chart1.Show();
            panel10.Show();
            panel9.Show();
            panel7.Show();
            //  panel8.Show();

            label28.Show();
            label21.Show();
            label9.Show();

            if (Usi.ProcessNo == 9)
            {
                label18.Show();
                panel8.Show();
            }


            textBoxCalculateNew.Text = ""; // Initialise action fields 
            textBoxWillLast.Text = "";

            //    Within = true;

            OldOper = false;

            if (radioButtonReplNextWork.Checked)
            {
                // We expect Next Working Date
                // We expect Amount == 0
                // We expect Date not to be chosen 

                ReplMethod = 1;
                InUserDate = new DateTime(1900, 01, 01);

                //    dateTimePicker1.Value = InUserDate; 

                InReplAmount = 0;

                ShowReplInfo(ReplMethod, InUserDate, InReplAmount);

                textBoxInReplAmount.Text = "";
            }
            if (radioButtonReplAtDate.Checked)
            {
                // We expect a future date
                // Amount = 0
                ReplMethod = 2;

                InUserDate = dateTimePicker1.Value.Date;

                InReplAmount = 0;

                ShowReplInfo(ReplMethod, InUserDate, InReplAmount);

                textBoxInReplAmount.Text = "";
            }
            if (radioButtonReplAtAmount.Checked)
            {
                // Amount to be > 0

                if (decimal.TryParse(textBoxInReplAmount.Text, out InReplAmount))
                {
                    // Take the correct action 
                }
                else
                {
                    MessageBox.Show(textBoxInReplAmount.Text, "Please enter a valid number!");
                    return;
                }

                ReplMethod = 3;
                InUserDate = new DateTime(1900, 01, 01);

                ShowReplInfo(ReplMethod, NullPastDate, InReplAmount);

            }

            if (radioButtonHowLongBal.Checked)
            {
                // GET Current AMOUNT 

                if (decimal.TryParse(textBoxBalances1MachineBal.Text, out InReplAmount))
                {
                    // Take the correct action 
                }
                else
                {
                    MessageBox.Show(textBoxBalances1MachineBal.Text, "Please enter a valid number!");
                    return;
                }

                ReplMethod = 4;
                InUserDate = new DateTime(1900, 01, 01);

                ShowReplInfo(ReplMethod, NullPastDate, InReplAmount);

                textBoxInReplAmount.Text = "";

            }

            //       Within = false;

            if (Usi.ProcessNo == 9)
            {
                guidanceMsg = " Review presented figures and take action!  ";

            }

            ChangeBoardMessage(this, e);

            ShowState = false;

        }

        // SHOW INFO ( Called from button SHOW)

        private void ShowReplInfo(int InReplMethod, DateTime InUserDate, decimal InUserAmount)
        {

            // InReplType = 1 Next Working
            // 2=Fixed date of replenishment
            // 3= Amount

            // ASSIGN WVARIABLES

            if (InUserDate != NullPastDate)
            {
                WReplDate = InUserDate;
                WCurrentBal = 0;
            }
            else WReplDate = InUserDate; // Assign Working to NULL

            WCurrentBal = InUserAmount;
            TotalRepl = 0;

            //  WDTm = DateTime.Today; Here we put 28/02/2014 for testing purposes
            //TEST
            WDTm = new DateTime(2014, 02, 28);
            if (WAtmNo == "AB104" || WAtmNo == "ABC502")
            {
                //  dateTimePicker1.Value = new DateTime(2014, 02, 13);

                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                WDTm = Ta.SesDtTimeEnd.Date;

                //    WDTm = new DateTime(2014, 02, 13);
            }

            // START CALCULATING REPLENISHMENT FIGURES 
            //    
            Ac.ReadAtm(WAtmNo); // READ TO GET TYPE OF REPLENISHMENT 

            //   Rc.GiveMeDataTableReplInfo(WUserOperator, WDTm, WOperator, WPrive, WAtmNo, WReplDate, WCurrentBal, Ac.MatchDatesCateg);

            Rc.GiveMeDataTableReplInfo(WOperator, WDTm,
                                       WReplDate, WAtmNo, WCurrentBal, Ac.MatchDatesCateg, InReplMethod);

            if (Rc.NoOfDays == 0 & Usi.ProcessNo != 9)
            {
                MessageBox.Show("Amount Not Enough For Calculation. Money must be added. ");
                return;
            }

            if (Rc.TotWkend == 2 & Rc.TotHol > 0) textBoxReplType.Text = "WKEnd + Hol";
            if (Rc.TotWkend == 2 & Rc.TotHol == 0) textBoxReplType.Text = "WKEnd";
            if (Rc.TotWkend == 0 & Rc.TotHol == 0) textBoxReplType.Text = "Normal";

            if (Rc.TotHol > 0)
            {
                textBoxHolDesc.Show();
                label34.Show();
                textBoxHolDesc.Text = Rc.HolDesc;
            }
            else
            {
                textBoxHolDesc.Hide();
                label34.Hide();
            }

            textBoxNumberOfDays.Text = (Rc.NoOfDays).ToString();

            Ac.ReadAtm(WAtmNo);

            textBoxOverEst.Text = Ac.OverEst.ToString();


            if (Rc.NoOfDays == 1) textBoxInsurLimit.Text = Ac.InsurOne.ToString("#,##0.00");
            if (Rc.NoOfDays == 2) textBoxInsurLimit.Text = Ac.InsurTwo.ToString("#,##0.00");
            if (Rc.NoOfDays == 3) textBoxInsurLimit.Text = Ac.InsurThree.ToString("#,##0.00");
            if (Rc.NoOfDays >= 4) textBoxInsurLimit.Text = Ac.InsurFour.ToString("#,##0.00");

            GridDays = new DataTable();
            GridDays.Clear();

            // DATA TABLE ROWS DEFINITION 
            GridDays.Columns.Add("Day", typeof(string));
            GridDays.Columns.Add("Date", typeof(DateTime));
            GridDays.Columns.Add("Amount", typeof(decimal));
            GridDays.Columns.Add("Type", typeof(string));

            DateTime TempDtTm = WDTm;
            // Fill IN GRIDDAYS BASED ON PUBLIC TABLE dtRDays of RC Class 
            //   DataRow RowGrid = GridDays.NewRow();
            I = 0;

            while (I < (Rc.dtRDays.Rows.Count - 1))
            {
                // IN WHILE LOOP WE LEAVE OUT THE LAST DAY. THIS IS THE REPLENISHEMENT DAY 
                DataRow RowGrid = GridDays.NewRow();

                TempDtTm = (DateTime)Rc.dtRDays.Rows[I]["Date"];
                // WE check to find out if amounts for specific dates dates are fixed 
                Fr.ReadFixedDaysReplAtm(WOperator, WAtmNo, TempDtTm); // Correct if fixed 

                if (Fr.RecordFound == true)
                {
                    Rc.dtRDays.Rows[I]["RecDispensed"] = Fr.Final;
                }

                RowGrid["Day"] = TempDtTm.Date.DayOfWeek.ToString();
                RowGrid["Date"] = TempDtTm.Date.ToString();
                RowGrid["Amount"] = Rc.dtRDays.Rows[I]["RecDispensed"];

                bool WorkingDay = (bool)Rc.dtRDays.Rows[I]["Normal"];
                bool Weekend = (bool)Rc.dtRDays.Rows[I]["Weekend"];
                bool Holiday = (bool)Rc.dtRDays.Rows[I]["Special"];

                if (WorkingDay == true) RowGrid["Type"] = "WorkingDay";
                if (Weekend == true) RowGrid["Type"] = "Weekend";
                if (Holiday == true) RowGrid["Type"] = "Holiday";

                // THIS FOR DEBUGGING 
                string X1 = Rc.dtRDays.Rows[I]["Date"].ToString();
                string X2 = Rc.dtRDays.Rows[I]["RecDispensed"].ToString();

                TotalRepl = TotalRepl + (decimal)Rc.dtRDays.Rows[I]["RecDispensed"];

                GridDays.Rows.Add(RowGrid);

                I++;
            }

            dataGridView1.DataSource = GridDays.DefaultView;


            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Name = "Day";

            dataGridView1.Columns["Day"].Width = 80; // Day
            dataGridView1.Columns[1].Width = 80; // Date
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Width = 80; // Amount 
            dataGridView1.Columns[2].DefaultCellStyle = style;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[3].Width = 80; // Type

            dataGridView1.Columns[1].ReadOnly = true; // Day

            //    string date = dataGridView1.Rows[I].Cells["Date"].Value.ToString();

            //  dataGridView1.Rows(0). 
            // Assign NEXT REPLENISHMENT DATE

            //   TempDtTm = (DateTime)Rc.dtRDays.Rows[I]["Date"];

            TempDtTm = (DateTime)Rc.dtRDays.Rows[I]["Date"];

            textBoxNextReplDay.Text = TempDtTm.Date.DayOfWeek.ToString();
            textBoxNextReplDate.Text = TempDtTm.Date.ToString();
            NextRepDtTm = TempDtTm.Date; // Needed to be saved in data base


            // Assign TOTAL
            textBoxTotalRepl.Text = TotalRepl.ToString("#,##0.00");
            // Add % exluding the ones with amounts 
            if (InReplMethod == 1 || InReplMethod == 2)
            {
                TotalRepl = TotalRepl * (100 + Ac.OverEst) / 100;
            }

            // READ CASSETTES NOTES FROM DATA BASES TO GET NEEDED INFO FOR NEXT METHODS 

            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

            // Total Notes for Cassette1 

            CalculateAddNotes(TotalRepl, 2); // THIS METHOD CALCULATES HOW MANY NOTES TO RECOMMEND 


            textBoxFinalTotal.Text = FinalCorrectedReplTotal.ToString("#,##0.00");

            Na.ReplAmountSuggest = FinalCorrectedReplTotal;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);


            if (OldOper == false || OnlyForInMoney == true) // RESULTED FROM ATMS in NEED ACTION Definition 
            {
                WFunction = 1;
                RecomCassettesNotes(Tot_51, Tot_52, Tot_53, Tot_54, WFunction);
                //          textBox9.Text = InUserAmount.ToString("#,##0.00");
                //        Within = true;
                textBoxOverride.Text = "";
                return;
            }

            // If system comes till here then there is Old Operation 
            // CHECK IF ALREADY SUGGESTED FOR THIS SESSION


            if (Na.Cassettes_1.NewInSuggest != 0 || Na.Cassettes_1.NewInUser >= 0 || Na.Cassettes_2.NewInSuggest != 0 || Na.Cassettes_2.NewInUser >= 0
                || Na.Cassettes_3.NewInSuggest != 0 || Na.Cassettes_3.NewInUser >= 0 || Na.Cassettes_4.NewInSuggest != 0 || Na.Cassettes_4.NewInUser >= 0)
            {
                ShowState = true; // OLD DATA ARE DISPLAYED

                int temp = Convert.ToInt32(Na.Cassettes_1.FaceValue);
                label19.Text = temp.ToString() + " " + Na.Cassettes_1.CurName;
                textBoxNotesOne.Text = (Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount).ToString();

                if (Na.Cassettes_1.NewInUser >= 0) // Overide exist 
                {
                    //  textBox32.Hide();
                    textBoxSugNotes1.Text = Na.Cassettes_1.NewInSuggest.ToString();
                    textBoxReplaceOne.Text = Na.Cassettes_1.NewInUser.ToString();
                    textBoxTotalOne.Text = (Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount
                        + Na.Cassettes_1.NewInUser).ToString();
                    textBoxTotalMoneyOne.Text = ((Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount
                        + Na.Cassettes_1.NewInUser) * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
                }
                else
                {
                    textBoxSugNotes1.Text = Na.Cassettes_1.NewInSuggest.ToString();
                    textBoxReplaceOne.Text = "";
                    textBoxTotalOne.Text = (Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount
                        + Na.Cassettes_1.NewInSuggest).ToString();
                    textBoxTotalMoneyOne.Text = ((Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount
                        + Na.Cassettes_1.NewInSuggest) * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
                }

                temp = Convert.ToInt32(Na.Cassettes_2.FaceValue);
                label1.Text = temp.ToString() + " " + Na.Cassettes_2.CurName;
                textBoxNotesTwo.Text = (Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount).ToString();

                if (Na.Cassettes_2.NewInUser >= 0) // Overide exist 
                {
                    //  textBox39.Hide();
                    textBoxSugNotes2.Text = Na.Cassettes_2.NewInSuggest.ToString();
                    textBoxReplaceTwo.Text = Na.Cassettes_2.NewInUser.ToString();
                    textBoxTotalTwo.Text = (Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount
                        + Na.Cassettes_2.NewInUser).ToString();
                    textBoxTotalMoneyTwo.Text = ((Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount
                        + Na.Cassettes_2.NewInUser) * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
                }
                else
                {
                    textBoxSugNotes2.Text = Na.Cassettes_2.NewInSuggest.ToString();
                    textBoxReplaceTwo.Text = "";
                    textBoxTotalTwo.Text = (Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount
                        + Na.Cassettes_2.NewInSuggest).ToString();
                    textBoxTotalMoneyTwo.Text = ((Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount
                        + Na.Cassettes_2.NewInSuggest) * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
                }


                temp = Convert.ToInt32(Na.Cassettes_3.FaceValue);
                label17.Text = temp.ToString() + " " + Na.Cassettes_3.CurName;
                textBoxNotesThree.Text = (Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount).ToString();

                if (Na.Cassettes_3.NewInUser >= 0) // Overide exist 
                {
                    //  textBox62.Hide();
                    textBoxSugNotes3.Text = Na.Cassettes_3.NewInSuggest.ToString();
                    textBoxReplaceThree.Text = Na.Cassettes_3.NewInUser.ToString();
                    textBoxTotalThree.Text = (Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount + Na.Cassettes_3.NewInUser).ToString();
                    textBoxTotalMoneyThree.Text = ((Na.Cassettes_3.CasCount
                        + Na.Cassettes_3.RejCount + Na.Cassettes_3.NewInUser) * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
                }
                else
                {
                    textBoxSugNotes3.Text = Na.Cassettes_3.NewInSuggest.ToString();
                    textBoxReplaceThree.Text = "";
                    textBoxTotalThree.Text = (Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount + Na.Cassettes_3.NewInSuggest).ToString();
                    textBoxTotalMoneyThree.Text = ((Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount
                        + Na.Cassettes_3.NewInSuggest) * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
                }


                temp = Convert.ToInt32(Na.Cassettes_4.FaceValue);
                label2.Text = temp.ToString() + " " + Na.Cassettes_4.CurName;
                textBoxNotesFour.Text = (Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount).ToString();

                if (Na.Cassettes_4.NewInUser >= 0) // Overide exist 
                {
                    //   textBox6.Hide();
                    textBoxSugNotes4.Text = Na.Cassettes_4.NewInSuggest.ToString();
                    textBoxReplaceFour.Text = Na.Cassettes_4.NewInUser.ToString();
                    textBoxTotalFour.Text = (Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount + Na.Cassettes_4.NewInUser).ToString();
                    textBoxTotalMoneyFour.Text = ((Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount
                        + Na.Cassettes_4.NewInUser) * Na.Cassettes_4.FaceValue).ToString("#,##0.00");
                }
                else
                {
                    textBoxSugNotes4.Text = Na.Cassettes_4.NewInSuggest.ToString();
                    textBoxReplaceFour.Text = "";
                    textBoxTotalFour.Text = (Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount + Na.Cassettes_4.NewInSuggest).ToString();
                    textBoxTotalMoneyFour.Text = ((Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount
                        + Na.Cassettes_4.NewInSuggest) * Na.Cassettes_4.FaceValue).ToString("#,##0.00");
                }


                CalculatePercentages();

                // Check if there is override 

                if (Na.Cassettes_1.NewInUser >= 0 || Na.Cassettes_2.NewInUser >= 0
                || Na.Cassettes_3.NewInUser >= 0 || Na.Cassettes_4.NewInUser >= 0)
                {
                    IsOverRide = true;
                    textBoxOverride.Text = Na.ReplAmountTotal.ToString("#,##0.00"); // You get the amount in 
                }
                else
                {
                    IsOverRide = false;
                    textBoxOverride.Text = "";
                }


                // View only 

                if (ViewWorkFlow == true)
                {
                    buttonShow.Hide();
                    buttonOverride.Hide();
                    buttonUpdate.Hide();
                }
            }
            else
            {

                //    WFunction = 1;
                //    RecomCassettesNotes(Tot_51, Tot_52, Tot_53, Tot_54, WFunction);

            }


            guidanceMsg = " System is Suggesting Replenishment figures based on duration till next and on history ";

        }
        //

        //
        // SHOW recomended cassettes Notes
        //

        // CALCULATE WHAT NOTES TO INSERT 
        private void CalculateAddNotes(decimal InAmount, int InFunction)
        {

            Ac.ReadAtm(WAtmNo);

            // ALL FOUR EQUAL TO DEPOSIT CURRENCY CODE = EUR
            if (Na.Cassettes_1.CurName == Ac.DepCurNm & Na.Cassettes_2.CurName == Ac.DepCurNm &
                    Na.Cassettes_3.CurName == Ac.DepCurNm & Na.Cassettes_4.CurName == Ac.DepCurNm)
            {
                if (InFunction == 2)
                {
                    Tot_51 = Convert.ToInt32(InAmount * Ac.CasCapacity_11 / 100 / Na.Cassettes_1.FaceValue);
                    Tot_52 = Convert.ToInt32(InAmount * Ac.CasCapacity_12 / 100 / Na.Cassettes_2.FaceValue);
                    Tot_53 = Convert.ToInt32(InAmount * Ac.CasCapacity_13 / 100 / Na.Cassettes_3.FaceValue);
                    Tot_54 = Convert.ToInt32(InAmount * Ac.CasCapacity_14 / 100 / Na.Cassettes_4.FaceValue);
                }

                // FINAL CORRECTED TOTAL 

                FinalCorrectedReplTotal = Tot_51 * Na.Cassettes_1.FaceValue + Tot_52 * Na.Cassettes_2.FaceValue
                                     + Tot_53 * Na.Cassettes_3.FaceValue + Tot_54 * Na.Cassettes_4.FaceValue;
            }
            else
            {
                // Only three cassettes 
                if (Na.Cassettes_1.CurName == Na.Cassettes_2.CurName &
                        Na.Cassettes_1.CurName == Na.Cassettes_3.CurName)
                {
                    Tot_51 = Convert.ToInt32(InAmount * Ac.CasCapacity_11 / 100 / Na.Cassettes_1.FaceValue);
                    Tot_52 = Convert.ToInt32(InAmount * Ac.CasCapacity_12 / 100 / Na.Cassettes_2.FaceValue);
                    Tot_53 = Convert.ToInt32(InAmount * Ac.CasCapacity_13 / 100 / Na.Cassettes_3.FaceValue);

                    // FINAL CORRECTED TOTAL 

                    FinalCorrectedReplTotal = Tot_51 * Na.Cassettes_1.FaceValue + Tot_52 * Na.Cassettes_2.FaceValue
                                         + Tot_53 * Na.Cassettes_3.FaceValue;
                }

                else
                {
                    // Only two cassetes 
                    if (Na.Cassettes_1.CurName == Na.Cassettes_2.CurName)
                    {
                        Tot_51 = Convert.ToInt32(InAmount * Ac.CasCapacity_11 / 100 / Na.Cassettes_1.FaceValue);
                        Tot_52 = Convert.ToInt32(InAmount * Ac.CasCapacity_12 / 100 / Na.Cassettes_2.FaceValue);
                        // FINAL CORRECTED TOTAL 

                        FinalCorrectedReplTotal = Tot_51 * Na.Cassettes_1.FaceValue + Tot_52 * Na.Cassettes_2.FaceValue;
                    }
                }
            }


            //   MessageBox.Show("Total In= " + InAmount.ToString() + "Total Calculated" + FinalCorrectedReplTotal.ToString()); 

        }
        private void RecomCassettesNotes(int InTot_51, int InTot_52, int InTot_53, int InTot_54, int InWFunction)
        {
            guidanceMsg = " REVIEW SUGGESTIONS AND USE OVERWRITE IF NEEDED ";

            // Here you fill up the table of cassetes -- all fields for the four cassettes

            if (InTot_51 > 0)
            {
                int temp = Convert.ToInt32(Na.Cassettes_1.FaceValue);
                label19.Text = temp.ToString() + " " + Na.Cassettes_1.CurName;

                if (OnlyForInMoney == true) // Based not on count notes 
                {
                    textBoxNotesOne.Text = (Na.Cassettes_1.RemNotes + Na.Cassettes_1.RejNotes).ToString();
                    InTot_51 = InTot_51 - (Na.Cassettes_1.RemNotes + Na.Cassettes_1.RejNotes);
                }
                else
                {
                    textBoxNotesOne.Text = (Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount).ToString();
                    InTot_51 = InTot_51 - (Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount); // To find how much to put in cassette one 
                }

                Add_51 = InTot_51;
                //     Add_51 = Na.Cassettes_1.DispNotes; // In future this will be calculate based on Statistical analysis
                if (InWFunction == 1) // No Override 
                {
                    textBoxSugNotes1.Text = Add_51.ToString();
                    Na.Cassettes_1.NewInSuggest = Add_51;
                    textBoxReplaceOne.Text = "";
                }
                if (InWFunction == 2) // OverWrite 
                {
                    textBoxReplaceOne.Text = Add_51.ToString();
                }
                if (OnlyForInMoney == true)
                {
                    textBoxTotalOne.Text = (Na.Cassettes_1.RemNotes + Na.Cassettes_1.RejNotes + Add_51).ToString();
                    textBoxTotalMoneyOne.Text = ((Na.Cassettes_1.RemNotes + Na.Cassettes_1.RejNotes + Add_51)
                        * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
                }
                else
                {
                    textBoxTotalOne.Text = (Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount + Add_51).ToString();
                    textBoxTotalMoneyOne.Text = ((Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount + Add_51)
                        * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
                }

            }
            else
            {
                if (((Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount + Add_51)
                        * Na.Cassettes_1.FaceValue) > 0)
                {
                    textBoxTotalMoneyOne.Text = ((Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount + Add_51)
                            * Na.Cassettes_1.FaceValue).ToString("#,##0.00"); // 
                }
            }

            if (InTot_52 > 0)
            {
                int temp = Convert.ToInt32(Na.Cassettes_2.FaceValue);
                label1.Text = temp.ToString() + " " + Na.Cassettes_2.CurName;

                if (OnlyForInMoney == true)
                {
                    textBoxNotesTwo.Text = (Na.Cassettes_2.RemNotes + Na.Cassettes_2.RejNotes).ToString();
                    InTot_52 = InTot_52 - (Na.Cassettes_2.RemNotes + Na.Cassettes_2.RejNotes);
                }
                else
                {
                    textBoxNotesTwo.Text = (Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount).ToString();
                    InTot_52 = InTot_52 - (Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount);
                }


                Add_52 = InTot_52;

                //     Add_52 = Na.Cassettes_2.DispNotes;

                if (InWFunction == 1)
                {

                    textBoxSugNotes2.Text = Add_52.ToString();
                    Na.Cassettes_2.NewInSuggest = Add_52;
                    textBoxReplaceTwo.Text = "";
                }
                if (InWFunction == 2) // OverWrite 
                {
                    textBoxReplaceTwo.Text = Add_52.ToString();
                }

                if (OnlyForInMoney == true)
                {
                    textBoxTotalTwo.Text = (Na.Cassettes_2.RemNotes + Na.Cassettes_2.RejNotes + Add_52).ToString();
                    textBoxTotalMoneyTwo.Text = ((Na.Cassettes_2.RemNotes + Na.Cassettes_2.RejNotes + Add_52)
                        * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
                }
                else
                {
                    textBoxTotalTwo.Text = (Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount + Add_52).ToString();
                    textBoxTotalMoneyTwo.Text = ((Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount + Add_52)
                        * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
                }


            }
            else
            {
                if (((Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount + Add_52)
                        * Na.Cassettes_2.FaceValue) > 0)
                {

                    {
                        textBoxTotalMoneyTwo.Text = ((Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount + Add_52)
                               * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
                    }
                }
            }

            // if (Na.ActiveCassettesNo >= 3)
            if (InTot_53 > 0)
            {
                int temp = Convert.ToInt32(Na.Cassettes_3.FaceValue);
                label17.Text = temp.ToString() + " " + Na.Cassettes_3.CurName;

                if (OnlyForInMoney == true)
                {
                    textBoxNotesThree.Text = (Na.Cassettes_3.RemNotes + Na.Cassettes_3.RejNotes).ToString();
                    InTot_53 = InTot_53 - (Na.Cassettes_3.RemNotes + Na.Cassettes_3.RejNotes);
                }
                else
                {
                    textBoxNotesThree.Text = (Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount).ToString();
                    InTot_53 = InTot_53 - (Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount);
                }

                Add_53 = InTot_53;

                //       Add_53 = Na.Cassettes_3.DispNotes;
                if (InWFunction == 1)
                {
                    textBoxSugNotes3.Text = Add_53.ToString();
                    Na.Cassettes_3.NewInSuggest = Add_53;
                    textBoxReplaceThree.Text = "";
                }
                if (InWFunction == 2) // OverWrite 
                {
                    textBoxReplaceThree.Text = Add_53.ToString();
                }

                if (OnlyForInMoney == true)
                {
                    textBoxTotalThree.Text = (Na.Cassettes_3.RemNotes + Na.Cassettes_3.RejNotes + Add_53).ToString();
                    textBoxTotalMoneyThree.Text = ((Na.Cassettes_3.RemNotes + Na.Cassettes_3.RejNotes + Add_53)
                        * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
                }
                else
                {
                    textBoxTotalThree.Text = (Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount + Add_53).ToString();
                    textBoxTotalMoneyThree.Text = ((Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount + Add_53)
                        * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
                }

            }
            else
            {
                if (((Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount + Add_53)
                        * Na.Cassettes_3.FaceValue) > 0)
                {

                    textBoxTotalMoneyThree.Text = ((Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount + Add_53)
                            * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
                }
            }


            if (InTot_54 > 0)
            {
                int temp = Convert.ToInt32(Na.Cassettes_4.FaceValue);
                label2.Text = temp.ToString() + " " + Na.Cassettes_4.CurName;
                if (OnlyForInMoney == true)
                {
                    textBoxNotesFour.Text = (Na.Cassettes_4.RemNotes + Na.Cassettes_4.RejNotes).ToString();
                    InTot_54 = InTot_54 - (Na.Cassettes_4.RemNotes + Na.Cassettes_4.RejNotes);
                }
                else
                {
                    textBoxNotesFour.Text = (Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount).ToString();
                    InTot_54 = InTot_54 - (Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount);
                }


                Add_54 = InTot_54;

                //       Add_54 = Na.Cassettes_4.DispNotes;
                if (InWFunction == 1)
                {
                    textBoxSugNotes4.Text = Add_54.ToString();
                    Na.Cassettes_4.NewInSuggest = Add_54;
                    textBoxReplaceFour.Text = "";
                }
                if (InWFunction == 2) // OverWrite 
                {
                    textBoxReplaceFour.Text = Add_54.ToString();
                }

                if (OnlyForInMoney == true)
                {
                    textBoxTotalFour.Text = (Na.Cassettes_4.RemNotes + Na.Cassettes_4.RejNotes + Add_54).ToString();
                    textBoxTotalMoneyFour.Text = ((Na.Cassettes_4.RemNotes + Na.Cassettes_4.RejNotes + Add_54)
                        * Na.Cassettes_4.FaceValue).ToString("#,##0.00");
                }
                else
                {
                    textBoxTotalFour.Text = (Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount + Add_54).ToString();
                    textBoxTotalMoneyFour.Text = ((Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount + Add_54)
                        * Na.Cassettes_4.FaceValue).ToString("#,##0.00");
                }


            }
            else
            {
                if (((Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount + Add_54)
                        * Na.Cassettes_4.FaceValue) > 0)
                {

                    textBoxTotalMoneyFour.Text = ((Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount + Add_54)
                            * Na.Cassettes_4.FaceValue).ToString("#,##0.00");
                }
            }


            if (Na.Cassettes_1.CurName == Ac.DepCurNm & Na.Cassettes_2.CurName == Ac.DepCurNm &
                         Na.Cassettes_3.CurName == Ac.DepCurNm & Na.Cassettes_3.CurName == Ac.DepCurNm)
            {
                CalculatePercentages();
            }



            //        ChangeBoardMessage(this, e);

        }


        // Override action 
        private void buttonOverride_Click(object sender, EventArgs e)
        {
            textBoxCalculateNew.Text = "";
            textBoxWillLast.Text = ""; 

            TotalOverrideState = true;

            if (decimal.TryParse(textBoxOverride.Text, out InOverride))
            {
                MessageBox.Show("The allocation of Notes to cassettes will be based on the set per cassette percentages");

                Override(InOverride, 2);
            }
            else
            {
                MessageBox.Show(textBoxOverride.Text, "Please enter a valid amount for Override!");
                return;
            }

            TotalOverrideState = false;

            //   textBoxOverride.Text = InOverride.ToString("#,##0.00");
        }

        // CALCULATE WHAT NOTES TO INSERT 
        private void Override(decimal InAmount, int InOrigin)
        {
            // InOrigin = 2 if it comes from main function and 3 when it comes from individual cassettes override.

            WFunction = InOrigin;

            if (WFunction == 2)
            {
                CalculateAddNotes(InAmount, WFunction);
            }
            if (WFunction == 3)
            {

            }

            RecomCassettesNotes(Tot_51, Tot_52, Tot_53, Tot_54, WFunction);

            //   textBox16.Text = InOverride.ToString("#,##0.00");
            textBoxOverride.Text = InOverride.ToString("#,##0.00"); // It needed due to correction 
            return;
        }
        // Replace for one cassette
        private void textBoxReplOne_TextChanged(object sender, EventArgs e)
        {
            if (ShowState == true || TotalOverrideState == true || TotalState == true)
            {
                return;
            }

            ReplaceState = true;

            int NotesOne = int.Parse(textBoxNotesOne.Text);
            int ReplOne;
            int SugNotes1 = int.Parse(textBoxSugNotes1.Text);
            // Cassette One 
            if (textBoxReplaceOne.Text == "")
            {
                // Nothing was entered 
                textBoxTotalOne.Text = (NotesOne + SugNotes1).ToString();
                textBoxTotalMoneyOne.Text = ((NotesOne + SugNotes1) * Na.Cassettes_1.FaceValue).ToString();

            }
            else
            {
                if (int.TryParse(textBoxReplaceOne.Text, out ReplOne))
                {
                    textBoxTotalOne.Text = (NotesOne + ReplOne).ToString();
                    textBoxTotalMoneyOne.Text = ((NotesOne + ReplOne) * Na.Cassettes_1.FaceValue).ToString();

                }
                else
                {
                    MessageBox.Show(textBoxReplaceOne.Text, "Please enter a valid number!");
                    return;
                }
            }
            // Go to calculate the rest 
            CalculateTotalsForOverrideAndPercentages();

            ReplaceState = false;
        }
        // Replace for two cassette
        private void textBoxReplTwo_TextChanged(object sender, EventArgs e)
        {
            if (ShowState == true || TotalOverrideState == true || TotalState == true)
            {
                return;
            }

            ReplaceState = true;

            int NotesTwo = int.Parse(textBoxNotesTwo.Text);
            int ReplTwo;
            int SugNotes2 = int.Parse(textBoxSugNotes2.Text);
            // Cassette One 
            if (textBoxReplaceTwo.Text == "")
            {
                // Nothing was entered 
                textBoxTotalTwo.Text = (NotesTwo + SugNotes2).ToString();
                textBoxTotalMoneyTwo.Text = ((NotesTwo + SugNotes2) * Na.Cassettes_2.FaceValue).ToString();

            }
            else
            {
                if (int.TryParse(textBoxReplaceTwo.Text, out ReplTwo))
                {
                    textBoxTotalTwo.Text = (NotesTwo + ReplTwo).ToString();
                    textBoxTotalMoneyTwo.Text = ((NotesTwo + ReplTwo) * Na.Cassettes_2.FaceValue).ToString();
                }
                else
                {
                    MessageBox.Show(textBoxReplaceTwo.Text, "Please enter a valid number!");
                    return;
                }
            }
            // Go to calculate the rest 
            CalculateTotalsForOverrideAndPercentages();

            ReplaceState = false;
        }
        // Replace for three cassette
        private void textBoxReplThree_TextChanged(object sender, EventArgs e)
        {
            if (ShowState == true || TotalOverrideState == true || TotalState == true)
            {
                return;
            }

            ReplaceState = true;

            int NotesThree = int.Parse(textBoxNotesThree.Text);
            int ReplThree;
            int SugNotes3 = int.Parse(textBoxSugNotes3.Text);
            // Cassette One 
            if (textBoxReplaceThree.Text == "")
            {
                // Nothing was entered 
                textBoxTotalThree.Text = (NotesThree + SugNotes3).ToString();
                textBoxTotalMoneyThree.Text = ((NotesThree + SugNotes3) * Na.Cassettes_3.FaceValue).ToString();

            }
            else
            {
                if (int.TryParse(textBoxReplaceThree.Text, out ReplThree))
                {
                    textBoxTotalThree.Text = (NotesThree + ReplThree).ToString();
                    textBoxTotalMoneyThree.Text = ((NotesThree + ReplThree) * Na.Cassettes_3.FaceValue).ToString();
                }
                else
                {
                    MessageBox.Show(textBoxReplaceThree.Text, "Please enter a valid number!");
                    return;
                }
            }
            // Go to calculate the rest 
            CalculateTotalsForOverrideAndPercentages();

            ReplaceState = false;
        }
        //  Replace for four cassette
        private void textBoxReplFour_TextChanged(object sender, EventArgs e)
        {
            if (ShowState == true || TotalOverrideState == true || TotalState == true)
            {
                return;
            }

            ReplaceState = true;

            int NotesFour = int.Parse(textBoxNotesFour.Text);
            int ReplFour;
            int SugNotes4 = int.Parse(textBoxSugNotes4.Text);
            // Cassette One 
            if (textBoxReplaceFour.Text == "")
            {
                // Nothing was entered 
                textBoxTotalFour.Text = (NotesFour + SugNotes4).ToString();
                textBoxTotalMoneyFour.Text = ((NotesFour + SugNotes4) * Na.Cassettes_4.FaceValue).ToString();

            }
            else
            {
                if (int.TryParse(textBoxReplaceFour.Text, out ReplFour))
                {
                    textBoxTotalFour.Text = (NotesFour + ReplFour).ToString();
                    textBoxTotalMoneyFour.Text = ((NotesFour + ReplFour) * Na.Cassettes_4.FaceValue).ToString();
                }
                else
                {
                    MessageBox.Show(textBoxReplaceFour.Text, "Please enter a valid number!");
                    return;
                }
            }
            // Go to calculate the rest 
            CalculateTotalsForOverrideAndPercentages();

            ReplaceState = false;
        }
        // 
        // Total Notes are changed by user for cassette 1 
        //
        private void textBoxTotalOne_TextChanged(object sender, EventArgs e)
        {
            if (ShowState == true || TotalOverrideState == true || ReplaceState == true)
            {
                return;
            }

            TotalState = true;

            int NotesOne;
            int TotalOne;

            if (int.TryParse(textBoxNotesOne.Text, out NotesOne))
            {

            }
            else
            {
                NotesOne = 0;
            }

            // Cassette One 
            if (textBoxTotalOne.Text == "")
            {
                MessageBox.Show(textBoxTotalOne.Text, "Null Not Allowed!");
                return;
            }
            else
            {
                if (int.TryParse(textBoxTotalOne.Text, out TotalOne))
                {
                    textBoxReplaceOne.Text = (TotalOne - NotesOne).ToString();
                    textBoxTotalMoneyOne.Text = ((TotalOne) * Na.Cassettes_1.FaceValue).ToString();

                }
                else
                {
                    MessageBox.Show(textBoxReplaceOne.Text, "Please enter a valid number!");
                    return;
                }
            }
            // Go to calculate the rest 
            CalculateTotalsForOverrideAndPercentages();

            TotalState = false;

        }
        // 
        // Total Notes are changed by user for cassette 2 
        //
        private void textBoxTotalTwo_TextChanged(object sender, EventArgs e)
        {
            if (ShowState == true || TotalOverrideState == true || ReplaceState == true)
            {
                return;
            }

            TotalState = true;

            int NotesTwo;
            int TotalTwo;

            if (int.TryParse(textBoxNotesTwo.Text, out NotesTwo))
            {

            }
            else
            {
                NotesTwo = 0;
            }

            // Cassette One 
            if (textBoxTotalTwo.Text == "")
            {
                MessageBox.Show(textBoxTotalTwo.Text, "Null Not Allowed!");
                return;
            }
            else
            {
                if (int.TryParse(textBoxTotalTwo.Text, out TotalTwo))
                {
                    textBoxReplaceTwo.Text = (TotalTwo - NotesTwo).ToString();
                    textBoxTotalMoneyTwo.Text = ((TotalTwo) * Na.Cassettes_2.FaceValue).ToString();

                }
                else
                {
                    MessageBox.Show(textBoxReplaceTwo.Text, "Please enter a valid number!");
                    return;
                }
            }
            // Go to calculate the rest 
            CalculateTotalsForOverrideAndPercentages();

            TotalState = false;

        }
        // 
        // Total Notes are changed by user for cassette 3
        //
        private void textBoxTotalThree_TextChanged(object sender, EventArgs e)
        {
            if (ShowState == true || TotalOverrideState == true || ReplaceState == true)
            {
                return;
            }

            TotalState = true;

            int NotesThree;
            int TotalThree;

            if (int.TryParse(textBoxNotesThree.Text, out NotesThree))
            {

            }
            else
            {
                NotesThree = 0;
            }

            // Cassette One 
            if (textBoxTotalThree.Text == "")
            {
                MessageBox.Show(textBoxTotalThree.Text, "Null Not Allowed!");
                return;
            }
            else
            {
                if (int.TryParse(textBoxTotalThree.Text, out TotalThree))
                {
                    textBoxReplaceThree.Text = (TotalThree - NotesThree).ToString();
                    textBoxTotalMoneyThree.Text = ((TotalThree) * Na.Cassettes_3.FaceValue).ToString();
                }
                else
                {
                    MessageBox.Show(textBoxReplaceThree.Text, "Please enter a valid number!");
                    return;
                }
            }
            // Go to calculate the rest 
            CalculateTotalsForOverrideAndPercentages();

            TotalState = false;

        }
        // 
        // Total Notes are changed by user for cassette 4 
        //
        private void textBoxTotalFour_TextChanged(object sender, EventArgs e)
        {
            if (ShowState == true || TotalOverrideState == true || ReplaceState == true)
            {
                return;
            }

            TotalState = true;

            int NotesFour;
            int TotalFour;

            if (int.TryParse(textBoxNotesFour.Text, out NotesFour))
            {

            }
            else
            {
                NotesFour = 0;
            }

            // Cassette One 
            if (textBoxTotalFour.Text == "")
            {
                MessageBox.Show(textBoxTotalFour.Text, "Null Not Allowed!");
                return;
            }
            else
            {
                if (int.TryParse(textBoxTotalFour.Text, out TotalFour))
                {
                    textBoxReplaceFour.Text = (TotalFour - NotesFour).ToString();
                    textBoxTotalMoneyFour.Text = ((TotalFour) * Na.Cassettes_4.FaceValue).ToString();
                }
                else
                {
                    MessageBox.Show(textBoxReplaceFour.Text, "Please enter a valid number!");
                    return;
                }
            }
            // Go to calculate the rest 
            CalculateTotalsForOverrideAndPercentages();

            TotalState = false;
        }

        // 
        private void CalculateTotalsForOverrideAndPercentages()
        {
            if (ShowState == true)
            {
                return;
            }

            try
            {
                int NotesOne;
                if (int.TryParse(textBoxNotesOne.Text, out NotesOne))
                {
                }
                else
                {
                    NotesOne = 0;
                }

                int NotesTwo;
                if (int.TryParse(textBoxNotesTwo.Text, out NotesTwo))
                {
                }
                else
                {
                    NotesTwo = 0;
                }

                int NotesThree;
                if (int.TryParse(textBoxNotesThree.Text, out NotesThree))
                {
                }
                else
                {
                    NotesThree = 0;
                }

                int NotesFour;
                if (int.TryParse(textBoxNotesFour.Text, out NotesFour))
                {
                }
                else
                {
                    NotesFour = 0;
                }

                int SugNotes1;
                int SugNotes2;
                int SugNotes3;
                int SugNotes4;

                if (textBoxSugNotes1.Text != "") SugNotes1 = int.Parse(textBoxSugNotes1.Text);
                else SugNotes1 = 0;
                if (textBoxSugNotes2.Text != "") SugNotes2 = int.Parse(textBoxSugNotes2.Text);
                else SugNotes2 = 0;
                if (textBoxSugNotes3.Text != "") SugNotes3 = int.Parse(textBoxSugNotes3.Text);
                else SugNotes3 = 0;
                if (textBoxSugNotes4.Text != "") SugNotes4 = int.Parse(textBoxSugNotes4.Text);
                else SugNotes4 = 0;

                int ReplOne;
                int ReplTwo;
                int ReplThree;
                int ReplFour;


                // Cassette One 
                if (textBoxReplaceOne.Text == "")
                {

                    Tot_51 = NotesOne + SugNotes1;
                }
                else
                {
                    if (int.TryParse(textBoxReplaceOne.Text, out ReplOne))
                    {

                        Tot_51 = NotesOne + ReplOne;
                    }
                    else
                    {
                        MessageBox.Show(textBoxReplaceOne.Text, "Please enter a valid number!");
                        return;
                    }
                }

                // Cassette Two
                if (textBoxReplaceTwo.Text == "")
                {
                    // Nothing was entered 

                    Tot_52 = (NotesTwo + SugNotes2);
                }
                else
                {
                    if (int.TryParse(textBoxReplaceTwo.Text, out ReplTwo))
                    {

                        Tot_52 = (NotesTwo + ReplTwo);
                    }
                    else
                    {
                        MessageBox.Show(textBoxReplaceTwo.Text, "Please enter a valid number!");
                        return;
                    }
                }

                // Cassette Three
                if (textBoxReplaceThree.Text == "")
                {
                    // Nothing was entered 

                    Tot_53 = (NotesThree + SugNotes3);
                }
                else
                {
                    if (int.TryParse(textBoxReplaceThree.Text, out ReplThree))
                    {

                        Tot_53 = (NotesThree + ReplThree);
                    }
                    else
                    {
                        MessageBox.Show(textBoxReplaceThree.Text, "Please enter a valid number!");
                        return;
                    }
                }

                // Cassette Four
                if (textBoxReplaceFour.Text == "")
                {
                    // Nothing was entered 

                    Tot_54 = (NotesFour + SugNotes4);
                }
                else
                {
                    if (int.TryParse(textBoxReplaceFour.Text, out ReplFour))
                    {

                        Tot_54 = (NotesFour + ReplFour);
                    }
                    else
                    {
                        MessageBox.Show(textBoxReplaceFour.Text, "Please enter a valid number!");
                        return;
                    }
                }
                CalculatePercentages();

                textBoxOverride.Text = WTotal.ToString("#,##0.00");  // WTotal Comes from calculatePercentages   
            }
            catch (Exception ex)
            {

                MessageBox.Show(textBoxReplaceOne.Text, "Error" + ex.Message);
                return;

            }

        }
        // Calculate Percentages      
        private void CalculatePercentages()
        {

            WTotal = 0;
            int TempPerc;

            WTotal = Tot_51 * Na.Cassettes_1.FaceValue + Tot_52 * Na.Cassettes_2.FaceValue + Tot_53 * Na.Cassettes_3.FaceValue + Tot_54 * Na.Cassettes_4.FaceValue;

            if (OnlyForInMoney == true)
            {
                // Total Current Balance Machine 

                Current = Na.Balances1.MachineBal; // Only FOR Main Currency  
            }
            else
            {
                Current = Na.Balances1.CountedBal; // ONLY FOR MAIN CURRENCY 
            }

            textBoxGlAmount.Text = (WTotal - Current).ToString("#,##0.00");

            if (WTotal > 0)
            {

                TempPerc = Convert.ToInt32(Tot_51 * Na.Cassettes_1.FaceValue / WTotal * 100);
                if (TempPerc > 0)
                {
                    textBoxPerc1.Text = TempPerc.ToString();
                }


                TempPerc = Convert.ToInt32(Tot_52 * Na.Cassettes_2.FaceValue / WTotal * 100);
                if (TempPerc > 0)
                {
                    textBoxPerc2.Text = TempPerc.ToString();
                }

                TempPerc = Convert.ToInt32(Tot_53 * Na.Cassettes_3.FaceValue / WTotal * 100);
                if (TempPerc > 0)
                {
                    textBoxPerc3.Text = TempPerc.ToString();
                }


                TempPerc = Convert.ToInt32(Tot_54 * Na.Cassettes_4.FaceValue / WTotal * 100);
                if (TempPerc > 0)
                {
                    textBoxPerc4.Text = TempPerc.ToString();
                }

            }
        }

        //
        // Update the inputed information
        //

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxOverride.Text, out InOverride))
            {
                if (InOverride > 0) IsOverRide = true;
            }
            else IsOverRide = false;

            int temp1, temp2, temp3;

            if (int.TryParse(textBoxReplaceOne.Text, out temp1)) // check override was inputed for first cassette field 
            {
            }
            else
            {
                temp1 = -1; // No override inputed 
            }

            if (temp1 >= 0 || temp1 < -1)
            {

                Na.Cassettes_1.NewInUser = temp1;

                temp2 = int.Parse(textBoxNotesOne.Text);
                temp3 = temp1 + temp2;
                textBoxTotalOne.Text = temp3.ToString();
                textBoxTotalMoneyOne.Text = (temp3 * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
            }
            else
            {
                Na.Cassettes_1.NewInUser = -1; // No Override was inputed 

                if ((Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount + Na.Cassettes_1.NewInSuggest) > 0)
                {
                    textBoxTotalOne.Text = (Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount + Na.Cassettes_1.NewInSuggest).ToString();
                    textBoxTotalMoneyOne.Text = ((Na.Cassettes_1.CasCount + Na.Cassettes_1.RejCount
                        + Na.Cassettes_1.NewInSuggest) * Na.Cassettes_1.FaceValue).ToString("#,##0.00");
                }


                //if (textBoxTotalOne.Text == "0") textBoxTotalOne.Text = "";
                //if (textBoxTotalMoneyOne.Text == "0,00") textBoxTotalMoneyOne.Text = "";
            }


            if (int.TryParse(textBoxReplaceTwo.Text, out temp1)) // continue with next cassette 
            {
            }
            else
            {
                temp1 = -1;
            }

            if (temp1 >= 0 || temp1 < -1)
            {
                //   textBox39.Hide();

                Na.Cassettes_2.NewInUser = temp1;

                temp2 = int.Parse(textBoxNotesTwo.Text);
                temp3 = temp1 + temp2;
                textBoxTotalTwo.Text = temp3.ToString();
                textBoxTotalMoneyTwo.Text = (temp3 * Na.Cassettes_2.FaceValue).ToString("#,##0.00");

            }
            else
            {
                Na.Cassettes_2.NewInUser = -1;

                if ((Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount + Na.Cassettes_2.NewInSuggest) > 0)
                {
                    textBoxTotalTwo.Text = (Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount + Na.Cassettes_2.NewInSuggest).ToString();
                    textBoxTotalMoneyTwo.Text = ((Na.Cassettes_2.CasCount + Na.Cassettes_2.RejCount
                        + Na.Cassettes_2.NewInSuggest) * Na.Cassettes_2.FaceValue).ToString("#,##0.00");
                }


                //if (textBoxTotalTwo.Text == "0") textBoxTotalTwo.Text = "";
                //if (textBoxTotalMoneyTwo.Text == "0,00") textBoxTotalMoneyTwo.Text = "";
            }

            if (int.TryParse(textBoxReplaceThree.Text, out temp1))
            {
                //
            }
            else
            {
                temp1 = -1;
            }

            if (temp1 >= 0 || temp1 < -1)
            {
                //  textBox62.Hide();

                Na.Cassettes_3.NewInUser = temp1;

                temp2 = int.Parse(textBoxNotesThree.Text);
                temp3 = temp1 + temp2;
                textBoxTotalThree.Text = temp3.ToString();
                textBoxTotalMoneyThree.Text = (temp3 * Na.Cassettes_3.FaceValue).ToString("#,##0.00");

            }
            else
            {
                Na.Cassettes_3.NewInUser = -1;

                if ((Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount + Na.Cassettes_3.NewInSuggest) > 0)
                {
                    textBoxTotalThree.Text = (Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount + Na.Cassettes_3.NewInSuggest).ToString();
                    textBoxTotalMoneyThree.Text = ((Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount
                        + Na.Cassettes_3.NewInSuggest) * Na.Cassettes_3.FaceValue).ToString("#,##0.00");
                }

                //if (textBoxTotalThree.Text == "0") textBoxTotalThree.Text = "";
                //if (textBoxTotalMoneyThree.Text == "0,00") textBoxTotalMoneyThree.Text = "";
            }

            if (int.TryParse(textBoxReplaceFour.Text, out temp1))
            {
            }
            else
            {
                temp1 = -1;
            }

            if (temp1 >= 0 || temp1 < -1)
            {
                //   textBox6.Hide();

                Na.Cassettes_4.NewInUser = temp1;

                temp2 = int.Parse(textBoxNotesFour.Text);
                temp3 = temp1 + temp2;
                textBoxTotalFour.Text = temp3.ToString();
                textBoxTotalMoneyFour.Text = (temp3 * Na.Cassettes_4.FaceValue).ToString("#,##0.00");

            }
            else
            {
                Na.Cassettes_4.NewInUser = -1;

                if ((Na.Cassettes_3.CasCount + Na.Cassettes_3.RejCount + Na.Cassettes_3.NewInSuggest) > 0)
                {
                    textBoxTotalFour.Text = (Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount + Na.Cassettes_4.NewInSuggest).ToString();
                    textBoxTotalMoneyFour.Text = ((Na.Cassettes_4.CasCount + Na.Cassettes_4.RejCount
                        + Na.Cassettes_4.NewInSuggest) * Na.Cassettes_4.FaceValue).ToString("#,##0.00");
                }

                //if (textBoxTotalFour.Text == "0") textBoxTotalFour.Text = "";
                //if (textBoxTotalMoneyFour.Text == "0,00") textBoxTotalMoneyFour.Text = "";
            }


            CalculatePercentages(); // Calculate resulted Percentages per cassettes. 

            if (IsOverRide == true)
            {


                if (InOverride != WTotal)
                {
                    MessageBox.Show(" A Slight correction will be done on In Override of amount of : "
                        + (InOverride - WTotal).ToString());
                }

                textBoxOverride.Text = WTotal.ToString("#,##0.00"); // Working Total is the total of the four cassettes

                FinalCorrectedReplTotal = WTotal; // Make final equal to this 

            }
            else
            {
                //   Within = true;
                textBoxOverride.Text = "";

            }

            //    Within = false;

            decimal tempInsur; // check insurance limit 

            if (decimal.TryParse(textBoxInsurLimit.Text, out tempInsur))
            {
                // MessageBox.Show(textBox54.Text, "The input number is correct!"); 

            }
            else
            {
                MessageBox.Show(textBoxInsurLimit.Text, "Please enter a valid number!");
                return;
            }

            if (WTotal > tempInsur) // TOTAL AMOUNT IS more THAN INSURED AMOUNT
            {
                if (MessageBox.Show("MSG946 - Replenishment amount is greater than Insured Amount." + Environment.NewLine
                                 + " Do you want to change(override) amount?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                               == DialogResult.Yes)
                {
                    return;
                }
                else
                {
                    // Continue 

                }

            }

            // UPdate Main with Next Replenishment Date 

            Am.ReadAtmsMainSpecific(WAtmNo);

            //     OldNextDate = Am.NextReplDt; 

            Am.NextReplDt = NextRepDtTm;

            Am.LastUpdated = DateTime.Now;

            Am.UpdateAtmsMain(WAtmNo);


            // Update Notes balances with new in figures 

            Na.ReplMethod = ReplMethod;
            Na.InUserDate = InUserDate;
            Na.InReplAmount = WTotal;

            Na.ReplAmountTotal = WTotal;

            Na.ReplAmountTotal = FinalCorrectedReplTotal;

            Na.InsuranceAmount = decimal.Parse(textBoxInsurLimit.Text);

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            // Update STEP

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep5_Updated = true;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //MessageBox.Show("Data is updated.");

            guidanceMsg = "Updating Done! Move to next step.";
            ChangeBoardMessage(this, e);
        }

        //
        // Show Charts information 
        //
        private void ShowCharts(string InAtmNo, DateTime InWDTm)
        {

            //
            // CREATE THE CHART INFORMATION 
            //
            DateTime WToDate = InWDTm;
            DateTime WFromDate = InWDTm.AddDays(-30);
            // First of seven 
            // End Date is today. 

            string SqlString1 = " Select DtTm As Date, DispensedAmt As Dispensed "
                + " FROM [dbo].[AtmDispAmtsByDay] "
             + " WHERE Operator=@Operator AND AtmNo=@AtmNo AND DtTm BETWEEN @WFromDate AND @WToDate"
             + " ORDER BY DtTm ASC ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString1, conn);

                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", WAtmNo);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@WFromDate", WFromDate);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@WToDate", WToDate);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(DispDaTable);

                    // Close conn
                    conn.Close();

                }

                catch (Exception ex)
                {

                    string exception = ex.ToString();

                }


            // Set chart1  data source  
            chart1.DataSource = DispDaTable.DefaultView;
            chart1.Series[0].Name = "Cash_Dispensed";
            // Set series members names for the X and Y values  
            chart1.Series[0].XValueMember = "Date";
            chart1.Series[0].YValueMembers = "Dispensed";

            // Data bind to the selected data source  
            chart1.DataBind();
            /*
                // Set chart2  data source  
                chart2.DataSource = DispDaTable.DefaultView;
                chart2.Series[0].Name = "Cash_Dispensed";
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "Date";
                chart2.Series[0].YValueMembers = "Dispensed";

                // Data bind to the selected data source  
                chart2.DataBind();
             */

        }
        // 
        //
        // SHOW ACTION 
        //
        private void button3_Click(object sender, EventArgs e)
        {
            button4.Visible = true;
            textBoxCalculateNew.Text = WTotal.ToString("#,##0.00");
            textBoxWillLast.Text = textBoxNextReplDate.Text;

            button4.Show();

            // Insert Action record

        }
        //
        // UPDATE ACTION 
        // 
        private void button4_Click(object sender, EventArgs e)
        {
            // Read ATMs Main to get info 
            // Update fields for action 
            // Insert Action (Create Action)

            Am.ReadAtmsMainSpecific(WAtmNo);
            Ac.ReadAtm(WAtmNo);

            // CHECK IF ACTION ALREADY INSERTED 

            Ra.ReadReplActionsForSpecificDate(WAtmNo, DateTime.Today);

            if (Ra.RecordFound == true)
            {
                if (Ra.ActiveRecord == true)
                {
                    // Move to update Existing Record 

                    Ra.ReadReplActionsForAtm(WAtmNo, Ra.ReplOrderNo);

                    Ra.ReplOrderId = 30;  // Do Replenishment  
                                        //Ra.AuthorisedDate = DateTime.Now; // Effective date 
                    Ra.AtmNo = WAtmNo;
                    Ra.AtmName = Ac.AtmName;

                    Ra.ReplCycleNo = WSesNo;

                    Ra.BankId = WBankId;

                    Ra.RespBranch = Ac.Branch;
                    Ra.BranchName = Ac.BranchName;

                    Ra.OffSite = Ac.OffSite;
                    Ra.LastReplDt = Am.LastReplDt;
                    Ra.TypeOfRepl = Ac.TypeOfRepl;

                    Ra.OverEst = Ac.OverEst;    // % To fill more 

                    // NULL Values 
                    Ra.NextReplDt = Am.NextReplDt; // OLd date set for replenishment 

                    Ra.CurrNm = Na.Balances1.CurrNm;

                    Ra.MinCash = Ac.MinCash;
                    Ra.MaxCash = Ac.MaxCash;

                    Ra.ReplAlertDays = Ac.ReplAlertDays;

                    Ra.ReconcDiff = Am.GL_ReconcDiff;
                    Ra.MoreMaxCash = Am.MoreMaxCash;
                    Ra.LessMinCash = Am.LessMinCash;
                    Ra.NeedType = Am.NeedType;

                    Ra.CurrCassettes = Am.CurrCassettes;
                    Ra.CurrentDeposits = Am.CurrentDeposits;
                    Ra.EstReplDt = Am.EstReplDt;

                    // THESE ARE THE NEW DATA FOR ACTION 

                    Ra.NewEstReplDt = NextRepDtTm;
               
                    if (decimal.TryParse(textBoxInsurLimit.Text, out Ra.InsuredAmount))
                    {
                       
                    }

                    if (decimal.TryParse(textBoxFinalTotal.Text, out Ra.SystemAmount))
                    {

                    }

                  

                    Ra.NewAmount = WTotal;

                    if (Ra.NewAmount > Ra.InsuredAmount) // TOTAL AMOUNT IS more THAN INSURED AMOUNT
                    {
                        if (MessageBox.Show("MSG946 - Replenishment amount is greater than Insured Amount." + Environment.NewLine
                                         + " Do you want to change(override) amount?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                       == DialogResult.Yes)
                        {
                            textBoxCalculateNew.Text = "";
                            textBoxWillLast.Text = "";
                            button4.Hide(); 
                            return;
                        }
                        else
                        {
                            // Continue 

                        }

                    }

                    Ra.Cassette_1 = Tot_51; // Number of notes 
                    Ra.Cassette_2 = Tot_52;
                    Ra.Cassette_3 = Tot_53;
                    Ra.Cassette_4 = Tot_54;

                    Ra.AtmsStatsGroup = Ac.AtmsStatsGroup;
                    Ra.AtmsReplGroup = Ac.AtmsReplGroup;
                    Ra.AtmsReconcGroup = Ac.AtmsReconcGroup;
                    Ra.DateInsert = DateTime.Now;
                    Ra.AuthUser = Am.AuthUser;
                    Ra.OwnerUser = Am.AuthUser;
                    Ra.CitId = Am.CitId;

                    Ra.ActiveRecord = true;

                    // UPDATE EXISTING 
                    Ra.UpdateReplActionsForAtm(WAtmNo, Ra.ReplOrderNo);

                    // Update ATMs Main with the action number 

                    // UPdate Main with Next Replenishment Date 

                    Am.ReadAtmsMainSpecific(WAtmNo);

                    Am.EstReplDt = NextRepDtTm;

                    Am.NextReplDt = NextRepDtTm;

                    Am.LastUpdated = DateTime.Now;

                    Am.UpdateAtmsMain(WAtmNo);


                    // Update Notes balances with new in figures 

                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                    Na.ReplMethod = ReplMethod;
                    Na.InUserDate = InUserDate;
                    Na.InReplAmount = WTotal;

                    Na.ReplAmountTotal = WTotal;

                    Na.ReplAmountTotal = FinalCorrectedReplTotal;

                    Na.InsuranceAmount = decimal.Parse(textBoxInsurLimit.Text);

                    Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                    MessageBox.Show("Action Updated.");

                }
                else
                {
                    // Warn User that action already taken.

                    MessageBox.Show(" For this ATM action was already taken");
                }
            }
            else
            {

                // INSERT NEW ACTION 
                // Repl ACtion FIELDS 

                Ra.ReplOrderId = 30;  // Do Replenishment  
                                      //Ra.AuthorisedDate = DateTime.Now; // Effective date 

                Ra.OrdersCycleNo = 0;

                Ra.AtmNo = Am.AtmNo;
                Ra.AtmName = Am.AtmName;
                Ra.ReplCycleNo = WSesNo;

                Ra.BankId = Am.BankId;

                Ra.RespBranch = Am.RespBranch;
                Ra.BranchName = Am.BranchName;

                Ra.OffSite = Ac.OffSite;
                Ra.LastReplDt = Am.LastReplDt;
                Ra.TypeOfRepl = Ac.TypeOfRepl;

                Ra.OverEst = Ac.OverEst;    // % To fill more 

                // NULL Values 
                Ra.NextReplDt = Am.NextReplDt; // OLd date set for replenishment 

                Ra.CurrNm = Na.Balances1.CurrNm;

                Ra.MinCash = Ac.MinCash;
                Ra.MaxCash = Ac.MaxCash;
                Ra.ReplAlertDays = Ac.ReplAlertDays;

                Ra.ReconcDiff = Am.GL_ReconcDiff;
                Ra.MoreMaxCash = Am.MoreMaxCash;
                Ra.LessMinCash = Am.LessMinCash;
                Ra.NeedType = Am.NeedType;

                Ra.CurrCassettes = Am.CurrCassettes;
                Ra.CurrentDeposits = Am.CurrentDeposits;
                Ra.EstReplDt = Am.EstReplDt;

                // THESE ARE THE NEW DATA FOR ACTION 

                Ra.NewEstReplDt = NextRepDtTm;

                if (decimal.TryParse(textBoxInsurLimit.Text, out Ra.InsuredAmount))
                {

                }

                if (decimal.TryParse(textBoxFinalTotal.Text, out Ra.SystemAmount))
                {

                }

                Ra.NewAmount = WTotal;

                if (Ra.NewAmount > Ra.InsuredAmount) // TOTAL AMOUNT IS more THAN INSURED AMOUNT
                {
                    if (MessageBox.Show("MSG946 - Replenishment amount is greater than Insured Amount." + Environment.NewLine
                                     + " Do you want to change(override) amount?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                   == DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        // Continue 

                    }

                }

                Ra.FaceValue_1 = Ac.FaceValue_11;
                Ra.FaceValue_2 = Ac.FaceValue_12;
                Ra.FaceValue_3 = Ac.FaceValue_13;
                Ra.FaceValue_4 = Ac.FaceValue_14;

                Ra.Cassette_1 = Tot_51; // Number of notes 
                Ra.Cassette_2 = Tot_52;
                Ra.Cassette_3 = Tot_53;
                Ra.Cassette_4 = Tot_54;

                Ra.AtmsStatsGroup = Ac.AtmsStatsGroup;
                Ra.AtmsReplGroup = Ac.AtmsReplGroup;
                Ra.AtmsReconcGroup = Ac.AtmsReconcGroup;
                Ra.DateInsert = DateTime.Now;
                Ra.AuthUser = Am.AuthUser;
                Ra.OwnerUser = Am.AuthUser;
                Ra.CitId = Am.CitId;

                Ra.ActiveRecord = true;

                Ra.Operator = Ac.Operator;

                Ra.ReplOrderNo = Ra.InsertReplOrdersTable(WAtmNo);

                // Update Main 
                Am.ReadAtmsMainSpecific(WAtmNo);

                Am.ActionNo = Ra.ReplOrderNo;

                Am.ActionConfirmed = false; 

                Am.EstReplDt = NextRepDtTm;

                Am.NextReplDt = NextRepDtTm;

                Am.LastUpdated = DateTime.Now;

                Am.UpdateAtmsMain(WAtmNo);


                // Update Notes balances with new in figures 

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2); 

                Na.ReplMethod = ReplMethod;
                Na.InUserDate = InUserDate;
                Na.InReplAmount = WTotal;

                Na.ReplAmountTotal = WTotal;

                Na.ReplAmountTotal = FinalCorrectedReplTotal;

                Na.InsuranceAmount = decimal.Parse(textBoxInsurLimit.Text);

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                MessageBox.Show("Action with Id :" + Ra.ReplOrderNo + " has been created.");

            }

            guidanceMsg = "INPUT DATA HAS BEEN UPDATED";
            ChangeBoardMessage(this, e);



        }

        // ZOOM chart 
        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            int WAction = 0; // Means show large for UCForm51d 
            string Heading = " Dispensed amounts : Last 30 days";
            NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, DispDaTable, Heading, WAction);
            NForm35.ShowDialog();
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
        // Choosen 
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            buttonShow.Show();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            buttonShow.Show();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            buttonShow.Show();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            buttonShow.Show();
        }

        private void textBoxOverride_TextChanged(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }
    }
}

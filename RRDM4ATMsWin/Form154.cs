using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form154 : Form
    {
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();

        RRDMSessions_Form153_Corrections Sc = new RRDMSessions_Form153_Corrections();

        RRDMGasParameters Gp = new RRDMGasParameters();

        //WTotalNotes = (int) SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteNotes"];
        //WTotalMoneyDecimal = (decimal) SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteMoney"];

        int WTotalCassetteNotes;
        decimal WTotalCassetteMoney;

        int WRetractTotalNotes;
        decimal WRetractTotalMoney;


        int WRECYCLEDTotalNotes;
        decimal WRECYCLEDTotalMoney;


        int EnterMode; // 1 is with Update
                       // 2 is from Row Enter 
        string WOperator;
        string WAtmNo;

        string WSignedId;
        int WRMCycleNo;
        int WAction;
        string WInAtmNo;

        public Form154(string InOperator, string InSignedId, int InRMCycleNo, int InAction, string InAtmNo)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WRMCycleNo = InRMCycleNo;

            WAction = InAction; // 1 is normal, 2 is coming from IST replenishement 
            WInAtmNo = InAtmNo;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            //checkBoxDep.Checked = false;

            // comboBoxCcy
            Gp.ParamId = "201"; // Currencies Second Cassettes 
            comboBoxCcy.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxCcy.DisplayMember = "DisplayValue";

            // Face Value  - comboBoxdenominations
            Gp.ParamId = "206";
            comboBoxFaceValue.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBoxFaceValue.DisplayMember = "DisplayValue";

            if (WAction == 2)
            {
                radioButtonUpdateOnlyDates.Show();
                buttonUpdateOnlyDATES.Hide();
            }
            else
            {
                radioButtonUpdateOnlyDates.Hide();
            }

        }
        // Load Form 
        private void Form153_Load(object sender, EventArgs e)
        {
            string WSelection = " WHERE ProcessMode IN (-5,-6)";
            Ta.ReadReplCycles_TheBaddies(WSelection);

            ShowGrid_1();
        }

        // ROW ENTER FOR THE BADDIES
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            if (WInAtmNo == "")
            {
                textBoxATMNo.Text = (string)rowSelected.Cells[0].Value;
            }
            else
            {
                // ATM was inputed
                textBoxATMNo.Text = WInAtmNo;
            }
        }

        // ROW Enter 
        int WSesNo;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            radioButtonNewCycle.Visible = true;
            radioButtonUpdate.Visible = true;
            radioButtonDeleteCycle.Visible = true;
            radioButtonChangeProcess.Visible = true;

            radioButtonNewCycle.Checked = false;
            radioButtonUpdate.Checked = false;
            radioButtonDeleteCycle.Checked = false;
            radioButtonChangeProcess.Checked = false;

            WSesNo = (int)rowSelected.Cells[0].Value;

            SM.Read_SM_Record_Specific_By_ReplCycle(WSesNo);

            labelHeader1.Text = "REPL CYCLE DETAILS";
            buttonValidateDates.Hide();
            buttonValidate.Hide();
            buttonCreateReplenishment.Hide();
            buttonUpdate.Hide();
            buttonDelete.Hide();
            buttonForceItToReplenish.Hide();
            buttonFromInProcessTo0.Hide();
            button_0toInProcess.Hide();

            panelDepBig.Show();
            labelDeposits.Show();
            panelDeposits.Show();
            panelDepBig.Show();


            //checkBoxMakeNewReplCycle.Checked = false;
            //checkBoxUpdate.Checked = false;

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            if (SM.RecordFound == true)
            {

                dateTimePicker1.Value = Ta.SesDtTimeStart;
                dateTimePicker2.Value = Ta.SesDtTimeEnd;
                //cmd.Parameters.AddWithValue("@SM_dateTime_Finish", SM_dateTime_Finish);
                //SM.SM_dateTime_Finish = dateTimePicker2.Value;

                ////cmd.Parameters.AddWithValue("@SM_LAST_CLEARED", SM_LAST_CLEARED);
                //SM.SM_LAST_CLEARED = dateTimePicker1.Value;

                //cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
                //SM.LoadedAtRMCycle = WRMCycleNo;

                textBoxATM_total1.Text = SM.ATM_total1.ToString("0");
                textBoxATM_total2.Text = SM.ATM_total2.ToString("0");
                textBoxATM_total3.Text = SM.ATM_total3.ToString("0");
                textBoxATM_total4.Text = SM.ATM_total4.ToString("0");

                //textBoxGrandTotal1.Text = "0.00";

                textBoxATM_Dispensed1.Text = SM.ATM_Dispensed1.ToString("0");
                textBoxATM_Dispensed2.Text = SM.ATM_Dispensed2.ToString("0");
                textBoxATM_Dispensed3.Text = SM.ATM_Dispensed3.ToString("0");
                textBoxATM_Dispensed4.Text = SM.ATM_Dispensed4.ToString("0");

                // textBoxGrandTotal2.Text = "0.00";

                textBoxATM_cassette1.Text = (SM.ATM_cassette1 + SM.ATM_Rejected1).ToString("0");
                textBoxATM_cassette2.Text = (SM.ATM_cassette2 + SM.ATM_Rejected2).ToString("0");
                textBoxATM_cassette3.Text = (SM.ATM_cassette3 + SM.ATM_Rejected3).ToString("0");
                textBoxATM_cassette4.Text = (SM.ATM_cassette4 + SM.ATM_Rejected4).ToString("0");

                //textBoxGrandTotal3.Text = "0.00";

                textBoxcashaddtype1.Text = SM.cashaddtype1.ToString("0");
                textBoxcashaddtype2.Text = SM.cashaddtype2.ToString("0");
                textBoxcashaddtype3.Text = SM.cashaddtype3.ToString("0");
                textBoxcashaddtype4.Text = SM.cashaddtype4.ToString("0");

                //textBoxGrandTotal4.Text = "0.00";
                EnterMode = 2;
                Calculate_Check_CassettesTotals(EnterMode);

                //dataGridView2.Columns[2].DefaultCellStyle.Format = "#,###";
                //dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                //dataGridView2.Columns[3].Width = 120; // CB_TXNs_AMT 
                //dataGridView2.Columns[3].DefaultCellStyle.Format = "#,##0.00";

                SM.Read_SM_AND_FillTable_Deposits_Baddies(WAtmNo, WSesNo);

                ShowGrid_3();

            }
            else
            {
                textBoxATM_total1.Text = "0";
                textBoxATM_total2.Text = "0";
                textBoxATM_total3.Text = "0";
                textBoxATM_total4.Text = "0";

                textBoxGrandTotal1.Text = "0.00";

                textBoxATM_Dispensed1.Text = "0";
                textBoxATM_Dispensed2.Text = "0";
                textBoxATM_Dispensed3.Text = "0";
                textBoxATM_Dispensed4.Text = "0";

                textBoxGrandTotal2.Text = "0.00";

                textBoxATM_cassette1.Text = "0";
                textBoxATM_cassette2.Text = "0";
                textBoxATM_cassette3.Text = "0";
                textBoxATM_cassette4.Text = "0";

                textBoxGrandTotal3.Text = "0.00";

                textBoxcashaddtype1.Text = "0";
                textBoxcashaddtype2.Text = "0";
                textBoxcashaddtype3.Text = "0";
                textBoxcashaddtype4.Text = "0";

                textBoxGrandTotal4.Text = "0.00";


                dateTimePicker1.Value = Ta.SesDtTimeStart;
                dateTimePicker2.Value = Ta.SesDtTimeEnd;

                RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                if (Na.RecordFound == true)
                {
                    textBoxATM_total1.Text = Na.Cassettes_1.InNotes.ToString("0");
                    textBoxATM_total2.Text = Na.Cassettes_2.InNotes.ToString("0");
                    textBoxATM_total3.Text = Na.Cassettes_3.InNotes.ToString("0");
                    textBoxATM_total4.Text = Na.Cassettes_4.InNotes.ToString("0");

                    EnterMode = 2;
                    Calculate_Check_CassettesTotals(EnterMode);
                }

                SM.Read_SM_AND_FillTable_Deposits_Baddies(WAtmNo, WSesNo);



                ShowGrid_3();
            }
            //
            // Set Deposits 
            //
            Set_IST_INFO();

            GetDepositsAmounts();
        }
        // ATM No 
        int NumberOfCycles;
        private void textBoxATMNo_TextChanged(object sender, EventArgs e)
        {
            WAtmNo = textBoxATMNo.Text;

            Ac.ReadAtm(WAtmNo);

            if (Ac.RecordFound == true)
            {

                if (int.TryParse(textBoxNumberOfCycles.Text, out NumberOfCycles))
                {

                }
                else
                {
                    MessageBox.Show(textBoxNumberOfCycles.Text, "Invalid field number of Cycles!");
                    return;
                }
                MessageBox.Show("It will show the last " + NumberOfCycles + " Repl Cycles. ");

                //string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "'";
                Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

                ShowGrid_2();

                labelAtmNm.Show();
                textBoxAtmNm.Show();
                textBoxAtmNm.Text = Ac.AtmName;
                labelCycles.Show();
                labelHeader1.Show();
                labelHeader2.Show();
                labelHeader3.Show();

                //checkBoxMakeNewReplCycle.Show();
                //checkBoxUpdate.Show();

                panel2.Show();
                panel5.Show();

                radioButtonNewCycle.Visible = true;
                radioButtonUpdate.Visible = true;
                radioButtonDeleteCycle.Visible = true;
                radioButtonChangeProcess.Visible = true;


                textBoxType1.Text = (Ac.FaceValue_11).ToString();
                textBoxType2.Text = (Ac.FaceValue_12).ToString();
                textBoxType3.Text = (Ac.FaceValue_13).ToString();
                textBoxType4.Text = (Ac.FaceValue_14).ToString();

            }
            else
            {
                labelAtmNm.Hide();
                textBoxAtmNm.Hide();
                labelCycles.Hide();
                labelHeader1.Hide();
                labelHeader2.Hide();
                labelHeader3.Hide();
                panel2.Hide();
                panel5.Hide();

                radioButtonNewCycle.Visible = false;
                radioButtonUpdate.Visible = false;
                radioButtonDeleteCycle.Visible = false;
                radioButtonChangeProcess.Visible = false;

                panelDepBig.Hide();
                labelDeposits.Hide();




                //checkBoxMakeNewReplCycle.Hide();
                //checkBoxUpdate.Hide();

                textBoxATM_total1.Text = "0";
                textBoxATM_total2.Text = "0";
                textBoxATM_total3.Text = "0";
                textBoxATM_total4.Text = "0";

                textBoxGrandTotal1.Text = "0.00";

                textBoxATM_Dispensed1.Text = "0";
                textBoxATM_Dispensed2.Text = "0";
                textBoxATM_Dispensed3.Text = "0";
                textBoxATM_Dispensed4.Text = "0";

                textBoxGrandTotal2.Text = "0.00";

                textBoxATM_cassette1.Text = "0";
                textBoxATM_cassette2.Text = "0";
                textBoxATM_cassette3.Text = "0";
                textBoxATM_cassette4.Text = "0";

                textBoxGrandTotal3.Text = "0.00";

                textBoxcashaddtype1.Text = "0";
                textBoxcashaddtype2.Text = "0";
                textBoxcashaddtype3.Text = "0";
                textBoxcashaddtype4.Text = "0";

                textBoxGrandTotal4.Text = "0.00";

            }
        }

        // Show 20 less than this cycle No 
        //private void textBoxCycleNo_TextChanged(object sender, EventArgs e)
        //{

        //    if (int.TryParse(textBoxCycleNo.Text, out WRMCycleNo))
        //    {

        //    }
        //    else
        //    {
        //        MessageBox.Show(textBoxCycleNo.Text, "Please enter a valid Cycle number!");
        //        return;
        //    }
        //    // Show only one Cycle No 
        //    string WSelectionCriteria = "WHERE SesNo =" + WRMCycleNo + "";
        //    Ta.ReadReplCycleONE_OnSesNo(WSelectionCriteria);

        //    if (Ta.RecordFound == true)
        //    {
        //        MessageBox.Show("It will show only one Cycle. ");
        //        //DateTime WDtFrom = new DateTime(1900, 01, 01);
        //        //DateTime WDtTo = DateTime.Today;
        //        //WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "'";
        //        //Ta.ReadReplCycles_Last20(WSelectionCriteria);

        //        ShowGrid_2();

        //        labelAtmNm.Show();
        //        textBoxAtmNm.Show();
        //        textBoxAtmNm.Text = Ac.AtmName;
        //        labelCycles.Show();
        //        labelHeader1.Show();
        //        labelHeader2.Show();
        //        labelHeader3.Show();

        //        checkBoxMakeNewReplCycle.Show();
        //        checkBoxUpdate.Show();

        //        panel2.Show();
        //        panel5.Show();


        //        textBoxType1.Text = (Ac.FaceValue_11).ToString();
        //        textBoxType2.Text = (Ac.FaceValue_12).ToString();
        //        textBoxType3.Text = (Ac.FaceValue_13).ToString();
        //        textBoxType4.Text = (Ac.FaceValue_14).ToString();

        //    }
        //    else
        //    {
        //        //MessageBox.Show("Record Not Found. ");
        //        labelAtmNm.Hide();
        //        textBoxAtmNm.Hide();
        //        labelCycles.Hide();
        //        labelHeader1.Hide();
        //        labelHeader2.Hide();
        //        labelHeader3.Hide();
        //        panel2.Hide();
        //        panel5.Hide();

        //        checkBoxMakeNewReplCycle.Hide();
        //        checkBoxUpdate.Hide();

        //        textBoxATM_total1.Text = "0";
        //        textBoxATM_total2.Text = "0";
        //        textBoxATM_total3.Text = "0";
        //        textBoxATM_total4.Text = "0";

        //        textBoxGrandTotal1.Text = "0.00";

        //        textBoxATM_Dispensed1.Text = "0";
        //        textBoxATM_Dispensed2.Text = "0";
        //        textBoxATM_Dispensed3.Text = "0";
        //        textBoxATM_Dispensed4.Text = "0";

        //        textBoxGrandTotal2.Text = "0.00";

        //        textBoxATM_cassette1.Text = "0";
        //        textBoxATM_cassette2.Text = "0";
        //        textBoxATM_cassette3.Text = "0";
        //        textBoxATM_cassette4.Text = "0";

        //        textBoxGrandTotal3.Text = "0.00";

        //        textBoxcashaddtype1.Text = "0";
        //        textBoxcashaddtype2.Text = "0";
        //        textBoxcashaddtype3.Text = "0";
        //        textBoxcashaddtype4.Text = "0";

        //        textBoxGrandTotal4.Text = "0.00";

        //    }
        //}

        //Show Grid
        private void ShowGrid_1()
        {

            dataGridView1.DataSource = Ta.ATMsReplCyclesTheBaddies.DefaultView;

            if (Ta.ATMsReplCyclesTheBaddies.Rows.Count > 0)
            {
                textBoxTotal.Text = Ta.ATMsReplCyclesTheBaddies.Rows.Count.ToString();
            }
            else
            {
                MessageBox.Show("Nothing to show for Baddies.");
                return;
            }

            dataGridView1.Columns[0].Width = 70; // ATM NO
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 40; // SesNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);

            dataGridView1.Columns[2].Width = 120; // SesDtTimeStart
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 120; // SesDtTimeEnd
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 250; // ProcessMode
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }

        // Show Grid
        private void ShowGrid_2()
        {

            dataGridView2.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;

            dataGridView2.Columns[0].Width = 70; // SesNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);

            dataGridView2.Columns[1].Width = 100; // SesDtTimeStart
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 100; // SesDtTimeEnd
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 50; // 
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 250; // ProcessMode
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }
        // Deposits
        private void ShowGrid_3()
        {
            panelDepBig.Show();
            panelDeposits.Show();
            labelDeposits.Show();
            panelDepBig.Show();

            dataGridView3.DataSource = SM.DataTable_SM_Deposits.DefaultView;

            dataGridView3.Columns[0].Width = 40; // Ccy 
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);

            dataGridView3.Columns[1].Width = 70; // denomination 
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[2].Width = 70; // Number 
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[3].Width = 80; // 
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView2.Columns[4].Width = 250; // ProcessMode
            //dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }
        // Validate dates 
        private void buttonValidateDates_Click(object sender, EventArgs e)
        {
            if (radioButtonNewCycle.Checked == true)
            {
                CheckErrorInDatesNewCycle();
            }
            if (radioButtonUpdate.Checked == true)
            {
                CheckErrorInDatesUpdate();
            }

        }
        // Validate all input 
        private void buttonValidate_Click(object sender, EventArgs e)
        {
            // Repeat checking of dates
            if (radioButtonNewCycle.Checked == true)
            {
                CheckErrorInDatesNewCycle();
            }
            if (radioButtonUpdate.Checked == true)
            {
                CheckErrorInDatesUpdate();
            }

            // Check totals
            Calculate_Check_CassettesTotals(1);

            //CheckDeposits();
        }
        // Validation
        bool ErrorInDates = false;
        private void CheckErrorInDatesNewCycle()
        {
            if (dateTimePicker2.Value <= dateTimePicker1.Value)
            {
                ErrorInDates = true;
                MessageBox.Show(" Second Date smaller than first " + Environment.NewLine
                    + "Please change dates"
                    );
                return;
            }

            //Ta.READ_CycleIn_Progress(WAtmNo); 

            //if (dateTimePicker1.Value.Date > Ta.SesDtTimeStart.Date & radioButtonNewCycle.Checked ==true & Ta.ProcessMode == -1)
            //{
            //    ErrorInDates = true;
            //    MessageBox.Show(" You cannot open a new cycle after the in process if date is not the date start is not the same " + Environment.NewLine
            //        + "Please change action"
            //        );
            //    return;
            //}
            // Check if dates as in selected cycle 
            //Ta.ReadReplCyclesAndValidateDatesforSelected(WAtmNo, dateTimePicker1.Value, dateTimePicker2.Value, WSesNo);
            //if (Ta.ErrorInDates == true & (Ta.ProcessMode == 2 || Ta.ProcessMode == 0))
            //{
            //    ErrorInDates = true;
            //    MessageBox.Show("Error In Dates. There is overlaping  " + Environment.NewLine
            //        + "Change Dates"
            //        );
            //    return;
            //}


            // Check for other Cycles than the selected
            Ta.ReadReplCyclesAndValidateDatesNewCycle(WAtmNo, dateTimePicker1.Value, dateTimePicker2.Value);

            if (Ta.ErrorInDates == true)
            {
                ErrorInDates = true;
                MessageBox.Show(Ta.ErrorInDatesMsg);

                //// this process is the last one 
                //if (Ta.ProcessMode == 0)
                //{
                //    ErrorInDates = true;
                //    MessageBox.Show(Ta.ErrorInDatesMsg);
                //    return;
                //}
                //else
                //{
                //    if (Ta.ProcessMode == -1)
                //    {
                //        ErrorInDates = false;
                //        MessageBox.Show("Despite dates overlapping " + Environment.NewLine
                //            + " this is accepted as it is the last cycle"
                //            );
                //    }
                //    if (Ta.ProcessMode == -5 || Ta.ProcessMode == -6)
                //    {
                //        ErrorInDates = true;
                //        MessageBox.Show("There is dates overlapping " + Environment.NewLine
                //            + "Please change date"
                //            );
                //    }

                //    //return;
                //}

            }
            else
            {
                ErrorInDates = false;

                MessageBox.Show("Input dates are accepted");
            }

        }

        private void CheckErrorInDatesUpdate()
        {
            if (dateTimePicker2.Value <= dateTimePicker1.Value)
            {
                ErrorInDates = true;
                MessageBox.Show(" Second Date smaller than first " + Environment.NewLine
                    + "Please change dates"
                    );
                return;
            }

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            Ta.ReadSessionsStatusTracesToFindNextSesion(WAtmNo, Ta.SesDtTimeEnd);

            // Ta.ReadSessionsStatusTraces(WAtmNo, Ta.NextSes);
            DateTime WorkingDt = dateTimePicker2.Value;
            if (WorkingDt > Ta.SesDtTimeStart & Ta.SesNo != WSesNo)
            {
                ErrorInDates = true;
                MessageBox.Show("Input second date greater " + Environment.NewLine
                    + "than start of the next cycle "
                    );
                return;
            }

            //Ta.READ_CycleIn_Progress(WAtmNo); 

            //if (dateTimePicker1.Value.Date > Ta.SesDtTimeStart.Date & radioButtonNewCycle.Checked == true & Ta.ProcessMode == -1)
            //{
            //    ErrorInDates = true;
            //    MessageBox.Show(" You cannot open a new cycle after the in process if date is not the date start is not the same " + Environment.NewLine
            //        + "Please change action"
            //        );
            //    return;
            //}

            //Ta.ReadReplCyclesAndValidateDatesforSelected(WAtmNo, dateTimePicker1.Value, dateTimePicker2.Value, WSesNo);
            //if (Ta.ErrorInDates == true & (Ta.ProcessMode == 2 || Ta.ProcessMode == 0))
            //{
            //    ErrorInDates = true;
            //    MessageBox.Show("Error In Dates. There is overlaping  " + Environment.NewLine
            //        + "Change Dates"
            //        );
            //    return;
            //}


            Ta.ReadReplCyclesAndValidateDatesUpdate(WAtmNo, dateTimePicker1.Value, dateTimePicker2.Value, WSesNo);

            if (Ta.ErrorInDates == true)
            {
                ErrorInDates = true;
                MessageBox.Show(Ta.ErrorInDatesMsg);
                //**************************
                //if (Ta.ProcessMode == 0)
                //{
                //    ErrorInDates = true;
                //    MessageBox.Show(Ta.ErrorInDatesMsg);
                //    return;
                //}
                //else
                //{
                //    if (Ta.ProcessMode == -1)
                //    {
                //        ErrorInDates = false;
                //        MessageBox.Show("Despite dates overlapping " + Environment.NewLine
                //            + " this is accepted as it is the last cycle"
                //            );
                //    }
                //    if (Ta.ProcessMode == -5 || Ta.ProcessMode == -6)
                //    {
                //        ErrorInDates = true;
                //        MessageBox.Show("There is dates overlapping " + Environment.NewLine
                //            + "Please change date"
                //            );
                //    }

                //    //return;
                //}

            }
            else
            {
                ErrorInDates = false;

                MessageBox.Show("Input dates are accepted");
            }

        }

        // Change Total 1
        int ATM_total1;
        int ATM_total2;
        int ATM_total3;
        int ATM_total4;

        int ATM_Dispensed1;
        int ATM_Dispensed2;
        int ATM_Dispensed3;
        int ATM_Dispensed4;

        int ATM_cassette1; // Remain
        int ATM_cassette2; // Remain
        int ATM_cassette3; // Remain
        int ATM_cassette4; // Remain


        int cashaddtype1;
        int cashaddtype2;
        int cashaddtype3;
        int cashaddtype4;

        int GrandTotal1;
        int GrandTotal2;
        int GrandTotal3;
        int GrandTotal4;


        private void Calculate_Check_CassettesTotals(int InMode)
        {
            if (int.TryParse(textBoxATM_total1.Text, out ATM_total1))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_total1.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxATM_total2.Text, out ATM_total2))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_total2.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxATM_total3.Text, out ATM_total3))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_total3.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxATM_total4.Text, out ATM_total4))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_total4.Text, "Please enter a valid number!");
                return;
            }
            //
            GrandTotal1 = Ac.FaceValue_11 * ATM_total1
                        + Ac.FaceValue_12 * ATM_total2
                        + Ac.FaceValue_13 * ATM_total3
                        + Ac.FaceValue_14 * ATM_total4;
            textBoxGrandTotal1.Text = GrandTotal1.ToString("#,##0.00");

            if (int.TryParse(textBoxATM_Dispensed1.Text, out ATM_Dispensed1))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_Dispensed1.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxATM_Dispensed2.Text, out ATM_Dispensed2))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_Dispensed2.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxATM_Dispensed3.Text, out ATM_Dispensed3))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_Dispensed3.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxATM_Dispensed4.Text, out ATM_Dispensed4))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_Dispensed4.Text, "Please enter a valid number!");
                return;
            }
            // Dispensed 
            GrandTotal2 = Ac.FaceValue_11 * ATM_Dispensed1
                        + Ac.FaceValue_12 * ATM_Dispensed2
                        + Ac.FaceValue_13 * ATM_Dispensed3
                        + Ac.FaceValue_14 * ATM_Dispensed4;
            textBoxGrandTotal2.Text = GrandTotal2.ToString("#,##0.00");

            //

            if (int.TryParse(textBoxATM_cassette1.Text, out ATM_cassette1))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_cassette1.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxATM_cassette2.Text, out ATM_cassette2))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_cassette2.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxATM_cassette3.Text, out ATM_cassette3))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_cassette3.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxATM_cassette4.Text, out ATM_cassette4))
            {
            }
            else
            {
                MessageBox.Show(textBoxATM_cassette4.Text, "Please enter a valid number!");
                return;
            }
            // Remains
            GrandTotal3 = Ac.FaceValue_11 * ATM_cassette1
                        + Ac.FaceValue_12 * ATM_cassette2
                        + Ac.FaceValue_13 * ATM_cassette3
                        + Ac.FaceValue_14 * ATM_cassette4;
            textBoxGrandTotal3.Text = GrandTotal3.ToString("#,##0.00");

            //

            if (int.TryParse(textBoxcashaddtype1.Text, out cashaddtype1))
            {
            }
            else
            {
                MessageBox.Show(textBoxcashaddtype1.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxcashaddtype2.Text, out cashaddtype2))
            {
            }
            else
            {
                MessageBox.Show(textBoxcashaddtype2.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxcashaddtype3.Text, out cashaddtype3))
            {
            }
            else
            {
                MessageBox.Show(textBoxcashaddtype3.Text, "Please enter a valid number!");
                return;
            }
            //
            if (int.TryParse(textBoxcashaddtype4.Text, out cashaddtype4))
            {
            }
            else
            {
                MessageBox.Show(textBoxcashaddtype4.Text, "Please enter a valid number!");
                return;
            }
            // CASH ADDED
            GrandTotal4 = Ac.FaceValue_11 * cashaddtype1
                        + Ac.FaceValue_12 * cashaddtype2
                        + Ac.FaceValue_13 * cashaddtype3
                        + Ac.FaceValue_14 * cashaddtype4;
            textBoxGrandTotal4.Text = GrandTotal4.ToString("#,##0.00");

            if (Ta.ProcessMode != -1 || radioButtonNewCycle.Checked == true)
            {
                if (GrandTotal1 != (GrandTotal2 + GrandTotal3))
                {
                    MessageBox.Show("Totals do not reconcile by " + (GrandTotal1 - (GrandTotal2 + GrandTotal3)));
                    return;
                }

                if (GrandTotal1 == 0 & GrandTotal2 == 0 & GrandTotal3 == 0)
                {
                    //MessageBox.Show("No Input Figures??? ");
                    return;
                }

                if (GrandTotal4 == 0)
                {
                    MessageBox.Show("THIS IS A WARNING MESSAGE " + Environment.NewLine
                        + "No Input Figures??? For Cash to be loaded " + Environment.NewLine
                         + "Maybe this is what you want. " + Environment.NewLine
                        );
                    return;
                }
            }


            // Input numbers are accepted 
            if (InMode == 2)
            {
                // Do not show message
            }
            else
            {
                MessageBox.Show("Input numbers and totals are accepted");
            }

            //if (GrandTotal1 != (GrandTotal2 + GrandTotal3))
            //{
            //    MessageBox.Show("Totals do not reconcile by " + (GrandTotal1 - (GrandTotal2 + GrandTotal3)));
            //    return;
            //}

        }



        //}
        // Make a new Repl Cycle
        private void radioButtonNewCycle_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNewCycle.Checked == true)
            {
                MessageBox.Show("You want to create a new Cycle" + Environment.NewLine
                      + "They shouldnt be dates overlapping" + Environment.NewLine
                      + "Totals must agree too!"
                      );
                //if (Ta.ProcessMode == -1 || Ta.ProcessMode == -5 )
                //{
                //    MessageBox.Show("Fill in the complete dates and figures" + Environment.NewLine
                //        + "Validation will made" + Environment.NewLine
                //        + "Replenishment Cycle will be created by pressing the button Create"
                //        );
                labelHeader1.Text = "NEW CYCLE FOR MISSING JOURNAL";
                buttonValidateDates.Show();
                buttonValidate.Show();
                buttonCreateReplenishment.Show();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                buttonForceItToReplenish.Hide();
                buttonFromInProcessTo0.Hide();
                button_0toInProcess.Hide();

                panelDepBig.Hide();
                panelDeposits.Hide();
                labelDeposits.Hide();

                //dataGridView2.Enabled = false;
                labelHeader3.Show();
                panel4.Show();
                labelDeposits.Show();
                panelDeposits.Show();
                panelDepBig.Show();

                buttonUpdateOnlyDATES.Hide();



            }
            else
            {
                labelHeader1.Text = "REPL CYCLE DETAILS";
                buttonValidateDates.Hide();
                buttonValidate.Hide();
                buttonCreateReplenishment.Hide();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                buttonForceItToReplenish.Hide();
                //dataGridView2.Enabled = true;
            }
        }


        // UPDATE REPLENISHMENT CYCLE
        // Update Cycle
        private void radioButtonUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonUpdate.Checked == true)
            {
                if (Ta.ProcessMode == 0 || Ta.ProcessMode == -6)
                {
                    MessageBox.Show("Fill in the figures - Maybe dates too" + Environment.NewLine
                        + "Validation will made" + Environment.NewLine
                        + "Replenishment Cycle will be Updated by pressing the button Update"
                        );
                    labelHeader1.Text = "UPDATE CYCLE DUE TO WRONG INITIAL FIGURES";
                    buttonValidateDates.Show();
                    buttonValidate.Show();
                    buttonCreateReplenishment.Hide();
                    buttonUpdate.Show();
                    //dataGridView2.Enabled = false;
                    labelHeader3.Show();
                    panel4.Show();
                    labelDeposits.Show();
                    panelDeposits.Show();
                    panelDepBig.Show();

                    buttonUpdateOnlyDATES.Hide();
                }
                else
                {
                    MessageBox.Show("The process mode for the selected is either -1, or -5 or 2 " + Environment.NewLine
                        + "Updating is not allowed for these" + Environment.NewLine
                        + "If -1 or -5 then select Make New Cycle instead" + Environment.NewLine
                        + " "
                        );
                    // radioButtonNewCycle.Checked = false;
                    radioButtonUpdate.Checked = false;
                    return;
                }

            }
            else
            {
                labelHeader1.Text = "REPL CYCLE DETAILS";
                buttonValidateDates.Hide();
                buttonValidate.Hide();
                buttonCreateReplenishment.Hide();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                buttonForceItToReplenish.Hide();
                //dataGridView2.Enabled = true;
            }
        }

        // Create Replenishment
        private void buttonCreateReplenishment_Click(object sender, EventArgs e)
        {



            // Repeat checking of dates
            CheckErrorInDatesNewCycle();


            if (ErrorInDates == true)
            {
                return;
            }

            Calculate_Check_CassettesTotals(1); // Leave it here . dont move it


            if (GrandTotal1 != (GrandTotal2 + GrandTotal3))
            {
                MessageBox.Show("Totals do not reconcile by " + (GrandTotal1 - (GrandTotal2 + GrandTotal3)));
                return;
            }
            // read all fields neaning you 
            SM.Read_ONE_SM_Record_For_ATM_To_See_Recycle(WAtmNo);
            if (SM.RecordFound == true)
            {
                SM.is_recycle = SM.is_recycle; // Get if recycle or not
            }
            else
            {
                // Check if Recycle or NOT
                if (MessageBox.Show("Is this ATM a Recycle one?  " + Environment.NewLine
                                   + "...? " + Environment.NewLine
                                   , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                {
                    // YES Proceed
                    SM.is_recycle = "Y";
                }
                else
                {
                    SM.is_recycle = "N";
                }

                //MessageBox.Show("No previous Repl record found for this ATM"+Environment.NewLine
                //    + "Examine loaded journals if Supervisor Mode area is valid"
                //    );
                //return; 
            }


            SM.AtmNo = WAtmNo;
            SM.FlagValid = "Y";

            //cmd.Parameters.AddWithValue("@AdditionalCash", AdditionalCash);

            SM.AdditionalCash = "N";
            //cmd.Parameters.AddWithValue("@Bank", BANK);

            SM.BANK = WOperator;
            //cmd.Parameters.AddWithValue("@fuid", fuid);

            SM.Fuid = 9999;

            //cmd.Parameters.AddWithValue("@SM_dateTime_Start", SM_dateTime_Start);
            SM.SM_dateTime_Start = dateTimePicker2.Value;

            SM.SM_dateTime_Finish = dateTimePicker2.Value;

            SM.SM_LAST_CLEARED = dateTimePicker1.Value;

            SM.LoadedAtRMCycle = WRMCycleNo;

            SM.ATM_total1 = ATM_total1;
            SM.ATM_total2 = ATM_total2;
            SM.ATM_total3 = ATM_total3;
            SM.ATM_total4 = ATM_total4;

            SM.ATM_Dispensed1 = ATM_Dispensed1;
            SM.ATM_Dispensed2 = ATM_Dispensed2;
            SM.ATM_Dispensed3 = ATM_Dispensed3;
            SM.ATM_Dispensed4 = ATM_Dispensed4;

            SM.ATM_Remaining1 = ATM_cassette1; // Remain is equal to cassette 
            SM.ATM_Remaining2 = ATM_cassette2;
            SM.ATM_Remaining3 = ATM_cassette3;
            SM.ATM_Remaining4 = ATM_cassette4;

            SM.ATM_Rejected1 = 0;
            SM.ATM_Rejected2 = 0;
            SM.ATM_Rejected3 = 0;
            SM.ATM_Rejected4 = 0;

            SM.ATM_cassette1 = ATM_cassette1; // Cassette is equal to Remain 
            SM.ATM_cassette2 = ATM_cassette2;
            SM.ATM_cassette3 = ATM_cassette3;
            SM.ATM_cassette4 = ATM_cassette4;

            SM.cashaddtype1 = cashaddtype1;
            SM.cashaddtype2 = cashaddtype2;
            SM.cashaddtype3 = cashaddtype3;
            SM.cashaddtype4 = cashaddtype4;

            SM.txtline = "MISSING_JOURNAL_RECORD";

            SM.previous_Repl_trace = "99999";
            SM.after_Repl_trace = "99999";

            // INSERT COMMAND 
            int WWWSeqNo = SM.InsertToPANICOS_SM_TableForNewCycle();

            int Tempfuid = WWWSeqNo; // Set fuid to a number like this one instead of 9999 which is not unique 

            SM.Update_SM_Record_From_Form154_FUID(WWWSeqNo, Tempfuid);

            //
            // Supervisor mode
            //
            RRDMRepl_SupervisorMode_Master Smaster = new RRDMRepl_SupervisorMode_Master();
            int Sm_Mode = 2;
            // Ta.ReadSessionsStatusTraces(WAtmNo, )
            RRDMSessionsTracesReadUpdate Tax = new RRDMSessionsTracesReadUpdate();
            Tax.ReadSessionsStatusTracesToFindLastRecord(WAtmNo); // find the last record if it -1
            if (
                (Ta.ProcessMode == -1   // this is the selected 
                || Tax.ProcessMode != -1 // This is the last Repl available. It Should have been -1 but somebody had delete it 
                )
                & Tax.SesDtTimeEnd <= dateTimePicker2.Value
                )
            {

                Sm_Mode = 3;
                // Here it creates the new one and also is forcing another one with -1 
            }
            else
            {
                // Here process mode == 0 
                Sm_Mode = 2; // This mode it doesnt create the in process 

            }
            //
            // EXAMINE LAST RECORD 
            //
            Tax.ReadSessionsStatusTracesToFindLastRecord(WAtmNo); // find the last record if it -1
            if (
                //(Ta.ProcessMode == -1   // this is the selected 
                //|| Tax.ProcessMode != -1 // This is the last Repl available. It Should have been -1 but somebody had delete it 
                //)
                //& Tax.SesDtTimeEnd <= dateTimePicker2.Value
                dateTimePicker2.Value > Tax.SesDtTimeStart // It is a new above 
                )
            {

                Sm_Mode = 3;
                // Here it creates the new one and also is forcing another one with -1 
            }
            else
            {
                // Here process mode == 0 
                Sm_Mode = 2; // This mode it doesnt create the in process because is a new in the middle of cycles 

            }

            Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, 0, WOperator,
                                                             Sm_Mode, "Form153");

            MessageBox.Show("New Cycle has been created");

            radioButtonNewCycle.Checked = false;

            if (int.TryParse(textBoxNumberOfCycles.Text, out NumberOfCycles))
            {

            }
            else
            {
                MessageBox.Show(textBoxNumberOfCycles.Text, "Invalid field number of Cycles!");
                return;
            }
            // MessageBox.Show("It will show the last " + NumberOfCycles + " Repl Cycles. ");

            Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

            // Keep trace of change 
            int WWSesNo = Ta.ReadReplCycles_Last_SesNo_For_This_ATM(WAtmNo);

            Ta.ReadSessionsStatusTraces(WAtmNo, WWSesNo);

            if (Ta.RecordFound == true)
            {

                Sc.AtmNo = WAtmNo;
                Sc.Form153ChangeDesc = "New Repl Cycle For ATM.." + WAtmNo + "..ReplNo=." + Ta.SesNo.ToString();
                Sc.DtTmAtChange = DateTime.Now;
                Sc.ChangeByUser = WSignedId;
                Sc.RMCycleOfChange = WRMCycleNo;
                Sc.ReplCycleNo = Ta.SesNo;

                Sc.SesDtTimeStart_Before = Ta.SesDtTimeStart;
                Sc.SesDtTimeEnd_Before = Ta.SesDtTimeEnd;
                Sc.ProcessMode_Before = Ta.ProcessMode;

                Sc.LoadedAtRMCycle = Ta.LoadedAtRMCycle;
                Sc.ReconcAtRMCycle = Ta.ReconcAtRMCycle;
                Sc.Operator = Ta.Operator;

                SeqNoCor = Sc.InsertSessions_Form153_Corrections(WAtmNo, Ta.SesNo);
            }

            ShowGrid_2();
        }

        // UPDATE REPLENISHMENT CYCLE RECORD
        int SeqNoCor = 0;
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            // Repeat checking of dates
            if (radioButtonNewCycle.Checked == true)
            {
                CheckErrorInDatesNewCycle();
            }
            if (radioButtonUpdate.Checked == true)
            {
                CheckErrorInDatesUpdate();
            }

            if (ErrorInDates == true)
            {
                return;
            }

            Calculate_Check_CassettesTotals(1);


            EnterMode = 2;
            Calculate_Check_CassettesTotals(EnterMode);

            if (GrandTotal1 != (GrandTotal2 + GrandTotal3))
            {
                MessageBox.Show("Totals do not reconcile by " + (GrandTotal1 - (GrandTotal2 + GrandTotal3)));
                return;
            }

            // Keep trace of change 
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            if (Ta.RecordFound == true)
            {

                Sc.AtmNo = WAtmNo;
                Sc.Form153ChangeDesc = " Updating of ATM.." + WAtmNo + "..ReplNo=." + WSesNo.ToString();
                Sc.DtTmAtChange = DateTime.Now;
                Sc.ChangeByUser = WSignedId;
                Sc.RMCycleOfChange = WRMCycleNo;
                Sc.ReplCycleNo = WSesNo;

                Sc.SesDtTimeStart_Before = Ta.SesDtTimeStart;
                Sc.SesDtTimeEnd_Before = Ta.SesDtTimeEnd;
                Sc.ProcessMode_Before = Ta.ProcessMode;

                Sc.LoadedAtRMCycle = Ta.LoadedAtRMCycle;
                Sc.ReconcAtRMCycle = Ta.ReconcAtRMCycle;
                Sc.Operator = Ta.Operator;

                SeqNoCor = Sc.InsertSessions_Form153_Corrections(WAtmNo, WSesNo);
            }

            SM.Read_SM_Record_Specific_By_ReplCycle(WSesNo);

            SM.SM_dateTime_Start = dateTimePicker1.Value;

            SM.SM_dateTime_Finish = dateTimePicker2.Value;

            SM.SM_LAST_CLEARED = dateTimePicker1.Value;

            //SM.LoadedAtRMCycle = WRMCycleNo;

            SM.ATM_total1 = ATM_total1;
            SM.ATM_total2 = ATM_total2;
            SM.ATM_total3 = ATM_total3;
            SM.ATM_total4 = ATM_total4;

            SM.ATM_Dispensed1 = ATM_Dispensed1;
            SM.ATM_Dispensed2 = ATM_Dispensed2;
            SM.ATM_Dispensed3 = ATM_Dispensed3;
            SM.ATM_Dispensed4 = ATM_Dispensed4;

            SM.ATM_Remaining1 = ATM_cassette1; // Remain is equal to cassette 
            SM.ATM_Remaining2 = ATM_cassette2;
            SM.ATM_Remaining3 = ATM_cassette3;
            SM.ATM_Remaining4 = ATM_cassette4;

            SM.ATM_Rejected1 = 0;
            SM.ATM_Rejected2 = 0;
            SM.ATM_Rejected3 = 0;
            SM.ATM_Rejected4 = 0;

            SM.ATM_cassette1 = ATM_cassette1; // Cassette is equal to Remain 
            SM.ATM_cassette2 = ATM_cassette2;
            SM.ATM_cassette3 = ATM_cassette3;
            SM.ATM_cassette4 = ATM_cassette4;

            SM.cashaddtype1 = cashaddtype1;
            SM.cashaddtype2 = cashaddtype2;
            SM.cashaddtype3 = cashaddtype3;
            SM.cashaddtype4 = cashaddtype4;

            //SM.txtline = "UPDATED_SM_RECORD";

            //SM.previous_Repl_trace = "99999";
            //SM.after_Repl_trace = "99999";
            if (SM.Fuid == 9999 || SM.Fuid == 0) // This is insert here because of some errors were we had this general number instead of 9999
            {
                SM.Fuid = SM.SeqNo;
            }

            SM.Update_SM_Record_From_Form153(SM.SeqNo);

            // INSERT COMMAND 
            //  SM.InsertToPANICOS_SM_TableForNewCycle();

            //
            // Supervisor mode
            //
            RRDMRepl_SupervisorMode_Master Smaster = new RRDMRepl_SupervisorMode_Master();
            int Sm_Mode = 4;
            //if (Ta.ProcessMode == -1)
            //{
            //    Sm_Mode = 3;
            //}
            //else
            //{
            // Here process mode == 0 
            //Sm_Mode = 2;
            //}
            Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, 0, WOperator,
                                                             Sm_Mode, "Form153");



            MessageBox.Show("Replenishment Cycle has been corrected");

            // radioButtonNewCycle.Checked = false;
            radioButtonUpdate.Checked = false;

            // Keep the trace

            // Keep trace of change 
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            if (Ta.RecordFound == true)
            {

                Sc.SesDtTimeStart_After = Ta.SesDtTimeStart;
                Sc.SesDtTimeEnd_After = Ta.SesDtTimeEnd;
                Sc.ProcessMode_After = Ta.ProcessMode;

                Sc.UpdateSessions_Form153_Corrections(SeqNoCor);

            }

            if (int.TryParse(textBoxNumberOfCycles.Text, out NumberOfCycles))
            {

            }
            else
            {
                MessageBox.Show(textBoxNumberOfCycles.Text, "Invalid field number of Cycles!");
                return;
            }
            // MessageBox.Show("It will show the last " + NumberOfCycles + " Repl Cycles. ");

            Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

            Set_IST_INFO();

            int WRowIndex = dataGridView2.SelectedRows[0].Index;

            int scrollPosition = dataGridView2.FirstDisplayedScrollingRowIndex;

            ShowGrid_2();

            dataGridView2.Rows[WRowIndex].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(2, WRowIndex));

            dataGridView2.FirstDisplayedScrollingRowIndex = scrollPosition;

        }


        // This dispose - Finish
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Show SM lines 
        private void linkLabelSMLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            DateTime NullPastDate = new DateTime(1900, 01, 01);

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
                NForm67_BDC.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not found records");
            }
        }
        // Refresh the Baddies 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            string WSelection = " WHERE ProcessMode IN (-5,-6)";
            Ta.ReadReplCycles_TheBaddies(WSelection);

            ShowGrid_1();
        }
        // CheckBox Delete Cycle 

        private void radioButtonDeleteCycle_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButtonDeleteCycle.Checked == true)
            {
                if (Ta.ProcessMode == 0 || Ta.ProcessMode == -6 || Ta.ProcessMode == -5 || Ta.ProcessMode == -1)
                {
                    MessageBox.Show("You want to delete the Cycle.." + WSesNo + Environment.NewLine
                        + "For ATM No.." + WAtmNo + Environment.NewLine
                        + "A Button Delete will appear.." + Environment.NewLine
                        + "Press it to delete.." + Environment.NewLine
                        );
                    labelHeader1.Text = "DELETE UNWANTED CYCLE";
                    //buttonValidateDates.Show();
                    //buttonValidate.Show();
                    //buttonCreateReplenishment.Hide();
                    buttonDelete.Show();
                    //dataGridView2.Enabled = false;
                    labelHeader3.Show();
                    panel4.Show();
                    labelDeposits.Show();
                    panelDeposits.Show();
                    panelDepBig.Show();
                    buttonUpdateOnlyDATES.Hide();
                }
                else
                {
                    MessageBox.Show("You Cannot Delete a completed Cycle " + Environment.NewLine
                        + " "
                        );

                    radioButtonDeleteCycle.Checked = false;
                    return;
                }

            }
            else
            {
                labelHeader1.Text = "REPL CYCLE DETAILS";
                buttonValidateDates.Hide();
                buttonValidate.Hide();
                buttonCreateReplenishment.Hide();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                buttonForceItToReplenish.Hide();
                //dataGridView2.Enabled = true;
            }
        }

        // Delete Buttom
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to Delete this Cycle?" + Environment.NewLine
                            + "Cycle is.. " + WSesNo + " ATM is.." + WAtmNo
                            , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                     == DialogResult.Yes)
            {
                //Check if outstanding before deleting
                // Keep trace of change 
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (Ta.RecordFound == true)
                {

                    Sc.AtmNo = WAtmNo;
                    Sc.Form153ChangeDesc = " DELETED CYCLE For ATM.." + WAtmNo + "..ReplNo=." + WSesNo.ToString();
                    Sc.DtTmAtChange = DateTime.Now;
                    Sc.ChangeByUser = WSignedId;
                    Sc.RMCycleOfChange = WRMCycleNo;
                    Sc.ReplCycleNo = WSesNo;

                    Sc.SesDtTimeStart_Before = Ta.SesDtTimeStart;
                    Sc.SesDtTimeEnd_Before = Ta.SesDtTimeEnd;
                    Sc.ProcessMode_Before = Ta.ProcessMode;

                    Sc.LoadedAtRMCycle = Ta.LoadedAtRMCycle;
                    Sc.ReconcAtRMCycle = Ta.ReconcAtRMCycle;
                    Sc.Operator = Ta.Operator;

                    SeqNoCor = Sc.InsertSessions_Form153_Corrections(WAtmNo, WSesNo);
                }
                // YES Proceed
                Ta.Delete_A_Cycle(WAtmNo, WSesNo);

                MessageBox.Show("Cycle Deleted");
                radioButtonDeleteCycle.Checked = false;

                // Keep trace of change 
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (Ta.RecordFound == true)
                {

                    Sc.AtmNo = WAtmNo;
                    Sc.Form153ChangeDesc = " Updating of ATM.." + WAtmNo + "..ReplNo=." + WSesNo.ToString();
                    Sc.DtTmAtChange = DateTime.Now;
                    Sc.ChangeByUser = WSignedId;
                    Sc.RMCycleOfChange = WRMCycleNo;
                    Sc.ReplCycleNo = WSesNo;

                    Sc.SesDtTimeStart_Before = Ta.SesDtTimeStart;
                    Sc.SesDtTimeEnd_Before = Ta.SesDtTimeEnd;
                    Sc.ProcessMode_Before = Ta.ProcessMode;

                    Sc.LoadedAtRMCycle = Ta.LoadedAtRMCycle;
                    Sc.ReconcAtRMCycle = Ta.ReconcAtRMCycle;
                    Sc.Operator = Ta.Operator;

                    SeqNoCor = Sc.InsertSessions_Form153_Corrections(WAtmNo, WSesNo);
                }

                // checkBoxDelete.Checked = false;

                if (int.TryParse(textBoxNumberOfCycles.Text, out NumberOfCycles))
                {

                }
                else
                {
                    MessageBox.Show(textBoxNumberOfCycles.Text, "Invalid field number of Cycles!");
                    return;
                }
                // MessageBox.Show("It will show the last " + NumberOfCycles + " Repl Cycles. ");

                Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);
                ShowGrid_2();

            }
            else
            {
                // Stop 
                radioButtonDeleteCycle.Checked = false;
                return;
            }

        }

        // Force it to replenish 
        private void radioButtonChangeProcess_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonChangeProcess.Checked == true)
            {

                if (Ta.ProcessMode == 0 || Ta.ProcessMode == -6 || Ta.ProcessMode == -5 || Ta.ProcessMode == -1)
                {
                    MessageBox.Show("You want to change process mode this Repl Cycle.." + WSesNo + Environment.NewLine
                        + "For ATM No.." + WAtmNo + Environment.NewLine
                        + "Buttons will appear.." + Environment.NewLine
                        + "Press the appropriate one .." + Environment.NewLine
                        );
                    labelHeader1.Text = "CHANGE PROCESS MODE BY FORCE";
                    //buttonValidateDates.Show();
                    //buttonValidate.Show();
                    //buttonCreateReplenishment.Hide();

                    buttonForceItToReplenish.Show();
                    buttonFromInProcessTo0.Show();
                    button_0toInProcess.Show();
                    //dataGridView2.Enabled = false;
                    labelHeader3.Show();
                    panel4.Show();
                    labelDeposits.Show();
                    panelDeposits.Show();
                    panelDepBig.Show();
                    buttonUpdateOnlyDATES.Hide();
                }
                else
                {
                    // This Cycle is completed 
                    //Check if transactions made 
                    RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

                    string WSelectionCriteria = " WHERE AtmNo ='" + WAtmNo
                                                          + "' AND ReplCycle =" + WSesNo;

                    Aoc.ReadActionsOccurancesToSeeIf_Exists(WSelectionCriteria);

                    if (Aoc.RecordFound == true)
                    {
                        // Transactions exists
                        MessageBox.Show("This cycle is already closed. " + Environment.NewLine
                                      + " "
                                           );
                        radioButtonChangeProcess.Checked = false;
                        return;
                    }
                    else
                    {
                        // Transactions do not exist 
                    }
                }

            }
            else
            {
                labelHeader1.Text = "REPL CYCLE DETAILS";
                buttonValidateDates.Hide();
                buttonValidate.Hide();
                buttonCreateReplenishment.Hide();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                buttonForceItToReplenish.Hide();
                buttonFromInProcessTo0.Hide();
                button_0toInProcess.Hide();
                //dataGridView2.Enabled = true;
            }
        }


        // Button to replenish 
        private void buttonForceItToReplenish_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to Force Replenish this Cycle?" + Environment.NewLine
                          + "Cycle is.. " + WSesNo + " ATM is.." + WAtmNo
                          , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                   == DialogResult.Yes)
            {
                // YES Proceed
                //Ta.Delete_A_Cycle(WAtmNo, WSesNo);
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                Sc.AtmNo = WAtmNo;
                Sc.Form153ChangeDesc = "UPDATE TO 2 For ATM.." + WAtmNo + "..ReplNo=." + WSesNo.ToString();
                Sc.DtTmAtChange = DateTime.Now;
                Sc.ChangeByUser = WSignedId;
                Sc.RMCycleOfChange = WRMCycleNo;
                Sc.ReplCycleNo = WSesNo;

                Sc.SesDtTimeStart_Before = Ta.SesDtTimeStart;
                Sc.SesDtTimeEnd_Before = Ta.SesDtTimeEnd;
                Sc.ProcessMode_Before = Ta.ProcessMode;

                Sc.LoadedAtRMCycle = Ta.LoadedAtRMCycle;
                Sc.ReconcAtRMCycle = Ta.ReconcAtRMCycle;
                Sc.Operator = Ta.Operator;

                SeqNoCor = Sc.InsertSessions_Form153_Corrections(WAtmNo, WSesNo);

                Ta.ProcessMode = 2;
                Ta.Maker = WSignedId;
                Ta.Authoriser = "Force Replenished";
                Ta.ReconcAtRMCycle = WRMCycleNo;
                Ta.Recon1.RecFinDtTm = DateTime.Now;

                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                MessageBox.Show("Cycle Forced Replenished");

                // Keep trace of change 
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (Ta.RecordFound == true)
                {

                    Sc.SesDtTimeStart_After = Ta.SesDtTimeStart;
                    Sc.SesDtTimeEnd_After = Ta.SesDtTimeEnd;
                    Sc.ProcessMode_After = Ta.ProcessMode;

                    Sc.UpdateSessions_Form153_Corrections(SeqNoCor);

                }

                // checkBoxForceReplenish.Checked = false;

                if (int.TryParse(textBoxNumberOfCycles.Text, out NumberOfCycles))
                {

                }
                else
                {
                    MessageBox.Show(textBoxNumberOfCycles.Text, "Invalid field number of Cycles!");
                    return;
                }
                // MessageBox.Show("It will show the last " + NumberOfCycles + " Repl Cycles. ");

                Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);
                ShowGrid_2();

                radioButtonChangeProcess.Checked = false;
                //checkBoxForceReplenish.Checked = false;

            }
            else
            {
                // Stop 
                radioButtonChangeProcess.Checked = false;
                return;
            }
        }

        // Check Box for changing deposits


        // Find Deposit

        private void GetDepositsAmounts()
        {
            // 
            // DEPOSITS HANDLING 
            // 
            // Find the first fuid to avoid duplication created by two the same journals
            //
            //ErrorInDeposits = false;

            SM.Read_SM_AND_Get_First_fuid(WAtmNo, WSesNo);

            //textBoxCcy1CassetteNotes.Text = "0";
            //textBoxCcy1CassetteAmount.Text = "0.00";


            bool IsCcy_1;
            // Get the totals from SM and not from Mpa            
            // GET TABLE
            SM.Read_SM_AND_FillTable_Deposits_2(WAtmNo, WSesNo, SM.Fuid);
            //
            //***********************
            //
            string SM_SelectionCriteria1 = " WHERE AtmNo ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + WSesNo
                                          + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                             ;

            SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, WSesNo, 2);

            if (SM.RecordFound == true)
            {
                // Get other Totals 

                int WDEP_COUNTERFEIT = SM.DEP_COUNTERFEIT;
                int WDEP_SUSPECT = SM.DEP_SUSPECT;

            }

            int WRECYCLEDTotalNotes;
            decimal WRECYCLEDTotalMoney;

            int WTotalNCR_DepositsDispensedNotes;
            decimal WTotalNCR_DepositsDispensedMoney;

            // Read Table 
            int K = 0;
            int I = 0;


            while (I <= (SM.DataTable_SM_Deposits.Rows.Count - 1))
            {
                // "  SELECT Currency As Ccy, SUM(Cassette) as TotalNotes, sum(Facevalue * CASSETTE) as TotalMoney "

                string WCcy = (string)SM.DataTable_SM_Deposits.Rows[I]["Ccy"];

                if (WCcy.Trim() == "")
                {
                    I = I + 1;
                    continue;
                }
                WTotalCassetteNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteNotes"];
                WTotalCassetteMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteMoney"];

                WRetractTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalRETRACTNotes"];
                WRetractTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalRETRACTMoney"];

                WRECYCLEDTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalRECYCLEDNotes"];
                WRECYCLEDTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalRECYCLEDMoney"];

                WTotalNCR_DepositsDispensedNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalNCR_DepositsDispensedNotes"];
                WTotalNCR_DepositsDispensedMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalNCR_DepositsDispensedMoney"];


                K = K + 1;

                if (K == 1)
                {
                    // Fill the first set
                    IsCcy_1 = true;

                    // TOTAL REMAIN IN CASSETTES 
                    // THIS IS THE FORMULA !!!!!!!!!!!!!!!!!!!
                    // (WTotalMoneyDecimal+WRECYCLEDTotalMoney) -  WTotalNCR_DepositsDispensedMoney
                    textBoxCcy1CassetteAmount.Text = ((WTotalCassetteMoney


                        + WRECYCLEDTotalMoney) - WTotalNCR_DepositsDispensedMoney).ToString("#,##0.00");

                    // MAKE CHECK WITH Mpa
                    RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                    int WMode = 2;
                    Mpa.ReadTableDepositsTxnsByAtmNoAndReplCycle_EGP(WAtmNo, dateTimePicker1.Value, dateTimePicker2.Value, WCcy, WMode, 2);

                    int TotNoBNA = Mpa.TotNoBNA;
                    decimal TotValueBNA = Mpa.TotValueBNA;

                }

                I = I + 1;
            }
        }
        // Show Journals 
        private void linkLabelShowJournals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (textBoxATMNo.Text != "")
            {
                MessageBox.Show("This will show the last twenty journals." + Environment.NewLine
               + "In The read sequence"
               );
            }
            else
            {
                MessageBox.Show("No ATM inputed." + Environment.NewLine
             + ""
             );
                return;
            }

            Form78d_Discre NForm78d_Discre;
            int WMode = 6; // So last 20 journals 
            NForm78d_Discre = new Form78d_Discre(WOperator, WSignedId, "", 0, WMode, textBoxATMNo.Text);
            NForm78d_Discre.ShowDialog();
        }
        // Force from -1 to 0 
        private void buttonFromInProcessTo0_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to Force from -1 to 0 ?" + Environment.NewLine
                          + "Cycle is.. " + WSesNo + " ATM is.." + WAtmNo
                          , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                   == DialogResult.Yes)
            {
                // YES Proceed
                //Ta.Delete_A_Cycle(WAtmNo, WSesNo);
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (Ta.ProcessMode == -1)
                {
                    // Keep trace of change 

                    Sc.AtmNo = WAtmNo;
                    Sc.Form153ChangeDesc = "UPDATE FROM -1 to 0 For ATM.." + WAtmNo + "..ReplNo=." + WSesNo.ToString();
                    Sc.DtTmAtChange = DateTime.Now;
                    Sc.ChangeByUser = WSignedId;
                    Sc.RMCycleOfChange = WRMCycleNo;
                    Sc.ReplCycleNo = WSesNo;

                    Sc.SesDtTimeStart_Before = Ta.SesDtTimeStart;
                    Sc.SesDtTimeEnd_Before = Ta.SesDtTimeEnd;
                    Sc.ProcessMode_Before = Ta.ProcessMode;

                    Sc.LoadedAtRMCycle = Ta.LoadedAtRMCycle;
                    Sc.ReconcAtRMCycle = Ta.ReconcAtRMCycle;
                    Sc.Operator = Ta.Operator;

                    SeqNoCor = Sc.InsertSessions_Form153_Corrections(WAtmNo, WSesNo);


                    Ta.ProcessMode = 0;
                    // Ta.Maker = WSignedId;
                    // Ta.Authoriser = "";
                    // Ta.ReconcAtRMCycle = WRMCycleNo;
                    // Ta.Recon1.RecFinDtTm = DateTime.Now;

                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                    MessageBox.Show("Cycle Forced From -1 to 0 ");
                    radioButtonChangeProcess.Checked = false;

                    // Keep trace of change 
                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                    if (Ta.RecordFound == true)
                    {

                        Sc.SesDtTimeStart_After = Ta.SesDtTimeStart;
                        Sc.SesDtTimeEnd_After = Ta.SesDtTimeEnd;
                        Sc.ProcessMode_After = Ta.ProcessMode;

                        Sc.UpdateSessions_Form153_Corrections(SeqNoCor);

                    }

                    //checkBoxForceReplenish.Checked = false;
                    if (int.TryParse(textBoxNumberOfCycles.Text, out NumberOfCycles))
                    {

                    }
                    else
                    {
                        MessageBox.Show(textBoxNumberOfCycles.Text, "Invalid field number of Cycles!");
                        return;
                    }
                    // MessageBox.Show("It will show the last " + NumberOfCycles + " Repl Cycles. ");

                    Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

                    ShowGrid_2();

                    //  checkBoxForceReplenish.Checked = false;
                }
                else
                {
                    MessageBox.Show("This Cycle has no Proces mode -1 ");
                    radioButtonChangeProcess.Checked = false;
                    return;
                }

            }
            else
            {
                // Stop 
                radioButtonChangeProcess.Checked = false;
                return;
            }
        }
        // From 0 to -1 
        private void button_0toInProcess_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to Force from 0 to -1 ?" + Environment.NewLine
                         + "Cycle is.. " + WSesNo + " ATM is.." + WAtmNo
                         , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                  == DialogResult.Yes)
            {
                // YES Proceed
                //Ta.Delete_A_Cycle(WAtmNo, WSesNo);
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (Ta.ProcessMode == 0)
                {
                    // Keep trace of change 

                    Sc.AtmNo = WAtmNo;
                    Sc.Form153ChangeDesc = "UPDATE FROM 0 to -1 For ATM.." + WAtmNo + "..ReplNo=." + WSesNo.ToString();
                    Sc.DtTmAtChange = DateTime.Now;
                    Sc.ChangeByUser = WSignedId;
                    Sc.RMCycleOfChange = WRMCycleNo;
                    Sc.ReplCycleNo = WSesNo;

                    Sc.SesDtTimeStart_Before = Ta.SesDtTimeStart;
                    Sc.SesDtTimeEnd_Before = Ta.SesDtTimeEnd;
                    Sc.ProcessMode_Before = Ta.ProcessMode;

                    Sc.LoadedAtRMCycle = Ta.LoadedAtRMCycle;
                    Sc.ReconcAtRMCycle = Ta.ReconcAtRMCycle;
                    Sc.Operator = Ta.Operator;

                    SeqNoCor = Sc.InsertSessions_Form153_Corrections(WAtmNo, WSesNo);



                    Ta.ProcessMode = -1;
                    // Ta.Maker = WSignedId;
                    // Ta.Authoriser = "";
                    // Ta.ReconcAtRMCycle = WRMCycleNo;
                    // Ta.Recon1.RecFinDtTm = DateTime.Now;

                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                    MessageBox.Show("Cycle Forced From 0 to -1 ");

                    radioButtonChangeProcess.Checked = false;

                    // Keep trace of change 
                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                    if (Ta.RecordFound == true)
                    {

                        Sc.SesDtTimeStart_After = Ta.SesDtTimeStart;
                        Sc.SesDtTimeEnd_After = Ta.SesDtTimeEnd;
                        Sc.ProcessMode_After = Ta.ProcessMode;

                        Sc.UpdateSessions_Form153_Corrections(SeqNoCor);

                    }

                    // checkBoxForceReplenish.Checked = false;

                    if (int.TryParse(textBoxNumberOfCycles.Text, out NumberOfCycles))
                    {

                    }
                    else
                    {
                        MessageBox.Show(textBoxNumberOfCycles.Text, "Invalid field number of Cycles!");
                        return;
                    }
                    // MessageBox.Show("It will show the last " + NumberOfCycles + " Repl Cycles. ");

                    Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

                    ShowGrid_2();

                    // checkBoxForceReplenish.Checked = false;
                }
                else
                {
                    MessageBox.Show("This Cycle has no Process mode 0 ");
                    radioButtonChangeProcess.Checked = false;
                    return;
                }



            }
            else
            {
                // Stop 
                radioButtonChangeProcess.Checked = false;
                return;
            }
        }
        // Audit trail 
        private void linkLabelAuditTrail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form67_BDC NForm67_BDC;
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            int Mode = 10; // 
            string WTraceRRNumber = "";
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTraceRRNumber, WAtmNo
                , 0, 0, NullPastDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();
        }
        // ADD DEPOSIT
        int I_FuId;
        string I_AtmNo;
        DateTime I_SM_DATE_TIME;
        string I_TYPE;

        string I_Currency;
        int I_FaceValue;
        int I_CASSETTE;
        int I_RETRACT;

        int I_RECYCLED;
        int I_ReplCycle;
        bool I_Processed;
        int I_LoadedAtRMCycle;

        string WCurrency2;
        int WFaceValue;
        int InNotes;

        private void buttonAdd_Click(object sender, EventArgs e)
        {

            WCurrency2 = comboBoxCcy.Text;

            if (int.TryParse(comboBoxFaceValue.Text, out WFaceValue))
            {
                // OK
            }
            else
            {
                MessageBox.Show("Please enter correct Facevalue");
            }


            if (int.TryParse(textBoxNumberOfNotes.Text, out InNotes))
            {
                // OK
            }
            else
            {
                MessageBox.Show("Please enter correct Deposit Notes");
                return;
            }

            // Check if already exists record Ccy and Denomination already exists 
            // Present that it will be deleted and this one will be inserted 



            // Update Deposits.
            // Delete all currect entries and insert a new single record if new amount > 0 

            // Prepare Fields 
            // Keep details
            // string WCurrency = "EGP";
            //SM.Read_SM_Deposits_Get_DetailsOfFirstRecord(WAtmNo, WSesNo, WCurrency2);
            //if (SM.RecordFound)
            //{
            //    I_FuId = SM.Fuid;
            //    I_AtmNo = SM.AtmNo;
            //    I_SM_DATE_TIME = SM.SM_DATE_TIME;
            //    I_TYPE = "EGP5";

            //    I_Currency = WCurrency2;
            //    I_FaceValue = WFaceValue;
            //    I_CASSETTE = InNotes;
            //    I_RETRACT = 0;

            //    I_RECYCLED = 0;
            //    I_ReplCycle = SM.ReplCycle;
            //    I_Processed = SM.Processed;
            //    I_LoadedAtRMCycle = SM.LoadedAtRMCycle;
            //}
            //else
            //{
            SM.Read_SM_Record_Specific_By_ReplCycle(WSesNo);

            I_FuId = SM.Fuid;
            I_AtmNo = WAtmNo;
            I_SM_DATE_TIME = SM.SM_dateTime_Start;
            I_TYPE = "EGP" + WFaceValue.ToString();

            I_Currency = WCurrency2;
            I_FaceValue = WFaceValue;
            //textBoxNumberOfNotes
            I_CASSETTE = InNotes;
            I_RETRACT = 0;

            I_RECYCLED = 0;
            I_ReplCycle = WSesNo;
            I_Processed = true;
            I_LoadedAtRMCycle = SM.LoadedAtRMCycle;
            //}

            // Delete records in Deposits for this entry 

            SM.Delete_Deposit_Entries(WAtmNo, WSesNo, WCurrency2, WFaceValue);

            // Insert Record
            SM.Fuid = I_FuId;
            SM.AtmNo = I_AtmNo;
            SM.SM_DATE_TIME = I_SM_DATE_TIME;
            SM.TYPE = I_TYPE;

            SM.Currency = I_Currency;
            SM.FaceValue = I_FaceValue;
            SM.CASSETTE = I_CASSETTE;
            SM.RETRACT = I_RETRACT;

            SM.RECYCLED = I_RECYCLED;
            SM.NCR_DepositsDispensed = 0;
            SM.ReplCycle = I_ReplCycle;
            SM.Processed = I_Processed;
            SM.LoadedAtRMCycle = I_LoadedAtRMCycle;

            SM.InsertDepositRecord(WAtmNo, WSesNo, WCurrency2);

            SM.Read_SM_AND_FillTable_Deposits_Baddies(WAtmNo, WSesNo);

            ShowGrid_3();

            GetDepositsAmounts();

        }
        // Delete Depo
        private void buttonDeleteDep_Click(object sender, EventArgs e)
        {
            WCurrency2 = comboBoxCcy.Text;

            if (int.TryParse(comboBoxFaceValue.Text, out WFaceValue))
            {
                // OK
            }
            else
            {
                MessageBox.Show("Please enter correct Facevalue");
            }


            if (int.TryParse(textBoxNumberOfNotes.Text, out InNotes))
            {
                // OK
            }
            else
            {
                MessageBox.Show("Please enter correct Deposit Notes");
                return;
            }


            if (MessageBox.Show("Warning: Do you want to Delete this deposit?" + Environment.NewLine
                           + "Ccy is.. " + WCurrency2 + Environment.NewLine
                            + "Face Value is.. " + comboBoxFaceValue.Text + Environment.NewLine
                             + "Motes is.. " + textBoxNumberOfNotes.Text
                           , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
            {
                //Check if outstanding before deleting
                // Keep trace of change 
                SM.Read_SM_Deposits_Get_DetailsBySelectionCriteria(WAtmNo, WSesNo, WCurrency2, WFaceValue, InNotes);
                //Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (SM.RecordFound == true)
                {

                    // Delete record 
                    SM.Delete_Deposit_Entries(WAtmNo, WSesNo, WCurrency2, WFaceValue);

                    MessageBox.Show("Deposit Record Deleted");

                    SM.Read_SM_AND_FillTable_Deposits_Baddies(WAtmNo, WSesNo);

                    ShowGrid_3();

                    GetDepositsAmounts();

                }
                else
                {
                    MessageBox.Show("Not Found Record");
                    return;
                }

            }
            else
            {
                // Stop 

                return;
            }

        }

        bool IsRecycle = false;
        bool FOREX_Deposits;
        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;
        bool ToxicJournal;

        decimal OpeningBal;
        decimal TotalDebit_IST;
        decimal TotalCredit_IST;

        decimal TotalReversals_IST;

        bool InternalChange;
        bool Show_No_Reversals;
        bool FirstCycle;

        decimal Corrected_DR_IST;

        decimal Remains_IST_DR;
        decimal Remains_IST_CR;

        decimal LoadedAmount;

        public void Set_IST_INFO()
        {

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

            RRDMRepl_SupervisorMode_Details_Recycle Sm = new RRDMRepl_SupervisorMode_Details_Recycle();

            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            string ParId2 = "948";
            string OccurId2 = "1"; // 
            //RRDMGasParameters Gp = new RRDMGasParameters(); 
            Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                // CHECK IF RECYCLING
                SM.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                if (SM.RecordFound == true)
                {
                    // Check if Reccyle 
                    if (SM.is_recycle == "Y")
                    {
                        IsRecycle = true;

                        //checkBox_Allow_Balance.Checked = true;

                        labelRemain.Text = "Remains for both" + Environment.NewLine
                                            + "Cassettes+Deposits";
                        //labelDiffCR.Text = "Difference" + Environment.NewLine
                        //                   + "Cassettes+Deposits";
                        labelDeposits.Text = "Deposits for" + Environment.NewLine
                                            + "Recycling";

                        labelRemainCR.Hide();
                        textBoxRemainingCR.Hide();

                        labelDiffCR.Hide();
                        textBoxIST_DiffDep.Hide();

                        //labelGuadingMessage.Show();
                        //panelGuidingInfo.Show(); 
                    }
                    else
                    {
                        //checkBox_Allow_Balance.Checked = false;
                        //labelRecycleNote.Hide();
                        // labelGuadingMessage.Hide();
                        // panelGuidingInfo.Hide();
                    }
                }
            }
            OpeningBal = 850000; // FOR BANK DE CAIRE
            // GET DATES 
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            DateTmSesStart = Ta.SesDtTimeStart;
            DateTmSesEnd = Ta.SesDtTimeEnd;

            // For header
            //int WFunction = 1;
            //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // REPL TO REP IS DONE WITHIN THIS CLASS 

            //textBoxType1.Text = Convert.ToInt32(Na.Cassettes_1.FaceValue).ToString();
            //textBoxType2.Text = Convert.ToInt32(Na.Cassettes_2.FaceValue).ToString();
            //textBoxType3.Text = Convert.ToInt32(Na.Cassettes_3.FaceValue).ToString();
            //textBoxType4.Text = Convert.ToInt32(Na.Cassettes_4.FaceValue).ToString();

            int WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);
            if (Na.GL_Balance == 0)
            {
                Na.GL_Balance = OpeningBal; // Keep the Change here
                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);
            }// 

            //// Has FOREX? 

            //// Check through IST for FOREX DEPOSITS function for this ATM
            //checkBoxHasForex.Checked = false;
            //string WTableId = "Switch_IST_Txns";
            //int WMode = 15;
            //Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, WMode, 2);

            //int rows = Mgt.DataTableAllFields.Rows.Count;

            //if (rows > 0)
            //{
            //    FOREX_Deposits = true;
            //    checkBoxHasForex.Checked = true;
            //}
            //else
            //{
            //    WTableId = "tblMatchingTxnsMasterPoolATMs";
            //    WMode = 16;
            //    Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, WMode, 2);
            //    rows = Mgt.DataTableAllFields.Rows.Count;
            //    if (rows > 0)
            //    {
            //        FOREX_Deposits = true;
            //        checkBoxHasForex.Checked = true;
            //    }
            //    else
            //    {
            //        FOREX_Deposits = false;
            //    }

            //}


            // HAS presenter
            //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            //int Mode = 1;
            //Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_REPL(WOperator, WSignedId, Mode, WAtmNo,
            //                               Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            //if (Mpa.TotalSelected > 0)
            //{
            //    // Exceptions to show for replenishment 
            //    Exceptions = true;
            //    checkboxHasExceptions.Checked = true;
            //}
            //else
            //{
            //    Exceptions = false;
            //    checkboxHasExceptions.Checked = false;
            //}



            //if (IsRecycle == true)
            //{
            //    panelDeposits.Show();
            //}
            //else
            //{
            //    panelDeposits.Hide();
            //}


            ToxicJournal = false;
            if (Ta.ProcessMode == -6)
            {
                ToxicJournal = true;
                //labelJournalHeading.Text = "JOURNAL Is TOXIC=>Information cannot be shown";
                //panel_JournalInfo.Hide();
                ////textBoxOpenBal.ReadOnly = false;
                //panelWarning.Hide();
                //panel_Journal_Excess.Hide();
                //labelJournalExcess.Hide();
            }

            // SHOW CIT ID AND NAME
            //Ac.ReadAtm(WAtmNo);
            //int WAtmsReconcGroup = Ac.AtmsReconcGroup;
            //labelGroup.Text = "Belongs to.." + WAtmsReconcGroup.ToString();
            //Ac.CitId

            //Us.ReadUsersRecord(Ac.CitId);

            //labelCitId.Text = "CIT_ID: " + Ac.CitId;
            //labelCIT_NAME.Text = "NAME: " + Us.UserName;

            //// NOTES for final comment 
            //Order = "Descending";
            //WParameter4 = "Replenishment closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            //WSearchP4 = "";
            //Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            //if (Cn.RecordFound == true)
            //{
            //    labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            //}
            //else labelNumberNotes2.Text = "0";

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            //Usi.ReadSignedActivityByKey(WSignRecordNo);

            //if (WViewFunction == true || WAuthoriser == true || WRequestor == true) // THIS is not normal process 
            //{
            //    ViewWorkFlow = true;

            //    if (Cn.TotalNotes == 0)
            //    {
            //        //label1.Hide();

            //        buttonNotes2.Hide();
            //        labelNumberNotes2.Hide();
            //    }
            //    else
            //    {
            //        buttonNotes2.Show();
            //        labelNumberNotes2.Show();

            //    }
            //}
            //else
            //{
            //    buttonNotes2.Show();
            //    labelNumberNotes2.Show();
            //}

            // Opening balance 
            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // REPL TO REP IS DONE WITHIN THIS CLASS 
            InternalChange = false;
            if (Na.GL_Balance > 0)
            {
                // Means that manual balance was inputed 
                InternalChange = true;
                //checkBox_Allow_Balance.Checked = true;
                OpeningBal = Na.GL_Balance;

                Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                OpeningBal = // After date Cap_date
                              Sm.ATM_total1 * Na.Cassettes_1.FaceValue
                            + Sm.ATM_total2 * Na.Cassettes_2.FaceValue
                            + Sm.ATM_total3 * Na.Cassettes_3.FaceValue
                            + Sm.ATM_total4 * Na.Cassettes_4.FaceValue;

                OpeningBal = 850000; // by force for Bank De Caire

            }
            else
            {
                //OpeningBal = Na.Balances1.OpenBal;// this is wrong for recycling

                if (Na.PreSes == 0)
                {

                    Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                    OpeningBal = // After date Cap_date
                                  Sm.ATM_total1 * Na.Cassettes_1.FaceValue
                                + Sm.ATM_total2 * Na.Cassettes_2.FaceValue
                                + Sm.ATM_total3 * Na.Cassettes_3.FaceValue
                                + Sm.ATM_total4 * Na.Cassettes_4.FaceValue;

                    OpeningBal = 850000; // by force for Bank De Caire

                    //MessageBox.Show("The Openning Balance is not available" + Environment.NewLine
                    //   + "Input Open Balance Manually" +
                    //    "");

                }
                else
                {
                    Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                    OpeningBal = // After date Cap_date
                                  Sm.ATM_total1 * Na.Cassettes_1.FaceValue
                                + Sm.ATM_total2 * Na.Cassettes_2.FaceValue
                                + Sm.ATM_total3 * Na.Cassettes_3.FaceValue
                                + Sm.ATM_total4 * Na.Cassettes_4.FaceValue
                                  ;
                    OpeningBal = 850000; // by force for Bank De Caire

                    //Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, Na.PreSes);
                    //OpeningBal = // After date Cap_date
                    //    //Sm.ATM_cassette1 
                    //              Sm.cashaddtype1 * Na.Cassettes_1.FaceValue
                    //            + Sm.cashaddtype2 * Na.Cassettes_2.FaceValue
                    //            + Sm.cashaddtype3 * Na.Cassettes_3.FaceValue
                    //            + Sm.cashaddtype4 * Na.Cassettes_4.FaceValue
                    //              ;
                    //textBoxInAmount_Excel_Amt.Text = LoadedAmount.ToString("#,##0.00");
                    //textBoxJournal.Text = LoadedAmount.ToString("#,##0.00");
                }

                // SET 
                OpeningBal = 850000;

            }

            //FROM IST
            int WMode = 1;
            string WTableId = "Switch_IST_Txns";

            if (WMode == 1)
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, DateTmSesStart, DateTmSesEnd, WMode, 2);

                TotalDebit_IST = Mgt.TotalDebit;
                TotalCredit_IST = Mgt.TotalCredit;


            }

            // FIND THE ONES THAT WERE REVERSED during reconciliation (Actions 91,92) 

            int Mode = 5;
            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_Actions_91_92(WOperator, WSignedId, Mode, WAtmNo,
                                           Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 1);

            if (Show_No_Reversals == true)
            {
                Mpa.TotalDebit = 0;
                Mpa.TotalCredit = 0;
            }

            //     public decimal TotalDebit;
            //public decimal TotalCredit;
            TotalReversals_IST = Mpa.TotalDebit;

            Corrected_DR_IST = TotalDebit_IST - TotalReversals_IST;

            if (IsRecycle == true)
            {
                // If Recycle we add all deposits on Opening balance and we subtract all Withdrawls
                // This gives us the remains for both the Cassettes + Deposits 
                Remains_IST_DR = (OpeningBal + (TotalCredit_IST + Mpa.TotalCredit)) - Corrected_DR_IST;
                // REMAINS CANNOT BE DEFINED FROM IST FOR DEPOSITS

            }
            else
            {
                // Not Recycle 
                Remains_IST_DR = OpeningBal - Corrected_DR_IST;

                Remains_IST_CR = TotalCredit_IST + Mpa.TotalCredit;
                //Remains_IST_CR 
            }
            // INPUT FIGURES ... LEAVE IT HERE
            //textBox_Input_Amnt.Text = Na.Cit_UnloadedCounted.ToString("##0.00");
            //textBoxCitInputDeposits.Text = Na.GL_Bal_Repl_Adjusted.ToString("##0.00");

            if (Na.Is_GL_Adjusted == true)
            {
                // radioButtonGLBased_Journal.Checked = true;
            }
            ;

            // Populate textBoxes 

            textBoxOpenBal.Text = OpeningBal.ToString("#,##0.00"); // Leave the opening balance here 

            textBoxDR_IST.Text = TotalDebit_IST.ToString("#,##0.00");

            textBoxReversals.Text = TotalReversals_IST.ToString("#,##0.00");

            textBoxReversals.Show();

            textBoxRemaining.Text = Remains_IST_DR.ToString("#,##0.00");

            if (Remains_IST_DR < 0)
            {
                labelInvalid.Show();
            }
            else
            {
                labelInvalid.Hide();
            }
            // TEXTBOX for Deposits
            //textBoxRemainingCR.Text = Remains_IST_CR.ToString("#,##0.00");
            textBoxDeposits.Text = TotalCredit_IST.ToString("#,##0.00");

            textBoxReversalsDep.Text = Mpa.TotalCredit.ToString("#,##0.00"); // Not defined yet 

            if (IsRecycle == true)
            {
                textBoxRemainingCR.Text = "Not Defined in IST";
                //textBoxRemainingCR.Text = textBoxCitInputDeposits.Text;
            }
            else
            {
                textBoxRemainingCR.Text = (Remains_IST_CR).ToString("#,##0.00");
            }

            // Journal Section 

            //textBoxMachineBal.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
            //JournalCassettes = Na.Balances1.MachineBal;
            //JournalPresenter = Na.Balances1.PresenterValue;
            //textBoxPresenter.Text = JournalPresenter.ToString("#,##0.00");

            //JournalExpected = (Na.Balances1.MachineBal + Na.Balances1.PresenterValue);

            //textBoxExpected.Text = JournalExpected.ToString("#,##0.00");

            //GetDeposits();

            //if (DepositsEGPFound == true)
            //{
            //    // Remaining Deposits for Recycling 
            //    SM_TotalDeposits = WTotalMoneyDecimal;
            //    //textBoxTotalDeposits.Text = SM_TotalDeposits.ToString("#,##0.00");
            //    //textBoxRecycleAmt.Text = WRECYCLEDTotalMoney.ToString("#,##0.00");

            //    SM_RemainingDep = WTotal_SM_Money;
            //    textBoxSM_deposits.Text = SM_RemainingDep.ToString("#,##0.00");

            //}
            //else
            //{
            //    textBoxSM_deposits.Text = "0.00";
            //}
            //// Haddle input figures
            //HandleInput(0);
            ////
            // Deal with Money In
            //


            //Sm.Read_SM_Record_For_AddedCash(WAtmNo, NextReplCycle, WCAP_DATE_2); 
            //decimal CashAdded = // After date Cap_date
            //              Sm.Total_cashaddtype1 * Na.Cassettes_1.FaceValue
            //            + Sm.Total_cashaddtype2 * Na.Cassettes_2.FaceValue
            //            + Sm.Total_cashaddtype3 * Na.Cassettes_3.FaceValue
            //            + Sm.Total_cashaddtype4 * Na.Cassettes_4.FaceValue
            //              ;
            //Na.ReadSessionsNotesAndValues(WAtmNo, NextReplCycle, 2);
            //LoadedAmount = Na.Balances1.OpenBal - CashAdded; 

            Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
            LoadedAmount = // After date Cap_date
                          Sm.cashaddtype1 * Na.Cassettes_1.FaceValue
                        + Sm.cashaddtype2 * Na.Cassettes_2.FaceValue
                        + Sm.cashaddtype3 * Na.Cassettes_3.FaceValue
                        + Sm.cashaddtype4 * Na.Cassettes_4.FaceValue
                          ;

            textBoxJournal.Text = LoadedAmount.ToString("#,##0.00");

        }

        private void linkLabelUnmatchedTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            int WSelection = 4; // include SOLO

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, 0, WOperator, WFunction, 0,
                                                WAtmNo, DateTmSesStart, DateTmSesEnd, WSelection
                );
            NForm80b2.Show();
        }

        private void buttonRefreshIST_Click(object sender, EventArgs e)
        {
            Set_IST_INFO();
        }
        // Show Update dates
        private void radioButtonUpdateOnlyDates_CheckedChanged(object sender, EventArgs e)
        {
            buttonUpdateOnlyDATES.Show();
            labelHeader3.Hide();
            panel4.Hide();
            labelDeposits.Hide();
            panelDeposits.Hide();
            panelDepBig.Hide(); 

            // buttonValidateDates.Show(); 
        }

        private void buttonUpdateOnlyDATES_Click(object sender, EventArgs e)
        {

            CheckErrorInDatesUpdate();

            if (ErrorInDates == true)
            {
                return;
            }

            // update 
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            if (SM.RecordFound == true)
            {
                Ta.SesDtTimeStart = dateTimePicker1.Value;
                Ta.SesDtTimeEnd = dateTimePicker2.Value;

                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
            }

            Set_IST_INFO();
        }
    }
}

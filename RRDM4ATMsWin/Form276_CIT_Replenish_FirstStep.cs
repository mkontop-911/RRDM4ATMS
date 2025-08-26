using RRDM4ATMs;
using System;
using System.Configuration;
using System.Data;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace RRDM4ATMsWin
{
    public partial class Form276_CIT_Replenish_FirstStep : Form
    {
        //   FormMainScreen NFormMainScreen;

        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        DateTime WDate;


        int TempMode;

        int WReconcCycleNo;

        bool ViewCompletedWorkFlow;
        bool ViewNotCompletedWorkFlow;

        DateTime FutureDate = new DateTime(2050, 11, 21);

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime WDtFrom = new DateTime(2021, 08, 20);

        DateTime WDtTo = new DateTime(2021, 11, 21);

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();

        RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();


        //     FormMainScreen NFormMainScreen;
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        int WAction;

        string WCitId;
        int WLoadingExcelCycle;
        int WRMCycle;

        public Form276_CIT_Replenish_FirstStep(string InSignedId, int SignRecordNo, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WAction = InAction;  // 1 

            // Leave them for the time being 
            WCitId = "";
            WLoadingExcelCycle = 0;
            WRMCycle = 0;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUser.Text = InSignedId;

            this.WindowState = FormWindowState.Maximized;

            //************************************************************
            // 2: Loaded from Excel
            // 4: Valid At Maker (Ready for transactions creation) 
            // 6: Invalid At Maker - Not Ready for transactions creation
            //************************************************************

            string WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
                                   //   NormalProcess = false;

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor)
            //      if (WViewFunction || WAuthoriser || WRequestor || WViewHistory)
            {


                // buttonAdd.Enabled = false;
                //buttonUpdate.Enabled = false;
                //buttonDelete.Enabled = false;

                // checkBoxSameAsPrevious.Enabled = false; 

            }
            else
            {
                //  NormalProcess = true;
            }

            if (WAuthoriser == true)
            {
                //panel2.Location.X.Equals = 9; 
            }

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewCompletedWorkFlow = true;
            }

            if (Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewNotCompletedWorkFlow = true;
            }
        }

        // ON LOAD OF FORM DO 
        private void Form26_Load(object sender, EventArgs e)
        {
            string WSelectionCriteria = " WHERE [Valid_Entry] = 1  AND UserId='" + WSignedId+ "'"; // Show the ones that Journal was found 

            Ce.ReadRecordsFrom_CIT_Excel_Records_Form276_CIT_Replenish(WSelectionCriteria);

            if (Ce.DataTableAllFields.Rows.Count > 0)
            {

                ShowGrid1();

                textBoxTotalBlue.Text = Total_DarkBlue.ToString();
                textBoxTotalGreen.Text = Total_Greens.ToString();
                textBoxTotalRed.Text = Total_Red.ToString();
                textBoxWaitingAuth.Text = Total_Yellow.ToString(); 

            }
            else
            {
                MessageBox.Show("NO outstanding ATMS For replenishement ");
            }

        }

        // Row Enter
        int WSeqNo;
        int WMode;
        string WAtmNo;
        int WReplCycle;
        double minutesDifference;
        int wholeNumber; 

        //
        // Choose from Grid 
        //
        //int WRow;
        //int scrollPosition;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Ce.Read_CIT_Excel_TableBySeqNo(WSeqNo);

            WAtmNo = Ce.AtmNo;

            checkBoxIsForex.Hide(); 

            //ItHasForex = Ce.Check_IF_FOREX_IN_BULK(WBulkRecordId);

            // Check if this entry is valid for the basics 
            // ATMs Group
            // Owner 
            // ATM replenished by whom? 
            //  RRDMUsersRecords Ur = new RRDMUsersRecords(); 
            Ac.ReadAtm(WAtmNo);
            if (Ac.RecordFound == true)
            {
                //if (Ac.CitId == "1000" || Ac.AtmsReplGroup == 0 || Ac.AtmReplUserId == null || Ac.AtmReplUserId =="")
                //{
                //    MessageBox.Show("Please update the details of this ATM"+ Environment.NewLine
                //                    + "Current details are: " + Environment.NewLine
                //                    + "CitId:.."+ Ac.CitId + Environment.NewLine
                //                    + "Ac.AtmReplUserId:.." + Ac.AtmReplUserId 
                //        );
                //    return; 
                //}
                
                
            }
            else
            {
                MessageBox.Show("ATM.." + WAtmNo + "..Not Found in Database" + Environment.NewLine
                                  + "No Journal was loaded yet  " + Environment.NewLine 
                                  + "Please insert all details " + Environment.NewLine
                                  ) ;
                return; 

            }

            //Ce.GroupOfAtmsRRDM = Ac.AtmsReplGroup;
            //Ce.UserId = Ac.AtmReplUserId;
            //Ce.Update_User_AND_GROUP(WSeqNo);

            Ta.ReadReplCycles_FOR_ATM_And_DATE(WAtmNo, Ce.ReplCycleEndDate.Date);

            if (Ta.RecordFound == true)
            {
                Ce.SesDtTimeStart = Ta.SesDtTimeStart;
                Ce.SesDtTimeEnd = Ta.SesDtTimeEnd;

                WJProcessMode = Ce.ProcessMode = Ta.ProcessMode;
                Ce.ReplCycle = Ta.SesNo;
                Ce.Journal = true;
                Ce.JournalForced = false;

                Ce.UpdateAtmOfDatesFromJournal(WSeqNo);
            }

            // READ AGAIN Updated Ce
            Ce.Read_CIT_Excel_TableBySeqNo(WSeqNo);

            //We have 5 groups(A, B & C) For withdrawal ATMS
            //(D & R) for (deposit & recycle) ATMS
            // For Recycling we apply : Remaining = (Open Bal+ Deposits)- Withdrawals
            // so this must be equal to what CIT input for cassettes + Deposits
            if (Ce.GROUP_ATMS == "A" || Ce.GROUP_ATMS == "B" || Ce.GROUP_ATMS == "C")
            {
                textBoxATM_Type.Text = ".."+Ce.GROUP_ATMS + "..No Deposit Machine";
            }
            if (Ce.GROUP_ATMS == "D")
            {
                textBoxATM_Type.Text = ".." + Ce.GROUP_ATMS + "..With Deposit Machine";
            }
            if (Ce.GROUP_ATMS == "R")
            {
                textBoxATM_Type.Text = ".." + Ce.GROUP_ATMS + "..Recycling ATM";
            }

            if (Ce.STATUS =="")
            {
                textBoxSTATUS.Text = "00_Not Valid Entry";
                linkLabelView.Hide();
                linkLabelView.Hide();
                buttonNextToRepl.Hide(); 
            }
            if (Ce.STATUS == "01")
            {
                textBoxSTATUS.Text = "01_To Take Action";
                buttonNextToRepl.Show();
                linkLabelView.Hide();
            }
            if (Ce.STATUS == "00")
            {
                textBoxSTATUS.Text = "00_To Take Action";
                buttonNextToRepl.Show();
                linkLabelView.Hide();
            }
            if (Ce.STATUS == "02")
            {
                textBoxSTATUS.Text = "02_Under Auth Status";
                linkLabelView.Show();
                buttonNextToRepl.Hide();
            }
            if (Ce.STATUS == "03")
            {
                textBoxSTATUS.Text = "03_Workflow Completed";
                linkLabelView.Show();
                buttonNextToRepl.Hide();
            }
            if (Ce.STATUS == "04")
            {
                textBoxSTATUS.Text = "04_Auto Completed";
                //buttonUndo.Show();
                linkLabelView.Show();
                buttonNextToRepl.Hide();

                buttonUpdateCIT_Dt.Enabled = false;
                buttonUpDateJNLDates.Enabled = false;
                buttonCreateNewReplCycle.Enabled = false;
                buttonUpdateLine.Enabled = false; 
            }
            else
            {
                //buttonUndo.Hide();
            }

            textBoxAtm.Text = WAtmNo;

            dateTimePicker1.Value = Ce.ReplCycleStartDate;
            dateTimePicker2.Value = Ce.ReplCycleEndDate;

            wholeNumber = 0; 

            if (Ce.Journal == true)
            {
                WReplCycle = Ce.ReplCycle;
                linkSM_lines.Show();
                TimeSpan difference = Ce.SesDtTimeEnd - Ce.ReplCycleEndDate;
                minutesDifference = difference.TotalMinutes;
                wholeNumber = (int)minutesDifference;
                textBoxDiffMin.Text = wholeNumber.ToString();
                labelDiff.Show();
                textBoxDiffMin.Show();
                linkShowJournal.Show();

                textBoxReplCycle.Text = WReplCycle.ToString();
            }
            else
            {
                WReplCycle = 0;
                linkShowJournal.Hide(); 
                linkSM_lines.Hide();
                labelDiff.Hide();
                textBoxDiffMin.Hide();
                textBoxDiffMin.Text = "";
                textBoxReplCycle.Text = ""; 
            }
            dateTimePicker3.Value = Ce.SesDtTimeStart;
            dateTimePicker4.Value = Ce.SesDtTimeEnd;

            // Show figures 
            if (Ce.STATUS == "01" || Ce.STATUS == "02")
            {
                ShowFiguresForCitOrJNL_Dates();
                labelAccounting.Show();
                panelAccounting.Show();
            }
            else
            {
                labelAccounting.Hide();
                panelAccounting.Hide(); 
            }
           

            // SHOW PEVIOUS RECORDS OF ATM 

            Ce.ReadRecordsFrom_CIT_Excel_Records_Replenish_for_ATM(WAtmNo, WSeqNo);

            // GET TABLE AND SHOW RESULTS
            ShowGrid_3();

            // SHOW REPLENISHMENT CYCLES
            int NumberOfCycles = 10;
            Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

            if (Ta.RecordFound == true)
            {
                ShowGrid_2();

                if (WRow2 == -2)
                {
                    // Continue without assigning the line 
                }
                else
                {
                    dataGridView2.Rows[WRow2].Selected = true;
                    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
                }
            }
            else
            {
                dataGridView2.DataSource = null; // Unbind the data source
                dataGridView2.DataSource = new DataTable(); // Set a new empty DataTable

                WReplCycle = 0;
                textBoxReplCycle.Text = ""; 
            }

        }

        bool IsRecycle = false;

        // int WSeqNo;
        //  string WAtmNo;
        DateTime WReplCycleStartDate;
        DateTime WReplCycleEndDate;
        decimal WCIT_Total_Replenished;
        decimal WCIT_Total_Returned;
        decimal WCIT_Total_Deposit_Local_Ccy;
        string WGROUP_ATMS;
        int WTransType;

        decimal SwitchDebits;
        decimal SwitchDeposits;

        decimal JournalDebits;
        decimal JournalDeposits;
        // Recycle 
        decimal Remains_Switch;
        decimal Remains_Journal;
        decimal Remains_CIT;
        // Not Recycle 
        decimal Remains_Switch_Returned;
        decimal Remains_Switch_Deposits;

        decimal Remains_Journal_Returned;
        decimal Remains_Journal_Deposits;

        bool ItHasForex ;

        decimal TotalPresenter;

        private void ShowFiguresForCitOrJNL_Dates()
        {
            // Initialise Variables
            //
            DateTime DateStart = NullPastDate;
            DateTime DateEnd = NullPastDate;

            textBoxDiff_Return.Text = "0.00";

            textBoxDiff_Deposits.Text = "0.00";
            //textBox_Error_DR.Text = "0.00";

            textBoxPresenter.Text = "0.00";

            //textBoxErrors_CR.Text = "0.00";

            WCIT_Total_Replenished = Ce.CIT_Total_Replenished;

            textBoxLoaded.Text = Ce.CIT_Total_Replenished.ToString("#,##0.00");

            WCIT_Total_Returned = Ce.CIT_Total_Returned;
            WCIT_Total_Deposit_Local_Ccy = Ce.CIT_Total_Deposit_Local_Ccy;

            if (radioButton_CIT_Based.Checked == true)
            {
                WReplCycleStartDate = dateTimePicker1.Value;
                WReplCycleEndDate = dateTimePicker2.Value;
            }

            if (radioButtonJLN_Based.Checked == true)
            {
                WReplCycleStartDate = dateTimePicker3.Value;
                WReplCycleEndDate = dateTimePicker4.Value;
            }

            // Check if it has forex. 
            ItHasForex = Ce.Check_IF_FOREX_IN_BULK(Ce.BulkRecordId);

            if (ItHasForex == true)
            {
                checkBoxIsForex.Show();
                checkBoxIsForex.Checked = true; 
            }
            else
            {
                checkBoxIsForex.Hide();
            }
            
            if (Ce.GROUP_ATMS == "R")
            {
                IsRecycle = true;
            }
            else
            {
                IsRecycle = false;
            }

            WTransType = 11;
            SwitchDebits = Ce.GetTotalsFrom_SWITCH_By_Dates(WAtmNo, WReplCycleStartDate
                                                                                    , WReplCycleEndDate, WTransType);
            WTransType = 23;
            SwitchDeposits = Ce.GetTotalsFrom_SWITCH_By_Dates(WAtmNo, WReplCycleStartDate
                                                                                    , WReplCycleEndDate, WTransType);
            TotalPresenter = Ce.GetTotalsFrom_Master_For_Presented(WAtmNo, WReplCycleStartDate
                                                                                    , WReplCycleEndDate);
            //decimal JournalDebits;
            //decimal JournalDeposits;
            WTransType = 11;
            JournalDebits = Ce.GetTotalsFrom_JOURNAL_By_Dates(WAtmNo, WReplCycleStartDate
                                                                                , WReplCycleEndDate, WTransType);
            WTransType = 23;
            JournalDeposits = Ce.GetTotalsFrom_JOURNAL_By_Dates(WAtmNo, WReplCycleStartDate
                                                                                , WReplCycleEndDate, WTransType);

            //We have 5 groups(A, B & C) For withdrawal ATMS
            //(D & R) for (deposit & recycle) ATMS
            // For Recycling we apply : Remaining = (Open Bal+ Deposits)- Withdrawals
            // so this must be equal to what CIT input for cassettes + Deposits

            if (IsRecycle == true)
            {
                // If Recycle we add all deposits on Opening balance and we subtract all Withdrawls
                // This gives us the remains for both the Cassettes + Deposits 
                // Remains_IST_DR = (WCIT_Total_Replenished + (TotalCredit_IST + Mpa.TotalCredit)) - Corrected_DR_IST;
                Remains_Switch = WCIT_Total_Replenished + SwitchDeposits - SwitchDebits;
                Remains_Journal = WCIT_Total_Replenished + JournalDeposits - JournalDebits;
                Remains_CIT = WCIT_Total_Returned + WCIT_Total_Deposit_Local_Ccy;
                // REMAINS CANNOT BE DEFINED FROM IST FOR DEPOSITS
                if (Remains_CIT == Remains_Switch)
                {
                    //CounterMathedRecycle = CounterMathedRecycle + 1;

                    Ce.SWITCH_Total_Returned = Remains_Switch;
                    Ce.SWITCH_Total_Deposit_Local_Ccy = 0;

                    // Journal
                    Ce.JNL_SM_Total_Returned = Remains_Journal;
                    Ce.JNL_SM_Deposit_Local_Ccy = 0;

                    Ce.OverFound_Amt_Cassettes = 0;
                    Ce.ShortFound_Amt_Cassettes = 0;
                    Ce.PresentedErrors = TotalPresenter;
                    Ce.CreatedDate = DateTime.Now;

                    //if (ItHasForex == true || TotalPresenter>0)
                    //{
                    //    Ce.STATUS = "01";
                    //}
                    //else
                    //{
                    //    Ce.STATUS = "03";
                    //}
                    

                    Ce.UpdateAtmDuringMatchingOfCitExcel(WSeqNo);

                }
                else
                {
                    //CounterUnMatchedRecycle = CounterUnMatchedRecycle + 1;

                    Ce.SWITCH_Total_Returned = Remains_Switch;
                    Ce.SWITCH_Total_Deposit_Local_Ccy = 0;

                    Ce.JNL_SM_Total_Returned = Remains_Journal;
                    Ce.JNL_SM_Deposit_Local_Ccy = 0;

                    if (Remains_CIT > Remains_Switch)
                    {
                        Ce.OverFound_Amt_Cassettes = Remains_CIT - Remains_Switch;
                    }
                    else
                    {
                        Ce.ShortFound_Amt_Cassettes = Remains_Switch - Remains_CIT;
                    }

                    // If presenter go thoough the cycle 
                    Ce.PresentedErrors = TotalPresenter;

                    //Ce.CreatedDate = DateTime.Now;
                    //Ce.STATUS = "01";

                    Ce.UpdateAtmDuringMatchingOfCitExcel(WSeqNo);
                }

                textBoxDiff_Return.Text = (Remains_CIT - Remains_Switch).ToString("#,##0.00");

            }
            else
            {
                // Not Recycle 

                Remains_Switch_Returned = WCIT_Total_Replenished - SwitchDebits;
                Remains_Switch_Deposits = SwitchDeposits;

                Remains_Journal_Returned = WCIT_Total_Replenished - JournalDebits;
                Remains_Journal_Deposits = JournalDeposits;

                // Cassettes

                Ce.SWITCH_Total_Returned = Remains_Switch_Returned;
                Ce.SWITCH_Total_Deposit_Local_Ccy = Remains_Switch_Deposits;

                Ce.JNL_SM_Total_Returned = Remains_Journal_Returned;
                Ce.JNL_SM_Deposit_Local_Ccy = Remains_Journal_Deposits;

                Ce.OverFound_Amt_Cassettes = 0;
                Ce.ShortFound_Amt_Cassettes = 0;

                if (WCIT_Total_Returned > Remains_Switch_Returned)
                {
                    Ce.OverFound_Amt_Cassettes = WCIT_Total_Returned - Remains_Switch_Returned;

                }
                if (Remains_Switch_Returned > WCIT_Total_Returned)
                {
                    Ce.ShortFound_Amt_Cassettes = Remains_Switch_Returned - WCIT_Total_Returned;
                    // Make it minus
                    //textBoxDiff_Return.Text = (-Ce.ShortFound_Amt_Cassettes).ToString("#,##0.00");
                }

                textBoxDiff_Return.Text = (WCIT_Total_Returned - Remains_Switch_Returned).ToString("#,##0.00");

                // Deposits 

                Ce.OverFound_Amt_Deposits = 0;
                Ce.ShortFound_Amt_Deposits = 0;

                if (WCIT_Total_Deposit_Local_Ccy > Remains_Switch_Deposits)
                {
                    Ce.OverFound_Amt_Deposits = WCIT_Total_Deposit_Local_Ccy - Remains_Switch_Deposits;

                }
                if (Remains_Switch_Deposits > WCIT_Total_Deposit_Local_Ccy)
                {
                    Ce.ShortFound_Amt_Deposits = Remains_Switch_Deposits - WCIT_Total_Deposit_Local_Ccy;
                }

                textBoxDiff_Deposits.Text = (WCIT_Total_Deposit_Local_Ccy - Remains_Switch_Deposits).ToString("#,##0.00");

                Ce.PresentedErrors = TotalPresenter;

                textBoxPresenter.Text = TotalPresenter.ToString("#,##0.00");

                // Update Process mode

                //if ((WCIT_Total_Returned + WCIT_Total_Deposit_Local_Ccy)
                //     == (Remains_Switch_Returned + Remains_Switch_Deposits) & ItHasForex == false & TotalPresenter == 0
                //     )
                //{
                //    Ce.STATUS = "03";
                //}
                //else
                //{
                //    Ce.STATUS = "01";
                //}

                Ce.CreatedDate = DateTime.Now;

                Ce.UpdateAtmDuringMatchingOfCitExcel(WSeqNo);

            }

            //Remains_Switch = WCIT_Total_Replenished + SwitchDeposits - SwitchDebits;
            //Remains_Journal = WCIT_Total_Replenished + JournalDeposits - JournalDebits;
            //Remains_CIT = WCIT_Total_Returned + WCIT_Total_Deposit_Local_Ccy;

            if (IsRecycle == true)
            {
                textBoxCIT_Return.Text = Remains_CIT.ToString("#,##0.00");
                textBoxSwitch_Return.Text = Remains_Switch.ToString("#,##0.00");
                textBoxJLN_Return.Text = Remains_Journal.ToString("#,##0.00");
                // For recycling are all included in above 
                textBoxCit_Deposits.Text = "0";
                textBoxSwitch_Deposits.Text = "0"; 
                textBoxJLN_Deposits.Text = "0";

            }
            else
            {
                //Remains_Switch_Returned = WCIT_Total_Replenished - SwitchDebits;
                //Remains_Switch_Deposits = SwitchDeposits;

                //Remains_Journal_Returned = WCIT_Total_Replenished - JournalDebits;
                //Remains_Journal_Deposits = JournalDeposits;

                textBoxCIT_Return.Text = Ce.CIT_Total_Returned.ToString("#,##0.00");
                textBoxCit_Deposits.Text = Ce.CIT_Total_Deposit_Local_Ccy.ToString("#,##0.00");

                textBoxSwitch_Return.Text = Remains_Switch_Returned.ToString("#,##0.00");
                textBoxSwitch_Deposits.Text = Remains_Switch_Deposits.ToString("#,##0.00");

                textBoxJLN_Return.Text = Remains_Journal_Returned.ToString("#,##0.00");
                textBoxJLN_Deposits.Text = Remains_Journal_Deposits.ToString("#,##0.00");
            }

            //MessageBox.Show("By Changing date look if there is a journal equivalent . DEVELOPMENT"); 

        }
        int WWSeqNo;
        int WJProcessMode; 
        // ROW ENTER 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WWSeqNo = (int)rowSelected.Cells[0].Value; // SesNo
            //textBoxReplNo.Text = WWSeqNo.ToString();
            Ta.ReadSessionsStatusTraces(WAtmNo, WWSeqNo);
            //WJProcessMode = Ta.ProcessMode; // This is the line process mode 
            //WReplCycle = WWSeqNo; 
            if (Ta.ProcessMode ==-1)
            {
                dateTimePicker7.Value = Ta.SesDtTimeStart;
                dateTimePicker8.Value = Ta.SesDtTimeEnd;
            }
            else
            {
                if (WWSeqNo == WReplCycle)
                {
                    dateTimePicker3.Value = Ta.SesDtTimeStart;
                    dateTimePicker4.Value = Ta.SesDtTimeEnd;
                    WJProcessMode = Ta.ProcessMode;
                }
               
                dateTimePicker7.Value = Ta.SesDtTimeStart;
                dateTimePicker8.Value = Ta.SesDtTimeEnd;
            }      

        }

        //******************
        // SHOW GRID dataGridView2
        //******************
        int Total_Greens;
        int Total_DarkBlue;
        int Total_Red;
        int Total_Yellow;
        private void ShowGrid1()
        {
            Total_DarkBlue = 0; 
            Total_Greens = 0 ; 
            Total_Red = 0 ;
            Total_Yellow = 0; 

            dataGridView1.DataSource = null;
            dataGridView1.Refresh();

            dataGridView1.DataSource = Ce.DataTableAllFields.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }
            //" SELECT SeqNo  "
            //     + " , STATUS "
            //    + " , AtmNo "
            //    + " , ReplCycleStartDate As CIT_Start "
            //    + "  , ReplCycleEndDate As CIT_End  "
            //    + "  , CIT_Total_Returned "
            //     + "  , CIT_Total_Deposit_Local_Ccy "
            //      + "  , SWITCH_Total_Returned As SWITCH_Returns "
            //     + "  , SWITCH_Total_Deposit_Local_Ccy As SWITCH_Deposits "
            //    + "  , Journal  "
                //+ "  , CIT_ID  "

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].Visible = true;

            dataGridView1.Columns[1].Width = 55; // STATUS
            //dataGridView1.Columns[1].DefaultCellStyle = style;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 60; // AtmNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 100; // ReplCycleStartDate As CIT_Start
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 100; // ReplCycleEndDate As CIT_End
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].Visible = true;

            dataGridView1.Columns[5].Width = 70; // CIT_Total_Returned
            dataGridView1.Columns[5].DefaultCellStyle = style;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].HeaderText = "CIT_Returned"; 

            dataGridView1.Columns[6].Width = 70; // CIT_Total_Deposit_Local_Ccy
            dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].HeaderText = "CIT_Deposits";

            dataGridView1.Columns[7].Width = 70; // SWITCH_Returns
            dataGridView1.Columns[7].DefaultCellStyle = style;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].HeaderText = "Switch_Returned";

            dataGridView1.Columns[8].Width = 70; // SWITCH_Deposits
            dataGridView1.Columns[8].DefaultCellStyle = style;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[8].HeaderText = "Switch_Deposits";

            dataGridView1.Columns[9].Width = 40; // Journal
            //dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 40; // CIT_ID 
            //dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                string STATUS = (string)row.Cells[1].Value;

                if (STATUS == "01" || STATUS == "00")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Red;
                    Total_Red = Total_Red + 1;
                }
                if (STATUS == "02")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Yellow;
                    Total_Yellow = Total_Yellow + 1;
                }
                if (STATUS == "03")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Green;
                    Total_Greens = Total_Greens + 1; 
                }
                if (STATUS == "04")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.DarkBlue;
                    Total_DarkBlue = Total_DarkBlue + 1;
                }

            }
        }

        // Show Grid 2
        //int WRowNumber = 0;  
        private void ShowGrid_2()
        {

            dataGridView2.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }

            dataGridView2.Columns[0].Width = 70; // SesNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);

            dataGridView2.Columns[1].Width = 120; // SesDtTimeStart
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 120; // SesDtTimeEnd
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 50; // 
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 250; // ProcessMode
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            int tempSesNo = 0;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {

                tempSesNo = (int)row.Cells[0].Value;
               
                if (tempSesNo == WReplCycle )
                {
                    if (WJProcessMode == 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.GreenYellow;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    
                    WRow2 = row.Index;

                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    WRow2 = -2;
                }

            }
        }
        private void ShowGrid_3()
        {

            dataGridView3.DataSource = Ce.DataTableATMFields.DefaultView;

            if (dataGridView3.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                //dataGridView3.Rows.Clear();

                return;
            }

            dataGridView3.Columns[0].Width = 70; // SesNo
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);
            dataGridView3.Columns[1].Width = 55; // STATUS
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[2].Width = 120; // CIT_TimeStart
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[3].Width = 120; // CIT_TimeEnd
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView3.Columns[3].Width = 50; // STATUS
            //dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[4].Width = 250; // ProcessMode
            //dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //int tempSesNo = 0;

            //foreach (DataGridViewRow row in dataGridView2.Rows)
            //{

            //    tempSesNo = (int)row.Cells[0].Value;

            //    if (tempSesNo == WReplCycle)
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Red;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //        WRow2 = row.Index;

            //    }
            //    else
            //    {
            //        row.DefaultCellStyle.BackColor = Color.White;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }

            //}
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Show SM LINES 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            string SM_SelectionCriteria1 = " WHERE atmno ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + WReplCycle
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

            SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, WReplCycle, 2);

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
        // IST Time Journey based on dates and Times of CIT 
        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime StartDate = dateTimePicker1.Value; // Make range of searhing bigger 
            DateTime EndDate = dateTimePicker2.Value;
            string WTableId = "CIT_EXCEL_TO_BANK";
            int WMode = 18;
            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, StartDate.Date
                               , EndDate.Date.AddDays(1), WReplCycle, NullPastDate, WMode);

            NForm78D_ATMRecords.Show();
        }
        // NEXT TO REPLENISH
        int WRow1;
        int WRow2;

        private void buttonNextToRepl_Click(object sender, EventArgs e)
        {

            if (WSignedId == "ahm.osman")
            {
                // Continue
            }
            else
            {
                MessageBox.Show("Only Osman can sign in ");

                return;
            }

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WReplCycle, "Replenishment");

            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
            {
                MessageBox.Show("This Replenishment Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process to complete."
                                                          );
                return;
            }

            if (Ce.STATUS == "02")
            {
                MessageBox.Show("Replenishment Cycle Under Checker Mode");
                return; 
            }
            if (Ce.STATUS == "03")
            {
                MessageBox.Show("Replenishment Cycle Completed");
                return;
            }
            int DifMinutes = 20; 
            if (wholeNumber > DifMinutes & wholeNumber != 0)
            {
                MessageBox.Show("CIT and Journal Repl Date "+ Environment.NewLine
                    + "Differ more than__"+ DifMinutes.ToString()
                    );
                return; 
            }

           // WReplCycle

            if (WReplCycle == 0)
            {
                MessageBox.Show("Repl Cycle is Zero " + Environment.NewLine
                 //  + "Differ more than__" + DifMinutes.ToString()
                   );
                return;
            }
            //Keep Row Selection positioning 
            WRow1 = dataGridView1.SelectedRows[0].Index;
            if (dataGridView2.Rows.Count > 0)
            {
                WRow2 = dataGridView2.SelectedRows[0].Index;
            }
            else
            {
                MessageBox.Show("No Replenishment Cycle Available");
                return;
            }

            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);

            if (Ta.RecordFound == false)
            {
                MessageBox.Show("Replenishment Cycle_"+ WReplCycle.ToString()+ " Not Found");
                return;
            }

            if (Ta.RecordFound == true & Ta.ProcessMode ==2)
            {
                MessageBox.Show("Already Replenishment Finalised");
                return;
            }

            // UPDATE TRANSACTIONS WITH REPL CYCLE BASED ON DATES

            Mpa.UpdateMpaRecordsWithReplCycle(WOperator, WSignedId
                                          , WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd
                                                     , WReplCycle, 2);
            // Check if outstanding 
            string SelectionCriteria2 = " WHERE Operator ='" + WOperator + "'"
                          + " AND  TerminalId ='" + WAtmNo + "'"
                         + "  AND IsMatchingDone = 1 "
                         + "  AND Matched = 0 "
                         + "  AND SettledRecord = 0 " // Not Settled missmatched for this cycle
                         + " And ReplCycleNo =" + WReplCycle;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria2, 1);

            if (Mpa.RecordFound == true)
            {
                if (MessageBox.Show("Important Warning:" + Environment.NewLine
                             + "There are outstanding Unmatched at RMCategory... " + Mpa.RMCateg + Environment.NewLine
                             + "You must settle the unmatched before you do replenishment." + Environment.NewLine
                             + "Do you want still want to proceed with replenishment?" + Environment.NewLine
                             , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                {
                    // YES

                }
                else
                {
                    // NOT TO PROCEED 
                    return;
                }
            }

            //
            // This has to go to Replenishment Cycle 
            //
            string WSelection = " WHERE AtmNo ='" + WAtmNo + "' And ReplCycle =" + WReplCycle;
            Ce.Read_CIT_Excel_Table_BySelectionCriteria(WSelection);

            // WSelection = " WHERE SeqNo=" + Ce.BulkRecordId; 
            Ce.UPDATE_FOREX_IN_SM(WAtmNo, WReplCycle, Ce.BulkRecordId);


            // Process No Updating 
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ProcessNo = 1; // NORMAL 

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            // CALL THE SAME If Recycle or not 
            bool IsFromExcel = true; 
            Form51_Repl_For_IST NForm51_Repl_For_IST;
            NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WReplCycle,IsFromExcel );
            NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
            NForm51_Repl_For_IST.ShowDialog();
        }

        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {

            int WRow1 = dataGridView1.SelectedRows[0].Index;

            int scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

            //int WRow2 = dataGridView2.SelectedRows[0].Index;

            //int scrollPosition2 = dataGridView2.FirstDisplayedScrollingRowIndex;

            Form26_Load(this, new EventArgs());
            // First
            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition1;

            // second

            //dataGridView2.Rows[WRow2].Selected = true;
            //dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

            //dataGridView2.FirstDisplayedScrollingRowIndex = scrollPosition2;

        }
        // Update New JLN Dates
        private void buttonUpDateJNLDates_Click(object sender, EventArgs e)
        {
            if (WReplCycle == 0)
            {
                MessageBox.Show("No Journal For Updating." + Environment.NewLine
                    + "Maybe you need to open a new Cycle? "
                    );

                return; 
            }
            CheckErrorInDatesUpdate(dateTimePicker3.Value,dateTimePicker4.Value, WWSeqNo);

            if (ErrorInDates == true)
            {
                return;
            }

            SM.Read_SM_Record_Specific_By_ReplCycle(WReplCycle);

            SM.SM_dateTime_Start = dateTimePicker4.Value;

            SM.SM_dateTime_Finish = dateTimePicker4.Value;

            SM.SM_LAST_CLEARED = dateTimePicker3.Value;

            SM.Update_SM_Record_From_Form153(SM.SeqNo);

            //
            // Supervisor mode
            //
            RRDMRepl_SupervisorMode_Master Smaster = new RRDMRepl_SupervisorMode_Master();
            int Sm_Mode = 4;

            Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, 0, WOperator,
                                                             Sm_Mode, "Form153");

            MessageBox.Show("Replenishment Dates has been Changed");

            // SHOW REPLENISHMENT CYCLES
            int NumberOfCycles = 10;
            Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

            // UPDATE Ce with new dates 

            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);

            Ce.SesDtTimeStart = Ta.SesDtTimeStart;
            Ce.SesDtTimeEnd = Ta.SesDtTimeEnd;
            Ce.ProcessMode = Ta.ProcessMode;
            Ce.ReplCycle = WReplCycle;
            Ce.Journal = true; 
            if (Ce.JournalForced == true)
            {
                Ce.JournalForced = true; 
            }
            else
            {
                Ce.JournalForced = false;
            }

            Ce.UpdateAtmOfDatesFromJournal(WSeqNo);

            ShowFiguresForCitOrJNL_Dates();


            int WRow1 = dataGridView1.SelectedRows[0].Index;

            int scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

            //int WRow2 = dataGridView2.SelectedRows[0].Index;

            //int scrollPosition2 = dataGridView2.FirstDisplayedScrollingRowIndex;

            Form26_Load(this, new EventArgs());
            // First
            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition1;

            // ShowGrid_2();

        }
        // CHECK DATES
        bool ErrorInDates;
        private void CheckErrorInDatesUpdate(DateTime DtFrom , DateTime DtTo, int InReplCycle)
        {
            if (DtTo <= DtFrom)
            {
                ErrorInDates = true;
                MessageBox.Show(" Second Date smaller than first " + Environment.NewLine
                    + "Please change dates"
                    );
                return;
            }

            Ta.ReadSessionsStatusTraces(WAtmNo, InReplCycle);

            Ta.ReadSessionsStatusTracesToFindNextSesion(WAtmNo, Ta.SesDtTimeEnd);

            // Ta.ReadSessionsStatusTraces(WAtmNo, Ta.NextSes);
            DateTime WorkingDt = DtTo;
            if (WorkingDt > Ta.SesDtTimeStart & Ta.SesNo != InReplCycle)
            {
                ErrorInDates = true;
                MessageBox.Show("Input second date greater " + Environment.NewLine
                    + "than start of the next cycle "
                    );
                return;
            }
            // Check for Overlapping
            Ta.ReadReplCyclesAndValidateDatesUpdate(WAtmNo, DtFrom, DtTo, InReplCycle);

            if (Ta.ErrorInDates == true)
            {
                ErrorInDates = true;
                MessageBox.Show(Ta.ErrorInDatesMsg);
            }
            else
            {
                ErrorInDates = false;

                //MessageBox.Show("Input dates are accepted");
            }

        }

        // UPDATE CIT DT 
        private void buttonUpdateCIT_Dt_Click(object sender, EventArgs e)
        {
            // Check input dates are valid
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("Invalid Dates");
                return;
            }

            Ce.ReplCycleStartDate = dateTimePicker1.Value;
            Ce.ReplCycleEndDate = dateTimePicker2.Value;

            Ce.Update_CIT_DATES(WSeqNo);

            ShowFiguresForCitOrJNL_Dates();

            int WRow1 = dataGridView1.SelectedRows[0].Index;

            int scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

            //int WRow2 = dataGridView2.SelectedRows[0].Index;

            //int scrollPosition2 = dataGridView2.FirstDisplayedScrollingRowIndex;

            Form26_Load(this, new EventArgs());
            // First
            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition1;



        }

        // New replenishment Cycle 
        private void buttonCreateNewReplCycle_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You want to create a new Replenishement Cycle" + Environment.NewLine
                    + "They shouldnt be dates overlapping" + Environment.NewLine
                    + "Loaded Amount will only be created"
                    );

            //CheckErrorInDatesUpdate();

            if (dateTimePicker6.Value <= dateTimePicker5.Value)
            {
                ErrorInDates = true;
                MessageBox.Show(" Second Date smaller than first " + Environment.NewLine
                    + "Please change dates"
                    );
                return;
            }

            // Check for Overlapping
            Ta.ReadReplCyclesAndValidateDatesUpdate(WAtmNo, dateTimePicker5.Value, dateTimePicker6.Value, WReplCycle);

            if (Ta.ErrorInDates == true)
            {
                ErrorInDates = true;
                MessageBox.Show(Ta.ErrorInDatesMsg);

                return;

            }
            else
            {
                // NO OVERLAPPING 
                ErrorInDates = false;
                //MessageBox.Show("Input dates are accepted");
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
                if (Ce.GROUP_ATMS == "R")
                {
                    IsRecycle = true;
                    SM.is_recycle = "Y";

                }
                else
                {
                    IsRecycle = false;
                    SM.is_recycle = "N";
                }
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
            SM.SM_dateTime_Start = dateTimePicker6.Value.AddMinutes(-1);

            SM.SM_dateTime_Finish = dateTimePicker6.Value;

            SM.SM_LAST_CLEARED = dateTimePicker5.Value;

            SM.LoadedAtRMCycle = Ce.LoadedAtRMCycle;

            SM.ATM_total1 = 0;
            SM.ATM_total2 = 1000;
            SM.ATM_total3 = 2000;
            SM.ATM_total4 = 3000;

            SM.ATM_Dispensed1 = 0;
            SM.ATM_Dispensed2 = 0;
            SM.ATM_Dispensed3 = 0;
            SM.ATM_Dispensed4 = 0;

            SM.ATM_Remaining1 = 0; // Remain is equal to cassette 
            SM.ATM_Remaining2 = 0;
            SM.ATM_Remaining3 = 0;
            SM.ATM_Remaining4 = 0;

            SM.ATM_Rejected1 = 0;
            SM.ATM_Rejected2 = 0;
            SM.ATM_Rejected3 = 0;
            SM.ATM_Rejected4 = 0;

            SM.ATM_cassette1 = 0; // Cassette is equal to Remain 
            SM.ATM_cassette2 = 1000;
            SM.ATM_cassette3 = 2000;
            SM.ATM_cassette4 = 3000;

            SM.cashaddtype1 = 0;
            SM.cashaddtype2 = 1000;
            SM.cashaddtype3 = 2000;
            SM.cashaddtype4 = 3000;

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
                & Tax.SesDtTimeEnd <= dateTimePicker6.Value
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
                dateTimePicker6.Value > Tax.SesDtTimeStart // It is a new above 
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

            // FIND REPL CYCLE through Tempfuid
            SM.Read_SM_Record_Specific(Tempfuid);
            WReplCycle = SM.RRDM_ReplCycleNo; 

            MessageBox.Show("New Cycle has been created");
            //SM.Read_SM_Record_Specific(hh);
            // SHOW REPLENISHMENT CYCLES
            int NumberOfCycles = 10;
            Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

            // UPDATE Ce with new dates 

            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);

            Ce.SesDtTimeStart = Ta.SesDtTimeStart;
            Ce.SesDtTimeEnd = Ta.SesDtTimeEnd;
            Ce.ProcessMode = Ta.ProcessMode;
            Ce.ReplCycle = WReplCycle;
            Ce.Journal = true;
            Ce.JournalForced = true; 

            Ce.UpdateAtmOfDatesFromJournal(WSeqNo);

            //ShowFiguresForCitOrJNL_Dates();

            //Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

            //// Keep trace of change 
            //int WWSesNo = Ta.ReadReplCycles_Last_SesNo_For_This_ATM(WAtmNo);

            //Ta.ReadSessionsStatusTraces(WAtmNo, WWSesNo);

            int WRow1 = dataGridView1.SelectedRows[0].Index;

            int scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

            //int WRow2 = dataGridView2.SelectedRows[0].Index;

            //int scrollPosition2 = dataGridView2.FirstDisplayedScrollingRowIndex;

            Form26_Load(this, new EventArgs());
            // First
            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition1;

        }
// CHECK FOREX 
        private void button1_Click(object sender, EventArgs e)
        {
            //
            // This has to go to Replenishment Cycle 
            //
            //string WSelection = " WHERE AtmNo ='" + WAtmNo + "' And ReplCycle =" + WReplCycle;
            //Ce.Read_CIT_Excel_Table_BySelectionCriteria(WSelection);

            //// WSelection = " WHERE SeqNo=" + Ce.BulkRecordId; 
            //Ce.UPDATE_FOREX_IN_SM(WAtmNo, WReplCycle, Ce.BulkRecordId);

        }
        // Show Journal 
        private void linkShowJournal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMReconcFileMonitorLog Fl = new RRDMReconcFileMonitorLog();
            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);
            DateTime ReplDate = Ta.SesDtTimeEnd.Date;
            
            string formateddate = ReplDate.ToString("yyyy-MM-dd");

            int WFuid = 0; 
            Fl.ReadFindFuidBased_ATM_DateOfFile(formateddate, WAtmNo); 
            
            if (Fl.RecordFound == true)
            {
                WFuid = Fl.stpFuid; 
            }

            Form67_BDC NForm67_BDC;

            int Mode = 3; // Specific Journal 
            string WTraceRRNumber = "";
            
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, WFuid, WTraceRRNumber
                              , WAtmNo, 0, 0, NullPastDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();
        }
// IST TRANSACTIONS 
        private void linkLabelCycleTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //if (WReplCycle == 0)
            //{
            //    MessageBox.Show("Nothing to show");
            //    return;
            //}
          
            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, Ce.ReplCycleStartDate
                               , Ce.ReplCycleEndDate, WReplCycle, NullPastDate, 1);

            NForm78D_ATMRecords.Show();
        }

// Show Unmatched 
        private void linkLabelUnmatchedTxns_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            int WSelection = 4; // include SOLO

            //Ce.SesDtTimeStart = Ta.SesDtTimeStart;
            //Ce.SesDtTimeEnd = Ta.SesDtTimeEnd;

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                                                WAtmNo, Ce.SesDtTimeStart, Ce.SesDtTimeEnd, WSelection
                );
            NForm80b2.Show();
        }
        // Update line 
        private void buttonUpdateLine_Click(object sender, EventArgs e)
        {
            if (WWSeqNo == 0)
            {
                MessageBox.Show("No Journal For Updating." + Environment.NewLine
                    + "Maybe you need to open a new Cycle? "
                    );

                return;
            }
            CheckErrorInDatesUpdate(dateTimePicker7.Value, dateTimePicker8.Value, WWSeqNo);

            if (ErrorInDates == true)
            {
                return;
            }

            Ta.ReadSessionsStatusTraces(WAtmNo, WWSeqNo);
            if (Ta.ProcessMode != -1)
            {
                SM.Read_SM_Record_Specific_By_ReplCycle(WWSeqNo);

                if (SM.RecordFound == true)
                {
                    
                    SM.SM_dateTime_Start = dateTimePicker8.Value;

                    SM.SM_dateTime_Finish = dateTimePicker8.Value;

                    SM.SM_LAST_CLEARED = dateTimePicker7.Value;

                    SM.Processed = false; 

                    SM.Update_SM_Record_From_Form153(SM.SeqNo);


                }
            }
            if (Ta.ProcessMode == -1)
            {
                Ta.SesDtTimeStart = dateTimePicker7.Value;
                Ta.SesDtTimeEnd = dateTimePicker8.Value;
                Ta.UpdateSessionsStatusTraces(WAtmNo, WWSeqNo);

                MessageBox.Show("Line with Process Mode = -1 has been Changed");
            }
            else
            {
                //
                // Supervisor mode
                //
                RRDMRepl_SupervisorMode_Master Smaster = new RRDMRepl_SupervisorMode_Master();
                int Sm_Mode = 4;

                Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, 0, WOperator,
                                                                 Sm_Mode, "Form153");

                

               
            }

            // SHOW REPLENISHMENT CYCLES
            int NumberOfCycles = 10;
            Ta.ReadReplCycles_Last_Numberof_Cycles(WAtmNo, NumberOfCycles);

            // UPDATE Ce with new dates 

            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);


            //Ce.SesDtTimeStart = Ta.SesDtTimeStart;
            //Ce.SesDtTimeEnd = Ta.SesDtTimeEnd;
            //Ce.ProcessMode = Ta.ProcessMode;
            //Ce.ReplCycle = WReplCycle;

            //Ce.UpdateAtmOfDatesFromJournal(WSeqNo);

            //ShowFiguresForCitOrJNL_Dates();

            int WRow1 = dataGridView1.SelectedRows[0].Index;

            int scrollPosition1 = dataGridView1.FirstDisplayedScrollingRowIndex;

            //int WRow2 = dataGridView2.SelectedRows[0].Index;

            //int scrollPosition2 = dataGridView2.FirstDisplayedScrollingRowIndex;

            Form26_Load(this, new EventArgs());
            // First
            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition1;

        }
// View 
        private void linkLabelView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);

            if (Ta.Stats1.NoOfCheques == 1)
            {
                if (Ta.ProcessMode > 0)
                {
                    RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);
                    Usi.ProcessNo = 54; // View only for replenishment already done  
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


                    // CALL THE SAME If Recycle or not 
                    bool IsFromExcel = true;
                    Form51_Repl_For_IST NForm51_Repl_For_IST;
                    NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WReplCycle, IsFromExcel);
                    NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
                    NForm51_Repl_For_IST.ShowDialog();

                }
                else
                {
                    MessageBox.Show("Not allowed operation. Repl Workflow not done yet");
                }

                return;
            }

        }
// Undo for cycle 
        private void buttonUndo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("THINK if you really need this button."+ Environment.NewLine
                           + "By pressing Undo the actions and transactions made will be deleted" + Environment.NewLine
                           + "The line will turn to Red = you should take new action on it." + Environment.NewLine
                           );
        }
    }
}

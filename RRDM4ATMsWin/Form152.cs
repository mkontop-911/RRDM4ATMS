using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form152 : Form
    {
        Form51 NForm51; // Go to next cash recomendation for replenishment 

        Form71 NForm71; // Reconciliation 

        Form26 NForm26; // Go to capture cards 

        Form38 NForm38; // Manage Deposis 

        string WAtmNo;
        int WSesNo;

        bool Recon_Equal_Repl;

        int WRow1;
        int WRow2;

        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;

        int WProcessMode;

        bool AudiType;

        // int WNextReplNo;

        //bool RecordFound;

        DateTime WDtFrom;
        DateTime WDtTo;

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        //RRDMReconcCountersClass_XXXXX Rc = new RRDMReconcCountersClass_XXXXX();

        RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        RRDMRightToAccessAtm Ra = new RRDMRightToAccessAtm();

        RRDMAtmsClass Aa = new RRDMAtmsClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReplOrdersClass ARa = new RRDMReplOrdersClass();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMErrorsClassWithActions Err = new RRDMErrorsClassWithActions();

        RRDMGasParameters Gp = new RRDMGasParameters();

        bool IsBankDeCaire;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WAction;

        public Form152(string InSignedId, int InSignRecordNo, string InOperator, int Action)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WAction = Action;

            InitializeComponent();

            // 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;

            // WAction codes 
            // 1 : Replenishment of ATMs
            // 2 : Rconciliation of individual ATms 
            // 3 : Captured Cards Management 
            // 4 : Manage Deposits 
            // 5 : Replenishment of ATMs
            // 8 : Calculate Money in 

            // Check if Replenishment = Reconciliation 
            // Centralised Reconciliation
            string ParId = "939";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                Recon_Equal_Repl = true;
            }
            else
            {
                Recon_Equal_Repl = false;
            }


            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            //RRDMGasParameters Gp = new RRDMGasParameters();
            AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(InOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;


                }
                else
                {
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }

            // Call Procedures to see if serious message

            if (WAction == 1) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "ATMs Replenishment";
            }

            if (WAction == 2) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "ATMs Reconciliation";
            }
            if (WAction == 3) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "ATMs Captured Cards";
            }
            if (WAction == 4) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "Navigation Towards Deposits Mgmt";
            }
            if (WAction == 5) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "ATMs Replenishment";
            }
            if (WAction == 8) // RECONCILIATION FOR NOT GROUPS OF ATM 
            {
                labelStep1.Text = "Navigation Towards Money In Need";
            }

            //-----------------UPDATE LATEST TRANSACTIONS----------------------//
            // Update latest transactions from Journal 
            //-------
            if (WOperator == "CRBAGRAA")
            {
                Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);
            }

            dateTimePickerFromDate.Value = DateTime.Today.Date.AddDays(-100);

            if (WOperator == "BCAIEGCX")
            {
                IsBankDeCaire = true;
            }
            else
            {
                IsBankDeCaire = false;
            }

            buttonNextTest.Show();
            linkLabelForexDetails.Show();
            linkLabelForexTotals.Show();
            ////-------------------
            //// SHOW OR HIDE THE BUTTONS FOR NEW CRS 
            //string ParId2 = "948";
            //string OccurId2 = "2"; // 

          
            //Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
            //if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            //{
            //    // Valid for UAT 

            //    DateTime Today = DateTime.Now;
            //    DateTime Sept05 = new DateTime(2024, 09, 05);

            //    if (Today >= Sept05)
            //    {
            //        MessageBox.Show("Communicate with RRDM for new version to extend UAT period");
            //        return;
            //    }
            //    buttonNextTest.Show();
            //    linkLabelForexDetails.Show();
            //    linkLabelForexTotals.Show(); 

            //}
            //else
            //{
            //    buttonNextTest.Hide();
            //    linkLabelForexDetails.Hide();
            //    linkLabelForexTotals.Hide();
            //}

        }
        // LOAD
        private void Form152_Load(object sender, EventArgs e)
        {
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Ua.ReadUserAccess_ToAtmsFillTable(WOperator, WSignedId, "", 2);

            if (Ua.UserGroups_ToAtms_Table.Rows.Count > 0)
            {
                dataGridViewMyATMS.DataSource = Ua.UserGroups_ToAtms_Table.DefaultView;

                if (AudiType == true)
                {
                    ShowGrid_ATMs_1_2();
                }
                else
                {
                    ShowGrid_ATMs_1();
                }

            }
            else
            {
                MessageBox.Show("NO ATMS For this User ");
            }
            //}
            //else
            //{
            //    // ==================ACCESS TO ATMS=========================

            //    //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE---------------//

            //    string AtmNo = "";
            //    string FromFunction = "General";
            //    string WCitId = "";

            //    Us.ReadUsersRecord(WSignedId);
            //    if (Us.CitId != "1000")
            //    {
            //        AtmNo = "";
            //        FromFunction = "FromCit";
            //        WCitId = Us.CitId;
            //    }
            //    else
            //    {
            //        AtmNo = "";
            //        FromFunction = "General";
            //        WCitId = "";
            //    }

            //    // Create table with the ATMs this user can access
            //    Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, AtmNo, FromFunction, WCitId);

            //    //Load Datagrid with what is allowed
            //    dataGridViewMyATMS.DataSource = Am.TableATMsMainSelected.DefaultView;

            //    ShowGrid_ATMs_2();
            //}

            //dataGridViewMyATMS.DataSource = Am.TableATMsMainSelected.DefaultView;


            //TEST
            if (WAction == 3 || WAction == 4)
            {
                WRow1 = 0;
                dataGridViewMyATMS.Rows[WRow1].Selected = true;
                dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            }
        }
        //
        // Row Enter 
        //
        private void dataGridViewMyATMS_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewMyATMS.Rows[e.RowIndex];

            WRow1 = e.RowIndex;

            //if (Recon_Equal_Repl == true & Usi.SecLevel == "03")
            //{
            WAtmNo = (string)rowSelected.Cells[2].Value;
            WAction = 1; // Simulation that this came from branch 
            WSesNo = 0;  // Initialise 
            //}
            //else
            //{
            //    WAtmNo = (string)rowSelected.Cells[0].Value;
            //}

            // CHECK IF ATM IS ACTIVE 
            Am.ReadAtmsMainSpecific(WAtmNo);

            if (Am.ProcessMode == -2)
            {
                label17.Hide();
                panel3.Hide();
                textBoxMsgBoard.Text = "This ATM is not active yet! It will become automatically active when money is added. ";
                MessageBox.Show("This ATM is not active yet!");

                return;
            }
            else
            {
                label17.Show();
                panel3.Show();
            }

            label17.Text = "REPL. CYCLE/s FOR ATM : " + WAtmNo;

            WDtFrom = dateTimePickerFromDate.Value;
            WDtTo = DateTime.Today;

            Ac.ReadAtm(WAtmNo);

            //if (Ac.CitId == "1000")
            //{
            //    MessageBox.Show("This is a not CIT ATM" + Environment.NewLine
            //        + "Replenishment functionallity not ready yet"
            //        ); 
            //}


            // panel3.Show();

            Ta.ReadReplCyclesForFromToDateFillTable(WOperator, WSignedId, WAtmNo, WDtFrom, WDtTo);

            ShowGrid_ReplCycles();


            //dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Columns[6].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            if (WAtmNo == "AB102")
            {
                WRow2 = 2;               // SET ROW Selection POSITIONING 
                dataGridView2.Rows[WRow2].Selected = true;
                dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
            }

            if (WAtmNo == "AB104")
            {
                WRow2 = 1;               // SET ROW Selection POSITIONING 
                dataGridView2.Rows[WRow2].Selected = true;
                dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
            }

            // Read and update 
            Am.ReadAtmsMainSpecific(WAtmNo);
            //    textBox2.Text = Am.CurrentSesNo.ToString();

            if (DateTime.Today > Am.NextReplDt & (WAction == 1 || WAction == 5))
            {
                textBoxMsgBoard.Text = " Replenishment Has been delayed . Please take action ";
            }
            else
            {
                textBoxMsgBoard.Text = " Choose combination of ATM and Repl Cycle and proceed ";
            }

            if (WAction == 2) // Reconciliation of individual ATMs   
            {
                textBoxMsgBoard.Text = "Choose combination of ATM and Repl Cycle and proceed ";
            }

            if (WAction == 3) // Capture Cards management 
            {
                textBoxMsgBoard.Text = "Choose combination of ATM and Repl Cycle and proceed ";
            }
            if (WAction == 4) // Deposits  
            {
                textBoxMsgBoard.Text = "Choose combination of ATM and Repl Cycle and proceed ";
            }
            //}
            //else
            //{
            //    panel3.Hide(); 
            //}


        }

        // ON ROW ENTER Sessions
        int WExcelStatus;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WRow2 = e.RowIndex;

            WSesNo = (int)rowSelected.Cells[0].Value;

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            WExcelStatus = Ta.LatestBatchNo; // Checking FAB for AUto

            WProcessMode = Ta.ProcessMode;
            textBoxProcessMode.Text = WProcessMode.ToString();

            if (Ta.ReplGenComment != "" & Ta.ReplGenComment != "N/A")
            {
                textBoxReplGenComment.Text = Ta.ReplGenComment;
                textBoxReplGenComment.Show();
            }
            else
            {
                textBoxReplGenComment.Hide();
            }

            DateTmSesStart = Ta.SesDtTimeStart;
            DateTmSesEnd = Ta.SesDtTimeEnd;

            if (DateTmSesStart == DateTmSesEnd
                || DateTmSesStart > DateTmSesEnd // Leave it here there is explanation 
                )
            {
                linkLabelFromE_Journal.Hide();
                linkLabelCycleTxns.Hide();
                linkLabelUnmatchedTxns.Hide();
            }
            else
            {
                linkLabelFromE_Journal.Show();
                linkLabelCycleTxns.Show();
                linkLabelUnmatchedTxns.Show();
            }

            if (WAction == 3 || WAction == 4)
            {
                // Captured and Deposits
                label4.Hide();
                textBoxProcessMode.Hide();
                textBoxReplStatus.Hide();
            }
            else
            {
                // Replenishment and reconciliation 
                //if (WProcessMode == -1 || WProcessMode == -5 || WProcessMode == -6)
                if (WProcessMode == -1)
                {
                    Color Red = Color.Red;
                    textBoxReplStatus.ForeColor = Red;
                    textBoxReplStatus.Text = "Not Ready for Repl Workflow" + Environment.NewLine
                        + Ta.ReplGenComment
                        ;
                    buttonNext.Enabled = false;
                    if (WProcessMode == -1 || WProcessMode == -5)
                    {
                        linkLabelSMLines.Enabled = false;
                    }
                    else
                    {
                        linkLabelSMLines.Enabled = true;
                    }
                }
                else
                {
                    buttonNext.Enabled = true;
                    linkLabelSMLines.Enabled = true;
                }

                if (WProcessMode == 1 || WProcessMode == 2)
                {
                    buttonNext.Enabled = false;
                }

                if (WProcessMode == -1)
                {
                    textBoxExcelStatus.Text = "";
                    textBoxExcelStatus.Hide();
                }
                else
                {
                    if (Ta.LatestBatchNo == 2)
                    {
                        textBoxExcelStatus.Show();
                        textBoxExcelStatus.Text = Ta.ExcelStatus;
                    }
                    else
                    {
                        textBoxExcelStatus.Text = "";
                        textBoxExcelStatus.Hide();
                    }

                }


                //
                // Check if this is the next for replenishment
                //

                if (WProcessMode == 0)
                {
                    textBoxReplStatus.Text = "Ready for Repl Workflow";
                    buttonNext.Enabled = true;

                    //RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                    Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

                    if (Ap.RecordFound == true)
                    {
                        textBoxReplStatus.Text = "Repl Cycle Under Authorisation Process";
                        buttonNext.Enabled = false;
                    }
                    else
                    {
                        buttonNext.Enabled = true;
                    }
                }

                if (WProcessMode == 1)
                {
                    if (WAction == 1)
                    {
                        Color Red = Color.Red;
                        textBoxReplStatus.ForeColor = Red;
                        textBoxReplStatus.Text = "Repl Workflow Done! ";
                        buttonNext.Enabled = false;
                    }
                    else
                    {
                        Color Black = Color.Black;
                        textBoxReplStatus.ForeColor = Black;
                        textBoxReplStatus.Text = "Ready for Withdrawls and Deposits Reconciliation";
                        buttonNext.Enabled = true;
                    }
                }

                if (WProcessMode == 2 || WProcessMode == 3)
                {
                    if (WAction == 2 || WAction == 1)
                    {
                        Color Red = Color.Red;
                        textBoxReplStatus.ForeColor = Red;
                        textBoxReplStatus.Text = "Atm has already passed the Reconciliation process for this Repl Cycle";
                        buttonNext.Enabled = false;
                    }
                    else
                    {
                        Color Black = Color.Black;
                        textBoxReplStatus.ForeColor = Black;
                        textBoxReplStatus.Text = "Atm has already passed the Reconciliation process for this Repl Cycle";
                        buttonNext.Enabled = false;
                    }
                }
            }


        }
        // Show Grid 1
        private void ShowGrid_ATMs_1()
        {

            if (dataGridViewMyATMS.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            this.dataGridViewMyATMS.Sort(dataGridViewMyATMS.Columns["InNeed"], System.ComponentModel.ListSortDirection.Descending);

            dataGridViewMyATMS.Columns[0].Width = 90; // User id 
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewMyATMS.Columns[0].Visible = false;

            dataGridViewMyATMS.Columns[1].Width = 60; // InNeed
            dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMyATMS.Columns[2].Width = 90; // ATM No
            dataGridViewMyATMS.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[3].Width = 140; // AtmName
            dataGridViewMyATMS.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[4].Width = 90; // Repl Pending
            dataGridViewMyATMS.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[5].Width = 90; // Repl Cycle
            dataGridViewMyATMS.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridViewMyATMS.Columns[6].Width = 120; // RespBranch
            dataGridViewMyATMS.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[7].Width = 150; // Branch Name
            dataGridViewMyATMS.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            foreach (DataGridViewRow row in dataGridViewMyATMS.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                bool WInNeed = (bool)row.Cells[1].Value;

                if (WInNeed == true)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        // Show Grid 1
        private void ShowGrid_ATMs_1_2()
        {

            if (dataGridViewMyATMS.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            this.dataGridViewMyATMS.Sort(dataGridViewMyATMS.Columns["InNeed"], System.ComponentModel.ListSortDirection.Descending);

            dataGridViewMyATMS.Columns[0].Width = 90; // User id 
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewMyATMS.Columns[0].Visible = false;

            dataGridViewMyATMS.Columns[1].Width = 60; // InNeed
            dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMyATMS.Columns[2].Width = 90; // ATM No
            dataGridViewMyATMS.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[3].Width = 140; // AtmName
            dataGridViewMyATMS.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridViewMyATMS.Columns[4].Width = 150; // Excel Status
            // dataGridViewMyATMS.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[4].Width = 90; // Repl Pending
            dataGridViewMyATMS.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[5].Width = 90; // Repl Cycle
            dataGridViewMyATMS.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridViewMyATMS.Columns[6].Width = 120; // RespBranch
            dataGridViewMyATMS.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[7].Width = 150; // Branch Name
            dataGridViewMyATMS.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            foreach (DataGridViewRow row in dataGridViewMyATMS.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                bool WInNeed = (bool)row.Cells[1].Value;

                if (WInNeed == true)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        // Show Grid 2
        private void ShowGrid_ATMs_2()
        {

            if (dataGridViewMyATMS.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            dataGridViewMyATMS.Columns[0].Width = 90; // ATM No
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridViewMyATMS.Columns[0].Visible = false;

            dataGridViewMyATMS.Columns[1].Width = 90; // ReplCycle
            dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMyATMS.Columns[2].Width = 140; // AtmName

            dataGridViewMyATMS.Columns[3].Width = 120; // RespBranch

            dataGridViewMyATMS.Columns[4].Width = 150; // UserId 

            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridViewMyATMS.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

        }

        // Show Grid 1
        private void ShowGrid_ReplCycles()
        {

            dataGridView2.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;

            dataGridView2.Columns[0].Width = 70; // SesNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            // dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);

            dataGridView2.Columns[1].Width = 125; // SesDtTimeStart
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 125; // SesDtTimeEnd
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 160; // ProcessMode
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 65; // Mode_2
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (AudiType == true)
            {
                dataGridView2.Columns[5].Width = 250; // ExcelStatus
                dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }



        }

        // Proceed button was pressed
        // GO TO NEXT - REPLENISHMENT OR MANAGE DEPOSITS OR RECONCILIATION 
        // OR CAPTURED CARDS OR Transactions 
        //

        // Next process 
        bool IsRecycle;
        private void buttonNext_Click(object sender, EventArgs e)
        {
            //string ParId2 = "948";
            //string OccurId2 = "2"; // 
            //// Haddle Recycling too
            ////RRDMGasParameters Gp = new RRDMGasParameters(); 
            //Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
            //if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            //{
            //    // Continue
            //}
            //else
            //{
            //    return;
            //}
            // Find ATM Branch to be equal to the departmental one
            //if (WSignedId == "ahm.osman")
            //{
            //    // Continue
            //}
            //else
            //{
            //    MessageBox.Show("Only Osman can sign in ");

            //    return;
            //}

            RRDMAccountsClass Acc = new RRDMAccountsClass();
            Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(WOperator, "30", WAtmNo);

            if (Acc.RecordFound == true)
            {
                if (Acc.BranchId == "015" || Acc.BranchId == "0001")
                {
                    // OK Continue 
                }
                else
                {
                    if (Environment.MachineName == "RRDM-PANICOS")
                    {
                        // OK Continue
                    }
                    else
                    {
                        MessageBox.Show("This ATM doesn't belong to branch 015 or 0001");
                        return;
                    }

                }
            }


            //
            // Find out if ATM is Recycling 
            //
            IsRecycle = false;

            string ParId2 = "948";
            string OccurId2 = "1"; // 
            //RRDMGasParameters Gp = new RRDMGasParameters(); 
            Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();
                SM.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                if (SM.RecordFound == true)
                {
                    // Check if Reccyle 
                    if (SM.is_recycle == "Y")
                    {
                        IsRecycle = true;
                    }
                }
            }

            if (AudiType == true)
            {
                if (WExcelStatus == 5)
                {
                    // OK to be done manually
                }
                else
                {
                    if (MessageBox.Show("This Cycle is not defined as ready for Manual input " + Environment.NewLine
                     + "It has status : " + textBoxExcelStatus.Text + Environment.NewLine
                     + "Do you want to Proceed? " + Environment.NewLine
                     , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        // YES Proceed

                    }
                    else
                    {
                        // Stop 
                        return;
                    }

                }
            }

          
            //RRDMGasParameters Gp = new RRDMGasParameters();
            //
            // Check if ATM is set to Hybrid Branch Replenishment / Reconciliation 
            string ParamId;
            string OccurId;
            ParamId = "823";
            string OccuranceId = "01"; // Short
            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);
            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // HYBRID IS ACCEPTED
                    //Ac.ReadAtm(WAtmNo);
                    //if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
                    //{
                    //    MessageBox.Show("Not allowed Operation"
                    //        + "With Current functionality ATM must belong to a CIT."
                    //        );
                    //    return;
                    //}
                }
                else
                {
                    // Hybrid Repl and Reconciliation not accepted
                    Ac.ReadAtm(WAtmNo);
                    if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
                    {
                        MessageBox.Show("Not allowed Operation"
                            + "With Current functionality ATM must belong to a CIT."
                            );
                        return;
                    }
                }
            }
            else
            {
                // Hybrid Repl and Reconciliation not accepted
                Ac.ReadAtm(WAtmNo);
                if ((Ac.CitId == "1000" & Ac.AtmsReplGroup == 0))
                {
                    MessageBox.Show("Not allowed Operation"
                        + "With Current functionality ATM must belong to a CIT "
                        );
                    return;
                }
            }


            Am.ReadAtmsMainSpecific(WAtmNo);
            if (Am.ProcessMode == -2)
            {
                MessageBox.Show("This ATm is not active!");
                return;
            }
            //Keep Row Selection positioning 
            WRow1 = dataGridViewMyATMS.SelectedRows[0].Index;
            if (dataGridView2.Rows.Count > 0)
            {
                WRow2 = dataGridView2.SelectedRows[0].Index;
            }
            else
            {
                MessageBox.Show("No Replenishment Cycle Available");
                return;
            }


            // UPDATE TRANSACTIONS WITH REPL CYCLE BASED ON DATES

            Mpa.UpdateMpaRecordsWithReplCycle(WOperator, WSignedId
                                          , WAtmNo, DateTmSesStart, DateTmSesEnd
                                                     , WSesNo, 2);
            // Check if outstanding 
            string SelectionCriteria2 = " WHERE Operator ='" + WOperator + "'"
                          + " AND  TerminalId ='" + WAtmNo + "'"
                         + "  AND IsMatchingDone = 1 "
                         + "  AND Matched = 0 "
                         + "  AND SettledRecord = 0 " // Not Settled missmatched for this cycle
                         + " And ReplCycleNo =" + WSesNo;

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

            // UPDATE ERRORS WITH REPL CYCLE BASED ON DATES
            //Err.UpdateErrorsWithReplCycleNo(WAtmNo, DateTmSesStart, DateTmSesEnd, WSesNo);

            // REPLENISHMENT PROCESS CODES
            // SINGLES
            // WFunction = 1 Normal Replenishment branch ATM
            // 25 Off site ATM = cassettes are ready and go in ATM
            // 26 Belongs to external - CIT 
            // GROUPS
            // 5 Normal Group belonging to Bank . 
            // 30 Offsite Group belonging to Bank
            // 31 Group belonging to - CIT 

            if (WAction == 1 // Normal from Branch
                || Recon_Equal_Repl == true & Usi.SecLevel == "03" // From Centralised Reconciliation
                ) // REPLENISH NO GROUP 
            {

                // Check LAST RECORD if Already in authorization process

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

                if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
                {
                    MessageBox.Show("This Replenishment Already has authorization record!" + Environment.NewLine
                                             + "Go to Pending Authorisations process to complete."
                                                              );
                    return;
                }

                //if (WSesNo != WNextReplNo)
                //{
                //    MessageBox.Show("This Repl Cycle is not ready. Select : " + WNextReplNo);
                //    return;
                //}
                // User Does Not Have Groups 

                // 

                Ac.ReadAtm(WAtmNo);
                //  CASH MANAGEMENT from RRDM during ATMs in NEED Process 
                string ParId = "202";
                OccurId = "1";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                string RRDMCashManagement = Gp.OccuranceNm;
                //  CASH MANAGEMENT Prior Replenishment Workflow  
                ParId = "211";
                OccurId = "1";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                string CashEstPriorReplen = Gp.OccuranceNm; // IF YES THEN IS PRIOR THOUGHT AN ACTION 

                if (Ac.CitId == "1000" & (RRDMCashManagement == "YES" & CashEstPriorReplen == "YES"))
                {
                    //Check that ACTion for money in was taken
                    //
                    if (WAtmNo == "AB104")
                    {
                        ARa.ReadReplActionsForAtmReplCycleNo(WAtmNo, 9051);
                    }
                    else ARa.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSesNo);

                    if (ARa.RecordFound & ARa.AuthorisedRecord == true)
                    {
                        // ACTION WAS CREATED AND IT IS CONFIRMED
                    }
                    else
                    {
                        if (ARa.RecordFound == false)
                        {

                            MessageBox.Show("No Action Record For Money to replenish created. " + Environment.NewLine
                                       + "Create it please.");

                            return;
                        }
                        if (ARa.RecordFound == true & ARa.AuthorisedRecord == false)
                        {

                            MessageBox.Show("Created Action for Loading Money Not Confirmed. " + Environment.NewLine
                                       + "Confirm it please.");

                            return;
                        }

                    }
                }

                //Ua.ReadUsersAccessAtmTableSpecific(WSignedId, WAtmNo, 0);

                //if (Ua.RecordFound == false || (Ua.RecordFound == true & Ua.Replenishment == false))
                //{
                //    MessageBox.Show(" YOU ARE NOT AUTHORISED TO REPLENISH THIS ATM ");
                //    return;
                //}

                if (WProcessMode == -1)
                {
                    MessageBox.Show("MSG668: Process Mode = -1 ... this means that not all information is available" + Environment.NewLine
                                + " for replenishement .. Supervisor Mode cassette data are missing ");
                    return;
                }

                if (WProcessMode == 1)
                {

                    if (MessageBox.Show("Process Mode = 1 ... This Atm already passed the Repl Workflow." + Environment.NewLine
                        + " Do you want to proceed to workflow?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                    {
                        // If Yes proceed .... 
                    }
                    else
                    {
                        return;
                    }

                }



                // SHOW When WAS LAST REPLENISHed
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                bool Panicos = false;
                if (Ta.PreSes == 0 & Panicos == true)
                {
                    MessageBox.Show("This is the first time for replenishming this ATM on RRDM" + Environment.NewLine
                        + "The Replenishment Cycle is not a complete one." + Environment.NewLine
                        + "For this reason RRDM System will void it." + Environment.NewLine
                        + "You should continue to the next complete one "
                        );

                    RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                    string WJobCategory = "ATMs";
                    int WReconcCycleNo;

                    WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                    RRDMAtmsClass Ac = new RRDMAtmsClass();
                    Ac.ReadAtm(WAtmNo);
                    int WAtmsReconcGroup = Ac.AtmsReconcGroup;

                    RRDMReconcCategories Rc = new RRDMReconcCategories();

                    Rc.ReadReconcCategorybyGroupId(WAtmsReconcGroup);

                    string WReconcCategoryId = Rc.CategoryId;
                    // UPDATE TRACES WITH FINISH 
                    // Update all fields and Reconciliation mode = 2 if all reconcile and Host files available 
                    int Mode = 1; // Before reconciliation 

                    // NBG CASE 
                    // UPDATE THAT RRDM REPLENISHMENT HAS FINISHED
                    // SET Ta Process Mode to 1 = ready for GL Reconciliation
                    // UPDATE Bank Record with counted inputed amount 

                    //Ta.UpdateTracesFinishRepl_From_Form51_NBG(WAtmNo, WSesNo, WSignedId, WReconcCategoryId);

                    Ta.UpdateTracesFinishRepl_From_Form152(WAtmNo, WSesNo,
                        WSignedId, WReconcCategoryId);

                    Form152_Load(this, new EventArgs());

                    dataGridViewMyATMS.Rows[WRow1].Selected = true;
                    dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
                    dataGridView2.Rows[WRow2].Selected = true;
                    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

                    return;

                }

                // Not the first replenishment

                string LastRepl = Ta.SM_LAST_CLEARED.ToString();

                if (MessageBox.Show("LAST REPLENISHMENT WAS DONE ON " + LastRepl + Environment.NewLine + Environment.NewLine
                    + " DO YOU WANT TO PROCEED WITH THIS ONE?"
                    , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                              == DialogResult.Yes)
                {
                    // Process No Updating 
                    RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);
                    // WFunction = 1 Normal branch ATM
                    // 25 Off site ATM = cassettes are ready and go in ATM
                    // 26 Belongs to external 
                    Ac.ReadAtm(WAtmNo);
                    Us.ReadUsersRecord(WSignedId);
                    if (Ac.OffSite == true & Us.UserType == "Employee")
                    {
                        Usi.ProcessNo = 25;
                    }
                    if ((Ac.CitId != "1000"))
                    {
                        Usi.ProcessNo = 26;
                    }
                    if (Ac.OffSite == false & Ac.CitId == "1000")
                    {
                        Usi.ProcessNo = 1; // NORMAL AT BRANCH
                    }

                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                    if (Usi.ReplStep1_Updated == false) // Start from beggining 
                    {
                        // ZeroData(WAtmNo, WSesNo);
                    }

                    if (Usi.ProcessNo == 30 || Usi.ProcessNo == 25)
                    {
                        MessageBox.Show("MSG667: Process codes 30 and 25 = off site ATMs note available in Form51 yet");
                    }

                    // Check if Outstanding Dispute
                    RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
                    RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

                    Dt.ReadDisputeTranByATMAndReplCycle(WAtmNo, WSesNo);
                    if (Dt.RecordFound == true)
                    {
                        if (Dt.ClosedDispute == true)
                        {
                            // OK 
                        }
                        else
                        {
                            if (Dt.DisputeActionId == 5)
                            {
                                // There is an open dispute 
                                MessageBox.Show("There is an open Dispute no.." + Dt.DisputeNumber.ToString()
                                    + " for this repl cycle" + Environment.NewLine
                                    + " The action taken on this Dispute is postponed" + Environment.NewLine
                                    + " Maybe you Settle or cancel the dispute to continue work." + Environment.NewLine
                                     + " You still can move to Replenishment workflow."
                                    );
                            }
                            else
                            {
                                // There is an open dispute 
                                MessageBox.Show("There is an open Dispute no.." + Dt.DisputeNumber.ToString()
                                    + " for this repl cycle" + Environment.NewLine
                                    + " Maybe you must Settle the dispute to continue work." + Environment.NewLine
                                    + " You still can move to Replenishment workflow."
                                    );
                            }

                            // return;
                        }
                        // OK 
                    }

                    //                ,[RecStartDtTm]
                    //,[RecFinDtTm]
                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                    Ta.Recon1.RecStartDtTm = DateTime.Now;
                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                    if (AudiType == true)
                    {
                        Form51_FAB_Type NForm51_AUDI_TYPE;
                        NForm51_AUDI_TYPE = new Form51_FAB_Type(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        NForm51_AUDI_TYPE.FormClosed += NForm51_FormClosed;
                        NForm51_AUDI_TYPE.ShowDialog();
                    }
                    else
                    {

                        if (IsRecycle == true)
                        {
                            
                            // CALL THE SAME If Recycle or not 
                            bool IsFromExcel = false;
                            Form51_Repl_For_IST NForm51_Repl_For_IST;
                            NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, IsFromExcel);
                            NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
                            NForm51_Repl_For_IST.ShowDialog();
                        }
                        else
                        {
                            bool IsFromExcel = false;
                            Form51_Repl_For_IST NForm51_Repl_For_IST;
                            NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, IsFromExcel);
                            NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
                            NForm51_Repl_For_IST.ShowDialog();
                        }
                        // Current Bank De Caire Type 

                    }


                    return;
                }
                else
                {
                    return;
                }

            }

            //if (IsRecycle == true)
            //{
            //    // Recycle Type 

            //    Form51_Test_For_IST NForm51_Test_For_IST;
            //    NForm51_Test_For_IST = new Form51_Test_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            //    NForm51_Test_For_IST.FormClosed += NForm51_FormClosed;
            //    NForm51_Test_For_IST.ShowDialog();
            //}
            //else
            //{
            //    // Current Bank De Caire Type 
            //    Form51_Test_For_IST NForm51_Test_For_IST;
            //    NForm51_Test_For_IST = new Form51_Test_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            //    NForm51_Test_For_IST.FormClosed += NForm51_FormClosed;
            //    NForm51_Test_For_IST.ShowDialog();
            //}
        }

        void NForm38_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form152_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRow1].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
        }

        void NForm26_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form152_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRow1].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
        }

        void NForm71_FormClosed(object sender, FormClosedEventArgs e)
        {

            Form152_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRow1].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));
        }

        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {

            int WRow1 = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition1 = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            //int WRow2 = dataGridView2.SelectedRows[0].Index;

            //int scrollPosition2 = dataGridView2.FirstDisplayedScrollingRowIndex;

            Form152_Load(this, new EventArgs());
            // First
            dataGridViewMyATMS.Rows[WRow1].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition1;

            // second

            //dataGridView2.Rows[WRow2].Selected = true;
            //dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

            //dataGridView2.FirstDisplayedScrollingRowIndex = scrollPosition2;

        }

        // Zero DATA METHOD 
        //   
        //
        private void ZeroData(string AtmNo, int SesNo)
        {
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 
            RRDMDepositsClass Da = new RRDMDepositsClass();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            RRDMSessionsPhysicalInspection Pi = new RRDMSessionsPhysicalInspection();

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            RRDMCaseNotes Cn = new RRDMCaseNotes();


            // UNDO Physical Inspection Data

            Pi.UpdateSessionsPhysicalInspectionRecord(WAtmNo, WSesNo, false);

            string WParameter4 = "Physical Ispection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;

            Cn.DeleteCaseNotesRecordByParameter4(WParameter4);


            // CASSETTES COUNT AND CAPTURED CARDS 
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.Cassettes_1.CasCount = 0;

            Na.Cassettes_1.RejCount = 0;

            Na.Cassettes_2.CasCount = 0;

            Na.Cassettes_2.RejCount = 0;

            Na.Cassettes_3.CasCount = 0;

            Na.Cassettes_3.RejCount = 0;

            Na.Cassettes_4.CasCount = 0;

            Na.Cassettes_4.RejCount = 0;


            // Captured Cards 

            Na.CaptCardsCount = 0;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            // DEPOSITS 

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            Da.DepositsCount1.Trans = 0;

            Da.DepositsCount1.Notes = 0;

            Da.DepositsCount1.Amount = 0;

            Da.DepositsCount1.NotesRej = 0;

            Da.DepositsCount1.AmountRej = 0;

            Da.DepositsCount1.Envelops = 0;

            Da.DepositsCount1.EnvAmount = 0;

            // CHEQUES
            //
            Da.ChequesCount1.Trans = 0;


            Da.ChequesCount1.Number = 0;

            Da.ChequesCount1.Amount = 0;

            Da.UpdateDepositsSessionsNotesAndValuesWithCount(WAtmNo, WSesNo); // UPDATE INPUT VALUES

            //     Replenishement 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.Cassettes_1.NewInUser = 0;

            Na.Cassettes_2.NewInUser = 0;

            Na.Cassettes_3.NewInUser = 0;

            Na.Cassettes_4.NewInUser = 0;

            // Update Notes balances with new in figures 

            Na.ReplMethod = 0;
            Na.InUserDate = new DateTime(2050, 11, 21);
            Na.InReplAmount = 0;

            Na.ReplAmountTotal = 0;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            Na.ReplUserComment = " ";

            Na.UpdateSessionsNotesAndValuesUserComment(WAtmNo, WSesNo);

            // DELETE ACTIONS 

            string WSelectionCriteria = " WHERE AtmNo ='" + WAtmNo
                                                          + "' AND ReplCycle =" + WSesNo;

            Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

            Aoc.ClearTableTxnsTableFromAction();

            int I = 0;

            while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
            {

                int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                string WActionId = (string)Aoc.TableActionOccurances_Big.Rows[I]["ActionId"];

                int WUniqueKey = (int)Aoc.TableActionOccurances_Big.Rows[I]["UniqueKey"];

                string WUniqueKeyOrigin = (string)Aoc.TableActionOccurances_Big.Rows[I]["UniqueKeyOrigin"];

                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueKeyOrigin, WUniqueKey, WActionId);

                if (WUniqueKeyOrigin == "Master_Pool")
                {
                    // THIS IS FOR PRESENTER ERROR
                    WSelectionCriteria = " WHERE UniqueRecordId =" + WUniqueKey;
                    Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2);

                    //Mpa.ActionByUser = true;
                    //Mpa.UserId = WSignedId;
                    //Mpa.Authoriser = Ap.Authoriser;
                    //Mpa.AuthoriserDtTm = DateTime.Now;

                    //Mpa.SettledRecord = true;

                    //WSelectionCriteria = " WHERE UniqueRecordId =" + WUniqueKey;
                    Mpa.UpdateMatchingTxnsMasterPoolATMsForcedMatched(WOperator, WSelectionCriteria, 1);

                }

                //********************************************
                // HERE WE CREATE THE ENTRIES AS PER BDC NEEDS
                //********************************************

                I = I + 1;
            }

        }

        // READ Errors AND UNDO  
        //
        private void UndoErrors(string InAtmNo, int InSesNo)
        {
            //RecordFound = false;

            int ErrNo;
            int ErrId;
            bool OpenErr;
            bool NeedAction;
            bool UnderAction;
            bool ManualAct;

            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

            string SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE AtmNo = @AtmNo AND SesNo = @SesNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            //RecordFound = true;

                            // Read error Details

                            ErrNo = (int)rdr["ErrNo"];
                            ErrId = (int)rdr["ErrId"];

                            NeedAction = (bool)rdr["NeedAction"];
                            UnderAction = (bool)rdr["UnderAction"];
                            ManualAct = (bool)rdr["ManualAct"];
                            OpenErr = (bool)rdr["OpenErr"];

                            if (ErrId < 200)
                            {
                                Ec.ReadErrorsTableSpecific(ErrNo);
                                Ec.OpenErr = true;
                                Ec.UnderAction = false;
                                Ec.ManualAct = false;
                                Ec.UpdateErrorsTableSpecific(ErrNo);
                            }

                            if (ErrId > 200) // Deposits 
                            {
                                Ec.ReadErrorsTableSpecific(ErrNo);
                                Ec.OpenErr = true;
                                Ec.UpdateErrorsTableSpecific(ErrNo);
                            }
                        }
                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();
                    //   MessageBox.Show(ex.ToString());
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }
        // Finish => go back to main 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        //
        // SHOW SM
        //

        private void linkLabelSMLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

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
        // Show UNMatched this Repl Cycle 
        private void linkLabelUnmatchedTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }
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
        }
        // Show Cycle Txns 
        private void linkLabelCycleTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateTmSesStart
                               , DateTmSesEnd, WSesNo, NullPastDate, 1);

            NForm78D_ATMRecords.Show();
        }

        private void linkLabelFromE_Journal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo
                                                    , DateTmSesStart, DateTmSesEnd, WSesNo, NullPastDate, 1);

            NForm78D_ATMRecords.Show();
        }
        // Show SM Details 
        private void linkLabelShowSMDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }
            int Mode = 7;
            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, "", WAtmNo, DateTmSesStart, DateTmSesEnd, WSesNo, NullPastDate, Mode);

            NForm78D_ATMRecords.ShowDialog();
        }
        // Show Rich Picture
        private void linkLabelRichPicture_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WProcessMode != 2)
            {
                MessageBox.Show("This is only available if Replenishment is completed");
                return;
            }
            string WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {

            }
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            Form67_BDC NForm67_BDC;

            int Mode = 3; // Show only for this ATM

            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                             , WReconcCycleNo, NullPastDate, NullPastDate, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();

        }
        // 
        private void linkLabelPresenter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            Form200JobCycles_Presenter_Repl NForm200JobCycles_Presenter_Repl;
            NForm200JobCycles_Presenter_Repl = new Form200JobCycles_Presenter_Repl(WOperator, WSignedId, WAtmNo, WSesNo);

            NForm200JobCycles_Presenter_Repl.ShowDialog();
        }
        // Refresh for date 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            int SavedRow1 = WRow1;
            int SavedRow2 = WRow2;
            Form152_Load(this, new EventArgs());
            if (SavedRow1 != 0)
            {
                dataGridViewMyATMS.Rows[SavedRow1].Selected = true;
                dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, SavedRow1));
                // dataGridView2.Rows[SavedRow2].Selected = true;
                // dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, SavedRow2));
            }

        }
        // Show transactions of the Flex or the COREBANKING 
        private void linkLabelCoreBanking_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (IsBankDeCaire == true)
            {
                // Call for Bank de Caire
                if (WSesNo == 0)
                {
                    MessageBox.Show("Nothing to show");
                    return;
                }

                string WTableId = "Flexcube";

                Form78d_ATMRecords NForm78D_ATMRecords;
                NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateTmSesStart
                                   , DateTmSesEnd, WSesNo, NullPastDate, 1);

                NForm78D_ATMRecords.Show();
            }
            else
            {
                // Call For Corebanking 
                if (WSesNo == 0)
                {
                    MessageBox.Show("Nothing to show");
                    return;
                }

                string WTableId = "COREBANKING";

                Form78d_ATMRecords NForm78D_ATMRecords;
                NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateTmSesStart
                                   , DateTmSesEnd, WSesNo, NullPastDate, 1);

                NForm78D_ATMRecords.Show();
            }
        }
        // Completed Cycle PlayBack 
        private void linkLabelCyclePlayback_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // RRDMGasParameters Gp = new RRDMGasParameters();
            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            bool AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;
                }
                else
                {
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }
            if (WProcessMode > 0)
            {
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // View only for replenishment already done  
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                //Ta.Stats1.NoOfCheques = 1
                RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (Ta.Stats1.NoOfCheques == 1)
                {

                    // CALL THE SAME If Recycle or not 
                    bool IsFromExcel = false;
                    Form51_Repl_For_IST NForm51_Repl_For_IST;
                    NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, IsFromExcel);
                    NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
                    NForm51_Repl_For_IST.ShowDialog();
                }
                else
                {

                        // Current Bank De Caire Type 
                        NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        NForm51.FormClosed += NForm51_FormClosed;
                        NForm51.ShowDialog();

                    

                }

            }
            else
            {
                MessageBox.Show("Not allowed operation. Repl Workflow not done yet");
            }
        }

        private void textBoxProcessMode_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        // NEXT TEST
        private void buttonNextTest_Click(object sender, EventArgs e)
        {
           
        }
        // FOREX DETAILS
        private void linkLabelForexDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateTmSesStart
                               , DateTmSesEnd, WSesNo, NullPastDate, 14);

            NForm78D_ATMRecords.Show();
        }
        // FOREX TOTALS 
        private void linkLabelForexTotals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateTmSesStart
                               , DateTmSesEnd, WSesNo, NullPastDate, 15);

            NForm78D_ATMRecords.Show();
        }

        private void linkLabel_IST_Playback_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Ta.Stats1.NoOfCheques = 1
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            if (Ta.Stats1.NoOfCheques == 1)
            {
                // CALL THE SAME If Recycle or not 
                bool IsFromExcel = false;
                Form51_Repl_For_IST NForm51_Repl_For_IST;
                NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, IsFromExcel);
                NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
                NForm51_Repl_For_IST.ShowDialog();
            }
            else
            {
                if (WProcessMode > 0)
                {
                    RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);
                    Usi.ProcessNo = 54; // View only for replenishment already done  
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                    // CALL THE SAME If Recycle or not 
                    bool IsFromExcel = false;
                    Form51_Repl_For_IST NForm51_Repl_For_IST;
                    NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, IsFromExcel);
                    // NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
                    NForm51_Repl_For_IST.ShowDialog();

                }
                else
                {
                    MessageBox.Show("Not allowed operation. Repl Workflow not done yet");
                }
            }

        }
// See details 
        private void linkLabel_SM_Cassettes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form51_SM_Cassettes NForm51_SM_Cassettes;
            NForm51_SM_Cassettes = new Form51_SM_Cassettes(WOperator,WSignedId, WAtmNo, WSesNo);
            NForm51_SM_Cassettes.Show();
            
        }
// OLD REPLENISHMENT
        private void linkLabel_OLDREPL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Find ATM Branch 

            RRDMAccountsClass Acc = new RRDMAccountsClass();
            Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(WOperator, "30", WAtmNo);

            if (Acc.RecordFound == true)
            {

                if (Acc.BranchId == "015" || Acc.BranchId == "0001")
                {
                    // OK TO PROCEED
                }
                else
                {
                    MessageBox.Show("This ATM doesn't belong to branch 015");
                    return;
                }
            }
            //
            // Find out if ATM is Recycling 
            //
            IsRecycle = false;

            string ParId2 = "948";
            string OccurId2 = "1"; // 
            //RRDMGasParameters Gp = new RRDMGasParameters(); 
            Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();
                SM.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                if (SM.RecordFound == true)
                {
                    // Check if Reccyle 
                    if (SM.is_recycle == "Y")
                    {
                        IsRecycle = true;
                    }
                }
            }

            if (AudiType == true)
            {
                if (WExcelStatus == 5)
                {
                    // OK to be done manually
                }
                else
                {
                    if (MessageBox.Show("This Cycle is not defined as ready for Manual input " + Environment.NewLine
                     + "It has status : " + textBoxExcelStatus.Text + Environment.NewLine
                     + "Do you want to Proceed? " + Environment.NewLine
                     , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        // YES Proceed

                    }
                    else
                    {
                        // Stop 
                        return;
                    }

                }
            }

            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            //RRDMGasParameters Gp = new RRDMGasParameters();
            //bool AudiType = false;
            //int IsAmountOneZero;
            //Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
            //if (Gp.RecordFound == true)
            //{
            //    IsAmountOneZero = (int)Gp.Amount;

            //    if (IsAmountOneZero == 1)
            //    {
            //        // Transactions will be done at the end 
            //        AudiType = true;


            //    }
            //    else
            //    {
            //        AudiType = false;
            //    }
            //}
            //else
            //{
            //    AudiType = false;
            //}


            //RRDMGasParameters Gp = new RRDMGasParameters();
            // Check if ATM is set to Hybrid Branch Replenishment / Reconciliation 
            string ParamId;
            string OccurId;
            ParamId = "823";
            string OccuranceId = "01"; // Short
            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);
            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // HYBRID IS ACCEPTED
                    //Ac.ReadAtm(WAtmNo);
                    //if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
                    //{
                    //    MessageBox.Show("Not allowed Operation"
                    //        + "With Current functionality ATM must belong to a CIT."
                    //        );
                    //    return;
                    //}
                }
                else
                {
                    // Hybrid Repl and Reconciliation not accepted
                    Ac.ReadAtm(WAtmNo);
                    if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
                    {
                        MessageBox.Show("Not allowed Operation"
                            + "With Current functionality ATM must belong to a CIT."
                            );
                        return;
                    }
                }
            }
            else
            {
                // Hybrid Repl and Reconciliation not accepted
                Ac.ReadAtm(WAtmNo);
                if ((Ac.CitId == "1000" & Ac.AtmsReplGroup == 0))
                {
                    MessageBox.Show("Not allowed Operation"
                        + "With Current functionality ATM must belong to a CIT "
                        );
                    return;
                }
            }


            Am.ReadAtmsMainSpecific(WAtmNo);
            if (Am.ProcessMode == -2)
            {
                MessageBox.Show("This ATm is not active!");
                return;
            }
            //Keep Row Selection positioning 
            WRow1 = dataGridViewMyATMS.SelectedRows[0].Index;
            if (dataGridView2.Rows.Count > 0)
            {
                WRow2 = dataGridView2.SelectedRows[0].Index;
            }
            else
            {
                MessageBox.Show("No Replenishment Cycle Available");
                return;
            }



            // UPDATE TRANSACTIONS WITH REPL CYCLE BASED ON DATES

            Mpa.UpdateMpaRecordsWithReplCycle(WOperator, WSignedId
                                          , WAtmNo, DateTmSesStart, DateTmSesEnd
                                                     , WSesNo, 2);
            // Check if outstanding 
            string SelectionCriteria2 = " WHERE Operator ='" + WOperator + "'"
                          + " AND  TerminalId ='" + WAtmNo + "'"
                         + "  AND IsMatchingDone = 1 "
                         + "  AND Matched = 0 "
                         + "  AND SettledRecord = 0 " // Not Settled missmatched for this cycle
                         + " And ReplCycleNo =" + WSesNo;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria2, 1);

            if (Mpa.RecordFound == true)
            {
                if (MessageBox.Show("Warning: There are outstanding Unmatched. Do you want to proceed?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
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

            // UPDATE ERRORS WITH REPL CYCLE BASED ON DATES
            Err.UpdateErrorsWithReplCycleNo(WAtmNo, DateTmSesStart, DateTmSesEnd, WSesNo);

            // REPLENISHMENT PROCESS CODES
            // SINGLES
            // WFunction = 1 Normal Replenishment branch ATM
            // 25 Off site ATM = cassettes are ready and go in ATM
            // 26 Belongs to external - CIT 
            // GROUPS
            // 5 Normal Group belonging to Bank . 
            // 30 Offsite Group belonging to Bank
            // 31 Group belonging to - CIT 

            if (WAction == 1 // Normal from Branch
                || Recon_Equal_Repl == true & Usi.SecLevel == "03" // From Centralised Reconciliation
                ) // REPLENISH NO GROUP 
            {

                // Check LAST RECORD if Already in authorization process

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

                if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
                {
                    MessageBox.Show("This Replenishment Already has authorization record!" + Environment.NewLine
                                             + "Go to Pending Authorisations process to complete."
                                                              );
                    return;
                }

                //if (WSesNo != WNextReplNo)
                //{
                //    MessageBox.Show("This Repl Cycle is not ready. Select : " + WNextReplNo);
                //    return;
                //}
                // User Does Not Have Groups 

                // 

                Ac.ReadAtm(WAtmNo);
                //  CASH MANAGEMENT from RRDM during ATMs in NEED Process 
                string ParId = "202";
                OccurId = "1";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                string RRDMCashManagement = Gp.OccuranceNm;
                //  CASH MANAGEMENT Prior Replenishment Workflow  
                ParId = "211";
                OccurId = "1";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                string CashEstPriorReplen = Gp.OccuranceNm; // IF YES THEN IS PRIOR THOUGHT AN ACTION 

                if (Ac.CitId == "1000" & (RRDMCashManagement == "YES" & CashEstPriorReplen == "YES"))
                {
                    //Check that ACTion for money in was taken
                    //
                    if (WAtmNo == "AB104")
                    {
                        ARa.ReadReplActionsForAtmReplCycleNo(WAtmNo, 9051);
                    }
                    else ARa.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSesNo);

                    if (ARa.RecordFound & ARa.AuthorisedRecord == true)
                    {
                        // ACTION WAS CREATED AND IT IS CONFIRMED
                    }
                    else
                    {
                        if (ARa.RecordFound == false)
                        {

                            MessageBox.Show("No Action Record For Money to replenish created. " + Environment.NewLine
                                       + "Create it please.");

                            return;
                        }
                        if (ARa.RecordFound == true & ARa.AuthorisedRecord == false)
                        {

                            MessageBox.Show("Created Action for Loading Money Not Confirmed. " + Environment.NewLine
                                       + "Confirm it please.");

                            return;
                        }

                    }
                }

                //Ua.ReadUsersAccessAtmTableSpecific(WSignedId, WAtmNo, 0);

                //if (Ua.RecordFound == false || (Ua.RecordFound == true & Ua.Replenishment == false))
                //{
                //    MessageBox.Show(" YOU ARE NOT AUTHORISED TO REPLENISH THIS ATM ");
                //    return;
                //}

                if (WProcessMode == -1)
                {
                    MessageBox.Show("MSG668: Process Mode = -1 ... this means that not all information is available" + Environment.NewLine
                                + " for replenishement .. Supervisor Mode cassette data are missing ");
                    return;
                }

                if (WProcessMode == 1)
                {

                    if (MessageBox.Show("Process Mode = 1 ... This Atm already passed the Repl Workflow." + Environment.NewLine
                        + " Do you want to proceed to workflow?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                    {
                        // If Yes proceed .... 
                    }
                    else
                    {
                        return;
                    }

                }



                // SHOW When WAS LAST REPLENISHed
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                bool Panicos = false;
                if (Ta.PreSes == 0 & Panicos == true)
                {
                    MessageBox.Show("This is the first time for replenishming this ATM on RRDM" + Environment.NewLine
                        + "The Replenishment Cycle is not a complete one." + Environment.NewLine
                        + "For this reason RRDM System will void it." + Environment.NewLine
                        + "You should continue to the next complete one "
                        );

                    RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                    string WJobCategory = "ATMs";
                    int WReconcCycleNo;

                    WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                    RRDMAtmsClass Ac = new RRDMAtmsClass();
                    Ac.ReadAtm(WAtmNo);
                    int WAtmsReconcGroup = Ac.AtmsReconcGroup;

                    RRDMReconcCategories Rc = new RRDMReconcCategories();

                    Rc.ReadReconcCategorybyGroupId(WAtmsReconcGroup);

                    string WReconcCategoryId = Rc.CategoryId;
                    // UPDATE TRACES WITH FINISH 
                    // Update all fields and Reconciliation mode = 2 if all reconcile and Host files available 
                    int Mode = 1; // Before reconciliation 

                    // NBG CASE 
                    // UPDATE THAT RRDM REPLENISHMENT HAS FINISHED
                    // SET Ta Process Mode to 1 = ready for GL Reconciliation
                    // UPDATE Bank Record with counted inputed amount 

                    //Ta.UpdateTracesFinishRepl_From_Form51_NBG(WAtmNo, WSesNo, WSignedId, WReconcCategoryId);

                    Ta.UpdateTracesFinishRepl_From_Form152(WAtmNo, WSesNo,
                        WSignedId, WReconcCategoryId);

                    Form152_Load(this, new EventArgs());

                    dataGridViewMyATMS.Rows[WRow1].Selected = true;
                    dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
                    dataGridView2.Rows[WRow2].Selected = true;
                    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

                    return;

                }

                // Not the first replenishment

                string LastRepl = Ta.SM_LAST_CLEARED.ToString();

                if (MessageBox.Show("LAST REPLENISHMENT WAS DONE ON " + LastRepl + Environment.NewLine + Environment.NewLine
                    + " DO YOU WANT TO PROCEED WITH THIS ONE?"
                    , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                              == DialogResult.Yes)
                {
                    // Process No Updating 
                    RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);
                    // WFunction = 1 Normal branch ATM
                    // 25 Off site ATM = cassettes are ready and go in ATM
                    // 26 Belongs to external 
                    Ac.ReadAtm(WAtmNo);
                    Us.ReadUsersRecord(WSignedId);
                    if (Ac.OffSite == true & Us.UserType == "Employee")
                    {
                        Usi.ProcessNo = 25;
                    }
                    if ((Ac.CitId != "1000"))
                    {
                        Usi.ProcessNo = 26;
                    }
                    if (Ac.OffSite == false & Ac.CitId == "1000")
                    {
                        Usi.ProcessNo = 1; // NORMAL AT BRANCH
                    }

                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                    if (Usi.ReplStep1_Updated == false) // Start from beggining 
                    {
                        // ZeroData(WAtmNo, WSesNo);
                    }

                    if (Usi.ProcessNo == 30 || Usi.ProcessNo == 25)
                    {
                        MessageBox.Show("MSG667: Process codes 30 and 25 = off site ATMs note available in Form51 yet");
                    }

                    // Check if Outstanding Dispute
                    RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
                    RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

                    Dt.ReadDisputeTranByATMAndReplCycle(WAtmNo, WSesNo);
                    if (Dt.RecordFound == true)
                    {
                        if (Dt.ClosedDispute == true)
                        {
                            // OK 
                        }
                        else
                        {
                            if (Dt.DisputeActionId == 5)
                            {
                                // There is an open dispute 
                                MessageBox.Show("There is an open Dispute no.." + Dt.DisputeNumber.ToString()
                                    + " for this repl cycle" + Environment.NewLine
                                    + " The action taken on this Dispute is postponed" + Environment.NewLine
                                    + " Maybe you Settle or cancel the dispute to continue work." + Environment.NewLine
                                     + " You still can move to Replenishment workflow."
                                    );
                            }
                            else
                            {
                                // There is an open dispute 
                                MessageBox.Show("There is an open Dispute no.." + Dt.DisputeNumber.ToString()
                                    + " for this repl cycle" + Environment.NewLine
                                    + " Maybe you must Settle the dispute to continue work." + Environment.NewLine
                                    + " You still can move to Replenishment workflow."
                                    );
                            }

                            // return;
                        }
                        // OK 
                    }

                    //                ,[RecStartDtTm]
                    //,[RecFinDtTm]
                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                    Ta.Recon1.RecStartDtTm = DateTime.Now;
                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                    if (AudiType == true)
                    {
                        Form51_FAB_Type NForm51_AUDI_TYPE;
                        NForm51_AUDI_TYPE = new Form51_FAB_Type(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        NForm51_AUDI_TYPE.FormClosed += NForm51_FormClosed;
                        NForm51_AUDI_TYPE.ShowDialog();
                    }
                    else
                    {
                        if (IsRecycle == true)
                        {
                            // Recycle Type 
                            Form51_Recycle NForm51_Recycle;
                            NForm51_Recycle = new Form51_Recycle(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                            NForm51_Recycle.FormClosed += NForm51_FormClosed;
                            NForm51_Recycle.ShowDialog();
                        }
                        else
                        {
                            // Current Bank De Caire Type 
                            NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                            NForm51.FormClosed += NForm51_FormClosed;
                            NForm51.ShowDialog();
                        }

                    }

                    return;
                }
                else
                {
                    return;
                }

            }
            //
            // REplenishment for Group
            //
            if (WAction == 5)
            {
                // User Have Group/s 

                Aa.ReadAtm(WAtmNo); // Read ATM

                Ua.ReadUsersAccessAtmTableSpecific(WSignedId, "", Aa.AtmsReplGroup);

                if (Ua.RecordFound == false)
                {
                    MessageBox.Show(" YOU ARE NOT AUTHORISED TO REPLENISH THIS ATM ");
                    return;
                }

                // SHOW When WAS LAST REPLENISHed

                Am.ReadAtmsMainSpecific(WAtmNo);

                if (Am.NextReplDt.Date > DateTime.Today)
                {

                    if (MessageBox.Show("NEXT REPLENISHMENT DATE IS GREATER THAN TODAY. Do you want to proceed with Replenishment?"
                        , "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                  == DialogResult.Yes)
                    {
                        // Process No Updating 

                        string LastRepl = Am.LastReplDt.ToString();

                        if (MessageBox.Show("LAST REPLENISHMENT WAS DONE ON " + LastRepl + " DO YOU WANT TO PROCEED WITH THIS ONE?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                      == DialogResult.Yes)
                        {
                            // Process No Updating 
                            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                            Usi.ReadSignedActivityByKey(WSignRecordNo);
                            // GROUPS
                            // 5 Normal Group belonging to Bank . 
                            // 30 Offsite Group belonging to Bank  ????? 
                            // 31 Group belonging to external like GROUP 4 OR CitNo>0
                            Ac.ReadAtm(WAtmNo);
                            Us.ReadUsersRecord(WSignedId);
                            if (Ac.OffSite == true & Ac.CitId == "1000")
                            {
                                Usi.ProcessNo = 30; // ?????? 
                            }
                            if ((Ac.CitId != "1000"))
                            {
                                Usi.ProcessNo = 31;
                            }
                            if (Ac.OffSite == false & Ac.CitId == "1000")
                            {
                                Usi.ProcessNo = 5; // NORMAL AT BRANCH
                            }
                            // 5 for internal group and 30 for OffSite ATM and 31 for external 

                            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                            if (Usi.ProcessNo == 30 || Usi.ProcessNo == 25)
                            {
                                MessageBox.Show("MSG667: Process codes 30 and 25 = off-site ATMs note available in Form51 yet");
                            }

                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                            if (WProcessMode == -1)
                            {
                                MessageBox.Show("MSG670: Process Mode = 1 ... this means that not all information is available"
                                            + " for replenishement .. Supervisor Mode cassette data are missing ");
                                return;
                            }

                            Ta.FindNextAndLastReplCycleId(WAtmNo);
                            if (WProcessMode == 0 & WSesNo != Ta.NextReplNo)
                            {
                                MessageBox.Show("MSG671: Choose the right Repl Number. Choose the : " + Ta.NextReplNo.ToString());
                                return;
                            }

                            Ta.FindNextAndLastReplCycleId(WAtmNo);
                            if (WProcessMode == 0 & WSesNo != Ta.NextReplNo)
                            {
                                MessageBox.Show("MSG672: Choose the right Repl Number. Choose the : " + Ta.NextReplNo.ToString());
                                return;
                            }

                            if (AudiType == true)
                            {
                                Form51_FAB_Type NForm51_AUDI_TYPE;
                                NForm51_AUDI_TYPE = new Form51_FAB_Type(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                                NForm51_AUDI_TYPE.FormClosed += NForm51_FormClosed;
                                NForm51_AUDI_TYPE.ShowDialog();
                            }
                            else
                            {
                                NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                                NForm51.FormClosed += NForm51_FormClosed;
                                NForm51.ShowDialog();
                            }


                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    // Not Greated than today 
                    string LastRepl = Am.LastReplDt.ToString();

                    if (MessageBox.Show("LAST REPLENISHMENT WAS DONE ON " + LastRepl + " DO YOU WANT TO PROCEED WITH THIS ONE?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                  == DialogResult.Yes)
                    {
                        // Process No Updating 
                        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        // GROUPS
                        // 5 Normal Group belonging to Bank . 
                        // 30 Offsite Group belonging to Bank
                        // 31 Group belonging to external like GROUP 4 
                        Ac.ReadAtm(WAtmNo);

                        Us.ReadUsersRecord(WSignedId); // Find CIT Company of User 
                        Us.ReadUsersRecord(Us.CitId); // Read the details of CIT Company 

                        if (Ac.OffSite == true & Us.UserType == "Operator Entity") // Our Own User but ATM is Offsite 
                        {
                            Usi.ProcessNo = 30;
                        }
                        if ((Ac.OffSite == true & Us.UserType == "CIT Company") || (Ac.OffSite == false & Us.UserType == "CIT Company"))
                        {
                            Usi.ProcessNo = 31;
                        }
                        if (Ac.OffSite == false & Us.UserType == "Operator Entity") // Owr own User and ATM is at the Baranch 
                        {
                            Usi.ProcessNo = 5; // NORMAL AT BRANCH
                        }
                        // 5 for internal group and 30 for OffSite ATM and 31 for external 

                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


                        if (WProcessMode == -1)
                        {
                            MessageBox.Show("MSG675: Process Mode = 1 ... this means that not all information is available"
                                        + " for replenishement .. Supervisor Mode cassette data are missing ");
                            return;
                        }


                        Ta.FindNextAndLastReplCycleId(WAtmNo);
                        if (WProcessMode == 0 & WSesNo != Ta.NextReplNo)
                        {
                            MessageBox.Show("MSG676: Choose the right Repl Number. Choose the : " + Ta.NextReplNo.ToString());
                            return;
                        }

                        if (AudiType == true)
                        {
                            // CHECK If IT HAS PASSED FROM G4 = Loading Amt and Unloading Amt 
                            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
                            G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 1);
                            if (G4.RecordFound == true)
                            {
                                if (G4.ProcessMode_Load == 2 & G4.ProcessMode_UnLoad == 2)
                                {
                                    // Fully CIT Loaded Both Loaded and unload Done
                                    // textBoxReplStatus.Text
                                }
                                else
                                {
                                    // Partially Loaded 
                                }
                            }
                            {
                                // Excel not loaded Yet 
                            }
                            Form51_FAB_Type NForm51_AUDI_TYPE;
                            NForm51_AUDI_TYPE = new Form51_FAB_Type(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                            NForm51_AUDI_TYPE.FormClosed += NForm51_FormClosed;
                            NForm51_AUDI_TYPE.ShowDialog();
                        }
                        else
                        {
                            NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                            NForm51.FormClosed += NForm51_FormClosed;
                            NForm51.ShowDialog();
                        }


                    }
                    else
                    {
                        return;
                    }
                }


            }



            //
            // GO TO UCForm51d to calculate amount to be replenished 
            //

            if (WAction == 8)
            {
                // Process No Updating
                //
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ProcessNo = 8; // Go to calculate money IN

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Am.ReadAtmsMainSpecific(WAtmNo);


                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                if (WSesNo != Am.CurrentSesNo)
                {
                    MessageBox.Show("MSG679: Choose the right Replenishment Cycle please! This is: " + Am.CurrentSesNo.ToString());

                    return;
                }

                if (AudiType == true)
                {
                    Form51_FAB_Type NForm51_AUDI_TYPE;
                    NForm51_AUDI_TYPE = new Form51_FAB_Type(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    NForm51_AUDI_TYPE.FormClosed += NForm51_FormClosed;
                    NForm51_AUDI_TYPE.ShowDialog();
                }
                else
                {
                    NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    NForm51.FormClosed += NForm51_FormClosed;
                    NForm51.ShowDialog();
                }

            }

            // RECONCILIATION for not Groups
            //
            if (WAction == 2)
            {

                // Check LAST RECORD if Already in authorization process

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

                if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
                {
                    MessageBox.Show("This Reconciliation Already has authorization record!" + Environment.NewLine
                                             + "Go to Pending Authorisations process to complete."
                                                              );
                    return;
                }

                //TEST
                if (WSignedId == "1005")
                {
                    if (WAtmNo == "AB104")
                    {
                        WAtmNo = "AB104";
                        //    CurrentSessionNo = int.Parse(textBox2.Text);
                    }

                    if (WAtmNo == "AB102")
                    {
                        //   CurrentSessionNo = 3144;
                        WSesNo = 3144;
                    }

                }

                if (WSignedId == "500")
                {
                    WAtmNo = "12507";
                    //    CurrentSessionNo = 1122;
                    WSesNo = 1122;
                }

                if (WSignedId == "03ServeUk")
                {
                    if (WAtmNo == "ServeUk102")
                    {

                        WSesNo = 6694;
                    }

                }
                if (WSignedId == "03ServeUk")
                {
                    if (WAtmNo == "ABC501")
                    {

                        WSesNo = 6695;
                    }

                }
                Ua.ReadUsersAccessAtmTableSpecific(WSignedId, WAtmNo, 0);

                if (Ua.RecordFound == false || (Ua.RecordFound == true & Ua.Reconciliation == false))
                {
                    MessageBox.Show(" YOU ARE NOT AUTHORISED TO RECONCILE THIS ATM ");
                    return;
                }
                // UPDATE INTENTED FUNCTION 
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ProcessNo = 2;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                if (WProcessMode == -1)
                {
                    MessageBox.Show("MSG680: This Repl Cycle is not ready for reconciliation.");
                    return;
                }

                if (WProcessMode == 2 || WProcessMode == 3)
                {

                    if (MessageBox.Show("Process Mode = 2 or 3 ... Atm Has already had passed the Reconciliation Workflow. Do you want to proceed ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                    {
                        // If Yes proceed .... 
                    }
                    else
                    {
                        return;
                    }
                }

                if (WProcessMode <= 1)
                {
                    Ta.FindNextAndLastReplCycleId(WAtmNo);
                    if (WSesNo != Ta.Last_1 & Ta.Last_1 > 0)
                    {
                        MessageBox.Show("MSG681: Choose the right Repl Number. Choose the : " + Ta.Last_1.ToString());
                        return;
                    }
                    else
                    {
                        if (Ta.Last_1 == 0)
                        {
                            MessageBox.Show("MSG682: ATM not ready for Reconciliation");
                            return;
                        }
                    }
                }

                NForm71 = new Form71(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm71.FormClosed += NForm71_FormClosed;
                NForm71.ShowDialog(); ;
            }
            //  GOTO MANAGE CAPTURED CARDS
            if (WAction == 3)
            {

                //TEST
                if (WSignedId == "1005")
                {
                    // WAtmNo = "AB102";
                    // CurrentSessionNo = 3144;
                    //  WSesNo = 3144;
                }

                if (WSignedId == "500")
                {
                    WAtmNo = "12507";
                    //      CurrentSessionNo = 1122;
                    WSesNo = 1122;
                }

                Ua.ReadUsersAccessAtmTableSpecific(WSignedId, WAtmNo, 0);

                if (Ua.RecordFound == false)
                {
                    MessageBox.Show(" You are not authorised for this ATM ");
                    return;
                }

                //CAPTURED CARDS 
                //Form26(string InSignedId, string InOperator, string InInputText
                //         , DateTime InDateTmFrom
                //         , DateTime InDateTmTo
                //         , int InMode
                //         )
                //NForm26 = new Form26(WSignedId, WOperator, WAtmNo, WSesNo);
                //NForm26.FormClosed += NForm26_FormClosed;
                //NForm26.ShowDialog(); ;

            }
            //
            //
            //  GOTO MANAGE DEPOSITS FOR A PARTICULAR REPL CYCLE
            // 
            if (WAction == 4)
            {

                if (WSignedId == "1005")
                {
                    /*
                    WAtmNo = "AB102";
                    CurrentSessionNo = 3144;
                    WSesNo = 3144;
                     */
                }

                if (WSignedId == "500")
                {
                    WAtmNo = "12507";
                    //     CurrentSessionNo = 1122;
                    WSesNo = 1122;
                }

                //
                // Check if USER is authorised for this ATM
                //
                Ua.ReadUsersAccessAtmTableSpecific(WSignedId, WAtmNo, 0);

                if (Ua.RecordFound == false || (Ua.RecordFound == true & Ua.Reconciliation == false))
                {
                    if (Ua.Reconciliation == false)
                    {
                        MessageBox.Show(" You are not authorised to Reconcile this ATM ");
                    }
                    MessageBox.Show(" You are not authorised for this ATM ");
                    return;
                }
                // DEPOSITS 

                Form51_CDM NForm51_CDM;

                NForm51_CDM = new Form51_CDM(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm51_CDM.FormClosed += NForm38_FormClosed;
                NForm51_CDM.ShowDialog();

                //NForm38 = new Form38(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                //NForm38.FormClosed += NForm38_FormClosed;
                //NForm38.ShowDialog(); 

            }
        }
    }
}

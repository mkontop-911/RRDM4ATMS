using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form50b : Form
    {

        Form51 NForm51; // Goto recommend money 

        int Tot_51;
        int Tot_52;
        int Tot_53;
        int Tot_54;

        int WRowIndex;

        Decimal FinalCorrectedReplTotal;

        bool FinishPressed;
        bool OutstandingFoundNoAction;
        bool OutstandingFoundNoConfirm;

        DateTime WEstReplDt; // NEXT REPLENISHMENT DATE

        DateTime WDTm = new DateTime(2014, 02, 28);

        decimal TotalRepl;

        int WorkingDaysTotal;

        int WNextReplNumber; 

        bool WorkingDay;
        bool Weekend;
        bool Holiday;

        //int WNextReplNo ;

        int I;
        int J;

        int InGroup;
        int WActionNo;

        //int WReplNumber; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;


        string Tablefilter;

        bool FirstCycle; 

        // DATATable for Grid 
        DataTable GridDays = new DataTable();
        //     DataTable dtAtmsMain = new DataTable();
        //    SqlDataAdapter daAtmsMain;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass(); // ATM MAIN CLASS TO UPDATE NEXT REPLENISHMENT DATE

        RRDMReplDatesCalc Rc = new RRDMReplDatesCalc(); // Locate next Replenishment 

        RRDMDepositsClass Dc = new RRDMDepositsClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMReplOrdersClass Ra = new RRDMReplOrdersClass();

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMFixedDaysReplAtmClass Fr = new RRDMFixedDaysReplAtmClass();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMDepositsClass Da = new RRDMDepositsClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        string WAtmNo;
        int WCurrentSesNo;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WFunction;

        public Form50b(string InSignedId, int SignRecordNo, string InOperator, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction;
            // InFunction = "PrepareMoneyIn" ... means ATM operator at Branch needs to move to Replenishment process
            // InFunction == "ATMsInNeed" .... means call came from ATMs in Need button ..........

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

            textBoxMsgBoard.Text = "Review data and take necessary action for each individual ATM";

            if (WFunction == "PrepareMoneyIn")
            {
                labelStep1.Text = "Define Money(Notes) to Load";
                textBox54.Text = "ATMs To Calculate Money In";

                textBoxReplOwner.Text = "Repl will be done by Branch personnel.";

                textBoxNeedReason.Text = "Scheduled ATM Replenishment";
            }
            FirstCycle = true; 
            //
            // If Call comes from ATMs In Need : ***********************************// 
            // Actions will be taken for all ATMs in need 
            // Read all ATMs you are allowed one by one and see if In Need
            // If In Need and Maximum say then create action. 
            // If In need and Maximum = 0 and "As Per RRDM sugest" then promt user to go and calculate and actions are created
            // ALL Actions created will be dealt by process (Button) "Actions Management"
            //
            // 
            // If Call comes from the replenishment process then: ******************//
            // BOC MODEL .... DAILY REPLENISHMENT 
            // USER Calculates money => Puts money in ATM and then does the process of RRDM Replenishment 
            // ACTIONS WILL BE TAKEN FOR ALL ATMS TO REPLENISH
            // Create actions as before.
            // Confirm them and print ticket. 
            // No need to to go butoon "Actions Management" 
            // 
            // Before you move to these actions you should force reading journal 
            // ATMs Main Record is updated all the way 
            //
        }
        // ON LOAD 
        int WTableSize; 
        private void Form50b_Load(object sender, EventArgs e)
        {
            OutstandingFoundNoAction = false;
            OutstandingFoundNoConfirm = false;
            //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE AND Use ---------------//
            //
            //LOAD TABLE 
            //MY ATMS if coming from pre-Repl
            //ATMs In Need if comes from need button
            //
            // Create table with the ATMs this user can access
            // Create table with the ATMs this user can access
            bool IncludeAction = true;
            Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, "", WFunction, "");

            //-----------------UPDATE LATEST TRANSACTIONS----------------------//
            // Update latest transactions from Journal 
            Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);

            //-------------------

            WTableSize = Am.TableATMsMainSelected.Rows.Count;

            textBoxTotalAtms.Text = WTableSize.ToString();
            //
            // Find ATMs in need
            //
            FindAtmsInNeed(WTableSize);  // AT THIS POINT ACTIONS ARE CREATED

            //
            //
            dataGridView1.DataSource = Am.TableATMsMainSelected.DefaultView;

            if (dataGridView1.Rows.Count == 0 & WFunction == "PrepareMoneyIn")
            {
                Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }
            if (dataGridView1.Rows.Count == 0 & WFunction == "ATMsInNeed")
            {
                Form2 MessageForm = new Form2("There are no ATMs in Need.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            dataGridView1.Columns[0].Width = 70; // AtmNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; // ReplCycle
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 120; // AtmName

            dataGridView1.Columns[3].Width = 130; // RespBranch

            dataGridView1.Columns[4].Width = 70; // Auth User 

            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            // InFunction = "PrepareMoneyIn" ... means ATM operator at Branch needs to move to Replenishment process
            // InFunction == "ATMsInNeed" .... means call came from ATMs in Need button ..........



        }
        // ON ROW ENTER 
        //bool Internal = false; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            buttonInMoney.Show();

            WAtmNo = (string)rowSelected.Cells[0].Value;

            textBoxAtmNo.Text = WAtmNo;

            Am.ReadAtmsMainSpecific(WAtmNo);

            textBoxNeedReason.Text = "";

            textBoxRecomAction.Text = "";

            //Ra.ReadReplActionsForSpecificDate(WAtmNo, DateTime.Today);

            //
            // Check if this is the next for replenishment
            //
            Ta.FindNextAndLastReplCycleId(WAtmNo);

            textBoxSesNo.Text = Ta.NextReplNo.ToString();

            WNextReplNumber = Ta.NextReplNo; 

            Ra.ReadReplActionsForSpecificReplCycle(WAtmNo, Ta.NextReplNo);

            if (Ra.RecordFound == true & Am.ActionConfirmed == true)
            {            
                DisplayPanelCassettesAndOverridde();
                pictureBoxConfirmed.Show();
            }
            else
            {               
                DisplayPanelCassettesAndOverridde();
                pictureBoxConfirmed.Hide();
            }

            FirstCycle = false; 

            //WSesNo = Am.ReplCycleNo;

            WCurrentSesNo = Am.CurrentSesNo;

            Ac.ReadAtm(WAtmNo);

            textBoxOutstandingErrors.Text = Am.ErrOutstanding.ToString();

            //Na.ReadSessionsNotesAndValues(WAtmNo, Am.CurrentSesNo, 2);
            if (WCurrentSesNo > 0)
            {
                Na.ReadSessionsNotesAndValues(WAtmNo, WCurrentSesNo, 2);
                textBoxCapturedCards.Text = Na.CaptCardsMachine.ToString();
                textBoxCassettesAmnt.Text = Na.Balances1.ReplToRepl.ToString("#,##0.00");

                Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WCurrentSesNo);

                textBoxDepositsBNAMachine.Text = (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej + Da.DepositsMachine1.EnvAmount
                            + Da.ChequesMachine1.Amount).ToString("#,##0.00");

            }
            else
            {
                MessageBox.Show("This ATM has no intiated session yet.");
            }


            //      WOperator = Ac.BankId;

            // InFunction = "PrepareMoneyIn" ... means ATM operator at Branch needs to move to Replenishment process
            // InFunction == "ATMsInNeed" .... means call came from ATMs in Need button ..........

            if (WFunction == "ATMsInNeed")
            {
                if (Ac.ActiveAtm == false) textBoxNeedReason.Text = " THIS ATM is not ACTIVE YET ";

                if (Am.NeedType == 10)
                {
                    textBoxNeedReason.Text = " THIS ATM is not in NEED ";
                }
                if (Am.NeedType == 11)
                {
                    textBoxNeedReason.Text = " Replenishment Has been delayed ";
                }
                if (Am.NeedType == 12)
                {
                    textBoxNeedReason.Text = " Replenish now because of low balance ";
                }
                if (Am.NeedType == 13)
                {
                    textBoxNeedReason.Text = "This ATM is in Need ";
                }
                if (Am.NeedType == 13 & Ac.CashInType == "At Defined Maximum" & Ra.RecordFound == true)
                {
                    if (Ac.CitId == "1000") // EMBORIKI MODEL 
                    {
                        textBoxNeedReason.Text = "Inform Bank ATM Operator to replenish ";

                        textBoxRecomAction.Text = " A default Replenishment Action with Id : " + Ra.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                                       + " Next Replenishment : " + Ra.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                                       + " Amount for Replenihsment : " + Ra.NewAmount.ToString("#,##0.00") + Environment.NewLine
                                       + " An Alerting email was sent to ATM owner at this email:  " + Environment.NewLine
                                       + " " + Ac.AtmReplUserEmail;
                    }
                    if (Ac.CitId != "1000" & Ac.CitId != "") // JCC MODEL 
                    {
                        textBoxNeedReason.Text = " Inform CIT " + Ac.CitId + " to replenish ";

                        textBoxRecomAction.Text = " A default Replenishment Action with Id : " + Ra.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                            + " Next Replenishment : " + Ra.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                            + " Amount for Replenihsment : " + Ra.NewAmount.ToString("#,##0.00") + Environment.NewLine
                            + " At Action management an email and report will be sent to  " + Environment.NewLine
                            + " CIT provider and responsible departments. ";
                    }

                    buttonInMoney.Hide();
                }

                if (Am.NeedType == 13 & Ac.CashInType == "As per RRDM suggest" & Ac.CitId != "1000" & Ra.RecordFound == false) // Repl daily calculated
                {
                    textBoxNeedReason.Text = " There is need. Go define Money in.";
                    textBoxRecomAction.Text = " By pressing the button Money in " + Environment.NewLine
                            + " You will prompt to define money " + Environment.NewLine
                            + " After Definintion the needed action will be created. ";
                }

                if ((Am.NeedType == 13 || Am.NeedType == 12) & Ac.CashInType == "As per RRDM suggest" & Ac.CitId == "1000" & Ra.RecordFound == true) // ALERT FOR BOC
                {
                    textBoxNeedReason.Text = "Bank ATM Operator was alerted to replenish ";

                    textBoxRecomAction.Text = " A alert Replenishment Action with Id : " + Ra.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                                   + " Next Replenishment : " + Ra.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                                   + " Amount for Replenihsment : " + Ra.NewAmount.ToString("#,##0.00") + Environment.NewLine
                                   + " An Alerting SMS and email was sent to ATM owner at this email:  " + Environment.NewLine
                                   + " " + Ac.AtmReplUserEmail;
                }

                if (Am.NeedType == 13 & Ac.CashInType == "As per RRDM suggest" & Ac.CitId != "1000" & Ra.RecordFound == true) // CIT CASE
                {
                    textBoxNeedReason.Text = " There is need. Action was created ";
                    textBoxRecomAction.Text = " An Replenishment Action with Id : " + Ra.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                            + " Next Replenishment : " + Ra.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                            + " Amount for Replenihsment : " + Ra.NewAmount.ToString("#,##0.00") + Environment.NewLine
                            + " At Action management an email and report will be sent to  " + Environment.NewLine
                            + " CIT provider and responsible departments. ";
                }

                if (Am.NeedType == 14)
                {
                    textBoxNeedReason.Text = " Current Balance will run low during not working day ";
                }
                if (Am.NeedType == 15)
                {
                    textBoxNeedReason.Text = " Estimated next is less than planed replenishment ";
                }
                if (Am.NeedType == 16)
                {
                    textBoxNeedReason.Text = " Replenishment Has been delayed And Running Out of Money";
                }

                // CIT operator 
                if (Ac.CitId != "1000")
                {
                    Us.ReadUsersRecord(Ac.CitId);
                    textBoxReplOwner.Text = Us.UserName;
                }
                else
                {
                    textBoxReplOwner.Text = "Repl will be done by Bank personnel.";
                }
            }
            //
            // PREPARE MONEY IN 
            //

            if (WFunction == "PrepareMoneyIn")
            {

                // MAX MODE THEN 

                if (Ac.CashInType == "At Defined Maximum" & Ra.RecordFound == true) // Repl At Maximum 
                {
                    buttonInMoney.Hide();

                    textBoxNeedReason.Text = "Bank Operator will Replenish";

                    textBoxRecomAction.Text = " A default Replenishment Action with Id : " + Ra.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                                   + " Amount for Replenihsment : " + Ra.SystemAmount.ToString("#,##0.00") + Environment.NewLine
                                   + " ";
                }

                // NOT MAXIMUM ... Go to define How Much money to insert
                if (Ac.CashInType == "As per RRDM suggest")
                {
                    buttonInMoney.Show();

                    if (Ra.RecordFound == true) // Action was created 
                    {
                        textBoxNeedReason.Text = "Bank operator will Replenish";

                        textBoxRecomAction.Text = " A alert Replenishment Action with Id : " + Ra.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                                       + " Next Replenishment : " + Ra.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                                       + " Amount for Replenihsment : " + Ra.NewAmount.ToString("#,##0.00");

                        button3.Show();
                    }
                    else
                    {
                        // No Action was created yet 
                        // Define Money In 
                        textBoxNeedReason.Text = "Bank Operator will Replenish ";

                        textBoxRecomAction.Text = "Go and define money in." + Environment.NewLine
                                       + "System will guide the amount of money to put in ATM.";

                        button3.Hide();
                    }
                }

            }

          
        }
        //
// ATMs in Need Loop
//
        private void FindAtmsInNeed(int InTableSize)
        {
            //
            // 
            // Read all ATMs you are allowed one by one and see if In Need
            // 
            J = 0;
            //
            // LOOP 
            //
            while (J < InTableSize)
            {

                // Read next ATM
                WAtmNo = (string)Am.TableATMsMainSelected.Rows[J]["AtmNo"];

                // Initialise variables
                Am.ReadAtmsMainSpecific(WAtmNo);
                WEstReplDt = DateTime.Now;
                Am.EstReplDt = WEstReplDt;
                Am.LessMinCash = false;
                Am.UpdateAtmsMain(WAtmNo);

                Am.ReadAtmsMainSpecific(WAtmNo);
                Ac.ReadAtm(WAtmNo);

                if (FinishPressed == true & Am.ActionNo == 0)
                {
                    // NO actION TAKEN
                    OutstandingFoundNoAction = true;
                    return;
                }

                if (FinishPressed == true & Am.ActionNo > 0 & WFunction == "PrepareMoneyIn")
                {
                    // NO Authorisation made 
                    if (Am.ActionConfirmed == false)
                    {
                        OutstandingFoundNoConfirm = true;
                        return;
                    }

                }

                //if (Ac.CitId == "1000" & Ac.CashInType == "As per RRDM suggest" & Ac.TypeOfRepl == "10")
                //{
                //}

                //if ( (Ac.CitId != "1000" & Ac.ActiveAtm == true)
                //    || ((Ac.TypeOfRepl == "20" & Ac.CashInType =="At Defined Maximum") & Ac.ActiveAtm == true))

                if (Ac.ActiveAtm == true)
                {
                    Ta.FindNextAndLastReplCycleId(WAtmNo);

                    //WCurrentSesNo =Ta.NextReplNo; 
                    //WSesNo = Am.ReplCycleNo;

                    // Read Transactions of this session which has Status = -1
                    // And find current Balance 

                    if (WFunction == "ATMsInNeed")
                    {
                        //Hide not needed controls 
                        //button3.Hide();
                        //pictureBoxConfirmed.Hide();

                        if (Am.CurrCassettes < Ac.MinCash & Ac.MinCash > 0)
                        {
                            Am.ReadAtmsMainSpecific(WAtmNo);
                            Am.LessMinCash = true;
                            Am.UpdateAtmsMain(WAtmNo);
                        }
                        else
                        {
                            Am.ReadAtmsMainSpecific(WAtmNo);
                            Am.LessMinCash = false;
                            Am.UpdateAtmsMain(WAtmNo);

                        }

                        // FIND NEXT REPLENISHEMENT DATE BASED ON THE MONEY AVAILABLE
                        //

                        FindNextRepl(WAtmNo, WCurrentSesNo, Am.CurrCassettes);

                        Am.EstReplDt = WEstReplDt;
                        //
                        // Initialise need type
                        //
                        // 10 : Normal for replenishment 
                        // 11 : Late Replenishment => Today.Now > Next Replenishment date
                        // 12 : Replenish Now not enough money for Today
                        // 13 : Inform G4S to Replenished in "two" days.
                        // 14 : ATM will run out of money during Weekened or Holiday
                        // 15 : Estimated next replenishement date < than next planned date 
                        // 16 : ATM appears to have many captured cards and has many Errors

                        Am.ReadAtmsMainSpecific(WAtmNo);
                        Am.NeedType = 10; // Initialise to Normal for Replenishment 
                        Am.UpdateAtmsMain(WAtmNo);

                        int result = DateTime.Compare(Am.NextReplDt, WDTm);
                        if (result < 0)
                        {
                            Am.ReadAtmsMainSpecific(WAtmNo);
                            Am.NeedType = 11; // 
                            Am.UpdateAtmsMain(WAtmNo);
                            // next planned for Replenishemnt day less than today
                            // = > Immetiate Replenishment needed. 
                        }

                        if (Am.NextReplDt != LongFutureDate)
                        {
                            result = DateTime.Compare(Am.EstReplDt, Am.NextReplDt);
                            if (result < 0)
                            {

                                Am.ReadAtmsMainSpecific(WAtmNo);
                                Am.NeedType = 15; // 
                                Am.UpdateAtmsMain(WAtmNo);
                                // Estimated next reple < Than planned replenishment 
                                // = > Think to do replenishent before planned. 
                            }

                        }

                        if (Rc.NoOfDays == 0)
                        {

                            Am.ReadAtmsMainSpecific(WAtmNo);
                            Am.NeedType = 12;
                            Am.UpdateAtmsMain(WAtmNo);
                        }
                        //*********************************************************
                        // JCC ... MODEL + Emporiki 
                        // WorkingDaysTotal is coming from Rc
                        //********************************************************* 
                        if (Ac.CitId != "1000" & (Ac.ReplAlertDays >= (WorkingDaysTotal - 1) || Am.CurrCassettes < Ac.MinCash)) // INFORM G4S X days before need
                        {
                            Am.ReadAtmsMainSpecific(WAtmNo);
                            Am.NeedType = 13; // THERE IS NEED
                            WEstReplDt = DateTime.Now.AddDays(Ac.ReplAlertDays);
                            Am.EstReplDt = WEstReplDt;
                            Am.LessMinCash = true;

                            Am.UpdateAtmsMain(WAtmNo);

                            //TEST 
                            // JCC NEED 
                            // Create Action with maximum amount 

                            // If After this process we have 10 then no action is needed. 
                            // *** JCC TYPE ******
                            // Inform Group 4 X days before needed repl. 
                            // Repl at Max amount 
                            // Action is created 

                            if (Ac.CashInType == "At Defined Maximum") // THERE IS NEED AND Repl At Max
                            {
                                // Create action with maximum amount 

                                CalculateAddNotes(Ac.MaxCash, 2);

                                //***************************
                                //INSERT ACTION 
                                //***************************

                                Update_Insert_Action(WEstReplDt, Ac.MaxCash,
                                                       Tot_51, Tot_52, Tot_53, Tot_54);

                                Am.ReadAtmsMainSpecific(WAtmNo);

                                Am.ActionNo = WActionNo;

                                Am.EstReplDt = WEstReplDt;

                                Am.NextReplDt = WEstReplDt;

                                Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 

                            }

                        }

                        //*********************************************************
                        // OUR ATMS - BOC + EMPORIKI MODEL
                        // Conditions in if statement must change 
                        // WorkingDaysTotal is coming from Rc
                        //********************************************************* 
                        if (Ac.CitId == "1000" & (Ac.ReplAlertDays >= (WorkingDaysTotal - 1) || Am.CurrCassettes < Ac.MinCash)) // INFORM BANK personel X days or if money not enough before next repl 
                        {
                            // THERE IS NEED

                            Am.ReadAtmsMainSpecific(WAtmNo);
                            Am.ActionNo = 13;
                            WEstReplDt = DateTime.Now.AddDays(Ac.ReplAlertDays);
                            Am.EstReplDt = WEstReplDt;
                            Am.LessMinCash = true;
                            Am.UpdateAtmsMain(WAtmNo); // ATM MAIN UPDATE

                            //TEST 
                            // JCC NEED 
                            // Create Action with maximum amount 

                            // If After this process we have 10 then no action is needed. 
                            // *** JCC TYPE ******
                            // Inform Group 4 X days before needed repl. 
                            // Repl at Max amount 
                            // Action is created 


                            // EMPORIKI
                            if (Ac.CashInType == "At Defined Maximum") // THERE IS NEED AND Repl At Max
                            {
                                // Create action with maximum amount 

                                CalculateAddNotes(Ac.MaxCash, 2);

                                //***************************
                                //INSERT ACTION 
                                //***************************

                                Update_Insert_Action(WEstReplDt, Ac.MaxCash,
                                                       Tot_51, Tot_52, Tot_53, Tot_54);

                                Am.ActionNo = WActionNo;

                                Am.EstReplDt = WEstReplDt;

                                Am.NextReplDt = WEstReplDt;

                                Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 

                                // EMBORIKI MODEL 

                                Ac.ReadAtmOwner(WAtmNo);

                                RRDMEmailClass2 Em = new RRDMEmailClass2();

                                string Recipient = Ac.AtmReplUserEmail;

                                string Subject = "Replenish ATM as per Instructions";

                                string EmailBody = "Dear " + Ac.AtmReplUserName + Environment.NewLine + Environment.NewLine
                                 + "Please replenish ATM as below instructions" + Environment.NewLine
                                 + "A default Replenishment Action with Id : " + WActionNo.ToString() + " is suggested" + Environment.NewLine
                                 + "Next Replenishment : " + Ra.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                                 + "Amount for Replenihsment : " + Ra.NewAmount.ToString("#,##0.00") + Environment.NewLine + Environment.NewLine
                                 + "Should you have any problems do not hesitate to call your controller ";

                                Em.SendEmail(WOperator, Recipient, Subject, EmailBody);

                                if (Em.MessageSent == true)
                                {
                                    // Deactivate Record 
                                    // In such cases another field to be used 
                                    // The record to be deactivated when Replenishment is made  
                                    Ra.ReadReplActionsForAtm(WAtmNo, Am.ActionNo);
                                    Ra.ActiveRecord = false;
                                    Ra.UpdateReplActionsForAtm(WAtmNo, Am.ActionNo);
                                }
                                else
                                {
                                    MessageBox.Show("Failure in sending email. Maybe no Internet Connection.");
                                    // Report Error 
                                }

                            }

                            //*********************************************
                            // BOC ALERT ---- ****************************
                            // CitId =1000
                            //*********************************************

                            if (Ac.CashInType == "As per RRDM suggest" || Ac.CashInType == "As per Third Party System") // THERE IS NEED AND Repl At Max
                            {
                                // Create action at no amount
                                // Send email and SMS to user 

                                CalculateAddNotes(Ac.MaxCash, 2);

                                //***************************
                                //INSERT ACTION 
                                //***************************

                                Update_Insert_Action(WEstReplDt, Ac.MaxCash,
                                                       Tot_51, Tot_52, Tot_53, Tot_54);

                                Am.ActionNo = WActionNo;

                                Am.EstReplDt = WEstReplDt;

                                Am.NextReplDt = WEstReplDt;

                                Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 

                                // BOC ALERT 

                                Ac.ReadAtmOwner(WAtmNo);

                                RRDMEmailClass2 Em = new RRDMEmailClass2();

                                string Recipient = Ac.AtmReplUserEmail;

                                string Subject = "Replenish ATM at once";

                                string EmailBody = "Dear " + Ac.AtmReplUserName + Environment.NewLine + Environment.NewLine
                                 + "Please replenish ATM with money." + Environment.NewLine
                                 + "A default Replenishment Action with Id : " + WActionNo.ToString() + " is suggested " + Environment.NewLine
                                 + "Next Replenishment : " + Ra.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                                 + "Amount for Replenihsment : " + Ra.NewAmount.ToString("#,##0.00") + Environment.NewLine + Environment.NewLine
                                 + "Should you have any problems do not hesitate to call your controller ";

                                Em.SendEmail(WOperator, Recipient, Subject, EmailBody);

                                if (Em.MessageSent == true)
                                {
                                    // Deactivate Record 
                                    // In such cases another field to be used 
                                    // The record to be deactivated when Replenishment is made  
                                    Ra.ReadReplActionsForAtm(WAtmNo, Am.ActionNo);
                                    Ra.ActiveRecord = false;
                                    Ra.UpdateReplActionsForAtm(WAtmNo, Am.ActionNo);
                                }
                                else
                                {
                                    MessageBox.Show("Failure in sending email. Maybe no Internet Connection.");
                                    // Report Error 
                                }

                            }

                        }

                        // 
                        // If shows normal and Next repl shows weekend or holiday 
                        if (Am.NeedType == 10 & (Weekend == true || Holiday == true))
                        {
                            Am.ReadAtmsMainSpecific(WAtmNo);
                            Am.NeedType = 14; // 
                            Am.UpdateAtmsMain(WAtmNo);
                        }

                        // If After this process we have 10 then no action is needed. 
                        if (Am.NeedType == 10 & Am.ActionNo != 0)
                        {
                            Ra.ReadReplActionsForAtm(WAtmNo, Am.ActionNo);

                            Ra.ActiveRecord = false;

                            Ra.UpdateReplActionsForAtm(WAtmNo, Am.ActionNo);

                            Am.ReadAtmsMainSpecific(WAtmNo);
                            Am.ActionNo = 0;
                            Am.UpdateAtmsMain(WAtmNo); // ATM MAIN UPDATE           
                        }

                        // Am.NeedType = 15 => Later use 
                        // See if ACtion was defined for today 
                        if (Am.ActionNo > 0)
                        {
                            Ra.ReadReplActionsForAtm(WAtmNo, Am.ActionNo);
                            if (Ra.RecordFound == true) // NOT FOUND FOR EMPORIKI CAUSE ACTION ALREADY TAKEN 
                            {
                                if (Ra.DateInsert.Date != DateTime.Today)
                                {

                                }
                            }
                            else
                            {
                                Am.ReadAtmsMainSpecific(WAtmNo);
                                Am.ActionNo = 0;
                                Am.UpdateAtmsMain(WAtmNo); // ATM MAIN UPDATE
                            }
                        }

                        //******************************************
                        // UPDATE 
                        //******************************************
                        Am.ReadAtmsMainSpecific(WAtmNo);

                        Am.LastInNeedReview = DateTime.Now;

                        Am.UpdateAtmsMain(WAtmNo); // ATM MAIN UPDATE


                    }
                    if (WFunction == "PrepareMoneyIn")
                    {
                        // Check that parameters are correct

                        //  CASH MANAGEMENT from RRDM during ATMs in NEED Process 
                        string ParId = "202";
                        string OccurId = "1";
                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                        string RRDMCashManagement = Gp.OccuranceNm;
                        //  CASH MANAGEMENT Prior Replenishment Workflow  
                        ParId = "211";
                        OccurId = "1";
                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                        string CashEstPriorReplen = Gp.OccuranceNm; // IF YES THEN IS PRIOR THOUGHT AN ACTION 

                        //Ra.ReadReplActionsSpecific(Am.ActionNo);
                        Ra.ReadReplActionsForSpecificReplCycle(WAtmNo, Ta.NextReplNo);
                        if (Ac.CitId == "1000" & (RRDMCashManagement == "YES" & CashEstPriorReplen == "YES"))
                        {
                            if (Ra.RecordFound == true & Am.ActionConfirmed == true)
                            {
                                J = J + 1;
                                continue; 
                            }
                            else
                            {
                                // MAX MODE THEN 
                                if (Ac.CashInType == "At Defined Maximum") // THERE IS NEED AND Repl At Max
                                {
                                    // Create action with maximum amount

                                    CalculateAddNotes(Ac.MaxCash, 2);

                                    //***************************
                                    //INSERT ACTION 
                                    //***************************

                                    Update_Insert_Action(WEstReplDt, Ac.MaxCash,
                                                           Tot_51, Tot_52, Tot_53, Tot_54);

                                    Am.ActionNo = WActionNo;

                                    Am.EstReplDt = WEstReplDt;

                                    Am.NextReplDt = WEstReplDt;

                                    Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 
                                                               //
                                                               // Show Override option
                                                               //
                                    

                                }
                                else
                                {
                                   
                                }
                            }
                          

                            // Go to define How Much money to insert
                            if (Ac.CashInType == "As per RRDM suggest") // THERE IS NEED AND Repl At Max
                            {
                                //Do not create action 
                                // Action will be created during definition 

                            }
                        }
                    }
                }

                J++;
            }
        }
        //
        // Display Panel with Cassettes
        //
        private void DisplayPanelCassettesAndOverridde()
        {
            Ra.ReadReplActionsForAtmReplCycleNo(WAtmNo,WCurrentSesNo); 
         
            if (Ra.RecordFound == true)
            {
                panelCassettes.Show();
                Ac.ReadAtm(WAtmNo);
                // Cassette 1
                int temp = Convert.ToInt32(Ac.FaceValue_11);
                labelCas1.Text = "Type 1- " + temp.ToString() + " " + Ac.CurName_11;
                textBoxCas1.Text = Ra.Cassette_1.ToString();
                if (Ac.CasCapacity_11 == 0) textBoxCas1.ReadOnly = true;
                else textBoxCas1.ReadOnly = false;
                // Cassette 2
                temp = Convert.ToInt32(Ac.FaceValue_12);
                labelCas2.Text = "Type 2 - " + temp.ToString() + " " + Ac.CurName_12;
                textBoxCas2.Text = Ra.Cassette_2.ToString();
                if (Ac.CasCapacity_12 == 0) textBoxCas2.ReadOnly = true;
                else textBoxCas2.ReadOnly = false;
                // Cassette 3
                temp = Convert.ToInt32(Ac.FaceValue_13);
                labelCas3.Text = "Type 3 - " + temp.ToString() + " " + Ac.CurName_13;
                textBoxCas3.Text = Ra.Cassette_3.ToString();
                if (Ac.CasCapacity_13 == 0) textBoxCas3.ReadOnly = true;
                else textBoxCas3.ReadOnly = false;
                // Cassette 4
                temp = Convert.ToInt32(Ac.FaceValue_14);
                labelCas4.Text = "Type 4 - " + temp.ToString() + " " + Ac.CurName_14;
                textBoxCas4.Text = Ra.Cassette_4.ToString();
                if (Ac.CasCapacity_14 == 0) textBoxCas4.ReadOnly = true;
                else textBoxCas4.ReadOnly = false;

            }
            else
            {
                panelCassettes.Hide();
            }

            //if (Ra.NewAmount >0 & Ra.SystemAmount != Ra.NewAmount)
            if (Ra.NewAmount > 0)
            {
                labelTotal.Show();
                textBoxTotal.Show();
                textBoxTotal.Text = Ra.NewAmount.ToString("#,##0.00");
            }
            else
            {
                labelTotal.Hide();
                textBoxTotal.Hide(); 
            }

        }

        int NotesCas1;
        int NotesCas2;
        int NotesCas3;
        int NotesCas4;

        // CONFIRM AND PRINT ACTION 
      
        private void button3_Click_1(object sender, EventArgs e)
        {
            // read textBox of cas 1
            if (decimal.TryParse(textBoxTotal.Text, out CasseteValue))
            {

            }
            else
            {
                MessageBox.Show(textBoxTotal.Text, "Please enter a valid amount for Overridde!");
                return;
            }
           
            //
            // Do Validation and update action if necessary 
            //
            
            Ac.ReadAtm(WAtmNo);
            if (Ac.CashInType == "At Defined Maximum")
            {
                if (CasseteValue > Ac.InsurOne)
                {
                    MessageBox.Show("Insured amount is : " + Ac.InsurOne.ToString("#,##0.00") + "Your input is greater than this amount!");
                    return;
                }
            }
           
            // Update Action If Overridden

            Ra.ReadReplActionsForSpecificReplCycle(WAtmNo, Ta.NextReplNo);
            if (Ra.RecordFound == true )
            {
                if (Ac.CashInType == "At Defined Maximum")
                {
                    if (Ra.SystemAmount != CasseteValue & CasseteValue > 0)
                    {
                        CassetteHandling(); 
                        Update_Insert_Action(WEstReplDt, CasseteValue,
                                       NotesCas1, NotesCas2, NotesCas3, NotesCas4);
                    }
                }          
            }
            else
            {
                //==================================

                MessageBox.Show("No Action was created.");
                return; 

            }

            // Print Report 
            Ra.ReadReplActionsForSpecificReplCycle(WAtmNo, Ta.NextReplNo);
            if (Ra.RecordFound == true)
            {
               
                // UPDATE REPL ACTIONS TABLE 
                Ra.AuthorisedDate = DateTime.Now;
               if (WFunction=="ATMsInNeed")
                {
                    Ra.AuthorisedRecord = false;
                }
               else
                {
                    Ra.AuthorisedRecord = true;
                }
               

                Ra.UpdateReplActionsForAtm(WAtmNo, Ra.ReplOrderNo);

                // Update ATMs Main 
                Am.ReadAtmsMainSpecific(WAtmNo);

                Am.ActionConfirmed = true;

                Am.UpdateAtmsMain(WAtmNo);

                //WRowIndex = dataGridView1.SelectedRows[0].Index;

                ////if (OverrideSwitch == false)
                ////{
                ////    Form50b_Load(this, new EventArgs());
                ////}

                //dataGridView1.Rows[WRowIndex].Selected = true;
                //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                string P1 = "Notes to be Loaded For ATM : " + WAtmNo;
                string P2 = WAtmNo;
                string P3 = textBoxRecomAction.Text;
                string P4 = WOperator;
                string P5 = WSignedId;
                string P6 = Ra.ReplOrderNo.ToString();

                Form56R60ATMS ReportATMS60 = new Form56R60ATMS(P1, P2, P3, P4, P5, P6);
                ReportATMS60.Show();

                // Create Transactions 

                Am.ReadAtmsMainSpecific(WAtmNo);
                
                RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();
                Tp.DeleteOldTransToBePostedByATMNoAndSession(WAtmNo, WNextReplNumber, "Cash moved to Atm Supervisor from Vaults");

                Ec.CreateTransTobepostedfromCashManagement(WOperator, WAtmNo, WNextReplNumber, WSignedId,
                                                           Ra.NewAmount);

                pictureBoxConfirmed.Show();

            }
            else
            {
                //==================================

                MessageBox.Show("No Action was created.");

            }

        }

        // GO TO CALCULATE MONEY  

        private void buttonInMoney_Click_1(object sender, EventArgs e)
        {
            if (Ra.RecordFound == true)
            {
                MessageBox.Show("ACtion with Id: " + Ra.ReplOrderNo.ToString() + " Will be reviewed!");
            }

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            // Process No Updating
            //
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ProcessNo = 9;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, Ta.NextReplNo);
            NForm51.FormClosed += NForm51_FormClosed;
            NForm51.ShowDialog();
        }

        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {
            FirstCycle = true;
            Form50b_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

        // FIND NEXT REPLENISHMENT DATE
        private void FindNextRepl(string InAtmNo, int InSesNo, decimal InAvailMoney)
        {
            Ac.ReadAtm(WAtmNo); // GET type of Replenishment 

            //  Rc.GiveMeDataTableReplInfo(WOperator, WDTm, WOperator, WPrive, InAtmNo, NullPastDate, InAvailMoney, Ac.MatchDatesCateg);
            int Request = 3; // Find money will last at 
            Rc.GiveMeDataTableReplInfo(WOperator, WDTm,
                   NullPastDate, WAtmNo, InAvailMoney, Ac.MatchDatesCateg, Request);

            if (Rc.NoOfDays == 0)
            {
                WEstReplDt = DateTime.Now;
            }

            if (Rc.NoOfDays > 0)
            {

                GridDays = new DataTable();
                GridDays.Clear();

                // DATA TABLE ROWS DEFINITION 
                GridDays.Columns.Add("Day", typeof(string));
                GridDays.Columns.Add("Date", typeof(DateTime));
                GridDays.Columns.Add("Amount", typeof(decimal));
                GridDays.Columns.Add("Type", typeof(string));
                GridDays.Columns.Add("Balance", typeof(decimal));

                DateTime TempDtTm = WDTm;
                //   DataRow RowGrid = GridDays.NewRow();

                WorkingDaysTotal = 0;

                TotalRepl = 0;

                I = 0;

                while (I < (Rc.dtRDays.Rows.Count))
                {
                    // IN WHILE LOOP WE LEAVE OUT THE LAST DAY. THIS IS THE REPLENISHEMENT DAY 
                    DataRow RowGrid = GridDays.NewRow();

                    TempDtTm = (DateTime)Rc.dtRDays.Rows[I]["Date"];

                    Fr.ReadFixedDaysReplAtm(WOperator, WAtmNo, TempDtTm); // Correct if fixed 

                    if (Fr.RecordFound == true)
                    {
                        Rc.dtRDays.Rows[I]["RecDispensed"] = Fr.Final;
                    }

                    RowGrid["Day"] = TempDtTm.Date.DayOfWeek.ToString();
                    RowGrid["Date"] = TempDtTm.Date.ToString();
                    RowGrid["Amount"] = Rc.dtRDays.Rows[I]["RecDispensed"];

                    WorkingDay = (bool)Rc.dtRDays.Rows[I]["Normal"];
                    Weekend = (bool)Rc.dtRDays.Rows[I]["Weekend"];
                    Holiday = (bool)Rc.dtRDays.Rows[I]["Special"];

                    if (WorkingDay == true) WorkingDaysTotal = WorkingDaysTotal + 1;

                    if (WorkingDay == true) RowGrid["Type"] = "WorkingDay";
                    if (Weekend == true) RowGrid["Type"] = "Weekend";
                    if (Holiday == true) RowGrid["Type"] = "Holiday";


                    TotalRepl = TotalRepl + (decimal)Rc.dtRDays.Rows[I]["RecDispensed"];

                    RowGrid["Balance"] = InAvailMoney - TotalRepl;

                    GridDays.Rows.Add(RowGrid);  // ADD THE ROW TO THE TABLE 
                    if (I == Rc.dtRDays.Rows.Count - 1) // Last ROW 
                    {
                        WEstReplDt = (DateTime)Rc.dtRDays.Rows[I]["Date"];
                        if (WorkingDay == true) RowGrid["Type"] = "Repl-WorkDay";
                        if (Weekend == true) RowGrid["Type"] = "Repl-Weekend";
                        if (Holiday == true) RowGrid["Type"] = "Repl-Holiday";
                    }

                    I++;
                }
            }
        }
        //
        // UPDATE ACTION 
        // 
        private void Update_Insert_Action(DateTime InNextRepDtTm, decimal InTotal,
                                         int InTot_51, int InTot_52, int InTot_53, int InTot_54)
        {
            // Read ATMs Main to get info 
            // Update fields for action 
            // Insert Action

            Am.ReadAtmsMainSpecific(WAtmNo);
            Ac.ReadAtm(WAtmNo);

            // CHECK IF ACTION ALREADY INSERTED 

            Ra.ReadReplActionsForSpecificReplCycle(WAtmNo, Ta.NextReplNo);

            if (Ra.RecordFound == true)
            {
                if (Ra.ActiveRecord == true)
                {
                    // Move to update Existing Record 

                    //   Ra.ReadReplActionsForAtm(WAtmNo, Ra.ReplActNo);
                    WActionNo = Ra.ReplOrderNo;
                    Ra.ReplOrderId = 30;  // Do Replenishment  
                    //Ra.AuthorisedDate = DateTime.Now; // Effective date 
                    Ra.AtmNo = WAtmNo;

                    Ra.ReplCycleNo = Ta.NextReplNo;

                    Ra.AtmName = Ac.AtmName;
                    Ra.BankId = WOperator;

                    Ra.RespBranch = Ac.Branch;
                    Ra.BranchName = Ac.BranchName;

                    Ra.OffSite = Ac.OffSite;
                    Ra.LastReplDt = Am.LastReplDt;
                    Ra.TypeOfRepl = Ac.TypeOfRepl;

                    Ra.OverEst = Ac.OverEst;    // % To fill more 

                    // NULL Values 
                    Ra.NextReplDt = Am.NextReplDt; // OLd date set for replenishment 

                    if (Ac.CurName_11 != "") Ra.CurrNm = Ac.CurName_11;
                    if (Ac.CurName_12 != "") Ra.CurrNm = Ac.CurName_12;
                    if (Ac.CurName_13 != "") Ra.CurrNm = Ac.CurName_13;
                    if (Ac.CurName_14 != "") Ra.CurrNm = Ac.CurName_14;

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

                    Ra.NewEstReplDt = InNextRepDtTm;
                    Ra.NewAmount = InTotal;

                    Ra.Cassette_1 = InTot_51; // Number of notes 
                    Ra.Cassette_2 = InTot_52;
                    Ra.Cassette_3 = InTot_53;
                    Ra.Cassette_4 = InTot_54;

                    Ra.AtmsStatsGroup = Ac.AtmsStatsGroup;
                    Ra.AtmsReplGroup = Ac.AtmsReplGroup;
                    Ra.AtmsReconcGroup = Ac.AtmsReconcGroup;
                    Ra.DateInsert = DateTime.Now;
                    Ra.AuthUser = WSignedId;
                    Ra.OwnerUser = WSignedId;
                    Ra.CitId = Am.CitId;

                    Ra.ActiveRecord = true;

                    // UPDATE EXISTING 
                    //

                    Ra.UpdateReplActionsForAtm(WAtmNo, Ra.ReplOrderNo);

                }
                else
                {
                    // Warn User that action already taken.

                    //MessageBox.Show(" For this ATM action was already taken");
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

                Ra.ReplCycleNo = Ta.NextReplNo;

                Ra.BankId = Am.BankId;

                Ra.RespBranch = Am.RespBranch;
                Ra.BranchName = Am.BranchName;

                Ra.OffSite = Ac.OffSite;
                Ra.LastReplDt = Am.LastReplDt;
                Ra.TypeOfRepl = Ac.TypeOfRepl;

                Ra.OverEst = Ac.OverEst;    // % To fill more 
                //cmd.Parameters.AddWithValue("@InsuredAmount", InsuredAmount);
                //cmd.Parameters.AddWithValue("@SystemAmount", SystemAmount);
                //cmd.Parameters.AddWithValue("@NewAmount", NewAmount);

                // NULL Values 
                Ra.NextReplDt = Am.NextReplDt; // OLd date set for replenishment 

                if (Ac.CurName_11 != "") Ra.CurrNm = Ac.CurName_11;
                if (Ac.CurName_12 != "") Ra.CurrNm = Ac.CurName_12;
                if (Ac.CurName_13 != "") Ra.CurrNm = Ac.CurName_13;
                if (Ac.CurName_14 != "") Ra.CurrNm = Ac.CurName_14;

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

                Ra.NewEstReplDt = InNextRepDtTm;
                Ra.SystemAmount = Ra.InsuredAmount = Ra.NewAmount = InTotal;

                if (WOperator == "ETHNCY2N") Ra.InsuredAmount = Ac.InsurOne;

                Ra.FaceValue_1 = Ac.FaceValue_11;
                Ra.Cassette_1 = InTot_51; // Number of notes 
                Ra.FaceValue_2 = Ac.FaceValue_12;
                Ra.Cassette_2 = InTot_52;
                Ra.FaceValue_3 = Ac.FaceValue_13;
                Ra.Cassette_3 = InTot_53;
                Ra.FaceValue_4 = Ac.FaceValue_14;
                Ra.Cassette_4 = InTot_54;

                Ra.AtmsStatsGroup = Ac.AtmsStatsGroup;
                Ra.AtmsReplGroup = Ac.AtmsReplGroup;
                Ra.AtmsReconcGroup = Ac.AtmsReconcGroup;
                Ra.DateInsert = DateTime.Now;
                Ra.AuthUser = WSignedId;
                Ra.OwnerUser = WSignedId;
                Ra.CitId = Am.CitId;

                Ra.ActiveRecord = true;
                Ac.ReadAtm(WAtmNo);
                Ra.Operator = Ac.Operator;

                WActionNo = Ra.InsertReplOrdersTable(WAtmNo);

                Am.ReadAtmsMainSpecific(WAtmNo);
                Am.ActionConfirmed = false;
                Am.UpdateAtmsMain(WAtmNo);

            }

        }

        // CALCULATE WHAT NOTES TO INSERT 
        private void CalculateAddNotes(decimal InAmount, int InFunction)
        {
            // SINGLE CURRENCY  

            Ac.ReadAtm(WAtmNo);

            if (InFunction == 2)
            {
                Tot_51 = Convert.ToInt32(InAmount * Ac.CasCapacity_11 / 100 / Ac.FaceValue_11);
                Tot_52 = Convert.ToInt32(InAmount * Ac.CasCapacity_12 / 100 / Ac.FaceValue_12);
                Tot_53 = Convert.ToInt32(InAmount * Ac.CasCapacity_13 / 100 / Ac.FaceValue_13);
                Tot_54 = Convert.ToInt32(InAmount * Ac.CasCapacity_14 / 100 / Ac.FaceValue_14);
            }

            // FINAL CORRECTED TOTAL 

            FinalCorrectedReplTotal = Tot_51 * Ac.FaceValue_11 + Tot_52 * Ac.FaceValue_12
                                 + Tot_53 * Ac.FaceValue_13 + Tot_54 * Ac.FaceValue_14;

        }

        // CALCULATE AMOUNT FROM CASSETTES INPUT 
        private decimal CalculateAmountFromCasseteInput(int InCas1, int InCas2, int InCas3, int InCas4)
        {
            // SINGLE CURRENCY  
            decimal OutAmount = 0;

            Ac.ReadAtm(WAtmNo);

            OutAmount = InCas1 * Ac.FaceValue_11 + InCas2 * Ac.FaceValue_12
                                 + InCas3 * Ac.FaceValue_13 + InCas4 * Ac.FaceValue_14;

            return OutAmount;
        }
        // FINISH 
        //bool StayInForm; 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            //StayInForm = false; 
            FinishPressed = true;
            //OutstandingFoundNoAction = false;
            FindAtmsInNeed(WTableSize);
            // Check if outstanding 
            if (OutstandingFoundNoAction == true)
            {
                if (MessageBox.Show("ATM = " + WAtmNo + " Not all actions are created." + Environment.NewLine
                    + " Do you want to close form?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
                {
                    // YES
                    this.Dispose();
                }
                else
                {
                    //StayInForm = true;
                }
            }
            else
            {
                //  this.Dispose();
            }

            if (OutstandingFoundNoConfirm == true)
            {
                if (MessageBox.Show("ATM = " + WAtmNo + " Not all actions are confirmed. " + Environment.NewLine
                    + "Do you want to close form?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                          == DialogResult.Yes)
                {
                    // YES
                    this.Dispose();
                }
                else
                {
                    //StayInForm = true;
                }
            }
            else
            {
                // this.Dispose();
            }

            if (OutstandingFoundNoAction == false & OutstandingFoundNoConfirm == false)
            {
                this.Dispose();
            }

        }
        // Show SELECTION 
        private void button2_Click(object sender, EventArgs e)
        {
            /*
                        if (Am.NeedType == 10) textBox5.Text = " THIS ATM is not in NEED ";
                        if (Am.NeedType == 11) textBox5.Text = " Replenishment Has been delayed ";
                        if (Am.NeedType == 12) textBox5.Text = " Replenish now because of low balance ";
                        if (Am.NeedType == 13) textBox5.Text = " Inform G4S to replenish in 2 days ";
                        if (Am.NeedType == 14) textBox5.Text = " Current Balance will run low during not working day ";
                        if (Am.NeedType == 15) textBox5.Text = " Estimated next is less than planed replenishment ";
                        if (Am.NeedType == 16) textBox5.Text = " Replenishment Has been delayed And Running Out of Money";
                         */

            // THIS FUNCTION IS FOR SECURITY LEVEL 4 
            string ForGroup = "";

            if (checkBoxIfGroup.Checked == true)
            {
                if (int.TryParse(textBoxGroupNumber.Text, out InGroup))
                {
                    ForGroup = " AND AtmsReplGroup =" + InGroup;
                }
                else
                {
                    MessageBox.Show(textBoxGroupNumber.Text, "Please enter a valid number!");
                    return;
                }
            }

            if (radioButton1.Checked == true) // All ATMS in NEED 
            {

                Tablefilter = "Operator ='" + WOperator + "'" + " AND NeedType > 10 " + ForGroup;

            }

            if (radioButton2.Checked == true) // Late replenishment  
            {

                Tablefilter = "Operator ='" + WOperator + "'" + " AND NeedType = 11 " + ForGroup;

            }

            if (radioButton3.Checked == true) // NEED OF MONEY - NEED OF REPLENISHMENT   
            {

                Tablefilter = "Operator ='" + WOperator + "'"
                    + " AND (NeedType = 12 OR NeedType = 13 OR NeedType = 14) " + ForGroup;

            }

            //atmsMainBindingSource.Filter = Tablefilter;
            ////        dataGridView1.Sort(dataGridView1.Columns[6], ListSortDirection.Ascending);
            //this.atmsMainTableAdapter.Fill(this.aTMSDataSet4.AtmsMain);
        }
// If First cassette change
        private void textBoxCas1_TextChanged(object sender, EventArgs e)
        {
            CassetteHandling();

        }

        // Second Cassette Change
        private void textBoxCas2_TextChanged(object sender, EventArgs e)
        {
            CassetteHandling();
        }
        // Third Cassette change
        private void textBoxCas3_TextChanged(object sender, EventArgs e)
        {
            CassetteHandling();
        }
        decimal CasseteValue; 
        // Forth cassette
        private void textBoxCas4_TextChanged(object sender, EventArgs e)
        {
            CassetteHandling();
        }

        private void CassetteHandling()
        {
            if (FirstCycle == true) return;

            // read textBox of cas 1
            if (int.TryParse(textBoxCas1.Text, out NotesCas1))
            {

            }
            else
            {
                MessageBox.Show(textBoxCas1.Text, "Please enter a valid amount for Cassete 1!");
                return;
            }
            // read textBox of cas 2
            if (int.TryParse(textBoxCas2.Text, out NotesCas2))
            {

            }
            else
            {
                MessageBox.Show(textBoxCas2.Text, "Please enter a valid amount for Cassete 2!");
                return;
            }
            // read textBox of cas 3
            if (int.TryParse(textBoxCas3.Text, out NotesCas3))
            {

            }
            else
            {
                MessageBox.Show(textBoxCas3.Text, "Please enter a valid amount for Cassete 3!");
                return;
            }
            // read textBox of cas 4
            if (int.TryParse(textBoxCas4.Text, out NotesCas4))
            {

            }
            else
            {
                MessageBox.Show(textBoxCas4.Text, "Please enter a valid amount for Cassete 1!");
                return;
            }

            CasseteValue = CalculateAmountFromCasseteInput(NotesCas1, NotesCas2, NotesCas3, NotesCas4);

            
            labelTotal.Show();
            textBoxTotal.Show();
          
            textBoxTotal.Text = CasseteValue.ToString("#,##0.00");
            pictureBoxConfirmed.Hide();
        }
    }
}

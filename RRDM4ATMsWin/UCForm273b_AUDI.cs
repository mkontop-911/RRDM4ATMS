using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
//using System.Runtime.InteropServices;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm273b_AUDI : UserControl
    {

        //public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        int Tot_51;
        int Tot_52;
        int Tot_53;
        int Tot_54;

        Decimal FinalCorrectedReplTotal;

        DateTime WDTm;

        int WActionNo;

        bool FirstCycle;

        // DATATable for Grid 
        DataTable TableNextDaysTillRepl = new DataTable();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass(); // ATM MAIN CLASS TO UPDATE NEXT REPLENISHMENT DATE

        RRDMReplDatesCalc Rc = new RRDMReplDatesCalc(); // Locate next Replenishment 

        RRDMDepositsClass Dc = new RRDMDepositsClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();

        RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMFixedDaysReplAtmClass Fr = new RRDMFixedDaysReplAtmClass();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMDepositsClass Da = new RRDMDepositsClass();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        string WAtmNo;

        int WCurrentSesNo;
        decimal WCurrCassettes;
        decimal WCurrentDeposits;
        int WCapturedCards;
        int WErrorsOutstanding;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCitId;
        int WOrdersCycle;
        string WFunction;

        public void UCForm273b_AUDI_Par(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InOrdersCycleNo, string InFunction)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId;
            WOrdersCycle = InOrdersCycleNo;
            WFunction = InFunction;
            // InFunction = "PrepareMoneyIn" ... means ATM operator at Branch needs to move to Replenishment process
            // InFunction == "ATMsInNeed" .... means call came from ATMs in Need button ..........

            InitializeComponent();

            // Set Working Date 

            //labelToday.Text = DateTime.Now.ToShortDateString();

            //pictureBox1.BackgroundImage = appResImg.logo2;
            //
            // For testing
            //
            if (WOperator == "CRBAGRAA")
            {
                WDTm = new DateTime(2014, 02, 28);
            }
            else
            {
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                string WJobCategory = "ATMs";
                int WReconcCycleNo;

                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
                if (WReconcCycleNo == 0)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
                DateTime WCut_Off_Date = Rjc.Cut_Off_Date;
                WDTm = WCut_Off_Date;
            }

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************

            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
                                   //   NormalProcess = false;

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            //WRMCategory = Us.WFieldChar1;
            //WRMCycle = Us.WFieldNumeric1;

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor)
            //      if (WViewFunction || WAuthoriser || WRequestor || WViewHistory)
            {
                //NormalProcess = false;

                panelCassettes.Enabled = false;
                panel5.Enabled = false; 
            }
            else
            {
                //NormalProcess = true;
            }

            if (WAuthoriser == true)
            {
                //panel2.Location.X.Equals = 9; 
            }

            //if (WViewFunction == true) textBoxMsgBoard.Text = "View Only!";
            //if (WAuthoriser == true) textBoxMsgBoard.Text = "Authoriser to examine one by one and take action.";
            //if (WRequestor == true) textBoxMsgBoard.Text = "Wait for Authoriser.Use Refresh if you wish.";
            //if (NormalProcess == true) textBoxMsgBoard.Text = "Review data and take necessary action for each individual ATM";

            //*************************************************** 
            //***************************************************

            FirstCycle = true;
            //
           
            if (WFunction == "PrepareMoneyIn")
            {
                //labelStep1.Text = "Define Money(Notes) to Load";
                //      textBoxGridHeader.Text = "ATMs To Calculate Money In";

                textBoxReplOwner.Text = "Repl will be done by Branch personnel.";

                textBoxNeedReason.Text = "Scheduled ATM Replenishment";
            }
            else
            {
                //labelStep1.Text = "Create Repl Orders for Cit.." + WCitId;

            }

            /// <summary>
            ///  DESCRIPTION OF PROCESS
            /// </summary>

            ///
            /// FIND HOW LONG MONEY WILL LAST
            ///
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

        // SHOW SCREEN 
        // ON LOAD 
        int WTableSize;
        public void SetScreen()
        {

            //  OutstandingFoundNoAction = false;
            //OutstandingFoundNoConfirm = false;
            //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE AND Use ---------------//
            //
            //LOAD TABLE 
            //MY ATMS if coming from pre-Repl
            //ATMs In Need if comes from need button
            //
            // Create table with the ATMs this user can access

            //bool IncludeAction = true;
            //Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, "", WFunction, WCitId);

            ////-----------------UPDATE LATEST TRANSACTIONS----------------------//
            //// Update latest transactions from Journal 
            //Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);

            ////-------------------

            //WTableSize = Am.TableATMsMainSelected.Rows.Count;


            // DISPLAY CREATED ORDERS
            //
            string WSelectionCriteria = "  CitId ='" + WCitId + "' AND OrdersCycleNo = " + WOrdersCycle;

            Ro.ReadReplActionsAndFillTable(WOperator, WSelectionCriteria, WDTm, WDTm, 1);

            // SHOW GRID
            ShowGrid();
            //

            textBoxTotalOrders.Text = Ro.TableReplOrders.Rows.Count.ToString();

        }

        // Row enter 
        int WReplOrderNo;
        int RowLine; 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WReplOrderNo = (int)rowSelected.Cells[0].Value;

            Ro.ReadReplActionsSpecific(WReplOrderNo);

            WCurrentSesNo = Ro.ReplCycleNo;

            WAtmNo = Ro.AtmNo;

            textBoxAtmNo.Text = WAtmNo;

            // Initialise variables

            Na.ReadSessionsNotesAndValues(WAtmNo, WCurrentSesNo, 2);
            WCurrCassettes = Na.Balances1.ReplToRepl;
            WCapturedCards = Na.CaptCardsMachine;
            WErrorsOutstanding = Na.ErrOutstanding;

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WCurrentSesNo);
            WCurrentDeposits = Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej + Da.DepositsMachine1.EnvAmount + Da.ChequesMachine1.Amount;

            textBoxSesNo.Text = WCurrentSesNo.ToString();
            textBoxCassettesAmnt.Text = WCurrCassettes.ToString("#,##0.00");
            textBoxDepositsBNAMachine.Text = WCurrentDeposits.ToString("#,##0.00");
            textBoxCapturedCards.Text = WCapturedCards.ToString();
            textBoxOutstandingErrors.Text = WErrorsOutstanding.ToString();

            Am.ReadAtmsMainSpecific(WAtmNo);

            textBoxNeedReason.Text = "";

            textBoxSesNo.Text = WCurrentSesNo.ToString();

            DisplayPanelCassettesAndOverridde(WReplOrderNo);

            FirstCycle = false;

            Ac.ReadAtm(WAtmNo);

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
            // Days to empty Before 
            Rc.FindNextRepl(WOperator, WAtmNo, WCurrentSesNo, WCurrCassettes, WDTm);

            textBoxDaysToEmpty.Text = (Rc.NoOfDays ).ToString();

            // Days to empty AFTER
            Rc.FindNextRepl(WOperator, WAtmNo, WCurrentSesNo, Ro.NewAmount, WDTm);

            textBoxDaysAfter.Text = Rc.NoOfDays.ToString();


            //      WOperator = Ac.BankId;

            // InFunction = "PrepareMoneyIn" ... means ATM operator at Branch needs to move to Replenishment process
            // InFunction == "ATMsInNeed" .... means call came from ATMs in Need button ..........

            if (WFunction == "ATMsInNeed")
            {
                if (Ac.ActiveAtm == false) textBoxNeedReason.Text = " THIS ATM is not ACTIVE YET ";

                if (Ro.NeedType == 10)
                {
                    textBoxNeedReason.Text = " THIS ATM is not in NEED ";
                }
                if (Ro.NeedType == 11)
                {
                    textBoxNeedReason.Text = " Replenishment Has been delayed ";
                }
                if (Ro.NeedType == 12)
                {
                    textBoxNeedReason.Text = " Replenish now because of low balance ";
                }
                if (Ro.NeedType == 13)
                {
                    textBoxNeedReason.Text = "This ATM is in Need ";
                }
                if (Ro.NeedType == 13 & Ac.CashInType == "At Defined Maximum" & Ro.RecordFound == true)
                {
                    if (Ac.CitId == "1000") // EMBORIKI MODEL 
                    {
                        textBoxNeedReason.Text = "Inform Bank ATM Operator to replenish ";

                        // textBoxRecomAction.Text = " A default Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                        //+ " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                        //+ " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine
                        //+ " An Alerting email was sent to ATM owner at this email:  " + Environment.NewLine
                        //+ " " + Ac.AtmReplUserEmail;
                    }
                    if (Ac.CitId != "1000" & Ac.CitId != "") // JCC MODEL 
                    {
                        textBoxNeedReason.Text = " Inform CIT " + Ac.CitId + " to replenish ";

                        //  textBoxRecomAction.Text = " A default Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                        //+ " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                        //+ " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine
                        //+ " At Action management an email and report will be sent to  " + Environment.NewLine
                        //+ " CIT provider and responsible departments. ";
                    }

                }

                if (Ro.NeedType == 13 & Ac.CashInType == "As per RRDM suggest" & Ac.CitId != "1000" & Ro.RecordFound == false) // Repl daily calculated
                {
                    textBoxNeedReason.Text = " There is need. Go define Money in.";
                    // textBoxRecomAction.Text = " By pressing the button Money in " + Environment.NewLine
                    //+ " You will prompt to define money " + Environment.NewLine
                    //+ " After Definintion the needed action will be created. ";
                }

                if ((Ro.NeedType == 13 || Ro.NeedType == 12) & Ac.CashInType == "As per RRDM suggest" & Ac.CitId == "1000" & Ro.RecordFound == true) // ALERT FOR BOC
                {
                    textBoxNeedReason.Text = "Bank ATM Operator was alerted to replenish ";

                    // textBoxRecomAction.Text = " A alert Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                    //+ " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                    //+ " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine
                    //+ " An Alerting SMS and email was sent to ATM owner at this email:  " + Environment.NewLine
                    //+ " " + Ac.AtmReplUserEmail;
                }

                if (Ro.NeedType == 13 & Ac.CashInType == "As per RRDM suggest" & Ac.CitId != "1000" & Ro.RecordFound == true) // CIT CASE
                {
                    textBoxNeedReason.Text = " There is need. Action was created ";
                    //  textBoxRecomAction.Text = " An Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                    //+ " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                    //+ " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine
                    //+ " At Action management an email and report will be sent to  " + Environment.NewLine
                    //+ " CIT provider and responsible departments. ";
                }

                if (Ro.NeedType == 14)
                {
                    textBoxNeedReason.Text = " Current Balance will run low during not working day ";
                }
                if (Ro.NeedType == 15)
                {
                    textBoxNeedReason.Text = " Estimated next is less than planed replenishment ";
                }
                if (Ro.NeedType == 16)
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

                if (Ac.CashInType == "At Defined Maximum" & Ro.RecordFound == true) // Repl At Maximum 
                {

                    textBoxNeedReason.Text = "Bank Operator will Replenish";

                    // textBoxRecomAction.Text = " A default Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                    //+ " Amount for Replenihsment : " + Ro.SystemAmount.ToString("#,##0.00") + Environment.NewLine
                    //+ " ";
                }

                // NOT MAXIMUM ... Go to define How Much money to insert
                if (Ac.CashInType == "As per RRDM suggest")
                {

                    if (Ro.RecordFound == true) // Action was created 
                    {
                        textBoxNeedReason.Text = "Bank operator will Replenish";

                        //  textBoxRecomAction.Text = " A alert Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                        //+ " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                        //+ " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00");

                    }
                    else
                    {
                        // No Action was created yet 
                        // Define Money In 
                        textBoxNeedReason.Text = "Bank Operator will Replenish ";

                        //textBoxRecomAction.Text = "Go and define money in." + Environment.NewLine
                        //               + "System will guide the amount of money to put in ATM.";

                    }
                }
            }
        }

        // Show Grid 
        public void ShowGrid()
        {

            dataGridView1.DataSource = Ro.TableReplOrders.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 70; // OrderNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 70; // AtmNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].HeaderText = "Atm No";

            dataGridView1.Columns[2].Width = 100; //  AtmName
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].HeaderText = "Atm Name";

            dataGridView1.Columns[3].Width = 70; // ReplCycleNo
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderText = "Repl Cycle";

            dataGridView1.Columns[4].Width = 100; // NeedStatus
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].HeaderText = "Need Status";

            dataGridView1.Columns[5].Width = 90; // OrdersCycleNo
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].HeaderText = "Orders Cycle No";
            dataGridView1.Columns[5].Visible = false;

            dataGridView1.Columns[6].Width = 90; // CurrentCassettes
            dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].HeaderText = "Current Cassettes";

            dataGridView1.Columns[7].Width = 70; //DaysToLast
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].HeaderText = "Days To Last";

            dataGridView1.Columns[8].Width = 90; // LastReplDt
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].HeaderText = "Last Repl Dt";

            dataGridView1.Columns[9].Width = 90; // NewEstReplDt
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[9].HeaderText = "New Est ReplDt";

            dataGridView1.Columns[10].Width = 90; //ToLoadAmount
            dataGridView1.Columns[10].DefaultCellStyle = style;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[10].HeaderText = "To Load Amount";

        }


    
        //
        // UPDATE ACTION 
        // 
        private int Update_Insert_Action(string InAtmNo, int InCurrentSesNo, DateTime InNextRepDtTm, decimal InTotal,
                                         int InTot_51, int InTot_52, int InTot_53, int InTot_54)
        {
            // Read ATMs Main to get info 
            // Update fields for action 
            // Insert Action

            Am.ReadAtmsMainSpecific(InAtmNo);
            Ac.ReadAtm(InAtmNo);

            // CHECK IF ACTION ALREADY INSERTED 

            Ro.ReadReplActionsForSpecificReplCycle(WAtmNo, InCurrentSesNo);

            if (Ro.RecordFound == true)
            {
                if (Ro.ActiveRecord == true)
                {
                    // Move to update Existing Record 

                    WActionNo = Ro.ReplOrderNo;

                    Ro.OrdersCycleNo = WOrdersCycle;
                    Ro.ReplOrderId = 30;  // Do Replenishment  

                    Ro.AtmNo = InAtmNo;

                    Ro.ReplCycleNo = InCurrentSesNo;

                    Ro.AtmName = Ac.AtmName;
                    Ro.BankId = WOperator;

                    Ro.RespBranch = Ac.Branch;
                    Ro.BranchName = Ac.BranchName;

                    Ro.OffSite = Ac.OffSite;
                    Ro.LastReplDt = Am.LastReplDt;
                    Ro.TypeOfRepl = Ac.TypeOfRepl;

                    Ro.OverEst = Ac.OverEst;    // % To fill more 

                    // NULL Values 
                    Ro.NextReplDt = Am.NextReplDt; // OLd date set for replenishment 

                    if (Ac.CurName_11 != "") Ro.CurrNm = Ac.CurName_11;
                    if (Ac.CurName_12 != "") Ro.CurrNm = Ac.CurName_12;
                    if (Ac.CurName_13 != "") Ro.CurrNm = Ac.CurName_13;
                    if (Ac.CurName_14 != "") Ro.CurrNm = Ac.CurName_14;

                    Ro.MinCash = Ac.MinCash;
                    Ro.MaxCash = Ac.MaxCash;
                    Ro.ReplAlertDays = Ac.ReplAlertDays;

                    Ro.ReconcDiff = Am.GL_ReconcDiff;
                    Ro.MoreMaxCash = Am.MoreMaxCash;
                    Ro.LessMinCash = Am.LessMinCash;
                    Ro.NeedType = Am.NeedType;

                    //Ro.CurrCassettes = WCurrCassettes;
                    //Ro.CurrentDeposits = WCurrentDeposits;

                    Ro.EstReplDt = Am.EstReplDt;

                    // THESE ARE THE NEW DATA FOR ACTION 

                    Ro.NewEstReplDt = InNextRepDtTm;
                    Ro.NextReplDt = InNextRepDtTm;
                    Ro.NewAmount = InTotal;

                    Ro.Cassette_1 = InTot_51; // Number of notes 
                    Ro.Cassette_2 = InTot_52;
                    Ro.Cassette_3 = InTot_53;
                    Ro.Cassette_4 = InTot_54;

                    Ro.AtmsStatsGroup = Ac.AtmsStatsGroup;
                    Ro.AtmsReplGroup = Ac.AtmsReplGroup;
                    Ro.AtmsReconcGroup = Ac.AtmsReconcGroup;
                    Ro.DateInsert = DateTime.Now;
                    Ro.AuthUser = WSignedId;
                    Ro.OwnerUser = WSignedId;
                    Ro.CitId = Am.CitId;

                    Ro.ActiveRecord = true;

                    // UPDATE EXISTING 
                    //

                    Ro.UpdateReplActionsForAtm(WAtmNo, Ro.ReplOrderNo);

                }
                else
                {
                    // Warn User that action already taken.

                    //MessageBox.Show(" For this ATM action was already taken");
                }
            }


            return WActionNo;

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

        //
        // Display Panel with Cassettes
        //
        private void DisplayPanelCassettesAndOverridde(int InReplOrderNo)
        {

            Ro.ReadReplActionsSpecific(InReplOrderNo);
            if (Ro.RecordFound == true)
            {
                panelCassettes.Show();
                Ac.ReadAtm(WAtmNo);
                // Cassette 1
                int temp = Convert.ToInt32(Ac.FaceValue_11);
                labelCas1.Text = "Type 1- " + temp.ToString() + " " + Ac.CurName_11;
                textBoxCas1.Text = Ro.Cassette_1.ToString();
                if (Ac.CasCapacity_11 == 0) textBoxCas1.ReadOnly = true;
                else textBoxCas1.ReadOnly = false;
                // Cassette 2
                temp = Convert.ToInt32(Ac.FaceValue_12);
                labelCas2.Text = "Type 2 - " + temp.ToString() + " " + Ac.CurName_12;
                textBoxCas2.Text = Ro.Cassette_2.ToString();
                if (Ac.CasCapacity_12 == 0) textBoxCas2.ReadOnly = true;
                else textBoxCas2.ReadOnly = false;
                // Cassette 3
                temp = Convert.ToInt32(Ac.FaceValue_13);
                labelCas3.Text = "Type 3 - " + temp.ToString() + " " + Ac.CurName_13;
                textBoxCas3.Text = Ro.Cassette_3.ToString();
                if (Ac.CasCapacity_13 == 0) textBoxCas3.ReadOnly = true;
                else textBoxCas3.ReadOnly = false;
                // Cassette 4
                temp = Convert.ToInt32(Ac.FaceValue_14);
                labelCas4.Text = "Type 4 - " + temp.ToString() + " " + Ac.CurName_14;
                textBoxCas4.Text = Ro.Cassette_4.ToString();
                if (Ac.CasCapacity_14 == 0) textBoxCas4.ReadOnly = true;
                else textBoxCas4.ReadOnly = false;

            }
            else
            {
                panelCassettes.Hide();
            }

            //if (Ra.NewAmount >0 & Ra.SystemAmount != Ra.NewAmount)
            if (Ro.NewAmount > 0)
            {
                labelTotal.Show();
                textBoxTotal.Show();
                textBoxTotal.Text = Ro.NewAmount.ToString("#,##0.00");
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


        // Go to days BEFORE LOADED
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //  Am.ReadAtmsMainSpecific(WAtmNo);
            Rc.FindNextRepl(WOperator, WAtmNo, WCurrentSesNo, WCurrCassettes, WDTm);
            if (Rc.NoOfDays <= 0)
            {
                MessageBox.Show("No days to show! ");
                return;
            }
            // Delivers TableNextDaysTillRepl table 
            Form78b NForm78b;
            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            string WHeader = "Next days till empty for : " + WAtmNo;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rc.TableNextDaysTillRepl, WHeader, "Form50b_AUDI");
            // NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }
        // VIEW WHEN LOADED 
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //  Am.ReadAtmsMainSpecific(WAtmNo);
            Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WCurrentSesNo);
            Rc.FindNextRepl(WOperator, WAtmNo, WCurrentSesNo, Ro.NewAmount, WDTm);
            if (Rc.NoOfDays <= 0)
            {
                MessageBox.Show("No days to show! ");
                return;
            }
            // Delivers TableNextDaysTillRepl table 
            Form78b NForm78b;
            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            string WHeader = "Next days till empty for : " + WAtmNo;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rc.TableNextDaysTillRepl, WHeader, "Form50b_AUDI");
            // NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }

        // Act 
        private void buttonActionOnSelected_Click(object sender, EventArgs e)
        {
            if (radioButtonInactivate.Checked == false & radioButtonActivate.Checked == false)
            {
                MessageBox.Show("Please make your choice");
                return;
            }

            // If Inactivate then 
            if (radioButtonInactivate.Checked == true)
            {
                // Set action to inactive

                if (textBoxComment.Text == "")
                {
                    MessageBox.Show("Please enter comment.");
                    return;
                }

                Ro.ReadReplActionsForAtm(WAtmNo, WReplOrderNo);

                Ro.ActiveRecord = false;

                Ro.InactivateComment = textBoxComment.Text;

                Ro.UpdateReplActionsForAtm(WAtmNo, WReplOrderNo);

                SetScreen();

            }
        }

        private void textBoxCas1_TextChanged(object sender, EventArgs e)
        {
            buttonUpdate.Enabled = true;
        }

        private void textBoxCas2_TextChanged(object sender, EventArgs e)
        {
            CassetteHandling();
        }

        private void textBoxCas3_TextChanged(object sender, EventArgs e)
        {
            CassetteHandling();
        }

        private void textBoxCas4_TextChanged(object sender, EventArgs e)
        {
            CassetteHandling();
        }

        decimal CasseteValue;

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
            // Update Action If Overridden     

        }

        // UPDATE 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            Ro.ReadReplActionsForSpecificReplCycle(WAtmNo, WCurrentSesNo);
            if (Ro.RecordFound == true)
            {

                if (Ro.SystemAmount != CasseteValue & CasseteValue > 0)
                {
                    Update_Insert_Action(WAtmNo, WCurrentSesNo, Ro.EstReplDt, CasseteValue,
                                         NotesCas1, NotesCas2, NotesCas3, NotesCas4);
                }

            }
            int WRowIndex = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            SetScreen();

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
            
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
        // Print Orders
        private void buttonPrintOrders_Click(object sender, EventArgs e)
        {
            string P1 = "REPLENISHMENT ORDERS CYCLE:" + WOrdersCycle.ToString() + " TO CIT:" + WCitId;

            string P2 = WCitId;
            string P3 = WOrdersCycle.ToString();
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_Repl_Orders Report_Repl_Orders = new Form56R69ATMS_Repl_Orders(P1, P2, P3, P4, P5);
            Report_Repl_Orders.Show();
        }

        private void labelAfter_Click(object sender, EventArgs e)
        {

        }

        private void textBoxDaysAfter_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

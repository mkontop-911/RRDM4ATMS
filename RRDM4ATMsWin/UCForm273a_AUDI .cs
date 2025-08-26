using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm273a_AUDI : UserControl
    {

        public string guidanceMsg;
        bool NormalProcess; 

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        int Tot_51;
        int Tot_52;
        int Tot_53;
        int Tot_54;

        Decimal FinalCorrectedReplTotal;

        DateTime WDTm;

        int J;
        int WActionNo;

        bool Temp_NBG_FIX;

        //RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

        //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMAtmsDailyTransHistory_Forecasting Hf = new RRDMAtmsDailyTransHistory_Forecasting(); 

        RRDM_IST_WebService IstWeb = new RRDM_IST_WebService(); 

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMDepositsClass Da = new RRDMDepositsClass();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass(); // ATM MAIN CLASS TO UPDATE NEXT REPLENISHMENT DATE

        RRDMReplDatesCalc Rc = new RRDMReplDatesCalc(); // Locate next Replenishment 

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();

        RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMFixedDaysReplAtmClass Fr = new RRDMFixedDaysReplAtmClass();

        RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

        RRDMReplOrders_Pre_ATMs Rpre = new RRDMReplOrders_Pre_ATMs();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        //// DATATable for Grid 
        //DataTable TableNextDaysTillRepl = new DataTable();

        string WAtmNo;
        //int WCurrentSesNo;
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

        public void UCForm273a_AUDI_Par(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InOrdersCycle, string InFunction)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId;
            WOrdersCycle = InOrdersCycle;
            WFunction = InFunction;
            // InFunction = "PrepareMoneyIn" ... means ATM operator at Branch needs to move to Replenishment process
            // InFunction == "ATMsInNeed" .... means call came from ATMs in Need button ..........

            InitializeComponent();

            // Set Working Date 

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
                NormalProcess = false;

                panelCassettes.Enabled = false;
                buttonForceOrder.Enabled = false;
                buttonUndoForced.Enabled = false;
            }
            else
            {
                NormalProcess = true;
            }

        

            //if (WViewFunction == true) textBoxMsgBoard.Text = "View Only!";
            //if (WAuthoriser == true) textBoxMsgBoard.Text = "Authoriser to examine one by one and take action.";
            //if (WRequestor == true) textBoxMsgBoard.Text = "Wait for Authoriser.Use Refresh if you wish.";
            //if (NormalProcess == true) textBoxMsgBoard.Text = "Review data and take necessary action for each individual ATM";

            //*************************************************** 
            //***************************************************

            //  FirstCycle = true;
            //
           
            if (WFunction == "PrepareMoneyIn")
            {
                // THIS A TEMPORARY FIX FOR NBG
                if (WOperator == "ETHNCY2N" || WOperator == "CRBAGRAA") Temp_NBG_FIX = true;
                else Temp_NBG_FIX = false;

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
            // If In need and Maximum = 0 and "As Per RRDM suggest" then promt user to go and calculate and actions are created
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
        bool InternalSwitch; 
        public void SetScreen()
        {

            //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE AND Use ---------------//
            //
            //LOAD TABLE 
            //MY ATMS if coming from pre-Repl
            //ATMs In Need if comes from need button
            //
            // Create table with the ATMs this user can access
            if (NormalProcess == true)
            {
                bool IncludeAction = true;

                string filter = "Operator = '" + WOperator + "' AND UserId ='" + WCitId + "'";
                
                Ua.ReadUserAccessToAtmsFillTable(filter);

                //Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, "", WFunction, WCitId);

                WTableSize = Ua.UsersToAtmsDataTable.Rows.Count;

                textBoxTotalAtms.Text = WTableSize.ToString();

                /// <summary>
                /// Find ATMs in need
                /// Read all in Table and examine one by one
                /// 

                if (ForcedOrder == false)
                {
                    //
                    // SUMMARY OF PROCESS
                    // 
                    // FOR each ATM DO
                    // 
                    // GET THE CURRENTLY AVAILABLE MONEY FROM IST WEB SERVICE
                    // FIND HOW MANY DAYS THE MONEY WILL LAST
                    // Having in mind the days to be replenished
                    // Weekends ETC decide if the ATM is in need. 
                    // 
                    FIND_Atms_InNeed_Step1(WTableSize);
                }

                //
                // DISPLAY ATMS STATUS BASED ON THE THE Atms in need result 
                //

                Am.ReadAtmsMainForReplNeedsStatus(WOperator, WSignedId, WSignRecordNo, WCitId, WDTm, Temp_NBG_FIX, WOrdersCycle);

                dataGridView1.DataSource = Am.TableATMsMainReplNeeds.DefaultView;
                textBoxTotalAtms.Text = Am.TableATMsMainReplNeeds.Rows.Count.ToString();
            }
            else
            {
                
                Rpre.ReadReplOrders_Pre_ATMsFillTable(WOrdersCycle);
                dataGridView1.DataSource = Rpre.TableReplOrders_Pre_ATMs.DefaultView;
                textBoxTotalAtms.Text = Rpre.TableReplOrders_Pre_ATMs.Rows.Count.ToString(); 
            }


            // SHOW GRID
            ShowGrid();

            // Read Orders Counters
            Ro.ReadReplActionsForCounters(WCitId, WOrdersCycle);

            textBoxTotalOrders.Text = Ro.NumberOfActiveOrders.ToString();

        }

        // Row Enter
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;

            Rpre.ReadReplOrders_Pre_ATMsByAtmNo(WAtmNo);

            textBoxRecomAction.Text = "";

            textBoxNeedReason.Text = "";

            //Am.ReadAtmsMainSpecific(WAtmNo);

            textBoxAtmNo.Text = WAtmNo;
            //
           // WCurrentSesNo = Rpre.ReplCycleNo;
            // 
            //
           
            int WIntAtmNo;
            if (int.TryParse(WAtmNo, out WIntAtmNo))
            {
            }
            IstWeb.ReadFieldsFromWebService(WIntAtmNo);

            WCurrCassettes = IstWeb.AvailableBalance;
            WCapturedCards = 0;
            WErrorsOutstanding = 0;
            WCurrentDeposits = 0;

            //Ta.FindNextAndLastReplCycleId(WAtmNo);
            //if (Ta.RecordFound == true)
            //{
            //    WCurrentSesNo = Ta.Last_1; // Current open session
            //    if (Temp_NBG_FIX == true)
            //    {
            //        WCurrentSesNo = Ta.NextReplNo;
            //    }


            //    // Initialise variables

            //    Na.ReadSessionsNotesAndValues(WAtmNo, WCurrentSesNo, 2);
            //    WCurrCassettes = Na.Balances1.ReplToRepl;
            //    WCapturedCards = Na.CaptCardsMachine;
            //    WErrorsOutstanding = Na.ErrOutstanding;

            //    Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WCurrentSesNo);
            //    WCurrentDeposits = Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej + Da.DepositsMachine1.EnvAmount + Da.ChequesMachine1.Amount;
            //}
            //else
            //{
            //    WCurrentSesNo = 0;
            //    WCurrCassettes = 0;
            //    WCapturedCards = 0;
            //    WErrorsOutstanding = 0;

            //    WCurrentDeposits = 0;

            //}

            //textBoxSesNo.Text = WCurrentSesNo.ToString();
            textBoxCassettesAmnt.Text = WCurrCassettes.ToString("#,##0.00");
            textBoxDepositsBNAMachine.Text = WCurrentDeposits.ToString("#,##0.00");
            textBoxCapturedCards.Text = WCapturedCards.ToString();
            textBoxOutstandingErrors.Text = WErrorsOutstanding.ToString();

            Ac.ReadAtm(WAtmNo);

            //
            // Read Created ORDER
            //
            Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WOrdersCycle);

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

                //
                if ((Ro.NeedType == 13 || Ro.NeedType == 12) & Ac.CashInType == "At Defined Maximum" & Ro.RecordFound == true)
                {
                    if (Ac.CitId == "1000") // EMBORIKI MODEL 
                    {
                        textBoxNeedReason.Text = "Inform Bank ATM Operator to replenish ";

                        textBoxRecomAction.Text = " A default Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                                       + " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                                       + " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine
                                       + " An Alerting email was sent to ATM owner at this email:  " + Environment.NewLine
                                       + " " + Ac.AtmReplUserEmail;
                    }
                    if (Ac.CitId != "1000" & Ac.CitId != "") // JCC MODEL 
                    {
                        textBoxNeedReason.Text = " Inform CIT " + Ac.CitId + " to replenish ";

                        textBoxRecomAction.Text = " A default Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                            + " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                            + " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine
                            + " At Action management an email and report will be sent to  " + Environment.NewLine
                            + " CIT provider and responsible departments. ";
                    }

                }

                if (Ro.NeedType == 13 & Ac.CashInType == "As per RRDM suggest" & Ac.CitId != "1000" & Ro.RecordFound == false) // Repl daily calculated
                {
                    textBoxNeedReason.Text = " There is need. Go define Money in.";
                    textBoxRecomAction.Text = " By pressing the button Money in " + Environment.NewLine
                            + " You will prompt to define money " + Environment.NewLine
                            + " After Definintion the needed action will be created. ";
                }

                if ((Ro.NeedType == 13 || Ro.NeedType == 12) & Ac.CashInType == "As per RRDM suggest" & Ac.CitId == "1000" & Ro.RecordFound == true) // ALERT FOR BOC
                {
                    textBoxNeedReason.Text = "Bank ATM Operator was alerted to replenish ";

                    textBoxRecomAction.Text = " A alert Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                                   + " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                                   + " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine
                                   + " An Alerting SMS and email was sent to ATM owner at this email:  " + Environment.NewLine
                                   + " " + Ac.AtmReplUserEmail;
                }

                if (Ro.NeedType == 13 & Ac.CashInType == "As per RRDM suggest" & Ac.CitId != "1000" & Ro.RecordFound == true) // CIT CASE
                {
                    textBoxNeedReason.Text = " There is need. Action was created ";
                    textBoxRecomAction.Text = " An Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                            + " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                            + " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine
                            + " At Action management an email and report will be sent to  " + Environment.NewLine
                            + " CIT provider and responsible departments. ";
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

            if (WFunction == "PrepareMoneyIn" || Ro.NeedType == 25)
            {

                Ro.ReadReplActionsForSpecificReplCycle(WAtmNo, WOrdersCycle);
                // MAX MODE THEN 

                if (Ac.CashInType == "At Defined Maximum" & Ro.RecordFound == true) // Repl At Maximum 
                {

                    textBoxNeedReason.Text = "Bank Operator will Replenish";

                    textBoxRecomAction.Text = " A default Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                                   + " Amount for Replenihsment : " + Ro.SystemAmount.ToString("#,##0.00") + Environment.NewLine
                                   + " ";
                }

                // NOT MAXIMUM ... Go to define How Much money to insert
                if (Ac.CashInType == "As per RRDM suggest")
                {

                    if (Ro.RecordFound == true) // Action was created 
                    {
                        textBoxNeedReason.Text = "Bank operator will Replenish";

                        textBoxRecomAction.Text = " A alert Replenishment Action with Id : " + Ro.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                                       + " Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                                       + " Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00");

                    }
                    else
                    {
                        // No Action was created yet 
                        // Define Money In 
                        textBoxNeedReason.Text = "Bank Operator will Replenish ";

                        textBoxRecomAction.Text = "Go and define money in." + Environment.NewLine
                                       + "System will guide the amount of money to put in ATM.";

                    }
                }
            }

            Rc.FindNextRepl(WOperator, WAtmNo, WOrdersCycle, WCurrCassettes, WDTm);

            textBoxDaysToEmpty.Text = (Rc.NoOfDays).ToString();

            // DISPLAY CREATED ORDER
            //
            string WSelectionCriteria = "  CitId ='" + WCitId
                  + "' AND OrdersCycleNo = " + WOrdersCycle
                  + " AND AtmNo ='" + WAtmNo + "'";

            Ro.ReadReplActionsAndFillTable(WOperator, WSelectionCriteria, WDTm, WDTm, 1);

            Ro.ReadReplActionsForSpecificReplCycle(WAtmNo, WOrdersCycle);

           // Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WOrdersCycle);

            if (Ro.RecordFound == true)
            {
                textBox7.Show();
                panel7.Show();

                DisplayPanelCassettesAndOverridde();

                textBoxCas1.ReadOnly = true;
                textBoxCas2.ReadOnly = true;
                textBoxCas3.ReadOnly = true;
                textBoxCas4.ReadOnly = true;

                // Days to empty AFTER
                Rc.FindNextRepl(WOperator, WAtmNo, WOrdersCycle, Ro.NewAmount, WDTm);

                textBoxDaysAfter.Text = Rc.NoOfDays.ToString();

            }
            else
            {
                textBox7.Hide();
                panel7.Hide();

            }

          
                // Check if there is existing opened ,
                Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WOrdersCycle);
                if (Ro.RecordFound == true)
                {
                    // Check
                    if (Ro.ActiveRecord == true & Ro.AuthorisedRecord == true)
                    {
                        // We should skip without creating new Order
                        labelActiveOrder.Show();
                        labelActiveOrder.Text = "There is outstanding order for this ATM at Cycle:.." + Ro.OrdersCycleNo.ToString();
                    }
                    else
                    {
                        labelActiveOrder.Hide();
                    }
                }
                else
                {
                    labelActiveOrder.Hide();
                }
            

        }

        // Show Grid 
        public void ShowGrid()
        {

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 70; // AtmNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 100; // AtmName
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].HeaderText = "Atm Name";

            dataGridView1.Columns[2].Width = 50; //  ReplCycleNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].HeaderText = "Repl CycleNo";

            dataGridView1.Columns[3].Width = 90; // NeedStatus
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderText = "Need Status";

            dataGridView1.Columns[4].Width = 50; // OrderCycle
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderText = "Order Cycle";

            dataGridView1.Columns[5].Width = 90; // CurrentCassetes
            dataGridView1.Columns[5].DefaultCellStyle = style;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].HeaderText = "Current Cassetes";

            dataGridView1.Columns[6].Width = 50; // DaysToLast
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].HeaderText = "Days To Last";

            dataGridView1.Columns[7].Width = 90; // Deposits
            dataGridView1.Columns[7].DefaultCellStyle = style;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[8].Width = 50; // CaptureCards
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].HeaderText = "Captured Cards";

            dataGridView1.Columns[9].Width = 50; // Errors
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 80; // NextRepl
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[10].DefaultCellStyle.ForeColor = Color.Red;
            //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;
            dataGridView1.Columns[10].HeaderText = "Next Repl";

            dataGridView1.Columns[11].Width = 90; // ToLoad
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].HeaderText = "Money To Load";

        }

        // Create a new Orders Cycle
      //  string WSelectionCriteria;
      //  DateTime WOutputDate;
      //  int WProcessStage;
      

        //
        // ATMs in Need Loop
        //
        private void FIND_Atms_InNeed_Step1(int InTableSize)
        {
            /// <summary>
            /// Read all ATMs you are allowed one by one and see if In Need
            /// 

            J = 0;
            //
            // LOOP 
            //
            while (J < InTableSize)
            {
               
                // Read next ATM
                // READ ATMS one by one
                WAtmNo = (string)Ua.UsersToAtmsDataTable.Rows[J]["AtmNo"];

                Ac.ReadAtm(WAtmNo);
                if (Ac.ActiveAtm == true)
                {
                    int WIntAtmNo;
                    if (int.TryParse(WAtmNo, out WIntAtmNo))
                    {
                    }
                    IstWeb.ReadFieldsFromWebService(WIntAtmNo);

                    WCurrCassettes = IstWeb.AvailableBalance;
                    WCapturedCards = 0 ;
                    WErrorsOutstanding = 0 ;
                    WCurrentDeposits = 0; 

                    //Ta.FindNextAndLastReplCycleId(WAtmNo);
                    //if (Ta.RecordFound == true)
                    //{
                    //    WCurrentSesNo = Ta.Last_1; // Current open session
                    //    if (Temp_NBG_FIX == true)
                    //    {
                    //        WCurrentSesNo = Ta.NextReplNo;
                    //    }
                    //    // Initialise variables

                    //    Na.ReadSessionsNotesAndValues(WAtmNo, WCurrentSesNo, 2);
                    //    WCurrCassettes = Na.Balances1.ReplToRepl;
                    //    WCapturedCards = Na.CaptCardsMachine;
                    //    WErrorsOutstanding = Na.ErrOutstanding;

                    //    Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WCurrentSesNo);
                    //    WCurrentDeposits = Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej + Da.DepositsMachine1.EnvAmount + Da.ChequesMachine1.Amount;

                    //}
                    //else
                    //{
                    //    // ATM without first SM
                    //    J++;
                    //    continue;
                    //}
                }
                else
                {
                    J++;
                    continue;
                }

                
                    // Check if there is existing opened , authorised order 
                    Ro.ReadReplActionsForAtmReplCycleAuther(WAtmNo, WOrdersCycle);
                    if (Ro.RecordFound == true)
                    {
                        // Check
                        if (Ro.ActiveRecord == true )
                        {
                            // We should skip without creating new Order
                            J++;
                            continue;
                        }
                    
                     }

                // Initialise variables

                Am.ReadAtmsMainSpecific(WAtmNo);
                Rc.EstReplDt = NullPastDate;
                Am.EstReplDt = NullPastDate;
                Am.LessMinCash = false;

                Am.UpdateAtmsMain(WAtmNo);
                //
                //
                if (WFunction == "ATMsInNeed")
                {
                    // Call Method to do all operation
                    FIND_Atms_InNeed_Step2();
                }

                /// <summary>
                /// THIS IS DURING REPLENISHEMENT PROCESS AT BRANCH
                /// </summary>
                if (WFunction == "PrepareMoneyIn")
                {
                    Am.ReadAtmsMainSpecific(WAtmNo);
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

                    Ro.ReadReplActionsForAtmReplCycleAuther(WAtmNo, WOrdersCycle);

                    if (Ac.CitId == "1000" & (RRDMCashManagement == "YES" & CashEstPriorReplen == "YES"))
                    {
                        // if (Ro.RecordFound == true || (Temp_NBG_FIX == true & Ta.ReplOutstanding == 0))
                        if (Ro.RecordFound == true || Temp_NBG_FIX == true)
                        {
                            J = J + 1;
                            continue;
                        }
                    }
                    //
                    //
                    PREPARE_MoneyIn(WOperator, WAtmNo, WOrdersCycle, WCurrCassettes, WDTm, false);
                    //
                    //
                }

                J++;
            }
        }
        //
        // ATMs in NEED Method
        //
        private void FIND_Atms_InNeed_Step2()
        {
          
            //
            // FIND NEXT REPLENISHEMENT DATE BASED ON THE MONEY AVAILABLE
            // RETURN A TABLE


            Rc.FindNextRepl(WOperator, WAtmNo, WOrdersCycle, WCurrCassettes, WDTm);


            // KEEP the Data table in SQL table
            int g = 0;

            while (g < (Rc.TableNextDaysTillRepl.Rows.Count))
            {
                Hf.ReadTransHistory_Forecasting("", "", DateTime.Now);

        //        public int SeqNo;
        //public string AtmNo;
        //public int OrdersCycleNo;
        //public int ReplOrderId;

        //public DateTime FutureDt;

        //public decimal Est_DispensedAmt;
        //public decimal Est_DepAmount;
        //public DateTime CreationDateTime;

        //public string Operator;

                string TempDayOftheWeek = (string)Rc.TableNextDaysTillRepl.Rows[g]["Day"];
                
                string Type = (string)Rc.TableNextDaysTillRepl.Rows[g]["Type"];
                decimal Balance = (decimal)Rc.TableNextDaysTillRepl.Rows[g]["Balance"];

                Hf.AtmNo = WAtmNo;
                Hf.OrdersCycleNo = WOrdersCycle;
                Hf.ReplOrderId = 0; 
                Hf.FutureDt = (DateTime)Rc.TableNextDaysTillRepl.Rows[g]["Date"];
                Hf.Est_DispensedAmt = (decimal)Rc.TableNextDaysTillRepl.Rows[g]["Amount"];
                Hf.Est_DepAmount = 0;
                Hf.CreationDateTime = DateTime.Now;
                Hf.Operator = WOperator;

                Hf.InsertTransHistory_ForecastingRecord(WAtmNo); 

                g++;
            }


            

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
            Am.EstReplDt = Rc.EstReplDt;
            Am.UpdateAtmsMain(WAtmNo);

            // Compare if current next is less than working date 
            int result = DateTime.Compare(Am.EstReplDt, WDTm);
            if (result < 0)
            {
                Am.ReadAtmsMainSpecific(WAtmNo);
                Am.NeedType = 11; // 
                Am.NextReplDt = Am.EstReplDt; 
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
                    Am.NextReplDt = Am.EstReplDt; 
                    Am.UpdateAtmsMain(WAtmNo);
                    // Estimated next reple < Than planned replenishment 
                    // = > Think to do replenishent before planned. 
                }
            }

            if (Rc.NoOfDays == 0)
            {
                Am.ReadAtmsMainSpecific(WAtmNo);
                Am.NeedType = 12;
                Am.NextReplDt = Am.EstReplDt;
                Am.UpdateAtmsMain(WAtmNo);
            }
            //*********************************************************
            // JCC ... MODEL + Emporiki 
            // WorkingDaysTotal is coming from FindNextRepl ---Rc
            //********************************************************* 
            if (Ac.CitId != "1000" &
                ((Rc.WorkingDaysTotal - 1 - Ac.ReplAlertDays < 0) || WCurrCassettes < Ac.MinCash)) // INFORM G4S X days before need
            {
                Am.ReadAtmsMainSpecific(WAtmNo);
                if (Rc.WorkingDaysTotal - 1 - Ac.ReplAlertDays < 0)
                {
                    Am.NeedType = 12; // THERE IS NEED NOW 
                    Am.NextReplDt = WDTm;
                    //   Am.NextReplDt = WDTm.AddDays(Rc.WorkingDaysTotal);
                    //   Am.EstReplDt = Rc.EstReplDt;
                }
                if (WCurrCassettes < Ac.MinCash)
                {
                    Am.LessMinCash = true;
                    Am.NeedType = 13; // THERE IS NEED
                    Am.NextReplDt = WDTm;
                 
                }
                else Am.LessMinCash = false;

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
                    // Calculate 
                    CalculateAddNotes(Ac.MaxCash, 2);

                    //// UPDATE Atms Main
                    //Am.ReadAtmsMainSpecific(WAtmNo);

                    //Am.EstReplDt = Rc.EstReplDt;

                    //Am.NextReplDt = Rc.EstReplDt;

                    //Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 

                    //***************************
                    //INSERT ACTION 
                    //***************************

                    int TempWActionNo = Update_Insert_Action(WAtmNo, WOrdersCycle, Am.NextReplDt, Ac.MaxCash,
                                               Tot_51, Tot_52, Tot_53, Tot_54);
                    // UPDATE Atms Main
                    Am.ReadAtmsMainSpecific(WAtmNo);

                    Am.ActionNo = TempWActionNo; // 

                    Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 


                }

            }

            //*********************************************************
            // OUR ATMS - BOC + EMPORIKI MODEL
            // Conditions in if statement must change 
            // WorkingDaysTotal is coming from Rc
            //********************************************************* 
            if (Ac.CitId == "1000" &
                        ((Rc.WorkingDaysTotal - 1 - Ac.ReplAlertDays < 0) || Am.CurrCassettes < Ac.MinCash)) // INFORM BANK personel X days or if money not enough before next repl 
            {
                // THERE IS NEED

                Am.ReadAtmsMainSpecific(WAtmNo);
                Am.ActionNo = 13;
                Rc.EstReplDt = WDTm.AddDays(Ac.ReplAlertDays);
                Am.EstReplDt = Rc.EstReplDt;
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

                    // UPDATE Atms Main
                    Am.ReadAtmsMainSpecific(WAtmNo);

                    Am.EstReplDt = Rc.EstReplDt;

                    Am.NextReplDt = WDTm;

                    Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 

                    //***************************
                    //INSERT ACTION 
                    //***************************

                    int TempWActionNo = Update_Insert_Action(WAtmNo, WOrdersCycle, WDTm, Ac.MaxCash,
                                               Tot_51, Tot_52, Tot_53, Tot_54);

                    // UPDATE Atms Main
                    Am.ReadAtmsMainSpecific(WAtmNo);

                    Am.ActionNo = TempWActionNo; // 

                    Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 
                                               // EMBORIKI MODEL 

                    Ac.ReadAtmOwner(WAtmNo);

                    RRDMEmailClass2 Em = new RRDMEmailClass2();

                    string Recipient = Ac.AtmReplUserEmail;

                    string Subject = "Replenish ATM as per Instructions";

                    string EmailBody = "Dear " + Ac.AtmReplUserName + Environment.NewLine + Environment.NewLine
                     + "Please replenish ATM as below instructions" + Environment.NewLine
                     + "A default Replenishment Action with Id : " + WActionNo.ToString() + " is suggested" + Environment.NewLine
                     + "Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                     + "Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine + Environment.NewLine
                     + "Should you have any problems do not hesitate to call your controller ";

                    Em.SendEmail(WOperator, Recipient, Subject, EmailBody);

                    if (Em.MessageSent == true)
                    {
                        // Deactivate Record 
                        // In such cases another field to be used 
                        // The record to be deactivated when Replenishment is made  
                        //Ro.ReadReplActionsForAtm(WAtmNo, Am.ActionNo);
                        //Ro.ActiveRecord = false;
                        //Ro.UpdateReplActionsForAtm(WAtmNo, Am.ActionNo);
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

                    // UPDATE Atms Main
                    Am.ReadAtmsMainSpecific(WAtmNo);

                    Am.EstReplDt = Rc.EstReplDt;

                    Am.NextReplDt = Rc.EstReplDt;

                    Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 

                    //***************************
                    //INSERT ACTION 
                    //***************************

                    int TempWActionNo = Update_Insert_Action(WAtmNo, WOrdersCycle, Rc.EstReplDt, Ac.MaxCash,
                                               Tot_51, Tot_52, Tot_53, Tot_54);


                    // UPDATE Atms Main
                    Am.ReadAtmsMainSpecific(WAtmNo);

                    Am.ActionNo = TempWActionNo; // 

                    Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 
                                               // EMBORIKI MODEL 

                    // BOC ALERT 

                    Ac.ReadAtmOwner(WAtmNo);

                    RRDMEmailClass2 Em = new RRDMEmailClass2();

                    string Recipient = Ac.AtmReplUserEmail;

                    string Subject = "Replenish ATM at once";

                    string EmailBody = "Dear " + Ac.AtmReplUserName + Environment.NewLine + Environment.NewLine
                     + "Please replenish ATM with money." + Environment.NewLine
                     + "A default Replenishment Action with Id : " + WActionNo.ToString() + " is suggested " + Environment.NewLine
                     + "Next Replenishment : " + Ro.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                     + "Amount for Replenihsment : " + Ro.NewAmount.ToString("#,##0.00") + Environment.NewLine + Environment.NewLine
                     + "Should you have any problems do not hesitate to call your controller ";

                    Em.SendEmail(WOperator, Recipient, Subject, EmailBody);

                    if (Em.MessageSent == true)
                    {
                        // Deactivate Record 
                        // In such cases another field to be used 
                        // The record to be deactivated when Replenishment is made  
                        //Ro.ReadReplActionsForAtm(WAtmNo, Am.ActionNo);
                        //Ro.ActiveRecord = false;
                        //Ro.UpdateReplActionsForAtm(WAtmNo, Am.ActionNo);
                    }
                    else
                    {
                        MessageBox.Show("Failure in sending email. Maybe no Internet Connection.");
                        // Report Error 
                    }

                }

            }


            // If shows normal and Next repl shows weekend or holiday 
            //if (Am.NeedType == 10 & (Weekend == true || Holiday == true))
            //{
            //    // NO NEED FOR THIS AS WE GET THIS SITUATION FROM THIS:  WorkingDaysTotal - 1 - Ac.ReplAlertDays < 0
            //    //Am.ReadAtmsMainSpecific(WAtmNo);
            //    //Am.NeedType = 14; // 
            //    //Am.UpdateAtmsMain(WAtmNo);
            //}

            // If After this process we have 10 then no action is needed. 
            if (Am.NeedType == 10 & Am.ActionNo != 0)
            {
                //Ro.ReadReplActionsForAtm(WAtmNo, Am.ActionNo);

                ////Ro.ActiveRecord = false;

                //Ro.UpdateReplActionsForAtm(WAtmNo, Am.ActionNo);

                //Am.ReadAtmsMainSpecific(WAtmNo);
                //Am.ActionNo = 0;
                //Am.UpdateAtmsMain(WAtmNo); // ATM MAIN UPDATE           
            }

            // Am.NeedType = 15 => Later use 
            // See if ACtion was defined for today 
            if (Am.ActionNo > 0)
            {
                Ro.ReadReplActionsForAtm(WAtmNo, Am.ActionNo);
                if (Ro.RecordFound == true) // NOT FOUND FOR EMPORIKI CAUSE ACTION ALREADY TAKEN 
                {
                    if (Ro.DateInsert.Date != DateTime.Today)
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

            Am.LastInNeedReview = WDTm;

            Am.UpdateAtmsMain(WAtmNo); // ATM MAIN UPDATE


        }
        //
        // Prepare Money In Method 
        //
        private void PREPARE_MoneyIn(string InOperator,
                         string InAtmNo, int InOrdersCycle,
                         decimal InCurrCassettes, DateTime InDTm,
                                  bool InForcedOrder   )
        {
            //
            // FIND NEXT REPLENISHEMENT DATE BASED ON THE MONEY AVAILABLE
            //

            Rc.FindNextRepl(InOperator, InAtmNo, InOrdersCycle, InCurrCassettes, InDTm);

            Ac.ReadAtm(InAtmNo);

            // MAX MODE THEN 
            if (Ac.CashInType == "At Defined Maximum") // THERE IS NEED AND Repl At Max
            {
                // Create action with maximum amount

                CalculateAddNotes(Ac.MaxCash, 2);

                // UPDATE Atms Main
                Am.ReadAtmsMainSpecific(WAtmNo);

                Am.EstReplDt = Rc.EstReplDt;

                Am.NextReplDt = WDTm;
                if  (InForcedOrder == true)
                {
                    Am.NeedType = 25; // Forced Matched
                }
                else
                {
                    Am.NeedType = 13; // normal need
                }
                
                Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 

                //***************************
                //INSERT ACTION 
                //***************************

                int TempWActionNo = Update_Insert_Action(WAtmNo, WOrdersCycle, WDTm, Ac.MaxCash,
                                           Tot_51, Tot_52, Tot_53, Tot_54);

                // UPDATE Atms Main
                Am.ReadAtmsMainSpecific(WAtmNo);

                Am.ActionNo = TempWActionNo; // 

                Am.UpdateAtmsMain(WAtmNo); // Update ATMs Main with the action number 

            }
            else
            {

            }

            // Go to define How Much money to insert
            //if (Ac.CashInType == "As per RRDM suggest") // THERE IS NEED AND Repl At Max
            //{
            //    //Do not create action 
            //    // Action will be created during definition 

            //}

        }
        //
        // UPDATE ACTION 
        // 
        private int Update_Insert_Action(string InAtmNo, int InOrdersCycle, DateTime InNextRepDtTm, decimal InTotal,
                                         int InTot_51, int InTot_52, int InTot_53, int InTot_54)
        {
            // Read ATMs Main to get info 
            // Update fields for action 
            // Insert Action

            Am.ReadAtmsMainSpecific(InAtmNo);
            Ac.ReadAtm(InAtmNo);

            // CHECK IF ACTION ALREADY INSERTED 

            Ro.ReadReplActionsForSpecificReplCycle(WAtmNo, InOrdersCycle);

            if (Ro.RecordFound == true)
            {
                if (Ro.ActiveRecord == true)
                {
                    // Move to update Existing Record 

                    //   Ra.ReadReplActionsForAtm(WAtmNo, Ra.ReplActNo);
                    WActionNo = Ro.ReplOrderNo;

                    Ro.OrdersCycleNo = WOrdersCycle;
                    Ro.ReplOrderId = 30;  // Do Replenishment  
                    //Ra.AuthorisedDate = DateTime.Now; // Effective date 
                    Ro.AtmNo = InAtmNo;

                    Ro.ReplCycleNo = 0;

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

                    Ro.CurrCassettes = WCurrCassettes;
                    Ro.CurrentDeposits = WCurrentDeposits;

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
            else
            {

                // INSERT NEW ACTION 
                // Repl ACtion FIELDS 

                Ro.ReplOrderId = 30;  // Do Replenishment  
                Ro.OrdersCycleNo = WOrdersCycle;
                //Ra.AuthorisedDate = DateTime.Now; // Effective date 
                Ro.AtmNo = Am.AtmNo;
                Ro.AtmName = Am.AtmName;

                Ro.ReplCycleNo = 0;

                Ro.BankId = Am.BankId;

                Ro.RespBranch = Am.RespBranch;
                Ro.BranchName = Am.BranchName;

                Ro.OffSite = Ac.OffSite;
                Ro.LastReplDt = Am.LastReplDt;
                Ro.TypeOfRepl = Ac.TypeOfRepl;

                Ro.OverEst = Ac.OverEst;    // % To fill more 
                //cmd.Parameters.AddWithValue("@InsuredAmount", InsuredAmount);
                //cmd.Parameters.AddWithValue("@SystemAmount", SystemAmount);
                //cmd.Parameters.AddWithValue("@NewAmount", NewAmount);

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

                Ro.CurrCassettes = WCurrCassettes;
                Ro.CurrentDeposits = WCurrentDeposits;

                // THESE ARE THE NEW DATA FOR ACTION 

                Ro.EstReplDt = Am.EstReplDt;

                Ro.NewEstReplDt = InNextRepDtTm;
                Ro.NextReplDt = InNextRepDtTm;

                Ro.SystemAmount = Ro.InsuredAmount = Ro.NewAmount = InTotal;

                if (WOperator == "ETHNCY2N") Ro.InsuredAmount = Ac.InsurOne;

                Ro.FaceValue_1 = Ac.FaceValue_11;
                Ro.Cassette_1 = InTot_51; // Number of notes 
                Ro.FaceValue_2 = Ac.FaceValue_12;
                Ro.Cassette_2 = InTot_52;
                Ro.FaceValue_3 = Ac.FaceValue_13;
                Ro.Cassette_3 = InTot_53;
                Ro.FaceValue_4 = Ac.FaceValue_14;
                Ro.Cassette_4 = InTot_54;

                Ro.AtmsStatsGroup = Ac.AtmsStatsGroup;
                Ro.AtmsReplGroup = Ac.AtmsReplGroup;
                Ro.AtmsReconcGroup = Ac.AtmsReconcGroup;
                Ro.DateInsert = DateTime.Now;
                Ro.AuthUser = WSignedId;
                Ro.OwnerUser = WSignedId;
                Ro.CitId = Am.CitId;

                Ro.ActiveRecord = true;
                Ac.ReadAtm(WAtmNo);
                Ro.Operator = Ac.Operator;

                WActionNo = Ro.InsertReplOrdersTable(WAtmNo);

                Am.ReadAtmsMainSpecific(WAtmNo);
                Am.ActionConfirmed = false;
                Am.UpdateAtmsMain(WAtmNo);

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
        private void DisplayPanelCassettesAndOverridde()
        {
            Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WOrdersCycle);

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


        // Go to days 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Am.ReadAtmsMainSpecific(WAtmNo);
            Rc.FindNextRepl(WOperator, WAtmNo, WOrdersCycle, WCurrCassettes, WDTm);
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

        // Days After
        private void linkLabelAfter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //  Am.ReadAtmsMainSpecific(WAtmNo);
            Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WOrdersCycle);
            if (Ro.RecordFound == true)
            {
                Rc.FindNextRepl(WOperator, WAtmNo, WOrdersCycle, Ro.NewAmount, WDTm);
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

        }
        // Print Orders 
        private void buttonPrintOrders_Click(object sender, EventArgs e)
        {
            PrintOrders();
        }
        // ORDERS
        private void PrintOrders()
        {
            if (Ro.NumberOfActiveOrders == 0)
            {
                MessageBox.Show("No Orders to print");
                return;
            }
            string P1 = "REPLENISHMENT ORDERS CYCLE:" + WOrdersCycle.ToString() + " TO CIT:" + WCitId;

            string P2 = WCitId;
            string P3 = WOrdersCycle.ToString();
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_Repl_Orders Report_Repl_Orders = new Form56R69ATMS_Repl_Orders(P1, P2, P3, P4, P5);
            Report_Repl_Orders.Show();
        }
        // Print ATMS need Status
        private void buttonPrintATMs_Click(object sender, EventArgs e)
        {
            string P1 = "ATMs In NEED for ORDERS CYCLE:" + WOrdersCycle.ToString() + " TO CIT:" + WCitId;

            string P2 = WOrdersCycle.ToString();
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R80 AtmsStatus = new Form56R80(P1, P2, P3, P4, P5);
            AtmsStatus.Show();
        }
        bool ForcedOrder = false; 
        private void buttonForceOrder_Click(object sender, EventArgs e)
        {
            Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WOrdersCycle);

            if (Ro.RecordFound == true)
            {
                MessageBox.Show("Order Already Exist");
                return; 
            }
            ForcedOrder = true;
            //
            //
            PREPARE_MoneyIn(WOperator, WAtmNo, WOrdersCycle, WCurrCassettes, WDTm, ForcedOrder);

            int WRow1 = dataGridView1.SelectedRows[0].Index;

            InternalSwitch = true; 

            SetScreen();

            InternalSwitch = false;

            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

            ForcedOrder = false; 
            //
            //
        }
// UNDO FORCED
        private void buttonUndoForced_Click(object sender, EventArgs e)
        {
            //
            // Read Created Action
            //
            Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WOrdersCycle);

            if (Ro.RecordFound == true)
            {
                Ro.DeleteReplOrder(Ro.ReplOrderNo);
            }
            else
            {
                MessageBox.Show("No Order To Delete"); 
            }

            int WRow1 = dataGridView1.SelectedRows[0].Index;

            InternalSwitch = true;
            SetScreen();
            InternalSwitch = false;

            dataGridView1.Rows[WRow1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));

        }
    }
}

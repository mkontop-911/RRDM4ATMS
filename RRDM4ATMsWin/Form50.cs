using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form50 : Form
    {
     //   FormMainScreen NFormMainScreen;
      
        Form51 NForm51; // Goto recommend money 

        int Tot_51 ;
        int Tot_52 ;
        int Tot_53 ;
        int Tot_54 ;

        Decimal FinalCorrectedReplTotal;

        bool FinishPressed;
        bool OutstandingFound; 

        DateTime WEstReplDt; // NEXT REPLENISHMENT DATE

        DateTime WDTm = new DateTime(2014, 02, 28);

        decimal TotalRepl; 

        int WorkingDaysTotal;

        bool WorkingDay; 
        bool Weekend; 
        bool Holiday; 

        int I;
        int J;

        int InGroup;
        int WActionNo; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        

        string Tablefilter; 

        // DATATable for Grid 
        DataTable GridDays = new DataTable();
   //     DataTable dtAtmsMain = new DataTable();
    //    SqlDataAdapter daAtmsMain;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass(); // ATM MAIN CLASS TO UPDATE NEXT REPLENISHMENT DATE

        RRDMReplDatesCalc Rc = new RRDMReplDatesCalc(); // Locate next Replenishment 

        RRDMDepositsClass Dc = new RRDMDepositsClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMReplOrdersClass Ra = new RRDMReplOrdersClass(); 

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMFixedDaysReplAtmClass Fr = new RRDMFixedDaysReplAtmClass();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        string WAtmNo;
        int WSesNo; 
   
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     
        public Form50(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator; 

            InitializeComponent();

            //buttonFinish.Hide();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            textBoxMsgBoard.Text = "Review data and take necessary action for each individual ATM"; 

        }
        //
        // ON LOADING 
        //
    
        private void Form50_Load(object sender, EventArgs e)
        {
            // ==================ACCESS TO ATMS=========================

            //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE---------------//

            string AtmNo = "";
            string FromFunction = "General";
            string WCitId = "";

            // Create table with the ATMs this user can access
            Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, AtmNo, FromFunction, WCitId);

            //-----------------UPDATE LATEST TRANSACTIONS----------------------//
            // Update latest transactions from Journal 
            Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);

            //-------------------

            int TableSize = Am.TableATMsMainSelected.Rows.Count;
            //
            // Read all in Table provided by Aj and see if need 
            //
            J = 0;
            //
            // LOOP 
            //
            while (J < TableSize)
            {
                // Initialise variables

                WEstReplDt = DateTime.Now;
                Am.EstReplDt = WEstReplDt;
                Am.LessMinCash = false;  

                // Read next ATM
                WAtmNo = (string)Am.TableATMsMainSelected.Rows[J]["AtmNo"];

                Am.ReadAtmsMainSpecific(WAtmNo);
                Ac.ReadAtm(WAtmNo);

                if (FinishPressed == true & Am.ActionNo == 0 ) 
                {
                    OutstandingFound = true ;
                    return; 
                }

                //if (Ac.CitId == "1000" & Ac.CashInType == "As per RRDM suggest" & Ac.TypeOfRepl == "10")
                //{
                //}

                //if ( (Ac.CitId != "1000" & Ac.ActiveAtm == true)
                //    || ((Ac.TypeOfRepl == "20" & Ac.CashInType =="At Defined Maximum") & Ac.ActiveAtm == true))

            if (  Ac.ActiveAtm == true)
                {
                    WSesNo = Am.CurrentSesNo;

                    // Read Transactions of this session which has Status = -1
                    // And find current Balance 

                    //      Tc.ReadTransForCurrBalance(WAtmNo, WSesNo); 

                    //     Am.CurrentDeposits = Tc.CurrentDeposits;

                    if (Am.CurrCassettes < Ac.MinCash & Ac.MinCash > 0)
                    {
                        Am.LessMinCash = true;
                    }
                    else
                    {
                        Am.LessMinCash = false;
                    }

                    FindNextRepl(WAtmNo, WSesNo, Am.CurrCassettes); // FIND NEXT REPLENISHEMENT DATE BASED ON THE MONEY AVAILABLE

                    Am.EstReplDt = WEstReplDt;

                    // 10 : Normal for replenishment 
                    // 11 : Late Replenishment => Today.Now > Next Replenishment date
                    // 12 : Replenish Now not enough money for Today
                    // 13 : Inform G4S to Replenished in "two" days.
                    // 14 : ATM will run out of money during Weekened or Holiday
                    // 15 : Estimated next replenishement date < than next planned date 
                    // 16 : ATM appears to have many captured cards and has many Errors

                    Am.NeedType = 10; // Initialise to Normal for Replenishment 

                    int result = DateTime.Compare(Am.NextReplDt, WDTm);
                    if (result < 0)
                    {
                        Am.NeedType = 11; // next planned for Replenishemnt day less than today
                        // = > Immetiate Replenishment needed. 
                    }
                        
                    if (Am.NextReplDt != LongFutureDate)
                    {
                        result = DateTime.Compare(Am.EstReplDt, Am.NextReplDt);
                        if (result < 0)
                        {
                            Am.NeedType = 15; // Estimated next reple < Than planned replenishment 
                            // = > Think to do replenishent before planned. 
                        }
                            
                    }

                    if (Rc.NoOfDays == 0 )
                    {
                        Am.NeedType = 12; // DO REPLENISHMENT Now - No Money
                                          // = > Immetiate Replenishment needed. 
                    }
                    //*********************************************************
                    // JCC ... MODEL + Emporiki 
                    // WorkingDaysTotal is coming from Rc
                    //********************************************************* 
                    if (Ac.CitId != "1000" & (Ac.ReplAlertDays >= (WorkingDaysTotal - 1) || Am.CurrCassettes < Ac.MinCash )) // INFORM G4S X days before need
                    {
                        Am.NeedType = 13; // THERE IS NEED

                          WEstReplDt = DateTime.Now.AddDays(Ac.ReplAlertDays); 
                          Am.EstReplDt = WEstReplDt;
                          Am.LessMinCash = true;  

                        //TEST 
                        // JCC NEED 
                        // Create Action with maximum amount 

                          // If After this process we have 10 then no action is needed. 
                          // *** JCC TYPE ******
                          // Inform Group 4 X days before needed repl. 
                          // Repl at Max amount 
                          // Action is created 

                          if ( Ac.CashInType == "At Defined Maximum") // THERE IS NEED AND Repl At Max
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

                          }
                         
                    }

                    //*********************************************************
                    // OUR ATMS - BOC + EMPORIKI MODEL
                    // Conditions in if statement must change 
                    // WorkingDaysTotal is coming from Rc
                    //********************************************************* 
                    if (Ac.CitId == "1000" & (Ac.ReplAlertDays >= (WorkingDaysTotal - 1) || Am.CurrCassettes < Ac.MinCash)) // INFORM BANK personel X days or if money not enough before next repl 
                    {
                        Am.NeedType = 13; // THERE IS NEED

                        WEstReplDt = DateTime.Now.AddDays(Ac.ReplAlertDays);
                        Am.EstReplDt = WEstReplDt;
                        Am.LessMinCash = true;

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
                        Am.NeedType = 14;
                    } 
                  
                    // If After this process we have 10 then no action is needed. 
                    if (Am.NeedType == 10 & Am.ActionNo != 0)
                    {
                        Ra.ReadReplActionsForAtm(WAtmNo, Am.ActionNo);

                        Ra.ActiveRecord = false;

                        Ra.UpdateReplActionsForAtm(WAtmNo, Am.ActionNo);

                        Am.ActionNo = 0;
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
                            Am.ActionNo = 0;
                        }
                    }

                    Am.LastInNeedReview = DateTime.Now;

                    //******************************************
                    // UPDATE 
                    //******************************************

                    Am.UpdateAtmsMain(WAtmNo); // ATM MAIN UPDATE 
                   
                }

                J++;
            }

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo); // REad Access level 

            if (Usi.SecLevel == "03")
            {
                Tablefilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'";
            }
            

            if (Usi.SecLevel == "04")
            {
                Tablefilter = "Operator ='" + WOperator + "'";
            }
           

            //atmsMainBindingSource.Filter = Tablefilter;
            ////        dataGridView1.Sort(dataGridView1.Columns[6], ListSortDirection.Ascending);
            //this.atmsMainTableAdapter.Fill(this.aTMSDataSet4.AtmsMain);

        }

        // ON ROW ENTER ASSIGN ATMNo
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            buttonInMoney.Show(); 

            WAtmNo = (string)rowSelected.Cells[0].Value;

            textBoxAtmNo.Text = WAtmNo;

            Am.ReadAtmsMainSpecific(WAtmNo);

            textBoxNeedReason.Text = "";

            textBoxRecomAction.Text = "";

            Ra.ReadReplActionsForSpecificDate(WAtmNo, DateTime.Today);
            if (Ra.RecordFound == true)
            {
            }
            else
            {
                //==================================

                MessageBox.Show("Remember to set the two testing ATMs to CIT 2000");
                textBoxRecomAction.Text = "No action taken yet";
            }

            WSesNo = Am.CurrentSesNo;

            Ac.ReadAtm(WAtmNo);

      //      WOperator = Ac.BankId;

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
                                   +" " +Ac.AtmReplUserEmail ;
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

            if (Am.NeedType == 13 & Ac.CashInType == "As per RRDM suggest" & Ac.CitId == "1000" & Ra.RecordFound == true) // ALERT FOR BOC
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

        // GO TO RECOMMEND MONEY 
        private void button3_Click(object sender, EventArgs e)
        {
           if (Ra.RecordFound == true)
           {
               MessageBox.Show("ACtion with Id: " + Ra.ReplOrderNo.ToString() + " Will be reviewed!"); 
           }

            // Process No Updating
            //
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ProcessNo = 9;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator,  WAtmNo, WSesNo);
            NForm51.FormClosed += NForm51_FormClosed;
            NForm51.ShowDialog(); 

        }

        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form50_Load(this, new EventArgs());
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

                while (I < (Rc.dtRDays.Rows.Count ))
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

                    if (WorkingDay == true ) WorkingDaysTotal = WorkingDaysTotal + 1; 

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
           

        // SHOW THE ATMS CHOSEN 
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
            string ForGroup =""; 

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
     
       
        // ON FORM CLOSING GO BACK TO MAIN
        private void Form50_FormClosed(object sender, FormClosedEventArgs e)
        {
      //      UsersAndSignedRecord Xa = new UsersAndSignedRecord(); // Make class availble 
       //     Xa.ReadUsersRecord(WSignedId); // READ INFORMATION 

      //      NFormMainScreen = new FormMainScreen(WSignedId, WSignRecordNo, WBankId, WPrive, Xa.SecLevel);
       //     NFormMainScreen.Show();
            
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

            Ra.ReadReplActionsForSpecificDate(WAtmNo, DateTime.Today);

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

                    Ra.ReplCycleNo = WSesNo; 

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
                    Ra.InsuredAmount = 0;
                    Ra.SystemAmount = 0;
                    Ra.NewAmount = InTotal;
                   
                    Ra.Cassette_1 = InTot_51; // Number of notes 
                    Ra.Cassette_2 = InTot_52;
                    Ra.Cassette_3 = InTot_53;
                    Ra.Cassette_4 = InTot_54;

                    Ra.AtmsStatsGroup = Ac.AtmsStatsGroup;
                    Ra.AtmsReplGroup = Ac.AtmsReplGroup;
                    Ra.AtmsReconcGroup = Ac.AtmsReconcGroup;
                    Ra.DateInsert = DateTime.Now;
                    Ra.AuthUser = Am.AuthUser;
                    Ra.OwnerUser = Am.AuthUser;
                    Ra.CitId = Am.CitId;

                    Ra.ActiveRecord = true;

                    // UPDATE EXISTING 
                    //

                    Ra.UpdateReplActionsForAtm(WAtmNo, Ra.ReplOrderNo);

                 
                    //Am.EstReplDt = InNextRepDtTm;

                    //Am.NextReplDt = InNextRepDtTm;

                    //Am.UpdateAtmsMain(WAtmNo); // Update

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

                //+"[InsuredAmount],[SystemAmount],[NewAmount],"

                Ra.InsuredAmount = 0;
                Ra.SystemAmount = 0;
                Ra.NewAmount = InTotal;

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
                Ra.AuthUser = Am.AuthUser;
                Ra.OwnerUser = Am.AuthUser;
                Ra.CitId = Am.CitId;

                Ra.ActiveRecord = true;
                Ac.ReadAtm(WAtmNo);
                Ra.Operator = Ac.Operator;

                WActionNo= Ra.InsertReplOrdersTable(WAtmNo);

            }

        }

         // CALCULATE WHAT NOTES TO INSERT 
        private void CalculateAddNotes(decimal InAmount, int InFunction)
        {
            // SINGLE CURRENCY  

            Ac.ReadAtm(WAtmNo);

            // ALL FOUR EQUAL TO DEPOSIT CURRENCY CODE = EUR
            //if (Na.Cassettes_1.CurName == Ac.DepCurNm & Na.Cassettes_2.CurName == Ac.DepCurNm &
            //        Na.Cassettes_3.CurName == Ac.DepCurNm & Na.Cassettes_4.CurName == Ac.DepCurNm)
            //{
                if (InFunction == 2)
                {
                    Tot_51 = Convert.ToInt32(InAmount * Ac.CasCapacity_11 / 100 / Ac.FaceValue_11);
                    Tot_52 = Convert.ToInt32(InAmount * Ac.CasCapacity_12 / 100 / Ac.FaceValue_12);
                    Tot_53 = Convert.ToInt32(InAmount * Ac.CasCapacity_13 / 100 / Ac.FaceValue_13);
                    Tot_54 = Convert.ToInt32(InAmount * Ac.CasCapacity_14 / 100 / Ac.FaceValue_14);
                }

                // FINAL CORRECTED TOTAL 

                FinalCorrectedReplTotal = Tot_51 * Na.Cassettes_1.FaceValue + Tot_52 * Na.Cassettes_2.FaceValue
                                     + Tot_53 * Na.Cassettes_3.FaceValue + Tot_54 * Na.Cassettes_4.FaceValue;
         
        }
        // Finish
       

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            
            FinishPressed = true;
            OutstandingFound = false; 
            Form50_Load(this, new EventArgs()); // Make validation than all done 
            if (OutstandingFound == true)
            {
                if (MessageBox.Show("ATM = " + WAtmNo + " Not all actions are created. Do you want to close form?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
                {
                    // YES
                    this.Dispose();
                }
                else
                {
                    return;
                }
            }
            else
            {
                this.Dispose(); 
            }
           
        }     

    }
}

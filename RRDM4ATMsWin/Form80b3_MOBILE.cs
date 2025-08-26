using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80b3_MOBILE : Form
    {

        RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
        RRDMMatchingTxns_InGeneralTables_EGATE M_EGATE = new RRDMMatchingTxns_InGeneralTables_EGATE();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDM_ReversalsTable Rev = new RRDM_ReversalsTable(); 

        RRDMMatchingCategories Rc = new RRDMMatchingCategories();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        //RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();

        RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMActions_Occurances Aoc = new RRDMActions_Occurances(); 

        RRDMCaseNotes Cn = new RRDMCaseNotes();



        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;

        bool FromReversals ;

        bool FromMaster;

        bool FirstCycle; 

        // NOTES END 

        string WSortCriteria;


        int WUniqueNo;
        string WRRN;

        string WSelectionCriteria;

        //int WUniqueTraceNo;

        DateTime FromDt;
        DateTime ToDt;

        string WFilterFinal;
        string WMask;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
      //  string W_Application; 
        DateTime WDateTimeA;
        DateTime WDateTimeB;
        string WAtmNo;

        string WCategoryId;

        //bool ViewCatAndCycle; 

        int WRMCycleNo;

        DateTime WCut_Off_Date;
        DateTime HST_DATE;

        bool Is_In_HST_Mobile;

        string WStringUniqueId;
        int WIntUniqueId;
        int WType;
        string WFunction;
        string WIncludeNotMatchedYet;
        int WReplCycle;
        DateTime WSettlementDate;
        string WCategoriesCombined;
        string WGL_AccountNo;
        string W_Application; 

        public Form80b3_MOBILE(string InSignedId, int InSignRecordNo, string InOperator, string InApplication ,DateTime InDateTimeA,
                                                 DateTime InDateTimeB, string InAtmNo, string InCategoryId, int InRMCycleNo,
                                                 string InStringUniqueId, int InIntUniqueId, int InType,
                                                 string InFunction, string InIncludeNotMatchedYet, int InReplCycle, 
                                                 DateTime InSettlementDate, string InCategoriesCombined, string InGL_AccountNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            W_Application = InApplication;

            WDateTimeA = InDateTimeA;
            WDateTimeB = InDateTimeB;
            WAtmNo = InAtmNo;

            WCategoryId = InCategoryId;
            WRMCycleNo = InRMCycleNo;

            WStringUniqueId = InStringUniqueId;
            WIntUniqueId = InIntUniqueId;

            WType = InType;

            WSettlementDate = InSettlementDate;
            WCategoriesCombined = InCategoriesCombined; 
            WGL_AccountNo = InGL_AccountNo;

            

            // 16 = Category and Cycle

            // 17 = Auditors Report 

            // 18 = Outstanding Actions this Cycle

            // 19 = Discrepancies this cycle

            // 20 = Show certain Mask and Certain Category 

            // 21 = Show certain Mask all 

            // 25 = Show actions taken today 

            // 26 = Outstanding Actions ALL Cycles 

            // 27 = Show Actions taken for all Cycles 

            // 28 = Show Discrepancies for 123 Settlement 

            // 29 = Show Matched for 123 Settlement 

            // 30 = Create tab delimeter file from Master 

            // 75 = Category and Cycle ETI375 // THIS IS FOR IRAQ SWITCH

            WFunction = InFunction; // "View"      

            WIncludeNotMatchedYet = InIncludeNotMatchedYet;
            // if "1" = Include not matched yet
            // if ""  = Do not include not matched yet
            //
            WReplCycle = InReplCycle; // This is for WIntUniqueId = 8 

            InitializeComponent();

            if ( (WCategoryId == "EGA375" & WType == 16) ) // THIS IS FOR IRAQ Switch // EXCEPTION 
            {

                // Turn the 16 to 75
                WType = 75; 

                string TempMerchant = ""; // Blank
                //RRDMMatchingTxns_InGeneralTables_EGATE M_EGATE = new RRDMMatchingTxns_InGeneralTables_EGATE();
                M_EGATE.ReadTablebyCycleAndCategory_EGA375(WCategoryId, TempMerchant, WRMCycleNo, W_Application);

                labelMerchant.Show();
                textBox_Merchant.Show();
                linkLabelToAnalysis.Show();

                //textBox_Merchant.Text = Mmob.RRNumber;

                // Grand Totals 
                textBox_Salaries_Amt.Show();
                textBox_Merchants_Amt.Show();
                textBox_Diff.Show();
                textBox_umatched_Settled.Show();
                textBox_Not_settled.Show();
                labelSalary.Show();
                label1.Show();
                label6.Show();
                label7.Show();
                label9.Show();

                textBox_Salaries_Amt.Text = M_EGATE.TotalAmount_1.ToString("#,##0.000");
                textBox_Merchants_Amt.Text = M_EGATE.TotalAmount_2.ToString("#,##0.000");
                decimal TempDiff = M_EGATE.TotalAmount_1 - M_EGATE.TotalAmount_2; 
                textBox_Diff.Text = TempDiff.ToString("#,##0.000");
                textBox_umatched_Settled.Text = M_EGATE.TotalNumber_3.ToString();
                textBox_Not_settled.Text = M_EGATE.TotalNumber_4.ToString();
            }
            else
            {
                labelMerchant.Show();
                textBox_Merchant.Show();
                linkLabelToAnalysis.Show();

                linkLabelToAnalysis.Hide();

                textBox_Salaries_Amt.Hide();
                textBox_Merchants_Amt.Hide();
                textBox_Diff.Hide();
                textBox_umatched_Settled.Hide();
                textBox_Not_settled.Hide();
                labelSalary.Hide();
                label1.Hide();
                label6.Hide();
                label7.Hide();
                label9.Hide();
            }

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            labelCycleNo.Text = WRMCycleNo.ToString();


            Rjc.ReadReconcJobCyclesById(WOperator, WRMCycleNo);

            WCut_Off_Date = Rjc.Cut_Off_Date;

            // FIND HISTORY DATE
            Is_In_HST_Mobile = false;
            string ParamId = "853";
            string OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true & Gp.OccuranceNm != "")
            {

                if (DateTime.TryParseExact(Gp.OccuranceNm, "yyyy-MM-dd", CultureInfo.InvariantCulture
                              , System.Globalization.DateTimeStyles.None, out HST_DATE))
                {
                    //textBoxHST_DATE.Text = HST_DATE.ToShortDateString();

                    //textBoxHST_DATE.Show();
                    //label_HST.Show();

                    if (WCut_Off_Date.Date <= HST_DATE)
                    {
                        Is_In_HST_Mobile = true;
                    }

                }
            }
            else
            {
                //textBoxHST_DATE.Hide();
                //label_HST.Hide();
            }

            if (WType == 16 || WType == 75)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "View Txns of Category =_" + WCategoryId
                                  + "_And Cycle =_" + WRMCycleNo.ToString();
                }

                WSelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId
                                        + "' AND MatchingAtRMCycle=" + WRMCycleNo;

                WSortCriteria = " ORDER BY Matched  ";

                FromReversals = false;

                FirstCycle = true;

            }
            if (WType == 17)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "Auditors UnMatched and Presenter for Cycle =_" + WRMCycleNo.ToString();
                }

                FromReversals = false;
                FromMaster = true;
                WSelectionCriteria = " WHERE "
                                  + "  (IsMatchingDone = 1 AND Matched = 0) OR (IsMatchingDone = 1 AND ActionByUser = 1 AND ActionType <> '00') "
                                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;

      //                ,[IsMatchingDone]
      //,[Matched]
      //,[MatchMask]
      //,[MatchingAtRMCycle]
        WSortCriteria = " ORDER BY MatchingCateg, TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
               // buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }
            // Outstanding This Cycle
            if (WType == 18)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "UnMatched without actions yet for Cycle =_" + WRMCycleNo.ToString();
                }

                FromReversals = false;
                FromMaster = true;
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND (Matched = 0   AND ActionType = '00') "
                                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;


                WSortCriteria = " ORDER BY MatchingCateg, TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }
            // outstanding all Cycles 
            if (WType == 26)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "UnMatched without actions yet for all Cycles " ;
                }

                FromReversals = false;
                FromMaster = true;
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND (Matched = 0   AND ActionType = '00') "
                                  + " ";


                WSortCriteria = " ORDER BY MatchingAtRMCycle, MatchingCateg, TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }

            if (WType == 19)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "Discrepancies this Cycle =_" + WRMCycleNo.ToString();
                }

                FromReversals = false;
                FromMaster = true;
                if (W_Application == "ETISALAT")
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND MatchingCateg in ('ETI310', 'ETI320') AND (Matched = 0 OR (Matched=1 and ActionByUser = 1 AND ActionType <> '00'))"
                                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;

                if (W_Application == "QAHERA")
                    WSelectionCriteria = " WHERE "
                                      + "  IsMatchingDone = 1 AND MatchingCateg in ('QAH310', 'QAH320') AND (Matched = 0 OR (Matched=1 and ActionByUser = 1 AND ActionType <> '00'))"
                                      + "  AND MatchingAtRMCycle=" + WRMCycleNo;

                if (W_Application == "EGATE")
                    WSelectionCriteria = " WHERE "
                                      + "  IsMatchingDone = 1 AND Matched = 0 "
                                      + "  AND MatchingAtRMCycle=" + WRMCycleNo;

                WSortCriteria = " ORDER BY MatchingCateg, TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }

            if (WType == 28)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "Discrepancies for.."+ WCategoriesCombined + " And Settlement.." + WSettlementDate.ToShortDateString();
                }
                //            WHERE IsMatchingDone = 1 AND Matched = 0
                //            AND MatchingCateg in  ('BDC210', 'BDC211') AND CAP_DATE = '2020-04-29'
                // WSettlementDate = InSettlementDate;
                //WCategoriesCombined = InCategoriesCombined;
                //WGL_AccountNo = InGL_AccountNo;
                FromReversals = false;
                FromMaster = true;
                //WSelectionCriteria = " WHERE "
                //                  + "  IsMatchingDone = 1 AND Matched = 0 "
                //                  + "  AND MatchingCateg in "+ WCategoriesCombined + " AND SET_DATE =@SET_DATE " ;
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND Matched = 0 "
                                  + "  AND MatchingCateg in " + WCategoriesCombined + " AND MatchingAtRMCycle=  "+ WRMCycleNo;

                WSortCriteria = " ORDER BY MatchingCateg, TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }

            if (WType == 20)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "Certain Mask for Category and Cycle =_" + WRMCycleNo.ToString();
                }

                FromReversals = false;
                FromMaster = true;
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND Matched = 0 AND MatchMask ='"+ WStringUniqueId +"'"
                                  + " AND MatchingCateg = '" + WCategoryId + "'"
                                   + "  AND MatchingAtRMCycle=" + WRMCycleNo;


                WSortCriteria = " ORDER BY  TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }

            if (WType == 21)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "Certain Mask for  Cycle =_" + WRMCycleNo.ToString();
                }

                FromReversals = false;
                FromMaster = true;
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND Matched = 0 AND MatchMask ='" + WStringUniqueId + "'"
                                 // + " AND MatchingCateg = '" + WCategoryId + "'"
                                   + "  AND MatchingAtRMCycle=" + WRMCycleNo;


                WSortCriteria = " ORDER BY TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }

            if (WType == 25)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "Actions Taken this Cycle =_" + WRMCycleNo.ToString();
                }

                FromReversals = false;
                FromMaster = true;
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND  ActionType <> '00' "
                                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;

                WSortCriteria = " ORDER BY MatchingCateg,  TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }

            if (WType == 27)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "Actions Taken for ALL Cycles";
                }

                FromReversals = false;
                FromMaster = true;
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND (Matched = 0 OR (Matched=1 and  ActionType <> '00'))  "
                                  + "  ";

                WSortCriteria = " ORDER BY MatchingAtRMCycle, MatchingCateg,  TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }


        }
        // Load 
        private void Form80b_Load(object sender, EventArgs e)
        {

            // FILL TABLE AND SHOW // Only this Categ and Cycle both
            textBoxMsgBoard.Text = "Make your selection ";

            //No Dates Are selected

            FromDt = NullPastDate;
            ToDt = NullPastDate;

            int Mode = 1;
            if (FromReversals == true)
            {
                FromMaster = false;
                //WSelectionCriteria = " Not defined yet "; 
                // Rev.ReadReversalsAndFillTable(WSignedId, WSelectionCriteria);

                Mmob.ReadMatchingTxnsMobileMasterAndFillTableGeneral(WOperator, WSignedId, WSelectionCriteria,
                                      WSortCriteria, 2, FirstCycle, NullPastDate, WType, W_Application);

                //dataGridView1.DataSource = Rev.ReversalsDataTable.DefaultView;
                dataGridView1.DataSource = Mmob.MatchingMasterDataTableGeneral_Mobile.DefaultView;

                //ShowGrid_1();


            }
            else
            {

                FromMaster = true;

                if (WType == 25 || WType == 35)
                {
                    // Create
                    // Table to show also two working tables
                    if (WType == 25)
                    {
                        Aoc.ReadActionsOccurancesAndFillTable_Small_Manual_AND_Other_Info(WSignedId, WRMCycleNo, "", NullPastDate, NullPastDate, WType);
                        dataGridView1.DataSource = Aoc.TableActionOccurances_Small.DefaultView;
                    }
                    if (WType == 35)
                    {
                        string WSelection = WCategoryId; 
                        Aoc.ReadActionsOccurancesAndFillTable_Small_Manual_AND_Other_Info(WSignedId, WRMCycleNo, WSelection, WDateTimeA, WDateTimeB, WType);
                        dataGridView1.DataSource = Aoc.TableActionOccurances_Small.DefaultView;
                    }


                }
                else
                {
                    if (WType == 28 || WType == 29)
                    {
                        Mpa.ReadMatchingTxnsMasterPoolAndFillTableGeneral(WOperator, WSignedId, WSelectionCriteria,
                                                                                                            WSortCriteria, 2, FirstCycle, WSettlementDate , WType);

                    }
                    else
                    {
                        if (CreateCsv == true)
                        {
                           
                            string FileA = W_Application + ".[dbo]." + W_Application + "_TPF_TXNS_MASTER";
                            string FileB_Matched = W_Application + "_MATCHED_TXNS.[dbo]." + W_Application + "_TPF_TXNS_MASTER";
                            string WSelectionCriteria_2 = WSelectionCriteria; // Get it 
                            string FileToBeCreated = labelStep1.Text;

                            int DB_Mode = 2;

                            //ReadMaster_MOBILE_By_SelectionCriteriaAnd_CREATE_CSV(string InFileA, string InFileB_Matched, string InSelectionCriteria, int In_DB_Mode)
                            Mmob.ReadMaster_MOBILE_By_SelectionCriteriaAnd_CREATE_CSV(FileA, FileB_Matched, WSelectionCriteria_2, FileToBeCreated, DB_Mode);
                            

                            MessageBox.Show("A Tap delimeter File is created in RRDM working directory" + Environment.NewLine
                                                          + FileToBeCreated
                                                        );
                            

                        }
                        else
                        {
                                                                                           
                            if (Is_In_HST_Mobile == false)
                            {
                                if (WType == 75)
                                {
                                    // FOR EGA375
                                    
                                    M_EGATE.ReadMatchingTxnsMobileMasterAndFillTableGeneral_2(WOperator, WSignedId, WSelectionCriteria,
                                    WSortCriteria, 2, FirstCycle, NullPastDate, WType, W_Application);
                                }
                                else
                                {
                                    if (W_Application == "EGATE")
                                    {
                                        //RRDMMatchingTxns_InGeneralTables_EGATE M_EGATE = new RRDMMatchingTxns_InGeneralTables_EGATE();
                                        M_EGATE.ReadMatchingTxnsMobileMasterAndFillTableGeneral(WOperator, WSignedId, WSelectionCriteria,
                                      WSortCriteria, 2, FirstCycle, NullPastDate, WType, W_Application);
                                    }
                                    else
                                    {
                                        Mmob.ReadMatchingTxnsMobileMasterAndFillTableGeneral(WOperator, WSignedId, WSelectionCriteria,
                                      WSortCriteria, 2, FirstCycle, NullPastDate, WType, W_Application);
                                    }
                                    
                                }
                               
                               
                            }
                            if (Is_In_HST_Mobile == true)
                            {
                                Mmob.ReadMatchingTxnsMobileMasterAndFillTableGeneral(WOperator, WSignedId, WSelectionCriteria,
                                       WSortCriteria, 2, FirstCycle, NullPastDate, WType, W_Application);
                            }
                        }
                    }

                    if (W_Application == "EGATE")
                    {
                        dataGridView1.DataSource = M_EGATE.MatchingMasterDataTableGeneral_Mobile.DefaultView;
                    }
                    else
                    {
                        dataGridView1.DataSource = Mmob.MatchingMasterDataTableGeneral_Mobile.DefaultView;
                    }  
                }

                

                if (FirstCycle == true)
                {
                    textBoxMsgBoard.Text = "Only up to 1,000 is shown. Select option to get all "; 
                    FirstCycle = false;
                }
                 
            }
   
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No transactions for this selection");

                panel21.Hide();

                textBoxLines.Text = dataGridView1.Rows.Count.ToString();
                //this.Dispose();
               // return;
            }
            else
            {
                // panel7.Show();
                panel21.Show();
                //   label11.Show();
                //  textBoxMask.Show();
                textBoxLines.Text = dataGridView1.Rows.Count.ToString();

                if (FromMaster == true)
                {
                    if (WType == 25)
                    {
                        // correct this 
                        ShowGrid_25();
                    }
                    else
                    {
                        if (WType!=75)
                        {
                            ShowGrid_1();
                        }
                        else
                        {
                            ShowGrid_75();
                        }
                        
                    }
                        
                }
                else
                {
                    // Coming from reversals
                    ShowGrid_1();
                }
                
            }

        }

       

        // On ROW ENTER 
        int WWUniqueRecordId;
        bool IsReversal;
        bool JournalFound;
        string Message;
        int WRevSeqNo;
        string WFileId;
        int WSeqNo;
        string WMatchingCateg; 

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //if (W_Application== "" || )
            //{
                WSeqNo = (int)rowSelected.Cells[0].Value;
                WFilterFinal = " Where  SeqNo = " + WSeqNo;
                if (W_Application == "EGATE")
            {
                M_EGATE.ReadMaster_MOBILE_By_SelectionCriteria(WFilterFinal, 2, W_Application);
                WMatchingCateg = M_EGATE.MatchingCateg;
            }
            else
            {
                Mmob.ReadMaster_MOBILE_By_SelectionCriteria(WFilterFinal, 2, W_Application);
                WMatchingCateg = Mmob.MatchingCateg;
            }
                
   
            if (WMatchingCateg == "EGA375")
            {
                textBox_Merchant.Text = M_EGATE.RRNumber; 
            }

           
            

            // NOTES START  
            Order = "Descending";
                WParameter4 = "SeqNo:" + Mmob.SeqNo;
                WSearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
                if (Cn.RecordFound == true)
                {
                    labelNumberNotes2.Text = Cn.TotalNotes.ToString();
                }
                else labelNumberNotes2.Text = "0";

                // NOTES END

               // WWUniqueRecordId = Mpa.UniqueRecordId;

                


                ////  textBoxMask.Text = Mpa.MatchMask;
                //// Check for exceptions 
                //if (Mpa.MetaExceptionNo > 0)
                //{
                //    label3.Show();
                //    panel3.Show();

                //    Ec.ReadErrorsTableSpecific(Mpa.MetaExceptionNo);

                //    textBoxExceptionNo.Text = Mpa.MetaExceptionNo.ToString();

                //    textBoxExceptionDesc.Text = Ec.ErrDesc;
                //}
                //else
                //{
                //    label3.Hide();
                //    panel3.Hide();
                //}

                labelReversal.Hide();

                Tp.ReadTransToBePostedSpecificByUniqueRecordId(Mpa.UniqueRecordId);
                if (Tp.RecordFound == true)
                {
                    label27.Show();
                    panel8.Show();

                    textBoxCreated.Text = Tp.OpenDate.ToString();
                    if (Tp.ActionDate != NullPastDate)
                    {
                        textBoxPosted.Text = Tp.ActionDate.ToString();
                        Tp.ReadTransToBePostedSpecificByUniqueRecordIdForReversal(Mpa.UniqueRecordId);
                        if (Tp.IsReversal == true) labelReversal.Show();
                        else labelReversal.Hide();
                    }
                    else textBoxPosted.Text = "Not Posted yet.";
                }
                else
                {
                    label27.Hide();
                    panel8.Hide();
                }

                if (Mpa.ActionType == "04")
                {
                    label27.Show();
                    panelForceMatched.Show();
                    textBoxForceReason.Text = Mpa.MatchedType;

                }
                else
                {
                    panelForceMatched.Hide();
                }

                //if (Mpa.Matched == false)
                //{
                //    if (Message == "")
                //        textBoxUnMatchedType.Text = Mpa.UnMatchedType;
                //    else textBoxUnMatchedType.Text = Message;

                //}
                //else textBoxUnMatchedType.Text = "Matched";


              
               WMask = Mmob.MatchMask;
                


                WRRN = Mpa.RRNumber;

                // Check if dispute already registered for this transaction 

                RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
                // Check if already exist
                Dt.ReadDisputeTranByUniqueRecordId(Mpa.UniqueRecordId);
                if (Dt.RecordFound == true)
                {
                    textBox9.Text = Dt.DisputeNumber.ToString();
                    linkLabelDispute.Show();
                    //WFoundDisp = Dt.DisputeNumber;
                    if (WFunction == "Investigation" & WWUniqueRecordId == 0)
                        MessageBox.Show("Dispute with no : " + Dt.DisputeNumber.ToString() + " already registered for this transaction.");
                    labelDispute.Show();
                    textBox9.Show();
                    //    buttonRegisterDispute.Hide();
                }
                else
                {
                    labelDispute.Hide();
                    textBox9.Hide();
                    linkLabelDispute.Hide();

                }
            }
            //else
            //{
            //    // COMING FROM REVERSAL 

            //    WRevSeqNo = (int)rowSelected.Cells[0].Value;
            //    string SelectionCriteria = " Where  SeqNo = " + WRevSeqNo;

            //    Rev.ReadReversalsBy_Selection_criteria(SelectionCriteria);

            //    WFileId = Rev.FileId;
            //    WCategoryId = Rev.MatchingCateg; 
            //}
            

        //    //textBoxInputField.Text = Mpa.CardNumber;
        //}


        public void ShowGrid_1()
        {
            if (W_Application == "EGATE")
            {
                if (M_EGATE.MatchingMasterDataTableGeneral_Mobile.Rows.Count == 0)
                {
                    MessageBox.Show("Nothing To Show");

                    this.Dispose();
                    return;
                }
            }
            else
            {
                if (Mmob.MatchingMasterDataTableGeneral_Mobile.Rows.Count == 0)
                {
                    MessageBox.Show("Nothing To Show");

                    this.Dispose();
                    return;
                }
            }
            
            

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 60; // RecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].HeaderText = "Record Id";

            dataGridView1.Columns[1].Width = 140; //   Descr
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 120; //   Customer Id 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 50; //   Action
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 40; // Ccy
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 90; // amount
            dataGridView1.Columns[5].DefaultCellStyle = style;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[6].Width = 90; // Billing Amt 
            dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[7].Width = 50; //   Type 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[7].Visible = true;

            dataGridView1.Columns[8].Width = 90; // Date 
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[9].Width = 100; //RRNumber 
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 90; //Reference PTF
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[11].Width = 40; // MASK
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[12].Width =50; // Settled 
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[13].Width = 50; // Action Type 
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[14].Width = 130; //Action Description
            //dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[15].Width = 70; // Maching Category
            //dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[16].Width = 80; // Maker
            //dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[17].Width = 80; // Checker
            //dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        public void ShowGrid_75()
        {

            if (M_EGATE.MatchingMasterDataTableGeneral_Mobile.Rows.Count == 0)
            {
                MessageBox.Show("Nothing To Show");

                this.Dispose();
                return;
            }

            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("RecordId", typeof(int));

            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("MatchingCateg", typeof(string));
            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("RRNumber", typeof(string));

            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("Ccy", typeof(string));
            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("Amount_Salaries", typeof(decimal));
            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("Amount_Merchant", typeof(decimal));
            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("Amount_Difference", typeof(decimal));

            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("Date", typeof(DateTime));

            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("MatchMask", typeof(string));

            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("Settled", typeof(bool));

            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("ActionType", typeof(string));
            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("ActionDesc", typeof(string));

            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("Maker", typeof(string));
            //MatchingMasterDataTableGeneral_Mobile.Columns.Add("Author", typeof(string)

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 60; // RecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].HeaderText = "Record Id";

            dataGridView1.Columns[1].Width = 70; //   MatchingCateg
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 110; // RRNumber
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].HeaderText  = "MERCHANT ID";
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[3].Width = 50; // Ccy
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[2].Name = "MERCHANT ID";

            dataGridView1.Columns[4].Width = 150; // amount Salaries
            dataGridView1.Columns[4].DefaultCellStyle = style;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 150; // amount Merhant 
            dataGridView1.Columns[5].DefaultCellStyle = style;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[6].Width = 150; // amount Difference  
            dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[7].Width = 90; // Date 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 50; // MASK
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 40; // Settled
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 50; // Action Type 
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 130; //Action Description
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[12].Width = 80; // Maker
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[13].Width = 80; // Checker
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        public void ShowGrid_25()
        {
            
          
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].HeaderText = "Rec No";

            dataGridView1.Columns[1].Width = 40; // ActionId   
            dataGridView1.Columns[1].HeaderText = "Action Id";
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 40; // Occurance
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 100; // ActionNm
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 70; // ActionReason
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 70; // ActionDateTime
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 40; // MatchMask
            dataGridView1.Columns[6].HeaderText = "MASK";
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 70; // Trans_Descr
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 50; // Terminal
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[9].Width = 60; // CardNo
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 60; // AccNo
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[11].Width = 70; // Amount
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[12].Width = 40; // TraceNo
            dataGridView1.Columns[12].HeaderText = "Trace No";
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[13].Width = 50; // RRNumber
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 70; // TransDate
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[15].Width = 40; // Maker
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[16].Width = 40; // Author
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[17].Width = 60; // MatchingCateg
            dataGridView1.Columns[17].HeaderText = "Matching Categ";
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[18].Width = 60; // MatchingCycle
            dataGridView1.Columns[18].HeaderText = "Match at Cycle";
            dataGridView1.Columns[18].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[19].Width = 40; // TXNSRC
            dataGridView1.Columns[19].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 

            dataGridView1.Columns[20].Width = 40; // TXNDEST
            dataGridView1.Columns[20].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[21].Width = 100; // UserId
            dataGridView1.Columns[21].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }


        public void ShowGrid_2()
        {
  
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";
          
            dataGridView1.Columns[0].Width = 60; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].HeaderText = "Record Id";
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 60; // RevSeqNo1     
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = true;
            dataGridView1.Columns[1].HeaderText = "Record Id";

            dataGridView1.Columns[2].Width = 70; // Terminal
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 100; // Descr
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
            dataGridView1.Columns[4].Width = 70; // Card
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 70; // Account
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 40; // Ccy
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 80; // Amount
            dataGridView1.Columns[7].DefaultCellStyle = style;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[7].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[8].Width = 70; // Date
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; 

            dataGridView1.Columns[9].Width = 70; // TraceNo
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 70; // RRNumber
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[11].Width = 40; // RespCode
            dataGridView1.Columns[11].HeaderText = "Resp Code1";
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[12].Width = 60; // FileId
            dataGridView1.Columns[12].HeaderText = "File Id";
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[13].Width = 70; // Category
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 60; // RevSeqNo2
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[14].HeaderText = "Rev SeqNo2";

            dataGridView1.Columns[15].Width = 40; // RevResp
            dataGridView1.Columns[15].HeaderText = "Resp Code2";
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            //MatchingMasterDataTableATMs.Columns.Add("Settled", typeof(bool));

            //MatchingMasterDataTableATMs.Columns.Add("ActionType", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("ActionDesc", typeof(string));
            //dataGridView1.Columns[11].Width = 90; // Card
            //dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    //WSeqNo = (int)rowSelected.Cells[0].Value;
            //    bool WSelect = (bool)row.Cells[1].Value;
            //    string WActionType = (string)row.Cells[3].Value;

            //    if (WSelect == true & WActionType == "04")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }
            //    if (WSelect == true & WActionType == "01")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Beige;
            //        row.DefaultCellStyle.ForeColor = Color.Black;

            //    }
            //    if (WSelect == false)
            //    {
            //        row.DefaultCellStyle.BackColor = Color.White;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }

            //}
        }


        // Show Exception 
        private void buttonShowException_Click(object sender, EventArgs e)
        {
            Form24 NForm24;
            bool Replenishment = true;
            int ErrNo = Mpa.MetaExceptionNo;
            string SearchFilter = "ErrNo =" + ErrNo;
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo, "", Replenishment, SearchFilter);
            NForm24.ShowDialog();
        }
        // Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "SeqNo:" + Mmob.SeqNo;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "SeqNo:" + WSeqNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            //SetScreen();
        }
        
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print 
            if (FromMaster == true)
            {
                //if (WType == 16)
                //{
                //    Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

                //    string P1 = labelStep1.Text; //  heading 
                //    string P2 = "Second Par";
                //    string P3 = "Third Par";
                //    string P4 = Us.BankId;
                //    string P5 = WSignedId;

                //    Form56R55_2 ReportATMS55_2 = new Form56R55_2(P1, P2, P3, P4, P5);
                //    ReportATMS55_2.Show();

                //}
                if (WType == 16 || WType == 17 || WType == 18 || WType == 19 || WType == 28 || WType == 29)
                {
                    Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

                    string P1 = labelStep1.Text; //  heading 
                    string P2 = "Second Par";
                    string P3 = "Third Par";
                    string P4 = Us.BankId;
                    string P5 = WSignedId;

                    Form56R55_4 ReportATMS55_4 = new Form56R55_4(P1, P2, P3, P4, P5);
                    ReportATMS55_4.Show();
                }

                if (WType == 25)
                {
                    Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

                    string P1 = labelStep1.Text; //  heading 
                    string P2 = "Second Par";
                    string P3 = "Third Par";
                    string P4 = Us.BankId;
                    string P5 = WSignedId;

                    Form56R55_5 ReportATMS55_5 = new Form56R55_5(P1, P2, P3, P4, P5);
                    ReportATMS55_5.Show();
                }

            }
            else
            {
                // NOT FROM Mpa .... REVERSALS 
                Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

                string P1 = labelStep1.Text; //  heading 
                string P2 = "Second Par";
                string P3 = "Third Par";
                string P4 = Us.BankId;
                string P5 = WSignedId;

                Form56R55_3 ReportATMS55_3 = new Form56R55_3(P1, P2, P3, P4, P5);
                ReportATMS55_3.Show();
            }
           
        }


        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // leave it here 
        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form80b_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }


        // EXPAND GRID
        //private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    //Form78b NForm78b;
        //    //string WHeader = "LIST OF TRANSACTIONS";
        //    //NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mpa.MatchingMasterDataTableATMs, WHeader, "Form80b");
        //    //NForm78b.FormClosed += NForm78b_FormClosed;
        //    //NForm78b.ShowDialog();
        //}
        // Show Trans to Be posted 
        private void buttonTranPosted_Click(object sender, EventArgs e)
        {
            Form78 NForm78;


            if (Mpa.MetaExceptionNo > 0)
            {
                NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator,
                                               "", 0, Mpa.MetaExceptionNo, 1);
                NForm78.ShowDialog();
            }
            else
            {
                MessageBox.Show("No Transactions/actions were taken for this.");
                return;
            }


        }
        // Replenishment Play 
        private void buttonReplPlay_Click(object sender, EventArgs e)
        {
            Form51 NForm51;
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            //
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54; // View only for replenishment already done  
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Ta.ReadSessionsStatusTraces(Mpa.TerminalId, Mpa.ReplCycleNo);
            if (Ta.RecordFound & Ta.ProcessMode > 0)
            {
                NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                NForm51.ShowDialog();
            }
            else
            {
                MessageBox.Show("No Replenishement done for this ATM and this Replen Cycle");
                return;
            }

        }
        
        // Text From Journal 
        private void button3_Click(object sender, EventArgs e)
        {

            // Show Lines of journal 
            string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            if (Mpa.MatchMask == "001"
                || Mpa.MatchMask == "011"
                || Mpa.MatchMask == "010"
                || Mpa.MatchMask == "01"
                )
            {
                MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
                                 + "Select Journal Lines Near To this"
                                 );
                return;
            }

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            if (Mpa.TraceNoWithNoEndZero == 0)
            {
                MessageBox.Show("No Available Trace to show the Journal Lines for this Txn/Category ");
                return;
            }
            else
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;
                WSeqNoB = Mpa.OriginalRecordId;
            }

            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();


        }
        // Full Journal
        private void button4_Click(object sender, EventArgs e)
        {
            Form67 NForm67;
            RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            String JournalId = "";
            int WTraceNo = Mpa.AtmTraceNo;
            int Mode; // FULL

            //if (WOperator == "BCAIEGCX")
            //{
                int WSeqNoA = 0;
                int WSeqNoB = 0;

                Jt.ReadJournalTxnsByParameters(WOperator, Mpa.TerminalId, Mpa.AtmTraceNo*10, Mpa.TransAmount, Mpa.CardNumber, Mpa.TransDate.Date);

                if (Jt.RecordFound)
                {
                    WSeqNoA = Jt.SeqNo;
                    WSeqNoB = Jt.SeqNo;

                }
                else
                {
                    MessageBox.Show("There is no recognisable valid record in Journal" + Environment.NewLine
                                  + "Search in Journals to find the occurance!"
                                );

                    Form200cATMs NForm200cATMs;

                    NForm200cATMs = new Form200cATMs(WSignedId, WSignRecordNo, "5", WOperator, Mpa.TerminalId);
                    NForm200cATMs.ShowDialog();
                }
                //
                // Bank De Caire
                //
                Form67_BDC NForm67_BDC;

                Mode = 3; // Specific Journal 
                string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
                if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
                return;
            //}


            


        }
        // SHOW Video Clip
        private void button5_Click(object sender, EventArgs e)
        {
            // Based on Transaction No show video clip 
            //TEST
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();
        }
        // Register Dispute 
        private void buttonRegisterDispute_Click(object sender, EventArgs e)
        {
            Form5 NForm5;
            int From = 2; // Coming from Pre-Investigattion ATMs 
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Mpa.CardNumber, WWUniqueRecordId, 0, 0, "", From, "ATM");
            NForm5.FormClosed += NForm5_FormClosed;
            NForm5.ShowDialog();
        }

        private void NForm271_FormClosed(object sender, FormClosedEventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80b_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        private void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80b_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }



        // Link to disputes 
        Form3 NForm3;
        private void linkLabelDispute_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator, textBox9.Text, NullPastDate, NullPastDate, 13);
            NForm3.FormClosed += NForm3_FormClosed;
            NForm3.ShowDialog();
        }

        void NForm3_FormClosed(object sender, FormClosedEventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80b_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
        // Source Records 
        private void buttonSourceReords_Click(object sender, EventArgs e)
        {
           
            string  WMasterTableName = W_Application + ".[dbo]." + W_Application + "_TPF_TXNS_MASTER";
            Form78d_AllFiles_BDC_3_MOBILE NForm78d_AllFiles_BDC_3_MOBILE; // 

            NForm78d_AllFiles_BDC_3_MOBILE = new Form78d_AllFiles_BDC_3_MOBILE(WOperator, WSignedId, WSeqNo, WMatchingCateg, 1, WMasterTableName, W_Application);

            NForm78d_AllFiles_BDC_3_MOBILE.ShowDialog();
        }

        // Near Journal 
        int IntTrace;

        private void buttonJournal_Near_Click(object sender, EventArgs e)
        {
            // Here we search the journal lines close to the selected transaction
            // If selected already in Journal then we show the one before and the one after
            // If transaction not in journal say 011 then we show 5 minutes before and 5 minutes after. 
            // This is because the time at mainframe maybe different that this of ATM. 

            string WSubString = WMask.Substring(0, 1);

            DateTime SavedTransDate = Mpa.TransDate;

            if (WSubString == "1")
            {
                IntTrace = Mpa.TraceNoWithNoEndZero;
            }

            bool In_HST = false;

            //if (Mpa.TransDate.Date <= HST_DATE)
            //{
            //    In_HST = true;
            //}
            // Show Lines of journal 
            //string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;
            //if (In_HST == true)
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            //}
            //else
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            //}

            string SelectionCriteria;
            int SaveSeqNo = WSeqNo;

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            //DateTime TestingDate = new DateTime(2019, 01, 03);

            SaveSeqNo = Mpa.SeqNo;

            DateTime WDtA = Mpa.TransDate;
            DateTime WDtB = Mpa.TransDate;
            DateTime WDt;

            WDt = WDtA.AddMinutes(-2);

            // FIND THE LESS
            SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                              + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate < @TransDate  ";
            string OrderBy = "  ORDER By TransDate Desc";

            if (In_HST == true)
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
            }
            else
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
            }

            if (Mpa.RecordFound == true)
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;

                // FIND THE GREATEST that exist 
                WDt = WDtB.AddMinutes(2);
                SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                           + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate > @TransDate ";

                OrderBy = "  ORDER By TransDate ASC ";


                if (In_HST == true)
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
                }
                else
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
                }

                if (Mpa.RecordFound == true)
                {
                    // 
                    WSeqNoB = Mpa.OriginalRecordId; // This is the SeqNo in Pambos Journal  

                }
                else
                {
                    MessageBox.Show("No Upper Limit. No Journal Lines to Show");

                    WSeqNoB = 0;
                    // Reestablish Mpa Data
                    //
                    //SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                    //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                    //return;
                }


            }
            else
            {
                MessageBox.Show("No Lower Limit. No Journal Lines to Show");
                // Reestablish Mpa Data
                //
                SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                if (In_HST == true)
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                }
                else
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
                }

                // Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                return;
            }
            //
            // Reestablish Mpa Data
            //
            SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

            if (In_HST == true)
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            }
            else
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            }

            //   Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific range
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (WSubString == "1")
            {
                WTraceRRNumber = IntTrace.ToString();
            }

            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                      SavedTransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();

        }
        // Show Not Processed yet
        private void buttonNotProcessed_Click(object sender, EventArgs e)
        {
            FromReversals = false;
            FromMaster = true;
            WSelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId
                               + "' AND IsMatchingDone = 0 ";

            WSortCriteria = " ORDER BY TransDate ";

            labelStep1.Text = "View Txns of Category =_" + WCategoryId
                          + "_And Not Processed yet " ;

            Form80b_Load(this, new EventArgs());

        }
// Show UNMATCHED
        private void buttonShowUmatched_Click(object sender, EventArgs e)
        {
            FromReversals = false;
            FromMaster = true; 
            WSelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId
                              + "' AND IsMatchingDone = 1 AND Matched = 0 "
                              + "  AND MatchingAtRMCycle=" + WRMCycleNo  ;
                               

            WSortCriteria = " ORDER BY TransDate ";


            labelStep1.Text = "View Txns of Category =_" + WCategoryId
                          + "_UN-MATCHED ";

            Form80b_Load(this, new EventArgs());
        }
// SHOW MATCHED 
        private void buttonShowMatched_Click(object sender, EventArgs e)
        {
            FromReversals = false;
            FromMaster = true;
            WSelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId
                              + "' AND IsMatchingDone = 1 AND Matched = 1 "
                              + "  AND MatchingAtRMCycle=" + WRMCycleNo ;
                                                      

            WSortCriteria = " ORDER BY TransDate ";


            labelStep1.Text = "View Txns of Category =_" + WCategoryId
                          + "_MATCHED ";

            Form80b_Load(this, new EventArgs());
        }
        // Show Reversals 
       
        private void buttonShowReversals_Click(object sender, EventArgs e)
        {
            // ,[RMCycleNo]
            // ,[MatchingCateg]
            FromReversals = true;
            FromMaster = false; 

            WSelectionCriteria = " WHERE MatchingCateg ='" +WCategoryId + "' AND IsReversal = 1 " 
                                          + "  AND LoadedAtRMCycle=" + WRMCycleNo;
            WSortCriteria = " ORDER BY Reference_TPF ";


            labelStep1.Text = "View Txns of Category =_" + WCategoryId
                          + "_Reversals ";

            Form80b_Load(this, new EventArgs());
        }
// Export to Excel
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 7000)
            {
                MessageBox.Show("Too Many lines for Excel "+Environment.NewLine
                           + "Create a tap delimeter txt file instead. " + Environment.NewLine
                           + ".. then you can easily and effectively import it to excel. " + Environment.NewLine
                    );
                return; 
            }
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelDATE = DateTime.Now.Year.ToString()
                       + DateTime.Now.Month.ToString()
                       + DateTime.Now.Day.ToString()
                       + "_"
                       + DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString()
                        ;

            string ExcelPath ;
            string WorkingDir ;

            if (FromMaster == true)
            { 
                if (WType == 25)
                {
                    ExcelPath = "C:\\RRDM\\Working\\Auditors_" + ExcelDATE + ".xls";
                    WorkingDir = "C:\\RRDM\\Working\\";
                    XL.ExportToExcel(Aoc.TableActionOccurances_Small, WorkingDir, ExcelPath);
                }
                else
                {
                    ExcelPath = "C:\\RRDM\\Working\\Files_MASTER_" + ExcelDATE + ".xls";
                    WorkingDir = "C:\\RRDM\\Working\\";
                    XL.ExportToExcel(Mmob.MatchingMasterDataTableGeneral_Mobile, WorkingDir, ExcelPath);
                }
               
            }
            else
            {
                ExcelPath = "C:\\RRDM\\Working\\Files_REV_" + ExcelDATE + ".xls";
                WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(Mpa.MatchingMasterDataTableATMs, ExcelPath);
            }
            
        }
// ALL UNMATCHED THIS CYCLE 
        private void buttonAllUnmatched_Click(object sender, EventArgs e)
        {
            //FromReversals = false;
            //FromMaster = true;
            //WSelectionCriteria = " WHERE "
            //                  + "  IsMatchingDone = 1 AND Matched = 0 "
            //                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;


            //WSortCriteria = " ORDER BY TransDate ";

            //labelStep1.Text = "View ALL Unmatched Txns for Cycle =_" + WRMCycleNo.ToString();                        

            //Form80b_Load(this, new EventArgs());
        }
        //
        // TAP delimeter file
        //
        bool CreateCsv = false;  

        private void buttonExport_CSV_Click(object sender, EventArgs e)
        {
            FromReversals = false;
            FromMaster = true;
            CreateCsv = true;
            //WSelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId
            //                  + "' AND IsMatchingDone = 1 AND Matched = 1 "
            //                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;


            //WSortCriteria = " ORDER BY TransDate ";


            //labelStep1.Text = "View Txns of Category =_" + WCategoryId
            //              + "_MATCHED ";

            Form80b_Load(this, new EventArgs());
        }
// Analysis Per Agent 
        private void linkLabelToAnalysis_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form78d_MOBILE_2_Grids NForm78d_MOBILE_2_Grids;
            int WMode = 9; // SHOW ETI375
            NForm78d_MOBILE_2_Grids = new Form78d_MOBILE_2_Grids(WOperator, WSignedId, WMatchingCateg, WRMCycleNo, textBox_Merchant.Text, WMode, W_Application);
            NForm78d_MOBILE_2_Grids.ShowDialog();
        }
    }
}


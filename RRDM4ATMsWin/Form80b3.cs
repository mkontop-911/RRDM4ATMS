using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Globalization;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80b3 : Form
    {
        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
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

        string W_Application; 

        int WUniqueNo;
        string WRRN;

        string WSelectionCriteria;

        //int WUniqueTraceNo;

        DateTime FromDt;
        DateTime ToDt;

        DateTime WCut_Off_Date;
        DateTime HST_DATE; 

        bool Is_In_HST; 


        string WFilterFinal;
        string WMask;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        DateTime WDateTimeA;
        DateTime WDateTimeB;
        string WAtmNo;

        string WCategoryId;

        //bool ViewCatAndCycle; 

        int WRMCycleNo;

        string WStringUniqueId;
        int WIntUniqueId;
        int WType;
        string WFunction;
        string WIncludeNotMatchedYet;
        int WReplCycle;
        DateTime WSettlementDate;
        string WCategoriesCombined;
        string WGL_AccountNo; 

        public Form80b3(string InSignedId, int InSignRecordNo, string InOperator, DateTime InDateTimeA,
                                                 DateTime InDateTimeB, string InAtmNo, string InCategoryId, int InRMCycleNo,
                                                 string InStringUniqueId, int InIntUniqueId, int InType,
                                                 string InFunction, string InIncludeNotMatchedYet, int InReplCycle, DateTime InSettlementDate, string InCategoriesCombined, string InGL_AccountNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

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

            // 25 = Show actions taken today = this cycle 

            // 26 = Outstanding Actions ALL Cycles 

            // 27 = Show Actions taken for all Cycles 

            // 28 = Show Discrepancies for 123 Settlement 

            // 29 = Show Matched for 123 Settlement 

            // 30 = Create tab delimeter file from Master 

            // 35 = Show Actions by input selection for a range of dates 

            // 95 = SOLO TXNs From Dispute pre investigation 

            // 96 = SOLO Actions This Cycle 

            WFunction = InFunction; // "View"      

            WIncludeNotMatchedYet = InIncludeNotMatchedYet;
            // if "1" = Include not matched yet
            // if ""  = Do not include not matched yet
            //
            WReplCycle = InReplCycle; // This is for WIntUniqueId = 8 

            InitializeComponent();

            //
            // FIND APPLICATION
            //
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;
                // "ATMS/CARDS"
                // "e_MOBILE"
            }

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            labelCycleNo.Text = WRMCycleNo.ToString();

            // FIND IF IN HISTORY

            //Flog.ReadDataTableFileMonitorLogByCycleNo_For_Not_Loaded(WOperator, WSignedId, WJobCycleNo);

            Rjc.ReadReconcJobCyclesById(WOperator, WRMCycleNo);

            WCut_Off_Date = Rjc.Cut_Off_Date;

            // FIND HISTORY DATE
            Is_In_HST = false;
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
                        Is_In_HST = true;
                    }

                }
            }
            else
            {
                //textBoxHST_DATE.Hide();
                //label_HST.Hide();
            }

            // Show the two Grids on the right

            if (Environment.MachineName == "RRDM-PANICOS")
            {
                // OK Continue 
            }
            else
            {
                // SET it accordingly not to be used for not authorised BDC
                Is_In_HST = false;
            }


            if (WType == 16)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "View Txns of Category =_" + WCategoryId
                                  + "_And Cycle =_" + WRMCycleNo.ToString();
                }

                WSelectionCriteria = " WHERE MatchingCateg ='" + WCategoryId
                                        + "' AND MatchingAtRMCycle=" + WRMCycleNo;

                WSortCriteria = " ORDER BY TransDate ";

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
                                  + "  IsMatchingDone = 1 AND (Matched = 0 OR (Matched=1 and MetaExceptionId = 55) OR (Matched=1 and ActionType <> '00'))"
                                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;


                WSortCriteria = " ORDER BY MatchingCateg, TerminalId, TransDate ";

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
                                  + "  IsMatchingDone = 1 AND (Matched = 0 OR (Matched=1 and MetaExceptionId = 55)) AND ActionType = '00' "
                                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;


                WSortCriteria = " ORDER BY MatchingCateg, TerminalId, TransDate ";

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
                                  + "  IsMatchingDone = 1 AND (Matched = 0 OR (Matched=1 and MetaExceptionId = 55)) AND ActionType = '00' "
                                  + " ";


                WSortCriteria = " ORDER BY MatchingAtRMCycle, MatchingCateg, TerminalId, TransDate ";

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
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND (Matched = 0 OR (Matched=1 and MetaExceptionId = 55))"
                                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;

                WSortCriteria = " ORDER BY MatchingCateg, TerminalId, TransDate ";

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

                WSortCriteria = " ORDER BY MatchingCateg, TerminalId, TransDate ";

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


                WSortCriteria = " ORDER BY TerminalId, TransDate ";

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


                WSortCriteria = " ORDER BY TerminalId, TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }

            if (WType == 25 || WType == 35 || WType == 96)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";
                    if (WType == 25)
                    {
                        labelStep1.Text = "Actions Taken this Cycle =_" + WRMCycleNo.ToString();
                    }
                    if (WType == 35)
                    {
                        labelStep1.Text = "Actions by Selection =_" + InCategoryId; // Eg 91,92 etc
                    }

                    if (WType == 96)
                    {
                        labelStep1.Text = "Actions SOLO Disputes this Cycle =_" + WRMCycleNo.ToString();
                    }

                }

                FromReversals = false;
                FromMaster = true;
                WSelectionCriteria = " WHERE "
                                  + "  IsMatchingDone = 1 AND (Matched = 0 OR (Matched=1 and MetaExceptionId = 55))"
                                  + "  AND MatchingAtRMCycle=" + WRMCycleNo;

                //WSelectionCriteria = " WHERE "
                //                  + "  IsMatchingDone = 1 AND Matched = 0 "
                //                  + "  AND MatchingCateg in " + WCategoriesCombined + " AND MatchingAtRMCycle=  " + WRMCycleNo;

                WSortCriteria = " ORDER BY MatchingCateg, TerminalId, TransDate ";

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
                                  + "  IsMatchingDone = 1 AND (Matched = 0 OR (Matched=1 and MetaExceptionId = 55)) AND ActionType <> '00' "
                                  + "  ";

                WSortCriteria = " ORDER BY MatchingAtRMCycle, MatchingCateg, TerminalId, TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                //buttonAllUnmatched.Hide();
                buttonNotProcessed.Hide();

                FromReversals = false;
                FirstCycle = false; // leave it here 

            }
            if (WType == 95)
            {
                if (WFunction == "View")
                {
                    textBoxMsgBoard.Text = "View Only";

                    labelStep1.Text = "Show SOLO Txns - This Cycle =_" + WRMCycleNo.ToString();
                }
                //Mpa.SeqNo06 = 95;
                //Mpa.SeqNo05 = WReconcCycleNo;
                FromReversals = false;
                FromMaster = true;
                WSelectionCriteria = " WHERE "
                                  + "  SeqNo06 = 95 " // HERE WE KEEP the SOLO 
                                  + "  AND SeqNo05=" + WRMCycleNo;


                WSortCriteria = " ORDER BY MatchingCateg, TerminalId, TransDate ";

                buttonShowMatched.Hide();
                buttonShowUmatched.Hide();
                buttonShowReversals.Hide();
                // buttonAllUnmatched.Hide();
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
                Rev.ReadReversalsAndFillTable(WSignedId, WSelectionCriteria);

                dataGridView1.DataSource = Rev.ReversalsDataTable.DefaultView;

               
            }
            else
            {

                FromMaster = true;

                if (WType == 25 || WType == 35 || WType == 96)
                {
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

                    if (WType == 96)
                    {
                        Aoc.ReadActionsOccurancesAndFillTable_Small_Manual_AND_Other_Info(WSignedId, WRMCycleNo, "", NullPastDate, NullPastDate, WType);
                        dataGridView1.DataSource = Aoc.TableActionOccurances_Small.DefaultView;
                    }
                    // Create
                    // Table to show also two working tables


                }
                else
                {
                    if (WType == 28 || WType == 29)
                    {
                        if (Is_In_HST == false)
                        {
                            Mpa.ReadMatchingTxnsMasterPoolAndFillTableGeneral(WOperator,
                            WSignedId, WSelectionCriteria, WSortCriteria, 2, FirstCycle, WSettlementDate, WType);
                        }
                        if (Is_In_HST == true)
                        {
                            Mpa.ReadMatchingTxnsMasterPoolAndFillTableGeneral_HST(WOperator,
                            WSignedId, WSelectionCriteria, WSortCriteria, 2, FirstCycle, WSettlementDate, WType);
                        }


                    }
                    else
                    {
                        if (CreateCsv == true)
                        {
                            string OutputFileNm = ""; 
                            if (oldType == 25 || oldType == 35)
                            {
                                OutputFileNm = Mpa.ReadFromWorking_4_AndFill_CSV(WOperator, WSignedId, WRMCycleNo);

                                WType = oldType; 
                            }
                            else
                            {
                                OutputFileNm = Mpa.ReadMatchingTxnsMasterPoolAndFillTableGeneral_CSV(WOperator, WSignedId, WSelectionCriteria,
                                                                                                                      WSortCriteria, WCategoryId);
                            }
                            
                            CreateCsv = false; 
                            if (Mpa.TotalSelected > 0 )
                            {
                                MessageBox.Show("Tab delimeter file created as.." + OutputFileNm);
                                return; 
                            }
                            else
                            {
                                MessageBox.Show("Tab delimeter was not created ");
                                return;
                            }

                        }
                        else
                        {
                            if (Is_In_HST == false)
                            {
                                Mpa.ReadMatchingTxnsMasterPoolAndFillTableGeneral(WOperator, WSignedId, WSelectionCriteria,
                                                                        WSortCriteria, 2, FirstCycle, NullPastDate, WType);
                            }
                            if (Is_In_HST == true)
                            {
                                Mpa.ReadMatchingTxnsMasterPoolAndFillTableGeneral_HST(WOperator, WSignedId, WSelectionCriteria,
                                                                        WSortCriteria, 2, FirstCycle, NullPastDate, WType);
                            }


                        }
                    }
                    
                    dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;
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
                    if (WType == 25 || WType == 35 || WType == 96)
                    {
                        ShowGrid_25();
                    }
                    else
                    {
                        ShowGrid_1();
                    }
                        
                }
                else
                {
                    // Coming from reversals
                    ShowGrid_2();
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

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            if (FromMaster == true)
            {
                WUniqueNo = (int)rowSelected.Cells[0].Value;
                WFilterFinal = " Where  UniqueRecordId = " + WUniqueNo;
                if (Is_In_HST == true)
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(WFilterFinal, 2);
                }
                else
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WFilterFinal, 2);
                }

                if (Mpa.Origin == "Our Atms")
                {
                    buttonJournalText.Show();
                    button4.Show();
                    button5.Show();
                    buttonJournal_Near.Show();
                    //   buttonPOS.Hide();

                    IsReversal = false;
                    Message = "";

                    RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();

                    Jt.ReadJournalTextByTrace(WOperator, Mpa.TerminalId, Mpa.MasterTraceNo, Mpa.TransDate.Date);

                    if (Jt.RecordFound == true)
                    {
                        JournalFound = true;

                        Message = "";
                    }
                    else
                    {
                        JournalFound = false;
                    }

                    // If First position is Zero check if TXN has been reversed


                    // Check if reversal at Target
                    string WWMask = Mpa.MatchMask;
                    string TableId = "";
                    if (WWMask == "01"
                        || WWMask == "011"
                        || WWMask == "010"
                        || WWMask == "001"
                        || WWMask == "0AA"
                        )
                    {

                        string SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + Mpa.FileId02;
                        string SourceTable_C = "[RRDM_Reconciliation_ITMX].[dbo]." + Mpa.FileId03;
                        string WSelectionCriteria;

                        if (WWMask == "01")
                        {
                            TableId = SourceTable_B;
                        }
                        if (WWMask == "011")
                        {
                            int Count_11_B = 0;
                            int Count_22_B = 0;

                            int Count_11_C = 0;
                            int Count_22_C = 0;

                            TableId = Mpa.FileId02;

                            WSelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId + "'"
                                                       + " AND  ProcessedAtRMCycle =" + Mpa.MatchingAtRMCycle
                                                       + " AND TraceNo =" + Mpa.TraceNoWithNoEndZero;
                            //RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables(); 
                            Mgt.ReadTransSpecificFromBothTables_By_SelectionCriteria(WSelectionCriteria, TableId);

                            Count_11_B = Mgt.Count_11;
                            Count_22_B = Mgt.Count_23;

                            // Take From Target 
                            TableId = Mpa.FileId03;

                            WSelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId + "'"
                                                    + " AND  ProcessedAtRMCycle =" + Mpa.MatchingAtRMCycle
                                                    + " AND TraceNo =" + Mpa.TraceNoWithNoEndZero;

                            Mgt.ReadTransSpecificFromBothTables_By_SelectionCriteria(WSelectionCriteria, TableId);

                            Count_11_C = Mgt.Count_11;
                            Count_22_C = Mgt.Count_23;

                            // Normal Case
                            if (
                                Count_11_B == 1 & Count_22_B == 0
                                &
                                Count_11_C == 1 & Count_22_C == 0
                                )
                            {
                                // Normal Case .. Look for error in Journal 
                            }

                            // Case With Reversals 
                            if (
                                Count_11_B == 1 & Count_22_B == 1
                                &
                                Count_11_C == 1 & Count_22_C == 1
                                )
                            {
                                // Entries Were Reversed 
                                IsReversal = true;
                            }

                        }

                        string WJournalId;

                        if (WOperator == "CRBAGRAA")
                        {

                            WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
                        }

                        if (WOperator == "ETHNCY2N")
                        {
                            WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
                        }


                        // See Journal To examine Error 
                        if (IsReversal == true & JournalFound == true)
                            Message = "There is in Journal but not valid. " + Environment.NewLine
                                  + "Entry was Reversed. " + Environment.NewLine
                                  + "";

                        if (IsReversal == true & JournalFound == false)
                            Message = "No Journal Record. " + Environment.NewLine
                                  + "Entry was Reversed. " + Environment.NewLine
                                  + "";

                        if (IsReversal == false & JournalFound == false)
                            Message = "No Journal Record. " + Environment.NewLine
                                  + "No Reversal Entry. " + Environment.NewLine
                                  + ""
                                  ;
                        if (IsReversal == false & JournalFound == true)
                            Message = "There is in Journal but not valid. " + Environment.NewLine
                                  + "No Reversal Entry. " + Environment.NewLine
                                  + "Customer was debited." + Environment.NewLine
                                  + "";
                    }
                }
                else
                {
                    // JCC 
                    buttonJournalText.Hide();
                    buttonJournal_Near.Hide();
                    button4.Hide();
                    button5.Show();
                    //  buttonPOS.Show();
                }

                textBoxEncrypted.Text = Mpa.Card_Encrypted; 

                // NOTES START  
                Order = "Descending";
                WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
                WSearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
                if (Cn.RecordFound == true)
                {
                    labelNumberNotes2.Text = Cn.TotalNotes.ToString();
                }
                else labelNumberNotes2.Text = "0";

                // NOTES END

                WWUniqueRecordId = Mpa.UniqueRecordId;


                //if (Mpa.TraceNoWithNoEndZero > 0 & Mpa.Origin == "Our Atms")
                //{
                //    textBoxTraceNo.Text = Mpa.TraceNoWithNoEndZero.ToString();
                //  //  textBoxLineDetails.Text = "DETAILS OF SELECTED";
                //}
                //if (Mpa.RRNumber != "0" & Mpa.Origin != "Our Atms")
                //{
                //    textBoxTraceNo.Text = Mpa.RRNumber;
                //}

                //if (Mpa.IsMatchingDone == false)
                //{
                //    //  label22.Show();
                //    textBoxUnMatchedType.Hide();
                //    //  buttonTransTrail.Hide();
                //}
                //else
                //{
                //    //  label22.Hide();
                //    textBoxUnMatchedType.Show();
                //    // buttonTransTrail.Show();
                //}

                
                if (Mpa.Origin == "Our Atms")
                {

                    buttonJournalText.Show();
                    button4.Show();
                    button5.Show();
                    labelTextFromJournal.Show();
                    buttonJournal_Near.Show();
                    labelNear_Journal.Show();
                    label25.Show();
                    label19.Show();

                    //Tp.ReadInPoolTransSpecific(Mpa.OriginalRecordId); // Read Transactions details 
                    //ATMTrans = true;
                }
                else
                {
                    //buttonReplPlay.Hide();

                    buttonJournalText.Hide();
                    button4.Hide();
                    button5.Hide();
                    buttonJournal_Near.Hide();
                    labelTextFromJournal.Hide();
                    labelNear_Journal.Hide();
                    label25.Hide();
                    label19.Hide();
                    //ATMTrans = false;
                }



                //  textBoxMask.Text = Mpa.MatchMask;
                // Check for exceptions 
                if (Mpa.MetaExceptionNo > 0)
                {
                    label3.Show();
                    panel3.Show();

                    Ec.ReadErrorsTableSpecific(Mpa.MetaExceptionNo);

                    textBoxExceptionNo.Text = Mpa.MetaExceptionNo.ToString();

                    textBoxExceptionDesc.Text = Ec.ErrDesc;
                }
                else
                {
                    label3.Hide();
                    panel3.Hide();
                }

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


                if (Mpa.UnMatchedType == "DUPLICATE")
                {
                    if (Mpa.MatchMask == "") WMask = "000";
                    else WMask = Mpa.MatchMask;
                }
                else
                {
                    WMask = Mpa.MatchMask;
                }


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
            else
            {
                // COMING FROM REVERSAL 

                WRevSeqNo = (int)rowSelected.Cells[0].Value;
                string SelectionCriteria = " Where  SeqNo = " + WRevSeqNo;

                Rev.ReadReversalsBy_Selection_criteria(SelectionCriteria);

                WFileId = Rev.FileId;
                WCategoryId = Rev.MatchingCateg; 
            }
            

            //textBoxInputField.Text = Mpa.CardNumber;
        }


        public void ShowGrid_1()
        {

            //dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            ////if (Mpa.MatchingMasterDataTableATMs.Rows.Count == 0)
            ////{
            ////    MessageBox.Show("No Unmatched To Show");

            ////    this.Dispose();
            ////    return;
            ////}

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 60; // RecordId
            //dataGridView1.Columns[0].Name = "Record Id";
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].HeaderText = "Record Id";

            dataGridView1.Columns[1].Width = 70; // ATMNo      
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 70; // Descr
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 100; // Card
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 100; // Card Encrypted 
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

            dataGridView1.Columns[8].Width = 120; // Date
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[9].Width = 50; // Trace No
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 80; // RRNumber
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[11].Width = 40; // MatchMask
            //dataGridView1.Columns[10].HeaderText = "Match Mask";
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[12].Width = 40; // Settled
            //dataGridView1.Columns[11].HeaderText = "Settled";
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[13].Width = 40; // Action Type
            //dataGridView1.Columns[12].HeaderText = "Action Type";
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[14].Width = 100; // Action Desc
                                                   // dataGridView1.Columns[13].HeaderText = "Action Desc";
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;



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

            dataGridView1.Columns[19].Width = 80; // OriginWorkFlow
            dataGridView1.Columns[19].HeaderText = "OriginWorkFlow";
            dataGridView1.Columns[19].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[20].Width = 80; // RMCateg
            dataGridView1.Columns[20].HeaderText = "RMCateg";
            dataGridView1.Columns[20].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[21].Width = 60; // ReplCycle
            dataGridView1.Columns[21].HeaderText = "ReplCycle";
            dataGridView1.Columns[21].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[22].Width = 40; // TXNSRC
            dataGridView1.Columns[22].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 

            dataGridView1.Columns[23].Width = 40; // TXNDEST
            dataGridView1.Columns[23].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[24].Width = 100; // UserId
            dataGridView1.Columns[24].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


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
            string WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
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
                //
                // Find out if ATM is Recycling 
                //
                RRDMGasParameters Gp = new RRDMGasParameters();
                bool IsRecycle = false;

                string ParId2 = "948";
                string OccurId2 = "1"; // 
                                       //RRDMGasParameters Gp = new RRDMGasParameters(); 
                Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
                if (Gp.RecordFound & Gp.OccuranceNm == "YES")
                {
                    RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();
                    SM.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WReplCycle);
                    if (SM.RecordFound == true)
                    {
                        // Check if Reccyle 
                        if (SM.is_recycle == "Y")
                        {
                            IsRecycle = true;
                        }
                    }
                }

                if (IsRecycle == true)
                {
                    // Recycle Type 
                    Form51_Recycle NForm51_Recycle;
                    NForm51_Recycle = new Form51_Recycle(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                    //NForm51_Recycle.FormClosed += NForm5_FormClosed;
                    NForm51_Recycle.ShowDialog();
                }
                else
                {
                    // Current Bank De Caire Type 
                    NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                    //NForm51.FormClosed += NForm5_FormClosed;
                    NForm51.ShowDialog();
                }
                //NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                //NForm51.ShowDialog();
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
            bool In_HST = false;

            if (Mpa.TransDate.Date <= HST_DATE)
            {
                In_HST = true;
            }
            // Show Lines of journal 
            string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;
            if (In_HST == true)
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            }
            else
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            }
            //
            // Check those where Mask Has 0 as first Character 
            //
            string FirstChar = Mpa.MatchMask.Substring(0, 1);

            if (FirstChar == "0")
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
            //// Show Lines of journal 
            //string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;

            //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            //// Show Lines of journal 
            ////
            //// Check those where Mask Has 0 as first Character 
            ////
            //string FirstChar = Mpa.MatchMask.Substring(0, 1);

            //if (FirstChar == "0")
            //{
            //    MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
            //                     + "Select Journal Lines Near To this"
            //                     );
            //    return;
            //}

            //int WSeqNoA = 0;
            //int WSeqNoB = 0;

            //if (Mpa.TraceNoWithNoEndZero == 0)
            //{
            //    MessageBox.Show("No Available Trace to show the Journal Lines for this Txn/Category ");
            //    return;
            //}
            //else
            //{
            //    // Assign Seq number for Pambos Journal table
            //    WSeqNoA = Mpa.OriginalRecordId;
            //    WSeqNoB = Mpa.OriginalRecordId;
            //}

            ////
            //// Bank De Caire
            ////
            //Form67_BDC NForm67_BDC;

            //int Mode = 5; // Specific
            //string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            //if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
            //NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            //NForm67_BDC.ShowDialog();


        }
        // Full Journal
        private void button4_Click(object sender, EventArgs e)
        {
            bool In_HST = false;

            if (Mpa.TransDate.Date <= HST_DATE)
            {
                In_HST = true;
            }

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
            if (In_HST == true)
            {
                Jt.ReadJournalTxnsByParameters_HST(WOperator, Mpa.TerminalId, Mpa.AtmTraceNo * 10, Mpa.TransAmount, Mpa.CardNumber, Mpa.TransDate.Date);
            }
            else
            {
                Jt.ReadJournalTxnsByParameters(WOperator, Mpa.TerminalId, Mpa.AtmTraceNo * 10, Mpa.TransAmount, Mpa.CardNumber, Mpa.TransDate.Date);
            }


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

                return;
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
            //Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            //Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

          
            //            if (FromMaster == true)
            //            {
            //                NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, Mpa.UniqueRecordId, 1);

            //                NForm78d_AllFiles_BDC_3.ShowDialog();
            //            }
            //            else
            //            {
            //                //break;
            //                Form78d_AllFiles_Reversals NForm78d_AllFiles_Reversals;

            //                NForm78d_AllFiles_Reversals = new Form78d_AllFiles_Reversals(WOperator, WSignedId, WFileId, WRevSeqNo, WCategoryId);

            //                NForm78d_AllFiles_Reversals.ShowDialog();
            //            }

            Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES


            bool In_HST = false;
            int Mode = 1;

            if (Mpa.TransDate.Date <= HST_DATE)
            {
                In_HST = true;
                Mode = 4;
            }

            NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, Mpa.UniqueRecordId, Mode);
            NForm78d_AllFiles_BDC_3.FormClosed += NForm5_FormClosed;
            NForm78d_AllFiles_BDC_3.ShowDialog();

        }
        // Near Journal 
        int IntTrace;

        private void buttonJournal_Near_Click(object sender, EventArgs e)
        {
            // Here we search the journal lines close to the selected transaction
            // If selected already in Journal then we show the one before and the one after
            // If transaction not in journal say 011 then we show 5 minutes before and 5 minutes after. 
            // This is because the time at mainframe maybe different that this of ATM. 

            string WWSubString = WMask.Substring(0, 1);

            DateTime SavedTransDate = Mpa.TransDate;

            if (WWSubString == "1")
            {
                IntTrace = Mpa.TraceNoWithNoEndZero;
            }

            bool In_HST = false;

            if (Mpa.TransDate.Date <= HST_DATE)
            {
                In_HST = true;
            }
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
            int SaveSeqNo = Mpa.SeqNo;

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
            if (WWSubString == "1")
            {
                WTraceRRNumber = IntTrace.ToString();
            }

            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                      SavedTransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();


        }        // Show Not Processed yet
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

            WSelectionCriteria = " WHERE MatchingCateg ='" +WCategoryId + "'" 
                                          + "  AND RMCycleNo=" + WRMCycleNo;
            WSortCriteria = " ORDER BY TransDate_4 , FileId ";


            labelStep1.Text = "View Txns of Category =_" + WCategoryId
                          + "_Reversals ";

            Form80b_Load(this, new EventArgs());
        }
// Export to Excel
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 4000)
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
                    XL.ExportToExcel(Mpa.MatchingMasterDataTableATMs, WorkingDir, ExcelPath);
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
        int oldType = 0; 
        private void buttonExport_CSV_Click(object sender, EventArgs e)
        {
            oldType = WType; 
            WType = 0; 
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}


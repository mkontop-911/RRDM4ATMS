using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class UCForm277b_EGA : UserControl
    {
        //RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
        RRDMMatchingTxns_InGeneralTables_EGATE Mmob = new RRDMMatchingTxns_InGeneralTables_EGATE();

        //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingOfTxnsFindOriginRAW Msf = new RRDMMatchingOfTxnsFindOriginRAW();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDM_BULK_IST_AndOthers_Records_ALL_2 Bist = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMActions_GL Ag = new RRDMActions_GL();
        RRDMActions_Occurances_E_WALLET Aoc_Wallet = new RRDMActions_Occurances_E_WALLET();

        //   string WUserOperator; 
        //public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool ViewWorkFlow;

        bool AudiType;

        bool PresenterError;

        bool IsMobile;

        bool Is_GL_Creation_Auto;
        bool Is_Presenter_InReconciliation;

        decimal CurrentExcess;
        decimal CurrentShortage;

        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;

        //bool ComingFromMeta;

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;

        int CallingMode; // 

        string WSubString;
        string WMask;
        string PRX;

        // NOTES END 
        bool IsAmtDifferent;

        DateTime LeftDt;

        int WSeqNo;

        //int WMaskRecordIdLeft;

        string WCardNumberLeft;
        string WAccNumberLeft;
        string WTransCurrLeft;
        decimal WTransAmountLeft;
        //int WTraceNo; 

        string SelectionCriteria;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WRMCategoryId;
        int WRMCycleNo;
        string WOperator;
        string WMainCateg;
        string W_Application; 

        public void UCForm277b_EGA_Par(string InSignedId, int SignRecordNo, string InOperator, string InRMCategoryId, int InRMCycle, string InW_Application)
        //public void UCForm277b_MOB_Par(string InSignedId, int SignRecordNo, string InOperator, string InRMCategoryId, int InRMCycle, int InActionOrigin
        //                                                        , string InAtmNo, int InSesNo)
        {
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.AllPaintingInWMmobint, true);

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WRMCategoryId = InRMCategoryId;
            WRMCycleNo = InRMCycle;
            WOperator = InOperator;
            W_Application = InW_Application; 

            WMainCateg = WRMCategoryId.Substring(0, 4);

            InitializeComponent();
            // Set it to Mobile
            // Set it to Mobile
            //
            IsMobile = true;

        
            this.DoubleBuffered = true;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            //labelStep1.Text = "Invstigation and Force Matching for RM Category Id : " + WRMCategoryId;

            // Force Matching Reason
            Gp.ParamId = "714";
            comboBoxReasonOfAction.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxReasonOfAction.DisplayMember = "DisplayValue";

            // Hide what is needed 
            //***************************************

            //panel3.Hide();

            textBox3.Hide();
            // textBox14.Hide();

            comboBoxLeft.Hide();

            textBoxHeaderLeft.Text = "TRANSACTIONS THAT NEED ACTION";
            //textBoxHeaderRight.Text = "MATCHING MASK";
            textBoxLineDetails.Text = "DETAILS OF SELECTED";
            textBox5.Text = "ACTIONS ON SELECTED UNMATCHED";


            if (WMainCateg == "RECA" || WMainCateg == "EWB3"
                || WMainCateg == "NBG3" || WMainCateg == PRX + "2"
                )
            {
                comboBoxLeft.Items.Add("UnMatched");
                comboBoxLeft.Items.Add("Matched");
                comboBoxLeft.Text = "UnMatched";
            }

            RRDMReconcCategories Rc = new RRDMReconcCategories();
            RRDMActions_GL Ag = new RRDMActions_GL();

            //************************************************************
            // Check if it should be based on IST
            int ComMode = 0;
            //
            // Auto Creation Of Transactions
            //
            string ParId = "945";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_GL_Creation_Auto = true;
                buttonAllAccounting.Show();
            }
            else
            {
                Is_GL_Creation_Auto = false;
                buttonAllAccounting.Hide();
            }
            // Presenter
            //ParId = "946";
            //OccurId = "1";
            //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            //if (Gp.OccuranceNm == "YES")
            //{
            //    Is_Presenter_InReconciliation = true;
            //}

            Rc.ReadReconcCategorybyCategId(WOperator, WRMCategoryId);

            if (Is_GL_Creation_Auto == true)
            {
                ComMode = 2;
            }
            else
            {
                ComMode = 4;
            }
            comboBoxActionNm.DataSource = Ag.ReadTableToGet_ActionNm_Array_List(WOperator, ComMode);
            comboBoxActionNm.DisplayMember = "DisplayValue";


            // ACTION TYPE
            // 00 ... No Action Taken 
            // 03 ... Move case to cash reconciliation 
            // 04 ... Force Match - Broken Disc Case  

            AudiType = false;
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

            if (AudiType == true)
            {
                labelExcess.Text = "ATM_Excess";
                labelShortage.Text = "ATM_Shortage";
            }

        }


        // SHOW SCREEN 
        string WSortCriteria;

        public void SetScreen()
        {

            try
            {
                //if (WActionOrigin == 1)
                //{
                labelExcess.Hide();
                textBoxExcess.Hide();

                comboBoxLeft.Text = "UnMatched";
                // Reconciliation 
                if (comboBoxLeft.Text == "UnMatched")
                {
                    if (ViewWorkFlow == true) // View Only 
                    {
                        //if (Is_Presenter_InReconciliation == true)
                        //{
                        //    SelectionCriteria = " WHERE RMCateg ='"
                        //                   + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo
                        //                   + " AND IsMatchingDone = 1 AND FastTrack = 0 "
                        //                   + " AND  ("
                        //                   + " (Matched = 0  " + " AND ActionType != '07') "
                        //                   + "  OR (Matched = 1 AND MetaExceptionId = 55 " + " AND ActionType != '07') )"
                        //                   ;
                        //}
                        //else
                        //{
                            //  where MatchingCateg = 'BDC700' and IsMatchingDone = 1 and Matched = 0 and ProcessedAtRMCycle = 200
                            SelectionCriteria = " WHERE MatchingCateg ='"
                                           + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo
                                           + " AND IsMatchingDone = 1 and Matched = 0 "
                                           ;

                        //}

                        CallingMode = 1; // View

                      
                       
                    }
                    else
                    {
                        //if (Is_Presenter_InReconciliation == true)
                        //{
                        //    // INCLUDE PRESENTER  
                        //    SelectionCriteria = " WHERE RMCateg ='" + WRMCategoryId + "'"
                        //              + "  AND MatchingAtRMCycle = " + WRMCycleNo
                        //              + "  AND IsMatchingDone = 1 AND FastTrack = 0 "
                        //              + "  AND ( "
                        //              + " (Matched = 0 AND SettledRecord = 0 " + " AND ActionType != '07') "
                        //              + "  OR (Matched = 1 AND SettledRecord = 0 AND MetaExceptionId = 55 " + " AND ActionType != '07') "
                        //              + ") "
                        //              ;
                        //}
                        //else
                        //{
                            // Matching is done but not Settled 
                            SelectionCriteria = " WHERE MatchingCateg ='"
                                           + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo
                                           + " AND IsMatchingDone = 1 and Matched = 0 "
                                           ;
                        //}

                        CallingMode = 2; // Updating 
                    }

                    textBoxHeaderLeft.Text = "EXCEPTIONS (UNMATCHED TRANSACTIONS)";
                    //buttonMovedToUnMatched.Hide();
                }

                if (comboBoxLeft.Text == "Matched")
                {
                    if (ViewWorkFlow == true) // View Only 
                    {
                        SelectionCriteria = " WHERE RMCateg ='"
                                          + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo + " Matched = 1 ";

                        CallingMode = 1; // View
                    }
                    else
                    {
                        SelectionCriteria = " WHERE RMCateg ='" + WRMCategoryId + "'"
                                          + " AND MatchingAtRMCycle =" + WRMCycleNo + " AND Matched = 1";

                        CallingMode = 1; // View 

                    }

                    textBoxHeaderLeft.Text = "MATCHED ATM TRANSACTIONS";
                    //buttonMoveToMatched.Hide();
                }



                // }
                // From Replenishment


                //No Dates Are selected

                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

                WSortCriteria = " ORDER BY TransDate ";

                //
                // Read from Master Mobile 
                //
                Mmob.ReadMatchingTxnsMaster_MOBILE_ByRangeAndFillTable(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria
                                                                         , FromDt, ToDt, 2, 0 , W_Application);

                ShowGrid();

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

        }
        bool IsReversal;
        bool JournalFound;
        string DiscrepancyMessage;
        string WMasterTableName; 
        // ON Left Grid Row Enter 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            SelectionCriteria = " WHERE  SeqNo=" + WSeqNo;

            //string MasterTableName = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatching_Master_MOBILE]";
            WMasterTableName = W_Application + ".[dbo]." + "TXNS_MASTER";
            Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, WMasterTableName, 1, W_Application);

            if (Mmob.RecordFound == true)
            {
                if (WRMCategoryId == "EGA375" )
                {
                    linkLabelToAnalysis.Show();
                }
                else
                {
                    linkLabelToAnalysis.Hide();
                }
            }
            // Check mask 
            if (Mmob.MatchMask == "" & Mmob.IsMatchingDone == false)
            {
                MessageBox.Show("Matching Not done for this transaction");
                Mmob.MatchMask = "FFF";
            }

            // SET AS EQUIVALENT 
            WActionId = Mmob.ActionType;

            DiscrepancyMessage = "";



            IsReversal = false;
            IsAmtDifferent = false;
          //  buttonAssignPartner.Hide();

            DiscrepancyMessage = "";
            // 
            if (Mmob.MatchMask.Contains("R") == true)
            {
                DiscrepancyMessage = "There is Reversal in the matching records. " + Environment.NewLine
                             + "If in position there is letter R then this was Reversed. " + Environment.NewLine
                             + "See them by pressing Source Records Button ";

                IsReversal = true;
            }

            // NOTES 2 START  
            Order = "Descending";
            WParameter4 = "SeqNo:" + Mmob.SeqNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
            // NOTES 2 END

            // NOTES 3 START  
            Order = "Descending";
            WParameter4 = "Force Matching for" + " Category: " + WRMCategoryId + " Matching SesNo: " + WSeqNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            // NOTES 3 END
            //SenderTelephone = (string)rdr["SenderTelephone"];
            //ReceivedTelephone = (string)rdr["ReceivedTelephone"];
            WTransCurrLeft = Mmob.TransCurr;
            WTransAmountLeft = Mmob.TransAmount;

            textBoxType.Text = Mmob.TransType;
            textBoxRRNumber.Text = Mmob.RRNumber;
            textBoxReference_TPF.Text = Mmob.Reference_TPF;
            textBoxAUTH.Text = Mmob.AUTHNUM;
            textBoxSenderNumber.Text = Mmob.SenderTelephone;
            textBoxReceiverNumber.Text = Mmob.ReceivedTelephone;
            textBoxTransDate.Text = Mmob.TransDate.ToString();

            textBoxCcy.Text = Mmob.TransCurr;
            if (WRMCategoryId == "ETI375")
            {
                textBoxTransAmt.Text = Mmob.TransAmount.ToString("#,##0.000");
            }
            else
            {
                textBoxTransAmt.Text = Mmob.TransAmount.ToString("#,##0.00");
            }
             

            //textBoxTXNSRC.Text = Mmob.TXNSRC;
            //textBoxTXNDEST.Text = Mmob.TXNDEST;

            textBoxCAP_DATE.Text = Mmob.CAP_DATE.ToShortDateString();
            textBoxSET_DATE.Text = Mmob.SET_DATE.ToShortDateString();

            //textBoxAuthNo.Text = Mmob.AUTHNUM; 

            Mc.ReadMatchingCategoryBySelectionCriteria(Mmob.MatchingCateg, 0, 12); 

            textBoxCategoryNm.Text = Mmob.MatchingCateg + Environment.NewLine
                               + Mc.CategoryName;

            textBoxUnMatchedType.Text = Mmob.Comments; 

            ShowMask(Mmob.MatchMask);

            if (Mmob.ActionType != "00" & Mmob.ActionType != "")
            // This the meta exception no created OR Moved from reconciliation
            {
                Aoc_Wallet.ReadActionsOccurancesByUniqueKey("Master_Pool_MOBILE", WSeqNo, Mmob.ActionType);
                comboBoxReasonOfAction.Text = Aoc_Wallet.Maker_ReasonOfAction;


                labelActionType.Text = "Action Taken";
                labelActionType.ForeColor = Color.Black;

                //buttonAction.Hide();
                buttonUndoAction.Show();

                comboBoxActionNm.Enabled = false;

                //string ActionNm = 
                Ag.ReadActionByActionId(WOperator, Mmob.ActionType, 1);

                comboBoxActionNm.Text = Ag.ActionNm;

                pictureBox2.Show();

                // Dissable selection and action
                // Enable Undo Action
                buttonAction.Enabled = false;
                buttonUndoAction.Enabled = true;

            }
            else
            {

                labelActionType.Text = "Action NOT Taken Yet!";
                labelActionType.ForeColor = Color.Red;
                pictureBox2.Hide();

                comboBoxActionNm.Text = "00_No Action Taken";
                comboBoxActionNm.Enabled = true;

                buttonAction.Enabled = true;
                buttonUndoAction.Enabled = false;

                labelForceMatching.Hide();
                comboBoxReasonOfAction.Hide();

            }
            //ComingFromMeta = false;
            LeftDt = Mmob.TransDate.Date;

            if (ViewWorkFlow == true)
            {
                buttonAction.Hide();
                buttonUndoAction.Hide();
            }

            // Show Dispute 

            Dt.ReadDisputeTranByUniqueRecordId(WSeqNo);
            if (Dt.RecordFound == true)
            {
                labelDisputeId.Show();
                textBoxDisputeId.Show();
                linkLabelDispute.Show();

                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
            }
            else
            {
                labelDisputeId.Hide();
                textBoxDisputeId.Hide();
                linkLabelDispute.Hide();
            }
        }

        // Row enter for MetaExceptions created 
        int WExcNo;
        
        //
        // Show info for Mask
        //
        public void ShowMask(string InMask)
        {

            //****************************************************************************
            //Translate MASK 
            //****************************************************************************

            RRDMMatchingCategoriesVsSourcesFiles Rcs = new RRDMMatchingCategoriesVsSourcesFiles();
            Rcs.ReadReconcCategoriesVsSourcesAll(WRMCategoryId);

            WMask = InMask;

            if (WMask == "")
            {
                WMask = "EEE";
            }
            // First Line
            if (Rcs.SourceFileNameA != "")
            {
                labelFileA.Show();
                textBox31.Show();

                labelFileA.Text = "File A : " + Rcs.SourceFileNameA;
                labelFileA.Show();
                WSubString = WMask.Substring(0, 1);

                if (WSubString == "1")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox31.BackColor = Color.Red;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = WSubString;
                }
            }
            else
            {
                labelFileA.Hide();
                textBox31.Hide();
            }

            // Second Line 
            if (Rcs.SourceFileNameB != "")
            {
                labelFileB.Show();
                textBox32.Show();

                labelFileB.Text = "File B : " + Rcs.SourceFileNameB;
                labelFileB.Show();
                WSubString = WMask.Substring(1, 1);

                if (WSubString == "1")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox32.BackColor = Color.Red;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = WSubString;
                }
            }
            else
            {
                labelFileB.Hide();
                textBox32.Hide();
            }

            // Third Line 
            //
            if (Rcs.SourceFileNameC != "" & WMask.Length > 2)
            {
                labelFileC.Show();
                textBox33.Show();

                labelFileC.Text = "File C : " + Rcs.SourceFileNameC;
                labelFileC.Show();

                WSubString = WMask.Substring(2, 1);

                if (WSubString == "1")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox33.BackColor = Color.Red;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = WSubString;
                }
            }
            else
            {
                labelFileC.Hide();
                textBox33.Hide();
            }
        }

        // Show Grid Left 
        public void ShowGrid()
        {
            //WTotalLeft = 0 ;

            //Keyboard.Focus(dataGridView1);

            dataGridView1.DataSource = Mmob.MatchingMasterDataTable_MOBILE.DefaultView;

            textBoxTotalSelected.Text = Mmob.MatchingMasterDataTable_MOBILE.Rows.Count.ToString();

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            //if (InMode == 1 || InMode == 2 || InMode == 5)
            //{
            //    MatchingMasterDataTable_MOBILE.Columns.Add("SeqNo", typeof(int));
            //    MatchingMasterDataTable_MOBILE.Columns.Add("Status", typeof(string));
            //    MatchingMasterDataTable_MOBILE.Columns.Add("Done", typeof(string));
            //}

            //MatchingMasterDataTable_MOBILE.Columns.Add("Action", typeof(string));

            //MatchingMasterDataTable_MOBILE.Columns.Add("Type", typeof(string));

            //MatchingMasterDataTable_MOBILE.Columns.Add("Mask", typeof(string));

            //MatchingMasterDataTable_MOBILE.Columns.Add("CR_DB", typeof(string));

            //MatchingMasterDataTable_MOBILE.Columns.Add("Ccy", typeof(string));
            //MatchingMasterDataTable_MOBILE.Columns.Add("Amount", typeof(decimal));
            //MatchingMasterDataTable_MOBILE.Columns.Add("BillingAmt", typeof(decimal));
            //MatchingMasterDataTable_MOBILE.Columns.Add("Date", typeof(string));

            //MatchingMasterDataTable_MOBILE.Columns.Add("MatchingRRN", typeof(string));

            //MatchingMasterDataTable_MOBILE.Columns.Add("MatchingCateg", typeof(string));

            //MatchingMasterDataTable_MOBILE.Columns.Add("Comments", typeof(string));

            dataGridView1.Columns[0].Width = 60; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].HeaderText = "Record Id";

            dataGridView1.Columns[1].Width = 50; //   Status
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 50; // DONE
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 50; // Action
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 50; // Type
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 50; // Mask
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 50; // CR_DB
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 50; // Ccy
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 80; // Amount
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[9].Width = 80; //  BillingAmt
            dataGridView1.Columns[9].DefaultCellStyle = style;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[9].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[10].Width = 100; // Date
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[11].Width = 50; // MatchingRRN
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            

        }

        // Force Matching 

        // NOTES FOR RM CYCLE 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Force Matching for" + " Category: " + WRMCategoryId + " Matching SesNo: " + WRMCycleNo;
            string SearchP4 = "";

            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "Force Matching for" + " Category: " + WRMCategoryId + " Matching SesNo: " + WRMCycleNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }
        // NOTES FOR ITEM 
        private void buttonNotes3_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "SeqNo:" + Mmob.SeqNo;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "SeqNo:" + Mmob.SeqNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
        }
        //EXPAND GRID 
        Form78b NForm78b;

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            string WHeader = "UNMATCHED TRANSACTIONS FOR RM CATEGORY : " + WRMCategoryId;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mmob.MatchingMasterDataTable_MOBILE, WHeader, "Form271b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }
        
        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //dataGridView1.Rows[NForm78b.WSelectedRow].Selected = true;
            if (NForm78b.UniqueIsChosen == 1)
            {
                SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                               + "  AND MatchingAtRMCycle = " + WRMCycleNo
                              + " AND SettledRecord = 0 AND  UniqueRecordId = " + NForm78b.WPostedNo.ToString() + " ";

                CallingMode = 2;
                string WSortCriteria = " ORDER BY TerminalId ";

                //No Dates Are selected

                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

               // Mmob.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt, 2, W_Application);
                //Mmob.ReadMatchingTxnsMasterPoolAndFillTable(WOperator, CallingMode, SelectionCriteria, "");

                ShowGrid();

            }
        }

        // Proceed to action 
        Form14b NForm14b;
        Form14b_POS NForm14b_POS;
        int WRowIndex;

        string WActionId;
        // string WAccount;
        bool DisputeOpened;
        // int WMetaExceptionId;
        string UniqueRecordIdOrigin;
        DataTable TEMPTableFromAction;

        //string WActionId; 

        private void buttonAction_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            bool ValidAction = false;

            RRDMActions_GL Ag = new RRDMActions_GL();
            Ag.ReadActionByActionNm(WOperator, comboBoxActionNm.Text);

            WActionId = Ag.ActionId;

            //MessageBox.Show(comboBoxActionNm.Text + Environment.NewLine
            //          + WActionId + Environment.NewLine
            //          );

            //WActionId = comboBoxActionNm.Text.Substring(0, 2);

            if (comboBoxActionNm.Text == "00_No Action Taken")
            {
                MessageBox.Show("Select Action Please!");
                return;
            }

            //MessageBox.Show(comboBoxActionNm.Text + Environment.NewLine
            //           + WActionId + Environment.NewLine
            //           );

            if (comboBoxReasonOfAction.Text == "Select Reason")
            {
                MessageBox.Show("Please Select Reason For taking this Action");
                return;
            }
            if (Mmob.TXNDEST == "DEBIT")
            {
                Mmob.TXNDEST = "1";
            }

           

            // 
            bool Is_POS = false;
            //if (Mmob.MatchedType == PRX + "231" || Mmob.MatchedType == PRX + "233")
            //{
            //    Is_POS = true;
            //}

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParId = "851"; // Under Development
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string UnderDevelopment = Gp.OccuranceNm;

            ParId = "852"; // IN PRODUCTION ENVIRONEMENT 
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string IsProductionEnv = Gp.OccuranceNm;



            if (IsProductionEnv == "YES" & UnderDevelopment == "YES")
            {
                // It Is YES for BDC
                if (WActionId == "04" || WActionId == "06")
                {
                    ValidAction = true; // For BDC too 
                }
                else
                {
                    return;
                }
            }
            else
            {
                // Testing Environment
                ValidAction = true;
            }

            // Read Action Details
            Ag.ReadActionByActionId_And_Occ(WOperator, WActionId, 1);

            

            // Dispute 
            if (WActionId == "05" & ValidAction == true //05_Move to dispute"
                                || WActionId == "12" // Whenever this is selected a dispute is oppened too
                                                     // Already a transaction was created through the previous process
                                || WActionId == "22" // Whenever this is selected a dispute is oppened too
                                )
            {
                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WUniqueRecordIdOrigin = "Master_Pool_MOBILE";
                string WMaker_ReasonOfAction = comboBoxReasonOfAction.Text;

                string WOriginWorkFlow = "";


                WOriginWorkFlow = "Reconciliation";



                Aoc_Wallet.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                    WActionId, WUniqueRecordIdOrigin,
                                                    WSeqNo, Mmob.TransCurr, Mmob.TransAmount,
                                                    Mmob.TerminalId, 0, WMaker_ReasonOfAction, WOriginWorkFlow, W_Application);



                if (WActionId == "12" || WActionId == "22")
                {
                    MessageBox.Show("The funds will be moved to Intermediate account" + Environment.NewLine
                                + "Also an internal dispute connected to the maker name will be oppened" + Environment.NewLine
                                );
                }

                SelectionCriteria = " WHERE  SeqNo =" + WSeqNo;

               // string MasterTableName = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatching_Master_MOBILE]";
                string MasterTableName = W_Application + ".[dbo]." + "TXNS_MASTER";
                Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, MasterTableName, 1, W_Application);

                Dt.ReadDisputeTranByUniqueRecordId(WSeqNo);
                if (Dt.RecordFound == true)
                {
                    MessageBox.Show(" Dispute already open for this Error");
                    return;
                }
                else
                {
                    MessageBox.Show(" Dispute not in functionality");
                    return;
                }
                Form5 NForm5;
                int From = 7; // From pre - dispute investigation 
                NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Mmob.CardNumber, WSeqNo, Mmob.TransAmount, 0, "From Reconciliation", From, "ATM");
                NForm5.FormClosed += NForm5_FormClosed;
                NForm5.ShowDialog();

                if (DisputeOpened == false)
                {
                    MessageBox.Show("Dispute was not opened! ");
                    return;
                }

                //Mmob.ActionByUser = true;
                //Mmob.UserId = WSignedId;

                //Mmob.MatchedType = "Move To Dispute";

                //Mmob.ActionType = WActionId;

                ////Mmob.Comments = Mmob.Comments + " ," + comboBoxReasonOfAction.Text;

               // Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WSeqNo, 1);

                // Update RM Cycle
                // For having these for Form271a
                //Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

                //Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow + 1;

                //Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);

                MessageBox.Show("UnMatched Record with UniqueRecordId : " + WSeqNo.ToString() + "...Has been moved to dispute");

                WRowIndex = dataGridView1.SelectedRows[0].Index;
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

                return; // leave it here 

            }


            // Force matched and other Manual 

            if (Ag.Is_GL_Action == false & (WActionId == "04"
               || WActionId == "06"
               || WActionId == "09"
               //|| WActionId == "23"
               //|| WActionId == "13"
               //|| WActionId == "73"
               //|| WActionId == "83"
               )
               )
            {
                // This is force Matching and other manual 
                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WUniqueRecordIdOrigin = "Master_Pool_MOBILE";
                string WMaker_ReasonOfAction = comboBoxReasonOfAction.Text;


                string WOriginWorkFlow = "";


                WOriginWorkFlow = "Reconciliation";

                Mmob.TerminalId = ""; 

                Aoc_Wallet.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                    WActionId, WUniqueRecordIdOrigin,
                                                    WSeqNo, Mmob.TransCurr, Mmob.TransAmount,
                                                    Mmob.TerminalId, 0, WMaker_ReasonOfAction, WOriginWorkFlow,W_Application);

                SelectionCriteria = " WHERE  SeqNo =" + WSeqNo;

               //string MasterTableName = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatching_Master_MOBILE]";
                string MasterTableName = W_Application + ".[dbo]." + "TXNS_MASTER";
                Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, MasterTableName, 1, W_Application);

                Mmob.ActionByUser = true;
                Mmob.UserId = WSignedId;

                Mmob.MatchedType = comboBoxReasonOfAction.Text;

                Mmob.ActionType = WActionId;

                Mmob.UpdateRecordAsUnmatchedBySeqNumber_ACTION_MOBILE(MasterTableName, WSeqNo);

                //Mmob.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WSeqNo, 2);

                MessageBox.Show("Record with UniqueRecordId : " + WSeqNo.ToString() + Environment.NewLine
                    + "Has been dealt by Maker");

                WRowIndex = dataGridView1.SelectedRows[0].Index;
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            }

           

        }

        // Form5 Close
        void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {

            Dt.ReadDisputeTranByUniqueRecordId(WSeqNo);
            if (Dt.RecordFound == true)
            {
                DisputeOpened = true;
                labelDisputeId.Show();
                textBoxDisputeId.Show();

                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
            }
            else
            {

                DisputeOpened = false;

            }
        }

        void NForm14b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //ComingFromMeta = true;
            // UPDATE TRANSACTION 
            if (NForm14b.Confirmed == true)
            {
               
                SelectionCriteria = " WHERE  SeqNo=" + WSeqNo;

                //string MasterTableName = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatching_Master_MOBILE]";
                string MasterTableName = W_Application + ".[dbo]." + "TXNS_MASTER";
                Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, MasterTableName, 1, W_Application);

                //Mmob.MetaExceptionNo = 0;
                Mmob.ActionByUser = true;
                Mmob.UserId = WSignedId;

                Mmob.ActionType = WActionId; // 

                Mmob.UpdateMatchingTxnsMasterFooter(WOperator, WSeqNo, 2, W_Application);


            }
            else
            {
                Aoc_Wallet.DeleteActionsOccurancesUniqueKeyAndActionID(UniqueRecordIdOrigin, Mmob.UniqueRecordId,
                                                                                                WActionId);
            }


        }

        void NForm14b_POS_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (NForm14b_POS.Confirmed == true)
            {
                SelectionCriteria = " WHERE  SeqNo=" + WSeqNo;

                string MasterTableName = W_Application + ".[dbo]."  + "TXNS_MASTER";
                Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, MasterTableName, 1, W_Application);

                //Mmob.MetaExceptionNo = 0;
                Mmob.ActionByUser = true;
                Mmob.UserId = WSignedId;

                Mmob.ActionType = WActionId; // 

                Mmob.UpdateMatchingTxnsMasterFooter(WOperator, WSeqNo, 2, W_Application);

                // Update RM Cycle
                // For having these for Form271a
                //Rcs.ReadReconcCategorySessionByRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

                //Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow + 1;

                //Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);

            }
            else
            {
                Aoc_Wallet.DeleteActionsOccurancesUniqueKeyAndActionID(UniqueRecordIdOrigin, Mmob.UniqueRecordId,
                                                                                                WActionId);
            }

        }


        // UNDO ACTION 
        private void buttonUndoAction_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;


            //SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;
            //Mmob.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //if (Mmob.SettledRecord ==true)
            //{
            //    MessageBox.Show("You can not Undo this Transaction"+Environment.NewLine
            //        + "This was settled with Action :.." + WActionId
            //        );

            //    return; 
            //}

            Aoc_Wallet.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool_MOBILE", WSeqNo, "90");

            // ReadActionsOccurancesByUniqueKey(InUniqueKeyOrigin, InUniqueKey, InActionId);
            Aoc_Wallet.ReadActionsOccurancesByUniqueKey("Master_Pool_MOBILE", WSeqNo, WActionId);
            if (Aoc_Wallet.RecordFound == true)
            {
                
                Aoc_Wallet.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool_MOBILE", WSeqNo, WActionId);
                // Leave it here
                if (Mmob.ActionType == "08")
                {
                    Mmob.SettledRecord = false; // In the case that was 08 which was moved from Reconciliation to Replenishment
                }
               // Mmob.MetaExceptionNo = 0;
                Mmob.ActionByUser = false;
                Mmob.UserId = "";
                Mmob.ActionType = "00";
               // Mmob.MatchedType = "";
                //Mmob.Comments = "";

                Mmob.UpdateMatchingTxnsMasterFooter(WOperator, WSeqNo, 2, W_Application);

                // Update RM Cycle
                // For having these for Form271a
                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

                Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow - 1;

                Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);
            }

            // Dispute 

            if (WActionId == "05" || WActionId == "12" || WActionId == "22") // This for Dispute 
            {
                //comboBoxForceMatching.Text = "Select Reason";
                Dt.ReadDisputeTranByUniqueRecordId(WSeqNo);
                if (Dt.RecordFound == true & Dt.ClosedDispute == false)
                {
                    Di.DeleteDisputeRecord(Dt.DisputeNumber);

                    textBoxDisputeId.Hide();
                    labelDisputeId.Hide();

                }
                if (Dt.RecordFound == true & Dt.ClosedDispute == true)
                {

                    MessageBox.Show("You cannot undo dispute" + Environment.NewLine
                              + "Dispute is already Closed/Settled"
                               );

                    return;
                }

            }

            // BY FORCE 

            //if (Mmob.ActionType == "04") // This is the Force Matching case 
            //{
            //    comboBoxReasonOfAction.Text = "Select Reason";

            //    Mmob.MetaExceptionNo = 0;
            //    Mmob.ActionByUser = false;
            //    Mmob.UserId = WSignedId;
            //    Mmob.ActionType = "00";

            //    Mmob.MatchedType = "";
            //    Mmob.Comments = ""; 
            //    Mmob.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);

            //    // Update RM Cycle
            //    // For having these for Form271a
            //    Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

            //    Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow - 1;

            //    Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);
            //}

            //// For moving them to replenishment 

            //if (Mmob.ActionType == "08") // 
            //{

            //    Mmob.MetaExceptionNo = 0;
            //    Mmob.ActionByUser = false;
            //    Mmob.UserId = WSignedId;
            //    Mmob.ActionType = "00";

            //    Mmob.MatchedType = "";
            //    Mmob.Comments = "";
            //    Mmob.SettledRecord = false; // In the case that was 08 which was moved from Reconciliation to Replenishment
            //    Mmob.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);

            //    // Update RM Cycle
            //    // For having these for Form271a
            //    Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

            //    Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow - 1;

            //    Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);
            //}




            //    //Mmob.MetaExceptionNo = 0;
            //    Mmob.ActionByUser = false;
            //    Mmob.UserId = WSignedId;
            //    Mmob.ActionType = "00";

            //    Mmob.MatchedType = "";
            //    Mmob.Comments = ""; 
            //    Mmob.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);

            //    // Update RM Cycle
            //    // For having these for Form271a
            //    Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

            //    Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow - 1;

            //    Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);
            //}

            // UNDO Move txns to pool to re-matched

            //if (Mmob.ActionType == "06") // This Move to Pool record
            //{
            //    comboBoxReasonOfAction.Text = "Select Reason";

            //    Mmob.MetaExceptionNo = 0;
            //    Mmob.ActionByUser = false;
            //    Mmob.UserId = WSignedId;
            //    Mmob.ActionType = "00";

            //    Mmob.MatchedType = "";
            //    Mmob.Comments = ""; 
            //    Mmob.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);

            //    // UPDATE RECORDS IN TABLES AS PROCESS
            //    //
            //    //RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            //    //int WMode = 2; // UNDO
            //    //Mgt.UpdateTablesRecordsAsNotProcessed(WUniqueRecordId, WMode);

            //    // Update RM Cycle
            //    // For having these for Form271a
            //    Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

            //    Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow - 1;

            //    Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);

            //}

            comboBoxActionNm.Enabled = true;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            SetScreen();

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }


        // Show Part of Journal 
       
        // TRANSACTION TRAIL FOR THE LEFT
        private void button1_Click(object sender, EventArgs e)
        {


            Form78d_AllFiles_BDC_3_MOBILE NForm78d_AllFiles_BDC_3_MOBILE; // BASED ON DIFFERENT VARIABLES

            NForm78d_AllFiles_BDC_3_MOBILE = new Form78d_AllFiles_BDC_3_MOBILE(WOperator, WSignedId, Mmob.SeqNo, WRMCategoryId, 1, "table", "W_Application");

            NForm78d_AllFiles_BDC_3_MOBILE.ShowDialog();

        }
        // If FORCE MATCHING
        //private void radioButtonForceMatching_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (radioButtonForceMatching.Checked == true)
        //    {
        //        labelForceMatching.Show();
        //        comboBoxForceMatching.Show();
        //    }
        //    else
        //    {
        //        labelForceMatching.Hide();
        //        comboBoxForceMatching.Hide();
        //    }
        //}



      
        // Video clip
        private void buttonVideoClip_Click(object sender, EventArgs e)
        {
            // Based on Transaction No show video clip 
            //TEST
            VideoWindow NvideoForm = new VideoWindow();
            MessageBox.Show("Video will be shown _1");
            NvideoForm.ShowDialog();
        }
        // POS INFORMATION 
        bool WRecordFound;
        private void buttonPOS_Click(object sender, EventArgs e)
        {
            //if (Mmob.Origin == "Our Atms")
            //{
            //    MessageBox.Show("Not Allowed Operation");
            //    return;
            //}
            //RRDMMatchingTxns_InGeneralTables_BDC Mg = new RRDMMatchingTxns_InGeneralTables_BDC();

            //string WSelectionCriteria = " WHERE SeqNo=" + Mmob.OriginalRecordId;
            //string FileId = "[RRDM_Reconciliation_ITMX].[dbo]." + Mmob.FileId01;
            //Mg.ReadTransSpecificFromSpecificTable_Primary(WSelectionCriteria, FileId, 1);
            //if (Mg.RecordFound == true)
            //{
            //    string Message = Mg.FullTraceNo;
            //    MessageBox.Show(Message);
            //}
            //else
            //{
            //    string Message = "Nothing Available to show!";
            //    MessageBox.Show(Message);
            //}

            //return;

            ////
            ////
            //// 
            //RRDMMatchingOfTxnsFindOriginRAW Msr = new RRDMMatchingOfTxnsFindOriginRAW();

            //WRecordFound =
            //    Msr.FindRawRecordFromMasterRecord
            //      (WOperator, Mmob.FileId01, Mmob.MatchingAtRMCycle,
            //      Mmob.TransDate.Date,
            //       Mmob.TerminalId, Mmob.TraceNoWithNoEndZero, Mmob.RRNumber, 2);
            ////(WOperator, Mmob.FileId01, Mmob.MatchingAtRMCycle, Mmob.TerminalId, Mmob.RRNumber, 2);

            //string WHeader = "MERCHANT DETAILS FOR Transaction with RRN : " + Mmob.RRNumber.ToString();
            //string WCardNumber = Mmob.CardNumber;
            //string WAccNo = Mmob.AccNumber;
            //string WDateTime = Mmob.TransDate.ToString();
            //string WAmount = Mmob.TransAmount.ToString("#,##0.00");
            //string WCcy = Mmob.TransCurr;
            //string WAutorisationCd = Msr.AuthorisationId;
            //string WMerchantId = Msr.MerchantId;
            //string WMerchantName = Msr.MerchantNm;
            //string WTranDescription = Msr.TranDescription;

            //Form2Merchant NForm2Merchant;

            //NForm2Merchant = new Form2Merchant(WOperator, WHeader, WCardNumber, WAccNo,
            //                                 WDateTime, WAmount, WCcy, WAutorisationCd,
            //                                        WMerchantId, WMerchantName, WTranDescription);
            //NForm2Merchant.Show();
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
        // Reversals Status
        private void linkLabelStatusOfReversals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMMask_Reversals Mr = new RRDMMask_Reversals();

            Mr.ReadAndFind_Reversals(WOperator, WSignedId, WSeqNo);

            if (Mr.Reversals_In_One || Mr.Reversals_In_Two || Mr.Reversals_In_Three)
            {

                string Message = " There are reversals ";
                if (Mr.Reversals_In_One == true)
                {
                    Message = Message + "..IN pos 1.. ";
                }
                if (Mr.Reversals_In_Two == true)
                {
                    Message = Message + "..IN pos 2..";
                }
                if (Mr.Reversals_In_Three == true)
                {
                    Message = Message + "..IN pos 3..";
                }

                MessageBox.Show(Message);

            }
            else
            {
                MessageBox.Show("No reversals found.");
            }
        }
        // Relations - show transaction taking part in other category
        private void linkLabelRelations_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // UniqueIdType = 13
            // WFunction = "Investigation"
            // WIncludeNotMatchedYet = true
            int UniqueIdType = 13;
            string WFunction = "Investigation";
            string WIncludeNotMatchedYet = "1";
            Form80b NForm80b;
            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator,
             Mmob.TransDate.AddDays(-1), Mmob.TransDate.AddDays(1),
             textBoxType.Text, "", 0, Mmob.RRNumber, 0, UniqueIdType, WFunction, WIncludeNotMatchedYet, 0);
            NForm80b.ShowDialog();
        }
        // Assign Partner  
        Form78d_POS_Reconc NForm78d_POS_Reconc;

        private void comboBoxActionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Leave it as it is 


            if (comboBoxActionNm.Text != "00_No Action Taken")
            {
                if ((Mmob.SettledRecord == true & Mmob.ActionType == "05") & ViewWorkFlow == false)
                {

                    labelForceMatching.Hide(); // Settled during Dispute Process no reason to show 
                    comboBoxReasonOfAction.Hide();
                }
                else
                {
                    labelForceMatching.Show();
                    comboBoxReasonOfAction.Show();
                }

            }
            else
            {
                // Leave it as it is
                labelForceMatching.Hide();
                comboBoxReasonOfAction.Hide();
            }

        }
        // All Actions 
        //
        private void buttonAllActions_Click(object sender, EventArgs e)
        {

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WSelectionCriteria = "";
            int WMode;


            WSelectionCriteria = "WHERE RMCateg='" + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo + " AND OriginWorkFlow ='Reconciliation'"; ;



            if (Is_GL_Creation_Auto == true)
            {
                WMode = 3;
                Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);
            }
            else
            {
                // Manual operation 
                WMode = 4;
                Aoc.ReadActionsOccurancesAndFillTable_Small_Manual(WSelectionCriteria);
            }

            //string WUniqueRecordIdOrigin = "Master_Pool_MOBILE";
            // PROVIDE TABLE to FORM
            Form14b_All_Actions NForm14b_All_Actions;

            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();

            //ReadActionsOccurancesAndFillTable_Small_Manual(string InSelectionCriteria)

        }
        //
        // All Accounting 
        //
        private void buttonAllAccounting_Click(object sender, EventArgs e)
        {

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "";



            WSelectionCriteria = "WHERE RMCateg='" + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo
                + " AND OriginWorkFlow ='Reconciliation'";
            // SelectionCriteria = "WHERE RMCateg='" + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo;
            Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

            Aoc.ClearTableTxnsTableFromAction();

            int I = 0;

            while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
            {
                //    RecordFound = true;
                int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                if (Aoc.Is_GL_Action == true)
                {

                    int WMode2 = 1; // DO NOT Create transaction in pool 
                    string WCallerProcess = Aoc.OriginWorkFlow;
                    Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey,
                                                                 Aoc.ActionId, Aoc.Occurance, WCallerProcess, WMode2);
                }

                I = I + 1;
            }

            DataTable TempTxnsTableFromAction;


            string WUniqueRecordIdOrigin = "Master_Pool_MOBILE";

            Form14b_All_Actions NForm14b_All_Actions;

            // Aoc.ReadActionsTxnsCreateTableByUniqueKey(WUniqueRecordIdOrigin, Dt.UniqueRecordId, "All");

            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;
            //Form14b_All_Actions NForm14b_All_Actions;
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();



        }
        // Link to Dispute
        private void linkLabelDispute_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form3 NForm3;
            //
            NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator, textBoxDisputeId.Text, NullPastDate, NullPastDate, 13);
            // NForm3.FormClosed += NForm3_FormClosed;
            NForm3.ShowDialog();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonSourceRecords_M_Click(object sender, EventArgs e)
        {
            int Mode = 1;

            Form78d_AllFiles_EGATE NForm78d_AllFiles_EGATE; // 

            NForm78d_AllFiles_EGATE = new Form78d_AllFiles_EGATE(WOperator, WSignedId,W_Application ,WSeqNo, Mode);

            NForm78d_AllFiles_EGATE.ShowDialog();

        }
// 
        private void buttonTEST_4_GRIDS_Click(object sender, EventArgs e)
        {
            Form78d_MOBILE_4_Grids NForm78d_MOBILE_4_Grids;
            int WMode = 8; // 
            NForm78d_MOBILE_4_Grids = new Form78d_MOBILE_4_Grids(WOperator, WSignedId, WRMCategoryId, WRMCycleNo, WMode, W_Application);
            NForm78d_MOBILE_4_Grids.ShowDialog();
        }
// ANALYSIS
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form78d_MOBILE_2_Grids NForm78d_MOBILE_2_Grids;
            int WMode = 9; // SHOW ETI375
            NForm78d_MOBILE_2_Grids = new Form78d_MOBILE_2_Grids(WOperator, WSignedId, WRMCategoryId, WRMCycleNo,Mmob.RRNumber ,WMode, W_Application);
            NForm78d_MOBILE_2_Grids.ShowDialog();
        }
// Export to excel 
        private void buttonExportExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string ExcelPath;

            string WDescription = "UnMatched_For_RMCateg_"+ WRMCategoryId+ "..Cycle.." + WRMCycleNo.ToString(); 
            ExcelPath = "C:\\RRDM\\Working\\.." + WDescription + ".xls";

            string WorkingDir = "C:\\RRDM\\Working\\";
            
            XL.ExportToExcel(Mmob.MatchingMasterDataTable_MOBILE, WorkingDir, ExcelPath);
        }
    }
}

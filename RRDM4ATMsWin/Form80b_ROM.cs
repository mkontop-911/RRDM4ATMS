using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80b_ROM : Form
    {

        RRDMMatchingTxns_MasterPoolATMs_ROM Mpa = new RRDMMatchingTxns_MasterPoolATMs_ROM();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        //RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();

        RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMAccountsClass Ac = new RRDMAccountsClass();

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;

        // NOTES END 

        string WSortCriteria;

        int WUniqueNo;
        string WRRN;
        int WSeqNo;

        //int WUniqueTraceNo;

        DateTime FromDt;
        DateTime ToDt;

        string WInputStringField;
        int WInputIntField;

        bool Is_GL_Creation_Auto;

        string WFilter1;
        string WFilter2;
        string WFilterFinal;

        bool FirstCycle;
        bool FirstCycleInvest;

        string ParamId;
        string OccuranceId;

        string WMask;
        string WSubString;

        bool IsRomaniaVersion; 

        DateTime HST_DATE;
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        DateTime WDateTimeA;
        DateTime WDateTimeB;
        string WAtmNo;
        string SavedAtmNo;

        string WCategoryId;

        bool AllowDisputes;

        //bool ViewCatAndCycle; 

        int WRMCycleNo;

        string WStringUniqueId;
        int WIntUniqueId;
        int WUniqueIdType;
        string WFunction;
        string WIncludeNotMatchedYet;
        int WReplCycle;

        public Form80b_ROM(string InSignedId, int InSignRecordNo, string InOperator, DateTime InDateTimeA, DateTime InDateTimeB, string InAtmNo, string InCategoryId, int InRMCycleNo,
                                                 string InStringUniqueId, int InIntUniqueId, int InUniqueIdType,
                                                 string InFunction, string InIncludeNotMatchedYet, int InReplCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WDateTimeA = InDateTimeA;
            WDateTimeB = InDateTimeB;
            WAtmNo = InAtmNo;
            SavedAtmNo = InStringUniqueId;

            WCategoryId = InCategoryId;
            WRMCycleNo = InRMCycleNo;

            WStringUniqueId = InStringUniqueId;
            WIntUniqueId = InIntUniqueId;

            WUniqueIdType = InUniqueIdType;
            // 1 = Investigation General 
            // 2 = WStringUniqueId = Card number - encrypted 
            // 3 = WStringUniqueId = Acc number 
            // 4 = IntUniqueid = UniqueRecordId  
            // 5 = IntUniqueid = ATM Trace Number  
            // 6 = ATM No  
            // 7 = ATM No show all unMatched 
            // 8 = ATM No show all unMatched
            // 13 = WStringUniqueId = RR Number or Reference Number
            // 14 = Presenter Errors

            WFunction = InFunction; // "Reconc", "View", "Investigation"       

            WIncludeNotMatchedYet = InIncludeNotMatchedYet;
            // if "1" = Include not matched yet
            // if ""  = Do not include not matched yet
            //
            WReplCycle = InReplCycle; // This is for WIntUniqueId = 8 

            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

           

            //
            // Check if ROMANIA VERSION
            //

            ParamId = "951";
            OccuranceId = "1";
            //TEST
            IsRomaniaVersion = false;
            Gp.ReadParametersSpecificId(WOperator, ParamId, OccuranceId, "", "");

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES") // ROMANIA 
                {
                    IsRomaniaVersion = true;
                }
                else
                {
                    IsRomaniaVersion = false;
                }

            }

            if (IsRomaniaVersion == true)
            {
                // buttonBT_TXN_BAL.Show();
                linkLabel_TXN_Bal.Show();
            }
            else
            {
                //  buttonBT_TXN_BAL.Hide();
                linkLabel_TXN_Bal.Hide();
            }


            // FIND HISTORY DATE

            ParamId = "853";
            OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true & Gp.OccuranceNm != "")
            {
                if (DateTime.TryParseExact(Gp.OccuranceNm, "yyyy-MM-dd", CultureInfo.InvariantCulture
                              , System.Globalization.DateTimeStyles.None, out HST_DATE))
                {

                }
            }
            else
            {
                HST_DATE = NullPastDate;
            }


            FirstCycle = true;

            // Auto GL
            ParamId = "945";
            OccuranceId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParamId, OccuranceId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_GL_Creation_Auto = true;
                buttonAllAccounting.Show();
            }
            else
            {
                buttonAllAccounting.Hide();
                //buttonRegisterDispute.Hide(); // This is for ABE where Dispute is not given 
            }

            if (WFunction == "View" & WUniqueIdType == 0 & WCategoryId != "" & WRMCycleNo > 0)
            {
                // Set boolean 
                //ViewCatAndCycle = true;

                radioButtonOnlyThisCatandCycle.Checked = true;
            }
            {
                //ViewCatAndCycle = false;
            }

            if (WFunction == "Investigation")
            {
                labelStep1.Text = "Dispute Investigation Process";

                //toolTipButtons.SetToolTip(labelDemoHelp,
                //                        "For Demo Select " + Environment.NewLine
                //                        + "1775, AB104, 1000 Normal Case" + Environment.NewLine
                //                        + "1773, AB104, 800 Missing at host" + Environment.NewLine
                //                        + "12378, AB104, 1100 Presented Error" + Environment.NewLine
                //                        + "12529, AB104, Deposit .. Suspected Notes " + Environment.NewLine
                //                        + "12369,71,72 Broken ATM Disc "
                //                       );
            }
            else
            {
                labelStep1.Text = "Matched and Unmatched Files of RM Category : " + WCategoryId + "/ RM Cycle : " + WRMCycleNo;
                if (WOperator == "BCAIEGCX")
                {
                    labelStep1.Text = "Unmatched Records of RM Category : " + WCategoryId + "/ RM Cycle : " + WRMCycleNo;
                }
            }

            if (WFunction == "View")
            {
                textBoxMsgBoard.Text = "View Only";
            }

            // SELECT

            if (WOperator == "BCAIEGCX" & WFunction != "Investigation")
            {
                comboBoxFilter.Items.Add("UnMatched");
                comboBoxFilter.Text = "UnMatched";
            }
            else
            {
                comboBoxFilter.Items.Add("Matched");

                comboBoxFilter.Items.Add("Both");

                comboBoxFilter.Items.Add("Both With Exceptions");

                comboBoxFilter.Text = "Matched";
            }

            // SORT 

            comboBoxSort.Items.Add("CardNo");

            comboBoxSort.Items.Add("AccountNo");

            comboBoxSort.Items.Add("SeqNo");

            comboBoxSort.Text = "SeqNo";

            //// Unique 

            //comboBoxUnique.Items.Add("SearchInCurrentRMCycle");
            //comboBoxUnique.Items.Add("SearchInAllCyclesThisCateg");
            //comboBoxUnique.Items.Add("SearchInAll-RMCategories");
            //comboBoxUnique.Items.Add("NoUnique");
            //comboBoxUnique.Text = "NoUnique";

            dateTimePicker1.Value = new DateTime(2013, 08, 28);

            if (WFunction == "View" & WUniqueIdType == 0 & WCategoryId != "" & WRMCycleNo > 0)
            {
                comboBoxFilter.Text = "Both";
            }
            {
                //ViewCatAndCycle = false;
            }


            panel5.Hide();

            if (WFunction == "Investigation" &
                (WUniqueIdType == 2 || WUniqueIdType == 3)) // Card, Account
            {
                radioButtonOnlyThisCatandCycle.Checked = false;
                radioButtonAllTxns.Checked = true;
                comboBoxFilter.Text = "Both";
                comboBoxSort.Text = "SeqNo";
                checkBoxUnique.Checked = true;
                FirstCycleInvest = true;
                if (WUniqueIdType == 2) radioButtonCard.Checked = true;
                if (WUniqueIdType == 3) radioButtonAccount.Checked = true;

                panel9.Hide();
                checkBoxUnique.Hide();

                buttonRegisterDispute.Show();

                textBoxInputField.Text = WStringUniqueId;

                dateTimePicker1.Value = WDateTimeA;
                dateTimePicker2.Value = WDateTimeB;

                textBoxMsgBoard.Text = "Change Selection If You Wish. ";
            }
            if (WFunction == "Investigation" &
                                  (WUniqueIdType == 6 || WUniqueIdType == 7)) // AtmNo
                                                                              // 6 all transactions 
                                                                              // 7 all discrepancies for this atm 
            {
                radioButtonOnlyThisCatandCycle.Checked = false;
                radioButtonAllTxns.Checked = true;
                comboBoxFilter.Text = "Both";
                comboBoxSort.Text = "SeqNo";
                checkBoxUnique.Checked = true; // leave it here as true
                FirstCycleInvest = true;

                radioButtonAtm.Checked = true;

                panel9.Hide();
                checkBoxUnique.Hide();

                buttonRegisterDispute.Show();

                textBoxInputField.Text = WStringUniqueId;

                dateTimePicker1.Value = WDateTimeA;
                dateTimePicker2.Value = WDateTimeB;

                textBoxMsgBoard.Text = "Change Selection If You Wish. ";
            }

            if (WFunction == "Investigation" &
                                  WUniqueIdType == 14) // Presenter Errors
            {
                radioButtonOnlyThisCatandCycle.Checked = false;
                radioButtonAllTxns.Checked = true;
                comboBoxFilter.Text = "Both";
                comboBoxSort.Text = "SeqNo";
                checkBoxUnique.Checked = true; // leave it here as true

                FirstCycleInvest = true;

                radioButtonAtm.Checked = false;
                radioButtonPresenterError.Checked = true;

                panel9.Hide();
                checkBoxUnique.Hide();

                buttonRegisterDispute.Show();

                //textBoxInputField.Text = WStringUniqueId;

                textBoxInputField.Text = "ALL Pr Err";

                dateTimePicker1.Value = WDateTimeA;
                dateTimePicker2.Value = WDateTimeB;

                textBoxMsgBoard.Text = "Change Selection If You Wish. ";
            }


            if (WFunction == "Investigation" &
             (WUniqueIdType == 7)) // AtmNo with Unmatched
            {
                radioButtonOnlyThisCatandCycle.Checked = false;
                radioButtonAllTxns.Checked = true;
                comboBoxFilter.Text = "UnMatched";
                comboBoxSort.Text = "SeqNo";
                checkBoxUnique.Checked = true; // Means Unique ATM
                FirstCycleInvest = true;

                if (WUniqueIdType == 7) radioButtonAtm.Checked = true;
                panel9.Hide();
                checkBoxUnique.Hide();

                buttonRegisterDispute.Show();

                textBoxInputField.Text = WStringUniqueId;

                dateTimePicker1.Value = WDateTimeA;
                dateTimePicker2.Value = WDateTimeB;

                textBoxMsgBoard.Text = "Select dates, and other and press Show. ";
            }
            if (WFunction == "Investigation" &
            (WUniqueIdType == 8)) // AtmNo with Unmatched and ReplCycle
            {
                radioButtonOnlyThisCatandCycle.Checked = false;
                radioButtonAllTxns.Checked = true;
                comboBoxFilter.Text = "UnMatched";
                comboBoxSort.Text = "SeqNo";
                checkBoxUnique.Checked = true; // Means Unique ATM
                FirstCycleInvest = true;

                if (WUniqueIdType == 8) radioButtonAtm.Checked = true;
                panel9.Hide();
                checkBoxUnique.Hide();

                buttonRegisterDispute.Show();

                textBoxInputField.Text = WStringUniqueId;

                dateTimePicker1.Value = WDateTimeA;
                dateTimePicker2.Value = WDateTimeB;

                //  textBoxMsgBoard.Text = "Select dates, and other and press Show. ";
            }

            if (WFunction == "Investigation" & WUniqueIdType == 4)  // 4 = IntUniqueid = UniqueRecordId  
            {
                radioButtonOnlyThisCatandCycle.Checked = false;
                radioButtonAllTxns.Checked = true;
                comboBoxFilter.Text = "Both";
                comboBoxSort.Text = "SeqNo";
                checkBoxUnique.Checked = true;
                FirstCycleInvest = true;
                radioButtonUniqueRecordId.Checked = true;

                panel9.Hide();
                checkBoxUnique.Hide();

                buttonRegisterDispute.Show();

                textBoxInputField.Text = WIntUniqueId.ToString();

                textBoxMsgBoard.Text = "This is a unique search ";
            }
            if (WFunction == "Investigation" & WUniqueIdType == 5)  // 5 = Trace No  
            {

                radioButtonOnlyThisCatandCycle.Checked = false;
                radioButtonAllTxns.Checked = true;
                comboBoxFilter.Text = "Both";
                comboBoxSort.Text = "SeqNo";
                checkBoxUnique.Checked = true;
                FirstCycleInvest = true;
                radioButtonTraceNo.Checked = true;

                panel9.Hide();
                checkBoxUnique.Hide();

                dateTimePicker1.Value = WDateTimeA;
                dateTimePicker2.Value = WDateTimeB;

                textBoxAtmNo.Text = WAtmNo;

                buttonRegisterDispute.Show();

                textBoxInputField.Text = WIntUniqueId.ToString();

                textBoxMsgBoard.Text = "This is a unique search ";
            }
            if (WFunction == "Investigation" & WUniqueIdType == 13)  //  = RR Number  
            {

                radioButtonOnlyThisCatandCycle.Checked = false;
                radioButtonAllTxns.Checked = false;
                comboBoxFilter.Text = "Both";
                comboBoxSort.Text = "SeqNo";
                checkBoxUnique.Checked = true;
                FirstCycleInvest = true;
                radioButtonRRNumber.Checked = true;

                panel9.Hide();
                checkBoxUnique.Hide();

                //dateTimePicker1.Value = WDateTimeA;
                //dateTimePicker2.Value = WDateTimeB;

                textBoxAtmNo.Text = WAtmNo;

                buttonRegisterDispute.Show();

                textBoxInputField.Text = WStringUniqueId.ToString();

                textBoxMsgBoard.Text = "This is a unique search ";
            }

            ParamId = "945";
            OccuranceId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParamId, OccuranceId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                AllowDisputes = true;
            }
            else
            {
                AllowDisputes = false;
                buttonRegisterDispute.Hide(); // This is for ABE where Dispute is not given 
                buttonReplPlay.Hide();
            }

        }
        // Load 
        private void Form80b_Load(object sender, EventArgs e)
        {
            if (WIncludeNotMatchedYet == "")
            {
                if (comboBoxFilter.Text == "UnMatched")
                {
                    WFilter2 = " AND IsMatchingDone = 1 AND Matched = 0 ";
                }

                if (comboBoxFilter.Text == "Matched")
                {
                    WFilter2 = " AND IsMatchingDone = 1 AND Matched = 1 ";
                }

                if (comboBoxFilter.Text == "Both")
                {
                    WFilter2 = " AND IsMatchingDone = 1 "; // No condition 
                }

                if (comboBoxFilter.Text == "Both With Exceptions")
                {
                    WFilter2 = " AND IsMatchingDone = 1 AND MetaExceptionId > 0 "; // No condition 
                }
            }
            else
            {
                // Include the Not Matched Yet
                if (WIncludeNotMatchedYet == "1")
                {
                    if (comboBoxFilter.Text == "UnMatched")
                    {
                        WFilter2 = " AND Matched = 0 ";
                    }

                    if (comboBoxFilter.Text == "Matched")
                    {
                        WFilter2 = " AND Matched = 1 ";
                    }

                    if (comboBoxFilter.Text == "Both")
                    {
                        WFilter2 = "  "; // No condition 
                    }

                    if (comboBoxFilter.Text == "Both With Exceptions")
                    {
                        WFilter2 = " AND MetaExceptionId > 0 "; // No condition 
                    }
                }
            }

            if (checkBoxUnique.Checked == false) // Not Unique 
            {
                if (comboBoxSort.Text == "UniqueNo") WSortCriteria = " ORDER BY MaskRecordId ASC";
                if (comboBoxSort.Text == "MobileNo") WSortCriteria = " ORDER BY MobileRequestor ASC";
                if (comboBoxSort.Text == "AccountNo") WSortCriteria = " ORDER BY AccountRequestor ASC";

                if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                {
                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo + " AND SettledRecord = 1 ";

                    if (WFunction == "View" || WFunction == "Investigation")
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo; // Open and closed 

                    }
                }
                if (radioButtonAllTxns.Checked == true) // All transactions
                {
                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND SettledRecord = 1 "; // Only open

                    if (WFunction == "View" || WFunction == "Investigation")
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'"; // Open and closed 
                    }
                }
            }
            else
            {
                //
                // Unique
                //
                if (radioButtonCard.Checked == false & radioButtonAccount.Checked == false
                        & radioButtonUniqueRecordId.Checked == false & radioButtonTraceNo.Checked == false & radioButtonRRNumber.Checked == false
                        & radioButtonAtm.Checked == false & radioButtonPresenterError.Checked == false
                        & radioButtonTxnAmt.Checked == false
                        )
                {
                    MessageBox.Show("Please select and Continue ");
                    return;
                }

                if (textBoxInputField.Text == "")
                {
                    if (WFunction != "Investigation")
                        MessageBox.Show("Please enter value!");
                    return;
                }

                if (radioButtonCard.Checked == true) // Card 
                {
                    WInputStringField = textBoxInputField.Text;
                    if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                        + " AND Card_Encrypted ='" + WInputStringField + "' AND SettledRecord = 1 ";

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                         + " AND Card_Encrypted ='" + WInputStringField + "'";  // Open and closed 
                        }
                    }
                    if (radioButtonAllTxns.Checked == true) // All transactions OR this MAskRecordId
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND Card_Encrypted ='" + WInputStringField + "' AND SettledRecord = 1 ";  // Only open

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND Card_Encrypted ='" + WInputStringField + "'";  // Open and closed 
                            if (WIntUniqueId > 0)
                                WFilter1 = " Where Operator ='" + WOperator + "'" + " AND UniqueRecordId = " + WIntUniqueId; // Only this transaction  
                        }
                    }
                }

                if (radioButtonAccount.Checked == true) // Account 
                {
                    WInputStringField = textBoxInputField.Text;

                    if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                        + " AND AccNumber ='" + WInputStringField + "' AND SettledRecord = 1 ";

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                         + " AND AccNumber ='" + WInputStringField + "'";  // Open and closed 
                        }
                    }
                    if (radioButtonAllTxns.Checked == true) // All transactions
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND AccNumber ='" + WInputStringField + "' AND SettledRecord = 1 ";  // Only open

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND AccNumber ='" + WInputStringField + "'";  // Open and closed 
                        }
                    }

                }
                // ******************
                decimal WInputAmount = 0;
                if (radioButtonTxnAmt.Checked == true) // AMOUNT 
                {
                    WInputStringField = textBoxInputField.Text;

                    if (decimal.TryParse(textBoxInputField.Text, out WInputAmount))
                    {
                    }
                    else
                    {
                        MessageBox.Show(textBoxInputField.Text, "Please enter a valid number for Amount!");

                        return;
                    }

                    if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId
                                   + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                        + " AND TransAmount =" + WInputAmount + " AND SettledRecord = 1 ";

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId
                                + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                         + " AND TransAmount = =" + WInputAmount + "";  // Open and closed 
                        }
                    }
                    if (radioButtonAllTxns.Checked == true) // All transactions for this ATM 
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TransAmount = " + WInputAmount
                            + " AND SettledRecord = 1 " + " AND TerminalId='" + SavedAtmNo + "'";   // Only open

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TransAmount =" + WInputAmount
                                + " AND TerminalId='" + SavedAtmNo + "'";  // Open and closed 
                        }
                    }

                }
                // ******************
                if (radioButtonRRNumber.Checked == true) // RR NUMBER
                {
                    WInputStringField = textBoxInputField.Text;

                    if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                        + " AND RRNumber ='" + WInputStringField + "' AND SettledRecord = 1 "
                                         + " AND TerminalId ='" + textBoxAtmNo.Text + "'";


                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                         + " AND RRNumber ='" + WInputStringField + "'"  // Open and closed 
                                         + " AND TerminalId ='" + textBoxAtmNo.Text + "'";
                        }
                    }
                    if (radioButtonOnlyThisCatandCycle.Checked == false) // unique for this RRN
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RRNumber ='"
                            + WInputStringField + "' AND SettledRecord = 1 "  // Only open
                            + " AND TerminalId ='" + textBoxAtmNo.Text + "'";

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RRNumber ='"
                                + WInputStringField + "'"  // Open and closed 
                              + " AND TerminalId ='" + textBoxAtmNo.Text + "'";
                        }
                    }
                }

                if (radioButtonAtm.Checked == true) // ATM
                {
                    WInputStringField = textBoxInputField.Text;

                    if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                        + " AND TerminalId ='" + WInputStringField + "' AND SettledRecord = 1 ";

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                         + " AND TerminalId ='" + WInputStringField + "'";  // Open and closed 
                        }
                    }
                    if (radioButtonAllTxns.Checked == true) // All transactions
                    {
                        if (WUniqueIdType == 8)
                        {
                            if (WReplCycle == 0)
                            {
                                WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' ";  // 

                                if (WFunction == "View" || WFunction == "Investigation")
                                {
                                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "'";  // Open and closed 
                                }
                            }
                            if (WReplCycle >= 0)
                            {
                                WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' AND ReplCycleNo=" + WReplCycle;  // 

                                if (WFunction == "View" || WFunction == "Investigation")
                                {
                                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' AND ReplCycleNo=" + WReplCycle;   // Open and closed 
                                }

                                labelStep1.Text = "Unmatched for ATM No:.." + WInputStringField + " And Repl Cycle :" + WReplCycle.ToString();
                            }
                        }
                        else
                        {
                            if (WUniqueIdType == 6)
                            {
                                WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' ";  // 

                                if (WFunction == "View" || WFunction == "Investigation")
                                {
                                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "'";  // Open and closed 
                                }
                            }
                            if (WUniqueIdType == 7)
                            {
                                WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' AND IsMatchingDone = 1 AND Matched = 0 ";  // 

                                if (WFunction == "View" || WFunction == "Investigation")
                                {
                                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "'  AND IsMatchingDone = 1 AND Matched = 0 ";  // Open and closed 
                                }
                            }


                        }

                    }

                }

                if (radioButtonPresenterError.Checked == true) // Presenter Error 
                {
                    if (WUniqueIdType == 14)
                    {
                        // To Show All presenter Errors


                        if (radioButtonAllTxns.Checked == true) // All transactions
                        {

                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND MetaExceptionId = 55 ";  // Open and closed 
                        }
                    }
                    else
                    {
                        WInputStringField = textBoxInputField.Text;


                        if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                        {
                            WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                            + " AND TerminalId ='" + WInputStringField + "' AND SettledRecord = 1 ";

                            if (WFunction == "View" || WFunction == "Investigation")
                            {
                                WFilter1 = " Where Operator ='" + WOperator + "'" + " AND RMCateg ='" + WCategoryId + "' AND MatchingAtRMCycle = " + WRMCycleNo
                                             + " AND TerminalId ='" + WInputStringField + "'";  // Open and closed 
                            }
                        }
                        if (radioButtonAllTxns.Checked == true) // All transactions
                        {
                            if (WUniqueIdType == 8)
                            {
                                if (WReplCycle == 0)
                                {
                                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' ";  // 

                                    if (WFunction == "View" || WFunction == "Investigation")
                                    {
                                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "'";  // Open and closed 
                                    }
                                }
                                if (WReplCycle >= 0)
                                {
                                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' AND ReplCycleNo=" + WReplCycle;  // 

                                    if (WFunction == "View" || WFunction == "Investigation")
                                    {
                                        WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' AND ReplCycleNo=" + WReplCycle;   // Open and closed 
                                    }

                                    labelStep1.Text = "Unmatched for ATM No:.." + WInputStringField + " And Repl Cycle :" + WReplCycle.ToString();
                                }
                            }
                            else
                            {
                                WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' AND MetaExceptionId = 55 ";  // 

                                if (WFunction == "View" || WFunction == "Investigation")
                                {
                                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND TerminalId ='" + WInputStringField + "' AND MetaExceptionId = 55 ";  // Open and closed 
                                }
                            }

                        }
                    }



                }

                if (radioButtonUniqueRecordId.Checked == true || radioButtonTraceNo.Checked == true) // Unique Id  or Trace Number 
                {
                    if (int.TryParse(textBoxInputField.Text, out WInputIntField))
                    {
                    }
                    else
                    {
                        MessageBox.Show(textBoxInputField.Text, "Please enter a valid number!");

                        return;
                    }

                    if (radioButtonUniqueRecordId.Checked == true)
                    {
                        WFilter1 = " Where Operator ='" + WOperator + "'"
                                       + " AND UniqueRecordId = " + WInputIntField;  // Open and closed 
                    }
                    if (radioButtonTraceNo.Checked == true)
                    {
                        if (textBoxAtmNo.Text == "")
                        {
                            MessageBox.Show("Please enter AtmNo");
                            return;
                        }
                        else
                        {
                            WAtmNo = textBoxAtmNo.Text;
                        }
                        //// Find Unique Trace Id for searching 


                        //int WUniqueRecordId = Mpa.ReadInPoolTransFindUniqueNumber(WAtmNo, WInputIntField, dateTimePicker1.Value, dateTimePicker2.Value);


                        WFilter1 = " Where Operator ='" + WOperator + "'"
                            + " AND TerminalId ='" + WAtmNo + "'"
                            + " AND TraceNoWithNoEndZero =" + WInputIntField;
                    }
                }
            }

            // FILL TABLE AND SHOW // Only this Categ and Cycle both
            if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this Categ and Cycle both
            {
                WFilterFinal = WFilter1 + " " + WFilter2 + " ";

                WSortCriteria = " ORDER BY TransDate";

                //No Dates Are selected

                FromDt = NullPastDate;
                ToDt = NullPastDate;

                int Mode = 1;

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, Mode, WFilterFinal, WSortCriteria, FromDt, ToDt, 2);

                //Mpa.ReadMatchingTxnsMasterPoolAndFillTable(WOperator, Mode, WFilterFinal, WSortCriteria);         

            }
            // FILL TABLE AND SHOW // All transactions
            if (radioButtonAllTxns.Checked == true) // All transactions
            {
                bool In_HST = false;
                if (dateTimePicker2.Value <= HST_DATE)
                {
                    In_HST = true;
                }
                if (radioButtonCard.Checked == true) // Card 
                {
                    WFilterFinal = WFilter1 + " " + WFilter2;
                }
                else
                {
                    WFilterFinal = WFilter1 + " " + WFilter2 + "  ";
                }

                WSortCriteria = " ORDER BY TransDate,MatchingCateg ASC";
                FromDt = dateTimePicker1.Value.AddDays(-1);
                ToDt = dateTimePicker2.Value.AddDays(1);
                int Mode = 1;

                if (In_HST == false)
                {
                    // Get from Running Data Bases
                    Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable
                        (WOperator, WSignedId, Mode, WFilterFinal, WSortCriteria, FromDt, ToDt, 2);
                }
                else
                {
                    // Get from History Data Bases
                    Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_HST
                        (WOperator, WSignedId, Mode, WFilterFinal, WSortCriteria, FromDt, ToDt, 2);
                }

            }

            //if (radioButtonUniqueNo.Checked == true || radioButtonTraceNo.Checked == true)
            if (radioButtonUniqueRecordId.Checked == true || radioButtonRRNumber.Checked == true)
            {
                // When it comes from Dispute Pre-Investigation this button is false
                WFilterFinal = WFilter1 + " " + WFilter2 + " AND Origin = 'Our Atms' ";

                WSortCriteria = " ORDER BY TransDate";

                //No Dates Are selected

                FromDt = NullPastDate;
                ToDt = NullPastDate;

                int Mode = 1;

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable
                    (WOperator, WSignedId, Mode, WFilterFinal, WSortCriteria, FromDt, ToDt, 2);
            }

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No transactions for this selection");

                panel7.Hide();
                panel21.Hide();
                label11.Hide();
                textBoxMask.Hide();
                textBoxLines.Text = dataGridView1.Rows.Count.ToString();
                if (FromShow == false) this.Dispose();
                return;
            }
            else
            {
                panel7.Show();
                panel21.Show();
                label11.Show();
                textBoxMask.Show();
                textBoxLines.Text = dataGridView1.Rows.Count.ToString();
            }
            //TEST
            if (textBoxInputField.Text == "9962224889" & WIntUniqueId == 0 & dataGridView1.Rows.Count > 2)
            {
                int WRow1 = 3;
                dataGridView1.Rows[WRow1].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            //if (InMode == 1 || InMode == 2 || InMode == 5)
            //{
            //    MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            //    MatchingMasterDataTableATMs.Columns.Add("Status", typeof(string));
            //    MatchingMasterDataTableATMs.Columns.Add("Done", typeof(string));
            //}


            //MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));

          
            //MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            //MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));

            //MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("UTRNNO", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("Trace", typeof(int));

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            dataGridView1.Columns[0].Width = 40; // MaskRecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 40; // Status
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; //  Done
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 65; // MatchingCateg
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].HeaderText = "Categ";

            //MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));

            //MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));

            //MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            //MatchingMasterDataTableATMs.Columns.Add("Mask", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            //MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));

            //MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("UTRNNO", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("Trace", typeof(int));


            dataGridView1.Columns[4].Width = 120; // Descr
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 40; // Ccy
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 80; // Amount
            dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[6].DefaultCellStyle.ForeColor = Color.Red;
           // dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;


            dataGridView1.Columns[7].Width = 40; // Mask
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 100; // Date
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView1.Columns[9].Width = 125; //  Card
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
          //  dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[10].Width = 80; // RRNumber
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[11].Width = 80; // UTRNNO
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[11].HeaderText = "UTRNNO";

            dataGridView1.Columns[12].Width = 80; // Trace
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView1.Columns[13].Width = 50; // acc
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            string TempDesc = "";

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                //bool WSelect = (bool)row.Cells[1].Value;

                TempDesc = (string)row.Cells[4].Value;

                //Dt.ReadDisputeTranByUniqueRecordId(tempUniqueNo);
                if (TempDesc == "REPLENISHMENT")
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }

            }

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No transactions to be posted");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            FirstCycle = false;
        }

        // Unique search 
        private void checkBoxUnique_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUnique.Checked == true)
            {
                panel5.Show();
                buttonShowSelection.Show();
            }
            else
            {
                panel5.Hide();

                radioButtonCard.Checked = false;
                radioButtonAccount.Checked = false;
                radioButtonUniqueRecordId.Checked = false;

                textBoxInputField.Text = "";
                buttonShowSelection.Hide();
            }
        }
        // Radio Button one only cat/cycle
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOnlyThisCatandCycle.Checked == true)
            {
                panel6.Hide();
                buttonShowSelection.Hide();
                if (FirstCycle == false)
                    Form80b_Load(this, new EventArgs());
            }
            else
            {
                panel6.Show();
                buttonShowSelection.Show();
            }
        }
        // Radio two all Trans 
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButtonAllTxns.Checked == true)
            {
                panel6.Show();
                buttonShowSelection.Show();
                labelStep1.Text = "Transactions For All Categories As Per Selection";
                //  MessageBox.Show("Make Dates Selection and press Show button."); 
            }
            else
            {
                panel6.Hide();
                buttonShowSelection.Hide();
                labelStep1.Text = "Matched and Unmatched Records of RM Category : " + WCategoryId + "/ RM Cycle : " + WRMCycleNo;

            }

        }

        // On ROW ENTER 
        int WWUniqueRecordId;
        bool IsReversal;
        bool JournalFound;
        string Message;
        string DiscrepancyMessage;

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WUniqueNo = (int)rowSelected.Cells[0].Value;
            WFilterFinal = " Where  UniqueRecordId = " + WUniqueNo;

            bool In_HST = false;
            if (dateTimePicker2.Value <= HST_DATE)
            {
                In_HST = true;
            }

            //if (In_HST == false)
            //{
            //    // Get from Running Data Bases
            //    Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable
            //        (WOperator, WSignedId, Mode, WFilterFinal, WSortCriteria, FromDt, ToDt, 2);
            //}
            //else
            //{
            //    // Get from History Data Bases
            //    Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_HST
            //        (WOperator, WSignedId, Mode, WFilterFinal, WSortCriteria, FromDt, ToDt, 2);
            //}
            if (In_HST == true)
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(WFilterFinal, 2);
            }
            else
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WFilterFinal, 2);
            }

            // Show what is needed

            if (IsRomaniaVersion == true & Mpa.TransDescr == "REPLENISHMENT")
            {
                linkLabel_TXN_Bal.Hide();
                linkLabelSMLines.Show();
                linkLabel_SM_Cassettes.Show();
                panel21.Hide(); 
            }
            else
            {
                linkLabel_TXN_Bal.Show();
                linkLabelSMLines.Hide();
                linkLabel_SM_Cassettes.Hide();
                panel21.Show();
            }

            WSeqNo = Mpa.SeqNo;

            if (Mpa.Origin == "Our Atms" & Mpa.MetaExceptionId == 55 & (Mpa.MatchMask == "11" || Mpa.MatchMask == "10"))
            {
                linkLabelForRelationsToPresenterError.Show();
            }
            else
            {
                linkLabelForRelationsToPresenterError.Hide();
            }

            if (Mpa.Origin == "Our Atms")
            {
                buttonTextFromJournal.Show();
                buttonShowJournal.Show();
                //button5.Show();
                //buttonPOS.Hide();
                string ParId = "945";
                string OccurId = "1";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                if (Gp.OccuranceNm == "YES")
                {
                    buttonRegisterDispute.Show();
                }
                else
                {
                    AllowDisputes = false;
                    buttonRegisterDispute.Hide(); // This is for ABE where Dispute is not given 
                    buttonReplPlay.Hide();
                }

                IsReversal = false;
                Message = "";

                RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();

                if (In_HST == true)
                {
                    Jt.ReadJournalTextByTrace_HST(WOperator, Mpa.TerminalId, Mpa.MasterTraceNo, Mpa.TransDate.Date);
                }
                else
                {
                    Jt.ReadJournalTextByTrace(WOperator, Mpa.TerminalId, Mpa.MasterTraceNo, Mpa.TransDate.Date);
                }

                if (Jt.RecordFound == true)
                {
                    JournalFound = true;

                    Message = "";
                }
                else
                {
                    JournalFound = false;
                }

                if (Mpa.NotInJournal == true & JournalFound == false)
                {
                    labelJournalNm.Hide();
                    textBoxJournalNm.Hide();
                }
                else
                {

                    labelJournalNm.Show();
                    textBoxJournalNm.Show();
                    RRDMReconcFileMonitorLog Rflog = new RRDMReconcFileMonitorLog();
                    Rflog.GetRecordByFuid(Mpa.FuID);
                    textBoxJournalNm.Text = Rflog.FileName;

                }

                // If First position is Zero check if TXN has been reversed

                // Check if reversal at Target
               // string WWMask = Mpa.MatchMask;
                string TableId = "";

            }
            else
            {
                // JCC 
                buttonTextFromJournal.Hide();
                //buttonRegisterDispute.Hide();
                buttonShowJournal.Hide();
                //button5.Show();
                //buttonPOS.Show();
            }

            IsReversal = false;

            DiscrepancyMessage = "";
            // 
            if (Mpa.MatchMask.Contains("R") == true)
            {
                DiscrepancyMessage = "There is Reversal in the matching records. " + Environment.NewLine
                             + "If in position there is letter R then this was Reversed. " + Environment.NewLine
                             + "See them by pressing Source Records Button ";

                IsReversal = true;
            }

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

            textBox10.Text = Mpa.RMCateg;
            textBoxTerminalId.Text = Mpa.TerminalId;
            //Mpa.TerminalId = "00000574";
            string Wselection = " WHERE AtmNo='" + Mpa.TerminalId + "'";
            Ac.ReadAccountTableBySelectionCriteria(Wselection);

            if (Ac.RecordFound == true)
            {
                labelBranch.Show();
                textBoxBranch.Show();
                textBoxBranch.Text = Ac.BranchId;
            }
            else
            {
                labelBranch.Hide();
                textBoxBranch.Hide();
            }


            textBoxRmCycle.Text = Mpa.MatchingAtRMCycle.ToString();

            // textBoxCardNo.Text = En.EncryptField(Mpa.CardNumber);
            textBoxCardNo.Text = Mpa.CardNumber;
            textBoxAccNo.Text = Mpa.AccNumber;
            textBoxCurr.Text = Mpa.TransCurr;
            textBoxAmnt.Text = Mpa.TransAmount.ToString("#,##0.00");
            textBoxDtTm.Text = Mpa.TransDate.ToString();

            if (IsRomaniaVersion == true)
            {
                label14.Text = "UTRNNO:";
                textBoxTraceNo.Text = Mpa.UTRNNO;
            }
            else
            {
                textBoxTraceNo.Text = Mpa.TraceNoWithNoEndZero.ToString();
            }

            

            textBoxRRN.Text = Mpa.RRNumber;
            textBoxA_No.Text = Mpa.AUTHNUM;
            textBoxTXNSRC.Text = Mpa.TXNSRC;
            textBoxTXNDEST.Text = Mpa.TXNDEST;

            //if (Mpa.TraceNoWithNoEndZero > 0 & Mpa.Origin == "Our Atms")
            //{
            //    textBoxTraceNo.Text = Mpa.TraceNoWithNoEndZero.ToString();
            //    //  textBoxLineDetails.Text = "DETAILS OF SELECTED";
            //}


            if (Mpa.IsMatchingDone == false)
            {
                label22.Show();
                textBoxUnMatchedType.Hide();
                buttonSourceReords.Enabled = false;
                //  buttonTransTrail.Hide();
            }
            else
            {
                label22.Hide();
                textBoxUnMatchedType.Show();
                buttonSourceReords.Enabled = true;
                // buttonTransTrail.Show();
            }

            // if (Mpa.RMCateg.Substring(0, 4) == "RECA")
            if (Mpa.Origin == "Our Atms")
            {

                RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                int TempReplNo = Ta.ReadFindReplCycleForGivenDate(Mpa.TerminalId, Mpa.TransDate);
                Ta.ReadSessionsStatusTraces(Mpa.TerminalId, TempReplNo);
                if (Ta.RecordFound & Ta.ProcessMode > 0)
                {
                    buttonReplPlay.Enabled = true;
                    buttonReplPlay.BackColor = Color.White;
                }
                else
                {
                    buttonReplPlay.Enabled = false;
                    buttonReplPlay.BackColor = Color.Silver;
                }

                buttonTextFromJournal.Show();
                buttonShowJournal.Show();
                //button5.Show();
                //labelTextFromJournal.Show();
                //label25.Show();
                //label19.Show();

                //Tp.ReadInPoolTransSpecific(Mpa.OriginalRecordId); // Read Transactions details 
                //ATMTrans = true;
            }
            else
            {
                //buttonReplPlay.Hide();
                buttonReplPlay.Enabled = false;
                buttonReplPlay.BackColor = Color.Silver;
                buttonTextFromJournal.Hide();
                buttonShowJournal.Hide();
                //button5.Hide();
                //labelTextFromJournal.Hide();
                //label25.Hide();
                //label19.Hide();
                //ATMTrans = false;
            }
            // FIX SEPT 2019
            Mc.ReadMatchingCategorybyActiveCategId(WOperator, Mpa.MatchingCateg);

            if (Mc.TWIN == true)
            {
                buttonTextFromJournal.Show();
                //  button4.Show();
                //labelTextFromJournal.Show();
                //button5.Show();
                //buttonPOS.Hide();
                //  linkLabelForRelationsToPresenterError.Show(); 
            }
            else
            {
                //  linkLabelForRelationsToPresenterError.Hide();
            }

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
            if (Rcs.RecordFound == true)
            {
                if (Rcs.EndReconcDtTm == NullPastDate)
                {
                    buttonReconcPlay.Enabled = false;
                    buttonReconcPlay.BackColor = Color.Silver;
                }
                else
                {
                    buttonReconcPlay.Enabled = true;
                    buttonReconcPlay.BackColor = Color.White;
                    buttonTextFromJournal.Show();
                }
            }
            else
            {
                // Not Found or not Matching Yet 
                buttonReconcPlay.Enabled = false;
                buttonReconcPlay.BackColor = Color.Silver;
            }

            textBoxMask.Text = Mpa.MatchMask; 
            // Check for exceptions 
            if (Mpa.ActionType != "00")
            {
                labelAction.Show();
                panel3.Show();

                labelActionTaken.Show();
                textBoxActionTaken.Show();
                textBoxMaker.Show();
                textBoxAuth.Show();
                labelMaker.Show();
                labelAuth.Show();

                Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", Mpa.UniqueRecordId, Mpa.ActionType);

                if (Aoc.RecordFound == true)
                {
                    textBoxActionTaken.Text = Aoc.ActionNm;

                    textBoxMaker.Text = Aoc.Maker;
                    textBoxAuth.Text = Aoc.Authoriser;
                }
               

            }
            else
            {
                labelAction.Hide();
                panel3.Hide();

                labelActionTaken.Hide();
                textBoxActionTaken.Hide();
                labelMaker.Hide();
                labelAuth.Hide();
                textBoxMaker.Hide();
                textBoxAuth.Hide();
            }

            labelReversal.Hide();

            Tp.ReadTransToBePostedSpecificByUniqueRecordId(Mpa.UniqueRecordId);
            if (Tp.RecordFound == true)
            {
                //labelActionTaken.Show();
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
                //labelActionTaken.Hide();
                panel8.Hide();
            }

            if (Mpa.ActionType == "04")
            {
                // labelActionTaken.Show();
                panelForceMatched.Show();
                textBoxForceReason.Text = Mpa.MatchedType;

            }
            else
            {
                panelForceMatched.Hide();
            }

            if (Mpa.Matched == false)
            {
                if (Message == "")
                    textBoxUnMatchedType.Text = Mpa.UnMatchedType;
                else textBoxUnMatchedType.Text = Message;

            }
            else textBoxUnMatchedType.Text = "Matched";


            if (Mpa.UnMatchedType == "DUPLICATE")
            {
                if (Mpa.MatchMask == "") WMask = "000";
                else WMask = Mpa.MatchMask;
            }
            else
            {
                WMask = Mpa.MatchMask;
            }

            ShowMask(WMask);

            //bool PresenterError; 

            //if (Mpa.MetaExceptionId == 55)
            //{
            //    PresenterError = true;
            //}
            //else
            //{
            //    PresenterError = false;
            //}


            if (DiscrepancyMessage == "")
            {
                textBoxUnMatchedType.Text = Mpa.UnMatchedType;
                if (Mpa.MetaExceptionId == 55) textBoxUnMatchedType.Text = "PRESENTER ERROR";

            }
            else textBoxUnMatchedType.Text = DiscrepancyMessage;


            WRRN = Mpa.RRNumber;

            // Check if dispute already registered for this transaction 
            textBoxPostponed.Hide();

            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
            // Check if already exist
            //+"  SeqNo06 = 95 " // HERE WE KEEP the SOLO 
            //                    + "  AND SeqNo05=" + WRMCycleNo;
          
            Dt.ReadDisputeTranByUniqueRecordId(Mpa.UniqueRecordId);
            
            if (Dt.RecordFound == true & 
                (Dt.DisputeActionId == 1
                || Dt.DisputeActionId == 2
                || Dt.DisputeActionId == 3
                || Dt.DisputeActionId == 4
                
                )
                ) // Found and not cancelled
            {
                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
                linkLabelDispute.Show();
                //WFoundDisp = Dt.DisputeNumber;
                if (WFunction == "Investigation" & WWUniqueRecordId == 0)
                    MessageBox.Show("Dispute with no : " + Dt.DisputeNumber.ToString() + " already registered for this transaction.");
                labelDispute.Show();
                textBoxDisputeId.Show();
                buttonRegisterDispute.Hide();
                buttonCreateTXNSolo.Hide();
                textBoxPostponed.Hide();
            }
            else
            {
                labelDispute.Hide();
                textBoxDisputeId.Hide();
                linkLabelDispute.Hide();
                if (Dt.RecordFound == true &
                (      
                  Dt.DisputeActionId == 5 // Dispute Postponed 
                || Dt.DisputeActionId == 6 // Dispute Cancel 
                || Dt.DisputeActionId == 0 // No Final Action Yet
                ))
                {
                    textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
                    linkLabelDispute.Show();
                    //WFoundDisp = Dt.DisputeNumber;
                    if (WFunction == "Investigation" & WWUniqueRecordId == 0)
                        MessageBox.Show("Dispute with no : " + Dt.DisputeNumber.ToString() + " already registered for this transaction.");
                    labelDispute.Show();
                    textBoxDisputeId.Show();
                    if (Dt.DisputeActionId == 5 || Dt.DisputeActionId == 6)
                    {
                        textBoxPostponed.Show();
                        
                        if (Dt.DisputeActionId == 5)
                        {
                            textBoxPostponed.Text = "Postponed for.." + Dt.PostDate.ToShortDateString();
                        }
                        if (Dt.DisputeActionId == 6)
                        {
                            textBoxPostponed.Text = "Canceled at.." + Dt.ActionDtTm.ToShortDateString();
                        }
                    }
                    else
                    {
                        textBoxPostponed.Hide();
                    }
                    
                }
                if (Mpa.Origin == "Our Atms")
                {
                    //if (WFunction == "Investigation")
                    //{
                        if (AllowDisputes == true)
                        {
                        // Allow for disputes 
                           buttonRegisterDispute.Show();
                           buttonCreateTXNSolo.Show();
                        }

                        Tp.ReadTransToBePostedSpecificByUniqueRecordId(Mpa.UniqueRecordId);
                        if (Tp.RecordFound == true)
                        {
                            // Not allowed Disputes if action is taken on transaction
                            buttonRegisterDispute.Hide();
                            buttonCreateTXNSolo.Hide();
                    }
                    //}

                }
            }

            if (WOperator == "BDACEGCA")
            {
                // ABE
                buttonAllAccounting.Hide();
            }
        }


        //
        // Show info for Mask
        //
        public void ShowMask(string InMask)
        {

            //****************************************************************************
            //Translate MASK 
            //****************************************************************************

            WMask = InMask;

            if (WMask == "")
            {
                WMask = "EEE";
            }
            // First Line
            if (Mpa.FileId01 != "")
            {
                labelFileA.Show();
                textBox31.Show();

                labelFileA.Text = "File A : " + Mpa.FileId01;
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
            if (Mpa.FileId02 != "")
            {
                labelFileB.Show();
                textBox32.Show();

                labelFileB.Text = "File B : " + Mpa.FileId02;
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
            if (Mpa.FileId03 != "")
            {
                labelFileC.Show();
                textBox33.Show();

                labelFileC.Text = "File C : " + Mpa.FileId03;
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

            // Forth Line 
            if (Mpa.FileId04 != "")
            {
                labelFileD.Show();
                textBox34.Show();

                labelFileD.Text = "File D : " + Mpa.FileId04;
                labelFileD.Show();
                WSubString = WMask.Substring(3, 1);

                if (WSubString == "1")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox34.BackColor = Color.Red;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = WSubString;
                }
            }
            else
            {
                labelFileD.Hide();
                textBox34.Hide();
            }

            // Fifth Line 
            if (Mpa.FileId05 != "")
            {
                labelFileE.Show();
                textBox35.Show();

                labelFileE.Text = "File E : " + Mpa.FileId05;
                labelFileE.Show();
                WSubString = WMask.Substring(4, 1);


                if (WSubString == "1")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox35.BackColor = Color.Red;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = WSubString;
                }

            }
            else
            {
                labelFileE.Hide();
                textBox35.Hide();
            }
            // sixth Line 
            if (Mpa.FileId06 != "")
            {
                labelFileF.Show();
                textBox36.Show();

                labelFileF.Text = "File F : " + Mpa.FileId06;
                labelFileF.Show();
                WSubString = WMask.Substring(5, 1);

                if (WSubString == "1")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox36.BackColor = Color.Red;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = WSubString;
                }
            }
            else
            {
                labelFileF.Hide();
                textBox36.Hide();
            }
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
        string P1;
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print 
            //comboBoxFilter.Items.Add("Matched");

            //comboBoxFilter.Items.Add("UnMatched");

            ////comboBoxFilter.Items.Add("UnMatchedAllCycles");

            //comboBoxFilter.Items.Add("Both");
            Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

            if (checkBoxUnique.Checked == false)
            {
                P1 = comboBoxFilter.Text + " Transactions";
            }
            if (checkBoxUnique.Checked == true)
            {
                P1 = "Transactions for unique Selection:" + Mpa.UniqueRecordId.ToString();

                if (radioButtonAtm.Checked == true) P1 = "Transactions for Atm No :" + textBoxInputField.Text;
            }

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId;

            Form56R54ATMS ReportATMS54 = new Form56R54ATMS(P1, P2, P3, P4, P5);
            ReportATMS54.Show();
        }
        bool FromShow;
        // SHOW Selection 
        private void buttonShowSelection_Click(object sender, EventArgs e)
        {
            //
            // Validate Input Fields for Invalid Characters
            //
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                (@"^[a-zA-Z0-9]*$");

            if (expr.IsMatch(textBoxInputField.Text))
            {
                //   MessageBox.Show("field");
            }
            else
            {
                MessageBox.Show("invalid Characters In Input Field");
                return;
            }

            if (radioButtonTraceNo.Checked == true || radioButtonAtm.Checked == true)
            {

                if (expr.IsMatch(textBoxAtmNo.Text))
                {
                    //   MessageBox.Show("field");
                }
                else
                {
                    MessageBox.Show("invalid Characters In Input Field");
                    return;
                }
            }

            FromShow = true;

            // load again
            Form80b_Load(this, new EventArgs());
        }
        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // ON COMBO CHANGE LOAD 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FirstCycle == false)
            {
                Form80b_Load(this, new EventArgs());
            }
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
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form78b NForm78b;
            string WHeader = "LIST OF TRANSACTIONS";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mpa.MatchingMasterDataTableATMs, WHeader, "Form80b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }
        // Show Trans to Be posted 
        private void buttonTranPosted_Click(object sender, EventArgs e)
        {
            Form78 NForm78;



            NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator,
                                           "", 0, Mpa.UniqueRecordId, 1);
            NForm78.ShowDialog();



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
                    SM.Read_SM_Record_Specific_By_ATMno_ReplCycle(Mpa.TerminalId, Mpa.ReplCycleNo);
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
                    NForm51_Recycle.FormClosed += NForm5_FormClosed;
                    NForm51_Recycle.ShowDialog();
                }
                else
                {
                    // Current Bank De Caire Type 
                    NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                    NForm51.FormClosed += NForm5_FormClosed;
                    NForm51.ShowDialog();
                }
                //NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                //NForm51.FormClosed += NForm5_FormClosed;
                //NForm51.ShowDialog();
            }
            else
            {
                MessageBox.Show("No Replenishement done for this ATM and this Replen Cycle");
                return;
            }

        }

        // PlayBack For reconciliation cash CASH 
        private void buttonReconcCash_Click(object sender, EventArgs e)
        {
            WCategoryId = Mpa.RMCateg;

            //Form52b_XXXX NForm52b;
            int WRMCycle = 205;
            //if (WSignedId == Ap.Requestor)
            //{
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54; // View
            Usi.WFieldChar1 = WCategoryId;
            Usi.WFieldNumeric1 = WRMCycle;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
            //}

            //NForm52b = new Form52b_XXXX(WSignedId, WSignRecordNo, WOperator, 11, WCategoryId, 205);
            ////NForm52b.FormClosed += NForm52b_FormClosed;
            //NForm52b.ShowDialog();

        }
        // Reconciliation Play 
        private void buttonReconcPlay_Click(object sender, EventArgs e)
        {
            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
            if (Rcs.EndReconcDtTm == NullPastDate)
            {
                MessageBox.Show("Reconciliation Not done yet!");
                return;
            }


            // Update Us Process number
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54; // View Only 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form271 NForm271;

            NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
            NForm271.FormClosed += NForm271_FormClosed;
            NForm271.ShowDialog();

            Form80b_Load(this, new EventArgs());

        }
        // Text From Journal 
        private void button3_Click(object sender, EventArgs e)
        {
            // Show Lines of journal 
            string SelectionCriteria = " WHERE UniqueRecordId =" + Mpa.UniqueRecordId;

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
            Form67_ROM NForm67_ROM;

            int Mode = 5; // Specific
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
            NForm67_ROM = new Form67_ROM(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_ROM.ShowDialog();

            //bool In_HST = false;

            //if (Mpa.TransDate.Date <= HST_DATE)
            //{
            //    In_HST = true;
            //}
            //// Show Lines of journal 
            //SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;
            //if (In_HST == true)
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            //}
            //else
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //}
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
                Jt.ReadJournalTxnsByParameters_HST(WOperator, Mpa.TerminalId, Mpa.AtmTraceNo*10, Mpa.TransAmount, Mpa.CardNumber, Mpa.TransDate.Date);
            }
            else
            {
                Jt.ReadJournalTxnsByParametersROM(WOperator, Mpa.TerminalId, Mpa.AtmTraceNo*10, Mpa.TransAmount, Mpa.CardNumber, Mpa.TransDate.Date);
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
            Form67_ROM NForm67_ROM;

            Mode = 3; // Specific Journal 
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
            NForm67_ROM = new Form67_ROM(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_ROM.ShowDialog();

            return;
            // }


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
            if (Mpa.ActionType != "00" & Mpa.SettledRecord == false)
            {
                // Transaction uder reconciliation or replenishment 
                MessageBox.Show("Record is not settled yet. "+Environment.NewLine
                 + "It might be under workflow process. " + Environment.NewLine
                 + "You cannot open a dispute. " + Environment.NewLine
                    );
                return; 
            }
            // Continue for cases where action was taken but no money were affected. 
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

        // Card Number 
        private void radioButtonCard_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCard.Checked == true & FirstCycleInvest == false)
            {
                textBoxInputField.Text = "";
            }
            FirstCycleInvest = false;
        }

        // Account Number 
        private void radioButtonAccount_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAccount.Checked == true & FirstCycleInvest == false)
            {
                textBoxInputField.Text = "";
            }
            FirstCycleInvest = false;
        }
        // Search by AMNT
        private void radioButtonTxnAmt_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTxnAmt.Checked == true & FirstCycleInvest == false)
            {
                textBoxInputField.Text = "";
            }
            FirstCycleInvest = false;
        }

        // Unique number 
        private void radioButtonUniqueNo_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonUniqueRecordId.Checked == true & FirstCycleInvest == false)
            {
                textBoxInputField.Text = "";
            }
            FirstCycleInvest = false;
        }
        // Trace No
        private void radioButtonTraceNo_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTraceNo.Checked == true & FirstCycleInvest == false)
            {
                textBoxAtmNo.Text = "";
                textBoxInputField.Text = "";
            }
            FirstCycleInvest = false;

            if (radioButtonTraceNo.Checked == true)
            {
                label13.Show();
                textBoxAtmNo.Show();
            }
            else
            {
                label13.Hide();
                textBoxAtmNo.Hide();
            }

        }
        // ATM
        private void radioButtonAtm_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAtm.Checked == true & FirstCycleInvest == false)
            {
                textBoxInputField.Text = "";
            }
            FirstCycleInvest = false;
        }
        // Link to disputes 
        Form3 NForm3;
        private void linkLabelDispute_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator, textBoxDisputeId.Text, NullPastDate, NullPastDate, 13);
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
            Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES
            Form78d_AllFiles_BDC_3_ROM NForm78d_AllFiles_BDC_3_ROM;

            if (IsRomaniaVersion == true)
            {
                NForm78d_AllFiles_BDC_3_ROM = new Form78d_AllFiles_BDC_3_ROM(WOperator, WSignedId, Mpa.UniqueRecordId, 1);
                NForm78d_AllFiles_BDC_3_ROM.FormClosed += NForm5_FormClosed;
                NForm78d_AllFiles_BDC_3_ROM.ShowDialog();
            }
            else
            {
                NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, Mpa.UniqueRecordId, 1);
                NForm78d_AllFiles_BDC_3.FormClosed += NForm5_FormClosed;
                NForm78d_AllFiles_BDC_3.ShowDialog();
            }

        }
        // Decrypt
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We cannot Decrypted the Card no " + Environment.NewLine
                            + "We must have the Bank's decription routine to do so.  "
                            );
            return;
            string WCardNo = En.DecryptField(textBoxCardNo.Text);

            MessageBox.Show("The Decrypted Card Is: " + Environment.NewLine
                            + WCardNo);
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }
        // SHOW Point of Sale info
        private void buttonPOS_Click(object sender, EventArgs e)
        {

            if (Mpa.Origin == "Our Atms")
            {
                MessageBox.Show("Not Allowed Operation");
                return;
            }
            if (WOperator == "BCAIEGCX")
            {
                MessageBox.Show("Not Allowed Operation for this Bank");
                return;
            }

            RRDMMatchingOfTxnsFindOriginRAW Msr = new RRDMMatchingOfTxnsFindOriginRAW();

            bool WRecordFound =
                Msr.FindRawRecordFromMasterRecord
                  (WOperator, Mpa.FileId01, Mpa.MatchingAtRMCycle,
                  Mpa.TransDate.Date,
                   Mpa.TerminalId, Mpa.TraceNoWithNoEndZero, Mpa.RRNumber, 2);
            //(WOperator, Mpa.FileId01, Mpa.MatchingAtRMCycle, Mpa.TerminalId, Mpa.RRNumber, 2);

            string WHeader = "MERCHANT DETAILS FOR Transaction with RRN : " + Mpa.RRNumber.ToString();
            string WCardNumber = Mpa.CardNumber;
            string WAccNo = Mpa.AccNumber;
            string WDateTime = Mpa.TransDate.ToString();
            string WAmount = Mpa.TransAmount.ToString("#,##0.00");
            string WCcy = Mpa.TransCurr;
            string WAutorisationCd = Msr.AuthorisationId;
            string WMerchantId = Msr.MerchantId;
            string WMerchantName = Msr.MerchantNm;
            string WTranDescription = Msr.TranDescription;

            Form2Merchant NForm2Merchant;

            NForm2Merchant = new Form2Merchant(WOperator, WHeader, WCardNumber, WAccNo,
                                             WDateTime, WAmount, WCcy, WAutorisationCd,
                                                    WMerchantId, WMerchantName, WTranDescription);
            NForm2Merchant.Show();
        }
        // Near Journal 
        int IntTrace;

        public string ParamId1 { get => ParamId; set => ParamId = value; }

        private void buttonJournal_Near_Click(object sender, EventArgs e)
        {
            // Here we search the journal lines close to the selected transaction
            // If selected already in Journal then we show the one before and the one after
            // If transaction not in journal say 011 then we show 5 minutes before and 5 minutes after. 
            // This is because the time at mainframe maybe different that this of ATM. 

            WSubString = WMask.Substring(0, 1);

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
            int SaveSeqNo;

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
            if (IsRomaniaVersion == true)
            {
                Form67_ROM NForm67_ROM;

                int Mode = 5; // Specific range
                string WTraceRRNumber = Mpa.UTRNNO;

                // if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

                NForm67_ROM = new Form67_ROM(WSignedId, 0, WOperator, 0, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                          SavedTransDate, NullPastDate, Mode);
                NForm67_ROM.ShowDialog();
            }
            else
            {
                Form67_BDC NForm67_BDC;

                int Mode = 5; // Specific range
                string WTraceRRNumber = Mpa.UTRNNO;

                if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                          SavedTransDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }


            //



        }
        // Txns At External Category 
        private void linkLabelForRelationsToPresenterError_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int PreviousUniqueRecordId = Mpa.UniqueRecordId;

            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            string WSelectionCriteria = " WHERE TraceNo =" + Mpa.TraceNoWithNoEndZero + " AND TerminalId ='" + Mpa.TerminalId + "'"
                                         + " AND RRNumber ='" + Mpa.RRNumber + "'";
            string WTableName = "Switch_IST_Txns_TWIN";

            Mgt.ReadTransSpecificFromBothTables_By_SelectionCriteria(WSelectionCriteria, WTableName);


            if (Mgt.RecordFound == true)
            {
                WSelectionCriteria = " WHERE MatchingCateg='" + Mgt.MatchingCateg + "'"
                                     + " AND TerminalId ='" + Mpa.TerminalId + "'"
                                     + " AND RRNumber ='" + Mpa.RRNumber + "'"
                                     + " AND TraceNoWithNoEndZero = " + Mpa.TraceNoWithNoEndZero
                                     ;
                Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2);

            }

            if (Mpa.RecordFound == true)
            {
                int NewUniqueRecordId = Mpa.UniqueRecordId;

                // Re-establish Mpa
                WSelectionCriteria = " WHERE UniqueRecordId=" + PreviousUniqueRecordId;

                Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2);


                Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

                NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, NewUniqueRecordId, 1);

                NForm78d_AllFiles_BDC_3.ShowDialog();
            }
            else
            {
                MessageBox.Show("No related txns yet. ");
            }

        }
        // RR NUMBER 
        private void radioButtonRRNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRRNumber.Checked == true & FirstCycleInvest == false)
            {
                textBoxAtmNo.Text = "";
                textBoxInputField.Text = "";
            }
            FirstCycleInvest = false;

            if (radioButtonRRNumber.Checked == true)
            {
                label13.Show();
                textBoxAtmNo.Show();
            }
            else
            {
                label13.Hide();
                textBoxAtmNo.Hide();
            }
        }

        // Show IST presenter errors
        private void buttonShow_IST_Presenter_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Functionality was moved to controller");
            //string FileId = "Switch_IST_Txns"; 
            //Form78d_FileRecords_IST_PRESENTER NForm78d_FileRecords_IST_PRESENTER;
            //NForm78d_FileRecords_IST_PRESENTER = new Form78d_FileRecords_IST_PRESENTER(WOperator, WSignedId, FileId, WDateTimeA, WDateTimeB, 0);
            //NForm78d_FileRecords_IST_PRESENTER.ShowDialog();

        }
        // Show All Actions
        private void buttonAllActions_Click(object sender, EventArgs e)
        {

            string WSelectionCriteria = "WHERE UniqueKey =" + Mpa.UniqueRecordId + " AND UniqueKeyOrigin = 'Master_Pool' ";

            Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

            string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            int WMode = 3; // Actions 
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();
            //*******************************************

        }
        // All Accounting
        private void buttonAllAccounting_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            Aoc.ClearTableTxnsTableFromAction();

            string WSelectionCriteria = "WHERE UniqueKey =" + Mpa.UniqueRecordId + " AND UniqueKeyOrigin = 'Master_Pool' ";

            Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

            int I = 0;

            while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
            {
                //    RecordFound = true;
                int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                if (Aoc.Is_GL_Action == true)
                {

                    int WMode2 = 1; // DO NOT Create transaction in pool 
                    string WCallerProcess = "Reconciliation";
                    Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey,
                                                                 Aoc.ActionId, Aoc.Occurance, WCallerProcess, WMode2);
                }

                I = I + 1;
            }

            DataTable TempTxnsTableFromAction;

            string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;

            // Aoc.ReadActionsTxnsCreateTableByUniqueKey(WUniqueRecordIdOrigin, Dt.UniqueRecordId, "All");

            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;
            //Form14b_All_Actions NForm14b_All_Actions;
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();

        }
        // Show Video 
        private void buttonVideo_Click(object sender, EventArgs e)
        {
            // Based on Transaction No show video clip 
            //TEST
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }
        // REGISTER MY DISPUTE 
        private void buttonRegisterMyDispute_Click(object sender, EventArgs e)
        {
            
            if (Mpa.ActionType != "00" & Mpa.SettledRecord == false)
            {
                // Transaction uder reconciliation or replenishment 
                MessageBox.Show("Record is not settled yet. " + Environment.NewLine
                 + "It might be under workflow process. " + Environment.NewLine
                 + "You cannot open a dispute. " + Environment.NewLine
                    );
                return;
            }
            // Continue for cases where action was taken but no money were affected. 
            Form5 NForm5;
            int From = 22; // Coming from Pre-Investigattion ATMs and the created dispute to be on user Name 
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Mpa.CardNumber, WWUniqueRecordId, 0, 0, "", From, "ATM");
            NForm5.FormClosed += NForm5_FormClosed;
            NForm5.ShowDialog();
        }
// Play Back IST
        private void buttonPlayBack_IST_Click(object sender, EventArgs e)
        {
            //if (WProcessMode > 0)
            //{
             //   RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
             //   Usi.ReadSignedActivityByKey(WSignRecordNo);
             //   Usi.ProcessNo = 54; // View only for replenishment already done  
             //   Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


             //   // CALL THE SAME If Recycle or not 
             //   Form51_Repl_For_IST NForm51_Repl_For_IST;
             //   NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
             ////   NForm51_Repl_For_IST.FormClosed += NForm51_FormClosed;
             //   NForm51_Repl_For_IST.ShowDialog();

            //}
            //else
            //{
            //    MessageBox.Show("Not allowed operation. Repl Workflow not done yet");
            //}

           // Form51 NForm51;
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            //
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54; // View only for replenishment already done  
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Ta.ReadSessionsStatusTraces(Mpa.TerminalId, Mpa.ReplCycleNo);
            if (Ta.RecordFound & Ta.ProcessMode > 0)
            {
                // CALL THE SAME If Recycle or not 
                    bool IsFromExcel = false;
                    Form51_Repl_For_IST NForm51_Repl_For_IST;
                    NForm51_Repl_For_IST = new Form51_Repl_For_IST(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo, IsFromExcel);
                     NForm51_Repl_For_IST.FormClosed += NForm5_FormClosed;
                     NForm51_Repl_For_IST.ShowDialog();
            }
            else
            {
                MessageBox.Show("No Replenishement done for this ATM and this Repl. Cycle");
                return;
            }
        }
// Create TXN SOLO
        private void buttonCreateTXNSolo_Click(object sender, EventArgs e)
        {
            if (Mpa.ActionType != "00" & Mpa.SettledRecord == false )
            {
                if (Mpa.ActionType == "10")
                {
                    // this is a postponed dispute 

                // leave it to continue
                }
                else
                {
                    // Transaction uder reconciliation or replenishment 
                    MessageBox.Show("Record is not settled yet. " + Environment.NewLine
                     + "It might be under workflow process. " + Environment.NewLine
                     + "You cannot open a dispute. " + Environment.NewLine
                        );
                    return;
                }
                
            }
            int WDispNo = 0;
            int DispTranNo = 0;
            RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

            bool TempPostponed = false;
            int TempDisputeId = 0;
            Dt.ReadDisputeTranByUniqueRecordId(WWUniqueRecordId);

            if (Dt.DisputeActionId == 5)
            {
                TempPostponed = true;
                TempDisputeId = Dt.DisputeNumber;
            }
            

            WDispNo = Di.Create_Pseudo_Dispute(WOperator, WSignedId, WWUniqueRecordId, 111);

            if (TempPostponed == true)
            {
                MessageBox.Show("This was a postponed Dispute with Id..= " + TempDisputeId.ToString()+Environment.NewLine
                    + "A New one will be created with id .. = " + WDispNo.ToString()
                    );
            }

            //Di.ReadDispute(WDispNo); 

            Dt.Create_Pseudo_Dispute_TXN(WOperator, WSignedId, WDispNo, WWUniqueRecordId, 111);

            Dt.ReadDisputeTranByUniqueRecordId(WWUniqueRecordId);
            DispTranNo = Dt.DispTranNo;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 2; // Return to stage 2  
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            DateTime LimitDate = new DateTime(2025, 06, 01);

        
            if (DateTime.Today> LimitDate)
            {
                MessageBox.Show("Please inform RRDM that SOLO is suspended");
                return; 
            }

            int WOrigin = 12; // From Requestor = Dispute Management 
            Form109 NForm109; 
            
            NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator, WDispNo, DispTranNo, WWUniqueRecordId, WOrigin);
            NForm109.FormClosed += NForm5_FormClosed;
            NForm109.ShowDialog();

        }
// EXPORT TO EXCEL 
        private void buttonExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString();
            string DD = DateTime.Now.Day.ToString();
            string Min = DateTime.Now.Minute.ToString();

            //MessageBox.Show("Excel will be created in RRDM Working Directory");
            string Id = "_Dispute-Pre-Investigation_";

            string ExcelPath = "C:\\RRDM\\Working\\" + Id + DD + Min + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Mpa.MatchingMasterDataTableATMs, WorkingDir, ExcelPath);
        }

        private void buttonBalance_Click(object sender, EventArgs e)
        {
            
        }
// Show The TXN BALANCE
        private void buttonBT_TXN_BAL_Click(object sender, EventArgs e)
        {
           
        }
// 
        private void buttonBT_TXN_BAL_V2_Click(object sender, EventArgs e)
        {
            // string WAtmNo = "EJ017002";
           
        }
// SHOW SM LINES 
        private void linkLabelSMLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Mpa.ReplCycleNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            string SM_SelectionCriteria1 = " WHERE atmno ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" +Mpa.ReplCycleNo
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

            SM.Read_SM_Record_Specific_By_Selection_ROM(SM_SelectionCriteria1, WAtmNo, Mpa.ReplCycleNo, 2);

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
/// <summary>
// Cassettes contents 

        private void linkLabel_SM_Cassettes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form51_SM_Cassettes NForm51_SM_Cassettes;
            NForm51_SM_Cassettes = new Form51_SM_Cassettes(WOperator, WSignedId, WAtmNo, Mpa.ReplCycleNo);
            NForm51_SM_Cassettes.Show();
        }
// TXN BALANCING 
        private void linkLabel_TXN_Bal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // For a particular transaction it shows its balancing
            // 
            int TempSeqNo = Mpa.OriginalRecordId; // original record id

            if (Mpa.MatchingCateg == "EMR204")
            {
                MessageBox.Show("Please select category EMR203");
                return;
            }

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 
                                                                                  // Read Traces to get values to display 
            Ta.ReadSessionsStatusTraces(WAtmNo, Mpa.ReplCycleNo);

            if (Ta.RecordFound == true)
            {
                // UPDATE CUMULATIVE TOTALS BASED ON THE SUPERVISOR MODE
                RRDM_Journal_TransactionSummary_V2 Ts = new RRDM_Journal_TransactionSummary_V2();
                Ts.ReadTXN_BALANCINGAndCorrect(WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd);
            }
            else
            {
                MessageBox.Show("No able to find replenishment records");
            }
            int Mode = 25;
            DateTime LineDate = Mpa.TransDate;

            //Form51_TXN_Cassettes_BAL Cb = new Form51_TXN_Cassettes_BAL();
            Form51_TXN_Cassettes_BAL_V2 NForm51_TXN_Cassettes_BAL_V2;
            NForm51_TXN_Cassettes_BAL_V2 = new Form51_TXN_Cassettes_BAL_V2(WOperator, WSignedId, WAtmNo,Mpa.TransAmount, TempSeqNo, Ta.SesDtTimeStart, LineDate,Mode );
            NForm51_TXN_Cassettes_BAL_V2.ShowDialog();

            
        }
// Show the balancing 
        private void linkLabelALL_TXN_Bal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Presents all Balancing records of 
            // FROM [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary_NEW]
            // FROM Start Replenishment till a certain date
            if (Mpa.MatchingCateg == "EMR204")
            {
                MessageBox.Show("Please select category EMR203");
                return;
            }
            if (Mpa.ReplCycleNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 
                                                                                  // Read Traces to get values to display 
            Ta.ReadSessionsStatusTraces(WAtmNo, Mpa.ReplCycleNo);

            if (Ta.RecordFound== true)
            {
                // OK
                RRDM_Journal_TransactionSummary_V2 Ts = new RRDM_Journal_TransactionSummary_V2();
                Ts.ReadTXN_BALANCINGAndCorrect(WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd);
            }
            else
            {
                MessageBox.Show("No able to find replenishment records");
            }
            int Mode = 20; 
            DateTime LineDate = Mpa.TransDate; 

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, "", WAtmNo
                                                   , Ta.SesDtTimeStart, LineDate, Mpa.ReplCycleNo, NullPastDate, Mode);
            NForm78D_ATMRecords.Show();
        }
// Unmatched 
        private void linkLabelUnmatched_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Mpa.ReplCycleNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 
                                                                                  // Read Traces to get values to display 
            Ta.ReadSessionsStatusTraces(WAtmNo, Mpa.ReplCycleNo);

            if (Ta.RecordFound == true)
            {
                // OK
            }
            else
            {
                MessageBox.Show("No able to find replenishment records");
            }
            int Mode = 20;
            DateTime LineDate = Mpa.TransDate;

            Form80b2_Unmatched_ROM NForm80b2;
            string WFunction = "View";

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched_ROM(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                                                WAtmNo, Ta.SesDtTimeStart, LineDate, 2
                );
            NForm80b2.Show();

        }
// Errors Origin 
        private void linkLabelErrorsOrigin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Mpa.MatchingCateg == "EMR204")
            {
                MessageBox.Show("Please select category EMR203");
                return;
            }
            if (Mpa.ReplCycleNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 
                                                                                  // Read Traces to get values to display 
            Ta.ReadSessionsStatusTraces(WAtmNo, Mpa.ReplCycleNo);

            if (Ta.RecordFound == true)
            {
                // OK
            }
            else
            {
                MessageBox.Show("No able to find replenishment records");
            }
            int Mode = 21;
            DateTime LineDate = Mpa.TransDate;

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, "", WAtmNo
                                                   , Ta.SesDtTimeStart, Ta.SesDtTimeEnd, Mpa.ReplCycleNo, NullPastDate, Mode);
            NForm78D_ATMRecords.Show();
        }
    }
}



using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
using System.Linq;

using System.Text;
//using System.Data.SqlClient;

//multilingual


namespace RRDM4ATMsWin
{
    public partial class Form5 : Form
    {

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDM_JccTransClass Jt = new RRDM_JccTransClass();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        string WD11 = "WD11";
        string WD12 = "WD12";
        string WD13 = "WD13";
        string WD14 = "Y";
        string WD15 = "N";
        string WD16 = "WD16";
        string WD17 = "WD17";
        string WD21 = "WD21";
        string WD22 = "WD22";
        string WD23 = "WD23";
        string WD24 = "WD24";
        string WD25 = "";
        string WD26 = "";
        string WD27 = "";
        string WD28 = "";
        string WD29 = "";
        string WD30 = "";
        string WD31 = "";
        string WD32 = "";
        string WD33 = "";
        string WD34 = "";
        string WD35 = "";
        string WD36 = "";

        string WD37 = "WD37";
        string WD38 = "WD38";

        string WD40 = "WD40";
        string WD41 = "WD41";
        // RRNumber or traces 
        string WD45 = "";
        string WD46 = "";
        string WD47 = "";
        string WD48 = "";

        string WD50 = "";

        string WD39 = "";

        int TranCount = 0;

        public DataTable CardAtmsTran = new DataTable();

        //string SQLString;

        string WFilter;

        //string connectionString = ConfigurationManager.ConnectionStrings
        //  ["ATMSConnectionString"].ConnectionString;

        bool AutoOwner = false;

        string UserToBeOwner = ""; 

        int WDispNo;
        bool WSelect;

        decimal DispAmnt;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WCardNo;

        string WCardNoBin;

        int WReconcCycleNo; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        bool CreateDisputeForCaller = false;

        DateTime WFromDate;
        DateTime WToDate;
        string WCardNoIn;
        int WUniqueRecordId;
        decimal WCounted;
        int WInDispNo;
        string WComment;
        int WFrom;
        string WOrigin; // Gets Values "ATM", "JCC" to denote 

        public Form5(string InSignedId, int InSignRecordNo, string InOperator, string InCardNo,
                                           int InTranNo, decimal InCounted, int InDispNo, string InComment, int InFrom, string InOrigin)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCardNoIn = InCardNo;
            WUniqueRecordId = InTranNo;
            WCounted = InCounted;
            WInDispNo = InDispNo; // > 0 if InFrom = 4 
            WComment = InComment;
            WFrom = InFrom;

            // 1 = call is coming from Main Form, 
            // 2 = Call is coming from Pre-Investigation (Unique)
            // 22 = Call is coming from Pre-Investigation (Unique) and Create Dispute on calling User Name 
            // 3 = Call is coming from deposits in difference,  
            // 4 = call is coming for updating details of dispute,  
            // 5 = call is coming from Reconciliation Process for ATMs CAsh - Record found in pool - Replenishment
            //
            // 7 = call is coming from Reconciliation Process matching reconciliation - Reconciliation
            // - record found through mask, OR Is Used from Replenishment in case of the presenter error.

            WOrigin = InOrigin; // "ATM" OR "JCC" OR "RMCategory"; 

            InitializeComponent();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelToday.Text = DateTime.Now.ToShortDateString();

            labelUserId.Text = WSignedId;
            
            if (WFrom == 22)
            {
                WFrom = 2; // turn it to 2
                CreateDisputeForCaller = true;  
                if (Environment.MachineName == "RRDM-PANICOS")
                {
                    // It is OK you proceed 
                    buttonNext.Text = "NEXT";
                    buttonNext.Hide(); 
                }
                else
                {
                    MessageBox.Show("You cannot proceed to create dispute on your name.");
                    return; 
                }
            }
            else
            {
                buttonNext.Text = "PRINT"; 
            }

            // Find Current RM Cycle 
                    string WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            textBoxMsgBoard.Text = "Input information for dispute";

            Gp.ParamId = "252"; // Dispute methods - Internal Or external Customer   
            comboBoxVisitType.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxVisitType.DisplayMember = "DisplayValue";

            string ParId = "253"; // AUTO Assign Owner 
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TempAuto = Gp.OccuranceNm;

            if (TempAuto == "YES")
            {
                AutoOwner = true; 
            }
            else
            {
                AutoOwner = false;

                label15.Hide();
                label16.Hide();
                textBoxOwnerId.Hide();
                textBoxOwnerNM.Hide();
            }

            tbComments.Text = WComment;

            if (WFrom == 3 || (WFrom == 2 & WOrigin == "ATM") || WFrom == 5 || WFrom == 7) // From deposits or Pre-investigation or reconciliation 
            {

                btTransactions.Text = "Show Transaction";

                if (WFrom == 3)
                {
                    string ParamId = "252"; // Dispute method Deposit    
                    string OccNumber = "7";
                    Gp.ReadParametersSpecificParmAndOccurance(WOperator, ParamId, OccNumber, "", "");

                    comboBoxVisitType.Text = Gp.OccuranceNm;

                    // Deposit difference 
                    radioButtonDepositDiff.Checked = true;

                    tbComments.Text = InComment;

                }
                if (WFrom == 5)
                {
                    string ParamId = "252"; // Dispute method replenishment 
                    string OccNumber = "8";
                    Gp.ReadParametersSpecificParmAndOccurance(WOperator, ParamId, OccNumber, "", "");

                    comboBoxVisitType.Text = Gp.OccuranceNm;

                    btTransactions.Text = "Show Trans";
                }

                if (WFrom == 7)
                {
                    string ParamId = "252"; // Dispute method Reconciliation 
                    string OccNumber = "8";
                    Gp.ReadParametersSpecificParmAndOccurance(WOperator, ParamId, OccNumber, "", "");

                    comboBoxVisitType.Text = Gp.OccuranceNm;

                    btTransactions.Text = "Show Transactions";
                }

                tbCardNo.Text = WCardNoIn;


                // find record details 
                if (WUniqueRecordId > 0)
                {

                    string SelectionCriteria = " WHERE UniqueRecordId = " + WUniqueRecordId;
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);
                    if (Mpa.RecordFound == true)
                    {
                        //FoundInPoolATMs = true;
                        //RRDMUsersRecords Ur = new RRDMUsersRecords(); 
                        Us.ReadUsersRecord(WSignedId); 
                        if (Us.Branch =="001" || Us.Branch == "0001")
                        {
                            tbCardNo.Text = Mpa.Card_Encrypted;
                        }
                        else
                        {
                            string input = Mpa.CardNumber;
                            /*
                             *
                             * Change * to 1
                             *
                             * */
                            char[] array = input.ToCharArray();
                            for (int i = 0; i < array.Length; i++)
                            {
                                char let = array[i];
                              
                                 if (let == '*')
                                    array[i] = 'M';
                            }
                            tbCardNo.Text = new string(array);
                        }

                        tbAccNo.Text = Mpa.AccNumber;

                        dateTimePickerFrom.Value = Mpa.TransDate;
                        dateTimePickerTo.Value = Mpa.TransDate;

                    }
                    else
                    {
                        MessageBox.Show("Record Not Found");
                        return;
                    }
                }


                if (WFrom == 5 || WFrom == 7)
                {
                    radioButtonReconcDiff.Checked = true;
                }
            }

            if (WFrom == 4)
            {
                labelStep1.Text = "Update Dispute Basic Data";
                textBoxMsgBoard.Text = "Change information to be updated ";

                tbId.Text = WInDispNo.ToString();

                Di.ReadDispute(WInDispNo);

                comboBoxVisitType.Text = Di.VisitType;

                tbCustName.Text = Di.CustName;
                tbCardNo.Text = Di.CardNo;
                tbAccNo.Text = Di.AccNo;
                tbCustPhone.Text = Di.CustPhone;
                tbCustEmail.Text = Di.CustEmail;

                if (Di.IsCardLostStolen == true)
                {
                    radioButtonLost.Checked = true;
                }
                else radioButtonLost.Checked = false;

                if (Di.DispType == 1) radioButton1.Checked = true;
                if (Di.DispType == 2) radioButton2.Checked = true;
                if (Di.DispType == 3) radioButton3.Checked = true;
                if (Di.DispType == 4) radioButtonDepositDiff.Checked = true;
                if (Di.DispType == 5) radioButtonReconcDiff.Checked = true;
                if (Di.DispType == 6) radioButtonOther.Checked = true;

                textBox4.Text = Di.OtherDispTypeDescr;

                tbComments.Text = Di.DispComments;
                //
                // HIDE FIELDS FOR DATES RELALED TO TRANSACTIONS - Not allowed 
                //
                btTransactions.Hide();
                label12.Hide();
                label4.Hide();
                label7.Hide();
                dateTimePickerFrom.Hide();
                dateTimePickerTo.Hide();
            }

            // Hide empty transactions datagrid  
            label13.Hide();
            panel4.Hide();

            buttonNext.Hide();


            buttonAdd.Hide();

            if (WFrom == 4)
            {
                buttonAdd.Text = "Update";
                buttonAdd.Show();
            }
        }
        //
        // SHOW THE TRANSACTIONS
        //
        private void btTransactions_Click_1(object sender, EventArgs e)
        {
            if (radioButtonNotLost.Checked == false & radioButtonLost.Checked == false)
            {
                MessageBox.Show("Please select if card is Stolen");
                return;
            }
            if (tbCustName.Text == "")
            {
                MessageBox.Show("Please enter Customer Name!");
                return;
            }

            // Telephone Validation 
            string Telephone = tbCustPhone.Text;

            if (Telephone != "")
            {
                System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                 (@"^\s*\+?\s*([0-9][\s-]*){9,}$");
                /*
                                ^           # Start of the string
                               \s*       # Ignore leading whitespace
                               \+?       # An optional plus
                               \s*       # followed by an optional space or multiple spaces
                               (
                                [0-9]  # A digit
                                [\s-]* # followed by an optional space or dash or more than one of those
                               )
                               {9,}     # That appears nine or more times
                               $           # End of the string
                 */

                if (expr.IsMatch(Telephone))
                {
                    //   MessageBox.Show("valid telephone");
                }
                else
                {
                    MessageBox.Show("invalid telephone");
                    return;
                }
            }

            string email = tbCustEmail.Text;

            if (email != "")
            {
                System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                 (@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");

                if (expr.IsMatch(email))
                {
                    //   MessageBox.Show("valid email");
                }
                else
                {
                    MessageBox.Show("invalid email");
                    return;
                }
            }

            if (radioButton1.Checked == false & radioButton2.Checked == false & radioButton3.Checked == false &
                 radioButtonDepositDiff.Checked == false & radioButtonReconcDiff.Checked == false & radioButtonOther.Checked == false
                 & radioButtonNeverDone.Checked == false
                  & radioButtonNeverReceived.Checked == false
                   & radioButtonNotAsDescribed.Checked == false
                    & radioButtonDefective.Checked == false
                     & radioButtonCanceled.Checked == false
                 )
            {
                MessageBox.Show("Choose type of Dispute please");
                return;
            }

            // Show transactions 
           // tbCardNo.Text = "526402******0569"; 
            if (radioButtonDepositDiff.Checked == true) // Deposits ...  ... 
            {
                WFilter = " WHERE CardNumber ='" + tbCardNo.Text + "' AND Operator ='" + WOperator + "'"
                          + " AND (TransType = 23 OR TransType = 24 OR TransType = 25) ";
            }
            else // Withdrawls ... ... 
            {
                WFilter = " WHERE CardNumber ='" + tbCardNo.Text + "' AND Operator ='" + WOperator + "'"
                          + " AND TransType = 11 ";

                if (tbAccNo.Text != "")
                {
                    WFilter = " WHERE AccNumber ='" + tbAccNo.Text + "' AND Operator ='" + WOperator + "'"
                         + " AND TransType = 11 ";
                }
            }

            if (WUniqueRecordId > 0) // Coming from pre-investigation or deposits where Tran is specified 
            {
                WFilter = " WHERE Operator ='" + WOperator + "'" + " AND UniqueRecordId = " + WUniqueRecordId; // Only this transaction  

                btTransactions.Hide();
            }

            WFromDate = dateTimePickerFrom.Value;
            WToDate = dateTimePickerTo.Value;
            WCardNo = tbCardNo.Text;
            //TEST
            if (WCardNo.Length > 15)
            {
                WCardNoBin = WCardNo.Substring(0, 6) + "******" + WCardNo.Substring(12, 4);
            }

            label13.Text = "TRANSACTIONS FOR: " + WCardNo;

            if ((WFrom == 2 & WOrigin == "ATM") || WFrom == 3 || WFrom == 5 || WFrom == 7)
            {

                string WSortCriteria = "";

                //No Dates Are selected

                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

                int Mode = 3;

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, Mode, WFilter, WSortCriteria, FromDt, ToDt,2);

                if (Mpa.RecordFound == true)
                {
                    dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;
                    textBox5.Text = Mpa.TotalSelected.ToString();
                }

                if (Mpa.TotalSelected == 1)
                {
                    // Check if already within this cycle
                    Dt.ReadDisputeTranByATMAndReplCycle(Mpa.TerminalId, Mpa.ReplCycleNo);

                    if (Dt.RecordFound == true)
                    {
                        // Get the User Id 
                        Di.ReadDispute(Dt.DisputeNumber);
                        if (Di.RecordFound == true)
                        {
                            // Check if exist 
                            UserToBeOwner = Di.OwnerId;
                        }
                    }
                }
            }
            else
            {
                //With Selected dates 
                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, 3, WFilter, "", WFromDate, WToDate,2);

                if (Mpa.RecordFound == true)
                {
                    dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;
                    textBox5.Text = Mpa.TotalSelected.ToString();

                
                }
                else
                {

                }
            }

            // Show Grid

            ShowGrid();

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            textBox5.Text = Mpa.TotalSelected.ToString();

            label13.Show();
            panel4.Show();

            //buttonNext.Show();

            buttonAdd.Show();

            textBoxMsgBoard.Text = "Review transactions and mark the dispute ones.";
        }

        //
        // ADD / update Dispute 
        // 
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (radioButtonNotLost.Checked == false & radioButtonLost.Checked == false)
                {
                    MessageBox.Show("Please select if card is Stolen");
                    return;
                }

                if (radioButton1.Checked == false & radioButton2.Checked == false & radioButton3.Checked == false &
                  radioButtonDepositDiff.Checked == false 
                  & radioButtonReconcDiff.Checked == false
                  & radioButtonOther.Checked == false
                  & radioButtonNeverDone.Checked == false
                   & radioButtonNeverReceived.Checked == false
                    & radioButtonNotAsDescribed.Checked == false
                     & radioButtonDefective.Checked == false
                      & radioButtonCanceled.Checked == false
                  )
                {
                    MessageBox.Show("Choose type of Dispute please");
                    return;
                }
                // Validate Input For Invalid Characters 

                if (RRDMInputValidationRoutines.IsAlfaNumeric(tbCustName.Text))
                {
                    //   No Problem 
                }
                else
                {
                    MessageBox.Show("invalid characters in Customer Name");
                    return;
                }
                if (RRDMInputValidationRoutines.IsAlfaNumeric(tbCardNo.Text))
                {
                    //   No Problem 
                }
                else
                {
                    MessageBox.Show("invalid characters in Card Number");
                    return;
                }
                if (RRDMInputValidationRoutines.IsAlfaNumeric(tbAccNo.Text))
                {
                    //   No Problem 
                }
                else
                {
                    MessageBox.Show("invalid characters in Account Number");
                    return;
                }

                if (tbCustPhone.Text == "")
                {
                    MessageBox.Show("Please Enter Customer Phone. It is Mandatory");
                    return; 
                }
                
                if (RRDMInputValidationRoutines.IsAlfaNumeric(tbCustPhone.Text))
                {
                    //   No Problem 
                }
                else
                {
                    MessageBox.Show("invalid characters in CustPhone ");
                    return;
                }

                if (RRDMInputValidationRoutines.IsAlfaNumeric(textBox2.Text))
                {
                    //   No Problem 
                }
                else
                {
                    MessageBox.Show("invalid characters in RRN ");
                    return;
                }

                if (WFrom == 4) // Updating of already existing Dispute 
                {
                    Di.ReadDispute(WInDispNo);

                    Di.CardNo = tbCardNo.Text.Trim();
                    Di.AccNo = tbAccNo.Text;
                    Di.CustName = tbCustName.Text;

                    if (tbCustPhone.Text == "")
                    {
                        Di.CustPhone = "Not Available";
                    }
                    else
                    {
                        Di.CustPhone = tbCustPhone.Text.Trim();
                    }

                    if (tbCustEmail.Text == "")
                    {
                        Di.CustEmail = "Not Available";
                    }
                    else
                    {
                        Di.CustEmail = tbCustEmail.Text;
                    }

                    if (radioButton1.Checked == true) Di.DispType = 1;
                    if (radioButton2.Checked == true) Di.DispType = 2;
                    if (radioButton3.Checked == true) Di.DispType = 3;
                    if (radioButtonDepositDiff.Checked == true) Di.DispType = 4;
                    if (radioButtonReconcDiff.Checked == true) Di.DispType = 5;
                    if (radioButtonOther.Checked == true) Di.DispType = 6;
                    if (radioButtonNeverDone.Checked == true) Di.DispType = 7;
                    if (radioButtonNotAsDescribed.Checked == true) Di.DispType = 8;
                    if (radioButtonDefective.Checked == true) Di.DispType = 9;
                    if (radioButtonCanceled.Checked == true) Di.DispType = 10;

                    Di.OtherDispTypeDescr = textBox4.Text;
                    Di.DispComments = tbComments.Text;
                    Di.VisitType = comboBoxVisitType.Text;
                    if (radioButtonLost.Checked == true)
                    {
                        Di.IsCardLostStolen = true;
                    }
                    else Di.IsCardLostStolen = false;

                    string OwnerXx = Di.OwnerId;

                    Di.UpdateDisputeRecord(Di.DispId);

                    MessageBox.Show("Dispute with number : " + Di.DispId.ToString() + " has been updated");

                    WDispNo = Di.DispId;

                    return;
                }

                if (int.TryParse(tbId.Text, out WDispNo))
                {
                }
                else
                {
                    //  MessageBox.Show(textBox1.Text, "Please enter a valid number!");
                    //   return;
                }

                int TranSelected = 0;

                int K = 0;

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    WSelect = (bool)dataGridView1.Rows[K].Cells["Select"].Value;
                    if (WFrom != 5) WUniqueRecordId = (int)dataGridView1.Rows[K].Cells["RecordId"].Value;

                    if (WSelect == true)
                    {
                        TranSelected = TranSelected + 1;
                        // Check if already exist
                        // And was not cancelled
                        Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);
                       // Dt.RecordFound == true & Dt.DisputeActionId != 6 // NOT CANCELLED
                       // In CASE Of Cancel another dispute can be oppened 
                        if (Dt.RecordFound == true & Dt.DisputeNumber != WDispNo)
                        {
                            MessageBox.Show("RecordId: " + WUniqueRecordId.ToString() + " Already exist in Dispute Number: "
                                                                                                    + Dt.DisputeNumber.ToString());
                            WDispNo = Dt.DisputeNumber;
                            return;
                        }

                    }

                    K++; // Read Next entry of the table 
                }

                if (TranSelected == 0) // User didnt select a transaction 
                {
                    MessageBox.Show("Please select a transaction to be moved to dispute.");
                    return;
                }

                if (tbId.Text == string.Empty || tbId.Text=="0")  // Create new dispute 
                {
                    Di.BankId = WOperator;
                    Us.ReadUsersRecord(WSignedId); 
                    Di.RespBranch = Us.Branch; // Creator Branch 
                    Di.LastUpdateDtTm = DateTime.Now;
                    Di.DispFrom = WFrom;
                    Di.DispType = 0;

                    if (radioButton1.Checked == true) Di.DispType = 1;
                    if (radioButton2.Checked == true) Di.DispType = 2;
                    if (radioButton3.Checked == true) Di.DispType = 3;
                    if (radioButtonDepositDiff.Checked == true) Di.DispType = 4;
                    if (radioButtonReconcDiff.Checked == true) Di.DispType = 5;
                    if (radioButtonOther.Checked == true) Di.DispType = 6;
                    if (radioButtonNeverDone.Checked == true) Di.DispType = 7;
                    if (radioButtonNotAsDescribed.Checked == true) Di.DispType = 8;
                    if (radioButtonDefective.Checked == true) Di.DispType = 9;
                    if (radioButtonCanceled.Checked == true) Di.DispType = 10;

                    Di.OpenDate = DateTime.Now;
                    DateTime today = DateTime.Now;

                    Gp.ReadParametersSpecificId(WOperator, "605", "1", "", ""); // LIMIT to be solved date // Dispute target dates 
                    int QualityRange1 = (int)Gp.Amount;

                    Di.TargetDate = today.AddDays(QualityRange1);

                    Di.CloseDate = NullPastDate;
                    Di.CardNo = tbCardNo.Text.Trim();
                   
                    Di.AccNo = tbAccNo.Text;
                    Di.CustName = tbCustName.Text;

                    if (tbCustPhone.Text == "")
                    {
                        Di.CustPhone = "Not Available";
                    }
                    else
                    {
                        Di.CustPhone = tbCustPhone.Text.Trim();
                    }

                    if (tbCustEmail.Text == "")
                    {
                        Di.CustEmail = "Not Available";
                    }
                    else
                    {
                        Di.CustEmail = tbCustEmail.Text;
                    }

                    Di.OtherDispTypeDescr = textBox4.Text;
                    Di.DispComments = tbComments.Text;
                    Di.VisitType = comboBoxVisitType.Text;

                    if (radioButtonLost.Checked == true)
                    {
                        Di.IsCardLostStolen = true;
                    }
                    else Di.IsCardLostStolen = false;

                    Di.OpenByUserId = WSignedId;

                    if (AutoOwner == true & CreateDisputeForCaller == false)
                    {
                        RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();

                        Uar.ReadUsersVsApplicationsVsRolesByUser(UserToBeOwner);
                        if (Uar.RecordFound == true & Uar.SecLevel == "04")
                        {
                            // Then OK USER STILL EXISTS 
                        }
                        else
                        {
                            // User no more an authoriser
                            UserToBeOwner = ""; 
                        }
                      
                        string MinUser;
                        if (UserToBeOwner != "")
                        {
                            MinUser = UserToBeOwner;

                            Di.HasOwner = true;
                            Di.OwnerId = MinUser;

                            label15.Show();
                            label16.Show();
                            textBoxOwnerId.Show();
                            textBoxOwnerNM.Show();

                            Us.ReadUsersRecord(MinUser);

                            textBoxOwnerId.Text = Di.OwnerId;
                            textBoxOwnerNM.Text = Us.UserName;
                        }
                        else
                        {
                            MinUser = Uar.ReadUsersVsApplicationsVsRolesByApplication_For_Disputes_Min_User("ATMS/CARDS");
                            if (Uar.RecordFound == true)
                            {
                                Di.HasOwner = true;
                                Di.OwnerId = MinUser;

                                label15.Show();
                                label16.Show();
                                textBoxOwnerId.Show();
                                textBoxOwnerNM.Show();

                                Us.ReadUsersRecord(MinUser);

                                textBoxOwnerId.Text = Di.OwnerId;
                                textBoxOwnerNM.Text = Us.UserName;
                            }
                            else
                            {
                                Di.HasOwner = false;
                                Di.OwnerId = "";
                            }
                        }              
                    }
                    else
                    {
                        if (WFrom == 5 || WFrom == 7 || CreateDisputeForCaller == true)
                        {
                            Di.HasOwner = true;
                            Di.OwnerId = WSignedId;

                            Us.ReadUsersRecord(WSignedId);

                            textBoxOwnerId.Text = Di.OwnerId;
                            textBoxOwnerNM.Text = Us.UserName;
                        }
                        else
                        {
                            Di.HasOwner = false;
                            Di.OwnerId = "";
                        }
                    }

         
                    Di.Active = true;

                    Di.DisputeCreatorId = WSignedId; 

                    Di.Operator = WOperator;

                    Di.DispId = Di.InsertDisputeRecord();

                    Dt.DeleteTransOfthisDispute(Di.DispId);
                    // Call Method to insert chosen transactions 
                    InsertDisputeTrans(Di.DispId);

                    // Create the action and Update the Mpa

                    RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    if (Usi.StepLevel < 1)
                    {
                        Usi.StepLevel = 1;
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    }

                    tbId.Text = Di.DispId.ToString();

                    MessageBox.Show("New Dispute with number: " + tbId.Text + " has been created");
                    WDispNo = Di.DispId;
                    buttonNext.Show();
                }
                else
                {
                    // Update Dispute record 

                    Di.ReadDispute(WDispNo);

                    Di.OpenDate = DateTime.Now;

                    Di.CardNo = tbCardNo.Text;
                    Di.AccNo = tbAccNo.Text;
                    Di.CustName = tbCustName.Text;

                    if (tbCustPhone.Text == "")
                    {
                        Di.CustPhone = "Not Available";
                    }
                    else
                    {
                        Di.CustPhone = tbCustPhone.Text;
                    }

                    if (tbCustEmail.Text == "")
                    {
                        Di.CustEmail = "Not Available";
                    }
                    else
                    {
                        Di.CustEmail = tbCustEmail.Text;
                    }

                    if (radioButton1.Checked == true) Di.DispType = 1;
                    if (radioButton2.Checked == true) Di.DispType = 2;
                    if (radioButton3.Checked == true) Di.DispType = 3;
                    if (radioButtonDepositDiff.Checked == true) Di.DispType = 4;
                    if (radioButtonReconcDiff.Checked == true) Di.DispType = 5;
                    if (radioButtonOther.Checked == true) Di.DispType = 6;
                    if (radioButtonNeverDone.Checked == true) Di.DispType = 7;
                    if (radioButtonNotAsDescribed.Checked == true) Di.DispType = 8;
                    if (radioButtonDefective.Checked == true) Di.DispType = 9;
                    if (radioButtonCanceled.Checked == true) Di.DispType = 10;

                    Di.OtherDispTypeDescr = textBox4.Text;
                    Di.DispComments = tbComments.Text;
                    Di.VisitType = comboBoxVisitType.Text;

                    if (radioButtonLost.Checked == true)
                    {
                        Di.IsCardLostStolen = true;
                    }
                    else Di.IsCardLostStolen = false;

                    Di.UpdateDisputeRecord(Di.DispId);

                    Dt.DeleteTransOfthisDispute(Di.DispId);
                    // Call Method to insert chosen transactions 
                    InsertDisputeTrans(Di.DispId);

                    if (CreateDisputeForCaller ==true)
                    {
                        buttonNext.Text = "NEXT";

                        buttonNext.Show(); 
                    }

                    MessageBox.Show("Dispute with number : " + Di.DispId.ToString() + " has been updated");
                    WDispNo = Di.DispId;
                    buttonNext.Show(); 
                }
            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

        }

        // Insert Dispute ALL Transactions 
        private void InsertDisputeTrans(int InDispNo)
        {
            try
            {
                TranCount = 0;
                // INSERT TRANSACTIONS FOR OUR ATMS

                int K = 0;

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    WSelect = (bool)dataGridView1.Rows[K].Cells["Select"].Value;
                    if (WFrom != 5) WUniqueRecordId = (int)dataGridView1.Rows[K].Cells["RecordId"].Value;

                    DispAmnt = (decimal)dataGridView1.Rows[K].Cells["DisputedAmnt"].Value;

                    if (WSelect == true)
                    {
                        // Check if already exist
                        Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);
                        if (Dt.RecordFound == true)
                        {
                            MessageBox.Show("RecordId: " + WUniqueRecordId.ToString() + " Already exist in Dispute Number: "
                                + Dt.DisputeNumber.ToString());
                            WDispNo = Dt.DisputeNumber;
                            return;
                        }
                        else // INSERT TRANSACTION 
                        {
                            if (WFrom == 5) // Record to be found from InPool
                            {
                                Ec.ReadErrorsTableSpecificByUniqueRecordId(WUniqueRecordId);
                                if (Ec.RecordFound == true)
                                {
                                    Dt.ErrNo = Ec.ErrNo; // GET THE ERROR No from Error table 
                                }
                                else Dt.ErrNo = 0;
                            }
                            else // Not coming from Cash reconciliation 
                            {
                                Ec.ReadErrorsTableSpecificByUniqueRecordId(WUniqueRecordId);
                                if (Ec.RecordFound == true)
                                {
                                    Dt.ErrNo = Ec.ErrNo; // GET THE ERROR No from Error table 
                                }
                                else Dt.ErrNo = 0;
                            }

                            // Find Details of Masked REcord 

                            string SelectionCriteria = " WHERE UniqueRecordId = " + WUniqueRecordId;
                            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);

                            if (Mpa.RecordFound == true)
                            {
                                //FoundInMatchedUnMatched = true;
                                Dt.UniqueRecordId = Mpa.UniqueRecordId;

                                Dt.BankId = Mpa.Operator;
                                Dt.DbTranNo = Mpa.SeqNo;
                                Dt.TranDate = Mpa.TransDate;
                                Dt.AtmNo = Mpa.TerminalId;

                                Dt.CardNo = Mpa.CardNumber;
                                Dt.AccNo = Mpa.AccNumber;
                                Dt.CurrencyNm = Mpa.TransCurr;
                                Dt.TranAmount = Mpa.TransAmount;

                                Dt.TransType = Mpa.TransType;
                                Dt.TransDesc = Mpa.TransDescr;
                                Dt.ReplCycle = Mpa.ReplCycleNo; 
                            }
                            else
                            {
                                MessageBox.Show("Record Not Found");
                                return;
                            }


                            Dt.DisputeNumber = InDispNo;
                            Dt.DispDtTm = DateTime.Now;
                            Dt.Origin = WOrigin;

                            //// READ ATM TO FIND REPL GROUP
                            Ac.ReadAtm(Dt.AtmNo);
                            if (Ac.RecordFound)
                            {
                                Dt.ReplGroup = Ac.AtmsReplGroup;

                            }
                            else
                            {
                                Dt.ReplGroup = 0;
                            }

                            Dt.CardType = "Chip";

                            Dt.DisputedAmt = DispAmnt;
                            Dt.DisputeActionId = 0;
                            Dt.ActionDtTm = Di.TargetDate;
                            Dt.DecidedAmount = 0;
                            Dt.ReasonForAction = 0;
                            Dt.ActionComment = "";
                            Dt.PostDate = NullPastDate;
                            Dt.ClosedDispute = false;

                            TranCount = TranCount + 1;
                            //Set up printing variables 
                            if (TranCount == 1)
                            {
                                WD21 = Dt.TranDate.ToString();
                                if (Mpa.ACCEPTOR_ID != "")
                                {
                                    WD22 = Mpa.ACCEPTOR_ID+ " "+ Mpa.ACCEPTORNAME ;
                                }
                                else
                                {
                                    WD22 = Dt.AtmNo.ToString();
                                }
                                
                                WD23 = Dt.TranAmount.ToString();
                                WD24 = Dt.DisputedAmt.ToString();

                                if (Mpa.RRNumber != "0")
                                {
                                    WD45 = Mpa.RRNumber.ToString();
                                }
                                if (Mpa.TraceNoWithNoEndZero > 0)
                                {
                                    WD45 = Mpa.TraceNoWithNoEndZero.ToString();
                                }
                            }

                            if (TranCount == 2)
                            {
                                WD25 = Dt.TranDate.ToString();
                                if (Mpa.ACCEPTOR_ID != "")
                                {
                                    WD26 = Mpa.ACCEPTOR_ID + " " + Mpa.ACCEPTORNAME;
                                }
                                else
                                {
                                    WD26 = Dt.AtmNo.ToString();
                                }
                                WD27 = Dt.TranAmount.ToString();
                                WD28 = Dt.DisputedAmt.ToString();

                                if (Mpa.RRNumber != "0")
                                {
                                    WD46 = Mpa.RRNumber.ToString();
                                }
                                if (Mpa.TraceNoWithNoEndZero > 0)
                                {
                                    WD46 = Mpa.TraceNoWithNoEndZero.ToString();
                                }
                            }

                            if (TranCount == 3)
                            {
                                WD29 = Dt.TranDate.ToString();
                                if (Mpa.ACCEPTOR_ID != "")
                                {
                                    WD30 = Mpa.ACCEPTOR_ID + " " + Mpa.ACCEPTORNAME;
                                }
                                else
                                {
                                    WD30 = Dt.AtmNo.ToString();
                                }
                                WD31 = Dt.TranAmount.ToString();
                                WD32 = Dt.DisputedAmt.ToString();

                                if (Mpa.RRNumber != "0")
                                {
                                    
                                       WD47 = Mpa.RRNumber.ToString();
                                }
                                if (Mpa.TraceNoWithNoEndZero > 0)
                                {
                                    WD47 = Mpa.TraceNoWithNoEndZero.ToString();
                                }
                            }

                            if (TranCount == 4)
                            {
                                WD33 = Dt.TranDate.ToString();
                                if (Mpa.ACCEPTOR_ID != "")
                                {
                                    WD34 = Mpa.ACCEPTOR_ID + " " + Mpa.ACCEPTORNAME;
                                }
                                else
                                {
                                    WD34 = Dt.AtmNo.ToString();
                                }
                                WD35 = Dt.TranAmount.ToString();
                                WD36 = Dt.DisputedAmt.ToString();

                                if (Mpa.RRNumber != "0")
                                {
                                    WD48 = Mpa.RRNumber.ToString();
                                }
                                if (Mpa.TraceNoWithNoEndZero > 0)
                                {
                                    WD48 = Mpa.TraceNoWithNoEndZero.ToString();
                                }
                            }
                            Dt.Operator = WOperator;
                            Dt.InsertDisputeTran(InDispNo);
                            WDispNo = InDispNo;

                           // Create Action Occurance
                            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                            string WUniqueRecordIdOrigin = "Master_Pool";
                            string WMaker_ReasonOfAction = "Money Difference";

                            string WOriginWorkFlow = "";

                            int WAction = 1;

                            WOriginWorkFlow = "Dispute";

                            if (WFrom == 5)
                            {
                                WOriginWorkFlow = "Replenishment";
                            }

                            if (WFrom == 7)
                            {
                                WOriginWorkFlow = "Reconciliation";
                            }

                            string WActionId = "05";

                            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                                WActionId, WUniqueRecordIdOrigin,
                                                                WUniqueRecordId, Mpa.TransCurr, Mpa.TransAmount,
                                                                Mpa.TerminalId, Mpa.ReplCycleNo, WMaker_ReasonOfAction, WOriginWorkFlow);

                            // UPDATE ACTION STAGE TO '03'
                            string WStage = "02"; // Confirmed by maker 
                            Aoc.UpdateOccurancesStage("Master_Pool", Mpa.UniqueRecordId, WStage, DateTime.Now, WReconcCycleNo, WSignedId);
                            // Also Authoriser 
                            // Also Update Stage as "03"
                            Aoc.UpdateOccurancesForAuthoriser("Master_Pool", Mpa.UniqueRecordId, WSignedId, 999, WSignedId);

                            // UPDATE ACTIONTYPE Mpa

                            Mpa.ActionType = WActionId; 
                            Mpa.Comments = "Transaction moved to dispute with ID="+InDispNo.ToString();
                            Mpa.UpdateMpaRecordsWithActionType(Mpa.UniqueRecordId, Mpa.ActionType
                                          , Mpa.Comments, 2); 
                        }
                    }
                    K++; // Read Next entry of the table 
                }
            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

           
        }

        //AUDIT TRAIL : GET IMAGE AND INSERT IT IN AUDIT TRAIL 
        private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory, string InTypeOfChange, int InUser)
        {
            Bitmap SCREENa;
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(panel4.Width, panel4.Height);
            panel4.DrawToBitmap(memoryImage, panel4.ClientRectangle);
            SCREENa = memoryImage;

            AuditTrailClass At = new AuditTrailClass();
            //     At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENa);
        }

        // Print Dispute 

        private void button1_Click(object sender, EventArgs e)
        {
            if (CreateDisputeForCaller == false)
            {
                //Create Printing 
                if (WDispNo == 0)
                {
                    MessageBox.Show("No dispute created yet");
                    return;
                }
                Di.ReadDispute(WDispNo);
                WD11 = Di.CustName;
                WD12 = Di.AccNo;
                WD13 = Di.CardNo;
                //TEST
                if (Mpa.TXNSRC == "1")
                {
                    WD14 = "Y";
                    WD15 = "N";
                }
                else
                {
                    WD14 = "N";
                    WD15 = "Y";
                }

                if (Mpa.TerminalType == "20")
                {
                    WD50 = "Y";
                    WD14 = "N";
                    WD15 = "N";
                }
                else
                {
                    WD50 = "N";
                }

                WD16 = Di.CustPhone;
                WD17 = Di.CustEmail;

                string InBankIdLogo = WOperator;

                string TDispType = "251"; // Parameter Id for dispute types

                Gp.ReadParametersSpecificId(WOperator, TDispType.ToString(), Di.DispType.ToString(), "", "");

                WD37 = Gp.OccuranceNm;

                if (Di.DispType == 5)
                {
                    WD38 = Di.OtherDispTypeDescr;
                }
                else
                {
                    WD38 = tbComments.Text;
                }

                WD40 = Di.DispId.ToString();

                RRDMUsersRecords Us = new RRDMUsersRecords();
                Us.ReadUsersRecord(WSignedId);

                WD41 = Us.UserName;

                if (radioButtonNotLost.Checked == true)
                {
                    WD39 = "NOT LOST CARD";
                }
                else
                {
                    WD39 = "LOST/STOLEN CARD";
                }

                Form56R5 ReportDispute = new Form56R5(WD11, WD12, WD13, WD14, WD15, WD16, WD17,
                            WD21, WD22, WD23, WD24,
                            WD25, WD26, WD27, WD28,
                            WD29, WD30, WD31, WD32,
                            WD33, WD34, WD35, WD36,
                            WD37, WD38,
                            WD40, WD41, InBankIdLogo, WD39, WD45, WD46, WD47, WD48, WD50);

                ReportDispute.Show();
            }
            else
            {
                //CreateDisputeForCaller we go to dispute 
                // THIS WFrom = 22 
                // You go to Form109 
                Form4 NForm4; 
                NForm4 = new Form4(WSignedId, WSignRecordNo, WOperator, WOperator, WDispNo);
                //NForm4.FormClosed += NForm4_FormClosed;
                NForm4.ShowDialog();
            }       
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

        //**********************************************************************
        // START NOTES 
        //**********************************************************************
        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // Add ATTACHements for notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            if (Di.DispId != 0)
            {
                Form197 NForm197;
                string WParameter3 = "";
                string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
                string SearchP4 = "";
                string WMode = "Update";
                NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
                NForm197.FormClosed += NForm197_FormClosed;
                NForm197.ShowDialog();
            }
            else
            {
                MessageBox.Show("Open dispute and then insert attachements");
            }
        }
        // Set Number of notes 
        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Show Map Info for all tansactions by time 
        private void buttonShowMaps_Click(object sender, EventArgs e)
        {
            Form350_Route NForm350_Route;

            NForm350_Route = new Form350_Route("TRANSACTIONS MADE ON MAP BY TIME");

            NForm350_Route.ShowDialog();
        }
        // Check if request is coming from JCC Or Visa 
        private void comboBoxVisitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxVisitType.Text == "Through visa request" || comboBoxVisitType.Text == "Through JCC request")
            {
                label10.Show();
                textBox2.Show();
            }
            else
            {
                label10.Hide();
                textBox2.Hide();
            }
        }
        //**********************************************************************
        // END NOTES 
        //**********************************************************************  

        // Show Grid 

        public void ShowGrid()
        {

            // Check If Data 
            if (dataGridView1.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("No transactions for this selection");
                MessageForm.ShowDialog();

                panel4.Hide();
                label13.Hide();

                return;
            }


            //MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));
            //MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            //    MatchingMasterDataTableATMs.Columns.Add("Select", typeof(bool));
            //    MatchingMasterDataTableATMs.Columns.Add("DisputedAmnt", typeof(decimal));
            //    MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));

            dataGridView1.Columns[0].Width = 50; // Select 
            //dataGridView1.Columns[0].Name = "Select";
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 60; // Disputed Amnt
            //dataGridView1.Columns[1].Name = "Disputed Amnt";
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[2].Width = 60; // Record Id
            //dataGridView1.Columns[2].Name = "RecordId";
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 40; // Action
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 70; // Terminal
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 50; // Terminal Type, ATM, POS etc 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 90; // Descr
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 40; // Err
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 40; // Mask
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 90; // Account
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[10].Width = 50; // Ccy
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 80; // Amount
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[12].Width = 120; // Date
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            int tempUniqueNo = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                //bool WSelect = (bool)row.Cells[1].Value;

                tempUniqueNo = (int)row.Cells[2].Value;

                Dt.ReadDisputeTranByUniqueRecordId(tempUniqueNo);
                if (Dt.RecordFound == true)
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
        }
        // Catch Details
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

            // Environment.Exit(0);
        }
    }
}


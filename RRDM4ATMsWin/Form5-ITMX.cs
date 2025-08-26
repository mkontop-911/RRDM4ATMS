using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
//multilingual


namespace RRDM4ATMsWin
{
    public partial class Form5ITMX : Form
    {
        //   Form11 NForm11;
        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();

        //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 

        //RRDMMatchingReconcExceptionsInfo Mre = new RRDMMatchingReconcExceptionsInfo();

        //RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass();

        RRDMDisputesTableClassITMX Di = new RRDMDisputesTableClassITMX();
        RRDMDisputeTransactionsClassITMX Dt = new RRDMDisputeTransactionsClassITMX();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDM_JccTransClass Jt = new RRDM_JccTransClass();

        RRDMBanks Ba = new RRDMBanks();

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

        int TranCount = 0;

        int WDispNo;
        bool WSelect;
       
        decimal DispAmnt;
      

        string UserBankId;

        int WTxnCode;
        int WDisputeActionId;
        //string UnMatchedName;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSelectionCriteria3;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        //DateTime WFromDate;
        //DateTime WToDate;
        string WCardNoIn;
        int WMaskRecordId;
        decimal WCounted;
        int WInDispNo;
        string WComment;
        int WFrom;
        string WOrigin; // Gets Values "ATM", "JCC" to denote 

        public Form5ITMX(string InSignedId, int InSignRecordNo, string InOperator, string InCardNo,
                                           int InMaskRecordId, decimal InCounted, int InDispNo, string InComment, int InFrom, string InOrigin)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCardNoIn = InCardNo;
            WMaskRecordId = InMaskRecordId;
            WCounted = InCounted;
            WInDispNo = InDispNo; // > 0 if InFrom = 4 
            WComment = InComment;
            WFrom = InFrom;
            // 1 = call is coming from Main Form, 
            // 2 = Call is coming from Pre-Investigation 
            // 3 = Call is coming from deposits in difference,  
            // 4= call is coming for updating details of dispute,  
            // 5= call is coming from Reconciliation Process for ATMs Cash - Record found in pool 
            //
            // 7= call is coming from Reconciliation Process matching reconciliation 
            // It can be From ITMX 

            WOrigin = InOrigin; // "ATM" OR "JCC" OR "RMCategory" OR "ITMX" OR Bank 

            InitializeComponent();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelToday.Text = DateTime.Now.ToShortDateString();
            labelUserId.Text = WSignedId;
            
            textBoxMsgBoard.Text = "Input information for dispute";

            Us.ReadUsersRecord(InSignedId);

            UserBankId = Us.BankId;

            if (UserBankId != WOperator)
            {
                // User belongs to the Bank Not to ITMX
                WSelectionCriteria3 = " AND ( DebitBank ='" + UserBankId + "' OR CreditBank ='" + UserBankId + "' )";
            }
            else
            {
                WSelectionCriteria3 = "";
            }

            Gp.ParamId = "252"; // Dispute methods - Internal Or external Customer   
            comboBoxVisitType.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxVisitType.DisplayMember = "DisplayValue";

            tbComments.Text = WComment;
            //TEST
            //
            if (WFrom ==1)
            {
                tbCustomerUniqueId.Text = "99622248"; 
            }

            if (WFrom == 2 & WOrigin == "ITMX")
            {
                //From pre-Investigation for ITMX 
                buttonTransactions.Text = "Show Transaction";
            }

            if (WFrom == 3 || (WFrom == 2 & WOrigin == "ATM") || WFrom == 5 || WFrom == 7)
            {
                // From deposits or Pre-investigation of ATMs or reconciliation 
                // This is not for ITMX 
                buttonTransactions.Text = "Show Transaction";

                if (WFrom == 3)
                {
                    string ParamId = "252"; // Dispute method Deposit    
                    string OccNumber = "7";
                    Gp.ReadParametersSpecificParmAndOccurance(WOperator, ParamId, OccNumber, "", "");

                    comboBoxVisitType.Text = Gp.OccuranceNm;
                }
                if (WFrom == 5)
                {
                    string ParamId = "252"; // Dispute method Reconciliation 
                    string OccNumber = "8";
                    Gp.ReadParametersSpecificParmAndOccurance(WOperator, ParamId, OccNumber, "", "");

                    comboBoxVisitType.Text = Gp.OccuranceNm;

                    buttonTransactions.Text = "Show Trans";
                }

                if (WFrom == 7)
                {
                    //This has to do with EWBank . In the future this will change.
                    //Transactions will be found in Matching Pool 
                    string ParamId = "252"; // Dispute method Reconciliation 
                    string OccNumber = "10";
                    Gp.ReadParametersSpecificParmAndOccurance(WOperator, ParamId, OccNumber, "", "");

                    comboBoxVisitType.Text = Gp.OccuranceNm;

                    buttonTransactions.Text = "Show Trans";
                }

                // find record details in InPool or matched unmatched tables

                if (WMaskRecordId > 0)
                {
                    if (WFrom == 5) // Record to be found from InPool
                    {
                        //TEST
                        Mp.ReadMatchingTxnsMasterPoolByMaskRecordId(WMaskRecordId);
                        if (Mp.RecordFound == true)
                        {
                            //FoundInPoolATMs = true;

                            //tbCustomerUniqueId.Text = Mp.CardNo;
                            //tbAccNo.Text = Tc.AccNo;


                            //dateTimePickerFrom.Value = Tc.AtmDtTime;
                            //dateTimePickerTo.Value = Tc.AtmDtTime;

                        }
                        else
                        {
                            MessageBox.Show("Record Not Found");
                            return;
                        }
                    }
                    if (WFrom == 7 & WOperator != "ITMX") // Transaction is not ITMX 
                    {
                        // Find Details of Masked REcord 

                        string WFilterFinal = " WHERE MaskRecordId =" + WMaskRecordId;
                        Mp.ReadMatchingTxnsMasterPoolBySelectionCriteria(WFilterFinal);
                    
                        if (Mp.RecordFound == true)
                        {
                            //FoundInMatchedUnMatched = true;
                            //FoundInMatched = true;
                            //dateTimePickerFrom.Value = Mpa.TransDate;
                            //dateTimePickerTo.Value = Mpa.TransDate;
                            //tbAccNo.Text = Mpa.AccNumber;
                        }
                        else
                        {
                            MessageBox.Show("Record Not Found");
                            return;     
                        }
                    }

                }


                tbCustomerUniqueId.Text = WCardNoIn;


                if (WFrom == 3)
                {
                    radioButtonDepositDiff.Checked = true;

                    MessageBox.Show("Under Construction");

                    //Tc.DepCount = WCounted;

                    //Tc.AtmMsg = "Transaction will be moved to dispute";

                    //Tc.UpdateTransforDep(WMaskRecordId);
                }
                if (WFrom == 5 || WFrom == 7)
                {
                    radioButtonReconcDiff.Checked = true;
                }
            }
            //
            //UPDATING DISPUTE DATA 
            //
            if (WFrom == 4)
            {
                labelStep1.Text = "Update Dispute Basic Data";
                textBoxMsgBoard.Text = "Amend information that need updating. ";

                tbId.Text = WInDispNo.ToString();

                Di.ReadDispute(WInDispNo);

                comboBoxVisitType.Text = Di.VisitType;

                tbCustName.Text = Di.CustName;
                tbCustomerUniqueId.Text = Di.CardNo;
                tbAccNo.Text = Di.AccNo;
                tbCustPhone.Text = Di.CustPhone;
                tbCustEmail.Text = Di.CustEmail;

                if (Di.DispType == 1) radioButton1.Checked = true;
                if (Di.DispType == 2) radioButton2.Checked = true;
                if (Di.DispType == 3) radioButton3.Checked = true;
                if (Di.DispType == 4) radioButtonDepositDiff.Checked = true;
                if (Di.DispType == 5) radioButtonReconcDiff.Checked = true;
                if (Di.DispType == 6) radioButtonOther.Checked = true;

                textBox4.Text = Di.OtherDispTypeDescr;

                tbComments.Text = Di.DispComments;
                //
                // HIDE FIELDS FOR DATES REALED TO TRANSACTIONS - Not allowed 
                //
                buttonTransactions.Hide();
                label29.Hide();
                label4.Hide();
                label30.Hide();
                dateTimePickerFrom.Hide();
                dateTimePickerTo.Hide();
            }

            // Hide empty transactions datagrid  
            label13.Hide();
            panel4.Hide();

            button1.Hide();


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

            if (tbCustName.Text == "")
            {
                MessageBox.Show("Please enter Customer Name!");
                return;
            }

            
            if (tbCustomerUniqueId.Text == "")
            {
                MessageBox.Show("Please enter Customer Unique Id!");
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
                 radioButtonDepositDiff.Checked == false & radioButtonReconcDiff.Checked == false & radioButtonOther.Checked == false)
            {
                MessageBox.Show("Choose type of Dispute please");
                return;
            }

            // Show transactions 

            if (WFrom == 1 & WOperator == "ITMX")
            // Coming from main Form - entered by user  
            {

                string WSelectionCriteria1 = " WHERE Operator ='" + WOperator + "'"
                                         + " AND (MobileRequestor = '" + tbCustomerUniqueId.Text +"'"
                                         + " OR MobileBeneficiary = '" + tbCustomerUniqueId.Text + "' )";

                string WSelectionCriteria2 = "";
                string WSortCriteria = "";

                //No Dates Are selected
                DateTime FromDt = dateTimePickerFrom.Value;
                DateTime ToDt = dateTimePickerTo.Value;
               
                int Mode = 3; // Show that this comes from Dispute registration 
                Mp.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WSignedId,Mode, WSelectionCriteria1, WSelectionCriteria2, 
                                                                 WSelectionCriteria3, WSortCriteria, FromDt, ToDt);
                dataGridView1.DataSource = Mp.MatchingMasterDataTableITMX.DefaultView;

                buttonTransactions.Hide();

            }
            if ((WFrom == 7 || (WFrom == 2 & WOrigin == "ITMX")) & WMaskRecordId > 0)
                // Coming from pre-investigation or deposits where Tran is specified 
              
            {

                //WSelectionCriteria3 = " AND ( DebitBank ='" + UserBankId + "' OR CreditBank ='" + UserBankId + "' )";


                string WSelectionCriteria1 = " WHERE Operator ='" + WOperator + "' AND MaskRecordId = " + WMaskRecordId; // Only this transaction 

                string WSelectionCriteria2 = "";
                string WSortCriteria = "";

                //No Dates Are selected
                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;


                int Mode = 3; // Show that this comes from Dispute registration 
                Mp.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WSignedId, Mode, WSelectionCriteria1, WSelectionCriteria2, WSelectionCriteria3, WSortCriteria, FromDt, ToDt);
                dataGridView1.DataSource = Mp.MatchingMasterDataTableITMX.DefaultView;

                buttonTransactions.Hide();

            }

            // Check If Data 
            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No transactions for this selection");
                MessageForm.ShowDialog();

                panel4.Hide();
                label13.Hide();

                return;
            }

            dataGridView1.Columns[0].Name = "Select";
            dataGridView1.Columns[1].Name = "DisputedAmnt";
            dataGridView1.Columns[2].Name = "RecordId";
            dataGridView1.Columns[3].Name = "Ccy";
            dataGridView1.Columns[4].Name = "Amount";
            dataGridView1.Columns[5].Name = "DebitMASK";
            dataGridView1.Columns[6].Name = "CreditMASK";
            dataGridView1.Columns[7].Name = "ExecutionTxnDtTm";
          
            // SIZE
            dataGridView1.Columns["Select"].Width = 50; //
            dataGridView1.Columns["Select"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["DisputedAmnt"].Width = 60;
            dataGridView1.Columns["DisputedAmnt"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["DisputedAmnt"].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns["RecordId"].Width = 60;
            dataGridView1.Columns["RecordId"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Ccy"].Width = 40;
            dataGridView1.Columns["Ccy"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Amount"].Width = 60;
            dataGridView1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["DebitMASK"].Width = 40;
            dataGridView1.Columns["DebitMASK"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["CreditMASK"].Width = 40;
            dataGridView1.Columns["CreditMASK"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["ExecutionTxnDtTm"].Width = 100;

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

            textBox5.Text = Mp.TotalSelectedITMX.ToString();

            label13.Show();
            panel4.Show();

            button1.Show();

            buttonAdd.Show();

            textBoxMsgBoard.Text = "Review transaction/s and mark the disputed ones.";
        }

        //
        // ADD / update Dispute 
        // 
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false & radioButton2.Checked == false &
              radioButton3.Checked == false & radioButtonDepositDiff.Checked == false & radioButtonReconcDiff.Checked == false & radioButtonOther.Checked == false)
            {
                MessageBox.Show("Please Choose Dispute type");
                return;
            }

            if (WFrom == 4) // Updating of already existing Dispute 
            {
                Di.ReadDispute(WInDispNo);

                Di.CardNo = tbCustomerUniqueId.Text;
                Di.AccNo = tbAccNo.Text;
                Di.CustName = tbCustName.Text;

                if (tbCustPhone.Text == "")
                {
                    Di.CustPhone = "Not Availble";
                }
                else
                {
                    Di.CustPhone = tbCustPhone.Text;
                }

                if (tbCustEmail.Text == "")
                {
                    Di.CustEmail = "Not Availble";
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
                Di.OtherDispTypeDescr = textBox4.Text;
                Di.DispComments = tbComments.Text;
                Di.VisitType = comboBoxVisitType.Text;

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
            //
            //VALIDATION 
            //
            while (K <= (dataGridView1.Rows.Count - 2))
            {
                WSelect = (bool)dataGridView1.Rows[K].Cells["Select"].Value;
                WMaskRecordId = (int)dataGridView1.Rows[K].Cells["RecordId"].Value;

                if (WSelect == true)
                {
                    TranSelected = TranSelected + 1;
                    // Check if already exist
                    Dt.ReadDisputeTranByMaskRecordId(WMaskRecordId);
                    if (Dt.RecordFound == true & Dt.DisputeNumber != WDispNo)
                    {
                        MessageBox.Show("MaskRecordId: " + WMaskRecordId.ToString() + " Already exist in Dispute Number: "
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

            if (tbId.Text == String.Empty)  // Create new dispute 
            {
                Di.BankId = UserBankId;
                Di.RespBranch = Us.Branch;
                Di.LastUpdateDtTm = DateTime.Now;
                Di.DispFrom = WFrom;

                Di.CreatedByEntity = UserBankId; 

                Di.DispType = 0;

                if (radioButton1.Checked == true) Di.DispType = 1;
                if (radioButton2.Checked == true) Di.DispType = 2;
                if (radioButton3.Checked == true) Di.DispType = 3;
                if (radioButtonDepositDiff.Checked == true) Di.DispType = 4;
                if (radioButtonReconcDiff.Checked == true) Di.DispType = 5;
                if (radioButtonOther.Checked == true) Di.DispType = 6;

                Di.OpenDate = DateTime.Now;
                DateTime today = DateTime.Now;

                Gp.ReadParametersSpecificId(WOperator, "605", "1", "", ""); // LIMIT to be solved date // Dispute target dates 
                int QualityRange1 = (int)Gp.Amount;

                Di.TargetDate = today.AddDays(QualityRange1);

                Di.CloseDate = NullPastDate;
                Di.CardNo = tbCustomerUniqueId.Text;
                Di.AccNo = tbAccNo.Text;
                Di.CustName = tbCustName.Text;

                if (tbCustPhone.Text == "")
                {
                    Di.CustPhone = "Not Availble";
                }
                else
                {
                    Di.CustPhone = tbCustPhone.Text;
                }

                if (tbCustEmail.Text == "")
                {
                    Di.CustEmail = "Not Availble";
                }
                else
                {
                    Di.CustEmail = tbCustEmail.Text;
                }

                Di.OtherDispTypeDescr = textBox4.Text;
                Di.DispComments = tbComments.Text;
                Di.VisitType = comboBoxVisitType.Text;

                Di.OpenByUserId = WSignedId;

                Di.Active = true;

                Di.Operator = WOperator;
                //INSERT DISPUTE 
                Di.DispId = Di.InsertDisputeRecord();

                Dt.DeleteTransOfthisDispute(Di.DispId);
                //******************************************
                // Call Method to insert chosen transactions 
                //******************************************

                InsertDisputeTrans(Di.DispId);

                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);

                if (Usi.StepLevel < 1)
                {
                    Usi.StepLevel = 1;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                tbId.Text = Di.DispId.ToString();
            }

            else
            {
                // Update Dispute record 

                Di.OpenDate = DateTime.Now;

                Di.CardNo = tbCustomerUniqueId.Text;
                Di.AccNo = tbAccNo.Text;
                Di.CustName = tbCustName.Text;

                if (tbCustPhone.Text == "")
                {
                    Di.CustPhone = "Not Availble";
                }
                else
                {
                    Di.CustPhone = tbCustPhone.Text;
                }

                if (tbCustEmail.Text == "")
                {
                    Di.CustEmail = "Not Availble";
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
                Di.OtherDispTypeDescr = textBox4.Text;
                Di.DispComments = tbComments.Text;
                Di.VisitType = comboBoxVisitType.Text;

                Di.UpdateDisputeRecord(Di.DispId);

                Dt.DeleteTransOfthisDispute(Di.DispId);
                // Call Method to insert chosen transactions 
                InsertDisputeTrans(Di.DispId);

                MessageBox.Show("Dispute with number : " + Di.DispId.ToString() + " has been updated");
                WDispNo = Di.DispId;
            }

        
        }


        // Insert Dispute ALL Transactions 
        private void InsertDisputeTrans(int InDispNo)
        {
            TranCount = 0;
            // INSERT TRANSACTIONS FOR OUR ATMS

            int K = 0;

            while (K <= (dataGridView1.Rows.Count - 2))
            {
                WSelect = (bool)dataGridView1.Rows[K].Cells["Select"].Value;
               
                if (WSelect == true)
                {
                    WMaskRecordId = (int)dataGridView1.Rows[K].Cells["RecordId"].Value;

                    DispAmnt = (decimal)dataGridView1.Rows[K].Cells["DisputedAmnt"].Value;

                    Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WOperator, WMaskRecordId);
                    // Check if already exist
                    Dt.ReadDisputeTranByMaskRecordId(WMaskRecordId);
                    if (Dt.RecordFound == true)
                    {
                        MessageBox.Show("RecordId: " + WMaskRecordId.ToString() + " Already exist in Dispute Number: "
                            + Dt.DisputeNumber.ToString());
                        WDispNo = Dt.DisputeNumber;
                        return;
                    }
                    else // INSERT TRANSACTION 
                    {
                        
                            Ec.ReadErrorsTableSpecificByUniqueRecordId(WMaskRecordId);
                            if (Ec.RecordFound == true)
                            {
                                Dt.ErrNo = Ec.ErrNo; // GET THE ERROR No from Error table 
                            }
                            else Dt.ErrNo = 0;


                        // Find Details of Masked REcord 

                        //TEST
                        Mp.ReadMatchingTxnsMasterPoolByMaskRecordId(WMaskRecordId);
                            if (Mp.RecordFound == true)
                            {
                                //Dt.ReplCycle = Tc.SesNo;
                                //Dt.StartTrxn = Tc.StartTrxn;
                                //Dt.EndTrxn = Tc.EndTrxn;
                                //Dt.ErrNo = Tc.ErrNo;
                            }
                            else
                            {
                            Dt.ReplCycle = 0;
                            Dt.StartTrxn = 0;
                            Dt.EndTrxn = 0;
                            Dt.ErrNo = 0;
                        }

                        Dt.BankId = UserBankId;

                        Dt.MaskRecordId = Mp.MaskRecordId;

                        Dt.BankId = Mp.Operator;

                        Dt.DisputeNumber = InDispNo;
                        Dt.DispDtTm = DateTime.Now;
                        Dt.Origin = WOrigin;

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
                            WD24 = Dt.DisputedAmt.ToString();
                        }

                        if (TranCount == 2)
                        {

                            WD28 = Dt.DisputedAmt.ToString();
                        }

                        if (TranCount == 3)
                        {
                            WD32 = Dt.DisputedAmt.ToString();
                        }

                        if (TranCount == 4)
                        {
                            WD36 = Dt.DisputedAmt.ToString();
                        }
                        Dt.Operator = WOperator;

                        int DTranNo = Dt.InsertDisputeTran(Di.DispId);

                        if (Mp.DebitBank == UserBankId) // Dispute Is On Debit 
                        {
                            WTxnCode = 11;
                            WDisputeActionId = 3;
                        }

                        if (Mp.CreditBank == UserBankId) // Dispute Is On Credit 
                        {
                            WTxnCode = 21;
                            WDisputeActionId = 1;
                        }                      

                        Dt.ReadDisputeTran(DTranNo); // Get all values
                                                     // Prepare the two values 
                        Dt.TxnCode = WTxnCode;
                        Dt.DisputeActionId = WDisputeActionId;

                        Dt.UpdateDisputeTranRecord(DTranNo);

                    }
                }

                K++; // Read Next entry of the table 
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

            //Create Printing 
            Di.ReadDispute(WDispNo);
            WD11 = Di.CustName;
            WD12 = Di.AccNo;
            WD13 = Di.CardNo;
            //TEST
            WD14 = "Y";
            WD15 = "N";

            WD16 = Di.CustPhone;
            WD17 = Di.CustEmail;

            string TDispType = "251"; // Parameter Id for dispute types

            Gp.ReadParametersSpecificId(WOperator, TDispType.ToString(), Di.DispType.ToString(), "", "");

            WD37 = Gp.OccuranceNm;

            if (Di.DispType == 5)
            {
                WD38 = Di.OtherDispTypeDescr;
            }
            else
            {
                WD38 = "";
            }

            WD40 = Di.DispId.ToString();

            string WD39 = ""; 

            RRDMUsersRecords Us = new RRDMUsersRecords();
            Us.ReadUsersRecord(WSignedId);

            WD41 = Us.UserName;

            
            Form56R5 ReportDispute = new Form56R5(WD11, WD12, WD13, WD14, WD15, WD16, WD17,
                        WD21, WD22, WD23, WD24,
                        WD25, WD26, WD27, WD28,
                        WD29, WD30, WD31, WD32,
                        WD33, WD34, WD35, WD36,
                        WD37, WD38,
                        WD40, WD41, WOperator, WD39, "WD45", "WD46", "WD47", "WD48","WD50");

            ReportDispute.Show();
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
// Show ALL Previous Disputes 
        private void button2_Click(object sender, EventArgs e)
        {
            Form3_ITMX NForm3ITMX;

            if (tbCustomerUniqueId.TextLength > 0)
            {
               // Data was inputed 
            }
            else
            {
                MessageBox.Show("Please enter data for customer id!");
                return; 
            }

            // Check if there are Disputes for this ID 

            Di.ReadDisputeTotals(tbCustomerUniqueId.Text, UserBankId); 


            if (Di.TotalForCard > 0)
            {
                NForm3ITMX = new Form3_ITMX(WSignedId, WSignRecordNo, WOperator, tbCustomerUniqueId.Text,"VIEW");
                NForm3ITMX.ShowDialog();
            }
            else
            {
                MessageBox.Show("No other Disputes for this customer");
            }
        }
        //**********************************************************************
        // END NOTES 
        //**********************************************************************    

    }
}


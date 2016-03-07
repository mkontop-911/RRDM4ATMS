using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;


namespace RRDM4ATMsWin
{
    public partial class Form5 : Form
    {
     //   Form11 NForm11;
        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 
        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
        RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();
        
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDM_JccTransClass Jt = new RRDM_JccTransClass(); 

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

        public DataTable CardAtmsTran = new DataTable();

        //string SQLString;

        string WFilter;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        //int TotalChosenATMs;
        //int TotalChosenJCC; 

        //int TotTrans;

        //bool FoundInMatchedUnMatched; 

        int WDispNo;
        bool chosen;
        string AtmNo;
        int ATMTranNo;
        int JCCTranNo;
        string Curr;
        decimal Amount;
        decimal DispAmnt;
        DateTime TranDate;
        string Descr;
        int ErrNo;
        string Origin;
        // "OurATMs"
        // "ProcessorATMs"
        // "ProcessorMerchants" 

        bool FoundInUnMatched;
        bool FoundInMatched;

        bool FoundInPoolATMs;
       

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WCardNo;

        string WCardNoBin;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     
        DateTime WFromDate;
        DateTime WToDate;
        string WCardNoIn;
        int WTranNo;
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
            WTranNo = InTranNo;
            WCounted = InCounted;
            WInDispNo = InDispNo; // > 0 if InFrom = 4 
            WComment = InComment; 
            WFrom = InFrom; 
            // 1 = call is coming from Main Form, 
            // 2 = Call is coming from Pre-Investigation 
            // 3 = Call is coming from deposits in difference,  
            // 4= call is coming for updating details of dispute,  
            // 5= call is coming from Reconciliation Process for ATMs CAsh - Record found in pool 
            //
            // 7= call is coming from Reconciliation Process matching reconciliation - record found through mask,

            WOrigin = InOrigin; // "ATM" OR "JCC" OR "RMCategory"; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            textBoxMsgBoard.Text = "Input information for dispute";

            Gp.ParamId = "252"; // Dispute methods - Internal Or external Customer   
            comboBoxVisitType.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxVisitType.DisplayMember = "DisplayValue";

            tbComments.Text = WComment;

            if (WFrom == 3 || (WFrom == 2 & WOrigin == "ATM") || WFrom == 5 || WFrom == 7) // From deposits or Pre-investigation or reconciliation 
            {

                btTransactions.Text = "Show Transaction";

                if (WFrom == 3)
                {
                    string ParamId = "252"; // Dispute method Deposit    
                    string OccNumber = "7";
                    Gp.ReadParametersSpecificParmAndOccurance(WOperator, ParamId, OccNumber);

                    comboBoxVisitType.Text = Gp.OccuranceNm; 
                }
                if (WFrom == 5)
                {
                    string ParamId = "252"; // Dispute method Reconciliation 
                    string OccNumber = "8";
                    Gp.ReadParametersSpecificParmAndOccurance(WOperator, ParamId, OccNumber);

                    comboBoxVisitType.Text = Gp.OccuranceNm;

                    btTransactions.Text = "Show Trans"; 
                }

                if (WFrom == 7)
                {
                    string ParamId = "252"; // Dispute method Reconciliation 
                    string OccNumber = "10";
                    Gp.ReadParametersSpecificParmAndOccurance(WOperator, ParamId, OccNumber);

                    comboBoxVisitType.Text = Gp.OccuranceNm;

                    btTransactions.Text = "Show Trans";
                }

     // find record details in InPool or matched unmatched tables

                if (WTranNo > 0) 
                {
                    if (WFrom == 5) // Record to be found from InPool
                    {
                        //TEST
                        Tc.ReadInPoolTransSpecific(WTranNo);
                        if (Tc.RecordFound == true)
                        {
                            FoundInPoolATMs = true;

                            tbCardNo.Text = Tc.CardNo;
                            tbAccNo.Text = Tc.AccNo;

                            dateTimePickerFrom.Value = Tc.AtmDtTime;
                            dateTimePickerTo.Value = Tc.AtmDtTime;

                        }
                        else
                        {
                            MessageBox.Show("Record Not Found");
                            return;
                        }
                     }
                    if (WFrom == 7) // Transaction will be found in Matched /Unmatched 
                    {
                        // Find Details of Masked REcord 
                        Rm.ReadMatchedORUnMatchedFileSpecificRecordByMaskId("Matched", WTranNo);
                        if (Rm.RecordFound == true)
                        {
                            //FoundInMatchedUnMatched = true;
                            FoundInMatched = true;
                            dateTimePickerFrom.Value = Rm.TransDate;
                            dateTimePickerTo.Value = Rm.TransDate;
                            tbAccNo.Text = Rm.AccNumber;
                        }
                        else
                        {
                            Rm.ReadMatchedORUnMatchedFileSpecificRecordByMaskId("UnMatched", WTranNo);
                            if (Rm.RecordFound == true)
                            {
                                //FoundInMatchedUnMatched = true;
                                FoundInUnMatched = true;
                                dateTimePickerFrom.Value = Rm.TransDate;
                                dateTimePickerTo.Value = Rm.TransDate;
                                tbAccNo.Text = Rm.AccNumber;
                            }
                            else
                            {
                                MessageBox.Show("Record Not Found");
                                    return;

                            }
                        }
                    }
                   
                }
                
             
                tbCardNo.Text = WCardNoIn;
                

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
                btTransactions.Hide();
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

        
                if (radioButtonDepositDiff.Checked == true) // Deposits ...  ... 
                {
                    WFilter = "CardNumber ='"+ tbCardNo.Text + "' AND Operator = @Operator "
                              + " AND (TransType = 23 OR TransType = 24) ";
                }
                else // Withdrawls ... ... 
                {
                    WFilter = "CardNumber ='" + tbCardNo.Text + "' AND Operator = @Operator "
                              + " AND TransType = 11 ";

                    //    + " WHERE CardNo = @CardNo AND TransType = 11 AND (AtmDtTime >= @WFromDate AND  AtmDtTime <= @WToDate) ";
                }


                if ((WFrom == 2 & WOrigin == "ATM") || WFrom == 3 || WFrom == 5) // Coming from pre-nvestigation or deposits where Tran is specified 
                {
                    //WFilter = " MaskRecordId = " + WTranNo;

                    btTransactions.Hide();
                }

                if (WFrom == 7) // Coming from pre-nvestigation or deposits where Tran is specified 
                {
                    WFilter = "Operator = @Operator AND MaskRecordId = " + WTranNo; // Only this transaction  

                    btTransactions.Hide();
                }

            WFromDate = dateTimePickerFrom.Value;
            WToDate = dateTimePickerTo.Value;
            WCardNo = tbCardNo.Text;
            //TEST
            if (WCardNo.Length >15)
            {
                WCardNoBin = WCardNo.Substring(0, 6) + "******" + WCardNo.Substring(12, 4);
            }
           
            label13.Text = "TRANSACTIONS FOR: " + WCardNo;

            string WSortValue = "SeqNo";
            //string WhatFile = "Both";
            int CallingFrom = 2; // Disputes 

            if ((WFrom == 2 & WOrigin == "ATM") || WFrom == 3 || WFrom == 5 || WFrom == 7)
            {
                

                Rm.ReadBothMatchedUnMatchedFileTable(WOperator, WFilter, NullPastDate, NullPastDate, WSortValue, CallingFrom);


                if (Rm.RecordFound == true)
                {
                    // Record found in Unmatched / Matched
                    dataGridView1.DataSource = Rm.RMDataTableLeft.DefaultView;
                    textBox5.Text = Rm.TotalSelected.ToString(); 
                }
                else
                {
                    Tc.ReadInPoolTransSpecificForDisputesTable(WTranNo);
                    dataGridView1.DataSource = Tc.TableDisputedTrans.DefaultView;
                    textBox5.Text = Tc.TotalSelected.ToString(); 
                  //  
                }
             
            }
            else
            {
                Rm.ReadBothMatchedUnMatchedFileTable(WOperator, WFilter, WFromDate, WToDate, WSortValue, CallingFrom);
                if (Rm.RecordFound == true)
                {
                    // Record found in Unmatched / Matched
                    dataGridView1.DataSource = Rm.RMDataTableLeft.DefaultView;
                    textBox5.Text = Rm.TotalSelected.ToString(); 
                }
                else
                {
                    //Tc.ReadInPoolTransSpecificForDisputesTable(WMaskRecordId);
                    //dataGridView1.DataSource = Tc.TableDisputedTrans.DefaultView;
                    ////  
                }
              
            }

           

                dataGridView1.Columns[0].Name = "Chosen";
                dataGridView1.Columns[1].Name = "DisputedAmnt";
                dataGridView1.Columns[2].Name = "Card";
                dataGridView1.Columns[3].Name = "Account";
                dataGridView1.Columns[4].Name = "Curr";
                dataGridView1.Columns[5].Name = "Amount";
                dataGridView1.Columns[6].Name = "TransDate";
                dataGridView1.Columns[7].Name = "TransDescr";
                dataGridView1.Columns[8].Name = "MaskRecordId";
                dataGridView1.Columns[9].Name = "RMCategory";

                // SIZE
                dataGridView1.Columns["Chosen"].Width = 50; //
                dataGridView1.Columns["DisputedAmnt"].Width = 60;
                dataGridView1.Columns["Card"].Width = 60;
                dataGridView1.Columns["Account"].Width = 60;
                dataGridView1.Columns["Curr"].Width = 60;
                dataGridView1.Columns["Amount"].Width = 70;
                dataGridView1.Columns["TransDate"].Width = 70;
                dataGridView1.Columns["TransDescr"].Width = 100;
                dataGridView1.Columns["MaskRecordId"].Width = 60;
                dataGridView1.Columns["RMCategory"].Width = 85;


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

            // Check If Data 
            if (dataGridView1.Rows.Count == 0 )
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No transactions for this selection");
                MessageForm.ShowDialog();

                panel4.Hide(); 
                label13.Hide();

                return;
            }
               

            textBox5.Text = Rm.TotalSelected.ToString();

            label13.Show();
            panel4.Show();

            button1.Show();


            buttonAdd.Show();

            textBoxMsgBoard.Text = "Review transactions and mark the dispute ones."; 
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

                Di.CardNo = tbCardNo.Text;
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

            for (int rows = 0; rows < dataGridView1.Rows.Count - 1; rows++)
            {

                chosen = (bool)dataGridView1.Rows[rows].Cells["Chosen"].Value;
                if (WFrom != 5) WTranNo = (int)dataGridView1.Rows[rows].Cells["MaskRecordId"].Value;

                if (chosen == true)
                {
                    TranSelected = TranSelected + 1; 
                    // Check if already exist
                    Dt.ReadDisputeTranForInPool(WTranNo);
                    if (Dt.RecordFound == true & Dt.DisputeNumber != WDispNo)
                    {
                        MessageBox.Show("MaskRecordId: " + WTranNo.ToString() + " Already exist in Dispute Number: "
                                                                                                + Dt.DisputeNumber.ToString());
                        WDispNo = Dt.DisputeNumber;
                        return;
                    }

                }
            }

            if (TranSelected == 0) // User didnt select a transaction 
            {
                MessageBox.Show("Please select a transaction to be moved to dispute.");
                return; 
            }

            if (tbId.Text == String.Empty)  // Create new dispute 
            {
                Di.BankId = WOperator; 
                Di.RespBranch = "251";
                Di.LastUpdateDtTm = DateTime.Now;
                Di.DispFrom = WFrom;
                Di.DispType = 0;

                if (radioButton1.Checked == true) Di.DispType = 1;
                if (radioButton2.Checked == true) Di.DispType = 2;
                if (radioButton3.Checked == true) Di.DispType = 3;
                if (radioButtonDepositDiff.Checked == true) Di.DispType = 4;
                if (radioButtonReconcDiff.Checked == true) Di.DispType = 5;
                if (radioButtonOther.Checked == true) Di.DispType = 6;

                Di.OpenDate = DateTime.Now;
                DateTime today = DateTime.Now;

                Gp.ReadParametersSpecificId(WOperator, "605", "1",  "", ""); // LIMIT to be solved date // Dispute target dates 
                int QualityRange1 = (int)Gp.Amount;

                Di.TargetDate = today.AddDays(QualityRange1);

                Di.CloseDate = NullPastDate; 
                Di.CardNo = tbCardNo.Text;
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

                Di.InsertDisputeRecord();

                //TEST 
                Di.ReadDisputeLastNo(WSignedId);

                Dt.DeleteTransOfthisDispute(Di.DispId);
                // Call Method to insert chosen transactions 
                InsertDisputeTrans(Di.DispId);

                Us.ReadSignedActivityByKey(WSignRecordNo);

                if (Us.StepLevel < 1)
                {
                    Us.StepLevel = 1;
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                //Identity = Di.Identity;
                //MessageBox.Show(Identity.ToString());
                //Identity = Di.nEOID;// nEOID = Convert.ToInt32(retVal);
                //MessageBox.Show(Identity.ToString());

                Di.ReadDisputeLastNo(WSignedId);

                //MessageBox.Show(DispID.ToString());
               
                tbId.Text = Di.DispId.ToString();

                MessageBox.Show("New Dispute with number: " + tbId.Text + " has been created");
                WDispNo = Di.DispId;
            }

            else
            {
              // Update Dispute record 
               
                Di.OpenDate = DateTime.Now;
                
                Di.CardNo = tbCardNo.Text;
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

                for (int rows = 0; rows < dataGridView1.Rows.Count - 1; rows++)
                {
                    chosen = (bool)dataGridView1.Rows[rows].Cells["Chosen"].Value;
                    if (WFrom != 5) WTranNo = (int)dataGridView1.Rows[rows].Cells["MaskRecordId"].Value;

                    DispAmnt = (decimal)dataGridView1.Rows[rows].Cells["DisputedAmnt"].Value;

                    if (chosen == true)
                    {
                        // Check if already exist
                        Dt.ReadDisputeTranForInPool(WTranNo);
                        if (Dt.RecordFound == true)
                        {
                            MessageBox.Show("RecordId: " + WTranNo.ToString() + " Already exist in Dispute Number: "
                                + Dt.DisputeNumber.ToString());
                            WDispNo = Dt.DisputeNumber;
                            return;
                        }
                        else // INSERT TRANSACTION 
                        {
                            if (WFrom == 5) // Record to be found from InPool
                            {
                                Ec.ReadErrorsTableSpecificByTransNo(WTranNo);
                                if (Ec.RecordFound == true)
                                {
                                    Dt.ErrNo = Ec.ErrNo; // GET THE ERROR No from Error table 
                                }
                                else Dt.ErrNo = 0;
                            }
                            else // Not coming from Cash reconciliation 
                            {
                                Ec.ReadErrorsTableSpecificByMaskRecordId(WTranNo);
                                if (Ec.RecordFound == true)
                                {
                                    Dt.ErrNo = Ec.ErrNo; // GET THE ERROR No from Error table 
                                }
                                else Dt.ErrNo = 0;

                            }
                          
                            // Find Details of Masked REcord 

                            if (WFrom == 5) // Record to be found from InPool
                            {
                                //TEST
                                Tc.ReadInPoolTransSpecific(WTranNo);
                                if (Tc.RecordFound == true)
                                {
                                    Dt.SystemTarget = Tc.SystemTarget;
                                    Dt.ReplCycle = Tc.SesNo;
                                    Dt.StartTrxn = Tc.StartTrxn;
                                    Dt.EndTrxn = Tc.EndTrxn;
                                    Dt.ErrNo = Tc.ErrNo;
                                    Dt.MaskRecordId = Tc.MaskRecordId;

                                    Dt.BankId = Tc.BankId;
                                    Dt.DbTranNo = WTranNo;
                                    Dt.TranDate = Tc.AtmDtTime;
                                    Dt.AtmNo = Tc.AtmNo;


                                    Dt.CardNo = Tc.CardNo;
                                    Dt.AccNo = Tc.AccNo;
                                    Dt.CurrencyNm = Tc.CurrDesc;
                                    Dt.TranAmount = Tc.TranAmount;

                                    Dt.TransType = Tc.TransType;
                                    Dt.TransDesc = Tc.TransDesc;

                                }
                                else
                                {
                                    MessageBox.Show("Record Not Found");
                                    return;
                                }
                            }
                            if (WFrom != 5) // Transaction doesnot come from ATMs cash reconciliation but comes from RM Matched /Unmatched  it can be 7 or 
                            {
                                Rm.ReadMatchedORUnMatchedFileSpecificRecordByMaskId("Matched", WTranNo);
                                if (Rm.RecordFound == true)
                                {
                                    //FoundInMatchedUnMatched = true;
                                    Dt.MaskRecordId = Rm.MaskRecordId;

                                    Dt.BankId = Rm.Operator;
                                    Dt.DbTranNo = Rm.SeqNo;
                                    Dt.TranDate = Rm.TransDate;
                                    Dt.AtmNo = Rm.TerminalId;

                                    Dt.CardNo = Rm.CardNumber;
                                    Dt.AccNo = Rm.AccNumber;
                                    Dt.CurrencyNm = Rm.TransCurr;
                                    Dt.TranAmount = Rm.TransAmount;

                                    Dt.TransType = Rm.TransType;
                                    Dt.TransDesc = Rm.TransDescr;

                                }
                                else
                                {
                                    Rm.ReadMatchedORUnMatchedFileSpecificRecordByMaskId("UnMatched", WTranNo);
                                    if (Rm.RecordFound == true)
                                    {
                                        //FoundInMatchedUnMatched = true;
                                        Dt.MaskRecordId = Rm.MaskRecordId;

                                        Dt.BankId = Rm.Operator;
                                        Dt.DbTranNo = Rm.SeqNo;
                                        Dt.TranDate = Rm.TransDate;
                                        Dt.AtmNo = Rm.TerminalId;

                                        Dt.CardNo = Rm.CardNumber;
                                        Dt.AccNo = Rm.AccNumber;
                                        Dt.CurrencyNm = Rm.TransCurr;
                                        Dt.TranAmount = Rm.TransAmount;

                                        Dt.TransType = Rm.TransType;
                                        Dt.TransDesc = Rm.TransDescr;

                                    }
                                    else
                                    {
                                        MessageBox.Show("Record Not Found");
                                        return;
                                    }
                                }
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
                                Dt.ReplGroup = 0 ;
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
                                WD21 = Dt.TranDate.Date.ToString();
                                WD22 = Dt.AtmNo.ToString();
                                WD23 = Dt.TranAmount.ToString();
                                WD24 = Dt.DisputedAmt.ToString();
                            }

                            if (TranCount == 2)
                            {
                                WD25 = Dt.TranDate.Date.ToString();
                                WD26 = Dt.AtmNo.ToString();
                                WD27 = Dt.TranAmount.ToString();
                                WD28 = Dt.DisputedAmt.ToString();
                            }

                            if (TranCount == 3)
                            {
                                WD29 = Dt.TranDate.Date.ToString();
                                WD30 = Dt.AtmNo.ToString();
                                WD31 = Dt.TranAmount.ToString();
                                WD32 = Dt.DisputedAmt.ToString();
                            }

                            if (TranCount == 4)
                            {
                                WD33 = Dt.TranDate.Date.ToString();
                                WD34 = Dt.AtmNo.ToString();
                                WD35 = Dt.TranAmount.ToString();
                                WD36 = Dt.DisputedAmt.ToString();
                            }
                            Dt.Operator = WOperator;
                            Dt.InsertDisputeTran(InDispNo);

                            WDispNo = InDispNo;
                        }
                    }
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

            RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
            Us.ReadUsersRecord(WSignedId);

            WD41 = Us.UserName;

            Form56R5 ReportDispute = new Form56R5(WD11, WD12, WD13, WD14, WD15, WD16, WD17,
                        WD21, WD22, WD23, WD24,
                        WD25, WD26, WD27, WD28,
                        WD29, WD30, WD31, WD32,
                        WD33, WD34, WD35, WD36,
                        WD37, WD38,
                        WD40, WD41); 

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
                NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
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
      
    }
}


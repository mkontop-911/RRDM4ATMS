using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80bΙΤΜΧ : Form
    {
        //RRDMReconcMatchedUnMatchedVisaAuthorClass Mp = new RRDMReconcMatchedUnMatchedVisaAuthorClass();
        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();
        RRDMMatchingReconcExceptionsInfoITMX Mre = new RRDMMatchingReconcExceptionsInfoITMX();
        RRDMMatchingCategories Rc = new RRDMMatchingCategories();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();
        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();

        RRDMMatchingMasksVsMetaExceptions Rme = new RRDMMatchingMasksVsMetaExceptions();

        RRDMBanks Ba = new RRDMBanks();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;
        // NOTES END 

        DateTime FromDt;
        DateTime ToDt;

        int Mode;

        int WMaskRecordId;
        string WInputField;
        string WSortCriteria;

        //bool ATMTrans;

        string WSelectionCriteria1;
        string WSelectionCriteria2;
        string WSelectionCriteria3;

        bool FirstCycle;

        string WMask;
        string WSubString;

        string UserBank; 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCategoryId;
        int WReconcCycleNo;
        int WInMaskRecordId;
        string WFunction;

        public Form80bΙΤΜΧ(string InSignedId, int InSignRecordNo, string InOperator, string InCategoryId, int InReconcCycleNo,
                                                  int InMaskRecordId, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategoryId = InCategoryId;
            WReconcCycleNo = InReconcCycleNo;

            WInMaskRecordId = InMaskRecordId; // Coming from externally 

            WFunction = InFunction; // "Reconc", "View", "Investigation"

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            Us.ReadUsersRecord(InSignedId);

            UserBank = Us.BankId; 

            if (UserBank != WOperator)
            {
                // User belongs to the Bank Not to ITMX
                WSelectionCriteria3 = " AND ( DebitBank ='" + UserBank + "' OR CreditBank ='" + UserBank + "' )";

                textBoxInputField.ReadOnly = true; 
            }
            else
            {
                WSelectionCriteria3 = "";
            }

            labelUserId.Text = InSignedId;
            if (WFunction == "DisputeInvestigation")
            {
                pictureBox2.Show();
                pictureBox2.BringToFront();

                if (UserBank != WOperator)
                {
                    labelStep1.Text = "Dispute Investigation Process by : " + UserBank;
                }
                else
                {
                    labelStep1.Text = "Dispute Investigation Process by :" + WOperator;
                }
            }
            else
            {
                Rcs.ReadReconcCategoriesByCategoryIdForName(WCategoryId);
                labelStep1.Text = "Matched and Unmatched Records for this Reconciliation Category/Cycle " +
                                   " OR All Transactions";

                radioButtonOnlyThisCatandCycle.Text = "This : " + Rcs.CategoryName + "/ Cycle:" + WReconcCycleNo.ToString();
                radioButtonAllTxns.Text = "All Txs for all categories";
            }

            if (WFunction == "View")
            {

                textBoxMsgBoard.Text = "View Only";
            }
            if (WFunction == "Interactive A") // FOR ACTION ON UNMATCHED 
            {

            }

            FirstCycle = true;
            // SELECT

            comboBoxFilter.Items.Add("Matched");

            comboBoxFilter.Items.Add("UnMatched");

            //comboBoxFilter.Items.Add("UnMatchedAllCycles");

            comboBoxFilter.Items.Add("Both");

            comboBoxFilter.Text = "Matched";

            // SORT 

            comboBoxSort.Items.Add("MobileNo");

            comboBoxSort.Items.Add("AccountNo");

            comboBoxSort.Items.Add("UniqueNo");

            comboBoxSort.Text = "UniqueNo";
            //TEST
            dateTimePicker1.Value = new DateTime(2015, 08, 28);

            radioButtonOnlyThisCatandCycle.Checked = true;

            panel5.Hide();

            if (WFunction == "DisputeInvestigation")
            {
                radioButtonOnlyThisCatandCycle.Checked = false;
                radioButtonAllTxns.Checked = true;
                comboBoxFilter.Text = "Both";
                comboBoxSort.Text = "UniqueNo";
                checkBoxUnique.Checked = true;
                if (WInMaskRecordId>0)
                {
                    radioButtonUniqueNo.Checked = true;
                    textBoxInputField.Text = WInMaskRecordId.ToString();
                    textBoxMsgBoard.Text = "View the transaction. ";
                }
                else
                {
                    radioButtonCard.Checked = true;
                    textBoxInputField.Text = "99622248";
                    textBoxMsgBoard.Text = "Select dates, and other and press Show. ";
                }
                             
                //panel9.Hide();
                checkBoxUnique.Hide();

                buttonRegisterDispute.Show();

                
            }
            else
            {
                buttonRegisterDispute.Hide();
            }
        }
        // Load 
        private void Form80b_Load(object sender, EventArgs e)
        {
            WSelectionCriteria2 = " ";

            if (comboBoxFilter.Text == "UnMatched")
            {
                WSelectionCriteria2 = " AND ReconcMatched = 0 ";

            }

            if (comboBoxFilter.Text == "Matched")
            {
                WSelectionCriteria2 = " AND ReconcMatched = 1 ";

            }

            if (comboBoxFilter.Text == "Both")
            {
                WSelectionCriteria2 = " "; // No condition 

            }

            if (checkBoxUnique.Checked == false) // Not Unique 
            //if (comboBoxUnique.Text == "NoUnique")
            {
                if (comboBoxSort.Text == "UniqueNo") WSortCriteria = " ORDER BY MaskRecordId ASC";
                if (comboBoxSort.Text == "MobileNo") WSortCriteria = " ORDER BY MobileRequestor ASC";
                if (comboBoxSort.Text == "AccountNo") WSortCriteria = " ORDER BY AccountRequestor ASC";

                if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                {

                    WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND ReconcCategoryId ='" + WCategoryId + "' AND ReconcCycleNo = " + WReconcCycleNo + " AND SettledRecord = 0 ";

                    if (WFunction == "View" || WFunction == "Investigation" || WFunction == "DisputeInvestigation")
                    {
                        WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND ReconcCategoryId ='" + WCategoryId + "' AND ReconcCycleNo = " + WReconcCycleNo; // Settled and unsettled 

                    }
                }
                if (radioButtonAllTxns.Checked == true) // All transactions
                {
                    WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND SettledRecord = 0 "; // Only Not Settled

                    if (WFunction == "View" || WFunction == "Investigation" || WFunction == "DisputeInvestigation")
                    {
                        WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "'"; // Open and closed 
                    }
                }

            }
            else
            {
                // Unique
                if (radioButtonCard.Checked == false & radioButtonAccount.Checked == false & radioButtonUniqueNo.Checked == false)
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

                WInputField = textBoxInputField.Text;

                if (radioButtonCard.Checked == true) // mobile 
                {
                    if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                    {
                        WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND ReconcCategoryId ='" + WCategoryId + "' AND ReconcCycleNo = " + WReconcCycleNo
                                        + " AND MobileRequestor ='" + WInputField + "' AND SettledRecord = 0 ";

                        if (WFunction == "View" || WFunction == "Investigation" || WFunction == "DisputeInvestigation")
                        {
                            WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND ReconcCategoryId ='" + WCategoryId + "' AND ReconcCycleNo = " + WReconcCycleNo
                                         + " AND MobileRequestor ='" + WInputField + "'";  // Open and closed 

                        }
                    }
                    if (radioButtonAllTxns.Checked == true) // All transactions OR this MAskRecordId
                    {
                        WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND MobileRequestor ='" + WInputField + "' AND SettledRecord = 0 ";  // Only open

                        if (WFunction == "View" || WFunction == "Investigation" || WFunction == "DisputeInvestigation")
                        {
                            WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND MobileRequestor ='" + WInputField + "'";  // Open and closed 
                        }
                    }

                }

                if (radioButtonAccount.Checked == true) // Account 
                {
                    if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this category and this Cycle
                    {
                        WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND ReconcCategoryId ='" + WCategoryId + "' AND ReconcCycleNo = " + WReconcCycleNo
                                        + " AND AccountRequestor ='" + WInputField + "' AND SettledRecord = 0 ";

                        if (WFunction == "View" || WFunction == "Investigation" || WFunction == "DisputeInvestigation")
                        {
                            WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND ReconcCategoryId ='" + WCategoryId + "' AND ReconcCycleNo = " + WReconcCycleNo
                                         + " AND AccountRequestor ='" + WInputField + "'";  // Open and closed 
                        }
                    }
                    if (radioButtonAllTxns.Checked == true) // All transactions
                    {
                        WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND AccountRequestor ='" + WInputField + "' AND SettledRecord = 0 ";  // Only open

                        if (WFunction == "View" || WFunction == "Investigation" || WFunction == "DisputeInvestigation")
                        {
                            WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND AccountRequestor ='" + WInputField + "'";  // Open and closed 

                        }
                    }

                }

                if (radioButtonUniqueNo.Checked == true) // Unique No 
                {
                    if (WInMaskRecordId > 0)
                    {
                        WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND MaskRecordId = " + WInMaskRecordId; // Only this transaction  
                    }         
                    else
                    {
                        if (int.TryParse(textBoxInputField.Text, out WInMaskRecordId))
                        {
                        }
                        else
                        {
                            MessageBox.Show(textBoxInputField.Text, "Please enter a valid number!");
                            return;
                        }
                        WSelectionCriteria1 = "WHERE Operator ='" + WOperator + "' AND MaskRecordId = " + WInMaskRecordId; // Only this transaction  
                    }
                   
                }

            }
            // FILL TABLE AND SHOW
            if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this Categ and Cycle both
            {
                //No Dates Are selected
                FromDt = NullPastDate;
                ToDt = NullPastDate;

                int Mode = 1;
                Mp.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WSignedId, Mode, WSelectionCriteria1, WSelectionCriteria2, WSelectionCriteria3, WSortCriteria, FromDt, ToDt);
                ShowGrid();
            }
            // FILL TABLE AND SHOW
            if (radioButtonAllTxns.Checked == true) // All transactions
            {
                WSortCriteria = " ORDER BY MaskRecordId ASC";
                FromDt = dateTimePicker1.Value.AddDays(-1);
                ToDt = dateTimePicker2.Value.AddDays(1);
                Mode = 1;
                Mp.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WSignedId, Mode, WSelectionCriteria1, WSelectionCriteria2, WSelectionCriteria3, WSortCriteria, FromDt, ToDt);
                ShowGrid();
            }

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No transactions for this selection");

                panel7.Hide();
                panel21.Hide();
                label11.Hide();
                textBoxMask.Hide();
                return;
            }
            else
            {
                panel7.Show();
                panel21.Show();
                label11.Show();
                textBoxMask.Show();
            }
            //TEST
            if (textBoxInputField.Text == "9962224889" & WInMaskRecordId == 0 & dataGridView1.Rows.Count > 2)
            {
                int WRow1 = 3;
                dataGridView1.Rows[WRow1].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            }

            FirstCycle = false;

        }

        // Show Grid Left 
        public void ShowGrid()
        {

            dataGridView1.DataSource = Mp.MatchingMasterDataTableITMX.DefaultView;

            dataGridView1.Columns[0].Width = 40; // RecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 60; // Status
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 100; // ReconcCategory
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[3].Width = 40; //  RecCycle

            dataGridView1.Columns[4].Width = 35; // Ccy

            dataGridView1.Columns[5].Width = 60; // Amount
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

            dataGridView1.Columns[6].Width = 30; //DebitMASK
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 30; // CreditMASK
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dataGridView1.Columns[8].Width = 70; // ExecutionTxnDtTm
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WMaskRecordId = (int)rowSelected.Cells[0].Value;

            Rcs.ReadReconcCategoriesByCategoryIdForName(WCategoryId); 

            Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WOperator, WMaskRecordId);

            if (WFunction == "DisputeInvestigation")
            {
                WCategoryId = Mp.ReconcCategoryId;
                WReconcCycleNo = Mp.ReconcCycleNo;
            }

            // NOTES START  
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mp.MaskRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            // NOTES END

            WMaskRecordId = Mp.MaskRecordId;

            textBox10.Text = Rcs.CategoryName;

            textBoxRmCycle.Text = Mp.ReconcCycleNo.ToString();
            textBoxCardNo.Text = Mp.MobileRequestor;
            textBoxAccNo.Text = Mp.AccountRequestor;
            textBoxCurr.Text = Mp.Ccy;
            textBoxAmnt.Text = Mp.Amount.ToString("#,##0.00");
            textBoxDtTm.Text = Mp.ExecutionTxnDtTm.ToString();

            textBoxITMXId.Text = Mp.ITMXUniqueTxnRef.ToString(); 
            textBoxRRDMXId.Text = Mp.MaskRecordId.ToString();

            WMask = Mp.DebitMASK + Mp.CreditMASK;

            Rme.ReadMatchingMaskRecordbyMaskId(WOperator, Mp.ReconcCategoryId, WMask, 11);

            textBoxUnMatchedType.Text = Rme.MaskName;

            Mre.ReadMatchingReconcExceptionsInfobyMaskRecordId(WOperator, WMaskRecordId);
            if (Mre.RecordFound == true)
            {
                Ec.ReadErrorsTableSpecific(Mre.MetaExceptionNo);
                textBoxActionDesc.Text = Mre.ActionTypeDescr;
                textBox6.Text = Mre.UserId;
                if (Mre.Authoriser != "")
                {
                    textBox13.Text = Mre.Authoriser;
                    textBox14.Text = Mre.AuthoriserDtTm.ToString();
                    Tp.ReadTransToBePostedSpecificByUniqueRecordId(WMaskRecordId); 
                    textBoxCreated.Text = Tp.OpenDate.ToString();
                 
                }
                else
                {
                    textBox13.Text = "Authorisation Outstanding";
                    textBox14.Text = "Authorisation Outstanding";
                }


                // User, Authoriser 
                panel3.Show();
                panel8.Show();
                label3.Show();
                label27.Show();
            }
            else
            {
                if (Mp.ReconcMatched == false)
                {
                    // Show panel and say no reconiled yet 
                    label3.Text = "Reconciliation Outstanding";

                    label3.Show();
                }
                else
                {
                    panel3.Hide();
                    panel8.Hide();
                    label3.Hide();
                    label27.Hide();
                }
            }


            // Get Matching sessions
            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);

            textBoxMask.Text = Mp.DebitMASK + "+" + Mp.CreditMASK;

            if (Mp.ReconcMatched == false) // Denotes if record is matched or unmatched
            {
                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, Mp.ReconcCategoryId, Mp.ReconcCycleNo); 
                if (Rcs.RemainReconcExceptions == 0)
                {
                    buttonReconcPlay.Show();
                }
                else
                {
                    buttonReconcPlay.Hide();
                }
                
            }
            else
            {
                textBoxUnMatchedType.Text = "Matched";
                buttonReconcPlay.Hide();
            }

            WMask = Mp.DebitMASK;

            // First Line
            if (Mp.DebitFileId01 != "")
            {
                labelFileA.Show();
                textBox1.Show();

                labelFileA.Text = "File A : " + Mp.DebitFileId01;
                labelFileA.Show();
                WSubString = WMask.Substring(0, 1);
                if (WSubString == "0")
                {
                    textBox1.BackColor = Color.Red;
                    textBox1.ForeColor = Color.White;
                    textBox1.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox1.BackColor = Color.Lime;
                    textBox1.ForeColor = Color.White;
                    textBox1.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox1.BackColor = Color.Lime;
                    textBox1.ForeColor = Color.White;
                    textBox1.Text = ">";
                }
            }
            else
            {
                labelFileA.Hide();
                textBox1.Hide();
            }

            // Second Line 
            if (Mp.DebitFileId02 != "")
            {
                labelFileB.Show();
                textBox2.Show();

                labelFileB.Text = "File B : " + Mp.DebitFileId02;
                labelFileB.Show();
                WSubString = WMask.Substring(1, 1);
                if (WSubString == "0")
                {
                    textBox2.BackColor = Color.Red;
                    textBox2.ForeColor = Color.White;
                    textBox2.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox2.BackColor = Color.Lime;
                    textBox2.ForeColor = Color.White;
                    textBox2.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox2.BackColor = Color.Lime;
                    textBox2.ForeColor = Color.White;
                    textBox2.Text = ">";
                }
            }
            else
            {
                labelFileB.Hide();
                textBox2.Hide();
            }

            // Third Line 
            //
            if (Mp.DebitFileId03 != "")
            {
                labelFileC.Show();
                textBox3.Show();

                labelFileC.Text = "File C : " + Mp.DebitFileId03;
                labelFileC.Show();
                WSubString = WMask.Substring(2, 1);
                if (WSubString == "0")
                {
                    textBox3.BackColor = Color.Red;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox3.BackColor = Color.Lime;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox3.BackColor = Color.Lime;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = ">";
                }
            }
            else
            {
                labelFileC.Hide();
                textBox3.Hide();
            }
            //****************************************************************************
            //Credit Leg 
            //****************************************************************************

            WMask = Mp.CreditMASK;

            // Forth Line 
            if (Mp.CreditFileId01 != "")
            {
                labelFileD.Show();
                textBox4.Show();

                labelFileD.Text = "File D : " + Mp.CreditFileId01;
                labelFileD.Show();
                WSubString = WMask.Substring(0, 1);
                if (WSubString == "0")
                {
                    textBox4.BackColor = Color.Red;
                    textBox4.ForeColor = Color.White;
                    textBox4.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox4.BackColor = Color.Lime;
                    textBox4.ForeColor = Color.White;
                    textBox4.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox4.BackColor = Color.Lime;
                    textBox4.ForeColor = Color.White;
                    textBox4.Text = ">";
                }
            }
            else
            {
                labelFileD.Hide();
                textBox4.Hide();
            }

            // Fifth Line 
            if (Mp.CreditFileId02 != "")
            {
                labelFileE.Show();
                textBox5.Show();

                labelFileE.Text = "File E : " + Mp.CreditFileId02;
                labelFileE.Show();
                WSubString = WMask.Substring(1, 1);
                if (WSubString == "0")
                {
                    textBox5.BackColor = Color.Red;
                    textBox5.ForeColor = Color.White;
                    textBox5.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox5.BackColor = Color.Lime;
                    textBox5.ForeColor = Color.White;
                    textBox5.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox5.BackColor = Color.Lime;
                    textBox5.ForeColor = Color.White;
                    textBox5.Text = ">";
                }
            }
            else
            {
                labelFileE.Hide();
                textBox5.Hide();
            }
            // sixth Line 
            if (Mp.CreditFileId03 != "")
            {
                labelFileF.Show();
                textBox12.Show();

                labelFileF.Text = "File F : " + Mp.CreditFileId03;
                labelFileF.Show();
                WSubString = WMask.Substring(2, 1);
                if (WSubString == "0")
                {
                    textBox12.BackColor = Color.Red;
                    textBox12.ForeColor = Color.White;
                    textBox12.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox12.BackColor = Color.Lime;
                    textBox12.ForeColor = Color.White;
                    textBox12.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox12.BackColor = Color.Lime;
                    textBox12.ForeColor = Color.White;
                    textBox12.Text = ">";
                }
            }
            else
            {
                labelFileF.Hide();
                textBox12.Hide();
            }

            //WRRN = Mp.RRNumber;

            // Check if dispute already registered for this transaction 

            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
            // Check if already exist
            Dt.ReadDisputeTranByUniqueRecordId(WMaskRecordId);
            if (Dt.RecordFound == true)
            {
                textBox9.Text = Dt.DisputeNumber.ToString();
                //WFoundDisp = Dt.DisputeNumber;
                if (WFunction == "Investigation" & WInMaskRecordId == 0)
                    MessageBox.Show("Dispute with no : " + Dt.DisputeNumber.ToString() + " already registered for this transaction.");
                labelDispute.Show();
                textBox9.Show();
                buttonRegisterDispute.Hide();
            }
            else
            {
                labelDispute.Hide();
                textBox9.Hide();
                if (WFunction == "Investigation")
                    buttonRegisterDispute.Show();
            }
            Tp.ReadTransToBePostedSpecificByUniqueRecordId(WMaskRecordId);

            if (Tp.ActionCd2 > 0)
            {
                textBoxPosted.Text = Tp.ActionDate.ToString();
            }
            //textBoxInputField.Text = Mp.MobileRequestor;
        }



        // Show Exception 
        private void buttonShowException_Click(object sender, EventArgs e)
        {
            Form24 NForm24;
            bool Replenishment = true;
            int ErrNo = Mre.MetaExceptionNo;
            string SearchFilter = "ErrNo =" + ErrNo;
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WCategoryId, Mp.ReconcCycleNo, "", Replenishment, SearchFilter);
            NForm24.ShowDialog();
        }
        // Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "UniqueRecordId: " + Mp.MaskRecordId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mp.MaskRecordId;
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

            string P1 = "REPORT OF SELECTED TRANSACTIONS";
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId; 

            Form56R34ITMX ReportITMX34 = new Form56R34ITMX(P1, P2, P3, P4, P5);
            ReportITMX34.Show();       

        }

        // SHOW Selection 
        private void buttonShowSelection_Click(object sender, EventArgs e)
        {


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

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
        // Transcaction Trail 
        private void buttonTransTrail_Click(object sender, EventArgs e)
        {
            Form78b NForm78b;
            RRDMMatchingMaskRecordsLocation Rj = new RRDMMatchingMaskRecordsLocation();
            //TEST
            Rj.ReadtblMatchingMaskSpecific(WMaskRecordId, WOperator, Mp.ReconcCategoryId, Mp.ReconcCycleNo);

            if (Rj.RecordFound == true)
            {
                string WHeader = "TRAIL FOR TRANSACTION WITH ID : " + WMaskRecordId.ToString();

                NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rj.JsonSelectedTable, WHeader, "Form80b");
                //NForm78b.FormClosed += NForm78b_FormClosed;
                NForm78b.ShowDialog();

            }
            else
            {
                MessageBox.Show("This selection shows all incidents of transaction" + Environment.NewLine
                                 + "In different matching files. " + Environment.NewLine
                                 + "For this transaction there is non testing data to show."
                                 );
                return;
            }
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void panel2_Paint(object sender, PaintEventArgs e)
        {

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
                buttonShowSelection.Hide();

                radioButtonCard.Checked = false;
                radioButtonAccount.Checked = false;
                radioButtonUniqueNo.Checked = false;

                textBoxInputField.Text = "";
            }
        }
        // Radio Button one only cat/cycle
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOnlyThisCatandCycle.Checked == true)
            {
                panel6.Hide();
            }
            else
            {
                panel6.Show();
            }
        }
        // Radio two all Trans 
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButtonAllTxns.Checked == true)
            {
                panel6.Show();
            }
            else
            {
                panel6.Hide();
            }

        }
        // EXPAND GRID
        private void buttonExpandGridRight_Click(object sender, EventArgs e)
        {
            Form78b NForm78b;
            string WHeader = "LIST OF TRANSACTIONS";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mp.MatchingMasterDataTableITMX, WHeader, "Form80bITMX");
            //NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }
        // Show Trans to Be posted 
        private void buttonTranPosted_Click(object sender, EventArgs e)
        {
            Form78 NForm78;

            Mp.ReadMatchingTxnsMasterPoolByMaskRecordId(Mp.MaskRecordId);
            //if (Mp.RecordFound & Mp.MetaExceptionNo > 0)

            //{
            //    NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator,
            //                                        "", 0, Mp.MetaExceptionNo, 1);
            //    NForm78.ShowDialog();
            //}
            //else
            //{
            //    if (Mre.MetaExceptionNo > 0)
            //    {
            //        NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator,
            //                                       "", 0, Mre.MetaExceptionNo, 1);
            //        NForm78.ShowDialog();
            //    }
            //    else
            //    {
            //        MessageBox.Show("No Transactions/actions were taken for this.");
            //        return;
            //    }

            //}
        }
        // Replenishment Play 
        private void buttonReplPlay_Click(object sender, EventArgs e)
        {
            Form51 NForm51;
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54; // View only for replenishment already done  
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Mp.ReadMatchingTxnsMasterPoolByMaskRecordId(Mp.MaskRecordId);
            if (Mp.RecordFound == true)
            {
                RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                //Ta.ReadSessionsStatusTraces(Mp.AtmNo, Tp.SesNo);
                //if (Ta.RecordFound & Ta.ProcessMode > 0)
                //{
                //    NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, Tp.AtmNo, Tp.SesNo);
                //    NForm51.ShowDialog();
                //}
                //else
                //{
                //    MessageBox.Show("No Replenishement done for this ATM and Replen Cycle");
                //    return;
                //}

            }
            else
            {
                MessageBox.Show("No Replenishement for this type of transaction");
                return;
            }

        }

        // Reconciliation Play 
        private void buttonReconcPlay_Click(object sender, EventArgs e)
        {
            if (WOperator == "ITMX")
            {

                //if (Rcs.StartReconcDtTm == NullPastDate)
                //{
                //    MessageBox.Show("Reconciliation not done yet");
                //    return;
                //}

                // Update Us Process number
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // Reconciliation view 
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Form281 NForm281;

                NForm281 = new Form281(WSignedId, WSignRecordNo, WOperator, Mp.ReconcCategoryId, Mp.ReconcCycleNo);

                NForm281.ShowDialog();

                //MessageBox.Show("No Reconciliation for this type of transaction");
                return;
            }
            if (WCategoryId == "EWB110")
            {
                Form71 NForm71;
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // View only for reconciliation already done  
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Mp.ReadMatchingTxnsMasterPoolByMaskRecordId(WMaskRecordId);
                if (Mp.RecordFound == true)
                {
                    //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                    //Ta.ReadSessionsStatusTraces(Tp.AtmNo, Tp.SesNo);
                    //if (Ta.RecordFound & Ta.ProcessMode > 1)
                    //{
                    //    NForm71 = new Form71(WSignedId, WSignRecordNo, WOperator, Tp.AtmNo, Tp.SesNo);
                    //    NForm71.ShowDialog();
                    //}
                    //else
                    //{
                    //    MessageBox.Show("No Reconciliation done for this ATM and Replen Cycle");
                    //    return;
                    //}
                }
            }
            else
            {
                // Update Us Process number
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // Reconciliation view 
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Form271 NForm271;

                NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, Mp.ReconcCategoryId, Mp.ReconcCycleNo);

                NForm271.ShowDialog();

                //MessageBox.Show("No Reconciliation for this type of transaction");
                return;
            }



        }

        // Register Dispute 
        private void buttonRegisterDispute_Click(object sender, EventArgs e)
        {
            Form5ITMX NForm5ITMX;
            int From = 2; // Coming from Pre-Investigattion ITMX
            NForm5ITMX = new Form5ITMX(WSignedId, WSignRecordNo, WOperator, Mp.MobileRequestor, WMaskRecordId, 0, 0, "", From, "ITMX");
            NForm5ITMX.ShowDialog();
            this.Dispose();
        }
// Show Previous Disputes 
        private void button2_Click(object sender, EventArgs e)
        {
            RRDMDisputesTableClassITMX Di = new RRDMDisputesTableClassITMX(); 

            Form3_ITMX NForm3ITMX;
            
            if (textBoxCardNo.TextLength > 0)
            {
                // Data was inputed 
            }
            else
            {
                MessageBox.Show("Please enter data for customer id!");
                return;
            }

            // Check if there are Disputes for this ID 

            Di.ReadDisputeTotals(textBoxCardNo.Text, UserBank);


            if (Di.TotalForCard > 0)
            {
                NForm3ITMX = new Form3_ITMX(WSignedId, WSignRecordNo, WOperator, textBoxCardNo.Text,"VIEW");
                NForm3ITMX.ShowDialog();
            }
            else
            {
                MessageBox.Show("No other Disputes for this customer");
            }
        }

        private void textBoxPosted_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

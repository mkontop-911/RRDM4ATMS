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
    public partial class UCForm271b : UserControl
    {
        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions(); 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();

        RRDMReconcMasksVsMetaExceptions Rme = new RRDMReconcMasksVsMetaExceptions(); 
      
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        //   string WUserOperator; 
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool ViewWorkFlow; 

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;
        // NOTES END 


        DateTime LeftDt;
        DateTime RightDt;

        string WhatFileLeft;
        string WhatFileRight;

        int WSeqNoLeft;
        int WSeqNoRight;

        int WMaskRecordIdLeft;
        int WMaskRecordIdRight;

        string ForceMatchingCommnets;

        string WCardNumberLeft;
        string WAccNumberLeft;
        string WTransCurrLeft;
        Decimal WTransAmountLeft;

        string WCardNumberRight;
        string WAccNumberRight;
        string WTransCurrRight;
        Decimal WTransAmountRight;

        decimal WAmount;

        string SearchingStringLeft;
        string SearchingStringRight;

        string WTransType;
        int WTransTypeInt;
        //string WFileName;
        //bool Matched;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WRMCategoryId;
        int WRMCycleNo;
        string WOperator;
        string WMainCateg;

        int WAction;
        
        public void UCForm271bPar(string InSignedId, int SignRecordNo, string InOperator, string InRMCategoryId, int InRMCycle, int InAction)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WRMCategoryId = InRMCategoryId;
            WRMCycleNo = InRMCycle;
            WOperator = InOperator;

            WMainCateg = WRMCategoryId.Substring(0, 4);

            WAction = InAction;  // 1 = Matching Actions 

            InitializeComponent();

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 54 || Us.ProcessNo == 55 || Us.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }       

            //labelStep1.Text = "Invstigation and Force Matching for RM Category Id : " + WRMCategoryId;

            // Hide RIGHT PANELS GRID AND INFORMATION 
            //***************************************
            
            panel6.Hide();
            //panel3.Hide();
            panel7.Hide();
            textBox3.Hide();
            textBox14.Hide();
            comboBoxRight.Hide();

            comboBoxLeft.Hide();
            label4.Hide();
            textBoxMaskLeft.Hide(); 

            textBoxHeaderLeft.Text = "ATM TRANSACTIONS";
            textBoxHeaderRight.Text = "MATCHED TRANSACTIONS"; 
            textBox1.Text = "DETAILS OF SELECTED";
            textBox5.Text = "ACTIONS ON SELECTED EXCEPTIONS"; 

            if (WMainCateg == "EWB5")
            {
                textBoxHeaderLeft.Text = "VISA SETTLEMENT TRANSACTIONS";
                textBoxHeaderRight.Text = "VISA AUTHORISATION POOL";
            }
            //else
            //{
            //    textBoxHeaderLeft.Text = "TRANSACTIONS ON THE LEFT";
            //    textBoxHeaderRight.Text = "TRANSACTIONS ON THE RIGHT";
            //}

            if (WMainCateg == "EWB1")
            {
                buttonJournal.Show();
            }
            else buttonJournal.Hide();


            if (WMainCateg == "EWB1" || WMainCateg == "EWB3")
            {
                comboBoxLeft.Items.Add("UnMatched");
                comboBoxLeft.Items.Add("Matched");
                comboBoxLeft.Text = "UnMatched";

                comboBoxRight.Items.Add("VisaAuthorPool");
                comboBoxRight.Items.Add("Matched");
                comboBoxRight.Text = "Matched";
            }
           
            //comboBoxReason.Items.Add("See attached");
            //comboBoxReason.Items.Add("Small difference");
            //comboBoxReason.Text = "See attached";

            if (WMainCateg == "EWB5")
            {
                textBoxHeaderLeft.Text = "VISA SETTLEMENT TRANSACTIONS";
                textBoxHeaderRight.Text = "VISA AUTHORISATION POOL";
                comboBoxRight.Text = "VisaAuthorPool";
            }
            //else
            //{
            //    textBoxHeaderLeft.Text = "TRANSACTIONS ON THE LEFT";
            //    textBoxHeaderRight.Text = "TRANSACTIONS ON THE RIGHT";
            //    comboBoxRight.Text = "UnMatched";
            //    buttonPrintPool.Hide();
            //}

            //textBoxMsgBoard.Text = "Investigate and force matching if needed. "; 
        }


  // SHOW SCREEN 

        public void SetScreen()
        {
            try
            {
                if (comboBoxLeft.Text == "UnMatched")
                {
                    WhatFileLeft = "UnMatched";

                    textBoxHeaderLeft.Text = "EXCEPTIONS (UNMATCHED ATM TRANSACTIONS)";
                    //buttonMovedToUnMatched.Hide();
                }

                if (comboBoxLeft.Text == "Matched")
                {
                    WhatFileLeft = "Matched";

                    textBoxHeaderLeft.Text = "MATCHED ATM TRANSACTIONS";
                    //buttonMoveToMatched.Hide();
                }
                if (ViewWorkFlow == true) // View Only 
                {
                    SearchingStringLeft = "Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"; 
                            

                }
                else
                {
                    SearchingStringLeft = "Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                              + " AND OpenRecord = 1 ";

                }

                Rm.ReadMatchedORUnMatchedFileTableLeft2(WOperator, WhatFileLeft, SearchingStringLeft);



                ShowGridLeft();

                //**********************************************************************
                // RIGHT DATA GRID 
                //**********************************************************************

                if (comboBoxRight.Text == "VisaAuthorPool")
                {
                    WhatFileRight = "VisaAuthorPool";

                    SearchingStringRight = "Operator ='" + WOperator + "'"
                                        + " AND OpenRecord = 1 AND RemainsForMatching = 1 ";

                }
                if (comboBoxRight.Text == "UnMatched")
                {
                    WhatFileRight = "UnMatched";


                    SearchingStringRight = "Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                                  + " AND OpenRecord = 1 ";

                }

                if (comboBoxRight.Text == "Matched")
                {
                    WhatFileRight = "Matched";

                    SearchingStringRight = "Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                                  + " AND OpenRecord = 1  ";
                }

                //Rm.ReadMatchedORUnMatchedFileTableRight2(WOperator, WhatFileRight, SearchingStringRight);

                Rm.ReadMatchedORUnMatchedFileTablerRight2(WOperator, WhatFileRight, SearchingStringRight);

                ShowGridRight();

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }
        }

// ON Left Grid Row Enter 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNoLeft = (int)rowSelected.Cells[0].Value;

            Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFileLeft, WSeqNoLeft);

            // NOTES 2 START  
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Rm.MaskRecordId;
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
            WParameter4 = "Force Matching for" + " Category: " + WRMCategoryId + " Matching SesNo: " + WRMCycleNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            // NOTES 3 END

            WMaskRecordIdLeft = Rm.MaskRecordId;

            WCardNumberLeft = Rm.CardNumber;
            WAccNumberLeft = Rm.AccNumber;
            WTransCurrLeft = Rm.TransCurr;
            WTransAmountLeft = Rm.TransAmount;

            textBoxCardNoLeft.Text = WCardNumberLeft;
            textBoxAccNoLeft.Text = WAccNumberLeft;
            textBoxCurrLeft.Text = WTransCurrLeft;
            textBoxAmountLeft.Text = WTransAmountLeft.ToString("#,##0.00");
            textBoxDateLeft.Text = Rm.TransDate.ToString();

            textBoxMaskLeft.Text = Rm.MatchMask;
            textBoxMaskLeft2.Text = Rm.MatchMask;

            textBox7.Text = Rm.UnMatchedType;

            radioButtonCreateDefault.Checked = false; // 1
            radioButtonCreateManual.Checked = false; // 2
            radioButtonMatched.Checked = false; // 3
            radioButtonPostpone.Checked = false; // 4
            radioButtonCloseIt.Checked = false; // 5 

            if (Rm.ActionType !="0" ) // This the meta exception no created 
            {
                label11.Text = "Action Taken";
                label11.ForeColor = Color.Black;

                buttonAction.Hide();
                buttonUndoAction.Show();

                if (Rm.ActionType == "1")
                {
                    radioButtonCreateDefault.Checked = true;
                    label10.Show();
                    label5.Show();
                    textBoxMetaNo.Show();
                    textBox4.Show();

                    textBoxMetaNo.Text = Rm.MetaExceptionNo.ToString(); // This is a number 
                    Er.ReadErrorsTableSpecific(Rm.MetaExceptionNo);

                    textBox4.Text = Er.ErrDesc;
                }

                if (Rm.ActionType == "4")
                {
                    radioButtonPostpone.Checked = true;
                    label10.Hide();
                    label5.Hide();
                    textBoxMetaNo.Hide();
                    textBox4.Hide();
                }
            }
            else
            {
                label11.Text = "Action NOT Taken Yet!";
                label11.ForeColor = Color.Red;  
                label10.Hide();
                label5.Hide();
                textBoxMetaNo.Hide();
                textBox4.Hide();

                buttonAction.Show();
                buttonUndoAction.Hide();

            }

            LeftDt = Rm.TransDate.Date;

            if (textBoxCardNoLeft.Text == textBoxCardNoRight.Text)
            {
                Color Black = Color.Black;
                labelCardDiff.ForeColor = Black;
                labelCardDiff.Text = "Same";
            }
            else
            {
                Color Red = Color.Red;
                labelCardDiff.ForeColor = Red;
                labelCardDiff.Text = "Differ";
            }

            if (textBoxAccNoLeft.Text == textBoxAccNoRight.Text)
            {
                Color Black = Color.Black;
                labelAccDiff.ForeColor = Black;
                labelAccDiff.Text = "Same";
            }
            else
            {
                Color Red = Color.Red;
                labelAccDiff.ForeColor = Red;
                labelAccDiff.Text = "Differ";
            }

            if (textBoxAmountLeft.Text == textBoxAmountRight.Text)
            {
                Color Black = Color.Black;
                labelMoney.ForeColor = Black;
                labelMoney.Text = "Same";
            }
            else
            {
                Color Red = Color.Red;
                labelMoney.ForeColor = Red;
                labelMoney.Text = "Differ";
            }

            if (ViewWorkFlow == true)
            {
                buttonAction.Hide();
                buttonUndoAction.Hide();
            }

        }
 // Row Enter for the Right  
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNoRight = (int)rowSelected.Cells[0].Value;

            Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFileRight, WSeqNoRight);

            WMaskRecordIdRight = Rm.MaskRecordId;

            WCardNumberRight = Rm.CardNumber;
            WAccNumberRight = Rm.AccNumber;
            WTransCurrRight = Rm.TransCurr;
            WTransAmountRight = Rm.TransAmount;

            textBoxCardNoRight.Text = WCardNumberRight;
            textBoxAccNoRight.Text = WAccNumberRight;
            textBoxCurrRight.Text = WTransCurrRight;
            textBoxAmountRight.Text = WTransAmountRight.ToString("#,##0.00");
            textBoxDateRight.Text = Rm.TransDate.ToString();

            textBoxMaskRight.Text = Rm.MatchMask;

            RightDt = Rm.TransDate.Date;

            if (textBoxCardNoLeft.Text == textBoxCardNoRight.Text)
            {
                Color Black = Color.Black;
                labelCardDiff.ForeColor = Black;
                labelCardDiff.Text = "Same";
            }
            else
            {
                Color Red = Color.Red;
                labelCardDiff.ForeColor = Red;
                labelCardDiff.Text = "Differ";
            }

            if (textBoxAccNoLeft.Text == textBoxAccNoRight.Text)
            {
                Color Black = Color.Black;
                labelAccDiff.ForeColor = Black;
                labelAccDiff.Text = "Same";
            }
            else
            {
                Color Red = Color.Red;
                labelAccDiff.ForeColor = Red;
                labelAccDiff.Text = "Differ";
            }

            if (textBoxCurrLeft.Text == textBoxCurrRight.Text)
            {
                Color Black = Color.Black;
                labelCurrDiff.ForeColor = Black;
                labelCurrDiff.Text = "Same";
            }
            else
            {
                Color Red = Color.Red;
                labelCurrDiff.ForeColor = Red;
                labelCurrDiff.Text = "Differ";
            }

            if (textBoxAmountLeft.Text == textBoxAmountRight.Text)
            {
                Color Black = Color.Black;
                labelMoney.ForeColor = Black;
                labelMoney.Text = "Same";
            }
            else
            {
                Color Red = Color.Red;
                labelMoney.ForeColor = Red;
                labelMoney.Text = "Differ";
            }       

        }
// MAKE Selection and Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (comboBoxRight.Text == "VisaAuthorPool")
            {
                if (checkBoxCardNumber.Checked == true)
                {
                    //MatchingString = "CardNumber = '" + textBoxCardNoLeft.Text + "'" ;

                    SearchingStringRight = "Operator ='" + WOperator + "'"
                                  + " AND OpenRecord = 1 AND RemainsForMatching = 1  AND CardNumber = '" + textBoxCardNoLeft.Text + "'";
                }
                if (checkBoxCardNumber.Checked == true & checkBoxAccNo.Checked == true)
                {
                    SearchingStringRight = "Operator ='" + WOperator + "'"
                                  + " AND OpenRecord = 1 AND RemainsForMatching = 1  AND CardNumber = '" + textBoxCardNoLeft.Text + "'" + " AND " + "AccNumber = " + textBoxAccNoLeft.Text;
                    //MatchingString = "CardNumber = " + textBoxCardNoLeft.Text + " AND " + "AccNumber = " + textBoxAccNoLeft.Text;
                }
            }

            if (comboBoxRight.Text == "UnMatched")
            {
                if (checkBoxCardNumber.Checked == true)
                {
                    //MatchingString = "CardNumber = '" + textBoxCardNoLeft.Text + "'" ;

                    SearchingStringRight = "Operator ='" + WOperator + "'"
                                  + " AND OpenRecord = 1 AND CardNumber = '" + textBoxCardNoLeft.Text + "'";
                }
                if (checkBoxCardNumber.Checked == true & checkBoxAccNo.Checked == true)
                {
                    SearchingStringRight = "Operator ='" + WOperator + "'"
                                  + " AND OpenRecord = 1 AND CardNumber = '" + textBoxCardNoLeft.Text + "'" + " AND " + "AccNumber = " + textBoxAccNoLeft.Text;
                    //MatchingString = "CardNumber = " + textBoxCardNoLeft.Text + " AND " + "AccNumber = " + textBoxAccNoLeft.Text;
                }
            }

            Rm.ReadMatchedORUnMatchedFileTableRight(WOperator, WhatFileRight, SearchingStringRight);
            //Rm.ReadRMUnMatchedVisaAuthorMatchingStringRight(WFileRight, WRMCategoryId, Matched, MatchingString);

            ShowGridRight(); 

        }

        // Show Grid Left 
        public void ShowGridLeft()
        {
            //WTotalLeft = 0 ;
    
            //Keyboard.Focus(dataGridView1);

            dataGridView1.DataSource = Rm.RMDataTableLeft.DefaultView;

            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            dataGridView1.Columns[1].Width = 40; // Done
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[2].Width = 40; //  Terminal Id
            dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Columns[3].Width = 50; // DEscr
            
            dataGridView1.Columns[4].Width = 100; // Card No
            //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

            dataGridView1.Columns[5].Width = 60; // Account no

            dataGridView1.Columns[6].Width = 30; // Currency 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 70; // Amount
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //RMDataTableLeft.Columns.Add("SeqNo", typeof(int));
            //RMDataTableLeft.Columns.Add("Select", typeof(bool));
            //RMDataTableLeft.Columns.Add("ATMNo", typeof(string));
            //RMDataTableLeft.Columns.Add("Descr", typeof(string));
            //RMDataTableLeft.Columns.Add("Card", typeof(string));
            //RMDataTableLeft.Columns.Add("Account", typeof(string));
            //RMDataTableLeft.Columns.Add("Curr", typeof(string));
            //RMDataTableLeft.Columns.Add("Amount", typeof(decimal));
            //RMDataTableLeft.Columns.Add("Date", typeof(DateTime));
            //RMDataTableLeft.Columns.Add("RRNumber", typeof(int));


        }

        // Show Grid Right 
        public void ShowGridRight()
        {
            //WTotalRight = 0;

            dataGridView2.DataSource = Rm.RMDataTableRight.DefaultView;

            dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridView2.Columns[0].Width = 40; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 40; // Select
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 50; // DEscr 

            dataGridView2.Columns[3].Width = 110; // Card no

            dataGridView2.Columns[4].Width = 70; // Account

            dataGridView2.Columns[5].Width = 40; // curre

            dataGridView2.Columns[6].Width = 70; // amount

            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("No  transactions available for this selection. ");

                panel7.Hide();
                textBox14.Hide();
                //this.Dispose();
                return;
            }
            else
            {
                //panel7.Show();
                //textBox14.Show();
            }
        }
// Force Matching 
        private void buttonForceMatching_Click(object sender, EventArgs e)
        {
            if (labelNumberNotes2.Text == "0")
            {
                MessageBox.Show("Please Enter Note");
                return;
            }

            if (WCardNumberLeft != WCardNumberRight || WAccNumberLeft != WAccNumberRight || WTransCurrLeft != WTransCurrRight)
            {
                MessageBox.Show("Not Allow Card No or Account or Currency to be different");
                return;
            }

            if (MessageBox.Show("Warning: Do you want to match by exception this unmatched? ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                               == DialogResult.Yes)
            {
                //// Check Input
                //if (comboBoxReason.Text == "Not Defined")
                //{
                //    MessageBox.Show("Please specify reason of force matching");
                //    return;
                //}

                // Update
                // Matched, date of matched and Reconcile record
                // GET AUTHORISATION 
                Form19c NForm19c;

                if (WTransAmountLeft - WTransAmountRight > 0)
                {
                    WAmount = WTransAmountLeft - WTransAmountRight;

                    WTransType = "DR";

                    ForceMatchingCommnets = "An UnMatched settlement transaction with Id = " + WMaskRecordIdLeft + " Will be Forced Matched with an Authorisation transaction Id = " + WMaskRecordIdRight + Environment.NewLine
                                                   + " The settlement amount is : " + WTransAmountLeft.ToString("#,##0.00") + " Whereas the Authorisation Amount was : " + WTransAmountRight.ToString("#,##0.00") + Environment.NewLine
                                                   + " The difference in amount is = " + WAmount.ToString("#,##0.00") + Environment.NewLine
                                                   + " The Customer Account will be Debited with this amount" + Environment.NewLine
                                                   + " The GL Account For EWB511 RM Category will be Credited";
                }
                else
                {
                    WAmount = -(WTransAmountLeft - WTransAmountRight);


                    WTransType = "CR";

                    ForceMatchingCommnets = "An UnMatched settlement transaction with Id = " + WMaskRecordIdLeft + " Will be Forced Matched with an Authorisation transaction Id = " + WMaskRecordIdRight + Environment.NewLine
                                                   + " The settlement amount is : " + WTransAmountLeft.ToString("#,##0.00") + " Whereas the Authorisation Amount was : " + WTransAmountRight.ToString("#,##0.00") + Environment.NewLine
                                                   + " The difference in amount is = " + WAmount.ToString("#,##0.00") + Environment.NewLine
                                                   + " The Customer Account will be Credited with this amount" + Environment.NewLine
                                                   + " The GL Account For EWB511 RM Category will be Debited";
                }

                RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
                if (WAmount != 0)
                {
                    if (WTransType == "DR") WTransTypeInt = 11;
                    if (WTransType == "CR") WTransTypeInt = 21;

                    Ec.CreateTransTobepostedfromForceSettlement(WSignedId, WhatFileLeft, WSeqNoLeft, WTransTypeInt, WAmount);
                }

                //// Update Step to pass the commnet to the general Authorization screen 
                Us.ReadSignedActivityByKey(WSignRecordNo);
                Us.GeneralUsedComment = ForceMatchingCommnets;
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                NForm19c = new Form19c(WSignedId, WSignRecordNo, WOperator, WRMCategoryId, WRMCycleNo);

                //   NForm109.FormClosed += NForm109_FormClosed;
                NForm19c.ShowDialog();
                // Send Emails for this exception
            }
            else
            {
                return;
            }

            SetScreen(); 
        }
// NOTES FOR RM CYCLE 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Force Matching for" + " Category: " + WRMCategoryId + " Matching SesNo: " + WRMCycleNo;
            string SearchP4 = "";

            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
         
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
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
            WParameter4 = "UniqueRecordId: " + Rm.MaskRecordId;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Rm.MaskRecordId;
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
        private void buttonExpandGrid_Click(object sender, EventArgs e)
        {
            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            string WHeader = "UNMATCHED TRANSACTIONS FOR RM CATEGORY : " + WRMCategoryId;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rm.RMDataTableLeft, WHeader,"Form271b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog(); 
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //dataGridView1.Rows[NForm78b.WSelectedRow].Selected = true;
            if (NForm78b.UniqueIsChosen == 1)
            {
                SearchingStringLeft = "Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                              + " AND OpenRecord = 1 AND RemainsForMatching = 0 AND SeqNo = " + NForm78b.WPostedNo.ToString() + " ";

                Rm.ReadMatchedORUnMatchedFileTableLeft2(WOperator, WhatFileLeft, SearchingStringLeft);

                ShowGridLeft();

            }
        }

// Proceed to action 
        Form14b NForm14b;
        int WRowIndex; 

        private void buttonAction_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            if (radioButtonCreateManual.Checked == true 
                || radioButtonMatched.Checked == true
                       || radioButtonCloseIt.Checked == true)
            {
                MessageBox.Show("Future Functionality!");
                return;
            }

            if (radioButtonCreateDefault.Checked == false 
                & radioButtonCreateManual.Checked == false
                    & radioButtonMatched.Checked == false
                         & radioButtonPostpone.Checked == false
                            & radioButtonCloseIt.Checked == false)
            {
                MessageBox.Show("Make Selection Please!");
                return; 
            }

            
            if (radioButtonCreateDefault.Checked == true)
            {
                // Check Radio and act 
               
                // Meta Default
                if (radioButtonCreateDefault.Checked == true)
                {
                    RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
                    Tc.ReadInPoolTransSpecific(Rm.SeqNo);

                    Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFileLeft, WSeqNoLeft);
                    NForm14b = new Form14b(WSignedId, WOperator, WRMCategoryId, WRMCycleNo, Rm.MaskRecordId, Rm.TerminalId,
                                                                         Tc.SesNo, Rm.SeqNo, Rm.TransAmount, Rm.TransCurr, Rm.MetaExceptionId);
                    NForm14b.FormClosed += NForm14b_FormClosed;
                    NForm14b.ShowDialog();
                }
            }

            if (radioButtonPostpone.Checked == true)
            {
                Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFileLeft, WSeqNoLeft);

                Rm.ActionByUser = true;
                Rm.UserId = WSignedId;

                Rm.ActionType = "4";

                Rm.UpdateMatchedORUnMatchedRecordFooter(WOperator, WhatFileLeft, WSeqNoLeft);

                // Update RM Cycle
                Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycleNo);

                Rms.SettledUnMatched = Rms.SettledUnMatched + 1;

                Rms.UpdateCategRMCycleWithAuthorClosing(WRMCategoryId, WRMCycleNo); 

                MessageBox.Show("Action of UnMatched Record with SeqNo : " + WSeqNoLeft.ToString() + " Has been postponed" );

                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
               

                //     button1.TabIndex = 1;
                //button1.TabStop = true;
            }
        }

        void NForm14b_FormClosed(object sender, FormClosedEventArgs e)
        { 
            // UPDATE TRANSACTION 
            if (NForm14b.MetaNumber > 0)
            {
                Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFileLeft, WSeqNoLeft); 

                Rm.MetaExceptionNo = NForm14b.MetaNumber;
                Rm.ActionByUser = true;
                Rm.UserId = WSignedId;

                Rm.ActionType = "1";

                Rm.UpdateMatchedORUnMatchedRecordFooter(WOperator, WhatFileLeft, WSeqNoLeft);
                // Update RM Cycle
                Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycleNo);

                Rms.SettledUnMatched = Rms.SettledUnMatched + 1;

                Rms.UpdateCategRMCycleWithAuthorClosing(WRMCategoryId, WRMCycleNo); 

            }
           
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            SetScreen();

            //dataGridView1.Focus();

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
            //dataGridView1.TabIndex = 1;
            //dataGridView1.TabStop = true;
            //this.ActiveControl = dataGridView1;

            //dataGridView1.Focusable = true;
            //Keyboard.Focus(dataGridView1);

        }


// UNDO ACTION 
        private void buttonUndoAction_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFileLeft, WSeqNoLeft);

            // UPDATE TRANSACTION 
            if (Rm.MetaExceptionNo > 0)
            {
                Er.DeleteErrorRecordByErrNo(Rm.MetaExceptionNo);

                Rm.MetaExceptionNo = 0 ;
                Rm.ActionByUser = false;
                Rm.UserId = WSignedId;
                Rm.ActionType = "0";

                Rm.UpdateMatchedORUnMatchedRecordFooter(WOperator, WhatFileLeft, WSeqNoLeft);

                // Update RM Cycle
                Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycleNo);

                Rms.SettledUnMatched = Rms.SettledUnMatched - 1;

                Rms.UpdateCategRMCycleWithAuthorClosing(WRMCategoryId, WRMCycleNo); 

            }

            if (Rm.ActionType == "4") // This is the postponed case 
            {
                Rm.MetaExceptionNo = 0;
                Rm.ActionByUser = false;
                Rm.UserId = WSignedId;
                Rm.ActionType = "0";

                Rm.UpdateMatchedORUnMatchedRecordFooter(WOperator, WhatFileLeft, WSeqNoLeft);

                // Update RM Cycle
                Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycleNo);

                Rms.SettledUnMatched = Rms.SettledUnMatched - 1;

                Rms.UpdateCategRMCycleWithAuthorClosing(WRMCategoryId, WRMCycleNo); 
            }
              
            
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            SetScreen();

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
        

// Show Part of Journal 
        private void buttonJournal_Click(object sender, EventArgs e)
        {
            Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFileLeft, WSeqNoLeft);

            if (Rm.TerminalId == "AB102")
            {
                MessageBox.Show ("No Journal for the ATM AB102. Try AB104");
                return; 
            }
            Form67 NForm67; 
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 1; // Specific

            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, Rm.TerminalId, Rm.AtmTraceNo, Rm.AtmTraceNo, Mode);
            NForm67.ShowDialog();
        }
// EXPAND GRID RIGHT 
        private void buttonExpandGridRight_Click(object sender, EventArgs e)
        {
            
            string WHeader = "MATCHED TRANSACTIONS FOR RM CATEGORY : " + WRMCategoryId;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rm.RMDataTableRight, WHeader,"Form271b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog(); 

        }
// TRANSACTION TRAIL FOR THE LEFT
        private void button1_Click(object sender, EventArgs e)
        {
            RRDMReconcMaskRecordsLocation Rj = new RRDMReconcMaskRecordsLocation();
            //TEST
            Rj.ReadtblReconcMaskSpecific(WMaskRecordIdLeft, WOperator, Rm.RMCateg, Rm.RMCycle);

            if (Rj.RecordFound == true)
            {
                string WHeader = "TRAIL FOR TRANSACTION WITH ID : " + WMaskRecordIdLeft.ToString();

                NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rj.JsonSelectedTable, WHeader,"UCForm271b");
                NForm78b.FormClosed += NForm78b_FormClosed;
                NForm78b.ShowDialog();

            }
            else
            {
                MessageBox.Show("No Information found for this transaction. Go to EWB311");
                return;
            }

        }
// TRANSANCTION TRAIL FOR THE RIGHT
        private void button2_Click(object sender, EventArgs e)
        {
            RRDMReconcMaskRecordsLocation Rj = new RRDMReconcMaskRecordsLocation();
            //TEST
         
            Rj.ReadtblReconcMaskSpecific(WMaskRecordIdRight, WOperator, Rm.RMCateg, Rm.RMCycle);

            if (Rj.RecordFound == true)
            {
                string WHeader = "TRAIL FOR TRANSACTION WITH ID : " + WMaskRecordIdRight.ToString();

                NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rj.JsonSelectedTable, WHeader, "Form271b");
                NForm78b.FormClosed += NForm78b_FormClosed;
                NForm78b.ShowDialog();

            }
            else
            {
                MessageBox.Show("No Information found for this transaction");
                return; 
            }

        }

    }
}

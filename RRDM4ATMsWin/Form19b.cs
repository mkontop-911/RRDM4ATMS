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
    public partial class Form19b : Form
    {

        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();
        //RRDMReconcFilesClassTEST Rf = new RRDMReconcFilesClassTEST();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

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

        int WUniqueRecordIdLeft;
        int WUniqueRecordIdRight;

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

        public Form19b(string InSignedId, int SignRecordNo, string InOperator, string InRMCategoryId, int InRMCycle, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WRMCategoryId = InRMCategoryId;
            WRMCycleNo = InRMCycle; 
            WOperator = InOperator;

            WMainCateg = WRMCategoryId.Substring(0, 4);

            WAction = InAction;  // 1 = Matching Actions 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            labelUserId.Text = InSignedId;

            labelStep1.Text = "Invstigation and Force Matching for RM Category Id : " + WRMCategoryId;

            if (WMainCateg == "EWB5")
            {
                textBoxHeaderLeft.Text = "VISA SETTLEMENT TRANSACTIONS";
                textBoxHeaderRight.Text = "VISA AUTHORISATION POOL";
            }
            else
            {
                textBoxHeaderLeft.Text = "TRANSACTIONS ON THE LEFT";
                textBoxHeaderRight.Text = "TRANSACTIONS ON THE RIGHT";
            }
            comboBoxLeft.Items.Add("UnMatched");
            comboBoxLeft.Items.Add("Matched");
            comboBoxLeft.Text = "UnMatched";

            comboBoxRight.Items.Add("VisaAuthorPool");
            comboBoxRight.Items.Add("UnMatched");
            

            comboBoxReason.Items.Add("See attached");
            comboBoxReason.Items.Add("Small difference");
            comboBoxReason.Text = "See attached";

            if (WMainCateg == "EWB5")
            {
                textBoxHeaderLeft.Text = "VISA SETTLEMENT TRANSACTIONS";
                textBoxHeaderRight.Text = "VISA AUTHORISATION POOL";
                comboBoxRight.Text = "VisaAuthorPool";
            }
            else
            {
                textBoxHeaderLeft.Text = "TRANSACTIONS ON THE LEFT";
                textBoxHeaderRight.Text = "TRANSACTIONS ON THE RIGHT";
                comboBoxRight.Text = "UnMatched";
                buttonPrintPool.Hide(); 
            }

            textBoxMsgBoard.Text = "Investigate and force matching if needed. "; 
        }
// Load 
        private void Form19b_Load(object sender, EventArgs e)
        {
            
            try
            {
                if (comboBoxLeft.Text == "UnMatched")
                {
                    WhatFileLeft = "UnMatched";
                    //buttonMovedToUnMatched.Hide();
                }

                if (comboBoxLeft.Text == "Matched")
                {
                    WhatFileLeft = "Matched";
                    //buttonMoveToMatched.Hide();
                }

                SearchingStringLeft = "Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                              + " AND OpenRecord = 1 AND RemainsForMatching = 0 " ;


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
                                  + " AND OpenRecord = 1 AND RemainsForMatching = 0 ";
                    
                }           

                Rm.ReadMatchedORUnMatchedFileTableRight(WOperator, WhatFileRight, SearchingStringRight);

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

            WUniqueRecordIdLeft = Rm.MaskRecordId; 

            WCardNumberLeft = Rm.CardNumber;
            WAccNumberLeft = Rm.AccNumber;
            WTransCurrLeft = Rm.TransCurr;
            WTransAmountLeft = Rm.TransAmount;

            textBoxCardNoLeft.Text = WCardNumberLeft;
            textBoxAccNoLeft.Text = WAccNumberLeft;
            textBoxCurrLeft.Text = WTransCurrLeft;
            textBoxAmountLeft.Text = WTransAmountLeft.ToString("#,##0.00");
            textBoxDateLeft.Text = Rm.TransDate.ToString();

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

        }

        // Row Enter for the right  
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNoRight = (int)rowSelected.Cells[0].Value;

            Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFileRight, WSeqNoRight);

            WUniqueRecordIdRight = Rm.MaskRecordId; 

            WCardNumberRight = Rm.CardNumber;
            WAccNumberRight = Rm.AccNumber;
            WTransCurrRight = Rm.TransCurr;
            WTransAmountRight = Rm.TransAmount;

            textBoxCardNoRight.Text = WCardNumberRight;
            textBoxAccNoRight.Text = WAccNumberRight;
            textBoxCurrRight.Text = WTransCurrRight;
            textBoxAmountRight.Text = WTransAmountRight.ToString("#,##0.00");
            textBoxDateRight.Text = Rm.TransDate.ToString();

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

        // Refresh Right Grid with selection 
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

            dataGridView1.DataSource = Rm.RMDataTableLeft.DefaultView;

           

            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // Select
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 50; // DEscr 

            dataGridView1.Columns[3].Width = 110; // Card no
            dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);

            dataGridView1.Columns[4].Width = 70; // Account
            dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

            dataGridView1.Columns[5].Width = 40; // curre

            dataGridView1.Columns[6].Width = 70; // amount

            //RMDataTableRight.Columns.Add("SeqNo", typeof(int));
            //RMDataTableRight.Columns.Add("Select", typeof(bool));
            //RMDataTableRight.Columns.Add("Descr", typeof(string));
            //RMDataTableRight.Columns.Add("Card", typeof(string));
            //RMDataTableRight.Columns.Add("Account", typeof(string));
            //RMDataTableRight.Columns.Add("Curr", typeof(string));
            //RMDataTableRight.Columns.Add("Amount", typeof(decimal));

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
                panel7.Show();
                textBox14.Show();
            }
        }

        //
        // Force Matching 
        // 
        private void buttonForceMatching_Click(object sender, EventArgs e)
        {
            if (labelNumberNotes2.Text == "0" & comboBoxReason.Text == "See attached")
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
                // Check Input
                if (comboBoxReason.Text == "Not Defined")
                {
                    MessageBox.Show("Please specify reason of force matching");
                    return;
                }

                // Update
                // Matched, date of matched and Reconcile record
                // GET AUTHORISATION 
                Form19c NForm19c;

                if (WTransAmountLeft - WTransAmountRight > 0)
                {
                    WAmount = WTransAmountLeft - WTransAmountRight;
 
                    WTransType = "DR" ;

                    ForceMatchingCommnets = "An UnMatched settlement transaction with Id = " + WUniqueRecordIdLeft + " Will be Forced Matched with an Authorisation transaction Id = " + WUniqueRecordIdRight + Environment.NewLine
                                                   + " The settlement amount is : " + WTransAmountLeft.ToString("#,##0.00") + " Whereas the Authorisation Amount was : " +  WTransAmountRight.ToString("#,##0.00") + Environment.NewLine
                                                   + " The difference in amount is = " + WAmount.ToString("#,##0.00") + Environment.NewLine
                                                   + " The Customer Account will be Debited with this amount" + Environment.NewLine
                                                   + " The GL Account For EWB511 RM Category will be Credited" ;
                }
                else
                {
                    WAmount = - (WTransAmountLeft - WTransAmountRight); 


                    WTransType = "CR" ;

                    ForceMatchingCommnets = "An UnMatched settlement transaction with Id = " + WUniqueRecordIdLeft + " Will be Forced Matched with an Authorisation transaction Id = " + WUniqueRecordIdRight + Environment.NewLine
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

                NForm19c = new Form19c(WSignedId, WSignRecordNo, WOperator,WRMCategoryId, WRMCycleNo);

                //   NForm109.FormClosed += NForm109_FormClosed;
                NForm19c.ShowDialog();
                // Send Emails for this exception
            }
            else
            {
                return;
            }

            Form19b_Load(this, new EventArgs());
        }

        private void buttonfinish_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
// On Cell click 
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            

            //int I = 0;

            //while (I <= (Rf.RMDataTableLeft.Rows.Count - 1))
            //{
            //    SelectedLeft = (bool)Rf.RMDataTableLeft.Rows[I]["Select"];
            //    if (SelectedLeft == true)
            //    {
            //        WTotalLeft = WTotalLeft + Rf.TransAmount;
            //    }

            //    I++;
            //}
            //WTotalLeft = 0;

            //for (int rows = 0; rows < dataGridView1.Rows.Count - 1; rows++)
            //{
            //    WSeqNo = (int)dataGridView1.Rows[rows].Cells["SeqNo"].Value;
            //    SelectedLeft = (bool)dataGridView1.Rows[rows].Cells["Select"].Value;

            //    if (SelectedLeft == true)
            //    {
            //        Rf.ReadRMFileSpecificBySeqNo(WFileRight, WRMCategoryId, WRMCycleNo, WSeqNo);

            //        WTotalLeft = WTotalLeft + Rf.TransAmount;
            //    }
            //}

            //textBox1.Text = WTotalLeft.ToString(); 
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //WTotalLeft = 0;

            //for (int rows = 0; rows < dataGridView1.Rows.Count - 1; rows++)
            //{
            //    WSeqNoLeft = (int)dataGridView1.Rows[rows].Cells["SeqNo"].Value;
            //    SelectedLeft = (bool)dataGridView1.Rows[rows].Cells["Select"].Value;

            //    if (SelectedLeft == true)
            //    {
            //        Rm.ReadRMFileSpecificBySeqNo(WFileLeft, WRMCategoryId, WRMCycleNo, WSeqNoLeft);

            //        WTotalLeft = WTotalLeft + Rm.TransAmount;
            //    }
            //}

            //textBox1.Text = WTotalLeft.ToString(); 
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
        //     WTotalLeft = 0;

        //    for (int rows = 0; rows < dataGridView1.Rows.Count - 1; rows++)
        //    {
        //        WSeqNoLeft = (int)dataGridView1.Rows[rows].Cells["SeqNo"].Value;
        //        SelectedLeft = (bool)dataGridView1.Rows[rows].Cells["Select"].Value;

        //        if (SelectedLeft == true)
        //        {
        //            Rm.ReadRMFileSpecificBySeqNo(WFileLeft, WRMCategoryId, WRMCycleNo, WSeqNoLeft);

        //            WTotalLeft = WTotalLeft + Rm.TransAmount;
        //        }
        //    }

        //    textBox1.Text = WTotalLeft.ToString();
        }
// Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Force Matching for" + " Category: " + WRMCategoryId + " Matching SesNo: " + WRMCycleNo;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
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
// Notes 3 
        private void buttonNotes3_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "UniqueRecordId: " + Rm.MaskRecordId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
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
// Print Pool 
        private void buttonPrintPool_Click(object sender, EventArgs e)
        {
            // Print 

            string WD11;

                WD11 = WOperator;
            
                Form56R_EWB00_VisaAuthPool ReportMatched = new Form56R_EWB00_VisaAuthPool(WD11);
                ReportMatched.Show();

        }
// Expand Grid 
        Form78b NForm78b; 
        private void buttonExpandGrid_Click(object sender, EventArgs e)
        {
            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            string WHeader = "SELECTED TRANSACTIONS";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rm.RMDataTableLeft, WHeader, "Form19b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog(); 
        }
        //bool WRange; 
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


        }

    }


using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class UCForm281b : UserControl
    {

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();
        RRDMMatchingReconcExceptionsInfoITMX Mre = new RRDMMatchingReconcExceptionsInfoITMX();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        //RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        RRDMMatchingMasksVsMetaExceptions Rme = new RRDMMatchingMasksVsMetaExceptions();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        //Form14bITMX NForm14bITMX;

        //   string WUserOperator; 
        //public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool ViewWorkFlow;

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;
        // NOTES END 

        int WMaskRecordId;
       
        string SearchingStringLeft;

        //string WFileName;
        //bool Matched;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WReconcCategoryId;
        int WReconcCycleNo;
        string WOperator;
        string WMainCateg;

        int WAction;

        public void UCForm281bPar(string InSignedId, int SignRecordNo, string InOperator, string InRMCategoryId, int InReconcCycleNo, int InAction)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WReconcCategoryId = InRMCategoryId;
            WReconcCycleNo = InReconcCycleNo;
            WOperator = InOperator;

            WMainCateg = WReconcCategoryId.Substring(0, 4);

            WAction = InAction;  // 1 = Matching Actions 

            InitializeComponent();

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            //labelStep1.Text = "Invstigation and Force Matching for RM Category Id : " + WRMCategoryId;

            // Hide RIGHT PANELS GRID AND INFORMATION 
            //***************************************


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


            if (WOperator == "ITMX")
            {

                textBoxHeaderLeft.Text = "UNMATCHED TRANSACTIONS";
                textBoxHeaderRight.Text = "DETAILS FOR MASK";
            }

        }


        // SHOW SCREEN 

        public void SetScreen()
        {
            try
            {

                if (ViewWorkFlow == true) // View Only 
                {
                    // Include close records because is view only 
                    SearchingStringLeft = " WHERE Operator ='" + WOperator + "' AND ReconcCategoryId ='" + WReconcCategoryId + "' AND ReconcMatched = 0 ";
                }
                else
                {
                    SearchingStringLeft = " WHERE Operator ='" + WOperator + "' AND ReconcCategoryId ='" + WReconcCategoryId + "'"
                              + " AND SettledRecord = 0 AND ReconcMatched = 0 ";

                }

                int Mode = 2; // Show DONE
                Mp.ReadMatchingTxnsMasterPoolAndFillTable(Mode, SearchingStringLeft, "");

                ShowGrid();

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

            WMaskRecordId = (int)rowSelected.Cells[0].Value;

            Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WOperator, WMaskRecordId);

            // NOTES 2 START  
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + WMaskRecordId;
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
            WParameter4 = "Reconc for" + " Category: " + WReconcCategoryId + " Reconc Cycle : " + WReconcCycleNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            // NOTES 3 END

            WMaskRecordId = Mp.MaskRecordId;

            textBox2.Text = Mp.MobileRequestor;
            textBoxAccNoLeft.Text = Mp.AccountRequestor;
            textBoxCurrLeft.Text = Mp.Ccy;
            textBoxAmountLeft.Text = Mp.Amount.ToString("#,##0.00");
            textBoxDateLeft.Text = Mp.ExecutionTxnDtTm.ToString();

            textBox9.Text = Mp.MobileBeneficiary;
            textBox6.Text = Mp.AccountBeneficiary;

            textBox8.Text = Mp.DebitBank;
            textBox10.Text = Mp.CreditBank;

            // Find meta EXCEPTION NUMBER 

            string DrCrMask = Mp.DebitMASK + Mp.CreditMASK;

            Rme.ReadMatchingMaskRecordbyMaskId(WOperator, WReconcCategoryId, DrCrMask, 11);

            string UnMatchedType;
            int MetaExceptionId;

            if (Rme.RecordFound)
            {
                UnMatchedType = Rme.MaskName;
                MetaExceptionId = Rme.MetaExceptionId;
            }
            else
            {
                UnMatchedType = "Not Specified";
                MetaExceptionId = 0;
            }

            //SetScreen and get data that you will need for Mre

            ShowInfoRightAndGetExceptionRecordFields();

            //Create Reconciliation Exception Record

            Mre.ReadMatchingReconcExceptionsInfobyMaskRecordId(WOperator, Mp.MaskRecordId);
            if (Mre.RecordFound == true)
            {
                // Show Action
                //

                if (Mre.ActionTypeId != 0) // This the meta exception no created 
                {
                    label11.Text = "Action Taken";
                    label11.ForeColor = Color.Black;
                    textBoxRecommendation.ForeColor = Color.Black; 

                    buttonAction.Hide();
                    buttonUndoAction.Show();

                    textBox3.Show();
                    textBox3.Text = Mre.ActionTypeDescr;

                    if (Mre.ActionTypeId < 5 )
                    {
                        radioButtonCreateDefault.Checked = true;     
                    }
                    else
                    {
                        if (Mre.ActionTypeId == 5) radioButtonPostponed.Checked = true;
                        if (Mre.ActionTypeId == 6) radioButtonCreateManual.Checked = true;
                    }
                }
                else
                {
                    label11.Text = "Action NOT Taken Yet!";
                    label11.ForeColor = Color.Red;
                    textBoxRecommendation.ForeColor = Color.Red;
                    //label10.Hide();
                    
                    //textBoxMetaNo.Hide();
                
                    textBox3.Hide();

                    buttonAction.Show();
                    buttonUndoAction.Hide();
                }
            }
            else
            {
                // Create Exception Record
                textBox3.Hide();
                Mre.MaskRecordId = Mp.MaskRecordId;
                Mre.ITMXUniqueTxnRef = Mp.ITMXUniqueTxnRef;
                Mre.ReconcCategoryId = Mp.ReconcCategoryId;
                Mre.ReconcCycleNo = Mp.ReconcCycleNo;

                Mre.UnMatchedName = Rme.MaskName;
                Mre.DefaultAction = false; 

                Mre.ExceptionRecomm = Er.CircularDesc;

                Mre.MetaExceptionId = Rme.MetaExceptionId;

                Mre.ActionTypeId = 0 ;
                Mre.ActionTypeDescr = "Not Taken yet";

                Mre.CreatedDtTm = DateTime.Now;
                Mre.Operator = WOperator;

                Mre.InsertMatchingReconcExceptionsInfo();

                label11.Text = "Action NOT Taken Yet!";
                label11.ForeColor = Color.Red;
                //label10.Hide();
              
                //textBoxMetaNo.Hide();
               
                textBox3.Hide();

                buttonAction.Show();
                buttonUndoAction.Hide();

            }   

            //LeftDt = Rm.TransDate.Date;
            if (ViewWorkFlow == true)
            {
                buttonAction.Hide();
                buttonUndoAction.Hide();
            }     
        }

        // Show Grid Left 
        public void ShowGrid()
        {
            //WTotalLeft = 0 ;

            //Keyboard.Focus(dataGridView1);

            dataGridView1.DataSource = Mp.MatchingMasterDataTableITMX.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("You do not have records to display.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            dataGridView1.Columns[0].Width = 40; // MaskRecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // cycle 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[2].Width = 40; //  done 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            

            dataGridView1.Columns[3].Width = 50; // 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 60; // 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 50; // currency 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 80; // amount  
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 70; // 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        string WSubString;
        string WMask;
        //
        // Show info within Panel  Right 
        //
        public void ShowInfoRightAndGetExceptionRecordFields()
        {
            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WReconcCategoryId, WReconcCycleNo);

            string WMaskDrCr = Mp.DebitMASK + Mp.CreditMASK;

            Rme.ReadMatchingMaskRecordbyMaskId(WOperator, WReconcCategoryId, WMaskDrCr, 11);

            textBoxUnMatchedType.Text = Rme.MaskName;

            Er.ReadErrorsIDRecord(Rme.MetaExceptionId, WOperator);

            textBoxRecommendation.Text = Er.CircularDesc;

           
            //****************************************************************************
            //Debit Leg 
            //****************************************************************************

            WMask = Mp.DebitMASK;

            if (Mp.DebitMASK == "" || WMask.Length != 3)
            {
                WMask = "EEE";
            }
            // First Line
            if (Mp.DebitFileId01 != "")
            {
                labelFileA.Show();
                textBox31.Show();

                labelFileA.Text = "File A : " + Mp.DebitFileId01;
                labelFileA.Show();
                WSubString = WMask.Substring(0, 1);
                if (WSubString == "0")
                {
                    textBox31.BackColor = Color.Red;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = "E";
                }
            }
            else
            {
                labelFileA.Hide();
                textBox31.Hide();
            }

            // Second Line 
            if (Mp.DebitFileId02 != "")
            {
                labelFileB.Show();
                textBox32.Show();

                labelFileB.Text = "File B : " + Mp.DebitFileId02;
                labelFileB.Show();
                WSubString = WMask.Substring(1, 1);
                if (WSubString == "0")
                {
                    textBox32.BackColor = Color.Red;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = "E";
                }
            }
            else
            {
                labelFileB.Hide();
                textBox32.Hide();
            }

            // Third Line 
            //
            if (Mp.DebitFileId03 != "")
            {
                labelFileC.Show();
                textBox33.Show();

                labelFileC.Text = "File C : " + Mp.DebitFileId03;
                labelFileC.Show();
                WSubString = WMask.Substring(2, 1);
                if (WSubString == "0")
                {
                    textBox33.BackColor = Color.Red;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = "E";
                }
            }
            else
            {
                labelFileC.Hide();
                textBox33.Hide();
            }
            //****************************************************************************
            //Credit Leg 
            //****************************************************************************

            WMask = Mp.CreditMASK;

            if (Mp.CreditMASK == "" || WMask.Length != 3)
            {
                WMask = "EEE";
            }

            // Forth Line 
            if (Mp.CreditFileId01 != "")
            {
                labelFileD.Show();
                textBox34.Show();

                labelFileD.Text = "File D : " + Mp.CreditFileId01;
                labelFileD.Show();
                WSubString = WMask.Substring(0, 1);
                if (WSubString == "0")
                {
                    textBox34.BackColor = Color.Red;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = "E";
                }
            }
            else
            {
                labelFileD.Hide();
                textBox34.Hide();
            }

            // Fifth Line 
            if (Mp.CreditFileId02 != "")
            {
                labelFileE.Show();
                textBox35.Show();

                labelFileE.Text = "File E : " + Mp.CreditFileId02;
                labelFileE.Show();
                WSubString = WMask.Substring(1, 1);
                if (WSubString == "0")
                {
                    textBox35.BackColor = Color.Red;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = "E";
                }
            }
            else
            {
                labelFileE.Hide();
                textBox35.Hide();
            }
            // sixth Line 
            if (Mp.CreditFileId03 != "")
            {
                labelFileF.Show();
                textBox36.Show();

                labelFileF.Text = "File F : " + Mp.CreditFileId03;
                labelFileF.Show();
                WSubString = WMask.Substring(2, 1);
                if (WSubString == "0")
                {
                    textBox36.BackColor = Color.Red;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = "E";
                }
            }
            else
            {
                labelFileF.Hide();
                textBox36.Hide();
            }
        }
        // Force Matching 
        private void buttonForceMatching_Click(object sender, EventArgs e)
        {
            //if (labelNumberNotes2.Text == "0")
            //{
            //    MessageBox.Show("Please Enter Note");
            //    return;
            //}

            //if (WCardNumberLeft != WCardNumberRight || WAccNumberLeft != WAccNumberRight || WTransCurrLeft != WTransCurrRight)
            //{
            //    MessageBox.Show("Not Allow Card No or Account or Currency to be different");
            //    return;
            //}

            //if (MessageBox.Show("Warning: Do you want to match by exception this unmatched? ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            //                   == DialogResult.Yes)
            //{
            //    //// Check Input
            //    //if (comboBoxReason.Text == "Not Defined")
            //    //{
            //    //    MessageBox.Show("Please specify reason of force matching");
            //    //    return;
            //    //}

            //    // Update
            //    // Matched, date of matched and Reconcile record
            //    // GET AUTHORISATION 
            //    Form19c NForm19c;

            //    if (WTransAmountLeft - WTransAmountRight > 0)
            //    {
            //        WAmount = WTransAmountLeft - WTransAmountRight;

            //        WTransType = "DR";

            //        ForceMatchingCommnets = "An UnMatched settlement transaction with Id = " + WMaskRecordIdLeft + " Will be Forced Matched with an Authorisation transaction Id = " + WMaskRecordIdRight + Environment.NewLine
            //                                       + " The settlement amount is : " + WTransAmountLeft.ToString("#,##0.00") + " Whereas the Authorisation Amount was : " + WTransAmountRight.ToString("#,##0.00") + Environment.NewLine
            //                                       + " The difference in amount is = " + WAmount.ToString("#,##0.00") + Environment.NewLine
            //                                       + " The Customer Account will be Debited with this amount" + Environment.NewLine
            //                                       + " The GL Account For EWB511 RM Category will be Credited";
            //    }
            //    else
            //    {
            //        WAmount = -(WTransAmountLeft - WTransAmountRight);


            //        WTransType = "CR";

            //        ForceMatchingCommnets = "An UnMatched settlement transaction with Id = " + WMaskRecordIdLeft + " Will be Forced Matched with an Authorisation transaction Id = " + WMaskRecordIdRight + Environment.NewLine
            //                                       + " The settlement amount is : " + WTransAmountLeft.ToString("#,##0.00") + " Whereas the Authorisation Amount was : " + WTransAmountRight.ToString("#,##0.00") + Environment.NewLine
            //                                       + " The difference in amount is = " + WAmount.ToString("#,##0.00") + Environment.NewLine
            //                                       + " The Customer Account will be Credited with this amount" + Environment.NewLine
            //                                       + " The GL Account For EWB511 RM Category will be Debited";
            //    }

            //    RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
            //    if (WAmount != 0)
            //    {
            //        if (WTransType == "DR") WTransTypeInt = 11;
            //        if (WTransType == "CR") WTransTypeInt = 21;

            //        Ec.CreateTransTobepostedfromForceSettlement(WSignedId, WhatFileLeft, WSeqNoLeft, WTransTypeInt, WAmount);
            //    }

            //    //// Update Step to pass the commnet to the general Authorization screen 
            //    Us.ReadSignedActivityByKey(WSignRecordNo);
            //    Us.GeneralUsedComment = ForceMatchingCommnets;
            //    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //    NForm19c = new Form19c(WSignedId, WSignRecordNo, WOperator, WReconcCategoryId, WReconcCycleNo);

            //    //   NForm109.FormClosed += NForm109_FormClosed;
            //    NForm19c.ShowDialog();
            //    // Send Emails for this exception
            //}
            //else
            //{
            //    return;
            //}

            //SetScreen(); 
        }
        // NOTES FOR RM CYCLE 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Reconc for" + " Category: " + WReconcCategoryId + " Reconc Cycle : " + WReconcCycleNo;
            string SearchP4 = "";

            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "Reconc for" + " Category: " + WReconcCategoryId + " Reconc Cycle : " + WReconcCycleNo;
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
            WParameter4 = "UniqueRecordId: " + WMaskRecordId;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + WMaskRecordId;
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
            string WHeader = "UNMATCHED TRANSACTIONS FOR RECONCILIATION CATEGORY : " + WReconcCategoryId;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mp.MatchingMasterDataTableITMX, WHeader, "Form271b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //dataGridView1.Rows[NForm78b.WSelectedRow].Selected = true;
            if (NForm78b.UniqueIsChosen == 1)
            {
                SearchingStringLeft = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WReconcCategoryId + "'"
                              + " AND SettledRecord = 0 AND RemainsForMatching = 0 AND SeqNo = " + NForm78b.WMaskRecordId + " ";

                int Mode = 2; // Show DONE
                Mp.ReadMatchingTxnsMasterPoolAndFillTable(Mode, SearchingStringLeft, " ORDER BY SeqNo ASC ");

                ShowGrid();

            }
        }
        //************************************
        // Proceed to action 

        int WRowIndex;

        private void buttonAction_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            if (radioButtonCreateDefault.Checked == false & radioButtonPostponed.Checked == false 
              & radioButtonCreateManual.Checked == false)
            {
                MessageBox.Show("Make Selection Please!");
                return;
            }
            // ACT AS PER SYSTEM 
            if (radioButtonCreateDefault.Checked == true)
            {
                // Update Mask Pool record 
                string SelectionCriteria = " WHERE MaskRecordId =" + WMaskRecordId;
                Mp.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria);

                Mp.ActionTaken = true;
                Mp.Postponed = false;
                Mp.UpdateMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WMaskRecordId);

                Mre.ReadMatchingReconcExceptionsInfobyMaskRecordId(WOperator, WMaskRecordId);
       
                if (Mre.MetaExceptionId == 47 )
                {
                    // Make Dispute for BankA

                    Mre.MetaExceptionNo = 0; // Insert here the Dispute Id
                    Mre.ActionTypeId = 2;
                    Mre.ActionTypeDescr = "Creation Of Dispute for " + Mp.DebitBank;
                }
                if (Mre.MetaExceptionId == 50)
                {
                    // Make Dispute for BankB

                    Mre.MetaExceptionNo = 0; // Insert here the Dispute Id
                    Mre.ActionTypeId = 2;
                    Mre.ActionTypeDescr = "Creation Of Dispute " + Mp.CreditBank;
                }
                if (Mre.MetaExceptionId == 51)
                {
                    // Make Disputes for BankA and BankB
                    Mre.MetaExceptionNo = 0; // Insert here the Dispute Id second id for second dispute 
                    Mre.ActionTypeId = 4;
                    Mre.ActionTypeDescr = "Creation Of Disputes For Both Banks";
                }
                if (Mre.MetaExceptionId == 49)
                {
                    // Make Transactions For ITMX to Bank A and B
                    Mre.MetaExceptionNo = 0; // Insert here the Transaction no
                    Mre.ActionTypeId = 1;
                    Mre.ActionTypeDescr = "Creation of posted transaction ";

                    ////                                                                       //NForm14bITMX = new Form14bITMX(WSignedId, WOperator, WReconcCategoryId, WReconcCycleNo, Mp.MaskRecordId, "",
                    //                                                   0, 0, Mp.Amount, Mp.Ccy, Mre.MetaExceptionId);
                    //NForm14bITMX.FormClosed += NForm14bITMX_FormClosed;
                    //NForm14bITMX.ShowDialog();
                }
                if (Mre.MetaExceptionId == 52)
                {
                    // Make SMS for requestor 
                    Mre.MetaExceptionNo = 0; // Insert SMS seq no 
                    Mre.ActionTypeId = 3;
                    Mre.ActionTypeDescr = "Creation of SMS Transaction ";
                }

                Mre.ActionByUser = true;
                Mre.UserId = WSignedId;

                Mre.UpdateMatchingReconcExceptionsInfo(WOperator, WMaskRecordId);
              

                MessageBox.Show("Action of UnMatched Record with Unique No : " + WMaskRecordId.ToString() + " Has been taken");

            }

            if (radioButtonPostponed.Checked == true)
            {
                // Check that there is accompanied message 

                // How many Notes 
                WParameter4 = "Reconc for" + " Category: " + WReconcCategoryId + " Reconc Cycle : " + WReconcCycleNo;
                string Order = "Ascending";
                string SearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, SearchP4);

                if (Cn.RecordFound == true)
                {
                }

                if (Cn.TotalNotes == 0)
                {
                    MessageBox.Show("You must FILL NOTES to explain this action ",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
     
                }
                else // There is value = something will be reported 
                {
                    // Continue with updating 
                }
                // Update Mask Pool record 
                string SelectionCriteria = " WHERE MaskRecordId =" + WMaskRecordId;
                Mp.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria);

                Mp.ActionTaken = true;
                Mp.Postponed = true;
                Mp.UpdateMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WMaskRecordId);

                //Update extention record

                Mre.ReadMatchingReconcExceptionsInfobyMaskRecordId(WOperator, WMaskRecordId);
               
                Mre.MetaExceptionNo = 0;
                Mre.ActionTypeId = 5;
                Mre.ActionTypeDescr = "Exception is postponed";
                Mre.ActionByUser = true;
                Mre.UserId = WSignedId;

                Mre.UpdateMatchingReconcExceptionsInfo(WOperator, WMaskRecordId);

                MessageBox.Show("Postponmet Action of UnMatched Record with Unique No : " + WMaskRecordId.ToString() + " Has been taken");

            }

            if (radioButtonCreateManual.Checked == true)
            {
                // Check that there is accompanied message 

                // How many Notes 
                WParameter4 = "Reconc for" + " Category: " + WReconcCategoryId + " Reconc Cycle : " + WReconcCycleNo;
                string Order = "Ascending";
                string SearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, SearchP4);

                if (Cn.RecordFound == true)
                {
                }

                if (Cn.TotalNotes == 0)
                {
                    MessageBox.Show("You must FILL NOTES to explain your manual action ",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; 
                    // STEPLEVEL

                    //Us.ReadSignedActivityByKey(WSignRecordNo);

                    //if (Us.ReplStep1_Updated == true)
                    //{
                    //    Us.ReplStep1_Updated = false;

                    //    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    //}
                }
                else // There is value = something will be reported 
                {
// Continue with updating 
                }
                // Update Mask Pool record 
                string SelectionCriteria = " WHERE MaskRecordId =" + WMaskRecordId ; 
                Mp.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria);

                Mp.ActionTaken = true;
                Mp.Postponed = false;
                Mp.UpdateMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WMaskRecordId);

             
                //Update extention record

                Mre.ReadMatchingReconcExceptionsInfobyMaskRecordId(WOperator, WMaskRecordId);

                Mre.MetaExceptionNo = 0;
                Mre.ActionTypeId = 6 ;
                Mre.ActionTypeDescr = "Closed exception by Reconc Officer";
                Mre.ActionByUser = true;
                Mre.UserId = WSignedId; 

                Mre.UpdateMatchingReconcExceptionsInfo(WOperator, WMaskRecordId);

                MessageBox.Show("Manual Action of UnMatched Record with Unique No : " + WMaskRecordId.ToString() + " Has been taken");

            }


            //*****************************************************
            //Set Screen 
            //*****************************************************
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            
        }


        // UNDO ACTION 
        private void buttonUndoAction_Click_1(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Mre.ReadMatchingReconcExceptionsInfobyMaskRecordId(WOperator, Mp.MaskRecordId);

            string SelectionCriteria = " WHERE MaskRecordId =" + WMaskRecordId;
            Mp.ReadMatchingTxnsMasterPoolBySelectionCriteria("");

            Mp.ActionTaken = false;
            Mp.Postponed = false;
            Mp.UpdateMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WMaskRecordId);

            //Update extention record

            Mre.ReadMatchingReconcExceptionsInfobyMaskRecordId(WOperator, WMaskRecordId);

            Mre.MetaExceptionNo = 0;
            Mre.ActionTypeId = 0;
            Mre.ActionTypeDescr = "";
            Mre.ActionByUser = false;
            Mre.UserId = "";

            Mre.UpdateMatchingReconcExceptionsInfo(WOperator, WMaskRecordId);

            // DELETE RECORDS CREATED
            // Such As Posted Transactions
            // Disputes 
            // SMS Etc  

            radioButtonCreateDefault.Checked = false; //
            radioButtonPostponed.Checked = false; // 
            radioButtonCreateManual.Checked = false; //

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            SetScreen();

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }


        // Show Part of Journal 
        private void buttonJournal_Click(object sender, EventArgs e)
        {
            //Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFileLeft, WSeqNoLeft);

            //string TempTermId ;

            //TempTermId = Rm.TerminalId; 

            //if (Rm.TerminalId == "AB102")
            //{
            //    TempTermId = "AB104"; 
            //    //MessageBox.Show ("No Journal for the ATM AB102. Try AB104");
            //    //return; 
            //}
            //Form67 NForm67; 
            //String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            //int Mode = 1; // Specific

            //NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, TempTermId, Rm.AtmTraceNo, Rm.AtmTraceNo, Mode);
            //NForm67.ShowDialog();
        }
       
        // TRANSACTION TRAIL FOR THE LEFT
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To Be Develop. Code is within program");
            return;
            //RRDMReconcMaskRecordsLocation Rj = new RRDMReconcMaskRecordsLocation();
            ////TEST
            //Rj.ReadtblReconcMaskSpecific(WMaskRecordIdLeft, WOperator, Rm.RMCateg, Rm.RMCycle);

            //if (Rj.RecordFound == true)
            //{
            //    string WHeader = "TRAIL FOR TRANSACTION WITH ID : " + WMaskRecordIdLeft.ToString();

            //    NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rj.JsonSelectedTable, WHeader,"UCForm271b");
            //    NForm78b.FormClosed += NForm78b_FormClosed;
            //    NForm78b.ShowDialog();

            //}
            //else
            //{
            //    MessageBox.Show("No Information found for this transaction. Go to EWB311");
            //    return;
            //}

        }
        // TRANSANCTION TRAIL FOR THE RIGHT
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To Be Develop. Code is within program");
            return;

            //RRDMReconcMaskRecordsLocation Rj = new RRDMReconcMaskRecordsLocation();
            ////TEST

            //Rj.ReadtblReconcMaskSpecific(WMaskRecordIdRight, WOperator, Rm.RMCateg, Rm.RMCycle);

            //if (Rj.RecordFound == true)
            //{
            //    string WHeader = "TRAIL FOR TRANSACTION WITH ID : " + WMaskRecordIdRight.ToString();

            //    NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rj.JsonSelectedTable, WHeader, "Form271b");
            //    NForm78b.FormClosed += NForm78b_FormClosed;
            //    NForm78b.ShowDialog();

            //}
            //else
            //{
            //    MessageBox.Show("No Information found for this transaction");
            //    return; 
            //}

        }
        // Printing of transactions 
        private void buttonPrintPool_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To Be Develop report. ");
            return;
        }


    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form291NVMatchingRulesDefinition : Form
    {
        /// <summary>
     
        /// </summary>

        //Form110 NForm110;
        //Form112 NForm112;

        //bool ReconciliationAuthor;
        //string StageDescr;
        //int WAuthorSeqNumber;

        // Working Fields 
        //bool WViewFunction;
        //bool WAuthoriser;
        //bool WRequestor;

        //bool NormalProcess;

        //bool ViewWorkFlow;

        string WModeNotes;

        //int WMode;

        //string WSelectionCriteria;
        //string WSortCriteria;

        int WRow; 

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        int WRowIndexGrid1; 

        RRDMNVRulesForMatchingClass Ru = new RRDMNVRulesForMatchingClass(); 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMAccountsClass Acc = new RRDMAccountsClass(); 

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime WDateOfExternal = new DateTime(2015, 02, 15);
        //
        string WCcy;

        // Balance Tolerance 
        //public string TransactionCode;
        //public bool DRTxn;
        //public bool CRTxn;
        //public bool IsToleranceAmount;
        //public bool IsValueDaysTolerance;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WInternalAccNo; 
        string WExternalBankID;
        string WExternalAccNo;
        int WMode;
        string WTransactionCode;
        string WDRCR;

        public Form291NVMatchingRulesDefinition(string InSignedId, int SignRecordNo, string InOperator,
                                           string InInternalAccNo, string InExternalBankID, string InExternalAccNo, 
                                           int InMode, string InStmtLineTrxCode, string InDRCR) 
        {
           
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WInternalAccNo = InInternalAccNo;
            WExternalBankID = InExternalBankID;
            WExternalAccNo = InExternalAccNo;

            WMode = InMode;
            // 1 Updating 
            // 2 View Only for Balance Tolerance 
            // 3 View ONLY for Val Dates Tolerance 
            // 9 View all rules 
            WTransactionCode = InStmtLineTrxCode;
            WDRCR = InDRCR;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            comboBoxLike.DataSource = Ru.GetRulesListGroups(WOperator); 
            comboBoxLike.DisplayMember = "DisplayValue";

            Acc.ReadAndFindAccountSpecificForNostroVostro(WExternalAccNo, WExternalBankID);
            WCcy = Acc.CurrNm;

            label6.Text = "EXTERNAL BANK : " + WExternalBankID;
            label21.Text = "EXTERNAL ACC : " + WExternalAccNo;
            label22.Text = "Ccy : " + WCcy; 

            if (WMode >= 2)
            {
                labelStep1.Text = "View Rules For This Pair";
                buttonDelete.Hide();
                buttonAdd.Hide();
                buttonUpdate.Hide();
                buttonSetRules.Hide();
                buttonAlerts.Hide(); 
                comboBoxLike.Hide();
                label7.Hide();  
            }
        }

        // LOAD SCREEN 
      
        private void Form291_Load(object sender, EventArgs e)
        {
            // Balance Tolerance 
            //public string TransactionCode;
            //public bool DRTxn;
            //public bool CRTxn;
            //public bool IsToleranceAmount;
            //public bool IsValueDaysTolerance;
            if (WMode == 1)
            {
                //labelStep1 = ""; 
                SelectionCriteria = "WHERE Operator ='" + WOperator + "' ";
            }

            if (WMode == 2)
            {
                labelStep1.Text = "Rules for Balance Tolerance ";
                if (WDRCR == "DR")
                {
                    SelectionCriteria = "WHERE TransactionCode ='" + WTransactionCode + "'" 
                                   + " AND DRTxn = 1 AND IsToleranceAmount = 1";
                }
                if (WDRCR == "CR")
                {
                    SelectionCriteria = "WHERE TransactionCode ='" + WTransactionCode + "'"
                                   + " AND CRTxn = 1 AND IsToleranceAmount = 1";
                }
            }


            if (WMode == 3)
            {
                labelStep1.Text = "Rules for Val Dates Tolerance ";
                if (WDRCR == "DR")
                {
                    SelectionCriteria = "WHERE TransactionCode ='" + WTransactionCode + "'"
                                   + " AND DRTxn = 1 AND IsValueDaysTolerance = 1";
                }
                if (WDRCR == "CR")
                {
                    SelectionCriteria = "WHERE TransactionCode ='" + WTransactionCode + "'"
                                   + " AND CRTxn = 1 AND IsValueDaysTolerance = 1";
                }
            }

            if (WMode == 9)
            {
                labelStep1.Text = "View All Rules For this pair";
                SelectionCriteria = "WHERE Operator ='" + WOperator + "' ";
            }

            Ru.ReadRulesBtExternalBankAndAccountNo(WOperator, WSignedId,
                                              WExternalBankID, WExternalAccNo, SelectionCriteria); 
        
            //Show Table
            ShowGrid();

        }

        int WSeqNo;
        string SelectionCriteria;
 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
         
            WSeqNo = (int)rowSelected.Cells[0].Value;
          
            // Find details of this LINE 
            SelectionCriteria = " WHERE SeqNo =" + WSeqNo ;

            Ru.ReadRuleBySelectionCriteria(SelectionCriteria); 

            SetScreen(); 

        }

        //UNDO Default 
        //int WRowIndex;


        //*************************************
        // Set Screen
        //*************************************
        public void SetScreen()
        {
            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);   

            // Initialise

            textBox9.Text = "0";
            textBox5.Text = "0";
            textBox10.Text = "0";

            textBox4.Text = Ru.TransactionCode;

            if (Ru.DRTxn == true)
            {
                checkBoxDR.Checked = true;
            }
            else
            {
                checkBoxDR.Checked = false;
            }

            if (Ru.CRTxn == true)
            {
                checkBoxCR.Checked = true;
            }
            else
            {
                checkBoxCR.Checked = false;
            }

            //
            // Reference
            //
            if (Ru.IsRefIdentical == true)
            {
                radioButtonRefIdentical.Checked = true;
            }
            if (Ru.IsRefPartial == true)
            {
                radioButtonRefPartial.Checked = true;
            }

            textBox6.Text = Ru.NoOfCharactersPartial.ToString();

            //
            // AMOUNT
            //
            if (Ru.IsIdenticalAmount == true)
            {
                radioButtonAmtIdentical.Checked = true;
            }

            //
            if (Ru.IsToleranceAmount == true)
            {
                radioButtonAmtTolerance.Checked = true;
            }

            //
            if (Ru.IsTolerancePercentage == true)
            {
                radioButtonPerc.Checked = true;
            }
            // 
            if (Ru.IsToleranceFixed == true)
            {
                radioButtonFixed.Checked = true;
            }

            textBox9.Text = Ru.TolerancePerc.ToString();
            textBox5.Text = Ru.ToleranceAmtFrom.ToString();
            textBox10.Text = Ru.ToleranceAmtTo.ToString();

            //
            // Vlue date 
            //
            if (Ru.IsValueDaysIdentical == true)
            {
                radioButtonValIdentical.Checked = true;
            }
            if (Ru.IsValueDaysTolerance == true)
            {
                radioButtonValWithToler.Checked = true;
            }

            textBox7.Text = Ru.ValueDaysBefore.ToString();
            textBox8.Text = Ru.ValueDaysAfter.ToString();

            // Agreggate 
            // 
            if (Ru.IsAgregation == true)
            {
                radioButtonWithAggregate.Checked = true;
            }
            if (Ru.IsAgregation == false)
            {
                radioButtonNoAggregate.Checked = true;
            }

            if (Ru.MatchedType == "SystemDefault")
            {
                radioButtonBySystem.Checked = true;
            }

            if (Ru.MatchedType == "AutoButToBeConfirmed")
            {
                radioButtonToBeConfirmed.Checked = true;
            }
        }
      
        // Show Grid Left 
        public void ShowGrid()
        {
            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Rules on: " + WExternalBankID + WExternalAccNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            dataGridView1.DataSource = Ru.TableRulesByExternalAccount.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No Records For Grid ");
                MessageForm.ShowDialog();

                return;
            }


            //DataGridViewCellStyle style = new DataGridViewCellStyle();
            //style.Format = "N2";

            dataGridView1.Columns[0].Visible = false;
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Visible = false;
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Visible = false;
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Visible = false;
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[4].Width = 40;  // TxnType 
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[5].Width = 80; //  ValueDate
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[6].Width = 80; // EntryDate
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[7].Width = 40; // DR/CR
            //dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[8].Width = 100; // Amt
            //dataGridView1.Columns[8].DefaultCellStyle = style;
            //dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            //dataGridView1.Columns[9].Width = 130; // OurRef
            //dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            //dataGridView1.Columns[10].Width = 95; // TheirRef
            //dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[11].Width = 95; // OtherDetails
            //dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // NOTES 
        private void buttonNotes2_Click_1(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Rules on: " + WExternalBankID + WExternalAccNo;
            string SearchP4 = "";
            WModeNotes = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WModeNotes, SearchP4);
            NForm197.ShowDialog();
          
        }
    
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// Print 
        private void button1_Click(object sender, EventArgs e)
        {
            //RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            //Usi.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

            string P1 = "REPORT OF MATCHED TRANSACTIONS";
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator; 
            string P5 = WSignedId;

            Form56R64NOSTRO ReportNOSTRO64 = new Form56R64NOSTRO(P1, P2, P3, P4, P5, "","");
            ReportNOSTRO64.Show();
        }
// DELETE 
       
// Set Rules 
        private void buttonSetRules_Click(object sender, EventArgs e)
        {
            // GET RULES FROM MODEL RULES 
            string s = comboBoxLike.Text;
            string[] values = s.Split(',');

            string ModelBank =  values[1]; 
            string ModelAcc = values[2];

            // Read if any other previous rules for Working Bank And Account 
          
            string WSelectionCriteria = 
                " WHERE ExternalBankID='"+ WExternalBankID + "' AND ExternalAccNo ='" + WExternalAccNo +"'"; 
            Ru.ReadRuleBySelectionCriteria(WSelectionCriteria); 

            if (Ru.TotalSelected > 0)
            {
                
                if (MessageBox.Show("Warning: There are previous : " + Ru.TotalSelected.ToString() + " Rules  Do you want to proceed ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                {
                    // YES 
                    Ru.DeleteRuleEntriesforBankAcc(WExternalBankID, WExternalAccNo); 
                }
                else
                {
                    // NO 
                    return;
                }
            }

            // Copy From Model to WExternalBank And WExternalAcc
            WSelectionCriteria =
               " WHERE ExternalBankID='" + ModelBank + "' AND ExternalAccNo ='" + ModelAcc + "'";
            Ru.ReadRuleBySelectionCriteriaAndInsert(WSelectionCriteria, WExternalBankID, WExternalAccNo, WCcy);

            MessageBox.Show("Number of records inserted :" + Ru.TotalSelected); 

            Form291_Load(this, new EventArgs());
        }
// Add
        private void buttonAdd_Click(object sender, EventArgs e)
        {
          
            // Validation 

            if (textBox4.Text== "")
            {
                MessageBox.Show("Please fill transaction type");
                return; 
            };

            // Initialise

            Ru.ExternalBankID = WExternalBankID;
            Ru.ExternalAccNo = WExternalAccNo;
            Ru.Ccy = WCcy;

            Ru.TransactionCode = textBox4.Text;

            // Validation 

            // DRCR 
            if (checkBoxDR.Checked == false & checkBoxCR.Checked == false)
            {
                MessageBox.Show("Please Select DR or CR");
                return;
            }

            if (checkBoxDR.Checked == true)
            {
                Ru.DRTxn = true;
            }
            else
            {
                Ru.DRTxn = false;
            }
            if (checkBoxCR.Checked == true)
            {
                Ru.CRTxn = true;
            }
            else
            {
                Ru.CRTxn = false;
            }

            int WDr = 0;
            int WCr = 0;

          

            //
            // Reference
            //
            if (radioButtonRefIdentical.Checked == true)
            {
                Ru.IsRefIdentical = true;
                Ru.IsRefPartial = false;
                textBox6.Text = "0";
                Ru.NoOfCharactersPartial = 0;
            }

            if (radioButtonRefPartial.Checked == true)
            {
                Ru.IsRefPartial = true;
                Ru.IsRefIdentical = false;
                if (int.TryParse(textBox6.Text, out Ru.NoOfCharactersPartial))
                {
                }
                else
                {
                    MessageBox.Show(textBox6.Text, "Please enter a valid no for ref");
                    return;
                }
                if (Ru.NoOfCharactersPartial > 0 & Ru.NoOfCharactersPartial <= 16)
                {
                    // Valid
                }
                else
                {
                    MessageBox.Show(textBox6.Text, "Please enter a valid no for ref");
                    return;
                }
            }


            //
            // AMOUNT
            //
            if (radioButtonAmtIdentical.Checked == true)
            {
                Ru.IsIdenticalAmount = true;

                Ru.IsToleranceAmount = false;
                Ru.IsTolerancePercentage = false;
                Ru.IsToleranceFixed = false;
                radioButtonPerc.Checked = false;
                radioButtonFixed.Checked = false;
                textBox9.Text = "0";
                textBox5.Text = "0";
                textBox10.Text = "0";
                Ru.TolerancePerc = 0;
                Ru.ToleranceAmtFrom = 0;
                Ru.ToleranceAmtTo = 0;

            }
            if (radioButtonAmtTolerance.Checked == true)
            {
                Ru.IsToleranceAmount = true;
                Ru.IsIdenticalAmount = false;
            }

            if (radioButtonPerc.Checked == true)
            {
                Ru.IsTolerancePercentage = true;
                Ru.IsToleranceFixed = false;
            }
            if (radioButtonFixed.Checked == true)
            {
                Ru.IsToleranceFixed = true;
                Ru.IsTolerancePercentage = false;
            }
            if (decimal.TryParse(textBox9.Text, out Ru.TolerancePerc))
            {
            }
            else
            {
                MessageBox.Show(textBox9.Text, "Please enter a valid Amt Perc!");
                return;
            }

            if (decimal.TryParse(textBox5.Text, out Ru.ToleranceAmtFrom))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid Amt From!");
                return;
            }

            if (decimal.TryParse(textBox10.Text, out Ru.ToleranceAmtTo))
            {
            }
            else
            {
                MessageBox.Show(textBox10.Text, "Please enter a valid Amt To!");
                return;
            }

            if (radioButtonAmtTolerance.Checked == true)
            {
                if (radioButtonPerc.Checked == false & radioButtonFixed.Checked == false)
                {
                    MessageBox.Show("Please Select Tolerance type");
                    return;
                }
                if (radioButtonPerc.Checked == true & Ru.TolerancePerc == 0)
                {
                    MessageBox.Show("Please Enter Valid Percentage ");
                    return;
                }
                if (radioButtonPerc.Checked == true)
                {
                    if (Ru.ToleranceAmtFrom > 0 || Ru.ToleranceAmtTo > 0)
                    {
                        MessageBox.Show("From and To must be equal to zero");
                        return;
                    }

                }
                if (radioButtonFixed.Checked == true)
                {
                    if ((Ru.ToleranceAmtFrom == 0 & Ru.ToleranceAmtTo == 0)
                        || (Ru.ToleranceAmtTo < Ru.ToleranceAmtFrom)
                        || Ru.TolerancePerc != 0)
                    {
                        MessageBox.Show("Please enter correct values For At Tolerance");
                        return;
                    }
                }

            }
            //
            // Value date 
            //
            if (radioButtonValIdentical.Checked == true)
            {
                Ru.IsValueDaysIdentical = true;
                Ru.IsValueDaysTolerance = false;
                textBox7.Text = "0";
                textBox8.Text = "0";
                Ru.ValueDaysBefore = 0;
                Ru.ValueDaysAfter = 0;
            }
            if (radioButtonValWithToler.Checked == true)
            {
                Ru.IsValueDaysTolerance = true;
                Ru.IsValueDaysIdentical = false;
                if (int.TryParse(textBox7.Text, out Ru.ValueDaysBefore))
                {
                }
                else
                {
                    MessageBox.Show(textBox7.Text, "Please enter a valid before!");
                    return;
                }
                if (int.TryParse(textBox8.Text, out Ru.ValueDaysAfter))
                {
                }
                else
                {
                    MessageBox.Show(textBox8.Text, "Please enter a valid After");
                    return;
                }
                if (Ru.ValueDaysBefore == 0 & Ru.ValueDaysAfter == 0)
                {
                    MessageBox.Show("Please enter a valid values for before and After");
                    return;
                }
            }


            // Agreggate 
            // 
            if (radioButtonWithAggregate.Checked == true)
            {
                Ru.IsAgregation = true;
            }
            if (radioButtonNoAggregate.Checked == true)
            {
                Ru.IsAgregation = false;
            }

            if (radioButtonBySystem.Checked == true)
            {
                Ru.MatchedType = "SystemDefault";
            }

            if (radioButtonToBeConfirmed.Checked == true)
            {
                Ru.MatchedType = "AutoButToBeConfirmed";
            }

            if (radioButtonOnlyManual.Checked == true)
            {
                Ru.MatchedType = "OnlyManual";
            }

            Ru.Operator = WOperator;

            // Check if doesnt exist 

            if (Ru.DRTxn == true) WDr = 1;
            else WDr = 0;

            if (Ru.CRTxn == true) WCr = 1;
            else WCr = 0;
            
            SelectionCriteria = " WHERE ExternalBankID ='" + WExternalBankID + "'"
                             + " AND ExternalAccNo ='" + WExternalAccNo + "'"
                             + " AND Ccy ='" + WCcy + "'"
                             + " AND TransactionType ='" + Ru.TransactionCode + "'"
                             + " AND DRTxn =" + WDr
                             + " AND CRTxn =" + WCr
                             + " AND ToleranceAmtFrom =" + Ru.ToleranceAmtFrom
                             + " AND ToleranceAmtTo =" + Ru.ToleranceAmtTo
                             ;
            Ru.ReadRuleBySelectionCriteria(SelectionCriteria);

            if (Ru.RecordFound == true)
            {
              
                MessageBox.Show("This Record, with the same txn Type and DR or CR already exist.");

                return;
            }

            int ReturnNo = Ru.InsertRule(WExternalBankID);

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form291_Load(this, new EventArgs());

            WRow = dataGridView1.Rows.Count - 1;  
            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
// Update
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            // Read Record
            // Assign updated fields
            // Make updating 
            SelectionCriteria = " WHERE SeqNo =" + WSeqNo;

            Ru.ReadRuleBySelectionCriteria(SelectionCriteria);

            // Assign updated fields 

            // Validation 
            Ru.TransactionCode = textBox4.Text;

            // DRCR 
            if (checkBoxDR.Checked == false & checkBoxCR.Checked == false)
            {
                MessageBox.Show("Please Select DR or CR");
                return;
            }

            if (checkBoxDR.Checked == true)
            {
                Ru.DRTxn = true;
            }
            else
            {
                Ru.DRTxn = false;
            }
            if (checkBoxCR.Checked == true)
            {
                Ru.CRTxn = true;
            }
            else
            {
                Ru.CRTxn = false;
            }
            //
            // Reference
            //
            if (radioButtonRefIdentical.Checked == true)
            {
                Ru.IsRefIdentical = true;
                Ru.IsRefPartial = false;
                textBox6.Text = "0"; 
                Ru.NoOfCharactersPartial = 0;    
            }
           
            if (radioButtonRefPartial.Checked == true)
            {
                Ru.IsRefPartial = true;
                Ru.IsRefIdentical = false;
                if (int.TryParse(textBox6.Text, out Ru.NoOfCharactersPartial))
                {
                }
                else
                {
                    MessageBox.Show(textBox6.Text, "Please enter a valid no for ref");
                    return;
                }
                if (Ru.NoOfCharactersPartial > 0 & Ru.NoOfCharactersPartial <= 16)
                {
                    // Valid
                }
                else
                {
                    MessageBox.Show(textBox6.Text, "Please enter a valid no for ref");
                    return;
                }
            }


            //
            // AMOUNT
            //
            if (radioButtonAmtIdentical.Checked == true)
            {
                Ru.IsIdenticalAmount = true;

                Ru.IsToleranceAmount = false;
                Ru.IsTolerancePercentage = false;
                Ru.IsToleranceFixed = false;
                radioButtonPerc.Checked = false;
                radioButtonFixed.Checked = false; 
                textBox9.Text = "0";
                textBox5.Text = "0";
                textBox10.Text = "0";
                Ru.TolerancePerc = 0;
                Ru.ToleranceAmtFrom = 0;
                Ru.ToleranceAmtTo = 0;

            }
            if (radioButtonAmtTolerance.Checked == true)
            {
                Ru.IsToleranceAmount = true;
            }

            if (radioButtonPerc.Checked == true)
            {
                Ru.IsTolerancePercentage = true;
            }
            if (radioButtonFixed.Checked == true)
            {
                Ru.IsToleranceFixed = true;
            }
            if (decimal.TryParse(textBox9.Text, out Ru.TolerancePerc))
            {
            }
            else
            {
                MessageBox.Show(textBox9.Text, "Please enter a valid Amt Perc!");
                return;
            }

            if (decimal.TryParse(textBox5.Text, out Ru.ToleranceAmtFrom))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid Amt From!");
                return;
            }

            if (decimal.TryParse(textBox10.Text, out Ru.ToleranceAmtTo))
            {
            }
            else
            {
                MessageBox.Show(textBox10.Text, "Please enter a valid Amt To!");
                return;
            }

            if (radioButtonAmtTolerance.Checked == true)
            {
                if (radioButtonPerc.Checked == false & radioButtonFixed.Checked == false)
                {
                    MessageBox.Show("Please Select Tolerance type");
                    return;
                }
                if (radioButtonPerc.Checked == true & Ru.TolerancePerc == 0)
                {
                    MessageBox.Show("Please Enter Valid Percentage ");
                    return;
                }
                if (radioButtonPerc.Checked == true )
                {
                    if (Ru.ToleranceAmtFrom > 0 || Ru.ToleranceAmtTo > 0)
                    {
                        MessageBox.Show("From and To must be equal to zero");
                        return;
                    }
                   
                }
                if (radioButtonFixed.Checked == true)
                {
                    if ((Ru.ToleranceAmtFrom == 0 & Ru.ToleranceAmtTo == 0) 
                        || (Ru.ToleranceAmtTo < Ru.ToleranceAmtFrom) 
                        || Ru.TolerancePerc != 0)
                    {
                        MessageBox.Show("Please enter correct values For At Tolerance");
                        return;
                    }
                }

            }
            //
            // Value date 
            //
            if (radioButtonValIdentical.Checked == true)
            {
                Ru.IsValueDaysIdentical = true;
                Ru.IsValueDaysTolerance = false;
                textBox7.Text = "0";
                textBox8.Text = "0";
                Ru.ValueDaysBefore = 0;
                Ru.ValueDaysAfter = 0;
            }
            if (radioButtonValWithToler.Checked == true)
            {
                Ru.IsValueDaysTolerance = true;
                Ru.IsValueDaysIdentical = false;
                if (int.TryParse(textBox7.Text, out Ru.ValueDaysBefore))
                {
                }
                else
                {
                    MessageBox.Show(textBox7.Text, "Please enter a valid before!");
                    return;
                }
                if (int.TryParse(textBox8.Text, out Ru.ValueDaysAfter))
                {
                }
                else
                {
                    MessageBox.Show(textBox8.Text, "Please enter a valid After");
                    return;
                }
                if (Ru.ValueDaysBefore == 0 & Ru.ValueDaysAfter == 0)
                {
                    MessageBox.Show("Please enter a valid values for before and After");
                    return;
                }
            }


            // Agreggate 
            // 
            if (radioButtonWithAggregate.Checked == true)
            {
                Ru.IsAgregation = true;
            }
            if (radioButtonNoAggregate.Checked == true)
            {
                Ru.IsAgregation = false;
            }
// Matched Type 
            if (radioButtonBySystem.Checked == true)
            {
                Ru.MatchedType = "SystemDefault";
            }

            if (radioButtonToBeConfirmed.Checked == true)
            {
                Ru.MatchedType = "AutoButToBeConfirmed";
            }

            if (radioButtonOnlyManual.Checked == true)
            {
                Ru.MatchedType = "OnlyManual";
            }

            Ru.UpdateRule(WSeqNo);

            WRow = dataGridView1.SelectedRows[0].Index;
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form291_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
          
        }
// Delete 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            WRowIndexGrid1 = dataGridView1.SelectedRows[0].Index;

            if (MessageBox.Show("Warning: Do you want to delete this field ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Ru.DeleteRuleEntry(WSeqNo);

                Form291_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
            if (WRowIndexGrid1 - 1 > 0)
            {
                dataGridView1.Rows[WRowIndexGrid1].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexGrid1));
            }
        }
// Reference Identical
        private void radioButtonRefIdentical_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRefIdentical.Checked == true)
            {
                label14.Hide(); 
                textBox6.Hide();
                textBox6.Text = "0"; 
            }         
        }
// Reference partial 
        private void radioButtonRefPartial_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonRefPartial.Checked == true)
            {
                label14.Show();
                textBox6.Show();
            }
        }
        // Amount Identical 
        private void radioButtonAmtIdentical_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAmtIdentical.Checked == true)
            {
                label12.Hide();
                panel5.Hide();
                radioButtonPerc.Checked = false;
                radioButtonFixed.Checked = false;
                textBox9.Text = "0";
                textBox5.Text = "0";
                textBox10.Text = "0";
            }
        }

// Amount with Tolerance 
        private void radioButtonAmtTolerance_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAmtTolerance.Checked == true)
            {
                label12.Show();
                panel5.Show();
                radioButtonPerc.Checked = false;
                radioButtonFixed.Checked = false;
            }
        }
        // Value date Identical 
        private void radioButtonValIdentical_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonValIdentical.Checked == true)
            {
                label17.Hide();
                label18.Hide();
                textBox7.Hide();
                textBox8.Hide();
                textBox7.Text = "0";
                textBox8.Text = "0";
            }
               
        }
        // Value date tolerance 
        private void radioButtonValWithToler_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonValWithToler.Checked == true)
            {
                label17.Show();
                label18.Show();
                textBox7.Show();
                textBox8.Show();
            }
            
        }
// Go To Alerts 
        private void buttonAlerts_Click(object sender, EventArgs e)
        {
            Form291NVAlertsParam NForm291NVAlertParam;
          
            int RuleMode = 1;
            NForm291NVAlertParam = new Form291NVAlertsParam(WSignedId, WSignRecordNo, WOperator, WInternalAccNo, 
                                          WExternalBankID, WExternalAccNo, RuleMode);
            NForm291NVAlertParam.FormClosed += NForm291NVAlertParam_FormClosed;
            NForm291NVAlertParam.ShowDialog();
        }
// On Close 
        private void NForm291NVAlertParam_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
    }


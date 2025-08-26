using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form502aNostro : Form
    {
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions();

        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMNVDisputes Di = new RRDMNVDisputes();

        RRDMNVDisputesTrans Dt = new RRDMNVDisputesTrans();

        int WSeqNo;
        int WSeqNo2;

        int WRowGrid2;

        string WMatchedType;

        string WUserBankId;

        //int RunningJob;

        string Ccy;
        decimal CcyRate;
        string InternalAcc;
        string ExternalAcc;

        //
        string W4DigitMainCateg;

        string SelectionCriteria;

        int WSeqNo3;

        //string WComboboxText;

        int WRowGrid1;
        //int WRowGrid3;

        //string WCategoryIdAndName;

        int WMode = 6;
        string WReference;

        string WOrderCriteria;

        //= new DateTime(2015, 02, 15);

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public DataTable TableFieldsOrder = new DataTable();

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSubSystem;

        string WCategoryId;
        int WReconcCycleNo;
        DateTime WWorkingDate;

        public Form502aNostro(string InSignedId, int SignRecordNo, string InOperator, string InSubSystem,
                                               string InReconcCategoryId, 
                                               int InReconcCycleNo, DateTime InWorkingDate)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WSubSystem = InSubSystem;
           
            WCategoryId = InReconcCategoryId;
            WReconcCycleNo = InReconcCycleNo;

            WWorkingDate = InWorkingDate;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            Us.ReadUsersRecord(WSignedId);
            WUserBankId = Us.BankId;

            Gp.ParamId = "702"; // LOAD GL ACCOUNTS 

            comboBoxGl.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBoxGl.DisplayMember = "DisplayValue";

            Gp.ParamId = "703"; // LOAD Matched Reasons 
            comboBoxReason.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBoxReason.DisplayMember = "DisplayValue";

            textBoxMsgBoard.Text = "Select Txns To Match. ";

            //Initialise 

            W4DigitMainCateg = WCategoryId.Substring(0, 4);
            //Find Internal and External Account 
            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);
            if (W4DigitMainCateg == "EWB8")
            {
                Ccy = Mc.VostroCurr;
                InternalAcc = Mc.InternalAcc;
                ExternalAcc = Mc.VostroAcc;

                labelPair.Text = "PAIR OF ACCOUNTS: " + Mc.CategoryName;
                textBox11.Text = Ccy;

            }

            Rcs.ReadNVReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);

            CcyRate = Rcs.NostroCcyRate;

            // DELETE ALL ADJUSTMENTS WITH ACTION TYPE = 3 

            Se.DeleteAdjustmentsForInternal(InternalAcc);

            // Turn to 0 Action type for all Internal AND External 
            string WActionType = "00";

            SelectionCriteria = " WHERE Origin = 'EXTERNAL' AND StmtAccountID ='" + ExternalAcc + "'";

            Se.UpdateActionTypeExternal(WOperator, SelectionCriteria, NullPastDate, WActionType, WWorkingDate.AddDays(1));

            SelectionCriteria = " WHERE Origin = 'INTERNAL' AND StmtAccountID ='" + InternalAcc + "'";

            Se.UpdateActionTypeInternal(WOperator, SelectionCriteria, NullPastDate, WActionType, WWorkingDate.AddDays(1));

            TableFieldsOrder = new DataTable();
            TableFieldsOrder.Clear();

            TableFieldsOrder.Columns.Add("SeqNo", typeof(int));
            TableFieldsOrder.Columns.Add("FieldName", typeof(string));
            TableFieldsOrder.Columns.Add("FieldDBName", typeof(string));

            //Home
            WMode = 5;
            WReference = "";
            WOrderCriteria = " Origin ASC ";

            textBoxMsgBoard.Text = "Create next match ";
        }

        private void Form502a_Load(object sender, EventArgs e)
        {
            //WDateOfExternal

            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, WMode, WReconcCycleNo, ExternalAcc, InternalAcc,
                                                                        WOrderCriteria, WWorkingDate, WReference);
            ShowGrid1();

            DateTime NullPastDate = new DateTime(1900, 01, 01);
            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, 4, WReconcCycleNo, ExternalAcc, InternalAcc, "", NullPastDate, "");

            ShowGrid2WithItsDetails(); // Show all with actiontype = 03 

            // Matching Fields (THREE)

            ShowGridMatchingFields();

        }

        string WOrigin;
        string WOrigin2;
        decimal LAmt;
        int WDispNo;

        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;
            WOrigin = (string)rowSelected.Cells[4].Value;
            WDispNo = 0;

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";
            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            LAmt = Se.StmtLineAmt;

            int WColorNo = (int)rowSelected.Cells[1].Value;

            if (WColorNo == 11)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DimGray;
            }
            else if (WColorNo == 12)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            }

            if (Se.IsException == true)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkOrange;
                AlertDetails(); // Show Alert Details 

            }
            else
            {
                textBoxAlert.Hide();
                AlertDetailsIsShown = false;
                if (AlertFound == true) labelAlertFound.Show();
                linkLabelDispute.Hide();
                labelSettledDisp.Hide();
            }

            textBox12.Text = Se.StmtLineRefForAccountOwner;
            textBox14.Text = "Panicos";
        }

        // Alert Details
        bool AlertDetailsIsShown;
        private void AlertDetails()
        {
            
            textBoxAlert.Show();
            AlertDetailsIsShown = true;
            labelAlertFound.Hide();
            textBoxAlert.ForeColor = Color.Red;

            if (Se.ExceptionNo > 0)
            {
                WDispNo = Se.ExceptionNo;
                Di.ReadNVDisputesBySeqNo(WOperator, WDispNo);
                if (Di.Active == false)
                {
                    labelSettledDisp.Show();
                    textBoxAlert.ForeColor = Color.DarkGreen; 

                }
                else labelSettledDisp.Hide();

                linkLabelDispute.Show();
            }
            else
            {
                linkLabelDispute.Hide();
                labelSettledDisp.Hide();
            }

            if (Se.ExceptionId == 12)
            {
                //int DiffInEntryDays = Convert.ToInt32((Se.StmtLineEntryDate - WWorkingDate).TotalDays);
                //int AbsDiffInEntryDays = Math.Abs(DiffInEntryDays);
                if (WDispNo > 0)
                {
                    textBoxAlert.Text = "There Is Alert on Value date for this transaction." + Environment.NewLine
                                       + "A Dispute was opened.";
                }
                else
                {
                    textBoxAlert.Text = "There Is Alert on Value date for this transaction. No dispute is opened yet. ";
                }
            }
            if (Se.ExceptionId == 47)
            {
                textBoxAlert.Text = "Difference In Amt";
            }
            if (Se.ExceptionId == 14)
            {
                textBoxAlert.Text = "Fantom txn";
            }
            if (Se.ExceptionId == 15)
            {
                textBoxAlert.Text = "Transaction omited by teller";
            }
            if (Se.ExceptionId == 88)
            {
                textBoxAlert.Text = "No txn from External Bank (account) yet. ";
            }
            // 01 We appear not to have debited so far
            // 02 We appear not to have credited so far  
            // 06 This transaction does not appear in your statement of account 
            // 47 Difference in amount - open Dispute and sent 995  
            // 12 Difference in Value Date 
            // 14 Fantom txn - open Dispute and sent 995 
            // 15 Transaction omited by teller - Open Dispute and send email to teller 
        }
        //
        // Row enter for Category vs Source files 
        private void dataGridView2_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNo2 = (int)rowSelected.Cells[0].Value;
            WOrigin2 = (string)rowSelected.Cells[4].Value;

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo2 + " AND Origin ='" + WOrigin2 + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            int WColorNo = (int)rowSelected.Cells[1].Value;

            if (WColorNo == 11)
            {
                dataGridView2.DefaultCellStyle.SelectionBackColor = Color.DimGray;
            }
            else if (WColorNo == 12)
            {
                dataGridView2.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            }

            if (Se.IsException == true)
            {
                dataGridView2.DefaultCellStyle.SelectionBackColor = Color.DarkOrange;
            }

            label3.Text = "SELECTED TRANSACTIONS";

            if (Se.Origin == "WAdjustment")
            {
                panel5.Show();
                textBox5.Text = Se.StmtLineAmt.ToString();
                textBox6.Text = Se.StmtLineSuplementaryDetails;
                comboBoxGl.Text = Se.AdjGlAccount;
                comboBoxReason.Text = Se.MatchedType;
                panelValAndRef.Hide();
            }
            else
            {
                panel5.Hide();
                textBox5.Text = "";
                textBox6.Text = "";
                textBox10.Text = "";
                checkBox5.Checked = false;
                panelValAndRef.Show();
            }
        }

        // Row Enter For Matching fields 
        private void dataGridView3_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            WSeqNo3 = (int)rowSelected.Cells[0].Value;

            //Rmf.ReadReconcMatchingFieldsSeqNo(WSeqNo3);
            Utd.ReadUniversalTableFieldsDefinitionSeqNo(WSeqNo);

            WFieldName3 = Utd.FieldName;
            WFieldDBName3 = Utd.FieldDBName;
        }

        // Add 
        private void buttonAdd_Click_1(object sender, EventArgs e)
        {
            WReference = textBox12.Text;

            // Check if old with such reference 
            WMode = 15;
            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, WMode, WReconcCycleNo, ExternalAcc, InternalAcc, WOrderCriteria, WWorkingDate, WReference);
            if (Se.RecordFound == true)
            {
                if (MessageBox.Show("There are old matched with this reference. " + Environment.NewLine
                    + "This entry might be double at External "
                    + "Do want to visit them?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                {
                    Form291NVMatched NForm291NVMatched;
                    int Mode = 2;

                    NForm291NVMatched = new Form291NVMatched(WSignedId, WSignRecordNo, WOperator, WSubSystem ,WCategoryId, WReconcCycleNo, Mode, WReference);
                   
                    NForm291NVMatched.ShowDialog();

                }
                else
                {
                    // Proceed
                }
            }

            string WActionType = "03"; // 


            SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);


            //Se.UpdateActionTypeExternal(WOperator, SelectionCriteria, WWorkingDate);
            SelectionCriteria = "";
            // Find all entries with the same reference and update Se.ActionType = 03 
            // NO SELECTION 
            if (checkBoxSameRef.Checked == false & checkBoxSameAmt.Checked == false & checkBoxSameVal.Checked == false)
            {
                SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

                if (WOrigin == "EXTERNAL")
                {
                    Se.UpdateActionTypeExternal(WOperator, SelectionCriteria,Se.StmtLineValueDate , WActionType, WWorkingDate);
                }
                else
                {
                    Se.UpdateActionTypeInternal(WOperator, SelectionCriteria, Se.StmtLineValueDate, WActionType, WWorkingDate);
                }


                WRowGrid1 = dataGridView1.SelectedRows[0].Index;
                WMode = 5;
                WReference = "";
                WOrderCriteria = " Origin ASC ";
                Form502a_Load(this, new EventArgs()); // Load Grid    

                textBoxMsgBoard.Text = "Comment. ";

                return;          
            }

            // REFERENCE 
            if (checkBoxSameRef.Checked == true & checkBoxSameAmt.Checked == false & checkBoxSameVal.Checked == false )
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "'"
                            + " AND Matched = 0 AND StmtLineRefForAccountOwner ='" + Se.StmtLineRefForAccountOwner + "'";
            }
            // REFERENCE With Amt 
            if (checkBoxSameRef.Checked == true & checkBoxSameAmt.Checked == true & checkBoxSameVal.Checked == false)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "'"
                             + " AND Matched = 0 AND StmtLineRefForAccountOwner ='" + Se.StmtLineRefForAccountOwner + "'"
                             + " AND ABS(StmtLineAmt) =" + Math.Abs(Se.StmtLineAmt);
            }
            // REFERENCE With Amt and Val date
            if (checkBoxSameRef.Checked == true & checkBoxSameAmt.Checked == true & checkBoxSameVal.Checked == true)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "'"
                             + " AND Matched = 0 AND StmtLineRefForAccountOwner ='" + Se.StmtLineRefForAccountOwner + "'"
                             + " AND ABS(StmtLineAmt) =" + Math.Abs(Se.StmtLineAmt)
                             + " AND StmtLineValueDate = @StmtLineValueDate";
            }
            // REFERENCE  and Val date
            if (checkBoxSameRef.Checked == true & checkBoxSameAmt.Checked == false & checkBoxSameVal.Checked == true)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "'"
                             + " AND Matched = 0 AND StmtLineRefForAccountOwner ='" + Se.StmtLineRefForAccountOwner + "'"
                             + " AND StmtLineValueDate = @StmtLineValueDate";
            }
            // AMOUNT 
            if (checkBoxSameAmt.Checked == true & checkBoxSameRef.Checked == false & checkBoxSameVal.Checked == false )
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "'"
                            + " AND Matched = 0 AND ABS(StmtLineAmt) =" + Math.Abs(Se.StmtLineAmt);
            }
            // AMOUNT With Value date 
            if (checkBoxSameAmt.Checked == true & checkBoxSameRef.Checked == false & checkBoxSameVal.Checked == true)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "'"
                            + " AND Matched = 0 AND ABS(StmtLineAmt) =" + Math.Abs(Se.StmtLineAmt)
                              + " AND StmtLineValueDate = @StmtLineValueDate";
            }
            // VALUE DATE 
            if (checkBoxSameVal.Checked == true & checkBoxSameRef.Checked == false & checkBoxSameAmt.Checked == false)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "'"
                            + " AND Matched = 0 AND StmtLineValueDate = @StmtLineValueDate" ;
            }
            

            if (SelectionCriteria == "" & 
                (checkBoxSameRef.Checked == true || checkBoxSameAmt.Checked == true || checkBoxSameVal.Checked == true))
            {
                MessageBox.Show("Your selection doesnt make sense");
                return; 
            }

            Se.UpdateActionTypeExternal(WOperator, SelectionCriteria, Se.StmtLineValueDate, WActionType, WWorkingDate);

            //
            //SEARCH IN INTERNAL AND UPDATE WITH 3 if found 
            //

            // REFERENCE 
            if (checkBoxSameRef.Checked == true & checkBoxSameAmt.Checked == false & checkBoxSameVal.Checked == false)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "'"
                            + " AND Matched = 0 AND StmtLineRefForAccountOwner ='" + Se.StmtLineRefForAccountOwner + "'";
            }
            // REFERENCE With Amt 
            if (checkBoxSameRef.Checked == true & checkBoxSameAmt.Checked == true & checkBoxSameVal.Checked == false)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "'"
                             + " AND Matched = 0 AND StmtLineRefForAccountOwner ='" + Se.StmtLineRefForAccountOwner + "'"
                             + " AND ABS(StmtLineAmt) =" + Math.Abs(Se.StmtLineAmt);
            }
            // REFERENCE With Amt and Val date
            if (checkBoxSameRef.Checked == true & checkBoxSameAmt.Checked == true & checkBoxSameVal.Checked == true)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "'"
                             + " AND Matched = 0 AND StmtLineRefForAccountOwner ='" + Se.StmtLineRefForAccountOwner + "'"
                             + " AND ABS(StmtLineAmt) =" + Math.Abs(Se.StmtLineAmt)
                             + " AND StmtLineValueDate = @StmtLineValueDate";
            }
            // REFERENCE  and Val date
            if (checkBoxSameRef.Checked == true & checkBoxSameAmt.Checked == false & checkBoxSameVal.Checked == true)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "'"
                             + " AND Matched = 0 AND StmtLineRefForAccountOwner ='" + Se.StmtLineRefForAccountOwner + "'"
                             + " AND StmtLineValueDate = @StmtLineValueDate";
            }
            // AMOUNT 
            if (checkBoxSameAmt.Checked == true & checkBoxSameRef.Checked == false & checkBoxSameVal.Checked == false)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "'"
                            + " AND Matched = 0 AND ABS(StmtLineAmt) =" + Math.Abs(Se.StmtLineAmt);
            }
            // AMOUNT With Value date 
            if (checkBoxSameAmt.Checked == true & checkBoxSameRef.Checked == false & checkBoxSameVal.Checked == true)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "'"
                            + " AND Matched = 0 AND ABS(StmtLineAmt) =" + Math.Abs(Se.StmtLineAmt)
                              + " AND StmtLineValueDate = @StmtLineValueDate";
            }
            // VALUE DATE 
            if (checkBoxSameVal.Checked == true & checkBoxSameRef.Checked == false & checkBoxSameAmt.Checked == false)
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "'"
                            + " AND Matched = 0 AND StmtLineValueDate = @StmtLineValueDate";
            }

            Se.UpdateActionTypeInternal(WOperator, SelectionCriteria, Se.StmtLineValueDate, WActionType, WWorkingDate);

            WRowGrid1 = dataGridView1.SelectedRows[0].Index;
            WMode = 5;
            WReference = "";
            WOrderCriteria = " Origin ASC ";
      
            Form502a_Load(this, new EventArgs()); // Load Grid    

            textBoxMsgBoard.Text = "Comment. ";
        }

        // Remove 
        private void buttonRemove_Click_1(object sender, EventArgs e)
        {
            string WActionType = "00"; // 

            if (WOrigin2 == "EXTERNAL")
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + WSeqNo2;

                Se.UpdateActionTypeExternal(WOperator, SelectionCriteria, Se.StmtLineValueDate, WActionType, WWorkingDate);
            }

            if (WOrigin2 == "INTERNAL")
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + WSeqNo2;

                Se.UpdateActionTypeInternal(WOperator, SelectionCriteria, Se.StmtLineValueDate, WActionType, WWorkingDate);
            }

            if (WOrigin2 == "WAdjustment")
            {
                MessageBox.Show("You cannot move adjustment. You can delete it!"); 
            }

            Form502a_Load(this, new EventArgs());

            textBoxMsgBoard.Text = "Comment. ";

        }

        // FINISH
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //**********************************************************************
        // END NOTES 
        //********************************************************************** 
        // Show Grid 1 
        decimal TotalExternal1;
        decimal TotalInternal1;
        decimal TotalAdjust1;
        decimal GTotal1;
        string WOurReference1;
        bool AlertFound;
        public void ShowGrid1()
        {

            dataGridView1.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                //Form2 MessageForm = new Form2("Selection has ended");
                //MessageForm.ShowDialog();
                textBox12.Text = "";
                textBox13.Text = "";
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; // MatchingNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 170; //MatchedType
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = false;

            dataGridView1.Columns[4].Width = 95;  // Origin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 105;  // Acc No 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = false;

            dataGridView1.Columns[6].Width = 40;  // Done 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = false;

            dataGridView1.Columns[7].Width = 40;  // Code 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 115; //  ValueDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[9].Width = 90; // EntryDate
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 40; // DR/CR
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 120; // Amt
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[12].Width = 130; // OurRef
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[12].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[12].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[12].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[13].Width = 100; // TheirRef
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 95; // OtherDetails
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[15].Width = 95; // Ccy
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[15].Visible = true;

            dataGridView1.Columns[16].Width = 95; // CcyRate
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[16].Visible = true;

            dataGridView1.Columns[17].Width = 95; // GLAccount
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[17].Visible = false;

            AlertFound = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int TempSeqNo = (int)row.Cells[0].Value;
                string TempOrigin = row.Cells[4].Value.ToString();

                if (TempOrigin == "EXTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (TempOrigin == "INTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }

                SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

                if (Se.IsException == true)
                {
                    AlertFound = true;
                    //dataGridView1.Columns[4].DefaultCellStyle.BackColor = Color.SaddleBrown;
                    //dataGridView1.Columns[4].DefaultCellStyle.ForeColor = Color.White;
                    row.DefaultCellStyle.BackColor = Color.SaddleBrown;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
            }
            if (AlertFound == true)
            {
                labelAlertFound.Show();
                if (AlertDetailsIsShown == true) labelAlertFound.Hide();
            }
            else
            {
                labelAlertFound.Hide();
                linkLabelDispute.Hide();
                labelSettledDisp.Hide();
            }

            TotalExternal1 = 0;
            TotalInternal1 = 0;
            TotalAdjust1 = 0;
            GTotal1 = 0;
            WOurReference1 = "";

            int K = 0;
            //
            // Totals and Analysis  
            //
            while (K <= (dataGridView1.Rows.Count - 1))
            {
                RSeqNo = (int)dataGridView1.Rows[K].Cells["SeqNo"].Value;
                ROrigin = (string)dataGridView1.Rows[K].Cells["Origin"].Value;

                if (ROrigin == "EXTERNAL")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_ExternalBySelectionCriteria(SelectionCriteria);

                    TotalExternal1 = TotalExternal1 + Se.StmtLineAmt;
                }

                if (ROrigin == "INTERNAL")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    WOurReference1 = Se.StmtLineRefForAccountOwner;

                    TotalInternal1 = TotalInternal1 + Se.StmtLineAmt;
                }

                if (ROrigin == "WAdjustment")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    TotalAdjust1 = TotalAdjust1 + Se.StmtLineAmt;
                }

                K++; // Read Next entry of the table 
            }
            // 
            GTotal1 = TotalExternal1 + TotalInternal1 + TotalAdjust1;
            textBox13.Text = GTotal1.ToString("#,##0.00");

        }
        //******************
        // SHOW GRID dataGridView2
        //******************
        decimal TotalExternal2;
        decimal TotalInternal2;
        decimal TotalAdjust2;
        decimal GTotal2;
        string WExternalReference2;
        string WInternalReference2;
        DateTime WExternalDtTm2;
        DateTime WInternalDtTm2;
        int WExternalCount2;
        int WInternalCount2;
        bool WDisputesOn2; 

        int WRightRows;
        public void ShowGrid2WithItsDetails()
        {

            dataGridView2.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                labelAmt.Hide();
                panel7.Hide();
                labelValRef.Hide();
                panelValAndRef.Hide();
                label28.Hide();
                panel5.Hide();
                checkBoxAdj.Checked = false;
                checkBoxDisputes.Checked = false;
                comboBoxReason.Hide();
                return;
            }
            else
            {
                labelAmt.Show();
                panel7.Show();
                labelValRef.Show();
                panelValAndRef.Show();
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView2.Columns[0].Width = 50; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].Visible = false;

            dataGridView2.Columns[1].Width = 40; // ColorNo
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].Visible = false;

            dataGridView2.Columns[2].Width = 70; // MatchingNo
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[2].Visible = false;

            dataGridView2.Columns[3].Width = 170; //MatchedType
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[3].Visible = false;

            dataGridView2.Columns[4].Width = 85;  // Origin
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[5].Width = 105;  // Acc No 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[5].Visible = false;

            dataGridView2.Columns[6].Width = 40;  // Done 
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[6].Visible = false;

            dataGridView2.Columns[7].Width = 40;  // Code 
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[8].Width = 90; //  ValueDate
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[9].Width = 90; // EntryDate
            dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[10].Width = 40; // DR/CR
            dataGridView2.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[11].Width = 100; // Amt
            dataGridView2.Columns[11].DefaultCellStyle = style;
            dataGridView2.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[12].Width = 130; // OurRef
            dataGridView2.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[12].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[12].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[12].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[13].Width = 100; // TheirRef
            dataGridView2.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[14].Width = 95; // OtherDetails
            dataGridView2.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[15].Width = 95; // Ccy
            dataGridView2.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[15].Visible = true;

            dataGridView2.Columns[16].Width = 95; // CcyRate
            dataGridView2.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[16].Visible = true;

            dataGridView2.Columns[17].Width = 95; // GLAccount
            dataGridView2.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[17].Visible = false;

            // SET LINE Colors
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                int TempSeqNo = (int)row.Cells[0].Value;
                string TempOrigin = row.Cells[4].Value.ToString();

                if (TempOrigin == "EXTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (TempOrigin == "INTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }

                SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

                if (Se.IsException == true)
                {
                    row.DefaultCellStyle.BackColor = Color.SaddleBrown;
                    row.DefaultCellStyle.ForeColor = Color.White;
                }
            }
            //SHOW TOTALS
            TotalExternal2 = 0;
            TotalInternal2 = 0;
            TotalAdjust2 = 0;
            GTotal2 = 0;
            WExternalReference2 = "";
            WInternalReference2 = "";
            WExternalDtTm2 = NullPastDate;
            WInternalDtTm2 = NullPastDate;
            WExternalCount2 = 0;
            WInternalCount2 = 0;
            WDisputesOn2 = false; 

            WRightRows = dataGridView2.Rows.Count;

            int K = 0;
            //
            // Totals and Analysis  
            //
            while (K <= (dataGridView2.Rows.Count - 1))
            {
                RSeqNo = (int)dataGridView2.Rows[K].Cells["SeqNo"].Value;
                ROrigin = (string)dataGridView2.Rows[K].Cells["Origin"].Value;
                RAmt = (decimal)dataGridView2.Rows[K].Cells["Amt"].Value;

                if (ROrigin == "EXTERNAL")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_ExternalBySelectionCriteria(SelectionCriteria);

                    if (Se.ExceptionNo > 0)
                    {
                        WDispNo = Se.ExceptionNo;
                        Di.ReadNVDisputesBySeqNo(WOperator, WDispNo);
                        if (Di.Active == true)
                        {
                            WDisputesOn2 = true;
                        }
                    }
                   
                    WExternalCount2 = WExternalCount2 + 1;

                    if (WExternalCount2 == 1)
                    {
                        // First Record
                        WExternalDtTm2 = Se.StmtLineValueDate;
                    }
                    else
                    {
                        if (Se.StmtLineIsDebit == true)
                        {
                            // Assign the biggest 
                            if (WExternalDtTm2 < Se.StmtLineValueDate)
                            {
                                WExternalDtTm2 = Se.StmtLineValueDate;
                            }
                        }
                        if (Se.StmtLineIsDebit == false)
                        {
                            // Assign the smallest
                            if (WExternalDtTm2 > Se.StmtLineValueDate)
                            {
                                WExternalDtTm2 = Se.StmtLineValueDate;
                            }
                        }
                    }

                    WExternalReference2 = Se.StmtLineRefForAccountOwner;

                    TotalExternal2 = TotalExternal2 + Se.StmtLineAmt;
                }

                if (ROrigin == "INTERNAL")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    if (Se.ExceptionNo > 0)
                    {
                        WDispNo = Se.ExceptionNo;
                        Di.ReadNVDisputesBySeqNo(WOperator, WDispNo);
                        if (Di.Active == true)
                        {
                            WDisputesOn2 = true;
                        }
                    }

                    WInternalCount2 = WInternalCount2 + 1;

                    WInternalDtTm2 = Se.StmtLineValueDate;

                    WInternalReference2 = Se.StmtLineRefForAccountOwner;

                    TotalInternal2 = TotalInternal2 + Se.StmtLineAmt;
                }

                if (ROrigin == "WAdjustment")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    TotalAdjust2 = TotalAdjust2 + Se.StmtLineAmt;
                }

                K++; // Read Next entry of the table 
            }
            // 
            textBox2.Text = TotalExternal2.ToString("#,##0.00");
            textBox4.Text = TotalInternal2.ToString("#,##0.00");
            textBox7.Text = TotalAdjust2.ToString("#,##0.00");
            GTotal2 = TotalExternal2 + TotalInternal2 + TotalAdjust2;
            textBox1.Text = GTotal2.ToString("#,##0.00");

            if (WExternalCount2 > 0 & WInternalCount2 > 0 & checkBoxAdj.Checked == false)
            {
                labelValRef.Show();
                panelValAndRef.Show();

                textBoxExtValDt.Text = WExternalDtTm2.Date.ToString();
                textBoxIntValDt.Text = WInternalDtTm2.Date.ToString();
                textBoxExtRef.Text = WExternalReference2;
                textBoxIntRef.Text = WInternalReference2;

                if (WExternalDtTm2.Date == WInternalDtTm2.Date)
                {
                    labelValDiff.Text = "Same";
                    labelValDiff.ForeColor = Color.Black;
                    linkLabelTolerValRight.Hide();
                }
                else
                {
                    int DiffInEntryDays = Convert.ToInt32((WExternalDtTm2 - WInternalDtTm2).TotalDays);
                    int AbsDiffInEntryDays = Math.Abs(DiffInEntryDays);

                    labelValDiff.Text = "Diff by " + AbsDiffInEntryDays;
                    labelValDiff.ForeColor = Color.Red;
                    linkLabelTolerValRight.Show();
                }
                if (WExternalReference2 == WInternalReference2)
                {
                    labelRefDiff.Text = "Same";
                    labelRefDiff.ForeColor = Color.Black;
                }
                else
                {
                    labelRefDiff.Text = "Diff";
                    labelRefDiff.ForeColor = Color.Red;
                }

            }
            else
            {
                labelValRef.Hide();
                panelValAndRef.Hide();
            }

            if (GTotal2 < 0) GTotal2 = -GTotal2;

            if (GTotal2 > 5)
            {
                pictureBoxBalStatus.BackgroundImage = appResImg.RED_LIGHT_Repl;
            }

            if (GTotal2 < 5)
            {
                pictureBoxBalStatus.BackgroundImage = appResImg.YELLOW_Repl;
            }

            if (GTotal2 == 0)
            {
                pictureBoxBalStatus.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }

            if (GTotal2 != 0)
            {
                checkBoxAdj.Show();
                linkLabelTolerBalRight.Show();

                button3.Hide();

                label28.Hide();
                comboBoxReason.Hide();
            }
            else
            {
                checkBoxAdj.Hide();

                linkLabelTolerBalRight.Hide();

                button3.Show();

                label28.Show();
                comboBoxReason.Show();
                comboBoxReason.Text = "Fill Entry";
            }
        }

        //******************
        // SHOW GRID dataGridView3
        //******************
        private void ShowGridMatchingFields()
        {
            // Keep Scroll position 


            // Matching Fields (THREE)

            //string WFilter3 = "Operator = '" + WOperator + "' AND Application = 'NOSTRO'";

            //Rmf.ReadReconcMatchingFieldsToFillDataTable(WFilter3);

            //dataGridView3.DataSource = Rmf.MatchingFieldsDataTable.DefaultView;

          
            string WTableStructureId = "NOSTRO";
            Utd.ReadUniversalTableMatchingFieldsToFillDataTable(WTableStructureId);

            dataGridView3.DataSource = Utd.MatchingFieldsDataTable.DefaultView;

            if (dataGridView3.Rows.Count == 0)
            {
                return;
            }

            int scrollPosition = 0;
            if (WRowGrid1 > 0)
            {
                scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            }
            dataGridView3.Columns[0].Width = 70; // SeqNo
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[1].Width = 250; // Field Name
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView3.Columns[2].Width = 90; // Field Type
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView3.Rows[WRowGrid3].Selected = true;
            //dataGridView3_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid3));

            //dataGridView3.FirstDisplayedScrollingRowIndex = scrollPosition;

            // DATA TABLE ROWS DEFINITION 
            //MatchingFieldsDataTable.Columns.Add("SeqNo", typeof(int));
            //MatchingFieldsDataTable.Columns.Add("Field Name", typeof(string));
            //MatchingFieldsDataTable.Columns.Add("Field Type", typeof(string));
        }

        //******************
        // SHOW GRID dataGridView4
        //******************
        private void ShowGrid4()
        {
            // Keep Scroll position 
            int scrollPosition = 0;
            if (WRowGrid1 > 0)
            {
                scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            }


            dataGridView4.DataSource = TableFieldsOrder.DefaultView;

            if (dataGridView4.Rows.Count == 0)
            {
                return;
            }
            dataGridView4.Columns[0].Width = 70; // SeqNo
            dataGridView4.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns[1].Width = 250; // Field Name
            dataGridView4.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView4.Columns[2].Width = 100; // Field Type
            dataGridView4.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.FirstDisplayedScrollingRowIndex = scrollPosition;

            // DATA TABLE ROWS DEFINITION 
            //MatchingFieldsDataTable.Columns.Add("SeqNo", typeof(int));
            //MatchingFieldsDataTable.Columns.Add("Field Name", typeof(string));
            //MatchingFieldsDataTable.Columns.Add("Field Type", typeof(string));
        }
        //
        // Force Matched - to be confirmed 
        //
        int RSeqNo;
        string ROrigin;
        decimal RAmt;
        int WUnique;
        private void button3_Click(object sender, EventArgs e)
        {

            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }

            if (WDisputesOn2 == true)
            {
                MessageBox.Show("There is outstanding dispute for this set. Id = " + WDispNo);
                return; 
            }

            if (GTotal2 != 0)
            {
                MessageBox.Show("Total different than zero. Make adjustment please.");
                checkBoxAdj.Show();
                return;
            }
            else
            {
                checkBoxAdj.Hide();
            }

            if (comboBoxReason.Text == "Fill Entry")
            {
                MessageBox.Show("Please select reason");
                return;
            }
            else
            {
                //WMatchedType = comboBoxReason.Text;
            }

            WUnique = Gu.GetNextValue();

            WMatchedType = "ManualToBeConfirmed"; 

            int K = 0;
            //
            //VALIDATION 
            //
            while (K <= (dataGridView2.Rows.Count - 1))
            {
                RSeqNo = (int)dataGridView2.Rows[K].Cells["SeqNo"].Value;
                ROrigin = (string)dataGridView2.Rows[K].Cells["Origin"].Value;
                RAmt = (decimal)dataGridView2.Rows[K].Cells["Amt"].Value;

                if (ROrigin == "EXTERNAL")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_ExternalBySelectionCriteria(SelectionCriteria);

                    Se.Matched = true;
                    Se.ToBeConfirmed = true;
                    Se.ActionType = "00";
                    Se.MatchedType = WMatchedType;

                    Se.UniqueMatchingNo = WUnique;

                    Se.UpdateExternalFooter(WOperator, SelectionCriteria);
                }

                if (ROrigin == "INTERNAL" || ROrigin == "WAdjustment")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    Se.Matched = true;
                    Se.ToBeConfirmed = true;
                    Se.ActionType = "00";
                    Se.MatchedType = WMatchedType;

                    Se.UniqueMatchingNo = WUnique;

                    Se.UpdateInternalFooter(WOperator, SelectionCriteria);
                }

                K++; // Read Next entry of the table 
            }

            //Home
            WMode = 5;
            WReference = "";
            WOrderCriteria = " Origin ASC ";

            panel5.Hide();
            labelAmt.Hide();
            panel7.Hide();
            labelValRef.Hide();
            panelValAndRef.Hide();
            label28.Hide();
            comboBoxReason.Hide();

            MessageBox.Show("Matching Has Been Made." + Environment.NewLine
                 + "Move to next matching or go to confirm.");


            textBoxMsgBoard.Text = "Create next match ";

            Form502a_Load(this, new EventArgs()); // Load Grid    

        }
        //Balance 
        private void button6_Click(object sender, EventArgs e)
        {

        }
        // Add Adjustments 

        private void buttonAddAdjust_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "" || textBox6.Text == "")
            {
                MessageBox.Show("Missing Information");
                return;
            }

            //
            // Read Fields of the selected row
            //
            SelectionCriteria = " WHERE SeqNo =" + WSeqNo2 + " AND Origin ='" + WOrigin2 + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            Se.StmtAccountID = InternalAcc;
            Se.Origin = "WAdjustment";

            Se.SubSystem = WSubSystem;

            Se.ActionType = "03"; // 

            if (decimal.TryParse(textBox5.Text, out Se.StmtLineAmt))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid Amt!");
                return;
            }

            if (decimal.TryParse(textBox8.Text, out Se.CcyRate))
            {
            }
            else
            {
                MessageBox.Show(textBox8.Text, "Please enter a valid Rate!");
                return;
            }


            if (Se.StmtLineAmt > 0)
            {
                Se.StmtLineIsDebit = true;
            }

            if (Se.StmtLineAmt < 0)
            {
                Se.StmtLineIsDebit = false;
            }

            Se.StmtLineSuplementaryDetails = textBox6.Text;
            Se.MatchedType = "Manual Adj";

            Se.IsAdjustment = true;
            Se.Ccy = Ccy;
            //Se.CcyRate = Se.CcyRate;
            Se.AdjGlAccount = comboBoxGl.Text;
            Se.IsAdjClosed = false;

            int InsertedId = Se.InsertNewRecordInInternalStatement();

            // Clear fields 
            textBox5.Text = "";
            textBox6.Text = "";

            textBox10.Text = "";

            Form502a_Load(this, new EventArgs()); // Load Grid    

            checkBoxAdj.Checked = false;

            textBoxMsgBoard.Text = "Comment. ";
        }
        // Update Adjustments 

        private void buttonUpdateAdjust_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "" || textBox6.Text == "")
            {
                MessageBox.Show("Missing Information");
                return;
            }


            SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + WSeqNo2;

            Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

            if (Se.Origin != "WAdjustment")
            {
                MessageBox.Show("You can delete only Adjustment");
                return;
            }

            if (decimal.TryParse(textBox5.Text, out Se.StmtLineAmt))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid Amt!");
                return;
            }

            if (decimal.TryParse(textBox8.Text, out Se.CcyRate))
            {
            }
            else
            {
                MessageBox.Show(textBox8.Text, "Please enter a valid Rate!");
                return;
            }

            if (Se.StmtLineAmt > 0)
            {
                Se.StmtLineIsDebit = true;
            }

            if (Se.StmtLineAmt < 0)
            {
                Se.StmtLineIsDebit = false;
            }

            Se.StmtLineSuplementaryDetails = textBox6.Text;
            Se.MatchedType = "";

            Se.AdjGlAccount = comboBoxGl.Text;

            Se.UpdateAdjustmentSpecific(WSeqNo2);

            WRowGrid2 = dataGridView2.SelectedRows[0].Index;

            Form502a_Load(this, new EventArgs()); // Load Grid    

            textBoxMsgBoard.Text = "Comment. ";

            dataGridView2.Rows[WRowGrid2].Selected = true;
            dataGridView2_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid2));

        }

        // Delete Adjustments 

        private void buttonDeleteAdjust_Click(object sender, EventArgs e)
        {
            if (Se.Origin != "WAdjustment")
            {
                MessageBox.Show("You can delete only Adjustment");
                return;
            }

            if (MessageBox.Show("Warning: Do you want to delete this Adjustment?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {

                Se.DeleteAdjustmentsForInternal(WSeqNo2);

                WRowGrid1 = dataGridView1.SelectedRows[0].Index;

                Form502a_Load(this, new EventArgs()); // Load Grid    

                textBoxMsgBoard.Text = "Comment. ";
            }
            else
            {
            }
        }

        // Clear Fields 
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                textBox5.Text = "";
                textBox6.Text = "";
                textBox8.Text = "";
                textBox10.Text = "";
                comboBoxReason.Text = "Fill Entry";
            }
        }

        // Calculate 
        decimal WAmnt;
        decimal WRate;

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            bool ValidAmt;
            bool ValidRate;
            ValidAmt = false;
            ValidRate = false;

            if (decimal.TryParse(textBox5.Text, out WAmnt))
            {
                ValidAmt = true;
            }
            else
            {
                return;
            }

            if (decimal.TryParse(textBox8.Text, out WRate))
            {
                ValidRate = true;
            }
            else
            {
                return;
            }

            if (ValidAmt == true & ValidRate == true)
            {
                textBox10.Text = (WAmnt * WRate).ToString();
            }

        }
        // Add Field

        string WFieldName3;
        string WFieldDBName3;
        private void buttonAddField_Click(object sender, EventArgs e)
        {

            DataRow RowSelected = TableFieldsOrder.NewRow();

            RowSelected["SeqNo"] = WSeqNo3;
            RowSelected["FieldName"] = WFieldName3;
            RowSelected["FieldDBName"] = WFieldDBName3;

            // ADD ROW
            TableFieldsOrder.Rows.Add(RowSelected);

            ShowGrid4();

        }
        // Remove Field
        private void buttonRemoveField_Click(object sender, EventArgs e)
        {
            for (int i = TableFieldsOrder.Rows.Count - 1; i >= 0; i--)
            {
                int TempSeqNo = (int)TableFieldsOrder.Rows[i]["SeqNo"];
                if (TempSeqNo == WSeqNo4)
                {
                    TableFieldsOrder.Rows[i].Delete();
                    return;
                }
                //DataRow dr = TableFieldsOrder.Rows[i];
                //if (dr["FieldName"] == WFieldName4)
                //    dr.Delete();
            }

        }
        // ROW ENTER FOR Grid 4 
        int WSeqNo4;
        string WFieldName4;
        string WFieldDBName4;
        private void dataGridView4_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView4.Rows[e.RowIndex];

            WSeqNo4 = (int)rowSelected.Cells[0].Value;
            WFieldName4 = (string)rowSelected.Cells[1].Value;
            WFieldDBName4 = (string)rowSelected.Cells[2].Value;


        }
        // Order By 
        private void buttonOrderBy_Click(object sender, EventArgs e)
        {

            StringBuilder WOrderCriteriaBuild = new StringBuilder();

            if (dataGridView4.Rows.Count == 0)
            {
                return;
            }

            int K = 0;
            //
            //VALIDATION 
            //
            while (K <= (dataGridView4.Rows.Count - 1))
            {
                WFieldDBName4 = (string)dataGridView4.Rows[K].Cells["FieldDBName"].Value;

                WOrderCriteriaBuild.Append(WFieldDBName4);
                if (dataGridView4.Rows.Count - 1 != K) WOrderCriteriaBuild.Append(", ");
                if (dataGridView4.Rows.Count - 1 == K) WOrderCriteriaBuild.Append(" ASC");
                K++; // Read Next entry of the table 
            }

            WOrderCriteria = WOrderCriteriaBuild.ToString();

            Form502a_Load(this, new EventArgs()); // Load Grid    

        }
 
        // Search Reference Like 
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox12.Text == "")
            {
                MessageBox.Show("Please enter reference");
                return;
            }

            WReference = textBox12.Text;

            // Check if old with such reference 
            WMode = 15;
            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, WMode, WReconcCycleNo, ExternalAcc, InternalAcc, WOrderCriteria, WWorkingDate, WReference);
            if (Se.RecordFound == true)
            {
                if (MessageBox.Show("There are old matched with this reference. Do want to visit them?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                {
                    Form291NVMatched NForm291NVMatched;
                    int Mode = 2;
                 
                    NForm291NVMatched = new Form291NVMatched(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo, Mode, WReference);
                    NForm291NVMatched.FormClosed += General_FormClosed;
                    NForm291NVMatched.ShowDialog();

                }
                else
                {
                    // Proceed
                }
            }

            textBox14.Text = textBox12.Text;
            WMode = 6;
            WOrderCriteria = " Origin ASC ";

            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, WMode, WReconcCycleNo, ExternalAcc, InternalAcc, WOrderCriteria, WWorkingDate, WReference);

            if (Se.TableNVStatement_Lines_Both.Rows.Count == 0)
            {
                MessageBox.Show("No Entries to show");
                return;
            }
            else
            {
                ShowGrid1();
            }
                     
        }
        private void General_FormClosed(object sender, FormClosedEventArgs e)
        {
            //WRow = dataGridViewMyATMS.SelectedRows[0].Index;
            //scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;
            //// Load Grid 
            //Form47_Load(this, new EventArgs());

            //dataGridViewMyATMS.Rows[WRow].Selected = true;
            //dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            //dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            //int WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            //int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            WMode = 5;
            WReference = "";
            WOrderCriteria = " Origin ASC ";

            Form502a_Load(this, new EventArgs()); // Load Grid   

            //dataGridView1.Rows[WRowIndexLeft].Selected = true;
            //dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            //dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
        // Refresh 
        private void button2_Click(object sender, EventArgs e)
        {
            WMode = 5;
            WReference = "";
            WOrderCriteria = " Origin ASC ";

            Form502a_Load(this, new EventArgs()); // Load Grid    
        }

        // Make Dispute 
        private void buttonMakeDispute_Click(object sender, EventArgs e)
        {

            // Go to open Dispute 
            Form5NV NForm5NV;
            int From = 1;
            NForm5NV = new Form5NV(WSignedId, WSignRecordNo, WOperator,
                           From, WCategoryId, Se.TableNVStatement_Lines_Both, 0);
            NForm5NV.FormClosed += NForm5NV_FormClosed;
            NForm5NV.ShowDialog();
        }

        private void NForm5NV_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form502a_Load(this, new EventArgs());
        }

        // Adjustment 
        private void checkBoxAdj_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAdj.Checked == true)
            {
                panel5.Show();
                textBox6.Text = "Adjustment for " + WInternalReference2;
                textBox8.Text = CcyRate.ToString();
                checkBoxDisputes.Checked = false;

                labelValRef.Hide();
                panelValAndRef.Hide();

            }
            else
            {
                //checkBox6.Hide(); 
                panel5.Hide();

                labelValRef.Show();
                panelValAndRef.Show();

            }
        }
        // Dispute 
        private void checkBoxDisputes_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisputes.Checked == true)
            {
                checkBoxAdj.Checked = false;
                int I = 0;

                while (I <= (Se.TableNVStatement_Lines_Both.Rows.Count - 1))
                {
                    //    RecordFound = true;

                    int TempSeqNo = (int)Se.TableNVStatement_Lines_Both.Rows[I]["SeqNo"];
                    string TempOrigin = (string)Se.TableNVStatement_Lines_Both.Rows[I]["Origin"];

                    if (TempOrigin == "EXTERNAL")
                    {
                        SelectionCriteria = " WHERE IsExternalFile = 1 AND SeqNoInFile = " + TempSeqNo;
                    }
                    if (TempOrigin == "INTERNAL")
                    {
                        SelectionCriteria = " WHERE IsInternalFile = 1 AND SeqNoInFile = " + TempSeqNo;
                    }

                    Dt.ReadNVDisputesTransBySelection(WOperator, SelectionCriteria);

                    if (Dt.RecordFound == true)
                    {
                        MessageBox.Show("Txn with SeqNo = " + TempSeqNo.ToString()
                                      + " In Statement " + TempOrigin + " Already Exist ");
                        return;
                    }

                    I++; // Read Next entry of the table 

                }
                buttonMakeDispute.Show();
                button3.Hide();
                label28.Hide();
                comboBoxReason.Hide();
            }
            else
            {
                buttonMakeDispute.Hide();
                if (GTotal2 == 0)
                {
                    button3.Show();
                    label28.Show();
                    comboBoxReason.Show();
                }
            }
        }
        // GL CHANGE
        private void comboBoxGl_SelectedIndexChanged(object sender, EventArgs e)
        {

            Gp.ParamId = "702";

            Gp.ReadParametersSpecificParmAndOccurance(WUserBankId, Gp.ParamId, comboBoxGl.Text, "", "");
            textBoxAdjDesc.Text = Gp.OccuranceNm;

        }
        // Balance Sheet
        private void linkLabelBalanceSheet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form291NVAccountStatus NForm291NVAccountStatus;

            int WRunningCycle = 503;
            int Mode = 1;
            NForm291NVAccountStatus = new Form291NVAccountStatus(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRunningCycle, Mode);
            NForm291NVAccountStatus.Show();
        }
        // Link To Dispute
        private void linkLabelDispute_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form3_NOSTRO NForm3_NOSTRO;
            string Mode = "VIEW";
            NForm3_NOSTRO = new Form3_NOSTRO(WSignedId, WSignRecordNo, WOperator,
                            Mode, WDispNo);
            NForm3_NOSTRO.FormClosed += NForm3_NOSTRO_FormClosed;
            NForm3_NOSTRO.Show();
        }

        private void NForm3_NOSTRO_FormClosed(object sender, FormClosedEventArgs e)
        {

            //int WRowIndex = dataGridView1.SelectedRows[0].Index;

            //Form503_Load(this, new EventArgs());

            //dataGridView1.Rows[WRowIndex].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
        // Balance Tolerance 
        //public string TransactionCode;
        //public bool DRTxn;
        //public bool CRTxn;
        //public bool IsToleranceAmount;
        //public bool IsValueDaysTolerance;
        string DRCR;
        private void linkLabelBalanceToler_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //WSeqNo = (int)rowSelected.Cells[0].Value;
            //WOrigin = (string)rowSelected.Cells[4].Value;

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            if (Se.Origin == "EXTERNAL" || Se.Origin == "INTERNAL")
            {

            }
            else
            {
                MessageBox.Show("This operation is allowed only for origin INTERNAL or EXTERNAL");
                return;
            }

            if (Se.Origin == "EXTERNAL")
            {
                if (Se.StmtLineIsDebit == true)
                {
                    DRCR = "DR";
                }
                else DRCR = "CR";
            }
            if (Se.Origin == "INTERNAL")
            {
                if (Se.StmtLineIsDebit == true)
                {
                    DRCR = "CR"; // REVERSE 
                }
                else DRCR = "DR";
            }

            Form291NVMatchingRulesDefinition NForm291NVMatchingRulesDefinition;
            int Mode = 2; // View Balance Tolerance 

            NForm291NVMatchingRulesDefinition = new Form291NVMatchingRulesDefinition(WSignedId, WSignRecordNo, WOperator,
                                           InternalAcc, Se.StmtExternalBankID, ExternalAcc, Mode, Se.StmtLineTrxCode, DRCR);
            NForm291NVMatchingRulesDefinition.Show();
        }
        // SHOW RULES FOR VALUE DATES TOLERANCE
        private void linkLabelValTolerance_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            if (Se.Origin == "EXTERNAL" || Se.Origin == "INTERNAL")
            {

            }
            else
            {
                MessageBox.Show("This operation is allowed only for origin INTERNAL or EXTERNAL");
                return;
            }

            if (Se.Origin == "EXTERNAL")
            {
                if (Se.StmtLineIsDebit == true)
                {
                    DRCR = "DR";
                }
                else DRCR = "CR";
            }
            if (Se.Origin == "INTERNAL")
            {
                if (Se.StmtLineIsDebit == true)
                {
                    DRCR = "CR"; // REVERSE 
                }
                else DRCR = "DR";
            }

            Form291NVMatchingRulesDefinition NForm291NVMatchingRulesDefinition;
            int Mode = 3; // View Dates Tolerance 

            NForm291NVMatchingRulesDefinition = new Form291NVMatchingRulesDefinition(WSignedId, WSignRecordNo, WOperator,
                                           InternalAcc, Se.StmtExternalBankID, ExternalAcc, Mode, Se.StmtLineTrxCode, DRCR);
            NForm291NVMatchingRulesDefinition.Show();
        }
        // Show All Rules 
        private void linkLabelAllRules_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form291NVMatchingRulesDefinition NForm291NVMatchingRulesDefinition;
            string InternalAccNo = Mc.InternalAcc;
            string ExternalBankID = Mc.VostroBank;
            string ExternalAccNo = ExternalAcc;
            int RuleMode = 9;
            NForm291NVMatchingRulesDefinition = new Form291NVMatchingRulesDefinition(WSignedId, WSignRecordNo, WOperator,
                                                InternalAccNo, ExternalBankID, ExternalAccNo, RuleMode, "", "");
            NForm291NVMatchingRulesDefinition.ShowDialog();
        }
        // Tolerance Val For External 
        private void linkLabelTolerValRight_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SelectionCriteria = " WHERE SeqNo =" + WSeqNo2 + " AND Origin ='" + WOrigin2 + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            if (Se.Origin == "EXTERNAL" || Se.Origin == "INTERNAL")
            {

            }
            else
            {
                MessageBox.Show("This operation is allowed only for origin INTERNAL or EXTERNAL");
                return;
            }

            if (Se.Origin == "EXTERNAL")
            {
                if (Se.StmtLineIsDebit == true)
                {
                    DRCR = "DR";
                }
                else DRCR = "CR";
            }
            if (Se.Origin == "INTERNAL")
            {
                if (Se.StmtLineIsDebit == true)
                {
                    DRCR = "CR"; // REVERSE 
                }
                else DRCR = "DR";
            }

            Form291NVMatchingRulesDefinition NForm291NVMatchingRulesDefinition;
            int Mode = 3; // View Dates Tolerance 

            NForm291NVMatchingRulesDefinition = new Form291NVMatchingRulesDefinition(WSignedId, WSignRecordNo, WOperator,
                                           InternalAcc, Se.StmtExternalBankID, ExternalAcc, Mode, Se.StmtLineTrxCode, DRCR);
            NForm291NVMatchingRulesDefinition.Show();
        }
        // Balance Tolerance 
        private void linkLabelTolerBalRight_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SelectionCriteria = " WHERE SeqNo =" + WSeqNo2 + " AND Origin ='" + WOrigin2 + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            if (Se.Origin == "EXTERNAL" || Se.Origin == "INTERNAL")
            {

            }
            else
            {
                MessageBox.Show("This operation is allowed only for origin INTERNAL or EXTERNAL");
                return;
            }


            if (Se.Origin == "EXTERNAL")
            {
                if (Se.StmtLineIsDebit == true)
                {
                    DRCR = "DR";
                }
                else DRCR = "CR";
            }
            if (Se.Origin == "INTERNAL")
            {
                if (Se.StmtLineIsDebit == true)
                {
                    DRCR = "CR"; // REVERSE 
                }
                else DRCR = "DR";
            }

            Form291NVMatchingRulesDefinition NForm291NVMatchingRulesDefinition;
            int Mode = 2; // View Balance Tolerance 

            NForm291NVMatchingRulesDefinition = new Form291NVMatchingRulesDefinition(WSignedId, WSignRecordNo, WOperator,
                                           InternalAcc, Se.StmtExternalBankID, ExternalAcc, Mode, Se.StmtLineTrxCode, DRCR);
            NForm291NVMatchingRulesDefinition.Show();
        }
// Print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo
           
            string P1 = "UNMATCHED TRANSACTIONS FOR INTERNAL:.." + InternalAcc + "..AND EXTERNAL:.." + ExternalAcc;
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId;
            string P6 = "No date";
            string P7 = "No date";

            //string P5 = "1005";
            Form56R64NOSTRO ReportNOSTRO64 = new Form56R64NOSTRO(P1, P2, P3, P4, P5, P6, P7);
            ReportNOSTRO64.Show();
        }
    }
}

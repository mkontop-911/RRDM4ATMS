using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form502aNostroAdj : Form
    {
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        RRDMMatchingCategoriesVsSourcesFiles Rcs = new RRDMMatchingCategoriesVsSourcesFiles();
        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();
        RRDMMatchingCategStageVsMatchingFields Rsm = new RRDMMatchingCategStageVsMatchingFields();

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();
        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        int WSeqNo;
        int WSeqNo2;

        //int WRowGrid2;

        //string WCategoryNm;

        string WUserBankId;

        int RunningJob;

        string Ccy;
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

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public DataTable TableFieldsOrder = new DataTable();

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSubSystem;

        string WCategoryId;
        int WReconcCycleNo;
        DateTime WWorkingDate;

        public Form502aNostroAdj(string InSignedId, int SignRecordNo, string InOperator, string InSubSystem ,string InReconcCategoryId, int InReconcCycleNo, DateTime InWorkingDate)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WSubSystem = InSubSystem; 

            WCategoryId = InReconcCategoryId;
            WReconcCycleNo = InReconcCycleNo;

            WWorkingDate = InWorkingDate.AddDays(1);

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            Us.ReadUsersRecord(WSignedId);
            WUserBankId = Us.BankId;

            textBoxMsgBoard.Text = "Select Txns To Match. ";

            //Initialise 

            RunningJob = 203;

            W4DigitMainCateg = WCategoryId.Substring(0, 4);
            //Find Internal and External Account 
            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);
            if (W4DigitMainCateg == "EWB8")
            {
                Ccy = Mc.VostroCurr;
                InternalAcc = Mc.InternalAcc;
                ExternalAcc = Mc.VostroAcc;

                label1.Text = "PAIR OF ACCOUNTS: " + Mc.CategoryName;
                textBox11.Text = Ccy;
            }

            // DELETE ALL ADJUSTMENTS WITH ACTION TYPE = 3 

            Se.DeleteAdjustmentsForInternal(InternalAcc);

            // Turn to 0 Action type 
            string WActionType = "00";

            SelectionCriteria = " WHERE Origin = 'EXTERNAL' AND StmtAccountID ='" + ExternalAcc + "'";

            Se.UpdateActionTypeExternal(WOperator, SelectionCriteria, NullPastDate, WActionType, WWorkingDate);

            SelectionCriteria = " WHERE (Origin = 'INTERNAL' OR Origin = 'WAdjustment') AND StmtAccountID ='" + InternalAcc + "'";

            Se.UpdateActionTypeInternal(WOperator, SelectionCriteria, NullPastDate, WActionType, WWorkingDate);

            TableFieldsOrder = new DataTable();
            TableFieldsOrder.Clear();

            TableFieldsOrder.Columns.Add("SeqNo", typeof(int));
            TableFieldsOrder.Columns.Add("FieldName", typeof(string));
            TableFieldsOrder.Columns.Add("FieldDBName", typeof(string));

            //Home
            WMode = 7;
            WReference = "";
            WOrderCriteria = " Origin ASC ";

            textBoxMsgBoard.Text = "Create next match ";
        }

        private void Form502a_Load(object sender, EventArgs e)
        {

            //WDateOfExternal

            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, WMode, RunningJob, ExternalAcc, InternalAcc, WOrderCriteria, WWorkingDate, WReference);

            ShowGrid1();

            DateTime NullPastDate = new DateTime(1900, 01, 01);
            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, 8, RunningJob, ExternalAcc, InternalAcc, "", NullPastDate, "");

            ShowGrid2(); // Show all with actiontype = 03 

            // Matching Fields (THREE)

            ShowGridMatchingFields();

        }

        string WOrigin;
        string WOrigin2;
        decimal LAmt;

        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;
            WOrigin = (string)rowSelected.Cells[4].Value;

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            LAmt = Se.StmtLineAmt;

            textBox12.Text = Se.StmtLineRefForAccountOwner;
        }
        // Row enter for Category vs Source files 
        private void dataGridView2_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            checkBox2.Checked = false;
            WSeqNo2 = (int)rowSelected.Cells[0].Value;
            WOrigin2 = (string)rowSelected.Cells[4].Value;

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo2 + " AND Origin ='" + WOrigin2 + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
        }

        // Row Enter For Matching fields 
        private void dataGridView3_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            WSeqNo3 = (int)rowSelected.Cells[0].Value;

            Utd.ReadUniversalTableFieldsDefinitionSeqNo(WSeqNo);

            WFieldName3 = Utd.FieldName;
            WFieldDBName3 = Utd.FieldDBName;
        }

        // Add 
        private void buttonAdd_Click_1(object sender, EventArgs e)
        {
            if (WRightRows == 2)
            {
                MessageBox.Show("On the right grid you must have no more than two entries");
                return;
            }
            if (WRightRows == 1 & (WOrigin2 == WOrigin))
            {
                if (WOrigin2 == "Internal") 
                MessageBox.Show("Not allowed origin. Select WAdjustment Origin");
                if (WOrigin2 == "WAdjustment")
                    MessageBox.Show("Not allowed origin. Select Internal Origin");
                return;
            }
     
            if (RAmt > 0 & RAmt != LAmt) 
            {
                MessageBox.Show("Amounts of entries must be equal ");
                return;
            }

            if (WRightRows == 1)
            {
                button3.Show();
            }
            else
            {
                button3.Hide();
            }

            WRowGrid1 = dataGridView1.SelectedRows[0].Index;

            string WActionType = "03"; // 

            if (WOrigin == "EXTERNAL")
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + WSeqNo;

                Se.UpdateActionTypeExternal(WOperator, SelectionCriteria, Se.StmtLineValueDate, WActionType, WWorkingDate);
            }
             
            if (WOrigin == "INTERNAL" || WOrigin == "WAdjustment")
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + WSeqNo;

                Se.UpdateActionTypeInternal(WOperator, SelectionCriteria, Se.StmtLineValueDate, WActionType, WWorkingDate);
            }


            WRowGrid1 = dataGridView1.SelectedRows[0].Index;

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

            if (WOrigin2 == "INTERNAL" || WOrigin2 == "WAdjustment")
            {
                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + WSeqNo2;

                Se.UpdateActionTypeInternal(WOperator, SelectionCriteria, Se.StmtLineValueDate, WActionType, WWorkingDate);
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

            dataGridView1.Columns[4].Width = 85;  // Origin
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

            dataGridView1.Columns[8].Width = 90; //  ValueDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[9].Width = 90; // EntryDate
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 40; // DR/CR
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 100; // Amt
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

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string WOrigin = row.Cells[4].Value.ToString();

                if (WOrigin == "EXTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WOrigin == "INTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
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
       
        string WOurReference2;
        int WRightRows;
        public void ShowGrid2()
        {
            dataGridView2.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                panel5.Hide();
                WRightRows = 0; 
                RAmt = 0;
                button3.Hide(); 
                return;
            }
            else
            {
                panel5.Show();
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

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                string WOrigin = row.Cells[4].Value.ToString();

                if (WOrigin == "EXTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WOrigin == "INTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }


            WOurReference2 = "";

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

              
                if (ROrigin == "INTERNAL")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    WOurReference2 = Se.StmtLineRefForAccountOwner;

                    // Fill Up Info for Panel5
                    textBox5.Text = Se.StmtLineAmt.ToString("#,##0.00");
                    textBox6.Text = Se.StmtLineRefForAccountOwner;
                    textBox10.Text = Se.StmtLineRefForServicingBank; 
                    textBox16.Text = Se.StmtLineSuplementaryDetails;
                }

                if (ROrigin == "WAdjustment")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    // Fill Up Info for Panel5
                    textBox14.Text = Se.StmtLineAmt.ToString("#,##0.00");
                    textBox8.Text = Se.StmtLineRefForAccountOwner;
                    textBox9.Text = Se.StmtLineRefForServicingBank;
                    textBox15.Text = Se.StmtLineSuplementaryDetails;
                    textBox1.Text = Se.UniqueMatchingNo.ToString(); 
                }

                K++; // Read Next entry of the table 
            }

            if (WRightRows ==2)
            {
                label6.Show();
                label7.Show();
                label11.Show();
                label16.Show();
                if (textBox5.Text == textBox14.Text) label6.Text = "Same"; 
                else label6.Text = "Diff";
                if (textBox6.Text == textBox8.Text) label7.Text = "Same";
                else label7.Text = "Diff";
                if (textBox10.Text == textBox9.Text) label11.Text = "Same";
                else label11.Text = "Diff";
                if (textBox16.Text == textBox15.Text) label16.Text = "Same";
                else label16.Text = "Diff";
            }
            else
            {
                label6.Hide();
                label7.Hide();
                label11.Hide();
                label16.Hide();
            } 
        }

        //******************
        // SHOW GRID dataGridView3
        //******************
        private void ShowGridMatchingFields()
        {
            // Keep Scroll position 
            int scrollPosition = 0;
            if (WRowGrid1 > 0)
            {
                scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            }

            // Matching Fields (THREE)

            //string WFilter3 = "Operator = '" + WOperator + "' AND Application = 'NOSTRO'";

            //Rmf.ReadReconcMatchingFieldsToFillDataTable(WFilter3);
            //dataGridView3.DataSource = Rmf.MatchingFieldsDataTable.DefaultView;

         
            string WTableStructureId = "NOSTRO";
            Utd.ReadUniversalTableMatchingFieldsToFillDataTable(WTableStructureId);

            if (dataGridView3.Rows.Count == 0)
            {
                return;
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
        int WMatchingCycle;
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count != 2 )
            {
                MessageBox.Show("You need two entris for this operation");
                return;
            }

            int K = 0;
            //
            //VALIDATION 
            //
            while (K <= (dataGridView2.Rows.Count - 1))
            {
                RSeqNo = (int)dataGridView2.Rows[K].Cells["SeqNo"].Value;
                ROrigin = (string)dataGridView2.Rows[K].Cells["Origin"].Value;

                if (ROrigin == "WAdjustment")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    WUnique = Se.UniqueMatchingNo;
                    WMatchingCycle = Se.MatchedRunningCycle; 
                }

                K++; // Read Next entry of the table 
            }

            K = 0;
            //
            //VALIDATION 
            //
            while (K <= (dataGridView2.Rows.Count - 1))
            {
                RSeqNo = (int)dataGridView2.Rows[K].Cells["SeqNo"].Value;
                ROrigin = (string)dataGridView2.Rows[K].Cells["Origin"].Value;

                if (ROrigin == "INTERNAL")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    Se.Matched = true;
                    Se.ToBeConfirmed = false;
                    Se.ActionType = "00";
                    Se.MatchedType = "Manual";

                    Se.UniqueMatchingNo = WUnique; // Set it as per adjustment 
                    Se.SettledRecord = true;
                    Se.ActionType = "00";

                    Se.MatchedRunningCycle = WMatchingCycle; // Set it as per adjustment 

                    Se.UpdateInternalFooter(WOperator, SelectionCriteria);
                    Se.StmtLineSuplementaryDetails = "Adjustment:" + WUnique.ToString(); 
                    Se.UpdateInternalStmtLineSuplementaryDetails(WOperator, SelectionCriteria);

                }

                if (ROrigin == "WAdjustment")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    string WActionType = "00";

                    Se.UpdateActionTypeInternal(WOperator, SelectionCriteria, Se.StmtLineValueDate, WActionType, WWorkingDate);

                    Se.IsAdjClosed = true;

                    Se.UpdateAdjustmentSpecific(RSeqNo); 
                }

                K++; // Read Next entry of the table 
            }

            //Home
            WMode = 7;
            WReference = "";
            WOrderCriteria = " Origin ASC ";

            MessageBox.Show("Matching Has Been Made."+ Environment.NewLine
                 + "Move to next matching or go to confirm.");


            textBoxMsgBoard.Text = "Create next match ";

            Form502a_Load(this, new EventArgs()); // Load Grid    
       
        }
        //Balance 
        private void button6_Click(object sender, EventArgs e)
        {
            Form291NVAccountStatus NForm291NVAccountStatus;

            
            int WRunningCycle = 503;
            int Mode = 1;
            NForm291NVAccountStatus = new Form291NVAccountStatus(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRunningCycle, Mode);
            NForm291NVAccountStatus.Show();
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

            WMode = 6;
            WReference = textBox12.Text;
            WOrderCriteria = " Origin ASC ";

            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, WMode, RunningJob, ExternalAcc, InternalAcc, WOrderCriteria, WWorkingDate, WReference);

            if (Se.TableNVStatement_Lines_Both.Rows.Count == 0)
            {
                MessageBox.Show("No Entries to show");
                return;
            }
            // Check table 
            ShowGrid1();
        }
        // Refresh 
        private void button2_Click(object sender, EventArgs e)
        {
            WMode = 5;
            WReference = "";
            WOrderCriteria = " Origin ASC ";

            Form502a_Load(this, new EventArgs()); // Load Grid    
        }
    }
}

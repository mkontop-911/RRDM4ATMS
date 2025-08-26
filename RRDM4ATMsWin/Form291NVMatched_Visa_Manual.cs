using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form291NVMatched_Visa_Manual : Form
    {
        /// <summary>
        /// In the morning after Matching took place last night the unmatched are examined
        /// Find the unmatched with default actions
        /// Create the default actions and present them
        /// User decides if some are rejected. The rejected ones will take place during reconciliation next day.
        /// The defualt go for a four eye control = electronic authorisation 
        /// Following the authoriation the actions are finalised and the unmatched record is settled 
        /// Remaining development (On the grid make possible rejections + On Finish finalise action and settle the unmatched record) 
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

        DateTime FromDt;
        DateTime ToDt;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        //int WOutstandingErrors;
        //int WOutstandingUnMatched;

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMNVCardsBothAuthorAndSettlement Sec = new RRDMNVCardsBothAuthorAndSettlement();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        //RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMReconcCategATMsAtRMCycles Rac = new RRDMReconcCategATMsAtRMCycles();

     

        RRDMReconcJobCycles Dj = new RRDMReconcJobCycles();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //
        string Ccy;
        string InternalAcc;
        string ExternalAcc;
        //
        //int WDifStatus;
        string W4DigitMainCateg;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSubSystem; 

        string WCategoryId;
        int WReconcCycleNo;

        int WMode;
        string WReferenceLike; 

        public Form291NVMatched_Visa_Manual(string InSignedId, int SignRecordNo, string InOperator, string InSubSystem , string InReconcCategoryId, int InReconcCycleNo, int InMode, string InReferenceLike)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WSubSystem = InSubSystem; 

            WCategoryId = InReconcCategoryId;
            WReconcCycleNo = InReconcCycleNo;

            WMode = InMode; // If WMode = 1 show matched for pair and Cycle.
                            // if WMode = 2 show all occurances for for this pair all Cycles. 
                            // If WMode = 3 show all Unmatched for this  Cycle.
            WReferenceLike = InReferenceLike;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            W4DigitMainCateg = WCategoryId.Substring(0, 4);
            //Find Internal and External Account 
            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);
            if (WSubSystem == "CardsSettlement" || WSubSystem == "E_FINANCE")
            {
                label1.Text = "PAIR OF ACCOUNTS: " + Mc.CategoryName;
                Ccy = Mc.VostroCurr;
                InternalAcc = Mc.InternalAcc;
                ExternalAcc = Mc.VostroAcc;
            }
         
            label6.Text = "PAIR ... INTERNAL : " + InternalAcc + " ..EXTERNAL : " + ExternalAcc ; 

            //No Dates Are selected

            FromDt = NullPastDate;
            ToDt = NullPastDate;
        }

        // LOAD SCREEN 
        private void Form291_Load(object sender, EventArgs e)
        {
            int SearchMode = 1;

            // Read and Fill Table only the default 
            if (WMode == 3)
            {
                SearchMode = 5;

                string WOrderCriteria = " Origin ASC ";

                Sec.ReadNVCardRecordsForBothByMode(WOperator, WSignedId, WSubSystem, SearchMode, WReconcCycleNo, ExternalAcc, InternalAcc, WOrderCriteria, DateTime.Now.Date, "");

                //Show Table
                ShowGrid();
                //************************************************************
                //************************************************************
                SetScreen();
            }
            // Read and Fill Table only the default 
            if (WMode == 2)
            {
                SearchMode = 15;

                string WOrderCriteria = " Origin ASC ";

                Sec.ReadNVCardRecordsForBothByMode(WOperator, WSignedId, WSubSystem, SearchMode, WReconcCycleNo, ExternalAcc, InternalAcc, WOrderCriteria, NullPastDate, WReferenceLike);

                //if (Se.TableNVStatement_Lines_Both.Rows.Count == 0)
                //{
                //    MessageBox.Show("No Entries to show");
                    
                //    return;
                //}

                //Show Table
                ShowGrid();
                //************************************************************
                //************************************************************
                SetScreen();
            }
        }

        int WSeqNo;
        string SelectionCriteria;
        string WOrigin;
        int WUniqueMatchingNo; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            //TableNVStatement_Lines_Internal.Columns.Add("SeqNo", typeof(int));
            //TableNVStatement_Lines_Internal.Columns.Add("NEXT", typeof(string));
            //TableNVStatement_Lines_Internal.Columns.Add("MatchingNo", typeof(string));
            //TableNVStatement_Lines_Internal.Columns.Add("Origin", typeof(string));

         
            WSeqNo = (int)rowSelected.Cells[0].Value;
            WOrigin = (string)rowSelected.Cells[4].Value;

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

            Sec.ReadNVFromBothVisaAutherAndSettlementBySelectionCriteria(SelectionCriteria);

            int WColorNo = (int)rowSelected.Cells[1].Value;

            if (WColorNo == 11)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DimGray;
            }
            else if (WColorNo == 12)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            }

            WUniqueMatchingNo = Sec.UniqueMatchingNo;

         
        }

        //UNDO Default 
        //int WRowIndex;


        //*************************************
        // Set Screen
        //*************************************
        public void SetScreen()
        {
           
            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Default Approval stage for Job Cycle No: " + WReconcCycleNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0"; 
          
        }

       
        // Show Grid 
        public void ShowGrid()
        {
            //dataGridView1.DataSource = null;
            //dataGridView1.Refresh();

            dataGridView1.DataSource = Sec.TableNVCards_Records_Both.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                //Form2 MessageForm = new Form2("Selection has ended");
                //MessageForm.ShowDialog();
            //    textBox12.Text = "";
                textBox13.Text = "";
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = true;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 40; // Category Id
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 70; // MatchingNo
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].Visible = false;

            dataGridView1.Columns[4].Width = 170; //MatchedType
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].Visible = false;

            dataGridView1.Columns[5].Width = 85;  // Origin
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[6].Width = 105;  // Acc No 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = false;

            dataGridView1.Columns[7].Width = 40;  // Done 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].Visible = false;

            dataGridView1.Columns[8].Width = 40;  // Code 
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

            dataGridView1.Columns[12].Width = 50; // Ccy
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[12].Visible = true;

            dataGridView1.Columns[13].Width = 130; // RRN
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[13].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[13].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[13].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[14].Width = 100; // Other Details
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[15].Width = 95; // Terminal Id 
            //dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[16].Width = 95; // CcyRate
            //dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[16].Visible = true;

            //dataGridView1.Columns[17].Width = 95; // GLAccount
            //dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[17].Visible = true;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int TempSeqNo = (int)row.Cells[0].Value;
                string TempOrigin = row.Cells[5].Value.ToString();

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

               
            }
           
            decimal TotalExternal1 = 0;
            decimal TotalInternal1 = 0;
            decimal TotalAdjust1 = 0;
            decimal GTotal1 = 0;
            string WOurReference1 = "";

            int K = 0;
            //
            // Totals and Analysis  
            //
            while (K <= (dataGridView1.Rows.Count - 1))
            {
                int RSeqNo = (int)dataGridView1.Rows[K].Cells["SeqNo"].Value;
                string ROrigin = (string)dataGridView1.Rows[K].Cells["Origin"].Value;

                if (ROrigin == "EXTERNAL")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + RSeqNo;

                    Sec.ReadNVCards_External_File_SetllementBySelectionCriteria(SelectionCriteria);

                    TotalExternal1 = TotalExternal1 + Sec.TxnAmt;
                }

                if (ROrigin == "INTERNAL")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(SelectionCriteria);

                    WOurReference1 = Sec.RRN;

                    TotalInternal1 = TotalInternal1 + Sec.TxnAmt;
                }

                if (ROrigin == "WAdjustment")
                {
                    SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

                    Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(SelectionCriteria);

                    TotalAdjust1 = TotalAdjust1 + Sec.TxnAmt;
                }

                K++; // Read Next entry of the table 
            }
            // 
            GTotal1 = TotalExternal1 + TotalInternal1 + TotalAdjust1;
            textBox13.Text = GTotal1.ToString("#,##0.00");

            dataGridView1.DataSource = Sec.TableNVCards_Records_Both.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                ////MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("Nothing to show for matched items. Form will not be shown.");
                MessageForm.ShowDialog();
                this.Dispose(); 
                //textBox1.Text = "";
                //textBox2.Text = "";
                //textBox3.Text = "";
                return;
            }

      

        }
        // NOTES 
        private void buttonNotes2_Click_1(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Default Approval stage for Job Cycle No: " + WReconcCycleNo;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WModeNotes = "Read";
            //else WModeNotes = "Update";
            WModeNotes = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WModeNotes, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }
        //Finish 
     
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
//Undo Matched 
        private void buttonUnDo_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to undo these matched txns?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {
               
                SelectionCriteria = " WHERE UniqueMatchingNo =" + WUniqueMatchingNo;

                Sec.Matched = false;
                Sec.ToBeConfirmed = false;
                Sec.ActionType = "00";
                Sec.MatchedType = "Manual";
                Sec.SettledRecord = false;

                Sec.UpdateExternalFooterMatchedToUnmatched(WOperator, SelectionCriteria);
                Sec.UpdateInternalFooterMatchedToUnmatched(WOperator, SelectionCriteria);

                Form291_Load(this, new EventArgs());
             
            }
            else
            {

            }        
            }
// Print 
        private void button1_Click(object sender, EventArgs e)
        {
            Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

            string P1 = "REPORT OF MATCHED TRANSACTIONS";
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId;
            string P6 = DateTime.Now.ToShortDateString();
            string P7 = DateTime.Now.ToShortDateString();

            Form56R66VISA ReportVISA66 = new Form56R66VISA(P1, P2, P3, P4, P5, P6, P7);
            ReportVISA66.Show();
        }
    }
    }


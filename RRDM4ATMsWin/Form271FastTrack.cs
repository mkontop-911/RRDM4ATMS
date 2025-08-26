using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using RRDM4ATMs;
using System.Configuration;

using System.Text;

using System.Security.Principal;

namespace RRDM4ATMsWin
{
    public partial class Form271FastTrack : Form
    {

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMGasParameters Gp = new RRDMGasParameters();

        readonly string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        bool IsOriginAtms;

        public int WUniqueRecordId;

        bool ViewWorkFlow;

        string LineAtmNo;

        //int CallingMode; // 

        public int WSelectedRow = 0;

        string SelectionCriteria;
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WReconcCategory;
        int WRMCycleNo;
        string WAtmNo;

        public Form271FastTrack(string InSignedId, int InSignRecordNo, string InOperator, string InReconcCategory, int InWRMCycleNo, string InAtmNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WReconcCategory = InReconcCategory;
            WRMCycleNo = InWRMCycleNo;
            WAtmNo = InAtmNo; // If valse = ALL then it is for all ATMs within category

            InitializeComponent();

            Rc.ReadReconcCategorybyCategId(WOperator, WReconcCategory);

            if (Rc.Origin == "Our Atms")
            {
                IsOriginAtms = true;
            }
            else
            {
                IsOriginAtms = false;
            }

            // Force Matching Reason
            Gp.ParamId = "714";
            comboBoxReason.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxReason.DisplayMember = "DisplayValue";

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
                labelView.Show();
                label18.Hide();
                label4.Hide();
                panel3.Hide();
                panel4.Hide();
            }

            if (IsOriginAtms)
            {
                label3.Text = "FAST TRACK FOR ATM : " + WAtmNo + " AND CATEGORY : " + WReconcCategory;
            }
            else
            {
                // Not from our ATMs
                label3.Text = "FAST TRACK FOR CATEGORY : " + WReconcCategory;
            }

        }
        private void Form271FastTrack_Load(object sender, EventArgs e)
        {
            try
            {
                if (ViewWorkFlow == true) // View Only 
                {
                    if (IsOriginAtms)
                    {
                        if (WAtmNo != "ALL")
                        {
                            SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                                                                + WReconcCategory + "' AND MatchingAtRMCycle =" + WRMCycleNo
                                                                + " AND TerminalId ='" + WAtmNo + "'"
                                                                + " AND IsMatchingDone = 1 AND Matched = 0 AND MetaExceptionId <> 55 AND ActionType != '07' ";
                        }
                        else
                        {
                            // ALL
                            SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                                    + WReconcCategory + "' AND MatchingAtRMCycle =" + WRMCycleNo
                                    //+ " AND TerminalId ='" + WAtmNo + "'"
                                    + " AND IsMatchingDone = 1 AND Matched = 0 AND MetaExceptionId <> 55 AND ActionType != '07' ";

                        }

                    }
                    else
                    {
                        // Not from our ATMs
                        SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                                    + WReconcCategory + "' AND MatchingAtRMCycle =" + WRMCycleNo
                                    + " AND IsMatchingDone = 1 AND Matched = 0 AND MetaExceptionId <> 55 AND ActionType != '07' ";
                    }


                    //CallingMode = 1; // View
                }
                else
                {
                    if (IsOriginAtms)
                    {
                        if (WAtmNo != "ALL")
                        {
                            // Matching is done but not Settled 
                            SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WReconcCategory + "'"
                                  + "  AND MatchingAtRMCycle =" + WRMCycleNo
                                  + " AND TerminalId ='" + WAtmNo + "'"
                                  + " AND IsMatchingDone = 1 AND Matched = 0  AND MetaExceptionId <> 55 AND SettledRecord = 0 "
                                  + " AND ActionType != '07' ";
                        }
                        else
                        {
                            // ALL
                            SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WReconcCategory + "'"
                                 + "  AND MatchingAtRMCycle =" + WRMCycleNo
                                 //+ " AND TerminalId ='" + WAtmNo + "'"
                                 + " AND IsMatchingDone = 1 AND Matched = 0  AND MetaExceptionId <> 55 AND SettledRecord = 0 "
                                 + " AND ActionType != '07' ";
                        }

                    }
                    else
                    {
                        // Not from our ATMs
                        SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WReconcCategory + "'"
                                 + "  AND MatchingAtRMCycle =" + WRMCycleNo
                                 + " AND IsMatchingDone = 1 AND Matched = 0  AND MetaExceptionId <> 55 AND SettledRecord = 0 "
                                 + " AND ActionType != '07' ";
                    }

                    //CallingMode = 2; // Updating 
                }

                string WSortCriteria = "";

                //No Dates Are selected

                Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria, 1);
                ShowGrid();

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();

                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }
        }

        // Show Grid 

        public void ShowGrid()
        {

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // Unique RecordId
            dataGridView1.Columns[0].Name = "RecordId";
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // Select
            dataGridView1.Columns[1].Name = "Select";
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; // mask
            dataGridView1.Columns[2].Name = "MatchMask";
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 60; // ActionType
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 100; // ActionDesc
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 60; // Settled
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 130; // date
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 90; // Descr
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 40; // Ccy
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 80; // Amount
            dataGridView1.Columns[9].DefaultCellStyle = style;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[9].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[10].Width = 60; // MetaExceptionId
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 90; // Card
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                bool WSelect = (bool)row.Cells[1].Value;
                string WActionType = (string)row.Cells[3].Value;

                if (WSelect == true & WActionType == "04")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                if (WSelect == true & WActionType == "06")
                {
                    row.DefaultCellStyle.BackColor = Color.AliceBlue;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                if (WSelect == true & WActionType == "05")
                {
                    row.DefaultCellStyle.BackColor = Color.AliceBlue;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                if (WSelect == true & WActionType == "08")
                {
                    row.DefaultCellStyle.BackColor = Color.Beige;
                    row.DefaultCellStyle.ForeColor = Color.Black;

                }
                if (WSelect == true & WActionType == "09")
                {
                    row.DefaultCellStyle.BackColor = Color.Beige;
                    row.DefaultCellStyle.ForeColor = Color.Black;

                }
                if (WSelect == false)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }

            }
        }

        // On ROW ENTER   

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WUniqueRecordId = (int)rowSelected.Cells[0].Value;


        }

        // Calculate Totals
        int TotalChecked;
        int TotalUnChecked;
        public void CalculateTotals()
        {
            TotalChecked = 0;
            TotalUnChecked = 0;

            int K = 0;

            while (K <= (dataGridView1.Rows.Count - 1))
            {
                WSelect = (bool)dataGridView1.Rows[K].Cells["Select"].Value;
                //WUniqueRecordId = (int)dataGridView1.Rows[K].Cells["RecordId"].Value;
                //WMask = (string)dataGridView1.Rows[K].Cells["MatchMask"].Value;

                if (WSelect == true)
                {
                    TotalChecked = TotalChecked + 1;
                }
                else
                {
                    TotalUnChecked = TotalUnChecked + 1;
                }


                K++; // Read Next entry of the table 
            }

            textBoxTotalChecked.Text = TotalChecked.ToString();
            textBoxTotalUnchecked.Text = TotalUnChecked.ToString();

            if (IsOriginAtms)
            {
                string SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
             + WReconcCategory + "' AND MatchingAtRMCycle =" + WRMCycleNo
             + " AND TerminalId ='" + WAtmNo + "'"
             + " AND IsMatchingDone = 1 ";
            }
            else
            {
                // Not from our ATMs
                string SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
             + WReconcCategory + "' AND MatchingAtRMCycle =" + WRMCycleNo
             //+ " AND TerminalId ='" + WAtmNo + "'"
             + " AND IsMatchingDone = 1 ";
            }


            Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria, 2);

            textBox11.Text = Mpa.TotalFastTrack.ToString();
            textBox13.Text = Mpa.TotalFastTrackAmount.ToString("#,##0.00");
        }


        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Update 

        bool WSelect;
        //int WUniqueRecordId;
        int TotalUpdated;
        private void Update_Click(object sender, EventArgs e)
        {
            if (radioButtonCreateDefault.Checked == false
                        & radioButtonForceMatching.Checked == false
                                )
            {
                MessageBox.Show("Make Selection Please!");
                return;
            }

            if (radioButtonCreateDefault.Checked == true
                             )
            {
                MessageBox.Show("This action is Dissable In this vesrion!" + Environment.NewLine
                    + "USE Force matching instead"

                    );
                return;
            }


            if (radioButtonCreateDefault.Checked == true)
            {
                MessageBox.Show("This will be implemented with the Bank's chart of accounts");
                return;
            }

            TotalUpdated = 0;

            // Read DataGrid and Update 

            RRDMActions_GL Ag = new RRDMActions_GL();
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WActionId = "";

            int K = 0;

            try
            {

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    WSelect = (bool)dataGridView1.Rows[K].Cells["Select"].Value;
                    WUniqueRecordId = (int)dataGridView1.Rows[K].Cells["RecordId"].Value;
                    WMask = (string)dataGridView1.Rows[K].Cells["MatchMask"].Value;
                    LineAtmNo = (string)dataGridView1.Rows[K].Cells["ATMNo"].Value;

                    if ((radioButtonForceMatching.Checked == true)
                        & comboBoxReason.Text == "Select Reason")
                    {
                        MessageBox.Show("Please Select Reason Of Force Matching");
                        return;
                    }

                    if (radioButtonForceMatching.Checked == true)
                    {
                        WActionId = "04";
                    }

                    if (WSelect == true)
                    {
                        //
                        //SqlConnection conn =
                        //              new SqlConnection(connectionString);
                        //conn.Open();

                        SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;
                        Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
                        //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

                        if ((WActionId == "04") & Mpa.ActionType == "00")
                        {

                            string WUniqueRecordIdOrigin = "Master_Pool";
                            string WMaker_ReasonOfAction = comboBoxReason.Text;


                            //Aoc.CreateAndInsertInActionOccurances(WOperator, WSignedId,
                            //                           WUniqueRecordIdOrigin, WUniqueRecordId, WActionId, 1
                            //                           , WMaker_ReasonOfAction, "Reconciliation");

                            Ag.ReadActionByActionId(WOperator, WActionId, 1);

                            Aoc.ActionId = Ag.ActionId;
                            Aoc.Occurance = Ag.Occurance;

                            Aoc.ActionNm = Ag.ActionNm;

                            // NOT A GL ACTION 
                            Aoc.GL_Sign_1 = "N/A";
                            Aoc.ShortAccID_1 = "N/A";
                            Aoc.AccName_1 = "N/A";

                            Aoc.Branch_1 = "N/A";
                            Aoc.AccNo_1 = "N/A";
                            //StatementDesc_1 = Ag.StatementDesc_1;

                            Aoc.StatementDesc_1 = "N/A";

                            Aoc.GL_Sign_2 = "N/A";
                            Aoc.ShortAccID_2 = "N/A";
                            Aoc.AccName_2 = "N/A";

                            Aoc.Branch_2 = "N/A";
                            Aoc.AccNo_2 = "N/A";

                            Aoc.StatementDesc_2 = "N/A";

                            Aoc.Ccy = Mpa.TransCurr;
                            Aoc.DoubleEntryAmt = Mpa.TransAmount;


                            Aoc.UniqueKeyOrigin = WUniqueRecordIdOrigin;
                            Aoc.UniqueKey = Mpa.UniqueRecordId;

                            Aoc.Maker = WSignedId;

                            Aoc.Maker_ReasonOfAction = WMaker_ReasonOfAction;

                            Aoc.Stage = "01"; // Temporary mode
                            Aoc.RMCateg = Mpa.RMCateg;
                            Aoc.MatchingAtRMCycle = Mpa.MatchingAtRMCycle;
                            Aoc.RMCycle = WRMCycleNo;

                            Aoc.AtmNo = Mpa.TerminalId;
                            Aoc.ReplCycle = Mpa.ReplCycleNo;

                            Aoc.Settled = false;

                            Aoc.OriginWorkFlow = "Reconciliation";

                            Aoc.Operator = WOperator;

                            Aoc.U_TransDate = Mpa.TransDate; 
                            // Insert Occurance 
                            Aoc.InsertActionOccurance();

                            // UPDATE MPA

                            Mpa.MatchedType = comboBoxReason.Text;

                            Mpa.ActionType = WActionId;

                            Mpa.FastTrack = true;
                            Mpa.ActionByUser = true;
                            Mpa.UserId = WSignedId;

                            Mpa.UpdateMatchingTxnsMasterPoolATMsFooterFastTruck(WOperator, WUniqueRecordId, 1);

                            //UpdateMatchingTxnsMasterPoolATMsFooterFastTruck(string InOperator, int InUniqueRecordId, int In_DB_Mode)

                            TotalUpdated = TotalUpdated + 1;

                           

                        }


                        if (radioButtonCreateDefault.Checked == true & Mpa.ActionType == "00")
                        {
                            //SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;
                            //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria);

                            if (Mpa.MetaExceptionNo > 0)
                            {
                                // Delete Existing 

                                Er.DeleteErrorRecordByErrNo(Mpa.MetaExceptionNo);
                                Mpa.MetaExceptionNo = 0;
                            }

                            CreateMetaException(WUniqueRecordId);

                            Mpa.MetaExceptionNo = MetaNumber;

                            Mpa.FastTrack = true;
                            Mpa.ActionByUser = true;
                            Mpa.UserId = WSignedId;

                            Mpa.ActionType = "01";

                            Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);

                            TotalUpdated = TotalUpdated + 1;

                        }

                    }
                    if (WSelect == false)
                    {
                        //SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;
                        //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

                        //if (Mpa.MetaExceptionNo > 0)
                        //{
                        //    // Delete Existing 

                        //    Er.DeleteErrorRecordByErrNo(Mpa.MetaExceptionNo);
                        //    Mpa.MetaExceptionNo = 0;
                        //}

                        //Mpa.FastTrack = false;
                        //Mpa.ActionByUser = false;
                        //Mpa.UserId = "";

                        //Mpa.MatchedType = "by System";

                        //Mpa.ActionType = "00";

                        //Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);
                    }

                   
                    K++; // Read Next entry of the table 
                }


            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }
            

            if (TotalUpdated == 0)
            {
                MessageBox.Show("Make Selection Please!");
                return;
            }

            Form271FastTrack_Load(this, new EventArgs());

            if (radioButtonForceMatching.Checked == true)
            {
                MessageBox.Show("Total updated By Force Matching are :" + TotalUpdated.ToString());
            }

            if (radioButtonCreateDefault.Checked == true)
            {
                MessageBox.Show("Total updated By Default are :" + TotalUpdated.ToString());
            }

            // Initialise variables 
            radioButtonCreateDefault.Checked = false;
            radioButtonForceMatching.Checked = false;
            checkBoxAll.Checked = false;
            checkBoxUnAll.Checked = false;
            checkBoxAllWithFirstZero.Checked = false;
        }

        // UNDO ACTION 
        private void buttonUnDo_Click(object sender, EventArgs e)
        {
            // Read DataGrid and Update 

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            try
            {
                int K = 0;

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    WSelect = (bool)dataGridView1.Rows[K].Cells["Select"].Value;
                    WUniqueRecordId = (int)dataGridView1.Rows[K].Cells["RecordId"].Value;
                    WMask = (string)dataGridView1.Rows[K].Cells["MatchMask"].Value;
                    if (WSelect == true)
                    {
                        SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;
                        Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

                        if (Mpa.ActionType == "04")
                        {
                            // ReadActionsOccurancesByUniqueKey(InUniqueKeyOrigin, InUniqueKey, InActionId);
                            Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", WUniqueRecordId, Mpa.ActionType);
                            if (Aoc.RecordFound == true)
                            {
                                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", WUniqueRecordId, Mpa.ActionType);
                            }

                            Mpa.FastTrack = false;
                            Mpa.ActionByUser = false;
                            Mpa.UserId = "";

                            Mpa.MatchedType = "by System";

                            Mpa.ActionType = "00";

                            Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);

                        }

                    }

                    K++; // Read Next entry of the table 
                }

                // Initialise variables 
                radioButtonCreateDefault.Checked = false;
                radioButtonForceMatching.Checked = false;
                checkBoxAll.Checked = false;
                checkBoxUnAll.Checked = false;
                checkBoxAllWithFirstZero.Checked = false;

            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }
           
            Form271FastTrack_Load(this, new EventArgs());
        }
        // Force Matching 
        private void radioButtonForceMatching_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonForceMatching.Checked == true)
            {
                labelReason.Show();
                comboBoxReason.Show();
            }
            else
            {
                labelReason.Hide();
                comboBoxReason.Hide();
            }
        }

        //
        // Check All 
        //
        private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAll.Checked == true)
            {
                checkBoxUnAll.Checked = false;
                checkBoxAllWithFirstZero.Checked = false;
                // Check all 
                // Read DataGrid and Update 

                int K = 0;

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    dataGridView1.Rows[K].Cells["Select"].Value = true;

                    K++; // Read Next entry of the table 
                }
            }

        }
        //
        // Un Checked all 
        //
        private void checkBoxUnAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUnAll.Checked == true)
            {
                checkBoxAll.Checked = false;
                checkBoxAllWithFirstZero.Checked = false;

                // Un Check all 

                int K = 0;

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    dataGridView1.Rows[K].Cells["Select"].Value = false;

                    K++; // Read Next entry of the table 
                }

            }


        }
        // Check With 011 
        string WMask;
        private void checkBoxAllWithFirstZero_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAllWithFirstZero.Checked == true)
            {
                checkBoxUnAll.Checked = false;
                checkBoxAll.Checked = false;

                // Check all 
                // Read DataGrid and Update 

                int K = 0;

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    WMask = (string)dataGridView1.Rows[K].Cells["MatchMask"].Value;

                    if (WMask == "01" || WMask == "011"
                        || WMask == "0111" || WMask == "01111"
                        || WMask == "011111" || WMask == "0111111")
                    {
                        dataGridView1.Rows[K].Cells["Select"].Value = true;
                    }

                    K++; // Read Next entry of the table 
                }
            }

        }
        // Show Totals
        private void button1_Click(object sender, EventArgs e)
        {
            panel5.Show();

            CalculateTotals();
        }

        int MetaNumber;
        public void CreateMetaException(int InUniqueRecordId)
        {
            string SelectionCriteria = " WHERE UniqueRecordId = " + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            Er.ReadErrorsIDRecord(Mpa.MetaExceptionId, WOperator);

            Ac.ReadAtm(LineAtmNo);
            if (Ac.RecordFound == true)
            {
                Er.BankId = Ac.BankId;
                Er.BranchId = Ac.Branch;
                Er.CitId = Ac.CitId;
            }
            else
            {
                // Read Category 
                Er.BankId = WOperator;
                Er.BranchId = "EWB HeadQuarters";
                Er.CitId = "1000";
            }

            // INITIALISED WHAT IS NEEDED 

            Er.CategoryId = Mpa.RMCateg;
            Er.RMCycle = WRMCycleNo;
            Er.UniqueRecordId = InUniqueRecordId;

            Er.AtmNo = LineAtmNo;
            Er.SesNo = Mpa.ReplCycleNo;
            Er.DateInserted = DateTime.Now;
            Er.DateTime = DateTime.Now;

            Er.UniqueRecordId = WUniqueRecordId;

            Er.ByWhom = WSignedId;

            Er.CurDes = Mpa.TransCurr;
            Er.ErrAmount = Mpa.TransAmount;

            Er.TraceNo = Mpa.TraceNoWithNoEndZero;
            Er.CardNo = Mpa.CardNumber;
            Er.CustAccNo = Mpa.AccNumber;
            Er.TransType = Mpa.TransType;
            Er.TransDescr = Mpa.TransDescr;

            Er.DatePrinted = NullPastDate;

            Er.OpenErr = true;

            Er.UnderAction = true;

            Er.Operator = WOperator;

            MetaNumber = Er.InsertError(); // INSERT ERROR 

        }
        // SHOW SOURCE RECORDS 
        private void buttonSourceRecords_Click(object sender, EventArgs e)
        {

            Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

            //switch (WOperator)
            //{
            //    case "CRBAGRAA":
            //        {
            //            // DEMO MODE

            //            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

            //            NForm78d_AllFiles.ShowDialog();

            //            break;
            //        }
            //    case "ETHNCY2N":
            //        {

            //            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

            //            NForm78d_AllFiles.ShowDialog();

            //            break;
            //        }
            //    case "BCAIEGCX":
            //        {
            //if (IsAmtDifferent == true)
            //{
            //    // Case where amounts are different in POS
            //    MessageBox.Show("Please note that Amounts of transactions are different!");
            //}
            NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, WUniqueRecordId, 1);

            NForm78d_AllFiles_BDC_3.ShowDialog();
            //            break;

            //        }
            //}

        }

        // Catch Details
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append(WindowsIdentity.GetCurrent().Name);
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                         + " . Application will be aborted! Call controller to take care. ");
            }
        }
    }
}


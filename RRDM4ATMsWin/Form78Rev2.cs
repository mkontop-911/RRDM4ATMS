using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using RRDM4ATMs;


namespace RRDM4ATMsWin
{
    public partial class Form78Rev2 : Form
    {

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        public DataTable WTransToBeReversed = new DataTable();

        string StageDescr;
        int WAuthorSeqNumber;

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        bool NormalProcess;

        int WTranNo;
        int WPostedNo;

        bool ViewWorkFlow;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        public int WSeqNo;


        //int CallingMode; // 

        public int WSelectedRow = 0;

        //string SelectionCriteria;
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        int WRMCycleNo;

        public Form78Rev2(string InSignedId, int InSignRecordNo, string InOperator,
            DataTable InTransToBePostedSelected, int InWRMCycleNo)
            {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WRMCycleNo = InWRMCycleNo;

            WTransToBeReversed = new DataTable();
            WTransToBeReversed.Clear();

            WTransToBeReversed = InTransToBePostedSelected;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            labelUser.Text = InSignedId;
            pictureBox1.BackgroundImage = appResImg.logo2;

            WTranNo = Gu.GetNextValue();

            // Force Matching Reason
            Gp.ParamId = "716";
            comboBoxReason.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxReason.DisplayMember = "DisplayValue";

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);


            //labelStep1.Text = "Category ID EWB110. Reconc Of ATMs for cash vs GL and Presenter Errors.";

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************

            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
            NormalProcess = false;

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReconcDifferenceStatus = 0;

            Usi.WFieldChar1 = "Reversals";

            Usi.WFieldNumeric1 = WTranNo;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor)
            {
                NormalProcess = false;
            }
            else NormalProcess = true;

            if (WViewFunction == true) textBoxMsgBoard.Text = "View Only!";
            if (WAuthoriser == true) textBoxMsgBoard.Text = "Authoriser to examine one by one and take action.";
            if (WRequestor == true) textBoxMsgBoard.Text = "Wait for Authoriser.Use Refresh if you wish.";
            if (NormalProcess == true) textBoxMsgBoard.Text = "Examine one by one and take actions.";

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
                //labelView.Show();
                //label18.Hide();

                panel3.Hide();

            }

            
        }

        private void Form78_Load(object sender, EventArgs e)
        {
            try
            {
                // NOTES for final comment 
                Order = "Descending";
                WParameter4 = "Reversals";
                WSearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
                if (Cn.RecordFound == true)
                {
                    labelNumberNotes2.Text = Cn.TotalNotes.ToString();
                }
                else labelNumberNotes2.Text = "0";

                ShowGrid();

                SetScreen();

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
            dataGridView1.DataSource = WTransToBeReversed.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 70; // 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; // SELECT
            dataGridView1.Columns[1].Visible = true; // 

            dataGridView1.Columns[2].Width = 180; // 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[3].Width = 70; // 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[4].Width = 70; //
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 80; //
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[6].Width = 60; //
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 150; //

            dataGridView1.Columns[8].Width = 100; // 

            dataGridView1.Columns[9].Width = 60; //

            dataGridView1.Columns[10].Width = 60; //
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
                if (WSelect == true & WActionType == "01")
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

            WSeqNo = (int)rowSelected.Cells[0].Value;
        }
      
        // FINISH 
        int TotalUpdated;
        bool WSelect;
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle("Reversals", WTranNo, "Reversals"); //

            if (Ap.RecordFound == true & Ap.OpenRecord == true & Ap.Stage == 4)
            {

                // Update stage 
                //
                Ap.Stage = 5;
                Ap.OpenRecord = false;

                Ap.UpdateAuthorisationRecord(Ap.SeqNumber);

                // Read Authorisation Record By Origin and Txn Number 
                // If found and stage equals to 4 proceed , otherwise return

                TotalUpdated = 0;

                // Read DataGrid and Update 

                int K = 0;

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    WSelect = (bool)dataGridView1.Rows[K].Cells["Select"].Value;
                    WPostedNo = (int)dataGridView1.Rows[K].Cells["PostedNo"].Value;

                    if (WSelect == true)
                    {
                        // Read Posted Details and Create reversal

                        Tp.ReadTransToBePostedSpecific(WPostedNo);

                        Tp.ReadTransToBePostedSpecificByUniqueRecordId(Tp.UniqueRecordId);
                        if (Tp.RecordFound == true & Tp.IsReversal== true)
                        {
                            MessageBox.Show("Reversal Already Done for :" + WPostedNo);
                            return;
                        }

                        if (Tp.TransType == 11) Tp.TransType = 22;
                        else Tp.TransType = 12;
                        Tp.TransDesc = "REVERSAL of " + WPostedNo.ToString();

                        if (Tp.TransType2 == 11) Tp.TransType2 = 22;
                        else Tp.TransType2 = 12;
                        Tp.TransDesc2 = "REVERSAL of " + WPostedNo.ToString();

                        Tp.MakerUser = WSignedId;

                        Tp.AuthUser = WSignedId;

                        Tp.OpenDate = DateTime.Now;

                        Tp.OpenRecord = true;

                        int NewTxn = Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo, Tp.OriginName);
                        //
                        // Update it as Reversal 
                        //
                        Tp.UpdateReversalTransToBePosted(NewTxn);

                        TotalUpdated = TotalUpdated + 1;
                    }

                    K++; // Read Next entry of the table 
                }

                MessageBox.Show("Total Reversals Made : " + TotalUpdated);

            }
            else
            {

            }

            this.Dispose();

        }
     
     

        private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAll.Checked == true)
            {
                checkBoxUnAll.Checked = false;
                //checkBoxAllWithFirstZero.Checked = false; 
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

        private void checkBoxUnAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUnAll.Checked == true)
            {
                checkBoxAll.Checked = false;
                //checkBoxAllWithFirstZero.Checked = false;

                // Un Check all 

                int K = 0;

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    dataGridView1.Rows[K].Cells["Select"].Value = false;

                    K++; // Read Next entry of the table 
                }

            }
        }
// Authorise
        private void buttonAuthor_Click(object sender, EventArgs e)
        {

            if (comboBoxReason.Text == "Select Reason")
            {
                MessageBox.Show("Please Define the reason for reversals");
                return;
            }

            TotalUpdated = 0;

            // Read DataGrid and find out the checked ones

            int K = 0;

            while (K <= (dataGridView1.Rows.Count - 1))
            {
                WSelect = (bool)dataGridView1.Rows[K].Cells["Select"].Value;
                WPostedNo = (int)dataGridView1.Rows[K].Cells["PostedNo"].Value;

                if (WSelect == true)
                {
                    // Read Posted Details and Create reversal

                    Tp.ReadTransToBePostedSpecific(WPostedNo);

                    Tp.ReadTransToBePostedSpecificByUniqueRecordIdForReversal(Tp.UniqueRecordId);
                    if (Tp.IsReversal == true)
                    {
                        MessageBox.Show("Reversal Already Done for :" + WPostedNo + Environment.NewLine
                                        + "Exclude and repeat operation");
                        return;
                    }

                    TotalUpdated = TotalUpdated + 1;
                }

                K++; // Read Next entry of the table 
            }

            if (TotalUpdated == 0)
            {
                MessageBox.Show("Make Selection Please!");
                return;
            }

            // Validate if outstanding no decision  

            Form110 NForm110;

            string WOrigin = "Reversals";


            //OriginId
            // "01" OurATMS-Matching
            // "02" BancNet Matching                               
            // "03" OurATMS-Reconc
            // "04" OurATMS-Repl
            // "05" Settlement
            // "07" Disputes 
            // "08" Settlement 
            // "09" Reversals

            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, "Reversals", WTranNo, AuthorSeqNumber,0,"",0 ,"Normal");
            //NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WAtmNo, WSesNo, AuthorSeqNumber, "Normal");
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();
        }

        void NForm110_FormClosed(object sender, FormClosedEventArgs e)
        {
            //************************************************************
            //************************************************************
            // AUTHOR PART
            //************************************************************
            WViewFunction = false;
            WAuthoriser = false;
            WRequestor = false;
            NormalProcess = false;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor
            else NormalProcess = true;

            //Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "ReconciliationBulk");

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm("Reversals", WTranNo, "Reversals");

            if (WRequestor == true & Ap.Stage == 1)
            {
                textBoxMsgBoard.Text = "Message was sent to authoriser. Refresh for progress ";
            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                textBoxMsgBoard.Text = "Authorisation made. Workflow can finish! ";

                dataGridView1.Enabled = false;
            }

            if (NormalProcess) // Orginator has deleted authoriser 
            {
                textBoxMsgBoard.Text = "Please make authorisation ";
            }

            Form78_Load(this, new EventArgs());

        }
        //*************************************
        // Set Screen
        //*************************************
        public void SetScreen()
        {
            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageReconCateg("Reversals", WTranNo, "Reversals", WAuthoriser, WRequestor, Reject);
                textBoxMsgBoard.Text = Ap.MessageOut;
            }

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Reversals" + WTranNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (WViewFunction == true || WAuthoriser == true || WRequestor == true) // THIS is not normal process 
            {
                ViewWorkFlow = true;

                if (Cn.TotalNotes == 0)
                {
                    //label1.Hide();

                    buttonNotes2.Hide();
                    labelNumberNotes2.Hide();
                }
                else
                {
                    buttonNotes2.Show();
                    labelNumberNotes2.Show();
                }
            }
            else
            {
                buttonNotes2.Show();
                labelNumberNotes2.Show();
            }

            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle("Reversals", WTranNo, "Reversals"); //

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        textBoxMsgBoard.Text = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {
                      
                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ProcessNo = 2; // Return to stage 2  
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                        NormalProcess = true; // TURN TO NORMAL TO SHOW WHAT IS NEEDED 

                        buttonNotes2.Show();
                        labelNumberNotes2.Show();
                    }

                }

            }

            // Show Authorisation record 
            ShowAuthorisationInfo();

        }

        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle("Reversals", WTranNo, "Reversals"); //
            if ((Ap.RecordFound == true & Ap.OpenRecord == true)
                   || (Ap.RecordFound == true & Ap.OpenRecord == false & Ap.Stage == 5))
            {
                labelDtAuthRequest.Text = "Date of Request : " + Ap.DateOriginated.ToString();

                if (Ap.Stage == 1) StageDescr = "Authoriser Not Available yet.";
                if (Ap.Stage == 2) StageDescr = "Authoriser got the message. He will get action.";
                if (Ap.Stage == 3) StageDescr = "Authoriser took action. Requestor must act. ";
                if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                {
                    StageDescr = "Authorization accepted. Ready for Finish";
                }
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    StageDescr = "Authorization REJECTED. ";
                    Color Red = Color.Red;
                    labelAuthStatus.ForeColor = Red;
                }

                if (Ap.Stage == 5) StageDescr = "Authorisation process is completed";

                labelAuthStatus.Text = "Current Status : " + StageDescr;

                Us.ReadUsersRecord(Ap.Requestor);
                labelRequestor.Text = "Requestor : " + Us.UserName;

                Us.ReadUsersRecord(Ap.Authoriser);
                labelAuthoriser.Text = "Authoriser : " + Us.UserName;

                textBoxComment.Text = Ap.AuthComment;

                WAuthorSeqNumber = Ap.SeqNumber;
                labelAuthHeading.Show();
                labelAuthHeading.Text = "AUTHORISER's SECTION FOR CATEGORY : " + "Reversals";
                panelAuthor.Show();

                if (WViewFunction == true) // For View only 
                {
                    // Main buttons
                    buttonAuthor.Hide();

                    // Authoriser
                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }

                if (WAuthoriser == true & (Ap.Stage == 2 || Ap.Stage == 3)) // For Authoriser from author management 
                {
                    // Main buttons
                    buttonAuthor.Hide();

                    // Authoriser
                    buttonAuthorise.Show();
                    buttonReject.Show();
                    textBoxComment.ReadOnly = false;
                }

                if (WAuthoriser == true & Ap.Stage == 4) // For Authoriser from author management 
                {
                    // Main buttons
                    buttonAuthor.Hide();

                    // Authoriser
                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }

                if (WRequestor == true || NormalProcess) // For Requestor from author management 
                {
                    if (Ap.Stage < 3) // Not authorise yet
                    {
                        // Main buttons
                        buttonAuthor.Hide();

                        // Authoriser
                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES") // Authorised and accepted
                    {
                        // Main buttons
                        buttonAuthor.Hide();

                        // Authoriser
                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;

                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO") // Authorised but rejected
                    {
                        // Main buttons
                        buttonAuthor.Show();

                        // Authoriser
                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;
                    }
                }
            }
            else
            {
                // THIS IS THE NORMAL ... You do not show the AUTH box 
                if (NormalProcess & WRequestor == false) // Normal Reconciliation 
                {
                    // Do not show Authorisation Section this will be shown after authorisation 
                    labelAuthHeading.Hide();
                    panelAuthor.Hide();

                }
            }

        }
        // Authorise
        private void buttonAuthorise_Click(object sender, EventArgs e)
        {
            UpdateAuthorRecord("YES");

            ShowAuthorisationInfo();
        }
        // Update Authorization Record 
        private void UpdateAuthorRecord(string InDecision)
        {

            Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);
            if (Ap.OpenRecord == true)
            {
                Ap.AuthDecision = InDecision;
                if (textBoxComment.TextLength > 0)
                {
                    Ap.AuthComment = textBoxComment.Text;
                }
                Ap.DateAuthorised = DateTime.Now;
                Ap.Stage = 3;

                Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);

                if (InDecision == "YES")
                {
                    MessageBox.Show("Authorization ACCEPTED! by : " + labelAuthoriser.Text);
                    //this.Dispose();
                }
                if (InDecision == "NO")
                {
                    MessageBox.Show("Authorization REJECTED! by : " + labelAuthoriser.Text);
                    //   this.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Authorization record is not open. Requestor has closed it.");
                return;
            }
        }
        // Notes
        string WMode;
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Reversals" + WTranNo;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            Form78_Load(this, new EventArgs());
        }
// History 
        private void buttonAuthHistory_Click(object sender, EventArgs e)
        {

        }
    }
}

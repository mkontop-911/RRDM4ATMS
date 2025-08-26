using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form49 : Form
    {
        // ERRORS MANAGEMENT 
        //
         RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();
         RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
         RRDMSessionsNewSession Sa = new RRDMSessionsNewSession();
         RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

         RRDMUsersRecords Us = new RRDMUsersRecords();

         string WUserBankId;

        int WRowIndex; 

         Bitmap SCREENinitial;
         string AuditTrailUniqueID = "";

         Form62 NForm62; 

         int WSesNo;

         bool StartInDiff;
         bool FinishInDiff;
         int WErrNo;

         int WFunction;
        string WOperator;
        string WSignedId;
        int WSignRecordNo;
        string WBankId;
   
        string WAtmNo;

        string errfilter; 
       // int WSesNo;

        public Form49(string InOperator, string InSignedId, int InSignRecordNo, string InBankId,  string InAtmNo)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WBankId = InBankId;
  
            WAtmNo = InAtmNo;
          
            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            // ================USER BANK =============================
            Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            WUserBankId = Us.Operator;
            // ========================================================

            textBoxMsgBoard.Text = " ANY CHANGE YOU WILL MAKE ON Errors may influence reconciliation! ";

            label2.Text = "LIST OF ACTIVE ERRORS FOR ATM : " + WAtmNo; 

        }

        private void Form49_Load(object sender, EventArgs e)
        {
           
            label2.Text = "LIST OF ACTIVE ERRORS FOR ATM : " + WAtmNo; 

            errfilter = "BankId = '" + WBankId + "'" + " AND " + "AtmNo = '" + WAtmNo + "' AND OpenErr=1";

            Er.ReadErrorsAndFillTable(WBankId, WSignedId, errfilter);

            ShowGrid();           

                System.Drawing.Bitmap memoryImage;
                memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
                tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
                SCREENinitial = memoryImage;
        }

        // ON ROW ENTER 
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow WRowIndex = dataGridView1.Rows[e.RowIndex];
            label7.Text = WRowIndex.Cells[0].Value.ToString();
            WErrNo = (int)WRowIndex.Cells[0].Value;
            label7.Text = WErrNo.ToString();

            Er.ReadErrorsTableSpecific(WErrNo);

          //  WAtmNo = Ea.WAtmNo;

            checkBox1Open.Checked = Er.OpenErr;
            checkBox2UnderAction.Checked = Er.UnderAction;
            checkBox3ManualAct.Checked = Er.ManualAct;

            textBox3usercommentold.Text = Er.UserComment;
            textBox2Newcomment.Text = ""; 
        }
        
       
        //
        // UPDATE ERROR  
        //

        private void button2_Click_1(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            if (String.IsNullOrEmpty(textBox2Newcomment.Text))
            {
                MessageBox.Show("FILL IN THE USER COMMENT Please");
                return;
            }
           

            // FIND LAST SESSION FOR THIS ATM

            Sa.ReadSessionsStatusTracesLastNo(WAtmNo);

            WSesNo = Sa.LastSesNo;

            WFunction = 4; // INCLUDE IN BALANCES ANY COREECTED ERRORS 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // CALL TO MAKE BALANCES AVAILABLE 


            if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false)
            {
                StartInDiff = false;
            }
            else StartInDiff = true;


            Er.OpenErr = checkBox1update.Checked;
            Er.UnderAction = checkBox2update.Checked;
            Er.ManualAct = checkBox3update.Checked;

            Er.UserComment = textBox2Newcomment.Text;

            Er.UpdateErrorsTableSpecific(WErrNo);

            if (StartInDiff == false)
            {

                WFunction = 4; // INCLUDE IN BALANCES ANY COREECTED ERRORS 

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // CALL TO MAKE BALANCES AVAILABLE 


                if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false)
                {
                    FinishInDiff = false;
                }
                else FinishInDiff = true;

                if (StartInDiff == false & FinishInDiff == true)
                {
                    MessageBox.Show("THE CHANGE IN ERROR STATUS HAS CREATED DIFFERENCES. YOU HAVE TO GO TO RECONCILIAION ");

                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);


                    Ta.Recon1.RecFinDtTm = DateTime.Now;
                    Ta.Recon1.DiffReconcEnd = true;
                    Ta.NumOfErrors = Na.NumberOfErrors;
                    Ta.ErrOutstanding = Na.ErrOutstanding;
                    //
                    // UPDATE SESSION TRACES
                    //

                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                    textBoxMsgBoard.Text = " DATA HAS BEEN UPDATED. YOU CHOOSE ANOTHER ERROR OR GO TO MAIN ";

                }
            }
            textBoxMsgBoard.Text = " DATA HAS BEEN UPDATED. YOU CHOOSE ANOTHER ERROR OR GO TO MAIN ";
            Form49_Load(this, new EventArgs());
            //AUDIT TRAIL 
            
            //AUDIT TRAIL 
            string AuditCategory = "Operations";
            string AuditSubCategory = "Errors Management";
            string AuditAction = "Change Error Status";
            string Message = textBoxMsgBoard.Text;
            GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);
        }
        

        private void buttonMsgs_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        //AUDIT TRAIL : GET IMAGE AND INSERT IT IN AUDIT TRAIL 
        private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory,
            string InTypeOfChange, string InUser, string Message)
        {

            Bitmap SCREENb;
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENb = memoryImage;

            AuditTrailClass At = new AuditTrailClass();

            if (AuditTrailUniqueID.Equals(""))
            {
                AuditTrailUniqueID = At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            }
            else
            {
                At.UpdateRecord(AuditTrailUniqueID, InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            }

        }
        // SHOW JOURNAL 
        private void button7_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            int Action = 25;
            string SingleChoice = Er.TraceNo.ToString();
            NForm62 = new Form62(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, Action,
                NullPastDate, NullPastDate, SingleChoice);
            NForm62.Show();
        }
        // SHOW ALL ACTIVE ERRORS 
        private void button1_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            label2.Text = "LIST OF ACTIVE ERRORS FOR ATM : " + WAtmNo;

            errfilter = "BankId = '" + WBankId + "'" + " AND " + "AtmNo = '" + WAtmNo + "' AND OpenErr=1";

            Er.ReadErrorsAndFillTable(WBankId, WSignedId, errfilter);

            ShowGrid();

        }
        // SHow all closed erros 
        private void button3_Click(object sender, EventArgs e)
        {

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            label2.Text = "LIST OF CLOSED ERRORS FOR ATM : " + WAtmNo;

            errfilter = "BankId = '" + WBankId + "'" + " AND " + "AtmNo = '" + WAtmNo + "' AND OpenErr=0";

            Er.ReadErrorsAndFillTable(WBankId, WSignedId, errfilter);

            ShowGrid();
        }
        // Show Grid 
        //******************
        // SHOW GRID
        //******************
        private void ShowGrid()
        {
            dataGridView1.DataSource = Er.ErrorsTable.DefaultView;

            dataGridView1.Columns[0].Width = 40; // ExcNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 80; // Desc
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 90; //  Card
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 50; // Ccy
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 80; // Amount
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 50; // NeedAction
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 50; // UnderAction 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 90; // DateTime

            dataGridView1.Columns[8].Width = 80; // TransDescr

            dataGridView1.Columns[9].Width = 140; // TransDescr

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No available errors with this search! ");
            }

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
// Finish 
        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}

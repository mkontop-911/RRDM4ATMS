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

namespace RRDM4ATMsWin
{
    public partial class Form49 : Form
    {
        // ERRORS MANAGEMENT 
        //
         RRDMErrorsClassWithActions Ea = new RRDMErrorsClassWithActions();
         RRDMNotesBalances Na = new RRDMNotesBalances();
         RRDMNewSession Sa = new RRDMNewSession();
         RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

         RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

         string WUserBankId;

         Bitmap SCREENinitial;
         string AuditTrailUniqueID = "";

         Form62 NForm62; 

         int WSesNo;

         bool StartInDiff;
         bool FinishInDiff;
         int WErrNo;

         int WFunction;

         string WSignedId;
        int WSignRecordNo;
        string WBankId;
      //  bool WPrive;
        string WAtmNo;

        string errfilter; 
       // int WSesNo;

        public Form49(string InSignedId, int InSignRecordNo, string InBankId,  string InAtmNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WBankId = InBankId;
      //      WPrive = InPrive; 
            WAtmNo = InAtmNo;
          
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

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

                errorsTableBindingSource.Filter = errfilter;

                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending); 

                this.errorsTableTableAdapter.Fill(this.aTMSDataSet2.ErrorsTable);

                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No open errors! If you wish push the appropriate button to view the closed ones. ");
                }

                System.Drawing.Bitmap memoryImage;
                memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
                tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
                SCREENinitial = memoryImage;
        }

        // ON ROW ENTER 
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            label7.Text = rowSelected.Cells[0].Value.ToString();
            WErrNo = (int)rowSelected.Cells[0].Value;
            label7.Text = WErrNo.ToString();

            Ea.ReadErrorsTableSpecific(WErrNo);

          //  WAtmNo = Ea.WAtmNo;

            checkBox1Open.Checked = Ea.OpenErr;
            checkBox2UnderAction.Checked = Ea.UnderAction;
            checkBox3ManualAct.Checked = Ea.ManualAct;

            textBox3usercommentold.Text = Ea.UserComment;
            textBox2Newcomment.Text = ""; 
        }
        
       
        //
        // UPDATE ERROR  
        //

        private void button2_Click_1(object sender, EventArgs e)
        {
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


            Ea.OpenErr = checkBox1update.Checked;
            Ea.UnderAction = checkBox2update.Checked;
            Ea.ManualAct = checkBox3update.Checked;

            Ea.UserComment = textBox2Newcomment.Text;

            Ea.UpdateErrorsTableSpecific(WErrNo);

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
            string SingleChoice = Ea.TraceNo.ToString();
            NForm62 = new Form62(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, Action,
                NullPastDate, NullPastDate, SingleChoice);
            NForm62.Show();
        }
        // SHOW ALL ACTIVE ERRORS 
        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = "LIST OF ACTIVE ERRORS FOR ATM : " + WAtmNo;

            errfilter = "BankId = '" + WBankId + "'" + " AND " + "AtmNo = '" + WAtmNo + "' AND OpenErr=1";

            errorsTableBindingSource.Filter = errfilter;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);

            this.errorsTableTableAdapter.Fill(this.aTMSDataSet2.ErrorsTable);

        }
        // SHow all closed erros 
        private void button3_Click(object sender, EventArgs e)
        {
            label2.Text = "LIST OF CLOSED ERRORS FOR ATM : " + WAtmNo;

            errfilter = "BankId = '" + WBankId + "'" + " AND " + "AtmNo = '" + WAtmNo + "' AND OpenErr=0";

            errorsTableBindingSource.Filter = errfilter;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);

            this.errorsTableTableAdapter.Fill(this.aTMSDataSet2.ErrorsTable);
        }

        

    }
}

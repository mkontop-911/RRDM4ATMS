using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
//using System.Data.OleDb;
using RRDM4ATMs;
using RRDMAgent_Classes;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form291NV_MT_103 : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;
        
        public bool Prive;

        //int WAction;

        int WJobCycleNo;

        bool ViewWorkFlow;

        string WCategoryId;

        DateTime WCut_Off_Date; 

        string MsgFilter;

        RRDMBanks Ba = new RRDMBanks();

       // RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

       // RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

     
        RRDMNV_MT_103 Mt = new RRDMNV_MT_103();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMActions_GL Ag = new RRDMActions_GL();
        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();


        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        string WMatchingRunningGroup;

        // Methods 
        // READ ATMs Main
        // 
        public Form291NV_MT_103(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InMatchingRunningGroup)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            WMatchingRunningGroup = InMatchingRunningGroup;

            InitializeComponent();

            // Set Working Date 
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            //*****************************************
            //
            //*****************************************

            

            textBoxMsgBoard.Text = "Job Cycles ";

            comboBoxActionNm.Items.Add("Case confirmed for crediting Account");
            comboBoxActionNm.Items.Add("Cancel case and create MT192");
            comboBoxActionNm.Items.Add("Add With New Beneficiary Account");
            comboBoxActionNm.Items.Add("Move the case to dispute");
            comboBoxActionNm.Items.Add("Move the case to Money Laundering Department");

            comboBoxActionNm.Text = "Case confirmed for crediting Account"; 

            comboBoxReasonOfAction.Items.Add("Check is made");
            comboBoxReasonOfAction.Items.Add("All previous Transactions where excused");
            comboBoxReasonOfAction.Items.Add("Looks that it is money laundering");
            comboBoxReasonOfAction.Items.Add("Account number is corrected.");

            comboBoxReasonOfAction.Text = "Check is made" ;

            // ....

            MsgFilter =
                  "(ReadMsg = 0 AND ToAllAtms = 1)"
              + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;

            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }

            toolTipController.SetToolTip(buttonCommController, "Communicate with today's controller.");

            WJobCycleNo = 0; 

        }
        string WJobCategory = "ATMs";
        // Load
        private void Form200JobCycles_Load(object sender, EventArgs e)
        {

            MessageBox.Show("DETAILS OF THE PREOCESS" + Environment.NewLine
                + "MT 103 messages are loaded from Swift Alliance or otherwise"+Environment.NewLine
                + "RRDM Loading engine is used" + Environment.NewLine
                + "After loading system validates against Bank's systens and creates" + Environment.NewLine
                + "a) Transactions ready to posted if these are below certain predefined limit" + Environment.NewLine
                + "b) Transactions to be verify and confirmed Manually" + Environment.NewLine
                + "c) Transactions with CR account not found or credit not allowed" + Environment.NewLine
                + "d) Transactions with DR account with no funds" + Environment.NewLine
                + "The work allocation of outstanding transactions for manual operation are   " + Environment.NewLine
                + "Based on Bank's operational policies" + Environment.NewLine
                + "System must be tailored towards this end" + Environment.NewLine
                + "Sub-Categories will be defined and owners will be assigned to them" + Environment.NewLine
                + "Some drivers for this are" + Environment.NewLine
                + "Corporate, Retail" + Environment.NewLine
                + "Currency" + Environment.NewLine
                + "Account Responsible officer" + Environment.NewLine
                + "Amount limit" + Environment.NewLine
                + "The remaining if not belong to certain sub-categories will be manually assigned to people " + Environment.NewLine
                + "The work will be done by the maker and authorise by the authoriser." + Environment.NewLine
                + "RRDM must currency rates from Banks systems or from IST" + Environment.NewLine
                + "Rates are based on amount ranges." + Environment.NewLine
                + "Calculation of commission is based on ranges of amount too." + Environment.NewLine
                );
            string SelectionCriteria = "";
            Mt.ReadMT_103AndFillTableOriginBanks(SelectionCriteria);

            ShowGrid1(); 
          
        }

        // ROW ENTER FOR JOB CYCLE 
        string WLatestStatus;
        string WTableId;
        string WSender; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            string WSender = (string)rowSelected.Cells[0].Value;

            Mt.ReadMT_103AndFillTable("WHERE Sender ='"+WSender+"' AND Settled = 0 "); 

            ShowGrid2();

        }

        // Row Enter second grid
        int WUniqueRecordId = 0;
        int WSeqNo;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Mt.ReadActionBySeqNo(WSeqNo); 

            textBoxSenderDetails.Text = Mt.SenderInfo;
            textBoxBanksAccNo.Text = Mt.DR_Account;
            textBoxValueDate.Text = Mt.ValueDate.ToShortDateString(); 
            textBoxReceiverDetails.Text = Mt.ReceiverInfo;
            textBoxAccount.Text = Mt.CR_Account;
            textBoxCcy.Text = Mt.Ccy;
            textBoxAmt.Text = Mt.Amount.ToString("#,##0.00");
            textBoxComAmt.Text = Mt.CommisionAmt.ToString();

            textBoxCommType.Text = Mt.CommisionType; 

            textBoxErrorMessage.Text = Mt.ErrorType;

            textBox1.Text = "EGP";

            textBox2.Text = "18.09";
            decimal temp = 1809/100; 
            textBox3.Text = (Mt.Amount * temp).ToString("#,##0.00");

            if (comboBoxActionNm.Text == "Move the case to dispute")
            {
                labelDisputeId.Show();
                textBoxDisputeId.Show(); 
            }
            else
            {
                labelDisputeId.Hide();
                textBoxDisputeId.Hide();
            }


            // NOTES 2 START  
            Order = "Descending";
            WParameter4 = Mt.UniqueRecordId.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";


            //string WSelectionCriteria = " WHERE SeqNo="+ WSeqNo;

            //Mgt.ReadTransSpecificFromBothTables_By_SelectionCriteria(WSelectionCriteria, WTableId); 


            //WSelectionCriteria = " WHERE TerminalId='" + Mgt.TerminalId + "'"
            //                    + " AND TraceNoWithNoEndZero =" + Mgt.TraceNo
            //                    + " AND TransAmount =" + Mgt.TransAmt
            //                    + " AND Card_Encrypted ='" + Mgt.Card_Encrypted + "'  AND TXNSRC= '1'  AND Origin = 'Our Atms' "
            //                   ;

            //Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2); 

            //if (Mpa.RecordFound)
            //{
            //    WUniqueRecordId = Mpa.UniqueRecordId;


            //}
            //else
            //{
            //    WUniqueRecordId = 0; 
            //}
        }

        // Show Grid1
        private void ShowGrid1()
        {

            dataGridView1.DataSource = Mt.MT_103_OriginBanks.DefaultView;

           
           
            //dataGridView1.Columns[0].Width = 60; // JobCycle;
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            ////dataGridView1.Columns[1].Width = 200; // JobCategory;
            //dataGridView1.Columns[1].Visible = false;
            
            //dataGridView1.Columns[2].Width = 120; // StartDateTm.ToString();
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[3].Width = 120; // FinishDateTm
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[4].Width = 120; //  Description
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[5].Width = 120; // "Status"
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // Show Grid2
        private void ShowGrid2()
        {
            
            dataGridView2.DataSource = Mt.MT_103_DataTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //textBoxTotalLines.Text = dataGridView2.Rows.Count.ToString(); 
                return;
            }
            else
            {

                textBoxTotalLines.Text = dataGridView2.Rows.Count.ToString();
            }

            //DataTableAllFields.Columns.Add("SeqNo", typeof(int));
            //DataTableAllFields.Columns.Add("Done", typeof(string));
            //DataTableAllFields.Columns.Add("Amount", typeof(string));
            //DataTableAllFields.Columns.Add("DR/CR", typeof(string));

            //DataTableAllFields.Columns.Add("IsInMaster", typeof(bool));
            //DataTableAllFields.Columns.Add("IsInJournal", typeof(bool));
            //DataTableAllFields.Columns.Add("IsPresenter", typeof(bool));
            //DataTableAllFields.Columns.Add("MASK", typeof(bool));
            //DataTableAllFields.Columns.Add("TerminalId", typeof(string));
            //DataTableAllFields.Columns.Add("Descr", typeof(string));
            //DataTableAllFields.Columns.Add("Date", typeof(string));
            //DataTableAllFields.Columns.Add("Response", typeof(string));

            //dataGridView2.Columns[0].Width = 60; // SeqNo;
            //dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView2.Columns[1].Width = 60; // Done;
            //dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[2].Width = 80; // Amount
            //dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView2.Columns[3].Width = 60; // DR/CR;
            //dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[4].Width = 60; // IsInMaster;
            //dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[5].Width = 60; // IsInJournal;
            //dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[6].Width = 60; // IsPresenter
            //dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[7].Width = 60; // MASK
            //dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[8].Width = 70; // TerminalId
            //dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Show Grid RCS
        int CountNotDone; 
      
        // ADD A NEW CYCLE 

        int OldReconcJobCycle;

       
        // Message from Controller 
        private void buttonMsgs_Click_1(object sender, EventArgs e)
        {
            NForm55 = new Form55(MsgFilter, WSignedId);
            NForm55.ShowDialog();

            MsgFilter =
                 "(ReadMsg = 0 AND ToAllAtms = 1)"
             + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;
            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }
        }
        // Todays Controller 
        private void buttonCommController_Click_1(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }
        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
       
       
        RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();
        // Show Matching Files 

      
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonViewFiles_Click(object sender, EventArgs e)
        {
           
        }

       
        // View Pending For Matching
        private void buttonOutstandings_Click_1(object sender, EventArgs e)
        {
            string WMatchingCateg ="";

            Form78d_AllFiles_Pending NForm78d_AllFiles_Pending;
            // textBoxFileId.Text
            int WMode = 2; //
                           // InMode = 1 : Not processed yet 
                           // InMode = 2 : Processed this Cycle

            NForm78d_AllFiles_Pending = new Form78d_AllFiles_Pending(WOperator, WSignedId, WMatchingCateg );
            NForm78d_AllFiles_Pending.Show();
           
        }
// Create file
        private void buttonCreateFile_Click(object sender, EventArgs e)
        {
            // Based on conditions read next 1 and fill a table line
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            
            // read records of output fie and fill up table
            // Loop and create a line
            // Insert line in file  
        }
        // Not Settled transactions 
        private void buttonNotSettled_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("The function has been built but it works when in production");
            //return; 

            Form78d_FileRecords NForm78d_FileRecords;
            // textBoxFileId.Text
            int WMode = 5; //
                           // Not Action Taken by Maker
                           // 
            bool WCategoryOnly = false;

            string WTableId = "Atms_Journals_Txns";

            NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, WTableId, "", WJobCycleNo, "", WMode, WCategoryOnly);
            NForm78d_FileRecords.Show();
        }



       

// All Auto 
        private void buttonAllAuto_Click(object sender, EventArgs e)
        {
            string FileId = "Switch_IST_Txns";
            Form78d_FileRecords_IST_PRESENTER NForm78d_FileRecords_IST_PRESENTER;
            NForm78d_FileRecords_IST_PRESENTER = new Form78d_FileRecords_IST_PRESENTER(WOperator, WSignedId, FileId, WJobCycleNo
                                                                                 , WCut_Off_Date, 0);
            NForm78d_FileRecords_IST_PRESENTER.ShowDialog();
        }
// All Manual
        private void buttonAllManual_Click(object sender, EventArgs e)
        {
            string FileId = "Switch_IST_Txns";
            Form78d_FileRecords_IST_PRESENTER NForm78d_FileRecords_IST_PRESENTER;
            NForm78d_FileRecords_IST_PRESENTER = new Form78d_FileRecords_IST_PRESENTER(WOperator, WSignedId, FileId, WJobCycleNo
                                                                                 , WCut_Off_Date, 0);
            NForm78d_FileRecords_IST_PRESENTER.ShowDialog();
        }
// Show Swift Message; 
      
        private void linkLabelViewSwift_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Mt.ReadNV_MT_103_BULK(Mt.Reference);
            MessageBox.Show(Mt.ab);
        }
// Show all transactions made through Swift for this account 
        private void linkLabelViewTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("When txns are available this link will show:"+Environment.NewLine
                            + "All remittance done for this account over the past period."
                               ); 
        }
// Move case to dispute 
        private void comboBoxActionNm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxActionNm.Text == "Move the case to dispute")
            {
                labelDisputeId.Show();
                textBoxDisputeId.Show();
            }
            else
            {
                labelDisputeId.Hide();
                textBoxDisputeId.Hide();
            }
        }
// NOTES

        private void buttonNotes3_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = Mt.UniqueRecordId.ToString();
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = Mt.UniqueRecordId.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
        }
    }
}

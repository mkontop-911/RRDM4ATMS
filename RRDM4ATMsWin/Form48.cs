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
using System.Drawing.Printing;
using System.Configuration;

//multilingual
using System.Resources;
using System.Globalization;


namespace RRDM4ATMsWin
{
    public partial class Form48 : Form
    {
        //string filter;

        Form24 NForm24;
        Form67 NForm67;

        Form51 NForm51;
        Form71 NForm71; 

        int WProcessMode; 

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();
        RRDMNotesBalances Na = new RRDMNotesBalances();
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
      
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        string WUserBankId; 
   
        int WSesNo;

       // string MsgFilter;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
    
        string WAtmNo;
        DateTime WDtFrom;
        DateTime WDtTo;

        public Form48(string InSignedId, int SignRecordNo, string InOperator,  string InAtmNo, DateTime InDtFrom, DateTime InDtTo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
       
            WAtmNo = InAtmNo;
            WDtFrom = InDtFrom;
            WDtTo = InDtTo; 
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            // ================USER BANK =============================
            Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            WUserBankId = Us.Operator;
            // ========================================================

            labelStep1.Text = "Replenishment Cycles for AtmNo : " + WAtmNo; 
           
        }

        private void Form48_Load(object sender, EventArgs e)
        {
           
            //filter = "BankId ='" + WOperator + "' AND AtmNo ='" + WAtmNo + "' AND SesDtTimeStart >='" + WDtFrom + "'" 
            //    + " AND SesDtTimeStart <='" + WDtTo + "'";
            //sessionsStatusTracesBindingSource.Filter = filter;
            //dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending); 
            //this.sessionsStatusTracesTableAdapter.Fill(this.aTMSDataSet46.SessionsStatusTraces);

            //
            //  DataGrid 
            //
            //

            Ta.ReadReplCyclesForFromToDate(WOperator, WAtmNo, WDtFrom, WDtTo);

            dataGridView1.DataSource = Ta.ATMsReplCyclesSelectedPeriod.DefaultView;

            dataGridView1.Columns[0].Width = 70; // SesNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending); 

            dataGridView1.Columns[1].Width = 120; // SesDtTimeStart
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 120; // SesDtTimeEnd
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 80; // FirstTraceNo
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[4].Width = 80; // LastTraceNo 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[5].Width = 80; // AtmNo 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[5].DefaultCellStyle.ForeColor = Color.LightSlateGray;

        }

        // On Row ENTER Identify WSesNo and ATM 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSesNo = (int)rowSelected.Cells[0].Value;

            ShowSessionInfo(WAtmNo, WSesNo); 

        }

        // Show Session Information 
        private void ShowSessionInfo(string InAtmNo, int InSesNo)
        {
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            label13.Text = "REPL CYCLE INFORMATION FOR : " + WSesNo.ToString();

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            if (Ta.RecordFound == false)
            {
                MessageBox.Show("REPL CYCLE NUMBER NOT FOUND ");
                return;
            }

            WProcessMode = Ta.ProcessMode; 

            int Function = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Function);
           

            textBox2.Text = WAtmNo;
            textBox3.Text = WSesNo.ToString();

            textBox5.Text = Ta.FirstTraceNo.ToString();
            textBox7.Text = Ta.LastTraceNo.ToString();

            textBox10.Text = (Ta.Stats1.NoOfTranCash + Ta.Stats1.NoOfTranDepCash + Ta.Stats1.NoOfTranDepCheq).ToString();
            textBox9.Text = (Na.Balances1.OpenBal - Na.Balances1.MachineBal).ToString("#,##0.00");
            textBox14.Text = Na.Balances1.MachineBal.ToString("#,##0.00");

            if (WProcessMode == -1)
            {
                textBox19.Text = "IN PROCESS Repl Cycle";
            }
            if (WProcessMode >= 1)
            {
                textBox19.Text = "COMPLETED Repl Cycle";
                textBox4.Text = Ta.Repl1.ReplStartDtTm.ToString();
                textBox6.Text = Ta.Repl1.ReplFinDtTm.ToString(); 
            }
            if (WProcessMode >= 2)
            {
                textBox19.Text = "COMPLETED Reconciliation and Repl Cycle";
                textBox17.Text = Ta.Recon1.RecStartDtTm.ToString();
                textBox18.Text = Ta.Recon1.RecFinDtTm.ToString(); 
            }

            textBox8.Text = Ta.Diff1.CurrNm1;
            textBox11.Text = Ta.Diff1.DiffCurr1.ToString("#,##0.00");
            textBox12.Text = Ta.SessionsInDiff.ToString();

            Ec.ReadAllErrorsTableForCounterReplCycle(WOperator, WAtmNo, WSesNo);

            textBox13.Text = (Ec.NumOfErrors - Ec.ErrUnderAction).ToString();

            textBox1.Text = Ec.NumOfErrors.ToString();

            if (Ec.NumOfErrors  > 0)
            {
                button1.Show();
            }
            else
            {
                button1.Hide();
            }

            Us.ReadUsersRecord(Ta.Repl1.SignIdRepl);
            textBox15.Text = Us.UserName;

            Us.ReadUsersRecord(Ta.Recon1.SignIdReconc);
            textBox16.Text = Us.UserName;

        }
        // Show Errors 
        private void button1_Click(object sender, EventArgs e)
        {
         
                bool Mode = true;
                string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo;
                NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Mode, SearchFilter);
                NForm24.ShowDialog();
       
        }
        // Show Journal 
        private void button3_Click(object sender, EventArgs e)
        {

            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 2; // FULL

            // WE SHOULD FIND OUT THE START AND OF THIS REPL. CYCLE 
            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, WAtmNo, Ta.FirstTraceNo, Ta.LastTraceNo, Mode);
            NForm67.ShowDialog();

        }
    
// View workflow 
        private void buttonVewWorkFlow_Click(object sender, EventArgs e)
        {
            if (WProcessMode > 0)
            {
                Us.ReadSignedActivityByKey(WSignRecordNo);
                Us.ProcessNo = 54; // View only for replenishment already done  
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm51.ShowDialog();

            }
            else
            {
                MessageBox.Show("Not allowed operation. Repl Workflow not done yet"); 
            }

        }
        // Show reconciliation workflow 
        private void button2_Click(object sender, EventArgs e)
        {
            if (WProcessMode > 1)
            {
                Us.ReadSignedActivityByKey(WSignRecordNo);
                Us.ProcessNo = 54; // View only for reconciliation already done  
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                NForm71 = new Form71(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm71.ShowDialog();

            }
            else
            {
                MessageBox.Show("Not allowed operation. Reconciliation Workflow not done yet");
            }
        }
        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using RRDM4ATMs;

// Alecos
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form503_GL_STATUS : Form
    {

        RRDMSessionsDataCombined Sc = new RRDMSessionsDataCombined();

        RRDMGL_Balances_Atms_Daily_AUDI Gl = new RRDMGL_Balances_Atms_Daily_AUDI(); 

        RRDMGasParameters Gp = new RRDMGasParameters();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime Test_Cut_Off_Date = new DateTime(2021, 09, 05);

        string Order;

        bool FromShow;

        DateTime WDateFrom;
        DateTime WDateTo;
        string WWAtmNo;

        string WParameter4;
        string WSearchP4;

        Form67 NForm67;
        RRDMCaptureCardsClass Cc = new RRDMCaptureCardsClass();
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        string WSelectionCriteria; 

       // bool InternalChange;

        int WSeqNo;

       // int WRowIndex;

       // string WPrefix; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WRMCycle;
        DateTime WCut_Off_Date;
        int WMode;
      //  WOperator, WReconcCycleNo, Rjc.Cut_Off_Date, Mode);
        public Form503_GL_STATUS(string InSignedId, int SignRecordNo, string InOperator,
                                                        int InRMCycle, DateTime InCut_Off_Date , int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WRMCycle = InRMCycle;
            WCut_Off_Date = InCut_Off_Date;
            WMode = InMode;
            // InMode = 1 means
          
            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelCut.Text = WCut_Off_Date.ToShortDateString();

            comboBoxSelection.Items.Add("ALL_This_Cycle");
            comboBoxSelection.Items.Add("This Cycle with Differences");
            comboBoxSelection.Items.Add("This ATM by Date range");

            comboBoxSelection.SelectedIndex = 0;

            WSelectionCriteria = "";

            WMode = 2;
            // 
            // UPDATE GL ENTRIES
            //
            //DateTime Test_Cut_Off_Date = new DateTime(2021, 09, 05);
            //RRDMGL_Balances_Atms_Daily_AUDI Gl = new RRDMGL_Balances_Atms_Daily_AUDI();
            //Gl.UpdateCalculatedGL_For_All_ATMs(WRMCycle, Test_Cut_Off_Date); 
            WCut_Off_Date = Test_Cut_Off_Date; 

        }
        // Load 
        private void Form503_Load(object sender, EventArgs e)
        {
            // SHOW ALL OF THIS comboBoxFilter
            if (FromShow == true)
            {
                // Make selection based on dates
                WSelectionCriteria = "";
                int WMode = 2;
                
                Gl.ReadGL_Balances_Atms_Daily_AUDI_Table_Short(WSelectionCriteria, WCut_Off_Date,
                     WDateFrom, WDateTo, WWAtmNo,
                   WMode);
            }
            else
            {
                int WMode = 1;
                Gl.ReadGL_Balances_Atms_Daily_AUDI_Table_Short(WSelectionCriteria, WCut_Off_Date,
                      NullPastDate, NullPastDate, "" ,
                    WMode); 
            }

            dataGridView1.DataSource = Gl.TableGL_Balances_Atms_Daily_AUDI.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to show");
                ///this.Dispose();
                return;
            }

           
            // *****************
            dataGridView1.Columns[0].Width = 60; // Seq No
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 60; // color 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 85; // Cut_Off_Date
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[3].Width = 70; // AtmNo
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 100; // AtmName
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].Visible = false;

            dataGridView1.Columns[5].Width = 100; // ReplStartDtTm
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 90; // GL_Bal_ATM_Cash   
            dataGridView1.Columns[6].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[7].Width = 90; // OpenningBalance
            dataGridView1.Columns[7].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            // JNL 
            dataGridView1.Columns[8].Width = 90; // Withdrawls_JNL
            dataGridView1.Columns[8].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[9].Width = 90; // Deposits_JNL
            dataGridView1.Columns[9].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[10].Width = 90; // Net_Balance_JNL
            dataGridView1.Columns[10].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[11].Width = 90; // GL_Difference_JNL
            dataGridView1.Columns[11].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            // IST 
            dataGridView1.Columns[12].Width = 90; // Withdrawls_IST
            dataGridView1.Columns[12].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[13].Width = 90; // Deposits_IST
            dataGridView1.Columns[13].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[14].Width = 90; // Net_Balance_IST
            dataGridView1.Columns[14].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[15].Width = 90; // GL_Difference_IST
            dataGridView1.Columns[15].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[15].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int WColorNo = (int)row.Cells[1].Value;
                if (WColorNo == 11)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WColorNo == 12)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

        }
        // On Row Enter
        string WAtmNo;
        int WSesNo; 
        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Gl.ReadGL_Balances_Atms_DailyBySeqNo(WSeqNo); 

            //WAtmNo = Sc.AtmNo;
            //WSesNo = Sc.SesNo; 
            //DateTmSesStart = Sc.SesDtTimeStart;
            //DateTmSesEnd = Sc.SesDtTimeEnd;

            //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            //Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            //if (Ta.RecordFound & Ta.ProcessMode > 0)
            //{
            //    linkLabelRepl.Enabled = true;
            //  //  buttonReplPlay.BackColor = Color.White;
            //}
            //else
            //{
            //    linkLabelRepl.Enabled = false;
            //  //  buttonReplPlay.BackColor = Color.Silver;
            //}

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Note:" + WSesNo.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

        }


        // Change Filter 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
           
         Form503_Load(this, new EventArgs());
        }
     
   
        // Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

// Print
        private void buttonPrint_Click(object sender, EventArgs e)
        {

            string P1 = "Branches Details ";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R76 Report76 = new Form56R76(P1, P2, P3, P4, P5);
            Report76.Show();
        }
// 
        private void linkLabelFromE_Journal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // THESE ARE THE DR TRANSACTIONS
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, Gl.AtmNo, Gl.ReplStartDtTm, Gl.Cut_Off_Date , Gl.ReplCycleNo, NullPastDate, 11);

            NForm78D_ATMRecords.Show();
        }
//
        private void linkLabelCycleTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, Gl.AtmNo, Gl.ReplStartDtTm, Gl.Cut_Off_Date, Gl.ReplCycleNo, NullPastDate, 11);
            

            NForm78D_ATMRecords.Show();
        }

        private void linkLabelUnmatchedTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                                                Gl.AtmNo, Gl.ReplStartDtTm, Gl.Cut_Off_Date, 2
                );
            NForm80b2.Show();
        }

        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Note:" + WSesNo.ToString();
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            string WMode = "Update";
            NForm197 = new Form197(WSignedId, 0, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "Note:" + WSesNo.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }
// EXPORT TO EXCEL 
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString();
            string DD = DateTime.Now.Day.ToString();
            string Min = DateTime.Now.Minute.ToString();

            //MessageBox.Show("Excel will be created in RRDM Working Directory");
            string Id = "GL_Details" + "_" + Min;

            string ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Gl.TableGL_Balances_Atms_Daily_AUDI, WorkingDir, ExcelPath);
        }

        // Selection changed 
        private void comboBoxSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            //comboBoxSelection.Items.Add("ALL_This_Cycle");
            //comboBoxSelection.Items.Add("This Cycle with Differences");
            //comboBoxSelection.Items.Add("This ATM by Date range");

            if (comboBoxSelection.Text == "ALL_This_Cycle")
            {
                WSelectionCriteria = " WHERE LoadedAtRMCycle =" + WRMCycle;
            }
            // *********
            if (comboBoxSelection.Text == "This Cycle with Differences")
            {
                WSelectionCriteria = " WHERE LoadedAtRMCycle =" + WRMCycle + " AND GL_Difference_JNL <> 0 ";
            }
         
            // *********
            if (comboBoxSelection.Text == "This ATM by Date range")
            {
                labelFrom.Show();
                labelTo.Show();
                labelATM.Show();
                dateTimePicker1.Show();
                dateTimePicker2.Show();

                textBoxATM.Show();
                buttonShow.Show();

                label1.Hide();
                panel2.Hide();

                FromShow = true;

                return;
            }
            else
            {
                labelFrom.Hide();
                labelTo.Hide();
                labelATM.Hide();
                dateTimePicker1.Hide();
                dateTimePicker2.Hide();
                textBoxATM.Hide();
                buttonShow.Hide();

                FromShow = false;

                label1.Show();
                panel2.Show();
                // 
            }

            Form503_Load(this, new EventArgs());
        }
// SHOW 
        private void buttonShow_Click(object sender, EventArgs e)
        {
            if (comboBoxSelection.Text == "This ATM by Date range")
            {
                // Check dates 
                if (dateTimePicker1.Value > dateTimePicker2.Value)
                {
                    // 
                    MessageBox.Show("The From date is creater than the to date");
                    return;
                }

                RRDMAtmsClass Ac = new RRDMAtmsClass();
                WWAtmNo = textBoxATM.Text;

                Ac.ReadAtm(WWAtmNo);

                if (Ac.RecordFound == true)
                {
                    // OK
                }
                else
                {
                    MessageBox.Show("ATM Not found");
                    return;
                }

                FromShow = true;

                WDateFrom = dateTimePicker1.Value.Date;
                WDateTo = dateTimePicker2.Value.Date;

                label1.Show();
                panel2.Show();

                // Make 
                Form503_Load(this, new EventArgs());

            }
        }
// EJournal CR
        private void linkLabelJournalCR_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // THESE ARE THE DR TRANSACTIONS
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, Gl.AtmNo, Gl.ReplStartDtTm, Gl.Cut_Off_Date, Gl.ReplCycleNo, NullPastDate, 12);

            NForm78D_ATMRecords.Show();
        }
// IST CR 
        private void linkLabel_IST_CR_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, Gl.AtmNo, Gl.ReplStartDtTm, Gl.Cut_Off_Date, Gl.ReplCycleNo, NullPastDate, 12);

            NForm78D_ATMRecords.Show();
        }
    }
}

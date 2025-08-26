using System;

using System.Windows.Forms;
using RRDM4ATMs;

using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;
using System.Drawing;


namespace RRDM4ATMsWin
{
    public partial class Form62_b : Form 
    {
        
        Form67 NForm67;

        Form84 NForm84;

        Form56R6 NForm56R6;
       
        int WRowGrid1; 

        int WUniqueRecordId;
        //int WTraceNo;

        //string WPrintTrace;
        //string WPrintTraceDtTm;

        int WMode;
        string SelectionCriteria;
        int WMasterTraceNo; 

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMJournalReadTxns_Text_Class Ej = new RRDMJournalReadTxns_Text_Class();

        //multilingual
        CultureInfo culture;

        RRDMUsersRecords Xa = new RRDMUsersRecords(); // Make class availble 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);
     
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
   
        string WAtmNo;
        int WSesNo; 
        int WAction;
        DateTime WFromDate;
        DateTime WToDate;
        string WSingleChoice; 

        // Action = 21 is for Session Transactions, 22 for period, 23 for given card ,
        // 24 for given account, 25 for given Trace No
          
        public Form62_b(string InSignedId, int InSignRecordNo, string InOperator,string InAtmNo,
            int InSesNo, int InAction, DateTime FromDate, DateTime ToDate, string InSingleChoice)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
       
            WAtmNo = InAtmNo;
            WSesNo = InSesNo ;
            WFromDate = FromDate;
            WToDate = ToDate;
            WSingleChoice = InSingleChoice; 

            WAction = InAction; // 21 = Transactions For specific Session , 
                                //  22 = Transactions per specific dates , 23 Trans for card

            InitializeComponent();

            if (WAction != 21)
            {
                button1.Hide(); 
            }
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.Culture == "English")
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (Usi.Culture == "Français")
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }

            if (WAction == 21) label1.Text = LocRM.GetString("Form62label1a", culture) + WSesNo.ToString() + " of Atm: "+WAtmNo  ;
            if (WAction == 22) label1.Text = LocRM.GetString("Form62label1b", culture) + FromDate.Date.ToString() + " -- " + ToDate.Date.ToString();
            if (WAction == 23) label1.Text = LocRM.GetString("Form62label1c", culture) + " " + WSingleChoice; // CARD
            if (WAction == 24) label1.Text = LocRM.GetString("Form62label1d", culture) + " " + WSingleChoice; // ACCOUNT
            if (WAction == 25) label1.Text = LocRM.GetString("Form62label1e", culture) + " " + WSingleChoice; // TRACE NUMBER 
            if (WAction == 26) label1.Text = " For Record Id " + WSingleChoice; // TRACE NUMBER 

            button1.Text = LocRM.GetString("Form62button1", culture);
         
            button3.Text = LocRM.GetString("Form62button3", culture);
      
            button5.Text = LocRM.GetString("Form62button5", culture);

        }

        private void Form62_Load(object sender, EventArgs e)
        {
            // InMode = 1 then selection criteria and no dates
            // InMode = 2 then single date exist 
            // InMode = 3 then range of dates exist 

            if (WAction == 21) // SHOW TRANSACTIONS PER SESSION
            {
                WMode = 1;
                SelectionCriteria = " Where TerminalId ='"+ WAtmNo  + "' AND ReplCycleNo =" + WSesNo;
            
                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator,WSignedId,WMode,
                    SelectionCriteria, "", NullPastDate, NullPastDate,2);

                ShowGridTransactions(); 

               
            }

            if (WAction == 22) // SHOW RANGE 
            {    
                //WMode = 2; 
                //SelectionCriteria = "AtmNo ='" +  WAtmNo + "' AND AtmDtTime BETWEEN @DtFrom AND @DtTo";

                //Tc.ReadInPoolTransDataTable(WMode, SelectionCriteria, NullPastDate, WFromDate, WToDate);

                WMode = 1;
                SelectionCriteria = " Where TerminalId ='" + WAtmNo +"'";

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, WMode,
                    SelectionCriteria, "", WFromDate, WToDate,2);

                ShowGridTransactions();
            }

            if (WAction == 23) // SHOW TRANSACTIONS FOR CARD NUMBER 
            {
               
                //WMode = 1;
                //SelectionCriteria = " CardNo ='" + WSingleChoice + "'";
                ////Read Table
                //Tc.ReadInPoolTransDataTable(WMode, SelectionCriteria, NullPastDate, NullPastDate, NullPastDate);

                WMode = 1;
                SelectionCriteria = " Where CardNumber ='" + WSingleChoice + "'";

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, WMode,
                    SelectionCriteria, "", NullPastDate, NullPastDate,2);

                ShowGridTransactions();

            }

            if (WAction == 24) // SHOW TRANSACTIONS FOR ACC Number  
            {
           //     AccNo LIKE '%013600004883%'
               
                //WMode = 1;
                //SelectionCriteria = " AccNo LIKE '%" + WSingleChoice + "%'";
                ////Read Table
                //Tc.ReadInPoolTransDataTable(WMode, SelectionCriteria, NullPastDate, NullPastDate, NullPastDate);

                WMode = 1;
                SelectionCriteria = " Where AccNumber LIKE '%" + WSingleChoice + "%'";

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, WMode,
                    SelectionCriteria, "", NullPastDate, NullPastDate,2);

                ShowGridTransactions();
                
            }

            if (WAction == 25) // SHOW TRANSACTION FOR TRACE Number  
            {
                int WAtmTraceNo = int.Parse(WSingleChoice);

                //WMode = 1;
                //SelectionCriteria = "AtmNo ='" + WAtmNo + "' AND EJournalTraceNo =" + WAtmTraceNo;
                ////Read Table
                //Tc.ReadInPoolTransDataTable(WMode, SelectionCriteria, NullPastDate, NullPastDate, NullPastDate);

                WMode = 1;
                SelectionCriteria = " Where TerminalId ='" + WAtmNo + "' AND MasterTraceNo =" + WAtmTraceNo;

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, WMode,
                                                    SelectionCriteria, "", NullPastDate, NullPastDate,2);

                ShowGridTransactions();           
            }

            if (WAction == 26) // SHOW TRANSACTION FOR TRANNO
            {
                int WUniqueRecordId = int.Parse(WSingleChoice);

                //WMode = 1;
                //SelectionCriteria = " AtmNo ='" + WAtmNo + "' AND TranNo =" + WTranNo;
                ////Read Table
                //Tc.ReadInPoolTransDataTable(WMode, SelectionCriteria, NullPastDate, NullPastDate, NullPastDate);
                WMode = 1;
                SelectionCriteria = " Where TerminalId ='" + WAtmNo + "' AND UniqueRecordId =" + WUniqueRecordId; 

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, WMode,
                                                    SelectionCriteria, "", NullPastDate, NullPastDate,2);
                ShowGridTransactions();
            }
        }
        //
        // ON ROW ENTER DEFINE TRANSACTION NO 
        //
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WUniqueRecordId = (int)rowSelected.Cells[0].Value;

            string WFilter = " Where  UniqueRecordId = " + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WFilter,2);

            WMasterTraceNo = Mpa.MasterTraceNo; 
            SelectionCriteria = " AtmNo ='" + WAtmNo + "' AND TraceNumber =" + WMasterTraceNo; 
           
            Ej.ReadJournalTextDataTable(WOperator,SelectionCriteria);

            ShowGridTraceJournalLines();

            label3.Text = "ATM Journal for trace :" + WMasterTraceNo; 

        }
        //
        // Show Video Clip
        //
        private void button3_Click(object sender, EventArgs e)
        {
            /*
             // Based on trace Number and start of transaction find the seconds. 
             
             SELECT SUBSTRING(LTRIM(RTRIM(TxtLine)), 6, 8) As TranDate
                   ,SUBSTRING(LTRIM(RTRIM(TxtLine)), 23, 8) As TransTime
                    FROM [ATMS_Journals].[dbo].[tblHstEjText]
                    where TraceNumber = 10042920 and Ruid = (44+3) 
                    order by traceNumber, RuId 
             */

            // Based on Transaction No show video clip 

            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog(); 
        }
        //
        // SHOW JOURNAL PART FOR THE CHOSEN TRANSACTION 
        //
        private void button4_Click(object sender, EventArgs e)
        {
            WRowGrid1 = dataGridView1.SelectedRows[0].Index;

            //String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 5; // Specific

            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator , 0, WAtmNo, WMasterTraceNo, WMasterTraceNo, Mpa.TransDate.Date,NullPastDate ,Mode);
            NForm67.ShowDialog();
        }

        // FULL Journal 

        private void button5_Click(object sender, EventArgs e)
        {
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            string WFilter = " Where  UniqueRecordId = " + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WFilter,2);

            Form67 NForm67;
            int Mode = 3;
            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, Mpa.FuID, WAtmNo, 0, 0, NullPastDate, NullPastDate, Mode);
            NForm67.Show();
        }

        // EXPAND SUPER
        private void buttonExpandSupert_Click(object sender, EventArgs e)
        {
            //String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            //int FromTrace;
            //int ToTrace;
            //int InVariance = 2; 

            //FromTrace = WMasterTraceNo - (InVariance * 10);
            //ToTrace = WMasterTraceNo + (InVariance * 10);

            //int Mode = 4; // range based on Specific

            //NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator,  0, WAtmNo, FromTrace, ToTrace, Mpa.TransDate.Date, NullPastDate ,Mode);
            //NForm67.Show();
            int TraceFrom;
            int TraceTo;
            int SaveSeqNo = Mpa.SeqNo;
            DateTime WDt = Mpa.TransDate;

            DateTime WDtA;
            DateTime WDtB;

            SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                              + "' AND Origin = 'Our Atms' AND TransDate < @TransDate ";
            string OrderBy = "  ORDER By TransDate Desc";
            Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);

            if (Mpa.RecordFound == true)
            {
                TraceFrom = Mpa.AtmTraceNo;
                WDtA = Mpa.TransDate;

                // Find the smallest Fuid, Ruid for this 
            }
            else
            {
                MessageBox.Show("No Journal Lines to Show");
                // Reestablish Mpa Data
                //
                SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);
                return;
            }

            SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                              + "' AND Origin = 'Our Atms' AND TransDate > @TransDate  ";
            OrderBy = " ORDER By TransDate Asc";
            Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);

            if (Mpa.RecordFound == true)
            {
                TraceTo = Mpa.AtmTraceNo;
                WDtB = Mpa.TransDate;
                // Find the largest Fuid, Ruid for this
            }
            else
            {
                MessageBox.Show("No Journal Lines to Show");
                // Reestablish Mpa Data
                //
                SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);

                return;
            }

            // Reestablish Mpa Data
            //
            SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);

            Form67 NForm67;

            int Mode = 5; // Range Fuid , Ruid 

            //NForm67 = new Form67(WSignedId, 0, WOperator, 0, Mpa.TerminalId, TraceFrom, TraceTo, Mpa.TransDate.Date, Mode);
            NForm67 = new Form67(WSignedId, 0, WOperator, 0, Mpa.TerminalId, TraceFrom, TraceTo, WDtA, WDtB, Mode);
            NForm67.ShowDialog();
        }
        // Print All 
        private void button1_Click(object sender, EventArgs e)
        {
            if (WAction == 21) // Print TRANSACTIONS PER SESSION
            {
                //Transfilter = "SesNo=" + WSesNo;
                NForm56R6 = new Form56R6(WOperator, WAtmNo, WSesNo);
                NForm56R6.Show();           
            }
        }
        // DATES TRAIL 
        private void button6_Click(object sender, EventArgs e)
        {
            NForm84 = new Form84(WSignedId, WSignRecordNo, WOperator, WAtmNo, WMasterTraceNo, WUniqueRecordId);
            NForm84.Show();
        }
// Show More Journal 
        private void buttonPrintTrace_Click(object sender, EventArgs e)
        {
            //Ej.ReadJournalTextByTrace(WOperator, WAtmNo, WMasterTraceNo, Mpa.TransDate.Date);

            //WPrintTraceDtTm = Ej.TransDate.ToString();
            //WPrintTrace = WMasterTraceNo.ToString();

            //// TRACE
            //// Show all lines for a TRACE NUMBER  for a specific ATM

            //Form56R16 PrintJournal = new Form56R16(WOperator, WPrintTrace, WPrintTraceDtTm, WAtmNo, WSignedId);
            //PrintJournal.Show();
            Form67 NForm67;

            int Mode = 5; // Specific

            //NForm67 = new Form67(WSignedId, 0, WOperator, 0, Mpa.TerminalId, Mpa.AtmTraceNo, Mpa.AtmTraceNo, Mpa.TransDate.Date, Mode);
            NForm67 = new Form67(WSignedId, 0, WOperator, 0, Mpa.TerminalId, Mpa.AtmTraceNo, Mpa.AtmTraceNo, Mpa.TransDate.Date, NullPastDate, Mode);
            NForm67.ShowDialog();
        }
        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridTransactions()
        {
            // Keep Scroll position 
            int scrollPosition = 0;
            if (WRowGrid1 > 0)
            {
                scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            }
          
            //this.reconcSourceFilesTableAdapter.Fill(this.aTMSDataSet63.ReconcSourceFiles);

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Transactions to Show for this Replenishement Cycle."); 
                this.Dispose();
                return;
            }
            else
            {
                textBoxTotal.Text = dataGridView1.Rows.Count.ToString(); 
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // MaskRecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form62Grd1Cl0", culture);

            dataGridView1.Columns[1].Width = 40; // Status
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; //  Done
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 70; // Terminal
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 50; // Terminal Type, ATM, POS etc 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 90; // Descr
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 40; // Err
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 40; // Mask
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 90; // Account
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[9].Width = 50; // Ccy
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 80; // Amount
            dataGridView1.Columns[10].DefaultCellStyle = style;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[10].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[11].Width = 120; // Date
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[12].Width = 70; // ActionType
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
          
        }

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridTraceJournalLines() 
        {        
            dataGridView2.DataSource = Ej.TraceJournalLinesDataTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }
            dataGridView2.Columns[0].Width = 50; // TranNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].Width = 300; // Journal Line 
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
           

            // DATA TABLE ROWS DEFINITION 
            //TraceJournalLinesDataTable.Columns.Add("LineNo", typeof(int));
            //TraceJournalLinesDataTable.Columns.Add("Journal Line", typeof(string));
        }
// Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
    }
 
    


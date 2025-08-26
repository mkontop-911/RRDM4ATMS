using System;

using System.Windows.Forms;
using RRDM4ATMs;
using System.Data;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;
using System.Drawing;


namespace RRDM4ATMsWin
{
    public partial class Form62 : Form 
    {
        
        Form67 NForm67;

        Form84 NForm84;

        Form56R6 NForm56R6;
       
        int WRowGrid1; 

        int WUniqueRecordId;
        //int WTraceNo;

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
          
        public Form62(string InSignedId, int InSignRecordNo, string InOperator,string InAtmNo,
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
                label6.Hide();
                textBoxPage.Hide();
                buttonNext.Hide();
                buttonPrevious.Hide();
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
        int WEntriesRead ;
    
        int WPageSize ;

        private void Form62_Load(object sender, EventArgs e)
        {
            // InMode = 1 then selection criteria and no dates
            // InMode = 2 then single date exist 
            // InMode = 3 then range of dates exist 
            if (int.TryParse(textBoxPageSize.Text, out WPageSize))
            {
            };

            if (WAction == 21) // SHOW TRANSACTIONS PER SESSION
            {

                WEntriesRead = 0;
                ReadNextPageAndFileTable(WPageSize, WEntriesRead); 

            }

            if (WAction == 22) // SHOW RANGE 
            {    
               
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

        public DataTable PageTableFirstLast = new DataTable();
        int IndexInPageTable;

        // Show Next Page 

        private void buttonNext_Click(object sender, EventArgs e)
        {
           
            if (int.TryParse(textBoxPageSize.Text, out WPageSize))
            {
            }
            else
            {
                MessageBox.Show(textBoxPageSize.Text, "Please enter a valid page number!");

                return;
            }
            ReadNextPageAndFileTable(WPageSize, WEntriesRead); 
        }

        // Previous page 
        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxPageSize.Text, out WPageSize))
            {
            }
            else
            {
                MessageBox.Show(textBoxPageSize.Text, "Please enter a valid page number!");

                return;
            }

            int I = IndexInPageTable -2;

            if (I==0)
            {
                buttonPrevious.Hide(); 
            }

            buttonNext.Show();

            int CurrentPage = (int)PageTableFirstLast.Rows[I]["PageNo"]; 
            int PageFirstSeqNo = (int)PageTableFirstLast.Rows[I]["PageFirstSeqNo"];
            int PageLastSeqNo = (int)PageTableFirstLast.Rows[I]["PageLastSeqNo"];
            WEntriesRead = (int)PageTableFirstLast.Rows[I]["ReadTillNow"];
            int GrandTotal = (int)PageTableFirstLast.Rows[I]["GrandTotal"];

            int pageCount = (Mpa.NumberOfRecords + WPageSize - 1) / WPageSize;
            textBoxPage.Text = CurrentPage.ToString() + "/" + pageCount.ToString();
           
            WMode = 1;
            SelectionCriteria = " Where TerminalId ='" + WAtmNo + "' AND ReplCycleNo =" + WSesNo
                             + " AND SeqNo >= " + PageFirstSeqNo;

            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_Paging(WOperator, WSignedId, WMode,
                SelectionCriteria, "", NullPastDate, NullPastDate, WPageSize,2);

            IndexInPageTable = IndexInPageTable - 1;
         
            ShowGridTransactions();
        }

        // Define next 
      
        private void ReadNextPageAndFileTable(int InPageSize, int InEntriesRead)
        {
            if (InEntriesRead == 0)
            {
                // FIND Max Number of records 
                WMode = 1;
                SelectionCriteria = " Where TerminalId ='" + WAtmNo + "' AND ReplCycleNo =" + WSesNo;

                Mpa.ReadMatchingTxnsMasterPoolBySelection_Paging_Max(WOperator, WSignedId, WMode,
                    SelectionCriteria, "", NullPastDate, NullPastDate, InPageSize);

                if (Mpa.NumberOfRecords == 0 )
                {
                    MessageBox.Show("No Entries to show");
                    return;
                }

                // Fill table 
                PageTableFirstLast = new DataTable();
                PageTableFirstLast.Clear();
                IndexInPageTable = 0;

                PageTableFirstLast.Columns.Add("PageNo", typeof(int));
                PageTableFirstLast.Columns.Add("PageFirstSeqNo", typeof(int));
                PageTableFirstLast.Columns.Add("PageLastSeqNo", typeof(int));
                PageTableFirstLast.Columns.Add("ReadTillNow", typeof(int));
                PageTableFirstLast.Columns.Add("GrandTotal", typeof(int));

                // Compile Command 
                WMode = 1;
                SelectionCriteria = " Where TerminalId ='" + WAtmNo + "' AND ReplCycleNo =" + WSesNo;

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_Paging(WOperator, WSignedId, WMode,
                    SelectionCriteria, "", NullPastDate, NullPastDate, InPageSize,2);
            
                    buttonPrevious.Hide();
                    buttonNext.Show();                     
           
            }
            else
            {

                WMode = 1;
                SelectionCriteria = " Where TerminalId ='" + WAtmNo + "' AND ReplCycleNo =" + WSesNo
                                 + " AND SeqNo > " + Mpa.PageLastSeqNo;

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_Paging(WOperator, WSignedId, WMode,
                    SelectionCriteria, "", NullPastDate, NullPastDate, InPageSize,2);

                if (Mpa.TotalSelected == 0)
                {
                    MessageBox.Show("No More to show");
                    return; 
                }
                else
                {
                  
                }
            }
        
            WEntriesRead = WEntriesRead + Mpa.TotalSelected;

            if (WEntriesRead == Mpa.NumberOfRecords)
            {
                buttonNext.Hide();
            }
            else
            {
                buttonNext.Show();
            }
            // Add Entries to table 

            if (IndexInPageTable > (PageTableFirstLast.Rows.Count-1))
            {
                DataRow RowSelected = PageTableFirstLast.NewRow();
            
                RowSelected["PageNo"] = IndexInPageTable +1 ;
                RowSelected["PageFirstSeqNo"] = Mpa.PageFirstSeqNo;
                RowSelected["PageLastSeqNo"] = Mpa.PageLastSeqNo;
                RowSelected["ReadTillNow"] = WEntriesRead;
                RowSelected["GrandTotal"] = Mpa.NumberOfRecords;

                // ADD ROW

                PageTableFirstLast.Rows.Add(RowSelected);
            }

            if (IndexInPageTable > 0)
            {
                buttonPrevious.Show(); 
            }      

            IndexInPageTable = IndexInPageTable + 1;

            int pageCount = (Mpa.NumberOfRecords + InPageSize - 1) / InPageSize;
            textBoxPage.Text = IndexInPageTable.ToString()+ "/"+ pageCount.ToString();

            ShowGridTransactions();
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

            if (Mpa.MatchMask == "001"
                || Mpa.MatchMask == "011"
                || Mpa.MatchMask == "010"
                || Mpa.MatchMask == "01"
                )
            {
                MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
                                 + "Select Journal Lines Near To this"
                                 );
                return;
            }

            int WSeqNoA = 0;
            
            if (Mpa.TraceNoWithNoEndZero == 0)
            {
                MessageBox.Show("No Available Trace to show the Journal Lines for this Txn/Category ");
                return;
            }
            else
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;
               
            }

         
            int Fuid_A = 0;
            int Ruid_A = 0;
            int Fuid_B = 0;
            int Ruid_B = 0;

                Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoA, 1);

                if (Ej.RecordFound == true)
                {
                    Fuid_A = Ej.FuId;
                    Ruid_A = Ej.Sessionstart;

                    Fuid_B = Fuid_A;
                    Ruid_B = Ej.SessionEnd;
                //
                Ej.CreateJournalLinesBasedonGivenFuid(WOperator, WSignedId, WAtmNo, Fuid_A);
                    if (Ej.RecordFound == true)
                    {
                        // OK
                    }
                    else
                    {
                        MessageBox.Show("Journal Is not created");
                        this.Dispose();
                        return;
                    }
                }
                
                    label14.Text = "Journal Lines for ATM : " + WAtmNo
                        + ".. And File Id .." + Fuid_A.ToString() + " And lines start.." + Ruid_A.ToString();
                    // label1.Hide();
                
                // Fill UP TABLE AND REPORT 
                //
                Ej.ReadJournalAndFillTableFrom_Fuid_Ruid_To_Fuid_Ruid(WOperator, WSignedId, WAtmNo
                     , Fuid_A, Ruid_A, Fuid_B, Ruid_B, 5);
                bool FoundRecord; 
                if (Ej.JournalLines.Rows.Count > 0)
                {
                    FoundRecord = true;

                //ShowGridTraceJournalLines();

                label3.Text = "ATM Journal for trace :" + Mpa.TraceNoWithNoEndZero.ToString();

                ShowGrid();
                }
                else
                {
                    FoundRecord = false;
                    MessageBox.Show("No Data to show for this selection. ");
                    this.Dispose();
                }      

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
            // Show Lines of journal 
            string SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            if (Mpa.MatchMask == "001"
                || Mpa.MatchMask == "011"
                || Mpa.MatchMask == "010"
                || Mpa.MatchMask == "01"
                )
            {
                MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
                                 + "Select Journal Lines Near To this"
                                 );
                return;
            }

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            if (Mpa.TraceNoWithNoEndZero == 0)
            {
                MessageBox.Show("No Available Trace to show the Journal Lines for this Txn/Category ");
                return;
            }
            else
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;
                WSeqNoB = Mpa.OriginalRecordId;
            }

            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();
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
        //private void ShowGridTraceJournalLines() 
        //{        
        //    dataGridView2.DataSource = Ej.TraceJournalLinesDataTable.DefaultView;

        //    if (dataGridView2.Rows.Count == 0)
        //    {
        //        return;
        //    }
        //    dataGridView2.Columns[0].Width = 50; // TranNo
        //    dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dataGridView2.Columns[1].Width = 300; // Journal Line 
        //    dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
           

        //    // DATA TABLE ROWS DEFINITION 
        //    //TraceJournalLinesDataTable.Columns.Add("LineNo", typeof(int));
        //    //TraceJournalLinesDataTable.Columns.Add("Journal Line", typeof(string));
        //}


        private void ShowGrid()
        {
            bool WSingle = true;
            string WTraceOrRRNumber = Mpa.TraceNoWithNoEndZero.ToString(); 
            dataGridView2.DataSource = Ej.JournalLines.DefaultView;
            dataGridView2.Show();

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView2.Columns[0].Width = 90; // AtmNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 90; // Journal_id
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 70; // Journal_LN
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 1500; // TxtLine
            dataGridView2.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            int scrollPosition = 0;
            int Count = 0;
            string hj;
            string WTextLine;
            int Journal_LN;
            int Journal_Id;
            try
            {
                bool FirstTime = true;

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {


                    if (string.IsNullOrEmpty((string)row.Cells[3].Value))
                    {
                        Count++;
                        continue;
                    }
                    else
                    {
                        // 
                        WTextLine = (string)row.Cells[3].Value;
                        Journal_LN = (int)row.Cells[2].Value;
                        Journal_Id = (int)row.Cells[1].Value;
                        Count++;
                    }
                    if (WTextLine.Length > 5 & WTextLine != null)
                    {
                        if (WTextLine.Contains("TRANSACTION STARTED") || WTextLine.Contains("TRANSACTION START")
                            || WTextLine.Contains("TRANSACTION END"))
                        {
                            row.DefaultCellStyle.BackColor = Color.Gainsboro;
                            row.DefaultCellStyle.ForeColor = Color.Black;

                        }
                        if (WTextLine.Contains("HOST TX TIMEOUT"))
                        {
                            MessageBox.Show("There is <HOST TX TIMEOUT> Will be shown in RED");
                            row.DefaultCellStyle.BackColor = Color.Red;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                        }

                        if (WSingle == true)
                        {
                            if (WTextLine.Contains(WTraceOrRRNumber))
                            {
                                row.DefaultCellStyle.BackColor = Color.Aqua;
                                row.DefaultCellStyle.ForeColor = Color.Black;
                            }
                            else
                            {
                                if (WTraceOrRRNumber.Length > 5)
                                {
                                    hj = WTraceOrRRNumber.Substring(2, 4);
                                    if (WTextLine.Contains(hj))
                                    {
                                        // row.DefaultCellStyle.BackColor = Color.AliceBlue;
                                        row.DefaultCellStyle.BackColor = Color.Aqua;
                                        row.DefaultCellStyle.ForeColor = Color.Black;

                                        dataGridView1.Rows[Count - 1].Selected = true;

                                    }
                                }
                                else
                                {

                                }
                            }
                        }


                    }
                    else
                    {
                        //MessageBox.Show("Hello New World"); 
                    }
                }
            }
            catch (Exception ex)
            {
              //  CatchDetails(ex, true);
            }

        }

        // Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

    }
    }
 
    


using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;
using System.Text;

using System.Drawing;

namespace RRDM4ATMsWin
{
    public partial class Form67 : Form
    {

        int Fuid_A;
        int Ruid_A;
        
        int Fuid_B;
        int Ruid_B;

        public bool FoundRecord;

        string WPrintTrace;
        string WPrintTraceDtTm;

        RRDMJournalReadTxns_Text_Class Ej = new RRDMJournalReadTxns_Text_Class();

        //DataTable JournalLines = new DataTable();

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //multilingual
        CultureInfo culture;

        RRDMUsersRecords Xa = new RRDMUsersRecords(); // Make class availble 
        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        //string SqlString;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

      //  string WJournalId;
        int WFileInJournal;
        string WAtmNo;

        int WTraceStart;
        int WTraceEnd;

        DateTime WDate_A;
        DateTime WDate_B;

        int WMode;

        public Form67(string InSignedId, int InSignRecordNo, string InOperator, int InFileInJournal,
                   string InAtmNo, int InTraceStart, int InTraceEnd, DateTime InDateA, DateTime InDateB, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WFileInJournal = InFileInJournal;
            WAtmNo = InAtmNo;

            WTraceStart = InTraceStart;
            WTraceEnd = InTraceEnd;

            WDate_A = InDateA;
            WDate_B = InDateB;

            WMode = InMode; // Mode 1 = single trace ---- Mode = 2 Whole Journal .... Mode = 3 working file in Journal
                            // Mode = 4 Range of traces (from .. to) 
                            // Mode = 5 Range of Fuid, Ruid 


            FoundRecord = false;

            if (WMode == 1 || WMode == 2) // Check last digit if different than zero then this comes from Supervisor Mode. 
            // Turn last digit to one. 
            {
                // Check START 
                Int32 LastDigit = WTraceStart % 10;

                if (LastDigit == 0)
                {
                    // OK
                }
                else
                {
                    WTraceStart = (WTraceStart - LastDigit) + 1;
                }

                // Check End 
                LastDigit = WTraceEnd % 10;

                if (LastDigit == 0)
                {
                    // OK
                }
                else
                {
                    WTraceEnd = (WTraceEnd - LastDigit) + 1;
                }
            }

            InitializeComponent();

            if (WMode == 5)
            {
                Ej.ReadJournalTxnsByTraceAndFind_Start_End(WOperator, WAtmNo, WTraceStart
                                                   , WDate_A);
                if (Ej.RecordFound == true)
                {
                    Fuid_A = Ej.FuId;
                    Ruid_A = Ej.StartTxn;
                   
                }
                if (WTraceStart != WTraceEnd)
                {
                    // Different Trace 
                    Ej.ReadJournalTxnsByTraceAndFind_Start_End(WOperator, WAtmNo, WTraceEnd
                                                                     , WDate_B);
                    if (Ej.RecordFound == true)
                    {
                        Fuid_B = Ej.FuId;
                        Ruid_B = Ej.EndTxn;
                      
                    }
                }
                else
                {
                    // Same Trace
                    // Get Info from previous read 
                    Fuid_B = Ej.FuId;
                    Ruid_B = Ej.EndTxn;
                   
                }
                //

            }

            if (WOperator == "CRBAGRAA")
            {

                //WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            }

            if (WOperator == "ETHNCY2N")
            {
               // WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            }

            textBoxTraceNo.Text = WTraceStart.ToString();

        }
        //
        // With LOAD LOAD DATA GRID
        //
        private void Form67_Load(object sender, EventArgs e)
        {
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

            if (WMode == 1)
            {
                label14.Text = LocRM.GetString("Form67label14a", culture);
                label1.Text = WTraceStart.ToString();
            }
            if (WMode == 2)
            {
                //label14.Text = LocRM.GetString("Form67label14b", culture);
                label14.Text = "Journal Lines for ATM : " + WAtmNo;
                //label1.Text = WSesNo.ToString(); 
                label1.Hide();
            }

            if (WMode == 3)
            {
                Ej.ReadJournalAndFillTableFrom_Fuid(WOperator, WSignedId, WAtmNo
                                                      , WFileInJournal, WMode);

                if (Ej.JournalLines.Rows.Count > 0)
                {
                    FoundRecord = true;

                    dataGridView1.DataSource = Ej.JournalLines.DefaultView;
                    dataGridView1.Show();

                    ShowGrid();
                }
                else
                {
                    FoundRecord = false;
                    MessageBox.Show("No Data to show for this selection. ");
                    this.Dispose();
                }

                label14.Text = "Journal Lines for ATM : " + WAtmNo + " For File ID:.." + WFileInJournal.ToString();
                label1.Hide();
            }

            if (WMode == 5)
            {
                if (WTraceStart != WTraceEnd)
                {
                    label14.Text = "Journal Lines for ATM : " + WAtmNo + " And from Trace " + (WTraceStart / 10).ToString() + " To Trace " + (WTraceEnd / 10).ToString();
                    label1.Hide();
                }
                else
                {
                    label14.Text = "Journal Lines for ATM : " + WAtmNo + " And Trace " + (WTraceStart / 10).ToString();
                    label1.Hide();
                }

                // Fill UP TABLE AND REPORT 
                //
                Ej.ReadJournalAndFillTableFrom_Fuid_Ruid_To_Fuid_Ruid(WOperator, WSignedId, WAtmNo
                     , Fuid_A, Ruid_A, Fuid_B, Ruid_B, WMode );

                if (Ej.JournalLines.Rows.Count > 0)
                {
                    FoundRecord = true;

                    dataGridView1.DataSource = Ej.JournalLines.DefaultView;
                    dataGridView1.Show();

                    ShowGrid();
                }
                else
                {
                    FoundRecord = false;
                    MessageBox.Show("No Data to show for this selection. ");
                    this.Dispose();
                }

            }

        }



        // On Row Enter
        DateTime LineDate;
        string TraceNo;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            TraceNo = (string)rowSelected.Cells[5].Value;

            LineDate = (DateTime)rowSelected.Cells[4].Value;

            textBoxTraceNo.Text = TraceNo;

            if (WMode == 3)
            {
                label14.Text = "JOURNAL LINES FOR ATM : " + WAtmNo + " WITH JOURNAL ID:.." + WFileInJournal.ToString();
                label1.Hide();
            }
        }

        // Show Grid
        private void ShowGrid()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 60; // Journal Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 60; // Journal Line
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 70; //Atmno
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 470; // txt Line
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 70; // date
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 70; // trace 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 90; // Descr
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 50; // Ccy
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 90; // amount
            dataGridView1.Columns[8].DefaultCellStyle = style;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
        }

        // Print Trace Journal 
        private void buttonPrintTrace_Click(object sender, EventArgs e)
        {
            if (textBoxTraceNo.Text == "0")
            {
                MessageBox.Show("This line has no Trace Number");
                return;
            }
           
            WPrintTraceDtTm = LineDate.ToString();
            WPrintTrace = textBoxTraceNo.Text;
            // TRACE
            // Show all lines for a TRACE NUMBER  for a specific ATM

            Form56R16 PrintJournal = new Form56R16(WOperator, WPrintTrace, WPrintTraceDtTm, WAtmNo, WSignedId);
            PrintJournal.Show();
        }

        //// Creat Print File 
        //private void CreateReportTable() 
        //{
        //    DataTable JournalLinesSelected = new DataTable();
        //    JournalLinesSelected = new DataTable();
        //    JournalLinesSelected.Clear();

        //    JournalLinesSelected.Columns.Add("UserId", typeof(string));
        //    JournalLinesSelected.Columns.Add("Journal_id", typeof(int));
        //    JournalLinesSelected.Columns.Add("Journal_LN", typeof(int));
        //    JournalLinesSelected.Columns.Add("TxtLine", typeof(string));
        //    JournalLinesSelected.Columns.Add("TraceNo", typeof(string));


        //    ////ReadTable And Insert In Sql Table 
        //    RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

        //        //Clear Table 
        //        Tr.DeleteReport75(WSignedId);

        //        int I = 0;

        //        while (I <= (JournalLines.Rows.Count - 1))
        //        {
        //        //    RecordFound = true;
        //        //"SELECT ISNULL(fuid, 0) AS Journal_id, ISNULL(Ruid, 0) AS Journal_LN,  AtmNo, ISNULL(TxtLine, '') AS TxtLine, ISNULL(TransDate,'1901-01-01') AS Tr_Date, "
        //        //      + "   ISNULL(TraceNo, 0) AS TraceNo, "
        //        //      + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
        //        //      + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
        //        int Journal_id = (int)JournalLines.Rows[I]["Journal_id"];
        //        int Journal_LN = (int)JournalLines.Rows[I]["Journal_LN"];
        //        string TxtLine = (string)JournalLines.Rows[I]["TxtLine"];
        //        string TraceNo = (string)JournalLines.Rows[I]["TraceNo"];

        //        // Fill In Table
        //        //
        //        DataRow RowSelected = JournalLinesSelected.NewRow();

        //        RowSelected["UserId"] = WSignedId;
        //        RowSelected["Journal_id"] = Journal_id;
        //        RowSelected["Journal_LN"] = Journal_LN;
        //        RowSelected["TxtLine"] = TxtLine;
        //        RowSelected["TraceNo"] = TraceNo; 

        //        // ADD ROW
        //        JournalLinesSelected.Rows.Add(RowSelected);

        //        I++; // Read Next entry of the table 

        //        }

        //    //
        //    //Insert Records For Report WReport75
        //    //
        //    using (SqlConnection conn2 =
        //                   new SqlConnection(connectionString))
        //        try
        //        {
        //            conn2.Open();

        //            using (SqlBulkCopy s = new SqlBulkCopy(conn2))
        //            {
        //                s.DestinationTableName = "[ATMS].[dbo].[WReport75]";

        //                foreach (var column in JournalLinesSelected.Columns)
        //                    s.ColumnMappings.Add(column.ToString(), column.ToString());

        //                s.WriteToServer(JournalLinesSelected);
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            conn2.Close();

        //            CatchDetails(ex);
        //        }


        //}

        // Finish 
        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        //
        // Catch Details
        //
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

            //    Environment.Exit(0);
        }

        private void buttonJournal_Click(object sender, EventArgs e)
        {

        }
    }
}

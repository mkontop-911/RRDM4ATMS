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
    public partial class Form67_BDC : Form
    {

        int Fuid_A;
        int Ruid_A;
        int Fuid_B;
        int Ruid_B;

        public bool FoundRecord;

        string WPrintTraceDtTm;

        RRDMJournalReadTxns_Text_Class Ej = new RRDMJournalReadTxns_Text_Class();
        RRDMSessions_Form153_Corrections Sc = new RRDMSessions_Form153_Corrections();

        DateTime HST_DATE;
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool In_HST ;

        //multilingual
        CultureInfo culture;

        // RRDMUsersRecords Xa = new RRDMUsersRecords(); // Make class availble 
        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        //  string WJournalId;
        int WFuid;
        string WAtmNo;

        int WSeqNoA;
        int WSeqNoB;

        bool NoUpperLimit;

        bool WSingle;

        DateTime WDate_A;
        DateTime WDate_B;

        int WMode;

        string WTraceOrRRNumber;

        public Form67_BDC(string InSignedId, int InSignRecordNo, string InOperator, int InFuid, string InTraceOrRRNumber
                   , string InAtmNo, int InSeqNoA, int InSeqNoB, DateTime InDateA, DateTime InDateB, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WFuid = InFuid; // THIS IS THE FUI Number 
            WAtmNo = InAtmNo;

            WSeqNoA = InSeqNoA;
            WSeqNoB = InSeqNoB;

            WDate_A = InDateA;
            WDate_B = InDateB;

            WMode = InMode; // Mode 1 = single trace OR USE 5 For single range 
                            // ---- Mode = 2 Whole Journal .... 
                            // Mode = 3 working file in Journal
                            // Mode = 4 Range of traces (from .. to) 
                            // Mode = 5 Range of Fuid, Ruid ... It can also be used to get range 
                            // Mode = 7 Given Fuid and Given line START within and Line END 
                            // For 7 the Ruid1 = SeqNoA and Ruid2 = SeqNoB
                            // For 9 Search within the DataGridview 
                            // 10 Show Changes done through FORM154 - For this ATM 


            InitializeComponent();

            // Check if in history 

            if (WMode == 10)
            {
                buttonExcel.Show();
            }
            else
            {
                buttonExcel.Hide();
            }

            RRDMGasParameters Gp = new RRDMGasParameters(); 

            string ParamId = "853";
            string OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true & Gp.OccuranceNm != "")
            {
                if (DateTime.TryParseExact(Gp.OccuranceNm, "yyyy-MM-dd", CultureInfo.InvariantCulture
                              , System.Globalization.DateTimeStyles.None, out HST_DATE))
                {

                }
            }
            else
            {
                HST_DATE = NullPastDate;
            }

            In_HST = false;

            if (InDateA.Date <= HST_DATE)
            {
                In_HST = true;
            }

            // END Of Checking
            if (InSeqNoB == 0)
            {
                NoUpperLimit = true;
            }
            else
            {
                NoUpperLimit = false;
            }

            WTraceOrRRNumber = InTraceOrRRNumber;

            if (WMode == 5 & WSeqNoA == WSeqNoB)
            {
                WSingle = true;
            }
            if (WMode == 5 & WSeqNoA != WSeqNoB)
            {
                WSingle = false;
            }
            if (WMode == 7)
            {
                Ruid_A = WSeqNoA;
                Ruid_B = WSeqNoB;
            }

            FoundRecord = false;

            if (WOperator == "CRBAGRAA")
            {

                //WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            }

            if (WOperator == "ETHNCY2N")
            {
                // WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            }

            //   textBoxTraceNo.Text = WSeqNoA.ToString();

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
                labelHeader.Text = LocRM.GetString("Form67label14a", culture);
                //   label1.Text = WSeqNoA.ToString();
            }
            if (WMode == 2)
            {
                //label14.Text = LocRM.GetString("Form67label14b", culture);
                labelHeader.Text = "Journal Lines for ATM : " + WAtmNo;
                //label1.Text = WSesNo.ToString(); 
                //  label1.Hide();
            }

            if (WMode == 3)
            {

                Ej.CreateJournalLinesBasedonGivenFuid(WOperator, WSignedId, WAtmNo, WFuid);

                Ej.ReadJournalAndFillTableFrom_Fuid_Short(WOperator, WSignedId, WAtmNo
                                                      , WFuid, WMode);

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

                labelHeader.Text = "Journal Lines for ATM : " + WAtmNo + " For File ID:.." + WFuid.ToString();
                // label1.Hide();
            }

            if (WMode == 5 & NoUpperLimit == false)
            {
                if (In_HST == true)
                {
                    Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoA, 2);
                }
                else
                {
                    Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoA, 1);
                }
               
                //Ej.ReadJournalTxnsByTraceAndFind_Start_End(WOperator, WAtmNo, WTraceStart
                //                                   , WDate_A);
                if (Ej.RecordFound == true)
                {
                    Fuid_A = Ej.FuId;
                    Ruid_A = Ej.Sessionstart;
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
                if (WSeqNoA != WSeqNoB)
                {
                    // Different Trace 
                    if (In_HST == true)
                    {
                        Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoB, 2);
                    }
                    else
                    {
                        Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoB, 1);
                    }
                    //Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoB,1);
                    if (Ej.RecordFound == true)
                    {
                        Fuid_B = Ej.FuId;
                        Ruid_B = Ej.SessionEnd;
                        //
                        if (Fuid_B != Fuid_A)
                        {
                            Ej.CreateJournalLinesBasedonGivenFuid(WOperator, WSignedId, WAtmNo, Fuid_B);
                        }

                    }
                }
                else
                {
                    // Same Trace
                    // Get Info from previous read 
                    Fuid_B = Ej.FuId;
                    Ruid_B = Ej.SessionEnd;
                }
                //

                if (WSeqNoA != WSeqNoB)
                {
                    labelHeader.Text = "Journal Lines for ATM : " + WAtmNo + "..And Near To.." + WDate_A.ToString() + "..And to Trace .." + WTraceOrRRNumber;
                    //    label1.Hide();
                }
                else
                {
                    labelHeader.Text = "Journal Lines for ATM : " + WAtmNo
                        + ".. And File Id .." + Fuid_A.ToString() + " And lines start.." + Ruid_A.ToString();
                    // label1.Hide();
                }

                // Fill UP TABLE AND REPORT 
                //
                Ej.ReadJournalAndFillTableFrom_Fuid_Ruid_To_Fuid_Ruid(WOperator, WSignedId, WAtmNo
                     , Fuid_A, Ruid_A, Fuid_B, Ruid_B, WMode);

                if (Ej.JournalLines.Rows.Count > 0)
                {
                    FoundRecord = true;

                    ShowGrid();
                }
                else
                {
                    FoundRecord = false;
                    MessageBox.Show("No Data to show for this selection. ");
                    this.Dispose();
                }

            }

            if (WMode == 5 & NoUpperLimit == true)
            {
                if (In_HST == true)
                {
                    Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoA, 2);
                }
                else
                {
                    Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoA, 1);
                }
                //Ej.ReadJournalTxnsBySeqNoAndFind_Start_End(WOperator, WSeqNoA, 1 );

                //Ej.ReadJournalTxnsByTraceAndFind_Start_End(WOperator, WAtmNo, WTraceStart
                //                                   , WDate_A);
                if (Ej.RecordFound == true)
                {
                    Fuid_A = Ej.FuId;
                    Ruid_A = Ej.Sessionstart;
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

                // Same Trace
                // Get Info from previous read 
                Fuid_B = Ej.FuId;
                Ruid_B = Ej.ReadJournalByFuidAndFindNumberOfLines(Ej.FuId);

                //
                labelHeader.Text = "Journal Lines for ATM : " + WAtmNo + "..And Near To.." + WDate_A.ToString() + "..And to Trace .." + WTraceOrRRNumber;
                //    label1.Hide();

                // Fill UP TABLE AND REPORT 
                //
                Ej.ReadJournalAndFillTableFrom_Fuid_Ruid_To_Fuid_Ruid(WOperator, WSignedId, WAtmNo
                     , Fuid_A, Ruid_A, Fuid_B, Ruid_B, WMode);

                if (Ej.JournalLines.Rows.Count > 0)
                {
                    FoundRecord = true;

                    ShowGrid();
                }
                else
                {
                    FoundRecord = false;
                    MessageBox.Show("No Data to show for this selection. " + Environment.NewLine
                        + "" + Environment.NewLine
                        + "Is it a new ATM with incomplete journals?" + Environment.NewLine
                        );
                    this.Dispose();
                }

            }
            if (WMode == 7)
            {
                Ej.CreateJournalLinesBasedonGivenFuid(WOperator, WSignedId, WAtmNo, WFuid);

                // Fill UP TABLE AND REPORT 
                //
                Ej.ReadJournalAndFillTableFrom_Fuid_Ruid_To_Fuid_Ruid(WOperator, WSignedId, WAtmNo
                     , WFuid, Ruid_A, WFuid, Ruid_B, WMode);

                if (Ej.JournalLines.Rows.Count > 0)
                {
                    FoundRecord = true;


                    ShowGrid();
                }
                else
                {
                    FoundRecord = false;
                    MessageBox.Show("No Data to show for this selection. ");
                    this.Dispose();
                }

            }

            if (WMode == 10)
            {

                // SHOW USE OF FORM153
               
                Sc.ReadSessions_Form153_Corrections_Fill_Table(WAtmNo); 
                textBoxFuidAndLine.Hide();

                textBox1.Hide(); 
                textBoxFuidAndLine.Hide();
                textBox2.Hide();
                button1.Hide();
                buttonShow.Hide(); 

                labelHeader.Text = "Use of Form154 for ATM.." + WAtmNo ; 
                dataGridView1.DataSource = Sc.ChangesInForm153Table.DefaultView;
                dataGridView1.Show();

                if (Sc.ChangesInForm153Table.Rows.Count > 0)
                {
                    dataGridView1.DataSource = Sc.ChangesInForm153Table.DefaultView;
                    dataGridView1.Show();
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
        // string TraceNo;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            if (WMode == 10)
            {
                // This is for Form53
            }
            else
            {
                int Fuid = (int)rowSelected.Cells[1].Value;

                RRDMReconcFileMonitorLog Rf = new RRDMReconcFileMonitorLog();

                Rf.GetRecordByFuid(Fuid);

                textBox2.Text = Rf.FileName;
            }

            // LineDate = (DateTime)rowSelected.Cells[4].Value;

            //textBoxTraceNo.Text = TraceNo;

            //if (WMode == 3)
            //{
            //    label14.Text = "JOURNAL LINES FOR ATM : " + WAtmNo + " WITH JOURNAL ID:.." + WFuid.ToString();
            //    label1.Hide();
            //}
        }

        // Show Grid

        private void ShowGrid()
        {
            textBoxFuidAndLine.Hide();
            
            dataGridView1.DataSource = Ej.JournalLines.DefaultView;
            dataGridView1.Show();

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 90; // AtmNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 90; // Journal_id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 70; // Journal_LN
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 1500; // TxtLine
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            int scrollPosition = 0;
            int Count = 0;
            string hj;
            string WTextLine;
            int Journal_LN;
            int Journal_Id;
            try
            {
                
                bool FirstTimeout = true; // within selection might be many time outs
                                          // You show message just for the one

                foreach (DataGridViewRow row in dataGridView1.Rows)
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
                            
                            if (FirstTimeout == true)
                            {
                                MessageBox.Show("There is <HOST TX TIMEOUT> Will be shown in RED");
                                FirstTimeout = false; 
                            }
                            
                            row.DefaultCellStyle.BackColor = Color.Red;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                        }

                        if (WSingle == true)
                        {
                            if (WTextLine.Contains(WTraceOrRRNumber))
                            {
                                row.DefaultCellStyle.BackColor = Color.Aqua;
                                row.DefaultCellStyle.ForeColor = Color.Black;

                                textBoxFuidAndLine.Show();
                                textBoxFuidAndLine.Text = "ID:..." + Journal_Id.ToString() + ".......LN:..." + Journal_LN.ToString();

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
                        else
                        {
                            if (WMode == 5)
                            {
                                // Not single 
                                int Temp1 = 0;
                                int Temp2 = 0;
                                int Temp3 = 0;
                                int Temp4 = 0;
                                int Temp5 = 0;

                                if (int.TryParse(WTraceOrRRNumber, out Temp3))
                                {
                                    Temp1 = Temp3 - 2;
                                    Temp2 = Temp3 - 1;
                                    Temp4 = Temp3 + 1;
                                    Temp5 = Temp3 + 2;
                                }

                                if (WTextLine.Contains(WTraceOrRRNumber))
                                {
                                    row.DefaultCellStyle.BackColor = Color.Aqua;
                                    row.DefaultCellStyle.ForeColor = Color.Black;

                                    //// dataGridView1.Rows[Count - 1].Selected = true;
                                    textBoxFuidAndLine.Show();

                                    textBoxFuidAndLine.Text = "ID:" + Journal_Id.ToString() + ".LN" + Journal_LN.ToString();

                                }
                                else
                                {
                                    //if (WTraceOrRRNumber.Length>5)
                                    //{
                                    //    hj = WTraceOrRRNumber.Substring(2, 4);
                                    //    if (WTextLine.Contains(hj))
                                    //    {
                                    //        // row.DefaultCellStyle.BackColor = Color.AliceBlue;
                                    //        row.DefaultCellStyle.BackColor = Color.Gainsboro;
                                    //        row.DefaultCellStyle.ForeColor = Color.Black;

                                    //        dataGridView1.Rows[Count - 1].Selected = true;

                                    //    }
                                    //}
                                    //else
                                    //{

                                    //}


                                }
                                if (Temp1 > 0)
                                {
                                    if (WTextLine.Contains(Temp1.ToString()))
                                    {
                                        row.DefaultCellStyle.BackColor = Color.Aqua;
                                        row.DefaultCellStyle.ForeColor = Color.Black;

                                        //  dataGridView1.Rows[Count - 1].Selected = true;

                                    }
                                }
                                if (Temp2 > 0)
                                {
                                    if (WTextLine.Contains(Temp2.ToString()))
                                    {
                                        row.DefaultCellStyle.BackColor = Color.Aqua;
                                        row.DefaultCellStyle.ForeColor = Color.Black;

                                        //dataGridView1.Rows[Count - 1].Selected = true;

                                    }
                                }
                                if (Temp4 > 0)
                                {
                                    if (WTextLine.Contains(Temp4.ToString()))
                                    {
                                        row.DefaultCellStyle.BackColor = Color.Aqua;
                                        row.DefaultCellStyle.ForeColor = Color.Black;

                                        //dataGridView1.Rows[Count - 1].Selected = true;

                                    }
                                }
                                if (Temp5 > 0)
                                {
                                    if (WTextLine.Contains(Temp5.ToString()))
                                    {
                                        row.DefaultCellStyle.BackColor = Color.Aqua;
                                        row.DefaultCellStyle.ForeColor = Color.Black;

                                        //dataGridView1.Rows[Count - 1].Selected = true;

                                    }
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
                CatchDetails(ex, true);
            }

        }

        // Print Trace Journal 
        private void buttonPrintTrace_Click(object sender, EventArgs e)
        {
            //if (textBoxTraceNo.Text == "0")
            //{
            //    MessageBox.Show("This line has no Trace Number");
            //    return;
            //}

            WPrintTraceDtTm = LineDate.ToString();

            // TRACE
            // Show all lines for a TRACE NUMBER  for a specific ATM

            Form56R16 PrintJournal = new Form56R16(WOperator, WTraceOrRRNumber, WDate_A.ToString(), WAtmNo, WSignedId);
            PrintJournal.Show();
        }

     

        // Finish 
        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        //
        // Catch Details
        //
        private static void CatchDetails(Exception ex, bool InIgnore)
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

            if (InIgnore == true)
            {
                // Do Nothing 
            }
            else
            {
                Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");

            }


            //    Environment.Exit(0);
        }

        private void buttonJournal_Click(object sender, EventArgs e)
        {

        }
        // Show
        private void buttonShow_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please enter value to show");
            }
            else
            {
                WMode = 9;
                WSingle = true;
                WTraceOrRRNumber = textBox1.Text;
                ShowGrid();

            }

        }
//
// Create Excel 
//
        private void buttonExcel_Click(object sender, EventArgs e)
        {

            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelPath = "C:\\RRDM\\Working\\Form154_"+ WAtmNo + ".xls";

            string WorkingDir = "C:\\RRDM\\Working\\";
     
            XL.ExportToExcel(Sc.ChangesInForm153Table, WorkingDir, ExcelPath);
        }
    }
}

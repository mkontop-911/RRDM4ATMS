using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        // Define the data table 
        public DataTable WDataTable = new DataTable();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingCategories Mc = new RRDMMatchingCategories(); 

        public int WPostedNo;
        public int UniqueIsChosen;

        public int WMaskRecordId; 

        public int WSelectedRow = 0;

        bool FromGrid04;

        bool FromGrid05;

        bool FromOwnATMs; 

        ////bool WithDate;
        //string Gridfilter; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WHeader;
        string WFromForm;
        string WMatchingCateg;
        int WRMCycle; 

        public Form78d(string InSignedId, int InSignRecordNo, string InOperator, 
            DataTable InDataTable, string InHeader, string InFromForm, string InMatchingCateg, int InRMCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WHeader = InHeader;
            WFromForm = InFromForm;
            WMatchingCateg = InMatchingCateg; 

            WDataTable = new DataTable();
            WDataTable.Clear();

            WDataTable = InDataTable;

            WRMCycle = InRMCycle; 

            InitializeComponent();

            labelWhatGrid.Text = WHeader;

            FromGrid04 = false;

            FromGrid05 = false;

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WMatchingCateg);

            if (Mc.ReconcMaster == true)
            {
                FromOwnATMs = false;
            }
            else
            {
                FromOwnATMs = true;
            }

            if (WFromForm == "Form78cTxns" || WFromForm == "Form78cSummary" 
                || WFromForm == "Form271b")  buttonFinish.Text = "Finish";

            if ( WFromForm == "Form78cSummary" ) buttonUpdate.Visible = true;

            if (WFromForm == "Atms-Admin")
            {
                buttonFinish.Text = "Continue Matching";
                label1.Hide();
                textBoxTrace.Hide();
                button1.Hide();
                linkLabelExpand.Hide(); 
            }
           
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            if (WDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No Entries to show."); 
                return; 
            }
            dataGridView1.DataSource = WDataTable.DefaultView;
            // SHOW GRID

            if (WFromForm == "Atms-Admin") // Categories 
            {
                ShowGrid01();
            }

            if (WFromForm == "Atms-Admin-Errors") // Errors
            {
                linkLabelExpand.Hide();
                buttonFinish.Text = "Finish"; 
                ShowGrid05();
            }

            if (WFromForm == "Form78cSummary") // Matching Summary (compressed)
            {
                ShowGrid04();
            }

            if (WFromForm == "Form78cTxns") // Txns of working files 
            {
                ShowGrid03();
            }

            if (WFromForm == "Form271b") // Detail of mask 
            {
                string PreviousAccno = ""; 
                decimal PreviousAmt = 0;
              
                // Read table of unmatched 
                int I = 0;

                while (I <= (WDataTable.Rows.Count - 1))
                {

                    int OriginSeqNo = (int)WDataTable.Rows[I]["OriginSeqNo"];
                    string FileId = (string)WDataTable.Rows[I]["FileId"];
                    string WCase = (string)WDataTable.Rows[I]["WCase"];
                    int Type = (int)WDataTable.Rows[I]["Type"];
                    int DublInPos = (int)WDataTable.Rows[I]["DublInPos"];
                    int InPos = (int)WDataTable.Rows[I]["InPos"];
                    int NotInPos = (int)WDataTable.Rows[I]["NotInPos"];
                    string TerminalId = (string)WDataTable.Rows[I]["TerminalId"];
                    int TraceNo = (int)WDataTable.Rows[I]["TraceNo"];
                    string AccNo = (string)WDataTable.Rows[I]["AccNo"];
                    decimal TranAmt = (decimal)WDataTable.Rows[I]["TranAmt"];
                    string MatchingCateg = (string)WDataTable.Rows[I]["MatchingCateg"];
                    int RMCycle = (int)WDataTable.Rows[I]["RMCycle"];

                    if (AccNo != PreviousAccno & I >= 1)
                    {
                        // First comparison is excluded
                        labelWorking1.Show();
                        labelWorking1.Text = "THERE IS DIFFERENCE IN ACCOUNTS";
                      
                    }

                    if (TranAmt != PreviousAmt & I >= 1)
                    {
                        // First comparison is excluded
                        labelWorking2.Show(); 
                        labelWorking2.Text =  "THERE IS DIFFERENCE IN AMTS";
                    }

                    PreviousAccno = AccNo;
                    PreviousAmt = TranAmt;

                    I++; // Read Next entry of the table 

                }

                ShowGrid02(); 
            }
                         
        }

        // Show Grid 01 
        public void ShowGrid01()
        {
            
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2"; 

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 90; // RMCategId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[2].Width = 250; //  FileName
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 150; // ProcessMode
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; 

            dataGridView1.Columns[4].Width = 120; // LastInFileDtTm
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

            dataGridView1.Columns[5].Width = 130; // LastMatchingDtTm
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        //
        // Show Grid 02
        // 
        public void ShowGrid02()
        {
            
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 50; // OriginSeqNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 50; // FileId
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[3].Width = 150; //  WCase
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 50; // Type
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 70; // DublInPos
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

            dataGridView1.Columns[6].Width = 70; // NotInPos
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 50; // TerminalId
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 50; // TraceNo
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Sort(dataGridView1.Columns[8], ListSortDirection.Ascending);

            dataGridView1.Columns[9].Width = 60; // AccNo
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 100; // TranAmt
            dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[11].Width = 70; // Matching Category
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[12].Width = 60; // RMCycle
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        // Show Grid 03 
        public void ShowGrid03()
        {

            dataGridView1.Columns[0].Width = 70; // WhatFile
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 60; // SeqNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 100; //  MatchingCateg
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 70; // RMCycle
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 70; // TerminalId
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

            dataGridView1.Columns[5].Width = 90; // TraceNo
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 90; // Accno 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 90; // TranAmt
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }

        // Show Grid 04 
      
        decimal TotalMatched, TotalUnMatched;
        public void ShowGrid04()
        {
            labelTotalItems.Show();
            labelTotalItems.Text = "Total Items :" + WDataTable.Rows.Count.ToString();
         
            // Our ATMS 
            //   " SELECT  SeqNo,  "
            // Date 
            //+ " AtmTraceNo,  "
            //+ " Matched,  "
            //+ " MatchMask,  "
            //+ " TerminalId,  "
            //+ " TransDescr,  "
            //+ " AccNumber,  "
            //+ " TransAmount,  "
            //+ " SettledRecord  "         

            FromGrid04 = true;

            if (FromOwnATMs == true)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 60; // SeqNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[1].Width = 80; // Date
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[2].Width = 90; // AtmTraceNo
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[3].Width = 50; // Matched
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[4].Width = 70; // MatchMask
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[5].Width = 60; // TerminalId
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

                dataGridView1.Columns[6].Width = 180; // TransDescr
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[7].Width = 120; // AccNumber
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[8].Width = 90; // TransAmount
                dataGridView1.Columns[8].DefaultCellStyle = style;
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[9].Width = 90; //SettledRecord
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[10].Width = 300; //Comments
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                TotalMatched = TotalUnMatched = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //WSeqNo = (int)rowSelected.Cells[0].Value;

                    bool WMatched = (bool)row.Cells[3].Value;
                    decimal WAmt = (decimal)row.Cells[8].Value;

                    if (WMatched == true)
                    {
                        TotalMatched = TotalMatched + WAmt;
                    }
                    else
                    {
                        TotalUnMatched = TotalUnMatched + WAmt;
                    }
                }
                //labelWorking1.Show();
                labelWorking2.Show();
                //labelWorking1.Text = "Total Matched = " + TotalMatched.ToString("#,##0.00");
                labelWorking2.Text = "Total UnMatched = " + TotalUnMatched.ToString("#,##0.00");
            }

            if (FromOwnATMs == false)
            {
                // NOT ATMS 
                // " SELECT  SeqNo,  "
                //+ " CAST(TransDate AS Date) As Date,"
                //+ " RRNumber ,  "
                //+ " Matched,  "
                //+ " MatchMask,  "
                //+ " TransDescr,  "
                //+ " AccNumber,  "
                //+ " TransAmount,  "
                //+ " SettledRecord,  "
                //+ " Comments  "
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 60; // SeqNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[1].Width = 80; // Date
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[2].Width = 90; // RRNumber
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[3].Width = 50; // Mask
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[4].Width = 80; // TransDescr
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[5].Width = 90; // TransAmount
                dataGridView1.Columns[5].DefaultCellStyle = style;
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                //dataGridView1.Columns[6].Width = 80; // 
                //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            }
        }

        // Show Grid 05

    
        public void ShowGrid05()
        {
            labelTotalItems.Show();
            labelTotalItems.Text = "Total Items :" + WDataTable.Rows.Count.ToString();

            FromGrid05 = true;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 60; // AtmNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; // Trace
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 80; // Descr
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 600; // Txt Line
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // On ROW ENTER 
        int WTraceNo;
        string WAtmNo;
        string WMask;
        DateTime WTranDate; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            if (FromGrid04 == true & FromOwnATMs == true)
            {
                WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

                WTranDate = (DateTime)dataGridView1.Rows[e.RowIndex].Cells["Date"].Value;

                WTraceNo = (int)dataGridView1.Rows[e.RowIndex].Cells["TraceNoWithNoEndZero"].Value;

                WMask = (string)dataGridView1.Rows[e.RowIndex].Cells["MatchMask"].Value;

                WAtmNo = (string)dataGridView1.Rows[e.RowIndex].Cells["TerminalId"].Value;
               
                textBoxTrace.Text = WTraceNo.ToString();
            }

            if (FromGrid04 == true & FromOwnATMs == false)
            {
               // " SELECT  SeqNo,  "
               //+ " CAST(TransDate AS Date) As Date,"
               //+ " RRNumber ,  "
               //+ " Matched,  "
               //+ " MatchMask,  "
               //+ " TransDescr,  "
               //+ " AccNumber,  "
               //+ " TransAmount,  "
               //+ " SettledRecord,  "
               //+ " Comments  "
                WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

                WTranDate = (DateTime)dataGridView1.Rows[e.RowIndex].Cells["Date"].Value;

                WTraceNo = (int)dataGridView1.Rows[e.RowIndex].Cells["RRNumber"].Value;

                WMask = (string)dataGridView1.Rows[e.RowIndex].Cells["MatchMask"].Value;

                label1.Text = "RRNumber"; 
                textBoxTrace.Text = WTraceNo.ToString();
            }

            if (FromGrid05 == true)
            {
                WMask = ""; 

                //WTranDate = (DateTime)dataGridView1.Rows[e.RowIndex].Cells["Date"].Value;

                WTraceNo = (int)dataGridView1.Rows[e.RowIndex].Cells["TraceNo"].Value;

                //WMask = (string)dataGridView1.Rows[e.RowIndex].Cells["MatchMask"].Value;

                WAtmNo = (string)dataGridView1.Rows[e.RowIndex].Cells["AtmNo"].Value;

                WTranDate = (DateTime)dataGridView1.Rows[e.RowIndex].Cells["TRanDate"].Value; 

                textBoxTrace.Text = (WTraceNo).ToString();
            }

        }


// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form78d_AllFiles NForm78d_AllFiles;

            string WSelectionCriteria = " WHERE "
                                        + "TerminalId ='" + WAtmNo + "'"
                                        + " AND MatchingAtRMCycle =" + WRMCycle
                                        + " AND (TraceNoWithNoEndZero =" + WTraceNo + " OR RRNumber = " + WTraceNo + ")"
                                       ;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria,2);

            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

            NForm78d_AllFiles.ShowDialog();

        }
        // Update Comments
        int WSeqNo;
        string WComments;
        string WSelectionCriteria;


        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (FromOwnATMs == true)
            {
                int K = 0;

                while (K <= (dataGridView1.Rows.Count - 1))
                {
                    WSeqNo = (int)dataGridView1.Rows[K].Cells["SeqNo"].Value;

                    WComments = (string)dataGridView1.Rows[K].Cells["Comments"].Value;


                    WSelectionCriteria = " WHERE  SeqNo =" + WSeqNo;
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria,2);

                    Mpa.Comments = WComments;

                    Mpa.UpdateMatchingTxnsMasterPoolATMsComments(WSelectionCriteria,2);


                    K++; // Read Next entry of the table 
                }

                // Print
                string P1 = "Discrepancies For Category: EWB103 AND CYCLE: 203 ";

                string P2 = WMatchingCateg;  // Category 
                string P3 = "203";
                string P4 = WOperator;
                string P5 = WSignedId;

                Form56R68ATMS ReportATMS68 = new Form56R68ATMS(P1, P2, P3, P4, P5);
                ReportATMS68.Show();
            }
            else
            {
                MessageBox.Show("Future Development"); 
            }
        }

        // EXPAND
        private void linkLabelExpand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form78c NForm78c;

            RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

            int TempTrace;

            if (int.TryParse(textBoxTrace.Text, out TempTrace))
            {
            }
            else
            {
                MessageBox.Show(textBoxTrace.Text, "Please enter a valid number!");
                return;
            }



            //string SelectionCriteria = " WHERE TransDate = " + WTranDate.Date + " AND TraceNo = " + TempTrace ;

            Md.ReadMatchingDiscrepanciesFillTable(WAtmNo, WTranDate.Date, TempTrace);

            //string WHeader = "LIST OF Unmatched By Trace No";
            //NForm78c = new Form78c(WSignedId, WSignRecordNo, WOperator, Md.TableMatchingDiscrepancies,
            //                        WHeader, "Form271b");

            WHeader = "LIST OF Dublicate And UnMatched FOR Trace : " + TempTrace;
            NForm78c = new Form78c(WSignedId, WSignRecordNo, WOperator,
                                   Md.TableMatchingDiscrepancies, Md.TableMatchingDiscrepancies,
                                   //Md.TableMatchingDiscrepancies, Md.TableMatchingDiscrepancies, Md.TableMatchingDiscrepancies,
                                   WHeader, "Form78d", WMatchingCateg);

            NForm78c.Show();
         
        }

    }
}

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_AllFiles : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMMatchingOfTxnsFindOriginRAW Msr = new RRDMMatchingOfTxnsFindOriginRAW();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
      //  RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables();
      //  RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

        //int MaskLength;

        string WOperator;
        string WSignedId;
        int WUniqueRecordId;

        public Form78d_AllFiles(string InOperator, string InSignedId, int InUniqueRecordId)

        {
            WSignedId = InSignedId;

            WOperator = InOperator;

            WUniqueRecordId = InUniqueRecordId;

            InitializeComponent();

            string WSelectionCriteria = " WHERE UniqueRecordId = " + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria,2);

            if (Mpa.TraceNoWithNoEndZero > 0)
            {
                labelWhatGrid.Text = "ORIGINAL DATA FOR TRACE : " + Mpa.TraceNoWithNoEndZero.ToString() + " AND MASK :" + Mpa.MatchMask;
            }
            else
            {
                labelWhatGrid.Text = "ORIGINAL DATA FOR TRACE : " + Mpa.RRNumber.ToString() + " AND MASK :" + Mpa.MatchMask; 
            }

            //MaskLength = Mpa.MatchMask.Length;

        }

        bool WRecordFoundInUniversal; 

        private void Form78b_Load(object sender, EventArgs e)
        {

            string WSelectionCriteria = " WHERE UniqueRecordId = " + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria,2);

            string TerminalId = Mpa.TerminalId;
            DateTime Date = Mpa.TransDate.Date;

            int WTrace = 0;
            string WRRN = "0";

            if (Mpa.Origin == "Our Atms")
            {
                // It is a Trace
                WTrace = Mpa.AtmTraceNo; // WITH ZERO AT THE END
               
                if (Mpa.NotInJournal == true)
                {
                    buttonJournal.Hide(); 
                }
                else
                {
                    buttonJournal.Show();
                }
            }
            else
            {
                // It is an RRN
                WRRN = Mpa.RRNumber;

                buttonJournal.Hide();
                buttonJournal_Near.Hide(); 
            }

            string FileA = Mpa.FileId01;
            string FileB = Mpa.FileId02;
            string FileC = Mpa.FileId03;

            //
            // DO FILE A 
            //
            string PhysicalNameA = "";
            string PhysicalNameB = "";
            string PhysicalNameC = "";

            if (Mpa.Origin == "Our Atms")
            {
                Msr.FillTablesProcessForJournal(WOperator, WSignedId,
                                       Mpa.TerminalId,
                                       Mpa.AtmTraceNo, Mpa.TransDate.Date);

                if (Msr.TableJournalDetails.Rows.Count > 0)
                {
                    dataGridView1.DataSource = Msr.TableJournalDetails.DefaultView;
                    label1.Text = "RECORD DETAILS FOR JOURNAL RECORD";
                }
                else
                {
                    label1.Text = "RECORD DETAILS NOT FOUND FOR Journal - Try journal" ;
                }
                              
            }
            else
            {
                WRecordFoundInUniversal = Msr.FindRawRecordFromMasterRecord(WOperator, FileA, Mpa.MatchingAtRMCycle ,Mpa.TransDate.Date ,TerminalId, WTrace,WRRN, 1);

                if (Msr.TableDetails_RAW.Rows.Count > 0)
                {
                    dataGridView1.DataSource = Msr.TableDetails_RAW.DefaultView;
                    if (WRecordFoundInUniversal)
                    {
                        label1.Text = "RECORD DETAILS FOR :" + FileA;
                    }
                    else
                    {
                        label1.Text = "UNMATCHED RECORD DETAILS FOR :" + FileA;
                    }
                    
                }
                else
                {
                    label1.Text = "RECORD DETAILS NOT FOUND FOR :" + FileA;
                }             

            }

            //*********************************************************************
            // Assign Without Zero 
            //*********************************************************************
            if (Mpa.Origin == "Our Atms")
            {
                // It is a Trace
                WTrace = Mpa.TraceNoWithNoEndZero; // WITH NO ZERO AT THE END
            }


            //
            // DO FILE B 
            //

            WRecordFoundInUniversal = Msr.FindRawRecordFromMasterRecord
                (WOperator, FileB, Mpa.MatchingAtRMCycle, Mpa.TransDate.Date, TerminalId, WTrace,WRRN, 1);

            if (Msr.TableDetails_RAW.Rows.Count > 0)
            {
                dataGridView2.DataSource = Msr.TableDetails_RAW.DefaultView;
                if (WRecordFoundInUniversal)
                {
                    label2.Text = "RECORD DETAILS FOR :" + FileB;
                }
                else
                {
                    label2.Text = "UNMATCHED RECORD DETAILS FOR :" + FileB;
                }

            }
            else
            {
                label2.Text = "RECORD DETAILS NOT FOUND FOR :" + FileB;
            }
            //
            // DO FILE C 
            //

            if (FileC != "")
            {
                dataGridView3.Show();
                WRecordFoundInUniversal = Msr.FindRawRecordFromMasterRecord(WOperator, FileC, Mpa.MatchingAtRMCycle, Mpa.TransDate.Date, TerminalId, WTrace, WRRN ,1);

                if (Msr.TableDetails_RAW.Rows.Count > 0)
                {
                    dataGridView3.DataSource = Msr.TableDetails_RAW.DefaultView;
                    if (WRecordFoundInUniversal)
                    {
                        label3.Text = "RECORD DETAILS FOR :" + FileC;
                    }
                    else
                    {
                        label3.Text = "UNMATCHED RECORD DETAILS FOR :" + FileC;
                    }

                }
                else
                {
                    label3.Text = "RECORD DETAILS NOT FOUND FOR :" + FileC;
                }
            }
            else
            {
                label3.Text = "NO OTHER FILE ";
                dataGridView3.Hide(); 
            }

        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];


        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// Journal Lines 
        private void buttonJournal_Click(object sender, EventArgs e)
        {
            Form67 NForm67;

            int Mode = 5; // Specific
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            NForm67 = new Form67(WSignedId, 0 , WOperator, 0, Mpa.TerminalId, Mpa.AtmTraceNo, Mpa.AtmTraceNo, Mpa.TransDate.Date, NullPastDate, Mode);
            NForm67.ShowDialog();
        }
// Near Journal Lines 
       

        private void buttonJournal_Near_Click(object sender, EventArgs e)
        {
            //int TraceFrom = (Mpa.TraceNoWithNoEndZero - 2) * 10;
            //int TraceTo = (Mpa.TraceNoWithNoEndZero + 2) * 10;

            //Form67 NForm67;

            //int Mode = 4; // Range 
            //DateTime NullPastDate = new DateTime(1900, 01, 01);
            //NForm67 = new Form67(WSignedId, 0, WOperator, 0, Mpa.TerminalId, TraceFrom, TraceTo, Mpa.TransDate.Date, NullPastDate, Mode);
            //NForm67.ShowDialog();
            int TraceFrom;
            int TraceTo;
            int SaveSeqNo = Mpa.SeqNo;
            DateTime WDt = Mpa.TransDate;

            DateTime WDtA;
            DateTime WDtB;

            string SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                              + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate < @TransDate   ";
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
                              + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate > @TransDate  ";

            OrderBy = "  ORDER By TransDate Asc";
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
    }
}

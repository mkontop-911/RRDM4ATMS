using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_AllFiles_BDC_3 : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMJournalAudi_BDC Msr = new RRDMJournalAudi_BDC();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories(); 
       
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //int MaskLength;
        int WTraceNo = 0;
        string WRRNumber = "";

        string WSelectionCriteria;

        bool RRNBased = false;

        string PRX; 

        bool IsOurAtm = false;

        string FileA;
        string FileB;
        string FileC;

        string WTerminalId;
        decimal WTransAmount; 
        DateTime WTransDate;
        string WCardNumber;
        string WEncryptedCard = "";
        string WMatchingCateg;
        string WOrigin;
        int WLoadedAtRMCycle; 

        bool WRecordFoundInUniversal;

        string WOperator;
        string WSignedId;
        int WUniqueRecordId;
        int WMode;

        public Form78d_AllFiles_BDC_3(string InOperator, string InSignedId, int InUniqueRecordId, int InMode)

        {
            WSignedId = InSignedId;

            WOperator = InOperator;

            WUniqueRecordId = InUniqueRecordId;
            WMode = InMode; // 1 Based on Mpa
                            // 2 Based on IST
                            // 4 Based on Mpa for HST 

            InitializeComponent();

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }

            if (WMode == 1 || WMode == 4)
            {
                // read record
                WSelectionCriteria = " WHERE UniqueRecordId = " + WUniqueRecordId;
                // Records are in the primary
                if (WMode == 1 )
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria, 2);
                }
                if (WMode == 4)
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(WSelectionCriteria, 2);
                }


                WRRNumber = Mpa.RRNumber;
                WTraceNo = Mpa.TraceNoWithNoEndZero;

                WTerminalId = Mpa.TerminalId;
                WTransAmount = Mpa.TransAmount;
                WTransDate = Mpa.TransDate.Date;
                WCardNumber = Mpa.CardNumber;
                WEncryptedCard = Mpa.Card_Encrypted;
                WMatchingCateg = Mpa.MatchingCateg;
                WOrigin = Mpa.Origin;
                WLoadedAtRMCycle = Mpa.MatchingAtRMCycle; 

                //
                // Read Category id and find if slave to ATM groups 
                Mc.ReadMatchingCategorybyActiveCategId(WOperator, Mpa.MatchingCateg); 

                if (Mc.ReconcMaster == true)
                {
                    // Not slave like the ones from our ATMs
                  
                    buttonJournal.Hide();
                    buttonJournal_Near.Hide();
                    buttonNear_IST.Hide();

                    labelWhatGrid.Text = "ORIGINAL DATA FOR RRN : " + Mpa.RRNumber.ToString() + " AND MASK :" + Mpa.MatchMask;
                    RRNBased = true; // It true for second and third file 
                }
                else
                {
                    // Slave categories coming from own ATMs

                    buttonJournal.Show();
                    buttonJournal_Near.Show();
                    buttonNear_IST.Hide();

                    labelWhatGrid.Text = "ORIGINAL DATA FOR TRACE : " + Mpa.TraceNoWithNoEndZero.ToString() + " AND MASK :" + Mpa.MatchMask;
                    RRNBased = false;
                }           
            }

            if (WMode == 2)
            {
                
                //return;
                // read record
                RRDMMatchingTxns_InGeneralTables_BDC Gpa = new RRDMMatchingTxns_InGeneralTables_BDC();
                WSelectionCriteria = " WHERE SeqNo = " + WUniqueRecordId;
                // Records are in the primary
                // FROM IST
                Gpa.ReadTransSpecificFromBothTables_By_SelectionCriteria(WSelectionCriteria, "Switch_IST_Txns");

                if (Gpa.RecordFound == true)
                {

                }
                WRRNumber = Gpa.RRNumber;
                WTraceNo = Gpa.TraceNo;
                WTerminalId = Gpa.TerminalId;
                WTransAmount = Gpa.TransAmt; 
                WTransDate = Gpa.TransDate;
                WCardNumber = Gpa.CardNumber; 
                WEncryptedCard = Gpa.Card_Encrypted;
                WMatchingCateg = Gpa.MatchingCateg;
                WLoadedAtRMCycle = Mpa.MatchingAtRMCycle;

                //
                // Read Category id and find if slave to ATM groups 
                Mc.ReadMatchingCategorybyActiveCategId(WOperator, Gpa.MatchingCateg);
                if (Mc.ReconcMaster == true)
                {
                    // Not slave like the ones from our ATMs
                    WOrigin = "Not Our Atms";
                    buttonJournal.Hide();
                    buttonJournal_Near.Hide();
                    buttonNear_IST.Hide();

                    labelWhatGrid.Text = "ORIGINAL DATA FOR RRN : " + Mpa.RRNumber.ToString();
                    RRNBased = true; // It true for second and third file 
                }
                else
                {
                    // Slave categories coming from own ATMs
                    WOrigin = "Our Atms";

                    buttonJournal.Show();
                    buttonJournal_Near.Show();
                    buttonNear_IST.Hide();

                    labelWhatGrid.Text = "ORIGINAL DATA FOR TRACE : " + WTraceNo.ToString();
                    RRNBased = false;
                }              

            }

            //
            // INITIALISE FILES
            //
            RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
            Mcs.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);

            FileA = Mcs.SourceFileNameA;
            FileB = Mcs.SourceFileNameB;
            FileC = Mcs.SourceFileNameC;

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            // FILL IN MATCHIND FIELDS 
            ShowMatchingFields();  // By Matching Category 
        }


        private void Form78b_Load(object sender, EventArgs e)
        {


            if (WOrigin == "Our Atms")
            {
                // READ PAMBOS FILE
                if (WMode == 1 || WMode == 2)
                {
                    Msr.FillTablesProcessForJournal(WOperator, WSignedId,
                                       WTerminalId,
                                       WTraceNo * 10, WTransDate.Date, WTransAmount);
                }

                if (WMode == 4 )
                {
                    Msr.FillTablesProcessForJournal_HST(WOperator, WSignedId,
                                       WTerminalId,
                                       WTraceNo * 10, WTransDate.Date, WTransAmount);
                }


                if (Msr.TableJournalDetails.Rows.Count > 0)
                {
                    dataGridView1.DataSource = Msr.TableJournalDetails.DefaultView;
                    label1.Text = "RECORD DETAILS FOR JOURNAL RECORD";
                    labelRespCode_1.Text = "RespCode_..:" + Msr.WResponseCode;
                    buttonJournal.Show();
                    if (WMode == 1)
                    {
                        buttonJournal_Near.Show();
                        buttonNear_IST.Hide(); 
                    }
                    if (WMode == 2)
                    {
                        buttonJournal_Near.Hide();
                        buttonNear_IST.Show();
                    }

                }
                else
                {
                    label1.Text = "RECORD DETAILS NOT FOUND FOR Journal - Try journal";
                    buttonJournal.Hide();
                   
                    if (WMode == 1)
                    {
                        buttonJournal_Near.Show();
                        buttonNear_IST.Hide();
                    }
                    if (WMode == 2)
                    {
                        buttonJournal_Near.Hide();
                        buttonNear_IST.Show();
                    }
                    labelRespCode_1.Hide();
                }

            }
            else
            {
                if (RRNBased == true)
                {
                    // RRN BASED
                    if (WMode == 1 || WMode == 2)
                    {
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileA, WMatchingCateg
                        , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);

                        if (Mgt.RecordFound == false & FileA != "MEEZA_GLOBAL_LCL")
                        {
                            
                            WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileA, WMatchingCateg
                                          , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 4, 2);

                            if (Mgt.RecordFound == false)
                            {
                                WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecordLoadedAtCycle(WOperator, FileA
                                     , WRRNumber, WTransAmount, WEncryptedCard, WLoadedAtRMCycle);
                            }         
                        }
                    }
                    if (WMode == 4)
                    {
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord_HST(WOperator, FileA, WMatchingCateg
                               , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                    }

                }
                else
                {
                    // TRACE BASED
                    if (WMode == 1 || WMode == 2)
                    {
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileA, WMatchingCateg
                         , WTransDate.Date, WTerminalId, WTraceNo, "", WTransAmount, WEncryptedCard, 1, 2);
                    }
                    if (WMode == 4)
                    {
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord_HST(WOperator, FileA, WMatchingCateg
                        , WTransDate.Date, WTerminalId, WTraceNo, "", WTransAmount, WEncryptedCard, 1, 2);
                    }
                }
               

                if (Mgt.TableDetails_RAW.Rows.Count > 0)
                {
                    dataGridView1.DataSource = Mgt.TableDetails_RAW.DefaultView;
                    if (WRecordFoundInUniversal)
                    {
                        label1.Text = "RECORD DETAILS FOR :" + FileA;
                        labelRespCode_1.Text = "RespCode_..:" + Mgt.ResponseCode;
                    }
                    else
                    {
                        label1.Text = "UNMATCHED RECORD DETAILS FOR :" + FileA;
                        labelRespCode_1.Text = "RespCode_..:" + Mgt.ResponseCode;
                    }
                }
                else
                {
                    label1.Text = "RECORD DETAILS NOT FOUND FOR :" + FileA;
                    labelRespCode_1.Hide();
                }

            }

            ////*********************************************************************
            //// Assign Without Zero 
            ////*********************************************************************
            //
            // DO FILE B 
            //
            if (RRNBased == true)
            {
                // RRN BASED

                if (WMatchingCateg.Substring(3, 3) == "231"
                   || WMatchingCateg.Substring(3, 3) == "233"
                   || WMatchingCateg.Substring(3, 3) == "272"
                   || WMatchingCateg.Substring(3, 3) == "273"
                   )
                {
                    // Mode = 2 // Matched without Amount
                    WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileB, WMatchingCateg
                    , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 2, 2);
                    if (Mgt.RecordFound == false)
                    {
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileB, WMatchingCateg
                   , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 4, 2);
                    }
                }
                else
                {
                    // Mode = 1
                    if (WMode == 1)
                    {
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileB, WMatchingCateg
                             , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                    }
                    if (WMode == 4)
                    {
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord_HST(WOperator, FileB, WMatchingCateg
                               , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                    }

                }

            }
            else
            {
                // TRACE BASED
                if (WMode == 1 || WMode == 2)
                {
                    WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileB,WMatchingCateg
                   , WTransDate.Date, WTerminalId, WTraceNo, "", WTransAmount, WEncryptedCard, 1, 2);
                }
              

                if (WMode == 4)
                {
                    WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord_HST(WOperator, FileB, WMatchingCateg
                  , WTransDate.Date, WTerminalId, WTraceNo, "", WTransAmount, WEncryptedCard, 1, 2);
                }


            }

            if (Mgt.TableDetails_RAW.Rows.Count > 0)
            {
                dataGridView2.DataSource = Mgt.TableDetails_RAW.DefaultView;
                if (WRecordFoundInUniversal)
                {
                    label2.Text = "RECORD DETAILS FOR :" + FileB;
                }
                else
                {
                    label2.Text = "UNMATCHED RECORD DETAILS FOR :" + FileB;
                }
                labelRespCode_2.Text = "RespCode_..:" + Mgt.ResponseCode;
            }
            else
            {

                label2.Text = "RECORD DETAILS NOT FOUND FOR :" + FileB;
                labelRespCode_2.Hide();

            }
            //
            // DO FILE C 
            //

            if (FileC != "")
            {
                if (WOrigin == "Our Atms") // eg BDC201
                {
                    RRNBased = false;
                }
                else
                {
                    // For the third file which is not ours we use RRN 
                    RRNBased = true;
                }
                dataGridView3.Show();
                if (RRNBased == true)
                {
                    // RRN BASED

                    if (WMatchingCateg.Substring(3, 3) == "231"
                   || WMatchingCateg.Substring(3, 3) == "233"
                   || WMatchingCateg.Substring(3, 3) == "272"
                   || WMatchingCateg.Substring(3, 3) == "273"
                   )
                    {
                        // Mode = 2 // Matched without Amount
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileC, WMatchingCateg
                        , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 2, 2);

                    }
                    else
                    {
                        // Mode = 1
                        if (WMode == 1)
                        {
                            WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileC, WMatchingCateg
                                   , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                        }
                        if (WMode == 4)
                        {
                            WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord_HST(WOperator, FileC, WMatchingCateg
                                   , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                        }

                    }
                }
                else
                {
                    // TRACE BASED
                    if (WMode == 1 || WMode == 2)
                    {
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileC, WMatchingCateg
                        , WTransDate.Date, WTerminalId, WTraceNo, "", WTransAmount, WEncryptedCard, 1, 2);
                    }
                    if (WMode == 4)
                    {
                        WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord_HST(WOperator, FileC, WMatchingCateg
                        , WTransDate.Date, WTerminalId, WTraceNo, "", WTransAmount, WEncryptedCard, 1, 2);
                    }

                }
                if (Mgt.TableDetails_RAW.Rows.Count > 0)
                {
                    dataGridView3.DataSource = Mgt.TableDetails_RAW.DefaultView;
                    if (WRecordFoundInUniversal)
                    {
                        label3.Text = "RECORD DETAILS FOR :" + FileC;
                    }
                    else
                    {
                        label3.Text = "UNMATCHED RECORD DETAILS FOR :" + FileC;
                    }

                    labelRespCode_3.Text = "RespCode_..:" + Mgt.ResponseCode;

                }
                else
                {
                    label3.Text = "RECORD DETAILS NOT FOUND FOR :" + FileC;
                    labelRespCode_3.Hide();
                }
            }
            else
            {
                label3.Text = "NO OTHER FILE ";
                dataGridView3.Hide();
                labelRespCode_3.Hide();
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
            int WSeqNoA = 0;
            int WSeqNoB = 0;

            RRDMJournalReadTxns_Text_Class Ej = new RRDMJournalReadTxns_Text_Class();

            Ej.ReadJournalTxnsByParameters(WOperator, WTerminalId, WTraceNo*10, WTransAmount, WCardNumber, WTransDate.Date);

            if (Ej.RecordFound)
            {
                WSeqNoA = Ej.SeqNo;
                WSeqNoB = Ej.SeqNo;
            }
            else
            {
                MessageBox.Show("No data found for this selection");
                return;
            }
            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific

            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceNo.ToString(), WTerminalId, WSeqNoA, WSeqNoB, WTransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();

            //Form67 NForm67;

            //int Mode = 5; // Specific
            //DateTime NullPastDate = new DateTime(1900, 01, 01);
            //NForm67 = new Form67(WSignedId, 0 , WOperator, 0, Mpa.TerminalId, Mpa.AtmTraceNo, Mpa.AtmTraceNo, Mpa.TransDate.Date, NullPastDate, Mode);
            //NForm67.ShowDialog();
        }
        // Near Journal Lines 

        // Near Journal 
        int IntTrace;

        private void buttonJournal_Near_Click(object sender, EventArgs e)
        {
            // Here we search the journal lines close to the selected transaction
            // If selected already in Journal then we show the one before and the one after
            // If transaction not in journal say 011 then we show 5 minutes before and 5 minutes after. 
            // This is because the time at mainframe maybe different that this of ATM. 

            string WSubString = Mpa.MatchMask.Substring(0, 1);

            DateTime SavedTransDate = Mpa.TransDate;

            if (WSubString == "1")
            {
                IntTrace = Mpa.TraceNoWithNoEndZero;
            }

            bool In_HST = false;

            //if (Mpa.TransDate.Date <= HST_DATE)
            //{
            //    In_HST = true;
            //}
            // Show Lines of journal 
            //string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;
            //if (In_HST == true)
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            //}
            //else
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            //}

            string SelectionCriteria;
            int SaveSeqNo ;

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            //DateTime TestingDate = new DateTime(2019, 01, 03);

            SaveSeqNo = Mpa.SeqNo;

            DateTime WDtA = Mpa.TransDate;
            DateTime WDtB = Mpa.TransDate;
            DateTime WDt;

            WDt = WDtA.AddMinutes(-2);

            // FIND THE LESS
            SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                              + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate < @TransDate  ";
            string OrderBy = "  ORDER By TransDate Desc";

            if (In_HST == true)
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
            }
            else
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
            }

            if (Mpa.RecordFound == true)
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;

                // FIND THE GREATEST that exist 
                WDt = WDtB.AddMinutes(2);
                SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                           + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate > @TransDate ";

                OrderBy = "  ORDER By TransDate ASC ";


                if (In_HST == true)
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
                }
                else
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
                }

                if (Mpa.RecordFound == true)
                {
                    // 
                    WSeqNoB = Mpa.OriginalRecordId; // This is the SeqNo in Pambos Journal  

                }
                else
                {
                    MessageBox.Show("No Upper Limit. No Journal Lines to Show");

                    WSeqNoB = 0;
                    // Reestablish Mpa Data
                    //
                    //SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                    //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                    //return;
                }


            }
            else
            {
                MessageBox.Show("No Lower Limit. No Journal Lines to Show");
                // Reestablish Mpa Data
                //
                SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                if (In_HST == true)
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                }
                else
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
                }

                // Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                return;
            }
            //
            // Reestablish Mpa Data
            //
            SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

            if (In_HST == true)
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            }
            else
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            }

            //   Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific range
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (WSubString == "1")
            {
                WTraceRRNumber = IntTrace.ToString();
            }

            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                      SavedTransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();

        }
        // Near IST 
        private void buttonNear_IST_Click(object sender, EventArgs e)
        {
            // Here we search the journal lines close to the selected transaction
            // If selected already in Journal then we show the one before and the one after
            // If transaction not in journal say 011 then we show 5 minutes before and 5 minutes after. 
            // This is because the time at mainframe maybe different that this of ATM. 

            // WMode = 2
            string SelectionCriteria;
            int SaveSeqNo ;

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            //DateTime TestingDate = new DateTime(2019, 01, 03);

            SaveSeqNo = WUniqueRecordId;
            DateTime WDtA = WTransDate;
            DateTime WDtB = WTransDate;
            DateTime WDt;

            WDt = WDtA.AddMinutes(-5);

            // FIND THE LESS
            SelectionCriteria = " WHERE TerminalId ='" + WTerminalId
                              + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate < @TransDate  ";
            string OrderBy = "  ORDER By TransDate Desc";
            Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
            if (Mpa.RecordFound == true)
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;

                // FIND THE GREATEST that exist 
                WDt = WDtB.AddMinutes(5);
                SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                           + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate > @TransDate ";

                OrderBy = "  ORDER By TransDate ASC ";
                Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);

                if (Mpa.RecordFound == true)
                {
                    // 
                    WSeqNoB = Mpa.OriginalRecordId; // This is the SeqNo in Pambos Journal  

                }
                else
                {
                    MessageBox.Show("No Upper Limit. No Journal Lines to Show");
                    // Reestablish Mpa Data
                    //
                    SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                    return;
                }


            }
            else
            {
                MessageBox.Show("No Lower Limit. No Journal Lines to Show");
                // Reestablish Mpa Data
                //
                SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
                return;
            }
            //
            // Reestablish Mpa Data
            //
            SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific range
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                      Mpa.TransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();
        }

        private void ShowMatchingFields()
        {
            RRDMMatchingCategStageVsMatchingFields Rsm = new RRDMMatchingCategStageVsMatchingFields();

            Rsm.ReadReconcCategVsMatchingFieldsDataTableByMatchingCateg(WMatchingCateg);

            dataGridView4.DataSource = Rsm.ReconcCategStagesDataTable.DefaultView;

            if (dataGridView4.Rows.Count == 0)
            {
                return;
            }
        
            dataGridView4.Columns[0].Width = 100; // MatchingCateg
            dataGridView4.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView4.Columns[1].Width = 270; // MatchingField
            dataGridView4.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
    }
}

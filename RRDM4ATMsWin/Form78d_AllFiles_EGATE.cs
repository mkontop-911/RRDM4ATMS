using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_AllFiles_EGATE : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMJournalAudi_BDC Msr = new RRDMJournalAudi_BDC();
        RRDMMatchingTxns_InGeneralTables_EGATE Mmob = new RRDMMatchingTxns_InGeneralTables_EGATE();
        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories(); 
       
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //int MaskLength;
        //int WTraceNo = 0;
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
        string W_Application; 
        int WSeqNo;
        int WMode;

        public Form78d_AllFiles_EGATE(string InOperator, string InSignedId,string InApplication ,int InSeqNo, int InMode)

        {
            WSignedId = InSignedId;

            WOperator = InOperator;

            W_Application = InApplication; 

            WSeqNo = InSeqNo;
            WMode = InMode; // 1 Based on Mpa
                          
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
                
                string SelectionCriteria = " WHERE  SeqNo=" + WSeqNo;

                //string MasterTableName = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatching_Master_MOBILE]";
                string WMasterTableName = W_Application + ".[dbo]." + "TXNS_MASTER";
                Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, WMasterTableName, 1, W_Application);

                WRRNumber = Mmob.RRNumber;
               
                WTerminalId = "";
                WTransAmount = Mmob.TransAmount;
                WTransDate = Mmob.TransDate.Date;
                //WCardNumber = Mmob.CardNumber;
                //WEncryptedCard = Mmob.Card_Encrypted;
                WMatchingCateg = Mmob.MatchingCateg;
                //WOrigin = Mmob.Origin;
                WLoadedAtRMCycle = Mmob.MatchingAtRMCycle; 

                //
                // Read Category id and find if slave or  to ATM groups 
                Mc.ReadMatchingCategorybyActiveCategId(WOperator, Mmob.MatchingCateg); 

                if (Mc.ReconcMaster == true)
                {
                    // Not slave like the ones from our ATMs

                    labelWhatGrid.Text = "ORIGINAL DATA FOR RRN : " + Mmob.RRNumber.ToString() + " AND MASK :" + Mmob.MatchMask;
                    RRNBased = true; // It true for second and third file 
                }
                else
                {
                    // Slave categories coming from own ATMs
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

            string Master_Primary = ""; 

            //RRDMAtmsClass Ac = new RRDMAtmsClass();

            // FILL IN MATCHIND FIELDS 
            ShowMatchingFields();  // By Matching Category 
        }


        private void Form78b_Load(object sender, EventArgs e)
        {

                if (RRNBased == true)
                {
                    // RRN BASED
                    if (WMode == 1 || WMode == 2)
                    {
                    WRecordFoundInUniversal = Mmob.FindRecordsFromFile_X(WOperator, FileA, WMatchingCateg
                    , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
          
                    }
                    if (WMode == 4)
                    {
                        //WRecordFoundInUniversal = Mmob.FindRecordsFromMasterRecord_HST(WOperator, FileA, WMatchingCateg
                        //       , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                    }

                }
             

                if (Mmob.DataTableAllFields.Rows.Count > 0)
                {
                    dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;
                   
                        label1.Text = "UNMATCHED RECORD DETAILS FOR :" + FileA;
                        labelRespCode_1.Text = "RespCode_..:" + Mgt.ResponseCode;
                 
                }
                else
                {
                    label1.Text = "RECORD DETAILS NOT FOUND FOR :" + FileA;
                    labelRespCode_1.Hide();
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

               
                    // Mode = 1
                    if (WMode == 1)
                    {
                    WRecordFoundInUniversal = Mmob.FindRecordsFromFile_X(WOperator, FileB, WMatchingCateg
                   , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);

                    //WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileB, WMatchingCateg
                    //         , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                    }
                    if (WMode == 4)
                    {
                        //WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord_HST(WOperator, FileB, WMatchingCateg
                        //       , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                    }

                

            }
           

            if (Mmob.DataTableAllFields.Rows.Count > 0)
            {
                dataGridView2.DataSource = Mmob.DataTableAllFields.DefaultView;
               
                    label2.Text = "UNMATCHED RECORD DETAILS FOR :" + FileB;
               
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
               
                    // For the third file which is not ours we use RRN 
                    RRNBased = true;
                
                dataGridView3.Show();
                if (RRNBased == true)
                {
                   
                        // Mode = 1
                        if (WMode == 1)
                        {
                        WRecordFoundInUniversal = Mmob.FindRecordsFromFile_X(WOperator, FileC, WMatchingCateg
              , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                    }
                        if (WMode == 4)
                        {
                            //WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord_HST(WOperator, FileC, WMatchingCateg
                            //       , WTransDate.Date, WTerminalId, 0, WRRNumber, WTransAmount, WEncryptedCard, 1, 2);
                        }

                    
                }
              
                if (Mmob.DataTableAllFields.Rows.Count > 0)
                {
                    dataGridView3.DataSource = Mmob.DataTableAllFields.DefaultView;
                   
                        label3.Text = "UNMATCHED RECORD DETAILS FOR :" + FileC;
                  
                   // labelRespCode_3.Text = "RespCode_..:" + Mgt.ResponseCode;

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

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_AllFiles_BDC_3_MOBILE_TOTALS : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMJournalAudi_BDC Msr = new RRDMJournalAudi_BDC();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //int MaskLength;

        //string WRRNumber;
        decimal WTransAmount;

        string WMatchMask;

        string WSelectionCriteria;

        bool RRNBased = false;

        string PRX;

        bool IsOurAtm = false;

        string Master_Primary;
        string Master_Matched;

       
        string FileC;

        string FileA_RAW;
        string FileB_RAW;

        string FileA_DETAIL;
        string FileB_DETAIL;


        string FileA_RRDM;
        string FileB_RRDM;


        string FileA_RRDM_MATCHED;
        string FileB_RRDM_MATCHED;

        bool Is_TOTALS_EQUAL;
        decimal TOTALS_Difference;
        int DMode;

        string WTransactionId;
        string WSendernumber;

        string WTerminalId;

        DateTime WTransDate;
        string WCardNumber;
        string WEncryptedCard = "";
        //string WMatchingCateg;
        string WOrigin;
        int WLoadedAtRMCycle;
        bool TPF_Origin;
        long WRawSeqNo;

        bool WRecordFoundInUniversal;
        string MasterTableName;

        decimal NODE_Total;
        decimal MEEZA_Total;

        string WOperator;
        string WSignedId;
        string WRRNumber;
        string WMatchingCateg;
        int WSeqNo; 
        int WMode;
        string W_Application;

        public Form78d_AllFiles_BDC_3_MOBILE_TOTALS(string InOperator,  int InSeqNo, string InMatchingCateg,
                                                                                   string In_Application)
        {
            //WSignedId = InSignedId;

            WOperator = InOperator;

            //WRRNumber = InRRNumber;
            WMatchingCateg = InMatchingCateg;
            WSeqNo = InSeqNo; 
            W_Application = In_Application;

            InitializeComponent();

                // read record
                WSelectionCriteria = " WHERE SeqNo = " + WSeqNo;
            
                Master_Primary = In_Application + ".[dbo]." + In_Application + "_TPF_TXNS_MASTER";
                Master_Matched = In_Application + "_MATCHED_TXNS.[dbo]."+ In_Application+ "_TPF_TXNS_MASTER";

            WMode = 1;
                DMode = 2;
               
                
                Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, Master_Primary, Master_Matched, WSelectionCriteria,
                                                               WMode, DMode);

                if (Mmob.RecordFound == true)
                {
                    WRRNumber = Mmob.RRNumber;
                    WTransAmount = Mmob.TransAmount;
                    WMatchingCateg = Mmob.MatchingCateg;
                 
                    WRawSeqNo = Mmob.OriginalRecordId;
                    WMatchMask = Mmob.MatchMask;

                    
                }

            //[ETISALAT].[dbo].
            //[ETISALAT_MATCHED_TXNS].

            // Physical Files
            FileA_RRDM = W_Application+ ".[dbo]." + W_Application +"_NODE_TOTALS_TXNS"; // NODE
            FileB_RRDM = W_Application + ".[dbo]." + W_Application + "_MEEZA_TOTALS_TXNS"; // MEEZA

            FileA_RRDM_MATCHED = W_Application + "_MATCHED_TXNS.[dbo]." + W_Application + "_NODE_TOTALS_TXNS"; // NODE MATCHED
            FileB_RRDM_MATCHED = W_Application + "_MATCHED_TXNS.[dbo]." + W_Application + "_MEEZA_TOTALS_TXNS"; // MEEZA MATCHED

            // Find Total Amount for NODE 
            WSelectionCriteria = " WHERE RRNumber='" + WRRNumber + "'";
            WMode = 1;
            DMode = 2;
            
            Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, FileA_RRDM, FileA_RRDM_MATCHED, WSelectionCriteria,
                                                             WMode, DMode);
            if (Mmob.RecordFound==true)
            {
                NODE_Total = Mmob.TransAmount;
            }
            else
            {
                // Not Found 
            }

            // Find Total Amount for MEEZA 
            WSelectionCriteria = " WHERE RRNumber='" + WRRNumber + "'";
            WMode = 1;
            DMode = 2;

            Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, FileB_RRDM, FileB_RRDM_MATCHED, WSelectionCriteria,
                                                             WMode, DMode);
            if (Mmob.RecordFound == true)
            {
                MEEZA_Total = Mmob.TransAmount;
            }
            else
            {
                // Not Found 
            }

            if (Math.Abs(NODE_Total) == Math.Abs(MEEZA_Total))
            {
                Is_TOTALS_EQUAL = true;
                TOTALS_Difference = 0;
                labelWhatGrid.Text = "Source Data : " + WRRNumber + " TOTALS ARE EQUAL" ;
            }
            else
            {
                Is_TOTALS_EQUAL = false;
                TOTALS_Difference = Math.Abs(NODE_Total) - Math.Abs(MEEZA_Total);
                labelWhatGrid.Text = "Source Data : " + WRRNumber + " TOTALS DIFFER BY.... " + TOTALS_Difference.ToString("#,##0.00");
            }



            // RAW FILES
            // FileA_RRDM = W_Application+ ".[dbo]." + W_Application +"_NODE_TOTALS_TXNS"; // NODE
            FileA_RAW = In_Application + ".[dbo]." + "BULK_" + W_Application + "_NODE_TOTALS_TXNS" + "_ALL";
            FileB_RAW = In_Application + ".[dbo]." + "BULK_" + W_Application + "_MEEZA_TOTALS_TXNS" + "_ALL";

            // DETAIL FILES

            FileA_DETAIL = In_Application + ".[dbo]." + In_Application + "_NODE_DETAILS_TOTALS_TXNS" ;
            FileB_DETAIL = In_Application + ".[dbo]." + In_Application + "_MEEZA_DETAILS_TOTALS_TXNS";



            // [ETISALAT].[dbo].[ETISALAT_NODE_DETAILS_TOTALS_TXNS]
            //[ETISALAT].[dbo].[ETISALAT_MEEZA_DETAILS_TOTALS_TXNS]
            //TPF_Origin = false;

            //if (InOperator == "BCAIEGCX")
            //{
            //    PRX = "BDC"; // "+PRX+" eg "BDC"
            //}
            //else
            //{
            //    PRX = "EMR";
            //}

            //
            // INITIALISE FILES
            //
            //Master_Primary = InTableName;
            //Master_Matched = In_Application + "_MATCHED_TXNS.[dbo]." + In_Application + "_TPF_TXNS_MASTER";

            //RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
            //Mcs.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);

            //FileA = Mcs.SourceFileNameA;
            //FileB = Mcs.SourceFileNameB;
            //FileC = Mcs.SourceFileNameC;

            ////[ETISALAT].[dbo].BULK_ETISALAT_TPF_TXNS_ALL
            //FileA_RAW = In_Application + ".[dbo]." + "BULK_" + FileA + "_ALL";
            //// [ETISALAT].[dbo].[BULK_ETISALAT_MEEZA_TXNS_ALL]
            //FileB_RAW = In_Application + ".[dbo]." + "BULK_" + FileB + "_ALL";

            //// Check if TWINS AND CORRECT BULK ACCORDNGLY 
            //bool IsTWINS_A;
            //string substr = FileA.Substring(FileA.Length - 5); // TWINS

            //if (substr == "TWINS")
            //{
            //    IsTWINS_A = true;
            //    FileA_RAW = In_Application + ".[dbo]." + "BULK_" + In_Application + "_TPF_TXNS" + "_ALL";
            //}

            //bool IsTWINS_B;
            //substr = FileB.Substring(FileB.Length - 5); // TWINS

            //if (substr == "TWINS")
            //{
            //    IsTWINS_B = true;
            //    FileB_RAW = In_Application + ".[dbo]." + "BULK_" + In_Application + "_TPF_TXNS" + "_ALL";
            //}

            ////[ETISALAT].[dbo].[ETISALAT_TPF_TXNS]
            //FileA_RRDM = W_Application + ".[dbo]." + FileA;
            //FileB_RRDM = W_Application + ".[dbo]." + FileB;

            //FileA_RRDM_MATCHED = W_Application + "_MATCHED_TXNS" + ".[dbo]." + FileA;
            //FileB_RRDM_MATCHED = W_Application + "_MATCHED_TXNS" + ".[dbo]." + FileB;
            //string substr2 = FileA.Substring(FileA.Length - 8); // Ending at TPF_TXNS 
            //if (substr2 == "TPF_TXNS"
            // //|| FileA == "QAHERA_TPF_TXNS"
            // //|| FileA == "IPN_TPF_TXNS"

            // )
            //{
            //    TPF_Origin = true;
            //    // BULK_ETISALAT_TPF_TXNS_ALL
            //    // FileA_RAW = W_Application + ".[dbo]." + "BULK_" + FileA + "_ALL";
            //    FileA_RRDM = FileA_RRDM + "_MASTER";
            //    FileA_RRDM_MATCHED = FileA_RRDM_MATCHED + "_MASTER";
            //}

            //// FILL IN MATCHIND FIELDS 
            //ShowMatchingFields(WMatchingCateg);  // By Matching Category 

            //if (WMode == 1)
            //{
            //    // GET FROM MASTER WHAT IS NEEDED
            //    // read record
            //    WSelectionCriteria = " WHERE SeqNo = " + WSeqNo;

            //    MasterTableName = InTableName;
            //    //Master_Primary = InTableName; 
            //    //Master_Matched = In_Application + "_MATCHED_TXNS.[dbo]."+ In_Application+ "_TPF_TXNS_MASTER";
            //    //[ETISALAT_MATCHED_TXNS].[dbo].[ETISALAT_TPF_TXNS_MASTER]
            //    DMode = 2;
            //    if (FileA == "QAHERA_MEEZA_TXNS_SW")// Temporary here THIS IS EXCEPTION
            //    {
            //        FileA_RRDM = MasterTableName;
            //    }
            //    //Mmob.ReadTransSpecificFromSpecificTable_MOBILE(WSelectionCriteria, MasterTableName, 2);
            //    Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, Master_Primary, Master_Matched, WSelectionCriteria,
            //                                                   WMode, DMode);

            //    if (Mmob.RecordFound == true)
            //    {
            //        WRRNumber = Mmob.RRNumber;
            //        WTransAmount = Mmob.TransAmount;
            //        WMatchingCateg = Mmob.MatchingCateg;
            //        //WSendernumber = Mmob.Sendernumber;
            //        WRawSeqNo = Mmob.OriginalRecordId;
            //        WMatchMask = Mmob.MatchMask;

            //        textBoxRRNumber.Text = WRRNumber;
            //        textBoxAmt.Text = WTransAmount.ToString();
            //        textBoxDate.Text = Mmob.TransDate.ToString();

            //        // If MatchMask = "10" then the Record A exist and Record B doesnt exist
            //        // If MatchMask = "01" then the Record B exist and Record A doesnt exist

            //        labelWhatGrid.Text = "Source Data : " + WRRNumber + " AND MASK :" + Mmob.MatchMask;
            //    }
            //}

        }


        private void Form78b_Load(object sender, EventArgs e)
        {
            // GET AND SHOW NODE TOTALS = FileA_DETAIL
            // 
            WMode = 1;
            DMode = 1;
            WSelectionCriteria = " WHERE RRNumber='" + WRRNumber + "'";
            WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files(WOperator, FileA_DETAIL, FileA_DETAIL, WSelectionCriteria,
                                                           WMode, DMode);

            if (Mmob.TableDetails_RRDM.Rows.Count > 0)
            {
                // Record found 
                dataGridView1.DataSource = Mmob.TableDetails_RRDM.DefaultView;

                labelTOTAL_Analysis_NODE.Text = "DETAILS FOR _NODE_TOTAL of Amount:.." + NODE_Total.ToString("#,##0.00");
                //labelRespCode_1.Text = "RespCode_..:" + Mmob.ResponseCode;

            }
            else
            {
                labelRAW_NODE.Text = "RECORD DETAILS NOT FOUND FOR RRDM_:" + FileA_DETAIL;
                //labelRespCode_1.Hide();
            }

            // GET AND SHOW MEEZA TOTALS = FileB_DETAIL
            // 
            WMode = 1;
            DMode = 1;
            WSelectionCriteria = " WHERE RRNumber='" + WRRNumber + "'";
            WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files(WOperator, FileB_DETAIL, FileB_DETAIL, WSelectionCriteria,
                                                           WMode, DMode);

            if (Mmob.TableDetails_RRDM.Rows.Count > 0)
            {
                // Record found 
                dataGridView3.DataSource = Mmob.TableDetails_RRDM.DefaultView;

                labelTOTAL_Analysis_MEEZA.Text = "DETAILS FOR _MEEZA_TOTAL of Amount:.." + MEEZA_Total.ToString("#,##0.00");
                //labelRespCode_1.Text = "RespCode_..:" + Mmob.ResponseCode;

            }
            else
            {
                labelRAW_NODE.Text = "RECORD DETAILS NOT FOUND FOR RRDM_:" + FileA_DETAIL;
                //labelRespCode_1.Hide();
            }



            //// Read FileA BULK 
            //if (WMatchMask == "01")
            //{
            //    // A record doesn't exist
            //    labelTOTAL_Analysis_NOTE.Text = "RECORD DETAILS NOT FOUND FOR Original:" + FileA_RAW;
            //    labelRAW_NOTE.Text = "RECORD DETAILS NOT FOUND FOR RRDM_:" + FileA;
            //    labelRespCode_1.Hide();

            //}
            //else
            //{
            //    // WMatchMask ="11" or "10"
            //    WMode = 1;
            //    DMode = 1;
            //    WSelectionCriteria = " WHERE SeqNo=" + WRawSeqNo;
            //    WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RAW(WOperator, FileA_RAW, WSelectionCriteria,
            //                                                   WMode, DMode);

            //    if (Mmob.TableDetails_RAW.Rows.Count > 0)
            //    {
            //        // Record found 
            //        dataGridView1.DataSource = Mmob.TableDetails_RAW.DefaultView;

            //        labelTOTAL_Analysis_NOTE.Text = "RECORD DETAILS FOR Original:" + FileA_RAW;


            //    }
            //    else
            //    {
            //        labelTOTAL_Analysis_NOTE.Text = "RECORD DETAILS NOT FOUND FOR Original:" + FileA_RAW;
            //        labelRespCode_1.Hide();
            //    }

            //    // Read FileA RRDM

            //    WMode = 1;
            //    DMode = 2;
            //   // WSelectionCriteria = " WHERE SeqNo=" + WSeqNo;
            //    WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files(WOperator, Master_Primary, Master_Matched, WSelectionCriteria,
            //                                                   WMode, DMode);

            //    if (Mmob.TableDetails_RRDM.Rows.Count > 0)
            //    {
            //        // Record found 
            //        dataGridView2.DataSource = Mmob.TableDetails_RRDM.DefaultView;

            //        labelRAW_NOTE.Text = "RECORD DETAILS FOR RRDM_:" + Master_Primary;
            //        labelRespCode_1.Text = "RespCode_..:" + Mmob.ResponseCode;

            //    }
            //    else
            //    {
            //        labelRAW_NOTE.Text = "RECORD DETAILS NOT FOUND FOR RRDM_:" + Master_Primary;
            //        labelRespCode_1.Hide();
            //    }
            //}


            //// File B BULK
            //// FIND THE RRDM FILE FIRST AND THEN THE RAW
            //int B_SeqNo;
            //// Take the original SeqNumber for RAW
            //long WWRawSeqNo;

            //WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'"
            //                     + " AND TransAmount =" + WTransAmount
            //                       ;
            ////Mmob.ReadTransSpecificFromSpecificTable_MOBILE(WSelectionCriteria, FileB_RRDM, 2);
            //Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, FileB_RRDM, FileB_RRDM_MATCHED, WSelectionCriteria,
            //                                                   WMode, DMode);


            //if (Mmob.RecordFound == true)
            //{
            //    // OK 
            //    B_SeqNo = Mmob.SeqNo;
            //    // Take the original SeqNumber for RAW
            //    WWRawSeqNo = Mmob.OriginalRecordId;
            //}
            //else
            //{
            //    // Check other sign for Amount
            //    WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'"
            //                     + " AND TransAmount =" + (-WTransAmount)
            //                       ;
            //    //Mmob.ReadTransSpecificFromSpecificTable_MOBILE(WSelectionCriteria, FileB_RRDM, 2);
            //    Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, FileB_RRDM, FileB_RRDM_MATCHED, WSelectionCriteria,
            //                                                       WMode, DMode);
            //    if (Mmob.RecordFound == true)
            //    {
            //        // OK 
            //        B_SeqNo = Mmob.SeqNo;
            //        // Take the original SeqNumber for RAW
            //        WWRawSeqNo = Mmob.OriginalRecordId;
            //    }
            //    else
            //    {
            //        B_SeqNo = 0;
            //        // Take the original SeqNumber for RAW
            //        WWRawSeqNo = 0;
            //    }


            //}



            //WMode = 1;
            //DMode = 1;
            //WSelectionCriteria = " WHERE SeqNo=" + WWRawSeqNo;
            //WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RAW(WOperator, FileB_RAW, WSelectionCriteria,
            //                                               WMode, DMode);

            //if (Mmob.TableDetails_RAW.Rows.Count > 0)
            //{
            //    // Record found 
            //    dataGridView3.DataSource = Mmob.TableDetails_RAW.DefaultView;

            //    labelTOTAL_Analysis_MEEZA.Text = "RECORD DETAILS FOR Original:" + FileB_RAW;
            //    // labelRespCode_1.Text = "RespCode_..:" + Mmob.ResponseCode;

            //}
            //else
            //{
            //    labelTOTAL_Analysis_MEEZA.Text = "RECORD DETAILS NOT FOUND Original:" + FileB_RAW;

            //}

            //// File B RRDM 
            //WMode = 1;
            //DMode = 2;
            //WSelectionCriteria = " WHERE SeqNo=" + B_SeqNo;
            //WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files(WOperator, FileB_RRDM, FileB_RRDM_MATCHED, WSelectionCriteria,
            //                                               WMode, DMode);

            //if (Mmob.TableDetails_RRDM.Rows.Count > 0)
            //{
            //    // Record found 
            //    dataGridView4.DataSource = Mmob.TableDetails_RRDM.DefaultView;

            //    labelRAW_MEEZA.Text = "RECORD DETAILS FOR RRDM_:" + FileB_RRDM;
            //    labelRespCode_2.Text = "RespCode_..:" + Mmob.ResponseCode;

            //}
            //else
            //{
            //    labelRAW_MEEZA.Text = "RECORD DETAILS NOT FOUND FOR RRDM_ :" + FileB_RRDM;
            //    labelRespCode_2.Hide();
            //}

        }

        // On ROW ENTER 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WRawSeqNo = (long)rowSelected.Cells[1].Value;

            // READ RECORD AND FIND SEQNO

            int SeqNo_RAW = 0;

            // WMatchMask ="11" or "10"
            WMode = 1;
            DMode = 1;
            WSelectionCriteria = " WHERE SeqNo=" + WRawSeqNo;
            WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RAW(WOperator, FileA_RAW, WSelectionCriteria,
                                                           WMode, DMode, W_Application);

            if (Mmob.TableDetails_RAW.Rows.Count > 0)
            {
                // Record found 
                dataGridView2.DataSource = Mmob.TableDetails_RAW.DefaultView;

                labelRAW_NODE.Text = "RECORD FOR Original NODE For SELECTED LIne" ;


            }
            else
            {
                labelRAW_NODE.Text = "RECORD DETAILS NOT FOUND FOR NODE For SELECTED LIne";
                //labelRespCode_1.Hide();
            }

        }

        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            WRawSeqNo = (long)rowSelected.Cells[1].Value;

            // READ RECORD AND FIND SEQNO



            // WMatchMask ="11" or "10"
            WMode = 1;
            DMode = 1;
            WSelectionCriteria = " WHERE SeqNo=" + WRawSeqNo;
            WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RAW(WOperator, FileB_RAW, WSelectionCriteria,
                                                           WMode, DMode, W_Application);

            if (Mmob.TableDetails_RAW.Rows.Count > 0)
            {
                // Record found 
                dataGridView4.DataSource = Mmob.TableDetails_RAW.DefaultView;

                labelRAW_NODE.Text = "RECORD FOR Original MEEZA For SELECTED Line";


            }
            else
            {
                labelRAW_NODE.Text = "RECORD DETAILS NOT FOUND FOR MEEZA For SELECTED Line";
                //labelRespCode_1.Hide();
            }

        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //private void ShowMatchingFields(string InMatchingCateg)
        //{
        //    RRDMMatchingCategStageVsMatchingFields Rsm = new RRDMMatchingCategStageVsMatchingFields();

        //    Rsm.ReadReconcCategVsMatchingFieldsDataTableByMatchingCateg(InMatchingCateg);

        //    dataGridView9.DataSource = Rsm.ReconcCategStagesDataTable.DefaultView;

        //    if (dataGridView9.Rows.Count == 0)
        //    {
        //        return;
        //    }

        //    dataGridView9.Columns[0].Width = 100; // MatchingCateg
        //    dataGridView9.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        //    dataGridView9.Columns[1].Width = 270; // MatchingField
        //    dataGridView9.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        //}


    }
}

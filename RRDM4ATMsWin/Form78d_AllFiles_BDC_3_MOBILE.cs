using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_AllFiles_BDC_3_MOBILE : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMJournalAudi_BDC Msr = new RRDMJournalAudi_BDC();
       // RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
        //RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //int MaskLength;

        string WRRNumber;
        decimal WTransAmount;

        string WMatchMask;

        string WSelectionCriteria;

        bool RRNBased = false;

        string PRX;

        bool IsOurAtm = false;

        string Master_Primary;
        string Master_Matched;

        string FileA;
        string FileB;
        string FileC;

        string FileA_RAW;
        string FileB_RAW;


        string FileA_RRDM;
        string FileB_RRDM;


        string FileA_RRDM_MATCHED;
        string FileB_RRDM_MATCHED;


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

        string WOperator;
        string WSignedId;
        int WSeqNo;
        string WMatchingCateg;
        int WMode;
        string W_Application;

        public Form78d_AllFiles_BDC_3_MOBILE(string InOperator, string InSignedId, int InSeqNo, string InMatchingCateg,
                                    int InMode, string InTableName, string In_Application)
        {
            WSignedId = InSignedId;

            WOperator = InOperator;

            WSeqNo = InSeqNo;
            WMatchingCateg = InMatchingCateg;
            WMode = InMode; // 1 Based on MOBILE MASTER
                            // 2 Based on ....
                            // 4 Based on ...... 

            W_Application = In_Application;

            InitializeComponent();

            TPF_Origin = false;

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }

            //
            // INITIALISE FILES
            //
            Master_Primary = InTableName;
            Master_Matched = In_Application + "_MATCHED_TXNS.[dbo]." + In_Application + "_TPF_TXNS_MASTER";

            RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
            Mcs.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);

            FileA = Mcs.SourceFileNameA;
            FileB = Mcs.SourceFileNameB;
            FileC = Mcs.SourceFileNameC;

            //[ETISALAT].[dbo].BULK_ETISALAT_TPF_TXNS_ALL
            FileA_RAW = In_Application + ".[dbo]." + "BULK_" + FileA + "_ALL";
            // [ETISALAT].[dbo].[BULK_ETISALAT_MEEZA_TXNS_ALL]
            FileB_RAW = In_Application + ".[dbo]." + "BULK_" + FileB + "_ALL";
            // This is an exception
            if (FileA == "QAHERA_MEEZA_TXNS_SW")
            {
                FileA_RAW = In_Application + ".[dbo]." + "BULK_" + "QAHERA_MEEZA_TXNS" + "_ALL";
            }

            // Check if TWINS AND CORRECT BULK ACCORDNGLY 
            bool IsTWINS_A;
         
            string substr = FileA.Substring(FileA.Length - 0); // TWINS
            
            if (substr == "TWINS")
            {
                IsTWINS_A = true;
                FileA_RAW = In_Application + ".[dbo]." + "BULK_" + In_Application + "_TPF_TXNS" + "_ALL";
            }

            bool IsTWINS_B;
            substr = FileB.Substring(FileB.Length - 5); // TWINS

            if (substr == "TWINS")
            {
                IsTWINS_B = true;
                FileB_RAW = In_Application + ".[dbo]." + "BULK_" + In_Application + "_TPF_TXNS" + "_ALL";
            }

            //[ETISALAT].[dbo].[ETISALAT_TPF_TXNS]
            FileA_RRDM = W_Application + ".[dbo]." + FileA;
            FileB_RRDM = W_Application + ".[dbo]." + FileB;

            FileA_RRDM_MATCHED = W_Application + "_MATCHED_TXNS" + ".[dbo]." + FileA;
            FileB_RRDM_MATCHED = W_Application + "_MATCHED_TXNS" + ".[dbo]." + FileB;
            string substr2 = FileA.Substring(FileA.Length - 8); // Ending at TPF_TXNS 
            if (substr2 == "TPF_TXNS"
             //|| FileA == "QAHERA_TPF_TXNS"
             //|| FileA == "IPN_TPF_TXNS"

             )
            {
                TPF_Origin = true;
                // BULK_ETISALAT_TPF_TXNS_ALL
                // FileA_RAW = W_Application + ".[dbo]." + "BULK_" + FileA + "_ALL";
                FileA_RRDM = FileA_RRDM + "_MASTER";
                FileA_RRDM_MATCHED = FileA_RRDM_MATCHED + "_MASTER";
            }

            // FILL IN MATCHIND FIELDS 
            ShowMatchingFields(WMatchingCateg);  // By Matching Category 

            if (WMode == 1)
            {
                // GET FROM MASTER WHAT IS NEEDED
                // read record
                WSelectionCriteria = " WHERE SeqNo = " + WSeqNo;

                MasterTableName = InTableName;
                //Master_Primary = InTableName; 
                //Master_Matched = In_Application + "_MATCHED_TXNS.[dbo]."+ In_Application+ "_TPF_TXNS_MASTER";
                //[ETISALAT_MATCHED_TXNS].[dbo].[ETISALAT_TPF_TXNS_MASTER]
                DMode = 2;
                if (FileA == "QAHERA_MEEZA_TXNS_SW")// Temporary here THIS IS EXCEPTION
                {
                    FileA_RRDM = MasterTableName;
                }
                //Mmob.ReadTransSpecificFromSpecificTable_MOBILE(WSelectionCriteria, MasterTableName, 2);
                if (Master_Primary == "")
                {
                    MessageBox.Show("This Is Blank");
                }
                Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, Master_Primary, Master_Matched, WSelectionCriteria,
                                                               WMode, DMode);

                if (Mmob.RecordFound == true)
                {
                    WRRNumber = Mmob.RRNumber;
                    WTransAmount = Mmob.TransAmount;
                    WMatchingCateg = Mmob.MatchingCateg;
                    //WSendernumber = Mmob.Sendernumber;
                    WRawSeqNo = Mmob.OriginalRecordId;
                    WMatchMask = Mmob.MatchMask;

                    textBoxRRNumber.Text = WRRNumber;
                    textBoxAmt.Text = WTransAmount.ToString();
                    textBoxDate.Text = Mmob.TransDate.ToString();

                    // If MatchMask = "10" then the Record A exist and Record B doesnt exist
                    // If MatchMask = "01" then the Record B exist and Record A doesnt exist

                    labelWhatGrid.Text = "Source Data : " + WRRNumber + " AND MASK :" + Mmob.MatchMask;
                }
                else
                {
                    MessageBox.Show("1265 Not Found Record"); 
                }
            }

        }


        private void Form78b_Load(object sender, EventArgs e)
        {
            // Read FileA BULK 
            if (WMatchMask == "01")
            {
                // A record doesn't exist
                label1.Text = "RECORD DETAILS NOT FOUND FOR Original:" + FileA_RAW;
                label2.Text = "RECORD DETAILS NOT FOUND FOR RRDM_:" + FileA;
                labelRespCode_1.Hide();

            }
            else
            {
                // WMatchMask ="11" or "10"
                WMode = 1;
                DMode = 1;
                WSelectionCriteria = " WHERE SeqNo=" + WRawSeqNo;
                WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RAW(WOperator, FileA_RAW, WSelectionCriteria,
                                                               WMode, DMode,W_Application);

                if (Mmob.TableDetails_RAW.Rows.Count > 0)
                {
                    // Record found 
                    dataGridView1.DataSource = Mmob.TableDetails_RAW.DefaultView;

                    label1.Text = "RECORD DETAILS FOR Original:" + FileA_RAW;


                }
                else
                {
                    label1.Text = "RECORD DETAILS NOT FOUND FOR Original:" + FileA_RAW;
                    labelRespCode_1.Hide();
                }

                // Read FileA RRDM

                WMode = 1;
                DMode = 2;
                if (WMatchingCateg == "ETI360" || WMatchingCateg == "QAH360" || WMatchingCateg == "IPL360")
                {
                    WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'";
                    //  + " AND TransAmount =" + WTransAmount
                }
                else
                {
                    WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'"
                                                    + " AND TransAmount =" + WTransAmount;
                }
               // WSelectionCriteria = " WHERE SeqNo=" + WSeqNo;
                WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files(WOperator, Master_Primary, Master_Matched, WSelectionCriteria,
                                                               WMode, DMode);

                if (Mmob.TableDetails_RRDM.Rows.Count > 0)
                {
                    // Record found 
                    dataGridView2.DataSource = Mmob.TableDetails_RRDM.DefaultView;

                    label2.Text = "RECORD DETAILS FOR RRDM_:" + Master_Primary;
                    labelRespCode_1.Text = "RespCode_..:" + Mmob.ResponseCode;

                }
                else
                {
                    label2.Text = "RECORD DETAILS NOT FOUND FOR RRDM_:" + Master_Primary;
                    labelRespCode_1.Hide();
                }
            }


            // File B BULK
            // FIND THE RRDM FILE FIRST AND THEN THE RAW
            int B_SeqNo;
            // Take the original SeqNumber for RAW
            long WWRawSeqNo;
            if (WMatchingCateg == "ETI360" || WMatchingCateg == "QAH360" || WMatchingCateg == "IPL360")
            {
                WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'";
                //  + " AND TransAmount =" + WTransAmount
            }
            else
            {
                WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'"
                                                + " AND TransAmount =" + WTransAmount ;
            }
           
                                   
            if (FileB_RRDM == "")
            {
                MessageBox.Show("This Is Blank");
            }
            //Mmob.ReadTransSpecificFromSpecificTable_MOBILE(WSelectionCriteria, FileB_RRDM, 2);
            Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, FileB_RRDM, FileB_RRDM_MATCHED, WSelectionCriteria,
                                                               WMode, DMode);


            if (Mmob.RecordFound == true)
            {
                // OK 
                B_SeqNo = Mmob.SeqNo;
                // Take the original SeqNumber for RAW
                WWRawSeqNo = Mmob.OriginalRecordId;
            }
            else
            {
                // Check other sign for Amount
                if (WMatchingCateg == "ETI360" || WMatchingCateg == "QAH360" || WMatchingCateg == "IPL360")
                {
                    WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'"
                                                   //  + " AND TransAmount =" + (-WTransAmount)
                                                       ;
                }
                else
                {
                    WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'"
                                                     + " AND TransAmount =" + (-WTransAmount)
                                                       ;
                }
                
                

                if (FileB_RRDM == "")
                {
                    MessageBox.Show("This Is Blank");
                }
                //Mmob.ReadTransSpecificFromSpecificTable_MOBILE(WSelectionCriteria, FileB_RRDM, 2);
                Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, FileB_RRDM, FileB_RRDM_MATCHED, WSelectionCriteria,
                                                                   WMode, DMode);
                if (Mmob.RecordFound == true)
                {
                    // OK 
                    B_SeqNo = Mmob.SeqNo;
                    // Take the original SeqNumber for RAW
                    WWRawSeqNo = Mmob.OriginalRecordId;
                }
                else
                {
                    B_SeqNo = 0;
                    // Take the original SeqNumber for RAW
                    WWRawSeqNo = 0;
                }

            }



            WMode = 1;
            DMode = 1;
            WSelectionCriteria = " WHERE SeqNo=" + WWRawSeqNo;
            WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RAW(WOperator, FileB_RAW, WSelectionCriteria,
                                                           WMode, DMode,W_Application);

            if (Mmob.TableDetails_RAW.Rows.Count > 0)
            {
                // Record found 
                dataGridView3.DataSource = Mmob.TableDetails_RAW.DefaultView;

                label3.Text = "RECORD DETAILS FOR Original:" + FileB_RAW;
                // labelRespCode_1.Text = "RespCode_..:" + Mmob.ResponseCode;

            }
            else
            {
                label3.Text = "RECORD DETAILS NOT FOUND Original:" + FileB_RAW;

            }

            // File B RRDM 

            WMode = 1;
            DMode = 2;
            if (WMatchingCateg == "ETI360" || WMatchingCateg == "QAH360" || WMatchingCateg == "IPL360")
            {
                WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'";
                //  + " AND TransAmount =" + WTransAmount
            }
            else
            {
                WSelectionCriteria = " WHERE RRNumber = '" + WRRNumber + "'"
                                                + " AND TransAmount =" + WTransAmount;
            }
            //WSelectionCriteria = " WHERE SeqNo=" + B_SeqNo;
            WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files(WOperator, FileB_RRDM, FileB_RRDM_MATCHED, WSelectionCriteria,
                                                           WMode, DMode);

            if (Mmob.TableDetails_RRDM.Rows.Count > 0)
            {
                // Record found 
                dataGridView4.DataSource = Mmob.TableDetails_RRDM.DefaultView;

                label4.Text = "RECORD DETAILS FOR RRDM_:" + FileB_RRDM;
                labelRespCode_2.Text = "RespCode_..:" + Mmob.ResponseCode;

            }
            else
            {
                label4.Text = "RECORD DETAILS NOT FOUND FOR RRDM_ :" + FileB_RRDM;
                labelRespCode_2.Hide();
            }


            // SHOW MONEY GRAM CONNECTOR
            if (WMatchingCateg == "ETI376")
            {
                // Show Connector 
                WMode = 1;
                DMode = 1;
                string File_Connector_RAW = "[ETISALAT].[dbo].[BULK_ETISALAT_7_MoneyGrane_Connector_TXNS]"; 
                WSelectionCriteria = " WHERE TRE_CAR_NUMB = '" + WRRNumber + "'  ";
                WRecordFoundInUniversal = Mmob.FindSourceRecords_MOBILE_2_RAW(WOperator, File_Connector_RAW, WSelectionCriteria,
                                                               WMode, DMode, W_Application);

                if (Mmob.TableDetails_RAW.Rows.Count > 0)
                {
                    // Record found 
                    dataGridView5.DataSource = Mmob.TableDetails_RAW.DefaultView;

                    labelConnector.Text = "RECORD DETAILS FOR Original:" + File_Connector_RAW;


                }
                else
                {
                    labelConnector.Text = "RECORD DETAILS NOT FOUND FOR Original:" + File_Connector_RAW;
                    labelRespCode_1.Hide();
                }

            }
            else
            {
                labelConnector.Hide();

                dataGridView5.Hide(); 
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

        private void ShowMatchingFields(string InMatchingCateg)
        {
            RRDMMatchingCategStageVsMatchingFields Rsm = new RRDMMatchingCategStageVsMatchingFields();

            Rsm.ReadReconcCategVsMatchingFieldsDataTableByMatchingCateg(InMatchingCateg);

            dataGridView9.DataSource = Rsm.ReconcCategStagesDataTable.DefaultView;

            if (dataGridView9.Rows.Count == 0)
            {
                return;
            }

            dataGridView9.Columns[0].Width = 100; // MatchingCateg
            dataGridView9.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView9.Columns[1].Width = 270; // MatchingField
            dataGridView9.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
    }
}

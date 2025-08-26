using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;
using System.Xml.Linq;
using RRDM4ATMs;
using System.Xml.Linq;
using System.Data;
using System.IO;


namespace RRDM4ATMsWin
{
    public partial class Form78d_Discre_MOBILE : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
        RRDMMatchingTxns_InGeneralTables_EGATE M_EGATE = new RRDMMatchingTxns_InGeneralTables_EGATE();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();
        // FIND CUTOFF CYCLE
        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime WCut_Off_Date;
        string WTableName;
        string WTableName_MATCHED;

        string FileB_RRDM ;
        string FileB_RRDM_MATCHED ;

        string FileB = "";

        bool TPF_Origin;

        string WUser; 

        // int WSeqNo;

        string WOperator;
        string WSignedId;
        string WMatchingCateg;
        string WTransType; 
        int WRMCycle;
        int WMode;
       
        string W_Application; 

        public Form78d_Discre_MOBILE(string InOpertor , string InSignedId, string InMatchingCateg, string InTransType,
                                               int InRMCycle, int InMode, string InApplication)
        {
            WOperator = InOpertor;
            WSignedId = InSignedId;
            WMatchingCateg = InMatchingCateg;
            WTransType = InTransType; 
            WRMCycle = InRMCycle;
            WMode = InMode;
            W_Application = InApplication;
            

            // WMode = 1 = Unmatched for Category
            // WMode = 2 = Unmatched for All at cycle
            // WMode = 3 = Unmatched Per Category and TransType
            // WMode = 4 = Unmatched for Category and TransType showing the 10 from master 
            // WMode = 6 = Matched for Category alone or By transType

            // WMode = 8 = GL Entries
            // WMode = 9 = Show Matched for second File for Category and TransType
            // WMode = 10 = Show Un Matched for second File for Category and TransType Mask = 01

            // WMode = 14 = Show Disputes
            // WMode = 15 = Show Surplus 

            // WMode = 17 = Show Checkers Audit TRAIL 

            InitializeComponent();

            if (WMode == 17)
            {
                // This is a special for History of users changes 
                WUser = WMatchingCateg; 
            }
            else
            {
                RRDM_MOBILE_TABLE_NAMES Mn = new RRDM_MOBILE_TABLE_NAMES();

                Mn.GetFileNames(W_Application);

                WTableName = Mn.WTableName_Master_Primary;
                WTableName_MATCHED = Mn.WTableName_Master_Matched;

                // Find Files
                // string FileA = "";


                RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
                Mcs.ReadReconcCategoriesVsSourcesAll(InMatchingCateg);

                //  FileA = Mcs.SourceFileNameA; // EG ETISALAT_TPF_TXNS
                FileB = Mcs.SourceFileNameB; // EG ETISALAT_TPF_TXNS 

                FileB_RRDM = W_Application + ".[dbo]." + FileB;
                FileB_RRDM_MATCHED = W_Application + "_MATCHED_TXNS.[dbo]." + FileB;
            }
        }
        string WSelectionCriteria;
        private void Form78b_Load(object sender, EventArgs e)
        {
          
            if (WMode == 1)
            {
                // UNMATCHED PER CATEGORY
                WSelectionCriteria = " WHERE IsMatchingDone = 1 And Matched = 0 AND IsReversal = 0"
                                         + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle
                                                                               ; 
                int DB_Mode = 1;
                if (W_Application == "EGATE")
                {
                    M_EGATE.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                                                   NullPastDate, NullPastDate, DB_Mode, W_Application);

                    dataGridView1.DataSource = M_EGATE.DataTableAllFields.DefaultView;
                }
                else
                {
                    Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                               NullPastDate, NullPastDate, DB_Mode, W_Application);

                    dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;
                }
                

                labelWhatGrid.Text = "DISCREPANCIES_BY CATEGORY_" + WMatchingCateg +"_FOR_CYCLE_"+ WRMCycle.ToString();
                dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

            }
            if (WMode == 2)
            {
                //if (W_Application == "EGATE")
                //{
                //    MessageBox.Show("Get The Discrepancies one by one");
                //    return; 
                //}
                    // UNMATCHED PER Cycle 
                WSelectionCriteria = " WHERE IsMatchingDone = 1 And Matched = 0 AND IsReversal = 0"
                                         // + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle
                                                                               ;
                int DB_Mode = 1;
                Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                               NullPastDate, NullPastDate, DB_Mode, W_Application);

                dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;

                labelWhatGrid.Text = "DISCREPANCIES_FOR_ALL_CATEGORIES_FOR_CYCLE_" + WRMCycle.ToString();
                dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

            }
            if (WMode == 3)
            {
                // Unmatched Per Category and TransType
                WSelectionCriteria = " WHERE IsMatchingDone = 1 And Matched = 0 AND IsReversal = 0"
                     + " AND TransType = '" + WTransType + "'"
                                         + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle ;
                int DB_Mode = 1 ;
                Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                               NullPastDate, NullPastDate, WMode, W_Application);

                dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;

                labelWhatGrid.Text = "DISCREPANCIES_BY_CATEGORY_" + WMatchingCateg + "_FOR_CYCLE_" + WRMCycle.ToString();
                dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

            }
            if (WMode == 4)
            {

                WSelectionCriteria = " WHERE IsMatchingDone = 1 And Matched = 0 AND IsReversal = 0  AND MatchMask = '10' "
                                         + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle
                                                                               ;
                int DB_Mode = 1;
                Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                               NullPastDate, NullPastDate, DB_Mode, W_Application);

                dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;

                labelWhatGrid.Text = "DISCREPANCIES_Mask=10_BY CATEGORY_" + WMatchingCateg + "_FOR_CYCLE_" + WRMCycle.ToString();
                dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

            }

            if (WMode == 6)
            {
                if (WTransType != "")
                {
                    WSelectionCriteria = " WHERE IsMatchingDone = 1 AND IsReversal = 0 "
                                         + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                          + " AND TransType = '" + WTransType + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle;
                    labelWhatGrid.Text = "MATCHED+UnMatched_BY_CATEGORY_And TransType.." + WMatchingCateg + "_FOR_CYCLE_" + WRMCycle.ToString();
                }
                else
                {
                    WSelectionCriteria = " WHERE IsMatchingDone = 1  AND IsReversal = 0 "
                                         + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle;
                    labelWhatGrid.Text = "MATCHED+UnMatched_BY_CATEGORY_" + WMatchingCateg + "_FOR_CYCLE_" + WRMCycle.ToString();
                }
                                                                   
                int WMode = 2; // GET From Matched and unmatched 

                if (W_Application == "EGATE")
                {
                    M_EGATE.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                                                   NullPastDate, NullPastDate, WMode, W_Application);

                    dataGridView1.DataSource = M_EGATE.DataTableAllFields.DefaultView;
                }
                else
                {
                    Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                               NullPastDate, NullPastDate, WMode, W_Application);

                    dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;
                }

                //Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                //                                               NullPastDate, NullPastDate, WMode, W_Application);

                //dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;

                
                dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

            }

            if (WMode == 8)
            {
                // GET THE RIGHT GL FILE 
                // [ETISALAT].[dbo].[HST_ETISALAT_GL_ENTRIES]
                string WGL_Entries_TABLE = W_Application + ".[dbo].[HST_"+W_Application+ "_GL_ENTRIES]";

                Mmob.Read_GL_TXNS_FOR_Cycle(WGL_Entries_TABLE, WRMCycle); 

                dataGridView1.DataSource = Mmob.DataTableSelectedFields.DefaultView;

                ShowGrid_GL(); 

                labelWhatGrid.Text = "GL ENTRIES" + "_FOR_CYCLE_" + WRMCycle.ToString();
               // dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableSelectedFields.Rows.Count.ToString();

                labelSeqNo.Hide();

                textBoxSeqNo.Hide();

                buttonSourceRecords.Hide(); 

            }

            if (WMode == 9)   // MATCHED SECOND FILE
            {
                if (WTransType != "")
                {
                    WSelectionCriteria = " WHERE IsMatchingDone = 1 AND IsReversal = 0 "
                                         + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                          + " AND TransType = '" + WTransType + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle;
                    labelWhatGrid.Text = "MATCHED+Unmatched_BY_CATEGORY_Trans_Type.." + WMatchingCateg + "_" + WTransType + "_FOR_CYCLE_" + WRMCycle.ToString();
                }
                else
                {
                    WSelectionCriteria = " WHERE IsMatchingDone = 1 And Matched = 0 AND IsReversal = 0 "
                                         + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle;
                    labelWhatGrid.Text = "MATCHED_BY_CATEGORY.." + WMatchingCateg + "_" + WTransType + "_FOR_CYCLE_" + WRMCycle.ToString();
                }
                //FileB_RRDM = W_Application + ".[dbo]." + FileB;
                //FileB_RRDM_MATCHED = W_Application + "_MATCHED_TXNS.[dbo]." + FileB;
                int WMode = 3; // GET From Matched 
                Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(FileB_RRDM, FileB_RRDM_MATCHED, WSelectionCriteria,
                                                               NullPastDate, NullPastDate, WMode, W_Application);

                dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;

               
                dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

            }

            if (WMode == 10)   // UN MATCHED SECOND FILE CATEGORY and TransType
            {
                if (WTransType != "")
                {
                    WSelectionCriteria = " WHERE IsMatchingDone = 1 And Matched = 0 AND IsReversal = 0 AND MatchMask = '01' "
                                         + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                          + " AND TransType = '" + WTransType + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle;
                }
                else
                {
                    WSelectionCriteria = " WHERE IsMatchingDone = 1 And Matched = 0 AND IsReversal = 0 "
                                         + " AND MatchingCateg = '" + WMatchingCateg + "'"
                                         + " AND MatchingAtRMCycle = " + WRMCycle;
                }

                int WMode = 1; // GET From UN Matched 
                Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                               NullPastDate, NullPastDate, WMode, W_Application);

                dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;

                labelWhatGrid.Text = "UN MATCHED_BY_CATEGORY_AND TransType.." + WMatchingCateg+" "+ WTransType + "_FOR_CYCLE_" + WRMCycle.ToString();
                dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

            }

            if (WMode == 14)
            {
               // DISPUTES
               
                // [ETISALAT].[dbo].[ETISALAT_CUSTOMER_DISPUTES_TXNS]
                string Disputes_TABLE = W_Application + ".[dbo].[" + W_Application + "_CUSTOMER_DISPUTES_TXNS]";

                Mmob.Read_TXNS_FOR_File_AND_Cycle_Disputes(Disputes_TABLE, WRMCycle);

                dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;

                ShowGrid_GL();

                labelWhatGrid.Text = "DISPUTES ENTRIES" + "_FOR_CYCLE_" + WRMCycle.ToString();
                // dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

                labelSeqNo.Hide();

                textBoxSeqNo.Hide();

                buttonSourceRecords.Hide();

            }

            if (WMode == 15)
            {
               
                // [ETISALAT].[dbo].[ETISALAT_CUSTOMER_SURPLUS_TXNS]
                string Surplus_TABLE = W_Application + ".[dbo].[" + W_Application + "_CUSTOMER_SURPLUS_TXNS]";

                Mmob.Read_TXNS_FOR_File_AND_Cycle_Surplus(Surplus_TABLE, WRMCycle);

                dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;

                ShowGrid_GL();

                labelWhatGrid.Text = "SURPLUS ENTRIES" + "_FOR_CYCLE_" + WRMCycle.ToString();
                // dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

                labelSeqNo.Hide();

                textBoxSeqNo.Hide();

                buttonSourceRecords.Hide();

            }

            if (WMode == 17)
            {
                RRDMUsersRecords Us = new RRDMUsersRecords();

                Us.Read_HST_TXNS_FOR_USER(WUser); 
                
                dataGridView1.DataSource = Us.UserHistoryDataTable.DefaultView;

               // ShowGrid_GL();

                labelWhatGrid.Text = "HISTORY RECORDS FOR USER.." + WUser;
                // dataGridView1.Show();

                textBoxTotalRec.Text = Us.UserHistoryDataTable.Rows.Count.ToString();

                labelSeqNo.Hide();

                textBoxSeqNo.Hide();

                buttonSourceRecords.Hide();

            }


            // SHOW GRID

        }


        private void ShowGrid_GL()
        {
            
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No GL to show"); 
                return;
            }
            else
            {
                dataGridView1.Columns[0].Width = 60; //Seqno
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[0].Visible = true;

                dataGridView1.Columns[1].Width = 90; //businessDate
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[1].Visible = true;

                dataGridView1.Columns[2].Width = 90; // GLacccount
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[2].Visible = true;

                dataGridView1.Columns[3].Width = 200; // GLdescription
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[3].Visible = true;

                dataGridView1.Columns[4].Width = 50; // type
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[4].Visible = true;

                dataGridView1.Columns[5].Width = 90; // amount
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[5].Visible = true;

                dataGridView1.Columns[6].Width = 70; // LoadedAtRMCycle
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[6].Visible = true;

            }
        }


        // On ROW ENTER 
        int WSeqNo;
        int WWSeqNo;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
            

            if (WMode ==9 || WMode == 10)
            {
                // Find The MASTER SEQNO 
                string RRNumber = (string)dataGridView1.Rows[e.RowIndex].Cells["RRNumber"].Value;
                decimal TransAmount = (decimal)dataGridView1.Rows[e.RowIndex].Cells["TransAmount"].Value;

                string Filter = " WHERE RRNumber='"+ RRNumber+ "'  AND TransAmount = "+ TransAmount + " AND MatchingAtRMCycle = " + WRMCycle;
                Mmob.ReadTransSpecificFromSpecificTable_MOBILE(Filter, WTableName, 2, W_Application);

                if (Mmob.RecordFound == true)
                {
                    WWSeqNo = Mmob.SeqNo; 
                }
            }

            labelCustId.Hide();
            textBoxCust_ID.Hide();

            if (WMode == 14 || WMode == 15)
            {
                //labelCustId.Show();
                //textBoxCust_ID.Show();
                ////MeezaDigitalTrxId = '4BCA1043-886D-4CCC-9820-BA3753E332AA'
                //string Transaction_Id = (string)dataGridView1.Rows[e.RowIndex].Cells["Transaction Id"].Value;
               
                //Mmob.ReadCustomerIdFrom_Bulk_TPF_MOBILE(Transaction_Id, W_Application); 

                //if (Mmob.RecordFound == true)
                //{
                //    textBoxCust_ID.Text = Mmob.CustomerID; 
                //}
                //else
                //{
                //    textBoxCust_ID.Text = "Not Found" ;
                //}
                
            }

            textBoxSeqNo.Text = WSeqNo.ToString();


        }
        // ROW ENTER FOR 2
        string WAtmNo; 
        

// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }


        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print
            string P1 = "";



            if (WMode == 3)
            {
                P1 = "Discrepancies for Category.." + WMatchingCateg + "..At Cycle.." + WRMCycle.ToString();
            }
            else
            {
                MessageBox.Show("No Printing for this"); 
            }

            string P2 = "";  // Category 
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

                Form56R68ATMS_W_Pool_Table ReportATMS_Pool_Table = new Form56R68ATMS_W_Pool_Table(P1, P2, P3, P4, P5);
                ReportATMS_Pool_Table.Show();
           
        }

        private void buttonXML_Click(object sender, EventArgs e)
        {
            //Mmob.DataTableAllFields
            string FileA = "[ETISALAT].[dbo].[ETISALAT_TPF_TXNS_MASTER]";
            string FileB_Matched = "[ETISALAT_MATCHED_TXNS].[dbo].[ETISALAT_TPF_TXNS_MASTER]";
            string WSelectionCriteria_2 = WSelectionCriteria; // Get it 
            string FileToBeCreated = labelWhatGrid.Text; 
            int DB_Mode = 2;

            //ReadMaster_MOBILE_By_SelectionCriteriaAnd_CREATE_CSV(string InFileA, string InFileB_Matched, string InSelectionCriteria, int In_DB_Mode)
            Mmob.ReadMaster_MOBILE_By_SelectionCriteriaAnd_CREATE_CSV(FileA, FileB_Matched, WSelectionCriteria_2, FileToBeCreated, DB_Mode); 
            //(string InFileA, string InFileB_Matched, string InSelectionCriteria,string InFileToBeCreated ,int In_DB_Mode)
            // OutputFileNm = Mmob.ReadFromWorking_4_AndFill_CSV(WOperator, WSignedId, WRMCycle, Mmob.DataTableAllFields); 
           // string XMLDoc;

            //////XMLDoc = ToXml(Ac.ATMsDetailsDataTable, 0);
            ////string sFileName = "C:\\RRDM\\Working\\TEXT.XML";
            ////StreamWriter outputFile = new StreamWriter(@sFileName);

            ////DataSet dS = new DataSet();
            ////dS.DataSetName = "RecordSet";
            ////dS.Tables.Add(Mmob.TableFullFromMaster);
            //////StringWriter sw = new StringWriter();
            ////dS.WriteXml(outputFile, XmlWriteMode.IgnoreSchema);

            MessageBox.Show("A Tap delimeter File is created in RRDM working directory"+Environment.NewLine
                                     + labelWhatGrid.Text
                                        );
        }

        private void buttonSourceRecords_Click(object sender, EventArgs e)
        {

            if (WMatchingCateg == "ETI350" || WMatchingCateg == "QAH350")
            {
                Form78d_AllFiles_BDC_3_MOBILE_TOTALS NForm78d_AllFiles_BDC_3_MOBILE_TOTALS;

                NForm78d_AllFiles_BDC_3_MOBILE_TOTALS = new Form78d_AllFiles_BDC_3_MOBILE_TOTALS(WOperator, WSeqNo, WMatchingCateg, W_Application);

                NForm78d_AllFiles_BDC_3_MOBILE_TOTALS.ShowDialog();

            }
            else
            {
                int TempSeqNo; 
                if (WMode == 9 || WMode == 10)
                {
                    TempSeqNo = WWSeqNo;
                }
                else
                {
                    TempSeqNo = WSeqNo;
                }
                //    // No Dates 
                //    WSelectionCriteria = " WHERE SeqNo=" + WSeqNo; 
                //    string Description = ""; 
                //    bool IsFromMaster = false; 
                //    Form78d_SHOW_MOBILE NForm78d_SHOW_MOBILE;
                //    NForm78d_SHOW_MOBILE = new Form78d_SHOW_MOBILE(WSignedId, Description, WSelectionCriteria,
                //                                   NullPastDate, NullPastDate, FileB, IsFromMaster, W_Application);
                //    NForm78d_SHOW_MOBILE.ShowDialog();
                //}
                //else
                //{
                    Form78d_AllFiles_BDC_3_MOBILE NForm78d_AllFiles_BDC_3_MOBILE; // 

                    NForm78d_AllFiles_BDC_3_MOBILE = new Form78d_AllFiles_BDC_3_MOBILE(WOperator, WSignedId, TempSeqNo, WMatchingCateg, 1, WTableName, W_Application);

                    NForm78d_AllFiles_BDC_3_MOBILE.ShowDialog();
               // }
              
            }
           
        }
// EXPORT To EXCEL 
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string ExcelPath; 

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            if (WMode == 8)
            {
                // GL ENTRIES
                ExcelPath = "C:\\RRDM\\Working\\GL_ENTRIES FOR.."+W_Application + "_Cycle_" + WRMCycle + ".xls";

                string WorkingDir = "C:\\RRDM\\Working\\";
                
                XL.ExportToExcel(Mmob.DataTableSelectedFields, WorkingDir, ExcelPath);
            }
            else
            {
                ExcelPath = "C:\\RRDM\\Working\\Category.." + WMatchingCateg + "_Cycle_" + WRMCycle + ".xls";

                string WorkingDir = "C:\\RRDM\\Working\\";
                if (Mmob.DataTableAllFields.Rows.Count > 1000)
                {
                    MessageBox.Show("Selected Records number is created than 1000" + Environment.NewLine
                        + "Please Create a Text file than excel." + Environment.NewLine
                        + "The Text File can be imported easily to Excel "
                        );
                    return;
                }
                XL.ExportToExcel(Mmob.DataTableAllFields, WorkingDir, ExcelPath);
            }
            
            
            //MessageBox.Show("Excel Created as" + Environment.NewLine
            //    + ExcelPath
            //    );
        }
    }
}

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
    public partial class Form78d_MOBILE_2_Grids : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
        RRDMMatchingTxns_InGeneralTables_EGATE M_EGATE = new RRDMMatchingTxns_InGeneralTables_EGATE();

        // RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();
        // FIND CUTOFF CYCLE
        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        //string FileA ;
        //string FileB ;

        //bool TPF_Origin ;
               
        //string FileA_RRDM ;
        //string FileA_RRDM_MATCHED ;

        //DateTime WCut_Off_Date;
        //string WTableName;
        //string WTableName_MATCHED; 

        // int WSeqNo;

        string WOperator;
        string WSignedId;
        string WMatchingCateg;
        int WRMCycle;
        string WRRNumber; // AGENT NUMBER
        int WMode;
       
        string W_Application; 

        public Form78d_MOBILE_2_Grids(string InOpertor , string InSignedId, string InMatchingCateg, 
                                               int InRMCycle,string InRRNumber ,int InMode, string InApplication)
        {
            WOperator = InOpertor;
            WSignedId = InSignedId;
            WMatchingCateg = InMatchingCateg;
            WRMCycle = InRMCycle;
            WRRNumber = InRRNumber; 
            WMode = InMode;
            W_Application = InApplication;

            InitializeComponent();

            // THIS IS MADE FOR ETI375 

            labelWhatGrid.Text = "LISTING OF TXNS FOR AGENT.."+ WRRNumber + " AT_CYCLE_" + WRMCycle.ToString();
            
        }

        string WSelectionCriteria;

        private void Form78b_Load(object sender, EventArgs e)
        {
            if (WMatchingCateg == "EGA375")
            {
                
                M_EGATE.ReadTablebyCycleAndCategory_EGA375(WMatchingCateg, WRRNumber, WRMCycle, W_Application);
            }
            else
            {
                Mmob.ReadTablebyCycleAndCategory_ETI375(WMatchingCateg, WRRNumber, WRMCycle, W_Application);
            }


            // Grid 1 
            //     public DataTable TXNS_ForAgent_Salaries = new DataTable();
            //public DataTable TXNS_ForAgent_Payments = new DataTable();
            if (M_EGATE.TXNS_ForAgent_Salaries.Rows.Count > 0)
            {
                dataGridView1.DataSource = M_EGATE.TXNS_ForAgent_Salaries.DefaultView;
                ShowGrid_1();

                labelFILE_A.Text = "TXNS FILE.." + M_EGATE.FileA + "..For" + WRRNumber;

                textBoxTotalAmt_1.Text = M_EGATE.TotalAmount_1.ToString("#,##0.000");
                textBoxTXNS_1.Text = M_EGATE.TotalNumber_1.ToString("#,##0");
            }
            else
            {
                labelFILE_A.Text = "No Data To show";
            }
            

            // Grid 2 Orizontal 

            
            if (M_EGATE.TXNS_ForAgent_Payments.Rows.Count > 0)
            {
                dataGridView2.DataSource = M_EGATE.TXNS_ForAgent_Payments.DefaultView;

                ShowGrid_2();

                labelFile_B.Text = "TXNS FILE.." + M_EGATE.FileB  + "..FOR.."+ WRRNumber;

                textBoxTotalAmt_2.Text = M_EGATE.TotalAmount_2.ToString("#,##0.000");
                textBoxTXNS_2.Text = M_EGATE.TotalNumber_2.ToString("#,##0");
            }
            else
            {
                labelFile_B.Text = "No Data To show";
            }

            // Find Differences

            decimal WDiff = 0; 
            WDiff = M_EGATE.TotalAmount_1 - M_EGATE.TotalAmount_2; 
            textBoxDiff.Text = WDiff.ToString("#,##0.000");

            if (WDiff == 0)
            {
                labelCommentOnDiff.Hide() ;
            }
            if (WDiff > 0)
            {
                labelCommentOnDiff.Text = "Left Total Anount is greater";
            }
            if (WDiff < 0)
            {
                labelCommentOnDiff.Text = "Right Total Anount is greater";
            }


          
            // Grid 3  

            
            if (M_EGATE.Duplicate_Salaries.Rows.Count > 0)
            {
                dataGridView3.DataSource = M_EGATE.Duplicate_Salaries.DefaultView;
                ShowGrid_3();

               // labelFILE_A_U.Text = "FILE.." + Mmob.FileA + "..Unmatched Mask exist in A  ";

                textBoxTotalAmt_3.Text = M_EGATE.TotalAmount_3.ToString("#,##0.00");
                textBoxTXNS_3.Text = M_EGATE.TotalNumber_3.ToString("#,##0");
            }
            else
            {
                labelFILE_A_U.Text = "No Data To show";
            }


            return;

            // Grid 4 Orizontal 


            if (M_EGATE.MatchingCategByTransType_D.Rows.Count > 0)
            {
                dataGridView4.DataSource = M_EGATE.MatchingCategByTransType_D.DefaultView;
                ShowGrid_4();

                labelFile_B_U.Text = "FILE.." + M_EGATE.FileB + "..Unmatched exist in B ";

                textBoxTotalAmt_4.Text = M_EGATE.TotalAmount_4.ToString("#,##0.00");
                textBoxTXNS_4.Text = M_EGATE.TotalNumber_4.ToString("#,##0");
            }
            else
            {
                labelFile_B_U.Text = "No Data To show";
            }

        }

        // On ROW ENTER 
        int WSeqNo;
        string TransType_1;
        string TransType_2;
        string TransType_3;
        string TransType_4;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //TransType_1 = (string)dataGridView1.Rows[e.RowIndex].Cells["TransType"].Value;


            //textBoxTransType_1.Text = TransType_1; 

        }
        // ROW ENTER FOR 2
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            //TransType_2 = (string)dataGridView2.Rows[e.RowIndex].Cells["TransType"].Value;

            //textBoxTransType_2.Text = TransType_2;
        }

        // ROW ENTER 3
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];
            //TransType_3 = (string)dataGridView3.Rows[e.RowIndex].Cells["TransType"].Value;

            //textBoxTransType_3.Text = TransType_3;
        }
        // ROW ENTER 4
        private void dataGridView4_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView4.Rows[e.RowIndex];
            TransType_4 = (string)dataGridView4.Rows[e.RowIndex].Cells["TransType"].Value;

            textBoxTransType_4.Text = TransType_4;
        }

        public void ShowGrid_1()
        {
            
           // DataGridViewCellStyle style = new DataGridViewCellStyle();
           // style.Format = "N2";

           // dataGridView1.Columns[0].Width = 220; // TransType
           // dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

           // dataGridView1.Columns[1].Width = 140; // TOTAL Amount
           // dataGridView1.Columns[1].DefaultCellStyle = style;
           // dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
           // dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
           //// dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Red;
           // //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

           // dataGridView1.Columns[2].Width = 80; // TXNS       
           // dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
           // dataGridView1.Columns[2].Visible = true;
        }

        public void ShowGrid_2()
        {

            //DataGridViewCellStyle style = new DataGridViewCellStyle();
            //style.Format = "N2";

            //dataGridView2.Columns[0].Width = 220; // TransType
            //dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView2.Columns[1].Width = 140; // TOTAL Amount
            //dataGridView2.Columns[1].DefaultCellStyle = style;
            //dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //// dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Red;
            ////dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            //dataGridView2.Columns[2].Width = 80; // TXNS       
            //dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[2].Visible = true;
        }


        public void ShowGrid_3()
        {

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView3.Columns[0].Width = 220; // TransType
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[1].Width = 140; // TOTAL Amount
            dataGridView3.Columns[1].DefaultCellStyle = style;
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView3.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            // dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Red;
            //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView3.Columns[2].Width = 80; // TXNS       
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView3.Columns[2].Visible = true;
        }

        public void ShowGrid_4()
        {

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView4.Columns[0].Width = 220; // TransType
            dataGridView4.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView4.Columns[1].Width = 140; // TOTAL Amount
            dataGridView4.Columns[1].DefaultCellStyle = style;
            dataGridView4.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView4.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            // dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Red;
            //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView4.Columns[2].Width = 80; // TXNS       
            dataGridView4.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView4.Columns[2].Visible = true;
        }


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
            // SHOW THE MATCHED Per Category
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 6; // Per Category from Master - MATCHED 1000
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchingCateg, TransType_1, WRMCycle, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();


            //if (WMatchingCateg == "ETI350" || WMatchingCateg == "QAH350")
            //{
            //    Form78d_AllFiles_BDC_3_MOBILE_TOTALS NForm78d_AllFiles_BDC_3_MOBILE_TOTALS;

            //    NForm78d_AllFiles_BDC_3_MOBILE_TOTALS = new Form78d_AllFiles_BDC_3_MOBILE_TOTALS(WOperator, WSeqNo, WMatchingCateg, W_Application);

            //    NForm78d_AllFiles_BDC_3_MOBILE_TOTALS.ShowDialog();

            //}
            //else
            //{
            //    Form78d_AllFiles_BDC_3_MOBILE NForm78d_AllFiles_BDC_3_MOBILE; // 

            //    NForm78d_AllFiles_BDC_3_MOBILE = new Form78d_AllFiles_BDC_3_MOBILE(WOperator, WSignedId, WSeqNo, WMatchingCateg, 1, WTableName, W_Application);

            //    NForm78d_AllFiles_BDC_3_MOBILE.ShowDialog();
            //}
           
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
            
         
        }
// SHOW UNMATCHED MASTER 10 for Category 
        private void buttonSource_3_Click(object sender, EventArgs e)
        {
            // SHOW UNMATCHED MASTER 10 for Category and TransType  
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 4; // Per Category from Master Mask 10
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchingCateg, TransType_3, WRMCycle, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }
        // Show Matched Second File Per Category and Transaction Type
        private void buttonSource_2_Click(object sender, EventArgs e)
        {
            // Show Matched Second File Per Category and Transaction Type
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 9; // Show Matched Second File Per Category and Transaction Type
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchingCateg, TransType_2, WRMCycle, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }
        // // Show UNMatched Second File Per Category and Transaction Type
        private void buttonSource_4_Click(object sender, EventArgs e)
        {
            // Show Matched Second File Per Category and Transaction Type
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 10; // Show Matched Second File Per Category and Transaction Type
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchingCateg, TransType_4, WRMCycle, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }
    }
}

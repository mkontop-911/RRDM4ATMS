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
    public partial class Form78d_SHOW_MOBILE : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();

        RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();
        // FIND CUTOFF CYCLE
        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime WCut_Off_Date;
        string WTableName;
        string WTableName_MATCHED;

        string MatchiingCateg;
        string RRNumber; 

        string WSignedId;
        string WDescription; 
        string WSelectionCriteria;
        DateTime WFromDt;
        DateTime WToDt;
        string WFileId;
        bool W_IsFromMaster;
        string W_Application; 

        public Form78d_SHOW_MOBILE(string InSignedId, string InDescription , string InSelectionCriteria, 
                                               DateTime InFromDt, DateTime InToDt,string InFileId,bool In_IsFromMaster,string InApplication)
        {
            
            WSignedId = InSignedId;
            WDescription = InDescription; 

            WSelectionCriteria = InSelectionCriteria;

            WFromDt = InFromDt;
            WToDt = InToDt;

            WFileId = InFileId;

            W_IsFromMaster = In_IsFromMaster; 

            W_Application = InApplication;

            InitializeComponent();

            RRDM_MOBILE_TABLE_NAMES Mn = new RRDM_MOBILE_TABLE_NAMES(); 

            Mn.GetFileNames(InApplication);

            WTableName = Mn.WTableName_Master_Primary;
            WTableName_MATCHED = Mn.WTableName_Master_Matched; 

            
        }
        //string WSelectionCriteria;
        private void Form78b_Load(object sender, EventArgs e)
        {
        //      ,[SET_DATE]
        //FROM[ETISALAT].[dbo].[ETISALAT_TPF_TXNS_MASTER]

                int DB_Mode = 2;
                WTableName = W_Application + ".[dbo]." + WFileId;
            // FROM[ETISALAT_MATCHED_TXNS].[dbo].[ETISALAT_TPF_TXNS_MASTER]
                WTableName_MATCHED = W_Application + "_MATCHED_TXNS.[dbo]." + WFileId;

                Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                               WFromDt, WToDt, DB_Mode,W_Application); 
               
                dataGridView1.DataSource = Mmob.DataTableAllFields.DefaultView;

                labelWhatGrid.Text = WDescription;
                dataGridView1.Show();

                textBoxTotalRec.Text = Mmob.DataTableAllFields.Rows.Count.ToString();

          
            // SHOW GRID

        }

        // On ROW ENTER 
        int WSeqNo;
        string  WMatchingCateg ;
        string WOperator;
        long LongSeqNumber;
        int  WRMCycleNo; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

            textBoxSeqNo.Text = WSeqNo.ToString();
            // READ RECORD througn SeqNo
            string WWSelectionCriteria = " WHERE SeqNo=" + WSeqNo;
            //WTableName 
            Mmob.ReadTransSpecificFromSpecificTable_MOBILE_2(WWSelectionCriteria, WTableName, WTableName_MATCHED, 2, W_Application); 

            if (Mmob.RecordFound == true)
            {
                WMatchingCateg = Mmob.MatchingCateg;
                RRNumber = Mmob.RRNumber;
                WRMCycleNo = Mmob.LoadedAtRMCycle;

                LongSeqNumber = Mmob.OriginalRecordId;

            }

            // NOTES START  
            string Order = "Descending";
            string WParameter4 = "SeqNo:" + WSeqNo;
            string WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";



        }
        // ROW ENTER FOR 2
        string WAtmNo; 
        

// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
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
            if (W_IsFromMaster == false )
            {
                Form78d_SHOW_MOBILE_RAW NForm78d_SHOW_MOBILE_RAW;

                NForm78d_SHOW_MOBILE_RAW = new Form78d_SHOW_MOBILE_RAW(WSignedId, LongSeqNumber, WFileId, W_Application);

                NForm78d_SHOW_MOBILE_RAW.ShowDialog();
                return; 
            }


            if (WMatchingCateg == "ETI350" || WMatchingCateg == "QAH350")
            {
                Form78d_AllFiles_BDC_3_MOBILE_TOTALS NForm78d_AllFiles_BDC_3_MOBILE_TOTALS;

                NForm78d_AllFiles_BDC_3_MOBILE_TOTALS = new Form78d_AllFiles_BDC_3_MOBILE_TOTALS(WOperator, WSeqNo, WMatchingCateg, W_Application);

                NForm78d_AllFiles_BDC_3_MOBILE_TOTALS.ShowDialog();

            }
            else
            {
                Form78d_AllFiles_BDC_3_MOBILE NForm78d_AllFiles_BDC_3_MOBILE; // 

                NForm78d_AllFiles_BDC_3_MOBILE = new Form78d_AllFiles_BDC_3_MOBILE(WOperator, WSignedId, WSeqNo, WMatchingCateg, 1, WTableName, W_Application);

                NForm78d_AllFiles_BDC_3_MOBILE.ShowDialog();
            }
           
        }
// EXPORT To EXCEL 
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string ExcelPath; 

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            //if (WMode == 8)
            //{
            //    // GL ENTRIES
            //    ExcelPath = "C:\\RRDM\\Working\\GL_ENTRIES FOR.."+W_Application + "_Cycle_" + WRMCycle + ".xls";

            //    string WorkingDir = "C:\\RRDM\\Working\\";
                
            //    XL.ExportToExcel(Mmob.DataTableSelectedFields, WorkingDir, ExcelPath);
            //}
            //else
            //{
                ExcelPath = "C:\\RRDM\\Working\\.." + WDescription + ".xls";

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
            //}
            
            
            //MessageBox.Show("Excel Created as" + Environment.NewLine
            //    + ExcelPath
            //    );
        }
// ANALYSIS
        private void linkLabelAnalysis_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WMatchingCateg != "ETI375")
            {
                MessageBox.Show("This is for category ETI375");
                return; 
            }
            WMatchingCateg = Mmob.MatchingCateg;
            RRNumber = Mmob.RRNumber;
            Form78d_MOBILE_2_Grids NForm78d_MOBILE_2_Grids;
            int WMode = 9; // SHOW ETI375
            NForm78d_MOBILE_2_Grids = new Form78d_MOBILE_2_Grids(WOperator, WSignedId, WMatchingCateg, WRMCycleNo, RRNumber, WMode, W_Application);
            NForm78d_MOBILE_2_Grids.ShowDialog();
        }
        // Notes
        
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "SeqNo:" + Mmob.SeqNo;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            string WMode = "Update";
            int WSignRecordNo = 7; 
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            string Order = "Descending";
            WParameter4 = "SeqNo:" + WSeqNo;
            string WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            //SetScreen();
        }

    }
}

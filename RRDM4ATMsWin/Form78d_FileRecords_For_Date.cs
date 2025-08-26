using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_FileRecords_For_Date : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        
       // RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 
       // int SavedMode;
      //  bool InternalUse; 

        int WSeqNo;

        string WOperator;
        string WSignedId; 
        string WTableId;
       
        string WMatchingCateg;
        DateTime WDate; 
        int WMode;
        
        public Form78d_FileRecords_For_Date(string InOperator, string InSignedId, string InTableId, string InMatchingCateg, DateTime InDate, int InMode)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WTableId = InTableId; // eg InTable == "Atms_Journals_Txns"
           
            WMatchingCateg = InMatchingCateg;

            WDate = InDate; 

            WMode = InMode; // 11: Non Processed for a date for a category 
                            
           // SavedMode = WMode; 
           
            
            InitializeComponent();
           
        }
        int W_DB_Mode; 
        private void Form78b_Load(object sender, EventArgs e)
        {
            labelWhatGrid.Text = "NON PROCESSED RECORDS FOR CAT.." + WMatchingCateg + "..AND Date.." + WDate.ToShortDateString();
            //W_DB_Mode = 1;
            Mgt.ReadTableAndFillTableForA_Date(WSignedId, WTableId, WMatchingCateg, WDate); 


            dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;

            int rows = Mgt.DataTableAllFields.Rows.Count;
            textBoxRecords.Text = rows.ToString();
            // SHOW GRID

        }

        // On ROW ENTER 
       
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //if (WMode==9 || WMode == 10)
            //{
            //    // Do nothing
            //}
            //else
            //{
            //    WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
            //    string SelectionCriteria = " WHERE SEQNO =" + WSeqNo;
            //    if (WTableId == "Atms_Journals_Txns")
            //    {
            //        Mpa.ReadInPoolTransSpecificBySelectionCriteria(SelectionCriteria, 2);
            //        textBoxTerm.Text = Mpa.TerminalId;
            //    }
            //    else
            //    {
            //        Mgt.ReadTransSpecificFromSpecificTable_Order_By_Date(SelectionCriteria, WTableId, 2);
            //        textBoxTerm.Text = Mgt.TerminalId;
            //    }

            //}

        }

     

// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print
            string P1 = "NON PROCESSED RECORDS FOR CAT.." + WMatchingCateg + "..AND Date.." + WDate.ToShortDateString();



            string P2 = "";  // Category 
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

            
            if (WTableId == "Atms_Journals_Txns")
            {
                Form56R68ATMS_W_Pool_Table ReportATMS_Pool_Table = new Form56R68ATMS_W_Pool_Table(P1, P2, P3, P4, P5);
                ReportATMS_Pool_Table.Show();
            }
            else
            {
                Form56R68ATMS_W_General_Files ReportATMS_General_Files = new Form56R68ATMS_W_General_Files(P1, P2, P3, P4, P5);
                ReportATMS_General_Files.Show();
            }

           
        }


// Export To excel 
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();
            
            string ExcelPath = "C:\\RRDM\\Working\\"+"Aging for a date" +"_"+ WDate + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Mgt.DataTableAllFields,WorkingDir ,ExcelPath);
        }
    }
}

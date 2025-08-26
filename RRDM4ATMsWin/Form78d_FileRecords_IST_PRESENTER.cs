using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_FileRecords_IST_PRESENTER : Form
    {
       

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
    
        int WSeqNo;

        string WOperator;
        string WSignedId; 
        string WTableId;
       
        int WRMCycle;
        DateTime WCutOffDate;
        int WMode;
        
        public Form78d_FileRecords_IST_PRESENTER(string InOperator, string InSignedId, string InTableId,int InRMCycle , DateTime InCutOffDate, int InMode)
             
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WTableId = InTableId; // eg InTable == "IST"
            WRMCycle = InRMCycle;
            WCutOffDate = InCutOffDate;

            WMode = InMode; // 1: 
                            // 2: 
                            // 3: 
            InitializeComponent();
           
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
           
           labelWhatGrid.Text = "Presenter Errors for RMCycle :.." + WRMCycle.ToString() + "..Cut OFF.." + WCutOffDate;

            Mgt.ReadTableAndFillTableWithPresenter(WTableId, WRMCycle, WSignedId,2); 

            dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;

            int rows = Mgt.DataTableAllFields.Rows.Count;
            textBoxRecords.Text = rows.ToString();
            if (rows == 0)
            {
                MessageBox.Show("No records to show");
                this.Dispose(); 
                return; 
            }
            // SHOW GRID

        }

        // On ROW ENTER 
       
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
            string SelectionCriteria = " WHERE SEQNO ="+ WSeqNo; 
         
                Mgt.ReadTransSpecificFromSpecificTable_Order_By_Date(SelectionCriteria, Mgt.LogicalFiledID,2);
                textBoxTerm.Text = Mgt.TerminalId;
         
           
        }

        // Show Grid 03 
        public void ShowGrid03()
        {

            //dataGridView1.Columns[0].Width = 70; // WhatFile
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[1].Width = 60; // SeqNo
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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


            P1 = "Presenter Errors for RMCycle :.." + WRMCycle.ToString() + "..Cut OFF.." + WCutOffDate;

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
// Show Terminal
        private void buttonShowTerm_Click(object sender, EventArgs e)
        {
           // InternalUse = true; 
          //  WMode = 3;
         //   WAtmNo = textBoxTerm.Text;
         //   Form78b_Load(this, new EventArgs());
        }
// Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {

           // WAtmNo = ""; 
           // WAtmNo = textBoxTerm.Text;
           // Form78b_Load(this, new EventArgs());
        }
// Show Source records
        private void buttonSourceRecords_Click(object sender, EventArgs e)
        {
            Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

            //switch (WOperator)
            //{
            //    case "BCAIEGCX":
            //        {
            //            NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, Mpa.UniqueRecordId);

            //            NForm78d_AllFiles_BDC_3.ShowDialog();
            //            break;
            //        }
            //}
        }
    }
}

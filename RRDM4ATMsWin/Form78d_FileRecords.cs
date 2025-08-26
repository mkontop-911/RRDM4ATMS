using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_FileRecords : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        
       // RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMGasParameters Gp = new RRDMGasParameters();

        // int SavedMode;
        //  bool InternalUse; 

        int WSeqNo;
        string WParamId = ""; 

        string WOperator;
        string WSignedId; 
        string WTableId;
        string WAtmNo; 
        int WRMCycle;
        string WMatchingCateg; 
        int WMode;
        bool WCategoryOnly; 

        public Form78d_FileRecords(string InOperator, string InSignedId, string InTableId, string InAtmNo, int InRMCycle,string InMatchingCateg, int InMode, bool InCategoryOnly)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WTableId = InTableId; // eg InTable == "Atms_Journals_Txns"
            WAtmNo = InAtmNo; 
            WRMCycle = InRMCycle;
            WMatchingCateg = InMatchingCateg; 

            WMode = InMode; // 1: Non Processed 
                            // 2: Processed 
                            // 3: per ATM
                            // 4: per ATM for Replenishement Cycle
                            // 5: Unmatched without actions from Maker  
                            // 9: Read BULK  
                            // 10: records within the RM Cycle 
                            // 11: Non Processed for a day  
                            // 12: Parameters History by Parameter Id = WAtmNo
            // SavedMode = WMode; 
            WCategoryOnly = InCategoryOnly;
            
            if (WMode == 12)
            {
                WParamId = WAtmNo; 
            }

            InitializeComponent();
           
        }
        int W_DB_Mode; 
        private void Form78b_Load(object sender, EventArgs e)
        {
            if (WCategoryOnly == true)
            {
                if (WMode == 1)
                {
                    labelWhatGrid.Text = "NON PROCESSED RECORDS FOR FILE.." + WTableId + "..AND CATEGORY.." + WMatchingCateg;
                    W_DB_Mode = 1; 
                }
                 
                if (WMode == 2)
                {
                    labelWhatGrid.Text = "PROCESSED RECORDS FOR FILE.." + WTableId + "..At Cycle.." + WRMCycle.ToString() + "..AND CATEGORY.." + WMatchingCateg;
                    W_DB_Mode = 2; 
                }
            }
            else
            {
                if (WMode == 1)
                {
                    labelWhatGrid.Text = "NON PROCESSED RECORDS FOR FILE.." + WTableId;
                    W_DB_Mode = 1;
                }
                if (WMode == 2)
                {
                    labelWhatGrid.Text = "PROCESSED RECORDS FOR FILE.." + WTableId + "..At Cycle.." + WRMCycle.ToString();
                    W_DB_Mode = 2;
                }
            }

            if (WMode == 3)
            {
                labelWhatGrid.Text = "Discrepancies for ATM " + "..At Cycle.." + WRMCycle.ToString();
                W_DB_Mode = 1;
            }
               
            if (WMode == 5)
            {
                labelWhatGrid.Text = "Discrepancies without Action Yet from Maker" ;
                W_DB_Mode = 1;
            }

            if (WMode == 9)
            {
                labelWhatGrid.Text = "Input Records for file..."+ WTableId;
                buttonPrint.Hide(); 
                W_DB_Mode = 1;
            }
            if (WMode == 10)
            {
                labelWhatGrid.Text = "All Records for file..." + WTableId +".. And Cycle.."+WRMCycle.ToString();
                buttonPrint.Hide();
                W_DB_Mode = 1;
            }

            if (WMode == 12)
            {
                labelWhatGrid.Text = "All History Records for ParamId..." + WParamId;
                buttonPrint.Hide();
                
                labelTerminal.Hide();
                textBoxTerm.Hide();

                buttonShowTerm.Hide();

                buttonRefresh.Hide(); 
                // W_DB_Mode = 1;
            }


            if (WMode < 12)
            {
                Mgt.ReadTableAndFillTable(WSignedId, WTableId, WAtmNo, WRMCycle, WMatchingCateg, WMode, WCategoryOnly, W_DB_Mode);

                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;

                int rows = Mgt.DataTableAllFields.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
            else
            {
                // WMode >= 12
                
               // string WString = WAtmNo;
                Gp.ReadParametersAndFillDataTable_History(WOperator, WSignedId, WParamId, WMode);
                dataGridView1.DataSource = Gp.DataTableAllParameters.DefaultView;
                int rows = Gp.DataTableAllParameters.Rows.Count;
                textBoxRecords.Text = rows.ToString();
            }
          
            // SHOW GRID

        }

        // On ROW ENTER 
       
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            if (WMode==9 || WMode == 10)
            {
                // Do nothing
            }
            else
            {
                if (WMode == 12 )
                {
                    WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["RefKey"].Value;
                    
                }
                else
                {
                    WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
                    string SelectionCriteria = " WHERE SEQNO =" + WSeqNo;
                    if (WTableId == "Atms_Journals_Txns")
                    {
                        Mpa.ReadInPoolTransSpecificBySelectionCriteria(SelectionCriteria, 2);
                        textBoxTerm.Text = Mpa.TerminalId;
                    }
                    else
                    {
                        Mgt.ReadTransSpecificFromSpecificTable_Order_By_Date(SelectionCriteria, WTableId, 2);
                        textBoxTerm.Text = Mgt.TerminalId;
                    }
                }         

            }

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

            if (WMode == 1)
            {
                P1 = "Non Processed Records of File..." + WTableId + "..At Cycle.." + WRMCycle.ToString();
            }

            if (WMode == 2)
            {
                P1 = "Processed Records of File..." + WTableId + "..At Cycle.." + WRMCycle.ToString();
            }

            if (WMode == 3)
            {
                P1 = "Discrepancies for AtmNo.." + WAtmNo + "..At Cycle.." + WRMCycle.ToString();
            }
            //if (WMode == 3 & InternalUse == true)
            //{
            //    P1 = "Records for AtmNo.." + WAtmNo + "..At Cycle.." + WRMCycle.ToString();
            //}

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
            WAtmNo = textBoxTerm.Text;
            Form78b_Load(this, new EventArgs());
        }
// Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {

            WAtmNo = ""; 
           // WAtmNo = textBoxTerm.Text;
            Form78b_Load(this, new EventArgs());
        }
// Export To excel 
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();
            
           
            //Gp.DataTableAllParameters
            if (WMode==12)
            {
                string ExcelPath = "C:\\RRDM\\Working\\" + "HST_Parameter ID_" + WParamId + "_" + DateTime.Now.Minute + ".xlsx";
                string WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(Gp.DataTableAllParameters, WorkingDir, ExcelPath);

            }
            else
            {
                string ExcelPath = "C:\\RRDM\\Working\\" + "Records for _" + WRMCycle.ToString() + "_" + DateTime.Now.Minute + ".xlsx";
                string WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(Mgt.DataTableAllFields, WorkingDir, ExcelPath);

            }
        }
    }
}

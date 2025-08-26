using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_BasedOnDataTable : Form
    {

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMRepl_SupervisorMode_Details Sm = new RRDMRepl_SupervisorMode_Details();
        // int SavedMode;
        //  bool InternalUse; 
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public DataTable WTableTXNS = new DataTable();

        int WSeqNo;

        string WOperator;
        string WSignedId;
        string WTableId;
        string WAtmNo;

        DateTime WDtFrom;
        DateTime WDtTo;
        int WSesNo;
        DateTime WCap_Date;
        int WMode;

        public Form78d_BasedOnDataTable(string InOperator, string InSignedId, string InTableId
                                          , DataTable InDataTable
                                                        , int InMode)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WTableId = InTableId; // eg InTable == "Atms_Journals_Txns" OR IST 
            WTableTXNS = InDataTable;

            WMode = InMode;
            // 1: Dublicates 
            // 2: Gaps Not Arrive yet 

            InitializeComponent();

            if (WMode == 1 || WMode ==3)
            {
                labelWhatGrid.Text = "Dublicate RECORDS FOR.." + InTableId;
            }
            if (WMode == 2)
            {
                labelWhatGrid.Text = "Records For Journals Gaps not arrived yet..";
            }

            textBoxRecords.Text = WTableTXNS.Rows.Count.ToString();

        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = WTableTXNS.DefaultView;
        }

        // On ROW ENTER 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            if (WMode == 1 || WMode == 2)
            {
                WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

                if (WMode == 1)
                {
                    Mgt.ReadTransSpecificFromSpecificTable_By_SeqNo(WTableId, WSeqNo, 1);
                }
                if (WMode == 2)
                {
                    string WSelection = "WHERE SeqNo =" + WSeqNo;
                    Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelection, 1);
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

       
        //
        // EXPORT TO EXCEL 
        //
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString();
            string DD = DateTime.Now.Day.ToString();
            string Min = DateTime.Now.Minute.ToString();

            //MessageBox.Show("Excel will be created in RRDM Working Directory");
            string Id = WAtmNo + "_" + WSesNo.ToString();

            string ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(WTableTXNS, WorkingDir, ExcelPath);
        }
      // FINISH
        private void buttonFinish_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

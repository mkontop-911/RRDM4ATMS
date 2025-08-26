using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_BDC_TO_POST : Form
    {
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //int WSeqNo;

        string WOperator;
        string WSignedId;

        int WRMCycle;

        int WMode;

        bool WT24Version; 

        DateTime WDate;

        string InMode_Name;

        public Form78d_BDC_TO_POST(string InOperator, string InSignedId
                                                   , int InRMCycle, DateTime InDate, int InMode, bool InT24Version)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WRMCycle = InRMCycle;

            WDate = InDate;

            WMode = InMode;

            WT24Version = InT24Version; 


            // 1: Gives all txns for BDC_GL
            // 2: Gives All txns for BDC_GIFU
            // 3: Gives All txns for BDC_Settlement_Dpt 

            bool IsSelectByDate = false;

            if (InDate != NullPastDate)
            {
                // Select By Date
                IsSelectByDate = true;
            }
            else
            {
                // Select By Repl 
                IsSelectByDate = false;
            }

            InitializeComponent();

            if (WMode == 1)
            {
                if (IsSelectByDate == true)
                {
                    labelWhatGrid.Text = "RECORDS FOR BANK_GL FOR DATE:." + WDate.ToShortDateString();
                }
                else
                {
                    labelWhatGrid.Text = "RECORDS FOR BANK_GL FOR THE RM CYCLE NO:." + WRMCycle.ToString();
                }

                InMode_Name = "BANK_GL";
            }
            if (WMode == 2)
            {
                if (IsSelectByDate == true)
                {
                    labelWhatGrid.Text = "RECORDS FOR BANK_GIFU FOR DATE:." + WDate.ToShortDateString();
                }
                else
                {
                    labelWhatGrid.Text = "RECORDS FOR BANK_GIFU FOR THE RM CYCLE NO:." + WRMCycle.ToString();
                }

                InMode_Name = "BANK_GIFU";
            }
            if (WMode == 3)
            {
                if (IsSelectByDate == true)
                {
                    labelWhatGrid.Text = "RECORDS FOR BANK_Settlement_Dpt FOR DATE:." + WDate.ToShortDateString();
                }
                else
                {
                    labelWhatGrid.Text = "RECORDS FOR BANK_Settlement_Dpt  FOR THE RM CYCLE NO:." + WRMCycle.ToString();
                }

                InMode_Name = "BANK_Settlement_Dpt";
            }
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            // Mgt.ReadTableAndFillTable(WTableId, WAtmNo, WRMCycle, WMatchingCateg, WMode, WCategoryOnly);
            if (WMode == 1)
            {
                Tc.ReadToBePostedAndFillTable_BDC_GL_NEW(WSignedId, WRMCycle, WDate, WMode);

                if (Tc.BDC_GL.Rows.Count == 0)
                {
                    MessageBox.Show("No transactions to show");
                    this.Dispose();
                }

                dataGridView1.DataSource = Tc.BDC_GL.DefaultView;
                int rows = Tc.BDC_GL.Rows.Count;
                textBoxRecords.Text = rows.ToString();
                textBoxTotalDebit.Text = Tc.TotalDrAmt.ToString("#,##0.00");
                textBoxTotalCredit.Text = Tc.TotalCrAmt.ToString("#,##0.00");

            }
            if (WMode == 2)
            {
                Tc.ReadToBePostedAndFillTable_BDC_GIFU(WSignedId, WRMCycle, WDate, WMode);

                if (Tc.BDC_GIFU.Rows.Count == 0)
                {
                    MessageBox.Show("No transactions to show");
                    this.Dispose();
                }


                dataGridView1.DataSource = Tc.BDC_GIFU.DefaultView;
                int rows = Tc.BDC_GIFU.Rows.Count;
                textBoxRecords.Text = rows.ToString();
                textBoxTotalDebit.Text = Tc.TotalDrAmt.ToString("#,##0.00");
                textBoxTotalCredit.Text = Tc.TotalCrAmt.ToString("#,##0.00");
            }
            if (WMode == 3)
            {
                // Find Settlement Account Number
                string SetlAccNo;
                RRDMAccountsClass Acc = new RRDMAccountsClass(); 
                Acc.ReadAccountsBasedOn_ShortAccID(WOperator, "53");
                
                if (Acc.RecordFound == true)
                {
                    SetlAccNo = Acc.AccNo; // The T24 is a different one  
                }
                else
                {
                    MessageBox.Show("Settlement Account Not Found");
                    return;
                }
                

                Tc.ReadToBePostedAndFillTable_BDC_Settlement_Dpt(WSignedId, WRMCycle, WDate, WMode, SetlAccNo);

                if (Tc.BDC_Settlement_Dpt.Rows.Count == 0)
                {
                    MessageBox.Show("No transactions to show");
                    this.Dispose();
                }

                dataGridView1.DataSource = Tc.BDC_Settlement_Dpt.DefaultView;
                int rows = Tc.BDC_Settlement_Dpt.Rows.Count;
                textBoxRecords.Text = rows.ToString();
                textBoxTotalDebit.Text = Tc.TotalDrAmt.ToString("#,##0.00");
                textBoxTotalCredit.Text = Tc.TotalCrAmt.ToString("#,##0.00");
            }


            // SHOW GRID

        }

        // On ROW ENTER 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {



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

        //
        // EXPORT TO EXCEL 
        //
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            //string YYYY = DateTime.Now.Year.ToString();
            //string MM = DateTime.Now.Month.ToString();
            //string DD = DateTime.Now.Day.ToString();
            //string Min = DateTime.Now.Minute.ToString();

            //MessageBox.Show("Excel will be created in RRDM Working Directory");
            //  string Id = InMode_Name+"_"+WRMCycle + "_" + YYYY+"_"+MM+ "_" + DD+Min;
            string Id = "";
            string ExcelPath = "";
            string WorkingDir = "C:\\RRDM\\Working\\";
            if (WMode == 1)
            {
                Id = "union batch_" + DateTime.Now.Date.ToString("yyyyMMdd");
                ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
                XL.ExportToExcel(Tc.BDC_GL, WorkingDir, ExcelPath);
            }
            if (WMode == 2)
            {
                Id = "GIFU_" + DateTime.Now.Date.ToString("yyyyMMdd");
                ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
                // GEFU_20210202
                XL.ExportToExcel(Tc.BDC_GIFU, WorkingDir, ExcelPath);
            }
            if (WMode == 3)
            {
                Id = "settlement report_" + DateTime.Now.Date.ToString("yyyyMMdd");
                ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
                XL.ExportToExcel(Tc.BDC_Settlement_Dpt, WorkingDir, ExcelPath);
            }
        }
        // Finish
        private void buttonFinish_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

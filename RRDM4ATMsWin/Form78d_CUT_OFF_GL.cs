using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_CUT_OFF_GL : Form
    {
       
        RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM Cgl = new RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM();

        string WSelectionCriteria;
        bool FirstTime; 

        int WSeqNo;

        int WMode_Origin; 

        string WOperator;
        string WSignedId;
        DateTime WCAP_DATE;
        string WMatchingCateg;
        string WAtmNo;
        int WRMCycle;
        int WMode; 
       
        public Form78d_CUT_OFF_GL(string InOpertor , string InSignedId, DateTime InCAP_DATE, 
            string InMatchingCateg, string InAtmNo ,int InRMCycle, int InMode)
        {
            WOperator = InOpertor;
            WSignedId = InSignedId;
            WCAP_DATE = InCAP_DATE;
            WMatchingCateg = InMatchingCateg;
            WAtmNo = InAtmNo; 
            WRMCycle = InRMCycle;
            WMode = InMode;

            // Mode = 6 => show Cut Off Summary All Categories for a particular Cut Off < than and created at RMCycle
            // Mode = 7 => show Cut Off Summary All  ATMs for a particular Cut Off 

            // Mode = 8 => show Cut Off List for one Category for all Cut Offs 
            // Mode = 9 => show Cut Off List for one ATM  for all cut off 

            InitializeComponent();

            WMode_Origin = WMode; 

            FirstTime = true; 

            //  

            comboBox1.Items.Add("Categories");

            comboBox1.Items.Add("ATMs");
            if (WMode == 6)
            comboBox1.Text = "Categories";

            FirstTime = false;
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
           
            if (WMode == 6)
            {
                WSelectionCriteria = " WHERE CAP_DATE <= @CAP_DATE AND UpdatedAtRMCycle = @UpdatedAtRMCycle "; 
                Cgl.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, WSelectionCriteria, WCAP_DATE, WRMCycle, WMode); 
                
                dataGridView1.DataSource = Cgl.Table_CAP_DATE.DefaultView;

                labelWhatGrid.Text = "LIST OF CATEGORIES STATUS FOR GL CREATED At RM and CUT_OFF: " + WCAP_DATE.ToShortDateString();
                dataGridView1.Show();
                
                labelAtmNo.Hide();
                textBoxAtmNo.Hide();
                buttonShow.Hide();

                labelCateg.Show();
                textBoxCateg.Show();
                buttonShowCateg.Show(); 

                textBoxTotalRec.Text = Cgl.Table_CAP_DATE.Rows.Count.ToString();

            }
            if (WMode == 7)
            {
                WSelectionCriteria = " WHERE CAP_DATE <= @CAP_DATE AND UpdatedAtRMCycle = @UpdatedAtRMCycle ";
                Cgl.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, WSelectionCriteria, WCAP_DATE, WRMCycle, WMode);

                dataGridView1.DataSource = Cgl.Table_CAP_DATE.DefaultView;

                labelWhatGrid.Text = "LIST OF ATMs STATUS FOR GL CUT_OFF: " + WCAP_DATE.ToShortDateString();
                dataGridView1.Show();

                labelAtmNo.Show();
                textBoxAtmNo.Show();
                buttonShow.Show();

                labelCateg.Hide();
                textBoxCateg.Hide();
                buttonShowCateg.Hide();

                textBoxTotalRec.Text = Cgl.Table_CAP_DATE.Rows.Count.ToString();
            }

            if (WMode == 8)
            {
                WSelectionCriteria = " WHERE MatchingCateg ='" + WWCategory +"' ORDER BY CAP_DATE";
                Cgl.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, WSelectionCriteria, WCAP_DATE, 0 ,WMode);

                dataGridView1.DataSource = Cgl.Table_CAP_DATE.DefaultView;

                labelWhatGrid.Text = "LIST OF CATEGORIES STATUS FOR GL CUT_OFF: " + WCAP_DATE.ToShortDateString();
                dataGridView1.Show();

                labelAtmNo.Hide();
                textBoxAtmNo.Hide();
                buttonShow.Hide();

                labelCateg.Show();
                textBoxCateg.Show();
                buttonShowCateg.Show();

                textBoxTotalRec.Text = Cgl.Table_CAP_DATE.Rows.Count.ToString();

            }

            if (WMode == 9)
            {
                WSelectionCriteria = " WHERE AtmNo ='" + WWAtmNo + "' ORDER BY CAP_DATE";
                Cgl.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, WSelectionCriteria, WCAP_DATE,0 ,WMode);

                dataGridView1.DataSource = Cgl.Table_CAP_DATE.DefaultView;

                labelWhatGrid.Text = "LIST OF AN ATM STATUS FOR ALL GL CUT_OFF: " + WCAP_DATE.ToShortDateString();
                dataGridView1.Show();

                labelAtmNo.Show();
                textBoxAtmNo.Show();
                buttonShow.Show();

                labelCateg.Hide();
                textBoxCateg.Hide();
                buttonShowCateg.Hide();

                textBoxTotalRec.Text = Cgl.Table_CAP_DATE.Rows.Count.ToString();
            }

            // SHOW GRID

        }

        // On ROW ENTER 
        string WWCategory;
        string WWAtmNo; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
            
            if (WMode==6 || WMode == 8)
            {
                Cgl.Read_CAP_DATEForSeqNo(WSeqNo, 1);
            }
            if (WMode == 7 || WMode == 9)
            {
                Cgl.Read_CAP_DATEForSeqNo(WSeqNo, 2);
            }
            textBoxCateg.Text= WWCategory = Cgl.MatchingCateg;
            textBoxAtmNo.Text = WWAtmNo = Cgl.AtmNo; 

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
// Show Discrepancies for ATM No
        private void buttonShow_Click(object sender, EventArgs e)
        {
            WMode = 9;
            Form78b_Load(this, new EventArgs());
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

        private void labelAtmNo_Click(object sender, EventArgs e)
        {

        }
// Show Category 
        private void buttonShowCateg_Click(object sender, EventArgs e)
        {
            WMode = 8 ; 
            Form78b_Load(this, new EventArgs());
        }
        // 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FirstTime == true) return;  
            if (comboBox1.Text == "Categories")
            {
                WMode = 6;
                Form78b_Load(this, new EventArgs());
            }
            if (comboBox1.Text == "ATMs")
            {
                WMode = 7;
                Form78b_Load(this, new EventArgs());
            }
        }

        private void buttonExtractToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelDATE = WCAP_DATE.Year.ToString()
                       + WCAP_DATE.Month.ToString()
                       + WCAP_DATE.Day.ToString()
                       + "_"
                       + DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString()
                        ;

           
                string ExcelPath = "C:\\RRDM\\Working\\GL_CUT_OFF_" + ExcelDATE + ".xls";
                string WorkingDir = "C:\\RRDM\\Working\\";
                XL.ExportToExcel(Cgl.Table_CAP_DATE, WorkingDir, ExcelPath);   

        }
    }
}

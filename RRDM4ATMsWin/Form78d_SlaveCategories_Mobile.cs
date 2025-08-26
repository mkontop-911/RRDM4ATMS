using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_SlaveCategories_Mobile : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        //RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        int WSeqNo;

        string WOperator;
        string WSignedId;
        string W_Application; 
        string WRMCateg;
        int WRMCycle;
        int WMode; 
       
        public Form78d_SlaveCategories_Mobile(string InOperator , string InSignedId,string In_Application ,string InRMCateg, int InRMCycle, int InMode)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            W_Application = In_Application; 
            WRMCateg = InRMCateg;
            WRMCycle = InRMCycle;
            WMode = InMode;

            // Mode = 4 => show All Categories not only slave
            // Mode = 5 => show  only slave categories

            InitializeComponent();
            if (WMode == 5)
            {
                labelWhatGrid.Text = "LIST OF CATEGORIES FOR THIS GROUP :.." + WRMCateg;
            }
            if (WMode == 4)
            {
                labelWhatGrid.Text = "LIST OF CATEGORIES " ;
            } 

        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            

            if (WMode == 5)
            {
                Mc.ReadMatchingCategoriesSlavesAndFillTable(WOperator, WRMCateg);
            }
            if (WMode == 4)
            {
                Mc.ReadMatchingCategoriesAndFillTableInDetail_Mobile(WOperator, W_Application);
            }

            dataGridView1.DataSource = Mc.TableMatchingCateg.DefaultView;

            // SHOW GRID
            ShowGrid03(); 

        }

        // On ROW ENTER 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

        }
       

        // Show Grid 03 
        public void ShowGrid03()
        {
          
            dataGridView1.Columns[0].Width = 70; //SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].Visible = false; 

            dataGridView1.Columns[1].Width = 70; //Select
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 90; // CategoryId
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 450; // Category-Name
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 70; // "Is POS_Type"
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 70; // "Days W"
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 70; // "Days C"
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 70; // "Days C"
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 160; // File_A
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[9].Width = 160; // File_B
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 160; // File_C
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //MathingDone
            dataGridView1.Columns[11].Width = 80; // MathingDone
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].Visible = false;

            dataGridView1.Columns[12].Width = 200; // Assign-to
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }



        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// EXCEL 
        private void buttonExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            if (Mc.TableMatchingCateg.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to Export to excel");
                return;
            }

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelDATE = DateTime.Now.Year.ToString()
                       + DateTime.Now.Month.ToString()
                       + DateTime.Now.Day.ToString()
                       + "_"
                       + DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString()
                        ;

            string ExcelPath;
            string WorkingDir;

            ExcelPath = "C:\\RRDM\\Working\\Categories" + ExcelDATE + ".xls";
            WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Mc.TableMatchingCateg, ExcelPath);
        }
    }
}

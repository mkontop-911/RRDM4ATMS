using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_Matched : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        
        RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();
        RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();

        int WSeqNo;

        string WOperator;
        string WSignedId;
        string WMatchingCateg;
        int WRMCycle;
        int WMode; 
       
        public Form78d_Matched(string InOperator , string InSignedId, string InMatchingCateg, int InRMCycle, int InMode)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WMatchingCateg = InMatchingCateg;
            WRMCycle = InRMCycle;
            WMode = InMode;

            // Mode = 4 => show Matched by category from Master

            // Mode = 5 => show the CIT entries that do not match with ATM group or owner 

            InitializeComponent();  
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
           
            //}
            if (WMode == 4)
            {
                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                Mpa.ReadMatchingDiscrepanciesFillTableByATM_RMCycle(WMatchingCateg, WRMCycle, WMode,2);

                labelWhatGrid.Text = "MATCHED FROM MASTER..FOR CATEGORY.."+ WMatchingCateg + "..FOR..CYCLE.." + WRMCycle.ToString();
               
                dataGridView1.DataSource = Mpa.TableFullFromMaster.DefaultView;

                dataGridView1.Show();
            
                textBoxTotalRec.Text = Mpa.TableFullFromMaster.Rows.Count.ToString();

                if (Mpa.TableFullFromMaster.Rows.Count > 500)
                {
                    // If File is rather big 
                    buttonExportToExcel.Hide(); 
                }
                else
                {
                    buttonExportToExcel.Show();
                }
            }

            if (WMode == 5)
            {

                labelWhatGrid.Text = "CIT ENTRIES WITHOUT ATM GROUP AND OWNER" ;

                string WSelectionCriteria = " WHERE GroupOfAtmsRRDM = 0  "; // Show the ones that Journal was found 

                Ce.ReadRecordsFrom_CIT_Excel_Records_Form276_CIT_Replenish(WSelectionCriteria);

                if (Ce.DataTableAllFields.Rows.Count > 0)
                {
                    //  ShowGrid1();
                    dataGridView1.DataSource = Ce.DataTableAllFields.DefaultView;
                    dataGridView1.Show();

                    textBoxTotalRec.Text = Ce.DataTableAllFields.Rows.Count.ToString();

                    buttonExportToExcel.Show();

                    ShowGrid1();
                }
                else
                {
                    MessageBox.Show("ALL ATMS HAVE GROUP AND OWNER");
                    this.Dispose();
                }
                
            }

            // SHOW GRID

        }

        // On ROW ENTER 
       
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

        }
       

       
        private void ShowGrid1()
        {

            dataGridView1.DataSource = null;
            dataGridView1.Refresh();

            dataGridView1.DataSource = Ce.DataTableAllFields.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }


            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].Visible = true;

            dataGridView1.Columns[1].Width = 55; // STATUS
            //dataGridView1.Columns[1].DefaultCellStyle = style;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 60; // AtmNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 100; // ReplCycleStartDate As CIT_Start
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 100; // ReplCycleEndDate As CIT_End
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].Visible = true;

            dataGridView1.Columns[5].Width = 50; // Journal
            //dataGridView1.Columns[5].DefaultCellStyle = style;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 52; // CIT ID 
            //dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 50; // Group 
            //dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    //WSeqNo = (int)rowSelected.Cells[0].Value;
            //    string STATUS = (string)row.Cells[1].Value;

            //    if (STATUS == "01" || STATUS == "00")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //        row.DefaultCellStyle.ForeColor = Color.Red;
            //    }
            //    if (STATUS == "02")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //        row.DefaultCellStyle.ForeColor = Color.Yellow;
            //    }
            //    if (STATUS == "03")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //        row.DefaultCellStyle.ForeColor = Color.Green;
            //    }

            //}
        }



        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Export To Excel 
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelPath = "C:\\RRDM\\Working\\CIT_INVALID" + ".xls";

            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Ce.DataTableAllFields, WorkingDir, ExcelPath);
        }
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_BULK_Records : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        
        RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();
        RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

        int WSeqNo;

        string WOperator;
        string WSignedId;
        string WSourceFileID;
        int WRMCycle;
        int WMode;
        //(WOperator, WSignedId, Flog.SourceFileID);
        public Form78d_BULK_Records(string InOperator , string InSignedId, string InSourceFileID)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WSourceFileID = InSourceFileID;
            //[RRDM_Reconciliation_ITMX].[dbo].[BULK_Switch_IST_Txns_2]
            if (WSourceFileID == "Switch_IST_Txns")
            {
                WSourceFileID = WSourceFileID + "_2"; 
            }


            InitializeComponent();

            labelWhatGrid.Text = "Records from Bulk Insert for "+ WSourceFileID; 
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
            int WMode = 1; //Show the BULK

            Mgt.ReadAndFillTableFor_File(WSourceFileID, WMode);

                dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;

                dataGridView1.Show();
            
                textBoxTotalRec.Text = Mgt.DataTableAllFields.Rows.Count.ToString();

            

            // SHOW GRID

        }

        // On ROW ENTER 
       
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

        }
     

// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Create Tap
        private void buttonCreateTap_Click(object sender, EventArgs e)
        {
            string FilePath = "C:\\RRDM\\Working\\BULK_" + WSourceFileID + ".txt";
            RRDM_EXCEL_AND_Directories Tap = new RRDM_EXCEL_AND_Directories();
            string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "BULK_" + WSourceFileID + "]";
            bool IsSuccess = Tap.CreateTapFromFile(PhysicalName, FilePath);

            if (IsSuccess == true)
            {
                MessageBox.Show("File Created..In.."+Environment.NewLine
                    + FilePath
                    );
            }
            else
            {
                MessageBox.Show("File Not Created");
            }
        }
    }
}

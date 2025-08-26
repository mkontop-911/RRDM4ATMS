using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_AllFiles_Reversals : Form
    {
      
        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC(); 
        
        string WOperator;
        string WSignedId;

        string WCategoryId;
        string WFileId;
        int WSeqNo;
     
        string PhysicalFileId; 

        public Form78d_AllFiles_Reversals(string InOperator, string InSignedId,
                           string InFiledId, int InSeqNo  ,string InCategoryId)

        {
            WSignedId = InSignedId;

            WOperator = InOperator;

            WCategoryId = InCategoryId;

            WFileId = InFiledId;

            WSeqNo = InSeqNo;
           
            InitializeComponent();

            PhysicalFileId = "[RRDM_Reconciliation_ITMX].[dbo]."+ WFileId;
            labelFileId.Text = WFileId; 
        }

        //bool WRecordFoundInUniversal;
        //string WSelectionCriteria;
      
        private void Form78b_Load(object sender, EventArgs e)
        {
            
            RRDM_ReversalsTable Rev = new RRDM_ReversalsTable();
            string WSelectionCriteria = " WHERE SeqNo="+ WSeqNo; 
            Rev.ReadReversalsBy_Selection_criteria(WSelectionCriteria);
            bool RRNBased = true; 
            if (RRNBased == true)
            {
                // RRN BASED
                Mgt.FindRecordsFromMasterRecord(WOperator, WFileId,Rev.MatchingCateg 
                    , Rev.TransDate_2.Date, Rev.TerminalId_2, 0, Rev.RRNumber_2, Rev.TransAmt_2, Rev.CardNumber_2, 3, 1);
                if (Mgt.TableDetails_RAW.Rows.Count > 0)
                {
                    dataGridView1.DataSource = Mgt.TableDetails_RAW.DefaultView;
                }

            }
            
            }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
      
    }
}

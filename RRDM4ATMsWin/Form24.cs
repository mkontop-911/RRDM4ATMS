using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 

namespace RRDM4ATMsWin
{
    public partial class Form24 : Form
    {
        // Values of ERROR Type
        //
        // 1 : Withdrawl EJournal Errors
        // 2 : Mainframe Withdrawl Errors
        // 3 : Deposit Errors Journal 
        // 4 : Deposit Mainframe Errors
        // 5 : Created by user Errors = eg moving to suspense 
        // 6 : Empty 
        // 7 : Created System Errors 
        // 

        int WTraceNo;
        int WErrNo; 
        Form62 NForm62; 
        int Action ; 

        string WFilter; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
      //  bool WPrive;
        string WAtmNo; 
        int WSesNo;

        public Form24(string InSignedId, int InSignRecordNo, string InOperator,  string InAtmNo, int InSesNo,
            string CurrNm, bool Replenishment, string InFilter)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
   
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WFilter = InFilter;

            InitializeComponent();
            // Call Procedures 
            textBox1.Text = InAtmNo;
            textBox2.Text = InSesNo.ToString();   
        }

        private void Form24_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet22.ErrorsTable' table. You can move, or remove it, as needed.
            this.errorsTableTableAdapter.Fill(this.aTMSDataSet22.ErrorsTable);
            errorsTableBindingSource.Filter = WFilter;
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
            this.errorsTableTableAdapter.Fill(this.aTMSDataSet22.ErrorsTable);

            //dataGridView1.Columns[0].HeaderText = LocRM.GetString("ErrorsTableHeader1", culture);;
        }
        // ON ROW ENTER GET THE TRACE NUMBER OR TRANS NO 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WErrNo = (int)rowSelected.Cells[0].Value;

            RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

            Er.ReadErrorsTableSpecific(WErrNo); 

            WTraceNo = Er.TraceNo;
        }
        // Show Journal 
        private void button7_Click(object sender, EventArgs e)
        {
            if (WAtmNo == "AB102" || WAtmNo == "AB104" || WAtmNo == "EWB102")
            {
                //Form67 NForm67;
                //String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
                if (WAtmNo == "AB102" || WAtmNo == "EWB102")
                {
                    WAtmNo = "AB104";
                    WSesNo = 7759;
                    WTraceNo = 10042990;
                }
              
                //int Mode = 1; // Specific

                //NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, WAtmNo, WTraceNo, WTraceNo, Mode);
                //NForm67.ShowDialog();

                DateTime NullPastDate = new DateTime(1900, 01, 01);
                Action = 25;
                string SingleChoice = WTraceNo.ToString();
                NForm62 = new Form62(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, Action,
                    NullPastDate, NullPastDate, SingleChoice);
                NForm62.ShowDialog();

                return; 
            }

            if (WAtmNo == "EWB311")
            {
                Form67 NForm67; 
                String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

                WAtmNo = "AB104";
                WSesNo = 7759;
                WTraceNo = 10043180;

                int Mode = 1; // Specific

                NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, WAtmNo, WTraceNo, WTraceNo, Mode);
                NForm67.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not available data for thid case "); 
            }
          
        }

        
    }
}

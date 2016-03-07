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
    public partial class Form75 : Form
    {
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;
        string WFilterAtm;
        string WFilterHost;
        int WTranNo; 

        public Form75(string InSignedId, int SignRecordNo, string InOperator, string InAtmNo, int InSesNo, string InFilterAtm, string InFilterHost, int InTranNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WFilterAtm = InFilterAtm;
            WFilterHost = InFilterHost;
            WTranNo = InTranNo; 


            InitializeComponent();
        }

        private void Form75_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet5.InPoolTrans' table. You can move, or remove it, as needed.
            
          
            // TODO: This line of code loads data into the 'aTMSDataSet12.InPoolTrans' table. You can move, or remove it, as needed.
            inPoolTransBindingSource.Filter = WFilterAtm;
            this.inPoolTransTableAdapter.Fill(this.aTMSDataSet5.InPoolTrans);

            tblHHostTransBindingSource.Filter = WFilterHost;
            this.tblHHostTransTableAdapter.Fill(this.aTMSDataSet13.tblHHostTrans);
  
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
// Trans and Journal 
        private void button6_Click(object sender, EventArgs e)
        {
            Form62 NForm62;

            int Action = 26 ;

            string SingleChoice = WTranNo.ToString(); 

            NForm62 = new Form62(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, Action,
               NullPastDate, NullPastDate, SingleChoice);
            NForm62.ShowDialog(); ;
        }
    }
}

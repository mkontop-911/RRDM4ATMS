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
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Configuration;

//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form136 : Form
    {
        Form200 NForm20MIS;
        Form21MIS NForm21MIS;

        string WBankId;
        bool WPrive;

        string BankName;
        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;

        string WGroupName;

        int WWf;
        public Form136(string InSignedId, int SignRecordNo, int InSecLevel, string InOperator, string InGroupName, int InWf)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WGroupName = InGroupName;
            WWf = InWf;

            // WF = 0 - Controllers stats 
            // WF = 1 - MIS for ATMs operation 
            // WF = 2 - Personnel Performance  
            // WF = 3 - ATMs Daily Business 
            // WF = 4 - CIT Performance 
            // WF = 5 - MIS for Disputes  
            // WF = 6 - ATMs Profitability  

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            if (WWf == 0 ) labelStep1.Text = "Controllers stats for Group: " + InGroupName;
            if (WWf == 1) labelStep1.Text = "MIS for ATMs operation for Group: " + InGroupName;
            if (WWf == 2) labelStep1.Text = "Personnel Performance for Group: " + InGroupName;
            if (WWf == 3) labelStep1.Text = "ATMs Daily Business for Group: " + InGroupName;
            if (WWf == 4) labelStep1.Text = "CIT Performance for Group: " + InGroupName;
            if (WWf == 5) labelStep1.Text = "MIS for Disputes for Group: " + InGroupName;
            if (WWf == 6) labelStep1.Text = "ATMs Profitability for Group: " + InGroupName;  

            textBoxMsgBoard.Text = "Choose the Bank from table and press Next"; 
        }

        private void Form136_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet31.BANKS' table. You can move, or remove it, as needed.
            string Gridfilter = "GroupName ='" + WGroupName + "'";
            bANKSBindingSource.Filter = Gridfilter;
            this.bANKSTableAdapter.Fill(this.aTMSDataSet31.BANKS);

        }
        // On Row Enter 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WBankId = (string)rowSelected.Cells[0].Value;
            WPrive = (bool)rowSelected.Cells[1].Value;
            BankName = (string)rowSelected.Cells[2].Value;
            textBox1.Text = BankName;
            string Country = (string)rowSelected.Cells[3].Value;
            textBox2.Text = Country;     
        }


        // NEXT =  // Go to Show today's Status AND MIS reports  
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (WWf == 0)
            {
                // 0 stands for CitNo... CitNo>0 when calling from Form18 (Cit View)
                NForm20MIS = new Form200(WSignedId, WSignRecordNo, WSecLevel, WOperator, "");
                NForm20MIS.Show();
            }

            if (WWf == 1)
            {
                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WWf);
                NForm21MIS.Show();
            }

            if (WWf == 2)
            {
                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WWf);
                NForm21MIS.Show();
            }

            if (WWf == 3)
            {
                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WWf);
                NForm21MIS.Show();
            }

            if (WWf == 4)
            {
                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WWf);
                NForm21MIS.Show();
            }

            if (WWf == 5)
            {
                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WWf);
                NForm21MIS.Show();
            }

            if (WWf == 6)
            {
                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WWf);
                NForm21MIS.Show();
            }
        }

       
    }
}

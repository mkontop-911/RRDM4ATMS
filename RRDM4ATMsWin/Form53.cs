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
    public partial class Form53 : Form
    {

        Form68 NForm68; 

        string Gridfilter;

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMAllowedAtmsAndUpdateFromJournal Aj = new RRDMAllowedAtmsAndUpdateFromJournal();

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); 

        DateTime FromDate ;
        DateTime ToDate;

        int SelModeA;
        int SelModeB;
        string EnteredNumber;
        int WTraceNo;

        string WAtmNo;
        int WSesNo;

        string WSignedId;
        int WSignRecordNo;
        string WOperator; 
  

        public Form53(string InSignedId, int InSignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            // Read USER and ATM Table 
            // GET TABLE OF ALLOWED ATMS FOR REPLENISH
            string WFunction = "Any";
            Aj.CreateTableOfAccess(WSignedId, WSignRecordNo, WOperator, WFunction);

            // From eJournal update traces and transactions based on table  
            Aj.UpdateLatestEjStatus(WSignedId, WSignRecordNo, WOperator);

            //TEST
            dateTimePicker1.Value = new DateTime(2014, 02, 12);
            dateTimePicker2.Value = new DateTime(2014, 02, 13);

            radioButton1.Checked = true; // Single ATM

            radioButton3.Checked = true; // All for period 

            textBoxMsgBoard.Text = "Make your selection for Drilling ";
         
          
        }
        // Load 
        private void Form53_Load(object sender, EventArgs e)
        {
                
                Gridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'";

                atmsMainBindingSource.Filter = Gridfilter;
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                this.atmsMainTableAdapter.Fill(this.aTMSDataSet44.AtmsMain);

                if (dataGridView1.Rows.Count == 0)
                {
                    Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                    MessageForm.ShowDialog();

                    this.Dispose();
                    return;
                }

                //TEST
                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[1].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, 1));

        }
        // ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;

            label17.Text = "REPL. CYCLE/s FOR ATM : " + WAtmNo; 

            string filter = "AtmNo ='" + WAtmNo + "' AND (ProcessMode = -1 OR ProcessMode = 0 OR ProcessMode = 1 OR ProcessMode = 2 OR ProcessMode = 3)";

            sessionsStatusTracesBindingSource.Filter = filter;
            dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);
            this.sessionsStatusTracesTableAdapter.Fill(this.aTMSDataSet45.SessionsStatusTraces);

            textBox2.Text = WAtmNo;
          
            textBoxMsgBoard.Text = "Make your selection for Drilling ";

        }
        //
        // On Row ENTER 
        //
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSesNo = (int)rowSelected.Cells[0].Value;

            textBox4.Text = WSesNo.ToString();

        }
        // Show 
        private void button7_Click(object sender, EventArgs e)
        {
            //TEST
            if (WAtmNo == "AB104" || WAtmNo == "ABC502")
            {
            }
            else
            {
                MessageBox.Show("For testing Please enter only AB104 or ABC502 ATM ... ");
                return;
            }

            FromDate = dateTimePicker1.Value;
            ToDate = dateTimePicker2.Value;

            if (radioButton1.Checked == true) SelModeA = 1;
            if (radioButton2.Checked == true) SelModeA = 2;
            if (radioButton3.Checked == true) SelModeB = 3;
            if (radioButton4.Checked == true) SelModeB = 4;
            if (radioButton5.Checked == true) SelModeB = 5;
            if (radioButton6.Checked == true) SelModeB = 6;
            if (radioButton7.Checked == true) SelModeB = 7;
            if (radioButton8.Checked == true) SelModeB = 8;
            if (radioButton9.Checked == true) SelModeB = 9;
            if (radioButton10.Checked == true) SelModeB = 10;

            if (SelModeA == 2 & SelModeB == 3 || SelModeA == 2 & SelModeB == 4 || SelModeA == 2 & SelModeB == 6
                || SelModeA == 2 & SelModeB == 7 || SelModeA == 2 & SelModeB == 10)
            {
                MessageBox.Show("This option is only available when one Atm is selected ");
                return; 
            }

            if (SelModeB == 8 || SelModeB == 9 || SelModeB == 10)
            {
                // Check if Number is entered 
                if (String.IsNullOrEmpty(textBox1.Text))
                {
                    if (SelModeB == 8)
                    {
                        MessageBox.Show("Enter Data for card such as 4506531111117072");
                        textBox1.Text = "4506531111117072";
                    }
                    if (SelModeB == 9)
                    {
                        MessageBox.Show("Enter Data for Account such as 012801038482");
                        textBox1.Text = "012801038482";
                    }
                    if (SelModeB == 10)
                    {
                        MessageBox.Show("Enter Data for TraceNumber such as 10042990 ");
                        textBox1.Text = "10042990";
                    } 
                      
                    return;
                }
                else // There is value = something will be reported 
                {
                    EnteredNumber = textBox1.Text; 
                }
            }
            if (SelModeB == 10)
            {
                
                if (int.TryParse(EnteredNumber, out WTraceNo))
                {
                    EnteredNumber = WTraceNo.ToString();
                }
                else
                {
                    MessageBox.Show(textBox1.Text, "Please enter a valid Trace number!");
                    return;
                }
            }
            
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            NForm68 = new Form68(WSignedId, WSignRecordNo, WOperator, JournalId, FromDate.Date,ToDate.Date, WAtmNo, WSesNo, 
                Ta.FirstTraceNo, Ta.LastTraceNo, EnteredNumber, WTraceNo, SelModeA, SelModeB);
            NForm68.ShowDialog(); 

        }
        // On Change 
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton3.Visible = false;
            radioButton4.Visible = false;
            radioButton6.Visible = false;
            radioButton7.Visible = false;
            radioButton10.Visible = false;
            radioButton5.Checked = true; 
        }
        // on change 
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton3.Visible = true;
            radioButton4.Visible = true;
            radioButton6.Visible = true;
            radioButton7.Visible = true;
            radioButton10.Visible = true;
            radioButton3.Checked = true; 

        }
        // Empty textBox1 
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = ""; 
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = ""; 
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        // Finish
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}

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
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form19a : Form
    {
       
        RRDMAllowedAtmsAndUpdateFromJournal Aj = new RRDMAllowedAtmsAndUpdateFromJournal();
        RRDMUpdateGrids Ug = new RRDMUpdateGrids();
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMHostMatchingClassGeneralLedger Hm = new RRDMHostMatchingClassGeneralLedger();
        RRDMHostGeneralLegerTblsClass Ht = new RRDMHostGeneralLegerTblsClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

  
        string Gridfilter1;
        string Gridfilter2;
        string WAtmNo; 
        int WPostedNo;
        int WRefNoHost; 
     
        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;

        int WAction;

        public Form19a(string InSignedId, int SignRecordNo, int InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            WAction = InAction;  // 1 = Matching 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            textBoxMsgBoard.Text = "Review status and use force matching facility if necessary.  "; 
           
        }

        private void Form19a_Load(object sender, EventArgs e)
        {
            //START
            //=================================================
            // LOAD FILES AND SET UP THE AUTHORISED USER IN GRID
            RRDMUpdateAuthUserForSpecialGrids Up = new RRDMUpdateAuthUserForSpecialGrids();
            int WMode = 1; // if 1 = No updating of latest ejournals info 
            // if 2 = Updating of the last ejournals info 
            Up.UpdateAuthUserForTransToBePostedMethod(WSignedId, WSignRecordNo,
                      WOperator, WMode);

            // =======================================================
            try
            {

                // Call the method in class for matching 

                bool HostMatched = false; // find the unmatched 
                Hm.MakeMatching(WOperator, HostMatched);

                Hm.GetMatchingTotals(WOperator);
                //END

                /*
            if (Hm.TotalNotMatched == 0)
            {
                MessageBox.Show("All Matched! ");
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Matched Process has finished. Total Not matched = " + Hm.TotalNotMatched.ToString());
            }
         */
                // Unmatched Trans to be posted Grid
                Gridfilter1 = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "' AND HostMatched = 0 AND ActionCd2 = 1";

                transToBePostedBindingSource.Filter = Gridfilter1;
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                this.transToBePostedTableAdapter.Fill(this.aTMSDataSet21.TransToBePosted);

                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No other transactions to be matched");
                    this.Dispose();
                    return;
                }

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }
        }
        // ON first Grid Row Enter 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;
            WPostedNo = (int)rowSelected.Cells[1].Value;

            Tc.ReadTransToBePostedSpecific(WPostedNo);

            textBox2.Text = "ATM NO : " + WAtmNo;

            textBox5.Text = Tc.ActionDate.ToShortDateString();
            textBox6.Text = Tc.PostedNo.ToString();
            textBox7.Text = Tc.CurrDesc;
            textBox8.Text = Tc.TranAmount.ToString();

            // Reason Of Force Matching 

            RRDMGasParameters Gp = new RRDMGasParameters();
            Gp.ParamId = "220"; // Reasons For Matching 
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            // Unmatched Host Trans Grid per ATM
            Gridfilter2 = "Operator ='" + WOperator + "' AND AtmNo ='" + WAtmNo + "' AND TranMatched = 0 ";

            tblHHostGeneralLedgerBindingSource.Filter = Gridfilter2;
            dataGridView2.Sort(dataGridView2.Columns[1], ListSortDirection.Descending);
            this.tblHHostGeneralLedgerTableAdapter.Fill(this.aTMSDataSet41.tblHHostGeneralLedger);
            if (dataGridView2.Rows.Count == 0)
            {
                label1.Text = "No Host Data";
                label2.Text = "No Host Data";
                label3.Text = "No Host Data";
                label4.Text = "No Host Data";
            }
            else
            {
            }
        }

        // Row Enter for General Ledger at Host 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            
                DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

                WAtmNo = (string)rowSelected.Cells[0].Value;
                WRefNoHost = (int)rowSelected.Cells[1].Value;

                Ht.ReadHostGeneralLedgerSpecificRefNo(WAtmNo, WRefNoHost);

                textBox10.Text = Ht.HostDtTm.ToShortDateString();
                textBox11.Text = Ht.RefNo.ToString();
                textBox12.Text = Ht.Curr;
                textBox13.Text = Ht.TranAmnt.ToString();

                // Compare values 
                if (Tc.ActionDate.Date == Ht.HostDtTm.Date)
                {
                    Color Black = Color.Black;
                    label1.ForeColor = Black;
                    label1.Text = "Same";
                }
                else
                {
                    label1.Text = "Differ";
                }
                if (Tc.PostedNo == Ht.RefNo)
                {
                    Color Black = Color.Black;
                    label2.ForeColor = Black;
                    label2.Text = "Same";
                }
                else
                {
                    Color Red = Color.Red;
                    label2.ForeColor = Red;
                    label2.Text = "Differ";
                }
                if (Tc.CurrDesc == Ht.Curr)
                {
                    Color Black = Color.Black;
                    label3.ForeColor = Black;
                    label3.Text = "Same";
                }
                else
                {
                    Color Red = Color.Red;
                    label3.ForeColor = Red;
                    label3.Text = "Differ";
                }
                if (Tc.TranAmount == Ht.TranAmnt)
                {
                    Color Black = Color.Black;
                    label4.ForeColor = Black;
                    label4.Text = "Same";
                }
                else
                {
                    Color Red = Color.Red;
                    label4.ForeColor = Red;
                    label4.Text = "Differ";
                }         

        }

        // Exception handling 
        private void button2_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Warning: Do you want to match by exception this unmatched? ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                               == DialogResult.Yes)
            {
                // Check Input
                if (comboBox1.Text == "Not Defined")
                {
                    MessageBox.Show("Please specify reason of force matching");
                    return;
                }

             // Update
             // Matched, date of matched and Reconcile record
             Tc.UpdateTransToBePostedMatched(WAtmNo, WPostedNo, true, DateTime.Now, 1);

             // Update host trans with the date of matching 
          
             Ht.TranMatched = true;
             Ht.MatchedDtTm = DateTime.Today;
             Ht.Comment = comboBox1.Text; 
             Ht.UpdateHostGeneralLedgerSpecific(WAtmNo, WRefNoHost);

             MessageBox.Show("Forced matching completed. Action will be treated as exception and emails will be sent.");

                // Send Emails for this exception
            }
            else
            {
                return; 
            }

            Form19a_Load(this, new EventArgs());
        }

        // Finish 
        private void buttonfinish_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

      
        
    }
}

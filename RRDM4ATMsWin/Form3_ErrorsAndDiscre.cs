using System;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form3_ErrorsAndDiscre : Form
    {
        //string WStringInput;
        string SelectionCriteria;
        //int WIntInput;
        //int UniqueIdType;
        //string WFunction;
        //string WIncludeNotMatchedYet;
        // "1" = Include
        // "" = Not Include

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //Form80b NForm80b;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
        string WMatchingCateg;
        int WRMCycle; 


        public Form3_ErrorsAndDiscre(string InSignedId, int InSignRecordNo, string InSecLevel, string InOperator, string InMatchingCateg, int InRMCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WMatchingCateg = InMatchingCateg;
            WRMCycle = InRMCycle; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            pictureBox2.BackgroundImage = appResImg.imageBranch;
            pictureBox3.BackgroundImage = appResImg.CallCentre1;
            pictureBox4.BackgroundImage = appResImg.BackOffice;

            UserId.Text = InSignedId;

            dateTimePicker2.Value = new DateTime(2017, 02, 01);
        }
       
        // Go to next 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (radioButtonDiscrepancies.Checked == false & radioButtonErrors.Checked == false
                 & radioButtonDiscrepanciesCateg.Checked == false & radioButtonErrorsAtm.Checked == false)
            {
                MessageBox.Show("Please select button");
                return;
            }
            if (radioButtonDiscrepancies.Checked == true || radioButtonDiscrepanciesCateg.Checked == true)
            {
               // RRDMMatchingOfTxnsBDO Mt = new RRDMMatchingOfTxnsBDO();

                Form78d NForm78d;

                if (radioButtonDiscrepancies.Checked == true)
                {
                   // Mt.ReadMpaAndFindDiscrepancies("", dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                }
                if (radioButtonDiscrepanciesCateg.Checked == true)
                {
                   // Mt.ReadMpaAndFindDiscrepancies(textBoxInputField.Text, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                }
                
                string WHeader = "LIST OF Matched And  UnMatched And Dublicate";

                //NForm78d = new Form78d(WSignedId, WSignRecordNo, WOperator,
                //    Mt.TableUnMatchedCompressed, WHeader, "Form78cSummary", WMatchingCateg, WRMCycle);

               // NForm78d.ShowDialog();
            }

                if (radioButtonErrors.Checked == true || radioButtonErrorsAtm.Checked == true)
            {
                Form78d NForm78d;

                RRDMMatchingtblHstAtmTxns Mht = new RRDMMatchingtblHstAtmTxns();

                if (radioButtonErrors.Checked == true)
                {
                    SelectionCriteria = " WHERE BankID = 'BNORPHMM' "
                          + " AND (TxtLine LIKE '%ERR112%' OR TxtLine LIKE '%ERR115%' )";
                }
                if (radioButtonErrorsAtm.Checked == true)
                {
                    SelectionCriteria = " WHERE BankID = 'BNORPHMM'  AND AtmNo ='" + textBoxInputField.Text +"'"
                          + " AND (TxtLine LIKE '%ERR112%' OR TxtLine LIKE '%ERR115%' )";
                }
             
                Mht.ReadtblHstAtmTxns(SelectionCriteria, dateTimePicker1.Value, dateTimePicker2.Value);

                string WHeader = "LIST OF Journal Errors From Date : "
                             + dateTimePicker1.Value.ToString() + " To "
                             + dateTimePicker2.Value.ToString(); 

                NForm78d = new Form78d(WSignedId, WSignRecordNo, WOperator,
                                       Mht.HstAtmTxnsDataTable, WHeader, "Atms-Admin-Errors", WMatchingCateg, WRMCycle);

                NForm78d.ShowDialog();

                return; 
            }

           
        }
        // Descrepancies 
        private void radioButtonDiscrepancies_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDiscrepancies.Checked == true)
            {
                labelInput.Hide();
                textBoxInputField.Hide();

                textBoxInputField.Text = "";

                dateTimePicker1.Value = new DateTime(2017, 05, 15);
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
// Errors
        private void radioButtonErrors_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonErrors.Checked == true)
            {
                labelInput.Hide();

                textBoxInputField.Hide();

                dateTimePicker1.Value = new DateTime(2017, 05, 15);
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
// Category Discrepancies 
        private void radioButtonDiscrepanciesCateg_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDiscrepanciesCateg.Checked == true)
            {
                
                labelInput.Show();
                textBoxInputField.Show();

                textBoxInputField.Text = "EWB103";

                labelInput.Text = "Category Id";
                
                dateTimePicker1.Value = new DateTime(2017, 05, 15);
                dateTimePicker2.Value = DateTime.Now.Date;

            }
        }
// Journal Errors ATM 
        private void radioButtonErrorsAtm_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonErrorsAtm.Checked == true)
            {
                labelInput.Show();
                labelInput.Text = "ATM No";
                textBoxInputField.Show(); 
                textBoxInputField.Text = "00005128";
              
                dateTimePicker1.Value = new DateTime(2017, 05, 15);
                dateTimePicker2.Value = DateTime.Now.Date;
            }
        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

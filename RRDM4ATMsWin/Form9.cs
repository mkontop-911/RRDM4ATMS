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

namespace RRDM4ATMsWin
{
    public partial class Form9 : Form
    {
        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;

        int WAction;

        public Form9(string InSignedId, int InSignRecordNo, int InSecLevel, string InOperator ,int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            WAction = InAction;  // 1 = By Period Quality of Repl and Reconc, 
            InitializeComponent();

            pictureBox1.BackgroundImage = Properties.Resources.logo2;
        }

        // ATMs Current Status 
       
        private void button1_Click_1(object sender, EventArgs e)
        {
            Form56R10 CurrentAtmsStatus = new Form56R10(WOperator);
            CurrentAtmsStatus.Show();
           
        }
        // Atms Basic information 
        private void button2_Click(object sender, EventArgs e)
        {
            Form56R11 AtmsBasic = new Form56R11(WOperator);
            AtmsBasic.Show();
        }
        // ATMs Basic Other including costs
        private void button3_Click(object sender, EventArgs e)
        {
            Form56R12 AtmsCost = new Form56R12(WOperator);
            AtmsCost.Show();
        }
        // ATMs Profitability 
        private void button4_Click(object sender, EventArgs e)
        {
            // Form56R13
            MessageBox.Show("Future development with SQL Reporting Services");
        }
        // ATMs Diputes 
        private void button5_Click(object sender, EventArgs e)
        {
            Form56R14 Disputes = new Form56R14(WOperator);
            Disputes.Show();
        }
// Visa Authorisations 
        private void button6_Click(object sender, EventArgs e)
        {
            // Print 

            string WD11;

            WD11 = WOperator;

            Form56R_EWB00_VisaAuthPool ReportMatched = new Form56R_EWB00_VisaAuthPool(WD11);
            ReportMatched.Show();
        }
    }
}

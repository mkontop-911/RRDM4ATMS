using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form9 : Form
    {
        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        int WAction;

        public Form9(string InSignedId, int InSignRecordNo, string InSecLevel, string InOperator ,int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            WAction = InAction;  // 1 = By Period Quality of Repl and Reconc, 
            InitializeComponent();

            pictureBox1.BackgroundImage = appResImg.logo2;
        }

        // ATMs Current Status 
       
        private void button1_Click_1(object sender, EventArgs e)
        {
            // Print
            string P1 = "Current ATMs Status ";

            string P2 = "";  
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R10 CurrentAtmsStatus = new Form56R10(P1, P2, P3, P4, P5);
            CurrentAtmsStatus.Show();
           
        }
        // Atms Basic information 
        private void button2_Click(object sender, EventArgs e)
        {
            string P1 = "ATMs Basic Information ";

            string P2 = "";
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R11 AtmsBasic = new Form56R11(P1, P2, P3, P4, P5);
            AtmsBasic.Show();
        }
        // ATMs Basic Other including costs
        private void button3_Click(object sender, EventArgs e)
        {
            string P1 = "ATMs Costs "; 

            string P2 = "";
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R12 AtmsCost = new Form56R12(P1, P2, P3, P4, P5);
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

            MessageBox.Show("Not available testing data for this report.");
            return; 

            //Form56R_EWB00_VisaAuthPool ReportMatched = new Form56R_EWB00_VisaAuthPool(WD11);
            //ReportMatched.Show();
        }

    }
}

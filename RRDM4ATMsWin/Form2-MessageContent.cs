using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form2_MessageContent : Form
    {
        RRDMEmailClass2 Em = new RRDMEmailClass2();

        string WSignedId;
        int WSignRecordNo;
        string WOperator; 
        string WHeading;
     
        string WMessageContent;

        int WRMCycle;
        int WMode; 

        public Form2_MessageContent(string InSignedId, int InSignRecordNo, string InOperator ,string InHeading ,string InMessageContent, 
                                    int InRMCycle, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator; 
            WHeading = InHeading;
            WMessageContent = InMessageContent;
            WRMCycle = InRMCycle;
            WMode = InMode; 
               // 1 coming from Files
               // 2 coming from Matching

            InitializeComponent();
          
        }
        // Load Form 
        private void Form2_MessageContent_Load(object sender, EventArgs e)
        {
            labelStep1.Text = WHeading;
            textBox1.Text = WMessageContent;

        }

        // Proceed and send email 
        private void buttonProceed_Click(object sender, EventArgs e)
        {

            string Recipient = "panicos.michael@cablenet.com.cy";

            string Subject = WHeading;

            Em.SendEmail(WOperator, Recipient, Subject, WMessageContent);
            if (Em.MessageSent == true)
            {
                MessageBox.Show("Email to: " + Recipient + " Has been sent");
               
                this.Close(); 
            }
            else
            {
                MessageBox.Show("Email to:" + Recipient + " was not sent. There might be problem with Internet. " + Environment.NewLine
                                + "Transactions for CIT were created." + Environment.NewLine
                                + "Send the created report to CIT through other ways."
                                );

            
                this.Close(); 
            }
        }
        // Finish
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
           
                this.Dispose();

        }
// Print
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            if (WMode == 1)
            {
                string WHeader = "Status of Loading Files for Cycle.." + WRMCycle;


                //("Header", WR1));
                //("InBankIdLogo", WR2));
                //("RMCycle", WR3));
                Form56R59ATMS_M_LoadingFiles NForm56R59ATMS_M_LoadingFiles;
                Form56R59ATMS_M_LoadingFiles Printing = new Form56R59ATMS_M_LoadingFiles(WHeader, WOperator, WRMCycle.ToString());
                Printing.Show();
                
            }
            if (WMode == 2)
            {
                string WHeader = "Status of Matching for Cycle.." + WRMCycle;


                //("Header", WR1));
                //("InBankIdLogo", WR2));
                //("RMCycle", WR3));
                Form56R59ATMS_Matching NForm56R59ATMS_Matching;
                Form56R59ATMS_Matching Printing = new Form56R59ATMS_Matching(WHeader, WOperator, WRMCycle.ToString());
                Printing.Show();
                
            }
            
        }
    }
}

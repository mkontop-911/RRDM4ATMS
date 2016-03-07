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
    public partial class Form2_EMailContent : Form
    {
        RRDMEmailClass2 Em = new RRDMEmailClass2();

        RRDMReplActionsClass Ra = new RRDMReplActionsClass();

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCitId;

        string WEmailContent;

        public Form2_EMailContent(string InSignedId, int InSignRecordNo, string InOperator, string InCitId, string InEmailContent)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId;
            WEmailContent = InEmailContent; 
            InitializeComponent();
        }
        // Load Form 
        private void Form2_EMailContent_Load(object sender, EventArgs e)
        {
            textBox1.Text = WEmailContent;  
        }
        // Proceed and send email 
        private void buttonProceed_Click(object sender, EventArgs e)
        {
            string Recipient = "panicos.michael@cablenet.com.cy";

            string Subject = "Replenish ATM";

            Em.SendEmail(WOperator, Recipient, Subject, WEmailContent);
            if (Em.MessageSent == true)
            {
                MessageBox.Show("Email to: " + Recipient + " Has been sent");

                int Function = 2; // .. Do update 
                Ra.ReadReplActionsForCITAndUpdate(WCitId, WOperator, WSignedId, Function);

                this.Close(); 
            }
            else
            {
                MessageBox.Show("Email to:" + Recipient + " was not sent. There might be problem with Internet. " + Environment.NewLine
                                + "Transactions for CIT were created." + Environment.NewLine
                                + "Send the created report to CIT through other ways."
                                );

                int Function = 2; // .. Do update 
                Ra.ReadReplActionsForCITAndUpdate(WCitId, WOperator, WSignedId, Function);

                this.Close(); 
            }
        }
        // Cancel 
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to cancel this operation? ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
            {
                this.Dispose();
            }
            else
            {
                return; 
            }

        }
// Route 
        private void button1_Click(object sender, EventArgs e)
        {
            Form350_Route NForm350_Route;
            NForm350_Route = new Form350_Route("RECOMMENDED ROUTE TO FOLLOW");

            NForm350_Route.ShowDialog();
        }

    }
}

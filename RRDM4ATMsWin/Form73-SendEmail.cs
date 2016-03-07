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
    public partial class Form73_SendEmail : Form
    {
        string WEmail;
        string WOperator; 
        RRDMEmailClass2 Em = new RRDMEmailClass2(); 

        public Form73_SendEmail(string InOperator, string InEmail)
        {
            WEmail = InEmail;
            WOperator = InOperator;
            InitializeComponent();

            textBox1.Text = WEmail; 
        }
        // Send Email 
        private void button3_Click(object sender, EventArgs e)
        {
            string Recipient = WEmail;

            string Subject = textBox2.Text;

            string EmailBody = textBox3.Text;

            Em.SendEmail(WOperator, Recipient, Subject, EmailBody);

            if (Em.MessageSent == true)
            {
                MessageBox.Show("Email to: " + Recipient + " Has been sent");
            }
            else
            {
                MessageBox.Show("Unable to send email to : " + Recipient ); 
            }          

            button3.Hide();
        }
    }
}

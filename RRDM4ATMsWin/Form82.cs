using System;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form82 : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings
                                  ["ATMSConnectionString"].ConnectionString;

        RRDMLog4Net Log = new RRDMLog4Net(); 

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form82(string InSignedId, int InSignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            // Set Working Date 
            
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            textBoxMsgBoard.Text = "System Errors Are Listed. Email can be sent with error contents"; 
        }
// 
        // LOad 
        //

        private void Form82_Load(object sender, EventArgs e)
        {
            Log.ReadRRDMLog4NetAndFillTable(WOperator);

            dataGridView1.DataSource = Log.ErrorsSelected.DefaultView;

            if (dataGridView1.Rows.Count == 0 )
            {
                Form2 MessageForm = new Form2("No System Errors");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            dataGridView1.Columns[0].Width = 80; // SesNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
           
            dataGridView1.Columns[1].Width = 200; // Date
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[2].Width = 200; // SesDtTimeEnd
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }

        // On Row Enter 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            int WSeqNo = (int)rowSelected.Cells[0].Value;

            Log.ReadRRDMLog4NetSpecific(WSeqNo);

            textBoxErrorDetails.Text = Log.Message;
            textBoxParameterDetails.Text = Log.Parameters; 
        }
// Finish
      
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// Send email 
        private void button1_Click(object sender, EventArgs e)
        {
            RRDMEmailClass2 Em = new RRDMEmailClass2();
            string Recipient = "panicos.michael@cablenet.com.cy";

            string Subject = "SYSTEM ERROR - REPORTED BY CONTROLLER.";
            string WEmailContent = Log.Message + "NEXT IS PARAMETERS :" + Log.Parameters; 
            Em.SendEmail(WOperator, Recipient, Subject, WEmailContent);
            if (Em.MessageSent == true)
            {
                MessageBox.Show("Email to: " + Recipient + " Has been sent");
            }
            else
            {
                MessageBox.Show("Email to:" + Recipient + " was not sent. There might be problem with Internet. " + Environment.NewLine
                                + "Transactions for CIT were created." + Environment.NewLine
                                + "Send the created report to CIT through other ways."
                                );
            }
        }
    
    }
}

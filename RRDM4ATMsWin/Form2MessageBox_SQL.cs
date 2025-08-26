using System;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class Form2MessageBox_SQL : Form
    {
        string WMessage1;
        string WMessage2; 

        public Form2MessageBox_SQL(string InMessage1, string InMessage2)
        {
            WMessage1 = InMessage1;
            WMessage2 = InMessage2;

            InitializeComponent();

            //label1.Text = WMessage1;
            //textBoxSQLCmd.Text = WMessage2;

        }
        // Load form 
        private void Form2MessageBox_Load(object sender, EventArgs e)
        {
          // MessageBox.Show(WMessage); 
            label1.Text = WMessage1; 
            textBoxSQLCmd.Text = WMessage2;
        }
        // Click OK 
        private void OK_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
       

    }
}

using System;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class Form304 : Form
    {
        Form307 NForm307;
        Form308 NForm308;

        public Form304()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NForm307 = new Form307();

            NForm307.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            NForm307 = new Form307();

            NForm307.Show();

        }
        // Dispute management 
        private void button3_Click(object sender, EventArgs e)
        {
            NForm308 = new Form308();

            NForm308.Show();

        }
    }
}

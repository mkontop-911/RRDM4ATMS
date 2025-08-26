using System;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class MessageBoxCustom : Form
    {
        public MessageBoxCustom(string Message)
        {
            InitializeComponent();
            label1.Text = Message;
        }

        private void CustomMessageBox_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

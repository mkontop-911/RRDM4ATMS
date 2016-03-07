using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class Form2 : Form
    {
        public Form2(string message)
        {
            InitializeComponent();

            label1.Text = message;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

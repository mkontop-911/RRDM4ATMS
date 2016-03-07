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
    public partial class Form2MessageBox : Form
    {
        string WMessage; 
        public Form2MessageBox(string InMessage)
        {
            WMessage = InMessage; 
            InitializeComponent();
        }
        // Load form 
        private void Form2MessageBox_Load(object sender, EventArgs e)
        {
          // MessageBox.Show(WMessage); 
            label1.Text = WMessage; 
        }
        // Click OK 
        private void OK_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
       

    }
}

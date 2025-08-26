using System;
using System.Threading.Tasks;
using System.Windows.Forms;

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

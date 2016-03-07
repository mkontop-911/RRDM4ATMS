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
    public partial class Form79 : Form
    {
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions(); 

        int WErrNo;
        string WErrDesc;
        string WCardNo;
        string WAccountNo;

        public Form79(int InErrNo, string InErrDesc, string InCardNo, string InAccountNo)
        {
            WErrNo = InErrNo;
            WErrDesc = InErrDesc;
            WCardNo = InCardNo;
            WAccountNo = InAccountNo;
           
            InitializeComponent();

            textBox1.Text = WCardNo;
            textBox2.Text = WAccountNo;
            textBox3.Text = WErrNo.ToString();
            textBox4.Text = WErrDesc; 

        }
        // UPDATE ERROR 
        private void button6_Click(object sender, EventArgs e)
        {
            Ec.ReadErrorsTableSpecific(WErrNo);

            Ec.CardNo = textBox1.Text;
            Ec.CustAccNo = textBox2.Text;

            Ec.FullCard = true; 

            Ec.UpdateErrorsTableSpecific(WErrNo);

            MessageBox.Show("Full Card No and Account number Updated");
        }
    }
}

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
    public partial class Form41 : Form
    {

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); // Make class availble 
        RRDMGasParameters Gp = new RRDMGasParameters(); 

        string WSignedId;
        string WOperator;

        public Form41(string InSignedId, string InOperator)
        {
            WSignedId = InSignedId;
            WOperator = InOperator;

            InitializeComponent();

            textBox1.Text = InSignedId; 

            pictureBox1.BackgroundImage = Properties.Resources.logo2;
        }
        // Change Password 
        private void button1_Click(object sender, EventArgs e)
        {
            // Make validation of input data 
            if (textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Please fill Data");
                return;
            }

            if (textBox3.Text.Length < 8)
            {
                MessageBox.Show("Please enter password size of 8 characters");
                return;
            }

            if (textBox3.Text != textBox4.Text)
            {
                MessageBox.Show("Data do not match");
                return;
            }
// =======================Update New Password ==========
            Us.ReadUsersRecord(WSignedId);

            Us.PassWord = textBox3.Text;

            Gp.ReadParametersSpecificId(WOperator, "451", "1", "", "");
            if (Gp.RecordFound == false)
            {
                MessageBox.Show("Parameter Not Updated");
                return; 
            }
            int Temp = ((int)Gp.Amount);

            Us.DtChanged = DateTime.Now;
            Us.DtToBeChanged = DateTime.Now.AddDays(Temp);
            Us.ForceChangePassword = false;

            Us.UpdateUser(WSignedId);

            MessageBox.Show("New Password has been updated");

            this.Dispose(); 
        }
    }
}

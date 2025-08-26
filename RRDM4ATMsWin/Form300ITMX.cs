using System;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class Form300ITMX : Form
    {
        Form304 NForm304;
        Form301 NForm301;
        Form302 NForm302;
        Form303 NForm303;
        Form305 NForm305;

        public Form300ITMX()
        {
            InitializeComponent();

            pictureBox4.BackgroundImage = appResImg.logo2;
            pictureBox1.Image = appResImg.Arrow;
            pictureBox2.Image = appResImg.Arrow;
            pictureBox3.Image = appResImg.Arrow;

            //panel17.SendToBack(); 
            //panel17.Hide(); 
        }
        // OTHER FUNCTIONS 
        private void button11_Click(object sender, EventArgs e)
        {
            NForm304 = new Form304();
            NForm304.ShowDialog();
        }

        // CIT MANAGEMENT 
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
            NForm301 = new Form301();
            NForm301.ShowDialog();

        }
        // MIS and Quality 
        private void pictureBox2_Click(object sender, EventArgs e)
        {

            NForm302 = new Form302();
            NForm302.ShowDialog();
        }
        // E- Journal and Video Clip 
        private void pictureBox3_Click(object sender, EventArgs e)
        {

            NForm303 = new Form303();
            NForm303.ShowDialog();
        }
// AUTHORISATION 
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            NForm305 = new Form305();
            NForm305.ShowDialog();
        }
    }
}

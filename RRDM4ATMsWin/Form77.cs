using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace RRDM4ATMsWin
{
    public partial class Form77 : Form
    {
        int position = 0;
        DataGridView data;

        public Form77(DataGridView dataGridView, int rowIndex)
        {

            InitializeComponent();

            data = dataGridView;
            position = rowIndex;

            SetParameters();

            CheckRange();
           
        }

        private void CheckRange()
        {
            if (position >= (data.Rows.Count-1))
            {
                linkLabelNext.Enabled = false;
            }
            else
            {
                linkLabelNext.Enabled = true;
            }
            if (position == 0)
            {
                linkLabelPrevious.Enabled = false;
            }
            else
            {
                linkLabelPrevious.Enabled = true;
            }
        }

        private void SetParameters()
        {
            labelCategory.Text = data.Rows[position].Cells[1].Value.ToString();
            labelSubCategory.Text = data.Rows[position].Cells[2].Value.ToString();
            labelChange.Text = data.Rows[position].Cells[3].Value.ToString();
            labelUser.Text = data.Rows[position].Cells[4].Value.ToString();
            labelDate.Text = data.Rows[position].Cells[5].Value.ToString();

            try
            {
                textBox1.Text = data.Rows[position].Cells[9].Value.ToString();
            }
            catch{
                textBox1.Text = "";
            }
            

            byte[] imgData = (byte[])data.Rows[position].Cells[6].Value;
            MemoryStream ms = new MemoryStream(imgData);
            System.Drawing.Image image = Image.FromStream(ms);

            pictureBox1.BackgroundImage = image;

            buttonAfterChange.Enabled = false;
            buttonPriorChange.Enabled = true;

            buttonAfterChange.Hide();
            buttonPriorChange.Show();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            position++;

            SetParameters();
            CheckRange();
            
        }

        private void linkLabelPrevious_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            position--;

            SetParameters();
            CheckRange();
        }

        private void Form77_Load(object sender, EventArgs e)
        {

        }

        private void buttonPriorChange_Click(object sender, EventArgs e)
        {
            byte[] imgData = (byte[])data.Rows[position].Cells[8].Value;
            MemoryStream ms = new MemoryStream(imgData);
            System.Drawing.Image image = Image.FromStream(ms);

            pictureBox1.BackgroundImage = image;

            label2.Text = "Prior Change";

            buttonPriorChange.Enabled = false;
            buttonAfterChange.Enabled = true;

            buttonPriorChange.Hide();
            buttonAfterChange.Show();

        }

        private void buttonAfterChange_Click(object sender, EventArgs e)
        {
            byte[] imgData = (byte[])data.Rows[position].Cells[6].Value;
            MemoryStream ms = new MemoryStream(imgData);
            System.Drawing.Image image = Image.FromStream(ms);

            pictureBox1.BackgroundImage = image;

            label2.Text = "After Change";

            buttonPriorChange.Enabled = true;
            buttonAfterChange.Enabled = false;

            buttonPriorChange.Show();
            buttonAfterChange.Hide();

        }
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using RRDM4ATMs;


//namespace TestImages
namespace TestImages
{
    public partial class FormImages : Form
    {
        RRDMBanks Ba = new RRDMBanks(); 
        //static string connectionString = ConfigurationManager.ConnectionStrings ["ImgConnectionString"].ConnectionString;
        //SqlConnection conn = new SqlConnection(connectionString);
        //SqlCommand sqlcmd;
        string ImgFileName;
        string ImgBrowseDir = ConfigurationManager.AppSettings["BrowseImageDir"];

        string WBankId; 
        public FormImages(string InBankId)
        {
            WBankId = InBankId; 

            InitializeComponent();

            textBoxImageID.Text = WBankId; 
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.InitialDirectory = ImgBrowseDir;
                //dlg.Filter = "JPG Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp|GIF Files (*.gif)|*.gif";
                dlg.Filter = "PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";
                dlg.Title = "Select an Image";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ImgFileName = dlg.FileName.ToString();
                    pictureBox.ImageLocation = ImgFileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] img = null;
                FileStream fs = new FileStream(ImgFileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                img = br.ReadBytes((int)fs.Length);

                Ba.ReadBank(WBankId);
                Ba.Logo = img;
                Ba.UpdateBank(WBankId); 

                //string sql =  "INSERT INTO Images(ImageID, ImageDescription, ImageStream) VALUES (@Id, @Descr, @Image)";
                //if (conn.State != ConnectionState.Open)
                //    conn.Open();
                //sqlcmd = new SqlCommand(sql, conn);
                //sqlcmd.Parameters.AddWithValue("@Id", textBoxImageID.Text);
                //sqlcmd.Parameters.AddWithValue("@Descr", textBoxImageDescr.Text);
                //sqlcmd.Parameters.AddWithValue("@Image", img);
                //int x = sqlcmd.ExecuteNonQuery();
                //conn.Close();
                MessageBox.Show(" record(s) saved!");
                textBoxImageID.Text = "";
                textBoxImageDescr.Text = "";
                pictureBox.Image = null;
            }
            catch (Exception ex)
            {
                //conn.Close();
                MessageBox.Show(ex.Message);
                textBoxImageID.Text = "";
                textBoxImageDescr.Text = "";
                pictureBox.Image = null;
            }

        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            textBoxImageDescr.Text = "";
            pictureBox.Image = null;
            try
            {
                Ba.ReadBank(WBankId);
                if (Ba.Logo == null)
                {
                    pictureBox.Image = null;
                    MessageBox.Show("No Logo assigned yet!");
                }
                else
                {
                    MemoryStream ms = new MemoryStream(Ba.Logo);
                    pictureBox.Image = Image.FromStream(ms);
                }
                //string sql = "SELECT ImageDescription, ImageStream FROM Images WHERE ImageId = " + textBoxImageID.Text + " ";
                //if (conn.State != ConnectionState.Open)
                //    conn.Open();
                //sqlcmd = new SqlCommand(sql, conn);
                //SqlDataReader reader = sqlcmd.ExecuteReader();
                //reader.Read();
                //if (reader.HasRows)
                //{
                //    textBoxImageDescr.Text = reader[0].ToString();
                //    byte[] img = (byte[])(reader[1]);
                //    if (img == null)
                //        pictureBox.Image = null;
                //    else
                //    {
                //        MemoryStream ms = new MemoryStream(img);
                //        pictureBox.Image = Image.FromStream(ms);
                //    }
                //}
                //else
                //{
                //    textBoxImageDescr.Text = "";
                //    pictureBox.Image = null;
                //    MessageBox.Show("The specified Image Id could not be found!");
                //}
                //conn.Close();

            }
            catch (Exception ex)
            {
                //conn.Close();
                MessageBox.Show(ex.Message);
            }

        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

        }
    }
}

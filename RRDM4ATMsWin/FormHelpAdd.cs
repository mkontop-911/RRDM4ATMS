using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace RRDM4ATMsWin
{
    public partial class FormHelpAdd : Form
    {
        RRDMHelp Cn = new RRDMHelp();
        private string SaveAttachmentPath = "";


        public FormHelpAdd()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Cn.Category = comboBox1.SelectedItem.ToString();
            Cn.Name = textBox2.Text;
            Cn.Text = textBox1.Text ;

            if (!AttachmentPath.Equals(""))
            {
                SaveAttachment();
            }

            Cn.AttachmentPath = SaveAttachmentPath;
            Cn.InsertHelpItem();

            MessageBox.Show("The item was added successfully");

            this.Dispose();

        }

        private void SaveAttachment()
        {
            string currentpath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            currentpath = currentpath + @"\Help Attachments";

            DirectoryInfo di = Directory.CreateDirectory(currentpath);

            SaveAttachmentPath = currentpath + @"\" + FileName;

            //Check if file already exists
            string[] filePaths = Directory.GetFiles(currentpath);
            bool exists = true;
            int i = 0;
            string OriginalSaveAttachmentPath = SaveAttachmentPath;
            while (exists)
            {
                int pos = Array.IndexOf(filePaths, SaveAttachmentPath);
                if (pos > -1)
                {
                    i++;
                    SaveAttachmentPath = OriginalSaveAttachmentPath;
                    // the array contains the string and the pos variable
                    // will have its position in the array
                    string extension = Path.GetExtension(SaveAttachmentPath);
                    SaveAttachmentPath = SaveAttachmentPath.Replace(extension, "");
                    SaveAttachmentPath = SaveAttachmentPath + "(" + i + ")" + extension;
                }
                else
                {
                    exists = false;
                }
            }


            System.IO.File.Copy(AttachmentPath, SaveAttachmentPath);
        }

        private string AttachmentPath = "";
        private string FileName = "";
        private bool attachmentAction=true;

        private void buttonAttach_Click(object sender, EventArgs e)
        {
            if (attachmentAction)
            {
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    AttachmentPath = openFileDialog1.FileName;

                    int i = openFileDialog1.FileName.LastIndexOf(@"\");
                    FileName = openFileDialog1.FileName.Substring(i + 1);

                    pictureBoxAttachment.BackgroundImage = GetIconImage(AttachmentPath);
                    labelAttchment.Text = FileName;

                    pictureBoxAttachment.Visible = true;
                    labelAttchment.Visible = true;
                }

                buttonAttach.Text = "Delete Attachment";
                attachmentAction = false;
            }
            else
            {
                try
                {
                    File.Delete(AttachmentPath);
                }
                catch { }

                AttachmentPath = "";
                FileName = "";
                pictureBoxAttachment.Visible = false;
                labelAttchment.Visible = false;
                buttonAttach.Text = "Insert Attachment";
                attachmentAction = true;
            }
        }

        public struct SHFILEINFO //Contains information about a file object
        {

            public IntPtr hIcon; //Icon

            public int iIcon; //Icondex

            public uint dwAttributes; //SFGAO_ flags

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;

        }

        [DllImport("shell32.dll")] //Retrieves information about an object in the file system, such as a file, folder, directory, or drive root
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        private const uint SHGFI_ICON = 0x100; //Icon
        private const uint SHGFI_SMALLICON = 0x1; //Small Icon	    
        private const uint SHGFI_LARGEICON = 0x0; // Large icon

        private const int MAX_PATH = 260; //Path to Icon

        private Bitmap GetIconImage(string strFileName)
        {

            char NullChar = Convert.ToChar(0); //NULL Character, C# Doesn't Have vbNullChar Constant Built In

            SHFILEINFO shInfo = default(SHFILEINFO); //Create File Info Object

            shInfo = new SHFILEINFO(); //Instantiate File Info Object

            shInfo.szDisplayName = new string(NullChar, MAX_PATH); //Get Display Name

            shInfo.szTypeName = new string(NullChar, 80); //Get File Type

            IntPtr hIcon = default(IntPtr); //Get File Type Icon Based On File Association

            hIcon = SHGetFileInfo(strFileName, 0, ref shInfo, (uint)Marshal.SizeOf(shInfo), SHGFI_ICON | SHGFI_SMALLICON);

            System.Drawing.Bitmap MyIcon = null; //Create Icon

            MyIcon = System.Drawing.Icon.FromHandle(shInfo.hIcon).ToBitmap(); //Set Icon

            return MyIcon;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            panel4.Enabled = checkBox2.Checked;

            if (checkBox2.Checked)
            {
                panel4.BackColor = Color.White;
            }
            else
            {
                panel4.BackColor = SystemColors.Control;
            }
        }
    }
}

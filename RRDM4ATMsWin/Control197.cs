using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using RRDM4ATMs; 

namespace RRDM4ATMsWin
{
    public partial class Control197 : UserControl
    {
        public string noteID;

        public event EventHandler deleteNote;

        public event EventHandler updateNote;

        public string AttachmentPath;

        //int originalLabel;

        public Control197(string ID, string Parameter3, string UserId, string UserName, string NoteText,string CreationDT,string attachment)
        {
            InitializeComponent();

            //originalLabel = labelNoteText.Height;

            noteID = ID;

            labelDt.Text = CreationDT;
            labelUserId.Text = UserId;
            labelUserName.Text = UserName;
            labelPar3.Text = Parameter3; 
            textBox1.Text = NoteText;

            AttachmentPath = attachment;

            if (!AttachmentPath.Equals(""))
            {
                int i = AttachmentPath.LastIndexOf(@"\");
                string FileName = AttachmentPath.Substring(i + 1);

                pictureBoxAttachment.BackgroundImage = GetIconImage(AttachmentPath);
                labelAttachment.Text = FileName;

                pictureBoxAttachment.Visible = true;
                labelAttachment.Visible = true;
            }
            
            //SetControlSize();
        }

        //private void SetControlSize()
        //{
        //    //int heightNotes = labelNoteText.Height;
        //     //float he=labelNoteText.Font.Size;

        //     string[] sArr = null;
        //     int lLines = 0;

        //     sArr=labelNoteText.Text.Split(System.Environment.NewLine.ToCharArray());
        //     lLines = (sArr.Length+ 1)/2;

        //     if (lLines > 2)
        //     {
        //         lLines = lLines - 2;
        //     }
        //     int difference = lLines* originalLabel;

        //    this.Size = new Size(this.Size.Width,this.Size.Height + difference);
        //}

        private void Control197_Load(object sender, EventArgs e)
        {

        }
        // Delete Note
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            
            if (MessageBox.Show("Are you sure you want to delete this note?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                deleteNote(this, e);
            }
        }

        // Update Note 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            updateNote(this, e);
        }

        private void pictureBoxAttachment_Click(object sender, EventArgs e)
        {

        }

        private void pictureBoxAttachment_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AttachmentPath);
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
     
    }
}

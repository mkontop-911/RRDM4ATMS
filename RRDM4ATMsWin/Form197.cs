using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.IO;
using System.Runtime.InteropServices;

namespace RRDM4ATMsWin
{
    public partial class Form197 : Form
    {
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        int WSeqNumber;

        bool WUpdateProcess;
        
        string WUserName ; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WParameter2;
        string WParameter3;
        string WParameter4;
        string WMode;
        string WSearchP4;

        public Form197(string InSignedId, int InSignRecordNo, string InOperator, string InParameter2, string InParameter3, string InParameter4, string InMode, string InSearchP4)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WParameter2 = InParameter2;
            WParameter3 = InParameter3;
            WParameter4 = InParameter4;
            WMode = InMode; // Mode Values are "Update" and "Read"
                            // With Read mode nothing can be updated 

            WSearchP4 = InSearchP4; 
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            
            comboBox1.Items.Add("Descending"); 
            comboBox1.Items.Add("Ascending");

            comboBox1.Text = "Ascending"; 

            Us.ReadUsersRecord(WSignedId); 

            WUserName = Us.UserName; 

            labelStep1.Text = "Notes For: " + WParameter4;  
           
        }
        // Load
        private void Form197_Load(object sender, EventArgs e)
        {
            if (WMode == "Read")
            {
                labelStep1.Text = "Notes - Read Only";

                if (WSearchP4 != "")
                {
                    labelStep1.Text = "Notes - Read Only for :" + WSearchP4 ;
                }
                panel2.Hide();
                label2.Hide(); 
                buttonNext.Hide();
                textBoxMsgBoard.Text = "View Notes";
            }

            textBox1.Text = "";
            Cn = new RRDMCaseNotes();

            for (int x = panelNotes.Controls.Count; x>0; x--)
            {
                panelNotes.Controls[x-1].Dispose();
            }

            string Order = comboBox1.Text;

            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4); 
            if (Cn.RecordFound == true)
            {
                //textBox2.Text = Cn.PublicNotesString; 
                for (int x = 0; Cn.NoteControlsArray.Count > x; x++)
                {
                    Control197 i = (Control197)Cn.NoteControlsArray[x];
                    i.deleteNote += i_deleteNote;
                    i.updateNote +=i_UpdateNote;
                    panelNotes.Controls.Add(i);
                    panelNotes.Controls[x].Dock = DockStyle.Top;
                }

                labelTotalNotes.Text = "Total Notes = :" + Cn.TotalNotes; 
            }
            else
            {
                textBoxMsgBoard.Text = "No Notes Available";
            }

        }

        void i_deleteNote(object sender, EventArgs e)
        {
            string tempId=((Control197)sender).noteID;
  
           int WSeqNumber;

           if (int.TryParse(tempId, out  WSeqNumber))
           {
           }
           else
           {
               MessageBox.Show(tempId, "Please enter a valid number for Sequence Number !");
               return;
           }
            
           Cn.ReadCaseNotesSpecific(WSeqNumber);
           if (Cn.RecordFound == true)
           {
               if ((Cn.PrivateForUser == true & WSignedId != Cn.UserId) )
               {
                   MessageBox.Show("User Not allowed to delete this");
                   return;
               }

               if (WMode == "Read")
               {
                   MessageBox.Show("This function is not available during viewing. ");
                   return;
               }

               Cn.DeleteCaseNotesRecord(WSeqNumber);

               try
               {
                   File.Delete(((Control197)sender).AttachmentPath);
               }
               catch { }
               
               //Form197_Load(this, new EventArgs());

               ((Control197)sender).Dispose();

               // Total Notes 
               string Order = comboBox1.Text;
               Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
               labelTotalNotes.Text = "Total Notes = :" + Cn.TotalNotes; 
                
           }
           else
           {
               MessageBox.Show(tempId, "Sequence Number not found!");
               return;
           }

           // Load to refresh
           // 
           Form197_Load(this, new EventArgs());
        }

        //************************************
        //********* UPDATE *******************
        //************************************
        string oldpath = "";

        void i_UpdateNote(object sender, EventArgs e)
        {
            if (WUpdateProcess == true)
            {
                MessageBox.Show("System already in update process !");
                return;
            }
            else
            {
                WUpdateProcess = true;
            }

            string tempId = ((Control197)sender).noteID;

            if (int.TryParse(tempId, out  WSeqNumber))
            {
            }
            else
            {
                MessageBox.Show(tempId, "Please enter a valid number for Sequence Number !");
                return;
            }

            Cn.ReadCaseNotesSpecific(WSeqNumber);
            if (Cn.RecordFound == true)
            {
                if ((Cn.PrivateForUser == true & WSignedId != Cn.UserId))
                {
                    MessageBox.Show("User Not allowed to Update this");
                    return;
                }

                if (WMode == "Read")
                {
                    MessageBox.Show("This function is not available during viewing. ");
                    return;
                }

                textBox1.Text = Cn.Notes;

                if (Cn.PrivateForUser == true) checkBoxPrivate.Checked = true;

                AttachmentPath = Cn.AttachmentPath;
                oldpath = AttachmentPath;

                if (!AttachmentPath.Equals(""))
                {
                    int i = AttachmentPath.LastIndexOf(@"\");
                    FileName = AttachmentPath.Substring(i + 1);

                    pictureBoxAttachment.BackgroundImage = GetIconImage(AttachmentPath);

                    pictureBoxAttachment.Visible = true;

                    buttonAttach.Text = "Delete Attachment";
                    attachmentAction = false;                
                
                }         

                buttonNext.Text = "Update";


                //try
                //{
                //    File.Delete(((Control197)sender).AttachmentPath);
                //}
                //catch { }

                //((Control197)sender).Dispose();

            }
            else
            {
                MessageBox.Show(tempId, "Sequence Number not found!");
                return;
            }
        }
        // Insert/Update Record 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length <= 8)
            {
                    MessageBox.Show(" Please Enter Note more than 8 characters");
                    return;             
            }

            if (buttonNext.Text == "Update")
            {

                Cn.ReadCaseNotesSpecific(WSeqNumber);
                if (Cn.RecordFound == true)
                {
                    Cn.Notes = textBox1.Text;

                    Cn.PrivateForUser = checkBoxPrivate.Checked;
                    if (!AttachmentPath.Equals(""))
                    {

                        //if (!oldpath.Equals(AttachmentPath))
                        //{
                            SaveAttachment();
                        //}
                        //else
                        //{
                        //    oldpath = "";
                        //}
                    }

                    Cn.AttachmentPath = SaveAttachmentPath;

                    Cn.UpdateCaseNotesRecord(WSeqNumber);

                    buttonNext.Text = "Add Note";

                    WUpdateProcess = false; 
                }
                else
                {
                    MessageBox.Show("Error 378: Note found note.");
                }

            }
            else
            {
                if (buttonNext.Text == "Add Note")
                {
                    Cn.Parameter2 = WParameter2;
                    Cn.Parameter3 = WParameter3;

                    Cn.UserId = WSignedId;
                    Cn.UserName = WUserName;

                    Cn.DateCreated = DateTime.Now;

                    Cn.Notes = textBox1.Text;

                    Cn.PrivateForUser = checkBoxPrivate.Checked;

                    Cn.Operator = WOperator;

                    if (!AttachmentPath.Equals(""))
                    {
                        SaveAttachment();
                    }

                    Cn.AttachmentPath = SaveAttachmentPath;
                    Cn.InsertCaseNotesRecord(WParameter4);
                }
            }

            ////panelNotes.Controls.Add(new Control197(WSignedId,WUserName,textBox1.Text,

            //Cn.FindCaseNotesLastNo(WParameter4);

            ////Add new control note
            //Control197 i = new Control197(Cn.SeqNumber.ToString(),WParameter3 ,WSignedId, WUserName, Cn.Notes, Cn.DateCreated.ToString(), SaveAttachmentPath);
            //i.deleteNote += i_deleteNote;
            //i.updateNote +=i_UpdateNote;
            //i.Dock = DockStyle.Top;
            //panelNotes.Controls.Add(i);

            textBox1.Text = "";
            AttachmentPath = "";
            SaveAttachmentPath = "";
            FileName = "";
            pictureBoxAttachment.Visible = false;
            labelAttchment.Visible = false;
            buttonAttach.Text = "Insert Attachment";
            attachmentAction = true;

            // Load to refresh
            // 
            Form197_Load(this, new EventArgs());

        }
// Undo specific entry in Notes 
        private void buttonBack_Click(object sender, EventArgs e)
        {
           

            this.Dispose();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Attachmentbtn_Click(object sender, EventArgs e)
        {

        }

        private bool attachmentAction=true; //true = Insert Attachment, false= Delete Attachment

        private string AttachmentPath = "";
        private string FileName = "";

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (attachmentAction)
            {
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    AttachmentPath=openFileDialog1.FileName;

                    int i=openFileDialog1.FileName.LastIndexOf(@"\");
                    FileName = openFileDialog1.FileName.Substring(i+1);

                    pictureBoxAttachment.BackgroundImage= GetIconImage(AttachmentPath);
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

        private string SaveAttachmentPath = "";

        private void SaveAttachment()
        {
            string currentpath=System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            currentpath=currentpath+@"\Notes Attachments";

            DirectoryInfo di = Directory.CreateDirectory(currentpath);

            SaveAttachmentPath = currentpath + @"\" + FileName;

            //Check if file already exists
            string[] filePaths = Directory.GetFiles(currentpath);
            bool exists = true;
            int i=0;
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
                    SaveAttachmentPath=SaveAttachmentPath.Replace(extension, "");
                    SaveAttachmentPath = SaveAttachmentPath + "(" + i + ")"+extension;
                }
                else
                {
                    exists = false;
                }
            }

            
            System.IO.File.Copy(AttachmentPath, SaveAttachmentPath);
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
        // Load form 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load to refresh
            // 
            Form197_Load(this, new EventArgs());
        }
// Print TextBox1 

        private void button1_Click_1(object sender, EventArgs e)
        {
            string WHeader = WParameter4;

            Form56R18 NotesPrinting = new Form56R18(WHeader, WOperator);
            NotesPrinting.Show();
        }
    }
}

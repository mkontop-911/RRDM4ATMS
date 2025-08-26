using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RRDM4ATMsWin
{
    public partial class FormHelp : Form
    {

        string AttachmentPath = "";

        public FormHelp()
        {
            InitializeComponent();

            LoadTree();
        }

        public FormHelp(string SelectedCategory)
        {
            InitializeComponent();

            label2.Text = "";

            LoadTree();

            SelectSpecificCategory(SelectedCategory);
        }

        private void SelectSpecificCategory(string selectedNode)
        {
            try
            {
                treeView1.SelectedNode = treeView1.Nodes[IndexOfParentNode(selectedNode)];
                treeView1.Nodes[IndexOfParentNode(selectedNode)].Expand();
          
            }
            catch { }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //int index=0;
            //int selectedParentNodeIndex = -1;
            //int selectedChildNodeIndex = -1;

            //foreach(var _item in treeView1.Nodes)
            //{
            //    if(_item==treeView1.SelectedNode)
            //    {
            //        selectedNodeIndex = index;
            //        break;
            //    }
            //    index++;
            //}

            if (treeView1.SelectedNode.Level > 0)
            {
                string selectedParentText = treeView1.SelectedNode.FullPath.Substring(0, treeView1.SelectedNode.FullPath.IndexOf("\\"));
                string selectedChildText = treeView1.SelectedNode.FullPath.Substring(treeView1.SelectedNode.FullPath.IndexOf("\\") + 1);

                //foreach (TreeNode n in treeView1.Nodes)
                //{
                //    if (n == treeView1.SelectedNode)
                //    {
                //        selectedParentNodeIndex = n.Index;

                //        foreach (TreeNode z in n.Nodes)
                //        {
                //            if (z == treeView1.SelectedNode)
                //            {
                //                selectedChildNodeIndex = z.Index;
                //                break;
                //            }
                //        }
                //        break;
                //    }
                //}

                DataRow[] SelectedRow = ds.Tables["Help"].Select("CATEGORY='" + selectedParentText + "' AND ITEM='" + selectedChildText + "'");

                label2.Text = selectedChildText;
                string textContent = SelectedRow[0]["Text"].ToString();
                textBoxTEXT.Text = textContent;
                textBoxTEXT.Visible = true;

                AttachmentPath = SelectedRow[0]["AttachmentPath"].ToString();

                if (!AttachmentPath.Equals(""))
                {
                    int i = AttachmentPath.LastIndexOf(@"\");
                    string FileName = AttachmentPath.Substring(i + 1);

                    pictureBoxAttachment.BackgroundImage = GetIconImage(AttachmentPath);
                    labelAttachment.Text = FileName;

                    pictureBoxAttachment.Visible = true;
                    labelAttachment.Visible = true;

                    panel3.Visible = true;
                }
                else
                {
                    panel3.Visible = false;
                }


            }
            else
            {
                label2.Text = "";
                textBoxTEXT.Visible = false;
                panel3.Visible = false;
            }
        }

       // ADD
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormHelpAdd FHA = new FormHelpAdd();
            FHA.Disposed += FHA_Disposed;
            FHA.ShowDialog();
        }

        void FHA_Disposed(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            LoadTree();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    
        DataSet ds = new DataSet();

        private void LoadTree()
        {
            treeView1.Nodes.Clear();

            RRDMHelp RH = new RRDMHelp();

            ds = RH.GetHelpItems();

            foreach (DataRow dr in ds.Tables["Help"].Rows)
            {
                string NodeParentName = dr["Category"].ToString();
                string NodeChildName = dr["Item"].ToString();

                TreeNode NodeParent = new TreeNode(NodeParentName);


                int position = IndexOfParentNode(NodeParentName);

                if (position < 0)
                {
                    treeView1.Nodes.Add(NodeParent);
                    position = IndexOfParentNode(NodeParentName);
                }

                TreeNode NodeChild = new TreeNode(NodeChildName);
                treeView1.Nodes[position].Nodes.Add(NodeChild);
                
            }
            
        }

        private void LoadTreeLimited(string selectedNode)
        {
            treeView1.Nodes.Clear();

            RRDMHelp RH = new RRDMHelp();

            ds = RH.GetHelpItems();

            foreach (DataRow dr in ds.Tables["Help"].Rows)
            {
                string NodeParentName = dr["Category"].ToString();

                if (NodeParentName.Equals(selectedNode))
                {
                    string NodeChildName = dr["Item"].ToString();

                    TreeNode NodeParent = new TreeNode(NodeParentName);


                    int position = IndexOfParentNode(NodeParentName);

                    if (position < 0)
                    {
                        treeView1.Nodes.Add(NodeParent);
                        position = IndexOfParentNode(NodeParentName);
                    }

                    TreeNode NodeChild = new TreeNode(NodeChildName);
                    treeView1.Nodes[position].Nodes.Add(NodeChild);
                }
            }

        }

        private int IndexOfParentNode(string NodeName)
        {
            int selectedNodeIndex = -1;

            foreach (TreeNode n in treeView1.Nodes)
            {
                if (n.Text.Equals(NodeName))
                {
                    selectedNodeIndex = n.Index;
                    break;
                }
            }

            return selectedNodeIndex;
        }

        private void FormHelp_Load(object sender, EventArgs e)
        {

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

        private void pictureBoxAttachment_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AttachmentPath);
        }

//DELETE 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeView1.SelectedNode.Level > 0)
                {
                    string selectedParentText = treeView1.SelectedNode.FullPath.Substring(0, treeView1.SelectedNode.FullPath.IndexOf("\\"));
                    string selectedChildText = treeView1.SelectedNode.FullPath.Substring(treeView1.SelectedNode.FullPath.IndexOf("\\") + 1);

                    DataRow[] SelectedRow = ds.Tables["Help"].Select("CATEGORY='" + selectedParentText + "' AND ITEM='" + selectedChildText + "'");

                    string IdToDelete = SelectedRow[0]["ID"].ToString();

                    RRDMHelp helpClass = new RRDMHelp();
                    helpClass.DeleteItem(IdToDelete);

                    LoadTree();
                }
                else
                {
                    MessageBox.Show("You need to select a specific item to delete");
                }
            }
            catch
            {
                MessageBox.Show("You need to select a specific item to delete");
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeView1.SelectedNode.Level > 0)
                {
                    string selectedParentText = treeView1.SelectedNode.FullPath.Substring(0, treeView1.SelectedNode.FullPath.IndexOf("\\"));
                    string selectedChildText = treeView1.SelectedNode.FullPath.Substring(treeView1.SelectedNode.FullPath.IndexOf("\\") + 1);

                    DataRow[] SelectedRow = ds.Tables["Help"].Select("CATEGORY='" + selectedParentText + "' AND ITEM='" + selectedChildText + "'");

                    string IdToUpdate = SelectedRow[0]["ID"].ToString();

                    string WText = textBoxTEXT.Text; 

                    RRDMHelp helpClass = new RRDMHelp();
              
                    helpClass.UpdateHelpText(IdToUpdate, WText);

                    LoadTree();
                }
                else
                {
                    MessageBox.Show("You need to select a specific item to update");
                }
            }
            catch
            {
                MessageBox.Show("You need to select a specific item to update");
            }
           
        }
    }
}

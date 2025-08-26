using System;
using System.Drawing;
using System.Configuration;
using System.Windows.Forms;
// Alecos
using System.IO;

using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class SWDForm502Pac : Form
    {
        RRDMSWDCategories Sc = new RRDMSWDCategories();
        RRDMSWDPackages Sp = new RRDMSWDPackages();
        RRDMSWDPackageFiles Pf = new RRDMSWDPackageFiles();
        RRDMSWDCatAtmDestDirectories Ad = new RRDMSWDCatAtmDestDirectories();
        RRDMSWDPackagesDistributionSessions Ds = new RRDMSWDPackagesDistributionSessions();

        int WSeqNoGrid1;
        int WSeqNoGrid2;
        int WSeqNoGrid3;

        string WSWDCategory;

        int WRowIndexLeft;
   
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        public SWDForm502Pac(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;

        }
        // Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            string Filter1 = " WHERE Operator = '" + WOperator + "' "
                             + " Order by SeqNo ASC";

            Sc.ReadSWDCategoriesAndFillTable(Filter1);

            dataGridView1.DataSource = Sc.TableSWDCateg.DefaultView;

            dataGridView1.Columns[0].Width = 60; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 80; // Id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 120; // Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // Ro
        private void dataGrid1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNoGrid1 = (int)rowSelected.Cells[0].Value;
            WSWDCategory = rowSelected.Cells[1].Value.ToString();

            // ATM destination
            Ad.ReadCatAtmDestDirectoriesbyCategory(WSWDCategory);

            if (Ad.RecordFound == true)
            {
                comboBoxAtmDestination.DataSource = Ad.GetCatAtmDestDirectories(WSWDCategory);
                comboBoxAtmDestination.DisplayMember = "DisplayValue";
            }
            else
            {

            }

            Sc.ReadSWDCategoriesbySeqNo(WOperator, WSeqNoGrid1);

            labelPackages.Text = "PACKAGES FOR : " + WSWDCategory;

            string Filter2 = " WHERE SWDCategoryId ='" + WSWDCategory + "'"
                             + " Order by SeqNo DESC"; ;
            Sp.ReadSWDPackagesAndFillTable(Filter2);
            //Rff.ReadFileFields(WSourceFileName);

            dataGridView2.DataSource = Sp.TableSWDPackages.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                // Create records
                return;
            }
            else
            {

            }

            dataGridView2.Columns[0].Width = 60; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[1].Width = 120; // PackageId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 120; // PackageName
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 90; // CreatedDtTm
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        //
        // Row Enter Right 
        //
        bool BlockPac; 
        private void dataGrid2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNoGrid2 = (int)rowSelected.Cells[0].Value;

            Sp.ReadSWDPackagesbySeqNo(WOperator, WSeqNoGrid2);

            textBoxPackId.Text = Sp.PackageId;
            textBox1.Text = Sp.BankPackId;
            textBox9.Text = Sp.PackageName;
            textBox10.Text = Sp.PackageDescription;

            checkBox1.Checked = Sp.ForUpdating;
            checkBox2.Checked = Sp.ForFullInstallation;

            checkBox5.Checked = Sp.PutOutOfService;
            checkBox6.Checked = Sp.NeedReboot;

            textBox2.Text = Sp.PriorityOneToFive.ToString();
    
            string SelectionCriteria =  " WHERE PackageId='"+ Sp.PackageId + "' AND TypeOfDistribution >1 AND Approver <> '' ";

            Ds.ReadSWDPackDistrSesbySelectionCriteria(WOperator, SelectionCriteria); 
            if (Ds.RecordFound ==true)
            {
                buttonDeleteRight.Hide();
                buttonUpdateRight.Hide();
                buttonAddFile.Hide();
                buttonDeleteFile.Hide();
                textBoxBlock.Show();
                BlockPac = true; 
            }
            else
            {
                buttonDeleteRight.Show();
                buttonUpdateRight.Show();
                buttonAddFile.Show();
                buttonDeleteFile.Show();
                textBoxBlock.Hide();
                BlockPac = false;
            }
            //
            CheckIfNotes();
            //
            labelFilesWithin.Text = "Files for Package : " + Sp.PackageId;

            string Filter3 = " WHERE SWDCategoryId ='" + WSWDCategory + "'"
                              + " AND PackId ='" + Sp.PackageId + "' ORDER By SeqNo DESC";

            Pf.ReadSWDPackageFilesFillTable(Filter3);

            dataGridView3.DataSource = Pf.TableSWDPackageFiles.DefaultView;

            if (dataGridView3.Rows.Count == 0)
            {
                // Create records
                return;
            }
            else
            {

            }

            dataGridView3.Columns[0].Width = 60; // SeqNo
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView3.Columns[1].Width = 120; // SWDCategory
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[1].Visible = false;

            dataGridView3.Columns[2].Width = 100; // PackId
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[2].Visible = false;

            dataGridView3.Columns[3].Width = 90; // Directory
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[3].Visible = false;

            dataGridView3.Columns[4].Width = 250; // FileIdOrigin
            dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[5].Width = 250; // FileIdDestination
            dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[6].Width = 250; // ATMs Directory
            dataGridView3.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        // Row Enter

        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            WSeqNoGrid3 = (int)rowSelected.Cells[0].Value;

            Pf.ReadPackageFilesbySeqNo(WOperator, WSeqNoGrid3);

            textBoxPacFile.Text = Pf.FileIdOrigin;
            comboBoxAtmDestination.Text = Pf.AtmDirId;

        }

        // Update Right 
        private void buttonUpdateRight_Click(object sender, EventArgs e)
        {
            Sp.ReadSWDPackagesbySeqNo(WOperator, WSeqNoGrid2);
            //
            //  Validation
            //  
            int Priority;
            if (int.TryParse(textBox2.Text, out Priority))
            {
            }
            else
            {
                MessageBox.Show(textBox2.Text, "Please enter a valid number for Priority.");
                return;
            }

            if (checkBox1.Checked == false & checkBox2.Checked == false)
            {
                MessageBox.Show("Please make a choice of type of package");
                return;
            }
            if (labelNumberNotes2.Text == "0")
            {
                MessageBox.Show("Please enter in Notes the document of instructions!");
                return;
            }
            
            Sp.BankPackId = textBox1.Text;

            Sp.PackageName = textBox9.Text;
            Sp.PackageDescription = textBox10.Text;

            Sp.ForUpdating = checkBox1.Checked;
            Sp.ForFullInstallation = checkBox2.Checked;

            Sp.PutOutOfService = checkBox5.Checked;
            Sp.NeedReboot = checkBox6.Checked;

            Sp.PriorityOneToFive = Priority;

            Sp.UpdateSWDPackages(WOperator, WSeqNoGrid2);

            int WRowGrid1 = dataGridView1.SelectedRows[0].Index;
            int WRowGrid2 = dataGridView2.SelectedRows[0].Index;

            Form502_Load(this, new EventArgs());

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGrid1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridView2.Rows[WRowGrid2].Selected = true;
            dataGrid2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid2));

            MessageBox.Show("Updated"); 

            panel2.Show();

        }
        // Delete Right 
        private void buttonDeleteRight_Click(object sender, EventArgs e)
        {
            if (BlockPac == true)
            {
                MessageBox.Show("You cannot delete a block Pac");
                return;
            }

            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            if (MessageBox.Show("Warning: Do you want to delete this Pack ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Sp.DeleteSWDPackages(WSeqNoGrid2);

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGrid1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // Browse 
        string ExcelFileName;
        string ExcelBrowseDir = ConfigurationManager.AppSettings["BrowseExcelDir"];
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.InitialDirectory = ExcelBrowseDir;
            
                dlg.Filter = "All files (*.*)|*.*";
                dlg.Title = "Select a File";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ExcelFileName = dlg.FileName.ToString();
                    textBoxPacFile.Text = ExcelFileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // ADD File
        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            string SWDPoolRootDir = "";
            string SWDPackageDir = "";
            string ValMsg = "";
            string ValMsgFmt = @"ParamId:{0}, OccuranceId:{1} --> {2}";


            if (textBoxPacFile.Text == "")
            {
                MessageBox.Show("Please fill in File");
            }

            Pf.ReadPackageFilesbyFileIdOrigin(WOperator, textBoxPackId.Text, textBoxPacFile.Text);
            if (Pf.RecordFound == true)
            {
                MessageBox.Show("Origin File Already Selected");
                return;
            }

            // Get SWDPackagePool from parameters table
            RRDMGasParameters Gp = new RRDMGasParameters();
            Gp.ReadParametersSpecificId(WOperator, "912", "1", "", "");
            SWDPoolRootDir = Gp.OccuranceNm;
            if (!Directory.Exists(SWDPoolRootDir))
            {
                ValMsg = string.Format(ValMsgFmt, "912", "1", "Invalid or non-existent 'SWDPackagePool' directory!\n");
                MessageBox.Show(ValMsg);
                return;
            }

            Pf.SWDCategoryId = WSWDCategory;
            Pf.PackId = textBoxPackId.Text;
            Pf.Directory = "NotDefined";
            Pf.FileIdOrigin = textBoxPacFile.Text;

            // Construct the directory name in the SWD package pool; create if it does not exist
            SWDPackageDir = string.Format("{0}\\{1}", SWDPoolRootDir.Trim(), Pf.PackId.Trim());
            try
            {
                if (!Directory.Exists(SWDPackageDir))
                {
                    Directory.CreateDirectory(SWDPackageDir);
                }
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("An exception occured while creating directory {0}! \nThe exceprion message reads:\n{1}", SWDPackageDir, ex.Message);
                MessageBox.Show(exMsg);
                return;
            }

            // Copy chosen file to the specific package dir
            string fName = Path.GetFileName(Pf.FileIdOrigin);
            string fDestFullName = Path.Combine(SWDPackageDir, fName);

            try
            {
                File.Copy(Pf.FileIdOrigin, fDestFullName, true);
                string Msg = string.Format("The file {0} was succesfully copied to directory {1}!", Pf.FileIdOrigin, SWDPackageDir);
                MessageBox.Show(Msg);

            }
            catch (Exception ex)
            {
                string exMsg = string.Format("An exception occured while copying file {0} to directory {1}! \nThe exceprion message reads:\n{2}", Pf.FileIdOrigin, SWDPackageDir, ex.Message);
                MessageBox.Show(exMsg);
                return;
            }

            Pf.FileIdDestination = fDestFullName;

            Pf.AtmDirId = comboBoxAtmDestination.Text;
            Pf.Operator = WOperator;

            Pf.InsertPackageFile();

            textBoxPacFile.Text = "";

            int WRowGrid1 = dataGridView1.SelectedRows[0].Index;
            int WRowGrid2 = dataGridView2.SelectedRows[0].Index;

            Form502_Load(this, new EventArgs());

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGrid1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridView2.Rows[WRowGrid2].Selected = true;
            dataGrid2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid2));
        }
        // Delete File 
        private void buttonDeleteFile_Click(object sender, EventArgs e)
        {
            int WRowGrid1 = dataGridView1.SelectedRows[0].Index;
            int WRowGrid2 = dataGridView2.SelectedRows[0].Index;

            if (MessageBox.Show("Warning: Do you want to delete this file ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                try
                {
                    if (File.Exists(Pf.FileIdDestination))
                    {
                        File.Delete(Pf.FileIdDestination);
                    }
                }
                catch (Exception ex)
                {
                    string exMsg = string.Format("An exception occured while deleting file {0}! \n You may need to delete this file manually.\nThe exceprion message reads:\n{1}", Pf.FileIdDestination, ex.Message);
                    MessageBox.Show(exMsg);
                }
                Pf.DeletePackageFile(WSeqNoGrid3);

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGrid1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridView2.Rows[WRowGrid2].Selected = true;
            dataGrid2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid2));
        }

        // NEW Package 

        private void checkBoxMakeNewPackage_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMakeNewPackage.Checked == true)
            {
                panel2.Hide();
                Sp.SWDCategoryId = WSWDCategory;
                Sp.PackageId = "FillData";
                Sp.BankPackId = "FillData";
                Sp.PackageName = "FillData";
                Sp.PackageDescription = "FillData";
                Sp.CreatedDtTm = DateTime.Now;
                Sp.ForUpdating = false;
                Sp.ForFullInstallation = false;
                Sp.PutOutOfService = false;
                Sp.NeedReboot = false;
                Sp.Maker = WSignedId;
                Sp.Approver = "";
                Sp.PriorityOneToFive = 3;
                Sp.Operator = WOperator;

                int PackNo = Sp.InsertSWDPackage();

                Sp.ReadSWDPackagesbySeqNo(WOperator, PackNo);

                Sp.PackageId = WSWDCategory + "-Pac-" + PackNo.ToString();

                Sp.UpdateSWDPackages(WOperator, PackNo);

                int WRowGrid1 = dataGridView1.SelectedRows[0].Index;

                Form502_Load(this, new EventArgs());

                dataGridView1.Rows[WRowGrid1].Selected = true;
                dataGrid1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));

                MessageBox.Show("A Pac with Id " + Sp.PackageId + " is created. Please fill in Data and Update");
                checkBoxMakeNewPackage.Checked = false;
            }
            else
            {
                // Do nothing 
            }

        }
        // Check If Notes 

        RRDMCaseNotes Cn = new RRDMCaseNotes();
        string WNotesMode;
        string NotesOrder;
        string WNotesSearchP4;
        string WNotesParameter3;
        string WNotesParameter4;
        string NotesSearchP4;
        private void CheckIfNotes()
        {
            // NOTES for final comment 
            NotesOrder = "Descending";
            WNotesParameter4 = "Notes for Pac:" + Sp.PackageId;
            WNotesSearchP4 = "";
            Cn.ReadAllNotes(WNotesParameter4, WSignedId, NotesOrder, WNotesSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }

        // Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            WNotesParameter3 = "";
            WNotesParameter4 = "Notes for Pac:" + Sp.PackageId;
            NotesSearchP4 = "";
            if (Sp.ClosedPack == true) WNotesMode = "Read";
            else WNotesMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WNotesParameter3, WNotesParameter4, WNotesMode, NotesSearchP4);
            NForm197.ShowDialog();

            // NOTES for final comment 
            NotesOrder = "Descending";
            WNotesParameter4 = "Notes for Pac:" + Sp.PackageId;
            WNotesSearchP4 = "";
            Cn.ReadAllNotes(WNotesParameter4, WSignedId, NotesOrder, WNotesSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }
    }
}

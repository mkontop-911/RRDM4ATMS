using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class SWDForm502Cat : Form
    {
        RRDMSWDCategories Sc = new RRDMSWDCategories();

        RRDMSWDCatAtmDestDirectories Ad = new RRDMSWDCatAtmDestDirectories();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        int WSeqNoLeft;

        string WSelectionCriteria; 

        string WSWDCategory; 

        int WRowIndexLeft;
  
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
       
        public SWDForm502Cat(string InSignedId, int SignRecordNo, string InOperator)
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

            // Suppliers 
            Gp.ParamId = "204"; // Suppliers   
            comboBoxSupplier.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxSupplier.DisplayMember = "DisplayValue";

            // Model 
            Gp.RelatedParmId = "204"; // Models
            Gp.RelatedOccuranceId = "1";
            comboBoxModel.DataSource = Gp.GetArrayParamOccurancesRelatedNm(WOperator, Gp.RelatedParmId, Gp.RelatedOccuranceId);
            comboBoxModel.DisplayMember = "DisplayValue";

        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            string Filter1 = " WHERE Operator = '" + WOperator + "' ";

            Sc.ReadSWDCategoriesAndFillTable(Filter1);
        
            dataGridView1.DataSource = Sc.TableSWDCateg.DefaultView;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 60; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 80; // Id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 150; // Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // Ro
        private void dataGrid1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNoLeft = (int)rowSelected.Cells[0].Value;
            WSWDCategory = rowSelected.Cells[1].Value.ToString();

            Sc.ReadSWDCategoriesbySeqNo(WOperator, WSeqNoLeft); 
       
            textBox1.Text = Sc.SWDCategoryId;
            textBox5.Text = Sc.SWDCategoryName;
          
            comboBoxSupplier.Text = Sc.ATMsSupplier;
        
            comboBoxModel.Text = Sc.ATMsModel; 
         
            textBox11.Text = Sc.OperatingSystem;

            textBox2.Text = Sc.EffectivePackageId;

            WSelectionCriteria = " WHERE SWDCategoryId = '" + Sc.SWDCategoryId + "' ";
            Ad.ReadCatAtmDestDirectoriesAndFillTable(WSelectionCriteria);

            dataGridView2.DataSource = Ad.TableCatAtmDestDirectories.DefaultView;

            if (dataGridView2.RowCount == 0)
            {
                buttonUpdateAtmDir.Hide();
                buttonDeleteAtmDir.Hide();
                textBoxAtmDir.Text = "";
                textBoxPurpose.Text = ""; 
                return;
            }
            else
            {
                buttonUpdateAtmDir.Show();
                buttonDeleteAtmDir.Show();
            }

            dataGridView2.Columns[0].Width = 60; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 80; // SWDCategory
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[1].Visible = false; 

            dataGridView2.Columns[2].Width = 300; // AtmDirectory
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 300; // Purpose
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Row Enter 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            int WSeqNo = (int)rowSelected.Cells[0].Value;

            Ad.ReadCatAtmDestDirectoriesbySeqNo(WOperator, WSeqNo);

            textBoxAtmDir.Text = Ad.AtmDirectory;
            textBoxPurpose.Text = Ad.Purpose; 
        }

        // ADD Left
        private void buttonAddLeft_Click(object sender, EventArgs e)
        {
            Sc.ReadSWDCategorybyCategId(WOperator, textBox1.Text);

            if (Sc.RecordFound == true)
            {
                MessageBox.Show("Category Already exist!");
                return;
            }
           
            Sc.SWDCategoryId = textBox1.Text;
            Sc.SWDCategoryName = textBox5.Text;
            Sc.SWDOrigin = "InternalSWD";
            Sc.ATMsSupplier = comboBoxSupplier.Text;
            Sc.ATMsModel = comboBoxModel.Text;
          
            Sc.OperatingSystem = textBox11.Text;
            //Sc.EffectivePackageId = textBox2.Text; 
            Sc.OpeningDateTm = DateTime.Now;
            Sc.Operator = WOperator; 

            Sc.InsertSWDCategory(); 

            Form502_Load(this, new EventArgs());

            MessageBox.Show("File Inserted");
            textBoxMsgBoard.Text = "File Inserted";

        }
// Update Left
        private void buttonUpdateLeft_Click(object sender, EventArgs e)
        {
            Sc.ReadSWDCategoriesbySeqNo(WOperator, WSeqNoLeft);

            Sc.SWDCategoryId = textBox1.Text;
            Sc.SWDCategoryName = textBox5.Text;
            Sc.SWDOrigin = "InternalSWD";
            Sc.ATMsSupplier = comboBoxSupplier.Text;
            Sc.ATMsModel = comboBoxModel.Text;
          
            Sc.OperatingSystem = textBox11.Text;
            Sc.EffectivePackageId = textBox2.Text; 
         
            Sc.Operator = WOperator;

            Sc.UpdateSWDCategory(WOperator, WSeqNoLeft);
         
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form502_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGrid1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            MessageBox.Show("File Updated");
            textBoxMsgBoard.Text = "File Updated";
        }
// Delete Left 
        private void buttonDeleteLeft_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this file ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {
                Sc.DeleteSWDCategory(WSeqNoLeft);
              
                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
           
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

// Change Supplier 
        private void comboBoxSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            Gp.ParamId = "204";
            Gp.ReadParametersSpecificNm(WOperator, Gp.ParamId, comboBoxSupplier.Text);

            // Model 
            Gp.RelatedParmId = "204"; // Supplier
            Gp.RelatedOccuranceId = Gp.OccuranceId;
            comboBoxModel.DataSource = Gp.GetArrayParamOccurancesRelatedNm(WOperator, Gp.RelatedParmId, Gp.RelatedOccuranceId);
            comboBoxModel.DisplayMember = "DisplayValue";
        }
// Add Dir
        private void buttonAddAtmDir_Click(object sender, EventArgs e)
        {
            
            Ad.SWDCategoryId = Sc.SWDCategoryId;
            Ad.AtmDirectory = textBoxAtmDir.Text;
            Ad.Purpose = textBoxPurpose.Text;
            Ad.Operator = WOperator;

            Ad.InsertCatAtmDestDirectory();

            int WRowGrid1 = dataGridView1.SelectedRows[0].Index;
          
            Form502_Load(this, new EventArgs());

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGrid1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));
        }
// Update Dir
        private void buttonUpdateAtmDir_Click(object sender, EventArgs e)
        {
            Ad.SWDCategoryId = Sc.SWDCategoryId;
            Ad.AtmDirectory = textBoxAtmDir.Text;
            Ad.Purpose = textBoxPurpose.Text;
            Ad.Operator = WOperator;

            Ad.UpdateCatAtmDestDirectory(WOperator, Ad.SeqNo);

            int WRowGrid1 = dataGridView1.SelectedRows[0].Index;
            int WRowGrid2 = dataGridView2.SelectedRows[0].Index;

            Form502_Load(this, new EventArgs());

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGrid1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridView2.Rows[WRowGrid2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid2));

        }
// Delete Dir
        private void buttonDeleteAtmDir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this Directory ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                   == DialogResult.Yes)
            {
                Ad.DeleteCatAtmDestDirectory(Ad.SeqNo);

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
        }

    }
}

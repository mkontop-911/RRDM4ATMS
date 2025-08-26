using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form502_U_Str : Form
    {

        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();

        RRDMGasParameters Gp = new RRDMGasParameters();

        int WSeqNo;

        bool FirstCycle; 
     
        string WSelectionCriteria; 

        int WRowIndex; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
       
        public Form502_U_Str(string InSignedId, int SignRecordNo, string InOperator)
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

            FirstCycle = true; 

            //Field Type for File Fields 
            comboBoxFieldType.Items.Add("Character");
            comboBoxFieldType.Items.Add("Numeric");
            comboBoxFieldType.Items.Add("Decimal");
            comboBoxFieldType.Items.Add("Boolean");
            comboBoxFieldType.Items.Add("DateTime");
            comboBoxFieldType.Items.Add("Date");
            comboBoxFieldType.Items.Add("Time");
            comboBoxFieldType.Text = "Character";


            // File Id Structure
            Gp.ParamId = "917";
            comboBoxStructureId.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxStructureId.DisplayMember = "DisplayValue";

            comboBoxStructureId.Text = "Atms And Cards";

            // If Primary then application has to be defined 
            comboBoxApplication.Items.Add("From Own ATMs");
            comboBoxApplication.Items.Add("Not From Own ATMs");

            comboBoxApplication.Text = "From Own ATMs";


        }
        // Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            WSelectionCriteria = "Where TableStructureId ='" + comboBoxStructureId.Text +"'"; 

            Utd.ReadUniversalTableFieldsDefinitionToFillDataTable(WSelectionCriteria);

            dataGridViewFields.DataSource = Utd.DataTableUniversalTableFields.DefaultView;

            if (dataGridViewFields.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No Fields");
                MessageForm.ShowDialog();

                textBoxSequence.Text = "";
                textBoxLogical.Text = "";
                textBoxPhysical.Text = "";
                textBoxLength.Text = ""; 

                return;
            }

            // ELSE SHOW GRID
            if (comboBoxStructureId.Text == "Atms And Cards") ShowGridFields01();

        }

// Row Enter Right 
        private void dataGridViewFields_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFields.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Utd.ReadUniversalTableFieldsDefinitionSeqNo(WSeqNo);

            textBoxSequence.Text = Utd.SortSequence.ToString();

            textBoxLogical.Text = Utd.FieldName;
            textBoxPhysical.Text = Utd.FieldDBName;
            
            textBoxLength.Text = Utd.FieldLength.ToString();

            comboBoxFieldType.Text = Utd.FieldType;

            checkBoxMatchingField.Checked = Utd.IsMatchingField;
            checkBoxPrimary.Checked = Utd.IsPrimaryMatchingField;

            comboBoxStructureId.Text = Utd.TableStructureId; 

            if (checkBoxPrimary.Checked == true)
            {
                label4.Show(); 
                comboBoxApplication.Show();
                comboBoxApplication.Text = Utd.Application;
            }
            else
            {
                label4.Hide();
                comboBoxApplication.Hide();
                comboBoxApplication.Text = Utd.Application;
            }

        }

// ADD
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            //
            //  Validation

            //
            int InSortSequence;

            if (int.TryParse(textBoxSequence.Text, out InSortSequence))
            {
            }
            else
            {
                MessageBox.Show(textBoxSequence.Text, "Please enter a valid number in field Sort Sequence. ");
                return;
            }

            if (int.TryParse(textBoxLength.Text, out Utd.FieldLength))
            {
            }
            else
            {
                MessageBox.Show(textBoxLength.Text, "Please enter a valid number in field length. ");
                return;
            }

            WSelectionCriteria = " WHERE "                            
                                + " TableStructureId='" + comboBoxStructureId.Text + "'"
                                + " AND ( FieldDBName ='" + textBoxPhysical.Text + "'"
                                 + " OR FieldName ='" + textBoxLogical.Text + "')";

            Utd.ReadUniversalTableFieldsDefinitionBySelectionCriteria(WSelectionCriteria);

            if (Utd.RecordFound == true)
            {
                MessageBox.Show("Same Field Already exist");
                return;
            }

            WSelectionCriteria = " WHERE "
                                   + " TableStructureId='" + comboBoxStructureId.Text + "'"
                                   + " AND SortSequence =" + InSortSequence;

            Utd.ReadUniversalTableFieldsDefinitionBySelectionCriteria(WSelectionCriteria);

            if (Utd.RecordFound == true)
            {
                MessageBox.Show("Sort Sequence Already Exist");
                return;
            }

            Utd.SortSequence = InSortSequence; 

            //textBoxSequence.Text = Utd.SortSequence.ToString();

            Utd.FieldName =  textBoxLogical.Text ;
            Utd.FieldDBName = textBoxPhysical.Text ;

            Utd.FieldType = comboBoxFieldType.Text ;

            Utd.IsMatchingField = checkBoxMatchingField.Checked ;
            Utd.IsPrimaryMatchingField = checkBoxPrimary.Checked ;
            Utd.IsMergedField = checkBoxMergedField.Checked;

            Utd.TableStructureId = comboBoxStructureId.Text ;

            if (checkBoxPrimary.Checked == true)
            {
                Utd.Application = comboBoxApplication.Text ;
            }
            else
            {
                Utd.Application = "" ;
            }

         

            int RecordId = Utd.InsertUniversalTableFieldsDefinitionRecord(); 

            Form502_Load(this, new EventArgs());

            Utd.ReadUniversalTableFieldsDefinitionToFindPossition(RecordId, Utd.TableStructureId);

            WRowIndex = Utd.PositionCount ; 

            dataGridViewFields.Rows[WRowIndex].Selected = true;
            dataGridViewFields_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
// UPDATE 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            //WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;
            WRowIndex = dataGridViewFields.SelectedRows[0].Index;

            //
            //  Validation
            //

            int InSortSequence; 

            if (int.TryParse(textBoxSequence.Text, out InSortSequence))
            {
            }
            else
            {
                MessageBox.Show(textBoxSequence.Text, "Please enter a valid number in field Sort Sequence. ");
                return;
            }

            if (int.TryParse(textBoxLength.Text, out Utd.FieldLength))
            {
            }
            else
            {
                MessageBox.Show(textBoxLength.Text, "Please enter a valid number in field length. ");
                return;
            }

            if (Utd.FieldDBName != textBoxPhysical.Text)
            {
                MessageBox.Show("You cannot Change DataBase Field Name");
                return;
            }

            //WSelectionCriteria = " WHERE "
            //                       + " TableStructureId='" + comboBoxStructureId.Text + "'"
            //                       + " AND FieldDBName ='" + textBoxPhysical.Text + "'"
            //                       + " AND SortSequence =" + InSortSequence
            //                       + " AND FieldDBName ='" + checkBoxMatchingField.Checked + "'";

            //Utd.ReadUniversalTableFieldsDefinitionBySelectionCriteria(WSelectionCriteria);

            //if (Utd.RecordFound == true)
            //{
            //    if (Utd.FieldDBName == textBoxPhysical.Text )
            //    {
            //        MessageBox.Show("Same Field Already exist");
            //        return;
            //    }       
            //}

            if (InSortSequence != Utd.SortSequence)
            {
                WSelectionCriteria = " WHERE "
                                    + " TableStructureId='" + comboBoxStructureId.Text + "'"
                                    + " AND SortSequence =" + InSortSequence;
                Utd.ReadUniversalTableFieldsDefinitionBySelectionCriteria(WSelectionCriteria);

                if (Utd.RecordFound == true)
                {
                    MessageBox.Show("Sort Sequence Already Exist");
                    return;
                }
            }

            Utd.SortSequence = InSortSequence; 

            //textBoxSequence.Text = Utd.SortSequence.ToString();

            Utd.FieldName = textBoxLogical.Text;
            Utd.FieldDBName = textBoxPhysical.Text;

            Utd.FieldType = comboBoxFieldType.Text;

            Utd.IsMatchingField = checkBoxMatchingField.Checked;
            Utd.IsPrimaryMatchingField = checkBoxPrimary.Checked;
            Utd.IsMergedField = checkBoxMergedField.Checked;

            Utd.TableStructureId = comboBoxStructureId.Text;

            if (checkBoxPrimary.Checked == true)
            {
                Utd.Application = comboBoxApplication.Text;
            }
            else
            {
                Utd.Application = "";
            }

            //Utd.Operator = WOperator;

            Utd.UpdateUniversalTableFieldsDefinitionRecord(WSeqNo); 

            Form502_Load(this, new EventArgs());

            dataGridViewFields.Rows[WRowIndex].Selected = true;
            dataGridViewFields_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
// DELETE 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridViewFields.SelectedRows[0].Index;

            if (MessageBox.Show("Warning: Do you want to delete this field ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Utd.DeleteUniversalTableFieldsDefinitionRecord(WSeqNo); 

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
            if (WRowIndex-1>=0)
            {
                dataGridViewFields.Rows[WRowIndex-1].Selected = true;
                dataGridViewFields_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex-1));
            }
         
        }


        // Show Grid Left 
        private void ShowGridFields01()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

//            SeqNo  int Unchecked
//SortSequence    int Unchecked
//TableSructureId nvarchar(50)    Unchecked
//FieldName   nvarchar(100)   Unchecked
//FieldDBName nvarchar(100)   Unchecked
//FieldType   nvarchar(100)   Unchecked
//FieldLength int Unchecked
//IsMatchingField bit Unchecked
//IsPrimaryMatchingField  bit Unchecked
//Operator    nvarchar(8) Unchecked

            dataGridViewFields.Columns[0].Width = 50; // "SeqNo"
            dataGridViewFields.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewFields.Columns[1].Width = 60; // "SortSequence"
            dataGridViewFields.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFields.Columns[1].HeaderText = "Sort Sequence";

            dataGridViewFields.Columns[2].Width = 100; // TableSructureId
            dataGridViewFields.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
          

            dataGridViewFields.Columns[3].Width = 190; //  FieldName
            dataGridViewFields.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewFields.Columns[3].HeaderText = "Field Name";

            dataGridViewFields.Columns[4].Width = 100; // FieldDBName
            dataGridViewFields.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridViewFields.Columns[5].Width = 80; // FieldType
            dataGridViewFields.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewFields.Columns[6].Width = 50; // FieldLength
            dataGridViewFields.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFields.Columns[6].HeaderText = "Length";
            //dataGridViewFields.Sort(dataGridViewFields.Columns[4], ListSortDirection.Ascending);

            dataGridViewFields.Columns[7].Width = 60; // IsMatchingField
            dataGridViewFields.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFields.Columns[7].HeaderText = "For Matching";

            dataGridViewFields.Columns[8].Width = 60; // IsPrimaryMatchingField
            dataGridViewFields.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFields.Columns[8].HeaderText = "Primary";

            dataGridViewFields.Columns[9].Width = 90; // Application
            dataGridViewFields.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridViewFields.Columns[10].Width = 90; // Operator
            //dataGridViewFields.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridViewFields.Columns[10].Visible = false;
            //dataGridViewFields.Columns[14].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }
        // FINISH
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

// If primary key 
        private void checkBoxPrimary_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPrimary.Checked == true)
            {
                label4.Show();
                comboBoxApplication.Show(); 
            }
            if (checkBoxPrimary.Checked == false )
            {
                label4.Hide();
                comboBoxApplication.Hide();
            }
        }
// Load if not first time
        private void comboBoxStructureId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FirstCycle == false) Form502_Load(this, new EventArgs());
            else FirstCycle = false;
        }
    }
}

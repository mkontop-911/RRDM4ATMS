using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos
using System.Configuration;
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form502 : Form
    {
       
        RRDMReconcSourceFiles Rs = new RRDMReconcSourceFiles();
        RRDMReconcFileFieldsFromBankToRRDM Rff = new RRDMReconcFileFieldsFromBankToRRDM();
        RRDMReconcMatchingFields Rmf = new RRDMReconcMatchingFields(); 

        ////int WRowIndex;
        string WSourceFileName;
        //string WReconcCategory;
        int WSeqNoLeft;
        int WSeqNoRight;

        int WRowIndexLeft;
        int WRowIndexRight; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
       
        public Form502(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
          
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            labelUserId.Text = InSignedId;

            comboBoxFieldType.Items.Add("Character");
            comboBoxFieldType.Items.Add("Numeric");
            comboBoxFieldType.Items.Add("Date");
            comboBoxFieldType.Items.Add("Time");
            comboBoxFieldType.Text = "Character";
        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            Rs.ReadReconcSourceFilesToFillDataTable(WOperator);

            dataGridViewFiles.DataSource = Rs.DataTableSourceFiles.DefaultView;

            dataGridViewFiles.Sort(dataGridViewFiles.Columns[0], ListSortDirection.Ascending);

            dataGridViewFiles.Columns[0].Width = 60; // Id
            dataGridViewFiles.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewFiles.Columns[1].Width = 170; // Id
            dataGridViewFiles.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewFiles.Columns[2].Width = 70; // Name

        }
// Row Enter Left
        private void dataGridViewFiles_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFiles.Rows[e.RowIndex];

            WSeqNoLeft = (int)rowSelected.Cells[0].Value;
            WSourceFileName = rowSelected.Cells[1].Value.ToString();

            labelFieldsDefinition.Text = "FIELDS DEFINITION FOR FILE : " + WSourceFileName ; 

            Rs.ReadReconcSourceFilesSeqNo(WSeqNoLeft);
            if (Rs.Enabled == true) checkBoxEnable.Checked = true; 
            else checkBoxEnable.Checked = false; 
            textBox5.Text = Rs.SystemOfOrigin;
            textBox1.Text = Rs.SourceFileId;
            textBox2.Text = Rs.SourceDirectory;
            textBox3.Text = Rs.FileNameMask;
            textBox6.Text = Rs.ArchiveDirectory;
            textBox11.Text = Rs.LayoutId;
       
            Rff.ReadFileFields(WSourceFileName);

            dataGridViewFields.DataSource = Rff.DataTableFileFields.DefaultView;

            if (dataGridViewFields.Rows.Count == 0)
            {
                // Create records
                Rmf.ReadMatchingFieldsToCreateBankToRRDMRecords(WOperator, WSourceFileName);

                // Read Again for updating 
                Rff.ReadFileFields(WSourceFileName);

                dataGridViewFields.DataSource = Rff.DataTableFileFields.DefaultView;
            }
            else
            {
               
            }


            dataGridViewFields.Columns[0].Width = 60; // 
            dataGridViewFields.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewFields.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridViewFields.Columns[1].Width = 120; // 
          

            dataGridViewFields.Columns[2].Width = 120; //      
            dataGridViewFields.Columns[3].Width = 60; //      
            dataGridViewFields.Columns[4].Width = 60; //      

        }
// Row Enter Right 
        private void dataGridViewFields_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewFields.Rows[e.RowIndex];

            WSeqNoRight = (int)rowSelected.Cells[0].Value;

            Rff.ReadReadFileFieldsbySeqNo(WSeqNoRight);
       
            textBox4.Text = Rff.SourceFieldNm;
            textBox7.Text = Rff.TargetFieldNm;
            textBox9.Text = Rff.SourceFieldPositionStart.ToString();
            textBox10.Text = Rff.SourceFieldPositionEnd.ToString();

            comboBoxFieldType.Text = Rff.SourceFieldType;     

            if (Rff.RoutineValidation == true)
            {
                checkBox1.Checked = true;
                textBox8.Text = Rff.RoutineNm;
            }
            else checkBox1.Checked = false; 

        }
// ADD Left
        private void buttonAddLeft_Click(object sender, EventArgs e)
        {
            Rs.Enabled = checkBoxEnable.Checked; 
            Rs.SystemOfOrigin = textBox5.Text ;
            Rs.SourceFileId = textBox1.Text ;

            Rs.ReadReconcSourceFilesByFileId(Rs.SourceFileId);
            if (Rs.RecordFound == true)
            {
                MessageBox.Show("File Id Already exist");
                return; 
            }
            Rs.SourceDirectory = textBox2.Text ;
            Rs.FileNameMask = textBox3.Text ;
            Rs.ArchiveDirectory = textBox6.Text ;
            Rs.LayoutId = textBox11.Text; 

            Rs.InsertReconcSourceFileRecord();

            Form502_Load(this, new EventArgs());


        }
// Update Left
        private void buttonUpdateLeft_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;

            Rs.ReadReconcSourceFilesSeqNo(WSeqNoLeft);

            Rs.Enabled = checkBoxEnable.Checked; 

            Rs.SystemOfOrigin = textBox5.Text;
            Rs.SourceFileId = textBox1.Text;
            Rs.SourceDirectory = textBox2.Text;
            Rs.FileNameMask = textBox3.Text;
            Rs.ArchiveDirectory = textBox6.Text;
            Rs.LayoutId = textBox11.Text;

            Rs.UpdateReconcSourceFileRecord(WSeqNoLeft);

            Form502_Load(this, new EventArgs());

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));
        }
// Delete Left 
        private void buttonDeleteLeft_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this file ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {
                Rs.DeleteFileFieldRecord(WSeqNoLeft); 

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
           
        }
// Add Right 
        private void buttonAddRight_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;
            //
            //  Validation
            //
            if (int.TryParse(textBox9.Text, out Rff.SourceFieldPositionStart))
            {
            }
            else
            {
                MessageBox.Show(textBox9.Text, "Please enter a valid number in field Start. ");
                return;
            }

            if (int.TryParse(textBox10.Text, out Rff.SourceFieldPositionEnd))
            {
            }
            else
            {
                MessageBox.Show(textBox10.Text, "Please enter a valid number in field End. ");
                return;
            }

            Rff.SourceFileId = WSourceFileName;
            Rff.SourceFieldNm = textBox4.Text;
            Rff.TargetFieldNm = textBox7.Text;

            Rff.ReadFileFieldsByTargetField(WSourceFileName, Rff.SourceFieldNm, Rff.SourceFieldPositionStart);
            if (Rff.RecordFound == true)
            {
                MessageBox.Show("Same SourceField Already exist");
                return;
            }

            
            Rff.SourceFieldType = comboBoxFieldType.Text;

            Rff.RoutineValidation = checkBox1.Checked;
            if (checkBox1.Checked == true) Rff.RoutineNm = textBox8.Text; 

            Rff.InsertFileFieldRecord();

            Form502_Load(this, new EventArgs());

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

        }
// Update Right 
        private void buttonUpdateRight_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;
            WRowIndexRight = dataGridViewFields.SelectedRows[0].Index;

            //
            //  Validation
            //
            if (int.TryParse(textBox9.Text, out Rff.SourceFieldPositionStart))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid number in field Start. ");
                return;
            }

            if (int.TryParse(textBox10.Text, out Rff.SourceFieldPositionEnd))
            {
            }
            else
            {
                MessageBox.Show(textBox10.Text, "Please enter a valid number in field End. ");
                return;
            }

            //if (Rff.RoutineValidation == true)
            //{
            //    checkBox1.Checked = true;
            //    textBox8.Text = Rff.RoutineNm;
            //}
            //else checkBox1.Checked = false;

            Rff.SourceFileId = WSourceFileName;
            Rff.SourceFieldNm = textBox4.Text;
            Rff.TargetFieldNm = textBox7.Text;

            Rff.SourceFieldType = comboBoxFieldType.Text;

            Rff.RoutineValidation = checkBox1.Checked;
            if (checkBox1.Checked == true) Rff.RoutineNm = textBox8.Text; 
            Rff.UpdateFileFieldRecord(WSeqNoRight);

            Form502_Load(this, new EventArgs());

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewFields.Rows[WRowIndexRight].Selected = true;
            dataGridViewFields_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));


        }
// Delete Right 
        private void buttonDeleteRight_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewFiles.SelectedRows[0].Index;

            if (MessageBox.Show("Warning: Do you want to delete this field ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Rff.DeleteFileFieldRecord(WSeqNoRight);

                Form502_Load(this, new EventArgs());
            }
            else
            {
                return;
            }

            dataGridViewFiles.Rows[WRowIndexLeft].Selected = true;
            dataGridViewFiles_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));
            
        }
// Show Routine Name 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                label16.Show();
                textBox8.Show(); 
            }
            else
            {
                label16.Hide();
                textBox8.Hide(); 
            }

        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

    }
}

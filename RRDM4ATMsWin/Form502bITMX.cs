using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing; 
using RRDM4ATMs;

// 

namespace RRDM4ATMsWin
{
    public partial class Form502bITMX : Form
    {
        RRDMMatchingCategories Rc = new RRDMMatchingCategories();
        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

        RRDMMatchingMasksVsMetaExceptions Rme = new RRDMMatchingMasksVsMetaExceptions();

        RRDMErrorsORExceptionsCharacteristics Ec = new RRDMErrorsORExceptionsCharacteristics();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions(); 

        int WRowIndex;
       
        string WSubString;

        string WMask; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;

        public Form502bITMX(string InSignedId, int SignRecordNo, string InOperator, string InCategoryId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCategoryId = InCategoryId;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;

            Rcs.ReadReconcCategoriesByCategoryIdForName(WCategoryId); 

            labelRMCategory.Text = "CATEGORY : " + Rcs.CategoryName; 

        }

        private void Form502b_Load(object sender, EventArgs e)
        {

            // TODO: This line of code loads data into the 'aTMSDataSet66.ReconcCategoryVsSourceFiles' table. You can move, or remove it, as needed.
            // Selected Files (TWO)


            // LOAD MAsks 
            
            // Create records if any new ones 
            Ec.CreateMaskVsExceptionsRecords(WOperator, WCategoryId);

            Rme.ReadMatchingMasksToFillDataTable(WOperator, WCategoryId);

            dataGridView1.DataSource = Rme.DataTableMasks.DefaultView;


            //// DATA TABLE ROWS DEFINITION 
            //DataTableMasks.Columns.Add("SeqNo", typeof(int));
            //DataTableMasks.Columns.Add("MaskId", typeof(string));
            //DataTableMasks.Columns.Add("MaskName", typeof(string));
            //DataTableMasks.Columns.Add("MetaExceptionId", typeof(string));

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 60; // Error Id 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; // Mask Id  
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 210; // Mask Name 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 80; // Exception Id  
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }
       


        // ON ROW ENTER FOR MASKS 
        int WSeqNoMask;
        

        // ON ROW ENTER FOR exceptions

        private void dataGridView2_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNoMask = (int)rowSelected.Cells[0].Value;
            Rme.ReadMatchingMaskBySeqNo(WSeqNoMask);

            textBoxSeqNo.Text = WSeqNoMask.ToString(); 

            textBox1.Text = Rme.MaskId;

            if (Rme.MaskId == "")
            {
                label10.Text = "Fill in Mask or delete record";
                label10.Show();
            }
            else
            {
                label10.Hide();
            }

            textBox2.Text = Rme.MaskName;

            textBox3.Text = Rme.MetaExceptionId.ToString();

            //Define Files 
            RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions(); 
           
            Rms.ReadMatchingCategoriesSessionsByCatAndRunningJobNo(WOperator,Rme.CategoryId ,202);

            if (Rme.MaskId == "" || Rme.MaskId.Length !=6)
            {
                Rme.MaskId = "EEEEEE";
            }


            WMask = Rme.MaskId.Substring(0,3); 

            // First Line
            if (Rms.FileId11 != "")
            {
                labelFileA.Show();
                textBox31.Show();

                labelFileA.Text = "File A : " + Rms.FileId11;
                labelFileA.Show();
                WSubString = WMask.Substring(0, 1);
                if (WSubString == "0")
                {
                    textBox31.BackColor = Color.Red;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = "E";
                }
            }
            else
            {
                labelFileA.Hide();
                textBox31.Hide();
            }

            // Second Line 
            if (Rms.FileId21 != "")
            {
                labelFileB.Show();
                textBox32.Show();

                labelFileB.Text = "File B : " + Rms.FileId21;
                labelFileB.Show();
                WSubString = WMask.Substring(1, 1);
                if (WSubString == "0")
                {
                    textBox32.BackColor = Color.Red;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = "E";
                }
            }
            else
            {
                labelFileB.Hide();
                textBox32.Hide();
            }

            // Third Line 
            //
            if (Rms.FileId31 != "")
            {
                labelFileC.Show();
                textBox33.Show();

                labelFileC.Text = "File C : " + Rms.FileId31;
                labelFileC.Show();
                WSubString = WMask.Substring(2, 1);
                if (WSubString == "0")
                {
                    textBox33.BackColor = Color.Red;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = "E";
                }
            }
            else
            {
                labelFileC.Hide();
                textBox33.Hide();
            }
            //****************************************************************************
            //Credit Leg 
            //****************************************************************************

            Rms.ReadMatchingCategoriesSessionsByCatAndRunningJobNo(WOperator,Rme.CategoryId ,202);

            WMask = Rme.MaskId.Substring(3, 3);

            // Forth Line 
            if (Rms.FileId11 != "")
            {
                labelFileD.Show();
                textBox34.Show();

                labelFileD.Text = "File D : " + Rms.FileId11;
                labelFileD.Show();
                WSubString = WMask.Substring(0, 1);
                if (WSubString == "0")
                {
                    textBox34.BackColor = Color.Red;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = "E";
                }
            }
            else
            {
                labelFileD.Hide();
                textBox34.Hide();
            }

            // Fifth Line 
            if (Rms.FileId21 != "")
            {
                labelFileE.Show();
                textBox35.Show();

                labelFileE.Text = "File E : " + Rms.FileId21;
                labelFileE.Show();
                WSubString = WMask.Substring(1, 1);
                if (WSubString == "0")
                {
                    textBox35.BackColor = Color.Red;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = "E";
                }
            }
            else
            {
                labelFileE.Hide();
                textBox35.Hide();
            }
            // sixth Line 
            if (Rms.FileId31 != "")
            {
                labelFileF.Show();
                textBox36.Show();

                labelFileF.Text = "File F : " + Rms.FileId31;
                labelFileF.Show();
                WSubString = WMask.Substring(2, 1);
                if (WSubString == "0")
                {
                    textBox36.BackColor = Color.Red;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = ">";
                }
                if (WSubString == "E")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = "E";
                }
            }
            else
            {
                labelFileF.Hide();
                textBox36.Hide();
            }
        }


// ADD MAsk 
        private void buttonAdd_Click(object sender, EventArgs e)
        {

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            if (textBox1.TextLength != 6)
            {
                MessageBox.Show("Please enter the correct length for mask. ");

                return;
            }

            Rme.MaskId = textBox1.Text ;

            Rme.ReadMatchingMaskRecordbyMaskId(WOperator, WCategoryId, Rme.MaskId, 11);

            if (Rme.RecordFound == true)
            {
                MessageBox.Show("This Mask already exist. Please correct your action. ");

                return;
            }

            // Meta exception no
             if (int.TryParse(textBox3.Text, out Rme.MetaExceptionId))
            {
                Ec.ReadErrorsIDRecord(Rme.MetaExceptionId, WOperator);
                 if (Ec.RecordFound == true)
                 {
                     if (textBox2.Text.Length >0)
                     {
                         MessageBox.Show("You Have insert Mask Name. " + Environment.NewLine
                             + "It will be overwritten by the metaexception description." + Environment.NewLine
                             + "You will be able to change it with the Update option ");
                     }
                     textBox2.Text = Ec.ErrDesc; 
                 }
                 else
                 {
                     MessageBox.Show("Not Such Meta Id");
                     return; 
                 }
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number!");

                return;
            }         

            Rme.MaskName = textBox2.Text ;

            Rme.InsertMatchingCategoryMaskRecord();

            Form502b_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView2_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndex));

        }
// UPDATE MASK RECORD 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
          
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Rme.MaskId = textBox1.Text;

            if (int.TryParse(textBox3.Text, out Rme.MetaExceptionId))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number!");

                return;
            }

            Rme.MaskName = textBox2.Text;

            Rme.ReadMatchingMaskRecordbyMaskId(WOperator, WCategoryId, Rme.MaskId, 11);

            if (Rme.RecordFound == true)
            {
                MessageBox.Show("This Mask already exist. Please correct your action. ");

                return;
            }

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Rme.UpdateMatchingMaskRecord(WSeqNoMask);

            Form502b_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView2_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndex));
            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

        }
// DElete Mask Record 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
          
            if (MessageBox.Show("Warning: Do you want to delete this row ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Rme.DeleteMaskRecord(WSeqNoMask);
            }
            else
            {
                return;
            }

            Form502b_Load(this, new EventArgs());

        }
// Show Options of Meta Ids
        private void buttonShowMetaIds_Click(object sender, EventArgs e)
        {
            Ec.ReadErrorsIDRecordsInTableDistict(WOperator);

            //dataGridView1.DataSource = Ec.ExceptionsCharacteristicsTable.DefaultView;
            Form78b NForm78b;

            string WHeader = "Meta Exceptions Ids " ;

            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Ec.ExceptionsCharacteristicsTable, WHeader, "Form502b");
            //NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();

        }
// FINISH
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

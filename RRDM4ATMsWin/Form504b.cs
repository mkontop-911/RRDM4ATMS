using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form504b : Form
    {
       
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMMatchingCategoriesVsBINs Mcb = new RRDMMatchingCategoriesVsBINs(); 
        RRDMGasParameters Gp = new RRDMGasParameters();
     
        //string WSelectionCriteria;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WCategoryId;

        int WRowIndex;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WMode;

        public Form504b(string InSignedId, int SignRecordNo, string InOperator, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WMode = InMode;
            // InMode = 1 means is ATMs
            // InMode = 3 means is ITMX
            // InMode = 5 means is NOSTRO 

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
            labelUserId.Text = WSignedId;

            Gp.ParamId = "707"; // fILTER 
            comboBoxFilter.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxFilter.DisplayMember = "DisplayValue";

            textBoxMsgBoard.Text = "Definition of Matching Categories Vs BINs";

            Mc.ReadMatchingCategorybyGetsAllOtherBINS(WOperator); 
            if (Mc.RecordFound == true)
            {
                labelAllOtherBINS.Text = "All BINS which are not our Own goes to Category : " + Mc.CategoryId; 
            }
            else
            {
                labelAllOtherBINS.Text = "NO Category is defined to accomodate the BINs of other Banks " ;
            }

        }
        // Load 
        private void Form504_Load(object sender, EventArgs e)
        {
            // SHOW ALL OF THIS comboBoxFilter

            // SHOW ALL OF THIS comboBoxFilter
            Mc.ReadMatchingCategoriesAndFillTable(WOperator, comboBoxFilter.Text);

            dataGridView1.DataSource = Mc.TableMatchingCateg.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //label12.Hide();
                //textBoxOwner.Hide(); 
                //buttonChangeOwner.Hide();
                return;
            }

            dataGridView1.Columns[0].Width = 60; // Seq No
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            //dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

            dataGridView1.Columns[1].Width = 60; // Category Id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 300; // Name
        }

        // On Row Enter 
        bool InternalChange; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WCategoryId = (string)rowSelected.Cells[1].Value;

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId); 

            Mcb.ReadReconcMatchingFieldsToFillDataTableByCategory(WOperator, Mc.Origin, WCategoryId);

            dataGridView2.DataSource = Mcb.TableMatchingCategoriesVsBINs.DefaultView;

            textBoxCategoryId.Text = WCategoryId ;

            //if (checkBoxMakeNewCategory.Checked == true)
            //{
            //    InternalChange = true;
            //    checkBoxMakeNewCategory.Checked = false;
            //}
            //else
            //{
            //    InternalChange = false;
            //}

            if (dataGridView2.Rows.Count == 0)
            {
                textBoxBIN.Text = "";
                textBoxBINDescription.Text = "";
                buttonUpdate.Hide();
                buttonDelete.Hide();
                buttonAdd.Enabled = true;
                buttonAdd.Show();
                return;
            }

            ShowGrid2();      

        }

        // On Row Enter for Grid 2 
        int WSeqNoRight; 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WSeqNoRight = (int)rowSelected.Cells[0].Value;
            //WSelectionCriteria = " WHERE SeqNo =" + WSeqNoRight; 
            Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, WSeqNoRight, "", "" ,11);

            textBoxBIN.Text = Mcb.BIN;

            textBoxBINDescription.Text = Mcb.BIN_Description;

            buttonUpdate.Show();
            buttonDelete.Show();
            buttonAdd.Hide();
            buttonRefresh.Hide();

            //if (checkBoxMakeNewCategory.Checked == true)
            //{
            //    InternalChange = true;
            //    checkBoxMakeNewCategory.Checked = false;
            //    buttonUpdate.Show();
            //    buttonDelete.Show();
            //    buttonAdd.Hide();
            //}
            //else
            //{
            //    InternalChange = false;

            //    buttonUpdate.Show();
            //    buttonDelete.Show();
            //    buttonAdd.Hide();
            //}

        }
        // ADD
   
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Validation
            if (textBoxBIN.TextLength <6 || textBoxBINDescription.Text == "")
            {
                MessageBox.Show("Please fill in complete data");
                return; 
            }
            //WSelectionCriteria = " WHERE BIN ='" + textBoxBIN.Text + "'";
            Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, WSeqNoRight, textBoxBIN.Text, Mc.Origin ,12);
            if (Mcb.RecordFound)
            {
                MessageBox.Show("This BIN exists with the Origin in Matching Category : " + Mcb.CategoryId);
                return; 
            }

            Mcb.CategoryId = textBoxCategoryId.Text;
            Mcb.BIN = textBoxBIN.Text;
            Mcb.BIN_Description = textBoxBINDescription.Text;

            Mcb.Origin = Mc.Origin;

            Mcb.Operator = Mc.Operator; 

            // INSERT Record 

            Mcb.InsertMatchingCategoryVsBIN();

            MessageBox.Show("New BIN inserted. Press Refresh to view");

            buttonRefresh.Show(); 

            textBoxMsgBoard.Text = "New BIN inserted. Press Refresh to view";

            //Form504_Load(this, new EventArgs());

            //Rc.ReadCategoriesToFindPositionOfSeqNo(WOperator, WSeqNo, "");


        }

        // UPDATE CATEGORY 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
           
            //WSelectionCriteria = " WHERE SeqNo =" + WSeqNoRight;
            Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, WSeqNoRight, "", "" ,11);

            if (textBoxBIN.TextLength < 6 || textBoxBINDescription.Text == "")
            {
                MessageBox.Show("Please fill in complete data");
                return;
            }
            //WSelectionCriteria = " WHERE BIN ='" + textBoxBIN.Text + "'";
            Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, WSeqNoRight, textBoxBIN.Text, Mc.Origin, 12);
            if (Mcb.RecordFound & Mcb.CategoryId != textBoxCategoryId.Text)
            {
                MessageBox.Show("This BIN exists in Matching Category :" + Mcb.CategoryId);
                return;
            }

            Mcb.CategoryId = textBoxCategoryId.Text;
            Mcb.BIN = textBoxBIN.Text;
            Mcb.BIN_Description = textBoxBINDescription.Text;
            //
            // Update Category 
            //
            Mcb.UpdateMatchingCategoryVsBIN(WOperator, WSeqNoRight); 
           
            MessageBox.Show("Updating Done!");

            textBoxMsgBoard.Text = "Matching Category Vs BIN updated.";

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form504_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // DELETE 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                // Delete Category VS BIN
                Mcb.DeleteMatchingCategoryVsBIN(WSeqNoRight);

                // Initiliase textBoxes
                textBoxBIN.Text = "";
                textBoxBINDescription.Text = ""; 

                int WRowIndex1 = dataGridView1.SelectedRows[0].Index;

                Form504_Load(this, new EventArgs());

                if (WRowIndex1 > 0)
                {
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }
            }
            else
            {
                return;
            }
        }

    

        // Change Filter 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WMode == 5)
            {
                return;
            }
            else Form504_Load(this, new EventArgs());
        }
       
        private void button52_Click(object sender, EventArgs e)
        {
            FormHelp helpForm = new FormHelp("Matching Categories Definition");
            helpForm.ShowDialog();
        }

        private void checkBoxMakeNewCategory_CheckedChanged(object sender, EventArgs e)
        {
            if (InternalChange == true)
            {
                InternalChange = false;
                return; 
            }
         
            if (checkBoxMakeNewEntry.Checked == true)
            {
                //buttonAdd.Enabled = true;
                buttonAdd.Show();
                buttonUpdate.Hide();
                buttonDelete.Hide();

                textBoxBIN.Text = "";
                textBoxBINDescription.Text = "";

                InternalChange = true; 

                checkBoxMakeNewEntry.Checked = false;       

            }
            else
            {
                buttonAdd.Hide();
                buttonUpdate.Show();
                buttonDelete.Show();
                buttonRefresh.Hide();

                //InternalChange = false;

                ////dataGridView2.Enabled = true;
                //int WRowIndex1 = -1;

                //if (dataGridView1.Rows.Count > 0)
                //{
                //    WRowIndex1 = dataGridView1.SelectedRows[0].Index;
                //}

                Form504_Load(this, new EventArgs());

                //if (dataGridView1.Rows.Count > 0)
                //{
                //    dataGridView1.Rows[WRowIndex1].Selected = true;
                //    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                //}

            }
        }
       
        // Show Grid2 
        private void ShowGrid2()
        {
          
            dataGridView2.Columns[0].Width = 60; // Seq No
            //dataGridView2.Columns[0].Name = "SeqNo";
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
          
            dataGridView2.Columns[1].Width = 80; // Category Id
            //dataGridView2.Columns[1].Name = "Select";
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 80; // BIN
            //dataGridView2.Columns[2].Name = "CategoryId";
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 260; // Description
        }

        // Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

// REFRESH 
      

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form504_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
    }
}

using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form504 : Form
    {
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGroups Gr = new RRDMGroups();
        RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();

        string SelectionCriteria;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WRecCategoryId;
        string WMainCateg;

        bool Entries;

        int WSeqNo;

        int WGroupId;

        int WGroupId_2;

        string WOrigin; 

        int WRowIndex;

        bool Is_Maker;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string W_Application; 
        int WMode;
        string WForWhatUser; 

        public Form504(string InSignedId, int SignRecordNo, string InOperator,string In_Application, int InMode, string InForWhatUser)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            W_Application = In_Application; 
            WMode = InMode;
            WForWhatUser = InForWhatUser; 
            // InMode = 1 means is ATMs
            // InMode = 3 means is ITMX
            // InMode = 5 means is NOSTRO 
            // InMode = 7 means is e_MOBILE

            InitializeComponent();

            if (WForWhatUser == "")
            {
                // Means comes from Form13_NEW and it is from Maker
                Is_Maker = false; 
            }
            else
            {
                Is_Maker = true;

                labelStep1.Text = "Reconciliation Categories Definition For.."+ WForWhatUser; 
            }

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

            //Gp.ParamId = "707"; // fILTER 
            //comboBoxFilter.DataSource = Gp.GetParamOccurancesNm(WOperator);
            //comboBoxFilter.DisplayMember = "DisplayValue";

            Gp.ParamId = "707"; // Origin  
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox2.DisplayMember = "DisplayValue";
            // Groups
            comboBoxGroups.DataSource = Gr.GetGroupNosList(WOperator);
            comboBoxGroups.DisplayMember = "DisplayValue";

            textBoxMsgBoard.Text = "Definition of Reconciliation Categories and Owners";

            //comboBoxFilter.Text = "ALL"; 

        }
        // Load 
        private void Form504_Load(object sender, EventArgs e)
        {
            // SHOW ALL OF THIS comboBoxFilter
            if (WMode==1)
            {
                Rc.ReadReconcCategoriesAndFillTable(WOperator, "ATMS/CARDs"); 
            }
            if (WMode == 5)
            {
                Rc.ReadReconcCategoriesAndFillTable(WOperator, "Nostro Reconc"); 
            }

            if (WMode == 7)
            {
                Rc.ReadReconcCategoriesAndFillTable(WOperator, W_Application); // ALL means all for ATM and not e_MOBILE or Nostro
            }


            ShowGrid1();
        }

        // On Row Enter 
        bool InternalChange; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Rc.ReadReconcCategoriesbySeqNo(WOperator, WSeqNo);

            if (checkBoxMakeNewCategory.Checked == true)
            {
                InternalChange = true;
                checkBoxMakeNewCategory.Checked = false;
            }
            else
            {
                InternalChange = false;
            }

            WRecCategoryId = Rc.CategoryId;

            WMainCateg = WRecCategoryId.Substring(0, 4);

            //WMainCategITMX = WCategoryId.Substring(0, 5);

            comboBox2.Text = Rc.Origin;

            WOrigin = Rc.Origin; 

            if (WOrigin == "Our Atms" || WOrigin == "ALPHA ATMs")
            {
                buttonUpdateHisATMs.Show(); 
            }
            else
            {
                buttonUpdateHisATMs.Hide();
            }

            comboBoxGroups.Text = Rc.AtmGroup.ToString();

            WGroupId_2 = Rc.AtmGroup; 

            textBoxCategoryId.Text = Rc.CategoryId;

            textBoxCategDescr.Text = Rc.CategoryName;
            if (Rc.IsOneMatchingCateg == false)
            {
                //SelectionCriteria = " WHERE ReconcMaster = 0";
                Mc.ReadMatchingCategoriesSlavesAndFillTable(WOperator, Rc.CategoryId);

                label5.Show();
                dataGridView2.Show();

                buttonUpdate.Show();
                buttonDelete.Show();
                buttonAdd.Hide();

                ShowGrid2();
            }
            else
            {
                label5.Hide();
                dataGridView2.Hide();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                buttonAdd.Hide();
            }

            if (Rc.HasOwner == true)
            {
                Us.ReadUsersRecord(Rc.OwnerUserID);

                textBoxOwner.Text = Rc.OwnerUserID + " " + Us.UserName;
                buttonChangeOwner.Text = "Change Owner";
            }
            else
            {
                textBoxOwner.Text = "Category has no Owner.";
                buttonChangeOwner.Text = "Assign Owner";
            }
            // Dissable
            comboBox2.Enabled = false;
            comboBoxGroups.Enabled = false;
           
            if (checkBoxMakeNewCategory.Checked == true)
            {
                InternalChange = true; 
                checkBoxMakeNewCategory.Checked = false;
            }
            else
            {
                InternalChange = false;
            }
           
           
        }
        // ADD

        bool WSelect;
        string WMatchingCategoryId;
        bool DublicateMatchingCat;
        int TotalGridSelect;
        int K = 0; 

        DateTime CurrentDt = DateTime.Now;

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Rc.ReadReconcCategorybyCategId(WOperator, textBoxCategoryId.Text);
            if (Rc.RecordFound == true)
            {
                MessageBox.Show(textBoxCategoryId.Text, "This Category Already exist.  ");
                return;
            }

            DublicateMatchingCat = false;

            Rc.CategoryId = textBoxCategoryId.Text;


            Rc.CategoryName = textBoxCategDescr.Text;

            Rc.Origin = comboBox2.Text;

            if (int.TryParse(comboBoxGroups.Text, out Rc.AtmGroup))
            {
            }
            if (comboBox2.Text == "Our Atms")
            {
                Rc.IsOneMatchingCateg = false;
            }
            else
            {
                Rc.IsOneMatchingCateg = true; // ONLY ONE CATEGORY 
            }

            Rc.HasOwner = false;
            Rc.OwnerUserID = "";

            Rc.OpeningDateTm = DateTime.Now;

            Rc.Operator = WOperator;


            if (comboBox2.Text == "Our Atms")
            {
                //
                // VALIDATION 
                //
                DublicateMatchingCat = false;
                TotalGridSelect = 0;
                K = 0;

                while (K <= (dataGridView2.Rows.Count - 1))
                {
                    WSelect = (bool)dataGridView2.Rows[K].Cells["Select"].Value;
                    WMatchingCategoryId = (string)dataGridView2.Rows[K].Cells["CategoryId"].Value;

                    if (WSelect == true)
                    {
                        TotalGridSelect = TotalGridSelect + 1;

                        SelectionCriteria = " WHERE MatchingCategoryId ='" + WMatchingCategoryId + "'";
                        RcMc.ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(SelectionCriteria);
                        if (RcMc.RecordFound)
                        {
                            //MessageBox.Show("This Matching Category ALready belongs to :" + RcMc.ReconcCategoryId + " Correct and repeat");
                            //K = 200;
                            //DublicateMatchingCat = true;
                            //return;
                        }

                    }
                    K++; // Read Next entry of the table 
                }

                if (DublicateMatchingCat == true)
                {
                    return; // Finish without updating 
                }
                else
                {
                    if (TotalGridSelect == 0)
                    {
                        MessageBox.Show("Please make selection of Matching Category");
                        return;
                    }         
                }

                // Insert all in Grid 

                K = 0;

                while (K <= (dataGridView2.Rows.Count - 1))
                {
                    WSelect = (bool)dataGridView2.Rows[K].Cells["Select"].Value;
                    WMatchingCategoryId = (string)dataGridView2.Rows[K].Cells["CategoryId"].Value;

                    if (WSelect == true)
                    {
                        RcMc.ReconcCategoryId = Rc.CategoryId;
                        RcMc.GroupId = Rc.AtmGroup;
                        RcMc.MatchingCategoryId = WMatchingCategoryId;
                        RcMc.OpeningDateTm = CurrentDt;
                        RcMc.Operator = WOperator;
                        // INSERT
                        RcMc.InsertReconcCateqoriesVsMatchingCategories();

                    }

                    K++; // Read Next entry of the table 
                }
            }
            //

            // INSERT CATEGORY 

            WSeqNo = Rc.InsertReconcCategory();


            MessageBox.Show("Record with id " + Rc.CategoryId + " Inserted! Assign Owner.");

            textBoxMsgBoard.Text = "New Category inserted.";

            Rc.ReadCategoriesToFindPositionOfSeqNo(WOperator, WSeqNo, "");

            //WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form504_Load(this, new EventArgs());

            dataGridView1.Rows[Rc.PositionInGrid].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, Rc.PositionInGrid));

        }

        // UPDATE CATEGORY 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Rc.ReadReconcCategoriesbySeqNo(WOperator, WSeqNo);

            Rc.CategoryName = textBoxCategDescr.Text;

            Rc.Origin = comboBox2.Text;

            if (int.TryParse(comboBoxGroups.Text, out Rc.AtmGroup))
            {
            }

            if (comboBox2.Text == "Our Atms")
            {
                Rc.IsOneMatchingCateg = false;
            }
            else
            {
                Rc.IsOneMatchingCateg = true; // ONLY ONE CATEGORY 
            }

            Rc.HasOwner = true;
            //Rc.OwnerUserID = textBoxOwner.Text;

            if (comboBox2.Text == "Our Atms")
            {
                //
                // VALIDATION 
                //
                DublicateMatchingCat = false;
                TotalGridSelect = 0;
                K = 0;

                while (K <= (dataGridView2.Rows.Count - 1))
                {
                    WSelect = (bool)dataGridView2.Rows[K].Cells["Select"].Value;
                    WMatchingCategoryId = (string)dataGridView2.Rows[K].Cells["CategoryId"].Value;

                    if (WSelect == true)
                    {
                        TotalGridSelect = TotalGridSelect + 1;

                        SelectionCriteria = " WHERE ReconcCategoryId='"+WRecCategoryId+"'" 
                                        +" AND MatchingCategoryId ='" + WMatchingCategoryId + "'";
                        RcMc.ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(SelectionCriteria);
                        if (RcMc.RecordFound & RcMc.ReconcCategoryId != Rc.CategoryId)
                        {
                            MessageBox.Show("This Matching Category ALready belongs to :" + RcMc.ReconcCategoryId + " Correct and repeat");
                            K = 200;
                            DublicateMatchingCat = true;
                            return;
                        }

                    }
                    K++; // Read Next entry of the table 
                }


                if (DublicateMatchingCat == true)
                {
                    return; // Finish without updating 
                }
                else
                {
                    if (TotalGridSelect == 0)
                    {
                        MessageBox.Show("Please make selection of Matching Category");
                        return;
                    }
                    // Delete all previous 
                    RcMc.DeleteReconcCateqoriesVsMatchingCategoriesByReconcCategory(Rc.CategoryId);
                }

                // Insert all in Grid 

                K = 0;

                while (K <= (dataGridView2.Rows.Count - 1))
                {
                    WSelect = (bool)dataGridView2.Rows[K].Cells["Select"].Value;
                    WMatchingCategoryId = (string)dataGridView2.Rows[K].Cells["CategoryId"].Value;

                    if (WSelect == true)
                    {
                        RcMc.ReconcCategoryId = Rc.CategoryId;
                        RcMc.GroupId = Rc.AtmGroup;
                        RcMc.MatchingCategoryId = WMatchingCategoryId;
                        RcMc.OpeningDateTm = CurrentDt;
                        RcMc.Operator = WOperator;
                        // INSERT
                        RcMc.InsertReconcCateqoriesVsMatchingCategories();

                    }

                    K++; // Read Next entry of the table 
                }
            }
            
            //
            // Update Category 
            //

            Rc.UpdateReconcCategory(WOperator, Rc.CategoryId);

            MessageBox.Show("Updating Done!");

            textBoxMsgBoard.Text = "Reconc Category updated.";

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form504_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // DELETE 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete(Close) this category?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                // Delete Category
                Rc.DeleteReconcCategory(WRecCategoryId);
                // Delete Relations 
                RcMc.DeleteReconcCateqoriesVsMatchingCategoriesByReconcCategory(Rc.CategoryId);

                int WRowIndex1 = dataGridView1.SelectedRows[0].Index;

                Form504_Load(this, new EventArgs());

                MessageBox.Show("Category " + Rc.CategoryId + " Deleted.");

                textBoxMsgBoard.Text = "Category "+ Rc.CategoryId + " Deleted.";

                if (WRowIndex1 > 0)
                {
                    WRowIndex1 = WRowIndex1 - 1;
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }
            }
            else
            {
                return;
            }
        }

        // Define Owners 
        string KeepPreviousOwner; 
        private void buttonChangeOwner_Click_1(object sender, EventArgs e)
        {
            KeepPreviousOwner = textBoxOwner.Text; 

            Form503_CategOwners NForm503_CategOwners;
            if (textBoxCategoryId.Text == "")
            {
                MessageBox.Show("Please define Category");
                return;
            }
            int Mode = 1;
            NForm503_CategOwners = new Form503_CategOwners(WSignedId, WSignRecordNo, WOperator,
                                                     textBoxCategoryId.Text, W_Application, Mode, WForWhatUser);
            NForm503_CategOwners.FormClosed += NForm503_CategOwners_FormClosed;
            NForm503_CategOwners.ShowDialog();
        }

        string KeepAfterOwner;
        void NForm503_CategOwners_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Entries == true) WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form504_Load(this, new EventArgs());

            if (Entries == true)
            {
                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
            }

            RRDMUsersAccessToAtms Uata = new RRDMUsersAccessToAtms();

            Rc.ReadReconcCategoriesbySeqNo(WOperator, WSeqNo);
            Uata.CreateEntriesInUsersAtmTableFortheNewOwner(WOperator, Rc.AtmGroup, Rc.OwnerUserID);
            //KeepPreviousOwner = ""; 
            KeepAfterOwner = textBoxOwner.Text;

            if (Is_Maker == true)
            {
                // DO ... 
                if (KeepAfterOwner != KeepPreviousOwner)
                {
                    // Means that changed took place 
                    // We must update the USER record
                    Us.ReadUsersRecord_FULL(WForWhatUser);

                    // Initialised Checker information on User Record

                    if (Us.Is_Approved == false)
                    {
                        // Maker already in operation
                        // just set up what is needed
                        Us.Is_NewCategory = true;
                        Us.DtTimeOfChange = DateTime.Now;

                        Us.UpdateTail_For_Maker_Work(WForWhatUser);
                    }

                    if (Us.Is_Approved == true)
                    {
                        // Open Case 
                        Us.ApprovedByWhatChecker = "";
                        Us.ApproveOR_Reject = "";
                        Us.DtTimeOfApproving = NullPastDate;
                        Us.Is_Approved = false;
                        Us.UserInactive = true;

                        // UPDATE CHECKER INFORMATION

                        Us.UpdateTail_For_Checker_Work(WForWhatUser);

                        // Initialised Maker information on User Record
                        Us.ChangedByWhatMaker = WSignedId; // is the Designated Maker
                        Us.DtTimeOfChange = DateTime.Now;

                        Us.Is_New_User = false;
                        Us.Is_NewAccessRights = false;
                        Us.Is_NewCategory = true;
                        Us.Is_NewAuthoriser = false;

                        // UPDATE MAKER INFORMATION

                        Us.UpdateTail_For_Maker_Work(WForWhatUser);
                    }

                }
            }

        }

        // Change Filter 
        //private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (WMode == 5)
        //    {
        //        return;
        //    }
        //    else Form504_Load(this, new EventArgs());
        //}
        // Check Origin and Show Panel
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InternalChange == true)
            {
                return;
            }
            if (comboBox2.Text == "Our Atms")
            {
                textBoxCategoryId.Text = "RECATMS-" + comboBoxGroups.Text;
                textBoxCategDescr.Text = "Own ATMs - Group " + comboBoxGroups.Text;
             
                label19.Show();
                comboBoxGroups.Show();
                linkLabel1.Show();

                if (checkBoxMakeNewCategory.Checked == true)
                {
                    label5.Show();
                    dataGridView2.Show();
                }
            }
            else
            {
                textBoxCategoryId.Text = "";
                textBoxCategDescr.Text = "";
              
                label19.Hide();
                comboBoxGroups.Hide();
                linkLabel1.Hide();

                label5.Hide();
                dataGridView2.Hide();
            }
        }

        // Group has changed 
        private void comboBoxGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InternalChange == true)
            {
                return;
            }
            if (comboBox2.Text == "Our Atms")
            {
                WGroupId = Convert.ToInt32(comboBoxGroups.Text);
              
                textBoxCategoryId.Text = "RECATMS-" + WGroupId;
                textBoxCategDescr.Text = "Own ATMs - Group " + WGroupId;   
            }
            else
            {
                textBoxCategoryId.Text = "";
                textBoxCategDescr.Text = "";
            }
        }


        private void button52_Click(object sender, EventArgs e)
        {
            FormHelp helpForm = new FormHelp("Matching Categories Definition");
            helpForm.ShowDialog();
        }

        // Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Expand Groups 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form45 NForm45;
            NForm45 = new Form45(WSignedId, WSignRecordNo, WOperator);
            NForm45.ShowDialog();
        }

        private void checkBoxMakeNewCategory_CheckedChanged(object sender, EventArgs e)
        {
            if (InternalChange == true)
            {
                return; 
            }
         
            if (checkBoxMakeNewCategory.Checked == true)
            {
                buttonAdd.Show();
                buttonUpdate.Hide();
                buttonDelete.Hide();

                // Enable
                comboBoxGroups.Enabled = true;
              

                Gp.ParamId = "707"; // Origin  
                comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
                comboBox2.DisplayMember = "DisplayValue";

                comboBox2.Text = "Our Atms";
                comboBox2.Enabled = false;
                // Groups
                comboBoxGroups.DataSource = Gr.GetGroupNosList(WOperator);
                comboBoxGroups.DisplayMember = "DisplayValue";

                //ReconcMaster
                //SelectionCriteria = " WHERE ReconcMaster = 0";
                Mc.ReadMatchingCategoriesSlavesAndFillTable(WOperator, "NoValue");

                ShowGrid2();

                textBoxCategoryId.Text = "RECATMS-" + comboBoxGroups.Text;
                textBoxCategDescr.Text = "Own ATMs - Group " + comboBoxGroups.Text;

                //textBoxCategoryId.Text = "";
                //textBoxCategDescr.Text = "";

                textBoxOwner.Text = ""; 

            }
            else
            {
                buttonAdd.Hide();
                buttonUpdate.Show();
                buttonDelete.Show();

                //dataGridView2.Enabled = true;
                int WRowIndex1 = -1;

                if (dataGridView1.Rows.Count > 0)
                {
                    WRowIndex1 = dataGridView1.SelectedRows[0].Index;
                }

                Form504_Load(this, new EventArgs());

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }

            }
        }
        // Show Grid1 
        private void ShowGrid1()
        {
            //dataGridView1.DataSource = null;
            //dataGridView1.Refresh();

            dataGridView1.DataSource = Rc.TableReconcCateg.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {

                checkBoxMakeNewCategory.Checked = true;

                Entries = false;
                return;
            }
            else
            {
                checkBoxMakeNewCategory.Checked = false;
                Entries = true;
            }

            dataGridView1.Columns[0].Width = 60; // Seq No
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 120; // Category Id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 280; // Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 80; // Origin
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 60; // Group
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 110; // OWNER
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        // Show Grid2 
        private void ShowGrid2()
        {
            dataGridView2.DataSource = Mc.TableMatchingCateg.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //label12.Hide();
                //textBoxOwner.Hide(); 
                //buttonChangeOwner.Hide();
                return;
            }

            dataGridView2.Columns[0].Width = 60; // Seq No
            dataGridView2.Columns[0].Name = "SeqNo";
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].Visible = false;

            dataGridView2.Columns[1].Width = 60; // Select
            dataGridView2.Columns[1].Name = "Select";
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 90; // Category Id
            dataGridView2.Columns[2].Name = "CategoryId";
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 240; // Name
        }
// Update His ATMs
        private void buttonUpdateHisATMs_Click(object sender, EventArgs e)
        {
            RRDMUsersAccessToAtms Uata = new RRDMUsersAccessToAtms();

            Rc.ReadReconcCategoriesbySeqNo(WOperator, WSeqNo);
            Uata.CreateEntriesInUsersAtmTableFortheNewOwner(WOperator, Rc.AtmGroup, Rc.OwnerUserID);

            // Show the ATMs of the group 
            Form78d_UserVsAtms NForm78d_UserVsAtms;

            int WMode = 1; // Show ATMs for User
            NForm78d_UserVsAtms = new Form78d_UserVsAtms(WOperator, WSignedId, Rc.OwnerUserID, Rc.AtmGroup, WMode);
            NForm78d_UserVsAtms.ShowDialog();

        }
    }
}

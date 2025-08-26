using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form503_WALLET : Form
    {
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMBanks Ba = new RRDMBanks();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();

        RRDMNVBanksNostroVostro Bnv = new RRDMNVBanksNostroVostro();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WCategoryId;
        string WMainCateg;
        string WMainCategITMX;

        int BIN;

        int WSeqNo;

        int WRowIndex;

        string PRX;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string W_Application;
        int WMode;

        public Form503_WALLET(string InSignedId, int SignRecordNo, string InOperator,
                                                        string In_Application, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            W_Application = In_Application;
            WMode = InMode;
            // InMode = 7 means is MOBILE_WALLET     

            InitializeComponent();

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = WSignedId;

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }


            //Gp.ReadParameterByOccuranceId("101", "2");
            //if (Gp.RecordFound == true)
            //{
            //    PRX = Gp.OccuranceNm;
            //}

            if (WOperator == "ETHNCY2N")
            {
                PRX = WOperator.Substring(0, 3);
                PRX = "NBG";
            }

            Gp.ParamId = "707"; // Origin  
            comboBoxOrigin.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxOrigin.DisplayMember = "DisplayValue";

            Gp.ParamId = "708"; // Transaction At Origin   
            comboBoxTrxType.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxTrxType.DisplayMember = "DisplayValue";

            Gp.ParamId = "712"; // Running Jobs   for Matching 
            comboBoxRunningJob.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxRunningJob.DisplayMember = "DisplayValue";

            Gp.ParamId = "709"; // Products   
            comboBoxProducts.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxProducts.DisplayMember = "DisplayValue";

            Gp.ParamId = "710"; // Cost Centre  
            comboBox5.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox5.DisplayMember = "DisplayValue";


            // Matching Schedule Groups
            RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();

            string WEventType = "Categories Matching";

            if (WMode == 1)
            {
                comboBoxFilter.Items.Add("ALL_Active");
                comboBoxFilter.Items.Add("ALL_Not_Active");
                comboBoxFilter.Items.Add("Active And Not Active");
                // comboBoxFilter.Items.Add("This ATM by Date range");

                comboBoxFilter.SelectedIndex = 0;
            }


            if (WMode == 7)
            {

                if (WMode == 7) comboBoxFilter.Items.Add("e_MOBILE");
                if (WMode == 7) comboBoxFilter.Text = "e_MOBILE";

                //buttonNext.Show();
            }


        }
        // Load 
        private void Form503_Load(object sender, EventArgs e)
        {
            // SHOW ALL OF THIS comboBoxFilter

            // ATMS AND OTHERS 
            Mc.ReadMatchingCategoriesAndFillTable(WOperator, W_Application);

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
            dataGridView1.Columns[0].Visible = true;

            //dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

            dataGridView1.Columns[1].Width = 90; // Category Id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 400; // Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 70; // "Is POS_Type"
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 70; // "Days W"
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 70; // "Days C"
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 70; // "TWIN"
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 150; // Origin 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 150; // TransAtOrigin
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[9].Width = 150; // Product 
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }
        // On Row Enter
        bool ExistOtherBINCategory;
        string WMainPRX;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Mc.ReadMatchingCategorybySeqNo_Any(WOperator, WSeqNo);

            WCategoryId = Mc.CategoryId;

            WMainCateg = WCategoryId.Substring(0, 4);

            WMainPRX = WCategoryId.Substring(0, 3);

            WMainCategITMX = WCategoryId.Substring(0, 5);

            textBox1.Text = Mc.CategoryId;

            textBoxCategDescr.Text = Mc.CategoryName; 

            comboBoxOrigin.Text = Mc.Origin;

            if (Mc.ReconcMaster == true)
            {
                // Master
                radioButtonMaster.Checked = true;
            }

            comboBoxTrxType.Text = Mc.TransTypeAtOrigin;

            comboBoxRunningJob.Text = Mc.RunningJobGroup;
            comboBoxProducts.Text = Mc.Product;

            //textBoxCategDescr.Text = Rc.CategoryName;
            textBoxCategDescr.ReadOnly = false;

            comboBox5.Text = Mc.CostCentre;
            //comboBoxMatchingScheduleID.Text = Mc.Periodicity;

            if (Mc.Pos_Type == true)
            {
                checkBoxIsPOS_type.Checked = true;
                textBoxUnmatchedWorking.Text = Mc.UnMatchedForWorkingDays.ToString();
                textBoxUnmatchedCalendar.Text = Mc.UnMatchedForCalendarDays.ToString();
            }
            else
            {
                checkBoxIsPOS_type.Checked = false;
                textBoxUnmatchedWorking.Text = "";
                textBoxUnmatchedCalendar.Text = "";
            }
            if (Mc.TWIN == true)
            {
                checkBoxTWIN.Checked = true;
            }
            else
            {
                checkBoxTWIN.Checked = false;
            }

            if (Mc.Active == true)
            {
                checkBoxIsActive.Checked = true;
            }
            else
            {
                checkBoxIsActive.Checked = false;
            }

            if (WMode == 1) // ATMS
            {
                // No need for action 
            }
            else
            {
                Rc.ReadReconcCategorybyCategId(WOperator, Mc.CategoryId);
                if (Rc.HasOwner == true)
                {
                    Us.ReadUsersRecord(Rc.OwnerUserID);

                    //textBoxOwner.Text = Mc.OwnerId + " " + Us.UserName;
                    //buttonChangeOwner.Text = "Change Owner";
                }
                else
                {
                    //textBoxOwner.Text = "Category has no Owner.";
                    //buttonChangeOwner.Text = "Assign Owner";
                }
            }

            //textBoxNextMatchingDt.Text = Mc.NextMatchingDt.ToString();

            buttonUpdate.Show();
        }
        // ADD
        private void buttonAdd_Click(object sender, EventArgs e)
        {

            if (textBox8.TextLength != 6)
            {
                MessageBox.Show("New category must have length of six digits");
                return;
            }

            if (
              textBox8.Text.Substring(0, 3) != "QAH"
              & textBox8.Text.Substring(0, 3) != "ETI"
              & textBox8.Text.Substring(0, 3) != "IPN"
              )

            {
                MessageBox.Show("Please enter the correct category.");
                return;
            }

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, textBox8.Text);
            if (Mc.RecordFound == true)
            {
                MessageBox.Show(textBox1.Text, "This Category Already exist.  ");
                return;
            }

            if (textBoxCategDescr.Text.Length < 5 )
            {
                MessageBox.Show("Please enter Category name.");
                return;
            }

            //
            //  Validation
            //

            if (checkBoxIsPOS_type.Checked == true)
            {

                Mc.Pos_Type = true;

                if (textBoxUnmatchedWorking.Text == "" & textBoxUnmatchedCalendar.Text == "")
                {
                    MessageBox.Show("Please enter value for Unmatched days period. ");
                    return;
                }
                else
                {
                    if (int.TryParse(textBoxUnmatchedWorking.Text, out Mc.UnMatchedForWorkingDays))
                    {
                    }
                    else
                    {
                        MessageBox.Show(textBoxUnmatchedWorking.Text, "Please enter a valid number in UnMatched Working Days. ");
                        return;
                    }
                    // 
                    if (int.TryParse(textBoxUnmatchedCalendar.Text, out Mc.UnMatchedForCalendarDays))
                    {
                    }
                    else
                    {
                        MessageBox.Show(textBoxUnmatchedWorking.Text, "Please enter a valid number in UnMatched Working Days. ");
                        return;
                    }
                }
            }
            else
            {
                Mc.Pos_Type = false;
                Mc.UnMatchedForWorkingDays = 0;
                Mc.UnMatchedForCalendarDays = 0;
            }


            if (checkBoxTWIN.Checked == true)
            {
                Mc.TWIN = true;
            }
            else
            {
                Mc.TWIN = false;
            }


            Mc.CategoryId = textBox8.Text;

            textBoxCategDescr.Text = textBoxCategDescr.Text;

            Mc.CategoryName = textBoxCategDescr.Text;

            Mc.Origin = comboBoxOrigin.Text;


            if (radioButtonMaster.Checked == true)
            {
                // Master
                Mc.ReconcMaster = true;
            }
            else
            {
                // Slave
                Mc.ReconcMaster = false;
            }

            Mc.TransTypeAtOrigin = comboBoxTrxType.Text;

            Mc.RunningJobGroup = comboBoxRunningJob.Text;
            Mc.Product = comboBoxProducts.Text;
            Mc.CostCentre = comboBox5.Text;
            //Mc.Periodicity = comboBoxMatchingScheduleID.Text;
            //Mc.GroupIdInFiles = textBoxBIN.Text;
            //Mc.FieldName = textBox4.Text;
            Mc.Currency = "";
            Mc.EntityA = "";

            //Rc.DR = radioButtonDR.Checked;
            //Rc.CR = radioButtonCR.Checked;

            Mc.EntityB = "";
            Mc.GlAccount = "";

            Mc.VostroBank = "";
            Mc.VostroCurr = "";
            Mc.VostroAcc = "";
            Mc.InternalAcc = "";
            Mc.TargetSystemNm = ""; 

            Mc.Operator = WOperator;

            // Insert NEXT LOADING 
            // Last loaded Date not available then insert current date 
            //

            //DateTime WDate;
            //RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();

            //string WSelectionCriteria = " WHERE ScheduleID ='" + Mc.Periodicity + "'";

            //Js.ReadJTMEventSchedulesToGetRecord(WSelectionCriteria);

            //if (Js.EffectiveDateTmFrom > DateTime.Now)
            //{
            //    WDate = Js.EffectiveDateTmFrom.AddDays(-1);
            //}
            //else
            //{
            //    WDate = DateTime.Now;
            //}
            //DateTime NextLoading = Js.ReadCalculatedNextEventDateTm(WOperator, Mc.Periodicity,
            //                                                           WDate, WDate);
            //textBoxNextMatchingDt.Text = NextLoading.ToString();
            Mc.Periodicity = ""; 

            Mc.NextMatchingDt = NullPastDate;

            WSeqNo = Mc.InsertMatchingCategory();

            
                // Create Record in Reconciliation File 

                
                    Rc.CategoryId = Mc.CategoryId;

                    Rc.CategoryName = Mc.CategoryName;

                    Rc.Origin = comboBoxOrigin.Text;

                    Rc.AtmGroup = 0;

                    Rc.IsOneMatchingCateg = true;

                    Rc.HasOwner = false;
                    Rc.OwnerUserID = "";

                    Rc.OpeningDateTm = DateTime.Now;

                    Rc.Operator = WOperator;

                    WSeqNo = Rc.InsertReconcCategory();

                    MessageBox.Show("Record with id " + Mc.CategoryId + " Inserted In Matching Categories!" + Environment.NewLine
                                + "Also A corresponding record was inserted in Reconciliation Categories" + Environment.NewLine
                                + "Go and Assign Owner for Reconciliation"
                              );

                

            

            textBoxMsgBoard.Text = "New Matching Category inserted.";

            textBox8.Text = "";

            Mc.ReadCategoriesToFindPositionOfSeqNo(WOperator, WSeqNo, "");
            
            Form503_Load(this, new EventArgs());

            //dataGridView1.Rows[Mc.PositionInGrid].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, Mc.PositionInGrid));

        }

        // UPDATE CATEGORY 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            Mc.ReadMatchingCategorybySeqNo_Any(WOperator, WSeqNo);

            Mc.CategoryId = textBox1.Text;
            //Mc.CategoryName = 
            //if (WMainCateg == "EWB3" || WMainCateg == "EWB4" || WMainCateg == "EWB5" || WMainCateg == "EWB8"
            //    || WMainPRX == PRX || WMainCateg == PRX + "2" || WMainCateg == PRX + "3" || WMainCateg == PRX + "5" || WMainCateg == PRX + "1"
            //    )
            //{
            //    if (WMainCateg == "EWB3" || WMainCateg == "EWB4" || WMainCateg == "EWB5"
            //        || WMainPRX == PRX || WMainCateg == PRX + "2" || WMainCateg == PRX + "3" || WMainCateg == PRX + "5" || WMainCateg == PRX + "1"
            //        )
            //    {
            //        textBoxCategDescr.Text = comboBoxOrigin.Text + " " + comboBoxTrxType.Text + " - " + comboBoxProducts.Text;
            //        textBoxCategDescr.ReadOnly = true;
            //    }
            //}
            //else
            //{
            //    //textBoxCategDescr.Text = Rc.CategoryName;
            //    textBoxCategDescr.ReadOnly = false;
            //}

            if (Mc.ReconcMaster == true & radioButtonMaster.Checked == true)
            {
                Rc.ReadReconcCategorybyCategId(WOperator, Mc.CategoryId);

                Rc.CategoryId = Mc.CategoryId;

                Rc.CategoryName = textBoxCategDescr.Text;

                Rc.Origin = comboBoxOrigin.Text;

                Rc.AtmGroup = 0;

                Rc.IsOneMatchingCateg = true;

                if (Rc.OwnerUserID == null) Rc.OwnerUserID = ""; 

                Rc.UpdateReconcCategory(WOperator, Mc.CategoryId);
            }


            if (checkBoxIsPOS_type.Checked == true)
            {

                Mc.Pos_Type = true;

                if (textBoxUnmatchedWorking.Text == "" & textBoxUnmatchedCalendar.Text == "")
                {
                    MessageBox.Show("Please enter value for Unmatched days period. ");
                    return;
                }
                else
                {
                    if (int.TryParse(textBoxUnmatchedWorking.Text, out Mc.UnMatchedForWorkingDays))
                    {
                    }
                    else
                    {
                        if (textBoxUnmatchedWorking.Text == "")
                        {

                        }
                        else
                        {
                            MessageBox.Show(textBoxUnmatchedWorking.Text, "Please enter a valid number in UnMatched Working Days. ");
                            return;
                        }

                    }
                    // 
                    if (int.TryParse(textBoxUnmatchedCalendar.Text, out Mc.UnMatchedForCalendarDays))
                    {
                    }
                    else
                    {
                        if (textBoxUnmatchedCalendar.Text == "")
                        {

                        }
                        else
                        {
                            MessageBox.Show(textBoxUnmatchedWorking.Text, "Please enter a valid number in UnMatched Calendar Days. ");
                            return;
                        }

                    }
                }
            }
            else
            {
                Mc.Pos_Type = false;
                Mc.UnMatchedForWorkingDays = 0;
                Mc.UnMatchedForCalendarDays = 0;
            }

            if (checkBoxTWIN.Checked == true)
            {
                Mc.TWIN = true;
            }
            else
            {
                Mc.TWIN = false;
            }

            if (checkBoxIsActive.Checked == true)
            {
                Mc.Active = true;
            }
            else
            {
                Mc.Active = false;
            }

            Mc.CategoryName = textBoxCategDescr.Text;

            Mc.Origin = comboBoxOrigin.Text;
            Mc.TransTypeAtOrigin = comboBoxTrxType.Text;

            Mc.TargetSystemNm = "";



            if (radioButtonMaster.Checked == true)
            {
                // Master
                Mc.ReconcMaster = true;
            }
            else
            {
                // Slave
                Mc.ReconcMaster = false;
            }

            Mc.RunningJobGroup = comboBoxRunningJob.Text;
            Mc.Product = comboBoxProducts.Text;
            Mc.CostCentre = comboBox5.Text;

            //Mc.GroupIdInFiles = textBoxBIN.Text;
            //Mc.FieldName = textBox4.Text;
            Mc.Currency = "";
            Mc.EntityA = "";
            //Rc.DR = radioButtonDR.Checked;
            //Rc.CR = radioButtonCR.Checked;

            Mc.EntityB = "";
            Mc.GlAccount = "";

            Mc.VostroBank = "";
            Mc.VostroCurr = "";
            Mc.VostroAcc = "";
            Mc.InternalAcc = "";

            //Mc.Periodicity = comboBoxMatchingScheduleID.Text;

            Mc.UpdateMatchingCategory(WOperator, Mc.CategoryId);

            MessageBox.Show("Updating Done!");

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            textBoxMsgBoard.Text = "Matching Category updated.";

            Form503_Load(this, new EventArgs());

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

                Mc.DeleteMatchingCategory(WSeqNo, WCategoryId);

                Rc.DeleteReconcCategory(WCategoryId);

                RcMc.DeleteReconcCateqoriesVsMatchingCategoriesByReconcCategory(WCategoryId);

                int WRowIndex1 = dataGridView1.SelectedRows[0].Index;

                Form503_Load(this, new EventArgs());

                MessageBox.Show("Deleted!");

                textBoxMsgBoard.Text = "Matching Category Deleted.";

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


        void NForm85_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }


        // Define Owners 
        private void buttonChangeOwner_Click(object sender, EventArgs e)
        {
            Form503_CategOwners NForm503_CategOwners;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            int Mode = 1;
            NForm503_CategOwners = new Form503_CategOwners(WSignedId, WSignRecordNo, WOperator, WCategoryId, W_Application, Mode, "");
            NForm503_CategOwners.FormClosed += NForm503_CategOwners_FormClosed;
            NForm503_CategOwners.ShowDialog();
        }

        void NForm503_CategOwners_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

        // Change Filter 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WMode == 5 || WMode == 7)
            {
                return;
            }
            else
                Form503_Load(this, new EventArgs());
        }
        // Check Origin and Show Panel
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        // Product has changed  - change the TextBoxBIN

        private void comboBoxProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            //bool found = false;

            Gp.ReadParametersSpecificNm(WOperator, "709", comboBoxProducts.Text);
            if (Gp.RecordFound == true)
            {
                BIN = (int)Gp.Amount;
                if (BIN == 0)
                {
                    // panel5.Hide();
                }
                else
                {
                    //textBoxBIN.Text = BIN.ToString();
                    //panel5.Show();
                }

            }




        }
        //If Transaction Type changes 
        private void comboBoxTrxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //buttonUpdate.Hide();
            ShowFields();
        }


        // Fill in fields 
        public void ShowFields()
        {

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
        // Show fields if checked
        private void checkBoxIsPOS_type_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxIsPOS_type.Checked == true)
            {
                label5.Show();
                label6.Show();
                textBoxUnmatchedWorking.Show();
                textBoxUnmatchedCalendar.Show();
                textBoxUnmatchedWorking.Text = "";
                textBoxUnmatchedCalendar.Text = "";
            }
            else
            {
                label5.Hide();
                label6.Hide();
                textBoxUnmatchedWorking.Hide();
                textBoxUnmatchedCalendar.Hide();
                textBoxUnmatchedWorking.Text = "";
                textBoxUnmatchedCalendar.Text = "";
            }
        }

    }
}

using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form503_Visa : Form
    {
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMBanks Ba = new RRDMBanks();
        RRDMUsersRecords Us = new RRDMUsersRecords();
       
        RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();

        RRDMNVBanksNostroVostro Bnv = new RRDMNVBanksNostroVostro();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WCategoryId;
        string WMainCateg;
        string WMainCategITMX;

        //int BIN;

        int WSeqNo;

        int WRowIndex;

        string WPrefix;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WOrigin;
        int WMode;

        public Form503_Visa(string InSignedId, int SignRecordNo, string InOperator,
                                                        string InOrigin, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WOrigin = InOrigin;
            WMode = InMode;

            // InMode = 5 means is NOSTRO 
            // InMode = 6 means is Visa Settlement 

        

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

            radioButtonMaster.Checked = true;
            radioButtonSlave.Checked = false;

            if (WOperator == "ETHNCY2N")
            {
                WPrefix = WOperator.Substring(0, 3);
                WPrefix = "NBG"; // National Bank
            }

            if (WOrigin == "E_FINANCE")
            {
                WPrefix = "BDC"; // Bank de Caire
            }

            radioButtonMaster.Enabled = false;
            radioButtonSlave.Enabled = false;

            Gp.ParamId = "201"; // Currencies  
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            //Gp.ParamId = "201"; // Currencies  
            //comboBox6.DataSource = Gp.GetParamOccurancesNm(WOperator);
            //comboBox6.DisplayMember = "DisplayValue";

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

            comboBoxMatchingScheduleID.DataSource = Js.GetScheduleIdsByType(WOperator, WEventType);
            comboBoxMatchingScheduleID.DisplayMember = "DisplayValue";

            Gp.ParamId = "707"; // fILTER 
            comboBoxFilter.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxFilter.DisplayMember = "DisplayValue";
             
            if (WOrigin == "E_FINANCE")
            {
                comboBoxFilter.Text = "E_FINANCE";
                panelSchedule.Hide(); 
            }

            if (WOrigin == "Visa")
            {
                comboBoxFilter.Text = "Visa Settlement";
                panelSchedule.Show();
            }

            buttonNext.Show();


            //External Bank Names 
            comboBoxExternalBank.DataSource = Bnv.GetExternalBanksNames(WOperator);
            comboBoxExternalBank.DisplayMember = "DisplayValue";


            textBoxMsgBoard.Text = "Definition of Matching Categories ";
        }
        // Load 
        private void Form503_Load(object sender, EventArgs e)
        {
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

            dataGridView1.Columns[1].Width = 90; // Category Id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 300; // Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView1.Columns[3].Width = 150; // Origin 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView1.Columns[4].Width = 150; // TransAtOrigin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 150; // Product 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }
        // On Row Enter
        
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Mc.ReadMatchingCategorybySeqNoActive(WOperator, WSeqNo);

            WCategoryId = Mc.CategoryId;

            textBoxCategDescr.Text = Mc.CategoryName;

            WMainCateg = WCategoryId.Substring(0, 4);

            WMainCategITMX = WCategoryId.Substring(0, 5);

            textBoxCategoryId.Text = Mc.CategoryId;

            comboBoxOrigin.Text = Mc.Origin;

            if (Mc.ReconcMaster == true)
            {
                // Master
                radioButtonMaster.Checked = true;
            }
            else
            {
                // Slave
                radioButtonSlave.Checked = true;
            }
            comboBoxTrxType.Text = Mc.TransTypeAtOrigin;

            comboBoxRunningJob.Text = Mc.RunningJobGroup;
            comboBoxProducts.Text = Mc.Product;

            comboBox1.Text = Mc.Currency;

            textBox7.Text = Mc.GlAccount;

            comboBoxExternalBank.Text = Mc.VostroBank;
            comboBoxExternalAccount.Text = Mc.VostroAcc;
            textBoxVostroCcy.Text = Mc.VostroCurr;
            textBoxInternalAccount.Text = Mc.InternalAcc;

            //textBoxCategDescr.Text = comboBoxOrigin.Text + " " + comboBoxTrxType.Text + " - " + comboBoxProducts.Text;
            //textBoxCategDescr.ReadOnly = true;

            comboBox5.Text = Mc.CostCentre;

            if (WOrigin == "Visa")
            {
                comboBoxMatchingScheduleID.Text = Mc.Periodicity;

                textBoxNextMatchingDt.Text = Mc.NextMatchingDt.ToString();
            }
          

            buttonUpdate.Show();
        }
        // ADD
        private void buttonAdd_Click(object sender, EventArgs e)
        {
           
            if (textBox8.TextLength != 6)
            {

                if (WOrigin == "Visa")
                {
                    MessageBox.Show("New category must have length of six digits" + Environment.NewLine
                           + "It starts with.." + WPrefix + " Next three is a serial number"
                                    );
                    return;
                }
                if (WOrigin == "E_FINANCE")
                {
                    MessageBox.Show("New category must have length of six digits" + Environment.NewLine
                        + "It starts with.." + WPrefix + " Next three digits is Branch id"
                         );

                    return;
                }

            }

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, textBox8.Text);
            if (Mc.RecordFound == true)
            {
                MessageBox.Show(textBoxCategoryId.Text, "This Category Already exist.  ");
                return;
            }

            Mc.CategoryId = textBox8.Text;

            Mc.Origin = comboBoxOrigin.Text;

            Mc.ReconcMaster = true;

            Mc.TransTypeAtOrigin = comboBoxTrxType.Text;

            Mc.TargetSystemId = 0;
            Mc.TargetSystemNm = "";

            Mc.RunningJobGroup = comboBoxRunningJob.Text;
            Mc.Product = comboBoxProducts.Text;
            Mc.CostCentre = comboBox5.Text;
            Mc.Periodicity = comboBoxMatchingScheduleID.Text;
          //  Mc.GroupIdInFiles = "";
            Mc.Pos_Type = false;
            Mc.Currency = comboBox1.Text;
            Mc.EntityA = "";

            Mc.EntityB = "";
            Mc.GlAccount = textBox7.Text;

            Mc.VostroBank = comboBoxExternalBank.Text;
            Mc.VostroCurr = textBoxVostroCcy.Text;
            Mc.VostroAcc = comboBoxExternalAccount.Text;
            Mc.InternalAcc = textBoxInternalAccount.Text;
           
            if (WOrigin == "E_FINANCE")
            {
                Mc.CategoryName = "E_FINANCE For " + Mc.InternalAcc ;
            }
            if (WOrigin == "Visa")
            {
                Mc.CategoryName = "Visa for " + Mc.InternalAcc;
            }

            Mc.Operator = WOperator;

            if (WOrigin == "Visa")
            {
                // Insert NEXT LOADING 
                // Last loaded Date not available then insert current date 
                //

                DateTime WDate;
                RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();

                string WSelectionCriteria = " WHERE ScheduleID ='" + Mc.Periodicity + "'";

                Js.ReadJTMEventSchedulesToGetRecord(WSelectionCriteria);

                if (Js.EffectiveDateTmFrom > DateTime.Now)
                {
                    WDate = Js.EffectiveDateTmFrom.AddDays(-1);
                }
                else
                {
                    WDate = DateTime.Now;
                }
                DateTime NextLoading = Js.ReadCalculatedNextEventDateTm(WOperator, Mc.Periodicity,
                                                                           WDate, WDate);
                textBoxNextMatchingDt.Text = NextLoading.ToString();

                Mc.NextMatchingDt = NextLoading;
            }
            else
            {
                Mc.NextMatchingDt = NullPastDate; 
            }
           

            WSeqNo = Mc.InsertMatchingCategory();


            // Create Record in Reconciliation File 

            if (Mc.ReconcMaster == true)
            {
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
            }
            else
            {
                MessageBox.Show("Record with id " + Mc.CategoryId + " Inserted!");
            }

            textBoxMsgBoard.Text = "New Matching Category inserted.";

            textBox8.Text = "";

            Mc.ReadCategoriesToFindPositionOfSeqNo(WOperator, WSeqNo, Mc.Origin);

            ////checkBoxMakeNewVersion.Checked = false;
            //WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form503_Load(this, new EventArgs());

            //dataGridView1.Rows[Mc.PositionInGrid].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, Mc.PositionInGrid));

        }

        // UPDATE CATEGORY 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            Mc.ReadMatchingCategorybySeqNoActive(WOperator, WSeqNo);
           
            Mc.CategoryId = textBoxCategoryId.Text;

            Mc.Origin = comboBoxOrigin.Text;

            Mc.ReconcMaster = true;

            Mc.TransTypeAtOrigin = comboBoxTrxType.Text;

            Mc.TargetSystemId = 0;
            Mc.TargetSystemNm = "";

            Mc.RunningJobGroup = comboBoxRunningJob.Text;
            Mc.Product = comboBoxProducts.Text;
            Mc.CostCentre = comboBox5.Text;
            Mc.Periodicity = comboBoxMatchingScheduleID.Text;
          //  Mc.GroupIdInFiles = "";
            Mc.Pos_Type = false;
            Mc.Currency = comboBox1.Text;
            Mc.EntityA = "";

            Mc.EntityB = "";
            Mc.GlAccount = textBox7.Text;

            Mc.VostroBank = comboBoxExternalBank.Text;
            Mc.VostroCurr = textBoxVostroCcy.Text;
            Mc.VostroAcc = comboBoxExternalAccount.Text;
            Mc.InternalAcc = textBoxInternalAccount.Text;

            if (WOrigin == "E_FINANCE")
            {
                Mc.CategoryName = "E_FINANCE For " + Mc.InternalAcc;
            }
            if (WOrigin == "Visa")
            {
                Mc.CategoryName = "Visa for " + Mc.InternalAcc;
            }

            Mc.Operator = WOperator;

            // Insert NEXT LOADING 
            // Last loaded Date not available then insert current date 
            //

        

            if (Mc.Periodicity != comboBoxMatchingScheduleID.Text || Mc.Periodicity == "")
            {
                Mc.Periodicity = comboBoxMatchingScheduleID.Text;

                // UPDATE NEXT LOADING 
                // Last loaded Date not available then insert current date 
                //
                DateTime WDate;
                RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();

                string WSelectionCriteria = " WHERE ScheduleID ='" + Mc.Periodicity + "'";

                Js.ReadJTMEventSchedulesToGetRecord(WSelectionCriteria);

                if (Js.EffectiveDateTmFrom > DateTime.Now)
                {
                    WDate = Js.EffectiveDateTmFrom.AddDays(-1);
                }
                else
                {
                    WDate = DateTime.Now;
                }

                DateTime NextLoading = Js.ReadCalculatedNextEventDateTm(WOperator, Mc.Periodicity,
                                                                           WDate, WDate);
                textBoxNextMatchingDt.Text = NextLoading.ToString();

                Mc.NextMatchingDt = NextLoading;
            }

            Mc.Periodicity = comboBoxMatchingScheduleID.Text;

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


        // Define Rules 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            Form291NVMatchingRulesDefinition NForm291NVMatchingRulesDefinition;
            string InternalAcc = Mc.InternalAcc;
            string ExternalBankID = Mc.VostroBank;
            string ExternalAccNo = Mc.VostroAcc;
            int RuleMode = 1;
            NForm291NVMatchingRulesDefinition = new Form291NVMatchingRulesDefinition(WSignedId, WSignRecordNo, WOperator,
                                          InternalAcc, ExternalBankID, ExternalAccNo, RuleMode, "", "");
            NForm291NVMatchingRulesDefinition.FormClosed += NForm291NVMatchingRulesDefinition_FormClosed;
            NForm291NVMatchingRulesDefinition.ShowDialog();
        }

        private void NForm291NVMatchingRulesDefinition_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

        void NForm503_CategOwners_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

        // Text Box Change for new Category 
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            textBoxCategoryId.Text = textBox8.Text;
            textBoxCategDescr.Text = "";
            if (WOrigin == "E_FINANCE")
            {
                Mc.CategoryName = "E_FINANCE For ......";
            }
            if (WOrigin == "Visa")
            {
                Mc.CategoryName = "Visa For Settlement.....";
            }
        }

        // Change Filter 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form503_Load(this, new EventArgs());
        }
        // Check Origin and Show Panel
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxOrigin.Text == "E_FINANCE"
                || comboBoxOrigin.Text == "Visa Settlement"
                )
            {
                panelVOSTRO.Show();
                label10.Text = "E_FINANCE";
                //panel5.Hide();
                label17.Hide();
                comboBox5.Hide();
                label11.Hide();
                comboBox1.Hide();
                label10.Hide();
                textBox7.Hide();
            }


        }
        
        private void button52_Click(object sender, EventArgs e)
        {
            FormHelp helpForm = new FormHelp("Matching Categories Definition");
            helpForm.ShowDialog();
        }
        // Handle change for External Banks  
        private void comboBoxExternalBanks_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Read all Accounts for this Bank 

            comboBoxExternalAccount.DataSource = Acc.GetExternalAccountsForExternalBank(WOperator, comboBoxExternalBank.Text);
            comboBoxExternalAccount.DisplayMember = "DisplayValue";


        }
        // Handle change for External Account 
        private void comboBoxExternalAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            Acc.ReadAndFindAccountSpecificForNostroVostro(comboBoxExternalAccount.Text, comboBoxExternalBank.Text);
            if (Acc.RecordFound == true)
            {
                textBoxVostroCcy.Text = Acc.CurrNm;
                textBoxInternalAccount.Text = Acc.AccNoInternal;
            }
            else
            {
                textBoxVostroCcy.Text = "";
                textBoxInternalAccount.Text = "";
            }
        }
        // Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

       
    }
}

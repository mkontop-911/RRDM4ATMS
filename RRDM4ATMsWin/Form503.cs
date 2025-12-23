using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form503 : Form
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
        string WOrigin;
        int WMode;

        public Form503(string InSignedId, int SignRecordNo, string InOperator,
                                                        string InOrigin, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WOrigin = InOrigin;
            WMode = InMode;
            // InMode = 1 means is ATMs
            // InMode = 3 means is ITMX
            // InMode = 5 means is NOSTRO 
            // InMode = 6 means is Visa Settlement     
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

            if (WMode == 1)
            {
                //label12.Hide();
                //textBoxOwner.Hide();
                //buttonChangeOwner.Hide();
            }

            Gp.ParamId = "201"; // Currencies  
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            //Gp.ParamId = "201"; // Currencies  
            //comboBox6.DataSource = Gp.GetParamOccurancesNm(WOperator);
            //comboBox6.DisplayMember = "DisplayValue";

            Gp.ParamId = "707"; // Origin  
            comboBoxOrigin.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxOrigin.DisplayMember = "DisplayValue";

            Gp.ParamId = "705"; // Target Systems 
            comboBoxTargetSystems.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxTargetSystems.DisplayMember = "DisplayValue";

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

            //comboBoxMatchingScheduleID.DataSource = Js.GetScheduleIdsByType(WOperator, WEventType);
            //comboBoxMatchingScheduleID.DisplayMember = "DisplayValue";

            //Gp.ParamId = "707"; // fILTER 
            //comboBoxFilter.DataSource = Gp.GetParamOccurancesNm(WOperator);
            //comboBoxFilter.DisplayMember = "DisplayValue";
            if (WMode == 1)
            {
                comboBoxFilter.Items.Add("ALL_Active");
                comboBoxFilter.Items.Add("ALL_Not_Active");
                comboBoxFilter.Items.Add("Active And Not Active");
                // comboBoxFilter.Items.Add("This ATM by Date range");

                comboBoxFilter.SelectedIndex = 0;
            }


            if (WMode == 5 || WMode == 6 || WMode == 7)
            {
                if (WMode == 5) comboBoxFilter.Items.Add("Nostro - Vostro");
                if (WMode == 5) comboBoxFilter.Text = "Nostro - Vostro";
                if (WMode == 7) comboBoxFilter.Items.Add("e_MOBILE");
                if (WMode == 7) comboBoxFilter.Text = "e_MOBILE";
                if (WMode == 6) comboBoxFilter.Text = "Visa Settlement";
                buttonNext.Show();
            }
            else
            {
                buttonNext.Hide();
            }

            // First Selection - Banks or other entities 
            comboBoxEntityA.DataSource = Ba.GetBanksShortNames(WOperator);
            comboBoxEntityA.DisplayMember = "DisplayValue";

            // Second Selection - Banks or other entities 
            comboBoxEntityB.DataSource = Ba.GetBanksShortNames(WOperator);
            comboBoxEntityB.DisplayMember = "DisplayValue";

            //External Bank Names 
            comboBoxExternalBank.DataSource = Bnv.GetExternalBanksNames(WOperator);
            comboBoxExternalBank.DisplayMember = "DisplayValue";

            if (WMode == 1) // ATMS
            {
                textBoxMsgBoard.Text = "Definition of Matching Categories ";
            }
            else
            {
                textBoxMsgBoard.Text = "Definition of Matching Categories and Owners";
            }

            if (WOperator == "ITMX")
            {
                //  panel5.Hide();
            }
            else
            {
                panel4.Hide();
            }
        }
        // Load 
        private void Form503_Load(object sender, EventArgs e)
        {
            // SHOW ALL OF THIS comboBoxFilter

            // ATMS AND OTHERS 
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

            //Mc.ReadMatchingCategorybyGetsAllOtherBINS(WOperator);
            //if (Mc.RecordFound == true)
            //{
            //    ExistOtherBINCategory = true; 
            //}
            //else
            //{
            //    ExistOtherBINCategory = false;
            //}

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Mc.ReadMatchingCategorybySeqNo_Any(WOperator, WSeqNo);

            WCategoryId = Mc.CategoryId;

            WMainCateg = WCategoryId.Substring(0, 4);

            WMainPRX = WCategoryId.Substring(0, 3);

            WMainCategITMX = WCategoryId.Substring(0, 5);

            textBox1.Text = Mc.CategoryId;

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

            comboBoxTargetSystems.Text = Mc.TargetSystemNm;

            checkBoxNotOwnBINS.Checked = Mc.GetsNotOwnBINS;

            comboBoxRunningJob.Text = Mc.RunningJobGroup;
            comboBoxProducts.Text = Mc.Product;

            //Debit Card
            //    Prepaid Card
            //        Gift Card
            //            Travel Money Card 

            comboBox1.Text = Mc.Currency;
            comboBoxEntityA.Text = Mc.EntityA;
            //radioButtonDR.Checked = Rc.DR;
            //radioButtonCR.Checked = Rc.CR;
            comboBoxEntityB.Text = Mc.EntityB;
            textBox7.Text = Mc.GlAccount;

            comboBoxExternalBank.Text = Mc.VostroBank;
            comboBoxExternalAccount.Text = Mc.VostroAcc;
            textBoxVostroCcy.Text = Mc.VostroCurr;
            textBoxInternalAccount.Text = Mc.InternalAcc;


            if (WMainCateg == "EWB3" || WMainCateg == "EWB4" || WMainCateg == "EWB5" || WMainCateg == "EWB8"
                 || WMainPRX == PRX || WMainCateg == PRX + "2" || WMainCateg == PRX + "3" || WMainCateg == PRX + "5" || WMainCateg == PRX + "1"
                )
            {
                if (WMainCateg == "EWB3" || WMainCateg == "EWB4" || WMainCateg == "EWB5"
                    || WMainPRX == PRX || WMainCateg == PRX + "2" || WMainCateg == PRX + "3" || WMainCateg == PRX + "5" || WMainCateg == PRX + "1"
                    )
                {
                    textBoxCategDescr.Text = comboBoxOrigin.Text + " " + comboBoxTrxType.Text + " - " + comboBoxProducts.Text;
                    textBoxCategDescr.ReadOnly = true;
                }
                if (WMainCateg == "EWB8") // Nostro Vostro 
                {
                    textBoxCategDescr.Text = "NoVo For " + textBoxVostroCcy.Text + " Int: " + textBoxInternalAccount.Text + " And Ext:" + comboBoxExternalAccount.Text;
                    textBoxCategDescr.ReadOnly = true;
                }
            }
            else
            {
                //textBoxCategDescr.Text = Rc.CategoryName;
                textBoxCategDescr.ReadOnly = false;
            }

            //if (WMainCateg == "EWB1" 
            //    || WMainCateg == WPrefix + "1"
            //    )
            //{
            //    textBoxCategDescr.Text = comboBoxOrigin.Text + " - " + " To Target System " + comboBoxTargetSystems.Text;
            //    textBoxCategDescr.ReadOnly = true;
            //}

            if (WMainCategITMX == "ITMX3") // Transfer of Funds 
            {
                textBoxCategDescr.Text = "ITMX Transfer Funds from " + comboBoxEntityA.Text + " To " + comboBoxEntityB.Text;
                textBoxCategDescr.ReadOnly = true;
            }

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
            //if (comboBoxOrigin.Text == "Our Atms")
            // {
            //     Mc.ReadMatchingCategorybyGetsAllOtherBINS(WOperator);
            //     if (Mc.RecordFound == true)
            //     {
            //         MessageBox.Show("Already Exist Category which accomodates the other Banks BINs");
            //         return;
            //     }
            // }


            if (textBox8.TextLength != 6 & WOperator != "ITMX")
            {
                MessageBox.Show("New category must have length of six digits");
                return;
            }
            if (WMode == 5) // Nostro Vostro 
            {
                if (textBox8.Text.Substring(0, 4) != "EWB8")
                {
                    MessageBox.Show("Please enter a value for NEW Category Id. It should start with EWB8 ");
                    return;
                }
            }

            if (
              (textBox8.Text.Substring(0, 3) != "EWB" & textBox8.Text.Substring(0, 3) != PRX 
              & textBox8.Text.Substring(0, 3) != "QAH"
              & textBox8.Text.Substring(0, 3) != "ETI"
              & textBox8.Text.Substring(0, 3) != "IPN"
              )
              & WOperator != "ITMX"
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

            if (comboBoxEntityA.Text == "SelectEntity" || comboBoxEntityB.Text == "SelectEntity")
            {
                MessageBox.Show("Please make selection of entities.");
                return;
            }
            //
            //  Validation
            //


            if (WMainCateg == "EWB3" || WMainCateg == "EWB4" || WMainCateg == "EWB5"
                || WMainPRX == PRX || WMainCateg == PRX + "2" || WMainCateg == PRX + "3" || WMainCateg == PRX + "5"
                )
            {
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


            }
            else
            {
                //Mc.PosStart = 0;
                //Mc.PosEnd = 0;
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

            if (WMainCateg == "EWB3" || WMainCateg == "EWB4" || WMainCateg == "EWB5" || WMainCateg == "EWB8"
                || WMainPRX == PRX || WMainCateg == PRX + "2" || WMainCateg == PRX + "3" || WMainCateg == PRX + "5"
                )
            {
                if (WMainCateg == "EWB3" || WMainCateg == "EWB4" || WMainCateg == "EWB5"
                    || WMainPRX == PRX || WMainCateg == PRX + "2" || WMainCateg == PRX + "3" || WMainCateg == PRX + "5"
                    )
                {
                    textBoxCategDescr.Text = comboBoxOrigin.Text + " " + comboBoxTrxType.Text + " - " + comboBoxProducts.Text;
                    textBoxCategDescr.ReadOnly = true;
                }
                if (WMainCateg == "EWB8") // Nostro Vostro 
                {
                    textBoxCategDescr.Text = "NoVo For " + textBoxVostroCcy.Text + " Int: " + textBoxInternalAccount.Text + " And Ext:" + comboBoxExternalAccount.Text;
                    textBoxCategDescr.ReadOnly = true;
                }
            }
            else
            {
                //textBoxCategDescr.Text = Rc.CategoryName;
                textBoxCategDescr.ReadOnly = false;
            }

            if (WMainCateg == "EWB1"
                || WMainCateg == PRX + "1"
                )
            {
                textBoxCategDescr.Text = comboBoxOrigin.Text + " - " + " To Target System " + comboBoxTargetSystems.Text;
                textBoxCategDescr.ReadOnly = true;
            }

            //if (WMainCategITMX == "ITMX3") // Transfer of Funds 
            //{
            //    textBoxCategDescr.Text = "ITMX Transfer Funds from " + comboBoxEntityA.Text + " To " + comboBoxEntityB.Text;
            //    textBoxCategDescr.ReadOnly = true;
            //}

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

            if (WMainCateg == "EWB1"
                || WMainCateg == PRX + "1"
                //|| comboBoxOrigin.Text == "Our Atms"
                )
            {
                Mc.TargetSystemNm = comboBoxTargetSystems.Text;
                Gp.ReadParametersSpecificNm(WOperator, "705", Mc.TargetSystemNm);
                if (Gp.RecordFound == true)
                {
                    Mc.TargetSystemId = int.Parse(Gp.OccuranceId);
                }
                else
                {
                    MessageBox.Show("Target System Id Not found");
                    return;
                }
                Mc.GetsNotOwnBINS = checkBoxNotOwnBINS.Checked;
            }
            else
            {
                Mc.TargetSystemId = 0;
                Mc.TargetSystemNm = "";
            }

            Mc.RunningJobGroup = comboBoxRunningJob.Text;
            Mc.Product = comboBoxProducts.Text;
            Mc.CostCentre = comboBox5.Text;
            //Mc.Periodicity = comboBoxMatchingScheduleID.Text;
            //Mc.GroupIdInFiles = textBoxBIN.Text;
            //Mc.FieldName = textBox4.Text;
            Mc.Currency = comboBox1.Text;
            Mc.EntityA = comboBoxEntityA.Text;

            //Rc.DR = radioButtonDR.Checked;
            //Rc.CR = radioButtonCR.Checked;

            Mc.EntityB = comboBoxEntityB.Text;
            Mc.GlAccount = textBox7.Text;

            Mc.VostroBank = comboBoxExternalBank.Text;
            Mc.VostroCurr = textBoxVostroCcy.Text;
            Mc.VostroAcc = comboBoxExternalAccount.Text;
            Mc.InternalAcc = textBoxInternalAccount.Text;

            //if (WMainCateg == "EWB5") // Visa settlement 
            //{
            //    Mc.VostroBank = "VISA";
            //    Mc.VostroCurr = "PHP";
            //    Mc.VostroAcc = "VISASettl-PHP";
            //    Mc.InternalAcc = "EWBAuth";
            //}

            Mc.Operator = WOperator;

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
            //textBoxNextMatchingDt.Text = NextLoading.ToString();

            Mc.NextMatchingDt = NextLoading;

            WSeqNo = Mc.InsertMatchingCategory();

            if (WMode == 1 || WMode == 6)
            {
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

            }
            else
            {
                MessageBox.Show("Record with id " + Mc.CategoryId + " Inserted! Assign Owner.");
            }

            textBoxMsgBoard.Text = "New Matching Category inserted.";

            textBox8.Text = "";
            if (Mc.Origin == "Nostro - Vostro")
            {
                Mc.ReadCategoriesToFindPositionOfSeqNo(WOperator, WSeqNo, Mc.Origin);
            }
            else
            {
                Mc.ReadCategoriesToFindPositionOfSeqNo(WOperator, WSeqNo, "");
            }


            ////checkBoxMakeNewVersion.Checked = false;
            //WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form503_Load(this, new EventArgs());

            //dataGridView1.Rows[Mc.PositionInGrid].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, Mc.PositionInGrid));

        }

        // UPDATE CATEGORY 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            Mc.ReadMatchingCategorybySeqNo_Any(WOperator, WSeqNo);

            if (Mc.GetsNotOwnBINS == false & ExistOtherBINCategory == true)
            {
                if (checkBoxNotOwnBINS.Checked == true)
                {
                    MessageBox.Show("Already exists Category that gets the Non our BINS.");
                    return;
                }
            }

            if (Mc.ReconcMaster == true & radioButtonMaster.Checked == true)
            {
                // It was Master and remains Master
                // Do nothing 
            }
            if (Mc.ReconcMaster == true & radioButtonSlave.Checked == true)
            {
                // It was Master but now becomes Slave.
                // Delete Record from ReconcCategories
                Rc.DeleteReconcCategory(WCategoryId);
            }
            if (Mc.ReconcMaster == false & radioButtonSlave.Checked == true)
            {
                // It was Slave and still is Slave 
                // do nothing.
            }

            if (Mc.ReconcMaster == false & radioButtonMaster.Checked == true)
            {
                // It was Slave and becomes a Master
                // Check for Slave relations and ask to free
                string SelectionCriteria = " WHERE MatchingCategoryId ='" + WCategoryId + "'";
                RcMc.ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(SelectionCriteria);
                if (RcMc.RecordFound == true)
                {
                    MessageBox.Show("This matching category belongs to Reconc Category : " + RcMc.ReconcCategoryId + Environment.NewLine
                        + " Clear it and repeat the operation");
                    return;
                }
                // Create Master 
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

                MessageBox.Show(
                                 "A corresponding record was inserted in Reconciliation Categories" + Environment.NewLine
                                 + "Go and Assign Owner for Reconciliation"
                               );
            }

            //if (Mc.ReconcMaster == true & radioButtonMaster.Checked == true)
            //{
            //    Rc.ReadReconcCategorybyCategId(WOperator, Mc.CategoryId);

            //    Rc.CategoryId = Mc.CategoryId;

            //    Rc.CategoryName = Mc.CategoryName;

            //    Rc.Origin = comboBoxOrigin.Text;

            //    Rc.AtmGroup = 0;

            //    Rc.IsOneMatchingCateg = true;

            //    Rc.UpdateReconcCategory(WOperator, Mc.CategoryId); 
            //}


            //
            //  Validation
            //
            //if (int.TryParse(textBox5.Text, out Mc.PosStart))
            //{
            //}
            //else
            //{
            //    MessageBox.Show(textBox5.Text, "Please enter a valid number in field PosStart. ");
            //    return;
            //}

            //if (int.TryParse(textBox6.Text, out Mc.PosEnd))
            //{
            //}
            //else
            //{
            //    MessageBox.Show(textBox6.Text, "Please enter a valid number in field PosEnd. ");
            //    return;
            //}

            //if (textBox1.Text == "")
            //{
            //    MessageBox.Show(textBox6.Text, "Please enter a value for Category Id. ");
            //    return;
            //}

            //if (textBoxCategDescr.Text == "")
            //{
            //    MessageBox.Show(textBox6.Text, "Please enter a value for Category Name. ");
            //    return;
            //}
            Mc.CategoryId = textBox1.Text;


            if (WMainCateg == "EWB3" || WMainCateg == "EWB4" || WMainCateg == "EWB5" || WMainCateg == "EWB8"
                || WMainPRX == PRX || WMainCateg == PRX + "2" || WMainCateg == PRX + "3" || WMainCateg == PRX + "5" || WMainCateg == PRX + "1"
                )
            {
                if (WMainCateg == "EWB3" || WMainCateg == "EWB4" || WMainCateg == "EWB5"
                    || WMainPRX == PRX || WMainCateg == PRX + "2" || WMainCateg == PRX + "3" || WMainCateg == PRX + "5" || WMainCateg == PRX + "1"
                    )
                {
                    textBoxCategDescr.Text = comboBoxOrigin.Text + " " + comboBoxTrxType.Text + " - " + comboBoxProducts.Text;
                    textBoxCategDescr.ReadOnly = true;
                }
                if (WMainCateg == "EWB8") // Nostro Vostro 
                {
                    textBoxCategDescr.Text = "NoVo For " + textBoxVostroCcy.Text + " Int: " + textBoxInternalAccount.Text + " And Ext:" + comboBoxExternalAccount.Text;
                    textBoxCategDescr.ReadOnly = true;
                }
            }
            else
            {
                //textBoxCategDescr.Text = Rc.CategoryName;
                textBoxCategDescr.ReadOnly = false;
            }

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

            Mc.TargetSystemNm = comboBoxTargetSystems.Text;



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
            Mc.Currency = comboBox1.Text;
            Mc.EntityA = comboBoxEntityA.Text;
            //Rc.DR = radioButtonDR.Checked;
            //Rc.CR = radioButtonCR.Checked;

            Mc.EntityB = comboBoxEntityB.Text;
            Mc.GlAccount = textBox7.Text;

            Mc.VostroBank = comboBoxExternalBank.Text;
            Mc.VostroCurr = textBoxVostroCcy.Text;
            Mc.VostroAcc = comboBoxExternalAccount.Text;
            Mc.InternalAcc = textBoxInternalAccount.Text;

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

        // DEFINE GL ACCOUNTS 
        private void button1_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form85 NForm85;

            string WSecLevel = "04";

            NForm85 = new Form85(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm85.FormClosed += NForm85_FormClosed;
            NForm85.ShowDialog(); ;
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
            NForm503_CategOwners = new Form503_CategOwners(WSignedId, WSignRecordNo, WOperator, WCategoryId, "", Mode, "");
            NForm503_CategOwners.FormClosed += NForm503_CategOwners_FormClosed;
            NForm503_CategOwners.ShowDialog();
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
            if (comboBoxOrigin.Text == "Nostro - Vostro"
                || comboBoxOrigin.Text == "Visa Settlement"
                || WMode == 5
                // || comboBoxOrigin.Text == "Master Card"
                )
            {
                panelVOSTRO.Show();
                label10.Text = "Nostro";
                //  panel5.Hide();
                label17.Hide();
                comboBox5.Hide();
                label11.Hide();
                comboBox1.Hide();
                label10.Hide();
                textBox7.Hide();
                checkBoxTWIN.Hide();
                checkBoxIsPOS_type.Hide();

            }
            else
            {
                panelVOSTRO.Hide();
                label10.Text = "GL Account";

                if (comboBoxOrigin.Text == "Our Atms")
                {
                    //comboBoxTargetSystems.Show();
                    //labelTargetSystems.Show();
                    //checkBoxNotOwnBINS.Show();
                }
                else
                {
                    //comboBoxTargetSystems.Hide();
                    //labelTargetSystems.Hide();

                    //checkBoxNotOwnBINS.Show();
                }

                Gp.ReadParametersSpecificNm(WOperator, "709", comboBoxProducts.Text);
                if (Gp.RecordFound == true)
                {
                    BIN = (int)Gp.Amount;
                    if (BIN == 0)
                    {
                        //   panel5.Hide();

                    }
                    else
                    {
                        //textBoxBIN.Text = BIN.ToString();
                        //panel5.Show();

                    }

                }
                label17.Show();
                comboBox5.Show();
                label11.Show();
                comboBox1.Show();
                label10.Show();
                textBox7.Show();
                checkBoxTWIN.Show();
                checkBoxIsPOS_type.Show();
            }

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


            //if (comboBoxProducts.Text == "Debit Card")
            //{
            //    Gp.ReadParametersSpecificId(WOperator, "709", "1", "", ""); // 
            //    BIN = (int)Gp.Amount;
            //    found = true; 
            //}
            //if (comboBoxProducts.Text == "Prepaid Card")
            //{
            //    Gp.ReadParametersSpecificId(WOperator, "709", "2", "", ""); // 
            //    BIN = (int)Gp.Amount;
            //    found = true; 
            //}
            //if (comboBoxProducts.Text == "Gift Card")
            //{
            //    Gp.ReadParametersSpecificId(WOperator, "709", "3", "", ""); // 
            //    BIN = (int)Gp.Amount;
            //    found = true; 
            //}
            //if (comboBoxProducts.Text == "Travel Money Card")
            //{
            //    Gp.ReadParametersSpecificId(WOperator, "709", "4", "", ""); // 
            //    BIN = (int)Gp.Amount;
            //    found = true; 
            //}
            //if (found == true)
            //{
            //    textBoxBIN.Text = BIN.ToString(); 
            //}
            //else
            //{
            //    // 
            //}

        }
        //If Transaction Type changes 
        private void comboBoxTrxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //buttonUpdate.Hide();
            ShowFields();
        }
        // if Entity A changes .. it is useally a Bank 
        private void comboBoxEntityA_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ba.ReadBankByShortName(comboBoxEntityA.Text);
            if (WOperator == "ITMX")
            {
                if (comboBoxEntityA.Text != WOperator & comboBoxEntityA.Text != "SelectEntity")
                {
                    MessageBox.Show("Selecting entity other than ITMX is not allowed. Please select ITMX.");
                    return;
                }
            }
            textBox3.Text = Ba.BankName;
            buttonUpdate.Hide();
            ShowFields();
        }
        // If Entity B Changes 
        private void comboBoxEntityB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ba.ReadBankByShortName(comboBoxEntityB.Text);
            textBox10.Text = Ba.BankName;
            buttonUpdate.Hide();
            ShowFields();
        }
        // Fill in fields 
        public void ShowFields()
        {
            if (WOperator == "ITMX")
            {
                if (comboBoxEntityA.Text != "SelectEntity" & comboBoxEntityB.Text != "SelectEntity")
                {
                    //ITMX-To-DR-BankA
                    //ITMX-To-CR-BankB


                    textBoxCategDescr.Text = "Matching ITMX Records with " + comboBoxEntityB.Text;
                    textBox8.Text = "ITMX TO " + comboBoxEntityB.Text;
                    textBox8.ReadOnly = true;

                    //textBoxCategDescr.Text = "ITMX Transfer Funds from " + comboBoxEntityA.Text + " To " + comboBoxEntityB.Text;
                    //textBox8.Text = "ITMX-FT-" + comboBoxEntityA.Text + "-" + comboBoxEntityB.Text;
                    //textBox8.ReadOnly = true;
                }
                else
                {
                    textBoxCategDescr.Text = "ITMX ... to do what? ... make selection please";

                    textBox8.Text = "";
                }
            }
            else
            {

            }
            //// Load Grid 
            //if (First == false)
            //{
            //    Form503_Load(this, new EventArgs());
            //}

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
            //Acc.ReadAndFindAccountSpecificForNostroVostro(comboBoxExternalAccount.Text, comboBoxExternalBank.Text);
            Acc.ReadAndFindAccountSpecificForNostroVostro(comboBoxExternalAccount.Text, WOperator);
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
        //// Radio Button DR Change 
        //private void radioButtonDR_CheckedChanged(object sender, EventArgs e)
        //{

        //        textBoxCategDescr.Text = "Matching ITMX Records with " + comboBoxEntityB.Text;
        //        textBox8.Text = "ITMX TO " + comboBoxEntityB.Text;
        //        textBox8.ReadOnly = true;
        //}

        //private void radioButtonCR_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (radioButtonDR.Checked == true)
        //    {
        //        textBoxCategDescr.Text = "Matching ITMX Debits with " + comboBoxEntityB.Text;
        //        textBox8.Text = "ITMX-To-DR-" + comboBoxEntityB.Text;
        //        textBox8.ReadOnly = true;
        //    }

        //    if (radioButtonCR.Checked == true)
        //    {
        //        textBoxCategDescr.Text = "Matching ITMX Credits with " + comboBoxEntityB.Text;
        //        textBox8.Text = "ITMX-To-CR-" + comboBoxEntityB.Text;
        //        textBox8.ReadOnly = true;
        //    }

        //}
    }
}
